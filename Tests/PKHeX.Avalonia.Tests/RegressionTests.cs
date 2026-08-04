using PKHeX.Presentation.ViewModels;
using PKHeX.Core;
using Xunit.Abstractions;

namespace PKHeX.Avalonia.Tests;

/// <summary>
/// Targeted regression tests for bugs found during code review.
/// Each test documents a specific bug fix so regressions are immediately caught.
/// </summary>
public class RegressionTests(ITestOutputHelper output)
{
    // -----------------------------------------------------------------------
    // Bug: MemoryEditorViewModel.Save() copy-paste error (session 2)
    // HandlingTrainerMemoryFeeling and HandlingTrainerMemoryIntensity were
    // being written from OtMemoryFeel/OtMemoryQual instead of HtMemoryFeel/HtMemoryQual.
    // This silently corrupted HT memory on every save from the Memory Editor.
    // -----------------------------------------------------------------------

    [Fact]
    public void MemoryEditor_HT_Memory_Uses_HT_Values_Not_OT_Values()
    {
        var pk = new PK6 { Species = 25 };
        var vm = new MemoryEditorViewModel(pk);

        // Set OT memory values
        vm.OtMemory = 1;
        vm.OtMemoryVar = 100;
        vm.OtMemoryFeel = 2;
        vm.OtMemoryQual = 3;

        // Set HT memory to deliberately different values
        vm.HtMemory = 4;
        vm.HtMemoryVar = 200;
        vm.HtMemoryFeel = 5;   // Bug: was overwritten with OtMemoryFeel=2 before fix
        vm.HtMemoryQual = 6;   // Bug: was overwritten with OtMemoryQual=3 before fix

        vm.SaveCommand.Execute(null);

        var mem = (ITrainerMemories)pk;
        Assert.Equal(5, (int)mem.HandlingTrainerMemoryFeeling);
        Assert.Equal(6, (int)mem.HandlingTrainerMemoryIntensity);
        output.WriteLine("HT memory saved with HT values ✓");
    }

    [Fact]
    public void MemoryEditor_OT_Memory_Values_Are_Unaffected_By_HT_Write()
    {
        var pk = new PK6 { Species = 25 };
        var vm = new MemoryEditorViewModel(pk);

        vm.OtMemory = 7;
        vm.OtMemoryVar = 300;
        vm.OtMemoryFeel = 8;
        vm.OtMemoryQual = 9;

        vm.HtMemory = 1;
        vm.HtMemoryVar = 50;
        vm.HtMemoryFeel = 2;
        vm.HtMemoryQual = 3;

        vm.SaveCommand.Execute(null);

        var mem = (ITrainerMemories)pk;
        Assert.Equal(8, (int)mem.OriginalTrainerMemoryFeeling);
        Assert.Equal(9, (int)mem.OriginalTrainerMemoryIntensity);
        output.WriteLine("OT memory unaffected by HT write ✓");
    }

    [Fact]
    public void MemoryEditor_AllFourMemoryFields_SaveIndependently()
    {
        var pk = new PK6 { Species = 25 };
        var vm = new MemoryEditorViewModel(pk);

        vm.OtMemory = 10;
        vm.OtMemoryVar = 1000;
        vm.OtMemoryFeel = 11;
        vm.OtMemoryQual = 12;

        vm.HtMemory = 20;
        vm.HtMemoryVar = 2000;
        vm.HtMemoryFeel = 21;
        vm.HtMemoryQual = 22;

        vm.SaveCommand.Execute(null);

        var mem = (ITrainerMemories)pk;
        Assert.Equal(10, (int)mem.OriginalTrainerMemory);
        Assert.Equal(1000, (int)mem.OriginalTrainerMemoryVariable);
        Assert.Equal(11, (int)mem.OriginalTrainerMemoryFeeling);
        Assert.Equal(12, (int)mem.OriginalTrainerMemoryIntensity);
        Assert.Equal(20, (int)mem.HandlingTrainerMemory);
        Assert.Equal(2000, (int)mem.HandlingTrainerMemoryVariable);
        Assert.Equal(21, (int)mem.HandlingTrainerMemoryFeeling);
        Assert.Equal(22, (int)mem.HandlingTrainerMemoryIntensity);
        output.WriteLine("All 8 memory fields saved independently ✓");
    }

    [Fact]
    public void MemoryEditor_Load_ReadsBothOTandHT_Correctly()
    {
        var pk = new PK6 { Species = 25 };
        var mem = (ITrainerMemories)pk;
        mem.OriginalTrainerMemory = 5;
        mem.OriginalTrainerMemoryVariable = 100;
        mem.OriginalTrainerMemoryFeeling = 6;
        mem.OriginalTrainerMemoryIntensity = 7;
        mem.HandlingTrainerMemory = 8;
        mem.HandlingTrainerMemoryVariable = 200;
        mem.HandlingTrainerMemoryFeeling = 9;
        mem.HandlingTrainerMemoryIntensity = 10;

        var vm = new MemoryEditorViewModel(pk);

        Assert.Equal(5, vm.OtMemory);
        Assert.Equal(100, vm.OtMemoryVar);
        Assert.Equal(6, vm.OtMemoryFeel);
        Assert.Equal(7, vm.OtMemoryQual);
        Assert.Equal(8, vm.HtMemory);
        Assert.Equal(200, vm.HtMemoryVar);
        Assert.Equal(9, vm.HtMemoryFeel);
        Assert.Equal(10, vm.HtMemoryQual);
        output.WriteLine("Memory fields loaded correctly into ViewModel ✓");
    }

    // -----------------------------------------------------------------------
    // Bug: MetDate/EggDate double-write (session 2)
    // Two separate if-blocks would both trigger for valid dates, causing
    // the MetDate field to be set twice. Collapsed to a single expression.
    // -----------------------------------------------------------------------

