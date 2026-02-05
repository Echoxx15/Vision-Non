using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Logger;

namespace Vision.Localization;

/// <summary>
/// 多语言服务 - 提供可扩展的多语言支持
/// </summary>
public sealed class LanguageService
{
    private static readonly Lazy<LanguageService> _instance = new(() => new LanguageService());
    public static LanguageService Instance => _instance.Value;

    private readonly string _configPath;
    private ILanguageProvider _provider;
    private Dictionary<string, string> _currentTranslations;
    private string _currentLanguageCode;

    /// <summary>
    /// 语言变更事件
    /// </summary>
    public event EventHandler<string> LanguageChanged;

    /// <summary>
    /// 当前语言代码
    /// </summary>
    public string CurrentLanguageCode => _currentLanguageCode;

    /// <summary>
    /// 支持的语言列表
    /// </summary>
    public List<LanguageInfo> SupportedLanguages { get; } = new()
    {
        new LanguageInfo("zh-CN", "简体中文", "Chinese Simplified"),
        new LanguageInfo("en-US", "English", "English"),
        // 扩展：添加更多语言
        // new LanguageInfo("ja-JP", "日本語", "Japanese"),
        // new LanguageInfo("ko-KR", "한국어", "Korean"),
    };

    private LanguageService()
    {
        _configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "language.config");
        _provider = new JsonLanguageProvider();
        _currentTranslations = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        _currentLanguageCode = "zh-CN"; // 默认中文
    }

    /// <summary>
    /// 设置语言提供者（支持扩展为其他格式）
    /// </summary>
    public void SetProvider(ILanguageProvider provider)
    {
        _provider = provider ?? throw new ArgumentNullException(nameof(provider));
    }

    /// <summary>
    /// 初始化语言服务
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
                    _currentLanguageCode = savedLang;
                }
            }

            // 加载翻译
            LoadTranslations(_currentLanguageCode);
            ApplyThreadCulture();

            LogHelper.Info($"[LanguageService] 初始化完成，当前语言: {_currentLanguageCode}");
        }
        catch (Exception ex)
        {
            LogHelper.Error(ex, "[LanguageService] 初始化失败");
        }
    }

    /// <summary>
    /// 切换语言（保存配置，重启软件后生效）
    /// </summary>
    /// <param name="languageCode">语言代码</param>
    public void SetLanguage(string languageCode)
    {
        if (string.IsNullOrWhiteSpace(languageCode)) return;
        if (!SupportedLanguages.Any(l => l.Code == languageCode))
        {
            LogHelper.Warn($"[LanguageService] 不支持的语言: {languageCode}");
            return;
        }

        try
        {
            // 只保存设置，不立即加载和应用
            File.WriteAllText(_configPath, languageCode);
            LogHelper.Info($"[LanguageService] 语言设置已保存: {languageCode}，重启软件后生效");
        }
        catch (Exception ex)
        {
            LogHelper.Error(ex, $"[LanguageService] 保存语言设置失败: {languageCode}");
        }
    }

    /// <summary>
    /// 获取翻译文本
    /// </summary>
    /// <param name="key">翻译键</param>
    /// <param name="defaultValue">默认值（如果未找到翻译）</param>
    public string Get(string key, string defaultValue = null)
    {
        if (string.IsNullOrWhiteSpace(key)) return defaultValue ?? key;

        if (_currentTranslations.TryGetValue(key, out var value) && !string.IsNullOrWhiteSpace(value))
        {
            return value;
        }

        return defaultValue ?? key;
    }

    /// <summary>
    /// 获取翻译文本（带格式化参数）
    /// </summary>
    public string GetFormat(string key, params object[] args)
    {
        var template = Get(key);
        try
        {
            return string.Format(template, args);
        }
        catch
        {
            return template;
        }
    }

    /// <summary>
    /// 获取当前语言信息
    /// </summary>
    public LanguageInfo GetCurrentLanguageInfo()
    {
        return SupportedLanguages.FirstOrDefault(l => l.Code == _currentLanguageCode)
               ?? SupportedLanguages.First();
    }

    /// <summary>
    /// 注册控件自动更新语言
    /// </summary>
    public void RegisterControl(Control control, Action<Control> updateAction)
    {
        if (control == null || updateAction == null) return;

        LanguageChanged += (_, _) =>
        {
            if (control.IsDisposed) return;
            try
            {
                if (control.InvokeRequired)
                    control.BeginInvoke(updateAction, control);
                else
                    updateAction(control);
            }
            catch { /* ignored */ }
        };
    }

    /// <summary>
    /// 添加支持的语言
    /// </summary>
    public void AddSupportedLanguage(LanguageInfo languageInfo)
    {
        if (languageInfo == null || string.IsNullOrWhiteSpace(languageInfo.Code)) return;
        if (SupportedLanguages.All(l => l.Code != languageInfo.Code))
        {
            SupportedLanguages.Add(languageInfo);
        }
    }

    private void LoadTranslations(string languageCode)
    {
        _currentTranslations = _provider.Load(languageCode);
    }

    private void ApplyThreadCulture()
    {
        try
        {
            var culture = CultureInfo.GetCultureInfo(_currentLanguageCode);
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;
        }
        catch { /* ignore */ }
    }

    private void ApplyToAllOpenForms()
    {
        try
        {
            foreach (Form form in Application.OpenForms)
            {
                if (form.IsDisposed) continue;

                // 尝试调用窗体的 ApplyLanguage 方法（如果实现了 ILocalizable）
                if (form is ILocalizable localizable)
                {
                    if (form.InvokeRequired)
                        form.BeginInvoke(new Action(() => localizable.ApplyLanguage()));
                    else
                        localizable.ApplyLanguage();
                }
            }
        }
        catch (Exception ex)
        {
            LogHelper.Warn($"[LanguageService] 更新窗体语言失败: {ex.Message}");
        }
    }
}

/// <summary>
/// 本地化接口 - 窗体/控件实现此接口以支持动态语言切换
/// </summary>
public interface ILocalizable
{
    /// <summary>
    /// 应用当前语言
    /// </summary>
    void ApplyLanguage();
}
