using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Application.UseCases;
using PKHeX.Core;
using PKHeX.Presentation.Localization;

namespace PKHeX.Presentation.ViewModels;

/// <summary>
/// Legality Audit: scans every occupied party/box slot of the current save with the same
/// <see cref="LegalityAnalysis"/> code path the editor's legality panel uses, and shows the
/// results in a filterable, exportable, sortable spreadsheet in a modeless tool window.
/// </summary>
public partial class LegalityAuditViewModel : ViewModelBase
{
    private readonly SaveFile _sav;
    private readonly IDialogService _dialogService;
    private readonly LegalityAuditUseCase _useCase = new();

    private List<LegalityAuditRow> _allRows = [];
    private CancellationTokenSource? _cts;

    [ObservableProperty]
    private ObservableCollection<LegalityAuditRow> _rows = [];

    [ObservableProperty]
    private LegalityAuditRow? _selectedRow;

    [ObservableProperty]
    private string _statusText = LocalizedStrings.Instance["LegalityAudit_InitialStatus"];

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(RunCommand))]
    [NotifyCanExecuteChangedFor(nameof(CancelCommand))]
    private bool _isRunning;

    [ObservableProperty]
    private double _progress;

    /// <summary>Verdict filter applied to <see cref="Rows"/>: "All", "Legal", or "Illegal".</summary>
    [ObservableProperty]
    private string _verdictFilter = "All";

    /// <summary>Free-text filter matched against species, nickname, and failure summary.</summary>
    [ObservableProperty]
    private string _textFilter = string.Empty;

    public static IReadOnlyList<string> VerdictFilterOptions { get; } = ["All", "Legal", "Illegal"];

    /// <summary>Raised when the user activates (double-clicks) a row to navigate to it in the editor.</summary>
    public event Action<LegalityAuditRow>? RowActivated;

    public LegalityAuditViewModel(SaveFile sav, IDialogService dialogService)
    {
        _sav = sav;
        _dialogService = dialogService;
    }

    partial void OnVerdictFilterChanged(string value) => ApplyFilter();
    partial void OnTextFilterChanged(string value) => ApplyFilter();

    [RelayCommand(CanExecute = nameof(CanRun))]
    private async Task RunAsync()
    {
        IsRunning = true;
        Progress = 0;
        StatusText = LocalizedStrings.Instance["LegalityAudit_Auditing"];
        _cts = new CancellationTokenSource();

        try
        {
            var strings = GameInfo.Strings;
            var progress = new Progress<LegalityAuditProgress>(p =>
                Progress = p.Total == 0 ? 0 : 100.0 * p.Completed / p.Total);
            var token = _cts.Token;

            var entries = await Task.Run(() => _useCase.Execute(_sav, progress, token), token);

            _allRows = entries.Select(e => new LegalityAuditRow(e, strings)).ToList();
            ApplyFilter();

            var illegalCount = _allRows.Count(r => !r.Valid);
            StatusText = illegalCount == 0
                ? LocalizedStrings.Instance.Format("LegalityAudit_CompleteNoFindings", _allRows.Count)
                : LocalizedStrings.Instance.Format("LegalityAudit_CompleteWithFindings", _allRows.Count, illegalCount);
        }
        catch (OperationCanceledException)
        {
            StatusText = LocalizedStrings.Instance["LegalityAudit_Cancelled"];
        }
        catch (Exception ex)
        {
            StatusText = LocalizedStrings.Instance.Format("LegalityAudit_Failed", ex.Message);
            await _dialogService.ShowErrorAsync(LocalizedStrings.Instance["LegalityAudit_FailedTitle"], ex.Message);
        }
        finally
        {
            IsRunning = false;
            Progress = 0;
            _cts = null;
        }
    }

    private bool CanRun() => !IsRunning;

    [RelayCommand(CanExecute = nameof(CanCancel))]
    private void Cancel() => _cts?.Cancel();

    private bool CanCancel() => IsRunning;

    private void ApplyFilter()
    {
        IEnumerable<LegalityAuditRow> filtered = _allRows;

        if (VerdictFilter is "Legal")
            filtered = filtered.Where(r => r.Valid);
        else if (VerdictFilter is "Illegal")
            filtered = filtered.Where(r => !r.Valid);

        if (!string.IsNullOrWhiteSpace(TextFilter))
        {
            var text = TextFilter.Trim();
            filtered = filtered.Where(r =>
                r.Species.Contains(text, StringComparison.OrdinalIgnoreCase) ||
                r.Nickname.Contains(text, StringComparison.OrdinalIgnoreCase) ||
                r.FailureSummary.Contains(text, StringComparison.OrdinalIgnoreCase));
        }

        Rows = new ObservableCollection<LegalityAuditRow>(filtered);
    }

    [RelayCommand]
    private void ActivateSelectedRow()
    {
        if (SelectedRow is not null)
            RowActivated?.Invoke(SelectedRow);
    }

    [RelayCommand]
    private async Task ExportCsvAsync()
    {
        var path = await _dialogService.SaveFileAsync("Export Legality Audit", "LegalityAudit.csv");
        if (string.IsNullOrEmpty(path))
            return;

        try
        {
            File.WriteAllText(path, BuildCsv(Rows), Encoding.UTF8);
            await _dialogService.ShowInformationAsync(LocalizedStrings.Instance["LegalityAudit_ExportCompleteTitle"], LocalizedStrings.Instance.Format("LegalityAudit_ExportedRows", Rows.Count, path));
        }
        catch (Exception ex)
        {
            await _dialogService.ShowErrorAsync(LocalizedStrings.Instance["LegalityAudit_ExportErrorTitle"], ex.Message);
        }
    }

    [RelayCommand]
    private async Task ExportTextAsync()
    {
        var path = await _dialogService.SaveFileAsync(LocalizedStrings.Instance["LegalityAudit_ExportReportsTitle"], "LegalityAudit.txt");
        if (string.IsNullOrEmpty(path))
            return;

        try
        {
            File.WriteAllText(path, BuildTextReport(Rows), Encoding.UTF8);
            await _dialogService.ShowInformationAsync(LocalizedStrings.Instance["LegalityAudit_ExportCompleteTitle"], LocalizedStrings.Instance.Format("LegalityAudit_ExportedReports", Rows.Count, path));
        }
        catch (Exception ex)
        {
            await _dialogService.ShowErrorAsync(LocalizedStrings.Instance["LegalityAudit_ExportErrorTitle"], ex.Message);
        }
    }

    [RelayCommand]
    private async Task CopySelectedReportAsync()
    {
        if (SelectedRow is null)
            return;

        await _dialogService.SetClipboardTextAsync(SelectedRow.ReportText);
    }

    private static readonly (string Header, Func<LegalityAuditRow, object?> Get)[] CsvColumns =
    [
        ("Location", r => r.Position),
        ("Species", r => r.Species),
        ("Nickname", r => r.Nickname),
        ("Verdict", r => r.Verdict),
        ("FailedChecks", r => r.FailureSummary),
    ];

    private static string BuildCsv(IEnumerable<LegalityAuditRow> rows)
    {
        var sb = new StringBuilder();
        sb.AppendLine(string.Join(',', Array.ConvertAll(CsvColumns, c => c.Header)));
        foreach (var row in rows)
        {
            for (int i = 0; i < CsvColumns.Length; i++)
            {
                if (i > 0)
                    sb.Append(',');
                sb.Append(EscapeCsv(CsvColumns[i].Get(row)?.ToString() ?? string.Empty));
            }
            sb.AppendLine();
        }
        return sb.ToString();
    }

    private static string EscapeCsv(string value)
        => value.Contains(',') || value.Contains('"') || value.Contains('\n')
            ? $"\"{value.Replace("\"", "\"\"")}\""
            : value;

    /// <summary>
    /// Builds the standard PKHeX legality report text for each row, one after another,
    /// each report identical to what the editor's legality panel would show for that entity.
    /// </summary>
    private static string BuildTextReport(IEnumerable<LegalityAuditRow> rows)
    {
        var sb = new StringBuilder();
        foreach (var row in rows)
        {
            if (sb.Length > 0)
                sb.AppendLine().AppendLine();
            sb.AppendLine($"{row.Position} - {row.Species} ({row.Nickname})");
            sb.Append(row.ReportText);
        }
        return sb.ToString();
    }
}
