using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;

namespace HardwareCommNet;

/// <summary>
/// 可选：支持在运行时修改设备名称的接口（插件可实现）。
/// </summary>
public interface IRenameableComm
{
    void SetName(string name);
}

/// <summary>
/// 通讯管理器（单例）：负责通讯设备的创建、管理和配置保存
/// </summary>
public sealed class CommunicationFactory
{
    private static readonly Lazy<CommunicationFactory> _instance = new(() => new CommunicationFactory());

    public static CommunicationFactory Instance => _instance.Value;

    // 存储：厂商名称 -> 插件类型
    private readonly Dictionary<string, Type> _manufacturerPluginMap = new(StringComparer.OrdinalIgnoreCase);

    // 已创建的通讯设备实例（名称 -> 实例）
    private readonly ConcurrentDictionary<string, IComm> _commDevices = new(StringComparer.OrdinalIgnoreCase);

    // 配置文件路径
    private readonly string _configFilePath;

    private int _deviceCounter;

    #region 事件

    /// <summary>
    /// 设备创建事件（设备名称、设备类型、设备实例）
    /// </summary>
    public event Action<string, string, IComm> DeviceCreated;

    /// <summary>
    /// 设备移除事件（设备名称、设备类型）
    /// </summary>
    public event Action<string, string> DeviceRemoved;

    /// <summary>
    /// 设备重命名事件（旧名称、新名称、设备类型）
    /// </summary>
    public event Action<string, string, string> DeviceRenamed;

    #endregion

    private CommunicationFactory()
    {
        _configFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Configs", "CommConfigs.xml");

        // 确保配置目录存在
        var dir = Path.GetDirectoryName(_configFilePath);
        if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
    }

    /// <summary>
    /// 初始化：从插件服务器构建厂商映射
    /// </summary>
    public void Initialize()
    {
        BuildManufacturerMap(CommPluginServer.Instance.GetLoadedPluginTypes().Values);
    }

    /// <summary>
    /// 构建厂商映射：从插件类型中提取厂商信息
    /// </summary>
    private void BuildManufacturerMap(IEnumerable<Type> commTypes)
    {
        _manufacturerPluginMap.Clear();

        foreach (var type in commTypes)
        {
            // 提取厂商特性（标记插件所属类型）
            var manufacturerAttr = type.GetCustomAttribute<CommManufacturerAttribute>();
            if (manufacturerAttr == null)
            {
                Console.WriteLine($"插件{type.FullName}未标记CommManufacturerAttribute，跳过");
                continue;
            }

            string manufacturerName = manufacturerAttr.ManufacturerName;
            if (_manufacturerPluginMap.ContainsKey(manufacturerName))
            {
                Console.WriteLine($"通讯类型{manufacturerName}已存在，跳过重复插件{type.FullName}");
                continue;
            }

            // 添加到厂商映射
            _manufacturerPluginMap.Add(manufacturerName, type);
            Console.WriteLine($"注册通讯类型：{manufacturerName}（插件类型：{type.FullName}）");
        }
    }

    /// <summary>
    /// 创建通讯设备
    /// </summary>
    /// <param name="manufacturerName">通讯类型名称（如"ModbusTcp"）</param>
    /// <param name="customName">自定义设备名称（可选）</param>
    /// <returns>创建的设备实例</returns>
    public IComm CreateCommDevice(string manufacturerName, string customName = null)
    {
        if (string.IsNullOrEmpty(manufacturerName))
        {
            Console.WriteLine("通讯类型名称不能为空");
            return null;
        }

        // 检查类型是否支持
        if (!_manufacturerPluginMap.TryGetValue(manufacturerName, out var type))
        {
            Console.WriteLine($"未支持的通讯类型：{manufacturerName}");
            return null;
        }

        // 生成设备名称
        _deviceCounter++;
        string deviceName = string.IsNullOrEmpty(customName)
            ? $"{manufacturerName}_{_deviceCounter}"
            : customName;

        // 如果名称已存在，添加后缀
        int suffix = 1;
        string finalName = deviceName;
        while (_commDevices.ContainsKey(finalName))
        {
            finalName = $"{deviceName}_{suffix}";
            suffix++;
        }

        try
        {
            var device = (IComm)Activator.CreateInstance(type, finalName);

            if (_commDevices.TryAdd(finalName, device))
            {
                Console.WriteLine($"创建通讯设备：{finalName}");

                // 触发设备创建事件
                try
                {
                    DeviceCreated?.Invoke(finalName, manufacturerName, device);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"触发设备创建事件失败：{ex.Message}");
                }

                // 自动保存配置
                try
                {
                    SaveConfigs();
                }
                catch
                {
                }

                return device;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"创建通讯设备失败：{ex.Message}");
        }

