using FluentAssertions;
using PKHeX.Core;
using PKHeX.Web.Services;
using PKHeX.Web.State;
using Xunit;

namespace PKHeX.Web.Tests;

/// <summary>
/// The party and box views read the current revision, with Core's counts and names (WEB-PARTY-001, WEB-BOX-001/002/008).
/// </summary>
[Trait(TestCategory.Name, TestCategory.Unit)]
public sealed class StorageViewTests
{
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void BoxesFollowCoreCounts(bool oras)
    {
        var session = SaveFixtures.Open(SaveFixtures.Synthetic(oras));
        var save = SaveFixtures.Parse(SaveFixtures.Synthetic(oras));
        StorageView.BoxCount(session).Should().Be(save.BoxCount).And.Be(31);
        for (var box = 0; box < save.BoxCount; box++)
        {
            var view = StorageView.Box(session, box);
            view.Index.Should().Be(box);
            view.Slots.Select(s => s.Ref).Should().Equal(Enumerable.Range(0, save.BoxSlotCount).Select(i => SlotRef.InBox(box, i)));
        }
        var outside = () => StorageView.Box(session, save.BoxCount);
        outside.Should().Throw<ArgumentOutOfRangeException>();
        var negative = () => StorageView.Box(session, -1);
        negative.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void SlotsDescribeWhatTheyHold()
    {
        var session = SaveFixtures.Open(SaveFixtures.Synthetic(false));
        var slots = StorageView.Box(session, 0).Slots;
        var native = new PK6(SaveFixtures.ReadEntity(true));

        slots[0].Should().Be(new SlotSummary(SlotRef.InBox(0, 0), true, true, native.Species, native.IsNicknamed ? native.Nickname : null, false, native.IsShiny));
        slots[0].CanOpen.Should().BeTrue();
        slots.Skip(1).Should().OnlyContain(s => !s.Occupied && s.Readable && !s.CanOpen && s.Species == 0 && s.Nickname == null);
    }

    [Fact]
    public void EggsShinyAndNicknamesAreReported()
    {
        var session = SaveFixtures.Open(SaveFixtures.Synthetic(false, customize: save =>
        {
            var egg = new PK6(SaveFixtures.ReadEntity(true)) { IsEgg = true, Nickname = "Egg", IsNicknamed = true };
            save.SetBoxSlotAtIndex(egg, 1, 0, EntityImportSettings.None);
            var shiny = new PK6(SaveFixtures.ReadEntity(true)) { Nickname = "Sparkle", IsNicknamed = true };
            shiny.SetShiny();
            save.SetBoxSlotAtIndex(shiny, 1, 1, EntityImportSettings.None);
        }));
        var slots = StorageView.Box(session, 1).Slots;

        slots[0].IsEgg.Should().BeTrue();
        slots[1].IsShiny.Should().BeTrue();
        slots[1].Nickname.Should().Be("Sparkle");
    }

    [Fact]
    public void UnreadableEntityIsFlaggedWithNothingReadFromIt()
    {
        var native = SaveFixtures.Parse(SaveFixtures.Synthetic(false));
        var target = native.GetBoxSlotOffset(0, 1);
        native.Data.Slice(native.GetBoxSlotOffset(0, 0), native.SIZE_BOXSLOT).CopyTo(native.Data[target..]);
        native.Data[target + native.SIZE_BOXSLOT - 1] ^= 1;
        var session = SaveFixtures.Open(native.Write().ToArray());

        StorageView.Box(session, 0).Slots[1].Should().Be(new SlotSummary(SlotRef.InBox(0, 1), true, false, 0, null, false, false));
    }

    [Fact]
    public void EntityFailingItsSanityCheckIsABadEggThatCannotBeOpened()
    {
        // A correct checksum is not enough: the game, and WinForms (PKM.Valid), also treat a set sanity flag as a bad egg.
        var session = SaveFixtures.Open(SaveFixtures.Synthetic(false, customize: save =>
        {
            var bad = new PK6(SaveFixtures.ReadEntity(true)) { Sanity = 1 };
            bad.RefreshChecksum();
            bad.ChecksumValid.Should().BeTrue();
            save.SetBoxSlotAtIndex(bad, 0, 1, EntityImportSettings.None);
        }));
        session.Working.GetBoxSlotAtIndex(0, 1).ChecksumValid.Should().BeTrue();

        StorageView.Box(session, 0).Slots[1].Should().Be(new SlotSummary(SlotRef.InBox(0, 1), true, false, 0, null, false, false));
        var select = () => session.Select(SlotRef.InBox(0, 1));
        select.Should().Throw<SessionException>().Which.Error.Should().Be(SessionError.EntityInvalid);
    }

    [Fact]
    public void PartyShowsEverySixPositionsWithEmptiesAfterTheCount()
    {
        var session = SaveFixtures.Open(SaveFixtures.Synthetic(false, customize: SaveFixtures.WithPartyMember("Leader")));
        var party = StorageView.Party(session);

        party.Select(s => s.Ref).Should().Equal(Enumerable.Range(0, 6).Select(SlotRef.InParty));
        party[0].Occupied.Should().BeTrue();
        party[0].Nickname.Should().Be("Leader");
        party.Skip(1).Should().OnlyContain(s => !s.Occupied);
    }

    [Fact]
    public void PartyBytesAfterTheCountAreNeverShown()
    {
        // A released member's bytes can remain after the party count; the game treats the position as empty, and so must we.
        var session = SaveFixtures.Open(SaveFixtures.Synthetic(false, customize: save =>
        {
            SaveFixtures.WithPartyMember("First")(save);
            var second = new PK6(SaveFixtures.ReadEntity(true)) { Nickname = "Leftover", IsNicknamed = true };
            save.SetPartySlotAtIndex(second, 1, EntityImportSettings.None);
            // The count byte follows the six party slots (SAV6.PartyCount); its setter is not public.
            save.Data[save.GetPartyOffset(6)] = 1;
        }));
        session.Working.GetPartySlotAtIndex(1).Species.Should().NotBe(0, "the fixture must keep bytes after the count");

        StorageView.Party(session)[1].Occupied.Should().BeFalse();
        var select = () => session.Select(SlotRef.InParty(1));
        select.Should().Throw<SessionException>().Which.Error.Should().Be(SessionError.SlotNotOccupied);
    }

    [Fact]
    public void BoxNamesAreStoredOrNull()
    {
        const string hostile = "<b>x</b>"; // Within the Gen 6 box name limit.
        var session = SaveFixtures.Open(SaveFixtures.Synthetic(false, customize: save =>
        {
            var names = (IBoxDetailName)save;
            names.SetBoxName(0, "Keepers");
            names.SetBoxName(1, "");
            names.SetBoxName(2, "   ");
            names.SetBoxName(3, hostile);
        }));

        StorageView.BoxName(session, 0).Should().Be("Keepers");
        StorageView.BoxName(session, 1).Should().BeNull();
        StorageView.BoxName(session, 2).Should().BeNull();
        StorageView.BoxName(session, 3).Should().Be(hostile);
        StorageView.Box(session, 0).StoredName.Should().Be("Keepers");
    }

    [Theory]
    [InlineData(7, 7)]
    [InlineData(30, 30)]
    [InlineData(31, 0)]
    [InlineData(200, 0)]
    public void InitialBoxIsTheInGameCurrentBoxWhenValid(int stored, int expected)
    {
        var session = SaveFixtures.Open(SaveFixtures.Synthetic(true, customize: save => save.CurrentBox = stored));
        StorageView.InitialBox(session).Should().Be(expected);
    }

    [Fact]
    public void ViewsFollowTheCurrentRevision()
    {
        var session = SaveFixtures.Open(SaveFixtures.Synthetic(false));
        var draft = session.Select(SaveFixtures.FirstBoxSlot);
        draft.EditNickname("Renamed", true);
        session.Apply(draft);

        StorageView.Box(session, 0).Slots[0].Nickname.Should().Be("Renamed");
    }
}
