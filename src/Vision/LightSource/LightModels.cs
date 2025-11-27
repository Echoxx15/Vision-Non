using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Vision.Solutions.Models;

namespace Vision.LightSource;

// 保留工位光源控制模型，其余光源枚举与配置迁移到 LightControlNet DLL

#region 工位光源配置

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
    [Description("选择光源配置，来自方案的光源配置集合")]
    [TypeConverter(typeof(LightConfigNameConverter))]
    public string LightConfigName { get; set; }

    [Category("通道设置"), DisplayName("通道1")]
    [Description("通道编号，1-8")]
    public int Channel1 { get; set; } = 1;

    [Category("通道设置"), DisplayName("亮度1")]
    [Description("亮度值 0-255")]
    public int Brightness1 { get; set; } = 255;

    [Category("通道设置"), DisplayName("多通道模式")]
    [Description("是否启用多通道模式，同时控制两个通道")]
    public bool IsMultiChannel { get; set; } = false;

    [Category("通道设置"), DisplayName("通道2")]
    [Description("通道编号，1-8；仅在多通道模式下有效")]
    public int Channel2 { get; set; } = 2;

    [Category("通道设置"), DisplayName("亮度2")]
    [Description("亮度值 0-255；仅在多通道模式下有效")]
    public int Brightness2 { get; set; } = 255;

    [Category("时序"), DisplayName("开灯延时(ms)")]
    [Description("执行打开光源后等待的时间，再进行触发，避免不同步")]
    public int OpenDelayMs { get; set; } = 50;

    public override string ToString()
    {
        if (!EnableLightControl) return "未启用";
        if (IsMultiChannel) return $"{LightConfigName} - CH{Channel1}+CH{Channel2}, 延时{OpenDelayMs}ms";
        return $"{LightConfigName} - CH{Channel1}, 延时{OpenDelayMs}ms";
    }
}

/// <summary>
/// 光源配置名称转换器（用于 PropertyGrid）
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
