using System;
using System.IO.Ports;
using System.Text;
using System.Threading;
using System.Windows.Forms;

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
    public Form TestForm { get { return CreateTestForm(); } }

    public FgenLightController(LightConfig config)
    {
      _config = config ?? throw new ArgumentNullException(nameof(config));
    }

    #region ���ڲ���

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

    #region ��Դָ��

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

      // �ɹ�ʱ�����ظ�������ͬ��ʽ��# + ���� + ͨ�� + ����(0XX) + У��(2 ASCII)
         // ʧ�ܷ��� &
    if (resp.Contains("&")) return -1;

         // �ҵ��ڸ���ʼ��
   int start = resp.IndexOf(START_CHAR);
      if (start < 0) return -1;
                var frame = resp.Substring(start);
      // ��̳��ȣ�# + c + ch + data(3) + checksum(2) = 7
       if (frame.Length < 7) return -1;

     // ȡ������λ��0XX��
                string dataStr = frame.Substring(3, 3);
          if (dataStr.Length != 3 || dataStr[0] != '0') return -1;
         // ����Ϊʮ�����Ƶ���λ
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

    #region Э���봮��ʵ��

    // ֡��ʽЭ��֡��# + ����(1-4) + ͨ��(1-4) + ����(0XX) + У��(2 ASCII)
    private string BuildCommand(int command, int channel, int data)
    {
  // ����Ϊ0XX����λʮ�����ƣ���λ���㣩
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

  // У�飺�Գ�У��λ���������ֽڣ���ΪASCII�ֽڣ���򣬽��ȡ8λ��
    // Ȼ�󽫸߰��ֽں͵Ͱ��ֽ�ת��ΪASCII��0-9,A-F���������ַ�ǰ�ߺ��
    private string CalculateChecksumAscii(string frameWithoutChecksum)
    {
  int xor = 0;
        // �ӿ�ʼ����ʼ������ĩβ������У�鱾����
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
        // �ɹ����� '#', ʧ�ܷ��� '&'�����ܰ��������ַ���
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

    #region ����У��

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
 "odd" or "��" => Parity.Odd,
            "even" or "ż" => Parity.Even,
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

    private Form CreateTestForm()
    {
        var f = new Form();
        f.Text = Name + " 测试";
        f.StartPosition = FormStartPosition.CenterParent;
        var btnOn = new Button { Text = "通道1开", Left = 20, Top = 20, Width = 100 };
        var btnOff = new Button { Text = "通道1关", Left = 140, Top = 20, Width = 100 };
        var track = new TrackBar { Left = 20, Top = 60, Width = 220, Minimum = 0, Maximum = 255, TickFrequency = 5 };
        var lbl = new Label { Left = 260, Top = 60, Width = 100, Text = "亮度:0" };
        btnOn.Click += (s, e) => TurnOn(1);
        btnOff.Click += (s, e) => TurnOff(1);
        track.Scroll += (s, e) => { SetBrightness(1, track.Value); lbl.Text = "亮度:" + track.Value; };
        f.Controls.Add(btnOn);
        f.Controls.Add(btnOff);
        f.Controls.Add(track);
        f.Controls.Add(lbl);
        return f;
    }
}
