using System;
using System.Windows.Forms;

namespace HardwareCommNet;

public interface IComm : IDisposable
{
	/// <summary>
	/// 消息回调事件，当接收到消息时触发
	/// </summary>
	event EventHandler<object> MessageReceived;

	/// <summary>
	/// 设备连接状态变化事件
	/// </summary>
	event EventHandler<bool> ConnectionStatusChanged;

	/// <summary>
	/// 设备名称
	/// </summary>
	string Name { get; }

	/// <summary>
	/// 是否已连接
	/// </summary>
	bool IsConnected { get; }

	/// <summary>
	/// 设备专属通讯表（输入/输出定义 + 可编辑触发值）。
	/// </summary>
	CommTable.CommTable Table { get; }

	UserControl GetConfigControl();

	/// <summary>
	/// 连接设备
	/// </summary>
	void Connect();

	/// <summary>
	/// 断开连接
	/// </summary>
	void Disconnect();

	/// <summary>
	/// 写入数据到指定地址
	/// </summary>
	/// <param name="address">地址（Modbus寄存器地址、PLC地址等）。TCP通讯可传入null或空字符串</param>
	/// <param name="data">要写入的数据。不能为null</param>
	void Write(string address, object data);

	/// <summary>
	/// 写入数组数据到指定地址
	/// </summary>
	/// <param name="address">地址（Modbus寄存器地址、PLC地址等）。TCP通讯可传入null或空字符串</param>
	/// <param name="data">要写入的数据数组。不能为null</param>
	void Write(string address, object[] data);
}

