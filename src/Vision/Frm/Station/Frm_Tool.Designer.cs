namespace Vision.Frm.Station
{
  partial class Frm_Tool
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
      if (disposing)
      {
        // 取消事件订阅，防止内存泄漏
        UnsubscribeToolBlockEvents();
        
        // 释放 VisionPro 编辑控件资源
        try
        {
          if (cogToolBlockEditV21 != null)
          {
            cogToolBlockEditV21.Subject = null;
          }
        }
        catch { }
        
        // 清空数据绑定
        try
        {
          dgv_Data.DataSource = null;
          _vars = null;
        }
        catch { }
        
        if (components != null)
        {
          components.Dispose();
        }
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.dgv_Data = new System.Windows.Forms.DataGridView();
            this.panel2 = new System.Windows.Forms.Panel();
            this.btn_Remove = new System.Windows.Forms.Button();
            this.btn_MoveDown = new System.Windows.Forms.Button();
            this.btn_MoveUp = new System.Windows.Forms.Button();
            this.btn_BoolArray = new System.Windows.Forms.Button();
            this.btn_Bool = new System.Windows.Forms.Button();
            this.btn_StringArray = new System.Windows.Forms.Button();
            this.btn_String = new System.Windows.Forms.Button();
            this.btn_FloatArray = new System.Windows.Forms.Button();
            this.btn_Float = new System.Windows.Forms.Button();
            this.btn_DoubleArray = new System.Windows.Forms.Button();
            this.btn_Double = new System.Windows.Forms.Button();
            this.btn_ShortArray = new System.Windows.Forms.Button();
            this.btn_Short = new System.Windows.Forms.Button();
            this.btn_IntArray = new System.Windows.Forms.Button();
            this.btn_Int = new System.Windows.Forms.Button();
            this.cogToolBlockEditV21 = new Cognex.VisionPro.ToolBlock.CogToolBlockEditV2();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.pnl_Edit = new System.Windows.Forms.Panel();
            this.btn_ToggleSidebar = new System.Windows.Forms.Button();
            this.btn_Seg = new System.Windows.Forms.Button();
            this.btn_OCR = new System.Windows.Forms.Button();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_Data)).BeginInit();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cogToolBlockEditV21)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.pnl_Edit.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 220F));
            this.tableLayoutPanel1.Controls.Add(this.dgv_Data, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.panel2, 1, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(832, 767);
            this.tableLayoutPanel1.TabIndex = 4;
            // 
            // dgv_Data
            // 
            this.dgv_Data.AllowUserToResizeRows = false;
            this.dgv_Data.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.dgv_Data.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            dataGridViewCellStyle1.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.Silver;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgv_Data.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgv_Data.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            dataGridViewCellStyle2.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.Silver;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgv_Data.DefaultCellStyle = dataGridViewCellStyle2;
            this.dgv_Data.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgv_Data.EnableHeadersVisualStyles = false;
            this.dgv_Data.GridColor = System.Drawing.Color.White;
            this.dgv_Data.Location = new System.Drawing.Point(3, 3);
            this.dgv_Data.Name = "dgv_Data";
            this.dgv_Data.RowHeadersWidth = 62;
            this.dgv_Data.RowTemplate.Height = 30;
            this.dgv_Data.Size = new System.Drawing.Size(606, 761);
            this.dgv_Data.TabIndex = 0;
            this.dgv_Data.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgv_Data_CellEndEdit);
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.panel2.Controls.Add(this.btn_OCR);
            this.panel2.Controls.Add(this.btn_Seg);
            this.panel2.Controls.Add(this.btn_Remove);
            this.panel2.Controls.Add(this.btn_MoveDown);
            this.panel2.Controls.Add(this.btn_MoveUp);
            this.panel2.Controls.Add(this.btn_BoolArray);
            this.panel2.Controls.Add(this.btn_Bool);
            this.panel2.Controls.Add(this.btn_StringArray);
            this.panel2.Controls.Add(this.btn_String);
            this.panel2.Controls.Add(this.btn_FloatArray);
            this.panel2.Controls.Add(this.btn_Float);
            this.panel2.Controls.Add(this.btn_DoubleArray);
            this.panel2.Controls.Add(this.btn_Double);
            this.panel2.Controls.Add(this.btn_ShortArray);
            this.panel2.Controls.Add(this.btn_Short);
            this.panel2.Controls.Add(this.btn_IntArray);
            this.panel2.Controls.Add(this.btn_Int);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(615, 3);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(214, 761);
            this.panel2.TabIndex = 1;
            // 
            // btn_Remove
            // 
            this.btn_Remove.BackColor = System.Drawing.Color.LightSeaGreen;
            this.btn_Remove.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_Remove.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_Remove.Location = new System.Drawing.Point(61, 613);
            this.btn_Remove.Name = "btn_Remove";
            this.btn_Remove.Size = new System.Drawing.Size(75, 35);
            this.btn_Remove.TabIndex = 14;
            this.btn_Remove.Text = "删除";
            this.btn_Remove.UseVisualStyleBackColor = false;
            // 
            // btn_MoveDown
            // 
            this.btn_MoveDown.BackColor = System.Drawing.Color.LightSeaGreen;
            this.btn_MoveDown.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_MoveDown.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_MoveDown.Location = new System.Drawing.Point(61, 552);
            this.btn_MoveDown.Name = "btn_MoveDown";
            this.btn_MoveDown.Size = new System.Drawing.Size(75, 35);
            this.btn_MoveDown.TabIndex = 13;
            this.btn_MoveDown.Text = "下移";
            this.btn_MoveDown.UseVisualStyleBackColor = false;
            // 
            // btn_MoveUp
            // 
            this.btn_MoveUp.BackColor = System.Drawing.Color.LightSeaGreen;
            this.btn_MoveUp.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_MoveUp.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_MoveUp.Location = new System.Drawing.Point(61, 495);
            this.btn_MoveUp.Name = "btn_MoveUp";
            this.btn_MoveUp.Size = new System.Drawing.Size(75, 35);
            this.btn_MoveUp.TabIndex = 12;
            this.btn_MoveUp.Text = "上移";
            this.btn_MoveUp.UseVisualStyleBackColor = false;
            // 
            // btn_BoolArray
            // 
            this.btn_BoolArray.BackColor = System.Drawing.Color.LightSeaGreen;
            this.btn_BoolArray.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_BoolArray.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_BoolArray.Location = new System.Drawing.Point(108, 305);
            this.btn_BoolArray.Name = "btn_BoolArray";
            this.btn_BoolArray.Size = new System.Drawing.Size(95, 35);
            this.btn_BoolArray.TabIndex = 11;
            this.btn_BoolArray.Text = "Bool[]";
            this.btn_BoolArray.UseVisualStyleBackColor = false;
            // 
            // btn_Bool
            // 
            this.btn_Bool.BackColor = System.Drawing.Color.LightSeaGreen;
            this.btn_Bool.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_Bool.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_Bool.Location = new System.Drawing.Point(8, 305);
            this.btn_Bool.Name = "btn_Bool";
            this.btn_Bool.Size = new System.Drawing.Size(90, 35);
            this.btn_Bool.TabIndex = 10;
            this.btn_Bool.Text = "Bool";
            this.btn_Bool.UseVisualStyleBackColor = false;
            // 
            // btn_StringArray
            // 
            this.btn_StringArray.BackColor = System.Drawing.Color.LightSeaGreen;
            this.btn_StringArray.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_StringArray.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_StringArray.Location = new System.Drawing.Point(108, 250);
            this.btn_StringArray.Name = "btn_StringArray";
            this.btn_StringArray.Size = new System.Drawing.Size(95, 35);
            this.btn_StringArray.TabIndex = 9;
            this.btn_StringArray.Text = "String[]";
            this.btn_StringArray.UseVisualStyleBackColor = false;
            // 
            // btn_String
            // 
            this.btn_String.BackColor = System.Drawing.Color.LightSeaGreen;
            this.btn_String.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_String.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_String.Location = new System.Drawing.Point(8, 250);
            this.btn_String.Name = "btn_String";
            this.btn_String.Size = new System.Drawing.Size(90, 35);
            this.btn_String.TabIndex = 8;
            this.btn_String.Text = "String";
            this.btn_String.UseVisualStyleBackColor = false;
            // 
            // btn_FloatArray
            // 
            this.btn_FloatArray.BackColor = System.Drawing.Color.LightSeaGreen;
            this.btn_FloatArray.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_FloatArray.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_FloatArray.Location = new System.Drawing.Point(108, 195);
            this.btn_FloatArray.Name = "btn_FloatArray";
            this.btn_FloatArray.Size = new System.Drawing.Size(95, 35);
            this.btn_FloatArray.TabIndex = 7;
            this.btn_FloatArray.Text = "Float[]";
            this.btn_FloatArray.UseVisualStyleBackColor = false;
            // 
            // btn_Float
            // 
            this.btn_Float.BackColor = System.Drawing.Color.LightSeaGreen;
            this.btn_Float.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_Float.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_Float.Location = new System.Drawing.Point(8, 195);
            this.btn_Float.Name = "btn_Float";
            this.btn_Float.Size = new System.Drawing.Size(90, 35);
            this.btn_Float.TabIndex = 6;
            this.btn_Float.Text = "Float";
            this.btn_Float.UseVisualStyleBackColor = false;
            // 
            // btn_DoubleArray
            // 
            this.btn_DoubleArray.BackColor = System.Drawing.Color.LightSeaGreen;
            this.btn_DoubleArray.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_DoubleArray.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_DoubleArray.Location = new System.Drawing.Point(108, 140);
            this.btn_DoubleArray.Name = "btn_DoubleArray";
            this.btn_DoubleArray.Size = new System.Drawing.Size(95, 35);
            this.btn_DoubleArray.TabIndex = 5;
            this.btn_DoubleArray.Text = "Double[]";
            this.btn_DoubleArray.UseVisualStyleBackColor = false;
            // 
            // btn_Double
            // 
            this.btn_Double.BackColor = System.Drawing.Color.LightSeaGreen;
            this.btn_Double.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_Double.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_Double.Location = new System.Drawing.Point(8, 140);
            this.btn_Double.Name = "btn_Double";
            this.btn_Double.Size = new System.Drawing.Size(90, 35);
            this.btn_Double.TabIndex = 4;
            this.btn_Double.Text = "Double";
            this.btn_Double.UseVisualStyleBackColor = false;
            // 
            // btn_ShortArray
            // 
            this.btn_ShortArray.BackColor = System.Drawing.Color.LightSeaGreen;
            this.btn_ShortArray.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_ShortArray.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_ShortArray.Location = new System.Drawing.Point(108, 85);
            this.btn_ShortArray.Name = "btn_ShortArray";
            this.btn_ShortArray.Size = new System.Drawing.Size(95, 35);
            this.btn_ShortArray.TabIndex = 3;
            this.btn_ShortArray.Text = "Short[]";
            this.btn_ShortArray.UseVisualStyleBackColor = false;
            // 
            // btn_Short
            // 
            this.btn_Short.BackColor = System.Drawing.Color.LightSeaGreen;
            this.btn_Short.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_Short.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_Short.Location = new System.Drawing.Point(8, 85);
            this.btn_Short.Name = "btn_Short";
            this.btn_Short.Size = new System.Drawing.Size(90, 35);
            this.btn_Short.TabIndex = 2;
            this.btn_Short.Text = "Short";
            this.btn_Short.UseVisualStyleBackColor = false;
            // 
            // btn_IntArray
            // 
            this.btn_IntArray.BackColor = System.Drawing.Color.LightSeaGreen;
            this.btn_IntArray.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_IntArray.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_IntArray.Location = new System.Drawing.Point(108, 30);
            this.btn_IntArray.Name = "btn_IntArray";
            this.btn_IntArray.Size = new System.Drawing.Size(95, 35);
            this.btn_IntArray.TabIndex = 1;
            this.btn_IntArray.Text = "Int[]";
            this.btn_IntArray.UseVisualStyleBackColor = false;
            // 
            // btn_Int
            // 
            this.btn_Int.BackColor = System.Drawing.Color.LightSeaGreen;
            this.btn_Int.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_Int.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_Int.Location = new System.Drawing.Point(8, 30);
            this.btn_Int.Name = "btn_Int";
            this.btn_Int.Size = new System.Drawing.Size(90, 35);
            this.btn_Int.TabIndex = 0;
            this.btn_Int.Text = "Int";
            this.btn_Int.UseVisualStyleBackColor = false;
            // 
            // cogToolBlockEditV21
            // 
            this.cogToolBlockEditV21.AllowDrop = true;
            this.cogToolBlockEditV21.ContextMenuCustomizer = null;
            this.cogToolBlockEditV21.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cogToolBlockEditV21.Location = new System.Drawing.Point(0, 0);
            this.cogToolBlockEditV21.MinimumSize = new System.Drawing.Size(300, 0);
            this.cogToolBlockEditV21.Name = "cogToolBlockEditV21";
            this.cogToolBlockEditV21.ShowNodeToolTips = true;
            this.cogToolBlockEditV21.Size = new System.Drawing.Size(595, 767);
            this.cogToolBlockEditV21.SuspendElectricRuns = false;
            this.cogToolBlockEditV21.TabIndex = 0;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.tableLayoutPanel1);
            this.splitContainer1.Panel1MinSize = 0;
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.pnl_Edit);
            this.splitContainer1.Panel2.Controls.Add(this.btn_ToggleSidebar);
            this.splitContainer1.Size = new System.Drawing.Size(1455, 767);
            this.splitContainer1.SplitterDistance = 832;
            this.splitContainer1.TabIndex = 2;
            // 
            // pnl_Edit
            // 
            this.pnl_Edit.Controls.Add(this.cogToolBlockEditV21);
            this.pnl_Edit.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnl_Edit.Location = new System.Drawing.Point(24, 0);
            this.pnl_Edit.Name = "pnl_Edit";
            this.pnl_Edit.Size = new System.Drawing.Size(595, 767);
            this.pnl_Edit.TabIndex = 2;
            // 
            // btn_ToggleSidebar
            // 
            this.btn_ToggleSidebar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.btn_ToggleSidebar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_ToggleSidebar.Dock = System.Windows.Forms.DockStyle.Left;
            this.btn_ToggleSidebar.FlatAppearance.BorderSize = 0;
            this.btn_ToggleSidebar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_ToggleSidebar.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold);
            this.btn_ToggleSidebar.ForeColor = System.Drawing.Color.White;
            this.btn_ToggleSidebar.Location = new System.Drawing.Point(0, 0);
            this.btn_ToggleSidebar.Name = "btn_ToggleSidebar";
            this.btn_ToggleSidebar.Size = new System.Drawing.Size(24, 767);
            this.btn_ToggleSidebar.TabIndex = 1;
            this.btn_ToggleSidebar.Text = "◀";
            this.btn_ToggleSidebar.UseVisualStyleBackColor = false;
            // 
            // btn_Seg
            // 
            this.btn_Seg.BackColor = System.Drawing.Color.LightSeaGreen;
            this.btn_Seg.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_Seg.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_Seg.Location = new System.Drawing.Point(8, 363);
            this.btn_Seg.Name = "btn_Seg";
            this.btn_Seg.Size = new System.Drawing.Size(90, 35);
            this.btn_Seg.TabIndex = 15;
            this.btn_Seg.Text = "语义分割";
            this.btn_Seg.UseVisualStyleBackColor = false;
            // 
            // btn_OCR
            // 
            this.btn_OCR.BackColor = System.Drawing.Color.LightSeaGreen;
            this.btn_OCR.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_OCR.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_OCR.Location = new System.Drawing.Point(108, 363);
            this.btn_OCR.Name = "btn_OCR";
            this.btn_OCR.Size = new System.Drawing.Size(95, 35);
            this.btn_OCR.TabIndex = 16;
            this.btn_OCR.Text = "OCR";
            this.btn_OCR.UseVisualStyleBackColor = false;
            // 
            // Frm_Tool
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ClientSize = new System.Drawing.Size(1455, 767);
            this.Controls.Add(this.splitContainer1);
            this.Name = "Frm_Tool";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Frm_Tool_FormClosed);
            this.Load += new System.EventHandler(this.Frm_Tool_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgv_Data)).EndInit();
            this.panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.cogToolBlockEditV21)).EndInit();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.pnl_Edit.ResumeLayout(false);
            this.ResumeLayout(false);

    }

    #endregion

    private Cognex.VisionPro.ToolBlock.CogToolBlockEditV2 cogToolBlockEditV21;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    private System.Windows.Forms.DataGridView dgv_Data;
    private System.Windows.Forms.Panel panel2;
    private System.Windows.Forms.Button btn_Remove;
    private System.Windows.Forms.Button btn_MoveDown;
    private System.Windows.Forms.Button btn_MoveUp;
    private System.Windows.Forms.Button btn_BoolArray;
    private System.Windows.Forms.Button btn_Bool;
    private System.Windows.Forms.Button btn_StringArray;
    private System.Windows.Forms.Button btn_String;
    private System.Windows.Forms.Button btn_FloatArray;
    private System.Windows.Forms.Button btn_Float;
    private System.Windows.Forms.Button btn_DoubleArray;
    private System.Windows.Forms.Button btn_Double;
    private System.Windows.Forms.Button btn_ShortArray;
    private System.Windows.Forms.Button btn_Short;
    private System.Windows.Forms.Button btn_IntArray;
    private System.Windows.Forms.Button btn_Int;
    private System.Windows.Forms.SplitContainer splitContainer1;
    private System.Windows.Forms.Button btn_ToggleSidebar;
    private System.Windows.Forms.Panel pnl_Edit;
        private System.Windows.Forms.Button btn_OCR;
        private System.Windows.Forms.Button btn_Seg;
    }
}
