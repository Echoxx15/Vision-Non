using System;
using System.Text;

using IKapCDotNet;
using IKapBoardDotNet;
using IKNS;
using System.Runtime.InteropServices;

namespace GeneralConfigureCameraNS
{

    class GeneralConfigureCamera
    {
        static public void ConfigureCamera(IKCamera cam, int index = -1, string deviceClass = null)
        {
            uint res = IKapC.ITKSTATUS_OK;
            int ret = IKapBoard.IK_RTN_OK;

            uint numCameras = 0;

            /// \~chinese 枚举可用相机的数量。在打开相机前，必须调用 ItkManGetDeviceCount() 函数				\~english Enumerate
            /// the number of available cameras. Before opening the camera, ItkManGetDeviceCount() function must be called.
            res = IKapC.ItkManGetDeviceCount(ref numCameras);
            IKUtils.CheckIKapC(res);

            if (numCameras == 0 || cam == null)
            {
                Console.Write("No camera.\n");
                IKapC.ItkManTerminate();
                IKUtils.pressEnterToExit();
            }

            uint i = 0;
            ITKDEV_INFO di = new ITKDEV_INFO();

            /// \~chinese 打开相机				          \~english Open camera
            for (; i < numCameras; i++)
            {
                /// \~chinese 获取相机设备信息				              \~english Get camera device information
                res = IKapC.ItkManGetDeviceInfo(i, di);

                if (cam.g_SerialNumber != null && cam.g_SerialNumber != "")
                {
                    /// \~chinese 当设备序列号正确时				          \~english When the serial number is proper
                    if (String.Compare(di.SerialNumber, cam.g_SerialNumber) == 0 && String.Compare(di.SerialNumber, "") != 0)
                    {
                        Console.Write("Using camera: serial: {0}, name: {1}, interface: {2}.\n", di.SerialNumber, di.FullName, di.DeviceClass);
                        res = IKapC.ItkDevOpen(i, IKapC.ITKDEV_VAL_ACCESS_MODE_EXCLUSIVE, ref cam.g_hCamera);
                        IKUtils.CheckIKapC(res);
                        break;
                    }
                }
                else if (index >= 0)
                {
                    /// \~chinese 当设备序号正确时				               \~english When the index is proper
                    if (i == index)
                    {
                        Console.Write("Using camera: serial: {0}, name: {1}, interface: {2}.\n", di.SerialNumber, di.FullName, di.DeviceClass);
                        res = IKapC.ItkDevOpen(i, IKapC.ITKDEV_VAL_ACCESS_MODE_EXCLUSIVE, ref cam.g_hCamera);
                        IKUtils.CheckIKapC(res);
                        cam.g_index = (int)i;
                        break;
                    }
                }
                else if (deviceClass != null)
                {
                    /// \~chinese 当设备类型正确时				               \~english When the device class is proper
                    if (String.Compare(di.DeviceClass, deviceClass) == 0)
                    {
                        Console.Write("Using camera: serial: {0}, name: {1}, interface: {2}.\n", di.SerialNumber, di.FullName, di.DeviceClass);
                        res = IKapC.ItkDevOpen(i, IKapC.ITKDEV_VAL_ACCESS_MODE_EXCLUSIVE, ref cam.g_hCamera);
                        IKUtils.CheckIKapC(res);
                        break;
                    }
                }
                else
                {
                    Console.Write("Please set proper attribute to open camera.\n");
                    IKapC.ItkManTerminate();
                    IKUtils.pressEnterToExit();
                }
            }

            if (i >= numCameras)
            {
                Console.Write("Cannot find proper camera.\n");
                IKapC.ItkManTerminate();
                IKUtils.pressEnterToExit();
            }

            // \~chinese 设置相机的超时时间				\~english set timeout of camera
            IntPtr timeOutPtr = Marshal.AllocHGlobal(4);
            Marshal.WriteInt32(timeOutPtr,20000);
            res = IKapC.ItkDevSetPrm(cam.g_hCamera, IKapC.ITKDEV_PRM_HEARTBEAT_TIMEOUT, timeOutPtr);
            IKUtils.CheckIKapC(res);
            Marshal.FreeHGlobal(timeOutPtr);

            // \~chinese 获取特征名称列表并配置参数设置方法				\~english Get list of features' name and select parameter
            // configure method
            GetAllConfigureMethods(cam);

            cam.g_devInfo = di;
            ITK_CXP_DEV_INFO cxp_cam_info = new ITK_CXP_DEV_INFO();
            ITK_GVB_DEV_INFO gvb_cam_info = new ITK_GVB_DEV_INFO();
            ITK_CL_DEV_INFO cl_cam_info = new ITK_CL_DEV_INFO();
            IntPtr info = IntPtr.Zero;
            if (String.Compare(di.DeviceClass, "CoaXPress") == 0)
            {
                /// \~chinese 获取CoaXPress相机设备信息				    \~english Get CoaXPress camera device information
                res = IKapC.ItkManGetCXPDeviceInfo(i, cxp_cam_info);
                IKUtils.CheckIKapC(res);
                info = IKapC.get_itk_cxp_dev_info_IntPtr(cxp_cam_info);
                cam.g_isCXP = true;
            }
            else if (String.Compare(di.DeviceClass, "GigEVisionBoard") == 0)
            {
                /// \~chinese 获取GigEVision相机设备信息				    \~english Get GigEVision camera device information
                res = IKapC.ItkManGetGVBDeviceInfo(i, gvb_cam_info);
                IKUtils.CheckIKapC(res);
                info = IKapC.get_itk_gvb_dev_info_IntPtr(gvb_cam_info);
            }
            else if (String.Compare(di.DeviceClass, "CameraLink") == 0)
            {
                /// \~chinese 获取CameraLink相机设备信息				    \~english Get CameraLink camera device information
                res = IKapC.ItkManGetCLDeviceInfo(i, cl_cam_info);
                IKUtils.CheckIKapC(res);
                info = IKapC.get_itk_cl_dev_info_IntPtr(cl_cam_info);
            }
            else
            {
                return;
            }

            cam.g_hBoard = IKapBoard.IKapOpenWithSpecificInfo(info);
            if (cam.g_hBoard == IntPtr.Zero)
                IKUtils.CheckIKapBoard(IKapBoard.IKStatus_OpenBoardFail);
        }

