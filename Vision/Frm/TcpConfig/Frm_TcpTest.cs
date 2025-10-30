using Logger;
using System;
using System.Drawing;
using System.Windows.Forms;
using Vision.Comm.TcpConfig;
using Vision.Comm.TcpManager;

namespace Vision.Frm.TcpConfig
{
    /// <summary>
 /// TCP收发测试窗口
    /// 注意：此窗口仅用于测试，不会创建新连接，使用TcpCommManager的现有连接
    /// </summary>
    public partial class Frm_TcpTest : Form
    {
        private TcpConfigModel _config;
        private int _sendCount = 0;
        private int _receiveCount = 0;

        public Frm_TcpTest(TcpConfigModel config)
        {
     InitializeComponent();
            _config = config;
            InitializeTest();
        }

     private void InitializeTest()
        {
  try
      {
    // 设置窗口标题
       this.Text = $"TCP收发测试 - {_config.Name}";

    // 订阅TcpCommManager的消息事件（仅监听当前配置）
      TcpCommManager.Instance.MessageReceived += OnMessageReceived;
       TcpCommManager.Instance.ConnectionStateChanged += OnConnectionStateChanged;

                // 显示初始状态
          UpdateConnectionStatus();

     AppendLog($"测试窗口已打开，使用现有连接: {_config.Name}");
                AppendLog($"类型: {_config.Type}, 地址: {_config.IpAddress}:{_config.Port}");
           AppendLog($"编码: {_config.Encoding}, 结束符: {(_config.UseTerminator ? _config.Terminator : "无")}");
      AppendLog("------------------------------------------------");
      }
  catch (Exception ex)
         {
            LogHelper.Error(ex, "初始化测试窗口失败");
     MessageBox.Show($"初始化失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
}
        }

        /// <summary>
      /// 消息接收处理（仅处理当前配置的消息）
  /// </summary>
      private void OnMessageReceived(string tcpName, string clientId, string msg)
     {
         if (tcpName != _config.Name) return;  // 只处理当前配置的消息

       _receiveCount++;
      UpdateStatistics();

            string source = string.IsNullOrEmpty(clientId) ? tcpName : $"{tcpName} from {clientId}";
  AppendReceiveLog($"[{DateTime.Now:HH:mm:ss.fff}] [接收 {source}] {msg}");
      }

        /// <summary>
        /// 连接状态变化处理
      /// </summary>
     private void OnConnectionStateChanged(string tcpName, bool connected)
     {
     if (tcpName != _config.Name) return;

            UpdateConnectionStatus();
            AppendLog($"[{DateTime.Now:HH:mm:ss.fff}] [状态] {(connected ? "已连接" : "已断开")}");
        }

        /// <summary>
        /// 更新连接状态显示
        /// </summary>
private void UpdateConnectionStatus()
        {
   if (InvokeRequired)
            {
                Invoke(new Action(UpdateConnectionStatus));
           return;
            }

      bool isActive = TcpCommManager.Instance.IsActive(_config.Name);
 lbl_Status.Text = isActive ? "状态: 已连接" : "状态: 未连接";
 lbl_Status.ForeColor = isActive ? Color.Green : Color.Red;
        }

        /// <summary>
        /// 更新统计信息
        /// </summary>
        private void UpdateStatistics()
        {
          if (InvokeRequired)
          {
                Invoke(new Action(UpdateStatistics));
  return;
    }

      lbl_Statistics.Text = $"发送: {_sendCount} 条 | 接收: {_receiveCount} 条";
        }

        /// <summary>
  /// 追加日志（通用日志区域）
        /// </summary>
        private void AppendLog(string message)
        {
            if (InvokeRequired)
      {
                Invoke(new Action<string>(AppendLog), message);
     return;
            }

          txt_Log.AppendText($"{message}\r\n");
            txt_Log.ScrollToCaret();
        }

        /// <summary>
        /// 追加接收日志（单独的接收显示区）
        /// </summary>
        private void AppendReceiveLog(string message)
      {
            if (InvokeRequired)
            {
            Invoke(new Action<string>(AppendReceiveLog), message);
     return;
     }

       txt_Receive.AppendText($"{message}\r\n");
         txt_Receive.ScrollToCaret();
   }

        /// <summary>
        /// 发送按钮点击
        /// </summary>
    private void btn_Send_Click(object sender, EventArgs e)
        {
var message = txt_Send.Text;
       if (string.IsNullOrWhiteSpace(message))
         {
    MessageBox.Show("请输入要发送的内容", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
              return;
        }

 try
            {
       // 根据选择的追加符添加后缀
   string suffix = "";
    if (rb_AppendCRLF.Checked) suffix = "\r\n";
        else if (rb_AppendCR.Checked) suffix = "\r";
             else if (rb_AppendLF.Checked) suffix = "\n";

            string fullMessage = message + suffix;

  // 使用TcpCommManager发送（不需要指定clientId，广播给所有客户端）
                bool success = TcpCommManager.Instance.Send(_config.Name, fullMessage);

       if (success)
        {
        _sendCount++;
        UpdateStatistics();
AppendLog($"[{DateTime.Now:HH:mm:ss.fff}] [发送] {message}{(string.IsNullOrEmpty(suffix) ? "" : $" (追加: {GetAppendName()})")}");

             if (chk_AutoClear.Checked)
             {
                txt_Send.Clear();
     }
       }
  else
         {
         AppendLog($"[{DateTime.Now:HH:mm:ss.fff}] [发送失败] 连接未激活或发送异常");
    }
            }
 catch (Exception ex)
          {
      AppendLog($"[{DateTime.Now:HH:mm:ss.fff}] [异常] {ex.Message}");
     LogHelper.Error(ex, "TCP测试发送失败");
            }
        }

        /// <summary>
        /// 获取追加符名称（用于日志显示）
        /// </summary>
        private string GetAppendName()
        {
       if (rb_AppendCRLF.Checked) return "\\r\\n";
        if (rb_AppendCR.Checked) return "\\r";
            if (rb_AppendLF.Checked) return "\\n";
        return "无";
        }

        /// <summary>
        /// 清空日志按钮
  /// </summary>
   private void btn_ClearLog_Click(object sender, EventArgs e)
    {
 txt_Log.Clear();
     txt_Receive.Clear();
       _sendCount = 0;
            _receiveCount = 0;
            UpdateStatistics();
     AppendLog("日志已清空");
  }

        /// <summary>
        /// 窗口关闭时取消订阅事件（不影响实际连接）
        /// </summary>
      protected override void OnFormClosing(FormClosingEventArgs e)
     {
     base.OnFormClosing(e);

    try
            {
      // 取消订阅事件
       TcpCommManager.Instance.MessageReceived -= OnMessageReceived;
     TcpCommManager.Instance.ConnectionStateChanged -= OnConnectionStateChanged;

           LogHelper.Info($"TCP测试窗口已关闭: {_config.Name}（连接保持）");
    }
  catch (Exception ex)
       {
          LogHelper.Error(ex, "关闭测试窗口异常");
       }
        }

        #region Designer

        private System.Windows.Forms.TextBox txt_Log;
        private System.Windows.Forms.TextBox txt_Send;
      private System.Windows.Forms.Button btn_Send;
        private System.Windows.Forms.Label lbl_Status;
        private System.Windows.Forms.Label lbl_Statistics;
        private System.Windows.Forms.RadioButton rb_AppendNone;
        private System.Windows.Forms.RadioButton rb_AppendCR;
        private System.Windows.Forms.RadioButton rb_AppendLF;
        private System.Windows.Forms.RadioButton rb_AppendCRLF;
        private System.Windows.Forms.CheckBox chk_AutoClear;
     private System.Windows.Forms.Button btn_ClearLog;
        private System.Windows.Forms.TextBox txt_Receive;
   private System.Windows.Forms.GroupBox grp_Append;
      private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;

 private void InitializeComponent()
 {
     this.txt_Log = new System.Windows.Forms.TextBox();
        this.txt_Send = new System.Windows.Forms.TextBox();
  this.btn_Send = new System.Windows.Forms.Button();
 this.lbl_Status = new System.Windows.Forms.Label();
this.lbl_Statistics = new System.Windows.Forms.Label();
   this.grp_Append = new System.Windows.Forms.GroupBox();
         this.rb_AppendCRLF = new System.Windows.Forms.RadioButton();
            this.rb_AppendLF = new System.Windows.Forms.RadioButton();
       this.rb_AppendCR = new System.Windows.Forms.RadioButton();
     this.rb_AppendNone = new System.Windows.Forms.RadioButton();
            this.chk_AutoClear = new System.Windows.Forms.CheckBox();
       this.btn_ClearLog = new System.Windows.Forms.Button();
    this.txt_Receive = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
          this.label2 = new System.Windows.Forms.Label();
            this.grp_Append.SuspendLayout();
            this.SuspendLayout();
   // 
            // txt_Log
     // 
    this.txt_Log.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txt_Log.BackColor = System.Drawing.Color.Black;
    this.txt_Log.Font = new System.Drawing.Font("Consolas", 9F);
            this.txt_Log.ForeColor = System.Drawing.Color.Lime;
        this.txt_Log.Location = new System.Drawing.Point(12, 65);
        this.txt_Log.Multiline = true;
            this.txt_Log.Name = "txt_Log";
            this.txt_Log.ReadOnly = true;
       this.txt_Log.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txt_Log.Size = new System.Drawing.Size(1160, 200);
            this.txt_Log.TabIndex = 0;
            // 
            // txt_Receive
      // 
            this.txt_Receive.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
        | System.Windows.Forms.AnchorStyles.Left) 
    | System.Windows.Forms.AnchorStyles.Right)));
            this.txt_Receive.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
this.txt_Receive.Font = new System.Drawing.Font("Consolas", 9F);
            this.txt_Receive.ForeColor = System.Drawing.Color.Cyan;
this.txt_Receive.Location = new System.Drawing.Point(12, 290);
        this.txt_Receive.Multiline = true;
            this.txt_Receive.Name = "txt_Receive";
   this.txt_Receive.ReadOnly = true;
            this.txt_Receive.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txt_Receive.Size = new System.Drawing.Size(1160, 250);
            this.txt_Receive.TabIndex = 7;
       // 
       // txt_Send
            // 
 this.txt_Send.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
     | System.Windows.Forms.AnchorStyles.Right)));
        this.txt_Send.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F);
         this.txt_Send.Location = new System.Drawing.Point(70, 556);
        this.txt_Send.Name = "txt_Send";
            this.txt_Send.Size = new System.Drawing.Size(960, 23);
    this.txt_Send.TabIndex = 1;
            // 
    // btn_Send
          // 
   this.btn_Send.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
       this.btn_Send.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Bold);
   this.btn_Send.Location = new System.Drawing.Point(1036, 554);
        this.btn_Send.Name = "btn_Send";
            this.btn_Send.Size = new System.Drawing.Size(136, 27);
            this.btn_Send.TabIndex = 2;
        this.btn_Send.Text = "发送数据";
  this.btn_Send.UseVisualStyleBackColor = true;
            this.btn_Send.Click += new System.EventHandler(this.btn_Send_Click);
        // 
    // lbl_Status
     // 
            this.lbl_Status.AutoSize = true;
  this.lbl_Status.Font = new System.Drawing.Font("Microsoft YaHei UI", 10F, System.Drawing.FontStyle.Bold);
    this.lbl_Status.ForeColor = System.Drawing.Color.Green;
          this.lbl_Status.Location = new System.Drawing.Point(12, 15);
       this.lbl_Status.Name = "lbl_Status";
          this.lbl_Status.Size = new System.Drawing.Size(93, 19);
            this.lbl_Status.TabIndex = 3;
            this.lbl_Status.Text = "状态: 未连接";
     // 
            // lbl_Statistics
     // 
   this.lbl_Statistics.AutoSize = true;
  this.lbl_Statistics.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F);
       this.lbl_Statistics.Location = new System.Drawing.Point(200, 17);
   this.lbl_Statistics.Name = "lbl_Statistics";
     this.lbl_Statistics.Size = new System.Drawing.Size(140, 17);
     this.lbl_Statistics.TabIndex = 4;
       this.lbl_Statistics.Text = "发送: 0 条 | 接收: 0 条";
       // 
            // grp_Append
    // 
