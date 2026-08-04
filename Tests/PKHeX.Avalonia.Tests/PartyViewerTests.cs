using Moq;
using PKHeX.Avalonia.Services;
using PKHeX.Presentation.ViewModels;
using PKHeX.Core;
using Xunit.Abstractions;

namespace PKHeX.Avalonia.Tests;

/// <summary>
/// Behavioral tests for PartyViewerViewModel.
/// Party always loads 6 slots; empty slots have IsEmpty=true.
/// </summary>
public class PartyViewerTests(ITestOutputHelper output)
{
    private static Mock<ISpriteRenderer> SpriteMock() => new();

    // -----------------------------------------------------------------------
    // 1. Always creates exactly 6 slots
    // -----------------------------------------------------------------------

    [Theory]
    [InlineData(GameVersion.E,  "Gen3-Emerald")]
    [InlineData(GameVersion.Pt, "Gen4-Platinum")]
    [InlineData(GameVersion.X,  "Gen6-X")]
    [InlineData(GameVersion.SL, "Gen9-Scarlet")]
    public void PartyViewer_AlwaysHas6Slots(GameVersion version, string label)
    {
        var sav = BlankSaveFile.Get(version);
        var vm = new PartyViewerViewModel(sav, SpriteMock().Object);

        Assert.Equal(6, vm.Slots.Count);
        output.WriteLine($"{label}: 6 party slots ✓");
    }

    // -----------------------------------------------------------------------
    // 2. Blank save: all slots are empty
    // -----------------------------------------------------------------------

    [Fact]
    public void PartyViewer_BlankSave_AllSlotsEmpty()
    {
        var sav = new SAV6XY();
        var vm = new PartyViewerViewModel(sav, SpriteMock().Object);

        Assert.All(vm.Slots, slot => Assert.True(slot.IsEmpty, $"Slot {slot.Slot} should be empty"));
        output.WriteLine("Gen6 blank save: all 6 party slots empty ✓");
    }

    // -----------------------------------------------------------------------
    // 3. Slot with PKM has IsEmpty=false and correct species
    // -----------------------------------------------------------------------

    [Fact]
    public void PartyViewer_SlotWithPKM_IsNotEmpty()
    {
        var sav = new SAV6XY();
        var pk = new PK6 { Species = 25 }; // Pikachu
        pk.CurrentLevel = 50;
        sav.SetPartySlotAtIndex(pk, 0);

        var vm = new PartyViewerViewModel(sav, SpriteMock().Object);

        Assert.False(vm.Slots[0].IsEmpty);
        Assert.Equal(25, vm.Slots[0].Species);
        Assert.Equal(50, vm.Slots[0].Level);
        output.WriteLine($"Gen6: slot 0 has Pikachu (species={vm.Slots[0].Species}, level={vm.Slots[0].Level}) ✓");
    }

    // -----------------------------------------------------------------------
    // 4. SelectSlotByClick updates SelectedIndex and IsSelected
    // -----------------------------------------------------------------------

    [Fact]
    public void PartyViewer_SelectSlotByClick_UpdatesSelection()
    {
        var sav = new SAV6XY();
        var vm = new PartyViewerViewModel(sav, SpriteMock().Object);

        // Start at slot 1 to ensure a real change fires
        vm.SelectedIndex = 1;
        Assert.True(vm.Slots[1].IsSelected);
        Assert.False(vm.Slots[3].IsSelected);

        // Click slot 3
        vm.SelectSlotByClickCommand.Execute(vm.Slots[3]);

        Assert.Equal(3, vm.SelectedIndex);
        Assert.True(vm.Slots[3].IsSelected);
        Assert.False(vm.Slots[1].IsSelected);
        output.WriteLine("SelectSlotByClick: slot 3 selected, slot 1 deselected ✓");
    }

    // -----------------------------------------------------------------------
    // 5. MoveSelection Up/Down navigates correctly
    // -----------------------------------------------------------------------

    [Fact]
    public void PartyViewer_MoveSelection_Up_DecreasesIndex()
    {
        var sav = new SAV6XY();
        var vm = new PartyViewerViewModel(sav, SpriteMock().Object);

        vm.SelectedIndex = 3;
        vm.MoveSelectionCommand.Execute("Up");

        Assert.Equal(2, vm.SelectedIndex);
        output.WriteLine("MoveSelection Up: 3 → 2 ✓");
    }

    [Fact]
    public void PartyViewer_MoveSelection_Down_IncreasesIndex()
    {
        var sav = new SAV6XY();
        var vm = new PartyViewerViewModel(sav, SpriteMock().Object);

        vm.SelectedIndex = 2;
        vm.MoveSelectionCommand.Execute("Down");

        Assert.Equal(3, vm.SelectedIndex);
        output.WriteLine("MoveSelection Down: 2 → 3 ✓");
    }

    [Fact]
    public void PartyViewer_MoveSelection_AtBoundary_Clamps()
    {
        var sav = new SAV6XY();
        var vm = new PartyViewerViewModel(sav, SpriteMock().Object);

        // At top — can't go further up
        vm.SelectedIndex = 0;
        vm.MoveSelectionCommand.Execute("Up");
        Assert.Equal(0, vm.SelectedIndex);

        // At bottom — can't go further down
        vm.SelectedIndex = 5;
        vm.MoveSelectionCommand.Execute("Down");
        Assert.Equal(5, vm.SelectedIndex);
        output.WriteLine("MoveSelection boundary clamp ✓");
    }

    // -----------------------------------------------------------------------
    // 6. ActivateSlot fires SlotActivated event
    // -----------------------------------------------------------------------

    [Fact]
    public void PartyViewer_ActivateSlot_FiresEvent()
    {
        var sav = new SAV6XY();
        var vm = new PartyViewerViewModel(sav, SpriteMock().Object);

        int activatedSlot = -1;
        vm.SlotActivated += slot => activatedSlot = slot;

        vm.SelectedIndex = 2;
        vm.ActivateSlotCommand.Execute(null);

        Assert.Equal(2, activatedSlot);
        output.WriteLine("ActivateSlot: SlotActivated event fired with slot=2 ✓");
    }

    // -----------------------------------------------------------------------
    // 7. RefreshParty reloads without throwing
    // -----------------------------------------------------------------------

    [Fact]
    public void PartyViewer_RefreshParty_DoesNotThrow()
    {
        var sav = new SAV6XY();
        var vm = new PartyViewerViewModel(sav, SpriteMock().Object);

        var ex = Record.Exception(() => vm.RefreshParty());
        Assert.Null(ex);
        Assert.Equal(6, vm.Slots.Count);
        output.WriteLine("RefreshParty: no exception, 6 slots ✓");
    }

    // -----------------------------------------------------------------------
    // 8. GetSlotPKM returns the PKM from save
    // -----------------------------------------------------------------------

    [Fact]
    public void PartyViewer_GetSlotPKM_ReturnsCorrectPKM()
    {
        var sav = new SAV6XY();
        var pk = new PK6 { Species = 7 }; // Squirtle
        sav.SetPartySlotAtIndex(pk, 0);

        var vm = new PartyViewerViewModel(sav, SpriteMock().Object);
        var result = vm.GetSlotPKM(0);

        Assert.Equal(7, result.Species);
        output.WriteLine($"GetSlotPKM(0): Species={result.Species} (Squirtle) ✓");
    }
}
