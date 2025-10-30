using System.Windows.Forms;

namespace Vision.Frm.TcpConfig
{
    partial class Frm_TcpConfig
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
     this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.listBox_Configs = new System.Windows.Forms.ListBox();
     this.panel1 = new System.Windows.Forms.Panel();
         this.btn_Delete = new System.Windows.Forms.Button();
    this.btn_AddServer = new System.Windows.Forms.Button();
      this.btn_AddClient = new System.Windows.Forms.Button();
       this.groupBox_Config = new System.Windows.Forms.GroupBox();
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
    this.panel2 = new System.Windows.Forms.Panel();
     this.btn_Save = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
    this.splitContainer1.SuspendLayout();
   this.panel1.SuspendLayout();
            this.groupBox_Config.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.num_MaxConnections)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_ReconnectInterval)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_ConnectTimeout)).BeginInit();
       ((System.ComponentModel.ISupportInitialize)(this.num_ReceiveBufferSize)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_SendBufferSize)).BeginInit();
          ((System.ComponentModel.ISupportInitialize)(this.num_Port)).BeginInit();
    this.panel2.SuspendLayout();
    this.SuspendLayout();
   // 
            // splitContainer1
         // 
       this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
 this.splitContainer1.Location = new System.Drawing.Point(0, 0);
     this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
  this.splitContainer1.Panel1.Controls.Add(this.listBox_Configs);
          this.splitContainer1.Panel1.Controls.Add(this.panel1);
    // 
      // splitContainer1.Panel2
            // 
   this.splitContainer1.Panel2.Controls.Add(this.groupBox_Config);
         this.splitContainer1.Panel2.Controls.Add(this.panel2);
     this.splitContainer1.Size = new System.Drawing.Size(984, 661);
 this.splitContainer1.SplitterDistance = 250;
    this.splitContainer1.TabIndex = 0;
            // 
            // listBox_Configs
            // 
            this.listBox_Configs.Dock = System.Windows.Forms.DockStyle.Fill;
     this.listBox_Configs.FormattingEnabled = true;
            this.listBox_Configs.ItemHeight = 17;
          this.listBox_Configs.Location = new System.Drawing.Point(0, 0);
   this.listBox_Configs.Name = "listBox_Configs";
      this.listBox_Configs.Size = new System.Drawing.Size(250, 581);
            this.listBox_Configs.TabIndex = 0;
  // 
        // panel1
         // 
       this.panel1.Controls.Add(this.btn_Delete);
            this.panel1.Controls.Add(this.btn_AddServer);
       this.panel1.Controls.Add(this.btn_AddClient);
     this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
    this.panel1.Location = new System.Drawing.Point(0, 581);
      this.panel1.Name = "panel1";
      this.panel1.Size = new System.Drawing.Size(250, 80);
        this.panel1.TabIndex = 1;
            // 
            // btn_Delete
       // 
      this.btn_Delete.Location = new System.Drawing.Point(12, 43);
    this.btn_Delete.Name = "btn_Delete";
      this.btn_Delete.Size = new System.Drawing.Size(226, 30);
            this.btn_Delete.TabIndex = 2;
        this.btn_Delete.Text = "删除";
        this.btn_Delete.UseVisualStyleBackColor = true;
    // 
            // btn_AddServer
  // 
          this.btn_AddServer.Location = new System.Drawing.Point(130, 7);
  this.btn_AddServer.Name = "btn_AddServer";
            this.btn_AddServer.Size = new System.Drawing.Size(108, 30);
            this.btn_AddServer.TabIndex = 1;
    this.btn_AddServer.Text = "添加服务器";
            this.btn_AddServer.UseVisualStyleBackColor = true;
            // 
            // btn_AddClient
            // 
            this.btn_AddClient.Location = new System.Drawing.Point(12, 7);
  this.btn_AddClient.Name = "btn_AddClient";
 this.btn_AddClient.Size = new System.Drawing.Size(108, 30);
   this.btn_AddClient.TabIndex = 0;
   this.btn_AddClient.Text = "添加客户端";
            this.btn_AddClient.UseVisualStyleBackColor = true;
    // 
 // groupBox_Config
            // 
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
          this.groupBox_Config.Size = new System.Drawing.Size(730, 601);
      this.groupBox_Config.TabIndex = 1;
     this.groupBox_Config.TabStop = false;
            this.groupBox_Config.Text = "配置详情";
            // 
          // btn_Test
         // 
            this.btn_Test.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
  this.btn_Test.Location = new System.Drawing.Point(612, 25);
            this.btn_Test.Name = "btn_Test";
       this.btn_Test.Size = new System.Drawing.Size(105, 30);
            this.btn_Test.TabIndex = 27;
    this.btn_Test.Text = "收发测试";
  this.btn_Test.UseVisualStyleBackColor = true;
      // 
            // txt_Remark
        // 
       this.txt_Remark.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
   | System.Windows.Forms.AnchorStyles.Right)));
            this.txt_Remark.Location = new System.Drawing.Point(120, 502);
        this.txt_Remark.Multiline = true;
        this.txt_Remark.Name = "txt_Remark";
            this.txt_Remark.Size = new System.Drawing.Size(597, 76);
         this.txt_Remark.TabIndex = 26;
  // 
          // label12
       // 
      this.label12.AutoSize = true;
        this.label12.Location = new System.Drawing.Point(13, 505);
 this.label12.Name = "label12";
      this.label12.Size = new System.Drawing.Size(44, 17);
  this.label12.TabIndex = 25;
            this.label12.Text = "备注：";
            // 
     // num_MaxConnections
        // 
 this.num_MaxConnections.Location = new System.Drawing.Point(510, 462);
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
     this.num_MaxConnections.Size = new System.Drawing.Size(207, 23);
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
       this.label11.Size = new System.Drawing.Size(140, 17);
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
            this.num_ReconnectInterval.Location = new System.Drawing.Point(510, 422);
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
            this.num_ReconnectInterval.Size = new System.Drawing.Size(207, 23);
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
            this.label10.Size = new System.Drawing.Size(115, 17);
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
            this.chk_AutoReconnect.Size = new System.Drawing.Size(147, 21);
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
      this.num_ConnectTimeout.Size = new System.Drawing.Size(207, 23);
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
            this.label9.Size = new System.Drawing.Size(115, 17);
        this.label9.TabIndex = 18;
            this.label9.Text = "连接超时（毫秒）：";
  // 
       // txt_Terminator
         // 
      this.txt_Terminator.Location = new System.Drawing.Point(510, 382);
         this.txt_Terminator.Name = "txt_Terminator";
       this.txt_Terminator.Size = new System.Drawing.Size(207, 23);
            this.txt_Terminator.TabIndex = 17;
            this.txt_Terminator.Text = "\\r\\n";
   // 
            // label8
            // 
            this.label8.AutoSize = true;
     this.label8.Location = new System.Drawing.Point(372, 385);
       this.label8.Name = "label8";
      this.label8.Size = new System.Drawing.Size(56, 17);
    this.label8.TabIndex = 16;
            this.label8.Text = "结束符：";
  // 
  // chk_UseTerminator
   // 
     this.chk_UseTerminator.AutoSize = true;
    this.chk_UseTerminator.Location = new System.Drawing.Point(120, 383);
   this.chk_UseTerminator.Name = "chk_UseTerminator";
       this.chk_UseTerminator.Size = new System.Drawing.Size(99, 21);
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
         this.cmb_Encoding.Size = new System.Drawing.Size(207, 25);
 this.cmb_Encoding.TabIndex = 14;
            // 
          // label7
        // 
            this.label7.AutoSize = true;
     this.label7.Location = new System.Drawing.Point(13, 345);
        this.label7.Name = "label7";
       this.label7.Size = new System.Drawing.Size(68, 17);
            this.label7.TabIndex = 13;
    this.label7.Text = "编码方式：";
      // 
            // num_ReceiveBufferSize
