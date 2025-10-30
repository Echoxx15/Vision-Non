using Logger;
using System;
using System.IO.Ports;
using System.Windows.Forms;
using Vision.LightSource;
using Vision.Solutions.Models;

namespace Vision.Frm.LightSource;

/// <summary>
/// 光源配置界面
/// 功能：添加/删除光源控制器、编辑串口参数、测试连接、保存配置
/// </summary>
public partial class Frm_LightConfig : Form
{
  private LightConfig _currentConfig;
  private LightConfigCollection _configs;

  public Frm_LightConfig()
  {
    InitializeComponent();
    InitializeForm();
  }

  private void InitializeForm()
  {
    LoadConfigsFromSolution();
    listBox_Configs.SelectedIndexChanged += ListBox_Configs_SelectedIndexChanged;
    btn_AddFgen.Click += Btn_AddFgen_Click;
    btn_AddOpt.Click += Btn_AddOpt_Click;
    btn_Delete.Click += Btn_Delete_Click;
 btn_Save.Click += Btn_Save_Click;
    btn_Test.Click += Btn_Test_Click;
    InitializeComboBoxes();
    RefreshConfigList();
  }

  private void InitializeComboBoxes()
  {
    cmb_Type.Items.AddRange(new object[] { "孚根", "奥普特" });
    cmb_Type.SelectedIndex = 0;
    cmb_Mode.Items.AddRange(new object[] { "数字控制器", "频闪控制器" });
  cmb_Mode.SelectedIndex = 0;
    cmb_PortName.Items.AddRange(SerialPort.GetPortNames());
    if (cmb_PortName.Items.Count > 0) cmb_PortName.SelectedIndex = 0;
    cmb_BaudRate.Items.AddRange(new object[] { 4800, 9600, 19200, 38400, 57600, 115200 });
    cmb_BaudRate.SelectedItem = 9600;
    cmb_DataBits.Items.AddRange(new object[] { 5, 6, 7, 8 });
    cmb_DataBits.SelectedItem = 8;
    cmb_StopBits.Items.AddRange(new object[] { 1.0, 1.5, 2.0 });
    cmb_StopBits.SelectedItem = 1.0;
    cmb_Parity.Items.AddRange(new object[] { "None", "Odd", "Even", "Mark", "Space" });
  cmb_Parity.SelectedItem = "None";
    cmb_ChannelCount.Items.AddRange(new object[] { 2, 4, 8 });
    cmb_ChannelCount.SelectedItem = 4;
  }

  private void LoadConfigsFromSolution()
  {
 var solution = SolutionManager.Instance.Current;
    if (solution?.LightConfigs != null)
    {
      _configs = solution.LightConfigs;
    }
else
    {
      _configs = new LightConfigCollection();
  if (solution != null) solution.LightConfigs = _configs;
    }
  }

  private void RefreshConfigList()
  {
    var selectedIndex = listBox_Configs.SelectedIndex;
    listBox_Configs.Items.Clear();
    foreach (var config in _configs.Configs)
    {
      var status = config.Enabled ? "?" : "?";
   var typeText = config.Type == LightControllerType.Fgen ? "孚根" : "奥普特";
      var modeText = config.Mode == LightControllerMode.Digital ? "数字" : "频闪";
      listBox_Configs.Items.Add($"{status} {config.Name} ({typeText}-{modeText})");
    }
    if (selectedIndex >= 0 && selectedIndex < listBox_Configs.Items.Count)
      listBox_Configs.SelectedIndex = selectedIndex;
    else if (listBox_Configs.Items.Count > 0)
    listBox_Configs.SelectedIndex = 0;
  }

  private void Btn_AddFgen_Click(object sender, EventArgs e)
  {
    var config = new LightConfig
    {
      Name = _configs.GenerateUniqueName(LightControllerType.Fgen),
      Type = LightControllerType.Fgen,
      Mode = LightControllerMode.Digital,
      Enabled = true,
      PortName = "COM1",
      BaudRate = 9600,
      DataBits = 8,
      StopBits = 1,
      Parity = "None",
      ChannelCount = 4
    };
    _configs.Add(config);
    RefreshConfigList();
    listBox_Configs.SelectedIndex = listBox_Configs.Items.Count - 1;
    LogHelper.Info($"添加孚根光源配置: {config.Name}");
  }

  private void Btn_AddOpt_Click(object sender, EventArgs e)
  {
    var config = new LightConfig
    {
      Name = _configs.GenerateUniqueName(LightControllerType.Opt),
      Type = LightControllerType.Opt,
      Mode = LightControllerMode.Digital,
   Enabled = true,
      PortName = "COM1",
      BaudRate = 9600,
      DataBits = 8,
      StopBits = 1,
 Parity = "None",
    ChannelCount = 4
    };
    _configs.Add(config);
    RefreshConfigList();
    listBox_Configs.SelectedIndex = listBox_Configs.Items.Count - 1;
 LogHelper.Info($"添加奥普特光源配置: {config.Name}");
  }

