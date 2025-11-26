using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using HardwareCommNet;

namespace Vision.Frm.Modbus;

public partial class Frm_Comm : Form
{
    private IComm _selectedDevice;
    private UserControl _currentConfigControl;
    private BindingList<CommDeviceModel> _deviceList;

    // 用于存储编辑前的原始名称
    private string _editingOriginalName;

    public Frm_Comm()
    {
        InitializeComponent();
        InitializeDataGridView();
        InitializeContextMenu();
        InitializeEvents();

        // 加载已保存的配置
        LoadExistingDevices();
    }

    private void InitializeDataGridView()
    {
        _deviceList = new BindingList<CommDeviceModel>();
        dgv_Devices.AutoGenerateColumns = false;
        dgv_Devices.DataSource = _deviceList;
        col_DeviceName.ReadOnly = false;
        col_DeviceType.ReadOnly = true;
        col_DeviceStatus.ReadOnly = true;
        dgv_Devices.RowTemplate.Height = 32;
    }

    private void InitializeContextMenu()
    {
        contextMenuStrip1.Items.Clear();
        var manufacturers = CommunicationFactory.Instance.GetAllManufacturers();
        foreach (var manu in manufacturers)
        {
            var menuItem = new ToolStripMenuItem($"添加 {manu}") { Tag = manu };
            menuItem.Click += MenuItem_AddDevice_Click;
            contextMenuStrip1.Items.Add(menuItem);
        }

        btn_Add.ContextMenuStrip = contextMenuStrip1;
    }

    private void InitializeEvents()
    {
        btn_Add.Click += (_, _) => contextMenuStrip1.Show(btn_Add, 0, btn_Add.Height);
        btn_Remove.Click += BtnRemove_Click;
        btn_Save.Click += BtnSave_Click;
        this.FormClosing += Frm_Comm_FormClosing;
        dgv_Devices.SelectionChanged += DgvDevices_SelectionChanged;
        dgv_Devices.CellBeginEdit += DgvDevices_CellBeginEdit;
        dgv_Devices.CellEndEdit += DgvDevices_CellEndEdit;
    }

