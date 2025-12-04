using System;
using System.Drawing;
using System.Windows.Forms;
using AntdUI;

namespace LightControlNet.UI;

/// <summary>
/// 光源设备列表项控件：用于FlowLayoutPanel中显示设备状态和连接开关
/// </summary>
public partial class uLightDeviceItem : UserControl
{
    private ILightController _device;
    private bool _isUpdating;
    private bool _isSelected;

    /// <summary>
    /// 设备实例
    /// </summary>
    public ILightController Device => _device;

    /// <summary>
    /// 设备名称
    /// </summary>
    public string DeviceName => _device?.Name ?? string.Empty;

    /// <summary>
    /// 是否选中
    /// </summary>
    public bool IsSelected
    {
        get => _isSelected;
        set
        {
            _isSelected = value;
            UpdateSelectionStyle();
        }
    }

    /// <summary>
    /// 设备按钮，可用于绑定右键菜单
    /// </summary>
    public AntdUI.Button DeviceButton => btn_DevName;

    /// <summary>
    /// 设备选中事件（点击设备项时触发）
    /// </summary>
    public event EventHandler<ILightController> DeviceSelected;

    /// <summary>
    /// 设备双击事件（双击设备项时触发，用于打开测试界面等操作）
    /// </summary>
    public event EventHandler<ILightController> DeviceDoubleClicked;

    /// <summary>
    /// 连接状态改变事件
    /// </summary>
    public event EventHandler<bool> ConnectionStateChanged;

    public uLightDeviceItem()
    {
        InitializeComponent();
        SetupEvents();
    }

    public uLightDeviceItem(ILightController device) : this()
    {
        BindDevice(device);
    }

    /// <summary>
    /// 绑定设备
    /// </summary>
    public void BindDevice(ILightController device)
    {
        // 解绑旧设备
        if (_device != null)
        {
            _device.ConnectionStatusChanged -= OnDeviceConnectionChanged;
        }

        _device = device;

        if (_device != null)
        {
            // 更新显示 - 只显示设备名
            btn_DevName.Text = _device.Name;
            UpdateConnectionState(_device.IsConnected);

            // 订阅连接状态变化事件
            _device.ConnectionStatusChanged += OnDeviceConnectionChanged;
        }
    }

    /// <summary>
    /// 更新设备名称显示
    /// </summary>
    public void UpdateDeviceName(string newName)
    {
        btn_DevName.Text = newName;
    }

    private void SetupEvents()
    {
        // 按钮左键点击选中设备
        btn_DevName.Click += (s, e) => DeviceSelected?.Invoke(this, _device);
        
        // 按钮双击打开测试界面
        btn_DevName.DoubleClick += (s, e) => DeviceDoubleClicked?.Invoke(this, _device);
        
        // 控件本身的点击事件
        this.Click += (s, e) => DeviceSelected?.Invoke(this, _device);
        this.DoubleClick += (s, e) => DeviceDoubleClicked?.Invoke(this, _device);
    }

    /// <summary>
    /// 为设备按钮绑定右键菜单
    /// </summary>
    public void BindContextMenu(System.Windows.Forms.ContextMenuStrip contextMenu)
    {
        btn_DevName.ContextMenuStrip = contextMenu;
    }

    private void OnDeviceConnectionChanged(object sender, bool connected)
    {
        if (InvokeRequired)
        {
            try { BeginInvoke(new Action<object, bool>(OnDeviceConnectionChanged), sender, connected); } catch { }
            return;
        }

        UpdateConnectionState(connected);
    }

    private void UpdateConnectionState(bool connected)
    {
        _isUpdating = true;
        try
        {
            sw_Connect.Checked = connected;
            // 使用按钮Type指示连接状态：Success=已连接，Error=未连接
            btn_DevName.Type = connected ? TTypeMini.Success : TTypeMini.Error;
        }
        finally
        {
            _isUpdating = false;
        }
    }

    private void UpdateSelectionStyle()
    {
        if (_isSelected)
        {
            this.BackColor = Color.FromArgb(70, 130, 180); // SteelBlue
        }
        else
        {
            this.BackColor = Color.FromArgb(64, 64, 64);
        }
    }

    private void sw_Connect_CheckedChanged(object sender, AntdUI.BoolEventArgs e)
    {
        if (_isUpdating || _device == null)
            return;

        try
        {
            if (e.Value)
            {
                var result = _device.Open();
                if (!result)
                {
                    // 打开失败，恢复开关状态
                    _isUpdating = true;
                    sw_Connect.Checked = false;
                    _isUpdating = false;
                }
            }
            else
            {
                _device.Close();
            }

            ConnectionStateChanged?.Invoke(this, _device.IsConnected);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[{_device.Name}] 连接操作失败: {ex.Message}");
            // 恢复开关状态
            _isUpdating = true;
            sw_Connect.Checked = _device.IsConnected;
            _isUpdating = false;
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            if (_device != null)
            {
                try { _device.ConnectionStatusChanged -= OnDeviceConnectionChanged; } catch { }
                _device = null;
            }

            if (components != null)
            {
                components.Dispose();
            }
        }
        base.Dispose(disposing);
    }
}
