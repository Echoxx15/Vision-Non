namespace Vision.Frm.DLModel
{
    partial class Frm_DLModelConfig
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
            this.components = new System.ComponentModel.Container();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.dgv_Models = new System.Windows.Forms.DataGridView();
            this.col_Name = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col_Type = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col_Status = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.panel_Buttons = new System.Windows.Forms.Panel();
            this.btn_Save = new System.Windows.Forms.Button();
            this.btn_Remove = new System.Windows.Forms.Button();
            this.btn_Add = new System.Windows.Forms.Button();
            this.panel_ConfigContainer = new System.Windows.Forms.Panel();
            this.panel_Config = new System.Windows.Forms.Panel();
            this.lbl_ConfigTitle = new System.Windows.Forms.Label();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_Models)).BeginInit();
            this.panel_Buttons.SuspendLayout();
            this.panel_ConfigContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.Location = new System.Drawing.Point(8, 8);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.dgv_Models);
            this.splitContainer1.Panel1.Controls.Add(this.panel_Buttons);
            this.splitContainer1.Panel1MinSize = 280;
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.panel_ConfigContainer);
            this.splitContainer1.Size = new System.Drawing.Size(884, 534);
            this.splitContainer1.SplitterDistance = 320;
            this.splitContainer1.SplitterWidth = 6;
            this.splitContainer1.TabIndex = 0;
            // 
            // dgv_Models
            // 
            this.dgv_Models.AllowUserToAddRows = false;
            this.dgv_Models.AllowUserToDeleteRows = false;
            this.dgv_Models.AllowUserToResizeRows = false;
            this.dgv_Models.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dgv_Models.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.dgv_Models.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv_Models.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.col_Name,
            this.col_Type,
            this.col_Status});
            this.dgv_Models.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgv_Models.Location = new System.Drawing.Point(0, 45);
            this.dgv_Models.MultiSelect = false;
            this.dgv_Models.Name = "dgv_Models";
            this.dgv_Models.RowHeadersVisible = false;
            this.dgv_Models.RowTemplate.Height = 28;
            this.dgv_Models.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgv_Models.Size = new System.Drawing.Size(320, 489);
            this.dgv_Models.TabIndex = 1;
            // 
            // col_Name
            // 
            this.col_Name.DataPropertyName = "Name";
            this.col_Name.HeaderText = "名称";
            this.col_Name.MinimumWidth = 80;
            this.col_Name.Name = "col_Name";
            this.col_Name.Width = 110;
            // 
            // col_Type
            // 
            this.col_Type.DataPropertyName = "Type";
            this.col_Type.HeaderText = "类型";
            this.col_Type.MinimumWidth = 60;
            this.col_Type.Name = "col_Type";
            this.col_Type.ReadOnly = true;
            this.col_Type.Width = 100;
            // 
            // col_Status
            // 
            this.col_Status.DataPropertyName = "Status";
            this.col_Status.HeaderText = "状态";
            this.col_Status.MinimumWidth = 50;
            this.col_Status.Name = "col_Status";
            this.col_Status.ReadOnly = true;
            this.col_Status.Width = 70;
            // 
            // panel_Buttons
            // 
            this.panel_Buttons.Controls.Add(this.btn_Save);
            this.panel_Buttons.Controls.Add(this.btn_Remove);
            this.panel_Buttons.Controls.Add(this.btn_Add);
            this.panel_Buttons.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel_Buttons.Location = new System.Drawing.Point(0, 0);
            this.panel_Buttons.Name = "panel_Buttons";
            this.panel_Buttons.Padding = new System.Windows.Forms.Padding(5);
            this.panel_Buttons.Size = new System.Drawing.Size(320, 45);
            this.panel_Buttons.TabIndex = 0;
            // 
            // btn_Save
            // 
            this.btn_Save.Location = new System.Drawing.Point(180, 8);
            this.btn_Save.Name = "btn_Save";
            this.btn_Save.Size = new System.Drawing.Size(80, 28);
            this.btn_Save.TabIndex = 2;
            this.btn_Save.Text = "保存配置";
            this.btn_Save.UseVisualStyleBackColor = true;
            // 
            // btn_Remove
            // 
            this.btn_Remove.Location = new System.Drawing.Point(93, 8);
            this.btn_Remove.Name = "btn_Remove";
            this.btn_Remove.Size = new System.Drawing.Size(80, 28);
            this.btn_Remove.TabIndex = 1;
            this.btn_Remove.Text = "删除";
            this.btn_Remove.UseVisualStyleBackColor = true;
            // 
            // btn_Add
            // 
            this.btn_Add.Location = new System.Drawing.Point(8, 8);
            this.btn_Add.Name = "btn_Add";
            this.btn_Add.Size = new System.Drawing.Size(80, 28);
            this.btn_Add.TabIndex = 0;
            this.btn_Add.Text = "添加 ▼";
            this.btn_Add.UseVisualStyleBackColor = true;
            // 
            // panel_ConfigContainer
            // 
            this.panel_ConfigContainer.Controls.Add(this.panel_Config);
            this.panel_ConfigContainer.Controls.Add(this.lbl_ConfigTitle);
            this.panel_ConfigContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel_ConfigContainer.Location = new System.Drawing.Point(0, 0);
            this.panel_ConfigContainer.Name = "panel_ConfigContainer";
            this.panel_ConfigContainer.Size = new System.Drawing.Size(558, 534);
            this.panel_ConfigContainer.TabIndex = 0;
            // 
            // panel_Config
            // 
            this.panel_Config.BackColor = System.Drawing.SystemColors.Window;
            this.panel_Config.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel_Config.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel_Config.Location = new System.Drawing.Point(0, 35);
            this.panel_Config.Name = "panel_Config";
            this.panel_Config.Size = new System.Drawing.Size(558, 499);
            this.panel_Config.TabIndex = 1;
            // 
            // lbl_ConfigTitle
            // 
            this.lbl_ConfigTitle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(130)))), ((int)(((byte)(180)))));
            this.lbl_ConfigTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.lbl_ConfigTitle.Font = new System.Drawing.Font("微软雅黑", 10F, System.Drawing.FontStyle.Bold);
            this.lbl_ConfigTitle.ForeColor = System.Drawing.Color.White;
            this.lbl_ConfigTitle.Location = new System.Drawing.Point(0, 0);
            this.lbl_ConfigTitle.Name = "lbl_ConfigTitle";
            this.lbl_ConfigTitle.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.lbl_ConfigTitle.Size = new System.Drawing.Size(558, 35);
            this.lbl_ConfigTitle.TabIndex = 0;
            this.lbl_ConfigTitle.Text = "模型配置";
            this.lbl_ConfigTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(61, 4);
            // 
            // Frm_DLModelConfig
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(900, 550);
            this.Controls.Add(this.splitContainer1);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(700, 450);
            this.Name = "Frm_DLModelConfig";
            this.Padding = new System.Windows.Forms.Padding(8);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "深度学习模型配置";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgv_Models)).EndInit();
            this.panel_Buttons.ResumeLayout(false);
            this.panel_ConfigContainer.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.DataGridView dgv_Models;
        private System.Windows.Forms.DataGridViewTextBoxColumn col_Name;
        private System.Windows.Forms.DataGridViewTextBoxColumn col_Type;
        private System.Windows.Forms.DataGridViewTextBoxColumn col_Status;
        private System.Windows.Forms.Panel panel_Buttons;
        private System.Windows.Forms.Button btn_Save;
        private System.Windows.Forms.Button btn_Remove;
        private System.Windows.Forms.Button btn_Add;
        private System.Windows.Forms.Panel panel_ConfigContainer;
        private System.Windows.Forms.Panel panel_Config;
        private System.Windows.Forms.Label lbl_ConfigTitle;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
    }
}
