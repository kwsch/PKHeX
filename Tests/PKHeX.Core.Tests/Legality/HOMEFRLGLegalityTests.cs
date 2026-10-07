using System.Linq;
using FluentAssertions;
using Xunit;

namespace PKHeX.Core.Tests.Legality;

public class HOMEFRLGLegalityTests
{
    private static PK9 CreatePrimeape() // Independent fixture with a reproducible Method 1 PID/IV spread.
    {
        var pk = new PK9
        {
            Species = 57, Version = GameVersion.LG_NX, MetLocation = LocationsHOME.Transfer3HOME,
            MetLevel = 42, Language = 2, Ball = (byte)Ball.Repeat, Nature = Nature.Hardy,
            PID = 0x25979DFF, EncryptionConstant = 0x25979DFF, ID32 = 0x12345678,
            OriginalTrainerName = "Test", OriginalTrainerGender = 1, Gender = 0,
            CurrentHandler = 1, HandlingTrainerName = "Other", HandlingTrainerFriendship = 50,
            Tracker = 1, TeraTypeOriginal = MoveType.Fighting, TeraTypeOverride = MoveType.Fighting,
            ObedienceLevel = 42, MetDate = new System.DateOnly(2026, 10, 7), HeightScalar = 128, WeightScalar = 128, Scale = 128,
        };
        pk.CurrentLevel = 42;
        pk.SetIVs([17, 29, 26, 6, 6, 1]);
        pk.RefreshAbility(0);
        pk.Nickname = SpeciesName.GetSpeciesNameGeneration(pk.Species, pk.Language, 9);
        EncounterUtil.SetEncounterMoves(pk, GameVersion.SL, pk.CurrentLevel);
        pk.RefreshChecksum();
        return pk;
    }

    [Theory]
    [InlineData(GameVersion.FR_NX)]
    [InlineData(GameVersion.LG_NX)]
    public void RecognizesNewStoredOrigins(GameVersion version)
    {
        var pk = CreatePrimeape();
        pk.Version = version;
        version.IsValidSavedVersion().Should().BeTrue();
        pk.Generation.Should().Be(8);
        var analysis = new LegalityAnalysis(pk);
        analysis.Valid.Should().BeTrue(analysis.Report(true));
    }

    [Theory]
    [InlineData(151)] // Mew: unavailable event.
    [InlineData(152)] // Chikorita: R/S/E/Colosseum dependency.
    [InlineData(251)] // Celebi HOME reward is not a captured FR/LG Pokémon.
    [InlineData(252)] // Treecko.
    [InlineData(385)] // Jirachi.
    [InlineData(1000)] // Gholdengo has no FR/LG ancestor.
    public void RejectsUnobtainableSpecies(ushort species)
    {
        var pk = CreatePrimeape();
        pk.Species = species;
        pk.CurrentLevel = 80;
        pk.MetLevel = 80;
        var info = new LegalInfo(pk, []);
        EncounterGenerator.GetEncounters(pk, info).Should().BeEmpty();
        new LegalityAnalysis(pk).Valid.Should().BeFalse();
    }

    [Theory]
    [InlineData(196, 0)] // Eevee -> Espeon in a later Switch game.
    [InlineData(197, 0)] // Eevee -> Umbreon in a later Switch game.
    [InlineData(979, 0)] // Primeape -> Annihilape.
    [InlineData(900, 0)] // Scyther -> Kleavor.
    [InlineData(461, 0)] // Sneasel -> Weavile.
    [InlineData(469, 0)] // Yanma -> Yanmega.
    [InlineData(863, 0, false)] // Kanto Meowth cannot evolve into Perrserker.
    [InlineData(903, 0, false)] // Kanto Sneasel cannot evolve into Sneasler.
    [InlineData(26, 1, false)] // Alolan Raichu requires Gen7/LGPE.
    public void RestrictsLaterEvolutionsToPossibleSwitchRoutes(ushort species, byte form, bool expected = true)
    {
        var pk = CreatePrimeape();
        pk.Species = species;
        pk.Form = form;
        pk.Ball = (byte)Ball.Poke;
        pk.MetLevel = 5;
        pk.CurrentLevel = 80;
        pk.Gender = (byte)EntityGender.GetFromPID(species, pk.EncryptionConstant);
        pk.RefreshAbility(0);
        var info = new LegalInfo(pk, []);
        var matches = EncounterGenerator.GetEncounters(pk, info).ToArray();
        matches.Any().Should().Be(expected);
        if (expected)
        {
            info.EncounterMatch = matches[0];
            var history = info.EvoChainsAllGens;
            history.HasVisitedGen4.Should().BeFalse();
            history.HasVisitedGen5.Should().BeFalse();
            history.HasVisitedGen6.Should().BeFalse();
            history.HasVisitedGen7.Should().BeFalse();
            history.HasVisitedLGPE.Should().BeFalse();
        }
    }

