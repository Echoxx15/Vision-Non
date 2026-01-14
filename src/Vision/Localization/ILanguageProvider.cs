using System.Collections.Generic;

namespace Vision.Localization;

/// <summary>
/// 语言提供者接口，可扩展支持不同的语言资源格式（JSON、YAML、数据库等）
/// </summary>
public interface ILanguageProvider
{
    /// <summary>
    /// 加载指定语言的所有翻译
    /// </summary>
    /// <param name="languageCode">语言代码，如 zh-CN, en-US</param>
    /// <returns>键值对字典</returns>
    Dictionary<string, string> Load(string languageCode);

    /// <summary>
    /// 保存翻译到指定语言
    /// </summary>
    /// <param name="languageCode">语言代码</param>
    /// <param name="translations">翻译字典</param>
    void Save(string languageCode, Dictionary<string, string> translations);

    /// <summary>
    /// 获取所有可用的语言代码
    /// </summary>
    IEnumerable<string> GetAvailableLanguages();
}
