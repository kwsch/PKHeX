using Avalonia.Headless.XUnit;
using Moq;
using PKHeX.Application.UseCases;
using PKHeX.Avalonia.Tests.Fixtures;
using PKHeX.Core;
using PKHeX.Presentation.ViewModels;
using Xunit.Abstractions;

namespace PKHeX.Avalonia.Tests;

public class LegalityAuditTests(ITestOutputHelper output)
{
    private readonly Mock<IDialogService> _dialogServiceMock = new();

    private SaveFile? GetEmerald(out string? skipReason)
    {
        var savPath = SaveFileFixture.FindSaveFilesPath();
        if (savPath == null) { skipReason = "savefiles directory not found"; return null; }

        var sav = SaveFileFixture.LoadSave(Path.Combine(savPath, "gen3_emerald.sav"));
        if (sav == null) { skipReason = "gen3_emerald.sav missing"; return null; }

        skipReason = null;
        return sav;
    }

    /// <summary>A synchronous <see cref="IProgress{T}"/> for deterministic cancellation tests
    /// (the real <see cref="Progress{T}"/> marshals callbacks asynchronously).</summary>
    private sealed class SyncProgress<T> : IProgress<T>
    {
        private readonly Action<T> _callback;
        public SyncProgress(Action<T> callback) => _callback = callback;
        public void Report(T value) => _callback(value);
    }

    [AvaloniaFact]
    public async Task Run_MatchesDirectLegalityAnalysisForEverySlot()
    {
        var sav = GetEmerald(out var skip);
        if (sav is null) { output.WriteLine($"Skip: {skip}"); return; }

        var vm = new LegalityAuditViewModel(sav, _dialogServiceMock.Object);
        await vm.RunCommand.ExecuteAsync(null);

        Assert.NotEmpty(vm.Rows);

        foreach (var row in vm.Rows)
        {
            var expected = new LegalityAnalysis(row.Entity, sav.Personal);
            Assert.Equal(expected.Valid, row.Valid);
            Assert.Equal(expected.Report(), row.ReportText);
        }
    }

    [AvaloniaFact]
    public void Execute_SkipsEmptySlotsAndEggsDoNotFalsePositive()
    {
        var sav = new SAV3E();
        var egg = new PK3
        {
            Species = (ushort)Species.Togepi,
            IsEgg = true,
            CurrentLevel = 5,
            OriginalTrainerName = sav.OT,
            TID16 = sav.TID16,
            SID16 = sav.SID16,
        };
        egg.RefreshChecksum();
        sav.SetBoxSlotAtIndex(egg, 2, 0); // box 2, slot 0; all other slots remain empty

        var useCase = new LegalityAuditUseCase();
        var entries = useCase.Execute(sav);

        // Only the one occupied slot should produce an entry; every empty slot is skipped
        // (no false-positive findings just because a slot is empty).
        var entry = Assert.Single(entries);
        Assert.Equal(2, entry.Box);
        Assert.Equal(0, entry.Slot);
    }

    [AvaloniaFact]
    public void Execute_EmptySaveReportsZeroFindings()
    {
        var sav = new SAV3E();
        var useCase = new LegalityAuditUseCase();

        var entries = useCase.Execute(sav);

        Assert.Empty(entries);
    }

    [AvaloniaFact]
    public void Execute_CancellationStopsFurtherProcessing()
    {
        var sav = new SAV3E();
        var pk = new PK3 { Species = (ushort)Species.Mudkip, CurrentLevel = 5 };
        pk.RefreshChecksum();
        sav.SetBoxSlotAtIndex(pk, 0, 0);

        var useCase = new LegalityAuditUseCase();
        using var cts = new CancellationTokenSource();

        // Cancel as soon as the very first slot has been processed; a correctly-implemented
        // service must observe this before finishing the (much larger) remaining slot scan.
        var progress = new SyncProgress<LegalityAuditProgress>(p =>
        {
            if (p.Completed >= 1)
                cts.Cancel();
        });

        Assert.Throws<OperationCanceledException>(() => useCase.Execute(sav, progress, cts.Token));
    }

