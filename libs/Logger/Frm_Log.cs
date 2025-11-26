#nullable enable
using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using NLog;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Logger;

public partial class Frm_Log : UserControl
{
    // 当前过滤类型，null表示全部
    private LogLevel? currentFilter;
    //仅初始化列一次
    private bool _columnsInitialized;
    //绑定源，避免频繁重设 DataSource 导致滚动条跳动
    private readonly BindingList<LogEntry> _binding = new();

    // 缓冲队列：接收外部快速追加的日志，定时批量刷新到UI
    private readonly ConcurrentQueue<LogEntry> _pending = new();
    // 内部完整列表：作为 UI 的数据源基底（避免直接枚举外部集合）
    private readonly List<LogEntry> _all = new();
    // UI线程定时器：批量刷新
    private readonly Timer _flushTimer = new Timer();

    //计数器（避免遍历外部集合导致并发异常）
    private int _infoCount;
    private int _warnCount;
    private int _errorCount;

    public Frm_Log()
    {
        InitializeComponent();
        InitUIControls();

        //绑定一次数据源
        dataGridView1.DataSource = _binding;

        LogHelper.LogChanged += UpdateLogView; //仍可用于外部请求刷新，但内部不再遍历外部集合
        LogHelper.LogAppended += OnLogAppended;

        // 启动批量刷新计时器
        _flushTimer.Interval =100; //100ms 刷新一次
        _flushTimer.Tick += FlushPendingToUi;
        _flushTimer.Start();

        // 清理事件，防止控件销毁后仍回调
        this.Disposed += (_, __) =>
        {
            try { LogHelper.LogChanged -= UpdateLogView; } catch { }
            try { LogHelper.LogAppended -= OnLogAppended; } catch { }
            try { _flushTimer.Stop(); } catch { }
            try { _flushTimer.Dispose(); } catch { }
        };

        EnsureColumns();
        UpdateButtonText();
    }

    private void InitUIControls()
    {
        // 外观
        dataGridView1.ReadOnly = true;
        dataGridView1.AllowUserToAddRows = false;
        dataGridView1.AllowUserToDeleteRows = false;
        dataGridView1.AllowUserToResizeRows = false;
        dataGridView1.MultiSelect = false;
        dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dataGridView1.RowHeadersVisible = false;
        dataGridView1.AutoGenerateColumns = false;
        dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells; // 消息自动换行撑开行高

        //颜色主题（黑底）
        dataGridView1.BackgroundColor = Color.Black;
        dataGridView1.GridColor = Color.DimGray;
        dataGridView1.DefaultCellStyle.BackColor = Color.Black;
        dataGridView1.DefaultCellStyle.ForeColor = Color.White;
        dataGridView1.DefaultCellStyle.SelectionBackColor = Color.FromArgb(72,72,72);
        dataGridView1.DefaultCellStyle.SelectionForeColor = Color.White;
        dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.Black;
        dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
        dataGridView1.EnableHeadersVisualStyles = false;

        //事件
        btn_Info.Click += (_, _) => { ToggleFilter(LogLevel.Info); };
        btn_Warning.Click += (_, _) => { ToggleFilter(LogLevel.Warn); };
        btn_Error.Click += (_, _) => { ToggleFilter(LogLevel.Error); };

        // 尺寸变化时，“消息”列自动填充
        dataGridView1.SizeChanged += (_, _) => AdjustMessageColumnWidth();
    }

    /// <summary>
    /// 批量刷新待处理日志到 UI（UI线程执行）
    /// </summary>
    private void FlushPendingToUi(object? sender, EventArgs e)
    {
        // 在过滤模式下：不自动滚动到底部；在非过滤模式：始终保持在底部
        bool allowAutoScroll = currentFilter == null;

        int processed =0;
        const int MaxBatch =200; // 限制单次刷新量，避免 UI 卡顿
        LogEntry entry;
        while (processed < MaxBatch && _pending.TryDequeue(out entry))
        {
            _all.Add(entry);
            // 更新计数器
            if (entry.Level == LogLevel.Info) _infoCount++;
            else if (entry.Level == LogLevel.Warn) _warnCount++;
            else if (entry.Level == LogLevel.Error) _errorCount++;

            // 根据过滤决定是否显示到绑定源
            if (currentFilter == null || entry.Level == currentFilter)
            {
                _binding.Add(entry);
            }

            processed++;
        }

        if (processed >0)
        {
            UpdateButtonText();
            AdjustMessageColumnWidth();
            if (allowAutoScroll)
            {
                ScrollToLast();
            }
        }
    }

    /// <summary>
    /// 刷新界面（例如外部请求重建视图或切换过滤）
    /// </summary>
    private void UpdateLogView()
    {
        if (InvokeRequired)
        {
            BeginInvoke(new MethodInvoker(UpdateLogView));
            return;
        }

        // 非过滤时：始终滚动到底部；过滤时：保留当前位置
        int prevTop = SafeFirstDisplayedRowIndex();
        int prevFocused = dataGridView1.CurrentCell?.RowIndex ?? -1;
        bool allowAutoScroll = currentFilter == null;

        // 使用内部 _all 构建当前视图
        var view = currentFilter == null
            ? _all.ToList()
            : _all.Where(l => l.Level == currentFilter).ToList();

        _binding.RaiseListChangedEvents = false;
        _binding.Clear();
        foreach (var l in view) _binding.Add(l);
        _binding.RaiseListChangedEvents = true;
        _binding.ResetBindings();

        EnsureColumns();
        AdjustMessageColumnWidth();

        if (allowAutoScroll)
        {
            ScrollToLast();
        }
        else
        {
            RestoreScroll(prevTop, prevFocused);
        }

        UpdateButtonText();
    }

