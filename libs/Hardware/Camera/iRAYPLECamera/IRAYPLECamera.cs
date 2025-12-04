using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using Cognex.VisionPro;
using HardwareCameraNet;
using MVSDK_Net;

namespace iRAYPLECamera;

// 标记支持的品牌名称
[CameraManufacturer("华睿面阵相机")]
public class IRAYPLECamera : ICamera, IDisposable
{

    #region 相机属性
    private readonly MyCamera device = new();
    private IMVDefine.IMV_ConnectCallBack pConnectCallBack;
    private IMVDefine.IMV_FrameCallBack frameCallBack;
    private readonly ConcurrentQueue<IMVDefine.IMV_Frame> frameQueue = new();
    private Thread _asyncProcessThread;
    private static readonly Dictionary<string, string> gDevInfoCameraKeys = new();

    private readonly Stopwatch _stopwatch = new();
    /// <summary>
    /// ch: 队列图像数量上限 | en: maximum number of frames in the queue
    /// </summary>
    private const uint _maxQueueSize = 200;
    /// <summary>
    /// ch: 异步处理线程退出标志 false 不退出 | en: Flag to notify the  processing thread to exit
    /// </summary>
    private volatile bool _processThreadExit;
    /// <summary>
    /// 标记是否已释放，防止多次释放
    /// </summary>
    private bool _disposed;
    #endregion

    #region 属性
    // 图像回调事件（需显式实现事件添加/移除逻辑，确保线程安全）
    private event EventHandler<ICogImage> frameGrabedEvent;
    public event EventHandler<ICogImage> FrameGrabedEvent
    {
        add => frameGrabedEvent += value;
        remove => frameGrabedEvent -= value;
    }
    public event EventHandler<bool> DisConnetEvent;
    
    public string SN { get; }
    public CameraType Type => CameraType.面阵相机;

    public bool IsConnected => device.IMV_IsOpen();

    
    public IParameters Parameters { get; }
    #endregion

    #region 构造函数

    public IRAYPLECamera(string sn)
    {
        SN = sn ?? throw new ArgumentNullException(nameof(sn));
        // 初始化海康相机的参数实现类
        Parameters = new DahuaParameters(this);
    }
    #endregion

    #region 静态枚举方法
    public static List<string> EnumerateDevices()
    {
        //枚举设备
        //Discover device
        IMVDefine.IMV_DeviceList deviceList = new IMVDefine.IMV_DeviceList();
        IMVDefine.IMV_EInterfaceType interfaceTp = IMVDefine.IMV_EInterfaceType.interfaceTypeGige | IMVDefine.IMV_EInterfaceType.interfaceTypeUsb3;
        var res = MyCamera.IMV_EnumDevices(ref deviceList, (uint)interfaceTp);
        if (res == IMVDefine.IMV_OK)
        {
            if (deviceList.nDevNum > 0)
            {
                for (int i = 0; i < deviceList.nDevNum; i++)
                {
                    IMVDefine.IMV_DeviceInfo devInfo =
                        (IMVDefine.IMV_DeviceInfo)
                        Marshal.PtrToStructure(
                            deviceList.pDevInfo + Marshal.SizeOf(typeof(IMVDefine.IMV_DeviceInfo)) * i,
                            typeof(IMVDefine.IMV_DeviceInfo));

                    if (devInfo.manufactureInfo != "Huaray Technology") continue;

                    if (!gDevInfoCameraKeys.ContainsKey(devInfo.serialNumber))
                        gDevInfoCameraKeys.Add(devInfo.serialNumber, devInfo.cameraKey);
                }
                return gDevInfoCameraKeys.Keys.ToList();
            }
        }
        Console.WriteLine("Enumeration devices failed! ErrorCode:[{0}]", res);
        return [];
    }
    #endregion

    #region 实现ICamera接口的方法



