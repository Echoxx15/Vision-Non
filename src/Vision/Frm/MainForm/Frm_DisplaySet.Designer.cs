using Vision.UI;

namespace Vision.Frm.MainForm
{
    partial class Frm_DisplaySet
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.labCol = new System.Windows.Forms.Label();
            this.dgv_Names = new System.Windows.Forms.DataGridView();
            this.cmbDisplaySize = new System.Windows.Forms.ComboBox();
            this.cmbDisplayRow = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.labRow = new System.Windows.Forms.Label();
            this.chkDisplay = new System.Windows.Forms.CheckBox();
            this.cmbDisplayCol = new System.Windows.Forms.ComboBox();
            this.btnSaveDC = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_Names)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.groupBox1.Controls.Add(this.labCol);
            this.groupBox1.Controls.Add(this.dgv_Names);
            this.groupBox1.Controls.Add(this.cmbDisplaySize);
            this.groupBox1.Controls.Add(this.cmbDisplayRow);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.labRow);
            this.groupBox1.Controls.Add(this.chkDisplay);
            this.groupBox1.Controls.Add(this.cmbDisplayCol);
            this.groupBox1.Controls.Add(this.btnSaveDC);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox1.Size = new System.Drawing.Size(586, 356);
            this.groupBox1.TabIndex = 11;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "视图窗体设置";
            // 
            // labCol
            // 
            this.labCol.Location = new System.Drawing.Point(24, 140);
            this.labCol.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labCol.Name = "labCol";
            this.labCol.Size = new System.Drawing.Size(104, 30);
            this.labCol.TabIndex = 3;
            this.labCol.Text = "列数";
            this.labCol.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // dgv_Names
            // 
            this.dgv_Names.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv_Names.Location = new System.Drawing.Point(273, 21);
            this.dgv_Names.Margin = new System.Windows.Forms.Padding(4);
            this.dgv_Names.Name = "dgv_Names";
            this.dgv_Names.RowHeadersWidth = 62;
            this.dgv_Names.RowTemplate.Height = 23;
            this.dgv_Names.Size = new System.Drawing.Size(296, 315);
            this.dgv_Names.TabIndex = 9;
            // 
            // cmbDisplaySize
            // 
            this.cmbDisplaySize.FormattingEnabled = true;
            this.cmbDisplaySize.Items.AddRange(new object[] {
            "0.1",
            "0.2",
            "0.3",
            "0.4",
            "0.5",
            "0.6",
            "0.7",
            "0.8"});
            this.cmbDisplaySize.Location = new System.Drawing.Point(140, 206);
            this.cmbDisplaySize.Margin = new System.Windows.Forms.Padding(4);
            this.cmbDisplaySize.Name = "cmbDisplaySize";
            this.cmbDisplaySize.Size = new System.Drawing.Size(98, 26);
            this.cmbDisplaySize.TabIndex = 8;
            this.cmbDisplaySize.Visible = false;
            // 
            // cmbDisplayRow
            // 
            this.cmbDisplayRow.FormattingEnabled = true;
            this.cmbDisplayRow.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4",
            "5",
            "6"});
            this.cmbDisplayRow.Location = new System.Drawing.Point(140, 76);
            this.cmbDisplayRow.Margin = new System.Windows.Forms.Padding(4);
            this.cmbDisplayRow.Name = "cmbDisplayRow";
            this.cmbDisplayRow.Size = new System.Drawing.Size(98, 26);
            this.cmbDisplayRow.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(24, 202);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(104, 30);
            this.label1.TabIndex = 7;
            this.label1.Text = "窗体大小";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label1.Visible = false;
            // 
            // labRow
            // 
            this.labRow.Location = new System.Drawing.Point(24, 76);
            this.labRow.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labRow.Name = "labRow";
            this.labRow.Size = new System.Drawing.Size(104, 30);
            this.labRow.TabIndex = 2;
            this.labRow.Text = "行数";
            this.labRow.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // chkDisplay
            // 
            this.chkDisplay.AutoSize = true;
            this.chkDisplay.Location = new System.Drawing.Point(140, 30);
            this.chkDisplay.Margin = new System.Windows.Forms.Padding(4);
            this.chkDisplay.Name = "chkDisplay";
            this.chkDisplay.Size = new System.Drawing.Size(106, 22);
            this.chkDisplay.TabIndex = 6;
            this.chkDisplay.Text = "窗体显示";
            this.chkDisplay.UseVisualStyleBackColor = true;
            // 
            // cmbDisplayCol
            // 
            this.cmbDisplayCol.FormattingEnabled = true;
            this.cmbDisplayCol.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
            "10"});
            this.cmbDisplayCol.Location = new System.Drawing.Point(140, 141);
            this.cmbDisplayCol.Margin = new System.Windows.Forms.Padding(4);
            this.cmbDisplayCol.Name = "cmbDisplayCol";
            this.cmbDisplayCol.Size = new System.Drawing.Size(98, 26);
            this.cmbDisplayCol.TabIndex = 4;
            // 
            // btnSaveDC
            // 
            this.btnSaveDC.Location = new System.Drawing.Point(27, 282);
            this.btnSaveDC.Margin = new System.Windows.Forms.Padding(4);
            this.btnSaveDC.Name = "btnSaveDC";
            this.btnSaveDC.Size = new System.Drawing.Size(112, 34);
            this.btnSaveDC.TabIndex = 5;
            this.btnSaveDC.Text = "保存";
            this.btnSaveDC.UseVisualStyleBackColor = true;
            // 
            // Frm_DisplaySet
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(586, 356);
            this.Controls.Add(this.groupBox1);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "Frm_DisplaySet";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "显示窗口设置";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_Names)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label labCol;
        private System.Windows.Forms.DataGridView dgv_Names;
        private System.Windows.Forms.ComboBox cmbDisplaySize;
        private System.Windows.Forms.ComboBox cmbDisplayRow;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label labRow;
        private System.Windows.Forms.CheckBox chkDisplay;
        private System.Windows.Forms.ComboBox cmbDisplayCol;
        private System.Windows.Forms.Button btnSaveDC;
    }
}