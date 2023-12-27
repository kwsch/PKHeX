using System.Linq;
using FluentAssertions;
using Xunit;

namespace PKHeX.Core.Tests.Legality;

public static class LearnabilityTests
{
    [Theory]
    [InlineData(nameof(Species.Bulbasaur),  "Razor Leaf", "Vine Whip")]
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
}