    public int Open()
    {
        try
        {
            // 创建设备句柄
            // Create Device Handle
            var res = device.IMV_CreateHandle(IMVDefine.IMV_ECreateHandleMode.modeByCameraKey, 0, gDevInfoCameraKeys[SN]);
            if (res != IMVDefine.IMV_OK)
            {
                Console.WriteLine("Create devHandle failed! ErrorCode[{0}]", res);
                return res;
            }
            // 打开相机设备 
            // Connect to camera 
            res = device.IMV_Open();
            if (res != IMVDefine.IMV_OK)
            {
                Console.WriteLine("Open camera failed! ErrorCode:[{0}]", res);
                return res;
            }
            SetSoftwareTrigger();
            // 注册数据帧回调函数
            // Register data frame callback function
            frameCallBack = onGetFrame;
            res = device.IMV_AttachGrabbing(frameCallBack, IntPtr.Zero);
            if (res != IMVDefine.IMV_OK)
            {
                Console.WriteLine("Attach grabbing failed! ErrorCode:[{0}]", res);
                return res;
            }
            // 设备连接状态事件回调函数
            // Device connection status event callback function
            pConnectCallBack = onDeviceLinkNotify;
            res = device.IMV_SubscribeConnectArg(pConnectCallBack, IntPtr.Zero);
            if (res != IMVDefine.IMV_OK)
            {
                Console.WriteLine("SubscribeConnectArg failed! ErrorCode:[{0}]", res);
                return res;
            }
            //ch：创建异步处理线程 | en: Create an asynchronous processing thread
            if (_asyncProcessThread == null)
            {
                _processThreadExit = false;
                _asyncProcessThread = new Thread(AsyncProcessThread) { IsBackground = true };
                _asyncProcessThread.Start();
            }
            res = StartGrabbing();
            return IMVDefine.IMV_OK != res ? res : IMVDefine.IMV_OK;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return -1;
        }
    }

    private void SetSoftwareTrigger()
    {
        // 设置触发源为软触发 
        // Set trigger source to Software 
        int res = device.IMV_SetEnumFeatureSymbol("TriggerSource", "Software");
        if (res != IMVDefine.IMV_OK)
        {
            Console.WriteLine("Set triggerSource value failed! ErrorCode[{0}]", res);
            return;
        }

        // 设置触发器 
        // Set trigger selector to FrameStart 
        res = device.IMV_SetEnumFeatureSymbol("TriggerSelector", "FrameStart");
        if (res != IMVDefine.IMV_OK)
        {
            Console.WriteLine("Set triggerSelector value failed! ErrorCode[{0}]", res);
            return;
        }

        // 设置触发模式 
        // Set trigger mode to On 
        res = device.IMV_SetEnumFeatureSymbol("TriggerMode", "On");
        if (res != IMVDefine.IMV_OK)
        {
            Console.WriteLine("Set triggerMode value failed! ErrorCode[{0}]", res);
        }
    }

    public void SoftwareTriggerOnce()
    {
        SetSoftwareTrigger();
        // 开始采集 
        // ExecuteCommand TriggerSoftware
        var res = device.IMV_ExecuteCommandFeature("TriggerSoftware");
        if (res != IMVDefine.IMV_OK)
        {
            Console.WriteLine("Set triggerMode value failed! ErrorCode[{0}]", res);
        }
    }

    public void ContinuousGrab()
    {
        // set TriggerMode Off
        device.IMV_SetEnumFeatureSymbol("TriggerMode", "Off");
    }

    public void StopContinuousGrab()
    {
        // set TriggerMode Off
        device.IMV_SetEnumFeatureSymbol("TriggerMode", "On");
        lock (this)
        {
            while (frameQueue.TryDequeue(out var _)) { }
        }
    }

    public int StartGrabbing()
    {
        if (device.IMV_IsGrabbing()) return IMVDefine.IMV_OK;
        // 开始拉流 
        // Start grabbing 
        var res = device.IMV_StartGrabbing();
        if (res != IMVDefine.IMV_OK)
        {
            Console.WriteLine("Start grabbing failed! ErrorCode:[{0}]", res);
            return res;
        }

        return IMVDefine.IMV_OK;
    }

    public int StopGrabbing()
    {
        if (!device.IMV_IsGrabbing()) return IMVDefine.IMV_OK;
        // 停止拉流 
        // Stop grabbing 
        var res = device.IMV_StopGrabbing();
        if (res != IMVDefine.IMV_OK)
        {
            Console.WriteLine("Stop grabbing failed! ErrorCode:[{0}]", res);
            return res;
        }
        return IMVDefine.IMV_OK;
    }

