using PKHeX.Presentation.ViewModels;
using PKHeX.Core;
using Xunit;

namespace PKHeX.Avalonia.Tests;

public class Misc5EditorTests
{
    [Fact]
    public void Misc5_BW_IsBWProperty()
    {
        // Arrange
        var savBW = new SAV5BW();
        var savB2W2 = new SAV5B2W2();

        // Act
        var vmBW = new Misc5EditorViewModel(savBW);
        var vmB2W2 = new Misc5EditorViewModel(savB2W2);

        // Assert
        Assert.True(vmBW.IsBW);
        Assert.False(vmBW.IsB2W2);
        Assert.False(vmB2W2.IsBW);
        Assert.True(vmB2W2.IsB2W2);
    }

    [Fact]
    public void Misc5_FlyDestinations_LoadsCorrectCount()
    {
        // Arrange
        var savBW = new SAV5BW();
        var savB2W2 = new SAV5B2W2();

        // Act
        var vmBW = new Misc5EditorViewModel(savBW);
        var vmB2W2 = new Misc5EditorViewModel(savB2W2);

        // Assert - BW has 16 locations, B2W2 has 24
        Assert.Equal(16, vmBW.FlyDestinations.Count);
        Assert.Equal(24, vmB2W2.FlyDestinations.Count);
    }

    [Fact]
    public void Misc5_UnlockAllFly_SetsAllFlags()
    {
        // Arrange
        var sav = new SAV5BW();
        var vm = new Misc5EditorViewModel(sav);

        // Act
        vm.UnlockAllFlyDestinationsCommand.Execute(null);

        // Assert
        Assert.All(vm.FlyDestinations, dest => Assert.True(dest.IsUnlocked));
    }

    [Fact]
    public void Misc5_SubwayRecords_Load()
    {
        // Arrange
        var sav = new SAV5B2W2();

        // Act
        var vm = new Misc5EditorViewModel(sav);

        // Assert - Default values should be 0
        Assert.Equal(0, vm.SinglePast);
        Assert.Equal(0, vm.SingleRecord);
    }

    [Fact]
    public void Misc5_UnlockAllSuperModes_SetsAllFlags()
    {
        // Arrange
        var sav = new SAV5BW();
        var vm = new Misc5EditorViewModel(sav);

        // Act
        vm.UnlockAllSuperModesCommand.Execute(null);

        // Assert
        Assert.True(vm.SuperSingleUnlocked);
        Assert.True(vm.SuperDoubleUnlocked);
        Assert.True(vm.SuperMultiUnlocked);
    }

    [Fact]
    public void Misc5_B2W2_KeySystem_Loads()
    {
        // Arrange
        var sav = new SAV5B2W2();

        // Act
        var vm = new Misc5EditorViewModel(sav);

        // Assert - Should have 5 keys
        Assert.Equal(5, vm.Keys.Count);
    }

    [Fact]
    public void Misc5_B2W2_UnlockAllKeys_SetsAllFlags()
    {
        // Arrange
        var sav = new SAV5B2W2();
        var vm = new Misc5EditorViewModel(sav);

        // Act
        vm.UnlockAllKeysCommand.Execute(null);

        // Assert
        Assert.All(vm.Keys, key =>
        {
            Assert.True(key.IsObtained);
            Assert.True(key.IsUnlocked);
        });
    }

    [Fact]
    public void Misc5_BW_Roamers_Loads()
    {
        // Arrange
        var sav = new SAV5BW();

        // Act
        var vm = new Misc5EditorViewModel(sav);

        // Assert - Should have 2 roamers
        Assert.Equal(2, vm.Roamers.Count);
    }

    [Fact]
    public void Misc5_BW_NoKeys()
    {
        // Arrange
        var sav = new SAV5BW();

        // Act
        var vm = new Misc5EditorViewModel(sav);

        // Assert - BW has no key system
        Assert.Empty(vm.Keys);
    }

    [Fact]
    public void Misc5_B2W2_NoRoamers()
    {
        // Arrange
        var sav = new SAV5B2W2();

        // Act
        var vm = new Misc5EditorViewModel(sav);

        // Assert - B2W2 has no roamers
        Assert.Empty(vm.Roamers);
    }
}
