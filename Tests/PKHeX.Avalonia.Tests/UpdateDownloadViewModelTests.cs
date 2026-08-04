using System.Threading;
using System.Threading.Tasks;
using PKHeX.Presentation.Localization;
using PKHeX.Presentation.ViewModels;

namespace PKHeX.Avalonia.Tests;

public class UpdateDownloadViewModelTests
{
    private sealed class FakeInstaller : IUpdateInstaller
    {
        public InstallKind CurrentInstallKind => InstallKind.WindowsPortable;
        public Func<ReleaseAsset, IProgress<UpdateProgress>, CancellationToken, Task<UpdateInstallResult>>? OnDownload { get; set; }

        public bool CanSelfUpdate(out string? reason)
        {
            reason = null;
            return true;
        }

        public Task<UpdateInstallResult> DownloadAndInstallAsync(ReleaseAsset asset, IProgress<UpdateProgress> progress, CancellationToken ct) =>
            OnDownload!(asset, progress, ct);
    }

    private sealed class FakeAppLifetime : IAppLifetime
    {
        public bool ShutdownCalled { get; private set; }
        public void Shutdown() => ShutdownCalled = true;
    }

    /// <summary>
    /// Runs <see cref="SynchronizationContext.Post"/> synchronously. The ViewModel reports progress
    /// via <see cref="Progress{T}"/>, which — with no ambient context — marshals through the thread
    /// pool and gives no ordering/completion guarantee relative to the awaited install task. Tests
    /// that assert on the sequence of reported phases need this installed first so every
    /// <c>Report</c> call applies before <c>InstallCommand.ExecuteAsync</c> returns.
    /// </summary>
    private sealed class ImmediateSynchronizationContext : SynchronizationContext
    {
        public override void Post(SendOrPostCallback d, object? state) => d(state);
    }

    private static ReleaseInfo Release(string tag = "v1.40.0") => new(tag, "Release", "notes", "https://example.com", false, []);
    private static ReleaseAsset Asset() => new("asset.zip", "https://example.com/asset.zip");

    [Fact]
    public async Task Successful_install_with_relaunch_shuts_down_the_app()
    {
        var installer = new FakeInstaller
        {
            OnDownload = (_, progress, _) =>
            {
                progress.Report(new UpdateProgress(UpdatePhase.Downloading, 50, 100));
                return Task.FromResult(new UpdateInstallResult(true, true, null));
            },
        };
        var lifetime = new FakeAppLifetime();
        var vm = new UpdateDownloadViewModel(Release(), Asset(), installer, lifetime);

        await vm.InstallCommand.ExecuteAsync(null);

        Assert.True(vm.IsDone);
        Assert.True(lifetime.ShutdownCalled);
        Assert.Null(vm.ErrorMessage);
        Assert.False(vm.IsRunning);
    }

    [Fact]
    public async Task Phases_map_to_the_expected_localized_text()
    {
        var installer = new FakeInstaller
        {
            OnDownload = (_, progress, _) =>
            {
                progress.Report(new UpdateProgress(UpdatePhase.Verifying, 0, null));
                progress.Report(new UpdateProgress(UpdatePhase.Extracting, 0, null));
                progress.Report(new UpdateProgress(UpdatePhase.Swapping, 0, null));
                progress.Report(new UpdateProgress(UpdatePhase.Relaunching, 0, null));
                return Task.FromResult(new UpdateInstallResult(true, true, null));
            },
        };
        var vm = new UpdateDownloadViewModel(Release(), Asset(), installer, new FakeAppLifetime());
        var seenPhaseTexts = new List<string>();
        vm.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(vm.PhaseText))
                seenPhaseTexts.Add(vm.PhaseText);
        };

        var originalContext = SynchronizationContext.Current;
        SynchronizationContext.SetSynchronizationContext(new ImmediateSynchronizationContext());
        try
        {
            await vm.InstallCommand.ExecuteAsync(null);
        }
        finally
        {
            SynchronizationContext.SetSynchronizationContext(originalContext);
        }

        Assert.Contains(LocalizedStrings.Instance["Update_Verifying"], seenPhaseTexts);
        Assert.Contains(LocalizedStrings.Instance["Update_Extracting"], seenPhaseTexts);
        Assert.Contains(LocalizedStrings.Instance["Update_Installing"], seenPhaseTexts);
        Assert.Contains(LocalizedStrings.Instance["Update_Relaunching"], seenPhaseTexts);
    }

    [Fact]
    public async Task Failure_surfaces_the_localized_error_and_never_shuts_down()
    {
        var installer = new FakeInstaller
        {
            OnDownload = (_, _, _) => Task.FromResult(new UpdateInstallResult(false, false, "Update_Error_Checksum")),
        };
        var lifetime = new FakeAppLifetime();
        var vm = new UpdateDownloadViewModel(Release(), Asset(), installer, lifetime);

        await vm.InstallCommand.ExecuteAsync(null);

        Assert.False(vm.IsDone);
        Assert.False(lifetime.ShutdownCalled);
        Assert.Equal(LocalizedStrings.Instance["Update_Error_Checksum"], vm.ErrorMessage);
    }

    [Fact]
    public async Task Cancel_stops_the_install_without_surfacing_an_error()
    {
        var startedTcs = new TaskCompletionSource();
        var installer = new FakeInstaller
        {
            OnDownload = async (_, _, ct) =>
            {
                startedTcs.SetResult();
                await Task.Delay(Timeout.Infinite, ct);
                return new UpdateInstallResult(false, false, "unreachable");
            },
        };
        var vm = new UpdateDownloadViewModel(Release(), Asset(), installer, new FakeAppLifetime());

        var installTask = vm.InstallCommand.ExecuteAsync(null);
        await startedTcs.Task;
        vm.CancelCommand.Execute(null);
        await installTask;

        Assert.Null(vm.ErrorMessage);
        Assert.False(vm.IsRunning);
        Assert.False(vm.IsDone);
    }
}
