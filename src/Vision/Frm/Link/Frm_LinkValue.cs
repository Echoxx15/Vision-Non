using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using Vision.Solutions.Models;
using HardwareCommNet;
using DnnInterfaceNet;

namespace Vision.Frm.Link;

public partial class Frm_LinkValue : Form
{
    private readonly Type _targetType;
    private readonly string _excludeStation;
    public string SelectedKind { get; private set; }
    public string SelectedStation { get; private set; }
    public string SelectedOutput { get; private set; }
    public string SelectedGlobal { get; private set; }
    public string SelectedCommDevice { get; private set; }
    public string SelectedCommInput { get; private set; }
    /// <summary>
    /// 选中的工位属性名称（带 [LinkableProperty] 特性的属性）
    /// </summary>
    public string SelectedStationProperty { get; private set; }
    /// <summary>
    /// 选中的深度学习模型名称（从 DnnModelFactory 获取）
    /// </summary>
    public string SelectedDLModel { get; private set; }


    public Frm_LinkValue(Type targetType = null, string excludeStation = null)
    {
        _targetType = targetType ?? typeof(object);
        _excludeStation = excludeStation;
        InitializeComponent();
        treeView1.AfterSelect += TreeView1_AfterSelect;
        btn_Confirm.Click += (_, _) => { CommitSelection(); DialogResult = DialogResult.OK; Close(); };
        btn_Cancel.Click += (_, _) => { DialogResult = DialogResult.Cancel; Close(); };
        dgv_List.CellDoubleClick += (_, __) => { btn_Confirm.PerformClick(); };
        Load += (_, __) => BuildTree();
    }

    private void BuildTree()
    {
        treeView1.Nodes.Clear();
        var sol = SolutionManager.Instance.Current;
        if (sol == null) return;
            
        // 深度学习模型节点（放在最前面，因为是最高优先级）
        // 显示 DnnModelFactory 中的模型
        var newModels = DnnModelFactory.Instance.GetAllModels().ToList();
        if (newModels.Count > 0)
        {
            var dlModelNode = new TreeNode("深度学习模型") { Tag = "DLModel" };
            treeView1.Nodes.Add(dlModelNode);
        }
            
        // 全局变量节点
        var globalNode = new TreeNode("全局变量") { Tag = "Global" };
        treeView1.Nodes.Add(globalNode);
            
        // 通讯设备节点
        var commDevices = CommunicationFactory.Instance.GetAllDevices().ToList();
        if (commDevices.Count > 0)
        {
            var commRootNode = new TreeNode("通讯输入") { Tag = "CommRoot" };
            foreach (var dev in commDevices)
            {
                var devNode = new TreeNode(dev.Name) { Tag = ("CommDevice", dev) };
                commRootNode.Nodes.Add(devNode);
            }
            treeView1.Nodes.Add(commRootNode);
        }
            
        // 工位属性变量节点（带 [LinkableProperty] 特性的属性）
        // ✅ 只显示自身工位（用于链接自身的触发次数等属性）
        if (!string.IsNullOrWhiteSpace(_excludeStation))
        {
            var selfStation = sol.Stations?.FirstOrDefault(s => 
                string.Equals(s?.Name, _excludeStation, StringComparison.OrdinalIgnoreCase));
            if (selfStation != null)
            {
                var stationPropertyRootNode = new TreeNode("工位属性变量") { Tag = "StationPropertyRoot" };
                var stPropNode = new TreeNode(selfStation.Name) { Tag = ("StationProperty", selfStation) };
                stationPropertyRootNode.Nodes.Add(stPropNode);
                treeView1.Nodes.Add(stationPropertyRootNode);
            }
        }
            
        // 工位节点（检测工具输出）
        // ✅ 排除自身工位（不能链接自己工具的输出）
        foreach (var st in sol.Stations ?? new List<StationConfig>())
        {
            if (st == null) continue;
            if (!string.IsNullOrWhiteSpace(_excludeStation) && 
                string.Equals(st.Name, _excludeStation, StringComparison.OrdinalIgnoreCase)) 
                continue;
            var node = new TreeNode(st.Name) { Tag = st };
            treeView1.Nodes.Add(node);
        }
        treeView1.ExpandAll();
        if (treeView1.Nodes.Count > 0) treeView1.SelectedNode = treeView1.Nodes[0];
    }

