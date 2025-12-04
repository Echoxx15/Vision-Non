using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using DnnInterfaceNet;

namespace Vision.Frm.DLModel;

public partial class Frm_DLModelConfig : Form
{
    private IDnnModel _selectedModel;
    private UserControl _currentConfigControl;
    private BindingList<DLModelItem> _modelList;
    private string _editingOriginalName;

    public Frm_DLModelConfig()
    {
        InitializeComponent();
        InitializeDataGridView();
        InitializeContextMenu();
        InitializeEvents();
        LoadExistingModels();
    }

    private void InitializeDataGridView()
    {
        _modelList = new BindingList<DLModelItem>();
        dgv_Models.AutoGenerateColumns = false;
        dgv_Models.DataSource = _modelList;
        col_Name.ReadOnly = false;
        col_Type.ReadOnly = true;
        col_Status.ReadOnly = true;
        dgv_Models.RowTemplate.Height = 32;
    }

    private void InitializeContextMenu()
    {
        contextMenuStrip1.Items.Clear();
        var modelTypes = DnnModelFactory.Instance.GetAllModelTypes();

        if (modelTypes.Count == 0)
        {
            var emptyItem = new ToolStripMenuItem("(无可用模型插件)") { Enabled = false };
            contextMenuStrip1.Items.Add(emptyItem);
        }
        else
        {
            foreach (var typeName in modelTypes)
            {
                var menuItem = new ToolStripMenuItem($"添加 {typeName}") { Tag = typeName };
                menuItem.Click += MenuItem_AddModel_Click;
                contextMenuStrip1.Items.Add(menuItem);
            }
        }

        btn_Add.ContextMenuStrip = contextMenuStrip1;
    }

    private void InitializeEvents()
    {
        btn_Add.Click += (_, _) => contextMenuStrip1.Show(btn_Add, 0, btn_Add.Height);
        btn_Remove.Click += BtnRemove_Click;
        btn_Save.Click += BtnSave_Click;
        this.FormClosing += Frm_DLModelConfig_FormClosing;
        dgv_Models.SelectionChanged += DgvModels_SelectionChanged;
        dgv_Models.CellBeginEdit += DgvModels_CellBeginEdit;
        dgv_Models.CellEndEdit += DgvModels_CellEndEdit;

        // 订阅工厂事件
        DnnModelFactory.Instance.ModelCreated += OnModelCreated;
        DnnModelFactory.Instance.ModelRemoved += OnModelRemoved;
        DnnModelFactory.Instance.ModelRenamed += OnModelRenamed;
    }

    private void LoadExistingModels()
    {
        _modelList.Clear();
        foreach (var model in DnnModelFactory.Instance.GetAllModels())
        {
            var item = new DLModelItem
            {
                Name = model.Name,
                Type = GetModelTypeName(model),
                Status = model.IsLoaded ? "已加载" : "未加载",
                Model = model
            };
            _modelList.Add(item);
        }

        if (_modelList.Count > 0)
        {
            dgv_Models.ClearSelection();
            dgv_Models.Rows[0].Selected = true;
        }
    }

    private string GetModelTypeName(IDnnModel model)
    {
        var attr = model.GetType().GetCustomAttributes(typeof(DnnModelTypeAttribute), false)
            .FirstOrDefault() as DnnModelTypeAttribute;
        return attr?.TypeName ?? model.GetType().Name;
    }

    #region 添加/删除模型

