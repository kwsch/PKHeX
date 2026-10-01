using Bunit;
using FluentAssertions;
using Microsoft.AspNetCore.Components.Web;
using PKHeX.Web.Components;
using PKHeX.Web.Services;
using PKHeX.Web.State;
using Xunit;

namespace PKHeX.Web.Tests;

/// <summary>
/// The slot grid is one tab stop with arrow-key movement, and every slot is named by its position and contents (WEB-A11Y-001/002).
/// </summary>
[Trait(TestCategory.Name, TestCategory.Unit)]
public sealed class SlotGridTests : IDisposable
{
    private readonly BunitContext context = new();

    public SlotGridTests()
    {
        context.JSInterop.Mode = JSRuntimeMode.Loose;
        SpriteFixtures.AddCatalog(context);
    }

    public void Dispose() => context.Dispose();

    /// <summary>A 30-slot box with an entity in slots 0 and 7.</summary>
    private static IReadOnlyList<SlotSummary> Box() => Enumerable.Range(0, 30)
        .Select(i => i is 0 or 7
            ? new SlotSummary(SlotRef.InBox(0, i), true, true, 263, null, false, false)
            : new SlotSummary(SlotRef.InBox(0, i), false, true, 0, null, false, false))
        .ToArray();

    private IRenderedComponent<SlotGrid> Render(IReadOnlyList<SlotSummary> slots, SlotRef? selected = null, Action<SlotSummary>? activated = null)
        => context.Render<SlotGrid>(p => p
            .Add(c => c.Id, "box-grid")
            .Add(c => c.Label, "Box 1")
            .Add(c => c.Slots, slots)
            .Add(c => c.Columns, SlotText.BoxColumns)
            .Add(c => c.Selected, selected)
            .Add(c => c.OnActivate, s => activated?.Invoke(s)));

    private static int TabStop(IRenderedComponent<SlotGrid> grid)
    {
        var stops = grid.FindAll("button[tabindex='0']");
        stops.Should().ContainSingle("the grid is a single tab stop");
        return int.Parse(stops[0].GetAttribute("data-slot")!);
    }

    private int FocusCalls() => context.JSInterop.Invocations.Count(i => i.Identifier == "Blazor._internal.domWrapper.focus");

    [Fact]
    public void RendersRowsOfSlotsNamedByPositionAndContents()
    {
        var grid = Render(Box());

        grid.Find("[role=grid]").GetAttribute("aria-label").Should().Be("Box 1");
        grid.FindAll("[role=row]").Should().HaveCount(5);
        grid.FindAll("[role=row]").Should().OnlyContain(r => r.QuerySelectorAll("[role=gridcell]").Length == 6);
        var buttons = grid.FindAll("button");
        buttons.Should().HaveCount(30);
        buttons[0].GetAttribute("aria-label").Should().Be("Box 1, slot 1 (row 1, column 1): Zigzagoon");
        buttons[0].TextContent.Trim().Should().Be("Zigzagoon");
        buttons[1].GetAttribute("aria-label").Should().Be("Box 1, slot 2 (row 1, column 2): Empty");
        buttons[1].ClassList.Should().Contain("slot-empty");
        TabStop(grid).Should().Be(0);
    }

    [Theory]
    [InlineData(0, "ArrowRight", false, 1)]
    [InlineData(5, "ArrowRight", false, 5)]
    [InlineData(6, "ArrowLeft", false, 6)]
    [InlineData(7, "ArrowLeft", false, 6)]
    [InlineData(1, "ArrowDown", false, 7)]
    [InlineData(25, "ArrowDown", false, 25)]
    [InlineData(7, "ArrowUp", false, 1)]
    [InlineData(1, "ArrowUp", false, 1)]
    [InlineData(9, "Home", false, 6)]
    [InlineData(9, "End", false, 11)]
    [InlineData(9, "Home", true, 0)]
    [InlineData(9, "End", true, 29)]
    [InlineData(9, "a", false, 9)]
    [InlineData(9, "Alt+ArrowRight", false, 9)]
    [InlineData(9, "Meta+ArrowDown", false, 9)]
    public void NavigationKeysMoveTheTabStopWithinTheGrid(int from, string key, bool ctrl, int expected)
    {
        var grid = Render(Box());
        grid.Find($"#box-grid-{from}").Click();
        TabStop(grid).Should().Be(from);
        var focusBefore = FocusCalls();

        // "Alt+" and "Meta+" prefixes in the data stand for the modifier held with the key.
        var args = new KeyboardEventArgs { Key = key.Split('+')[^1], CtrlKey = ctrl, AltKey = key.StartsWith("Alt+"), MetaKey = key.StartsWith("Meta+") };
        grid.Find($"#box-grid-{from}").KeyDown(args);

        TabStop(grid).Should().Be(expected);
        FocusCalls().Should().Be(focusBefore + (expected == from ? 0 : 1), "focus follows the tab stop");
    }

    [Fact]
    public void ActivationReportsTheSlotWhateverItHolds()
    {
        var activated = new List<SlotSummary>();
        var slots = Box();
        var grid = Render(slots, activated: activated.Add);

        grid.Find("#box-grid-7").Click();
        grid.Find("#box-grid-8").Click();

        activated.Should().Equal(slots[7], slots[8]);
        TabStop(grid).Should().Be(8);
    }

    [Fact]
    public void SelectedSlotIsMarked()
    {
        var grid = Render(Box(), SlotRef.InBox(0, 7));
        var selected = grid.FindAll("[role=gridcell][aria-selected=true]");
        selected.Should().ContainSingle().Which.QuerySelector("button")!.Id.Should().Be("box-grid-7");
        grid.FindAll("[role=gridcell][aria-selected=false]").Should().HaveCount(29);
    }

    [Fact]
    public void TabStopStaysInRangeWhenTheGridShrinks()
    {
        var grid = Render(Box());
        grid.Find("#box-grid-29").Click();
        var party = Enumerable.Range(0, 6).Select(i => new SlotSummary(SlotRef.InParty(i), false, true, 0, null, false, false)).ToArray();

        grid.Render(p => p.Add(c => c.Slots, party).Add(c => c.Columns, SlotText.PartyColumns));

        TabStop(grid).Should().Be(5);
        grid.FindAll("[role=row]").Should().HaveCount(3);

        // Buttons kept across the re-render must still be focusable from the keyboard.
        grid.Find("#box-grid-0").Click();
        grid.Find("#box-grid-0").KeyDown(new KeyboardEventArgs { Key = "ArrowRight" });
        TabStop(grid).Should().Be(1);
        grid.Find("#box-grid-1").KeyDown(new KeyboardEventArgs { Key = "ArrowDown" });
        TabStop(grid).Should().Be(3);
    }

    [Fact]
    public void NewGridPutsItsTabStopOnTheOpenSlot()
    {
        TabStop(Render(Box(), SlotRef.InBox(0, 7))).Should().Be(7);
        TabStop(Render(Box(), SlotRef.InParty(0))).Should().Be(0, "the open slot is not in this grid");
    }
}