        static public void GetAllConfigureMethods(IKCamera cam)
        {
            uint res = IKapC.ITKSTATUS_OK;
            ITK_FEATURE_BASE_INFO featureInfo = new ITK_FEATURE_BASE_INFO();

            // width
            if (IKapC.ItkDevGetFeatureInfo(cam.g_hCamera, "RegionEnable", featureInfo) == IKapC.ITKSTATUS_OK)
            {
                if (IKapC.ItkDevGetFeatureInfo(cam.g_hCamera, "RegionWidth", featureInfo) == IKapC.ITKSTATUS_OK)
                {
                    cam.g_configMethod[IKCamera.ITEK_CAMERA_WIDTH_CONFIG] = IKCamera.ITEK_CAMERA_WIDTH_CONFIG_METHOD_1;
                }
            }
            else if (IKapC.ItkDevGetFeatureInfo(cam.g_hCamera, "ROIEnable", featureInfo) == IKapC.ITKSTATUS_OK)
            {
                if (IKapC.ItkDevGetFeatureInfo(cam.g_hCamera, "ROIWidth", featureInfo) == IKapC.ITKSTATUS_OK)
                {
                    cam.g_configMethod[IKCamera.ITEK_CAMERA_WIDTH_CONFIG] = IKCamera.ITEK_CAMERA_WIDTH_CONFIG_METHOD_2;
                }
            }
            else if (IKapC.ItkDevGetFeatureInfo(cam.g_hCamera, "RegionWidth", featureInfo) == IKapC.ITKSTATUS_OK)
            {
                cam.g_configMethod[IKCamera.ITEK_CAMERA_WIDTH_CONFIG] = IKCamera.ITEK_CAMERA_WIDTH_CONFIG_METHOD_3;

            }
            else if (IKapC.ItkDevGetFeatureInfo(cam.g_hCamera, "ROIWidth", featureInfo) == IKapC.ITKSTATUS_OK)
            {
                cam.g_configMethod[IKCamera.ITEK_CAMERA_WIDTH_CONFIG] = IKCamera.ITEK_CAMERA_WIDTH_CONFIG_METHOD_4;
            }

            // height
            if (IKapC.ItkDevGetFeatureInfo(cam.g_hCamera, "RegionEnable", featureInfo) == IKapC.ITKSTATUS_OK)
            {
                if (IKapC.ItkDevGetFeatureInfo(cam.g_hCamera, "RegionHeight", featureInfo) == IKapC.ITKSTATUS_OK)
                {
                    cam.g_configMethod[IKCamera.ITEK_CAMERA_WIDTH_CONFIG] = IKCamera.ITEK_CAMERA_HEIGHT_CONFIG_METHOD_1;
                }
            }
            else if (IKapC.ItkDevGetFeatureInfo(cam.g_hCamera, "ROIEnable", featureInfo) == IKapC.ITKSTATUS_OK)
            {
                if (IKapC.ItkDevGetFeatureInfo(cam.g_hCamera, "ROIHeight", featureInfo) == IKapC.ITKSTATUS_OK)
                {
                    cam.g_configMethod[IKCamera.ITEK_CAMERA_WIDTH_CONFIG] = IKCamera.ITEK_CAMERA_HEIGHT_CONFIG_METHOD_2;
                }
            }
            else if (IKapC.ItkDevGetFeatureInfo(cam.g_hCamera, "RegionHeight", featureInfo) == IKapC.ITKSTATUS_OK)
            {
                cam.g_configMethod[IKCamera.ITEK_CAMERA_WIDTH_CONFIG] = IKCamera.ITEK_CAMERA_HEIGHT_CONFIG_METHOD_3;

            }
            else if (IKapC.ItkDevGetFeatureInfo(cam.g_hCamera, "ROIHeight", featureInfo) == IKapC.ITKSTATUS_OK)
            {
                cam.g_configMethod[IKCamera.ITEK_CAMERA_WIDTH_CONFIG] = IKCamera.ITEK_CAMERA_HEIGHT_CONFIG_METHOD_4;
            }

            // exposure time
            if (IKapC.ItkDevGetFeatureInfo(cam.g_hCamera, "ExposureTimeRaw", featureInfo) == IKapC.ITKSTATUS_OK)
            {
                cam.g_configMethod[IKCamera.ITEK_CAMERA_EXPOSURE_TIME_CONFIG] = IKCamera.ITEK_CAMERA_EXPOSURE_TIME_CONFIG_METHOD_1;
            }
            else if (IKapC.ItkDevGetFeatureInfo(cam.g_hCamera, "AreaMode", featureInfo) == IKapC.ITKSTATUS_OK)
            {
                if (IKapC.ItkDevGetFeatureInfo(cam.g_hCamera, "ExposureTime", featureInfo) == IKapC.ITKSTATUS_OK)
                {
                    cam.g_configMethod[IKCamera.ITEK_CAMERA_EXPOSURE_TIME_CONFIG] = IKCamera.ITEK_CAMERA_EXPOSURE_TIME_CONFIG_METHOD_2;

                }
                else if (IKapC.ItkDevGetFeatureInfo(cam.g_hCamera, "AreaExposureTime", featureInfo) == IKapC.ITKSTATUS_OK)
                {
                    cam.g_configMethod[IKCamera.ITEK_CAMERA_EXPOSURE_TIME_CONFIG] = IKCamera.ITEK_CAMERA_EXPOSURE_TIME_CONFIG_METHOD_3;
                }
            }
            else if (IKapC.ItkDevGetFeatureInfo(cam.g_hCamera, "ExposureTime", featureInfo) == IKapC.ITKSTATUS_OK)
            {
                cam.g_configMethod[IKCamera.ITEK_CAMERA_EXPOSURE_TIME_CONFIG] = IKCamera.ITEK_CAMERA_EXPOSURE_TIME_CONFIG_METHOD_4;
            }
            else if (IKapC.ItkDevGetFeatureInfo(cam.g_hCamera, "Area", featureInfo) == IKapC.ITKSTATUS_OK)
            {
                if (IKapC.ItkDevGetFeatureInfo(cam.g_hCamera, "AreaExposureTime", featureInfo) == IKapC.ITKSTATUS_OK)
                {
                    cam.g_configMethod[IKCamera.ITEK_CAMERA_EXPOSURE_TIME_CONFIG] = IKCamera.ITEK_CAMERA_EXPOSURE_TIME_CONFIG_METHOD_5;
                }
            }

            // digital Gain
            if (IKapC.ItkDevGetFeatureInfo(cam.g_hCamera, "DigitalGain", featureInfo) == IKapC.ITKSTATUS_OK)
            {
                cam.g_configMethod[IKCamera.ITEK_CAMERA_DIGITAL_GAIN_CONFIG] = IKCamera.ITEK_CAMERA_DIGITAL_GAIN_CONFIG_METHOD_1;
            }
            else if (IKapC.ItkDevGetFeatureInfo(cam.g_hCamera, "Gain", featureInfo) == IKapC.ITKSTATUS_OK)
            {
                cam.g_configMethod[IKCamera.ITEK_CAMERA_DIGITAL_GAIN_CONFIG] = IKCamera.ITEK_CAMERA_DIGITAL_GAIN_CONFIG_METHOD_2;
            }

            // line Rate
            if (IKapC.ItkDevGetFeatureInfo(cam.g_hCamera, "LinePeriodTime", featureInfo) == IKapC.ITKSTATUS_OK)
            {
                cam.g_configMethod[IKCamera.ITEK_CAMERA_LINE_RATE_CONFIG] = IKCamera.ITEK_CAMERA_LINE_RATE_CONFIG_METHOD_1;
            }
            else if (IKapC.ItkDevGetFeatureInfo(cam.g_hCamera, "AcquisitionLinePeriod", featureInfo) == IKapC.ITKSTATUS_OK)
            {
                cam.g_configMethod[IKCamera.ITEK_CAMERA_LINE_RATE_CONFIG] = IKCamera.ITEK_CAMERA_LINE_RATE_CONFIG_METHOD_2;
            }
            else if (IKapC.ItkDevGetFeatureInfo(cam.g_hCamera, "AcquisitionLineRate", featureInfo) == IKapC.ITKSTATUS_OK)
            {
                cam.g_configMethod[IKCamera.ITEK_CAMERA_LINE_RATE_CONFIG] = IKCamera.ITEK_CAMERA_LINE_RATE_CONFIG_METHOD_3;
            }

            // frame Rate
            if (IKapC.ItkDevGetFeatureInfo(cam.g_hCamera, "AcquisitionFramePeriodRaw", featureInfo) == IKapC.ITKSTATUS_OK)
            {
                cam.g_configMethod[IKCamera.ITEK_CAMERA_FRAME_RATE_CONFIG] = IKCamera.ITEK_CAMERA_FRAME_RATE_CONFIG_METHOD_1;

            }
            else if (IKapC.ItkDevGetFeatureInfo(cam.g_hCamera, "AcquisitionFramePeriod", featureInfo) == IKapC.ITKSTATUS_OK)
            {
                if ((uint)ITKFEATURE_VAL_TYPE_LIST.ITKFEATURE_VAL_TYPE_INT64 == featureInfo.Type)
                {
                    cam.g_configMethod[IKCamera.ITEK_CAMERA_FRAME_RATE_CONFIG] = IKCamera.ITEK_CAMERA_FRAME_RATE_CONFIG_METHOD_2;
                }
                else if ((uint)ITKFEATURE_VAL_TYPE_LIST.ITKFEATURE_VAL_TYPE_DOUBLE == featureInfo.Type)
                {
                    cam.g_configMethod[IKCamera.ITEK_CAMERA_FRAME_RATE_CONFIG] = IKCamera.ITEK_CAMERA_FRAME_RATE_CONFIG_METHOD_3;
                }

            }
            else if (IKapC.ItkDevGetFeatureInfo(cam.g_hCamera, "FramePeriodTime", featureInfo) == IKapC.ITKSTATUS_OK)
            {
                cam.g_configMethod[IKCamera.ITEK_CAMERA_FRAME_RATE_CONFIG] = IKCamera.ITEK_CAMERA_FRAME_RATE_CONFIG_METHOD_4;
            }
            else if (
              IKapC.ItkDevGetFeatureInfo(cam.g_hCamera, "AcquisitionFrameRateEnable", featureInfo) == IKapC.ITKSTATUS_OK &&
              IKapC.ItkDevGetFeatureInfo(cam.g_hCamera, "AcquisitionFrameRate", featureInfo) == IKapC.ITKSTATUS_OK)
            {
                cam.g_configMethod[IKCamera.ITEK_CAMERA_FRAME_RATE_CONFIG] = IKCamera.ITEK_CAMERA_FRAME_RATE_CONFIG_METHOD_5;
            }
            else if (IKapC.ItkDevGetFeatureInfo(cam.g_hCamera, "AcquisitionFrameRate", featureInfo) == IKapC.ITKSTATUS_OK)
            {
                cam.g_configMethod[IKCamera.ITEK_CAMERA_FRAME_RATE_CONFIG] = IKCamera.ITEK_CAMERA_FRAME_RATE_CONFIG_METHOD_6;
            }

            // line Trigger
            if (IKapC.ItkDevGetFeatureInfo(cam.g_hCamera, "TriggerSelector", featureInfo) == IKapC.ITKSTATUS_OK)
            {
                if (IKapC.ItkDevGetFeatureInfo(cam.g_hCamera, "TriggerMode", featureInfo) == IKapC.ITKSTATUS_OK &&
                    IKapC.ItkDevGetFeatureInfo(cam.g_hCamera, "ExposureMode", featureInfo) == IKapC.ITKSTATUS_OK)
                {
                    cam.g_configMethod[IKCamera.ITEK_CAMERA_LINE_TRIGGER_CONFIG] = IKCamera.ITEK_CAMERA_LINE_TRIGGER_CONFIG_METHOD_1;
                }

            }
            else if (
              IKapC.ItkDevGetFeatureInfo(cam.g_hCamera, "TriggerMode", featureInfo) == IKapC.ITKSTATUS_OK &&
              IKapC.ItkDevGetFeatureInfo(cam.g_hCamera, "ExposureMode", featureInfo) == IKapC.ITKSTATUS_OK)
            {
                cam.g_configMethod[IKCamera.ITEK_CAMERA_LINE_TRIGGER_CONFIG] = IKCamera.ITEK_CAMERA_LINE_TRIGGER_CONFIG_METHOD_2;
            }
            else if (IKapC.ItkDevGetFeatureInfo(cam.g_hCamera, "SynchronizationMode", featureInfo) == IKapC.ITKSTATUS_OK)
            {
                cam.g_configMethod[IKCamera.ITEK_CAMERA_LINE_TRIGGER_CONFIG] = IKCamera.ITEK_CAMERA_LINE_TRIGGER_CONFIG_METHOD_3;
            }

            // frame Trigger
            if (IKapC.ItkDevGetFeatureInfo(cam.g_hCamera, "TriggerSelector", featureInfo) == IKapC.ITKSTATUS_OK)
            {
                if (IKapC.ItkDevGetFeatureInfo(cam.g_hCamera, "TriggerMode", featureInfo) == IKapC.ITKSTATUS_OK &&
                    IKapC.ItkDevGetFeatureInfo(cam.g_hCamera, "ExposureMode", featureInfo) == IKapC.ITKSTATUS_OK)
                {
                    cam.g_configMethod[IKCamera.ITEK_CAMERA_FRAME_TRIGGER_CONFIG] = IKCamera.ITEK_CAMERA_FRAME_TRIGGER_CONFIG_METHOD_1;
                }
            }
            else if (
              IKapC.ItkDevGetFeatureInfo(cam.g_hCamera, "FrameTriggerEnable", featureInfo) == IKapC.ITKSTATUS_OK &&
              IKapC.ItkDevGetFeatureInfo(cam.g_hCamera, "FrameTriggerType", featureInfo) == IKapC.ITKSTATUS_OK)
            {
                cam.g_configMethod[IKCamera.ITEK_CAMERA_FRAME_TRIGGER_CONFIG] = IKCamera.ITEK_CAMERA_FRAME_TRIGGER_CONFIG_METHOD_2;
            }
            else if (IKapC.ItkDevGetFeatureInfo(cam.g_hCamera, "ExposureMode", featureInfo) == IKapC.ITKSTATUS_OK)
            {
                cam.g_configMethod[IKCamera.ITEK_CAMERA_FRAME_TRIGGER_CONFIG] = IKCamera.ITEK_CAMERA_FRAME_TRIGGER_CONFIG_METHOD_3;
            }

            // trigger source
            if (IKapC.ItkDevGetFeatureInfo(cam.g_hCamera, "FrameTriggerSource", featureInfo) == IKapC.ITKSTATUS_OK)
            {
                cam.g_configMethod[IKCamera.ITEK_CAMERA_FRAME_TRIGGER_CONFIG_METHOD_1] = IKCamera.ITEK_CAMERA_TRIGGER_SOURCE_CONFIG_METHOD_1;
            }
            else if (IKapC.ItkDevGetFeatureInfo(cam.g_hCamera, "ExternalTriggerSource", featureInfo) == IKapC.ITKSTATUS_OK)
            {
                cam.g_configMethod[IKCamera.ITEK_CAMERA_FRAME_TRIGGER_CONFIG_METHOD_1] = IKCamera.ITEK_CAMERA_TRIGGER_SOURCE_CONFIG_METHOD_2;
            }
            else if (IKapC.ItkDevGetFeatureInfo(cam.g_hCamera, "TriggerSource", featureInfo) == IKapC.ITKSTATUS_OK)
            {
                cam.g_configMethod[IKCamera.ITEK_CAMERA_FRAME_TRIGGER_CONFIG_METHOD_1] = IKCamera.ITEK_CAMERA_TRIGGER_SOURCE_CONFIG_METHOD_3;
            }

            // trigger polarity
            if (IKapC.ItkDevGetFeatureInfo(cam.g_hCamera, "FrameTriggerPolarity", featureInfo) == IKapC.ITKSTATUS_OK)
            {
                cam.g_configMethod[IKCamera.ITEK_CAMERA_TRIGGER_POLARITY_CONFIG] = IKCamera.ITEK_CAMERA_TRIGGER_POLARITY_CONFIG_METHOD_1;
            }
            else if (IKapC.ItkDevGetFeatureInfo(cam.g_hCamera, "ExternalTriggerActivation", featureInfo) == IKapC.ITKSTATUS_OK)
            {
                cam.g_configMethod[IKCamera.ITEK_CAMERA_TRIGGER_POLARITY_CONFIG] = IKCamera.ITEK_CAMERA_TRIGGER_POLARITY_CONFIG_METHOD_2;
            }
            else if (IKapC.ItkDevGetFeatureInfo(cam.g_hCamera, "LinePolarity", featureInfo) == IKapC.ITKSTATUS_OK)
            {
                cam.g_configMethod[IKCamera.ITEK_CAMERA_TRIGGER_POLARITY_CONFIG] = IKCamera.ITEK_CAMERA_TRIGGER_POLARITY_CONFIG_METHOD_3;
            }
            else if (IKapC.ItkDevGetFeatureInfo(cam.g_hCamera, "TriggerActivation", featureInfo) == IKapC.ITKSTATUS_OK)
            {
                cam.g_configMethod[IKCamera.ITEK_CAMERA_TRIGGER_POLARITY_CONFIG] = IKCamera.ITEK_CAMERA_TRIGGER_POLARITY_CONFIG_METHOD_4;
            }

            // multi exposure
            if (IKapC.ItkDevGetFeatureInfo(cam.g_hCamera, "MultiExposureTimeCount", featureInfo) == IKapC.ITKSTATUS_OK)
            {
                cam.g_configMethod[IKCamera.ITEK_CAMERA_MULTI_EXPOSURE_CONFIG] = IKCamera.ITEK_CAMERA_MULTI_EXPOSURE_CONFIG_METHOD_1;
            }

            // chunk data
            if (IKapC.ItkDevGetFeatureInfo(cam.g_hCamera, "ChunkModeActive", featureInfo) == IKapC.ITKSTATUS_OK &&
                IKapC.ItkDevGetFeatureInfo(cam.g_hCamera, "ChunkSelector", featureInfo) == IKapC.ITKSTATUS_OK &&
                IKapC.ItkDevGetFeatureInfo(cam.g_hCamera, "ChunkEnable", featureInfo) == IKapC.ITKSTATUS_OK)
            {
                cam.g_configMethod[IKCamera.ITEK_CAMERA_CHUNK_DATA_CONFIG] = IKCamera.ITEK_CAMERA_CHUNK_DATA_CONFIG_METHOD_1;
            }
        }

