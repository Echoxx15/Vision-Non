namespace DnnOCRNet
{
    partial class OCRConfigControl
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
            this.chkLoadOnStartup = new System.Windows.Forms.CheckBox();
            this.groupBoxOCR = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanelOCR = new System.Windows.Forms.TableLayoutPanel();
            this.lblBatchSize = new System.Windows.Forms.Label();
            this.nudBatchSize = new System.Windows.Forms.NumericUpDown();
            this.lblTiling = new System.Windows.Forms.Label();
            this.chkTiling = new System.Windows.Forms.CheckBox();
            this.lblOverlap = new System.Windows.Forms.Label();
            this.nudOverlap = new System.Windows.Forms.NumericUpDown();
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
            this.groupBoxOCR.SuspendLayout();
            this.tableLayoutPanelOCR.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudBatchSize)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudOverlap)).BeginInit();
            this.panelButtons.SuspendLayout();
            this.groupBoxOptimize.SuspendLayout();
            this.tableLayoutPanelOptimize.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudOptimizeBatchSize)).BeginInit();
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
            this.tableLayoutPanel1.Controls.Add(this.chkLoadOnStartup, 1, 5);
            this.tableLayoutPanel1.Controls.Add(this.groupBoxOCR, 0, 6);
            this.tableLayoutPanel1.Controls.Add(this.panelButtons, 1, 7);
            this.tableLayoutPanel1.Controls.Add(this.groupBoxOptimize, 0, 8);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(10, 10);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 10;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 45F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 160F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(380, 530);
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
            // chkLoadOnStartup
            // 
            this.chkLoadOnStartup.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.chkLoadOnStartup.AutoSize = true;
            this.chkLoadOnStartup.Location = new System.Drawing.Point(93, 162);
            this.chkLoadOnStartup.Name = "chkLoadOnStartup";
            this.chkLoadOnStartup.Size = new System.Drawing.Size(104, 19);
            this.chkLoadOnStartup.TabIndex = 10;
            this.chkLoadOnStartup.Text = "启动时加载";
            this.chkLoadOnStartup.UseVisualStyleBackColor = true;
            // 
            // groupBoxOCR
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.groupBoxOCR, 2);
            this.groupBoxOCR.Controls.Add(this.tableLayoutPanelOCR);
            this.groupBoxOCR.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxOCR.Location = new System.Drawing.Point(3, 191);
            this.groupBoxOCR.Name = "groupBoxOCR";
            this.groupBoxOCR.Padding = new System.Windows.Forms.Padding(6);
            this.groupBoxOCR.Size = new System.Drawing.Size(374, 94);
            this.groupBoxOCR.TabIndex = 11;
            this.groupBoxOCR.TabStop = false;
            this.groupBoxOCR.Text = "OCR 参数";
            // 
            // tableLayoutPanelOCR
            // 
            this.tableLayoutPanelOCR.ColumnCount = 4;
            this.tableLayoutPanelOCR.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 75F));
            this.tableLayoutPanelOCR.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 80F));
            this.tableLayoutPanelOCR.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 75F));
            this.tableLayoutPanelOCR.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelOCR.Controls.Add(this.lblBatchSize, 0, 0);
            this.tableLayoutPanelOCR.Controls.Add(this.nudBatchSize, 1, 0);
            this.tableLayoutPanelOCR.Controls.Add(this.lblTiling, 0, 1);
            this.tableLayoutPanelOCR.Controls.Add(this.chkTiling, 1, 1);
            this.tableLayoutPanelOCR.Controls.Add(this.lblOverlap, 2, 1);
            this.tableLayoutPanelOCR.Controls.Add(this.nudOverlap, 3, 1);
            this.tableLayoutPanelOCR.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanelOCR.Location = new System.Drawing.Point(6, 22);
            this.tableLayoutPanelOCR.Name = "tableLayoutPanelOCR";
            this.tableLayoutPanelOCR.RowCount = 2;
            this.tableLayoutPanelOCR.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            this.tableLayoutPanelOCR.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            this.tableLayoutPanelOCR.Size = new System.Drawing.Size(362, 66);
            this.tableLayoutPanelOCR.TabIndex = 0;
            // 
            // lblBatchSize
            // 
            this.lblBatchSize.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblBatchSize.AutoSize = true;
            this.lblBatchSize.Location = new System.Drawing.Point(3, 8);
            this.lblBatchSize.Name = "lblBatchSize";
            this.lblBatchSize.Size = new System.Drawing.Size(67, 15);
            this.lblBatchSize.TabIndex = 0;
            this.lblBatchSize.Text = "识别批量:";
            // 
            // nudBatchSize
            // 
            this.nudBatchSize.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.nudBatchSize.Location = new System.Drawing.Point(78, 4);
            this.nudBatchSize.Maximum = new decimal(new int[] {
            32,
            0,
            0,
            0});
            this.nudBatchSize.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudBatchSize.Name = "nudBatchSize";
            this.nudBatchSize.Size = new System.Drawing.Size(70, 25);
            this.nudBatchSize.TabIndex = 1;
            this.nudBatchSize.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // lblTiling
            // 
            this.lblTiling.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblTiling.AutoSize = true;
            this.lblTiling.Location = new System.Drawing.Point(3, 40);
            this.lblTiling.Name = "lblTiling";
            this.lblTiling.Size = new System.Drawing.Size(67, 15);
            this.lblTiling.TabIndex = 2;
            this.lblTiling.Text = "分块检测:";
            // 
            // chkTiling
            // 
            this.chkTiling.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.chkTiling.AutoSize = true;
            this.chkTiling.Checked = true;
            this.chkTiling.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkTiling.Location = new System.Drawing.Point(78, 38);
            this.chkTiling.Name = "chkTiling";
            this.chkTiling.Size = new System.Drawing.Size(56, 19);
            this.chkTiling.TabIndex = 3;
            this.chkTiling.Text = "启用";
            this.chkTiling.UseVisualStyleBackColor = true;
            // 
            // lblOverlap
            // 
            this.lblOverlap.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblOverlap.AutoSize = true;
            this.lblOverlap.Location = new System.Drawing.Point(158, 40);
            this.lblOverlap.Name = "lblOverlap";
            this.lblOverlap.Size = new System.Drawing.Size(67, 15);
            this.lblOverlap.TabIndex = 4;
            this.lblOverlap.Text = "重叠像素:";
            // 
            // nudOverlap
            // 
            this.nudOverlap.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.nudOverlap.Location = new System.Drawing.Point(233, 36);
            this.nudOverlap.Maximum = new decimal(new int[] {
            200,
            0,
            0,
            0});
            this.nudOverlap.Name = "nudOverlap";
            this.nudOverlap.Size = new System.Drawing.Size(70, 25);
            this.nudOverlap.TabIndex = 5;
            this.nudOverlap.Value = new decimal(new int[] {
            60,
            0,
            0,
            0});
            // 
            // panelButtons
            // 
            this.panelButtons.Controls.Add(this.btnUnload);
            this.panelButtons.Controls.Add(this.btnLoad);
            this.panelButtons.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelButtons.Location = new System.Drawing.Point(93, 291);
            this.panelButtons.Name = "panelButtons";
            this.panelButtons.Size = new System.Drawing.Size(284, 39);
            this.panelButtons.TabIndex = 12;
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
            // groupBoxOptimize
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.groupBoxOptimize, 2);
            this.groupBoxOptimize.Controls.Add(this.tableLayoutPanelOptimize);
            this.groupBoxOptimize.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxOptimize.Location = new System.Drawing.Point(3, 336);
            this.groupBoxOptimize.Name = "groupBoxOptimize";
            this.groupBoxOptimize.Padding = new System.Windows.Forms.Padding(6);
            this.groupBoxOptimize.Size = new System.Drawing.Size(374, 154);
            this.groupBoxOptimize.TabIndex = 13;
            this.groupBoxOptimize.TabStop = false;
            this.groupBoxOptimize.Text = "模型优化导出";
            // 
            // tableLayoutPanelOptimize
            // 
            this.tableLayoutPanelOptimize.ColumnCount = 4;
            this.tableLayoutPanelOptimize.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 75F));
            this.tableLayoutPanelOptimize.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanelOptimize.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60F));
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
            this.tableLayoutPanelOptimize.Location = new System.Drawing.Point(6, 22);
            this.tableLayoutPanelOptimize.Name = "tableLayoutPanelOptimize";
            this.tableLayoutPanelOptimize.RowCount = 5;
            this.tableLayoutPanelOptimize.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tableLayoutPanelOptimize.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tableLayoutPanelOptimize.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanelOptimize.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanelOptimize.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelOptimize.Size = new System.Drawing.Size(362, 126);
            this.tableLayoutPanelOptimize.TabIndex = 0;
            // 
            // lblOptimizeDevice
            // 
            this.lblOptimizeDevice.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblOptimizeDevice.AutoSize = true;
            this.lblOptimizeDevice.Location = new System.Drawing.Point(3, 6);
            this.lblOptimizeDevice.Name = "lblOptimizeDevice";
            this.lblOptimizeDevice.Size = new System.Drawing.Size(43, 15);
            this.lblOptimizeDevice.TabIndex = 0;
            this.lblOptimizeDevice.Text = "设备:";
            // 
            // cmbOptimizeDevice
            // 
            this.tableLayoutPanelOptimize.SetColumnSpan(this.cmbOptimizeDevice, 3);
            this.cmbOptimizeDevice.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbOptimizeDevice.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbOptimizeDevice.FormattingEnabled = true;
            this.cmbOptimizeDevice.Location = new System.Drawing.Point(78, 3);
            this.cmbOptimizeDevice.Name = "cmbOptimizeDevice";
            this.cmbOptimizeDevice.Size = new System.Drawing.Size(281, 23);
            this.cmbOptimizeDevice.TabIndex = 1;
            // 
            // lblOptimizePrecision
            // 
            this.lblOptimizePrecision.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblOptimizePrecision.AutoSize = true;
            this.lblOptimizePrecision.Location = new System.Drawing.Point(3, 34);
            this.lblOptimizePrecision.Name = "lblOptimizePrecision";
            this.lblOptimizePrecision.Size = new System.Drawing.Size(55, 15);
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
            this.cmbOptimizePrecision.Location = new System.Drawing.Point(78, 31);
            this.cmbOptimizePrecision.Name = "cmbOptimizePrecision";
            this.cmbOptimizePrecision.Size = new System.Drawing.Size(100, 23);
            this.cmbOptimizePrecision.TabIndex = 3;
            // 
            // lblOptimizeBatchSize
            // 
            this.lblOptimizeBatchSize.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblOptimizeBatchSize.AutoSize = true;
            this.lblOptimizeBatchSize.Location = new System.Drawing.Point(191, 34);
            this.lblOptimizeBatchSize.Name = "lblOptimizeBatchSize";
            this.lblOptimizeBatchSize.Size = new System.Drawing.Size(55, 15);
            this.lblOptimizeBatchSize.TabIndex = 4;
            this.lblOptimizeBatchSize.Text = "批次:";
            // 
            // nudOptimizeBatchSize
            // 
            this.nudOptimizeBatchSize.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.nudOptimizeBatchSize.Location = new System.Drawing.Point(251, 31);
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
            this.nudOptimizeBatchSize.Size = new System.Drawing.Size(60, 25);
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
            this.lblOptimizeStatusLabel.Location = new System.Drawing.Point(3, 61);
            this.lblOptimizeStatusLabel.Name = "lblOptimizeStatusLabel";
            this.lblOptimizeStatusLabel.Size = new System.Drawing.Size(43, 15);
            this.lblOptimizeStatusLabel.TabIndex = 6;
            this.lblOptimizeStatusLabel.Text = "状态:";
            // 
            // lblOptimizeStatus
            // 
            this.tableLayoutPanelOptimize.SetColumnSpan(this.lblOptimizeStatus, 3);
            this.lblOptimizeStatus.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblOptimizeStatus.AutoSize = true;
            this.lblOptimizeStatus.ForeColor = System.Drawing.Color.Gray;
            this.lblOptimizeStatus.Location = new System.Drawing.Point(78, 61);
            this.lblOptimizeStatus.Name = "lblOptimizeStatus";
            this.lblOptimizeStatus.Size = new System.Drawing.Size(15, 15);
            this.lblOptimizeStatus.TabIndex = 7;
            this.lblOptimizeStatus.Text = "-";
            // 
            // btnExportOptimized
            // 
            this.tableLayoutPanelOptimize.SetColumnSpan(this.btnExportOptimized, 3);
            this.btnExportOptimized.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.btnExportOptimized.Location = new System.Drawing.Point(78, 84);
            this.btnExportOptimized.Name = "btnExportOptimized";
            this.btnExportOptimized.Size = new System.Drawing.Size(120, 28);
            this.btnExportOptimized.TabIndex = 8;
            this.btnExportOptimized.Text = "导出优化模型";
            this.btnExportOptimized.UseVisualStyleBackColor = true;
            // 
            // progressBarOptimize
            // 
            this.tableLayoutPanelOptimize.SetColumnSpan(this.progressBarOptimize, 3);
            this.progressBarOptimize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBarOptimize.Location = new System.Drawing.Point(78, 119);
            this.progressBarOptimize.Name = "progressBarOptimize";
            this.progressBarOptimize.Size = new System.Drawing.Size(281, 4);
            this.progressBarOptimize.TabIndex = 9;
            this.progressBarOptimize.Visible = false;
            // 
            // OCRConfigControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.MinimumSize = new System.Drawing.Size(350, 550);
            this.Name = "OCRConfigControl";
            this.Padding = new System.Windows.Forms.Padding(10);
            this.Size = new System.Drawing.Size(400, 550);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.panelPath.ResumeLayout(false);
            this.panelPath.PerformLayout();
            this.groupBoxOCR.ResumeLayout(false);
            this.tableLayoutPanelOCR.ResumeLayout(false);
            this.tableLayoutPanelOCR.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudBatchSize)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudOverlap)).EndInit();
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
        private System.Windows.Forms.CheckBox chkLoadOnStartup;
        private System.Windows.Forms.GroupBox groupBoxOCR;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelOCR;
        private System.Windows.Forms.Label lblBatchSize;
        private System.Windows.Forms.NumericUpDown nudBatchSize;
        private System.Windows.Forms.Label lblTiling;
        private System.Windows.Forms.CheckBox chkTiling;
        private System.Windows.Forms.Label lblOverlap;
        private System.Windows.Forms.NumericUpDown nudOverlap;
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