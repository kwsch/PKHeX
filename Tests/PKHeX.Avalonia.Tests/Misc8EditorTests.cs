using PKHeX.Presentation.ViewModels;
using PKHeX.Core;
using Xunit;

namespace PKHeX.Avalonia.Tests;

public class Misc8EditorTests
{
    [Fact]
    public void Misc8_LoadsCorrectly()
    {
        // Arrange
        var sav = new SAV8SWSH();

        // Act
        var vm = new Misc8EditorViewModel(sav);

        // Assert - default values should be 0
        Assert.Equal(0u, vm.Watts);
        Assert.Equal(0, vm.Bp);
    }

    [Fact]
    public void Misc8_MaxWatts_SetsMaxValue()
    {
        // Arrange
        var sav = new SAV8SWSH();
        var vm = new Misc8EditorViewModel(sav);

        // Act
        vm.MaxWattsCommand.Execute(null);

        // Assert
        Assert.Equal(MyStatus8.MaxWatt, vm.Watts);
    }

    [Fact]
    public void Misc8_MaxBP_SetsMaxValue()
    {
        // Arrange
        var sav = new SAV8SWSH();
        var vm = new Misc8EditorViewModel(sav);

        // Act
        vm.MaxBPCommand.Execute(null);

        // Assert
        Assert.Equal(9999, vm.Bp);
    }

    [Fact]
    public void Misc8_BattleTower_LoadsCorrectly()
    {
        // Arrange
        var sav = new SAV8SWSH();

        // Act
        var vm = new Misc8EditorViewModel(sav);

        // Assert - default values
        Assert.Equal(0u, vm.SinglesWins);
        Assert.Equal(0u, vm.DoublesWins);
        Assert.Equal((ushort)0, vm.SinglesStreak);
        Assert.Equal((ushort)0, vm.DoublesStreak);
    }

    [Fact]
    public void Misc8_IsIoA_DependsOnRevision()
    {
        // Arrange
        var sav = new SAV8SWSH();

        // Act
        var vm = new Misc8EditorViewModel(sav);

        // Assert - default SAV8SWSH constructor creates a save with DLC support
        Assert.True(vm.IsIoA);
    }
}