        static public uint SetWidth(IKCamera cam, int width)
        {
            uint res = IKapC.ITKSTATUS_OK;
            int ret = IKapBoard.IK_RTN_OK;
            ITKFEATURE pFea = null;
            uint accMode = (uint)ITKFEATURE_VAL_ACCESS_MODE_LIST.ITKFEATURE_VAL_ACCESS_MODE_NA;
            IKapC.ItkDevGetAccessMode(cam.g_hCamera, "Width", ref accMode);
            if ((uint)ITKFEATURE_VAL_ACCESS_MODE_LIST.ITKFEATURE_VAL_ACCESS_MODE_RW == accMode)
            {
                return IKapC.ItkDevSetInt64(cam.g_hCamera, "Width", width);
            }
            switch (cam.g_configMethod[IKCamera.ITEK_CAMERA_WIDTH_CONFIG])
            {
                case IKCamera.ITEK_CAMERA_WIDTH_CONFIG_METHOD_1:
                    res = IKapC.ItkDevSetBool(cam.g_hCamera, "RegionEnable", true);
                    if (IKapC.ITKSTATUS_OK != res)
                        return res;
                    res = IKapC.ItkDevSetInt64(cam.g_hCamera, "RegionWidth", width);
                    if (IKapC.ITKSTATUS_OK != res)
                        return res;

                    break;
                case IKCamera.ITEK_CAMERA_WIDTH_CONFIG_METHOD_2:
                    res = IKapC.ItkDevSetBool(cam.g_hCamera, "ROIEnable", true);
                    if (IKapC.ITKSTATUS_OK != res)
                        return res;
                    res = IKapC.ItkDevSetInt64(cam.g_hCamera, "ROIWidth", width);
                    if (IKapC.ITKSTATUS_OK != res)
                        return res;

                    break;
                case IKCamera.ITEK_CAMERA_WIDTH_CONFIG_METHOD_3:
                    res = IKapC.ItkDevSetInt64(cam.g_hCamera, "RegionWidth", width);
                    if (IKapC.ITKSTATUS_OK != res)
                        return res;

                    break;
                case IKCamera.ITEK_CAMERA_WIDTH_CONFIG_METHOD_4:
                    res = IKapC.ItkDevSetInt64(cam.g_hCamera, "ROIWidth", width);
                    if (IKapC.ITKSTATUS_OK != res)
                        return res;

                    break;
                default:
                    return IKapC.ITKSTATUS_INVALID_ARG;
            }

            if (cam.g_hBoard != IntPtr.Zero)
            {
                ret = IKapBoard.IKapSetInfo(cam.g_hBoard, (uint)IKapBoard.IKP_IMAGE_WIDTH, width);
                if (ret != IKapBoard.IK_RTN_OK)
                {
                    return IKapC.ITKSTATUS_INVALID_ARG;
                }
            }
            return res;
        }

        static public uint GetWidth(IKCamera cam, ref long width)
        {
            return IKapC.ItkDevGetInt64(cam.g_hCamera, "Width", ref width);
        }

