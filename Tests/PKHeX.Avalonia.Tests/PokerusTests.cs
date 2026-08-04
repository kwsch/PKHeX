
using PKHeX.Core;
using PKHeX.Presentation.ViewModels;

namespace PKHeX.Avalonia.Tests;

public class PokerusTests
{
    [Fact]
    public void Toggling_Infected_OnClean_PKM_InfectsIt()
    {
        var sav = new SAV8SWSH();
        var pkm = new PK8 { Species = 810 }; // Grookey
        var (vm, _, _) = TestHelpers.CreateTestViewModel(pkm, sav);

        vm.IsPokerusInfected = true;

        var result = vm.PreparePKM();
        Assert.True(result.IsPokerusInfected);
        Assert.True(result.PokerusStrain >= 1);
        Assert.True(result.PokerusDays >= 1);
        Assert.False(result.IsPokerusCured);
        Assert.Equal(result.PokerusStrain, vm.PkrsStrain);
        Assert.Equal(result.PokerusDays, vm.PkrsDays);
    }

    [Fact]
    public void Toggling_Infected_Off_ClearsInfection()
    {
        var sav = new SAV8SWSH();
        var pkm = new PK8 { Species = 810, PokerusStrain = 3, PokerusDays = 2 }; // Grookey
        var (vm, _, _) = TestHelpers.CreateTestViewModel(pkm, sav);

        Assert.True(vm.IsPokerusInfected);
        Assert.False(vm.IsPokerusCured);

        vm.IsPokerusInfected = false;

        var result = vm.PreparePKM();
        Assert.False(result.IsPokerusInfected);
        Assert.False(result.IsPokerusCured);
        Assert.Equal(0, result.PokerusStrain);
        Assert.Equal(0, result.PokerusDays);
    }

    [Fact]
    public void Toggling_Cured_SetsStrainAndNoDays()
    {
        var sav = new SAV9SV();
        var pkm = new PK9 { Species = 906 }; // Sprigatito
        var (vm, _, _) = TestHelpers.CreateTestViewModel(pkm, sav);

        vm.IsPokerusCured = true;

        var result = vm.PreparePKM();
        Assert.True(result.IsPokerusCured);
        Assert.True(result.IsPokerusInfected);
        Assert.True(result.PokerusStrain >= 1);
        Assert.Equal(0, result.PokerusDays);
        Assert.True(vm.IsPokerusInfected);
        Assert.Equal(result.PokerusStrain, vm.PkrsStrain);
        Assert.Equal(result.PokerusDays, vm.PkrsDays);
    }

    [Fact]
    public void Toggling_Cured_Off_AfterInfected_KeepsInfected()
    {
        var sav = new SAV9SV();
        var pkm = new PK9 { Species = 906 }; // Sprigatito
        var (vm, _, _) = TestHelpers.CreateTestViewModel(pkm, sav);

        vm.IsPokerusCured = true;
        vm.IsPokerusCured = false;

        var result = vm.PreparePKM();
        Assert.True(result.IsPokerusInfected);
        Assert.False(result.IsPokerusCured);
        Assert.True(result.PokerusStrain >= 1);
        Assert.Equal(1, result.PokerusDays);
    }

    [Fact]
    public void Loading_PKM_SurfacesPokerusCheckboxStates()
    {
        var sav = new SAV8SWSH();
        var pkm = new PK8 { Species = 810, PokerusStrain = 4, PokerusDays = 0 }; // Grookey, cured
        var (vm, _, _) = TestHelpers.CreateTestViewModel(pkm, sav);

        Assert.True(vm.IsPokerusInfected);
        Assert.True(vm.IsPokerusCured);

        pkm.PokerusDays = 3;
        var (vm2, _, _) = TestHelpers.CreateTestViewModel(pkm, sav);

        Assert.True(vm2.IsPokerusInfected);
        Assert.False(vm2.IsPokerusCured);

        var clean = new PK8 { Species = 810 };
        var (vm3, _, _) = TestHelpers.CreateTestViewModel(clean, sav);

        Assert.False(vm3.IsPokerusInfected);
        Assert.False(vm3.IsPokerusCured);
    }
}
