using System;
using HalconDotNet;

namespace DnnInferenceNet.DnnBase;

/// <summary>
/// 深度学习模型缓存 - 存储加载后的模型句柄和预处理参数
/// 提供给具体类型（DnnSemanticSegmetation、DnnDeepOCR）使用
/// </summary>
public class DLModelCache : IDisposable
{
    /// <summary>
    /// 模型句柄
    /// </summary>
    private HTuple _modelHandle;
    public HTuple ModelHandle => _modelHandle;

    /// <summary>
    /// 预处理参数
    /// </summary>
    private HTuple _preprocessParam;
    public HTuple PreprocessParam => _preprocessParam;

    /// <summary>
    /// 设备句柄
    /// </summary>
    private HTuple _deviceHandle;
    public HTuple DeviceHandle => _deviceHandle;

    /// <summary>
    /// 可用设备列表
    /// </summary>
    private HTuple _deviceHandles;
    public HTuple DeviceHandles => _deviceHandles;

    /// <summary>
    /// 类别名称（用于语义分割）
    /// </summary>
    private HTuple _classNames;
    public HTuple ClassNames => _classNames;

    /// <summary>
    /// 类别ID（用于语义分割）
    /// </summary>
    private HTuple _classIDs;
    public HTuple ClassIDs => _classIDs;

    /// <summary>
    /// 数据集信息
    /// </summary>
    private HTuple _datasetInfo;
    public HTuple DatasetInfo => _datasetInfo;

    /// <summary>
    /// 模型文件路径
    /// </summary>
    public string ModelPath { get; private set; }

    /// <summary>
    /// 是否已成功加载
    /// </summary>
    public bool IsLoaded { get; private set; }

    /// <summary>
    /// 运行时类型
    /// </summary>
    public Runtime RuntimeType { get; private set; }

    /// <summary>
    /// 加载模型（从文件夹路径）
    /// </summary>
    public bool Load(string modelPath, DeviceType deviceType, Runtime runtime)
    {
        try
        {
            Unload();

            ModelPath = modelPath;
            RuntimeType = runtime;

            // 查询可用设备
            if (!QueryAvailableDevices(deviceType, runtime))
                return false;

            // 扫描并读取模型文件
            if (!TryFindAndReadModel(modelPath, out var hdlFile, out var hdictFile))
                return false;

            // 读取预处理参数
            if (!string.IsNullOrEmpty(hdictFile))
            {
                HOperatorSet.ReadDict(hdictFile.Replace("\\", "/"), new HTuple(), new HTuple(), out _preprocessParam);
            }

            // 获取模型参数（如果是语义分割模型）
            try
            {
                HOperatorSet.GetDlModelParam(_modelHandle, "class_names", out _classNames);
                HOperatorSet.GetDlModelParam(_modelHandle, "class_ids", out _classIDs);
                
                HOperatorSet.CreateDict(out _datasetInfo);
                HOperatorSet.SetDictTuple(_datasetInfo, "class_ids", _classIDs);
                HOperatorSet.SetDictTuple(_datasetInfo, "class_names", _classNames);
            }
            catch
            {
                // 不是语义分割模型，忽略
            }

            // 设置设备
            SelectDevice(_deviceHandles, deviceType.ToString());

            IsLoaded = true;
            Console.WriteLine($"模型加载成功: {System.IO.Path.GetFileName(hdlFile)}");
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"加载模型失败: {ex.Message}");
            Unload();
            return false;
        }
    }

    /// <summary>
    /// 卸载模型
    /// </summary>
    public void Unload()
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

    public void Dispose()
    {
        Unload();
    }

    #region 私有辅助方法

    private bool QueryAvailableDevices(DeviceType deviceType, Runtime runtime)
    {
        try
        {
            switch (runtime)
            {
                case Runtime.GC:
                    HOperatorSet.QueryAvailableDlDevices(
                        (new HTuple("runtime")).TupleConcat("runtime"),
                        (new HTuple("gpu")).TupleConcat("cpu"),
                        out _deviceHandles);
                    break;

                case Runtime.OPENVINO:
                    HOperatorSet.QueryAvailableDlDevices(
                        "ai_accelerator_interface",
                        "openvino",
                        out _deviceHandles);
                    break;

                case Runtime.TENSORRT:
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

    private bool TryFindAndReadModel(string directory, out string hdlFile, out string hdictFile)
    {
        hdlFile = null;
        hdictFile = null;

        try
        {
            if (!System.IO.Directory.Exists(directory))
                return false;

            // 查找 .hdl 文件
            var hdlFiles = System.IO.Directory.GetFiles(directory, "*.hdl", System.IO.SearchOption.TopDirectoryOnly);
            if (hdlFiles.Length == 0)
                return false;

            // 使用第一个找到的模型文件
            hdlFile = hdlFiles[0];
            string modelFilePath = hdlFile.Replace("\\", "/");
            HOperatorSet.ReadDlModel(modelFilePath, out _modelHandle);

            // 查找对应的预处理参数文件
            string hdlPrefix = System.IO.Path.GetFileNameWithoutExtension(hdlFile);
            var hdictFiles = System.IO.Directory.GetFiles(directory, $"{hdlPrefix}_dl_preprocess_params.hdict", System.IO.SearchOption.TopDirectoryOnly);
            if (hdictFiles.Length > 0)
            {
                hdictFile = hdictFiles[0];
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

    #endregion
}
