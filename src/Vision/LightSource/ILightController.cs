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

  /// <summary>
  ///发送原始命令字符串，不做任何转换。
  /// 返回控制器的原始响应字符串（可能为空）。
  /// </summary>
  string SendRawCommand(string command);
}
