using System.Linq;
using System.Reflection;
using PKHeX.Presentation.Localization;

namespace PKHeX.Presentation.ViewModels;

/// <summary>Outcome of a user-initiated ("Check for Updates") update check.</summary>
public enum UpdateCheckOutcome
{
    /// <summary>The running build is the newest available release.</summary>
    UpToDate,

    /// <summary>A newer release exists; the status-bar notification was (re)raised.</summary>
    UpdateAvailable,

    /// <summary>The check could not complete (offline, rate-limited, timed out, malformed response).</summary>
    Failed,
}

/// <summary>A user-facing result for a manual update check: an outcome plus a localized message.</summary>
public sealed record ManualUpdateCheckResult(UpdateCheckOutcome Outcome, string Message);

/// <summary>
/// Single owner of the update-check flow, shared by the silent startup check and the explicit
/// "Check for Updates" buttons (Settings / About). Centralizing it here means both paths resolve the
/// running version the same way and raise the same status-bar notification, and it lets the modal
/// Settings/About dialogs trigger a check without reaching into <see cref="MainWindowViewModel"/>.
/// </summary>
/// <remarks>
/// Registered as a singleton so the notification it raises (via <see cref="NotificationChanged"/>)
/// survives for the life of the main window regardless of which entry point ran the check.
/// </remarks>
public sealed class UpdateCheckCoordinator
{
    private readonly IUpdateCheckService _updateCheckService;
    private readonly IWindowService _windowService;
    private readonly IUpdateInstaller _updateInstaller;
    private readonly IAppLifetime _appLifetime;
    private readonly AppSettings _settings;
    private readonly ISettingsStore _settingsStore;
    private readonly string _currentVersion;

    /// <summary>
    /// Raised whenever the active status-bar notification changes (a newer release was found, or the
    /// notification was dismissed/skipped). <see cref="MainWindowViewModel"/> mirrors this into its
    /// bound <c>UpdateNotification</c> property. Always raised on the thread the check completes on;
    /// subscribers that touch UI state are responsible for marshalling if needed.
    /// </summary>
    public event Action<UpdateNotificationViewModel?>? NotificationChanged;

    private UpdateNotificationViewModel? _notification;
    public UpdateNotificationViewModel? Notification
    {
        get => _notification;
        private set
        {
            _notification = value;
            NotificationChanged?.Invoke(value);
        }
    }

    /// <summary>The running build's version (e.g. "1.38.1"), resolved once from the host assembly.</summary>
    public string CurrentVersion => _currentVersion;

    public UpdateCheckCoordinator(
        IUpdateCheckService updateCheckService,
        IWindowService windowService,
        IUpdateInstaller updateInstaller,
        IAppLifetime appLifetime,
        AppSettings settings,
        ISettingsStore settingsStore)
    {
        _updateCheckService = updateCheckService;
        _windowService = windowService;
        _updateInstaller = updateInstaller;
        _appLifetime = appLifetime;
        _settings = settings;
        _settingsStore = settingsStore;
        _currentVersion = ResolveCurrentVersion();
    }

