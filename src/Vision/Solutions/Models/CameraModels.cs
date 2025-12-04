using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using HardwareCameraNet;

namespace Vision.Solutions.Models;

/// <summary>
/// 相机序列号下拉列表转换器
/// 显示格式：注释（序列号）
/// 存储值：序列号
/// 
/// 使用示例：
/// - 下拉显示："上料相机（AA11BB22CC33）"
/// - 实际保存："AA11BB22CC33"
/// </summary>
internal sealed class SnStandardValuesConverter : TypeConverter
{
    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
    {
        return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
    }

    public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
    {
        return destinationType == typeof(string) || base.CanConvertTo(context, destinationType);
    }

    /// <summary>
    /// 从显示值转换为序列号
    /// 输入："上料相机（A1B2C3D4）" → 输出："A1B2C3D4"
    /// </summary>
    public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
    {
        if (value is not string str)
            return base.ConvertFrom(context, culture, value);

        if (string.IsNullOrWhiteSpace(str))
            return string.Empty;

        // 尝试从显示格式中提取序列号
        // 格式1："注释（序列号）" → 提取序列号
        // 格式2："序列号" → 直接返回
        var startIdx = str.LastIndexOf('（');
        var endIdx = str.LastIndexOf('）');

        if (startIdx >= 0 && endIdx > startIdx)
        {
// 提取括号内的序列号
            var sn = str.Substring(startIdx + 1, endIdx - startIdx - 1).Trim();
            return string.IsNullOrWhiteSpace(sn) ? str.Trim() : sn;
        }

        // 没有找到括号格式，直接返回原值（可能就是序列号）
        return str.Trim();
    }

    /// <summary>
    /// 从序列号转换为显示值
    /// 输入:"A1B2C3D4" → 输出："上料相机（A1B2C3D4）"
    /// </summary>
    public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value,
        Type destinationType)
    {
        if (destinationType != typeof(string) || value is not string sn)
            return base.ConvertTo(context, culture, value, destinationType);

        // 如果序列号为空，返回空字符串
        if (string.IsNullOrWhiteSpace(sn))
            return string.Empty;

        try
        {
            // 从相机管理器中查找对应的相机配置
            var config = CameraFactory.Instance.GetAllConfigs()
                .FirstOrDefault(c => string.Equals(c.SerialNumber, sn, StringComparison.OrdinalIgnoreCase));

            if (config != null && !string.IsNullOrWhiteSpace(config.Expain))
            {
                // 有注释：显示为 "注释（序列号）"
                return $"{config.Expain}（{sn}）";
            }
        }
        catch
        {
            // 忽略异常（可能相机管理器未初始化）
        }

        // 没有找到配置或注释为空，直接显示序列号
        return sn;
    }

    public override bool GetStandardValuesSupported(ITypeDescriptorContext context) => true;

    public override bool GetStandardValuesExclusive(ITypeDescriptorContext context) => true;

    public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
    {
        try
        {
            var configs = CameraFactory.Instance.GetAllConfigs();
            if (configs.Count == 0)
            {
                return new StandardValuesCollection(new[] { string.Empty });
            }

            // 返回序列号列表（显示由ConvertTo处理）
            var values = configs
                .Where(c => !string.IsNullOrWhiteSpace(c.SerialNumber))
                .Select(c => c.SerialNumber)
                .OrderBy(sn => sn)
                .ToList();

            // 添加空选项
            values.Insert(0, string.Empty);

            return new StandardValuesCollection(values);
        }
        catch
        {
            return new StandardValuesCollection(new[] { string.Empty });
        }
    }
}

/// <summary>
/// 文件夹路径选择器转换器，支持点击省略号按钮选择文件夹
/// </summary>
internal sealed class FolderPathConverter : StringConverter
{
    public override bool GetStandardValuesSupported(ITypeDescriptorContext context) => false;

    // 重要：返回 true 表示可以通过 UITypeEditor 编辑（将在 ProcessStation 中通过 Editor 特性关联）
    // 但由于 .NET Framework 4.8.1 PropertyGrid 的限制，我们直接通过自定义编辑器实现
}

/// <summary>
/// 文件夹路径编辑器（用于 PropertyGrid）
/// </summary>
internal sealed class FolderPathEditor : System.Drawing.Design.UITypeEditor
{
    public override System.Drawing.Design.UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
    {
        return System.Drawing.Design.UITypeEditorEditStyle.Modal;
    }

    public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
    {
        using (var dialog = new FolderBrowserDialog())
        {
            dialog.Description = "请选择深度学习模型文件夹";
            dialog.ShowNewFolderButton = false;

            // 设置初始路径
            if (value is string currentPath && !string.IsNullOrWhiteSpace(currentPath) &&
                System.IO.Directory.Exists(currentPath))
            {
                dialog.SelectedPath = currentPath;
            }

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                return dialog.SelectedPath;
            }
        }

        return value;
    }
}

[TypeConverter(typeof(ExpandableObjectConverter))]
public class StationCameraParams
{
    [Category("采集参数"), DisplayName("曝光")]
    public double Exposure { get; set; }
    [Category("采集参数"), DisplayName("增益")]
    public double Gain { get; set; }
    [Category("采集参数"), DisplayName("采集超时时间")]
    public int TimeoutMs { get; set; } = 3000;
    [Category("采集参数"), DisplayName("图像宽度")]
    public int Width { get; set; }
    [Category("采集参数"), DisplayName("图像高度")]
    public int Height { get; set; }
    [Category("采集参数"), DisplayName("触发模式")]
    public TriggerMode TriggerMode { get; set; } = TriggerMode.软触发;
    [Category("采集参数"), DisplayName("触发次数")]
    public int TriggerCount { get; set; } = 1;
    public override string ToString() => $"曝光={Exposure}, 增益={Gain}, 触发模式={TriggerMode}, 触发次数={TriggerCount}, 图像宽度={Width}, 图像高度={Height}";
}
