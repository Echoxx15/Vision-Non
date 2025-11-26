using System;
using System.Windows.Forms;
using HardwareCommNet;
using HardwareCommNet.UI; // Frm_CommTable

namespace ModbusControlNet.UI;

public partial class uConfigControl : UserControl
{
	private IComm _device;

	public uConfigControl()
	{
		InitializeComponent();
		cmbDataFormat.SelectedIndex = 1; // 默认选择CDAB
		btn_OpenTable.Click += Btn_OpenTable_Click;
	}

	private void Btn_OpenTable_Click(object sender, EventArgs e)
	{
		if (_device == null)
		{
			MessageBox.Show("设备未绑定", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
			return;
		}
		try
		{
			using var frm = new Frm_CommTable(_device);
			frm.ShowDialog(this);
		}
		catch (Exception ex)
		{
			MessageBox.Show($"打开通讯表失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
		}
	}

	/// <summary>
	/// 设置绑定的设备实例
	/// </summary>
	public void SetDevice(IComm device)
	{
		// ✅ 解除旧设备的事件订阅
		if (_device != null)
		{
			_device.ConnectionStatusChanged -= Device_ConnectionStatusChanged;
		}

		_device = device;

		if (_device != null)
		{
			// 显示设备名称
			txt_Name.Text = _device.Name;

			// ✅ 订阅连接状态变化事件（仅用于更新UI）
			_device.ConnectionStatusChanged += Device_ConnectionStatusChanged;

			// 更新按钮状态
			UpdateConnectButtonState();

			// ✅ 从设备加载当前配置（如果设备支持）
			LoadConfigFromDevice();
		}
	}

	/// <summary>
	/// 从设备加载配置参数
	/// </summary>
	private void LoadConfigFromDevice()
	{
		if (_device == null) return;

		try
		{
			if (_device is ModbusTcp modbus)
			{
				// 通过反射或公开属性获取配置
				// 这里假设 ModbusTcp 提供了配置访问接口
				var config = modbus.GetConfig();
				
				txtIpAddress.Text = config.GetParameter("IpAddress", "127.0.0.1");
				
				if (int.TryParse(config.GetParameter("Port", "502"), out var port))
					numPort.Value = port;
				
				if (byte.TryParse(config.GetParameter("Station", "1"), out var station))
					numStation.Value = station;

				chkEnabled.Checked = true; // 默认启用
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine($"加载配置失败: {ex.Message}");
		}
	}

	/// <summary>
	/// 保存配置到设备
	/// </summary>
	private void SaveConfigToDevice()
	{
		if (_device == null) return;

		try
		{
			if (_device is ModbusTcp modbus && !_device.IsConnected)
			{
				// ⚠️ 只有在未连接状态下才能修改配置
				var config = new CommConfig(modbus.Name, "ModbusTcp");
				config.SetParameter("IpAddress", txtIpAddress.Text);
				config.SetParameter("Port", numPort.Value.ToString());
				config.SetParameter("Station", numStation.Value.ToString());
				
				modbus.ApplyConfig(config);

				// 保存到 CommunicationFactory
				CommunicationFactory.Instance.SaveConfigs();
			}
			else if (_device.IsConnected)
			{
				MessageBox.Show("设备已连接，无法修改配置。请先断开连接。", "提示",
					MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}
		}
		catch (Exception ex)
		{
			MessageBox.Show($"保存配置失败: {ex.Message}", "错误",
				MessageBoxButtons.OK, MessageBoxIcon.Error);
		}
	}

	/// <summary>
	/// 设备连接状态变化事件处理
	/// ✅ 仅用于更新UI，不改变设备状态
	/// </summary>
	private void Device_ConnectionStatusChanged(object sender, bool isConnected)
	{
		if (this.InvokeRequired)
		{
			this.BeginInvoke(new Action(() => Device_ConnectionStatusChanged(sender, isConnected)));
			return;
		}

		UpdateConnectButtonState();
	}

	/// <summary>
	/// 更新连接按钮状态
	/// </summary>
	private void UpdateConnectButtonState()
	{
		if (_device != null)
		{
			btnConnect.Text = _device.IsConnected ? "断开" : "连接";
			btnConnect.BackColor =
				_device.IsConnected ? System.Drawing.Color.LightGreen : System.Drawing.Color.LightCoral;
			
			// ✅ 连接时禁用配置编辑
			txtIpAddress.Enabled = !_device.IsConnected;
			numPort.Enabled = !_device.IsConnected;
			numStation.Enabled = !_device.IsConnected;
		}
	}

	/// <summary>
	/// 连接按钮点击事件
	/// ✅ 用户显式操作：连接/断开
	/// </summary>
	private void btnConnect_Click(object sender, EventArgs e)
	{
		if (_device == null)
		{
			MessageBox.Show("设备未绑定", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
			return;
		}

		try
		{
			if (_device.IsConnected)
			{
				// ✅ 用户显式点击断开按钮
				_device.Disconnect();
				MessageBox.Show("断开成功", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
			else
			{
				// ✅ 连接前保存配置
				SaveConfigToDevice();
				
				// ✅ 用户显式点击连接按钮
				_device.Connect();
				
				if (_device.IsConnected)
				{
					MessageBox.Show("连接成功", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
				}
				else
				{
					MessageBox.Show("连接失败，请检查配置", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}
		}
		catch (Exception ex)
		{
			MessageBox.Show($"操作失败: {ex.Message}", "错误",
				MessageBoxButtons.OK, MessageBoxIcon.Error);
		}
	}

	/// <summary>
	/// 测试按钮点击事件
	/// ✅ 仅使用设备，不改变设备状态
	/// </summary>
	private void btn_Test_Click(object sender, EventArgs e)
	{
		if (_device == null)
		{
			MessageBox.Show("设备未绑定", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
			return;
		}

		if (!_device.IsConnected)
		{
			var result = MessageBox.Show(
				"设备未连接，是否先连接？", 
				"提示", 
				MessageBoxButtons.YesNo, 
				MessageBoxIcon.Question);
			
			if (result == DialogResult.Yes)
			{
				btnConnect_Click(sender, e);
				
				if (!_device.IsConnected)
				{
					return; // 连接失败，不打开测试窗口
				}
			}
			else
			{
				return;
			}
		}

		try
		{
			// ✅ 使用 FormModbusTest（在同一个项目中，更可靠）
			using var frm = new HslCommunicationDemo.FormModbus(_device);
			frm.ShowDialog();
		}
		catch (Exception ex)
		{
			MessageBox.Show($"打开测试界面失败: {ex.Message}", "错误",
				MessageBoxButtons.OK, MessageBoxIcon.Error);
		}
	}

	/// <summary>
	/// 获取配置参数
	/// </summary>
	public void GetConfigFromUI()
	{
		// 暂不实现，配置由 CommConfig 管理
	}

	/// <summary>
	/// 设置配置参数
	/// </summary>
	public void SetConfigToUI(CommConfig config)
	{
		if (config == null) return;

		txtIpAddress.Text = config.GetParameter("IpAddress", "127.0.0.1");
		
		if (int.TryParse(config.GetParameter("Port", "502"), out var port))
			numPort.Value = port;
		
		if (byte.TryParse(config.GetParameter("Station", "1"), out var station))
			numStation.Value = station;

		chkEnabled.Checked = true;
	}
}