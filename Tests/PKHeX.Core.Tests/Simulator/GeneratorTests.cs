using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Xunit;
#pragma warning disable xUnit1004 // Test methods should not be skipped

namespace PKHeX.Core.Tests.Simulator;

public class GeneratorTests
{
    private const string SkipReasonLong = "Long duration test, run manually & very infrequently.";
    static GeneratorTests() => TestUtil.InitializeLegality();

    public static IEnumerable<object[]> GetSpecies17() => GetSpecies(PersonalTable.USUM, new SimpleTrainerInfo(GameVersion.US), () => new PK7(), []);
    public static IEnumerable<object[]> GetSpeciesLGPE() => GetSpecies(PersonalTable.GG, new SimpleTrainerInfo(GameVersion.GP), () => new PB7(), [GameVersion.GP, GameVersion.GE]);
    public static IEnumerable<object[]> GetSpeciesSWSH() => GetSpecies(PersonalTable.SWSH, new SimpleTrainerInfo(GameVersion.SW), () => new PK8(), [GameVersion.SW, GameVersion.SH]);
    public static IEnumerable<object[]> GetSpeciesPLA() => GetSpecies(PersonalTable.LA, new SimpleTrainerInfo(GameVersion.PLA), () => new PA8(), [GameVersion.PLA]);
    public static IEnumerable<object[]> GetSpeciesBDSP() => GetSpecies(PersonalTable.BDSP, new SimpleTrainerInfo(GameVersion.BD), () => new PB8(), [GameVersion.BD, GameVersion.SP]);
    public static IEnumerable<object[]> GetSpeciesSV() => GetSpecies(PersonalTable.SV, new SimpleTrainerInfo(GameVersion.SL), () => new PK9(), [GameVersion.SL, GameVersion.VL]);

    private static IEnumerable<object[]> GetSpecies<T>(T table, SimpleTrainerInfo tr, Func<Core.PKM> ctor, GameVersion[] games) where T : IPersonalTable
    {
        for (ushort i = 1; i <= table.MaxSpeciesID; i++)
        {
            if (table.IsSpeciesInGame(i))
                yield return new object[] { (Species)i, tr, ctor(), games };
        }
    }

    [Theory(Skip = SkipReasonLong)]
    [MemberData(nameof(GetSpecies17))]
    [MemberData(nameof(GetSpeciesLGPE))]
    [MemberData(nameof(GetSpeciesSWSH))]
    [MemberData(nameof(GetSpeciesPLA))]
    [MemberData(nameof(GetSpeciesBDSP))]
    [MemberData(nameof(GetSpeciesSV))]
    public void PokemonGenerationReturnsLegalPokemon(Species species, SimpleTrainerInfo tr, Core.PKM template, GameVersion[] games)
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
