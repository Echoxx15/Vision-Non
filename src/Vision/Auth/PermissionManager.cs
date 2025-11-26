using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Vision.Auth.Storage;

namespace Vision.Auth;

/// <summary>
/// 统一的认证数据库路径解析与规范化。
/// 确保 Data 目录存在；失败时回退到本机公共应用数据目录。
/// </summary>
internal static class AuthPath
{
    public static readonly string DbPath = Init();

    private static string Init()
    {
        try
        {
            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            var target = Path.GetFullPath(Path.Combine(baseDir, "Data", "auth.db"));
            
            var dir = Path.GetDirectoryName(target);
            EnsureDirectory(dir);
            
            // 验证路径是否真实存在或可创建
            if (!Directory.Exists(dir))
            {
                Console.WriteLine($"[AuthPath] 警告：无法创建目录 {dir}，尝试回退方案");
                return GetFallbackDbPath();
            }
            
            Console.WriteLine($"[AuthPath] 数据库路径: {target}");
            return target;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[AuthPath] 初始化失败: {ex.Message}");
            return GetFallbackDbPath();
        }
    }

    private static string GetFallbackDbPath()
    {
        // 回退方案 1: 使用当前工作目录
        try
        {
            var currentDir = Environment.CurrentDirectory;
            var dataDir = Path.Combine(currentDir, "Data");
            EnsureDirectory(dataDir);
            
            if (Directory.Exists(dataDir))
            {
                var dbPath = Path.Combine(dataDir, "auth.db");
                Console.WriteLine($"[AuthPath] 使用工作目录: {dbPath}");
                return dbPath;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[AuthPath] 工作目录方案失败: {ex.Message}");
        }
        
        // 回退方案 2: %ProgramData%\Vision\Data\auth.db
        try
        {
            var altRoot = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), 
                "Vision", 
                "Data");
            EnsureDirectory(altRoot);
            var dbPath = Path.Combine(altRoot, "auth.db");
            Console.WriteLine($"[AuthPath] 使用 ProgramData: {dbPath}");
            return dbPath;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[AuthPath] ProgramData 方案失败: {ex.Message}");
        }
        
        // 回退方案 3: 临时目录
        try
        {
            var tempPath = Path.Combine(Path.GetTempPath(), "Vision", "Data");
            EnsureDirectory(tempPath);
            var dbPath = Path.Combine(tempPath, "auth.db");
            Console.WriteLine($"[AuthPath] 使用临时目录: {dbPath}");
            return dbPath;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[AuthPath] 临时目录方案失败: {ex.Message}");
        }
        
        // 最后的回退：当前目录的 auth.db
        var lastResort = Path.GetFullPath("auth.db");
        Console.WriteLine($"[AuthPath] 使用最后回退方案: {lastResort}");
        return lastResort;
    }

    private static void EnsureDirectory(string dir)
    {
        if (string.IsNullOrWhiteSpace(dir)) return;
        
        try
        {
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[AuthPath] 创建目录失败 {dir}: {ex.Message}");
            throw;
        }
    }
}

/// <summary>
/// 角色-控件权限：每个角色对每个控件分别设置 Enabled（可点/禁用）。
/// 表：Permissions(Id, Role, ItemName, Enabled)
/// </summary>
public static class PermissionManager
{
    private static readonly string _dbPath = AuthPath.DbPath;

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
        "btn_User",
        "btn_Login"
    };

    static PermissionManager()
    {
        using var db = new SqliteDb(_dbPath);
        if (!db.Available) return;
        db.ExecuteNonQuery(_tblSql1);
        db.ExecuteNonQuery(_tblSql2);
    }

    public static void SyncItems(IEnumerable<ToolStripItem> items, Role defaultMinRole = Role.Admin,
        params string[] skipNames)
    {
        var skip = new HashSet<string>((skipNames ?? Array.Empty<string>()).Concat(AlwaysAllow),
            StringComparer.OrdinalIgnoreCase);
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
                db.ExecuteNonQuery("INSERT INTO Permissions(ItemName,MinRole) VALUES(@n,@r)", ("@n", name),
                    ("@r", (int)defaultMinRole));
                foreach (Role role in new[] { Role.Technician, Role.Engineer, Role.Admin })
                {
                    int en = (role == Role.Admin) ? 1 : 0;
                    db.ExecuteNonQuery(
                        "INSERT OR IGNORE INTO RolePermissions(Role,ItemName,Enabled) VALUES(@role,@n,@e)",
                        ("@role", (int)role), ("@n", name), ("@e", en));
                }
            }
        }
    }

    public static void RegisterBarItem(ToolStripItem item, Role defaultMinRole)
    {
        if (item == null) return;
        using var db = new SqliteDb(_dbPath);
        if (!db.Available) return;
        var exists = db.ExecuteScalar("SELECT 1 FROM Permissions WHERE ItemName=@n", ("@n", item.Name)) != null;
        if (!exists)
        {
            db.ExecuteNonQuery("INSERT INTO Permissions(ItemName,MinRole) VALUES(@n,@r)", ("@n", item.Name),
                ("@r", (int)defaultMinRole));
            foreach (Role role in new[] { Role.Technician, Role.Engineer, Role.Admin })
            {
                int en = (role == Role.Admin) ? 1 : 0;
                db.ExecuteNonQuery("INSERT OR IGNORE INTO RolePermissions(Role,ItemName,Enabled) VALUES(@role,@n,@e)",
                    ("@role", (int)role), ("@n", item.Name), ("@e", en));
            }
        }
    }

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

    public static void ApplyPermissions(User user, IEnumerable<ToolStripItem> items)
    {
        using var db = new SqliteDb(_dbPath);
        var hasDb = db.Available;

        var dict = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        if (hasDb && user != null)
        {
            db.ExecuteReader("SELECT ItemName,Enabled FROM RolePermissions WHERE Role=@r",
                r => { dict[r.GetString(0)] = r.GetInt32(1); }, ("@r", (int)user.Role));
        }

        foreach (var it in items ?? Array.Empty<ToolStripItem>())
        {
            if (it == null) continue;
            var name = it.Name;
            if (AlwaysAllow.Contains(name))
            {
                it.Enabled = true;
                continue;
            }

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
                allow = false;
            }
            else
            {
                allow = dict.TryGetValue(name, out var en) && en == 1;
            }

            it.Enabled = allow;
        }
    }

    public static IEnumerable<(string Name, int Enabled)> GetRoleAssignments(Role role)
    {
        var list = new List<(string, int)>();
        using var db = new SqliteDb(_dbPath);
        if (!db.Available) return list;
        db.ExecuteReader(@"SELECT p.ItemName, COALESCE(r.Enabled,0)
FROM Permissions p
LEFT JOIN RolePermissions r ON r.ItemName = p.ItemName AND r.Role=@r",
            r => { list.Add((r.GetString(0), r.GetInt32(1))); }, ("@r", (int)role));
        return list;
    }
}