this.grp_Append.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
     this.grp_Append.Controls.Add(this.rb_AppendCRLF);
        this.grp_Append.Controls.Add(this.rb_AppendLF);
        this.grp_Append.Controls.Add(this.rb_AppendCR);
       this.grp_Append.Controls.Add(this.rb_AppendNone);
         this.grp_Append.Location = new System.Drawing.Point(700, 586);
            this.grp_Append.Name = "grp_Append";
      this.grp_Append.Size = new System.Drawing.Size(330, 50);
     this.grp_Append.TabIndex = 5;
            this.grp_Append.TabStop = false;
 this.grp_Append.Text = "追加";
            // 
            // rb_AppendCRLF
 // 
            this.rb_AppendCRLF.AutoSize = true;
      this.rb_AppendCRLF.Checked = true;
          this.rb_AppendCRLF.Location = new System.Drawing.Point(250, 20);
            this.rb_AppendCRLF.Name = "rb_AppendCRLF";
      this.rb_AppendCRLF.Size = new System.Drawing.Size(59, 21);
            this.rb_AppendCRLF.TabIndex = 3;
        this.rb_AppendCRLF.TabStop = true;
       this.rb_AppendCRLF.Text = "\\r\\n";
  this.rb_AppendCRLF.UseVisualStyleBackColor = true;
   // 
      // rb_AppendLF
    // 
    this.rb_AppendLF.AutoSize = true;
            this.rb_AppendLF.Location = new System.Drawing.Point(175, 20);
      this.rb_AppendLF.Name = "rb_AppendLF";
    this.rb_AppendLF.Size = new System.Drawing.Size(42, 21);
   this.rb_AppendLF.TabIndex = 2;
            this.rb_AppendLF.Text = "\\n";
            this.rb_AppendLF.UseVisualStyleBackColor = true;
            // 
            // rb_AppendCR
          // 
            this.rb_AppendCR.AutoSize = true;
        this.rb_AppendCR.Location = new System.Drawing.Point(100, 20);
 this.rb_AppendCR.Name = "rb_AppendCR";
            this.rb_AppendCR.Size = new System.Drawing.Size(40, 21);
            this.rb_AppendCR.TabIndex = 1;
   this.rb_AppendCR.Text = "\\r";
            this.rb_AppendCR.UseVisualStyleBackColor = true;
         // 
            // rb_AppendNone
            // 
   this.rb_AppendNone.AutoSize = true;
        this.rb_AppendNone.Location = new System.Drawing.Point(15, 20);
     this.rb_AppendNone.Name = "rb_AppendNone";
     this.rb_AppendNone.Size = new System.Drawing.Size(62, 21);
       this.rb_AppendNone.TabIndex = 0;
         this.rb_AppendNone.Text = "None";
      this.rb_AppendNone.UseVisualStyleBackColor = true;
    // 
            // chk_AutoClear
