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
using static GeneralGrabNS.GeneralGrab;

namespace IKapGIGECL;

/// <summary>
/// 埃科相机实现类
/// </summary>
// 标记支持的品牌名称
[CameraManufacturer("埃科相机")]
public class IKapGIGECL: ICamera
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
    private readonly ConcurrentQueue<ITK_BUFFER_INFO> frameQueue = new();

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
    public CameraType Type => CameraType.LineScan;
    public bool IsConnected { get; }
    public IParameters Parameters { get; }

    #endregion

    #region 构造函数
    public IKapGIGECL(string sn)
    {
        SN = sn ?? throw new ArgumentNullException(nameof(sn));
        // 初始化海康相机的参数实现类
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
        var cam = new IKCamera
        {
            /// \~chinese 设置buffer个数				    \~english Set count of buffers
            g_bufferCount = 10,
            /// \~chinese 设置要采集的帧数			        \~english Set count of frames wanted
            g_grabCount = IKapC.ITKSTREAM_CONTINUOUS,
            /// \~chinese 设置g_SerialNumber为NULL，后续ConfigureCamera函数中使用index打开相机			    \~english Set
            /// g_SerialNumber to NULL, ConfigureCamera function will use index to open camera later
            g_SerialNumber = null,
            /// \~chinese 设置相机软触发为关闭			    \~english Disable softTrigger
            g_bSoftTriggerUsed = true,
            /// \~chinese 设置chunkdata为关闭			    \~english disable chunkdata
            bEnableChunkData = false,
            /// \~chinese 设置保存图片的文件名称			\~english Set filename of image to be saved
            g_saveFileName = "D:\\CImage.tif"
        };

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
        IntPtr timeOutPtr = Marshal.AllocHGlobal(4);
        Marshal.WriteInt32(timeOutPtr, 20000);
        res = IKapC.ItkDevSetPrm(device.g_hCamera, IKapC.ITKDEV_PRM_HEARTBEAT_TIMEOUT, timeOutPtr);
        IKUtils.CheckIKapC(res);
        Marshal.FreeHGlobal(timeOutPtr);

        // \~chinese 获取特征名称列表并配置参数设置方法				\~english Get list of features' name and select parameter
        // configure method
        GetAllConfigureMethods(device);

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
            device.g_isCXP = true;
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
            return -1;
        }

        device.g_hBoard = IKapBoard.IKapOpenWithSpecificInfo(info);
        if (device.g_hBoard == IntPtr.Zero)
            IKUtils.CheckIKapBoard(IKapBoard.IKStatus_OpenBoardFail);

        if (device.g_hBoard != IntPtr.Zero)
        {
            Console.Write("Please select camera without grabber.\n");
            IKapC.ItkManTerminate();
            IKUtils.pressEnterToExit();
        }
        /// \~chinese 配置相机触发方式		            \~english Configure trigger method of the camera
        SetSoftTriggerWithoutGrabber(device);

        /// \~chinese 配置相机chunkdata		            \~english Configure chunkdata of the camera
        SetChunkData(device, device.bEnableChunkData);

        /// \~chinese 创建数据流和缓冲区				    \~english Create data stream and buffer
        CreateStreamAndBuffer(device);

        /// \~chinese 注册回调函数                        \~english Register callback functions
        RegisterCallbackWithoutGrabber(device);



        /// \~chinese 注册设备移除回调函数		            \~english Register device removal callback function
        DeviceRemoveCallBackDelegate deviceRemoveCallbackDelegate = cbOnReconnect; 
        res = IKapC.ItkDevRegisterCallback(device.g_hCamera, "DeviceRemove", Marshal.GetFunctionPointerForDelegate(deviceRemoveCallbackDelegate), IntPtr.Zero);
        IKUtils.CheckIKapC(res);

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
        device.g_bSoftTriggerUsed = true;
        SetSoftTriggerWithoutGrabber(device);
        IKapC.ItkDevExecuteCommand(device.g_hCamera, "TriggerSoftware");
    }

    public void ContinuousGrab()
    {
        device.g_bSoftTriggerUsed = false;
        SetSoftTriggerWithoutGrabber(device);
    }

    public void StopContinuousGrab()
    {
        device.g_bSoftTriggerUsed = true;
        SetSoftTriggerWithoutGrabber(device);
    }

    public int StartGrabbing()
    {
        /// \~chinese 开始图像采集				        \~english Start grabbing images
        var res = IKapC.ItkStreamStart(device.g_hStream, device.g_grabCount);
        return (int)res;
    }

    public int StopGrabbing()
    {
        /// \~chinese 开始图像采集				        \~english Start grabbing images
        var res = IKapC.ItkStreamStop(device.g_hStream);
        return (int)res;
    }

    public void DisConnet()
    {
        _processThreadExit = true;

        StopGrabbing();

        /// \~chinese 清除回调函数				        \~english Unregister callback functions
        UnRegisterCallbackWithoutGrabber(device);

        /// \~chinese 关闭采集卡设备				        \~english Close frame grabber device
        var ret = IKapBoard.IKapClose(device.g_hBoard);
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
        DisConnet();
        /// \~chinese 释放 IKapBoard.IKapC 运行环境				\~english Release IKapBoard.IKapC runtime environment
        IKapC.ItkManTerminate();

        IKUtils.WaitEnterKeyInput();
    }
    #endregion

    #region 内部方法

    #region 图像回调


    /// \~chinese  注册回调函数		            \~english Register callback functions
    private void RegisterCallbackWithoutGrabber(IKCamera cam)
    {
        cam.IKCameraGCHandle = GCHandle.Alloc(cam);
        IntPtr IKCameraIntPtr = GCHandle.ToIntPtr(cam.IKCameraGCHandle);

        //注册采集开始回调
        cam.OnGrabStartDelegate = cbStartOfStream;
        var res = IKapC.ItkStreamRegisterCallback(cam.g_hStream, IKapC.ITKSTREAM_VAL_EVENT_TYPE_START_OF_STREAM, Marshal.GetFunctionPointerForDelegate(cam.OnGrabStartDelegate), IKCameraIntPtr);
        IKUtils.CheckIKapC(res);

        //注册采集超时回调
        cam.OnTimeoutDelegate = cbOnTimeOut;
        res = IKapC.ItkStreamRegisterCallback(cam.g_hStream, IKapC.ITKSTREAM_VAL_EVENT_TYPE_TIME_OUT, Marshal.GetFunctionPointerForDelegate(cam.OnTimeoutDelegate), IKCameraIntPtr);
        IKUtils.CheckIKapC(res);

        //注册采集丢帧回调
        cam.OnFrameLostDelegate = cbOnFrameLost;
        res = IKapC.ItkStreamRegisterCallback(cam.g_hStream, IKapC.ITKSTREAM_VAL_EVENT_TYPE_FRAME_LOST, Marshal.GetFunctionPointerForDelegate(cam.OnFrameLostDelegate), IKCameraIntPtr);
        IKUtils.CheckIKapC(res);

        //注册采集结束回调
        cam.OnGrabStopDelegate = cbEndOfStream;
        res = IKapC.ItkStreamRegisterCallback(cam.g_hStream, IKapC.ITKSTREAM_VAL_EVENT_TYPE_END_OF_STREAM, Marshal.GetFunctionPointerForDelegate(cam.OnGrabStopDelegate), IKCameraIntPtr);
        IKUtils.CheckIKapC(res);

        //注册帧结束回调
        cam.OnFrameReadyDelegate = cbOnEndOfFrame;
        res = IKapC.ItkStreamRegisterCallback(cam.g_hStream, IKapC.ITKSTREAM_VAL_EVENT_TYPE_END_OF_FRAME, Marshal.GetFunctionPointerForDelegate(cam.OnFrameReadyDelegate), IKCameraIntPtr);
        IKUtils.CheckIKapC(res);
    }

    /// \~chinese  清除回调函数		            \~english Unregister callback functions
    private  void UnRegisterCallbackWithoutGrabber(IKCamera cam)
    {
        //注销回调函数
        IKapC.ItkStreamUnregisterCallback(cam.g_hStream, IKapC.ITKSTREAM_VAL_EVENT_TYPE_START_OF_STREAM);
        IKapC.ItkStreamUnregisterCallback(cam.g_hStream, IKapC.ITKSTREAM_VAL_EVENT_TYPE_END_OF_STREAM);
        IKapC.ItkStreamUnregisterCallback(cam.g_hStream, IKapC.ITKSTREAM_VAL_EVENT_TYPE_FRAME_LOST);
        IKapC.ItkStreamUnregisterCallback(cam.g_hStream, IKapC.ITKSTREAM_VAL_EVENT_TYPE_TIME_OUT);
        IKapC.ItkStreamUnregisterCallback(cam.g_hStream, IKapC.ITKSTREAM_VAL_EVENT_TYPE_END_OF_FRAME);

        cam.IKCameraGCHandle.Free();
    }


    private void cbStartOfStream(uint eventType, IntPtr pContext)
    {
        Console.Write("On start of stream. \n");
        if (pContext != IntPtr.Zero)
        {
            GCHandle camGCHanle = GCHandle.FromIntPtr(pContext);
            IKCamera cam = (IKCamera)camGCHanle.Target; //前提：注册回调传入的是IKCamera object
            if (cam == null)
            {
                return;
            }
            Console.Write("On start of stream of camera with serialNumber:{0}. \n", cam.g_devInfo.SerialNumber);
        }
    }

    private void cbEndOfStream(uint eventType, IntPtr pContext)
    {
        Console.Write("On end of stream. \n");
        if (pContext != IntPtr.Zero)
        {
            GCHandle camGCHanle = GCHandle.FromIntPtr(pContext);
            IKCamera cam = (IKCamera)camGCHanle.Target; //前提：注册回调传入的是IKCamera object
            if (cam == null)
            {
                return;
            }
            Console.Write("On end of stream of camera with serialNumber:{0}. \n", cam.g_devInfo.SerialNumber);
        }
    }

    private void cbOnEndOfFrame(uint eventType, IntPtr pContext)
    {
        Console.Write("On end of frame. \n");

        if (pContext != IntPtr.Zero)
        {
            GCHandle camGCHanle = GCHandle.FromIntPtr(pContext);
            IKCamera cam = (IKCamera)camGCHanle.Target; //前提：注册回调传入的是IKCamera object
            if (cam == null)
            {
                return;
            }
            Console.Write("On end of frame of camera with serialNumber:{0}. \n", cam.g_devInfo.SerialNumber);

            /// 获取缓冲区对象
            /// get the ITKBUFFER object
            ITKBUFFER hBuffer = new ITKBUFFER();
            var res = IKapC.ItkStreamGetCurrentBuffer(cam.g_hStream, ref hBuffer);
            IKUtils.CheckIKapC(res);
            /// 获取缓冲区信息
            /// get the info of buffer
            ITK_BUFFER_INFO bufferInfo = new ITK_BUFFER_INFO(); 
            res = IKapC.ItkBufferGetInfo(hBuffer, bufferInfo);
            IKUtils.CheckIKapC(res);
            lock (this)
            {
                frameQueue.Enqueue(bufferInfo);
            }
        }
    }

    private void cbOnTimeOut(uint eventType, IntPtr pContext)
    {
        Console.Write("on time out. \n");
        if (pContext != IntPtr.Zero)
        {
            GCHandle camGCHanle = GCHandle.FromIntPtr(pContext);
            IKCamera cam = (IKCamera)camGCHanle.Target; //前提：注册回调传入的是IKCamera object
            if (cam == null)
            {
                return;
            }
            Console.Write("on time out of camera with serialNumber:{0}. \n", cam.g_devInfo.SerialNumber);
        }
    }

    private void cbOnFrameLost(uint eventType, IntPtr pContext)
    {
        Console.Write("on frame lost. \n");
        if (pContext != IntPtr.Zero)
        {
            GCHandle camGCHanle = GCHandle.FromIntPtr(pContext);
            IKCamera cam = (IKCamera)camGCHanle.Target; //前提：注册回调传入的是IKCamera object
            if (cam == null)
            {
                return;
            }
            Console.Write("on frame lost of camera with serialNumber:{0}. \n", cam.g_devInfo.SerialNumber);

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
                    if (!frameQueue.TryDequeue(out var bufferInfo)) continue;

                    Console.Write("On end of frame. \n");

                    uint bufferStatus = bufferInfo.State;
                    /// \~chinese 当图像缓冲区满或者图像缓冲区非满但是无法采集完整的一帧图像时 /// \~english When buffer is full or buffer is not full but cannot grab a complete frame of image
                    if (bufferStatus == IKapC.ITKBUFFER_VAL_STATE_FULL || bufferStatus == IKapC.ITKBUFFER_VAL_STATE_UNCOMPLETED)
                    {

                        /// \~chinese 保存图像				    \~english Save image
                        //res = IKapC.ItkBufferSave(hBuffer, cam.g_saveFileName,IKapC.ITKBUFFER_VAL_TIFF);
                        //IKUtils.CheckIKapC(res);

                        // 转换为VisionPro图像
                        // 
                        // Convert to VisionPro format
                        bool bIsValidImageBit = false;
                        bool bIsColorImage = false;
                        ulong nImageBit = bufferInfo.ImagePixelDepth;
                        if (nImageBit == 8 && nImageBit == 24)
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
                            var image = CreateCogImage(bufferInfo.ImageAddress, (int)bufferInfo.ImageWidth, (int)bufferInfo.ImageHeight, bIsColorImage);
                            frameGrabedEvent?.Invoke(this, image);
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
        CogImage8Root greyRoot = new CogImage8Root();
        greyRoot.Initialize(width, height, buffer, width, null);

        // Create CogImage8Grey object and set root
        CogImage8Grey greyImage = new CogImage8Grey();
        greyImage.SetRoot(greyRoot);

        return greyImage;
    }


    private unsafe ICogImage CreateColorImage(IntPtr buffer, int width, int height)
    {

        // Assuming buffer contains interleaved RGB data (24 bits per pixel)
        byte* pixelData = (byte*)buffer.ToPointer();

        byte[] redPlane = new byte[width * height];
        byte[] greenPlane = new byte[width * height];
        byte[] bluePlane = new byte[width * height];

        for (int i = 0; i < width * height; i++)
        {
            redPlane[i] = pixelData[3 * i];
            greenPlane[i] = pixelData[3 * i + 1];
            bluePlane[i] = pixelData[3 * i + 2];
        }

        // Allocate unmanaged memory for each color plane
        IntPtr redPtr = Marshal.AllocHGlobal(redPlane.Length);
        IntPtr greenPtr = Marshal.AllocHGlobal(greenPlane.Length);
        IntPtr bluePtr = Marshal.AllocHGlobal(bluePlane.Length);

        // Copy managed arrays to unmanaged memory
        Marshal.Copy(redPlane, 0, redPtr, redPlane.Length);
        Marshal.Copy(greenPlane, 0, greenPtr, greenPlane.Length);
        Marshal.Copy(bluePlane, 0, bluePtr, bluePlane.Length);

        CogImage8Root redRoot = new CogImage8Root();
        CogImage8Root greenRoot = new CogImage8Root();
        CogImage8Root blueRoot = new CogImage8Root();

        redRoot.Initialize(width, height, redPtr, width, null);
        greenRoot.Initialize(width, height, greenPtr, width, null);
        blueRoot.Initialize(width, height, bluePtr, width, null);

        CogImage24PlanarColor image = new CogImage24PlanarColor();
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
        UnRegisterCallbackWithoutGrabber(device);

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

    private class IKapParameters(IKapGIGECL IKCamera) : IParameters
    {
        public double ExposureTime
        {
            get
            {
                double val = 0;
                GetExposureTime(IKCamera.device, ref val);
                return val;
            }
            set => SetExposureTime(IKCamera.device,value);
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
        public int Width {
            get
            {
                long val = 0;
                GetWidth(IKCamera.device, ref val);
                return (int)val;
            }
            set => SetWidth(IKCamera.device, value);
        }
        public int Height {
            get
            {
                long val = 0;
                GetHeight(IKCamera.device, ref val);
                return (int) val;
            }
            set => SetHeight(IKCamera.device, value);
        }

        public string TriggerSoure
        {
            get
            {
                var triggersource = new StringBuilder();
                uint len = 0;
                GetTriggerSource(IKCamera.device, triggersource,ref len);
                return triggersource.ToString();
            }
            set
            {
                IKCamera.device.g_bSoftTriggerUsed = true;
                SetSoftTriggerWithoutGrabber(IKCamera.device);
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

