using System.Threading;
using Moq;
using PKHeX.Presentation.Localization;
using PKHeX.Presentation.ViewModels;
using Xunit;

namespace PKHeX.Avalonia.Tests;

public class SemanticVersionTests
{
    [Theory]
    [InlineData("1.2.3")]
    [InlineData("v1.2.3")]
    [InlineData("V1.2.3")]
    [InlineData("1.2.3-beta.1")]
    [InlineData("1.2.3+build.5")]
    [InlineData("1.2.3-beta.1+build.5")]
    public void TryParse_accepts_valid_versions(string input)
    {
        Assert.True(SemanticVersion.TryParse(input, out _));
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("not-a-version")]
    [InlineData("1.2")]
    [InlineData("1")]
    public void TryParse_rejects_invalid_versions(string? input)
    {
        Assert.False(SemanticVersion.TryParse(input, out _));
    }

    [Theory]
    [InlineData("1.0.0", "2.0.0")]     // major
    [InlineData("1.1.0", "1.2.0")]     // minor
    [InlineData("1.1.1", "1.1.2")]     // patch
    [InlineData("1.0.0-alpha", "1.0.0")] // pre-release < stable
    [InlineData("1.0.0-alpha", "1.0.0-alpha.1")] // fewer identifiers < more, when equal prefix
    [InlineData("1.0.0-alpha.1", "1.0.0-alpha.beta")] // numeric identifier < alphanumeric
    [InlineData("1.0.0-alpha.beta", "1.0.0-beta")]
    [InlineData("1.0.0-beta", "1.0.0-beta.2")]
    [InlineData("1.0.0-beta.2", "1.0.0-beta.11")] // numeric comparison, not lexical ("11" > "2")
    [InlineData("1.0.0-beta.11", "1.0.0-rc.1")]
    [InlineData("1.0.0-rc.1", "1.0.0")]
    public void CompareTo_orders_versions_including_prerelease_precedence(string lesser, string greater)
    {
        var left = SemanticVersion.Parse(lesser);
        var right = SemanticVersion.Parse(greater);

        Assert.True(left < right, $"'{lesser}' should sort before '{greater}'");
        Assert.True(right > left);
        Assert.False(left == right);
    }

    [Fact]
    public void CompareTo_treats_equal_versions_as_equal_regardless_of_v_prefix()
    {
        var left = SemanticVersion.Parse("v1.25.3");
        var right = SemanticVersion.Parse("1.25.3");

        Assert.True(left == right);
        Assert.Equal(0, left.CompareTo(right));
    }

    [Fact]
    public void ToString_round_trips_stable_and_prerelease_versions()
    {
        Assert.Equal("1.2.3", SemanticVersion.Parse("1.2.3").ToString());
        Assert.Equal("1.2.3-beta.1", SemanticVersion.Parse("1.2.3-beta.1").ToString());
    }
}

public class UpdateAvailabilityEvaluatorTests
{
    private static ReleaseInfo Release(string tag, bool prerelease = false) =>
        new(tag, $"Release {tag}", $"Notes for {tag}", $"https://example.com/{tag}", prerelease, []);

    [Fact]
    public void GetLatestRelease_picks_highest_stable_version_and_skips_prereleases()
    {
        var releases = new[]
        {
            Release("1.0.0"),
            Release("1.2.0"),
            Release("2.0.0-beta.1", prerelease: true),
            Release("1.1.0"),
        };

        var latest = UpdateAvailabilityEvaluator.GetLatestRelease(releases);

        Assert.NotNull(latest);
        Assert.Equal("1.2.0", latest!.TagName);
    }

    [Fact]
    public void GetLatestRelease_ignores_unparseable_tags()
    {
        var releases = new[] { Release("not-a-version"), Release("1.0.0") };

        var latest = UpdateAvailabilityEvaluator.GetLatestRelease(releases);

        Assert.Equal("1.0.0", latest!.TagName);
    }

    [Fact]
    public void GetLatestRelease_returns_null_when_no_stable_release_parses()
    {
        var releases = new[] { Release("2.0.0-beta.1", prerelease: true), Release("garbage") };

        Assert.Null(UpdateAvailabilityEvaluator.GetLatestRelease(releases));
    }

