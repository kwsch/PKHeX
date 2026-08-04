using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Application.Abstractions;
using PKHeX.Application.Services;
using PKHeX.Presentation.Localization;

namespace PKHeX.Presentation.ViewModels;

/// <summary>
/// Backs the non-intrusive "update available" status-bar item. Created only when
/// <see cref="UpdateAvailabilityEvaluator.ShouldNotify"/> says a newer, non-skipped release exists;
/// null/absent otherwise, so the status bar shows nothing on a normal launch.
/// </summary>
public partial class UpdateNotificationViewModel : ViewModelBase
{
    private readonly IReadOnlyList<ReleaseInfo> _releasesNewestFirst;
    private readonly IWindowService _windowService;
    private readonly IUpdateInstaller _updateInstaller;
    private readonly IAppLifetime _appLifetime;
    private readonly AppSettings _settings;
    private readonly ISettingsStore _settingsStore;

    public string LatestVersion { get; }
    public string Message => LocalizedStrings.Instance.Format("UpdateNotification_UpdateAvailable", LatestVersion);

    /// <summary>Raised when the notification should be removed from the status bar (dismissed or skipped).</summary>
    public event Action? Dismissed;

    public UpdateNotificationViewModel(
        IReadOnlyList<ReleaseInfo> releasesNewestFirst, IWindowService windowService, IUpdateInstaller updateInstaller,
        IAppLifetime appLifetime, AppSettings settings, ISettingsStore settingsStore)
    {
        _releasesNewestFirst = releasesNewestFirst;
        _windowService = windowService;
        _updateInstaller = updateInstaller;
        _appLifetime = appLifetime;
        _settings = settings;
        _settingsStore = settingsStore;
        LatestVersion = releasesNewestFirst.Count > 0 ? releasesNewestFirst[0].TagName.TrimStart('v', 'V') : string.Empty;
    }

    [RelayCommand]
    private async Task ShowChangelog()
    {
        var changelog = new UpdateChangelogViewModel(_releasesNewestFirst, _updateInstaller, _windowService, _appLifetime);
        await _windowService.ShowDialogAsync(changelog, LocalizedStrings.Instance["UpdateNotification_WhatsNewTitle"]);
    }

    [RelayCommand]
    private async Task Download()
    {
        await UpdateDownloadLauncher.DownloadAsync(_releasesNewestFirst, _updateInstaller, _windowService, _appLifetime);
    }

    [RelayCommand]
    private void Skip()
    {
        _settings.Startup.SkippedUpdateVersion = LatestVersion;
        _settingsStore.Save(_settings);
        Dismissed?.Invoke();
    }

    [RelayCommand]
    private void Dismiss()
    {
        Dismissed?.Invoke();
    }
}
