namespace Vision.Frm.MainForm
{
    partial class Frm_Camera2D
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.split_Display = new System.Windows.Forms.SplitContainer();
            this.tablePanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.pnl_DevInfo = new System.Windows.Forms.Panel();
            this.btn_Remove = new System.Windows.Forms.Button();
            this.btn_Add = new System.Windows.Forms.Button();
            this.labelControl10 = new System.Windows.Forms.Label();
            this.cmb_SnList = new System.Windows.Forms.ComboBox();
            this.labelControl6 = new System.Windows.Forms.Label();
            this.cmb_Manufacturers = new System.Windows.Forms.ComboBox();
            this.panelControl3 = new System.Windows.Forms.Panel();
            this.dgv_CameraConfig = new System.Windows.Forms.DataGridView();
            this.col_SerialNumber = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col_Manufacturer = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col_Expain = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.panelControl2 = new System.Windows.Forms.Panel();
            this.cmb_TriggerSource = new System.Windows.Forms.ComboBox();
            this.txt_MaxExposure = new System.Windows.Forms.Label();
            this.labelControl9 = new System.Windows.Forms.Label();
            this.labelControl8 = new System.Windows.Forms.Label();
            this.labelControl7 = new System.Windows.Forms.Label();
            this.chk_HardTrigger = new System.Windows.Forms.CheckBox();
            this.txt_MaxGain = new System.Windows.Forms.Label();
            this.labelControl5 = new System.Windows.Forms.Label();
            this.txt_Exposure = new System.Windows.Forms.NumericUpDown();
            this.labelControl4 = new System.Windows.Forms.Label();
            this.txt_Gain = new System.Windows.Forms.NumericUpDown();
            this.labelControl1 = new System.Windows.Forms.Label();
            this.labelControl3 = new System.Windows.Forms.Label();
            this.labelControl2 = new System.Windows.Forms.Label();
            this.panelControl1 = new System.Windows.Forms.Panel();
            this.btn_Continuous = new System.Windows.Forms.Button();
            this.btn_TriggerOnce = new System.Windows.Forms.Button();
            this.btn_DisConnect = new System.Windows.Forms.Button();
            this.btn_Connect = new System.Windows.Forms.Button();
            this.pictureEdit_Display = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.split_Display)).BeginInit();
            this.split_Display.Panel1.SuspendLayout();
            this.split_Display.Panel2.SuspendLayout();
            this.split_Display.SuspendLayout();
            this.tablePanel1.SuspendLayout();
            this.pnl_DevInfo.SuspendLayout();
            this.panelControl3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_CameraConfig)).BeginInit();
            this.panelControl2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txt_Exposure)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txt_Gain)).BeginInit();
            this.panelControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureEdit_Display)).BeginInit();
            this.SuspendLayout();
            // 
            // split_Display
            // 
            this.split_Display.Dock = System.Windows.Forms.DockStyle.Fill;
            this.split_Display.Location = new System.Drawing.Point(0, 0);
            this.split_Display.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.split_Display.Name = "split_Display";
            // 
            // split_Display.Panel1
            // 
            this.split_Display.Panel1.Controls.Add(this.tablePanel1);
            // 
            // split_Display.Panel2
            // 
            this.split_Display.Panel2.Controls.Add(this.pictureEdit_Display);
            this.split_Display.Size = new System.Drawing.Size(1212, 524);
            this.split_Display.SplitterDistance = 709;
            this.split_Display.TabIndex = 2;
            // 
            // tablePanel1
            // 
            this.tablePanel1.AutoSize = true;
            this.tablePanel1.ColumnCount = 1;
            this.tablePanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tablePanel1.Controls.Add(this.pnl_DevInfo, 0, 0);
            this.tablePanel1.Controls.Add(this.panelControl3, 0, 1);
            this.tablePanel1.Controls.Add(this.panelControl2, 0, 2);
            this.tablePanel1.Controls.Add(this.panelControl1, 0, 3);
            this.tablePanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tablePanel1.Location = new System.Drawing.Point(0, 0);
            this.tablePanel1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tablePanel1.Name = "tablePanel1";
            this.tablePanel1.RowCount = 4;
            this.tablePanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 106F));
            this.tablePanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 169F));
            this.tablePanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 183F));
            this.tablePanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tablePanel1.Size = new System.Drawing.Size(709, 524);
            this.tablePanel1.TabIndex = 0;
            // 
            // pnl_DevInfo
            // 
            this.pnl_DevInfo.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.pnl_DevInfo.Controls.Add(this.btn_Remove);
            this.pnl_DevInfo.Controls.Add(this.btn_Add);
            this.pnl_DevInfo.Controls.Add(this.labelControl10);
            this.pnl_DevInfo.Controls.Add(this.cmb_SnList);
            this.pnl_DevInfo.Controls.Add(this.labelControl6);
            this.pnl_DevInfo.Controls.Add(this.cmb_Manufacturers);
            this.pnl_DevInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnl_DevInfo.Location = new System.Drawing.Point(3, 2);
            this.pnl_DevInfo.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.pnl_DevInfo.Name = "pnl_DevInfo";
            this.pnl_DevInfo.Size = new System.Drawing.Size(703, 102);
            this.pnl_DevInfo.TabIndex = 3;
            // 
            // btn_Remove
            // 
            this.btn_Remove.Location = new System.Drawing.Point(372, 51);
            this.btn_Remove.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btn_Remove.Name = "btn_Remove";
            this.btn_Remove.Size = new System.Drawing.Size(36, 28);
            this.btn_Remove.TabIndex = 10;
            this.btn_Remove.Text = "-";
            this.btn_Remove.UseVisualStyleBackColor = true;
            this.btn_Remove.Click += new System.EventHandler(this.btn_Remove_Click);
            // 
            // btn_Add
            // 
            this.btn_Add.Location = new System.Drawing.Point(322, 51);
            this.btn_Add.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btn_Add.Name = "btn_Add";
            this.btn_Add.Size = new System.Drawing.Size(36, 28);
            this.btn_Add.TabIndex = 9;
            this.btn_Add.Text = "+";
            this.btn_Add.UseVisualStyleBackColor = true;
            this.btn_Add.Click += new System.EventHandler(this.btn_Add_Click);
            // 
            // labelControl10
            // 
            this.labelControl10.AutoSize = true;
            this.labelControl10.Location = new System.Drawing.Point(11, 56);
            this.labelControl10.Name = "labelControl10";
            this.labelControl10.Size = new System.Drawing.Size(80, 18);
            this.labelControl10.TabIndex = 7;
            this.labelControl10.Text = "相机列表";
            // 
            // cmb_SnList
            // 
            this.cmb_SnList.FormattingEnabled = true;
            this.cmb_SnList.Location = new System.Drawing.Point(92, 54);
            this.cmb_SnList.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.cmb_SnList.Name = "cmb_SnList";
            this.cmb_SnList.Size = new System.Drawing.Size(215, 26);
            this.cmb_SnList.TabIndex = 8;
            this.cmb_SnList.SelectedIndexChanged += new System.EventHandler(this.cmb_SnList_SelectedIndexChanged);
            // 
            // labelControl6
            // 
            this.labelControl6.AutoSize = true;
            this.labelControl6.Location = new System.Drawing.Point(11, 15);
            this.labelControl6.Name = "labelControl6";
            this.labelControl6.Size = new System.Drawing.Size(80, 18);
            this.labelControl6.TabIndex = 5;
            this.labelControl6.Text = "相机品牌";
            // 
            // cmb_Manufacturers
            // 
            this.cmb_Manufacturers.FormattingEnabled = true;
            this.cmb_Manufacturers.Location = new System.Drawing.Point(92, 12);
            this.cmb_Manufacturers.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.cmb_Manufacturers.Name = "cmb_Manufacturers";
            this.cmb_Manufacturers.Size = new System.Drawing.Size(215, 26);
            this.cmb_Manufacturers.TabIndex = 6;
            this.cmb_Manufacturers.SelectedIndexChanged += new System.EventHandler(this.cmb_Manufacturers_SelectedIndexChanged);
            // 
            // panelControl3
            // 
            this.panelControl3.Controls.Add(this.dgv_CameraConfig);
            this.panelControl3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelControl3.Location = new System.Drawing.Point(3, 108);
            this.panelControl3.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.panelControl3.Name = "panelControl3";
            this.panelControl3.Size = new System.Drawing.Size(703, 165);
            this.panelControl3.TabIndex = 25;
            // 
            // dgv_CameraConfig
            // 
            this.dgv_CameraConfig.AllowUserToAddRows = false;
            this.dgv_CameraConfig.AllowUserToDeleteRows = false;
            this.dgv_CameraConfig.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv_CameraConfig.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.col_SerialNumber,
            this.col_Manufacturer,
            this.col_Expain});
            this.dgv_CameraConfig.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgv_CameraConfig.Location = new System.Drawing.Point(0, 0);
            this.dgv_CameraConfig.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.dgv_CameraConfig.Name = "dgv_CameraConfig";
            this.dgv_CameraConfig.RowHeadersVisible = false;
            this.dgv_CameraConfig.RowHeadersWidth = 62;
            this.dgv_CameraConfig.RowTemplate.Height = 30;
            this.dgv_CameraConfig.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgv_CameraConfig.Size = new System.Drawing.Size(703, 165);
            this.dgv_CameraConfig.TabIndex = 0;
            this.dgv_CameraConfig.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgv_CameraConfig_CellValueChanged);
            // 
            // col_SerialNumber
            // 
            this.col_SerialNumber.DataPropertyName = "SerialNumber";
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.White;
            this.col_SerialNumber.DefaultCellStyle = dataGridViewCellStyle1;
            this.col_SerialNumber.HeaderText = "序列号";
            this.col_SerialNumber.MinimumWidth = 8;
            this.col_SerialNumber.Name = "col_SerialNumber";
            this.col_SerialNumber.ReadOnly = true;
            this.col_SerialNumber.Width = 150;
            // 
            // col_Manufacturer
            // 
            this.col_Manufacturer.DataPropertyName = "Manufacturer";
            this.col_Manufacturer.HeaderText = "厂商";
            this.col_Manufacturer.MinimumWidth = 8;
            this.col_Manufacturer.Name = "col_Manufacturer";
            this.col_Manufacturer.ReadOnly = true;
            this.col_Manufacturer.Width = 150;
            // 
            // col_Expain
            // 
            this.col_Expain.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.col_Expain.DataPropertyName = "Expain";
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.White;
            this.col_Expain.DefaultCellStyle = dataGridViewCellStyle2;
            this.col_Expain.HeaderText = "备注";
            this.col_Expain.MinimumWidth = 8;
            this.col_Expain.Name = "col_Expain";
            // 
            // panelControl2
            // 
            this.panelControl2.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.panelControl2.Controls.Add(this.cmb_TriggerSource);
            this.panelControl2.Controls.Add(this.txt_MaxExposure);
            this.panelControl2.Controls.Add(this.labelControl9);
            this.panelControl2.Controls.Add(this.labelControl8);
            this.panelControl2.Controls.Add(this.labelControl7);
            this.panelControl2.Controls.Add(this.chk_HardTrigger);
            this.panelControl2.Controls.Add(this.txt_MaxGain);
            this.panelControl2.Controls.Add(this.labelControl5);
            this.panelControl2.Controls.Add(this.txt_Exposure);
            this.panelControl2.Controls.Add(this.labelControl4);
            this.panelControl2.Controls.Add(this.txt_Gain);
            this.panelControl2.Controls.Add(this.labelControl1);
            this.panelControl2.Controls.Add(this.labelControl3);
            this.panelControl2.Controls.Add(this.labelControl2);
            this.panelControl2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelControl2.Location = new System.Drawing.Point(3, 277);
            this.panelControl2.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.panelControl2.Name = "panelControl2";
            this.panelControl2.Size = new System.Drawing.Size(703, 179);
            this.panelControl2.TabIndex = 13;
            // 
            // cmb_TriggerSource
            // 
            this.cmb_TriggerSource.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmb_TriggerSource.FormattingEnabled = true;
            this.cmb_TriggerSource.Location = new System.Drawing.Point(181, 45);
            this.cmb_TriggerSource.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.cmb_TriggerSource.Name = "cmb_TriggerSource";
            this.cmb_TriggerSource.Size = new System.Drawing.Size(126, 26);
            this.cmb_TriggerSource.TabIndex = 19;
            this.cmb_TriggerSource.SelectedIndexChanged += new System.EventHandler(this.cmb_TriggerSource_SelectedIndexChanged);
            // 
            // txt_MaxExposure
            // 
            this.txt_MaxExposure.AutoSize = true;
            this.txt_MaxExposure.Location = new System.Drawing.Point(399, 97);
            this.txt_MaxExposure.Name = "txt_MaxExposure";
            this.txt_MaxExposure.Size = new System.Drawing.Size(17, 18);
            this.txt_MaxExposure.TabIndex = 18;
            this.txt_MaxExposure.Text = "0";
            // 
            // labelControl9
            // 
            this.labelControl9.AutoSize = true;
            this.labelControl9.Location = new System.Drawing.Point(313, 97);
            this.labelControl9.Name = "labelControl9";
            this.labelControl9.Size = new System.Drawing.Size(89, 18);
            this.labelControl9.TabIndex = 17;
            this.labelControl9.Text = "最大曝光:";
            // 
            // labelControl8
            // 
            this.labelControl8.AutoSize = true;
            this.labelControl8.Location = new System.Drawing.Point(211, 97);
            this.labelControl8.Name = "labelControl8";
            this.labelControl8.Size = new System.Drawing.Size(26, 18);
            this.labelControl8.TabIndex = 16;
            this.labelControl8.Text = "us";
            // 
            // labelControl7
            // 
            this.labelControl7.AutoSize = true;
            this.labelControl7.Location = new System.Drawing.Point(211, 133);
            this.labelControl7.Name = "labelControl7";
            this.labelControl7.Size = new System.Drawing.Size(26, 18);
            this.labelControl7.TabIndex = 15;
            this.labelControl7.Text = "db";
            // 
            // chk_HardTrigger
            // 
            this.chk_HardTrigger.AutoSize = true;
            this.chk_HardTrigger.Location = new System.Drawing.Point(92, 47);
            this.chk_HardTrigger.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.chk_HardTrigger.Name = "chk_HardTrigger";
            this.chk_HardTrigger.Size = new System.Drawing.Size(88, 22);
            this.chk_HardTrigger.TabIndex = 14;
            this.chk_HardTrigger.Text = "硬触发";
            this.chk_HardTrigger.UseVisualStyleBackColor = true;
            this.chk_HardTrigger.CheckedChanged += new System.EventHandler(this.chk_HardTrigger_CheckedChanged);
            // 
            // txt_MaxGain
            // 
            this.txt_MaxGain.AutoSize = true;
            this.txt_MaxGain.Location = new System.Drawing.Point(399, 133);
            this.txt_MaxGain.Name = "txt_MaxGain";
            this.txt_MaxGain.Size = new System.Drawing.Size(17, 18);
            this.txt_MaxGain.TabIndex = 12;
            this.txt_MaxGain.Text = "0";
            // 
            // labelControl5
            // 
            this.labelControl5.AutoSize = true;
            this.labelControl5.Location = new System.Drawing.Point(313, 133);
            this.labelControl5.Name = "labelControl5";
            this.labelControl5.Size = new System.Drawing.Size(89, 18);
            this.labelControl5.TabIndex = 11;
            this.labelControl5.Text = "最大增益:";
            // 
            // txt_Exposure
            // 
            this.txt_Exposure.DecimalPlaces = 3;
            this.txt_Exposure.Location = new System.Drawing.Point(92, 96);
            this.txt_Exposure.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txt_Exposure.Maximum = new decimal(new int[] {
            10000000,
            0,
            0,
            0});
            this.txt_Exposure.Name = "txt_Exposure";
            this.txt_Exposure.Size = new System.Drawing.Size(111, 28);
            this.txt_Exposure.TabIndex = 7;
            this.txt_Exposure.ValueChanged += new System.EventHandler(this.txt_Exposure_ValueChanged);
            // 
            // labelControl4
            // 
            this.labelControl4.AutoSize = true;
            this.labelControl4.Location = new System.Drawing.Point(35, 133);
            this.labelControl4.Name = "labelControl4";
            this.labelControl4.Size = new System.Drawing.Size(44, 18);
            this.labelControl4.TabIndex = 10;
            this.labelControl4.Text = "增益";
            // 
            // txt_Gain
            // 
            this.txt_Gain.DecimalPlaces = 3;
            this.txt_Gain.Location = new System.Drawing.Point(92, 132);
            this.txt_Gain.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txt_Gain.Name = "txt_Gain";
            this.txt_Gain.Size = new System.Drawing.Size(111, 28);
            this.txt_Gain.TabIndex = 9;
            this.txt_Gain.ValueChanged += new System.EventHandler(this.txt_Gain_ValueChanged);
            // 
            // labelControl1
            // 
            this.labelControl1.AutoSize = true;
            this.labelControl1.Location = new System.Drawing.Point(7, 13);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(62, 18);
            this.labelControl1.TabIndex = 1;
            this.labelControl1.Text = "序列号";
            // 
            // labelControl3
            // 
            this.labelControl3.AutoSize = true;
            this.labelControl3.Location = new System.Drawing.Point(35, 97);
            this.labelControl3.Name = "labelControl3";
            this.labelControl3.Size = new System.Drawing.Size(44, 18);
            this.labelControl3.TabIndex = 8;
            this.labelControl3.Text = "曝光";
            // 
            // labelControl2
            // 
            this.labelControl2.AutoSize = true;
            this.labelControl2.Location = new System.Drawing.Point(7, 50);
            this.labelControl2.Name = "labelControl2";
            this.labelControl2.Size = new System.Drawing.Size(80, 18);
            this.labelControl2.TabIndex = 3;
            this.labelControl2.Text = "触发模式";
            // 
            // panelControl1
            // 
            this.panelControl1.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.panelControl1.Controls.Add(this.btn_Continuous);
            this.panelControl1.Controls.Add(this.btn_TriggerOnce);
            this.panelControl1.Controls.Add(this.btn_DisConnect);
            this.panelControl1.Controls.Add(this.btn_Connect);
            this.panelControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelControl1.Location = new System.Drawing.Point(3, 460);
            this.panelControl1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Size = new System.Drawing.Size(703, 62);
            this.panelControl1.TabIndex = 14;
            // 
            // btn_Continuous
            // 
            this.btn_Continuous.Location = new System.Drawing.Point(396, 17);
            this.btn_Continuous.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btn_Continuous.Name = "btn_Continuous";
            this.btn_Continuous.Size = new System.Drawing.Size(100, 30);
            this.btn_Continuous.TabIndex = 3;
            this.btn_Continuous.Text = "连续采集";
            this.btn_Continuous.UseVisualStyleBackColor = true;
            this.btn_Continuous.Click += new System.EventHandler(this.btn_Continuous_Click);
            // 
            // btn_TriggerOnce
            // 
            this.btn_TriggerOnce.Location = new System.Drawing.Point(264, 17);
            this.btn_TriggerOnce.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btn_TriggerOnce.Name = "btn_TriggerOnce";
            this.btn_TriggerOnce.Size = new System.Drawing.Size(115, 30);
            this.btn_TriggerOnce.TabIndex = 2;
            this.btn_TriggerOnce.Text = "软触发一次";
            this.btn_TriggerOnce.UseVisualStyleBackColor = true;
            this.btn_TriggerOnce.Click += new System.EventHandler(this.btn_TriggerOnce_Click);
            // 
            // btn_DisConnect
            // 
            this.btn_DisConnect.Location = new System.Drawing.Point(141, 17);
            this.btn_DisConnect.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btn_DisConnect.Name = "btn_DisConnect";
            this.btn_DisConnect.Size = new System.Drawing.Size(100, 30);
            this.btn_DisConnect.TabIndex = 1;
            this.btn_DisConnect.Text = "断开";
            this.btn_DisConnect.UseVisualStyleBackColor = true;
            this.btn_DisConnect.Click += new System.EventHandler(this.btn_DisConnect_Click);
            // 
            // btn_Connect
            // 
            this.btn_Connect.Location = new System.Drawing.Point(11, 17);
            this.btn_Connect.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btn_Connect.Name = "btn_Connect";
            this.btn_Connect.Size = new System.Drawing.Size(100, 30);
            this.btn_Connect.TabIndex = 0;
            this.btn_Connect.Text = "连接";
            this.btn_Connect.UseVisualStyleBackColor = true;
            this.btn_Connect.Click += new System.EventHandler(this.btn_Connect_Click);
            // 
            // pictureEdit_Display
            // 
            this.pictureEdit_Display.BackColor = System.Drawing.Color.Black;
            this.pictureEdit_Display.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureEdit_Display.Location = new System.Drawing.Point(0, 0);
            this.pictureEdit_Display.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.pictureEdit_Display.Name = "pictureEdit_Display";
            this.pictureEdit_Display.Size = new System.Drawing.Size(499, 524);
            this.pictureEdit_Display.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureEdit_Display.TabIndex = 0;
            this.pictureEdit_Display.TabStop = false;
            // 
            // Frm_Camera2D
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1212, 524);
            this.Controls.Add(this.split_Display);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.MaximizeBox = false;
            this.Name = "Frm_Camera2D";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "相机配置界面";
            this.Load += new System.EventHandler(this.Frm_Camera2D_Load);
            this.split_Display.Panel1.ResumeLayout(false);
            this.split_Display.Panel1.PerformLayout();
            this.split_Display.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.split_Display)).EndInit();
            this.split_Display.ResumeLayout(false);
            this.tablePanel1.ResumeLayout(false);
            this.pnl_DevInfo.ResumeLayout(false);
            this.pnl_DevInfo.PerformLayout();
            this.panelControl3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgv_CameraConfig)).EndInit();
            this.panelControl2.ResumeLayout(false);
            this.panelControl2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txt_Exposure)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txt_Gain)).EndInit();
            this.panelControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureEdit_Display)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer split_Display;
        private System.Windows.Forms.TableLayoutPanel tablePanel1;
        private System.Windows.Forms.Panel panelControl1;
        private System.Windows.Forms.Button btn_Continuous;
        private System.Windows.Forms.Button btn_TriggerOnce;
        private System.Windows.Forms.Button btn_DisConnect;
        private System.Windows.Forms.Button btn_Connect;
        private System.Windows.Forms.Panel panelControl3;
        private System.Windows.Forms.Panel panelControl2;
        private System.Windows.Forms.ComboBox cmb_TriggerSource;
        private System.Windows.Forms.Label txt_MaxExposure;
        private System.Windows.Forms.Label labelControl9;
        private System.Windows.Forms.Label labelControl8;
        private System.Windows.Forms.Label labelControl7;
        private System.Windows.Forms.CheckBox chk_HardTrigger;
        private System.Windows.Forms.Label txt_MaxGain;
        private System.Windows.Forms.Label labelControl5;
        private System.Windows.Forms.NumericUpDown txt_Exposure;
        private System.Windows.Forms.Label labelControl4;
        private System.Windows.Forms.NumericUpDown txt_Gain;
        private System.Windows.Forms.Label labelControl1;
        private System.Windows.Forms.Label labelControl3;
        private System.Windows.Forms.Label labelControl2;
        private System.Windows.Forms.Panel pnl_DevInfo;
        private System.Windows.Forms.Label labelControl10;
        private System.Windows.Forms.ComboBox cmb_SnList;
        private System.Windows.Forms.Label labelControl6;
        private System.Windows.Forms.ComboBox cmb_Manufacturers;
        private System.Windows.Forms.PictureBox pictureEdit_Display;
        private System.Windows.Forms.DataGridView dgv_CameraConfig;
        private System.Windows.Forms.DataGridViewTextBoxColumn col_SerialNumber;
        private System.Windows.Forms.DataGridViewTextBoxColumn col_Manufacturer;
        private System.Windows.Forms.DataGridViewTextBoxColumn col_Expain;
        private System.Windows.Forms.Button btn_Add;
        private System.Windows.Forms.Button btn_Remove;
    }
}