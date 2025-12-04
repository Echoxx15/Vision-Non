using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HardwareCommNet;
using TcpControlNet.UI;
using Vision.Comm.Sokect;
using Logger;

namespace TcpControlNet;

/// <summary>
/// TCP 服务器通讯适配器（IComm 实现）
/// </summary>
[CommManufacturer("TcpServer")]
public class TcpServerAdapter : CommAdapterBase
{
    #region 字段

    private NetTcpServer _server;
    private readonly List<string> _connectedClients = new List<string>();

    // 配置参数
    public string IpAddress { get; set; } = "0.0.0.0";
    public int Port { get; set; } = 5000;
    public string EncodingName { get; set; } = "UTF-8";
    public bool FrameByNewLine { get; set; } = true;
    public int MaxConnections { get; set; } = 100;

    /// <summary>
    /// 是否已连接（重写基类属性）
    /// </summary>
    public override bool IsConnected { get; protected set; }

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
    /// 初始化 TCP 服务器适配器
    /// </summary>
    public TcpServerAdapter(string name = "TcpServer") : base(name)
    {
    }

    #endregion

    #region 重写基类方法

    /// <summary>
    /// 获取配置控件
    /// </summary>
    public override UserControl GetConfigControl()
    {
        if (_configControl == null || _configControl.IsDisposed)
        {
            _configControl = new uConfigControl();
            _configControl.SetDevice(this, TcpType.Server);
        }

        return _configControl;
    }

    /// <summary>
    /// 连接设备（启动服务器）
    /// </summary>
    public override void Connect()
    {
        if (IsConnected) return;

        try
        {
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

            // 创建底层服务器
            _server = new NetTcpServer(IpAddress, Port)
            {
                TextEncoding = encoding,
                FrameByNewLine = FrameByNewLine
            };

            Console.WriteLine($"[{Name}] TCP服务器配置: FrameByNewLine={FrameByNewLine}, Encoding={encoding.EncodingName}");

            // 订阅底层服务器事件
            _server.ClientConnected += OnServerClientConnected;
            _server.ClientDisconnected += OnServerClientDisconnected;
            _server.TextReceived += OnServerTextReceived;

            // NetTcpServer 在构造函数中已经自动启动监听
            IsConnected = true;

            // 使用基类方法发送事件
            OnConnectionStatusChanged(true);

            Console.WriteLine($"[{Name}] TCP服务器已启动: {IpAddress}:{Port}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[{Name}] TCP服务器启动失败: {ex.Message}");
            IsConnected = false;
            OnConnectionStatusChanged(false);
        }
    }

    /// <summary>
    /// 断开连接（停止服务器）
    /// </summary>
    public override void Disconnect()
    {
        if (!IsConnected) return;

        try
        {
            if (_server != null)
            {
                _server.ClientConnected -= OnServerClientConnected;
                _server.ClientDisconnected -= OnServerClientDisconnected;
                _server.TextReceived -= OnServerTextReceived;
                _server.Dispose();
                _server = null;
            }

            _connectedClients.Clear();
            IsConnected = false;

            // 使用基类方法发送事件
            OnConnectionStatusChanged(false);

            Console.WriteLine($"[{Name}] TCP服务器已停止");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[{Name}] TCP服务器停止失败: {ex.Message}");
        }
    }

    /// <summary>
    /// 写入数据
    /// </summary>
    /// <param name="address">客户端ID（为空时广播给所有客户端）</param>
    /// <param name="data">要发送的数据</param>
    public override void Write(string address, object data)
    {
        // 使用基类验证数据
        ValidateNotNull(data, nameof(data));

        if (!IsConnected || _server == null)
        {
            Console.WriteLine($"[{Name}] TCP服务器未启动，无法发送数据");
            return;
        }

        try
        {
            string message = data.ToString();

            // 如果指定了地址（客户端ID），发送给指定客户端
            // 否则广播给所有客户端
            if (!string.IsNullOrEmpty(address))
            {
                _server.Send(address, message);
                Console.WriteLine($"[{Name}] 发送消息到客户端[{address}]: {message}");
            }
            else
            {
                _server.Broadcast(message);
                Console.WriteLine($"[{Name}] 广播消息: {message}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[{Name}] TCP服务器发送失败: {ex.Message}");
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
        config.SetParameter("MaxConnections", MaxConnections.ToString());
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
        IpAddress = config.GetParameter("IpAddress", "0.0.0.0");

        if (int.TryParse(config.GetParameter("Port", "5000"), out var port))
            Port = port;

        EncodingName = config.GetParameter("Encoding", "UTF-8");

        if (bool.TryParse(config.GetParameter("FrameByNewLine", "true"), out var frameByNewLine))
            FrameByNewLine = frameByNewLine;

        if (int.TryParse(config.GetParameter("MaxConnections", "100"), out var maxConnections))
            MaxConnections = maxConnections;

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
                _server?.Dispose();
                _server = null;
                _connectedClients.Clear();
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

    #region 事件处理

    /// <summary>
    /// 客户端连接事件处理
    /// </summary>
    private void OnServerClientConnected(string clientId)
    {
        lock (_connectedClients)
        {
            if (!_connectedClients.Contains(clientId))
            {
                _connectedClients.Add(clientId);
            }
        }

        Console.WriteLine($"[{Name}] 客户端已连接: {clientId}");
    }

    /// <summary>
    /// 客户端断开事件处理
    /// </summary>
    private void OnServerClientDisconnected(string clientId)
    {
        lock (_connectedClients)
        {
            _connectedClients.Remove(clientId);
        }

        Console.WriteLine($"[{Name}] 客户端已断开: {clientId}");
    }

    /// <summary>
    /// 收到文本消息的事件处理
    /// </summary>
    private void OnServerTextReceived(string clientId, string text)
    {
        // 详细日志：打印接收到的消息
        LogHelper.Info($"📥 [TcpServerAdapter] 服务器收到消息");
        LogHelper.Info($"   ├─ 适配器名称: {Name}");
        LogHelper.Info($"   ├─ 客户端ID: {clientId}");
        LogHelper.Info($"   ├─ 消息内容: {text}");
        LogHelper.Info($"   └─ 准备处理消息...");

        // 触发原始消息事件（用于UI显示）
        try
        {
            RawMessageReceived?.Invoke(this, $"[{clientId}] {text}");
        }
        catch
        {
            // 事件处理异常不影响消息处理
        }

        // 使用TcpMessageHelper处理消息（日志已在Helper中精简）
        var result = TcpMessageHelper.ProcessMessage(text, Table, Name);

        // 如果有触发，发送MessageReceived事件
        if (result.HasTrigger)
        {
            var payload = TcpMessageHelper.CreateTriggerPayload(result);
            OnMessageReceived(payload);
        }
    }

    #endregion

    #region 辅助方法

    /// <summary>
    /// 获取当前连接的客户端列表
    /// </summary>
    public string[] GetConnectedClients()
    {
        lock (_connectedClients)
        {
            return _connectedClients.ToArray();
        }
    }

    #endregion
}