// 
            this.num_ReceiveBufferSize.Location = new System.Drawing.Point(510, 302);
            this.num_ReceiveBufferSize.Maximum = new decimal(new int[] {
      1048576,
    0,
      0,
       0});
   this.num_ReceiveBufferSize.Name = "num_ReceiveBufferSize";
this.num_ReceiveBufferSize.Size = new System.Drawing.Size(207, 23);
            this.num_ReceiveBufferSize.TabIndex = 12;
       // 
      // label6
     // 
            this.label6.AutoSize = true;
  this.label6.Location = new System.Drawing.Point(372, 305);
      this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(140, 17);
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
            this.num_SendBufferSize.Size = new System.Drawing.Size(207, 23);
       this.num_SendBufferSize.TabIndex = 10;
            // 
            // label5
     // 
          this.label5.AutoSize = true;
    this.label5.Location = new System.Drawing.Point(13, 305);
    this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(140, 17);
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
     this.chk_Enabled.Size = new System.Drawing.Size(63, 21);
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
 this.num_Port.Size = new System.Drawing.Size(207, 23);
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
            this.label4.Size = new System.Drawing.Size(44, 17);
            this.label4.TabIndex = 6;
  this.label4.Text = "端口：";
  // 
     // txt_IpAddress
            // 
            this.txt_IpAddress.Location = new System.Drawing.Point(120, 182);
            this.txt_IpAddress.Name = "txt_IpAddress";
