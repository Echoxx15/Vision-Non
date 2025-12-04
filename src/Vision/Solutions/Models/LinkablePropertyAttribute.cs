using System;

namespace Vision.Solutions.Models;

/// <summary>
/// 标记工位配置中可被输入变量链接的属性。
/// 被此特性标记的属性将出现在输入映射界面的"工位属性变量"节点中。
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public class LinkablePropertyAttribute : Attribute
{
    /// <summary>
    /// 在界面上显示的名称（若为空则使用属性名）
    /// </summary>
    public string DisplayName { get; }
    
    /// <summary>
    /// 属性描述
    /// </summary>
    public string Description { get; }

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="displayName">显示名称</param>
    /// <param name="description">描述</param>
    public LinkablePropertyAttribute(string displayName = null, string description = null)
    {
        DisplayName = displayName;
        Description = description;
    }
}
