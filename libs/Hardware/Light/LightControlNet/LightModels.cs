using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace LightControlNet;

#region 枚举定义

/// <summary>
/// 光源控制器类型（品牌）
/// </summary>
public enum LightControllerType
{
    [Description("孚根")] Fgen = 0,

    [Description("奥普特")] Opt = 1
}

#endregion

#region 光源配置

/// <summary>
/// 光源配置,保存到本地xml文件
/// </summary>
[Serializable]
public class LightConfig
{
  [Category("基本信息"), DisplayName("配置名称")]
    [Description("光源配置的唯一标识名称")]
    public string Name { get; set; }

    [Category("基本信息"), DisplayName("控制器类型")]
    [Description("光源控制器品牌：孚根、奥普特等")]
    public LightControllerType Type { get; set; } = LightControllerType.Fgen;

    [Category("基本信息"), DisplayName("是否启用")]
    [Description("是否启用此光源控制")]
    public bool Enabled { get; set; } = true;

    [Category("串口参数"), DisplayName("端口号")]
    [Description("串口号，如COM1、COM3等")]
    public string PortName { get; set; } = "COM1";

  [Category("串口参数"), DisplayName("波特率")]
    [Description("通讯波特率，通常为9600")]
    public int BaudRate { get; set; } = 9600;

    [Category("串口参数"), DisplayName("数据位")]
 [Description("数据位数，通常为8")]
    public int DataBits { get; set; } = 8;

    [Category("串口参数"), DisplayName("停止位")]
    [Description("停止位：1, 1.5, 2")]
    public double StopBits { get; set; } = 1;

    [Category("串口参数"), DisplayName("校验位")]
    [Description("校验位：None, Odd, Even")]
    public string Parity { get; set; } = "None";

    [Category("硬件参数"), DisplayName("通道数量")]
    [Description("光源控制器的通道数量：2/4/8")]
    public int ChannelCount { get; set; } = 4;

    [Category("备注"), DisplayName("备注")]
    [Description("备注信息")]
    public string Remark { get; set; } = string.Empty;

    public override string ToString()
    {
        return $"{Name} ({Type}, {PortName})";
    }
}

/// <summary>
/// 光源配置集合
/// </summary>
[Serializable]
public class LightConfigCollection
{
    public List<LightConfig> Configs { get; set; } = new List<LightConfig>();

    /// <summary>
    /// 生成唯一配置名称
    /// </summary>
    /// <param name="type">控制器类型</param>
    /// <returns>唯一名称</returns>
    public string GenerateUniqueName(LightControllerType type)
    {
        var prefix = type == LightControllerType.Fgen ? "飞格光源" : "奥普特光源";
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

    public void Add(LightConfig config)
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

    public bool Remove(LightConfig config)
    {
        return config != null && Configs.Remove(config);
    }

    public LightConfig FindByName(string name)
    {
        return Configs.FirstOrDefault(c =>
            string.Equals(c.Name, name, StringComparison.OrdinalIgnoreCase));
    }
}

#endregion

