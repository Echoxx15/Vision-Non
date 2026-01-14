using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using Cognex.VisionPro;
using Cognex.VisionPro.ToolBlock;
using DnnInterfaceNet;
using Vision.Common;
using Vision.Frm.DLModel;
using Vision.Solutions.Models;

namespace Vision.Frm.Station;

public partial class Frm_Tool : Form
{
  // 将控件中的 Subject 透传为属性，便于外部注入/读取 ToolBlock
  public CogToolBlock ToolBlock
  {
    get => cogToolBlockEditV21.Subject;
    set => cogToolBlockEditV21.Subject = value;
  }

  // 绑定到 Detection.Vars，使 DataGridView 与业务数据联动
  private BindingList<StationConfig.DetectVarDef> _vars;
  private string _originalValueEditing;  // 进入编辑前的值，用于校验/回滚

  private StationConfig.Tools Detection { get; set; }
  
  // 当前工位配置（用于传递给变量链接窗体）
  private StationConfig CurrentStation { get; set; }
  
  // 是否正在同步中（防止递归）
  private bool _isSyncing;

  public Frm_Tool()
  {
    InitializeComponent();
    InitDgv();

    // 变量类型按钮绑定 - 快速添加输入端子
    btn_Bool.Click += (_, _) => AddInputTerminal(typeof(bool));
    btn_Short.Click += (_, _) => AddInputTerminal(typeof(short));
    btn_Int.Click += (_, _) => AddInputTerminal(typeof(int));
    btn_Float.Click += (_, _) => AddInputTerminal(typeof(float));
    btn_Double.Click += (_, _) => AddInputTerminal(typeof(double));
    btn_String.Click += (_, _) => AddInputTerminal(typeof(string));
    btn_BoolArray.Click += (_, _) => AddInputTerminal(typeof(bool[]));
    btn_ShortArray.Click += (_, _) => AddInputTerminal(typeof(short[]));
    btn_IntArray.Click += (_, _) => AddInputTerminal(typeof(int[]));
    btn_FloatArray.Click += (_, _) => AddInputTerminal(typeof(float[]));
    btn_DoubleArray.Click += (_, _) => AddInputTerminal(typeof(double[]));
    btn_StringArray.Click += (_, _) => AddInputTerminal(typeof(string[]));
    btn_Seg.Click += (_, _) => AddDLInputTerminal("语义分割");
    btn_OCR.Click += (_, _) => AddDLInputTerminal("深度OCR");

        // 删除按钮 - 删除当前选中的输入端子
        btn_Remove.Click += (_, _) => RemoveCurrentTerminal();
    
    // 上移/下移按钮 - 移动 DataGridView 当前行
    btn_MoveUp.Click += (_, _) => MoveCurrentVarUp();
    btn_MoveDown.Click += (_, _) => MoveCurrentVarDown();
    
    // 侧边栏收起/展开按钮
    btn_ToggleSidebar.Click += (_, _) => ToggleSidebar();
  }

  /// <summary>
  /// 切换侧边栏的显示/隐藏状态
  /// </summary>
  private void ToggleSidebar()
  {
    bool isCollapsed = splitContainer1.Panel1Collapsed;
    splitContainer1.Panel1Collapsed = !isCollapsed;
    // 更新按钮文字：收起时显示 ▶，展开时显示 ◀
    btn_ToggleSidebar.Text = isCollapsed ? "◀" : "▶";
  }

  // LoadDetection增加工位参数，用于传递给链接窗体
  public void LoadDetection(StationConfig.Tools det, StationConfig station = null)
  {
    // 取消之前的订阅
    UnsubscribeToolBlockEvents();
    
    Detection = det;
    CurrentStation = station;
    _vars = new BindingList<StationConfig.DetectVarDef>(Detection.Vars ?? (Detection.Vars = []));
    dgv_Data.AutoGenerateColumns = false;
    dgv_Data.DataSource = _vars;
    
    // 同步编辑控件
    cogToolBlockEditV21.Subject = Detection.ToolBlock;
    
    // 订阅 ToolBlock 输入端子变化事件（不做初始化同步，信任已保存的 Vars 数据）
    SubscribeToolBlockEvents();
    
    AdjustNoteFill();
  }

  private void Frm_Tool_Load(object sender, EventArgs e)
  {
    AdjustNoteFill();
  }
  
  #region ToolBlock 输入端子事件订阅
  
