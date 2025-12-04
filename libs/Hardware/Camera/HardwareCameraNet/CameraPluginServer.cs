using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;

namespace HardwareCameraNet;

/// <summary>
/// 相机插件服务器：负责加载和管理相机插件（单例模式）
/// </summary>
public sealed class CameraPluginServer
{
    private static readonly Lazy<CameraPluginServer> _instance = new(() => new CameraPluginServer(),
        LazyThreadSafetyMode.ExecutionAndPublication);

    public static CameraPluginServer Instance => _instance.Value;

    // 类型-插件映射（类型全名 -> 插件信息）
    private readonly Dictionary<string, PluginInfo> _pluginInfos = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// 加载的插件类型映射（类型全名 -> 类型）
    /// </summary>
    private readonly Dictionary<string, Type> _pluginTypes = new(StringComparer.OrdinalIgnoreCase);

    private CameraPluginServer()
    {
    }

    /// <summary>
    /// 从指定目录加载相机插件
    /// </summary>
    public void LoadPlugins()
    {
        var pluginDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Plugins", "Camera");
        if (!Directory.Exists(pluginDir))
        {
            Console.WriteLine($"相机插件目录不存在: {pluginDir}");
            return;
        }

        foreach (var dll in Directory.GetFiles(pluginDir, "*.dll"))
        {
            try
            {
                var asm = Assembly.LoadFrom(dll);
                foreach (var type in asm.GetTypes())
                {
                    if (!type.IsClass || type.IsAbstract) continue;
                    // 只收集ICamera实现类
                    if (!typeof(ICamera).IsAssignableFrom(type)) continue;
                    var info = new PluginInfo(type.FullName, asm.GetName().Name);
                    _pluginInfos[type.FullName] = info;
                    _pluginTypes[type.FullName] = type;
                    Console.WriteLine($"加载相机插件: {type.FullName}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"相机插件加载失败: {dll}, {ex.Message}");
            }
        }

        Console.WriteLine($"相机插件加载完成，共 {_pluginTypes.Count} 个插件");
    }

    /// <summary>
    /// 获取已加载的插件类型（返回副本，避免外部修改）
    /// </summary>
    public Dictionary<string, Type> GetLoadedPluginTypes()
    {
        return new Dictionary<string, Type>(_pluginTypes, StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>
    /// 获取插件信息
    /// </summary>
    public PluginInfo GetPluginInfo(string typeName)
    {
        if (_pluginInfos.TryGetValue(typeName, out var info))
            return info;
        return null;
    }

    /// <summary>
    /// 根据厂商名称获取插件信息
    /// </summary>
    public PluginInfo GetPluginInfoByManufacturer(string manufacturerName)
    {
        if (string.IsNullOrEmpty(manufacturerName))
            return null;

        foreach (var kvp in _pluginTypes)
        {
            var type = kvp.Value;
            var manufacturerAttr = type.GetCustomAttribute<CameraManufacturerAttribute>();
            if (manufacturerAttr != null &&
                string.Equals(manufacturerAttr.ManufacturerName, manufacturerName, StringComparison.OrdinalIgnoreCase))
            {
                return _pluginInfos.TryGetValue(kvp.Key, out var info) ? info : null;
            }
        }
        return null;
    }

    /// <summary>
    /// 获取插件类型
    /// </summary>
    public Type GetPluginType(string typeName)
    {
        _pluginTypes.TryGetValue(typeName, out var type);
        return type;
    }
}

/// <summary>
/// 插件信息
/// </summary>
public class PluginInfo
{
    public string TypeFullName { get; set; }
    public string AssemblyName { get; set; }

    public PluginInfo() { }

    public PluginInfo(string typeFullName, string assemblyName)
    {
        TypeFullName = typeFullName;
        AssemblyName = assemblyName;
    }
}
