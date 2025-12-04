using System;
using System.Windows.Forms;

namespace LightControlNet
{
    /// <summary>
    /// 光源控制器接口
    /// </summary>
    public interface ILightController : IDisposable
    {
        /// <summary>
        /// 设备名称
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 控制器类型（品牌）
        /// </summary>
        LightControllerType Type { get; }

        /// <summary>
        /// 是否已连接
        /// </summary>
        bool IsConnected { get; }

        /// <summary>
        /// 通道数量
        /// </summary>
        int ChannelCount { get; }

        /// <summary>
        /// 测试窗体
        /// </summary>
        Form TestForm { get; }

        /// <summary>
        /// 连接状态变化事件
        /// </summary>
        event EventHandler<bool> ConnectionStatusChanged;

        /// <summary>
        /// 打开连接
        /// </summary>
        bool Open();

        /// <summary>
        /// 关闭连接
        /// </summary>
        void Close();

        /// <summary>
        /// 打开指定通道
        /// </summary>
        bool TurnOn(int channel);

        /// <summary>
        /// 关闭指定通道
        /// </summary>
        bool TurnOff(int channel);

        /// <summary>
        /// 设置指定通道亮度
        /// </summary>
        bool SetBrightness(int channel, int brightness);

        /// <summary>
        /// 获取指定通道亮度
        /// </summary>
        int GetBrightness(int channel);

        /// <summary>
        /// 设置多通道亮度
        /// </summary>
        bool SetMultiChannelBrightness(int[] channels, int brightness);

        /// <summary>
        /// 关闭所有通道
        /// </summary>
        void TurnOffAllChannels();

        /// <summary>
        /// 发送原始命令字符串，不做任何转换
        /// 返回控制器原始响应字符串；为空表示失败
        /// </summary>
        string SendRawCommand(string command);

        /// <summary>
        /// 获取配置控件
        /// </summary>
        UserControl GetConfigControl();

        /// <summary>
        /// 设置设备名称（用于重命名）
        /// </summary>
        void SetName(string name);
    }
}
