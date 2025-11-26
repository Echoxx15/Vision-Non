using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using HardwareCommNet;
using HslCommunication.ModBus;
using ModbusControlNet.UI;
using HardwareCommNet.CommTable;

namespace ModbusControlNet;

[CommManufacturer("ModbusTcp")]
public class ModbusTcp : CommAdapterBase
{
    #region 属性

    /// <summary>
    /// Modbus 客户端实例
    /// </summary>
    private ModbusTcpNet ModbusClient { get; set; }

    // 配置参数
    private string IpAddress { get; set; } = "127.0.0.1";
    private int Port { get; set; } = 502;
    private byte Station { get; set; } = 1;

    /// <summary>
    /// 是否已连接（重写基类属性）
    /// </summary>
    public override bool IsConnected { get; protected set; }

    private CommTable Table => base.Table; // reuse base table instance

    #endregion

    #region 诊断与轮询控制

    //轮询间隔
    private readonly int PollIntervalMs = 50;

    //读写计数（用于定位是否存在后台写入）
    private long _readOps;
    private long _writeOps;
    public (long reads, long writes) GetIoStats() => (Interlocked.Read(ref _readOps), Interlocked.Read(ref _writeOps));
    public void DumpIoStats(string prefix = null)
    {
        var (r, w) = GetIoStats();
        Console.WriteLine($"[{Name}] IO统计{(string.IsNullOrWhiteSpace(prefix) ? "" : $"({prefix})")}: Reads={r}, Writes={w}");
    }

    #endregion

    #region 私有字段

    private uConfigControl _configControl;
    private Thread _pollThread;
    private volatile bool _polling;

    #endregion

    #region 构造函数

    /// <summary>
    /// 初始化 Modbus TCP适配器
    /// </summary>
    public ModbusTcp(string name = "ModbusTcp") : base(name)
    {
    }

    #endregion

    #region 重写基类方法

    /// <summary>
    /// 获取配置控件
    /// </summary>
    public override UserControl GetConfigControl()
    {
        // 如果配置控件未创建，则创建新的
        if (_configControl == null || _configControl.IsDisposed)
        {
            _configControl = new uConfigControl();
            _configControl.SetDevice(this); //绑定设备实例
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
            // 创建 Modbus 客户端
            if (ModbusClient == null)
            {
                ModbusClient = new ModbusTcpNet(IpAddress, Port, Station);
            }

            // 尝试连接
            var result = ModbusClient.ConnectServer();
            if (!result.IsSuccess)
            {
                Console.WriteLine($"Modbus连接失败: {result.Message}");
                return;
            }

            IsConnected = true;

            // ✅ 使用基类方法触发事件
            OnConnectionStatusChanged(true);

            Console.WriteLine($"[{Name}] Modbus连接成功: {IpAddress}:{Port}");
            StartPolling();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[{Name}] Modbus连接异常: {ex.Message}");
            IsConnected = false;
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
            StopPolling();
            ModbusClient?.ConnectClose();
            IsConnected = false;

            // ✅ 使用基类方法触发事件
            OnConnectionStatusChanged(false);

            Console.WriteLine($"[{Name}] Modbus已断开连接");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[{Name}] Modbus断开连接异常: {ex.Message}");
        }
    }

    private void StartPolling()
    {
        if (_polling) return;
        _polling = true;
        _readOps = 0;
        _writeOps = 0;
        _pollThread = new Thread(PollLoop) { IsBackground = true, Name = $"ModbusPoll_{Name}" };
        _pollThread.Start();
        Console.WriteLine($"[{Name}]轮询线程已启动");
    }
    private void StopPolling()
    {
        _polling = false;
        if (_pollThread != null && _pollThread.IsAlive) _pollThread.Join(500);
        Console.WriteLine($"[{Name}]轮询线程已停止");
    }

