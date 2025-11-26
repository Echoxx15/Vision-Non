using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace LightControlNet.UI
{
    partial class Frm_LightConfig
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.Button btn_AddFgen;
        private System.Windows.Forms.Button btn_Delete;
        private System.Windows.Forms.Button btn_Save;
        private System.Windows.Forms.GroupBox grp_Config;
        private System.Windows.Forms.Panel panel_TestHost;
        private System.Windows.Forms.Panel panel_Params;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }

            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.btn_AddFgen = new System.Windows.Forms.Button();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsm_Fugen = new System.Windows.Forms.ToolStripMenuItem();
            this.btn_Delete = new System.Windows.Forms.Button();
            this.btn_Save = new System.Windows.Forms.Button();
            this.grp_Config = new System.Windows.Forms.GroupBox();
            this.panel_TestHost = new System.Windows.Forms.Panel();
            this.panel_Params = new System.Windows.Forms.Panel();
            this.btn_Connet = new System.Windows.Forms.Button();
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
            this.cmb_ChannelCount = new System.Windows.Forms.ComboBox();
            this.label11 = new System.Windows.Forms.Label();
            this.listBox_Configs = new System.Windows.Forms.ListBox();
            this.cmb_Parity = new System.Windows.Forms.ComboBox();
            this.label10 = new System.Windows.Forms.Label();
            this.grp_Config.SuspendLayout();
            this.panel_Params.SuspendLayout();
            this.SuspendLayout();
            // 
            // btn_AddFgen
            // 
            this.btn_AddFgen.ContextMenuStrip = this.contextMenuStrip1;
            this.btn_AddFgen.Location = new System.Drawing.Point(440, 92);
            this.btn_AddFgen.Margin = new System.Windows.Forms.Padding(4);
            this.btn_AddFgen.Name = "btn_AddFgen";
            this.btn_AddFgen.Size = new System.Drawing.Size(100, 34);
            this.btn_AddFgen.TabIndex = 1;
            this.btn_AddFgen.Text = "添加";
            this.btn_AddFgen.UseVisualStyleBackColor = true;
            this.btn_AddFgen.Click += new System.EventHandler(this.btn_AddFgen_Click);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(61, 4);
            // 
            // tsm_Fugen
            // 
            this.tsm_Fugen.Name = "tsm_Fugen";
            this.tsm_Fugen.Size = new System.Drawing.Size(170, 30);
            this.tsm_Fugen.Text = "孚根控制器";
            // 
            // btn_Delete
            // 
            this.btn_Delete.Location = new System.Drawing.Point(440, 134);
            this.btn_Delete.Margin = new System.Windows.Forms.Padding(4);
            this.btn_Delete.Name = "btn_Delete";
            this.btn_Delete.Size = new System.Drawing.Size(100, 34);
            this.btn_Delete.TabIndex = 3;
            this.btn_Delete.Text = "删除";
            this.btn_Delete.UseVisualStyleBackColor = true;
            // 
            // btn_Save
            // 
            this.btn_Save.Location = new System.Drawing.Point(440, 176);
            this.btn_Save.Margin = new System.Windows.Forms.Padding(4);
            this.btn_Save.Name = "btn_Save";
            this.btn_Save.Size = new System.Drawing.Size(108, 37);
            this.btn_Save.TabIndex = 4;
            this.btn_Save.Text = "保存配置";
            this.btn_Save.UseVisualStyleBackColor = true;
            this.btn_Save.Click += new System.EventHandler(this.btn_Save_Click);
            // 
            // grp_Config
            // 
            this.grp_Config.Controls.Add(this.panel_TestHost);
            this.grp_Config.Controls.Add(this.panel_Params);
            this.grp_Config.Controls.Add(this.listBox_Configs);
            this.grp_Config.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grp_Config.Location = new System.Drawing.Point(0, 0);
            this.grp_Config.Margin = new System.Windows.Forms.Padding(4);
            this.grp_Config.Name = "grp_Config";
            this.grp_Config.Padding = new System.Windows.Forms.Padding(4);
            this.grp_Config.Size = new System.Drawing.Size(1225, 380);
            this.grp_Config.TabIndex = 8;
            this.grp_Config.TabStop = false;
            this.grp_Config.Text = "光源配置";
            // 
            // panel_TestHost
            // 
            this.panel_TestHost.Anchor =
                ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top |
                                                       System.Windows.Forms.AnchorStyles.Bottom)
                                                      | System.Windows.Forms.AnchorStyles.Right)));
            this.panel_TestHost.Location = new System.Drawing.Point(792, 20);
            this.panel_TestHost.Name = "panel_TestHost";
            this.panel_TestHost.Size = new System.Drawing.Size(423, 336);
            this.panel_TestHost.TabIndex = 48;
            // 
            // panel_Params
            // 
            this.panel_Params.Anchor =
                ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top |
                                                        System.Windows.Forms.AnchorStyles.Bottom)
                                                       | System.Windows.Forms.AnchorStyles.Left)
                                                      | System.Windows.Forms.AnchorStyles.Right)));
            this.panel_Params.Controls.Add(this.btn_Connet);
            this.panel_Params.Controls.Add(this.btn_Save);
            this.panel_Params.Controls.Add(this.btn_Delete);
            this.panel_Params.Controls.Add(this.btn_AddFgen);
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
            this.panel_Params.Controls.Add(this.cmb_ChannelCount);
            this.panel_Params.Controls.Add(this.label11);
            this.panel_Params.Location = new System.Drawing.Point(220, 20);
            this.panel_Params.Name = "panel_Params";
            this.panel_Params.Size = new System.Drawing.Size(566, 336);
            this.panel_Params.TabIndex = 49;
            // 
            // btn_Connet
            // 
            this.btn_Connet.Location = new System.Drawing.Point(440, 47);
            this.btn_Connet.Name = "btn_Connet";
            this.btn_Connet.Size = new System.Drawing.Size(100, 29);
            this.btn_Connet.TabIndex = 47;
            this.btn_Connet.Text = "打开";
            this.btn_Connet.UseVisualStyleBackColor = true;
            // 
            // cmb_StopBits
            // 
            this.cmb_StopBits.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmb_StopBits.FormattingEnabled = true;
            this.cmb_StopBits.Location = new System.Drawing.Point(120, 127);
            this.cmb_StopBits.Name = "cmb_StopBits";
            this.cmb_StopBits.Size = new System.Drawing.Size(300, 26);
            this.cmb_StopBits.TabIndex = 41;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(10, 132);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(62, 18);
            this.label9.TabIndex = 40;
            this.label9.Text = "停止位";
            // 
            // cmb_DataBits
            // 
            this.cmb_DataBits.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmb_DataBits.FormattingEnabled = true;
            this.cmb_DataBits.Location = new System.Drawing.Point(120, 167);
            this.cmb_DataBits.Name = "cmb_DataBits";
            this.cmb_DataBits.Size = new System.Drawing.Size(300, 26);
            this.cmb_DataBits.TabIndex = 39;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(10, 172);
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
            // 
            // label7
            // 
            this.label7.AutoSize = true;
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
            // 
            // label6
            // 
            this.label6.AutoSize = true;
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
            this.chk_Enabled.Location = new System.Drawing.Point(420, 10);
            this.chk_Enabled.Name = "chk_Enabled";
            this.chk_Enabled.Size = new System.Drawing.Size(70, 22);
            this.chk_Enabled.TabIndex = 33;
            this.chk_Enabled.Text = "启用";
            this.chk_Enabled.UseVisualStyleBackColor = true;
            // 
            // lbl_Type
            // 
            this.lbl_Type.AutoSize = true;
            this.lbl_Type.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Bold,
                System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lbl_Type.Location = new System.Drawing.Point(80, 10);
            this.lbl_Type.Name = "lbl_Type";
            this.lbl_Type.Size = new System.Drawing.Size(48, 25);
            this.lbl_Type.TabIndex = 28;
            this.lbl_Type.Text = "类型";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(10, 14);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(44, 18);
            this.label2.TabIndex = 27;
            this.label2.Text = "类型";
            // 
            // cmb_ChannelCount
            // 
            this.cmb_ChannelCount.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmb_ChannelCount.FormattingEnabled = true;
            this.cmb_ChannelCount.Location = new System.Drawing.Point(120, 247);
            this.cmb_ChannelCount.Name = "cmb_ChannelCount";
            this.cmb_ChannelCount.Size = new System.Drawing.Size(300, 26);
            this.cmb_ChannelCount.TabIndex = 45;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(10, 252);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(62, 18);
            this.label11.TabIndex = 44;
            this.label11.Text = "通道数";
            // 
            // listBox_Configs
            // 
            this.listBox_Configs.FormattingEnabled = true;
            this.listBox_Configs.ItemHeight = 18;
            this.listBox_Configs.Location = new System.Drawing.Point(17, 20);
            this.listBox_Configs.Margin = new System.Windows.Forms.Padding(4);
            this.listBox_Configs.Name = "listBox_Configs";
            this.listBox_Configs.Size = new System.Drawing.Size(182, 274);
            this.listBox_Configs.TabIndex = 46;
            // 
            // cmb_Parity
            // 
            this.cmb_Parity.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmb_Parity.FormattingEnabled = true;
            this.cmb_Parity.Location = new System.Drawing.Point(120, 207);
            this.cmb_Parity.Name = "cmb_Parity";
            this.cmb_Parity.Size = new System.Drawing.Size(300, 26);
            this.cmb_Parity.TabIndex = 43;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(10, 212);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(80, 18);
            this.label10.TabIndex = 42;
            this.label10.Text = "校验位";
            // 
            // Frm_LightConfig
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(1225, 380);
            this.Controls.Add(this.grp_Config);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Frm_LightConfig";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "光源配置";
            this.grp_Config.ResumeLayout(false);
            this.panel_Params.ResumeLayout(false);
            this.panel_Params.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private ContextMenuStrip contextMenuStrip1;
        private ToolStripMenuItem tsm_Fugen;
        private ComboBox cmb_ChannelCount;
        private Label label11;
        private ComboBox cmb_Parity;
        private Label label10;
        private ComboBox cmb_StopBits;
        private Label label9;
        private ComboBox cmb_DataBits;
        private Label label8;
        private ComboBox cmb_BaudRate;
        private Label label7;
        private ComboBox cmb_PortName;
        private Label label6;
        private CheckBox chk_Enabled;
        private Label lbl_Type;
        private Label label2;
        private ListBox listBox_Configs;
        private Button btn_Connet;
    }
}
