using PKHeX.Presentation.ViewModels;
using PKHeX.Core;
using Xunit;

namespace PKHeX.Avalonia.Tests;

public class Misc7EditorTests
{
    [Fact]
    public void Misc7_IsUSUM_Property()
    {
        // Arrange
        var savSM = new SAV7SM();
        var savUSUM = new SAV7USUM();

        // Act
        var vmSM = new Misc7EditorViewModel(savSM);
        var vmUSUM = new Misc7EditorViewModel(savUSUM);

        // Assert
        Assert.False(vmSM.IsUSUM);
        Assert.True(vmSM.IsSM);
        Assert.True(vmUSUM.IsUSUM);
        Assert.False(vmUSUM.IsSM);
    }

    [Fact]
    public void Misc7_BattleTree_LoadsCorrectly()
    {
        // Arrange
        var sav = new SAV7SM();

        // Act
        var vm = new Misc7EditorViewModel(sav);

        // Assert - default values should be 0
        Assert.Equal(0, vm.SingleCurrentStreak);
        Assert.Equal(0, vm.SingleMaxStreak);
    }

    [Fact]
    public void Misc7_UnlockAllBattleTree_SetsAllFlags()
    {
        // Arrange
        var sav = new SAV7SM();
        var vm = new Misc7EditorViewModel(sav);

        // Act
        vm.UnlockAllBattleTreeModesCommand.Execute(null);

        // Assert
        Assert.True(vm.SuperSingleUnlocked);
        Assert.True(vm.SuperDoubleUnlocked);
        Assert.True(vm.SuperMultiUnlocked);
    }

    [Fact]
    public void Misc7_PokeFinder_LoadsCorrectly()
    {
        // Arrange
        var sav = new SAV7SM();

        // Act
        var vm = new Misc7EditorViewModel(sav);

        // Assert
        Assert.Equal(0u, vm.SnapCount);
    }

    [Fact]
    public void Misc7_MaxPokeFinder_SetsMaxValues()
    {
        // Arrange
        var sav = new SAV7SM();
        var vm = new Misc7EditorViewModel(sav);

        // Act
        vm.MaxPokeFinderCommand.Execute(null);

        // Assert
        Assert.Equal(999999u, vm.SnapCount);
        Assert.Equal(9999999u, vm.ThumbsTotal);
        Assert.Equal(2, vm.CameraVersion);
    }

    [Fact]
    public void Misc7_Stamps_Loads()
    {
        // Arrange
        var sav = new SAV7SM();

        // Act
        var vm = new Misc7EditorViewModel(sav);

        // Assert - Stamp7 enum should have some entries
        Assert.NotEmpty(vm.Stamps);
    }

    [Fact]
    public void Misc7_FlyDestinations_Loads()
    {
        // Arrange
        var savSM = new SAV7SM();
        var savUSUM = new SAV7USUM();

        // Act
        var vmSM = new Misc7EditorViewModel(savSM);
        var vmUSUM = new Misc7EditorViewModel(savUSUM);

        // Assert - SM has fewer fly destinations than USUM
        Assert.True(vmSM.FlyDestinations.Count < vmUSUM.FlyDestinations.Count);
    }

    [Fact]
    public void Misc7_UnlockAllFlyDestinations_SetsAllFlags()
    {
        // Arrange
        var sav = new SAV7SM();
        var vm = new Misc7EditorViewModel(sav);

        // Act
        vm.UnlockAllFlyDestinationsCommand.Execute(null);

        // Assert
        Assert.All(vm.FlyDestinations, dest => Assert.True(dest.IsUnlocked));
    }

    [Fact]
    public void Misc7_UnlockAllStamps_SetsAllFlags()
    {
        // Arrange
        var sav = new SAV7SM();
        var vm = new Misc7EditorViewModel(sav);

        // Act
        vm.UnlockAllStampsCommand.Execute(null);

        // Assert
        Assert.All(vm.Stamps, stamp => Assert.True(stamp.IsObtained));
    }
}