    /// <summary>
    /// 添加设备（菜单项）
    /// </summary>
    private void MenuItem_AddDevice_Click(object sender, EventArgs e)
    {
        if (sender is not ToolStripMenuItem mi || mi.Tag is not string manufacturerName) return;
        try
        {
            var uniqueName = GenerateUniqueName(manufacturerName);
            var device = CommunicationFactory.Instance.CreateCommDevice(manufacturerName, uniqueName);
            if (device == null) return;

            device.ConnectionStatusChanged += Device_ConnectionStatusChanged;

            var model = new CommDeviceModel
            {
                Name = uniqueName,
                Type = manufacturerName,
                Status = device.IsConnected ? "已连接" : "未连接",
                Device = device
            };
            _deviceList.Add(model);

            // 不再手动添加 StateItem，由 CommunicationFactory 事件触发 Frm_HardwareState 添加

            if (dgv_Devices.Rows.Count > 0)
            {
                dgv_Devices.ClearSelection();
                dgv_Devices.Rows[dgv_Devices.Rows.Count - 1].Selected = true;
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"添加设备失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    /// <summary>
    /// 生成唯一名称（特性名 + 最小未占用序号，从0开始，如：ModbusTcp0）
    /// </summary>
    private string GenerateUniqueName(string baseName)
    {
        var usedIndexes = _deviceList
            .Where(d => d.Type.Equals(baseName, StringComparison.OrdinalIgnoreCase))
            .Select(d => d.Name)
            .Select(n => n.StartsWith(baseName, StringComparison.OrdinalIgnoreCase) ? n.Substring(baseName.Length) : "")
            .Select(s => int.TryParse(s, out var i) ? i : (int?)null)
            .Where(i => i.HasValue)
            .Select(i => i.Value)
            .ToHashSet();

        int idx = 0;
        while (usedIndexes.Contains(idx)) idx++;
        return $"{baseName}{idx}";
    }

    private bool IsNameUnique(string name, CommDeviceModel current)
    {
        return !_deviceList.Any(d =>
            !ReferenceEquals(d, current) && string.Equals(d.Name, name, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// 单元格开始编辑：记录原始名称
    /// </summary>
    private void DgvDevices_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
    {
        if (e.RowIndex < 0 || e.ColumnIndex != col_DeviceName.Index) return;
        var row = dgv_Devices.Rows[e.RowIndex];
        if (row.DataBoundItem is CommDeviceModel model)
        {
            _editingOriginalName = model.Name;
        }
    }

    /// <summary>
    /// 单元格编辑完成：验证并重命名
    /// </summary>
    private void DgvDevices_CellEndEdit(object sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex < 0 || e.ColumnIndex != col_DeviceName.Index) return;
        var row = dgv_Devices.Rows[e.RowIndex];
        if (row.DataBoundItem is not CommDeviceModel model) return;

        var newName = row.Cells[e.ColumnIndex].Value?.ToString().Trim() ?? string.Empty;
        var oldName = _editingOriginalName ?? model.Name;

        // 如果名称没有改变，直接返回
        if (string.Equals(oldName, newName, StringComparison.OrdinalIgnoreCase))
        {
            _editingOriginalName = null;
            return;
        }

        // 验证新名称
        if (string.IsNullOrEmpty(newName) || !IsNameUnique(newName, model))
        {
            MessageBox.Show(
                string.IsNullOrEmpty(newName) ? "设备名称不能为空" : "设备名称已存在，请更换",
                "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);

            // 恢复原名称到单元格
            row.Cells[e.ColumnIndex].Value = oldName;
            _editingOriginalName = null;
            return;
        }

        // 尝试在工厂层重命名
        if (!CommunicationFactory.Instance.RenameDevice(oldName, newName))
        {
            MessageBox.Show("重命名失败：新名称可能已存在", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            // 恢复原名称到单元格
            row.Cells[e.ColumnIndex].Value = oldName;
            _editingOriginalName = null;
            return;
        }

        // 更新模型
        model.Name = newName;

        // 不再手动更新 StateItem，由 CommunicationFactory 事件触发 Frm_HardwareState 更新

        _editingOriginalName = null;
    }

    private void DgvDevices_SelectionChanged(object sender, EventArgs e)
    {
        if (dgv_Devices.SelectedRows.Count > 0)
        {
            var model = dgv_Devices.SelectedRows[0].DataBoundItem as CommDeviceModel;
            _selectedDevice = model?.Device;
            if (_selectedDevice != null) ShowConfigControl(_selectedDevice);
        }
        else
        {
            _selectedDevice = null;
            ClearConfigControl();
        }
    }

    private void ShowConfigControl(IComm device)
    {
        try
        {
            ClearConfigControl();
            _currentConfigControl = device.GetConfigControl();
            if (_currentConfigControl != null)
            {
                _currentConfigControl.Dock = DockStyle.Fill;
                panel2.Controls.Add(_currentConfigControl);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"加载配置控件失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void ClearConfigControl()
    {
        if (_currentConfigControl != null)
        {
            panel2.Controls.Remove(_currentConfigControl);
            _currentConfigControl.Dispose();
            _currentConfigControl = null;
        }
    }

    private void BtnRemove_Click(object sender, EventArgs e)
    {
        if (dgv_Devices.SelectedRows.Count == 0)
        {
            MessageBox.Show("请先选择要删除的设备", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        if (MessageBox.Show("确定要删除选中的设备吗？", "确认", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            return;

        var model = dgv_Devices.SelectedRows[0].DataBoundItem as CommDeviceModel;
        if (model == null) return;

        try
        {
            // 退订事件
            try
            {
                model.Device.ConnectionStatusChanged -= Device_ConnectionStatusChanged;
            }
            catch
            {
            }

            // 从工厂移除设备（会自动触发事件移除 StateItem）
            CommunicationFactory.Instance.RemoveDevice(model.Name);

            // 清除配置控件
            if (_selectedDevice == model.Device)
            {
                ClearConfigControl();
                _selectedDevice = null;
            }

            // 从列表移除
            _deviceList.Remove(model);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"删除失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void BtnSave_Click(object sender, EventArgs e)
    {
        try
        {
// 刷新所有设备的配置
            foreach (var model in _deviceList)
            {
                if (model.Device is IConfigurableComm configurableDevice)
                {
                    // 从配置控件获取最新配置并应用
                    var config = configurableDevice.GetConfig();
                    configurableDevice.ApplyConfig(config);
                }
            }

            // 保存到本地
            CommunicationFactory.Instance.SaveConfigs();

            MessageBox.Show("配置保存成功", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"保存配置失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    /// <summary>
    /// 加载已存在的设备配置
    /// </summary>
    private void LoadExistingDevices()
    {
        try
        {
            // 从工厂加载配置（会自动触发 DeviceCreated 事件添加 StateItem）
            CommunicationFactory.Instance.LoadConfigs();

// 获取所有已加载的设备
            var devices = CommunicationFactory.Instance.GetAllDevices().ToList();

            foreach (var device in devices)
            {
                try
                {
                    // 订阅状态事件
                    device.ConnectionStatusChanged += Device_ConnectionStatusChanged;

                    // 获取设备类型
                    var deviceType = GetDeviceType(device);

// 创建模型并添加到列表
                    var model = new CommDeviceModel
                    {
                        Name = device.Name,
                        Type = deviceType,
                        Status = device.IsConnected ? "已连接" : "未连接",
                        Device = device
                    };
                    _deviceList.Add(model);

                    // 不再手动添加 StateItem，由 LoadConfigs 触发的事件自动添加
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"加载设备 {device?.Name} 失败: {ex.Message}");
                }
            }

            if (devices.Count > 0)
            {
                Console.WriteLine($"已加载 {devices.Count} 个通讯设备");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"加载设备配置失败: {ex.Message}");
        }
    }

    /// <summary>
    /// 获取设备的类型名称
    /// </summary>
    private string GetDeviceType(IComm device)
    {
        if (device == null) return string.Empty;

        var type = device.GetType();
        var attr = type.GetCustomAttribute<CommManufacturerAttribute>();
        return attr?.ManufacturerName ?? type.Name;
    }

    /// <summary>
    ///关闭窗体：退订事件
    /// </summary>
    private void Frm_Comm_FormClosing(object sender, FormClosingEventArgs e)
    {
        foreach (var model in _deviceList.ToList())
        {
            try
            {
                model.Device.ConnectionStatusChanged -= Device_ConnectionStatusChanged;
            }
            catch
            {
            }
        }
    }

    /// <summary>
    ///连接状态变更事件（事件驱动UI刷新）
    /// </summary>
    private void Device_ConnectionStatusChanged(object sender, bool isConnected)
    {
        if (InvokeRequired)
        {
            BeginInvoke(new Action(() => Device_ConnectionStatusChanged(sender, isConnected)));
            return;
        }

        if (sender is IComm device)
        {
            var model = _deviceList.FirstOrDefault(m => m.Device == device);
            if (model != null)
            {
                model.Status = isConnected ? "已连接" : "未连接";
                var idx = _deviceList.IndexOf(model);
                if (idx >= 0 && idx < dgv_Devices.Rows.Count)
                    dgv_Devices.InvalidateRow(idx);

                // StateItem 已通过订阅设备事件自动更新，无需手动同步
            }
        }
    }
}

