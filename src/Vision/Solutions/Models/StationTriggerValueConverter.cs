using System;
using System.ComponentModel;
using System.Linq;
using HardwareCommNet;

namespace Vision.Solutions.Models;

public class StationTriggerValueConverter : StringConverter
{
 public override bool GetStandardValuesSupported(ITypeDescriptorContext context) => true;
 public override bool GetStandardValuesExclusive(ITypeDescriptorContext context) => true; //ֻ��������ѡ��

 public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
 {
  try
  {
   // ֱ�Ӵ�ProcessStationʵ����ȡ
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