    private void TreeView1_AfterSelect(object sender, TreeViewEventArgs e)
    {
        dgv_List.Rows.Clear();
        var sol = SolutionManager.Instance.Current;
        if (sol == null) return;

        if (e.Node?.Tag is string s && s == "DLModel")
        {
            // 深度学习模型列表
            SelectedKind = "DLModel"; SelectedStation = null; SelectedOutput = null; SelectedGlobal = null;
            SelectedCommDevice = null; SelectedCommInput = null; SelectedStationProperty = null; SelectedDLModel = null;
                
            // 显示插件系统模型
            var newModels = DnnModelFactory.Instance.GetAllModels().ToList();
                
            foreach (var model in newModels)
            {
                // 类型兼容性检查：目标类型可以是 IDnnModel 接口、具体模型类型、或 object
                var actualModelType = model.GetType();
                bool isCompatible = _targetType == typeof(object) ||
                                    _targetType.IsAssignableFrom(actualModelType) ||
                                    _targetType.IsAssignableFrom(typeof(IDnnModel)) ||
                                    typeof(IDnnModel).IsAssignableFrom(_targetType);
                if (!isCompatible) continue;
                    
                // 获取加载状态
                var statusStr = model.IsLoaded ? "<已加载>" : "<未加载>";
                    
                // 获取模型类型名称
                var typeAttr = model.GetType().GetCustomAttribute<DnnModelTypeAttribute>();
                var typeName = typeAttr?.TypeName ?? model.GetType().Name;
                    
                var rowIdx = dgv_List.Rows.Add(typeName, model.Name, statusStr);
                dgv_List.Rows[rowIdx].Tag = model.Name; // 存储模型名称
            }
        }
        else if (e.Node?.Tag is string s2 && s2 == "Global")
        {
            SelectedKind = "Global"; SelectedStation = null; SelectedOutput = null; SelectedGlobal = null;
            SelectedCommDevice = null; SelectedCommInput = null; SelectedStationProperty = null; SelectedDLModel = null;
            foreach (var g in sol.Globals ?? new List<GlobalVariableDef>())
            {
                var t = SolutionManager.ResolveType(g.TypeName);
                if (t == null) continue; 
                // ✅ 修复：使用IsAssignableFrom判断类型兼容性（支持数组、继承关系）
                if (!_targetType.IsAssignableFrom(t)) continue;
                object v = null;
                sol.GlobalValues?.TryGetValue(g.Name, out v);
                dgv_List.Rows.Add(t.Name, g.Name, v == null ? "<null>" : v.ToString());
            }
        }
        else if (e.Node?.Tag is ValueTuple<string, IComm> commTag && commTag.Item1 == "CommDevice")
        {
            // 通讯设备输入
            SelectedKind = "Comm"; SelectedStation = null; SelectedOutput = null; SelectedGlobal = null;
            SelectedCommDevice = commTag.Item2.Name; SelectedCommInput = null; SelectedStationProperty = null; SelectedDLModel = null;
            var inputs = commTag.Item2.Table?.Inputs;
            if (inputs != null)
            {
                foreach (var cell in inputs)
                {
                    var t = cell.RealType;
                    if (t == null) continue;
                    if (!_targetType.IsAssignableFrom(t)) continue;
                    // 直接获取缓存值（轮询线程已经在持续更新）
                    var valStr = "<未读取>";
                    try
                    {
                        var v = cell.CachedValue;
                        valStr = v == null ? "<null>" : v.ToString();
                    }
                    catch { }
                    dgv_List.Rows.Add(t.Name, cell.Name, valStr);
                }
            }
        }
        else if (e.Node?.Tag is ValueTuple<string, StationConfig> stPropTag && stPropTag.Item1 == "StationProperty")
        {
            // 工位属性变量（带 [LinkableProperty] 特性的属性）
            var st = stPropTag.Item2;
            SelectedKind = "StationProperty"; SelectedStation = st.Name; SelectedOutput = null; SelectedGlobal = null;
            SelectedCommDevice = null; SelectedCommInput = null; SelectedStationProperty = null; SelectedDLModel = null;
                
            // 使用反射获取带有 LinkablePropertyAttribute 的属性
            var props = typeof(StationConfig).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var prop in props)
            {
                var attr = prop.GetCustomAttribute<LinkablePropertyAttribute>();
                if (attr == null) continue;
                    
                var propType = prop.PropertyType;
                // 类型兼容性检查
                if (!_targetType.IsAssignableFrom(propType)) continue;
                    
                var displayName = string.IsNullOrEmpty(attr.DisplayName) ? prop.Name : attr.DisplayName;
                var valStr = "<null>";
                try
                {
                    var v = prop.GetValue(st);
                    valStr = v == null ? "<null>" : v.ToString();
                }
                catch { }
                    
                // 在dgv中存储属性名用于后续选择
                var rowIdx = dgv_List.Rows.Add(propType.Name, displayName, valStr);
                dgv_List.Rows[rowIdx].Tag = prop.Name; // 存储真实属性名
            }
        }
        else if (e.Node?.Tag is StationConfig st)
        {
            SelectedKind = "Station"; SelectedStation = st.Name; SelectedOutput = null; SelectedGlobal = null;
            SelectedCommDevice = null; SelectedCommInput = null; SelectedStationProperty = null; SelectedDLModel = null;
            var tb = st.DetectionTool?.ToolBlock;
            if (tb != null)
            {
                for (int i = 0; i < tb.Outputs.Count; i++)
                {
                    var term = tb.Outputs[i];
                    Type t = null;
                    try { t = term.Value?.GetType(); } catch { }
                    if (t == null)
                    {
                        try { t = term.GetType().GetProperty("ValueType")?.GetValue(term) as Type; } catch { }
                        if (t == null) { try { t = term.GetType().GetProperty("Type")?.GetValue(term) as Type; } catch { } }
                    }
                    if (t == null) t = typeof(object);
                        
                    // ✅ 修复：使用IsAssignableFrom判断类型兼容性（支持数组、继承关系）
                    if (!_targetType.IsAssignableFrom(t)) continue;
                        
                    var valStr = "<null>";
                    try { var v = term.Value; valStr = v == null ? "<null>" : v.ToString(); } catch { }
                    dgv_List.Rows.Add(t.Name, term.Name, valStr);
                }
            }
        }
    }

    private void CommitSelection()
    {
        if (dgv_List.CurrentRow == null) return;
        var name = dgv_List.CurrentRow.Cells["名称"].Value?.ToString();
        if (SelectedKind == "DLModel")
        {
            // 使用存储在Row.Tag中的模型名称
            var modelName = dgv_List.CurrentRow.Tag as string;
            SelectedDLModel = modelName ?? name;
        }
        else if (SelectedKind == "Global") SelectedGlobal = name; 
        else if (SelectedKind == "Station") SelectedOutput = name;
        else if (SelectedKind == "Comm") SelectedCommInput = name;
        else if (SelectedKind == "StationProperty")
        {
            // 使用存储在Row.Tag中的真实属性名
            var propName = dgv_List.CurrentRow.Tag as string;
            SelectedStationProperty = propName ?? name;
        }
    }
}
