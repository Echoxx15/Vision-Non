using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace HardwareCameraNet;

/// <summary>
/// 触发模式
/// </summary>
public enum TriggerMode
{
    软触发,
    硬触发
}

/// <summary>
/// 相机配置类
/// </summary>
[XmlRoot("CameraConfig")]
public class CameraConfig
{
    /// <summary>
    /// 相机序列号（唯一标识）
    /// </summary>
    [XmlElement("SerialNumber")]
    public string SerialNumber { get; set; }

    /// <summary>
    /// 相机品牌/厂商名称
    /// </summary>
    [XmlElement("Manufacturer")]
    public string Manufacturer { get; set; }

    /// <summary>
    /// 备注说明
    /// </summary>
    [XmlElement("Expain")]
    public string Expain { get; set; } = "";

    /// <summary>
    /// 是否启用
    /// </summary>
    [XmlElement("Enabled")]
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// 插件信息
    /// </summary>
    [XmlElement("PluginInfo")]
    public PluginInfo PluginInfo { get; set; }

    public CameraConfig() { }

    public CameraConfig(string serialNumber, string manufacturer, PluginInfo pluginInfo = null)
    {
        SerialNumber = serialNumber;
        Manufacturer = manufacturer;
        PluginInfo = pluginInfo;
    }
}

/// <summary>
/// 相机配置集合（用于XML序列化）
/// </summary>
[XmlRoot("CameraConfigs")]
public class CameraConfigCollection
{
    [XmlElement("Config")]
    public List<CameraConfig> Configs { get; set; } = new();

    /// <summary>
    /// 查找配置（忽略大小写）
    /// </summary>
    public CameraConfig FindBySerialNumber(string serialNumber)
    {
        if (string.IsNullOrWhiteSpace(serialNumber))
            return null;

        return Configs.Find(c => string.Equals(c.SerialNumber, serialNumber, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// 生成唯一名称
    /// </summary>
    public string GenerateUniqueName(string manufacturer)
    {
        int counter = 1;
        string baseName = manufacturer ?? "Camera";
        string name;
        do
        {
            name = $"{baseName}_{counter}";
            counter++;
        } while (FindBySerialNumber(name) != null);

        return name;
    }
}
