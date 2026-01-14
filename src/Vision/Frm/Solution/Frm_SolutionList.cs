using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Vision.Localization;
using Vision.Solutions.Models;

namespace Vision.Frm.Solution;

public partial class Frm_SolutionList : Form
{
    public event Action<Solutions.Models.Solution> OpenSolutionRequested;

    private readonly Regex _nameValid = new("^[A-Za-z0-9_\u4e00-\u9fa5]+$", RegexOptions.Compiled);
    private string _originalNameEditing;
    private readonly BindingSource _bs = new();
    private bool _columnsBuilt;
    private bool _adjustingDesc; // 防抖，避免事件递归

    public Frm_SolutionList()
    {
        InitializeComponent();
        InitGrid();
        Load += (_, _) => BindData();
        
        // 多语言支持
        UITranslationService.Instance.LanguageChanged += (_, _) => ApplyLanguage();
        ApplyLanguage();
    }

    /// <summary>
    /// 应用多语言翻译
    /// </summary>
    private void ApplyLanguage()
    {
        this.Text = this.T("Title");
        btn_Open.Text = this.T("btn_Open");
        btn_AddNew.Text = this.T("btn_AddNew");
        btn_AddCur.Text = this.T("btn_AddCur");
        btn_SetStart.Text = this.T("btn_SetStart");
        btn_Delete.Text = this.T("btn_Delete");
        
        // 更新列标题
        UpdateColumnHeaders();
    }

    /// <summary>
    /// 更新DataGridView列标题
    /// </summary>
    private void UpdateColumnHeaders()
    {
        if (dgvSolutions.Columns.Count == 0) return;
        
        var colName = dgvSolutions.Columns[nameof(SolutionInfo.Name)];
        var colDesc = dgvSolutions.Columns[nameof(SolutionInfo.Description)];
        var colEnable = dgvSolutions.Columns[nameof(SolutionInfo.Enable)];
        var colCreate = dgvSolutions.Columns[nameof(SolutionInfo.CreateTime)];
        var colModify = dgvSolutions.Columns[nameof(SolutionInfo.LastModifyTime)];
        
        if (colName != null) colName.HeaderText = this.T("col_Name");
        if (colDesc != null) colDesc.HeaderText = this.T("col_Description");
        if (colEnable != null) colEnable.HeaderText = this.T("col_Enable");
        if (colCreate != null) colCreate.HeaderText = this.T("col_CreateTime");
        if (colModify != null) colModify.HeaderText = this.T("col_ModifyTime");
    }

    private void BindData()
    {
        _bs.DataSource = SolutionManager.Instance.Solutions;
        dgvSolutions.AutoGenerateColumns = false;
        dgvSolutions.DataSource = _bs;
        BuildColumnsOnce();
        if (_bs.Count > 0)
        {
            dgvSolutions.ClearSelection();
            dgvSolutions.Rows[0].Selected = true;
        }
        AdjustDescriptionColumnWidth();
    }

