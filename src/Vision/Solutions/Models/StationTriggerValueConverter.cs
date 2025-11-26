using System;
using System.ComponentModel;
using System.Linq;
using HardwareCommNet;
using Vision.Solutions.Models;

namespace Vision.Frm.Process;

public class StationTriggerValueConverter : StringConverter
{
 public override bool GetStandardValuesSupported(ITypeDescriptorContext context) => true;
 public override bool GetStandardValuesExclusive(ITypeDescriptorContext context) => true; //只允许下拉选择

 public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
 {
  try
  {
   // 直接从ProcessStation实例获取
   if (context?.Instance is StationConfig st && !string.IsNullOrWhiteSpace(st.CommDeviceName) &&
       !string.IsNullOrWhiteSpace(st.TriggerVariableName))
   {
    var dev = CommunicationFactory.Instance.GetDevice(st.CommDeviceName);
    var cell = dev?.Table?.Inputs?.FirstOrDefault(c =>
     string.Equals(c.Name, st.TriggerVariableName, StringComparison.OrdinalIgnoreCase));
    var list = cell?.TriggerValues?.Where(v => !string.IsNullOrWhiteSpace(v))?.ToArray() ?? [];
    return new StandardValuesCollection(list);
   }
  }
  catch
  {
  }

  return new StandardValuesCollection(Array.Empty<string>());
 }
}