using PKHeX.Application.Abstractions;

namespace PKHeX.Application.Services;

/// <summary>
/// Pure logic for picking the release asset that matches the running OS/architecture (e.g. an
/// asset named "PKHeX-Avalonia-win-x64.zip" for a Windows x64 install). Takes plain os/arch
/// strings rather than querying the platform itself, so it can be unit tested without a real
/// OS/architecture and without any I/O.
/// </summary>
public static class AssetSelector
{
    private static readonly (string Canonical, string[] Aliases)[] OsTokens =
    [
        ("windows", ["windows", "win", "win64", "win32"]),
        ("macos", ["macos", "osx", "mac", "darwin"]),
        ("linux", ["linux"]),
    ];

    private static readonly (string Canonical, string[] Aliases)[] ArchTokens =
    [
        ("arm64", ["arm64", "aarch64"]),
        ("x64", ["x64", "amd64", "x86_64", "x86-64"]),
        ("x86", ["x86", "win32", "i386", "i686"]),
    ];

    /// <summary>
    /// Returns the asset whose file name best matches <paramref name="os"/> and <paramref name="arch"/>,
    /// or <see langword="null"/> if no asset matches. Prefers an asset that matches both OS and
    /// architecture; falls back to an OS-only match (common for single-architecture builds, or
    /// platforms where the asset name omits the architecture) when no exact combination exists.
    /// </summary>
    public static ReleaseAsset? SelectAsset(IEnumerable<ReleaseAsset> assets, string? os, string? arch)
    {
        var osAliases = ResolveAliases(OsTokens, os);
        if (osAliases.Length == 0)
            return null;

        var list = assets as IReadOnlyList<ReleaseAsset> ?? assets.ToList();
        var archAliases = ResolveAliases(ArchTokens, arch);

        if (archAliases.Length > 0)
        {
            var exact = list.FirstOrDefault(a => ContainsAny(a.Name, osAliases) && ContainsAny(a.Name, archAliases));
            if (exact is not null)
                return exact;
        }

        return list.FirstOrDefault(a => ContainsAny(a.Name, osAliases));
    }

    private static string[] ResolveAliases((string Canonical, string[] Aliases)[] table, string? value)
    {
        if (string.IsNullOrEmpty(value))
            return [];

        foreach (var (canonical, aliases) in table)
        {
            if (string.Equals(canonical, value, StringComparison.OrdinalIgnoreCase)
                || aliases.Any(a => string.Equals(a, value, StringComparison.OrdinalIgnoreCase)))
                return aliases;
        }

        return [];
    }

    private static bool ContainsAny(string name, IEnumerable<string> tokens) =>
        tokens.Any(t => name.Contains(t, StringComparison.OrdinalIgnoreCase));
}
