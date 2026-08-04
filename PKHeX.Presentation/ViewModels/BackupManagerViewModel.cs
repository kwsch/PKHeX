using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Application.Abstractions;
using PKHeX.Application.Services;
using PKHeX.Core;
using PKHeX.Presentation.Localization;

namespace PKHeX.Presentation.ViewModels;

/// <summary>
/// Backup Manager: lists the timestamped backups (issue #135) kept for the currently open save,
/// and lets the user restore, delete, or reveal one in the OS file manager. Shown as a modeless
/// tool window via <see cref="IWindowService.ShowTool"/>.
/// </summary>
public partial class BackupManagerViewModel : ViewModelBase
{
    private readonly ISaveBackupService _backupService;
    private readonly ISaveFileGateway _saveFileService;
    private readonly IDialogService _dialogService;
    private readonly AppSettings _settings;

    [ObservableProperty]
    private ObservableCollection<BackupEntryRow> _backups = [];

    [ObservableProperty]
    private BackupEntryRow? _selectedBackup;

    [ObservableProperty]
    private string _statusText = string.Empty;

    /// <summary>Raised after a successful restore so the host can refresh any other open editors/tools.</summary>
    public event Action? Restored;

    public BackupManagerViewModel(ISaveBackupService backupService, ISaveFileGateway saveFileService, IDialogService dialogService, AppSettings settings)
    {
        _backupService = backupService;
        _saveFileService = saveFileService;
        _dialogService = dialogService;
        _settings = settings;
        Refresh();
    }

    [RelayCommand]
    public void Refresh()
    {
        var path = _saveFileService.CurrentPath;
        if (string.IsNullOrEmpty(path))
        {
            Backups = [];
            StatusText = LocalizedStrings.Instance["BackupManager_SaveFirst"];
            return;
        }

        var identity = SaveIdentity.Compute(path);
        var result = _backupService.ListBackups(identity);

        Backups = new ObservableCollection<BackupEntryRow>(result.Entries.Select(e => new BackupEntryRow(e)));
        StatusText = result.Warnings.Count == 0
            ? LocalizedStrings.Instance.Format("BackupManager_BackupCount", Backups.Count)
            : LocalizedStrings.Instance.Format("BackupManager_BackupCountWithWarnings", Backups.Count, result.Warnings.Count, string.Join("; ", result.Warnings));
    }

    [RelayCommand]
    private async Task RestoreSelectedAsync()
    {
        if (SelectedBackup is null)
            return;

        var currentPath = _saveFileService.CurrentPath;
        if (string.IsNullOrEmpty(currentPath))
        {
            await _dialogService.ShowErrorAsync(LocalizedStrings.Instance["BackupManager_RestoreFailedTitle"], LocalizedStrings.Instance["BackupManager_NoSaveOpen"]);
            return;
        }

        var confirmed = await _dialogService.ShowConfirmationAsync(
            LocalizedStrings.Instance["BackupManager_RestoreBackupTitle"],
            LocalizedStrings.Instance.Format("BackupManager_RestoreConfirm", SelectedBackup.Timestamp));
        if (!confirmed)
            return;

        byte[] backupData;
        try
        {
            backupData = _backupService.ReadBackup(SelectedBackup.Entry);
        }
        catch (Exception ex)
        {
            await _dialogService.ShowErrorAsync(LocalizedStrings.Instance["BackupManager_RestoreFailedTitle"], LocalizedStrings.Instance.Format("BackupManager_ReadFailed", ex.Message));
            Refresh();
            return;
        }

        // Validate the backup actually parses as a save before touching anything on disk.
        // (Not just a non-throwing check — FileUtil also recognizes PKM/box-binary/etc. formats, so make
        // sure what comes back is specifically a SaveFile.)
        SaveFile? restoredSave;
        try
        {
            restoredSave = FileUtil.GetSupportedFile(backupData, Path.GetFileName(currentPath)) as SaveFile;
        }
        catch
        {
            restoredSave = null;
        }

        if (restoredSave is null)
        {
            await _dialogService.ShowErrorAsync(LocalizedStrings.Instance["BackupManager_RestoreFailedTitle"], LocalizedStrings.Instance["BackupManager_CorruptRestore"]);
            Refresh();
            return;
        }

        // Pre-restore backup of the current on-disk state, so restoring is itself undoable.
        if (File.Exists(currentPath))
        {
            try
            {
                var identity = SaveIdentity.Compute(currentPath);
                var existing = File.ReadAllBytes(currentPath);
                _backupService.CreateBackup(identity, existing, _settings.SaveBackup.MaxBackupsPerSave);
            }
            catch
            {
                // Non-fatal — proceed with the restore even if the pre-restore snapshot failed.
            }
        }

        try
        {
            File.WriteAllBytes(currentPath, backupData);
        }
        catch (Exception ex)
        {
            await _dialogService.ShowErrorAsync(LocalizedStrings.Instance["BackupManager_RestoreFailedTitle"], LocalizedStrings.Instance.Format("BackupManager_WriteFailed", ex.Message));
            Refresh();
            return;
        }

        var reloaded = await _saveFileService.LoadSaveFileAsync(currentPath);
        if (!reloaded)
        {
            await _dialogService.ShowErrorAsync(LocalizedStrings.Instance["BackupManager_RestoreFailedTitle"], LocalizedStrings.Instance["BackupManager_ReloadFailed"]);
            return;
        }

        Refresh();
        Restored?.Invoke();
        await _dialogService.ShowInformationAsync(LocalizedStrings.Instance["BackupManager_RestoreCompleteTitle"], LocalizedStrings.Instance["BackupManager_RestoreCompleteMessage"]);
    }

    [RelayCommand]
    private async Task DeleteSelectedAsync()
    {
        if (SelectedBackup is null)
            return;

        var confirmed = await _dialogService.ShowConfirmationAsync(
            LocalizedStrings.Instance["BackupManager_DeleteBackupTitle"],
            LocalizedStrings.Instance.Format("BackupManager_DeleteConfirm", SelectedBackup.Timestamp));
        if (!confirmed)
            return;

        _backupService.DeleteBackup(SelectedBackup.Entry);
        Refresh();
    }

    [RelayCommand]
    private void RevealSelected()
    {
        if (SelectedBackup is null)
            return;
        _dialogService.RevealInFileManager(SelectedBackup.FilePath);
    }
}
