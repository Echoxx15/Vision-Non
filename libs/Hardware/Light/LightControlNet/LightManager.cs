using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace LightControlNet
{
    public sealed class LightManager
    {
        private static readonly Lazy<LightManager> _instance = new(() => new LightManager());
        public static LightManager Instance => _instance.Value;

        private readonly Dictionary<LightControllerType, (Type PluginType, Func<List<string>> Enumerate)> _map = new();

        private LightManager() { }

        public void BuildMap()
        {
            _map.Clear();
            foreach (var t in LightPluginServer.Instance.GetLoadedPluginTypes())
            {
                var attr = t.GetCustomAttribute<LightManufacturerAttribute>();
                if (attr == null) continue;

                var enumerate = t.GetMethod("EnumerateDevices", BindingFlags.Public | BindingFlags.Static);
                Func<List<string>> func = null;
                if (enumerate != null)
                {
                    func = () => (List<string>)enumerate.Invoke(null, null);
                }
                _map[attr.Type] = (t, func);
            }
        }

        public List<string> EnumerateDevices(LightControllerType type)
        {
            if (_map.TryGetValue(type, out var v) && v.Enumerate != null)
            {
                return v.Enumerate();
            }
            return new List<string>();
        }

        public ILightController CreateController(LightConfig config)
        {
            if (config == null) return null;
            if (!_map.TryGetValue(config.Type, out var v) || v.PluginType == null) return null;
            try
            {
                return (ILightController)Activator.CreateInstance(v.PluginType, config);
            }
            catch
            {
                return null;
            }
        }
    }
}