    private void MenuItem_AddModel_Click(object sender, EventArgs e)
    {
        if (sender is not ToolStripMenuItem mi || mi.Tag is not string typeName) return;

        try
        {
            var uniqueName = GenerateUniqueName(typeName);
            var model = DnnModelFactory.Instance.CreateModel(typeName, uniqueName);
            if (model == null) return;

            var item = new DLModelItem
            {
                Name = uniqueName,
                Type = typeName,
                Status = "未加载",
                Model = model
            };
            _modelList.Add(item);

            if (dgv_Models.Rows.Count > 0)
            {
                dgv_Models.ClearSelection();
                dgv_Models.Rows[dgv_Models.Rows.Count - 1].Selected = true;
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"添加模型失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private string GenerateUniqueName(string baseName)
    {
        var usedIndexes = _modelList
            .Where(d => d.Type.Equals(baseName, StringComparison.OrdinalIgnoreCase))
            .Select(d => d.Name)
            .Select(n => n.StartsWith(baseName, StringComparison.OrdinalIgnoreCase) ? n.Substring(baseName.Length) : "")
            .Select(s => int.TryParse(s, out var i) ? i : (int?)null)
            .Where(i => i.HasValue)
            .Select(i => i.Value)
            .ToHashSet();

        int idx = 0;
        while (usedIndexes.Contains(idx)) idx++;
        return $"{baseName}{idx}";
    }

    private void BtnRemove_Click(object sender, EventArgs e)
    {
        if (dgv_Models.SelectedRows.Count == 0) return;
        var row = dgv_Models.SelectedRows[0];
        if (row.DataBoundItem is not DLModelItem item) return;

        var result = MessageBox.Show($"确定要删除模型 \"{item.Name}\" 吗？\n这将卸载模型并释放资源。",
            "确认删除", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

        if (result == DialogResult.Yes)
        {
            DnnModelFactory.Instance.RemoveModel(item.Name);
            _modelList.Remove(item);
            ClearConfigPanel();
        }
    }

    #endregion

    #region 选择和配置

    private void DgvModels_SelectionChanged(object sender, EventArgs e)
    {
        if (dgv_Models.SelectedRows.Count == 0)
        {
            ClearConfigPanel();
            return;
        }

        var row = dgv_Models.SelectedRows[0];
        if (row.DataBoundItem is not DLModelItem item) return;

        _selectedModel = item.Model;
        ShowConfigControl(_selectedModel);
    }

    private void ShowConfigControl(IDnnModel model)
    {
        ClearConfigPanel();

        if (model == null) return;

        try
        {
            var control = model.GetConfigControl();
            if (control != null)
            {
                control.Dock = DockStyle.Fill;
                panel_Config.Controls.Add(control);
                _currentConfigControl = control;
            }
        }
        catch (Exception ex)
        {
            var lbl = new Label
            {
                Text = $"加载配置界面失败: {ex.Message}",
                Dock = DockStyle.Fill,
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter
            };
            panel_Config.Controls.Add(lbl);
        }
    }

    private void ClearConfigPanel()
    {
        if (_currentConfigControl != null)
        {
            panel_Config.Controls.Remove(_currentConfigControl);
            _currentConfigControl.Dispose();
            _currentConfigControl = null;
        }
        panel_Config.Controls.Clear();
        _selectedModel = null;
    }

    #endregion

    #region 名称编辑

    private void DgvModels_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
    {
        if (e.RowIndex < 0 || e.ColumnIndex != col_Name.Index) return;
        var row = dgv_Models.Rows[e.RowIndex];
        if (row.DataBoundItem is DLModelItem item)
        {
            _editingOriginalName = item.Name;
        }
    }

    private void DgvModels_CellEndEdit(object sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex < 0 || e.ColumnIndex != col_Name.Index) return;
        var row = dgv_Models.Rows[e.RowIndex];
        if (row.DataBoundItem is not DLModelItem item) return;

        var newName = row.Cells[e.ColumnIndex].Value?.ToString()?.Trim() ?? string.Empty;
        var oldName = _editingOriginalName ?? item.Name;

        // 验证名称
        if (string.IsNullOrWhiteSpace(newName))
        {
            MessageBox.Show("模型名称不能为空", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            item.Name = oldName;
            dgv_Models.Refresh();
            return;
        }

        if (!string.Equals(newName, oldName, StringComparison.OrdinalIgnoreCase))
        {
            if (!IsNameUnique(newName, item))
            {
                MessageBox.Show($"模型名称 \"{newName}\" 已存在", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                item.Name = oldName;
                dgv_Models.Refresh();
                return;
            }

            // 执行重命名
            if (DnnModelFactory.Instance.RenameModel(oldName, newName))
            {
                item.Name = newName;
            }
            else
            {
                MessageBox.Show("重命名失败", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                item.Name = oldName;
            }
            dgv_Models.Refresh();
        }

        _editingOriginalName = null;
    }

    private bool IsNameUnique(string name, DLModelItem current)
    {
        return !_modelList.Any(d =>
            !ReferenceEquals(d, current) && string.Equals(d.Name, name, StringComparison.OrdinalIgnoreCase));
    }

    #endregion

    #region 保存和关闭

    private void BtnSave_Click(object sender, EventArgs e)
    {
        try
        {
            DnnModelFactory.Instance.SaveConfigs();
            MessageBox.Show("配置保存成功", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"保存配置失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void Frm_DLModelConfig_FormClosing(object sender, FormClosingEventArgs e)
    {
        // 取消订阅事件
        DnnModelFactory.Instance.ModelCreated -= OnModelCreated;
        DnnModelFactory.Instance.ModelRemoved -= OnModelRemoved;
        DnnModelFactory.Instance.ModelRenamed -= OnModelRenamed;

        ClearConfigPanel();
    }

    #endregion

    #region 工厂事件处理

    private void OnModelCreated(string name, string type, IDnnModel model)
    {
        // 如果不是通过本界面创建的，需要同步到列表
        if (!_modelList.Any(m => m.Name == name))
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() =>
                {
                    _modelList.Add(new DLModelItem
                    {
                        Name = name,
                        Type = type,
                        Status = model.IsLoaded ? "已加载" : "未加载",
                        Model = model
                    });
                }));
            }
        }
    }

    private void OnModelRemoved(string name, string type)
    {
        var item = _modelList.FirstOrDefault(m => m.Name == name);
        if (item != null)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => _modelList.Remove(item)));
            }
        }
    }

    private void OnModelRenamed(string oldName, string newName, string type)
    {
        var item = _modelList.FirstOrDefault(m => m.Name == oldName);
        if (item != null)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() =>
                {
                    item.Name = newName;
                    dgv_Models.Refresh();
                }));
            }
        }
    }

    #endregion

    /// <summary>
    /// 刷新模型状态显示
    /// </summary>
    public void RefreshStatus()
    {
        foreach (var item in _modelList)
        {
            item.Status = item.Model?.IsLoaded == true ? "已加载" : "未加载";
        }
        dgv_Models.Refresh();
    }
}

/// <summary>
/// 模型列表项
/// </summary>
public class DLModelItem : INotifyPropertyChanged
{
    private string _name;
    private string _type;
    private string _status;

    public string Name
    {
        get => _name;
        set { _name = value; OnPropertyChanged(nameof(Name)); }
    }

    public string Type
    {
        get => _type;
        set { _type = value; OnPropertyChanged(nameof(Type)); }
    }

    public string Status
    {
        get => _status;
        set { _status = value; OnPropertyChanged(nameof(Status)); }
    }

    public IDnnModel Model { get; set; }

    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
