using FluentAssertions;
using PKHeX.Core;
using PKHeX.Web.Services;
using PKHeX.Web.State;
using Xunit;

namespace PKHeX.Web.Tests;

/// <summary>
/// Drafting the held item, moves, PP and PP Ups (WEB-PKM-010, WEB-PKM-011, WEB-PKM-012): items and moves come from Core's lists for the
/// game, a move change sets that slot's PP as the desktop does, PP and PP Ups are refused outside Core's rules rather than clamped, only
/// the edited slot's bytes change, and none of these edits touches a party member's battle state.
/// </summary>
[Trait(TestCategory.Name, TestCategory.Unit)]
public sealed class ItemMoveDraftTests
{
    private const int Burn = 0x10;

    /// <summary>Item IDs (Core has no item enum): both are in the XY and ORAS held item lists.</summary>
    internal const int Leftovers = 234, ChoiceScarf = 287;
    private const ushort Tackle = (ushort)Move.Tackle, Surf = (ushort)Move.Surf, Sketch = (ushort)Move.Sketch, Thunderbolt = (ushort)Move.Thunderbolt;

    /// <summary>
    /// A box 1, slot 2 Pokémon with Tackle (2 PP Ups, 20 PP), an empty slot, Surf (3 PP Ups, full PP) and Sketch (no PP Ups, full PP),
    /// holding a Leftovers.
    /// </summary>
    internal static byte[] Moveset(bool oras = true, Action<PK6>? change = null) => SaveFixtures.Synthetic(oras, customize: SaveFixtures.WithBoxEntity(0, 1, p =>
    {
        p.Move1 = Tackle;
        p.Move2 = 0;
        p.Move3 = Surf;
        p.Move4 = Sketch;
        p.Move1_PPUps = 2;
        p.Move1_PP = 20;
        p.Move2_PPUps = 0;
        p.Move2_PP = 0;
        p.Move3_PPUps = 3;
        p.Move3_PP = p.GetMovePP(Surf, 3);
        p.Move4_PPUps = 0;
        p.Move4_PP = p.GetMovePP(Sketch, 0);
        p.HeldItem = Leftovers;
        change?.Invoke(p);
    }));

    private static EditorDraft Open(byte[] bytes) => SaveFixtures.Open(bytes).Select(SlotRef.InBox(0, 1));

