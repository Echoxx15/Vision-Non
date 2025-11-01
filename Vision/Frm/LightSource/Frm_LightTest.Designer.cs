namespace Vision.Frm.LightSource
{
 partial class Frm_LightTest
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
 this.lblLightSource = new System.Windows.Forms.Label();
 this.cmbLightSource = new System.Windows.Forms.ComboBox();
 this.grpChannels = new System.Windows.Forms.GroupBox();
 this.btnApply4 = new System.Windows.Forms.Button();
 this.numBrightness4 = new System.Windows.Forms.NumericUpDown();
 this.trackBar4 = new System.Windows.Forms.TrackBar();
 this.chkCh4 = new System.Windows.Forms.CheckBox();
 this.lblCh4 = new System.Windows.Forms.Label();
 this.btnApply3 = new System.Windows.Forms.Button();
 this.numBrightness3 = new System.Windows.Forms.NumericUpDown();
 this.trackBar3 = new System.Windows.Forms.TrackBar();
 this.chkCh3 = new System.Windows.Forms.CheckBox();
 this.lblCh3 = new System.Windows.Forms.Label();
 this.btnApply2 = new System.Windows.Forms.Button();
 this.numBrightness2 = new System.Windows.Forms.NumericUpDown();
 this.trackBar2 = new System.Windows.Forms.TrackBar();
 this.chkCh2 = new System.Windows.Forms.CheckBox();
 this.lblCh2 = new System.Windows.Forms.Label();
 this.btnApply1 = new System.Windows.Forms.Button();
 this.numBrightness1 = new System.Windows.Forms.NumericUpDown();
 this.trackBar1 = new System.Windows.Forms.TrackBar();
 this.chkCh1 = new System.Windows.Forms.CheckBox();
 this.lblCh1 = new System.Windows.Forms.Label();
 this.grpMode = new System.Windows.Forms.GroupBox();
 this.rdConstantOff = new System.Windows.Forms.RadioButton();
 this.rdConstantOn = new System.Windows.Forms.RadioButton();
 this.grpCommand = new System.Windows.Forms.GroupBox();
 this.btnSend = new System.Windows.Forms.Button();
 this.txtCommand = new System.Windows.Forms.TextBox();
 this.lblSendData = new System.Windows.Forms.Label();
 this.lblLog = new System.Windows.Forms.Label();
 this.txtLog = new System.Windows.Forms.TextBox();
 this.lblStatus = new System.Windows.Forms.Label();
 this.grpChannels.SuspendLayout();
 ((System.ComponentModel.ISupportInitialize)(this.numBrightness4)).BeginInit();
 ((System.ComponentModel.ISupportInitialize)(this.trackBar4)).BeginInit();
 ((System.ComponentModel.ISupportInitialize)(this.numBrightness3)).BeginInit();
 ((System.ComponentModel.ISupportInitialize)(this.trackBar3)).BeginInit();
 ((System.ComponentModel.ISupportInitialize)(this.numBrightness2)).BeginInit();
 ((System.ComponentModel.ISupportInitialize)(this.trackBar2)).BeginInit();
 ((System.ComponentModel.ISupportInitialize)(this.numBrightness1)).BeginInit();
 ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).BeginInit();
 this.grpMode.SuspendLayout();
 this.grpCommand.SuspendLayout();
 this.SuspendLayout();
 // 
 // lblLightSource
 // 
 this.lblLightSource.AutoSize = true;
 this.lblLightSource.Location = new System.Drawing.Point(20,20);
 this.lblLightSource.Name = "lblLightSource";
 this.lblLightSource.Size = new System.Drawing.Size(68,17);
 this.lblLightSource.TabIndex =0;
 this.lblLightSource.Text = "光源选择:";
 // 
 // cmbLightSource
 // 
 this.cmbLightSource.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
 this.cmbLightSource.FormattingEnabled = true;
 this.cmbLightSource.Location = new System.Drawing.Point(94,17);
 this.cmbLightSource.Name = "cmbLightSource";
 this.cmbLightSource.Size = new System.Drawing.Size(300,25);
 this.cmbLightSource.TabIndex =1;
 this.cmbLightSource.SelectedIndexChanged += new System.EventHandler(this.cmbLightSource_SelectedIndexChanged);
 // 
 // grpChannels
 // 
 this.grpChannels.BackColor = System.Drawing.Color.White;
 this.grpChannels.Controls.Add(this.btnApply4);
 this.grpChannels.Controls.Add(this.numBrightness4);
 this.grpChannels.Controls.Add(this.trackBar4);
 this.grpChannels.Controls.Add(this.chkCh4);
 this.grpChannels.Controls.Add(this.lblCh4);
 this.grpChannels.Controls.Add(this.btnApply3);
 this.grpChannels.Controls.Add(this.numBrightness3);
 this.grpChannels.Controls.Add(this.trackBar3);
 this.grpChannels.Controls.Add(this.chkCh3);
 this.grpChannels.Controls.Add(this.lblCh3);
 this.grpChannels.Controls.Add(this.btnApply2);
 this.grpChannels.Controls.Add(this.numBrightness2);
 this.grpChannels.Controls.Add(this.trackBar2);
 this.grpChannels.Controls.Add(this.chkCh2);
 this.grpChannels.Controls.Add(this.lblCh2);
 this.grpChannels.Controls.Add(this.btnApply1);
 this.grpChannels.Controls.Add(this.numBrightness1);
 this.grpChannels.Controls.Add(this.trackBar1);
 this.grpChannels.Controls.Add(this.chkCh1);
 this.grpChannels.Controls.Add(this.lblCh1);
 this.grpChannels.Location = new System.Drawing.Point(12,55);
 this.grpChannels.Name = "grpChannels";
 this.grpChannels.Size = new System.Drawing.Size(810,280);
 this.grpChannels.TabIndex =2;
 this.grpChannels.TabStop = false;
 this.grpChannels.Text = "亮度设置";
 // 
 // btnApply4
 // 
 this.btnApply4.Location = new System.Drawing.Point(700,233);
 this.btnApply4.Name = "btnApply4";
 this.btnApply4.Size = new System.Drawing.Size(80,28);
 this.btnApply4.TabIndex =19;
 this.btnApply4.Text = "应用";
 this.btnApply4.UseVisualStyleBackColor = true;
 this.btnApply4.Click += new System.EventHandler(this.btnApply_Click);
 // 
 // numBrightness4
 // 
 this.numBrightness4.Location = new System.Drawing.Point(620,235);
 this.numBrightness4.Maximum = new decimal(new int[] {
255,
0,
0,
0});
 this.numBrightness4.Name = "numBrightness4";
 this.numBrightness4.Size = new System.Drawing.Size(70,23);
 this.numBrightness4.TabIndex =18;
 this.numBrightness4.ValueChanged += new System.EventHandler(this.NumericUpDown_ValueChanged);
 // 
 // trackBar4
 // 
 this.trackBar4.Location = new System.Drawing.Point(110,230);
 this.trackBar4.Maximum =255;
 this.trackBar4.Name = "trackBar4";
 this.trackBar4.Size = new System.Drawing.Size(500,45);
 this.trackBar4.TabIndex =17;
 this.trackBar4.TickFrequency =25;
 this.trackBar4.Scroll += new System.EventHandler(this.TrackBar_Scroll);
 // 
 // chkCh4
 // 
 this.chkCh4.AutoSize = true;
 this.chkCh4.Location = new System.Drawing.Point(85,235);
 this.chkCh4.Name = "chkCh4";
 this.chkCh4.Size = new System.Drawing.Size(15,14);
 this.chkCh4.TabIndex =16;
 this.chkCh4.UseVisualStyleBackColor = true;
 this.chkCh4.CheckedChanged += new System.EventHandler(this.CheckBox_CheckedChanged);
 // 
 // lblCh4
 // 
 this.lblCh4.AutoSize = true;
 this.lblCh4.Location = new System.Drawing.Point(30,235);
 this.lblCh4.Name = "lblCh4";
 this.lblCh4.Size = new System.Drawing.Size(39,17);
 this.lblCh4.TabIndex =15;
 this.lblCh4.Text = "CH4:";
 // 
 // btnApply3
 // 
 this.btnApply3.Location = new System.Drawing.Point(700,173);
 this.btnApply3.Name = "btnApply3";
 this.btnApply3.Size = new System.Drawing.Size(80,28);
 this.btnApply3.TabIndex =14;
 this.btnApply3.Text = "应用";
 this.btnApply3.UseVisualStyleBackColor = true;
 this.btnApply3.Click += new System.EventHandler(this.btnApply_Click);
 // 
 // numBrightness3
 // 
 this.numBrightness3.Location = new System.Drawing.Point(620,175);
 this.numBrightness3.Maximum = new decimal(new int[] {
255,
0,
0,
0});
 this.numBrightness3.Name = "numBrightness3";
 this.numBrightness3.Size = new System.Drawing.Size(70,23);
 this.numBrightness3.TabIndex =13;
 this.numBrightness3.ValueChanged += new System.EventHandler(this.NumericUpDown_ValueChanged);
 // 
 // trackBar3
 // 
 this.trackBar3.Location = new System.Drawing.Point(110,170);
 this.trackBar3.Maximum =255;
 this.trackBar3.Name = "trackBar3";
 this.trackBar3.Size = new System.Drawing.Size(500,45);
 this.trackBar3.TabIndex =12;
 this.trackBar3.TickFrequency =25;
 this.trackBar3.Scroll += new System.EventHandler(this.TrackBar_Scroll);
 // 
 // chkCh3
 // 
 this.chkCh3.AutoSize = true;
 this.chkCh3.Location = new System.Drawing.Point(85,175);
 this.chkCh3.Name = "chkCh3";
 this.chkCh3.Size = new System.Drawing.Size(15,14);
 this.chkCh3.TabIndex =11;
 this.chkCh3.UseVisualStyleBackColor = true;
 this.chkCh3.CheckedChanged += new System.EventHandler(this.CheckBox_CheckedChanged);
 // 
 // lblCh3
 // 
 this.lblCh3.AutoSize = true;
 this.lblCh3.Location = new System.Drawing.Point(30,175);
 this.lblCh3.Name = "lblCh3";
 this.lblCh3.Size = new System.Drawing.Size(39,17);
 this.lblCh3.TabIndex =10;
 this.lblCh3.Text = "CH3:";
 // 
 // btnApply2
 // 
 this.btnApply2.Location = new System.Drawing.Point(700,113);
 this.btnApply2.Name = "btnApply2";
 this.btnApply2.Size = new System.Drawing.Size(80,28);
 this.btnApply2.TabIndex =9;
 this.btnApply2.Text = "应用";
 this.btnApply2.UseVisualStyleBackColor = true;
 this.btnApply2.Click += new System.EventHandler(this.btnApply_Click);
 // 
 // numBrightness2
 // 
 this.numBrightness2.Location = new System.Drawing.Point(620,115);
 this.numBrightness2.Maximum = new decimal(new int[] {
255,
0,
0,
0});
 this.numBrightness2.Name = "numBrightness2";
 this.numBrightness2.Size = new System.Drawing.Size(70,23);
 this.numBrightness2.TabIndex =8;
 this.numBrightness2.ValueChanged += new System.EventHandler(this.NumericUpDown_ValueChanged);
 // 
 // trackBar2
 // 
 this.trackBar2.Location = new System.Drawing.Point(110,110);
 this.trackBar2.Maximum =255;
 this.trackBar2.Name = "trackBar2";
 this.trackBar2.Size = new System.Drawing.Size(500,45);
 this.trackBar2.TabIndex =7;
 this.trackBar2.TickFrequency =25;
 this.trackBar2.Scroll += new System.EventHandler(this.TrackBar_Scroll);
 // 
 // chkCh2
 // 
 this.chkCh2.AutoSize = true;
 this.chkCh2.Location = new System.Drawing.Point(85,115);
 this.chkCh2.Name = "chkCh2";
 this.chkCh2.Size = new System.Drawing.Size(15,14);
 this.chkCh2.TabIndex =6;
 this.chkCh2.UseVisualStyleBackColor = true;
 this.chkCh2.CheckedChanged += new System.EventHandler(this.CheckBox_CheckedChanged);
 // 
 // lblCh2
 // 
 this.lblCh2.AutoSize = true;
 this.lblCh2.Location = new System.Drawing.Point(30,115);
 this.lblCh2.Name = "lblCh2";
 this.lblCh2.Size = new System.Drawing.Size(39,17);
 this.lblCh2.TabIndex =5;
 this.lblCh2.Text = "CH2:";
 // 
 // btnApply1
 // 
 this.btnApply1.Location = new System.Drawing.Point(700,53);
 this.btnApply1.Name = "btnApply1";
 this.btnApply1.Size = new System.Drawing.Size(80,28);
 this.btnApply1.TabIndex =4;
 this.btnApply1.Text = "应用";
 this.btnApply1.UseVisualStyleBackColor = true;
 this.btnApply1.Click += new System.EventHandler(this.btnApply_Click);
 // 
 // numBrightness1
 // 
 this.numBrightness1.Location = new System.Drawing.Point(620,55);
 this.numBrightness1.Maximum = new decimal(new int[] {
255,
0,
0,
0});
 this.numBrightness1.Name = "numBrightness1";
 this.numBrightness1.Size = new System.Drawing.Size(70,23);
 this.numBrightness1.TabIndex =3;
 this.numBrightness1.ValueChanged += new System.EventHandler(this.NumericUpDown_ValueChanged);
 // 
 // trackBar1
 // 
 this.trackBar1.Location = new System.Drawing.Point(110,50);
 this.trackBar1.Maximum =255;
 this.trackBar1.Name = "trackBar1";
 this.trackBar1.Size = new System.Drawing.Size(500,45);
 this.trackBar1.TabIndex =2;
 this.trackBar1.TickFrequency =25;
 this.trackBar1.Scroll += new System.EventHandler(this.TrackBar_Scroll);
 // 
 // chkCh1
 // 
 this.chkCh1.AutoSize = true;
 this.chkCh1.Location = new System.Drawing.Point(85,55);
 this.chkCh1.Name = "chkCh1";
 this.chkCh1.Size = new System.Drawing.Size(15,14);
 this.chkCh1.TabIndex =1;
 this.chkCh1.UseVisualStyleBackColor = true;
 this.chkCh1.CheckedChanged += new System.EventHandler(this.CheckBox_CheckedChanged);
 // 
 // lblCh1
 // 
 this.lblCh1.AutoSize = true;
 this.lblCh1.Location = new System.Drawing.Point(30,55);
 this.lblCh1.Name = "lblCh1";
 this.lblCh1.Size = new System.Drawing.Size(39,17);
 this.lblCh1.TabIndex =0;
 this.lblCh1.Text = "CH1:";
 // 
 // grpMode
 // 
 this.grpMode.BackColor = System.Drawing.Color.White;
 this.grpMode.Controls.Add(this.rdConstantOff);
 this.grpMode.Controls.Add(this.rdConstantOn);
 this.grpMode.Location = new System.Drawing.Point(12,345);
 this.grpMode.Name = "grpMode";
 this.grpMode.Size = new System.Drawing.Size(810,60);
 this.grpMode.TabIndex =3;
 this.grpMode.TabStop = false;
 this.grpMode.Text = "常亮/常灭设置";
 // 
 // rdConstantOff
 // 
 this.rdConstantOff.AutoSize = true;
 this.rdConstantOff.Checked = true;
 this.rdConstantOff.Location = new System.Drawing.Point(460,25);
 this.rdConstantOff.Name = "rdConstantOff";
 this.rdConstantOff.Size = new System.Drawing.Size(50,21);
 this.rdConstantOff.TabIndex =1;
 this.rdConstantOff.TabStop = true;
 this.rdConstantOff.Text = "常灭";
 this.rdConstantOff.UseVisualStyleBackColor = true;
 this.rdConstantOff.CheckedChanged += new System.EventHandler(this.rdConstantOff_CheckedChanged);
 // 
 // rdConstantOn
 // 
 this.rdConstantOn.AutoSize = true;
 this.rdConstantOn.Location = new System.Drawing.Point(260,25);
 this.rdConstantOn.Name = "rdConstantOn";
 this.rdConstantOn.Size = new System.Drawing.Size(50,21);
 this.rdConstantOn.TabIndex =0;
 this.rdConstantOn.Text = "常亮";
 this.rdConstantOn.UseVisualStyleBackColor = true;
 this.rdConstantOn.CheckedChanged += new System.EventHandler(this.rdConstantOn_CheckedChanged);
 // 
 // grpCommand
 // 
 this.grpCommand.BackColor = System.Drawing.Color.White;
 this.grpCommand.Controls.Add(this.btnSend);
 this.grpCommand.Controls.Add(this.txtCommand);
 this.grpCommand.Controls.Add(this.lblSendData);
 this.grpCommand.Location = new System.Drawing.Point(12,415);
 this.grpCommand.Name = "grpCommand";
 this.grpCommand.Size = new System.Drawing.Size(810,80);
 this.grpCommand.TabIndex =4;
 this.grpCommand.TabStop = false;
 this.grpCommand.Text = "串口通讯数据";
 // 
 // btnSend
 // 
 this.btnSend.Location = new System.Drawing.Point(685,28);
 this.btnSend.Name = "btnSend";
 this.btnSend.Size = new System.Drawing.Size(100,28);
 this.btnSend.TabIndex =2;
 this.btnSend.Text = "发送";
 this.btnSend.UseVisualStyleBackColor = true;
 this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
 // 
 // txtCommand
 // 
 this.txtCommand.Location = new System.Drawing.Point(125,30);
 this.txtCommand.Name = "txtCommand";
 this.txtCommand.Size = new System.Drawing.Size(550,23);
 this.txtCommand.TabIndex =1;
 // 
 // lblSendData
 // 
 this.lblSendData.AutoSize = true;
 this.lblSendData.Location = new System.Drawing.Point(30,33);
 this.lblSendData.Name = "lblSendData";
 this.lblSendData.Size = new System.Drawing.Size(80,17);
 this.lblSendData.TabIndex =0;
 this.lblSendData.Text = "发送数据框:";
 // 
 // lblLog
 // 
 this.lblLog.AutoSize = true;
 this.lblLog.Location = new System.Drawing.Point(12,505);
 this.lblLog.Name = "lblLog";
 this.lblLog.Size = new System.Drawing.Size(68,17);
 this.lblLog.TabIndex =5;
 this.lblLog.Text = "提示信息:";
 // 
 // txtLog
 // 
 this.txtLog.BackColor = System.Drawing.Color.LightYellow;
 this.txtLog.Location = new System.Drawing.Point(12,525);
 this.txtLog.Multiline = true;
 this.txtLog.Name = "txtLog";
 this.txtLog.ReadOnly = true;
 this.txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
 this.txtLog.Size = new System.Drawing.Size(810,100);
 this.txtLog.TabIndex =6;
 // 
 // lblStatus
 // 
 this.lblStatus.AutoSize = true;
 this.lblStatus.ForeColor = System.Drawing.Color.Gray;
 this.lblStatus.Location = new System.Drawing.Point(400,21);
 this.lblStatus.Name = "lblStatus";
 this.lblStatus.Size = new System.Drawing.Size(176,17);
 this.lblStatus.TabIndex =7;
 this.lblStatus.Text = "(已自动连接到启用的光源)";
 // 
 // Frm_LightTest
 // 
 this.AutoScaleDimensions = new System.Drawing.SizeF(7F,17F);
 this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
 this.BackColor = System.Drawing.Color.WhiteSmoke;
 this.ClientSize = new System.Drawing.Size(834,637);
 this.Controls.Add(this.lblStatus);
 this.Controls.Add(this.txtLog);
 this.Controls.Add(this.lblLog);
 this.Controls.Add(this.grpCommand);
 this.Controls.Add(this.grpMode);
 this.Controls.Add(this.grpChannels);
 this.Controls.Add(this.cmbLightSource);
 this.Controls.Add(this.lblLightSource);
 this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
 this.MaximizeBox = false;
 this.Name = "Frm_LightTest";
 this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
 this.Text = "光源测试 -4通道控制";
 this.Load += new System.EventHandler(this.Frm_LightTest_Load);
 this.grpChannels.ResumeLayout(false);
 this.grpChannels.PerformLayout();
 ((System.ComponentModel.ISupportInitialize)(this.numBrightness4)).EndInit();
 ((System.ComponentModel.ISupportInitialize)(this.trackBar4)).EndInit();
 ((System.ComponentModel.ISupportInitialize)(this.numBrightness3)).EndInit();
 ((System.ComponentModel.ISupportInitialize)(this.trackBar3)).EndInit();
 ((System.ComponentModel.ISupportInitialize)(this.numBrightness2)).EndInit();
 ((System.ComponentModel.ISupportInitialize)(this.trackBar2)).EndInit();
 ((System.ComponentModel.ISupportInitialize)(this.numBrightness1)).EndInit();
 ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).EndInit();
 this.grpMode.ResumeLayout(false);
 this.grpMode.PerformLayout();
 this.grpCommand.ResumeLayout(false);
 this.grpCommand.PerformLayout();
 this.ResumeLayout(false);
 this.PerformLayout();

 }

 #endregion

 private System.Windows.Forms.Label lblLightSource;
 private System.Windows.Forms.ComboBox cmbLightSource;
 private System.Windows.Forms.GroupBox grpChannels;
 private System.Windows.Forms.Button btnApply4;
 private System.Windows.Forms.NumericUpDown numBrightness4;
 private System.Windows.Forms.TrackBar trackBar4;
 private System.Windows.Forms.CheckBox chkCh4;
 private System.Windows.Forms.Label lblCh4;
 private System.Windows.Forms.Button btnApply3;
 private System.Windows.Forms.NumericUpDown numBrightness3;
 private System.Windows.Forms.TrackBar trackBar3;
 private System.Windows.Forms.CheckBox chkCh3;
 private System.Windows.Forms.Label lblCh3;
 private System.Windows.Forms.Button btnApply2;
 private System.Windows.Forms.NumericUpDown numBrightness2;
 private System.Windows.Forms.TrackBar trackBar2;
 private System.Windows.Forms.CheckBox chkCh2;
 private System.Windows.Forms.Label lblCh2;
 private System.Windows.Forms.Button btnApply1;
 private System.Windows.Forms.NumericUpDown numBrightness1;
 private System.Windows.Forms.TrackBar trackBar1;
 private System.Windows.Forms.CheckBox chkCh1;
 private System.Windows.Forms.Label lblCh1;
 private System.Windows.Forms.GroupBox grpMode;
 private System.Windows.Forms.RadioButton rdConstantOff;
 private System.Windows.Forms.RadioButton rdConstantOn;
 private System.Windows.Forms.GroupBox grpCommand;
 private System.Windows.Forms.Button btnSend;
 private System.Windows.Forms.TextBox txtCommand;
 private System.Windows.Forms.Label lblSendData;
 private System.Windows.Forms.Label lblLog;
 private System.Windows.Forms.TextBox txtLog;
 private System.Windows.Forms.Label lblStatus;
 }
}
