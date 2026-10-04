using FluentAssertions;
using PKHeX.Core;
using PKHeX.Web.Components;
using PKHeX.Web.Services;
using Xunit;

namespace PKHeX.Web.Tests;

/// <summary>Where legality findings link to in the inspector.</summary>
[Trait(TestCategory.Name, TestCategory.Unit)]
public sealed class LegalitySectionsTests
{
    [Fact]
    public void EveryCoreCheckIdentifierIsMapped()
    {
        // A new Core identifier throws until it is mapped (to a section, or explicitly to none).
        foreach (var identifier in Enum.GetValues<CheckIdentifier>())
        {
            var act = () => LegalitySections.For(identifier);
            act.Should().NotThrow($"{identifier} must be mapped");
        }
    }

    [Theory]
    [InlineData(CheckIdentifier.CurrentMove, InspectorArea.Moves)]
    [InlineData(CheckIdentifier.IVs, InspectorArea.Stats)]
    [InlineData(CheckIdentifier.EVs, InspectorArea.Stats)]
    [InlineData(CheckIdentifier.Ball, InspectorArea.Origin)]
    [InlineData(CheckIdentifier.Encounter, InspectorArea.Origin)]
    [InlineData(CheckIdentifier.Nickname, InspectorArea.Identity)]
    [InlineData(CheckIdentifier.HeldItem, InspectorArea.Identity)]
    [InlineData(CheckIdentifier.PID, InspectorArea.Advanced)]
    [InlineData(CheckIdentifier.Ribbon, InspectorArea.Advanced)]
    public void ChecksPointAtTheSectionShowingTheirValues(CheckIdentifier identifier, InspectorArea area) =>
        LegalitySections.For(identifier).Should().Be(area);

    [Theory]
    [InlineData(CheckIdentifier.RelearnMove)]
    [InlineData(CheckIdentifier.Memory)]
    [InlineData(CheckIdentifier.Misc)]
    public void ChecksTheInspectorDoesNotShowPointNowhere(CheckIdentifier identifier) => LegalitySections.For(identifier).Should().BeNull();

    [Fact]
    public void EverySectionLinkTargetsAnInspectorHeading()
    {
        var session = SaveFixtures.Open(SaveFixtures.Synthetic(false));
        var headings = InspectorText.Sections(session.Select(SaveFixtures.FirstBoxSlot).Inspect()).Select(s => (s.Id, s.Title)).ToList();
        foreach (var identifier in Enum.GetValues<CheckIdentifier>())
        {
            if (LegalityText.Section(identifier) is { } section)
            {
                headings.Should().Contain(section, $"{identifier} links to a real inspector heading");
            }
        }
        Enum.GetValues<InspectorArea>().Should().HaveCount(headings.Count, "each inspector section is an area");
    }
}
