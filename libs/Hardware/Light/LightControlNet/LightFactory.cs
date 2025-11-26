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
/// ��Դ������������ģʽ��
///ͳһ�������й�Դ�����������õļ����뱣��
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
        // ���Լ��ر�������
        try
        {
            LoadConfigs();
        }
        catch (Exception ex)
        {
            LogHelper.Error(ex, "��ʼ�����ع�Դ����ʧ��");
            _configs = new LightConfigCollection();
        }
    }

    /// <summary>
    /// �����ļ�·��
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
    /// ��ȡ��ǰ���ü��ϣ����ã������÷������޸ĺ������ SaveConfigs��
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
    ///���¸������ü��ϴ���������
    /// </summary>
    private void InitializeFromConfigs(LightConfigCollection configCollection)
    {
        try
        {
            //�ͷ����п�����
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
                        LogHelper.Info($"��Դ������[{config.Name}]��ʼ���ɹ�");
                    }
                    else
                    {
                        LogHelper.Warn($"��Դ������[{config.Name}]��ʧ��");
                        controller.Dispose();
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.Error(ex, $"��ʼ����Դ������[{config.Name}]ʧ��");
                }
            }

            LogHelper.Info($"��Դ��������ʼ����ɣ��ɹ�����{_controllers.Count}��������");
        }
        catch (Exception ex)
        {
            LogHelper.Error(ex, "�����ó�ʼ����Դ������ʧ��");
        }
    }

    #region ���ر���

    /// <summary>
    /// ���ر������ò����������豸
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
                LogHelper.Error(ex, "���ع�Դ�����ļ�ʧ��");
                _configs = new LightConfigCollection();
            }
        }

        // �������������ؽ�������
        InitializeFromConfigs(_configs);
    }

    /// <summary>
    /// ���浱ǰ���õ�����
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
                LogHelper.Error(ex, "�����Դ�����ļ�ʧ��");
            }
        }
    }

    #endregion

    #region �ⲿ���ʣ��������������

    /// <summary>
    /// ����һ�����ò����贴��������
    /// </summary>
    public LightConfig AddConfig(LightConfig config)
    {
        if (config == null) return null;
        lock (_configLock)
        {
            _configs.Add(config);
        }

        //��������
        SaveConfigs();

        // ���贴��������
        TryCreateController(config);
        return config;
    }

    /// <summary>
    /// ɾ�����ƶ�Ӧ�����ò��رտ�����
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

        //�رղ��Ƴ�������
        if (_controllers.TryRemove(name, out var controller))
        {
            try { controller.Dispose(); } catch (Exception ex) { LogHelper.Error(ex, $"�ͷſ�����[{name}]ʧ��"); }
        }

        SaveConfigs();
        return true;
    }

    /// <summary>
    /// �������ã���������ƥ�䣩�����ؽ���Ӧ������
    /// </summary>
    public bool UpdateConfig(LightConfig updated)
    {
        if (updated == null || string.IsNullOrWhiteSpace(updated.Name)) return false;

        lock (_configLock)
        {
            var existing = _configs.FindByName(updated.Name);
            if (existing == null) return false;

            // �򵥸�������
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

        // �ؽ���Ӧ������
        RebuildControllerFor(updated.Name);
        return true;
    }

    /// <summary>
    ///ͨ���豸���ƻ�ȡ��Դ������ʵ��
    /// </summary>
    public ILightController GetController(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return null;

        _controllers.TryGetValue(name, out var controller);
        return controller;
    }

    /// <summary>
    /// ��ȡ���ƶ�Ӧ������
    /// </summary>
    public LightConfig GetConfig(string name)
    {
        lock (_configLock)
        {
            return _configs.FindByName(name);
        }
    }

    /// <summary>
    ///ʹ��ǰ������Ч���ؽ����п�������
    /// </summary>
    public void ApplyConfigs()
    {
        InitializeFromConfigs(Configs);
    }

    #endregion


    /// <summary>
    ///�ͷ����п�����
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
                LogHelper.Error(ex, $"�ͷŹ�Դ������[{kvp.Key}]ʧ��");
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

            var controller = LightManager.Instance.CreateController(config);

            if (controller == null) { LogHelper.Warn($"不支持的光源控制器: {config?.Type}"); return; }

            if (controller.Open())
            {
                _controllers[config.Name] = controller;
                LogHelper.Info($"��Դ������[{config.Name}]��ʼ���ɹ�");
            }
            else
            {
                LogHelper.Warn($"��Դ������[{config.Name}]��ʧ��");
                controller.Dispose();
            }
        }
        catch (Exception ex)
        {
            LogHelper.Error(ex, $"������Դ������[{config?.Name}]ʧ��");
        }
    }

    private void RebuildControllerFor(string name)
    {
        try
        {
            //���ͷžɵ�
            if (_controllers.TryRemove(name, out var old))
            {
                try { old.Dispose(); } catch (Exception ex) { LogHelper.Error(ex, $"�ͷžɿ�����[{name}]ʧ��"); }
            }

            // �ٰ���ǰ���ô���
            var cfg = GetConfig(name);
            if (cfg != null)
            {
                TryCreateController(cfg);
            }
        }
        catch (Exception ex)
        {
            LogHelper.Error(ex, $"�ؽ�������[{name}]ʧ��");
        }
    }

    public void Dispose()
    {
        DisposeAllControllers();
    }
}
