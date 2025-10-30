using System.ComponentModel;

namespace Vision.Solutions.Models;

/// <summary>
/// 深度学习运行时类型
/// 对应 DnnInferenceNet.BassClass.Runtime 枚举
/// </summary>
public enum DLRuntime
{
  /// <summary>
  /// GPU 运行时（默认）- 兼容性最好
  /// </summary>
  [Description("GPU 运行时")]
  GC = 0,

  /// <summary>
  /// OpenVINO 加速 - Intel 硬件优化
  /// </summary>
  [Description("OpenVINO 加速")]
  OpenVINO = 1,

  /// <summary>
  /// TensorRT 硬件加速 - NVIDIA GPU 高性能
  /// </summary>
  [Description("TensorRT 加速")]
  TensorRT = 2
}