using System.Drawing;
using System.Windows.Forms;

namespace Fgen.LightPlugin.UI
{
    partial class Frm_FgenTest
    {
        private System.ComponentModel.IContainer components = null;
        private GroupBox gbBrightness;
        private TableLayoutPanel tlpBrightness;
        private GroupBox gbMode;
        private RadioButton rbOn;
        private RadioButton rbOff;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            gbBrightness = new GroupBox();
            tlpBrightness = new TableLayoutPanel();
            gbMode = new GroupBox();
            rbOn = new RadioButton();
            rbOff = new RadioButton();

            SuspendLayout();

            gbBrightness.Text = "亮度设置";
            gbBrightness.Dock = DockStyle.Top;
            gbBrightness.Height = 260;
            tlpBrightness.Dock = DockStyle.Fill;
            tlpBrightness.ColumnCount = 3;
            tlpBrightness.RowCount = 4;
            tlpBrightness.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 60));
            tlpBrightness.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            tlpBrightness.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 60));
            gbBrightness.Controls.Add(tlpBrightness);

            gbMode.Text = "常开/常关设置";
            gbMode.Dock = DockStyle.Fill;
            rbOn.Text = "常开";
            rbOn.Left = 30;
            rbOn.Top = 30;
            rbOff.Text = "常关";
            rbOff.Left = 120;
            rbOff.Top = 30;
            gbMode.Controls.Add(rbOn);
            gbMode.Controls.Add(rbOff);

            Controls.Add(gbMode);
            Controls.Add(gbBrightness);

            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(640, 420);
            Text = "FG 测试";

            ResumeLayout(false);
        }
    }
}
