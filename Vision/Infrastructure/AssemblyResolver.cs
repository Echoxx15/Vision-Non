using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Vision.Infrastructure;

/// <summary>
/// Adds additional probing directories for managed and native dlls at runtime.
/// Scans given directories (and all sub-directories) to resolve assemblies.
/// Also prepends these directories to PATH so native dependencies can be found.
/// </summary>
internal static class AssemblyResolver
{
    private static readonly object _lock = new object();
    private static bool _registered;
    private static readonly HashSet<string> _probeDirs = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
    private static readonly Dictionary<string, Assembly> _cache = new Dictionary<string, Assembly>(StringComparer.OrdinalIgnoreCase);

    public static void RegisterProbingFolders(params string[] relativeFolders)
    {
        lock (_lock)
        {
            if (_registered) return;
            _registered = true;

            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            var folders = (relativeFolders ?? Array.Empty<string>()).Select(f => Path.Combine(baseDir, f)).ToList();

            foreach (var root in folders)
            {
                if (!Directory.Exists(root)) continue;
                AddProbeDir(root);
                try
                {
                    foreach (var dir in Directory.EnumerateDirectories(root, "*", SearchOption.AllDirectories))
                    {
                        AddProbeDir(dir);
                    }
                }
                catch { }
            }

            // Prepend to PATH for native DLL lookup
            try
            {
                var path = Environment.GetEnvironmentVariable("PATH") ?? string.Empty;
                var parts = new HashSet<string>(path.Split(Path.PathSeparator), StringComparer.OrdinalIgnoreCase);
                var toPrepend = _probeDirs.Where(d => !parts.Contains(d)).ToArray();
                if (toPrepend.Length > 0)
                {
                    var newPath = string.Join(Path.PathSeparator.ToString(), toPrepend) + Path.PathSeparator + path;
                    Environment.SetEnvironmentVariable("PATH", newPath);
                }
            }
            catch { }

            AppDomain.CurrentDomain.AssemblyResolve += OnResolve;
            AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve += OnReflectionOnlyResolve;
        }
    }

    private static void AddProbeDir(string dir)
    {
        if (string.IsNullOrWhiteSpace(dir)) return;
        if (_probeDirs.Add(dir)) { /* reserved for future logging */ }
    }

    private static Assembly OnReflectionOnlyResolve(object sender, ResolveEventArgs args)
    {
        var asm = TryResolve(args.Name, reflectionOnly: true);
        return asm;
    }

    private static Assembly OnResolve(object sender, ResolveEventArgs args)
    {
        var asm = TryResolve(args.Name, reflectionOnly: false);
        return asm;
    }

    private static Assembly TryResolve(string fullName, bool reflectionOnly)
    {
        try
        {
            var name = new AssemblyName(fullName).Name;
            if (string.IsNullOrWhiteSpace(name)) return null;

            lock (_lock)
            {
                if (_cache.TryGetValue(name, out var cached)) return cached;

                // Try exact file match "Name.dll"
                foreach (var dir in _probeDirs)
                {
                    var file = Path.Combine(dir, name + ".dll");
                    if (File.Exists(file))
                    {
                        var asm = reflectionOnly ? Assembly.ReflectionOnlyLoadFrom(file) : Assembly.LoadFrom(file);
                        if (asm != null) { _cache[name] = asm; return asm; }
                    }
                }

                // Fallback: search recursively
                foreach (var dir in _probeDirs)
                {
                    try
                    {
                        var match = Directory.EnumerateFiles(dir, name + ".dll", SearchOption.AllDirectories).FirstOrDefault();
                        if (!string.IsNullOrEmpty(match))
                        {
                            var asm = reflectionOnly ? Assembly.ReflectionOnlyLoadFrom(match) : Assembly.LoadFrom(match);
                            if (asm != null) { _cache[name] = asm; return asm; }
                        }
                    }
                    catch { }
                }
            }
        }
        catch { }
        return null;
    }
}