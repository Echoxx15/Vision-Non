namespace Vision.Frm.FrmHardwareState
{
    partial class Frm_HardwareState
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tablePanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.flowlayoutPanel_HardCamera = new System.Windows.Forms.FlowLayoutPanel();
            this.flowlayoutPanel_HardComm = new System.Windows.Forms.FlowLayoutPanel();
            this.tablePanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tablePanel1
            // 
            this.tablePanel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.tablePanel1.ColumnCount = 2;
            this.tablePanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tablePanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tablePanel1.Controls.Add(this.flowlayoutPanel_HardCamera, 0, 0);
            this.tablePanel1.Controls.Add(this.flowlayoutPanel_HardComm, 1, 0);
            this.tablePanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tablePanel1.Location = new System.Drawing.Point(0, 0);
            this.tablePanel1.Name = "tablePanel1";
            this.tablePanel1.RowCount = 1;
            this.tablePanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tablePanel1.Size = new System.Drawing.Size(367, 413);
            this.tablePanel1.TabIndex = 0;
            // 
            // flowlayoutPanel_HardCamera
            // 
            this.flowlayoutPanel_HardCamera.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowlayoutPanel_HardCamera.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowlayoutPanel_HardCamera.Location = new System.Drawing.Point(3, 3);
            this.flowlayoutPanel_HardCamera.Name = "flowlayoutPanel_HardCamera";
            this.flowlayoutPanel_HardCamera.Size = new System.Drawing.Size(177, 407);
            this.flowlayoutPanel_HardCamera.TabIndex = 1;
            // 
            // flowlayoutPanel_HardComm
            // 
            this.flowlayoutPanel_HardComm.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowlayoutPanel_HardComm.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowlayoutPanel_HardComm.Location = new System.Drawing.Point(186, 3);
            this.flowlayoutPanel_HardComm.Name = "flowlayoutPanel_HardComm";
            this.flowlayoutPanel_HardComm.Size = new System.Drawing.Size(178, 407);
            this.flowlayoutPanel_HardComm.TabIndex = 0;
            // 
            // Frm_HardwareState
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 22F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tablePanel1);
            this.Name = "Frm_HardwareState";
            this.Size = new System.Drawing.Size(367, 413);
            this.tablePanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tablePanel1;
        private System.Windows.Forms.FlowLayoutPanel flowlayoutPanel_HardCamera;
        private System.Windows.Forms.FlowLayoutPanel flowlayoutPanel_HardComm;
    }
}
