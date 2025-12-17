using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using Logger;
using static System.IO.Ports.SerialPort;

namespace LightControlNet.UI;

/// <summary>
/// 光源配置界面：用于管理和配置所有光源控制器
/// </summary>
public partial class uLightConfig : UserControl
{
    private readonly Dictionary<string, uLightDeviceItem> _deviceItems = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, Control> _configControls = new(StringComparer.OrdinalIgnoreCase);
    private ILightController _selectedDevice;
    private uLightDeviceItem _selectedItem;
    private uLightDeviceItem _rightClickedItem;
    private LightConfig _currentConfig;
    private bool _isLoadingConfig;

    /// <summary>
    /// 当前选中的设备
    /// </summary>
    public ILightController SelectedDevice => _selectedDevice;

    /// <summary>
    /// 设备选中事件
    /// </summary>
    public event EventHandler<ILightController> DeviceSelected;

    /// <summary>
    /// 请求打开测试界面事件
    /// </summary>
    public event EventHandler<ILightController> OpenTestFormRequested;

    public uLightConfig()
    {
        InitializeComponent();
        SetupEventHandlers();
        LoadCommTypesToMenu();
        InitializeComboBoxes();
        LoadExistingDevices();
    }

    private void SetupEventHandlers()
    {
        // 订阅LightFactory事件
        LightFactory.Instance.DeviceCreated += OnDeviceCreated;
        LightFactory.Instance.DeviceRemoved += OnDeviceRemoved;
        LightFactory.Instance.DeviceRenamed += OnDeviceRenamed;

        // FlowLayoutPanel大小变化时调整设备项宽度
        flowPanel_Devices.SizeChanged += FlowPanel_Devices_SizeChanged;

        // 右键菜单打开时获取当前点击的设备项
        contextMenu_Device.Opening += ContextMenu_Device_Opening;
    }

    private void ContextMenu_Device_Opening(object sender, System.ComponentModel.CancelEventArgs e)
    {
        // 通过SourceControl获取当前右键点击的控件
        if (contextMenu_Device.SourceControl is AntdUI.Button btn)
        {
            // 查找对应的uLightDeviceItem
            var deviceItem = _deviceItems.Values.FirstOrDefault(item => item.DeviceButton == btn);
            if (deviceItem != null)
            {
                _rightClickedItem = deviceItem;
                return;
            }
        }
        
        // 如果无法找到对应的设备项，取消显示菜单
        e.Cancel = true;
    }

    /// <summary>
    /// 加载光源控制器类型到添加按钮的右键菜单
    /// </summary>
    private void LoadCommTypesToMenu()
    {
        contextMenu_Add.Items.Clear();
        foreach (var t in LightPluginServer.Instance.GetLoadedPluginTypes())
        {
            var attr = t.GetCustomAttribute<LightManufacturerAttribute>();
            if (attr == null) continue;
            var lt = attr.Type;
            var item = new ToolStripMenuItem($"{attr.ManufacturerName}");
            item.Tag = lt;
            item.Click += CommTypeMenuItem_Click;
            contextMenu_Add.Items.Add(item);
        }
    }

    /// <summary>
    /// 光源类型菜单项点击事件
    /// </summary>
    private void CommTypeMenuItem_Click(object sender, EventArgs e)
    {
        if (sender is ToolStripMenuItem menuItem && menuItem.Tag is LightControllerType type)
        {
            CreateDevice(type);
        }
    }

