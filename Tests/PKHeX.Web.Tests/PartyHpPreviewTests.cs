using System.Globalization;
using AngleSharp.Diffing.Core;
using AngleSharp.Dom;
using Bunit;
using FluentAssertions;
using PKHeX.Web.Components;
using PKHeX.Web.State;
using Xunit;

namespace PKHeX.Web.Tests;

/// <summary>
/// The preview of a party member's HP reduction before apply (WEB-PKM-014): shown only when HP is lowered, never claiming a heal.
/// </summary>
[Trait(TestCategory.Name, TestCategory.Unit)]
public sealed class PartyHpPreviewTests : IDisposable
{
    private readonly BunitContext context = new();

    public void Dispose() => context.Dispose();

    private IRenderedComponent<PartyHpPreview> Render(PartyHpChange? change) => context.Render<PartyHpPreview>(p => p.Add(c => c.Change, change));

    [Fact]
    public void NothingIsShownWithoutAReduction()
    {
        var empty = Render(null);
        empty.FindAll("#party-hp-preview").Should().BeEmpty();
        empty.Find("#party-hp-region").TextContent.Trim().Should().BeEmpty();
        Render(new PartyHpChange(7, 7, 40, 60)).FindAll("#party-hp-preview").Should().BeEmpty("a higher maximum alone lowers nothing");
        Render(new PartyHpChange(20, 20, 40, 40)).FindAll("#party-hp-preview").Should().BeEmpty();
    }

    [Fact]
    public void AReductionIsAnnouncedWithBothValues()
    {
        var preview = Render(new PartyHpChange(40, 12, 40, 12)).Find("#party-hp-preview");

        preview.TextContent.Should().Be("Applying this draft lowers current HP from 40 to 12. Maximum HP goes from 40 to 12. Its status is kept, and nothing is healed.");
    }

    [Fact]
    public void TheRegionIsPresentBeforeItsTextArrivesAndIsNotLive()
    {
        // The preview follows typed level, IV and EV edits, so it is read with Apply rather than announced on every keystroke.
        var change = new PartyHpChange(40, 12, 40, 12);
        var rendered = Render(null);
        rendered.Find("#party-hp-region").HasAttribute("role").Should().BeFalse();
        var before = rendered.Markup;

        rendered.Render(p => p.Add(c => c.Change, change));

        // The only change is the preview added inside the region: the region itself is neither removed nor replaced.
        var added = rendered.CompareTo(before).Should().ContainSingle().Which.Should().BeOfType<UnexpectedNodeDiff>().Subject;
        var node = added.Test.Node.Should().BeAssignableTo<IElement>().Subject;
        node.Id.Should().Be("party-hp-preview");
        node.ParentElement!.Id.Should().Be("party-hp-region");
        rendered.Find("#party-hp-region #party-hp-preview").TextContent.Should().StartWith("Applying this draft lowers current HP from 40 to 12.");
    }

    [Fact]
    public void ALoweredCurrentHpWithTheSameMaximumDoesNotMentionTheMaximum()
    {
        PartyText.HpPreview(new PartyHpChange(30, 10, 40, 40)).Should().Be("Applying this draft lowers current HP from 30 to 10. Its status is kept, and nothing is healed.");
    }

    [Fact]
    public void AFaintingReductionSaysSo()
    {
        PartyText.HpPreview(new PartyHpChange(5, 0, 40, 40)).Should().StartWith("Applying this draft lowers current HP from 5 to 0: this Pokémon will be fainted.");
    }

    [Fact]
    public void NumbersAreInvariant()
    {
        var culture = CultureInfo.CurrentCulture;
        try
        {
            CultureInfo.CurrentCulture = new CultureInfo("ar-SA");
            PartyText.HpPreview(new PartyHpChange(1000, 999, 1000, 999)).Should().Contain("from 1000 to 999");
        }
        finally
        {
            CultureInfo.CurrentCulture = culture;
        }
    }
}