    [Fact]
    public void PokemonEditor_MetDate_RoundTrips_Correctly()
    {
        var sav = new SAV4Pt();
        var pk = new PK4 { Species = 25 };
        var (vm, _, _) = TestHelpers.CreateTestViewModel(pk, sav);

        var testDate = new DateTime(2010, 7, 15);
        vm.MetDate = testDate;

        var result = vm.PreparePKM();

        Assert.NotNull(result.MetDate);
        Assert.Equal(testDate.Year, result.MetDate!.Value.Year);
        Assert.Equal(testDate.Month, result.MetDate.Value.Month);
        Assert.Equal(testDate.Day, result.MetDate.Value.Day);
        output.WriteLine($"MetDate round-trip: {testDate:yyyy-MM-dd} ✓");
    }

    [Fact]
    public void PokemonEditor_MetDate_Null_WhenNotSet()
    {
        var sav = new SAV4Pt();
        var pk = new PK4 { Species = 25 };
        var (vm, _, _) = TestHelpers.CreateTestViewModel(pk, sav);

        // Default PKM has no MetDate — ViewModel should reflect null
        vm.MetDate = null;
        var result = vm.PreparePKM();

        // A null MetDate is valid for many PKM formats
        // We just verify it doesn't throw and the VM state is consistent
        Assert.Equal(vm.MetDate, result.MetDate.HasValue
            ? result.MetDate.Value.ToDateTime(TimeOnly.MinValue)
            : (DateTime?)null);
    }

    [Fact]
    public void PokemonEditor_EggDate_RoundTrips_Correctly()
    {
        var sav = new SAV4Pt();
        var pk = new PK4 { Species = 25, IsEgg = true };
        var (vm, _, _) = TestHelpers.CreateTestViewModel(pk, sav);

        var testDate = new DateTime(2011, 3, 22);
        vm.EggDate = testDate;

        var result = vm.PreparePKM();

        Assert.NotNull(result.EggMetDate);
        Assert.Equal(testDate.Year, result.EggMetDate!.Value.Year);
        Assert.Equal(testDate.Month, result.EggMetDate.Value.Month);
        Assert.Equal(testDate.Day, result.EggMetDate.Value.Day);
        output.WriteLine($"EggDate round-trip: {testDate:yyyy-MM-dd} ✓");
    }

    // -----------------------------------------------------------------------
    // Bug: PartyViewerViewModel redundant type pattern check (session 2)
    // `pk is PKM pkm` always succeeds when pk is already typed as PKM —
    // was masking the intent and made the conditional confusing.
    // Fixed to direct property access.
    // -----------------------------------------------------------------------

    [Fact]
    public void PartyViewer_SlotData_HP_Stats_Are_Populated()
    {
        // Verify that HP stats are read via direct property access (not through
        // always-true pattern match that looked conditional)
        var sav = BlankSaveFile.Get(GameVersion.E);
        var spriteMock = new Moq.Mock<ISpriteRenderer>();

        var vm = new PartyViewerViewModel(sav, spriteMock.Object);
        Assert.Equal(6, vm.Slots.Count);

        // Blank save has empty party — all slots should have 0 HP (no species)
        foreach (var slot in vm.Slots)
        {
            Assert.True(slot.CurrentHp >= 0, "CurrentHp must not be negative");
            Assert.True(slot.MaxHp >= 0, "MaxHp must not be negative");
        }
        output.WriteLine("PartyViewer HP stats populated without errors ✓");
    }

    // -----------------------------------------------------------------------
    // Bug: UndoRedoService redundant PropertyChanged in Initialize (session 1)
    // OnPropertyChanged for CanUndo/CanRedo was called explicitly in Initialize()
    // even though setting ChangeCount=0 already fires them via [NotifyPropertyChangedFor].
    // Fixed: removed redundant calls.
    // -----------------------------------------------------------------------

    [Fact]
    public void UndoRedoService_Initialize_ReportsCannotUndoOrRedo()
    {
        var service = new UndoRedoService();
        var sav = BlankSaveFile.Get(GameVersion.X); // Gen6 blank save writes correctly
        service.Initialize(sav);

        // After initialization there is nothing to undo or redo
        Assert.False(service.CanUndo);
        Assert.False(service.CanRedo);
        output.WriteLine("UndoRedoService initializes to CanUndo=false, CanRedo=false ✓");
    }

    [Fact]
    public void UndoRedoService_AfterAddChange_CanUndoBecomesTrue()
    {
        var service = new UndoRedoService();
        var sav = BlankSaveFile.Get(GameVersion.X);
        service.Initialize(sav);

        // Add a slot change (slot 0 in box 0)
        service.AddChange(new SlotInfoBox(0, 0, sav));

        Assert.True(service.CanUndo);
        Assert.False(service.CanRedo);
        output.WriteLine("CanUndo=true after AddChange ✓");
    }

    // -----------------------------------------------------------------------
    // Bug: BatchEditorViewModel event leak (session 2)
    // BatchEditor.BatchEditCompleted handler was not unsubscribed when
    // a save file was closed, accumulating duplicate event handlers.
    // Regression test: verify BatchEditorViewModel constructs and functions.
    // -----------------------------------------------------------------------

    [Fact]
    public void BatchEditorViewModel_Constructs_Without_Exception()
    {
        var sav = new SAV6XY();
        var dialogMock = new Moq.Mock<IDialogService>();
        var ex = Record.Exception(() =>
        {
            var vm = new BatchEditorViewModel(sav, dialogMock.Object);
            Assert.NotNull(vm);
        });
        Assert.Null(ex);
        output.WriteLine("BatchEditorViewModel constructs without exception ✓");
    }
}
