using System;
using System.Collections.Concurrent;
using HslCommunication;
using HslCommunication.Core;
using HslCommunication.ModBus;

namespace Vision.Comm.Modbus;

/// <summary>
/// Modbus TCP 常用选项（用于构建或复用底层 ModbusTcpNet）。
/// </summary>
internal sealed class ModbusTcpOptions
{
    /// <summary>目标主机</summary>
    public string Host { get; set; }
    /// <summary>端口（默认 502）</summary>
    public int Port { get; set; } = 502;
    /// <summary>站号（默认 1）</summary>
    public byte Station { get; set; } = 1;
    /// <summary>连接超时（毫秒）</summary>
    public int ConnectTimeoutMs { get; set; } = 5000;
    /// <summary>字节序</summary>
    public DataFormat DataFormat { get; set; } = DataFormat.CDAB;
    /// <summary>字符串是否反转</summary>
    public bool StringReverse { get; set; } = false;

    /// <summary>用于缓存键值（同配置共用一个底层连接）。</summary>
    public string Key => string.Format("{0}:{1}:{2}:{3}:{4}", Host, Port, Station, (int)DataFormat, StringReverse ? 1 : 0);
}

/// <summary>
/// Modbus TCP 工厂：按选项缓存底层 <see cref="ModbusTcpNet"/>，避免重复创建。
/// </summary>
internal static class ModbusTcpFactory
{
    private static readonly ConcurrentDictionary<string, ModbusTcpNet> Cache = new ConcurrentDictionary<string, ModbusTcpNet>(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// 获取或创建 ModbusTcpNet；首次创建会根据选项配置 DataFormat/IsStringReverse 并尝试连接一次。
    /// </summary>
    public static ModbusTcpNet GetOrCreate(ModbusTcpOptions opt)
    {
        if (opt == null) throw new ArgumentNullException(nameof(opt));
        if (string.IsNullOrWhiteSpace(opt.Host)) throw new ArgumentException("Host 不能为空");
        return Cache.GetOrAdd(opt.Key, _ => CreateNew(opt));
    }

    private static ModbusTcpNet CreateNew(ModbusTcpOptions opt)
    {
        var cli = new ModbusTcpNet(opt.Host, opt.Port, opt.Station)
        {
            ConnectTimeOut = opt.ConnectTimeoutMs,
            DataFormat = opt.DataFormat,
            IsStringReverse = opt.StringReverse
        };
        try { cli.ConnectServer(); } catch { } // 连接失败不抛，调用方可重试
        return cli;
    }
}

/// <summary>
/// 强类型 Modbus 访问器：封装常见读写，避免调用方传错类型。
/// </summary>
public sealed class ModbusAccessor
{
    private readonly ModbusTcpNet _client;
    public ModbusAccessor(ModbusTcpNet client) { _client = client; }

    /// <summary>是否已连接（来自 Hsl 的 IsConnectSuccess）。</summary>
    public bool IsConnected => _client != null && _client.ConnectionId !="";
    /// <summary>尝试连接。</summary>
    public OperateResult Connect() => _client.ConnectServer();
    /// <summary>关闭连接。</summary>
    public void Close() { try { _client.ConnectClose(); } catch { } }

    // Bool
    public bool WriteBool(string address, bool value) => _client.Write(address, value).IsSuccess;
    public bool[] ReadBool(string address, ushort length) => _client.ReadBool(address, length).Content;

    // Int16/UInt16
    public bool WriteInt16(string address, short value) => _client.Write(address, value).IsSuccess;
    public short ReadInt16(string address) => _client.ReadInt16(address).Content;
    public short[] ReadInt16(string address, ushort length) => _client.ReadInt16(address, length).Content;

    public bool WriteUInt16(string address, ushort value) => _client.Write(address, value).IsSuccess;
    public ushort ReadUInt16(string address) => _client.ReadUInt16(address).Content;
    public ushort[] ReadUInt16(string address, ushort length) => _client.ReadUInt16(address, length).Content;

    // Int32/UInt32
    public bool WriteInt32(string address, int value) => _client.Write(address, value).IsSuccess;
    public int ReadInt32(string address) => _client.ReadInt32(address).Content;
    public int[] ReadInt32(string address, ushort length) => _client.ReadInt32(address, length).Content;

    public bool WriteUInt32(string address, uint value) => _client.Write(address, value).IsSuccess;
    public uint ReadUInt32(string address) => _client.ReadUInt32(address).Content;
    public uint[] ReadUInt32(string address, ushort length) => _client.ReadUInt32(address, length).Content;

    // Int64/UInt64
    public bool WriteInt64(string address, long value) => _client.Write(address, value).IsSuccess;
    public long ReadInt64(string address) => _client.ReadInt64(address).Content;
    public long[] ReadInt64(string address, ushort length) => _client.ReadInt64(address, length).Content;

    public bool WriteUInt64(string address, ulong value) => _client.Write(address, value).IsSuccess;
    public ulong ReadUInt64(string address) => _client.ReadUInt64(address).Content;
    public ulong[] ReadUInt64(string address, ushort length) => _client.ReadUInt64(address, length).Content;

    // 浮点
    public bool WriteFloat(string address, float value) => _client.Write(address, value).IsSuccess;
    /// <summary>批量写入 float[]（常用于坐标阵列回写）。</summary>
    public bool WriteFloat(string address, float[] values) => _client.Write(address, values).IsSuccess;
    public float ReadFloat(string address) => _client.ReadFloat(address).Content;
    public float[] ReadFloat(string address, ushort length) => _client.ReadFloat(address, length).Content;

    // 双精度
    public bool WriteDouble(string address, double value) => _client.Write(address, value).IsSuccess;
    public double ReadDouble(string address) => _client.ReadDouble(address).Content;
    public double[] ReadDouble(string address, ushort length) => _client.ReadDouble(address, length).Content;

    // 字符串
    public bool WriteString(string address, string value) => _client.Write(address, value).IsSuccess;
    public string ReadString(string address, ushort length) => _client.ReadString(address, length).Content;
}

/// <summary>
/// 对外入口：通过参数快速获取强类型访问器（内部缓存底层连接）。
/// </summary>
internal static class ModbusTcp
{
    /// <summary>
    /// 获取访问器。相同参数将复用底层连接（减少重复创建）。
    /// </summary>
    public static ModbusAccessor Get(string host, int port, DataFormat fmt = DataFormat.CDAB, byte station = 1, int connectTimeoutMs = 5000, bool stringReverse = false)
    {
        var opt = new ModbusTcpOptions
        {
            Host = host,
            Port = port,
            Station = station,
            ConnectTimeoutMs = connectTimeoutMs,
            DataFormat = fmt,
            StringReverse = stringReverse
        };
        var cli = ModbusTcpFactory.GetOrCreate(opt);
        return new ModbusAccessor(cli);
    }
}