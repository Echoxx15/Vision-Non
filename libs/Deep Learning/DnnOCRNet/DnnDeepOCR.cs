using System;
using System.IO;
using System.Windows.Forms;
using DnnInterfaceNet;
using HalconDotNet;

namespace DnnOCRNet;

/// <summary>
/// 深度OCR模型插件 - 实现 IDnnModel 接口
/// </summary>
[DnnModelType("深度OCR", "基于深度学习的OCR模型，支持文字检测和识别")]
public class DnnDeepOCR : IDnnModel, IRenameableDnnModel, IConfigurableDnnModel
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
