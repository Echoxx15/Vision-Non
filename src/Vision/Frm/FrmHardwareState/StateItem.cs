using System;
using System.Drawing;
using HardwareCommNet;
using LightControlNet;

namespace Vision.Frm.FrmHardwareState;

public sealed partial class StateItem : AntdUI.Button
{
    private string _sn { get; }
    public string SN => _sn;

    private IComm _commDevice; // 通讯设备（可选）
    private ILightController _lightController; // 光源控制器（可选）
    public bool Connected { get; private set; }

    /// <summary>
    /// 创建状态Item
    /// </summary>
    /// <param name="sn">唯一键（如 CAMERA:SN, COMM:Name, LIGHT:Name）</param>
    /// <param name="text">设备显示名</param>
    /// <param name="state">初始状态</param>
    public StateItem(string sn, string text, bool state)
    {
        InitializeComponent();
        _sn = sn;
        OriginalBackColor = Color.FromArgb(64,64,64);
        SetState(text, state);

        // 释放时自动退订，避免与 Designer 的 Dispose 冲突
        this.Disposed += (_, __) =>
        {
            try
            {
                if (_commDevice != null)
                {
                    _commDevice.ConnectionStatusChanged -= OnCommDeviceConnectionChanged;
                    _commDevice = null;
                }
                if (_lightController != null)
                {
                    _lightController.ConnectionStatusChanged -= OnLightControllerConnectionChanged;
                    _lightController = null;
                }
            }
            catch { }
        };
    }

    /// <summary>
    /// 创建状态Item并订阅通讯设备状态事件
    /// </summary>
    public StateItem(string sn, string text, bool state, IComm commDevice) : this(sn, text, state)
    {
        AttachCommDevice(commDevice);
    }

    /// <summary>
    /// 创建状态Item并订阅光源控制器状态事件
    /// </summary>
    public StateItem(string sn, string text, bool state, ILightController lightController) : this(sn, text, state)
    {
        AttachLightController(lightController);
    }

    private void AttachCommDevice(IComm device)
    {
        _commDevice = device;
        if (_commDevice == null) return;
        try
        {
            _commDevice.ConnectionStatusChanged -= OnCommDeviceConnectionChanged; // 防重
            _commDevice.ConnectionStatusChanged += OnCommDeviceConnectionChanged;
        }
        catch { }
    }

    private void AttachLightController(ILightController controller)
    {
        _lightController = controller;
        if (_lightController == null) return;
        try
        {
            _lightController.ConnectionStatusChanged -= OnLightControllerConnectionChanged; // 防重
            _lightController.ConnectionStatusChanged += OnLightControllerConnectionChanged;
        }
        catch { }
    }

    private void OnCommDeviceConnectionChanged(object sender, bool connected)
    {
        UpdateConnectionState(connected);
    }

    private void OnLightControllerConnectionChanged(object sender, bool connected)
    {
        UpdateConnectionState(connected);
    }

    private void UpdateConnectionState(bool connected)
    {
        if (InvokeRequired)
        {
            try { BeginInvoke(new Action<bool>(UpdateConnectionState), connected); } catch { }
            return;
        }
        try
        {
            Connected = connected;
            // 仅刷新颜色，不改标题
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
