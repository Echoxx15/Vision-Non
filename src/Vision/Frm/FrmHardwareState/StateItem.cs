using System;
using System.Drawing;
using System.Windows.Forms;
using HardwareCommNet;

namespace Vision.Frm.FrmHardwareState;

public sealed partial class StateItem : AntdUI.Button
{
    private string _sn { get; }
    public string SN => _sn;

    private IComm _device; // 可选：订阅其连接状态事件
    public bool Connected { get; private set; }

    /// <summary>
    /// 创建状态Item
    /// </summary>
    /// <param name="sn">唯一键（如 COMM:Name 或 TCP:Name）</param>
    /// <param name="text">设备显示名</param>
    /// <param name="state">初始状态</param>
    public StateItem(string sn, string text, bool state)
    {
        InitializeComponent();
        _sn = sn;
        OriginalBackColor = Color.FromArgb(64,64,64);
        SetState(text, state);

        //释放时自动退订，避免与 Designer 的 Dispose 冲突
        this.Disposed += (_, __) =>
        {
            try
            {
                if (_device != null)
                {
                    _device.ConnectionStatusChanged -= OnDeviceConnectionChanged;
                    _device = null;
                }
            }
            catch { }
        };
    }

    /// <summary>
    /// 创建状态Item并订阅设备状态事件
    /// </summary>
    public StateItem(string sn, string text, bool state, IComm device) : this(sn, text, state)
    {
        AttachDevice(device);
    }

    private void AttachDevice(IComm device)
    {
        _device = device;
        if (_device == null) return;
        try
        {
            _device.ConnectionStatusChanged -= OnDeviceConnectionChanged; // 防重
            _device.ConnectionStatusChanged += OnDeviceConnectionChanged;
        }
        catch { }
    }

    private void OnDeviceConnectionChanged(object sender, bool connected)
    {
        if (InvokeRequired)
        {
            try { BeginInvoke(new Action<object, bool>(OnDeviceConnectionChanged), sender, connected); } catch { }
            return;
        }
        try
        {
            Connected = connected;
            //仅刷新颜色，不改标题
            DefaultBack = connected ? Color.Lime : Color.Red;
        }
        catch { }
    }

    /// <summary>
    /// 更新设备状态和备注
    /// </summary>
    public void SetState(string text, bool state)
    {
        Text = text;
        Connected = state;
        DefaultBack = state ? Color.Lime : Color.Red;
    }
}