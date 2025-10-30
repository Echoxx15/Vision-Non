using System.Windows.Forms;

namespace Vision.Frm.LightSource
{
    partial class Frm_LightConfig
    {
 private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
     {
            if (disposing && (components != null))
   {
       components.Dispose();
         }
   base.Dispose(disposing);
   }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.listBox_Configs = new System.Windows.Forms.ListBox();
  this.panel1 = new System.Windows.Forms.Panel();
      this.btn_Delete = new System.Windows.Forms.Button();
 this.btn_AddOpt = new System.Windows.Forms.Button();
            this.btn_AddFgen = new System.Windows.Forms.Button();
     this.groupBox_Config = new System.Windows.Forms.GroupBox();
 this.btn_Test = new System.Windows.Forms.Button();
      this.txt_Remark = new System.Windows.Forms.TextBox();
       this.label12 = new System.Windows.Forms.Label();
       this.cmb_ChannelCount = new System.Windows.Forms.ComboBox();
            this.label11 = new System.Windows.Forms.Label();
  this.cmb_Parity = new System.Windows.Forms.ComboBox();
        this.label10 = new System.Windows.Forms.Label();
  this.cmb_StopBits = new System.Windows.Forms.ComboBox();
 this.label9 = new System.Windows.Forms.Label();
 this.cmb_DataBits = new System.Windows.Forms.ComboBox();
        this.label8 = new System.Windows.Forms.Label();
            this.cmb_BaudRate = new System.Windows.Forms.ComboBox();
        this.label7 = new System.Windows.Forms.Label();
            this.cmb_PortName = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
        this.chk_Enabled = new System.Windows.Forms.CheckBox();
    this.cmb_Mode = new System.Windows.Forms.ComboBox();
this.label5 = new System.Windows.Forms.Label();
            this.cmb_Type = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
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
            this.panel1.Controls.Add(this.btn_AddOpt);
       this.panel1.Controls.Add(this.btn_AddFgen);
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
    // btn_AddOpt
          // 
            this.btn_AddOpt.Location = new System.Drawing.Point(130, 7);
      this.btn_AddOpt.Name = "btn_AddOpt";
 this.btn_AddOpt.Size = new System.Drawing.Size(108, 30);
        this.btn_AddOpt.TabIndex = 1;
            this.btn_AddOpt.Text = "添加奥普特";
     this.btn_AddOpt.UseVisualStyleBackColor = true;
// 
            // btn_AddFgen
// 
   this.btn_AddFgen.Location = new System.Drawing.Point(12, 7);
        this.btn_AddFgen.Name = "btn_AddFgen";
      this.btn_AddFgen.Size = new System.Drawing.Size(108, 30);
      this.btn_AddFgen.TabIndex = 0;
            this.btn_AddFgen.Text = "添加孚根";
            this.btn_AddFgen.UseVisualStyleBackColor = true;
   // 
            // groupBox_Config
    // 
            this.groupBox_Config.Controls.Add(this.btn_Test);
    this.groupBox_Config.Controls.Add(this.txt_Remark);
     this.groupBox_Config.Controls.Add(this.label12);
      this.groupBox_Config.Controls.Add(this.cmb_ChannelCount);
  this.groupBox_Config.Controls.Add(this.label11);
  this.groupBox_Config.Controls.Add(this.cmb_Parity);
        this.groupBox_Config.Controls.Add(this.label10);
    this.groupBox_Config.Controls.Add(this.cmb_StopBits);
          this.groupBox_Config.Controls.Add(this.label9);
  this.groupBox_Config.Controls.Add(this.cmb_DataBits);
     this.groupBox_Config.Controls.Add(this.label8);
            this.groupBox_Config.Controls.Add(this.cmb_BaudRate);
  this.groupBox_Config.Controls.Add(this.label7);
            this.groupBox_Config.Controls.Add(this.cmb_PortName);
        this.groupBox_Config.Controls.Add(this.label6);
      this.groupBox_Config.Controls.Add(this.chk_Enabled);
            this.groupBox_Config.Controls.Add(this.cmb_Mode);
      this.groupBox_Config.Controls.Add(this.label5);
       this.groupBox_Config.Controls.Add(this.cmb_Type);
            this.groupBox_Config.Controls.Add(this.label4);
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
            this.groupBox_Config.Text = "配置参数";
    // 
            // btn_Test
// 
          this.btn_Test.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
       this.btn_Test.Location = new System.Drawing.Point(612, 25);
       this.btn_Test.Name = "btn_Test";
            this.btn_Test.Size = new System.Drawing.Size(105, 30);
       this.btn_Test.TabIndex = 27;
            this.btn_Test.Text = "打开测试";
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
            // cmb_ChannelCount
  // 
this.cmb_ChannelCount.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmb_ChannelCount.FormattingEnabled = true;
        this.cmb_ChannelCount.Location = new System.Drawing.Point(120, 462);
    this.cmb_ChannelCount.Name = "cmb_ChannelCount";
            this.cmb_ChannelCount.Size = new System.Drawing.Size(207, 25);
          this.cmb_ChannelCount.TabIndex = 24;
            // 
     // label11
          // 
        this.label11.AutoSize = true;
   this.label11.Location = new System.Drawing.Point(13, 465);
     this.label11.Name = "label11";
      this.label11.Size = new System.Drawing.Size(68, 17);
     this.label11.TabIndex = 23;
            this.label11.Text = "通道数量：";
            // 
            // cmb_Parity
            // 
            this.cmb_Parity.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
    this.cmb_Parity.FormattingEnabled = true;
    this.cmb_Parity.Location = new System.Drawing.Point(510, 422);
            this.cmb_Parity.Name = "cmb_Parity";
            this.cmb_Parity.Size = new System.Drawing.Size(207, 25);
this.cmb_Parity.TabIndex = 22;
          // 
     // label10
    // 
   this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(372, 425);
      this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(56, 17);
          this.label10.TabIndex = 21;
    this.label10.Text = "校验位：";
            // 
         // cmb_StopBits
 // 
     this.cmb_StopBits.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
 this.cmb_StopBits.FormattingEnabled = true;
          this.cmb_StopBits.Location = new System.Drawing.Point(120, 422);
     this.cmb_StopBits.Name = "cmb_StopBits";
            this.cmb_StopBits.Size = new System.Drawing.Size(207, 25);
   this.cmb_StopBits.TabIndex = 20;
      // 
        // label9
            // 
            this.label9.AutoSize = true;
        this.label9.Location = new System.Drawing.Point(13, 425);
          this.label9.Name = "label9";
       this.label9.Size = new System.Drawing.Size(56, 17);
        this.label9.TabIndex = 18;
            this.label9.Text = "停止位：";
        // 
         // cmb_DataBits
          // 
        this.cmb_DataBits.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
this.cmb_DataBits.FormattingEnabled = true;
      this.cmb_DataBits.Location = new System.Drawing.Point(510, 382);
         this.cmb_DataBits.Name = "cmb_DataBits";
            this.cmb_DataBits.Size = new System.Drawing.Size(207, 25);
            this.cmb_DataBits.TabIndex = 17;
            // 
     // label8
            // 
         this.label8.AutoSize = true;
   this.label8.Location = new System.Drawing.Point(372, 385);
       this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(56, 17);
   this.label8.TabIndex = 16;
            this.label8.Text = "数据位：";
  // 
          // cmb_BaudRate
      // 
            this.cmb_BaudRate.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
         this.cmb_BaudRate.FormattingEnabled = true;
   this.cmb_BaudRate.Location = new System.Drawing.Point(120, 382);
      this.cmb_BaudRate.Name = "cmb_BaudRate";
            this.cmb_BaudRate.Size = new System.Drawing.Size(207, 25);
       this.cmb_BaudRate.TabIndex = 14;
    // 
            // label7
            // 
       this.label7.AutoSize = true;
       this.label7.Location = new System.Drawing.Point(13, 385);
            this.label7.Name = "label7";
         this.label7.Size = new System.Drawing.Size(56, 17);
            this.label7.TabIndex = 13;
            this.label7.Text = "波特率：";
       // 
            // cmb_PortName
     // 
 this.cmb_PortName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
this.cmb_PortName.FormattingEnabled = true;
            this.cmb_PortName.Location = new System.Drawing.Point(120, 342);
            this.cmb_PortName.Name = "cmb_PortName";
        this.cmb_PortName.Size = new System.Drawing.Size(207, 25);
            this.cmb_PortName.TabIndex = 12;
            // 
    // label6
    // 
            this.label6.AutoSize = true;
 this.label6.Location = new System.Drawing.Point(13, 345);
        this.label6.Name = "label6";
    this.label6.Size = new System.Drawing.Size(56, 17);
    this.label6.TabIndex = 11;
        this.label6.Text = "串口号：";
      // 
            // chk_Enabled
    // 
            this.chk_Enabled.AutoSize = true;
      this.chk_Enabled.Checked = true;
   this.chk_Enabled.CheckState = System.Windows.Forms.CheckState.Checked;
    this.chk_Enabled.Location = new System.Drawing.Point(120, 303);
      this.chk_Enabled.Name = "chk_Enabled";
         this.chk_Enabled.Size = new System.Drawing.Size(51, 21);
            this.chk_Enabled.TabIndex = 8;
            this.chk_Enabled.Text = "启用";
   this.chk_Enabled.UseVisualStyleBackColor = true;
            // 
            // cmb_Mode
       // 
     this.cmb_Mode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
         this.cmb_Mode.FormattingEnabled = true;
            this.cmb_Mode.Location = new System.Drawing.Point(120, 262);
       this.cmb_Mode.Name = "cmb_Mode";
        this.cmb_Mode.Size = new System.Drawing.Size(207, 25);
    this.cmb_Mode.TabIndex = 7;
      // 
     // label5
   // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(13, 265);
            this.label5.Name = "label5";
    this.label5.Size = new System.Drawing.Size(92, 17);
     this.label5.TabIndex = 6;
   this.label5.Text = "控制器模式：";
            // 
            // cmb_Type
        // 
      this.cmb_Type.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
        this.cmb_Type.FormattingEnabled = true;
            this.cmb_Type.Location = new System.Drawing.Point(120, 222);
    this.cmb_Type.Name = "cmb_Type";
            this.cmb_Type.Size = new System.Drawing.Size(207, 25);
    this.cmb_Type.TabIndex = 5;
            // 
          // label4
            // 
      this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(13, 225);
 this.label4.Name = "label4";
  this.label4.Size = new System.Drawing.Size(92, 17);
            this.label4.TabIndex = 4;
        this.label4.Text = "控制器类型：";
    // 
         // lbl_Type
         // 
            this.lbl_Type.AutoSize = true;
  this.lbl_Type.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lbl_Type.Location = new System.Drawing.Point(116, 142);
   this.lbl_Type.Name = "lbl_Type";
    this.lbl_Type.Size = new System.Drawing.Size(65, 17);
            this.lbl_Type.TabIndex = 3;
this.lbl_Type.Text = "孚根控制器";
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
   // Frm_LightConfig
            // 
this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
    this.ClientSize = new System.Drawing.Size(984, 661);
  this.Controls.Add(this.splitContainer1);
     this.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
      this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "Frm_LightConfig";
    this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
      this.Text = "光源配置";
    this.splitContainer1.Panel1.ResumeLayout(false);
        this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
       this.panel1.ResumeLayout(false);
        this.groupBox_Config.ResumeLayout(false);
            this.groupBox_Config.PerformLayout();
     this.panel2.ResumeLayout(false);
     this.ResumeLayout(false);
        }

