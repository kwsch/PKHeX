using PKHeX.Presentation.ViewModels;
using PKHeX.Core;
using Xunit.Abstractions;

namespace PKHeX.Avalonia.Tests;

/// <summary>
/// Behavioral tests for RTC3EditorViewModel.
/// Only applicable to Gen3 Hoenn saves (RS/E/FRLG that implement IGen3Hoenn).
/// Uses SAV3E (Emerald) which implements IGen3Hoenn.
/// </summary>
public class RTC3EditorTests(ITestOutputHelper output)
{
    private static SAV3E CreateSave() => new(new byte[0x20000]);

    // -----------------------------------------------------------------------
    // 1. Initial load reads clock fields from save
    // -----------------------------------------------------------------------

    [Fact]
    public void RTC3_Load_ReadsClockFields()
    {
        var sav = CreateSave();
        var vm = new RTC3EditorViewModel(sav);

        // All fields should be accessible without throwing
        _ = vm.InitialDay;
        _ = vm.InitialHour;
        _ = vm.InitialMinute;
        _ = vm.InitialSecond;
        _ = vm.ElapsedDay;
        _ = vm.ElapsedHour;
        _ = vm.ElapsedMinute;
        _ = vm.ElapsedSecond;

        output.WriteLine($"Load: Initial={vm.InitialDay}d {vm.InitialHour}h:{vm.InitialMinute}m:{vm.InitialSecond}s, " +
                         $"Elapsed={vm.ElapsedDay}d {vm.ElapsedHour}h:{vm.ElapsedMinute}m:{vm.ElapsedSecond}s ✓");
    }

    // -----------------------------------------------------------------------
    // 2. Hours/Minutes/Seconds are clamped to valid ranges on load
    // -----------------------------------------------------------------------

    [Fact]
    public void RTC3_Load_ClampsTimeToValidRanges()
    {
        var sav = CreateSave();
        var vm = new RTC3EditorViewModel(sav);

        Assert.InRange(vm.InitialHour,   0, 23);
        Assert.InRange(vm.InitialMinute, 0, 59);
        Assert.InRange(vm.InitialSecond, 0, 59);
        Assert.InRange(vm.ElapsedHour,   0, 23);
        Assert.InRange(vm.ElapsedMinute, 0, 59);
        Assert.InRange(vm.ElapsedSecond, 0, 59);
        output.WriteLine("RTC3 time fields clamped to valid ranges ✓");
    }

    // -----------------------------------------------------------------------
    // 3. ResetCommand zeros all clock fields
    // -----------------------------------------------------------------------

    [Fact]
    public void RTC3_ResetCommand_ZerosAllFields()
    {
        var sav = CreateSave();
        var vm = new RTC3EditorViewModel(sav);

        // Set some values
        vm.InitialDay    = 10;
        vm.InitialHour   = 12;
        vm.ElapsedDay    = 365;
        vm.ElapsedMinute = 30;

        vm.ResetCommand.Execute(null);

        Assert.Equal(0, vm.InitialDay);
        Assert.Equal(0, vm.InitialHour);
        Assert.Equal(0, vm.InitialMinute);
        Assert.Equal(0, vm.InitialSecond);
        Assert.Equal(0, vm.ElapsedDay);
        Assert.Equal(0, vm.ElapsedHour);
        Assert.Equal(0, vm.ElapsedMinute);
        Assert.Equal(0, vm.ElapsedSecond);
        output.WriteLine("ResetCommand: all clock fields zeroed ✓");
    }

    // -----------------------------------------------------------------------
    // 4. BerryFixCommand ensures ElapsedDay is at least berry-glitch threshold
    // -----------------------------------------------------------------------

    [Fact]
    public void RTC3_BerryFixCommand_SetsMinimumElapsedDays()
    {
        var sav = CreateSave();
        var vm = new RTC3EditorViewModel(sav);
        const int berryFixThreshold = (2 * 366) + 2; // 734

        vm.ElapsedDay = 0;
        vm.BerryFixCommand.Execute(null);

        Assert.True(vm.ElapsedDay >= berryFixThreshold,
            $"ElapsedDay should be >= {berryFixThreshold} after berry fix, got {vm.ElapsedDay}");
        output.WriteLine($"BerryFix: ElapsedDay={vm.ElapsedDay} >= {berryFixThreshold} ✓");
    }

    [Fact]
    public void RTC3_BerryFixCommand_PreservesLargerElapsedDay()
    {
        var sav = CreateSave();
        var vm = new RTC3EditorViewModel(sav);

        // If already above threshold, should stay at that value
        vm.ElapsedDay = 1000;
        vm.BerryFixCommand.Execute(null);

        Assert.Equal(1000, vm.ElapsedDay);
        output.WriteLine("BerryFix: preserves ElapsedDay=1000 when already above threshold ✓");
    }

    // -----------------------------------------------------------------------
    // 5. SaveCommand persists to the underlying save
    // -----------------------------------------------------------------------

    [Fact]
    public void RTC3_SaveCommand_PersistsToSave()
    {
        var sav = CreateSave();
        var vm = new RTC3EditorViewModel(sav);

        vm.InitialDay    = 5;
        vm.InitialHour   = 10;
        vm.InitialMinute = 30;
        vm.InitialSecond = 15;

        vm.ElapsedDay    = 100;
        vm.ElapsedHour   = 8;
        vm.ElapsedMinute = 45;
        vm.ElapsedSecond = 0;

        vm.SaveCommand.Execute(null);

        // Verify by loading a fresh VM from the same save
        var vm2 = new RTC3EditorViewModel(sav);

        Assert.Equal(5,   vm2.InitialDay);
        Assert.Equal(10,  vm2.InitialHour);
        Assert.Equal(30,  vm2.InitialMinute);
        Assert.Equal(15,  vm2.InitialSecond);
        Assert.Equal(100, vm2.ElapsedDay);
        Assert.Equal(8,   vm2.ElapsedHour);
        Assert.Equal(45,  vm2.ElapsedMinute);
        Assert.Equal(0,   vm2.ElapsedSecond);
        output.WriteLine("RTC3 Save+Reload round-trip ✓");
    }

    // -----------------------------------------------------------------------
    // 6. BerryFix + Save round-trips correctly
    // -----------------------------------------------------------------------

    [Fact]
    public void RTC3_BerryFixThenSave_RoundTrips()
    {
        var sav = CreateSave();
        var vm = new RTC3EditorViewModel(sav);

        vm.BerryFixCommand.Execute(null);
        var elapsedAfterFix = vm.ElapsedDay;
        vm.SaveCommand.Execute(null);

        var vm2 = new RTC3EditorViewModel(sav);
        Assert.Equal(elapsedAfterFix, vm2.ElapsedDay);
        output.WriteLine($"BerryFix+Save round-trip: ElapsedDay={elapsedAfterFix} ✓");
    }
}
