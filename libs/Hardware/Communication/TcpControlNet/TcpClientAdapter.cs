using System;
using System.Text;
using System.Windows.Forms;
using System.Linq;
using HardwareCommNet;
using TcpControlNet.UI;
using Logger;
using HardwareCommNet.CommTable;

namespace TcpControlNet;

/// <summary>
/// TCP 客户端通讯适配器（IComm 实现）
/// </summary>
[CommManufacturer("TcpClient")]
public class TcpClientAdapter : CommAdapterBase
{
	#region 属性

	private NetTcpClient _client;
	public CommTable Table { get; } = new CommTable();

	// 配置参数
	public string IpAddress { get; set; } = "127.0.0.1";
	public int Port { get; set; } = 5000;
	public string EncodingName { get; set; } = "UTF-8";
	public bool FrameByNewLine { get; set; } = true;
	public bool AutoReconnect { get; set; } = true;
	public int ReconnectDelayMs { get; set; } = 3000;
	public int ConnectTimeoutMs { get; set; } = 5000;

	/// <summary>
	/// 是否已连接（只读属性）
	/// </summary>
	public override bool IsConnected => _client?.IsConnected ?? false;

	#endregion

	#region 私有字段

	private uConfigControl _configControl;

	#endregion

	#region 构造函数

	/// <summary>
	/// 初始化 TCP 客户端适配器
	/// </summary>
	public TcpClientAdapter(string name = "TcpClient") : base(name)
	{
	}

	#endregion

	#region 重写接口方法

	/// <summary>
	/// 获取配置控件
	/// </summary>
	public override UserControl GetConfigControl()
	{
		if (_configControl == null || _configControl.IsDisposed)
		{
			_configControl = new uConfigControl();
			_configControl.SetDevice(this, TcpType.Client);
		}

		return _configControl;
	}

	/// <summary>
	/// 连接设备
	/// </summary>
	public override void Connect()
	{
		if (IsConnected) return;

		try
		{
			LogHelper.Info($"[{Name}] ========== TCP客户端开始连接 ==========");
			LogHelper.Info($"[{Name}] 目标地址: {IpAddress}:{Port}");

			// 获取编码
			Encoding encoding;
			try
			{
				encoding = Encoding.GetEncoding(EncodingName);
			}
			catch
			{
				encoding = Encoding.UTF8;
			}

			// 创建底层客户端（传入 Name 参数）
			_client = new NetTcpClient(IpAddress, Port, encoding, Name)
			{
				AutoReconnect = AutoReconnect,
				ReconnectDelayMs = ReconnectDelayMs,
				ConnectTimeoutMs = ConnectTimeoutMs,
				FrameByNewLine = FrameByNewLine
			};

			// ? 订阅底层客户端事件（使用基类方法触发）
			_client.ConnectionStatusChanged += OnClientConnectionStatusChanged;
			_client.MessageReceived += OnClientMessageReceived;

			LogHelper.Info($"[{Name}] ? 已订阅事件");

			// ?? 重要：显式启动连接
			_client.Start();

			LogHelper.Info($"[{Name}] TCP客户端开始连接: {IpAddress}:{Port}");
		}
		catch (Exception ex)
		{
			LogHelper.Error(ex, $"[{Name}] TCP客户端创建失败");
			OnConnectionStatusChanged(false);
		}
	}

	/// <summary>
	/// 断开连接
	/// </summary>
	public override void Disconnect()
	{
		if (!IsConnected) return;

		try
		{
			if (_client != null)
			{
				// ? 取消订阅
				_client.ConnectionStatusChanged -= OnClientConnectionStatusChanged;
				_client.MessageReceived -= OnClientMessageReceived;

				_client.Dispose();
				_client = null;
			}

			OnConnectionStatusChanged(false);
			LogHelper.Info($"[{Name}] TCP客户端已断开连接");
		}
		catch (Exception ex)
		{
			LogHelper.Error(ex, $"[{Name}] TCP客户端断开失败");
		}
	}

