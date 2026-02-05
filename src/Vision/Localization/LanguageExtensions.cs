using System.Windows.Forms;

namespace Vision.Localization;

/// <summary>
/// 语言扩展方法 - 简化使用（基于 UITranslationService）
/// </summary>
public static class LanguageExtensions
{
    /// <summary>
    /// 获取通用翻译文本的快捷方法
    /// </summary>
    /// <param name="key">翻译键</param>
    /// <param name="defaultValue">默认值</param>
    /// <returns>翻译后的文本</returns>
    public static string L(this string key, string defaultValue = null)
    {
        return UITranslationService.Instance.GetCommon(key, defaultValue);
    }

    /// <summary>
    /// 设置控件文本并注册语言变更自动更新
    /// </summary>
    public static void SetLocalizedText(this Control control, string formName, string key, string defaultValue = null)
    {
        if (control == null) return;

        control.Text = UITranslationService.Instance.Get(formName, key, defaultValue);
        UITranslationService.Instance.LanguageChanged += (_, _) =>
        {
            try
            {
                if (!control.IsDisposed)
                {
                    control.Text = UITranslationService.Instance.Get(formName, key, defaultValue);
                }
            }
            catch { /* ignored */ }
        };
    }

    /// <summary>
    /// 设置 ToolStripItem 文本并注册语言变更自动更新
    /// </summary>
    public static void SetLocalizedText(this ToolStripItem item, string formName, string key, string defaultValue = null)
    {
        if (item == null) return;

        item.Text = UITranslationService.Instance.Get(formName, key, defaultValue);
        UITranslationService.Instance.LanguageChanged += (_, _) =>
        {
            try
            {
                item.Text = UITranslationService.Instance.Get(formName, key, defaultValue);
            }
            catch { /* ignored */ }
        };
    }

    /// <summary>
    /// 批量设置控件文本
    /// </summary>
    public static void LocalizeControls(this Form form, params (Control control, string key)[] mappings)
    {
        var formName = form.GetType().Name;
        foreach (var (control, key) in mappings)
        {
            control.SetLocalizedText(formName, key);
        }
    }
}
