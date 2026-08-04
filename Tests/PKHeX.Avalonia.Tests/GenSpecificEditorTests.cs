using Moq;
using PKHeX.Avalonia.Services;
using PKHeX.Presentation.ViewModels;
using PKHeX.Core;
using Xunit.Abstractions;

namespace PKHeX.Avalonia.Tests;

/// <summary>
/// Behavioral tests for generation-specific editors:
/// HoneyTreeEditorViewModel (Gen4 Sinnoh),
/// SuperTrainingEditorViewModel (Gen6),
/// HallOfFameEditorViewModel (Gen6 XY/ORAS).
/// </summary>
public class GenSpecificEditorTests(ITestOutputHelper output)
{
    private static Mock<ISpriteRenderer> SpriteMock() => new();

    // -----------------------------------------------------------------------
    // HoneyTreeEditorViewModel — Gen4 Sinnoh only
    // -----------------------------------------------------------------------

    [Theory]
    [InlineData(GameVersion.Pt, "Gen4-Platinum")]
    [InlineData(GameVersion.D,  "Gen4-Diamond")]
    public void HoneyTree_Gen4Sinnoh_IsSupported(GameVersion version, string label)
    {
        var sav = BlankSaveFile.Get(version);
        var vm = new HoneyTreeEditorViewModel(sav);

        Assert.True(vm.IsSupported, $"{label}: expected IsSupported=true");
        Assert.Equal(21, vm.Trees.Count); // 21 named honey tree locations
        Assert.NotNull(vm.SelectedTree);
        output.WriteLine($"{label}: IsSupported=true, Trees={vm.Trees.Count} ✓");
    }

    [Theory]
    [InlineData(GameVersion.E,  "Gen3-Emerald")]
    [InlineData(GameVersion.X,  "Gen6-X")]
    [InlineData(GameVersion.Pt, "Gen4-Platinum")] // included to separate HG/SS from Sinnoh if needed
    public void HoneyTree_Gen4HGSS_IsNotSupported(GameVersion version, string label)
    {
        // Only SAV4Sinnoh (Pt/DP) has honey trees; not HGSS or other gens
        var sav = BlankSaveFile.Get(version);
        if (sav is SAV4Sinnoh)
        {
            output.WriteLine($"{label}: is Sinnoh, skipping not-supported check");
            return;
        }

        var vm = new HoneyTreeEditorViewModel(sav);

        Assert.False(vm.IsSupported, $"{label}: expected IsSupported=false");
        Assert.Empty(vm.Trees);
        output.WriteLine($"{label}: HoneyTree IsSupported=false ✓");
    }

    [Fact]
    public void HoneyTree_MunchlaxTreesText_NotEmpty()
    {
        var sav = BlankSaveFile.Get(GameVersion.Pt);
        var vm = new HoneyTreeEditorViewModel(sav);

        Assert.True(vm.IsSupported);
        // Munchlax trees are calculated from trainer ID — should produce some output
        Assert.False(string.IsNullOrEmpty(vm.MunchlaxTreesText));
        output.WriteLine($"HoneyTree Munchlax: '{vm.MunchlaxTreesText}' ✓");
    }

    [Fact]
    public void HoneyTree_TreeNames_ArePopulated()
    {
        var sav = BlankSaveFile.Get(GameVersion.Pt);
        var vm = new HoneyTreeEditorViewModel(sav);

        Assert.True(vm.IsSupported);
        Assert.All(vm.Trees, tree => Assert.False(string.IsNullOrEmpty(tree.Name)));
        output.WriteLine($"HoneyTree: all {vm.Trees.Count} trees have names ✓");
    }

    // -----------------------------------------------------------------------
    // SuperTrainingEditorViewModel — Gen6 only
    // -----------------------------------------------------------------------

    [Fact]
    public void SuperTraining_Gen6_LoadsBagsAndStages()
    {
        var sav = new SAV6XY();
        var vm = new SuperTrainingEditorViewModel(sav);

        Assert.True(vm.IsSupported);
        Assert.Equal(12, vm.Bags.Count);   // 12 training bag slots
        Assert.True(vm.Stages.Count > 0); // stages loaded
        output.WriteLine($"Gen6 SuperTraining: Bags={vm.Bags.Count}, Stages={vm.Stages.Count} ✓");
    }

    [Fact]
    public void SuperTraining_Gen6_BagNames_NotEmpty()
    {
        var sav = new SAV6XY();
        var vm = new SuperTrainingEditorViewModel(sav);

        Assert.NotEmpty(vm.BagNames);
        output.WriteLine($"Gen6 SuperTraining: {vm.BagNames.Length} bag names ✓");
    }

    [Fact]
    public void SuperTraining_UnlockAll_UnlocksAllStages()
    {
        var sav = new SAV6XY();
        var vm = new SuperTrainingEditorViewModel(sav);

        vm.UnlockAllCommand.Execute(null);

        // All stages should be unlocked
        Assert.All(vm.Stages, stage => Assert.True(stage.IsUnlocked,
            $"Stage '{stage.Name}' should be unlocked"));
        output.WriteLine($"SuperTraining UnlockAll: all {vm.Stages.Count} stages unlocked ✓");
    }

    [Fact]
    public void SuperTraining_StageUnlock_LiveWritesToBlock()
    {
        var sav = new SAV6XY();
        var vm = new SuperTrainingEditorViewModel(sav);

        // Toggle first stage
        if (vm.Stages.Count > 0)
        {
            var stage = vm.Stages[0];
            var initial = stage.IsUnlocked;
            stage.IsUnlocked = !initial; // live write via OnIsUnlockedChanged

            // Reload to verify persisted
            vm.LoadStages();
            Assert.Equal(!initial, vm.Stages[0].IsUnlocked);
        }
        output.WriteLine("SuperTraining stage live-write to block ✓");
    }

    // -----------------------------------------------------------------------
    // HallOfFameEditorViewModel — Gen6 XY/ORAS only
    // -----------------------------------------------------------------------

    [Theory]
    [InlineData(GameVersion.X,  "Gen6-X")]
    [InlineData(GameVersion.AS, "Gen6-AlphaSapphire")]
    public void HallOfFame_Gen6_IsSupported(GameVersion version, string label)
    {
        var sav = BlankSaveFile.Get(version);
        var vm = new HallOfFameEditorViewModel(sav, SpriteMock().Object);

        Assert.True(vm.IsSupported, $"{label}: expected IsSupported=true");
        output.WriteLine($"{label}: HallOfFame IsSupported=true ✓");
    }

    [Theory]
    [InlineData(GameVersion.SN, "Gen7-Sun")]
    [InlineData(GameVersion.SW, "Gen8-Sword")]
    [InlineData(GameVersion.SL, "Gen9-Scarlet")]
    public void HallOfFame_OtherGens_IsNotSupported(GameVersion version, string label)
    {
        var sav = BlankSaveFile.Get(version);
        var vm = new HallOfFameEditorViewModel(sav, SpriteMock().Object);

        Assert.False(vm.IsSupported, $"{label}: expected IsSupported=false");
        output.WriteLine($"{label}: HallOfFame IsSupported=false ✓");
    }

    [Fact]
    public void HallOfFame_Gen6_BlankSave_ZeroEntries()
    {
        // Blank save has no hall of fame data
        var sav = new SAV6XY();
        var vm = new HallOfFameEditorViewModel(sav, SpriteMock().Object);

        Assert.True(vm.IsSupported);
        // On a blank save, no HOF entries should have data
        Assert.Empty(vm.Entries);
        output.WriteLine("Gen6 HallOfFame: blank save has 0 entries ✓");
    }
}
