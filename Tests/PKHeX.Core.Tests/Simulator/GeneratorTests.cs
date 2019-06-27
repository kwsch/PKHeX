using System.Collections.Generic;
using FluentAssertions;
using PKHeX.Core;
using Xunit;

namespace PKHeX.Tests.Simulator
{
    public class GeneratorTests
    {
        public static IEnumerable<object[]> PokemonGenerationTestData()
        {
            for (int i = 1; i <= 807; i++)
            {
                yield return new object[] { i };
            }
        }

        [Theory(Skip = "Feature not ready yet")]
        [MemberData(nameof(PokemonGenerationTestData))]
        public void PokemonGenerationReturnsLegalPokemon(int species)
        {
            int count = 0;
            var tr = new SimpleTrainerInfo();

            var pk = new PK7 { Species = species };
            pk.Gender = pk.GetSaneGender();
            var ez = EncounterMovesetGenerator.GeneratePKMs(pk, tr);
            foreach (var e in ez)
            {
                var la = new LegalityAnalysis(e);
                la.Valid.Should().BeTrue($"Because generated Pokemon {count} for {species:000} should be valid");
                Assert.True(la.Valid);
                count++;
            }
        }
    }
}