        return null;
    }

    /// <summary>
    /// 重命名设备（不释放实例，仅更新键；若设备实现 IRenameableComm 则同步内部名称）。
    /// </summary>
    public bool RenameDevice(string oldName, string newName)
    {
        if (string.IsNullOrWhiteSpace(oldName) || string.IsNullOrWhiteSpace(newName))
            return false;
        if (string.Equals(oldName, newName, StringComparison.OrdinalIgnoreCase))
            return true;
        if (_commDevices.ContainsKey(newName))
            return false;

        if (_commDevices.TryRemove(oldName, out var dev))
        {
            try
            {
                if (dev is IRenameableComm r)
                    r.SetName(newName);
            }
            catch
            {
            }

            var result = _commDevices.TryAdd(newName, dev);

            // 触发设备重命名事件
            if (result)
            {
                try
                {
                    var deviceType = GetDeviceType(dev);
                    DeviceRenamed?.Invoke(oldName, newName, deviceType);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"触发设备重命名事件失败：{ex.Message}");
                }

                // 自动保存配置
                try
                {
                    SaveConfigs();
                }
                catch
                {
                }
            }

            return result;
        }

        return false;
    }

    /// <summary>
    /// 移除指定名称的通讯设备
    /// </summary>
    public bool RemoveDevice(string name)
    {
        if (_commDevices.TryGetValue(name, out var device))
        {
            var deviceType = GetDeviceType(device);

            if (_commDevices.TryRemove(name, out _))
            {
                try
                {
                    device?.Dispose();
                }
                catch
                {
                }

                // 触发设备移除事件
                try
                {
                    DeviceRemoved?.Invoke(name, deviceType);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"触发设备移除事件失败：{ex.Message}");
                }

                // 自动保存配置
                try
                {
                    SaveConfigs();
                }
                catch
                {
                }

                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// 保存所有设备配置到 XML 文件
    /// </summary>
    public void SaveConfigs()
    {
        try
        {
            var collection = new CommConfigCollection();

            foreach (var kvp in _commDevices)
            {
                var device = kvp.Value;
                CommConfig config;

                // 如果设备实现了 IConfigurableComm，使用其提供的配置
                if (device is IConfigurableComm configurableDevice)
                {
                    config = configurableDevice.GetConfig();

                    // 若表为空也要输出，但仍保存设备基础信息
                    var hasTable = config.TableConfig != null && ((config.TableConfig.Inputs?.Count ?? 0) > 0 || (config.TableConfig.Outputs?.Count ?? 0) > 0);
                    Console.WriteLine(hasTable
                        ? $"[{config.Name}] 保存通讯表: 输入{config.TableConfig.Inputs?.Count ?? 0}项, 输出{config.TableConfig.Outputs?.Count ?? 0}项"
                        : $"[{config.Name}] 通讯表为空(不会丢失，只是当前为空)");
                }
                else
                {
                    // 否则创建基本配置
                    config = new CommConfig
                    {
                        Name = kvp.Key,
                        Type = GetDeviceType(device)
                    };
                    Console.WriteLine($"[{config.Name}] 设备未实现IConfigurableComm");
                }

                collection.Configs.Add(config);
            }

            var serializer = new XmlSerializer(typeof(CommConfigCollection));
            using (var stream = File.Create(_configFilePath))
            {
                serializer.Serialize(stream, collection);
            }

            Console.WriteLine($"✅ 通讯配置已保存到: {_configFilePath}");
            Console.WriteLine($"✅ 保存设备数: {collection.Configs.Count}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ 保存通讯配置失败：{ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// 从 XML 文件加载配置并创建设备
    /// </summary>
    public void LoadConfigs()
    {
        if (!File.Exists(_configFilePath))
        {
            Console.WriteLine("通讯配置文件不存在，跳过加载");
            return;
        }

        try
        {
            var serializer = new XmlSerializer(typeof(CommConfigCollection));
            CommConfigCollection collection;

            using (var stream = File.OpenRead(_configFilePath))
            {
                collection = (CommConfigCollection)serializer.Deserialize(stream);
            }

            if (collection?.Configs == null || collection.Configs.Count == 0)
            {
                Console.WriteLine("通讯配置文件为空");
                return;
            }

            int successCount = 0;
            int failCount = 0;

            foreach (var config in collection.Configs)
            {
                try
                {
                    if (string.IsNullOrEmpty(config.Name) || string.IsNullOrEmpty(config.Type))
                    {
                        failCount++;
                        continue;
                    }

                    // 检查类型是否支持
                    if (!_manufacturerPluginMap.TryGetValue(config.Type, out var type))
                    {
                        Console.WriteLine($"不支持的设备类型：{config.Type}，跳过设备：{config.Name}");
                        failCount++;
                        continue;
                    }

                    // 创建设备实例
                    var device = (IComm)Activator.CreateInstance(type, config.Name);

                    // ✅ 先应用配置（包括通讯表和连接参数），再连接
                    if (device is IConfigurableComm configurableDevice)
                    {
                        Console.WriteLine($"[{config.Name}] 应用配置...");
                        configurableDevice.ApplyConfig(config);

                        // 调试：输出加载的通讯表信息
                        if (config.TableConfig != null)
                        {
                            Console.WriteLine($"[{config.Name}] 加载通讯表: 输入{config.TableConfig.Inputs?.Count ?? 0}项, 输出{config.TableConfig.Outputs?.Count ?? 0}项");
                        }
                    }

                    // 添加到设备字典（不触发自动保存）
                    if (_commDevices.TryAdd(config.Name, device))
                    {
                        successCount++;
                        Console.WriteLine($"✅ 加载通讯设备：{config.Name} ({config.Type})");
                        //连接动作统一在此处执行，避免在 ApplyConfig 内部二次连接
                        try { device.Connect(); } catch (Exception connEx) { Console.WriteLine($"[{config.Name}]连接异常: {connEx.Message}"); }
                        try { DeviceCreated?.Invoke(config.Name, config.Type, device); } catch (Exception ex) { Console.WriteLine($"触发设备创建事件失败：{ex.Message}"); }
                    }
                    else
                    {
                        Console.WriteLine($"❌ 添加设备失败（名称冲突）：{config.Name}");
                        failCount++;
                        device?.Dispose();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ 加载设备失败 [{config?.Name}]：{ex.Message}");
                    failCount++;
                }
            }

            Console.WriteLine($"===== 通讯配置加载完成：成功 {successCount} 个，失败 {failCount} 个 =====");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ 加载通讯配置文件失败：{ex.Message}");
            Console.WriteLine($"   堆栈: {ex.StackTrace}");
        }
    }

    /// <summary>
    /// 获取设备的类型名称（通过反射查找 CommManufacturerAttribute）
    /// </summary>
    private string GetDeviceType(IComm device)
    {
        if (device == null) return string.Empty;

        var type = device.GetType();
        var attr = type.GetCustomAttribute<CommManufacturerAttribute>();
        return attr?.ManufacturerName ?? type.Name;
    }

    /// <summary>
    /// 获取所有支持的通讯类型
    /// </summary>
    public List<string> GetAllManufacturers()
    {
        return _manufacturerPluginMap.Keys.OrderBy(k => k).ToList();
    }

    /// <summary>
    /// 获取指定名称的通讯设备
    /// </summary>
    public IComm GetDevice(string name)
    {
        return _commDevices.TryGetValue(name, out var device) ? device : null;
    }

    /// <summary>
    /// 获取所有通讯设备
    /// </summary>
    public IEnumerable<IComm> GetAllDevices()
    {
        return _commDevices.Values.ToList();
    }

    /// <summary>
    /// 清空所有设备
    /// </summary>
    public void Clear()
    {
        foreach (var device in _commDevices.Values)
        {
            try
            {
                device?.Dispose();
            }
            catch
            {
            }
        }

        _commDevices.Clear();
        _deviceCounter = 0;
    }

    /// <summary>
    /// 获取设备数量
    /// </summary>
    public int DeviceCount => _commDevices.Count;
}