        static public uint SetHeight(IKCamera cam, int height)
        {
            uint res = IKapC.ITKSTATUS_OK;
            int ret = IKapBoard.IK_RTN_OK;
            ITKFEATURE pFea = null;
            uint accMode = (uint)ITKFEATURE_VAL_ACCESS_MODE_LIST.ITKFEATURE_VAL_ACCESS_MODE_NA;
            IKapC.ItkDevGetAccessMode(cam.g_hCamera, "Height", ref accMode);
            if ((uint)ITKFEATURE_VAL_ACCESS_MODE_LIST.ITKFEATURE_VAL_ACCESS_MODE_RW == accMode)
            {
                return IKapC.ItkDevSetInt64(cam.g_hCamera, "Height", height);
            }

            switch (cam.g_configMethod[IKCamera.ITEK_CAMERA_WIDTH_CONFIG])
            {
                case IKCamera.ITEK_CAMERA_HEIGHT_CONFIG_METHOD_1:
                    res = IKapC.ItkDevSetBool(cam.g_hCamera, "RegionEnable", true);
                    if (IKapC.ITKSTATUS_OK != res)
                        return res;
                    res = IKapC.ItkDevSetInt64(cam.g_hCamera, "RegionHeight", height);
                    if (IKapC.ITKSTATUS_OK != res)
                        return res;

                    break;
                case IKCamera.ITEK_CAMERA_HEIGHT_CONFIG_METHOD_2:
                    res = IKapC.ItkDevSetBool(cam.g_hCamera, "ROIEnable", true);
                    if (IKapC.ITKSTATUS_OK != res)
                        return res;
                    res = IKapC.ItkDevSetInt64(cam.g_hCamera, "ROIHeight", height);
                    if (IKapC.ITKSTATUS_OK != res)
                        return res;

                    break;
                case IKCamera.ITEK_CAMERA_HEIGHT_CONFIG_METHOD_3:
                    res = IKapC.ItkDevSetInt64(cam.g_hCamera, "RegionHeight", height);
                    if (IKapC.ITKSTATUS_OK != res)
                        return res;

                    break;
                case IKCamera.ITEK_CAMERA_HEIGHT_CONFIG_METHOD_4:
                    res = IKapC.ItkDevSetInt64(cam.g_hCamera, "ROIHeight", height);
                    if (IKapC.ITKSTATUS_OK != res)
                        return res;

                    break;
                default:
                    return IKapC.ITKSTATUS_INVALID_ARG;
            }
            if (cam.g_hBoard != IntPtr.Zero)
            {
                ret = IKapBoard.IKapSetInfo(cam.g_hBoard, (uint)IKapBoard.IKP_IMAGE_HEIGHT, height);
                if (ret != IKapBoard.IK_RTN_OK)
                {
                    return IKapC.ITKSTATUS_INVALID_ARG;
                }
            }
            return res;
        }

        static public uint GetHeight(IKCamera cam, ref long height)
        {
            return IKapC.ItkDevGetInt64(cam.g_hCamera, "Height", ref height);
        }

        static public uint SetPixelFormat(IKCamera cam, string pixelFormat)
        {
            return IKapC.ItkDevFromString(cam.g_hCamera, "PixelFormat", pixelFormat);
        }

        static public uint GetPixelFormat(IKCamera cam, StringBuilder pBuffer, ref uint count)
        {
            return IKapC.ItkDevToString(cam.g_hCamera, "PixelFormat", pBuffer, ref count);
        }

        static public uint SetExposureTime(IKCamera cam, double exposureTime)
        {
            uint res = IKapC.ITKSTATUS_OK;
            long exposureTime_int64 = (long)exposureTime;
            bool areaMode = false;
            uint ExposureTimeType = (uint)ITKFEATURE_VAL_TYPE_LIST.ITKFEATURE_VAL_TYPE_UNDEFINED;
            uint AreaExposureTimeType = (uint)ITKFEATURE_VAL_TYPE_LIST.ITKFEATURE_VAL_TYPE_UNDEFINED;
            if (IKCamera.ITEK_CAMERA_EXPOSURE_TIME_CONFIG_METHOD_2 == cam.g_configMethod[IKCamera.ITEK_CAMERA_EXPOSURE_TIME_CONFIG] ||
                IKCamera.ITEK_CAMERA_EXPOSURE_TIME_CONFIG_METHOD_4 == cam.g_configMethod[IKCamera.ITEK_CAMERA_EXPOSURE_TIME_CONFIG])
            {
                ITK_FEATURE_BASE_INFO featureInfo = new ITK_FEATURE_BASE_INFO();
                res = IKapC.ItkDevGetFeatureInfo(cam.g_hCamera, "ExposureTime", featureInfo);
                if (IKapC.ITKSTATUS_OK != res)
                    return res;
                ExposureTimeType = featureInfo.Type;
            }
            if (IKCamera.ITEK_CAMERA_EXPOSURE_TIME_CONFIG_METHOD_3 == cam.g_configMethod[IKCamera.ITEK_CAMERA_EXPOSURE_TIME_CONFIG] ||
                IKCamera.ITEK_CAMERA_EXPOSURE_TIME_CONFIG_METHOD_5 == cam.g_configMethod[IKCamera.ITEK_CAMERA_EXPOSURE_TIME_CONFIG])
            {
                ITK_FEATURE_BASE_INFO featureInfo = new ITK_FEATURE_BASE_INFO();
                res = IKapC.ItkDevGetFeatureInfo(cam.g_hCamera, "AreaExposureTime", featureInfo);
                if (IKapC.ITKSTATUS_OK != res)
                    return res;
                AreaExposureTimeType = featureInfo.Type;
            }

            switch (cam.g_configMethod[IKCamera.ITEK_CAMERA_EXPOSURE_TIME_CONFIG])
            {
                case IKCamera.ITEK_CAMERA_EXPOSURE_TIME_CONFIG_METHOD_1:
                    return IKapC.ItkDevSetInt64(cam.g_hCamera, "ExposureTimeRaw", exposureTime_int64);
                case IKCamera.ITEK_CAMERA_EXPOSURE_TIME_CONFIG_METHOD_2:
                    res = IKapC.ItkDevGetBool(cam.g_hCamera, "AreaMode", ref areaMode);
                    if (IKapC.ITKSTATUS_OK != res)
                        return res;
                    if (false == areaMode)
                        return IKapC.ITKSTATUS_INVALID_ARG;

                    if ((uint)ITKFEATURE_VAL_TYPE_LIST.ITKFEATURE_VAL_TYPE_INT64 == ExposureTimeType)
                    {
                        return IKapC.ItkDevSetInt64(cam.g_hCamera, "ExposureTime", exposureTime_int64);
                    }
                    else if ((uint)ITKFEATURE_VAL_TYPE_LIST.ITKFEATURE_VAL_TYPE_DOUBLE == ExposureTimeType)
                    {
                        return IKapC.ItkDevSetDouble(cam.g_hCamera, "ExposureTime", exposureTime);
                    }
                    break;
                case IKCamera.ITEK_CAMERA_EXPOSURE_TIME_CONFIG_METHOD_3:
                    res = IKapC.ItkDevGetBool(cam.g_hCamera, "AreaMode", ref areaMode);
                    if (IKapC.ITKSTATUS_OK != res)
                        return res;
                    if (false == areaMode)
                        return IKapC.ITKSTATUS_INVALID_ARG;

                    if ((uint)ITKFEATURE_VAL_TYPE_LIST.ITKFEATURE_VAL_TYPE_INT64 == AreaExposureTimeType)
                    {
                        return IKapC.ItkDevSetInt64(cam.g_hCamera, "AreaExposureTime", exposureTime_int64);
                    }
                    else if ((uint)ITKFEATURE_VAL_TYPE_LIST.ITKFEATURE_VAL_TYPE_DOUBLE == ExposureTimeType)
                    {
                        return IKapC.ItkDevSetDouble(cam.g_hCamera, "AreaExposureTime", exposureTime);
                    }
                    break;
                case IKCamera.ITEK_CAMERA_EXPOSURE_TIME_CONFIG_METHOD_4:

                    if ((uint)ITKFEATURE_VAL_TYPE_LIST.ITKFEATURE_VAL_TYPE_INT64 == ExposureTimeType)
                    {
                        return IKapC.ItkDevSetInt64(cam.g_hCamera, "ExposureTime", exposureTime_int64);
                    }
                    else if ((uint)ITKFEATURE_VAL_TYPE_LIST.ITKFEATURE_VAL_TYPE_DOUBLE == ExposureTimeType)
                    {
                        return IKapC.ItkDevSetDouble(cam.g_hCamera, "ExposureTime", exposureTime);
                    }
                    break;
                case IKCamera.ITEK_CAMERA_EXPOSURE_TIME_CONFIG_METHOD_5:
                    res = IKapC.ItkDevGetBool(cam.g_hCamera, "Area", ref areaMode);
                    if (IKapC.ITKSTATUS_OK != res)
                        return res;
                    if (false == areaMode)
                        return IKapC.ITKSTATUS_INVALID_ARG;

                    if ((uint)ITKFEATURE_VAL_TYPE_LIST.ITKFEATURE_VAL_TYPE_INT64 == AreaExposureTimeType)
                    {
                        return IKapC.ItkDevSetInt64(cam.g_hCamera, "AreaExposureTime", exposureTime_int64);
                    }
                    else if ((uint)ITKFEATURE_VAL_TYPE_LIST.ITKFEATURE_VAL_TYPE_DOUBLE == ExposureTimeType)
                    {
                        return IKapC.ItkDevSetDouble(cam.g_hCamera, "AreaExposureTime", exposureTime);
                    }
                    break;
                default:
                    break;
            }
            return IKapC.ITKSTATUS_INVALID_ARG;
        }

