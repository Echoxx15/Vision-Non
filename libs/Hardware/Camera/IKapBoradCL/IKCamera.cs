/* @brief：本示例向用户演示如何运用IKapC库和IKapBoard库，对ITEK多相机进行采图。
 *
 * @brief：This example shows users how to use IKapC library and IKapBoard Library to grab images continuously with
 * ITEK cameras. */

using System;
using System.Runtime.InteropServices;
using IKapCDotNet;

namespace IKNS
{
    class IKCamera
    {
        public const uint ITEK_CAMERA_WIDTH_CONFIG = 0;
        public const uint ITEK_CAMERA_HEIGHT_CONFIG = 1;
        public const uint ITEK_CAMERA_PIXEL_FORMAT_CONFIG = 2;
        public const uint ITEK_CAMERA_EXPOSURE_TIME_CONFIG = 3;
        public const uint ITEK_CAMERA_EXPOSURE_MODE_CONFIG = 4;
        public const uint ITEK_CAMERA_DIGITAL_GAIN_CONFIG = 5;
        public const uint ITEK_CAMERA_PIXEL_GAIN_CONFIG = 6;
        public const uint ITEK_CAMERA_LINE_RATE_CONFIG = 7;
        public const uint ITEK_CAMERA_FRAME_RATE_CONFIG = 8;
        public const uint ITEK_CAMERA_LINE_TRIGGER_CONFIG = 9;
        public const uint ITEK_CAMERA_FRAME_TRIGGER_CONFIG = 10;
        public const uint ITEK_CAMERA_TRIGGER_SOURCE_CONFIG = 11;
        public const uint ITEK_CAMERA_TRIGGER_POLARITY_CONFIG = 12;
        public const uint ITEK_CAMERA_MULTI_EXPOSURE_CONFIG = 13;
        public const uint ITEK_CAMERA_CHUNK_DATA_CONFIG = 14;
        public const uint ITEK_CAMERA_CONFIG_TOTAL_COUNT = 15;

        public const uint ITEK_CAMERA_WIDTH_CONFIG_METHOD_1 = 1;// RegionEnable ReginWidth
        public const uint ITEK_CAMERA_WIDTH_CONFIG_METHOD_2 = 2;  // ROIEnable ROIWidth
        public const uint ITEK_CAMERA_WIDTH_CONFIG_METHOD_3 = 3; // ReginWidth
        public const uint ITEK_CAMERA_WIDTH_CONFIG_METHOD_4 = 4;  // ROIWidth

        public const uint ITEK_CAMERA_HEIGHT_CONFIG_METHOD_1 = 1;  // RegionEnable ReginHeight
        public const uint ITEK_CAMERA_HEIGHT_CONFIG_METHOD_2 = 2;  // ROIEnable ROIHeight
        public const uint ITEK_CAMERA_HEIGHT_CONFIG_METHOD_3 = 3;  // ReginHeight
        public const uint ITEK_CAMERA_HEIGHT_CONFIG_METHOD_4 = 4;  // ROIHeight

        public const uint ITEK_CAMERA_PIXEL_FORMAT_CONFIG_METHOD_1 = 1;

        public const uint ITEK_CAMERA_EXPOSURE_TIME_CONFIG_METHOD_1 = 1;  // ExposureTimeRaw
        public const uint ITEK_CAMERA_EXPOSURE_TIME_CONFIG_METHOD_2 = 2;  // AreaMode (true) ExposureTime AreaExposureTime
        public const uint ITEK_CAMERA_EXPOSURE_TIME_CONFIG_METHOD_3 = 3;  // AreaMode (true) AreaExposureTime
        public const uint ITEK_CAMERA_EXPOSURE_TIME_CONFIG_METHOD_4 = 4;  // ExposureTime
        public const uint ITEK_CAMERA_EXPOSURE_TIME_CONFIG_METHOD_5 = 5;  // Area (true) AreaExposureTime

        public const uint ITEK_CAMERA_EXPOSURE_MODE_CONFIG_METHOD_1 = 1;
        public const uint ITEK_CAMERA_EXPOSURE_MODE_CONFIG_METHOD_2 = 2;

        public const uint ITEK_CAMERA_DIGITAL_GAIN_CONFIG_METHOD_1 = 1;  // Digital Gain
        public const uint ITEK_CAMERA_DIGITAL_GAIN_CONFIG_METHOD_2 = 2;  // Gain

        public const uint ITEK_CAMERA_PIXEL_GAIN_CONFIG_METHOD_1 = 1;

        public const uint ITEK_CAMERA_LINE_RATE_CONFIG_METHOD_1 = 1;  // LinePeriodTime
        public const uint ITEK_CAMERA_LINE_RATE_CONFIG_METHOD_2 = 2;  // AcquisitionLinePeriod
        public const uint ITEK_CAMERA_LINE_RATE_CONFIG_METHOD_3 = 3;  // AcquisitionLineRate

        public const uint ITEK_CAMERA_FRAME_RATE_CONFIG_METHOD_1 = 1;  // AcquisitionFramePeriodRaw
        public const uint ITEK_CAMERA_FRAME_RATE_CONFIG_METHOD_2 = 2;  // AcquisitionFramePeriod(int64)
        public const uint ITEK_CAMERA_FRAME_RATE_CONFIG_METHOD_3 = 3;  // AcquisitionFramePeriod(double)
        public const uint ITEK_CAMERA_FRAME_RATE_CONFIG_METHOD_4 = 4;  // FramePeriodTime
        public const uint ITEK_CAMERA_FRAME_RATE_CONFIG_METHOD_5 = 5;  // AcquisitionFrameRateEnable AcquisitionFrameRate
        public const uint ITEK_CAMERA_FRAME_RATE_CONFIG_METHOD_6 = 6;  // AcquisitionFrameRate

