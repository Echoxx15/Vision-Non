using System;
using System.Windows.Forms;
using HardwareCommNet;
using HardwareCommNet.UI; // for Frm_CommTable

namespace TcpControlNet.UI;

public partial class uConfigControl : UserControl
{
	private object _device;
	private TcpType _type;
	private bool _isLoading;

	public uConfigControl()
	{
		InitializeComponent();
		InitializeEncodingComboBox();
	}

	/// <summary>
	/// 初始化编码下拉框
	/// </summary>
	private void InitializeEncodingComboBox()
	{
		cmb_Encoding.Items.Clear();
		cmb_Encoding.Items.AddRange(["UTF-8", "GBK", "GB2312", "ASCII", "Unicode"]);
		cmb_Encoding.SelectedIndex = 0;
	}

	/// <summary>
	/// 设置要配置的设备
	/// </summary>
	public void SetDevice(object device, TcpType type)
	{
		// 取消订阅旧设备的事件
		UnsubscribeDeviceEvents();

		_device = device;
		_type = type;
		LoadConfigFromDevice();
		UpdateUIForDeviceType();

		// 订阅连接状态变化事件和原始消息接收事件（用于UI显示）
		if (_device is IComm comm)
		{
			comm.ConnectionStatusChanged += OnConnectionStatusChanged;
			UpdateConnectButtonState();
		}

		// 订阅原始消息事件（用于UI接收框显示）
		if (_device is TcpClientAdapter clientAdapter)
		{
			clientAdapter.RawMessageReceived += OnRawMessageReceived;
		}
		else if (_device is TcpServerAdapter serverAdapter)
		{
			serverAdapter.RawMessageReceived += OnRawMessageReceived;
		}
	}

	/// <summary>
	/// 取消订阅设备事件
	/// </summary>
	private void UnsubscribeDeviceEvents()
	{
		if (_device is IComm oldComm)
		{
			try { oldComm.ConnectionStatusChanged -= OnConnectionStatusChanged; } catch { }
		}

		// 取消订阅原始消息事件
		if (_device is TcpClientAdapter clientAdapter)
		{
			try { clientAdapter.RawMessageReceived -= OnRawMessageReceived; } catch { }
		}
		else if (_device is TcpServerAdapter serverAdapter)
		{
			try { serverAdapter.RawMessageReceived -= OnRawMessageReceived; } catch { }
		}
	}

	/// <summary>
	/// 连接状态变化事件处理
	/// </summary>
	private void OnConnectionStatusChanged(object sender, bool isConnected)
	{
		if (InvokeRequired)
		{
			BeginInvoke(new Action(() => OnConnectionStatusChanged(sender, isConnected)));
			return;
		}

		UpdateConnectButtonState();
	}

	/// <summary>
	/// 原始消息接收事件处理（用于UI显示）
	/// </summary>
	private void OnRawMessageReceived(object sender, string message)
	{
		if (InvokeRequired)
		{
			try { BeginInvoke(new Action<object, string>(OnRawMessageReceived), sender, message); } catch { }
			return;
		}

		AppendReceiveText($"[{DateTime.Now:HH:mm:ss.fff}] {message}");
	}

	/// <summary>
	/// 追加接收文本
	/// </summary>
	private void AppendReceiveText(string text)
	{
		if (txt_Receive == null || txt_Receive.IsDisposed) return;

		txt_Receive.AppendText(text + Environment.NewLine);
		txt_Receive.ScrollToCaret();
	}

	/// <summary>
	/// 更新连接状态相关的 UI
	/// </summary>
	private void UpdateConnectButtonState()
	{
		if (_device is IComm comm)
		{
			// ✅ 连接时禁用配置编辑
			txt_IpAddress.Enabled = !comm.IsConnected;
			num_Port.Enabled = !comm.IsConnected;
			cmb_Encoding.Enabled = !comm.IsConnected;
			chk_UseTerminator.Enabled = !comm.IsConnected;
			txt_Terminator.Enabled = !comm.IsConnected;

			// 根据类型禁用特定控件
			if (_type == TcpType.Client)
			{
				chk_AutoReconnect.Enabled = !comm.IsConnected;
				num_ReconnectInterval.Enabled = !comm.IsConnected;
				num_ConnectTimeout.Enabled = !comm.IsConnected;
			}
			else if (_type == TcpType.Server)
			{
				num_MaxConnections.Enabled = !comm.IsConnected;
			}
		}
	}

