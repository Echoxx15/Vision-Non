using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Vision.Solutions.Models;

namespace Vision.Frm.Link
{
    public partial class Frm_LinkValue : Form
    {
        private readonly Type _targetType;
        private readonly string _excludeStation;
        public string SelectedKind { get; private set; }
        public string SelectedStation { get; private set; }
        public string SelectedOutput { get; private set; }
        public string SelectedGlobal { get; private set; }


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
            var globalNode = new TreeNode("全局变量") { Tag = "Global" };
            treeView1.Nodes.Add(globalNode);
            foreach (var st in sol.Stations ?? new List<StationConfig>())
            {
                if (st == null) continue;
                // ✅ 修复：排除当前工位自身
                if (!string.IsNullOrWhiteSpace(_excludeStation) && string.Equals(st.Name, _excludeStation, StringComparison.OrdinalIgnoreCase)) continue;
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

            if (e.Node?.Tag is string s && s == "Global")
            {
                SelectedKind = "Global"; SelectedStation = null; SelectedOutput = null; SelectedGlobal = null;
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
            else if (e.Node?.Tag is StationConfig st)
            {
                SelectedKind = "Station"; SelectedStation = st.Name; SelectedOutput = null; SelectedGlobal = null;
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
            if (SelectedKind == "Global") SelectedGlobal = name; else if (SelectedKind == "Station") SelectedOutput = name;
        }
    }
}
