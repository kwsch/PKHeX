using System.Collections.Generic;
using System.IO;
using PKHeX.Infrastructure.Configuration;

namespace PKHeX.Avalonia.Tests;

/// <summary>
/// Locks in platform-standard config/data directory resolution (issue #138). Inputs are passed
/// explicitly so every platform is covered regardless of the host OS the tests run on.
/// </summary>
public class AppPathResolverTests
{
    private const string App = "PKHeX-Avalonia";

    private static System.Func<string, string?> Env(Dictionary<string, string?> vars)
        => key => vars.TryGetValue(key, out var v) ? v : null;

    [Fact]
    public void Windows_Config_UsesAppData()
    {
        var env = Env(new() { ["APPDATA"] = @"C:\Users\ash\AppData\Roaming" });
        var dir = AppPathResolver.ResolveConfigDirectory(OSKind.Windows, @"C:\Users\ash", env, App);
        Assert.Equal(Path.Combine(@"C:\Users\ash\AppData\Roaming", App), dir);
    }

    [Fact]
    public void Windows_Config_FallsBackToProfile_WhenAppDataUnset()
    {
        var dir = AppPathResolver.ResolveConfigDirectory(OSKind.Windows, @"C:\Users\ash", Env(new()), App);
        Assert.Equal(Path.Combine(@"C:\Users\ash", "AppData", "Roaming", App), dir);
    }

    [Fact]
    public void Windows_Data_UsesLocalAppData()
    {
        var env = Env(new() { ["LOCALAPPDATA"] = @"C:\Users\ash\AppData\Local" });
        var dir = AppPathResolver.ResolveDataDirectory(OSKind.Windows, @"C:\Users\ash", env, App);
        Assert.Equal(Path.Combine(@"C:\Users\ash\AppData\Local", App), dir);
    }

    [Fact]
    public void MacOS_Config_UsesApplicationSupport()
    {
        var dir = AppPathResolver.ResolveConfigDirectory(OSKind.MacOS, "/Users/ash", Env(new()), App);
        Assert.Equal(Path.Combine("/Users/ash", "Library", "Application Support", App), dir);
    }

    [Fact]
    public void MacOS_Data_SharesApplicationSupport()
    {
        var dir = AppPathResolver.ResolveDataDirectory(OSKind.MacOS, "/Users/ash", Env(new()), App);
        Assert.Equal(Path.Combine("/Users/ash", "Library", "Application Support", App), dir);
    }

    [Fact]
    public void Linux_Config_UsesXdgConfigHome_WhenSet()
    {
        var env = Env(new() { ["XDG_CONFIG_HOME"] = "/home/ash/.custom-config" });
        var dir = AppPathResolver.ResolveConfigDirectory(OSKind.Linux, "/home/ash", env, App);
        Assert.Equal(Path.Combine("/home/ash/.custom-config", App), dir);
    }

    [Fact]
    public void Linux_Config_FallsBackToDotConfig_WhenXdgUnset()
    {
        var dir = AppPathResolver.ResolveConfigDirectory(OSKind.Linux, "/home/ash", Env(new()), App);
        Assert.Equal(Path.Combine("/home/ash", ".config", App), dir);
    }

    [Fact]
    public void Linux_Config_FallsBackToDotConfig_WhenXdgBlank()
    {
        var env = Env(new() { ["XDG_CONFIG_HOME"] = "   " });
        var dir = AppPathResolver.ResolveConfigDirectory(OSKind.Linux, "/home/ash", env, App);
        Assert.Equal(Path.Combine("/home/ash", ".config", App), dir);
    }

    [Fact]
    public void Linux_Data_UsesXdgDataHome_WhenSet()
    {
        var env = Env(new() { ["XDG_DATA_HOME"] = "/home/ash/.custom-data" });
        var dir = AppPathResolver.ResolveDataDirectory(OSKind.Linux, "/home/ash", env, App);
        Assert.Equal(Path.Combine("/home/ash/.custom-data", App), dir);
    }

    [Fact]
    public void Linux_Data_FallsBackToLocalShare_WhenXdgUnset()
    {
        var dir = AppPathResolver.ResolveDataDirectory(OSKind.Linux, "/home/ash", Env(new()), App);
        Assert.Equal(Path.Combine("/home/ash", ".local", "share", App), dir);
    }
}
