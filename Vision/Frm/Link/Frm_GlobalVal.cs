using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using Vision.Common;
using Vision.Solutions.Models;

namespace Vision.Frm.Link;

public partial class Frm_GlobalVal : Form
{
  private readonly BindingList<Row> _rows = new();
  private string _originalNameEditing;
  private string _originalValueEditing;

  private class Row
  {
    public int 序号 { get; set; }
    public string 类型 { get; set; }
    public string 名称 { get; set; }
    public string 值 { get; set; }
    public string 注释 { get; set; }
    public Type RealType { get; set; }
  }

  public Frm_GlobalVal()
  {
    InitializeComponent();
    InitGrid();
  }

  private void InitGrid()
  {
    dgv_Data.AutoGenerateColumns = false;
    dgv_Data.AllowUserToAddRows = false;
    dgv_Data.AllowUserToDeleteRows = false;
    dgv_Data.MultiSelect = false;
    dgv_Data.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
    dgv_Data.RowHeadersVisible = false;
    dgv_Data.Columns.Clear();
    dgv_Data.AllowUserToResizeColumns = true;
    dgv_Data.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;

    dgv_Data.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(Row.序号), HeaderText = "序号", ReadOnly = true, Width = 60 });
    dgv_Data.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(Row.类型), HeaderText = "类型", ReadOnly = true, Width = 100 });
    dgv_Data.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(Row.名称), HeaderText = "名称", ReadOnly = false, Width = 160 });
    dgv_Data.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(Row.值), HeaderText = "值", ReadOnly = false, Width = 160 });
    var colNote = new DataGridViewTextBoxColumn { DataPropertyName = nameof(Row.注释), HeaderText = "注释", ReadOnly = false, MinimumWidth = 200, Width = 300 };
    dgv_Data.Columns.Add(colNote);
    dgv_Data.DataSource = _rows;
    dgv_Data.SizeChanged += (_, _) => AdjustNoteFill();
    dgv_Data.ColumnWidthChanged += (_, e) => { if (e.Column.DataPropertyName != nameof(Row.注释)) AdjustNoteFill(); };

    dgv_Data.CellBeginEdit += (_, e) =>
    {
      if (e.RowIndex < 0 || e.ColumnIndex < 0) return;
      var col = dgv_Data.Columns[e.ColumnIndex];
      var rowObj = dgv_Data.Rows[e.RowIndex].DataBoundItem as Row;
      if (rowObj == null) return;
      if (col.DataPropertyName == nameof(Row.名称))
      {
        _originalNameEditing = dgv_Data.Rows[e.RowIndex].Cells[e.ColumnIndex].Value?.ToString();
      }
      else if (col.DataPropertyName == nameof(Row.值))
      {
        _originalValueEditing = dgv_Data.Rows[e.RowIndex].Cells[e.ColumnIndex].Value?.ToString();
        if (rowObj.RealType != null && rowObj.RealType.IsArray)
        {
          e.Cancel = true; // 数组不允许编辑
        }
      }
    };

    dgv_Data.CellEndEdit += (_, e) =>
    {
      if (e.RowIndex < 0 || e.ColumnIndex < 0) return;
      var col = dgv_Data.Columns[e.ColumnIndex];
      var rowObj = dgv_Data.Rows[e.RowIndex].DataBoundItem as Row;
      if (rowObj == null) return;

      if (col.DataPropertyName == nameof(Row.名称))
      {
        var newName = (rowObj.名称 ?? string.Empty).Trim();
        // 未改动
        if (string.Equals(newName, _originalNameEditing, StringComparison.OrdinalIgnoreCase))
        { rowObj.名称 = _originalNameEditing; return; }
        // 校验
        if (!IsNameValid(newName, e.RowIndex, out var reason))
        {
          MessageBox.Show(reason, "名称无效", MessageBoxButtons.OK, MessageBoxIcon.Warning);
          rowObj.名称 = _originalNameEditing;
          dgv_Data.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = _originalNameEditing;
          return;
        }
        // 确认
        if (MessageBox.Show($"确认将名称修改为：{newName}?", "确认", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
        {
          rowObj.名称 = newName;
          dgv_Data.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = newName;
        }
        else
        {
          rowObj.名称 = _originalNameEditing;
          dgv_Data.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = _originalNameEditing;
        }
      }
      else if (col.DataPropertyName == nameof(Row.值))
      {
        var s = (rowObj.值 ?? string.Empty).Trim();
        if (string.Equals(s, _originalValueEditing, StringComparison.Ordinal)) return; // unchanged
        if (!TypeValueUtil.TryParseValue(s, rowObj.RealType, out var _, out var reason))
        {
          MessageBox.Show(reason, "值无效", MessageBoxButtons.OK, MessageBoxIcon.Warning);
          rowObj.值 = _originalValueEditing;
          dgv_Data.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = _originalValueEditing;
        }
        // 否则合法，已更新字符串形式，实际对象将在保存时写入 GlobalValues
      }
    };

    LoadFromSolution();
  }

  private bool IsNameValid(string name, int currentIndex, out string reason)
  {
    if (string.IsNullOrWhiteSpace(name)) { reason = "名称不能为空"; return false; }
    if (_rows.Where((_, idx) => idx != currentIndex).Any(r => string.Equals(r.名称, name, StringComparison.OrdinalIgnoreCase)))
    { reason = "名称已存在"; return false; }
    reason = null; return true;
  }

  private void AdjustNoteFill()
  {
    if (dgv_Data.Columns.Count == 0) return;
    var note = dgv_Data.Columns[nameof(Row.注释)];
    if (note == null) return;
    int available = dgv_Data.ClientSize.Width - (dgv_Data.RowHeadersVisible ? dgv_Data.RowHeadersWidth : 0);
    var vs = dgv_Data.Controls.OfType<VScrollBar>().FirstOrDefault();
    if (vs != null && vs.Visible) available -= vs.Width;
    int others = 0;
    foreach (DataGridViewColumn c in dgv_Data.Columns) if (c != note && c.Visible) others += c.Width;
    int target = Math.Max(note.MinimumWidth, available - others);
    if (target != note.Width) note.Width = target;
  }

  private void LoadFromSolution()
  {
    _rows.Clear();
    var sol = SolutionManager.Instance.Current;
    if (sol?.Globals == null) return;
    int i = 0;
    foreach (var g in sol.Globals)
    {
      var t = SolutionManager.ResolveType(g.TypeName);
      sol.GlobalValues.TryGetValue(g.Name, out var curVal);
      _rows.Add(new Row { 序号 = ++i, 类型 = g.TypeName, 名称 = g.Name, 值 = TypeValueUtil.ValueToString(curVal, t), 注释 = g.Comment, RealType = t });
    }
    AdjustNoteFill();
  }

  private void AddVar(Type t)
  {
    string baseName = "Value";
    int suffix = 0;
    string name;
    do { name = baseName + suffix++; } while (_rows.Any(r => string.Equals(r.名称, name, StringComparison.OrdinalIgnoreCase)));
    var sol = SolutionManager.Instance.Current;
    // 默认值
    object defObj = null; string defStr = string.Empty;
    if (t == typeof(int)) { defObj = 0; defStr = "0"; }
    else if (t == typeof(double)) { defObj = 0d; defStr = "0"; }
    else if (t == typeof(bool)) { defObj = false; defStr = "False"; }
    else if (t == typeof(string)) { defObj = string.Empty; defStr = string.Empty; }
    // 数组不在界面初始化

    _rows.Add(new Row { 序号 = _rows.Count + 1, 类型 = TypeValueUtil.TypeToName(t), 名称 = name, 值 = defStr, 注释 = string.Empty, RealType = t });
    if (sol != null)
    {
      sol.GlobalValues[name] = defObj;
    }
    AdjustNoteFill();
  }

  private void Renumber()
  {
    for (int i = 0; i < _rows.Count; i++) _rows[i].序号 = i + 1;
  }

  private bool EnsureUniqueName(int rowIndex)
  {
    if (rowIndex < 0 || rowIndex >= _rows.Count) return true;
    var name = _rows[rowIndex].名称?.Trim();
    if (string.IsNullOrWhiteSpace(name)) { MessageBox.Show("名称不能为空"); return false; }
    if (_rows.Where((_, idx) => idx != rowIndex).Any(r => string.Equals(r.名称, name, StringComparison.OrdinalIgnoreCase))) { MessageBox.Show("名称已存在"); return false; }
    _rows[rowIndex].名称 = name; return true;
  }

  private void btn_Int_Click(object sender, EventArgs e) => AddVar(typeof(int));
  private void btn_Double_Click(object sender, EventArgs e) => AddVar(typeof(double));
  private void btn_String_Click(object sender, EventArgs e) => AddVar(typeof(string));
  private void btn_Bool_Click(object sender, EventArgs e) => AddVar(typeof(bool));
  private void btn_IntArray_Click(object sender, EventArgs e) => AddVar(typeof(int[]));
  private void btn_DoubleArray_Click(object sender, EventArgs e) => AddVar(typeof(double[]));
  private void btn_StringArray_Click(object sender, EventArgs e) => AddVar(typeof(string[]));
  private void btn_BoolArray_Click(object sender, EventArgs e) => AddVar(typeof(bool[]));

  private void btn_MoveUp_Click(object sender, EventArgs e)
  {
    if (dgv_Data.CurrentRow == null) return;
    int idx = dgv_Data.CurrentRow.Index; if (idx <= 0) return;
    var item = _rows[idx]; _rows.RemoveAt(idx); _rows.Insert(idx - 1, item); Renumber();
    dgv_Data.CurrentCell = dgv_Data.Rows[idx - 1].Cells[0];
  }

  private void btn_MoveDown_Click(object sender, EventArgs e)
  {
    if (dgv_Data.CurrentRow == null) return;
    int idx = dgv_Data.CurrentRow.Index; if (idx >= _rows.Count - 1) return;
    var item = _rows[idx]; _rows.RemoveAt(idx); _rows.Insert(idx + 1, item); Renumber();
    dgv_Data.CurrentCell = dgv_Data.Rows[idx + 1].Cells[0];
  }

  private void btn_Remove_Click(object sender, EventArgs e)
  {
    if (dgv_Data.CurrentRow == null) return;
    int idx = dgv_Data.CurrentRow.Index; _rows.RemoveAt(idx); Renumber();
  }

  private void btn_Confirm_Click(object sender, EventArgs e)
  {
    // 校验唯一
    for (int i = 0; i < _rows.Count; i++) { if (!EnsureUniqueName(i)) return; }
    // 保存到当前方案
    var sol = SolutionManager.Instance.Current;
    if (sol == null) { MessageBox.Show("无当前方案"); return; }
    sol.Globals = _rows.Select(r => new GlobalVariableDef
    {
      Name = r.名称,
      TypeName = TypeValueUtil.TypeToName(r.RealType),
      Comment = r.注释
    }).ToList();
    // 重建运行时字典（使用界面值，数组维持原值）
    var newDict = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
    foreach (var r in _rows)
    {
      object obj = null;
      if (r.RealType != null && !r.RealType.IsArray)
      {
        if (TypeValueUtil.TryParseValue(r.值 ?? string.Empty, r.RealType, out var parsed, out _)) obj = parsed;
      }
      // 对于数组：保留旧值（可能由程序初始化）
      if (obj == null && sol.GlobalValues.TryGetValue(r.名称, out var existed)) obj = existed;
      newDict[r.名称] = obj;
    }
    sol.GlobalValues = newDict;
    SolutionManager.Instance.SaveCurrent();
    DialogResult = DialogResult.OK;
    Close();
  }

  private void btn_Cancel_Click(object sender, EventArgs e)
  {
    DialogResult = DialogResult.Cancel;
    Close();
  }
}