using FluentAssertions;
using PKHeX.Core;
using PKHeX.Web.Services;
using PKHeX.Web.State;
using Xunit;

namespace PKHeX.Web.Tests;

/// <summary>
/// Drafting the IVs and EVs (WEB-PKM-013): each value is refused out of Core's range rather than clamped, an EV edit may not raise the
/// total above Core's limit, only the edited stat's stored bits change, and a party member's stats follow the PK6 party-stat policy.
/// </summary>
[Trait(TestCategory.Name, TestCategory.Unit)]
public sealed class IvEvDraftTests
{
    private const int Burn = 0x10;

    /// <summary>Core's stat index (HP, Attack, Defense, Speed, Sp. Atk, Sp. Def) for each summary index (HP, Attack, Defense, Sp. Atk, Sp. Def, Speed).</summary>
    private static readonly int[] CoreIndex = [0, 1, 2, 4, 5, 3];

    /// <summary>The known legal Zigzagoon in box 1, slot 1.</summary>
    private static EditorDraft Zigzagoon(bool oras = true) => SaveFixtures.Open(SaveFixtures.Synthetic(oras)).Select(SaveFixtures.FirstBoxSlot);

    /// <summary>A box 1, slot 2 Pokémon with the given EVs (summary order), stored as given even when Core's legality would refuse them.</summary>
    private static EditorDraft WithEvs(params int[] evs)
    {
        var bytes = SaveFixtures.Synthetic(true, customize: SaveFixtures.WithBoxEntity(0, 1, p =>
        {
            for (var i = 0; i < evs.Length; i++)
            {
                p.SetEV(CoreIndex[i], evs[i]);
            }
        }));
        return SaveFixtures.Open(bytes).Select(SlotRef.InBox(0, 1));
    }

    /// <summary>A party member at 7 HP with a burn and a stored Attack Core would not calculate, so a heal, a cleared status or a recalculation would show.</summary>
    private static byte[] InjuredAndBurned(bool oras) => SaveFixtures.Synthetic(oras, customize: SaveFixtures.WithPartyMember("Leader", p =>
    {
        p.Stat_HPCurrent = 7;
        p.Status_Condition = Burn;
        p.Stat_ATK = 1;
    }));

