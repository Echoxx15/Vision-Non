using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Vision.Auth.Storage;

namespace Vision.Auth;

/// <summary>
/// 角色定义：按从低到高（技术员 &lt; 工程师 &lt; 管理员）。
/// </summary>
public enum Role
{
    Technician = 1,
    Engineer = 2,
    Admin = 3
}

/// <summary>
/// 用户模型。
/// </summary>
public class User
{
    public long Id { get; set; }
    public string Username { get; set; }
    public string PasswordHash { get; set; }
    public Role Role { get; set; }
}

/// <summary>
/// 用户与角色管理（SQLite 持久化）。
/// - 首次启动自动建表并创建3个默认用户（管理员/工程师/技术员，密码123456）。
/// - 提供登录/注册/查询接口。
/// </summary>
public sealed class UserManager
{
    private static readonly Lazy<UserManager> _inst = new Lazy<UserManager>(() => new UserManager());
    public static UserManager Instance => _inst.Value;

    private readonly string _dbPath = AuthPath.DbPath;

    /// <summary>
    /// 当前已登录用户（为空表示未登录）。
    /// </summary>
    public User CurrentUser { get; private set; }

    /// <summary>
    /// 用户状态改变事件，登录/注销时触发。
    /// </summary>
    public event Action<User> CurrentUserChanged;

    private UserManager()
    {
        EnsureSchema();
        EnsureDefaults();
    }

    /// <summary>
    /// 创建 Users 表。
    /// </summary>
    private void EnsureSchema()
    {
        using var db = new SqliteDb(_dbPath);
        if (!db.Available) return;
        db.ExecuteNonQuery(@"CREATE TABLE IF NOT EXISTS Users(
Id INTEGER PRIMARY KEY AUTOINCREMENT,
Username TEXT UNIQUE NOT NULL,
PasswordHash TEXT NOT NULL,
Role INTEGER NOT NULL
);");
    }

    /// <summary>
    /// 计算密码哈希，避免明文存储。
    /// </summary>
    private static string HashPassword(string password)
    {
        using var sha = SHA256.Create();
        var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password ?? string.Empty));
        return BitConverter.ToString(bytes).Replace("-", string.Empty).ToLowerInvariant();
    }

    /// <summary>
    /// 校验登录。
    /// </summary>
    public bool Login(string username, string password)
    {
        using var db = new SqliteDb(_dbPath);
        if (!db.Available) return false;
        User user = null;
        db.ExecuteReader("SELECT Id,Username,PasswordHash,Role FROM Users WHERE Username=@u", r =>
        {
            user = new User
            {
                Id = r.GetInt64(0),
                Username = r.GetString(1),
                PasswordHash = r.GetString(2),
                Role = (Role)r.GetInt32(3)
            };
        }, ("@u", username));
        if (user == null) return false;
        if (!string.Equals(user.PasswordHash, HashPassword(password))) return false;
        CurrentUser = user;
        try { CurrentUserChanged?.Invoke(CurrentUser); } catch { }
        return true;
    }

    /// <summary>
    /// 注册新用户，用户名唯一。
    /// </summary>
    public bool Register(string username, string password, Role role, out string error)
    {
        error = null;
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrEmpty(password))
        {
            error = "用户名或密码不能为空";
            return false;
        }
        using var db = new SqliteDb(_dbPath);
        if (!db.Available)
        {
            error = "数据库不可用";
            return false;
        }
        var exists = db.ExecuteScalar("SELECT 1 FROM Users WHERE Username=@u", ("@u", username)) != null;
        if (exists)
        {
            error = "用户名已存在";
            return false;
        }
        var rows = db.ExecuteNonQuery("INSERT INTO Users(Username,PasswordHash,Role) VALUES(@u,@p,@r)",
            ("@u", username), ("@p", HashPassword(password)), ("@r", (int)role));
        return rows > 0;
    }

    /// <summary>
    /// 读取全部用户（用于登录下拉等）。
    /// </summary>
    public List<User> GetAllUsers()
    {
        var list = new List<User>();
        using var db = new SqliteDb(_dbPath);
        if (!db.Available) return list;
        db.ExecuteReader("SELECT Id,Username,PasswordHash,Role FROM Users", r =>
        {
            list.Add(new User
            {
                Id = r.GetInt64(0),
                Username = r.GetString(1),
                PasswordHash = r.GetString(2),
                Role = (Role)r.GetInt32(3)
            });
        });
        return list;
    }

    /// <summary>
    /// 注销当前用户。
    /// </summary>
    public void Logout()
    {
        CurrentUser = null;
        try { CurrentUserChanged?.Invoke(CurrentUser); } catch { }
    }

    /// <summary>
    /// 首次运行插入三个默认用户。
    /// </summary>
    private void EnsureDefaults()
    {
        using var db = new SqliteDb(_dbPath);
        if (!db.Available) return;
        object c = db.ExecuteScalar("SELECT COUNT(1) FROM Users");
        long count = 0;
        try { count = Convert.ToInt64(c ?? 0); } catch { }
        if (count > 0) return;

        void AddDefault(string name, string pwd, Role role)
        {
            db.ExecuteNonQuery("INSERT INTO Users(Username,PasswordHash,Role) VALUES(@u,@p,@r)",
                ("@u", name), ("@p", HashPassword(pwd)), ("@r", (int)role));
        }
        AddDefault("管理员", "admin", Role.Admin);
        AddDefault("工程师", "123456", Role.Engineer);
        AddDefault("技术员", "123", Role.Technician);
    }
}