	/// <summary>
	/// 写入数据
	/// </summary>
	public override void Write(string address, object data)
	{
		// ? 使用基类验证方法
		ValidateNotNull(data, nameof(data));

		if (!IsConnected || _client == null)
		{
			LogHelper.Warn($"[{Name}] TCP客户端未连接，无法发送数据");
			return;
		}

		try
		{
			string message = data.ToString();
			_client.Send(message);
			LogHelper.Info($"[{Name}] 发送消息: {message}");
		}
		catch (Exception ex)
		{
			LogHelper.Error(ex, $"[{Name}] TCP客户端发送失败");
		}
	}

	/// <summary>
	/// 写入数据数组
	/// </summary>
	public override void Write(string address, object[] data)
	{
		// ? 使用基类验证方法
		ValidateNotEmpty(data, nameof(data));

		// TCP不需要地址，将数组转换为字符串发送
		Write(address, string.Join("", data));
	}

	/// <summary>
	/// 获取当前配置
	/// </summary>
	public override CommConfig GetConfig()
	{
		var config = new CommConfig(Name, "TcpClient");
		config.SetParameter("IpAddress", IpAddress);
		config.SetParameter("Port", Port.ToString());
		config.SetParameter("Encoding", EncodingName);
		config.SetParameter("FrameByNewLine", FrameByNewLine.ToString());
		config.SetParameter("AutoReconnect", AutoReconnect.ToString());
		config.SetParameter("ReconnectDelayMs", ReconnectDelayMs.ToString());
		config.SetParameter("ConnectTimeoutMs", ConnectTimeoutMs.ToString());
		config.SetParameter("IsConnected", IsConnected.ToString());
		return config;
	}

	/// <summary>
	/// 应用配置
	/// </summary>
	public override void ApplyConfig(CommConfig config)
	{
		if (config == null) return;

		IpAddress = config.GetParameter("IpAddress", "127.0.0.1");

		if (int.TryParse(config.GetParameter("Port", "5000"), out var port))
			Port = port;

		EncodingName = config.GetParameter("Encoding", "UTF-8");

		if (bool.TryParse(config.GetParameter("FrameByNewLine", "true"), out var frameByNewLine))
			FrameByNewLine = frameByNewLine;

		if (bool.TryParse(config.GetParameter("AutoReconnect", "true"), out var autoReconnect))
			AutoReconnect = autoReconnect;

		if (int.TryParse(config.GetParameter("ReconnectDelayMs", "3000"), out var reconnectDelay))
			ReconnectDelayMs = reconnectDelay;

		if (int.TryParse(config.GetParameter("ConnectTimeoutMs", "5000"), out var connectTimeout))
			ConnectTimeoutMs = connectTimeout;

		// 恢复之前的连接状态（如果需要自动连接）
		if (bool.TryParse(config.GetParameter("IsConnected", "false"), out var wasConnected) && wasConnected)
		{
			try
			{
				Connect();
			}
			catch
			{
			}
		}
	}

	/// <summary>
	/// 释放资源
	/// </summary>
	protected override void Dispose(bool disposing)
	{
		if (disposing)
		{
			try
			{
				_client?.Dispose();
				_client = null;
				_configControl?.Dispose();
				_configControl = null;
			}
			catch
			{
			}
		}

		base.Dispose(disposing);
	}

	#endregion

	#region 事件处理（转发给基类）

	/// <summary>
	/// 底层客户端连接状态变化事件处理
	/// </summary>
	private void OnClientConnectionStatusChanged(object sender, bool connected)
	{
		LogHelper.Info($"[{Name}] ?? 底层连接状态变化: {connected}");
		OnConnectionStatusChanged(connected);
	}

	/// <summary>
	/// 底层客户端消息接收事件处理
	/// </summary>
	private void OnClientMessageReceived(object sender, object message)
	{
		LogHelper.Info($"?? [TcpClientAdapter] 客户端收到消息");
		LogHelper.Info($"   ├─ 适配器名称: {Name}");
		LogHelper.Info($"   ├─ 消息内容: {message}");
		LogHelper.Info($"   └─ 准备发布 MessageReceived 事件...");

		var firstInput = Table.Inputs.FirstOrDefault();
		var payload = new { Name = firstInput?.Name ?? "Message", Value = message?.ToString() };
		OnMessageReceived(payload);

		LogHelper.Info($"   ? 事件已发布");
	}

	#endregion
}
