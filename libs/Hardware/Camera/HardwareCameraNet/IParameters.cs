using System.Collections.Generic;

namespace HardwareCameraNet;

/// <summary>
/// 相机参数操作接口，统一参数获取/设置的方法
/// </summary>
public interface IParameters
{
    /// <summary>
    /// 曝光
    /// </summary>
    double ExposureTime { get; set; }
    double MaxExposureTime { get; }
    /// <summary>
    /// 增益
    /// </summary>
    double Gain { get; set; }
    double MaxGain { get; }
    /// <summary>
    /// 图像宽度
    /// </summary>
    int Width { get; set; }
    /// <summary>
    /// 图像高度
    /// </summary>
    int Height { get; set; }

    /// <summary>
    /// 当前触发源
    /// </summary>
    string TriggerSoure { get;set; }
    /// <summary>
    /// 相机触发源枚举项
    /// </summary>
    List<string> TriggerSoures { get; }
}