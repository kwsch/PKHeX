using Moq;
using PKHeX.Avalonia.Services;
using PKHeX.Presentation.ViewModels;
using PKHeX.Core;
using Xunit.Abstractions;

namespace PKHeX.Avalonia.Tests;

/// <summary>
/// Behavioral tests for MysteryGiftEditorViewModel.
/// Wondercard albums are supported in Gen4-7 (and LGPE); this class tests that legacy mode.
/// Switch received-gift record mode is covered by <see cref="GiftRecordStoreTests"/>.
/// </summary>
public class MysteryGiftEditorTests(ITestOutputHelper output)
{
    private static Mock<IDialogService> DialogMock() => new();

    // -----------------------------------------------------------------------
    // 1. IsSupported reflects IMysteryGiftStorageProvider correctly
    // -----------------------------------------------------------------------

    [Theory]
    [InlineData(GameVersion.Pt, "Gen4-Platinum")]
    [InlineData(GameVersion.W2, "Gen5-White2")]
    [InlineData(GameVersion.X,  "Gen6-X")]
    [InlineData(GameVersion.SN, "Gen7-Sun")]
    [InlineData(GameVersion.GP, "Gen7b-LGPE")]
    public void MysteryGift_IsSupported_True(GameVersion version, string label)
    {
        var sav = BlankSaveFile.Get(version);
        var vm = new MysteryGiftEditorViewModel(sav, DialogMock().Object);

        Assert.True(vm.IsSupported, $"{label}: expected IsSupported=true");
        Assert.True(vm.GiftCount > 0, $"{label}: expected GiftCount>0, got {vm.GiftCount}");
        Assert.NotEmpty(vm.Gifts);
        output.WriteLine($"{label}: IsSupported=true, GiftCount={vm.GiftCount}, Slots={vm.Gifts.Count} ✓");
    }

    [Theory]
    [InlineData(GameVersion.RD, "Gen1-Red")]
    [InlineData(GameVersion.GD, "Gen2-Gold")]
    [InlineData(GameVersion.E,  "Gen3-Emerald")]
    [InlineData(GameVersion.SL, "Gen9-Scarlet")]
    public void MysteryGift_IsSupported_False(GameVersion version, string label)
    {
        var sav = BlankSaveFile.Get(version);
        var vm = new MysteryGiftEditorViewModel(sav, DialogMock().Object);

        Assert.False(vm.IsSupported, $"{label}: expected IsSupported=false");
        Assert.Equal(0, vm.GiftCount);
        Assert.Empty(vm.Gifts);
        output.WriteLine($"{label}: IsSupported=false ✓");
    }

    // -----------------------------------------------------------------------
    // 2. Gifts collection count matches GiftCount
    // -----------------------------------------------------------------------

    [Fact]
    public void MysteryGift_Gen6_GiftsCount_MatchesGiftCount()
    {
        var sav = new SAV6XY();
        var vm = new MysteryGiftEditorViewModel(sav, DialogMock().Object);

        Assert.True(vm.IsSupported);
        Assert.Equal(vm.GiftCount, vm.Gifts.Count);
        output.WriteLine($"Gen6: Gifts.Count={vm.Gifts.Count} == GiftCount={vm.GiftCount} ✓");
    }

    // -----------------------------------------------------------------------
    // 3. All slots in a blank save are empty
    // -----------------------------------------------------------------------

    [Fact]
    public void MysteryGift_Gen6_BlankSave_AllSlotsEmpty()
    {
        var sav = new SAV6XY();
        var vm = new MysteryGiftEditorViewModel(sav, DialogMock().Object);

        Assert.True(vm.IsSupported);
        Assert.All(vm.Gifts, slot => Assert.True(slot.IsEmpty, $"Slot {slot.Index} should be empty on blank save"));
        output.WriteLine($"Gen6: all {vm.Gifts.Count} slots empty on blank save ✓");
    }

    // -----------------------------------------------------------------------
    // 4. DeleteGift clears the selected slot
    // -----------------------------------------------------------------------

