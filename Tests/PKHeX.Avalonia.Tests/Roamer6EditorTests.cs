using PKHeX.Presentation.ViewModels;
using PKHeX.Core;
using Xunit;

namespace PKHeX.Avalonia.Tests;

public class Roamer6EditorTests
{
    [Fact]
    public void Roamer6_LoadsCorrectly()
    {
        // Arrange
        var sav = new SAV6XY();
        sav.Encount.Roamer.Species = 144; // Articuno
        sav.Encount.Roamer.RoamStatus = Roamer6State.Roaming;
        sav.Encount.Roamer.TimesEncountered = 5;

        // Act
        var vm = new Roamer6EditorViewModel(sav);

        // Assert
        Assert.Equal(0, vm.SelectedSpeciesIndex); // Articuno is index 0
        Assert.Equal(1, vm.RoamStateIndex); // Roaming is index 1
        Assert.Equal(5u, vm.TimesEncountered);
    }

    [Fact]
    public void Roamer6_SavesCorrectly()
    {
        // Arrange
        var sav = new SAV6XY();
        var vm = new Roamer6EditorViewModel(sav);

        // Act
        vm.SelectedSpeciesIndex = 2; // Moltres
        vm.RoamStateIndex = 3; // Defeated
        vm.TimesEncountered = 10;
        vm.SaveCommand.Execute(null);

        // Assert
        Assert.Equal(146, sav.Encount.Roamer.Species); // Moltres
        Assert.Equal(Roamer6State.Defeated, sav.Encount.Roamer.RoamStatus);
        Assert.Equal(10u, sav.Encount.Roamer.TimesEncountered);
    }
}
