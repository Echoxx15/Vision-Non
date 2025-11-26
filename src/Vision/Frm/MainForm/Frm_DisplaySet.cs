using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using Vision.Solutions.Models;

namespace Vision.Frm.MainForm;

public partial class Frm_DisplaySet : Form
{
    public event Action<DisplayConfig> Saved; // 通知外部刷新

    public Frm_DisplaySet()
    {
        InitializeComponent();
        InitUi();
    }

    private void InitUi()
    {
        Text = "显示窗口设置";
        // 绑定控件事件
        btnSaveDC.Click += (_, _) => SaveAndClose();
        cmbDisplayRow.SelectedIndexChanged += (_, _) => RebuildGrid();
        cmbDisplayCol.SelectedIndexChanged += (_, _) => RebuildGrid();

        // 初始化下拉
        if (cmbDisplayRow.Items.Count == 0)
            cmbDisplayRow.Items.AddRange(["1","2","3","4","5","6"]);
        if (cmbDisplayCol.Items.Count == 0)
            cmbDisplayCol.Items.AddRange(["1","2","3","4","5","6","7","8","9","10"]);

        // 初始化表格
        dgv_Names.AllowUserToAddRows = false;
        dgv_Names.AllowUserToOrderColumns = false;
        dgv_Names.RowHeadersVisible = false;
        dgv_Names.AutoGenerateColumns = false;
        dgv_Names.Columns.Clear();
        var colKey = new DataGridViewTextBoxColumn { Name = "Key", HeaderText = "Key", ReadOnly = true, DataPropertyName = "Key", Width = 120 };
        var colName = new DataGridViewTextBoxColumn { Name = "DisplayName", HeaderText = "显示名", DataPropertyName = "DisplayName", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill };
        dgv_Names.Columns.Add(colKey);
        dgv_Names.Columns.Add(colName);

        // 加载当前方案
        var disp = SolutionManager.Instance.Current?.Display ?? new DisplayConfig();
        cmbDisplayRow.Text = Math.Max(1, disp.Rows).ToString();
        cmbDisplayCol.Text = Math.Max(1, disp.Cols).ToString();
        chkDisplay.Checked = true; // 是否显示，保留开关但不持久
        LoadGridFromConfig(disp);
    }

    private void LoadGridFromConfig(DisplayConfig cfg)
    {
        var rows = Math.Max(1, cfg.Rows);
        var cols = Math.Max(1, cfg.Cols);
        var total = rows * cols;
        var list = new BindingList<DisplayItem>();
        // 确保唯一 Key：显示1..N，若已有则复用
        var used = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        int next = 1;
        for (int i = 0; i < total; i++)
        {
            DisplayItem item = null;
            if (i < (cfg.Items?.Count ?? 0)) item = cfg.Items[i];
            if (item == null) item = new DisplayItem();
            // 生成 Key
            if (string.IsNullOrWhiteSpace(item.Key) || used.Contains(item.Key))
            {
                string key;
                do { key = $"显示{next++}"; } while (used.Contains(key));
                item.Key = key;
            }
            used.Add(item.Key);
            if (string.IsNullOrWhiteSpace(item.DisplayName)) item.DisplayName = item.Key;
            list.Add(new DisplayItem { Key = item.Key, DisplayName = item.DisplayName });
        }
        dgv_Names.DataSource = list;
    }

    private void RebuildGrid()
    {
        if (!int.TryParse(cmbDisplayRow.Text, out var rows)) rows = 1;
        if (!int.TryParse(cmbDisplayCol.Text, out var cols)) cols = 1;
        var current = dgv_Names.DataSource as BindingList<DisplayItem>;
        var cfg = new DisplayConfig { Rows = rows, Cols = cols, Items = current?.ToList() ?? new List<DisplayItem>() };
        LoadGridFromConfig(cfg);
    }

    private void SaveAndClose()
    {
        // 关键：保存前强制提交 DataGridView 当前单元格的编辑，否则最后一次编辑不会写回数据源
        try
        {
            if (dgv_Names.IsCurrentCellInEditMode)
            {
                dgv_Names.CommitEdit(DataGridViewDataErrorContexts.Commit);
                dgv_Names.EndEdit();
            }
            var cm = BindingContext[dgv_Names.DataSource] as CurrencyManager;
            cm?.EndCurrentEdit();
        }
        catch { }

        if (!int.TryParse(cmbDisplayRow.Text, out var rows) || rows <= 0) rows = 1;
        if (!int.TryParse(cmbDisplayCol.Text, out var cols) || cols <= 0) cols = 1;
        var list = (dgv_Names.DataSource as BindingList<DisplayItem>)?.ToList() ?? new List<DisplayItem>();
        // 重新保证 Key 唯一且连续从显示1开始
        var result = new List<DisplayItem>();
        int idx = 1;
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var it in list)
        {
            var key = $"显示{idx++}";
            while (seen.Contains(key)) key = $"显示{idx++}";
            seen.Add(key);
            result.Add(new DisplayItem { Key = key, DisplayName = string.IsNullOrWhiteSpace(it.DisplayName) ? key : it.DisplayName });
        }
        var cfgToSave = new DisplayConfig { Rows = rows, Cols = cols, Items = result };
        var sol = SolutionManager.Instance.Current;
        if (sol != null) { sol.Display = cfgToSave; SolutionManager.Instance.SaveCurrent(); }
        Saved?.Invoke(cfgToSave);
        DialogResult = DialogResult.OK;
        Close();
    }
}