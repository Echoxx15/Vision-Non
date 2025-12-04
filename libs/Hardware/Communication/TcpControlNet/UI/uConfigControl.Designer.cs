namespace TcpControlNet.UI
{
	partial class uConfigControl
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
            this.splitContainer_Main = new System.Windows.Forms.SplitContainer();
            this.panel_Test = new System.Windows.Forms.Panel();
            this.splitContainer_Test = new System.Windows.Forms.SplitContainer();
            this.panel_Receive = new System.Windows.Forms.Panel();
            this.txt_Receive = new System.Windows.Forms.TextBox();
            this.panel_ReceiveHeader = new System.Windows.Forms.Panel();
            this.btn_ClearReceive = new System.Windows.Forms.Button();
            this.lbl_Receive = new System.Windows.Forms.Label();
            this.panel_Send = new System.Windows.Forms.Panel();
            this.txt_Send = new System.Windows.Forms.TextBox();
            this.panel_SendHeader = new System.Windows.Forms.Panel();
            this.btn_Send = new System.Windows.Forms.Button();
            this.lbl_Send = new System.Windows.Forms.Label();
            this.groupBox_Config = new System.Windows.Forms.GroupBox();
            this.txt_Remark = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.btn_Save = new System.Windows.Forms.Button();
            this.num_MaxConnections = new System.Windows.Forms.NumericUpDown();
            this.label11 = new System.Windows.Forms.Label();
            this.num_ReconnectInterval = new System.Windows.Forms.NumericUpDown();
            this.label10 = new System.Windows.Forms.Label();
            this.chk_AutoReconnect = new System.Windows.Forms.CheckBox();
            this.num_ConnectTimeout = new System.Windows.Forms.NumericUpDown();
            this.label9 = new System.Windows.Forms.Label();
            this.txt_Terminator = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.chk_UseTerminator = new System.Windows.Forms.CheckBox();
            this.cmb_Encoding = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.num_ReceiveBufferSize = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.num_SendBufferSize = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.chk_Enabled = new System.Windows.Forms.CheckBox();
            this.num_Port = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.txt_IpAddress = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.lbl_Type = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txt_Name = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer_Main)).BeginInit();
            this.splitContainer_Main.Panel1.SuspendLayout();
            this.splitContainer_Main.Panel2.SuspendLayout();
            this.splitContainer_Main.SuspendLayout();
            this.panel_Test.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer_Test)).BeginInit();
            this.splitContainer_Test.Panel1.SuspendLayout();
            this.splitContainer_Test.Panel2.SuspendLayout();
            this.splitContainer_Test.SuspendLayout();
            this.panel_Receive.SuspendLayout();
            this.panel_ReceiveHeader.SuspendLayout();
            this.panel_Send.SuspendLayout();
            this.panel_SendHeader.SuspendLayout();
            this.groupBox_Config.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.num_MaxConnections)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_ReconnectInterval)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_ConnectTimeout)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_ReceiveBufferSize)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_SendBufferSize)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_Port)).BeginInit();
            this.SuspendLayout();
            // 
            // splitContainer_Main
            // 
            this.splitContainer_Main.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer_Main.Location = new System.Drawing.Point(0, 0);
            this.splitContainer_Main.Name = "splitContainer_Main";
            this.splitContainer_Main.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer_Main.Panel1
            // 
            this.splitContainer_Main.Panel1.Controls.Add(this.panel_Test);
            this.splitContainer_Main.Panel1MinSize = 150;
            // 
            // splitContainer_Main.Panel2
            // 
            this.splitContainer_Main.Panel2.Controls.Add(this.groupBox_Config);
            this.splitContainer_Main.Size = new System.Drawing.Size(794, 854);
            this.splitContainer_Main.SplitterDistance = 261;
            this.splitContainer_Main.TabIndex = 3;
            // 
            // panel_Test
            // 
            this.panel_Test.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.panel_Test.Controls.Add(this.splitContainer_Test);
            this.panel_Test.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel_Test.Location = new System.Drawing.Point(0, 0);
            this.panel_Test.Name = "panel_Test";
            this.panel_Test.Padding = new System.Windows.Forms.Padding(5);
            this.panel_Test.Size = new System.Drawing.Size(794, 261);
            this.panel_Test.TabIndex = 0;
            // 
            // splitContainer_Test
            // 
            this.splitContainer_Test.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer_Test.Location = new System.Drawing.Point(5, 5);
            this.splitContainer_Test.Name = "splitContainer_Test";
            this.splitContainer_Test.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer_Test.Panel1
            // 
            this.splitContainer_Test.Panel1.Controls.Add(this.panel_Receive);
            this.splitContainer_Test.Panel1MinSize = 60;
            // 
            // splitContainer_Test.Panel2
            // 
            this.splitContainer_Test.Panel2.Controls.Add(this.panel_Send);
            this.splitContainer_Test.Panel2MinSize = 50;
            this.splitContainer_Test.Size = new System.Drawing.Size(784, 251);
            this.splitContainer_Test.SplitterDistance = 132;
            this.splitContainer_Test.TabIndex = 0;
            // 
            // panel_Receive
            // 
            this.panel_Receive.Controls.Add(this.txt_Receive);
            this.panel_Receive.Controls.Add(this.panel_ReceiveHeader);
            this.panel_Receive.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel_Receive.Location = new System.Drawing.Point(0, 0);
            this.panel_Receive.Name = "panel_Receive";
            this.panel_Receive.Size = new System.Drawing.Size(784, 132);
            this.panel_Receive.TabIndex = 0;
            // 
            // txt_Receive
            // 
            this.txt_Receive.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.txt_Receive.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txt_Receive.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txt_Receive.Font = new System.Drawing.Font("Consolas", 9F);
            this.txt_Receive.ForeColor = System.Drawing.Color.LightGreen;
            this.txt_Receive.Location = new System.Drawing.Point(0, 32);
            this.txt_Receive.Multiline = true;
            this.txt_Receive.Name = "txt_Receive";
            this.txt_Receive.ReadOnly = true;
            this.txt_Receive.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txt_Receive.Size = new System.Drawing.Size(784, 100);
            this.txt_Receive.TabIndex = 1;
            // 
            // panel_ReceiveHeader
            // 
            this.panel_ReceiveHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(55)))), ((int)(((byte)(60)))));
            this.panel_ReceiveHeader.Controls.Add(this.btn_ClearReceive);
            this.panel_ReceiveHeader.Controls.Add(this.lbl_Receive);
            this.panel_ReceiveHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel_ReceiveHeader.Location = new System.Drawing.Point(0, 0);
            this.panel_ReceiveHeader.Name = "panel_ReceiveHeader";
            this.panel_ReceiveHeader.Size = new System.Drawing.Size(784, 32);
            this.panel_ReceiveHeader.TabIndex = 0;
            // 
            // btn_ClearReceive
            // 
            this.btn_ClearReceive.Dock = System.Windows.Forms.DockStyle.Right;
            this.btn_ClearReceive.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_ClearReceive.ForeColor = System.Drawing.Color.White;
            this.btn_ClearReceive.Location = new System.Drawing.Point(724, 0);
            this.btn_ClearReceive.Name = "btn_ClearReceive";
            this.btn_ClearReceive.Size = new System.Drawing.Size(60, 32);
            this.btn_ClearReceive.TabIndex = 1;
            this.btn_ClearReceive.Text = "清空";
            this.btn_ClearReceive.UseVisualStyleBackColor = true;
            this.btn_ClearReceive.Click += new System.EventHandler(this.btn_ClearReceive_Click);
            // 
            // lbl_Receive
            // 
            this.lbl_Receive.Dock = System.Windows.Forms.DockStyle.Left;
            this.lbl_Receive.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Bold);
            this.lbl_Receive.ForeColor = System.Drawing.Color.White;
            this.lbl_Receive.Location = new System.Drawing.Point(0, 0);
            this.lbl_Receive.Name = "lbl_Receive";
            this.lbl_Receive.Padding = new System.Windows.Forms.Padding(8, 0, 0, 0);
            this.lbl_Receive.Size = new System.Drawing.Size(80, 32);
            this.lbl_Receive.TabIndex = 0;
            this.lbl_Receive.Text = "接收";
            this.lbl_Receive.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // panel_Send
            // 
            this.panel_Send.Controls.Add(this.txt_Send);
            this.panel_Send.Controls.Add(this.panel_SendHeader);
            this.panel_Send.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel_Send.Location = new System.Drawing.Point(0, 0);
            this.panel_Send.Name = "panel_Send";
            this.panel_Send.Size = new System.Drawing.Size(784, 115);
            this.panel_Send.TabIndex = 0;
            // 
            // txt_Send
            // 
            this.txt_Send.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.txt_Send.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txt_Send.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txt_Send.Font = new System.Drawing.Font("Consolas", 9F);
            this.txt_Send.ForeColor = System.Drawing.Color.White;
            this.txt_Send.Location = new System.Drawing.Point(0, 32);
            this.txt_Send.Multiline = true;
            this.txt_Send.Name = "txt_Send";
            this.txt_Send.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txt_Send.Size = new System.Drawing.Size(784, 83);
            this.txt_Send.TabIndex = 1;
            // 
            // panel_SendHeader
            // 
            this.panel_SendHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(55)))), ((int)(((byte)(60)))));
            this.panel_SendHeader.Controls.Add(this.btn_Send);
            this.panel_SendHeader.Controls.Add(this.lbl_Send);
            this.panel_SendHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel_SendHeader.Location = new System.Drawing.Point(0, 0);
            this.panel_SendHeader.Name = "panel_SendHeader";
            this.panel_SendHeader.Size = new System.Drawing.Size(784, 32);
            this.panel_SendHeader.TabIndex = 0;
            // 
            // btn_Send
            // 
            this.btn_Send.Dock = System.Windows.Forms.DockStyle.Right;
            this.btn_Send.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_Send.ForeColor = System.Drawing.Color.White;
            this.btn_Send.Location = new System.Drawing.Point(724, 0);
            this.btn_Send.Name = "btn_Send";
            this.btn_Send.Size = new System.Drawing.Size(60, 32);
            this.btn_Send.TabIndex = 1;
            this.btn_Send.Text = "发送";
            this.btn_Send.UseVisualStyleBackColor = true;
            this.btn_Send.Click += new System.EventHandler(this.btn_Send_Click);
            // 
            // lbl_Send
            // 
            this.lbl_Send.Dock = System.Windows.Forms.DockStyle.Left;
            this.lbl_Send.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Bold);
            this.lbl_Send.ForeColor = System.Drawing.Color.White;
            this.lbl_Send.Location = new System.Drawing.Point(0, 0);
            this.lbl_Send.Name = "lbl_Send";
            this.lbl_Send.Padding = new System.Windows.Forms.Padding(8, 0, 0, 0);
            this.lbl_Send.Size = new System.Drawing.Size(80, 32);
            this.lbl_Send.TabIndex = 0;
            this.lbl_Send.Text = "发送";
            this.lbl_Send.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // groupBox_Config
            // 
            this.groupBox_Config.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.groupBox_Config.ForeColor = System.Drawing.Color.White;
            this.groupBox_Config.Controls.Add(this.btn_Save);
            this.groupBox_Config.Controls.Add(this.txt_Remark);
            this.groupBox_Config.Controls.Add(this.label12);
            this.groupBox_Config.Controls.Add(this.num_MaxConnections);
            this.groupBox_Config.Controls.Add(this.label11);
            this.groupBox_Config.Controls.Add(this.num_ReconnectInterval);
            this.groupBox_Config.Controls.Add(this.label10);
            this.groupBox_Config.Controls.Add(this.chk_AutoReconnect);
            this.groupBox_Config.Controls.Add(this.num_ConnectTimeout);
            this.groupBox_Config.Controls.Add(this.label9);
            this.groupBox_Config.Controls.Add(this.txt_Terminator);
            this.groupBox_Config.Controls.Add(this.label8);
            this.groupBox_Config.Controls.Add(this.chk_UseTerminator);
            this.groupBox_Config.Controls.Add(this.cmb_Encoding);
            this.groupBox_Config.Controls.Add(this.label7);
            this.groupBox_Config.Controls.Add(this.num_ReceiveBufferSize);
            this.groupBox_Config.Controls.Add(this.label6);
            this.groupBox_Config.Controls.Add(this.num_SendBufferSize);
            this.groupBox_Config.Controls.Add(this.label5);
            this.groupBox_Config.Controls.Add(this.chk_Enabled);
            this.groupBox_Config.Controls.Add(this.num_Port);
            this.groupBox_Config.Controls.Add(this.label4);
            this.groupBox_Config.Controls.Add(this.txt_IpAddress);
            this.groupBox_Config.Controls.Add(this.label3);
            this.groupBox_Config.Controls.Add(this.lbl_Type);
            this.groupBox_Config.Controls.Add(this.label2);
            this.groupBox_Config.Controls.Add(this.txt_Name);
            this.groupBox_Config.Controls.Add(this.label1);
            this.groupBox_Config.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox_Config.Location = new System.Drawing.Point(0, 0);
            this.groupBox_Config.Name = "groupBox_Config";
            this.groupBox_Config.Padding = new System.Windows.Forms.Padding(10);
            this.groupBox_Config.Size = new System.Drawing.Size(794, 589);
            this.groupBox_Config.TabIndex = 2;
            this.groupBox_Config.TabStop = false;
            this.groupBox_Config.Text = "配置详情";
            // 
            // txt_Remark
            // 
            this.txt_Remark.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txt_Remark.Location = new System.Drawing.Point(116, 437);
            this.txt_Remark.Multiline = true;
            this.txt_Remark.Name = "txt_Remark";
            this.txt_Remark.Size = new System.Drawing.Size(661, 76);
            this.txt_Remark.TabIndex = 26;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(9, 440);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(62, 18);
            this.label12.TabIndex = 25;
            this.label12.Text = "备注：";
            // 
            // btn_Save
            // 
            this.btn_Save.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_Save.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.btn_Save.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_Save.Font = new System.Drawing.Font("微软雅黑", 10F, System.Drawing.FontStyle.Bold);
            this.btn_Save.ForeColor = System.Drawing.Color.White;
            this.btn_Save.Location = new System.Drawing.Point(661, 520);
            this.btn_Save.Name = "btn_Save";
            this.btn_Save.Size = new System.Drawing.Size(120, 40);
            this.btn_Save.TabIndex = 27;
            this.btn_Save.Text = "保存配置";
            this.btn_Save.UseVisualStyleBackColor = false;
            this.btn_Save.Click += new System.EventHandler(this.btn_Save_Click);
            // 
            // num_MaxConnections
            // 
            this.num_MaxConnections.Location = new System.Drawing.Point(569, 397);
            this.num_MaxConnections.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.num_MaxConnections.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.num_MaxConnections.Name = "num_MaxConnections";
            this.num_MaxConnections.Size = new System.Drawing.Size(207, 28);
            this.num_MaxConnections.TabIndex = 24;
            this.num_MaxConnections.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(368, 400);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(206, 18);
            this.label11.TabIndex = 23;
            this.label11.Text = "最大连接数（服务器）：";
            // 
            // num_ReconnectInterval
            // 
            this.num_ReconnectInterval.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.num_ReconnectInterval.Location = new System.Drawing.Point(569, 357);
            this.num_ReconnectInterval.Maximum = new decimal(new int[] {
            60000,
            0,
            0,
            0});
            this.num_ReconnectInterval.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.num_ReconnectInterval.Name = "num_ReconnectInterval";
            this.num_ReconnectInterval.Size = new System.Drawing.Size(207, 28);
            this.num_ReconnectInterval.TabIndex = 22;
            this.num_ReconnectInterval.Value = new decimal(new int[] {
            3000,
            0,
            0,
            0});
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(368, 360);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(170, 18);
            this.label10.TabIndex = 21;
            this.label10.Text = "重连间隔（毫秒）：";
            // 
            // chk_AutoReconnect
            // 
            this.chk_AutoReconnect.AutoSize = true;
            this.chk_AutoReconnect.Checked = true;
            this.chk_AutoReconnect.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chk_AutoReconnect.Location = new System.Drawing.Point(116, 358);
            this.chk_AutoReconnect.Name = "chk_AutoReconnect";
            this.chk_AutoReconnect.Size = new System.Drawing.Size(196, 22);
            this.chk_AutoReconnect.TabIndex = 20;
            this.chk_AutoReconnect.Text = "自动重连（客户端）";
            this.chk_AutoReconnect.UseVisualStyleBackColor = true;
            // 
            // num_ConnectTimeout
            // 
            this.num_ConnectTimeout.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.num_ConnectTimeout.Location = new System.Drawing.Point(116, 397);
            this.num_ConnectTimeout.Maximum = new decimal(new int[] {
            60000,
            0,
            0,
            0});
            this.num_ConnectTimeout.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.num_ConnectTimeout.Name = "num_ConnectTimeout";
            this.num_ConnectTimeout.Size = new System.Drawing.Size(207, 28);
            this.num_ConnectTimeout.TabIndex = 19;
            this.num_ConnectTimeout.Value = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(9, 400);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(170, 18);
            this.label9.TabIndex = 18;
            this.label9.Text = "连接超时（毫秒）：";
            // 
            // txt_Terminator
            // 
            this.txt_Terminator.Location = new System.Drawing.Point(569, 317);
            this.txt_Terminator.Name = "txt_Terminator";
            this.txt_Terminator.Size = new System.Drawing.Size(207, 28);
            this.txt_Terminator.TabIndex = 17;
            this.txt_Terminator.Text = "\\r\\n";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(368, 320);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(80, 18);
            this.label8.TabIndex = 16;
            this.label8.Text = "结束符：";
            // 
            // chk_UseTerminator
            // 
            this.chk_UseTerminator.AutoSize = true;
            this.chk_UseTerminator.Location = new System.Drawing.Point(116, 318);
            this.chk_UseTerminator.Name = "chk_UseTerminator";
            this.chk_UseTerminator.Size = new System.Drawing.Size(124, 22);
            this.chk_UseTerminator.TabIndex = 15;
            this.chk_UseTerminator.Text = "使用结束符";
            this.chk_UseTerminator.UseVisualStyleBackColor = true;
            // 
            // cmb_Encoding
            // 
            this.cmb_Encoding.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmb_Encoding.FormattingEnabled = true;
            this.cmb_Encoding.Items.AddRange(new object[] {
            "UTF-8",
            "GBK",
            "GB2312",
            "ASCII"});
            this.cmb_Encoding.Location = new System.Drawing.Point(116, 277);
            this.cmb_Encoding.Name = "cmb_Encoding";
            this.cmb_Encoding.Size = new System.Drawing.Size(207, 26);
            this.cmb_Encoding.TabIndex = 14;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(9, 280);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(98, 18);
            this.label7.TabIndex = 13;
            this.label7.Text = "编码方式：";
            // 
            // num_ReceiveBufferSize
            // 
            this.num_ReceiveBufferSize.Location = new System.Drawing.Point(569, 237);
            this.num_ReceiveBufferSize.Maximum = new decimal(new int[] {
            1048576,
            0,
            0,
            0});
            this.num_ReceiveBufferSize.Name = "num_ReceiveBufferSize";
            this.num_ReceiveBufferSize.Size = new System.Drawing.Size(207, 28);
            this.num_ReceiveBufferSize.TabIndex = 12;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(368, 240);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(206, 18);
            this.label6.TabIndex = 11;
            this.label6.Text = "接收缓冲(字节,0不限)：";
            // 
            // num_SendBufferSize
            // 
            this.num_SendBufferSize.Location = new System.Drawing.Point(116, 237);
            this.num_SendBufferSize.Maximum = new decimal(new int[] {
            1048576,
            0,
            0,
            0});
            this.num_SendBufferSize.Name = "num_SendBufferSize";
            this.num_SendBufferSize.Size = new System.Drawing.Size(207, 28);
            this.num_SendBufferSize.TabIndex = 10;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(9, 240);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(206, 18);
            this.label5.TabIndex = 9;
            this.label5.Text = "发送缓冲(字节,0不限)：";
            // 
            // chk_Enabled
            // 
            this.chk_Enabled.AutoSize = true;
            this.chk_Enabled.Checked = true;
            this.chk_Enabled.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chk_Enabled.Location = new System.Drawing.Point(116, 198);
            this.chk_Enabled.Name = "chk_Enabled";
            this.chk_Enabled.Size = new System.Drawing.Size(70, 22);
            this.chk_Enabled.TabIndex = 8;
            this.chk_Enabled.Text = "启用";
            this.chk_Enabled.UseVisualStyleBackColor = true;
            // 
            // num_Port
            // 
            this.num_Port.Location = new System.Drawing.Point(116, 157);
            this.num_Port.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.num_Port.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.num_Port.Name = "num_Port";
            this.num_Port.Size = new System.Drawing.Size(207, 28);
            this.num_Port.TabIndex = 7;
            this.num_Port.Value = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(9, 160);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(62, 18);
            this.label4.TabIndex = 6;
            this.label4.Text = "端口：";
            // 
            // txt_IpAddress
            // 
            this.txt_IpAddress.Location = new System.Drawing.Point(116, 117);
            this.txt_IpAddress.Name = "txt_IpAddress";
            this.txt_IpAddress.Size = new System.Drawing.Size(207, 28);
            this.txt_IpAddress.TabIndex = 5;
            this.txt_IpAddress.Text = "127.0.0.1";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(9, 120);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(80, 18);
            this.label3.TabIndex = 4;
            this.label3.Text = "IP地址：";
            // 
            // lbl_Type
            // 
            this.lbl_Type.AutoSize = true;
            this.lbl_Type.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lbl_Type.Location = new System.Drawing.Point(112, 77);
            this.lbl_Type.Name = "lbl_Type";
            this.lbl_Type.Size = new System.Drawing.Size(101, 25);
            this.lbl_Type.TabIndex = 3;
            this.lbl_Type.Text = "TCP客户端";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 77);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(62, 18);
            this.label2.TabIndex = 2;
            this.label2.Text = "类型：";
            // 
            // txt_Name
            // 
            this.txt_Name.Enabled = false;
            this.txt_Name.Location = new System.Drawing.Point(116, 34);
            this.txt_Name.Name = "txt_Name";
            this.txt_Name.ReadOnly = true;
            this.txt_Name.Size = new System.Drawing.Size(207, 28);
            this.txt_Name.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 37);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(62, 18);
            this.label1.TabIndex = 0;
            this.label1.Text = "名称：";
            // 
            // uConfigControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer_Main);
            this.Name = "uConfigControl";
            this.Size = new System.Drawing.Size(794, 854);
            this.splitContainer_Main.Panel1.ResumeLayout(false);
            this.splitContainer_Main.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer_Main)).EndInit();
            this.splitContainer_Main.ResumeLayout(false);
            this.panel_Test.ResumeLayout(false);
            this.splitContainer_Test.Panel1.ResumeLayout(false);
            this.splitContainer_Test.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer_Test)).EndInit();
            this.splitContainer_Test.ResumeLayout(false);
            this.panel_Receive.ResumeLayout(false);
            this.panel_Receive.PerformLayout();
            this.panel_ReceiveHeader.ResumeLayout(false);
            this.panel_Send.ResumeLayout(false);
            this.panel_Send.PerformLayout();
            this.panel_SendHeader.ResumeLayout(false);
            this.groupBox_Config.ResumeLayout(false);
            this.groupBox_Config.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.num_MaxConnections)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_ReconnectInterval)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_ConnectTimeout)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_ReceiveBufferSize)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_SendBufferSize)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_Port)).EndInit();
            this.ResumeLayout(false);

		}

		#endregion

        private System.Windows.Forms.SplitContainer splitContainer_Main;
        private System.Windows.Forms.Panel panel_Test;
        private System.Windows.Forms.SplitContainer splitContainer_Test;
        private System.Windows.Forms.Panel panel_Receive;
        private System.Windows.Forms.TextBox txt_Receive;
        private System.Windows.Forms.Panel panel_ReceiveHeader;
        private System.Windows.Forms.Button btn_ClearReceive;
        private System.Windows.Forms.Label lbl_Receive;
        private System.Windows.Forms.Panel panel_Send;
        private System.Windows.Forms.TextBox txt_Send;
        private System.Windows.Forms.Panel panel_SendHeader;
        private System.Windows.Forms.Button btn_Send;
        private System.Windows.Forms.Label lbl_Send;
		private System.Windows.Forms.GroupBox groupBox_Config;
		private System.Windows.Forms.TextBox txt_Remark;
		private System.Windows.Forms.Label label12;
		private System.Windows.Forms.Button btn_Save;
		private System.Windows.Forms.NumericUpDown num_MaxConnections;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.NumericUpDown num_ReconnectInterval;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.CheckBox chk_AutoReconnect;
		private System.Windows.Forms.NumericUpDown num_ConnectTimeout;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.TextBox txt_Terminator;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.CheckBox chk_UseTerminator;
		private System.Windows.Forms.ComboBox cmb_Encoding;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.NumericUpDown num_ReceiveBufferSize;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.NumericUpDown num_SendBufferSize;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.CheckBox chk_Enabled;
		private System.Windows.Forms.NumericUpDown num_Port;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox txt_IpAddress;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label lbl_Type;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox txt_Name;
		private System.Windows.Forms.Label label1;
    }
}
