using System;
using FluentAssertions;
using Xunit;

namespace PKHeX.Core.Tests.Legality;

public class EffortExpLegalityTests
{
    private static readonly int[] Empty = new int[6];

    [Fact]
    public void ZeroEVs_ReturnsZero()
    {
        EffortExpLegality.GetRequiredEffortEXP(
                Empty,
                gainedEXP: 999,
                hasPokerus: false,
                originFormat: 4,
                currentFormat: 4)
            .Should().Be(0);
    }

    [Theory]
    [InlineData(100, 0)]
    [InlineData(101, 1)]
    [InlineData(104, 4)]
    [InlineData(150, 10)] // 10 * 5[power item] = 50 EVs, 10 EXP.
    public void Vitamins_AreFreeUpTo100(int hp, int expectedRemaining)
    {
        Span<int> evs = [hp, 0, 0, 0, 0, 0];

        EffortExpLegality.GetRequiredEffortEXP(
                evs,
                gainedEXP: 0,
                hasPokerus: false,
                originFormat: 4,
                currentFormat: 4)
            .Should().Be(expectedRemaining);
    }

    [Fact]
    public void Gen4_NoPokerus_UsesPowerItemEfficiency()
    {
        Span<int> evs = [115, 0, 0, 0, 0, 0];

        // 15 actual EVs = 5 + 5 + 5.
        EffortExpLegality.GetRequiredEffortEXP(
                evs,
                gainedEXP: 0,
                hasPokerus: false,
                originFormat: 4,
                currentFormat: 4)
            .Should().Be(3);
    }

    [Fact]
    public void Gen4_Pokerus_CanBeIntroducedMidTraining()
    {
        Span<int> evs = [115, 0, 0, 0, 0, 0];

        // 10 EV after Pokerus + 5 EV before/without Pokerus.
        // Only 2 EXP required.
        EffortExpLegality.GetRequiredEffortEXP(
                evs,
                gainedEXP: 0,
                hasPokerus: true,
                originFormat: 4,
                currentFormat: 4)
            .Should().Be(2);
    }

    [Theory]
    [InlineData(106, 2)] // 6 = 5 + 1
    [InlineData(109, 3)] // 9 = 5 + 2 + 2
    [InlineData(111, 2)] // 11 = 10 + 1
    [InlineData(114, 3)] // 14 = 10 + 2 + 2
    public void Gen4_Pokerus_HandlesRemainders(int hp, int expectedEXP)
    {
        Span<int> evs = [hp, 0, 0, 0, 0, 0];

        EffortExpLegality.GetRequiredEffortEXP(
                evs,
                gainedEXP: 0,
                hasPokerus: true,
                originFormat: 4,
                currentFormat: 4)
            .Should().Be(expectedEXP);
    }

    [Fact]
    public void Gen4_SufficientEXP_ReturnsNegativeDelta()
    {
        Span<int> evs = [115, 0, 0, 0, 0, 0];

        EffortExpLegality.GetRequiredEffortEXP(
                evs,
                gainedEXP: 5,
                hasPokerus: false,
                originFormat: 4,
                currentFormat: 4)
            .Should().Be(-2);
    }

    [Fact]
    public void Gen4_InsufficientEXP_ReturnsPositiveDelta()
    {
        Span<int> evs = [115, 0, 0, 0, 0, 0];

        EffortExpLegality.GetRequiredEffortEXP(
                evs,
                gainedEXP: 1,
                hasPokerus: false,
                originFormat: 4,
                currentFormat: 4)
            .Should().Be(2);
    }

    [Fact]
    public void Gen3_HP_UsesMachoBrace()
    {
        Span<int> evs = [108, 0, 0, 0, 0, 0];

        // 8 EV = 4 + 4, one EXP each.
        EffortExpLegality.GetRequiredEffortEXP(
                evs,
                gainedEXP: 0,
                hasPokerus: true,
                originFormat: 3,
                currentFormat: 3)
            .Should().Be(2);
    }

    [Fact]
    public void Gen3_HP_PokerusDoublesMachoBrace()
    {
        Span<int> evs = [108, 0, 0, 0, 0, 0];

        // 8 EV can be obtained in two 4-EV batches.
        EffortExpLegality.GetRequiredEffortEXP(
                evs,
                gainedEXP: 0,
                hasPokerus: true,
                originFormat: 3,
                currentFormat: 3)
            .Should().Be(2);
    }

