namespace ModbusControlNet.UI
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
            this.grpConnection = new System.Windows.Forms.GroupBox();
            this.btn_Save = new System.Windows.Forms.Button();
            this.txt_Name = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btn_Test = new System.Windows.Forms.Button();
            this.chkEnabled = new System.Windows.Forms.CheckBox();
            this.chkStringReverse = new System.Windows.Forms.CheckBox();
            this.cmbDataFormat = new System.Windows.Forms.ComboBox();
            this.label8 = new System.Windows.Forms.Label();
            this.numReceiveTimeout = new System.Windows.Forms.NumericUpDown();
            this.label7 = new System.Windows.Forms.Label();
            this.numConnectTimeout = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.numStation = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.numPort = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.txtIpAddress = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.grpConnection.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numReceiveTimeout)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numConnectTimeout)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numStation)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPort)).BeginInit();
            this.SuspendLayout();
            // 
            // grpConnection
            // 
            this.grpConnection.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.grpConnection.Controls.Add(this.btn_Save);
            this.grpConnection.Controls.Add(this.txt_Name);
            this.grpConnection.Controls.Add(this.label2);
            this.grpConnection.Controls.Add(this.btn_Test);
            this.grpConnection.Controls.Add(this.chkEnabled);
            this.grpConnection.Controls.Add(this.chkStringReverse);
            this.grpConnection.Controls.Add(this.cmbDataFormat);
            this.grpConnection.Controls.Add(this.label8);
            this.grpConnection.Controls.Add(this.numReceiveTimeout);
            this.grpConnection.Controls.Add(this.label7);
            this.grpConnection.Controls.Add(this.numConnectTimeout);
            this.grpConnection.Controls.Add(this.label6);
            this.grpConnection.Controls.Add(this.numStation);
            this.grpConnection.Controls.Add(this.label5);
            this.grpConnection.Controls.Add(this.numPort);
            this.grpConnection.Controls.Add(this.label4);
            this.grpConnection.Controls.Add(this.txtIpAddress);
            this.grpConnection.Controls.Add(this.label3);
            this.grpConnection.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpConnection.ForeColor = System.Drawing.Color.White;
            this.grpConnection.Location = new System.Drawing.Point(0, 0);
            this.grpConnection.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.grpConnection.Name = "grpConnection";
            this.grpConnection.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.grpConnection.Size = new System.Drawing.Size(587, 351);
            this.grpConnection.TabIndex = 1;
            this.grpConnection.TabStop = false;
            this.grpConnection.Text = "连接配置";
            // 
            // txt_Name
            // 
            this.txt_Name.AutoSize = true;
            this.txt_Name.ForeColor = System.Drawing.Color.White;
            this.txt_Name.Location = new System.Drawing.Point(177, 27);
            this.txt_Name.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.txt_Name.Name = "txt_Name";
            this.txt_Name.Size = new System.Drawing.Size(80, 18);
            this.txt_Name.TabIndex = 17;
            this.txt_Name.Text = "设备名称";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(26, 30);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(89, 18);
            this.label2.TabIndex = 16;
            this.label2.Text = "设备名称:";
            // 
            // btn_Test
            // 
            this.btn_Test.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(65)))));
            this.btn_Test.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_Test.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_Test.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn_Test.ForeColor = System.Drawing.Color.White;
            this.btn_Test.Location = new System.Drawing.Point(457, 56);
            this.btn_Test.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btn_Test.Name = "btn_Test";
            this.btn_Test.Size = new System.Drawing.Size(113, 31);
            this.btn_Test.TabIndex = 14;
            this.btn_Test.Text = "测试界面";
            this.btn_Test.UseVisualStyleBackColor = false;
            this.btn_Test.Click += new System.EventHandler(this.btn_Test_Click);
            // 
            // btn_Save
            // 
            this.btn_Save.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.btn_Save.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_Save.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_Save.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn_Save.ForeColor = System.Drawing.Color.White;
            this.btn_Save.Location = new System.Drawing.Point(457, 96);
            this.btn_Save.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btn_Save.Name = "btn_Save";
            this.btn_Save.Size = new System.Drawing.Size(113, 31);
            this.btn_Save.TabIndex = 19;
            this.btn_Save.Text = "保存配置";
            this.btn_Save.UseVisualStyleBackColor = false;
            this.btn_Save.Click += new System.EventHandler(this.btn_Save_Click);
            // 
            // chkEnabled
            // 
            this.chkEnabled.AutoSize = true;
            this.chkEnabled.ForeColor = System.Drawing.Color.White;
            this.chkEnabled.Location = new System.Drawing.Point(180, 322);
            this.chkEnabled.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.chkEnabled.Name = "chkEnabled";
            this.chkEnabled.Size = new System.Drawing.Size(106, 22);
            this.chkEnabled.TabIndex = 13;
            this.chkEnabled.Text = "启用连接";
            this.chkEnabled.UseVisualStyleBackColor = true;
            // 
            // chkStringReverse
            // 
            this.chkStringReverse.AutoSize = true;
            this.chkStringReverse.ForeColor = System.Drawing.Color.White;
            this.chkStringReverse.Location = new System.Drawing.Point(180, 292);
            this.chkStringReverse.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.chkStringReverse.Name = "chkStringReverse";
            this.chkStringReverse.Size = new System.Drawing.Size(196, 22);
            this.chkStringReverse.TabIndex = 18;
            this.chkStringReverse.Text = "字符串反转(AB→BA)";
            this.chkStringReverse.UseVisualStyleBackColor = true;
            // 
            // cmbDataFormat
            // 
            this.cmbDataFormat.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(65)))));
            this.cmbDataFormat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDataFormat.ForeColor = System.Drawing.Color.White;
            this.cmbDataFormat.FormattingEnabled = true;
            this.cmbDataFormat.Items.AddRange(new object[] {
            "ABCD",
            "CDAB",
            "BADC",
            "DCBA"});
            this.cmbDataFormat.Location = new System.Drawing.Point(180, 170);
            this.cmbDataFormat.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.cmbDataFormat.Name = "cmbDataFormat";
            this.cmbDataFormat.Size = new System.Drawing.Size(192, 26);
            this.cmbDataFormat.TabIndex = 11;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.ForeColor = System.Drawing.Color.White;
            this.label8.Location = new System.Drawing.Point(26, 173);
            this.label8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(107, 18);
            this.label8.TabIndex = 10;
            this.label8.Text = "DataFormat:";
            // 
            // numReceiveTimeout
            // 
            this.numReceiveTimeout.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(65)))));
            this.numReceiveTimeout.ForeColor = System.Drawing.Color.White;
            this.numReceiveTimeout.Increment = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numReceiveTimeout.Location = new System.Drawing.Point(180, 244);
            this.numReceiveTimeout.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.numReceiveTimeout.Maximum = new decimal(new int[] {
            60000,
            0,
            0,
            0});
            this.numReceiveTimeout.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.numReceiveTimeout.Name = "numReceiveTimeout";
            this.numReceiveTimeout.Size = new System.Drawing.Size(193, 28);
            this.numReceiveTimeout.TabIndex = 9;
            this.numReceiveTimeout.Value = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.ForeColor = System.Drawing.Color.White;
            this.label7.Location = new System.Drawing.Point(26, 246);
            this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(125, 18);
            this.label7.TabIndex = 8;
            this.label7.Text = "接收超时(ms):";
            // 
            // numConnectTimeout
            // 
            this.numConnectTimeout.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(65)))));
            this.numConnectTimeout.ForeColor = System.Drawing.Color.White;
            this.numConnectTimeout.Increment = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numConnectTimeout.Location = new System.Drawing.Point(180, 207);
            this.numConnectTimeout.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.numConnectTimeout.Maximum = new decimal(new int[] {
            60000,
            0,
            0,
            0});
            this.numConnectTimeout.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.numConnectTimeout.Name = "numConnectTimeout";
            this.numConnectTimeout.Size = new System.Drawing.Size(193, 28);
            this.numConnectTimeout.TabIndex = 7;
            this.numConnectTimeout.Value = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.ForeColor = System.Drawing.Color.White;
            this.label6.Location = new System.Drawing.Point(26, 209);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(125, 18);
            this.label6.TabIndex = 6;
            this.label6.Text = "连接超时(ms):";
            // 
            // numStation
            // 
            this.numStation.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(65)))));
            this.numStation.ForeColor = System.Drawing.Color.White;
            this.numStation.Location = new System.Drawing.Point(180, 133);
            this.numStation.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.numStation.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.numStation.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numStation.Name = "numStation";
            this.numStation.Size = new System.Drawing.Size(193, 28);
            this.numStation.TabIndex = 5;
            this.numStation.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.ForeColor = System.Drawing.Color.White;
            this.label5.Location = new System.Drawing.Point(26, 135);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(53, 18);
            this.label5.TabIndex = 4;
            this.label5.Text = "站号:";
            // 
            // numPort
            // 
            this.numPort.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(65)))));
            this.numPort.ForeColor = System.Drawing.Color.White;
            this.numPort.Location = new System.Drawing.Point(180, 96);
            this.numPort.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.numPort.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.numPort.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numPort.Name = "numPort";
            this.numPort.Size = new System.Drawing.Size(193, 28);
            this.numPort.TabIndex = 3;
            this.numPort.Value = new decimal(new int[] {
            502,
            0,
            0,
            0});
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.ForeColor = System.Drawing.Color.White;
            this.label4.Location = new System.Drawing.Point(26, 98);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(53, 18);
            this.label4.TabIndex = 2;
            this.label4.Text = "端口:";
            // 
            // txtIpAddress
            // 
            this.txtIpAddress.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(65)))));
            this.txtIpAddress.ForeColor = System.Drawing.Color.White;
            this.txtIpAddress.Location = new System.Drawing.Point(180, 59);
            this.txtIpAddress.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.txtIpAddress.Name = "txtIpAddress";
            this.txtIpAddress.Size = new System.Drawing.Size(256, 28);
            this.txtIpAddress.TabIndex = 1;
            this.txtIpAddress.Text = "127.0.0.1";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.ForeColor = System.Drawing.Color.White;
            this.label3.Location = new System.Drawing.Point(26, 62);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(71, 18);
            this.label3.TabIndex = 0;
            this.label3.Text = "IP地址:";
            // 
            // uConfigControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.grpConnection);
            this.Name = "uConfigControl";
            this.Size = new System.Drawing.Size(587, 351);
            this.grpConnection.ResumeLayout(false);
            this.grpConnection.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numReceiveTimeout)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numConnectTimeout)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numStation)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPort)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox grpConnection;
        private System.Windows.Forms.Button btn_Test;
        private System.Windows.Forms.CheckBox chkEnabled;
        private System.Windows.Forms.ComboBox cmbDataFormat;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.NumericUpDown numReceiveTimeout;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.NumericUpDown numConnectTimeout;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.NumericUpDown numStation;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown numPort;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtIpAddress;
        private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label txt_Name;
        private System.Windows.Forms.CheckBox chkStringReverse;
        private System.Windows.Forms.Button btn_Save;
    }
}
