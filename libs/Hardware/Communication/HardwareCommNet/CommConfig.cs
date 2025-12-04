using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using HardwareCommNet.CommTable;

namespace HardwareCommNet;

/// <summary>
/// 通讯设备配置模型
/// </summary>
[Serializable]
public class CommConfig
{
    /// <summary>
    /// 设备名称
    /// </summary>
    [XmlElement("Name")]
    public string Name { get; set; }

    /// <summary>
    /// 设备类型（制造商名称，如 ModbusTcp）
    /// </summary>
    [XmlElement("Type")]
    public string Type { get; set; }

    /// <summary>
    /// 是否启用
    /// </summary>
  [XmlElement("Enabled")]
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// 配置参数（键值对形式存储，如 IP、Port 等）
    /// </summary>
    [XmlArray("Parameters")]
    [XmlArrayItem("Parameter")]
    public List<CommParameter> Parameters { get; set; } = new List<CommParameter>();

    /// <summary>
    /// 通讯表配置（输入/输出变量的持久化）
    /// </summary>
    [XmlElement("CommTableConfig")]
    public CommTableConfig TableConfig { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    [XmlElement("CreateTime")]
    public DateTime CreateTime { get; set; } = DateTime.Now;

  /// <summary>
    /// 最后修改时间
    /// </summary>
    [XmlElement("LastModifyTime")]
 public DateTime LastModifyTime { get; set; } = DateTime.Now;

    public CommConfig()
    {
    }

    public CommConfig(string name, string type)
    {
        Name = name;
    Type = type;
    }

    /// <summary>
    /// 获取参数值
    /// </summary>
    public string GetParameter(string key, string defaultValue = "")
    {
   var param = Parameters?.Find(p => string.Equals(p.Key, key, StringComparison.OrdinalIgnoreCase));
        return param?.Value ?? defaultValue;
    }

    /// <summary>
    /// 设置参数值
    /// </summary>
    public void SetParameter(string key, string value)
    {
        if (Parameters == null) Parameters = new List<CommParameter>();
        
     var param = Parameters.Find(p => string.Equals(p.Key, key, StringComparison.OrdinalIgnoreCase));
        if (param != null)
        {
    param.Value = value;
        }
   else
        {
            Parameters.Add(new CommParameter { Key = key, Value = value });
        }
        
        LastModifyTime = DateTime.Now;
    }
}

/// <summary>
/// 通讯参数键值对
/// </summary>
[Serializable]
public class CommParameter
{
    [XmlAttribute("Key")]
    public string Key { get; set; }

    [XmlAttribute("Value")]
    public string Value { get; set; }
}

/// <summary>
/// 通讯表配置（可序列化）
/// </summary>
[Serializable]
public class CommTableConfig
{
    [XmlArray("Inputs")]
    [XmlArrayItem("Cell")]
    public List<CommCellConfig> Inputs { get; set; } = new List<CommCellConfig>();

    [XmlArray("Outputs")]
    [XmlArrayItem("Cell")]
    public List<CommCellConfig> Outputs { get; set; } = new List<CommCellConfig>();
}

/// <summary>
/// CommCell 的可序列化版本
/// </summary>
[Serializable]
public class CommCellConfig
{
    [XmlElement("Name")]
    public string Name { get; set; }
    
    [XmlElement("ValueType")]
    public string ValueType { get; set; }
    
    [XmlElement("StartByte")]
    public int StartByte { get; set; }
    
    [XmlElement("Length")]
    public int Length { get; set; }
    
    [XmlElement("Address")]
    public string Address { get; set; }
    
    [XmlArray("TriggerValues")]
    [XmlArrayItem("Value")]
    public List<string> TriggerValues { get; set; } = new List<string>();
    
    [XmlElement("Description")]
    public string Description { get; set; }
    
    /// <summary>
    /// 是否为触发信号（支持自动复位）
    /// </summary>
    [XmlElement("IsTrigger")]
    public bool IsTrigger { get; set; } = false;
    
    #region TCP 专用字段
    
    /// <summary>
    /// TCP字段索引（按分隔符拆分后的索引，从0开始）
    /// -1 表示不使用字段索引，使用整个消息
    /// </summary>
    [XmlElement("FieldIndex")]
    public int FieldIndex { get; set; } = -1;
    
    /// <summary>
    /// TCP消息分隔符
    /// </summary>
    [XmlElement("Delimiter")]
    public string Delimiter { get; set; } = ",";
    
    /// <summary>
    /// TCP消息结束符
    /// </summary>
    [XmlElement("Terminator")]
    public string Terminator { get; set; } = "\r\n";
    
    /// <summary>
    /// 触发模式
    /// 0 = 值匹配触发（默认）
    /// 1 = 值变化触发
    /// 2 = 任意非空触发
    /// </summary>
    [XmlElement("TriggerMode")]
    public int TriggerMode { get; set; } = 0;
    
    /// <summary>
    /// 是否使用正则表达式匹配
    /// </summary>
    [XmlElement("UseRegex")]
    public bool UseRegex { get; set; } = false;
    
    /// <summary>
    /// 正则表达式模式
    /// </summary>
    [XmlElement("RegexPattern")]
    public string RegexPattern { get; set; } = string.Empty;
    
    #endregion
}

/// <summary>
/// 通讯配置集合（用于 XML 序列化）
/// </summary>
[Serializable]
[XmlRoot("CommConfigs")]
public class CommConfigCollection
{
    [XmlArray("Configs")]
    [XmlArrayItem("Config")]
 public List<CommConfig> Configs { get; set; } = new List<CommConfig>();
}
