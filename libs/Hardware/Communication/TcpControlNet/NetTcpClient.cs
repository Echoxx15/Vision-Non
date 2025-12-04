using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Logger;

namespace TcpControlNet;

/// <summary>
/// TCP 客户端，统一使用 IComm 接口事件。
/// - async/await 架构
/// - 自动重连（可关闭）
/// - 统一通过 MessageReceived 分发消息
/// - 统一通过 ConnectionStatusChanged 报告连接状态
/// - 简单可控的换行分帧（可选）
/// </summary>
public sealed class NetTcpClient : IDisposable
{
    private readonly string _ip;
    private readonly int _port;
    private Encoding _encoding;
    private TcpClient _client;
    private NetworkStream _stream;
    private CancellationTokenSource _cts;
    private Task _runTask;
    private volatile bool _connected;

    private readonly byte[] _rxBuf = new byte[8192];
    private readonly StringBuilder _lineAcc = new StringBuilder();

    // 配置属性
    /// <summary>是否自动重连（默认 true）</summary>
    public bool AutoReconnect { get; set; } = true;

    /// <summary>重连间隔（毫秒）</summary>
    public int ReconnectDelayMs { get; set; } = 3000;

    /// <summary>连接超时（毫秒）</summary>
    public int ConnectTimeoutMs { get; set; } = 5000;

    // 分帧
    /// <summary>是否按换行符分帧（true 时按"\n"分割上报消息事件）</summary>
    public bool FrameByNewLine { get; set; }

    // 统一使用 IComm 接口事件
    /// <summary>消息接收事件（统一接口）</summary>
    public event EventHandler<object> MessageReceived;

    /// <summary>连接状态变化事件（统一接口）</summary>
    public event EventHandler<bool> ConnectionStatusChanged;

    /// <summary>原始字节接收事件（供特殊调试场景）</summary>
    public event Action<byte[], int> BytesReceived;

    /// <summary>原始文本消息接收事件（未经处理的原始消息，用于UI显示）</summary>
    public event EventHandler<string> RawMessageReceived;

    public string Name { get; }

    /// <summary>当前是否处于连接状态</summary>
    public bool IsConnected => _connected;

    /// <summary>
    /// 更改编码方式（默认 UTF8）。可在运行时修改。
    /// </summary>
    public Encoding TextEncoding
    {
        get => _encoding;
        set => _encoding = value ?? Encoding.UTF8;
    }

    /// <summary>
    /// 创建 TCP 客户端。
    /// </summary>
    /// <param name="ip">服务器IP地址</param>
    /// <param name="port">服务器端口</param>
    /// <param name="encoding">文本编码（默认UTF8）</param>
    /// <param name="name">客户端名称（用于日志标识）</param>
    public NetTcpClient(string ip, int port, Encoding encoding = null, string name = null)
    {
        _ip = ip;
        _port = port;
        _encoding = encoding ?? Encoding.UTF8;
        Name = name ?? $"TcpClient_{ip}:{port}";
        // 不在构造函数中启动连接，由外部显式调用 Start()
        // 这样可以避免在初始化阶段启动异步线程
    }

    /// <summary>
    /// 开始连接并启动接收循环。如果断开且 AutoReconnect=true 会自动重连。
    /// </summary>
    public void Start()
    {
        if (_runTask != null) return; // 防止重复启动
        _cts = new CancellationTokenSource();
        _runTask = RunAsync(_cts.Token); // 启动后台连接/接收循环
    }

    /// <summary>
    /// 停止连接，断开连接并取消后台循环。
    /// </summary>
    public void Stop()
    {
        var cts = _cts;
        _cts = null;
        try
        {
            cts?.Cancel();
        }
        catch
        {
        }

        try
        {
            _runTask?.Wait(1000);
        }
        catch
        {
        }

        _runTask = null;
        Cleanup(); // 关闭底层连接套接字
        SetConnected(false);
    }

    /// <summary>
    /// 后台连接接收循环：连接 -> 接收 -> 异常/断开 -> 重试（可选重连）
    /// </summary>
    private async Task RunAsync(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            try
            {
                await ConnectOnceAsync(token).ConfigureAwait(false);
                await ReceiveLoopAsync(token).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch
            {
                /* 捕获循环，自动重连 */
            }

            SetConnected(false);
            Cleanup();
            if (!AutoReconnect) break; // 关闭自动重连则退出
            try
            {
                await Task.Delay(ReconnectDelayMs, token).ConfigureAwait(false);
            }
            catch
            {
                break;
            }
        }
    }

