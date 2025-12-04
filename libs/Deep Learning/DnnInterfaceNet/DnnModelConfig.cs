using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace DnnInterfaceNet;

/// <summary>
/// 深度学习模型配置（用于序列化保存）
/// </summary>
[Serializable]
public class DnnModelConfig
{
    /// <summary>
    /// 模型名称（用户定义）
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 模型类型（插件类型名称，如"语义分割"）
    /// </summary>
    public string Type { get; set; }

    /// <summary>
    /// 模型文件夹路径
    /// </summary>
    public string ModelPath { get; set; }

    /// <summary>
    /// 设备类型
    /// </summary>
    public DnnDeviceType DeviceType { get; set; } = DnnDeviceType.GPU;

    /// <summary>
    /// 运行时类型
    /// </summary>
    public DnnRuntime Runtime { get; set; } = DnnRuntime.GC;

    /// <summary>
    /// 是否在启动时自动加载
    /// </summary>
    public bool LoadOnStartup { get; set; } = false;

    /// <summary>
    /// 扩展参数（插件特有配置）
    /// </summary>
    public List<ConfigParameter> ExtendedParams { get; set; } = new();
}

/// <summary>
/// 配置参数（键值对）
/// </summary>
[Serializable]
public class ConfigParameter
{
    public string Key { get; set; }
    public string Value { get; set; }
    public string TypeName { get; set; }

    public ConfigParameter() { }

    public ConfigParameter(string key, string value, string typeName = null)
    {
        Key = key;
        Value = value;
        TypeName = typeName ?? typeof(string).FullName;
    }
}

/// <summary>
/// 模型配置集合（用于XML序列化）
/// </summary>
[Serializable]
[XmlRoot("DnnModelConfigs")]
public class DnnModelConfigCollection
{
    [XmlArray("Models")]
    [XmlArrayItem("Model")]
    public List<DnnModelConfig> Configs { get; set; } = new();
}