    private static EditorDraft Draft(bool oras = true, Action<PK6>? change = null) => Open(Moveset(oras, change));

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
    public void TheReadersShowTheStoredSlotsInOrder(bool oras)
    {
        var draft = Draft(oras);

        draft.HeldItem.Should().Be(Leftovers);
        draft.Moves.Should().Equal(
            new MoveSlot(Tackle, 20, 2, 49, true), // Tackle has 35 PP in Generation 6; two PP Ups make it 35 * 7 / 5
            new MoveSlot(0, 0, 0, 0, false),
            new MoveSlot(Surf, 24, 3, 24, true),
            new MoveSlot(Sketch, 1, 0, 1, false));
        draft.Moves.Select(m => m.IsEmpty).Should().Equal(false, true, false, false);
        draft.StoredMoves.Should().Equal(draft.Moves);
        draft.Inspect().Moves.Moves.Select(m => (m.Move.Value, m.Pp, m.PpUps, m.MaxPp))
            .Should().Equal(draft.Moves.Select(m => ((int)m.Move, m.Pp, m.PpUps, m.MaxPp)), "the editor and the inspector read the same slots");
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void AHeldItemEditMatchesNativeCore(bool oras)
    {
        var draft = Draft(oras);
        var stored = draft.Preview();

        draft.EditHeldItem(ChoiceScarf);
        AssertMatchesNative(draft, stored, p => p.HeldItem = ChoiceScarf);

        draft.EditHeldItem(0);
        AssertMatchesNative(draft, stored, p => p.HeldItem = 0);
        draft.HeldItem.Should().Be(0, "none is an explicit choice");
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void AMoveChangeGetsFullPpForTheKeptPpUpsAndMatchesNativeCore(bool oras)
    {
        var draft = Draft(oras);
        var stored = draft.Preview();

        draft.EditMove(0, Thunderbolt);

        draft.Moves[0].Should().Be(new MoveSlot(Thunderbolt, 21, 2, 21, true)); // 15 * 7 / 5
        AssertMatchesNative(draft, stored, p =>
        {
            p.Move1 = Thunderbolt;
            p.Move1_PP = p.GetMovePP(Thunderbolt, 2);
        });
    }

    [Fact]
    public void EachSlotIsEditedAloneAndKeepsItsPlace()
    {
        for (var slot = 0; slot < EditorDraft.MoveCount; slot++)
        {
            var draft = Draft();
            var before = draft.Moves;

            draft.EditMove(slot, Thunderbolt);

            var after = draft.Moves;
            after[slot].Move.Should().Be(Thunderbolt);
            for (var other = 0; other < EditorDraft.MoveCount; other++)
            {
                if (other != slot)
                {
                    after[other].Should().Be(before[other], "an edit of one slot leaves the others, and their order, as they were");
                }
            }
        }
    }

    [Fact]
    public void ClearingASlotZeroesItsPpAndLeavesTheGapInPlace()
    {
        var draft = Draft();
        var stored = draft.Preview();

        draft.EditMove(0, 0);

        draft.Moves[0].Should().Be(new MoveSlot(0, 0, 0, 0, false));
        draft.Moves[2].Move.Should().Be(Surf, "nothing moves up into an emptied slot, as Core's FixMoves would");
        AssertMatchesNative(draft, stored, p =>
        {
            p.Move1 = 0;
            p.Move1_PP = 0;
            p.Move1_PPUps = 0;
        });
    }

    [Fact]
    public void AnEmptySlotCanBeFilled()
    {
        var draft = Draft();

        draft.EditMove(1, Surf);

        draft.Moves[1].Should().Be(new MoveSlot(Surf, 15, 0, 15, true));
        draft.Moves[2].Move.Should().Be(Surf, "a move may be chosen twice; legality analysis reports it");
    }

    [Fact]
    public void AMoveThatCannotTakePpUpsHasThemRemoved()
    {
        var draft = Draft();
        var stored = draft.Preview();

        draft.EditMove(2, Sketch);

        draft.Moves[2].Should().Be(new MoveSlot(Sketch, 1, 0, 1, false));
        AssertMatchesNative(draft, stored, p =>
        {
            p.Move3 = Sketch;
            p.Move3_PPUps = 0;
            p.Move3_PP = 1;
        });
        var raise = () => draft.EditPpUps(2, 1);
        raise.Should().Throw<SessionException>().Which.Error.Should().Be(SessionError.PpUpsNotAllowed);
    }

    [Fact]
    public void ReturningToTheStoredMoveRestoresItsStoredPp()
    {
        // Tackle is stored with 20 of 49 PP; going through other moves (and an empty slot) and back must not refill it.
        var draft = Draft();

        draft.EditMove(0, Thunderbolt);
        draft.EditMove(0, Sketch);
        draft.EditMove(0, 0);
        draft.EditMove(0, Tackle);

        draft.Moves[0].Should().Be(new MoveSlot(Tackle, 20, 2, 49, true));
        draft.IsDirty.Should().BeFalse();
    }

    [Fact]
    public void ChoosingTheDraftedMoveAgainKeepsItsPp()
    {
        var draft = Draft();
        draft.EditMove(0, Thunderbolt);
        draft.EditPp(0, 3);

        draft.EditMove(0, Thunderbolt);

        draft.Moves[0].Pp.Should().Be(3);
    }

    [Fact]
    public void APpUpsChangeRefillsPpAndChangingBackRestoresTheStoredPp()
    {
        var draft = Draft();
        var stored = draft.Preview();

        draft.EditPpUps(0, 3);
        draft.Moves[0].Should().Be(new MoveSlot(Tackle, 56, 3, 56, true));
        AssertMatchesNative(draft, stored, p =>
        {
            p.Move1_PPUps = 3;
            p.Move1_PP = p.GetMovePP(Tackle, 3);
        });

        draft.EditPpUps(0, 0);
        draft.Moves[0].Should().Be(new MoveSlot(Tackle, 35, 0, 35, true));

        draft.EditPpUps(0, 2);
        draft.Moves[0].Pp.Should().Be(20, "the stored count gives back the stored PP");
        draft.IsDirty.Should().BeFalse();
    }

    [Fact]
    public void APpUpsChangeOnANewMoveRefillsIt()
    {
        var draft = Draft();
        draft.EditMove(0, Thunderbolt);

        draft.EditPpUps(0, 0);

        draft.Moves[0].Should().Be(new MoveSlot(Thunderbolt, 15, 0, 15, true), "the stored PP belongs to the stored move only");
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void APpEditMatchesNativeCore(bool oras)
    {
        var draft = Draft(oras);
        var stored = draft.Preview();

        draft.EditPp(0, 49);
        draft.EditPp(2, 0);

        AssertMatchesNative(draft, stored, p =>
        {
            p.Move1_PP = 49;
            p.Move3_PP = 0;
        });
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(50)]
    [InlineData(255)]
    [InlineData(256)]
    [InlineData(int.MaxValue)]
    [InlineData(int.MinValue)]
    public void PpOutOfRangeIsRefusedNotClamped(int value)
    {
        // Core's setter would store the low byte, and nothing in Core keeps PP within the move's maximum.
        var draft = Draft();

        var edit = () => draft.EditPp(0, value);

        edit.Should().Throw<SessionException>().Which.Error.Should().Be(SessionError.PpOutOfRange);
        draft.IsDirty.Should().BeFalse();
        draft.EditRevision.Should().Be(0);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(4)]
    [InlineData(255)]
    [InlineData(int.MaxValue)]
    public void PpUpsOutOfRangeAreRefusedNotClamped(int value)
    {
        var draft = Draft();

        var edit = () => draft.EditPpUps(0, value);

        edit.Should().Throw<SessionException>().Which.Error.Should().Be(SessionError.PpUpsOutOfRange);
        draft.IsDirty.Should().BeFalse();
    }

    [Fact]
    public void AnEmptySlotHasNoPpToEdit()
    {
        var draft = Draft();

        Action[] edits = [() => draft.EditPp(1, 0), () => draft.EditPpUps(1, 0), () => draft.EditPpUps(1, 1)];

        foreach (var edit in edits)
        {
            edit.Should().Throw<SessionException>().Which.Error.Should().Be(SessionError.MoveSlotEmpty);
        }
        draft.IsDirty.Should().BeFalse();
    }

    [Fact]
    public void ASlotOutsideTheFourIsAProgrammingError()
    {
        var draft = Draft();

        Action[] edits = [() => draft.EditMove(-1, Tackle), () => draft.EditMove(4, Tackle), () => draft.EditPp(4, 1), () => draft.EditPpUps(-1, 1)];

        foreach (var edit in edits)
        {
            edit.Should().Throw<ArgumentOutOfRangeException>();
        }
        draft.IsDirty.Should().BeFalse();
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void AMoveOrItemOutsideTheGamesListIsRefused(bool oras)
    {
        var draft = Draft(oras);
        var lists = draft.Capabilities.Lists;
        var maxItem = oras ? 775 : 717;
        var unholdable = Enumerable.Range(1, maxItem).First(i => lists.Items.All(item => item.Value != i));

        // 622 is the first Z-move, past the last ORAS move (621); XY ends at 617.
        foreach (var move in new[] { lists.Moves.Max(m => m.Value) + 1, 622, (int)Move.MaxFlare, 0x7FFF, -1 })
        {
            var edit = () => draft.EditMove(0, move);
            edit.Should().Throw<SessionException>().Which.Error.Should().Be(SessionError.MoveNotAvailable);
        }
        foreach (var item in new[] { maxItem + 1, unholdable, -1, 0xFFFF })
        {
            var edit = () => draft.EditHeldItem(item);
            edit.Should().Throw<SessionException>().Which.Error.Should().Be(SessionError.ItemNotAvailable);
        }
        draft.IsDirty.Should().BeFalse();
    }

    [Fact]
    public void AStoredItemOutsideTheListIsKeptUntilChanged()
    {
        var draft = Draft(change: p => p.HeldItem = 0x7FFF);

        draft.EditMove(0, Thunderbolt);
        draft.HeldItem.Should().Be(0x7FFF);
        var retype = () => draft.EditHeldItem(0x7FFF);
        retype.Should().Throw<SessionException>().Which.Error.Should().Be(SessionError.ItemNotAvailable);

        draft.EditHeldItem(0);
        draft.HeldItem.Should().Be(0);
    }

    [Fact]
    public void StoredPpAboveTheMaximumIsKeptUntilEdited()
    {
        var draft = Draft(change: p => p.Move3_PP = 60);

        draft.EditMove(0, Thunderbolt);
        draft.Moves[2].Pp.Should().Be(60);
        var retype = () => draft.EditPp(2, 60);
        retype.Should().Throw<SessionException>().Which.Error.Should().Be(SessionError.PpOutOfRange);

        draft.EditPp(2, 24);
        draft.Moves[2].Pp.Should().Be(24);
    }

    [Fact]
    public void StoredPpUpsOnSketchCanBeRemoved()
    {
        var draft = Draft(change: p => p.Move4_PPUps = 3);

        draft.EditPpUps(3, 0);

        draft.Moves[3].Should().Be(new MoveSlot(Sketch, 1, 0, 1, false));
    }

    [Fact]
    public void ValuesChangedAndChangedBackLeaveTheDraftClean()
    {
        var draft = Draft();

        draft.EditHeldItem(0);
        draft.EditMove(2, Tackle);
        draft.EditPp(0, 1);
        draft.IsDirty.Should().BeTrue();
        draft.EditHeldItem(Leftovers);
        draft.EditMove(2, Surf);
        draft.EditPp(0, 20);

        draft.IsDirty.Should().BeFalse();
    }

    [Fact]
    public void TheEditsAreGatedOnTheFamilyAndRefusedForEggs()
    {
        var bytes = Moveset(change: p => p.IsEgg = true);
        var save = SaveFixtures.Parse(bytes);
        SaveSession Session(EditableFields fields) => new(bytes.ToArray(), save, "fixture.sav", SaveCapabilities.For(save, SupportMatrix.Find(save)! with { Editable = fields }));
        var gated = Session(EditableFields.Nickname).Select(SaveFixtures.FirstBoxSlot);
        var egg = SaveFixtures.Open(bytes).Select(SlotRef.InBox(0, 1));

        foreach (var (draft, error) in new[] { (gated, SessionError.FieldNotEditable), (egg, SessionError.EggNotEditable) })
        {
            Action[] edits = [() => draft.EditHeldItem(0), () => draft.EditMove(0, Tackle), () => draft.EditPp(0, 1), () => draft.EditPpUps(0, 1)];
            foreach (var edit in edits)
            {
                edit.Should().Throw<SessionException>().Which.Error.Should().Be(error);
            }
            draft.IsDirty.Should().BeFalse();
        }

        var onlyMoves = Session(EditableFields.Moves).Select(SaveFixtures.FirstBoxSlot);
        onlyMoves.EditMove(0, Tackle);
        Action[] others = [() => onlyMoves.EditPp(0, 1), () => onlyMoves.EditPpUps(0, 1), () => onlyMoves.EditHeldItem(0)];
        foreach (var edit in others)
        {
            edit.Should().Throw<SessionException>().Which.Error.Should().Be(SessionError.FieldNotEditable, "moves, PP and the held item are gated separately");
        }
        var onlyPp = Session(EditableFields.Pp).Select(SaveFixtures.FirstBoxSlot);
        onlyPp.EditPp(0, 1);
        var move = () => onlyPp.EditMove(0, Tackle);
        move.Should().Throw<SessionException>().Which.Error.Should().Be(SessionError.FieldNotEditable);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void PartyEditsKeepTheBattleStateAndMatchNativeCore(bool oras)
    {
        var source = InjuredAndBurned(oras);
        var session = SaveFixtures.Open(source);
        var draft = session.Select(SlotRef.InParty(0));
        var stored = draft.Preview();
        var ups = stored.Move1_PPUps == 3 ? 2 : 3;

        draft.EditHeldItem(Leftovers);
        draft.EditMove(1, Thunderbolt);
        draft.EditPpUps(0, ups);
        draft.EditPp(0, 1);

        draft.Preview().Data[stored.SIZE_STORED..].ToArray().Should().Equal(stored.Data[stored.SIZE_STORED..].ToArray(), "no stat is affected, so the battle state is kept byte for byte");
        draft.HpChange.Should().BeNull();
        draft.Inspect().Stats.Source.Should().Be(StatsSource.Stored);
        session.Apply(draft);

        var output = SaveExporter.Export(session, session.Select(SlotRef.InParty(0)));
        var native = SaveFixtures.Parse(source);
        var pk = native.GetPartySlotAtIndex(0);
        pk.HeldItem = Leftovers;
        pk.Move2 = Thunderbolt;
        pk.Move2_PP = pk.GetMovePP(Thunderbolt, pk.Move2_PPUps);
        pk.Move1_PPUps = ups;
        pk.Move1_PP = 1;
        native.SetPartySlotAtIndex(pk, 0, EntityImportSettings.None);
        output.Should().Equal(native.Write().ToArray(), "the session must match the native Core edit");
    }

    [Fact]
    public void APartyMemberStoredWithoutStatsCanHaveItsMovesEdited()
    {
        // These edits affect no stat, so they need no current HP to keep; Apply still refuses to write such a member.
        var draft = SaveFixtures.Open(PartyApplyTests.WithoutStoredStats()).Select(SlotRef.InParty(0));

        draft.EditMove(0, Thunderbolt);

        draft.Moves[0].Move.Should().Be(Thunderbolt);
    }

    [Fact]
    public void EachEditCountsAsAnEdit()
    {
        // The revision tags legality results, so every accepted edit makes the last result stale.
        var draft = Draft();

        draft.EditHeldItem(0);
        draft.EditMove(0, Thunderbolt);
        draft.EditPpUps(0, 1);
        draft.EditPp(0, 2);

        draft.EditRevision.Should().Be(4);
    }

    [Fact]
    public void TheMoveChangeIsDescribedFromTheSlotsBeforeAndAfter()
    {
        var draft = Draft();
        var stored = draft.StoredMoves;
        MoveChange? Edit(int slot, Action<EditorDraft> edit)
        {
            var before = draft.Moves[slot];
            edit(draft);
            return MoveChange.Between(slot, before, draft.Moves[slot], stored[slot]);
        }

        Edit(0, d => d.EditMove(0, Thunderbolt))!.Kind.Should().Be(MoveChangeKind.MoveSet);
        Edit(2, d => d.EditMove(2, Sketch))!.Kind.Should().Be(MoveChangeKind.PpUpsCleared);
        Edit(3, d => d.EditMove(3, Sketch)).Should().BeNull("choosing the drafted move again changes nothing");
        Edit(3, d => d.EditMove(3, Tackle))!.Kind.Should().Be(MoveChangeKind.MoveSet, "Sketch had no PP Ups to remove");
        Edit(0, d => d.EditMove(0, 0))!.Kind.Should().Be(MoveChangeKind.Emptied);
        Edit(0, d => d.EditMove(0, Tackle))!.Kind.Should().Be(MoveChangeKind.Restored);
        Edit(0, d => d.EditPpUps(0, 3))!.Kind.Should().Be(MoveChangeKind.PpUpsChanged);
        Edit(0, d => d.EditPp(0, 3)).Should().BeNull("a PP edit is shown in its own field");
    }

    [Fact]
    public void PpUpsSurviveAPassThroughAMoveWithoutThemOrAnEmptySlot()
    {
        // Typing "Sky Attack" into a move box picks "Sketch" on the way; the PP Ups the slot had must reach the move meant.
        var draft = Draft();

        draft.EditMove(2, Sketch);
        draft.Moves[2].PpUps.Should().Be(0);
        draft.EditMove(2, (ushort)Move.SkyAttack);
        draft.Moves[2].Should().Be(new MoveSlot((ushort)Move.SkyAttack, 8, 3, 8, true)); // 5 PP, 3 PP Ups: 5 * 8 / 5

        draft.EditMove(2, 0);
        draft.EditMove(2, Thunderbolt);
        draft.Moves[2].PpUps.Should().Be(3, "an emptied slot keeps the count for its next move");
    }

    [Fact]
    public void TheLastPpUpsChosenAreCarried()
    {
        var draft = Draft();

        draft.EditPpUps(0, 1);
        draft.EditMove(0, Sketch);
        draft.EditMove(0, Thunderbolt);
        draft.Moves[0].PpUps.Should().Be(1);

        // Back to the stored move restores its stored count, which is then the one carried.
        draft.EditMove(0, Tackle);
        draft.EditMove(0, Surf);
        draft.Moves[0].PpUps.Should().Be(2);
    }

    [Fact]
    public void AStoredPpUpsCountAboveTheMaximumIsNotCarried()
    {
        // Core's PP for 200 PP Ups is far above the byte PK6 stores PP in; carrying it would store a truncated value.
        var draft = Draft(change: p => p.Move1_PPUps = 200);

        draft.EditMove(0, Surf);

        draft.Moves[0].Should().Be(new MoveSlot(Surf, 15, 0, 15, true));
        draft.EditMove(0, Tackle);
        draft.Moves[0].PpUps.Should().Be(200, "the stored move comes back as stored");
    }

    [Fact]
    public void PpAboveTheStoredByteIsRefusedWhateverThePpUps()
    {
        var draft = Draft(change: p => p.Move1_PPUps = 200);
        draft.Moves[0].MaxPp.Should().BeGreaterThan(255, "the fixture must make Core's figure larger than a byte");

        var edit = () => draft.EditPp(0, 256);

        edit.Should().Throw<SessionException>().Which.Error.Should().Be(SessionError.PpOutOfRange);
        draft.EditPp(0, 255);
        draft.Moves[0].Pp.Should().Be(255);
    }
}
