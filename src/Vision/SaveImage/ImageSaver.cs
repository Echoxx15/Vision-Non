using System;
using System.Collections.Concurrent;
using System.Drawing.Imaging;
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
    /// 完整保存路径（优先使用）
    /// </summary>
    public string FullPath { get; set; }
    
    /// <summary>
    /// 工位名称（兼容旧逻辑）
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
    /// 图片名
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
                    SaveInternal(r);
                else
                    Thread.Sleep(30);
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
        string filePath;
        
        if (!string.IsNullOrWhiteSpace(r.FullPath))
        {
            filePath = r.FullPath;
            var dir = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(dir)) Directory.CreateDirectory(dir);
        }
        else
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
            filePath = Path.Combine(path, $"{r.ImageName}_{ts}.{r.Type}");
        }

        // 根据图像类型选择最优保存方式
        if (r.Type == SaveImageType.cdb)
        {
            // CDB 格式：使用 VisionPro 原生保存
            SaveAsCdb(r.VisionProImage, filePath);
        }
        else if (r.VisionProImage is CogImage8Grey)
        {
            // 灰度图：使用 VisionPro ImageFile 保存（保持原始格式，文件最小）
            SaveGreyImageOptimized(r.VisionProImage, filePath, r.Type);
        }
        else
        {
            // 彩色图：使用压缩格式保存
            SaveColorImageOptimized(r.VisionProImage, filePath, r.Type);
        }
    }

    /// <summary>
    /// 保存为 CDB 格式（VisionPro 原生格式）
    /// </summary>
    private static void SaveAsCdb(ICogImage image, string filePath)
    {
        try
        {
            if (!filePath.EndsWith(".cdb", StringComparison.OrdinalIgnoreCase))
                filePath = Path.ChangeExtension(filePath, ".cdb");
            using var imgFile = new CogImageFile();
            imgFile.Open(filePath, CogImageFileModeConstants.Write);
            imgFile.Append(image);
            imgFile.Close();
        }
        catch { }
    }

    /// <summary>
    /// 保存灰度图：使用 VisionPro ImageFile（保持 8bpp 格式，文件最小）
    /// </summary>
    private static void SaveGreyImageOptimized(ICogImage image, string filePath, SaveImageType type)
    {
        try
        {
            // 对于灰度图，使用 CogImageFile 保存可以保持 8bpp 格式
            // 根据扩展名自动选择格式（.bmp/.png/.jpg）
            using var imageFile = new CogImageFile();
            
            // 确保文件路径有正确的扩展名
            var ext = Path.GetExtension(filePath);
            if (string.IsNullOrEmpty(ext) || !ext.Equals($".{type}", StringComparison.OrdinalIgnoreCase))
            {
                filePath = Path.ChangeExtension(filePath, type.ToString());
            }
            
            // CogImageFile 会根据文件扩展名自动选择保存格式
            // .bmp -> BMP (8bpp grayscale)
            // .png -> PNG (8bpp grayscale, compressed)
            // .jpg -> JPEG (grayscale)
            imageFile.Open(filePath, CogImageFileModeConstants.Write);
            imageFile.Append(image);
            imageFile.Close();
        }
        catch
        {
            // 备用方案：使用 Bitmap（虽然会转为 24bpp，但至少能保存）
            try
            {
                using var bmp = image?.ToBitmap();
                if (bmp == null) return;
                var fmt = type switch
                {
                    SaveImageType.jpg => ImageFormat.Jpeg,
                    SaveImageType.png => ImageFormat.Png,
                    _ => ImageFormat.Bmp
                };
                bmp.Save(filePath, fmt);
            }
            catch { }
        }
    }

    /// <summary>
    /// 保存彩色图：使用高质量压缩格式
    /// PNG：无损压缩，文件较小
    /// JPEG：有损压缩，质量 95%，文件最小
    /// </summary>
    private static void SaveColorImageOptimized(ICogImage image, string filePath, SaveImageType type)
    {
        try
        {
            using var bmp = image?.ToBitmap();
            if (bmp == null) return;
            
            switch (type)
            {
                case SaveImageType.jpg:
                    // JPEG：使用高质量压缩（95%），平衡质量和文件大小
                    var jpegEncoder = GetEncoder(ImageFormat.Jpeg);
                    if (jpegEncoder != null)
                    {
                        var encoderParams = new EncoderParameters(1);
                        encoderParams.Param[0] = new EncoderParameter(Encoder.Quality, 95L); // 95% 质量
                        bmp.Save(filePath, jpegEncoder, encoderParams);
                    }
                    else
                    {
                        bmp.Save(filePath, ImageFormat.Jpeg);
                    }
                    break;
                    
                case SaveImageType.png:
                    // PNG：无损压缩，文件小于 BMP，质量不损失
                    bmp.Save(filePath, ImageFormat.Png);
                    break;
                    
                default:
                    // BMP：无压缩，文件最大
                    bmp.Save(filePath, ImageFormat.Bmp);
                    break;
            }
        }
        catch { }
    }

    /// <summary>
    /// 获取图像编码器（用于设置 JPEG 质量）
    /// </summary>
    private static ImageCodecInfo GetEncoder(ImageFormat format)
    {
        try
        {
            var codecs = ImageCodecInfo.GetImageDecoders();
            foreach (var codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                    return codec;
            }
        }
        catch { }
        return null;
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
            // 按“最后写入时间”判断是否过期，避免仅凭日期目录名导致误删
            // Raw/Deal 各自独立保留策略
            var rawCutoff = DateTime.Now.AddDays(-cfg.RawRetentionDays);
            var dealCutoff = DateTime.Now.AddDays(-cfg.DealRetentionDays);

            Logger.LogHelper.Info($"[ImageSaver] 开始清理过期图片，Raw保留{cfg.RawRetentionDays}天(截止{rawCutoff:yyyy-MM-dd HH:mm:ss})，Deal保留{cfg.DealRetentionDays}天(截止{dealCutoff:yyyy-MM-dd HH:mm:ss})");

            int deletedDirCount = 0;

            // 遍历 root 下的日期目录（yyyy-MM-dd 等），但不再直接删除日期目录
            foreach (var dayDir in Directory.EnumerateDirectories(root))
            {
                var folderName = Path.GetFileName(dayDir);
                if (!TryParseDateFolderName(folderName, out _)) continue;

                // 逐层清理 dayDir/Raw/* 及 dayDir/Deal/*
                deletedDirCount += DeleteExpiredSubFolders(Path.Combine(dayDir, "Raw"), rawCutoff);
                deletedDirCount += DeleteExpiredSubFolders(Path.Combine(dayDir, "Deal"), dealCutoff);

                // 清理空的日期目录
                try
                {
                    if (Directory.Exists(dayDir) && !Directory.EnumerateFileSystemEntries(dayDir).Any())
                    {
                        Directory.Delete(dayDir, true);
                        Logger.LogHelper.Info($"[ImageSaver] 已删除空目录: {dayDir}");
                    }
                }
                catch { }
            }

            if (deletedDirCount > 0)
                Logger.LogHelper.Info($"[ImageSaver] 清理完成，共删除{deletedDirCount}个过期目录");
        }
        catch (Exception ex)
        {
            Logger.LogHelper.Error(ex, "[ImageSaver] 清理过期图片异常");
        }
    }

    /// <summary>
    /// 删除指定目录下“最后写入时间早于 cutoff”的子目录。
    /// 例如：root/yyyy-MM-dd/Raw/工位/OK 这种层级下，会递归扫描并删除叶子目录。
    /// </summary>
    private static int DeleteExpiredSubFolders(string parentDir, DateTime cutoff)
    {
        if (string.IsNullOrWhiteSpace(parentDir) || !Directory.Exists(parentDir)) return 0;

        int deletedCount = 0;

        try
        {
            // 递归遍历，优先删除最深层目录（通常是 OK/NG 或工位目录）
            foreach (var dir in Directory.EnumerateDirectories(parentDir, "*", SearchOption.AllDirectories)
                                         .OrderByDescending(p => p.Length))
            {
                try
                {
                    if (!Directory.Exists(dir)) continue;

                    // 以目录最后写入时间作为“最新图片时间”的近似值（创建/写入文件会更新）
                    var lastWrite = Directory.GetLastWriteTime(dir);
                    if (lastWrite >= cutoff) continue;

                    Directory.Delete(dir, true);
                    deletedCount++;
                    Logger.LogHelper.Info($"[ImageSaver] 已删除过期目录: {dir} (LastWrite={lastWrite:yyyy-MM-dd HH:mm:ss})");
                }
                catch (Exception ex)
                {
                    Logger.LogHelper.Warn($"[ImageSaver] 删除目录失败: {dir}, 原因: {ex.Message}");
                }
            }

            // 删除 parentDir 下已空的子目录（从浅到深清理）
            foreach (var dir in Directory.EnumerateDirectories(parentDir, "*", SearchOption.AllDirectories)
                                         .OrderByDescending(p => p.Length))
            {
                try
                {
                    if (Directory.Exists(dir) && !Directory.EnumerateFileSystemEntries(dir).Any())
                    {
                        Directory.Delete(dir, true);
                    }
                }
                catch { }
            }

            // parentDir 本身若为空也清理
            try
            {
                if (Directory.Exists(parentDir) && !Directory.EnumerateFileSystemEntries(parentDir).Any())
                {
                    Directory.Delete(parentDir, true);
                }
            }
            catch { }
        }
        catch { }

        return deletedCount;
    }
}
