using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Logger;
using Vision.LightSource;
using VisionSolution = Vision.Solutions.Models.Solution;
using Vision.Solutions.Models;

namespace Vision.Frm.LightSource
{
 public partial class Frm_LightTest : Form
 {
 private ILightController _currentController;
 private LightConfig _currentConfig;
 private readonly VisionSolution _solution;

 // 引用 Designer 控件的简化数组
 private TrackBar[] _trackBars = new TrackBar[4];
 private NumericUpDown[] _numericUpDowns = new NumericUpDown[4];
 private CheckBox[] _checkBoxes = new CheckBox[4];
 private Button[] _btnApply = new Button[4];

 public Frm_LightTest()
 {
 InitializeComponent();
 _solution = SolutionManager.Instance.Current;
 }

 private void Frm_LightTest_Load(object sender, EventArgs e)
 {
 //绑定 Designer 控件到数组，便于统一处理
 _trackBars = new[] { trackBar1, trackBar2, trackBar3, trackBar4 };
 _numericUpDowns = new[] { numBrightness1, numBrightness2, numBrightness3, numBrightness4 };
 _checkBoxes = new[] { chkCh1, chkCh2, chkCh3, chkCh4 };
 _btnApply = new[] { btnApply1, btnApply2, btnApply3, btnApply4 };

 // 初始禁用所有操作控件，由配置连接状态决定
 EnableControls(false);

 // 设置下拉框显示字段
 cmbLightSource.DisplayMember = "Name";

 // 加载光源配置
 LoadLightSources();
 }

 private void LoadLightSources()
 {
 cmbLightSource.Items.Clear();

 if (_solution?.LightConfigs?.Configs == null)
 {
 AppendLog("? 没有光源配置");
 return;
 }

 var enabledConfigs = _solution.LightConfigs.Configs
 .Where(c => c != null && c.Enabled && c.Mode == LightControllerMode.Digital)
 .ToList();

 foreach (var config in enabledConfigs)
 {
 cmbLightSource.Items.Add(config);
 }

 if (cmbLightSource.Items.Count >0)
 {
 cmbLightSource.SelectedIndex =0;
 }
 else
 {
 AppendLog("? 没有启用的数字光源配置");
 EnableControls(false);
 }
 }

 private void cmbLightSource_SelectedIndexChanged(object sender, EventArgs e)
 {
 _currentConfig = cmbLightSource.SelectedItem as LightConfig;

 if (_currentConfig == null)
 {
 _currentController = null;
 EnableControls(false);
 AppendLog("?请选择光源");
 return;
 }

 // 从 LightSourceManager 获取已连接的控制器
 _currentController = LightSourceManager.Instance.GetController(_currentConfig.Name);

 if (_currentController != null && _currentController.IsConnected)
 {
 EnableControls(true);
 AppendLog($"? 已连接: {_currentConfig.Name} ({_currentConfig.PortName})");
 SyncBrightnessDisplay();
 }
 else
 {
 EnableControls(false);
 AppendLog($"? 光源未连接: {_currentConfig.Name}");
 AppendLog(" 提示: 请在光源配置界面启用该光源");
 }
 }

 private void SyncBrightnessDisplay()
 {
 if (_currentController == null) return;

 try
 {
 for (int i =0; i <4; i++)
 {
 int channel = i +1;
 int brightness = _currentController.GetBrightness(channel);
 _trackBars[i].Value = Math.Max(_trackBars[i].Minimum, Math.Min(_trackBars[i].Maximum, brightness));
 _numericUpDowns[i].Value = Math.Max(_numericUpDowns[i].Minimum, Math.Min(_numericUpDowns[i].Maximum, brightness));
 }
 }
 catch (Exception ex)
 {
 LogHelper.Warn($"同步亮度显示失败: {ex.Message}");
 }
 }

 private void CheckBox_CheckedChanged(object sender, EventArgs e)
 {
 if (_currentController == null) return;
 var checkBox = sender as CheckBox;
 int index = Array.IndexOf(_checkBoxes, checkBox);
 if (index <0) return;

 int channel = index +1;
 try
 {
 if (checkBox.Checked)
 {
 int brightness = _trackBars[index].Value;
 _currentController.SetBrightness(channel, brightness);
 _currentController.TurnOn(channel);
 // 不打印发送内容
 }
 else
 {
 _currentController.TurnOff(channel);
 }
 }
 catch (Exception ex)
 {
 AppendLog($"? CH{channel} 操作失败: {ex.Message}");
 }
 }

