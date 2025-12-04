using System;
using DnnInferenceNet.BassClass;
using HalconDotNet;

namespace DnnInferenceNet.DnnBase;

public class DeepBase
{
    public HDevelopExport developExport = new HDevelopExport();
    public HTuple hv_DLDevice = new HTuple(), hv_DLModelHandle = new HTuple(), hv_DLPreprocessParam = new HTuple();
    public HTuple DeviceHandles = new HTuple();

    public bool IsInitialSuccese { get; set; }

    public int Batch_Size
    {
        set => HOperatorSet.SetDlModelParam(hv_DLModelHandle, "batch_size", value);
    }

    public virtual void UnInit()
    {
        hv_DLDevice.Dispose();
        hv_DLPreprocessParam.Dispose();
        hv_DLModelHandle.Dispose();
    }

    public virtual int QueryAvailableDlDevices(DeviceType deviceType,Runtime runtime)
    {
        switch (runtime)
        {
            case Runtime.GC:
            {
                HOperatorSet.QueryAvailableDlDevices((new HTuple("runtime")).TupleConcat("runtime"),
                    (new HTuple("gpu")).TupleConcat("cpu"), out DeviceHandles);
                if (DeviceHandles.Length == 0)
                {
                    throw (new Exception("没有检测到可用设备"));
                }

                break;
            }
            case Runtime.OPENVINO:
            {
                HOperatorSet.QueryAvailableDlDevices("ai_accelerator_interface", "openvino",
                    out DeviceHandles);
                if (DeviceHandles.Length == 0)
                {
                    throw (new Exception("没有检测到可用设备"));
                }

                break;
            }
            case Runtime.TENSORRT:
            {
                HOperatorSet.QueryAvailableDlDevices("ai_accelerator_interface", "tensorrt",
                    out DeviceHandles);
                if (DeviceHandles.Length == 0)
                {
                    throw (new Exception("没有检测到可用设备"));
                }

                break;
            }
        }

        if (DeviceHandles.Length == 0)
        {
            return -1;
        }

        return 0;
    }


}