this.txt_IpAddress.Size = new System.Drawing.Size(207, 23);
            this.txt_IpAddress.TabIndex = 5;
     this.txt_IpAddress.Text = "127.0.0.1";
            // 
            // label3
            // 
      this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 185);
         this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(63, 17);
   this.label3.TabIndex = 4;
            this.label3.Text = "IP地址：";
     // 
            // lbl_Type
            // 
            this.lbl_Type.AutoSize = true;
     this.lbl_Type.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
   this.lbl_Type.Location = new System.Drawing.Point(116, 142);
      this.lbl_Type.Name = "lbl_Type";
    this.lbl_Type.Size = new System.Drawing.Size(65, 17);
   this.lbl_Type.TabIndex = 3;
  this.lbl_Type.Text = "TCP客户端";
   // 
    // label2
            // 
         this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 142);
            this.label2.Name = "label2";
   this.label2.Size = new System.Drawing.Size(44, 17);
            this.label2.TabIndex = 2;
            this.label2.Text = "类型：";
    // 
            // txt_Name
         // 
       this.txt_Name.Location = new System.Drawing.Point(120, 99);
       this.txt_Name.Name = "txt_Name";
   this.txt_Name.ReadOnly = true;
      this.txt_Name.Size = new System.Drawing.Size(207, 23);
    this.txt_Name.TabIndex = 1;
      // 
   // label1
            // 
     this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 102);
            this.label1.Name = "label1";
 this.label1.Size = new System.Drawing.Size(44, 17);
    this.label1.TabIndex = 0;
          this.label1.Text = "名称：";
            // 
  // panel2
      // 
    this.panel2.Controls.Add(this.btn_Save);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(0, 601);
   this.panel2.Name = "panel2";
         this.panel2.Size = new System.Drawing.Size(730, 60);
            this.panel2.TabIndex = 0;
     // 
     // btn_Save
            // 
            this.btn_Save.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
  this.btn_Save.Location = new System.Drawing.Point(612, 15);
   this.btn_Save.Name = "btn_Save";
      this.btn_Save.Size = new System.Drawing.Size(105, 35);
    this.btn_Save.TabIndex = 0;
            this.btn_Save.Text = "保存方案";
   this.btn_Save.UseVisualStyleBackColor = true;
            // 
            // Frm_TcpConfig
       // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(984, 661);
     this.Controls.Add(this.splitContainer1);
    this.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
        this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
         this.Name = "Frm_TcpConfig";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "TCP通讯配置";
      this.splitContainer1.Panel1.ResumeLayout(false);
        this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
       this.panel1.ResumeLayout(false);
            this.groupBox_Config.ResumeLayout(false);
this.groupBox_Config.PerformLayout();
       ((System.ComponentModel.ISupportInitialize)(this.num_MaxConnections)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_ReconnectInterval)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_ConnectTimeout)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_ReceiveBufferSize)).EndInit();
          ((System.ComponentModel.ISupportInitialize)(this.num_SendBufferSize)).EndInit();
 ((System.ComponentModel.ISupportInitialize)(this.num_Port)).EndInit();
       this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);

     }

  #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ListBox listBox_Configs;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btn_Delete;
  private System.Windows.Forms.Button btn_AddServer;
        private System.Windows.Forms.Button btn_AddClient;
        private System.Windows.Forms.GroupBox groupBox_Config;
     private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button btn_Save;
   private System.Windows.Forms.TextBox txt_Name;
   private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lbl_Type;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txt_IpAddress;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown num_Port;
      private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox chk_Enabled;
        private System.Windows.Forms.NumericUpDown num_SendBufferSize;
        private System.Windows.Forms.Label label5;
   private System.Windows.Forms.NumericUpDown num_ReceiveBufferSize;
      private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox cmb_Encoding;
        private System.Windows.Forms.Label label7;
      private System.Windows.Forms.CheckBox chk_UseTerminator;
   private System.Windows.Forms.TextBox txt_Terminator;
      private System.Windows.Forms.Label label8;
        private System.Windows.Forms.NumericUpDown num_ConnectTimeout;
      private System.Windows.Forms.Label label9;
        private System.Windows.Forms.CheckBox chk_AutoReconnect;
        private System.Windows.Forms.NumericUpDown num_ReconnectInterval;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.NumericUpDown num_MaxConnections;
 private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox txt_Remark;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Button btn_Test;
    }
}