    /// <summary>
    /// 执行一次连接（带超时），成功则获取 NetworkStream。
    /// </summary>
    private async Task ConnectOnceAsync(CancellationToken token)
    {
        Cleanup();
        _client = new TcpClient();
        var connectTask = _client.ConnectAsync(_ip, _port);
        var timeout = Task.Delay(ConnectTimeoutMs, token);
        var done = await Task.WhenAny(connectTask, timeout).ConfigureAwait(false);
        if (done != connectTask) throw new TimeoutException("TCP 连接超时"); // 连接超时
        _stream = _client.GetStream(); // 获取网络流
        SetConnected(true);
    }

    /// <summary>
    /// 接收循环：ReadAsync -> 上报原始字节 -> 转换文本 -> 触发 MessageReceived
    /// </summary>
    private async Task ReceiveLoopAsync(CancellationToken token)
    {
        while (!token.IsCancellationRequested && _client != null && _client.Connected)
        {
            int n;
            try
            {
                n = await _stream.ReadAsync(_rxBuf, 0, _rxBuf.Length, token).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch
            {
                break;
            } // 任何异常视为断开

            if (n <= 0) break; // 远程端主动断开

            // 可选：触发原始字节事件（供特殊调试场景，如自定义协议）
            try
            {
                BytesReceived?.Invoke(_rxBuf, n);
            }
            catch
            {
                // 用户事件处理异常不影响接收循环
            }

            // 统一处理文本消息并触发 MessageReceived
            try
            {
                var text = _encoding.GetString(_rxBuf, 0, n); // 本次收到的数据转换为字符串

                if (FrameByNewLine)
                {
                    // 换行符分帧，累积并分行上报
                    _lineAcc.Append(text);
                    string acc = _lineAcc.ToString();
                    int idx;
                    while ((idx = acc.IndexOf('\n')) >= 0)
                    {
                        var line = acc.Substring(0, idx).TrimEnd('\r');
                        try
                        {
                            LogHelper.Info($"[{Name}] 收到消息: {line}");
                        }
                        catch
                        {
                            // 日志失败不影响消息处理
                        }
                        
                        // 触发原始消息事件（用于UI显示）
                        try { RawMessageReceived?.Invoke(this, line); } catch { }
                        
                        // 触发处理后的消息事件
                        try { MessageReceived?.Invoke(this, line); } catch { }
                        
                        acc = acc.Substring(idx + 1);
                    }

                    _lineAcc.Clear();
                    _lineAcc.Append(acc); // 保留未形成一行的部分继续累积
                }
                else
                {
                    // 直接按批上报
                    try
                    {
                        LogHelper.Info($"[{Name}] 收到消息块: {text.Length} 字节, 内容: {text}");
                    }
                    catch
                    {
                        // 日志失败不影响消息处理
                    }
                    
                    // 触发原始消息事件（用于UI显示）
                    try { RawMessageReceived?.Invoke(this, text); } catch { }
                    
                    // 触发处理后的消息事件
                    try { MessageReceived?.Invoke(this, text); } catch { }
                }
            }
            catch
            {
                // 用户事件处理异常不影响接收循环
            }
        }
    }

    /// <summary>
    /// 发送文本（使用当前编码）。
    /// </summary>
    public async Task<bool> SendAsync(string text)
    {
        if (_stream == null || !_connected) return false;
        try
        {
            var data = _encoding.GetBytes(text);
            await _stream.WriteAsync(data, 0, data.Length).ConfigureAwait(false); // 异步写入
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// 发送二进制数据。
    /// </summary>
    public async Task<bool> SendAsync(byte[] data, int offset = 0, int count = -1)
    {
        if (_stream == null || !_connected) return false;
        try
        {
            if (count < 0) count = data.Length - offset;
            await _stream.WriteAsync(data, offset, count).ConfigureAwait(false);
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// 同步发送文本（会阻塞当前线程，慎重使用）
    /// </summary>
    public bool Send(string text)
    {
        try
        {
            return SendAsync(text).GetAwaiter().GetResult();
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// 设置连接状态并触发 ConnectionStatusChanged 事件
    /// </summary>
    private void SetConnected(bool c)
    {
        _connected = c;

        try
        {
            // 统一使用 ConnectionStatusChanged
            ConnectionStatusChanged?.Invoke(this, c);
        }
        catch
        {
            // 用户事件处理异常不影响状态更新
        }
    }

    /// <summary>
    /// 释放底层网络资源（不改变 AutoReconnect）
    /// </summary>
    private void Cleanup()
    {
        try
        {
            _stream?.Dispose();
        }
        catch
        {
        }

        _stream = null;
        try
        {
            _client?.Close();
        }
        catch
        {
        }

        _client = null;
    }

    /// <summary>
    /// 释放资源，停止连接。
    /// </summary>
    public void Dispose()
    {
        Stop();
    }
}
