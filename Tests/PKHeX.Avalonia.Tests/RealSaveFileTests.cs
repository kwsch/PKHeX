using Moq;
using PKHeX.Avalonia.Services;
using PKHeX.Presentation.ViewModels;
using PKHeX.Core;
using Xunit.Abstractions;

namespace PKHeX.Avalonia.Tests;

/// <summary>
/// Tests using real save files stored in Tests/savefiles/.
/// Covers BoxViewer, PartyViewer, PokemonEditor, and write round-trips against real data.
/// Currently covers: Gen3 (Emerald), Gen7 (Sun/Moon).
/// </summary>
public class RealSaveFileTests(ITestOutputHelper output)
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

    private static SaveFile? LoadSave(string path)
    {
        if (!File.Exists(path)) return null;
        return SaveUtil.GetSaveFile(File.ReadAllBytes(path));
    }

    private SaveFile? GetEmerald(out string? skipReason)
    {
        var savPath = FindSaveFilesPath();
        if (savPath == null) { skipReason = "savefiles directory not found"; return null; }

        var sav = LoadSave(Path.Combine(savPath, "gen3_emerald.sav"));
        if (sav == null) { skipReason = "gen3_emerald.sav missing"; return null; }

        skipReason = null;
        return sav;
    }

    // -----------------------------------------------------------------------
    // 1. Basic load and checksum validation
    // -----------------------------------------------------------------------
    [Fact]
    public void Emerald_Loads_With_Valid_Checksum()
    {
        var sav = GetEmerald(out var skip);
        if (sav == null) { output.WriteLine($"Skip: {skip}"); return; }

        output.WriteLine($"Loaded {sav.GetType().Name}, Version={sav.Version}, OT={sav.OT}");
        Assert.True(sav.ChecksumsValid, "Emerald save checksum must be valid");
        Assert.Equal(3, sav.Generation);
    }

    // -----------------------------------------------------------------------
    // 2. BoxViewerViewModel construction with real data
    // -----------------------------------------------------------------------
    [Fact]
    public void Emerald_BoxViewer_Constructs_Without_Exception()
    {
        var sav = GetEmerald(out var skip);
        if (sav == null) { output.WriteLine($"Skip: {skip}"); return; }

        var spriteMock = new Mock<ISpriteRenderer>();
        var ex = Record.Exception(() => new BoxViewerViewModel(sav, spriteMock.Object));
        Assert.Null(ex);
    }

    // -----------------------------------------------------------------------
    // 3. All box slots are iterable
    // -----------------------------------------------------------------------
    [Fact]
    public void Emerald_BoxSlots_AreIterable()
    {
        var sav = GetEmerald(out var skip);
        if (sav == null) { output.WriteLine($"Skip: {skip}"); return; }

        int occupied = 0;
        var ex = Record.Exception(() =>
        {
            for (int box = 0; box < sav.BoxCount; box++)
            {
                foreach (var pk in sav.GetBoxData(box))
                    if (pk.Species != 0) occupied++;
            }
        });

        Assert.Null(ex);
        output.WriteLine($"Emerald: {sav.BoxCount} boxes, {occupied} occupied slots");
    }

    // -----------------------------------------------------------------------
    // 4. PartyViewerViewModel construction with real data
    // -----------------------------------------------------------------------
    [Fact]
    public void Emerald_PartyViewer_Constructs_Without_Exception()
    {
        var sav = GetEmerald(out var skip);
        if (sav == null) { output.WriteLine($"Skip: {skip}"); return; }

        var spriteMock = new Mock<ISpriteRenderer>();
        var ex = Record.Exception(() =>
        {
            var vm = new PartyViewerViewModel(sav, spriteMock.Object);
            output.WriteLine($"Party slots: {vm.Slots.Count(s => !s.IsEmpty)} occupied");
        });
        Assert.Null(ex);
    }

    // -----------------------------------------------------------------------
    // 5. Every occupied box slot loads into PokemonEditorViewModel
    // -----------------------------------------------------------------------
    [Fact]
    public void Emerald_AllOccupiedSlots_LoadIntoViewModel_WithoutException()
    {
        var sav = GetEmerald(out var skip);
        if (sav == null) { output.WriteLine($"Skip: {skip}"); return; }

        var errors = new List<string>();
        int count = 0;

        for (int i = 0; i < sav.BoxCount * sav.BoxSlotCount; i++)
        {
            var pk = sav.GetBoxSlotAtIndex(i);
            if (pk.Species == 0) continue;

            try
            {
                var (vm, _, _) = TestHelpers.CreateTestViewModel(pk, sav);
                _ = vm.Species;
                _ = vm.Level;
                _ = vm.IsLegal;
                count++;
            }
            catch (Exception ex)
            {
                errors.Add($"Slot {i} (species={pk.Species}): {ex.Message}");
            }
        }

        output.WriteLine($"Loaded {count} Pokemon from Emerald save without exception.");
        Assert.Empty(errors);
    }

    // -----------------------------------------------------------------------
    // 6. Nickname edit + write-back round-trip on first occupied slot
    // -----------------------------------------------------------------------
    [Fact]
    public void Emerald_NicknameEdit_WritesBackToSave()
    {
        var sav = GetEmerald(out var skip);
        if (sav == null) { output.WriteLine($"Skip: {skip}"); return; }

        PKM? pkm = null;
        int slotIndex = -1;
        for (int i = 0; i < sav.BoxCount * sav.BoxSlotCount; i++)
        {
            var candidate = sav.GetBoxSlotAtIndex(i);
            if (candidate.Species != 0) { pkm = candidate; slotIndex = i; break; }
        }

        if (pkm == null) { output.WriteLine("No Pokemon in boxes, skipping edit test."); return; }

        var original = pkm.Nickname;
        output.WriteLine($"Editing slot {slotIndex}: species={pkm.Species}, nickname={original}");

        var (vm, _, _) = TestHelpers.CreateTestViewModel(pkm, sav);
        vm.Nickname = "EDITED";
        var result = vm.PreparePKM();

        Assert.Equal("EDITED", result.Nickname);
        sav.SetBoxSlotAtIndex(result, slotIndex);
        Assert.Equal("EDITED", sav.GetBoxSlotAtIndex(slotIndex).Nickname);
        output.WriteLine($"Nickname round-trip: {original} → EDITED ✓");
    }

    // -----------------------------------------------------------------------
    // 7. Write round-trip produces a reloadable save with valid checksums
    // -----------------------------------------------------------------------
    [Fact]
    public void Emerald_WriteRoundTrip_ProducesValidSave()
    {
        var sav = GetEmerald(out var skip);
        if (sav == null) { output.WriteLine($"Skip: {skip}"); return; }

        var written = sav.Write();
        Assert.True(written.Length > 0);

        var sav2 = SaveUtil.GetSaveFile(written);
        Assert.NotNull(sav2);
        Assert.True(sav2.ChecksumsValid, "Reloaded save must have valid checksums");
        output.WriteLine($"Write round-trip: {written.Length} bytes, valid checksums ✓");
    }

    // -----------------------------------------------------------------------
    // 8. InventoryEditorViewModel works with real items
    // -----------------------------------------------------------------------
    [Fact]
    public void Emerald_InventoryEditor_Constructs_Without_Exception()
    {
        var sav = GetEmerald(out var skip);
        if (sav == null) { output.WriteLine($"Skip: {skip}"); return; }

        var ex = Record.Exception(() =>
        {
            var vm = new InventoryEditorViewModel(sav, new Moq.Mock<ISpriteRenderer>().Object);
            output.WriteLine($"Inventory pouches: {vm.Pouches.Count}");
        });
        Assert.Null(ex);
    }

    // -----------------------------------------------------------------------
    // 9. TrainerEditorViewModel works with real trainer data
    // -----------------------------------------------------------------------
    [Fact]
    public void Emerald_TrainerEditor_Loads_Correct_OT()
    {
        var sav = GetEmerald(out var skip);
        if (sav == null) { output.WriteLine($"Skip: {skip}"); return; }

        var vm = new TrainerEditorViewModel(sav);
        Assert.NotNull(vm.TrainerName);
        Assert.Equal(sav.OT, vm.TrainerName);
        output.WriteLine($"Emerald OT: {vm.TrainerName}");
    }

    // =======================================================================
    // Gen7 Sun/Moon: Tests/savefiles/gen7_sun.main
    // =======================================================================

    private SaveFile? GetSun(out string? skipReason)
    {
        var savPath = FindSaveFilesPath();
        if (savPath == null) { skipReason = "savefiles directory not found"; return null; }

        var sav = LoadSave(Path.Combine(savPath, "gen7_sun.main"));
        if (sav == null) { skipReason = "gen7_sun.main missing or unrecognised format"; return null; }

        skipReason = null;
        return sav;
    }

    [Fact]
    public void Sun_Loads_As_Gen7_Save()
    {
        var sav = GetSun(out var skip);
        if (sav == null) { output.WriteLine($"Skip: {skip}"); return; }

        output.WriteLine($"Loaded {sav.GetType().Name}, Version={sav.Version}, OT={sav.OT}");
        Assert.Equal(7, sav.Generation);
        Assert.True(sav.ChecksumsValid, "Sun save checksum must be valid");
    }

    [Fact]
    public void Sun_BoxViewer_Constructs_Without_Exception()
    {
        var sav = GetSun(out var skip);
        if (sav == null) { output.WriteLine($"Skip: {skip}"); return; }

        var spriteMock = new Mock<ISpriteRenderer>();
        var ex = Record.Exception(() => new BoxViewerViewModel(sav, spriteMock.Object));
        Assert.Null(ex);
    }

    [Fact]
    public void Sun_BoxSlots_AreIterable()
    {
        var sav = GetSun(out var skip);
        if (sav == null) { output.WriteLine($"Skip: {skip}"); return; }

        int occupied = 0;
        var ex = Record.Exception(() =>
        {
            for (int box = 0; box < sav.BoxCount; box++)
            {
                foreach (var pk in sav.GetBoxData(box))
                    if (pk.Species != 0) occupied++;
            }
        });

        Assert.Null(ex);
        output.WriteLine($"Sun: {sav.BoxCount} boxes, {occupied} occupied slots");
    }

    [Fact]
    public void Sun_PartyViewer_Constructs_Without_Exception()
    {
        var sav = GetSun(out var skip);
        if (sav == null) { output.WriteLine($"Skip: {skip}"); return; }

        var spriteMock = new Mock<ISpriteRenderer>();
        var ex = Record.Exception(() =>
        {
            var vm = new PartyViewerViewModel(sav, spriteMock.Object);
            output.WriteLine($"Sun party: {vm.Slots.Count(s => !s.IsEmpty)} occupied");
        });
        Assert.Null(ex);
    }

    [Fact]
    public void Sun_AllOccupiedSlots_LoadIntoViewModel_WithoutException()
    {
        var sav = GetSun(out var skip);
        if (sav == null) { output.WriteLine($"Skip: {skip}"); return; }

        var errors = new List<string>();
        int count = 0;

        for (int i = 0; i < sav.BoxCount * sav.BoxSlotCount; i++)
        {
            var pk = sav.GetBoxSlotAtIndex(i);
            if (pk.Species == 0) continue;

            try
            {
                var (vm, _, _) = TestHelpers.CreateTestViewModel(pk, sav);
                _ = vm.Species;
                _ = vm.Level;
                _ = vm.IsLegal;
                count++;
            }
            catch (Exception ex)
            {
                errors.Add($"Slot {i} (species={pk.Species}): {ex.Message}");
            }
        }

        output.WriteLine($"Sun: loaded {count} Pokemon from boxes without exception.");
        Assert.Empty(errors);
    }

    [Fact]
    public void Sun_WriteRoundTrip_ProducesValidSave()
    {
        var sav = GetSun(out var skip);
        if (sav == null) { output.WriteLine($"Skip: {skip}"); return; }

        var written = sav.Write();
        Assert.True(written.Length > 0);

        var sav2 = SaveUtil.GetSaveFile(written);
        Assert.NotNull(sav2);
        Assert.True(sav2.ChecksumsValid, "Reloaded Sun save must have valid checksums");
        output.WriteLine($"Sun write round-trip: {written.Length} bytes, valid checksums ✓");
    }

    [Fact]
    public void Sun_TrainerEditor_Loads_OT()
    {
        var sav = GetSun(out var skip);
        if (sav == null) { output.WriteLine($"Skip: {skip}"); return; }

        var vm = new TrainerEditorViewModel(sav);
        Assert.NotNull(vm.TrainerName);
        Assert.Equal(sav.OT, vm.TrainerName);
        output.WriteLine($"Sun OT: {vm.TrainerName}");
    }

    [Fact]
    public void Sun_InventoryEditor_Constructs_Without_Exception()
    {
        var sav = GetSun(out var skip);
        if (sav == null) { output.WriteLine($"Skip: {skip}"); return; }

        var ex = Record.Exception(() =>
        {
            var vm = new InventoryEditorViewModel(sav, new Moq.Mock<ISpriteRenderer>().Object);
            output.WriteLine($"Sun inventory pouches: {vm.Pouches.Count}");
        });
        Assert.Null(ex);
    }
}
