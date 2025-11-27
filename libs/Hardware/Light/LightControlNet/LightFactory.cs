using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Serialization;
using LightControlNet.UI;
using Logger;

namespace LightControlNet;

/// <summary>
/// 光源控制工厂（单例）
/// 统一管理所有光源控制器的创建、配置与保存
/// </summary>
public sealed class LightFactory : IDisposable
{
    private static readonly Lazy<LightFactory> _instance = new(() => new LightFactory());

    public static LightFactory Instance => _instance.Value;

    private readonly ConcurrentDictionary<string, ILightController> _controllers = new();
    private LightConfigCollection _configs = new();

    private readonly object _configLock = new();

    public Form GetConfigForm => new Frm_LightConfig();

    private LightFactory()
    {
        // 尝试加载并恢复配置
        try
        {
            LoadConfigs();
        }
        catch (Exception ex)
        {
            LogHelper.Error(ex, "初始化加载光源配置失败");
            _configs = new LightConfigCollection();
        }
    }

    /// <summary>
    /// 配置文件路径
    /// </summary>
    private static string ConfigFilePath
    {
        get
        {
            try
            {
                var dir = AppDomain.CurrentDomain.BaseDirectory;
                return Path.Combine(dir,"Configs", "LightConfigs.xml");
            }
            catch
            {
                return "LightConfigs.xml";
            }
        }
    }

    /// <summary>
    /// 获取当前配置集合（只读）；修改后请调用 SaveConfigs
    /// </summary>
    public LightConfigCollection Configs
    {
        get
        {
            lock (_configLock)
            {
                return _configs;
            }
        }
    }

    /// <summary>
    /// 根据配置集合重新创建控制器
    /// </summary>
    private void InitializeFromConfigs(LightConfigCollection configCollection)
    {
        try
        {
            // 释放所有已有控制器
            DisposeAllControllers();

            if (configCollection?.Configs == null)
            {
                return;
            }

            foreach (var config in configCollection.Configs.Where(c => c is { Enabled: true }))
            {
                try
                {
                    var controller = LightManager.Instance.CreateController(config);
                    if (controller == null) throw new NotSupportedException($"不支持的光源控制器: {config.Type}");

                    if (controller.Open())
                    {
                        _controllers[config.Name] = controller;
                        LogHelper.Info($"光源控制器[{config.Name}]初始化成功");
                    }
                    else
                    {
                        LogHelper.Warn($"光源控制器[{config.Name}]初始化失败");
                        controller.Dispose();
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.Error(ex, $"初始化光源控制器[{config.Name}]失败");
                }
            }

            LogHelper.Info($"光源控制器初始化完成，共成功{_controllers.Count}个实例");
        }
        catch (Exception ex)
        {
            LogHelper.Error(ex, "根据配置初始化光源控制器失败");
        }
    }

    #region 加载与保存

    /// <summary>
    /// 加载配置并重建设备
    /// </summary>
    private void LoadConfigs()
    {
        lock (_configLock)
        {
            try
            {
                if (File.Exists(ConfigFilePath))
                {
                    using var fs = File.OpenRead(ConfigFilePath);
                    var serializer = new XmlSerializer(typeof(LightConfigCollection));
                    var loaded = serializer.Deserialize(fs) as LightConfigCollection;
                    _configs = loaded ?? new LightConfigCollection();
                }
                else
                {
                    _configs = new LightConfigCollection();
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "加载光源配置文件失败");
                _configs = new LightConfigCollection();
            }
        }

        // 加载后自动重建控制器
        InitializeFromConfigs(_configs);
    }

    /// <summary>
    /// 保存当前配置到文件
    /// </summary>
    private void SaveConfigs()
    {
        lock (_configLock)
        {
            try
            {
                var dir = Path.GetDirectoryName(ConfigFilePath);
                if (!string.IsNullOrWhiteSpace(dir) && !Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }

                using var fs = File.Create(ConfigFilePath);
                var serializer = new XmlSerializer(typeof(LightConfigCollection));
                serializer.Serialize(fs, _configs);
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "保存光源配置文件失败");
            }
        }
    }

    #endregion

    #region 对外访问（增删改查）

    /// <summary>
    /// 添加一个配置并尝试创建控制器
    /// </summary>
    public LightConfig AddConfig(LightConfig config)
    {
        if (config == null) return null;
        lock (_configLock)
        {
            _configs.Add(config);
        }

        // 保存文件
        SaveConfigs();

        // 尝试创建控制器
        TryCreateController(config);
        return config;
    }

    /// <summary>
    /// 删除名称对应的配置并关闭控制器
    /// </summary>
    public bool RemoveConfig(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) return false;

        lock (_configLock)
        {
            var toRemove = _configs.FindByName(name);
            if (toRemove == null) return false;
            _configs.Remove(toRemove);
        }

        // 关闭并移除控制器
        if (_controllers.TryRemove(name, out var controller))
        {
            try { controller.Dispose(); } catch (Exception ex) { LogHelper.Error(ex, $"释放控制器[{name}]失败"); }
        }

