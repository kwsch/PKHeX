using System.Runtime.InteropServices;
using System.Text.Json;
using Microsoft.Playwright;

namespace PKHeX.Web.Tests;

/// <summary>
/// Measures cold and warm boots of the published app (WEB-PERF-001) through a <see cref="StaticHost"/> in deployment-caching mode.
/// </summary>
/// <remarks>
/// Each sample uses its own browser profile. The cold boot launches a new browser process on the empty profile; the warm boot closes that browser
/// and relaunches it on the same profile, like a returning visit: only what the browser stored in the profile (its HTTP cache) carries over.
/// Chromium is measured on the <see cref="ThrottledProfile"/> through the DevTools protocol, which Firefox and WebKit do not offer to Playwright;
/// all three engines are also measured on unthrottled loopback, which shows the startup cost without the network.
/// The measurement fails only when a boot cannot be measured; it has no timing threshold.
/// </remarks>
internal static class BootBaseline
{
    /// <summary>Measured boots per configuration when <see cref="TestEnvironment.PerfRuns"/> is not set.</summary>
    public const int DefaultRuns = 5;

    /// <summary>Label of the network profile <c>PKHeX.Web.md</c> states its startup targets for.</summary>
    public const string ThrottledProfile = "20 Mbps / 50 ms";

    /// <summary>20 Mbps in bytes per second, for both directions.</summary>
    private const double ThrottledBytesPerSecond = 20_000_000 / 8.0;

    /// <summary>Added latency per request, in milliseconds.</summary>
    private const double ThrottledLatencyMs = 50;

    /// <summary>Longest wait for the usable shell before the boot counts as failed.</summary>
    private const float ShellTimeoutMs = 60_000;

    /// <summary>Requested by some browsers on their own; the published app has none.</summary>
    private const string FaviconPath = "/favicon.ico";

    /// <summary>
    /// Records <c>performance.now()</c> the first time the file input exists and is enabled, which is when the shell is usable.
    /// A mutation observer reacts in the same task as the render, unlike polling from the test. Playwright runs init scripts outside the page's CSP.
    /// </summary>
    private const string ShellReadyRecorder = """
        (() => {
            const observer = new MutationObserver(() => {
                const input = document.getElementById('save-file');
                if (input && !input.disabled && window.__pkhexShellReady === undefined) {
                    window.__pkhexShellReady = performance.now();
                    observer.disconnect();
                }
            });
            observer.observe(document, { childList: true, subtree: true, attributes: true, attributeFilter: ['disabled'] });
        })();
        """;

    /// <summary>
    /// Reads the build the published app reports in its About panel: the version and the full commit.
    /// The test assembly's own <see cref="PKHeX.Web.Services.BuildInfo"/> is not used, because the publish under test may come from another build.
    /// The panel is in the page while collapsed, so nothing has to be clicked. Missing values read as <c>unknown</c>.
    /// Keep the selectors in step with the E2E boot test's About check.
    /// </summary>
    internal const string ReadBuild = """
        () => ({
            version: document.getElementById('about-version')?.textContent.trim() || 'unknown',
            commit: document.getElementById('about-commit')?.textContent.trim() || 'unknown',
        })
        """;

    /// <summary>Reads the shell-ready time and the navigation timing of the current document.</summary>
    private const string ReadTimings = """
        () => {
            const navigation = performance.getEntriesByType('navigation')[0];
            return { shellReady: window.__pkhexShellReady, domContentLoaded: navigation ? navigation.domContentLoadedEventEnd : -1 };
        }
        """;

    /// <summary>Measures every configuration <paramref name="runs"/> times against the publish at <paramref name="published"/>.</summary>
    /// <param name="published">The Release publish <c>wwwroot</c>.</param>
    /// <param name="runs">Measured cold/warm pairs per configuration, after one unrecorded pair.</param>
    public static async Task<BootBaselineResult> MeasureAsync(string published, int runs)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(runs, 1);
        var root = Path.GetFullPath(published);
        using var host = new StaticHost(root, deploymentCaching: true);
        using var playwright = await Playwright.CreateAsync();

        var coreResources = MeasureCoreResources();
        (string Engine, bool Throttled)[] configurations = [("chromium", true), ("chromium", false), ("firefox", false), ("webkit", false)];
        var results = new List<BootConfiguration>();
        var bootSet = new SortedSet<string>(StringComparer.Ordinal);
        JsonElement? build = null;
        foreach (var (engine, throttled) in configurations)
        {
            var type = engine switch
            {
                "chromium" => playwright.Chromium,
                "firefox" => playwright.Firefox,
                "webkit" => playwright.Webkit,
                _ => throw new ArgumentOutOfRangeException(nameof(engine), engine, null),
            };
            var cold = new List<BootSample>();
            var warm = new List<BootSample>();
            var network = throttled ? ThrottledProfile : "loopback";
            string version;
            await using (var browser = await type.LaunchAsync(new() { Headless = true }))
            {
                // A persistent context has no browser object to ask.
                version = browser.Version;
            }

            var profiles = Directory.CreateTempSubdirectory("pkhex-perf-");
            try
            {
                // Run 0 warms the host and the operating system's file cache and is not recorded.
                for (var run = 0; run <= runs; run++)
                {
                    var profile = Path.Combine(profiles.FullName, run.ToString(System.Globalization.CultureInfo.InvariantCulture));
                    var (coldSample, coldLog) = await BootInProfileAsync(type, profile, host, throttled, $"{engine} ({network}) run {run} cold",
                        async page => build ??= await page.EvaluateAsync<JsonElement>(ReadBuild));
                    var (warmSample, _) = await BootInProfileAsync(type, profile, host, throttled, $"{engine} ({network}) run {run} warm", null);
                    if (run == 0)
                    {
                        continue;
                    }
                    cold.Add(coldSample);
                    warm.Add(warmSample);
                    bootSet.UnionWith(coldLog.Where(r => r.Status == 200).Select(r => ToPublishedPath(r.Path)));
                }
            }
            finally
            {
                DeleteProfiles(profiles);
            }
            results.Add(new(engine, version, throttled, cold, warm));
        }

