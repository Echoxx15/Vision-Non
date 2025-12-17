using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using DnnInterfaceNet;
using HalconDotNet;

namespace DnnOCRNet;

/// <summary>
/// 深度OCR模型插件 - 实现 IDnnModel 接口
/// </summary>
[DnnModelType("深度OCR", "基于深度学习的OCR模型，支持文字检测和识别")]
public class DnnDeepOCR : IDnnModel, IRenameableDnnModel, IConfigurableDnnModel, IOptimizableDnnModel, IInferenceDeviceProvider
{
    #region 私有字段

    private string _name;
    private readonly object _lock = new();

    // Halcon 句柄
    private HTuple _modelHandle;
    private HTuple _deviceHandle;
    private HTuple _deviceHandles;

    // 配置参数
    private DnnDeviceType _deviceType = DnnDeviceType.GPU;
    private DnnRuntime _runtime = DnnRuntime.GC;
    private bool _loadOnStartup;
    
    // OCR 特有参数
    private int _recognitionBatchSize = 1;
    private bool _detectionTiling = true;
    private int _detectionTilingOverlap = 60;

    #endregion

    #region 属性

    public string Name => _name;
    public bool IsLoaded { get; private set; }
    public string ModelPath { get; private set; }

    /// <summary>
    /// 是否在启动时自动加载
    /// </summary>
    public bool LoadOnStartup
    {
        get => _loadOnStartup;
        set => _loadOnStartup = value;
    }

    /// <summary>
    /// 模型句柄（供外部高级使用）
    /// </summary>
    public HTuple ModelHandle => _modelHandle;

    /// <summary>
    /// 识别批量大小
    /// </summary>
    public int RecognitionBatchSize
    {
        get => _recognitionBatchSize;
        set
        {
            _recognitionBatchSize = value;
            if (IsLoaded && _modelHandle != null)
            {
                try { HOperatorSet.SetDeepOcrParam(_modelHandle, "recognition_batch_size", value); }
                catch { }
            }
        }
    }

    /// <summary>
    /// 是否启用检测分块
    /// </summary>
    public bool DetectionTiling
    {
        get => _detectionTiling;
        set
        {
            _detectionTiling = value;
            if (IsLoaded && _modelHandle != null)
            {
                try { HOperatorSet.SetDeepOcrParam(_modelHandle, "detection_tiling", value ? "true" : "false"); }
                catch { }
            }
        }
    }

    /// <summary>
    /// 检测分块重叠像素
    /// </summary>
    public int DetectionTilingOverlap
    {
        get => _detectionTilingOverlap;
        set
        {
            _detectionTilingOverlap = value;
            if (IsLoaded && _modelHandle != null)
            {
                try { HOperatorSet.SetDeepOcrParam(_modelHandle, "detection_tiling_overlap", value); }
                catch { }
            }
        }
    }

    #endregion

    #region 构造函数

    public DnnDeepOCR(string name)
    {
        _name = name ?? throw new ArgumentNullException(nameof(name));
    }

    #endregion

    #region IDnnModel 实现

    public UserControl GetConfigControl()
    {
        return new OCRConfigControl(this);
    }

