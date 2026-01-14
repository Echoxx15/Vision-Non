using System.ComponentModel;
using System.Xml.Serialization;
using Cognex.VisionPro;
using Cognex.VisionPro.ToolBlock;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Vision.Common;
using Vision.LightSource;
using HardwareCommNet;
using DnnInterfaceNet;

namespace Vision.Solutions.Models;

public class StationConfig
{
    [Category("工位"), DisplayName("工位名称")] 
    public string Name { get; set; }
    
    [Category("工位"), DisplayName("是否启用")] 
    public bool Enable { get; set; } = true;

    // ==================== 可链接的工位属性 ====================
    
    /// <summary>
    /// 触发次数（可链接属性），用于判断是否是最后一张图片
    /// </summary>
    [Browsable(false)]
    [LinkableProperty("触发次数", "硬触发模式下的触发次数")]
    public int TriggerCount => CameraParams?.TriggerCount ?? 1;

    /// <summary>
    /// 工位名称（可链接属性）
    /// </summary>
    [Browsable(false)]
    [LinkableProperty("工位名称", "当前工位的名称")]
    public string StationName => Name;

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
    [Category("采集参数"), DisplayName("采集触发延时时间-毫秒")]
    public int TrgDelay { get; set; } = 0;

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

    [Category("通讯配置"), DisplayName("码来源"), Description("作为本次流程Code的来源输入变量(字符串)，若未选择则使用触发命中时间(时分秒)")]
    [TypeConverter(typeof(StationIOTableInputVarConverter))]
    public string CodeInputName { get; set; }

    [Browsable(false)]
    public List<OutputMapping> OutputMappings { get; set; } = [];

    [Category("通讯配置"), DisplayName("通讯输出配置"), ReadOnly(true)]
    [Description("检测工具输出端子到通讯输出变量的映射关系，右键打开通讯输出配置窗口")]
    public string OutputMappingsDisplay
    {
        get
        {
            if (OutputMappings == null || OutputMappings.Count == 0)
                return "<未配置>";
            return $"<已配置{OutputMappings.Count}项映射>";
        }
    }

    // 光源控制
    [Category("光源控制"), DisplayName("光源控制")]
    [Description("配置工位的光源控制参数")]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public StationLightControl LightControl { get; set; } = new();


    // 算法配置
    [Category("算法"), DisplayName("棋盘格标定启用"), Description("是否使用棋盘格标定，右键打开工具配置")]
    public bool bCalibCheckboardTool { get; set; } = false;

    [Category("算法"), DisplayName("九点标定工具启用"), Description("是否使用九点标定，右键打开工具配置")]
    public bool bCalibNPointTool { get; set; } = false;
    [Category("算法"), DisplayName("检测工具启用"), ReadOnly(true),Description("启用检测工具，目前必须启用，右键打开工具配置")]
    public bool bDetectionTool { get; set; } = true;

    // 显示配置
    [Category("显示"), DisplayName("是否显示")]
    public bool bShow { get; set; }

    [Category("显示"), DisplayName("显示窗口"), TypeConverter(typeof(DisplayWindowStandardValuesConverter))]
    public string DisplayName { get; set; }

    [Category("显示"), DisplayName("显示图层")]
    public int RecoredIndex { get; set; }

    // ==================== 工具定义 ====================
    
    /// <summary>
    /// 棋盘格标定工具
    /// </summary>
    [Browsable(false)]
    public Tools CheckerboardTool { get; set; } = new("棋盘格工具.vpp");

    /// <summary>
    /// 九点标定工具
    /// </summary>
    [Browsable(false)]
    public Tools NPointTool { get; set; } = new("九点标定工具.vpp");

    /// <summary>
    /// 检测工具
    /// </summary>
    [Browsable(false)]
    public Tools DetectionTool { get; set; } = new("检测工具.vpp");

    /// <summary>
    /// 检测变量定义（用于工具输入端子的配置）
    /// </summary>
    public class DetectVarDef
    {
        public string Name { get; set; }
        public string TypeName { get; set; }
        public string Value { get; set; }
        public string Comment { get; set; }
        
