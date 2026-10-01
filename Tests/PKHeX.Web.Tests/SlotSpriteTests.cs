using AngleSharp.Dom;
using Bunit;
using FluentAssertions;
using PKHeX.Web.Components;
using PKHeX.Web.Services;
using PKHeX.Web.Services.Sprites;
using PKHeX.Web.State;
using Xunit;

namespace PKHeX.Web.Tests;

/// <summary>
/// Sprites are decorative layers drawn from the preloaded atlas; the text and accessible names stay as they are (WEB-BOX-004, WEB-A11Y-001).
/// </summary>
[Trait(TestCategory.Name, TestCategory.Unit)]
public sealed class SlotSpriteTests : IDisposable
{
    private readonly BunitContext context = new();

    public SlotSpriteTests() => context.JSInterop.Mode = JSRuntimeMode.Loose;

    public void Dispose() => context.Dispose();

    private static SlotSummary Occupied(int index, ushort species, byte form = 0, bool shiny = false, bool egg = false, bool item = false)
        => new(SlotRef.InBox(0, index), true, true, species, null, egg, shiny, form, 0, item);

    private static SlotSummary[] Box(params SlotSummary[] occupied) => [.. Enumerable.Range(0, 30)
        .Select(i => occupied.FirstOrDefault(s => s.Ref == SlotRef.InBox(0, i)) ?? new SlotSummary(SlotRef.InBox(0, i), false, true, 0, null, false, false))];

    private IRenderedComponent<SlotGrid> Grid(IReadOnlyList<SlotSummary> slots) => context.Render<SlotGrid>(p => p
        .Add(c => c.Id, "box-grid")
        .Add(c => c.Label, "Box 1")
        .Add(c => c.Slots, slots)
        .Add(c => c.Columns, SlotText.BoxColumns));

    private static string CellClass(string name) => $"sprite-c{SpriteFixtures.Manifest().Sprites[name].Cell}";

    [Fact]
    public void WithoutSpritesSlotsAreText()
    {
        SpriteFixtures.AddCatalog(context);
        var slot = Occupied(0, 25);

        var grid = Grid(Box(slot));

        grid.FindAll("img, .sprite").Should().BeEmpty();
        var button = grid.Find("#box-grid-0");
        button.TextContent.Trim().Should().Be(SlotText.Short(slot));
        button.GetAttribute("aria-label").Should().Be(SlotText.Label(slot));
        button.GetAttribute("title").Should().Be(SlotText.Label(slot));
    }

    [Fact]
    public void ASpriteIsDecorativeAndKeepsTheText()
    {
        SpriteFixtures.AddCatalog(context, SpriteFixtures.Sheet());
        var slot = Occupied(0, 25);

        var grid = Grid(Box(slot));

        var button = grid.Find("#box-grid-0");
        var sprite = button.QuerySelector(".sprite")!;
        sprite.GetAttribute("aria-hidden").Should().Be("true");
        Images(sprite).Should().ContainSingle().Which.Should().Satisfy<IElement>(img =>
        {
            img.GetAttribute("alt").Should().BeEmpty();
            img.GetAttribute("src").Should().Be("sprites/" + SpriteFixtures.Atlas.AtlasFileName);
            img.ClassList.Should().Equal(CellClass("b_25"));
        });
        button.TextContent.Trim().Should().Be(SlotText.Short(slot));
        button.GetAttribute("aria-label").Should().Be(SlotText.Label(slot));
        grid.FindAll(".sprite").Should().ContainSingle("empty slots have no sprite");
    }

    [Fact]
    public void LayersFollowTheDesktop()
    {
        SpriteFixtures.AddCatalog(context, SpriteFixtures.Sheet());

        var grid = Grid(Box(
            Occupied(0, 25, form: 1, shiny: true),
            Occupied(1, 1, form: 5),
            Occupied(2, 490, egg: true),
            Occupied(3, 25, egg: true, item: true),
            Occupied(4, 1, form: 5, egg: true, item: true)));

        Classes(grid, 0).Should().Equal(CellClass("b_25_1cs"), $"{CellClass("rare_icon_alt")} sprite-star");
        Classes(grid, 1).Should().Equal(CellClass("b_1"), $"{CellClass("b_unknown")} sprite-unknown");
        Classes(grid, 2).Should().Equal(CellClass("b_490"), $"{CellClass("b_490_e")} sprite-egg-item");
        Classes(grid, 3).Should().Equal(CellClass("b_25"), $"{CellClass("b_egg")} sprite-egg");
        // The desktop fades the species image after laying the unknown mark on it, so both are faded together, as a group.
        Classes(grid, 4).Should().Equal(CellClass("b_1"), $"{CellClass("b_unknown")} sprite-unknown", $"{CellClass("b_egg")} sprite-egg");
        Enumerable.Range(0, 5).Select(i => grid.Find($"#box-grid-{i} .sprite-species").ClassList.Contains("sprite-faded"))
            .Should().Equal(false, false, false, true, true);
        grid.Find("#box-grid-4 .sprite-faded").QuerySelectorAll("img").Should().HaveCount(2, "the species and its unknown mark");
    }

    [Fact]
    public void ABadEggHasNoSprite()
    {
        SpriteFixtures.AddCatalog(context, SpriteFixtures.Sheet());

        var grid = Grid(Box(new SlotSummary(SlotRef.InBox(0, 0), true, false, 0, null, false, false)));

        grid.FindAll(".sprite").Should().BeEmpty();
    }

    [Fact]
    public void TheListViewShowsTheSameSprite()
    {
        SpriteFixtures.AddCatalog(context, SpriteFixtures.Sheet());

        var list = context.Render<SlotList>(p => p
            .Add(c => c.Id, "box-list")
            .Add(c => c.Caption, "Box 1")
            .Add(c => c.Slots, Box(Occupied(0, 25))));

        list.FindAll(".sprite").Should().ContainSingle().Which.GetAttribute("aria-hidden").Should().Be("true");
        list.Find(".sprite img").ClassList.Should().Equal(CellClass("b_25"));
    }

    private static IElement[] Images(IElement sprite) => [.. sprite.QuerySelectorAll("img")];

    /// <summary>Each layer's classes, bottom layer first.</summary>
    private static string[] Classes(IRenderedComponent<SlotGrid> grid, int index)
        => [.. Images(grid.Find($"#box-grid-{index} .sprite")).Select(img => string.Join(' ', img.ClassList))];
}
