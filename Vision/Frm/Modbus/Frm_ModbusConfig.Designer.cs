namespace Vision.Frm.Modbus
{
    partial class Frm_ModbusConfig
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }

            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.splitMain = new System.Windows.Forms.SplitContainer();
            this.pnlLeft = new System.Windows.Forms.Panel();
            this.grpTest = new System.Windows.Forms.GroupBox();
            this.txtLog = new System.Windows.Forms.RichTextBox();
            this.pnlTestWrite = new System.Windows.Forms.Panel();
            this.label15 = new System.Windows.Forms.Label();
            this.txtWriteValue = new System.Windows.Forms.TextBox();
            this.txtWriteAddress = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.btnWriteString = new System.Windows.Forms.Button();
            this.btnWriteFloat = new System.Windows.Forms.Button();
            this.btnWriteShort = new System.Windows.Forms.Button();
            this.btnWriteBool = new System.Windows.Forms.Button();
            this.label13 = new System.Windows.Forms.Label();
            this.pnlTestRead = new System.Windows.Forms.Panel();
            this.txtReadResult = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.numReadLength = new System.Windows.Forms.NumericUpDown();
            this.label11 = new System.Windows.Forms.Label();
            this.txtReadAddress = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.btnReadString = new System.Windows.Forms.Button();
            this.btnReadFloat = new System.Windows.Forms.Button();
            this.btnReadShort = new System.Windows.Forms.Button();
            this.btnReadBool = new System.Windows.Forms.Button();
            this.label9 = new System.Windows.Forms.Label();
            this.grpConnection = new System.Windows.Forms.GroupBox();
            this.btnConnect = new System.Windows.Forms.Button();
            this.cmbDataFormat = new System.Windows.Forms.ComboBox();
            this.label8 = new System.Windows.Forms.Label();
            this.numReceiveTimeout = new System.Windows.Forms.NumericUpDown();
            this.label7 = new System.Windows.Forms.Label();
            this.numConnectTimeout = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.numStation = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.numPort = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.txtIpAddress = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.pnlRight = new System.Windows.Forms.Panel();
            this.tabVariables = new System.Windows.Forms.TabControl();
            this.pnlInputButtons = new System.Windows.Forms.Panel();
            this.btnDeleteInput = new System.Windows.Forms.Button();
            this.btnAddInputString = new System.Windows.Forms.Button();
            this.btnAddInputBool = new System.Windows.Forms.Button();
            this.btnAddInputFloat = new System.Windows.Forms.Button();
            this.btnAddInputShort = new System.Windows.Forms.Button();
            this.tabOutput = new System.Windows.Forms.TabPage();
            this.pnlOutputButtons = new System.Windows.Forms.Panel();
            this.btnDeleteOutput = new System.Windows.Forms.Button();
            this.btnAddOutputString = new System.Windows.Forms.Button();
            this.btnAddOutputBool = new System.Windows.Forms.Button();
            this.btnAddOutputFloat = new System.Windows.Forms.Button();
            this.btnAddOutputShort = new System.Windows.Forms.Button();
            this.pnlBottom = new System.Windows.Forms.Panel();
            this.btnSave = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.splitMain)).BeginInit();
            this.splitMain.Panel1.SuspendLayout();
            this.splitMain.Panel2.SuspendLayout();
            this.splitMain.SuspendLayout();
            this.pnlLeft.SuspendLayout();
            this.grpTest.SuspendLayout();
            this.pnlTestWrite.SuspendLayout();
            this.pnlTestRead.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numReadLength)).BeginInit();
            this.grpConnection.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numReceiveTimeout)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numConnectTimeout)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numStation)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPort)).BeginInit();
            this.pnlRight.SuspendLayout();
            this.tabVariables.SuspendLayout();
            this.pnlInputButtons.SuspendLayout();
            this.tabOutput.SuspendLayout();
            this.pnlOutputButtons.SuspendLayout();
            this.pnlBottom.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitMain
            // 
            this.splitMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitMain.Location = new System.Drawing.Point(0, 0);
            this.splitMain.Name = "splitMain";
            // 
            // splitMain.Panel1
            // 
            this.splitMain.Panel1.Controls.Add(this.pnlLeft);
            // 
            // splitMain.Panel2
            // 
            this.splitMain.Panel2.Controls.Add(this.pnlRight);
            this.splitMain.Size = new System.Drawing.Size(1400, 800);
            this.splitMain.SplitterDistance = 600;
            this.splitMain.TabIndex = 0;
            // 
