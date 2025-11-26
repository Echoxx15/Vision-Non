namespace Vision.Frm.Station
{
    partial class Frm_OutputMapping
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.DataGridView dgv_Mappings;
        private System.Windows.Forms.Button btn_Add;
        private System.Windows.Forms.Button btn_Remove;
        private System.Windows.Forms.Button btn_MoveUp;
        private System.Windows.Forms.Button btn_MoveDown;
        private System.Windows.Forms.Button btn_Refresh;
        private System.Windows.Forms.Button btn_Save;
        private System.Windows.Forms.Button btn_Cancel;
        private System.Windows.Forms.Panel panel_Buttons;
        private System.Windows.Forms.Panel panel_Top;
        private System.Windows.Forms.Label lbl_Title;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.dgv_Mappings = new System.Windows.Forms.DataGridView();
            this.btn_Add = new System.Windows.Forms.Button();
            this.btn_Remove = new System.Windows.Forms.Button();
            this.btn_MoveUp = new System.Windows.Forms.Button();
            this.btn_MoveDown = new System.Windows.Forms.Button();
            this.btn_Refresh = new System.Windows.Forms.Button();
            this.btn_Save = new System.Windows.Forms.Button();
            this.btn_Cancel = new System.Windows.Forms.Button();
            this.panel_Buttons = new System.Windows.Forms.Panel();
            this.panel_Top = new System.Windows.Forms.Panel();
            this.lbl_Title = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_Mappings)).BeginInit();
            this.panel_Buttons.SuspendLayout();
            this.panel_Top.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel_Top
            // 
            this.panel_Top.Controls.Add(this.lbl_Title);
            this.panel_Top.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel_Top.Location = new System.Drawing.Point(0, 0);
            this.panel_Top.Name = "panel_Top";
            this.panel_Top.Padding = new System.Windows.Forms.Padding(10);
            this.panel_Top.Size = new System.Drawing.Size(900, 50);
            this.panel_Top.TabIndex = 0;
            // 
            // lbl_Title
            // 
            this.lbl_Title.AutoSize = true;
            this.lbl_Title.Font = new System.Drawing.Font("Microsoft YaHei UI", 12F, System.Drawing.FontStyle.Bold);
            this.lbl_Title.Location = new System.Drawing.Point(13, 13);
            this.lbl_Title.Name = "lbl_Title";
            this.lbl_Title.Size = new System.Drawing.Size(362, 22);
            this.lbl_Title.TabIndex = 0;
            this.lbl_Title.Text = "工具输出 ? 通讯输出 映射配置";
            // 
            // dgv_Mappings
            // 
            this.dgv_Mappings.AllowUserToAddRows = false;
            this.dgv_Mappings.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgv_Mappings.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv_Mappings.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgv_Mappings.Location = new System.Drawing.Point(0, 50);
            this.dgv_Mappings.Name = "dgv_Mappings";
            this.dgv_Mappings.RowHeadersWidth = 51;
            this.dgv_Mappings.RowTemplate.Height = 27;
            this.dgv_Mappings.Size = new System.Drawing.Size(900, 450);
            this.dgv_Mappings.TabIndex = 1;
            // 
            // panel_Buttons
            // 
            this.panel_Buttons.Controls.Add(this.btn_Add);
            this.panel_Buttons.Controls.Add(this.btn_Remove);
            this.panel_Buttons.Controls.Add(this.btn_MoveUp);
            this.panel_Buttons.Controls.Add(this.btn_MoveDown);
            this.panel_Buttons.Controls.Add(this.btn_Refresh);
            this.panel_Buttons.Controls.Add(this.btn_Save);
            this.panel_Buttons.Controls.Add(this.btn_Cancel);
            this.panel_Buttons.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel_Buttons.Location = new System.Drawing.Point(0, 500);
            this.panel_Buttons.Name = "panel_Buttons";
            this.panel_Buttons.Padding = new System.Windows.Forms.Padding(10);
            this.panel_Buttons.Size = new System.Drawing.Size(900, 60);
            this.panel_Buttons.TabIndex = 2;
            // 
            // btn_Add
            // 
            this.btn_Add.Location = new System.Drawing.Point(13, 13);
            this.btn_Add.Name = "btn_Add";
            this.btn_Add.Size = new System.Drawing.Size(90, 34);
            this.btn_Add.TabIndex = 0;
            this.btn_Add.Text = "添加";
            this.btn_Add.UseVisualStyleBackColor = true;
            // 
            // btn_Remove
            // 
            this.btn_Remove.Location = new System.Drawing.Point(109, 13);
            this.btn_Remove.Name = "btn_Remove";
            this.btn_Remove.Size = new System.Drawing.Size(90, 34);
            this.btn_Remove.TabIndex = 1;
            this.btn_Remove.Text = "删除";
            this.btn_Remove.UseVisualStyleBackColor = true;
            // 
            // btn_MoveUp
            // 
            this.btn_MoveUp.Location = new System.Drawing.Point(205, 13);
            this.btn_MoveUp.Name = "btn_MoveUp";
            this.btn_MoveUp.Size = new System.Drawing.Size(90, 34);
            this.btn_MoveUp.TabIndex = 2;
            this.btn_MoveUp.Text = "上移";
            this.btn_MoveUp.UseVisualStyleBackColor = true;
            // 
            // btn_MoveDown
            // 
            this.btn_MoveDown.Location = new System.Drawing.Point(301, 13);
            this.btn_MoveDown.Name = "btn_MoveDown";
            this.btn_MoveDown.Size = new System.Drawing.Size(90, 34);
            this.btn_MoveDown.TabIndex = 3;
            this.btn_MoveDown.Text = "下移";
            this.btn_MoveDown.UseVisualStyleBackColor = true;
            // 
            // btn_Refresh
            // 
            this.btn_Refresh.Location = new System.Drawing.Point(397, 13);
            this.btn_Refresh.Name = "btn_Refresh";
            this.btn_Refresh.Size = new System.Drawing.Size(90, 34);
            this.btn_Refresh.TabIndex = 4;
            this.btn_Refresh.Text = "刷新列表";
            this.btn_Refresh.UseVisualStyleBackColor = true;
            // 
            // btn_Save
            // 
            this.btn_Save.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_Save.Location = new System.Drawing.Point(697, 13);
            this.btn_Save.Name = "btn_Save";
            this.btn_Save.Size = new System.Drawing.Size(90, 34);
            this.btn_Save.TabIndex = 5;
            this.btn_Save.Text = "保存";
            this.btn_Save.UseVisualStyleBackColor = true;
            // 
            // btn_Cancel
            // 
            this.btn_Cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_Cancel.Location = new System.Drawing.Point(797, 13);
            this.btn_Cancel.Name = "btn_Cancel";
            this.btn_Cancel.Size = new System.Drawing.Size(90, 34);
            this.btn_Cancel.TabIndex = 6;
            this.btn_Cancel.Text = "取消";
            this.btn_Cancel.UseVisualStyleBackColor = true;
            // 
            // Frm_OutputMapping
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(900, 560);
            this.Controls.Add(this.dgv_Mappings);
            this.Controls.Add(this.panel_Buttons);
            this.Controls.Add(this.panel_Top);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Frm_OutputMapping";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "输出映射配置";
            ((System.ComponentModel.ISupportInitialize)(this.dgv_Mappings)).EndInit();
            this.panel_Buttons.ResumeLayout(false);
            this.panel_Top.ResumeLayout(false);
            this.panel_Top.PerformLayout();
            this.ResumeLayout(false);
        }
    }
}
