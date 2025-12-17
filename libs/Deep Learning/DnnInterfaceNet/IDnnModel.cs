using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace DnnInterfaceNet;

/// <summary>
/// 深度学习模型接口 - 所有模型插件必须实现此接口
/// </summary>
public interface IDnnModel : IDisposable
{
    /// <summary>
    /// 模型唯一名称（用户命名，如"语义分割模型1"）
    /// </summary>
    string Name { get; }

    /// <summary>
    /// 模型是否已加载
    /// </summary>
    bool IsLoaded { get; }

    /// <summary>
    /// 模型文件路径
    /// </summary>
    string ModelPath { get; }

    /// <summary>
    /// 获取配置界面控件
    /// </summary>
    UserControl GetConfigControl();

    /// <summary>
    /// 加载模型
    /// </summary>
    /// <param name="modelPath">模型文件夹路径</param>
    /// <param name="deviceType">设备类型</param>
    /// <param name="runtime">运行时类型</param>
    /// <returns>是否加载成功</returns>
    bool Load(string modelPath, DnnDeviceType deviceType, DnnRuntime runtime);

    /// <summary>
    /// 卸载模型
    /// </summary>
    void Unload();

    /// <summary>
    /// 推理（返回结果字典）
    /// 图像类型取决于具体实现（如 HalconDotNet.HObject）
    /// </summary>
    /// <param name="image">输入图像对象</param>
    /// <returns>推理结果</returns>
    DnnResult Infer(object image);
}

/// <summary>
/// 可选：支持在运行时修改名称的接口
/// </summary>
public interface IRenameableDnnModel
{
    void SetName(string name);
}

/// <summary>
/// 可选：支持配置序列化的接口
/// </summary>
public interface IConfigurableDnnModel
{
    /// <summary>
    /// 获取当前配置
    /// </summary>
    DnnModelConfig GetConfig();

    /// <summary>
    /// 应用配置
    /// </summary>
    void ApplyConfig(DnnModelConfig config);
}

/// <summary>
/// 坐标格式枚举
/// </summary>
public enum CoordinateFormat
{
    /// <summary>
    /// 常规XY格式：第一个数组为X坐标（列），第二个数组为Y坐标（行）
    /// </summary>
    XY,
    /// <summary>
    /// Halcon行列格式：第一个数组为Row（行/Y），第二个数组为Column（列/X）
    /// </summary>
    RowColumn
}

/// <summary>
/// 可选：支持批量推理的接口（适用于大图分块推理场景）
/// </summary>
public interface IBatchInferableDnnModel
{
    /// <summary>
    /// 批量推理（分块拼接模式）- 适用于大图片分块推理场景
    /// </summary>
    /// <param name="image">原始大图像对象（如 HalconDotNet.HObject）</param>
    /// <param name="coordArray1">第一个坐标数组（XY格式时为X/列坐标，RowColumn格式时为Row/行坐标）</param>
    /// <param name="coordArray2">第二个坐标数组（XY格式时为Y/行坐标，RowColumn格式时为Column/列坐标）</param>
    /// <param name="cropWidth">裁剪宽度</param>
    /// <param name="cropHeight">裁剪高度</param>
    /// <param name="format">坐标格式：XY格式或Halcon行列格式（默认XY）</param>
    /// <param name="maskRegions">可选的遮罩区域列表（这些区域会被填充为背景）</param>
    /// <returns>推理结果，ResultImage 为拼接后的完整分割图</returns>
    DnnResult InferBatch(object image, int[] coordArray1, int[] coordArray2, 
        int cropWidth, int cropHeight, 
        CoordinateFormat format = CoordinateFormat.XY,
        List<object> maskRegions = null);

    /// <summary>
    /// 使用网格自动分块推理大图
    /// </summary>
    /// <param name="image">原始大图像对象</param>
    /// <param name="tileWidth">每块宽度</param>
    /// <param name="tileHeight">每块高度</param>
    /// <param name="overlapX">X方向重叠像素（默认0）</param>
    /// <param name="overlapY">Y方向重叠像素（默认0）</param>
    /// <param name="maskRegions">可选的遮罩区域列表</param>
    /// <returns>推理结果</returns>
    DnnResult InferWithTiling(object image, int tileWidth, int tileHeight, 
        int overlapX = 0, int overlapY = 0, List<object> maskRegions = null);
}

