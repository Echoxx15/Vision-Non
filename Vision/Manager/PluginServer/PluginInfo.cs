namespace Vision.Manager.PluginServer;

/// <summary>
/// 插件信息结构体，存储插件类型信息
/// </summary>
public struct PluginInfo(string typeName, string assemblyName)
{
    public string TypeName { get; set; } = typeName; // 插件类型全名（命名空间+类名）
    public string AssemblyName { get; set; } = assemblyName; // 插件程序集名称
}

