using System;
using System.Runtime.InteropServices;
using FluentAssertions;
using Xunit;
using static PKHeX.Core.GameVersion;
using static PKHeX.Core.Move;
using static PKHeX.Core.Species;

namespace PKHeX.Core.Tests.Legality;

public class ChainBreedLegalityTests
{
    private static ReadOnlySpan<ushort> GetMoves(ReadOnlySpan<Move> moves)
        => MemoryMarshal.Cast<Move, ushort>(moves);

    [Theory]
    [InlineData(BW, Marill, BellyDrum, AquaJet)]
    [InlineData(B2W2, Azurill, BellyDrum, AquaJet)]
    [InlineData(FR, Squirtle, Haze, Flail)]
    [InlineData(B2W2, Chansey, EggBomb)]
    [InlineData(GS, Oddish, Flail, RazorLeaf, SwordsDance, Synthesis)]
    [InlineData(GS, Smoochum, LovelyKiss)] // egg move removed from table (no parents)
    public void DetectsInvalidChains(GameVersion version, Species species, params Move[] movelist)
    {
        var moves = GetMoves(movelist);
        ChainBreedLegality.IsValid((ushort)species, 0, version, moves).Should().BeFalse();
    }

    [Theory]
    [InlineData(HGSS, Slugma, Smokescreen, HeatWave)] // Heat Wave is a Tutor in HG/SS.
    [InlineData(GS, Paras, Counter, Flail, LightScreen)]
    [InlineData(HGSS, Mankey, Encore, Meditate, SmellingSalts)]
    [InlineData(GS, Chansey, DoubleEdge)] // via Jigglypuff (Level 39)
    [InlineData(Pt, Shellder, RapidSpin, IcicleSpear)]
    [InlineData(R, Volbeat, HelpingHand)] // level up, breed with Illumise
    public void DetectsValidChains(GameVersion version, Species species, params Move[] movelist)
        => ValidateSimple(version, species, 0, movelist);

    [Theory]
    // Avalanche learned in Gen4 TM, TakeDown learned via *special encounter* move in Gen3 XD.
    [InlineData(B2W2, Shellder, TakeDown)] // Gen3 encounter move (in XD)
    [InlineData(B2W2, Shellder, Avalanche)] // Gen4 TM move
    [InlineData(B2W2, Shellder, Avalanche, TakeDown)] // Valid Gen5 parent from a Gen3 encounter=>Gen4=>Gen5 transfer route.
    [InlineData(BW, Munchlax, Curse, SelfDestruct, Counter)] // Gen3 (Curse Egg) XD Tutor SelfDestruct, Gen3 Tutor (Counter) => Gen5 breed.
    [InlineData(HGSS, Corphish, MetalClaw, DragonDance)] // Gen3 Charmander (Dragon Dance egg) => Gen3 Level Up => Gen4 Totodile => Gen4 Corphish.
    public void DetectValidChainPastFather(GameVersion version, Species species, params Move[] movelist)
        => ValidateSimple(version, species, 0, movelist);

    [Theory]
    // evolve=>pass chain with same species lineage: multiple Tyrogue evolutions (hitmonlee, hitmonchan, hitmontop), all providing one move.
    [InlineData(GS, Tyrogue, 0, HighJumpKick, MachPunch, RapidSpin)]
    public void DetectValidChainCyclic(GameVersion version, Species species, byte form, params Move[] movelist)
        => ValidateSimple(version, species, form, movelist);

    private static void ValidateSimple(GameVersion version, Species species, byte form, ReadOnlySpan<Move> movelist)
    {
        var moves = GetMoves(movelist);
        ChainBreedLegality.IsValid((ushort)species, form, version, moves, out var summary).Should().BeTrue();
        summary.MotherSpecies.Should().NotBe(0);
        summary.FatherSpecies.Should().NotBe(0);
        summary.ChainDepth.Should().BeGreaterThan(0);
    }

    [Theory]
    [InlineData(HGSS, Mankey, 0, Smeargle, Encore, Meditate, SmellingSalts)]
    [InlineData(E, Pichu, 0, Smeargle, Reversal, Encore, Wish, Present)] // via Pikachu mother
    public void DetectsValidChainSmeargle(GameVersion version, Species species, byte form, Species father, params Move[] movelist)
    {
        var moves = GetMoves(movelist);
        ChainBreedLegality.IsValid((ushort)species, form, version, moves, out var summary).Should().BeTrue();
        summary.MotherSpecies.Should().NotBe(0);
        summary.FatherSpecies.Should().Be((ushort)father);
        summary.ChainDepth.Should().BeGreaterThan(0);
    }

    [Theory]
    [InlineData(BD, Smoochum, false, PowderSnow)] // none in Human-like
    [InlineData(BD, Smoochum, true, Confusion)] // via Alakazam
    // Female only species: Male must be able to pass inheritable level up move.
    public void DetectsInvalidInheritedLevelUpMove(GameVersion version, Species species, bool expect, params Move[] movelist)
    {
        var moves = GetMoves(movelist);
        ChainBreedLegality.IsValid((ushort)species, 0, version, moves).Should().Be(expect);
    }

    [Theory]
    // For breeding purposes of Volbeat and Nidoran-M, the father must be able to pass all egg moves.
    // If Gen6+, the mother can aggregate egg moves, so mother egg moves can be ignored.
    // Volbeat can get Lunge from Mothim and Dewpider, but Dizzy Punch is only from Spinda and Lopunny.
    // Level up moves are not considered, as Volbeat/etc can breed with Ditto to pass level-up and acquired egg chain moves.
    // This check is only relevant up to Gen7, as egg move sharing became a thing.
    [InlineData(US, Volbeat, false, DizzyPunch, Lunge)] // mother can't learn either, father must pass both (none can do both).
    [InlineData(B2, Volbeat, true, DizzyPunch, SeismicToss)] // Gen3 Spinda (Dizzy Punch level up) tutored with Seismic Toss
    [InlineData(B2, Volbeat, true, DizzyPunch, BugBuzz)] // Gen3 Spinda (Dizzy Punch level up), both parents level up
    [InlineData(B2, Volbeat, true, DizzyPunch, Trick)] // Spinda can get Trick via Tutor in B2/W2
    [InlineData(HG, Volbeat, false, DizzyPunch, Trick)] // Spinda can get Trick via Tutor in B2/W2
    // Amnesia via Psyduck, Head Smash via Rampardos. No father can pass both when breeding with Nidoran-F, but it can breed with Smeargle!
    [InlineData(X, NidoranM, true, Amnesia, HeadSmash)] // mother can't learn either, father must pass both (none can do both).
    [InlineData(Y, NidoranM, true, BeatUp, HeadSmash)] // mother can pass Beat Up, father can pass Head Smash.
    [InlineData(B2, NidoranM, true, BeatUp, HeadSmash)] // father must pass both in Gen5 (Smeargle)
    public void DetectInvalidSpeciesMaleSplit(GameVersion version, Species species, bool expect, params Move[] movelist)
    {
        var moves = GetMoves(movelist);
        ChainBreedLegality.IsValid((ushort)species, 0, version, moves).Should().Be(expect);
    }
}
