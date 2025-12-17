namespace Vision.Frm.Station
{
    partial class Frm_OutputMapping
    {
        private System.ComponentModel.IContainer components = null;
        
        // 左右分割容器
        private System.Windows.Forms.SplitContainer splitContainer1;
        
        // 左侧：工具树
        private System.Windows.Forms.TreeView treeView_Tools;
        
        // 右侧：输出列表和映射配置
        private System.Windows.Forms.SplitContainer splitContainer_Right;
        private System.Windows.Forms.DataGridView dgv_Outputs;
        private System.Windows.Forms.DataGridView dgv_Mappings;
        
        // 底部按钮
        private System.Windows.Forms.Panel panel_Buttons;
        private System.Windows.Forms.Button btn_Add;
        private System.Windows.Forms.Button btn_Remove;
        private System.Windows.Forms.Button btn_MoveUp;
        private System.Windows.Forms.Button btn_MoveDown;
        private System.Windows.Forms.Button btn_Save;
        private System.Windows.Forms.Button btn_Cancel;
        
        // 顶部标题
        private System.Windows.Forms.Panel panel_Top;
        private System.Windows.Forms.Label lbl_Title;
        
        // 右上标签
        private System.Windows.Forms.Label lbl_Outputs;
        // 右下标签  
        private System.Windows.Forms.Label lbl_Mappings;

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
            // 定义深色主题颜色
            System.Drawing.Color darkBackground = System.Drawing.Color.FromArgb(45, 45, 48);
            System.Drawing.Color darkControl = System.Drawing.Color.FromArgb(60, 60, 65);
            System.Drawing.Color darkBorder = System.Drawing.Color.FromArgb(80, 80, 85);
            System.Drawing.Color lightText = System.Drawing.Color.FromArgb(220, 220, 220);
            System.Drawing.Color accentBlue = System.Drawing.Color.FromArgb(0, 122, 204);
            System.Drawing.Color headerBackground = System.Drawing.Color.FromArgb(37, 37, 38);
            System.Drawing.Color selectedRow = System.Drawing.Color.FromArgb(51, 51, 55);
            
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.treeView_Tools = new System.Windows.Forms.TreeView();
            this.splitContainer_Right = new System.Windows.Forms.SplitContainer();
            this.dgv_Outputs = new System.Windows.Forms.DataGridView();
            this.lbl_Outputs = new System.Windows.Forms.Label();
            this.dgv_Mappings = new System.Windows.Forms.DataGridView();
            this.lbl_Mappings = new System.Windows.Forms.Label();
            this.panel_Buttons = new System.Windows.Forms.Panel();
            this.btn_Add = new System.Windows.Forms.Button();
            this.btn_Remove = new System.Windows.Forms.Button();
            this.btn_MoveUp = new System.Windows.Forms.Button();
            this.btn_MoveDown = new System.Windows.Forms.Button();
            this.btn_Save = new System.Windows.Forms.Button();
            this.btn_Cancel = new System.Windows.Forms.Button();
            this.panel_Top = new System.Windows.Forms.Panel();
            this.lbl_Title = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer_Right)).BeginInit();
            this.splitContainer_Right.Panel1.SuspendLayout();
            this.splitContainer_Right.Panel2.SuspendLayout();
            this.splitContainer_Right.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_Outputs)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_Mappings)).BeginInit();
            this.panel_Buttons.SuspendLayout();
            this.panel_Top.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 54);
            this.splitContainer1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.BackColor = darkBackground;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.treeView_Tools);
            this.splitContainer1.Panel1.BackColor = darkBackground;
            this.splitContainer1.Panel1MinSize = 180;
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer_Right);
            this.splitContainer1.Panel2.BackColor = darkBackground;
            this.splitContainer1.Size = new System.Drawing.Size(1069, 552);
            this.splitContainer1.SplitterDistance = 200;
            this.splitContainer1.SplitterWidth = 3;
            this.splitContainer1.TabIndex = 1;
            // 
            // treeView_Tools
            // 
            this.treeView_Tools.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView_Tools.Font = new System.Drawing.Font("Microsoft YaHei UI", 10F);
            this.treeView_Tools.Location = new System.Drawing.Point(0, 0);
            this.treeView_Tools.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.treeView_Tools.Name = "treeView_Tools";
            this.treeView_Tools.Size = new System.Drawing.Size(200, 552);
            this.treeView_Tools.TabIndex = 0;
            this.treeView_Tools.BackColor = darkControl;
            this.treeView_Tools.ForeColor = lightText;
            this.treeView_Tools.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.treeView_Tools.ItemHeight = 28;
            this.treeView_Tools.FullRowSelect = true;
            // 
            // splitContainer_Right
            // 
            this.splitContainer_Right.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer_Right.Location = new System.Drawing.Point(0, 0);
            this.splitContainer_Right.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.splitContainer_Right.Name = "splitContainer_Right";
            this.splitContainer_Right.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.splitContainer_Right.BackColor = darkBackground;
            // 
            // splitContainer_Right.Panel1
            // 
            this.splitContainer_Right.Panel1.Controls.Add(this.dgv_Outputs);
            this.splitContainer_Right.Panel1.Controls.Add(this.lbl_Outputs);
            this.splitContainer_Right.Panel1.BackColor = darkBackground;
            // 
            // splitContainer_Right.Panel2
            // 
            this.splitContainer_Right.Panel2.Controls.Add(this.dgv_Mappings);
            this.splitContainer_Right.Panel2.Controls.Add(this.lbl_Mappings);
            this.splitContainer_Right.Panel2.BackColor = darkBackground;
            this.splitContainer_Right.Size = new System.Drawing.Size(866, 552);
            this.splitContainer_Right.SplitterDistance = 220;
            this.splitContainer_Right.SplitterWidth = 4;
            this.splitContainer_Right.TabIndex = 0;
            // 
            // dgv_Outputs
            // 
            this.dgv_Outputs.AllowUserToAddRows = false;
            this.dgv_Outputs.AllowUserToDeleteRows = false;
            this.dgv_Outputs.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv_Outputs.ColumnHeadersHeight = 32;
            this.dgv_Outputs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgv_Outputs.Location = new System.Drawing.Point(0, 34);
            this.dgv_Outputs.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.dgv_Outputs.MultiSelect = false;
            this.dgv_Outputs.Name = "dgv_Outputs";
            this.dgv_Outputs.ReadOnly = true;
            this.dgv_Outputs.RowHeadersVisible = false;
            this.dgv_Outputs.RowHeadersWidth = 62;
            this.dgv_Outputs.RowTemplate.Height = 28;
            this.dgv_Outputs.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgv_Outputs.Size = new System.Drawing.Size(866, 186);
            this.dgv_Outputs.TabIndex = 0;
            // 深色样式
            this.dgv_Outputs.BackgroundColor = darkControl;
            this.dgv_Outputs.GridColor = darkBorder;
            this.dgv_Outputs.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgv_Outputs.DefaultCellStyle.BackColor = darkControl;
            this.dgv_Outputs.DefaultCellStyle.ForeColor = lightText;
            this.dgv_Outputs.DefaultCellStyle.SelectionBackColor = accentBlue;
            this.dgv_Outputs.DefaultCellStyle.SelectionForeColor = System.Drawing.Color.White;
            this.dgv_Outputs.ColumnHeadersDefaultCellStyle.BackColor = headerBackground;
            this.dgv_Outputs.ColumnHeadersDefaultCellStyle.ForeColor = lightText;
            this.dgv_Outputs.ColumnHeadersDefaultCellStyle.SelectionBackColor = headerBackground;
            this.dgv_Outputs.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Bold);
            this.dgv_Outputs.EnableHeadersVisualStyles = false;
            this.dgv_Outputs.AlternatingRowsDefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(50, 50, 54);
            // 
            // lbl_Outputs
            // 
            this.lbl_Outputs.AutoSize = false;
            this.lbl_Outputs.Dock = System.Windows.Forms.DockStyle.Top;
            this.lbl_Outputs.Font = new System.Drawing.Font("Microsoft YaHei UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.lbl_Outputs.Location = new System.Drawing.Point(0, 0);
            this.lbl_Outputs.Name = "lbl_Outputs";
            this.lbl_Outputs.Padding = new System.Windows.Forms.Padding(8, 0, 0, 0);
            this.lbl_Outputs.Size = new System.Drawing.Size(866, 34);
            this.lbl_Outputs.TabIndex = 1;
            this.lbl_Outputs.Text = "📤 工具输出 (双击添加映射)";
            this.lbl_Outputs.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lbl_Outputs.BackColor = headerBackground;
            this.lbl_Outputs.ForeColor = System.Drawing.Color.FromArgb(86, 156, 214);
            // 
            // dgv_Mappings
            // 
            this.dgv_Mappings.AllowUserToAddRows = false;
            this.dgv_Mappings.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv_Mappings.ColumnHeadersHeight = 32;
            this.dgv_Mappings.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgv_Mappings.Location = new System.Drawing.Point(0, 34);
            this.dgv_Mappings.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.dgv_Mappings.MultiSelect = false;
            this.dgv_Mappings.Name = "dgv_Mappings";
            this.dgv_Mappings.RowHeadersVisible = false;
            this.dgv_Mappings.RowHeadersWidth = 62;
            this.dgv_Mappings.RowTemplate.Height = 28;
            this.dgv_Mappings.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgv_Mappings.Size = new System.Drawing.Size(866, 294);
            this.dgv_Mappings.TabIndex = 0;
            // 深色样式
            this.dgv_Mappings.BackgroundColor = darkControl;
            this.dgv_Mappings.GridColor = darkBorder;
            this.dgv_Mappings.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgv_Mappings.DefaultCellStyle.BackColor = darkControl;
            this.dgv_Mappings.DefaultCellStyle.ForeColor = lightText;
            this.dgv_Mappings.DefaultCellStyle.SelectionBackColor = accentBlue;
            this.dgv_Mappings.DefaultCellStyle.SelectionForeColor = System.Drawing.Color.White;
            this.dgv_Mappings.ColumnHeadersDefaultCellStyle.BackColor = headerBackground;
            this.dgv_Mappings.ColumnHeadersDefaultCellStyle.ForeColor = lightText;
            this.dgv_Mappings.ColumnHeadersDefaultCellStyle.SelectionBackColor = headerBackground;
            this.dgv_Mappings.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Bold);
            this.dgv_Mappings.EnableHeadersVisualStyles = false;
            this.dgv_Mappings.AlternatingRowsDefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(50, 50, 54);
            // 
            // lbl_Mappings
            // 
            this.lbl_Mappings.AutoSize = false;
            this.lbl_Mappings.Dock = System.Windows.Forms.DockStyle.Top;
            this.lbl_Mappings.Font = new System.Drawing.Font("Microsoft YaHei UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.lbl_Mappings.Location = new System.Drawing.Point(0, 0);
            this.lbl_Mappings.Name = "lbl_Mappings";
            this.lbl_Mappings.Padding = new System.Windows.Forms.Padding(8, 0, 0, 0);
            this.lbl_Mappings.Size = new System.Drawing.Size(866, 34);
            this.lbl_Mappings.TabIndex = 1;
            this.lbl_Mappings.Text = "📋 已配置映射";
            this.lbl_Mappings.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lbl_Mappings.BackColor = headerBackground;
            this.lbl_Mappings.ForeColor = System.Drawing.Color.FromArgb(78, 201, 176);
            // 
            // panel_Buttons
            // 
            this.panel_Buttons.Controls.Add(this.btn_Add);
            this.panel_Buttons.Controls.Add(this.btn_Remove);
            this.panel_Buttons.Controls.Add(this.btn_MoveUp);
            this.panel_Buttons.Controls.Add(this.btn_MoveDown);
            this.panel_Buttons.Controls.Add(this.btn_Save);
            this.panel_Buttons.Controls.Add(this.btn_Cancel);
            this.panel_Buttons.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel_Buttons.Location = new System.Drawing.Point(0, 606);
            this.panel_Buttons.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.panel_Buttons.Name = "panel_Buttons";
            this.panel_Buttons.Padding = new System.Windows.Forms.Padding(12);
            this.panel_Buttons.Size = new System.Drawing.Size(1069, 66);
            this.panel_Buttons.TabIndex = 2;
            this.panel_Buttons.BackColor = headerBackground;
            // 
            // btn_Add
            // 
            this.btn_Add.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F);
            this.btn_Add.Location = new System.Drawing.Point(15, 14);
            this.btn_Add.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btn_Add.Name = "btn_Add";
            this.btn_Add.Size = new System.Drawing.Size(108, 38);
            this.btn_Add.TabIndex = 0;
            this.btn_Add.Text = "➕ 添加映射";
            this.btn_Add.UseVisualStyleBackColor = false;
            this.btn_Add.BackColor = darkControl;
            this.btn_Add.ForeColor = lightText;
            this.btn_Add.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_Add.FlatAppearance.BorderColor = darkBorder;
            this.btn_Add.FlatAppearance.MouseOverBackColor = accentBlue;
            this.btn_Add.Cursor = System.Windows.Forms.Cursors.Hand;
            // 
            // btn_Remove
            // 
            this.btn_Remove.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F);
            this.btn_Remove.Location = new System.Drawing.Point(130, 14);
            this.btn_Remove.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btn_Remove.Name = "btn_Remove";
            this.btn_Remove.Size = new System.Drawing.Size(108, 38);
            this.btn_Remove.TabIndex = 1;
            this.btn_Remove.Text = "➖ 删除映射";
            this.btn_Remove.UseVisualStyleBackColor = false;
            this.btn_Remove.BackColor = darkControl;
            this.btn_Remove.ForeColor = lightText;
            this.btn_Remove.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_Remove.FlatAppearance.BorderColor = darkBorder;
            this.btn_Remove.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(180, 80, 80);
            this.btn_Remove.Cursor = System.Windows.Forms.Cursors.Hand;
            // 
            // btn_MoveUp
            // 
            this.btn_MoveUp.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F);
            this.btn_MoveUp.Location = new System.Drawing.Point(250, 14);
            this.btn_MoveUp.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btn_MoveUp.Name = "btn_MoveUp";
            this.btn_MoveUp.Size = new System.Drawing.Size(80, 38);
            this.btn_MoveUp.TabIndex = 2;
            this.btn_MoveUp.Text = "⬆ 上移";
            this.btn_MoveUp.UseVisualStyleBackColor = false;
            this.btn_MoveUp.BackColor = darkControl;
            this.btn_MoveUp.ForeColor = lightText;
            this.btn_MoveUp.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_MoveUp.FlatAppearance.BorderColor = darkBorder;
            this.btn_MoveUp.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(80, 140, 80);
            this.btn_MoveUp.Cursor = System.Windows.Forms.Cursors.Hand;
            // 
            // btn_MoveDown
            // 
            this.btn_MoveDown.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F);
            this.btn_MoveDown.Location = new System.Drawing.Point(337, 14);
            this.btn_MoveDown.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btn_MoveDown.Name = "btn_MoveDown";
            this.btn_MoveDown.Size = new System.Drawing.Size(80, 38);
            this.btn_MoveDown.TabIndex = 3;
            this.btn_MoveDown.Text = "⬇ 下移";
            this.btn_MoveDown.UseVisualStyleBackColor = false;
            this.btn_MoveDown.BackColor = darkControl;
            this.btn_MoveDown.ForeColor = lightText;
            this.btn_MoveDown.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_MoveDown.FlatAppearance.BorderColor = darkBorder;
            this.btn_MoveDown.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(80, 140, 80);
            this.btn_MoveDown.Cursor = System.Windows.Forms.Cursors.Hand;
            // 
            // btn_Save
            // 
            this.btn_Save.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_Save.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Bold);
            this.btn_Save.Location = new System.Drawing.Point(833, 14);
            this.btn_Save.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btn_Save.Name = "btn_Save";
            this.btn_Save.Size = new System.Drawing.Size(108, 38);
            this.btn_Save.TabIndex = 4;
            this.btn_Save.Text = "✔ 保存";
            this.btn_Save.UseVisualStyleBackColor = false;
            this.btn_Save.BackColor = accentBlue;
            this.btn_Save.ForeColor = System.Drawing.Color.White;
            this.btn_Save.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_Save.FlatAppearance.BorderColor = accentBlue;
            this.btn_Save.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(0, 150, 230);
            this.btn_Save.Cursor = System.Windows.Forms.Cursors.Hand;
            // 
            // btn_Cancel
            // 
            this.btn_Cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_Cancel.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F);
            this.btn_Cancel.Location = new System.Drawing.Point(948, 14);
            this.btn_Cancel.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btn_Cancel.Name = "btn_Cancel";
            this.btn_Cancel.Size = new System.Drawing.Size(108, 38);
            this.btn_Cancel.TabIndex = 5;
            this.btn_Cancel.Text = "✖ 取消";
            this.btn_Cancel.UseVisualStyleBackColor = false;
            this.btn_Cancel.BackColor = darkControl;
            this.btn_Cancel.ForeColor = lightText;
            this.btn_Cancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_Cancel.FlatAppearance.BorderColor = darkBorder;
            this.btn_Cancel.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(100, 100, 105);
            this.btn_Cancel.Cursor = System.Windows.Forms.Cursors.Hand;
            // 
            // panel_Top
            // 
            this.panel_Top.Controls.Add(this.lbl_Title);
            this.panel_Top.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel_Top.Location = new System.Drawing.Point(0, 0);
            this.panel_Top.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.panel_Top.Name = "panel_Top";
            this.panel_Top.Padding = new System.Windows.Forms.Padding(12);
            this.panel_Top.Size = new System.Drawing.Size(1069, 54);
            this.panel_Top.TabIndex = 0;
            this.panel_Top.BackColor = headerBackground;
            // 
            // lbl_Title
            // 
            this.lbl_Title.AutoSize = true;
            this.lbl_Title.Font = new System.Drawing.Font("Microsoft YaHei UI", 12F, System.Drawing.FontStyle.Bold);
            this.lbl_Title.Location = new System.Drawing.Point(12, 12);
            this.lbl_Title.Name = "lbl_Title";
            this.lbl_Title.Size = new System.Drawing.Size(320, 30);
            this.lbl_Title.TabIndex = 0;
            this.lbl_Title.Text = "🔗 工具输出 → 通讯输出 映射配置";
            this.lbl_Title.ForeColor = lightText;
            // 
            // Frm_OutputMapping
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1069, 672);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.panel_Buttons);
            this.Controls.Add(this.panel_Top);
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MinimizeBox = false;
            this.Name = "Frm_OutputMapping";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "输出映射配置";
            this.BackColor = darkBackground;
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer_Right.Panel1.ResumeLayout(false);
            this.splitContainer_Right.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer_Right)).EndInit();
            this.splitContainer_Right.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgv_Outputs)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_Mappings)).EndInit();
            this.panel_Buttons.ResumeLayout(false);
            this.panel_Top.ResumeLayout(false);
            this.panel_Top.PerformLayout();
            this.ResumeLayout(false);

        }
    }
}
