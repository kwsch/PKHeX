using PKHeX.Application.Abstractions;

namespace PKHeX.Application.Services;

/// <summary>
/// Pure decision logic for the update checker: which release is "latest", whether the user should
/// be notified, and which releases fall between the installed and latest version for changelog
/// display. No I/O, so it can be unit tested without a mocked network call.
/// </summary>
public static class UpdateAvailabilityEvaluator
{
    /// <summary>Returns the highest-precedence stable (non-prerelease) release with a parseable tag, or null if none.</summary>
    public static ReleaseInfo? GetLatestRelease(IEnumerable<ReleaseInfo> releases)
    {
        ReleaseInfo? latest = null;
        var latestVersion = default(SemanticVersion);

        foreach (var release in releases)
        {
            if (release.Prerelease)
                continue;
            if (!SemanticVersion.TryParse(release.TagName, out var version))
                continue;
            if (latest is null || version > latestVersion)
            {
                latest = release;
                latestVersion = version;
            }
        }

        return latest;
    }

    /// <summary>
    /// True when <paramref name="latestTag"/> is newer than <paramref name="currentVersion"/> and the
    /// user has not explicitly skipped that version.
    /// </summary>
    public static bool ShouldNotify(string currentVersion, string latestTag, string? skippedVersion)
    {
        if (!SemanticVersion.TryParse(currentVersion, out var current))
            return false;
        if (!SemanticVersion.TryParse(latestTag, out var latest))
            return false;
        if (latest <= current)
            return false;
        if (!string.IsNullOrEmpty(skippedVersion)
            && SemanticVersion.TryParse(skippedVersion, out var skipped)
            && skipped == latest)
            return false;

        return true;
    }

    /// <summary>Releases newer than <paramref name="currentVersion"/>, newest first, for changelog display.</summary>
    public static IReadOnlyList<ReleaseInfo> GetReleasesNewerThan(IEnumerable<ReleaseInfo> releases, string currentVersion)
    {
        if (!SemanticVersion.TryParse(currentVersion, out var current))
            return [];

        var newer = new List<(ReleaseInfo Release, SemanticVersion Version)>();
        foreach (var release in releases)
        {
            if (SemanticVersion.TryParse(release.TagName, out var version) && version > current)
                newer.Add((release, version));
        }

        newer.Sort((a, b) => b.Version.CompareTo(a.Version));
        return newer.Select(x => x.Release).ToList();
    }
}
