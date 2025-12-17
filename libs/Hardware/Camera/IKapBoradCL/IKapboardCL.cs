using Cognex.VisionPro;
using HardwareCameraNet;
using IKapBoardDotNet;
using IKapCDotNet;
using IKNS;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using static GeneralConfigureCameraNS.GeneralConfigureCamera;
using static GeneralGrabWithGrabberNS.GeneralGrabWithGrabber;

namespace IKapBoradCL;

/// <summary>
/// 埃科采集卡实现类
/// </summary>
// 标记支持的品牌名称
[CameraManufacturer("埃科采集卡")]
public class IKapBoradCL : ICamera, IDisposable
{
    #region 相机属性

    /// <summary>
    /// 断线重连回调函数
    /// </summary>
    /// <param name="pContext"></param>
    /// <param name="hEventInfo"></param>
    private delegate void DeviceRemoveCallBackDelegate(IntPtr pContext, IntPtr hEventInfo);

    // 相机特有属性
    private IKCamera device = new();

    /// <summary>
    /// 保存相机序号
    /// </summary>
    private static readonly Dictionary<string, IKCamera> gDeviceInfos = new();

    /// <summary>
    /// ch: 断线重连线程 | en: Reconnect thread
    /// </summary>
    private Thread ReconnectThread;

    private readonly Stopwatch _stopwatch = new();

    /// <summary>
    /// 标记是否已释放，防止多次释放
    /// </summary>
    private bool _disposed;


    #endregion

    #region 接口属性

    /// <summary>
    /// 图片回调 Action
    /// </summary>
    public Action<ICogImage> OnFrameGrabed { get; set; }

    /// <summary>
    /// 采集超时回调 Action
    /// </summary>
    public Action<int> OnGrabTimeout { get; set; }

    public event EventHandler<bool> DisConnetEvent;
    public string SN { get; }
    public CameraType Type => CameraType.线扫相机;
    public bool IsConnected { get; private set; }
    public IParameters Parameters { get; }

    #endregion

    #region 构造函数

    public IKapBoradCL(string sn)
    {
        SN = sn ?? throw new ArgumentNullException(nameof(sn));
        // 初始化相机的参数实现类
        Parameters = new IKapParameters(this);
    }

    #endregion

    #region 静态枚举方法

    public static List<string> EnumerateDevices()
    {
        //IKapBoard.IKapC 函数返回值				   

        //chinese IKapBoard.IKapBoard 函数返回值

        // ch:枚举设备 | en:Enum device
        //初始化 IKapBoard.IKapC 运行环境
        var res = IKapC.ItkManInitialize();
        IKUtils.CheckIKapC(res);

        // 获取设备信息列表
        uint numDevices = 0;
        res = IKapC.ItkManGetDeviceCount(ref numDevices);
        IKUtils.CheckIKapC(res);

        // 当没有连接的设备时。
        //
        // When there is no connected devices.
        if (numDevices == 0)
        {
            Console.Write("No device.\n");
            IKapC.ItkManTerminate();
            IKUtils.pressEnterToExit();
        }

        uint i = 0;
        for (; i < numDevices; ++i)
        {
            // 相机信息结构体。
            //
            // Camera information structure.
            var di = new ITKDEV_INFO();

            // 获取设备信息。
            //
            // Get device information.
            IKapC.ItkManGetDeviceInfo(i, di);

            if (di.SerialNumber == "" || gDeviceInfos.ContainsKey(di.SerialNumber))
                continue;
            var cam = new IKCamera();

            cam.g_bufferCount = 5;

            cam.g_saveFileName = "D:\\CImage.tif";

            cam.g_SerialNumber = null;

            cam.g_bSoftTriggerUsed = false;

            cam.g_bLoadGrabberConfig = false;
            cam.g_index = (int)i;
            cam.g_devInfo = di;
            gDeviceInfos.Add(di.SerialNumber, cam);
            // 打印设备信息
            //
            // Print information.
            Console.WriteLine($"Device num: {i}");
            Console.Write(
                $"Device Full Name:{di.FullName}\n Friendly Name:{di.FriendlyName}\n Vendor Name:{di.VendorName}\n " +
                $"Model Name:{di.ModelName}\n Serial Name:{di.SerialNumber}\n Device Class:{di.DeviceClass}\n " +
                $"Device Version:{di.DeviceVersion}\n User Defined Name:{di.UserDefinedName}\n");

        }

        return gDeviceInfos.Keys.ToList();
    }

