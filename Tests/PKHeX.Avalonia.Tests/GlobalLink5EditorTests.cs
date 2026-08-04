using PKHeX.Core;
using PKHeX.Presentation.ViewModels;
using Xunit;

namespace PKHeX.Avalonia.Tests;

public class GlobalLink5EditorTests
{
    [Fact]
    public void GlobalLink5_B2W2_LoadsAndIsSupported()
    {
        var sav = new SAV5B2W2();
        var vm = new GlobalLink5EditorViewModel(sav);

        Assert.True(vm.IsSupported);
        Assert.Equal(GlobalLink5.CountItems, vm.Items.Count);
        Assert.Equal(GlobalLink5.CountFurniture, vm.Furniture.Count);
        Assert.NotEmpty(vm.ItemList);
    }

    [Fact]
    public void GlobalLink5_BW_LoadsAndIsSupported()
    {
        var sav = new SAV5BW();
        var vm = new GlobalLink5EditorViewModel(sav);

        Assert.True(vm.IsSupported);
        Assert.Equal(GlobalLink5.CountItems, vm.Items.Count);
        Assert.Equal(GlobalLink5.CountFurniture, vm.Furniture.Count);
    }

    [Fact]
    public void GlobalLink5_ReflectsExistingBlockValues()
    {
        var sav = new SAV5B2W2();
        sav.GlobalLink.UploadCount = 42;
        sav.GlobalLink.IsRegistered = true;
        sav.GlobalLink.Musical = 7;
        sav.GlobalLink.SetItem(0, 1);          // Master Ball
        sav.GlobalLink.SetItemQuantity(0, 5);

        var vm = new GlobalLink5EditorViewModel(sav);

        Assert.Equal(42, vm.UploadCount);
        Assert.True(vm.IsRegistered);
        Assert.Equal(7, vm.Musical);
        Assert.Equal(1, vm.Items[0].ItemId);
        Assert.Equal(5, vm.Items[0].Quantity);
    }

    [Fact]
    public void GlobalLink5_Flags_WriteThroughToSave()
    {
        var sav = new SAV5B2W2();
        var vm = new GlobalLink5EditorViewModel(sav);

        vm.IsSlotPresent = true;
        vm.IsRegistered = true;
        vm.IsAccountFullAccess = true;
        vm.IsFurnitureSynchronized = true;

        Assert.True(sav.GlobalLink.IsSlotPresent);
        Assert.True(sav.GlobalLink.IsRegistered);
        Assert.True(sav.GlobalLink.IsAccountFullAccess);
        Assert.True(sav.GlobalLink.IsFurnitureSynchronized);
        Assert.True(sav.State.Edited);
    }

    [Fact]
    public void GlobalLink5_Counters_WriteThroughToSave()
    {
        var sav = new SAV5B2W2();
        var vm = new GlobalLink5EditorViewModel(sav);

        vm.UploadCount = 123;
        vm.UploadStatus = 9;
        vm.SelectedFurnitureIndex = 3;
        vm.Musical = 11;
        vm.CGearSkin = 22;
        vm.DexSkin = 33;

        Assert.Equal(123, sav.GlobalLink.UploadCount);
        Assert.Equal(9, sav.GlobalLink.UploadStatus);
        Assert.Equal(3, sav.GlobalLink.SelectedFurnitureIndex);
        Assert.Equal(11, sav.GlobalLink.Musical);
        Assert.Equal(22, sav.GlobalLink.CGearSkin);
        Assert.Equal(33, sav.GlobalLink.DexSkin);
    }

    [Fact]
    public void GlobalLink5_Items_WriteThroughToSave()
    {
        var sav = new SAV5B2W2();
        var vm = new GlobalLink5EditorViewModel(sav);

        vm.Items[2].ItemId = 4;       // Poké Ball
        vm.Items[2].Quantity = 250;

        Assert.Equal(4, sav.GlobalLink.GetItem(2));
        Assert.Equal(250, sav.GlobalLink.GetItemQuantity(2));
        Assert.True(sav.State.Edited);
    }

    [Fact]
    public void GlobalLink5_Furniture_WriteThroughToSave()
    {
        var sav = new SAV5B2W2();
        var vm = new GlobalLink5EditorViewModel(sav);

        vm.Furniture[1].Value = 200;

        Assert.Equal(200, sav.GlobalLink.GetFurniture(1).Value);
        Assert.True(sav.State.Edited);
    }

    [Fact]
    public void GlobalLink5_LoadingDoesNotMarkEdited()
    {
        var sav = new SAV5B2W2();
        _ = new GlobalLink5EditorViewModel(sav);

        Assert.False(sav.State.Edited);
    }
}
