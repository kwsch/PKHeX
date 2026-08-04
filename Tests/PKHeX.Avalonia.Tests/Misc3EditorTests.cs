using PKHeX.Presentation.ViewModels;
using PKHeX.Core;
using Xunit;

namespace PKHeX.Avalonia.Tests;

public class Misc3EditorTests
{
    [Fact]
    public void Misc3_LoadCoins_VerifyValue()
    {
        // Arrange
        var sav = new SAV3E();
        sav.Coin = 1234;

        // Act
        var vm = new Misc3EditorViewModel(sav);

        // Assert
        Assert.Equal((ushort)1234, vm.Coins);
    }

    [Fact]
    public void Misc3_SaveCoins_PersistsCorrectly()
    {
        // Arrange
        var sav = new SAV3E();
        var vm = new Misc3EditorViewModel(sav);

        // Act
        vm.Coins = 5678;
        vm.SaveCommand.Execute(null);

        // Assert
        Assert.Equal(5678u, sav.Coin);
    }

    [Fact]
    public void Misc3_LoadBP_Emerald()
    {
        // Arrange
        var sav = new SAV3E();
        sav.SmallBlock.BP = 999;

        // Act
        var vm = new Misc3EditorViewModel(sav);

        // Assert
        Assert.Equal(999u, vm.Bp);
        Assert.True(vm.IsBpVisible);
    }

    [Fact]
    public void Misc3_SaveBP_PersistsCorrectly()
    {
        // Arrange
        var sav = new SAV3E();
        var vm = new Misc3EditorViewModel(sav);

        // Act
        vm.Bp = 2500;
        vm.SaveCommand.Execute(null);

        // Assert
        Assert.Equal(2500u, (uint)sav.SmallBlock.BP);
    }

    [Fact]
    public void Misc3_BPNotVisible_ForRubySapphire()
    {
        // Arrange
        var sav = new SAV3RS();

        // Act
        var vm = new Misc3EditorViewModel(sav);

        // Assert
        Assert.False(vm.IsBpVisible);
    }

    [Fact]
    public void Misc3_JoyfulVisible_ForJoyfulSaves()
    {
        // Arrange - Emerald implements IGen3Joyful
        var sav = new SAV3E();

        // Act
        var vm = new Misc3EditorViewModel(sav);

        // Assert
        Assert.True(vm.IsJoyfulVisible);
    }

    [Fact]
    public void Misc3_BattleFrontierSymbols_LoadCorrectly()
    {
        // Arrange
        var sav = new SAV3E();

        // Act
        var vm = new Misc3EditorViewModel(sav);

        // Assert - Should have 7 facilities
        Assert.Equal(7, vm.FrontierSymbols.Count);
        Assert.True(vm.IsBattleFrontierVisible);
    }

    [Fact]
    public void Misc3_GiveAllSymbols_SetsAllToGold()
    {
        // Arrange
        var sav = new SAV3E();
        var vm = new Misc3EditorViewModel(sav);

        // Act
        vm.GiveAllSymbolsCommand.Execute(null);

        // Assert
        foreach (var symbol in vm.FrontierSymbols)
            Assert.Equal(2, symbol.Status); // 2 = Gold
    }

    [Fact]
    public void Misc3_ClearAllSymbols_SetsAllToNone()
    {
        // Arrange
        var sav = new SAV3E();
        var vm = new Misc3EditorViewModel(sav);
        vm.GiveAllSymbolsCommand.Execute(null); // First set to gold

        // Act
        vm.ClearAllSymbolsCommand.Execute(null);

        // Assert
        foreach (var symbol in vm.FrontierSymbols)
            Assert.Equal(0, symbol.Status); // 0 = None
    }

    [Fact]
    public void Misc3_FerryVisible_OnlyForEmerald()
    {
        // Arrange
        var savE = new SAV3E();
        var savRS = new SAV3RS();
        var savFRLG = new SAV3FRLG();

        // Act
        var vmE = new Misc3EditorViewModel(savE);
        var vmRS = new Misc3EditorViewModel(savRS);
        var vmFRLG = new Misc3EditorViewModel(savFRLG);

        // Assert
        Assert.True(vmE.IsFerryVisible);
        Assert.False(vmRS.IsFerryVisible);
        Assert.False(vmFRLG.IsFerryVisible);
    }

    [Fact]
    public void Misc3_UnlockAllFerry_SetsAllFlags()
    {
        // Arrange
        var sav = new SAV3E();
        var vm = new Misc3EditorViewModel(sav);

        // Act
        vm.UnlockAllFerryDestinationsCommand.Execute(null);

        // Assert
        Assert.True(vm.ReachSouthernIsland);
        Assert.True(vm.ReachBirthIsland);
        Assert.True(vm.ReachFarawayIsland);
        Assert.True(vm.ReachNavelRock);
        Assert.True(vm.ReachBattleFrontier);
        Assert.True(vm.InitialSouthernIsland);
        Assert.True(vm.InitialBirthIsland);
        Assert.True(vm.InitialFarawayIsland);
        Assert.True(vm.InitialNavelRock);
    }

    [Fact]
    public void Misc3_RecordList_LoadsItems()
    {
        // Arrange
        var sav = new SAV3E();

        // Act
        var vm = new Misc3EditorViewModel(sav);

        // Assert
        Assert.NotEmpty(vm.RecordList);
        Assert.NotNull(vm.SelectedRecord);
    }
}