  private void SubscribeToolBlockEvents()
  {
    if (ToolBlock?.Inputs == null) return;
    
    // 订阅具体的增删改移动事件
    ToolBlock.Inputs.InsertedItem += OnInputItemInserted;
    ToolBlock.Inputs.RemovedItem += OnInputItemRemoved;
    //ToolBlock.Inputs.MovedItem += OnInputItemMoved;
    ToolBlock.Inputs.TrackedItemNameChanging += OnInputItemRenamed;
  }
  
  private void UnsubscribeToolBlockEvents()
  {
    if (ToolBlock?.Inputs == null) return;
    
    ToolBlock.Inputs.InsertedItem -= OnInputItemInserted;
    ToolBlock.Inputs.RemovedItem -= OnInputItemRemoved;
    //ToolBlock.Inputs.MovedItem -= OnInputItemMoved;
    ToolBlock.Inputs.TrackedItemNameChanging -= OnInputItemRenamed;
  }
  
  /// <summary>
  /// 输入端子插入事件
  /// </summary>
  private void OnInputItemInserted(object sender, CogCollectionInsertEventArgs e)
  {
    if (_isSyncing) return;
    if (e.Index >= 0)
      InvokeOnUI(() => HandleItemInserted(e.Index));
  }
  
  /// <summary>
  /// 输入端子删除事件
  /// </summary>
  private void OnInputItemRemoved(object sender, CogCollectionRemoveEventArgs e)
  {
    if (_isSyncing) return;
    var termtTerminal = (CogToolBlockTerminal)e.Value;
    var termName = termtTerminal.Name;
    if (!string.IsNullOrEmpty(termName))
      InvokeOnUI(() => HandleItemRemoved(termName));
  }
  
  /// <summary>
  /// 输入端子重命名事件
  /// </summary>
  private void OnInputItemRenamed(object sender, CogCancelChangingEventArgs<string> e)
  {
    if (_isSyncing || e.Exception != null) return;
    var oldName = e.OldValue;
    var newName = e.NewValue;
    if (!string.IsNullOrEmpty(oldName) && !string.IsNullOrEmpty(newName))
      InvokeOnUI(() => HandleItemRenamed(oldName, newName));
  }
  
  /// <summary>
  /// 在 UI 线程执行
  /// </summary>
  private void InvokeOnUI(Action action)
  {
    if (InvokeRequired)
      BeginInvoke(action);
    else
      action();
  }
  
  /// <summary>
  /// 处理端子插入 - 在指定位置添加新变量
  /// </summary>
  private void HandleItemInserted(int index)
  {
    if (_vars == null || ToolBlock?.Inputs == null) return;
    if (index < 0 || index >= ToolBlock.Inputs.Count) return;
    
    _isSyncing = true;
    try
    {
      var term = ToolBlock.Inputs[index];
      if (term == null || string.IsNullOrWhiteSpace(term.Name)) return;
      
      // 检查是否已存在
      if (_vars.Any(v => string.Equals(v.Name, term.Name, StringComparison.OrdinalIgnoreCase)))
        return;
      
      var type = GetTerminalType(term);
      var newVar = new StationConfig.DetectVarDef
      {
        Name = term.Name,
        TypeName = TypeValueUtil.TypeToName(type),
        Value = GetDefaultValue(type),
        Comment = string.Empty
      };
      
      // 插入到对应位置
      if (index < _vars.Count)
        _vars.Insert(index, newVar);
      else
        _vars.Add(newVar);
      
      dgv_Data.Refresh();
      AdjustNoteFill();
    }
    catch (Exception ex)
    {
      System.Diagnostics.Debug.WriteLine($"[Frm_Tool] HandleItemInserted 异常: {ex.Message}");
    }
    finally
    {
      _isSyncing = false;
    }
  }
  
  /// <summary>
  /// 处理端子删除 - 根据名称移除变量
  /// </summary>
  private void HandleItemRemoved(string termName)
  {
    if (_vars == null) return;
    
    _isSyncing = true;
    try
    {
      var varToRemove = _vars.FirstOrDefault(v => 
        string.Equals(v.Name, termName, StringComparison.OrdinalIgnoreCase));
      if (varToRemove != null)
      {
        _vars.Remove(varToRemove);
        dgv_Data.Refresh();
        AdjustNoteFill();
      }
    }
    catch (Exception ex)
    {
      System.Diagnostics.Debug.WriteLine($"[Frm_Tool] HandleItemRemoved 异常: {ex.Message}");
    }
    finally
    {
      _isSyncing = false;
    }
  }
  
