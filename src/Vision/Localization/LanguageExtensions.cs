using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Vision.Localization;

/// <summary>
/// 语言扩展方法 - 简化使用
/// </summary>
public static class LanguageExtensions
{
    /// <summary>
    /// 获取翻译文本的快捷方法
    /// </summary>
    /// <param name="key">翻译键</param>
    /// <param name="defaultValue">默认值</param>
    /// <returns>翻译后的文本</returns>
    public static string L(this string key, string defaultValue = null)
    {
        return LanguageService.Instance.Get(key, defaultValue);
    }

    /// <summary>
    /// 获取翻译文本（带格式化参数）
    /// </summary>
    public static string LFormat(this string key, params object[] args)
    {
        return LanguageService.Instance.GetFormat(key, args);
    }

    /// <summary>
    /// 设置控件文本并注册语言变更自动更新
    /// </summary>
    public static void SetLocalizedText(this Control control, string key, string defaultValue = null)
    {
        if (control == null) return;

        control.Text = LanguageService.Instance.Get(key, defaultValue);
        LanguageService.Instance.RegisterControl(control, c => c.Text = LanguageService.Instance.Get(key, defaultValue));
    }

    /// <summary>
    /// 设置 ToolStripItem 文本并注册语言变更自动更新
    /// </summary>
    public static void SetLocalizedText(this ToolStripItem item, string key, string defaultValue = null)
    {
        if (item == null) return;

        item.Text = LanguageService.Instance.Get(key, defaultValue);
        LanguageService.Instance.LanguageChanged += (_, _) =>
        {
            try
            {
                item.Text = LanguageService.Instance.Get(key, defaultValue);
            }
            catch { /* ignored */ }
        };
    }

    /// <summary>
    /// 批量设置按钮/标签文本
    /// </summary>
    public static void LocalizeControls(this Control parent, params (Control control, string key)[] mappings)
    {
        foreach (var (control, key) in mappings)
        {
            control.SetLocalizedText(key);
        }
    }
}

/// <summary>
/// 语言选择下拉框 - 可直接放入工具栏或菜单
/// </summary>
public class LanguageComboBox : ToolStripComboBox
{
    public LanguageComboBox()
    {
        DropDownStyle = ComboBoxStyle.DropDownList;
        AutoSize = false;
        Size = new Size(100, 25);

        InitializeLanguages();

        SelectedIndexChanged += OnSelectedIndexChanged;
        LanguageService.Instance.LanguageChanged += OnLanguageChanged;
    }

    private void InitializeLanguages()
    {
        Items.Clear();
        foreach (var lang in LanguageService.Instance.SupportedLanguages)
        {
            Items.Add(lang);
        }

        // 选中当前语言
        var currentLang = LanguageService.Instance.GetCurrentLanguageInfo();
        SelectedItem = Items.Cast<LanguageInfo>().FirstOrDefault(l => l.Code == currentLang.Code);
    }

    private void OnSelectedIndexChanged(object sender, EventArgs e)
    {
        if (SelectedItem is LanguageInfo selectedLang)
        {
            if (selectedLang.Code != LanguageService.Instance.CurrentLanguageCode)
            {
                LanguageService.Instance.SetLanguage(selectedLang.Code);
            }
        }
    }

    private void OnLanguageChanged(object sender, string languageCode)
    {
        var lang = Items.Cast<LanguageInfo>().FirstOrDefault(l => l.Code == languageCode);
        if (lang != null && SelectedItem != lang)
        {
            SelectedItem = lang;
        }
    }
}

/// <summary>
/// 语言切换菜单项
/// </summary>
public class LanguageMenuItem : ToolStripMenuItem
{
    public LanguageMenuItem() : base()
    {
        Text = LanguageService.Instance.Get("LanguageMenu", "语言");
        InitializeLanguageItems();
        LanguageService.Instance.LanguageChanged += OnLanguageChanged;
    }

    private void InitializeLanguageItems()
    {
        DropDownItems.Clear();

        foreach (var lang in LanguageService.Instance.SupportedLanguages)
        {
            var item = new ToolStripMenuItem(lang.NativeName)
            {
                Tag = lang.Code,
                Checked = lang.Code == LanguageService.Instance.CurrentLanguageCode
            };
            item.Click += OnLanguageItemClick;
            DropDownItems.Add(item);
        }
    }

    private void OnLanguageItemClick(object sender, EventArgs e)
    {
        if (sender is ToolStripMenuItem item && item.Tag is string langCode)
        {
            LanguageService.Instance.SetLanguage(langCode);
        }
    }

    private void OnLanguageChanged(object sender, string languageCode)
    {
        Text = LanguageService.Instance.Get("LanguageMenu", "语言");

        foreach (ToolStripMenuItem item in DropDownItems)
        {
            item.Checked = item.Tag as string == languageCode;
        }
    }
}
