
using PKHeX.Core;
using PKHeX.Presentation.ViewModels;

namespace PKHeX.Avalonia.Tests;

public class HyperTrainingTests
{
    [Fact]
    public void HyperTrainedFlags_SurfaceAfterLoad()
    {
        var sav = new SAV8SWSH();
        var pkm = new PK8 // Grookey
        {
            Species = 810,
            HT_HP = true,
            HT_SPE = true,
        };
        var (vm, _, _) = TestHelpers.CreateTestViewModel(pkm, sav);

        Assert.True(vm.HasHyperTraining);
        Assert.True(vm.HyperTrainedHP);
        Assert.False(vm.HyperTrainedATK);
        Assert.False(vm.HyperTrainedDEF);
        Assert.False(vm.HyperTrainedSPA);
        Assert.False(vm.HyperTrainedSPD);
        Assert.True(vm.HyperTrainedSPE);
    }

    [Fact]
    public void Toggling_HyperTrained_WritesFlagBack()
    {
        var sav = new SAV9SV();
        var pkm = new PK9 { Species = 906 }; // Sprigatito
        var (vm, _, _) = TestHelpers.CreateTestViewModel(pkm, sav);

        vm.HyperTrainedDEF = true;
        vm.HyperTrainedSPA = true;

        var result = vm.PreparePKM();
        var ht = Assert.IsAssignableFrom<IHyperTrain>(result);
        Assert.True(ht.HT_DEF);
        Assert.True(ht.HT_SPA);
        Assert.False(ht.HT_HP);
    }

    [Fact]
    public void LowLevel_PKM_CannotHyperTrain()
    {
        var sav = new SAV9SV();
        var pkm = new PK9 { Species = 906 }; // Sprigatito
        var (vm, _, _) = TestHelpers.CreateTestViewModel(pkm, sav);

        Assert.False(vm.CanHyperTrain);
    }

    [Fact]
    public void Level100_PKM_CanHyperTrain()
    {
        var sav = new SAV9SV();
        var pkm = new PK9 { Species = 906 }; // Sprigatito
        pkm.CurrentLevel = 100;
        var (vm, _, _) = TestHelpers.CreateTestViewModel(pkm, sav);

        Assert.True(vm.CanHyperTrain);
    }

    [Fact]
    public void Gen5_PKM_HasNoHyperTraining()
    {
        var sav = new SAV5BW();
        var pkm = new PK5 { Species = 495 }; // Snivy
        var (vm, _, _) = TestHelpers.CreateTestViewModel(pkm, sav);

        Assert.False(vm.HasHyperTraining);
    }
}
