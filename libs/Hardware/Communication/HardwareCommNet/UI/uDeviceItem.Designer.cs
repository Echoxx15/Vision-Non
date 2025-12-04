namespace HardwareCommNet.UI
{
    partial class uDeviceItem
    {
        /// <summary> 
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.sw_Connect = new AntdUI.Switch();
            this.btn_DevName = new AntdUI.Button();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 75F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.Controls.Add(this.sw_Connect, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.btn_DevName, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(280, 40);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // sw_Connect
            // 
            this.sw_Connect.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.sw_Connect.Location = new System.Drawing.Point(220, 8);
            this.sw_Connect.Name = "sw_Connect";
            this.sw_Connect.Size = new System.Drawing.Size(50, 24);
            this.sw_Connect.TabIndex = 1;
            this.sw_Connect.CheckedChanged += new AntdUI.BoolEventHandler(this.sw_Connect_CheckedChanged);
            // 
            // btn_DevName
            // 
            this.btn_DevName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btn_DevName.ForeColor = System.Drawing.Color.White;
            this.btn_DevName.Location = new System.Drawing.Point(3, 3);
            this.btn_DevName.Name = "btn_DevName";
            this.btn_DevName.Size = new System.Drawing.Size(204, 34);
            this.btn_DevName.TabIndex = 2;
            this.btn_DevName.Text = "button1";
            this.btn_DevName.Type = AntdUI.TTypeMini.Error;
            // 
            // uDeviceItem
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.Controls.Add(this.tableLayoutPanel1);
            this.Margin = new System.Windows.Forms.Padding(0, 0, 0, 2);
            this.Name = "uDeviceItem";
            this.Size = new System.Drawing.Size(280, 40);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private AntdUI.Switch sw_Connect;
        private AntdUI.Button btn_DevName;
    }
}
