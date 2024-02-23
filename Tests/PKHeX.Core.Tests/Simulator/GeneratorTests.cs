using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Xunit;
using static PKHeX.Core.GameVersion;
using PT = PKHeX.Core.PersonalTable;
using TR = PKHeX.Core.SimpleTrainerInfo;
#pragma warning disable xUnit1004 // Test methods should not be skipped

namespace PKHeX.Core.Tests.Simulator;

public class GeneratorTests
{
    private const string SkipReasonLong = "Long duration test, run manually & very infrequently.";
    static GeneratorTests() => TestUtil.InitializeLegality();

    public static IEnumerable<object[]> GetSpecies17() => GetSpecies(PT.USUM, new TR(US), () => new PK7(), []);
    public static IEnumerable<object[]> GetSpeciesLGPE() => GetSpecies(PT.GG, new TR(GP), () => new PB7(), [GP, GE]);
    public static IEnumerable<object[]> GetSpeciesSWSH() => GetSpecies(PT.SWSH, new TR(SW), () => new PK8(), [SW, SH]);
    public static IEnumerable<object[]> GetSpeciesPLA() => GetSpecies(PT.LA, new TR(PLA), () => new PA8(), [PLA]);
    public static IEnumerable<object[]> GetSpeciesBDSP() => GetSpecies(PT.BDSP, new TR(BD), () => new PB8(), [BD, SP]);
    public static IEnumerable<object[]> GetSpeciesSV() => GetSpecies(PT.SV, new TR(SL), () => new PK9(), [SL, VL]);

    private static IEnumerable<object[]> GetSpecies<T>(T table, TR tr, Func<Core.PKM> ctor, GameVersion[] games) where T : IPersonalTable
    {
        for (ushort i = 1; i <= table.MaxSpeciesID; i++)
        {
            if (table.IsSpeciesInGame(i))
                yield return [(Species)i, tr, ctor(), games];
        }
    }

    [Theory(Skip = SkipReasonLong)]
    [MemberData(nameof(GetSpecies17))]
    [MemberData(nameof(GetSpeciesLGPE))]
    [MemberData(nameof(GetSpeciesSWSH))]
    [MemberData(nameof(GetSpeciesPLA))]
    [MemberData(nameof(GetSpeciesBDSP))]
    [MemberData(nameof(GetSpeciesSV))]
    public void PokemonGenerationReturnsLegalPokemon(Species species, TR tr, Core.PKM template, GameVersion[] games)
    {
        int count = 0;
        template.Species = (ushort)species;
        template.Gender = template.PersonalInfo.RandomGender();
        var moves = ReadOnlyMemory<ushort>.Empty;
        var encounters = EncounterMovesetGenerator.GenerateEncounters(template, tr, moves, games);

        foreach (var enc in encounters)
        {
            var pk = enc.ConvertToPKM(tr);
            var la = new LegalityAnalysis(pk);
            la.Valid.Should().BeTrue($"Because encounter #{count} for {species} ({(ushort)species:000}) should be valid, {Environment.NewLine}{la.Report()}{Environment.NewLine}{enc}");
            count++;
        }
    }

    [Fact]
    public void CanGenerateMG5Case()
    {
        const Species species = Species.Haxorus;
        var pk5 = new PK5 {Species = (int) species};
        var moves = ReadOnlyMemory<ushort>.Empty;
        var ez = EncounterMovesetGenerator.GenerateEncounters(pk5, moves, W2).OfType<EncounterStatic5>().First();
        ez.Should().NotBeNull("Shiny Haxorus stationary encounter exists for B2/W2");

        var criteria = EncounterCriteria.Unrestricted;
        var tr = new TR(B2)
        {
            TID16 = 57600,
            SID16 = 62446,
        };

        for (var nature = Nature.Hardy; nature <= Nature.Quirky; nature++)
        {
            criteria = criteria with {Nature = nature};
            var pk = ez.ConvertToPKM(tr, criteria);
            pk.Nature.Should().Be(nature, "not nature locked");
            pk.IsShiny.Should().BeTrue("encounter is shiny locked");
            pk.TID16.Should().Be(tr.TID16);
            pk.SID16.Should().Be(tr.SID16);
        }
    }
}
