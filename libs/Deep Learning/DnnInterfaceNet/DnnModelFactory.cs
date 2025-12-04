using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;

namespace DnnInterfaceNet;

/// <summary>
/// 深度学习模型工厂（单例）- 负责模型实例的创建、管理和配置保存
/// </summary>
public sealed class DnnModelFactory
{
    private static readonly Lazy<DnnModelFactory> _instance = new(() => new DnnModelFactory());
    public static DnnModelFactory Instance => _instance.Value;

    /// <summary>
    /// 已创建的模型实例（名称 -> 实例）
    /// </summary>
    private readonly ConcurrentDictionary<string, IDnnModel> _models = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// 模型类型名称映射（名称 -> 类型名称）
    /// </summary>
    private readonly ConcurrentDictionary<string, string> _modelTypeNames = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// 配置文件路径（跟随方案）
    /// </summary>
    private string _configFilePath;

    private int _modelCounter;

    #region 事件

    /// <summary>
    /// 模型创建事件（模型名称、模型类型、模型实例）
    /// </summary>
    public event Action<string, string, IDnnModel> ModelCreated;

    /// <summary>
    /// 模型移除事件（模型名称、模型类型）
    /// </summary>
    public event Action<string, string> ModelRemoved;

    /// <summary>
    /// 模型重命名事件（旧名称、新名称、模型类型）
    /// </summary>
    public event Action<string, string, string> ModelRenamed;

    /// <summary>
    /// 模型加载状态变化事件（模型名称、是否已加载）
    /// </summary>
    public event Action<string, bool> ModelLoadStatusChanged;

    #endregion

    private DnnModelFactory() { }

    /// <summary>
    /// 初始化：从插件服务器构建模型类型映射
    /// </summary>
    public void Initialize()
    {
        DnnPluginServer.Instance.LoadPlugins();
    }

    /// <summary>
    /// 设置配置文件路径（跟随方案切换）
    /// </summary>
    public void SetConfigPath(string solutionPath)
    {
        if (string.IsNullOrEmpty(solutionPath))
        {
            _configFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Configs", "DnnModels.xml");
        }
        else
        {
            _configFilePath = Path.Combine(solutionPath, "DnnModels.xml");
        }

        var dir = Path.GetDirectoryName(_configFilePath);
        if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
    }

    /// <summary>
    /// 创建深度学习模型
    /// </summary>
    /// <param name="modelTypeName">模型类型名称（如"语义分割"）</param>
    /// <param name="customName">自定义模型名称（可选）</param>
    /// <returns>创建的模型实例</returns>
    public IDnnModel CreateModel(string modelTypeName, string customName = null)
    {
        if (string.IsNullOrEmpty(modelTypeName))
        {
            Console.WriteLine("模型类型名称不能为空");
            return null;
        }

        // 获取插件类型
        var type = DnnPluginServer.Instance.GetPluginTypeByModelType(modelTypeName);
        if (type == null)
        {
            Console.WriteLine($"未支持的模型类型: {modelTypeName}");
            return null;
        }

        // 生成模型名称
        _modelCounter++;
        string modelName = string.IsNullOrEmpty(customName)
            ? $"{modelTypeName}_{_modelCounter}"
            : customName;

        // 如果名称已存在，添加后缀
        int suffix = 1;
        string finalName = modelName;
        while (_models.ContainsKey(finalName))
        {
            finalName = $"{modelName}_{suffix}";
            suffix++;
        }

        try
        {
            var model = (IDnnModel)Activator.CreateInstance(type, finalName);

            if (_models.TryAdd(finalName, model))
            {
                _modelTypeNames[finalName] = modelTypeName;
                Console.WriteLine($"创建深度学习模型: {finalName} ({modelTypeName})");

                try { ModelCreated?.Invoke(finalName, modelTypeName, model); }
                catch (Exception ex) { Console.WriteLine($"触发模型创建事件失败: {ex.Message}"); }

                return model;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"创建模型失败: {ex.Message}");
        }

        return null;
    }

