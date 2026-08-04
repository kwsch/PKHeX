using System.Runtime.InteropServices;

namespace PKHeX.Presentation.ViewModels;

/// <summary>
/// Thin, impure wrapper around the running OS/architecture, kept separate from
/// <see cref="Services.AssetSelector"/> so that asset-matching logic itself stays a pure function
/// of plain os/arch strings and is testable without depending on the real platform.
/// </summary>
internal static class CurrentPlatform
{
    public static string Os => OperatingSystem.IsWindows() ? "windows"
        : OperatingSystem.IsMacOS() ? "macos"
        : OperatingSystem.IsLinux() ? "linux"
        : "unknown";

    public static string Arch => RuntimeInformation.ProcessArchitecture switch
    {
        Architecture.Arm64 => "arm64",
        Architecture.X64 => "x64",
        Architecture.X86 => "x86",
        var other => other.ToString(),
    };
}
