using System;
using Cognex.VisionPro;

namespace HardwareCameraNet;

/// <summary>
/// 相机接口，定义所有相机插件必须实现的方法和属性
/// </summary>
public interface ICamera
{
    /// <summary>
    /// 图片回调 Action，使用时直接赋值即可，无需订阅/取消订阅
    /// 参数：ICogImage 图像
    /// </summary>
    Action<ICogImage> OnFrameGrabed { get; set; }

    /// <summary>
    /// 采集超时回调 Action
    /// 参数：超时时间（毫秒）
    /// </summary>
    Action<int> OnGrabTimeout { get; set; }

    /// <summary>
    /// 掉线事件,掉线时触发，true 表示掉线，false表示恢复
    /// </summary>
    event EventHandler<bool> DisConnetEvent;

    #region 属性
    /// <summary>
    /// 设备序列号
    /// </summary>
    string SN { get; }
    /// <summary>
    /// 相机类型
    /// </summary>
    CameraType Type { get; }

    /// <summary>
    /// 是否已连接
    /// </summary>
    bool IsConnected { get; }

    IParameters Parameters { get; }
    #endregion

    #region 方法
    /// <summary>
    /// 打开相机
    /// </summary>
    /// <returns>成功返回true，失败返回false</returns>
    int Open();
    /// <summary>
    /// 关闭相机
    /// </summary>
    ///     /// <summary>
    /// 软触发一次
    /// </summary>
    void SoftwareTriggerOnce();
    /// <summary>
    /// 持续抓图
    /// </summary>
    /// <returns></returns>
    void ContinuousGrab();
    /// <summary>
    /// 停止持续抓图
    /// </summary>
    void StopContinuousGrab();
    /// <summary>
    /// 开始抓图
    /// </summary>
    /// <returns></returns>
    int StartGrabbing();
    /// <summary>
    /// 停止抓图
    /// </summary>
    /// <returns></returns>
    int StopGrabbing();
    void Close();
    #endregion
}
