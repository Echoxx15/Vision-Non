using System.Collections.Generic;

namespace Vision.Comm.TcpConfig;

/// <summary>
/// TCP通讯类型
/// </summary>
public enum TcpType
{
    /// <summary>
    /// TCP客户端（连接到远程服务器）
    /// </summary>
 Client,

    /// <summary>
    /// TCP服务器（监听客户端连接）
    /// </summary>
    Server
}

/// <summary>
/// TCP通讯配置模型
/// 保存到解决方案中
/// </summary>
public class TcpConfigModel
{
    /// <summary>
    /// 配置名称（唯一标识）
    /// 格式：TCP客户端1, TCP服务器1, TCP客户端2...
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// TCP类型（客户端/服务器）
    /// </summary>
  public TcpType Type { get; set; }

    /// <summary>
  /// IP地址
    /// 客户端：远程服务器IP
    /// 服务器：监听IP（0.0.0.0表示所有网卡）
    /// </summary>
    public string IpAddress { get; set; } = "127.0.0.1";

    /// <summary>
    /// 端口号
    /// </summary>
    public int Port { get; set; } = 5000;

    /// <summary>
/// 是否启用
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// 发送字节长度（0表示不限制）
    /// </summary>
    public int SendBufferSize { get; set; } = 0;

    /// <summary>
    /// 接收字节长度（0表示不限制）
    /// </summary>
    public int ReceiveBufferSize { get; set; } = 0;

    /// <summary>
    /// 编码方式
    /// </summary>
    public string Encoding { get; set; } = "UTF-8";

    /// <summary>
    /// 是否使用结束符
    /// </summary>
    public bool UseTerminator { get; set; } = false;

    /// <summary>
    /// 结束符（如 \r\n）
    /// </summary>
    public string Terminator { get; set; } = @"\r\n";

    /// <summary>
    /// 连接超时（毫秒，仅客户端）
    /// </summary>
  public int ConnectTimeout { get; set; } = 5000;

    /// <summary>
    /// 是否自动重连（仅客户端）
    /// </summary>
    public bool AutoReconnect { get; set; } = true;

    /// <summary>
    /// 重连间隔（毫秒）
    /// </summary>
public int ReconnectInterval { get; set; } = 3000;

    /// <summary>
    /// 最大客户端连接数（仅服务器）
    /// </summary>
    public int MaxConnections { get; set; } = 10;

    /// <summary>
    /// 备注
    /// </summary>
    public string Remark { get; set; } = string.Empty;
}

/// <summary>
/// TCP通讯配置集合
/// </summary>
public class TcpConfigCollection
{
    /// <summary>
    /// 所有TCP配置列表
    /// </summary>
    public List<TcpConfigModel> Configs { get; set; } = new List<TcpConfigModel>();

    /// <summary>
    /// 生成下一个唯一名称
    /// </summary>
    /// <param name="type">TCP类型</param>
    /// <returns>唯一名称</returns>
    public string GenerateUniqueName(TcpType type)
    {
        var prefix = type == TcpType.Client ? "TCP客户端" : "TCP服务器";
        int index = 1;

        while (true)
        {
            var name = $"{prefix}{index}";
            if (!Configs.Exists(c => c.Name == name))
            {
                return name;
            }

            index++;
        }
    }

    /// <summary>
    /// 添加配置
    /// </summary>
    public void Add(TcpConfigModel config)
    {
        if (config == null) return;

        // 确保名称唯一
        if (string.IsNullOrWhiteSpace(config.Name))
        {
            config.Name = GenerateUniqueName(config.Type);
        }
        else if (Configs.Exists(c => c.Name == config.Name))
        {
            config.Name = GenerateUniqueName(config.Type);
        }

        Configs.Add(config);
    }

    /// <summary>
    /// 删除配置
    /// </summary>
    public bool Remove(TcpConfigModel config)
    {
        return config != null && Configs.Remove(config);
    }

    /// <summary>
    /// 根据名称查找配置
    /// </summary>
    public TcpConfigModel FindByName(string name)
    {
        return Configs.Find(c => c.Name == name);
    }
}
