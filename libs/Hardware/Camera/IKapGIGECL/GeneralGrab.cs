using System;
using System.Runtime.InteropServices;

using IKapCDotNet;
using IKNS;
using GeneralConfigureCameraNS;
using System.Text;

namespace GeneralGrabNS
{
    class GeneralGrab
    {
        public static void SetSoftTriggerWithoutGrabber(IKCamera cam)
        {
            uint res = IKapC.ITKSTATUS_OK;
            if (cam.g_bSoftTriggerUsed)
            {
                res = IKapC.ItkDevFromString(cam.g_hCamera, "TriggerSelector", "FrameStart");
                res = IKapC.ItkDevFromString(cam.g_hCamera, "TriggerMode", "On");
                res = IKapC.ItkDevFromString(cam.g_hCamera, "TriggerSource", "Software");
            }
            else
            {
                res = IKapC.ItkDevFromString(cam.g_hCamera, "TriggerSelector", "FrameStart");
                res = IKapC.ItkDevFromString(cam.g_hCamera, "TriggerMode", "Off");
            }
        }
        /// \~chinese 创建数据流和缓冲区				\~english Create data stream and buffer
        public static void CreateStreamAndBuffer(IKCamera cam)
        {
            uint res = IKapC.ITKSTATUS_OK;

            /// \~chinese  数据流数量				     \~english The number of data stream
            uint streamCount = 0;

            /// \~chinese  获取数据流数量			     \~english Get the number of data stream
            res = IKapC.ItkDevGetStreamCount(cam.g_hCamera, ref streamCount);
            IKUtils.CheckIKapC(res);

            if (streamCount == 0)
            {
                Console.Write("Camera does not have image stream channel.");
                IKapC.ItkManTerminate();
                IKUtils.pressEnterToExit();
            }

            /// \~chinese  申请数据流资源		            \~english Allocate data stream source
            res = IKapC.ItkDevAllocStreamEx(cam.g_hCamera, 0, cam.g_bufferCount, ref cam.g_hStream);
            IKUtils.CheckIKapC(res);

            ITKBUFFER hBuffer = new ITKBUFFER();

            res = IKapC.ItkStreamGetBuffer(cam.g_hStream, 0, ref hBuffer);
            IKUtils.CheckIKapC(res);
            ITK_BUFFER_INFO bufferInfo = new ITK_BUFFER_INFO();
            res = IKapC.ItkBufferGetInfo(hBuffer, bufferInfo);
            IKUtils.CheckIKapC(res);

            /// \~chinese  创建缓冲区数据存储		       \~english Create buffer data saving
            cam.g_bufferData = Marshal.AllocHGlobal((int)bufferInfo.TotalSize);
            if (cam.g_bufferData == IntPtr.Zero)
            {
                IKapC.ItkManTerminate();
                IKUtils.pressEnterToExit();
            }

            /***************************/
            ///// \~chinese  展示如果使用用户申请的内存地址作为Buffer的内存地址，注意不要忘记释放该内存	       \~english Show how to using the memory address as the memory address of Buffer,be careful not to forget to release the memory
            ///// \~chinese  创建用于设置Buffer地址的内存		       \~english Create the memory that the user requests for setting the Buffer address
            //cam.g_user_buffer = Marshal.AllocHGlobal((int)bufferInfo.TotalSize);
            //if (cam.g_user_buffer == IntPtr.Zero)
            //{
            //    IKUtils.pressEnterToExit();
            //}

            //// \~chinese 将序号为0的Buffer的内存地址改为用户申请的大小合适的内存地址，序号为1~g_bufferCount-1的Buffer同理。		\~english The memory address of Buffer with index number 0 is changed to the appropriate size memory address applied by the user, just as with Buffer with index number 1~g_bufferCount-1.
            //res = IKapC.ItkBufferSetAddress(hBuffer, cam.g_user_buffer, bufferInfo.TotalSize);
            //IKUtils.CheckIKapC(res); 
            /***************************/
        }

