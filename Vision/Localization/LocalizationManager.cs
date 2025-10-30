using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;

namespace Vision.Localization;

/// <summary>
/// Centralized localization manager for WinForms.
/// </summary>
public static class LocalizationManager
{
    public static event EventHandler LanguageChanged;
    private static CultureInfo CurrentCulture { get; set; } = CultureInfo.CurrentUICulture;

    private static readonly string ConfigPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "language.config");

    public static void Initialize()
    {
        try
        {
            if (File.Exists(ConfigPath))
            {
                var name = (File.ReadAllText(ConfigPath) ?? string.Empty).Trim();
                if (!string.IsNullOrWhiteSpace(name))
                {
                    // Do not apply to open forms (none yet), just set thread culture
                    SetCulture(name, applyToOpenForms: false);
                    return;
                }
            }
            ApplyThreadCulture();
        }
        catch
        {
            // ignored
        }
    }

    public static void SetCulture(string cultureName, bool applyToOpenForms = true)
    {
        var culture = CultureInfo.GetCultureInfo(cultureName);
        CurrentCulture = culture;
        ApplyThreadCulture();

        try { File.WriteAllText(ConfigPath, cultureName); } catch { /* ignore */ }

        LanguageChanged?.Invoke(null, EventArgs.Empty);

        if (applyToOpenForms)
        {
            try
            {
                foreach (Form f in Application.OpenForms)
                {
                    if (f.IsDisposed) continue;
                    Apply(f);
                }
            }
            catch
            {
                // ignored
            }
        }
    }

    private static void ApplyThreadCulture()
    {
        try
        {
            Thread.CurrentThread.CurrentCulture = CurrentCulture;
            Thread.CurrentThread.CurrentUICulture = CurrentCulture;
        }
        catch { /* ignore */ }
    }

    /// <summary>
    /// Apply resources from .resx to the provided control and its children/components.
    /// </summary>
    public static void Apply(Control root)
    {
        if (root == null) return;
        try
        {
            var res = new ComponentResourceManager(root.GetType());
            ApplyRecursive(root, res, CurrentCulture);
            ApplyComponents(root, res, CurrentCulture);
            root.PerformLayout();
        }
        catch
        {
            // ignored
        }
    }

    /// <summary>
    /// Enable auto re-localization for a control on LanguageChanged.
    /// Call once, e.g., in Form/UserControl constructor after InitializeComponent().
    /// </summary>
    public static void EnableAutoLocalization(Control ctrl)
    {
        if (ctrl == null) return;
        LanguageChanged += (_, _) =>
        {
            if (ctrl.IsDisposed) return;
            try
            {
                if (ctrl.InvokeRequired)
                    ctrl.BeginInvoke(new Action<Control>(Apply), ctrl);
                else
                    Apply(ctrl);
            }
            catch { /* ignored */ }
        };
    }

    private static void ApplyRecursive(Control ctrl, ComponentResourceManager res, CultureInfo culture)
    {
        try { res.ApplyResources(ctrl, ctrl.Name, culture); } catch { /* ignore */ }
        foreach (Control child in ctrl.Controls)
            ApplyRecursive(child, res, culture);
    }

    private static void ApplyComponents(Control root, ComponentResourceManager res, CultureInfo culture)
    {
        // Find the designer components container via reflection
        var field = root.GetType().GetField("components", BindingFlags.Instance | BindingFlags.NonPublic);
        var container = field?.GetValue(root) as IContainer;
        if (container == null) return;

        foreach (Component comp in container.Components)
        {
            var name = GetComponentName(comp);
            if (!string.IsNullOrEmpty(name))
            {
                try { res.ApplyResources(comp, name, culture); } catch { /* ignore */ }
            }
        }
    }

    private static string GetComponentName(Component comp)
    {
        try
        {
            // Prefer public Name property if present
            var pi = comp.GetType().GetProperty("Name", BindingFlags.Instance | BindingFlags.Public);
            if (pi != null && pi.PropertyType == typeof(string))
            {
                var val = pi.GetValue(comp) as string;
                if (!string.IsNullOrWhiteSpace(val)) return val;
            }
            // Fall back to Site.Name if available
            return comp.Site?.Name;
        }
        catch { return null; }
    }

    public static void Apply(Form form)
    {
        Apply((Control)form);
    }
}