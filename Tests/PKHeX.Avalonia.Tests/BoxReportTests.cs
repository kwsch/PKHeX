using Avalonia.Headless.XUnit;
using Moq;
using PKHeX.Core;
using PKHeX.Presentation.ViewModels;

namespace PKHeX.Avalonia.Tests;

public class BoxReportTests
{
    private readonly Mock<IDialogService> _dialogServiceMock = new();

    private static SAV3E CreateSaveWithBoxMon(out PK3 pk)
    {
        var sav = new SAV3E();
        pk = new PK3
        {
            Species = (ushort)Species.Mudkip,
            CurrentLevel = 5,
        };
        sav.SetBoxSlotAtIndex(pk, 1, 3); // box 1, slot 3
        return sav;
    }

    [AvaloniaFact]
    public void Refresh_BuildsOneRowPerOccupiedSlot()
    {
        var sav = CreateSaveWithBoxMon(out _);
        var vm = new BoxReportViewModel(sav, _dialogServiceMock.Object);

        var row = Assert.Single(vm.Rows);
        Assert.Equal(1, row.Box);
        Assert.Equal(3, row.Slot);
        Assert.Equal("B02:04", row.Position);
        Assert.Equal((byte)5, row.Level);
        Assert.Contains("1 Pokémon", vm.StatusText);
    }

    [AvaloniaFact]
    public void Refresh_PicksUpBoxChanges()
    {
        var sav = CreateSaveWithBoxMon(out var pk);
        var vm = new BoxReportViewModel(sav, _dialogServiceMock.Object);
        Assert.Single(vm.Rows);

        sav.SetBoxSlotAtIndex(pk, 0, 0);
        vm.Refresh();

        Assert.Equal(2, vm.Rows.Count);
    }

    [AvaloniaFact]
    public void ActivateSelectedRow_RaisesRowActivated()
    {
        var sav = CreateSaveWithBoxMon(out _);
        var vm = new BoxReportViewModel(sav, _dialogServiceMock.Object);

        BoxReportRow? activated = null;
        vm.RowActivated += r => activated = r;

        vm.ActivateSelectedRowCommand.Execute(null); // no selection: no event
        Assert.Null(activated);

        vm.SelectedRow = vm.Rows[0];
        vm.ActivateSelectedRowCommand.Execute(null);
        Assert.Same(vm.Rows[0], activated);
    }

    [AvaloniaFact]
    public async Task ExportCsv_WritesHeaderAndRows()
    {
        var sav = CreateSaveWithBoxMon(out _);
        var vm = new BoxReportViewModel(sav, _dialogServiceMock.Object);

        var path = Path.Combine(Path.GetTempPath(), $"boxreport-test-{Guid.NewGuid():N}.csv");
        _dialogServiceMock
            .Setup(d => d.SaveFileAsync(It.IsAny<string>(), It.IsAny<string?>(), It.IsAny<string[]?>()))
            .ReturnsAsync(path);

        try
        {
            await vm.ExportCsvCommand.ExecuteAsync(null);

            var lines = File.ReadAllLines(path);
            Assert.Equal(2, lines.Length); // header + 1 row
            Assert.StartsWith("Position,Species,", lines[0]);
            Assert.StartsWith("B02:04,", lines[1]);
        }
        finally
        {
            File.Delete(path);
        }
    }
}
