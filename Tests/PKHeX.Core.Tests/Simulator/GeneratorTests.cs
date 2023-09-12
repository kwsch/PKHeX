using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Xunit;
#pragma warning disable xUnit1004 // Test methods should not be skipped

namespace PKHeX.Core.Tests.Simulator;

public class GeneratorTests
{
    public static IEnumerable<object[]> PokemonGenerationTestData()
    {
        for (int i = 1; i <= 807; i++)
            yield return new object[] { i };
    }

    [Theory(Skip = "Long duration test, run manually & very infrequently.")]
    [MemberData(nameof(PokemonGenerationTestData))]
    public void PokemonGenerationReturnsLegalPokemon(ushort species)
    {
        int count = 0;
        var tr = new SimpleTrainerInfo(GameVersion.SN);

        var template = new PK7 { Species = species };
        template.Gender = template.PersonalInfo.RandomGender();
        var encounters = EncounterMovesetGenerator.GenerateEncounters(template, tr, Array.Empty<ushort>());

        foreach (var enc in encounters)
        {
            var pk = enc.ConvertToPKM(tr);
            var la = new LegalityAnalysis(pk);
            la.Valid.Should().BeTrue($"Because encounter #{count} for {(Species)species} ({species:000}) should be valid, {Environment.NewLine}{la.Report()}");
            count++;
        }
    }

    [Fact]
    public void CanGenerateMG5Case()
    {
        const Species species = Species.Haxorus;
        var pk5 = new PK5 {Species = (int) species};
        var ez = EncounterMovesetGenerator.GenerateEncounters(pk5, pk5.Moves, GameVersion.W2).OfType<EncounterStatic5>().First();
        ez.Should().NotBeNull("Shiny Haxorus stationary encounter exists for B2/W2");

        var criteria = EncounterCriteria.Unrestricted;
        var tr = new SimpleTrainerInfo(GameVersion.B2)
        {
            TID16 = 57600,
            SID16 = 62446,
        };

        for (var nature = Nature.Hardy; nature <= Nature.Quirky; nature++)
        {
            criteria = criteria with {Nature = nature};
            var pk = ez.ConvertToPKM(tr, criteria);
            pk.Nature.Should().Be((int)nature, "not nature locked");
            pk.IsShiny.Should().BeTrue("encounter is shiny locked");
            pk.TID16.Should().Be(tr.TID16);
            pk.SID16.Should().Be(tr.SID16);
        }
    }
}
