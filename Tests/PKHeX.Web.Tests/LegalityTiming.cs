using System.Globalization;
using System.Text;
using System.Text.Json;
using Microsoft.Playwright;
using PKHeX.Web.Services;
using PKHeX.Web.State;

namespace PKHeX.Web.Tests;

/// <summary>One timed analysis: how long the legality panel was busy, from marking the run to showing its verdict.</summary>
/// <param name="Fixture">The corpus file analysed.</param>
/// <param name="Family">"XY" or "ORAS", the save the entity was opened in.</param>
/// <param name="Verdict">The verdict the page showed.</param>
/// <param name="Milliseconds">Busy time in the page, including the wait for one paint and the final render.</param>
internal sealed record LegalitySample(string Fixture, string Family, string Verdict, double Milliseconds);

/// <summary>Timings of one engine: the first analysis of the page (which loads Core's legality tables) and every later one.</summary>
internal sealed record LegalityEngineTiming(string Engine, string BrowserVersion, LegalitySample First, IReadOnlyList<LegalitySample> Warm);

/// <summary>The legality timing run: environment, the corpus size, and the timings per engine.</summary>
internal sealed record LegalityTimingResult(BootEnvironment Environment, int CorpusSize, IReadOnlyList<LegalityEngineTiming> Engines);

/// <summary>
/// Times the selected-entity legality analysis in the published app (WEB-PERF-004), for each engine on loopback, over every PK6 in Core's
/// legality test fixtures, opened in an XY and an ORAS save.
/// </summary>
/// <remarks>
/// Each slot is opened as a user would, and the analysis runs through the app's idle-delay path. The time is measured in the page from the
/// moment the panel is marked busy (<c>aria-busy</c>) to the render that shows the verdict, so it includes one paint wait and the render, but
/// not the idle delay. Core's analysis is synchronous on the page's only thread, so this is also how long the page stops responding.
/// The first analysis of each page is reported apart: it loads Core's legality tables.
/// </remarks>
internal static class LegalityTiming
{
    /// <summary>Engineering target for the selected-entity analysis at p95 (<c>PKHeX.Web.md</c>, performance section); not enforced.</summary>
    public const double TargetP95Ms = 500;

    /// <summary>Stall length <c>PKHeX.Web.md</c> treats as unresponsive when repeated.</summary>
    public const double StallMs = 200;

    /// <summary>Longest wait for one analysis before the measurement counts as failed.</summary>
    private const float AnalysisTimeoutMs = 60_000;

    /// <summary>Resets the in-page record and starts watching the legality panel for its busy mark and final verdict.</summary>
    private const string StartRecording = """
        () => {
            const record = window.legalityTiming = { busyAt: null, doneAt: null, verdict: null };
            window.legalityTimingObserver?.disconnect();
            const check = () => {
                const panel = document.getElementById('legality');
                const status = document.getElementById('legality-status');
                if (!panel || !status) {
                    return;
                }
                const now = performance.now();
                if (panel.getAttribute('aria-busy') === 'true') {
                    record.busyAt ??= now;
                } else if (record.busyAt !== null && record.doneAt === null && ['Valid', 'Invalid', 'Unavailable'].includes(status.textContent)) {
                    record.doneAt = now;
                    record.verdict = status.textContent;
                }
            };
            window.legalityTimingObserver = new MutationObserver(check);
            window.legalityTimingObserver.observe(document.body, { subtree: true, childList: true, characterData: true, attributes: true, attributeFilter: ['aria-busy'] });
        }
        """;

    /// <summary>Measures every engine against the publish at <paramref name="published"/>.</summary>
    public static async Task<LegalityTimingResult> MeasureAsync(string published)
    {
        var corpus = SaveFixtures.LegalityCorpus();
        (string Family, byte[] Bytes, SaveSession Native)[] saves =
        [
            Save(false, corpus),
            Save(true, corpus),
        ];
        using var host = new StaticHost(Path.GetFullPath(published), deploymentCaching: true);
        using var playwright = await Playwright.CreateAsync();
        var engines = new List<LegalityEngineTiming>();
        JsonElement? build = null;
        foreach (var engine in new[] { "chromium", "firefox", "webkit" })
        {
            var type = engine switch
            {
                "chromium" => playwright.Chromium,
                "firefox" => playwright.Firefox,
                _ => playwright.Webkit,
            };
            await using var browser = await type.LaunchAsync(new() { Headless = true });
            var page = await browser.NewPageAsync();
            var pageErrors = 0;
            page.PageError += (_, _) => Interlocked.Increment(ref pageErrors);
            await page.GotoAsync(host.Url);
            await page.Locator("#save-file:enabled").WaitForAsync(new() { Timeout = AnalysisTimeoutMs });
            build ??= await page.EvaluateAsync<JsonElement>(BootBaseline.ReadBuild);

            var samples = new List<LegalitySample>();
            foreach (var (family, bytes, native) in saves)
            {
                await ProofPage.Load(page, bytes, $"timing-{family}");
                await page.Locator("#session-state").WaitForAsync();
                for (var i = 0; i < corpus.Count; i++)
                {
                    samples.Add(await TimeSlotAsync(page, engine, family, corpus[i].Name, native, SlotRef.InBox(0, i + 1)));
                }
            }
            if (pageErrors != 0)
            {
                throw new InvalidOperationException($"{engine}: {pageErrors} page errors during legality timing.");
            }
            engines.Add(new(engine, browser.Version, samples[0], samples.Skip(1).ToList()));
        }
        var reported = build!.Value;
        var environment = BootBaseline.DescribeEnvironment(reported.GetProperty("version").GetString()!, reported.GetProperty("commit").GetString()!);
        return new(environment, corpus.Count, engines);
    }

