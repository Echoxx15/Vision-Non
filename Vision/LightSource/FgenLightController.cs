using System;
using System.IO.Ports;
using System.Text;
using System.Threading;
using Logger;

namespace Vision.LightSource;

/// <summary>
/// 孚根光源控制器实现（数字控制器）
/// 协议：特征字(#) + 命令字(1-4) + 通道字(1-4) + 数据(0-255) + 校验和
/// </summary>
public class FgenLightController : ILightController
{
    private SerialPort _serialPort;
    private readonly LightConfig _config;
    private readonly object _lock = new object();
    
    // 协议常量
    private const char START_CHAR = '#';  // 特征字
    
    public string Name => _config.Name;
    public LightControllerType Type => LightControllerType.Fgen;
    public LightControllerMode Mode => _config.Mode;
    public bool IsConnected => _serialPort?.IsOpen ?? false;
    public int ChannelCount => _config.ChannelCount;
    
  public FgenLightController(LightConfig config)
    {
        _config = config ?? throw new ArgumentNullException(nameof(config));
    }
    
  #region 串口操作
    
    /// <summary>
    /// 打开串口连接
    /// </summary>
    public bool Open()
    {
        try
  {
            if (IsConnected)
          {
                LogHelper.Warn($"光源控制器[{Name}]已经打开");
                return true;
 }
            
         _serialPort = new SerialPort
     {
             PortName = _config.PortName,
    BaudRate = _config.BaudRate,
       DataBits = _config.DataBits,
   StopBits = ParseStopBits(_config.StopBits),
          Parity = ParseParity(_config.Parity),
            ReadTimeout = 1000,
      WriteTimeout = 1000
     };
            
            _serialPort.Open();
    
          LogHelper.Info($"光源控制器[{Name}]打开成功: {_config.PortName}");
        return true;
        }
        catch (Exception ex)
        {
    LogHelper.Error(ex, $"光源控制器[{Name}]打开失败");
 return false;
        }
    }
    
    /// <summary>
    /// 关闭串口连接
  /// </summary>
    public void Close()
    {
        try
        {
            if (_serialPort != null && _serialPort.IsOpen)
            {
                _serialPort.Close();
    _serialPort.Dispose();
    _serialPort = null;
      
       LogHelper.Info($"光源控制器[{Name}]已关闭");
    }
 }
        catch (Exception ex)
        {
            LogHelper.Error(ex, $"光源控制器[{Name}]关闭异常");
        }
 }
    
    #endregion
    
    #region 光源控制
    
    /// <summary>
    /// 打开指定通道光源
    /// 命令字 = 1
    /// </summary>
    public bool TurnOn(int channel)
    {
        if (!ValidateChannel(channel)) return false;
        
      try
        {
  lock (_lock)
      {
        var command = BuildCommand(1, channel, 0);
         return SendCommand(command);
            }
        }
        catch (Exception ex)
        {
      LogHelper.Error(ex, $"光源控制器[{Name}]打开通道{channel}失败");
    return false;
        }
    }
    
    /// <summary>
    /// 关闭指定通道光源
    /// 命令字 = 2
    /// </summary>
    public bool TurnOff(int channel)
    {
        if (!ValidateChannel(channel)) return false;
        
  try
        {
     lock (_lock)
       {
             var command = BuildCommand(2, channel, 0);
         return SendCommand(command);
        }
        }
        catch (Exception ex)
        {
 LogHelper.Error(ex, $"光源控制器[{Name}]关闭通道{channel}失败");
            return false;
  }
    }
    
    /// <summary>
    /// 设置指定通道亮度
    /// 命令字 = 3
    /// </summary>
    public bool SetBrightness(int channel, int brightness)
 {
if (!ValidateChannel(channel)) return false;
        if (!ValidateBrightness(brightness)) return false;
        
        try
        {
   lock (_lock)
        {
       var command = BuildCommand(3, channel, brightness);
                return SendCommand(command);
            }
      }
        catch (Exception ex)
        {
          LogHelper.Error(ex, $"光源控制器[{Name}]设置通道{channel}亮度失败");
    return false;
   }
    }

    /// <summary>
    /// 读取指定通道亮度
    /// 命令字 = 4
    /// </summary>
    public int GetBrightness(int channel)
    {
        if (!ValidateChannel(channel)) return -1;
    
        try
        {
            lock (_lock)
            {
       var command = BuildCommand(4, channel, 0);
 if (!SendCommand(command)) return -1;
         
     // 等待响应
          Thread.Sleep(50);
         
    // 读取响应（格式：#通道数据校验）
   var response = ReadResponse();
     if (string.IsNullOrEmpty(response)) return -1;

     return ParseBrightnessResponse(response);
   }
        }
   catch (Exception ex)
        {
            LogHelper.Error(ex, $"光源控制器[{Name}]读取通道{channel}亮度失败");
            return -1;
        }
  }
    
