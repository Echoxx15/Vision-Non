using System;
using System.IO.Ports;
using System.Text;
using System.Windows.Forms;

namespace UserControls.LightTestForm
{
    /// <summary>
    /// 孚根频闪光源测试窗口
    /// 通讯协议：串口 19200,8,1,无校验
    /// 帧格式：特征字(3字节) + 通道字(1字节) + 数据(3字节) + #(结束符)
    /// </summary>
    public partial class FugenPinshanLight : Form
    {
        private SerialPort _serialPort;
        private bool _isConnected;

        public FugenPinshanLight()
        {
            InitializeComponent();
            InitializeForm();
        }

        private void InitializeForm()
        {
            // 枚举可用串口
            RefreshPorts();

            // 绑定事件
            btnConnect.Click += BtnConnect_Click;
            btnWriteTriggerTime.Click += BtnWriteTriggerTime_Click;
            btnReadTriggerTime.Click += BtnReadTriggerTime_Click;
            btnWriteDelayTime.Click += BtnWriteDelayTime_Click;
            btnReadDelayTime.Click += BtnReadDelayTime_Click;
            btnWriteTriggerType.Click += BtnWriteTriggerType_Click;
            btnReadTriggerType.Click += BtnReadTriggerType_Click;
            btnWriteInternalPeriod.Click += BtnWriteInternalPeriod_Click;
            btnReadInternalPeriod.Click += BtnReadInternalPeriod_Click;
            btnSoftTrigger.Click += BtnSoftTrigger_Click;

            // 窗体关闭时断开连接
            FormClosing += FugenPinshanLight_FormClosing;

            // 默认选择
            cmbChannel.SelectedIndex = 0;
            cmbTriggerType.SelectedIndex = 0;
        }

        private void RefreshPorts()
        {
            cmbPort.Items.Clear();
            var ports = SerialPort.GetPortNames();
            cmbPort.Items.AddRange(ports);
            if (cmbPort.Items.Count > 0)
                cmbPort.SelectedIndex = 0;
        }

        #region 连接/断开

        private void BtnConnect_Click(object sender, EventArgs e)
        {
            if (_isConnected)
            {
                Disconnect();
            }
            else
            {
                Connect();
            }
        }