    [Theory]
    [InlineData(0)] // Lost met location.
    [InlineData(30018)] // Wrong HOME transfer location.
    [InlineData(59994)] // SWSH remap in a PK9 does not establish a FR/LG origin.
    public void RejectsWrongTransferLocations(ushort location)
    {
        var pk = CreatePrimeape();
        pk.MetLocation = location;
        new LegalityAnalysis(pk).Valid.Should().BeFalse();
    }

    [Theory]
    [InlineData("move")]
    [InlineData("ability")]
    [InlineData("ball")]
    [InlineData("nature")]
    [InlineData("gender")]
    [InlineData("ec")]
    [InlineData("language")]
    [InlineData("egg")]
    [InlineData("origin")]
    [InlineData("fateful")]
    [InlineData("level")]
    public void RetainsOtherLegalityChecks(string change)
    {
        var pk = CreatePrimeape();
        switch (change)
        {
            case "move": pk.Move1 = (ushort)Move.SpacialRend; break;
            case "ability": pk.Ability = (int)Ability.WonderGuard; break;
            case "ball": pk.Ball = (byte)Ball.Beast; break;
            case "nature": pk.Nature = Nature.Lonely; break;
            case "gender": pk.Gender = 1; break;
            case "ec": pk.EncryptionConstant++; break;
            case "language": pk.Language = 8; break;
            case "egg": pk.IsEgg = true; break;
            case "origin": pk.Version = 0; break;
            case "fateful": pk.FatefulEncounter = true; break;
            case "level": pk.CurrentLevel = 41; break;
        }
        new LegalityAnalysis(pk).Valid.Should().BeFalse();
    }

    [Theory]
    [InlineData("pk8", GameVersion.SW, LocationsHOME.SWFR)]
    [InlineData("pk8", GameVersion.SH, LocationsHOME.SHLG)]
    [InlineData("pa8", GameVersion.LG_NX, LocationsHOME.Transfer3HOME)]
    [InlineData("pb8", GameVersion.LG_NX, LocationsHOME.Transfer3HOME)]
    [InlineData("pk9", GameVersion.LG_NX, LocationsHOME.Transfer3HOME)]
    [InlineData("pa9", GameVersion.LG_NX, LocationsHOME.Transfer3HOME)]
    public void MatchesTransfersAcrossSupportedFormats(string format, GameVersion version, ushort location)
    {
        PKM pk = format switch
        {
            "pk8" => new PK8(), "pa8" => new PA8(), "pb8" => new PB8(),
            "pk9" => new PK9(), _ => new PA9(),
        };
        pk.Version = version;
        pk.MetLocation = location;
        pk.EggLocation = pk is PB8 ? Locations.Default8bNone : (ushort)0;
        pk.Species = 25;
        pk.CurrentLevel = 30;
        pk.MetLevel = 30;
        pk.PID = pk.EncryptionConstant = 0x12345678;
        pk.Ball = (byte)Ball.Poke;
        pk.Language = 2;
        ((IHomeTrack)pk).Tracker = 1;
        var info = new LegalInfo(pk, []);
        EncounterGenerator.GetEncounters(pk, info).Should().NotBeEmpty();
    }

