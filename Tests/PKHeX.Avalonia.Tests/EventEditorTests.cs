using PKHeX.Presentation.ViewModels;
using PKHeX.Core;
using Xunit.Abstractions;

namespace PKHeX.Avalonia.Tests;

/// <summary>
/// Behavioral tests for EventFlagsEditorViewModel and EventWorkEditorViewModel.
/// Event flags exist in most main-series saves (Gen3+); event work is Gen7b-specific.
/// </summary>
public class EventEditorTests(ITestOutputHelper output)
{
    // -----------------------------------------------------------------------
    // EventFlagsEditorViewModel
    // -----------------------------------------------------------------------

    // Gen1/2 implement IEventFlagArray directly; Gen3-7 implement IEventFlagProvider37.
    // Gen7b/8+ use SCBlock-based event data not covered by these interfaces.
    [Theory]
    [InlineData(GameVersion.RD, "Gen1-Red")]
    [InlineData(GameVersion.GD, "Gen2-Gold")]
    [InlineData(GameVersion.E,  "Gen3-Emerald")]
    [InlineData(GameVersion.Pt, "Gen4-Platinum")]
    [InlineData(GameVersion.W2, "Gen5-White2")]
    [InlineData(GameVersion.X,  "Gen6-X")]
    [InlineData(GameVersion.SN, "Gen7-Sun")]
    public void EventFlags_IsSupported_True(GameVersion version, string label)
    {
        var sav = BlankSaveFile.Get(version);
        var vm = new EventFlagsEditorViewModel(sav);

        Assert.True(vm.IsSupported, $"{label}: expected IsSupported=true");
        Assert.True(vm.FlagCount > 0, $"{label}: expected FlagCount>0");
        Assert.Equal(vm.FlagCount, vm.Flags.Count);
        output.WriteLine($"{label}: IsSupported=true, FlagCount={vm.FlagCount} ✓");
    }

    [Theory]
    [InlineData(GameVersion.GP, "Gen7b-LGPE")]
    [InlineData(GameVersion.SW, "Gen8-Sword")]
    [InlineData(GameVersion.SL, "Gen9-Scarlet")]
    public void EventFlags_IsSupported_False(GameVersion version, string label)
    {
        var sav = BlankSaveFile.Get(version);
        var vm = new EventFlagsEditorViewModel(sav);

        Assert.False(vm.IsSupported, $"{label}: expected IsSupported=false");
        Assert.Equal(0, vm.FlagCount);
        output.WriteLine($"{label}: IsSupported=false ✓");
    }

    [Fact]
    public void EventFlags_Gen6_FlagRoundTrips_ThroughSave()
    {
        var sav = new SAV6XY();
        var vm = new EventFlagsEditorViewModel(sav);

        Assert.True(vm.IsSupported);
        Assert.True(vm.FlagCount > 0);

        // Toggle flag 0
        var initial = vm.Flags[0].IsSet;
        vm.Flags[0].IsSet = !initial;
        vm.SaveCommand.Execute(null);

        // Verify via fresh VM
        var vm2 = new EventFlagsEditorViewModel(sav);
        Assert.Equal(!initial, vm2.Flags[0].IsSet);
        output.WriteLine($"Gen6 flag[0]: {initial} → {!initial} round-tripped ✓");
    }

    [Fact]
    public void EventFlags_SetAll_SetsAllFlags()
    {
        var sav = new SAV6XY();
        var vm = new EventFlagsEditorViewModel(sav);

        Assert.True(vm.IsSupported);
        vm.SetAllCommand.Execute(null);

        Assert.All(vm.Flags, f => Assert.True(f.IsSet, $"Flag {f.Index} should be set"));
        output.WriteLine($"SetAll: {vm.Flags.Count} flags all set ✓");
    }

    [Fact]
    public void EventFlags_ClearAll_ClearsAllFlags()
    {
        var sav = new SAV6XY();
        var vm = new EventFlagsEditorViewModel(sav);

        Assert.True(vm.IsSupported);
        vm.SetAllCommand.Execute(null);
        vm.ClearAllCommand.Execute(null);

        Assert.All(vm.Flags, f => Assert.False(f.IsSet, $"Flag {f.Index} should be cleared"));
        output.WriteLine($"ClearAll: {vm.Flags.Count} flags all cleared ✓");
    }

    [Fact]
    public void EventFlags_SearchFilter_NarrowsResults()
    {
        var sav = new SAV6XY();
        var vm = new EventFlagsEditorViewModel(sav);

        Assert.True(vm.IsSupported);
        var total = vm.FilteredFlags.Count;
        Assert.True(total > 0);

        // Filter by hex index "0x0000"
        vm.SearchText = "0x0000";
        Assert.True(vm.FilteredFlags.Count < total, "Search should narrow results");
        Assert.True(vm.FilteredFlags.Count > 0, "At least flag 0 should match '0x0000'");

        // Non-matching search
        vm.SearchText = "ZZZNOMATCH";
        Assert.Empty(vm.FilteredFlags);

        // Clear
        vm.SearchText = string.Empty;
        Assert.Equal(total, vm.FilteredFlags.Count);
        output.WriteLine($"SearchFilter: total={total}, '0x0000' filtered correctly ✓");
    }