    public void Close()
    {
        if (_disposed) return;
        _disposed = true;
        //关闭相机
        //Close camera 
        //ch: 通知异步处理线程退出 | en: Notify the thread to exit
        _processThreadExit = true;
        StopGrabbing();
        var res = device.IMV_Close();
        if (res != IMVDefine.IMV_OK)
        {
            Console.WriteLine("Close camera failed! ErrorCode:[{0}]", res);
        }

        // 销毁设备句柄
        // Destroy Device Handle
        res = device.IMV_DestroyHandle();
        if (res != IMVDefine.IMV_OK)
        {
            Console.WriteLine("Destroy camera failed! ErrorCode[{0}]", res);
        }
    }

    /// <summary>
    /// 实现IDisposable接口，释放资源时自动调用Close
    /// </summary>
    public void Dispose()
    {
        Close();
        GC.SuppressFinalize(this);
    }
    #endregion

    #region 内部方法
    #region 断线重连
    // 连接事件通知回调函数
    // Connect event notify callback function
    private void onDeviceLinkNotify(ref IMVDefine.IMV_SConnectArg pConnectArg, IntPtr pUser)
    {
        switch (pConnectArg.EvType)
        {
            // 断线通知
            // offLine notify 
            case IMVDefine.IMV_EVType.offLine:
                DisConnetEvent?.Invoke(this, true);
                Console.WriteLine($"[{SN}]Device disconnect!");
                break;
            // 上线通知
            // onLine notify 
            case IMVDefine.IMV_EVType.onLine:
                {
                    device.IMV_Close();
                    if (IMVDefine.IMV_OK != Open())
                    {
                        Console.WriteLine($"[{SN}]Device Reconnect Fail!");
                        return;
                    }
                        
                    DisConnetEvent?.Invoke(this, false);
                    break;
                }
        }
    }
    #endregion