    [Theory]
    [InlineData(249, 70)] // Lugia.
    [InlineData(250, 70)] // Ho-Oh.
    [InlineData(386, 30)] // Deoxys.
    public void AdmitsIncludedEventTicketsButStillRequiresTheirEncounterData(ushort species, byte level)
    {
        var pk = CreatePrimeape();
        pk.Species = species;
        pk.CurrentLevel = level;
        pk.MetLevel = level;
        pk.FatefulEncounter = true;
        pk.Language = 1; // Switch tickets also exist for Japanese games.
        var info = new LegalInfo(pk, []);
        var matches = EncounterGenerator.GetEncounters(pk, info).Cast<EncounterTransfer3HOME>().ToArray();
        matches.Should().NotBeEmpty();
        matches.Should().OnlyContain(z => z.Original is EncounterStatic3 && z.FatefulEncounter);
        pk.MetLevel = (byte)(level - 1);
        EncounterGenerator.GetEncounters(pk, info).Should().BeEmpty();
    }

    [Fact]
    public void RetainsUnownFormPIDCorrelation()
    {
        var tr = new SimpleTrainerInfo(GameVersion.LG) { Language = 2, OT = "Test", TID16 = 1234, SID16 = 5678 };
        var template = new PK3 { Species = 201, Form = 0, CurrentLevel = 100 };
        var chain = EncounterOrigin.GetOriginChain(template, 3, EntityContext.Gen3);
        var enc = EncounterGenerator3.Instance.GetPossible(template, chain, GameVersion.LG, EncounterTypeGroup.Slot)
            .OfType<EncounterSlot3>().First(z => z.Form == 0);
        var native = enc.ConvertToPKM(tr);
        var pk = new PB8
        {
            Version = GameVersion.LG_NX, Species = 201, Form = 0,
            PID = PK5.GetTransferPID(native.PID, native.ID32, out _), EncryptionConstant = native.PID,
            ID32 = native.ID32, Nature = native.Nature, StatAlignment = native.Nature, Gender = 2, Language = 2,
            OriginalTrainerName = "Test", CurrentHandler = 1, HandlingTrainerName = "Other",
            MetLocation = LocationsHOME.Transfer3HOME, EggLocation = Locations.Default8bNone,
            MetLevel = native.CurrentLevel, Ball = native.Ball, Tracker = 1,
            MetDate = new System.DateOnly(2026, 10, 7),
        };
        pk.SetIVs(native.GetIVs());
        pk.CurrentLevel = pk.MetLevel;
        pk.RefreshAbility(0);
        pk.Nickname = SpeciesName.GetSpeciesNameGeneration(pk.Species, 2, 8);
        EncounterUtil.SetEncounterMoves(pk, GameVersion.BD, pk.CurrentLevel);
        var valid = new LegalityAnalysis(pk);
        valid.Valid.Should().BeTrue(valid.Report(true));
        pk.Form = 1;
        new LegalityAnalysis(pk).Valid.Should().BeFalse();
    }

    [Fact]
    public void DoesNotAcceptTransferEggLocationsInBDSP()
    {
        var pk = new PB8
        {
            Version = GameVersion.LG_NX, Species = 25, MetLocation = LocationsHOME.Transfer3HOME,
            MetLevel = 30, EggLocation = 0, PID = 0x12345678, EncryptionConstant = 0x12345678,
        };
        pk.CurrentLevel = 30;
        EncounterGenerator.GetEncounters(pk, new LegalInfo(pk, [])).Should().BeEmpty();
    }


    [Theory]
    [InlineData(GameVersion.FR_NX, LocationsHOME.SWFR, GameVersion.SW)]
    [InlineData(GameVersion.LG_NX, LocationsHOME.SHLG, GameVersion.SH)]
    public void MapsBothFRLGOriginsToSWSH(GameVersion version, ushort location, GameVersion swsh)
    {
        LocationsHOME.IsVersionRemapNeeded(version).Should().BeTrue();
        LocationsHOME.GetMetSWSH(LocationsHOME.Transfer3HOME, version).Should().Be(location);
        LocationsHOME.GetVersionSWSH(version).Should().Be(swsh);
        LocationsHOME.GetVersionSWSHOriginal(location).Should().Be(version);
    }
}