    [Fact]
    public void EventFlags_ResetCommand_ReloadsFromSave()
    {
        var sav = new SAV6XY();
        var vm = new EventFlagsEditorViewModel(sav);

        Assert.True(vm.IsSupported);
        var original = vm.Flags[0].IsSet;

        // Modify without saving
        vm.Flags[0].IsSet = !original;
        Assert.Equal(!original, vm.Flags[0].IsSet);

        // Reset reloads from save (original value)
        vm.ResetCommand.Execute(null);
        Assert.Equal(original, vm.Flags[0].IsSet);
        output.WriteLine($"ResetCommand: flag[0] restored to {original} ✓");
    }

    [Fact]
    public void EventFlags_SelectedFlagIndex_UpdatesSelectedFlagValue()
    {
        var sav = new SAV6XY();
        var vm = new EventFlagsEditorViewModel(sav);

        Assert.True(vm.IsSupported);

        // Set flag 5 manually
        vm.Flags[5].IsSet = true;
        vm.SaveCommand.Execute(null);

        // Load fresh and select index 5
        var vm2 = new EventFlagsEditorViewModel(sav);
        vm2.SelectedFlagIndex = 5;
        Assert.True(vm2.SelectedFlagValue, "SelectedFlagValue should reflect flag 5 = true");
        output.WriteLine("SelectedFlagIndex → SelectedFlagValue reflects saved state ✓");
    }

    [Fact]
    public void EventFlags_HexIndex_FormattedCorrectly()
    {
        var sav = new SAV6XY();
        var vm = new EventFlagsEditorViewModel(sav);

        Assert.True(vm.IsSupported);
        // Flag at index 0 should have HexIndex "0x0000"
        Assert.Equal("0x0000", vm.Flags[0].HexIndex);
        // Flag at index 255 should have HexIndex "0x00FF"
        if (vm.Flags.Count > 255)
            Assert.Equal("0x00FF", vm.Flags[255].HexIndex);
        output.WriteLine("HexIndex formatting correct ✓");
    }

    // -----------------------------------------------------------------------
    // EventWorkEditorViewModel (Gen7b only)
    // -----------------------------------------------------------------------

    [Fact]
    public void EventWork_Gen7b_IsSupported_True()
    {
        var sav = BlankSaveFile.Get(GameVersion.GP);
        var vm = new EventWorkEditorViewModel(sav);

        Assert.True(vm.IsSupported);
        Assert.True(vm.WorkCount > 0);
        Assert.True(vm.FlagCount > 0);
        output.WriteLine($"Gen7b: IsSupported=true, WorkCount={vm.WorkCount}, FlagCount={vm.FlagCount} ✓");
    }

    [Theory]
    [InlineData(GameVersion.X,  "Gen6-X")]
    [InlineData(GameVersion.SN, "Gen7-Sun")]
    [InlineData(GameVersion.SL, "Gen9-Scarlet")]
    public void EventWork_NonGen7b_IsSupported_False(GameVersion version, string label)
    {
        var sav = BlankSaveFile.Get(version);
        var vm = new EventWorkEditorViewModel(sav);

        Assert.False(vm.IsSupported, $"{label}: expected IsSupported=false");
        output.WriteLine($"{label}: EventWork IsSupported=false ✓");
    }

    [Fact]
    public void EventWork_Gen7b_SelectWork_ReadsValue()
    {
        var sav = BlankSaveFile.Get(GameVersion.GP);
        var vm = new EventWorkEditorViewModel(sav);

        Assert.True(vm.IsSupported && vm.WorkCount > 0);

        vm.SelectedWorkIndex = 0;
        // Should not throw and should provide a value
        _ = vm.SelectedWorkValue;
        output.WriteLine($"Gen7b: SelectedWork[0]={vm.SelectedWorkValue} ✓");
    }

    [Fact]
    public void EventWork_Gen7b_ApplyWork_WritesValue()
    {
        var sav = BlankSaveFile.Get(GameVersion.GP);
        var vm = new EventWorkEditorViewModel(sav);

        Assert.True(vm.IsSupported && vm.WorkCount > 0);

        vm.SelectedWorkIndex = 0;
        vm.SelectedWorkValue = 42;
        vm.ApplyWorkCommand.Execute(null);

        // Read back
        vm.SelectedWorkIndex = 0;
        Assert.Equal(42, vm.SelectedWorkValue);
        output.WriteLine("Gen7b: ApplyWork[0]=42 written and read back ✓");
    }
}
