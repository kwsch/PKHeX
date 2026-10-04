using System.Globalization;
using System.Text;
using System.Text.Json;
using Microsoft.Playwright;
using PKHeX.Web.Components;
using PKHeX.Web.Services;
using static Microsoft.Playwright.Assertions;

namespace PKHeX.Web.Tests;

/// <summary>One timed box change: from the click on Previous or Next box to the render that shows the new box.</summary>
/// <param name="From">The box shown before (zero-based).</param>
/// <param name="To">The box shown after.</param>
internal sealed record BoxStepSample(int From, int To, double Milliseconds);

/// <summary>The box changes timed in one engine.</summary>
internal sealed record BoxEngineTiming(string Engine, string BrowserVersion, IReadOnlyList<BoxStepSample> Steps);

/// <summary>A box navigation timing run.</summary>
/// <param name="BoxCount">Boxes in the save measured.</param>
internal sealed record BoxNavigationResult(BootEnvironment Environment, int BoxCount, IReadOnlyList<BoxEngineTiming> Engines);

/// <summary>
/// Times box changes in the published app on the largest admitted save, <see cref="SaveFixtures.Full"/> for ORAS, whose every
/// box is full, for each engine on loopback.
/// </summary>
/// <remarks>
/// After one unrecorded lap through every box, it steps through them all with Next box and back with Previous box. The time runs in the page
/// from the click to the render that changes the box heading. Blazor applies a render's changes together, so the grid has its new slots by
/// then. It fails if a step shows the wrong box or any request reaches the host while browsing, and has no timing threshold.
/// </remarks>
internal static class BoxNavigationTiming
{
    /// <summary>Engineering target for an ordinary box change once assets are warm; not enforced.</summary>
    public const double TargetMs = 100;

    /// <summary>Longest wait for one box change before the measurement counts as failed.</summary>
    private const float StepTimeoutMs = 30_000;

    /// <summary>Resets the in-page record and starts watching for the click and the change of the box heading.</summary>
    private const string StartRecording = """
        () => {
            const record = window.boxTiming = { clickAt: null, doneAt: null, title: null };
            const before = document.getElementById('box-title').textContent;
            if (window.boxTimingListener) {
                document.removeEventListener('click', window.boxTimingListener, true);
            }
            window.boxTimingListener = e => {
                if (e.target.closest('#box-prev, #box-next')) {
                    record.clickAt ??= performance.now();
                }
            };
            document.addEventListener('click', window.boxTimingListener, true);
            window.boxTimingObserver?.disconnect();
            window.boxTimingObserver = new MutationObserver(() => {
                const title = document.getElementById('box-title');
                if (record.clickAt !== null && record.doneAt === null && title && title.textContent !== before) {
                    record.doneAt = performance.now();
                    record.title = title.textContent;
                }
            });
            window.boxTimingObserver.observe(document.body, { subtree: true, childList: true, characterData: true });
        }
        """;

