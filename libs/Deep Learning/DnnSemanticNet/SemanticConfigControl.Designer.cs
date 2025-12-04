namespace DnnSemanticNet
{
    partial class SemanticConfigControl
    {
        /// <summary> 
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.lblName = new System.Windows.Forms.Label();
            this.txtName = new System.Windows.Forms.TextBox();
            this.lblDeviceType = new System.Windows.Forms.Label();
            this.cmbDeviceType = new System.Windows.Forms.ComboBox();
            this.lblRuntime = new System.Windows.Forms.Label();
            this.cmbRuntime = new System.Windows.Forms.ComboBox();
            this.lblModelPath = new System.Windows.Forms.Label();
            this.panelPath = new System.Windows.Forms.Panel();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.txtModelPath = new System.Windows.Forms.TextBox();
            this.lblStatus = new System.Windows.Forms.Label();
            this.lblStatusValue = new System.Windows.Forms.Label();
            this.lblClassCount = new System.Windows.Forms.Label();
            this.lblClassCountValue = new System.Windows.Forms.Label();
            this.chkLoadOnStartup = new System.Windows.Forms.CheckBox();
            this.panelButtons = new System.Windows.Forms.Panel();
            this.btnUnload = new System.Windows.Forms.Button();
            this.btnLoad = new System.Windows.Forms.Button();
            this.tableLayoutPanel1.SuspendLayout();
            this.panelPath.SuspendLayout();
            this.panelButtons.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 90F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.lblName, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.txtName, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.lblDeviceType, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.cmbDeviceType, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.lblRuntime, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.cmbRuntime, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.lblModelPath, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.panelPath, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.lblStatus, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.lblStatusValue, 1, 4);
            this.tableLayoutPanel1.Controls.Add(this.lblClassCount, 0, 5);
            this.tableLayoutPanel1.Controls.Add(this.lblClassCountValue, 1, 5);
            this.tableLayoutPanel1.Controls.Add(this.chkLoadOnStartup, 1, 6);
            this.tableLayoutPanel1.Controls.Add(this.panelButtons, 1, 7);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(10, 10);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 9;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 45F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(380, 280);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // lblName
            // 
            this.lblName.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblName.AutoSize = true;
            this.lblName.Location = new System.Drawing.Point(3, 8);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(67, 15);
            this.lblName.TabIndex = 0;
            this.lblName.Text = "模型名称:";
            // 
            // txtName
            // 
            this.txtName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtName.Location = new System.Drawing.Point(93, 4);
            this.txtName.Name = "txtName";
            this.txtName.ReadOnly = true;
            this.txtName.Size = new System.Drawing.Size(284, 25);
            this.txtName.TabIndex = 1;
            // 
            // lblDeviceType
            // 
            this.lblDeviceType.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblDeviceType.AutoSize = true;
            this.lblDeviceType.Location = new System.Drawing.Point(3, 40);
            this.lblDeviceType.Name = "lblDeviceType";
            this.lblDeviceType.Size = new System.Drawing.Size(67, 15);
            this.lblDeviceType.TabIndex = 2;
            this.lblDeviceType.Text = "设备类型:";
            // 
            // cmbDeviceType
            // 
            this.cmbDeviceType.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.cmbDeviceType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDeviceType.FormattingEnabled = true;
            this.cmbDeviceType.Items.AddRange(new object[] {
            "GPU",
            "CPU"});
            this.cmbDeviceType.Location = new System.Drawing.Point(93, 36);
            this.cmbDeviceType.Name = "cmbDeviceType";
            this.cmbDeviceType.Size = new System.Drawing.Size(120, 23);
            this.cmbDeviceType.TabIndex = 3;
            // 
            // lblRuntime
            // 
            this.lblRuntime.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblRuntime.AutoSize = true;
            this.lblRuntime.Location = new System.Drawing.Point(3, 72);
            this.lblRuntime.Name = "lblRuntime";
            this.lblRuntime.Size = new System.Drawing.Size(55, 15);
            this.lblRuntime.TabIndex = 4;
            this.lblRuntime.Text = "运行时:";
            // 
            // cmbRuntime
            // 
            this.cmbRuntime.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.cmbRuntime.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbRuntime.FormattingEnabled = true;
            this.cmbRuntime.Items.AddRange(new object[] {
            "GC",
            "OpenVINO",
            "TensorRT"});
            this.cmbRuntime.Location = new System.Drawing.Point(93, 68);
            this.cmbRuntime.Name = "cmbRuntime";
            this.cmbRuntime.Size = new System.Drawing.Size(120, 23);
            this.cmbRuntime.TabIndex = 5;
            // 
            // lblModelPath
            // 
            this.lblModelPath.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblModelPath.AutoSize = true;
            this.lblModelPath.Location = new System.Drawing.Point(3, 104);
            this.lblModelPath.Name = "lblModelPath";
            this.lblModelPath.Size = new System.Drawing.Size(67, 15);
            this.lblModelPath.TabIndex = 6;
            this.lblModelPath.Text = "模型路径:";
            // 
            // panelPath
            // 
            this.panelPath.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.panelPath.Controls.Add(this.btnBrowse);
            this.panelPath.Controls.Add(this.txtModelPath);
            this.panelPath.Location = new System.Drawing.Point(90, 99);
            this.panelPath.Margin = new System.Windows.Forms.Padding(0, 3, 3, 3);
            this.panelPath.Name = "panelPath";
            this.panelPath.Size = new System.Drawing.Size(287, 26);
            this.panelPath.TabIndex = 7;
            // 
            // btnBrowse
            // 
            this.btnBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowse.Location = new System.Drawing.Point(237, 0);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(50, 26);
            this.btnBrowse.TabIndex = 1;
            this.btnBrowse.Text = "...";
            this.btnBrowse.UseVisualStyleBackColor = true;
            // 
            // txtModelPath
            // 
            this.txtModelPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtModelPath.Location = new System.Drawing.Point(0, 1);
            this.txtModelPath.Name = "txtModelPath";
            this.txtModelPath.Size = new System.Drawing.Size(234, 25);
            this.txtModelPath.TabIndex = 0;
            // 
            // lblStatus
            // 
            this.lblStatus.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new System.Drawing.Point(3, 134);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(67, 15);
            this.lblStatus.TabIndex = 8;
            this.lblStatus.Text = "加载状态:";
            // 
            // lblStatusValue
            // 
            this.lblStatusValue.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblStatusValue.AutoSize = true;
            this.lblStatusValue.ForeColor = System.Drawing.Color.Gray;
            this.lblStatusValue.Location = new System.Drawing.Point(93, 134);
            this.lblStatusValue.Name = "lblStatusValue";
            this.lblStatusValue.Size = new System.Drawing.Size(52, 15);
            this.lblStatusValue.TabIndex = 9;
            this.lblStatusValue.Text = "未加载";
            // 
            // lblClassCount
            // 
            this.lblClassCount.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblClassCount.AutoSize = true;
            this.lblClassCount.Location = new System.Drawing.Point(3, 162);
            this.lblClassCount.Name = "lblClassCount";
            this.lblClassCount.Size = new System.Drawing.Size(67, 15);
            this.lblClassCount.TabIndex = 10;
            this.lblClassCount.Text = "类别数量:";
            // 
            // lblClassCountValue
            // 
            this.lblClassCountValue.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblClassCountValue.AutoSize = true;
            this.lblClassCountValue.Location = new System.Drawing.Point(93, 162);
            this.lblClassCountValue.Name = "lblClassCountValue";
            this.lblClassCountValue.Size = new System.Drawing.Size(14, 15);
            this.lblClassCountValue.TabIndex = 11;
            this.lblClassCountValue.Text = "-";
            // 
            // chkLoadOnStartup
            // 
            this.chkLoadOnStartup.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.chkLoadOnStartup.AutoSize = true;
            this.chkLoadOnStartup.Location = new System.Drawing.Point(93, 192);
            this.chkLoadOnStartup.Name = "chkLoadOnStartup";
            this.chkLoadOnStartup.Size = new System.Drawing.Size(104, 19);
            this.chkLoadOnStartup.TabIndex = 12;
            this.chkLoadOnStartup.Text = "启动时加载";
            this.chkLoadOnStartup.UseVisualStyleBackColor = true;
            // 
            // panelButtons
            // 
            this.panelButtons.Controls.Add(this.btnUnload);
            this.panelButtons.Controls.Add(this.btnLoad);
            this.panelButtons.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelButtons.Location = new System.Drawing.Point(93, 219);
            this.panelButtons.Name = "panelButtons";
            this.panelButtons.Size = new System.Drawing.Size(284, 39);
            this.panelButtons.TabIndex = 13;
            // 
            // btnUnload
            // 
            this.btnUnload.Location = new System.Drawing.Point(95, 5);
            this.btnUnload.Name = "btnUnload";
            this.btnUnload.Size = new System.Drawing.Size(85, 28);
            this.btnUnload.TabIndex = 1;
            this.btnUnload.Text = "卸载模型";
            this.btnUnload.UseVisualStyleBackColor = true;
            // 
            // btnLoad
            // 
            this.btnLoad.Location = new System.Drawing.Point(0, 5);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(85, 28);
            this.btnLoad.TabIndex = 0;
            this.btnLoad.Text = "加载模型";
            this.btnLoad.UseVisualStyleBackColor = true;
            // 
            // SemanticConfigControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.MinimumSize = new System.Drawing.Size(350, 280);
            this.Name = "SemanticConfigControl";
            this.Padding = new System.Windows.Forms.Padding(10);
            this.Size = new System.Drawing.Size(400, 300);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.panelPath.ResumeLayout(false);
            this.panelPath.PerformLayout();
            this.panelButtons.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.Label lblDeviceType;
        private System.Windows.Forms.ComboBox cmbDeviceType;
        private System.Windows.Forms.Label lblRuntime;
        private System.Windows.Forms.ComboBox cmbRuntime;
        private System.Windows.Forms.Label lblModelPath;
        private System.Windows.Forms.Panel panelPath;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.TextBox txtModelPath;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Label lblStatusValue;
        private System.Windows.Forms.Label lblClassCount;
        private System.Windows.Forms.Label lblClassCountValue;
        private System.Windows.Forms.CheckBox chkLoadOnStartup;
        private System.Windows.Forms.Panel panelButtons;
        private System.Windows.Forms.Button btnUnload;
        private System.Windows.Forms.Button btnLoad;
    }
}
