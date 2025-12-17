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
            this.groupBoxOptimize = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanelOptimize = new System.Windows.Forms.TableLayoutPanel();
            this.lblOptimizeDevice = new System.Windows.Forms.Label();
            this.cmbOptimizeDevice = new System.Windows.Forms.ComboBox();
            this.lblOptimizePrecision = new System.Windows.Forms.Label();
            this.cmbOptimizePrecision = new System.Windows.Forms.ComboBox();
            this.lblOptimizeBatchSize = new System.Windows.Forms.Label();
            this.nudOptimizeBatchSize = new System.Windows.Forms.NumericUpDown();
            this.lblOptimizeStatusLabel = new System.Windows.Forms.Label();
            this.lblOptimizeStatus = new System.Windows.Forms.Label();
            this.btnExportOptimized = new System.Windows.Forms.Button();
            this.progressBarOptimize = new System.Windows.Forms.ProgressBar();
            this.tableLayoutPanel1.SuspendLayout();
            this.panelPath.SuspendLayout();
            this.panelButtons.SuspendLayout();
            this.groupBoxOptimize.SuspendLayout();
            this.tableLayoutPanelOptimize.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudOptimizeBatchSize)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 101F));
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
            this.tableLayoutPanel1.Controls.Add(this.groupBoxOptimize, 0, 8);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(11, 12);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 10;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 38F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 38F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 38F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 38F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 34F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 34F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 38F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 54F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 192F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(428, 528);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // lblName
            // 
            this.lblName.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblName.AutoSize = true;
            this.lblName.Location = new System.Drawing.Point(3, 10);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(89, 18);
            this.lblName.TabIndex = 0;
            this.lblName.Text = "模型名称:";
            // 
            // txtName
            // 
            this.txtName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtName.Location = new System.Drawing.Point(104, 5);
            this.txtName.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtName.Name = "txtName";
            this.txtName.ReadOnly = true;
            this.txtName.Size = new System.Drawing.Size(321, 28);
            this.txtName.TabIndex = 1;
            // 
            // lblDeviceType
            // 
            this.lblDeviceType.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblDeviceType.AutoSize = true;
            this.lblDeviceType.Location = new System.Drawing.Point(3, 48);
            this.lblDeviceType.Name = "lblDeviceType";
            this.lblDeviceType.Size = new System.Drawing.Size(89, 18);
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
            this.cmbDeviceType.Location = new System.Drawing.Point(104, 44);
            this.cmbDeviceType.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cmbDeviceType.Name = "cmbDeviceType";
            this.cmbDeviceType.Size = new System.Drawing.Size(134, 26);
            this.cmbDeviceType.TabIndex = 3;
            // 
            // lblRuntime
            // 
            this.lblRuntime.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblRuntime.AutoSize = true;
            this.lblRuntime.Location = new System.Drawing.Point(3, 86);
            this.lblRuntime.Name = "lblRuntime";
            this.lblRuntime.Size = new System.Drawing.Size(71, 18);
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
            this.cmbRuntime.Location = new System.Drawing.Point(104, 82);
            this.cmbRuntime.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cmbRuntime.Name = "cmbRuntime";
            this.cmbRuntime.Size = new System.Drawing.Size(134, 26);
            this.cmbRuntime.TabIndex = 5;
            // 
            // lblModelPath
            // 
            this.lblModelPath.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblModelPath.AutoSize = true;
            this.lblModelPath.Location = new System.Drawing.Point(3, 124);
            this.lblModelPath.Name = "lblModelPath";
            this.lblModelPath.Size = new System.Drawing.Size(89, 18);
            this.lblModelPath.TabIndex = 6;
            this.lblModelPath.Text = "模型路径:";
            // 
            // panelPath
            // 
            this.panelPath.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.panelPath.Controls.Add(this.btnBrowse);
            this.panelPath.Controls.Add(this.txtModelPath);
            this.panelPath.Location = new System.Drawing.Point(101, 118);
            this.panelPath.Margin = new System.Windows.Forms.Padding(0, 4, 3, 4);
            this.panelPath.Name = "panelPath";
            this.panelPath.Size = new System.Drawing.Size(324, 30);
            this.panelPath.TabIndex = 7;
            // 
            // btnBrowse
            // 
            this.btnBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowse.Location = new System.Drawing.Point(268, 0);
            this.btnBrowse.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(56, 31);
            this.btnBrowse.TabIndex = 1;
            this.btnBrowse.Text = "...";
            this.btnBrowse.UseVisualStyleBackColor = true;
            // 
            // txtModelPath
            // 
            this.txtModelPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtModelPath.Location = new System.Drawing.Point(0, 1);
            this.txtModelPath.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtModelPath.Name = "txtModelPath";
            this.txtModelPath.Size = new System.Drawing.Size(264, 28);
            this.txtModelPath.TabIndex = 0;
            // 
            // lblStatus
            // 
            this.lblStatus.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new System.Drawing.Point(3, 160);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(89, 18);
            this.lblStatus.TabIndex = 8;
            this.lblStatus.Text = "加载状态:";
            // 
            // lblStatusValue
            // 
            this.lblStatusValue.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblStatusValue.AutoSize = true;
            this.lblStatusValue.ForeColor = System.Drawing.Color.Gray;
            this.lblStatusValue.Location = new System.Drawing.Point(104, 160);
            this.lblStatusValue.Name = "lblStatusValue";
            this.lblStatusValue.Size = new System.Drawing.Size(62, 18);
            this.lblStatusValue.TabIndex = 9;
            this.lblStatusValue.Text = "未加载";
            // 
            // lblClassCount
            // 
            this.lblClassCount.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblClassCount.AutoSize = true;
            this.lblClassCount.Location = new System.Drawing.Point(3, 194);
            this.lblClassCount.Name = "lblClassCount";
            this.lblClassCount.Size = new System.Drawing.Size(89, 18);
            this.lblClassCount.TabIndex = 10;
            this.lblClassCount.Text = "类别数量:";
            // 
            // lblClassCountValue
            // 
            this.lblClassCountValue.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblClassCountValue.AutoSize = true;
            this.lblClassCountValue.Location = new System.Drawing.Point(104, 194);
            this.lblClassCountValue.Name = "lblClassCountValue";
            this.lblClassCountValue.Size = new System.Drawing.Size(17, 18);
            this.lblClassCountValue.TabIndex = 11;
            this.lblClassCountValue.Text = "-";
            // 
            // chkLoadOnStartup
            // 
            this.chkLoadOnStartup.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.chkLoadOnStartup.AutoSize = true;
            this.chkLoadOnStartup.Location = new System.Drawing.Point(104, 228);
            this.chkLoadOnStartup.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.chkLoadOnStartup.Name = "chkLoadOnStartup";
            this.chkLoadOnStartup.Size = new System.Drawing.Size(124, 22);
            this.chkLoadOnStartup.TabIndex = 12;
            this.chkLoadOnStartup.Text = "启动时加载";
            this.chkLoadOnStartup.UseVisualStyleBackColor = true;
            // 
            // panelButtons
            // 
            this.panelButtons.Controls.Add(this.btnUnload);
            this.panelButtons.Controls.Add(this.btnLoad);
            this.panelButtons.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelButtons.Location = new System.Drawing.Point(104, 262);
            this.panelButtons.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.panelButtons.Name = "panelButtons";
            this.panelButtons.Size = new System.Drawing.Size(321, 46);
            this.panelButtons.TabIndex = 13;
            // 
            // btnUnload
            // 
            this.btnUnload.Location = new System.Drawing.Point(107, 6);
            this.btnUnload.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnUnload.Name = "btnUnload";
            this.btnUnload.Size = new System.Drawing.Size(96, 34);
            this.btnUnload.TabIndex = 1;
            this.btnUnload.Text = "卸载模型";
            this.btnUnload.UseVisualStyleBackColor = true;
            // 
            // btnLoad
            // 
            this.btnLoad.Location = new System.Drawing.Point(0, 6);
            this.btnLoad.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(96, 34);
            this.btnLoad.TabIndex = 0;
            this.btnLoad.Text = "加载模型";
            this.btnLoad.UseVisualStyleBackColor = true;
            // 
            // groupBoxOptimize
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.groupBoxOptimize, 2);
            this.groupBoxOptimize.Controls.Add(this.tableLayoutPanelOptimize);
            this.groupBoxOptimize.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxOptimize.Location = new System.Drawing.Point(3, 316);
            this.groupBoxOptimize.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBoxOptimize.Name = "groupBoxOptimize";
            this.groupBoxOptimize.Padding = new System.Windows.Forms.Padding(7, 7, 7, 7);
            this.groupBoxOptimize.Size = new System.Drawing.Size(422, 184);
            this.groupBoxOptimize.TabIndex = 14;
            this.groupBoxOptimize.TabStop = false;
            this.groupBoxOptimize.Text = "模型优化导出";
            // 
            // tableLayoutPanelOptimize
            // 
            this.tableLayoutPanelOptimize.ColumnCount = 4;
            this.tableLayoutPanelOptimize.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 84F));
            this.tableLayoutPanelOptimize.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanelOptimize.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 68F));
            this.tableLayoutPanelOptimize.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanelOptimize.Controls.Add(this.lblOptimizeDevice, 0, 0);
            this.tableLayoutPanelOptimize.Controls.Add(this.cmbOptimizeDevice, 1, 0);
            this.tableLayoutPanelOptimize.Controls.Add(this.lblOptimizePrecision, 0, 1);
            this.tableLayoutPanelOptimize.Controls.Add(this.cmbOptimizePrecision, 1, 1);
            this.tableLayoutPanelOptimize.Controls.Add(this.lblOptimizeBatchSize, 2, 1);
            this.tableLayoutPanelOptimize.Controls.Add(this.nudOptimizeBatchSize, 3, 1);
            this.tableLayoutPanelOptimize.Controls.Add(this.lblOptimizeStatusLabel, 0, 2);
            this.tableLayoutPanelOptimize.Controls.Add(this.lblOptimizeStatus, 1, 2);
            this.tableLayoutPanelOptimize.Controls.Add(this.btnExportOptimized, 1, 3);
            this.tableLayoutPanelOptimize.Controls.Add(this.progressBarOptimize, 1, 4);
            this.tableLayoutPanelOptimize.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanelOptimize.Location = new System.Drawing.Point(7, 28);
            this.tableLayoutPanelOptimize.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tableLayoutPanelOptimize.Name = "tableLayoutPanelOptimize";
            this.tableLayoutPanelOptimize.RowCount = 5;
            this.tableLayoutPanelOptimize.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 34F));
            this.tableLayoutPanelOptimize.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 34F));
            this.tableLayoutPanelOptimize.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanelOptimize.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 42F));
            this.tableLayoutPanelOptimize.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelOptimize.Size = new System.Drawing.Size(408, 149);
            this.tableLayoutPanelOptimize.TabIndex = 0;
            // 
            // lblOptimizeDevice
            // 
            this.lblOptimizeDevice.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblOptimizeDevice.AutoSize = true;
            this.lblOptimizeDevice.Location = new System.Drawing.Point(3, 8);
            this.lblOptimizeDevice.Name = "lblOptimizeDevice";
            this.lblOptimizeDevice.Size = new System.Drawing.Size(53, 18);
            this.lblOptimizeDevice.TabIndex = 0;
            this.lblOptimizeDevice.Text = "设备:";
            // 
            // cmbOptimizeDevice
            // 
            this.cmbOptimizeDevice.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanelOptimize.SetColumnSpan(this.cmbOptimizeDevice, 3);
            this.cmbOptimizeDevice.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbOptimizeDevice.FormattingEnabled = true;
            this.cmbOptimizeDevice.Location = new System.Drawing.Point(87, 4);
            this.cmbOptimizeDevice.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cmbOptimizeDevice.Name = "cmbOptimizeDevice";
            this.cmbOptimizeDevice.Size = new System.Drawing.Size(318, 26);
            this.cmbOptimizeDevice.TabIndex = 1;
            // 
            // lblOptimizePrecision
            // 
            this.lblOptimizePrecision.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblOptimizePrecision.AutoSize = true;
            this.lblOptimizePrecision.Location = new System.Drawing.Point(3, 42);
            this.lblOptimizePrecision.Name = "lblOptimizePrecision";
            this.lblOptimizePrecision.Size = new System.Drawing.Size(71, 18);
            this.lblOptimizePrecision.TabIndex = 2;
            this.lblOptimizePrecision.Text = "精确率:";
            // 
            // cmbOptimizePrecision
            // 
            this.cmbOptimizePrecision.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.cmbOptimizePrecision.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbOptimizePrecision.FormattingEnabled = true;
            this.cmbOptimizePrecision.Items.AddRange(new object[] {
            "浮点 32位",
            "浮点 16位",
            "整型 8位"});
            this.cmbOptimizePrecision.Location = new System.Drawing.Point(87, 38);
            this.cmbOptimizePrecision.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cmbOptimizePrecision.Name = "cmbOptimizePrecision";
            this.cmbOptimizePrecision.Size = new System.Drawing.Size(112, 26);
            this.cmbOptimizePrecision.TabIndex = 3;
            // 
            // lblOptimizeBatchSize
            // 
            this.lblOptimizeBatchSize.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblOptimizeBatchSize.AutoSize = true;
            this.lblOptimizeBatchSize.Location = new System.Drawing.Point(215, 42);
            this.lblOptimizeBatchSize.Name = "lblOptimizeBatchSize";
            this.lblOptimizeBatchSize.Size = new System.Drawing.Size(53, 18);
            this.lblOptimizeBatchSize.TabIndex = 4;
            this.lblOptimizeBatchSize.Text = "批次:";
            // 
            // nudOptimizeBatchSize
            // 
            this.nudOptimizeBatchSize.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.nudOptimizeBatchSize.Location = new System.Drawing.Point(283, 38);
            this.nudOptimizeBatchSize.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.nudOptimizeBatchSize.Maximum = new decimal(new int[] {
            16,
            0,
            0,
            0});
            this.nudOptimizeBatchSize.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudOptimizeBatchSize.Name = "nudOptimizeBatchSize";
            this.nudOptimizeBatchSize.Size = new System.Drawing.Size(68, 28);
            this.nudOptimizeBatchSize.TabIndex = 5;
            this.nudOptimizeBatchSize.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // lblOptimizeStatusLabel
            // 
            this.lblOptimizeStatusLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblOptimizeStatusLabel.AutoSize = true;
            this.lblOptimizeStatusLabel.Location = new System.Drawing.Point(3, 74);
            this.lblOptimizeStatusLabel.Name = "lblOptimizeStatusLabel";
            this.lblOptimizeStatusLabel.Size = new System.Drawing.Size(53, 18);
            this.lblOptimizeStatusLabel.TabIndex = 6;
            this.lblOptimizeStatusLabel.Text = "状态:";
            // 
            // lblOptimizeStatus
            // 
            this.lblOptimizeStatus.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblOptimizeStatus.AutoSize = true;
            this.tableLayoutPanelOptimize.SetColumnSpan(this.lblOptimizeStatus, 3);
            this.lblOptimizeStatus.ForeColor = System.Drawing.Color.Gray;
            this.lblOptimizeStatus.Location = new System.Drawing.Point(87, 74);
            this.lblOptimizeStatus.Name = "lblOptimizeStatus";
            this.lblOptimizeStatus.Size = new System.Drawing.Size(17, 18);
            this.lblOptimizeStatus.TabIndex = 7;
            this.lblOptimizeStatus.Text = "-";
            // 
            // btnExportOptimized
            // 
            this.btnExportOptimized.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.tableLayoutPanelOptimize.SetColumnSpan(this.btnExportOptimized, 3);
            this.btnExportOptimized.Location = new System.Drawing.Point(87, 102);
            this.btnExportOptimized.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnExportOptimized.Name = "btnExportOptimized";
            this.btnExportOptimized.Size = new System.Drawing.Size(135, 34);
            this.btnExportOptimized.TabIndex = 8;
            this.btnExportOptimized.Text = "导出优化模型";
            this.btnExportOptimized.UseVisualStyleBackColor = true;
            // 
            // progressBarOptimize
            // 
            this.progressBarOptimize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanelOptimize.SetColumnSpan(this.progressBarOptimize, 3);
            this.progressBarOptimize.Location = new System.Drawing.Point(87, 144);
            this.progressBarOptimize.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.progressBarOptimize.Name = "progressBarOptimize";
            this.progressBarOptimize.Size = new System.Drawing.Size(318, 1);
            this.progressBarOptimize.TabIndex = 9;
            this.progressBarOptimize.Visible = false;
            // 
            // SemanticConfigControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MinimumSize = new System.Drawing.Size(394, 552);
            this.Name = "SemanticConfigControl";
            this.Padding = new System.Windows.Forms.Padding(11, 12, 11, 12);
            this.Size = new System.Drawing.Size(450, 552);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.panelPath.ResumeLayout(false);
            this.panelPath.PerformLayout();
            this.panelButtons.ResumeLayout(false);
            this.groupBoxOptimize.ResumeLayout(false);
            this.tableLayoutPanelOptimize.ResumeLayout(false);
            this.tableLayoutPanelOptimize.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudOptimizeBatchSize)).EndInit();
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
        private System.Windows.Forms.GroupBox groupBoxOptimize;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelOptimize;
        private System.Windows.Forms.Label lblOptimizeDevice;
        private System.Windows.Forms.ComboBox cmbOptimizeDevice;
        private System.Windows.Forms.Label lblOptimizePrecision;
        private System.Windows.Forms.ComboBox cmbOptimizePrecision;
        private System.Windows.Forms.Label lblOptimizeBatchSize;
        private System.Windows.Forms.NumericUpDown nudOptimizeBatchSize;
        private System.Windows.Forms.Label lblOptimizeStatusLabel;
        private System.Windows.Forms.Label lblOptimizeStatus;
        private System.Windows.Forms.Button btnExportOptimized;
        private System.Windows.Forms.ProgressBar progressBarOptimize;
    }
}
