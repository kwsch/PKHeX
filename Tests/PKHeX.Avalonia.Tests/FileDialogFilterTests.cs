using Avalonia.Platform.Storage;
using PKHeX.Avalonia.Services;

namespace PKHeX.Avalonia.Tests;

/// <summary>
/// Regression tests for the file-open dialog filters (.main save files unselectable on macOS).
///
/// Avalonia's macOS native dialog does NOT use glob patterns directly. It converts each
/// FilePickerFileType to a list of file extensions via TryGetExtensions(), silently dropping
/// any pattern without a ".ext" suffix (e.g. "main", "*"). A type only allows arbitrary
/// files when its patterns contain the literal "*.*" (IsAnyType). With the old filters
/// ["*.sav", "*.bin", "main", "*"], the default "Supported Files" entry collapsed to just
/// {sav, bin} on macOS, greying out .main Switch saves in the picker.
/// </summary>
public class FileDialogFilterTests
{
    /// <summary>
    /// Mirror of Avalonia 11.2.3 FilePickerFileType.TryGetExtensions()
    /// (src/Avalonia.Base/Platform/Storage/FilePickerFileType.cs) — what the macOS
    /// native dialog actually receives for a filter type.
    /// </summary>
    private static string[] GetMacOsExtensions(FilePickerFileType type) =>
        (type.Patterns ?? [])
        .Select(Path.GetExtension)
        .Where(e => !string.IsNullOrEmpty(e) && !e.Contains('*') && e.StartsWith('.'))
        .Select(e => e!.TrimStart('.'))
        .ToArray();

    /// <summary>
    /// Mirror of Avalonia's IsAnyType check (Avalonia.Native StorageProviderApi.cs):
    /// only the literal "*.*" pattern makes a filter type allow any file on macOS.
    /// </summary>
    private static bool IsMacOsAnyType(FilePickerFileType type) =>
        type.Patterns?.Contains("*.*") == true;

    [Fact]
    public void OpenSavePatterns_IncludeDotMainExtension()
    {
        Assert.Contains("*.main", FileDialogFilters.OpenSaveFile);
    }

    [Fact]
    public void OpenSavePatterns_KeepExtensionlessMainAndWildcard()
    {
        // Windows/Linux match the literal file name "main" and arbitrary names via glob.
        Assert.Contains("main", FileDialogFilters.OpenSaveFile);
        Assert.Contains("*", FileDialogFilters.OpenSaveFile);
    }

    [Fact]
    public void SupportedFiles_OnMacOs_AllowMainExtension()
    {
        var types = FileDialogFilterFactory.BuildOpenFileTypes(FileDialogFilters.OpenSaveFile);
        var supported = types[0];

        Assert.Equal("Supported Files", supported.Name);
        Assert.Contains("main", GetMacOsExtensions(supported));
    }

    [Fact]
    public void SupportedFiles_WithWildcardIntent_AllowAnyFileOnMacOs()
    {
        // An extensionless Switch save named "main" cannot be expressed as a macOS file
        // extension, so a caller passing a wildcard must yield an any-type filter there.
        var types = FileDialogFilterFactory.BuildOpenFileTypes(["*.sav", "*"]);
        var supported = types[0];

        Assert.True(IsMacOsAnyType(supported));
    }

    [Fact]
    public void SupportedFiles_WithoutWildcard_KeepStrictFiltering()
    {
        var types = FileDialogFilterFactory.BuildOpenFileTypes(["*.pl6"]);
        var supported = types[0];

        Assert.False(IsMacOsAnyType(supported));
        Assert.Equal(["pl6"], GetMacOsExtensions(supported));
    }

    [Fact]
    public async Task MainSaveFiles_LoadThroughTheAppLoadPath()
    {
        // The dialog filter was the only thing blocking .main files; lock down the rest
        // of the pipeline (SaveFileService -> FileUtil -> SaveUtil) so it stays that way.
        var dir = Fixtures.SaveFileFixture.FindSaveFilesPath();
        Assert.NotNull(dir);

        var mainSaves = Directory.GetFiles(dir, "*.main");
        Assert.NotEmpty(mainSaves);

        var service = new SaveFileService(new SaveBackupService(new FakeAppPaths()), new AppSettings());
        foreach (var path in mainSaves)
        {
            Assert.True(await service.LoadSaveFileAsync(path), $"Failed to load {Path.GetFileName(path)}");
        }
    }

    [Fact]
    public void AllFiles_MatchExtensionlessFilesOnLinuxAndAnyFileOnMacOs()
    {
        // "*.*" alone fails to match extensionless files ("main") on glob-based pickers;
        // "*" alone is dropped on macOS. Both forms must be present.
        var types = FileDialogFilterFactory.BuildOpenFileTypes(null);
        var allFiles = types[^1];

        Assert.Equal("All Files", allFiles.Name);
        Assert.Contains("*", allFiles.Patterns!);
        Assert.True(IsMacOsAnyType(allFiles));
    }
}
