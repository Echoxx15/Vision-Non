using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using Cognex.VisionPro;
using Logger;

namespace UserControls.Display;

[Description("视图窗体")]
public partial class ImageDisplay : UserControl
{
    private readonly List<ICogRecord> _recordList = [];
    private int _currentIndex = 0;

    public int TotalCount => _recordList.Count;
    public int CurrentDisplayIndex => _recordList.Count > 0 ? _currentIndex + 1 : 0;

    public ICogImage CogImage
    {
        get => cogRecordDisplay?.Image;
        set
        {
            if (InvokeRequired) { BeginInvoke(new Action(() => CogImage = value)); return; }
            
            try
            {
                if (cogRecordDisplay is { IsDisposed: false })
                {
                    cogRecordDisplay.StaticGraphics?.Clear();
                    cogRecordDisplay.InteractiveGraphics?.Clear();
                    cogRecordDisplay.Image = value;
                    cogRecordDisplay.Fit(true);
                }
            }
            catch (AccessViolationException ex)
            {
                LogHelper.Warn($"[ImageDisplay] CogImage setter 访问冲突: {ex.Message}");
            }
            catch (Exception ex)
            {
                LogHelper.Warn($"[ImageDisplay] CogImage setter 异常: {ex.Message}");
            }
        }
    }

    public ICogRecord Record
    {
        get => cogRecordDisplay?.Record;
        set
        {
            if (InvokeRequired) { BeginInvoke(new Action(() => Record = value)); return; }
            
            try
            {
                if (cogRecordDisplay is { IsDisposed: false })
                {
                    cogRecordDisplay.StaticGraphics?.Clear();
                    cogRecordDisplay.InteractiveGraphics?.Clear();
                    cogRecordDisplay.Record = value;
                    cogRecordDisplay.Fit(true);
                }
            }
            catch (AccessViolationException ex)
            {
                LogHelper.Warn($"[ImageDisplay] Record setter 访问冲突: {ex.Message}");
            }
            catch (Exception ex)
            {
                LogHelper.Warn($"[ImageDisplay] Record setter 异常: {ex.Message}");
            }
        }
    }
    
    public CogRecordDisplay RecordDisplay => cogRecordDisplay;

    public string DisplayName
    {
        get => grb_ShowName.Text;
        set
        {
            if (InvokeRequired) { BeginInvoke(new Action(() => DisplayName = value)); return; }
            grb_ShowName.Text = value;
        }
    }

    public IReadOnlyList<ICogRecord> RecordList => _recordList;

    public ImageDisplay()
    {
        InitializeComponent();
    }

    public ImageDisplay(string name)
    {
        InitializeComponent();
        grb_ShowName.Text = name;
    }

    private void UpdateIndexLabel()
    {
        if (InvokeRequired)
        {
            BeginInvoke(new Action(UpdateIndexLabel));
            return;
        }

        if (lblIndex != null)
        {
            lblIndex.Text = _recordList.Count > 0
                ? $"{_currentIndex + 1}/{_recordList.Count}"
                : "0/0";
        }
    }

    private void btnPrev_Click(object sender, EventArgs e)
    {
        ShowPrevious();
    }

    private void btnNext_Click(object sender, EventArgs e)
    {
        ShowNext();
    }

    private void ShowPrevious()
    {
        if (_recordList.Count == 0) return;
        _currentIndex = (_currentIndex - 1 + _recordList.Count) % _recordList.Count;
        ShowCurrentRecord();
    }

    private void ShowNext()
    {
        if (_recordList.Count == 0) return;
        _currentIndex = (_currentIndex + 1) % _recordList.Count;
        ShowCurrentRecord();
    }

    public void ShowAt(int index)
    {
        if (_recordList.Count == 0 || index < 0 || index >= _recordList.Count) return;
        _currentIndex = index;
        ShowCurrentRecord();
    }

    private void ShowCurrentRecord()
    {
        if (InvokeRequired)
        {
            BeginInvoke(new Action(ShowCurrentRecord));
            return;
        }

        try
        {
            if (cogRecordDisplay is { IsDisposed: false } && _recordList.Count > 0 && _currentIndex >= 0 && _currentIndex < _recordList.Count)
            {
                cogRecordDisplay.StaticGraphics?.Clear();
                cogRecordDisplay.InteractiveGraphics?.Clear();
                cogRecordDisplay.Record = _recordList[_currentIndex];
                cogRecordDisplay.Fit(true);
            }
        }
        catch (AccessViolationException ex)
        {
            LogHelper.Warn($"[ImageDisplay] ShowCurrentRecord() 访问冲突: {ex.Message}");
        }
        catch (Exception ex)
        {
            LogHelper.Warn($"[ImageDisplay] ShowCurrentRecord() 异常: {ex.Message}");
        }
        UpdateIndexLabel();
    }

    public void AddRecord(ICogRecord record)
    {
        if (InvokeRequired)
        {
            BeginInvoke(new Action(() => AddRecord(record)));
            return;
        }

        if (record == null) return;
        _recordList.Add(record);
        _currentIndex = _recordList.Count - 1;
        ShowCurrentRecord();
    }

    public void Clear()
    {
        if (InvokeRequired)
        {
            BeginInvoke(new Action(Clear));
            return;
        }

        _recordList.Clear();
        _currentIndex = 0;
        
        // 防止屏保或控件销毁时的 AccessViolationException
        try
        {
            if (cogRecordDisplay is { IsDisposed: false })
            {
                cogRecordDisplay.StaticGraphics?.Clear();
                cogRecordDisplay.InteractiveGraphics?.Clear();
                cogRecordDisplay.Record = null;
                cogRecordDisplay.Image = null;
            }
        }
        catch (AccessViolationException ex)
        {
            LogHelper.Warn($"[ImageDisplay] Clear() 访问冲突: {ex.Message}");
        }
        catch (Exception ex)
        {
            LogHelper.Warn($"[ImageDisplay] Clear() 异常: {ex.Message}");
        }
        
        UpdateIndexLabel();
    }

    private void cogRecordDisplay_DoubleClick(object sender, EventArgs e)
    {
        var recordDisplay = sender as CogRecordDisplay;
        var maxDisplay = new MaxDisplay(recordDisplay, DisplayName, _recordList, _currentIndex);
        maxDisplay.ShowDialog();
        _currentIndex = maxDisplay.CurrentIndex;
        ShowCurrentRecord();
    }

    private void ImageDisplay_Load(object sender, EventArgs e)
    {
        Dock = DockStyle.Fill;
        UpdateIndexLabel();
    }
}
