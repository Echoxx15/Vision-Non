using System;

namespace HardwareCommNet;

/// <summary>
/// 通讯制造商标识特性，用于标识通讯插件支持的制造商名称。
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class CommManufacturerAttribute : Attribute
{
	/// <summary>
	/// 通讯类型名称，如"ModbusTcp"、"TcpClient"。
	/// </summary>
	public string ManufacturerName { get; }

	public CommManufacturerAttribute(string manufacturerName)
	{
		ManufacturerName = manufacturerName;
	}
}