        private void Connect()
        {
            if (cmbPort.SelectedItem == null)
            {
                MessageBox.Show("请选择串口", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                _serialPort = new SerialPort
                {
                    PortName = cmbPort.SelectedItem.ToString(),
                    BaudRate = 19200,
                    DataBits = 8,
                    StopBits = StopBits.One,
                    Parity = Parity.None,
                    ReadTimeout = 1000,
                    WriteTimeout = 1000
                };
                _serialPort.Open();
                _isConnected = true;

                btnConnect.Text = "断开连接";
                cmbPort.Enabled = false;
                SetControlsEnabled(true);

                MessageBox.Show($"串口 {_serialPort.PortName} 连接成功", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"连接失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                _isConnected = false;
            }
        }

        private void Disconnect()
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

            _isConnected = false;
            btnConnect.Text = "连接";
            cmbPort.Enabled = true;
            SetControlsEnabled(false);
        }

        private void SetControlsEnabled(bool enabled)
        {
            cmbChannel.Enabled = enabled;
            nudTriggerTimeWrite.Enabled = enabled;
            nudDelayTimeWrite.Enabled = enabled;
            cmbTriggerType.Enabled = enabled;
            nudInternalPeriodWrite.Enabled = enabled;

            btnWriteTriggerTime.Enabled = enabled;
            btnReadTriggerTime.Enabled = enabled;
            btnWriteDelayTime.Enabled = enabled;
            btnReadDelayTime.Enabled = enabled;
            btnWriteTriggerType.Enabled = enabled;
            btnReadTriggerType.Enabled = enabled;
            btnWriteInternalPeriod.Enabled = enabled;
            btnReadInternalPeriod.Enabled = enabled;
            btnSoftTrigger.Enabled = enabled;
        }

        private void FugenPinshanLight_FormClosing(object sender, FormClosingEventArgs e)
        {
            Disconnect();
        }

        #endregion

        #region 通讯方法

        /// <summary>
        /// 获取当前选择的通道号 (1-4)
        /// </summary>
        private string GetChannel()
        {
            return (cmbChannel.SelectedIndex + 1).ToString();
        }

        /// <summary>
        /// 发送命令并接收响应
        /// </summary>
        /// <param name="command">命令字符串（不含#结束符）</param>
        /// <returns>响应字符串（不含#结束符）</returns>
        private string SendCommand(string command)
        {
            if (_serialPort == null || !_serialPort.IsOpen)
            {
                throw new InvalidOperationException("串口未连接");
            }

            // 清空缓冲区
            _serialPort.DiscardInBuffer();
            _serialPort.DiscardOutBuffer();

            // 发送命令（加#结束符）
            string fullCommand = command + "#";
            byte[] data = Encoding.ASCII.GetBytes(fullCommand);
            _serialPort.Write(data, 0, data.Length);

            // 读取响应（直到#结束符）
            var response = new StringBuilder();
            var timeout = DateTime.Now.AddMilliseconds(1000);

            while (DateTime.Now < timeout)
            {
                if (_serialPort.BytesToRead > 0)
                {
                    char c = (char)_serialPort.ReadChar();
                    if (c == '#')
                        break;
                    response.Append(c);
                }
                System.Threading.Thread.Sleep(10);
            }

            return response.ToString();
        }

        /// <summary>
        /// 格式化数值为3位字符串 (000-999)
        /// </summary>
        private string FormatValue3(int value)
        {
            return value.ToString("D3");
        }

        /// <summary>
        /// 格式化数值为2位字符串 (00-99)
        /// </summary>
        private string FormatValue2(int value)
        {
            return value.ToString("D2");
        }

        #endregion

        #region 3.2 设置/读取触发点亮时间 (SWT/RWT, 0-999μs)

        private void BtnWriteTriggerTime_Click(object sender, EventArgs e)
        {
            try
            {
                // 格式: SWT + 通道(1字节) + 时间(3字节) + #
                string cmd = $"SWT{GetChannel()}{FormatValue3((int)nudTriggerTimeWrite.Value)}";
                string response = SendCommand(cmd);

                // 响应格式: SWT + 通道 + 时间
                if (response.StartsWith("SWT"))
                {
                    MessageBox.Show("设置触发点亮时间成功", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show($"设置失败，响应: {response}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"操作失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnReadTriggerTime_Click(object sender, EventArgs e)
        {
            try
            {
                // 格式: RWT + 通道(1字节) + #
                string cmd = $"RWT{GetChannel()}";
                string response = SendCommand(cmd);

                // 响应格式: RWT + 通道 + 时间(3字节)
                if (response.StartsWith("RWT") && response.Length >= 7)
                {
                    string timeStr = response.Substring(4, 3);
                    if (int.TryParse(timeStr, out int time))
                    {
                        nudTriggerTimeRead.Value = time;
                        MessageBox.Show($"读取触发点亮时间: {time}μs", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                else
                {
                    MessageBox.Show($"读取失败，响应: {response}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"操作失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region 3.4/3.5 设置/读取延时点亮时间 (SYT/RYT, 0-99μs)

        private void BtnWriteDelayTime_Click(object sender, EventArgs e)
        {
            try
            {
                // 格式: SYT + 通道(1字节) + 时间(2字节) + #
                string cmd = $"SYT{GetChannel()}{FormatValue2((int)nudDelayTimeWrite.Value)}";
                string response = SendCommand(cmd);

                if (response.StartsWith("SYT"))
                {
                    MessageBox.Show("设置延时点亮时间成功", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show($"设置失败，响应: {response}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"操作失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnReadDelayTime_Click(object sender, EventArgs e)
        {
            try
            {
                // 格式: RYT + 通道(1字节) + #
                string cmd = $"RYT{GetChannel()}";
                string response = SendCommand(cmd);

                // 响应格式: RYT + 通道 + 时间(2字节)
                if (response.StartsWith("RYT") && response.Length >= 6)
                {
                    string timeStr = response.Substring(4, 2);
                    if (int.TryParse(timeStr, out int time))
                    {
                        nudDelayTimeRead.Value = time;
                        MessageBox.Show($"读取延时点亮时间: {time}μs", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                else
                {
                    MessageBox.Show($"读取失败，响应: {response}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"操作失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region 3.6/3.7 设置/读取触发器类型 (SCM/RCM, 0-3)

        private void BtnWriteTriggerType_Click(object sender, EventArgs e)
        {
            try
            {
                // 格式: SCM + 通道(1字节) + 模式(1字节) + #
                string mode = cmbTriggerType.SelectedIndex.ToString();
                string cmd = $"SCM{GetChannel()}{mode}";
                string response = SendCommand(cmd);

                if (response.StartsWith("SCM"))
                {
                    MessageBox.Show("设置触发器类型成功", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show($"设置失败，响应: {response}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"操作失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnReadTriggerType_Click(object sender, EventArgs e)
        {
            try
            {
                // 格式: RCM + 通道(1字节) + #
                string cmd = $"RCM{GetChannel()}";
                string response = SendCommand(cmd);

                // 响应格式: RCM + 通道 + 模式(1字节)
                if (response.StartsWith("RCM") && response.Length >= 5)
                {
                    string modeStr = response.Substring(4, 1);
                    if (int.TryParse(modeStr, out int mode) && mode >= 0 && mode <= 3)
                    {
                        string[] modeNames = { "上升沿触发", "下降沿触发", "常亮", "内部触发" };
                        lblTriggerTypeRead.Text = $"{mode}-{modeNames[mode]}";
                        MessageBox.Show($"读取触发器类型: {mode}-{modeNames[mode]}", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                else
                {
                    MessageBox.Show($"读取失败，响应: {response}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"操作失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region 3.8/3.9 设置/读取内部触发周期 (SNT/RNT, 10-999ms)

        private void BtnWriteInternalPeriod_Click(object sender, EventArgs e)
        {
            try
            {
                // 格式: SNT + 时间(3字节) + #  （无通道号）
                string cmd = $"SNT{FormatValue3((int)nudInternalPeriodWrite.Value)}";
                string response = SendCommand(cmd);

                if (response.StartsWith("SNT"))
                {
                    MessageBox.Show("设置内部触发周期成功", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show($"设置失败，响应: {response}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"操作失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnReadInternalPeriod_Click(object sender, EventArgs e)
        {
            try
            {
                // 格式: RNT + #  （无通道号）
                string cmd = "RNT";
                string response = SendCommand(cmd);

                // 响应格式: RNT + 时间(3字节)
                if (response.StartsWith("RNT") && response.Length >= 6)
                {
                    string timeStr = response.Substring(3, 3);
                    if (int.TryParse(timeStr, out int time))
                    {
                        nudInternalPeriodRead.Value = Math.Max(10, Math.Min(999, time));
                        MessageBox.Show($"读取内部触发周期: {time}ms", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                else
                {
                    MessageBox.Show($"读取失败，响应: {response}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"操作失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region 3.10 软件触发通道点亮一次 (TRIG)

        private void BtnSoftTrigger_Click(object sender, EventArgs e)
        {
            try
            {
                // 格式: TRIG + 通道(1字节) + #
                string cmd = $"TRIG{GetChannel()}";
                string response = SendCommand(cmd);

                // 响应格式: TRIG + 通道
                if (response.StartsWith("TRIG"))
                {
                    MessageBox.Show($"通道{GetChannel()}已触发点亮一次", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show($"触发失败，响应: {response}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"操作失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion
    }
}
