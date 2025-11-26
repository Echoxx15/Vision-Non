using System;
using System.IO.Ports;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using LightControlNet;

namespace Fgen.LightPlugin
{
    [LightManufacturer(LightControllerType.Fgen, "孚根FG")]
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

        public static System.Collections.Generic.List<string> EnumerateDevices()
        {
            return new System.Collections.Generic.List<string>(SerialPort.GetPortNames());
        }

        public bool Open()
        {
            try
            {
                if (IsConnected) return true;
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
            catch { return false; }
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
            catch { }
        }

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
            catch { return false; }
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
            catch { return false; }
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
            catch { return false; }
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
                    if (resp.Contains("&")) return -1;
                    int start = resp.IndexOf(START_CHAR);
                    if (start < 0) return -1;
                    var frame = resp.Substring(start);
                    if (frame.Length < 7) return -1;
                    string dataStr = frame.Substring(3, 3);
                    if (dataStr.Length != 3 || dataStr[0] != '0') return -1;
                    string hex = dataStr.Substring(1, 2);
                    int val;
                    if (int.TryParse(hex, System.Globalization.NumberStyles.HexNumber, null, out val)) return val;
                }
            }
            catch { }
            return -1;
        }

        public bool SetMultiChannelBrightness(int[] channels, int brightness)
        {
            if (channels == null || channels.Length == 0) return false;
            if (!ValidateBrightness(brightness)) return false;
            bool allSuccess = true;
            foreach (var channel in channels)
            {
                if (!SetBrightness(channel, brightness)) allSuccess = false;
            }
            return allSuccess;
        }

        private string BuildCommand(int command, int channel, int data)
        {
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

        private string CalculateChecksumAscii(string frameWithoutChecksum)
        {
            int xor = 0;
            foreach (char ch in frameWithoutChecksum) { xor ^= (byte)ch; }
            int hi = (xor >> 4) & 0x0F;
            int lo = xor & 0x0F;
            char hiAsc = NibbleToAscii(hi);
            char loAsc = NibbleToAscii(lo);
            return new string(new[] { hiAsc, loAsc });
        }

        private char NibbleToAscii(int n) { return (char)(n < 10 ? ('0' + n) : ('A' + (n - 10))); }

        private bool SendAndExpectAck(string command)
        {
            var resp = SendAndRead(command, 80);
            if (string.IsNullOrEmpty(resp)) return false;
            return resp.Contains("#") && !resp.Contains("&");
        }

        private string SendAndRead(string command, int waitMs)
        {
            if (!IsConnected) return string.Empty;
            try
            {
                _serialPort.DiscardInBuffer();
                _serialPort.Write(command);
                Thread.Sleep(waitMs);
                if (_serialPort.BytesToRead > 0) return _serialPort.ReadExisting();
            }
            catch { }
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
            catch { return string.Empty; }
        }

        private bool ValidateChannel(int channel)
        {
            if (channel < 1 || channel > ChannelCount) return false;
            return true;
        }

        private bool ValidateBrightness(int brightness)
        {
            if (brightness < 0 || brightness > 255) return false;
            return true;
        }

        private StopBits ParseStopBits(double value)
        {
            return value == 1 ? StopBits.One : value == 1.5 ? StopBits.OnePointFive : value == 2 ? StopBits.Two : StopBits.One;
        }

        private Parity ParseParity(string value)
        {
            var v = value == null ? string.Empty : value.ToLower();
            switch (v)
            {
                case "odd": return Parity.Odd;
                case "even": return Parity.Even;
                case "mark": return Parity.Mark;
                case "space": return Parity.Space;
                default: return Parity.None;
            }
        }

        private Form CreateTestForm()
        {
            var f = new Form();
            f.Text = Name + " 测试";
            f.StartPosition = FormStartPosition.CenterParent;
            f.AutoScaleMode = AutoScaleMode.Font;
            f.Width = 640;
            f.Height = 420;

            var panel = new Panel { Dock = DockStyle.Fill };
            f.Controls.Add(panel);

            var gbBrightness = new GroupBox { Text = "亮度设置", Dock = DockStyle.Top, Height = ChannelCount * 60 + 60 };
            var tlp = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 3, RowCount = ChannelCount };
            tlp.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 60));
            tlp.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            tlp.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 60));

            var tracks = new TrackBar[ChannelCount];
            var nums = new NumericUpDown[ChannelCount];

            for (int i = 0; i < ChannelCount; i++)
            {
                var lbl = new Label { Text = "CH" + (i + 1) + ":", AutoSize = true, Anchor = AnchorStyles.Left };
                var track = new TrackBar { Minimum = 0, Maximum = 255, TickFrequency = 5, Dock = DockStyle.Fill };
                var num = new NumericUpDown { Minimum = 0, Maximum = 255, Width = 60, Anchor = AnchorStyles.Right };

                int ch = i + 1;
                track.Scroll += (s, e) => { num.Value = track.Value; SetBrightness(ch, track.Value); };
                num.ValueChanged += (s, e) => { track.Value = (int)num.Value; SetBrightness(ch, (int)num.Value); };

                try
                {
                    int cur = GetBrightness(ch);
                    if (cur >= 0) { track.Value = cur; num.Value = cur; }
                }
                catch { }

                tlp.Controls.Add(lbl, 0, i);
                tlp.Controls.Add(track, 1, i);
                tlp.Controls.Add(num, 2, i);

                tracks[i] = track;
                nums[i] = num;
            }

            gbBrightness.Controls.Add(tlp);
            panel.Controls.Add(gbBrightness);

            var gbMode = new GroupBox { Text = "常亮/常灭设置", Dock = DockStyle.Fill };
            var rbOn = new RadioButton { Text = "常亮", Left = 30, Top = 30, AutoSize = true };
            var rbOff = new RadioButton { Text = "常灭", Left = 120, Top = 30, AutoSize = true };
            rbOn.Checked = true;
            rbOn.CheckedChanged += (s, e) => { if (rbOn.Checked) { for (int ch = 1; ch <= ChannelCount; ch++) TurnOn(ch); } };
            rbOff.CheckedChanged += (s, e) => { if (rbOff.Checked) { for (int ch = 1; ch <= ChannelCount; ch++) TurnOff(ch); } };
            gbMode.Controls.Add(rbOn);
            gbMode.Controls.Add(rbOff);
            panel.Controls.Add(gbMode);

            return f;
        }

        public void Dispose() { Close(); }
    }
}
