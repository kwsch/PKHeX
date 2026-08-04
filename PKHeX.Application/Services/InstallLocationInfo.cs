namespace PKHeX.Application.Services;

/// <summary>
/// How the running app was installed/launched, as it matters to the self-update pipeline: where the
/// swap needs to happen, and whether it needs elevated rights.
/// </summary>
public enum InstallKind
{
    /// <summary>macOS <c>.app</c> bundle (DMG-distributed or otherwise).</summary>
    MacAppBundle,

    /// <summary>Windows portable build (extracted zip), running from a user-writable directory.</summary>
    WindowsPortable,

    /// <summary>Windows build installed by the Inno Setup installer into an owned (e.g. Program Files) directory.</summary>
    WindowsInstaller,

    /// <summary>Linux AppImage (running with the <c>APPIMAGE</c> environment variable set).</summary>
    LinuxAppImage,

    /// <summary>Linux portable build (extracted zip), running from a user-writable directory.</summary>
    LinuxPortable,

    /// <summary>Could not be classified — self-update is not attempted.</summary>
    Unknown,
}

/// <summary>
/// Where the running app lives and how it was installed. Produced by
/// <c>PKHeX.Infrastructure.Updating.InstallLocationResolver</c> (Infrastructure, since resolving it
/// touches <see cref="Environment.ProcessPath"/>/environment variables/file-system writability) but
/// the record itself is a pure value type kept in Application so <c>UpdateAssetSelector</c> can
/// depend on it without pulling in Infrastructure.
/// </summary>
/// <param name="Kind">The classified install kind.</param>
/// <param name="Root">
/// The root path relevant to the swap: the <c>.app</c> bundle path on macOS, the install directory on
/// Windows/Linux portable builds, the owned install directory for the Windows installer, or the
/// AppImage file path on Linux AppImage.
/// </param>
public sealed record InstallLocationInfo(InstallKind Kind, string Root);
