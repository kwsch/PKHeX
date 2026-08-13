using System;
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

    public static TheoryData<SpeciesTestRow> Species17 => GetSpecies(PT.USUM, new(US), () => new PK7(), []);
    public static TheoryData<SpeciesTestRow> SpeciesLGPE => GetSpecies(PT.GG, new(GP), () => new PB7(), [GP, GE]);
    public static TheoryData<SpeciesTestRow> SpeciesSWSH => GetSpecies(PT.SWSH, new(SW), () => new PK8(), [SW, SH]);
    public static TheoryData<SpeciesTestRow> SpeciesPLA => GetSpecies(PT.LA, new(PLA), () => new PA8(), [PLA]);
    public static TheoryData<SpeciesTestRow> SpeciesBDSP => GetSpecies(PT.BDSP, new(BD), () => new PB8(), [BD, SP]);
    public static TheoryData<SpeciesTestRow> SpeciesSV => GetSpecies(PT.SV, new(SL), () => new PK9(), [SL, VL]);

    public readonly record struct SpeciesTestRow(Species Species, TR Trainer, PKM Template, GameVersion[] Games);

    private static TheoryData<SpeciesTestRow> GetSpecies<T>(T table, TR tr, Func<PKM> ctor, GameVersion[] games) where T : IPersonalTable
    {
        var data = new TheoryData<SpeciesTestRow>();
        for (ushort i = 1; i <= table.MaxSpeciesID; i++)
        {
            if (table.IsSpeciesInGame(i))
                data.Add(new SpeciesTestRow((Species)i, tr, ctor(), games));
        }
        return data;
    }

    [Theory(Skip = SkipReasonLong)]
    [MemberData(nameof(Species17))]
    [MemberData(nameof(SpeciesLGPE))]
    [MemberData(nameof(SpeciesSWSH))]
    [MemberData(nameof(SpeciesPLA))]
    [MemberData(nameof(SpeciesBDSP))]
    [MemberData(nameof(SpeciesSV))]
    public void PokemonGenerationReturnsLegalPokemon(SpeciesTestRow row)
    {
        var (species, tr, template, games) = row;
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
            pk.ID32.Should().Be(tr.ID32);
        }
    }
}
