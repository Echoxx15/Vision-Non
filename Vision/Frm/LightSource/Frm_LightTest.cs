using System;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;
using Logger;
using VisionSolution = Vision.Solutions.Models.Solution;
using Vision.Solutions.Models;
using Vision.LightSource;

namespace Vision.Frm.LightSource
{
    /// <summary>
    /// 光源测试界面（基于上传图片的4通道设计）
    /// 功能：
    /// 1. 实时控制光源开关
    /// 2. 调节亮度（滑动条+数值框）
    /// 3. 常亮/常灭切换
    /// 4. 发送自定义命令
    /// </summary>
    public partial class Frm_LightTest : Form
    {
        private ILightController _currentController;
        private LightConfig _currentConfig;
        private readonly VisionSolution _solution;

        // UI控件集合
        private ComboBox _cmbLightSource;
    private RadioButton _rdConstantOn;
        private RadioButton _rdConstantOff;
        private TextBox _txtCommand;
        private Button _btnSend;
private TextBox _txtLog;

        // 4个通道的控件
        private TrackBar[] _trackBars = new TrackBar[4];
      private NumericUpDown[] _numericUpDowns = new NumericUpDown[4];
        private CheckBox[] _checkBoxes = new CheckBox[4];

    public Frm_LightTest()
{
  InitializeComponent();
            _solution = SolutionManager.Instance.Current;
            InitializeUI();
     LoadLightSources();
        }