    private static (string, byte[], SaveSession) Save(bool oras, IReadOnlyList<(string Name, byte[] Data)> corpus)
    {
        var bytes = SaveFixtures.Synthetic(oras, customize: SaveFixtures.WithBoxEntities(corpus.Select(c => c.Data)));
        return (oras ? "ORAS" : "XY", bytes, SaveFixtures.Open(bytes));
    }

    /// <summary>Opens <paramref name="slot"/>, waits for its verdict, checks it against native Core and returns the busy time.</summary>
    private static async Task<LegalitySample> TimeSlotAsync(IPage page, string engine, string family, string fixture, SaveSession native, SlotRef slot)
    {
        var label = $"{engine} {family} {fixture}";
        await page.EvaluateAsync(StartRecording);
        await ProofPage.Select(page, slot);
        await page.WaitForFunctionAsync("() => window.legalityTiming.doneAt !== null", null, new() { Timeout = AnalysisTimeoutMs });
        var record = await page.EvaluateAsync<JsonElement>("() => window.legalityTiming");
        var verdict = record.GetProperty("verdict").GetString()!;
        var expected = LegalityService.Default.Analyze(native, native.Select(slot), out _).Verdict.ToString();
        if (verdict != expected)
        {
            throw new InvalidOperationException($"{label}: the page showed {verdict}, native Core gives {expected}.");
        }
        var milliseconds = record.GetProperty("doneAt").GetDouble() - record.GetProperty("busyAt").GetDouble();
        return new(fixture, family, verdict, milliseconds);
    }

    /// <summary>The value at <paramref name="percentile"/> (0–100) of <paramref name="values"/>, by the nearest-rank method.</summary>
    public static double Percentile(IReadOnlyList<double> values, double percentile)
    {
        ArgumentOutOfRangeException.ThrowIfZero(values.Count);
        var sorted = values.Order().ToArray();
        var rank = (int)Math.Ceiling(percentile / 100 * sorted.Length);
        return sorted[Math.Clamp(rank - 1, 0, sorted.Length - 1)];
    }

    /// <summary>The report for the CI run summary: per engine, the first analysis and the warm p50/p95/max against the target.</summary>
    public static string ToMarkdown(LegalityTimingResult result)
    {
        var invariant = CultureInfo.InvariantCulture;
        var env = result.Environment;
        var sb = new StringBuilder();
        sb.AppendLine("## Legality analysis timing (WEB-PERF-004)");
        sb.AppendLine();
        sb.AppendLine(invariant, $"PKHeX.Web {env.WebVersion} ({env.SourceCommit}), measured {env.MeasuredUtc} UTC on {env.Processor} ({env.ProcessorCount} logical CPUs), {env.OperatingSystem} {env.Architecture}{(env.ContinuousIntegration is { } ci ? $", {ci}" : "")}.");
        sb.AppendLine();
        sb.AppendLine(invariant, $"Every PK6 in Core's legality test fixtures ({result.CorpusSize}), opened in an XY and an ORAS save, analysed once each through the app's idle-delay path on loopback. Time is how long the panel was busy: one paint wait, Core's synchronous analysis and the render. The first analysis of each page loads Core's legality tables and is shown apart. The target (p95 ≤ {TargetP95Ms:F0} ms, `PKHeX.Web.md`) is not enforced here.");
        sb.AppendLine();
        sb.AppendLine("| Engine | First analysis | Warm p50 | Warm p95 | Warm max | Warm over 200 ms | Samples |");
        sb.AppendLine("|---|---:|---:|---:|---:|---:|---:|");
        foreach (var engine in result.Engines)
        {
            var warm = engine.Warm.Select(s => s.Milliseconds).ToList();
            var over = warm.Count(ms => ms > StallMs);
            sb.AppendLine(invariant, $"| {engine.Engine} {engine.BrowserVersion} | {engine.First.Milliseconds:F0} ms | {Percentile(warm, 50):F0} ms | {Percentile(warm, 95):F0} ms | {warm.Max():F0} ms | {over} | {warm.Count + 1} |");
        }
        sb.AppendLine();
        sb.AppendLine("Slowest warm analyses:");
        sb.AppendLine();
        sb.AppendLine("| Engine | Save | Fixture | Verdict | Time |");
        sb.AppendLine("|---|---|---|---|---:|");
        foreach (var engine in result.Engines)
        {
            foreach (var sample in engine.Warm.OrderByDescending(s => s.Milliseconds).Take(3))
            {
                sb.AppendLine(invariant, $"| {engine.Engine} | {sample.Family} | `{sample.Fixture}` | {sample.Verdict} | {sample.Milliseconds:F0} ms |");
            }
        }
        return sb.ToString();
    }

    /// <summary>The full result as indented JSON, for comparing runs.</summary>
    public static string ToJson(LegalityTimingResult result) => JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true });
}
