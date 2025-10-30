namespace Vision.UserUI
{
  partial class uAddValueCollection
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
      this.dgv_Data = new System.Windows.Forms.DataGridView();
      this.panel1 = new System.Windows.Forms.Panel();
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
      ((System.ComponentModel.ISupportInitialize)(this.dgv_Data)).BeginInit();
      this.panel1.SuspendLayout();
      this.SuspendLayout();
      // 
      // tableLayoutPanel1
      // 
      this.tableLayoutPanel1.ColumnCount = 2;
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 77.09164F));
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 22.90837F));
      this.tableLayoutPanel1.Controls.Add(this.dgv_Data, 0, 0);
      this.tableLayoutPanel1.Controls.Add(this.panel1, 1, 0);
      this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
      this.tableLayoutPanel1.Name = "tableLayoutPanel1";
      this.tableLayoutPanel1.RowCount = 1;
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.tableLayoutPanel1.Size = new System.Drawing.Size(927, 723);
      this.tableLayoutPanel1.TabIndex = 0;
      // 
      // dgv_Data
      // 
      this.dgv_Data.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
      this.dgv_Data.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      this.dgv_Data.Dock = System.Windows.Forms.DockStyle.Fill;
      this.dgv_Data.Location = new System.Drawing.Point(3, 3);
      this.dgv_Data.Name = "dgv_Data";
      this.dgv_Data.RowHeadersWidth = 62;
      this.dgv_Data.RowTemplate.Height = 30;
      this.dgv_Data.Size = new System.Drawing.Size(708, 717);
      this.dgv_Data.TabIndex = 0;
      // 
      // panel1
      // 
      this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
      this.panel1.Controls.Add(this.btn_Remove);
      this.panel1.Controls.Add(this.btn_MoveDown);
      this.panel1.Controls.Add(this.btn_MoveUp);
      this.panel1.Controls.Add(this.btn_BoolArray);
      this.panel1.Controls.Add(this.btn_Bool);
      this.panel1.Controls.Add(this.btn_StringArray);
      this.panel1.Controls.Add(this.btn_String);
      this.panel1.Controls.Add(this.btn_DoubleArray);
      this.panel1.Controls.Add(this.btn_Double);
      this.panel1.Controls.Add(this.btn_IntArray);
      this.panel1.Controls.Add(this.btn_Int);
      this.panel1.Location = new System.Drawing.Point(717, 3);
      this.panel1.Name = "panel1";
      this.panel1.Size = new System.Drawing.Size(207, 717);
      this.panel1.TabIndex = 1;
      // 
      // btn_Remove
      // 
      this.btn_Remove.BackColor = System.Drawing.Color.LightSeaGreen;
      this.btn_Remove.Cursor = System.Windows.Forms.Cursors.Hand;
      this.btn_Remove.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.btn_Remove.Location = new System.Drawing.Point(61, 386);
      this.btn_Remove.Name = "btn_Remove";
      this.btn_Remove.Size = new System.Drawing.Size(75, 30);
      this.btn_Remove.TabIndex = 10;
      this.btn_Remove.Text = "删除";
      this.btn_Remove.UseVisualStyleBackColor = false;
      // 
      // btn_MoveDown
      // 
      this.btn_MoveDown.BackColor = System.Drawing.Color.LightSeaGreen;
      this.btn_MoveDown.Cursor = System.Windows.Forms.Cursors.Hand;
      this.btn_MoveDown.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.btn_MoveDown.Location = new System.Drawing.Point(61, 325);
      this.btn_MoveDown.Name = "btn_MoveDown";
      this.btn_MoveDown.Size = new System.Drawing.Size(75, 30);
      this.btn_MoveDown.TabIndex = 9;
      this.btn_MoveDown.Text = "下移";
      this.btn_MoveDown.UseVisualStyleBackColor = false;
      // 
      // btn_MoveUp
      // 
      this.btn_MoveUp.BackColor = System.Drawing.Color.LightSeaGreen;
      this.btn_MoveUp.Cursor = System.Windows.Forms.Cursors.Hand;
      this.btn_MoveUp.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.btn_MoveUp.Location = new System.Drawing.Point(61, 268);
      this.btn_MoveUp.Name = "btn_MoveUp";
      this.btn_MoveUp.Size = new System.Drawing.Size(75, 30);
      this.btn_MoveUp.TabIndex = 8;
      this.btn_MoveUp.Text = "上移";
      this.btn_MoveUp.UseVisualStyleBackColor = false;
      // 
      // btn_BoolArray
      // 
      this.btn_BoolArray.BackColor = System.Drawing.Color.LightSeaGreen;
      this.btn_BoolArray.Cursor = System.Windows.Forms.Cursors.Hand;
      this.btn_BoolArray.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.btn_BoolArray.Location = new System.Drawing.Point(105, 195);
      this.btn_BoolArray.Name = "btn_BoolArray";
      this.btn_BoolArray.Size = new System.Drawing.Size(90, 30);
      this.btn_BoolArray.TabIndex = 7;
      this.btn_BoolArray.Text = "Bool[]";
      this.btn_BoolArray.UseVisualStyleBackColor = false;
      // 
      // btn_Bool
      // 
      this.btn_Bool.BackColor = System.Drawing.Color.LightSeaGreen;
      this.btn_Bool.Cursor = System.Windows.Forms.Cursors.Hand;
      this.btn_Bool.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.btn_Bool.Location = new System.Drawing.Point(16, 195);
      this.btn_Bool.Name = "btn_Bool";
      this.btn_Bool.Size = new System.Drawing.Size(75, 30);
      this.btn_Bool.TabIndex = 6;
      this.btn_Bool.Text = "Bool";
      this.btn_Bool.UseVisualStyleBackColor = false;
      // 
      // btn_StringArray
      // 
      this.btn_StringArray.BackColor = System.Drawing.Color.LightSeaGreen;
      this.btn_StringArray.Cursor = System.Windows.Forms.Cursors.Hand;
      this.btn_StringArray.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.btn_StringArray.Location = new System.Drawing.Point(105, 140);
      this.btn_StringArray.Name = "btn_StringArray";
      this.btn_StringArray.Size = new System.Drawing.Size(90, 30);
      this.btn_StringArray.TabIndex = 5;
      this.btn_StringArray.Text = "String[]";
      this.btn_StringArray.UseVisualStyleBackColor = false;
      // 
      // btn_String
      // 
      this.btn_String.BackColor = System.Drawing.Color.LightSeaGreen;
      this.btn_String.Cursor = System.Windows.Forms.Cursors.Hand;
      this.btn_String.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.btn_String.Location = new System.Drawing.Point(16, 140);
      this.btn_String.Name = "btn_String";
      this.btn_String.Size = new System.Drawing.Size(75, 30);
      this.btn_String.TabIndex = 4;
      this.btn_String.Text = "String";
      this.btn_String.UseVisualStyleBackColor = false;
      // 
      // btn_DoubleArray
      // 
      this.btn_DoubleArray.BackColor = System.Drawing.Color.LightSeaGreen;
      this.btn_DoubleArray.Cursor = System.Windows.Forms.Cursors.Hand;
      this.btn_DoubleArray.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.btn_DoubleArray.Location = new System.Drawing.Point(105, 90);
      this.btn_DoubleArray.Name = "btn_DoubleArray";
      this.btn_DoubleArray.Size = new System.Drawing.Size(90, 30);
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
      this.btn_Double.Size = new System.Drawing.Size(75, 30);
      this.btn_Double.TabIndex = 2;
      this.btn_Double.Text = "Double";
      this.btn_Double.UseVisualStyleBackColor = false;
      // 
      // btn_IntArray
      // 
      this.btn_IntArray.BackColor = System.Drawing.Color.LightSeaGreen;
      this.btn_IntArray.Cursor = System.Windows.Forms.Cursors.Hand;
      this.btn_IntArray.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.btn_IntArray.Location = new System.Drawing.Point(105, 40);
      this.btn_IntArray.Name = "btn_IntArray";
      this.btn_IntArray.Size = new System.Drawing.Size(90, 30);
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
      this.btn_Int.Size = new System.Drawing.Size(75, 30);
      this.btn_Int.TabIndex = 0;
      this.btn_Int.Text = "Int";
      this.btn_Int.UseVisualStyleBackColor = false;
      // 
      // uAddValueCollection
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.tableLayoutPanel1);
      this.Name = "uAddValueCollection";
      this.Size = new System.Drawing.Size(927, 723);
      this.tableLayoutPanel1.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.dgv_Data)).EndInit();
      this.panel1.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    private System.Windows.Forms.DataGridView dgv_Data;
    private System.Windows.Forms.Panel panel1;
    private System.Windows.Forms.Button btn_Int;
    private System.Windows.Forms.Button btn_BoolArray;
    private System.Windows.Forms.Button btn_Bool;
    private System.Windows.Forms.Button btn_StringArray;
    private System.Windows.Forms.Button btn_String;
    private System.Windows.Forms.Button btn_DoubleArray;
    private System.Windows.Forms.Button btn_Double;
    private System.Windows.Forms.Button btn_IntArray;
    private System.Windows.Forms.Button btn_Remove;
    private System.Windows.Forms.Button btn_MoveDown;
    private System.Windows.Forms.Button btn_MoveUp;
  }
}
