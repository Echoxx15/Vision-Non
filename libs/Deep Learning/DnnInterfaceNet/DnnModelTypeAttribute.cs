using System;

namespace DnnInterfaceNet;

/// <summary>
/// 深度学习模型类型特性 - 标记插件的模型类型
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public class DnnModelTypeAttribute : Attribute
{
    /// <summary>
    /// 模型类型名称（如"语义分割"、"深度OCR"）
    /// </summary>
    public string TypeName { get; }

    /// <summary>
    /// 模型类型描述
    /// </summary>
    public string Description { get; }

    /// <summary>
    /// 图标资源路径（可选）
    /// </summary>
    public string IconPath { get; }

    public DnnModelTypeAttribute(string typeName, string description = null, string iconPath = null)
    {
        TypeName = typeName;
        Description = description ?? typeName;
        IconPath = iconPath;
    }
}
