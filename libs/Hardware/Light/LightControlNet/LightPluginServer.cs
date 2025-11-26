using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace LightControlNet
{
    public sealed class LightPluginServer
    {
        private static readonly Lazy<LightPluginServer> _instance = new(() => new LightPluginServer());
        public static LightPluginServer Instance => _instance.Value;

        private readonly Dictionary<LightControllerType, Type> _typeByEnum = new();
        private readonly List<Type> _loadedTypes = new();

        private LightPluginServer() { }

        public void LoadPlugins()
        {
            _typeByEnum.Clear();
            _loadedTypes.Clear();

            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            var pluginDir = Path.Combine(baseDir, "Plugins", "Light");
            if (!Directory.Exists(pluginDir)) return;

            var dlls = Directory.GetFiles(pluginDir, "*.dll", SearchOption.TopDirectoryOnly);
            foreach (var dll in dlls)
            {
                Assembly asm;
                try { asm = Assembly.LoadFrom(dll); }
                catch { continue; }

                Type[] types;
                try { types = asm.GetTypes(); }
                catch { continue; }

                foreach (var t in types)
                {
                    if (t.IsAbstract) continue;
                    if (!typeof(ILightController).IsAssignableFrom(t)) continue;

                    var attr = t.GetCustomAttribute<LightManufacturerAttribute>();
                    if (attr == null) continue;

                    _loadedTypes.Add(t);
                    _typeByEnum[attr.Type] = t;
                }
            }
        }

        public IReadOnlyList<Type> GetLoadedPluginTypes() => _loadedTypes;
        public Type GetPluginType(LightControllerType type)
        {
            _typeByEnum.TryGetValue(type, out var t);
            return t;
        }
    }
}
