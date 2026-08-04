using System.Threading;

namespace PKHeX.Application.Abstractions;

/// <summary>
/// Fetches release metadata for the update checker. Implemented in the Infrastructure layer
/// (an HTTP call to the GitHub Releases API); keeping this as a port lets the Application and
/// Presentation layers stay free of networking concerns, and lets tests substitute a fake.
/// </summary>
public interface IUpdateCheckService
{
    /// <summary>
    /// Returns all published (non-draft) releases for the app's repository, or <see langword="null"/>
    /// if the check could not complete (offline, rate-limited, timed out, malformed response, etc.).
    /// Callers must treat <see langword="null"/> as "stay silent" — no error dialog, just a log entry.
    /// </summary>
    Task<IReadOnlyList<ReleaseInfo>?> GetReleasesAsync(CancellationToken cancellationToken = default);
}

/// <summary>A single GitHub release, trimmed to what the update notification/changelog need.</summary>
public sealed record ReleaseInfo(
    string TagName, string Name, string Body, string HtmlUrl, bool Prerelease, IReadOnlyList<ReleaseAsset> Assets);

/// <summary>A downloadable file attached to a GitHub release (e.g. a per-platform zip).</summary>
/// <param name="Sha256">
/// The asset's SHA-256 digest (lowercase hex, no "sha256:" prefix), when GitHub reports one via the
/// release asset's <c>digest</c> field; <see langword="null"/> for older releases that predate digest
/// reporting, in which case the installer skips verification rather than failing.
/// </param>
public sealed record ReleaseAsset(string Name, string BrowserDownloadUrl, string? Sha256 = null, long Size = 0);
