using PKHeX.Avalonia.Tests.Fixtures;
using PKHeX.Presentation.ViewModels;
using PKHeX.Core;
using Xunit.Abstractions;

namespace PKHeX.Avalonia.Tests;

/// <summary>
/// Regression tests for GitHub issues #74 and #76, run against the real save
/// files in Tests/savefiles/ (one per generation).
///
/// #74: Boxes dumped from Gold could not be imported into Crystal
///      ("0 files imported, 65 skipped") because the UI wrote raw internal
///      slot data instead of the standard .pk* file format, and the import
///      path performed no game/generation conversion.
///
/// #76: Money and Pokémon stat edits "reset back to default" after saving.
/// </summary>
public class IssueRegressionTests(ITestOutputHelper output)
{
    public static IEnumerable<object[]> AllSaves() => SaveFileFixture.AllRealSavesSimple();

    // =========================================================================
    // Issue #74: Dump boxes -> import into a different game of the same gen
    // =========================================================================

    [Fact]
    public void Issue74_Gold_DumpBoxes_ImportsInto_Crystal()
    {
        var saveDir = SaveFileFixture.FindSaveFilesPath();
        Assert.NotNull(saveDir);

        var gold = SaveFileFixture.LoadSave(Path.Combine(saveDir, "gen2_gold.sav"));
        var crystal = SaveFileFixture.LoadSave(Path.Combine(saveDir, "gen2_crystal.sav"));
        Assert.NotNull(gold);
        Assert.NotNull(crystal);

        var dumpDir = Directory.CreateTempSubdirectory("pkhex-dump-").FullName;
        try
        {
            int dumped = gold.DumpBoxes(dumpDir);
            output.WriteLine($"Dumped {dumped} Pokémon from Gold");
            Assert.True(dumped > 0, "Gold save must dump at least one Pokémon");

            // Every dumped file must be re-readable as a PKM
            var files = Directory.GetFiles(dumpDir);
            Assert.Equal(dumped, files.Length);

            // The reporter's Crystal save had empty boxes; RoC's is full, so clear first.
            crystal.ClearBoxes();
            int imported = crystal.LoadBoxes(dumpDir, out var result, all: true);
            output.WriteLine($"Crystal import result: {result}");
            Assert.True(imported > 0, $"Import into Crystal must succeed, got: {result}");
            Assert.Equal(dumped, imported);
        }
        finally
        {
            Directory.Delete(dumpDir, recursive: true);
        }
    }

    [Theory, MemberData(nameof(AllSaves))]
    public void Issue74_DumpBoxes_RoundTrips_Into_Same_Save(SaveFile sav, string label)
    {
        if (!sav.HasBox) { output.WriteLine($"{label}: no boxes, skip"); return; }

        int occupied = SaveFileFixture.CountOccupiedSlots(sav);
        if (occupied == 0) { output.WriteLine($"{label}: no occupied slots, skip"); return; }

        var dumpDir = Directory.CreateTempSubdirectory("pkhex-dump-").FullName;
        try
        {
            int dumped = sav.DumpBoxes(dumpDir);
            output.WriteLine($"{label}: dumped {dumped}/{occupied}");
            Assert.True(dumped > 0, $"{label}: dump must produce files");

            // Import into a copy of the same save with cleared boxes.
            var copy = sav.Clone();
            copy.ClearBoxes();
            int imported = copy.LoadBoxes(dumpDir, out var result, all: true);
            output.WriteLine($"{label}: {result}");
            Assert.True(imported > 0, $"{label}: import must succeed, got: {result}");
        }
        finally
        {
            Directory.Delete(dumpDir, recursive: true);
        }
    }

    // =========================================================================
    // Issue #76: Money edits must survive a write/reload round-trip
    // =========================================================================

    [Theory, MemberData(nameof(AllSaves))]
    public void Issue76_MoneyEdit_Survives_WriteReload(SaveFile sav, string label)
    {
        uint target = (uint)Math.Min(12345, sav.MaxMoney);

        var vm = new TrainerEditorViewModel(sav)
        {
            Money = target,
        };
        vm.SaveCommand.Execute(null);
        Assert.Equal(target, sav.Money);

        // Full file round-trip (what File > Save does)
        var data = sav.Write().ToArray();
        var reloaded = SaveUtil.GetSaveFile(data);
        Assert.NotNull(reloaded);
        output.WriteLine($"{label}: money {target} -> reloaded {reloaded.Money}");
        Assert.Equal(target, reloaded.Money);
    }

    [Theory, MemberData(nameof(AllSaves))]
    public void Issue76_PartyStatEdit_Survives_WriteReload(SaveFile sav, string label)
    {
        if (sav.PartyCount == 0) { output.WriteLine($"{label}: empty party, skip"); return; }

        var pk = sav.GetPartySlotAtIndex(0);
        if (pk.Species == 0) { output.WriteLine($"{label}: empty slot, skip"); return; }

        // Simulate a stat edit in the Pokémon editor: change an EV and level.
        var (vm, _, _) = TestHelpers.CreateTestViewModel(pk, sav);
        vm.EvHP = vm.EvHP == 4 ? 8 : 4;
        var edited = vm.PreparePKM();
        var expectedEv = edited.EV_HP;

        sav.SetPartySlotAtIndex(edited, 0);

        var data = sav.Write().ToArray();
        var reloaded = SaveUtil.GetSaveFile(data);
        Assert.NotNull(reloaded);
        var reread = reloaded.GetPartySlotAtIndex(0);
        output.WriteLine($"{label}: EV_HP {pk.EV_HP} -> {expectedEv}, reloaded {reread.EV_HP}");
        Assert.Equal(expectedEv, reread.EV_HP);
        Assert.Equal(edited.Species, reread.Species);
    }

    [Theory, MemberData(nameof(AllSaves))]
    public void Issue76_BoxStatEdit_Survives_WriteReload(SaveFile sav, string label)
    {
        var slot = SaveFileFixture.GetFirstOccupiedSlot(sav);
        if (slot == null) { output.WriteLine($"{label}: no occupied box slots, skip"); return; }

        var (pk, idx) = slot.Value;
        var (vm, _, _) = TestHelpers.CreateTestViewModel(pk, sav);
        vm.EvATK = vm.EvATK == 4 ? 8 : 4;
        var edited = vm.PreparePKM();
        var expectedEv = edited.EV_ATK;

        sav.SetBoxSlotAtIndex(edited, idx);

        var data = sav.Write().ToArray();
        var reloaded = SaveUtil.GetSaveFile(data);
        Assert.NotNull(reloaded);
        var reread = reloaded.GetBoxSlotAtIndex(idx);
        output.WriteLine($"{label}: slot {idx} EV_ATK -> {expectedEv}, reloaded {reread.EV_ATK}");
        Assert.Equal(expectedEv, reread.EV_ATK);
    }
}
