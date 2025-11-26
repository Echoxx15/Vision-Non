using Logger;
using System;
using System.Windows.Forms;
using System.Reflection;
using static System.IO.Ports.SerialPort;

namespace LightControlNet.UI;

/// <summary>
/// 光源配置界面
/// 功能：添加/删除光源配置、编辑串口参数、测试连接、测试操作
/// </summary>
public partial class Frm_LightConfig : Form
{
    private LightConfig _currentConfig;

    public Frm_LightConfig()
    {
        InitializeComponent();
        InitializeForm();
    }

    private void InitializeForm()
    {
        listBox_Configs.SelectedIndexChanged += ListBox_Configs_SelectedIndexChanged;
        btn_Delete.Click += Btn_Delete_Click;
        btn_Save.Click += btn_Save_Click;
        btn_AddFgen.Click += btn_AddFgen_Click;
        btn_Connet.Click += btn_Connet_Click;
        cmb_PortName.SelectedIndexChanged += cmb_PortName_SelectedIndexChanged;

        btn_AddFgen.Text = "添加";
        PopulateAddMenuItems();

        InitializeComboBoxes();

        RefreshConfigList();
    }

    private void InitializeComboBoxes()
    {
        cmb_PortName.Items.Clear();
        cmb_PortName.Items.AddRange(GetPortNames());
        if (cmb_PortName.Items.Count > 0) cmb_PortName.SelectedIndex = 0;

        cmb_BaudRate.Items.Clear();
        cmb_BaudRate.Items.AddRange([4800, 9600, 19200, 38400, 57600, 115200]);
        cmb_BaudRate.SelectedItem = 9600;

        cmb_DataBits.Items.Clear();
        cmb_DataBits.Items.AddRange([5, 6, 7, 8]);
        cmb_DataBits.SelectedItem = 8;

        cmb_StopBits.Items.Clear();
        cmb_StopBits.Items.AddRange([1.0, 1.5, 2.0]);
        cmb_StopBits.SelectedItem = 1.0;

        cmb_Parity.Items.Clear();
        cmb_Parity.Items.AddRange(["None", "Odd", "Even", "Mark", "Space"]);
        cmb_Parity.SelectedItem = "None";

        cmb_ChannelCount.Items.Clear();
        cmb_ChannelCount.Items.AddRange([2, 4, 8]);
        cmb_ChannelCount.SelectedItem = 4;
    }

    private void ConfigureAnchors()
    {
        listBox_Configs.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
        panel_TestHost.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
        cmb_PortName.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        cmb_BaudRate.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        cmb_StopBits.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        cmb_DataBits.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        cmb_Parity.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        cmb_ChannelCount.Anchor = AnchorStyles.Top | AnchorStyles.Left;
    }

    private void ApplyResponsiveLayout()
    {
        var g = grp_Config.ClientSize;
        int margin = 10;
        int leftWidth = (int)(g.Width * 0.25);
        int rightWidth = (int)(g.Width * 0.40);
        int centerWidth = g.Width - leftWidth - rightWidth - margin * 4;

        listBox_Configs.Width = leftWidth - margin * 2;
        listBox_Configs.Left = margin;
        listBox_Configs.Top = margin;
        listBox_Configs.Height = g.Height - margin * 2;

        panel_Params.Width = centerWidth;
        panel_Params.Left = listBox_Configs.Right + margin;
        panel_Params.Top = margin;
        panel_Params.Height = g.Height - margin * 2;

        panel_TestHost.Width = rightWidth;
        panel_TestHost.Left = g.Width - rightWidth - margin;
        panel_TestHost.Top = margin;
        panel_TestHost.Height = g.Height - margin * 2;
    }