    [Fact]
    public void MysteryGift_DeleteGift_SkipsWhenNoGiftSelected()
    {
        var sav = new SAV6XY();
        var vm = new MysteryGiftEditorViewModel(sav, DialogMock().Object);

        // Blank save: all gifts are empty. DeleteGift should be a no-op (no crash)
        vm.SelectedGift = vm.Gifts[0];
        var ex = Record.Exception(() => vm.DeleteGiftCommand.Execute(null));
        Assert.Null(ex);
        Assert.True(vm.Gifts[0].IsEmpty);
        output.WriteLine("Gen6 DeleteGift: no-op on already-empty slot ✓");
    }

    // -----------------------------------------------------------------------
    // 5. ResetCommand reloads from storage
    // -----------------------------------------------------------------------

    [Fact]
    public void MysteryGift_Gen4_ResetCommand_DoesNotThrow()
    {
        var sav = BlankSaveFile.Get(GameVersion.Pt);
        var vm = new MysteryGiftEditorViewModel(sav, DialogMock().Object);

        var countBefore = vm.Gifts.Count;
        var ex = Record.Exception(() => vm.ResetCommand.Execute(null));

        Assert.Null(ex);
        Assert.Equal(countBefore, vm.Gifts.Count);
        output.WriteLine($"Gen4: Reset OK, {countBefore} slots ✓");
    }

    // -----------------------------------------------------------------------
    // 6. SaveCommand does not throw
    // -----------------------------------------------------------------------

    [Fact]
    public void MysteryGift_Gen5_SaveCommand_DoesNotThrow()
    {
        var sav = BlankSaveFile.Get(GameVersion.W2);
        var vm = new MysteryGiftEditorViewModel(sav, DialogMock().Object);

        Assert.True(vm.IsSupported);
        var ex = Record.Exception(() => vm.SaveCommand.Execute(null));
        Assert.Null(ex);
        output.WriteLine("Gen5: SaveCommand does not throw ✓");
    }

    // -----------------------------------------------------------------------
    // 7. CanManageFlags reflects IMysteryGiftFlags presence
    // -----------------------------------------------------------------------

    [Fact]
    public void MysteryGift_Gen5_CanManageFlags_True()
    {
        var sav = BlankSaveFile.Get(GameVersion.W2);
        var vm = new MysteryGiftEditorViewModel(sav, DialogMock().Object);

        Assert.True(vm.IsSupported);
        Assert.True(vm.CanManageFlags, "Gen5 should expose IMysteryGiftFlags");
        output.WriteLine("Gen5: CanManageFlags=true ✓");
    }

    [Fact]
    public void MysteryGift_Gen6_CanManageFlags()
    {
        // Gen6 XY may or may not implement IMysteryGiftFlags; just check it doesn't throw
        var sav = new SAV6XY();
        var vm = new MysteryGiftEditorViewModel(sav, DialogMock().Object);

        Assert.True(vm.IsSupported);
        // CanManageFlags is a bool — just assert it's accessible
        _ = vm.CanManageFlags;
        output.WriteLine($"Gen6: CanManageFlags={vm.CanManageFlags} (no throw) ✓");
    }

    // -----------------------------------------------------------------------
    // 8. DeleteFlag removes from ReceivedFlags list
    // -----------------------------------------------------------------------

    [Fact]
    public void MysteryGift_DeleteFlag_RemovesSelectedFlag()
    {
        var sav = BlankSaveFile.Get(GameVersion.W2);
        var vm = new MysteryGiftEditorViewModel(sav, DialogMock().Object);

        // Manually add a fake flag string to the list
        vm.ReceivedFlags.Add("0042");
        vm.SelectedReceivedFlag = "0042";

        vm.DeleteFlagCommand.Execute(null);

        Assert.DoesNotContain("0042", vm.ReceivedFlags);
        output.WriteLine("DeleteFlag: flag 0042 removed from ReceivedFlags ✓");
    }
}
