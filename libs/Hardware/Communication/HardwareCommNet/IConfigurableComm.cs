using System.Windows.Forms;

namespace HardwareCommNet;

/// <summary>
/// 可配置的通讯设备接口（可选实现）
/// 用于获取对应的设备配置参数
/// </summary>
public interface IConfigurableComm
{
    /// <summary>
    /// 获取当前设备的配置参数
    /// </summary>
    CommConfig GetConfig();

    /// <summary>
    /// 应用配置参数到设备
    /// </summary>
    void ApplyConfig(CommConfig config);
}
