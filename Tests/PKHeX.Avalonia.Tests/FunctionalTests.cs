using Moq;
using PKHeX.Avalonia.Services;
using PKHeX.Presentation.ViewModels;
using PKHeX.Core;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace PKHeX.Avalonia.Tests;

public class FunctionalTests
{
    private readonly SaveFile _saveFile;

    public FunctionalTests()
    {
        _saveFile = new SAV3E(new byte[0x20000]);
    }

    [Fact]
    public void Audit_PokemonEditor_SpeciesDependencies()
    {
        var pkm = new PK3();
        var (vm, _, _) = TestHelpers.CreateTestViewModel(pkm, _saveFile);
        
        // Changing Species should notify all Base Stats
        bool hpNotified = false;
        vm.PropertyChanged += (s, e) => { if (e.PropertyName == nameof(vm.Base_HP)) hpNotified = true; };
        
        vm.Species = 1; // Bulbasaur
        Assert.True(hpNotified, "Changing Species should have notified Base_HP, but it didn't.");
    }

    [Fact]
    public void Audit_PokemonEditor_Dependencies()
    {
        var pkm = new PK3();
        var (vm, _, _) = TestHelpers.CreateTestViewModel(pkm, _saveFile);
        
        // This will find any [NotifyPropertyChangedFor] that doesn't actually work
        LogicAuditor.AuditPropertyDependencies(vm);
    }

    [Fact]
    public void Audit_PokemonEditor_InitialLoadState()
    {
        var pkm = new PK3 { IV_HP = 31, IV_ATK = 31 };
        var (vm, _, _) = TestHelpers.CreateTestViewModel(pkm, _saveFile);

        // Check that computed properties are populated immediately after load
        Assert.Equal(62, vm.IVTotal);
        Assert.NotEqual(0, vm.Stat_HP);
        Assert.False(string.IsNullOrEmpty(vm.Title));
    }

    [Fact]
    public void Audit_PokemonEditor_ModelSync()
    {
        var pkm = new PK7();
        pkm.TID16 = 12345;
        pkm.SID16 = 54321;
        var (vm, _, _) = TestHelpers.CreateTestViewModel(pkm, _saveFile);

        LogicAuditor.AuditModelSync(vm, vm.TargetPKM, nameof(vm.IvHP), nameof(pkm.IV_HP));
        LogicAuditor.AuditModelSync(vm, vm.TargetPKM, nameof(vm.IsEgg), nameof(pkm.IsEgg));
    }

    [Fact]
    public async Task Verify_EncounterDatabase_Functionality()
    {
        var sav = BlankSaveFile.Get(GameVersion.SL);
        var spriteMock = new Mock<ISpriteRenderer>();
        var dialogMock = new Mock<IDialogService>();
        var vm = new EncounterDatabaseViewModel(sav, spriteMock.Object, dialogMock.Object, _ => { });
        
        // Search for Pikachu (Species 25)
        vm.SelectedSpecies = 25;
        await vm.SearchCommand.ExecuteAsync(null);

        Assert.NotEmpty(vm.Results);
    }

    [Fact]
    public async Task Verify_PKMDatabase_Functionality()
    {
        var (vm, spriteMock, dialogMock) = TestHelpers.CreateTestViewModel(new PK3(), _saveFile);
        var dbVm = new PKMDatabaseViewModel(_saveFile, spriteMock.Object, dialogMock.Object);
        
        // Search current save (at least the active pkm should be there if it's in a box, 
        // but SAV3E starting blank might have nothing).
        // Let's at least check that the command exists and doesn't throw.
        await dbVm.SearchSaveCommand.ExecuteAsync(null);
        Assert.NotNull(dbVm.Results);
    }

    [Theory]
    [InlineData(3)] // Gen 3 - SAV3E
    [InlineData(4)] // Gen 4 - SAV4DP
    [InlineData(5)] // Gen 5 - SAV5BW
    public void Audit_MultiGen_InitialState(int gen)
    {
        // Use BlankSaveFile to get properly initialized saves
        SaveFile sav = gen switch {
            3 => BlankSaveFile.Get(GameVersion.E),
            4 => BlankSaveFile.Get(GameVersion.D),
            5 => BlankSaveFile.Get(GameVersion.B),
            _ => throw new ArgumentException()
        };
        
        PKM pkm = sav.BlankPKM;
        pkm.IV_HP = 31;
        
        var (vm, _, _) = TestHelpers.CreateTestViewModel(pkm, sav);
        Assert.Equal(31, vm.IVTotal);
    }
}
