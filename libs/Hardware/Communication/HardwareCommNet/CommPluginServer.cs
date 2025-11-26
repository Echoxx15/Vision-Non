using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace HardwareCommNet;

/// <summary>
/// 通讯插件服务器：负责从 Plugins/Comm目录加载所有通讯插件（单例模式）
/// </summary>
public sealed class CommPluginServer
{
	private static readonly Lazy<CommPluginServer> _instance = new Lazy<CommPluginServer>(() => new CommPluginServer(),
		LazyThreadSafetyMode.ExecutionAndPublication);

	public static CommPluginServer Instance => _instance.Value;

// 类型-插件映射（类型全名 -> 插件信息）
	private readonly Dictionary<string, PluginInfo> _pluginInfos =
		new Dictionary<string, PluginInfo>(StringComparer.OrdinalIgnoreCase);

	/// <summary>
	/// 加载的插件类型映射（类型全名 -> 类型）
	/// </summary>
	private readonly Dictionary<string, Type> _pluginTypes =
		new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase);

	private CommPluginServer()
	{
	}

	/// <summary>
	/// 从 Plugins/Comm目录加载所有通讯插件
	/// </summary>
	public void LoadPlugins()
	{
		var pluginDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Plugins", "Comm");
		if (!Directory.Exists(pluginDir))
		{
			Console.WriteLine($"通讯插件目录不存在: {pluginDir}");
			return;
		}

		foreach (var dll in Directory.GetFiles(pluginDir, "*.dll"))
		{
			try
			{
				var asm = Assembly.LoadFrom(dll);

				Type[] types;
				try
				{
					types = asm.GetTypes();
				}
				catch (ReflectionTypeLoadException rtlEx)
				{
					//记录依赖加载异常并尽可能使用已成功加载的类型继续
					var msgs = rtlEx.LoaderExceptions?.Where(e => e != null).Select(e => e.Message).ToArray() ?? Array.Empty<string>();
					Console.WriteLine($"程序集类型加载部分失败: {dll}\n -> {string.Join(" | ", msgs)}");
					types = rtlEx.Types?.Where(t => t != null).ToArray() ?? Array.Empty<Type>();
				}

				foreach (var type in types)
				{
					if (type == null) continue;
					if (!type.IsClass || type.IsAbstract) continue;

					//只收集IComm实现类
					if (!typeof(IComm).IsAssignableFrom(type)) continue;

					var info = new PluginInfo(type.FullName, asm.GetName().Name);
					_pluginInfos[type.FullName] = info;
					_pluginTypes[type.FullName] = type;

					Console.WriteLine($"加载通讯插件: {type.FullName}");
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"插件加载失败: {dll}, {ex.Message}");
			}
		}
	}

	/// <summary>
	/// 获取已加载的插件类型（返回副本，避免外部修改）
	/// </summary>
	public Dictionary<string, Type> GetLoadedPluginTypes()
	{
		return new Dictionary<string, Type>(_pluginTypes, StringComparer.OrdinalIgnoreCase);
	}

	public PluginInfo GetPluginInfo(string typeName)
	{
		if (_pluginInfos.TryGetValue(typeName, out var info))
			return info;
		return null;
	}

	public Type GetPluginType(string typeName)
	{
		_pluginTypes.TryGetValue(typeName, out var type);
		return type;
	}
}

/// <summary>
/// 插件信息
/// </summary>
public class PluginInfo
{
	public string TypeName { get; set; }
	public string AssemblyName { get; set; }

	public PluginInfo()
	{
	}

	public PluginInfo(string typeName, string assemblyName)
	{
		TypeName = typeName;
		AssemblyName = assemblyName;
	}
}
