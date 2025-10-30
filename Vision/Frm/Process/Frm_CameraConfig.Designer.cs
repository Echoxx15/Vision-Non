namespace Vision.Frm.Process
{
    partial class Frm_CameraConfig
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
      this.panel1 = new System.Windows.Forms.Panel();
      this.nUD_Height = new System.Windows.Forms.NumericUpDown();
      this.label10 = new System.Windows.Forms.Label();
      this.nUD_Width = new System.Windows.Forms.NumericUpDown();
      this.label12 = new System.Windows.Forms.Label();
      this.rb_Hardware = new System.Windows.Forms.RadioButton();
      this.rb_SoftWare = new System.Windows.Forms.RadioButton();
      this.label7 = new System.Windows.Forms.Label();
      this.nUD_TriggerCount = new System.Windows.Forms.NumericUpDown();
      this.label8 = new System.Windows.Forms.Label();
      this.label5 = new System.Windows.Forms.Label();
      this.nUD_TimeOut = new System.Windows.Forms.NumericUpDown();
      this.label6 = new System.Windows.Forms.Label();
      this.label3 = new System.Windows.Forms.Label();
      this.nUD_Gain = new System.Windows.Forms.NumericUpDown();
      this.label4 = new System.Windows.Forms.Label();
      this.label2 = new System.Windows.Forms.Label();
      this.nUD_ExposureTime = new System.Windows.Forms.NumericUpDown();
      this.label1 = new System.Windows.Forms.Label();
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
      this.splitContainer1.Panel1.SuspendLayout();
      this.splitContainer1.SuspendLayout();
      this.panel1.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.nUD_Height)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.nUD_Width)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.nUD_TriggerCount)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.nUD_TimeOut)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.nUD_Gain)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.nUD_ExposureTime)).BeginInit();
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
      this.splitContainer1.Panel1.Controls.Add(this.panel1);
      this.splitContainer1.Size = new System.Drawing.Size(800, 708);
      this.splitContainer1.SplitterDistance = 303;
      this.splitContainer1.TabIndex = 0;
      // 
      // panel1
      // 
      this.panel1.Controls.Add(this.nUD_Height);
      this.panel1.Controls.Add(this.label10);
      this.panel1.Controls.Add(this.nUD_Width);
      this.panel1.Controls.Add(this.label12);
      this.panel1.Controls.Add(this.rb_Hardware);
      this.panel1.Controls.Add(this.rb_SoftWare);
      this.panel1.Controls.Add(this.label7);
      this.panel1.Controls.Add(this.nUD_TriggerCount);
      this.panel1.Controls.Add(this.label8);
      this.panel1.Controls.Add(this.label5);
      this.panel1.Controls.Add(this.nUD_TimeOut);
      this.panel1.Controls.Add(this.label6);
      this.panel1.Controls.Add(this.label3);
      this.panel1.Controls.Add(this.nUD_Gain);
      this.panel1.Controls.Add(this.label4);
      this.panel1.Controls.Add(this.label2);
      this.panel1.Controls.Add(this.nUD_ExposureTime);
      this.panel1.Controls.Add(this.label1);
    
      // 添加确定和取消按钮
      var btnOK = new System.Windows.Forms.Button
      {
        Text = "确定",
        DialogResult = System.Windows.Forms.DialogResult.OK,
        Location = new System.Drawing.Point(48, 450),
        Size = new System.Drawing.Size(90, 35)
      };
      var btnCancel = new System.Windows.Forms.Button
      {
        Text = "取消",
        DialogResult = System.Windows.Forms.DialogResult.Cancel,
     Location = new System.Drawing.Point(160, 450),
        Size = new System.Drawing.Size(90, 35)
      };
      this.panel1.Controls.Add(btnOK);
      this.panel1.Controls.Add(btnCancel);
      this.AcceptButton = btnOK;
      this.CancelButton = btnCancel;
      
      this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.panel1.Location = new System.Drawing.Point(0, 0);
      this.panel1.Name = "panel1";
      this.panel1.Size = new System.Drawing.Size(303, 708);
      this.panel1.TabIndex = 0;
      this.panel1.Text = "panel1";
      // 
      // nUD_Height
      // 
      this.nUD_Height.Location = new System.Drawing.Point(108, 374);
      this.nUD_Height.Maximum = new decimal(new int[] {
            99999,
            0,
            0,
            0});
      this.nUD_Height.Name = "nUD_Height";
      this.nUD_Height.Size = new System.Drawing.Size(120, 28);
      this.nUD_Height.TabIndex = 18;
      // 
      // label10
      // 
      this.label10.Location = new System.Drawing.Point(12, 376);
      this.label10.Name = "label10";
      this.label10.Size = new System.Drawing.Size(86, 23);
      this.label10.TabIndex = 17;
      this.label10.Text = "图像高度";
      this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
      // 
      // nUD_Width
      // 
      this.nUD_Width.Location = new System.Drawing.Point(108, 331);
      this.nUD_Width.Maximum = new decimal(new int[] {
            99999999,
            0,
            0,
            0});
      this.nUD_Width.Name = "nUD_Width";
      this.nUD_Width.Size = new System.Drawing.Size(120, 28);
      this.nUD_Width.TabIndex = 15;
      // 
      // label12
      // 
      this.label12.Location = new System.Drawing.Point(12, 333);
      this.label12.Name = "label12";
      this.label12.Size = new System.Drawing.Size(86, 23);
      this.label12.TabIndex = 14;
      this.label12.Text = "图像宽度";
      this.label12.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
      // 
      // rb_Hardware
      // 
      this.rb_Hardware.AutoSize = true;
      this.rb_Hardware.Location = new System.Drawing.Point(170, 86);
      this.rb_Hardware.Name = "rb_Hardware";
      this.rb_Hardware.Size = new System.Drawing.Size(87, 22);
      this.rb_Hardware.TabIndex = 13;
      this.rb_Hardware.TabStop = true;
      this.rb_Hardware.Text = "硬触发";
      this.rb_Hardware.UseVisualStyleBackColor = true;
      // 
      // rb_SoftWare
      // 
      this.rb_SoftWare.AutoSize = true;
      this.rb_SoftWare.Location = new System.Drawing.Point(51, 86);
      this.rb_SoftWare.Name = "rb_SoftWare";
      this.rb_SoftWare.Size = new System.Drawing.Size(87, 22);
      this.rb_SoftWare.TabIndex = 12;
      this.rb_SoftWare.TabStop = true;
      this.rb_SoftWare.Text = "软触发";
      this.rb_SoftWare.UseVisualStyleBackColor = true;
      // 
      // label7
      // 
      this.label7.Location = new System.Drawing.Point(234, 292);
      this.label7.Name = "label7";
      this.label7.Size = new System.Drawing.Size(50, 23);
      this.label7.TabIndex = 11;
      this.label7.Text = "次";
      this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
      // 
      // nUD_TriggerCount
      // 
      this.nUD_TriggerCount.Location = new System.Drawing.Point(108, 290);
      this.nUD_TriggerCount.Name = "nUD_TriggerCount";
      this.nUD_TriggerCount.Size = new System.Drawing.Size(120, 28);
      this.nUD_TriggerCount.TabIndex = 10;
      // 
      // label8
      // 
      this.label8.Location = new System.Drawing.Point(12, 292);
      this.label8.Name = "label8";
      this.label8.Size = new System.Drawing.Size(86, 23);
      this.label8.TabIndex = 9;
      this.label8.Text = "触发次数";
      this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
      // 
      // label5
      // 
      this.label5.Location = new System.Drawing.Point(234, 249);
      this.label5.Name = "label5";
      this.label5.Size = new System.Drawing.Size(50, 23);
      this.label5.TabIndex = 8;
      this.label5.Text = "ms";
      this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
      // 
      // nUD_TimeOut
      // 
      this.nUD_TimeOut.Location = new System.Drawing.Point(108, 247);
      this.nUD_TimeOut.Maximum = new decimal(new int[] {
            99999999,
            0,
            0,
            0});
      this.nUD_TimeOut.Name = "nUD_TimeOut";
      this.nUD_TimeOut.Size = new System.Drawing.Size(120, 28);
      this.nUD_TimeOut.TabIndex = 7;
      // 
      // label6
      // 
      this.label6.Location = new System.Drawing.Point(12, 249);
      this.label6.Name = "label6";
      this.label6.Size = new System.Drawing.Size(86, 23);
      this.label6.TabIndex = 6;
      this.label6.Text = "超时时间";
      this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
      // 
      // label3
      // 
      this.label3.Location = new System.Drawing.Point(234, 208);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(50, 23);
      this.label3.TabIndex = 5;
      this.label3.Text = "db";
      this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
      // 
      // nUD_Gain
      // 
      this.nUD_Gain.DecimalPlaces = 5;
      this.nUD_Gain.Location = new System.Drawing.Point(108, 206);
      this.nUD_Gain.Maximum = new decimal(new int[] {
            99999999,
            0,
            0,
            0});
      this.nUD_Gain.Name = "nUD_Gain";
      this.nUD_Gain.Size = new System.Drawing.Size(120, 28);
      this.nUD_Gain.TabIndex = 4;
      // 
      // label4
      // 
      this.label4.Location = new System.Drawing.Point(48, 208);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(50, 23);
      this.label4.TabIndex = 3;
      this.label4.Text = "增益";
      this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
      // 
      // label2
      // 
      this.label2.Location = new System.Drawing.Point(234, 164);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(50, 23);
      this.label2.TabIndex = 2;
      this.label2.Text = "us";
      this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
      // 
      // nUD_ExposureTime
      // 
      this.nUD_ExposureTime.DecimalPlaces = 5;
      this.nUD_ExposureTime.Location = new System.Drawing.Point(108, 162);
      this.nUD_ExposureTime.Maximum = new decimal(new int[] {
            99999999,
            0,
            0,
            0});
      this.nUD_ExposureTime.Name = "nUD_ExposureTime";
      this.nUD_ExposureTime.Size = new System.Drawing.Size(120, 28);
      this.nUD_ExposureTime.TabIndex = 1;
      // 
      // label1
      // 
      this.label1.Location = new System.Drawing.Point(48, 164);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(50, 23);
      this.label1.TabIndex = 0;
      this.label1.Text = "曝光";
      this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
      // 
      // Frm_Camera
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.BackColor = System.Drawing.SystemColors.ActiveCaption;
      this.ClientSize = new System.Drawing.Size(800, 708);
      this.Controls.Add(this.splitContainer1);
      this.Name = "Frm_Camera";
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
      this.Text = "相机配置";
      this.splitContainer1.Panel1.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
      this.splitContainer1.ResumeLayout(false);
      this.panel1.ResumeLayout(false);
      this.panel1.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.nUD_Height)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.nUD_Width)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.nUD_TriggerCount)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.nUD_TimeOut)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.nUD_Gain)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.nUD_ExposureTime)).EndInit();
      this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown nUD_ExposureTime;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown nUD_Gain;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.NumericUpDown nUD_TriggerCount;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown nUD_TimeOut;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.RadioButton rb_Hardware;
        private System.Windows.Forms.RadioButton rb_SoftWare;
        private System.Windows.Forms.NumericUpDown nUD_Height;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.NumericUpDown nUD_Width;
        private System.Windows.Forms.Label label12;
    }
}