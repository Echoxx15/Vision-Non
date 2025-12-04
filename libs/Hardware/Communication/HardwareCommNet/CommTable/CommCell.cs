using System;
using System.Collections.Generic;

namespace HardwareCommNet.CommTable;

/// <summary>
/// 通讯变量模型
/// </summary>
public sealed class CommCell
{
    public int Index { get; set; }
    public string Name { get; set; } = string.Empty;
    public CommValueType ValueType { get; set; } = CommValueType.Int;
    
    /// <summary>起始字节位置（TCP字符串解析用，从0开始，Modbus不使用）</summary>
    public int StartByte { get; set; } = 0;
    
    /// <summary>字节长度/数量（Modbus读取长度、TCP读取长度）</summary>
    public int Length { get; set; } = 1;
    
    public string Address { get; set; } = string.Empty;
    public List<string> TriggerValues { get; set; } = new List<string>();
    public string Description { get; set; } = string.Empty;
    
    /// <summary>
    /// 是否为触发信号
    /// 标识该变量是否需要在触发后自动复位（写入0/false）
    /// </summary>
    public bool IsTrigger { get; set; } = false;
    
    /// <summary>
    /// 缓存的当前值（由轮询线程更新，外部读取时直接获取此值）
    /// </summary>
    public object CachedValue { get; set; }
    
    /// <summary>
    /// 缓存值的更新时间
    /// </summary>
    public DateTime CachedTime { get; set; }
    
    #region TCP 专用字段
    
    /// <summary>
    /// TCP字段索引（按分隔符拆分后的索引，从0开始）
    /// -1 表示不使用字段索引，使用整个消息
    /// </summary>
    public int FieldIndex { get; set; } = -1;
    
    /// <summary>
    /// TCP消息分隔符（用于拆分字段）
    /// </summary>
    public string Delimiter { get; set; } = ",";
    
    /// <summary>
    /// TCP消息结束符
    /// </summary>
    public string Terminator { get; set; } = "\r\n";
    
    /// <summary>
    /// 触发模式
    /// 0 = 值匹配触发（默认）：当值等于TriggerValues中的任意值时触发
    /// 1 = 值变化触发：当值发生变化时触发
    /// 2 = 任意非空触发：当收到任意非空消息时触发
    /// </summary>
    public int TriggerMode { get; set; } = 0;
    
    /// <summary>
    /// 是否使用正则表达式匹配
    /// </summary>
    public bool UseRegex { get; set; } = false;
    
    /// <summary>
    /// 正则表达式模式（当 UseRegex=true 时使用）
    /// </summary>
    public string RegexPattern { get; set; } = string.Empty;
    
    #endregion
    
    /// <summary>实际的.NET类型（根据TypeValueUtil转换，或根据ValueType自动生成）</summary>
    public Type RealType
    {
        get
        {
            return ValueType switch
            {
                CommValueType.Bool => typeof(bool),
                CommValueType.Int => typeof(int),
                CommValueType.Short => typeof(short),
                CommValueType.Float => typeof(float),
                CommValueType.Double => typeof(double),
                CommValueType.String => typeof(string),
                CommValueType.BoolArray => typeof(bool[]),
                CommValueType.IntArray => typeof(int[]),
                CommValueType.ShortArray => typeof(short[]),
                CommValueType.FloatArray => typeof(float[]),
                CommValueType.DoubleArray => typeof(double[]),
                CommValueType.StringArray => typeof(string[]),
                _ => typeof(object)
            };
        }
    }
}