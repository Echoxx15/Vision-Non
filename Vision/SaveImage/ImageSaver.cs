using System;
using System.Collections.Concurrent;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using Cognex.VisionPro;
using Cognex.VisionPro.ImageFile;
using Vision.Settings;

namespace Vision.SaveImage;

public enum SaveImageType
{
    bmp,
    jpg,
    png,
    cdb
}

public sealed class SaveRequest
{
    /// <summary>
    /// 工位名称
    /// </summary>
    public string Station { get; set; }
    /// <summary>
    /// 运行结果 OK/NG
    /// </summary>
    public string Result { get; set; } // OK/NG
    /// <summary>
    /// 图片
    /// </summary>
    public ICogImage VisionProImage { get; set; } // 用于cdb或位图转换
    /// <summary>
    /// 图片格式 bmp/jpg/png/cdb
    /// </summary>
    public SaveImageType Type { get; set; }
    /// <summary>
    /// 存图比例
    /// </summary>
    public int ScalePercent { get; set; } = 100;
    /// <summary>
    /// 原图或处理图
    /// </summary>
    public bool IsDealImage { get; set; }
    /// <summary>
    /// 保存的图片名
    /// </summary>
    public string ImageName { get; set; }
}

public static class ImageSaver
{
    private static readonly ConcurrentQueue<SaveRequest> _queue = new();
    private static readonly CancellationTokenSource _cts = new();
    private static readonly Thread _worker;
    private static readonly Thread _cleaner;
    private static DateTime _lastClean = DateTime.MinValue;
    private static readonly TimeSpan CleanInterval = TimeSpan.FromMinutes(10); // 最短10分钟清理一次

    static ImageSaver()
    {
        _worker = new Thread(WorkerLoop) { IsBackground = true, Name = "ImageSaverWorker" };
        _worker.Start();

        _cleaner = new Thread(CleanLoop) { IsBackground = true, Name = "ImageCleaner" };
        _cleaner.Start();

        try { FileSettingsManager.SettingsChanged += () => _lastClean = DateTime.MinValue; } catch { }
    }

    public static void Enqueue(SaveRequest req)
    {
        if (req == null) return;
        _queue.Enqueue(req);
    }

    private static void WorkerLoop()
    {
        while (!_cts.IsCancellationRequested)
        {
            try
            {
                if (_queue.TryDequeue(out var r))
                {
                    SaveInternal(r);
                }
                else
                {
                    Thread.Sleep(30);
                }
            }
            catch { Thread.Sleep(100); }
        }
    }

    private static void CleanLoop()
    {
        while (!_cts.IsCancellationRequested)
        {
            try
            {
                var cfg = FileSettingsManager.Current;
                if (cfg.EnableAutoDelete && DateTime.Now - _lastClean >= CleanInterval)
                {
                    DeleteOldInternal();
                    _lastClean = DateTime.Now;
                }
            }
            catch { }
            Thread.Sleep(1000 * 60); // 每分钟检查一次是否到达清理周期
        }
    }

    private static void SaveInternal(SaveRequest r)
    {
        var cfg = FileSettingsManager.Current;
        var root = cfg.SavePath;
        if (string.IsNullOrWhiteSpace(root)) return;

        var date = DateTime.Now.ToString("yyyy-MM-dd");
        var kind = r.IsDealImage ? "Deal" : "Raw";
        var path = Path.Combine(root, date, kind, r.Station ?? "");
        if (!string.IsNullOrWhiteSpace(r.Result) && cfg.SeparateOkNg)
            path = Path.Combine(path, r.Result);
        Directory.CreateDirectory(path);

        var ts = DateTime.Now.ToString("HH_mm_ss_ffff");
        var fileBase = Path.Combine(path, $"{r.ImageName}_{ts}");

        if (r.Type == SaveImageType.cdb)
        {
            try
            {
                var file = fileBase + ".cdb";
                using var imgFile = new CogImageFile();
                imgFile.Open(file, CogImageFileModeConstants.Write);
                imgFile.Append(r.VisionProImage);
                imgFile.Close();
            }
            catch { }
        }
        else
        {
            // 通过位图保存为 bmp/jpg/png
            try
            {
                using var bmp = r.VisionProImage?.ToBitmap();
                if (bmp == null) return;
                var fmt = r.Type switch
                {
                    SaveImageType.jpg => System.Drawing.Imaging.ImageFormat.Jpeg,
                    SaveImageType.png => System.Drawing.Imaging.ImageFormat.Png,
                    _ => System.Drawing.Imaging.ImageFormat.Bmp
                };
                var file = fileBase + "." + r.Type;
                bmp.Save(file, fmt);
            }
            catch { }
        }
    }

    private static bool TryParseDateFolderName(string name, out DateTime day)
    {
        // 支持 yyyy-MM-dd、yyyyMMdd、yyyy_MM_dd
        var formats = new[] { "yyyy-MM-dd", "yyyyMMdd", "yyyy_MM_dd" };
        foreach (var fmt in formats)
        {
            if (DateTime.TryParseExact(name, fmt, CultureInfo.InvariantCulture, DateTimeStyles.None, out day))
                return true;
        }
        return DateTime.TryParse(name, out day);
    }

    private static void DeleteOldInternal()
    {
        var cfg = FileSettingsManager.Current;
        if (!cfg.EnableAutoDelete) return; // 未勾选不自动删除
        var root = cfg.SavePath;
        if (string.IsNullOrWhiteSpace(root) || !Directory.Exists(root)) return;

        try
        {
            var cutoffRaw = DateTime.Now.Date.AddDays(-Math.Max(0, cfg.RawRetentionDays));
            var cutoffDeal = DateTime.Now.Date.AddDays(-Math.Max(0, cfg.DealRetentionDays));

            foreach (var dayDir in Directory.EnumerateDirectories(root))
            {
                var folderName = Path.GetFileName(dayDir);
                if (!TryParseDateFolderName(folderName, out var day)) continue;

                // 按 Raw / Deal 分别判断，避免整天文件夹被直接删除
                var rawDir = Path.Combine(dayDir, "Raw");
                var dealDir = Path.Combine(dayDir, "Deal");
                if (day < cutoffRaw && Directory.Exists(rawDir))
                {
                    try { Directory.Delete(rawDir, true); } catch { }
                }
                if (day < cutoffDeal && Directory.Exists(dealDir))
                {
                    try { Directory.Delete(dealDir, true); } catch { }
                }

                // 清理空的日期目录
                try
                {
                    if (Directory.Exists(dayDir) && !Directory.EnumerateFileSystemEntries(dayDir).Any())
                        Directory.Delete(dayDir, true);
                }
                catch { }
            }
        }
        catch { }
    }
}