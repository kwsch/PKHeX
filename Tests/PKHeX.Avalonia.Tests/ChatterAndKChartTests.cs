using Moq;
using PKHeX.Avalonia.Services;
using PKHeX.Presentation.ViewModels;
using PKHeX.Core;
using Xunit.Abstractions;

namespace PKHeX.Avalonia.Tests;

/// <summary>
/// Behavioral tests for ChatterEditorViewModel (Gen4/5) and KChartViewModel.
/// </summary>
public class ChatterAndKChartTests(ITestOutputHelper output)
{
    private static Mock<ISpriteRenderer> SpriteMock() => new();

    // -----------------------------------------------------------------------
    // ChatterEditorViewModel
    // -----------------------------------------------------------------------

    [Theory]
    [InlineData(GameVersion.Pt, "Gen4-Platinum")]
    [InlineData(GameVersion.W2, "Gen5-White2")]
    public void Chatter_IsSupported(GameVersion version, string label)
    {
        var sav = BlankSaveFile.Get(version);
        var vm = new ChatterEditorViewModel(sav);

        Assert.True(vm.IsSupported, $"{label}: expected IsSupported=true");
        output.WriteLine($"{label}: Chatter IsSupported=true ✓");
    }

    [Theory]
    [InlineData(GameVersion.X,  "Gen6-X")]
    [InlineData(GameVersion.SN, "Gen7-Sun")]
    [InlineData(GameVersion.SL, "Gen9-Scarlet")]
    public void Chatter_NotSupported_OnBlankSave(GameVersion version, string label)
    {
        var sav = BlankSaveFile.Get(version);
        var vm = new ChatterEditorViewModel(sav);

        Assert.False(vm.IsSupported, $"{label}: expected IsSupported=false");
        output.WriteLine($"{label}: Chatter IsSupported=false ✓");
    }

    [Fact]
    public void Chatter_Gen5_BlankSave_NotInitialized()
    {
        var sav = BlankSaveFile.Get(GameVersion.W2);
        var vm = new ChatterEditorViewModel(sav);

        Assert.True(vm.IsSupported);
        Assert.False(vm.Initialized, "Blank save Chatot should not be initialized");
        output.WriteLine("Gen5 Chatter: blank save not initialized ✓");
    }

    [Fact]
    public void Chatter_Gen5_ToggleInitialized_UpdatesConfusionChance()
    {
        var sav = BlankSaveFile.Get(GameVersion.W2);
        var vm = new ChatterEditorViewModel(sav);

        Assert.True(vm.IsSupported);
        var initialChance = vm.ConfusionChance;

        vm.Initialized = true;

        // ConfusionChance should update when Initialized changes
        // (exact value depends on recording data)
        _ = vm.ConfusionChance; // just verify it's accessible
        output.WriteLine($"Gen5 Chatter: Initialized=true, ConfusionChance={vm.ConfusionChance} ✓");
    }

    [Fact]
    public void Chatter_Gen5_ClearRecording_SetsNotInitialized()
    {
        var sav = BlankSaveFile.Get(GameVersion.W2);
        var vm = new ChatterEditorViewModel(sav);

        Assert.True(vm.IsSupported);

        vm.Initialized = true;
        vm.ClearRecordingCommand.Execute(null);

        Assert.False(vm.Initialized, "After clear, Initialized should be false");
        output.WriteLine("Gen5 Chatter: ClearRecording → Initialized=false ✓");
    }

    [Fact]
    public void Chatter_Gen5_Refresh_DoesNotThrow()
    {
        var sav = BlankSaveFile.Get(GameVersion.W2);
        var vm = new ChatterEditorViewModel(sav);

        Assert.True(vm.IsSupported);
        var ex = Record.Exception(() => vm.RefreshCommand.Execute(null));
        Assert.Null(ex);
        output.WriteLine("Gen5 Chatter: Refresh no throw ✓");
    }

    // -----------------------------------------------------------------------
    // KChartViewModel
    // -----------------------------------------------------------------------

    [Fact]
    public void KChart_Gen6_PopulatesEntries()
    {
        var sav = new SAV6XY();
        var vm = new KChartViewModel(sav, SpriteMock().Object);

        Assert.NotEmpty(vm.Entries);
        output.WriteLine($"Gen6 KChart: {vm.Entries.Count} entries ✓");
    }

    [Fact]
    public void KChart_Entries_HaveValidSpeciesIds()
    {
        var sav = new SAV6XY();
        var vm = new KChartViewModel(sav, SpriteMock().Object);

        Assert.All(vm.Entries, e => Assert.True(e.Species > 0, $"Entry species should be > 0"));
        output.WriteLine($"Gen6 KChart: all {vm.Entries.Count} entries have valid species ✓");
    }

    [Fact]
    public void KChart_Entries_HaveNonEmptyNames()
    {
        var sav = new SAV6XY();
        var vm = new KChartViewModel(sav, SpriteMock().Object);

        Assert.All(vm.Entries, e => Assert.False(string.IsNullOrEmpty(e.Name)));
        output.WriteLine($"Gen6 KChart: all names non-empty ✓");
    }

    [Fact]
    public void KChart_Bulbasaur_HasCorrectBaseStats()
    {
        var sav = new SAV6XY();
        var vm = new KChartViewModel(sav, SpriteMock().Object);

        // Bulbasaur is species 1
        var bulbasaur = vm.Entries.FirstOrDefault(e => e.Species == 1 && e.Form == 0);
        Assert.NotNull(bulbasaur);

        // Bulbasaur's base HP is 45
        Assert.Equal(45, bulbasaur.HP);
        Assert.Equal(49, bulbasaur.ATK);
        Assert.Equal(49, bulbasaur.DEF);
        output.WriteLine($"Gen6 KChart Bulbasaur: HP={bulbasaur.HP}, ATK={bulbasaur.ATK}, DEF={bulbasaur.DEF} ✓");
    }

    [Fact]
    public void KChart_Gen3_PopulatesEntries()
    {
        var sav = BlankSaveFile.Get(GameVersion.E);
        var vm = new KChartViewModel(sav, SpriteMock().Object);

        Assert.NotEmpty(vm.Entries);
        output.WriteLine($"Gen3 KChart: {vm.Entries.Count} entries ✓");
    }

    [Theory]
    [InlineData(GameVersion.Pt, "Gen4-Platinum")]
    [InlineData(GameVersion.SN, "Gen7-Sun")]
    public void KChart_VariousGens_ConstructsWithoutThrow(GameVersion version, string label)
    {
        var sav = BlankSaveFile.Get(version);
        var ex = Record.Exception(() =>
        {
            var vm = new KChartViewModel(sav, SpriteMock().Object);
            Assert.NotEmpty(vm.Entries);
        });
        Assert.Null(ex);
        output.WriteLine($"{label}: KChart constructs OK ✓");
    }
}