  private void Btn_Delete_Click(object sender, EventArgs e)
  {
    if (_currentConfig == null)
    {
      MessageBox.Show("请先选择要删除的配置", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
      return;
    }
    var result = MessageBox.Show($"确认删除配置 [{_currentConfig.Name}] ?",
    "确认删除", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
    if (result == DialogResult.Yes)
    {
  _configs.Remove(_currentConfig);
      _currentConfig = null;
      RefreshConfigList();
      ClearConfigPanel();
      LogHelper.Info("删除光源配置");
    }
  }

  private void ListBox_Configs_SelectedIndexChanged(object sender, EventArgs e)
  {
    if (listBox_Configs.SelectedIndex < 0 || listBox_Configs.SelectedIndex >= _configs.Configs.Count)
    {
      ClearConfigPanel();
  return;
}
    _currentConfig = _configs.Configs[listBox_Configs.SelectedIndex];
    LoadConfigToPanel(_currentConfig);
  }

  private void LoadConfigToPanel(LightConfig config)
  {
    if (config == null) return;
    txt_Name.Text = config.Name;
    cmb_Type.SelectedIndex = (int)config.Type;
    cmb_Mode.SelectedIndex = (int)config.Mode;
    chk_Enabled.Checked = config.Enabled;
    cmb_PortName.Text = config.PortName;
    cmb_BaudRate.SelectedItem = config.BaudRate;
    cmb_DataBits.SelectedItem = config.DataBits;
    cmb_StopBits.SelectedItem = config.StopBits;
    cmb_Parity.SelectedItem = config.Parity;
    cmb_ChannelCount.SelectedItem = config.ChannelCount;
    txt_Remark.Text = config.Remark;
    lbl_Type.Text = config.Type == LightControllerType.Fgen ? "孚根控制器" : "奥普特控制器";
  }

  private void ClearConfigPanel()
  {
    txt_Name.Clear();
    lbl_Type.Text = "-";
    cmb_Type.SelectedIndex = 0;
    cmb_Mode.SelectedIndex = 0;
    chk_Enabled.Checked = true;
    cmb_PortName.SelectedIndex = cmb_PortName.Items.Count > 0 ? 0 : -1;
    cmb_BaudRate.SelectedItem = 9600;
    cmb_DataBits.SelectedItem = 8;
    cmb_StopBits.SelectedItem = 1.0;
    cmb_Parity.SelectedItem = "None";
    cmb_ChannelCount.SelectedItem = 4;
    txt_Remark.Clear();
  }

  private void SavePanelToConfig()
  {
    if (_currentConfig == null) return;
    _currentConfig.Type = (LightControllerType)cmb_Type.SelectedIndex;
  _currentConfig.Mode = (LightControllerMode)cmb_Mode.SelectedIndex;
    _currentConfig.Enabled = chk_Enabled.Checked;
    _currentConfig.PortName = cmb_PortName.Text;
    _currentConfig.BaudRate = (int)cmb_BaudRate.SelectedItem;
    _currentConfig.DataBits = (int)cmb_DataBits.SelectedItem;
    _currentConfig.StopBits = (double)cmb_StopBits.SelectedItem;
    _currentConfig.Parity = cmb_Parity.SelectedItem?.ToString() ?? "None";
    _currentConfig.ChannelCount = (int)cmb_ChannelCount.SelectedItem;
    _currentConfig.Remark = txt_Remark.Text;
  }

  private void Btn_Save_Click(object sender, EventArgs e)
  {
    try
  {
      SavePanelToConfig();
  SolutionManager.Instance.SaveCurrent();
      MessageBox.Show("光源配置已保存到方案", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
      LogHelper.Info("光源配置已保存到方案");
      var solution = SolutionManager.Instance.Current;
      if (solution != null)
     LightSourceManager.Instance.InitializeFromSolution(solution);
    }
    catch (Exception ex)
    {
      MessageBox.Show($"保存失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
LogHelper.Error(ex, "保存光源配置失败");
    }
  }

  private void Btn_Test_Click(object sender, EventArgs e)
  {
    if (_currentConfig == null)
    {
      MessageBox.Show("请先选择要测试的配置", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
      return;
  }
    SavePanelToConfig();
    // 打开测试窗口（测试窗口会自动加载已启用的配置）
    using (var testForm = new Frm_LightTest())
    {
      testForm.ShowDialog(this);
    }
  }
}
