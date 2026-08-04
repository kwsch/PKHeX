using System.IO;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using PKHeX.Infrastructure.Updating;

namespace PKHeX.Avalonia.Tests;

public class GitHubUpdateInstallerTests
{
    private sealed class FakeStrategy : IPlatformUpdateStrategy
    {
        public bool Invoked { get; private set; }
        public string? DownloadedFilePath { get; private set; }
        public UpdateInstallResult Result { get; set; } = new(true, true, null);

        public Task<UpdateInstallResult> InstallAsync(
            string downloadedFilePath, ReleaseAsset asset, InstallLocationInfo location, IProgress<UpdateProgress> progress, CancellationToken ct)
        {
            Invoked = true;
            DownloadedFilePath = downloadedFilePath;
            return Task.FromResult(Result);
        }
    }

    /// <summary>Deterministic, synchronous <see cref="IProgress{T}"/> — avoids <see cref="Progress{T}"/>'s SynchronizationContext posting, which would race with assertions.</summary>
    private sealed class SyncProgress : IProgress<UpdateProgress>
    {
        public List<UpdateProgress> Reports { get; } = [];
        public void Report(UpdateProgress value) => Reports.Add(value);
    }

    /// <summary>Cancels the given <see cref="CancellationTokenSource"/> synchronously on the first report.</summary>
    private sealed class CancelingProgress(CancellationTokenSource cts) : IProgress<UpdateProgress>
    {
        public void Report(UpdateProgress value) => cts.Cancel();
    }

    private static GitHubUpdateInstaller CreateInstaller(
        HttpMessageHandler handler, FakeStrategy strategy, InstallKind kind = InstallKind.WindowsPortable, string root = "/install/dir") =>
        new(new HttpClient(handler),
            new Dictionary<InstallKind, IPlatformUpdateStrategy> { [kind] = strategy },
            () => new InstallLocationInfo(kind, root));

