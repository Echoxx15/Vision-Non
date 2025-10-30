using Logger;
using System;
using System.Windows.Forms;
using Vision.Comm.TcpConfig;
using Vision.Solutions.Models;

namespace Vision.Frm.TcpConfig;

/// <summary>
/// TCP通讯配置界面
/// 
/// 功能：
/// 1. 添加/删除TCP客户端和服务器配置
/// 2. 编辑TCP配置参数
/// 3. 收发测试功能
/// 4. 保存配置到方案
/// </summary>
public partial class Frm_TcpConfig : Form
{
  private TcpConfigModel _currentConfig;
  private TcpConfigCollection _configs;

  public Frm_TcpConfig()
  {
    InitializeComponent();
    InitializeForm();
  }

  /// <summary>
  /// 初始化界面
  /// </summary>
  private void InitializeForm()
  {
    // 从当前方案加载配置
    LoadConfigsFromSolution();

    // 绑定事件
    listBox_Configs.SelectedIndexChanged += ListBox_Configs_SelectedIndexChanged;
    btn_AddClient.Click += Btn_AddClient_Click;
    btn_AddServer.Click += Btn_AddServer_Click;
    btn_Delete.Click += Btn_Delete_Click;
    btn_Save.Click += Btn_Save_Click;
    btn_Test.Click += Btn_Test_Click;

    // 编码下拉框默认选择
    cmb_Encoding.SelectedIndex = 0; // UTF-8

    // 刷新列表
    RefreshConfigList();
  }

  /// <summary>
  /// 从方案加载配置
  /// </summary>
  private void LoadConfigsFromSolution()
  {
    var solution = SolutionManager.Instance.Current;
    if (solution?.TcpConfigs != null)
    {
      _configs = solution.TcpConfigs;
    }
    else
    {
      _configs = new TcpConfigCollection();
      if (solution != null)
      {
        solution.TcpConfigs = _configs;
      }
    }
  }

  /// <summary>
  /// 刷新配置列表
  /// </summary>
  private void RefreshConfigList()
  {
    var selectedIndex = listBox_Configs.SelectedIndex;

    listBox_Configs.Items.Clear();
    foreach (var config in _configs.Configs)
    {
      var status = config.Enabled ? "?" : "?";
      var typeText = config.Type == TcpType.Client ? "客户端" : "服务器";
      listBox_Configs.Items.Add($"{status} {config.Name} ({typeText})");
    }

    // 恢复选择
    if (selectedIndex >= 0 && selectedIndex < listBox_Configs.Items.Count)
    {
      listBox_Configs.SelectedIndex = selectedIndex;
    }
    else if (listBox_Configs.Items.Count > 0)
    {
      listBox_Configs.SelectedIndex = 0;
    }
  }

  /// <summary>
  /// 添加客户端按钮点击
  /// </summary>
  private void Btn_AddClient_Click(object sender, EventArgs e)
  {
    var config = new TcpConfigModel
    {
      Name = _configs.GenerateUniqueName(TcpType.Client),
      Type = TcpType.Client,
      IpAddress = "127.0.0.1",
      Port = 5000,
      Enabled = true,
      Encoding = "UTF-8",
      AutoReconnect = true,
      ConnectTimeout = 5000,
      ReconnectInterval = 3000
    };

    _configs.Add(config);
    RefreshConfigList();

    // 选中新添加的项
    listBox_Configs.SelectedIndex = listBox_Configs.Items.Count - 1;

    LogHelper.Info($"添加TCP客户端配置: {config.Name}");
  }

  /// <summary>
  /// 添加服务器按钮点击
  /// </summary>
  private void Btn_AddServer_Click(object sender, EventArgs e)
  {
    var config = new TcpConfigModel
    {
      Name = _configs.GenerateUniqueName(TcpType.Server),
      Type = TcpType.Server,
      IpAddress = "0.0.0.0",
      Port = 5000,
      Enabled = true,
      Encoding = "UTF-8",
      MaxConnections = 10
    };

    _configs.Add(config);
    RefreshConfigList();

    // 选中新添加的项
    listBox_Configs.SelectedIndex = listBox_Configs.Items.Count - 1;

    LogHelper.Info($"添加TCP服务器配置: {config.Name}");
  }

