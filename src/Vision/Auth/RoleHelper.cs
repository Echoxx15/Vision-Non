using System;
using System.Collections.Generic;

namespace Vision.Auth;

/// <summary>
/// 角色显示名与枚举的互转（中文）。
/// </summary>
public static class RoleHelper
{
    private static readonly Dictionary<Role, string> _display = new()
    {
        { Role.Admin, "管理员" },
        { Role.Engineer, "工程师" },
        { Role.Technician, "技术员" },
    };

    public static string GetDisplay(Role role) => _display.TryGetValue(role, out var s) ? s : role.ToString();

    public static Role ParseDisplay(string text)
    {
        foreach (var kv in _display)
        {
            if (string.Equals(kv.Value, text, StringComparison.OrdinalIgnoreCase)) return kv.Key;
        }
        // fallbacks
        if (Enum.TryParse<Role>(text, out var r)) return r;
        return Role.Technician;
    }

    public static object[] GetAllDisplays() => [GetDisplay(Role.Admin), GetDisplay(Role.Engineer), GetDisplay(Role.Technician)
    ];
}