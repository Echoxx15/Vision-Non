using System;
using System.Windows.Forms;

namespace UniVision.Forms;
public partial class Frm_Splash : Form
{
    public Frm_Splash()
    {
        InitializeComponent();
    }

    public void SetProgress(int percent, string info)
    {
        if (InvokeRequired)
            Invoke(new Action<int, string>(SetProgress), percent, info);
        else
        {
            progressBar1.Value = percent;
            lbl_Splash.Text = info;
        }
    }
}
