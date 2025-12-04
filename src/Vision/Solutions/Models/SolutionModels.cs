using Cognex.VisionPro;
using Cognex.VisionPro.ToolBlock;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;
using System.Xml.Serialization;
using Logger;
using Vision.Common;
using Vision.UserUI;

// 添加引用

namespace Vision.Solutions.Models;

[Serializable]
public class GlobalVariableDef
{
 // 全局变量名（区分大小写规则由外部字典控制，此处仅保存原始名称）
 public string Name { get; set; }
 // 全局变量的类型名，使用 TypeValueUtil进行解析，例如：int,double,string,bool,int[],...
 public string TypeName { get; set; } // e.g. int,double,string,bool,int[],...
 //变量用途说明，仅用于显示
 public string Comment { get; set; }
}

/// <summary>
///方案对象（可 XML 序列化）。包含工位列表、显示配置、全局变量等。
/// 注意：运行时字段（如 GlobalValues、DisplayControls）标记为 XmlIgnore，不参与持久化。
/// </summary>
[XmlRoot("Solution")]
public class Solution
{
 [Category("方案"), DisplayName("方案名称")]
 public string Name { get; set; } = "默认方案";

 // 工位集合，序列化为 <Stations><Station>...</Station></Stations>
 [XmlArray("Stations"), XmlArrayItem("Station")]
 public List<StationConfig> Stations { get; set; } = [];

 //方案文件路径，仅运行时使用（指向 .uv 文件）
 [XmlIgnore]
 public string FilePath { get; set; }

 // 显示布局配置（参与序列化）
 public DisplayConfig Display { get; set; } = new();

 // 全局变量定义（参与持久化），仅保存“定义”与注释，不含运行时值
 public List<GlobalVariableDef> Globals { get; set; } = [];

 // ✅ 深度学习模型已移至 DnnInterfaceNet.DnnModelFactory 统一管理
 // 模型配置存储在独立的配置文件中（Configs/DnnModelConfigs.xml）

 // ✅ 通讯设备配置已移至 HardwareCommNet.CommunicationFactory 统一管理
 // 设备配置存储在独立的配置文件中（Configs/Comm/），不再保存在方案文件中
 // Solution 只需要存储 IOTable 配置（哪个工位使用哪个设备）

 //运行时全局变量值（不参与序列化），按名称索引，大小写不敏感
  [XmlIgnore]
  public Dictionary<string, object> GlobalValues { get; set; } = new(StringComparer.OrdinalIgnoreCase);

  // 运行时：各工位最新工具输出缓存 StationName -> (OutputName -> Value)
  [XmlIgnore]
  public Dictionary<string, Dictionary<string, object>> LastOutputs { get; set; } = new(StringComparer.OrdinalIgnoreCase);

 //运行时控件缓存：Key -> ImageDisplay（不参与序列化），由显示配置动态重建
 [XmlIgnore]
 public Dictionary<string, ImageDisplay> DisplayControls { get; set; } = new(StringComparer.OrdinalIgnoreCase);
}

/// <summary>
///方案索引信息（列表项），用于 solutions.index.xml 的轻量索引。
/// 注意：Path 字段现在存储“方案文件夹路径”。
/// 为兼容旧索引，Path 若指向 .uv 文件也能被识别。
/// </summary>
public class SolutionInfo
{
 public string Name { get; set; }
 public string Description { get; set; }
 // 是否为默认启动方案
 public bool Enable { get; set; }
 public DateTime CreateTime { get; set; }
 public DateTime LastModifyTime { get; set; }
 //方案文件夹路径（兼容旧版：也可能是 .uv 文件路径）
 public string Path { get; set; }
}

/// <summary>
///方案管理器（单例）。负责：
/// - 索引文件读取/保存
/// -方案新建/打开/保存
/// -运行时对象与显示控件的初始化
/// - 全局变量读写
/// -解析方案/工位/工具文件路径
/// </summary>
public sealed class SolutionManager
{
    private static readonly Lazy<SolutionManager> _inst = new(() => new SolutionManager());
    public static SolutionManager Instance => _inst.Value;

    // 当前已加载的方案对象
    public Solution Current { get; private set; }

    //方案索引列表（轻量）
    public List<SolutionInfo> Solutions { get; private set; } = new();

    //方案根目录：应用程序所在目录下的 Solutions
    public string SolutionsDir { get; }

    // 索引文件全路径（保存 Solutions 列表）
    private string IndexFile => Path.Combine(SolutionsDir, "solutions.index.xml");

    // 默认方案文件路径（用于第一次启动或没有方案时）
    private string DefaultFile => Path.Combine(SolutionsDir, "默认方案", "默认方案.uv");