    [Theory]
    [InlineData("1.25.2", "1.25.3", null, true)]   // newer, not skipped -> notify
    [InlineData("1.25.3", "1.25.3", null, false)]  // same version -> no notify
    [InlineData("1.25.3", "1.25.2", null, false)]  // current is newer -> no notify
    [InlineData("1.25.2", "1.25.3", "1.25.3", false)] // newer, but exactly the skipped version -> no notify
    [InlineData("1.25.2", "1.25.3", "1.25.2", true)]  // an older skip entry does not suppress a newer release
    public void ShouldNotify_respects_version_ordering_and_skip_list(
        string current, string latestTag, string? skipped, bool expected)
    {
        Assert.Equal(expected, UpdateAvailabilityEvaluator.ShouldNotify(current, latestTag, skipped));
    }

    [Fact]
    public void ShouldNotify_treats_a_later_release_as_not_skipped_by_an_older_skip_entry()
    {
        // User skipped 1.26.0; a subsequent 1.27.0 release must still notify.
        Assert.True(UpdateAvailabilityEvaluator.ShouldNotify("1.25.0", "1.27.0", "1.26.0"));
    }

    [Fact]
    public void GetReleasesNewerThan_returns_only_newer_releases_ordered_newest_first()
    {
        var releases = new[] { Release("1.0.0"), Release("1.2.0"), Release("1.1.0"), Release("0.9.0") };

        var newer = UpdateAvailabilityEvaluator.GetReleasesNewerThan(releases, "1.0.0");

        Assert.Equal(["1.2.0", "1.1.0"], newer.Select(r => r.TagName));
    }

    [Fact]
    public void GetReleasesNewerThan_returns_empty_when_current_version_is_unparseable()
    {
        var releases = new[] { Release("1.0.0") };

        Assert.Empty(UpdateAvailabilityEvaluator.GetReleasesNewerThan(releases, "not-a-version"));
    }

    // Regression: pins the real-world GitHub tag format (leading "v") against the app's own version
    // string (no "v"). The field bug was NOT here — this guards the chain that was wrongly suspected.
    [Fact]
    public void ShouldNotify_handles_real_github_tag_format_with_leading_v()
    {
        // App reports "1.37.1" (from AssemblyInformationalVersion); GitHub tag is "v1.38.0".
        Assert.True(UpdateAvailabilityEvaluator.ShouldNotify("1.37.1", "v1.38.0", skippedVersion: null));
        Assert.False(UpdateAvailabilityEvaluator.ShouldNotify("1.38.0", "v1.38.0", skippedVersion: null));
        Assert.False(UpdateAvailabilityEvaluator.ShouldNotify("1.38.1", "v1.38.0", skippedVersion: null));
    }

    [Fact]
    public void GetLatestRelease_picks_highest_across_real_v_prefixed_tags()
    {
        var releases = new[]
        {
            Release("v1.37.0"), Release("v1.38.1"), Release("v1.38.0"), Release("v1.37.1"),
        };

        var latest = UpdateAvailabilityEvaluator.GetLatestRelease(releases);

        Assert.Equal("v1.38.1", latest!.TagName);
    }
}

public class MainWindowUpdateCheckTests
{
    private static ReleaseInfo Release(string tag, bool prerelease = false) =>
        new(tag, $"Release {tag}", $"Notes for {tag}", $"https://example.com/{tag}", prerelease, []);

    private static (MainWindowViewModel Vm, UpdateCheckCoordinator Coordinator) Create(
        Mock<IUpdateCheckService> updateCheckServiceMock,
        Mock<IWindowService> windowServiceMock,
        AppSettings settings)
    {
        var coordinator = new UpdateCheckCoordinator(
            updateCheckServiceMock.Object, windowServiceMock.Object,
            new Mock<IUpdateInstaller>().Object, new Mock<IAppLifetime>().Object,
            settings, new FakeSettingsStore());

        var vm = new MainWindowViewModel(
            new Mock<ISaveFileGateway>().Object,
            new Mock<IDialogService>().Object,
            windowServiceMock.Object,
            new Mock<ISpriteRenderer>().Object,
            new Mock<ISlotService>().Object,
            new Mock<IClipboardService>().Object,
            new Mock<IQrCodeService>().Object,
            coordinator,
            new Mock<ISaveBackupService>().Object,
            settings,
            new FakeSettingsStore(),
            new Mock<IThemeService>().Object,
            new UndoRedoService(),
            new LanguageService(),
            new Mock<IAutoLegalityService>().Object,
            new Mock<PKHeX.Application.Abstractions.LiveHex.ILiveHexService>().Object,
            new Mock<ILivingDexService>().Object,
            new Mock<PKHeX.Application.Abstractions.GiftRecords.IGiftRecordProvider>().Object);
        return (vm, coordinator);
    }