 private void TrackBar_Scroll(object sender, EventArgs e)
 {
 var trackBar = sender as TrackBar;
 int index = Array.IndexOf(_trackBars, trackBar);
 if (index <0) return;

 // 同步数值框
 _numericUpDowns[index].Value = trackBar.Value;

 // 实时改变亮度（不打印发送内容）
 if (_currentController != null)
 {
 int channel = index +1;
 try
 {
 _currentController.SetBrightness(channel, trackBar.Value);
 // 协议约定设置即自动开灯，同步界面勾选
 if (!_checkBoxes[index].Checked)
 _checkBoxes[index].Checked = true;
 }
 catch (Exception ex)
 {
 //仅记录错误，不打印发送帧
 LogHelper.Warn($"实时设置亮度失败 CH{channel}: {ex.Message}");
 }
 }
 }

 private void NumericUpDown_ValueChanged(object sender, EventArgs e)
 {
 var n = sender as NumericUpDown;
 int index = Array.IndexOf(_numericUpDowns, n);
 if (index <0) return;
 _trackBars[index].Value = (int)n.Value;
 // 数值框变更不强制实时下发，避免输入期间过多通讯
 }

 private void btnApply_Click(object sender, EventArgs e)
 {
 if (_currentController == null)
 {
 AppendLog("? 请先选择已连接的光源");
 return;
 }

 var btn = sender as Button;
 int index = Array.IndexOf(_btnApply, btn);
 if (index <0) return;

 int channel = index +1;
 int brightness = (int)_numericUpDowns[index].Value;
 try
 {
 _currentController.SetBrightness(channel, brightness);
 AppendLog($"? CH{channel}亮度已设置为 {brightness}");
 }
 catch (Exception ex)
 {
 AppendLog($"? CH{channel} 设置失败: {ex.Message}");
 }
 }

 private void rdConstantOn_CheckedChanged(object sender, EventArgs e)
 {
 if (_currentController == null || !rdConstantOn.Checked) return;

 try
 {
 for (int i =0; i <4; i++)
 {
 int channel = i +1;
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

 private void rdConstantOff_CheckedChanged(object sender, EventArgs e)
 {
 if (_currentController == null || !rdConstantOff.Checked) return;

 try
 {
 for (int i =0; i <4; i++)
 {
 int channel = i +1;
 _currentController.TurnOff(channel);
 _checkBoxes[i].Checked = false;
 }
 AppendLog("? 所有通道已关闭（常灭模式）");
 }
 catch (Exception ex)
 {
 AppendLog($"? 常灭模式设置失败: {ex.Message}");
 }
 }

 private void btnSend_Click(object sender, EventArgs e)
 {
 if (_currentController == null)
 {
 AppendLog("? 请先选择已连接的光源");
 return;
 }

 string command = txtCommand.Text.Trim();
 if (string.IsNullOrEmpty(command))
 {
 AppendLog("?请输入命令");
 return;
 }

 try
 {
 //直接发送原始命令，不做任何转换
 var resp = _currentController.SendRawCommand(command);
 AppendLog($"→发送: {command}");
 if (!string.IsNullOrEmpty(resp))
 AppendLog($"← 接收: {resp}");
 }
 catch (Exception ex)
 {
 AppendLog($"?发送失败: {ex.Message}");
 }
 }

 private void EnableControls(bool enabled)
 {
 foreach (var cb in _checkBoxes) if (cb != null) cb.Enabled = enabled;
 foreach (var tb in _trackBars) if (tb != null) tb.Enabled = enabled;
 foreach (var nud in _numericUpDowns) if (nud != null) nud.Enabled = enabled;
 rdConstantOn.Enabled = enabled;
 rdConstantOff.Enabled = enabled;
 btnSend.Enabled = enabled;
 }

 private void AppendLog(string message)
 {
 string timestamp = DateTime.Now.ToString("HH:mm:ss");
 txtLog.AppendText($"[{timestamp}] {message}\r\n");
 txtLog.ScrollToCaret();
 }

 protected override void OnFormClosing(FormClosingEventArgs e)
 {
 base.OnFormClosing(e);
 // 不断开连接，连接由 LightSourceManager统一管理
 }
 }
}
