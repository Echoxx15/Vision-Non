using System;
using System.Diagnostics;
using System.IO;
using Cognex.VisionPro;
using DnnInferenceNet.BassClass;
using HalconDotNet;

namespace DnnInferenceNet.DnnBase;

public class DnnInferenceInfo : DeepBase
{
    HTuple hv_ClassNames = new HTuple(), hv_ClassIDs = new HTuple();


    private object _lock = new();


    ~DnnInferenceInfo()
    {
        UnInit();
    }

    public int Init(string modelPath, DeviceType deviceType = DeviceType.GPU, Runtime runtime = Runtime.GC)
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
            string modelpath = Path.Combine(modelPath, "model.hdl");
            modelpath = modelpath.Replace("\\", "/");
            HOperatorSet.ReadDlModel(modelpath, out hv_DLModelHandle);

            //
            //Get the parameters used for preprocessing.
            string preprocess_param_path = Path.Combine(modelPath, "params.hdict");
            preprocess_param_path = preprocess_param_path.Replace("\\", "/");
            HOperatorSet.ReadDict(preprocess_param_path, new HTuple(), new HTuple(), out hv_DLPreprocessParam);
            //Retrieve the class names and IDs.
            hv_ClassNames.Dispose();
            HOperatorSet.GetDlModelParam(hv_DLModelHandle, "class_names", out hv_ClassNames);
            hv_ClassIDs.Dispose();
            HOperatorSet.GetDlModelParam(hv_DLModelHandle, "class_ids", out hv_ClassIDs);
            HOperatorSet.CreateDict(out var hv_DatasetInfo);
            HOperatorSet.SetDictTuple(hv_DatasetInfo, "class_ids", hv_ClassIDs);
            HOperatorSet.SetDictTuple(hv_DatasetInfo, "class_names", hv_ClassNames);


            SelectDevice(DeviceHandles, deviceType.ToString());
            IsInitialSuccese = true;
            return -1;
        }
        catch (HalconException HDevExpDefaultException)
        {
            UnInit();
            throw;
        }
    }

    public void SelectDevice(HTuple handles, string type)
    {
        for (int i = 0; i < handles.Length; i++)
        {
            HOperatorSet.GetDlDeviceParam(handles[i], "type", out var devType);
            if (devType.S == type.ToLower())
            {
                hv_DLDevice = handles[i];
                HOperatorSet.SetDlModelParam(hv_DLModelHandle, "device", hv_DLDevice);
                break;
            }
        }
    }


    /// <summary>
    /// 单张图片推理
    /// </summary>
    /// <param name="image"></param>Vp原图
    /// <param name="hv_reImage"></param>推理结果图,返回映射的图像
    /// <returns></returns>
    public int ApplyModel(ICogImage image, out HObject hv_reImage)
    {
        lock (_lock)
        {
            hv_reImage = new HObject();
            try
            {
                ICogImage img = image.CopyBase(CogImageCopyModeConstants.CopyPixels);
                int Ret = -1;
                int Width = img.Width;
                int Height = img.Height;

                ImageProcess.ImageConvertVisionPro2HObject(img.CopyBase(CogImageCopyModeConstants.CopyPixels),
                    out var hv_Image);
                developExport.gen_dl_samples_from_images(hv_Image, out var hv_DLSampleBatch);
                ////
                ////Preprocess the DLSampleBatch.
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
                Console.WriteLine(e);
                return -1;
            }

        }
    }


    /// <summary>
    /// 批量推理
    /// </summary>
    /// <param name="image"></param>
    /// <param name="PixelXArray"></param>图像X像素坐标数组
    /// <param name="PixelYArray"></param>图像Y像素坐标数组
    /// <param name="catWidth"></param>裁剪宽度
    /// <param name="catHeight"></param>裁剪高度
    /// <param name="hv_reImage"></param>推理结果图,返回映射的图像
    /// <returns></returns>
    public int ApplyModel(ICogImage image, int[] PixelXArray, int[] PixelYArray, int catWidth, int catHeight, out HObject hv_reImage)
    {
        lock (_lock)
        {
            hv_reImage = new HObject();
            try
            {
                ImageProcess.ImageConvertVisionPro2HObject(image, out var hv_Image);

                int[] Rows = PixelYArray;
                int[] Cols = PixelXArray;
                int cnt = Rows.Length;
                HTuple[] hv_DLSamples = new HTuple[cnt];
                int[] hv_rCat = new int[cnt];
                HObject h_ImageArray = new HObject();
                for (int i = 0; i < Rows.Length; i++)
                {
                    hv_rCat[i] = -1;
                    HOperatorSet.GenRectangle1(out var rectangle, Rows[i], Cols[i], Rows[i] + catHeight - 1,
                        Cols[i] + catWidth - 1);
                    HOperatorSet.ReduceDomain(hv_Image, rectangle, out var imageReduced);
                    HOperatorSet.CropDomain(imageReduced, out var imagePart);
                    h_ImageArray = i == 0 ? imagePart : h_ImageArray.ConcatObj(imagePart);
                }

                developExport.gen_dl_samples_from_images(h_ImageArray, out var hv_DLSampleBatch);
                developExport.preprocess_dl_samples(hv_DLSampleBatch, hv_DLPreprocessParam);
                Stopwatch sw = new Stopwatch();
                sw.Start();
                Inference(hv_DLSampleBatch, catWidth, catHeight, out var hResultImages);
                sw.Stop();
                var h_rsImgArray = hResultImages[0];
                for (int i = 1; i < hResultImages.Length; i++)
                {
                    h_rsImgArray = h_rsImgArray.ConcatObj(hResultImages[i]);
                }
                HOperatorSet.TileImagesOffset(h_rsImgArray, out var tiledImage, Rows, Cols, hv_rCat, hv_rCat, hv_rCat, hv_rCat, image.Width, image.Height);
                hv_reImage = tiledImage;
                return 1;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return -1;
            }

        }
    }

    private bool Inference(HTuple hv_DLSampleBatch, int rawWidth, int rawHeight, out HObject[] hv_reImages)
    {
        try
        {
            hv_reImages = new HObject[hv_DLSampleBatch.Length];
            HOperatorSet.ApplyDlModel(hv_DLModelHandle, hv_DLSampleBatch, new HTuple(),
                out var hv_DLResultBatch);

            for (int i = 0; i < hv_DLSampleBatch.Length; i++)
            {
                var ho_SegmentationImage = hv_DLResultBatch.TupleSelect(i).TupleGetDictObject("segmentation_image");
                HOperatorSet.GetImageSize(ho_SegmentationImage, out var width, out var height);
                HOperatorSet.GenImageConst(out var hv_MapImage, "byte", width, height);
                // Convert segmentation labels to grayscale image
                for (int j = 0; j < hv_ClassIDs.Length; j++)
                {
                    HOperatorSet.Threshold(ho_SegmentationImage, out var ho_ClassRegion, hv_ClassIDs[j],
                        hv_ClassIDs[j]);
                    HOperatorSet.PaintRegion(ho_ClassRegion, hv_MapImage, out hv_MapImage, hv_ClassIDs[j],
                        "fill");
                }

                HOperatorSet.ZoomImageSize(hv_MapImage, out hv_MapImage, rawWidth, rawHeight, "constant");
                hv_reImages[i] = hv_MapImage;
            }
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            hv_reImages = null;
            return false;
        }
    }
}