        /// \~chinese  注册回调函数		            \~english Register callback functions
        public static void RegisterCallbackWithoutGrabber(IKCamera cam)
        {
            uint res = IKapC.ITKSTATUS_OK;

            cam.IKCameraGCHandle = GCHandle.Alloc(cam);
            IntPtr IKCameraIntPtr = GCHandle.ToIntPtr(cam.IKCameraGCHandle);

            //注册采集开始回调
            cam.OnGrabStartDelegate = new IKCamera.IKapCCallBackDelegate(cbStartOfStream);
            res = IKapC.ItkStreamRegisterCallback(cam.g_hStream, IKapC.ITKSTREAM_VAL_EVENT_TYPE_START_OF_STREAM, Marshal.GetFunctionPointerForDelegate(cam.OnGrabStartDelegate), IKCameraIntPtr);
            IKUtils.CheckIKapC(res);

            //注册采集超时回调
            cam.OnTimeoutDelegate = new IKCamera.IKapCCallBackDelegate(cbOnTimeOut);
            res = IKapC.ItkStreamRegisterCallback(cam.g_hStream, IKapC.ITKSTREAM_VAL_EVENT_TYPE_TIME_OUT, Marshal.GetFunctionPointerForDelegate(cam.OnTimeoutDelegate), IKCameraIntPtr);
            IKUtils.CheckIKapC(res);

            //注册采集丢帧回调
            cam.OnFrameLostDelegate = new IKCamera.IKapCCallBackDelegate(cbOnFrameLost);
            res = IKapC.ItkStreamRegisterCallback(cam.g_hStream, IKapC.ITKSTREAM_VAL_EVENT_TYPE_FRAME_LOST, Marshal.GetFunctionPointerForDelegate(cam.OnFrameLostDelegate), IKCameraIntPtr);
            IKUtils.CheckIKapC(res);

            //注册采集结束回调
            cam.OnGrabStopDelegate = new IKCamera.IKapCCallBackDelegate(cbEndOfStream);
            res = IKapC.ItkStreamRegisterCallback(cam.g_hStream, IKapC.ITKSTREAM_VAL_EVENT_TYPE_END_OF_STREAM, Marshal.GetFunctionPointerForDelegate(cam.OnGrabStopDelegate), IKCameraIntPtr);
            IKUtils.CheckIKapC(res);

            //注册帧结束回调
            cam.OnFrameReadyDelegate = new IKCamera.IKapCCallBackDelegate(cbOnEndOfFrame);
            res = IKapC.ItkStreamRegisterCallback(cam.g_hStream, IKapC.ITKSTREAM_VAL_EVENT_TYPE_END_OF_FRAME, Marshal.GetFunctionPointerForDelegate(cam.OnFrameReadyDelegate), IKCameraIntPtr);
            IKUtils.CheckIKapC(res);
        }

        /// \~chinese  清除回调函数		            \~english Unregister callback functions
        public static void UnRegisterCallbackWithoutGrabber(IKCamera cam)
        {
            uint res = IKapC.ITKSTATUS_OK;

            //注销回调函数
            IKapC.ItkStreamUnregisterCallback(cam.g_hStream, IKapC.ITKSTREAM_VAL_EVENT_TYPE_START_OF_STREAM);
            IKapC.ItkStreamUnregisterCallback(cam.g_hStream, IKapC.ITKSTREAM_VAL_EVENT_TYPE_END_OF_STREAM);
            IKapC.ItkStreamUnregisterCallback(cam.g_hStream, IKapC.ITKSTREAM_VAL_EVENT_TYPE_FRAME_LOST);
            IKapC.ItkStreamUnregisterCallback(cam.g_hStream, IKapC.ITKSTREAM_VAL_EVENT_TYPE_TIME_OUT);
            IKapC.ItkStreamUnregisterCallback(cam.g_hStream, IKapC.ITKSTREAM_VAL_EVENT_TYPE_END_OF_FRAME);

            cam.IKCameraGCHandle.Free();
        }

        public static void cbStartOfStream(uint eventType, IntPtr pContext)
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
        public static void cbEndOfStream(uint eventType, IntPtr pContext)
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

