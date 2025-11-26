using System;
using System.IO.Ports;
using System.Text;
using System.Threading;

namespace LightControlNet;

public class FgenLightController : ILightController
{
    private SerialPort _serialPort;
    private readonly LightConfig _config;
    private readonly object _lock = new object();

    private const char START_CHAR = '#';

    public string Name => _config.Name;
    public LightControllerType Type => LightControllerType.Fgen;
    public bool IsConnected => _serialPort?.IsOpen ?? false;
    public int ChannelCount => _config.ChannelCount;

    public FgenLightController(LightConfig config)
    {
      _config = config ?? throw new ArgumentNullException(nameof(config));
    }

    #region 串口操作

    public bool Open()
    {
        try
        {
if (IsConnected)
    {
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

    return true;
        }
        catch (Exception)
        {
       return false;
  }
    }

    public void Close()
    {
        try
        {
    if (_serialPort != null && _serialPort.IsOpen)
            {
     _serialPort.Close();
        _serialPort.Dispose();
        _serialPort = null;
       }
    }
        catch (Exception)
        {
        }
    }

    #endregion

    #region 光源指令

    public bool TurnOn(int channel)
    {
        if (!ValidateChannel(channel)) return false;

        try
        {
    lock (_lock)
   {
       var command = BuildCommand(1, channel, 0);
       return SendAndExpectAck(command);
      }
        }
 catch (Exception)
 {
      return false;
        }
    }

    public bool TurnOff(int channel)
    {
        if (!ValidateChannel(channel)) return false;

        try
      {
   lock (_lock)
        {
        var command = BuildCommand(2, channel, 0);
 return SendAndExpectAck(command);
            }
 }
        catch (Exception)
        {
 return false;
        }
    }

    public bool SetBrightness(int channel, int brightness)
    {
        if (!ValidateChannel(channel)) return false;
        if (!ValidateBrightness(brightness)) return false;

    try
        {
       lock (_lock)
{
       var command = BuildCommand(3, channel, brightness);
                return SendAndExpectAck(command);
     }
        }
   catch (Exception)
   {
            return false;
    }
    }

    public int GetBrightness(int channel)
    {
 if (!ValidateChannel(channel)) return -1;

        try
        {
  lock (_lock)
            {
      var command = BuildCommand(4, channel, 0);
                var resp = SendAndRead(command, 100);
       if (string.IsNullOrEmpty(resp)) return -1;

      // 成功时，返回跟发送相同格式：# + 命令 + 通道 + 数据(0XX) + 校验(2 ASCII)
         // 失败返回 &
    if (resp.Contains("&")) return -1;

         // 找到第个开始符
   int start = resp.IndexOf(START_CHAR);
      if (start < 0) return -1;
                var frame = resp.Substring(start);
      // 最短长度：# + c + ch + data(3) + checksum(2) = 7
       if (frame.Length < 7) return -1;

     // 取数据域位置0XX）
                string dataStr = frame.Substring(3, 3);
          if (dataStr.Length != 3 || dataStr[0] != '0') return -1;
         // 数据为十六进制的两位
          string hex = dataStr.Substring(1, 2);
           if (int.TryParse(hex, System.Globalization.NumberStyles.HexNumber, null, out int val))
 return val;
  }
        }
   catch (Exception)
        {
  }

        return -1;
    }

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

    #region 协议与串口实现

    // 帧格式协议帧：# + 命令(1-4) + 通道(1-4) + 数据(0XX) + 校验(2 ASCII)
    private string BuildCommand(int command, int channel, int data)
    {
  // 数据为0XX（两位十六进制，高位补零）
   string dataField = $"0{data:X2}";
        var sb = new StringBuilder();
        sb.Append(START_CHAR);
        sb.Append(command);
  sb.Append(channel);
        sb.Append(dataField);

        string checksum = CalculateChecksumAscii(sb.ToString());
        sb.Append(checksum);

        return sb.ToString();
    }

  // 校验：对除校验位以外所有字节（均为ASCII字节）异或，结果取8位）
    // 然后将高半字节和低半字节转换为ASCII（0-9,A-F），两个字符前高后低
    private string CalculateChecksumAscii(string frameWithoutChecksum)
    {
  int xor = 0;
        // 从开始符开始遍历到末尾（不含校验本身）
        foreach (char ch in frameWithoutChecksum)
        {
 xor ^= (byte)ch;
     }

        int hi = (xor >> 4) & 0x0F;
        int lo = xor & 0x0F;
 char hiAsc = NibbleToAscii(hi);
  char loAsc = NibbleToAscii(lo);
        return new string(new[] { hiAsc, loAsc });
    }

    private char NibbleToAscii(int n)
    {
 return (char)(n < 10 ? ('0' + n) : ('A' + (n - 10)));
    }

    private bool SendAndExpectAck(string command)
    {
    var resp = SendAndRead(command, 80);
        if (string.IsNullOrEmpty(resp)) return false;
        // 成功返回 '#', 失败返回 '&'（可能包含其它字符）
      return resp.Contains("#") && !resp.Contains("&");
    }

    private string SendAndRead(string command, int waitMs)
    {
        if (!IsConnected)
        {
            return string.Empty;
        }

      try
    {
            _serialPort.DiscardInBuffer();
            _serialPort.Write(command);

            Thread.Sleep(waitMs);

 if (_serialPort.BytesToRead > 0)
            {
       var resp = _serialPort.ReadExisting();
 return resp;
  }
}
        catch (Exception)
        {
        }

      return string.Empty;
    }

    public string SendRawCommand(string command)
    {
        if (!IsConnected) return string.Empty;
        try
        {
      _serialPort.DiscardInBuffer();
            _serialPort.Write(command);
          Thread.Sleep(50);
   return _serialPort.ReadExisting();
        }
        catch (Exception)
        {
   return string.Empty;
        }
    }

    #endregion

    #region 参数校验

    private bool ValidateChannel(int channel)
    {
   if (channel < 1 || channel > ChannelCount)
        {
       return false;
  }

        return true;
    }

    private bool ValidateBrightness(int brightness)
    {
        if (brightness < 0 || brightness > 255)
        {
            return false;
     }

        return true;
 }

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

    public void Dispose()
    {
        Close();
    }
}
