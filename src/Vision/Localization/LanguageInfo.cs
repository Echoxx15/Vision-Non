namespace Vision.Localization;

/// <summary>
/// 支持的语言信息
/// </summary>
public class LanguageInfo
{
    /// <summary>
    /// 语言代码 (如 zh-CN, en-US)
    /// </summary>
    public string Code { get; set; }

    /// <summary>
    /// 语言的本地名称 (如 简体中文, English)
    /// </summary>
    public string NativeName { get; set; }

    /// <summary>
    /// 语言的英文名称 (如 Chinese Simplified, English)
    /// </summary>
    public string EnglishName { get; set; }

    /// <summary>
    /// 语言图标（可选，用于显示国旗等）
    /// </summary>
    public string IconPath { get; set; }

    public LanguageInfo() { }

    public LanguageInfo(string code, string nativeName, string englishName, string iconPath = null)
    {
        Code = code;
        NativeName = nativeName;
        EnglishName = englishName;
        IconPath = iconPath;
    }

    public override string ToString() => NativeName;
}
