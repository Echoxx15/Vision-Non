using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Forms;
using HardwareCommNet.CommTable;

namespace HardwareCommNet;

public abstract class CommAdapterBase : IComm, IRenameableComm, IConfigurableComm
{
    public CommTable.CommTable Table { get; } = new CommTable.CommTable();
    public string Name { get; private set; }
    public virtual bool IsConnected { get; protected set; }

    public event EventHandler<object> MessageReceived;
    public event EventHandler<bool> ConnectionStatusChanged;

    protected CommAdapterBase(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("设备名称不能为空", nameof(name));
        Name = name.Trim();
    }

    public abstract UserControl GetConfigControl();
    public abstract void Connect();
    public abstract void Disconnect();
    public abstract void Write(string address, object data);
    public abstract void Write(string address, object[] data);

    public virtual CommConfig GetConfig()
    {
        var attr = GetType().GetCustomAttribute<CommManufacturerAttribute>();
        var config = new CommConfig(Name, attr?.ManufacturerName ?? GetType().Name);

        Console.WriteLine($"[{Name}] GetConfig: 开始序列化配置...");

        if (Table != null)
        {
            var inputCount = Table.Inputs?.Count ?? 0;
            var outputCount = Table.Outputs?.Count ?? 0;

            Console.WriteLine($"[{Name}] Table状态: 输入{inputCount}个, 输出{outputCount}个");

            if (inputCount > 0 || outputCount > 0)
            {
                config.TableConfig = new CommTableConfig();

                foreach (var input in Table.Inputs)
                {
                    config.TableConfig.Inputs.Add(new CommCellConfig
                    {
                        Name = input.Name,
                        ValueType = input.ValueType.ToString(),
                        StartByte = input.StartByte,
                        Length = input.Length,
                        Address = input.Address,
                        TriggerValues = new List<string>(input.TriggerValues ?? new List<string>()),
                        Description = input.Description,
                        IsTrigger = input.IsTrigger,
                        FieldIndex = input.FieldIndex,
                        UseRegex = input.UseRegex,
                        RegexPattern = input.RegexPattern
                    });
                }

                foreach (var output in Table.Outputs)
                {
                    config.TableConfig.Outputs.Add(new CommCellConfig
                    {
                        Name = output.Name,
                        ValueType = output.ValueType.ToString(),
                        StartByte = output.StartByte,
                        Length = output.Length,
                        Address = output.Address,
                        TriggerValues = new List<string>(output.TriggerValues ?? new List<string>()),
                        Description = output.Description,
                        IsTrigger = output.IsTrigger,
                        FieldIndex = output.FieldIndex,
                        UseRegex = output.UseRegex,
                        RegexPattern = output.RegexPattern
                    });
                }

                Console.WriteLine(
                    $"[{Name}] ✓ TableConfig已创建: 输入{config.TableConfig.Inputs.Count}个, 输出{config.TableConfig.Outputs.Count}个");
            }
            else
            {
                Console.WriteLine($"[{Name}] ⚠ Table为空，不创建TableConfig");
            }
        }
        else
        {
            Console.WriteLine($"[{Name}] ⚠ Table为null");
        }

        return config;
    }

