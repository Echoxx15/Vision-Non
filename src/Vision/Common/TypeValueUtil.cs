using System;
using System.Collections.Generic;
using System.Globalization;

namespace Vision.Common;

/// <summary>
/// 支持的变量类型定义 - 以 HSL 通讯库的 Read/Write 方法支持的类型为准
/// 统一定义所有支持的变量类型，确保全局变量、工位变量、通讯变量类型一致
/// </summary>
public static class SupportedTypes
{
    /// <summary>
    /// 所有支持的类型列表（按显示顺序）
    /// </summary>
    public static readonly IReadOnlyList<Type> All = new Type[]
    {
        typeof(bool),
        typeof(short),
        typeof(int),
        typeof(float),
        typeof(double),
        typeof(string),
        typeof(bool[]),
        typeof(short[]),
        typeof(int[]),
        typeof(float[]),
        typeof(double[]),
        typeof(string[])
    };

    /// <summary>
    /// 标量类型（非数组）
    /// </summary>
    public static readonly IReadOnlyList<Type> Scalars = new Type[]
    {
        typeof(bool),
        typeof(short),
        typeof(int),
        typeof(float),
        typeof(double),
        typeof(string)
    };

    /// <summary>
    /// 数组类型
    /// </summary>
    public static readonly IReadOnlyList<Type> Arrays = new Type[]
    {
        typeof(bool[]),
        typeof(short[]),
        typeof(int[]),
        typeof(float[]),
        typeof(double[]),
        typeof(string[])
    };

    /// <summary>
    /// 检查类型是否被支持
    /// </summary>
    public static bool IsSupported(Type type)
    {
        if (type == null) return false;
        foreach (var t in All)
        {
            if (t == type) return true;
        }
        return false;
    }

    /// <summary>
    /// 获取类型的显示名称
    /// </summary>
    public static string GetDisplayName(Type type)
    {
        return TypeValueUtil.TypeToName(type);
    }
}

public static class TypeValueUtil
{
    /// <summary>
    /// 类型转换为名称字符串（用于序列化）
    /// 支持：bool, short, int, float, double, string 及其数组，以及其他类型
    /// </summary>
    public static string TypeToName(Type t)
    {
        if (t == null) return string.Empty;
        if (t == typeof(bool)) return "bool";
        if (t == typeof(short)) return "short";
        if (t == typeof(int)) return "int";
        if (t == typeof(float)) return "float";
        if (t == typeof(double)) return "double";
        if (t == typeof(string)) return "string";
        if (t == typeof(bool[])) return "bool[]";
        if (t == typeof(short[])) return "short[]";
        if (t == typeof(int[])) return "int[]";
        if (t == typeof(float[])) return "float[]";
        if (t == typeof(double[])) return "double[]";
        if (t == typeof(string[])) return "string[]";
        
        // 对于其他类型，返回完整类型名（含命名空间）以便反序列化时能找到
        return t.FullName ?? t.Name;
    }

    /// <summary>
    /// 名称字符串转换为类型（用于反序列化）
    /// 支持：bool, short, int, float, double, string 及其数组，以及动态加载的类型
    /// </summary>
    public static Type ResolveType(string name)
    {
        if (string.IsNullOrEmpty(name)) return null;
        
        switch (name)
        {
            case "bool": return typeof(bool);
            case "short": return typeof(short);
            case "int": return typeof(int);
            case "float": return typeof(float);
            case "double": return typeof(double);
            case "string": return typeof(string);
            case "bool[]": return typeof(bool[]);
            case "short[]": return typeof(short[]);
            case "int[]": return typeof(int[]);
            case "float[]": return typeof(float[]);
            case "double[]": return typeof(double[]);
            case "string[]": return typeof(string[]);
        }
        
        // 尝试从已加载的程序集中解析类型（支持深度学习模型等动态类型）
        try
        {
            // 尝试直接获取类型
            var type = Type.GetType(name);
            if (type != null) return type;
            
            // 遍历所有已加载的程序集查找类型
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                try
                {
                    type = assembly.GetType(name);
                    if (type != null) return type;
                    
                    // 尝试只用类名匹配（不含命名空间）
                    if (!name.Contains("."))
                    {
                        foreach (var t in assembly.GetExportedTypes())
                        {
                            if (t.Name == name) return t;
                        }
                    }
                }
                catch { }
            }
        }
        catch { }
        