// pnlLeft
            // 
            this.pnlLeft.Controls.Add(this.grpTest);
            this.pnlLeft.Controls.Add(this.grpConnection);
            this.pnlLeft.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlLeft.Location = new System.Drawing.Point(0, 0);
            this.pnlLeft.Name = "pnlLeft";
            this.pnlLeft.Size = new System.Drawing.Size(600, 800);
            this.pnlLeft.TabIndex = 0;
            // 
            // grpTest
            // 
            this.grpTest.Controls.Add(this.txtLog);
            this.grpTest.Controls.Add(this.pnlTestWrite);
            this.grpTest.Controls.Add(this.pnlTestRead);
            this.grpTest.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpTest.Location = new System.Drawing.Point(0, 280);
            this.grpTest.Name = "grpTest";
            this.grpTest.Size = new System.Drawing.Size(600, 520);
            this.grpTest.TabIndex = 1;
            this.grpTest.TabStop = false;
            this.grpTest.Text = "数据读写测试";
            // 
            // txtLog
            // 
            this.txtLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtLog.Location = new System.Drawing.Point(3, 351);
            this.txtLog.Name = "txtLog";
            this.txtLog.ReadOnly = true;
            this.txtLog.Size = new System.Drawing.Size(594, 166);
            this.txtLog.TabIndex = 2;
            this.txtLog.Text = "";
            // 
            // pnlTestWrite
            // 
            this.pnlTestWrite.Controls.Add(this.label15);
            this.pnlTestWrite.Controls.Add(this.txtWriteValue);
            this.pnlTestWrite.Controls.Add(this.txtWriteAddress);
            this.pnlTestWrite.Controls.Add(this.label14);
            this.pnlTestWrite.Controls.Add(this.btnWriteString);
            this.pnlTestWrite.Controls.Add(this.btnWriteFloat);
            this.pnlTestWrite.Controls.Add(this.btnWriteShort);
            this.pnlTestWrite.Controls.Add(this.btnWriteBool);
            this.pnlTestWrite.Controls.Add(this.label13);
            this.pnlTestWrite.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTestWrite.Location = new System.Drawing.Point(3, 191);
            this.pnlTestWrite.Name = "pnlTestWrite";
            this.pnlTestWrite.Size = new System.Drawing.Size(594, 160);
            this.pnlTestWrite.TabIndex = 1;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(15, 48);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(32, 17);
            this.label15.TabIndex = 7;
            this.label15.Text = "值:";
            // 
            // txtWriteValue
// 
            this.txtWriteValue.Location = new System.Drawing.Point(60, 45);
            this.txtWriteValue.Name = "txtWriteValue";
            this.txtWriteValue.Size = new System.Drawing.Size(200, 23);
            this.txtWriteValue.TabIndex = 6;
            this.txtWriteValue.Text = "0";
            // 
            // txtWriteAddress
            // 
            this.txtWriteAddress.Location = new System.Drawing.Point(60, 15);
            this.txtWriteAddress.Name = "txtWriteAddress";
            this.txtWriteAddress.Size = new System.Drawing.Size(200, 23);
            this.txtWriteAddress.TabIndex = 5;
            this.txtWriteAddress.Text = "D100";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(15, 18);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(44, 17);
            this.label14.TabIndex = 4;
            this.label14.Text = "地址:";
            // 
            // btnWriteString
