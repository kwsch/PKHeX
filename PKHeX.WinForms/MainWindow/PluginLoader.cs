using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace PKHeX.WinForms
{
    public static class PluginLoader
    {
        public static IEnumerable<T> LoadPlugins<T>(string pluginPath, PluginLoadSetting loadSetting) where T : class
        {
            var dllFileNames = !Directory.Exists(pluginPath)
                ? Enumerable.Empty<string>()
                : Directory.EnumerateFiles(pluginPath, "*.dll", SearchOption.AllDirectories);
            var assemblies = GetAssemblies(dllFileNames, loadSetting);
            var pluginTypes = GetPluginsOfType<T>(assemblies);
            return LoadPlugins<T>(pluginTypes);
        }

        private static IEnumerable<T> LoadPlugins<T>(IEnumerable<Type> pluginTypes) where T : class
        {
            foreach (var t in pluginTypes)
            {
                T? activate;
                try { activate = (T?)Activator.CreateInstance(t); }
#pragma warning disable CA1031 // Do not catch general exception types
                catch (Exception ex)
#pragma warning restore CA1031 // Do not catch general exception types
                {
                    System.Diagnostics.Debug.WriteLine($"Unable to load plugin [{t.Name}]: {t.FullName}");
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                    continue;
                }
                if (activate != null)
                    yield return activate;
            }
        }

        private static IEnumerable<Assembly> GetAssemblies(IEnumerable<string> dllFileNames, PluginLoadSetting loadSetting)
        {
            var assemblies = dllFileNames.Select(GetPluginLoadMethod(loadSetting));
            if (loadSetting is PluginLoadSetting.LoadFromMerged or PluginLoadSetting.LoadFileMerged or PluginLoadSetting.UnsafeMerged)
                assemblies = assemblies.Concat(new[] { Assembly.GetExecutingAssembly() }); // load merged too
            return assemblies;
        }

        private static Func<string, Assembly> GetPluginLoadMethod(PluginLoadSetting pls) => pls switch
        {
            PluginLoadSetting.LoadFrom or PluginLoadSetting.LoadFromMerged => Assembly.LoadFrom,
            PluginLoadSetting.LoadFile or PluginLoadSetting.LoadFileMerged => Assembly.LoadFile,
            PluginLoadSetting.UnsafeLoadFrom or PluginLoadSetting.UnsafeMerged => Assembly.UnsafeLoadFrom,
            _ => throw new NotImplementedException($"PluginLoadSetting: {pls} method not defined."),
        };

        private static IEnumerable<Type> GetPluginsOfType<T>(IEnumerable<Assembly> assemblies)
        {
            var pluginType = typeof(T);
            return assemblies.SelectMany(z => GetPluginTypes(z, pluginType));
        }

        private static IEnumerable<Type> GetPluginTypes(Assembly z, Type pluginType)
        {
            try
            {
                var types = z.GetTypes();
                return types.Where(type => IsTypePlugin(type, pluginType));
            }
#pragma warning disable CA1031 // Do not catch general exception types
            // User plugins can be out of date, with mismatching API surfaces.
            catch (Exception ex)
#pragma warning restore CA1031 // Do not catch general exception types
            {
                System.Diagnostics.Debug.WriteLine($"Unable to load plugin [{pluginType.Name}]: {z.FullName}");
                System.Diagnostics.Debug.WriteLine(ex.Message);
                if (ex is ReflectionTypeLoadException rtle)
                {
                    foreach (var le in rtle.LoaderExceptions)
                    {
                        if (le is not null)
                            System.Diagnostics.Debug.WriteLine(le.Message);
                    }
                }
                return Array.Empty<Type>();
            }
        }

        private static bool IsTypePlugin(Type type, Type pluginType)
        {
            if (type.IsInterface || type.IsAbstract)
                return false;
            var name = pluginType.FullName;
            if (name == null)
                return false;
            if (type.GetInterface(name) == null)
                return false;
            return true;
        }
    }
}
