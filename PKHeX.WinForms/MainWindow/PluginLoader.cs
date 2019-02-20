using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace PKHeX.WinForms
{
    public static class PluginLoader
    {
        public static IEnumerable<T> LoadPlugins<T>(string pluginPath)
        {
            var dllFileNames = !Directory.Exists(pluginPath)
                ? Enumerable.Empty<string>()
                : Directory.EnumerateFiles(pluginPath, "*.dll", SearchOption.AllDirectories);
            var assemblies = GetAssemblies(dllFileNames);
            var pluginTypes = GetPluginsOfType<T>(assemblies);
            return LoadPlugins<T>(pluginTypes);
        }

        private static IEnumerable<T> LoadPlugins<T>(IEnumerable<Type> pluginTypes)
        {
            return pluginTypes.Select(type => (T)Activator.CreateInstance(type));
        }

        private static IEnumerable<Assembly> GetAssemblies(IEnumerable<string> dllFileNames)
        {
            #if UNSAFEDLL
            var assemblies = dllFileNames.Select(Assembly.UnsafeLoadFrom);
            #else
            var assemblies = dllFileNames.Select(Assembly.LoadFrom);
            #endif
            #if MERGED
            assemblies = assemblies.Concat(new[] { Assembly.GetExecutingAssembly() }); // load merged too
            #endif
            return assemblies;
        }

        private static IEnumerable<Type> GetPluginsOfType<T>(IEnumerable<Assembly> assemblies)
        {
            var pluginType = typeof(T);
            return assemblies.Where(z => z != null).SelectMany(z => GetPluginTypes(z, pluginType));
        }

        private static IEnumerable<Type> GetPluginTypes(Assembly z, Type pluginType)
        {
            try
            {
                var types = z.GetTypes();
                return types.Where(type => IsTypePlugin(type, pluginType));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Unable to load plugin [{pluginType.Name}]: {z.FullName}", ex.Message);
                return Enumerable.Empty<Type>();
            }
        }

        private static bool IsTypePlugin(Type type, Type pluginType)
        {
            if (type.IsInterface || type.IsAbstract)
                return false;
            if (type.GetInterface(pluginType.FullName) == null)
                return false;
            return true;
        }
    }
}
