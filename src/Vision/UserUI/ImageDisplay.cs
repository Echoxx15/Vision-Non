using System;
using System.ComponentModel;
using System.Windows.Forms;
using Cognex.VisionPro;

namespace Vision.UserUI;

[Description("视图窗体")]
public partial class ImageDisplay : UserControl
{
    public ICogImage CogImage
    {
        get => cogRecordDisplay.Image;
        set
        {
            if (InvokeRequired) { BeginInvoke(new Action(() => CogImage = value)); return; }
            cogRecordDisplay.StaticGraphics.Clear();
            cogRecordDisplay.InteractiveGraphics.Clear();
            cogRecordDisplay.Image = value;
            cogRecordDisplay.Fit(true);
        }
    }

    public ICogRecord Record
    {
        get => cogRecordDisplay.Record;
        set
        {
            if (InvokeRequired) { BeginInvoke(new Action(() => Record = value)); return; }
            cogRecordDisplay.StaticGraphics.Clear();
            cogRecordDisplay.InteractiveGraphics.Clear();
            cogRecordDisplay.Record = value;
            cogRecordDisplay.Fit(true);
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

    public ImageDisplay()
    {
        InitializeComponent();
    }

    public ImageDisplay(string name)
    {
        InitializeComponent();
        grb_ShowName.Text = name;
    }

    private void cogRecordDisplay_DoubleClick(object sender, EventArgs e)
    {
        var recordDisplay = sender as CogRecordDisplay;
        var maxDisplay = new MaxDisplay(recordDisplay, DisplayName);
        maxDisplay.ShowDialog();
    }

    private void ImageDisplay_Load(object sender, EventArgs e)
    {
        Dock = DockStyle.Fill;
    }
}
