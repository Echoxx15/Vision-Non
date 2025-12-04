using System;
using System.Text;
using System.Windows.Forms;
using System.Linq;
using HardwareCommNet;
using TcpControlNet.UI;
using Logger;

namespace TcpControlNet;

/// <summary>
/// TCP 客户端通讯适配器（IComm 实现）
/// </summary>
[CommManufacturer("TcpClient")]
public class TcpClientAdapter : CommAdapterBase
{
    #region 字段

    private NetTcpClient _client;

    // 配置参数
    public string IpAddress { get; set; } = "127.0.0.1";
    public int Port { get; set; } = 5000;
    public string EncodingName { get; set; } = "UTF-8";
    public bool FrameByNewLine { get; set; } = true;
    public bool AutoReconnect { get; set; } = true;
    public int ReconnectDelayMs { get; set; } = 3000;
    public int ConnectTimeoutMs { get; set; } = 5000;

    /// <summary>
    /// 是否已连接（只读属性）
    /// </summary>
    public override bool IsConnected => _client?.IsConnected ?? false;

    /// <summary>
    /// 原始消息接收事件（用于UI直接显示，未经处理的消息）
    /// </summary>
    public event EventHandler<string> RawMessageReceived;

    #endregion

    #region 私有字段

    private uConfigControl _configControl;

    #endregion

    #region 构造函数

    /// <summary>
    /// 初始化 TCP 客户端适配器
    /// </summary>
    public TcpClientAdapter(string name = "TcpClient") : base(name)
    {
    }

    #endregion

    #region 重写接口方法

    /// <summary>
    /// 获取配置控件
    /// </summary>
    public override UserControl GetConfigControl()
    {
        if (_configControl == null || _configControl.IsDisposed)
        {
            _configControl = new uConfigControl();
            _configControl.SetDevice(this, TcpType.Client);
        }

        return _configControl;
    }

    /// <summary>
    /// 连接设备
    /// </summary>
    public override void Connect()
    {
        if (IsConnected) return;

        try
        {
            LogHelper.Info($"[{Name}] ========== TCP客户端开始连接 ==========");
            LogHelper.Info($"[{Name}] 目标地址: {IpAddress}:{Port}");

            // 获取编码
            Encoding encoding;
            try
            {
                encoding = Encoding.GetEncoding(EncodingName);
            }
            catch
            {
                encoding = Encoding.UTF8;
            }

            // 创建底层客户端（传入 Name 参数）
            _client = new NetTcpClient(IpAddress, Port, encoding, Name)
            {
                AutoReconnect = AutoReconnect,
                ReconnectDelayMs = ReconnectDelayMs,
                ConnectTimeoutMs = ConnectTimeoutMs,
                FrameByNewLine = FrameByNewLine
            };

            // 订阅底层客户端事件，使用基类方法来发送
            _client.ConnectionStatusChanged += OnClientConnectionStatusChanged;
            _client.MessageReceived += OnClientMessageReceived;
            _client.RawMessageReceived += OnClientRawMessageReceived;

            LogHelper.Info($"[{Name}] ✓ 已订阅事件");

            // 开始连接
            _client.Start();

            LogHelper.Info($"[{Name}] TCP客户端开始连接: {IpAddress}:{Port}");
        }
        catch (Exception ex)
        {
            LogHelper.Error(ex, $"[{Name}] TCP客户端创建失败");
            OnConnectionStatusChanged(false);
        }
    }

    /// <summary>
    /// 断开连接
    /// </summary>
    public override void Disconnect()
    {
        if (!IsConnected) return;

        try
        {
            if (_client != null)
            {
                // 取消订阅
                _client.ConnectionStatusChanged -= OnClientConnectionStatusChanged;
                _client.MessageReceived -= OnClientMessageReceived;
                _client.RawMessageReceived -= OnClientRawMessageReceived;

                _client.Dispose();
                _client = null;
            }

            OnConnectionStatusChanged(false);
            LogHelper.Info($"[{Name}] TCP客户端已断开连接");
        }
        catch (Exception ex)
        {
            LogHelper.Error(ex, $"[{Name}] TCP客户端断开失败");
        }
    }