    private void PollLoop()
    {
        while (_polling)
        {
            try
            {
                if (!IsConnected) { Thread.Sleep(PollIntervalMs); continue; }

                foreach (var cell in Table.Inputs)
                {
                    object value = ReadCellValue(cell);
                    Interlocked.Increment(ref _readOps);
                    if (IsNonZero(value))
                    {
                        //先发布事件（让上层尽快收到)
                        var payload = new { cell.Name, Value = ValueToString(value), RawValue = value };
                        OnMessageReceived(payload);

                        // 日志紧随其后输出（注意上层可能异步处理，不会阻塞)
                        Logger.LogHelper.Info($"Received value for {cell.Name}: {ValueToString(value)}");

                        // 自动复位：发布事件后延时50ms写0，避免阻塞轮询
                        if (cell.IsTrigger)
                        {
                            Task.Run(() =>
                            {
                                try
                                {
                                    Thread.Sleep(50);
                                    switch (cell.ValueType)
                                    {
                                        case CommValueType.Bool:
                                            ModbusClient.Write(cell.Address, false);
                                            break;
                                        case CommValueType.Short:
                                            ModbusClient.Write(cell.Address, (short)0);
                                            break;
                                        case CommValueType.Int:
                                            ModbusClient.Write(cell.Address, 0);
                                            break;
                                        case CommValueType.Float:
                                            ModbusClient.Write(cell.Address, 0f);
                                            break;
                                        case CommValueType.Double:
                                            ModbusClient.Write(cell.Address, 0d);
                                            break;
                                    }

                                    Interlocked.Increment(ref _writeOps);
                                    Console.WriteLine($"[{Name}] AutoReset(50ms) -> {cell.Address} (type={cell.ValueType}) =0");
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine($"[{Name}] AutoReset 写入异常: {ex.Message}");
                                }
                            });
                        }
                    }
                }
            }
            catch { }
            Thread.Sleep(PollIntervalMs);
        }
    }

    /// <summary>
    /// 写入数据到指定地址
    /// </summary>
    public override void Write(string address, object data)
    {
        // ✅ 使用基类的验证方法
        ValidateNotEmpty(address, nameof(address));
        ValidateNotNull(data, nameof(data));

        if (!IsConnected || ModbusClient == null)
        {
            Console.WriteLine($"[{Name}] Modbus未连接，无法写入");
            return;
        }

        try
        {
            switch (data)
            {
                case bool b: ModbusClient.Write(address, b); break;
                case short s: ModbusClient.Write(address, s); break;
                case ushort us: ModbusClient.Write(address, us); break;
                case int i: ModbusClient.Write(address, i); break;
                case uint ui: ModbusClient.Write(address, ui); break;
                case float f: ModbusClient.Write(address, f); break;
                case double d: ModbusClient.Write(address, (float)d); break; // Modbus 可用float
                case string str: ModbusClient.Write(address, str); break;
                case bool[] boolArray: ModbusClient.Write(address, boolArray); break;
                case int[] intArray: ModbusClient.Write(address, intArray); break;
                case float[] floatArray: ModbusClient.Write(address, floatArray); break;
                default: throw new NotSupportedException($"不支持类型 {data.GetType().Name}");
            }
            Interlocked.Increment(ref _writeOps);
            Console.WriteLine($"[{Name}] Write -> {address}, value=({data.GetType().Name}) {ValueToString(data)}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[{Name}] 写入异常: {ex.Message}");
        }
    }

    /// <summary>
    /// 写入数组数据到指定地址
    /// </summary>
    public override void Write(string address, object[] data)
    {
        // ✅ 使用基类的验证方法
        ValidateNotEmpty(address, nameof(address));
        ValidateNotEmpty(data, nameof(data));

        // 简化: 写第一元素
        Write(address, data[0]);
    }

