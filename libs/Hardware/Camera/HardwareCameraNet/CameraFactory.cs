using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Xml.Serialization;

namespace HardwareCameraNet;

/// <summary>
/// 相机管理工厂（单例）：负责相机设备的创建、管理和配置保存
/// 统一在DLL中管理，主程序只负责访问
/// </summary>
public sealed class CameraFactory
{
    private static readonly Lazy<CameraFactory> _instance = new(() => new CameraFactory(),
        LazyThreadSafetyMode.ExecutionAndPublication);

    public static CameraFactory Instance => _instance.Value;

    // 存储：厂商名称 -> (枚举方法委托, 插件类型)
    private readonly Dictionary<string, (Func<List<string>> EnumerateFunc, Type PluginType)> _manufacturerPluginMap 
        = new(StringComparer.OrdinalIgnoreCase);

    // 相机配置集合
    private CameraConfigCollection _configs = new();
    private readonly object _configLock = new();

    // 已创建的相机实例（序列号 -> 相机实例）
    private readonly ConcurrentDictionary<string, ICamera> _cameraInstances = new(StringComparer.OrdinalIgnoreCase);

    // 设备状态缓存
    private readonly ConcurrentDictionary<string, (string expain, bool isConnected)> _deviceStates 
        = new(StringComparer.OrdinalIgnoreCase);

    // 配置文件路径
    private readonly string _configFilePath;

    #region 事件

    /// <summary>
    /// 设备创建事件（序列号、厂商、设备实例）
    /// </summary>
    public event Action<string, string, ICamera> DeviceCreated;

    /// <summary>
    /// 设备移除事件（序列号、厂商）
    /// </summary>
    public event Action<string, string> DeviceRemoved;

    /// <summary>
    /// 设备状态变更事件（序列号、备注、连接状态）
    /// </summary>
    public event Action<string, string, bool> DeviceStateChanged;

    #endregion

    private CameraFactory()
    {
        _configFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Configs", "CameraConfigs.xml");

        // 确保配置目录存在
        var dir = Path.GetDirectoryName(_configFilePath);
        if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
    }

    /// <summary>
    /// 初始化：从插件服务器构建厂商映射并加载配置
    /// </summary>
    public void Initialize()
    {
        BuildManufacturerMap(CameraPluginServer.Instance.GetLoadedPluginTypes().Values);
        LoadConfigs();
    }

