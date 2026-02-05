using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Vision.Localization;

/// <summary>
/// 基于 JSON 文件的语言提供者
/// 读取 ui-translations.json 文件，格式：窗体名 -> 控件名 -> {zh-CN: 中文, en-US: 英文}
/// </summary>
public class JsonLanguageProvider : ILanguageProvider
{
    private readonly string _languagesFolder;
    private const string TranslationFileName = "ui-translations.json";

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
        var filePath = Path.Combine(_languagesFolder, TranslationFileName);
        if (!File.Exists(filePath))
        {
            return new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        }

        try
        {
            var json = File.ReadAllText(filePath, System.Text.Encoding.UTF8);
            var root = JObject.Parse(json);
            var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            // 遍历所有窗体/分组
            foreach (var formProp in root.Properties())
            {
                if (formProp.Name.StartsWith("_")) continue; // 跳过元数据

                var formName = formProp.Name;
                if (formProp.Value is JObject formObj)
                {
                    // 遍历窗体下的所有控件
                    foreach (var controlProp in formObj.Properties())
                    {
                        if (controlProp.Name.StartsWith("_")) continue; // 跳过元数据

                        var controlName = controlProp.Name;
                        if (controlProp.Value is JObject translations)
                        {
                            // 获取当前语言的翻译
                            var translation = translations[languageCode]?.ToString();
                            if (!string.IsNullOrEmpty(translation))
                            {
                                // 直接使用控件名作为键（如 btn_User）
                                // 如果有重复，后面的会覆盖前面的
                                result[controlName] = translation;
                            }
                        }
                    }
                }
            }

            return result;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"加载语言文件失败: {ex.Message}");
            return new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        }
    }

    public void Save(string languageCode, Dictionary<string, string> translations)
    {
        // ui-translations.json 格式不支持单独保存某个语言
        // 如需要保存，需要读取整个文件，修改对应语言，再写回
        throw new NotSupportedException("ui-translations.json格式不支持单独保存语言");
    }

    public IEnumerable<string> GetAvailableLanguages()
    {
        var filePath = Path.Combine(_languagesFolder, TranslationFileName);
        if (!File.Exists(filePath))
        {
            return new[] { "zh-CN" }; // 默认返回中文
        }

        try
        {
            var json = File.ReadAllText(filePath, System.Text.Encoding.UTF8);
            var root = JObject.Parse(json);
            
            // 从第一个非元数据项中获取支持的语言列表
            foreach (var formProp in root.Properties())
            {
                if (formProp.Name.StartsWith("_")) continue;
                
                if (formProp.Value is JObject formObj)
                {
                    foreach (var controlProp in formObj.Properties())
                    {
                        if (controlProp.Name.StartsWith("_")) continue;
                        
                        if (controlProp.Value is JObject translations)
                        {
                            return translations.Properties().Select(p => p.Name).ToList();
                        }
                    }
                }
            }
        }
        catch { }
        
        return new[] { "zh-CN", "en-US" }; // 默认返回中英文
    }
}
