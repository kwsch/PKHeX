using FluentAssertions;
using PKHeX.Web.State;
using Xunit;

namespace PKHeX.Web.Tests;

/// <summary>
/// The structural slot diff behind apply (from PKForge's write-safety check): every party position and box slot, the target excepted.
/// </summary>
[Trait(TestCategory.Name, TestCategory.Unit)]
public sealed class SlotImagesTests
{
    [Fact]
    public void EveryPartyPositionAndBoxSlotIsListed()
    {
        var save = SaveFixtures.Parse(SaveFixtures.Synthetic(false, customize: SaveFixtures.WithPartyMember()));

        var images = SlotImages.Of(save);

        images.Should().HaveCount(SlotRef.PartyPositions + (save.BoxCount * save.BoxSlotCount));
        images.Take(SlotRef.PartyPositions).Select(i => i.Slot).Should().Equal(Enumerable.Range(0, SlotRef.PartyPositions).Select(SlotRef.InParty));
        images[SlotRef.PartyPositions].Slot.Should().Be(SlotRef.InBox(0, 0));
        images[^1].Slot.Should().Be(SlotRef.InBox(save.BoxCount - 1, save.BoxSlotCount - 1));
        images.Select(i => i.Slot).Should().OnlyHaveUniqueItems();
    }

    [Fact]
    public void OnlyTheTargetMayChange()
    {
        var save = SaveFixtures.Parse(SaveFixtures.Synthetic(false, customize: SaveFixtures.WithPartyMember()));
        var before = SlotImages.Of(save);
        var after = before.Select(i => i with { Bytes = i.Bytes.ToArray() }).ToList();
        after[SlotRef.PartyPositions].Bytes[0] ^= 1;

        SlotImages.FirstUntargetedChange(before, after, SlotRef.InBox(0, 0)).Should().BeNull();
        SlotImages.FirstUntargetedChange(before, after, SlotRef.InParty(0)).Should().Be(SlotRef.InBox(0, 0));
        SlotImages.FirstUntargetedChange(before, before, SlotRef.InParty(0)).Should().BeNull();
    }

    [Fact]
    public void AMissingOrExtraPositionIsAChange()
    {
        var save = SaveFixtures.Parse(SaveFixtures.Synthetic(false));
        var before = SlotImages.Of(save);

        SlotImages.FirstUntargetedChange(before, before.Take(before.Count - 1).ToList(), SlotRef.InParty(0)).Should().Be(before[^1].Slot);
        SlotImages.FirstUntargetedChange(before.Take(before.Count - 1).ToList(), before, SlotRef.InParty(0)).Should().Be(before[^1].Slot);
    }
}
