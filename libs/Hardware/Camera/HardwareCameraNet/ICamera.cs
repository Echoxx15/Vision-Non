using System;
using Cognex.VisionPro;

namespace HardwareCameraNet;

/// <summary>
/// 相机接口，定义所有相机插件必须实现的方法和属性
/// </summary>
public interface ICamera
{

    /// <summary>
    /// 图片回调事件,康耐视图像
    /// </summary>
    event EventHandler<ICogImage> FrameGrabedEvent;

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
