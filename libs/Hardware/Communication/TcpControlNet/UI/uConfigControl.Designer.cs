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
			this.groupBox_Config = new System.Windows.Forms.GroupBox();
			this.btn_OpenTable = new System.Windows.Forms.Button();
			this.btn_Connect = new System.Windows.Forms.Button();
			this.btn_Test = new System.Windows.Forms.Button();
			this.txt_Remark = new System.Windows.Forms.TextBox();
			this.label12 = new System.Windows.Forms.Label();
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
			this.groupBox_Config.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.num_MaxConnections)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.num_ReconnectInterval)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.num_ConnectTimeout)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.num_ReceiveBufferSize)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.num_SendBufferSize)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.num_Port)).BeginInit();
			this.SuspendLayout();
			// 
			// groupBox_Config
			// 
			this.groupBox_Config.BackColor = System.Drawing.SystemColors.ActiveCaption;
			this.groupBox_Config.Controls.Add(this.btn_OpenTable);
			this.groupBox_Config.Controls.Add(this.btn_Connect);
			this.groupBox_Config.Controls.Add(this.btn_Test);
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
			this.groupBox_Config.Size = new System.Drawing.Size(794, 587);
			this.groupBox_Config.TabIndex = 2;
			this.groupBox_Config.TabStop = false;
			this.groupBox_Config.Text = "配置详情";
			// 
			// btn_OpenTable
			// 
			this.btn_OpenTable.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btn_OpenTable.BackColor = System.Drawing.SystemColors.ActiveCaption;
			this.btn_OpenTable.Cursor = System.Windows.Forms.Cursors.Hand;
			this.btn_OpenTable.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btn_OpenTable.Location = new System.Drawing.Point(375, 160);
			this.btn_OpenTable.Name = "btn_OpenTable";
			this.btn_OpenTable.Size = new System.Drawing.Size(142, 30);
			this.btn_OpenTable.TabIndex = 29;
			this.btn_OpenTable.Text = "通讯表配置";
			this.btn_OpenTable.UseVisualStyleBackColor = false;
			// 
			// btn_Connect
			// 
			this.btn_Connect.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btn_Connect.Location = new System.Drawing.Point(375, 96);
			this.btn_Connect.Name = "btn_Connect";
			this.btn_Connect.Size = new System.Drawing.Size(85, 31);
			this.btn_Connect.TabIndex = 28;
			this.btn_Connect.Text = "连接";
			this.btn_Connect.UseVisualStyleBackColor = true;
			this.btn_Connect.Click += new System.EventHandler(this.btn_Connect_Click);
			// 
			// btn_Test
			// 
			this.btn_Test.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btn_Test.BackColor = System.Drawing.SystemColors.ActiveCaption;
			this.btn_Test.Cursor = System.Windows.Forms.Cursors.Hand;
			this.btn_Test.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btn_Test.Location = new System.Drawing.Point(499, 97);
			this.btn_Test.Name = "btn_Test";
			this.btn_Test.Size = new System.Drawing.Size(105, 30);
			this.btn_Test.TabIndex = 27;
			this.btn_Test.Text = "测试界面";
			this.btn_Test.UseVisualStyleBackColor = false;
			this.btn_Test.Click += new System.EventHandler(this.btn_Test_Click);
			// 
			// txt_Remark
			// 
			this.txt_Remark.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.txt_Remark.Location = new System.Drawing.Point(120, 502);
			this.txt_Remark.Multiline = true;
			this.txt_Remark.Name = "txt_Remark";
			this.txt_Remark.Size = new System.Drawing.Size(661, 76);
			this.txt_Remark.TabIndex = 26;
			// 
			// label12
			// 
			this.label12.AutoSize = true;
			this.label12.Location = new System.Drawing.Point(13, 505);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(62, 18);
			this.label12.TabIndex = 25;
			this.label12.Text = "备注：";
			// 
			// num_MaxConnections
			// 
			this.num_MaxConnections.Location = new System.Drawing.Point(573, 462);
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
			this.label11.Location = new System.Drawing.Point(372, 465);
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
			this.num_ReconnectInterval.Location = new System.Drawing.Point(573, 422);
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
			this.label10.Location = new System.Drawing.Point(372, 425);
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
			this.chk_AutoReconnect.Location = new System.Drawing.Point(120, 423);
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
			this.num_ConnectTimeout.Location = new System.Drawing.Point(120, 462);
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
			this.label9.Location = new System.Drawing.Point(13, 465);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(170, 18);
			this.label9.TabIndex = 18;
			this.label9.Text = "连接超时（毫秒）：";
			// 
			// txt_Terminator
			// 
			this.txt_Terminator.Location = new System.Drawing.Point(573, 382);
			this.txt_Terminator.Name = "txt_Terminator";
			this.txt_Terminator.Size = new System.Drawing.Size(207, 28);
			this.txt_Terminator.TabIndex = 17;
			this.txt_Terminator.Text = "\\r\\n";
			// 
			// label8
			// 
			this.label8.AutoSize = true;
			this.label8.Location = new System.Drawing.Point(372, 385);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(80, 18);
			this.label8.TabIndex = 16;
			this.label8.Text = "结束符：";
			// 
			// chk_UseTerminator
			// 
			this.chk_UseTerminator.AutoSize = true;
			this.chk_UseTerminator.Location = new System.Drawing.Point(120, 383);
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
			this.cmb_Encoding.Location = new System.Drawing.Point(120, 342);
			this.cmb_Encoding.Name = "cmb_Encoding";
			this.cmb_Encoding.Size = new System.Drawing.Size(207, 26);
			this.cmb_Encoding.TabIndex = 14;
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point(13, 345);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(98, 18);
			this.label7.TabIndex = 13;
			this.label7.Text = "编码方式：";
			// 
			// num_ReceiveBufferSize
			// 
			this.num_ReceiveBufferSize.Location = new System.Drawing.Point(573, 302);
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
			this.label6.Location = new System.Drawing.Point(372, 305);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(206, 18);
			this.label6.TabIndex = 11;
			this.label6.Text = "接收缓冲(字节,0不限)：";
			// 
			// num_SendBufferSize
			// 
			this.num_SendBufferSize.Location = new System.Drawing.Point(120, 302);
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
			this.label5.Location = new System.Drawing.Point(13, 305);
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
			this.chk_Enabled.Location = new System.Drawing.Point(120, 263);
			this.chk_Enabled.Name = "chk_Enabled";
			this.chk_Enabled.Size = new System.Drawing.Size(70, 22);
			this.chk_Enabled.TabIndex = 8;
			this.chk_Enabled.Text = "启用";
			this.chk_Enabled.UseVisualStyleBackColor = true;
			// 
			// num_Port
			// 
			this.num_Port.Location = new System.Drawing.Point(120, 222);
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
			this.label4.Location = new System.Drawing.Point(13, 225);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(62, 18);
			this.label4.TabIndex = 6;
			this.label4.Text = "端口：";
			// 
			// txt_IpAddress
			// 
			this.txt_IpAddress.Location = new System.Drawing.Point(120, 182);
			this.txt_IpAddress.Name = "txt_IpAddress";
			this.txt_IpAddress.Size = new System.Drawing.Size(207, 28);
			this.txt_IpAddress.TabIndex = 5;
			this.txt_IpAddress.Text = "127.0.0.1";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(13, 185);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(80, 18);
			this.label3.TabIndex = 4;
			this.label3.Text = "IP地址：";
			// 
			// lbl_Type
			// 
			this.lbl_Type.AutoSize = true;
			this.lbl_Type.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.lbl_Type.Location = new System.Drawing.Point(116, 142);
			this.lbl_Type.Name = "lbl_Type";
			this.lbl_Type.Size = new System.Drawing.Size(101, 25);
			this.lbl_Type.TabIndex = 3;
			this.lbl_Type.Text = "TCP客户端";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(13, 142);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(62, 18);
			this.label2.TabIndex = 2;
			this.label2.Text = "类型：";
			// 
			// txt_Name
			// 
			this.txt_Name.Enabled = false;
			this.txt_Name.Location = new System.Drawing.Point(120, 99);
			this.txt_Name.Name = "txt_Name";
			this.txt_Name.ReadOnly = true;
			this.txt_Name.Size = new System.Drawing.Size(207, 28);
			this.txt_Name.TabIndex = 1;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(13, 102);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(62, 18);
			this.label1.TabIndex = 0;
			this.label1.Text = "名称：";
			// 
			// uConfigControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.groupBox_Config);
			this.Name = "uConfigControl";
			this.Size = new System.Drawing.Size(794, 587);
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

		private System.Windows.Forms.GroupBox groupBox_Config;
		private System.Windows.Forms.Button btn_Test;
		private System.Windows.Forms.TextBox txt_Remark;
		private System.Windows.Forms.Label label12;
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
		private System.Windows.Forms.Button btn_Connect;
        private System.Windows.Forms.Button btn_OpenTable;
    }
}