    [Fact]
    public async Task Download_reports_progress_with_correct_totals_then_verifies_then_invokes_strategy()
    {
        var payload = new byte[50_000];
        new Random(42).NextBytes(payload);
        var sha256 = Convert.ToHexStringLower(SHA256.HashData(payload));

        var handler = new FakeHttpMessageHandler(_ =>
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK) { Content = new ByteArrayContent(payload) };
            response.Content.Headers.ContentLength = payload.Length;
            return response;
        });
        var strategy = new FakeStrategy();
        var installer = CreateInstaller(handler, strategy);
        var asset = new ReleaseAsset("asset.zip", "https://example.com/asset.zip", sha256, payload.Length);
        var progress = new SyncProgress();

        var result = await installer.DownloadAndInstallAsync(asset, progress, CancellationToken.None);

        Assert.True(result.Success);
        Assert.True(strategy.Invoked);
        Assert.NotNull(strategy.DownloadedFilePath);

        // Progress must have reached the full byte count with the correct total, and Verifying must
        // have been reported (and thus completed, since InstallAsync ran) before the strategy ran.
        var lastDownload = progress.Reports.Last(p => p.Phase == UpdatePhase.Downloading);
        Assert.Equal(payload.Length, lastDownload.BytesReceived);
        Assert.Equal(payload.Length, lastDownload.TotalBytes);
        Assert.Contains(progress.Reports, p => p.Phase == UpdatePhase.Verifying);

        var verifyIndex = progress.Reports.FindIndex(p => p.Phase == UpdatePhase.Verifying);
        var lastDownloadIndex = progress.Reports.FindLastIndex(p => p.Phase == UpdatePhase.Downloading);
        Assert.True(verifyIndex > lastDownloadIndex, "Verifying must be reported after the download completes.");
    }

    [Fact]
    public async Task Checksum_mismatch_aborts_with_checksum_error_deletes_temp_and_never_invokes_strategy()
    {
        var payload = "hello world"u8.ToArray();
        var handler = new FakeHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK) { Content = new ByteArrayContent(payload) });
        var strategy = new FakeStrategy();
        var installer = CreateInstaller(handler, strategy);
        const string wrongHash = "0000000000000000000000000000000000000000000000000000000000000000";
        var asset = new ReleaseAsset("checksum-mismatch.zip", "https://example.com/checksum-mismatch.zip", wrongHash, payload.Length);
        var progress = new SyncProgress();

        var result = await installer.DownloadAndInstallAsync(asset, progress, CancellationToken.None);

        Assert.False(result.Success);
        Assert.Equal("Update_Error_Checksum", result.ErrorKey);
        Assert.False(strategy.Invoked);
        Assert.False(File.Exists(Path.Combine(Path.GetTempPath(), $"{asset.Name}.part")));
    }

    [Fact]
    public async Task Missing_checksum_skips_verification_and_still_installs()
    {
        var payload = "no digest published for this asset"u8.ToArray();
        var handler = new FakeHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK) { Content = new ByteArrayContent(payload) });
        var strategy = new FakeStrategy();
        var installer = CreateInstaller(handler, strategy);
        var asset = new ReleaseAsset("no-digest.zip", "https://example.com/no-digest.zip", Sha256: null, Size: payload.Length);
        var progress = new SyncProgress();

        var result = await installer.DownloadAndInstallAsync(asset, progress, CancellationToken.None);

        Assert.True(result.Success);
        Assert.True(strategy.Invoked);
    }

    [Fact]
    public async Task Cancellation_during_download_deletes_the_partial_temp_file()
    {
        var payload = new byte[300_000];
        new Random(7).NextBytes(payload);
        using var cts = new CancellationTokenSource();

        var handler = new FakeHttpMessageHandler(_ =>
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ByteArrayContent(payload) { Headers = { ContentLength = payload.Length } },
            };
            return response;
        });
        var strategy = new FakeStrategy();
        var installer = CreateInstaller(handler, strategy);
        var asset = new ReleaseAsset("cancelled.zip", "https://example.com/cancelled.zip", Sha256: null, Size: payload.Length);

        // Cancel synchronously as soon as the first progress report arrives (part-way through the
        // multi-chunk download, since the buffer is 80KB and the payload is ~300KB). Deliberately
        // not System.Progress<T>: it posts through a captured SynchronizationContext, which would
        // race with (and likely lose to) the fully-synchronous in-memory download loop below.
        var progress = new CancelingProgress(cts);

        // TaskCanceledException (a subclass of OperationCanceledException) is what the framework's
        // I/O APIs actually throw for a cancelled token, so match on the base type.
        await Assert.ThrowsAnyAsync<OperationCanceledException>(
            () => installer.DownloadAndInstallAsync(asset, progress, cts.Token));

        Assert.False(strategy.Invoked);
        Assert.False(File.Exists(Path.Combine(Path.GetTempPath(), $"{asset.Name}.part")));
    }

            [Fact]
            public async Task Windows_installer_receives_the_correct_extension_not_part()
            {
                // Regression: the .part extension has no shell association on Windows, so Process.Start
                // would show an "open with" dialog instead of launching the installer. The fix renames
                // the temp file to the sanitized asset name before handing it to the platform strategy.
                var payload = new byte[30_000];
                new Random(99).NextBytes(payload);
                var sha256 = Convert.ToHexStringLower(SHA256.HashData(payload));

                var handler = new FakeHttpMessageHandler(_ =>
                {
                    var response = new HttpResponseMessage(HttpStatusCode.OK) { Content = new ByteArrayContent(payload) };
                    response.Content.Headers.ContentLength = payload.Length;
                    return response;
                });
                var strategy = new FakeStrategy();
                var installer = CreateInstaller(handler, strategy, InstallKind.WindowsInstaller);
                var asset = new ReleaseAsset("MyApp-Setup.exe", "https://example.com/MyApp-Setup.exe", sha256, payload.Length);
                var progress = new SyncProgress();

                var result = await installer.DownloadAndInstallAsync(asset, progress, CancellationToken.None);

                Assert.True(result.Success);
                Assert.True(strategy.Invoked);
                Assert.NotNull(strategy.DownloadedFilePath);
                // The received file must have the .exe extension, not .part.
                Assert.EndsWith(".exe", strategy.DownloadedFilePath, StringComparison.OrdinalIgnoreCase);
                Assert.DoesNotContain(".part", strategy.DownloadedFilePath, StringComparison.OrdinalIgnoreCase);
                // The temp .part file must not be left behind after the rename.
                Assert.False(File.Exists(Path.Combine(Path.GetTempPath(), $"{asset.Name}.part")));
            }
        }
