using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Vision.Solutions.Models;

[Serializable]
public class DisplayItem
{
    public string Key { get; set; }
    public string DisplayName { get; set; }
}

[Serializable]
public class DisplayConfig
{
    // 默认一行一列
    public int Rows { get; set; } = 1;
    public int Cols { get; set; } = 1;
    // 默认包含一个显示窗口："显示1"
    public List<DisplayItem> Items { get; set; } = new List<DisplayItem>
    {
      new DisplayItem { Key = "显示1", DisplayName = "显示1" }
    };
}

/// <summary>
/// 显示窗口下拉列表转换器
/// 从方案的Display配置中获取可用的显示窗口列表
/// </summary>
internal sealed class DisplayWindowStandardValuesConverter : TypeConverter
{
    public override bool GetStandardValuesSupported(ITypeDescriptorContext context) => true;
    
    public override bool GetStandardValuesExclusive(ITypeDescriptorContext context) => true;
    
    public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
    {
        try
 {
  var items = SolutionManager.Instance.Current?.Display?.Items;
            var list = (items == null)
       ? new List<string> { string.Empty }
                : items
          .Select(i => string.IsNullOrWhiteSpace(i.DisplayName) ? i.Key : i.DisplayName)
        .Where(s => !string.IsNullOrWhiteSpace(s))
          .Distinct(StringComparer.OrdinalIgnoreCase)
           .ToList();
       
            // 确保有空选项
            if (list.Count > 0 && !string.IsNullOrEmpty(list[0]))
     {
  list.Insert(0, string.Empty);
   }
      
            return new TypeConverter.StandardValuesCollection(list);
        }
        catch 
        { 
return new TypeConverter.StandardValuesCollection(new List<string> { string.Empty }); 
 }
    }
}

/// <summary>
/// TCP连接名称下拉列表转换器
/// 从方案的TCP配置中获取可用的TCP连接列表
/// </summary>
internal sealed class TcpConnectionNameConverter : TypeConverter
{
    public override bool GetStandardValuesSupported(ITypeDescriptorContext context) => true;
    
    public override bool GetStandardValuesExclusive(ITypeDescriptorContext context) => true;
    
    public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
    {
try
        {
    var configs = SolutionManager.Instance.Current?.TcpConfigs?.Configs;
            var list = new List<string> { string.Empty }; // 空选项表示不发送
          
  if (configs != null && configs.Count > 0)
    {
    // 获取所有已启用的TCP连接名称
          var tcpNames = configs
       .Where(c => c != null && c.Enabled && !string.IsNullOrWhiteSpace(c.Name))
      .Select(c => c.Name)
     .OrderBy(name => name)
         .ToList();
      
            list.AddRange(tcpNames);
            }

            return new TypeConverter.StandardValuesCollection(list);
        }
        catch 
        { 
 return new TypeConverter.StandardValuesCollection(new List<string> { string.Empty }); 
 }
    }
}