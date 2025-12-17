namespace HardwareCommNet.UI
{
    partial class Frm_CommConfig
    {
        /// <summary> 
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.flowPanel_Devices = new System.Windows.Forms.FlowLayoutPanel();
            this.panel_DeviceHeader = new System.Windows.Forms.Panel();
            this.lbl_DeviceList = new AntdUI.Label();
            this.panel_DeviceButtons = new System.Windows.Forms.Panel();
            this.btn_Add = new AntdUI.Button();
            this.btn_Remove = new AntdUI.Button();
            this.panel_ConfigArea = new System.Windows.Forms.Panel();
            this.lbl_ConfigTip = new AntdUI.Label();
            this.contextMenu_Add = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.contextMenu_Device = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmi_Rename = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmi_CommTable = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.panel_DeviceHeader.SuspendLayout();
            this.panel_DeviceButtons.SuspendLayout();
            this.panel_ConfigArea.SuspendLayout();
            this.contextMenu_Device.SuspendLayout();
            this.SuspendLayout();
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
            this.splitContainer1.Panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.splitContainer1.Panel1.Controls.Add(this.flowPanel_Devices);
            this.splitContainer1.Panel1.Controls.Add(this.panel_DeviceHeader);
            this.splitContainer1.Panel1.Controls.Add(this.panel_DeviceButtons);
            this.splitContainer1.Panel1MinSize = 220;
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(37)))), ((int)(((byte)(38)))));
            this.splitContainer1.Panel2.Controls.Add(this.panel_ConfigArea);
            this.splitContainer1.Size = new System.Drawing.Size(1100, 900);
            this.splitContainer1.SplitterDistance = 240;
            this.splitContainer1.TabIndex = 0;
            // 
            // flowPanel_Devices
            // 
            this.flowPanel_Devices.AutoScroll = true;
            this.flowPanel_Devices.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.flowPanel_Devices.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowPanel_Devices.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowPanel_Devices.Location = new System.Drawing.Point(0, 40);
            this.flowPanel_Devices.Name = "flowPanel_Devices";
            this.flowPanel_Devices.Padding = new System.Windows.Forms.Padding(2);
            this.flowPanel_Devices.Size = new System.Drawing.Size(240, 800);
            this.flowPanel_Devices.TabIndex = 2;
            this.flowPanel_Devices.WrapContents = false;
            // 
            // panel_DeviceHeader
            // 
            this.panel_DeviceHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(62)))), ((int)(((byte)(62)))), ((int)(((byte)(66)))));
            this.panel_DeviceHeader.Controls.Add(this.lbl_DeviceList);
            this.panel_DeviceHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel_DeviceHeader.Location = new System.Drawing.Point(0, 0);
            this.panel_DeviceHeader.Name = "panel_DeviceHeader";
            this.panel_DeviceHeader.Size = new System.Drawing.Size(240, 40);
            this.panel_DeviceHeader.TabIndex = 0;
            // 
            // lbl_DeviceList
            // 
            this.lbl_DeviceList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbl_DeviceList.Font = new System.Drawing.Font("微软雅黑", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lbl_DeviceList.ForeColor = System.Drawing.Color.White;
            this.lbl_DeviceList.Location = new System.Drawing.Point(0, 0);
            this.lbl_DeviceList.Name = "lbl_DeviceList";
            this.lbl_DeviceList.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.lbl_DeviceList.Size = new System.Drawing.Size(240, 40);
            this.lbl_DeviceList.TabIndex = 0;
            this.lbl_DeviceList.Text = "通讯设备列表";
            // 
            // panel_DeviceButtons
            // 
            this.panel_DeviceButtons.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(55)))));
            this.panel_DeviceButtons.Controls.Add(this.btn_Add);
            this.panel_DeviceButtons.Controls.Add(this.btn_Remove);
            this.panel_DeviceButtons.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel_DeviceButtons.Location = new System.Drawing.Point(0, 840);
            this.panel_DeviceButtons.Name = "panel_DeviceButtons";
            this.panel_DeviceButtons.Padding = new System.Windows.Forms.Padding(8);
            this.panel_DeviceButtons.Size = new System.Drawing.Size(240, 60);
            this.panel_DeviceButtons.TabIndex = 3;
            // 
            // btn_Add
            // 
            this.btn_Add.Dock = System.Windows.Forms.DockStyle.Right;
            this.btn_Add.Font = new System.Drawing.Font("微软雅黑", 10F, System.Drawing.FontStyle.Bold);
            this.btn_Add.ForeColor = System.Drawing.Color.White;
            this.btn_Add.Location = new System.Drawing.Point(124, 8);
            this.btn_Add.Name = "btn_Add";
            this.btn_Add.Size = new System.Drawing.Size(108, 44);
            this.btn_Add.TabIndex = 0;
            this.btn_Add.Text = "添加";
            this.btn_Add.Type = AntdUI.TTypeMini.Primary;
            this.btn_Add.Click += new System.EventHandler(this.btn_Add_Click);
            // 
            // btn_Remove
            // 
            this.btn_Remove.Dock = System.Windows.Forms.DockStyle.Left;
            this.btn_Remove.Font = new System.Drawing.Font("微软雅黑", 10F, System.Drawing.FontStyle.Bold);
            this.btn_Remove.ForeColor = System.Drawing.Color.White;
            this.btn_Remove.Location = new System.Drawing.Point(8, 8);
            this.btn_Remove.Name = "btn_Remove";
            this.btn_Remove.Size = new System.Drawing.Size(108, 44);
            this.btn_Remove.TabIndex = 1;
            this.btn_Remove.Text = "删除";
            this.btn_Remove.Type = AntdUI.TTypeMini.Error;
            this.btn_Remove.Click += new System.EventHandler(this.btn_Remove_Click);
            // 
            // panel_ConfigArea
            // 
            this.panel_ConfigArea.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(37)))), ((int)(((byte)(38)))));
            this.panel_ConfigArea.Controls.Add(this.lbl_ConfigTip);
            this.panel_ConfigArea.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel_ConfigArea.Location = new System.Drawing.Point(0, 0);
            this.panel_ConfigArea.Name = "panel_ConfigArea";
            this.panel_ConfigArea.Size = new System.Drawing.Size(856, 900);
            this.panel_ConfigArea.TabIndex = 0;
            // 
            // lbl_ConfigTip
            // 
            this.lbl_ConfigTip.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbl_ConfigTip.Font = new System.Drawing.Font("微软雅黑", 11F);
            this.lbl_ConfigTip.ForeColor = System.Drawing.Color.Gray;
            this.lbl_ConfigTip.Location = new System.Drawing.Point(0, 0);
            this.lbl_ConfigTip.Name = "lbl_ConfigTip";
            this.lbl_ConfigTip.Size = new System.Drawing.Size(856, 900);
            this.lbl_ConfigTip.TabIndex = 0;
            this.lbl_ConfigTip.Text = "点击左侧设备查看配置\r\n右键设备显示更多操作";
            this.lbl_ConfigTip.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // contextMenu_Add
            // 
            this.contextMenu_Add.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.contextMenu_Add.Name = "contextMenu_Add";
            this.contextMenu_Add.Size = new System.Drawing.Size(61, 4);
            // 
            // contextMenu_Device
            // 
            this.contextMenu_Device.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.contextMenu_Device.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmi_Rename,
            this.toolStripSeparator1,
            this.tsmi_CommTable});
            this.contextMenu_Device.Name = "contextMenu_Device";
            this.contextMenu_Device.Size = new System.Drawing.Size(241, 103);
            // 
            // tsmi_Rename
            // 
            this.tsmi_Rename.Name = "tsmi_Rename";
            this.tsmi_Rename.Size = new System.Drawing.Size(240, 30);
            this.tsmi_Rename.Text = "重命名";
            this.tsmi_Rename.Click += new System.EventHandler(this.tsmi_Rename_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(237, 6);
            // 
            // tsmi_CommTable
            // 
            this.tsmi_CommTable.Name = "tsmi_CommTable";
            this.tsmi_CommTable.Size = new System.Drawing.Size(240, 30);
            this.tsmi_CommTable.Text = "通讯表配置";
            this.tsmi_CommTable.Click += new System.EventHandler(this.tsmi_CommTable_Click);
            // 
            // Frm_CommConfig
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.Controls.Add(this.splitContainer1);
            this.MinimumSize = new System.Drawing.Size(1100, 800);
            this.Name = "Frm_CommConfig";
            this.Size = new System.Drawing.Size(1100, 900);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.panel_DeviceHeader.ResumeLayout(false);
            this.panel_DeviceButtons.ResumeLayout(false);
            this.panel_ConfigArea.ResumeLayout(false);
            this.contextMenu_Device.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.FlowLayoutPanel flowPanel_Devices;
        private System.Windows.Forms.Panel panel_DeviceHeader;
        private AntdUI.Label lbl_DeviceList;
        private System.Windows.Forms.Panel panel_DeviceButtons;
        private AntdUI.Button btn_Remove;
        private AntdUI.Button btn_Add;
        private System.Windows.Forms.Panel panel_ConfigArea;
        private AntdUI.Label lbl_ConfigTip;
        private System.Windows.Forms.ContextMenuStrip contextMenu_Add;
        private System.Windows.Forms.ContextMenuStrip contextMenu_Device;
        private System.Windows.Forms.ToolStripMenuItem tsmi_Rename;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem tsmi_CommTable;
    }
}