// 
      this.chk_AutoClear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.chk_AutoClear.AutoSize = true;
            this.chk_AutoClear.Checked = true;
            this.chk_AutoClear.CheckState = System.Windows.Forms.CheckState.Checked;
   this.chk_AutoClear.Location = new System.Drawing.Point(1036, 606);
  this.chk_AutoClear.Name = "chk_AutoClear";
 this.chk_AutoClear.Size = new System.Drawing.Size(75, 21);
  this.chk_AutoClear.TabIndex = 6;
            this.chk_AutoClear.Text = "自动回显";
       this.chk_AutoClear.UseVisualStyleBackColor = true;
    // 
      // btn_ClearLog
   // 
            this.btn_ClearLog.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
this.btn_ClearLog.Location = new System.Drawing.Point(1036, 12);
       this.btn_ClearLog.Name = "btn_ClearLog";
 this.btn_ClearLog.Size = new System.Drawing.Size(136, 27);
     this.btn_ClearLog.TabIndex = 8;
    this.btn_ClearLog.Text = "清空日志";
 this.btn_ClearLog.UseVisualStyleBackColor = true;
            this.btn_ClearLog.Click += new System.EventHandler(this.btn_ClearLog_Click);
            // 
            // label1
         // 
            this.label1.AutoSize = true;
     this.label1.Location = new System.Drawing.Point(12, 45);
      this.label1.Name = "label1";
   this.label1.Size = new System.Drawing.Size(68, 17);
      this.label1.TabIndex = 9;
            this.label1.Text = "通用日志:";
        // 
    // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 270);
   this.label2.Name = "label2";
        this.label2.Size = new System.Drawing.Size(68, 17);
            this.label2.TabIndex = 10;
            this.label2.Text = "接收数据:";
 // 
          // Frm_TcpTest
            // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.ClientSize = new System.Drawing.Size(1184, 641);
            this.Controls.Add(this.label2);
          this.Controls.Add(this.label1);
  this.Controls.Add(this.btn_ClearLog);
     this.Controls.Add(this.txt_Receive);
            this.Controls.Add(this.chk_AutoClear);
        this.Controls.Add(this.grp_Append);
            this.Controls.Add(this.lbl_Statistics);
   this.Controls.Add(this.lbl_Status);
            this.Controls.Add(this.btn_Send);
        this.Controls.Add(this.txt_Send);
            this.Controls.Add(this.txt_Log);
          this.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F);
  this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
       this.MinimumSize = new System.Drawing.Size(1000, 600);
            this.Name = "Frm_TcpTest";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
       this.Text = "TCP收发测试";
     this.grp_Append.ResumeLayout(false);
    this.grp_Append.PerformLayout();
       this.ResumeLayout(false);
   this.PerformLayout();

        }

 #endregion
    }
}
