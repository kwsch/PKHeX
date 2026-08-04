using PKHeX.Core;
using Xunit;

namespace PKHeX.Avalonia.Tests;

/// <summary>
/// Tests for the interactive Experience bar wiring on <see cref="PKHeX.Presentation.ViewModels.PokemonEditorViewModel"/>.
/// Ports upstream's <c>ExperienceBar</c> behavior: the bar fills to the current level's progress, and clicking/dragging
/// sets EXP within the current level (never changing the level).
/// </summary>
public class ExperienceBarTests
{
    private static (PKHeX.Presentation.ViewModels.PokemonEditorViewModel VM, byte Growth) CreateVm(byte level)
    {
        var sav = BlankSaveFile.Get(GameVersion.SL);
        var pkm = new PK9 { Species = 906 }; // Sprigatito
        var (vm, _, _) = TestHelpers.CreateTestViewModel(pkm, sav);

        var growth = pkm.PersonalInfo.EXPGrowth;
        vm.Level = level;
        // Seat EXP at the start of the level so we have a deterministic baseline.
        vm.Exp = Experience.GetEXP(level, growth);
        return (vm, growth);
    }

    [Fact]
    public void ExpPercent_ReflectsKnownLevelExpGrowth()
    {
        const byte level = 50;
        var (vm, growth) = CreateVm(level);

        var start = Experience.GetEXP(level, growth);
        var range = Experience.GetEXPToLevelUp(level, growth);

        // Place EXP roughly mid-level and confirm the bar percent matches the Core formula.
        var exp = start + (range / 2);
        vm.Exp = exp;

        var expected = Experience.GetEXPToLevelUpPercentage(level, exp, growth);
        Assert.Equal(expected, vm.ExpPercent, 6);
        Assert.InRange(vm.ExpPercent, 0.0, 1.0);
    }

    [Fact]
    public void ExpPercent_AtStartOfLevel_IsZero()
    {
        const byte level = 50;
        var (vm, _) = CreateVm(level);
        Assert.Equal(0.0, vm.ExpPercent, 6);
    }

    [Fact]
    public void SetExpFromFraction_Zero_SetsExpToLevelStart()
    {
        const byte level = 42;
        var (vm, growth) = CreateVm(level);

        // Move off the start first, then snap back to 0.
        var range = Experience.GetEXPToLevelUp(level, growth);
        vm.Exp = Experience.GetEXP(level, growth) + (range / 2);

        vm.SetExpFromFraction(0.0);

        Assert.Equal(Experience.GetEXP(level, growth), (uint)vm.Exp);
        Assert.Equal(0.0, vm.ExpPercent, 6);
    }

    [Fact]
    public void SetExpFromFraction_NearOne_SetsNearLevelHighEdge()
    {
        const byte level = 42;
        var (vm, growth) = CreateVm(level);

        var start = Experience.GetEXP(level, growth);
        var range = Experience.GetEXPToLevelUp(level, growth);
        var highEdge = start + range - 1; // one EXP below the next level-up

        vm.SetExpFromFraction(0.999999);

        // Must stay within the current level (strictly below the next level threshold)...
        Assert.True((uint)vm.Exp < start + range);
        Assert.Equal(level, Experience.GetLevel((uint)vm.Exp, growth));
        // ...and land at (or essentially at) the high edge.
        Assert.True((uint)vm.Exp <= highEdge);
        Assert.True(highEdge - (uint)vm.Exp <= 1);
    }

    [Fact]
    public void SetExpFromFraction_ClampsAboveOne_StaysWithinLevel()
    {
        const byte level = 30;
        var (vm, growth) = CreateVm(level);

        var start = Experience.GetEXP(level, growth);
        var range = Experience.GetEXPToLevelUp(level, growth);

        vm.SetExpFromFraction(5.0); // out of range, should clamp to the high edge of the level

        Assert.Equal(start + range - 1, (uint)vm.Exp);
        Assert.Equal(level, Experience.GetLevel((uint)vm.Exp, growth));
    }

    [Fact]
    public void SetExpToLevelEdgeHigh_SetsHighEdge()
    {
        const byte level = 30;
        var (vm, growth) = CreateVm(level);

        var start = Experience.GetEXP(level, growth);
        var range = Experience.GetEXPToLevelUp(level, growth);

        vm.SetExpToLevelEdgeHigh();

        Assert.Equal(start + range - 1, (uint)vm.Exp);
        Assert.Equal(level, Experience.GetLevel((uint)vm.Exp, growth));
    }

    [Fact]
    public void SetExpFromFraction_AtMaxLevel_IsNoOp()
    {
        const byte level = 100;
        var (vm, _) = CreateVm(level);
        var before = vm.Exp;

        vm.SetExpFromFraction(0.0);
        vm.SetExpFromFraction(0.5);
        vm.SetExpToLevelEdgeHigh();

        Assert.Equal(before, vm.Exp);
        // At MaxLevel the bar reads as full.
        Assert.Equal(1.0, vm.ExpPercent, 6);
    }
}