        static public uint GetExposureTime(IKCamera cam, ref double exposureTime)
        {
            uint res = IKapC.ITKSTATUS_OK;

            uint ExposureTimeType = (uint)ITKFEATURE_VAL_TYPE_LIST.ITKFEATURE_VAL_TYPE_UNDEFINED;
            uint AreaExposureTimeType = (uint)ITKFEATURE_VAL_TYPE_LIST.ITKFEATURE_VAL_TYPE_UNDEFINED;
            if (IKCamera.ITEK_CAMERA_EXPOSURE_TIME_CONFIG_METHOD_2 == cam.g_configMethod[IKCamera.ITEK_CAMERA_EXPOSURE_TIME_CONFIG] ||
                IKCamera.ITEK_CAMERA_EXPOSURE_TIME_CONFIG_METHOD_4 == cam.g_configMethod[IKCamera.ITEK_CAMERA_EXPOSURE_TIME_CONFIG])
            {
                ITK_FEATURE_BASE_INFO featureInfo = new ITK_FEATURE_BASE_INFO();
                res = IKapC.ItkDevGetFeatureInfo(cam.g_hCamera, "ExposureTime", featureInfo);
                if (IKapC.ITKSTATUS_OK != res)
                    return res;
                ExposureTimeType = featureInfo.Type;
            }
            if (IKCamera.ITEK_CAMERA_EXPOSURE_TIME_CONFIG_METHOD_3 == cam.g_configMethod[IKCamera.ITEK_CAMERA_EXPOSURE_TIME_CONFIG] || IKCamera.ITEK_CAMERA_EXPOSURE_TIME_CONFIG_METHOD_5 == cam.g_configMethod[IKCamera.ITEK_CAMERA_EXPOSURE_TIME_CONFIG])
            {
                ITK_FEATURE_BASE_INFO featureInfo = new ITK_FEATURE_BASE_INFO();
                res = IKapC.ItkDevGetFeatureInfo(cam.g_hCamera, "AreaExposureTime", featureInfo);
                if (IKapC.ITKSTATUS_OK != res)
                    return res;
                AreaExposureTimeType = featureInfo.Type;
            }

            long expTime = 0;
            switch (cam.g_configMethod[IKCamera.ITEK_CAMERA_EXPOSURE_TIME_CONFIG])
            {
                case IKCamera.ITEK_CAMERA_EXPOSURE_TIME_CONFIG_METHOD_1:
                    res = IKapC.ItkDevGetInt64(cam.g_hCamera, "ExposureTimeRaw", ref expTime);
                    if (res == IKapC.ITKSTATUS_OK)
                    {
                        exposureTime = expTime;
                    }
                    break;
                case IKCamera.ITEK_CAMERA_EXPOSURE_TIME_CONFIG_METHOD_2:
                case IKCamera.ITEK_CAMERA_EXPOSURE_TIME_CONFIG_METHOD_4:
                    if ((uint)ITKFEATURE_VAL_TYPE_LIST.ITKFEATURE_VAL_TYPE_INT64 == ExposureTimeType)
                    {
                        res = IKapC.ItkDevGetInt64(cam.g_hCamera, "ExposureTime", ref expTime);
                        if (res == IKapC.ITKSTATUS_OK)
                        {
                            exposureTime = expTime;
                        }
                    }
                    else if ((uint)ITKFEATURE_VAL_TYPE_LIST.ITKFEATURE_VAL_TYPE_DOUBLE == ExposureTimeType)
                    {
                        res = IKapC.ItkDevGetDouble(cam.g_hCamera, "ExposureTime", ref exposureTime);
                    }
                    else
                    {
                        return IKapC.ITKSTATUS_INVALID_ARG;
                    }
                    break;
                case IKCamera.ITEK_CAMERA_EXPOSURE_TIME_CONFIG_METHOD_3:
                case IKCamera.ITEK_CAMERA_EXPOSURE_TIME_CONFIG_METHOD_5:
                    if ((uint)ITKFEATURE_VAL_TYPE_LIST.ITKFEATURE_VAL_TYPE_INT64 == AreaExposureTimeType)
                    {
                        res = IKapC.ItkDevGetInt64(cam.g_hCamera, "AreaExposureTime", ref expTime);
                        if (res == IKapC.ITKSTATUS_OK)
                        {
                            exposureTime = expTime;
                        }
                    }
                    else if ((uint)ITKFEATURE_VAL_TYPE_LIST.ITKFEATURE_VAL_TYPE_DOUBLE == AreaExposureTimeType)
                    {
                        res = IKapC.ItkDevGetDouble(cam.g_hCamera, "AreaExposureTime", ref exposureTime);
                    }
                    else
                    {
                        return IKapC.ITKSTATUS_INVALID_ARG;
                    }
                    break;
                default:
                    break;
            }

            return res;
        }

        static public uint SetDigitalGain(IKCamera cam, double digitalGain)
        {
            uint res = IKapC.ITKSTATUS_OK;
            switch (cam.g_configMethod[IKCamera.ITEK_CAMERA_DIGITAL_GAIN_CONFIG])
            {
                case IKCamera.ITEK_CAMERA_DIGITAL_GAIN_CONFIG_METHOD_1:
                    return IKapC.ItkDevSetDouble(cam.g_hCamera, "DigitalGain", digitalGain);
                case IKCamera.ITEK_CAMERA_DIGITAL_GAIN_CONFIG_METHOD_2:
                    return IKapC.ItkDevSetDouble(cam.g_hCamera, "Gain", digitalGain);
                default:
                    break;
            }

            return IKapC.ITKSTATUS_INVALID_ARG;
        }

        static public uint GetDigitalGain(IKCamera cam, ref double digitalGain)
        {
            uint res = IKapC.ITKSTATUS_OK;
            switch (cam.g_configMethod[IKCamera.ITEK_CAMERA_DIGITAL_GAIN_CONFIG])
            {
                case IKCamera.ITEK_CAMERA_DIGITAL_GAIN_CONFIG_METHOD_1:
                    return IKapC.ItkDevGetDouble(cam.g_hCamera, "DigitalGain", ref digitalGain);
                case IKCamera.ITEK_CAMERA_DIGITAL_GAIN_CONFIG_METHOD_2:
                    return IKapC.ItkDevGetDouble(cam.g_hCamera, "Gain", ref digitalGain);
                default:
                    break;
            }
            return IKapC.ITKSTATUS_INVALID_ARG;
        }

        static public uint SetPixelGain(IKCamera cam, string pixelGain)
        {
            uint res = IKapC.ITKSTATUS_OK;
            return IKapC.ItkDevFromString(cam.g_hCamera, "PixelGain", pixelGain);
        }

        static public uint GetPixelGain(IKCamera cam, StringBuilder pixelGain, ref uint len)
        {
            uint res = IKapC.ITKSTATUS_OK;
            return IKapC.ItkDevToString(cam.g_hCamera, "PixelGain", pixelGain, ref len);
        }

        static public uint SetLineRate(IKCamera cam, double lineRate)
        {
            uint res = IKapC.ITKSTATUS_OK;
            if (lineRate <= 0)
                return IKapC.ITKSTATUS_INVALID_ARG;

            double linePeriodTime = 1000000 / lineRate;  // us
            switch (cam.g_configMethod[IKCamera.ITEK_CAMERA_LINE_RATE_CONFIG])
            {
                case IKCamera.ITEK_CAMERA_LINE_RATE_CONFIG_METHOD_1:
                    return IKapC.ItkDevSetDouble(cam.g_hCamera, "LinePeriodTime", linePeriodTime);
                case IKCamera.ITEK_CAMERA_LINE_RATE_CONFIG_METHOD_2:
                    return IKapC.ItkDevSetDouble(cam.g_hCamera, "AcquisitionLinePeriod", linePeriodTime);
                case IKCamera.ITEK_CAMERA_LINE_RATE_CONFIG_METHOD_3:
                    return IKapC.ItkDevSetDouble(cam.g_hCamera, "AcquisitionLineRate", lineRate);
                default:
                    break;
            }
            return IKapC.ITKSTATUS_INVALID_ARG;
        }

        static public uint GetLineRate(IKCamera cam, ref double lineRate)
        {
            uint res = IKapC.ITKSTATUS_OK;
            double linePeriodTime = 0;
            switch (cam.g_configMethod[IKCamera.ITEK_CAMERA_LINE_RATE_CONFIG])
            {
                case IKCamera.ITEK_CAMERA_LINE_RATE_CONFIG_METHOD_1:
                    res = IKapC.ItkDevGetDouble(cam.g_hCamera, "LinePeriodTime", ref linePeriodTime);
                    if (IKapC.ITKSTATUS_OK != res)
                        return res;
                    lineRate = 1000000 / linePeriodTime;
                    return res;
                case IKCamera.ITEK_CAMERA_LINE_RATE_CONFIG_METHOD_2:
                    res = IKapC.ItkDevGetDouble(cam.g_hCamera, "AcquisitionLinePeriod", ref linePeriodTime);
                    if (IKapC.ITKSTATUS_OK != res)
                        return res;
                    lineRate = 1000000 / linePeriodTime;
                    return res;
                case IKCamera.ITEK_CAMERA_LINE_RATE_CONFIG_METHOD_3:
                    return IKapC.ItkDevGetDouble(cam.g_hCamera, "AcquisitionLineRate", ref lineRate);
                default:
                    break;
            }

            return IKapC.ITKSTATUS_INVALID_ARG;
        }

