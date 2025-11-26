using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Vision.Solutions.Models;

namespace Vision.LightSource;

#region 枚举定义

/// <summary>
/// 光源控制器类型（品牌）
/// </summary>
public enum LightControllerType 
{ 
    [Description("孚根")]
    Fgen = 0, 
    
    [Description("奥普特")]
    Opt = 1 
}

/// <summary>
/// 光源控制器模式
/// </summary>
public enum LightControllerMode 
{ 
    [Description("数字控制器")]
    Digital = 0, 
    
    [Description("频闪控制器")]
    Strobe = 1 
}

#endregion

#region 光源配置

/// <summary>
/// 光源配置（保存在方案中）
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
    
    [Category("基本信息"), DisplayName("控制器模式")]
    [Description("数字控制器：程序控制；频闪控制器：外部信号控制")]
 public LightControllerMode Mode { get; set; } = LightControllerMode.Digital;
    
    [Category("基本信息"), DisplayName("是否启用")]
    [Description("是否启用此光源配置")]
public bool Enabled { get; set; } = true;
    
    [Category("串口配置"), DisplayName("串口号")]
    [Description("串口号，如COM1、COM3等")]
    public string PortName { get; set; } = "COM1";
 
    [Category("串口配置"), DisplayName("波特率")]
    [Description("通讯波特率，通常为9600")]
    public int BaudRate { get; set; } = 9600;
    
    [Category("串口配置"), DisplayName("数据位")]
    [Description("数据位数，通常为8")]
 public int DataBits { get; set; } = 8;
    
    [Category("串口配置"), DisplayName("停止位")]
    [Description("停止位：1, 1.5, 2")]
    public double StopBits { get; set; } = 1;
    
    [Category("串口配置"), DisplayName("校验位")]
    [Description("校验位：None, Odd, Even")]
    public string Parity { get; set; } = "None";
    
    [Category("硬件配置"), DisplayName("通道数量")]
    [Description("光源控制器的通道数量：2/4/8")]
    public int ChannelCount { get; set; } = 4;
    
    [Category("其他"), DisplayName("备注")]
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
  /// 生成唯一的配置名称
    /// </summary>
    /// <param name="type">控制器类型</param>
    /// <returns>唯一名称</returns>
    public string GenerateUniqueName(LightControllerType type)
    {
        var prefix = type == LightControllerType.Fgen ? "孚根光源" : "奥普特光源";
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

#region 工位光源控制

/// <summary>
/// 工位光源控制配置
/// </summary>
[Serializable]
[TypeConverter(typeof(ExpandableObjectConverter))]
public class StationLightControl
{
    [Category("光源控制"), DisplayName("启用光源控制")]
    [Description("是否启用光源控制")]
    public bool EnableLightControl { get; set; } = false;

    [Category("光源控制"), DisplayName("光源配置")]
    [Description("选择光源配置（从方案的光源配置中选择）")]
    [TypeConverter(typeof(LightConfigNameConverter))]
    public string LightConfigName { get; set; }

    [Category("通道配置"), DisplayName("主通道")]
    [Description("主通道号（1-8）")]
    public int Channel1 { get; set; } = 1;

    [Category("通道配置"), DisplayName("主通道亮度")]
    [Description("主通道亮度值（0-255）")]
    public int Brightness1 { get; set; } = 255;

    [Category("通道配置"), DisplayName("多通道模式")]
    [Description("是否启用多通道模式（同时控制两个通道）")]
    public bool IsMultiChannel { get; set; } = false;

    [Category("通道配置"), DisplayName("副通道")]
    [Description("副通道号（1-8），仅在多通道模式下有效")]
    public int Channel2 { get; set; } = 2;

    [Category("通道配置"), DisplayName("副通道亮度")]
    [Description("副通道亮度值（0-255），仅在多通道模式下有效")]
    public int Brightness2 { get; set; } = 255;

    [Category("时序配置"), DisplayName("打开延时(ms)")]
    [Description("执行打开光源后延时这个时间再往下执行，防止不同步")]
    public int OpenDelayMs { get; set; } = 50;

    public override string ToString()
    {
        if (!EnableLightControl) return "未启用";
      if (IsMultiChannel) return $"{LightConfigName} - CH{Channel1}+CH{Channel2}, 延时{OpenDelayMs}ms";
        return $"{LightConfigName} - CH{Channel1}, 延时{OpenDelayMs}ms";
    }
}

/// <summary>
/// 光源配置名称下拉转换器（用于PropertyGrid）
/// </summary>
internal sealed class LightConfigNameConverter : StringConverter
{
    public override bool GetStandardValuesSupported(ITypeDescriptorContext context) => true;

    public override bool GetStandardValuesExclusive(ITypeDescriptorContext context) => true;

    public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
    {
        try
        {
            var solution = SolutionManager.Instance.Current;
            var list = new List<string> { string.Empty };

            //if (solution?.LightConfigs?.Configs != null)
            //{
            //    list.AddRange(solution.LightConfigs.Configs
            //        .Where(c => c?.Enabled == true &&
            //                    c.Mode == (LightControlNet.LightControllerMode)LightControllerMode.Digital)
            //        .Select(c => c.Name)
            //        .OrderBy(n => n));
            //}

            return new StandardValuesCollection(list);
        }
        catch
        {
            return new StandardValuesCollection(new List<string> { string.Empty });
        }
    }
}

#endregion
