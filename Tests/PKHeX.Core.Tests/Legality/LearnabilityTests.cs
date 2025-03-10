using System;
using System.Linq;
using FluentAssertions;
using Xunit;

namespace PKHeX.Core.Tests.Legality;

public static class LearnabilityTests
{
    [Theory]
    [InlineData(nameof(Species.Bulbasaur), "Razor Leaf", "Vine Whip")]
    [InlineData(nameof(Species.Charizard), "Fly")]
    [InlineData(nameof(Species.Mew), "Pound")]
    [InlineData(nameof(Species.Smeargle), "Frenzy Plant")]
    public static void VerifyCanLearn(string species, params string[] moves)
    {
        var encs = EncounterLearn.GetLearn(species, moves);
        encs.Any().Should().BeTrue($"{species} should be able to learn all moves: {string.Join(", ", moves)}");
    }

    [Theory]
    [InlineData(nameof(Species.Perrserker), "Swift")]
    [InlineData(nameof(Species.Perrserker), "Shock Wave")]
    [InlineData(nameof(Species.Sirfetchd), "False Swipe")]
    [InlineData(nameof(Species.Bulbasaur), "Fly")]
    [InlineData(nameof(Species.Charizard), "Bubble")]
    [InlineData(nameof(Species.Mew), "Struggle")]
    [InlineData(nameof(Species.Smeargle), "Chatter")]
    public static void VerifyCannotLearn(string species, params string[] moves)
    {
        var encs = EncounterLearn.GetLearn(species, moves);
        encs.Any().Should().BeFalse($"{species} should not be able to learn all moves: {string.Join(", ", moves)}");
    }

    [Theory]
    [InlineData(nameof(Species.Chansey), "Wish")]
    [InlineData("Ho-Oh", "Celebrate")]
    [InlineData(nameof(Species.Pikachu), "Happy Hour")]
    [InlineData(nameof(Species.Rayquaza), "V-Create")]
    public static void VerifyCanLearnSpecial(string species, params string[] moves)
    {
        var encs = EncounterLearn.GetLearn(species, moves);
        encs.Any().Should().BeTrue($"{species} should be able to learn all moves: {string.Join(", ", moves)}");
    }

    [Theory]
    [InlineData("flaBeBe", "pEtaL Dance")]
    public static void VerifyCanLearnParse(string species, params string[] moves)
    {
        var encs = EncounterLearn.GetLearn(species, moves);
        encs.Any().Should().BeTrue($"{species} should be able to learn all moves: {string.Join(", ", moves)}");
    }

    [Theory]
    [InlineData(nameof(Species.Unown), "Hidden Power")]
    [InlineData(nameof(Species.Pikachu), "Hidden Power")]
    public static void VerifyCanLearnTM(string species, params string[] moves)
    {
        var can = EncounterLearn.CanLearn(species, moves);
        can.Should().BeTrue($"{species} should be able to learn all moves: {string.Join(", ", moves)}");
    }

    [Theory]
    [InlineData(GameVersion.GS, Species.Tyrogue, Move.HighJumpKick, Move.MachPunch, Move.RapidSpin)]
    [InlineData(GameVersion.GS, Species.Chansey, Move.DoubleEdge)]
    public static void VerifyCanLearnEgg(GameVersion version, Species species, params Move[] moves)
    {
        var needs = new ushort[4];
        moves.CopyTo(needs, 0);
        var can = EggMoveVerifier.IsPossible(needs, (ushort)species, version);
        can.Should().BeTrue($"{species} in {version} should be able to learn all moves: {string.Join(", ", moves)}");
    }

    [Theory]
    [InlineData(GameVersion.HGSS, Species.Mankey, Species.Smeargle, Move.Encore, Move.Meditate, Move.SmellingSalts)]
    public static void VerifyCanLearnEggFather(GameVersion version, Species child, Species father, params Move[] moves)
    {
        var needs = new ushort[4];
        moves.CopyTo(needs, 0);
        var can = EggMoveVerifier.IsPossible(needs, (ushort)child, version, out var chain, out _);
        chain?[0].Should().Be((ushort)father);
        can.Should().BeTrue($"{child} in {version} should be able to learn all moves: {string.Join(", ", moves)}");
    }

    [Theory]
    [InlineData(typeof(EncounterShadow3XD), GameVersion.B2W2, Species.Shellder, Move.Avalanche, Move.TakeDown)]
    public static void VerifyCanLearnEggSpecial(Type encType, GameVersion version, Species species, params Move[] moves)
    {
        var needs = new ushort[4];
        moves.CopyTo(needs, 0);
        var can = EggMoveVerifier.IsPossible(needs, (ushort)species, version, out _, out var enc);
        enc.Should().BeOfType(encType);
        can.Should().BeTrue($"{species} in {version} should be able to learn all moves: {string.Join(", ", moves)}");
    }

    [Theory]
    [InlineData(GameVersion.FRLG, Species.Squirtle, Move.Haze, Move.Flail)]
    [InlineData(GameVersion.DP, Species.Slugma, Move.HeatWave, Move.Smokescreen)]
    [InlineData(GameVersion.B2W2, Species.Chansey, Move.EggBomb)]
    public static void VerifyCannotLearnEgg(GameVersion version, Species species, params Move[] moves)
    {
        var needs = new ushort[4];
        moves.CopyTo(needs, 0);
        var can = EggMoveVerifier.IsPossible(needs, (ushort)species, version);
        can.Should().BeFalse($"{species} in {version} should not be able to learn all moves: {string.Join(", ", moves)}");
    }
}
