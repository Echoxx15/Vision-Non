using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Cognex.VisionPro;
using DnnInferenceNet.BassClass;
using HalconDotNet;

namespace DnnInferenceNet.DnnBase;

public class DnnSemanticSegmetation : DeepBase
{
  HTuple hv_ClassNames = new HTuple(), hv_ClassIDs = new HTuple();
  private HTuple hv_DatasetInfo = new HTuple();

  private readonly object _lock = new();


  ~DnnSemanticSegmetation()
  {
    UnInit();
  }

  /// <summary>
  /// 从缓存创建实例（推荐用于 ToolBlock 输入端子）
  /// </summary>
  public static DnnSemanticSegmetation CreateFromCache(DLModelCache cache)
  {
    if (cache == null || !cache.IsLoaded)
      throw new ArgumentException("模型缓存无效或未加载");

    var instance = new DnnSemanticSegmetation
    {
      // 共享句柄（不拥有所有权，析构时不释放）
      hv_DLModelHandle = cache.ModelHandle,
      hv_DLPreprocessParam = cache.PreprocessParam,
      hv_DLDevice = cache.DeviceHandle,
      DeviceHandles = cache.DeviceHandles,
      hv_ClassNames = cache.ClassNames,
      hv_ClassIDs = cache.ClassIDs,
      hv_DatasetInfo = cache.DatasetInfo,
      IsInitialSuccese = true,
      _isFromCache = true  // 标记为从缓存创建
    };

    Console.WriteLine($"从缓存创建语义分割实例");
    return instance;
  }

  // 标记是否从缓存创建（影响析构行为）
  private bool _isFromCache = false;

  public override void UnInit()
  {
    try
    {
      // 如果是从缓存创建的，不释放句柄（句柄由缓存管理）
      if (!_isFromCache)
      {
        hv_ClassNames?.Dispose();
        hv_ClassIDs?.Dispose();
        hv_DatasetInfo?.Dispose();
        base.UnInit();
      }
      else
      {
        // 只清空引用，不释放
        hv_ClassNames = null;
        hv_ClassIDs = null;
        hv_DatasetInfo = null;
        hv_DLModelHandle = null;
        hv_DLPreprocessParam = null;
        hv_DLDevice = null;
        DeviceHandles = null;
      }
    }
    catch (Exception ex)
    {
      Console.WriteLine($"UnInit error: {ex.Message}");
    }
  }

  /// <summary>
  /// 初始化深度学习模型
  /// </summary>
  /// <param name="modelPath">模型文件所在目录</param>
  /// <param name="deviceType">设备类型</param>
  /// <param name="runtime">运行时</param>
  /// <returns>0-成功, -1-失败</returns>
  //public int Init(string modelPath, DeviceType deviceType = DeviceType.GPU, Runtime runtime = Runtime.GC)
  //{
  //  try
  //  {
  //    IsInitialSuccese = false;
  //    if (QueryAvailableDlDevices(deviceType, runtime) != 0)
  //    {
  //      return -1;
  //    }

  //    // 扫描目录并选择最新的模型文件对
  //    if (!TryFindLatestModelFiles(modelPath, out var hdlFile, out var hdictFile, out var error))
  //    {
  //      throw new FileNotFoundException($"未找到有效的模型文件: {error}");
  //    }

  //    // 读取模型文件
  //    hv_DLModelHandle?.Dispose();
  //    string modelFilePath = hdlFile.Replace("\\", "/");
  //    HOperatorSet.ReadDlModel(modelFilePath, out hv_DLModelHandle);

  //    // 读取预处理参数文件
  //    string preprocessParamPath = hdictFile.Replace("\\", "/");
  //    hv_DLPreprocessParam?.Dispose();
  //    HOperatorSet.ReadDict(preprocessParamPath, new HTuple(), new HTuple(), out hv_DLPreprocessParam);

  //    // 获取类别名称和ID
  //    hv_ClassNames?.Dispose();
  //    HOperatorSet.GetDlModelParam(hv_DLModelHandle, "class_names", out hv_ClassNames);
  //    hv_ClassIDs?.Dispose();
  //    HOperatorSet.GetDlModelParam(hv_DLModelHandle, "class_ids", out hv_ClassIDs);

