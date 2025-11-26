namespace Vision.Frm.Modbus
{
	partial class Frm_Comm
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.dgv_Devices = new System.Windows.Forms.DataGridView();
            this.col_DeviceName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col_DeviceType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col_DeviceStatus = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.btn_Save = new System.Windows.Forms.Button();
            this.btn_Add = new AntdUI.Button();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.btn_Remove = new AntdUI.Button();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_Devices)).BeginInit();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.Controls.Add(this.dgv_Devices);
            this.panel1.Location = new System.Drawing.Point(12, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(384, 566);
            this.panel1.TabIndex = 0;
            // 
            // dgv_Devices
            // 
            this.dgv_Devices.AllowUserToAddRows = false;
            this.dgv_Devices.AllowUserToDeleteRows = false;
            this.dgv_Devices.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgv_Devices.BackgroundColor = System.Drawing.SystemColors.ActiveCaption;
            this.dgv_Devices.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv_Devices.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.col_DeviceName,
            this.col_DeviceType,
            this.col_DeviceStatus});
            this.dgv_Devices.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgv_Devices.Location = new System.Drawing.Point(0, 0);
            this.dgv_Devices.MultiSelect = false;
            this.dgv_Devices.Name = "dgv_Devices";
            this.dgv_Devices.RowHeadersVisible = false;
            this.dgv_Devices.RowHeadersWidth = 62;
            this.dgv_Devices.RowTemplate.Height = 28;
            this.dgv_Devices.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgv_Devices.Size = new System.Drawing.Size(384, 566);
            this.dgv_Devices.TabIndex = 0;
            // 
            // col_DeviceName
            // 
            this.col_DeviceName.DataPropertyName = "Name";
            this.col_DeviceName.HeaderText = "设备名称";
            this.col_DeviceName.MinimumWidth = 8;
            this.col_DeviceName.Name = "col_DeviceName";
            // 
            // col_DeviceType
            // 
            this.col_DeviceType.DataPropertyName = "Type";
            this.col_DeviceType.HeaderText = "设备类型";
            this.col_DeviceType.MinimumWidth = 8;
            this.col_DeviceType.Name = "col_DeviceType";
            this.col_DeviceType.ReadOnly = true;
            // 
            // col_DeviceStatus
            // 
            this.col_DeviceStatus.DataPropertyName = "Status";
            this.col_DeviceStatus.HeaderText = "设备状态";
            this.col_DeviceStatus.MinimumWidth = 8;
            this.col_DeviceStatus.Name = "col_DeviceStatus";
            this.col_DeviceStatus.ReadOnly = true;
            // 
            // panel2
            // 
            this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel2.Location = new System.Drawing.Point(474, 12);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(895, 566);
            this.panel2.TabIndex = 1;
            // 
            // panel3
            // 
            this.panel3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel3.Controls.Add(this.btn_Save);
            this.panel3.Location = new System.Drawing.Point(12, 584);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(1357, 50);
            this.panel3.TabIndex = 1;
            // 
            // btn_Save
            // 
            this.btn_Save.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_Save.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_Save.Location = new System.Drawing.Point(1202, 10);
            this.btn_Save.Name = "btn_Save";
            this.btn_Save.Size = new System.Drawing.Size(137, 30);
            this.btn_Save.TabIndex = 2;
            this.btn_Save.Text = "保存";
            this.btn_Save.UseVisualStyleBackColor = true;
            // 
            // btn_Add
            // 
            this.btn_Add.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_Add.Font = new System.Drawing.Font("宋体", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn_Add.Location = new System.Drawing.Point(399, 136);
            this.btn_Add.Name = "btn_Add";
            this.btn_Add.Shape = AntdUI.TShape.Circle;
            this.btn_Add.Size = new System.Drawing.Size(70, 39);
            this.btn_Add.TabIndex = 3;
            this.btn_Add.Text = "+";
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.contextMenuStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(61, 4);
            // 
            // btn_Remove
            // 
            this.btn_Remove.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_Remove.Font = new System.Drawing.Font("宋体", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn_Remove.Location = new System.Drawing.Point(399, 213);
            this.btn_Remove.Name = "btn_Remove";
            this.btn_Remove.Shape = AntdUI.TShape.Circle;
            this.btn_Remove.Size = new System.Drawing.Size(70, 39);
            this.btn_Remove.TabIndex = 4;
            this.btn_Remove.Text = "-";
            // 
            // Frm_Comm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ClientSize = new System.Drawing.Size(1381, 640);
            this.Controls.Add(this.btn_Add);
            this.Controls.Add(this.btn_Remove);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Name = "Frm_Comm";
            this.Text = "Frm_Comm";
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgv_Devices)).EndInit();
            this.panel3.ResumeLayout(false);
            this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.Panel panel3;
		private AntdUI.Button btn_Add;
		private System.Windows.Forms.Button btn_Save;
		private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
		private AntdUI.Button btn_Remove;
		private System.Windows.Forms.DataGridView dgv_Devices;
		private System.Windows.Forms.DataGridViewTextBoxColumn col_DeviceName;
		private System.Windows.Forms.DataGridViewTextBoxColumn col_DeviceType;
		private System.Windows.Forms.DataGridViewTextBoxColumn col_DeviceStatus;
	}
}