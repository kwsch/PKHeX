using FluentAssertions;
using PKHeX.Core;
using PKHeX.Web.Services;
using PKHeX.Web.State;
using Xunit;

namespace PKHeX.Web.Tests;

/// <summary>
/// Applying party members (WEB-SESSION-002, WEB-PKM-014): a non-stat edit keeps the stored battle stats, HP and status byte for byte,
/// and a stat-affecting edit follows the PK6 party-stat policy through apply and export.
/// </summary>
[Trait(TestCategory.Name, TestCategory.Unit)]
public sealed class PartyApplyTests
{
    private const int Burn = 0x10;

    /// <summary>A party member at 7 HP with a burn, so a heal or a cleared status would show.</summary>
    private static byte[] InjuredAndBurned(bool oras) => SaveFixtures.Synthetic(oras, customize: SaveFixtures.WithPartyMember("Leader", p =>
    {
        p.Stat_HPCurrent = 7;
        p.Status_Condition = Burn;
        p.Move1_PP = 1;
    }));

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void ANicknameEditKeepsEveryBattleByteAndMatchesNativeCore(bool oras)
    {
        var source = InjuredAndBurned(oras);
        var session = SaveFixtures.Open(source);
        var stored = session.Working.GetPartySlotAtIndex(0);
        var draft = session.Select(SlotRef.InParty(0));

        draft.EditNickname("Renamed", true);
        draft.HpChange.Should().BeNull("a nickname does not affect stats");
        session.Apply(draft);

        session.Revision.Should().Be(1);
        session.HasChangesSinceOpen.Should().BeTrue();
        var applied = session.Working.GetPartySlotAtIndex(0);
        applied.Nickname.Should().Be("Renamed");
        applied.Stat_HPCurrent.Should().Be(7);
        applied.Status_Condition.Should().Be(Burn);
        applied.Move1_PP.Should().Be(1);
        applied.Data[stored.SIZE_STORED..].ToArray().Should().Equal(stored.Data[stored.SIZE_STORED..].ToArray(), "the party-only bytes are kept as stored");
        session.Working.PartyCount.Should().Be(1);

        var output = SaveExporter.Export(session, session.Select(SlotRef.InParty(0)));
        var native = SaveFixtures.Parse(source);
        var pk = native.GetPartySlotAtIndex(0);
        pk.Nickname = "Renamed";
        pk.IsNicknamed = true;
        native.SetPartySlotAtIndex(pk, 0, EntityImportSettings.None);
        output.Should().Equal(native.Write().ToArray(), "the session must match the native Core edit");
    }

    [Fact]
    public void AStatEditFollowsThePolicyThroughApplyAndExport()
    {
        var session = SaveFixtures.Open(InjuredAndBurned(true));
        var draft = session.Select(SlotRef.InParty(0));

        draft.EditForTest(p => p.CurrentLevel = 2, affectsStats: true);

        var preview = draft.Preview();
        preview.Stat_Level.Should().Be(2);
        preview.Stat_HPCurrent.Should().Be(Math.Min(7, preview.Stat_HPMax));
        preview.Status_Condition.Should().Be(Burn);
        preview.Move1_PP.Should().Be(1, "an edit never refills PP");
        draft.HpChange.Should().NotBeNull();

        session.Apply(draft);
        var output = SaveExporter.Export(session, session.Select(SlotRef.InParty(0)));
        var reopened = SaveFixtures.Parse(output).GetPartySlotAtIndex(0);
        reopened.Stat_Level.Should().Be(2);
        reopened.Stat_HPMax.Should().Be(preview.Stat_HPMax);
        reopened.Stat_HPCurrent.Should().Be(preview.Stat_HPCurrent);
        reopened.Status_Condition.Should().Be(Burn);
        new[] { reopened.Stat_ATK, reopened.Stat_DEF, reopened.Stat_SPE, reopened.Stat_SPA, reopened.Stat_SPD }
            .Should().Equal(reopened.GetStats(reopened.PersonalInfo)[1..].Select(s => (int)s));
    }

    [Fact]
    public void AStatEditThatLowersHpIsPreviewed()
    {
        var session = SaveFixtures.Open(SaveFixtures.Synthetic(true, customize: SaveFixtures.WithPartyMember()));
        var draft = session.Select(SlotRef.InParty(0));
        var stored = draft.Preview();

        draft.EditForTest(p => p.CurrentLevel = 2, affectsStats: true);

        var change = draft.HpChange!.Value;
        change.IsReduction.Should().BeTrue();
        change.PreviousHp.Should().Be(stored.Stat_HPCurrent);
        change.PreviousMax.Should().Be(stored.Stat_HPMax);
        change.NewHp.Should().Be(change.NewMax, "a healthy member is clamped to the new maximum");
    }

    [Fact]
    public void AFaintedMemberStaysFaintedAfterAStatEdit()
    {
        var session = SaveFixtures.Open(SaveFixtures.Synthetic(true, customize: SaveFixtures.WithPartyMember(battle: p => p.Stat_HPCurrent = 0)));
        var draft = session.Select(SlotRef.InParty(0));

        draft.EditForTest(p => p.CurrentLevel = 100, affectsStats: true);
        session.Apply(draft);

        session.Working.GetPartySlotAtIndex(0).Stat_HPCurrent.Should().Be(0);
    }