        public const uint ITEK_CAMERA_LINE_TRIGGER_CONFIG_METHOD_1 = 1;  // TriggerSelector TriggerMode ExposureMode
        public const uint ITEK_CAMERA_LINE_TRIGGER_CONFIG_METHOD_2 = 2;  // TriggerMode ExposureMode
        public const uint ITEK_CAMERA_LINE_TRIGGER_CONFIG_METHOD_3 = 3;  // SynchronizationMode

        public const uint ITEK_CAMERA_FRAME_TRIGGER_CONFIG_METHOD_1 = 1;  // TriggerSelector TriggerMode ExposureMode
        public const uint ITEK_CAMERA_FRAME_TRIGGER_CONFIG_METHOD_2 = 2;  // FrameTriggerEnable
        public const uint ITEK_CAMERA_FRAME_TRIGGER_CONFIG_METHOD_3 = 3;  // ExposureMode

        public const uint ITEK_CAMERA_TRIGGER_SOURCE_CONFIG_METHOD_1 = 1;  // FrameTriggerSource
        public const uint ITEK_CAMERA_TRIGGER_SOURCE_CONFIG_METHOD_2 = 2;  // ExternalTriggerSource
        public const uint ITEK_CAMERA_TRIGGER_SOURCE_CONFIG_METHOD_3 = 3;  // TriggerSource

        public const uint ITEK_CAMERA_TRIGGER_POLARITY_CONFIG_METHOD_1 = 1;  // FrameTriggerPolarity
        public const uint ITEK_CAMERA_TRIGGER_POLARITY_CONFIG_METHOD_2 = 2;  // ExternalTriggerActivation
        public const uint ITEK_CAMERA_TRIGGER_POLARITY_CONFIG_METHOD_3 = 3;  // LinePolarity
        public const uint ITEK_CAMERA_TRIGGER_POLARITY_CONFIG_METHOD_4 = 4;  // TriggerActivation

        public const uint ITEK_CAMERA_MULTI_EXPOSURE_CONFIG_METHOD_1 = 1; // MultiExposureTimeCount

        public const uint ITEK_CAMERA_CHUNK_DATA_CONFIG_METHOD_1 = 1; // ChunkModeActive ChunkSelector ChunkEnable


        /// \~chinese 参数配置方式列表	\~english List of method of parameter configure
        public uint[] g_configMethod = new uint[ITEK_CAMERA_CONFIG_TOTAL_COUNT];

        /// \~chinese 图像缓冲区申请的帧数				\~english The number of frames requested by buffer
        public uint g_bufferCount = 5;

        /// \~chinese 保存图像的文件名					\~english File name of image
        public string g_saveFileName;

        /// \~chinese 要打开相机的序列号					\~english SerialNumber of camera to open
        public string g_SerialNumber;

        /// \~chinese 希望采集的帧数						\~english Number of frames wanted
        public uint g_grabCount = IKapC.ITKSTREAM_CONTINUOUS;

        /// \~chinese 是否开启软触发						\~english Whether enable softTrigger or not
        public bool g_bSoftTriggerUsed = false;

        /// \~chinese 是否加载采集卡配置文件				\~english Whether load grabber configure file or not
        public bool g_bLoadGrabberConfig = false;

        /// \~chinese 相机设备句柄						\~english Camera device handle
        public ITKDEVICE g_hCamera;

        /// \~chinese 数据流句柄						\~english Data stream handle
        public ITKSTREAM g_hStream;

        /// \~chinese 相机设备信息						\~english Camera device info
        public ITKDEV_INFO g_devInfo;

        /// \~chinese 采集卡设备句柄						\~english Frame grabber device handle
        public IntPtr g_hBoard = IntPtr.Zero;

        /// \~chinese 缓冲区数据						\~english Buffer data
        public IntPtr g_bufferData = IntPtr.Zero;

        /// \~chinese 用户申请的Buffer内存						\~english The Buffer of memory requested by the user
        public IntPtr g_user_buffer = IntPtr.Zero;

        /// \~chinese 是否开启chunkdata						\~english enable chunkdata or not
        public bool bEnableChunkData = false;

        /// \~chinese 相机序号						\~english Index of Camera
        public int g_index = -1;

        /// \~chinese 图像大小						\~english Image Size
        public long g_nFrameSize = 0;

        /// \~chinese 图像宽度						\~english Image Width
        public long g_nWidth = 0;

        /// \~chinese 图像高度						\~english Image Height
        public long g_nHeight = 0;

        public bool g_isCXP = false;

        public delegate void IKapCCallBackDelegate(uint eventType, IntPtr pContext);
        public IKapCCallBackDelegate OnGrabStartDelegate;
        public IKapCCallBackDelegate OnTimeoutDelegate;
        public IKapCCallBackDelegate OnFrameLostDelegate;
        public IKapCCallBackDelegate OnGrabStopDelegate;
        public IKapCCallBackDelegate OnFrameReadyDelegate;

        public delegate void IKapBoardCallBackDelegate(IntPtr pParam);
        public IKapBoardCallBackDelegate OnBoardGrabStartDelegate;
        public IKapBoardCallBackDelegate OnBoardFrameReadyDelegate;
        public IKapBoardCallBackDelegate OnBoardFrameLostDelegate;
        public IKapBoardCallBackDelegate OnBoardTimeoutDelegate;
        public IKapBoardCallBackDelegate OnBoardGrabStopDelegate;

        public GCHandle IKCameraGCHandle;
    }
}
