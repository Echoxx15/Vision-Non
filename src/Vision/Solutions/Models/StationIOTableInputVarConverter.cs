using System;
using System.ComponentModel;
using System.Linq;
using HardwareCommNet;

namespace Vision.Solutions.Models;

/// <summary>
/// 为工位选择通讯设备时提供输入变量的下拉选择
/// </summary>
public class StationIOTableInputVarConverter : StringConverter
{
 public override bool GetStandardValuesSupported(ITypeDescriptorContext context) => true;
 public override bool GetStandardValuesExclusive(ITypeDescriptorContext context) => true;

 public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
 {
  try
  {
   // 直接从 StationConfig 实例读取
   if (context?.Instance is StationConfig st && !string.IsNullOrWhiteSpace(st.CommDeviceName))
   {
    var dev = CommunicationFactory.Instance.GetDevice(st.CommDeviceName);
    var list =
     dev?.Table?.Inputs?.Select(c => c?.Name).Where(n => !string.IsNullOrWhiteSpace(n)).Distinct().ToArray() ??
     Array.Empty<string>();
    return new StandardValuesCollection(list);
   }
  }
  catch
  {
  }

  return new StandardValuesCollection(Array.Empty<string>());
 }
}
