using PKHeX.Presentation.ViewModels;
using PKHeX.Core;
using Xunit;
using System.Linq;

namespace PKHeX.Avalonia.Tests;

public class PokepuffEditorTests
{
    [Fact]
    public void Pokepuff_LoadsCorrectly()
    {
        // Arrange
        var sav = new SAV6XY();
        // SAV6XY initializes with default puffs (empty) but fixed capacity
        
        // Act
        var vm = new PokepuffEditorViewModel(sav);

        // Assert
        // Gen 6 usually has 100 slots
        Assert.Equal(100, vm.Puffs.Count);
    }

    [Fact]
    public void Pokepuff_GiveAllBest_FillsPuffs()
    {
        // Arrange
        var sav = new SAV6XY();
        var vm = new PokepuffEditorViewModel(sav);

        // Act
        vm.GiveAllBestCommand.Execute(null);

        // Assert
        // Should have high-tier puffs
        Assert.Contains(vm.Puffs, p => p.PuffIndex >= 4); 
    }

    [Fact]
    public void Pokepuff_RemoveAll_ClearPuffs()
    {
        // Arrange
        var sav = new SAV6XY();
        sav.Puff.MaxCheat(true);
        var vm = new PokepuffEditorViewModel(sav);
        
        // Act
        vm.RemoveAllCommand.Execute(null);

        // Assert
        // Reset() restores 5 default puffs (1-5), rest are 0
        Assert.Equal(1, vm.Puffs[0].PuffIndex);
        Assert.Equal(2, vm.Puffs[1].PuffIndex);
        Assert.Equal(3, vm.Puffs[2].PuffIndex);
        Assert.Equal(4, vm.Puffs[3].PuffIndex);
        Assert.Equal(5, vm.Puffs[4].PuffIndex);
        Assert.Equal(0, vm.Puffs[5].PuffIndex);
    }

    [Fact]
    public void Pokepuff_SavesCorrectly()
    {
        // Arrange
        var sav = new SAV6XY();
        var vm = new PokepuffEditorViewModel(sav);
        
        // Modify a slot
        vm.Puffs[0].PuffIndex = 5;

        // Act
        vm.SaveCommand.Execute(null);

        // Assert
        // Reload from sav to verify
        var savedPuffs = sav.Puff.GetPuffs();
        Assert.Equal(5, savedPuffs[0]);
    }
}