    //方案切换事件（例如用于 UI 刷新）
    public event Action CurrentChanged; //方案切换

    private SolutionManager()
    {
        SolutionsDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Solutions");
        Directory.CreateDirectory(SolutionsDir);
    }

    // 全局变量外部访问（非泛型版本）
    public object GetGlobalValue(string name)
    {
        if (Current == null || string.IsNullOrWhiteSpace(name)) return null;
        return Current.GlobalValues != null && Current.GlobalValues.TryGetValue(name, out var v) ? v : null;
    }

    /// <summary>
    /// 静态便捷方法：按名称读取全局变量（object）。
    /// </summary>
    public static object GetGlobal(string name) => Instance?.GetGlobalValue(name);

    /// <summary>
    ///读取全局变量（泛型）。若类型不匹配，则尝试 Convert.ChangeType，失败返回默认值。
    /// </summary>
    public T GetGlobalValue<T>(string name, T defaultValue = default)
    {
        var v = GetGlobalValue(name);
        if (v is T t) return t;
        try
        {
            return (T)Convert.ChangeType(v, typeof(T));
        }
        catch
        {
            return defaultValue;
        }
    }

    /// <summary>
    /// 静态便捷方法：按名称读取全局变量（泛型）。
    /// </summary>
    public static T GetGlobal<T>(string name, T defaultValue = default) =>
        Instance != null ? Instance.GetGlobalValue(name, defaultValue) : defaultValue;

    /// <summary>
    /// 尝试设置全局变量值：校验是否定义、类型是否匹配（或可转换），并写入运行时字典。
    /// </summary>
    public bool TrySetGlobalValue(string name, object value, out string reason)
    {
        reason = null;
        if (Current == null)
        {
            reason = "无当前方案";
            return false;
        }

        var def = Current.Globals?.FirstOrDefault(g => string.Equals(g.Name, name, StringComparison.OrdinalIgnoreCase));
        if (def == null)
        {
            reason = "变量未定义";
            return false;
        }

        var type = ResolveType(def.TypeName);
        if (type == null)
        {
            reason = "变量类型无效";
            return false;
        }

        if (value != null && !type.IsAssignableFrom(value.GetType()))
        {
            try
            {
                value = Convert.ChangeType(value, type);
            }
            catch
            {
                reason = "赋值类型不匹配";
                return false;
            }
        }

        Current.GlobalValues[name] = value;
        return true;
    }

    // 类型解析委托到公共工具类（集中管理类型名与 System.Type 的映射）
    public static Type ResolveType(string name) => TypeValueUtil.ResolveType(name);

    /// <summary>
    /// 启动时确保方案列表可用：
    ///1) 扫描 Solutions目录下所有子文件夹并校验有效性；
    ///2) 若无有效方案则自动创建默认方案并设为默认启动；
    ///3) 若当前未加载，则加载默认启动方案。
    /// </summary>
    public void EnsureLoaded()
    {
        LoadList();
        if (!Solutions.Any())
        {
            // 无本地有效方案：新建一个默认方案
            NewSolution("默认方案", string.Empty, setAsDefault: true);
            SaveList();
        }

        if (Current == null)
        {
            // 按优先顺序尝试加载可用方案，遇到损坏跳过
            foreach (var start in Solutions.OrderByDescending(s => s.Enable))
            {
                var uv = GetUvPath(start);
                if (string.IsNullOrEmpty(uv) || !File.Exists(uv)) continue;
                try
                {
                    Current = Load(uv);
                    break; // 成功加载
                }
                catch (Exception ex)
                {
                    LogHelper.Error(ex, "加载方案失败，已跳过: " + start.Name);
                    //继续尝试下一个方案
                }
            }

            // 如果仍未能加载任何方案，则创建一个内存默认方案但不覆盖磁盘文件
            if (Current == null)
            {
                Current = CreateDefaultInMemory("默认方案");
            }
        }
    }