    /// <summary>
    /// 创建设备
    /// </summary>
    private void CreateDevice(LightControllerType type, string customName = null)
    {
        // 获取已使用的端口列表
        var usedPorts = LightFactory.Instance.Configs.Configs
            .Select(c => c.PortName)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
        
        // 获取可用端口列表
        var availablePorts = GetPortNames();
        
        // 选择第一个未被使用的端口
        string selectedPort = "COM1";
        foreach (var port in availablePorts)
        {
            if (!usedPorts.Contains(port))
            {
                selectedPort = port;
                break;
            }
        }
        
        var cfg = new LightConfig
        {
            Name = customName ?? LightFactory.Instance.Configs.GenerateUniqueName(type),
            Type = type,
            Enabled = true,
            PortName = selectedPort,
            BaudRate = 9600,
            DataBits = 8,
            StopBits = 1,
            Parity = "None",
            ChannelCount = 4
        };

        var config = LightFactory.Instance.AddConfig(cfg);
        if (config != null)
        {
            Console.WriteLine($"创建光源设备成功: {config.Name}");
        }
        else
        {
            MessageBox.Show("创建光源设备失败", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    /// <summary>
    /// 加载已存在的设备
    /// </summary>
    private void LoadExistingDevices()
    {
        try
        {
            // 加载配置中的所有设备
            foreach (var config in LightFactory.Instance.Configs.Configs)
            {
                var controller = LightFactory.Instance.GetController(config.Name);
                if (controller != null)
                {
                    AddDeviceItem(controller, config);
                }
                else
                {
                    // 配置存在但控制器未创建（可能是disabled或创建失败），仍然显示在列表中
                    AddDeviceItemFromConfig(config);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"加载已存在的光源设备失败: {ex.Message}");
        }
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

    #region 设备管理

    private void AddDeviceItem(ILightController device, LightConfig config)
    {
        if (device == null || _deviceItems.ContainsKey(device.Name))
            return;

        var item = new uLightDeviceItem(device)
        {
            Width = flowPanel_Devices.ClientSize.Width - 4,
            Height = 40
        };

        item.DeviceSelected += OnDeviceItemSelected;
        item.DeviceDoubleClicked += OnDeviceItemDoubleClicked;
        item.ConnectionStateChanged += OnDeviceConnectionStateChanged;
        // 使用AntdUI Button的ContextMenuStrip绑定右键菜单
        item.BindContextMenu(contextMenu_Device);

        _deviceItems[device.Name] = item;
        flowPanel_Devices.Controls.Add(item);
    }

    private void AddDeviceItemFromConfig(LightConfig config)
    {
        if (config == null || _deviceItems.ContainsKey(config.Name))
            return;

        // 尝试创建控制器
        var controller = LightFactory.Instance.GetController(config.Name);
        if (controller != null)
        {
            AddDeviceItem(controller, config);
            return;
        }

        // 如果控制器不存在，创建一个空的设备项
        var item = new uLightDeviceItem()
        {
            Width = flowPanel_Devices.ClientSize.Width - 4,
            Height = 40
        };

        item.DeviceSelected += OnDeviceItemSelected;
        item.DeviceDoubleClicked += OnDeviceItemDoubleClicked;
        item.BindContextMenu(contextMenu_Device);

        _deviceItems[config.Name] = item;
        flowPanel_Devices.Controls.Add(item);
    }

    private void RemoveDeviceItem(string deviceName)
    {
        if (_deviceItems.TryGetValue(deviceName, out var item))
        {
            flowPanel_Devices.Controls.Remove(item);
            item.DeviceSelected -= OnDeviceItemSelected;
            item.DeviceDoubleClicked -= OnDeviceItemDoubleClicked;
            item.ConnectionStateChanged -= OnDeviceConnectionStateChanged;
            item.Dispose();
            _deviceItems.Remove(deviceName);

            // 如果删除的是选中项，清除选中状态并隐藏配置界面
            if (_selectedDevice?.Name == deviceName)
            {
                _selectedDevice = null;
                _selectedItem = null;
                _currentConfig = null;
                ShowConfigTip();
            }

            // 清理配置控件缓存
            if (_configControls.TryGetValue(deviceName, out var configCtrl))
            {
                configCtrl.Dispose();
                _configControls.Remove(deviceName);
            }
        }
    }

    private void UpdateDeviceItemName(string oldName, string newName)
    {
        if (_deviceItems.TryGetValue(oldName, out var item))
        {
            item.UpdateDeviceName(newName);
            _deviceItems.Remove(oldName);
            _deviceItems[newName] = item;

            // 更新配置控件缓存的键名
            if (_configControls.TryGetValue(oldName, out var configCtrl))
            {
                _configControls.Remove(oldName);
                _configControls[newName] = configCtrl;
            }
        }
    }

    #endregion

    #region 配置界面管理

    /// <summary>
    /// 显示设备配置界面
    /// </summary>
    private void ShowDeviceConfig(ILightController device)
    {
        if (device == null)
        {
            ShowConfigTip();
            return;
        }

        // 获取配置
        _currentConfig = LightFactory.Instance.GetConfig(device.Name);
        if (_currentConfig == null)
        {
            ShowConfigTip();
            return;
        }

        // 隐藏提示标签
        lbl_ConfigTip.Visible = false;
        panel_Params.Visible = true;

        // 加载配置到面板
        LoadConfigToPanel(_currentConfig);
    }

    private void LoadConfigToPanel(LightConfig config)
    {
        if (config == null)
        {
            ClearConfigPanel();
            return;
        }

        _isLoadingConfig = true;
        try
        {
            lbl_Type.Text = config.Type.ToString();
            chk_Enabled.Checked = config.Enabled;
            cmb_PortName.Text = config.PortName;
            cmb_BaudRate.SelectedItem = config.BaudRate;
            cmb_DataBits.SelectedItem = config.DataBits;
            cmb_StopBits.SelectedItem = config.StopBits;
            cmb_Parity.SelectedItem = config.Parity;
            cmb_ChannelCount.SelectedItem = config.ChannelCount;
            
            // 根据连接状态禁用/启用配置控件
            var controller = LightFactory.Instance.GetController(config.Name);
            bool isConnected = controller?.IsConnected ?? false;
            SetConfigControlsEnabled(!isConnected);
        }
        finally
        {
            _isLoadingConfig = false;
        }
    }
    
    /// <summary>
    /// 设置配置控件的启用/禁用状态
    /// </summary>
    private void SetConfigControlsEnabled(bool enabled)
    {
        cmb_PortName.Enabled = enabled;
        cmb_BaudRate.Enabled = enabled;
        cmb_DataBits.Enabled = enabled;
        cmb_StopBits.Enabled = enabled;
        cmb_Parity.Enabled = enabled;
        cmb_ChannelCount.Enabled = enabled;
        chk_Enabled.Enabled = enabled;
    }

    private void ClearConfigPanel()
    {
        lbl_Type.Text = "-";
        chk_Enabled.Checked = true;
        cmb_PortName.SelectedIndex = cmb_PortName.Items.Count > 0 ? 0 : -1;
        cmb_BaudRate.SelectedItem = 9600;
        cmb_DataBits.SelectedItem = 8;
        cmb_StopBits.SelectedItem = 1.0;
        cmb_Parity.SelectedItem = "None";
        cmb_ChannelCount.SelectedItem = 4;
    }

    /// <summary>
    /// 显示提示标签
    /// </summary>
    private void ShowConfigTip()
    {
        panel_Params.Visible = false;
        lbl_ConfigTip.Visible = true;
    }

    #endregion

    #region 事件处理

    private void OnDeviceCreated(string name, LightControllerType type, ILightController device)
    {
        if (InvokeRequired)
        {
            BeginInvoke(new Action<string, LightControllerType, ILightController>(OnDeviceCreated), name, type, device);
            return;
        }

        var config = LightFactory.Instance.GetConfig(name);
        AddDeviceItem(device, config);
    }

    private void OnDeviceRemoved(string name, LightControllerType type)
    {
        if (InvokeRequired)
        {
            BeginInvoke(new Action<string, LightControllerType>(OnDeviceRemoved), name, type);
            return;
        }

        RemoveDeviceItem(name);
    }

    private void OnDeviceRenamed(string oldName, string newName, LightControllerType type)
    {
        if (InvokeRequired)
        {
            BeginInvoke(new Action<string, string, LightControllerType>(OnDeviceRenamed), oldName, newName, type);
            return;
        }

        UpdateDeviceItemName(oldName, newName);
    }

    private void OnDeviceItemSelected(object sender, ILightController device)
    {
        // 清除之前的选中状态
        if (_selectedItem != null)
        {
            _selectedItem.IsSelected = false;
        }

        // 设置新的选中状态
        _selectedDevice = device;
        _selectedItem = sender as uLightDeviceItem;
        if (_selectedItem != null)
        {
            _selectedItem.IsSelected = true;
        }

        // 显示配置界面
        ShowDeviceConfig(device);

        // 触发选中事件
        DeviceSelected?.Invoke(this, device);
    }

    private void OnDeviceItemDoubleClicked(object sender, ILightController device)
    {
        // 双击打开测试界面
        if (device != null && device.IsConnected)
        {
            OpenTestFormRequested?.Invoke(this, device);
            using var frm = device.TestForm;
            frm?.ShowDialog(this.FindForm());
        }
        else
        {
            MessageBox.Show("请先连接设备后再测试", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }

    private void OnDeviceConnectionStateChanged(object sender, bool connected)
    {
        if (InvokeRequired)
        {
            try { BeginInvoke(new Action<object, bool>(OnDeviceConnectionStateChanged), sender, connected); } catch { }
            return;
        }

        // 获取触发事件的设备项
        var deviceItem = sender as uLightDeviceItem;
        if (deviceItem?.Device == null) return;

        // 如果是当前选中的设备，更新配置控件的启用状态
        if (_selectedDevice != null && _selectedDevice.Name == deviceItem.Device.Name)
        {
            SetConfigControlsEnabled(!connected);
        }
    }

    private void FlowPanel_Devices_SizeChanged(object sender, EventArgs e)
    {
        // 调整所有设备项宽度
        foreach (Control ctrl in flowPanel_Devices.Controls)
        {
            ctrl.Width = flowPanel_Devices.ClientSize.Width - 4;
        }
    }

    private void btn_Add_Click(object sender, EventArgs e)
    {
        // 点击添加按钮显示光源类型菜单
        contextMenu_Add.Show(btn_Add, new Point(0, -contextMenu_Add.Height));
    }

    private void btn_Remove_Click(object sender, EventArgs e)
    {
        if (_selectedDevice == null && _currentConfig == null)
        {
            MessageBox.Show("请先选择要删除的设备", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        DeleteDevice(_currentConfig?.Name ?? _selectedDevice?.Name);
    }

    private void DeleteDevice(string deviceName)
    {
        if (string.IsNullOrEmpty(deviceName)) return;

        var result = MessageBox.Show($"确定要删除设备 [{deviceName}] 吗？", "确认删除",
            MessageBoxButtons.YesNo, MessageBoxIcon.Question);

        if (result == DialogResult.Yes)
        {
            if (LightFactory.Instance.RemoveConfig(deviceName))
            {
                Console.WriteLine($"删除光源设备成功: {deviceName}");
            }
        }
    }

    private void btn_Save_Click(object sender, EventArgs e)
    {
        // 保存按钮已禁用，配置修改即生效
    }

    /// <summary>
    /// 保存当前配置到文件（内部调用）
    /// </summary>
    private void SaveCurrentConfig()
    {
        if (_currentConfig == null) return;
        
        // 通过工厂更新与保存
        LightFactory.Instance.UpdateConfig(_currentConfig);
        LogHelper.Info($"已保存光源配置[{_currentConfig.Name}]");
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

        // 通过工厂更新与保存
        LightFactory.Instance.UpdateConfig(_currentConfig);
        LogHelper.Info($"已保存光源配置[{_currentConfig.Name}]");
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
        frm?.ShowDialog(this.FindForm());
    }

    private void cmb_PortName_SelectedIndexChanged(object sender, EventArgs e)
    {
        // 加载配置时不触发保存
        if (_isLoadingConfig || _currentConfig == null) return;
        
        // 更新配置并保存
        _currentConfig.PortName = cmb_PortName.Text;
        SaveCurrentConfig();
    }

    private void cmb_BaudRate_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (_isLoadingConfig || _currentConfig == null) return;
        _currentConfig.BaudRate = (int)cmb_BaudRate.SelectedItem;
        SaveCurrentConfig();
    }

    private void cmb_DataBits_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (_isLoadingConfig || _currentConfig == null) return;
        _currentConfig.DataBits = (int)cmb_DataBits.SelectedItem;
        SaveCurrentConfig();
    }

    private void cmb_StopBits_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (_isLoadingConfig || _currentConfig == null) return;
        _currentConfig.StopBits = (double)cmb_StopBits.SelectedItem;
        SaveCurrentConfig();
    }

    private void cmb_Parity_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (_isLoadingConfig || _currentConfig == null) return;
        _currentConfig.Parity = cmb_Parity.SelectedItem?.ToString() ?? "None";
        SaveCurrentConfig();
    }

    private void cmb_ChannelCount_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (_isLoadingConfig || _currentConfig == null) return;
        _currentConfig.ChannelCount = (int)cmb_ChannelCount.SelectedItem;
        SaveCurrentConfig();
    }

    private void chk_Enabled_CheckedChanged(object sender, EventArgs e)
    {
        if (_isLoadingConfig || _currentConfig == null) return;
        _currentConfig.Enabled = chk_Enabled.Checked;
        SaveCurrentConfig();
    }

    private void tsmi_Rename_Click(object sender, EventArgs e)
    {
        if (_rightClickedItem?.Device == null) return;

        var device = _rightClickedItem.Device;
        var inputForm = new Form
        {
            Text = "重命名设备",
            Size = new Size(400, 180),
            StartPosition = FormStartPosition.CenterParent,
            FormBorderStyle = FormBorderStyle.FixedDialog,
            MaximizeBox = false,
            MinimizeBox = false
        };

        var label = new Label
        {
            Text = "新名称:",
            Location = new Point(20, 30),
            AutoSize = true
        };

        var textBox = new TextBox
        {
            Text = device.Name,
            Location = new Point(90, 27),
            Width = 270
        };

        var btnOK = new Button
        {
            Text = "确定",
            DialogResult = DialogResult.OK,
            Location = new Point(190, 90),
            Width = 80
        };

        var btnCancel = new Button
        {
            Text = "取消",
            DialogResult = DialogResult.Cancel,
            Location = new Point(280, 90),
            Width = 80
        };

        inputForm.Controls.AddRange([label, textBox, btnOK, btnCancel]);
        inputForm.AcceptButton = btnOK;
        inputForm.CancelButton = btnCancel;

        if (inputForm.ShowDialog() == DialogResult.OK)
        {
            var newName = textBox.Text.Trim();
            if (!string.IsNullOrEmpty(newName) && newName != device.Name)
            {
                if (LightFactory.Instance.RenameConfig(device.Name, newName))
                {
                    Console.WriteLine($"重命名光源设备成功: {device.Name} -> {newName}");
                }
                else
                {
                    MessageBox.Show("重命名失败，可能名称已存在", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }

    private void tsmi_Test_Click(object sender, EventArgs e)
    {
        if (_rightClickedItem?.Device != null && _rightClickedItem.Device.IsConnected)
        {
            using var frm = _rightClickedItem.Device.TestForm;
            frm?.ShowDialog(this.FindForm());
        }
        else
        {
            MessageBox.Show("请先连接设备后再测试", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }

    #endregion

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            // 取消订阅工厂事件
            LightFactory.Instance.DeviceCreated -= OnDeviceCreated;
            LightFactory.Instance.DeviceRemoved -= OnDeviceRemoved;
            LightFactory.Instance.DeviceRenamed -= OnDeviceRenamed;

            // 清理配置控件缓存
            foreach (var ctrl in _configControls.Values)
            {
                ctrl?.Dispose();
            }
            _configControls.Clear();

            if (components != null)
            {
                components.Dispose();
            }
        }
        base.Dispose(disposing);
    }
}