// 
            this.btnWriteString.Location = new System.Drawing.Point(285, 85);
            this.btnWriteString.Name = "btnWriteString";
            this.btnWriteString.Size = new System.Drawing.Size(80, 50);
            this.btnWriteString.TabIndex = 4;
            this.btnWriteString.Text = "String写入";
            this.btnWriteString.UseVisualStyleBackColor = true;
            // 
            // btnWriteFloat
            // 
            this.btnWriteFloat.Location = new System.Drawing.Point(195, 85);
            this.btnWriteFloat.Name = "btnWriteFloat";
            this.btnWriteFloat.Size = new System.Drawing.Size(80, 50);
            this.btnWriteFloat.TabIndex = 3;
            this.btnWriteFloat.Text = "Float写入";
            this.btnWriteFloat.UseVisualStyleBackColor = true;
            // 
            // btnWriteShort
            // 
            this.btnWriteShort.Location = new System.Drawing.Point(105, 85);
            this.btnWriteShort.Name = "btnWriteShort";
            this.btnWriteShort.Size = new System.Drawing.Size(80, 50);
            this.btnWriteShort.TabIndex = 2;
            this.btnWriteShort.Text = "Short写入";
            this.btnWriteShort.UseVisualStyleBackColor = true;
            // 
            // btnWriteBool
            // 
            this.btnWriteBool.Location = new System.Drawing.Point(15, 85);
            this.btnWriteBool.Name = "btnWriteBool";
            this.btnWriteBool.Size = new System.Drawing.Size(80, 50);
            this.btnWriteBool.TabIndex = 1;
            this.btnWriteBool.Text = "Bool写入";
            this.btnWriteBool.UseVisualStyleBackColor = true;
            // 
            // label13
            // 
            this.label13.BackColor = System.Drawing.SystemColors.ControlDark;
            this.label13.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.label13.Location = new System.Drawing.Point(0, 158);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(594, 2);
            this.label13.TabIndex = 0;
            // 
            // pnlTestRead
            // 
            this.pnlTestRead.Controls.Add(this.txtReadResult);
            this.pnlTestRead.Controls.Add(this.label12);
            this.pnlTestRead.Controls.Add(this.numReadLength);
            this.pnlTestRead.Controls.Add(this.label11);
            this.pnlTestRead.Controls.Add(this.txtReadAddress);
            this.pnlTestRead.Controls.Add(this.label10);
            this.pnlTestRead.Controls.Add(this.btnReadString);
            this.pnlTestRead.Controls.Add(this.btnReadFloat);
            this.pnlTestRead.Controls.Add(this.btnReadShort);
            this.pnlTestRead.Controls.Add(this.btnReadBool);
            this.pnlTestRead.Controls.Add(this.label9);
            this.pnlTestRead.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTestRead.Location = new System.Drawing.Point(3, 19);
            this.pnlTestRead.Name = "pnlTestRead";
            this.pnlTestRead.Size = new System.Drawing.Size(594, 172);
            this.pnlTestRead.TabIndex = 0;
            // 
            // txtReadResult
            // 
            this.txtReadResult.Location = new System.Drawing.Point(60, 75);
            this.txtReadResult.Multiline = true;
            this.txtReadResult.Name = "txtReadResult";
            this.txtReadResult.ReadOnly = true;
            this.txtReadResult.Size = new System.Drawing.Size(500, 60);
            this.txtReadResult.TabIndex = 9;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(15, 78);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(44, 17);
            this.label12.TabIndex = 8;
            this.label12.Text = "结果:";
            // 
// numReadLength
            // 
            this.numReadLength.Location = new System.Drawing.Point(320, 15);
            this.numReadLength.Maximum = new decimal(new int[] { 100, 0, 0, 0 });
            this.numReadLength.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            this.numReadLength.Name = "numReadLength";
            this.numReadLength.Size = new System.Drawing.Size(80, 23);
            this.numReadLength.TabIndex = 7;
            this.numReadLength.Value = new decimal(new int[] { 10, 0, 0, 0 });
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(270, 18);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(44, 17);
            this.label11.TabIndex = 6;
            this.label11.Text = "长度:";
            // 
            // txtReadAddress
            // 
            this.txtReadAddress.Location = new System.Drawing.Point(60, 15);
            this.txtReadAddress.Name = "txtReadAddress";
            this.txtReadAddress.Size = new System.Drawing.Size(200, 23);
            this.txtReadAddress.TabIndex = 5;
            this.txtReadAddress.Text = "D100";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(15, 18);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(44, 17);
            this.label10.TabIndex = 4;
            this.label10.Text = "地址:";
            // 
            // btnReadString
            // 
            this.btnReadString.Location = new System.Drawing.Point(285, 145);
            this.btnReadString.Name = "btnReadString";
            this.btnReadString.Size = new System.Drawing.Size(80, 25);
            this.btnReadString.TabIndex = 4;
            this.btnReadString.Text = "String读取";
            this.btnReadString.UseVisualStyleBackColor = true;
            // 