    /// <summary>
    /// 构建方案列表：总是基于目录扫描，若存在索引文件则仅用于读取描述/默认标记。
    ///仅将“包含有效 .uv 且能成功反序列化”的文件夹作为有效方案。
    /// </summary>
    private void LoadList()
    {
        try
        {
            //读取索引（仅用于附加描述/默认标记）
            var meta = new Dictionary<string, (string desc, bool enable)>(StringComparer.OrdinalIgnoreCase);
            if (File.Exists(IndexFile))
            {
                try
                {
                    var ser = new XmlSerializer(typeof(List<SolutionInfo>));
                    using var fs = File.OpenRead(IndexFile);
                    var list = (List<SolutionInfo>)ser.Deserialize(fs);
                    foreach (var s in list ?? new List<SolutionInfo>())
                    {
                        if (s == null) continue;
                        string folder = null;
                        if (!string.IsNullOrEmpty(s.Path) && Directory.Exists(s.Path)) folder = s.Path;
                        else if (!string.IsNullOrEmpty(s.Path) && File.Exists(s.Path))
                            folder = Path.GetDirectoryName(s.Path);
                        if (!string.IsNullOrEmpty(folder))
                        {
                            meta[folder] = (s.Description ?? string.Empty, s.Enable);
                        }
                    }
                }
                catch
                {
                    meta.Clear();
                }
            }

            // 扫描 Solutions目录的子文件夹
            var result = new List<SolutionInfo>();
            foreach (var dir in Directory.GetDirectories(SolutionsDir))
            {
                var name = Path.GetFileName(dir);
                var uv = GetUvPath(dir, name) ?? Directory.GetFiles(dir, "*.uv").FirstOrDefault();
                if (string.IsNullOrEmpty(uv) || !File.Exists(uv)) continue;
                if (!IsValidUv(uv)) continue; // 无效/损坏，不认为是方案

                var fi = new FileInfo(uv);
                meta.TryGetValue(dir, out var m);
                result.Add(new SolutionInfo
                {
                    Name = name,
                    Description = m.desc,
                    Enable = m.enable,
                    CreateTime = fi.CreationTime,
                    LastModifyTime = fi.LastWriteTime,
                    Path = dir
                });
            }

            // 若没有任何方案，将保留为空并由 EnsureLoaded处理新建
            Solutions = result;

            // 确保存在一个默认启动项
            if (!Solutions.Any(s => s.Enable) && Solutions.Count > 0)
            {
                Solutions[0].Enable = true;
            }

            //立即保存索引（清理无效项）
            SaveList();
        }
        catch
        {
            //任何异常都清空列表，交由 EnsureLoaded处理新建
            Solutions.Clear();
        }
    }

