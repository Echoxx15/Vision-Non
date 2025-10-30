using System.ComponentModel;
using System.Xml.Serialization;
using Cognex.VisionPro;
using Cognex.VisionPro.ToolBlock;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Vision.Common;

namespace Vision.Solutions.Models;

public class ProcessStation
{
    [Category("基本"), DisplayName("工位名称")] public string Name { get; set; }
    [Category("基本"), DisplayName("是否启用")] public bool Enable { get; set; } = true;

    [Category("取像配置"), DisplayName("相机序列号"), TypeConverter(typeof(SnStandardValuesConverter))]
    public string SN { get; set; }

    [Category("取像配置"), DisplayName("相机类型"), ReadOnly(true), Description("相机的类型-面阵，线扫，3D，右键打开配置界面")]
    public string CameraType { get; set; }

    [Category("取像配置"), DisplayName("是否保存原图")]
    public bool SaveRawImage { get; set; } = false;

    [Category("取像配置"), DisplayName("是否保存结果图")]
    public bool SaveDealImage { get; set; } = false;

    [Category("通讯配置"), DisplayName("TCP连接名称"), TypeConverter(typeof(TcpConnectionNameConverter))]
    [Description("选择用于发送检测结果的TCP连接，空白表示不发送。")]
    public string TcpConnectionName { get; set; }

    [Category("通讯配置"), DisplayName("是否启用TCP")]
    [Description("是否启用TCP发送检测结果，禁用后不会发送数据")]
    public bool EnableTcp { get; set; } = true;

    /// <summary>
    /// 光源控制配置
    /// </summary>
    [Category("光源配置"), DisplayName("光源配置")]
    [Description("配置工位的光源控制参数")]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public LightSource.StationLightControl LightControl { get; set; } = new();

    [Category("深度学习"), DisplayName("是否加载模型")]
    public bool bLoadModel { get; set; } = false;

    [Category("深度学习"), DisplayName("运行时类型")]
    [Description("选择深度学习运行时：GPU(兼容性好) / OpenVINO(Intel优化) / TensorRT(NVIDIA高性能)")]
    public DLRuntime RuntimeType { get; set; } = DLRuntime.GC;

    [Category("深度学习"), DisplayName("模型文件夹路径")]
    [Editor(typeof(FolderPathEditor), typeof(System.Drawing.Design.UITypeEditor))]
    [TypeConverter(typeof(FolderPathConverter))]
    public string ModelPath { get; set; } = string.Empty;

    // 只读属性，显示模型文件夹名称
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

    // 运行时模型实例（不参与序列化）
    [XmlIgnore] [Browsable(false)] public object DLModel { get; set; }

    [Category("算法"), DisplayName("棋盘格标定工具"), Description("图像是否启用棋盘格标定")]
    public bool bCalibCheckboardTool { get; set; } = false;

    [Category("算法"), DisplayName("九点标定工具"), Description("图像是否启用九点标定")]
    public bool bCalibNPointTool { get; set; } = false;

    [Category("算法"), DisplayName("检测工具"), ReadOnly(true), Description("是否运行检测工具，默认运行")]
    public bool bCheckTool { get; set; } = true;

    [Category("显示配置"), DisplayName("是否显示")]
    public bool bShow { get; set; }

    [Category("显示配置"), DisplayName("显示窗口"), TypeConverter(typeof(DisplayWindowStandardValuesConverter))]
    public string DisplayName { get; set; }

    [Category("显示配置"), DisplayName("显示图层")]
    public int RecoredIndex { get; set; } = 0;


    /// <summary>
    /// 棋盘格标定工具
    /// </summary>
    [Browsable(false)]
    public ToolBase CheckerboardTool { get; set; } = new(new CogToolBlock
    {
        Inputs =
        {
            new CogToolBlockTerminal("Image", typeof(ICogImage), true),
            new CogToolBlockTerminal("Index", typeof(int), true),
            new CogToolBlockTerminal("NarmolMode", typeof(bool), true),
        },
        Outputs =
        {
            new CogToolBlockTerminal("Image", typeof(ICogImage), true),
            new CogToolBlockTerminal("Result", typeof(bool), true)
        }
    }, "棋盘格标定工具.vpp");


    /// <summary>
    /// 九点标定工具
    /// </summary>
    [Browsable(false)]
    public ToolBase NPointTool { get; set; } = new(new CogToolBlock
    {
        Inputs =
        {
            new CogToolBlockTerminal("Image", typeof(ICogImage), true),
            new CogToolBlockTerminal("Index", typeof(int), true),
            new CogToolBlockTerminal("NarmolMode", typeof(bool), true),
        },
        Outputs =
        {
            new CogToolBlockTerminal("Image", typeof(ICogImage), true),
            new CogToolBlockTerminal("Result", typeof(bool), true)
        }
    }, "九点标定工具.vpp");

    /// <summary>
    /// 检测工具
    /// </summary>

