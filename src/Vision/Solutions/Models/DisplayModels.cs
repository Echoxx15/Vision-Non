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
 public int Rows { get; set; } =1;
 public int Cols { get; set; } =1;
 public List<DisplayItem> Items { get; set; } = [];
}

/// <summary>
/// 显示窗口标准值转换器
/// 从当前方案的 Display 中获取可用的显示窗口列表
/// </summary>
public sealed class DisplayWindowStandardValuesConverter : TypeConverter
{
 public override bool GetStandardValuesSupported(ITypeDescriptorContext context) => true;

 public override bool GetStandardValuesExclusive(ITypeDescriptorContext context) => true;

 public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
 {
  try
  {
   var items = SolutionManager.Instance.Current?.Display?.Items;
   var list = (items == null)
    ? [string.Empty]
    : items
     .Select(i => string.IsNullOrWhiteSpace(i.DisplayName) ? i.Key : i.DisplayName)
     .Where(s => !string.IsNullOrWhiteSpace(s))
     .Distinct(StringComparer.OrdinalIgnoreCase)
     .ToList();

   //追加一个空行作为“可不选”的项
   if (list.Count > 0 && !string.IsNullOrEmpty(list[0]))
   {
    list.Insert(0, string.Empty);
   }

   return new StandardValuesCollection(list);
  }
  catch
  {
   return new StandardValuesCollection(new List<string> { string.Empty });
  }
 }
}

/// <summary>
/// TCP连接名称标准值转换器
/// 从当前方案的 TCP 配置中获取可用的连接名称列表
/// </summary>
public sealed class TcpConnectionNameConverter : TypeConverter
{
 public override bool GetStandardValuesSupported(ITypeDescriptorContext context) => true;
 
 public override bool GetStandardValuesExclusive(ITypeDescriptorContext context) => true;
 
 public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
 {
 try
 {
 // 从 CommunicationFactory 获取所有通讯设备
 var devices = HardwareCommNet.CommunicationFactory.Instance.GetAllDevices();
 var list = new List<string> { string.Empty };
 
 if (devices != null)
 {
 // 筛选 TCP设备（设备名称或类型包含 "Tcp"）
 var tcpNames = devices
 .Where(d => d != null &&
 d.IsConnected &&
 (d.Name.IndexOf("Tcp", StringComparison.OrdinalIgnoreCase) >=0 ||
 d.GetType().Name.IndexOf("Tcp", StringComparison.OrdinalIgnoreCase) >=0))
 .Select(d => d.Name)
 .OrderBy(name => name)
 .ToList();
 
 list.AddRange(tcpNames);
 }

 return new StandardValuesCollection(list);
 }
 catch 
 { 
 return new StandardValuesCollection(new List<string> { string.Empty }); 
 }
 }
}

/// <summary>
/// 通讯设备名称转换器
/// 从 CommunicationFactory 获取所有可用的通讯设备列表
/// </summary>
public sealed class CommDeviceNameConverter : TypeConverter
{
 public override bool GetStandardValuesSupported(ITypeDescriptorContext context) => true;

 public override bool GetStandardValuesExclusive(ITypeDescriptorContext context) => true;

 public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
 {
  try
  {
   // 从 CommunicationFactory 获取所有通讯设备
   var devices = HardwareCommNet.CommunicationFactory.Instance.GetAllDevices();
   var list = new List<string> { string.Empty }; // 空选项（可选）

   if (devices != null)
   {
    // 获取所有设备名称
    var deviceNames = devices
     .Where(d => d != null)
     .Select(d => d.Name)
     .OrderBy(name => name)
     .ToList();

    list.AddRange(deviceNames);
   }

   return new StandardValuesCollection(list);
  }
  catch
  {
   return new StandardValuesCollection(new List<string> { string.Empty });
  }
 }
}
