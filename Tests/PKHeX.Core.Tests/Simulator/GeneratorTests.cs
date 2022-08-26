using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using PKHeX.Core;
using Xunit;

namespace PKHeX.Tests.Simulator;

public class GeneratorTests
{
    static GeneratorTests()
    {
        if (!EncounterEvent.Initialized)
            EncounterEvent.RefreshMGDB();
    }

    public static IEnumerable<object[]> PokemonGenerationTestData()
    {
        for (int i = 1; i <= 807; i++)
        {
            yield return new object[] { i };
        }
    }

    [Theory(Skip = "Feature not ready yet")]
    [MemberData(nameof(PokemonGenerationTestData))]
    public void PokemonGenerationReturnsLegalPokemon(ushort species)
    {
        int count = 0;
        var tr = new SimpleTrainerInfo(GameVersion.SN);

        var pk = new PK7 { Species = species };
        pk.Gender = pk.GetSaneGender();
        var ez = EncounterMovesetGenerator.GeneratePKMs(pk, tr, pk.Moves);
        foreach (var e in ez)
        {
            var la = new LegalityAnalysis(e);
            la.Valid.Should().BeTrue($"Because generated Pokemon {count} for {species:000} should be valid");
            Assert.True(la.Valid);
            count++;
        }
    }

    [Fact]
    public void CanGenerateMG5Case()
    {
        const Species species = Species.Haxorus;
        var pk5 = new PK5 {Species = (int) species};
        var ez = EncounterMovesetGenerator.GenerateEncounters(pk5, pk5.Moves, GameVersion.W2).OfType<EncounterStatic>().First();
        ez.Should().NotBeNull("Shiny Haxorus stationary encounter exists for B2/W2");

        var criteria = EncounterCriteria.Unrestricted;
        var tr = new SimpleTrainerInfo(GameVersion.B2)
        {
            TID = 57600,
            SID = 62446,
        };

        for (var nature = Nature.Hardy; nature <= Nature.Quirky; nature++)
        {
            criteria = criteria with {Nature = nature};
            var pk = ez.ConvertToPKM(tr, criteria);
            pk.Nature.Should().Be((int)nature, "not nature locked");
            pk.IsShiny.Should().BeTrue("encounter is shiny locked");
            pk.TID.Should().Be(tr.TID);
            pk.SID.Should().Be(tr.SID);
        }
    }
}
