using System.Collections.Generic;
using System.Threading.Tasks;
using PKHeX.Application.Abstractions;
using PKHeX.Application.Services;
using PKHeX.Presentation.Localization;

namespace PKHeX.Presentation.ViewModels;

/// <summary>
/// Shared "Download" behavior for the changelog dialog and the status-bar notification: prefers an
/// in-app self-update (progress dialog -&gt; download -&gt; verify -&gt; install -&gt; relaunch) when the
/// running install supports it and a matching asset exists, falling back to opening the best-matching
/// asset (or the release page) in the browser otherwise — the same fallback behavior the app has always had.
/// </summary>
internal static class UpdateDownloadLauncher
{
    public static async Task DownloadAsync(
        IReadOnlyList<ReleaseInfo> releasesNewestFirst, IUpdateInstaller updateInstaller, IWindowService windowService, IAppLifetime appLifetime)
    {
        if (releasesNewestFirst.Count == 0)
            return;

        var latest = releasesNewestFirst[0];

        if (updateInstaller.CanSelfUpdate(out _))
        {
            var selfUpdateAsset = UpdateAssetSelector.SelectAsset(
                latest.Assets, CurrentPlatform.Os, CurrentPlatform.Arch, updateInstaller.CurrentInstallKind);
            if (selfUpdateAsset is not null)
            {
                var download = new UpdateDownloadViewModel(latest, selfUpdateAsset, updateInstaller, appLifetime);
                await windowService.ShowDialogAsync(download, LocalizedStrings.Instance["Update_InstallButton"]);
                return;
            }
        }

        var browserAsset = AssetSelector.SelectAsset(latest.Assets, CurrentPlatform.Os, CurrentPlatform.Arch);
        UpdateChangelogViewModel.OpenUrl(browserAsset?.BrowserDownloadUrl ?? latest.HtmlUrl);
    }
}
