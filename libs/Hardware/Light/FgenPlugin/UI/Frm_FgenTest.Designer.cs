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
            this.gbBrightness = new System.Windows.Forms.GroupBox();
            this.tlpBrightness = new System.Windows.Forms.TableLayoutPanel();
            this.gbMode = new System.Windows.Forms.GroupBox();
            this.rbOn = new System.Windows.Forms.RadioButton();
            this.rbOff = new System.Windows.Forms.RadioButton();
            this.gbBrightness.SuspendLayout();
            this.gbMode.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbBrightness
            // 
            this.gbBrightness.Controls.Add(this.tlpBrightness);
            this.gbBrightness.Dock = System.Windows.Forms.DockStyle.Top;
            this.gbBrightness.Location = new System.Drawing.Point(0, 0);
            this.gbBrightness.Name = "gbBrightness";
            this.gbBrightness.Size = new System.Drawing.Size(640, 260);
            this.gbBrightness.TabIndex = 1;
            this.gbBrightness.TabStop = false;
            this.gbBrightness.Text = "亮度设置";
            // 
            // tlpBrightness
            // 
            this.tlpBrightness.ColumnCount = 3;
            this.tlpBrightness.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.tlpBrightness.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpBrightness.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.tlpBrightness.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpBrightness.Location = new System.Drawing.Point(3, 24);
            this.tlpBrightness.Name = "tlpBrightness";
            this.tlpBrightness.RowCount = 4;
            this.tlpBrightness.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpBrightness.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpBrightness.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpBrightness.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpBrightness.Size = new System.Drawing.Size(634, 233);
            this.tlpBrightness.TabIndex = 0;
            // 
            // gbMode
            // 
            this.gbMode.Controls.Add(this.rbOn);
            this.gbMode.Controls.Add(this.rbOff);
            this.gbMode.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbMode.Location = new System.Drawing.Point(0, 260);
            this.gbMode.Name = "gbMode";
            this.gbMode.Size = new System.Drawing.Size(640, 68);
            this.gbMode.TabIndex = 0;
            this.gbMode.TabStop = false;
            this.gbMode.Text = "常开/常关设置";
            // 
            // rbOn
            // 
            this.rbOn.Location = new System.Drawing.Point(30, 30);
            this.rbOn.Name = "rbOn";
            this.rbOn.Size = new System.Drawing.Size(104, 24);
            this.rbOn.TabIndex = 0;
            this.rbOn.Text = "常开";
            // 
            // rbOff
            // 
            this.rbOff.Location = new System.Drawing.Point(140, 30);
            this.rbOff.Name = "rbOff";
            this.rbOff.Size = new System.Drawing.Size(104, 24);
            this.rbOff.TabIndex = 1;
            this.rbOff.Text = "常关";
            // 
            // Frm_FgenTest
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(640, 328);
            this.Controls.Add(this.gbMode);
            this.Controls.Add(this.gbBrightness);
            this.Name = "Frm_FgenTest";
            this.Text = "FG 测试";
            this.gbBrightness.ResumeLayout(false);
            this.gbMode.ResumeLayout(false);
            this.ResumeLayout(false);

        }
    }
}