/// <summary>
/// 可选：支持模型优化导出的接口
/// </summary>
public interface IOptimizableDnnModel
{
    /// <summary>
    /// 获取可用的优化设备列表
    /// </summary>
    /// <returns>设备名称列表（如 "NVIDIA GeForce RTX 4060 with TensorRT"）</returns>
    List<string> GetAvailableOptimizeDevices();

    /// <summary>
    /// 优化并导出模型
    /// </summary>
    /// <param name="deviceName">设备名称</param>
    /// <param name="precision">优化精度</param>
    /// <param name="batchSize">批次大小</param>
    /// <param name="progress">进度回调（0-100）</param>
    /// <returns>是否成功</returns>
    bool OptimizeAndExport(string deviceName, DnnOptimizePrecision precision, int batchSize, Action<int, string> progress = null);

    /// <summary>
    /// 检查优化模型是否存在
    /// </summary>
    /// <param name="runtime">运行时类型</param>
    /// <returns>是否存在</returns>
    bool HasOptimizedModel(DnnRuntime runtime);
}

/// <summary>
/// 可选：支持枚举可用推理设备/运行时的接口
/// </summary>
public interface IInferenceDeviceProvider
{
    /// <summary>
    /// 获取所有可用的推理设备（包含运行时和设备类型）
    /// </summary>
    List<DnnInferenceDeviceInfo> GetAvailableInferenceDevices();
}

/// <summary>
/// 推理设备信息
/// </summary>
public class DnnInferenceDeviceInfo
{
    public string Name { get; set; }
    public DnnRuntime Runtime { get; set; }
    public DnnDeviceType DeviceType { get; set; }

    public override string ToString() => $"{Name} ({Runtime}, {DeviceType})";
}

/// <summary>
/// 设备类型枚举
/// </summary>
public enum DnnDeviceType
{
    CPU,
    GPU
}

/// <summary>
/// 运行时枚举
/// </summary>
public enum DnnRuntime
{
    /// <summary>
    /// GPU/CPU 通用
    /// </summary>
    GC,
    /// <summary>
    /// OpenVINO 加速
    /// </summary>
    OpenVINO,
    /// <summary>
    /// TensorRT 加速
    /// </summary>
    TensorRT
}

/// <summary>
/// 模型优化精度枚举
/// </summary>
public enum DnnOptimizePrecision
{
    /// <summary>
    /// 浮点32位（最高精度）
    /// </summary>
    FP32,
    /// <summary>
    /// 浮点16位（平衡）
    /// </summary>
    FP16,
    /// <summary>
    /// 整型8位（最快速度，精度较低）
    /// </summary>
    INT8
}

/// <summary>
/// 深度学习推理结果
/// </summary>
public class DnnResult
{
    /// <summary>
    /// 是否成功
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// 错误消息
    /// </summary>
    public string ErrorMessage { get; set; }

    /// <summary>
    /// 结果图像对象（如分割掩码，具体类型取决于实现）
    /// </summary>
    public object ResultImage { get; set; }

    /// <summary>
    /// 结果区域对象（具体类型取决于实现）
    /// </summary>
    public object ResultRegion { get; set; }

    /// <summary>
    /// 结果文本（如OCR识别结果）
    /// </summary>
    public string[] ResultTexts { get; set; }

    /// <summary>
    /// 置信度数组
    /// </summary>
    public double[] Confidences { get; set; }

    /// <summary>
    /// 类别ID数组（语义分割）
    /// </summary>
    public int[] ClassIDs { get; set; }

    /// <summary>
    /// 类别名称数组
    /// </summary>
    public string[] ClassNames { get; set; }

    /// <summary>
    /// 原始结果字典（扩展用，具体类型取决于实现）
    /// </summary>
    public object RawResult { get; set; }

    /// <summary>
    /// 扩展属性字典
    /// </summary>
    public Dictionary<string, object> ExtendedProperties { get; set; } = new Dictionary<string, object>();
}
