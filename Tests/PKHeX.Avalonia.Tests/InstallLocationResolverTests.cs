using PKHeX.Infrastructure.Updating;

namespace PKHeX.Avalonia.Tests;

public class InstallLocationResolverTests
{
    // APPIMAGE takes priority over everything else, and doesn't depend on the running OS branch,
    // so it's the one case this suite can assert unconditionally on any CI platform.
    [Fact]
    public void AppImage_env_set_wins_regardless_of_process_path()
    {
        var result = InstallLocationResolver.Resolve("/some/random/path/PKHeX.Avalonia", "/home/user/PKHeX.AppImage", _ => true);

        Assert.Equal(InstallKind.LinuxAppImage, result.Kind);
        Assert.Equal("/home/user/PKHeX.AppImage", result.Root);
    }

    [Fact]
    public void Null_process_path_and_no_appimage_is_unknown()
    {
        var result = InstallLocationResolver.Resolve(null, null, _ => true);

        Assert.Equal(InstallKind.Unknown, result.Kind);
    }

    // The remaining branches key off OperatingSystem.IsWindows()/IsMacOS()/IsLinux() (the resolver
    // deliberately doesn't take an injectable OS, only an injectable writability predicate — see
    // design notes), so they only assert on the OS the test suite is actually running on.

    [Fact]
    public void MacOs_app_bundle_is_resolved_three_levels_up_from_the_process_path()
    {
        if (!OperatingSystem.IsMacOS())
            return;

        var processPath = "/Applications/PKHeX.Avalonia.app/Contents/MacOS/PKHeX.Avalonia";
        var result = InstallLocationResolver.Resolve(processPath, null, _ => true);

        Assert.Equal(InstallKind.MacAppBundle, result.Kind);
        Assert.Equal("/Applications/PKHeX.Avalonia.app", result.Root);
    }

    [Fact]
    public void MacOs_process_path_outside_an_app_bundle_is_unknown()
    {
        if (!OperatingSystem.IsMacOS())
            return;

        var result = InstallLocationResolver.Resolve("/usr/local/bin/PKHeX.Avalonia", null, _ => true);

        Assert.Equal(InstallKind.Unknown, result.Kind);
    }

    [Fact]
    public void Windows_writable_directory_is_portable()
    {
        if (!OperatingSystem.IsWindows())
            return;

        var result = InstallLocationResolver.Resolve(@"C:\Users\Test\PKHeX\PKHeX.Avalonia.exe", null, _ => true);

        Assert.Equal(InstallKind.WindowsPortable, result.Kind);
        Assert.Equal(@"C:\Users\Test\PKHeX", result.Root);
    }

    [Fact]
    public void Windows_non_writable_directory_is_installer_owned()
    {
        if (!OperatingSystem.IsWindows())
            return;

        var result = InstallLocationResolver.Resolve(@"C:\Program Files\PKHeX\PKHeX.Avalonia.exe", null, _ => false);

        Assert.Equal(InstallKind.WindowsInstaller, result.Kind);
        Assert.Equal(@"C:\Program Files\PKHeX", result.Root);
    }

    [Fact]
    public void Linux_process_path_is_portable()
    {
        if (!OperatingSystem.IsLinux())
            return;

        var result = InstallLocationResolver.Resolve("/opt/pkhex/PKHeX.Avalonia", null, _ => true);

        Assert.Equal(InstallKind.LinuxPortable, result.Kind);
        Assert.Equal("/opt/pkhex", result.Root);
    }
}
