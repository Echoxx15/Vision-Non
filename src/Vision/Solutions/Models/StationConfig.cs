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

namespace Vision.Solutions.Models;

public class StationConfig
{
    [Category("基本"), DisplayName("工位名称")] 
    public string Name { get; set; }
    
    [Category("基本"), DisplayName("是否启用")] 
    public bool Enable { get; set; } = true;

    [Category("取像配置"), DisplayName("相机序列号"), TypeConverter(typeof(SnStandardValuesConverter))]
    public string SN { get; set; }

    [Category("取像配置"), DisplayName("相机类型"), ReadOnly(true), Description("相机的类型-面阵，线扫，3D，右键打开配置界面")]
    public string CameraType { get; set; }

    [Category("取像配置"), DisplayName("相机参数"), Description("点击打开相机参数配置窗口")]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public StationCameraParams CameraParams { get; set; }

    [Category("取像配置"), DisplayName("是否保存原图")]
    public bool SaveRawImage { get; set; } = false;

    [Category("取像配置"), DisplayName("是否保存结果图")]
    public bool SaveDealImage { get; set; } = false;

    // 通讯配置
    [Category("通讯配置"), DisplayName("通讯设备")]
    [Description("选择工位使用的通讯设备（从通讯配置中选择）")]
    [TypeConverter(typeof(CommDeviceNameConverter))]
    public string CommDeviceName { get; set; }

    [Category("通讯配置"), DisplayName("触发变量"), Description("选择工位使用的通讯设备（从通讯配置中选择）")]
    [TypeConverter(typeof(StationIOTableInputVarConverter))]
    public string TriggerVariableName { get; set; }

    [Category("通讯配置"), DisplayName("触发值"), Description("选择工位使用的通讯设备（从通讯配置中选择）")]
    [TypeConverter(typeof(StationTriggerValueConverter))]
    public string TriggerValue { get; set; }

    // 光源配置
    [Category("光源配置"), DisplayName("光源配置")]
    [Description("配置工位的光源控制参数")]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public StationLightControl LightControl { get; set; } = new();

    // 深度学习
    [Category("深度学习"), DisplayName("是否加载模型")]
    public bool bLoadModel { get; set; } = false;

    [Category("深度学习"), DisplayName("运行时类型")]
    [Description("选择深度学习运行时：GPU(兼容性好) / OpenVINO(Intel优化) / TensorRT(NVIDIA高性能)")]
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
    [Category("算法"), DisplayName("棋盘格标定工具"), Description("图像是否启用棋盘格标定")]
    public bool bCalibCheckboardTool { get; set; } = false;

    [Category("算法"), DisplayName("九点标定工具"), Description("图像是否启用九点标定")]
    public bool bCalibNPointTool { get; set; } = false;

    [Category("算法"), DisplayName("检测工具"), ReadOnly(true), Description("是否运行检测工具，默认运行")]
    public bool bCheckTool { get; set; } = true;

    // 显示配置
    [Category("显示"), DisplayName("是否显示")]
    public bool bShow { get; set; }

    [Category("显示"), DisplayName("显示窗口"), TypeConverter(typeof(DisplayWindowStandardValuesConverter))]
    public string DisplayName { get; set; }

    [Category("显示"), DisplayName("显示图像")]
    public int RecoredIndex { get; set; }

    [XmlIgnore]
    [Browsable(false)]
    public object DLModel { get; set; }

    //统一的工具集合（参与序列化）
    [Browsable(false)]
    private class ToolSet
    {
        public ToolBase Checkerboard { get; set; } = new("Checkerboard.vpp");
        public ToolBase NPoint { get; set; } = new("NPoint.vpp");
        public ToolBase Detection { get; set; } = new("Detection.vpp");
    }

    [Browsable(false)] private ToolSet Tools { get; set; } = new();

    //兼容旧字段：改为代理到 Tools，避免重复序列化
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

    // 添加只读代理属性用于PropertyGrid显示（右键菜单会根据这些属性判断）
    [Category("算法"), DisplayName("检测工具"), ReadOnly(true), Description("右键打开检测工具配置")]
    //[TypeConverter(typeof(ExpandableObjectConverter))]
    public ToolBase DetectionToolDisplay => DetectionTool;

    [Category("算法"), DisplayName("棋盘格标定工具配置"), ReadOnly(true), Description("右键打开棋盘格标定工具配置")]
    //[TypeConverter(typeof(ExpandableObjectConverter))]
    public ToolBase CheckerboardToolDisplay => CheckerboardTool;

    [Category("算法"), DisplayName("九点标定工具配置"), ReadOnly(true), Description("右键打开九点标定工具配置")]
    //[TypeConverter(typeof(ExpandableObjectConverter))]
    public ToolBase NPointToolDisplay => NPointTool;

    [Category("通讯输出"), DisplayName("输出映射")]
    [Description("配置检测工具的输出端子与通讯输出变量的映射关系")]
    [Browsable(false)] // 隐藏List本身，使用下面的只读代理属性
    public List<OutputMapping> OutputMappings { get; set; } = new();

    [Category("通讯输出"), DisplayName("输出映射配置"), ReadOnly(true)]
    [Description("右键打开输出映射配置窗口")]
    public string OutputMappingsDisplay
    {
        get
        {
            if (OutputMappings == null || OutputMappings.Count == 0)
                return "<未配置>";
            return $"<已配置{OutputMappings.Count}个映射>";
        }
    }

    public class DetectVarDef
    {
        public string Name { get; set; }
        public string TypeName { get; set; }
        public string Value { get; set; }
        public string Comment { get; set; }
    }

    /// <summary>
    /// 工具输出端子与通讯输出变量的映射
    /// </summary>
    public class OutputMapping
    {
        /// <summary>
        /// 工具输出端子名称（从 DetectionTool.ToolBlock.Outputs 中选择）
        /// </summary>
        public string ToolOutputName { get; set; }
        
        /// <summary>
        /// 通讯输出变量名称（从 CommDevice.Table.Outputs 中选择）
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
                if (TypeValueUtil.TryParseValue(v.Value, targetType, out var obj, out var _))
                {
                    try { if (term.Value == null || (term.Value.GetType() == targetType || targetType.IsAssignableFrom(term.Value.GetType()))) { term.Value = obj; } }
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
                else { Vars ??= new List<DetectVarDef>(); Vars.Add(new DetectVarDef { Name = term.Name, TypeName = typeName, Value = TypeValueUtil.ValueToString(term.Value, valueType), Comment = "自动同步自输入端子" }); }
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
