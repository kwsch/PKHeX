using FluentAssertions;
using PKHeX.Core;
using PKHeX.Web.Services;
using PKHeX.Web.State;
using Xunit;

namespace PKHeX.Web.Tests;

/// <summary>
/// The read-only inspector reads every value through a typed Core member, names it with Core's strings, and labels what Core cannot
/// name instead of guessing (WEB-PKM-001).
/// </summary>
[Trait(TestCategory.Name, TestCategory.Unit)]
public sealed class EntityInspectionTests
{
    private static readonly SaveCapabilities Capabilities = SaveFixtures.Open(SaveFixtures.Synthetic(true)).Capabilities;

    /// <summary>The known legal Zigzagoon the synthetic saves store.</summary>
    private static PK6 Known() => new(SaveFixtures.ReadEntity(true));

    private static EntityInspection Inspect(PK6 pk, SlotRef? slot = null) => EntityInspection.From(pk, slot ?? SaveFixtures.FirstBoxSlot, Capabilities);

    [Fact]
    public void IdentityIsReadFromTheEntity()
    {
        var pk = Known();
        var strings = GameInfo.Strings;

        var identity = Inspect(pk).Identity;

        identity.Species.Should().Be(new NamedValue((int)Species.Zigzagoon, "Zigzagoon"));
        identity.Form.Value.Should().Be(0);
        identity.HasForms.Should().BeFalse("Zigzagoon has no alternate form in Generation 6");
        identity.Nickname.Should().Be(pk.Nickname);
        identity.IsNicknamed.Should().Be(pk.IsNicknamed);
        identity.IsEgg.Should().Be(pk.IsEgg);
        identity.Gender.Should().Be(pk.Gender);
        identity.IsShiny.Should().Be(pk.IsShiny);
        identity.Language.Value.Should().Be(pk.Language);
        identity.Language.Name.Should().NotBeNull();
        identity.Level.Should().Be(pk.CurrentLevel);
        identity.Experience.Should().Be(pk.EXP);
        identity.Nature.Should().Be(new NamedValue((int)pk.Nature, strings.natures[(int)pk.Nature]));
        identity.Ability.Should().Be(new NamedValue(pk.Ability, strings.abilitylist[pk.Ability]));
        identity.AbilitySlot.Should().Be(pk.AbilityNumber);
        identity.HeldItem.Value.Should().Be(pk.HeldItem);
        identity.Friendship.Should().Be(pk.CurrentFriendship);
    }

    [Fact]
    public void BoxStatsAreCalculatedByCoreWithoutChangingTheEntity()
    {
        var pk = Known();
        var before = pk.Data.ToArray();
        var core = pk.GetStats(pk.PersonalInfo);

        var stats = Inspect(pk).Stats;

        stats.Source.Should().Be(StatsSource.Calculated);
        stats.Stats.Select(s => s.Value).Should().Equal(core[0], core[1], core[2], core[4], core[5], core[3]);
        stats.Stats.Select(s => s.Iv).Should().Equal(pk.IV_HP, pk.IV_ATK, pk.IV_DEF, pk.IV_SPA, pk.IV_SPD, pk.IV_SPE);
        stats.Stats.Select(s => s.Ev).Should().Equal(pk.EV_HP, pk.EV_ATK, pk.EV_DEF, pk.EV_SPA, pk.EV_SPD, pk.EV_SPE);
        var personal = pk.PersonalInfo;
        stats.Stats.Select(s => s.Base).Should().Equal(personal.HP, personal.ATK, personal.DEF, personal.SPA, personal.SPD, personal.SPE);
        stats.IvTotal.Should().Be(pk.IVTotal);
        stats.EvTotal.Should().Be(pk.EVTotal);
        stats.CurrentHp.Should().BeNull("a boxed Pokémon stores no HP");
        stats.Status.Should().BeNull();
        pk.Data.ToArray().Should().Equal(before, "inspecting must not write calculated stats to the entity");
    }

    [Fact]
    public void PartyStatsHpAndStatusAreShownAsStored()
    {
        var pk = Known();
        pk.ResetPartyStats();
        pk.Stat_ATK = 999;
        pk.Stat_HPCurrent = 1;
        pk.Status_Condition = (int)StatusType.Burn;

        var stats = Inspect(pk, SlotRef.InParty(0)).Stats;

        stats.Source.Should().Be(StatsSource.Stored);
        stats.Stats[1].Value.Should().Be(999, "stored stats are shown even when they differ from a recalculation");
        stats.Stats[0].Value.Should().Be(pk.Stat_HPMax);
        stats.CurrentHp.Should().Be(1);
        stats.Status.Should().Be(new NamedValue((int)StatusType.Burn, "Burn"));
    }