  /// <summary>
  /// 处理端子重命名 - 更新变量名称
  /// </summary>
  private void HandleItemRenamed(string oldName, string newName)
  {
    if (_vars == null) return;
    if (string.IsNullOrWhiteSpace(oldName) || string.IsNullOrWhiteSpace(newName)) return;
    
    _isSyncing = true;
    try
    {
      var varToRename = _vars.FirstOrDefault(v => 
        string.Equals(v.Name, oldName, StringComparison.OrdinalIgnoreCase));
      if (varToRename != null)
      {
        varToRename.Name = newName;
        dgv_Data.Refresh();
      }
    }
    catch (Exception ex)
    {
      System.Diagnostics.Debug.WriteLine($"[Frm_Tool] HandleItemRenamed 异常: {ex.Message}");
    }
    finally
    {
      _isSyncing = false;
    }
  }
  
  /// <summary>
  /// 获取端子的类型
  /// </summary>
  private static Type GetTerminalType(CogToolBlockTerminal term)
  {
    Type t = null;
    try { t = term.Value?.GetType(); } catch { }
    if (t == null)
    {
      try { t = term.GetType().GetProperty("ValueType")?.GetValue(term) as Type; } catch { }
      if (t == null) { try { t = term.GetType().GetProperty("Type")?.GetValue(term) as Type; } catch { } }
    }
    return t ?? typeof(object);
  }
  
  /// <summary>
  /// 获取类型的默认值字符串
  /// </summary>
  private static string GetDefaultValue(Type t)
  {
    if (t == typeof(int) || t == typeof(short) || t == typeof(float) || t == typeof(double)) return "0";
    if (t == typeof(bool)) return "False";
    return string.Empty;
  }
  
  #endregion

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
    // 名称列（只读 - 名称由输入端子决定）
    dgv_Data.Columns.Add(new DataGridViewTextBoxColumn
    {
      DataPropertyName = nameof(StationConfig.DetectVarDef.Name), Name = "名称", HeaderText = "名称", ReadOnly = true,
      Width = 120
    });
    // 值列（可编辑，数组不允许直接编辑）
    dgv_Data.Columns.Add(new DataGridViewTextBoxColumn
    {
      DataPropertyName = nameof(StationConfig.DetectVarDef.Value), Name = "值", HeaderText = "值", ReadOnly = false,
      Width = 100
    });
    // 链接路径列（只读，显示当前链接关系）
    dgv_Data.Columns.Add(new DataGridViewTextBoxColumn
    {
      DataPropertyName = nameof(StationConfig.DetectVarDef.LinkPath), Name = "链接路径", HeaderText = "链接路径", ReadOnly = true,
      Width = 150
    });
    
    // 链接按钮列
    var linkCol = new DataGridViewButtonColumn
    {
      Name = "链接",
      HeaderText = "链接",
      Text = "设置",
      UseColumnTextForButtonValue = true,
      Width = 50
    };
    dgv_Data.Columns.Add(linkCol);
    