    public bool Load(string modelPath, DnnDeviceType deviceType, DnnRuntime runtime)
    {
        lock (_lock)
        {
            try
            {
                Unload();

                ModelPath = modelPath;
                _deviceType = deviceType;
                _runtime = runtime;

                // 查询可用设备
                if (!QueryAvailableDevices(deviceType, runtime))
                {
                    Console.WriteLine($"[{_name}] 查询设备失败");
                    return false;
                }

                // 创建 Deep OCR 模型
                HOperatorSet.CreateDeepOcr(new HTuple(), new HTuple(), out _modelHandle);

                // 设置识别模型路径
                string recognitionModelPath = Path.Combine(modelPath, "recognition.hdl").Replace("\\", "/");
                if (!File.Exists(recognitionModelPath.Replace("/", "\\")))
                {
                    Console.WriteLine($"[{_name}] 识别模型文件不存在: {recognitionModelPath}");
                    Unload();
                    return false;
                }

                HOperatorSet.SetDeepOcrParam(_modelHandle, "recognition_model", recognitionModelPath);
                HOperatorSet.SetDeepOcrParam(_modelHandle, "recognition_batch_size", _recognitionBatchSize);
                HOperatorSet.SetDeepOcrParam(_modelHandle, "detection_tiling", _detectionTiling ? "true" : "false");
                HOperatorSet.SetDeepOcrParam(_modelHandle, "detection_tiling_overlap", _detectionTilingOverlap);

                // 设置设备
                SelectDevice(_deviceHandles, deviceType.ToString());

                IsLoaded = true;
                Console.WriteLine($"[{_name}] OCR模型加载成功: {Path.GetFileName(modelPath)}");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[{_name}] 加载OCR模型失败: {ex.Message}");
                Unload();
                return false;
            }
        }
    }

    public void Unload()
    {
        lock (_lock)
        {
            try
            {
                _modelHandle?.Dispose();
                _deviceHandle?.Dispose();
                _deviceHandles?.Dispose();
            }
            catch { }
            finally
            {
                _modelHandle = new HTuple();
                _deviceHandle = new HTuple();
                _deviceHandles = new HTuple();
                IsLoaded = false;
            }
        }
    }

    public DnnResult Infer(object imageObj)
    {
        lock (_lock)
        {
            var result = new DnnResult();

            if (!IsLoaded)
            {
                result.Success = false;
                result.ErrorMessage = "模型未加载";
                return result;
            }

            if (!(imageObj is HObject image))
            {
                result.Success = false;
                result.ErrorMessage = "输入图像必须是 HalconDotNet.HObject 类型";
                return result;
            }

            try
            {
                // 执行 OCR
                HOperatorSet.ApplyDeepOcr(image, _modelHandle, "auto", out var deepOcrResult);

                // 获取识别结果
                HOperatorSet.GetDictTuple(deepOcrResult, "words", out var words);
                HOperatorSet.GetDictTuple(words, "word", out var word);
                
                result.Success = true;
                result.ResultTexts = word.SArr ?? Array.Empty<string>();

                // 尝试获取置信度
                try
                {
                    HOperatorSet.GetDictTuple(words, "confidence", out var confidence);
                    result.Confidences = confidence.DArr;
                }
                catch { }

                deepOcrResult?.Dispose();
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.ErrorMessage = ex.Message;
            }

            return result;
        }
    }

    public void Dispose()
    {
        Unload();
    }

    #endregion

    #region IRenameableDnnModel 实现

    public void SetName(string name)
    {
        _name = name;
    }

    #endregion

    #region IConfigurableDnnModel 实现

    public DnnModelConfig GetConfig()
    {
        var config = new DnnModelConfig
        {
            Name = _name,
            Type = "深度OCR",
            ModelPath = ModelPath,
            DeviceType = _deviceType,
            Runtime = _runtime,
            LoadOnStartup = _loadOnStartup
        };

        // 添加 OCR 特有参数
        config.ExtendedParams.Add(new ConfigParameter("RecognitionBatchSize", _recognitionBatchSize.ToString(), typeof(int).FullName));
        config.ExtendedParams.Add(new ConfigParameter("DetectionTiling", _detectionTiling.ToString(), typeof(bool).FullName));
        config.ExtendedParams.Add(new ConfigParameter("DetectionTilingOverlap", _detectionTilingOverlap.ToString(), typeof(int).FullName));

        return config;
    }

