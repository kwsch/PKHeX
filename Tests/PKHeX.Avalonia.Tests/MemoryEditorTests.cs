using PKHeX.Presentation.ViewModels;
using PKHeX.Core;
using Xunit.Abstractions;

namespace PKHeX.Avalonia.Tests;

/// <summary>
/// Behavioral tests for MemoryEditorViewModel.
/// The VM loads from and saves back to PKM via interface checks:
/// IGeoTrack, IFullnessEnjoyment, IAffection, ITrainerMemories.
/// </summary>
public class MemoryEditorTests(ITestOutputHelper output)
{
    // -----------------------------------------------------------------------
    // 1. OT friendship round-trips for any PKM
    // -----------------------------------------------------------------------

    [Fact]
    public void Memory_OtFriendship_RoundTrips()
    {
        var pk = new PK5 { Species = 1, OriginalTrainerFriendship = 50 };
        var vm = new MemoryEditorViewModel(pk);

        Assert.Equal(50, vm.OtFriendship);

        vm.OtFriendship = 200;
        vm.SaveCommand.Execute(null);

        Assert.Equal(200, pk.OriginalTrainerFriendship);
        output.WriteLine("OT Friendship round-trip: 50 → 200 ✓");
    }

    // -----------------------------------------------------------------------
    // 2. Gen6 PKM (PK6) supports all memory interfaces
    // -----------------------------------------------------------------------

    [Fact]
    public void Memory_Gen6_AllFields_RoundTrip()
    {
        var pk = new PK6 { Species = 25 };

        var vm = new MemoryEditorViewModel(pk);

        // Geo
        vm.Country0 = 1;
        vm.Region0  = 2;

        // Fullness / Enjoyment
        vm.Fullness  = 100;
        vm.Enjoyment = 50;

        // Affection
        vm.OtAffection = 255;
        vm.HtAffection = 128;

        // Memories
        vm.OtMemory    = 5;
        vm.OtMemoryVar = 10;
        vm.OtMemoryFeel = 4;
        vm.OtMemoryQual = 7;

        vm.SaveCommand.Execute(null);

        var g = (IGeoTrack)pk;
        Assert.Equal(1, g.Geo1_Country);
        Assert.Equal(2, g.Geo1_Region);

        var f = (IFullnessEnjoyment)pk;
        Assert.Equal(100, f.Fullness);
        Assert.Equal(50,  f.Enjoyment);

        var a = (IAffection)pk;
        Assert.Equal(255, a.OriginalTrainerAffection);
        Assert.Equal(128, a.HandlingTrainerAffection);

        var m = (ITrainerMemories)pk;
        Assert.Equal(5,  m.OriginalTrainerMemory);
        Assert.Equal(10, m.OriginalTrainerMemoryVariable);
        Assert.Equal(4,  m.OriginalTrainerMemoryFeeling);
        Assert.Equal(7,  m.OriginalTrainerMemoryIntensity);

        output.WriteLine("Gen6 all memory fields round-tripped ✓");
    }

    // -----------------------------------------------------------------------
    // 3. Gen3 PKM has no geo/affection/memories — no crash on save
    // -----------------------------------------------------------------------

    [Fact]
    public void Memory_Gen3_NoInterfaceFields_NoThrow()
    {
        var pk = new PK3 { Species = 1, OriginalTrainerFriendship = 70 };
        var vm = new MemoryEditorViewModel(pk);

        // Gen3 doesn't implement IGeoTrack / IAffection / ITrainerMemories
        var ex = Record.Exception(() =>
        {
            vm.OtFriendship = 100;
            vm.Country0 = 1;
            vm.OtAffection = 255;
            vm.OtMemory = 3;
            vm.SaveCommand.Execute(null);
        });

        Assert.Null(ex);
        // Only OT friendship should persist
        Assert.Equal(100, pk.OriginalTrainerFriendship);
        output.WriteLine("Gen3 no-interface fields: no exception, friendship saved ✓");
    }

    // -----------------------------------------------------------------------
    // 4. CloseCommand invokes the close callback without saving
    // -----------------------------------------------------------------------

    [Fact]
    public void Memory_CloseCommand_InvokesCallbackWithoutSaving()
    {
        var pk = new PK6 { Species = 1, OriginalTrainerFriendship = 50 };
        bool closed = false;
        var vm = new MemoryEditorViewModel(pk, () => closed = true);

        vm.OtFriendship = 200;
        vm.CloseCommand.Execute(null);

        Assert.True(closed, "Close callback should have been invoked");
        // Friendship NOT saved since we called Close, not Save
        Assert.Equal(50, pk.OriginalTrainerFriendship);
        output.WriteLine("CloseCommand: callback invoked, changes not persisted ✓");
    }

    // -----------------------------------------------------------------------
    // 5. SaveCommand invokes close callback
    // -----------------------------------------------------------------------

    [Fact]
    public void Memory_SaveCommand_InvokesCloseCallback()
    {
        var pk = new PK5 { Species = 1 };
        bool closed = false;
        var vm = new MemoryEditorViewModel(pk, () => closed = true);

        vm.SaveCommand.Execute(null);

        Assert.True(closed, "SaveCommand should invoke close callback");
        output.WriteLine("SaveCommand: close callback invoked ✓");
    }

    // -----------------------------------------------------------------------
    // 6. Load reads existing values from PKM
    // -----------------------------------------------------------------------

    [Fact]
    public void Memory_Load_ReadsExistingValues()
    {
        var pk = new PK6 { Species = 1 };
        var a = (IAffection)pk;
        a.OriginalTrainerAffection = 42;

        var m = (ITrainerMemories)pk;
        m.OriginalTrainerMemory = 7;

        var vm = new MemoryEditorViewModel(pk);

        Assert.Equal(42, vm.OtAffection);
        Assert.Equal(7,  vm.OtMemory);
        output.WriteLine("Load: existing Gen6 values read into VM ✓");
    }
}
