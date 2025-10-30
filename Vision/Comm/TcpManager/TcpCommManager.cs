using Logger;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using Vision.Comm.Sokect;
using Vision.Comm.TcpConfig;
using Vision.Solutions.Models;

namespace Vision.Comm.TcpManager;

/// <summary>
/// TCP通讯运行时实例
/// 包装实际的TCP客户端或服务器对象
/// </summary>
public class TcpRuntimeInstance
{
    /// <summary>
    /// 配置信息
    /// </summary>
    public TcpConfigModel Config { get; set; }

    /// <summary>
    /// TCP客户端实例（如果是客户端模式）
    /// </summary>
    public NetTcpClient Client { get; set; }

    /// <summary>
    /// TCP服务器实例（如果是服务器模式）
    /// </summary>
    public NetTcpServer Server { get; set; }

    /// <summary>
    /// 服务器是否已启动（手动标志）
    /// </summary>
    public bool ServerStarted { get; set; }

    /// <summary>
    /// 是否已连接/启动
    /// </summary>
    public bool IsActive
    {
        get
        {
       if (Config.Type == TcpType.Client)
   return Client?.IsConnected ?? false;
            else
return ServerStarted;
        }
    }

    /// <summary>
    /// 获取编码对象
    /// </summary>
    public Encoding GetEncoding()
    {
      try
    {
        return Encoding.GetEncoding(Config.Encoding);
        }
    catch
   {
        return Encoding.UTF8;
  }
    }
}

/// <summary>
/// TCP通讯管理器（单例）
/// 负责：
/// 1. 根据方案配置初始化所有TCP连接
/// 2. 管理TCP实例生命周期
/// 3. 统一消息事件分发
/// 4. 提供发送接口
/// </summary>
public sealed class TcpCommManager
{
  #region 单例模式

  private static readonly Lazy<TcpCommManager> _inst = new Lazy<TcpCommManager>(() => new TcpCommManager());
  public static TcpCommManager Instance => _inst.Value;

  #endregion

  #region 私有字段

  /// <summary>
  /// 所有TCP实例字典：名称 → 实例
  /// </summary>
  private readonly ConcurrentDictionary<string, TcpRuntimeInstance> _instances =
    new(StringComparer.OrdinalIgnoreCase);

  #endregion

  #region 事件

  /// <summary>
  /// TCP消息接收事件
  /// 参数：(TCP名称, 客户端ID, 消息内容)
  /// 客户端模式：clientId 为空字符串
  /// 服务器模式：clientId 为客户端标识
  /// </summary>
  public event Action<string, string, string> MessageReceived;

  /// <summary>
  /// TCP连接状态变化事件
  /// 参数：(TCP名称, 是否连接)
  /// </summary>
  public event Action<string, bool> ConnectionStateChanged;

  #endregion

  #region 私有构造函数

  private TcpCommManager()
  {
  }

  #endregion

  #region 初始化和释放

  /// <summary>
  /// 根据方案配置初始化所有TCP连接
  /// </summary>
  public void InitializeFromSolution(Solution solution)
  {
    if (solution?.TcpConfigs?.Configs == null)
    {
      return;
    }

    // 先释放所有现有连接
    DisposeAll();

    int successCount = 0;
    int totalCount = solution.TcpConfigs.Configs.Count(c => c != null && c.Enabled);
    
    if (totalCount == 0)
    {
      return;
    }

    foreach (var config in solution.TcpConfigs.Configs)
    {
      if (config == null || !config.Enabled)
        continue;

      try
      {
        if (config.Type == TcpType.Client)
     {
          InitializeClient(config);
        }
     else
        {
          InitializeServer(config);
        }
        successCount++;
      }
      catch (Exception ex)
  {
        LogHelper.Error(ex, $"TCP配置初始化失败: {config.Name}");
      }
    }

    if (successCount > 0)
    {
      LogHelper.Info($"TCP通讯初始化完成: 成功 {successCount}/{totalCount}");
 }
  }

