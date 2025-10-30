using System;
using System.Windows.Forms;
using HardwareCameraNet;
using Vision.Manager.CameraManager;
using Vision.Solutions.Models;
// ICamera
// TriggerMode enum

// StationCameraParams

namespace Vision.Frm.Process;

public partial class Frm_CameraConfig : Form
{
    private ICamera _camera; // 可选：用于提供参数上限
    private StationCameraParams _params; // 必选：用于界面双向绑定
    private bool _loading;

    public Frm_CameraConfig()
    {
        InitializeComponent();
        // 绑定事件（写入到 _params）
        nUD_ExposureTime.ValueChanged += (_, _) => { if (!_loading && _params != null) _params.Exposure = (double)nUD_ExposureTime.Value; };
        nUD_Gain.ValueChanged +=       (_, _) => { if (!_loading && _params != null) _params.Gain     = (double)nUD_Gain.Value; };
        nUD_Width.ValueChanged += (_, _) => { if (!_loading && _params != null) _params.Width = (int)nUD_Width.Value; };
        nUD_Height.ValueChanged += (_, _) => { if (!_loading && _params != null) _params.Height = (int)nUD_Height.Value; };
        nUD_TimeOut.ValueChanged +=    (_, _) => { if (!_loading && _params != null) _params.TimeoutMs = (int)nUD_TimeOut.Value; };
        nUD_TriggerCount.ValueChanged += (_, _) => { if (!_loading && _params != null) _params.TriggerCount = (int)nUD_TriggerCount.Value; };
        rb_SoftWare.CheckedChanged +=  (_, _) => { if (!_loading && _params != null && rb_SoftWare.Checked) _params.TriggerMode = TriggerMode.Software; };
        rb_Hardware.CheckedChanged +=  (_, _) => { if (!_loading && _params != null && rb_Hardware.Checked) _params.TriggerMode = TriggerMode.Hardware; };
    }

    // 初始化：绑定工位相机参数类与（可选）相机实例
    public void Init(StationCameraParams cameraParams, ICamera camera = null)
    {
        _params = cameraParams ?? throw new ArgumentNullException(nameof(cameraParams));
        _camera = camera; // 可能为 null
        InitUIFromParams();
    }

    private void InitUIFromParams()
    {
        _loading = true;
        try
        {
            // 将参数填充到控件
            nUD_ExposureTime.Value = SafeDecimal(_params.Exposure);
            nUD_Gain.Value = SafeDecimal(_params.Gain);
            nUD_TimeOut.Value = _params.TimeoutMs <= 0 ? 3000 : _params.TimeoutMs;
            nUD_TriggerCount.Value = _params.TriggerCount <= 0 ? 1 : _params.TriggerCount;
            nUD_Width.Value = SafeDecimal(_params.Width);
            nUD_Height.Value = SafeDecimal(_params.Height);
            rb_SoftWare.Checked = _params.TriggerMode == TriggerMode.Software;
            rb_Hardware.Checked = _params.TriggerMode == TriggerMode.Hardware;

            // 如果提供了相机实例，则用其实时能力限制控件的上限，并显示类型
            if (_camera != null)
            {
                try { nUD_ExposureTime.Maximum = (decimal)Math.Max(1.0, _camera.Parameters.MaxExposureTime); } catch { }
                try { nUD_Gain.Maximum = (decimal)Math.Max(1.0, _camera.Parameters.MaxGain); } catch { }
                Text = $"相机配置 - {_camera.Type}";
            }
        }
        finally { _loading = false; }
    }

    private static decimal SafeDecimal(double val)
    {
        if (double.IsNaN(val) || double.IsInfinity(val)) return 0m;
        try { return (decimal)val; } catch { return 0m; }
    }
}