    [Theory]
    [InlineData(0, "None")]
    [InlineData(1, "Paralysis")]
    [InlineData(2, "Sleep")]
    [InlineData(3, "Freeze")]
    [InlineData(4, "Burn")]
    [InlineData(5, "Poison")]
    [InlineData(0x104, "Burn")]
    [InlineData(16, null)]
    public void PartyStatusIsReadAsGenerationFiveAndLaterStore(int stored, string? name)
    {
        // PK6 stores a StatusType value in the low byte, as the desktop's status view reads it, not the Generation 1-4 flags.
        var pk = Known();
        pk.ResetPartyStats();
        pk.Status_Condition = stored;

        Inspect(pk, SlotRef.InParty(0)).Stats.Status.Should().Be(new NamedValue(stored & 0xFF, name));
    }

    [Fact]
    public void APartyMemberWithoutStoredStatsShowsNoHpAgainstCalculatedStats()
    {
        var pk = Known();
        pk.Stat_HPMax = 0;

        var stats = Inspect(pk, SlotRef.InParty(0)).Stats;

        stats.Source.Should().Be(StatsSource.Calculated);
        stats.CurrentHp.Should().BeNull("a stored HP cannot be read against a calculated maximum");
        stats.Status.Should().NotBeNull();
    }

    [Fact]
    public void CharacteristicAndHiddenPowerComeFromCore()
    {
        var pk = Known();
        var strings = GameInfo.Strings;

        var stats = Inspect(pk).Stats;

        stats.Characteristic.Should().Be(new NamedValue(pk.Characteristic, strings.characteristics[pk.Characteristic]));
        stats.HiddenPowerType.Should().Be(new NamedValue(pk.HPType, strings.HiddenPowerTypes[pk.HPType]));
    }

    [Fact]
    public void MovesShowPpWithCoresMaximum()
    {
        var pk = Known();
        pk.Move4 = 0;
        pk.Move1_PPUps = 3;

        var moves = Inspect(pk).Moves;

        moves.Moves[0].Move.Should().Be(new NamedValue(pk.Move1, GameInfo.Strings.movelist[pk.Move1]));
        moves.Moves[0].PpUps.Should().Be(3);
        moves.Moves[0].MaxPp.Should().Be(pk.GetMovePP(pk.Move1, 3));
        moves.Moves[0].Pp.Should().Be(pk.Move1_PP);
        moves.Moves[3].Move.Value.Should().Be(0);
        moves.Moves[3].MaxPp.Should().Be(0, "an empty slot has no PP");
        moves.Relearn.Select(m => m.Value).Should().Equal(pk.RelearnMove1, pk.RelearnMove2, pk.RelearnMove3, pk.RelearnMove4);
    }

    [Fact]
    public void OriginIsReadFromTheEntity()
    {
        var pk = Known();

        var origin = Inspect(pk).Origin;

        origin.TrainerName.Should().Be(pk.OriginalTrainerName);
        origin.TrainerGender.Should().Be(pk.OriginalTrainerGender);
        origin.DisplayTid.Should().Be(pk.DisplayTID);
        origin.DisplaySid.Should().Be(pk.DisplaySID);
        origin.IdFormat.Should().Be(TrainerIDFormat.SixteenBit);
        origin.OriginGame.Value.Should().Be((int)pk.Version);
        origin.OriginGame.Name.Should().Be(GameInfo.GetVersionName(pk.Version));
        origin.MetLocation.Should().Be(new NamedValue(pk.MetLocation, GameInfo.GetLocationName(false, pk.MetLocation, 6, 6, pk.Version)));
        origin.MetLevel.Should().Be(pk.MetLevel);
        origin.MetDate.Should().Be(pk.MetDate);
        origin.Ball.Value.Should().Be(pk.Ball);
        origin.Ball.Name.Should().NotBeNull();
        origin.EggLocation.Should().BeNull("the fixture was not hatched");
        origin.FatefulEncounter.Should().Be(pk.FatefulEncounter);
        origin.TrainerFriendship.Should().Be(pk.OriginalTrainerFriendship);
        origin.CurrentHandler.Should().Be(pk.CurrentHandler);
    }

