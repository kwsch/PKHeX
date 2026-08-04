using Moq;
using PKHeX.Avalonia.Services;
using PKHeX.Presentation.ViewModels;
using PKHeX.Core;
using Xunit.Abstractions;

namespace PKHeX.Avalonia.Tests;

/// <summary>
/// Behavioral tests for BoxViewerViewModel and BoxListEditorViewModel.
/// BoxViewer loads one box at a time; navigation changes CurrentBox.
/// BoxListEditor shows a summary of all boxes.
/// </summary>
public class BoxViewerTests(ITestOutputHelper output)
{
    private static Mock<ISpriteRenderer> SpriteMock() => new();

    // -----------------------------------------------------------------------
    // BoxViewerViewModel
    // -----------------------------------------------------------------------

    [Fact]
    public void BoxViewer_Constructs_WithCorrectBoxCount()
    {
        var sav = new SAV6XY();
        var vm = new BoxViewerViewModel(sav, SpriteMock().Object);

        Assert.Equal(sav.BoxCount, vm.BoxCount);
        Assert.Equal(sav.BoxSlotCount, vm.SlotsPerBox);
        Assert.Equal(sav.BoxSlotCount, vm.Slots.Count);
        output.WriteLine($"Gen6: BoxCount={vm.BoxCount}, SlotsPerBox={vm.SlotsPerBox}, Slots={vm.Slots.Count} ✓");
    }

    [Fact]
    public void BoxViewer_BlankSave_AllSlotsEmpty()
    {
        var sav = new SAV6XY();
        var vm = new BoxViewerViewModel(sav, SpriteMock().Object);

        Assert.All(vm.Slots, slot => Assert.True(slot.IsEmpty, $"Slot {slot.Slot} should be empty"));
        output.WriteLine($"Gen6 blank save: all {vm.Slots.Count} slots empty ✓");
    }

    [Fact]
    public void BoxViewer_SlotWithPKM_IsNotEmpty()
    {
        var sav = new SAV6XY();
        sav.SetBoxSlotAtIndex(new PK6 { Species = 25 }, 0); // Pikachu in box 0, slot 0

        var vm = new BoxViewerViewModel(sav, SpriteMock().Object);

        Assert.False(vm.Slots[0].IsEmpty);
        Assert.Equal(25, vm.Slots[0].Species);
        output.WriteLine($"Gen6: slot 0 has Pikachu (species={vm.Slots[0].Species}) ✓");
    }

    [Fact]
    public void BoxViewer_NextBox_AdvancesCurrentBox()
    {
        var sav = new SAV6XY();
        var vm = new BoxViewerViewModel(sav, SpriteMock().Object);

        Assert.Equal(0, vm.CurrentBox);
        vm.NextBoxCommand.Execute(null);
        Assert.Equal(1, vm.CurrentBox);
        output.WriteLine($"NextBox: CurrentBox={vm.CurrentBox} ✓");
    }

    [Fact]
    public void BoxViewer_PrevBox_DecreasesCurrentBox()
    {
        var sav = new SAV6XY();
        var vm = new BoxViewerViewModel(sav, SpriteMock().Object);

        vm.NextBoxCommand.Execute(null); // go to box 1
        vm.PreviousBoxCommand.Execute(null); // go back to box 0

        Assert.Equal(0, vm.CurrentBox);
        output.WriteLine("PrevBox: back to CurrentBox=0 ✓");
    }

    [Fact]
    public void BoxViewer_PrevBox_AtFirst_WrapsToLast()
    {
        var sav = new SAV6XY();
        var vm = new BoxViewerViewModel(sav, SpriteMock().Object);

        // At box 0, PreviousBox wraps to the last box
        vm.PreviousBoxCommand.Execute(null);
        Assert.Equal(sav.BoxCount - 1, vm.CurrentBox);
        output.WriteLine($"PreviousBox at box 0: wraps to last box ({sav.BoxCount - 1}) ✓");
    }

    [Fact]
    public void BoxViewer_NextBox_AtLast_WrapsToFirst()
    {
        var sav = new SAV6XY();
        var vm = new BoxViewerViewModel(sav, SpriteMock().Object);

        // Navigate to last box
        for (int i = 0; i < sav.BoxCount - 1; i++)
            vm.NextBoxCommand.Execute(null);

        Assert.Equal(sav.BoxCount - 1, vm.CurrentBox);
        vm.NextBoxCommand.Execute(null);
        Assert.Equal(0, vm.CurrentBox); // wraps to first
        output.WriteLine($"NextBox at last box: wraps to box 0 ✓");
    }

    [Fact]
    public void BoxViewer_LoadBoxSwitchesSlots()
    {
        var sav = new SAV6XY();
        // Put Pikachu in box 0, slot 0; Bulbasaur in box 1, slot 0
        sav.SetBoxSlotAtIndex(new PK6 { Species = 25 }, 0, 0);
        sav.SetBoxSlotAtIndex(new PK6 { Species = 1  }, 1, 0);

        var vm = new BoxViewerViewModel(sav, SpriteMock().Object);

        Assert.Equal(25, vm.Slots[0].Species); // box 0 shows Pikachu

        vm.NextBoxCommand.Execute(null); // go to box 1

        Assert.Equal(1, vm.Slots[0].Species); // box 1 shows Bulbasaur
        output.WriteLine("BoxViewer: switching boxes updates slots ✓");
    }

