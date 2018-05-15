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
            var dllFileNames = Directory.EnumerateFiles(pluginPath, "*.dll", SearchOption.AllDirectories);
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
            return dllFileNames.Select(AssemblyName.GetAssemblyName).Select(Assembly.Load);
        }
        private static IEnumerable<Type> GetPluginsOfType<T>(IEnumerable<Assembly> assemblies)
        {
            var pluginType = typeof(T);
            foreach (var z in assemblies.Where(z => z != null))
            {
                Type[] types; try { types = z.GetTypes(); }
                catch (Exception ex)
                {
                    Console.WriteLine($"Unable to load plugin [{pluginType.Name}]: {z.FullName}", ex.Message);
                    continue;
                }
                foreach (Type type in types)
                {
                    if (type.IsInterface || type.IsAbstract)
                        continue;
                    if (type.GetInterface(pluginType.FullName) == null)
                        continue;
                    yield return type;
                }
            }
        }
    }
}
