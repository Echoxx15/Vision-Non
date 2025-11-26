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
            this.btn_Connet = new System.Windows.Forms.Button();
            this.listBox_Configs = new System.Windows.Forms.ListBox();
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
            this.contextMenuStrip1.SuspendLayout();
            this.grp_Config.SuspendLayout();
            this.SuspendLayout();
            // 
            // btn_AddFgen
            // 
            this.btn_AddFgen.ContextMenuStrip = this.contextMenuStrip1;
            this.btn_AddFgen.Location = new System.Drawing.Point(241, 57);
            this.btn_AddFgen.Margin = new System.Windows.Forms.Padding(4);
            this.btn_AddFgen.Name = "btn_AddFgen";
            this.btn_AddFgen.Size = new System.Drawing.Size(121, 34);
            this.btn_AddFgen.TabIndex = 1;
            this.btn_AddFgen.Text = "���ӿ�����";
            this.btn_AddFgen.UseVisualStyleBackColor = true;
            this.btn_AddFgen.Click += new System.EventHandler(this.btn_AddFgen_Click);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.contextMenuStrip1.Items.Clear();
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(171, 34);
            // 
            // tsm_Fugen
            // 
            this.tsm_Fugen.Name = "tsm_Fugen";
            this.tsm_Fugen.Size = new System.Drawing.Size(170, 30);
            this.tsm_Fugen.Text = "�ڸ�������";
            
            // 
            // btn_Delete
            // 
            this.btn_Delete.Location = new System.Drawing.Point(241, 192);
            this.btn_Delete.Margin = new System.Windows.Forms.Padding(4);
            this.btn_Delete.Name = "btn_Delete";
            this.btn_Delete.Size = new System.Drawing.Size(108, 34);
            this.btn_Delete.TabIndex = 3;
            this.btn_Delete.Text = "ɾ��";
            this.btn_Delete.UseVisualStyleBackColor = true;
            // 
            // btn_Save
            // 
            this.btn_Save.Location = new System.Drawing.Point(241, 234);
            this.btn_Save.Margin = new System.Windows.Forms.Padding(4);
            this.btn_Save.Name = "btn_Save";
            this.btn_Save.Size = new System.Drawing.Size(108, 37);
            this.btn_Save.TabIndex = 4;
            this.btn_Save.Text = "��������";
            this.btn_Save.UseVisualStyleBackColor = true;
            this.btn_Save.Click += new System.EventHandler(this.btn_Save_Click);
            // 
            // grp_Config
            // 
            this.grp_Config.Controls.Add(this.btn_Connet);
            this.grp_Config.Controls.Add(this.listBox_Configs);
            this.grp_Config.Controls.Add(this.cmb_ChannelCount);
            this.grp_Config.Controls.Add(this.label11);
            this.grp_Config.Controls.Add(this.cmb_Parity);
            this.grp_Config.Controls.Add(this.btn_Save);
            this.grp_Config.Controls.Add(this.label10);
            this.grp_Config.Controls.Add(this.btn_Delete);
            this.grp_Config.Controls.Add(this.btn_AddFgen);
            this.grp_Config.Controls.Add(this.cmb_StopBits);
            this.grp_Config.Controls.Add(this.label9);
            this.grp_Config.Controls.Add(this.cmb_DataBits);
            this.grp_Config.Controls.Add(this.label8);
            this.grp_Config.Controls.Add(this.cmb_BaudRate);
            this.grp_Config.Controls.Add(this.label7);
            this.grp_Config.Controls.Add(this.cmb_PortName);
            this.grp_Config.Controls.Add(this.label6);
            this.grp_Config.Controls.Add(this.chk_Enabled);
            this.grp_Config.Controls.Add(this.lbl_Type);
            this.grp_Config.Controls.Add(this.label2);
            this.grp_Config.Location = new System.Drawing.Point(13, 13);
            this.grp_Config.Margin = new System.Windows.Forms.Padding(4);
            this.grp_Config.Name = "grp_Config";
            this.grp_Config.Padding = new System.Windows.Forms.Padding(4);
            this.grp_Config.Size = new System.Drawing.Size(1020, 318);
            this.grp_Config.TabIndex = 8;
            this.grp_Config.TabStop = false;
            this.grp_Config.Text = "��������";
            // 
            // btn_Connet
            // 
            this.btn_Connet.Location = new System.Drawing.Point(241, 106);
            this.btn_Connet.Name = "btn_Connet";
            this.btn_Connet.Size = new System.Drawing.Size(111, 29);
            this.btn_Connet.TabIndex = 47;
            this.btn_Connet.Text = "����";
            this.btn_Connet.UseVisualStyleBackColor = true;
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
            // cmb_ChannelCount
            // 
            this.cmb_ChannelCount.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmb_ChannelCount.FormattingEnabled = true;
            this.cmb_ChannelCount.Location = new System.Drawing.Point(587, 189);
            this.cmb_ChannelCount.Name = "cmb_ChannelCount";
            this.cmb_ChannelCount.Size = new System.Drawing.Size(207, 26);
            this.cmb_ChannelCount.TabIndex = 45;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(480, 192);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(98, 18);
            this.label11.TabIndex = 44;
            this.label11.Text = "ͨ��������";
            // 
            // cmb_Parity
            // 
            this.cmb_Parity.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmb_Parity.FormattingEnabled = true;
            this.cmb_Parity.Location = new System.Drawing.Point(587, 267);
            this.cmb_Parity.Name = "cmb_Parity";
            this.cmb_Parity.Size = new System.Drawing.Size(207, 26);
            this.cmb_Parity.TabIndex = 43;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(480, 272);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(80, 18);
            this.label10.TabIndex = 42;
            this.label10.Text = "У��λ��";
            // 
            // cmb_StopBits
            // 
            this.cmb_StopBits.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmb_StopBits.FormattingEnabled = true;
            this.cmb_StopBits.Location = new System.Drawing.Point(587, 149);
            this.cmb_StopBits.Name = "cmb_StopBits";
            this.cmb_StopBits.Size = new System.Drawing.Size(207, 26);
            this.cmb_StopBits.TabIndex = 41;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(480, 152);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(80, 18);
            this.label9.TabIndex = 40;
            this.label9.Text = "ֹͣλ��";
            // 
            // cmb_DataBits
            // 
            this.cmb_DataBits.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmb_DataBits.FormattingEnabled = true;
            this.cmb_DataBits.Location = new System.Drawing.Point(587, 227);
            this.cmb_DataBits.Name = "cmb_DataBits";
            this.cmb_DataBits.Size = new System.Drawing.Size(207, 26);
            this.cmb_DataBits.TabIndex = 39;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(480, 232);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(80, 18);
            this.label8.TabIndex = 38;
            this.label8.Text = "����λ��";
            // 
            // cmb_BaudRate
            // 
            this.cmb_BaudRate.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmb_BaudRate.FormattingEnabled = true;
            this.cmb_BaudRate.Location = new System.Drawing.Point(587, 109);
            this.cmb_BaudRate.Name = "cmb_BaudRate";
            this.cmb_BaudRate.Size = new System.Drawing.Size(207, 26);
            this.cmb_BaudRate.TabIndex = 37;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(480, 112);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(80, 18);
            this.label7.TabIndex = 36;
            this.label7.Text = "�����ʣ�";
            // 
            // cmb_PortName
            // 
            this.cmb_PortName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmb_PortName.FormattingEnabled = true;
            this.cmb_PortName.Location = new System.Drawing.Point(587, 69);
            this.cmb_PortName.Name = "cmb_PortName";
            this.cmb_PortName.Size = new System.Drawing.Size(207, 26);
            this.cmb_PortName.TabIndex = 35;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(480, 72);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(80, 18);
            this.label6.TabIndex = 34;
            this.label6.Text = "���ںţ�";
            // 
            // chk_Enabled
            // 
            this.chk_Enabled.AutoSize = true;
            this.chk_Enabled.Checked = true;
            this.chk_Enabled.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chk_Enabled.Location = new System.Drawing.Point(724, 25);
            this.chk_Enabled.Name = "chk_Enabled";
            this.chk_Enabled.Size = new System.Drawing.Size(70, 22);
            this.chk_Enabled.TabIndex = 33;
            this.chk_Enabled.Text = "����";
            this.chk_Enabled.UseVisualStyleBackColor = true;
            // 
            // lbl_Type
            // 
            this.lbl_Type.AutoSize = true;
            this.lbl_Type.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lbl_Type.Location = new System.Drawing.Point(582, 25);
            this.lbl_Type.Name = "lbl_Type";
            this.lbl_Type.Size = new System.Drawing.Size(102, 25);
            this.lbl_Type.TabIndex = 28;
            this.lbl_Type.Text = "�ڸ�������";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(496, 29);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(62, 18);
            this.label2.TabIndex = 27;
            this.label2.Text = "���ͣ�";
            // 
            // Frm_LightConfig
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(827, 329);
            this.Controls.Add(this.grp_Config);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Frm_LightConfig";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "��Դ����";
            this.contextMenuStrip1.ResumeLayout(false);
            this.grp_Config.ResumeLayout(false);
            this.grp_Config.PerformLayout();
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
