using PKHeX.Presentation.ViewModels;
using PKHeX.Core;
using Xunit.Abstractions;

namespace PKHeX.Avalonia.Tests;

/// <summary>
/// Behavioral tests for ReportViewModel.
/// Generates a legality summary for all non-empty PKM in save boxes.
/// </summary>
public class ReportViewModelTests(ITestOutputHelper output)
{
    // -----------------------------------------------------------------------
    // 1. Empty save produces empty Items list
    // -----------------------------------------------------------------------

    [Fact]
    public void Report_BlankSave_EmptyItems()
    {
        var sav = new SAV6XY();
        var vm = new ReportViewModel(sav);

        Assert.Empty(vm.Items);
        output.WriteLine("Gen6 blank save: 0 report items ✓");
    }

    // -----------------------------------------------------------------------
    // 2. Items count matches number of non-empty PKM in boxes
    // -----------------------------------------------------------------------

    [Fact]
    public void Report_ItemCount_MatchesNonEmptyBoxSlots()
    {
        var sav = new SAV6XY();

        // Inject 3 Pokémon
        sav.SetBoxSlotAtIndex(new PK6 { Species = 1 }, 0);
        sav.SetBoxSlotAtIndex(new PK6 { Species = 4 }, 1);
        sav.SetBoxSlotAtIndex(new PK6 { Species = 7 }, 2);

        var vm = new ReportViewModel(sav);

        Assert.Equal(3, vm.Items.Count);
        output.WriteLine($"Gen6: 3 PKM → 3 report items ✓");
    }

    // -----------------------------------------------------------------------
    // 3. ReportEntryViewModel properties populated correctly
    // -----------------------------------------------------------------------

    [Fact]
    public void Report_EntryProperties_PopulatedFromPKM()
    {
        var sav = new SAV6XY();

        var pk = new PK6 { Species = 25 }; // Pikachu
        pk.CurrentLevel = 55;
        pk.Nickname = "Sparky";
        pk.IV_HP  = 31;
        pk.IV_ATK = 30;
        pk.EV_HP  = 252;

        sav.SetBoxSlotAtIndex(pk, 0);

        var vm = new ReportViewModel(sav);
        Assert.Single(vm.Items);

        var entry = vm.Items[0];
        Assert.Equal(55,  entry.Level);
        Assert.Equal("Sparky", entry.Nickname);
        Assert.Equal(31,  entry.IV_HP);
        Assert.Equal(30,  entry.IV_Atk);
        Assert.Equal(252, entry.EV_HP);
        output.WriteLine($"Report entry: Species={entry.Species}, Level={entry.Level}, Nickname={entry.Nickname} ✓");
    }

    // -----------------------------------------------------------------------
    // 4. Slots with Species=0 are excluded
    // -----------------------------------------------------------------------

    [Fact]
    public void Report_EmptySlots_Excluded()
    {
        var sav = new SAV6XY();

        // Slot 0: real PKM; slot 1: blank (Species=0)
        sav.SetBoxSlotAtIndex(new PK6 { Species = 1 }, 0);
        sav.SetBoxSlotAtIndex(new PK6 { Species = 0 }, 1);

        var vm = new ReportViewModel(sav);

        Assert.Single(vm.Items);
        output.WriteLine("Report: blank slot excluded ✓");
    }

    // -----------------------------------------------------------------------
    // 5. Multiple generations produce valid reports
    // -----------------------------------------------------------------------

    [Theory]
    [InlineData(GameVersion.E,  "Gen3-Emerald")]
    [InlineData(GameVersion.Pt, "Gen4-Platinum")]
    [InlineData(GameVersion.SN, "Gen7-Sun")]
    public void Report_BlankSave_VariousGens_NoThrow(GameVersion version, string label)
    {
        var sav = BlankSaveFile.Get(version);
        var ex = Record.Exception(() =>
        {
            var vm = new ReportViewModel(sav);
            Assert.Empty(vm.Items);
        });
        Assert.Null(ex);
        output.WriteLine($"{label}: ReportViewModel OK on blank save ✓");
    }

    // -----------------------------------------------------------------------
    // 6. Nature and Ability strings are not empty for valid PKM
    // -----------------------------------------------------------------------

    [Fact]
    public void Report_Nature_And_Ability_NotEmpty()
    {
        var sav = new SAV6XY();
        var pk = new PK6 { Species = 6, Ability = 46 }; // Charizard with Blaze
        pk.Nature = (Nature)3; // Adamant
        sav.SetBoxSlotAtIndex(pk, 0);

        var vm = new ReportViewModel(sav);
        Assert.Single(vm.Items);

        var entry = vm.Items[0];
        Assert.False(string.IsNullOrEmpty(entry.Nature));
        Assert.False(string.IsNullOrEmpty(entry.Ability));
        output.WriteLine($"Report entry: Nature='{entry.Nature}', Ability='{entry.Ability}' ✓");
    }
}