    private void RefreshConfigList()
    {
        var selectedName = _currentConfig?.Name;

        listBox_Configs.Items.Clear();
        foreach (var config in LightFactory.Instance.Configs.Configs)
        {
            var status = config.Enabled ? "✔" : "❌";
            var typeText = config.Type == LightControllerType.Fgen ? "孚根" : "奥普特";
            listBox_Configs.Items.Add($"{status} {config.Name} ({typeText})");
        }

        // 恢复选择
        if (!string.IsNullOrEmpty(selectedName))
        {
            var idx = LightFactory.Instance.Configs.Configs.FindIndex(c => c.Name == selectedName);
            if (idx >= 0 && idx < listBox_Configs.Items.Count)
            {
                listBox_Configs.SelectedIndex = idx;
                return;
            }
        }

        if (listBox_Configs.Items.Count > 0)
        {
            listBox_Configs.SelectedIndex = 0;
        }
        else
        {
            ClearConfigPanel();
        }
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
            if (LightFactory.Instance.RemoveConfig(_currentConfig.Name))
            {
                LogHelper.Info($"已删除光源配置[{_currentConfig.Name}]");
            }
            _currentConfig = null;
            RefreshConfigList();
        }
    }

    private void ListBox_Configs_SelectedIndexChanged(object sender, EventArgs e)
    {
        var idx = listBox_Configs.SelectedIndex;
        var list = LightFactory.Instance.Configs.Configs;

        if (idx < 0 || idx >= list.Count)
        {
            _currentConfig = null;
            ClearConfigPanel();
            return;
        }

        _currentConfig = list[idx];
        LoadConfigToPanel(_currentConfig);
        var controller = LightFactory.Instance.GetController(_currentConfig.Name);
        UpdateTestPanelEnabled(controller);
    }

    private void LoadConfigToPanel(LightConfig config)
    {
        if (config == null)
        {
            ClearConfigPanel();
            return;
        }

        chk_Enabled.Checked = config.Enabled;
        cmb_PortName.Text = config.PortName;
        cmb_BaudRate.SelectedItem = config.BaudRate;
        cmb_DataBits.SelectedItem = config.DataBits;
        cmb_StopBits.SelectedItem = config.StopBits;
        cmb_Parity.SelectedItem = config.Parity;
        cmb_ChannelCount.SelectedItem = config.ChannelCount;
        lbl_Type.Text = config.Type.ToString();
    }

    private void ClearConfigPanel()
    {
        chk_Enabled.Checked = true;
        cmb_PortName.SelectedIndex = cmb_PortName.Items.Count > 0 ? 0 : -1;
        cmb_BaudRate.SelectedItem = 9600;
        cmb_DataBits.SelectedItem = 8;
        cmb_StopBits.SelectedItem = 1.0;
        cmb_Parity.SelectedItem = "None";
        cmb_ChannelCount.SelectedItem = 4;
        lbl_Type.Text = "-";
    }

    private void SavePanelToConfig()
    {
        if (_currentConfig == null) return;

        // 将界面参数写回当前配置
        _currentConfig.Enabled = chk_Enabled.Checked;
        _currentConfig.PortName = cmb_PortName.Text;
        _currentConfig.BaudRate = (int)cmb_BaudRate.SelectedItem;
        _currentConfig.DataBits = (int)cmb_DataBits.SelectedItem;
        _currentConfig.StopBits = (double)cmb_StopBits.SelectedItem;
        _currentConfig.Parity = cmb_Parity.SelectedItem?.ToString() ?? "None";
        _currentConfig.ChannelCount = (int)cmb_ChannelCount.SelectedItem;

        //通过工厂更新与保存，并重建对应控制器
        LightFactory.Instance.UpdateConfig(_currentConfig);
        LogHelper.Info($"已保存光源配置[{_currentConfig.Name}]");

        RefreshConfigList();

        var controller = LightFactory.Instance.GetController(_currentConfig.Name);
        UpdateTestPanelEnabled(controller);
    }

    private void PopulateAddMenuItems()
    {
        contextMenuStrip1.Items.Clear();
        foreach (var t in LightPluginServer.Instance.GetLoadedPluginTypes())
        {
            var attr = t.GetCustomAttribute<LightManufacturerAttribute>();
            if (attr == null) continue;
            var lt = attr.Type;
            var item = new ToolStripMenuItem($"{lt} - {attr.ManufacturerName}");
            item.Click += (s, e) => AddByType(lt);
            contextMenuStrip1.Items.Add(item);
        }
    }

    private void AddByType(LightControllerType type)
    {
        var cfg = new LightConfig
        {
            Name = LightFactory.Instance.Configs.GenerateUniqueName(type),
            Type = type,
            Enabled = true,
            PortName = "COM1",
            BaudRate = 9600,
            DataBits = 8,
            StopBits = 1,
            Parity = "None",
            ChannelCount = 4
        };

        try
        {
            var ports = LightManager.Instance.EnumerateDevices(type);
            if (ports != null && ports.Count > 0)
            {
                cfg.PortName = ports[0];
            }
        }
        catch
        {
        }

        LightFactory.Instance.AddConfig(cfg);
        RefreshConfigList();

        var idx = LightFactory.Instance.Configs.Configs.FindIndex(c => c.Name == cfg.Name);
        if (idx >= 0) listBox_Configs.SelectedIndex = idx;
    }

    private void btn_Save_Click(object sender, EventArgs e)
    {
        SavePanelToConfig();
    }

    private void btn_AddFgen_Click(object sender, EventArgs e)
    {
        PopulateAddMenuItems();
        var pt = btn_AddFgen.PointToScreen(new System.Drawing.Point(0, btn_AddFgen.Height));
        contextMenuStrip1.Show(pt);
    }

    private Control _currentTestHost;

    private void UpdateTestPanelEnabled(ILightController controller)
    {
        panel_TestHost.Enabled = controller?.IsConnected ?? false;
    }

    private void btn_Connet_Click(object sender, EventArgs e)
    {
        if (_currentConfig == null)
        {
            MessageBox.Show("请先选择配置", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        var ok = LightFactory.Instance.ConnectController(_currentConfig.Name);
        var controller = LightFactory.Instance.GetController(_currentConfig.Name);
        UpdateTestPanelEnabled(controller);
        if (!ok)
        {
            MessageBox.Show("打开失败，请检查串口参数与硬件连接", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }

    private void cmb_PortName_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (_currentConfig == null) return;
        // 断开当前连接
        LightFactory.Instance.DisconnectController(_currentConfig.Name);
        var controller = LightFactory.Instance.GetController(_currentConfig.Name);
        UpdateTestPanelEnabled(controller);
        // 更新界面上的端口号，并提示需要点击“打开”
        _currentConfig.PortName = cmb_PortName.Text;
    }

    private void btn_Test_Click(object sender, EventArgs e)
    {
        if (_currentConfig == null)
        {
            MessageBox.Show("请先选择配置", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }
        var controller = LightFactory.Instance.GetController(_currentConfig.Name);
        if (controller == null || !controller.IsConnected)
        {
            MessageBox.Show("请先打开连接后再测试", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }
        using var frm = controller.TestForm;
        frm.ShowDialog(this);
    }
}
