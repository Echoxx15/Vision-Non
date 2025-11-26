namespace HardwareCommNet.UI
{
	partial class uInputTable
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.dgv_Data = new System.Windows.Forms.DataGridView();
            this.panel2 = new System.Windows.Forms.Panel();
            this.btn_ShortArray = new System.Windows.Forms.Button();
            this.btn_Short = new System.Windows.Forms.Button();
            this.btn_FLoatArray = new System.Windows.Forms.Button();
            this.btn_Float = new System.Windows.Forms.Button();
            this.btn_Remove = new System.Windows.Forms.Button();
            this.btn_MoveDown = new System.Windows.Forms.Button();
            this.btn_MoveUp = new System.Windows.Forms.Button();
            this.btn_BoolArray = new System.Windows.Forms.Button();
            this.btn_Bool = new System.Windows.Forms.Button();
            this.btn_StringArray = new System.Windows.Forms.Button();
            this.btn_String = new System.Windows.Forms.Button();
            this.btn_DoubleArray = new System.Windows.Forms.Button();
            this.btn_Double = new System.Windows.Forms.Button();
            this.btn_IntArray = new System.Windows.Forms.Button();
            this.btn_Int = new System.Windows.Forms.Button();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_Data)).BeginInit();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1482, 725);
            this.tableLayoutPanel1.TabIndex = 3;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 85.0271F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 14.9729F));
            this.tableLayoutPanel2.Controls.Add(this.dgv_Data, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.panel2, 1, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(1476, 719);
            this.tableLayoutPanel2.TabIndex = 2;
            // 
            // dgv_Data
            // 
            this.dgv_Data.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
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
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgv_Data.DefaultCellStyle = dataGridViewCellStyle2;
            this.dgv_Data.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgv_Data.Location = new System.Drawing.Point(3, 3);
            this.dgv_Data.Name = "dgv_Data";
            this.dgv_Data.RowHeadersWidth = 62;
            this.dgv_Data.RowTemplate.Height = 30;
            this.dgv_Data.Size = new System.Drawing.Size(1249, 713);
            this.dgv_Data.TabIndex = 0;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.panel2.Controls.Add(this.btn_ShortArray);
            this.panel2.Controls.Add(this.btn_Short);
            this.panel2.Controls.Add(this.btn_FLoatArray);
            this.panel2.Controls.Add(this.btn_Float);
            this.panel2.Controls.Add(this.btn_Remove);
            this.panel2.Controls.Add(this.btn_MoveDown);
            this.panel2.Controls.Add(this.btn_MoveUp);
            this.panel2.Controls.Add(this.btn_BoolArray);
            this.panel2.Controls.Add(this.btn_Bool);
            this.panel2.Controls.Add(this.btn_StringArray);
            this.panel2.Controls.Add(this.btn_String);
            this.panel2.Controls.Add(this.btn_DoubleArray);
            this.panel2.Controls.Add(this.btn_Double);
            this.panel2.Controls.Add(this.btn_IntArray);
            this.panel2.Controls.Add(this.btn_Int);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(1258, 3);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(215, 713);
            this.panel2.TabIndex = 1;
            // 
            // btn_ShortArray
            // 
            this.btn_ShortArray.BackColor = System.Drawing.Color.LightSeaGreen;
            this.btn_ShortArray.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_ShortArray.Enabled = false;
            this.btn_ShortArray.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_ShortArray.Location = new System.Drawing.Point(105, 183);
            this.btn_ShortArray.Name = "btn_ShortArray";
            this.btn_ShortArray.Size = new System.Drawing.Size(95, 35);
            this.btn_ShortArray.TabIndex = 14;
            this.btn_ShortArray.Text = "Short[]";
            this.btn_ShortArray.UseVisualStyleBackColor = false;
            // 
            // btn_Short
            // 
            this.btn_Short.BackColor = System.Drawing.Color.LightSeaGreen;
            this.btn_Short.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_Short.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_Short.Location = new System.Drawing.Point(16, 183);
            this.btn_Short.Name = "btn_Short";
            this.btn_Short.Size = new System.Drawing.Size(75, 35);
            this.btn_Short.TabIndex = 13;
            this.btn_Short.Text = "Short";
            this.btn_Short.UseVisualStyleBackColor = false;
            // 
            // btn_FLoatArray
            // 
            this.btn_FLoatArray.BackColor = System.Drawing.Color.LightSeaGreen;
            this.btn_FLoatArray.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_FLoatArray.Enabled = false;
            this.btn_FLoatArray.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_FLoatArray.Location = new System.Drawing.Point(105, 138);
            this.btn_FLoatArray.Name = "btn_FLoatArray";
            this.btn_FLoatArray.Size = new System.Drawing.Size(95, 35);
            this.btn_FLoatArray.TabIndex = 12;
            this.btn_FLoatArray.Text = "Float[]";
            this.btn_FLoatArray.UseVisualStyleBackColor = false;
            // 
            // btn_Float
            // 
            this.btn_Float.BackColor = System.Drawing.Color.LightSeaGreen;
            this.btn_Float.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_Float.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_Float.Location = new System.Drawing.Point(16, 138);
            this.btn_Float.Name = "btn_Float";
            this.btn_Float.Size = new System.Drawing.Size(75, 35);
            this.btn_Float.TabIndex = 11;
            this.btn_Float.Text = "Float";
            this.btn_Float.UseVisualStyleBackColor = false;
            // 
            // btn_Remove
            // 
            this.btn_Remove.BackColor = System.Drawing.Color.LightSeaGreen;
            this.btn_Remove.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_Remove.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_Remove.Location = new System.Drawing.Point(61, 475);
            this.btn_Remove.Name = "btn_Remove";
            this.btn_Remove.Size = new System.Drawing.Size(75, 35);
            this.btn_Remove.TabIndex = 10;
            this.btn_Remove.Text = "删除";
            this.btn_Remove.UseVisualStyleBackColor = false;
            // 
            // btn_MoveDown
            // 
            this.btn_MoveDown.BackColor = System.Drawing.Color.LightSeaGreen;
            this.btn_MoveDown.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_MoveDown.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_MoveDown.Location = new System.Drawing.Point(61, 414);
            this.btn_MoveDown.Name = "btn_MoveDown";
            this.btn_MoveDown.Size = new System.Drawing.Size(75, 35);
            this.btn_MoveDown.TabIndex = 9;
            this.btn_MoveDown.Text = "下移";
            this.btn_MoveDown.UseVisualStyleBackColor = false;
            // 
            // btn_MoveUp
            // 
            this.btn_MoveUp.BackColor = System.Drawing.Color.LightSeaGreen;
            this.btn_MoveUp.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_MoveUp.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_MoveUp.Location = new System.Drawing.Point(61, 357);
            this.btn_MoveUp.Name = "btn_MoveUp";
            this.btn_MoveUp.Size = new System.Drawing.Size(75, 35);
            this.btn_MoveUp.TabIndex = 8;
            this.btn_MoveUp.Text = "上移";
            this.btn_MoveUp.UseVisualStyleBackColor = false;
            // 
            // btn_BoolArray
            // 
            this.btn_BoolArray.BackColor = System.Drawing.Color.LightSeaGreen;
            this.btn_BoolArray.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_BoolArray.Enabled = false;
            this.btn_BoolArray.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_BoolArray.Location = new System.Drawing.Point(105, 284);
            this.btn_BoolArray.Name = "btn_BoolArray";
            this.btn_BoolArray.Size = new System.Drawing.Size(95, 35);
            this.btn_BoolArray.TabIndex = 7;
            this.btn_BoolArray.Text = "Bool[]";
            this.btn_BoolArray.UseVisualStyleBackColor = false;
            // 
            // btn_Bool
            // 
            this.btn_Bool.BackColor = System.Drawing.Color.LightSeaGreen;
            this.btn_Bool.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_Bool.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_Bool.Location = new System.Drawing.Point(16, 284);
            this.btn_Bool.Name = "btn_Bool";
            this.btn_Bool.Size = new System.Drawing.Size(75, 35);
            this.btn_Bool.TabIndex = 6;
            this.btn_Bool.Text = "Bool";
            this.btn_Bool.UseVisualStyleBackColor = false;
            // 
            // btn_StringArray
            // 
            this.btn_StringArray.BackColor = System.Drawing.Color.LightSeaGreen;
            this.btn_StringArray.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_StringArray.Enabled = false;
            this.btn_StringArray.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_StringArray.Location = new System.Drawing.Point(105, 229);
            this.btn_StringArray.Name = "btn_StringArray";
            this.btn_StringArray.Size = new System.Drawing.Size(95, 35);
            this.btn_StringArray.TabIndex = 5;
            this.btn_StringArray.Text = "String[]";
            this.btn_StringArray.UseVisualStyleBackColor = false;
            // 
            // btn_String
            // 
            this.btn_String.BackColor = System.Drawing.Color.LightSeaGreen;
            this.btn_String.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_String.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_String.Location = new System.Drawing.Point(16, 229);
            this.btn_String.Name = "btn_String";
            this.btn_String.Size = new System.Drawing.Size(75, 35);
            this.btn_String.TabIndex = 4;
            this.btn_String.Text = "String";
            this.btn_String.UseVisualStyleBackColor = false;
            // 
            // btn_DoubleArray
            // 
            this.btn_DoubleArray.BackColor = System.Drawing.Color.LightSeaGreen;
            this.btn_DoubleArray.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_DoubleArray.Enabled = false;
            this.btn_DoubleArray.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_DoubleArray.Location = new System.Drawing.Point(105, 90);
            this.btn_DoubleArray.Name = "btn_DoubleArray";
            this.btn_DoubleArray.Size = new System.Drawing.Size(95, 35);
            this.btn_DoubleArray.TabIndex = 3;
            this.btn_DoubleArray.Text = "Double[]";
            this.btn_DoubleArray.UseVisualStyleBackColor = false;
            // 
            // btn_Double
            // 
            this.btn_Double.BackColor = System.Drawing.Color.LightSeaGreen;
            this.btn_Double.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_Double.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_Double.Location = new System.Drawing.Point(16, 90);
            this.btn_Double.Name = "btn_Double";
            this.btn_Double.Size = new System.Drawing.Size(75, 35);
            this.btn_Double.TabIndex = 2;
            this.btn_Double.Text = "Double";
            this.btn_Double.UseVisualStyleBackColor = false;
            // 
            // btn_IntArray
            // 
            this.btn_IntArray.BackColor = System.Drawing.Color.LightSeaGreen;
            this.btn_IntArray.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_IntArray.Enabled = false;
            this.btn_IntArray.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_IntArray.Location = new System.Drawing.Point(105, 40);
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
            this.btn_Int.Location = new System.Drawing.Point(16, 40);
            this.btn_Int.Name = "btn_Int";
            this.btn_Int.Size = new System.Drawing.Size(75, 35);
            this.btn_Int.TabIndex = 0;
            this.btn_Int.Text = "Int";
            this.btn_Int.UseVisualStyleBackColor = false;
            // 
            // uInputTable
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "uInputTable";
            this.Size = new System.Drawing.Size(1482, 725);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgv_Data)).EndInit();
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
		private System.Windows.Forms.DataGridView dgv_Data;
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.Button btn_Remove;
		private System.Windows.Forms.Button btn_MoveDown;
		private System.Windows.Forms.Button btn_MoveUp;
		private System.Windows.Forms.Button btn_BoolArray;
		private System.Windows.Forms.Button btn_Bool;
		private System.Windows.Forms.Button btn_StringArray;
		private System.Windows.Forms.Button btn_String;
		private System.Windows.Forms.Button btn_DoubleArray;
		private System.Windows.Forms.Button btn_Double;
		private System.Windows.Forms.Button btn_IntArray;
		private System.Windows.Forms.Button btn_Int;
  private System.Windows.Forms.Button btn_FLoatArray;
  private System.Windows.Forms.Button btn_Float;
		private System.Windows.Forms.Button btn_ShortArray;
		private System.Windows.Forms.Button btn_Short;
	}
}
