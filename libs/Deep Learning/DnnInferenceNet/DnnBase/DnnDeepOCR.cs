using System.IO;
using Cognex.VisionPro;
using DnnInferenceNet.BassClass;
using HalconDotNet;

namespace DnnInferenceNet.DnnBase;

public class DnnDeepOCR:DeepBase
{
    private object _lock = new();
    
    // 标记是否从缓存创建（影响析构行为）
    private bool _isFromCache = false;
    
    /// <summary>
    /// 从缓存创建实例（推荐用于 ToolBlock 输入端子）
    /// </summary>
    public static DnnDeepOCR CreateFromCache(DLModelCache cache)
    {
        if (cache == null || !cache.IsLoaded)
            throw new System.ArgumentException("模型缓存无效或未加载");

        var instance = new DnnDeepOCR
        {
            hv_DLModelHandle = cache.ModelHandle,
            hv_DLDevice = cache.DeviceHandle,
            DeviceHandles = cache.DeviceHandles,
            IsInitialSuccese = true,
            _isFromCache = true
        };

        System.Console.WriteLine($"从缓存创建深度OCR实例");
        return instance;
    }
    
    public override void UnInit()
    {
        // 如果是从缓存创建的，不释放句柄
        if (!_isFromCache)
        {
            base.UnInit();
        }
        else
        {
            hv_DLModelHandle = null;
            hv_DLDevice = null;
            DeviceHandles = null;
        }
    }
    
    public int Init(string modelPath,DeviceType deviceType = DeviceType.GPU, Runtime runtime = Runtime.GC)
    {
        try
        {
            IsInitialSuccese = false;
            if (QueryAvailableDlDevices(deviceType, runtime) != 0)
            {
                return -1;
            }

            //Read in the retrained model.
            hv_DLModelHandle.Dispose();
            HOperatorSet.CreateDeepOcr(new HTuple(),new HTuple(), out hv_DLModelHandle);
            string recognition_model_path = Path.Combine(modelPath, "recognition.hdl");
            recognition_model_path = recognition_model_path.Replace("\\", "/");
            HOperatorSet.SetDeepOcrParam(hv_DLModelHandle, "recognition_model", recognition_model_path);
            HOperatorSet.SetDeepOcrParam(hv_DLModelHandle, "recognition_batch_size", 1);

            HOperatorSet.SetDeepOcrParam(hv_DLModelHandle, "detection_tiling", "true");
            HOperatorSet.SetDeepOcrParam(hv_DLModelHandle, "detection_tiling_overlap", 60); 
            SelectDevice(DeviceHandles,deviceType.ToString());
            IsInitialSuccese = true;
            return 0;
        }
        catch
        {
        }

        return -1;
    }

    public void SelectDevice(HTuple handles, string type)
    {
        for (int i = 0; i < handles.Length; i++)
        {
            HOperatorSet.GetDlDeviceParam(handles[i], "type", out var devType);
            if (devType.S == type.ToLower())
            {
                hv_DLDevice = handles[i];
                HOperatorSet.SetDeepOcrParam(hv_DLModelHandle, "device", hv_DLDevice);
                break;
            }
        }
    }

    public string[] ApplyModel(ICogImage cogImage)
    {
        lock (_lock)
        {
            
            try
            {
                ICogImage img = cogImage.CopyBase(CogImageCopyModeConstants.CopyPixels);
                ImageProcess.ImageConvertVisionPro2HObject(img.CopyBase(CogImageCopyModeConstants.CopyPixels),
                    out var hv_Image);
                HOperatorSet.ApplyDeepOcr(hv_Image, hv_DLModelHandle,"auto",out var DeepOcrResult);
                HOperatorSet.GetDictTuple(DeepOcrResult, "words", out var words);
                HOperatorSet.GetDictTuple(words, "word", out var word);
                return word.SArr;
            }
            catch
            {
            }

            return new string[] {};
        }
    }
}