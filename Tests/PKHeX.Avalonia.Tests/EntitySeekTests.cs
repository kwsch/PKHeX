using PKHeX.Presentation.ViewModels;
using PKHeX.Core;
using Xunit.Abstractions;

namespace PKHeX.Avalonia.Tests;

/// <summary>
/// Unit tests for <see cref="EntitySeekViewModel"/> using a stub <see cref="IBoxNavigator"/>,
/// so seek logic is verified independently of the box viewer / Avalonia.
/// </summary>
public class EntitySeekTests(ITestOutputHelper output)
{
    private sealed class StubNavigator(int boxCount, int slotsPerBox) : IBoxNavigator
    {
        public int BoxCount { get; } = boxCount;
        public int SlotsPerBox { get; } = slotsPerBox;
        public int CurrentBox { get; set; }
        public int CurrentSlot { get; set; }
        public (int Box, int Slot)? LastNavigation { get; private set; }

        public void NavigateTo(int box, int slot)
        {
            CurrentBox = box;
            CurrentSlot = slot;
            LastNavigation = (box, slot);
        }
    }

    [Fact]
    public void Seek_JumpsToMatchInLaterBox()
    {
        var sav = new SAV6XY();
        sav.SetBoxSlotAtIndex(new PK6 { Species = 25 }, 2, 5);
        var nav = new StubNavigator(sav.BoxCount, sav.BoxSlotCount);
        var vm = new EntitySeekViewModel(sav, nav);
        vm.Filter.Species = 25;

        vm.SeekNextCommand.Execute(null);

        Assert.Equal((2, 5), nav.LastNavigation);
        Assert.Contains("Box 3", vm.SeekStatus); // 1-based display
        output.WriteLine($"Seek -> {nav.LastNavigation}, status '{vm.SeekStatus}' ✓");
    }

    [Fact]
    public void Seek_NoMatch_ReportsStatus_AndDoesNotNavigate()
    {
        var sav = new SAV6XY();
        var nav = new StubNavigator(sav.BoxCount, sav.BoxSlotCount);
        var vm = new EntitySeekViewModel(sav, nav);
        vm.Filter.Species = 25; // none present

        vm.SeekNextCommand.Execute(null);

        Assert.Equal("No matches.", vm.SeekStatus);
        Assert.Null(nav.LastNavigation);
        output.WriteLine("No match: status set, no navigation ✓");
    }

    [Fact]
    public void Seek_WrapsAroundToEarlierBox()
    {
        var sav = new SAV6XY();
        sav.SetBoxSlotAtIndex(new PK6 { Species = 25 }, 0, 0); // only match is behind the cursor
        var nav = new StubNavigator(sav.BoxCount, sav.BoxSlotCount) { CurrentBox = 1, CurrentSlot = 3 };
        var vm = new EntitySeekViewModel(sav, nav);
        vm.Filter.Species = 25;

        vm.SeekNextCommand.Execute(null);

        Assert.Equal((0, 0), nav.LastNavigation);
        output.WriteLine("Seek wrapped around to box 0 ✓");
    }

    [Fact]
    public void Seek_Previous_FindsMatchBehindCursor()
    {
        var sav = new SAV6XY();
        sav.SetBoxSlotAtIndex(new PK6 { Species = 25 }, 0, 0);
        var nav = new StubNavigator(sav.BoxCount, sav.BoxSlotCount) { CurrentBox = 1, CurrentSlot = 0 };
        var vm = new EntitySeekViewModel(sav, nav);
        vm.Filter.Species = 25;

        vm.SeekPreviousCommand.Execute(null);

        Assert.Equal((0, 0), nav.LastNavigation);
        output.WriteLine("SeekPrevious found the earlier match ✓");
    }

    [Fact]
    public void Seek_BatchInstruction_FiltersByProperty()
    {
        var sav = new SAV6XY();
        // Two Pikachu: one flawless IVs, one zero IVs. Batch filter should pick only the flawless one.
        var flawless = new PK6 { Species = 25, IV_HP = 31, IV_ATK = 31, IV_DEF = 31, IV_SPA = 31, IV_SPD = 31, IV_SPE = 31 };
        var weak = new PK6 { Species = 25 };
        sav.SetBoxSlotAtIndex(weak, 0, 0);
        sav.SetBoxSlotAtIndex(flawless, 1, 0);

        var nav = new StubNavigator(sav.BoxCount, sav.BoxSlotCount);
        var vm = new EntitySeekViewModel(sav, nav);
        vm.Filter.Species = 25;
        vm.Filter.BatchInstructions = "IV_HP=31"; // batch-editor filter syntax

        vm.SeekNextCommand.Execute(null);

        Assert.Equal((1, 0), nav.LastNavigation); // skipped the weak one in box 0
        output.WriteLine($"Batch instruction filtered to {nav.LastNavigation} ✓");
    }
}
