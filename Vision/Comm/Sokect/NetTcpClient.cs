using Logger;
using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Vision.Comm.Sokect;

/// <summary>
/// 事件：连接状态变化（true=已连接；false=未连接/断开）。
/// </summary>
public delegate void TcpConnectedChangedHandler(bool connected);
/// <summary>
/// 事件：收到文本（UTF8）。
/// </summary>
public delegate void TcpTextReceivedHandler(string text);
/// <summary>
/// 事件：收到原始字节（不做分帧）。
/// </summary>
public delegate void TcpBytesReceivedHandler(byte[] buffer, int count);

/// <summary>
/// - async/await 架构
/// - 自动重连（可关闭）
/// - 文本/二进制两种上报事件
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

  // 连接与重连
  /// <summary>是否自动重连（默认 true）。</summary>
  public bool AutoReconnect { get; set; } = true;

  /// <summary>重连间隔（毫秒）。</summary>
  public int ReconnectDelayMs { get; set; } = 3000;

  /// <summary>连接超时（毫秒）。</summary>
  public int ConnectTimeoutMs { get; set; } = 5000;

  // 解析
  /// <summary>是否按换行符分帧（true 时按"\n"拆包触发文本事件）。</summary>
  public bool FrameByNewLine { get; set; } = false;

  // 事件
  public event TcpConnectedChangedHandler ConnectedChanged;
  public event TcpTextReceivedHandler TextReceived;
  public event TcpBytesReceivedHandler BytesReceived;

  /// <summary>当前是否处于连接状态。</summary>
  public bool IsConnected => _connected;

  /// <summary>
  /// 文本编解码方式（默认 UTF8），可在运行时修改。
  /// </summary>
  public Encoding TextEncoding
  {
    get => _encoding;
    set => _encoding = value ?? Encoding.UTF8;
  }

  /// <summary>
  /// 构造 TCP 客户端。
  /// </summary>
  public NetTcpClient(string ip, int port, Encoding encoding = null)
  {
    _ip = ip;
    _port = port;
    _encoding = encoding ?? Encoding.UTF8;
    Start();
  }

  /// <summary>
  /// 开始连接并进入接收循环（如断开且 AutoReconnect=true 将自动重连）。
  /// </summary>
  private void Start()
  {
    if (_runTask != null) return; // 已启动则忽略
    _cts = new CancellationTokenSource();
    _runTask = RunAsync(_cts.Token); // 启动后台连接/接收循环
  }

  /// <summary>
  /// 停止并断开连接（取消后台循环）。
  /// </summary>
  private void Stop()
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
    Cleanup(); // 关闭底层流与套接字
    SetConnected(false);
  }

  /// <summary>
  /// 后台运行主循环：连接 -> 接收 -> 异常/断开 -> （可选）重连。
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
        /* 保持循环，进入重连 */
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
  /// 执行一次连接（带超时）。成功后获取 NetworkStream。
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
  /// 接收循环：ReadAsync -> 上报 Bytes/Text -> 断开则退出。
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
      } // 读异常，认为断开

      if (n <= 0) break; // 远端优雅断开

      BytesReceived?.Invoke(_rxBuf, n); // 原始字节上报

      if (FrameByNewLine || TextReceived != null)
      {
        var text = _encoding.GetString(_rxBuf, 0, n); // 将本次缓冲转换为字符串
        LogHelper.Info($"TCP客户端[{_ip}:{_port}]收到原始数据({n}字节): {text}");
        if (FrameByNewLine)
        {
          // 按换行累计并逐行上报
          _lineAcc.Append(text);
          string acc = _lineAcc.ToString();
          int idx;
          while ((idx = acc.IndexOf('\n')) >= 0)
          {
            var line = acc.Substring(0, idx).TrimEnd('\r');
            TextReceived?.Invoke(line);
            acc = acc.Substring(idx + 1);
          }

          _lineAcc.Clear();
          _lineAcc.Append(acc); // 保留未形成一行的残余数据
        }
        else
        {
          TextReceived?.Invoke(text); // 直接按块上报
        }
      }
    }
  }

  /// <summary>
  /// 发送 UTF8 文本。
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
  /// 同步发送 UTF8 文本（阻塞当前线程，谨慎使用）。
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

  private void SetConnected(bool c)
  {
    _connected = c;
    if (c)
    {
      LogHelper.Info($"Tcp客户端连接：{_ip}:{_port}");
    }
    else
    {
      LogHelper.Warn($"Tcp客户端断开：{_ip}:{_port}");
    }

    try
    {
      ConnectedChanged?.Invoke(c);
    }
    catch
    {
      // ignored
    }
  }

  /// <summary>
  /// 释放底层网络资源（不改变 AutoReconnect）。
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
  /// 释放资源并停止连接。
  /// </summary>
  public void Dispose()
  {
    Stop();
  }
}