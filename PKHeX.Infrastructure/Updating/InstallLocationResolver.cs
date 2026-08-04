using PKHeX.Application.Services;

namespace PKHeX.Infrastructure.Updating;

/// <summary>
/// Classifies how the running app was installed/launched, from <see cref="Environment.ProcessPath"/>
/// (deliberately not <c>AppContext.BaseDirectory</c>, which for a PublishSingleFile build resolves to
/// a throwaway extraction temp directory, not the real install location) and the <c>APPIMAGE</c>
/// environment variable set by the AppImage runtime.
/// </summary>
internal static class InstallLocationResolver
{
    public static InstallLocationInfo Resolve() =>
        Resolve(Environment.ProcessPath, Environment.GetEnvironmentVariable("APPIMAGE"), DefaultIsWritable);

    /// <summary>Testable overload: everything impure is passed in rather than read from the environment.</summary>
    internal static InstallLocationInfo Resolve(string? processPath, string? appImagePath, Func<string, bool> isWritable)
    {
        if (!string.IsNullOrEmpty(appImagePath))
            return new InstallLocationInfo(InstallKind.LinuxAppImage, appImagePath);

        if (string.IsNullOrEmpty(processPath))
            return new InstallLocationInfo(InstallKind.Unknown, string.Empty);

        if (OperatingSystem.IsMacOS())
        {
            var bundle = FindMacAppBundle(processPath);
            return bundle is null
                ? new InstallLocationInfo(InstallKind.Unknown, string.Empty)
                : new InstallLocationInfo(InstallKind.MacAppBundle, bundle);
        }

        var dir = Path.GetDirectoryName(processPath);
        if (string.IsNullOrEmpty(dir))
            return new InstallLocationInfo(InstallKind.Unknown, string.Empty);

        if (OperatingSystem.IsWindows())
        {
            return isWritable(dir)
                ? new InstallLocationInfo(InstallKind.WindowsPortable, dir)
                : new InstallLocationInfo(InstallKind.WindowsInstaller, dir);
        }

        if (OperatingSystem.IsLinux())
            return new InstallLocationInfo(InstallKind.LinuxPortable, dir);

        return new InstallLocationInfo(InstallKind.Unknown, string.Empty);
    }

    /// <summary>
    /// Walks up from &lt;bundle&gt;.app/Contents/MacOS/PKHeX.Avalonia to the .app bundle itself
    /// (three directory levels up from the executable).
    /// </summary>
    private static string? FindMacAppBundle(string processPath)
    {
        var macOsDir = Path.GetDirectoryName(processPath);          // .../Contents/MacOS
        var contentsDir = macOsDir is null ? null : Path.GetDirectoryName(macOsDir); // .../Contents
        var appDir = contentsDir is null ? null : Path.GetDirectoryName(contentsDir); // .../PKHeX.Avalonia.app

        return appDir is not null && appDir.EndsWith(".app", StringComparison.OrdinalIgnoreCase)
            ? appDir
            : null;
    }

    private static bool DefaultIsWritable(string directory)
    {
        try
        {
            var probePath = Path.Combine(directory, $".pkhex-update-write-probe-{Guid.NewGuid():N}.tmp");
            File.WriteAllBytes(probePath, [0]);
            File.Delete(probePath);
            return true;
        }
        catch
        {
            return false;
        }
    }
}
