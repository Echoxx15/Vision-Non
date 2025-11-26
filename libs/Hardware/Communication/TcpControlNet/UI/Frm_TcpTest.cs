using System;
using System.Drawing;
using System.Windows.Forms;
using HardwareCommNet;

namespace TcpControlNet.UI;

/// <summary>
/// TCP 收发测试窗口
/// 用于测试 TcpClientAdapter 和 TcpServerAdapter 的收发功能
/// </summary>
public partial class Frm_TcpTest : Form
{
	private readonly IComm _device;
	private readonly string _deviceName;
	private readonly TcpType _deviceType;
	private int _sendCount = 0;
	private int _receiveCount = 0;

	public Frm_TcpTest(IComm device, TcpType deviceType)
	{
		_device = device ?? throw new ArgumentNullException(nameof(device));
		_deviceName = device.Name;
		_deviceType = deviceType;

		InitializeComponent();
		InitializeTest();
	}

	private void InitializeTest()
	{
		try
		{
			// ? 添加调试输出
			Console.WriteLine($"[测试窗口] ========== 初始化测试窗口 ==========");
			Console.WriteLine($"[测试窗口] 设备类型: {_device?.GetType().FullName}");
			Console.WriteLine($"[测试窗口] 设备名称: {_deviceName}");
			Console.WriteLine($"[测试窗口] 连接类型: {_deviceType}");
			Console.WriteLine($"[测试窗口] 连接状态: {_device?.IsConnected}");

			// 设置窗口标题
			this.Text = $"TCP 收发测试 - {_deviceName} ({(_deviceType == TcpType.Client ? "客户端" : "服务器")})";

			// 订阅设备事件
			_device.MessageReceived += OnMessageReceived;
			_device.ConnectionStatusChanged += OnConnectionStatusChanged;

			// ? 验证事件订阅
			Console.WriteLine($"[测试窗口] 已订阅 MessageReceived 事件");
			Console.WriteLine($"[测试窗口] 已订阅 ConnectionStatusChanged 事件");

			// 显示初始状态
			UpdateConnectionStatus();
			UpdateStatistics();

			// ? 隐藏连接按钮（设备连接由配置界面管理）
			btn_Connect.Visible = false;

			AppendLog($"测试窗口已打开");
			AppendLog($"设备: {_deviceName}");
			AppendLog($"类型: {(_deviceType == TcpType.Client ? "TCP客户端" : "TCP服务器")}");
			AppendLog($"状态: {(_device.IsConnected ? "已连接" : "未连接")}");

			if (!_device.IsConnected)
			{
				AppendLog("?? 设备未连接，请在配置界面连接设备后再进行测试");
			}

			AppendLog("------------------------------------------------");

			Console.WriteLine($"[测试窗口] ========== 初始化完成 ==========");
		}
		catch (Exception ex)
		{
			Console.WriteLine($"[测试窗口] ? 初始化异常: {ex.Message}");
			Console.WriteLine($"[测试窗口] 堆栈: {ex.StackTrace}");
			MessageBox.Show($"初始化测试窗口失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
		}
	}

	/// <summary>
	/// 消息接收处理
	/// </summary>
	private void OnMessageReceived(object sender, object msg)
	{
		// ? 第一行就输出，确保方法被调用
		Console.WriteLine($"[测试窗口] ########## OnMessageReceived 被调用 ##########");
		Console.WriteLine($"[测试窗口] Sender: {sender?.GetType().Name}");
		Console.WriteLine($"[测试窗口] Message Type: {msg?.GetType().FullName}");
		Console.WriteLine($"[测试窗口] Message Content: {msg}");
		Console.WriteLine($"[测试窗口] 当前线程ID: {System.Threading.Thread.CurrentThread.ManagedThreadId}");
		Console.WriteLine($"[测试窗口] InvokeRequired: {InvokeRequired}");

		_receiveCount++;
		UpdateStatistics();

		string message = msg?.ToString() ?? "";

		// 检查是否是服务器消息（可能包含客户端信息）
		if (_deviceType == TcpType.Server && msg != null)
		{
			try
			{
				Console.WriteLine($"[测试窗口] 开始解析服务器消息...");

				// 尝试解析为动态对象（服务器消息格式：{ ClientId, Message }）
				var type = msg.GetType();
				Console.WriteLine($"[测试窗口] 消息类型名称: {type.Name}");

				var clientIdProp = type.GetProperty("ClientId");
				var messageProp = type.GetProperty("Message");

				Console.WriteLine($"[测试窗口] ClientId 属性存在: {clientIdProp != null}");
				Console.WriteLine($"[测试窗口] Message 属性存在: {messageProp != null}");

				if (clientIdProp != null && messageProp != null)
				{
					var clientId = clientIdProp.GetValue(msg)?.ToString();
					var text = messageProp.GetValue(msg)?.ToString();
					message = $"[客户端 {clientId}] {text}";

					Console.WriteLine($"[测试窗口] ? 解析成功 - ClientId={clientId}, Text={text}");
				}
				else
				{
					Console.WriteLine($"[测试窗口] ?? 消息对象没有 ClientId/Message 属性");
				}
			}
			catch (Exception ex)
			{
				// 解析失败，使用原始消息
				Console.WriteLine($"[测试窗口] ? 解析失败: {ex.Message}");
			}
		}

		Console.WriteLine($"[测试窗口] 准备更新UI，最终消息: {message}");

		// ? 调用前输出
		Console.WriteLine($"[测试窗口] 调用 AppendReceiveLog...");
		AppendReceiveLog($"[{DateTime.Now:HH:mm:ss.fff}] [接收] {message}");
		Console.WriteLine($"[测试窗口] AppendReceiveLog 调用完成");
		Console.WriteLine($"[测试窗口] ########## OnMessageReceived 结束 ##########");
	}

	/// <summary>
	/// 连接状态变化处理
	/// </summary>
	private void OnConnectionStatusChanged(object sender, bool connected)
	{
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

		bool isConnected = _device.IsConnected;
		lbl_Status.Text = isConnected ? "状态: 已连接" : "状态: 未连接";
		lbl_Status.ForeColor = isConnected ? Color.Green : Color.Red;

		// 根据连接状态启用/禁用发送按钮
		btn_Send.Enabled = isConnected;
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
		// ? 第一行就输出
		Console.WriteLine($"[测试窗口] --- AppendReceiveLog 被调用 ---");
		Console.WriteLine($"[测试窗口] 消息内容: {message}");
		Console.WriteLine($"[测试窗口] InvokeRequired: {InvokeRequired}");

		if (InvokeRequired)
		{
			Console.WriteLine($"[测试窗口] 需要跨线程调用，使用 Invoke");
			Invoke(new Action<string>(AppendReceiveLog), message);
			return;
		}

		Console.WriteLine($"[测试窗口] 在主线程，直接更新UI");
		Console.WriteLine($"[测试窗口] txt_Receive 是否为空: {txt_Receive == null}");
		Console.WriteLine($"[测试窗口] txt_Receive.IsDisposed: {txt_Receive?.IsDisposed}");

		try
		{
			txt_Receive.AppendText($"{message}\r\n");
			txt_Receive.ScrollToCaret();
			Console.WriteLine($"[测试窗口] ? UI 更新成功");
		}
		catch (Exception ex)
		{
			Console.WriteLine($"[测试窗口] ? UI 更新失败: {ex.Message}");
		}

		Console.WriteLine($"[测试窗口] --- AppendReceiveLog 结束 ---");
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

			// 发送数据（TCP不需要地址，传入null或空字符串）
			_device.Write(null, fullMessage);

			_sendCount++;
			UpdateStatistics();
			AppendLog(
				$"[{DateTime.Now:HH:mm:ss.fff}] [发送] {message}{(string.IsNullOrEmpty(suffix) ? "" : $" (追加: {GetAppendName()})")}");

			if (chk_AutoClear.Checked)
			{
				txt_Send.Clear();
			}
		}
		catch (Exception ex)
		{
			AppendLog($"[{DateTime.Now:HH:mm:ss.fff}] [异常] {ex.Message}");
			MessageBox.Show($"发送失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
		}
	}

	/// <summary>
	/// 连接/断开按钮点击
	/// ?? 测试窗口不应该改变设备连接状态
	/// </summary>
	private void btn_Connect_Click(object sender, EventArgs e)
	{
		// ?? 移除此功能：测试窗口不应控制设备连接
   // 设备连接由 CommunicationFactory 和配置界面管理
        MessageBox.Show(
   "测试窗口不支持连接控制。\r\n请在配置界面或主界面管理设备连接。",
            "提示",
    MessageBoxButtons.OK,
        MessageBoxIcon.Information);
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
	/// 窗口关闭时取消订阅事件
	/// </summary>
	protected override void OnFormClosing(FormClosingEventArgs e)
	{
		base.OnFormClosing(e);

		try
		{
			// 取消订阅事件
			if (_device != null)
			{
				_device.MessageReceived -= OnMessageReceived;
				_device.ConnectionStatusChanged -= OnConnectionStatusChanged;
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine($"关闭测试窗口异常: {ex.Message}");
		}
	}
}