    /// <summary>Measures every engine against the publish at <paramref name="published"/>.</summary>
    /// <param name="published">The Release publish <c>wwwroot</c>.</param>
    /// <param name="channel">Installed Chromium channel to measure instead of Playwright's Chromium (<see cref="PerfBrowser.ParseChannel"/>), or null.</param>
    public static async Task<BoxNavigationResult> MeasureAsync(string published, string? channel)
    {
        var save = SaveFixtures.Full(true);
        var native = SaveFixtures.Open(save);
        var boxCount = StorageView.BoxCount(native);
        var titles = Enumerable.Range(0, boxCount).Select(b => SlotText.BoxOption(b, StorageView.Box(native, b).StoredName)).ToArray();

        using var host = new StaticHost(Path.GetFullPath(published), deploymentCaching: true);
        using var playwright = await Playwright.CreateAsync();
        var engines = new List<BoxEngineTiming>();
        JsonElement? build = null;
        foreach (var engine in PerfBrowser.Engines)
        {
            var name = PerfBrowser.Label(engine, channel);
            await using var browser = await PerfBrowser.LaunchAsync(playwright, engine, channel);
            var page = await browser.NewPageAsync();
            var pageErrors = 0;
            page.PageError += (_, _) => Interlocked.Increment(ref pageErrors);
            await page.GotoAsync(host.Url);
            await page.Locator("#save-file:enabled").WaitForAsync(new() { State = WaitForSelectorState.Attached, Timeout = StepTimeoutMs });
            build ??= await page.EvaluateAsync<JsonElement>(BootBaseline.ReadBuild);
            await ProofPage.Load(page, save, "boxes");
            await Expect(page.Locator("#box-title")).ToHaveTextAsync(titles[0], new() { Timeout = StepTimeoutMs });
            await page.WaitForLoadStateAsync(LoadState.NetworkIdle);
            host.TakeLog();

            var box = 0;
            for (var i = 0; i < boxCount - 1; i++)
            {
                await StepAsync(page, name, "#box-next", box, ++box, titles);
            }
            for (var i = 0; i < boxCount - 1; i++)
            {
                await StepAsync(page, name, "#box-prev", box, --box, titles);
            }
            var steps = new List<BoxStepSample>();
            for (var i = 0; i < boxCount - 1; i++)
            {
                steps.Add(await StepAsync(page, name, "#box-next", box, ++box, titles));
            }
            for (var i = 0; i < boxCount - 1; i++)
            {
                steps.Add(await StepAsync(page, name, "#box-prev", box, --box, titles));
            }

            var requests = host.TakeLog();
            if (requests.Count != 0)
            {
                throw new InvalidOperationException($"{name}: browsing boxes made requests: {string.Join(", ", requests.Select(r => $"{r.Method} {r.Path}"))}.");
            }
            if (pageErrors != 0)
            {
                throw new InvalidOperationException($"{name}: {pageErrors} page errors while browsing boxes.");
            }
            engines.Add(new(name, browser.Version, steps));
        }
        var reported = build!.Value;
        var environment = BootBaseline.DescribeEnvironment(reported.GetProperty("version").GetString()!, reported.GetProperty("commit").GetString()!);
        return new(environment, boxCount, engines);
    }

    /// <summary>Clicks <paramref name="button"/>, waits for the box heading to change and checks it names box <paramref name="to"/>.</summary>
    private static async Task<BoxStepSample> StepAsync(IPage page, string engine, string button, int from, int to, string[] titles)
    {
        await page.EvaluateAsync(StartRecording);
        await page.Locator(button).ClickAsync();
        await page.WaitForFunctionAsync("() => window.boxTiming.doneAt !== null", null, new() { Timeout = StepTimeoutMs });
        var record = await page.EvaluateAsync<JsonElement>("() => window.boxTiming");
        if (record.GetProperty("title").GetString() != titles[to])
        {
            throw new InvalidOperationException($"{engine}: stepping from box {from + 1} did not show box {to + 1}.");
        }
        return new(from, to, record.GetProperty("doneAt").GetDouble() - record.GetProperty("clickAt").GetDouble());
    }

    /// <summary>The report for the CI run summary: per engine, p50/p95/max against the target.</summary>
    public static string ToMarkdown(BoxNavigationResult result)
    {
        var invariant = CultureInfo.InvariantCulture;
        var env = result.Environment;
        var sb = new StringBuilder();
        sb.AppendLine("## Box navigation timing");
        sb.AppendLine();
        sb.AppendLine(invariant, $"PKHeX.Web {env.WebVersion} (`{env.SourceCommit}`), measured {env.MeasuredUtc} UTC on {env.Processor} ({env.ProcessorCount} logical CPUs), {env.OperatingSystem} {env.Architecture}{(env.ContinuousIntegration is { } ci ? $", {ci}" : "")}.");
        sb.AppendLine();
        sb.AppendLine(invariant, $"A full ORAS save ({result.BoxCount} full boxes) on loopback. After one unrecorded lap, every box with Next box and back with Previous box. Time runs from the click to the render showing the new box. No request reached the host while browsing. The target (≤ {TargetMs:F0} ms once assets are warm) is not enforced here.");
        sb.AppendLine();
        sb.AppendLine("| Engine | p50 | p95 | Max | Over target | Steps |");
        sb.AppendLine("|---|---:|---:|---:|---:|---:|");
        foreach (var engine in result.Engines)
        {
            var times = engine.Steps.Select(s => s.Milliseconds).ToList();
            sb.AppendLine(invariant, $"| {engine.Engine} {engine.BrowserVersion} | {LegalityTiming.Percentile(times, 50):F0} ms | {LegalityTiming.Percentile(times, 95):F0} ms | {times.Max():F0} ms | {times.Count(t => t > TargetMs)} | {times.Count} |");
        }
        return sb.ToString();
    }

    /// <summary>The full result as indented JSON, for comparing runs.</summary>
    public static string ToJson(BoxNavigationResult result) => JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true });
}
