using CommunityToolkit.Mvvm.Input;

namespace PKHeX.Presentation.ViewModels;

public partial class MainWindowViewModel
{
    // Cached so re-invoking the menu item focuses the existing tool window instead of opening a
    // duplicate (ShowTool keys off the ViewModel instance). Reset on save change.
    private LiveHeXViewModel? _liveHeX;

    [RelayCommand(CanExecute = nameof(HasSave))]
    private void OpenLiveHeX()
    {
        if (CurrentSave is null)
            return;

        _liveHeX ??= new LiveHeXViewModel(
            CurrentSave,
            _liveHexService,
            _dialogService,
            getCurrentBox: () => BoxViewer?.CurrentBox ?? 0,
            onBoxUpdated: () => BoxViewer?.RefreshCurrentBox());

        _windowService.ShowTool(_liveHeX, "LiveHeX");
    }

    // Called on save change: drop the cached tool VM and close any live console session.
    private void DisposeLiveHeX()
    {
        if (_liveHeX is null)
            return;
        // Fire-and-forget: observe a faulted disconnect so it never surfaces as an
        // UnobservedTaskException — a failed disconnect here is non-fatal (best effort).
        _ = _liveHexService.DisconnectAsync().ContinueWith(
            t => System.Diagnostics.Trace.TraceWarning($"LiveHeX disconnect failed: {t.Exception?.GetBaseException().Message}"),
            TaskContinuationOptions.OnlyOnFaulted);
        _liveHeX = null;
    }
}
