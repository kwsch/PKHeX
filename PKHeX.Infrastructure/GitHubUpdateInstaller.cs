using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using PKHeX.Application.Abstractions;
using PKHeX.Application.Services;
using PKHeX.Infrastructure.Updating;

namespace PKHeX.Infrastructure;

/// <summary>
/// Downloads a release asset with progress reporting, verifies its SHA-256 checksum, then dispatches
/// to the platform-specific <see cref="IPlatformUpdateStrategy"/> to actually install it. Every
/// failure mode maps to a localization key rather than an exception surfacing to the caller, except
/// <see cref="OperationCanceledException"/>, which propagates so callers can distinguish "the user
/// cancelled" from "it failed".
/// </summary>
public sealed class GitHubUpdateInstaller : IUpdateInstaller
{
    private readonly HttpClient _client;
    private readonly IReadOnlyDictionary<InstallKind, IPlatformUpdateStrategy> _strategies;
    private readonly Func<InstallLocationInfo> _resolveLocation;

    public GitHubUpdateInstaller() : this(CreateDefaultClient(), DefaultStrategies(), InstallLocationResolver.Resolve)
    {
    }

    // Internal for testability: lets tests inject a fake HttpClient handler, fake strategies, and a
    // synthetic install location instead of touching the network/file system/real platform.
    internal GitHubUpdateInstaller(
        HttpClient client, IReadOnlyDictionary<InstallKind, IPlatformUpdateStrategy> strategies, Func<InstallLocationInfo> resolveLocation)
    {
        _client = client;
        _strategies = strategies;
        _resolveLocation = resolveLocation;
    }

    private static HttpClient CreateDefaultClient() => new() { Timeout = TimeSpan.FromMinutes(15) };

    private static IReadOnlyDictionary<InstallKind, IPlatformUpdateStrategy> DefaultStrategies() =>
        new Dictionary<InstallKind, IPlatformUpdateStrategy>
        {
            [InstallKind.MacAppBundle] = new MacOsUpdateStrategy(),
            [InstallKind.WindowsPortable] = new WindowsUpdateStrategy(),
            [InstallKind.WindowsInstaller] = new WindowsUpdateStrategy(),
            [InstallKind.LinuxAppImage] = new LinuxUpdateStrategy(),
            [InstallKind.LinuxPortable] = new LinuxUpdateStrategy(),
        };

    public InstallKind CurrentInstallKind => _resolveLocation().Kind;

    public bool CanSelfUpdate(out string? reason)
    {
        var location = _resolveLocation();
        if (location.Kind == InstallKind.Unknown || !_strategies.ContainsKey(location.Kind))
        {
            reason = "Update_Error_Generic";
            return false;
        }

        reason = null;
        return true;
    }

    public async Task<UpdateInstallResult> DownloadAndInstallAsync(ReleaseAsset asset, IProgress<UpdateProgress> progress, CancellationToken ct)
    {
        var location = _resolveLocation();
        if (!_strategies.TryGetValue(location.Kind, out var strategy))
            return new UpdateInstallResult(false, false, "Update_Error_Generic");

            var sanitizedName = SanitizeFileName(asset.Name);
            var tempPath = Path.Combine(Path.GetTempPath(), $"{sanitizedName}.part");
            var finalPath = Path.Combine(Path.GetTempPath(), sanitizedName);
            try
            {
                progress.Report(new UpdateProgress(UpdatePhase.Downloading, 0, asset.Size > 0 ? asset.Size : null));
                await DownloadAsync(asset, tempPath, progress, ct).ConfigureAwait(false);

                progress.Report(new UpdateProgress(UpdatePhase.Verifying, 0, null));
                if (!await VerifyChecksumAsync(tempPath, asset.Sha256, ct).ConfigureAwait(false))
                {
                    TryDelete(tempPath);
                    return new UpdateInstallResult(false, false, "Update_Error_Checksum");
                }

                // Rename from .part to the proper filename so platform strategies receive the correct extension.
                File.Move(tempPath, finalPath, overwrite: true);

                return await strategy.InstallAsync(finalPath, asset, location, progress, ct).ConfigureAwait(false);
        }
            catch (OperationCanceledException)
        {
                TryDelete(tempPath);
                TryDelete(finalPath);
                throw;
        }
            catch (Exception ex)
            {
                Trace.TraceWarning($"Update install failed: {ex.Message}");
                TryDelete(tempPath);
                TryDelete(finalPath);
                return new UpdateInstallResult(false, false, "Update_Error_Generic");
            }
        }

    private async Task DownloadAsync(ReleaseAsset asset, string destinationPath, IProgress<UpdateProgress> progress, CancellationToken ct)
    {
        using var response = await _client.GetAsync(asset.BrowserDownloadUrl, HttpCompletionOption.ResponseHeadersRead, ct).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();

        var totalBytes = response.Content.Headers.ContentLength ?? (asset.Size > 0 ? asset.Size : (long?)null);

        await using var httpStream = await response.Content.ReadAsStreamAsync(ct).ConfigureAwait(false);
        await using var fileStream = new FileStream(destinationPath, FileMode.Create, FileAccess.Write, FileShare.None);

        var buffer = new byte[81920];
        long bytesReceived = 0;
        int read;
        while ((read = await httpStream.ReadAsync(buffer, ct).ConfigureAwait(false)) > 0)
        {
            await fileStream.WriteAsync(buffer.AsMemory(0, read), ct).ConfigureAwait(false);
            bytesReceived += read;
            progress.Report(new UpdateProgress(UpdatePhase.Downloading, bytesReceived, totalBytes));
        }
    }

    /// <summary>
    /// Returns <see langword="true"/> when the file's SHA-256 matches <paramref name="expectedSha256"/>.
    /// When the release didn't publish a digest (older releases predate GitHub's digest reporting),
    /// verification is skipped — logged, not treated as a failure — rather than blocking the update.
    /// </summary>
    private static async Task<bool> VerifyChecksumAsync(string filePath, string? expectedSha256, CancellationToken ct)
    {
        if (string.IsNullOrEmpty(expectedSha256))
        {
            Trace.TraceWarning($"Update asset '{filePath}' has no published checksum; skipping verification.");
            return true;
        }

        await using var stream = File.OpenRead(filePath);
        var hash = await SHA256.HashDataAsync(stream, ct).ConfigureAwait(false);
        var hex = Convert.ToHexStringLower(hash);
        return string.Equals(hex, expectedSha256, StringComparison.OrdinalIgnoreCase);
    }

    private static string SanitizeFileName(string name)
    {
        foreach (var invalid in Path.GetInvalidFileNameChars())
            name = name.Replace(invalid, '_');
        return name;
    }

    private static void TryDelete(string path)
    {
        try
        {
            if (File.Exists(path))
                File.Delete(path);
        }
        catch
        {
            // best-effort cleanup
        }
    }
}
