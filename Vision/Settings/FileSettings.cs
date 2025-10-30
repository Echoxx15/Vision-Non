using System;
using System.IO;
using System.Xml.Serialization;
using Vision.SaveImage;

namespace Vision.Settings;

[Serializable]
public class FileSettings
{
    /// <summary>
    /// 存图根目录
    /// </summary>
    public string SavePath { get; set; } = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images");
    /// <summary>
    /// 是否保存原图
    /// </summary>
    public bool SaveRawImage { get; set; } = false;
    /// <summary>
    /// 是否保存处理图
    /// </summary>
    public bool SaveDealImage { get; set; } = false;
    /// <summary>
    /// 是否区分OK\NG
    /// </summary>
    public bool SeparateOkNg { get; set; } = true;

    public SaveImageType RawImageType { get; set; } = SaveImageType.png; // bmp|jpg|png|cdb
    public SaveImageType DealImageType { get; set; } = SaveImageType.jpg;
    /// <summary>
    /// 原图保存天数
    /// </summary>
    public int RawRetentionDays { get; set; } = 7;
    /// <summary>
    /// 处理图保存天数
    /// </summary>
    public int DealRetentionDays { get; set; } = 7;
    /// <summary>
    /// 是否自动删除图片
    /// </summary>
    public bool EnableAutoDelete { get; set; } = false;

    // 磁盘监控
    public bool EnableDiskCheck { get; set; } = false;
    public int DiskThresholdMB { get; set; } = 3000; // M
    public TimeSpan PollTime1 { get; set; } = new TimeSpan(8, 0, 0);
    public TimeSpan PollTime2 { get; set; } = new TimeSpan(20, 0, 0);
}

public static class FileSettingsManager
{
    private static readonly string ConfigDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Configs");
    private static readonly string ConfigPath = Path.Combine(ConfigDir, "file.settings.xml");

    private static FileSettings _current;
    public static FileSettings Current
    {
        get
        {
            if (_current == null) Load();
            return _current;
        }
    }

    public static event Action SettingsChanged;

    public static void Load()
    {
        try
        {
            if (File.Exists(ConfigPath))
            {
                var xs = new XmlSerializer(typeof(FileSettings));
                using var fs = File.OpenRead(ConfigPath);
                _current = xs.Deserialize(fs) as FileSettings ?? new FileSettings();
            }
            else
            {
                _current = new FileSettings();
                Save();
            }
        }
        catch
        {
            _current = new FileSettings();
        }
    }

    public static void Save()
    {
        try
        {
            Directory.CreateDirectory(ConfigDir);
            var xs = new XmlSerializer(typeof(FileSettings));
            using var fs = File.Create(ConfigPath);
            xs.Serialize(fs, _current ?? new FileSettings());
            SettingsChanged?.Invoke();
        }
        catch { }
    }

    public static void Update(FileSettings s)
    {
        _current = s ?? new FileSettings();
        Save();
    }
}