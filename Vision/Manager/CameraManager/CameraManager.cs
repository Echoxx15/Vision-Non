#nullable enable
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Xml.Serialization;
using HardwareCameraNet;
using Logger;
using Vision.Manager.PluginServer;

namespace Vision.Manager.CameraManager;

public sealed class CameraManager
{
    /// <summary>
    /// 相机状态变更事件，string-设备序列号，string-设备名，bool-连接状态
    /// </summary>
    public event Action<string, string, bool>? DeviceStateChanged; // deviceId,
    /// <summary>
    /// 相机配置移除事件，string-设备序列号
    /// </summary>
    public event Action<string>? DeviceRemoved;
    private readonly ConcurrentDictionary<string, (string expain, bool isConnected)> _deviceStates = new();

    // 单例实例（线程安全）
    private static readonly Lazy<CameraManager> _instance = new(() => new CameraManager(),
        LazyThreadSafetyMode.ExecutionAndPublication);
    public static CameraManager Instance => _instance.Value;

    // 配置文件路径
    private readonly string _configFilePath;

    // 存储：厂商 -> (枚举方法委托, 插件类型)
    private readonly Dictionary<string, (Func<List<string>> EnumerateFunc, Type PluginType)> _manufacturerPluginMap = new(StringComparer.OrdinalIgnoreCase);

    // 已添加的相机配置（用户配置）
    private readonly ConcurrentDictionary<string, CameraConfig> _cameraConfigs = new(StringComparer.OrdinalIgnoreCase);

    // 相机实例缓存（序列号 -> 相机实例）
    private readonly ConcurrentDictionary<string, ICamera?> _cameraInstances = new(StringComparer.OrdinalIgnoreCase);
    // 已连接的相机实例缓存（防止重复连接)
    private readonly ConcurrentDictionary<string, ICamera> _temCameraInstances = new(StringComparer.OrdinalIgnoreCase);

    private CameraManager()
    {
        _configFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Configs", "CameraConfigs.xml");

        if (!Directory.Exists(Path.GetDirectoryName(_configFilePath)!))
            Directory.CreateDirectory(Path.GetDirectoryName(_configFilePath)!);
        BuildManufacturerMap(CameraPluginServer.Instance.GetLoadedPluginTypes().Values);
        LoadCameraConfigs();
    }

