using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;
using PKHeX.Presentation.Localization;

namespace PKHeX.Presentation.ViewModels;

public partial class MainWindowViewModel
{
    [RelayCommand]
    private async Task OpenFileAsync()
    {
        var path = await _dialogService.OpenFileAsync(
            T("File_OpenSaveFileTitle"),
            FileDialogFilters.OpenSaveFile);

        if (string.IsNullOrEmpty(path))
            return;

        await OpenSaveFilePathAsync(path);
    }

    private async Task OpenSaveFilePathAsync(string path)
    {
        var success = await _saveFileService.LoadSaveFileAsync(path);
        if (!success)
            await _dialogService.ShowErrorAsync(T("Common_Error"), LocalizedStrings.Instance.Format("File_CouldNotLoadSave", path));
    }

    // Fired by BoxViewer/PartyViewer when an OS file dropped onto a slot turns out to be a save
    // file rather than a Pokémon entity; routed through the same open-save path as File > Open.
    // Fire-and-forget from an event handler (can't be awaited): observe a faulted continuation so
    // it never surfaces as an UnobservedTaskException instead of the usual failed-load dialog.
    private void OnSaveFileDropRequested(string path) => _ = OpenSaveFilePathAsync(path).ContinueWith(
        t => System.Diagnostics.Trace.TraceWarning($"Failed to open dropped save file '{path}': {t.Exception?.GetBaseException().Message}"),
        TaskContinuationOptions.OnlyOnFaulted);

    /// <summary>
    /// Handles one or more OS files dropped anywhere on the main window. A save file is opened
    /// (same path as File &gt; Open); Pokémon entity files are loaded into the current editor
    /// without writing to a slot (drops onto a specific box/party slot are handled separately by
    /// <see cref="BoxViewerViewModel.HandleFileDropAsync"/>/<see cref="PartyViewerViewModel.HandleFileDropAsync"/>).
    /// </summary>
    public async Task HandleWindowFileDropAsync(IReadOnlyList<string> paths)
    {
        if (paths.Count == 0)
            return;

        // A dropped save file always wins, regardless of how many other files came with it.
        foreach (var path in paths)
        {
            var obj = TryGetSupportedFile(path);
            if (obj is SaveFile)
            {
                await OpenSaveFilePathAsync(path);
                return;
            }
        }

        if (CurrentSave is null || CurrentPokemonEditor is null)
            return;

        // Load the first recognizable entity into the editor; extras are ignored (the editor holds one Pokémon).
        foreach (var path in paths)
        {
            var result = new ImportEntityFileUseCase().Execute(CurrentSave, path);
            if (result.Kind == EntityFileDropKind.Entity)
            {
                CurrentPokemonEditor.LoadPKM(result.Entity!);
                return;
            }
        }

        await _dialogService.ShowErrorAsync(T("File_ImportFailedTitle"), T("File_NoSupportedFileFound"));
    }

    private object? TryGetSupportedFile(string path)
    {
        try { return FileUtil.GetSupportedFile(path, CurrentSave); }
        catch { return null; }
    }

    [RelayCommand(CanExecute = nameof(HasSave))]
    private async Task SaveFileAsync()
    {
        var success = await _saveFileService.SaveFileAsync();
        if (!success)
            await _dialogService.ShowErrorAsync(T("Common_Error"), T("File_FailedToSaveFile"));
    }

    [RelayCommand(CanExecute = nameof(HasSave))]
    private async Task SaveFileAsAsync()
    {
        var path = await _dialogService.SaveFileAsync(T("File_SaveAsTitle"), CurrentSave?.Metadata.FileName);
        if (string.IsNullOrEmpty(path))
            return;

        var success = await _saveFileService.SaveFileAsync(path);
        if (!success)
            await _dialogService.ShowErrorAsync(T("Common_Error"), T("File_FailedToSaveFile"));
    }

    [RelayCommand(CanExecute = nameof(HasSave))]
    private void CloseFile() => _saveFileService.CloseSave();

    [RelayCommand(CanExecute = nameof(HasSave))]
    private async Task ImportShowdownAsync()
    {
        if (CurrentSave is null || CurrentPokemonEditor is null) return;

        var text = await _clipboardService.GetTextAsync();
        var result = new ImportShowdownSetUseCase().Execute(CurrentSave, text);
        if (!result.Success)
        {
            await _dialogService.ShowErrorAsync(T("File_ImportFailedTitle"), result.Error!);
            return;
        }

        CurrentPokemonEditor.LoadPKM(result.Pokemon!);
    }

    [RelayCommand(CanExecute = nameof(HasSave))]
    private async Task ExportShowdownAsync()
    {
        if (CurrentPokemonEditor is null) return;

        var text = new ExportShowdownSetUseCase().Execute(CurrentPokemonEditor.PreparePKM());
        if (text is null) return;

        await _clipboardService.SetTextAsync(text);
    }