   #endregion

      private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ListBox listBox_Configs;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btn_Delete;
        private System.Windows.Forms.Button btn_AddOpt;
        private System.Windows.Forms.Button btn_AddFgen;
  private System.Windows.Forms.GroupBox groupBox_Config;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button btn_Save;
  private System.Windows.Forms.TextBox txt_Name;
  private System.Windows.Forms.Label label1;
      private System.Windows.Forms.Label lbl_Type;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cmb_Type;
        private System.Windows.Forms.Label label4;
     private System.Windows.Forms.ComboBox cmb_Mode;
   private System.Windows.Forms.Label label5;
    private System.Windows.Forms.CheckBox chk_Enabled;
        private System.Windows.Forms.ComboBox cmb_PortName;
     private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox cmb_BaudRate;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ComboBox cmb_DataBits;
    private System.Windows.Forms.Label label8;
 private System.Windows.Forms.ComboBox cmb_StopBits;
        private System.Windows.Forms.Label label9;
  private System.Windows.Forms.ComboBox cmb_Parity;
 private System.Windows.Forms.Label label10;
        private System.Windows.Forms.ComboBox cmb_ChannelCount;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox txt_Remark;
        private System.Windows.Forms.Label label12;
    private System.Windows.Forms.Button btn_Test;
    }
}
