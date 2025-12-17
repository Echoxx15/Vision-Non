namespace Vision.Frm.Station
{
    partial class Frm_StationConfig
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.tree_Station = new System.Windows.Forms.TreeView();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsb_Add = new System.Windows.Forms.ToolStripMenuItem();
            this.tsm_Remove = new System.Windows.Forms.ToolStripMenuItem();
            this.tsm_ReName = new System.Windows.Forms.ToolStripMenuItem();
            this.tsm_LoadImageRun = new System.Windows.Forms.ToolStripMenuItem();
            this.tsm_TriggerCameraRun = new System.Windows.Forms.ToolStripMenuItem();
            this.tsm_SimulateFlyCapture = new System.Windows.Forms.ToolStripMenuItem();
            this.safePropertyGrid1 = new Vision.Controls.SafePropertyGrid();
            this.contextMenuStrip2 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsm_OpenForm = new System.Windows.Forms.ToolStripMenuItem();
            this.tableLayoutPanel1.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            this.contextMenuStrip2.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 21.802F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 78.198F));
            this.tableLayoutPanel1.Controls.Add(this.panel1, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.tree_Station, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.safePropertyGrid1, 1, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 91.05812F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 8.941877F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(899, 671);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(198, 614);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(698, 54);
            this.panel1.TabIndex = 2;
            // 
            // tree_Station
            // 
            this.tree_Station.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.tree_Station.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tree_Station.ContextMenuStrip = this.contextMenuStrip1;
            this.tree_Station.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tree_Station.Font = new System.Drawing.Font("Microsoft YaHei UI", 10F);
            this.tree_Station.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.tree_Station.FullRowSelect = true;
            this.tree_Station.HideSelection = false;
            this.tree_Station.ItemHeight = 32;
            this.tree_Station.Location = new System.Drawing.Point(3, 3);
            this.tree_Station.Name = "tree_Station";
            this.tree_Station.ShowLines = false;
            this.tree_Station.Size = new System.Drawing.Size(189, 605);
            this.tree_Station.TabIndex = 3;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsb_Add,
            this.tsm_Remove,
            this.tsm_ReName,
            this.tsm_LoadImageRun,
            this.tsm_TriggerCameraRun,
            this.tsm_SimulateFlyCapture});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(233, 184);
            // 
            // tsb_Add
            // 
            this.tsb_Add.Name = "tsb_Add";
            this.tsb_Add.Size = new System.Drawing.Size(232, 30);
            this.tsb_Add.Text = "添加";
            this.tsb_Add.Click += new System.EventHandler(this.tsb_Add_Click);
            // 
            // tsm_Remove
            // 
            this.tsm_Remove.Name = "tsm_Remove";
            this.tsm_Remove.Size = new System.Drawing.Size(232, 30);
            this.tsm_Remove.Text = "删除";
            this.tsm_Remove.Click += new System.EventHandler(this.tsm_Remove_Click);
            // 
            // tsm_ReName
            // 
            this.tsm_ReName.Name = "tsm_ReName";
            this.tsm_ReName.Size = new System.Drawing.Size(232, 30);
            this.tsm_ReName.Text = "重命名";
            this.tsm_ReName.Click += new System.EventHandler(this.tsm_ReName_Click);
            // 
            // tsm_LoadImageRun
            // 
            this.tsm_LoadImageRun.Name = "tsm_LoadImageRun";
            this.tsm_LoadImageRun.Size = new System.Drawing.Size(232, 30);
            this.tsm_LoadImageRun.Text = "运行检测-加载图片";
            // 
            // tsm_TriggerCameraRun
            // 
            this.tsm_TriggerCameraRun.Name = "tsm_TriggerCameraRun";
            this.tsm_TriggerCameraRun.Size = new System.Drawing.Size(232, 30);
            this.tsm_TriggerCameraRun.Text = "运行检测-触发相机";
            // 
            // tsm_SimulateFlyCapture
            // 
            this.tsm_SimulateFlyCapture.Name = "tsm_SimulateFlyCapture";
            this.tsm_SimulateFlyCapture.Size = new System.Drawing.Size(232, 30);
            this.tsm_SimulateFlyCapture.Text = "模拟飞拍流程";
            // 
            // safePropertyGrid1
            // 
            this.safePropertyGrid1.BlockDoubleClickToggle = true;
            this.safePropertyGrid1.BlockMouseWheelOnDropDown = true;
            this.safePropertyGrid1.CategorySplitterColor = System.Drawing.SystemColors.ActiveCaption;
            this.safePropertyGrid1.ContextMenuStrip = this.contextMenuStrip2;
            this.safePropertyGrid1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.safePropertyGrid1.HelpBackColor = System.Drawing.SystemColors.ActiveCaption;
            this.safePropertyGrid1.LineColor = System.Drawing.SystemColors.ActiveCaption;
            this.safePropertyGrid1.Location = new System.Drawing.Point(198, 3);
            this.safePropertyGrid1.Name = "safePropertyGrid1";
            this.safePropertyGrid1.SelectedItemWithFocusForeColor = System.Drawing.SystemColors.ActiveCaption;
            this.safePropertyGrid1.Size = new System.Drawing.Size(698, 605);
            this.safePropertyGrid1.TabIndex = 4;
            this.safePropertyGrid1.ViewBackColor = System.Drawing.SystemColors.ActiveCaption;
            // 
            // contextMenuStrip2
            // 
            this.contextMenuStrip2.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.contextMenuStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsm_OpenForm});
            this.contextMenuStrip2.Name = "contextMenuStrip2";
            this.contextMenuStrip2.Size = new System.Drawing.Size(153, 34);
            // 
            // tsm_OpenForm
            // 
            this.tsm_OpenForm.Name = "tsm_OpenForm";
            this.tsm_OpenForm.Size = new System.Drawing.Size(152, 30);
            this.tsm_OpenForm.Text = "打开配置";
            // 
            // Frm_StationConfig
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ClientSize = new System.Drawing.Size(899, 671);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "Frm_StationConfig";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "工位配置";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.contextMenuStrip1.ResumeLayout(false);
            this.contextMenuStrip2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TreeView tree_Station;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem tsb_Add;
        private System.Windows.Forms.ToolStripMenuItem tsm_Remove;
        private System.Windows.Forms.ToolStripMenuItem tsm_ReName;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip2;
        private System.Windows.Forms.ToolStripMenuItem tsm_OpenForm;
        private System.Windows.Forms.ToolStripMenuItem tsm_LoadImageRun;
        private System.Windows.Forms.ToolStripMenuItem tsm_TriggerCameraRun;
        private System.Windows.Forms.ToolStripMenuItem tsm_SimulateFlyCapture;
        private Controls.SafePropertyGrid safePropertyGrid1;
    }
}