    [Fact]
    public void HatchedPokemonShowWhereTheEggWasReceived()
    {
        var pk = Known();
        pk.EggLocation = Locations.LinkTrade6;
        pk.EggMetDate = new DateOnly(2015, 3, 4);

        var origin = Inspect(pk).Origin;

        origin.EggLocation.Should().Be(new NamedValue(Locations.LinkTrade6, GameInfo.GetLocationName(true, Locations.LinkTrade6, 6, 6, pk.Version)));
        origin.EggLocation!.Value.Name.Should().NotBeNull();
        origin.EggDate.Should().Be(new DateOnly(2015, 3, 4));
    }

    [Fact]
    public void AdvancedValuesAreReadThroughTypedMembers()
    {
        var pk = Known();
        var ribbons = pk.RibbonCount;
        pk.RibbonChampionKalos = true;
        pk.RibbonBestFriends = true;
        pk.SetMarking(1, true);
        pk.PokerusStrain = 2;
        pk.PokerusDays = 0;

        var advanced = Inspect(pk).Advanced;

        advanced.Pid.Should().Be(pk.PID);
        advanced.EncryptionConstant.Should().Be(pk.EncryptionConstant);
        advanced.RibbonCount.Should().BeGreaterThanOrEqualTo(ribbons).And.Be(pk.RibbonCount);
        advanced.Markings.Should().Equal(false, true, false, false, false, false);
        (advanced.PokerusStrain, advanced.PokerusDays).Should().Be((2, 0));
        advanced.Format.Should().Be("PK6");
    }

    [Fact]
    public void ValuesCoreCannotNameAreKeptRawAndValuesOutsideTheGameAreMarked()
    {
        var pk = Known();
        pk.HeldItem = 9999;
        pk.Nature = (Nature)30;
        pk.Form = 5;
        pk.MetLocation = 65000;
        pk.Move2 = (ushort)(Capabilities.Lists.Moves.Max(m => m.Value) + 1);

        var inspection = Inspect(pk);

        inspection.Identity.HeldItem.Should().Be(new NamedValue(9999, null, OutsideGameList: true));
        inspection.Identity.Nature.Should().Be(new NamedValue(30, null));
        inspection.Identity.Form.Should().Be(new NamedValue(5, null));
        inspection.Origin.MetLocation.Should().Be(new NamedValue(65000, null));
        var move = inspection.Moves.Moves[1].Move;
        move.Name.Should().Be(GameInfo.Strings.movelist[pk.Move2], "Core still names a later generation's move");
        move.OutsideGameList.Should().BeTrue("Generation 6 cannot hold it");
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void OutOfRangeStoredValuesAreInspectedWithoutFailing(bool party)
    {
        // A checksum-valid entity can hold any value in any field; the inspector must still render it, never fault the page.
        var pk = Known();
        pk.Species = 2000;
        pk.Form = 200;
        pk.Move1 = ushort.MaxValue;
        pk.Move1_PPUps = 3;
        pk.RelearnMove1 = ushort.MaxValue;
        pk.HeldItem = ushort.MaxValue;
        pk.Ability = ushort.MaxValue;
        pk.AbilityNumber = 7;
        pk.Ball = byte.MaxValue;
        pk.Version = (GameVersion)byte.MaxValue;
        pk.Language = byte.MaxValue;
        pk.MetLocation = ushort.MaxValue;
        pk.EggLocation = ushort.MaxValue;
        pk.OriginalTrainerGender = 1;
        pk.Gender = 3;
        pk.CurrentHandler = 9;
        pk.Status_Condition = byte.MaxValue;
        pk.RefreshChecksum();

        var inspection = Inspect(pk, party ? SlotRef.InParty(0) : null);
        var sections = Components.InspectorText.Sections(inspection);

        inspection.Identity.Species.Should().Be(new NamedValue(2000, null, OutsideGameList: true));
        inspection.Moves.Moves[0].Move.Name.Should().BeNull();
        sections.SelectMany(s => s.Rows).Should().OnlyContain(r => r.Value.Length > 0);
    }

    [Fact]
    public void ListedValuesAreNotMarked()
    {
        var inspection = Inspect(Known());

        inspection.Identity.Species.OutsideGameList.Should().BeFalse();
        inspection.Identity.Ability.OutsideGameList.Should().BeFalse();
        inspection.Origin.Ball.OutsideGameList.Should().BeFalse();
        inspection.Origin.OriginGame.OutsideGameList.Should().BeFalse();
        inspection.Moves.Moves.Where(m => m.Move.Value != 0).Should().OnlyContain(m => !m.Move.OutsideGameList);
    }
}
