using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace PKHeX.WinForms;

/// <summary>
/// Encapsulates the result of loading plugins, including their contexts and assemblies.
/// </summary>
public sealed class PluginLoadResult
{
    public List<PluginLoadContext> Contexts { get; } = [];
    public List<Assembly> Assemblies { get; } = [];

    /// <summary>
    /// Returns all loaded assemblies for downstream use.
    /// </summary>
    public IEnumerable<Assembly> GetAssemblies() => Assemblies;

    public void Load(string pluginFile)
    {
        var context = new PluginLoadContext(pluginFile);
        var asm = context.LoadFromAssemblyPath(pluginFile);
        Contexts.Add(context);
        Assemblies.Add(asm);
    }

    public void LoadFromAssembly(Assembly getExecutingAssembly)
    {
        Contexts.Add(new(""));
        Assemblies.Add(getExecutingAssembly);
    }

    public bool Unload(string pluginFile)
    {
        var context = Contexts.FirstOrDefault(z => z.Assemblies.Any(a => a.Location == pluginFile));
        if (context is null)
            return false;
        context.Unload();
        Contexts.Remove(context);
        Assemblies.RemoveAll(a => a.Location == pluginFile);
        GC.Collect();
        GC.WaitForPendingFinalizers();
        return true;
    }
}
