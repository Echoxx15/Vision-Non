using System;
using System.Windows.Forms;

namespace LightControlNet
{
    /// <summary>
    /// 光源控制器接口
    /// </summary>
    public interface ILightController : IDisposable
    {
        string Name { get; }
        LightControllerType Type { get; }
        bool IsConnected { get; }
        int ChannelCount { get; }
        Form TestForm { get; }

        bool Open();
        void Close();
        bool TurnOn(int channel);
        bool TurnOff(int channel);
        bool SetBrightness(int channel, int brightness);
        int GetBrightness(int channel);
        bool SetMultiChannelBrightness(int[] channels, int brightness);
        void TurnOffAllChannels();

        /// <summary>
        /// 发送原始命令字符串，不做任何转换
        /// 返回控制器原始响应字符串；为空表示失败
        /// </summary>
        string SendRawCommand(string command);
    }
}