    #region 插件管理
    /// <summary>
    /// 构建厂商映射：从插件类型中提取厂商信息和枚举方法
    /// </summary>
    private void BuildManufacturerMap(IEnumerable<Type> cameraTypes)
    {
        _manufacturerPluginMap.Clear();

        foreach (var type in cameraTypes)
        {
            // 提取厂商特性（标记插件所属厂商）
            var manufacturerAttr = type.GetCustomAttribute<CameraManufacturerAttribute>();
            if (manufacturerAttr == null)
            {
                Console.WriteLine($"插件{type.FullName}未标记CameraManufacturerAttribute，跳过");
                continue;
            }

            string manufacturerName = manufacturerAttr.ManufacturerName;
            if (_manufacturerPluginMap.ContainsKey(manufacturerName))
            {
                Console.WriteLine($"厂商{manufacturerName}已存在，跳过重复插件{type.FullName}");
                continue;
            }

            // 查找静态枚举方法（无参数，返回List<string>序列号列表）
            MethodInfo? enumerateMethod = type.GetMethod(
                name: "EnumerateDevices",
                bindingAttr: BindingFlags.Public | BindingFlags.Static,
                binder: null,  // 补充binder参数（可设为null）
                types: Type.EmptyTypes,
                modifiers: null  // 补充modifiers参数（可设为null）
            );

            if (enumerateMethod == null || enumerateMethod.ReturnType != typeof(List<string>))
            {
                Console.WriteLine($"插件{type.FullName}缺少有效的EnumerateDevices静态方法，跳过");
                continue;
            }

            // 封装枚举委托（统一调用方式）
            List<string> EnumerateFunc()
            {
                try
                {
                    return (List<string>)enumerateMethod.Invoke(null, null) ?? [];
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"枚举{manufacturerName}设备失败：{ex.InnerException?.Message}");
                    return [];
                }
            }

            // 添加到厂商映射
            _manufacturerPluginMap.Add(manufacturerName, (EnumerateFunc, type));
            Console.WriteLine($"注册厂商：{manufacturerName}（插件类型：{type.FullName}）");
        }
    }

    #endregion

    #region 配置管理

    /// <summary>
    /// 加载本地相机配置
    /// </summary>
    private void LoadCameraConfigs()
    {
        if (!File.Exists(_configFilePath))
            return;

        try
        {
            var serializer = new XmlSerializer(typeof(List<CameraConfig>));
            using var stream = File.OpenRead(_configFilePath);
            var configs = (List<CameraConfig>)serializer.Deserialize(stream)!;

            foreach (var config in configs)
            {
                _cameraConfigs[config.SerialNumber] = config;
                var cam = CreateCamera(config.Manufacturer, config.SerialNumber);
                if (cam == null)
                {
                    // 连接、掉线、备注修改时调用
                    UpdateDeviceState(config.SerialNumber, config.Expain, false);
                    continue;
                }
                var ret = cam.Open();
                if (ret != 0)
                {
                    LogHelper.Warn($"打开[{config.Manufacturer}]相机[{config.SerialNumber}]失败,错误码:{ret}");
                }
                _cameraInstances.TryAdd(config.SerialNumber, cam);
                _temCameraInstances.TryAdd(config.SerialNumber, cam);
                cam.DisConnetEvent += DisConnectedEventHandler;
                // 连接、掉线、备注修改时调用
                UpdateDeviceState(config.SerialNumber, config.Expain, cam.IsConnected);
            }
            Console.WriteLine($"加载相机配置：共{_cameraConfigs.Count}台");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"加载配置失败：{ex.Message}");
        }
    }

    /// <summary>
    /// 保存相机配置到本地
    /// </summary>
    private void SaveCameraConfigs()
    {
        try
        {
            var serializer = new XmlSerializer(typeof(List<CameraConfig>));
            using var stream = File.Create(_configFilePath);
            serializer.Serialize(stream, _cameraConfigs.Values.ToList());
        }
        catch (Exception ex)
        {
            Console.WriteLine($"保存配置失败：{ex.Message}");
        }
    }
    #endregion

    #region 设备状态事件


    private void UpdateDeviceState(string sn, string expain, bool isConnected = true)
    {
        _deviceStates[sn] = (expain, isConnected);
        DeviceStateChanged?.Invoke(sn, expain, isConnected);
    }

    public IReadOnlyDictionary<string, (string expain, bool isConnected)> GetAllDeviceStates()
    {
        return _deviceStates;
    }


    #endregion

    private void DisConnectedEventHandler(object sender, bool disconnect)
    {
        var sn = ((ICamera)sender).SN;
        if (disconnect)
            LogHelper.Warn($"序列号[{sn}]相机掉线了");
        else
            LogHelper.Warn($"序列号[{sn}]相机重连成功");

        UpdateDeviceState(sn, "", !disconnect);
    }


    //-----------对外API-----------
    /// <summary>
    /// 添加/更新相机配置
    /// </summary>
    public bool AddOrUpdateCameraConfig(CameraConfig config)
    {
        if (string.IsNullOrEmpty(config.SerialNumber) || string.IsNullOrEmpty(config.Manufacturer))
            throw new ArgumentException("序列号和品牌不能为空");
            

        if (!_manufacturerPluginMap.ContainsKey(config.Manufacturer))
            throw new ArgumentException($"不支持的相机品牌：{config.Manufacturer}");

        _cameraConfigs.AddOrUpdate(
            config.SerialNumber,                // 键
            config,                             // 如果不存在则添加的值
            (_, _) => config           // 如果已存在则更新的值
        );
        _cameraInstances.TryAdd(config.SerialNumber, CreateCamera(config.Manufacturer, config.SerialNumber));
        SaveCameraConfigs();
        var connect = _cameraInstances.TryGetValue(config.SerialNumber, out var instance) && instance is
        {
            IsConnected: true
        };

        // 连接、掉线、备注修改时调用
        UpdateDeviceState(config.SerialNumber, config.Expain, connect);
        return true;
    }

    /// <summary>
    /// 删除相机配置
    /// </summary>
    public bool RemoveCameraConfig(string serialNumber)
    {
        var removed = _cameraConfigs.TryRemove(serialNumber, out _);
        // 同时关闭并移除实例
        if (_cameraInstances.TryRemove(serialNumber, out var inst) && inst != null)
        {
            try { inst.DisConnetEvent -= DisConnectedEventHandler; inst.Close(); } catch { }
        }
        if (_temCameraInstances.TryRemove(serialNumber, out var cached))
        {
            try { cached.DisConnetEvent -= DisConnectedEventHandler; cached.Close(); } catch { }
        }
        // 清理设备状态并通知界面移除
        _deviceStates.TryRemove(serialNumber, out _);
        if (removed)
        {
            SaveCameraConfigs();
            try { DeviceRemoved?.Invoke(serialNumber); } catch { }
            return true;
        }
        return false;
    }

    /// <summary>
    /// 获取所有相机配置
    /// </summary>
    public List<CameraConfig> GetAllCameraConfigs()
    {
        return _cameraConfigs.Values.ToList();
    }


    /// <summary>
    /// 枚举指定厂商的设备（返回序列号列表）
    /// </summary>
    public List<string> EnumerateDevices(string manufacturerName)
    {
        if (string.IsNullOrEmpty(manufacturerName))
        {
            Console.WriteLine("厂商名称不能为空");
            return [];
        }

        lock (this)
        {
            if (_manufacturerPluginMap.TryGetValue(manufacturerName, out var item))
            {
                var devices = item.EnumerateFunc.Invoke();
                Console.WriteLine($"枚举{manufacturerName}设备：{devices.Count}台");
                return devices;
            }

            Console.WriteLine($"未支持的厂商：{manufacturerName}");
            return [];
        }
    }

    /// <summary>
    /// 根据厂商和序列号创建相机实例
    /// </summary>
    public ICamera? CreateCamera(string manufacturerName, string serialNumber)
    {
        if (string.IsNullOrEmpty(manufacturerName) || string.IsNullOrEmpty(serialNumber))
        {
            Console.WriteLine("厂商名称和序列号不能为空");
            return null;
        }

        // 先检查缓存：存在则直接返回（避免重复创建）
        if (_temCameraInstances.TryGetValue(serialNumber, out var cachedCamera))
        {
            Console.WriteLine($"复用缓存的相机实例：{serialNumber}");
            return cachedCamera;
        }


        // 1. 检查厂商是否支持
        if (!_manufacturerPluginMap.TryGetValue(manufacturerName, out var item))
        {
            Console.WriteLine($"未支持的厂商：{manufacturerName}");
            return null;
        }

        //2.检查设备是否已枚举（可选：避免创建不存在的设备）
        var enumeratedDevices = item.EnumerateFunc.Invoke();
        if (!enumeratedDevices.Contains(serialNumber, StringComparer.OrdinalIgnoreCase))
        {
            LogHelper.Warn($"相机枚举失败,{manufacturerName}无序列号{serialNumber}的设备");
            return null;
        }

        if (_temCameraInstances.TryGetValue(serialNumber, out var cam))
        {
            return cam;
        }

        // 3. 创建实例（用插件类型完全限定名）
        var newCamera = (ICamera)Activator.CreateInstance(item.PluginType, serialNumber)!;
        // 添加到线程安全缓存
        _temCameraInstances.TryAdd(serialNumber, newCamera);
        Console.WriteLine($"创建新相机实例并缓存：{serialNumber}");
        return newCamera;
    }

    /// <summary>
    /// 获取所有相机实例
    /// </summary>
    public List<ICamera?> GetAllCameras()
    {
        return _cameraInstances.Values.ToList();
    }

    /// <summary>
    /// 获取所有相机实例
    /// </summary>
    public ICamera? GetCameras(string sn)
    {
        return _cameraInstances[sn];
    }


    /// <summary>
    /// 获取所有支持的相机品牌
    /// </summary>
    public List<string> GetAllManufacturers()
    {
        return _manufacturerPluginMap.Keys.OrderBy(k => k).ToList();
    }

    public void UnInitialize()
    {
        foreach (var item in _cameraInstances.Values)
        {
            try
            {
                if (item == null) continue;
                item.DisConnetEvent -= DisConnectedEventHandler;
                item.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        foreach (var item in _temCameraInstances.Values)
        {
            try
            {
                item.DisConnetEvent -= DisConnectedEventHandler;
                item.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}