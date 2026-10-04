using System.Globalization;
using FluentAssertions;
using PKHeX.Web.Components;
using PKHeX.Web.Services;
using PKHeX.Web.State;
using Xunit;

namespace PKHeX.Web.Tests;

/// <summary>
/// Slot and box text is built from coordinates and contents only, in any browser locale.
/// </summary>
[Trait(TestCategory.Name, TestCategory.Unit)]
public sealed class SlotTextTests
{
    private static SlotSummary Holding(SlotRef slot, ushort species = 263, string? nickname = null, bool egg = false, bool shiny = false)
        => new(slot, true, true, species, nickname, egg, shiny);

    [Theory]
    [InlineData(0, 0, "Box 1, slot 1 (row 1, column 1)")]
    [InlineData(0, 5, "Box 1, slot 6 (row 1, column 6)")]
    [InlineData(2, 6, "Box 3, slot 7 (row 2, column 1)")]
    [InlineData(30, 29, "Box 31, slot 30 (row 5, column 6)")]
    public void BoxPositionsNameBoxSlotRowAndColumn(int box, int slot, string expected)
        => SlotText.Position(SlotRef.InBox(box, slot)).Should().Be(expected);

    [Fact]
    public void PartyPositionsAreNumbered() => SlotText.Position(SlotRef.InParty(1)).Should().Be("Party position 2");

    [Fact]
    public void ContentsDescribeEveryKindOfSlot()
    {
        var slot = SlotRef.InBox(0, 0);
        SlotText.Contents(new SlotSummary(slot, false, true, 0, null, false, false)).Should().Be("Empty");
        SlotText.Contents(new SlotSummary(slot, true, false, 0, null, false, false)).Should().Be("Bad egg");
        SlotText.Contents(Holding(slot)).Should().Be("Zigzagoon");
        SlotText.Contents(Holding(slot, nickname: "Ziggy")).Should().Be($"Zigzagoon \"{TestText.Isolated("Ziggy")}\"");
        SlotText.Contents(Holding(slot, nickname: "Ziggy", shiny: true)).Should().Be($"Zigzagoon \"{TestText.Isolated("Ziggy")}\", shiny");
        SlotText.Contents(Holding(slot, nickname: "Egg", egg: true)).Should().Be("Egg");
        SlotText.Contents(Holding(slot, species: 60000)).Should().Be("Unknown species (stored value 60000)");
        SlotText.Short(Holding(slot, nickname: "Ziggy", shiny: true)).Should().Be("Zigzagoon");
    }

    [Fact]
    public void LabelIsPositionThenContents()
    {
        SlotText.Label(Holding(SlotRef.InBox(2, 6), nickname: "Ziggy")).Should().Be($"Box 3, slot 7 (row 2, column 1): Zigzagoon \"{TestText.Isolated("Ziggy")}\"");
        SlotText.Label(new SlotSummary(SlotRef.InParty(5), false, true, 0, null, false, false)).Should().Be("Party position 6: Empty");
    }

    [Fact]
    public void HostileNamesCannotReorderTheLabel()
    {
        // An override in a nickname or box name would otherwise reverse the rest of the label, position and all.
        SlotText.Label(Holding(SlotRef.InBox(0, 0), nickname: "\u202Eevil\u2069")).Should().Be($"Box 1, slot 1 (row 1, column 1): Zigzagoon \"{TestText.Isolated("evil")}\"");
        SlotText.BoxTitle(0, "\u202EBox").Should().Be("Box");
        SlotText.BoxOption(0, "\u202EBox").Should().Be($"1. {TestText.Isolated("Box")}");
    }

    [Fact]
    public void ABoxNameOfOnlyControlsFallsBackToCoresDefault()
    {
        // Bidirectional controls are not whitespace, so the stored name is not blank, but nothing of it would be shown.
        SlotText.BoxTitle(2, "\u202E\u2066").Should().Be("Box 3");
        SlotText.BoxOption(2, "\u202E").Should().Be("3. Box 3");
    }

    [Fact]
    public void BoxTitlesUseStoredNamesOrCoreDefaults()
    {
        SlotText.BoxTitle(0, "Keepers").Should().Be("Keepers");
        SlotText.BoxTitle(4, null).Should().Be("Box 5");
        SlotText.BoxOption(4, null).Should().Be("5. Box 5");
        SlotText.BoxOption(30, "Keepers").Should().Be($"31. {TestText.Isolated("Keepers")}", "a stored name is isolated from the number before it");
    }

    [Theory]
    [InlineData("de-DE")]
    [InlineData("ar-SA")]
    [InlineData("hi-IN")]
    public void TextIsTheSameInEveryLocale(string culture)
    {
        var expected = ("Box 31, slot 30 (row 5, column 6)", "31. Box 31", "Unknown species (stored value 60000)");
        var previous = CultureInfo.CurrentCulture;
        try
        {
            CultureInfo.CurrentCulture = new CultureInfo(culture);
            (SlotText.Position(SlotRef.InBox(30, 29)), SlotText.BoxOption(30, null), SlotText.Contents(Holding(SlotRef.InBox(0, 0), species: 60000)))
                .Should().Be(expected);
        }
        finally
        {
            CultureInfo.CurrentCulture = previous;
        }
    }
}
