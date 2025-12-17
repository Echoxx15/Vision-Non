using System;
using System.Windows.Forms;
using HardwareCommNet;


namespace ModbusControlNet.UI;

public partial class uConfigControl : UserControl
{
	private IComm _device;
	private bool _isLoading = false; // 防止加载配置时触发保存

	public uConfigControl()
	{
		InitializeComponent();
		cmbDataFormat.SelectedIndex = 1; // 默认选择CDAB
		
		// 隐藏保存按钮（修改即生效）
		btn_Save.Visible = false;
		
		// 订阅控件值变更事件
		txtIpAddress.TextChanged += ConfigValue_Changed;
		numPort.ValueChanged += ConfigValue_Changed;
		numStation.ValueChanged += ConfigValue_Changed;
		chkStringReverse.CheckedChanged += ConfigValue_Changed;
		cmbDataFormat.SelectedIndexChanged += ConfigValue_Changed;
		numConnectTimeout.ValueChanged += ConfigValue_Changed;
		numReceiveTimeout.ValueChanged += ConfigValue_Changed;
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
			_isLoading = true; // 开始加载，禁止触发保存
			
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
				
				// 加载字符串反转属性
				if (bool.TryParse(config.GetParameter("StringReverse", "false"), out var stringReverse))
					chkStringReverse.Checked = stringReverse;

				chkEnabled.Checked = true; // 默认启用
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine($"加载配置失败: {ex.Message}");
		}
		finally
		{
			_isLoading = false; // 加载完成，允许触发保存
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
			if (_device is ModbusTcp modbus)
			{
				// ⚠️ 只有在未连接状态下才能修改配置
				if (_device.IsConnected)
				{
					MessageBox.Show("设备已连接，无法修改配置。请先断开连接。", "提示",
						MessageBoxButtons.OK, MessageBoxIcon.Warning);
					return;
				}
				
				var config = new CommConfig(modbus.Name, "ModbusTcp");
				config.SetParameter("IpAddress", txtIpAddress.Text);
				config.SetParameter("Port", numPort.Value.ToString());
				config.SetParameter("Station", numStation.Value.ToString());
				config.SetParameter("StringReverse", chkStringReverse.Checked.ToString());
				
				modbus.ApplyConfig(config);

				// 保存到 CommunicationFactory
				CommunicationFactory.Instance.SaveConfigs();
				
				MessageBox.Show("配置已保存", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
		}
		catch (Exception ex)
		{
			MessageBox.Show($"保存配置失败: {ex.Message}", "错误",
				MessageBoxButtons.OK, MessageBoxIcon.Error);
		}
	}

	/// <summary>
	/// 保存按钮点击事件
	/// </summary>
	private void btn_Save_Click(object sender, EventArgs e)
	{
		SaveConfigToDevice();
	}

	/// <summary>
	/// 配置值变更事件 - 修改即生效
	/// </summary>
	private void ConfigValue_Changed(object sender, EventArgs e)
	{
		// 加载配置时不触发保存
		if (_isLoading) return;
		
		// 设备未绑定时不处理
		if (_device == null) return;
		
		// 设备已连接时不允许修改（控件应该已被禁用，这里做双重保护）
		if (_device.IsConnected) return;
		
		// 自动保存配置
		SaveConfigToDeviceSilent();
	}

	/// <summary>
	/// 静默保存配置（不弹出提示框）
	/// </summary>
	private void SaveConfigToDeviceSilent()
	{
		if (_device == null) return;

		try
		{
			if (_device is ModbusTcp modbus)
			{
				var config = new CommConfig(modbus.Name, "ModbusTcp");
				config.SetParameter("IpAddress", txtIpAddress.Text);
				config.SetParameter("Port", numPort.Value.ToString());
				config.SetParameter("Station", numStation.Value.ToString());
				config.SetParameter("StringReverse", chkStringReverse.Checked.ToString());
				
				modbus.ApplyConfig(config);

				// 保存到 CommunicationFactory
				CommunicationFactory.Instance.SaveConfigs();
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine($"自动保存配置失败: {ex.Message}");
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
	/// 更新连接状态相关的 UI
	/// </summary>
	private void UpdateConnectButtonState()
	{
		if (_device != null)
		{
			// ✅ 连接时禁用配置编辑
			txtIpAddress.Enabled = !_device.IsConnected;
			numPort.Enabled = !_device.IsConnected;
			numStation.Enabled = !_device.IsConnected;
			chkStringReverse.Enabled = !_device.IsConnected;
		}
	}

	/// <summary>
	/// 尝试连接设备
	/// </summary>
	/// <returns>连接是否成功</returns>
	private bool TryConnectDevice()
	{
		if (_device == null)
		{
			MessageBox.Show("设备未绑定", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
			return false;
		}

		if (_device.IsConnected)
		{
			return true;
		}

		try
		{
			// 连接前保存配置
			SaveConfigToDevice();
			
			// 尝试连接
			_device.Connect();
			
			if (_device.IsConnected)
			{
				MessageBox.Show("连接成功", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
				return true;
			}
			else
			{
				MessageBox.Show("连接失败，请检查配置", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return false;
			}
		}
		catch (Exception ex)
		{
			MessageBox.Show($"连接失败: {ex.Message}", "错误",
				MessageBoxButtons.OK, MessageBoxIcon.Error);
			return false;
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
				if (!TryConnectDevice())
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
		
		if (bool.TryParse(config.GetParameter("StringReverse", "false"), out var stringReverse))
			chkStringReverse.Checked = stringReverse;

		chkEnabled.Checked = true;
	}
}
