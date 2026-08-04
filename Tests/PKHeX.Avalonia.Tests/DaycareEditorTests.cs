using Moq;
using PKHeX.Avalonia.Services;
using PKHeX.Presentation.ViewModels;
using PKHeX.Core;
using Xunit.Abstractions;

namespace PKHeX.Avalonia.Tests;

/// <summary>
/// Behavioral tests for DaycareEditorViewModel.
/// Daycare is generation-specific: Gen2 (Crystal), Gen3-6 expose IDaycareExperience,
/// Gen7/8b have paired slots, Gen1/Gen7b/9 have no daycare at all.
/// </summary>
public class DaycareEditorTests(ITestOutputHelper output)
{
    private static Mock<ISpriteRenderer> SpriteMock() => new();

    // -----------------------------------------------------------------------
    // 1. Saves with daycares report HasDaycare=true with correct slot count
    // -----------------------------------------------------------------------

    private static readonly (GameVersion Version, int ExpectedSlots, string Label)[] DaycareVersions =
    [
        (GameVersion.C,  2, "Gen2-Crystal"),
        (GameVersion.E,  2, "Gen3-Emerald"),
        (GameVersion.Pt, 2, "Gen4-Platinum"),
        (GameVersion.W2, 2, "Gen5-White2"),
        (GameVersion.X,  2, "Gen6-X"),
        (GameVersion.SN, 2, "Gen7-Sun"),
        (GameVersion.BD, 2, "Gen8b-BrilliantDiamond"),
    ];

    public static IEnumerable<object[]> DaycareData()
    {
        foreach (var (v, slots, label) in DaycareVersions)
        {
            yield return [BlankSaveFile.Get(v), slots, label];
        }
    }

    [Theory, MemberData(nameof(DaycareData))]
    public void Daycare_HasDaycare_True_And_SlotCount_Correct(SaveFile sav, int expectedSlots, string label)
    {
        var vm = new DaycareEditorViewModel(sav, SpriteMock().Object);

        Assert.True(vm.HasDaycare, $"{label}: expected HasDaycare=true");
        Assert.Equal(expectedSlots, vm.SlotCount);
        Assert.Equal(expectedSlots, vm.Slots.Count);
        output.WriteLine($"{label}: HasDaycare=true, Slots={vm.SlotCount} ✓");
    }

    // -----------------------------------------------------------------------
    // 2. Saves without daycares report HasDaycare=false with 0 slots
    // -----------------------------------------------------------------------

    [Fact]
    public void Daycare_Gen9_HasNoDaycare()
    {
        var sav = BlankSaveFile.Get(GameVersion.SL);
        var vm = new DaycareEditorViewModel(sav, SpriteMock().Object);

        Assert.False(vm.HasDaycare);
        Assert.Equal(0, vm.SlotCount);
        Assert.Empty(vm.Slots);
        output.WriteLine("Gen9 Scarlet: HasDaycare=false, no slots ✓");
    }

    [Fact]
    public void Daycare_Gen7b_HasNoDaycare()
    {
        var sav = BlankSaveFile.Get(GameVersion.GP);
        var vm = new DaycareEditorViewModel(sav, SpriteMock().Object);

        Assert.False(vm.HasDaycare);
        Assert.Equal(0, vm.SlotCount);
        output.WriteLine("Gen7b LGPE: HasDaycare=false, no slots ✓");
    }

    // -----------------------------------------------------------------------
    // 3. Toggling IsOccupied immediately writes to the save via property change
    // -----------------------------------------------------------------------

    [Fact]
    public void Daycare_Gen6_SetSlotOccupied_WritesBackToSave()
    {
        var sav = new SAV6XY();
        var vm = new DaycareEditorViewModel(sav, SpriteMock().Object);

        Assert.True(vm.HasDaycare);
        Assert.Equal(2, vm.SlotCount);

        // Slot 0 starts unoccupied in a blank save
        var slot0 = vm.Slots[0];
        var initialOccupied = slot0.IsOccupied;
        output.WriteLine($"Gen6 slot0 initial IsOccupied={initialOccupied}");

        // Toggle and verify it writes back via the property change handler
        slot0.IsOccupied = !initialOccupied;
        var storage = (IDaycareStorage)sav;
        Assert.Equal(!initialOccupied, storage.IsDaycareOccupied(0));
        output.WriteLine($"Gen6 slot0 IsOccupied={!initialOccupied} persisted to save ✓");
    }

    // -----------------------------------------------------------------------
    // 4. HasExperience reflects IDaycareExperience interface correctly
    // -----------------------------------------------------------------------

    [Fact]
    public void Daycare_Gen3_HasExperience_True()
    {
        var sav = BlankSaveFile.Get(GameVersion.E);
        var vm = new DaycareEditorViewModel(sav, SpriteMock().Object);

        Assert.True(vm.HasExperience, "Gen3 Emerald should expose IDaycareExperience");
        output.WriteLine("Gen3: HasExperience=true ✓");
    }

    [Fact]
    public void Daycare_Gen7_HasExperience_False()
    {
        var sav = BlankSaveFile.Get(GameVersion.SN);
        var vm = new DaycareEditorViewModel(sav, SpriteMock().Object);

        Assert.False(vm.HasExperience, "Gen7 Sun does not expose IDaycareExperience");
        output.WriteLine("Gen7: HasExperience=false ✓");
    }

    // -----------------------------------------------------------------------
    // 5. HasEggState reflects IDaycareEggState interface
    // -----------------------------------------------------------------------

    [Fact]
    public void Daycare_Gen6_HasEggState_True()
    {
        var sav = new SAV6XY();
        var vm = new DaycareEditorViewModel(sav, SpriteMock().Object);

        Assert.True(vm.HasEggState);
        output.WriteLine("Gen6: HasEggState=true ✓");
    }

    // -----------------------------------------------------------------------
    // 6. Toggling IsEggAvailable writes directly to save via OnChanged
    // -----------------------------------------------------------------------

    [Fact]
    public void Daycare_Gen6_IsEggAvailable_WritesBackToSave()
    {
        var sav = new SAV6XY();
        var vm = new DaycareEditorViewModel(sav, SpriteMock().Object);

        var initial = vm.IsEggAvailable;
        vm.IsEggAvailable = !initial;

        var eggState = (IDaycareEggState)sav;
        Assert.Equal(!initial, eggState.IsEggAvailable);
        output.WriteLine($"Gen6 IsEggAvailable={!initial} persisted to save ✓");
    }

    // -----------------------------------------------------------------------
    // 7. RefreshDaycare reloads slot data without throwing
    // -----------------------------------------------------------------------

    [Fact]
    public void Daycare_Gen5_Refresh_DoesNotThrow()
    {
        var sav = BlankSaveFile.Get(GameVersion.W2);
        var vm = new DaycareEditorViewModel(sav, SpriteMock().Object);

        var ex = Record.Exception(() => vm.RefreshDaycareCommand.Execute(null));
        Assert.Null(ex);
        Assert.Equal(2, vm.Slots.Count);
        output.WriteLine("Gen5 Daycare refresh: no exception, 2 slots ✓");
    }
}