        return null;
    }

    /// <summary>
    /// 将值转换为字符串（用于显示）
    /// </summary>
    public static string ValueToString(object v, Type t)
    {
        if (v == null) return string.Empty;
        if (t == typeof(bool)) return (v is bool b ? b : (v?.ToString() == "1")).ToString();
        if (t == typeof(short)) return Convert.ToInt16(v).ToString(CultureInfo.InvariantCulture);
        if (t == typeof(int)) return Convert.ToInt32(v).ToString(CultureInfo.InvariantCulture);
        if (t == typeof(float)) return Convert.ToSingle(v, CultureInfo.InvariantCulture).ToString(CultureInfo.InvariantCulture);
        if (t == typeof(double)) return Convert.ToDouble(v, CultureInfo.InvariantCulture).ToString(CultureInfo.InvariantCulture);
        if (t != null && t.IsArray) return "<数组>";
        return v.ToString();
    }

    /// <summary>
    /// 尝试将字符串解析为指定类型的值
    /// </summary>
    public static bool TryParseValue(string s, Type t, out object value, out string reason, bool disallowArrays = true)
    {
        value = null; reason = null;
        if (t == null) { value = s; return true; }
        if (disallowArrays && t.IsArray) { reason = "数组值不能在此编辑"; return false; }
        if (t == typeof(string)) { value = s; return true; }
        if (t == typeof(bool))
        {
            if (bool.TryParse(s, out var bv)) { value = bv; return true; }
            if (s == "1") { value = true; return true; }
            if (s == "0") { value = false; return true; }
            reason = "请输入 true/false 或 1/0"; return false;
        }
        if (t == typeof(short))
        {
            if (short.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out var sv)) { value = sv; return true; }
            reason = "请输入整数"; return false;
        }
        if (t == typeof(int))
        {
            if (int.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out var iv)) { value = iv; return true; }
            reason = "请输入整数"; return false;
        }
        if (t == typeof(float))
        {
            if (float.TryParse(s, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out var fv)) { value = fv; return true; }
            if (float.TryParse(s, out fv)) { value = fv; return true; }
            reason = "请输入数字"; return false;
        }
        if (t == typeof(double))
        {
            if (double.TryParse(s, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out var dv)) { value = dv; return true; }
            if (double.TryParse(s, out dv)) { value = dv; return true; }
            reason = "请输入数字"; return false;
        }
        try
        {
            value = Convert.ChangeType(s, t, CultureInfo.InvariantCulture);
            return true;
        }
        catch { reason = "值类型不匹配"; return false; }
    }

    /// <summary>
    /// 获取类型的默认值
    /// </summary>
    public static object GetDefaultValue(Type t)
    {
        if (t == typeof(bool)) return false;
        if (t == typeof(short)) return (short)0;
        if (t == typeof(int)) return 0;
        if (t == typeof(float)) return 0f;
        if (t == typeof(double)) return 0d;
        if (t == typeof(string)) return string.Empty;
        if (t == typeof(bool[])) return Array.Empty<bool>();
        if (t == typeof(short[])) return Array.Empty<short>();
        if (t == typeof(int[])) return Array.Empty<int>();
        if (t == typeof(float[])) return Array.Empty<float>();
        if (t == typeof(double[])) return Array.Empty<double>();
        if (t == typeof(string[])) return Array.Empty<string>();
        return null;
    }

    /// <summary>
    /// 获取类型的默认值字符串表示
    /// </summary>
    public static string GetDefaultValueString(Type t)
    {
        if (t == typeof(bool)) return "False";
        if (t == typeof(short)) return "0";
        if (t == typeof(int)) return "0";
        if (t == typeof(float)) return "0";
        if (t == typeof(double)) return "0";
        if (t == typeof(string)) return string.Empty;
        if (t != null && t.IsArray) return "<数组>";
        return string.Empty;
    }
}