        static public uint SetFrameRate(IKCamera cam, double frameRate)
        {
            uint res = IKapC.ITKSTATUS_OK;
            if (frameRate <= 0)
            {
                return IKapC.ITKSTATUS_INVALID_ARG;
            }
            double framePeriod = 1000000 / frameRate;
            long framePeriod_int64 = (long)framePeriod;
            switch (cam.g_configMethod[IKCamera.ITEK_CAMERA_FRAME_RATE_CONFIG])
            {
                case IKCamera.ITEK_CAMERA_FRAME_RATE_CONFIG_METHOD_1:
                    return IKapC.ItkDevSetInt64(cam.g_hCamera, "AcquisitionFramePeriodRaw", framePeriod_int64);
                case IKCamera.ITEK_CAMERA_FRAME_RATE_CONFIG_METHOD_2:
                    return IKapC.ItkDevSetInt64(cam.g_hCamera, "AcquisitionFramePeriod", framePeriod_int64);
                case IKCamera.ITEK_CAMERA_FRAME_RATE_CONFIG_METHOD_3:
                    return IKapC.ItkDevSetDouble(cam.g_hCamera, "AcquisitionFramePeriod", framePeriod);
                case IKCamera.ITEK_CAMERA_FRAME_RATE_CONFIG_METHOD_4:
                    return IKapC.ItkDevSetInt64(cam.g_hCamera, "FramePeriodTime", framePeriod_int64);
                case IKCamera.ITEK_CAMERA_FRAME_RATE_CONFIG_METHOD_5:
                    res = IKapC.ItkDevSetBool(cam.g_hCamera, "AcquisitionFrameRateEnable", true);
                    if (IKapC.ITKSTATUS_OK != res)
                        return res;
                    return IKapC.ItkDevSetDouble(cam.g_hCamera, "AcquisitionFrameRate", frameRate);
                case IKCamera.ITEK_CAMERA_FRAME_RATE_CONFIG_METHOD_6:
                    return IKapC.ItkDevSetDouble(cam.g_hCamera, "AcquisitionFrameRate", frameRate);
                default:
                    break;
            }
            return IKapC.ITKSTATUS_INVALID_ARG;
        }

        static public uint GetFrameRate(IKCamera cam, ref double frameRate)
        {
            uint res = IKapC.ITKSTATUS_OK;
            long framePeriod_int64 = 0;
            double framePeriod = 0;
            switch (cam.g_configMethod[IKCamera.ITEK_CAMERA_FRAME_RATE_CONFIG])
            {
                case IKCamera.ITEK_CAMERA_FRAME_RATE_CONFIG_METHOD_1:
                    res = IKapC.ItkDevGetInt64(cam.g_hCamera, "AcquisitionFramePeriodRaw", ref framePeriod_int64);
                    if (framePeriod_int64 > 0)
                    {
                        frameRate = (double)1000000 / (double)framePeriod_int64;
                    }
                    return res;
                case IKCamera.ITEK_CAMERA_FRAME_RATE_CONFIG_METHOD_2:
                    res = IKapC.ItkDevGetInt64(cam.g_hCamera, "AcquisitionFramePeriod", ref framePeriod_int64);
                    if (framePeriod_int64 > 0)
                    {
                        frameRate = (double)1000000 / (double)framePeriod_int64;
                    }
                    return res;
                case IKCamera.ITEK_CAMERA_FRAME_RATE_CONFIG_METHOD_3:
                    res = IKapC.ItkDevGetDouble(cam.g_hCamera, "AcquisitionFramePeriod", ref framePeriod);
                    if (framePeriod > 0)
                    {
                        frameRate = 1000000 / framePeriod;
                    }
                    return res;
                case IKCamera.ITEK_CAMERA_FRAME_RATE_CONFIG_METHOD_4:
                    res = IKapC.ItkDevGetInt64(cam.g_hCamera, "FramePeriodTime", ref framePeriod_int64);
                    if (framePeriod_int64 > 0)
                    {
                        frameRate = (double)1000000 / (double)framePeriod_int64;
                    }
                    return res;
                case IKCamera.ITEK_CAMERA_FRAME_RATE_CONFIG_METHOD_5:
                case IKCamera.ITEK_CAMERA_FRAME_RATE_CONFIG_METHOD_6:
                    return IKapC.ItkDevGetDouble(cam.g_hCamera, "AcquisitionFrameRate", ref frameRate);
                default:
                    break;
            }
            return IKapC.ITKSTATUS_INVALID_ARG;
        }

        static public uint SetFrameTrigger(IKCamera cam, string status, bool level)
        {
            uint res = IKapC.ITKSTATUS_OK;
            bool bSet = false;
            if (String.Compare(status, "On") == 0)
            {
                bSet = true;
            }
            if (level)
            {
                switch (cam.g_configMethod[IKCamera.ITEK_CAMERA_FRAME_TRIGGER_CONFIG])
                {
                    case IKCamera.ITEK_CAMERA_FRAME_TRIGGER_CONFIG_METHOD_1:
                        res = IKapC.ItkDevFromString(cam.g_hCamera, "TriggerSelector", "FrameActive");
                        if (IKapC.ITKSTATUS_OK != res)
                            return res;
                        return IKapC.ItkDevFromString(cam.g_hCamera, "TriggerMode", status);
                    case IKCamera.ITEK_CAMERA_FRAME_TRIGGER_CONFIG_METHOD_2:
                        res = IKapC.ItkDevSetBool(cam.g_hCamera, "FrameTriggerEnable", bSet);
                        if (IKapC.ITKSTATUS_OK != res)
                            return res;
                        return IKapC.ItkDevFromString(cam.g_hCamera, "FrameTriggerType", "LevelSensitive");
                    case IKCamera.ITEK_CAMERA_FRAME_TRIGGER_CONFIG_METHOD_3:
                        if (bSet)
                        {
                            return IKapC.ItkDevFromString(cam.g_hCamera, "ExposureMode", "TriggerWidth");
                        }
                        else
                        {
                            return IKapC.ItkDevFromString(cam.g_hCamera, "ExposureMode", "Timed");
                        }
                    default:
                        break;
                }
            }
            else
            {
                switch (cam.g_configMethod[IKCamera.ITEK_CAMERA_FRAME_TRIGGER_CONFIG])
                {
                    case IKCamera.ITEK_CAMERA_FRAME_TRIGGER_CONFIG_METHOD_1:
                        res = IKapC.ItkDevFromString(cam.g_hCamera, "TriggerSelector", "FrameStart");
                        if (IKapC.ITKSTATUS_OK != res)
                            return res;
                        return IKapC.ItkDevFromString(cam.g_hCamera, "TriggerMode", status);
                    case IKCamera.ITEK_CAMERA_FRAME_TRIGGER_CONFIG_METHOD_2:
                        res = IKapC.ItkDevSetBool(cam.g_hCamera, "FrameTriggerEnable", bSet);
                        if (IKapC.ITKSTATUS_OK != res)
                            return res;
                        return IKapC.ItkDevFromString(cam.g_hCamera, "FrameTriggerType", "EdgeSensitive");
                    case IKCamera.ITEK_CAMERA_FRAME_TRIGGER_CONFIG_METHOD_3:
                        if (bSet)
                        {
                            return IKapC.ItkDevFromString(cam.g_hCamera, "ExposureMode", "TriggerPulse");
                        }
                        else
                        {
                            return IKapC.ItkDevFromString(cam.g_hCamera, "ExposureMode", "Timed");
                        }
                    default:
                        break;
                }
            }
            return IKapC.ITKSTATUS_INVALID_ARG;
        }

        static public uint SetLineTrigger(IKCamera cam, string status, bool level)
        {
            uint res = IKapC.ITKSTATUS_OK;
            bool bSet = false;
            if (String.Compare(status, "On") == 0)
            {
                bSet = true;
            }
            if (level)
            {
                switch (cam.g_configMethod[IKCamera.ITEK_CAMERA_LINE_TRIGGER_CONFIG])
                {
                    case IKCamera.ITEK_CAMERA_LINE_TRIGGER_CONFIG_METHOD_1:
                        res = IKapC.ItkDevFromString(cam.g_hCamera, "TriggerSelector", "LineStart");
                        if (IKapC.ITKSTATUS_OK != res)
                            return res;
                        break;
                    case IKCamera.ITEK_CAMERA_LINE_TRIGGER_CONFIG_METHOD_2:
                        res = IKapC.ItkDevFromString(cam.g_hCamera, "TriggerMode", status);
                        if (IKapC.ITKSTATUS_OK != res)
                            return res;
                        if (bSet)
                        {
                            return IKapC.ItkDevFromString(cam.g_hCamera, "ExposureMode", "TriggerWidth");
                        }
                        else
                        {
                            return IKapC.ItkDevFromString(cam.g_hCamera, "ExposureMode", "Timed");
                        }
                    case IKCamera.ITEK_CAMERA_LINE_TRIGGER_CONFIG_METHOD_3:
                        if (bSet)
                        {
                            return IKapC.ItkDevFromString(cam.g_hCamera, "SynchronizationMode", "TriggerWidth");
                        }
                        else
                        {
                            return IKapC.ItkDevFromString(cam.g_hCamera, "SynchronizationMode", "Timed");
                        }
                    default:
                        break;
                }
            }
            else
            {
                switch (cam.g_configMethod[IKCamera.ITEK_CAMERA_LINE_TRIGGER_CONFIG])
                {
                    case IKCamera.ITEK_CAMERA_LINE_TRIGGER_CONFIG_METHOD_1:
                        res = IKapC.ItkDevFromString(cam.g_hCamera, "TriggerSelector", "LineStart");
                        if (IKapC.ITKSTATUS_OK != res)
                            return res;
                        break;
                    case IKCamera.ITEK_CAMERA_LINE_TRIGGER_CONFIG_METHOD_2:
                        res = IKapC.ItkDevFromString(cam.g_hCamera, "TriggerMode", status);
                        if (IKapC.ITKSTATUS_OK != res)
                            return res;
                        return IKapC.ItkDevFromString(cam.g_hCamera, "ExposureMode", "Timed");
                    case IKCamera.ITEK_CAMERA_LINE_TRIGGER_CONFIG_METHOD_3:
                        if (bSet)
                        {
                            return IKapC.ItkDevFromString(cam.g_hCamera, "SynchronizationMode", "TriggerPulse");
                        }
                        else
                        {
                            return IKapC.ItkDevFromString(cam.g_hCamera, "SynchronizationMode", "Timed");
                        }
                    default:
                        break;
                }
            }
            return IKapC.ITKSTATUS_INVALID_ARG;
        }

