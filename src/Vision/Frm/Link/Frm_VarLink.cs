using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Vision.Solutions.Models;

namespace Vision.Frm.Link
{
  public partial class Frm_VarLink : Form
  {
    private readonly Type _targetType;
    public string SelectedKind { get; private set; } // "Station" | "Global"
    public string SelectedStation { get; private set; }
    public string SelectedOutput { get; private set; }
    public string SelectedGlobal { get; private set; }

    public Frm_VarLink(Type targetType)
    {
      _targetType = targetType ?? typeof(object);
      InitializeComponent();
      btn_OK.Click += (_, _) => { DialogResult = DialogResult.OK; Close(); };
      btn_Cancel.Click += (_, _) => { DialogResult = DialogResult.Cancel; Close(); };
      tree_Stations.AfterSelect += Tree_Stations_AfterSelect;
      dgv_List.AutoGenerateColumns = false;
      dgv_List.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
      dgv_List.RowHeadersVisible = false;
      dgv_List.MultiSelect = false;
      dgv_List.Columns.Add(new DataGridViewTextBoxColumn { Name = "类型", HeaderText = "类型", Width = 80 });
      dgv_List.Columns.Add(new DataGridViewTextBoxColumn { Name = "名称", HeaderText = "名称", Width = 160 });
      dgv_List.Columns.Add(new DataGridViewTextBoxColumn { Name = "值", HeaderText = "值", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
      dgv_List.CellDoubleClick += (_, __) => { if (dgv_List.CurrentRow != null) btn_OK.PerformClick(); };
    }

    private void Frm_VarLink_Load(object sender, EventArgs e)
    {
      BuildTree();
    }

    private void BuildTree()
    {
      tree_Stations.Nodes.Clear();
      var sol = SolutionManager.Instance.Current;
      if (sol == null) return;
      var globalNode = new TreeNode("全局变量") { Tag = "Global" };
      tree_Stations.Nodes.Add(globalNode);
      foreach (var st in sol.Stations ?? new List<StationConfig>())
      {
        if (st == null) continue;
        var node = new TreeNode(st.Name) { Tag = st };
        tree_Stations.Nodes.Add(node);
      }
      if (tree_Stations.Nodes.Count > 0) tree_Stations.SelectedNode = tree_Stations.Nodes[0];
    }

    private void Tree_Stations_AfterSelect(object sender, TreeViewEventArgs e)
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
            var t = term.Value?.GetType();
            if (t == null) continue;
            if (!_targetType.IsAssignableFrom(t)) continue;
            var val = term.Value;
            dgv_List.Rows.Add(t.Name, term.Name, val == null ? "<null>" : val.ToString());
          }
        }
      }
    }

    public void CommitSelection()
    {
      if (dgv_List.CurrentRow == null) return;
      var name = dgv_List.CurrentRow.Cells["名称"].Value?.ToString();
      if (SelectedKind == "Global") { SelectedGlobal = name; }
      else if (SelectedKind == "Station") { SelectedOutput = name; }
    }
  }
}