  //    hv_DatasetInfo?.Dispose();
  //    HOperatorSet.CreateDict(out hv_DatasetInfo);
  //    HOperatorSet.SetDictTuple(hv_DatasetInfo, "class_ids", hv_ClassIDs);
  //    HOperatorSet.SetDictTuple(hv_DatasetInfo, "class_names", hv_ClassNames);

  //    SelectDevice(DeviceHandles, deviceType.ToString());
  //    IsInitialSuccese = true;

  //    Console.WriteLine($"模型初始化成功: {Path.GetFileName(hdlFile)}");
  //    Console.WriteLine($"类别数量: {hv_ClassIDs.Length}");

  //    return 0;
  //  }
  //  catch (HalconException ex)
  //  {
  //    Console.WriteLine($"Halcon异常: {ex.GetErrorMessage()}");
  //    UnInit();
  //    throw;
  //  }
  //  catch (Exception ex)
  //  {
  //    Console.WriteLine($"初始化异常: {ex.Message}");
  //    UnInit();
  //    throw;
  //  }
  //}

  /// <summary>
  /// 扫描目录查找最新的模型文件对（.hdl 和对应的 .hdict）
  /// </summary>
  /// <param name="directory">模型目录</param>
  /// <param name="hdlFile">输出的.hdl文件路径</param>
  /// <param name="hdictFile">输出的.hdict文件路径</param>
  /// <param name="error">错误信息</param>
  /// <returns>是否找到有效文件对</returns>
  //private bool TryFindLatestModelFiles(string directory, out string hdlFile, out string hdictFile, out string error)
  //{
  //  hdlFile = null;
  //  hdictFile = null;
  //  error = null;

  //  try
  //  {
  //    if (!Directory.Exists(directory))
  //    {
  //      error = $"目录不存在: {directory}";
  //      return false;
  //    }

  //    // 查找所有.hdl文件
  //    var hdlFiles = Directory.GetFiles(directory, "*.hdl", SearchOption.TopDirectoryOnly)
  //      .Select(f => new FileInfo(f))
  //      .OrderByDescending(f => f.LastWriteTime)
  //      .ToList();

  //    if (hdlFiles.Count == 0)
  //    {
  //      error = "目录中未找到.hdl模型文件";
  //      return false;
  //    }

  //    // 查找所有.hdict文件
  //    var hdictFiles = Directory.GetFiles(directory, "*_dl_preprocess_params.hdict", SearchOption.TopDirectoryOnly)
  //      .Select(f => new FileInfo(f))
  //      .ToList();

  //    if (hdictFiles.Count == 0)
  //    {
  //      error = "目录中未找到.hdict预处理参数文件";
  //      return false;
  //    }

  //    // 尝试匹配最新的模型文件和对应的参数文件
  //    foreach (var hdl in hdlFiles)
  //    {
  //      // 提取模型文件的前缀（去除扩展名）
  //      string hdlPrefix = Path.GetFileNameWithoutExtension(hdl.Name);

  //      // 查找匹配的.hdict文件（前缀相同）
  //      var matchingHdict = hdictFiles.FirstOrDefault(hdict =>
  //      {
  //        string hdictName = hdict.Name;
  //        // hdict文件格式：{prefix}_dl_preprocess_params.hdict
  //        return hdictName.StartsWith(hdlPrefix + "_", StringComparison.OrdinalIgnoreCase);
  //      });

  //      if (matchingHdict != null)
  //      {
  //        hdlFile = hdl.FullName;
  //        hdictFile = matchingHdict.FullName;
  //        Console.WriteLine($"找到模型对: {Path.GetFileName(hdlFile)} <-> {Path.GetFileName(hdictFile)}");
  //        return true;
  //      }
  //    }

  //    error = "未找到匹配的模型文件对（.hdl 和对应的 .hdict 文件前缀不匹配）";
  //    return false;
  //  }
  //  catch (Exception ex)
  //  {
  //    error = $"扫描文件时发生错误: {ex.Message}";
  //    return false;
  //  }
  //}