  /// <summary>
  /// 初始化TCP客户端
  /// </summary>
  private void InitializeClient(TcpConfigModel config)
  {
    var client = new NetTcpClient(config.IpAddress, config.Port);

    // 设置编码
    try
 {
      client.TextEncoding = Encoding.GetEncoding(config.Encoding);
 }
catch
    {
   client.TextEncoding = Encoding.UTF8;
    }

    // 设置自动重连
    client.AutoReconnect = config.AutoReconnect;
    client.ReconnectDelayMs = config.ReconnectInterval;
 client.ConnectTimeoutMs = config.ConnectTimeout;

  // 应用结束符配置
    if (config.UseTerminator)
    {
      var terminator = ParseTerminator(config.Terminator);
      if (terminator == "\n" || terminator == "\r\n")
      {
        client.FrameByNewLine = true;
      }
      else if (!string.IsNullOrWhiteSpace(terminator))
      {
     LogHelper.Warn($"TCP客户端[{config.Name}]配置了自定义结束符，但仅支持换行符分帧");
   }
    }
    else
    {
      LogHelper.Warn($"TCP客户端[{config.Name}]未启用结束符分帧，可能导致消息不完整");
    }

    // 订阅消息接收事件
    client.TextReceived += (msg) => OnClientMessageReceived(config.Name, msg);

    // 订阅连接状态变化
    client.ConnectedChanged += (connected) => OnConnectionChanged(config.Name, connected);

    var instance = new TcpRuntimeInstance
    {
      Config = config,
  Client = client
    };

    _instances[config.Name] = instance;
  }

  /// <summary>
  /// 初始化TCP服务器
  /// </summary>
  private void InitializeServer(TcpConfigModel config)
  {
    var server = new NetTcpServer(config.IpAddress, config.Port);

    // 设置编码
    try
    {
      server.TextEncoding = Encoding.GetEncoding(config.Encoding);
    }
    catch
  {
      server.TextEncoding = Encoding.UTF8;
 }

    // 应用结束符配置
    if (config.UseTerminator)
    {
 var terminator = ParseTerminator(config.Terminator);
      if (terminator == "\n" || terminator == "\r\n")
      {
    server.FrameByNewLine = true;
      }
    else if (!string.IsNullOrWhiteSpace(terminator))
      {
        LogHelper.Warn($"TCP服务器[{config.Name}]配置了自定义结束符，但仅支持换行符分帧");
      }
    }
    else
    {
      LogHelper.Warn($"TCP服务器[{config.Name}]未启用结束符分帧，可能导致消息不完整");
    }

    // 订阅消息接收事件
    server.TextReceived += (clientId, msg) => OnServerMessageReceived(config.Name, clientId, msg);

    // 订阅客户端连接事件
    server.ClientConnected += (clientId) =>
    {
      LogHelper.Info($"TCP服务器[{config.Name}]客户端[{clientId}]已连接");
OnConnectionChanged(config.Name, true);
    };

    server.ClientDisconnected += (clientId) => 
    { 
      LogHelper.Info($"TCP服务器[{config.Name}]客户端[{clientId}]已断开"); 
    };

    var instance = new TcpRuntimeInstance
    {
      Config = config,
Server = server,
      ServerStarted = true
    };

    _instances[config.Name] = instance;
  }

  /// <summary>
  /// 释放所有TCP连接
  /// </summary>
  public void DisposeAll()
  {
    foreach (var kvp in _instances)
    {
      try
      {
        if (kvp.Value.Client != null)
        {
          kvp.Value.Client.Dispose();
          kvp.Value.Client = null;
        }

        if (kvp.Value.Server != null)
        {
          kvp.Value.Server.Stop();
          kvp.Value.Server = null;
          kvp.Value.ServerStarted = false;
        }

        LogHelper.Info($"TCP连接已释放: {kvp.Key}");
      }
      catch (Exception ex)
      {
        LogHelper.Error(ex, $"释放TCP连接异常: {kvp.Key}");
      }
    }

    _instances.Clear();
  }

  /// <summary>
  /// 释放指定TCP连接
  /// </summary>
  public void Dispose(string name)
  {
    if (_instances.TryRemove(name, out var instance))
    {
      try
      {
        instance.Client?.Dispose();
        instance.Server?.Stop();
        LogHelper.Info($"TCP连接已释放: {name}");
      }
      catch (Exception ex)
      {
        LogHelper.Error(ex, $"释放TCP连接异常: {name}");
      }
    }
  }