        // static public uint SetTriggerMode(IKCamera cam, const char *triggerSelector,
        //                         const char *exposureMode) {
        //  uint res = IKapC.ITKSTATUS_OK;
        //  if (triggerSelector) {
        //    res = IKapC.ItkDevFromString(cam.g_hCamera, "TriggerSelector", triggerSelector);
        //    if (res != IKapC.ITKSTATUS_OK) {
        //      return res;
        //    }
        //  }
        //  res = IKapC.ItkDevFromString(cam.g_hCamera, "TriggerMode", "On");
        //  if (IKapC.ITKSTATUS_OK == res)
        //    return res;
        //  res = ItkDevSetBool(cam.g_hCamera, "FrameTriggerEnable", true);
        //  if (IKapC.ITKSTATUS_OK == res)
        //	  return res;
        //  res = IKapC.ItkDevFromString(cam.g_hCamera, "ExposureMode", exposureMode);
        //  if (IKapC.ITKSTATUS_OK == res)
        //	  return res;
        //  return IKapC.ItkDevFromString(cam.g_hCamera, "SynchronizationMode", exposureMode);
        //}
        //
        // uint GetTriggerMode(ref IKCamera cam, StringBuilder triggerSelector, uint triggerSelectorLen, StringBuilder
        // TriggerModeOrexposureMode, uint Len, bool* frameTriggerEnable)
        //{
        //	uint res = IKapC.ITKSTATUS_OK;
        //	if (triggerSelector)
        //	{
        //          res = IKapC.ItkDevToString(cam.g_hCamera, "TriggerSelector",
        //                               triggerSelector, triggerSelectorLen);
        //		  if (res!=IKapC.ITKSTATUS_OK)
        //		  {
        //			  return res;
        //		  }
        //	}
        //	res = IKapC.ItkDevToString(cam.g_hCamera, "TriggerMode", TriggerModeOrexposureMode,Len);
        //	if (res==IKapC.ITKSTATUS_OK)
        //	{
        //		return res;
        //	}
        //	if (frameTriggerEnable)
        //	{
        //	if (ItkDevGetBool(cam.g_hCamera, "FrameTriggerEnable", frameTriggerEnable)==IKapC.ITKSTATUS_OK) {
        //		return IKapC.ITKSTATUS_OK;
        //	}
        //	}
        //
        //	if (IKapC.ItkDevToString(cam.g_hCamera,"ExposureMode", TriggerModeOrexposureMode,Len) == IKapC.ITKSTATUS_OK ||
        // IKapC.ItkDevToString(cam.g_hCamera, "SynchronizationMode", TriggerModeOrexposureMode, Len) == IKapC.ITKSTATUS_OK)
        //	{
        //		return IKapC.ITKSTATUS_OK;
        //	}
        //	return IKapC.ITKSTATUS_INVALID_ARG;
        //}

        static public uint SetTriggerSource(IKCamera cam, string triggerSource)
        {
            uint res = IKapC.ITKSTATUS_OK;
            switch (cam.g_configMethod[IKCamera.ITEK_CAMERA_FRAME_TRIGGER_CONFIG_METHOD_1])
            {
                case IKCamera.ITEK_CAMERA_TRIGGER_SOURCE_CONFIG_METHOD_1:
                    return IKapC.ItkDevFromString(cam.g_hCamera, "FrameTriggerSource", triggerSource);
                case IKCamera.ITEK_CAMERA_TRIGGER_SOURCE_CONFIG_METHOD_2:
                    return IKapC.ItkDevFromString(cam.g_hCamera, "ExternalTriggerSource", triggerSource);
                case IKCamera.ITEK_CAMERA_TRIGGER_SOURCE_CONFIG_METHOD_3:
                    return IKapC.ItkDevFromString(cam.g_hCamera, "TriggerSource", triggerSource);
                default:
                    break;
            }
            return IKapC.ITKSTATUS_INVALID_ARG;
        }

        static public uint GetTriggerSource(IKCamera cam, StringBuilder triggerSource, ref uint len)
        {
            uint res = IKapC.ITKSTATUS_OK;
            switch (cam.g_configMethod[IKCamera.ITEK_CAMERA_FRAME_TRIGGER_CONFIG_METHOD_1])
            {
                case IKCamera.ITEK_CAMERA_TRIGGER_SOURCE_CONFIG_METHOD_1:
                    return IKapC.ItkDevToString(cam.g_hCamera, "FrameTriggerSource", triggerSource, ref len);
                case IKCamera.ITEK_CAMERA_TRIGGER_SOURCE_CONFIG_METHOD_2:
                    return IKapC.ItkDevToString(cam.g_hCamera, "ExternalTriggerSource", triggerSource, ref len);
                case IKCamera.ITEK_CAMERA_TRIGGER_SOURCE_CONFIG_METHOD_3:
                    return IKapC.ItkDevToString(cam.g_hCamera, "TriggerSource", triggerSource, ref len);
                default:
                    break;
            }
            return IKapC.ITKSTATUS_INVALID_ARG;
        }

        static public uint SetTriggerPolarity(IKCamera cam, string triggerPolarity)
        {
            uint res = IKapC.ITKSTATUS_OK;
            switch (cam.g_configMethod[IKCamera.ITEK_CAMERA_TRIGGER_POLARITY_CONFIG])
            {
                case IKCamera.ITEK_CAMERA_TRIGGER_POLARITY_CONFIG_METHOD_1:
                    return IKapC.ItkDevFromString(cam.g_hCamera, "FrameTriggerPolarity", triggerPolarity);
                case IKCamera.ITEK_CAMERA_TRIGGER_POLARITY_CONFIG_METHOD_2:
                    return IKapC.ItkDevFromString(cam.g_hCamera, "ExternalTriggerActivation", triggerPolarity);
                case IKCamera.ITEK_CAMERA_TRIGGER_POLARITY_CONFIG_METHOD_3:
                    return IKapC.ItkDevFromString(cam.g_hCamera, "LinePolarity", triggerPolarity);
                case IKCamera.ITEK_CAMERA_TRIGGER_POLARITY_CONFIG_METHOD_4:
                    return IKapC.ItkDevFromString(cam.g_hCamera, "TriggerActivation", triggerPolarity);
                default:
                    break;
            }
            return IKapC.ITKSTATUS_INVALID_ARG;
        }

        static public uint GetTriggerPolarity(IKCamera cam, StringBuilder triggerPolarity, ref uint len)
        {
            uint res = IKapC.ITKSTATUS_OK;
            switch (cam.g_configMethod[IKCamera.ITEK_CAMERA_TRIGGER_POLARITY_CONFIG])
            {
                case IKCamera.ITEK_CAMERA_TRIGGER_POLARITY_CONFIG_METHOD_1:
                    return IKapC.ItkDevToString(cam.g_hCamera, "FrameTriggerPolarity", triggerPolarity, ref len);
                case IKCamera.ITEK_CAMERA_TRIGGER_POLARITY_CONFIG_METHOD_2:
                    return IKapC.ItkDevToString(cam.g_hCamera, "ExternalTriggerActivation", triggerPolarity, ref len);
                case IKCamera.ITEK_CAMERA_TRIGGER_POLARITY_CONFIG_METHOD_3:
                    return IKapC.ItkDevToString(cam.g_hCamera, "LinePolarity", triggerPolarity, ref len);
                case IKCamera.ITEK_CAMERA_TRIGGER_POLARITY_CONFIG_METHOD_4:
                    return IKapC.ItkDevToString(cam.g_hCamera, "TriggerActivation", triggerPolarity, ref len);
                default:
                    break;
            }
            return IKapC.ITKSTATUS_INVALID_ARG;
        }
        static public uint SetMultiExposureTime(IKCamera cam, string multiExposureTime)
        {
            uint res = IKapC.ITKSTATUS_OK;
            switch (cam.g_configMethod[IKCamera.ITEK_CAMERA_MULTI_EXPOSURE_CONFIG])
            {
                case IKCamera.ITEK_CAMERA_MULTI_EXPOSURE_CONFIG_METHOD_1:
                    return IKapC.ItkDevFromString(cam.g_hCamera, "MultiExposureTimeCount", multiExposureTime);
                default:
                    break;
            }
            return IKapC.ITKSTATUS_INVALID_ARG;
        }

        static public uint GetMultiExposureTime(IKCamera cam, StringBuilder multiExposureTime, ref uint bufLen)
        {
            uint res = IKapC.ITKSTATUS_OK;
            switch (cam.g_configMethod[IKCamera.ITEK_CAMERA_MULTI_EXPOSURE_CONFIG])
            {
                case IKCamera.ITEK_CAMERA_MULTI_EXPOSURE_CONFIG_METHOD_1:
                    return IKapC.ItkDevToString(cam.g_hCamera, "MultiExposureTimeCount", multiExposureTime, ref bufLen);
                default:
                    break;
            }
            return IKapC.ITKSTATUS_INVALID_ARG;
        }