     private void InitializeUI()
      {
     // 设置窗体
       this.Text = "光源测试 - 4通道控制";
      this.Size = new Size(850, 650);
  this.StartPosition = FormStartPosition.CenterParent;
      this.FormBorderStyle = FormBorderStyle.FixedDialog;
     this.MaximizeBox = false;
          this.BackColor = Color.WhiteSmoke;

            // 创建主面板
        var mainPanel = new Panel
  {
    Location = new Point(10, 10),
     Size = new Size(820, 600)
            };

// === 顶部：光源选择 ===
      var lblSource = new Label
     {
         Text = "光源选择:",
       Location = new Point(10, 15),
         Size = new Size(80, 25),
    TextAlign = ContentAlignment.MiddleRight
      };

      _cmbLightSource = new ComboBox
      {
      Location = new Point(95, 15),
      Size = new Size(200, 25),
       DropDownStyle = ComboBoxStyle.DropDownList
  };
            _cmbLightSource.SelectedIndexChanged += CmbLightSource_SelectedIndexChanged;

            var btnConnect = new Button
   {
     Text = "连接",
  Location = new Point(305, 15),
      Size = new Size(80, 25)
 };
 btnConnect.Click += BtnConnect_Click;

  var btnDisconnect = new Button
 {
     Text = "断开",
   Location = new Point(395, 15),
           Size = new Size(80, 25)
   };
      btnDisconnect.Click += BtnDisconnect_Click;

            // === 中部：4通道控制区域 ===
   var channelPanel = new GroupBox
         {
   Text = "亮度设置",
    Location = new Point(10, 50),
    Size = new Size(800, 280),
       BackColor = Color.White
            };

            for (int i = 0; i < 4; i++)
 {
      int channel = i + 1;
     int yOffset = 30 + i * 60;

     // 通道标签
     var lblChannel = new Label
     {
         Text = $"CH{channel}:",
   Location = new Point(20, yOffset + 5),
   Size = new Size(50, 25),
            TextAlign = ContentAlignment.MiddleRight,
  Font = new Font("微软雅黑", 10, FontStyle.Bold)
       };

       // 启用复选框
          _checkBoxes[i] = new CheckBox
     {
  Text = "",
           Location = new Point(75, yOffset + 5),
          Size = new Size(20, 25),
          Checked = false
      };
         _checkBoxes[i].CheckedChanged += (s, e) => CheckBox_CheckedChanged(channel, _checkBoxes[i].Checked);

     // 滑动条
  _trackBars[i] = new TrackBar
        {
   Location = new Point(100, yOffset),
     Size = new Size(500, 45),
   Minimum = 0,
Maximum = 255,
          Value = 0,
   TickFrequency = 25,
    TickStyle = TickStyle.BottomRight
        };
   _trackBars[i].Scroll += (s, e) => TrackBar_Scroll(channel, _trackBars[i].Value);

                // 数值框
     _numericUpDowns[i] = new NumericUpDown
                {
        Location = new Point(610, yOffset + 5),
   Size = new Size(70, 25),
         Minimum = 0,
         Maximum = 255,
              Value = 0,
      Font = new Font("微软雅黑", 10)
       };
    _numericUpDowns[i].ValueChanged += (s, e) => NumericUpDown_ValueChanged(channel, (int)_numericUpDowns[i].Value);

 // 应用按钮
     var btnApply = new Button
  {
         Text = "应用",
          Location = new Point(690, yOffset + 3),
 Size = new Size(80, 28)
       };
    int channelIndex = i; // 捕获当前索引
     btnApply.Click += (s, e) => BtnApply_Click(channel, (int)_numericUpDowns[channelIndex].Value);

          channelPanel.Controls.AddRange(new Control[]
     {
         lblChannel, _checkBoxes[i], _trackBars[i], _numericUpDowns[i], btnApply
       });
   }

            // === 常亮/常灭设置 ===
    var modePanel = new GroupBox
         {
      Text = "常亮/常灭设置",
   Location = new Point(10, 340),
  Size = new Size(800, 60),
       BackColor = Color.White
  };

    _rdConstantOn = new RadioButton
  {
     Text = "常亮",
          Location = new Point(250, 25),
 Size = new Size(80, 25),
      Checked = false
       };
 _rdConstantOn.CheckedChanged += RdConstant_CheckedChanged;

  _rdConstantOff = new RadioButton
 {
      Text = "常灭",
     Location = new Point(450, 25),
     Size = new Size(80, 25),
    Checked = true
       };

   modePanel.Controls.AddRange(new Control[] { _rdConstantOn, _rdConstantOff });

     // === 串口通讯数据 ===
  var commPanel = new GroupBox
       {
      Text = "串口通讯数据",
          Location = new Point(10, 410),
        Size = new Size(800, 80),
     BackColor = Color.White
    };

          var lblSendData = new Label
     {
     Text = "发送数据框:",
     Location = new Point(20, 30),
      Size = new Size(90, 25)
            };

            _txtCommand = new TextBox
     {
   Location = new Point(115, 30),
        Size = new Size(550, 25),
      Font = new Font("Consolas", 10)
  };

            _btnSend = new Button
 {
    Text = "发送",
         Location = new Point(675, 28),
           Size = new Size(100, 28)
         };
   _btnSend.Click += BtnSend_Click;

 commPanel.Controls.AddRange(new Control[] { lblSendData, _txtCommand, _btnSend });

       // === 提示信息 ===
        var lblLog = new Label
            {
      Text = "提示信息:",
       Location = new Point(10, 500),
          Size = new Size(100, 20)
   };

     _txtLog = new TextBox
     {
   Location = new Point(10, 520),
       Size = new Size(800, 60),
  Multiline = true,
    ScrollBars = ScrollBars.Vertical,
    ReadOnly = true,
    BackColor = Color.LightYellow,
   Font = new Font("Consolas", 9)
       };

       // 添加所有控件到主面板
            mainPanel.Controls.AddRange(new Control[]
 {
     lblSource, _cmbLightSource, btnConnect, btnDisconnect,
     channelPanel, modePanel, commPanel, lblLog, _txtLog
   });

 this.Controls.Add(mainPanel);
        }

        private void LoadLightSources()
    {
_cmbLightSource.Items.Clear();

     if (_solution?.LightConfigs?.Configs == null) return;

          var enabledConfigs = _solution.LightConfigs.Configs
     .Where(c => c != null && c.Enabled && c.Mode == LightControllerMode.Digital)
        .ToList();

         foreach (var config in enabledConfigs)
{
            _cmbLightSource.Items.Add(config);
       }

   _cmbLightSource.DisplayMember = "Name";

     if (_cmbLightSource.Items.Count > 0)
      {
      _cmbLightSource.SelectedIndex = 0;
      }
     }

  private void CmbLightSource_SelectedIndexChanged(object sender, EventArgs e)
   {
   _currentConfig = _cmbLightSource.SelectedItem as LightConfig;
      }

      private void BtnConnect_Click(object sender, EventArgs e)
        {
      if (_currentConfig == null)
   {
       AppendLog("? 请先选择光源");
       return;
         }

       try
    {
    _currentController = _currentConfig.Type switch
   {
     LightControllerType.Fgen => new FgenLightController(_currentConfig),
_ => throw new NotSupportedException($"不支持的光源类型: {_currentConfig.Type}")
      };

           if (_currentController.Open())
  {
  AppendLog($"? 连接成功: {_currentConfig.Name} ({_currentConfig.PortName})");
     EnableControls(true);
    }
         else
   {
    AppendLog($"? 连接失败: {_currentConfig.Name}");
   _currentController = null;
        }
       }
            catch (Exception ex)
      {
    LogHelper.Error(ex, "连接光源失败");
   AppendLog($"? 异常: {ex.Message}");
    }
        }

