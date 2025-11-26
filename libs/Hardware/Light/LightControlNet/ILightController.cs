using System;
using System.Windows.Forms;

namespace LightControlNet
{
    /// <summary>
    /// ��Դ�������ӿ�
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

        /// <summary>
        /// ����ԭʼ�����ַ������������κ�ת����
        /// ���ؿ�������ԭʼ��Ӧ�ַ��������Ϊ�գ���
        /// </summary>
        string SendRawCommand(string command);
    }
}