    /// <summary>
    /// 验证 .uv 文件是否为一个可反序列化的方案（不加载 ToolBlock）。
    /// </summary>
    private static bool IsValidUv(string uvPath)
    {
        try
        {
            var ser = new XmlSerializer(typeof(Solution));
            using var fs = File.OpenRead(uvPath);
            var _ = (Solution)ser.Deserialize(fs);
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// 保存方案列表索引文件（不保存具体方案内容）。
    /// </summary>
    public void SaveList()
    {
        try
        {
            Directory.CreateDirectory(SolutionsDir);
            var ser = new XmlSerializer(typeof(List<SolutionInfo>));
            using (var fs = File.Create(IndexFile))
            {
                ser.Serialize(fs, Solutions);
            }
        }
        catch
        {
        }
    }

    /// <summary>
    /// 新建方案：创建默认内存对象 -> 保存为 文件夹/同名.uv -> 更新 Solutions 索引。
    /// setAsDefault 为 true 时，会取消其他默认标记，仅保留一个默认启动方案。
    /// </summary>
    public SolutionInfo NewSolution(string name, string description, bool setAsDefault)
    {
        //规范化文件名
        var safe = SanitizeFileName(name);
        var folder = Path.Combine(SolutionsDir, safe);
        Directory.CreateDirectory(folder);
        var file = Path.Combine(folder, safe + ".uv");


        // 构建内存默认方案（包含一个默认工位，占位）
        var sol = CreateDefaultInMemory(name);

        // 持久化保存（失败则保持最小化文件不为空）
        Save(sol, file);
        var fi = new FileInfo(file);
        var info = new SolutionInfo
        {
            Name = name,
            Description = description ?? string.Empty,
            Enable = setAsDefault,
            CreateTime = fi.CreationTime,
            LastModifyTime = fi.LastWriteTime,
            Path = folder
        };

        // 更新列表，仅允许一个默认启动
        if (setAsDefault)
        {
            foreach (var s in Solutions) s.Enable = false;
        }

        Solutions.Add(info);
        SaveList();
        return info;
    }

    /// <summary>
    /// 打开指定方案并设为 Current，触发 CurrentChanged。
    /// </summary>
    public Solution OpenSolution(SolutionInfo info)
    {
        if (info == null || string.IsNullOrEmpty(info.Path))
            throw new FileNotFoundException("方案文件不存在", info?.Path);
        var uv = GetUvPath(info);
        if (string.IsNullOrEmpty(uv) || !File.Exists(uv))
            throw new FileNotFoundException("方案文件不存在", uv);
        try
        {
            var sol = Load(uv);
            Current = sol;
            CurrentChanged?.Invoke();
            return Current;
        }
        catch (Exception ex)
        {
            LogHelper.Error(ex, $"打开方案失败: {info.Name}");
            throw new InvalidOperationException($"打开方案失败: {info?.Name}", ex);
        }
    }

    /// <summary>
    /// 对 DisplayConfig进行归一化：
    /// - 确保 Items 长度 == Rows*Cols，缺失项使用默认Key/DisplayName
    /// - 保证 Key 唯一（忽略大小写），必要时生成 "显示N"形式的新 Key
    /// - 若 DisplayName为空则用 Key 填充
    /// - **重要**：保留用户已修改的 DisplayName，不要覆盖
    /// 此方法不会修改行列配置，只调整 Items 内容顺序与键值，便于持久化和 UI 一致性。
    /// </summary>
    private static void NormalizeDisplayConfig(DisplayConfig cfg)
    {
        if (cfg == null) return;
        int rows = Math.Max(1, cfg.Rows);
        int cols = Math.Max(1, cfg.Cols);
        int total = rows * cols;
        cfg.Items ??= new List<DisplayItem>();

        // 保留原有项（Key -> DisplayItem）
        var existingByKey = new Dictionary<string, DisplayItem>(StringComparer.OrdinalIgnoreCase);
        foreach (var it in cfg.Items)
        {
            if (it != null && !string.IsNullOrWhiteSpace(it.Key))
            {
                if (!existingByKey.ContainsKey(it.Key))
                {
                    existingByKey[it.Key] = it;
                }
            }
        }

        var result = new List<DisplayItem>(total);
        var usedKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        // 首先，按原有顺序保留存在的项（保留DisplayName）
        for (int i = 0; i < Math.Min(cfg.Items.Count, total); i++)
        {
            var item = cfg.Items[i];
            if (item != null && !string.IsNullOrWhiteSpace(item.Key) && usedKeys.Add(item.Key))
            {
                // 确保DisplayName不为空，但不覆盖已有的
                if (string.IsNullOrWhiteSpace(item.DisplayName))
                {
                    item.DisplayName = item.Key;
                }

                result.Add(new DisplayItem { Key = item.Key, DisplayName = item.DisplayName });
                continue;
            }

            // 如果Key无效或重复，生成新Key但保留DisplayName
            string newKey = $"显示{i + 1}";
            int suffix = i + 1;
            while (usedKeys.Contains(newKey))
            {
                suffix++;
                newKey = $"显示{suffix}";
            }

            usedKeys.Add(newKey);

            // 保留原DisplayName（如果有的话）
            string displayName = item?.DisplayName;
            if (string.IsNullOrWhiteSpace(displayName))
            {
                displayName = newKey;
            }

            result.Add(new DisplayItem { Key = newKey, DisplayName = displayName });
        }

        // 如果Items不足，补充新项
        while (result.Count < total)
        {
            int next = result.Count + 1;
            string key = $"显示{next}";
            while (usedKeys.Contains(key))
            {
                next++;
                key = $"显示{next}";
            }

            usedKeys.Add(key);
            result.Add(new DisplayItem { Key = key, DisplayName = key });
        }

        cfg.Items = result;
    }

    public void RebuildDisplayControls(DisplayConfig cfg)
    {
        if (Current == null || cfg == null) return;

        //归一化配置，但保留用户已修改的 DisplayName
        try
        {
            {
                int rows = Math.Max(1, cfg.Rows);
                int cols = Math.Max(1, cfg.Cols);
                int total = rows * cols;
                cfg.Items ??= new List<DisplayItem>();

                // 确保Items数量正确，但不修改已存在的DisplayName
                while (cfg.Items.Count < total)
                {
                    int index = cfg.Items.Count + 1;
                    var key = $"显示{index}";
                    cfg.Items.Add(new DisplayItem { Key = key, DisplayName = key });
                }

                // 移除多余的项
                if (cfg.Items.Count > total)
                {
                    cfg.Items = cfg.Items.Take(total).ToList();
                }

                // 确保每个项都有Key和DisplayName（但不覆盖已存在的DisplayName）
                for (int i = 0; i < cfg.Items.Count; i++)
                {
                    if (string.IsNullOrWhiteSpace(cfg.Items[i].Key))
                    {
                        cfg.Items[i].Key = $"显示{i + 1}";
                    }

                    if (string.IsNullOrWhiteSpace(cfg.Items[i].DisplayName))
                    {
                        cfg.Items[i].DisplayName = cfg.Items[i].Key;
                    }
                }
            }
        }
        catch
        {
        }

        // 清理旧控件引用（交给主界面从父容器移除，这里仅释放引用）
        Current.DisplayControls.Clear();
        var items = cfg.Items ?? new List<DisplayItem>();
        foreach (var di in items)
        {
            var name = string.IsNullOrWhiteSpace(di.DisplayName) ? di.Key : di.DisplayName;
            var img = new ImageDisplay(name) { Dock = DockStyle.Fill, Margin = new Padding(2) };
            if (!string.IsNullOrWhiteSpace(di.Key)) Current.DisplayControls[di.Key] = img;
        }

        // 把规范化后的配置写回 Current，确保保存时一致
        Current.Display = cfg;

        CurrentChanged?.Invoke();
    }

    public void Save(Solution sol, string file)
    {
        string tempFile = null;
        string bakFile = null;

        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(file) ?? string.Empty);

            // 在保存前归一化显示配置以保证持久化一致性
            try
            {
                if (sol?.Display != null) NormalizeDisplayConfig(sol.Display);
            }
            catch
            {
            }

            // === 第1步：先保存到临时文件 ===
            tempFile = file + ".tmp";
            try
            {
                var ser = new XmlSerializer(typeof(Solution));
                using (var fs = File.Create(tempFile))
                {
                    if (sol != null) ser.Serialize(fs, sol);
                }

                LogHelper.Info($"方案序列化成功: {Path.GetFileName(file)}");
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, $"方案序列化失败: {Path.GetFileName(file)}");
                LogHelper.Warn(" →方案保存中止，原文件保持不变");

                // 清理临时文件
                try
                {
                    if (File.Exists(tempFile)) File.Delete(tempFile);
                }
                catch
                {
                }

                throw; // 序列化失败不继续保存
            }

            // === 第2步：备份原文件（如果存在） ===
            if (File.Exists(file))
            {
                try
                {
                    bakFile = file + ".bak";
                    File.Copy(file, bakFile, true);
                }
                catch (Exception ex)
                {
                    LogHelper.Warn($"备份原文件失败: {ex.Message}");
                    //不中断保存流程
                }
            }

            // === 第3步：原子替换：临时文件 → 正式文件 ===
            try
            {
                File.Copy(tempFile, file, true);
                File.Delete(tempFile);
                sol.FilePath = file;
                LogHelper.Info($"方案文件写入成功: {Path.GetFileName(file)}");
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, $" 文件替换失败: {Path.GetFileName(file)}");

                // 尝试恢复备份
                if (bakFile != null && File.Exists(bakFile))
                {
                    try
                    {
                        File.Copy(bakFile, file, true);
                        LogHelper.Info(" → 已从备份恢复原文件");
                    }
                    catch
                    {
                    }
                }

                throw;
            }

