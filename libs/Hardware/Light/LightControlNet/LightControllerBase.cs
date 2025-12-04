using System;
using System.Windows.Forms;

namespace LightControlNet;

/// <summary>
/// 光源控制器基类
/// 提供通用功能实现，子类只需实现具体的硬件通信逻辑
/// </summary>
public abstract class LightControllerBase : ILightController
{
    private bool _disposed;
    private bool _isConnected;

    /// <summary>
    /// 设备名称
    /// </summary>
    public string Name { get; protected set; }

    /// <summary>
    /// 控制器类型（品牌）
    /// </summary>
    public abstract LightControllerType Type { get; }

    /// <summary>
    /// 是否已连接
    /// </summary>
    public bool IsConnected
    {
        get => _isConnected;
        protected set
        {
            if (_isConnected != value)
            {
                _isConnected = value;
                OnConnectionStatusChanged(value);
            }
        }
    }

    /// <summary>
    /// 通道数量
    /// </summary>
    public int ChannelCount { get; protected set; }

    /// <summary>
    /// 测试窗体
    /// </summary>
    public abstract Form TestForm { get; }

    /// <summary>
    /// 连接状态变化事件
    /// </summary>
    public event EventHandler<bool> ConnectionStatusChanged;

    protected LightControllerBase(LightConfig config)
    {
        if (config == null)
            throw new ArgumentNullException(nameof(config));

        Name = config.Name;
        ChannelCount = config.ChannelCount;
    }

    /// <summary>
    /// 打开连接
    /// </summary>
    public abstract bool Open();

    /// <summary>
    /// 关闭连接
    /// </summary>
    public abstract void Close();

    /// <summary>
    /// 打开指定通道
    /// </summary>
    public abstract bool TurnOn(int channel);

    /// <summary>
    /// 关闭指定通道
    /// </summary>
    public abstract bool TurnOff(int channel);

    /// <summary>
    /// 设置指定通道亮度
    /// </summary>
    public abstract bool SetBrightness(int channel, int brightness);

    /// <summary>
    /// 获取指定通道亮度
    /// </summary>
    public abstract int GetBrightness(int channel);

    /// <summary>
    /// 设置多通道亮度
    /// </summary>
    public virtual bool SetMultiChannelBrightness(int[] channels, int brightness)
    {
        if (channels == null || channels.Length == 0)
            return false;

        bool allSuccess = true;
        foreach (var channel in channels)
        {
            if (!SetBrightness(channel, brightness))
                allSuccess = false;
        }
        return allSuccess;
    }

    /// <summary>
    /// 关闭所有通道
    /// </summary>
    public virtual void TurnOffAllChannels()
    {
        for (int i = 1; i <= ChannelCount; i++)
        {
            TurnOff(i);
        }
    }

    /// <summary>
    /// 发送原始命令
    /// </summary>
    public abstract string SendRawCommand(string command);

    /// <summary>
    /// 获取配置控件
    /// </summary>
    public virtual UserControl GetConfigControl()
    {
        return null;
    }

    /// <summary>
    /// 触发连接状态变化事件
    /// </summary>
    protected virtual void OnConnectionStatusChanged(bool connected)
    {
        try
        {
            ConnectionStatusChanged?.Invoke(this, connected);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[{Name}] 连接状态变化事件处理异常: {ex.Message}");
        }
    }

    /// <summary>
    /// 设置设备名称
    /// </summary>
    public virtual void SetName(string name)
    {
        if (!string.IsNullOrWhiteSpace(name))
            Name = name.Trim();
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed) return;

        if (disposing)
        {
            try
            {
                if (IsConnected)
                {
                    TurnOffAllChannels();
                    Close();
                }
            }
            catch { }
        }

        _disposed = true;
    }

    ~LightControllerBase()
    {
        Dispose(false);
    }
}
