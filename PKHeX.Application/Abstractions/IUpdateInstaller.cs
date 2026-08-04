using PKHeX.Application.Services;

namespace PKHeX.Application.Abstractions;

/// <summary>
/// Downloads a release asset, verifies it, and installs it over the running application —
/// implemented in the Infrastructure layer so the platform-specific process/file-system work
/// (hdiutil, zip extraction, helper scripts, Process.Start) never leaks into Presentation.
/// </summary>
public interface IUpdateInstaller
{
    /// <summary>
    /// Reports whether this install can be self-updated in place. When <see langword="false"/>,
    /// <paramref name="reason"/> is a localization key (e.g. "Update_Error_NeedsAdmin") explaining
    /// why, so the caller can fall back to opening the download page in a browser.
    /// </summary>
    bool CanSelfUpdate(out string? reason);

    /// <summary>
    /// The classified install kind of the running app (e.g. macOS app bundle, Windows portable).
    /// Exposed so Presentation-layer callers can pick the right release asset via
    /// <see cref="UpdateAssetSelector"/> without depending on the Infrastructure layer.
    /// </summary>
    InstallKind CurrentInstallKind { get; }

    /// <summary>
    /// Downloads <paramref name="asset"/>, verifies its checksum, and installs it over the running
    /// application. On success with <see cref="UpdateInstallResult.WillRelaunch"/> set, the caller
    /// should exit the application shortly after this returns — a detached helper process performs
    /// the actual file swap and relaunch once this process has exited.
    /// </summary>
    Task<UpdateInstallResult> DownloadAndInstallAsync(ReleaseAsset asset, IProgress<UpdateProgress> progress, CancellationToken ct);
}

/// <summary>A step in the download/install pipeline, for progress reporting.</summary>
public enum UpdatePhase
{
    Downloading,
    Verifying,
    Extracting,
    Swapping,
    Relaunching,
}

/// <summary>
/// A progress snapshot for the update pipeline. <see cref="BytesReceived"/>/<see cref="TotalBytes"/>
/// are only meaningful during <see cref="UpdatePhase.Downloading"/>; other phases report indeterminate
/// progress (the UI should show a spinner, not a percentage).
/// </summary>
public readonly record struct UpdateProgress(UpdatePhase Phase, long BytesReceived, long? TotalBytes);

/// <summary>
/// The outcome of <see cref="IUpdateInstaller.DownloadAndInstallAsync"/>.
/// </summary>
/// <param name="Success">Whether the download+verify+install pipeline completed successfully.</param>
/// <param name="WillRelaunch">
/// When <see langword="true"/>, a detached helper process is waiting for this process to exit before
/// it swaps files and relaunches the app — the caller must exit the application soon after this
/// result is returned.
/// </param>
/// <param name="ErrorKey">
/// A localization key describing the failure (e.g. "Update_Error_Checksum"), or <see langword="null"/>
/// on success.
/// </param>
public sealed record UpdateInstallResult(bool Success, bool WillRelaunch, string? ErrorKey);
