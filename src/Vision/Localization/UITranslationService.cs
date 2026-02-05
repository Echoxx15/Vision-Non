using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Logger;
using Newtonsoft.Json.Linq;

namespace Vision.Localization;

/// <summary>
/// UI翻译服务 - 基于窗体->控件->语言的JSON架构
/// 
/// JSON格式示例:
/// {
///   "Frm_Main": {
///     "btn_User": { "zh-CN": "用户(U)", "en-US": "User(U)" }
///   }
/// }
/// </summary>
public sealed class UITranslationService
{
    private static readonly Lazy<UITranslationService> _instance = new(() => new UITranslationService());
    public static UITranslationService Instance => _instance.Value;

    private readonly string _translationFilePath;
    private readonly string _configPath;
    private JObject _translations;
    private string _currentLanguage;

    /// <summary>
    /// 语言变更事件
    /// </summary>
    public event EventHandler<string> LanguageChanged;

    /// <summary>
    /// 当前语言代码
    /// </summary>
    public string CurrentLanguage => _currentLanguage;
    
    /// <summary>
    /// 当前语言代码（别名，兼容旧API）
    /// </summary>
    public string CurrentLanguageCode => _currentLanguage;

    /// <summary>
    /// 支持的语言列表
    /// </summary>
    public List<LanguageInfo> SupportedLanguages { get; } =
    [
        new("zh-CN", "简体中文", "Chinese"),
        new("en-US", "English", "English")
    ];

    private UITranslationService()
    {
        _translationFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Languages", "ui-translations.json");
        _configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "language.config");
        _currentLanguage = "zh-CN";
        _translations = new JObject();
    }

    /// <summary>
    /// 初始化服务
    /// </summary>
    public void Initialize()
    {
        try
        {
            // 读取保存的语言设置
            if (File.Exists(_configPath))
            {
                var savedLang = File.ReadAllText(_configPath).Trim();
                if (!string.IsNullOrWhiteSpace(savedLang) && SupportedLanguages.Any(l => l.Code == savedLang))
                {
                    _currentLanguage = savedLang;
                }
            }

            // 加载翻译文件
            LoadTranslations();
            ApplyThreadCulture();

            LogHelper.Info($"[UITranslationService] 初始化完成，当前语言: {_currentLanguage}");
        }
        catch (Exception ex)
        {
            LogHelper.Error(ex, "[UITranslationService] 初始化失败");
        }
    }

    /// <summary>
    /// 加载翻译文件
    /// </summary>
    private void LoadTranslations()
    {
        try
        {
            if (File.Exists(_translationFilePath))
            {
                var json = File.ReadAllText(_translationFilePath);
                _translations = JObject.Parse(json);
                LogHelper.Info($"[UITranslationService] 翻译文件加载成功");
            }
            else
            {
                LogHelper.Warn($"[UITranslationService] 翻译文件不存在: {_translationFilePath}");
            }
        }
        catch (Exception ex)
        {
            LogHelper.Error(ex, "[UITranslationService] 加载翻译文件失败");
        }
    }

    /// <summary>
    /// 重新加载翻译文件（用于热更新）
    /// </summary>
    public void ReloadTranslations()
    {
        LoadTranslations();
        LanguageChanged?.Invoke(this, _currentLanguage);
    }

    /// <summary>
    /// 切换语言（只保存配置，重启软件后生效）
    /// </summary>
    public void SetLanguage(string languageCode)
    {
        if (string.IsNullOrWhiteSpace(languageCode)) return;
        if (!SupportedLanguages.Any(l => l.Code == languageCode))
        {
            LogHelper.Warn($"[UITranslationService] 不支持的语言: {languageCode}");
            return;
        }

        try
        {
            // 只保存设置，不立即加载和应用
            File.WriteAllText(_configPath, languageCode);
            LogHelper.Info($"[UITranslationService] 语言设置已保存: {languageCode}，重启软件后生效");
        }
        catch (Exception ex)
        {
            LogHelper.Error(ex, $"[UITranslationService] 保存语言设置失败: {languageCode}");
        }
    }

    /// <summary>
    /// 获取指定窗体和控件的翻译文本
    /// </summary>
    /// <param name="formName">窗体名称（如 Frm_Main）</param>
    /// <param name="controlName">控件名称（如 btn_User）</param>
    /// <param name="defaultValue">默认值</param>
    public string Get(string formName, string controlName, string defaultValue = null)
    {
        try
        {
            var formObj = _translations[formName] as JObject;
            if (formObj == null) return defaultValue ?? controlName;

            var controlObj = formObj[controlName] as JObject;
            if (controlObj == null) return defaultValue ?? controlName;

            var text = controlObj[_currentLanguage]?.ToString();
            return !string.IsNullOrEmpty(text) ? text : (defaultValue ?? controlName);
        }
        catch
        {
            return defaultValue ?? controlName;
        }
    }

    /// <summary>
    /// 获取通用文本
    /// </summary>
    public string GetCommon(string key, string defaultValue = null)
    {
        return Get("Common", key, defaultValue);
    }

    /// <summary>
    /// 获取当前语言信息
    /// </summary>
    public LanguageInfo GetCurrentLanguageInfo()
    {
        return SupportedLanguages.FirstOrDefault(l => l.Code == _currentLanguage)
               ?? SupportedLanguages.First();
    }

    private void ApplyThreadCulture()
    {
        try
        {
            var culture = CultureInfo.GetCultureInfo(_currentLanguage);
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;
        }
        catch { /* ignore */ }
    }

    /// <summary>
    /// 添加新的支持语言
    /// </summary>
    public void AddSupportedLanguage(LanguageInfo languageInfo)
    {
        if (languageInfo == null || string.IsNullOrWhiteSpace(languageInfo.Code)) return;
        if (SupportedLanguages.All(l => l.Code != languageInfo.Code))
        {
            SupportedLanguages.Add(languageInfo);
        }
    }
}