  //private void SelectDevice(HTuple handles, string type)
  //{
  //  for (int i = 0; i < handles.Length; i++)
  //  {
  //    HOperatorSet.GetDlDeviceParam(handles[i], "type", out var devType);
  //    if (devType.S == type.ToLower())
  //    {
  //      hv_DLDevice = handles[i];
  //      HOperatorSet.SetDlModelParam(hv_DLModelHandle, "device", hv_DLDevice);
  //      Console.WriteLine($"使用设备: {type}");
  //      break;
  //    }
  //  }
  //}


  /// <summary>
  /// 单张图片推理
  /// </summary>
  /// <param name="image">VisionPro原图</param>
  /// <param name="hv_reImage">推理结果图,返回映射的图像</param>
  /// <returns>1-成功, -1-失败</returns>
  public int ApplyModel(ICogImage image, out HObject hv_reImage)
  {
    lock (_lock)
    {
      hv_reImage = null;
      HObject hv_Image = null;
      HTuple hv_DLSampleBatch = null;

      try
      {
        if (!IsInitialSuccese)
        {
          Console.WriteLine("模型未初始化");
          return -1;
        }

        ICogImage img = image.CopyBase(CogImageCopyModeConstants.CopyPixels);
        int Width = img.Width;
        int Height = img.Height;

        ImageProcess.ImageConvertVisionPro2HObject(img, out hv_Image);
        developExport.gen_dl_samples_from_images(hv_Image, out hv_DLSampleBatch);
        developExport.preprocess_dl_samples(hv_DLSampleBatch, hv_DLPreprocessParam);

        if (Inference(hv_DLSampleBatch, Width, Height, out var hv_reImages))
        {
          hv_reImage = hv_reImages[0];
          return 1;
        }
        else
        {
          return -1;
        }
      }
      catch (Exception e)
      {
        Console.WriteLine($"ApplyModel异常: {e.Message}");
        return -1;
      }
      finally
      {
        // 释放临时资源
        hv_Image?.Dispose();
        hv_DLSampleBatch?.Dispose();
      }
    }
  }


  /// <summary>
  /// 批量推理（拼接模式）
  /// </summary>
  /// <param name="image">原始图像</param>
  /// <param name="PixelXArray">图像X像素坐标数组</param>
  /// <param name="PixelYArray">图像Y像素坐标数组</param>
  /// <param name="catWidth">裁剪宽度</param>
  /// <param name="catHeight">裁剪高度</param>
  /// <param name="hv_reImage">推理结果图</param>
  /// <param name="maskRegions">遮罩区域</param>
  /// <returns>1-成功, -1-失败</returns>
  public int ApplyModel(ICogImage image, int[] PixelXArray, int[] PixelYArray, int catWidth, int catHeight,
    out HObject hv_reImage, List<HObject> maskRegions = null)
  {
    lock (_lock)
    {
      hv_reImage = null;
      HObject hv_Image = null;
      HObject h_ImageArray = null;
      HTuple hv_DLSampleBatch = null;

      try
      {
        if (!IsInitialSuccese)
        {
          Console.WriteLine("模型未初始化");
          return -1;
        }

        ImageProcess.ImageConvertVisionPro2HObject(image, out hv_Image);

        int[] Rows = PixelYArray;
        int[] Cols = PixelXArray;
        int cnt = Rows.Length;
        int[] hv_rCat = new int[cnt];

        h_ImageArray = new HObject();
        h_ImageArray.GenEmptyObj();

        for (int i = 0; i < Rows.Length; i++)
        {
          HObject rectangle = null, imageReduced = null, imagePart = null;
          try
          {
            hv_rCat[i] = -1;
            HOperatorSet.GenRectangle1(out rectangle, Rows[i], Cols[i], Rows[i] + catHeight - 1,
              Cols[i] + catWidth - 1);
            HOperatorSet.ReduceDomain(hv_Image, rectangle, out imageReduced);
            HOperatorSet.CropDomain(imageReduced, out imagePart);
            h_ImageArray = h_ImageArray.ConcatObj(imagePart);
          }
          finally
          {
            rectangle?.Dispose();
            imageReduced?.Dispose();
            imagePart?.Dispose();
          }
        }

        developExport.gen_dl_samples_from_images(h_ImageArray, out hv_DLSampleBatch);
        developExport.preprocess_dl_samples(hv_DLSampleBatch, hv_DLPreprocessParam);

        if (!Inference(hv_DLSampleBatch, catWidth, catHeight, out var hResultImages, maskRegions))
        {
          return -1;
        }

        HObject h_rsImgArray = null;
        try
        {
          h_rsImgArray = hResultImages[0];
          for (int i = 1; i < hResultImages.Length; i++)
          {
            var temp = h_rsImgArray.ConcatObj(hResultImages[i]);
            h_rsImgArray.Dispose();
            h_rsImgArray = temp;
          }

          HOperatorSet.TileImagesOffset(h_rsImgArray, out hv_reImage, Rows, Cols, hv_rCat, hv_rCat, hv_rCat,
            hv_rCat, image.Width, image.Height);
          return 1;
        }
        finally
        {
          h_rsImgArray?.Dispose();
          if (hResultImages != null)
          {
            foreach (var img in hResultImages)
            {
              img?.Dispose();
            }
          }
        }
      }
      catch (Exception e)
      {
        Console.WriteLine($"批量推理异常: {e.Message}");
        return -1;
      }
      finally
      {
        hv_Image?.Dispose();
        h_ImageArray?.Dispose();
        hv_DLSampleBatch?.Dispose();
      }
    }
  }

