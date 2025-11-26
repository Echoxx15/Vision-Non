namespace TcpControlNet.UI
{
	partial class Frm_TcpTest
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
      this.txt_Log = new System.Windows.Forms.TextBox();
      this.txt_Send = new System.Windows.Forms.TextBox();
      this.btn_Send = new System.Windows.Forms.Button();
      this.lbl_Status = new System.Windows.Forms.Label();
      this.lbl_Statistics = new System.Windows.Forms.Label();
      this.grp_Append = new System.Windows.Forms.GroupBox();
      this.rb_AppendCRLF = new System.Windows.Forms.RadioButton();
      this.rb_AppendLF = new System.Windows.Forms.RadioButton();
      this.rb_AppendCR = new System.Windows.Forms.RadioButton();
      this.rb_AppendNone = new System.Windows.Forms.RadioButton();
      this.chk_AutoClear = new System.Windows.Forms.CheckBox();
      this.btn_ClearLog = new System.Windows.Forms.Button();
      this.txt_Receive = new System.Windows.Forms.TextBox();
      this.label1 = new System.Windows.Forms.Label();
      this.label2 = new System.Windows.Forms.Label();
      this.btn_Connect = new System.Windows.Forms.Button();
      this.grp_Append.SuspendLayout();
      this.SuspendLayout();
      // 
      // txt_Log
      // 
      this.txt_Log.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.txt_Log.BackColor = System.Drawing.Color.Black;
      this.txt_Log.Font = new System.Drawing.Font("Consolas", 9F);
      this.txt_Log.ForeColor = System.Drawing.Color.Lime;
      this.txt_Log.Location = new System.Drawing.Point(12, 72);
      this.txt_Log.Multiline = true;
      this.txt_Log.Name = "txt_Log";
      this.txt_Log.ReadOnly = true;
      this.txt_Log.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
      this.txt_Log.Size = new System.Drawing.Size(1160, 193);
      this.txt_Log.TabIndex = 0;
      // 
      // txt_Send
      // 
      this.txt_Send.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.txt_Send.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F);
      this.txt_Send.Location = new System.Drawing.Point(70, 556);
      this.txt_Send.Name = "txt_Send";
      this.txt_Send.Size = new System.Drawing.Size(960, 30);
      this.txt_Send.TabIndex = 1;
      // 
      // btn_Send
      // 
      this.btn_Send.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.btn_Send.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Bold);
      this.btn_Send.Location = new System.Drawing.Point(1036, 554);
      this.btn_Send.Name = "btn_Send";
      this.btn_Send.Size = new System.Drawing.Size(136, 32);
      this.btn_Send.TabIndex = 2;
      this.btn_Send.Text = "发送数据";
      this.btn_Send.UseVisualStyleBackColor = true;
      this.btn_Send.Click += new System.EventHandler(this.btn_Send_Click);
      // 
      // lbl_Status
      // 
      this.lbl_Status.AutoSize = true;
      this.lbl_Status.Font = new System.Drawing.Font("Microsoft YaHei UI", 10F, System.Drawing.FontStyle.Bold);
      this.lbl_Status.ForeColor = System.Drawing.Color.Green;
      this.lbl_Status.Location = new System.Drawing.Point(12, 15);
      this.lbl_Status.Name = "lbl_Status";
      this.lbl_Status.Size = new System.Drawing.Size(124, 27);
      this.lbl_Status.TabIndex = 3;
      this.lbl_Status.Text = "状态: 未连接";
      // 
      // lbl_Statistics
      // 
      this.lbl_Statistics.AutoSize = true;
      this.lbl_Statistics.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F);
      this.lbl_Statistics.Location = new System.Drawing.Point(250, 17);
      this.lbl_Statistics.Name = "lbl_Statistics";
      this.lbl_Statistics.Size = new System.Drawing.Size(183, 24);
      this.lbl_Statistics.TabIndex = 4;
      this.lbl_Statistics.Text = "发送: 0 条 | 接收: 0 条";
      // 
      // grp_Append
      // 
      this.grp_Append.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.grp_Append.Controls.Add(this.rb_AppendCRLF);
      this.grp_Append.Controls.Add(this.rb_AppendLF);
      this.grp_Append.Controls.Add(this.rb_AppendCR);
      this.grp_Append.Controls.Add(this.rb_AppendNone);
      this.grp_Append.Location = new System.Drawing.Point(700, 586);
      this.grp_Append.Name = "grp_Append";
      this.grp_Append.Size = new System.Drawing.Size(330, 50);
      this.grp_Append.TabIndex = 5;
      this.grp_Append.TabStop = false;
      this.grp_Append.Text = "追加";
      // 
      // rb_AppendCRLF
      // 
      this.rb_AppendCRLF.AutoSize = true;
      this.rb_AppendCRLF.Checked = true;
      this.rb_AppendCRLF.Location = new System.Drawing.Point(250, 20);
      this.rb_AppendCRLF.Name = "rb_AppendCRLF";
      this.rb_AppendCRLF.Size = new System.Drawing.Size(67, 28);
      this.rb_AppendCRLF.TabIndex = 3;
      this.rb_AppendCRLF.TabStop = true;
      this.rb_AppendCRLF.Text = "\\r\\n";
      this.rb_AppendCRLF.UseVisualStyleBackColor = true;
      // 
      // rb_AppendLF
      // 
      this.rb_AppendLF.AutoSize = true;
      this.rb_AppendLF.Location = new System.Drawing.Point(175, 20);
      this.rb_AppendLF.Name = "rb_AppendLF";
      this.rb_AppendLF.Size = new System.Drawing.Size(53, 28);
      this.rb_AppendLF.TabIndex = 2;
      this.rb_AppendLF.Text = "\\n";
      this.rb_AppendLF.UseVisualStyleBackColor = true;
      // 
      // rb_AppendCR
      // 
      this.rb_AppendCR.AutoSize = true;
      this.rb_AppendCR.Location = new System.Drawing.Point(100, 20);
      this.rb_AppendCR.Name = "rb_AppendCR";
      this.rb_AppendCR.Size = new System.Drawing.Size(49, 28);
      this.rb_AppendCR.TabIndex = 1;
      this.rb_AppendCR.Text = "\\r";
      this.rb_AppendCR.UseVisualStyleBackColor = true;
      // 
      // rb_AppendNone
      // 
      this.rb_AppendNone.AutoSize = true;
      this.rb_AppendNone.Location = new System.Drawing.Point(15, 20);
      this.rb_AppendNone.Name = "rb_AppendNone";
      this.rb_AppendNone.Size = new System.Drawing.Size(82, 28);
      this.rb_AppendNone.TabIndex = 0;
      this.rb_AppendNone.Text = "None";
      this.rb_AppendNone.UseVisualStyleBackColor = true;
      // 
      // chk_AutoClear
      // 
      this.chk_AutoClear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.chk_AutoClear.AutoSize = true;
      this.chk_AutoClear.Checked = true;
      this.chk_AutoClear.CheckState = System.Windows.Forms.CheckState.Checked;
      this.chk_AutoClear.Location = new System.Drawing.Point(1036, 606);
      this.chk_AutoClear.Name = "chk_AutoClear";
      this.chk_AutoClear.Size = new System.Drawing.Size(126, 28);
      this.chk_AutoClear.TabIndex = 6;
      this.chk_AutoClear.Text = "发送后清空";
      this.chk_AutoClear.UseVisualStyleBackColor = true;
      // 
      // btn_ClearLog
      // 
      this.btn_ClearLog.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.btn_ClearLog.Location = new System.Drawing.Point(1036, 12);
      this.btn_ClearLog.Name = "btn_ClearLog";
      this.btn_ClearLog.Size = new System.Drawing.Size(136, 35);
      this.btn_ClearLog.TabIndex = 8;
      this.btn_ClearLog.Text = "清空日志";
      this.btn_ClearLog.UseVisualStyleBackColor = true;
      this.btn_ClearLog.Click += new System.EventHandler(this.btn_ClearLog_Click);
      // 
      // txt_Receive
      // 
      this.txt_Receive.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.txt_Receive.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
      this.txt_Receive.Font = new System.Drawing.Font("Consolas", 9F);
      this.txt_Receive.ForeColor = System.Drawing.Color.Cyan;
      this.txt_Receive.Location = new System.Drawing.Point(12, 297);
      this.txt_Receive.Multiline = true;
      this.txt_Receive.Name = "txt_Receive";
      this.txt_Receive.ReadOnly = true;
      this.txt_Receive.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
      this.txt_Receive.Size = new System.Drawing.Size(1160, 243);
      this.txt_Receive.TabIndex = 7;
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(12, 45);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(86, 24);
      this.label1.TabIndex = 9;
      this.label1.Text = "通用日志:";
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(12, 270);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(86, 24);
      this.label2.TabIndex = 10;
      this.label2.Text = "接收数据:";
      // 
      // btn_Connect
      // 
      this.btn_Connect.Location = new System.Drawing.Point(900, 12);
      this.btn_Connect.Name = "btn_Connect";
      this.btn_Connect.Size = new System.Drawing.Size(120, 35);
      this.btn_Connect.TabIndex = 11;
      this.btn_Connect.Text = "连接";
      this.btn_Connect.UseVisualStyleBackColor = true;
      this.btn_Connect.Click += new System.EventHandler(this.btn_Connect_Click);
      // 
      // Frm_TcpTest
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 24F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(1184, 641);
      this.Controls.Add(this.btn_Connect);
      this.Controls.Add(this.label2);
      this.Controls.Add(this.label1);
      this.Controls.Add(this.btn_ClearLog);
      this.Controls.Add(this.txt_Receive);
      this.Controls.Add(this.chk_AutoClear);
      this.Controls.Add(this.grp_Append);
      this.Controls.Add(this.lbl_Statistics);
      this.Controls.Add(this.lbl_Status);
      this.Controls.Add(this.btn_Send);
      this.Controls.Add(this.txt_Send);
      this.Controls.Add(this.txt_Log);
      this.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F);
      this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
      this.MinimumSize = new System.Drawing.Size(1000, 600);
      this.Name = "Frm_TcpTest";
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
      this.Text = "TCP收发测试";
      this.grp_Append.ResumeLayout(false);
      this.grp_Append.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox txt_Log;
		private System.Windows.Forms.TextBox txt_Send;
		private System.Windows.Forms.Button btn_Send;
		private System.Windows.Forms.Label lbl_Status;
		private System.Windows.Forms.Label lbl_Statistics;
		private System.Windows.Forms.GroupBox grp_Append;
		private System.Windows.Forms.RadioButton rb_AppendCRLF;
		private System.Windows.Forms.RadioButton rb_AppendLF;
		private System.Windows.Forms.RadioButton rb_AppendCR;
		private System.Windows.Forms.RadioButton rb_AppendNone;
		private System.Windows.Forms.CheckBox chk_AutoClear;
		private System.Windows.Forms.Button btn_ClearLog;
		private System.Windows.Forms.TextBox txt_Receive;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button btn_Connect;
	}
}
