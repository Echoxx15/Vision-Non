namespace LightControlNet.UI
{
    partial class uLightConfig
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
            this.panel_DeviceButtons = new System.Windows.Forms.Panel();
            this.btn_Remove = new AntdUI.Button();
            this.btn_Add = new AntdUI.Button();
            this.panel_ConfigArea = new System.Windows.Forms.Panel();
            this.panel_Params = new System.Windows.Forms.Panel();
            this.btn_Test = new System.Windows.Forms.Button();
            this.btn_Save = new System.Windows.Forms.Button();
            this.cmb_ChannelCount = new System.Windows.Forms.ComboBox();
            this.label11 = new System.Windows.Forms.Label();
            this.cmb_Parity = new System.Windows.Forms.ComboBox();
            this.label10 = new System.Windows.Forms.Label();
            this.cmb_StopBits = new System.Windows.Forms.ComboBox();
            this.label9 = new System.Windows.Forms.Label();
            this.cmb_DataBits = new System.Windows.Forms.ComboBox();
            this.label8 = new System.Windows.Forms.Label();
            this.cmb_BaudRate = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.cmb_PortName = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.chk_Enabled = new System.Windows.Forms.CheckBox();
            this.lbl_Type = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lbl_ConfigTip = new System.Windows.Forms.Label();
            this.contextMenu_Device = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmi_Rename = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmi_Test = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenu_Add = new System.Windows.Forms.ContextMenuStrip(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.panel_DeviceButtons.SuspendLayout();
            this.panel_ConfigArea.SuspendLayout();
            this.panel_Params.SuspendLayout();
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
            this.splitContainer1.Panel1.Controls.Add(this.panel_DeviceButtons);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.panel_ConfigArea);
            this.splitContainer1.Size = new System.Drawing.Size(900, 600);
            this.splitContainer1.SplitterDistance = 240;
            this.splitContainer1.TabIndex = 0;
            // 
            // flowPanel_Devices
            // 
            this.flowPanel_Devices.AutoScroll = true;
            this.flowPanel_Devices.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.flowPanel_Devices.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowPanel_Devices.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowPanel_Devices.Location = new System.Drawing.Point(0, 0);
            this.flowPanel_Devices.Name = "flowPanel_Devices";
            this.flowPanel_Devices.Padding = new System.Windows.Forms.Padding(2);
            this.flowPanel_Devices.Size = new System.Drawing.Size(240, 540);
            this.flowPanel_Devices.TabIndex = 0;
            this.flowPanel_Devices.WrapContents = false;
            // 
            // panel_DeviceButtons
            // 
            this.panel_DeviceButtons.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.panel_DeviceButtons.Controls.Add(this.btn_Remove);
            this.panel_DeviceButtons.Controls.Add(this.btn_Add);
            this.panel_DeviceButtons.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel_DeviceButtons.Location = new System.Drawing.Point(0, 540);
            this.panel_DeviceButtons.Name = "panel_DeviceButtons";
            this.panel_DeviceButtons.Size = new System.Drawing.Size(240, 60);
            this.panel_DeviceButtons.TabIndex = 1;
            // 
            // btn_Remove
            // 
            this.btn_Remove.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_Remove.Location = new System.Drawing.Point(130, 8);
            this.btn_Remove.Name = "btn_Remove";
            this.btn_Remove.Shape = AntdUI.TShape.Circle;
            this.btn_Remove.Size = new System.Drawing.Size(45, 45);
            this.btn_Remove.TabIndex = 1;
            this.btn_Remove.Text = "-";
            this.btn_Remove.Type = AntdUI.TTypeMini.Error;
            this.btn_Remove.Click += new System.EventHandler(this.btn_Remove_Click);
            // 
            // btn_Add
            // 
            this.btn_Add.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_Add.Location = new System.Drawing.Point(65, 8);
            this.btn_Add.Name = "btn_Add";
            this.btn_Add.Shape = AntdUI.TShape.Circle;
            this.btn_Add.Size = new System.Drawing.Size(45, 45);
            this.btn_Add.TabIndex = 0;
            this.btn_Add.Text = "+";
            this.btn_Add.Type = AntdUI.TTypeMini.Info;
            this.btn_Add.Click += new System.EventHandler(this.btn_Add_Click);
            // 
            // panel_ConfigArea
            // 
            this.panel_ConfigArea.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(37)))), ((int)(((byte)(38)))));
            this.panel_ConfigArea.Controls.Add(this.panel_Params);
            this.panel_ConfigArea.Controls.Add(this.lbl_ConfigTip);
            this.panel_ConfigArea.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel_ConfigArea.Location = new System.Drawing.Point(0, 0);
            this.panel_ConfigArea.Name = "panel_ConfigArea";
            this.panel_ConfigArea.Size = new System.Drawing.Size(656, 600);
            this.panel_ConfigArea.TabIndex = 0;
            // 
            // panel_Params
            // 
            this.panel_Params.Controls.Add(this.btn_Test);
            this.panel_Params.Controls.Add(this.btn_Save);
            this.panel_Params.Controls.Add(this.cmb_ChannelCount);
            this.panel_Params.Controls.Add(this.label11);
            this.panel_Params.Controls.Add(this.cmb_Parity);
            this.panel_Params.Controls.Add(this.label10);
            this.panel_Params.Controls.Add(this.cmb_StopBits);
            this.panel_Params.Controls.Add(this.label9);
            this.panel_Params.Controls.Add(this.cmb_DataBits);
            this.panel_Params.Controls.Add(this.label8);
            this.panel_Params.Controls.Add(this.cmb_BaudRate);
            this.panel_Params.Controls.Add(this.label7);
            this.panel_Params.Controls.Add(this.cmb_PortName);
            this.panel_Params.Controls.Add(this.label6);
            this.panel_Params.Controls.Add(this.chk_Enabled);
            this.panel_Params.Controls.Add(this.lbl_Type);
            this.panel_Params.Controls.Add(this.label2);
            this.panel_Params.Location = new System.Drawing.Point(20, 20);
            this.panel_Params.Name = "panel_Params";
            this.panel_Params.Size = new System.Drawing.Size(600, 350);
            this.panel_Params.TabIndex = 1;
            this.panel_Params.Visible = false;
            // 
            // btn_Test
            // 
            this.btn_Test.Location = new System.Drawing.Point(450, 47);
            this.btn_Test.Name = "btn_Test";
            this.btn_Test.Size = new System.Drawing.Size(100, 29);
            this.btn_Test.TabIndex = 50;
            this.btn_Test.Text = "测试";
            this.btn_Test.UseVisualStyleBackColor = true;
            this.btn_Test.Click += new System.EventHandler(this.btn_Test_Click);
            // 
            // btn_Save
            // 
            this.btn_Save.Location = new System.Drawing.Point(450, 172);
            this.btn_Save.Name = "btn_Save";
            this.btn_Save.Size = new System.Drawing.Size(100, 37);
            this.btn_Save.TabIndex = 4;
            this.btn_Save.Text = "保存配置";
            this.btn_Save.UseVisualStyleBackColor = true;
            this.btn_Save.Visible = false;
            this.btn_Save.Click += new System.EventHandler(this.btn_Save_Click);
            // 
            // cmb_ChannelCount
            // 
            this.cmb_ChannelCount.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmb_ChannelCount.FormattingEnabled = true;
            this.cmb_ChannelCount.Location = new System.Drawing.Point(120, 247);
            this.cmb_ChannelCount.Name = "cmb_ChannelCount";
            this.cmb_ChannelCount.Size = new System.Drawing.Size(300, 26);
            this.cmb_ChannelCount.TabIndex = 45;
            this.cmb_ChannelCount.SelectedIndexChanged += new System.EventHandler(this.cmb_ChannelCount_SelectedIndexChanged);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.ForeColor = System.Drawing.Color.White;
            this.label11.Location = new System.Drawing.Point(10, 252);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(62, 18);
            this.label11.TabIndex = 44;
            this.label11.Text = "通道数";
            // 
            // cmb_Parity
            // 
            this.cmb_Parity.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmb_Parity.FormattingEnabled = true;
            this.cmb_Parity.Location = new System.Drawing.Point(120, 207);
            this.cmb_Parity.Name = "cmb_Parity";
            this.cmb_Parity.Size = new System.Drawing.Size(300, 26);
            this.cmb_Parity.TabIndex = 43;
            this.cmb_Parity.SelectedIndexChanged += new System.EventHandler(this.cmb_Parity_SelectedIndexChanged);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.ForeColor = System.Drawing.Color.White;
            this.label10.Location = new System.Drawing.Point(10, 212);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(62, 18);
            this.label10.TabIndex = 42;
            this.label10.Text = "校验位";
            // 
            // cmb_StopBits
            // 
            this.cmb_StopBits.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmb_StopBits.FormattingEnabled = true;
            this.cmb_StopBits.Location = new System.Drawing.Point(120, 167);
            this.cmb_StopBits.Name = "cmb_StopBits";
            this.cmb_StopBits.Size = new System.Drawing.Size(300, 26);
            this.cmb_StopBits.TabIndex = 41;
            this.cmb_StopBits.SelectedIndexChanged += new System.EventHandler(this.cmb_StopBits_SelectedIndexChanged);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.ForeColor = System.Drawing.Color.White;
            this.label9.Location = new System.Drawing.Point(10, 172);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(62, 18);
            this.label9.TabIndex = 40;
            this.label9.Text = "停止位";
            // 
            // cmb_DataBits
            // 
            this.cmb_DataBits.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmb_DataBits.FormattingEnabled = true;
            this.cmb_DataBits.Location = new System.Drawing.Point(120, 127);
            this.cmb_DataBits.Name = "cmb_DataBits";
            this.cmb_DataBits.Size = new System.Drawing.Size(300, 26);
            this.cmb_DataBits.TabIndex = 39;
            this.cmb_DataBits.SelectedIndexChanged += new System.EventHandler(this.cmb_DataBits_SelectedIndexChanged);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.ForeColor = System.Drawing.Color.White;
            this.label8.Location = new System.Drawing.Point(10, 132);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(62, 18);
            this.label8.TabIndex = 38;
            this.label8.Text = "数据位";
            // 
            // cmb_BaudRate
            // 
            this.cmb_BaudRate.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmb_BaudRate.FormattingEnabled = true;
            this.cmb_BaudRate.Location = new System.Drawing.Point(120, 87);
            this.cmb_BaudRate.Name = "cmb_BaudRate";
            this.cmb_BaudRate.Size = new System.Drawing.Size(300, 26);
            this.cmb_BaudRate.TabIndex = 37;
            this.cmb_BaudRate.SelectedIndexChanged += new System.EventHandler(this.cmb_BaudRate_SelectedIndexChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.ForeColor = System.Drawing.Color.White;
            this.label7.Location = new System.Drawing.Point(10, 92);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(62, 18);
            this.label7.TabIndex = 36;
            this.label7.Text = "波特率";
            // 
            // cmb_PortName
            // 
            this.cmb_PortName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmb_PortName.FormattingEnabled = true;
            this.cmb_PortName.Location = new System.Drawing.Point(120, 47);
            this.cmb_PortName.Name = "cmb_PortName";
            this.cmb_PortName.Size = new System.Drawing.Size(300, 26);
            this.cmb_PortName.TabIndex = 35;
            this.cmb_PortName.SelectedIndexChanged += new System.EventHandler(this.cmb_PortName_SelectedIndexChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.ForeColor = System.Drawing.Color.White;
            this.label6.Location = new System.Drawing.Point(10, 52);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(62, 18);
            this.label6.TabIndex = 34;
            this.label6.Text = "端口号";
            // 
            // chk_Enabled
            // 
            this.chk_Enabled.AutoSize = true;
            this.chk_Enabled.Checked = true;
            this.chk_Enabled.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chk_Enabled.ForeColor = System.Drawing.Color.White;
            this.chk_Enabled.Location = new System.Drawing.Point(350, 10);
            this.chk_Enabled.Name = "chk_Enabled";
            this.chk_Enabled.Size = new System.Drawing.Size(70, 22);
            this.chk_Enabled.TabIndex = 33;
            this.chk_Enabled.Text = "启用";
            this.chk_Enabled.UseVisualStyleBackColor = true;
            this.chk_Enabled.CheckedChanged += new System.EventHandler(this.chk_Enabled_CheckedChanged);
            // 
            // lbl_Type
            // 
            this.lbl_Type.AutoSize = true;
            this.lbl_Type.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Bold);
            this.lbl_Type.ForeColor = System.Drawing.Color.White;
            this.lbl_Type.Location = new System.Drawing.Point(80, 10);
            this.lbl_Type.Name = "lbl_Type";
            this.lbl_Type.Size = new System.Drawing.Size(48, 25);
            this.lbl_Type.TabIndex = 28;
            this.lbl_Type.Text = "类型";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(10, 14);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(44, 18);
            this.label2.TabIndex = 27;
            this.label2.Text = "类型";
            // 
            // lbl_ConfigTip
            // 
            this.lbl_ConfigTip.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbl_ConfigTip.ForeColor = System.Drawing.Color.Gray;
            this.lbl_ConfigTip.Location = new System.Drawing.Point(0, 0);
            this.lbl_ConfigTip.Name = "lbl_ConfigTip";
            this.lbl_ConfigTip.Size = new System.Drawing.Size(656, 600);
            this.lbl_ConfigTip.TabIndex = 0;
            this.lbl_ConfigTip.Text = "请在左侧选择或添加光源设备";
            this.lbl_ConfigTip.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // contextMenu_Device
            // 
            this.contextMenu_Device.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.contextMenu_Device.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmi_Rename,
            this.toolStripSeparator1,
            this.tsmi_Test});
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
            // tsmi_Test
            // 
            this.tsmi_Test.Name = "tsmi_Test";
            this.tsmi_Test.Size = new System.Drawing.Size(240, 30);
            this.tsmi_Test.Text = "测试";
            this.tsmi_Test.Click += new System.EventHandler(this.tsmi_Test_Click);
            // 
            // contextMenu_Add
            // 
            this.contextMenu_Add.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.contextMenu_Add.Name = "contextMenu_Add";
            this.contextMenu_Add.Size = new System.Drawing.Size(61, 4);
            // 
            // uLightConfig
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(37)))), ((int)(((byte)(38)))));
            this.Controls.Add(this.splitContainer1);
            this.Name = "uLightConfig";
            this.Size = new System.Drawing.Size(900, 600);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.panel_DeviceButtons.ResumeLayout(false);
            this.panel_ConfigArea.ResumeLayout(false);
            this.panel_Params.ResumeLayout(false);
            this.panel_Params.PerformLayout();
            this.contextMenu_Device.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Panel panel_DeviceButtons;
        private AntdUI.Button btn_Remove;
        private AntdUI.Button btn_Add;
        private System.Windows.Forms.FlowLayoutPanel flowPanel_Devices;
        private System.Windows.Forms.Panel panel_ConfigArea;
        private System.Windows.Forms.Label lbl_ConfigTip;
        private System.Windows.Forms.ContextMenuStrip contextMenu_Device;
        private System.Windows.Forms.ToolStripMenuItem tsmi_Rename;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem tsmi_Test;
        private System.Windows.Forms.Panel panel_Params;
        private System.Windows.Forms.Button btn_Test;
        private System.Windows.Forms.Button btn_Save;
        private System.Windows.Forms.ComboBox cmb_ChannelCount;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.ComboBox cmb_Parity;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.ComboBox cmb_StopBits;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.ComboBox cmb_DataBits;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ComboBox cmb_BaudRate;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ComboBox cmb_PortName;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.CheckBox chk_Enabled;
        private System.Windows.Forms.Label lbl_Type;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ContextMenuStrip contextMenu_Add;
    }
}