    #endregion


    #region 接口方法

    public int Open()
    {
        device = gDeviceInfos[SN];
        var i = (uint)device.g_index;
        var di = device.g_devInfo;
        Console.Write("Using camera: serial: {0}, name: {1}, interface: {2}.\n", di.SerialNumber, di.FullName,
            di.DeviceClass);
        var res = IKapC.ItkDevOpen(i, IKapC.ITKDEV_VAL_ACCESS_MODE_EXCLUSIVE, ref device.g_hCamera);
        //IKUtils.CheckIKapC(res);
        if (IKapC.ITKSTATUS_OK != res)
            return -1;

        // \~chinese 设置相机的超时时间				\~english set timeout of camera
        var timeOutPtr = Marshal.AllocHGlobal(4);
        Marshal.WriteInt32(timeOutPtr, 20000);
        res = IKapC.ItkDevSetPrm(device.g_hCamera, IKapC.ITKDEV_PRM_HEARTBEAT_TIMEOUT, timeOutPtr);
        if (IKapC.ITKSTATUS_OK != res)
            return -1;
        Marshal.FreeHGlobal(timeOutPtr);

        // \~chinese 获取特征名称列表并配置参数设置方法				\~english Get list of features' name and select parameter
        // configure method
        GetAllConfigureMethods(device);

        var cxp_cam_info = new ITK_CXP_DEV_INFO();
        var gvb_cam_info = new ITK_GVB_DEV_INFO();
        var cl_cam_info = new ITK_CL_DEV_INFO();
        IntPtr info;
        if (string.Compare(device.g_devInfo.DeviceClass, "CoaXPress") == 0)
        {
            //chinese 获取CoaXPress相机设备信息				    \~english Get CoaXPress camera device information
            res = IKapC.ItkManGetCXPDeviceInfo(i, cxp_cam_info);
            if (IKapC.ITKSTATUS_OK != res)
                return -1;
            info = IKapC.get_itk_cxp_dev_info_IntPtr(cxp_cam_info);
            device.g_isCXP = true;
        }
        else if (string.Compare(di.DeviceClass, "GigEVisionBoard") == 0)
        {
            // 获取GigEVision相机设备信息				    \~english Get GigEVision camera device information
            res = IKapC.ItkManGetGVBDeviceInfo(i, gvb_cam_info);
            if (IKapC.ITKSTATUS_OK != res)
                return -1;
            info = IKapC.get_itk_gvb_dev_info_IntPtr(gvb_cam_info);
        }
        else if (string.Compare(di.DeviceClass, "CameraLink") == 0)
        {
            // 获取CameraLink相机设备信息				    \~english Get CameraLink camera device information
            res = IKapC.ItkManGetCLDeviceInfo(i, cl_cam_info);
            if (IKapC.ITKSTATUS_OK != res)
                return -1;
            info = IKapC.get_itk_cl_dev_info_IntPtr(cl_cam_info);
        }
        else
        {
            Console.Write("Not camera Type");
            return -1;
        }

        device.g_hBoard = IKapBoard.IKapOpenWithSpecificInfo(info);


        if (device.g_hBoard == IntPtr.Zero)
        {
            Console.Write("Please select camera with grabber.\n");
            return -1;
        }
        // ch: 配置采集卡参数 | en: Configure frame grabber parameters
        ConfigureFrameGrabber(device);


        /// \~chinese 注册设备移除回调函数		            \~english Register device removal callback function
        DeviceRemoveCallBackDelegate deviceRemoveCallbackDelegate = cbOnReconnect;
        res = IKapC.ItkDevRegisterCallback(device.g_hCamera, "DeviceRemove",
            Marshal.GetFunctionPointerForDelegate(deviceRemoveCallbackDelegate), IntPtr.Zero);
        if (IKapC.ITKSTATUS_OK != res)
            return -1;




        //chinese 注册回调函数                        \~english Register callback functions.
        RegisterCallbackWithGrabber(device);

        //chinese 配置相机触发方式		            \~english Configure trigger method of the camera
        //SetSoftTriggerWithGrabber(device);
        StartGrabbing();

        IsConnected = true;
        return (int)IKapC.ITKSTATUS_OK;
    }