/// <summary>
/// 窗体多语言扩展方法
/// </summary>
public static class UITranslationExtensions
{
    /// <summary>
    /// 获取当前窗体的控件翻译
    /// </summary>
    public static string T(this Form form, string controlName, string defaultValue = null)
    {
        var formName = form.GetType().Name;
        return UITranslationService.Instance.Get(formName, controlName, defaultValue);
    }

    /// <summary>
    /// 获取通用翻译
    /// </summary>
    public static string TC(string key, string defaultValue = null)
    {
        return UITranslationService.Instance.GetCommon(key, defaultValue);
    }
}

/// <summary>
/// 语言切换菜单项 - 使用新的 UITranslationService
/// </summary>
public class UILanguageMenuItem : ToolStripMenuItem
{
    public UILanguageMenuItem()
    {
        Text = UITranslationService.Instance.Get("Frm_Main", "LanguageMenu", "语言(L)");
        InitializeLanguageItems();
        UITranslationService.Instance.LanguageChanged += OnLanguageChanged;
    }

    private void InitializeLanguageItems()
    {
        DropDownItems.Clear();

        foreach (var lang in UITranslationService.Instance.SupportedLanguages)
        {
            var item = new ToolStripMenuItem(lang.NativeName)
            {
                Tag = lang.Code,
                Checked = lang.Code == UITranslationService.Instance.CurrentLanguage
            };
            item.Click += OnLanguageItemClick;
            DropDownItems.Add(item);
        }
    }

    private void OnLanguageItemClick(object sender, EventArgs e)
    {
        if (sender is ToolStripMenuItem item && item.Tag is string langCode)
        {
            UITranslationService.Instance.SetLanguage(langCode);
        }
    }

    private void OnLanguageChanged(object sender, string languageCode)
    {
        Text = UITranslationService.Instance.Get("Frm_Main", "LanguageMenu", "语言(L)");

        foreach (ToolStripMenuItem item in DropDownItems)
        {
            item.Checked = item.Tag as string == languageCode;
        }
    }
}
