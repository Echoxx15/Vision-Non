using System;
using System.Collections.Generic;

namespace HardwareCommNet.CommTable;

/// <summary>
/// 通讯表行模型
/// </summary>
public sealed class CommCell
{
    public int Index { get; set; }
    public string Name { get; set; } = string.Empty;
    public CommValueType ValueType { get; set; } = CommValueType.Int;
    
    /// <summary>起始字节位置（TCP字符串拆分用，从0开始；Modbus不使用）</summary>
    public int StartByte { get; set; } = 0;
    
    /// <summary>字节长度/数量（Modbus读取长度、TCP截取长度）</summary>
    public int Length { get; set; } = 1;
    
    public string Address { get; set; } = string.Empty;
    public List<string> TriggerValues { get; set; } = new List<string>();
    public string Description { get; set; } = string.Empty;
    
    /// <summary>
    /// 是否为触发信号
    /// 标识该变量是否需要在触发后自动复位（写回0或false）
    /// </summary>
    public bool IsTrigger { get; set; } = false;
    
    /// <summary>实际的.NET类型（用于TypeValueUtil转换），根据ValueType自动计算</summary>
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