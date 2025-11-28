using System.ComponentModel;
using System.Xml.Serialization;
using Cognex.VisionPro;
using Cognex.VisionPro.ToolBlock;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Vision.Common;
using Vision.LightSource;
using Vision.Frm.Process;
using HardwareCommNet;

namespace Vision.Solutions.Models;

public class StationConfig
{
    [Category("工位"), DisplayName("工位名称")] 
    public string Name { get; set; }
    
    [Category("工位"), DisplayName("是否启用")] 
    public bool Enable { get; set; } = true;

    [Category("采集参数"), DisplayName("相机序列号"), TypeConverter(typeof(SnStandardValuesConverter))]
    public string SN { get; set; }

    [Category("采集参数"), DisplayName("相机类型"), ReadOnly(true), Description("面阵/线阵/3D等，仅用于显示")]
    public string CameraType { get; set; }

    [Category("采集参数"), DisplayName("采集参数"), Description("采集参数设置")]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public StationCameraParams CameraParams { get; set; }

    [Category("采集参数"), DisplayName("是否保存原图")]
    public bool SaveRawImage { get; set; } = false;

    [Category("采集参数"), DisplayName("是否保存处理图")]
    public bool SaveDealImage { get; set; } = false;

    // 通讯配置
    [Category("通讯配置"), DisplayName("通讯设备")]
    [Description("选择工位使用的通讯设备（在通讯管理中选择）")]
    [TypeConverter(typeof(CommDeviceNameConverter))]
    public string CommDeviceName { get; set; }

    [Category("通讯配置"), DisplayName("触发变量"), Description("选择用于触发的输入变量名称")]
    [TypeConverter(typeof(StationIOTableInputVarConverter))]
    public string TriggerVariableName { get; set; }

    [Category("通讯配置"), DisplayName("触发值"), Description("触发匹配的目标值")]
    [TypeConverter(typeof(StationTriggerValueConverter))]
    public string TriggerValue { get; set; }

    // 光源控制
    [Category("光源控制"), DisplayName("光源控制")]
    [Description("配置工位的光源控制参数")]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public StationLightControl LightControl { get; set; } = new();

    // 深度学习
    [Category("深度学习"), DisplayName("是否加载模型")]
    public bool bLoadModel { get; set; } = false;

    [Category("深度学习"), DisplayName("运行时类型")]
    [Description("GPU / OpenVINO / TensorRT")]
    public DLRuntime RuntimeType { get; set; } = DLRuntime.GC;

    [Category("深度学习"), DisplayName("模型文件夹路径")]
    [Editor(typeof(FolderPathEditor), typeof(System.Drawing.Design.UITypeEditor))]
    [TypeConverter(typeof(FolderPathConverter))]
    public string ModelPath { get; set; } = string.Empty;

