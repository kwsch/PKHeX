using System.Collections.Generic;
using System.Diagnostics;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Application.Abstractions;
using PKHeX.Application.Services;

namespace PKHeX.Presentation.ViewModels;

/// <summary>
/// "What's new" dialog: renders the release notes for every version between the installed build
/// and the latest one, with a link to the release page for full details/assets.
/// </summary>
public partial class UpdateChangelogViewModel : ViewModelBase, ICloseableDialog
{
    private readonly IReadOnlyList<ReleaseInfo> _releasesNewestFirst;
    private readonly IUpdateInstaller _updateInstaller;
    private readonly IWindowService _windowService;
    private readonly IAppLifetime _appLifetime;

    public Action? CloseRequested { get; set; }

    public IReadOnlyList<ReleaseNoteViewModel> Releases { get; }

    public string LatestVersion { get; }
    public string LatestReleaseUrl { get; }

    public UpdateChangelogViewModel(
        IReadOnlyList<ReleaseInfo> releasesNewestFirst, IUpdateInstaller updateInstaller, IWindowService windowService, IAppLifetime appLifetime)
    {
        _releasesNewestFirst = releasesNewestFirst;
        _updateInstaller = updateInstaller;
        _windowService = windowService;
        _appLifetime = appLifetime;

        Releases = releasesNewestFirst
            .Select(r => new ReleaseNoteViewModel(r.TagName, r.Name, r.Body, r.HtmlUrl))
            .ToList();

        var latest = releasesNewestFirst.Count > 0 ? releasesNewestFirst[0] : null;
        LatestVersion = latest?.TagName ?? string.Empty;
        LatestReleaseUrl = latest?.HtmlUrl ?? string.Empty;
    }

    [RelayCommand]
    private void OpenReleasePage()
    {
        OpenUrl(LatestReleaseUrl);
    }

    [RelayCommand]
    private async Task Download()
    {
        await UpdateDownloadLauncher.DownloadAsync(_releasesNewestFirst, _updateInstaller, _windowService, _appLifetime);
    }

    [RelayCommand]
    private void Close()
    {
        CloseRequested?.Invoke();
    }

    internal static void OpenUrl(string url)
    {
        if (string.IsNullOrEmpty(url))
            return;

        try
        {
            if (OperatingSystem.IsWindows())
                Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
            else if (OperatingSystem.IsMacOS())
                Process.Start("open", url);
            else if (OperatingSystem.IsLinux())
                Process.Start("xdg-open", url);
        }
        catch (Exception ex)
        {
            Trace.TraceWarning($"Failed to open release/asset URL '{url}': {ex.Message}");
        }
    }
}

/// <summary>A single release's notes, formatted for display in the changelog dialog.</summary>
public sealed class ReleaseNoteViewModel(string tagName, string name, string body, string htmlUrl)
{
    public string TagName { get; } = tagName;
    public string Name { get; } = name;
    public string Body { get; } = body;
    public string HtmlUrl { get; } = htmlUrl;
}
