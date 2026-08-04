using PKHeX.Presentation.ViewModels;
using PKHeX.Core;
using Xunit;

namespace PKHeX.Avalonia.Tests;

public class Misc4EditorTests
{
    [Fact]
    public void Misc4_LoadCoins_VerifyValue()
    {
        // Arrange
        var sav = new SAV4DP();
        sav.Coin = 1234;

        // Act
        var vm = new Misc4EditorViewModel(sav);

        // Assert
        Assert.Equal(1234u, vm.Coins);
    }

    [Fact]
    public void Misc4_SaveCoins_PersistsCorrectly()
    {
        // Arrange
        var sav = new SAV4DP();
        var vm = new Misc4EditorViewModel(sav);

        // Act
        vm.Coins = 5678;
        vm.SaveCommand.Execute(null);

        // Assert
        Assert.Equal(5678u, sav.Coin);
    }

    [Fact]
    public void Misc4_LoadBP_VerifyValue()
    {
        // Arrange
        var sav = new SAV4Pt();
        sav.BP = 999;

        // Act
        var vm = new Misc4EditorViewModel(sav);

        // Assert
        Assert.Equal((ushort)999, vm.Bp);
    }

    [Fact]
    public void Misc4_WalkerVisible_OnlyForHGSS()
    {
        // Arrange
        var savDP = new SAV4DP();
        var savPt = new SAV4Pt();
        var savHGSS = new SAV4HGSS();

        // Act
        var vmDP = new Misc4EditorViewModel(savDP);
        var vmPt = new Misc4EditorViewModel(savPt);
        var vmHGSS = new Misc4EditorViewModel(savHGSS);

        // Assert
        Assert.False(vmDP.IsWalkerVisible);
        Assert.False(vmPt.IsWalkerVisible);
        Assert.True(vmHGSS.IsWalkerVisible);
    }

    [Fact]
    public void Misc4_BattleFrontierVisible_NotForDP()
    {
        // Arrange
        var savDP = new SAV4DP();
        var savPt = new SAV4Pt();
        var savHGSS = new SAV4HGSS();

        // Act
        var vmDP = new Misc4EditorViewModel(savDP);
        var vmPt = new Misc4EditorViewModel(savPt);
        var vmHGSS = new Misc4EditorViewModel(savHGSS);

        // Assert
        Assert.False(vmDP.IsBattleFrontierVisible);
        Assert.True(vmPt.IsBattleFrontierVisible);
        Assert.True(vmHGSS.IsBattleFrontierVisible);
    }

    [Fact]
    public void Misc4_UnlockAllFly_SetsAllFlags()
    {
        // Arrange
        var sav = new SAV4DP();
        var vm = new Misc4EditorViewModel(sav);

        // Act
        vm.UnlockAllFlyDestinationsCommand.Execute(null);

        // Assert
        Assert.All(vm.FlyDestinations, dest => Assert.True(dest.IsUnlocked));
    }

    [Fact]
    public void Misc4_FlyDestinations_LoadsLocations()
    {
        // Arrange
        var sav = new SAV4DP();

        // Act
        var vm = new Misc4EditorViewModel(sav);

        // Assert
        Assert.NotEmpty(vm.FlyDestinations);
    }

    [Fact]
    public void Misc4_HGSS_WalkerCourses_Load()
    {
        // Arrange
        var sav = new SAV4HGSS();

        // Act
        var vm = new Misc4EditorViewModel(sav);

        // Assert
        Assert.NotEmpty(vm.WalkerCourses);
    }

    [Fact]
    public void Misc4_HGSS_UnlockAllWalkerCourses()
    {
        // Arrange
        var sav = new SAV4HGSS();
        var vm = new Misc4EditorViewModel(sav);

        // Act
        vm.UnlockAllWalkerCoursesCommand.Execute(null);

        // Assert - at least some courses should be unlocked
        Assert.Contains(vm.WalkerCourses, course => course.IsUnlocked);
    }

    [Fact]
    public void Misc4_PtHGSS_FrontierPrints_Load()
    {
        // Arrange
        var sav = new SAV4Pt();

        // Act
        var vm = new Misc4EditorViewModel(sav);

        // Assert - Should have 5 facilities
        Assert.Equal(5, vm.FrontierPrints.Count);
    }

    [Fact]
    public void Misc4_GiveAllPrints_SetsGoldStatus()
    {
        // Arrange
        var sav = new SAV4Pt();
        var vm = new Misc4EditorViewModel(sav);

        // Act
        vm.GiveAllPrintsCommand.Execute(null);

        // Assert
        Assert.All(vm.FrontierPrints, print =>
            Assert.Equal(BattleFrontierPrintStatus4.SecondReceived, print.Status));
    }

    [Fact]
    public void Misc4_ClearAllPrints_SetsNoneStatus()
    {
        // Arrange
        var sav = new SAV4Pt();
        var vm = new Misc4EditorViewModel(sav);
        vm.GiveAllPrintsCommand.Execute(null); // First set to gold

        // Act
        vm.ClearAllPrintsCommand.Execute(null);

        // Assert
        Assert.All(vm.FrontierPrints, print =>
            Assert.Equal(BattleFrontierPrintStatus4.None, print.Status));
    }

    [Fact]
    public void Misc4_IsSinnoh_CorrectForDPPt()
    {
        // Arrange
        var savDP = new SAV4DP();
        var savPt = new SAV4Pt();
        var savHGSS = new SAV4HGSS();

        // Act
        var vmDP = new Misc4EditorViewModel(savDP);
        var vmPt = new Misc4EditorViewModel(savPt);
        var vmHGSS = new Misc4EditorViewModel(savHGSS);

        // Assert
        Assert.True(vmDP.IsSinnoh);
        Assert.True(vmPt.IsSinnoh);
        Assert.False(vmHGSS.IsSinnoh);
    }
}