        // 变量链接：可同时链接工位输出、全局变量、通讯输入、工位属性、深度学习模型
        // 运行时按"深度学习模型（最高）> 通讯输入 > 工位属性 > 工位输出 > 全局变量"的顺序赋值
        public string LinkStation { get; set; }
        public string LinkOutput { get; set; }
        public string LinkGlobal { get; set; }
        
        // 通讯输入链接
        public string LinkCommDevice { get; set; }
        public string LinkCommInput { get; set; }
        
        // 工位属性链接（链接其他工位带 [LinkableProperty] 特性的属性）
        public string LinkPropertyStation { get; set; }
        public string LinkPropertyName { get; set; }
        
        // 深度学习模型链接（链接方案级全局深度学习模型）
        /// <summary>
        /// 链接的深度学习模型名称（从方案的 DLModels 列表中选择）
        /// </summary>
        public string LinkDLModel { get; set; }
        
        /// <summary>
        /// 获取链接路径的显示文本（只读，用于UI显示）
        /// </summary>
        [XmlIgnore]
        public string LinkPath
        {
            get
            {
                // 按优先级顺序：深度学习模型 > 通讯输入 > 工位属性 > 工位输出 > 全局变量
                if (!string.IsNullOrEmpty(LinkDLModel))
                    return $"模型:{LinkDLModel}";
                if (!string.IsNullOrEmpty(LinkCommDevice) && !string.IsNullOrEmpty(LinkCommInput))
                    return $"通讯:{LinkCommDevice}.{LinkCommInput}";
                if (!string.IsNullOrEmpty(LinkPropertyStation) && !string.IsNullOrEmpty(LinkPropertyName))
                    return $"属性:{LinkPropertyStation}.{LinkPropertyName}";
                if (!string.IsNullOrEmpty(LinkStation) && !string.IsNullOrEmpty(LinkOutput))
                    return $"{LinkStation}.{LinkOutput}";
                if (!string.IsNullOrEmpty(LinkGlobal))
                    return $"全局:{LinkGlobal}";
                return string.Empty;
            }
        }
    }

    /// <summary>
    /// 检测工具输出到通讯输出的映射
    /// </summary>
    public class OutputMapping
    {
        /// <summary>
        /// 工具类型（检测工具/棋盘格标定工具/九点标定工具）
        /// </summary>
        public string ToolType { get; set; } = "检测工具";
        
        /// <summary>
        /// 工具输出名称，在 ToolBlock.Outputs 中选择
        /// </summary>
        public string ToolOutputName { get; set; }
        
        /// <summary>
        /// 通讯输出变量名称，在 CommDevice.Table.Outputs 中选择
        /// </summary>
        public string CommOutputName { get; set; }
        
        /// <summary>
        /// 是否每次拍照都发送
        /// true: 每次拍照后立即发送（默认）
        /// false: 仅在最后一次拍照（ImageIndex == TriggerCount）时发送
        /// </summary>
        public bool SendEveryTime { get; set; } = true;
        
        /// <summary>
        /// 备注说明
        /// </summary>
        public string Description { get; set; }
    }

    /// <summary>
    /// 统一的工具类 - 棋盘格、九点标定、检测工具共用此结构
    /// 只有 FileName 和具体的 ToolBlock 内容不同
    /// </summary>
    public class Tools
    {
        /// <summary>
        /// 工具保存的文件名（如 Checkerboard.vpp, NPoint.vpp, Detection.vpp）
        /// </summary>
        [Browsable(false)]
        public string FileName { get; set; } = "Tool.vpp";

        /// <summary>
        /// VisionPro ToolBlock（运行时加载，不序列化到XML）
        /// </summary>
        [XmlIgnore]
        [Browsable(false)]
        public CogToolBlock ToolBlock { get; set; }

        /// <summary>
        /// 输入变量定义列表（用于配置和链接）
        /// </summary>
        public List<DetectVarDef> Vars { get; set; } = [];

        /// <summary>
        /// 无参构造函数（XML序列化需要）
        /// </summary>
        public Tools()
        {
            FileName = "Tool.vpp";
            InitToolBlock();
        }

        /// <summary>
        /// 带文件名的构造函数
        /// </summary>
        /// <param name="fileName">工具文件名</param>
        public Tools(string fileName)
        {
            FileName = fileName;
            InitToolBlock();
        }