    /// <summary>
    /// 获取当前配置（包含通讯表）
    /// </summary>
    public override CommConfig GetConfig()
    {
        //先让基类序列化通讯表
        var config = base.GetConfig();
        // 再写入本设备特有参数
        config.SetParameter("IpAddress", IpAddress);
        config.SetParameter("Port", Port.ToString());
        config.SetParameter("Station", Station.ToString());
        // 不保存 IsConnected以避免自动重连引起的杂乱日志
        return config;
    }

    /// <summary>
    /// 应用配置（先应用通讯表，再应用本设备参数）
    /// </summary>
    public override void ApplyConfig(CommConfig config)
    {
        if (config == null) return;

        //先让基类恢复通讯表
        base.ApplyConfig(config);

        // 应用本设备参数
        IpAddress = config.GetParameter("IpAddress", "127.0.0.1");
        if (int.TryParse(config.GetParameter("Port", "502"), out var port)) Port = port;
        if (byte.TryParse(config.GetParameter("Station", "1"), out var station)) Station = station;

        // 不在此处自动连接，交由工厂在 ApplyConfig之后调用 Connect()
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
                StopPolling();
                ModbusClient?.ConnectClose();
                ModbusClient = null;
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

    private object ReadCellValue(CommCell cell)
    {
        try
        {
            // Modbus 只使用 Length，不使用 StartByte
            int len = cell.Length > 0 ? cell.Length : 1;

            switch (cell.ValueType)
            {
                case CommValueType.Bool:
                {
                    var r = ModbusClient.ReadBool(cell.Address);
                    return r.IsSuccess && r.Content;
                }
                case CommValueType.Short:
                {
                    var r = ModbusClient.ReadInt16(cell.Address);
                    return r.IsSuccess ? r.Content : (short)0;
                }
                case CommValueType.Int:
                {
                    var r = ModbusClient.ReadInt32(cell.Address);
                    return r.IsSuccess ? r.Content : 0;
                }
                case CommValueType.Float:
                {
                    var r = ModbusClient.ReadFloat(cell.Address);
                    return r.IsSuccess ? r.Content : 0f;
                }
                case CommValueType.Double:
                {
                    var r = ModbusClient.ReadDouble(cell.Address);
                    return r.IsSuccess ? r.Content : 0d;
                }
                case CommValueType.String:
                {
                    var r = ModbusClient.ReadString(cell.Address, (ushort)len);
                    return r.IsSuccess ? r.Content : string.Empty;
                }
                case CommValueType.BoolArray:
                {
                    var r = ModbusClient.ReadBool(cell.Address, (ushort)len);
                    return r.IsSuccess ? r.Content : [];
                }
                case CommValueType.ShortArray:
                {
                    var r = ModbusClient.ReadInt16(cell.Address, (ushort)len);
                    return r.IsSuccess ? r.Content : [];
                }
                case CommValueType.IntArray:
                {
                    var r = ModbusClient.ReadInt32(cell.Address, (ushort)len);
                    return r.IsSuccess ? r.Content : [];
                }
                case CommValueType.FloatArray:
                {
                    var r = ModbusClient.ReadFloat(cell.Address, (ushort)len);
                    return r.IsSuccess ? r.Content : [];
                }
                case CommValueType.DoubleArray:
                {
                    var r = ModbusClient.ReadDouble(cell.Address, (ushort)len);
                    return r.IsSuccess ? r.Content : [];
                }
                default:
                    return null;
            }
        }
        catch
        {
            return null;
        }
    }

    private bool IsNonZero(object v)
    {
        if (v == null) return false;
        // 使用 ValueToString 判断非零
        var str = ValueToString(v);
        if (string.IsNullOrEmpty(str) || str == "0" || str == "False" || str == "false") return false;
        return true;
    }

    private string ValueToString(object value)
    {
        if (value == null) return string.Empty;
        if (value is Array arr) return $"[{string.Join(",", arr.Cast<object>().Take(10))}]";
        return Convert.ToString(value, System.Globalization.CultureInfo.InvariantCulture);
    }
}