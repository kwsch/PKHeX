using System.Reflection;
using System.Runtime.Loader;

namespace PKHeX.WinForms;

/// <summary>
/// Custom AssemblyLoadContext for loading plugin assemblies in isolation.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="PluginLoadContext"/> class.
/// </remarks>
/// <param name="pluginPath">The path to the plugin assembly.</param>
public sealed class PluginLoadContext(string pluginPath) : AssemblyLoadContext(isCollectible: true)
{
    private readonly AssemblyDependencyResolver Resolver = new(pluginPath);

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