    private void BuildColumnsOnce()
    {
        if (_columnsBuilt) return;
        _columnsBuilt = true;
        dgvSolutions.Columns.Clear();

        // 允许用户拖拽调整列宽
        dgvSolutions.AllowUserToResizeColumns = true;
        dgvSolutions.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;

        var colName = new DataGridViewTextBoxColumn
        {
            DataPropertyName = nameof(SolutionInfo.Name),
            Name = nameof(SolutionInfo.Name),
            HeaderText = this.T("col_Name"),
            AutoSizeMode = DataGridViewAutoSizeColumnMode.NotSet,
            ReadOnly = false,
            SortMode = DataGridViewColumnSortMode.NotSortable,
            MinimumWidth = 120,
            Width = 120,
            Resizable = DataGridViewTriState.True
        };
        var colDesc = new DataGridViewTextBoxColumn
        {
            DataPropertyName = nameof(SolutionInfo.Description),
            Name = nameof(SolutionInfo.Description),
            HeaderText = this.T("col_Description"),
            AutoSizeMode = DataGridViewAutoSizeColumnMode.NotSet,
            ReadOnly = false,
            SortMode = DataGridViewColumnSortMode.NotSortable,
            MinimumWidth = 200,
            Width = 200,
            Resizable = DataGridViewTriState.True
        };
        var colEnable = new DataGridViewCheckBoxColumn
        {
            DataPropertyName = nameof(SolutionInfo.Enable),
            Name = nameof(SolutionInfo.Enable),
            HeaderText = this.T("col_Enable"),
            AutoSizeMode = DataGridViewAutoSizeColumnMode.NotSet,
            ReadOnly = true,
            SortMode = DataGridViewColumnSortMode.NotSortable,
            MinimumWidth = 60,
            Width = 60,
            Resizable = DataGridViewTriState.True
        };
        var colCreate = new DataGridViewTextBoxColumn
        {
            DataPropertyName = nameof(SolutionInfo.CreateTime),
            Name = nameof(SolutionInfo.CreateTime),
            HeaderText = this.T("col_CreateTime"),
            AutoSizeMode = DataGridViewAutoSizeColumnMode.NotSet,
            ReadOnly = true,
            SortMode = DataGridViewColumnSortMode.NotSortable,
            MinimumWidth = 120,
            Width = 140,
            Resizable = DataGridViewTriState.True
        };
        var colModify = new DataGridViewTextBoxColumn
        {
            DataPropertyName = nameof(SolutionInfo.LastModifyTime),
            Name = nameof(SolutionInfo.LastModifyTime),
            HeaderText = this.T("col_ModifyTime"),
            AutoSizeMode = DataGridViewAutoSizeColumnMode.NotSet,
            ReadOnly = true,
            SortMode = DataGridViewColumnSortMode.NotSortable,
            MinimumWidth = 120,
            Width = 140,
            Resizable = DataGridViewTriState.True
        };

        // 顺序：名称、默认、创建、修改、描述（描述最后用于填充剩余宽度）
        dgvSolutions.Columns.AddRange(colName, colEnable, colCreate, colModify, colDesc);

        // 监听尺寸/列宽变化以调整描述列填充
        dgvSolutions.SizeChanged += (_, _) => AdjustDescriptionColumnWidth();
        dgvSolutions.ColumnWidthChanged += (_, e) =>
        {
            if (e?.Column != null && !string.Equals(e.Column.DataPropertyName, nameof(SolutionInfo.Description), StringComparison.Ordinal))
                AdjustDescriptionColumnWidth();
        };
        dgvSolutions.DataBindingComplete += (_, _) => AdjustDescriptionColumnWidth();
    }

    // 将剩余宽度分配给“描述”列
    private void AdjustDescriptionColumnWidth()
    {
        if (_adjustingDesc) return;
        if (dgvSolutions.Columns.Count == 0) return;
        var descCol = dgvSolutions.Columns[nameof(SolutionInfo.Description)];
        if (descCol == null) return;
        try
        {
            _adjustingDesc = true;
            // 可用宽度 = 客户端宽度 - 行头 - 垂直滚动条（若可见）
            int available = dgvSolutions.ClientSize.Width - (dgvSolutions.RowHeadersVisible ? dgvSolutions.RowHeadersWidth : 0);
            var vs = dgvSolutions.Controls.OfType<VScrollBar>().FirstOrDefault();
            if (vs != null && vs.Visible) available -= vs.Width;
            // 其它列占用宽度
            int others = 0;
            foreach (DataGridViewColumn c in dgvSolutions.Columns)
            {
                if (c == descCol) continue;
                if (!c.Visible) continue;
                others += c.Width;
            }
            int target = Math.Max(descCol.MinimumWidth, available - others);
            if (target < descCol.MinimumWidth) target = descCol.MinimumWidth;
            if (target != descCol.Width)
            {
                descCol.Width = target;
            }
        }
        catch { }
        finally { _adjustingDesc = false; }
    }

