using HslCommunication.Core;
using Logger;
using System;
using System.Linq;
using Vision.Solutions.Models;

namespace Vision.Comm.Modbus;

/// <summary>
/// Modbus通讯管理器（单例模式 - 单设备版本）
/// 针对实际应用场景优化：一个系统只连接一个 Modbus PLC
/// 
/// 设计理念：
/// - 单设备管理：使用单个字段而非字典
/// - 配置界面友好：支持断开/重连，无冲突
/// - 简洁高效：减少不必要的复杂度
/// </summary>
public sealed class ModbusManager
{
    private static readonly Lazy<ModbusManager> _instance = new(() => new ModbusManager());

    public static ModbusManager Instance => _instance.Value;

    #region 单设备字段

    /// <summary>
    /// 当前Modbus配置
    /// </summary>
    private ModbusConfig _config;

    /// <summary>
    /// 当前Modbus访问器
    /// </summary>
    private ModbusAccessor _accessor;

    #endregion

    private ModbusManager()
    {
     LogHelper.Info("Modbus管理器已初始化（单设备模式）");
    }

    #region 初始化和释放

    /// <summary>
    /// 从方案初始化Modbus连接（单设备模式）
    /// 只处理方案中的第一个Modbus配置
    /// </summary>
    public void InitializeFromSolution(Solution solution)
    {
        try
        {
            // 先清理旧连接
  DisposeAll();

            if (solution?.ModbusConfigs?.Configs == null || solution.ModbusConfigs.Configs.Count == 0)
            {
  LogHelper.Info("方案中没有Modbus配置");
             return;
       }

      // 单设备模式：只获取第一个配置
     _config = solution.ModbusConfigs.Configs[0];

    if (!_config.Enabled)
     {
     LogHelper.Info($"Modbus配置 [{_config.Name}] 未启用");
                return;
            }

 // 解析数据格式
 DataFormat dataFormat = _config.DataFormat switch
         {
     "ABCD" => DataFormat.ABCD,
       "CDAB" => DataFormat.CDAB,
    "BADC" => DataFormat.BADC,
    "DCBA" => DataFormat.DCBA,
      _ => DataFormat.CDAB
        };

         // 创建访问器
            _accessor = ModbusTcp.Get(
                _config.IpAddress,
       _config.Port,
       dataFormat,
     _config.Station,
        _config.ConnectTimeout,
    _config.StringReverse);

   // 尝试连接
            var result = _accessor.Connect();
            if (result.IsSuccess)
        {
      LogHelper.Info($"Modbus [{_config.Name}] 连接成功: {_config.IpAddress}:{_config.Port}");
            }
            else
            {
     LogHelper.Warn($"Modbus [{_config.Name}] 连接失败: {result.Message}");
       }
        }
   catch (Exception ex)
    {
        LogHelper.Error(ex, "从方案初始化Modbus失败");
        }
    }

    /// <summary>
    /// 释放所有连接（单设备模式）
    /// </summary>
    public void DisposeAll()
    {
   if (_accessor != null)
        {
            try
      {
       _accessor.Close();
                LogHelper.Info($"Modbus [{_config?.Name ?? "Unknown"}] 已断开");
            }
          catch (Exception ex)
            {
           LogHelper.Error(ex, "断开Modbus连接失败");
    }
        }

        _accessor = null;
   _config = null;
    }

    public void Dispose()
    {
        DisposeAll();
    LogHelper.Info("Modbus管理器已释放");
    }

    #endregion

    #region 状态查询

    /// <summary>
    /// 获取当前Modbus配置
    /// </summary>
  public ModbusConfig GetConfig()
    {
        return _config;
    }

    /// <summary>
 /// 获取当前Modbus访问器
 /// </summary>
    public ModbusAccessor GetAccessor()
    {
        return _accessor;
    }

    /// <summary>
  /// 检查当前连接是否有效
    /// </summary>
    public bool IsConnected()
    {
        return _accessor?.IsConnected == true;
    }

/// <summary>
    /// 获取当前配置名称
    /// </summary>
    public string GetCurrentName()
    {
        return _config?.Name ?? string.Empty;
    }

    #endregion

  #region 变量读写

    /// <summary>
 /// 读取变量值（按名称）
    /// </summary>
    public object ReadVariable(string variableName)
    {
        if (_config == null || _accessor == null)
  {
   LogHelper.Warn("Modbus未初始化");
      return null;
     }

      var variable = _config.Variables.FirstOrDefault(v =>
         string.Equals(v.Name, variableName, StringComparison.OrdinalIgnoreCase));

        if (variable == null)
      {
       LogHelper.Warn($"未找到变量: {variableName}");
       return null;
     }

      return ReadVariable(_accessor, variable);
    }

    /// <summary>
    /// 读取变量值（按变量对象）
    /// </summary>
    private object ReadVariable(ModbusAccessor accessor, ModbusVariable variable)
    {
   try
        {
            switch (variable.Type)
      {
        case ModbusDataType.Bool:
         return accessor.ReadBool(variable.Address, 1)[0];

   case ModbusDataType.Short:
         return accessor.ReadInt16(variable.Address);

              case ModbusDataType.Float:
          return accessor.ReadFloat(variable.Address);

                case ModbusDataType.String:
          return accessor.ReadString(variable.Address, (ushort)variable.Length);

      case ModbusDataType.BoolArray:
         return accessor.ReadBool(variable.Address, (ushort)variable.Length);

case ModbusDataType.ShortArray:
return accessor.ReadInt16(variable.Address, (ushort)variable.Length);

   case ModbusDataType.FloatArray:
           return accessor.ReadFloat(variable.Address, (ushort)variable.Length);

             case ModbusDataType.StringArray:
     // 字符串数组需要特殊处理
 var result = new string[variable.Length];
              for (int i = 0; i < variable.Length; i++)
      {
            result[i] = accessor.ReadString($"{variable.Address}+{i * 10}", 10);
  }
   return result;

 default:
        return null;
            }
      }
        catch (Exception ex)
     {
LogHelper.Error(ex, $"读取变量失败: {variable.Name}");
            return null;
        }
    }

    /// <summary>
    /// 写入变量值（按名称）
    /// </summary>
    public bool WriteVariable(string variableName, object value)
    {
        if (_config == null || _accessor == null)
        {
  LogHelper.Warn("Modbus未初始化");
       return false;
        }

        var variable = _config.Variables.FirstOrDefault(v =>
            string.Equals(v.Name, variableName, StringComparison.OrdinalIgnoreCase));

        if (variable == null)
        {
      LogHelper.Warn($"未找到变量: {variableName}");
   return false;
   }

     return WriteVariable(_accessor, variable, value);
    }

    /// <summary>
    /// 写入变量值（按变量对象）
    /// </summary>
    private bool WriteVariable(ModbusAccessor accessor, ModbusVariable variable, object value)
    {
        try
        {
   switch (variable.Type)
    {
   case ModbusDataType.Bool:
         return accessor.WriteBool(variable.Address, Convert.ToBoolean(value));

 case ModbusDataType.Short:
         return accessor.WriteInt16(variable.Address, Convert.ToInt16(value));

    case ModbusDataType.Float:
        return accessor.WriteFloat(variable.Address, Convert.ToSingle(value));

 case ModbusDataType.String:
        return accessor.WriteString(variable.Address, value?.ToString() ?? "");

                default:
                LogHelper.Warn($"不支持写入数组类型: {variable.Type}");
     return false;
       }
        }
        catch (Exception ex)
    {
            LogHelper.Error(ex, $"写入变量失败: {variable.Name}");
            return false;
        }
    }

    #endregion
}
