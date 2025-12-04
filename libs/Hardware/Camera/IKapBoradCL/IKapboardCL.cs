using Cognex.VisionPro;
using HardwareCameraNet;
using IKapBoardDotNet;
using IKapCDotNet;
using IKNS;
using System;
using System.Collections.Concurrent;
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
public class IKapBoradCL : ICamera
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
    /// <summary>
    /// ch: 帧缓存队列 | en: frame queue for process
    /// </summary>
    private readonly ConcurrentQueue<IntPtr> frameQueue = new();

    /// <summary>
    /// ch: 异步处理线程 | asynchronous processing thread
    /// </summary>
    private Thread _asyncProcessThread;

    /// <summary>
    /// ch: 队列图像数量上限 | en: maximum number of frames in the queue
    /// </summary>
    private const uint _maxQueueSize = 200;
    /// <summary>
    /// ch: 异步处理线程退出标志 false 不退出 | en: Flag to notify the  processing thread to exit
    /// </summary>
    private volatile bool _processThreadExit;

    private readonly Stopwatch _stopwatch = new();



    #endregion

    #region 接口属性
    // 图像回调事件（需显式实现事件添加/移除逻辑，确保线程安全）
    private event EventHandler<ICogImage> frameGrabedEvent;
    public event EventHandler<ICogImage> FrameGrabedEvent
    {
        add => frameGrabedEvent += value;
        remove => frameGrabedEvent -= value;
    }
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
        var cam = new IKCamera();

        cam.g_bufferCount = 10;

        cam.g_saveFileName = "D:\\CImage.tif";

        cam.g_SerialNumber = null;

        cam.g_bSoftTriggerUsed = false;

        cam.g_bLoadGrabberConfig = false;

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

            if(di.SerialNumber == "" || gDeviceInfos.ContainsKey(di.SerialNumber))
                continue;
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
        Console.Write("Using camera: serial: {0}, name: {1}, interface: {2}.\n", di.SerialNumber, di.FullName, di.DeviceClass);
        var res = IKapC.ItkDevOpen(i, IKapC.ITKDEV_VAL_ACCESS_MODE_EXCLUSIVE, ref device.g_hCamera);
        IKUtils.CheckIKapC(res);

        // \~chinese 设置相机的超时时间				\~english set timeout of camera
        var timeOutPtr = Marshal.AllocHGlobal(4);
        Marshal.WriteInt32(timeOutPtr, 20000);
        res = IKapC.ItkDevSetPrm(device.g_hCamera, IKapC.ITKDEV_PRM_HEARTBEAT_TIMEOUT, timeOutPtr);
        IKUtils.CheckIKapC(res);
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
            IKUtils.CheckIKapC(res);
            info = IKapC.get_itk_cxp_dev_info_IntPtr(cxp_cam_info);
            device.g_isCXP = true;
        }
        else if (string.Compare(di.DeviceClass, "GigEVisionBoard") == 0)
        {
            // 获取GigEVision相机设备信息				    \~english Get GigEVision camera device information
            res = IKapC.ItkManGetGVBDeviceInfo(i, gvb_cam_info);
            IKUtils.CheckIKapC(res);
            info = IKapC.get_itk_gvb_dev_info_IntPtr(gvb_cam_info);
        }
        else if (string.Compare(di.DeviceClass, "CameraLink") == 0)
        {
            // 获取CameraLink相机设备信息				    \~english Get CameraLink camera device information
            res = IKapC.ItkManGetCLDeviceInfo(i, cl_cam_info);
            IKUtils.CheckIKapC(res);
            info = IKapC.get_itk_cl_dev_info_IntPtr(cl_cam_info);
        }
        else
        {
            Console.Write("Not camera Type");
            return -1;
        }

        device.g_hBoard = IKapBoard.IKapOpenWithSpecificInfo(info);
        if (device.g_hBoard == IntPtr.Zero)
            IKUtils.CheckIKapBoard(IKapBoard.IKStatus_OpenBoardFail);

        if (device.g_hBoard == IntPtr.Zero)
        {
            Console.Write("Please select camera with grabber.\n");
            IKapC.ItkManTerminate();
            IKUtils.pressEnterToExit();
            return -1;
        }

        /// \~chinese 注册设备移除回调函数		            \~english Register device removal callback function
        DeviceRemoveCallBackDelegate deviceRemoveCallbackDelegate = cbOnReconnect;
        res = IKapC.ItkDevRegisterCallback(device.g_hCamera, "DeviceRemove", Marshal.GetFunctionPointerForDelegate(deviceRemoveCallbackDelegate), IntPtr.Zero);
        IKUtils.CheckIKapC(res);

        // ch: 配置采集卡参数 | en: Configure frame grabber parameters
        ConfigureFrameGrabber(device);


        //chinese 注册回调函数                        \~english Register callback functions.
        RegisterCallbackWithGrabber(device);

        //chinese 配置相机触发方式		            \~english Configure trigger method of the camera
        SetSoftTriggerWithGrabber(device);

        if (_asyncProcessThread == null)
        {
            _processThreadExit = false;
            _asyncProcessThread = new Thread(AsyncProcessThread)
            {
                IsBackground = true
            };
            _asyncProcessThread.Start();
        }
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

    public void DisConnet()
    {
        _processThreadExit = true;

        /// \~chinese 停止图像采集				        \~english Stop grabbing images
        var ret = IKapBoard.IKapStopGrab(device.g_hBoard);
        IKUtils.CheckIKapBoard(ret);

        /// \~chinese 清除回调函数				        \~english Unregister callback functions
        UnRegisterCallbackWithGrabber(device);

        /// \~chinese 关闭采集卡设备				        \~english Close frame grabber device
        ret = IKapBoard.IKapClose(device.g_hBoard);
        IKUtils.CheckIKapBoard(ret);

        /// \~chinese 关闭相机设备				        \~english Close camera device
        var res = IKapC.ItkDevClose(device.g_hCamera);
        IKUtils.CheckIKapC(res);

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
    }

    public void Close()
    {
        _processThreadExit = true;

        /// \~chinese 停止图像采集				        \~english Stop grabbing images
        var ret = IKapBoard.IKapStopGrab(device.g_hBoard);
        IKUtils.CheckIKapBoard(ret);

        /// \~chinese 清除回调函数				        \~english Unregister callback functions
        UnRegisterCallbackWithGrabber(device);

        /// \~chinese 关闭采集卡设备				        \~english Close frame grabber device
        ret = IKapBoard.IKapClose(device.g_hBoard);
        IKUtils.CheckIKapBoard(ret);

        /// \~chinese 关闭相机设备				        \~english Close camera device
        var res = IKapC.ItkDevClose(device.g_hCamera);
        IKUtils.CheckIKapC(res);

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

        /// \~chinese 释放 IKapBoard.IKapC 运行环境				\~english Release IKapBoard.IKapC runtime environment
        IKapC.ItkManTerminate();

        IKUtils.WaitEnterKeyInput();
    }
    #endregion

    #region 内部方法

    #region 图像回调

    private void RegisterCallbackWithGrabber(IKCamera cam)
    {
        cam.IKCameraGCHandle = GCHandle.Alloc(cam);
        var IKCameraIntPtr = GCHandle.ToIntPtr(cam.IKCameraGCHandle);

        cam.OnBoardGrabStartDelegate = OnGrabStart;
        var ret = IKapBoard.IKapRegisterCallback(cam.g_hBoard, (uint)IKapBoard.IKEvent_GrabStart, Marshal.GetFunctionPointerForDelegate(cam.OnBoardGrabStartDelegate), IntPtr.Zero);
        IKUtils.CheckIKapBoard(ret);

        cam.OnBoardFrameReadyDelegate = OnFrameReady;
        ret = IKapBoard.IKapRegisterCallback(cam.g_hBoard, (uint)IKapBoard.IKEvent_FrameReady, Marshal.GetFunctionPointerForDelegate(cam.OnBoardFrameReadyDelegate), IKCameraIntPtr);
        IKUtils.CheckIKapBoard(ret);

        cam.OnBoardFrameLostDelegate = OnFrameLost;
        ret = IKapBoard.IKapRegisterCallback(cam.g_hBoard, (uint)IKapBoard.IKEvent_FrameLost, Marshal.GetFunctionPointerForDelegate(cam.OnBoardFrameLostDelegate), IntPtr.Zero);
        IKUtils.CheckIKapBoard(ret);

        cam.OnBoardTimeoutDelegate = OnTimeout;
        ret = IKapBoard.IKapRegisterCallback(cam.g_hBoard, (uint)IKapBoard.IKEvent_TimeOut, Marshal.GetFunctionPointerForDelegate(cam.OnBoardTimeoutDelegate), IntPtr.Zero);
        IKUtils.CheckIKapBoard(ret);

        cam.OnBoardGrabStopDelegate = OnGrabStop;
        ret = IKapBoard.IKapRegisterCallback(cam.g_hBoard, (uint)IKapBoard.IKEvent_GrabStop, Marshal.GetFunctionPointerForDelegate(cam.OnBoardGrabStopDelegate), IntPtr.Zero);
        IKUtils.CheckIKapBoard(ret);

    }

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

    private void OnFrameReady(IntPtr pContext)
    {
        Console.Write("On end of frame. \n");

        if (pContext != IntPtr.Zero)
        {
            var camGCHanle = GCHandle.FromIntPtr(pContext);
            var cam = (IKCamera)camGCHanle.Target; //前提：注册回调传入的是IKCamera object
            if (cam == null)
            {
                return;
            }
            Console.Write("Grab frame ready of camera with serialNumber:{0}.\n", cam.g_devInfo.SerialNumber);
            lock (this)
            {
                if (frameQueue.Count <= _maxQueueSize)
                {
                    //ch: 添加到队列
                    frameQueue.Enqueue(pContext);
                }
            }
        }
    }

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


    void AsyncProcessThread()
    {
        while (!_processThreadExit)
        {
            try
            {
                lock (this)
                {
                    _stopwatch.Restart();
                    if (!frameQueue.TryDequeue(out var pContext)) continue;

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
                        int nFrameIndex = 0;
                        IKAPBUFFERSTATUS status = new IKAPBUFFERSTATUS();

                        var ret = IKapBoard.IKapGetInfo(cam.g_hBoard, (uint)IKapBoard.IKP_CURRENT_BUFFER_INDEX, ref nFrameIndex);
                        IKUtils.CheckIKapBoard(ret);

                        ret = IKapBoard.IKapGetBufferStatus(cam.g_hBoard, nFrameIndex, status);
                        IKUtils.CheckIKapBoard(ret);

                        /// \~chinese 当图像缓冲区满时				      \~english When the buffer is full
                        if (status.uFull == 1)
                        {

                            /// \~chinese 获取缓冲区地址				  \~english Get the buffer address
                            ret = IKapBoard.IKapGetBufferAddress(cam.g_hBoard, nFrameIndex, ref pUserBuffer);
                            IKUtils.CheckIKapBoard(ret);

                            /// \~chinese 保存图像				       \~english Save image
                            /*
                            ret=IKapBoard.IKapSaveBuffer(cam.g_hBoard,nFrameIndex,cam.g_saveFileName,(uint)IKapBoard.IKP_DEFAULT_COMPRESSION);
                            IKUtils.CheckIKapBoard(ret);
                            */

                            // 转换为VisionPro图像
                            // 
                            // Convert to VisionPro format
                            int nFrameWidth = 0;
                            int nFrameHeight = 0;
                            int nImageBit = 0; //像素深度
                            IKapBoard.IKapGetInfo(cam.g_hBoard, (uint)IKapBoard.IKP_IMAGE_WIDTH, ref nFrameWidth);
                            IKapBoard.IKapGetInfo(cam.g_hBoard, (uint)IKapBoard.IKP_IMAGE_HEIGHT, ref nFrameHeight);
                            IKapBoard.IKapGetInfo(cam.g_hBoard, (uint)IKapBoard.IKP_BOARD_BIT, ref nImageBit);
                            bool bIsValidImageBit = false;
                            bool bIsColorImage = false; //图像位深是否为24
                            if (nImageBit == 8 || nImageBit == 24)
                            {
                                //ImageHelper 实现8/24图像位深的实现
                                bIsValidImageBit = true;
                                if (nImageBit == 24)
                                {
                                    bIsColorImage = true;
                                }
                            }
                            if (bIsValidImageBit)
                            {
                                var image = CreateCogImage(pUserBuffer, nFrameWidth, nFrameHeight, bIsColorImage);
                                frameGrabedEvent?.Invoke(this, image);
                            }
                        }
                    }
                    GC.Collect();
                    Console.WriteLine("转换耗时:{0}", _stopwatch.ElapsedMilliseconds);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("AsyncProcessThread exception: " + e.Message);
            }
            Thread.Sleep(2);
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

    private void cbOnReconnect(IntPtr pContext, IntPtr eventInfo)
    {
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

    private void ReconnectProcess()
    {
        while (!IsConnected)
        {
            if (Open() == (int)IKapC.ITKSTATUS_OK)
            {
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

        public double MaxExposureTime => 0;

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

        public double MaxGain => 0;
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
            set
            {

                SetTriggerSource(IKCamera.device, value);
            }
        }

        public List<string> TriggerSoures
        {
            get
            {
                //GeneralConfigureCamera.GetTriggerSource(IKCamera.device, out var val);
                return [];//val.SupportEnumEntries.Select(e => e.Symbolic).ToList();
            }
        }
    }
    #endregion

}

