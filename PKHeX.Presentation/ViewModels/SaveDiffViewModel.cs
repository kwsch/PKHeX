using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Application.Abstractions;
using PKHeX.Application.Services;
using PKHeX.Application.UseCases;
using PKHeX.Core;
using PKHeX.Presentation.Localization;

namespace PKHeX.Presentation.ViewModels;

/// <summary>
/// Save Comparison viewer (issue #135): diffs the currently open save against one of its backups,
/// or diffs two backups against each other. Shown as a modeless tool window.
/// </summary>
public partial class SaveDiffViewModel : ViewModelBase
{
    private readonly SaveFile _currentSave;
    private readonly string? _currentPath;
    private readonly ISaveBackupService _backupService;
    private readonly IDialogService _dialogService;

    [ObservableProperty]
    private ObservableCollection<BackupEntryRow> _availableBackups = [];

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(CompareCurrentWithBackupCommand))]
    [NotifyCanExecuteChangedFor(nameof(CompareTwoBackupsCommand))]
    private BackupEntryRow? _selectedBackupA;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(CompareTwoBackupsCommand))]
    private BackupEntryRow? _selectedBackupB;

    [ObservableProperty]
    private ObservableCollection<SaveDiffChangeRow> _changes = [];

    [ObservableProperty]
    private string _statusText = string.Empty;

    public SaveDiffViewModel(SaveFile currentSave, string? currentPath, ISaveBackupService backupService, IDialogService dialogService)
    {
        _currentSave = currentSave;
        _currentPath = currentPath;
        _backupService = backupService;
        _dialogService = dialogService;
        RefreshBackups();
    }

    [RelayCommand]
    public void RefreshBackups()
    {
        if (string.IsNullOrEmpty(_currentPath))
        {
            AvailableBackups = [];
            StatusText = LocalizedStrings.Instance["SaveDiff_SaveFileFirst"];
            return;
        }

        var identity = SaveIdentity.Compute(_currentPath);
        var result = _backupService.ListBackups(identity);
        AvailableBackups = new ObservableCollection<BackupEntryRow>(result.Entries.Select(e => new BackupEntryRow(e)));

        StatusText = result.Warnings.Count > 0
            ? LocalizedStrings.Instance.Format("SaveDiff_BackupsSkipped", result.Warnings.Count, string.Join("; ", result.Warnings))
            : LocalizedStrings.Instance.Format("SaveDiff_BackupsAvailable", AvailableBackups.Count);
    }

    [RelayCommand(CanExecute = nameof(CanCompareCurrentWithBackup))]
    private async Task CompareCurrentWithBackupAsync()
    {
        if (SelectedBackupA is null)
            return;

        if (!TryLoadBackup(SelectedBackupA, out var other, out var error))
        {
            await _dialogService.ShowErrorAsync(LocalizedStrings.Instance["SaveDiff_CompareFailedTitle"], error!);
            return;
        }

        RunDiff(_currentSave, other!, LocalizedStrings.Instance.Format("SaveDiff_CurrentVsBackupLabel", SelectedBackupA.Timestamp));
    }

    private bool CanCompareCurrentWithBackup => SelectedBackupA is not null;

    [RelayCommand(CanExecute = nameof(CanCompareTwoBackups))]
    private async Task CompareTwoBackupsAsync()
    {
        if (SelectedBackupA is null || SelectedBackupB is null)
            return;

        if (!TryLoadBackup(SelectedBackupA, out var left, out var errorA))
        {
            await _dialogService.ShowErrorAsync(LocalizedStrings.Instance["SaveDiff_CompareFailedTitle"], errorA!);
            return;
        }
        if (!TryLoadBackup(SelectedBackupB, out var right, out var errorB))
        {
            await _dialogService.ShowErrorAsync(LocalizedStrings.Instance["SaveDiff_CompareFailedTitle"], errorB!);
            return;
        }

        RunDiff(left!, right!, LocalizedStrings.Instance.Format("SaveDiff_BackupVsBackupLabel", SelectedBackupA.Timestamp, SelectedBackupB.Timestamp));
    }

    private bool CanCompareTwoBackups => SelectedBackupA is not null && SelectedBackupB is not null;

    private bool TryLoadBackup(BackupEntryRow row, out SaveFile? save, out string? error)
    {
        save = null;
        try
        {
            var data = _backupService.ReadBackup(row.Entry);
            save = FileUtil.GetSupportedFile(data, row.FileName) as SaveFile;
        }
        catch (Exception ex)
        {
            error = LocalizedStrings.Instance.Format("SaveDiff_BackupCorruptWithDetail", row.Timestamp, ex.Message);
            return false;
        }

        if (save is null)
        {
            error = LocalizedStrings.Instance.Format("SaveDiff_BackupCorrupt", row.Timestamp);
            return false;
        }

        error = null;
        return true;
    }

    private void RunDiff(SaveFile left, SaveFile right, string label)
    {
        var result = new SaveDiffUseCase().Execute(left, right);
        if (!result.Success)
        {
            Changes = [];
            StatusText = result.Error ?? LocalizedStrings.Instance["SaveDiff_SavesCannotBeCompared"];
            return;
        }

        Changes = new ObservableCollection<SaveDiffChangeRow>(result.Changes.Select(c => new SaveDiffChangeRow(c)));
        StatusText = Changes.Count == 0
            ? LocalizedStrings.Instance.Format("SaveDiff_NoDifferencesFound", label)
            : LocalizedStrings.Instance.Format("SaveDiff_DifferencesFound", Changes.Count, label);
    }
}
