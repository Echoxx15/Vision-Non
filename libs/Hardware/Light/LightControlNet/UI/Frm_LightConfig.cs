using Logger;
using System;
using System.IO.Ports;
using System.Windows.Forms;

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
        tsm_Fugen.Click += tsm_Fugen_Click;

        InitializeComboBoxes();

        RefreshConfigList();
    }

    private void InitializeComboBoxes()
    {
        cmb_PortName.Items.Clear();
        cmb_PortName.Items.AddRange(SerialPort.GetPortNames());
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
    }

    private void tsm_Fugen_Click(object sender, EventArgs e)
    {
        // 新建一个孚根配置并交给工厂管理
        var cfg = new LightConfig
        {
            Name = LightFactory.Instance.Configs.GenerateUniqueName(LightControllerType.Fgen),
            Type = LightControllerType.Fgen,
            Enabled = true,
            PortName = "COM1",
            BaudRate = 9600,
            DataBits = 8,
            StopBits = 1,
            Parity = "None",
            ChannelCount = 4
        };

        LightFactory.Instance.AddConfig(cfg);
        RefreshConfigList();

        //选中新添加项
        var idx = LightFactory.Instance.Configs.Configs.FindIndex(c => c.Name == cfg.Name);
        if (idx >= 0) listBox_Configs.SelectedIndex = idx;
    }

    private void btn_Save_Click(object sender, EventArgs e)
    {
        SavePanelToConfig();
    }

    private void btn_AddFgen_Click(object sender, EventArgs e)
    {
        // 与右键菜单相同：添加一个孚根控制器配置
        tsm_Fugen_Click(sender, e);
    }
}
