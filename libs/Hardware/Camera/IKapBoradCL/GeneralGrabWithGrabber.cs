using System;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using IKapCDotNet;
using IKapBoardDotNet;
using IKNS;

namespace GeneralGrabWithGrabberNS
{
    class GeneralGrabWithGrabber
    {
        public static void SetSoftTriggerWithGrabber(IKCamera cam)
        {
            uint res = IKapC.ITKSTATUS_OK;
            int ret = IKapBoard.IK_RTN_OK;
            if (cam.g_bSoftTriggerUsed)
            {
                if (String.Compare(cam.g_devInfo.DeviceClass, "CoaXPress") == 0)
                {
                    res = IKapC.ItkDevSetDouble(cam.g_hCamera, "ExposureTime", 80);
                    res = IKapC.ItkDevFromString(cam.g_hCamera, "TriggerMode", "On");
                    res = IKapC.ItkDevFromString(cam.g_hCamera, "ExposureMode", "TriggerPulse");
                    ret = IKapBoard.IKapSetInfo(cam.g_hBoard, (uint)IKapBoard.IKP_IMAGE_HEIGHT, 1000);
                    IKUtils.CheckIKapBoard(ret);

                    ret = IKapBoard.IKapSetInfo(cam.g_hBoard, (uint)IKapBoard.IKP_CXP_TRIGGER_OUTPUT_SELECTOR, 1);  // Integration Signal1
                    IKUtils.CheckIKapBoard(ret);

                    ret = IKapBoard.IKapSetInfo(cam.g_hBoard, (uint)IKapBoard.IKP_INTEGRATION_TRIGGER_SOURCE, 9);  // Software Trigger
                    IKUtils.CheckIKapBoard(ret);

                    ret = IKapBoard.IKapSetInfo(cam.g_hBoard, (uint)IKapBoard.IKP_INTEGRATION_METHOD, 4);  // Method5
                    IKUtils.CheckIKapBoard(ret);

                    ret = IKapBoard.IKapSetInfo(cam.g_hBoard, (uint)IKapBoard.IKP_INTEGRATION_PARAM2, 5000);
                    IKUtils.CheckIKapBoard(ret);

                    ret = IKapBoard.IKapSetInfo(cam.g_hBoard, (uint)IKapBoard.IKP_CXP_FRAME_BURST_COUNT, 1050);
                    IKUtils.CheckIKapBoard(ret);

                    ret = IKapBoard.IKapSetInfo(cam.g_hBoard, (uint)IKapBoard.IKP_CXP_FRAME_BURST_PERIOD, 100000);
                    IKUtils.CheckIKapBoard(ret);

                    ret = IKapBoard.IKapSetInfo(cam.g_hBoard, (uint)IKapBoard.IKP_SOFTWARE_TRIGGER_WIDTH, 5000);
                    IKUtils.CheckIKapBoard(ret);

                }
                else if (String.Compare(cam.g_devInfo.DeviceClass, "CameraLink") == 0)
                {
                    res = IKapC.ItkDevFromString(cam.g_hCamera, "SynchronizationMode", "ExternalPulse");
                    res = IKapC.ItkDevFromString(cam.g_hCamera, "InputLineTriggerSource", "CC1");  // InputLineTriggerSource
                    res = IKapC.ItkDevSetDouble(cam.g_hCamera, "LinePeriodTime", 50);
                    ret = IKapBoard.IKapSetInfo(cam.g_hBoard, (uint)IKapBoard.IKP_IMAGE_HEIGHT, 2000);
                    ret = IKapBoard.IKapSetInfo(cam.g_hBoard, (uint)IKapBoard.IKP_CC1_SOURCE, 5);                  // Software Trigger
                    ret = IKapBoard.IKapSetInfo(cam.g_hBoard, (uint)IKapBoard.IKP_SOFTWARE_TRIGGER_SYNC_MODE, 0);  // Off
                    ret = IKapBoard.IKapSetInfo(cam.g_hBoard, (uint)IKapBoard.IKP_SOFTWARE_TRIGGER_PERIOD, 80);    // softTriggerPeriod
                    ret = IKapBoard.IKapSetInfo(cam.g_hBoard, (uint)IKapBoard.IKP_SOFTWARE_TRIGGER_COUNT, 2050);   // softTriggerCount

                }
                else
                {
                    res = IKapC.ItkDevFromString(cam.g_hCamera, "TriggerSelector", "FrameStart");
                    res = IKapC.ItkDevFromString(cam.g_hCamera, "TriggerMode", "On");
                    res = IKapC.ItkDevFromString(cam.g_hCamera, "TriggerSource", "Software");
                }
            }
            else
            {
                if (String.Compare(cam.g_devInfo.DeviceClass, "CoaXPress") == 0)
                {
                    res = IKapC.ItkDevFromString(cam.g_hCamera, "ExposureMode", "Timed");
                }
                else if (String.Compare(cam.g_devInfo.DeviceClass, "CameraLink") == 0)
                {
                    res = IKapC.ItkDevFromString(cam.g_hCamera, "SynchronizationMode", "InternalFreeRun");
                }
                else
                {
                    res = IKapC.ItkDevFromString(cam.g_hCamera, "TriggerSelector", "FrameStart");
                    res = IKapC.ItkDevFromString(cam.g_hCamera, "TriggerMode", "Off");
                }
            }
        }

