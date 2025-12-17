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
            // 计算截止日期：保留N天意味着N天前（含）的数据应删除
            // 例如：保留7天 + 今天12-17，则 12-10 及之前的数据应删除（保留12-11到12-17共7天）
            var cutoffRaw = DateTime.Now.Date.AddDays(-cfg.RawRetentionDays);
            var cutoffDeal = DateTime.Now.Date.AddDays(-cfg.DealRetentionDays);
            
            Logger.LogHelper.Info($"[ImageSaver] 开始清理过期图片，Raw保留{cfg.RawRetentionDays}天(截止{cutoffRaw:yyyy-MM-dd})，Deal保留{cfg.DealRetentionDays}天(截止{cutoffDeal:yyyy-MM-dd})");

            int deletedCount = 0;
            foreach (var dayDir in Directory.EnumerateDirectories(root))
            {
                var folderName = Path.GetFileName(dayDir);
                if (!TryParseDateFolderName(folderName, out var day)) continue;

                // 按 Raw / Deal 分别判断，使用 <= 确保截止日期当天的也被删除
                var rawDir = Path.Combine(dayDir, "Raw");
                var dealDir = Path.Combine(dayDir, "Deal");
                
                if (day <= cutoffRaw && Directory.Exists(rawDir))
                {
                    try 
                    { 
                        Directory.Delete(rawDir, true);
                        deletedCount++;
                        Logger.LogHelper.Info($"[ImageSaver] 已删除过期Raw目录: {rawDir}");
                    } 
                    catch (Exception ex)
                    {
                        Logger.LogHelper.Warn($"[ImageSaver] 删除Raw目录失败: {rawDir}, 原因: {ex.Message}");
                    }
                }
                
                if (day <= cutoffDeal && Directory.Exists(dealDir))
                {
                    try 
                    { 
                        Directory.Delete(dealDir, true);
                        deletedCount++;
                        Logger.LogHelper.Info($"[ImageSaver] 已删除过期Deal目录: {dealDir}");
                    } 
                    catch (Exception ex)
                    {
                        Logger.LogHelper.Warn($"[ImageSaver] 删除Deal目录失败: {dealDir}, 原因: {ex.Message}");
                    }
                }

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
            
            if (deletedCount > 0)
                Logger.LogHelper.Info($"[ImageSaver] 清理完成，共删除{deletedCount}个过期目录");
        }
        catch (Exception ex)
        {
            Logger.LogHelper.Error(ex, "[ImageSaver] 清理过期图片异常");
        }
    }
}
