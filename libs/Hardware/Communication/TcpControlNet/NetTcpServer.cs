using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Vision.Comm.Sokect;

public delegate void NetClientConnectedHandler(string clientId);
public delegate void NetClientDisconnectedHandler(string clientId);
public delegate void NetServerTextHandler(string clientId, string text);
public delegate void NetServerBytesHandler(string clientId, byte[] buffer, int count);

public sealed class NetTcpServer
{
    private readonly string _ip;
    private readonly int _port;
    private Socket _listener;
    private CancellationTokenSource _cts;
    private readonly ConcurrentDictionary<string, Client> _clients = new ConcurrentDictionary<string, Client>();
    private Encoding _encoding = Encoding.UTF8;

    public bool FrameByNewLine { get; set; } = false;

    public Encoding TextEncoding
    {
        get => _encoding;
        set => _encoding = value ?? Encoding.UTF8;
    }

    public event NetClientConnectedHandler ClientConnected;
    public event NetClientDisconnectedHandler ClientDisconnected;
    public event NetServerTextHandler TextReceived;
    public event NetServerBytesHandler BytesReceived;

    private sealed class Client
    {
        public Socket Socket;
        public string Id;
        public StringBuilder Acc = new StringBuilder();
    }

    public NetTcpServer(string ip, int port)
    {
        _ip = ip;
        _port = port;
        Start();
    }

    public bool Start()
    {
        try
        {
            Stop();
            _cts = new CancellationTokenSource();
            _listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _listener.Bind(new IPEndPoint(IPAddress.Parse(_ip), _port));
            _listener.Listen(200);
            _ = AcceptLoopAsync(_cts.Token);
            return true;
        }
        catch
        {
            return false;
        }
    }

    private async Task AcceptLoopAsync(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            Socket s = null;
            try
            {
                s = await _listener.AcceptAsync().ConfigureAwait(false);
                var ep = (IPEndPoint)s.RemoteEndPoint;
                var id = string.Format("{0}:{1}", ep.Address, ep.Port);
                var c = new Client { Socket = s, Id = id };
                if (_clients.TryAdd(id, c))
                {
                    try
                    {
                        var h = ClientConnected;
                        if (h != null) h(id);
                    }
                    catch
                    {
                    }

                    _ = ReceiveLoopAsync(c, token);
                }
                else
                {
                    try
                    {
                        s.Close();
                    }
                    catch
                    {
                    }
                }
            }
            catch (ObjectDisposedException)
            {
                break;
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch
            {
                try
                {
                    await Task.Delay(200, token).ConfigureAwait(false);
                }
                catch
                {
                    break;
                }
            }
        }
    }

    private async Task ReceiveLoopAsync(Client c, CancellationToken token)
    {
        var sock = c.Socket;
        var buf = new byte[8192];
        try
        {
            while (!token.IsCancellationRequested)
            {
                int n = await sock.ReceiveAsync(new ArraySegment<byte>(buf), SocketFlags.None).ConfigureAwait(false);
                if (n <= 0) break;

                var bytesHandler = BytesReceived;
                if (bytesHandler != null) bytesHandler(c.Id, buf, n);

                var textHandler = TextReceived;
                if (FrameByNewLine || textHandler != null)
                {
                    var text = _encoding.GetString(buf, 0, n);
                    if (FrameByNewLine)
                    {
                        c.Acc.Append(text);
                        string acc = c.Acc.ToString();
                        int idx;
                        while ((idx = acc.IndexOf('\n')) >= 0)
                        {
                            var line = acc.Substring(0, idx).TrimEnd('\r');
                            if (textHandler != null) textHandler(c.Id, line);
                            acc = acc.Substring(idx + 1);
                        }

                        c.Acc.Clear();
                        c.Acc.Append(acc);
                    }
                    else
                    {
                        if (textHandler != null)
                        {
                            textHandler(c.Id, text);
                        }
                    }
                }
            }
        }
        catch (ObjectDisposedException)
        {
        }
        catch (SocketException)
        {
        }
        catch
        {
        }
        finally
        {
            RemoveClient(c);
        }
    }

    public bool Send(string clientId, string text)
    {
        Client c;
        if (!_clients.TryGetValue(clientId, out c)) return false;
        try
        {
            var data = _encoding.GetBytes(text);
            c.Socket.Send(data);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public bool Send(string clientId, byte[] data, int offset = 0, int count = -1)
    {
        Client c;
        if (!_clients.TryGetValue(clientId, out c)) return false;
        try
        {
            if (count < 0) count = data.Length - offset;
            c.Socket.Send(data, offset, count, SocketFlags.None);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public void Broadcast(string text)
    {
        var data = _encoding.GetBytes(text);
        foreach (var kv in _clients)
        {
            try
            {
                kv.Value.Socket.Send(data);
            }
            catch
            {
            }
        }
    }

    public void Broadcast(byte[] data)
    {
        foreach (var kv in _clients)
        {
            try
            {
                kv.Value.Socket.Send(data);
            }
            catch
            {
            }
        }
    }

    private void RemoveClient(Client c)
    {
        try
        {
            c.Socket.Shutdown(SocketShutdown.Both);
        }
        catch
        {
        }

        try
        {
            c.Socket.Close();
        }
        catch
        {
        }

        Client tmp;
        _clients.TryRemove(c.Id, out tmp);
        try
        {
            var h = ClientDisconnected;
            if (h != null) h(c.Id);
        }
        catch
        {
        }
    }

    public void Stop()
    {
        var t = _cts;
        _cts = null;
        try
        {
            t?.Cancel();
        }
        catch
        {
        }

        try
        {
            if (_listener != null) _listener.Close();
        }
        catch
        {
        }

        _listener = null;
        foreach (var kv in _clients.ToArray()) RemoveClient(kv.Value);
    }

    public void Dispose()
    {
        Stop();
    }
}