    [Category("深度学习"), DisplayName("模型文件夹名称"), ReadOnly(true)]
    [Browsable(true)]
    public string ModelFolderName
    {
        get
        {
            if (string.IsNullOrWhiteSpace(ModelPath) || !Directory.Exists(ModelPath))
                return "<未选择>";
            return Path.GetFileName(ModelPath.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
        }
    }

    // 算法配置
    [Category("算法"), DisplayName("棋盘格标定"), Description("是否执行棋盘格标定")]
    public bool bCalibCheckboardTool { get; set; } = false;

    [Category("算法"), DisplayName("九点标定"), Description("是否执行九点标定")]
    public bool bCalibNPointTool { get; set; } = false;

    [Category("算法"), DisplayName("检测工具"), ReadOnly(true), Description("是否存在检测工具（只读）")]
    public bool bCheckTool { get; set; } = true;

    // 显示配置
    [Category("显示"), DisplayName("是否显示")]
    public bool bShow { get; set; }

    [Category("显示"), DisplayName("显示窗口"), TypeConverter(typeof(DisplayWindowStandardValuesConverter))]
    public string DisplayName { get; set; }

    [Category("显示"), DisplayName("显示图层")]
    public int RecoredIndex { get; set; }

    [XmlIgnore]
    [Browsable(false)]
    public object DLModel { get; set; }

    // 统一的工具集合，用于隔离序列化
    [Browsable(false)]
    private class ToolSet
    {
        public ToolBase Checkerboard { get; set; } = new("Checkerboard.vpp");
        public ToolBase NPoint { get; set; } = new("NPoint.vpp");
        public ToolBase Detection { get; set; } = new("Detection.vpp");
    }

    [Browsable(false)] private ToolSet Tools { get; set; } = new();

    // 将工具字段放入内部集合，避免重复序列化
    [XmlIgnore]
    [Browsable(false)]
    public ToolBase CheckerboardTool { get => Tools?.Checkerboard; set
    {
        Tools ??= new ToolSet();
        Tools.Checkerboard = value;
    } }

    [XmlIgnore]
    [Browsable(false)]
    public ToolBase NPointTool { get => Tools?.NPoint; set { if (Tools == null) Tools = new ToolSet(); Tools.NPoint = value; } }

    [XmlIgnore]
    [Browsable(false)]
    public ToolBase DetectionTool { get => Tools?.Detection; set
    {
        Tools ??= new ToolSet();
        Tools.Detection = value;
    } }

    // 下列只读展示项用于 PropertyGrid 右键打开配置
    [Category("算法"), DisplayName("检测工具"), ReadOnly(true), Description("右键打开检测工具配置")]
    //[TypeConverter(typeof(ExpandableObjectConverter))]
    public ToolBase DetectionToolDisplay => DetectionTool;

    [Category("算法"), DisplayName("棋盘格标定工具"), ReadOnly(true), Description("右键打开棋盘格标定工具配置")]
    //[TypeConverter(typeof(ExpandableObjectConverter))]
    public ToolBase CheckerboardToolDisplay => CheckerboardTool;

    [Category("算法"), DisplayName("九点标定工具"), ReadOnly(true), Description("右键打开九点标定工具配置")]
    //[TypeConverter(typeof(ExpandableObjectConverter))]
    public ToolBase NPointToolDisplay => NPointTool;

    [Category("通讯映射"), DisplayName("输出映射")]
    [Description("检测工具输出端子到通讯输出变量的映射关系")]
    [Browsable(false)]
    public List<OutputMapping> OutputMappings { get; set; } = new();

    [Category("通讯映射"), DisplayName("输出映射展示"), ReadOnly(true)]
    [Description("右键打开输出映射配置窗口")]
    public string OutputMappingsDisplay
    {
        get
        {
            if (OutputMappings == null || OutputMappings.Count == 0)
                return "<未配置>";
            return $"<已配置{OutputMappings.Count}项映射>";
        }
    }

  public class DetectVarDef
  {
    public string Name { get; set; }
    public string TypeName { get; set; }
    public string Value { get; set; }
    public string Comment { get; set; }
    // 变量链接：可同时链接工位输出、全局变量、通讯输入；运行时按"通讯输入优先，其次工位输出，最后全局"的顺序赋值
    public string LinkStation { get; set; }
    public string LinkOutput { get; set; }
    public string LinkGlobal { get; set; }
    // 通讯输入链接
    public string LinkCommDevice { get; set; }
    public string LinkCommInput { get; set; }
  }

    /// <summary>
    /// 检测工具输出到通讯输出的映射
    /// </summary>
    public class OutputMapping
    {
        /// <summary>
        /// 工具输出名称，在 DetectionTool.ToolBlock.Outputs 中选择
        /// </summary>
        public string ToolOutputName { get; set; }
        
        /// <summary>
        /// 通讯输出变量名称，在 CommDevice.Table.Outputs 中选择
        /// </summary>
        public string CommOutputName { get; set; }
        
        /// <summary>
        /// 备注说明
        /// </summary>
        public string Description { get; set; }
    }

  public class ToolBase
  {
        [Browsable(false)]
        public string FileName { get; }

        [XmlIgnore]
        [Browsable(false)]
        public CogToolBlock ToolBlock { get; set; }

        public List<DetectVarDef> Vars { get; set; } = [];

        // parameterless ctor for XmlSerializer
        public ToolBase()
        {
            FileName = "Tool.vpp";
            ToolBlock = new CogToolBlock
            {
                Inputs = { new CogToolBlockTerminal("Image", typeof(ICogImage), true) },
                Outputs =
                {
                    new CogToolBlockTerminal("Result", typeof(bool), true) ,
                    new CogToolBlockTerminal("Image", typeof(ICogImage), true),
                }
            };
        }

        public ToolBase(string fileName)
        {
            FileName = fileName;
            ToolBlock = new CogToolBlock
            {
                Inputs = { new CogToolBlockTerminal("Image", typeof(ICogImage), true) },
                Outputs =
                {
                    new CogToolBlockTerminal("Result", typeof(bool), true) ,
                    new CogToolBlockTerminal("Image", typeof(ICogImage), true),
                }
            };
        }

    public void ApplyVarsToInputs()
    {
      if (ToolBlock == null || Vars == null) return;
      foreach (var v in Vars)
      {
        if (string.IsNullOrWhiteSpace(v?.Name)) continue;
        CogToolBlockTerminal term = null;
        for (var t =0; t < ToolBlock.Inputs.Count; t++)
        {
          var tt = ToolBlock.Inputs[t]; if (!string.Equals(tt.Name, v.Name, StringComparison.OrdinalIgnoreCase)) continue; term = tt; break;
        }
        if (term == null) continue;
        var targetType = TypeValueUtil.ResolveType(v.TypeName); if (targetType == null) continue;

        object obj = null;
        bool haveValue = false;
        // 变量链接优先：先尝试通讯输入，其次工位输出，最后全局变量
        try
        {
          var sol = SolutionManager.Instance.Current;
          
          // 通讯输入（最高优先级）- 直接从缓存获取，不重新读取PLC
          if (!haveValue && !string.IsNullOrWhiteSpace(v.LinkCommDevice) && !string.IsNullOrWhiteSpace(v.LinkCommInput))
          {
            try
            {
              var device = CommunicationFactory.Instance.GetDevice(v.LinkCommDevice);
              if (device != null)
              {
                // 直接从通讯表缓存获取值（轮询线程已经在持续更新）
                var cv = device.Table?.GetInputCachedValue(v.LinkCommInput);
                if (cv != null)
                {
                  if (targetType.IsAssignableFrom(cv.GetType())) { obj = cv; haveValue = true; }
                  else { try { obj = Convert.ChangeType(cv, targetType); haveValue = true; } catch { haveValue = false; } }
                }
              }
            }
            catch { }
          }
          
          if (sol != null)
          {
            // 工位输出
            if (!haveValue && !string.IsNullOrWhiteSpace(v.LinkStation) && !string.IsNullOrWhiteSpace(v.LinkOutput))
            {
              if (sol.LastOutputs != null && sol.LastOutputs.TryGetValue(v.LinkStation, out var dict) && dict != null && dict.TryGetValue(v.LinkOutput, out var sv) && sv != null)
              {
                if (targetType.IsAssignableFrom(sv.GetType())) { obj = sv; haveValue = true; }
                else { try { obj = Convert.ChangeType(sv, targetType); haveValue = true; } catch { haveValue = false; } }
              }
            }
            // 全局变量（若工位未取到值，则尝试全局）
            if (!haveValue && !string.IsNullOrWhiteSpace(v.LinkGlobal))
            {
              if (sol.GlobalValues != null && sol.GlobalValues.TryGetValue(v.LinkGlobal, out var gv) && gv != null)
              {
                if (targetType.IsAssignableFrom(gv.GetType())) { obj = gv; haveValue = true; }
                else { try { obj = Convert.ChangeType(gv, targetType); haveValue = true; } catch { haveValue = false; } }
              }
            }
          }
        }
        catch { }

        // 若无链接值，则按文本解析默认值
        if (!haveValue)
        {
          if (!TypeValueUtil.TryParseValue(v.Value, targetType, out obj, out var _)) continue;
        }

        // 赋值到输入端子：只要 obj 有值就尝试赋值
        if (obj != null)
        {
          try { term.Value = obj; }
          catch { }
        }
      }
    }

        public void SyncInputsToVars()
        {
            if (ToolBlock == null) return;
            Vars?.RemoveAll(v =>
            {
                if (string.IsNullOrWhiteSpace(v?.Name)) return true;
                for (int i =0; i < ToolBlock.Inputs.Count; i++) if (string.Equals(ToolBlock.Inputs[i].Name, v.Name, StringComparison.OrdinalIgnoreCase)) return false; return true;
            });
            for (int i =0; i < ToolBlock.Inputs.Count; i++)
            {
                var term = ToolBlock.Inputs[i]; if (term?.Value == null) continue;
                var valueType = term.Value.GetType(); var typeName = TypeValueUtil.TypeToName(valueType);
                if (string.IsNullOrEmpty(typeName) || TypeValueUtil.ResolveType(typeName) == null) continue;
                var existingVar = Vars?.FirstOrDefault(v => string.Equals(v?.Name, term.Name, StringComparison.OrdinalIgnoreCase));
                if (existingVar != null) { existingVar.Value = TypeValueUtil.ValueToString(term.Value, valueType); existingVar.TypeName = typeName; }
                else { Vars ??= new List<DetectVarDef>(); Vars.Add(new DetectVarDef { Name = term.Name, TypeName = typeName, Value = TypeValueUtil.ValueToString(term.Value, valueType), Comment = "自动同步输入变量" }); }
        }
    }

        public override string ToString()
        {
            if (ToolBlock == null) return "<未配置>";
            int toolCount = 0;
            try { toolCount = ToolBlock.Tools.Count; } catch { }
            return $"<包含{toolCount}个工具>";
        }
    }
}
