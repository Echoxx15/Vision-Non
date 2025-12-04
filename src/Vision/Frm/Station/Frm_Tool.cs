using System;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Cognex.VisionPro.ToolBlock;
using Vision.Common;
using Vision.Solutions.Models;

namespace Vision.Frm.Process;

public partial class Frm_Tool : Form
{
  // 将控件中的 Subject 透传为属性，便于外部注入/读取 ToolBlock
  public CogToolBlock ToolBlock
  {
    get => cogToolBlockEditV21.Subject;
    set => cogToolBlockEditV21.Subject = value;
  }

  // 绑定到 Detection.Vars，使 DataGridView 与业务数据联动
  private BindingList<StationConfig.DetectVarDef> _vars; // 绑定到 Detection.Vars
  private string _originalNameEditing;   // 进入编辑前的名称，用于校验/回滚
  private string _originalValueEditing;  // 进入编辑前的值，用于校验/回滚

  private StationConfig.ToolBase Detection { get; set; }
  
  // ✅ 添加：当前工位配置（用于传递给变量链接窗体）
  private StationConfig CurrentStation { get; set; }

  // C# 关键字（禁止使用）
  private static readonly System.Collections.Generic.HashSet<string> CsKeywords = new([
    "abstract", "as", "base", "bool", "break", "byte", "case", "catch", "char", "checked", "class", "const", "continue",
    "decimal", "default", "delegate", "do", "double", "else", "enum", "event", "explicit", "extern", "false", "finally",
    "fixed", "float", "for", "foreach", "goto", "if", "implicit", "in", "int", "interface", "internal", "is", "lock",
    "long", "namespace", "new", "null", "object", "operator", "out", "override", "params", "private", "protected",
    "public", "readonly", "ref", "return", "sbyte", "sealed", "short", "sizeof", "stackalloc", "static", "string",
    "struct", "switch", "this", "throw", "true", "try", "typeof", "uint", "ulong", "unchecked", "unsafe", "ushort",
    "using", "virtual", "void", "volatile", "while"
  ], StringComparer.Ordinal);

  public Frm_Tool()
  {
    InitializeComponent();
    InitDgv();

    // 变量类型按钮绑定 - 类型以 HSL 通讯库支持的类型为准
    btn_Bool.Click += (_, _) => AddVar(typeof(bool));
    btn_Short.Click += (_, _) => AddVar(typeof(short));
    btn_Int.Click += (_, _) => AddVar(typeof(int));
    btn_Float.Click += (_, _) => AddVar(typeof(float));
    btn_Double.Click += (_, _) => AddVar(typeof(double));
    btn_String.Click += (_, _) => AddVar(typeof(string));
    btn_BoolArray.Click += (_, _) => AddVar(typeof(bool[]));
    btn_ShortArray.Click += (_, _) => AddVar(typeof(short[]));
    btn_IntArray.Click += (_, _) => AddVar(typeof(int[]));
    btn_FloatArray.Click += (_, _) => AddVar(typeof(float[]));
    btn_DoubleArray.Click += (_, _) => AddVar(typeof(double[]));
    btn_StringArray.Click += (_, _) => AddVar(typeof(string[]));

    // 行移动与删除
    btn_MoveUp.Click += (_, _) => MoveUp();
    btn_MoveDown.Click += (_, _) => MoveDown();
    btn_Remove.Click += (_, _) => RemoveCurrent();
  }

  // ✅ 修改：LoadDetection增加工位参数，用于传递给链接窗体
  public void LoadDetection(StationConfig.ToolBase det, StationConfig station = null)
  {
    Detection = det;
    CurrentStation = station;
    _vars = new BindingList<StationConfig.DetectVarDef>(Detection.Vars ?? (Detection.Vars = []));
    dgv_Data.AutoGenerateColumns = false;
    dgv_Data.DataSource = _vars;
    AdjustNoteFill();
    cogToolBlockEditV21.Subject = Detection.ToolBlock; // 同步编辑控件
  }

  private void Frm_Tool_Load(object sender, EventArgs e)
  {
    AdjustNoteFill();
  }