        /// <summary>
        /// 初始化默认的 ToolBlock 结构（统一的输入输出端子）
        /// </summary>
        private void InitToolBlock()
        {
            ToolBlock = new CogToolBlock
            {
                Inputs =
                {
                    new CogToolBlockTerminal("Image", typeof(ICogImage), true),
                    new CogToolBlockTerminal("Index", typeof(int), true)
                },
                Outputs =
                {
                    new CogToolBlockTerminal("Result", typeof(bool), true),
                    new CogToolBlockTerminal("Image", typeof(ICogImage), true),
                }
            };
        }

        /// <summary>
        /// 将 Vars 中的链接值应用到 ToolBlock 的输入端子
        /// </summary>
        public void ApplyVarsToInputs()
        {
            if (ToolBlock == null || Vars == null) return;
            
            foreach (var v in Vars)
            {
                // 每个变量单独捕获异常，不影响其他变量的赋值
                try
                {
                    ApplySingleVarToInput(v);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"[ApplyVarsToInputs] 变量 '{v?.Name}' 赋值异常: {ex.Message}");
                }
            }
        }
        
        /// <summary>
        /// 将单个变量应用到对应的输入端子
        /// </summary>
        private void ApplySingleVarToInput(DetectVarDef v)
        {
            if (string.IsNullOrWhiteSpace(v?.Name)) return;
            
            // 查找对应的输入端子
            CogToolBlockTerminal term = null;
            for (var t = 0; t < ToolBlock.Inputs.Count; t++)
            {
                var tt = ToolBlock.Inputs[t];
                if (string.Equals(tt.Name, v.Name, StringComparison.OrdinalIgnoreCase))
                {
                    term = tt;
                    break;
                }
            }
            if (term == null) return;  // 输入端子不存在，跳过
            
            var targetType = TypeValueUtil.ResolveType(v.TypeName);
            if (targetType == null) return;

            object obj = null;
            var haveValue = false;
            
            // 变量链接优先级：深度学习模型 > 通讯输入 > 工位属性 > 工位输出 > 全局变量
            var sol = SolutionManager.Instance.Current;
            
            // 深度学习模型链接（最高优先级）- 优先从新的 DnnModelFactory 获取
            if (!string.IsNullOrWhiteSpace(v.LinkDLModel))
            {
                try
                {
                    // 优先从新的插件系统获取模型
                    var model = DnnModelFactory.Instance.GetModel(v.LinkDLModel);
                    if (model != null && model.IsLoaded)
                    {
                        // 返回 IDnnModel 接口，让工具自行调用 Infer 方法
                        if (targetType.IsAssignableFrom(model.GetType()) || 
                            targetType == typeof(IDnnModel) ||
                            targetType.IsInterface && model.GetType().GetInterfaces().Contains(targetType))
                        {
                            obj = model;
                            haveValue = true;
                        }
                    }
                    
                    if (!haveValue)
                    {
                        System.Diagnostics.Debug.WriteLine($"[ApplySingleVarToInput] 深度学习模型 '{v.LinkDLModel}' 未找到或未加载");
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"[ApplySingleVarToInput] 获取深度学习模型 '{v.LinkDLModel}' 异常: {ex.Message}");
                }
            }
            
            // 通讯输入（第二优先级）- 直接从缓存获取
            if (!haveValue && !string.IsNullOrWhiteSpace(v.LinkCommDevice) && !string.IsNullOrWhiteSpace(v.LinkCommInput))
            {
                try
                {
                    var device = CommunicationFactory.Instance.GetDevice(v.LinkCommDevice);
                    if (device != null)
                    {
                        var cv = device.Table?.GetInputCachedValue(v.LinkCommInput);
                        if (cv != null)
                        {
                            if (targetType.IsAssignableFrom(cv.GetType()))
                            {
                                obj = cv;
                                haveValue = true;
                            }
                            else
                            {
                                try
                                {
                                    obj = Convert.ChangeType(cv, targetType);
                                    haveValue = true;
                                }
                                catch { }
                            }
                        }
                    }
                }
                catch { }
            }
            
            // 工位属性链接（第三优先级）- 通过反射获取带 [LinkableProperty] 特性的属性
            if (!haveValue && !string.IsNullOrWhiteSpace(v.LinkPropertyStation) && !string.IsNullOrWhiteSpace(v.LinkPropertyName))
            {
                try
                {
                    var targetSt = sol?.Stations?.FirstOrDefault(s => string.Equals(s.Name, v.LinkPropertyStation, StringComparison.OrdinalIgnoreCase));
                    if (targetSt != null)
                    {
                        var prop = typeof(StationConfig).GetProperty(v.LinkPropertyName, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                        if (prop != null && prop.GetCustomAttribute<LinkablePropertyAttribute>() != null)
                        {
                            var pv = prop.GetValue(targetSt);
                            if (pv != null)
                            {
                                if (targetType.IsAssignableFrom(pv.GetType()))
                                {
                                    obj = pv;
                                    haveValue = true;
                                }
                                else
                                {
                                    try
                                    {
                                        obj = Convert.ChangeType(pv, targetType);
                                        haveValue = true;
                                    }
                                    catch { }
                                }
                            }
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
                    if (sol.LastOutputs != null && 
                        sol.LastOutputs.TryGetValue(v.LinkStation, out var dict) && 
                        dict != null && 
                        dict.TryGetValue(v.LinkOutput, out var sv) && 
                        sv != null)
                    {
                        if (targetType.IsAssignableFrom(sv.GetType()))
                        {
                            obj = sv;
                            haveValue = true;
                        }
                        else
                        {
                            try
                            {
                                obj = Convert.ChangeType(sv, targetType);
                                haveValue = true;
                            }
                            catch { }
                        }
                    }
                }
                
                // 全局变量（若工位未取到值，则尝试全局）
                if (!haveValue && !string.IsNullOrWhiteSpace(v.LinkGlobal))
                {
                    if (sol.GlobalValues != null && 
                        sol.GlobalValues.TryGetValue(v.LinkGlobal, out var gv) && 
                        gv != null)
                    {
                        if (targetType.IsAssignableFrom(gv.GetType()))
                        {
                            obj = gv;
                            haveValue = true;
                        }
                        else
                        {
                            try
                            {
                                obj = Convert.ChangeType(gv, targetType);
                                haveValue = true;
                            }
                            catch { }
                        }
                    }
                }
            }

            // 若无链接值，则按文本解析默认值
            if (!haveValue)
            {
                if (!TypeValueUtil.TryParseValue(v.Value, targetType, out obj, out _))
                    return;
            }

            // 赋值到输入端子
            if (obj != null)
            {
                try { term.Value = obj; }
                catch { }
            }
        }

        /// <summary>
        /// 从 ToolBlock 的输入端子同步到 Vars 列表
        /// </summary>
        public void SyncInputsToVars()
        {
            if (ToolBlock == null) return;
            
            // 移除 Vars 中不存在于 ToolBlock.Inputs 的项
            Vars?.RemoveAll(v =>
            {
                if (string.IsNullOrWhiteSpace(v?.Name)) return true;
                for (int i = 0; i < ToolBlock.Inputs.Count; i++)
                {
                    if (string.Equals(ToolBlock.Inputs[i].Name, v.Name, StringComparison.OrdinalIgnoreCase))
                        return false;
                }
                return true;
            });
            
            // 同步 ToolBlock.Inputs 到 Vars
            for (int i = 0; i < ToolBlock.Inputs.Count; i++)
            {
                var term = ToolBlock.Inputs[i];
                if (term?.Value == null) continue;
                
                var valueType = term.Value.GetType();
                var typeName = TypeValueUtil.TypeToName(valueType);
                if (string.IsNullOrEmpty(typeName) || TypeValueUtil.ResolveType(typeName) == null)
                    continue;
                
                var existingVar = Vars?.FirstOrDefault(v => 
                    string.Equals(v?.Name, term.Name, StringComparison.OrdinalIgnoreCase));
                
                if (existingVar != null)
                {
                    existingVar.Value = TypeValueUtil.ValueToString(term.Value, valueType);
                    existingVar.TypeName = typeName;
                }
                else
                {
                    Vars ??= [];
                    Vars.Add(new DetectVarDef
                    {
                        Name = term.Name,
                        TypeName = typeName,
                        Value = TypeValueUtil.ValueToString(term.Value, valueType),
                        Comment = "自动同步输入变量"
                    });
                }
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