    [Browsable(false)]
    public ToolBase DetectionTool { get; set; } = new(new CogToolBlock
    {
        Inputs = { new CogToolBlockTerminal("Image", typeof(ICogImage), true) },
        Outputs = { new CogToolBlockTerminal("Result", typeof(bool), true) ,
            new CogToolBlockTerminal("RecordIndex", typeof(int),true)}
    }, "检测工具.vpp");

    [Browsable(false)] public StationCameraParams CameraParams { get; set; }


    #region 工具类

    public class DetectVarDef
    {
        public string Name { get; set; }
        public string TypeName { get; set; }
        public string Value { get; set; }
        public string Comment { get; set; }
    }

    public class ToolBase
    {
        /// <summary>
        /// vpp文件名
        /// </summary>
        [Browsable(false)]
        public string FileName { get; set; }

        [XmlIgnore] [Browsable(false)] public CogToolBlock ToolBlock { get; set; }

        public List<DetectVarDef> Vars { get; set; } = [];

        public ToolBase()
        {

        }

        public ToolBase(CogToolBlock toolBlock, string fileName)
        {
            ToolBlock = toolBlock;
            FileName = fileName;
        }

        /// <summary>
        /// 将 Vars 中的简单类型参数应用到 ToolBlock.Inputs
        /// 注意：仅处理简单类型（int, double, string, bool 及其数组），不覆盖复杂对象
        /// 用于 WorkFlow 运行前的参数应用
        /// </summary>
        public void ApplyVarsToInputs()
        {
            if (ToolBlock == null || Vars == null) return;
            foreach (var v in Vars)
            {
                if (string.IsNullOrWhiteSpace(v?.Name)) continue;

                // 查找匹配的输入端子
                CogToolBlockTerminal term = null;
                for (var t = 0; t < ToolBlock.Inputs.Count; t++)
                {
                    var tt = ToolBlock.Inputs[t];
                    if (!string.Equals(tt.Name, v.Name, StringComparison.OrdinalIgnoreCase)) continue;
                    term = tt;
                    break;
                }

                if (term == null) continue;

                // 解析目标类型
                var targetType = TypeValueUtil.ResolveType(v.TypeName);
                if (targetType == null)
                {
                    // 不支持的类型（如 List、模型类等），跳过以保留原有值
                    continue;
                }

                // 尝试转换并赋值
                if (TypeValueUtil.TryParseValue(v.Value, targetType, out var obj, out var _))
                {
                    try
                    {
                        // 安全检查：只在以下情况下赋值
                        // 1. 当前值为 null
                        // 2. 当前值类型与目标类型相同或可兼容
                        // 这样可以避免意外覆盖复杂对象（如模型实例）
                        if (term.Value == null ||
                            (term.Value.GetType() == targetType || targetType.IsAssignableFrom(term.Value.GetType())))
                        {
                            term.Value = obj;
                        }
                    }
                    catch
                    {
                        /* 赋值失败时静默忽略，不中断流程 */
                    }
                }
            }
        }

        /// <summary>
        /// 从 ToolBlock.Inputs 同步简单类型参数到 Vars（可选功能，通常不需要调用）
        /// 用于在手动编辑 ToolBlock 后保存简单类型参数
        /// 注意：复杂类型（List、自定义类）不会被同步
        /// </summary>
        public void SyncInputsToVars()
        {
            if (ToolBlock == null) return;

            // 清理已删除的输入端子对应的 Vars
            Vars?.RemoveAll(v =>
            {
                if (string.IsNullOrWhiteSpace(v?.Name)) return true;
                // 检查端子是否还存在
                for (int i = 0; i < ToolBlock.Inputs.Count; i++)
                {
                    if (string.Equals(ToolBlock.Inputs[i].Name, v.Name, StringComparison.OrdinalIgnoreCase))
                        return false; // 端子存在，保留
                }

                return true; // 端子不存在，删除
            });

            // 从 Inputs 更新或添加简单类型的 Vars
            for (int i = 0; i < ToolBlock.Inputs.Count; i++)
            {
                var term = ToolBlock.Inputs[i];
                if (term?.Value == null) continue;

                var valueType = term.Value.GetType();
                var typeName = TypeValueUtil.TypeToName(valueType);

                // 只处理支持的简单类型
                if (string.IsNullOrEmpty(typeName) || TypeValueUtil.ResolveType(typeName) == null)
                    continue;

                // 查找或创建 Var
                var existingVar = Vars?.FirstOrDefault(v =>
                    string.Equals(v?.Name, term.Name, StringComparison.OrdinalIgnoreCase));
                if (existingVar != null)
                {
                    // 更新现有 Var 的值
                    existingVar.Value = TypeValueUtil.ValueToString(term.Value, valueType);
                    existingVar.TypeName = typeName;
                }
                else
                {
                    // 添加新 Var（通常不应该发生，因为 Frm_Tool 已经同步添加）
                    Vars ??= new List<DetectVarDef>();
                    Vars.Add(new DetectVarDef
                    {
                        Name = term.Name,
                        TypeName = typeName,
                        Value = TypeValueUtil.ValueToString(term.Value, valueType),
                        Comment = $"自动同步自输入端子"
                    });
                }
            }
        }
    }

    #endregion
}
