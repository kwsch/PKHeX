using FluentAssertions;
using PKHeX.Core;
using PKHeX.Web.Services;
using PKHeX.Web.State;
using Xunit;

namespace PKHeX.Web.Tests;

/// <summary>
/// Previous and Next Pokémon in the editor: party then every box in Core's order, skipping empty positions and bad eggs, wrapping at either
/// end, and never replacing unapplied work.
/// </summary>
[Trait(TestCategory.Name, TestCategory.Unit)]
public sealed class SlotStepTests
{
    /// <summary>Party 1 and 2, box 1 slot 1, an egg in box 3 slot 6, and the last slot of the last box; a bad egg in box 1 slot 2.</summary>
    private static readonly SlotRef[] Openable =
    [
        SlotRef.InParty(0), SlotRef.InParty(1), SlotRef.InBox(0, 0), SlotRef.InBox(2, 5), SlotRef.InBox(30, 29),
    ];

    private static readonly SlotRef BadEgg = SlotRef.InBox(0, 1);

    /// <summary>The save described by <see cref="Openable"/>, as XY or ORAS.</summary>
    private static SaveSession Fixture(bool oras = false)
    {
        var native = SaveFixtures.Parse(SaveFixtures.Synthetic(oras, customize: SaveFixtures.All(
            SaveFixtures.WithPartyMember("First"),
            SaveFixtures.WithPartyMember("Second", position: 1),
            SaveFixtures.WithBoxEntity(2, 5, e => e.IsEgg = true),
            SaveFixtures.WithBoxEntity(30, 29, _ => { }))));
        // A copy of box 1, slot 1 with one stored byte flipped, so it fails its checksum: a bad egg.
        var target = native.GetBoxSlotOffset(BadEgg.Box, BadEgg.Slot);
        native.Data.Slice(native.GetBoxSlotOffset(0, 0), native.SIZE_BOXSLOT).CopyTo(native.Data[target..]);
        native.Data[target + native.SIZE_BOXSLOT - 1] ^= 1;
        return SaveFixtures.Open(native.Write().ToArray());
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void NextVisitsEveryOpenablePositionInPartyThenBoxOrderAndWraps(bool oras)
    {
        var session = Fixture(oras);
        var visited = new List<SlotRef>();
        var at = Openable[0];
        for (var i = 0; i < Openable.Length; i++)
        {
            at = StorageView.Neighbour(session, at, 1)!.Value;
            visited.Add(at);
        }
        visited.Should().Equal([.. Openable.Skip(1), Openable[0]], "the last box's last slot wraps to the first party member");
    }

    [Fact]
    public void PreviousVisitsTheSameOrderBackwards()
    {
        var session = Fixture();
        var visited = new List<SlotRef>();
        var at = Openable[0];
        for (var i = 0; i < Openable.Length; i++)
        {
            at = StorageView.Neighbour(session, at, -1)!.Value;
            visited.Add(at);
        }
        visited.Should().Equal([.. Openable.Reverse()]);
    }

    [Fact]
    public void EmptyPositionsBadEggsAndPartyPositionsPastTheCountAreSkippedButEggsAreNot()
    {
        var session = Fixture();
        StorageView.Neighbour(session, SlotRef.InBox(0, 0), 1).Should().Be(SlotRef.InBox(2, 5), "box 1 slot 2 is a bad egg and the rest is empty");
        StorageView.Neighbour(session, SlotRef.InBox(2, 5), -1).Should().Be(SlotRef.InBox(0, 0));
        StorageView.Neighbour(session, SlotRef.InParty(1), 1).Should().Be(SlotRef.InBox(0, 0), "party positions 3 to 6 are past the party count");
        StorageView.Slot(session, SlotRef.InBox(2, 5)).IsEgg.Should().BeTrue();
    }

    [Fact]
    public void AnyPositionCanBeSteppedFromEvenOneThatCannotBeOpened()
    {
        var session = Fixture();
        StorageView.Neighbour(session, BadEgg, 1).Should().Be(SlotRef.InBox(2, 5));
        StorageView.Neighbour(session, SlotRef.InParty(5), 1).Should().Be(SlotRef.InBox(0, 0));
        StorageView.Neighbour(session, SlotRef.InParty(5), -1).Should().Be(SlotRef.InParty(1));
    }

    [Fact]
    public void ASaveWithOnePokemonHasNoOther()
    {
        var session = SaveFixtures.Open(SaveFixtures.Synthetic(false));
        StorageView.Neighbour(session, SaveFixtures.FirstBoxSlot, 1).Should().BeNull();
        StorageView.Neighbour(session, SaveFixtures.FirstBoxSlot, -1).Should().BeNull();
        StorageView.Neighbour(session, SlotRef.InBox(4, 4), 1).Should().Be(SaveFixtures.FirstBoxSlot, "from an empty slot the only Pokémon is another one");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(2)]
    [InlineData(-2)]
    public void TheDirectionIsOneStep(int direction)
    {
        var session = Fixture();
        var step = () => StorageView.Neighbour(session, SaveFixtures.FirstBoxSlot, direction);
        step.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void APositionOutsideTheSaveIsRefused()
    {
        var session = Fixture();
        var step = () => StorageView.Neighbour(session, SlotRef.InBox(31, 0), 1);
        step.Should().Throw<ArgumentOutOfRangeException>();
        var slot = () => StorageView.Slot(session, SlotRef.InBox(0, 30));
        slot.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void StepOpensTheNeighbourAndShowsItsBox()
    {
        var state = SaveFixtures.NewState();
        state.Open(Fixture());
        var noDraft = () => state.Step(1);
        noDraft.Should().Throw<InvalidOperationException>();
        state.OpenSlot(SlotRef.InParty(1)).Should().Be(SlotOpening.Opened);
        state.ShowBox(7);

        state.Step(1).Should().Be(new SlotStep(StepOutcome.Opened, SlotRef.InBox(0, 0)));
        state.Draft!.Slot.Should().Be(SlotRef.InBox(0, 0));
        state.CurrentBox.Should().Be(0, "the storage browser follows the draft into its box");
        state.View.Pane.Should().Be(WorkspacePane.Editor);

        state.Step(1).Should().Be(new SlotStep(StepOutcome.Opened, SlotRef.InBox(2, 5)));
        state.CurrentBox.Should().Be(2);
        state.Step(-1).Target.Should().Be(SlotRef.InBox(0, 0));
        state.Step(-1).Target.Should().Be(SlotRef.InParty(1));
        state.CurrentBox.Should().Be(0, "a party member leaves the box shown as it was");
    }

    [Fact]
    public void StepNeverReplacesUnappliedOrRefusedWork()
    {
        var state = SaveFixtures.NewState();
        state.Open(Fixture());
        state.OpenSlot(SlotRef.InParty(0));
        var draft = state.Draft!;
        draft.EditNickname("Unapplied", true);
        state.ShowBox(5);

        state.Step(1).Should().Be(new SlotStep(StepOutcome.DraftPending));
        state.Step(-1).Should().Be(new SlotStep(StepOutcome.DraftPending));
        state.Draft.Should().BeSameAs(draft);
        state.CurrentBox.Should().Be(5, "a refused step changes nothing, not even the box shown");

        state.SetDraft(state.Session!.Select(SlotRef.InParty(0)));
        state.RefuseDraftEdit(new FieldRefusal("level", SessionError.LevelOutOfRange));
        state.Step(1).Outcome.Should().Be(StepOutcome.DraftPending);
        state.Draft!.Slot.Should().Be(SlotRef.InParty(0));
    }

    [Fact]
    public void StepWithNoOtherPokemonKeepsTheDraft()
    {
        var state = SaveFixtures.NewState();
        state.Open(SaveFixtures.Open(SaveFixtures.Synthetic(false)));
        state.OpenSlot(SaveFixtures.FirstBoxSlot);
        var draft = state.Draft;
        state.ShowBox(3);

        state.Step(1).Should().Be(new SlotStep(StepOutcome.NoOther));
        state.Draft.Should().BeSameAs(draft);
        state.CurrentBox.Should().Be(3);
    }

    [Fact]
    public void StepReadsTheCurrentRevision()
    {
        var state = SaveFixtures.NewState();
        state.Open(Fixture());
        state.OpenSlot(SlotRef.InBox(2, 5));
        // Nothing is remembered between steps: a position emptied since the last one is skipped.
        state.Session!.Working.SetBoxSlotAtIndex(state.Session.Working.BlankPKM, 30, 29, EntityImportSettings.None);
        state.Step(1).Target.Should().Be(SlotRef.InParty(0), "the emptied last slot is skipped");
    }
}
