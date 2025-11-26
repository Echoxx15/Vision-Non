#nullable enable
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using HardwareCameraNet;

namespace Vision.Manager.PluginServer;

/// <summary>
/// 插件管理器：负责插件加载和类型查找（单例模式）
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

    public void LoadPlugins()
    {
        var pluginDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Plugins", "Camera");
        if (!Directory.Exists(pluginDir)) return;
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
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"插件加载失败: {dll}, {ex.Message}");
            }
        }
    }

    /// <summary>
    /// 获取已加载的插件类型（返回副本，避免外部修改）
    /// </summary>
    public Dictionary<string, Type> GetLoadedPluginTypes()
    {
        return new Dictionary<string, Type>(_pluginTypes, StringComparer.OrdinalIgnoreCase);
    }

    public PluginInfo? GetPluginInfo(string typeName)
    {
        if (_pluginInfos.TryGetValue(typeName, out var info))
            return info;
        return null;
    }

    public Type? GetPluginType(string typeName)
    {
        _pluginTypes.TryGetValue(typeName, out var type);
        return type;
    }
}