  /// <summary>
    /// 设置多个通道亮度（同时控制）
    /// </summary>
    public bool SetMultiChannelBrightness(int[] channels, int brightness)
    {
 if (channels == null || channels.Length == 0) return false;
        if (!ValidateBrightness(brightness)) return false;
   
        bool allSuccess = true;
        foreach (var channel in channels)
        {
            if (!SetBrightness(channel, brightness))
 {
           allSuccess = false;
      }
      }
        
        return allSuccess;
    }
    
    #endregion
    
    #region 协议实现
    
    /// <summary>
    /// 构建命令字符串
    /// 格式：特征字(#) + 命令字(1-4) + 通道字(1-4) + 数据(0-255) + 校验和
    /// 
    /// 示例：设置通道1亮度为100
    /// #3106413 (# + 3 + 1 + 064 + 校验和)
 /// </summary>
    private string BuildCommand(int command, int channel, int data)
    {
   var sb = new StringBuilder();
        
        // 1. 特征字
        sb.Append(START_CHAR);
        
        // 2. 命令字（1字节）
        sb.Append(command);
        
        // 3. 通道字（1字节）
        sb.Append(channel);
        
     // 4. 数据（3字节，不足补0）
        sb.Append(data.ToString("D3"));
        
        // 5. 校验和（2字节）
     var checksum = CalculateChecksum(sb.ToString());
        sb.Append(checksum.ToString("D2"));
  
        return sb.ToString();
    }
    
    /// <summary>
    /// 计算校验和
    /// 异或校验：特征字、命令字、通道字和数据的异或和，然后取低半字节和高半字节的ASCII码
    /// </summary>
    private int CalculateChecksum(string data)
    {
        int xorSum = 0;
      
        // 跳过特征字，计算命令字、通道字、数据的异或
 for (int i = 1; i < data.Length; i++)
        {
       xorSum ^= data[i];
      }
   
        // 取结果（根据协议，这里简化处理，实际可能需要更复杂的计算）
        return xorSum % 100;
    }
    
    /// <summary>
    /// 发送命令
    /// </summary>
    private bool SendCommand(string command)
    {
        if (!IsConnected)
        {
      LogHelper.Warn($"光源控制器[{Name}]未连接");
        return false;
        }
        
        try
        {
    _serialPort.Write(command);
 LogHelper.Info($"光源控制器[{Name}]发送命令: {command}");
        return true;
        }
   catch (Exception ex)
  {
            LogHelper.Error(ex, $"光源控制器[{Name}]发送命令失败: {command}");
  return false;
        }
    }
    
    /// <summary>
    /// 读取响应
    /// </summary>
    private string ReadResponse()
    {
 if (!IsConnected) return string.Empty;
    
        try
{
        if (_serialPort.BytesToRead > 0)
        {
    var response = _serialPort.ReadExisting();
    LogHelper.Info($"光源控制器[{Name}]接收响应: {response}");
         return response;
       }
  }
 catch (Exception ex)
   {
     LogHelper.Error(ex, $"光源控制器[{Name}]读取响应失败");
    }
   
        return string.Empty;
    }

    /// <summary>
 /// 解析亮度响应
    /// 格式：#通道数据校验
    /// </summary>
    private int ParseBrightnessResponse(string response)
    {
        try
        {
 if (response.Length < 6 || response[0] != START_CHAR)
{
      return -1;
   }
       
      // 提取数据部分（3字节）
            var dataStr = response.Substring(2, 3);
            if (int.TryParse(dataStr, out int brightness))
{
                return brightness;
   }
        }
   catch (Exception ex)
        {
            LogHelper.Error(ex, $"光源控制器[{Name}]解析亮度响应失败: {response}");
        }
      
    return -1;
    }
    
    #endregion
    
    #region 辅助方法
 
    /// <summary>
    /// 验证通道号
    /// </summary>
  private bool ValidateChannel(int channel)
    {
        if (channel < 1 || channel > ChannelCount)
    {
            LogHelper.Warn($"光源控制器[{Name}]通道号无效: {channel}，有效范围: 1-{ChannelCount}");
  return false;
  }
        return true;
    }
    
    /// <summary>
    /// 验证亮度值
    /// </summary>
    private bool ValidateBrightness(int brightness)
    {
     if (brightness < 0 || brightness > 255)
        {
            LogHelper.Warn($"光源控制器[{Name}]亮度值无效: {brightness}，有效范围: 0-255");
 return false;
        }
return true;
    }
  
    /// <summary>
    /// 解析停止位
    /// </summary>
    private StopBits ParseStopBits(double value)
 {
   return value switch
        {
            1 => StopBits.One,
            1.5 => StopBits.OnePointFive,
            2 => StopBits.Two,
 _ => StopBits.One
        };
    }
    
    /// <summary>
    /// 解析校验位
    /// </summary>
    private Parity ParseParity(string value)
  {
        return value?.ToLower() switch
      {
            "odd" or "奇" => Parity.Odd,
  "even" or "偶" => Parity.Even,
       "mark" => Parity.Mark,
            "space" => Parity.Space,
  _ => Parity.None
   };
    }
    
    #endregion
    
    #region IDisposable
    
    public void Dispose()
    {
        Close();
    }
    
    #endregion
}