  private bool Inference(HTuple hv_DLSampleBatch, int rawWidth, int rawHeight, out HObject[] hv_reImages,
    List<HObject> maskRegions = null)
  {
    hv_reImages = null;
    HTuple hv_DLResultBatch = null;

    try
    {
      HOperatorSet.ApplyDlModel(hv_DLModelHandle, hv_DLSampleBatch, new HTuple(),
        out hv_DLResultBatch);

      hv_reImages = new HObject[hv_DLSampleBatch.Length];

      for (int i = 0; i < hv_DLSampleBatch.Length; i++)
      {
        HObject ho_SegmentationImage = null;
        HObject hv_MapImage = null;
        HTuple width = null, height = null;

        try
        {
          ho_SegmentationImage = hv_DLResultBatch.TupleSelect(i).TupleGetDictObject("segmentation_image");
          HOperatorSet.GetImageSize(ho_SegmentationImage, out width, out height);
          HOperatorSet.GenImageConst(out hv_MapImage, "byte", width, height);

          // 将分割标签转换为灰度图
          for (int j = 0; j < hv_ClassIDs.Length; j++)
          {
            HObject ho_ClassRegion = null;
            try
            {
              HOperatorSet.Threshold(ho_SegmentationImage, out ho_ClassRegion, hv_ClassIDs[j],
                hv_ClassIDs[j]);
              HOperatorSet.PaintRegion(ho_ClassRegion, hv_MapImage, out hv_MapImage, hv_ClassIDs[j],
                "fill");
            }
            finally
            {
              ho_ClassRegion?.Dispose();
            }
          }

          // 应用遮罩区域
          if (maskRegions != null)
          {
            foreach (var t in maskRegions)
            {
              HOperatorSet.OverpaintRegion(hv_MapImage, t, 0, "fill");
            }
          }

          // 缩放到原始尺寸
          HOperatorSet.ZoomImageSize(hv_MapImage, out hv_reImages[i], rawWidth, rawHeight, "constant");
        }
        finally
        {
          ho_SegmentationImage?.Dispose();
          hv_MapImage?.Dispose();
          width?.Dispose();
          height?.Dispose();
        }
      }

      return true;
    }
    catch (Exception e)
    {
      Console.WriteLine($"推理异常: {e.Message}");
      if (hv_reImages != null)
      {
        foreach (var img in hv_reImages)
        {
          img?.Dispose();
        }
      }

      hv_reImages = null;
      return false;
    }
    finally
    {
      hv_DLResultBatch?.Dispose();
    }
  }
}