    [Fact]
    public void AnEditThatDoesNotAffectStatsKeepsStoredStats()
    {
        var session = SaveFixtures.Open(SaveFixtures.Synthetic(true, customize: SaveFixtures.WithPartyMember()));
        var draft = session.Select(SlotRef.InParty(0));
        var stored = draft.Preview();

        draft.EditForTest(p => p.Ball = (byte)Ball.Great, affectsStats: false);

        draft.Preview().Data[stored.SIZE_STORED..].ToArray().Should().Equal(stored.Data[stored.SIZE_STORED..].ToArray());
        draft.HpChange.Should().BeNull();
    }

    [Fact]
    public void ABoxedEntityHasNoBattleStateToRecalculate()
    {
        var session = SaveFixtures.Open(SaveFixtures.Synthetic(true));
        var draft = session.Select(SaveFixtures.FirstBoxSlot);

        draft.EditForTest(p => p.CurrentLevel = 2, affectsStats: true);

        draft.HpChange.Should().BeNull();
        draft.Preview().PartyStatsPresent.Should().BeFalse("stats of a boxed entity are calculated for display, never stored");
        session.Apply(draft);
        session.Working.GetBoxSlotAtIndex(0).CurrentLevel.Should().Be(2);
    }

    [Fact]
    public void AMemberOfALaterPositionIsWrittenInPlace()
    {
        var session = SaveFixtures.Open(SaveFixtures.Synthetic(false, customize: SaveFixtures.All(
            SaveFixtures.WithPartyMember("First"), SaveFixtures.WithPartyMember("Second", position: 1))));
        var first = session.Working.GetPartySlotAtIndex(0).Data.ToArray();
        var draft = session.Select(SlotRef.InParty(1));

        draft.EditNickname("Changed", true);
        session.Apply(draft);

        session.Working.PartyCount.Should().Be(2);
        session.Working.GetPartySlotAtIndex(1).Nickname.Should().Be("Changed");
        session.Working.GetPartySlotAtIndex(0).Data.ToArray().Should().Equal(first);
    }

    /// <summary>A party member stored without battle stats, written directly: Core's party setter would recalculate them.</summary>
    private static byte[] WithoutStoredStats() => SaveFixtures.Synthetic(false, customize: SaveFixtures.All(SaveFixtures.WithPartyMember(), save =>
    {
        var pk = save.GetPartySlotAtIndex(0);
        pk.Stat_HPMax = 0;
        pk.Stat_HPCurrent = 0;
        pk.WriteEncryptedDataParty(save.Data[save.GetPartyOffset(0)..]);
    }));

    [Fact]
    public void AStatEditOfAMemberWithoutStoredStatsIsRefused()
    {
        // The policy would recalculate from no current HP and leave the member fainted, and the draft would then carry stats.
        var session = SaveFixtures.Open(WithoutStoredStats());
        var draft = session.Select(SlotRef.InParty(0));
        var before = draft.Preview().Data.ToArray();

        var edit = () => draft.EditForTest(p => p.CurrentLevel = 50, affectsStats: true);

        edit.Should().Throw<SessionException>().Which.Error.Should().Be(SessionError.PartyStatsMissing);
        draft.Preview().Data.ToArray().Should().Equal(before);
        draft.EditRevision.Should().Be(0);
        draft.IsDirty.Should().BeFalse();
    }

    [Fact]
    public void AMemberWithoutStoredStatsIsNotWritten()
    {
        var session = SaveFixtures.Open(WithoutStoredStats());
        var draft = session.Select(SlotRef.InParty(0));
        draft.EditNickname("Changed", true);

        var apply = () => session.Apply(draft);

        apply.Should().Throw<SessionException>().Which.Error.Should().Be(SessionError.PartyStatsMissing);
        session.Revision.Should().Be(0);
    }

    [Fact]
    public void ALoneEggIsNotWrittenToTheParty()
    {
        var session = SaveFixtures.Open(SaveFixtures.Synthetic(false, customize: SaveFixtures.WithPartyMember(battle: p => p.IsEgg = true)));
        var draft = session.Select(SlotRef.InParty(0));
        draft.EditNickname("Egg", false);

        var apply = () => session.Apply(draft);

        apply.Should().Throw<SessionException>().Which.Error.Should().Be(SessionError.SlotNotWritable, "Core refuses a party of only eggs");
        session.Revision.Should().Be(0);
    }

    [Fact]
    public void AnEggBesideAnotherMemberIsWritten()
    {
        var session = SaveFixtures.Open(SaveFixtures.Synthetic(false, customize: SaveFixtures.All(
            SaveFixtures.WithPartyMember(),
            SaveFixtures.WithPartyMember(battle: p => p.IsEgg = true, position: 1))));
        var draft = session.Select(SlotRef.InParty(1));
        draft.EditNickname("Egg", false);

        session.Apply(draft);

        session.Working.GetPartySlotAtIndex(1).Nickname.Should().Be("Egg");
    }
}
