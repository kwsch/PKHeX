using PKHeX.Application.Abstractions;

namespace PKHeX.Application.Services;

/// <summary>
/// Pure, deterministic logic for picking the release asset the self-update pipeline should download,
/// given the running OS/architecture and how the app was installed (<see cref="InstallKind"/>). This
/// is deliberately separate from <see cref="AssetSelector"/> (used for the "open in browser" fallback,
/// which is happy to hand the user any OS-matching asset): self-update has stricter requirements —
/// e.g. on macOS it must never pick the ad-hoc-signed <c>.zip</c> build, because that build cannot
/// carry Gatekeeper's Developer ID/notarization ticket and DMG-only distribution is required to
/// preserve Disaster Recovery (DR) signing continuity across updates.
/// </summary>
public static class UpdateAssetSelector
{
    private static readonly (string Canonical, string[] Aliases)[] ArchTokens =
    [
        ("arm64", ["arm64", "aarch64"]),
        ("x64", ["x64", "amd64", "x86_64", "x86-64"]),
        ("x86", ["x86", "win32", "i386", "i686"]),
    ];

    /// <summary>
    /// Returns the single best asset to self-update with, or <see langword="null"/> if none of the
    /// release's assets are suitable for <paramref name="kind"/> on this OS/architecture.
    /// </summary>
    /// <param name="os">
    /// The running OS ("windows"/"macos"/"linux"), kept for parity with <see cref="AssetSelector"/>
    /// and for callers/tests that want to assert it against <paramref name="kind"/>. Filtering itself
    /// is driven by <paramref name="kind"/>, which already implies the OS.
    /// </param>
    public static ReleaseAsset? SelectAsset(IEnumerable<ReleaseAsset> assets, string? os, string? arch, InstallKind kind)
    {
        _ = os;
        var list = assets as IReadOnlyList<ReleaseAsset> ?? assets.ToList();
        var archAliases = ResolveArchAliases(arch);

        return kind switch
        {
            InstallKind.MacAppBundle => SelectMac(list, archAliases),
            InstallKind.WindowsPortable => SelectFirst(list, archAliases, a => IsZip(a.Name) && ContainsWinToken(a.Name)),
            InstallKind.WindowsInstaller => SelectFirst(list, archAliases, a => IsExe(a.Name) && a.Name.Contains("Setup", StringComparison.OrdinalIgnoreCase)),
            InstallKind.LinuxAppImage => SelectFirst(list, archAliases, a => a.Name.EndsWith(".AppImage", StringComparison.OrdinalIgnoreCase)),
            InstallKind.LinuxPortable => SelectFirst(list, archAliases, a => IsZip(a.Name) && ContainsLinuxToken(a.Name)),
            _ => null,
        };
    }

    private static ReleaseAsset? SelectMac(IReadOnlyList<ReleaseAsset> assets, string[] archAliases)
    {
        // Never the ad-hoc-signed .zip — DR/notarization preservation. Prefer the self-signed DMG,
        // then a plain (unsuffixed) DMG, then the unsigned DMG as a last resort. Prefer an
        // arch-tagged match but fall back to any DMG if none is arch-tagged (e.g. a universal build).
        var dmgs = assets.Where(a => IsDmg(a.Name)).ToList();
        var candidates = dmgs.Where(a => MatchesArch(a.Name, archAliases)).ToList();
        if (candidates.Count == 0)
            candidates = dmgs;

        return candidates.FirstOrDefault(a => a.Name.Contains("-selfsigned.dmg", StringComparison.OrdinalIgnoreCase))
            ?? candidates.FirstOrDefault(a => !a.Name.Contains("-unsigned.dmg", StringComparison.OrdinalIgnoreCase)
                                               && !a.Name.Contains("-selfsigned.dmg", StringComparison.OrdinalIgnoreCase))
            ?? candidates.FirstOrDefault(a => a.Name.Contains("-unsigned.dmg", StringComparison.OrdinalIgnoreCase));
    }

    // Prefers an arch-tagged match, but falls back to any predicate match when the asset naming
    // scheme doesn't encode architecture at all (e.g. a single-architecture Windows installer or
    // AppImage build), the same fallback AssetSelector uses for the OS-only case.
    private static ReleaseAsset? SelectFirst(
        IReadOnlyList<ReleaseAsset> assets, string[] archAliases, Func<ReleaseAsset, bool> predicate)
    {
        var candidates = assets.Where(predicate).ToList();
        return candidates.FirstOrDefault(a => MatchesArch(a.Name, archAliases))
            ?? candidates.FirstOrDefault();
    }

    private static bool MatchesArch(string name, string[] archAliases) =>
        archAliases.Length == 0 || archAliases.Any(t => name.Contains(t, StringComparison.OrdinalIgnoreCase));

    private static bool ContainsWinToken(string name) => name.Contains("win", StringComparison.OrdinalIgnoreCase);

    private static bool ContainsLinuxToken(string name) => name.Contains("linux", StringComparison.OrdinalIgnoreCase);

    private static bool IsDmg(string name) => name.EndsWith(".dmg", StringComparison.OrdinalIgnoreCase);

    private static bool IsZip(string name) => name.EndsWith(".zip", StringComparison.OrdinalIgnoreCase);

    private static bool IsExe(string name) => name.EndsWith(".exe", StringComparison.OrdinalIgnoreCase);

    private static string[] ResolveArchAliases(string? value)
    {
        if (string.IsNullOrEmpty(value))
            return [];

        foreach (var (canonical, aliases) in ArchTokens)
        {
            if (string.Equals(canonical, value, StringComparison.OrdinalIgnoreCase)
                || aliases.Any(a => string.Equals(a, value, StringComparison.OrdinalIgnoreCase)))
                return aliases;
        }

        return [];
    }
}