    private void InitGrid()
    {
        dgvSolutions.AllowUserToAddRows = false;
        dgvSolutions.AllowUserToDeleteRows = false;
        dgvSolutions.RowHeadersVisible = false;
        dgvSolutions.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgvSolutions.MultiSelect = false;
        dgvSolutions.EditMode = DataGridViewEditMode.EditProgrammatically; // 单击不进入编辑，双击或F2进入
        dgvSolutions.AllowUserToResizeColumns = true; // 允许拖拽改变列宽
        dgvSolutions.DataError += (_, e) => { e.ThrowException = false; };

        // 双击进入编辑
        dgvSolutions.CellDoubleClick += dgvSolutions_CellDoubleClick;
        // 记录编辑前原值
        dgvSolutions.CellBeginEdit += dgvSolutions_CellBeginEdit;
        // 结束编辑时校验与重命名
        dgvSolutions.CellEndEdit += dgvSolutions_CellEndEdit;
    }

    private void dgvSolutions_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex < 0 || e.ColumnIndex < 0) return;
        if (!dgvSolutions.Rows[e.RowIndex].Cells[e.ColumnIndex].ReadOnly)
        {
            dgvSolutions.BeginEdit(true);
        }
    }

    private void dgvSolutions_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
    {
        if (e.RowIndex < 0 || e.ColumnIndex < 0) return;
        if (dgvSolutions.Columns[e.ColumnIndex].DataPropertyName == nameof(SolutionInfo.Name))
        {
            _originalNameEditing = dgvSolutions.Rows[e.RowIndex].Cells[e.ColumnIndex].Value?.ToString();
        }
    }

    private void dgvSolutions_CellEndEdit(object sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex < 0 || e.ColumnIndex < 0) return;
        var col = dgvSolutions.Columns[e.ColumnIndex];
        if (col.DataPropertyName != nameof(SolutionInfo.Name)) return;

        var rowInfo = dgvSolutions.Rows[e.RowIndex].DataBoundItem as SolutionInfo;
        if (rowInfo == null) return;
        var newName = (rowInfo.Name ?? string.Empty).Trim();

        // 未修改直接返回
        if (string.Equals(newName, _originalNameEditing, StringComparison.OrdinalIgnoreCase)) return;

        // 校验
        if (!IsNameValid(newName, out var reason))
        {
            MessageBox.Show(reason, "名称无效", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            rowInfo.Name = _originalNameEditing;
            dgvSolutions.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = _originalNameEditing;
            _bs.ResetCurrentItem();
            return;
        }
        if (SolutionManager.Instance.Solutions.Any(s => !ReferenceEquals(s, rowInfo) && s.Name.Equals(newName, StringComparison.OrdinalIgnoreCase)))
        {
            MessageBox.Show("方案名称已存在，请更换名称。", "名称冲突", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            rowInfo.Name = _originalNameEditing;
            dgvSolutions.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = _originalNameEditing;
            _bs.ResetCurrentItem();
            return;
        }

        try
        {
            var oldFolder = Directory.Exists(rowInfo.Path) ? rowInfo.Path : Path.GetDirectoryName(rowInfo.Path);
            var solutionsDir = SolutionManager.Instance.SolutionsDir;
            var safeNewName = SanitizeFileName(newName);
            var baseNewFolder = Path.Combine(solutionsDir, safeNewName);
            var newFolder = baseNewFolder;
            int suffix = 1;
            while (!string.Equals(oldFolder, newFolder, StringComparison.OrdinalIgnoreCase) && Directory.Exists(newFolder))
                newFolder = baseNewFolder + $"_{suffix++}";

            // 1. 重命名 .uv 文件
            if (!string.IsNullOrEmpty(oldFolder) && Directory.Exists(oldFolder))
            {
                var uvFiles = Directory.GetFiles(oldFolder, "*.uv");
                foreach (var uvFile in uvFiles)
                {
                    var newUvPath = Path.Combine(oldFolder, safeNewName + ".uv");
                    if (!string.Equals(uvFile, newUvPath, StringComparison.OrdinalIgnoreCase))
                    {
                        if (File.Exists(newUvPath)) File.Delete(newUvPath);
                        File.Move(uvFile, newUvPath);
                    }
                }
            }

            // 2. 重命名文件夹
            if (!string.Equals(oldFolder, newFolder, StringComparison.OrdinalIgnoreCase))
            {
                Directory.Move(oldFolder, newFolder);
                rowInfo.Path = newFolder;
            }

            rowInfo.Name = newName;
            rowInfo.LastModifyTime = DateTime.Now;
            SolutionManager.Instance.SaveList();
            _bs.ResetCurrentItem();
            
            // 3. 如果重命名的是当前方案，同步更新 Solution.Name
            var current = SolutionManager.Instance.Current;
            if (current != null)
            {
                var currentUv = current.FilePath;
                var renamedUv = SolutionManager.GetUvPath(rowInfo);
                if (!string.IsNullOrEmpty(currentUv) && !string.IsNullOrEmpty(renamedUv) &&
                    string.Equals(Path.GetFullPath(currentUv), Path.GetFullPath(renamedUv), StringComparison.OrdinalIgnoreCase))
                {
                    current.Name = newName;
                    // 更新 FilePath 指向新路径
                    current.FilePath = renamedUv;
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show("重命名失败: " + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            rowInfo.Name = _originalNameEditing;
            dgvSolutions.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = _originalNameEditing;
            _bs.ResetCurrentItem();
        }
    }

    #region 按钮

    private void btn_Open_Click(object sender, EventArgs e)
    {
        var info = GetFocusedInfo();
        if (info == null) return;
        try
        {
            // 若已是当前方案则不切换
            var uv = SolutionManager.GetUvPath(info);
            if (SolutionManager.Instance.Current != null &&
                string.Equals(SolutionManager.Instance.Current.FilePath, uv, StringComparison.OrdinalIgnoreCase))
            {
                MessageBox.Show($"已在当前方案：{info.Name}");
                return;
            }
            var sol = SolutionManager.Instance.OpenSolution(info);
            OpenSolutionRequested?.Invoke(sol);
            MessageBox.Show($"打开方案成功：{info.Name}");
        }
        catch (Exception ex)
        {
            MessageBox.Show("打开方案失败: " + ex.Message);
        }
    }

    private void btn_AddNew_Click(object sender, EventArgs e)
    {
        try
        {
            var name = MakeUniqueBase("新方案");
            var info = SolutionManager.Instance.NewSolution(name, string.Empty, false);
            _bs.ResetBindings(false);
            FocusRowByName(info.Name);
        }
        catch (Exception ex)
        {
            MessageBox.Show("添加空白方案失败: " + ex.Message);
        }
    }

    private void btn_AddCur_Click(object sender, EventArgs e)
    {
        try
        {
            var srcInfo = GetFocusedInfo();
            if (srcInfo == null)
            {
                MessageBox.Show("请先选中要复制的方案");
                return;
            }
            var newName = MakeUniqueBase(srcInfo.Name + "_Copy");
            var folder = Path.Combine(SolutionManager.Instance.SolutionsDir, SanitizeFileName(newName));
            Directory.CreateDirectory(folder);
            var newFile = Path.Combine(folder, SanitizeFileName(newName) + ".uv");

            // 深拷贝：读取源方案对象，修改名称，另存为新文件
            var srcUv = SolutionManager.GetUvPath(srcInfo);
            var src = SolutionManager.Instance.LoadForClone(srcUv);
            src.Name = newName;
            SolutionManager.Instance.Save(src, newFile);

            // 同时复制工位文件夹结构（不强制，如需可加）
            try
            {
                var srcFolder = Directory.Exists(srcInfo.Path) ? srcInfo.Path : Path.GetDirectoryName(srcInfo.Path);
                foreach (var dir in Directory.GetDirectories(srcFolder))
                {
                    var target = Path.Combine(folder, Path.GetFileName(dir));
                    CopyDirectory(dir, target);
                }
            }
            catch { }

            var fi = new FileInfo(newFile);
            var info = new SolutionInfo
            {
                Name = newName,
                Description = srcInfo.Description,
                Enable = false,
                CreateTime = fi.CreationTime,
                LastModifyTime = fi.LastWriteTime,
                Path = folder
            };
            SolutionManager.Instance.Solutions.Add(info);
            SolutionManager.Instance.SaveList();
            _bs.ResetBindings(false);
            FocusRowByName(newName);
        }
        catch (Exception ex)
        {
            MessageBox.Show("复制选中方案失败: " + ex.Message);
        }
    }

    private void btn_Delete_Click(object sender, EventArgs e)
    {
        var info = GetFocusedInfo();
        if (info == null) return;
        var activeName = SolutionManager.Instance.Current?.Name;
        if (!string.IsNullOrEmpty(activeName) && info.Name.Equals(activeName, StringComparison.OrdinalIgnoreCase))
        {
            MessageBox.Show("当前激活方案不能删除，请先切换到其它方案。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }
        if (SolutionManager.Instance.Solutions.Count <= 1)
        {
            MessageBox.Show("至少需要保留一个方案，无法删除。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }
        if (MessageBox.Show($"确定删除方案：{info.Name}?", "确认", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;
        try
        {
            var folder = Directory.Exists(info.Path) ? info.Path : Path.GetDirectoryName(info.Path);
            try { if (!string.IsNullOrEmpty(folder) && Directory.Exists(folder)) Directory.Delete(folder, true); } catch { }
            SolutionManager.Instance.Solutions.Remove(info);
            SolutionManager.Instance.SaveList();
            _bs.ResetBindings(false);
        }
        catch (Exception ex)
        {
            MessageBox.Show("删除方案失败: " + ex.Message);
        }
    }

    private void btn_SetStart_Click(object sender, EventArgs e)
    {
        var info = GetFocusedInfo();
        if (info == null)
        {
            MessageBox.Show("请先选中要设为默认启动的方案。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }
        if (info.Enable)
        {
            MessageBox.Show("该方案已经是默认启动。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }
        if (MessageBox.Show($"设定 {info.Name} 为默认启动方案?", "确认", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            return;
        foreach (var s in SolutionManager.Instance.Solutions) s.Enable = ReferenceEquals(s, info);
        SolutionManager.Instance.SaveList();
        _bs.ResetBindings(false);
        FocusRowByName(info.Name);
    }
    #endregion

    #region Helper
    private SolutionInfo GetFocusedInfo()
    {
        if (dgvSolutions.CurrentRow == null) return null;
        return dgvSolutions.CurrentRow.DataBoundItem as SolutionInfo;
    }
    private string MakeUniqueBase(string baseName)
    {
        int i = 0; string name;
        do { name = i == 0 ? baseName : baseName + i; i++; }
        while (SolutionManager.Instance.Solutions.Any(s => s.Name.Equals(name, StringComparison.OrdinalIgnoreCase)));
        return name;
    }
    private string SanitizeFileName(string name)
    {
        var invalid = Path.GetInvalidFileNameChars();
        foreach (var c in invalid) name = name.Replace(c, '_');
        return name;
    }
    private bool IsNameValid(string name, out string reason)
    {
        if (string.IsNullOrWhiteSpace(name)) { reason = "名称不能为空"; return false; }
        if (!_nameValid.IsMatch(name)) { reason = "只能包含中文、字母、数字、下划线"; return false; }
        // 继续在 CellEndEdit 中做重名校验
        reason = null; return true;
    }
    private void FocusRowByName(string name)
    {
        if (dgvSolutions.Rows.Count == 0 || dgvSolutions.Columns.Count == 0) return;
        foreach (DataGridViewRow row in dgvSolutions.Rows)
        {
            if (row.DataBoundItem is SolutionInfo info && info.Name == name)
            {
                row.Selected = true;
                dgvSolutions.CurrentCell = row.Cells[0];
                break;
            }
        }
    }

    private static void CopyDirectory(string src, string dest)
    {
        Directory.CreateDirectory(dest);
        foreach (var file in Directory.GetFiles(src))
        {
            var name = Path.GetFileName(file);
            File.Copy(file, Path.Combine(dest, name), true);
        }
        foreach (var dir in Directory.GetDirectories(src))
        {
            var name = Path.GetFileName(dir);
            CopyDirectory(dir, Path.Combine(dest, name));
        }
    }
    #endregion
}
