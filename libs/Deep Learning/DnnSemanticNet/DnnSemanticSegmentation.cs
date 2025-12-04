using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using DnnInterfaceNet;
using HalconDotNet;

namespace DnnSemanticNet;

/// <summary>
/// 语义分割模型插件 - 实现 IDnnModel 接口
/// </summary>
[DnnModelType("语义分割", "基于深度学习的语义分割模型，支持像素级分类")]
public class DnnSemanticSegmentation : IDnnModel, IRenameableDnnModel, IConfigurableDnnModel
{
    #region 私有字段

    private string _name;
    private readonly object _lock = new();

    // Halcon 句柄
    private HTuple _modelHandle;
    private HTuple _preprocessParam;
    private HTuple _deviceHandle;
    private HTuple _deviceHandles;
    private HTuple _classNames;
    private HTuple _classIDs;
    private HTuple _datasetInfo;

    // 配置参数
    private DnnDeviceType _deviceType = DnnDeviceType.GPU;
    private DnnRuntime _runtime = DnnRuntime.GC;
    private bool _loadOnStartup;

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
    /// 类别名称数组
    /// </summary>
    public HTuple ClassNames => _classNames;

    /// <summary>
    /// 类别ID数组
    /// </summary>
    public HTuple ClassIDs => _classIDs;

    /// <summary>
    /// 数据集信息
    /// </summary>
    public HTuple DatasetInfo => _datasetInfo;

    /// <summary>
    /// 模型句柄（供外部高级使用）
    /// </summary>
    public HTuple ModelHandle => _modelHandle;

    /// <summary>
    /// 预处理参数
    /// </summary>
    public HTuple PreprocessParam => _preprocessParam;

    #endregion

    #region 构造函数

    public DnnSemanticSegmentation(string name)
    {
        _name = name ?? throw new ArgumentNullException(nameof(name));
    }

    #endregion

    #region IDnnModel 实现