        SaveConfigs();
        return true;
    }

    /// <summary>
    /// 更新配置（名称匹配）
    /// ⚠️ 注意：仅更新配置文件，不影响当前运行中的控制器
    /// 如果需要应用新配置，请手动断开并重新连接控制器
    /// </summary>
    public bool UpdateConfig(LightConfig updated)
    {
        if (updated == null || string.IsNullOrWhiteSpace(updated.Name)) return false;

        lock (_configLock)
        {
            var existing = _configs.FindByName(updated.Name);
            if (existing == null) return false;

            // 更新配置属性
            existing.Type = updated.Type;
            existing.Enabled = updated.Enabled;
            existing.PortName = updated.PortName;
            existing.BaudRate = updated.BaudRate;
            existing.DataBits = updated.DataBits;
            existing.StopBits = updated.StopBits;
            existing.Parity = updated.Parity;
            existing.ChannelCount = updated.ChannelCount;
            existing.Remark = updated.Remark;
        }

        // ✅ 只保存配置文件，不重建控制器
        // 原因：重建会关闭所有通道，影响正在运行的光源
        SaveConfigs();
        
        LogHelper.Info($"光源配置[{updated.Name}]已保存到文件，当前连接状态保持不变");
        return true;
    }

    /// <summary>
    /// 通过设备名称获取光源控制器实例
    /// </summary>
    public ILightController GetController(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return null;

        _controllers.TryGetValue(name, out var controller);
        return controller;
    }

    /// <summary>
    /// 打开指定名称的控制器（若未创建则按当前配置创建并打开）
    /// </summary>
    public bool ConnectController(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) return false;
        var ctrl = GetController(name);
        if (ctrl != null)
        {
            return ctrl.IsConnected || ctrl.Open();
        }

        var cfg = GetConfig(name);
        if (cfg == null || !cfg.Enabled) return false;
        TryCreateController(cfg);
        ctrl = GetController(name);
        return ctrl?.IsConnected ?? false;
    }

    /// <summary>
    /// 关闭并移除指定名称的控制器
    /// </summary>
    public void DisconnectController(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) return;
        if (_controllers.TryRemove(name, out var controller))
        {
            try { controller.Close(); controller.Dispose(); }
            catch (Exception ex) { LogHelper.Error(ex, $"释放控制器[{name}]失败"); }
        }
    }

    /// <summary>
    /// 获取名称对应的配置
    /// </summary>
    public LightConfig GetConfig(string name)
    {
        lock (_configLock)
        {
            return _configs.FindByName(name);
        }
    }

    /// <summary>
    /// 使用当前配置生效并重建所有控制器
    /// </summary>
    public void ApplyConfigs()
    {
        InitializeFromConfigs(Configs);
    }

    #endregion


    /// <summary>
    /// 释放所有控制器（程序退出时调用）
    /// </summary>
    private void DisposeAllControllers()
    {
        LogHelper.Info("开始释放所有光源控制器...");
        
        foreach (var kvp in _controllers)
        {
            try
            {
                if (kvp.Value != null)
                {
                    // ✅ 释放前先关闭所有通道
                    try
                    {
                        if (kvp.Value.IsConnected)
                        {
                            LogHelper.Info($"正在关闭光源控制器[{kvp.Key}]的所有通道...");
                            kvp.Value.TurnOffAllChannels();
                        }
                    }
                    catch (Exception ex)
                    {
                        LogHelper.Error(ex, $"关闭光源控制器[{kvp.Key}]通道失败");
                    }
                    
                    // 释放资源
                    kvp.Value.Dispose();
                    LogHelper.Info($"光源控制器[{kvp.Key}]已释放");
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, $"释放光源控制器[{kvp.Key}]失败");
            }
        }

        _controllers.Clear();
        LogHelper.Info("所有光源控制器释放完成");
    }

    private void TryCreateController(LightConfig config)
    {
        try
        {
            if (!(config?.Enabled ?? false))
            {
                return;
            }

            var controller = LightManager.Instance.CreateController(config);

            if (controller == null) { LogHelper.Warn($"不支持的光源控制器: {config?.Type}"); return; }

            if (controller.Open())
            {
                _controllers[config.Name] = controller;
                LogHelper.Info($"光源控制器[{config.Name}]初始化成功");
            }
            else
            {
                LogHelper.Warn($"光源控制器[{config.Name}]初始化失败");
                controller.Dispose();
            }
        }
        catch (Exception ex)
        {
            LogHelper.Error(ex, $"创建光源控制器[{config?.Name}]失败");
        }
    }

    private void RebuildControllerFor(string name)
    {
        try
        {
            // 先释放旧实例
            if (_controllers.TryRemove(name, out var old))
            {
                try { old.Dispose(); } catch (Exception ex) { LogHelper.Error(ex, $"释放旧控制器[{name}]失败"); }
            }

            // 再按当前配置重建
            var cfg = GetConfig(name);
            if (cfg != null)
            {
                TryCreateController(cfg);
            }
        }
        catch (Exception ex)
        {
            LogHelper.Error(ex, $"重建控制器[{name}]失败");
        }
    }

    public void Dispose()
    {
        DisposeAllControllers();
    }
}