    /// <summary>
    /// 构建厂商映射：从插件类型中提取厂商信息和枚举方法
    /// </summary>
    private void BuildManufacturerMap(IEnumerable<Type> cameraTypes)
    {
        _manufacturerPluginMap.Clear();

        foreach (var type in cameraTypes)
        {
            // 提取厂商特性
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

            // 查找静态枚举方法
            MethodInfo enumerateMethod = type.GetMethod(
                name: "EnumerateDevices",
                bindingAttr: BindingFlags.Public | BindingFlags.Static,
                binder: null,
                types: Type.EmptyTypes,
                modifiers: null
            );

            if (enumerateMethod == null || enumerateMethod.ReturnType != typeof(List<string>))
            {
                Console.WriteLine($"插件{type.FullName}缺少有效的EnumerateDevices静态方法，跳过");
                continue;
            }

            // 封装枚举委托
            List<string> EnumerateFunc()
            {
                try
                {
                    return (List<string>)enumerateMethod.Invoke(null, null) ?? new List<string>();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"枚举{manufacturerName}设备失败：{ex.InnerException?.Message ?? ex.Message}");
                    return new List<string>();
                }
            }

            _manufacturerPluginMap.Add(manufacturerName, (EnumerateFunc, type));
            Console.WriteLine($"注册相机厂商：{manufacturerName}（插件类型：{type.FullName}）");
        }
    }

    #region 配置管理

    /// <summary>
    /// 获取配置集合
    /// </summary>
    public CameraConfigCollection Configs
    {
        get
        {
            lock (_configLock)
            {
                return _configs;
            }
        }
    }

    /// <summary>
    /// 添加或更新相机配置
    /// </summary>
    public CameraConfig AddOrUpdateConfig(CameraConfig config)
    {
        if (string.IsNullOrEmpty(config?.SerialNumber) || string.IsNullOrEmpty(config.Manufacturer))
            throw new ArgumentException("序列号和品牌不能为空");

        if (!_manufacturerPluginMap.ContainsKey(config.Manufacturer))
            throw new ArgumentException($"不支持的相机品牌：{config.Manufacturer}");

        lock (_configLock)
        {
            var existing = _configs.FindBySerialNumber(config.SerialNumber);
            if (existing != null)
            {
                // 更新现有配置
                existing.Manufacturer = config.Manufacturer;
                existing.Expain = config.Expain;
                existing.Enabled = config.Enabled;
                existing.PluginInfo = config.PluginInfo;
            }
            else
            {
                // 添加新配置
                _configs.Configs.Add(config);
            }
        }

        // 创建或获取相机实例
        var camera = GetOrCreateCamera(config.Manufacturer, config.SerialNumber);
        
        // 保存配置
        SaveConfigs();

        // 更新设备状态
        UpdateDeviceState(config.SerialNumber, config.Expain, camera?.IsConnected ?? false);

        // 触发事件
        if (camera != null && !_cameraInstances.ContainsKey(config.SerialNumber))
        {
            _cameraInstances.TryAdd(config.SerialNumber, camera);
            try { DeviceCreated?.Invoke(config.SerialNumber, config.Manufacturer, camera); }
            catch (Exception ex) { Console.WriteLine($"触发设备创建事件失败：{ex.Message}"); }
        }

        return config;
    }

    /// <summary>
    /// 移除相机配置
    /// </summary>
    public bool RemoveConfig(string serialNumber)
    {
        if (string.IsNullOrWhiteSpace(serialNumber))
            return false;

        string manufacturer = null;
        lock (_configLock)
        {
            var config = _configs.FindBySerialNumber(serialNumber);
            if (config == null)
                return false;

            manufacturer = config.Manufacturer;
            _configs.Configs.Remove(config);
        }

        // 关闭并移除实例
        if (_cameraInstances.TryRemove(serialNumber, out var camera) && camera != null)
        {
            try
            {
                camera.DisConnetEvent -= OnCameraDisconnected;
                camera.Close();
            }
            catch { }
        }

        // 清理设备状态
        _deviceStates.TryRemove(serialNumber, out _);

        // 保存配置
        SaveConfigs();

        // 触发事件
        try { DeviceRemoved?.Invoke(serialNumber, manufacturer); }
        catch (Exception ex) { Console.WriteLine($"触发设备移除事件失败：{ex.Message}"); }

        Console.WriteLine($"移除相机配置成功: {serialNumber}");
        return true;
    }

    /// <summary>
    /// 获取配置
    /// </summary>
    public CameraConfig GetConfig(string serialNumber)
    {
        lock (_configLock)
        {
            return _configs.FindBySerialNumber(serialNumber);
        }
    }

    /// <summary>
    /// 获取所有相机配置
    /// </summary>
    public List<CameraConfig> GetAllConfigs()
    {
        lock (_configLock)
        {
            return _configs.Configs.ToList();
        }
    }

    /// <summary>
    /// 保存配置到文件
    /// </summary>
    public void SaveConfigs()
    {
        try
        {
            lock (_configLock)
            {
                var serializer = new XmlSerializer(typeof(CameraConfigCollection));
                using (var stream = File.Create(_configFilePath))
                {
                    serializer.Serialize(stream, _configs);
                }
            }
            Console.WriteLine($"相机配置已保存到: {_configFilePath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"保存相机配置失败：{ex.Message}");
        }
    }

    /// <summary>
    /// 从文件加载配置并创建设备
    /// </summary>
    public void LoadConfigs()
    {
        if (!File.Exists(_configFilePath))
        {
            Console.WriteLine("相机配置文件不存在，跳过加载");
            return;
        }

        try
        {
            var serializer = new XmlSerializer(typeof(CameraConfigCollection));
            using (var stream = File.OpenRead(_configFilePath))
            {
                _configs = (CameraConfigCollection)serializer.Deserialize(stream) ?? new CameraConfigCollection();
            }

            Console.WriteLine($"加载相机配置：共 {_configs.Configs.Count} 台");

            // 为每个配置创建相机实例并尝试连接
            foreach (var config in _configs.Configs)
            {
                try
                {
                    if (!config.Enabled)
                    {
                        UpdateDeviceState(config.SerialNumber, config.Expain, false);
                        continue;
                    }

                    var camera = CreateCamera(config.Manufacturer, config.SerialNumber);
                    if (camera == null)
                    {
                        UpdateDeviceState(config.SerialNumber, config.Expain, false);
                        continue;
                    }

                    // 尝试打开相机
                    var ret = camera.Open();
                    if (ret != 0)
                    {
                        Console.WriteLine($"打开[{config.Manufacturer}]相机[{config.SerialNumber}]失败, 错误码:{ret}");
                    }

                    _cameraInstances.TryAdd(config.SerialNumber, camera);
                    camera.DisConnetEvent += OnCameraDisconnected;
                    UpdateDeviceState(config.SerialNumber, config.Expain, camera.IsConnected);

                    // 触发事件
                    try { DeviceCreated?.Invoke(config.SerialNumber, config.Manufacturer, camera); }
                    catch (Exception ex) { Console.WriteLine($"触发设备创建事件失败：{ex.Message}"); }

                    Console.WriteLine($"✅ 加载相机：{config.SerialNumber} ({config.Manufacturer}) - 连接状态: {camera.IsConnected}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ 加载相机失败 [{config.SerialNumber}]：{ex.Message}");
                    UpdateDeviceState(config.SerialNumber, config.Expain, false);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"加载相机配置失败：{ex.Message}");
        }
    }

    #endregion

    #region 设备管理

    /// <summary>
    /// 枚举指定厂商的设备（返回序列号列表）
    /// </summary>
    public List<string> EnumerateDevices(string manufacturerName)
    {
        if (string.IsNullOrEmpty(manufacturerName))
        {
            Console.WriteLine("厂商名称不能为空");
            return new List<string>();
        }

        if (_manufacturerPluginMap.TryGetValue(manufacturerName, out var item))
        {
            var devices = item.EnumerateFunc.Invoke();
            Console.WriteLine($"枚举{manufacturerName}设备：{devices.Count}台");
            return devices;
        }

        Console.WriteLine($"未支持的厂商：{manufacturerName}");
        return new List<string>();
    }

    /// <summary>
    /// 创建相机实例（不检查缓存）
    /// </summary>
    private ICamera CreateCamera(string manufacturerName, string serialNumber)
    {
        if (string.IsNullOrEmpty(manufacturerName) || string.IsNullOrEmpty(serialNumber))
        {
            Console.WriteLine("厂商名称和序列号不能为空");
            return null;
        }

        // 先检查缓存
        if (_cameraInstances.TryGetValue(serialNumber, out var cachedCamera))
        {
            return cachedCamera;
        }

        // 检查厂商是否支持
        if (!_manufacturerPluginMap.TryGetValue(manufacturerName, out var item))
        {
            Console.WriteLine($"未支持的厂商：{manufacturerName}");
            return null;
        }

        // 检查设备是否已枚举
        var enumeratedDevices = item.EnumerateFunc.Invoke();
        if (!enumeratedDevices.Contains(serialNumber, StringComparer.OrdinalIgnoreCase))
        {
            Console.WriteLine($"相机枚举失败, {manufacturerName}无序列号{serialNumber}的设备");
            return null;
        }

        try
        {
            // 创建实例
            var newCamera = (ICamera)Activator.CreateInstance(item.PluginType, serialNumber);
            Console.WriteLine($"创建相机实例：{serialNumber}");
            return newCamera;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"创建相机实例失败：{ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// 获取或创建相机实例（不重新枚举设备）
    /// </summary>
    public ICamera GetOrCreateCamera(string manufacturerName, string serialNumber)
    {
        if (string.IsNullOrEmpty(manufacturerName) || string.IsNullOrEmpty(serialNumber))
        {
            Console.WriteLine("厂商名称和序列号不能为空");
            return null;
        }

        // 先检查缓存
        if (_cameraInstances.TryGetValue(serialNumber, out var cachedCamera))
        {
            return cachedCamera;
        }

        // 检查厂商是否支持
        if (!_manufacturerPluginMap.TryGetValue(manufacturerName, out var item))
        {
            Console.WriteLine($"未支持的厂商：{manufacturerName}");
            return null;
        }

        try
        {
            // 直接创建实例
            var newCamera = (ICamera)Activator.CreateInstance(item.PluginType, serialNumber);
            _cameraInstances.TryAdd(serialNumber, newCamera);
            newCamera.DisConnetEvent += OnCameraDisconnected;
            Console.WriteLine($"创建相机实例并缓存：{serialNumber}");
            return newCamera;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"创建相机实例失败：{ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// 获取相机实例
    /// </summary>
    public ICamera GetCamera(string serialNumber)
    {
        if (string.IsNullOrWhiteSpace(serialNumber))
            return null;

        _cameraInstances.TryGetValue(serialNumber, out var camera);
        return camera;
    }

    /// <summary>
    /// 获取所有相机实例
    /// </summary>
    public List<ICamera> GetAllCameras()
    {
        return _cameraInstances.Values.ToList();
    }

    /// <summary>
    /// 获取所有支持的相机品牌
    /// </summary>
    public List<string> GetAllManufacturers()
    {
        return _manufacturerPluginMap.Keys.OrderBy(k => k).ToList();
    }

    /// <summary>
    /// 连接相机
    /// </summary>
    public bool ConnectCamera(string serialNumber)
    {
        var camera = GetCamera(serialNumber);
        if (camera == null)
        {
            var config = GetConfig(serialNumber);
            if (config == null) return false;
            camera = GetOrCreateCamera(config.Manufacturer, serialNumber);
        }

        if (camera == null) return false;
        if (camera.IsConnected) return true;

        var ret = camera.Open();
        var success = ret == 0;
        
        var config2 = GetConfig(serialNumber);
        UpdateDeviceState(serialNumber, config2?.Expain ?? "", success);
        
        return success;
    }

    /// <summary>
    /// 断开相机
    /// </summary>
    public void DisconnectCamera(string serialNumber)
    {
        var camera = GetCamera(serialNumber);
        if (camera == null) return;

        try
        {
            camera.Close();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"关闭相机失败：{ex.Message}");
        }

        var config = GetConfig(serialNumber);
        UpdateDeviceState(serialNumber, config?.Expain ?? "", false);
    }

    #endregion

    #region 设备状态

    /// <summary>
    /// 更新设备状态
    /// </summary>
    private void UpdateDeviceState(string sn, string expain, bool isConnected)
    {
        _deviceStates[sn] = (expain, isConnected);
        try { DeviceStateChanged?.Invoke(sn, expain, isConnected); }
        catch (Exception ex) { Console.WriteLine($"触发设备状态变更事件失败：{ex.Message}"); }
    }

    /// <summary>
    /// 外部通知设备状态变化
    /// </summary>
    public void NotifyDeviceStateChanged(string sn, string expain, bool isConnected)
    {
        UpdateDeviceState(sn, expain, isConnected);
    }

    /// <summary>
    /// 获取所有设备状态
    /// </summary>
    public IReadOnlyDictionary<string, (string expain, bool isConnected)> GetAllDeviceStates()
    {
        return _deviceStates;
    }

    /// <summary>
    /// 相机掉线事件处理
    /// </summary>
    private void OnCameraDisconnected(object sender, bool disconnect)
    {
        var camera = sender as ICamera;
        if (camera == null) return;

        var sn = camera.SN;
        if (disconnect)
            Console.WriteLine($"序列号[{sn}]相机掉线了");
        else
            Console.WriteLine($"序列号[{sn}]相机重连成功");

        var config = GetConfig(sn);
        UpdateDeviceState(sn, config?.Expain ?? "", !disconnect);
    }

    #endregion

    /// <summary>
    /// 清理资源
    /// </summary>
    public void UnInitialize()
    {
        var allCameras = _cameraInstances.Values.ToList();

        foreach (var cam in allCameras)
        {
            try
            {
                cam.DisConnetEvent -= OnCameraDisconnected;
                cam.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine($"关闭相机失败：{e.Message}");
            }
        }

        _cameraInstances.Clear();
        Console.WriteLine("相机资源清理完成");
    }
}
