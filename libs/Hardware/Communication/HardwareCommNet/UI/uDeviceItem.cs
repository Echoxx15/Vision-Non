using System;
using System.Drawing;
using System.Windows.Forms;
using AntdUI;

namespace HardwareCommNet.UI;

/// <summary>
/// 设备列表项控件：用于FlowLayoutPanel中显示设备状态和连接开关
/// </summary>
public partial class uDeviceItem : UserControl
{
    private IComm _device;
    private bool _isUpdating;
    private bool _isSelected;

    /// <summary>
    /// 设备实例
    /// </summary>
    public IComm Device => _device;

    /// <summary>
    /// 设备名称（不含类型前缀）
    /// </summary>
    public string DeviceName => GetPureDeviceName(_device?.Name);

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
    public event EventHandler<IComm> DeviceSelected;

    /// <summary>
    /// 设备双击事件（双击设备项时触发，用于重命名等操作）
    /// </summary>
    public event EventHandler<IComm> DeviceDoubleClicked;

    /// <summary>
    /// 连接状态改变事件
    /// </summary>
    public event EventHandler<bool> ConnectionStateChanged;

    public uDeviceItem()
    {
        InitializeComponent();
        SetupEvents();
    }

    public uDeviceItem(IComm device) : this()
    {
        BindDevice(device);
    }

    /// <summary>
    /// 绑定设备
    /// </summary>
    public void BindDevice(IComm device)
    {
        // 解绑旧设备
        if (_device != null)
        {
            _device.ConnectionStatusChanged -= OnDeviceConnectionChanged;
        }

        _device = device;

        if (_device != null)
        {
            // 更新显示 - 只显示设备名，不显示类型前缀
            btn_DevName.Text = GetPureDeviceName(_device.Name);
            UpdateConnectionState(_device.IsConnected);

            // 订阅连接状态变化事件
            _device.ConnectionStatusChanged += OnDeviceConnectionChanged;
        }
    }

    /// <summary>
    /// 获取纯设备名（去除类型前缀）
    /// </summary>
    private string GetPureDeviceName(string fullName)
    {
        if (string.IsNullOrEmpty(fullName))
            return string.Empty;

        // 移除类型前缀，如 "modbustcp-设备名" -> "设备名"
        var prefixes = new[] { "modbustcp-", "tcpclient-", "tcpserver-", "modbus-", "tcp-" };
        var lowerName = fullName.ToLowerInvariant();
        
        foreach (var prefix in prefixes)
        {
            if (lowerName.StartsWith(prefix))
            {
                return fullName.Substring(prefix.Length);
            }
        }

        return fullName;
    }

    /// <summary>
    /// 更新设备名称显示
    /// </summary>
    public void UpdateDeviceName(string newName)
    {
        btn_DevName.Text = GetPureDeviceName(newName);
    }

    private void SetupEvents()
    {
        // 按钮左键点击选中设备
        btn_DevName.Click += (s, e) => DeviceSelected?.Invoke(this, _device);
        
        // 按钮双击打开通讯表配置
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
                _device.Connect();
            }
            else
            {
                _device.Disconnect();
            }

            ConnectionStateChanged?.Invoke(this, e.Value);
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
