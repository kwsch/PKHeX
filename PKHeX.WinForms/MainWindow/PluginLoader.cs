using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;

namespace PKHeX.WinForms;

/// <summary>
/// Provides functionality to load plugins from assemblies at runtime.
/// </summary>
public static class PluginLoader
{
    /// <summary>
    /// Loads plugin assemblies from the given directory using the provided load setting.
    /// </summary>
    /// <param name="pluginPath">The directory path to search for plugin assemblies.</param>
    /// <param name="loadMerged">The plugin load setting to use.</param>
    /// <returns>A PluginLoadResult containing contexts and assemblies.</returns>
    public static PluginLoadResult LoadPluginAssemblies(string pluginPath, bool loadMerged)
    {
        var result = new PluginLoadResult();
        var dllFileNames = !Directory.Exists(pluginPath)
            ? []
            : Directory.EnumerateFiles(pluginPath, "*.dll", SearchOption.AllDirectories);
        foreach (var file in dllFileNames)
        {
            try
            {
                var context = new PluginLoadContext(file);
                var asm = context.LoadFromAssemblyPath(file);
                result.Contexts.Add(context);
                result.Assemblies.Add(asm);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Unable to load plugin from file: {file}");
                Debug.WriteLine(ex.Message);
            }
        }
        if (loadMerged)
            result.Assemblies.Add(Assembly.GetExecutingAssembly());
        return result;
    }

    /// <summary>
    /// Loads plugins of the specified type from the given directory using the provided load setting.
    /// </summary>
    /// <typeparam name="T">The type of plugin to load.</typeparam>
    /// <param name="pluginPath">The directory path to search for plugin assemblies.</param>
    /// <param name="loadMerged">The plugin load setting to use.</param>
    /// <returns>An enumerable of loaded plugin instances of type <typeparamref name="T"/>.</returns>
    public static IEnumerable<T> LoadPlugins<T>(string pluginPath, bool loadMerged) where T : class
    {
        var result = LoadPluginAssemblies(pluginPath, loadMerged);
        var pluginTypes = GetPluginsOfType<T>(result.GetAssemblies());
        return LoadPlugins<T>(pluginTypes);
    }

    /// <summary>
    /// Loads plugin instances of the specified type from the given plugin types.
    /// </summary>
    /// <typeparam name="T">The type of plugin to load.</typeparam>
    /// <param name="pluginTypes">The types of plugins to instantiate.</param>
    /// <returns>An enumerable of loaded plugin instances of type <typeparamref name="T"/>.</returns>
    private static IEnumerable<T> LoadPlugins<T>(IEnumerable<Type> pluginTypes) where T : class
    {
        foreach (var t in pluginTypes)
        {
            T? activate;
            try { activate = (T?)Activator.CreateInstance(t); }
            catch (Exception ex)
            {
                Debug.WriteLine($"Unable to load plugin [{t.Name}]: {t.FullName}");
                Debug.WriteLine(ex.Message);
                continue;
            }
            if (activate is not null)
                yield return activate;
        }
    }

    /// <summary>
    /// Gets all plugin types of the specified type from the given assemblies.
    /// </summary>
    /// <typeparam name="T">The type of plugin to search for.</typeparam>
    /// <param name="assemblies">The assemblies to search for plugins.</param>
    /// <returns>An enumerable of plugin types.</returns>
    private static IEnumerable<Type> GetPluginsOfType<T>(IEnumerable<Assembly> assemblies)
    {
        var pluginType = typeof(T);
        return assemblies.SelectMany(z => GetPluginTypes(z, pluginType));
    }

    /// <summary>
    /// Gets all types from the specified assembly that match the given plugin type.
    /// </summary>
    /// <param name="z">The assembly to search.</param>
    /// <param name="plugin">The plugin type to match.</param>
    /// <returns>An enumerable of matching types.</returns>
    private static IEnumerable<Type> GetPluginTypes(Assembly z, Type plugin)
    {
        try
        {
            // Handle Costura merged plugin dll's; need to Attach for them to correctly retrieve their dependencies.
            var assemblyLoaderType = z.GetType("Costura.AssemblyLoader", false);
            var attachMethod = assemblyLoaderType?.GetMethod("Attach", BindingFlags.Static | BindingFlags.Public);
            attachMethod?.Invoke(null, []);

            var types = z.GetExportedTypes();
            return types.Where(type => IsTypePlugin(type, plugin));
        }
        // User plugins can be out of date, with mismatching API surfaces.
        catch (Exception ex)
        {
            Debug.WriteLine($"Unable to load plugin [{plugin.FullName}]: {z.FullName}");
            Debug.WriteLine(ex.Message);
            if (ex is not ReflectionTypeLoadException rtle)
                return [];

            foreach (var le in rtle.LoaderExceptions)
            {
                if (le is not null)
                    Debug.WriteLine(le.Message);
            }
            return [];
        }
    }

    /// <summary>
    /// Determines whether the specified type is a valid plugin type.
    /// </summary>
    /// <param name="type">The type to check.</param>
    /// <param name="plugin">The plugin type to match.</param>
    /// <returns><see langword="true"/> if the type is a valid plugin type; otherwise, <see langword="false"/>.</returns>
    private static bool IsTypePlugin(Type type, Type plugin)
    {
        if (type.IsInterface || type.IsAbstract)
            return false;
        return plugin.IsAssignableFrom(type);
    }
}

/// <summary>
/// Encapsulates the result of loading plugins, including their contexts and assemblies.
/// </summary>
public class PluginLoadResult
{
    public List<PluginLoadContext> Contexts { get; } = new();
    public List<Assembly> Assemblies { get; } = new();

    /// <summary>
    /// Returns all loaded assemblies for downstream use.
    /// </summary>
    public IEnumerable<Assembly> GetAssemblies() => Assemblies;
}

/// <summary>
/// Custom AssemblyLoadContext for loading plugin assemblies in isolation.
/// </summary>
public class PluginLoadContext : AssemblyLoadContext
{
    private readonly AssemblyDependencyResolver Resolver;

    /// <summary>
    /// Initializes a new instance of the <see cref="PluginLoadContext"/> class.
    /// </summary>
    /// <param name="pluginPath">The path to the plugin assembly.</param>
    public PluginLoadContext(string pluginPath) : base(isCollectible: true)
    {
        Resolver = new AssemblyDependencyResolver(pluginPath);
    }

    /// <summary>
    /// Loads the main plugin assembly from the specified path. Delegates framework assemblies to the default context.
    /// </summary>
    /// <param name="assemblyName">The assembly name to load.</param>
    /// <returns>The loaded assembly, or null if not the main plugin assembly.</returns>
    protected override Assembly? Load(AssemblyName assemblyName)
    {
        // Try to resolve plugin-local dependencies
        var assemblyPath = Resolver.ResolveAssemblyToPath(assemblyName);
        if (assemblyPath != null)
            return LoadFromAssemblyPath(assemblyPath);

        // Fallback: try to resolve from the default context (main app/shared dependencies)
        try
        {
            return Default.LoadFromAssemblyName(assemblyName);
        }
        catch
        {
            // Not found in default context
            return null;
        }
    }
}