    public virtual void ApplyConfig(CommConfig config)
    {
        if (config == null)
        {
            Console.WriteLine($"[{Name}] ApplyConfig: config为null");
            return;
        }

        Console.WriteLine($"[{Name}] 开始应用配置...");

        if (config.TableConfig != null && Table != null)
        {
            Console.WriteLine($"[{Name}] 发现TableConfig，开始加载通讯表...");
            Table.ClearInputs();
            Table.ClearOutputs();

            int inputCount = 0;
            foreach (var cellCfg in config.TableConfig.Inputs ?? new List<CommCellConfig>())
            {
                if (Enum.TryParse<CommTable.CommValueType>(cellCfg.ValueType, out var vt))
                {
                    Table.AddOrUpdateInput(new CommTable.CommCell
                    {
                        Name = cellCfg.Name,
                        ValueType = vt,
                        StartByte = cellCfg.StartByte,
                        Length = cellCfg.Length > 0 ? cellCfg.Length : 1,
                        Address = cellCfg.Address,
                        TriggerValues = new List<string>(cellCfg.TriggerValues ?? new List<string>()),
                        Description = cellCfg.Description,
                        IsTrigger = cellCfg.IsTrigger,
                        // TCP特有字段
                        FieldIndex = cellCfg.FieldIndex,
                        Delimiter = cellCfg.Delimiter ?? ",",
                        Terminator = cellCfg.Terminator ?? "\r\n",
                        TriggerMode = cellCfg.TriggerMode
                    });
                    inputCount++;
                }
                else
                {
                    Console.WriteLine($"[{Name}] 警告：无法解析ValueType: {cellCfg.ValueType}");
                }
            }

            int outputCount = 0;
            foreach (var cellCfg in config.TableConfig.Outputs ?? new List<CommCellConfig>())
            {
                if (Enum.TryParse<CommTable.CommValueType>(cellCfg.ValueType, out var vt))
                {
                    Table.AddOrUpdateOutput(new CommTable.CommCell
                    {
                        Name = cellCfg.Name,
                        ValueType = vt,
                        StartByte = cellCfg.StartByte,
                        Length = cellCfg.Length > 0 ? cellCfg.Length : 1,
                        Address = cellCfg.Address,
                        TriggerValues = new List<string>(cellCfg.TriggerValues ?? new List<string>()),
                        Description = cellCfg.Description,
                        IsTrigger = cellCfg.IsTrigger,
                        // TCP特有字段
                        FieldIndex = cellCfg.FieldIndex,
                        Delimiter = cellCfg.Delimiter ?? ",",
                        Terminator = cellCfg.Terminator ?? "\r\n",
                        TriggerMode = cellCfg.TriggerMode
                    });
                    outputCount++;
                }
                else
                {
                    Console.WriteLine($"[{Name}] 警告：无法解析ValueType: {cellCfg.ValueType}");
                }
            }

            Console.WriteLine($"[{Name}] ✓ 通讯表加载完成: 输入{inputCount}个, 输出{outputCount}个");
        }
        else
        {
            if (config.TableConfig == null)
                Console.WriteLine($"[{Name}] config.TableConfig为null");
            if (Table == null)
                Console.WriteLine($"[{Name}] Table为null");
        }
    }

    private int ParseByteSpecSize(string spec)
    {
        if (string.IsNullOrWhiteSpace(spec)) return 1;
        var s = spec.Trim();
        if (int.TryParse(s, out var single) && single >= 1) return single;
        var parts = s.Split(new[] { '-', '~' }, StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 2 && int.TryParse(parts[0], out var a) && int.TryParse(parts[1], out var b) && a >= 1 &&
            b >= a)
            return b - a + 1;
        return 1;
    }

    protected virtual void OnMessageReceived(object message)
    {
        try
        {
            MessageReceived?.Invoke(this, message);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[{Name}] 消息接收事件处理异常: {ex.Message}");
        }
    }

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

    public virtual void SetName(string name)
    {
        if (!string.IsNullOrWhiteSpace(name)) Name = name.Trim();
    }

    private bool _disposed = false;

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
                Disconnect();
            }
            catch
            {
            }
        }

        _disposed = true;
    }

    protected void ValidateNotNull(object value, string paramName)
    {
        if (value == null) throw new ArgumentNullException(paramName);
    }

    protected void ValidateNotEmpty(string value, string paramName)
    {
        if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException($"{paramName} 不能为空", paramName);
    }

    protected void ValidateNotEmpty<T>(T[] value, string paramName)
    {
        if (value == null || value.Length == 0) throw new ArgumentException($"{paramName} 不能为空", paramName);
    }
}