    /// <summary>
    /// 重命名模型
    /// </summary>
    public bool RenameModel(string oldName, string newName)
    {
        if (string.IsNullOrWhiteSpace(oldName) || string.IsNullOrWhiteSpace(newName))
            return false;
        if (string.Equals(oldName, newName, StringComparison.OrdinalIgnoreCase))
            return true;
        if (_models.ContainsKey(newName))
            return false;

        if (_models.TryRemove(oldName, out var model))
        {
            try
            {
                if (model is IRenameableDnnModel r)
                    r.SetName(newName);
            }
            catch { }

            var result = _models.TryAdd(newName, model);

            if (result)
            {
                // 更新类型名称映射
                if (_modelTypeNames.TryRemove(oldName, out var typeName))
                {
                    _modelTypeNames[newName] = typeName;
                }

                try { ModelRenamed?.Invoke(oldName, newName, GetModelType(model)); }
                catch (Exception ex) { Console.WriteLine($"触发模型重命名事件失败: {ex.Message}"); }
            }

            return result;
        }

        return false;
    }

    /// <summary>
    /// 移除指定名称的模型
    /// </summary>
    public bool RemoveModel(string name)
    {
        if (_models.TryGetValue(name, out var model))
        {
            var modelType = GetModelType(model);

            if (_models.TryRemove(name, out _))
            {
                _modelTypeNames.TryRemove(name, out _);

                try { model?.Dispose(); }
                catch { }

                try { ModelRemoved?.Invoke(name, modelType); }
                catch (Exception ex) { Console.WriteLine($"触发模型移除事件失败: {ex.Message}"); }

                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// 获取模型的类型名称
    /// </summary>
    private string GetModelType(IDnnModel model)
    {
        if (model == null) return string.Empty;

        var type = model.GetType();
        var attr = type.GetCustomAttribute<DnnModelTypeAttribute>();
        return attr?.TypeName ?? type.Name;
    }

    /// <summary>
    /// 保存所有模型配置到 XML 文件
    /// </summary>
    public void SaveConfigs()
    {
        if (string.IsNullOrEmpty(_configFilePath))
        {
            Console.WriteLine("配置文件路径未设置");
            return;
        }

        try
        {
            var collection = new DnnModelConfigCollection();

            foreach (var kvp in _models)
            {
                var model = kvp.Value;
                DnnModelConfig config;

                if (model is IConfigurableDnnModel configurableModel)
                {
                    config = configurableModel.GetConfig();
                }
                else
                {
                    config = new DnnModelConfig
                    {
                        Name = kvp.Key,
                        Type = GetModelType(model),
                        ModelPath = model.ModelPath
                    };
                }

                collection.Configs.Add(config);
            }

            var serializer = new XmlSerializer(typeof(DnnModelConfigCollection));
            using (var stream = File.Create(_configFilePath))
            {
                serializer.Serialize(stream, collection);
            }

            Console.WriteLine($"✅ 深度学习模型配置已保存: {_configFilePath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ 保存模型配置失败: {ex.Message}");
        }
    }

    /// <summary>
    /// 从 XML 文件加载配置并创建模型
    /// </summary>
    public void LoadConfigs()
    {
        if (string.IsNullOrEmpty(_configFilePath) || !File.Exists(_configFilePath))
        {
            Console.WriteLine("模型配置文件不存在，跳过加载");
            return;
        }

        try
        {
            var serializer = new XmlSerializer(typeof(DnnModelConfigCollection));
            DnnModelConfigCollection collection;

            using (var stream = File.OpenRead(_configFilePath))
            {
                collection = (DnnModelConfigCollection)serializer.Deserialize(stream);
            }

            if (collection?.Configs == null || collection.Configs.Count == 0)
            {
                Console.WriteLine("模型配置文件为空");
                return;
            }

            int successCount = 0, failCount = 0;

            foreach (var config in collection.Configs)
            {
                try
                {
                    if (string.IsNullOrEmpty(config.Name) || string.IsNullOrEmpty(config.Type))
                    {
                        failCount++;
                        continue;
                    }

                    var type = DnnPluginServer.Instance.GetPluginTypeByModelType(config.Type);
                    if (type == null)
                    {
                        Console.WriteLine($"不支持的模型类型: {config.Type}，跳过模型: {config.Name}");
                        failCount++;
                        continue;
                    }

                    var model = (IDnnModel)Activator.CreateInstance(type, config.Name);

                    // 应用配置
                    if (model is IConfigurableDnnModel configurableModel)
                    {
                        configurableModel.ApplyConfig(config);
                    }

                    // 如果配置为启动时加载且有模型路径，则加载模型
                    if (config.LoadOnStartup && !string.IsNullOrEmpty(config.ModelPath) && Directory.Exists(config.ModelPath))
                    {
                        model.Load(config.ModelPath, config.DeviceType, config.Runtime);
                    }

                    if (_models.TryAdd(config.Name, model))
                    {
                        _modelTypeNames[config.Name] = config.Type;
                        successCount++;
                        Console.WriteLine($"✅ 加载模型: {config.Name} ({config.Type})");

                        try { ModelCreated?.Invoke(config.Name, config.Type, model); }
                        catch { }
                    }
                    else
                    {
                        failCount++;
                        model?.Dispose();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ 加载模型失败 [{config?.Name}]: {ex.Message}");
                    failCount++;
                }
            }

            Console.WriteLine($"===== 模型配置加载完成: 成功 {successCount} 个，失败 {failCount} 个 =====");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ 加载模型配置文件失败: {ex.Message}");
        }
    }

    /// <summary>
    /// 获取所有支持的模型类型
    /// </summary>
    public List<string> GetAllModelTypes()
    {
        return DnnPluginServer.Instance.GetAllModelTypeNames();
    }

    /// <summary>
    /// 获取指定名称的模型
    /// </summary>
    public IDnnModel GetModel(string name)
    {
        return _models.TryGetValue(name, out var model) ? model : null;
    }

    /// <summary>
    /// 获取所有模型
    /// </summary>
    public IEnumerable<IDnnModel> GetAllModels()
    {
        return _models.Values.ToList();
    }

    /// <summary>
    /// 获取所有模型名称
    /// </summary>
    public List<string> GetAllModelNames()
    {
        return _models.Keys.ToList();
    }

    /// <summary>
    /// 获取指定类型的所有已加载模型
    /// </summary>
    /// <param name="modelTypeName">模型类型名称（如"语义分割"、"SemanticSegmentation"）</param>
    public List<IDnnModel> GetLoadedModelsByCategory(string modelTypeName)
    {
        if (string.IsNullOrEmpty(modelTypeName))
            return new List<IDnnModel>();

        var result = new List<IDnnModel>();
        foreach (var kvp in _models)
        {
            var model = kvp.Value;
            if (!model.IsLoaded) continue;

            // 检查类型名称是否匹配
            if (_modelTypeNames.TryGetValue(kvp.Key, out var typeName))
            {
                if (string.Equals(typeName, modelTypeName, StringComparison.OrdinalIgnoreCase))
                {
                    result.Add(model);
                    continue;
                }
            }

            // 检查特性标记的类型名称
            var attr = model.GetType().GetCustomAttribute<DnnModelTypeAttribute>();
            if (attr != null && string.Equals(attr.TypeName, modelTypeName, StringComparison.OrdinalIgnoreCase))
            {
                result.Add(model);
            }
        }

        return result;
    }

    /// <summary>
    /// 卸载所有模型并清空
    /// </summary>
    public void UnloadAll()
    {
        foreach (var model in _models.Values)
        {
            try { model?.Dispose(); }
            catch { }
        }

        _models.Clear();
        _modelTypeNames.Clear();
        _modelCounter = 0;
    }

    /// <summary>
    /// 获取模型数量
    /// </summary>
    public int ModelCount => _models.Count;
}