    [RelayCommand(CanExecute = nameof(HasSave))]
    private async Task ShowQrCodeAsync()
    {
        if (CurrentSave is null || CurrentPokemonEditor is null) return;

        var pk = CurrentPokemonEditor.PreparePKM();
        if (pk.Species == 0)
        {
            await _dialogService.ShowErrorAsync(T("Menu_Tools_QRCode"), T("File_LoadPokemonFirst"));
            return;
        }

        var message = QRMessageUtil.GetMessage(pk);
        var png = _qrCodeService.GeneratePng(message);
        var species = GameInfo.Strings.Species[pk.Species];
        var caption = pk.Format == 7
            ? LocalizedStrings.Instance.Format("File_QrCaptionGen7", species)
            : LocalizedStrings.Instance.Format("File_QrCaptionRaw", species);

        await _windowService.ShowDialogAsync(
            new QrCodeViewModel(png, caption, $"{species}_qr.png", _dialogService),
            T("Menu_Tools_QRCode"));
    }

    [RelayCommand(CanExecute = nameof(HasSave))]
    private async Task ImportQrCodeAsync()
    {
        if (CurrentSave is null || CurrentPokemonEditor is null) return;

        var path = await _dialogService.OpenFileAsync(T("File_OpenQrCodeImageTitle"), ["*.png", "*.jpg", "*.jpeg", "*.bmp"]);
        if (string.IsNullOrEmpty(path)) return;

        string? message;
        try
        {
            message = _qrCodeService.DecodePng(File.ReadAllBytes(path));
        }
        catch (Exception ex)
        {
            await _dialogService.ShowErrorAsync(T("File_QrImportFailedTitle"), LocalizedStrings.Instance.Format("File_CouldNotReadImage", ex.Message));
            return;
        }

        if (message is null)
        {
            await _dialogService.ShowErrorAsync(T("File_QrImportFailedTitle"), T("File_NoQrCodeFound"));
            return;
        }

        var pk = QRMessageUtil.GetPKM(message, CurrentSave.Context);
        if (pk is null)
        {
            await _dialogService.ShowErrorAsync(T("File_QrImportFailedTitle"),
                T("File_QrIncompatibleData"));
            return;
        }

        CurrentPokemonEditor.LoadPKM(pk);
    }

    [RelayCommand(CanExecute = nameof(HasSave))]
    private async Task ImportShowdownTeamAsync()
    {
        if (CurrentSave is null || BoxViewer is null) return;

        var text = await _clipboardService.GetTextAsync();
        var result = new ImportShowdownTeamUseCase().Execute(CurrentSave, text, BoxViewer.CurrentBox);
        if (!result.Success)
        {
            var detail = result.SetErrors.Count > 0
                ? $"{result.FatalError}\n\n{string.Join("\n", result.SetErrors)}"
                : result.FatalError!;
            await _dialogService.ShowErrorAsync(T("File_ImportTeamFailedTitle"), detail);
            return;
        }

        BoxViewer.RefreshCurrentBox();

        var message = LocalizedStrings.Instance.Format("File_ImportedPokemonIntoBox", result.Imported);
        if (result.SetErrors.Count > 0)
            message += "\n\n" + LocalizedStrings.Instance.Format("File_SkippedSets", string.Join("\n", result.SetErrors));
        await _dialogService.ShowInformationAsync(T("File_ImportTeamTitle"), message);
    }

    [RelayCommand(CanExecute = nameof(HasSave))]
    private async Task ExportShowdownBoxAsync()
    {
        if (CurrentSave is null || BoxViewer is null) return;

        var text = new ExportShowdownBoxUseCase().Execute(CurrentSave, BoxViewer.CurrentBox);
        if (text.Length == 0)
        {
            await _dialogService.ShowInformationAsync(T("File_ExportBoxTitle"), T("File_CurrentBoxEmpty"));
            return;
        }
        await _clipboardService.SetTextAsync(text);
    }

    [RelayCommand(CanExecute = nameof(HasSave))]
    private async Task ExportShowdownAllBoxesAsync()
    {
        if (CurrentSave is null) return;

        var text = new ExportShowdownBoxUseCase().ExecuteAll(CurrentSave);
        if (text.Length == 0)
        {
            await _dialogService.ShowInformationAsync(T("File_ExportAllBoxesTitle"), T("File_AllBoxesEmpty"));
            return;
        }
        await _clipboardService.SetTextAsync(text);
    }

    [RelayCommand(CanExecute = nameof(HasSave))]
    private async Task OpenPKMDatabaseAsync()
    {
        if (CurrentSave is null) return;

        var vm = new PKMDatabaseViewModel(CurrentSave, _spriteRenderer, _dialogService);
        vm.PokemonSelected += LoadEntity;

        await _windowService.ShowDialogAsync(vm, T("Menu_Data_PKMDatabase"));
    }

