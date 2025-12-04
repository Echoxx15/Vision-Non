using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace DnnInterfaceNet;

/// <summary>
/// 深度学习模型插件服务器 - 从 Plugins/Dnn 目录加载模型插件
/// </summary>
public sealed class DnnPluginServer
{
    private static readonly Lazy<DnnPluginServer> _instance = new(() => new DnnPluginServer());
    public static DnnPluginServer Instance => _instance.Value;

    /// <summary>
    /// 已加载的插件类型映射（类型全名 -> Type）
    /// </summary>
    private readonly Dictionary<string, Type> _pluginTypes = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// 模型类型名称映射（类型名称 -> Type）如 "语义分割" -> DnnSemanticSegmentation
    /// </summary>
    private readonly Dictionary<string, Type> _modelTypeMap = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// 插件信息映射（类型全名 -> 信息）
    /// </summary>
    private readonly Dictionary<string, DnnPluginInfo> _pluginInfos = new(StringComparer.OrdinalIgnoreCase);

    private DnnPluginServer() { }

    /// <summary>
    /// 从 Plugins/Dnn 目录加载所有深度学习模型插件
    /// </summary>
    public void LoadPlugins()
    {
        var pluginDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Plugins", "Dnn");
        if (!Directory.Exists(pluginDir))
        {
            Console.WriteLine($"深度学习插件目录不存在: {pluginDir}，正在创建...");
            Directory.CreateDirectory(pluginDir);
            return;
        }

        foreach (var dll in Directory.GetFiles(pluginDir, "*.dll"))
        {
            try
            {
                var asm = Assembly.LoadFrom(dll);
                Type[] types;

                try
                {
                    types = asm.GetTypes();
                }
                catch (ReflectionTypeLoadException rtlEx)
                {
                    var msgs = rtlEx.LoaderExceptions?.Where(e => e != null).Select(e => e.Message).ToArray() ?? Array.Empty<string>();
                    Console.WriteLine($"部分类型加载失败: {dll}\n -> {string.Join(" | ", msgs)}");
                    types = rtlEx.Types?.Where(t => t != null).ToArray() ?? Array.Empty<Type>();
                }

                foreach (var type in types)
                {
                    if (type == null || !type.IsClass || type.IsAbstract) continue;

                    // 只收集 IDnnModel 实现
                    if (!typeof(IDnnModel).IsAssignableFrom(type)) continue;

                    // 获取模型类型特性
                    var modelTypeAttr = type.GetCustomAttribute<DnnModelTypeAttribute>();
                    if (modelTypeAttr == null)
                    {
                        Console.WriteLine($"插件 {type.FullName} 未标记 DnnModelTypeAttribute，跳过");
                        continue;
                    }

                    // 注册插件
                    var info = new DnnPluginInfo(
                        type.FullName,
                        asm.GetName().Name,
                        modelTypeAttr.TypeName,
                        modelTypeAttr.Description
                    );

                    _pluginTypes[type.FullName] = type;
                    _pluginInfos[type.FullName] = info;

                    // 按模型类型名注册
                    if (!_modelTypeMap.ContainsKey(modelTypeAttr.TypeName))
                    {
                        _modelTypeMap[modelTypeAttr.TypeName] = type;
                    }

                    Console.WriteLine($"注册深度学习插件: {modelTypeAttr.TypeName} ({type.FullName})");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"加载插件失败: {dll}, {ex.Message}");
            }
        }

        Console.WriteLine($"共加载 {_pluginTypes.Count} 个深度学习模型插件");
    }

    /// <summary>
    /// 获取所有已加载的插件类型
    /// </summary>
    public Dictionary<string, Type> GetLoadedPluginTypes()
    {
        return new Dictionary<string, Type>(_pluginTypes, StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>
    /// 获取所有支持的模型类型名称（用于UI显示）
    /// </summary>
    public List<string> GetAllModelTypeNames()
    {
        return _modelTypeMap.Keys.OrderBy(k => k).ToList();
    }

    /// <summary>
    /// 根据模型类型名称获取插件类型
    /// </summary>
    public Type GetPluginTypeByModelType(string modelTypeName)
    {
        return _modelTypeMap.TryGetValue(modelTypeName, out var type) ? type : null;
    }

    /// <summary>
    /// 获取插件信息
    /// </summary>
    public DnnPluginInfo GetPluginInfo(string typeName)
    {
        return _pluginInfos.TryGetValue(typeName, out var info) ? info : null;
    }

    /// <summary>
    /// 获取所有插件信息（用于UI显示）
    /// </summary>
    public List<DnnPluginInfo> GetAllPluginInfos()
    {
        return _pluginInfos.Values.ToList();
    }
}

/// <summary>
/// 插件信息
/// </summary>
public class DnnPluginInfo
{
    public string TypeName { get; set; }
    public string AssemblyName { get; set; }
    public string ModelTypeName { get; set; }
    public string Description { get; set; }

    public DnnPluginInfo() { }

    public DnnPluginInfo(string typeName, string assemblyName, string modelTypeName, string description)
    {
        TypeName = typeName;
        AssemblyName = assemblyName;
        ModelTypeName = modelTypeName;
        Description = description;
    }
}