    [Theory]
    [InlineData(GameVersion.E,  "Gen3-Emerald")]
    [InlineData(GameVersion.Pt, "Gen4-Platinum")]
    [InlineData(GameVersion.SL, "Gen9-Scarlet")]
    public void BoxViewer_VariousGens_Constructs(GameVersion version, string label)
    {
        var sav = BlankSaveFile.Get(version);
        var ex = Record.Exception(() =>
        {
            var vm = new BoxViewerViewModel(sav, SpriteMock().Object);
            Assert.Equal(sav.BoxSlotCount, vm.Slots.Count);
        });
        Assert.Null(ex);
        output.WriteLine($"{label}: BoxViewer constructs OK ✓");
    }

    // -----------------------------------------------------------------------
    // In-save seek (detached tool, driven through BoxViewerViewModel.Seek)
    // -----------------------------------------------------------------------

    [Fact]
    public void BoxViewer_Seek_DrivesBoxNavigationToMatch()
    {
        var sav = new SAV6XY();
        // Pikachu sits in box 2, slot 5; nothing else matches.
        sav.SetBoxSlotAtIndex(new PK6 { Species = 25 }, 2, 5);

        var vm = new BoxViewerViewModel(sav, SpriteMock().Object);
        vm.Seek.Filter.Species = 25;

        vm.Seek.SeekNextCommand.Execute(null);

        // The seek tool navigated the box viewer to the matching slot.
        Assert.Equal(2, vm.CurrentBox);
        Assert.Equal(5, vm.SelectedIndex);
        output.WriteLine($"Seek drove box view to box {vm.CurrentBox}, slot {vm.SelectedIndex} ✓");
    }

    // -----------------------------------------------------------------------
    // BoxListEditorViewModel
    // -----------------------------------------------------------------------

    [Fact]
    public void BoxListEditor_BoxCount_MatchesSave()
    {
        var sav = new SAV6XY();
        var vm = new BoxListEditorViewModel(sav);

        Assert.Equal(sav.BoxCount, vm.BoxCount);
        Assert.Equal(sav.BoxCount, vm.Boxes.Count);
        output.WriteLine($"Gen6: BoxListEditor has {vm.Boxes.Count} boxes ✓");
    }

    [Fact]
    public void BoxListEditor_BlankSave_AllBoxesEmpty()
    {
        var sav = new SAV6XY();
        var vm = new BoxListEditorViewModel(sav);

        Assert.All(vm.Boxes, box => Assert.Equal(0, box.OccupiedSlots));
        output.WriteLine("Gen6 blank save: all boxes show 0 occupied ✓");
    }

    [Fact]
    public void BoxListEditor_OccupiedCount_ReflectsBoxContent()
    {
        var sav = new SAV6XY();
        // Put 3 Pokémon in box 0
        sav.SetBoxSlotAtIndex(new PK6 { Species = 1 }, 0, 0);
        sav.SetBoxSlotAtIndex(new PK6 { Species = 4 }, 0, 1);
        sav.SetBoxSlotAtIndex(new PK6 { Species = 7 }, 0, 2);

        var vm = new BoxListEditorViewModel(sav);

        Assert.Equal(3, vm.Boxes[0].OccupiedSlots);
        Assert.Equal(0, vm.Boxes[1].OccupiedSlots);
        output.WriteLine($"Gen6: box 0 has {vm.Boxes[0].OccupiedSlots} occupied, box 1 has {vm.Boxes[1].OccupiedSlots} ✓");
    }

    [Fact]
    public void BoxListEditor_RefreshCommand_ReloadsBoxes()
    {
        var sav = new SAV6XY();
        var vm = new BoxListEditorViewModel(sav);

        var initialCount = vm.Boxes[0].OccupiedSlots;
        Assert.Equal(0, initialCount);

        // Add a PKM directly to save then refresh
        sav.SetBoxSlotAtIndex(new PK6 { Species = 25 }, 0, 0);
        vm.RefreshCommand.Execute(null);

        Assert.Equal(1, vm.Boxes[0].OccupiedSlots);
        output.WriteLine("BoxListEditor Refresh: detects new PKM ✓");
    }

    [Fact]
    public void BoxListEditor_ClearSelectedBox_RemovesPKM()
    {
        var sav = new SAV6XY();
        sav.SetBoxSlotAtIndex(new PK6 { Species = 1 }, 0, 0);
        sav.SetBoxSlotAtIndex(new PK6 { Species = 4 }, 0, 1);

        var vm = new BoxListEditorViewModel(sav);
        Assert.Equal(2, vm.Boxes[0].OccupiedSlots);

        vm.SelectedBox = vm.Boxes[0];
        vm.ClearSelectedBoxCommand.Execute(null);

        Assert.Equal(0, vm.Boxes[0].OccupiedSlots);
        output.WriteLine("BoxListEditor ClearSelectedBox: box 0 cleared ✓");
    }
}
