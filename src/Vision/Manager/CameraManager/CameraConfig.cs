using System.Xml.Serialization;
using Vision.Manager.PluginServer;

namespace Vision.Manager.CameraManager;

// 触发模式（供站位配置等使用）
public enum TriggerMode { 软触发, 硬触发 }

/// <summary>
/// 相机配置类（仅用于设备管理的标识与插件元信息，不承载站位参数）
/// </summary>
[XmlRoot("CameraConfig")]
public class CameraConfig
{
    [XmlElement("SerialNumber")]
    public string SerialNumber { get; set; }

    [XmlElement("Manufacturer")]
    public string Manufacturer { get; set; }

    [XmlElement("Expain")]
    public string Expain { get; set; } = "";

    [XmlElement("PluginInfo")]
    public PluginInfo? PluginInfo { get; set; }

    public CameraConfig() { }

    public CameraConfig(string serialNumber, string manufacturer, PluginInfo? pluginInfo)
    {
        SerialNumber = serialNumber;
        Manufacturer = manufacturer;
        PluginInfo = pluginInfo;
    }
}
