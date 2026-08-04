using PKHeX.Core;
using PKHeX.Presentation.ViewModels;

namespace PKHeX.Avalonia.Tests;

public class DonutEditorSupportTests
{
    private static string? FindSaveFilesPath()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir != null)
        {
            var savePath = Path.Combine(dir.FullName, "savefiles");
            if (Directory.Exists(savePath))
                return savePath;
            dir = dir.Parent;
        }
        return null;
    }

    private static SAV9ZA? LoadZASave()
    {
        var dir = FindSaveFilesPath();
        if (dir is null) return null;
        var path = Path.Combine(dir, "gen9a_legendsza.main");
        if (!File.Exists(path)) return null;
        return SaveUtil.GetSaveFile(File.ReadAllBytes(path)) as SAV9ZA;
    }

    [Fact]
    public void DonutEditor_DoesNotCrash_WhenSaveHasNoDonutBlock()
    {
        // This save predates the in-game donut unlock: the KDonuts block is absent, so the
        // accessor substitutes an empty dummy block and any GetDonut() slice throws.
        var sav = LoadZASave();
        if (sav is null)
            return; // gen9a_legendsza.main not present — run Tests/savefiles/download_saves.sh

        Assert.True(sav.Donuts.Data.Length < DonutPocket9a.MaxCount * Donut9a.Size,
            "Test save unexpectedly has a full donut block; this regression test needs a save without one.");

        var vm = new DonutEditorViewModel(sav); // used to throw ArgumentOutOfRangeException

        Assert.False(vm.IsSupported);
        Assert.Empty(vm.Donuts);
    }

    [Fact]
    public void DonutEditor_IsSupported_WithFullSizeBlock()
    {
        // A blank SAV9ZA is constructed with all blocks present at full size.
        var sav = new SAV9ZA();
        if (sav.Donuts.Data.Length < DonutPocket9a.MaxCount * Donut9a.Size)
            return; // supported path not constructible in-memory on this Core revision

        var vm = new DonutEditorViewModel(sav);

        Assert.True(vm.IsSupported);
        Assert.Equal(DonutPocket9a.MaxCount, vm.Donuts.Count);
    }
}
