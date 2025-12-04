namespace Vision.Frm.MainForm
{
    partial class Frm_Main
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Frm_Main));
            this.menuMain = new System.Windows.Forms.MenuStrip();
            this.btn_User = new System.Windows.Forms.ToolStripMenuItem();
            this.btn_Login = new System.Windows.Forms.ToolStripMenuItem();
            this.btn_Register = new System.Windows.Forms.ToolStripMenuItem();
            this.btn_Permission = new System.Windows.Forms.ToolStripMenuItem();
            this.btn_System = new System.Windows.Forms.ToolStripMenuItem();
            this.barButtonItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.btn_File = new System.Windows.Forms.ToolStripMenuItem();
            this.tsm_SystemState = new System.Windows.Forms.ToolStripMenuItem();
            this.barSubItem3 = new System.Windows.Forms.ToolStripMenuItem();
            this.barSubItem4 = new System.Windows.Forms.ToolStripMenuItem();
            this.btn_Chinese = new System.Windows.Forms.ToolStripMenuItem();
            this.btn_English = new System.Windows.Forms.ToolStripMenuItem();
            this.toolMain = new System.Windows.Forms.ToolStrip();
            this.btn_SolutionList = new System.Windows.Forms.ToolStripButton();
            this.btn_SaveSolution = new System.Windows.Forms.ToolStripButton();
            this.btn_CreateVar = new System.Windows.Forms.ToolStripButton();
            this.btn_Station = new System.Windows.Forms.ToolStripButton();
            this.btn_HardwareCamera = new System.Windows.Forms.ToolStripButton();
            this.tsm_Comm = new System.Windows.Forms.ToolStripButton();
            this.tsm_LightControl = new System.Windows.Forms.ToolStripButton();
            this.tsm_DLModel = new System.Windows.Forms.ToolStripButton();
            this.btn_UI = new System.Windows.Forms.ToolStripButton();
            this.statusMain = new System.Windows.Forms.StatusStrip();
            this.tsl_SystemState = new System.Windows.Forms.ToolStripStatusLabel();
            this.barStaticItem1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.btn_Logout = new System.Windows.Forms.ToolStripStatusLabel();
            this.txt_JobName = new System.Windows.Forms.ToolStripStatusLabel();
            this.txt_RunTime = new System.Windows.Forms.ToolStripStatusLabel();
            this.txt_Memory = new System.Windows.Forms.ToolStripStatusLabel();
            this.txt_CPU = new System.Windows.Forms.ToolStripStatusLabel();
            this.txt_Disk = new System.Windows.Forms.ToolStripStatusLabel();
            this.tlp_Display = new System.Windows.Forms.TableLayoutPanel();
            this.grb_State = new System.Windows.Forms.GroupBox();
            this.grb_Log = new System.Windows.Forms.GroupBox();
            this.split_Main = new System.Windows.Forms.SplitContainer();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.menuMain.SuspendLayout();
            this.toolMain.SuspendLayout();
            this.statusMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.split_Main)).BeginInit();
            this.split_Main.Panel1.SuspendLayout();
            this.split_Main.Panel2.SuspendLayout();
            this.split_Main.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuMain
            // 
            this.menuMain.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.menuMain.GripMargin = new System.Windows.Forms.Padding(2, 2, 0, 2);
            this.menuMain.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.menuMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btn_User,
            this.btn_System,
            this.barSubItem3,
            this.barSubItem4});
            this.menuMain.Location = new System.Drawing.Point(0, 0);
            this.menuMain.Name = "menuMain";
            this.menuMain.Padding = new System.Windows.Forms.Padding(5, 2, 0, 2);
            this.menuMain.Size = new System.Drawing.Size(1212, 36);
            this.menuMain.TabIndex = 0;
            this.menuMain.Text = "menuMain";
            // 
            // btn_User
            // 
            this.btn_User.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btn_Login,
            this.btn_Register,
            this.btn_Permission});
            this.btn_User.Image = global::Vision.Properties.Resources.用户;
            this.btn_User.Name = "btn_User";
            this.btn_User.Size = new System.Drawing.Size(111, 28);
            this.btn_User.Text = "用户(U)";
            // 
            // btn_Login
            // 
            this.btn_Login.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.btn_Login.Image = global::Vision.Properties.Resources.用户登录;
            this.btn_Login.Name = "btn_Login";
            this.btn_Login.Size = new System.Drawing.Size(206, 34);
            this.btn_Login.Text = "用户登录(L)";
            this.btn_Login.Click += new System.EventHandler(this.btn_Login_Click);
            // 
            // btn_Register
            // 
            this.btn_Register.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.btn_Register.Image = global::Vision.Properties.Resources.用户注册;
            this.btn_Register.Name = "btn_Register";
            this.btn_Register.Size = new System.Drawing.Size(206, 34);
            this.btn_Register.Text = "用户注册(R)";
            this.btn_Register.Click += new System.EventHandler(this.btn_Register_Click);
            // 
            // btn_Permission
            // 
            this.btn_Permission.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.btn_Permission.Image = global::Vision.Properties.Resources.用户权限;
            this.btn_Permission.Name = "btn_Permission";
            this.btn_Permission.Size = new System.Drawing.Size(206, 34);
            this.btn_Permission.Text = "用户权限(P)";
            this.btn_Permission.Click += new System.EventHandler(this.btn_Permission_Click);
            // 
            // btn_System
            // 
            this.btn_System.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.barButtonItem1,
            this.btn_File,
            this.tsm_SystemState});
            this.btn_System.Image = global::Vision.Properties.Resources.系统配置;
            this.btn_System.Name = "btn_System";
            this.btn_System.Size = new System.Drawing.Size(108, 28);
            this.btn_System.Text = "系统(S)";
            // 
            // barButtonItem1
            // 
            this.barButtonItem1.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.barButtonItem1.Name = "barButtonItem1";
            this.barButtonItem1.Size = new System.Drawing.Size(182, 34);
            this.barButtonItem1.Text = "系统参数";
            // 
            // btn_File
            // 
            this.btn_File.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.btn_File.Image = global::Vision.Properties.Resources.文件;
            this.btn_File.Name = "btn_File";
            this.btn_File.Size = new System.Drawing.Size(182, 34);
            this.btn_File.Text = "文件参数";
            this.btn_File.Click += new System.EventHandler(this.btn_File_Click);
            // 
            // tsm_SystemState
            // 
            this.tsm_SystemState.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.tsm_SystemState.Name = "tsm_SystemState";
            this.tsm_SystemState.Size = new System.Drawing.Size(182, 34);
            this.tsm_SystemState.Text = "系统在线";
            this.tsm_SystemState.Click += new System.EventHandler(this.btn_SystemOnline_Click);
            // 
            // barSubItem3
            // 
            this.barSubItem3.Image = global::Vision.Properties.Resources.帮助;
            this.barSubItem3.Name = "barSubItem3";
            this.barSubItem3.Size = new System.Drawing.Size(139, 28);
            this.barSubItem3.Text = "帮助(Help)";
            // 
            // barSubItem4
            // 
            this.barSubItem4.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btn_Chinese,
            this.btn_English});
            this.barSubItem4.Image = global::Vision.Properties.Resources.语言;
            this.barSubItem4.Name = "barSubItem4";
            this.barSubItem4.Size = new System.Drawing.Size(107, 28);
            this.barSubItem4.Text = "语言(L)";
            // 
            // btn_Chinese
            // 
            this.btn_Chinese.Name = "btn_Chinese";
            this.btn_Chinese.Size = new System.Drawing.Size(146, 34);
            this.btn_Chinese.Text = "中文";
            this.btn_Chinese.Click += new System.EventHandler(this.btn_Chinese_Click);
            // 
            // btn_English
            // 
            this.btn_English.Name = "btn_English";
            this.btn_English.Size = new System.Drawing.Size(146, 34);
            this.btn_English.Text = "英文";
            this.btn_English.Click += new System.EventHandler(this.btn_English_Click);
            // 
            // toolMain
            // 
            this.toolMain.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.toolMain.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.toolMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btn_SolutionList,
            this.btn_SaveSolution,
            this.btn_CreateVar,
            this.btn_Station,
            this.btn_HardwareCamera,
            this.tsm_Comm,
            this.tsm_LightControl,
            this.tsm_DLModel,
            this.btn_UI});
            this.toolMain.Location = new System.Drawing.Point(0, 36);
            this.toolMain.Name = "toolMain";
            this.toolMain.Size = new System.Drawing.Size(1212, 57);
            this.toolMain.TabIndex = 1;
            this.toolMain.Text = "toolMain";
            // 
            // btn_SolutionList
            // 
            this.btn_SolutionList.Image = global::Vision.Properties.Resources.方案列表;
            this.btn_SolutionList.Name = "btn_SolutionList";
            this.btn_SolutionList.Size = new System.Drawing.Size(86, 52);
            this.btn_SolutionList.Text = "方案列表";
            this.btn_SolutionList.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btn_SolutionList.Click += new System.EventHandler(this.btn_SolutionList_ItemClick);
            // 
            // btn_SaveSolution
            // 
            this.btn_SaveSolution.Image = global::Vision.Properties.Resources.保存方案;
            this.btn_SaveSolution.Name = "btn_SaveSolution";
            this.btn_SaveSolution.Size = new System.Drawing.Size(86, 52);
            this.btn_SaveSolution.Text = "保存方案";
            this.btn_SaveSolution.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btn_SaveSolution.Click += new System.EventHandler(this.btn_SaveSolution_ItemClick);
            // 
            // btn_CreateVar
            // 
            this.btn_CreateVar.Image = global::Vision.Properties.Resources.全局变量;
            this.btn_CreateVar.Name = "btn_CreateVar";
            this.btn_CreateVar.Size = new System.Drawing.Size(86, 52);
            this.btn_CreateVar.Text = "全局变量";
            this.btn_CreateVar.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btn_CreateVar.Click += new System.EventHandler(this.btn_CreateVar_ItemClick);
            // 
            // btn_Station
            // 
            this.btn_Station.Image = global::Vision.Properties.Resources.工位配置;
            this.btn_Station.Name = "btn_Station";
            this.btn_Station.Size = new System.Drawing.Size(86, 52);
            this.btn_Station.Text = "工位配置";
            this.btn_Station.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btn_Station.Click += new System.EventHandler(this.btn_Station_ItemClick);
            // 
            // btn_HardwareCamera
            // 
            this.btn_HardwareCamera.Image = global::Vision.Properties.Resources.相机配置;
            this.btn_HardwareCamera.Name = "btn_HardwareCamera";
            this.btn_HardwareCamera.Size = new System.Drawing.Size(86, 52);
            this.btn_HardwareCamera.Text = "相机硬件";
            this.btn_HardwareCamera.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btn_HardwareCamera.Click += new System.EventHandler(this.btn_HardwareCamera_ItemClick);
            // 
            // tsm_Comm
            // 
            this.tsm_Comm.Image = global::Vision.Properties.Resources.通讯配置;
            this.tsm_Comm.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsm_Comm.Name = "tsm_Comm";
            this.tsm_Comm.Size = new System.Drawing.Size(86, 52);
            this.tsm_Comm.Text = "通讯硬件";
            this.tsm_Comm.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.tsm_Comm.Click += new System.EventHandler(this.tsm_Comm_Click);
            // 
            // tsm_LightControl
            // 
            this.tsm_LightControl.Image = global::Vision.Properties.Resources.光源;
            this.tsm_LightControl.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsm_LightControl.Name = "tsm_LightControl";
            this.tsm_LightControl.Size = new System.Drawing.Size(86, 52);
            this.tsm_LightControl.Text = "光源配置";
            this.tsm_LightControl.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.tsm_LightControl.Click += new System.EventHandler(this.tsm_LightControl_Click);
            // 
            // tsm_DLModel
            // 
            this.tsm_DLModel.Image = ((System.Drawing.Image)(resources.GetObject("tsm_DLModel.Image")));
            this.tsm_DLModel.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsm_DLModel.Name = "tsm_DLModel";
            this.tsm_DLModel.Size = new System.Drawing.Size(86, 52);
            this.tsm_DLModel.Text = "深度学习";
            this.tsm_DLModel.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.tsm_DLModel.Click += new System.EventHandler(this.tsm_DLModel_Click);
            // 
            // btn_UI
            // 
            this.btn_UI.Image = global::Vision.Properties.Resources.显示布局;
            this.btn_UI.Name = "btn_UI";
            this.btn_UI.Size = new System.Drawing.Size(86, 52);
            this.btn_UI.Text = "显示设置";
            this.btn_UI.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btn_UI.Click += new System.EventHandler(this.btn_UI_ItemClick);
            // 
            // statusMain
            // 
            this.statusMain.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.statusMain.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.statusMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsl_SystemState,
            this.barStaticItem1,
            this.btn_Logout,
            this.txt_JobName,
            this.txt_RunTime,
            this.txt_Memory,
            this.txt_CPU,
            this.txt_Disk});
            this.statusMain.Location = new System.Drawing.Point(0, 609);
            this.statusMain.Name = "statusMain";
            this.statusMain.Padding = new System.Windows.Forms.Padding(1, 0, 13, 0);
            this.statusMain.Size = new System.Drawing.Size(1212, 35);
            this.statusMain.TabIndex = 4;
            this.statusMain.Text = "statusMain";
            // 
            // tsl_SystemState
            // 
            this.tsl_SystemState.ForeColor = System.Drawing.Color.Red;
            this.tsl_SystemState.Name = "tsl_SystemState";
            this.tsl_SystemState.Size = new System.Drawing.Size(86, 28);
            this.tsl_SystemState.Text = "系统:离线";
            // 
            // barStaticItem1
            // 
            this.barStaticItem1.Name = "barStaticItem1";
            this.barStaticItem1.Size = new System.Drawing.Size(68, 28);
            this.barStaticItem1.Text = "用户:无";
            // 
            // btn_Logout
            // 
            this.btn_Logout.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left;
            this.btn_Logout.IsLink = true;
            this.btn_Logout.Name = "btn_Logout";
            this.btn_Logout.Size = new System.Drawing.Size(50, 28);
            this.btn_Logout.Text = "注销";
            this.btn_Logout.Click += new System.EventHandler(this.btn_Logout_Click);
            // 
            // txt_JobName
            // 
            this.txt_JobName.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left;
            this.txt_JobName.Name = "txt_JobName";
            this.txt_JobName.Size = new System.Drawing.Size(90, 28);
            this.txt_JobName.Text = "当前方案:";
            // 
            // txt_RunTime
            // 
            this.txt_RunTime.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left;
            this.txt_RunTime.Name = "txt_RunTime";
            this.txt_RunTime.Size = new System.Drawing.Size(90, 28);
            this.txt_RunTime.Text = "运行时间:";
            // 
            // txt_Memory
            // 
            this.txt_Memory.Name = "txt_Memory";
            this.txt_Memory.Size = new System.Drawing.Size(82, 28);
            this.txt_Memory.Text = "内存占用";
            // 
            // txt_CPU
            // 
            this.txt_CPU.Name = "txt_CPU";
            this.txt_CPU.Size = new System.Drawing.Size(82, 28);
            this.txt_CPU.Text = "CPU占用";
            // 
            // txt_Disk
            // 
            this.txt_Disk.Name = "txt_Disk";
            this.txt_Disk.Size = new System.Drawing.Size(102, 28);
            this.txt_Disk.Text = "磁盘剩余:%";
            // 
            // tlp_Display
            // 
            this.tlp_Display.ColumnCount = 1;
            this.tlp_Display.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlp_Display.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlp_Display.Location = new System.Drawing.Point(0, 0);
            this.tlp_Display.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tlp_Display.Name = "tlp_Display";
            this.tlp_Display.RowCount = 1;
            this.tlp_Display.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 85.06224F));
            this.tlp_Display.Size = new System.Drawing.Size(1212, 335);
            this.tlp_Display.TabIndex = 6;
            // 
            // grb_State
            // 
            this.grb_State.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.grb_State.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grb_State.Location = new System.Drawing.Point(1033, 2);
            this.grb_State.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.grb_State.Name = "grb_State";
            this.grb_State.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.grb_State.Size = new System.Drawing.Size(176, 174);
            this.grb_State.TabIndex = 2;
            this.grb_State.TabStop = false;
            this.grb_State.Text = "硬件状态";
            // 
            // grb_Log
            // 
            this.grb_Log.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.grb_Log.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grb_Log.Location = new System.Drawing.Point(3, 2);
            this.grb_Log.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.grb_Log.Name = "grb_Log";
            this.grb_Log.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.grb_Log.Size = new System.Drawing.Size(1024, 174);
            this.grb_Log.TabIndex = 1;
            this.grb_Log.TabStop = false;
            this.grb_Log.Text = "日志";
            // 
            // split_Main
            // 
            this.split_Main.Dock = System.Windows.Forms.DockStyle.Fill;
            this.split_Main.Location = new System.Drawing.Point(0, 93);
            this.split_Main.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.split_Main.Name = "split_Main";
            this.split_Main.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // split_Main.Panel1
            // 
            this.split_Main.Panel1.Controls.Add(this.tlp_Display);
            // 
            // split_Main.Panel2
            // 
            this.split_Main.Panel2.Controls.Add(this.tableLayoutPanel2);
            this.split_Main.Size = new System.Drawing.Size(1212, 516);
            this.split_Main.SplitterDistance = 335;
            this.split_Main.SplitterWidth = 3;
            this.split_Main.TabIndex = 11;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 85F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 15F));
            this.tableLayoutPanel2.Controls.Add(this.grb_State, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.grb_Log, 0, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(1212, 178);
            this.tableLayoutPanel2.TabIndex = 0;
            // 
            // Frm_Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ClientSize = new System.Drawing.Size(1212, 644);
            this.Controls.Add(this.split_Main);
            this.Controls.Add(this.toolMain);
            this.Controls.Add(this.menuMain);
            this.Controls.Add(this.statusMain);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuMain;
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "Frm_Main";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Vision";
            this.WindowState = System.Windows.Forms.FormWindowState.Minimized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Frm_Main_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Frm_Main_FormClosed);
            this.Load += new System.EventHandler(this.Frm_Main_Load);
            this.menuMain.ResumeLayout(false);
            this.menuMain.PerformLayout();
            this.toolMain.ResumeLayout(false);
            this.toolMain.PerformLayout();
            this.statusMain.ResumeLayout(false);
            this.statusMain.PerformLayout();
            this.split_Main.Panel1.ResumeLayout(false);
            this.split_Main.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.split_Main)).EndInit();
            this.split_Main.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuMain;
        private System.Windows.Forms.ToolStrip toolMain;
        private System.Windows.Forms.StatusStrip statusMain;
        private System.Windows.Forms.ToolStripMenuItem btn_System;
        private System.Windows.Forms.ToolStripMenuItem barSubItem3;
        private System.Windows.Forms.ToolStripButton btn_SaveSolution;
        private System.Windows.Forms.ToolStripButton btn_CreateVar;
        private System.Windows.Forms.ToolStripButton btn_HardwareCamera;
        private System.Windows.Forms.ToolStripStatusLabel barStaticItem1;
        private System.Windows.Forms.ToolStripButton btn_SolutionList;
        private System.Windows.Forms.ToolStripMenuItem btn_User;
        private System.Windows.Forms.ToolStripMenuItem btn_Login;
        private System.Windows.Forms.ToolStripMenuItem btn_Register;
        private System.Windows.Forms.ToolStripMenuItem btn_Permission;
        private System.Windows.Forms.ToolStripMenuItem barButtonItem1;
        private System.Windows.Forms.ToolStripMenuItem barSubItem4;
        private System.Windows.Forms.ToolStripMenuItem btn_Chinese;
        private System.Windows.Forms.ToolStripMenuItem btn_English;
        private System.Windows.Forms.ToolStripStatusLabel btn_Logout;
        private System.Windows.Forms.ToolStripStatusLabel txt_JobName;
        private System.Windows.Forms.ToolStripStatusLabel txt_RunTime;
        private System.Windows.Forms.ToolStripButton btn_Station;
        private System.Windows.Forms.ToolStripButton btn_UI;
        private System.Windows.Forms.TableLayoutPanel tlp_Display;
        private System.Windows.Forms.GroupBox grb_State;
        private System.Windows.Forms.GroupBox grb_Log;
        private System.Windows.Forms.SplitContainer split_Main;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.ToolStripMenuItem btn_File;
        private System.Windows.Forms.ToolStripStatusLabel txt_Memory;
        private System.Windows.Forms.ToolStripStatusLabel txt_CPU;
        private System.Windows.Forms.ToolStripStatusLabel txt_Disk;
        private System.Windows.Forms.ToolStripMenuItem tsm_SystemState;
        private System.Windows.Forms.ToolStripStatusLabel tsl_SystemState;
        private System.Windows.Forms.ToolStripButton tsm_LightControl;
        private System.Windows.Forms.ToolStripButton tsm_DLModel;
        private System.Windows.Forms.ToolStripButton tsm_Comm;
    }
}