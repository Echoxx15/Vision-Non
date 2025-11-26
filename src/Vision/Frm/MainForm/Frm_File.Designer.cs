namespace Vision.Frm.MainForm
{
    partial class Frm_File
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
      this.groupBox2 = new System.Windows.Forms.GroupBox();
      this.label9 = new System.Windows.Forms.Label();
      this.dtp_PollTime1 = new System.Windows.Forms.DateTimePicker();
      this.dtp_PollTime2 = new System.Windows.Forms.DateTimePicker();
      this.label11 = new System.Windows.Forms.Label();
      this.label10 = new System.Windows.Forms.Label();
      this.nud_Threshold = new System.Windows.Forms.NumericUpDown();
      this.label7 = new System.Windows.Forms.Label();
      this.label8 = new System.Windows.Forms.Label();
      this.label6 = new System.Windows.Forms.Label();
      this.rtn_false = new System.Windows.Forms.RadioButton();
      this.rtn_true = new System.Windows.Forms.RadioButton();
      this.groupBox1 = new System.Windows.Forms.GroupBox();
      this.label23 = new System.Windows.Forms.Label();
      this.cmb_ImageToolType = new System.Windows.Forms.ComboBox();
      this.label4 = new System.Windows.Forms.Label();
      this.label5 = new System.Windows.Forms.Label();
      this.txt_days_Deal = new System.Windows.Forms.TextBox();
      this.chk_SaveOKNG = new System.Windows.Forms.CheckBox();
      this.label3 = new System.Windows.Forms.Label();
      this.cmb_ImageType = new System.Windows.Forms.ComboBox();
      this.chk_Delete = new System.Windows.Forms.CheckBox();
      this.label2 = new System.Windows.Forms.Label();
      this.label12 = new System.Windows.Forms.Label();
      this.txt_days = new System.Windows.Forms.TextBox();
      this.chk_SaveDealImage = new System.Windows.Forms.CheckBox();
      this.chk_SaveRawImage = new System.Windows.Forms.CheckBox();
      this.btn_SaveConfig = new System.Windows.Forms.Button();
      this.groupBox3 = new System.Windows.Forms.GroupBox();
      this.btn_Select = new System.Windows.Forms.Button();
      this.txt_Path = new System.Windows.Forms.TextBox();
      this.label1 = new System.Windows.Forms.Label();
      this.groupBox2.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.nud_Threshold)).BeginInit();
      this.groupBox1.SuspendLayout();
      this.groupBox3.SuspendLayout();
      this.SuspendLayout();
      // 
      // groupBox2
      // 
      this.groupBox2.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
      this.groupBox2.Controls.Add(this.label9);
      this.groupBox2.Controls.Add(this.dtp_PollTime1);
      this.groupBox2.Controls.Add(this.dtp_PollTime2);
      this.groupBox2.Controls.Add(this.label11);
      this.groupBox2.Controls.Add(this.label10);
      this.groupBox2.Controls.Add(this.nud_Threshold);
      this.groupBox2.Controls.Add(this.label7);
      this.groupBox2.Controls.Add(this.label8);
      this.groupBox2.Controls.Add(this.label6);
      this.groupBox2.Controls.Add(this.rtn_false);
      this.groupBox2.Controls.Add(this.rtn_true);
      this.groupBox2.Location = new System.Drawing.Point(669, 268);
      this.groupBox2.Margin = new System.Windows.Forms.Padding(4);
      this.groupBox2.Name = "groupBox2";
      this.groupBox2.Padding = new System.Windows.Forms.Padding(4);
      this.groupBox2.Size = new System.Drawing.Size(411, 236);
      this.groupBox2.TabIndex = 12;
      this.groupBox2.TabStop = false;
      this.groupBox2.Text = "磁盘报警设置";
      // 
      // label9
      // 
      this.label9.AutoSize = true;
      this.label9.Location = new System.Drawing.Point(10, 201);
      this.label9.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
      this.label9.Name = "label9";
      this.label9.Size = new System.Drawing.Size(260, 18);
      this.label9.TabIndex = 38;
      this.label9.Text = "注：内存最大值包含当前输入值";
      // 
      // dtp_PollTime1
      // 
      this.dtp_PollTime1.CustomFormat = "HH:mm:ss ";
      this.dtp_PollTime1.Format = System.Windows.Forms.DateTimePickerFormat.Time;
      this.dtp_PollTime1.Location = new System.Drawing.Point(210, 110);
      this.dtp_PollTime1.Margin = new System.Windows.Forms.Padding(4);
      this.dtp_PollTime1.Name = "dtp_PollTime1";
      this.dtp_PollTime1.ShowUpDown = true;
      this.dtp_PollTime1.Size = new System.Drawing.Size(106, 28);
      this.dtp_PollTime1.TabIndex = 37;
      this.dtp_PollTime1.Value = new System.DateTime(2022, 4, 19, 8, 0, 0, 0);
      // 
      // dtp_PollTime2
      // 
      this.dtp_PollTime2.CustomFormat = "HH:mm:ss ";
      this.dtp_PollTime2.Format = System.Windows.Forms.DateTimePickerFormat.Time;
      this.dtp_PollTime2.Location = new System.Drawing.Point(210, 152);
      this.dtp_PollTime2.Margin = new System.Windows.Forms.Padding(4);
      this.dtp_PollTime2.Name = "dtp_PollTime2";
      this.dtp_PollTime2.ShowUpDown = true;
      this.dtp_PollTime2.Size = new System.Drawing.Size(106, 28);
      this.dtp_PollTime2.TabIndex = 36;
      this.dtp_PollTime2.Value = new System.DateTime(2022, 4, 19, 20, 0, 0, 0);
      // 
      // label11
      // 
      this.label11.AutoSize = true;
      this.label11.Location = new System.Drawing.Point(10, 159);
      this.label11.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
      this.label11.Name = "label11";
      this.label11.Size = new System.Drawing.Size(107, 18);
      this.label11.TabIndex = 16;
      this.label11.Text = "检测时间2：";
      // 
      // label10
      // 
      this.label10.AutoSize = true;
      this.label10.Location = new System.Drawing.Point(10, 117);
      this.label10.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
      this.label10.Name = "label10";
      this.label10.Size = new System.Drawing.Size(107, 18);
      this.label10.TabIndex = 15;
      this.label10.Text = "检测时间1：";
      // 
      // nud_Threshold
      // 
      this.nud_Threshold.Location = new System.Drawing.Point(210, 68);
      this.nud_Threshold.Margin = new System.Windows.Forms.Padding(4);
      this.nud_Threshold.Maximum = new decimal(new int[] {
            8000000,
            0,
            0,
            0});
      this.nud_Threshold.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
      this.nud_Threshold.Name = "nud_Threshold";
      this.nud_Threshold.Size = new System.Drawing.Size(108, 28);
      this.nud_Threshold.TabIndex = 13;
      this.nud_Threshold.Value = new decimal(new int[] {
            3000,
            0,
            0,
            0});
      // 
      // label7
      // 
      this.label7.AutoSize = true;
      this.label7.Location = new System.Drawing.Point(326, 75);
      this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
      this.label7.Name = "label7";
      this.label7.Size = new System.Drawing.Size(80, 18);
      this.label7.TabIndex = 12;
      this.label7.Text = "单位/(M)";
      // 
      // label8
      // 
      this.label8.AutoSize = true;
      this.label8.Location = new System.Drawing.Point(10, 75);
      this.label8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
      this.label8.Name = "label8";
      this.label8.Size = new System.Drawing.Size(98, 18);
      this.label8.TabIndex = 11;
      this.label8.Text = "报警阈值：";
      // 
      // label6
      // 
      this.label6.AutoSize = true;
      this.label6.Location = new System.Drawing.Point(10, 33);
      this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
      this.label6.Name = "label6";
      this.label6.Size = new System.Drawing.Size(170, 18);
      this.label6.TabIndex = 1;
      this.label6.Text = "磁盘检测是否开启：";
      // 
      // rtn_false
      // 
      this.rtn_false.AutoSize = true;
      this.rtn_false.Checked = true;
      this.rtn_false.Location = new System.Drawing.Point(272, 34);
      this.rtn_false.Margin = new System.Windows.Forms.Padding(4);
      this.rtn_false.Name = "rtn_false";
      this.rtn_false.Size = new System.Drawing.Size(51, 22);
      this.rtn_false.TabIndex = 0;
      this.rtn_false.TabStop = true;
      this.rtn_false.Text = "否";
      this.rtn_false.UseVisualStyleBackColor = true;
      // 
      // rtn_true
      // 
      this.rtn_true.AutoSize = true;
      this.rtn_true.Location = new System.Drawing.Point(210, 33);
      this.rtn_true.Margin = new System.Windows.Forms.Padding(4);
      this.rtn_true.Name = "rtn_true";
      this.rtn_true.Size = new System.Drawing.Size(51, 22);
      this.rtn_true.TabIndex = 0;
      this.rtn_true.Text = "是";
      this.rtn_true.UseVisualStyleBackColor = true;
      // 
      // groupBox1
      // 
      this.groupBox1.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
      this.groupBox1.Controls.Add(this.label23);
      this.groupBox1.Controls.Add(this.cmb_ImageToolType);
      this.groupBox1.Controls.Add(this.label4);
      this.groupBox1.Controls.Add(this.label5);
      this.groupBox1.Controls.Add(this.txt_days_Deal);
      this.groupBox1.Controls.Add(this.chk_SaveOKNG);
      this.groupBox1.Controls.Add(this.label3);
      this.groupBox1.Controls.Add(this.cmb_ImageType);
      this.groupBox1.Controls.Add(this.chk_Delete);
      this.groupBox1.Controls.Add(this.label2);
      this.groupBox1.Controls.Add(this.label12);
      this.groupBox1.Controls.Add(this.txt_days);
      this.groupBox1.Controls.Add(this.chk_SaveDealImage);
      this.groupBox1.Controls.Add(this.chk_SaveRawImage);
      this.groupBox1.Location = new System.Drawing.Point(15, 268);
      this.groupBox1.Margin = new System.Windows.Forms.Padding(4);
      this.groupBox1.Name = "groupBox1";
      this.groupBox1.Padding = new System.Windows.Forms.Padding(4);
      this.groupBox1.Size = new System.Drawing.Size(645, 279);
      this.groupBox1.TabIndex = 11;
      this.groupBox1.TabStop = false;
      this.groupBox1.Text = "存储设置";
      // 
      // label23
      // 
      this.label23.AutoSize = true;
      this.label23.Location = new System.Drawing.Point(390, 84);
      this.label23.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
      this.label23.Name = "label23";
      this.label23.Size = new System.Drawing.Size(107, 18);
      this.label23.TabIndex = 19;
      this.label23.Text = "处理图类型:";
      // 
      // cmb_ImageToolType
      // 
      this.cmb_ImageToolType.FormattingEnabled = true;
      this.cmb_ImageToolType.Location = new System.Drawing.Point(504, 79);
      this.cmb_ImageToolType.Margin = new System.Windows.Forms.Padding(4);
      this.cmb_ImageToolType.Name = "cmb_ImageToolType";
      this.cmb_ImageToolType.Size = new System.Drawing.Size(115, 26);
      this.cmb_ImageToolType.TabIndex = 18;
      // 
      // label4
      // 
      this.label4.AutoSize = true;
      this.label4.Location = new System.Drawing.Point(270, 228);
      this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(89, 18);
      this.label4.TabIndex = 15;
      this.label4.Text = "单位/(天)";
      // 
      // label5
      // 
      this.label5.AutoSize = true;
      this.label5.Location = new System.Drawing.Point(3, 228);
      this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
      this.label5.Name = "label5";
      this.label5.Size = new System.Drawing.Size(152, 18);
      this.label5.TabIndex = 14;
      this.label5.Text = "效果图保留时间：";
      // 
      // txt_days_Deal
      // 
      this.txt_days_Deal.Location = new System.Drawing.Point(194, 222);
      this.txt_days_Deal.Margin = new System.Windows.Forms.Padding(4);
      this.txt_days_Deal.Name = "txt_days_Deal";
      this.txt_days_Deal.Size = new System.Drawing.Size(60, 28);
      this.txt_days_Deal.TabIndex = 13;
      // 
      // chk_SaveOKNG
      // 
      this.chk_SaveOKNG.AutoSize = true;
      this.chk_SaveOKNG.Location = new System.Drawing.Point(9, 140);
      this.chk_SaveOKNG.Margin = new System.Windows.Forms.Padding(4);
      this.chk_SaveOKNG.Name = "chk_SaveOKNG";
      this.chk_SaveOKNG.Size = new System.Drawing.Size(214, 22);
      this.chk_SaveOKNG.TabIndex = 12;
      this.chk_SaveOKNG.Text = "区分OK、NG文件夹存储";
      this.chk_SaveOKNG.UseVisualStyleBackColor = true;
      // 
      // label3
      // 
      this.label3.AutoSize = true;
      this.label3.Location = new System.Drawing.Point(410, 33);
      this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(89, 18);
      this.label3.TabIndex = 11;
      this.label3.Text = "原图类型:";
      // 
      // cmb_ImageType
      // 
      this.cmb_ImageType.FormattingEnabled = true;
      this.cmb_ImageType.Location = new System.Drawing.Point(504, 30);
      this.cmb_ImageType.Margin = new System.Windows.Forms.Padding(4);
      this.cmb_ImageType.Name = "cmb_ImageType";
      this.cmb_ImageType.Size = new System.Drawing.Size(115, 26);
      this.cmb_ImageType.TabIndex = 7;
      // 
      // chk_Delete
      // 
      this.chk_Delete.AutoSize = true;
      this.chk_Delete.Location = new System.Drawing.Point(9, 104);
      this.chk_Delete.Margin = new System.Windows.Forms.Padding(4);
      this.chk_Delete.Name = "chk_Delete";
      this.chk_Delete.Size = new System.Drawing.Size(142, 22);
      this.chk_Delete.TabIndex = 10;
      this.chk_Delete.Text = "是否删除图片";
      this.chk_Delete.UseVisualStyleBackColor = true;
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(270, 184);
      this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(89, 18);
      this.label2.TabIndex = 9;
      this.label2.Text = "单位/(天)";
      // 
      // label12
      // 
      this.label12.AutoSize = true;
      this.label12.Location = new System.Drawing.Point(20, 184);
      this.label12.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
      this.label12.Name = "label12";
      this.label12.Size = new System.Drawing.Size(134, 18);
      this.label12.TabIndex = 8;
      this.label12.Text = "原图保留时间：";
      // 
      // txt_days
      // 
      this.txt_days.Location = new System.Drawing.Point(194, 178);
      this.txt_days.Margin = new System.Windows.Forms.Padding(4);
      this.txt_days.Name = "txt_days";
      this.txt_days.Size = new System.Drawing.Size(60, 28);
      this.txt_days.TabIndex = 7;
      // 
      // chk_SaveDealImage
      // 
      this.chk_SaveDealImage.AutoSize = true;
      this.chk_SaveDealImage.Location = new System.Drawing.Point(9, 68);
      this.chk_SaveDealImage.Margin = new System.Windows.Forms.Padding(4);
      this.chk_SaveDealImage.Name = "chk_SaveDealImage";
      this.chk_SaveDealImage.Size = new System.Drawing.Size(160, 22);
      this.chk_SaveDealImage.TabIndex = 5;
      this.chk_SaveDealImage.Text = "保存本地结果图";
      this.chk_SaveDealImage.UseVisualStyleBackColor = true;
      // 
      // chk_SaveRawImage
      // 
      this.chk_SaveRawImage.AutoSize = true;
      this.chk_SaveRawImage.Location = new System.Drawing.Point(9, 33);
      this.chk_SaveRawImage.Margin = new System.Windows.Forms.Padding(4);
      this.chk_SaveRawImage.Name = "chk_SaveRawImage";
      this.chk_SaveRawImage.Size = new System.Drawing.Size(142, 22);
      this.chk_SaveRawImage.TabIndex = 4;
      this.chk_SaveRawImage.Text = "保存本地原图";
      this.chk_SaveRawImage.UseVisualStyleBackColor = true;
      // 
      // btn_SaveConfig
      // 
      this.btn_SaveConfig.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
      this.btn_SaveConfig.Location = new System.Drawing.Point(886, 513);
      this.btn_SaveConfig.Margin = new System.Windows.Forms.Padding(4);
      this.btn_SaveConfig.Name = "btn_SaveConfig";
      this.btn_SaveConfig.Size = new System.Drawing.Size(112, 34);
      this.btn_SaveConfig.TabIndex = 10;
      this.btn_SaveConfig.Text = "保存";
      this.btn_SaveConfig.UseVisualStyleBackColor = true;
      // 
      // groupBox3
      // 
      this.groupBox3.Controls.Add(this.btn_Select);
      this.groupBox3.Controls.Add(this.txt_Path);
      this.groupBox3.Controls.Add(this.label1);
      this.groupBox3.Location = new System.Drawing.Point(21, 12);
      this.groupBox3.Name = "groupBox3";
      this.groupBox3.Size = new System.Drawing.Size(1054, 235);
      this.groupBox3.TabIndex = 13;
      this.groupBox3.TabStop = false;
      this.groupBox3.Text = "图片设置";
      // 
      // btn_Select
      // 
      this.btn_Select.Location = new System.Drawing.Point(817, 44);
      this.btn_Select.Name = "btn_Select";
      this.btn_Select.Size = new System.Drawing.Size(129, 30);
      this.btn_Select.TabIndex = 2;
      this.btn_Select.Text = "选择文件夹";
      this.btn_Select.UseVisualStyleBackColor = true;
      // 
      // txt_Path
      // 
      this.txt_Path.Location = new System.Drawing.Point(152, 45);
      this.txt_Path.Name = "txt_Path";
      this.txt_Path.Size = new System.Drawing.Size(658, 28);
      this.txt_Path.TabIndex = 1;
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(20, 50);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(116, 18);
      this.label1.TabIndex = 0;
      this.label1.Text = "本地存储路径";
      // 
      // Frm_File
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.BackColor = System.Drawing.SystemColors.ActiveCaption;
      this.ClientSize = new System.Drawing.Size(1094, 564);
      this.Controls.Add(this.groupBox3);
      this.Controls.Add(this.groupBox2);
      this.Controls.Add(this.groupBox1);
      this.Controls.Add(this.btn_SaveConfig);
      this.Name = "Frm_File";
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
      this.Text = "文件参数配置";
      this.Load += new System.EventHandler(this.Frm_File_Load);
      this.groupBox2.ResumeLayout(false);
      this.groupBox2.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.nud_Threshold)).EndInit();
      this.groupBox1.ResumeLayout(false);
      this.groupBox1.PerformLayout();
      this.groupBox3.ResumeLayout(false);
      this.groupBox3.PerformLayout();
      this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.DateTimePicker dtp_PollTime1;
        private System.Windows.Forms.DateTimePicker dtp_PollTime2;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.NumericUpDown nud_Threshold;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.RadioButton rtn_false;
        private System.Windows.Forms.RadioButton rtn_true;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.ComboBox cmb_ImageToolType;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txt_days_Deal;
        private System.Windows.Forms.CheckBox chk_SaveOKNG;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cmb_ImageType;
        private System.Windows.Forms.CheckBox chk_Delete;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox txt_days;
        private System.Windows.Forms.CheckBox chk_SaveDealImage;
        private System.Windows.Forms.CheckBox chk_SaveRawImage;
        private System.Windows.Forms.Button btn_SaveConfig;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button btn_Select;
        private System.Windows.Forms.TextBox txt_Path;
        private System.Windows.Forms.Label label1;
    }
}