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
   this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
   this.gbBrightness.SuspendLayout();
   this.gbMode.SuspendLayout();
   this.tableLayoutPanel1.SuspendLayout();
   this.SuspendLayout();
   // 
   // gbBrightness
   // 
   this.gbBrightness.Controls.Add(this.tlpBrightness);
   this.gbBrightness.Dock = System.Windows.Forms.DockStyle.Fill;
   this.gbBrightness.Location = new System.Drawing.Point(3, 3);
   this.gbBrightness.Name = "gbBrightness";
   this.gbBrightness.Size = new System.Drawing.Size(701, 179);
   this.gbBrightness.TabIndex = 1;
   this.gbBrightness.TabStop = false;
   this.gbBrightness.Text = "亮度设置";
   // 
   // tlpBrightness
   // 
   this.tlpBrightness.ColumnCount = 3;
   this.tlpBrightness.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
   this.tlpBrightness.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
   this.tlpBrightness.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
   this.tlpBrightness.Dock = System.Windows.Forms.DockStyle.Fill;
   this.tlpBrightness.Location = new System.Drawing.Point(3, 24);
   this.tlpBrightness.Name = "tlpBrightness";
   this.tlpBrightness.RowCount = 4;
   this.tlpBrightness.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
   this.tlpBrightness.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
   this.tlpBrightness.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
   this.tlpBrightness.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
   this.tlpBrightness.Size = new System.Drawing.Size(695, 152);
   this.tlpBrightness.TabIndex = 0;
   // 
   // gbMode
   // 
   this.gbMode.Controls.Add(this.rbOn);
   this.gbMode.Controls.Add(this.rbOff);
   this.gbMode.Dock = System.Windows.Forms.DockStyle.Fill;
   this.gbMode.Location = new System.Drawing.Point(3, 188);
   this.gbMode.Name = "gbMode";
   this.gbMode.Size = new System.Drawing.Size(701, 78);
   this.gbMode.TabIndex = 0;
   this.gbMode.TabStop = false;
   this.gbMode.Text = "常开/常关设置";
   // 
   // rbOn
   // 
   this.rbOn.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
   this.rbOn.Location = new System.Drawing.Point(190, 32);
   this.rbOn.Name = "rbOn";
   this.rbOn.Size = new System.Drawing.Size(76, 34);
   this.rbOn.TabIndex = 0;
   this.rbOn.Text = "常开";
   // 
   // rbOff
   // 
   this.rbOff.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
   this.rbOff.Location = new System.Drawing.Point(347, 32);
   this.rbOff.Name = "rbOff";
   this.rbOff.Size = new System.Drawing.Size(74, 34);
   this.rbOff.TabIndex = 1;
   this.rbOff.Text = "常关";
   // 
   // tableLayoutPanel1
   // 
   this.tableLayoutPanel1.ColumnCount = 1;
   this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
   this.tableLayoutPanel1.Controls.Add(this.gbMode, 0, 1);
   this.tableLayoutPanel1.Controls.Add(this.gbBrightness, 0, 0);
   this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
   this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
   this.tableLayoutPanel1.Name = "tableLayoutPanel1";
   this.tableLayoutPanel1.RowCount = 2;
   this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 68.77323F));
   this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 31.22677F));
   this.tableLayoutPanel1.Size = new System.Drawing.Size(707, 269);
   this.tableLayoutPanel1.TabIndex = 2;
   // 
   // Frm_FgenTest
   // 
   this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
   this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
   this.BackColor = System.Drawing.SystemColors.ActiveCaption;
   this.ClientSize = new System.Drawing.Size(707, 269);
   this.Controls.Add(this.tableLayoutPanel1);
   this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
   this.MaximizeBox = false;
   this.MinimizeBox = false;
   this.Name = "Frm_FgenTest";
   this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
   this.Text = "FG 测试";
   this.gbBrightness.ResumeLayout(false);
   this.gbMode.ResumeLayout(false);
   this.tableLayoutPanel1.ResumeLayout(false);
   this.ResumeLayout(false);

        }

        private TableLayoutPanel tableLayoutPanel1;
    }
}