  #endregion

  #region 消息处理

  /// <summary>
  /// 客户端消息接收处理
  /// </summary>
  private void OnClientMessageReceived(string name, string msg)
  {
    try
    {
      MessageReceived?.Invoke(name, string.Empty, msg);
    }
    catch (Exception ex)
    {
      LogHelper.Error(ex, $"TCP客户端[{name}]消息处理异常");
    }
  }

  /// <summary>
  /// 服务器消息接收处理
  /// </summary>
  private void OnServerMessageReceived(string name, string clientId, string msg)
  {
    try
    {
      MessageReceived?.Invoke(name, clientId, msg);
    }
    catch (Exception ex)
    {
      LogHelper.Error(ex, $"TCP服务器[{name}]消息处理异常");
    }
  }

  /// <summary>
  /// 连接状态变化处理
  /// </summary>
  private void OnConnectionChanged(string name, bool connected)
  {
    try
    {
      ConnectionStateChanged?.Invoke(name, connected);
    }
    catch (Exception ex)
    {
      LogHelper.Error(ex, $"TCP连接状态变化事件处理异常: {name}");
    }
  }

  #endregion

  #region 发送消息

  /// <summary>
  /// 发送消息
  /// </summary>
  /// <param name="name">TCP配置名称</param>
  /// <param name="message">消息内容</param>
  /// <param name="clientId">客户端ID（仅服务器模式需要）</param>
  /// <returns>是否发送成功</returns>
  public bool Send(string name, string message, string clientId = "")
  {
    if (!_instances.TryGetValue(name, out var instance))
    {
      LogHelper.Warn($"TCP连接不存在: {name}");
      return false;
    }

    try
    {
      if (instance.Config.Type == TcpType.Client)
      {
        // 客户端模式
        if (instance.Client == null || !instance.Client.IsConnected)
        {
          LogHelper.Warn($"TCP客户端[{name}]未连接");
          return false;
        }

        instance.Client.Send(message);
        return true;
      }
      else
      {
        // 服务器模式
        if (instance.Server == null || !instance.ServerStarted)
        {
          LogHelper.Warn($"TCP服务器[{name}]未启动");
          return false;
        }

        if (string.IsNullOrEmpty(clientId))
        {
          // 广播给所有客户端
          instance.Server.Broadcast(message);
        }
        else
        {
          // 发送给指定客户端
          instance.Server.Send(clientId, message);
        }

        return true;
      }
    }
    catch (Exception ex)
    {
      LogHelper.Error(ex, $"TCP发送消息异常: {name}");
      return false;
    }
  }

  #endregion

  #region 查询接口

  /// <summary>
  /// 获取所有TCP实例
  /// </summary>
  public ConcurrentDictionary<string, TcpRuntimeInstance> GetAllInstances()
  {
    return _instances;
  }

  /// <summary>
  /// 获取指定TCP实例
  /// </summary>
  public TcpRuntimeInstance GetInstance(string name)
  {
    _instances.TryGetValue(name, out var instance);
    return instance;
  }

  /// <summary>
  /// 判断TCP是否已连接/启动
  /// </summary>
  public bool IsActive(string name)
  {
    if (_instances.TryGetValue(name, out var instance))
    {
      return instance.IsActive;
    }

    return false;
  }

  #endregion

  #region 辅助方法

  /// <summary>
  /// 解析结束符字符串
  /// 将转义符号转换为实际字符
  /// </summary>
  /// <param name="terminator">配置的结束符（如 \r\n）</param>
  /// <returns>实际的结束符字符串</returns>
  private static string ParseTerminator(string terminator)
  {
    if (string.IsNullOrEmpty(terminator))
      return string.Empty;

    // 处理常见的转义序列
    return terminator
      .Replace("\\r", "\r")
      .Replace("\\n", "\n")
      .Replace("\\t", "\t")
      .Replace("\\0", "\0");
  }

  #endregion
}