    [AvaloniaFact]
    public async Task ExportCsv_HasExpectedColumnsAndRowCount()
    {
        var sav = GetEmerald(out var skip);
        if (sav is null) { output.WriteLine($"Skip: {skip}"); return; }

        var vm = new LegalityAuditViewModel(sav, _dialogServiceMock.Object);
        await vm.RunCommand.ExecuteAsync(null);
        Assert.NotEmpty(vm.Rows);

        var path = Path.Combine(Path.GetTempPath(), $"legalityaudit-test-{Guid.NewGuid():N}.csv");
        _dialogServiceMock
            .Setup(d => d.SaveFileAsync(It.IsAny<string>(), It.IsAny<string?>(), It.IsAny<string[]?>()))
            .ReturnsAsync(path);

        try
        {
            await vm.ExportCsvCommand.ExecuteAsync(null);

            var lines = File.ReadAllLines(path);
            Assert.Equal("Location,Species,Nickname,Verdict,FailedChecks", lines[0]);
            Assert.Equal(vm.Rows.Count + 1, lines.Length);

            // Any exported field containing a raw comma must be quoted so the CSV parses correctly
            // (failed-check summaries frequently contain commas from the underlying report text).
            foreach (var row in vm.Rows)
            {
                if (!row.FailureSummary.Contains(','))
                    continue;
                Assert.Contains(lines, l => l.Contains($"\"{row.FailureSummary.Replace("\"", "\"\"")}\""));
            }
        }
        finally
        {
            File.Delete(path);
        }
    }

    [AvaloniaFact]
    public async Task ExportText_SingleFlaggedEntryMatchesLegalityAnalysisReport()
    {
        var sav = GetEmerald(out var skip);
        if (sav is null) { output.WriteLine($"Skip: {skip}"); return; }

        var vm = new LegalityAuditViewModel(sav, _dialogServiceMock.Object);
        await vm.RunCommand.ExecuteAsync(null);

        vm.VerdictFilter = "Illegal";
        if (vm.Rows.Count == 0) { output.WriteLine("Skip: gen3_emerald.sav has no illegal entries to export"); return; }

        var illegalPk = vm.Rows[0].Entity;
        var expectedReport = new LegalityAnalysis(illegalPk, sav.Personal).Report();

        var path = Path.Combine(Path.GetTempPath(), $"legalityaudit-text-{Guid.NewGuid():N}.txt");
        _dialogServiceMock
            .Setup(d => d.SaveFileAsync(It.IsAny<string>(), It.IsAny<string?>(), It.IsAny<string[]?>()))
            .ReturnsAsync(path);

        try
        {
            vm.SelectedRow = vm.Rows[0];
            await vm.ExportTextCommand.ExecuteAsync(null);

            var text = File.ReadAllText(path);
            Assert.Contains(expectedReport, text);
        }
        finally
        {
            File.Delete(path);
        }
    }

    [AvaloniaFact]
    public async Task VerdictFilter_ShowsOnlyMatchingRows()
    {
        var sav = GetEmerald(out var skip);
        if (sav is null) { output.WriteLine($"Skip: {skip}"); return; }

        var vm = new LegalityAuditViewModel(sav, _dialogServiceMock.Object);
        await vm.RunCommand.ExecuteAsync(null);
        var total = vm.Rows.Count;
        Assert.NotEmpty(vm.Rows);

        vm.VerdictFilter = "Illegal";
        Assert.All(vm.Rows, r => Assert.False(r.Valid));
        var illegalCount = vm.Rows.Count;

        vm.VerdictFilter = "Legal";
        Assert.All(vm.Rows, r => Assert.True(r.Valid));
        var legalCount = vm.Rows.Count;

        Assert.Equal(total, legalCount + illegalCount);

        vm.VerdictFilter = "All";
        Assert.Equal(total, vm.Rows.Count);
    }

    [AvaloniaFact]
    public async Task TextFilter_MatchesSpeciesOrNicknameCaseInsensitively()
    {
        var sav = GetEmerald(out var skip);
        if (sav is null) { output.WriteLine($"Skip: {skip}"); return; }

        var vm = new LegalityAuditViewModel(sav, _dialogServiceMock.Object);
        await vm.RunCommand.ExecuteAsync(null);
        Assert.NotEmpty(vm.Rows);

        var target = vm.Rows[0];
        vm.TextFilter = target.Species.ToUpperInvariant();

        Assert.Contains(vm.Rows, r => r.Species == target.Species);
        Assert.All(vm.Rows, r => Assert.Equal(target.Species, r.Species));
    }

    [AvaloniaFact]
    public async Task ActivateSelectedRow_RaisesRowActivatedWithBoxAndSlot()
    {
        var sav = GetEmerald(out var skip);
        if (sav is null) { output.WriteLine($"Skip: {skip}"); return; }

        var vm = new LegalityAuditViewModel(sav, _dialogServiceMock.Object);
        await vm.RunCommand.ExecuteAsync(null);
        Assert.NotEmpty(vm.Rows);

        var partyRow = vm.Rows.FirstOrDefault(r => r.IsParty);
        var boxRow = vm.Rows.FirstOrDefault(r => !r.IsParty);
        Assert.True(partyRow is not null || boxRow is not null);

        LegalityAuditRow? activated = null;
        vm.RowActivated += r => activated = r;

        vm.ActivateSelectedRowCommand.Execute(null); // no selection: no event
        Assert.Null(activated);

        var target = boxRow ?? partyRow!;
        vm.SelectedRow = target;
        vm.ActivateSelectedRowCommand.Execute(null);

        Assert.NotNull(activated);
        Assert.Same(target.Entity, activated!.Entity);
        Assert.Equal(target.IsParty, activated.IsParty);
        Assert.Equal(target.Box, activated.Box);
        Assert.Equal(target.Slot, activated.Slot);
    }
}
