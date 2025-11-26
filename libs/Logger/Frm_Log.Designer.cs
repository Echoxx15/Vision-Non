namespace Logger
{
    partial class Frm_Log
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
      this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
      this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
      this.btn_Info = new System.Windows.Forms.Button();
      this.btn_Warning = new System.Windows.Forms.Button();
      this.btn_Error = new System.Windows.Forms.Button();
      this.dataGridView1 = new System.Windows.Forms.DataGridView();
      this.tableLayoutPanel1.SuspendLayout();
      this.flowLayoutPanel1.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
      this.SuspendLayout();
      // 
      // tableLayoutPanel1
      // 
      this.tableLayoutPanel1.ColumnCount = 1;
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel1, 0, 0);
      this.tableLayoutPanel1.Controls.Add(this.dataGridView1, 0, 1);
      this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
      this.tableLayoutPanel1.Name = "tableLayoutPanel1";
      this.tableLayoutPanel1.RowCount = 2;
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 72F));
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel1.Size = new System.Drawing.Size(563, 747);
      this.tableLayoutPanel1.TabIndex = 0;
      // 
      // flowLayoutPanel1
      // 
      this.flowLayoutPanel1.BackColor = System.Drawing.Color.Black;
      this.flowLayoutPanel1.Controls.Add(this.btn_Info);
      this.flowLayoutPanel1.Controls.Add(this.btn_Warning);
      this.flowLayoutPanel1.Controls.Add(this.btn_Error);
      this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.flowLayoutPanel1.Location = new System.Drawing.Point(3, 3);
      this.flowLayoutPanel1.Name = "flowLayoutPanel1";
      this.flowLayoutPanel1.Size = new System.Drawing.Size(557, 66);
      this.flowLayoutPanel1.TabIndex = 2;
      // 
      // btn_Info
      // 
      this.btn_Info.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
      this.btn_Info.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
      this.btn_Info.ForeColor = System.Drawing.Color.White;
      this.btn_Info.Image = global::Logger.Properties.Resources.信息;
      this.btn_Info.Location = new System.Drawing.Point(3, 3);
      this.btn_Info.Name = "btn_Info";
      this.btn_Info.Size = new System.Drawing.Size(148, 60);
      this.btn_Info.TabIndex = 4;
      this.btn_Info.Text = "信息";
      this.btn_Info.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
      this.btn_Info.UseVisualStyleBackColor = false;
      // 
      // btn_Warning
      // 
      this.btn_Warning.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
      this.btn_Warning.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
      this.btn_Warning.ForeColor = System.Drawing.Color.White;
      this.btn_Warning.Image = global::Logger.Properties.Resources.警告;
      this.btn_Warning.Location = new System.Drawing.Point(157, 3);
      this.btn_Warning.Name = "btn_Warning";
      this.btn_Warning.Size = new System.Drawing.Size(148, 60);
      this.btn_Warning.TabIndex = 5;
      this.btn_Warning.Text = "警告";
      this.btn_Warning.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
      this.btn_Warning.UseVisualStyleBackColor = false;
      // 
      // btn_Error
      // 
      this.btn_Error.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
      this.btn_Error.ForeColor = System.Drawing.Color.White;
      this.btn_Error.Image = global::Logger.Properties.Resources.错误;
      this.btn_Error.Location = new System.Drawing.Point(311, 3);
      this.btn_Error.Name = "btn_Error";
      this.btn_Error.Size = new System.Drawing.Size(148, 60);
      this.btn_Error.TabIndex = 3;
      this.btn_Error.Text = "错误";
      this.btn_Error.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
      this.btn_Error.UseVisualStyleBackColor = false;
      // 
      // dataGridView1
      // 
      this.dataGridView1.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
      this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.dataGridView1.Location = new System.Drawing.Point(3, 75);
      this.dataGridView1.Name = "dataGridView1";
      this.dataGridView1.RowHeadersWidth = 62;
      this.dataGridView1.RowTemplate.Height = 30;
      this.dataGridView1.Size = new System.Drawing.Size(557, 669);
      this.dataGridView1.TabIndex = 3;
      // 
      // Frm_Log
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.tableLayoutPanel1);
      this.Name = "Frm_Log";
      this.Size = new System.Drawing.Size(563, 747);
      this.tableLayoutPanel1.ResumeLayout(false);
      this.flowLayoutPanel1.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
      this.ResumeLayout(false);

        }

        #endregion
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
    private System.Windows.Forms.Button btn_Info;
    private System.Windows.Forms.Button btn_Warning;
    private System.Windows.Forms.Button btn_Error;
    private System.Windows.Forms.DataGridView dataGridView1;
  }
}