    /// <summary>
    /// Silent startup check. Honors the <see cref="AppSettings.StartupSettings.CheckForUpdatesOnStartup"/>
    /// opt-out and stays completely quiet on failure (offline/rate-limited resolve to a null release
    /// list). Also runs the once-per-upgrade "What's New" changelog. Fire-and-forget from the host.
    /// </summary>
    public async Task RunStartupCheckAsync()
    {
        var startup = _settings.Startup;
        var previousVersion = startup.Version;
        var showChangelogOnUpgrade = startup.ShowChangelogOnUpdate
            && SemanticVersion.TryParse(previousVersion, out var previous)
            && SemanticVersion.TryParse(_currentVersion, out var current)
            && current > previous;

        // Record that this version has now been run, so the "just upgraded" changelog fires only once.
        if (!string.Equals(previousVersion, _currentVersion, StringComparison.Ordinal))
        {
            startup.Version = _currentVersion;
            _settingsStore.Save(_settings);
        }

        if (!startup.CheckForUpdatesOnStartup)
            return; // Settings toggle disabled: zero network calls.

        var releases = await _updateCheckService.GetReleasesAsync();
        if (releases is null || releases.Count == 0)
            return; // Offline/rate-limited/malformed — already logged by the service; stay silent.

        if (showChangelogOnUpgrade)
        {
            var upgradeNotes = UpdateAvailabilityEvaluator.GetReleasesNewerThan(releases, previousVersion);
            if (upgradeNotes.Count > 0)
            {
                var changelog = new UpdateChangelogViewModel(upgradeNotes, _updateInstaller, _windowService, _appLifetime);
                await _windowService.ShowDialogAsync(changelog, LocalizedStrings.Instance["Update_WhatsNew"]);
            }
        }

        var latest = UpdateAvailabilityEvaluator.GetLatestRelease(releases);
        if (latest is null)
            return;
        if (!UpdateAvailabilityEvaluator.ShouldNotify(_currentVersion, latest.TagName, startup.SkippedUpdateVersion))
            return;

        RaiseNotification(releases, latest);
    }

    /// <summary>
    /// Explicit user-initiated check. Unlike the startup path it ignores both the startup opt-out and
    /// the "skip this version" preference (the user is asking right now), and it never stays silent:
    /// every outcome maps to a message the caller shows. A found update also (re)raises the status-bar
    /// notification so the normal What's New / Download / Skip actions remain available.
    /// </summary>
    public async Task<ManualUpdateCheckResult> CheckNowAsync()
    {
        var releases = await _updateCheckService.GetReleasesAsync();
        if (releases is null || releases.Count == 0)
        {
            var reason = LocalizedStrings.Instance["Update_CheckFailedReason"];
            return new ManualUpdateCheckResult(
                UpdateCheckOutcome.Failed, LocalizedStrings.Instance.Format("Update_CheckFailed", reason));
        }

        var latest = UpdateAvailabilityEvaluator.GetLatestRelease(releases);
        // skippedVersion: null so a previously-skipped release is still reported on an explicit check.
        if (latest is null || !UpdateAvailabilityEvaluator.ShouldNotify(_currentVersion, latest.TagName, skippedVersion: null))
        {
            return new ManualUpdateCheckResult(
                UpdateCheckOutcome.UpToDate,
                LocalizedStrings.Instance.Format("Update_UpToDate", DisplayVersion(_currentVersion)));
        }

        RaiseNotification(releases, latest);
        return new ManualUpdateCheckResult(
            UpdateCheckOutcome.UpdateAvailable,
            LocalizedStrings.Instance.Format("Update_AvailableManual", DisplayVersion(latest.TagName)));
    }

    private void RaiseNotification(IReadOnlyList<ReleaseInfo> releases, ReleaseInfo latest)
    {
        var newerReleases = UpdateAvailabilityEvaluator.GetReleasesNewerThan(releases, _currentVersion);
        var notification = new UpdateNotificationViewModel(
            newerReleases.Count > 0 ? newerReleases : [latest], _windowService, _updateInstaller, _appLifetime, _settings, _settingsStore);
        notification.Dismissed += () => Notification = null;
        Notification = notification;
    }

    private static string DisplayVersion(string version) => version.TrimStart('v', 'V');

    private static string ResolveCurrentVersion()
    {
        // Looked up by assembly name so this Presentation-layer type never references the host
        // assembly directly (same trick used by AboutViewModel).
        var asm = AppDomain.CurrentDomain.GetAssemblies()
            .FirstOrDefault(a => string.Equals(a.GetName().Name, "PKHeX.Avalonia", StringComparison.Ordinal));
        var version = asm?.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion
                      ?? asm?.GetName().Version?.ToString(3)
                      ?? "0.0.0";
        return version.Contains('+') ? version[..version.IndexOf('+')] : version;
    }
}
