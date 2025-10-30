using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Vision.Auth.Storage;

namespace Vision.Auth;

/// <summary>
/// 角色-控件权限：每个角色对每个控件分别设置 Enabled（可点/禁用）。
/// 表：Permissions(Id, Role, ItemName, Enabled)
/// </summary>
public static class PermissionManager
{
    private static readonly string _dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "auth.db");
    private static readonly string _tblSql1 = @"CREATE TABLE IF NOT EXISTS Permissions(
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    ItemName TEXT UNIQUE NOT NULL,
    MinRole INTEGER NOT NULL
);";
    private static readonly string _tblSql2 = @"CREATE TABLE IF NOT EXISTS RolePermissions(
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Role INTEGER NOT NULL,
    ItemName TEXT NOT NULL,
    Enabled INTEGER NOT NULL,
    UNIQUE(Role, ItemName)
);";

    private static readonly HashSet<string> AlwaysAllow = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        "btn_User", // 用户菜单
        "btn_Login" // 登录按钮
    };

    static PermissionManager()
    {
        using var db = new SqliteDb(_dbPath);
        if (!db.Available) return;
        db.ExecuteNonQuery(_tblSql1);
        db.ExecuteNonQuery(_tblSql2);
    }

    // 扫描并注册：把界面收集到的控件写入 Permissions/RolePermissions，默认 MinRole=Admin
    public static void SyncItems(IEnumerable<ToolStripItem> items, Role defaultMinRole = Role.Admin, params string[] skipNames)
    {
        var skip = new HashSet<string>((skipNames ?? Array.Empty<string>()).Concat(AlwaysAllow), StringComparer.OrdinalIgnoreCase);
        using var db = new SqliteDb(_dbPath);
        if (!db.Available) return;
        db.ExecuteNonQuery(_tblSql1);
        db.ExecuteNonQuery(_tblSql2);

        foreach (var item in items ?? Enumerable.Empty<ToolStripItem>())
        {
            if (item == null) continue;
            var name = item.Name;
            if (string.IsNullOrWhiteSpace(name)) continue;
            if (skip.Contains(name)) continue;
            var exists = db.ExecuteScalar("SELECT 1 FROM Permissions WHERE ItemName=@n", ("@n", name)) != null;
            if (!exists)
            {
                // 新增项：默认仅 Admin 启用
                db.ExecuteNonQuery("INSERT INTO Permissions(ItemName,MinRole) VALUES(@n,@r)", ("@n", name), ("@r", (int)defaultMinRole));
                foreach (Role role in new[] { Role.Technician, Role.Engineer, Role.Admin })
                {
                    int en = (role == Role.Admin) ? 1 : 0;
                    db.ExecuteNonQuery("INSERT OR IGNORE INTO RolePermissions(Role,ItemName,Enabled) VALUES(@role,@n,@e)",
                        ("@role", (int)role), ("@n", name), ("@e", en));
                }
            }
        }
    }

    // 初次对注册到菜单/工具栏的控件，写入 MinRole；后续以 RolePermissions 为准
    public static void RegisterBarItem(ToolStripItem item, Role defaultMinRole)
    {
        if (item == null) return;
        using var db = new SqliteDb(_dbPath);
        if (!db.Available) return;
        var exists = db.ExecuteScalar("SELECT 1 FROM Permissions WHERE ItemName=@n", ("@n", item.Name)) != null;
        if (!exists)
        {
            db.ExecuteNonQuery("INSERT INTO Permissions(ItemName,MinRole) VALUES(@n,@r)", ("@n", item.Name), ("@r", (int)defaultMinRole));
            // 也为各角色写入默认 Enabled：仅 Admin 启用
            foreach (Role role in new[] { Role.Technician, Role.Engineer, Role.Admin })
            {
                int en = (role == Role.Admin) ? 1 : 0;
                db.ExecuteNonQuery("INSERT OR IGNORE INTO RolePermissions(Role,ItemName,Enabled) VALUES(@role,@n,@e)",
                    ("@role", (int)role), ("@n", item.Name), ("@e", en));
            }
        }
    }

    // 权限设置：为选中角色，批量设置 Enabled
    public static void SetRolePermissions(Role role, IEnumerable<string> itemNames, bool enabled)
    {
        using var db = new SqliteDb(_dbPath);
        if (!db.Available) return;
        int en = enabled ? 1 : 0;
        foreach (var name in itemNames ?? Array.Empty<string>())
        {
            var updated = db.ExecuteNonQuery("UPDATE RolePermissions SET Enabled=@e WHERE Role=@r AND ItemName=@n",
                ("@e", en), ("@r", (int)role), ("@n", name));
            if (updated == 0)
            {
                db.ExecuteNonQuery("INSERT INTO RolePermissions(Role,ItemName,Enabled) VALUES(@r,@n,@e)",
                    ("@r", (int)role), ("@n", name), ("@e", en));
            }
        }
    }

    /// <summary>
    /// 应用权限：根据当前用户角色，设置每个控件的 Enabled。
    /// Admin 始终全开；未登录仅允许登录/用户菜单；数据库不可用时默认全部允许。
    /// 未注册项：Admin 允许，其他角色默认禁用。
    /// </summary>
    public static void ApplyPermissions(User user, IEnumerable<ToolStripItem> items)
    {
        using var db = new SqliteDb(_dbPath);
        var hasDb = db.Available;

        var dict = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        if (hasDb && user != null)
        {
            db.ExecuteReader("SELECT ItemName,Enabled FROM RolePermissions WHERE Role=@r", r =>
            {
                dict[r.GetString(0)] = r.GetInt32(1);
            }, ("@r", (int)user.Role));
        }

        foreach (var it in items ?? Array.Empty<ToolStripItem>())
        {
            if (it == null) continue;
            var name = it.Name;
            if (AlwaysAllow.Contains(name)) { it.Enabled = true; continue; }

            bool allow;
            if (user != null && user.Role == Role.Admin)
            {
                allow = true;
            }
            else if (!hasDb)
            {
                allow = true;
            }
            else if (user == null)
            {
                // 未登录：除 AlwaysAllow 以外默认禁用
                allow = false;
            }
            else
            {
                allow = dict.TryGetValue(name, out var en) && en == 1; // 未注册/未配置默认禁用
            }
            it.Enabled = allow;
        }
    }

    // 供权限编辑窗口显示：返回全部注册项及当前角色 Enable
    public static IEnumerable<(string Name, int Enabled)> GetRoleAssignments(Role role)
    {
        var list = new List<(string, int)>();
        using var db = new SqliteDb(_dbPath);
        if (!db.Available) return list;
        db.ExecuteReader(@"SELECT p.ItemName, COALESCE(r.Enabled,0)
FROM Permissions p
LEFT JOIN RolePermissions r ON r.ItemName = p.ItemName AND r.Role=@r", r =>
        {
            list.Add((r.GetString(0), r.GetInt32(1)));
        }, ("@r", (int)role));
        return list;
    }
}