      private void BtnDisconnect_Click(object sender, EventArgs e)
     {
         if (_currentController == null) return;

   try
       {
      _currentController.Close();
     _currentController.Dispose();
        _currentController = null;
    AppendLog("? 已断开连接");
    EnableControls(false);
  }
            catch (Exception ex)
     {
   LogHelper.Error(ex, "断开光源失败");
  }
      }

        private void CheckBox_CheckedChanged(int channel, bool isChecked)
{
     if (_currentController == null) return;

      try
            {
  if (isChecked)
     {
          int brightness = _trackBars[channel - 1].Value;
         _currentController.SetBrightness(channel, brightness);
      _currentController.TurnOn(channel);
              AppendLog($"? CH{channel} 已打开，亮度={brightness}");
   }
        else
                {
    _currentController.TurnOff(channel);
      AppendLog($"? CH{channel} 已关闭");
  }
   }
     catch (Exception ex)
     {
    AppendLog($"? CH{channel} 操作失败: {ex.Message}");
    }
      }

        private void TrackBar_Scroll(int channel, int value)
        {
       // 同步数值框
       _numericUpDowns[channel - 1].Value = value;
  }

private void NumericUpDown_ValueChanged(int channel, int value)
        {
       // 同步滑动条
     _trackBars[channel - 1].Value = value;
        }

        private void BtnApply_Click(int channel, int brightness)
        {
if (_currentController == null)
       {
        AppendLog("? 请先连接光源");
            return;
    }

       try
  {
        _currentController.SetBrightness(channel, brightness);
AppendLog($"? CH{channel} 亮度已设置为 {brightness}");
            }
          catch (Exception ex)
       {
       AppendLog($"? CH{channel} 设置失败: {ex.Message}");
        }
        }

        private void RdConstant_CheckedChanged(object sender, EventArgs e)
     {
   if (_currentController == null || !_rdConstantOn.Checked) return;

  try
      {
          // 常亮：打开所有通道，亮度设为当前滑动条值
    for (int i = 0; i < 4; i++)
      {
          int channel = i + 1;
        int brightness = _trackBars[i].Value;
        _currentController.SetBrightness(channel, brightness);
         _currentController.TurnOn(channel);
        _checkBoxes[i].Checked = true;
      }
      AppendLog("? 所有通道已打开（常亮模式）");
   }
            catch (Exception ex)
       {
   AppendLog($"? 常亮模式设置失败: {ex.Message}");
      }
  }

   private void BtnSend_Click(object sender, EventArgs e)
        {
     if (_currentController == null)
  {
       AppendLog("? 请先连接光源");
 return;
     }

 string command = _txtCommand.Text.Trim();
       if (string.IsNullOrEmpty(command))
     {
    AppendLog("? 请输入命令");
                return;
 }

     try
 {
      // 发送自定义命令（需要在ILightController中添加SendRawCommand方法）
       AppendLog($"→ 发送: {command}");
            // TODO: 实现原始命令发送
      // _currentController.SendRawCommand(command);
    AppendLog("? 原始命令发送功能待实现");
  }
            catch (Exception ex)
       {
      AppendLog($"? 发送失败: {ex.Message}");
            }
        }

private void EnableControls(bool enabled)
  {
      foreach (var cb in _checkBoxes) cb.Enabled = enabled;
            foreach (var tb in _trackBars) tb.Enabled = enabled;
   foreach (var nud in _numericUpDowns) nud.Enabled = enabled;
     _rdConstantOn.Enabled = enabled;
      _rdConstantOff.Enabled = enabled;
     _btnSend.Enabled = enabled;
        }

 private void AppendLog(string message)
        {
     string timestamp = DateTime.Now.ToString("HH:mm:ss");
            _txtLog.AppendText($"[{timestamp}] {message}\r\n");
         _txtLog.ScrollToCaret();
    }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
     
    if (_currentController != null)
   {
    try
    {
_currentController.Close();
      _currentController.Dispose();
       }
     catch { }
       }
        }
    }
}