    /// <summary>
    /// 写入数据
    /// </summary>
    public override void Write(string address, object data)
    {
        // 使用基类验证数据
        ValidateNotNull(data, nameof(data));

        if (!IsConnected || _client == null)
        {
            LogHelper.Warn($"[{Name}] TCP客户端未连接，无法发送数据");
            return;
        }

        try
        {
            string message = data.ToString();
            _client.Send(message);
            LogHelper.Info($"[{Name}] 发送消息: {message}");
        }
        catch (Exception ex)
        {
            LogHelper.Error(ex, $"[{Name}] TCP客户端发送失败");
        }
    }

    /// <summary>
    /// 写入多个数据
    /// </summary>
    public override void Write(string address, object[] data)
    {
        // 使用基类验证数据
        ValidateNotEmpty(data, nameof(data));

        // TCP不需要地址，将数据转换为字符串发送
        Write(address, string.Join("", data));
    }

    /// <summary>
    /// 获取当前配置
    /// </summary>
    public override CommConfig GetConfig()
    {
        // 先让基类序列化通讯表
        var config = base.GetConfig();
        // 再写入本设备特有参数
        config.SetParameter("IpAddress", IpAddress);
        config.SetParameter("Port", Port.ToString());
        config.SetParameter("Encoding", EncodingName);
        config.SetParameter("FrameByNewLine", FrameByNewLine.ToString());
        config.SetParameter("AutoReconnect", AutoReconnect.ToString());
        config.SetParameter("ReconnectDelayMs", ReconnectDelayMs.ToString());
        config.SetParameter("ConnectTimeoutMs", ConnectTimeoutMs.ToString());
        config.SetParameter("IsConnected", IsConnected.ToString());
        return config;
    }

    /// <summary>
    /// 应用配置
    /// </summary>
    public override void ApplyConfig(CommConfig config)
    {
        if (config == null) return;

        // 先让基类恢复通讯表
        base.ApplyConfig(config);

        // 应用本设备参数
        IpAddress = config.GetParameter("IpAddress", "127.0.0.1");

        if (int.TryParse(config.GetParameter("Port", "5000"), out var port))
            Port = port;

        EncodingName = config.GetParameter("Encoding", "UTF-8");

        if (bool.TryParse(config.GetParameter("FrameByNewLine", "true"), out var frameByNewLine))
            FrameByNewLine = frameByNewLine;

        if (bool.TryParse(config.GetParameter("AutoReconnect", "true"), out var autoReconnect))
            AutoReconnect = autoReconnect;

        if (int.TryParse(config.GetParameter("ReconnectDelayMs", "3000"), out var reconnectDelay))
            ReconnectDelayMs = reconnectDelay;

        if (int.TryParse(config.GetParameter("ConnectTimeoutMs", "5000"), out var connectTimeout))
            ConnectTimeoutMs = connectTimeout;

        // 不在此处自动连接，交由工厂在 ApplyConfig 之后调用 Connect()
    }

    /// <summary>
    /// 释放资源
    /// </summary>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            try
            {
                _client?.Dispose();
                _client = null;
                _configControl?.Dispose();
                _configControl = null;
            }
            catch
            {
            }
        }

        base.Dispose(disposing);
    }

    #endregion

    #region 事件处理（转发到基类）

    /// <summary>
    /// 底层客户端连接状态变化事件处理
    /// </summary>
    private void OnClientConnectionStatusChanged(object sender, bool connected)
    {
        if (connected)
        {
            LogHelper.Info($"[{Name}] ✅ TCP客户端连接成功: {IpAddress}:{Port}");
        }
        else
        {
            LogHelper.Warn($"[{Name}] ❌ TCP客户端连接失败，将自动重连...");
        }
        OnConnectionStatusChanged(connected);
    }

    /// <summary>
    /// 底层客户端消息接收事件处理
    /// </summary>
    private void OnClientMessageReceived(object sender, object message)
    {
        var text = message?.ToString() ?? "";

        // 使用TcpMessageHelper处理消息（日志已在Helper中精简）
        var result = TcpMessageHelper.ProcessMessage(text, Table, Name);

        // 如果有触发，发送MessageReceived事件
        if (result.HasTrigger)
        {
            var payload = TcpMessageHelper.CreateTriggerPayload(result);
            OnMessageReceived(payload);
        }
    }

    /// <summary>
    /// 底层客户端原始消息接收事件处理（用于UI显示）
    /// </summary>
    private void OnClientRawMessageReceived(object sender, string message)
    {
        // 转发原始消息给UI
        try
        {
            RawMessageReceived?.Invoke(this, message);
        }
        catch
        {
            // 事件处理异常不影响消息处理
        }
    }

    #endregion
}