        public static void cbOnEndOfFrame(uint eventType, IntPtr pContext)
        {
            Console.Write("On end of frame. \n");

            uint res = IKapC.ITKSTATUS_OK;

            if (pContext != IntPtr.Zero)
            {
                GCHandle camGCHanle = GCHandle.FromIntPtr(pContext);
                IKCamera cam = (IKCamera)camGCHanle.Target; //前提：注册回调传入的是IKCamera object
                if (cam == null)
                {
                    return;
                }
                Console.Write("On end of frame of camera with serialNumber:{0}. \n", cam.g_devInfo.SerialNumber);

                /// \~chinese 获取缓冲区对象  \~english  get the ITKBUFFER object
                ITKBUFFER hBuffer = new ITKBUFFER();
                res = IKapC.ItkStreamGetCurrentBuffer(cam.g_hStream, ref hBuffer);
                IKUtils.CheckIKapC(res);

                /// \~chinese 获取缓冲区信息  \~english  get the info of buffer
                ITK_BUFFER_INFO bufferInfo = new ITK_BUFFER_INFO();
                res = IKapC.ItkBufferGetInfo(hBuffer, bufferInfo);

                uint bufferStatus = bufferInfo.State;
                /// \~chinese 当图像缓冲区满或者图像缓冲区非满但是无法采集完整的一帧图像时 /// \~english When buffer is full or buffer is not full but cannot grab a complete frame of image
                if (bufferStatus == IKapC.ITKBUFFER_VAL_STATE_FULL || bufferStatus == IKapC.ITKBUFFER_VAL_STATE_UNCOMPLETED)
                {

                    /// \~chinese 获取chunk数据，前提是ChunkModeActive已开启，ChunkSelector中以下对应参数已使能      \~english Get chunk data provided that ChunkModeActive is on and the following corresponding parameters in ChunkSelector are enabled
                    if (cam.bEnableChunkData)
                    {
                        long nChunkWidth = 0;
                        res = IKapC.ItkBufferGetChunkInt64(hBuffer, "ChunkWidth", ref nChunkWidth);
                        IKUtils.CheckIKapC(res);

                        long nChunkHeight = 0;
                        res = IKapC.ItkBufferGetChunkInt64(hBuffer, "ChunkHeight", ref nChunkHeight);
                        IKUtils.CheckIKapC(res);

                        long nChunkOffsetX = 0;
                        res = IKapC.ItkBufferGetChunkInt64(hBuffer, "ChunkOffsetX", ref nChunkOffsetX);
                        IKUtils.CheckIKapC(res);

                        long nChunkOffsetY = 0;
                        res = IKapC.ItkBufferGetChunkInt64(hBuffer, "ChunkOffsetY", ref nChunkOffsetY);
                        IKUtils.CheckIKapC(res);

                        uint strLen = 128;
                        StringBuilder strChunkPixelFormat = new StringBuilder((int)strLen);
                        res = IKapC.ItkBufferToChunkString(hBuffer, "ChunkPixelFormat", strChunkPixelFormat, ref strLen);
                        IKUtils.CheckIKapC(res);

                        long nChunkTimestamp = 0;
                        res = IKapC.ItkBufferGetChunkInt64(hBuffer, "ChunkTimestamp", ref nChunkTimestamp);
                        IKUtils.CheckIKapC(res);

                        double dchunkGain = 0;
                        res = IKapC.ItkBufferGetChunkDouble(hBuffer, "ChunkGain", ref dchunkGain);
                        IKUtils.CheckIKapC(res);

                        long nChunkExposureTime = 0;
                        res = IKapC.ItkBufferGetChunkInt64(hBuffer, "ChunkExposureTime", ref nChunkExposureTime);
                        IKUtils.CheckIKapC(res);

                        long nChunkBrightnessInfo = 0;
                        res = IKapC.ItkBufferGetChunkInt64(hBuffer, "ChunkBrightnessInfo", ref nChunkBrightnessInfo);
                        IKUtils.CheckIKapC(res);

                        long nChunkFrameCounter = 0;
                        res = IKapC.ItkBufferGetChunkInt64(hBuffer, "ChunkFrameCounter", ref nChunkFrameCounter);
                        IKUtils.CheckIKapC(res);

                        long nChunkExtTriggerCount = 0;
                        res = IKapC.ItkBufferGetChunkInt64(hBuffer, "ChunkExtTriggerCount", ref nChunkExtTriggerCount);
                        IKUtils.CheckIKapC(res);

                        /// \~chinese  只有彩色相机才有这些参数		            \~english                       Only the color cameras have these parameters
                        /*{
                            double dchunkRedGain = 0;
                            res = IKapC.ItkBufferGetChunkDouble(hBuffer, "ChunkRedGain", ref dchunkRedGain);
                            IKUtils.CheckIKapC(res);

                            double dChunkG1Gain = 0;
                            res = IKapC.ItkBufferGetChunkDouble(hBuffer, "ChunkG1Gain", ref dChunkG1Gain);
                            IKUtils.CheckIKapC(res);

                            double dChunkG2Gain = 0;
                            res = IKapC.ItkBufferGetChunkDouble(hBuffer, "ChunkG2Gain", ref dChunkG2Gain);
                            IKUtils.CheckIKapC(res);

                            double dChunkBlueGain = 0;
                            res = IKapC.ItkBufferGetChunkDouble(hBuffer, "ChunkBlueGain", ref dChunkBlueGain);
                            IKUtils.CheckIKapC(res);
                        }*/
                    }

                    /// \~chinese 判断像素格式是否可转换                \~english Judge if need convert
                    byte bNeedCvt = 0;
                    res = IKapC.ItkBufferNeedAutoConvert(hBuffer, ref bNeedCvt);
                    IKUtils.CheckIKapC(res);
                    if (bNeedCvt != 0)
                    {
                        ITKBUFFER hConvert = new ITKBUFFER();
                        res = IKapC.ItkBufferNew(1024, 1024, IKapC.ITKBUFFER_VAL_FORMAT_MONO8, ref hConvert);
                        IKUtils.CheckIKapC(res);
                        res = IKapC.ItkBufferConvert(hBuffer, hConvert, 0, IKapC.ITKBUFFER_VAL_CONVERT_OPTION_AUTO_FORMAT);
                        IKUtils.CheckIKapC(res);

                        IKapC.ItkBufferFree(hConvert);
                    }

                    /// \~chinese 读取缓冲区数据				\~english Read buffer data
                    /// \~chinese 如果MultiExposureTimeCount特征值大于1则开启了多重曝光功能，采集到的图像均分为N种曝光时间，[0~1*Height/N-1]行对应ExposureSelect为1时的ExposureTime，[1*Height/N~2*Height/N-1]行对应ExposureSelect为2时的ExposureTime，...，[(N-1)*Height/N~Height-1]行对应ExposureSelect为N时的ExposureTime.	\~english	If the MultiExposureTimeCount feature value is greater than 1, the multiple exposure function is turned on, The collected images were all divided into N exposure times, line [0~1 * Height / N-1] corresponds to the ExposureTime at a ExposureSelect of 1, line [1 * Height / N~2 * Height / N-1] corresponds to ExposureTime at ExposureSelect 2,..., line [(N-1) * Height / N~Height-1] corresponds to ExposureTime at ExposureSelect N.
                    ulong nImageSize = bufferInfo.ValidImageSize;
                    res = IKapC.ItkBufferRead(hBuffer, 0, cam.g_bufferData, (uint)nImageSize);
                    IKUtils.CheckIKapC(res);

                    /// \~chinese 保存图像				    \~english Save image
                    //res = IKapC.ItkBufferSave(hBuffer, cam.g_saveFileName, IKapC.ITKBUFFER_VAL_TIFF);
                    //IKUtils.CheckIKapC(res);
                }
            }
        }

        public static void cbOnTimeOut(uint eventType, IntPtr pContext)
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

        public static void cbOnFrameLost(uint eventType, IntPtr pContext)
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
    }
}