    private int SafeFirstDisplayedRowIndex()
    {
        try { return dataGridView1.FirstDisplayedScrollingRowIndex; } catch { return 0; }
    }

    private void RestoreScroll(int prevTop, int prevFocused)
    {
        try
        {
            if (dataGridView1.RowCount ==0) return;
            var top = Math.Max(0, Math.Min(prevTop, dataGridView1.RowCount -1));
            dataGridView1.FirstDisplayedScrollingRowIndex = top;
            if (prevFocused >=0 && prevFocused < dataGridView1.RowCount)
            {
                var col = Math.Max(0, dataGridView1.CurrentCell?.ColumnIndex ??0);
                dataGridView1.CurrentCell = dataGridView1[col, prevFocused];
            }
        }
        catch { }
    }

    private void ScrollToLast()
    {
        try
        {
            var last = dataGridView1.RowCount -1;
            if (last >=0)
            {
                dataGridView1.ClearSelection();
                var col = Math.Max(0, dataGridView1.CurrentCell?.ColumnIndex ??0);
                dataGridView1.CurrentCell = dataGridView1[col, last];
            }
        }
        catch { }
    }

    private bool IsAtBottom()
    {
        if (dataGridView1.RowCount ==0) return true;
        try
        {
            int first = dataGridView1.FirstDisplayedScrollingRowIndex;
            int displayed = dataGridView1.DisplayedRowCount(false);
            return first >=0 && first + displayed >= dataGridView1.RowCount;
        }
        catch { return true; }
    }

    // 首次绑定后生成并规范列
    private void EnsureColumns()
    {
        if (_columnsInitialized) return;

        dataGridView1.Columns.Clear();

        // Time 列
        var colTime = new DataGridViewTextBoxColumn
        {
            DataPropertyName = nameof(LogEntry.Time),
            Name = "Time",
            HeaderText = "时间",
            Width =200,
            ReadOnly = true,
            AutoSizeMode = DataGridViewAutoSizeColumnMode.None
        };
        colTime.DefaultCellStyle.Format = "HH:mm:ss.fff";
        colTime.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        dataGridView1.Columns.Add(colTime);

        // Level 列
        var colLevel = new DataGridViewTextBoxColumn
        {
            DataPropertyName = nameof(LogEntry.Level),
            Name = "Level",
            HeaderText = "级别",
            Width =100,
            ReadOnly = true,
            AutoSizeMode = DataGridViewAutoSizeColumnMode.None
        };
        colLevel.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        dataGridView1.Columns.Add(colLevel);

        // Source 列
        var colSource = new DataGridViewTextBoxColumn
        {
            DataPropertyName = nameof(LogEntry.Source),
            Name = "Source",
            HeaderText = "来源",
            Width =300,
            ReadOnly = true,
            AutoSizeMode = DataGridViewAutoSizeColumnMode.None
        };
        dataGridView1.Columns.Add(colSource);

        // Message 列（自动换行 + 填充剩余宽度）
        var colMsg = new DataGridViewTextBoxColumn
        {
            DataPropertyName = nameof(LogEntry.Message),
            Name = "Message",
            HeaderText = "消息",
            ReadOnly = true,
            AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
        };
        colMsg.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
        dataGridView1.Columns.Add(colMsg);

        // 单元格着色：按行级别设置前景色
        dataGridView1.CellFormatting += DataGridView1_CellFormatting;

        _columnsInitialized = true;
    }

    private void DataGridView1_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
    {
        if (e.RowIndex <0) return;
        var rowObj = dataGridView1.Rows[e.RowIndex].DataBoundItem as LogEntry;
        if (rowObj == null) return;

        e.CellStyle.BackColor = Color.Black;
        var levelName = rowObj.Level?.Name;
        if (levelName == "Info") e.CellStyle.ForeColor = Color.Lime;
        else if (levelName == "Warn") e.CellStyle.ForeColor = Color.Orange;
        else if (levelName == "Error") e.CellStyle.ForeColor = Color.Red;
        else e.CellStyle.ForeColor = Color.White;
    }

    //让“消息”列填充剩余宽度（使用 Fill 已满足，这里保证最小宽度）
    private void AdjustMessageColumnWidth()
    {
        var msgCol = dataGridView1.Columns["Message"];
        if (msgCol == null) return;
        //设定最小宽度，防止过小
        msgCol.MinimumWidth =200;
    }

    /// <summary>
    /// 按钮文本显示数量（使用内部计数器，避免遍历外部集合）
    /// </summary>
    private void UpdateButtonText()
    {
        btn_Info.Text = $"信息({_infoCount})";
        btn_Warning.Text = $"警告({_warnCount})";
        btn_Error.Text = $"错误({_errorCount})";

        // 重置所有按钮颜色
        btn_Info.BackColor = Color.FromArgb(64,64,64);
        btn_Warning.BackColor = Color.FromArgb(64,64,64);
        btn_Error.BackColor = Color.FromArgb(64,64,64);

        // 根据当前过滤类型设置按钮颜色
        if (currentFilter == LogLevel.Info)
            btn_Info.BackColor = Color.Lime;
        else if (currentFilter == LogLevel.Warn)
            btn_Warning.BackColor = Color.Lime;
        else if (currentFilter == LogLevel.Error)
            btn_Error.BackColor = Color.Lime;
    }

    /// <summary>
    /// 切换过滤类型，支持再次点击显示全部
    /// </summary>
    private void ToggleFilter(LogLevel level)
    {
        currentFilter = currentFilter == level ? null : level;
        UpdateLogView();
    }

    private void OnLogAppended(LogEntry entry)
    {
        //任何线程均可调用：仅入队，UI 刷新交由计时器批量执行
        _pending.Enqueue(entry);
    }
}