  /// <summary>
  /// 删除按钮点击
  /// </summary>
  private void Btn_Delete_Click(object sender, EventArgs e)
  {
    if (_currentConfig == null)
    {
      MessageBox.Show("请先选择要删除的配置", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
      return;
    }

    var result = MessageBox.Show(
      $"确认删除配置 [{_currentConfig.Name}] ?",
      "确认删除",
      MessageBoxButtons.YesNo,
      MessageBoxIcon.Question);

    if (result == DialogResult.Yes)
    {
      _configs.Remove(_currentConfig);
      _currentConfig = null;
      RefreshConfigList();
      ClearConfigPanel();

      LogHelper.Info($"删除TCP配置: {_currentConfig?.Name}");
    }
  }

  /// <summary>
  /// 列表选择变化
  /// </summary>
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

  /// <summary>
  /// 将配置加载到界面
  /// </summary>
  private void LoadConfigToPanel(TcpConfigModel config)
  {
    if (config == null) return;

    txt_Name.Text = config.Name;
    lbl_Type.Text = config.Type == TcpType.Client ? "TCP客户端" : "TCP服务器";
    txt_IpAddress.Text = config.IpAddress;
    num_Port.Value = config.Port;
    chk_Enabled.Checked = config.Enabled;
    num_SendBufferSize.Value = config.SendBufferSize;
    num_ReceiveBufferSize.Value = config.ReceiveBufferSize;

    // 编码
    var encodingIndex = cmb_Encoding.Items.IndexOf(config.Encoding);
    cmb_Encoding.SelectedIndex = encodingIndex >= 0 ? encodingIndex : 0;

    chk_UseTerminator.Checked = config.UseTerminator;
    txt_Terminator.Text = config.Terminator;

    // 客户端特有参数
    num_ConnectTimeout.Value = config.ConnectTimeout;
    chk_AutoReconnect.Checked = config.AutoReconnect;
    num_ReconnectInterval.Value = config.ReconnectInterval;

    // 服务器特有参数
    num_MaxConnections.Value = config.MaxConnections;

    txt_Remark.Text = config.Remark;

    // 根据类型启用/禁用控件
    UpdateControlsVisibility(config.Type);
  }

  /// <summary>
  /// 根据TCP类型更新控件可见性
  /// </summary>
  private void UpdateControlsVisibility(TcpType type)
  {
    bool isClient = type == TcpType.Client;

    // 客户端特有
    num_ConnectTimeout.Enabled = isClient;
    chk_AutoReconnect.Enabled = isClient;
    num_ReconnectInterval.Enabled = isClient;

    // 服务器特有
    num_MaxConnections.Enabled = !isClient;
  }

  /// <summary>
  /// 清空配置面板
  /// </summary>
  private void ClearConfigPanel()
  {
    txt_Name.Clear();
    lbl_Type.Text = "-";
    txt_IpAddress.Text = "127.0.0.1";
    num_Port.Value = 5000;
    chk_Enabled.Checked = true;
    num_SendBufferSize.Value = 0;
    num_ReceiveBufferSize.Value = 0;
    cmb_Encoding.SelectedIndex = 0;
    chk_UseTerminator.Checked = false;
    txt_Terminator.Text = @"\r\n";
    num_ConnectTimeout.Value = 5000;
    chk_AutoReconnect.Checked = true;
    num_ReconnectInterval.Value = 3000;
    num_MaxConnections.Value = 10;
    txt_Remark.Clear();
  }

  /// <summary>
  /// 保存界面数据到配置
  /// </summary>
  private void SavePanelToConfig()
  {
    if (_currentConfig == null) return;

    _currentConfig.IpAddress = txt_IpAddress.Text.Trim();
    _currentConfig.Port = (int)num_Port.Value;
    _currentConfig.Enabled = chk_Enabled.Checked;
    _currentConfig.SendBufferSize = (int)num_SendBufferSize.Value;
    _currentConfig.ReceiveBufferSize = (int)num_ReceiveBufferSize.Value;
    _currentConfig.Encoding = cmb_Encoding.SelectedItem?.ToString() ?? "UTF-8";
    _currentConfig.UseTerminator = chk_UseTerminator.Checked;
    _currentConfig.Terminator = txt_Terminator.Text;
    _currentConfig.ConnectTimeout = (int)num_ConnectTimeout.Value;
    _currentConfig.AutoReconnect = chk_AutoReconnect.Checked;
    _currentConfig.ReconnectInterval = (int)num_ReconnectInterval.Value;
    _currentConfig.MaxConnections = (int)num_MaxConnections.Value;
    _currentConfig.Remark = txt_Remark.Text;
  }

  /// <summary>
  /// 保存方案按钮点击
  /// </summary>
  private void Btn_Save_Click(object sender, EventArgs e)
  {
    try
    {
      // 保存当前编辑的配置
      SavePanelToConfig();

      // 保存方案
      SolutionManager.Instance.SaveCurrent();

      MessageBox.Show("TCP配置已保存到方案", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
      LogHelper.Info("TCP配置已保存到方案");
    }
    catch (Exception ex)
    {
      MessageBox.Show($"保存失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
      LogHelper.Error(ex, "保存TCP配置失败");
    }
  }

  /// <summary>
  /// 收发测试按钮点击
  /// </summary>
  private void Btn_Test_Click(object sender, EventArgs e)
  {
    if (_currentConfig == null)
    {
      MessageBox.Show("请先选择要测试的配置", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
      return;
    }

    // 保存当前配置
    SavePanelToConfig();

    // 打开测试窗口
    using (var testForm = new Frm_TcpTest(_currentConfig))
    {
      testForm.ShowDialog(this);
    }
  }
}