// btnReadFloat
            // 
            this.btnReadFloat.Location = new System.Drawing.Point(195, 145);
            this.btnReadFloat.Name = "btnReadFloat";
            this.btnReadFloat.Size = new System.Drawing.Size(80, 25);
            this.btnReadFloat.TabIndex = 3;
            this.btnReadFloat.Text = "Float读取";
            this.btnReadFloat.UseVisualStyleBackColor = true;
            // 
            // btnReadShort
            // 
            this.btnReadShort.Location = new System.Drawing.Point(105, 145);
            this.btnReadShort.Name = "btnReadShort";
            this.btnReadShort.Size = new System.Drawing.Size(80, 25);
            this.btnReadShort.TabIndex = 2;
            this.btnReadShort.Text = "Short读取";
            this.btnReadShort.UseVisualStyleBackColor = true;
            // 
            // btnReadBool
            // 
            this.btnReadBool.Location = new System.Drawing.Point(15, 145);
            this.btnReadBool.Name = "btnReadBool";
            this.btnReadBool.Size = new System.Drawing.Size(80, 25);
            this.btnReadBool.TabIndex = 1;
            this.btnReadBool.Text = "Bool读取";
            this.btnReadBool.UseVisualStyleBackColor = true;
            // 
            // label9
            // 
            this.label9.BackColor = System.Drawing.SystemColors.ControlDark;
            this.label9.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.label9.Location = new System.Drawing.Point(0, 170);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(594, 2);
            this.label9.TabIndex = 0;
            // 
            // grpConnection
            // 
            this.grpConnection.Controls.Add(this.btnConnect);
            this.grpConnection.Controls.Add(this.cmbDataFormat);
            this.grpConnection.Controls.Add(this.label8);
            this.grpConnection.Controls.Add(this.numReceiveTimeout);
            this.grpConnection.Controls.Add(this.label7);
            this.grpConnection.Controls.Add(this.numConnectTimeout);
            this.grpConnection.Controls.Add(this.label6);
            this.grpConnection.Controls.Add(this.numStation);
            this.grpConnection.Controls.Add(this.label5);
            this.grpConnection.Controls.Add(this.numPort);
            this.grpConnection.Controls.Add(this.label4);
            this.grpConnection.Controls.Add(this.txtIpAddress);
            this.grpConnection.Controls.Add(this.label3);
            this.grpConnection.Dock = System.Windows.Forms.DockStyle.Top;
            this.grpConnection.Location = new System.Drawing.Point(0, 0);
            this.grpConnection.Name = "grpConnection";
            this.grpConnection.Size = new System.Drawing.Size(600, 280);
            this.grpConnection.TabIndex = 0;
            this.grpConnection.TabStop = false;
            this.grpConnection.Text = "连接配置";
            // 
            // btnConnect
            // 
            this.btnConnect.Font = new System.Drawing.Font("Microsoft YaHei UI", 12F, System.Drawing.FontStyle.Bold);
            this.btnConnect.Location = new System.Drawing.Point(450, 20);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(120, 240);
            this.btnConnect.TabIndex = 12;
            this.btnConnect.Text = "连接";
            this.btnConnect.UseVisualStyleBackColor = true;
            // 
            // cmbDataFormat
            // 
            this.cmbDataFormat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDataFormat.FormattingEnabled = true;
            this.cmbDataFormat.Items.AddRange(new object[] { "ABCD", "CDAB", "BADC", "DCBA" });
            this.cmbDataFormat.Location = new System.Drawing.Point(140, 155);
            this.cmbDataFormat.Name = "cmbDataFormat";
            this.cmbDataFormat.Size = new System.Drawing.Size(150, 25);
            this.cmbDataFormat.TabIndex = 11;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(20, 158);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(116, 17);
            this.label8.TabIndex = 10;
            this.label8.Text = "DataFormat(字节序):";
            // 
            // numReceiveTimeout
            // 
            this.numReceiveTimeout.Increment = new decimal(new int[] { 1000, 0, 0, 0 });
            this.numReceiveTimeout.Location = new System.Drawing.Point(140, 225);
            this.numReceiveTimeout.Maximum = new decimal(new int[] { 60000, 0, 0, 0 });
            this.numReceiveTimeout.Minimum = new decimal(new int[] { 100, 0, 0, 0 });
            this.numReceiveTimeout.Name = "numReceiveTimeout";
            this.numReceiveTimeout.Size = new System.Drawing.Size(150, 23);
            this.numReceiveTimeout.TabIndex = 9;
            this.numReceiveTimeout.Value = new decimal(new int[] { 10000, 0, 0, 0 });
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(20, 227);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(92, 17);
            this.label7.TabIndex = 8;
            this.label7.Text = "接收超时(ms):";
            // 
            // numConnectTimeout
            // 
            this.numConnectTimeout.Increment = new decimal(new int[] { 1000, 0, 0, 0 });
            this.numConnectTimeout.Location = new System.Drawing.Point(140, 190);
            this.numConnectTimeout.Maximum = new decimal(new int[] { 60000, 0, 0, 0 });
            this.numConnectTimeout.Minimum = new decimal(new int[] { 100, 0, 0, 0 });
            this.numConnectTimeout.Name = "numConnectTimeout";
            this.numConnectTimeout.Size = new System.Drawing.Size(150, 23);
            this.numConnectTimeout.TabIndex = 7;
            this.numConnectTimeout.Value = new decimal(new int[] { 5000, 0, 0, 0 });
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(20, 192);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(92, 17);
            this.label6.TabIndex = 6;
            this.label6.Text = "连接超时(ms):";
            // 
            // numStation
            // 
            this.numStation.Location = new System.Drawing.Point(140, 120);
            this.numStation.Maximum = new decimal(new int[] { 255, 0, 0, 0 });
            this.numStation.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            this.numStation.Name = "numStation";
            this.numStation.Size = new System.Drawing.Size(150, 23);
            this.numStation.TabIndex = 5;
            this.numStation.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(20, 122);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(44, 17);
            this.label5.TabIndex = 4;
            this.label5.Text = "站号:";
            // 
            // numPort
            // 
            this.numPort.Location = new System.Drawing.Point(140, 85);
            this.numPort.Maximum = new decimal(new int[] { 65535, 0, 0, 0 });
            this.numPort.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            this.numPort.Name = "numPort";
            this.numPort.Size = new System.Drawing.Size(150, 23);
            this.numPort.TabIndex = 3;
            this.numPort.Value = new decimal(new int[] { 502, 0, 0, 0 });
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(20, 87);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(56, 17);
            this.label4.TabIndex = 2;
            this.label4.Text = "端口号:";
            // 
            // txtIpAddress
            // 
            this.txtIpAddress.Location = new System.Drawing.Point(140, 50);
            this.txtIpAddress.Name = "txtIpAddress";
            this.txtIpAddress.Size = new System.Drawing.Size(200, 23);
            this.txtIpAddress.TabIndex = 1;
            this.txtIpAddress.Text = "127.0.0.1";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(20, 53);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(56, 17);
            this.label3.TabIndex = 0;
            this.label3.Text = "IP地址:";
            // 
            // pnlRight
            // 
            this.pnlRight.Controls.Add(this.tabVariables);
            this.pnlRight.Controls.Add(this.pnlBottom);
            this.pnlRight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlRight.Location = new System.Drawing.Point(0, 0);
            this.pnlRight.Name = "pnlRight";
            this.pnlRight.Size = new System.Drawing.Size(796, 800);
            this.pnlRight.TabIndex = 0;
            // 
            // pnlInputButtons
            // 
            this.pnlInputButtons.Controls.Add(this.btnDeleteInput);
            this.pnlInputButtons.Controls.Add(this.btnAddInputString);
            this.pnlInputButtons.Controls.Add(this.btnAddInputBool);
            this.pnlInputButtons.Controls.Add(this.btnAddInputFloat);
            this.pnlInputButtons.Controls.Add(this.btnAddInputShort);
            this.pnlInputButtons.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlInputButtons.Location = new System.Drawing.Point(3, 3);
            this.pnlInputButtons.Name = "pnlInputButtons";
            this.pnlInputButtons.Size = new System.Drawing.Size(782, 60);
            this.pnlInputButtons.TabIndex = 0;
            // 
            // btnDeleteInput
            // 
            this.btnDeleteInput.Location = new System.Drawing.Point(450, 10);
            this.btnDeleteInput.Name = "btnDeleteInput";
            this.btnDeleteInput.Size = new System.Drawing.Size(100, 40);
            this.btnDeleteInput.TabIndex = 4;
            this.btnDeleteInput.Text = "删除选中";
            this.btnDeleteInput.UseVisualStyleBackColor = true;
