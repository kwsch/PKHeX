namespace PKHeX.Avalonia.Tests;

public class UpdateAssetSelectorTests
{
    // The real v1.39.x asset name set, plus hypothetical "-selfsigned.dmg" variants that don't
    // exist yet in a real release but are the target of the self-signed macOS identity work.
    private static readonly ReleaseAsset[] RealAssets =
    [
        Asset("PKHeX-Avalonia-win-x64.zip"),
        Asset("PKHeX-Avalonia-Setup-unsigned.exe"),
        Asset("PKHeX-Avalonia-linux-x64.zip"),
        Asset("PKHeX-Avalonia-linux-x64.AppImage"),
        Asset("PKHeX-Avalonia-osx-arm64.zip"),
        Asset("PKHeX-Avalonia-osx-arm64-unsigned.dmg"),
        Asset("PKHeX-Avalonia-osx-x64.zip"),
        Asset("PKHeX-Avalonia-osx-x64-unsigned.dmg"),
    ];

    private static ReleaseAsset Asset(string name) => new(name, $"https://example.com/{name}");

    [Fact]
    public void MacOs_prefers_selfsigned_dmg_over_unsigned_dmg_and_never_picks_zip()
    {
        var assets = RealAssets.Append(Asset("PKHeX-Avalonia-osx-arm64-selfsigned.dmg")).ToArray();

        var selected = UpdateAssetSelector.SelectAsset(assets, "macos", "arm64", InstallKind.MacAppBundle);

        Assert.NotNull(selected);
        Assert.Equal("PKHeX-Avalonia-osx-arm64-selfsigned.dmg", selected!.Name);
    }

    [Fact]
    public void MacOs_falls_back_to_unsigned_dmg_when_no_selfsigned_or_plain_dmg_exists()
    {
        var selected = UpdateAssetSelector.SelectAsset(RealAssets, "macos", "arm64", InstallKind.MacAppBundle);

        Assert.NotNull(selected);
        Assert.Equal("PKHeX-Avalonia-osx-arm64-unsigned.dmg", selected!.Name);
    }

    [Fact]
    public void MacOs_prefers_plain_dmg_over_unsigned_dmg()
    {
        var assets = RealAssets.Append(Asset("PKHeX-Avalonia-osx-x64.dmg")).ToArray();

        var selected = UpdateAssetSelector.SelectAsset(assets, "macos", "x64", InstallKind.MacAppBundle);

        Assert.NotNull(selected);
        Assert.Equal("PKHeX-Avalonia-osx-x64.dmg", selected!.Name);
    }

    [Fact]
    public void MacOs_never_selects_the_zip_build()
    {
        // Only a zip is available for this made-up architecture: self-update must refuse rather
        // than fall back to the ad-hoc-signed zip (DR/notarization preservation).
        var assets = new[] { Asset("PKHeX-Avalonia-osx-arm64.zip") };

        var selected = UpdateAssetSelector.SelectAsset(assets, "macos", "arm64", InstallKind.MacAppBundle);

        Assert.Null(selected);
    }

    [Fact]
    public void Windows_portable_picks_the_zip()
    {
        var selected = UpdateAssetSelector.SelectAsset(RealAssets, "windows", "x64", InstallKind.WindowsPortable);

        Assert.NotNull(selected);
        Assert.Equal("PKHeX-Avalonia-win-x64.zip", selected!.Name);
    }

    [Fact]
    public void Windows_installer_picks_the_setup_exe()
    {
        var selected = UpdateAssetSelector.SelectAsset(RealAssets, "windows", "x64", InstallKind.WindowsInstaller);

        Assert.NotNull(selected);
        Assert.Equal("PKHeX-Avalonia-Setup-unsigned.exe", selected!.Name);
    }

    [Fact]
    public void Linux_appimage_picks_the_appimage_over_the_zip()
    {
        var selected = UpdateAssetSelector.SelectAsset(RealAssets, "linux", "x64", InstallKind.LinuxAppImage);

        Assert.NotNull(selected);
        Assert.Equal("PKHeX-Avalonia-linux-x64.AppImage", selected!.Name);
    }

    [Fact]
    public void Linux_portable_picks_the_zip()
    {
        var selected = UpdateAssetSelector.SelectAsset(RealAssets, "linux", "x64", InstallKind.LinuxPortable);

        Assert.NotNull(selected);
        Assert.Equal("PKHeX-Avalonia-linux-x64.zip", selected!.Name);
    }

    [Fact]
    public void Unknown_kind_returns_null()
    {
        var selected = UpdateAssetSelector.SelectAsset(RealAssets, "macos", "arm64", InstallKind.Unknown);

        Assert.Null(selected);
    }

    [Fact]
    public void No_matching_asset_returns_null()
    {
        var assets = new[] { Asset("SomethingUnrelated.txt") };

        var selected = UpdateAssetSelector.SelectAsset(assets, "windows", "x64", InstallKind.WindowsPortable);

        Assert.Null(selected);
    }
}