  private void InitDgv()
  {
    dgv_Data.AutoGenerateColumns = false;
    dgv_Data.Columns.Clear();
    dgv_Data.AllowUserToAddRows = false;
    dgv_Data.AllowUserToDeleteRows = false;
    dgv_Data.MultiSelect = false;
    dgv_Data.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
    dgv_Data.RowHeadersVisible = false;

    // 类型列（只读）
    dgv_Data.Columns.Add(new DataGridViewTextBoxColumn
    {
      DataPropertyName = nameof(StationConfig.DetectVarDef.TypeName), Name = "类型", HeaderText = "类型", ReadOnly = true,
      Width = 60
    });
    // 名称列（可编辑）
    dgv_Data.Columns.Add(new DataGridViewTextBoxColumn
    {
      DataPropertyName = nameof(StationConfig.DetectVarDef.Name), Name = "名称", HeaderText = "名称", ReadOnly = false,
      Width = 120
    });
    // 值列（可编辑，数组不允许直接编辑）
    dgv_Data.Columns.Add(new DataGridViewTextBoxColumn
    {
      DataPropertyName = nameof(StationConfig.DetectVarDef.Value), Name = "值", HeaderText = "值", ReadOnly = false,
      Width = 100
    });
    // 注释列（自适应填充）
    var noteCol = new DataGridViewTextBoxColumn
    {
      DataPropertyName = nameof(StationConfig.DetectVarDef.Comment), Name = "注释", HeaderText = "注释", ReadOnly = false,
      MinimumWidth = 100, Width = 120
    };
    dgv_Data.Columns.Add(noteCol);

    // 链接列（按钮）
    var linkCol = new DataGridViewButtonColumn
    {
      Name = "链接",
      HeaderText = "链接",
      Text = "设置",
      UseColumnTextForButtonValue = true,
      Width = 60
    };
    dgv_Data.Columns.Add(linkCol);

    // 列宽变化与控件大小变化时，自适应注释列
    dgv_Data.SizeChanged += (_, _) => AdjustNoteFill();
    dgv_Data.ColumnWidthChanged += (_, e) =>
    {
      if (e.Column.Name != "注释") AdjustNoteFill();
    };

    // 编辑行为：记录原值，以便校验失败回滚
    dgv_Data.CellBeginEdit += (_, e) =>
    {
      if (e.RowIndex < 0 || e.ColumnIndex < 0) return;
      var col = dgv_Data.Columns[e.ColumnIndex];
      switch (col.DataPropertyName)
      {
        case nameof(StationConfig.DetectVarDef.Name):
          _originalNameEditing = dgv_Data.Rows[e.RowIndex].Cells[e.ColumnIndex].Value?.ToString();
          break;
        case nameof(StationConfig.DetectVarDef.Value):
          _originalValueEditing = dgv_Data.Rows[e.RowIndex].Cells[e.ColumnIndex].Value?.ToString();
          break;
      }
    };

    // 链接按钮点击
    dgv_Data.CellContentClick += (sender, e) =>
    {
      if (e.RowIndex < 0 || e.ColumnIndex < 0) return;
      var col = dgv_Data.Columns[e.ColumnIndex];
      if (col.Name != "链接") return;
      if (dgv_Data.Rows[e.RowIndex].DataBoundItem is not StationConfig.DetectVarDef r) return;
      var t = TypeValueUtil.ResolveType(r.TypeName);
      if (t == null)
      {
        MessageBox.Show("变量类型无效，无法链接", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        return;
      }
      
      // ✅ 修复：传递当前工位名称，排除自身输出
      using var frm = new Vision.Frm.Link.Frm_LinkValue(t, CurrentStation?.Name);
      
      if (frm.ShowDialog(this) == DialogResult.OK)
      {
        // ✅ 清空原链接信息
        r.LinkGlobal = null;
        r.LinkStation = null;
        r.LinkOutput = null;
        r.LinkCommDevice = null;
        r.LinkCommInput = null;
        
        if (frm.SelectedKind == "Global")
        {
          r.LinkGlobal = frm.SelectedGlobal;
        }
        else if (frm.SelectedKind == "Station")
        {
          r.LinkStation = frm.SelectedStation; 
          r.LinkOutput = frm.SelectedOutput;
        }
        else if (frm.SelectedKind == "Comm")
        {
          r.LinkCommDevice = frm.SelectedCommDevice;
          r.LinkCommInput = frm.SelectedCommInput;
        }
        var parts = new System.Collections.Generic.List<string>();
        if (!string.IsNullOrWhiteSpace(r.LinkCommDevice) && !string.IsNullOrWhiteSpace(r.LinkCommInput)) parts.Add($"通讯[{r.LinkCommDevice}].输入[{r.LinkCommInput}]");
        if (!string.IsNullOrWhiteSpace(r.LinkGlobal)) parts.Add($"全局[{r.LinkGlobal}]");
        if (!string.IsNullOrWhiteSpace(r.LinkStation) && !string.IsNullOrWhiteSpace(r.LinkOutput)) parts.Add($"工位[{r.LinkStation}].输出[{r.LinkOutput}]");
        if (parts.Count > 0) r.Comment = "链接: " + string.Join("; ", parts);
        dgv_Data.Refresh();
      }
    };
  }

  // 名称合法性校验：
  // 1. 非空 2. 符合 C# 标识符 3. 非关键字 4. 当前列表唯一 5. 不与 ToolBlock 输入端子冲突
  private bool IsNameValid(string name, int currentIndex, out string reason)
  {
    if (string.IsNullOrWhiteSpace(name))
    {
      reason = "名称不能为空";
      return false;
    }

    // C# 标识符规范（仅允许 ASCII 字母、数字、下划线；且必须以字母或下划线开头）
    if (!Regex.IsMatch(name, @"^[A-Za-z_][A-Za-z0-9_]*$"))
    {
      reason = "名称不合法：必须以字母或下划线开头，且只能包含字母、数字或下划线";
      return false;
    }

    // 关键字禁止
    if (CsKeywords.Contains(name))
    {
      reason = "名称不合法：不能使用C#关键字";
      return false;
    }

    // 界面数据源内唯一
    if (_vars.Where((_, idx) => idx != currentIndex)
        .Any(v => string.Equals(v.Name, name, StringComparison.OrdinalIgnoreCase)))
    {
      reason = "名称已存在";
      return false;
    }

    // 与 CogToolBlock.Inputs 冲突则不合法（不操作 Inputs，仅校验）
    if (ToolBlock != null)
    {
      for (var i = 0; i < ToolBlock.Inputs.Count; i++)
      {
        if (string.Equals(ToolBlock.Inputs[i].Name, name, StringComparison.OrdinalIgnoreCase))
        {
          reason = "名称与输入端子冲突";
          return false;
        }
      }
    }

    reason = null;
    return true;
  }

  // 添加变量：
  // - 自动生成不冲突名称；
  // - 写入变量列表与 ToolBlock 输入端子；
  // - UI 自适应列宽。
  private void AddVar(Type t)
  {
    if (ToolBlock == null)
    {
      MessageBox.Show("尚未加载工具块，无法添加变量", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
      return;
    }

    var baseName = "Value";
    var suffix = 0;
    string name;
    do
    {
      name = baseName + suffix++;
    } while (_vars.Any(v => string.Equals(v.Name, name, StringComparison.OrdinalIgnoreCase)) || ExistsInInputs(name));

    var typeName = TypeValueUtil.TypeToName(t);
    var defStr = t == typeof(int) || t == typeof(double) ? "0" : (t == typeof(bool) ? "False" : string.Empty);
    _vars.Add(new StationConfig.DetectVarDef
      { Name = name, TypeName = typeName, Value = defStr, Comment = string.Empty });

    // 与列表同步新增输入端子
    ToolBlock.Inputs.Add(new CogToolBlockTerminal(name, t));
    AdjustNoteFill();
  }

  // 判断输入端子集合中是否存在该名称
  private bool ExistsInInputs(string name)
  {
    if (ToolBlock == null) return false;
    for (var i = 0; i < ToolBlock.Inputs.Count; i++)
      if (string.Equals(ToolBlock.Inputs[i].Name, name, StringComparison.OrdinalIgnoreCase))
        return true;
    return false;
  }

  // 根据名称查找输入端子（忽略大小写）；不存在返回 null
  private CogToolBlockTerminal FindInputTerminal(string name)
  {
    if (ToolBlock == null || string.IsNullOrWhiteSpace(name)) return null;
    for (var i = 0; i < ToolBlock.Inputs.Count; i++)
    {
      var t = ToolBlock.Inputs[i];
      if (string.Equals(t?.Name, name, StringComparison.OrdinalIgnoreCase)) return t;
    }
    return null;
  }

  private void MoveUp()
  {
    if (dgv_Data.CurrentRow == null) return;
    var idx = dgv_Data.CurrentRow.Index;
    if (idx <= 0) return;
    var item = _vars[idx];
    _vars.RemoveAt(idx);
    _vars.Insert(idx - 1, item);
    dgv_Data.CurrentCell = dgv_Data.Rows[idx - 1].Cells[0];
  }

  private void MoveDown()
  {
    if (dgv_Data.CurrentRow == null) return;
    var idx = dgv_Data.CurrentRow.Index;
    if (idx >= _vars.Count - 1) return;
    var item = _vars[idx];
    _vars.RemoveAt(idx);
    _vars.Insert(idx + 1, item);
    dgv_Data.CurrentCell = dgv_Data.Rows[idx + 1].Cells[0];
  }

  private void RemoveCurrent()
  {
    if (dgv_Data.CurrentRow == null) return;
    if (MessageBox.Show("是否删除当前行?", "确认", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
    var idx = dgv_Data.CurrentRow.Index;
    var name = _vars[idx].Name;
    _vars.RemoveAt(idx);

    // 尝试同步移除 ToolBlock 的输入端子（若存在）
    try
    {
      if (ToolBlock != null)
      {
        // 优先按名称查找终端并移除
        var term = FindInputTerminal(name);
        if (term != null)
        {
          ToolBlock.Inputs.Remove(term);
        }
        else
        {
          // 某些版本可直接按名称移除
          ToolBlock.Inputs.Remove(name);
        }
      }
    }
    catch (Exception)
    {
      // ignored：对 UI 连贯性影响小，不中断
    }
  }

  // 根据可视区域动态调整“注释”列的宽度，使其填满剩余空间
  private void AdjustNoteFill()
  {
    if (dgv_Data.Columns.Count == 0) return;
    var note = dgv_Data.Columns["注释"];
    if (note == null) return;
    var available = dgv_Data.ClientSize.Width - (dgv_Data.RowHeadersVisible ? dgv_Data.RowHeadersWidth : 0);
    var vs = dgv_Data.Controls.OfType<VScrollBar>().FirstOrDefault();
    if (vs is { Visible: true }) available -= vs.Width;
    var others = 0;
    foreach (DataGridViewColumn c in dgv_Data.Columns)
      if (c != note && c.Visible)
        others += c.Width;
    var target = Math.Max(100, available - others);
    if (target != note.Width)
      note.Width = target;
  }

  // 结束单元格编辑：
  // - 名称：校验合法性并与 ToolBlock 输入端子同步改名；
  // - 值：检查与类型匹配（数组类型禁止直接编辑）。
  private void dgv_Data_CellEndEdit(object sender, DataGridViewCellEventArgs e)
  {
    if (e.RowIndex < 0 || e.ColumnIndex < 0) return;
    var col = dgv_Data.Columns[e.ColumnIndex];
    if (dgv_Data.Rows[e.RowIndex].DataBoundItem is not StationConfig.DetectVarDef r) return;
    if (col.DataPropertyName == nameof(StationConfig.DetectVarDef.Name))
    {
      var newName = (r.Name ?? string.Empty).Trim();
      if (string.Equals(newName, _originalNameEditing, StringComparison.OrdinalIgnoreCase))
      {
        r.Name = _originalNameEditing;
        return;
      }

      if (!IsNameValid(newName, e.RowIndex, out var reason))
      {
        MessageBox.Show(reason, "名称无效", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        r.Name = _originalNameEditing;
        dgv_Data.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = _originalNameEditing;
        return;
      }

      if (MessageBox.Show($"确认将名称修改为：{newName}?", "确认", MessageBoxButtons.YesNo, MessageBoxIcon.Question) ==
          DialogResult.Yes)
      {
        r.Name = newName;
        dgv_Data.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = newName;
        // 同步修改 ToolBlock 输入端子的名称（忽略大小写查找）
        if (ToolBlock != null && !string.IsNullOrWhiteSpace(_originalNameEditing))
        {
          var term = FindInputTerminal(_originalNameEditing);
          if (term != null)
          {
            term.Name = newName;
          }
          else
          {
            // 兼容性兜底：某些版本可通过索引器按名称访问，但不保证存在
            try { ToolBlock.Inputs[_originalNameEditing].Name = newName; } catch { }
          }
        }
      }
      else
      {
        r.Name = _originalNameEditing;
        dgv_Data.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = _originalNameEditing;
      }
    }
    else if (col.DataPropertyName == nameof(StationConfig.DetectVarDef.Value))
    {
      var s = (r.Value ?? string.Empty).Trim();
      var t = TypeValueUtil.ResolveType(r.TypeName);
      if (t is { IsArray: true })
      {
        // 数组不允许编辑
        r.Value = _originalValueEditing;
        dgv_Data.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = _originalValueEditing;
        return;
      }

      if (string.Equals(s, _originalValueEditing, StringComparison.Ordinal)) return;
      if (!TypeValueUtil.TryParseValue(s, t, out var _, out var reason))
      {
        MessageBox.Show(reason, "值无效", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        r.Value = _originalValueEditing;
        dgv_Data.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = _originalValueEditing;
      }
    }
  }

  private void Frm_Tool_FormClosed(object sender, FormClosedEventArgs e)
  {
    // 关闭窗体时同步回写 ToolBlock 到内存（不保存到磁盘）
    if (Detection != null)
    {
      Detection.ToolBlock = cogToolBlockEditV21.Subject;
    }
  }
}