    #region 图像回调
    // 数据帧回调函数
    // Data frame callback function
    private void onGetFrame(ref IMVDefine.IMV_Frame frame, IntPtr pUser)
    {
        try
        {
            if (frame.frameHandle == IntPtr.Zero)
            {
                Console.WriteLine("frame is NULL");
                return;
            }


            if (frame.pData == IntPtr.Zero || frame.frameInfo.size == 0)
            {
                Console.WriteLine("Invalid frame data");
                // 即使数据无效，也需要释放帧
                device.IMV_ReleaseFrame(ref frame);
                return;
            }

            IMVDefine.IMV_Frame frameclone = new IMVDefine.IMV_Frame();
            int res = device.IMV_CloneFrame(ref frame, ref frameclone);

            if (res != IMVDefine.IMV_OK)
            {
                Console.WriteLine($"Clone frame failed: {res}");
                device.IMV_ReleaseFrame(ref frame);
                return;
            }
            lock (this)
            {
                if (frameQueue.Count <= _maxQueueSize)
                    frameQueue.Enqueue(frameclone);
            }

            // 释放图像缓存
            // Free image buffer
            device.IMV_ReleaseFrame(ref frame);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    private void AsyncProcessThread()
    {
        while (!_processThreadExit)
        {
            lock (this)
            {
                if (frameQueue.TryDequeue(out var frame))
                {
                    try
                    {
                        _stopwatch.Restart();
                        Bitmap bmp = null;
                        ConvertToBitmap(ref frame, ref bmp);

                        if (bmp != null)
                        {
                            ICogImage img;
                            if (bmp.PixelFormat == PixelFormat.Format24bppRgb)
                            {
                                img = new CogImage24PlanarColor(bmp);
                            }
                            else
                            {
                                img = new CogImage8Grey(bmp);
                            }

                            frameGrabedEvent?.Invoke(this, img);
                            _stopwatch.Stop();
                            bmp.Dispose();
                            GC.Collect();
                            Console.WriteLine("回调转换耗时:{0}", _stopwatch.ElapsedMilliseconds);
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                    finally
                    {
                        //释放克隆帧
                        var res = device.IMV_ReleaseFrame(ref frame);
                        if (res != IMVDefine.IMV_OK)
                        {
                            Console.WriteLine($"Release cloned frame failed: {res}");
                        }
                    }
                }
            }

            Thread.Sleep(1);
        }
    }

    private bool isColor(IMVDefine.IMV_EPixelType format)
    {
        if (format == IMVDefine.IMV_EPixelType.gvspPixelMono8 || format == IMVDefine.IMV_EPixelType.gvspPixelMono10 ||
            format == IMVDefine.IMV_EPixelType.gvspPixelMono12 || format == IMVDefine.IMV_EPixelType.gvspPixelMono10Packed ||
            format == IMVDefine.IMV_EPixelType.gvspPixelMono12Packed)
        {
            return false;
        }

        return true;
    }

    private void ConvertToBitmap(ref IMVDefine.IMV_Frame frame, ref Bitmap bitmap)
    {
        try
        {
            BitmapData bmpData;
            Rectangle bitmapRect = new Rectangle();
            IMVDefine.IMV_PixelConvertParam stPixelConvertParam = new IMVDefine.IMV_PixelConvertParam();

            // mono8和BGR8裸数据不需要转码
            // mono8 and BGR8 raw data is not need to convert
            IntPtr pDstRGB;
            if (frame.frameInfo.pixelFormat != IMVDefine.IMV_EPixelType.gvspPixelMono8
                && frame.frameInfo.pixelFormat != IMVDefine.IMV_EPixelType.gvspPixelBGR8)
            {
                //转目标内存 彩色
                var ImgSize = (int)frame.frameInfo.width * (int)frame.frameInfo.height * 3;
                //当内存申请失败，返回false
                try
                {
                    pDstRGB = Marshal.AllocHGlobal(ImgSize);
                }
                catch
                {
                    return;
                }

                if (pDstRGB == IntPtr.Zero)
                {
                    return;
                }

                // 图像转换成BGR8
                // convert image to BGR8
                stPixelConvertParam.nWidth = frame.frameInfo.width;
                stPixelConvertParam.nHeight = frame.frameInfo.height;
                stPixelConvertParam.ePixelFormat = frame.frameInfo.pixelFormat;
                stPixelConvertParam.pSrcData = frame.pData;
                stPixelConvertParam.nSrcDataLen = frame.frameInfo.size;
                stPixelConvertParam.nPaddingX = frame.frameInfo.paddingX;
                stPixelConvertParam.nPaddingY = frame.frameInfo.paddingY;
                stPixelConvertParam.eBayerDemosaic = IMVDefine.IMV_EBayerDemosaic.demosaicNearestNeighbor;
                stPixelConvertParam.eDstPixelFormat = IMVDefine.IMV_EPixelType.gvspPixelBGR8;
                stPixelConvertParam.pDstBuf = pDstRGB;
                stPixelConvertParam.nDstBufSize = (uint)ImgSize;
                var res = device.IMV_PixelConvert(ref stPixelConvertParam);
                if (res != IMVDefine.IMV_OK)
                {
                    Console.WriteLine("image convert to BGR failed!");
                    return;
                }
            }
            else
            {
                pDstRGB = frame.pData;
            }

            if (frame.frameInfo.pixelFormat == IMVDefine.IMV_EPixelType.gvspPixelMono8)
            {
                // 用Mono8数据生成Bitmap
                bitmap = new Bitmap((int)frame.frameInfo.width, (int)frame.frameInfo.height, PixelFormat.Format8bppIndexed);
                ColorPalette colorPalette = bitmap.Palette;
                for (int i = 0; i != 256; ++i)
                {
                    colorPalette.Entries[i] = Color.FromArgb(i, i, i);
                }

                bitmap.Palette = colorPalette;

                bitmapRect.Height = bitmap.Height;
                bitmapRect.Width = bitmap.Width;
                bmpData = bitmap.LockBits(bitmapRect, ImageLockMode.ReadWrite, bitmap.PixelFormat);
                CopyMemory(bmpData.Scan0, pDstRGB, bmpData.Stride * bitmap.Height);
                bitmap.UnlockBits(bmpData);
            }
            else
            {
                // 用BGR24数据生成Bitmap
                bitmap = new Bitmap((int)frame.frameInfo.width, (int)frame.frameInfo.height, PixelFormat.Format24bppRgb);
                bitmapRect.Height = bitmap.Height;
                bitmapRect.Width = bitmap.Width;
                bmpData = bitmap.LockBits(bitmapRect, ImageLockMode.ReadWrite, bitmap.PixelFormat);
                CopyMemory(bmpData.Scan0, pDstRGB, bmpData.Stride * bitmap.Height);
                bitmap.UnlockBits(bmpData);
                if (frame.frameInfo.pixelFormat != IMVDefine.IMV_EPixelType.gvspPixelBGR8)
                {
                    Marshal.FreeHGlobal(pDstRGB);
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    #endregion

    /// <summary>
    /// 指针之间进行数据拷贝
    /// </summary>
    /// <param name="pDst">目标地址</param>
    /// <param name="pSrc">源地址</param>
    /// <param name="len">拷贝数据长度</param>
    [DllImport("Kernel32.dll", EntryPoint = "RtlMoveMemory", CharSet = CharSet.Ansi)]
    private static extern void CopyMemory(IntPtr pDst, IntPtr pSrc, int len);

    #region 相机的IParameters实现类（内部类）

    private class DahuaParameters(IRAYPLECamera dahuaCamera) : IParameters
    {
        public double ExposureTime
        {
            get
            {
                double val = 0;
                dahuaCamera.device.IMV_GetDoubleFeatureValue("ExposureTime", ref val);
                return val;
            }
            set => dahuaCamera.device.IMV_SetDoubleFeatureValue("ExposureTime", value);
        }


        public double MaxExposureTime
        {
            get
            {
                double val = 0;
                dahuaCamera.device.IMV_GetDoubleFeatureMax("ExposureTime", ref val);
                return val;
            }
        }

        public double Gain {
            get
            {
                double val = 0;
                dahuaCamera.device.IMV_GetDoubleFeatureValue("GainRaw", ref val);
                return val;
            }
            set => dahuaCamera.device.IMV_SetDoubleFeatureValue("GainRaw", value);
        }

        public double MaxGain
        {
            get
            {
                double val = 0;
                dahuaCamera.device.IMV_GetDoubleFeatureMax("GainRaw", ref val);
                return val;
            }
        }

        public int Width {
            get
            {
                long val = 0;
                dahuaCamera.device.IMV_GetIntFeatureValue("Width", ref val);
                return (int)val;
            }
            set
            {

            }
        }
        public int Height
        {
            get
            {
                long val = 0;
                dahuaCamera.device.IMV_GetIntFeatureValue("Height", ref val);
                return (int)val;
            }
            set
            {

            }
        }

        public string TriggerSoure
        {
            get
            {
                var val = new IMVDefine.IMV_String();
                dahuaCamera.device.IMV_GetEnumFeatureSymbol("TriggerSource", ref val);

                return val.str;
            }
            set
            {
                dahuaCamera.device.IMV_SetEnumFeatureSymbol("TriggerMode", "On");
                dahuaCamera.device.IMV_SetEnumFeatureSymbol("TriggerSource", value);
            }
        }

        public List<string> TriggerSoures
        {
            get
            {
                IMVDefine.IMV_EnumEntryList enumEntryList = new IMVDefine.IMV_EnumEntryList();
                uint nEntryNum = 0;

                // 获取枚举属性的可设枚举值的个数
                //  Get the number of enumeration property settable enumeration
                var res = dahuaCamera.device.IMV_GetEnumFeatureEntryNum("TriggerSource", ref nEntryNum);
                if (res != IMVDefine.IMV_OK)
                {
                    Console.WriteLine("Get settable enumeration number failed! ErrorCode[{0}]", res);
                    return null;
                }

                enumEntryList.nEnumEntryBufferSize = (uint)Marshal.SizeOf(typeof(IMVDefine.IMV_EnumEntryInfo)) * nEntryNum;
                enumEntryList.pEnumEntryInfo = Marshal.AllocHGlobal((int)enumEntryList.nEnumEntryBufferSize);

                if (enumEntryList.pEnumEntryInfo == IntPtr.Zero)
                {
                    Console.WriteLine("pEnumEntryInfo is NULL");
                    return [];
                }

                // 获取枚举属性的可设枚举值列表
                // Get enumeration property's settable enumeration value list
                res = dahuaCamera.device.IMV_GetEnumFeatureEntrys("TriggerSource", ref enumEntryList);
                if (res != IMVDefine.IMV_OK)
                {
                    Console.WriteLine("Get settable enumeration value failed! ErrorCode[{0}]", res);
                    return [];
                }

                var vals = new List<string>();
                for (int i = 0; i < nEntryNum; i++)
                {
                    var t =
                        (IMVDefine.IMV_EnumEntryInfo)
                        Marshal.PtrToStructure(
                            enumEntryList.pEnumEntryInfo + Marshal.SizeOf(typeof(IMVDefine.IMV_EnumEntryInfo)) * i,
                            typeof(IMVDefine.IMV_EnumEntryInfo));

                    vals.Add(t.name);
                }

                return vals;
            }
        }
    }
    #endregion


    #endregion
}

