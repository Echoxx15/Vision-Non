namespace UserControls.LightTestForm
{
    partial class FugenPinshanLight
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
            this.cmbPort = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.cmbChannel = new System.Windows.Forms.ComboBox();
            this.nudTriggerTimeWrite = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.btnWriteTriggerTime = new System.Windows.Forms.Button();
            this.btnReadTriggerTime = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.nudTriggerTimeRead = new System.Windows.Forms.NumericUpDown();
            this.btnReadDelayTime = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.nudDelayTimeRead = new System.Windows.Forms.NumericUpDown();
            this.btnWriteDelayTime = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.nudDelayTimeWrite = new System.Windows.Forms.NumericUpDown();
            this.btnReadTriggerType = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.btnWriteTriggerType = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.cmbTriggerType = new System.Windows.Forms.ComboBox();
            this.lblTriggerTypeRead = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.btnReadInternalPeriod = new System.Windows.Forms.Button();
            this.label11 = new System.Windows.Forms.Label();
            this.nudInternalPeriodRead = new System.Windows.Forms.NumericUpDown();
            this.btnWriteInternalPeriod = new System.Windows.Forms.Button();
            this.label12 = new System.Windows.Forms.Label();
            this.nudInternalPeriodWrite = new System.Windows.Forms.NumericUpDown();
            this.btnSoftTrigger = new System.Windows.Forms.Button();
            this.btnConnect = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.nudTriggerTimeWrite)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudTriggerTimeRead)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudDelayTimeRead)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudDelayTimeWrite)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudInternalPeriodRead)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudInternalPeriodWrite)).BeginInit();
            this.SuspendLayout();
            // 
            // cmbPort
            // 
            this.cmbPort.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPort.FormattingEnabled = true;
            this.cmbPort.Location = new System.Drawing.Point(277, 37);
            this.cmbPort.Name = "cmbPort";
            this.cmbPort.Size = new System.Drawing.Size(210, 26);
            this.cmbPort.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(194, 41);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(44, 18);
            this.label1.TabIndex = 1;
            this.label1.Text = "端口";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(179, 93);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(71, 18);
            this.label2.TabIndex = 3;
            this.label2.Text = "通道1-4";
            // 
            // cmbChannel
            // 
            this.cmbChannel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbChannel.Enabled = false;
            this.cmbChannel.FormattingEnabled = true;
            this.cmbChannel.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4"});
            this.cmbChannel.Location = new System.Drawing.Point(277, 89);
            this.cmbChannel.Name = "cmbChannel";
            this.cmbChannel.Size = new System.Drawing.Size(210, 26);
            this.cmbChannel.TabIndex = 2;
            // 
            // nudTriggerTimeWrite
            // 
            this.nudTriggerTimeWrite.Enabled = false;
            this.nudTriggerTimeWrite.Location = new System.Drawing.Point(277, 138);
            this.nudTriggerTimeWrite.Maximum = new decimal(new int[] {
            999,
            0,
            0,
            0});
            this.nudTriggerTimeWrite.Name = "nudTriggerTimeWrite";
            this.nudTriggerTimeWrite.Size = new System.Drawing.Size(210, 28);
            this.nudTriggerTimeWrite.TabIndex = 4;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 143);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(242, 18);
            this.label3.TabIndex = 5;
            this.label3.Text = "设置触发点亮时间(0~999μs)";
            // 
            // btnWriteTriggerTime
            // 
            this.btnWriteTriggerTime.Enabled = false;
            this.btnWriteTriggerTime.Location = new System.Drawing.Point(512, 138);
            this.btnWriteTriggerTime.Name = "btnWriteTriggerTime";
            this.btnWriteTriggerTime.Size = new System.Drawing.Size(75, 28);
            this.btnWriteTriggerTime.TabIndex = 6;
            this.btnWriteTriggerTime.Text = "写入";
            this.btnWriteTriggerTime.UseVisualStyleBackColor = true;
            // 
            // btnReadTriggerTime
            // 
            this.btnReadTriggerTime.Enabled = false;
            this.btnReadTriggerTime.Location = new System.Drawing.Point(512, 181);
            this.btnReadTriggerTime.Name = "btnReadTriggerTime";
            this.btnReadTriggerTime.Size = new System.Drawing.Size(75, 28);
            this.btnReadTriggerTime.TabIndex = 9;
            this.btnReadTriggerTime.Text = "读取";
            this.btnReadTriggerTime.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 186);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(242, 18);
            this.label4.TabIndex = 8;
            this.label4.Text = "读取触发点亮时间(0~999μs)";
            // 
            // nudTriggerTimeRead
            // 
            this.nudTriggerTimeRead.Enabled = false;
            this.nudTriggerTimeRead.Location = new System.Drawing.Point(277, 181);
            this.nudTriggerTimeRead.Maximum = new decimal(new int[] {
            999,
            0,
            0,
            0});
            this.nudTriggerTimeRead.Name = "nudTriggerTimeRead";
            this.nudTriggerTimeRead.ReadOnly = true;
            this.nudTriggerTimeRead.Size = new System.Drawing.Size(210, 28);
            this.nudTriggerTimeRead.TabIndex = 7;
            // 
            // btnReadDelayTime
            // 
            this.btnReadDelayTime.Enabled = false;
            this.btnReadDelayTime.Location = new System.Drawing.Point(512, 267);
            this.btnReadDelayTime.Name = "btnReadDelayTime";
            this.btnReadDelayTime.Size = new System.Drawing.Size(75, 28);
            this.btnReadDelayTime.TabIndex = 15;
            this.btnReadDelayTime.Text = "读取";
            this.btnReadDelayTime.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 272);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(233, 18);
            this.label5.TabIndex = 14;
            this.label5.Text = "读取延时点亮时间(0~99μs)";
            // 
            // nudDelayTimeRead
            // 
            this.nudDelayTimeRead.Enabled = false;
            this.nudDelayTimeRead.Location = new System.Drawing.Point(277, 267);
            this.nudDelayTimeRead.Maximum = new decimal(new int[] {
            99,
            0,
            0,
            0});
            this.nudDelayTimeRead.Name = "nudDelayTimeRead";
            this.nudDelayTimeRead.ReadOnly = true;
            this.nudDelayTimeRead.Size = new System.Drawing.Size(210, 28);
            this.nudDelayTimeRead.TabIndex = 13;
            // 
            // btnWriteDelayTime
            // 
            this.btnWriteDelayTime.Enabled = false;
            this.btnWriteDelayTime.Location = new System.Drawing.Point(512, 224);
            this.btnWriteDelayTime.Name = "btnWriteDelayTime";
            this.btnWriteDelayTime.Size = new System.Drawing.Size(75, 28);
            this.btnWriteDelayTime.TabIndex = 12;
            this.btnWriteDelayTime.Text = "写入";
            this.btnWriteDelayTime.UseVisualStyleBackColor = true;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(12, 229);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(233, 18);
            this.label6.TabIndex = 11;
            this.label6.Text = "设置延时点亮时间(0~99μs)";
            // 
            // nudDelayTimeWrite
            // 
            this.nudDelayTimeWrite.Enabled = false;
            this.nudDelayTimeWrite.Location = new System.Drawing.Point(277, 224);
            this.nudDelayTimeWrite.Maximum = new decimal(new int[] {
            99,
            0,
            0,
            0});
            this.nudDelayTimeWrite.Name = "nudDelayTimeWrite";
            this.nudDelayTimeWrite.Size = new System.Drawing.Size(210, 28);
            this.nudDelayTimeWrite.TabIndex = 10;
            // 
            // btnReadTriggerType
            // 
            this.btnReadTriggerType.Enabled = false;
            this.btnReadTriggerType.Location = new System.Drawing.Point(512, 355);
            this.btnReadTriggerType.Name = "btnReadTriggerType";
            this.btnReadTriggerType.Size = new System.Drawing.Size(75, 28);
            this.btnReadTriggerType.TabIndex = 21;
            this.btnReadTriggerType.Text = "读取";
            this.btnReadTriggerType.UseVisualStyleBackColor = true;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(12, 360);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(134, 18);
            this.label7.TabIndex = 20;
            this.label7.Text = "读取触发器类型";
            // 
            // btnWriteTriggerType
            // 
            this.btnWriteTriggerType.Enabled = false;
            this.btnWriteTriggerType.Location = new System.Drawing.Point(512, 312);
            this.btnWriteTriggerType.Name = "btnWriteTriggerType";
            this.btnWriteTriggerType.Size = new System.Drawing.Size(75, 28);
            this.btnWriteTriggerType.TabIndex = 18;
            this.btnWriteTriggerType.Text = "写入";
            this.btnWriteTriggerType.UseVisualStyleBackColor = true;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(12, 317);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(134, 18);
            this.label8.TabIndex = 17;
            this.label8.Text = "设置触发器类型";
            // 
            // cmbTriggerType
            // 
            this.cmbTriggerType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTriggerType.Enabled = false;
            this.cmbTriggerType.FormattingEnabled = true;
            this.cmbTriggerType.Items.AddRange(new object[] {
            "0-上升沿触发",
            "1-下降沿触发",
            "2-常亮",
            "3-内部触发"});
            this.cmbTriggerType.Location = new System.Drawing.Point(277, 313);
            this.cmbTriggerType.Name = "cmbTriggerType";
            this.cmbTriggerType.Size = new System.Drawing.Size(210, 26);
            this.cmbTriggerType.TabIndex = 22;
            // 
            // lblTriggerTypeRead
            // 
            this.lblTriggerTypeRead.AutoSize = true;
            this.lblTriggerTypeRead.Location = new System.Drawing.Point(236, 360);
            this.lblTriggerTypeRead.Name = "lblTriggerTypeRead";
            this.lblTriggerTypeRead.Size = new System.Drawing.Size(35, 18);
            this.lblTriggerTypeRead.TabIndex = 23;
            this.lblTriggerTypeRead.Text = "---";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.ForeColor = System.Drawing.Color.Gray;
            this.label10.Location = new System.Drawing.Point(12, 397);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(458, 18);
            this.label10.TabIndex = 24;
            this.label10.Text = "注：0-上升沿触发，1-下降沿触发，2-常亮，3-内部触发";
            // 
            // btnReadInternalPeriod
            // 
            this.btnReadInternalPeriod.Enabled = false;
            this.btnReadInternalPeriod.Location = new System.Drawing.Point(512, 487);
            this.btnReadInternalPeriod.Name = "btnReadInternalPeriod";
            this.btnReadInternalPeriod.Size = new System.Drawing.Size(75, 28);
            this.btnReadInternalPeriod.TabIndex = 30;
            this.btnReadInternalPeriod.Text = "读取";
            this.btnReadInternalPeriod.UseVisualStyleBackColor = true;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(12, 492);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(242, 18);
            this.label11.TabIndex = 29;
            this.label11.Text = "读取内部触发周期(10~999ms)";
            // 
            // nudInternalPeriodRead
            // 
            this.nudInternalPeriodRead.Enabled = false;
            this.nudInternalPeriodRead.Location = new System.Drawing.Point(277, 487);
            this.nudInternalPeriodRead.Maximum = new decimal(new int[] {
            999,
            0,
            0,
            0});
            this.nudInternalPeriodRead.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nudInternalPeriodRead.Name = "nudInternalPeriodRead";
            this.nudInternalPeriodRead.ReadOnly = true;
            this.nudInternalPeriodRead.Size = new System.Drawing.Size(210, 28);
            this.nudInternalPeriodRead.TabIndex = 28;
            this.nudInternalPeriodRead.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // btnWriteInternalPeriod
            // 
            this.btnWriteInternalPeriod.Enabled = false;
            this.btnWriteInternalPeriod.Location = new System.Drawing.Point(512, 444);
            this.btnWriteInternalPeriod.Name = "btnWriteInternalPeriod";
            this.btnWriteInternalPeriod.Size = new System.Drawing.Size(75, 28);
            this.btnWriteInternalPeriod.TabIndex = 27;
            this.btnWriteInternalPeriod.Text = "写入";
            this.btnWriteInternalPeriod.UseVisualStyleBackColor = true;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(12, 449);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(242, 18);
            this.label12.TabIndex = 26;
            this.label12.Text = "设置内部触发周期(10~999ms)";
            // 
            // nudInternalPeriodWrite
            // 
            this.nudInternalPeriodWrite.Enabled = false;
            this.nudInternalPeriodWrite.Location = new System.Drawing.Point(277, 444);
            this.nudInternalPeriodWrite.Maximum = new decimal(new int[] {
            999,
            0,
            0,
            0});
            this.nudInternalPeriodWrite.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nudInternalPeriodWrite.Name = "nudInternalPeriodWrite";
            this.nudInternalPeriodWrite.Size = new System.Drawing.Size(210, 28);
            this.nudInternalPeriodWrite.TabIndex = 25;
            this.nudInternalPeriodWrite.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // btnSoftTrigger
            // 
            this.btnSoftTrigger.Enabled = false;
            this.btnSoftTrigger.Location = new System.Drawing.Point(277, 538);
            this.btnSoftTrigger.Name = "btnSoftTrigger";
            this.btnSoftTrigger.Size = new System.Drawing.Size(310, 35);
            this.btnSoftTrigger.TabIndex = 31;
            this.btnSoftTrigger.Text = "软件触发通道点亮一次";
            this.btnSoftTrigger.UseVisualStyleBackColor = true;
            // 
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(512, 35);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(89, 28);
            this.btnConnect.TabIndex = 32;
            this.btnConnect.Text = "连接";
            this.btnConnect.UseVisualStyleBackColor = true;
            // 
            // FugenPinshanLight
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(672, 600);
            this.Controls.Add(this.btnConnect);
            this.Controls.Add(this.btnSoftTrigger);
            this.Controls.Add(this.btnReadInternalPeriod);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.nudInternalPeriodRead);
            this.Controls.Add(this.btnWriteInternalPeriod);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.nudInternalPeriodWrite);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.lblTriggerTypeRead);
            this.Controls.Add(this.cmbTriggerType);
            this.Controls.Add(this.btnReadTriggerType);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.btnWriteTriggerType);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.btnReadDelayTime);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.nudDelayTimeRead);
            this.Controls.Add(this.btnWriteDelayTime);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.nudDelayTimeWrite);
            this.Controls.Add(this.btnReadTriggerTime);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.nudTriggerTimeRead);
            this.Controls.Add(this.btnWriteTriggerTime);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.nudTriggerTimeWrite);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cmbChannel);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cmbPort);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FugenPinshanLight";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "孚根频闪光源测试";
            ((System.ComponentModel.ISupportInitialize)(this.nudTriggerTimeWrite)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudTriggerTimeRead)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudDelayTimeRead)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudDelayTimeWrite)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudInternalPeriodRead)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudInternalPeriodWrite)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cmbPort;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cmbChannel;
        private System.Windows.Forms.NumericUpDown nudTriggerTimeWrite;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnWriteTriggerTime;
        private System.Windows.Forms.Button btnReadTriggerTime;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown nudTriggerTimeRead;
        private System.Windows.Forms.Button btnReadDelayTime;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown nudDelayTimeRead;
        private System.Windows.Forms.Button btnWriteDelayTime;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.NumericUpDown nudDelayTimeWrite;
        private System.Windows.Forms.Button btnReadTriggerType;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button btnWriteTriggerType;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ComboBox cmbTriggerType;
        private System.Windows.Forms.Label lblTriggerTypeRead;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Button btnReadInternalPeriod;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.NumericUpDown nudInternalPeriodRead;
        private System.Windows.Forms.Button btnWriteInternalPeriod;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.NumericUpDown nudInternalPeriodWrite;
        private System.Windows.Forms.Button btnSoftTrigger;
        private System.Windows.Forms.Button btnConnect;
    }
}
