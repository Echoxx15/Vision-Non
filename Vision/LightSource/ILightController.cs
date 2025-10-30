using System;

namespace Vision.LightSource;

/// <summary>
/// 光源控制器接口
/// </summary>
public interface ILightController : IDisposable
{
    string Name { get; }
 LightControllerType Type { get; }
    LightControllerMode Mode { get; }
    bool IsConnected { get; }
    int ChannelCount { get; }
    
   bool Open();
    void Close();
    bool TurnOn(int channel);
    bool TurnOff(int channel);
bool SetBrightness(int channel, int brightness);
    int GetBrightness(int channel);
    bool SetMultiChannelBrightness(int[] channels, int brightness);
}