        public static void ConfigureFrameGrabber(IKCamera cam)
        {
            uint res = IKapC.ITKSTATUS_OK;
            int ret = IKapBoard.IK_RTN_OK;

            if (cam.g_bLoadGrabberConfig)
            {
                string configFileName = GetOption();
                if (configFileName == null)
                {
                    Console.Write("Fail to get configuration, using default setting!\n");
                }
                else
                {
                    ret = IKapBoard.IKapLoadConfigurationFromFile(cam.g_hBoard, configFileName);
                    IKUtils.CheckIKapBoard(ret);
                }
            }

            ret = IKapBoard.IKapSetInfo(cam.g_hBoard, (uint)IKapBoard.IKP_FRAME_COUNT, (int)cam.g_bufferCount);
            IKUtils.CheckIKapBoard(ret);

            int timeout = -1;
            ret = IKapBoard.IKapSetInfo(cam.g_hBoard, (uint)IKapBoard.IKP_TIME_OUT, timeout);
            IKUtils.CheckIKapBoard(ret);

            int grab_mode = IKapBoard.IKP_GRAB_NON_BLOCK;
            ret = IKapBoard.IKapSetInfo(cam.g_hBoard, (uint)IKapBoard.IKP_GRAB_MODE, grab_mode);
            IKUtils.CheckIKapBoard(ret);

            int transfer_mode = IKapBoard.IKP_FRAME_TRANSFER_SYNCHRONOUS_NEXT_EMPTY_WITH_PROTECT;
            ret = IKapBoard.IKapSetInfo(cam.g_hBoard, (uint)IKapBoard.IKP_FRAME_TRANSFER_MODE, transfer_mode);
            IKUtils.CheckIKapBoard(ret);

            /***************************/
            /// \~chinese  展示如果使用用户申请的内存地址作为Buffer的内存地址，注意不要忘记释放该内存	       \~english Show how to using the memory address as the memory address of Buffer,be careful not to forget to release the memory
            /// \~chinese  创建用于设置Buffer地址的内存		       \~english Create the memory that the user requests for setting the Buffer address
            //long nFrameSize = 0;
            //ret = IKapBoard.IKapGetInfo64(cam.g_hBoard, (uint)IKapBoard.IKP_FRAME_SIZE, ref nFrameSize);
            //IKUtils.CheckIKapBoard(ret);

            //cam.g_user_buffer = Marshal.AllocHGlobal((IntPtr)nFrameSize);
            //if (cam.g_user_buffer == IntPtr.Zero)
            //{
            //    IKUtils.pressEnterToExit();
            //}

            // \~chinese 将序号为0的Buffer的内存地址改为用户申请的大小合适的内存地址，序号为1~g_bufferCount-1的Buffer同理。		\~english The memory address of Buffer with index number 0 is changed to the appropriate size memory address applied by the user, just as with Buffer with index number 1~g_bufferCount-1.
            //ret = IKapBoard.IKapSetBufferAddress(cam.g_hBoard, 0, cam.g_user_buffer);
            //IKUtils.CheckIKapBoard(ret);
            /***************************/
        }

        public static void StartGrabImage(IKCamera cam)
        {
            uint res = IKapC.ITKSTATUS_OK;
            int ret = IKapBoard.IK_RTN_OK;

            if (String.Compare(cam.g_devInfo.DeviceClass, "CameraLink") != 0)
            {
                res = IKapC.ItkDevExecuteCommand(cam.g_hCamera, "AcquisitionStop");
                IKUtils.CheckIKapC(res);
            }

            ret = IKapBoard.IKapStartGrab(cam.g_hBoard, 0);
            IKUtils.CheckIKapBoard(ret);

            if (String.Compare(cam.g_devInfo.DeviceClass, "CameraLink") != 0)
            {
                res = IKapC.ItkDevExecuteCommand(cam.g_hCamera, "AcquisitionStart");
                IKUtils.CheckIKapC(res);
            }
        }

        /// \~chinese 选择用户配置文件				                \~english Select configuration file
        public static string GetOption()
        {
            string vlcfFileName = null;
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "vlcf文件(*.vlcf)|*.vlcf|所有文件(*.*)|*.*";
            ofd.FilterIndex = 1;
            ofd.Title = "选择打开文件";
            if (ofd.ShowDialog() == DialogResult.OK)
                vlcfFileName = ofd.FileName;
            return vlcfFileName;
        }
    }

}
