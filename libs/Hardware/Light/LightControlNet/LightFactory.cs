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
/// 光源管理器（单例模式）
///统一管理所有光源控制器、配置的加载与保存
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
        // 尝试加载本地配置
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
    /// 获取当前配置集合（引用）。调用方谨慎修改后请调用 SaveConfigs。
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
    ///重新根据配置集合创建控制器
    /// </summary>
    private void InitializeFromConfigs(LightConfigCollection configCollection)
    {
        try
        {
            //释放所有控制器
            DisposeAllControllers();

            if (configCollection?.Configs == null)
            {
                return;
            }

            foreach (var config in configCollection.Configs.Where(c => c is { Enabled: true }))
            {
                try
                {
                    ILightController controller = config.Type switch
                    {
                        LightControllerType.Fgen => new FgenLightController(config),
                        _ => throw new NotSupportedException($"不支持的光源类型: {config.Type}")
                    };

                    if (controller.Open())
                    {
                        _controllers[config.Name] = controller;
                        LogHelper.Info($"光源控制器[{config.Name}]初始化成功");
                    }
                    else
                    {
                        LogHelper.Warn($"光源控制器[{config.Name}]打开失败");
                        controller.Dispose();
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.Error(ex, $"初始化光源控制器[{config.Name}]失败");
                }
            }

            LogHelper.Info($"光源控制器初始化完成，成功启动{_controllers.Count}个控制器");
        }
        catch (Exception ex)
        {
            LogHelper.Error(ex, "从配置初始化光源控制器失败");
        }
    }

    #region 加载保存

    /// <summary>
    /// 加载本地配置并创建控制设备
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

        // 根据最新配置重建控制器
        InitializeFromConfigs(_configs);
    }

    /// <summary>
    /// 保存当前配置到本地
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

    #region 外部访问（配置与控制器）

    /// <summary>
    /// 添加一个配置并按需创建控制器
    /// </summary>
    public LightConfig AddConfig(LightConfig config)
    {
        if (config == null) return null;
        lock (_configLock)
        {
            _configs.Add(config);
        }

        //立即保存
        SaveConfigs();

        // 按需创建控制器
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

        //关闭并移除控制器
        if (_controllers.TryRemove(name, out var controller))
        {
            try { controller.Dispose(); } catch (Exception ex) { LogHelper.Error(ex, $"释放控制器[{name}]失败"); }
        }

        SaveConfigs();
        return true;
    }

    /// <summary>
    /// 更新配置（根据名称匹配），并重建对应控制器
    /// </summary>
    public bool UpdateConfig(LightConfig updated)
    {
        if (updated == null || string.IsNullOrWhiteSpace(updated.Name)) return false;

        lock (_configLock)
        {
            var existing = _configs.FindByName(updated.Name);
            if (existing == null) return false;

            // 简单覆盖属性
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

        SaveConfigs();

        // 重建对应控制器
        RebuildControllerFor(updated.Name);
        return true;
    }

    /// <summary>
    ///通过设备名称获取光源控制器实例
    /// </summary>
    public ILightController GetController(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return null;

        _controllers.TryGetValue(name, out var controller);
        return controller;
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
    ///使当前配置生效（重建所有控制器）
    /// </summary>
    public void ApplyConfigs()
    {
        InitializeFromConfigs(Configs);
    }

    #endregion


    /// <summary>
    ///释放所有控制器
    /// </summary>
    private void DisposeAllControllers()
    {
        foreach (var kvp in _controllers)
        {
            try
            {
                kvp.Value?.Dispose();
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, $"释放光源控制器[{kvp.Key}]失败");
            }
        }

        _controllers.Clear();
    }

    private void TryCreateController(LightConfig config)
    {
        try
        {
            if (!(config?.Enabled ?? false))
            {
                return;
            }

            ILightController controller = config.Type switch
            {
                LightControllerType.Fgen => new FgenLightController(config),
                _ => null
            };

            if (controller == null)
            {
                LogHelper.Warn($"不支持的光源类型: {config?.Type}");
                return;
            }

            if (controller.Open())
            {
                _controllers[config.Name] = controller;
                LogHelper.Info($"光源控制器[{config.Name}]初始化成功");
            }
            else
            {
                LogHelper.Warn($"光源控制器[{config.Name}]打开失败");
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
            //先释放旧的
            if (_controllers.TryRemove(name, out var old))
            {
                try { old.Dispose(); } catch (Exception ex) { LogHelper.Error(ex, $"释放旧控制器[{name}]失败"); }
            }

            // 再按当前配置创建
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
