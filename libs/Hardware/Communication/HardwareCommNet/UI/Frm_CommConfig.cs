using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace HardwareCommNet.UI;

/// <summary>
/// 通讯配置界面：用于管理和配置所有通讯设备
/// </summary>
public partial class Frm_CommConfig : UserControl
{
    private readonly Dictionary<string, uDeviceItem> _deviceItems = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, Control> _configControls = new(StringComparer.OrdinalIgnoreCase);
    private IComm _selectedDevice;
    private uDeviceItem _selectedItem;
    private uDeviceItem _rightClickedItem;

    /// <summary>
    /// 当前选中的设备
    /// </summary>
    public IComm SelectedDevice => _selectedDevice;

    /// <summary>
    /// 设备选中事件
    /// </summary>
    public event EventHandler<IComm> DeviceSelected;

    /// <summary>
    /// 请求打开通讯表配置事件
    /// </summary>
    public event EventHandler<IComm> OpenCommTableRequested;

    public Frm_CommConfig()
    {
        InitializeComponent();
        SetupEventHandlers();
        LoadCommTypesToMenu();
        LoadExistingDevices();
    }

    private void SetupEventHandlers()
    {
        // 订阅CommunicationFactory事件
        CommunicationFactory.Instance.DeviceCreated += OnDeviceCreated;
        CommunicationFactory.Instance.DeviceRemoved += OnDeviceRemoved;
        CommunicationFactory.Instance.DeviceRenamed += OnDeviceRenamed;

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
            // 查找对应的uDeviceItem
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
    /// 加载通讯类型到添加按钮的右键菜单
    /// </summary>
    private void LoadCommTypesToMenu()
    {
        contextMenu_Add.Items.Clear();
        var types = CommunicationFactory.Instance.GetAllManufacturers();
        foreach (var type in types)
        {
            var menuItem = new ToolStripMenuItem(type)
            {
                Tag = type
            };
            menuItem.Click += CommTypeMenuItem_Click;
            contextMenu_Add.Items.Add(menuItem);
        }
    }

    /// <summary>
    /// 通讯类型菜单项点击事件
    /// </summary>
    private void CommTypeMenuItem_Click(object sender, EventArgs e)
    {
        if (sender is ToolStripMenuItem menuItem && menuItem.Tag is string commType)
        {
            CreateDevice(commType);
        }
    }

    /// <summary>
    /// 创建设备
    /// </summary>
    private void CreateDevice(string commType, string customName = null)
    {
        var device = CommunicationFactory.Instance.CreateCommDevice(commType, customName);
        if (device != null)
        {
            Console.WriteLine($"创建通讯设备成功: {device.Name}");
        }
        else
        {
            MessageBox.Show("创建通讯设备失败", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    /// <summary>
    /// 加载已存在的设备
    /// </summary>
    private void LoadExistingDevices()
    {
        try
        {
            var devices = CommunicationFactory.Instance.GetAllDevices().ToList();
            foreach (var device in devices)
            {
                AddDeviceItem(device);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"加载已存在的通讯设备失败: {ex.Message}");
        }
    }

    #region 设备管理

    private void AddDeviceItem(IComm device)
    {
        if (device == null || _deviceItems.ContainsKey(device.Name))
            return;

        var item = new uDeviceItem(device)
        {
            Width = flowPanel_Devices.ClientSize.Width - 4,
            Height = 40
        };

        item.DeviceSelected += OnDeviceItemSelected;
        item.DeviceDoubleClicked += OnDeviceItemDoubleClicked;
        // 使用AntdUI Button的ContextMenuStrip绑定右键菜单
        item.BindContextMenu(contextMenu_Device);

        _deviceItems[device.Name] = item;
        flowPanel_Devices.Controls.Add(item);
    }

    private void RemoveDeviceItem(string deviceName)
    {
        if (_deviceItems.TryGetValue(deviceName, out var item))
        {
            flowPanel_Devices.Controls.Remove(item);
            item.DeviceSelected -= OnDeviceItemSelected;
            item.DeviceDoubleClicked -= OnDeviceItemDoubleClicked;
            item.Dispose();
            _deviceItems.Remove(deviceName);

            // 如果删除的是选中项，清除选中状态并隐藏配置界面
            if (_selectedDevice?.Name == deviceName)
            {
                _selectedDevice = null;
                _selectedItem = null;
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
    private void ShowDeviceConfig(IComm device)
    {
        if (device == null) return;

        // 隐藏提示标签
        lbl_ConfigTip.Visible = false;

        // 获取或创建配置控件
        if (!_configControls.TryGetValue(device.Name, out var configCtrl))
        {
            configCtrl = CreateConfigControl(device);
            if (configCtrl != null)
            {
                _configControls[device.Name] = configCtrl;
            }
        }

        if (configCtrl != null)
        {
            // 隐藏其他配置控件
            foreach (Control ctrl in panel_ConfigArea.Controls)
            {
                if (ctrl != lbl_ConfigTip)
                {
                    ctrl.Visible = false;
                }
            }

            // 如果控件还没添加到面板，则添加
            if (!panel_ConfigArea.Controls.Contains(configCtrl))
            {
                configCtrl.Dock = DockStyle.Fill;
                panel_ConfigArea.Controls.Add(configCtrl);
            }

            // 显示当前配置控件
            configCtrl.Visible = true;
            configCtrl.BringToFront();
        }
        else
        {
            ShowConfigTip();
        }
    }

    /// <summary>
    /// 创建设备配置控件
    /// </summary>
    private Control CreateConfigControl(IComm device)
    {
        if (device is CommAdapterBase adapter)
        {
            return adapter.GetConfigControl();
        }
        return null;
    }

    /// <summary>
    /// 显示提示标签
    /// </summary>
    private void ShowConfigTip()
    {
        foreach (Control ctrl in panel_ConfigArea.Controls)
        {
            if (ctrl != lbl_ConfigTip)
            {
                ctrl.Visible = false;
            }
        }
        lbl_ConfigTip.Visible = true;
    }

    #endregion

    #region 事件处理

    private void OnDeviceCreated(string name, string type, IComm device)
    {
        if (InvokeRequired)
        {
            BeginInvoke(new Action<string, string, IComm>(OnDeviceCreated), name, type, device);
            return;
        }

        AddDeviceItem(device);
    }

    private void OnDeviceRemoved(string name, string type)
    {
        if (InvokeRequired)
        {
            BeginInvoke(new Action<string, string>(OnDeviceRemoved), name, type);
            return;
        }

        RemoveDeviceItem(name);
    }

    private void OnDeviceRenamed(string oldName, string newName, string type)
    {
        if (InvokeRequired)
        {
            BeginInvoke(new Action<string, string, string>(OnDeviceRenamed), oldName, newName, type);
            return;
        }

        UpdateDeviceItemName(oldName, newName);
    }

    private void OnDeviceItemSelected(object sender, IComm device)
    {
        // 清除之前的选中状态
        if (_selectedItem != null)
        {
            _selectedItem.IsSelected = false;
        }

        // 设置新的选中状态
        _selectedDevice = device;
        _selectedItem = sender as uDeviceItem;
        if (_selectedItem != null)
        {
            _selectedItem.IsSelected = true;
        }

        // 显示配置界面
        ShowDeviceConfig(device);

        // 触发选中事件
        DeviceSelected?.Invoke(this, device);
    }

    private void OnDeviceItemDoubleClicked(object sender, IComm device)
    {
        // 双击打开通讯表配置
        OpenCommTableRequested?.Invoke(this, device);
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
        // 点击添加按钮显示通讯类型菜单
        contextMenu_Add.Show(btn_Add, new Point(0, -contextMenu_Add.Height));
    }

    private void btn_Remove_Click(object sender, EventArgs e)
    {
        if (_selectedDevice == null)
        {
            MessageBox.Show("请先选择要删除的设备", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        DeleteDevice(_selectedDevice);
    }

    private void DeleteDevice(IComm device)
    {
        if (device == null) return;

        var result = MessageBox.Show($"确定要删除设备 [{device.Name}] 吗？", "确认删除",
            MessageBoxButtons.YesNo, MessageBoxIcon.Question);

        if (result == DialogResult.Yes)
        {
            var deviceName = device.Name;
            if (CommunicationFactory.Instance.RemoveDevice(deviceName))
            {
                Console.WriteLine($"删除通讯设备成功: {deviceName}");
            }
        }
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

        inputForm.Controls.AddRange(new Control[] { label, textBox, btnOK, btnCancel });
        inputForm.AcceptButton = btnOK;
        inputForm.CancelButton = btnCancel;

        if (inputForm.ShowDialog() == DialogResult.OK)
        {
            var newName = textBox.Text.Trim();
            if (!string.IsNullOrEmpty(newName) && newName != device.Name)
            {
                if (CommunicationFactory.Instance.RenameDevice(device.Name, newName))
                {
                    Console.WriteLine($"重命名通讯设备成功: {device.Name} -> {newName}");
                }
                else
                {
                    MessageBox.Show("重命名失败，可能名称已存在", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }

    private void tsmi_Delete_Click(object sender, EventArgs e)
    {
        if (_rightClickedItem?.Device != null)
        {
            DeleteDevice(_rightClickedItem.Device);
        }
    }

    private void tsmi_CommTable_Click(object sender, EventArgs e)
    {
        if (_rightClickedItem?.Device != null)
        {
            OpenCommTableRequested?.Invoke(this, _rightClickedItem.Device);
        }
    }

    #endregion

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            // 取消订阅工厂事件
            CommunicationFactory.Instance.DeviceCreated -= OnDeviceCreated;
            CommunicationFactory.Instance.DeviceRemoved -= OnDeviceRemoved;
            CommunicationFactory.Instance.DeviceRenamed -= OnDeviceRenamed;

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
