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
