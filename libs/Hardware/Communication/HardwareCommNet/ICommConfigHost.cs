using System;
using System.Windows.Forms;

namespace HardwareCommNet;

/// <summary>
/// 通讯配置宿主接口：供主界面访问通讯配置功能
/// </summary>
public interface ICommConfigHost
{
    /// <summary>
    /// 获取通讯配置控件
    /// </summary>
    UserControl ConfigControl { get; }

    /// <summary>
    /// 当前选中的设备
    /// </summary>
    IComm SelectedDevice { get; }

    /// <summary>
    /// 设备选中事件
    /// </summary>
    event EventHandler<IComm> DeviceSelected;

    /// <summary>
    /// 请求打开通讯表配置事件
    /// </summary>
    event EventHandler<IComm> OpenCommTableRequested;

    /// <summary>
    /// 打开指定设备的通讯表配置
    /// </summary>
    void OpenCommTable(IComm device);

    /// <summary>
    /// 保存配置
    /// </summary>
    void SaveConfigs();

    /// <summary>
    /// 加载配置
    /// </summary>
    void LoadConfigs();
}

/// <summary>
/// 通讯配置宿主实现
/// </summary>
public sealed class CommConfigHost : ICommConfigHost
{
    private static readonly Lazy<CommConfigHost> _instance = new(() => new CommConfigHost());
    public static CommConfigHost Instance => _instance.Value;

    private UI.Frm_CommConfig _configControl;

    public UserControl ConfigControl
    {
        get
        {
            if (_configControl == null || _configControl.IsDisposed)
            {
                _configControl = new UI.Frm_CommConfig();
                _configControl.DeviceSelected += (s, e) => DeviceSelected?.Invoke(s, e);
                _configControl.OpenCommTableRequested += (s, e) => OpenCommTableRequested?.Invoke(s, e);
            }
            return _configControl;
        }
    }

    public IComm SelectedDevice => _configControl?.SelectedDevice;

    public event EventHandler<IComm> DeviceSelected;
    public event EventHandler<IComm> OpenCommTableRequested;

    private CommConfigHost()
    {
    }

    /// <summary>
    /// 打开指定设备的通讯表配置
    /// </summary>
    public void OpenCommTable(IComm device)
    {
        if (device == null) return;

        try
        {
            using (var frm = new UI.Frm_CommTable(device))
            {
                frm.ShowDialog();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"打开通讯表配置失败: {ex.Message}");
            MessageBox.Show($"打开通讯表配置失败: {ex.Message}", "错误", 
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    /// <summary>
    /// 保存配置
    /// </summary>
    public void SaveConfigs()
    {
        CommunicationFactory.Instance.SaveConfigs();
    }

    /// <summary>
    /// 加载配置
    /// </summary>
    public void LoadConfigs()
    {
        CommunicationFactory.Instance.LoadConfigs();
    }
}
