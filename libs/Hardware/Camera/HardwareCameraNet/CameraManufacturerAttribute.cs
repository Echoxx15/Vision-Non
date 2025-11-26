using System;

namespace HardwareCameraNet;

/// <summary>
/// 品牌标识特性：用于标记相机插件支持的品牌名称
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class CameraManufacturerAttribute(string manufacturerName) : Attribute
{
    /// <summary>
    /// 厂商名称（如"海康"、"大华"）
    /// </summary>
    public string ManufacturerName { get; } = manufacturerName;
}