    public void ApplyConfig(DnnModelConfig config)
    {
        if (config == null) return;

        _deviceType = config.DeviceType;
        _runtime = config.Runtime;
        _loadOnStartup = config.LoadOnStartup;

        if (!string.IsNullOrEmpty(config.ModelPath))
        {
            ModelPath = config.ModelPath;
        }

        // 应用 OCR 特有参数
        foreach (var param in config.ExtendedParams)
        {
            switch (param.Key)
            {
                case "RecognitionBatchSize":
                    if (int.TryParse(param.Value, out var batchSize))
                        _recognitionBatchSize = batchSize;
                    break;
                case "DetectionTiling":
                    if (bool.TryParse(param.Value, out var tiling))
                        _detectionTiling = tiling;
                    break;
                case "DetectionTilingOverlap":
                    if (int.TryParse(param.Value, out var overlap))
                        _detectionTilingOverlap = overlap;
                    break;
            }
        }
    }

    #endregion

    #region IOptimizableDnnModel 实现

    /// <summary>
    /// 获取可用的优化设备列表
    /// </summary>
    public List<string> GetAvailableOptimizeDevices()
    {
        var devices = new List<string>();
        
        try
        {
            // 查询 TensorRT 设备
            try
            {
                HOperatorSet.QueryAvailableDlDevices("ai_accelerator_interface", "tensorrt", out var tensorrtDevices);
                for (int i = 0; i < tensorrtDevices.Length; i++)
                {
                    HOperatorSet.GetDlDeviceParam(tensorrtDevices[i], "name", out var deviceName);
                    devices.Add($"{deviceName.S} with TensorRT");
                }
            }
            catch { }

            // 查询 OpenVINO 设备
            try
            {
                HOperatorSet.QueryAvailableDlDevices("ai_accelerator_interface", "openvino", out var openvinoDevices);
                for (int i = 0; i < openvinoDevices.Length; i++)
                {
                    HOperatorSet.GetDlDeviceParam(openvinoDevices[i], "name", out var deviceName);
                    var name = $"{deviceName.S} with OpenVINO";
                    if (!devices.Contains(name))
                        devices.Add(name);
                }
            }
            catch { }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"查询优化设备失败: {ex.Message}");
        }

