using System;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Application.Abstractions;
using PKHeX.Presentation.Localization;

namespace PKHeX.Presentation.ViewModels;

/// <summary>
/// Progress dialog for the in-app self-update pipeline (download -&gt; verify -&gt; platform install -&gt;
/// relaunch). Shown instead of the browser-download fallback when <see cref="IUpdateInstaller.CanSelfUpdate"/>
/// succeeds and <see cref="Application.Services.UpdateAssetSelector"/> finds a matching asset for the
/// running install. On a successful install that requires a relaunch, the app is shut down via
/// <see cref="IAppLifetime"/> so the detached platform helper can swap files and relaunch.
/// </summary>
public partial class UpdateDownloadViewModel : ViewModelBase, ICloseableDialog
{
    private readonly IUpdateInstaller _installer;
    private readonly IAppLifetime _appLifetime;
    private readonly ReleaseAsset _asset;

    private CancellationTokenSource? _cts;

    public Action? CloseRequested { get; set; }

    /// <summary>The version being installed (release tag, "v" prefix stripped), e.g. "1.39.0".</summary>
    public string Version { get; }

    public string InstallPrompt => LocalizedStrings.Instance.Format("Update_InstallPrompt", Version);

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(InstallCommand))]
    [NotifyCanExecuteChangedFor(nameof(CancelCommand))]
    private bool _isRunning;

    /// <summary>0-100. Only meaningful while <see cref="IsIndeterminate"/> is <see langword="false"/> (download phase).</summary>
    [ObservableProperty] private double _progress;

    /// <summary>True for every phase after the download completes — the UI should show a spinner, not a percentage.</summary>
    [ObservableProperty] private bool _isIndeterminate;

    [ObservableProperty] private string _phaseText = string.Empty;

    [ObservableProperty] private string? _errorMessage;

    /// <summary>True once the install succeeded and the app is about to shut down for the relaunch helper.</summary>
    [ObservableProperty] private bool _isDone;

    public UpdateDownloadViewModel(ReleaseInfo release, ReleaseAsset asset, IUpdateInstaller installer, IAppLifetime appLifetime)
    {
        _asset = asset;
        _installer = installer;
        _appLifetime = appLifetime;
        Version = release.TagName.TrimStart('v', 'V');
    }

    private bool CanInstall => !IsRunning && !IsDone;

    [RelayCommand(CanExecute = nameof(CanInstall))]
    private async Task InstallAsync()
    {
        IsRunning = true;
        ErrorMessage = null;
        Progress = 0;
        IsIndeterminate = false;
        PhaseText = LocalizedStrings.Instance.Format("Update_Downloading", 0);
        _cts = new CancellationTokenSource();

        var progress = new Progress<UpdateProgress>(OnProgress);

        try
        {
            var result = await _installer.DownloadAndInstallAsync(_asset, progress, _cts.Token);
            if (!result.Success)
            {
                ErrorMessage = LocalizedStrings.Instance.Format(result.ErrorKey ?? "Update_Error_Generic", string.Empty);
                return;
            }

            if (result.WillRelaunch)
            {
                IsDone = true;
                _appLifetime.Shutdown();
            }
        }
        catch (OperationCanceledException)
        {
            // User cancelled: dialog stays open in its pre-download state, no error shown.
            PhaseText = string.Empty;
        }
        catch (Exception ex)
        {
            ErrorMessage = LocalizedStrings.Instance.Format("Update_Error_Generic", ex.Message);
        }
        finally
        {
            IsRunning = false;
            _cts = null;
        }
    }

    private void OnProgress(UpdateProgress p)
    {
        IsIndeterminate = p.Phase != UpdatePhase.Downloading;

        PhaseText = p.Phase switch
        {
            UpdatePhase.Downloading => LocalizedStrings.Instance.Format(
                "Update_Downloading", p.TotalBytes is > 0 ? (int)(100.0 * p.BytesReceived / p.TotalBytes.Value) : 0),
            UpdatePhase.Verifying => LocalizedStrings.Instance["Update_Verifying"],
            UpdatePhase.Extracting => LocalizedStrings.Instance["Update_Extracting"],
            UpdatePhase.Swapping => LocalizedStrings.Instance["Update_Installing"],
            UpdatePhase.Relaunching => LocalizedStrings.Instance["Update_Relaunching"],
            _ => PhaseText,
        };

        if (p.Phase == UpdatePhase.Downloading && p.TotalBytes is > 0)
            Progress = 100.0 * p.BytesReceived / p.TotalBytes.Value;
    }

    private bool CanCancel => IsRunning;

    [RelayCommand(CanExecute = nameof(CanCancel))]
    private void Cancel() => _cts?.Cancel();

    [RelayCommand]
    private void Close() => CloseRequested?.Invoke();
}