    [Fact]
    public async Task CheckForUpdatesAsync_does_not_call_the_service_when_disabled_in_settings()
    {
        var updateCheckServiceMock = new Mock<IUpdateCheckService>();
        var windowServiceMock = new Mock<IWindowService>();
        var settings = new AppSettings();
        settings.Startup.CheckForUpdatesOnStartup = false;

        var (vm, _) = Create(updateCheckServiceMock, windowServiceMock, settings);
        await vm.CheckForUpdatesAsync();

        updateCheckServiceMock.Verify(
            s => s.GetReleasesAsync(It.IsAny<CancellationToken>()), Times.Never);
        Assert.Null(vm.UpdateNotification);
    }

    // Pins the field-reported root cause: when the startup toggle is off, a newer release exists but
    // no notification is raised — the whole check short-circuits before the service call.
    [Fact]
    public async Task CheckForUpdatesAsync_does_not_notify_when_disabled_even_though_a_newer_release_exists()
    {
        var updateCheckServiceMock = new Mock<IUpdateCheckService>();
        updateCheckServiceMock
            .Setup(s => s.GetReleasesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync((IReadOnlyList<ReleaseInfo>?)[Release("99.0.0")]);
        var windowServiceMock = new Mock<IWindowService>();
        var settings = new AppSettings();
        settings.Startup.CheckForUpdatesOnStartup = false;

        var (vm, _) = Create(updateCheckServiceMock, windowServiceMock, settings);
        await vm.CheckForUpdatesAsync();

        Assert.Null(vm.UpdateNotification);
    }

    [Fact]
    public async Task CheckForUpdatesAsync_stays_silent_when_the_service_fails()
    {
        var updateCheckServiceMock = new Mock<IUpdateCheckService>();
        updateCheckServiceMock
            .Setup(s => s.GetReleasesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync((IReadOnlyList<ReleaseInfo>?)null);
        var windowServiceMock = new Mock<IWindowService>();
        var settings = new AppSettings();

        var (vm, _) = Create(updateCheckServiceMock, windowServiceMock, settings);
        await vm.CheckForUpdatesAsync();

        Assert.Null(vm.UpdateNotification);
        windowServiceMock.Verify(
            w => w.ShowDialogAsync(It.IsAny<object>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task CheckForUpdatesAsync_surfaces_a_notification_when_a_newer_release_exists()
    {
        var updateCheckServiceMock = new Mock<IUpdateCheckService>();
        updateCheckServiceMock
            .Setup(s => s.GetReleasesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync((IReadOnlyList<ReleaseInfo>?)[Release("99.0.0")]);
        var windowServiceMock = new Mock<IWindowService>();
        var settings = new AppSettings();

        var (vm, _) = Create(updateCheckServiceMock, windowServiceMock, settings);
        await vm.CheckForUpdatesAsync();

        Assert.NotNull(vm.UpdateNotification);
        Assert.Equal("99.0.0", vm.UpdateNotification!.LatestVersion);
    }

    [Fact]
    public async Task CheckForUpdatesAsync_does_not_notify_for_a_skipped_version()
    {
        var updateCheckServiceMock = new Mock<IUpdateCheckService>();
        updateCheckServiceMock
            .Setup(s => s.GetReleasesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync((IReadOnlyList<ReleaseInfo>?)[Release("99.0.0")]);
        var windowServiceMock = new Mock<IWindowService>();
        var settings = new AppSettings();
        settings.Startup.SkippedUpdateVersion = "99.0.0";

        var (vm, _) = Create(updateCheckServiceMock, windowServiceMock, settings);
        await vm.CheckForUpdatesAsync();

        Assert.Null(vm.UpdateNotification);
    }

    [Fact]
    public async Task Skip_command_persists_the_skipped_version_and_dismisses_the_notification()
    {
        var updateCheckServiceMock = new Mock<IUpdateCheckService>();
        updateCheckServiceMock
            .Setup(s => s.GetReleasesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync((IReadOnlyList<ReleaseInfo>?)[Release("99.0.0")]);
        var windowServiceMock = new Mock<IWindowService>();
        var settings = new AppSettings();

        var (vm, _) = Create(updateCheckServiceMock, windowServiceMock, settings);
        await vm.CheckForUpdatesAsync();
        Assert.NotNull(vm.UpdateNotification);

        vm.UpdateNotification!.SkipCommand.Execute(null);

        Assert.Equal("99.0.0", settings.Startup.SkippedUpdateVersion);
        Assert.Null(vm.UpdateNotification);
    }

    // A manual check found an update: the same status-bar notification must appear on the main window.
    [Fact]
    public async Task Manual_check_surfaces_the_status_bar_notification_on_the_main_window()
    {
        var updateCheckServiceMock = new Mock<IUpdateCheckService>();
        updateCheckServiceMock
            .Setup(s => s.GetReleasesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync((IReadOnlyList<ReleaseInfo>?)[Release("99.0.0")]);
        var windowServiceMock = new Mock<IWindowService>();
        var settings = new AppSettings();
        settings.Startup.CheckForUpdatesOnStartup = false; // even with startup disabled, manual works

        var (vm, coordinator) = Create(updateCheckServiceMock, windowServiceMock, settings);
        await coordinator.CheckNowAsync();

        Assert.NotNull(vm.UpdateNotification);
        Assert.Equal("99.0.0", vm.UpdateNotification!.LatestVersion);
    }
}

public class UpdateCheckCoordinatorTests
{
    private static ReleaseInfo Release(string tag, bool prerelease = false) =>
        new(tag, $"Release {tag}", $"Notes for {tag}", $"https://example.com/{tag}", prerelease, []);

    private static UpdateCheckCoordinator Create(
        Mock<IUpdateCheckService> svc, AppSettings settings, Mock<IWindowService>? win = null) =>
        new(svc.Object, (win ?? new Mock<IWindowService>()).Object,
            new Mock<IUpdateInstaller>().Object, new Mock<IAppLifetime>().Object,
            settings, new FakeSettingsStore());

    [Fact]
    public async Task CheckNowAsync_reports_update_available_and_raises_the_notification()
    {
        var svc = new Mock<IUpdateCheckService>();
        svc.Setup(s => s.GetReleasesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync((IReadOnlyList<ReleaseInfo>?)[Release("v99.0.0")]);
        var coordinator = Create(svc, new AppSettings());

        var result = await coordinator.CheckNowAsync();

        Assert.Equal(UpdateCheckOutcome.UpdateAvailable, result.Outcome);
        Assert.Equal(LocalizedStrings.Instance.Format("Update_AvailableManual", "99.0.0"), result.Message);
        Assert.NotNull(coordinator.Notification);
    }

    [Fact]
    public async Task CheckNowAsync_reports_up_to_date_when_no_newer_release_exists()
    {
        var svc = new Mock<IUpdateCheckService>();
        // "0.0.0" == the version resolved in the test host (PKHeX.Avalonia assembly not loaded).
        svc.Setup(s => s.GetReleasesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync((IReadOnlyList<ReleaseInfo>?)[Release("0.0.0")]);
        var coordinator = Create(svc, new AppSettings());

        var result = await coordinator.CheckNowAsync();

        Assert.Equal(UpdateCheckOutcome.UpToDate, result.Outcome);
        Assert.Null(coordinator.Notification);
    }

    [Fact]
    public async Task CheckNowAsync_reports_failure_with_a_reason_when_the_service_fails()
    {
        var svc = new Mock<IUpdateCheckService>();
        svc.Setup(s => s.GetReleasesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync((IReadOnlyList<ReleaseInfo>?)null);
        var coordinator = Create(svc, new AppSettings());

        var result = await coordinator.CheckNowAsync();

        Assert.Equal(UpdateCheckOutcome.Failed, result.Outcome);
        Assert.Equal(
            LocalizedStrings.Instance.Format("Update_CheckFailed", LocalizedStrings.Instance["Update_CheckFailedReason"]),
            result.Message);
        Assert.Null(coordinator.Notification);
    }

    // A manual check is explicit user intent: a previously-skipped version must still be reported.
    [Fact]
    public async Task CheckNowAsync_ignores_the_skipped_version()
    {
        var svc = new Mock<IUpdateCheckService>();
        svc.Setup(s => s.GetReleasesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync((IReadOnlyList<ReleaseInfo>?)[Release("v99.0.0")]);
        var settings = new AppSettings();
        settings.Startup.SkippedUpdateVersion = "99.0.0";
        var coordinator = Create(svc, settings);

        var result = await coordinator.CheckNowAsync();

        Assert.Equal(UpdateCheckOutcome.UpdateAvailable, result.Outcome);
    }
}

public class AssetSelectorTests
{
    private static ReleaseAsset Asset(string name) => new(name, $"https://example.com/{name}");

    [Theory]
    [InlineData("windows", "x64", "PKHeX-Avalonia-win-x64.zip")]
    [InlineData("macos", "arm64", "PKHeX-Avalonia-osx-arm64.zip")]
    [InlineData("macos", "x64", "PKHeX-Avalonia-osx-x64.zip")]
    [InlineData("linux", "x64", "PKHeX-Avalonia-linux-x64.zip")]
    public void SelectAsset_matches_os_and_architecture(string os, string arch, string expected)
    {
        var assets = new[]
        {
            Asset("PKHeX-Avalonia-win-x64.zip"),
            Asset("PKHeX-Avalonia-osx-arm64.zip"),
            Asset("PKHeX-Avalonia-osx-x64.zip"),
            Asset("PKHeX-Avalonia-linux-x64.zip"),
        };

        var selected = AssetSelector.SelectAsset(assets, os, arch);

        Assert.NotNull(selected);
        Assert.Equal(expected, selected!.Name);
    }

    [Fact]
    public void SelectAsset_accepts_common_aliases_for_os_and_arch()
    {
        var assets = new[] { Asset("PKHeX-Avalonia-macos-aarch64.zip") };

        // "macos"/"aarch64" are aliases for the canonical "osx"/"arm64" tokens used elsewhere.
        var selected = AssetSelector.SelectAsset(assets, "macos", "aarch64");

        Assert.NotNull(selected);
        Assert.Equal("PKHeX-Avalonia-macos-aarch64.zip", selected!.Name);
    }

    [Fact]
    public void SelectAsset_falls_back_to_os_only_match_when_no_os_plus_arch_asset_exists()
    {
        // Single-architecture release: the asset name has no arch token at all.
        var assets = new[] { Asset("PKHeX-Avalonia-win.zip") };

        var selected = AssetSelector.SelectAsset(assets, "windows", "x64");

        Assert.NotNull(selected);
        Assert.Equal("PKHeX-Avalonia-win.zip", selected!.Name);
    }

    [Fact]
    public void SelectAsset_prefers_exact_os_and_arch_match_over_an_os_only_match()
    {
        var assets = new[] { Asset("PKHeX-Avalonia-win-x86.zip"), Asset("PKHeX-Avalonia-win-x64.zip") };

        var selected = AssetSelector.SelectAsset(assets, "windows", "x64");

        Assert.Equal("PKHeX-Avalonia-win-x64.zip", selected!.Name);
    }

    [Fact]
    public void SelectAsset_returns_null_when_no_asset_matches_the_os()
    {
        var assets = new[] { Asset("PKHeX-Avalonia-win-x64.zip") };

        Assert.Null(AssetSelector.SelectAsset(assets, "linux", "x64"));
    }

    [Fact]
    public void SelectAsset_returns_null_for_an_unrecognized_os()
    {
        var assets = new[] { Asset("PKHeX-Avalonia-win-x64.zip") };

        Assert.Null(AssetSelector.SelectAsset(assets, "amigaos", "m68k"));
    }

    [Fact]
    public void SelectAsset_returns_null_when_there_are_no_assets()
    {
        Assert.Null(AssetSelector.SelectAsset([], "windows", "x64"));
    }
}