    // 注释列（自适应填充）
    var noteCol = new DataGridViewTextBoxColumn
    {
      DataPropertyName = nameof(StationConfig.DetectVarDef.Comment), Name = "注释", HeaderText = "注释", ReadOnly = false,
      MinimumWidth = 80, Width = 100
    };
    dgv_Data.Columns.Add(noteCol);

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
      if (col.DataPropertyName == nameof(StationConfig.DetectVarDef.Value))
      {
        _originalValueEditing = dgv_Data.Rows[e.RowIndex].Cells[e.ColumnIndex].Value?.ToString();
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
      
      // 传递当前工位名称，排除自身输出
      using var frm = new Link.Frm_LinkValue(t, CurrentStation?.Name);
      
      if (frm.ShowDialog(this) == DialogResult.OK)
      {
        // 清空原链接信息
        r.LinkGlobal = null;
        r.LinkStation = null;
        r.LinkOutput = null;
        r.LinkCommDevice = null;
        r.LinkCommInput = null;
        r.LinkPropertyStation = null;
        r.LinkPropertyName = null;
        r.LinkDLModel = null;
        
        if (frm.SelectedKind == "DLModel")
        {
          r.LinkDLModel = frm.SelectedDLModel;
        }
        else if (frm.SelectedKind == "Global")
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
        else if (frm.SelectedKind == "StationProperty")
        {
          r.LinkPropertyStation = frm.SelectedStation;
          r.LinkPropertyName = frm.SelectedStationProperty;
        }
        var parts = new System.Collections.Generic.List<string>();
        if (!string.IsNullOrWhiteSpace(r.LinkDLModel)) parts.Add($"模型[{r.LinkDLModel}]");
        if (!string.IsNullOrWhiteSpace(r.LinkCommDevice) && !string.IsNullOrWhiteSpace(r.LinkCommInput)) parts.Add($"通讯[{r.LinkCommDevice}].输入[{r.LinkCommInput}]");
        if (!string.IsNullOrWhiteSpace(r.LinkPropertyStation) && !string.IsNullOrWhiteSpace(r.LinkPropertyName)) parts.Add($"工位属性[{r.LinkPropertyStation}].{r.LinkPropertyName}");
        if (!string.IsNullOrWhiteSpace(r.LinkGlobal)) parts.Add($"全局[{r.LinkGlobal}]");
        if (!string.IsNullOrWhiteSpace(r.LinkStation) && !string.IsNullOrWhiteSpace(r.LinkOutput)) parts.Add($"工位[{r.LinkStation}].输出[{r.LinkOutput}]");
        if (parts.Count > 0) r.Comment = "链接: " + string.Join("; ", parts);
        dgv_Data.Refresh();
      }
    };
  }

  #region 输入端子操作
  
  /// <summary>
  /// 快速添加输入端子（按钮触发）
  /// </summary>
  private void AddInputTerminal(Type t)
  {
    if (ToolBlock == null)
    {
      MessageBox.Show("尚未加载工具块，无法添加输入端子", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
      return;
    }

    // 生成不冲突的名称：Value + 最小数字，只检查输入端子（输入端子最优先）
    var baseName = "Value";
    var suffix = 0;
    string name;
    do
    {
      name = baseName + suffix++;
    } while (ExistsInInputs(name));

    // 添加到 ToolBlock 输入端子（会触发 InsertedItem 事件，自动同步到 DataGridView）
    ToolBlock.Inputs.Add(new CogToolBlockTerminal(name, t));
  }

  /// <summary>
  /// 添加深度学习输入端子（从全局模型管理器获取）
  /// </summary>
  private void AddDLInputTerminal(string modelCategory)
  {
    if (ToolBlock == null)
    {
      MessageBox.Show("尚未加载工具块，无法添加输入端子", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
      return;
    }

    // 从全局模型工厂获取指定类型的已加载模型
    var models = DnnModelFactory.Instance.GetLoadedModelsByCategory(modelCategory);
    if (models == null || models.Count == 0)
    {
      MessageBox.Show($"未找到已加载的 {modelCategory} 模型\n\n请先在「深度学习模型配置」中添加并加载模型", 
                      "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
      return;
    }

    // 如果有多个模型，让用户选择
    IDnnModel selectedModel;
    if (models.Count == 1)
    {
      selectedModel = models[0];
    }
    else
    {
      // 弹出选择对话框
      var items = models.Select(m => m.Name).ToArray();
      using var dlg = new FrmSelectItem("选择模型", items);
      if (dlg.ShowDialog() != DialogResult.OK || string.IsNullOrEmpty(dlg.SelectedItem))
        return;
      selectedModel = models.FirstOrDefault(m => m.Name == dlg.SelectedItem);
    }

    if (selectedModel == null) return;

    try
    {
      // 生成不冲突的名称 - 使用类名作为基础名称（确保是有效的 .NET 变量名）
      var typeName = selectedModel.GetType().Name; // 如 DnnSemanticSegmentation
      var baseName = typeName;
      var suffix = 0;
      string name;
      do
      {
        name = baseName + (suffix == 0 ? "" : suffix.ToString());
        suffix++;
      } while (ExistsInInputs(name));

      // 创建端子并设置初始值为模型实例
      var terminal = new CogToolBlockTerminal(name, selectedModel.GetType())
      {
          Value = selectedModel
      };

      // 添加到 ToolBlock
      ToolBlock.Inputs.Add(terminal);

      MessageBox.Show($"已添加深度学习端子: {name}\n模型: {selectedModel.Name}", 
                      "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }
    catch (Exception ex)
    {
      MessageBox.Show($"添加深度学习端子失败:\n{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
      System.Diagnostics.Debug.WriteLine($"[Frm_Tool] AddDLInputTerminal 异常: {ex.Message}");
    }
  }

  /// <summary>
  /// 删除当前选中的输入端子
  /// </summary>
  private void RemoveCurrentTerminal()
  {
    if (dgv_Data.CurrentRow == null) return;
    if (ToolBlock == null) return;
    
    var idx = dgv_Data.CurrentRow.Index;
    if (idx < 0 || idx >= _vars.Count) return;
    
    var name = _vars[idx].Name;
    if (string.IsNullOrWhiteSpace(name)) return;
    
    // 检查输入端子是否存在
    var term = FindInputTerminal(name);
    if (term == null)
    {
      // 输入端子不存在，询问是否只删除 DataGridView 行
      if (MessageBox.Show($"输入端子 '{name}' 不存在，是否只删除配置行？", "确认", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
      {
        RemoveVarAt(idx);
      }
      return;
    }
    
    if (MessageBox.Show($"是否删除输入端子 '{name}'?", "确认", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) 
      return;

    // 从 ToolBlock 删除输入端子（会触发 RemovedItem 事件，自动同步到 DataGridView）
    ToolBlock.Inputs.Remove(term);
  }
  
  /// <summary>
  /// 单独删除 DataGridView 中的配置行（不影响输入端子）
  /// 用于清理不匹配的错误数据
  /// </summary>
  private void RemoveVarAt(int index)
  {
    if (_vars == null || index < 0 || index >= _vars.Count) return;
    
    _isSyncing = true;
    try
    {
      _vars.RemoveAt(index);
      dgv_Data.Refresh();
      AdjustNoteFill();
    }
    catch (Exception ex)
    {
      System.Diagnostics.Debug.WriteLine($"[Frm_Tool] RemoveVarAt 异常: {ex.Message}");
    }
    finally
    {
      _isSyncing = false;
    }
  }
  
  /// <summary>
  /// 上移当前选中的配置行
  /// </summary>
  private void MoveCurrentVarUp()
  {
    if (_vars == null || dgv_Data.CurrentRow == null) return;
    
    var idx = dgv_Data.CurrentRow.Index;
    if (idx <= 0 || idx >= _vars.Count) return;  // 第一行无法上移
    
    _isSyncing = true;
    try
    {
      var item = _vars[idx];
      _vars.RemoveAt(idx);
      _vars.Insert(idx - 1, item);
      dgv_Data.ClearSelection();
      dgv_Data.Rows[idx - 1].Selected = true;
      dgv_Data.CurrentCell = dgv_Data.Rows[idx - 1].Cells[0];
    }
    catch (Exception ex)
    {
      System.Diagnostics.Debug.WriteLine($"[Frm_Tool] MoveCurrentVarUp 异常: {ex.Message}");
    }
    finally
    {
      _isSyncing = false;
    }
  }
  
  /// <summary>
  /// 下移当前选中的配置行
  /// </summary>
  private void MoveCurrentVarDown()
  {
    if (_vars == null || dgv_Data.CurrentRow == null) return;
    
    var idx = dgv_Data.CurrentRow.Index;
    if (idx < 0 || idx >= _vars.Count - 1) return;  // 最后一行无法下移
    
    _isSyncing = true;
    try
    {
      var item = _vars[idx];
      _vars.RemoveAt(idx);
      _vars.Insert(idx + 1, item);
      dgv_Data.ClearSelection();
      dgv_Data.Rows[idx + 1].Selected = true;
      dgv_Data.CurrentCell = dgv_Data.Rows[idx + 1].Cells[0];
    }
    catch (Exception ex)
    {
      System.Diagnostics.Debug.WriteLine($"[Frm_Tool] MoveCurrentVarDown 异常: {ex.Message}");
    }
    finally
    {
      _isSyncing = false;
    }
  }

  /// <summary>
  /// 判断输入端子集合中是否存在该名称
  /// </summary>
  private bool ExistsInInputs(string name)
  {
    if (ToolBlock == null) return false;
    for (var i = 0; i < ToolBlock.Inputs.Count; i++)
      if (string.Equals(ToolBlock.Inputs[i].Name, name, StringComparison.OrdinalIgnoreCase))
        return true;
    return false;
  }

  /// <summary>
  /// 根据名称查找输入端子（忽略大小写）
  /// </summary>
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
  
  #endregion

  /// <summary>
  /// 根据可视区域动态调整"注释"列的宽度，使其填满剩余空间
  /// </summary>
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

  /// <summary>
  /// 结束单元格编辑：值列检查与类型匹配
  /// </summary>
  private void dgv_Data_CellEndEdit(object sender, DataGridViewCellEventArgs e)
  {
    if (e.RowIndex < 0 || e.ColumnIndex < 0) return;
    var col = dgv_Data.Columns[e.ColumnIndex];
    if (dgv_Data.Rows[e.RowIndex].DataBoundItem is not StationConfig.DetectVarDef r) return;
    
    if (col.DataPropertyName == nameof(StationConfig.DetectVarDef.Value))
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
  }
}