    public void SoftwareTriggerOnce()
    {
        throw new NotImplementedException();
    }

    public void ContinuousGrab()
    {
        throw new NotImplementedException();
    }

    public void StopContinuousGrab()
    {
        throw new NotImplementedException();
    }

    public int StartGrabbing()
    {
        /// \~chinese 开始图像采集				        \~english Start grabbing images
        StartGrabImage(device);
        return 0;
    }

    public int StopGrabbing()
    {
        /// \~chinese 释放用户申请的用于存放缓冲区数据的内存				    \~english Release the memory for storing the buffer data
        if (device.g_bufferData != IntPtr.Zero)
        {
            Marshal.FreeHGlobal(device.g_bufferData);
        }

        /// \~chinese 释放用户申请的用于设置Buffer地址的内存				    \~english Release the memory that the user requests for setting the Buffer address
        if (device.g_user_buffer != IntPtr.Zero)
        {
            Marshal.FreeHGlobal(device.g_user_buffer);
        }

        return IKapBoard.IKapStopGrab(device.g_hBoard);
    }

    public void Close()
    {
        // 防止多次释放
        if (!IsConnected) return;

        try
        {
            /// \~chinese 停止图像采集				        \~english Stop grabbing images
            var ret = IKapBoard.IKapStopGrab(device.g_hBoard);
            IKUtils.CheckIKapBoard(ret);

            /// \~chinese 清除回调函数				        \~english Unregister callback functions
            UnRegisterCallbackWithGrabber(device);

            /// \~chinese 释放用户申请的用于存放缓冲区数据的内存				    \~english Release the memory for storing the buffer data
            if (device.g_bufferData != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(device.g_bufferData);
                device.g_bufferData = IntPtr.Zero;
            }

            /// \~chinese 释放用户申请的用于设置Buffer地址的内存				    \~english Release the memory that the user requests for setting the Buffer address
            if (device.g_user_buffer != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(device.g_user_buffer);
                device.g_user_buffer = IntPtr.Zero;
            }

            /// \~chinese 关闭采集卡设备				        \~english Close frame grabber device
            ret = IKapBoard.IKapClose(device.g_hBoard);
            IKUtils.CheckIKapBoard(ret);
            device.g_hBoard = IntPtr.Zero;

            /// \~chinese 关闭相机设备				        \~english Close camera device
            var res = IKapC.ItkDevClose(device.g_hCamera);
            IKUtils.CheckIKapC(res);
            // 注意：g_hCamera 是 ITKDEVICE 结构体，不能直接赋值 IntPtr.Zero
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[IKapBoradCL] Close 异常: {ex.Message}");
        }
        finally
        {
            IsConnected = false;
            // 注意：不设置 _disposed = true，允许重新 Open
        }
    }

