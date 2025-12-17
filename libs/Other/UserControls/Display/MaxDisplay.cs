using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Cognex.VisionPro;

namespace UserControls.Display;

public partial class MaxDisplay : Form
{
    private readonly CogRecordDisplay mRecordDisplay;
    private readonly IReadOnlyList<ICogRecord> _recordList;
    private int _currentIndex;

    public int CurrentIndex => _currentIndex;

    public MaxDisplay(CogRecordDisplay recordDisplay, string name)
        : this(recordDisplay, name, null, 0) { }

    public MaxDisplay(CogRecordDisplay recordDisplay, string name, IReadOnlyList<ICogRecord> recordList, int currentIndex)
    {
        InitializeComponent();
        mRecordDisplay = recordDisplay;
        _recordList = recordList ?? new List<ICogRecord>();
        _currentIndex = currentIndex;
        MinimizeBox = false;
        Text = name;
        InitializeNavigationControls();
    }

    private void InitializeNavigationControls()
    {
        panelNav.BringToFront();
        KeyPreview = true;
    }

    private string GetIndexText()
    {
        if (_recordList == null || _recordList.Count == 0) return "0/0";
        return $"{_currentIndex + 1}/{_recordList.Count}";
    }

    private void MaxDisplay_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Left) { ShowPrevious(); e.Handled = true; }
        else if (e.KeyCode == Keys.Right) { ShowNext(); e.Handled = true; }
        else if (e.KeyCode == Keys.Escape) { Close(); e.Handled = true; }
    }

    private void ShowPrevious()
    {
        if (_recordList == null || _recordList.Count <= 1) return;
        _currentIndex = (_currentIndex - 1 + _recordList.Count) % _recordList.Count;
        ShowCurrentRecord();
    }

    private void ShowNext()
    {
        if (_recordList == null || _recordList.Count <= 1) return;
        _currentIndex = (_currentIndex + 1) % _recordList.Count;
        ShowCurrentRecord();
    }

    private void ShowCurrentRecord()
    {
        if (_recordList != null && _currentIndex >= 0 && _currentIndex < _recordList.Count)
        {
            cogRecordDisplay1.StaticGraphics.Clear();
            cogRecordDisplay1.InteractiveGraphics.Clear();
            cogRecordDisplay1.Record = _recordList[_currentIndex];
            cogRecordDisplay1.Fit();
        }
        UpdateIndexLabel();
    }

    private void UpdateIndexLabel()
    {
        var arr = Controls.Find("lblIndexMax", true);
        if (arr.Length > 0 && arr[0] is Label lbl)
        {
            lbl.Text = GetIndexText();
            if (panelNav != null)
                lbl.Location = new Point((panelNav.Width - lbl.Width) / 2, 8);
        }
    }

    public sealed override string Text
    {
        get => base.Text;
        set => base.Text = value;
    }

    private void MaxDisplay_FormClosing(object sender, FormClosingEventArgs e) { }

    private void MaxDisplay_Shown(object sender, EventArgs e)
    {
        if (_recordList != null && _recordList.Count > 0 && _currentIndex >= 0 && _currentIndex < _recordList.Count)
        {
            cogRecordDisplay1.StaticGraphics.Clear();
            cogRecordDisplay1.InteractiveGraphics.Clear();
            cogRecordDisplay1.Record = _recordList[_currentIndex];
            cogRecordDisplay1.Fit();
        }
        else if (mRecordDisplay?.Record != null)
        {
            cogRecordDisplay1.StaticGraphics.Clear();
            cogRecordDisplay1.InteractiveGraphics.Clear();
            cogRecordDisplay1.Record = mRecordDisplay.Record;
            cogRecordDisplay1.Fit();
        }
        UpdateIndexLabel();
    }

    private void btnPrev_Click(object sender, EventArgs e)
    {
        ShowPrevious();
    }

    private void btnNext_Click(object sender, EventArgs e)
    {
        ShowNext();
    }
}