    /// <summary>Asserts the draft holds exactly the stored entity changed by <paramref name="nativeEdit"/>, so the edit wrote nothing else.</summary>
    private static void AssertMatchesNative(EditorDraft draft, PK6 stored, Action<PK6> nativeEdit)
    {
        var expected = (PK6)stored.Clone();
        nativeEdit(expected);
        draft.Preview().Data.ToArray().Should().Equal(expected.Data.ToArray());
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void EachIvEditMatchesNativeCoreForItsStat(bool oras)
    {
        for (var stat = 0; stat < EditorDraft.StatCount; stat++)
        {
            var draft = Zigzagoon(oras);
            var stored = draft.Preview();
            var value = stored.GetIV(CoreIndex[stat]) == 7 ? 8 : 7;

            draft.EditIv(stat, value);

            draft.Ivs[stat].Should().Be(value);
            var index = CoreIndex[stat];
            AssertMatchesNative(draft, stored, p => p.SetIV(index, value));
        }
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void EachEvEditMatchesNativeCoreForItsStat(bool oras)
    {
        for (var stat = 0; stat < EditorDraft.StatCount; stat++)
        {
            var draft = Zigzagoon(oras);
            var stored = draft.Preview();
            var value = stored.GetEV(CoreIndex[stat]) == 100 ? 101 : 100;

            draft.EditEv(stat, value);

            draft.Evs[stat].Should().Be(value);
            var index = CoreIndex[stat];
            AssertMatchesNative(draft, stored, p => p.SetEV(index, value));
        }
    }

    [Fact]
    public void TheReadersFollowTheSummaryOrder()
    {
        var bytes = SaveFixtures.Synthetic(true, customize: SaveFixtures.WithBoxEntity(0, 1, p =>
        {
            p.SetIVs([1, 2, 3, 4, 5, 6]); // Core order: HP, Attack, Defense, Speed, Sp. Atk, Sp. Def
            p.SetEVs([10, 20, 30, 40, 50, 60]);
        }));
        var draft = SaveFixtures.Open(bytes).Select(SlotRef.InBox(0, 1));

        draft.Ivs.Should().Equal(1, 2, 3, 5, 6, 4);
        draft.Evs.Should().Equal(10, 20, 30, 50, 60, 40);
        (draft.IvTotal, draft.EvTotal).Should().Be((21, 210));
        (draft.MaxIv, draft.MaxEv, draft.MaxEvTotal).Should().Be((31, 252, 510));
        draft.Inspect().Stats.Stats.Select(s => s.Iv).Should().Equal(draft.Ivs, "the editor and the inspector use the same order");
    }

    [Fact]
    public void TheMaximumIvIsAcceptedAndKeepsTheFlagsStoredBesideIt()
    {
        var bytes = SaveFixtures.Synthetic(true, customize: SaveFixtures.WithBoxEntity(0, 1, p => p.IsNicknamed = true));
        var draft = SaveFixtures.Open(bytes).Select(SlotRef.InBox(0, 1));

        for (var stat = 0; stat < EditorDraft.StatCount; stat++)
        {
            draft.EditIv(stat, 31);
            draft.EditIv(stat, 0);
        }

        draft.Ivs.Should().AllBeEquivalentTo(0);
        var preview = draft.Preview();
        (preview.IsNicknamed, preview.IsEgg).Should().Be((true, false), "the IVs share their stored word with the egg and nickname flags");
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(32)]
    [InlineData(int.MaxValue)]
    [InlineData(int.MinValue)]
    public void AnIvOutOfRangeIsRefusedNotClamped(int value)
    {
        // Core's PK6 setter would clamp 32 to 31 and write -1 over the neighbouring IVs and the egg and nickname flags.
        var draft = Zigzagoon();
        var before = draft.Preview().Data.ToArray();

        for (var stat = 0; stat < EditorDraft.StatCount; stat++)
        {
            var s = stat;
            var edit = () => draft.EditIv(s, value);
            edit.Should().Throw<SessionException>().Which.Error.Should().Be(SessionError.IvOutOfRange);
        }

        draft.Preview().Data.ToArray().Should().Equal(before);
        draft.EditRevision.Should().Be(0);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(253)]
    [InlineData(255)]
    [InlineData(256)]
    [InlineData(int.MaxValue)]
    public void AnEvOutOfRangeIsRefusedNotClamped(int value)
    {
        // Core's setter would store the low byte, so 256 would become 0.
        var draft = WithEvs(0, 0, 0, 0, 0, 0);

        var edit = () => draft.EditEv(1, value);

        edit.Should().Throw<SessionException>().Which.Error.Should().Be(SessionError.EvOutOfRange);
        draft.IsDirty.Should().BeFalse();
    }

    [Fact]
    public void AStatIndexOutsideTheSixIsAProgrammingError()
    {
        var draft = Zigzagoon();

        Action[] edits = [() => draft.EditIv(-1, 0), () => draft.EditIv(6, 0), () => draft.EditEv(-1, 0), () => draft.EditEv(6, 0)];

        foreach (var edit in edits)
        {
            edit.Should().Throw<ArgumentOutOfRangeException>();
        }
        draft.IsDirty.Should().BeFalse();
    }

    [Fact]
    public void TheEvTotalMayReachTheLimitButNotPassIt()
    {
        var draft = WithEvs(252, 252, 0, 0, 0, 0);

        draft.EditEv(2, 6);
        draft.EvTotal.Should().Be(510);

        var over = () => draft.EditEv(3, 1);

        over.Should().Throw<SessionException>().Which.Error.Should().Be(SessionError.EvTotalAboveLimit);
        draft.EvTotal.Should().Be(510, "a refused value is never clamped and leaves the draft unchanged");
        draft.EditEv(2, 5);
        draft.EditEv(3, 1);
        draft.EvTotal.Should().Be(510, "moving an EV between stats within the limit is accepted");
    }

    [Fact]
    public void AStoredTotalAboveTheLimitCanBeLoweredOneStatAtATime()
    {
        var draft = WithEvs(252, 252, 252, 0, 0, 0);

        // Typing 252 down to 0 passes through 25 and 2; each lowers the total, and is accepted while it is still above 510.
        draft.EditEv(1, 25);
        draft.EvTotal.Should().Be(529);
        var raise = () => draft.EditEv(3, 1);
        raise.Should().Throw<SessionException>().Which.Error.Should().Be(SessionError.EvTotalAboveLimit);
        var regain = () => draft.EditEv(1, 26);
        regain.Should().Throw<SessionException>().Which.Error.Should().Be(SessionError.EvTotalAboveLimit, "raising one while the total is over the limit is refused");

        draft.EditEv(1, 2);
        draft.EvTotal.Should().Be(506);
        draft.EditEv(1, 6);
        draft.EvTotal.Should().Be(510);
    }

    [Fact]
    public void AStoredEvAboveTheMaximumIsKeptUntilChanged()
    {
        var draft = WithEvs(255, 0, 0, 0, 0, 0);

        draft.EditEv(1, 4);
        draft.Evs[0].Should().Be(255, "other edits keep the stored value as stored");
        var retype = () => draft.EditEv(0, 255);
        retype.Should().Throw<SessionException>().Which.Error.Should().Be(SessionError.EvOutOfRange);

        draft.EditEv(0, 252);
        draft.Evs[0].Should().Be(252);
    }

    [Fact]
    public void ValuesChangedAndChangedBackLeaveTheDraftClean()
    {
        var draft = Zigzagoon();
        var ivs = draft.Ivs;
        var evs = draft.Evs;

        for (var stat = 0; stat < EditorDraft.StatCount; stat++)
        {
            draft.EditIv(stat, ivs[stat] == 0 ? 1 : 0);
            draft.EditEv(stat, evs[stat] == 0 ? 1 : 0);
        }
        draft.IsDirty.Should().BeTrue();
        for (var stat = 0; stat < EditorDraft.StatCount; stat++)
        {
            draft.EditIv(stat, ivs[stat]);
            draft.EditEv(stat, evs[stat]);
        }

        draft.IsDirty.Should().BeFalse();
    }

    [Fact]
    public void TheEditsAreGatedOnTheFamilyAndRefusedForEggs()
    {
        var bytes = SaveFixtures.Synthetic(true, customize: SaveFixtures.WithBoxEntity(0, 1, p => p.IsEgg = true));
        var save = SaveFixtures.Parse(bytes);
        var gated = new SaveSession(bytes.ToArray(), save, "fixture.sav", SaveCapabilities.For(save, SupportMatrix.Find(save)! with { Editable = EditableFields.Nickname }))
            .Select(SaveFixtures.FirstBoxSlot);
        var egg = SaveFixtures.Open(bytes).Select(SlotRef.InBox(0, 1));

        foreach (var (draft, error) in new[] { (gated, SessionError.FieldNotEditable), (egg, SessionError.EggNotEditable) })
        {
            Action[] edits = [() => draft.EditIv(0, 5), () => draft.EditEv(0, 5)];
            foreach (var edit in edits)
            {
                edit.Should().Throw<SessionException>().Which.Error.Should().Be(error);
            }
            draft.IsDirty.Should().BeFalse();
        }

        var onlyIvs = new SaveSession(bytes.ToArray(), save, "fixture.sav", SaveCapabilities.For(save, SupportMatrix.Find(save)! with { Editable = EditableFields.Ivs }))
            .Select(SaveFixtures.FirstBoxSlot);
        onlyIvs.EditIv(0, 5);
        var ev = () => onlyIvs.EditEv(0, 5);
        ev.Should().Throw<SessionException>().Which.Error.Should().Be(SessionError.FieldNotEditable, "IVs and EVs are gated separately");
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void APartyEvEditFollowsThePolicyAndMatchesNativeCore(bool oras)
    {
        var source = InjuredAndBurned(oras);
        var session = SaveFixtures.Open(source);
        var draft = session.Select(SlotRef.InParty(0));
        var stored = draft.Preview();
        var attack = stored.EV_ATK == 252 ? 0 : 252;

        draft.EditEv(1, attack);

        var preview = draft.Preview();
        var expectedStats = preview.GetStats(preview.PersonalInfo);
        new[] { preview.Stat_HPMax, preview.Stat_ATK, preview.Stat_DEF, preview.Stat_SPE, preview.Stat_SPA, preview.Stat_SPD }
            .Should().Equal(expectedStats.Select(s => (int)s), "the stats are recalculated, the odd stored Attack included");
        preview.Stat_ATK.Should().NotBe(1);
        preview.Stat_HPCurrent.Should().Be(Math.Min(7, (int)expectedStats[0]));
        preview.Status_Condition.Should().Be(Burn);
        draft.Inspect().Stats.Source.Should().Be(StatsSource.Recalculated);
        session.Apply(draft);

        var output = SaveExporter.Export(session, session.Select(SlotRef.InParty(0)));
        var native = SaveFixtures.Parse(source);
        var pk = native.GetPartySlotAtIndex(0);
        pk.EV_ATK = attack;
        pk.SetStats(expectedStats);
        pk.Stat_HPCurrent = Math.Min(7, (int)expectedStats[0]);
        native.SetPartySlotAtIndex(pk, 0, EntityImportSettings.None);
        output.Should().Equal(native.Write().ToArray(), "the session must match the native Core edit");
    }

    [Fact]
    public void APartyIvEditRecalculatesWithoutHealing()
    {
        var session = SaveFixtures.Open(InjuredAndBurned(true));
        var draft = session.Select(SlotRef.InParty(0));
        var stored = draft.Preview();

        draft.EditIv(0, stored.IV_HP == 31 ? 0 : 31);

        var preview = draft.Preview();
        preview.Stat_HPMax.Should().Be(preview.GetStats(preview.PersonalInfo)[0]);
        preview.Stat_HPCurrent.Should().Be(Math.Min(7, preview.Stat_HPMax), "an IV edit never heals");
        preview.Status_Condition.Should().Be(Burn);
    }

    [Fact]
    public void TypingAPartyEvAKeyAtATimeDoesNotLowerHpForGood()
    {
        // Typing 248 over a stored 252 passes through 2 and 24, which lower the maximum HP far below the member's current HP. HP clamped at
        // those keystrokes must not carry over: at 248 it is the smaller of the stored HP and the new maximum.
        var draft = SaveFixtures.Open(SaveFixtures.Synthetic(true, customize: SaveFixtures.WithPartyMember(battle: p =>
        {
            p.EXP = Experience.GetEXP(100, p.PersonalInfo.EXPGrowth);
            p.SetEVs([252, 0, 0, 0, 0, 0]);
            p.ResetPartyStats();
        }))).Select(SlotRef.InParty(0));
        var stored = draft.Preview();

        draft.EditEv(0, 2);
        draft.HpChange!.Value.NewHp.Should().BeLessThan(stored.Stat_HPCurrent - 1, "the fixture must clamp HP part-way");
        draft.EditEv(0, 24);
        draft.EditEv(0, 248);

        var preview = draft.Preview();
        preview.Stat_HPMax.Should().Be(stored.Stat_HPMax - 1, "at level 100 one EV step is one HP");
        preview.Stat_HPCurrent.Should().Be(Math.Min(stored.Stat_HPCurrent, preview.Stat_HPMax));

        // Typing the stored value back a key at a time leaves the draft as it was taken.
        draft.EditEv(0, 2);
        draft.EditEv(0, 25);
        draft.EditEv(0, 252);
        draft.IsDirty.Should().BeFalse();
    }

    [Fact]
    public void AStatEditOfAPartyMemberStoredWithoutStatsIsRefused()
    {
        var draft = SaveFixtures.Open(PartyApplyTests.WithoutStoredStats()).Select(SlotRef.InParty(0));

        Action[] edits = [() => draft.EditIv(0, 5), () => draft.EditEv(0, 5)];

        foreach (var edit in edits)
        {
            edit.Should().Throw<SessionException>().Which.Error.Should().Be(SessionError.PartyStatsMissing);
        }
        draft.IsDirty.Should().BeFalse();
    }

    [Fact]
    public void APartyEvEditThatChangesNoStatKeepsTheStoredBattleState()
    {
        // Stats grow every 4 EVs, so one more EV changes no stat; the stored Attack Core would not calculate is kept.
        var session = SaveFixtures.Open(InjuredAndBurned(true));
        var draft = session.Select(SlotRef.InParty(0));
        var stored = draft.Preview();
        var stat = stored.EV_ATK % 4 == 3 ? stored.EV_ATK - 1 : stored.EV_ATK + 1;

        draft.EditEv(1, stat);

        draft.Preview().Stat_ATK.Should().Be(1);
        draft.Preview().Data[stored.SIZE_STORED..].ToArray().Should().Equal(stored.Data[stored.SIZE_STORED..].ToArray());
        draft.Inspect().Stats.Source.Should().Be(StatsSource.Stored);
    }
}
