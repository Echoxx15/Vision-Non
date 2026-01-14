using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace Vision.Localization;

/// <summary>
/// 基于 JSON 文件的语言提供者
/// 语言文件存放在 Languages 目录下，文件名格式: {languageCode}.json
/// </summary>
public class JsonLanguageProvider : ILanguageProvider
{
    private readonly string _languagesFolder;

    public JsonLanguageProvider() : this(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Languages"))
    {
    }

    public JsonLanguageProvider(string languagesFolder)
    {
        _languagesFolder = languagesFolder;
        if (!Directory.Exists(_languagesFolder))
        {
            Directory.CreateDirectory(_languagesFolder);
        }
    }

    public Dictionary<string, string> Load(string languageCode)
    {
        var filePath = GetFilePath(languageCode);
        if (!File.Exists(filePath))
        {
            return new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        }

        try
        {
            var json = File.ReadAllText(filePath);
            var dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            // 使用忽略大小写的字典
            var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            if (dict != null)
            {
                foreach (var kvp in dict)
                {
                    result[kvp.Key] = kvp.Value;
                }
            }
            return result;
        }
        catch
        {
            return new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        }
    }

    public void Save(string languageCode, Dictionary<string, string> translations)
    {
        var filePath = GetFilePath(languageCode);
        var json = JsonConvert.SerializeObject(translations, Formatting.Indented);
        File.WriteAllText(filePath, json);
    }

    public IEnumerable<string> GetAvailableLanguages()
    {
        if (!Directory.Exists(_languagesFolder))
        {
            return Enumerable.Empty<string>();
        }

        return Directory.GetFiles(_languagesFolder, "*.json")
            .Select(f => Path.GetFileNameWithoutExtension(f))
            .Where(name => !string.IsNullOrWhiteSpace(name));
    }

    private string GetFilePath(string languageCode)
    {
        return Path.Combine(_languagesFolder, $"{languageCode}.json");
    }
}
