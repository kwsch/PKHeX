using PKHeX.Application.Abstractions;
using PKHeX.Application.Services;

namespace PKHeX.Infrastructure.Updating;

/// <summary>
/// Platform-specific half of the self-update pipeline: given an already-downloaded and
/// checksum-verified asset, stage it and swap it in over the running install. Implementations never
/// touch the network — that's <see cref="GitHubUpdateInstaller"/>'s job — so they can be substituted
/// with a fake in tests that only need to assert "verified, then strategy invoked".
/// </summary>
internal interface IPlatformUpdateStrategy
{
    /// <summary>
    /// Installs the downloaded asset at <paramref name="downloadedFilePath"/> over the current
    /// install described by <paramref name="location"/>. Reports <see cref="UpdatePhase.Extracting"/>,
    /// <see cref="UpdatePhase.Swapping"/>, and <see cref="UpdatePhase.Relaunching"/> as it progresses.
    /// Cancellation is only honored up to the point the actual file swap begins — once a detached
    /// helper process is waiting to swap files, the operation cannot be cancelled.
    /// </summary>
    Task<UpdateInstallResult> InstallAsync(
        string downloadedFilePath, ReleaseAsset asset, InstallLocationInfo location,
        IProgress<UpdateProgress> progress, CancellationToken ct);
}