// 
            // btnAddInputString
            // 
            this.btnAddInputString.Location = new System.Drawing.Point(330, 10);
            this.btnAddInputString.Name = "btnAddInputString";
            this.btnAddInputString.Size = new System.Drawing.Size(100, 40);
            this.btnAddInputString.TabIndex = 3;
            this.btnAddInputString.Text = "添加String";
            this.btnAddInputString.UseVisualStyleBackColor = true;
            // 
            // btnAddInputBool
            // 
            this.btnAddInputBool.Location = new System.Drawing.Point(220, 10);
            this.btnAddInputBool.Name = "btnAddInputBool";
            this.btnAddInputBool.Size = new System.Drawing.Size(100, 40);
            this.btnAddInputBool.TabIndex = 2;
            this.btnAddInputBool.Text = "添加Bool";
            this.btnAddInputBool.UseVisualStyleBackColor = true;
            // 
            // btnAddInputFloat
// 
            this.btnAddInputFloat.Location = new System.Drawing.Point(110, 10);
            this.btnAddInputFloat.Name = "btnAddInputFloat";
            this.btnAddInputFloat.Size = new System.Drawing.Size(100, 40);
            this.btnAddInputFloat.TabIndex = 1;
            this.btnAddInputFloat.Text = "添加Float";
            this.btnAddInputFloat.UseVisualStyleBackColor = true;
            // 
            // btnAddInputShort
            // 
            this.btnAddInputShort.Location = new System.Drawing.Point(0, 10);
            this.btnAddInputShort.Name = "btnAddInputShort";
            this.btnAddInputShort.Size = new System.Drawing.Size(100, 40);
            this.btnAddInputShort.TabIndex = 0;
            this.btnAddInputShort.Text = "添加Short";
            this.btnAddInputShort.UseVisualStyleBackColor = true;
            // 
            // pnlOutputButtons
            // 
            this.pnlOutputButtons.Controls.Add(this.btnDeleteOutput);
            this.pnlOutputButtons.Controls.Add(this.btnAddOutputString);
            this.pnlOutputButtons.Controls.Add(this.btnAddOutputBool);
            this.pnlOutputButtons.Controls.Add(this.btnAddOutputFloat);
            this.pnlOutputButtons.Controls.Add(this.btnAddOutputShort);
            this.pnlOutputButtons.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlOutputButtons.Location = new System.Drawing.Point(3, 3);
            this.pnlOutputButtons.Name = "pnlOutputButtons";
            this.pnlOutputButtons.Size = new System.Drawing.Size(782, 60);
            this.pnlOutputButtons.TabIndex = 0;
            // 
            // btnDeleteOutput
            // 
            this.btnDeleteOutput.Location = new System.Drawing.Point(450, 10);
            this.btnDeleteOutput.Name = "btnDeleteOutput";
            this.btnDeleteOutput.Size = new System.Drawing.Size(100, 40);
            this.btnDeleteOutput.TabIndex = 4;
            this.btnDeleteOutput.Text = "删除选中";
            this.btnDeleteOutput.UseVisualStyleBackColor = true;
            // 
            // btnAddOutputString
            // 
            this.btnAddOutputString.Location = new System.Drawing.Point(330, 10);
            this.btnAddOutputString.Name = "btnAddOutputString";
            this.btnAddOutputString.Size = new System.Drawing.Size(100, 40);
            this.btnAddOutputString.TabIndex = 3;
            this.btnAddOutputString.Text = "添加String";
            this.btnAddOutputString.UseVisualStyleBackColor = true;
            // 
            // btnAddOutputBool
            // 
            this.btnAddOutputBool.Location = new System.Drawing.Point(220, 10);
            this.btnAddOutputBool.Name = "btnAddOutputBool";
            this.btnAddOutputBool.Size = new System.Drawing.Size(100, 40);
            this.btnAddOutputBool.TabIndex = 2;
            this.btnAddOutputBool.Text = "添加Bool";
            this.btnAddOutputBool.UseVisualStyleBackColor = true;
            // 
            // btnAddOutputFloat
            // 
            this.btnAddOutputFloat.Location = new System.Drawing.Point(110, 10);
            this.btnAddOutputFloat.Name = "btnAddOutputFloat";
            this.btnAddOutputFloat.Size = new System.Drawing.Size(100, 40);
            this.btnAddOutputFloat.TabIndex = 1;
            this.btnAddOutputFloat.Text = "添加Float";
            this.btnAddOutputFloat.UseVisualStyleBackColor = true;
            // 
            // btnAddOutputShort
            // 
            this.btnAddOutputShort.Location = new System.Drawing.Point(0, 10);
            this.btnAddOutputShort.Name = "btnAddOutputShort";
            this.btnAddOutputShort.Size = new System.Drawing.Size(100, 40);
            this.btnAddOutputShort.TabIndex = 0;
            this.btnAddOutputShort.Text = "添加Short";
            this.btnAddOutputShort.UseVisualStyleBackColor = true;
