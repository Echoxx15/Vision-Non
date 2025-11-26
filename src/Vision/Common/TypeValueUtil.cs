using System;
using System.Globalization;

namespace Vision.Common;

public static class TypeValueUtil
{
    public static string TypeToName(Type t)
    {
        if (t == typeof(int)) return "int";
        if (t == typeof(double)) return "double";
        if (t == typeof(string)) return "string";
        if (t == typeof(bool)) return "bool";
        if (t == typeof(int[])) return "int[]";
        if (t == typeof(double[])) return "double[]";
        if (t == typeof(string[])) return "string[]";
        if (t == typeof(bool[])) return "bool[]";
        return t?.Name ?? string.Empty;
    }

    public static Type ResolveType(string name)
    {
        switch (name)
        {
            case "int": return typeof(int);
            case "double": return typeof(double);
            case "string": return typeof(string);
            case "bool": return typeof(bool);
            case "int[]": return typeof(int[]);
            case "double[]": return typeof(double[]);
            case "string[]": return typeof(string[]);
            case "bool[]": return typeof(bool[]);
            default: return null;
        }
    }

    public static string ValueToString(object v, Type t)
    {
        if (v == null) return string.Empty;
        if (t == typeof(bool)) return (v is bool b ? b : (v?.ToString() == "1")).ToString();
        if (t == typeof(double)) return Convert.ToDouble(v, CultureInfo.InvariantCulture).ToString(CultureInfo.InvariantCulture);
        if (t == typeof(int)) return Convert.ToInt32(v).ToString(CultureInfo.InvariantCulture);
        if (t != null && t.IsArray) return "<数组>";
        return v.ToString();
    }

    public static bool TryParseValue(string s, Type t, out object value, out string reason, bool disallowArrays = true)
    {
        value = null; reason = null;
        if (t == null) { value = s; return true; }
        if (disallowArrays && t.IsArray) { reason = "数组值不可在此编辑"; return false; }
        if (t == typeof(string)) { value = s; return true; }
        if (t == typeof(int))
        {
            if (int.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out var iv)) { value = iv; return true; }
            reason = "请输入整数"; return false;
        }
        if (t == typeof(double))
        {
            if (double.TryParse(s, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out var dv)) { value = dv; return true; }
            if (double.TryParse(s, out dv)) { value = dv; return true; }
            reason = "请输入数字"; return false;
        }
        if (t == typeof(bool))
        {
            if (bool.TryParse(s, out var bv)) { value = bv; return true; }
            if (s == "1") { value = true; return true; }
            if (s == "0") { value = false; return true; }
            reason = "请输入 true/false 或 1/0"; return false;
        }
        try
        {
            value = Convert.ChangeType(s, t, CultureInfo.InvariantCulture);
            return true;
        }
        catch { reason = "值类型不匹配"; return false; }
    }
}