    [Fact]
    public void Gen3_Defense_PrefersEightEVThreeEXPRoute()
    {
        Span<int> evs = [0, 0, 108, 0, 0, 0];

        // 8 EV = one Pokerus + Macho Brace batch = 3 EXP.
        EffortExpLegality.GetRequiredEffortEXP(
                evs,
                gainedEXP: 0,
                hasPokerus: true,
                originFormat: 3,
                currentFormat: 3)
            .Should().Be(3);
    }

    [Fact]
    public void Gen3_Defense_HandlesOddResidual()
    {
        Span<int> evs = [0, 0, 101, 0, 0, 0];

        // Vitamin -> 1 EV remaining.
        // Uses the 1 EV / 1 EXP fallback.
        EffortExpLegality.GetRequiredEffortEXP(
                evs,
                gainedEXP: 0,
                hasPokerus: false,
                originFormat: 3,
                currentFormat: 3)
            .Should().Be(4);
    }

    [Fact]
    public void Gen3_SpecialAttack_UsesThreeEXPDonor()
    {
        Span<int> evs = [0, 0, 0, 0, 101, 0];

        // One remaining SpA EV, Ralts route = 3 EXP.
        EffortExpLegality.GetRequiredEffortEXP(
                evs,
                gainedEXP: 0,
                hasPokerus: false,
                originFormat: 3,
                currentFormat: 3)
            .Should().Be(3);
    }

    [Fact]
    public void Gen3_SpecialDefense_UsesTwoEXPDonor()
    {
        Span<int> evs = [0, 0, 0, 0, 0, 101];

        // One remaining SpD EV, Lotad route = 2 EXP.
        EffortExpLegality.GetRequiredEffortEXP(
                evs,
                gainedEXP: 0,
                hasPokerus: false,
                originFormat: 3,
                currentFormat: 3)
            .Should().Be(2);
    }

    [Fact]
    public void Gen3_Defense_ChoosesEfficientMixForNonMultipleOfEight()
    {
        Span<int> evs = [0, 0, 110, 0, 0, 0];

        // 10 EV:
        //   8 EV / 3 EXP
        //   2 EV / 3 EXP
        // => 6 EXP
        EffortExpLegality.GetRequiredEffortEXP(
                evs,
                gainedEXP: 0,
                hasPokerus: true,
                originFormat: 3,
                currentFormat: 3)
            .Should().Be(6);
    }

    [Fact]
    public void Gen3Origin_CanUseGen4TrainingAfterTransfer()
    {
        Span<int> evs = [115, 0, 0, 0, 0, 0];

        // Originated in Gen3, currently Gen4.
        // Gen4 route is much cheaper: 15 EV => 3 EXP without Pokerus.
        EffortExpLegality.GetRequiredEffortEXP(
                evs,
                gainedEXP: 0,
                hasPokerus: false,
                originFormat: 3,
                currentFormat: 4)
            .Should().Be(3);
    }

    [Fact]
    public void MultipleStats_AreIndependent()
    {
        Span<int> evs =
        [
            115, // HP 15
            110, // Atk 10
            105, // Def 5
            106, // Spe 6
            103, // SpA 3
            102, // SpD 2
        ];

        // Gen4 + Pokerus:
        // HP  15 -> 2
        // Atk 10 -> 1
        // Def  5 -> 1
        // Spe  6 -> 2
        // SpA  3 -> 2
        // SpD  2 -> 1
        // Total = 8
        EffortExpLegality.GetRequiredEffortEXP(
                evs,
                gainedEXP: 0,
                hasPokerus: true,
                originFormat: 4,
                currentFormat: 4)
            .Should().Be(9);
    }

    [Fact]
    public void ExistingEXP_IsSubtractedFromRequirement()
    {
        Span<int> evs = [115, 110, 105, 106, 103, 102];

        EffortExpLegality.GetRequiredEffortEXP(
                evs,
                gainedEXP: 7,
                hasPokerus: true,
                originFormat: 4,
                currentFormat: 4)
            .Should().Be(2);
    }

    [Theory]
    [InlineData(false, 14)]
    [InlineData(true, 7)]
    public void ESPHERE_ElectrodeSpread(bool hasPokerus, int expect)
    {
        Span<int> evs = [112, 80, 40, 116, 60, 90];
        // 1,2,4* per exp
        // HP  12 -> 3
        // Spe 16 -> 4
        // No pokerus requires twice as many as 4* is not available

        EffortExpLegality.GetRequiredEffortEXP(
                evs,
                gainedEXP: 0,
                hasPokerus: hasPokerus,
                originFormat: 3,
                currentFormat: 3)
            .Should().Be(expect);
    }
}
