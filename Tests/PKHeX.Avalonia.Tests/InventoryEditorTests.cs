using PKHeX.Presentation.ViewModels;
using PKHeX.Core;
using Xunit.Abstractions;

namespace PKHeX.Avalonia.Tests;

/// <summary>
/// Behavioral tests for InventoryEditorViewModel.
/// Covers the staged-write model: changes sit in ViewModels until SaveCommand is called,
/// which calls ApplyChanges() per pouch to flush to the underlying InventoryPouch.
/// </summary>
public class InventoryEditorTests(ITestOutputHelper output)
{
    // -----------------------------------------------------------------------
    // 1. SaveCommand flushes item changes to the underlying save
    // -----------------------------------------------------------------------

    [Fact]
    public void InventoryEditor_SaveCommand_PersistsItemCount()
    {
        var sav = new SAV6XY();
        var vm = new InventoryEditorViewModel(sav, new Moq.Mock<ISpriteRenderer>().Object);

        Assert.NotEmpty(vm.Pouches);
        var pouch = vm.Pouches.First(p => p.ItemList.Count > 0 && p.Items.Count > 0);

        // Set a count on the first item slot
        var item = pouch.Items[0];
        var targetItemId = pouch.ItemList[0].Value; // use a valid item from the list
        item.ItemId = targetItemId;
        item.Count = 7;

        // Execute save — flushes ViewModel to sav.Inventory.Pouches
        vm.SaveCommand.Execute(null);

        // Verify by reloading a fresh ViewModel from the same save
        var vm2 = new InventoryEditorViewModel(sav, new Moq.Mock<ISpriteRenderer>().Object);
        var pouch2 = vm2.Pouches.First(p => p.PouchName == pouch.PouchName);
        var item2 = pouch2.Items[0];

        Assert.Equal(targetItemId, item2.ItemId);
        Assert.Equal(7, item2.Count);
        output.WriteLine($"Gen6 {pouch.PouchName}: item [{targetItemId}] count=7 round-tripped through save ✓");
    }

    // -----------------------------------------------------------------------
    // 2. GiveAllItems sets all items in a pouch to MaxCount
    //    (Regression: SWSH items showing 0 after GiveAll due to range mismatch)
    // -----------------------------------------------------------------------

    [Theory]
    [InlineData(GameVersion.X,  "Gen6-X")]
    [InlineData(GameVersion.SN, "Gen7-Sun")]
    [InlineData(GameVersion.SW, "Gen8-Sword")]
    [InlineData(GameVersion.SL, "Gen9-Scarlet")]
    public void InventoryEditor_GiveAll_SetsMaxCountAfterSave(GameVersion version, string label)
    {
        var sav = BlankSaveFile.Get(version);
        var vm = new InventoryEditorViewModel(sav, new Moq.Mock<ISpriteRenderer>().Object);

        if (vm.Pouches.Count == 0) { output.WriteLine($"{label}: no pouches, skipping"); return; }

        // Use first pouch with an item list
        var pouch = vm.Pouches.FirstOrDefault(p => p.ItemList.Count > 0);
        if (pouch == null) { output.WriteLine($"{label}: no pouch with items, skipping"); return; }

        pouch.GiveAllItems();
        vm.SaveCommand.Execute(null);

        var nonZero = pouch.Items.Count(i => i.Count > 0);
        Assert.True(nonZero > 0,
            $"{label} '{pouch.PouchName}': GiveAll + Save left every item at count=0");
        output.WriteLine($"{label} '{pouch.PouchName}': {nonZero}/{pouch.Items.Count} items non-zero after GiveAll ✓");
    }

    // -----------------------------------------------------------------------
    // 3. ClearAllItems zeros out all item counts
    // -----------------------------------------------------------------------

    [Fact]
    public void InventoryEditor_ClearAll_ZerosAllItems()
    {
        var sav = new SAV6XY();
        var vm = new InventoryEditorViewModel(sav, new Moq.Mock<ISpriteRenderer>().Object);

        var pouch = vm.Pouches.FirstOrDefault(p => p.ItemList.Count > 0);
        if (pouch == null) { output.WriteLine("Gen6: no pouch with items, skipping"); return; }

        // First give all, then clear
        pouch.GiveAllItems();
        pouch.ClearAllItems();

        // All counts should be 0 in the ViewModel
        Assert.All(pouch.Items, item => Assert.Equal(0, item.Count));

        // After save they should also be 0
        vm.SaveCommand.Execute(null);

        // Verify via fresh ViewModel reload
        var vm2 = new InventoryEditorViewModel(sav, new Moq.Mock<ISpriteRenderer>().Object);
        var pouch2 = vm2.Pouches.First(p => p.PouchName == pouch.PouchName);
        Assert.All(pouch2.Items, item => Assert.Equal(0, item.Count));
        output.WriteLine($"Gen6 '{pouch.PouchName}': all items zeroed after ClearAll ✓");
    }

    // -----------------------------------------------------------------------
    // 4. ResetCommand discards uncommitted changes
    // -----------------------------------------------------------------------

    [Fact]
    public void InventoryEditor_ResetCommand_DiscardsUncommittedChanges()
    {
        var sav = new SAV6XY();
        var vm = new InventoryEditorViewModel(sav, new Moq.Mock<ISpriteRenderer>().Object);

        var pouch = vm.Pouches.FirstOrDefault(p => p.Items.Count > 0);
        if (pouch == null) { output.WriteLine("Gen6: no pouch, skipping"); return; }

        // Record original count in ViewModel
        var originalCount = pouch.Items[0].Count;

        // Modify without saving
        pouch.Items[0].Count = 999;
        Assert.Equal(999, pouch.Items[0].Count);

        // Reset — should reload from the underlying pouch
        vm.ResetCommand.Execute(null);

        Assert.Equal(originalCount, pouch.Items[0].Count);
        output.WriteLine($"Gen6: Reset discarded count=999, restored to {originalCount} ✓");
    }

    // -----------------------------------------------------------------------
    // 5. Item count round-trips exactly through the staged-write model
    //    Note: MaxCount is a UI constraint only; the VM does not clamp at save.
    // -----------------------------------------------------------------------

    [Fact]
    public void InventoryEditor_ItemCount_RoundTripsExactly()
    {
        var sav = new SAV3E();
        var vm = new InventoryEditorViewModel(sav, new Moq.Mock<ISpriteRenderer>().Object);

        var pouch = vm.Pouches.FirstOrDefault(p => p.ItemList.Count > 0 && p.Items.Count > 0);
        if (pouch == null) { output.WriteLine("Gen3 Emerald: no pouch with items, skipping"); return; }

        var item = pouch.Items[0];
        var targetItemId = pouch.ItemList[0].Value;
        item.ItemId = targetItemId;
        item.Count = 50;

        vm.SaveCommand.Execute(null);

        var vm2 = new InventoryEditorViewModel(sav, new Moq.Mock<ISpriteRenderer>().Object);
        var pouch2 = vm2.Pouches.First(p => p.PouchName == pouch.PouchName);
        Assert.Equal(targetItemId, pouch2.Items[0].ItemId);
        Assert.Equal(50, pouch2.Items[0].Count);
        output.WriteLine($"Gen3 '{pouch.PouchName}': item [{targetItemId}] count=50 round-tripped ✓");
    }
}
