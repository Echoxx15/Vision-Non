using System;
using System.Data;
using System.IO;
using System.Reflection;

namespace Vision.Auth.Storage;

/// <summary>
/// 轻量级 SQLite 封装：通过反射加载 System.Data.SQLite，并使用 ADO.NET 接口执行 SQL。
/// 目的：避免显式编译依赖，方便以本地 DLL 部署；接口最小化，足以支持用户/权限持久化。
/// </summary>
public sealed class SqliteDb : IDisposable
{
    private readonly Type _paramType;
    private IDbConnection _conn;

    /// <summary>
    /// 指示 SQLite 是否可用（加载到 DLL 且连接打开成功）。
    /// </summary>
    public bool Available { get; }

    /// <summary>
    /// 打开 SQLite 连接（若不存在会自动创建 Data 目录）。
    /// </summary>
    public SqliteDb(string dbPath)
    {
        var dbPath1 = dbPath;
        try
        {
            var asm = LoadSqliteAssembly();
            if (asm == null) return;
            var connType = asm.GetType("System.Data.SQLite.SQLiteConnection", throwOnError: false);
            _paramType = asm.GetType("System.Data.SQLite.SQLiteParameter", throwOnError: false);
            if (connType == null || _paramType == null) return;
            Directory.CreateDirectory(Path.GetDirectoryName(dbPath1) ?? AppDomain.CurrentDomain.BaseDirectory);
            _conn = (IDbConnection)Activator.CreateInstance(connType, new object[] { $"Data Source={dbPath1};Version=3;" });
            _conn.Open();
            Available = true;
        }
        catch
        {
            Available = false;
        }
    }

    /// <summary>
    /// 尝试从已加载 / GAC / 程序目录加载 System.Data.SQLite.dll。
    /// 优先从 bin/Debug/Sql 或运行目录下的 Sql 子目录加载，便于集中放置 SQLite 相关 DLL。
    /// </summary>
    private static Assembly LoadSqliteAssembly()
    {
        try
        {
            foreach (var a in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (string.Equals(a.GetName().Name, "System.Data.SQLite", StringComparison.OrdinalIgnoreCase))
                    return a;
            }
            return Assembly.Load("System.Data.SQLite");
        }
        catch
        {
            try
            {
                var baseDir = AppDomain.CurrentDomain.BaseDirectory;
                // 优先查找 bin\Debug\Sql 或运行目录下的 Sql 子目录
                var sqlSubDir = Path.Combine(baseDir, "Sql");
                var dllSql = Path.Combine(sqlSubDir, "System.Data.SQLite.dll");
                if (File.Exists(dllSql)) return Assembly.LoadFrom(dllSql);

                // 回退到运行目录根
                var dll = Path.Combine(baseDir, "System.Data.SQLite.dll");
                if (File.Exists(dll)) return Assembly.LoadFrom(dll);
            }
            catch { }
            return null;
        }
    }

    /// <summary>
    /// 执行 INSERT/UPDATE/DELETE 等非查询语句。
    /// </summary>
    public int ExecuteNonQuery(string sql, params (string name, object value)[] parameters)
    {
        try
        {
            using var cmd = _conn.CreateCommand();
            cmd.CommandText = sql;
            if (parameters != null)
            {
                foreach (var p in parameters)
                {
                    var prm = (IDbDataParameter)Activator.CreateInstance(_paramType, new[] { p.name, p.value });
                    cmd.Parameters.Add(prm);
                }
            }
            return cmd.ExecuteNonQuery();
        }
        catch
        {
            return 0;
        }
    }

    /// <summary>
    /// 执行返回单值的查询（如 SELECT COUNT(1) ...）。
    /// </summary>
    public object ExecuteScalar(string sql, params (string name, object value)[] parameters)
    {
        try
        {
            using var cmd = _conn.CreateCommand();
            cmd.CommandText = sql;
            if (parameters != null)
            {
                foreach (var p in parameters)
                {
                    var prm = (IDbDataParameter)Activator.CreateInstance(_paramType, new[] { p.name, p.value });
                    cmd.Parameters.Add(prm);
                }
            }
            return cmd.ExecuteScalar();
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// 执行查询并逐行回调读取器。
    /// </summary>
    public void ExecuteReader(string sql, Action<IDataRecord> rowReader, params (string name, object value)[] parameters)
    {
        try
        {
            using var cmd = _conn.CreateCommand();
            cmd.CommandText = sql;
            if (parameters != null)
            {
                foreach (var p in parameters)
                {
                    var prm = (IDbDataParameter)Activator.CreateInstance(_paramType, new[] { p.name, p.value });
                    cmd.Parameters.Add(prm);
                }
            }

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                rowReader(reader);
            }
        }
        catch
        {
            // 忽略，调用方自行处理无结果情况
        }
    }

    public void Dispose()
    {
        try { _conn?.Close(); } catch { }
        try { _conn?.Dispose(); } catch { }
        _conn = null;
    }
}