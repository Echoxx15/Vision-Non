using System;
using System.ComponentModel;
using System.Linq;
using HardwareCommNet;
using Vision.Solutions.Models;

namespace Vision.Frm.Process;

/// <summary>
/// 根据所选通讯设备，提供输入变量名下拉候选
/// </summary>
public class StationIOTableInputVarConverter : StringConverter
{
 public override bool GetStandardValuesSupported(ITypeDescriptorContext context) => true;
 public override bool GetStandardValuesExclusive(ITypeDescriptorContext context) => true;

 public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
 {
  try
  {
   // 直接从ProcessStation实例获取
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