// 
            // pnlBottom
            // 
            this.pnlBottom.Controls.Add(this.btnSave);
            this.pnlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlBottom.Location = new System.Drawing.Point(0, 740);
            this.pnlBottom.Name = "pnlBottom";
            this.pnlBottom.Size = new System.Drawing.Size(796, 60);
            this.pnlBottom.TabIndex = 0;
            // 
            // btnSave
            // 
            this.btnSave.Font = new System.Drawing.Font("Microsoft YaHei UI", 10F);
            this.btnSave.Location = new System.Drawing.Point(676, 10);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(100, 40);
            this.btnSave.TabIndex = 0;
            this.btnSave.Text = "保存配置";
            this.btnSave.UseVisualStyleBackColor = true;
            // 
            // Frm_ModbusConfig
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1400, 800);
            this.Controls.Add(this.splitMain);
            this.Name = "Frm_ModbusConfig";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Modbus通讯配置";
            this.splitMain.Panel1.ResumeLayout(false);
            this.splitMain.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitMain)).EndInit();
            this.splitMain.ResumeLayout(false);
            this.pnlLeft.ResumeLayout(false);
            this.grpTest.ResumeLayout(false);
            this.pnlTestWrite.ResumeLayout(false);
            this.pnlTestWrite.PerformLayout();
            this.pnlTestRead.ResumeLayout(false);
            this.pnlTestRead.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numReadLength)).EndInit();
            this.grpConnection.ResumeLayout(false);
            this.grpConnection.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numReceiveTimeout)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numConnectTimeout)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numStation)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPort)).EndInit();
            this.pnlRight.ResumeLayout(false);
            this.tabVariables.ResumeLayout(false);
            this.pnlInputButtons.ResumeLayout(false);
            this.tabOutput.ResumeLayout(false);
            this.pnlOutputButtons.ResumeLayout(false);
            this.pnlBottom.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.SplitContainer splitMain;
        private System.Windows.Forms.Panel pnlLeft;
        private System.Windows.Forms.GroupBox grpConnection;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.ComboBox cmbDataFormat;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.NumericUpDown numReceiveTimeout;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.NumericUpDown numConnectTimeout;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.NumericUpDown numStation;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown numPort;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtIpAddress;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox grpTest;
        private System.Windows.Forms.Panel pnlTestRead;
        private System.Windows.Forms.Button btnReadFloat;
        private System.Windows.Forms.Button btnReadShort;
        private System.Windows.Forms.Button btnReadBool;
        private System.Windows.Forms.Button btnReadString;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Panel pnlTestWrite;
        private System.Windows.Forms.Button btnWriteFloat;
        private System.Windows.Forms.Button btnWriteShort;
        private System.Windows.Forms.Button btnWriteBool;
        private System.Windows.Forms.Button btnWriteString;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.RichTextBox txtLog;
        private System.Windows.Forms.Panel pnlRight;
        private System.Windows.Forms.TabControl tabVariables;
        private System.Windows.Forms.Panel pnlInputButtons;
        private System.Windows.Forms.Button btnDeleteInput;
        private System.Windows.Forms.Button btnAddInputString;
        private System.Windows.Forms.Button btnAddInputBool;
        private System.Windows.Forms.Button btnAddInputFloat;
        private System.Windows.Forms.Button btnAddInputShort;
        private System.Windows.Forms.TabPage tabOutput;
        private System.Windows.Forms.Panel pnlOutputButtons;
        private System.Windows.Forms.Button btnDeleteOutput;
        private System.Windows.Forms.Button btnAddOutputString;
        private System.Windows.Forms.Button btnAddOutputBool;
        private System.Windows.Forms.Button btnAddOutputFloat;
        private System.Windows.Forms.Button btnAddOutputShort;
        private System.Windows.Forms.Panel pnlBottom;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.TextBox txtReadResult;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.NumericUpDown numReadLength;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox txtReadAddress;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.TextBox txtWriteValue;
        private System.Windows.Forms.TextBox txtWriteAddress;
        private System.Windows.Forms.Label label14;
    }
}
