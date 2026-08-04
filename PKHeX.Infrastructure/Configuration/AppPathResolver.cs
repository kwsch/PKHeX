using System;
using System.IO;
using System.Runtime.InteropServices;

namespace PKHeX.Infrastructure.Configuration;

/// <summary>The three platform families we resolve paths for.</summary>
public enum OSKind
{
    Windows,
    MacOS,
    Linux,
}

/// <summary>
/// Pure, side-effect-free resolution of platform-standard config/data directories. All OS and
/// environment inputs are passed in explicitly so the logic can be unit-tested for every platform
/// regardless of the host the tests run on.
/// </summary>
public static class AppPathResolver
{
    /// <summary>Detects the current host platform.</summary>
    public static OSKind DetectOS()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            return OSKind.Windows;
        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            return OSKind.MacOS;
        return OSKind.Linux;
    }

    /// <summary>
    /// Resolves the per-user configuration directory for <paramref name="appName"/>.
    /// Windows: <c>%APPDATA%\appName</c>. macOS: <c>~/Library/Application Support/appName</c>.
    /// Linux: <c>$XDG_CONFIG_HOME/appName</c>, falling back to <c>~/.config/appName</c>.
    /// </summary>
    public static string ResolveConfigDirectory(OSKind os, string home, Func<string, string?> getEnv, string appName)
    {
        return os switch
        {
            OSKind.Windows => Path.Combine(WindowsRoaming(getEnv, home), appName),
            OSKind.MacOS => Path.Combine(home, "Library", "Application Support", appName),
            _ => Path.Combine(NonEmptyEnv(getEnv, "XDG_CONFIG_HOME") ?? Path.Combine(home, ".config"), appName),
        };
    }

    /// <summary>
    /// Resolves the per-user data directory (backups, caches) for <paramref name="appName"/>.
    /// Windows: <c>%LOCALAPPDATA%\appName</c>. macOS: shares Application Support with config.
    /// Linux: <c>$XDG_DATA_HOME/appName</c>, falling back to <c>~/.local/share/appName</c>.
    /// </summary>
    public static string ResolveDataDirectory(OSKind os, string home, Func<string, string?> getEnv, string appName)
    {
        return os switch
        {
            OSKind.Windows => Path.Combine(WindowsLocal(getEnv, home), appName),
            OSKind.MacOS => Path.Combine(home, "Library", "Application Support", appName),
            _ => Path.Combine(NonEmptyEnv(getEnv, "XDG_DATA_HOME") ?? Path.Combine(home, ".local", "share"), appName),
        };
    }

    private static string WindowsRoaming(Func<string, string?> getEnv, string home)
        => NonEmptyEnv(getEnv, "APPDATA") ?? Path.Combine(home, "AppData", "Roaming");

    private static string WindowsLocal(Func<string, string?> getEnv, string home)
        => NonEmptyEnv(getEnv, "LOCALAPPDATA") ?? Path.Combine(home, "AppData", "Local");

    private static string? NonEmptyEnv(Func<string, string?> getEnv, string key)
    {
        var value = getEnv(key);
        return string.IsNullOrWhiteSpace(value) ? null : value;
    }
}
