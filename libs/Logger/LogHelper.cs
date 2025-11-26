#nullable enable
using NLog;
using NLog.Config;
using NLog.Targets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Logger;

public static class LogHelper
{
    public static event Action? LogChanged;
    internal static event Action<LogEntry>? LogAppended;

    private static readonly NLog.Logger logger = LogManager.GetCurrentClassLogger();
    private static readonly List<LogEntry> _cache = new();
    private static readonly object _cacheLock = new object();
    private const int _maxCount = 2000;

    static LogHelper()
    {
        try
        {
            // 优先从基目录加载 NLog.config
            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            var configPath = Path.Combine(baseDir, "NLog.config");
            if (File.Exists(configPath))
            {
                LogManager.LoadConfiguration(configPath);
            }

            // 若未配置任何目标，则启用代码配置到本地文件
            if (LogManager.Configuration == null || !LogManager.Configuration.AllTargets.Any())
            {
                var cfg = new LoggingConfiguration();
                var userDir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                var normalPath = Path.Combine(userDir, "temp", "UV", "Log", "${shortdate}.log");
                var errorPath = Path.Combine(userDir, "temp", "UV", "Log", "Error", "${shortdate}.log");

                var fileTarget = new FileTarget("file")
                {
                    FileName = normalPath,
                    Layout = "${longdate} | ${level:uppercase=true} | ${logger} | ${message} ${exception:format=tostring}"
                };
                var errTarget = new FileTarget("errorfile")
                {
                    FileName = errorPath,
                    Layout = "${longdate} | ${level:uppercase=true} | ${logger} | ${message} ${exception:format=tostring}"
                };
                cfg.AddTarget(fileTarget);
                cfg.AddTarget(errTarget);
                cfg.AddRule(NLog.LogLevel.Info, NLog.LogLevel.Fatal, fileTarget);
                cfg.AddRule(NLog.LogLevel.Error, NLog.LogLevel.Fatal, errTarget);
                LogManager.Configuration = cfg;
            }
        }
        catch { }
    }

    internal static IReadOnlyList<LogEntry> Logs
    {
        get
        {
            lock (_cacheLock)
            {
                return _cache.ToList();
            }
        }
    }

    /// <summary>
    /// 记录信息日志（来源：软件）
    /// </summary>
    public static void Info(string message, string src = "软件")
    {
        logger.Info(message);
        Cache(LogLevel.Info, message, src);
    }

    /// <summary>
    /// 记录警告日志（来源：软件）
    /// </summary>
    public static void Warn(string message, string src = "软件")
    {
        logger.Warn(message);
        Cache(LogLevel.Warn, message, src);
    }

    /// <summary>
    /// 记录错误日志（来源：异常来源）
    /// </summary>
    public static void Error(Exception ex, string message = "")
    {
        if (string.IsNullOrEmpty(message))
            logger.Error(ex);
        else
            logger.Error(ex, message);

        var src = GetExceptionSource(ex);
        Cache(LogLevel.Error, $"[{message}],{ex}", src);
    }

    /// <summary>
    /// 记录致命错误日志（保持原行为）
    /// </summary>
    public static void Fatal(Exception ex, string message = "")
    {
        if (string.IsNullOrEmpty(message))
            logger.Fatal(ex);
        else
            logger.Fatal(ex, message);
    }

    private static void Cache(LogLevel level, string message, string source)
    {
        var entry = new LogEntry
        {
            Time = DateTime.Now,
            Level = level,
            Source = source,
            Message = message
        };

        lock (_cacheLock)
        {
            if (_cache.Count >= _maxCount)
                _cache.RemoveAt(0);
            _cache.Add(entry);
        }

        // 仅在日志实际改变时才触发事件，避免不必要的UI更新
        if (level == LogLevel.Error || level == LogLevel.Fatal)
        {
            LogChanged?.Invoke();
        }

        try { LogAppended?.Invoke(entry); } catch { }
    }

    private static string GetExceptionSource(Exception ex)
    {
        var site = ex.TargetSite;
        var typeName = site?.DeclaringType?.FullName;
        var methodName = site?.Name;
        var src = (!string.IsNullOrWhiteSpace(typeName) && !string.IsNullOrWhiteSpace(methodName))
            ? $"{typeName}.{methodName}"
            : ex.Source;

        return string.IsNullOrWhiteSpace(src) ? (ex.GetType().FullName ?? "异常") : src;
    }

    internal static List<LogEntry> GetLogsByType(LogLevel? level = null)
    {
        lock (_cacheLock)
        {
            if (level == null)
                return _cache.ToList();
            return _cache.Where(l => l.Level == level).ToList();
        }
    }
}

