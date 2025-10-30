namespace Vision.Frm.Solution
{
    partial class Frm_SolutionList
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
            this.tablePanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.panelControl1 = new System.Windows.Forms.Panel();
            this.btn_SetStart = new System.Windows.Forms.Button();
            this.btn_Delete = new System.Windows.Forms.Button();
            this.btn_AddCur = new System.Windows.Forms.Button();
            this.btn_AddNew = new System.Windows.Forms.Button();
            this.btn_Open = new System.Windows.Forms.Button();
            this.dgvSolutions = new System.Windows.Forms.DataGridView();
            this.tablePanel1.SuspendLayout();
            this.panelControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSolutions)).BeginInit();
            this.SuspendLayout();
            // 
            // tablePanel1
            // 
            this.tablePanel1.ColumnCount = 2;
            this.tablePanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 88.67924F));
            this.tablePanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 11.32076F));
            this.tablePanel1.Controls.Add(this.panelControl1, 1, 0);
            this.tablePanel1.Controls.Add(this.dgvSolutions, 0, 0);
            this.tablePanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tablePanel1.Location = new System.Drawing.Point(0, 0);
            this.tablePanel1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tablePanel1.Name = "tablePanel1";
            this.tablePanel1.RowCount = 1;
            this.tablePanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tablePanel1.Size = new System.Drawing.Size(1166, 747);
            this.tablePanel1.TabIndex = 5;
            // 
            // panelControl1
            // 
            this.panelControl1.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.panelControl1.Controls.Add(this.btn_SetStart);
            this.panelControl1.Controls.Add(this.btn_Delete);
            this.panelControl1.Controls.Add(this.btn_AddCur);
            this.panelControl1.Controls.Add(this.btn_AddNew);
            this.panelControl1.Controls.Add(this.btn_Open);
            this.panelControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelControl1.Location = new System.Drawing.Point(1036, 2);
            this.panelControl1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Size = new System.Drawing.Size(127, 743);
            this.panelControl1.TabIndex = 5;
            // 
            // btn_SetStart
            // 
            this.btn_SetStart.BackColor = System.Drawing.Color.SeaGreen;
            this.btn_SetStart.Location = new System.Drawing.Point(13, 179);
            this.btn_SetStart.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btn_SetStart.Name = "btn_SetStart";
            this.btn_SetStart.Size = new System.Drawing.Size(100, 35);
            this.btn_SetStart.TabIndex = 5;
            this.btn_SetStart.Text = "设为默认";
            this.btn_SetStart.UseVisualStyleBackColor = false;
            this.btn_SetStart.Click += new System.EventHandler(this.btn_SetStart_Click);
            // 
            // btn_Delete
            // 
            this.btn_Delete.BackColor = System.Drawing.Color.SeaGreen;
            this.btn_Delete.Location = new System.Drawing.Point(13, 226);
            this.btn_Delete.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btn_Delete.Name = "btn_Delete";
            this.btn_Delete.Size = new System.Drawing.Size(100, 35);
            this.btn_Delete.TabIndex = 4;
            this.btn_Delete.Text = "删除方案";
            this.btn_Delete.UseVisualStyleBackColor = false;
            this.btn_Delete.Click += new System.EventHandler(this.btn_Delete_Click);
            // 
            // btn_AddCur
            // 
            this.btn_AddCur.BackColor = System.Drawing.Color.SeaGreen;
            this.btn_AddCur.Location = new System.Drawing.Point(13, 129);
            this.btn_AddCur.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btn_AddCur.Name = "btn_AddCur";
            this.btn_AddCur.Size = new System.Drawing.Size(100, 35);
            this.btn_AddCur.TabIndex = 3;
            this.btn_AddCur.Text = "复制选中";
            this.btn_AddCur.UseVisualStyleBackColor = false;
            this.btn_AddCur.Click += new System.EventHandler(this.btn_AddCur_Click);
            // 
            // btn_AddNew
            // 
            this.btn_AddNew.BackColor = System.Drawing.Color.SeaGreen;
            this.btn_AddNew.Location = new System.Drawing.Point(13, 79);
            this.btn_AddNew.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btn_AddNew.Name = "btn_AddNew";
            this.btn_AddNew.Size = new System.Drawing.Size(100, 35);
            this.btn_AddNew.TabIndex = 2;
            this.btn_AddNew.Text = "添加空白";
            this.btn_AddNew.UseVisualStyleBackColor = false;
            this.btn_AddNew.Click += new System.EventHandler(this.btn_AddNew_Click);
            // 
            // btn_Open
            // 
            this.btn_Open.BackColor = System.Drawing.Color.SeaGreen;
            this.btn_Open.Location = new System.Drawing.Point(13, 29);
            this.btn_Open.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btn_Open.Name = "btn_Open";
            this.btn_Open.Size = new System.Drawing.Size(100, 35);
            this.btn_Open.TabIndex = 0;
            this.btn_Open.Text = "切换方案";
            this.btn_Open.UseVisualStyleBackColor = false;
            this.btn_Open.Click += new System.EventHandler(this.btn_Open_Click);
            // 
            // dgvSolutions
            // 
            this.dgvSolutions.AllowUserToAddRows = false;
            this.dgvSolutions.AllowUserToDeleteRows = false;
            this.dgvSolutions.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvSolutions.BackgroundColor = System.Drawing.SystemColors.ActiveCaption;
            this.dgvSolutions.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvSolutions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvSolutions.Location = new System.Drawing.Point(3, 2);
            this.dgvSolutions.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.dgvSolutions.MultiSelect = false;
            this.dgvSolutions.Name = "dgvSolutions";
            this.dgvSolutions.RowHeadersVisible = false;
            this.dgvSolutions.RowHeadersWidth = 62;
            this.dgvSolutions.RowTemplate.Height = 28;
            this.dgvSolutions.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvSolutions.Size = new System.Drawing.Size(1027, 743);
            this.dgvSolutions.TabIndex = 6;
            this.dgvSolutions.CellBeginEdit += new System.Windows.Forms.DataGridViewCellCancelEventHandler(this.dgvSolutions_CellBeginEdit);
            this.dgvSolutions.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvSolutions_CellDoubleClick);
            // 
            // Frm_SolutionList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ClientSize = new System.Drawing.Size(1166, 747);
            this.Controls.Add(this.tablePanel1);
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "Frm_SolutionList";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "方案列表";
            this.tablePanel1.ResumeLayout(false);
            this.panelControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvSolutions)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tablePanel1;
        private System.Windows.Forms.Panel panelControl1;
        private System.Windows.Forms.Button btn_Delete;
        private System.Windows.Forms.Button btn_AddCur;
        private System.Windows.Forms.Button btn_AddNew;
        private System.Windows.Forms.Button btn_Open;
        private System.Windows.Forms.Button btn_SetStart;
        private System.Windows.Forms.DataGridView dgvSolutions;
    }
}