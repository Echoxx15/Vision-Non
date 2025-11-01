using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Xml.Serialization;

namespace Vision.Comm.Modbus;

#region 枚举定义

/// <summary>
/// Modbus变量数据类型
/// </summary>
public enum ModbusDataType
{
    [Description("布尔型")]
    Bool = 0,
    
    [Description("短整型")]
    Short = 1,
    
    [Description("浮点型")]
    Float = 2,

  [Description("字符串")]
    String = 3,
    
    [Description("布尔数组")]
    BoolArray = 4,
    
    [Description("短整型数组")]
    ShortArray = 5,
    
    [Description("浮点数组")]
    FloatArray = 6,
    
    [Description("字符串数组")]
    StringArray = 7
}

/// <summary>
/// Modbus变量方向
/// </summary>
public enum ModbusDirection
{
    /// <summary>
    /// 输入变量（从PLC读取）
    /// </summary>
    Input = 0,
    
    /// <summary>
    /// 输出变量（写入到PLC）
    /// </summary>
    Output = 1
}

#endregion

#region Modbus配置

/// <summary>
/// Modbus连接配置
/// </summary>
[Serializable]
public class ModbusConfig
{
    /// <summary>
    /// 配置名称（唯一标识）
    /// </summary>
    [Category("基本信息"), DisplayName("配置名称")]
    [Description("Modbus配置的唯一标识名称")]
 public string Name { get; set; }
    
    /// <summary>
    /// 是否启用
    /// </summary>
    [Category("基本信息"), DisplayName("是否启用")]
    [Description("是否启用此Modbus配置")]
    public bool Enabled { get; set; } = true;
    
    /// <summary>
    /// IP地址
  /// </summary>
    [Category("连接配置"), DisplayName("IP地址")]
    [Description("PLC或Modbus设备的IP地址")]
 public string IpAddress { get; set; } = "127.0.0.1";
 
    /// <summary>
  /// 端口号
    /// </summary>
[Category("连接配置"), DisplayName("端口号")]
    [Description("Modbus TCP端口，通常为502")]
  public int Port { get; set; } = 502;

    /// <summary>
    /// 站号
    /// </summary>
    [Category("连接配置"), DisplayName("站号")]
    [Description("Modbus从站号，通常为1")]
    public byte Station { get; set; } = 1;
    
    /// <summary>
    /// 连接超时（毫秒）
    /// </summary>
    [Category("连接配置"), DisplayName("连接超时(ms)")]
    [Description("连接超时时间，单位毫秒")]
    public int ConnectTimeout { get; set; } = 5000;
    
    /// <summary>
    /// 接收超时（毫秒）
    /// </summary>
    [Category("连接配置"), DisplayName("接收超时(ms)")]
    [Description("接收超时时间，单位毫秒")]
    public int ReceiveTimeout { get; set; } = 10000;
    
    /// <summary>
    /// 数据格式
    /// </summary>
    [Category("数据格式"), DisplayName("字节序")]
    [Description("数据字节序：ABCD, CDAB, BADC, DCBA")]
    public string DataFormat { get; set; } = "CDAB";
    
    /// <summary>
    /// 字符串是否反转
  /// </summary>
    [Category("数据格式"), DisplayName("字符串反转")]
    [Description("字符串是否需要反转")]
    public bool StringReverse { get; set; }
    
    /// <summary>
    /// 备注
    /// </summary>
    [Category("其他"), DisplayName("备注")]
    [Description("备注信息")]
    public string Remark { get; set; } = string.Empty;
    
    public override string ToString()
    {
        return $"{Name} ({IpAddress}:{Port})";
    }
}

#endregion