    public UserControl GetConfigControl()
    {
        // 返回配置界面控件
        return new SemanticConfigControl(this);
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

                // 扫描并读取模型文件
                if (!TryFindAndReadModel(modelPath))
                {
                    Console.WriteLine($"[{_name}] 读取模型文件失败");
                    return false;
                }

                // 设置设备
                SelectDevice(_deviceHandles, deviceType.ToString());

                IsLoaded = true;
                Console.WriteLine($"[{_name}] 模型加载成功: {Path.GetFileName(modelPath)}");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[{_name}] 加载模型失败: {ex.Message}");
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
                _preprocessParam?.Dispose();
                _deviceHandle?.Dispose();
                _deviceHandles?.Dispose();
                _classNames?.Dispose();
                _classIDs?.Dispose();
                _datasetInfo?.Dispose();
            }
            catch { }
            finally
            {
                _modelHandle = new HTuple();
                _preprocessParam = new HTuple();
                _deviceHandle = new HTuple();
                _deviceHandles = new HTuple();
                _classNames = new HTuple();
                _classIDs = new HTuple();
                _datasetInfo = new HTuple();
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

            HTuple dlSampleBatch = null;
            HTuple dlResultBatch = null;

            try
            {
                // 获取图像尺寸
                HOperatorSet.GetImageSize(image, out var width, out var height);

                // 生成样本
                GenDlSamplesFromImages(image, out dlSampleBatch);
                PreprocessDlSamples(dlSampleBatch, _preprocessParam);

                // 推理
                HOperatorSet.ApplyDlModel(_modelHandle, dlSampleBatch, new HTuple(), out dlResultBatch);

                // 处理结果
                var segImage = dlResultBatch.TupleSelect(0).TupleGetDictObject("segmentation_image");
                
                // 转换为映射图像
                HOperatorSet.GetImageSize(segImage, out var segWidth, out var segHeight);
                HOperatorSet.GenImageConst(out var mapImage, "byte", segWidth, segHeight);

                for (int j = 0; j < _classIDs.Length; j++)
                {
                    HOperatorSet.Threshold(segImage, out var classRegion, _classIDs[j], _classIDs[j]);
                    HOperatorSet.PaintRegion(classRegion, mapImage, out mapImage, _classIDs[j], "fill");
                    classRegion?.Dispose();
                }

                // 缩放到原始尺寸
                HOperatorSet.ZoomImageSize(mapImage, out var resultImage, width, height, "constant");

                result.Success = true;
                result.ResultImage = resultImage;

                // 收集类别信息
                result.ClassIDs = new int[_classIDs.Length];
                result.ClassNames = new string[_classNames.Length];
                for (int i = 0; i < _classIDs.Length; i++)
                {
                    result.ClassIDs[i] = _classIDs[i].I;
                    result.ClassNames[i] = _classNames[i].S;
                }

                segImage?.Dispose();
                mapImage?.Dispose();
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.ErrorMessage = ex.Message;
            }
            finally
            {
                dlSampleBatch?.Dispose();
                dlResultBatch?.Dispose();
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
        return new DnnModelConfig
        {
            Name = _name,
            Type = "语义分割",
            ModelPath = ModelPath,
            DeviceType = _deviceType,
            Runtime = _runtime,
            LoadOnStartup = _loadOnStartup
        };
    }

    public void ApplyConfig(DnnModelConfig config)
    {
        if (config == null) return;

        _deviceType = config.DeviceType;
        _runtime = config.Runtime;
        _loadOnStartup = config.LoadOnStartup;

        // 如果有模型路径且尚未加载，可选择加载
        if (!string.IsNullOrEmpty(config.ModelPath))
        {
            ModelPath = config.ModelPath;
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

    private bool TryFindAndReadModel(string directory)
    {
        try
        {
            if (!Directory.Exists(directory))
                return false;

            // 查找 .hdl 文件
            var hdlFiles = Directory.GetFiles(directory, "*.hdl", SearchOption.TopDirectoryOnly);
            if (hdlFiles.Length == 0)
                return false;

            // 使用第一个找到的模型文件
            var hdlFile = hdlFiles[0];
            string modelFilePath = hdlFile.Replace("\\", "/");
            HOperatorSet.ReadDlModel(modelFilePath, out _modelHandle);

            // 查找对应的预处理参数文件
            string hdlPrefix = Path.GetFileNameWithoutExtension(hdlFile);
            var hdictFiles = Directory.GetFiles(directory, $"{hdlPrefix}_dl_preprocess_params.hdict", SearchOption.TopDirectoryOnly);
            if (hdictFiles.Length > 0)
            {
                string hdictFile = hdictFiles[0].Replace("\\", "/");
                HOperatorSet.ReadDict(hdictFile, new HTuple(), new HTuple(), out _preprocessParam);
            }

            // 获取模型参数（语义分割模型）
            try
            {
                HOperatorSet.GetDlModelParam(_modelHandle, "class_names", out _classNames);
                HOperatorSet.GetDlModelParam(_modelHandle, "class_ids", out _classIDs);

                HOperatorSet.CreateDict(out _datasetInfo);
                HOperatorSet.SetDictTuple(_datasetInfo, "class_ids", _classIDs);
                HOperatorSet.SetDictTuple(_datasetInfo, "class_names", _classNames);

                Console.WriteLine($"模型类别数量: {_classIDs.Length}");
            }
            catch
            {
                Console.WriteLine("警告: 无法获取类别信息（可能不是语义分割模型）");
            }

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"读取模型文件失败: {ex.Message}");
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
                HOperatorSet.SetDlModelParam(_modelHandle, "device", _deviceHandle);
                Console.WriteLine($"使用设备: {type}");
                break;
            }
        }
    }

    private void GenDlSamplesFromImages(HObject image, out HTuple dlSamples)
    {
        HOperatorSet.CreateDict(out var dlSample);
        HOperatorSet.SetDictObject(image, dlSample, "image");
        dlSamples = dlSample;
    }

    private void PreprocessDlSamples(HTuple dlSamples, HTuple preprocessParam)
    {
        // 获取预处理参数
        HOperatorSet.GetDictTuple(preprocessParam, "image_width", out var imageWidth);
        HOperatorSet.GetDictTuple(preprocessParam, "image_height", out var imageHeight);
        HOperatorSet.GetDictTuple(preprocessParam, "image_num_channels", out var numChannels);
        HOperatorSet.GetDictTuple(preprocessParam, "image_range_min", out var rangeMin);
        HOperatorSet.GetDictTuple(preprocessParam, "image_range_max", out var rangeMax);

        // 获取原始图像
        var origImage = dlSamples.TupleGetDictObject("image");

        // 转换通道数
        HOperatorSet.CountChannels(origImage, out var channels);
        HObject convertedImage;
        if (channels.I == 1 && numChannels.I == 3)
        {
            HOperatorSet.Compose3(origImage, origImage, origImage, out convertedImage);
        }
        else if (channels.I == 3 && numChannels.I == 1)
        {
            HOperatorSet.Rgb1ToGray(origImage, out convertedImage);
        }
        else
        {
            convertedImage = origImage.Clone();
        }

        // 缩放到目标尺寸
        HOperatorSet.ZoomImageSize(convertedImage, out var zoomedImage, imageWidth, imageHeight, "constant");

        // 转换为实数图像并归一化
        HOperatorSet.ConvertImageType(zoomedImage, out var realImage, "real");
        HOperatorSet.ScaleImage(realImage, out var scaledImage, 
            (rangeMax.D - rangeMin.D) / 255.0, rangeMin.D);

        // 更新样本
        HOperatorSet.SetDictObject(scaledImage, dlSamples, "image");

        origImage?.Dispose();
        convertedImage?.Dispose();
        zoomedImage?.Dispose();
        realImage?.Dispose();
    }

    #endregion
}