        var reported = build!.Value;
        var environment = DescribeEnvironment(reported.GetProperty("version").GetString()!, reported.GetProperty("commit").GetString()!);
        return new(environment, runs, results, bootSet.Select(p => DescribeFile(root, p)).ToList(), coreResources);
    }

    /// <summary>
    /// Launches <paramref name="type"/> on the persistent <paramref name="profile"/>, boots once and closes the browser, so the next launch on the profile starts a new process.
    /// </summary>
    private static async Task<(BootSample Sample, IReadOnlyList<StaticHost.ServedResponse> Log)> BootInProfileAsync(IBrowserType type, string profile, StaticHost host, bool throttled, string label, Func<IPage, Task>? afterReady)
    {
        await using var context = await type.LaunchPersistentContextAsync(profile, new() { Headless = true });
        await context.AddInitScriptAsync(ShellReadyRecorder);
        return await BootAsync(context, host, throttled, label, afterReady);
    }

    /// <summary>Removes the temporary profiles. A browser that is still releasing its files must not turn a measured run into a failure.</summary>
    private static void DeleteProfiles(DirectoryInfo profiles)
    {
        try
        {
            profiles.Delete(true);
        }
        catch (IOException)
        {
        }
        catch (UnauthorizedAccessException)
        {
        }
    }

    /// <summary>
    /// Raw sizes of the resources embedded in PKHeX.Core, grouped by their folder under <c>Resources/</c> (<c>text</c>, <c>legality</c>, …).
    /// They are read from the PKHeX.Core the tests reference, built from the same sources as the publish of this commit.
    /// Core loads them by name, so they are downloaded with the assembly on every boot whether or not a feature uses them.
    /// </summary>
    internal static IReadOnlyList<CoreResourceGroup> MeasureCoreResources()
    {
        const string prefix = "PKHeX.Core.Resources.";
        var assembly = typeof(PKHeX.Core.SaveFile).Assembly;
        return assembly.GetManifestResourceNames()
            .Select(name =>
            {
                using var stream = assembly.GetManifestResourceStream(name)!;
                var group = name.StartsWith(prefix, StringComparison.Ordinal) ? name[prefix.Length..].Split('.')[0] : "(other)";
                return (Group: group, stream.Length);
            })
            .GroupBy(r => r.Group, StringComparer.Ordinal)
            .Select(g => new CoreResourceGroup(g.Key, g.Count(), g.Sum(r => r.Length)))
            .OrderByDescending(g => g.Bytes)
            .ThenBy(g => g.Group, StringComparer.Ordinal)
            .ToList();
    }

    /// <summary>Boots the app once in a new page of <paramref name="context"/> and closes the page, returning the sample and the host's responses.</summary>
    /// <param name="context">Browser context to open the page in.</param>
    /// <param name="host">Host serving the publish; its log is taken for this boot only.</param>
    /// <param name="throttled">Whether to apply <see cref="ThrottledProfile"/>.</param>
    /// <param name="label">Names the boot in a failure message.</param>
    /// <param name="afterReady">Optional extra read from the page once the shell is ready, before the page closes.</param>
    private static async Task<(BootSample Sample, IReadOnlyList<StaticHost.ServedResponse> Log)> BootAsync(IBrowserContext context, StaticHost host, bool throttled, string label, Func<IPage, Task>? afterReady)
    {
        var page = await context.NewPageAsync();
        var pageErrors = 0;
        page.PageError += (_, _) => Interlocked.Increment(ref pageErrors);
        try
        {
            if (throttled)
            {
                var devTools = await context.NewCDPSessionAsync(page);
                await devTools.SendAsync("Network.enable");
                await devTools.SendAsync("Network.emulateNetworkConditions", new Dictionary<string, object>
                {
                    ["offline"] = false,
                    ["latency"] = ThrottledLatencyMs,
                    ["downloadThroughput"] = ThrottledBytesPerSecond,
                    ["uploadThroughput"] = ThrottledBytesPerSecond,
                });
            }

            host.TakeLog();
            await page.GotoAsync(host.Url);
            await page.WaitForFunctionAsync("() => window.__pkhexShellReady !== undefined", null, new() { Timeout = ShellTimeoutMs });
            // Late requests still belong to this boot; wait for them so they are not counted in the next one.
            await page.WaitForLoadStateAsync(LoadState.NetworkIdle);
            var timings = await page.EvaluateAsync<JsonElement>(ReadTimings);
            if (afterReady is not null)
            {
                await afterReady(page);
            }
            var log = host.TakeLog();

            AssertMeasurable(log, Volatile.Read(ref pageErrors));
            var byEncoding = log.GroupBy(r => r.Encoding ?? "identity").ToDictionary(g => g.Key, g => g.Sum(r => r.BodyBytes));
            var sample = new BootSample(
                timings.GetProperty("shellReady").GetDouble(),
                timings.GetProperty("domContentLoaded").GetDouble(),
                log.Count(r => r.Path != FaviconPath),
                log.Count(r => r.Status == 304),
                log.Sum(r => r.BodyBytes),
                byEncoding);
            await page.CloseAsync();
            return (sample, log);
        }
        catch (Exception ex)
        {
            // Closing a page of a crashed browser can throw too; that must not replace the reason the boot failed.
            try
            {
                await page.CloseAsync();
            }
            catch (PlaywrightException)
            {
            }
            throw new InvalidOperationException($"Boot {label} could not be measured: {ex.Message}", ex);
        }
    }

    /// <summary>A boot counts only if it made plain GETs for published files (or the favicon) and raised no page error.</summary>
    private static void AssertMeasurable(IReadOnlyList<StaticHost.ServedResponse> log, int pageErrors)
    {
        if (pageErrors != 0)
        {
            throw new InvalidOperationException($"The boot raised {pageErrors} page errors.");
        }
        if (log.Count == 0)
        {
            throw new InvalidOperationException("The host served nothing for the boot.");
        }
        var unexpected = log.Where(r => r.Method != "GET" || !(r.Status is 200 or 304 || (r.Status == 404 && r.Path == FaviconPath))).ToList();
        if (unexpected.Count != 0)
        {
            throw new InvalidOperationException($"Unexpected responses during boot: {string.Join(", ", unexpected.Select(r => $"{r.Method} {r.Path} {r.Status}"))}");
        }
    }

    /// <summary>Maps a request path at the site root to the published file it served.</summary>
    private static string ToPublishedPath(string requestPath) => requestPath == "/" ? "index.html" : Uri.UnescapeDataString(requestPath.TrimStart('/'));

    private static BootSetFile DescribeFile(string root, string relativePath)
    {
        var file = Path.Combine(root, relativePath);
        return new(relativePath, new FileInfo(file).Length, SizeOf(file + ".br"), SizeOf(file + ".gz"));
    }

    private static long? SizeOf(string file) => File.Exists(file) ? new FileInfo(file).Length : null;

    /// <summary>The processor model: <c>machdep.cpu.brand_string</c> on macOS, <c>model name</c> in <c>/proc/cpuinfo</c> on Linux, <c>PROCESSOR_IDENTIFIER</c> on Windows.</summary>
    private static string DescribeProcessor()
    {
        try
        {
            if (OperatingSystem.IsMacOS())
            {
                using var sysctl = System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo("sysctl", "-n machdep.cpu.brand_string") { RedirectStandardOutput = true })!;
                var model = sysctl.StandardOutput.ReadToEnd().Trim();
                sysctl.WaitForExit();
                return model.Length > 0 ? model : "unknown";
            }
            if (OperatingSystem.IsLinux())
            {
                var line = File.ReadLines("/proc/cpuinfo").FirstOrDefault(l => l.StartsWith("model name", StringComparison.Ordinal));
                return line?[(line.IndexOf(':') + 1)..].Trim() ?? "unknown";
            }
            return Environment.GetEnvironmentVariable("PROCESSOR_IDENTIFIER") ?? "unknown";
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or System.ComponentModel.Win32Exception)
        {
            return "unknown";
        }
    }

    /// <param name="webVersion">Version the published app reports.</param>
    /// <param name="sourceCommit">Commit the published app reports.</param>
    internal static BootEnvironment DescribeEnvironment(string webVersion, string sourceCommit)
    {
        string? ci = null;
        if (Environment.GetEnvironmentVariable("GITHUB_ACTIONS") == "true")
        {
            ci = $"GitHub Actions, {Environment.GetEnvironmentVariable("RUNNER_OS")} image {Environment.GetEnvironmentVariable("ImageOS")} {Environment.GetEnvironmentVariable("ImageVersion")}".TrimEnd();
        }
        return new(
            DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm", System.Globalization.CultureInfo.InvariantCulture),
            RuntimeInformation.OSDescription,
            RuntimeInformation.OSArchitecture.ToString(),
            DescribeProcessor(),
            Environment.ProcessorCount,
            GC.GetGCMemoryInfo().TotalAvailableMemoryBytes,
            RuntimeInformation.FrameworkDescription,
            typeof(IPlaywright).Assembly.GetName().Version?.ToString(3) ?? "unknown",
            webVersion,
            sourceCommit,
            ci);
    }
}