        return devices;
    }

    /// <summary>
    /// 优化并导出模型
    /// </summary>
    public bool OptimizeAndExport(string deviceName, DnnOptimizePrecision precision, int batchSize, Action<int, string> progress = null)
    {
        if (!IsLoaded || string.IsNullOrEmpty(ModelPath))
        {
            progress?.Invoke(0, "模型未加载");
            return false;
        }

        try
        {
            progress?.Invoke(5, "正在查询设备...");
            
            // 解析设备类型
            bool isTensorRT = deviceName.Contains("TensorRT");
            bool isOpenVINO = deviceName.Contains("OpenVINO");
            
            if (!isTensorRT && !isOpenVINO)
            {
                progress?.Invoke(0, "不支持的设备类型");
                return false;
            }

            // 查询对应设备
            HTuple deviceHandles;
            string accelerator = isTensorRT ? "tensorrt" : "openvino";
            
            HOperatorSet.QueryAvailableDlDevices("ai_accelerator_interface", accelerator, out deviceHandles);
            
            if (deviceHandles.Length == 0)
            {
                progress?.Invoke(0, "未找到可用设备");
                return false;
            }

            // 选择第一个匹配的设备
            HTuple selectedDevice = null;
            for (int i = 0; i < deviceHandles.Length; i++)
            {
                HOperatorSet.GetDlDeviceParam(deviceHandles[i], "name", out var name);
                if (deviceName.Contains(name.S))
                {
                    selectedDevice = deviceHandles[i];
                    break;
                }
            }

            if (selectedDevice == null)
                selectedDevice = deviceHandles[0];

            progress?.Invoke(10, "正在准备优化...");

            // 读取原始识别模型
            string recognitionModelPath = Path.Combine(ModelPath, "recognition.hdl").Replace("\\", "/");
            if (!File.Exists(recognitionModelPath.Replace("/", "\\")))
            {
                progress?.Invoke(0, "识别模型文件不存在");
                return false;
            }

            progress?.Invoke(15, "正在读取模型...");
            HOperatorSet.ReadDlModel(recognitionModelPath, out var dlModelHandle);

            // 设置批次大小
            HOperatorSet.SetDlModelParam(dlModelHandle, "batch_size", batchSize);

            progress?.Invoke(25, "正在优化模型（这可能需要几分钟）...");

            // 转换精度字符串
            string precisionStr = precision switch
            {
                DnnOptimizePrecision.FP16 => "float16",
                DnnOptimizePrecision.INT8 => "int8",
                _ => "float32"
            };

            // 优化模型 - Halcon 24.11 正确调用方式
            // 首先获取优化参数
            HTuple optimizeForInferenceParams;
            try
            {
                HOperatorSet.GetDlDeviceParam(selectedDevice, "optimize_for_inference_params", out optimizeForInferenceParams);
                Console.WriteLine($"[{_name}] 获取优化参数成功");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[{_name}] 获取优化参数失败: {ex.Message}，使用默认参数");
                optimizeForInferenceParams = new HTuple();
            }
            
            HTuple optimizedModelHandle;
            HTuple conversionReport;
            
            try
            {
                Console.WriteLine($"[{_name}] 调用 OptimizeDlModelForInference (Halcon 24.11)");
                
                // Halcon 24.11 签名:
                // OptimizeDlModelForInference(DLModelHandle, DLDevice, Precision, GenParamName, OptimizeForInferenceParams, 
                //                            out DLModelHandleOptimized, out ConversionReport)
                HOperatorSet.OptimizeDlModelForInference(
                    dlModelHandle,              // DLModelHandle - 原始模型
                    selectedDevice,             // DLDevice - 设备句柄
                    precisionStr,               // Precision - 精度
                    new HTuple(),               // GenParamName - 空的额外参数
                    optimizeForInferenceParams, // OptimizeForInferenceParams - 优化参数
                    out optimizedModelHandle,   // DLModelHandleOptimized - 优化后的模型
                    out conversionReport);      // ConversionReport - 转换报告
                    
                Console.WriteLine($"[{_name}] 优化成功！");
            }
            catch (Exception ex)
            {
                progress?.Invoke(0, $"Halcon优化失败: {ex.Message}");
                Console.WriteLine($"[{_name}] OptimizeDlModelForInference 失败: {ex.Message}");
                Console.WriteLine($"[{_name}] 异常详情: {ex}");
                return false;
            }

            progress?.Invoke(80, "正在保存优化模型...");

            // 生成输出文件名
            string suffix = isTensorRT ? "_tensorrt" : "_openvino";
            string optimizedPath = Path.Combine(ModelPath, $"recognition{suffix}.hdl").Replace("\\", "/");
            
            HOperatorSet.WriteDlModel(optimizedModelHandle, optimizedPath);

            progress?.Invoke(100, "优化完成");

            // 清理
            dlModelHandle?.Dispose();
            optimizedModelHandle?.Dispose();

            Console.WriteLine($"[{_name}] 模型优化导出成功: {optimizedPath}");
            return true;
        }
        catch (Exception ex)
        {
            progress?.Invoke(0, $"优化失败: {ex.Message}");
            Console.WriteLine($"[{_name}] 模型优化失败: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// 检查优化模型是否存在
    /// </summary>
    public bool HasOptimizedModel(DnnRuntime runtime)
    {
        if (string.IsNullOrEmpty(ModelPath))
            return false;

        string suffix = runtime switch
        {
            DnnRuntime.TensorRT => "_tensorrt",
            DnnRuntime.OpenVINO => "_openvino",
            _ => ""
        };

        if (string.IsNullOrEmpty(suffix))
            return false;

        string optimizedPath = Path.Combine(ModelPath, $"recognition{suffix}.hdl");
        return File.Exists(optimizedPath);
    }

    #endregion

    #region IInferenceDeviceProvider 实现

    /// <summary>
    /// 枚举可用推理设备及对应运行时
    /// </summary>
    public List<DnnInferenceDeviceInfo> GetAvailableInferenceDevices()
    {
        var devices = new List<DnnInferenceDeviceInfo>();

        // 通用 GC 运行时
        TryCollectDevices(
            (new HTuple("runtime")).TupleConcat("runtime"),
            (new HTuple("gpu")).TupleConcat("cpu"),
            DnnRuntime.GC,
            devices);

        // OpenVINO
        TryCollectDevices(
            new HTuple("ai_accelerator_interface"),
            new HTuple("openvino"),
            DnnRuntime.OpenVINO,
            devices);

        // TensorRT
        TryCollectDevices(
            new HTuple("ai_accelerator_interface"),
            new HTuple("tensorrt"),
            DnnRuntime.TensorRT,
            devices);

        return devices;
    }

    private void TryCollectDevices(HTuple interfaceName, HTuple deviceName, DnnRuntime runtime, List<DnnInferenceDeviceInfo> output)
    {
        try
        {
            HOperatorSet.QueryAvailableDlDevices(interfaceName, deviceName, out var handles);
            AppendDeviceInfo(handles, runtime, output);
            handles?.Dispose();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[{_name}] 查询{runtime}设备失败: {ex.Message}");
        }
    }

    private void AppendDeviceInfo(HTuple handles, DnnRuntime runtime, List<DnnInferenceDeviceInfo> output)
    {
        if (handles == null || handles.Length == 0) return;

        for (int i = 0; i < handles.Length; i++)
        {
            try
            {
                HOperatorSet.GetDlDeviceParam(handles[i], "name", out var name);
                HOperatorSet.GetDlDeviceParam(handles[i], "type", out var type);

                var deviceType = string.Equals(type.S, "cpu", StringComparison.OrdinalIgnoreCase)
                    ? DnnDeviceType.CPU
                    : DnnDeviceType.GPU;

                output.Add(new DnnInferenceDeviceInfo
                {
                    Name = name.S,
                    Runtime = runtime,
                    DeviceType = deviceType
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[{_name}] 读取{runtime}设备信息失败: {ex.Message}");
            }
        }
    }

    #endregion

    #region 私有辅助方法

    private bool QueryAvailableDevices(DnnDeviceType deviceType, DnnRuntime runtime)
    {
        try
        {
            switch (runtime)
            {
                case DnnRuntime.GC:
                    HOperatorSet.QueryAvailableDlDevices(
                        (new HTuple("runtime")).TupleConcat("runtime"),
                        (new HTuple("gpu")).TupleConcat("cpu"),
                        out _deviceHandles);
                    break;

                case DnnRuntime.OpenVINO:
                    HOperatorSet.QueryAvailableDlDevices(
                        "ai_accelerator_interface",
                        "openvino",
                        out _deviceHandles);
                    break;

                case DnnRuntime.TensorRT:
                    HOperatorSet.QueryAvailableDlDevices(
                        "ai_accelerator_interface",
                        "tensorrt",
                        out _deviceHandles);
                    break;
            }

            if (_deviceHandles.Length == 0)
            {
                Console.WriteLine("没有检测到可用设备");
                return false;
            }

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"查询设备失败: {ex.Message}");
            return false;
        }
    }

    private void SelectDevice(HTuple handles, string type)
    {
        for (int i = 0; i < handles.Length; i++)
        {
            HOperatorSet.GetDlDeviceParam(handles[i], "type", out var devType);
            if (devType.S == type.ToLower())
            {
                _deviceHandle = handles[i];
                HOperatorSet.SetDeepOcrParam(_modelHandle, "device", _deviceHandle);
                Console.WriteLine($"使用设备: {type}");
                break;
            }
        }
    }

    #endregion
}