	/// <summary>
	/// 根据设备类型更新 UI 控件的可见性
	/// </summary>
	private void UpdateUIForDeviceType()
	{
		if (_type == TcpType.Client)
		{
			lbl_Type.Text = "TCP客户端";

			// 客户端特有控件可见
			chk_AutoReconnect.Visible = true;
			num_ReconnectInterval.Visible = true;
			label10.Visible = true;

			// 服务器特有控件隐藏
			num_MaxConnections.Visible = false;
			label11.Visible = false;

			// 客户端可以设置服务器IP
			txt_IpAddress.Text = "127.0.0.1";
		}
		else if (_type == TcpType.Server)
		{
			lbl_Type.Text = "TCP服务器";

			// 服务器特有控件可见
			num_MaxConnections.Visible = true;
			label11.Visible = true;

			// 客户端特有控件隐藏
			chk_AutoReconnect.Visible = false;
			num_ReconnectInterval.Visible = false;
			label10.Visible = false;

			// 服务器监听所有IP
			txt_IpAddress.Text = "0.0.0.0";
		}
	}

	/// <summary>
	/// 从设备加载配置到界面
	/// </summary>
	private void LoadConfigFromDevice()
	{
		if (_device == null) return;

		try
		{
			_isLoading = true;

			if (_type == TcpType.Client && _device is TcpClientAdapter client)
			{
				txt_Name.Text = client.Name;
				txt_IpAddress.Text = client.IpAddress;
				num_Port.Value = client.Port;
				cmb_Encoding.Text = client.EncodingName;
				chk_AutoReconnect.Checked = client.AutoReconnect;
				num_ReconnectInterval.Value = client.ReconnectDelayMs;
				num_ConnectTimeout.Value = client.ConnectTimeoutMs;
				chk_UseTerminator.Checked = client.FrameByNewLine;
				txt_Terminator.Text = "\\r\\n"; // 默认
				chk_Enabled.Checked = true;

				// 这些是新增字段，可能需要在 Adapter 中添加
				num_SendBufferSize.Value = 0;
				num_ReceiveBufferSize.Value = 0;
				txt_Remark.Text = "";
			}
			else if (_type == TcpType.Server && _device is TcpServerAdapter server)
			{
				txt_Name.Text = server.Name;
				txt_IpAddress.Text = server.IpAddress;
				num_Port.Value = server.Port;
				cmb_Encoding.Text = server.EncodingName;
				chk_UseTerminator.Checked = server.FrameByNewLine;
				txt_Terminator.Text = "\\r\\n";
				num_MaxConnections.Value = server.MaxConnections;
				chk_Enabled.Checked = true;

				// 这些是新增字段
				num_SendBufferSize.Value = 0;
				num_ReceiveBufferSize.Value = 0;
				num_ConnectTimeout.Value = 5000;
				txt_Remark.Text = "";
			}
		}
		catch (Exception ex)
		{
			MessageBox.Show($"加载配置失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
		}
		finally
		{
			_isLoading = false;
		}
	}

	/// <summary>
	/// 保存界面配置到设备（实时应用）
	/// </summary>
	public void SaveConfigToDevice()
	{
		if (_device == null || _isLoading) return;

		try
		{
			if (_type == TcpType.Client && _device is TcpClientAdapter client)
			{
				// 如果设备已连接，不允许修改连接参数
				if (client.IsConnected)
				{
					MessageBox.Show("设备已连接，无法修改连接参数。请先断开连接。", "提示",
						MessageBoxButtons.OK, MessageBoxIcon.Warning);
					return;
				}

				client.IpAddress = txt_IpAddress.Text.Trim();
				client.Port = (int)num_Port.Value;
				client.EncodingName = cmb_Encoding.Text;
				client.AutoReconnect = chk_AutoReconnect.Checked;
				client.ReconnectDelayMs = (int)num_ReconnectInterval.Value;
				client.ConnectTimeoutMs = (int)num_ConnectTimeout.Value;
				client.FrameByNewLine = chk_UseTerminator.Checked;
			}
			else if (_type == TcpType.Server && _device is TcpServerAdapter server)
			{
				// 如果设备已连接，不允许修改连接参数
				if (server.IsConnected)
				{
					MessageBox.Show("设备已启动，无法修改连接参数。请先停止服务。", "提示",
						MessageBoxButtons.OK, MessageBoxIcon.Warning);
					return;
				}

				server.IpAddress = txt_IpAddress.Text.Trim();
				server.Port = (int)num_Port.Value;
				server.EncodingName = cmb_Encoding.Text;
				server.FrameByNewLine = chk_UseTerminator.Checked;
				server.MaxConnections = (int)num_MaxConnections.Value;
			}

			// 保存到 CommunicationFactory 配置文件
			CommunicationFactory.Instance.SaveConfigs();

			MessageBox.Show("配置已保存", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
		}
		catch (Exception ex)
		{
			MessageBox.Show($"保存配置失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
		}
	}

	/// <summary>
	/// 保存配置到设备（静默保存，不显示提示）
	/// </summary>
	private void SaveConfigToDeviceSilent()
	{
		if (_device == null || _isLoading) return;

		try
		{
			if (_type == TcpType.Client && _device is TcpClientAdapter client)
			{
				client.IpAddress = txt_IpAddress.Text.Trim();
				client.Port = (int)num_Port.Value;
				client.EncodingName = cmb_Encoding.Text;
				client.AutoReconnect = chk_AutoReconnect.Checked;
				client.ReconnectDelayMs = (int)num_ReconnectInterval.Value;
				client.ConnectTimeoutMs = (int)num_ConnectTimeout.Value;
				client.FrameByNewLine = chk_UseTerminator.Checked;
			}
			else if (_type == TcpType.Server && _device is TcpServerAdapter server)
			{
				server.IpAddress = txt_IpAddress.Text.Trim();
				server.Port = (int)num_Port.Value;
				server.EncodingName = cmb_Encoding.Text;
				server.FrameByNewLine = chk_UseTerminator.Checked;
				server.MaxConnections = (int)num_MaxConnections.Value;
			}

			// 保存到 CommunicationFactory 配置文件
			CommunicationFactory.Instance.SaveConfigs();
		}
		catch
		{
			// 静默保存失败不提示
		}
	}

	#region 收发测试功能

	/// <summary>
	/// 发送按钮点击
	/// </summary>
	private void btn_Send_Click(object sender, EventArgs e)
	{
		if (_device == null)
		{
			MessageBox.Show("设备未初始化", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
			return;
		}

		if (!(_device as IComm).IsConnected)
		{
			MessageBox.Show("设备未连接，请先连接设备", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
			return;
		}

		var message = txt_Send.Text;
		if (string.IsNullOrWhiteSpace(message))
		{
			MessageBox.Show("请输入要发送的内容", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
			return;
		}

		try
		{
			// 发送数据（TCP不需要地址，传null），自动追加结束符
			(_device as IComm).Write(null, message + "\r\n");
			// 不打印发送内容，只依赖接收方的回复来确认发送成功
		}
		catch (Exception ex)
		{
			MessageBox.Show($"发送失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
		}
	}

	/// <summary>
	/// 清空接收区
	/// </summary>
	private void btn_ClearReceive_Click(object sender, EventArgs e)
	{
		txt_Receive?.Clear();
	}

	#endregion

	/// <summary>
	/// 保存按钮点击事件
	/// </summary>
	private void btn_Save_Click(object sender, EventArgs e)
	{
		SaveConfigToDevice();
	}


	/// <summary>
	/// 应用配置到设备（不保存到文件）
	/// </summary>
	private void ApplyConfigToDevice()
	{
		if (_device == null || _isLoading) return;

		try
		{
			if (_type == TcpType.Client && _device is TcpClientAdapter client)
			{
				client.IpAddress = txt_IpAddress.Text.Trim();
				client.Port = (int)num_Port.Value;
				client.EncodingName = cmb_Encoding.Text;
				client.AutoReconnect = chk_AutoReconnect.Checked;
				client.ReconnectDelayMs = (int)num_ReconnectInterval.Value;
				client.ConnectTimeoutMs = (int)num_ConnectTimeout.Value;
				client.FrameByNewLine = chk_UseTerminator.Checked;
			}
			else if (_type == TcpType.Server && _device is TcpServerAdapter server)
			{
				server.IpAddress = txt_IpAddress.Text.Trim();
				server.Port = (int)num_Port.Value;
				server.EncodingName = cmb_Encoding.Text;
				server.FrameByNewLine = chk_UseTerminator.Checked;
				server.MaxConnections = (int)num_MaxConnections.Value;
			}
		}
		catch
		{
			// 应用配置失败不提示
		}
	}

}

public enum TcpType
{
    Client,
    Server
}