    /// <summary>
    /// 实现IDisposable接口，释放资源时自动调用Close
    /// </summary>
    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        
        Close();
        /// \~chinese 释放 IKapBoard.IKapC 运行环境				\~english Release IKapBoard.IKapC runtime environment
        IKapC.ItkManTerminate();
        GC.SuppressFinalize(this);
    }

    #endregion

    #region 内部方法

    #region 图像回调

    /// <summary>
    /// 注册函数
    /// </summary>
    /// <param name="cam"></param>
    private void RegisterCallbackWithGrabber(IKCamera cam)
    {
        cam.IKCameraGCHandle = GCHandle.Alloc(cam);
        var IKCameraIntPtr = GCHandle.ToIntPtr(cam.IKCameraGCHandle);

        cam.OnBoardGrabStartDelegate = OnGrabStart;
        var ret = IKapBoard.IKapRegisterCallback(cam.g_hBoard, (uint)IKapBoard.IKEvent_GrabStart,
            Marshal.GetFunctionPointerForDelegate(cam.OnBoardGrabStartDelegate), IntPtr.Zero);
        IKUtils.CheckIKapBoard(ret);

        cam.OnBoardFrameReadyDelegate = OnFrameReady;
        ret = IKapBoard.IKapRegisterCallback(cam.g_hBoard, (uint)IKapBoard.IKEvent_FrameReady,
            Marshal.GetFunctionPointerForDelegate(cam.OnBoardFrameReadyDelegate), IKCameraIntPtr);
        IKUtils.CheckIKapBoard(ret);

        cam.OnBoardFrameLostDelegate = OnFrameLost;
        ret = IKapBoard.IKapRegisterCallback(cam.g_hBoard, (uint)IKapBoard.IKEvent_FrameLost,
            Marshal.GetFunctionPointerForDelegate(cam.OnBoardFrameLostDelegate), IntPtr.Zero);
        IKUtils.CheckIKapBoard(ret);

        cam.OnBoardTimeoutDelegate = OnTimeout;
        ret = IKapBoard.IKapRegisterCallback(cam.g_hBoard, (uint)IKapBoard.IKEvent_TimeOut,
            Marshal.GetFunctionPointerForDelegate(cam.OnBoardTimeoutDelegate), IntPtr.Zero);
        IKUtils.CheckIKapBoard(ret);

        cam.OnBoardGrabStopDelegate = OnGrabStop;
        ret = IKapBoard.IKapRegisterCallback(cam.g_hBoard, (uint)IKapBoard.IKEvent_GrabStop,
            Marshal.GetFunctionPointerForDelegate(cam.OnBoardGrabStopDelegate), IntPtr.Zero);
        IKUtils.CheckIKapBoard(ret);

    }

    /// <summary>
    /// 释放函数
    /// </summary>
    /// <param name="cam"></param>
    private void UnRegisterCallbackWithGrabber(IKCamera cam)
    {
        var ret = IKapBoard.IKapUnRegisterCallback(cam.g_hBoard, (uint)IKapBoard.IKEvent_GrabStart);
        IKUtils.CheckIKapBoard(ret);

        ret = IKapBoard.IKapUnRegisterCallback(cam.g_hBoard, (uint)IKapBoard.IKEvent_FrameReady);
        IKUtils.CheckIKapBoard(ret);

        ret = IKapBoard.IKapUnRegisterCallback(cam.g_hBoard, (uint)IKapBoard.IKEvent_FrameLost);
        IKUtils.CheckIKapBoard(ret);

        ret = IKapBoard.IKapUnRegisterCallback(cam.g_hBoard, (uint)IKapBoard.IKEvent_TimeOut);
        IKUtils.CheckIKapBoard(ret);

        ret = IKapBoard.IKapUnRegisterCallback(cam.g_hBoard, (uint)IKapBoard.IKEvent_GrabStop);
        IKUtils.CheckIKapBoard(ret);

        cam.IKCameraGCHandle.Free();
    }

    private void OnGrabStart(IntPtr pContext)
    {
        Console.Write("Start grabbing image.\n");
    }

    /// <summary>
    /// 回调函数
    /// </summary>
    /// <param name="pContext"></param>
    private void OnFrameReady(IntPtr pContext)
    {
        Console.Write("On end of frame. \n");

        uint res = IKapC.ITKSTATUS_OK;
        int ret = IKapBoard.IK_RTN_OK;

        if (pContext != IntPtr.Zero)
        {
            GCHandle camGCHanle = GCHandle.FromIntPtr(pContext);
            IKCamera cam = (IKCamera)camGCHanle.Target; //前提：注册回调传入的是IKCamera object
            if (cam == null)
            {
                return;
            }

            Console.Write("Grab frame ready of camera with serialNumber:{0}.\n", cam.g_devInfo.SerialNumber);

            IntPtr pUserBuffer = IntPtr.Zero;
            long nFrameSize = 0;
            int nFrameCount = 0;
            int nFrameIndex = 0;

            IKAPBUFFERSTATUS status = new IKAPBUFFERSTATUS();

            ret = IKapBoard.IKapGetInfo(cam.g_hBoard, (uint)IKapBoard.IKP_FRAME_COUNT, ref nFrameCount);
            IKUtils.CheckIKapBoard(ret);

            ret = IKapBoard.IKapGetInfo(cam.g_hBoard, (uint)IKapBoard.IKP_CURRENT_BUFFER_INDEX, ref nFrameIndex);
            IKUtils.CheckIKapBoard(ret);


            ret = IKapBoard.IKapGetBufferStatus(cam.g_hBoard, nFrameIndex, status);
            IKUtils.CheckIKapBoard(ret);


            /// \~chinese 当图像缓冲区满时				      \~english When the buffer is full
            if (status.uFull == 1)
            {
                /// \~chinese 获取一帧图像的大小		      \~english Get the size of a frame of image
                ret = IKapBoard.IKapGetInfo64(cam.g_hBoard, (uint)IKapBoard.IKP_FRAME_SIZE, ref nFrameSize);
                IKUtils.CheckIKapBoard(ret);

                /// \~chinese 如果相机的MultiExposureTimeCount特征值大于1并且采集卡的IKP_MULTIPLE_LIGHT_COUNT参数设置为与MultiExposureTimeCount相同的值则开启了多重曝光功能，采集到的图像均分为N种曝光时间，[0~1*Height/N-1]行对应ExposureSelect为1时的ExposureTime，[1*Height/N~2*Height/N-1]行对应ExposureSelect为2时的ExposureTime，...，[(N-1)*Height/N~Height-1]行对应ExposureSelect为N时的ExposureTime.	\~english	If the MultiExposureTimeCount feature value is greater than 1, the multiple exposure function is turned on, The collected images were all divided into N exposure times, line [0~1 * Height / N-1] corresponds to the ExposureTime at a ExposureSelect of 1, line [1 * Height / N~2 * Height / N-1] corresponds to ExposureTime at ExposureSelect 2,..., line [(N-1) * Height / N~Height-1] corresponds to ExposureTime at ExposureSelect N.
                /// \~chinese 获取缓冲区地址				  \~english Get the buffer address
                ret = IKapBoard.IKapGetBufferAddress(cam.g_hBoard, nFrameIndex, ref pUserBuffer);
                IKUtils.CheckIKapBoard(ret);

                // 转换为VisionPro图像
                // 
                // Convert to VisionPro format
                int nFrameWidth = 0;
                int nFrameHeight = 0;
                int nImageBit = 0; //像素深度
                IKapBoard.IKapGetInfo(cam.g_hBoard, (uint)IKapBoard.IKP_IMAGE_WIDTH, ref nFrameWidth);
                IKapBoard.IKapGetInfo(cam.g_hBoard, (uint)IKapBoard.IKP_IMAGE_HEIGHT, ref nFrameHeight);
                IKapBoard.IKapGetInfo(cam.g_hBoard, (uint)IKapBoard.IKP_BOARD_BIT, ref nImageBit);

                ///获取图像类型
                int nImageFormat = 0;
                ret = IKapBoard.IKapGetInfo(cam.g_hBoard, (uint)IKapBoard.IKP_BAYER_PATTERN, ref nImageFormat);
                IKUtils.CheckIKapBoard(ret);


                bool bIsValidImageBit = false;
                bool bIsColorImage = false; //图像位深是否为24
                if (nImageBit == 8 || nImageBit == 24)
                {
                    bIsColorImage = true;
                    //ImageHelper 实现8/24图像位深的实现
                    bIsValidImageBit = true;
                    if (nImageFormat == IKapBoard.IKP_BAYER_PATTERN_VAL_RGGB)
                    {
                        #region 图像格式转换

                        _stopwatch.Restart();
                        // 图像行高，宽幅
                        int bufferWidth = 0;
                        int bufferHeight = 0;

                        ITKBUFFER m_grayer = new ITKBUFFER();
                        ITKBUFFER m_color = new ITKBUFFER();

                        //缓冲区建立
                        ret = IKapBoard.IKapGetInfo(cam.g_hBoard, (uint)IKapBoard.IKP_IMAGE_HEIGHT,
                            ref bufferHeight);
                        IKUtils.CheckIKapBoard(ret);

                        ret = IKapBoard.IKapGetInfo(cam.g_hBoard, (uint)IKapBoard.IKP_IMAGE_WIDTH, ref bufferWidth);
                        IKUtils.CheckIKapBoard(ret);

                        res = IKapC.ItkBufferNew(bufferWidth, bufferHeight, IKapC.ITKBUFFER_VAL_FORMAT_BAYER_RG8,
                            ref m_grayer);
                        IKUtils.CheckIKapC(res);
                        res = IKapC.ItkBufferNew(bufferWidth, bufferHeight, IKapC.ITKBUFFER_VAL_FORMAT_RGB888,
                            ref m_color);
                        IKUtils.CheckIKapC(res);

                        res = IKapC.ItkBufferWrite(m_grayer, 0, pUserBuffer, (uint)nFrameSize);
                        IKUtils.CheckIKapC(res);


                        res = IKapC.ItkBufferBayerConvert(m_grayer, m_color, IKapC.ITKBUFFER_VAL_BAYER_RGGB);
                        IKUtils.CheckIKapC(res);

                        ITK_BUFFER_INFO bufferInfo = new ITK_BUFFER_INFO();
                        res = IKapC.ItkBufferGetInfo(m_color, bufferInfo);
                        IKUtils.CheckIKapC(res);


                        var image = CreateCogImage(bufferInfo.ImageAddress, nFrameWidth, nFrameHeight, true);

                        _stopwatch.Stop();
                        Console.Write($"转换耗时:{_stopwatch.ElapsedMilliseconds}");
                        // 使用 Action 回调
                        OnFrameGrabed?.Invoke(image);
                        GC.Collect();
                        return;

                        #endregion
                    }
                }

                if (bIsValidImageBit)
                {
                    var image = CreateCogImage(pUserBuffer, nFrameWidth, nFrameHeight, bIsColorImage);
                    // 使用 Action 回调
                    OnFrameGrabed?.Invoke(image);
                }

                GC.Collect();
            }
        }
    }

    /// <summary>
    /// 采集超时
    /// </summary>
    /// <param name="pContext"></param>
    private void OnTimeout(IntPtr pContext)
    {
        Console.Write("Grab image timeout.\n");
    }

    private void OnFrameLost(IntPtr pContext)
    {
        Console.Write("Grab frame lost.\n");
    }

    private void OnGrabStop(IntPtr pContext)
    {
        Console.Write("Stop grabbing image.\n");
        if (pContext != IntPtr.Zero)
        {
            var camGCHanle = GCHandle.FromIntPtr(pContext);
            var cam = (IKCamera)camGCHanle.Target; //前提：注册回调传入的是IKCamera object
            if (cam == null)
            {
                return;
            }

            Console.Write("On end of stream of camera with serialNumber:{0}. \n", cam.g_devInfo.SerialNumber);
        }
    }

    private ICogImage CreateCogImage(IntPtr buffer, int width, int height, bool isColor)
    {
        if (isColor)
        {
            return CreateColorImage(buffer, width, height);
        }
        else
        {
            return CreateGreyImage(buffer, width, height);
        }
    }

    private ICogImage CreateGreyImage(IntPtr buffer, int width, int height)
    {
        // Create and initialize CogImage8Root for grayscale image
        var greyRoot = new CogImage8Root();
        greyRoot.Initialize(width, height, buffer, width, null);

        // Create CogImage8Grey object and set root
        var greyImage = new CogImage8Grey();
        greyImage.SetRoot(greyRoot);

        return greyImage;
    }


    private unsafe ICogImage CreateColorImage(IntPtr buffer, int width, int height)
    {

        // Assuming buffer contains interleaved RGB data (24 bits per pixel)
        var pixelData = (byte*)buffer.ToPointer();

        var redPlane = new byte[width * height];
        var greenPlane = new byte[width * height];
        var bluePlane = new byte[width * height];

        for (var i = 0; i < width * height; i++)
        {
            redPlane[i] = pixelData[3 * i];
            greenPlane[i] = pixelData[3 * i + 1];
            bluePlane[i] = pixelData[3 * i + 2];
        }

        // Allocate unmanaged memory for each color plane
        var redPtr = Marshal.AllocHGlobal(redPlane.Length);
        var greenPtr = Marshal.AllocHGlobal(greenPlane.Length);
        var bluePtr = Marshal.AllocHGlobal(bluePlane.Length);

        // Copy managed arrays to unmanaged memory
        Marshal.Copy(redPlane, 0, redPtr, redPlane.Length);
        Marshal.Copy(greenPlane, 0, greenPtr, greenPlane.Length);
        Marshal.Copy(bluePlane, 0, bluePtr, bluePlane.Length);

        var redRoot = new CogImage8Root();
        var greenRoot = new CogImage8Root();
        var blueRoot = new CogImage8Root();

        redRoot.Initialize(width, height, redPtr, width, null);
        greenRoot.Initialize(width, height, greenPtr, width, null);
        blueRoot.Initialize(width, height, bluePtr, width, null);

        var image = new CogImage24PlanarColor();
        image.SetRoots(redRoot, greenRoot, blueRoot);

        return image;
    }

    #endregion


    #region 断线重连

    /// <summary>
    /// 断线事件
    /// </summary>
    /// <param name="pContext"></param>
    /// <param name="eventInfo"></param>
    private void cbOnReconnect(IntPtr pContext, IntPtr eventInfo)
    {
        IsConnected = false;
        DisConnetEvent?.Invoke(this, true);
        // 获取事件信息。
        //
        // Get the event info.
        ITK_DEVEVENT_INFO devEventInfo = new ITK_DEVEVENT_INFO();
        uint res = IKapC.ItkEventGetInfo(eventInfo, devEventInfo);
        IKUtils.CheckIKapC(res);

        // 判断类型位数并打印信息。
        //
        // Determine the type bit and print information.
        Console.Write($"event name: {devEventInfo.eventName}, time: {devEventInfo.timestamp}.\n");

        Console.WriteLine("Triggerd 'DeviceRemove' callback function!");
        IKapC.ItkStreamStop(device.g_hStream);

        // 清除回调函数。
        //
        // Unregister callback functions.
        UnRegisterCallbackWithGrabber(device);

        IKapC.ItkDevFreeStream(device.g_hStream);

        ReconnectThread = new Thread(ReconnectProcess) { IsBackground = true };
        ReconnectThread.Start();
    }

    /// <summary>
    /// 重连事件
    /// </summary>
    private void ReconnectProcess()
    {
        while (!IsConnected)
        {
            if (Open() == (int)IKapC.ITKSTATUS_OK)
            {
                IsConnected = true;
                DisConnetEvent?.Invoke(this, false);
                Console.WriteLine($"[{SN}]相机重连成功");
                break;
            }

            Thread.Sleep(3000);
        }
    }

    #endregion


    #endregion

    #region 相机的IParameters实现类（内部类）

    private class IKapParameters(IKapBoradCL IKCamera) : IParameters
    {
        public double ExposureTime
        {
            get
            {
                double val = 0;
                GetExposureTime(IKCamera.device, ref val);
                return val;
            }
            set => SetExposureTime(IKCamera.device, value);
        }

        public double MaxExposureTime => 99999;

        public double Gain
        {
            get
            {
                double val = 0;
                GetDigitalGain(IKCamera.device, ref val);
                return val;
            }
            set => SetDigitalGain(IKCamera.device, value);
        }

        public double MaxGain => 99;

        public int Width
        {
            get
            {
                long val = 0;
                GetWidth(IKCamera.device, ref val);
                return (int)val;
            }
            set => SetWidth(IKCamera.device, value);
        }

        public int Height
        {
            get
            {
                long val = 0;
                GetHeight(IKCamera.device, ref val);
                return (int)val;
            }
            set => SetHeight(IKCamera.device, value);
        }

        public string TriggerSoure
        {
            get
            {
                var triggersource = new StringBuilder();
                uint len = 0;
                GetTriggerSource(IKCamera.device, triggersource, ref len);
                return triggersource.ToString();
            }
            set { SetTriggerSource(IKCamera.device, value); }
        }

        public List<string> TriggerSoures
        {
            get
            {
                //GeneralConfigureCamera.GetTriggerSource(IKCamera.device, out var val);
                return []; //val.SupportEnumEntries.Select(e => e.Symbolic).ToList();
            }
        }
    }

    #endregion

}