            // === 第4步：保存 VPP 文件（容错保存） ===
            try
            {
                SaveVppFile(sol);
            }
            catch (Exception ex)
            {
                LogHelper.Warn($" VPP文件保存时发生异常: {ex.Message}");
                LogHelper.Warn(" →方案配置已保存，但部分工具文件可能未更新");
                //不中断，VPP失败不影响方案可用性
            }

            // === 第5步：清理备份文件 ===
            if (bakFile != null && File.Exists(bakFile))
            {
                try
                {
                    File.Delete(bakFile);
                }
                catch
                {
                } // 清理失败不重要
            }
        }
        catch (Exception e)
        {
            LogHelper.Error(e, $"保存方案失败: {sol?.Name}");

            // 清理临时文件
            try
            {
                if (tempFile != null && File.Exists(tempFile)) File.Delete(tempFile);
            }
            catch
            {
            }
        }
    }

    /// <summary>
    /// 加载方案：
    ///1)反序列化 .uv（XML）;
    ///2) 初始化显示配置、全局变量默认值;
    ///3) 从磁盘加载各工位的 .vpp 和 ToolBlock;
    ///4) 重建显示控件集合。
    ///失败时返回内存默认方案，保证程序可继续运行。
    /// </summary>
    private Solution Load(string file)
    {
        try
        {
            // === 第1步：反序列化方案文件 ===
            Solution sol;
            try
            {
                var ser = new XmlSerializer(typeof(Solution));
                using var fs = File.OpenRead(file);
                sol = (Solution)ser.Deserialize(fs);
                sol.FilePath = file;

            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, $"方案文件反序列化失败: {Path.GetFileName(file)}");
                //备份损坏的文件，以便人工恢复
                try
                {
                    var bakFile = file + $".bak.{DateTime.Now:yyyyMMddHHmmss}";
                    File.Copy(file, bakFile, true);
                    LogHelper.Info($"已备份损坏方案为: {Path.GetFileName(bakFile)}");
                }
                catch
                {
                }

                // 抛出异常，不在此处用默认对象替换磁盘文件或 Current
                throw new InvalidDataException($"方案文件反序列化失败: {file}", ex);
            }

            // === 第2步：初始化配置结构 ===
            try
            {
                sol.Display ??= new DisplayConfig();
                sol.Display.Items ??= new List<DisplayItem>();

                // 去重：按 Key（忽略大小写）保留第一个
                if (sol.Display.Items.Count > 1)
                {
                    sol.Display.Items = sol.Display.Items
                        .Where(i => i != null && (!string.IsNullOrWhiteSpace(i.Key) ||
                                                  !string.IsNullOrWhiteSpace(i.DisplayName)))
                        .GroupBy(i => string.IsNullOrWhiteSpace(i.Key) ? (i.DisplayName ?? "") : i.Key,
                            StringComparer.OrdinalIgnoreCase)
                        .Select(g => g.First())
                        .ToList();
                }

                sol.Globals ??= new List<GlobalVariableDef>();
                sol.GlobalValues = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
                sol.LastOutputs = new Dictionary<string, Dictionary<string, object>>(StringComparer.OrdinalIgnoreCase);
                foreach (var g in sol.Globals)
                {
                    if (string.IsNullOrWhiteSpace(g?.Name)) continue;
                    if (!sol.GlobalValues.ContainsKey(g.Name)) sol.GlobalValues[g.Name] = null;
                }

                // 根据 StationEnum 同步工位（替代原来的手动确保逻辑）
                sol.Stations ??= new List<StationConfig>();
                //SyncStationsFromEnum(sol); // 已移除枚举工位自动同步，工位仅来源于方案文件和界面操作
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "方案配置初始化异常");
                //不中断，继续执行
            }

            // === 第3步：加载工具和模型（容错加载） ===
            try
            {
                LoadVppFile(sol);
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, " 加载VPP文件时发生异常");
                LogHelper.Warn(" → 部分工具可能未正确加载");
                //不中断，继续执行
            }

            // === 第4步：重建显示控件 ===
            try
            {
                Current = sol;
                RebuildDisplayControls(sol.Display);
            }
            catch (Exception ex)
            {
                LogHelper.Warn($" 重建显示控件异常: {ex.Message}");
                LogHelper.Warn(" → 显示功能可能受影响");
                //不中断，继续执行
            }

            return sol;
        }
        catch (Exception ex)
        {
            LogHelper.Error(ex, $"方案加载严重异常: {Path.GetFileName(file)}");
            // 不在这里返回默认方案，改为抛出异常给调用方处理
            throw;
        }
    }

    #region 保存 VPP

    private void SaveVppFile(Solution sol)
    {
        if (sol?.Stations == null) return;
        var solutionFolder = GetSolutionFolder(sol);
        if (string.IsNullOrEmpty(solutionFolder)) return;

        foreach (var st in sol.Stations.Where(st => st != null))
        {
            try
            {
                var stationDir = GetStationFolder(sol, st);
                Directory.CreateDirectory(stationDir);

                // 保存三个工具（棋盘格、九点、检测）
                SaveSingleTool(sol, st, st.CheckerboardTool, "棋盘格标定");
                SaveSingleTool(sol, st, st.NPointTool, "九点标定");
                SaveSingleTool(sol, st, st.DetectionTool, "检测");
            }
            catch (Exception ex)
            {
                LogHelper.Warn($"工位[{st?.Name}]保存异常: {ex.Message}");
            }
        }
    }

    /// <summary>
    /// 保存单个工具的 VPP 和变量文件
    /// </summary>
    private static void SaveSingleTool(Solution sol, StationConfig st, StationConfig.Tools tool, string toolName)
    {
        if (tool?.ToolBlock == null) return;

        var path = GetToolVppPath(sol, st, tool);
        if (string.IsNullOrEmpty(path)) return;

        try
        {
            CogSerializer.SaveObjectToFile(tool.ToolBlock, path,
                typeof(BinaryFormatter), CogSerializationOptionsConstants.Minimum);
        }
        catch (Exception ex)
        {
            LogHelper.Warn($"工位[{st.Name}]{toolName}工具保存失败: {ex.Message}");
        }

        try
        {
            SaveToolVarsSidecar(sol, st, tool);
        }
        catch (Exception ex)
        {
            LogHelper.Warn($"工位[{st.Name}]{toolName}工具变量保存失败: {ex.Message}");
        }
    }

    #endregion

    #region 加载 VPP

    /// <summary>
    ///反序列化后恢复运行时对象：
    /// - 从磁盘加载各工具的 .vpp（基于方案/工位目录结构）;
    /// - 加载深度学习模型（如果工位启用了模型加载）
    /// 注意：采用容错加载策略，单个组件失败不影响整体加载
    /// </summary>
    private static void LoadVppFile(Solution sol)
    {
        if (sol?.Stations == null) return;
        var solutionFolder = GetSolutionFolder(sol);
        if (string.IsNullOrEmpty(solutionFolder)) return;

        int successCount = 0;
        int failCount = 0;

        foreach (var st in sol.Stations.Where(st => st != null))
        {
            try
            {
                var stationDir = GetStationFolder(sol, st);
                Directory.CreateDirectory(stationDir);

                // 加载三个工具（棋盘格、九点、检测）
                if (LoadSingleTool(sol, st, st.CheckerboardTool, "棋盘格标定")) successCount++;
                else if (st.bCalibCheckboardTool) failCount++;
                if (LoadSingleTool(sol, st, st.NPointTool, "九点标定")) successCount++;
                else if (st.bCalibNPointTool) failCount++;
                if (LoadSingleTool(sol, st, st.DetectionTool, "检测")) successCount++;
                else failCount++;
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, $"工位[{st?.Name}]加载异常");
                failCount++;
            }
        }

        if (successCount > 0 || failCount > 0)
        {
            if (failCount > 0)
            {
                LogHelper.Info($"方案[{sol.Name}]加载完成: 成功 {successCount} 项,失败 {failCount} 项");
            }
            else
            {
                LogHelper.Info($"方案[{sol.Name}]加载完成: 成功 {successCount} 项");
            }
        }
    }

    /// <summary>
    /// 加载单个工具的 VPP 和变量文件
    /// </summary>
    /// <returns>是否成功加载</returns>
    private static bool LoadSingleTool(Solution sol, StationConfig st, StationConfig.Tools tool, string toolName)
    {
        if (tool == null) return false;

        var path = GetToolVppPath(sol, st, tool);
        if (string.IsNullOrEmpty(path) || !File.Exists(path)) return false;

        try
        {
            tool.ToolBlock = CogSerializer.LoadObjectFromFile(path) as CogToolBlock;
        }
        catch (Exception ex)
        {
            LogHelper.Warn($"工位[{st.Name}]{toolName}工具加载失败: {ex.Message}");
            return false;
        }

        try
        {
            LoadToolVarsSidecar(sol, st, tool);
        }
        catch (Exception ex)
        {
            LogHelper.Warn($"工位[{st.Name}]{toolName}工具变量加载失败: {ex.Message}");
        }

        return true;
    }

    #endregion

    #region 路径辅助

    /// <summary>
    /// 获取方案文件夹。
    /// </summary>
    public static string GetSolutionFolder(Solution sol)
    {
        try
        {
            return string.IsNullOrEmpty(sol?.FilePath) ? null : Path.GetDirectoryName(sol.FilePath);
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// 获取工位文件夹路径：Solutions/方案/工位名称。
    /// </summary>
    public static string GetStationFolder(Solution sol, StationConfig st)
    {
        var root = GetSolutionFolder(sol);
        if (string.IsNullOrEmpty(root)) return null;
        return Path.Combine(root, SanitizeFileName(st?.Name ?? "Station"));
    }

    /// <summary>
    /// 获取工具的 .vpp 完整路径。
    /// </summary>
    public static string GetToolVppPath(Solution sol, StationConfig st, StationConfig.Tools tool)
    {
        var folder = GetStationFolder(sol, st);
        if (string.IsNullOrEmpty(folder)) return null;
        var file = tool?.FileName;
        if (string.IsNullOrWhiteSpace(file)) return null;
        return Path.Combine(folder, file);
    }

    /// <summary>
    /// 获取工具变量侧车文件完整路径（与 .vpp 同名，扩展名改为 .vars.xml）。
    /// </summary>
    public static string GetToolVarsPath(Solution sol, StationConfig st, StationConfig.Tools tool)
    {
        var folder = GetStationFolder(sol, st);
        if (string.IsNullOrEmpty(folder)) return null;
        var file = tool?.FileName;
        if (string.IsNullOrWhiteSpace(file)) return null;
        var stem = Path.GetFileNameWithoutExtension(file);
        return Path.Combine(folder, stem + ".vars.xml");
    }

    /// <summary>
    /// 从 SolutionInfo 获取 .uv 路径：Path可能为文件夹或 .uv 文件。
    /// </summary>
    public static string GetUvPath(SolutionInfo info)
    {
        if (info == null || string.IsNullOrEmpty(info.Path)) return null;
        if (Directory.Exists(info.Path)) return GetUvPath(info.Path, info.Name);
        if (File.Exists(info.Path)) return info.Path;
        return null;
    }

    /// <summary>
    /// 从文件夹和方案名构建 .uv 路径，优先同名，否则取第一个 .uv。
    /// </summary>
    private static string GetUvPath(string folder, string solutionName)
    {
        try
        {
            if (string.IsNullOrEmpty(folder)) return null;
            var safe = SanitizeFileName(solutionName ?? "");
            var target = Path.Combine(folder, safe + ".uv");
            if (File.Exists(target)) return target;
            var any = Directory.GetFiles(folder, "*.uv").FirstOrDefault();
            return any;
        }
        catch
        {
            return null;
        }
    }

    #endregion

    #region 工具

    /// <summary>
    ///规范化文件名：将非法文件名字符替换为下划线。
    /// </summary>
    private static string SanitizeFileName(string name)
    {
        var invalid = Path.GetInvalidFileNameChars();
        foreach (var c in invalid) name = name.Replace(c, '_');
        return name;
    }


    /// <summary>
    /// 创建一个内存中的默认方案对象（不持久），
    /// 用于加载失败的回退或新建空白方案。保证程序可继续运行。
    /// </summary>
    private static Solution CreateDefaultInMemory(string name)
    {
        var sol = new Solution { Name = name };
        sol.Stations.Add(new StationConfig
        {
            Name = "工位1",
            Enable = true,
            SN = string.Empty,
            CameraType = string.Empty,
            bShow = false,
            DisplayName = string.Empty,
            RecoredIndex = 0
        });
        return sol;
    }

    #endregion

    #region 工位同步

    // 工位同步逻辑已移除，使用 UI 管理工位增删改。

    #endregion

    #region 辅助

    /// <summary>
    /// 保存当前方案并更新索引中的修改时间。
    /// </summary>
    public void SaveCurrent()
    {
        if (Current == null) return;
        Save(Current, Current.FilePath ?? DefaultFile);
        var uv = Current.FilePath ?? DefaultFile;
        var info = Solutions.FirstOrDefault(s => string.Equals(GetUvPath(s), uv, StringComparison.OrdinalIgnoreCase));
        if (info != null)
        {
            info.LastModifyTime = DateTime.Now;
            SaveList();
        }
    }

    /// <summary>
    /// 用于克隆、复制方案，不设置为 Current，也不触发事件。
    /// </summary>
    public Solution LoadForClone(string file)
    {
        var ser = new XmlSerializer(typeof(Solution));
        using var fs = File.OpenRead(file);
        var sol = (Solution)ser.Deserialize(fs);
        sol.Display ??= new DisplayConfig();
        sol.Display.Items ??= new List<DisplayItem>();
        LoadVppFile(sol);
        return sol;
    }

    #endregion

    #region 变量侧车文件（保存/加载）

    /// <summary>
    /// 保存工具的 Vars 到工位目录下与 VPP 同名的 .vars.xml 文件。
    /// 当 Vars为空时会删除已存在的侧车文件，避免遗留旧配置。
    /// </summary>
    private static void SaveToolVarsSidecar(Solution sol, StationConfig st, StationConfig.Tools tool)
    {
        if (tool == null) return;
        var varsPath = GetToolVarsPath(sol, st, tool);
        if (string.IsNullOrEmpty(varsPath)) return;

        var list = tool.Vars ?? new List<StationConfig.DetectVarDef>();
        try
        {
            // 若为空则删除旧文件
            if (list.Count == 0)
            {
                if (File.Exists(varsPath))
                {
                    try
                    {
                        File.Delete(varsPath);
                    }
                    catch
                    {
                    }
                }

                return;
            }

            var dir = Path.GetDirectoryName(varsPath);
            if (!string.IsNullOrEmpty(dir)) Directory.CreateDirectory(dir);

            var ser = new XmlSerializer(typeof(List<StationConfig.DetectVarDef>));
            using (var fs = File.Create(varsPath))
            {
                ser.Serialize(fs, list);
            }
        }
        catch (Exception ex)
        {
            // 将异常抛给调用方记录日志
            throw new InvalidOperationException($"保存变量文件失败: {Path.GetFileName(varsPath)}", ex);
        }
    }

    /// <summary>
    /// 从工位目录加载工具的 Vars（若存在）并覆盖当前 Vars。
    /// </summary>
    private static void LoadToolVarsSidecar(Solution sol, StationConfig st, StationConfig.Tools tool)
    {
        if (tool == null) return;
        var varsPath = GetToolVarsPath(sol, st, tool);
        if (string.IsNullOrEmpty(varsPath) || !File.Exists(varsPath)) return; // 无文件则保持 .uv 中的内容

        try
        {
            var ser = new XmlSerializer(typeof(List<StationConfig.DetectVarDef>));
            using var fs = File.OpenRead(varsPath);
            var list = (List<StationConfig.DetectVarDef>)ser.Deserialize(fs);
            tool.Vars = list ?? new List<StationConfig.DetectVarDef>();
        }
        catch (Exception ex)
        {
            // 将异常抛给调用方记录日志
            throw new InvalidOperationException($"加载变量文件失败: {Path.GetFileName(varsPath)}", ex);
        }
    }

    #endregion
}