        static public uint SetChunkData(IKCamera cam, bool bEnable)
        {
            uint res = IKapC.ITKSTATUS_OK;
            switch (cam.g_configMethod[IKCamera.ITEK_CAMERA_CHUNK_DATA_CONFIG])
            {
                case IKCamera.ITEK_CAMERA_CHUNK_DATA_CONFIG_METHOD_1:
                    {
                        res = IKapC.ItkDevSetBool(cam.g_hCamera, "ChunkModeActive", bEnable);
                        if (res != IKapC.ITKSTATUS_OK)
                        {
                            return res;
                        }
                        if (bEnable)
                        {
                            ITK_FEATURE_BASE_INFO featureInfo = new ITK_FEATURE_BASE_INFO();
                            res = IKapC.ItkDevGetFeatureInfo(cam.g_hCamera, "ChunkSelector", featureInfo);
                            if (res != IKapC.ITKSTATUS_OK)
                            {
                                return res;
                            }
                            uint enumCount = featureInfo.EnumCount;
  
                            if (enumCount == 0)
                            {
                                return IKapC.ITKSTATUS_INVALID_ARG;
                            }
                            // 开启所有chunk项
                            //
                            // Enable all chunk items
                            for (int i = 0; i < enumCount; i++)
                            {
                                ITK_FEATURE_ENUM_ENTRY_INFO enumEntryInfo = new ITK_FEATURE_ENUM_ENTRY_INFO();
                                res = IKapC.ItkDevGetEnumEntryFeatureInfo(cam.g_hCamera, "ChunkSelector", (uint)i, enumEntryInfo);
                                if (res != IKapC.ITKSTATUS_OK)
                                {
                                    return res;
                                }
                                res = IKapC.ItkDevFromString(cam.g_hCamera, "ChunkSelector", enumEntryInfo.ValueStr);
                                if (res != IKapC.ITKSTATUS_OK)
                                {
                                    return res;
                                }
                                res = IKapC.ItkDevSetBool(cam.g_hCamera, "ChunkEnable", true);
                                if (res != IKapC.ITKSTATUS_OK)
                                {
                                    return res;
                                }
                            }
                        }

                    }
                    break;
                default:
                    break;
            }
            return IKapC.ITKSTATUS_INVALID_ARG;
        }

        static public uint GetChunkData(IKCamera cam, bool bEnable)
        {
            uint res = IKapC.ITKSTATUS_OK;
            switch (cam.g_configMethod[IKCamera.ITEK_CAMERA_CHUNK_DATA_CONFIG])
            {
                case IKCamera.ITEK_CAMERA_CHUNK_DATA_CONFIG_METHOD_1:
                    return IKapC.ItkDevGetBool(cam.g_hCamera, "ChunkModeActive", ref bEnable);
                default:
                    break;
            }
            return IKapC.ITKSTATUS_INVALID_ARG;
        }

        //演示如何注册相机FrameTrigger事件、FrameTriggerIgnored事件、LineTriggerIgnored事件、MessageChannel事件
        #region 
        
        public static void FrameTriggerCallback(IntPtr pContext, IntPtr hEventInfo)
        {
            // 获取事件信息。
            //
            // Get the event info.
            ITK_DEVEVENT_INFO devEventInfo = new ITK_DEVEVENT_INFO();
            uint res = IKapC.ItkEventGetInfo(hEventInfo, devEventInfo);
            IKUtils.CheckIKapC(res);

            // 判断类型位数并打印信息。
            //
            // Determine the type bit and print information.
            Console.Write($"event name: {devEventInfo.eventName}, time: {devEventInfo.timestamp}.\n");
        } 

        public static void FrameTriggerIgnoredCallback(IntPtr pContext, IntPtr hEventInfo)
        {
            // 获取事件信息。
            //
            // Get the event info.
            ITK_DEVEVENT_INFO devEventInfo = new ITK_DEVEVENT_INFO();
            uint res = IKapC.ItkEventGetInfo(hEventInfo, devEventInfo);
            IKUtils.CheckIKapC(res);

            // 判断类型位数并打印信息。
            //
            // Determine the type bit and print information.
            Console.Write($"event name: {devEventInfo.eventName}, time: {devEventInfo.timestamp}.\n");
        }

        public static void LineTriggerIgnoredCallback(IntPtr pContext, IntPtr hEventInfo)
        {
            // 获取事件信息。
            //
            // Get the event info.
            ITK_DEVEVENT_INFO devEventInfo = new ITK_DEVEVENT_INFO();
            uint res = IKapC.ItkEventGetInfo(hEventInfo, devEventInfo);
            IKUtils.CheckIKapC(res);

            // 判断类型位数并打印信息。
            //
            // Determine the type bit and print information.
            Console.Write($"event name: {devEventInfo.eventName}, time: {devEventInfo.timestamp}.\n");
        }

        public static void MessageChannelCallback(IntPtr pContext, IntPtr hEventInfo)
        {
            // 获取事件信息。
            //
            // Get the event info.
            ITK_DEVEVENT_INFO devEventInfo = new ITK_DEVEVENT_INFO();
            uint res = IKapC.ItkEventGetInfo(hEventInfo, devEventInfo);
            IKUtils.CheckIKapC(res);

            // 判断类型位数并打印信息。
            //
            // Determine the type bit and print information.
            Console.Write($"event name: {devEventInfo.eventName}, event source: {devEventInfo.eventSource}, time: {devEventInfo.timestamp}.\n");
        }

        public delegate void IKapCDeviceCallBackDelegate(IntPtr pContext, IntPtr hEventInfo); 
        private static IKapCDeviceCallBackDelegate frameTriggerDelegate;
        private static IKapCDeviceCallBackDelegate frameTriggerIgnoredDelegate;
        private static IKapCDeviceCallBackDelegate lineTriggerIgnoredDelegate;
        private static IKapCDeviceCallBackDelegate messageChannelDelegate;

        /// <summary>
        /// 仅部分相机支持FrameTrigger事件、FrameTriggerIgnored事件、LineTriggerIgnored事件、MessageChannel事件，请查阅相机说明书确保相机支持该功能;
        /// </summary>
        static public uint RegisterDeviceCallback(IKCamera cam)
        {
            uint res = IKapC.ITKSTATUS_OK;

            // 注册FrameTrigger事件。
            //
            // register FrameTrigger event.
            frameTriggerDelegate = new IKapCDeviceCallBackDelegate(FrameTriggerCallback);
            res = IKapC.ItkDevRegisterCallback(cam.g_hCamera, "FrameTrigger", Marshal.GetFunctionPointerForDelegate(frameTriggerDelegate), IntPtr.Zero);
            IKUtils.CheckIKapC(res);

            // 注册FrameTriggerIgnored事件。
            //
            // register FrameTriggerIgnored event.
            frameTriggerIgnoredDelegate = new IKapCDeviceCallBackDelegate(FrameTriggerIgnoredCallback);
            res = IKapC.ItkDevRegisterCallback(cam.g_hCamera, "FrameTriggerIgnored", Marshal.GetFunctionPointerForDelegate(frameTriggerIgnoredDelegate), IntPtr.Zero);
            IKUtils.CheckIKapC(res);

            // 注册LineTriggerIgnored事件。
            //
            // register LineTriggerIgnored event.
            lineTriggerIgnoredDelegate = new IKapCDeviceCallBackDelegate(LineTriggerIgnoredCallback);
            res = IKapC.ItkDevRegisterCallback(cam.g_hCamera, "LineTriggerIgnored", Marshal.GetFunctionPointerForDelegate(lineTriggerIgnoredDelegate), IntPtr.Zero);
            IKUtils.CheckIKapC(res);

            // 注册MessageChannel事件。
            //
            // register MessageChannel event.
            messageChannelDelegate = new IKapCDeviceCallBackDelegate(MessageChannelCallback);
            res = IKapC.ItkDevRegisterCallback(cam.g_hCamera, "MessageChannel", Marshal.GetFunctionPointerForDelegate(messageChannelDelegate), IntPtr.Zero);
            IKUtils.CheckIKapC(res);

            return res;
        }

        static public uint UnRegisterDeviceCallback(IKCamera cam)
        {
            uint res = IKapC.ITKSTATUS_OK;

            // 取消注册FrameTrigger事件。
            //
            // unregister FrameTrigger event.
            res = IKapC.ItkDevUnregisterCallback(cam.g_hCamera, "FrameTrigger");
            IKUtils.CheckIKapC(res);

            // 取消注册FrameTriggerIgnored事件。
            //
            // unregister FrameTriggerIgored event.
            res = IKapC.ItkDevUnregisterCallback(cam.g_hCamera, "FrameTriggerIgnored");
            IKUtils.CheckIKapC(res);

            // 取消注册LineTriggerIgnored事件。
            //
            // unregister LineTriggerIgnored event.
            res = IKapC.ItkDevUnregisterCallback(cam.g_hCamera, "LineTriggerIgnored");
            IKUtils.CheckIKapC(res);

            // 取消注册MessageChannel事件。
            //
            // unregister MessageChannel event.
            res = IKapC.ItkDevUnregisterCallback(cam.g_hCamera, "MessageChannel");
            IKUtils.CheckIKapC(res);

            return res;
        }
        #endregion
    }
}