    // Cached so re-invoking the menu item focuses the existing tool window instead of
    // opening a duplicate (ShowTool keys off the ViewModel instance). Reset on save change.
    private BoxReportViewModel? _boxReport;

    [RelayCommand(CanExecute = nameof(HasSave))]
    private void OpenBoxReport()
    {
        if (CurrentSave is null) return;

        if (_boxReport is null)
        {
            _boxReport = new BoxReportViewModel(CurrentSave, _dialogService);
            _boxReport.RowActivated += row =>
            {
                ((IBoxNavigator?)BoxViewer)?.NavigateTo(row.Box, row.Slot);
                LoadEntity(row.Entity);
            };
        }
        else
        {
            _boxReport.Refresh();
        }

        _windowService.ShowTool(_boxReport, T("File_BoxDataReportTitle"));
    }

    // Cached so re-invoking the menu item focuses the existing tool window instead of
    // opening a duplicate (ShowTool keys off the ViewModel instance). Reset on save change.
    private LegalityAuditViewModel? _legalityAudit;

    [RelayCommand(CanExecute = nameof(HasSave))]
    private void OpenLegalityAudit()
    {
        if (CurrentSave is null) return;

        if (_legalityAudit is null)
        {
            _legalityAudit = new LegalityAuditViewModel(CurrentSave, _dialogService);
            _legalityAudit.RowActivated += row =>
            {
                if (row.IsParty)
                    PartyViewer?.RefreshParty();
                else
                    ((IBoxNavigator?)BoxViewer)?.NavigateTo(row.Box, row.Slot);
                LoadEntity(row.Entity);
            };
        }

        _windowService.ShowTool(_legalityAudit, T("File_LegalityAuditTitle"));
    }

    // Cached so re-invoking the menu item focuses the existing tool window instead of
    // opening a duplicate (ShowTool keys off the ViewModel instance). Reset on save change.
    private BackupManagerViewModel? _backupManager;

    [RelayCommand(CanExecute = nameof(HasSave))]
    private void OpenBackupManager()
    {
        if (CurrentSave is null) return;

        if (_backupManager is null)
        {
            // Restoring reloads the save via ISaveFileGateway, which already re-fires
            // SaveFileChanged (see the subscription in the constructor) — no separate hook needed here.
            _backupManager = new BackupManagerViewModel(_saveBackupService, _saveFileService, _dialogService, _settings);
        }
        else
        {
            _backupManager.Refresh();
        }

        _windowService.ShowTool(_backupManager, T("File_BackupManagerTitle"));
    }

    private SaveDiffViewModel? _saveDiff;

    [RelayCommand(CanExecute = nameof(HasSave))]
    private void OpenSaveDiff()
    {
        if (CurrentSave is null) return;

        if (_saveDiff is null)
            _saveDiff = new SaveDiffViewModel(CurrentSave, _saveFileService.CurrentPath, _saveBackupService, _dialogService);
        else
            _saveDiff.RefreshBackups();

        _windowService.ShowTool(_saveDiff, T("File_CompareSavesTitle"));
    }

    [RelayCommand(CanExecute = nameof(HasSave))]
    private async Task OpenMysteryGiftDatabaseAsync()
    {
        if (CurrentSave is null) return;

        var vm = new MysteryGiftDatabaseViewModel(CurrentSave, _spriteRenderer, _dialogService);
        vm.GiftSelected += LoadEntity;

        await _windowService.ShowDialogAsync(vm, T("Menu_Data_MysteryGiftDatabase"));
    }

    [RelayCommand(CanExecute = nameof(HasSave))]
    private async Task DumpBoxesAsync()
    {
        if (CurrentSave is null) return;

        var path = await _dialogService.OpenFolderAsync(T("File_SelectFolderDumpBoxes"));
        if (string.IsNullOrEmpty(path)) return;

        var result = new DumpBoxesUseCase().Execute(CurrentSave, path);
        if (!result.Success)
        {
            await _dialogService.ShowErrorAsync(T("Menu_Data_DumpBoxes"), result.Message);
            return;
        }

        await _dialogService.ShowInformationAsync(T("Menu_Data_DumpBoxes"), result.Message);
    }

    [RelayCommand(CanExecute = nameof(HasSave))]
    private async Task LoadBoxesAsync()
    {
        if (CurrentSave is null) return;

        var path = await _dialogService.OpenFolderAsync(T("File_SelectFolderLoadBoxes"));
        if (string.IsNullOrEmpty(path)) return;

        var result = new LoadBoxesUseCase().Execute(CurrentSave, path);

        BoxViewer?.RefreshCurrentBox();

        if (!result.Success)
            await _dialogService.ShowErrorAsync(T("Menu_Data_LoadBoxes"), result.Message);
        else
            await _dialogService.ShowInformationAsync(T("Menu_Data_LoadBoxes"), result.Message);
    }
}
