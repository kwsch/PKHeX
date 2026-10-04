using System.Buffers.Binary;
using System.Globalization;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.Playwright;
using PKHeX.Core;
using PKHeX.Web.Components;
using PKHeX.Web.Interop;
using PKHeX.Web.Services;
using PKHeX.Web.State;
using static Microsoft.Playwright.Assertions;

namespace PKHeX.Web.Tests;

/// <summary>Memory after one step of the session in the page.</summary>
/// <param name="Stage">What had just happened.</param>
/// <param name="LinearBytes">Size of the .NET runtime's WebAssembly memory. It never shrinks, so it is the peak up to this point.</param>
/// <param name="JsHeapBytes">The page's JavaScript heap after a forced collection; Chromium only, null elsewhere.</param>
internal sealed record MemoryStage(string Stage, long LinearBytes, long? JsHeapBytes);

/// <summary>Memory of one engine: after each step of one session, and after each of the repeated sessions.</summary>
internal sealed record MemoryEngine(string Engine, string BrowserVersion, IReadOnlyList<MemoryStage> Stages, IReadOnlyList<MemoryStage> Cycles);

/// <summary>Managed bytes the same step allocates when the Web code runs natively on desktop .NET.</summary>
/// <param name="InputBytes">Size of the file the step works on, to show the allocation as copies of it.</param>
internal sealed record NativeAllocation(string Stage, long Bytes, long InputBytes);

/// <summary>The sprite atlas of the publish made with sprites: its file size and the memory it takes once a browser decodes it.</summary>
/// <param name="DecodedBytes">Width × height × 4 (RGBA), what a browser holds for the decoded image.</param>
internal sealed record AtlasSize(string File, long Bytes, int Width, int Height, long DecodedBytes);

/// <summary>A memory baseline run (WEB-PERF-002).</summary>
/// <param name="SaveBytes">Size of the save measured.</param>
/// <param name="Entities">Occupied party positions and box slots in it.</param>
/// <param name="Atlas">Null when no publish with sprites was given.</param>
internal sealed record MemoryBaselineResult(BootEnvironment Environment, int SaveBytes, int Entities, IReadOnlyList<NativeAllocation> Native, IReadOnlyList<MemoryEngine> Engines, AtlasSize? Atlas);

/// <summary>
/// Measures the memory of the published app (WEB-PERF-002) on the largest admitted save, <see cref="SaveFixtures.Full"/> for ORAS, in each engine
/// on loopback: after each step of a session, over repeated sessions in one page, and then after refusing files at the read limit.
/// </summary>
/// <remarks>
/// <para>
/// The page's figure is the size of the WebAssembly memory of the .NET runtime, read through its public <c>getDotnetRuntime</c> API. It holds
/// the managed heap (original bytes, working save, staged clones, export bytes, legality tables) and the runtime itself. WebAssembly memory
/// only grows, so each reading is the peak so far, and a series that stops growing shows memory is reused rather than leaked. Chromium also
/// reports its JavaScript heap after a forced collection, which holds the copies passed to and from the page's scripts. A downloaded Blob lives
/// in the browser process and is not in either figure.
/// </para>
/// <para>
/// The native figures run the same Web code on desktop .NET and count the managed bytes each step allocates, which shows where the page's
/// memory goes (how many copies of the save each step makes). They are not WebAssembly measurements.
/// </para>
/// It has no threshold: it fails only if a step cannot be measured.
/// </remarks>
internal static class MemoryBaseline
{
    /// <summary>
    /// Sessions opened, edited, applied, downloaded and closed one after another in one page, when <see cref="TestEnvironment.PerfSessions"/> is not set.
    /// </summary>
    /// <remarks>
    /// The runtime grows its memory in large steps (about 26 MiB), not a little per session. In every run so far each engine settled at
    /// 153–161 MiB within its first 30 sessions and then stayed there, over 100 sessions too.
    /// </remarks>
    public const int DefaultSessions = 40;

    /// <summary>Longest wait for one step before the measurement counts as failed.</summary>
    private const float StepTimeoutMs = 60_000;

    /// <summary>The size of the .NET runtime's WebAssembly memory.</summary>
    private const string ReadLinearMemory = "() => globalThis.getDotnetRuntime(0).localHeapViewU8().byteLength";

    /// <summary>Measures every engine against the publish at <paramref name="published"/>.</summary>
    /// <param name="published">The Release publish <c>wwwroot</c>.</param>
    /// <param name="channel">Installed Chromium channel to measure instead of Playwright's Chromium (<see cref="PerfBrowser.ParseChannel"/>), or null.</param>
    /// <param name="publishedSprites">The <c>wwwroot</c> of the publish made with sprites, for the atlas size, or null.</param>
    /// <param name="sessions">Repeated sessions in one page (<see cref="DefaultSessions"/> unless set).</param>
    public static async Task<MemoryBaselineResult> MeasureAsync(string published, string? channel, string? publishedSprites, int sessions = DefaultSessions)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(sessions, 2);
        var save = SaveFixtures.Full(true);
        var native = MeasureNative(save);
        var opened = SaveFixtures.Open(save);
        var entities = StorageView.Party(opened).Count(s => s.Occupied)
            + Enumerable.Range(0, StorageView.BoxCount(opened)).Sum(b => StorageView.Box(opened, b).Slots.Count(s => s.Occupied));

        using var host = new StaticHost(Path.GetFullPath(published), deploymentCaching: true);
        using var playwright = await Playwright.CreateAsync();
        var engines = new List<MemoryEngine>();
        JsonElement? build = null;
        foreach (var engine in PerfBrowser.Engines)
        {
            var name = PerfBrowser.Label(engine, channel);
            await using var browser = await PerfBrowser.LaunchAsync(playwright, engine, channel);
            var context = await browser.NewContextAsync(new() { AcceptDownloads = true });
            var page = await context.NewPageAsync();
            var pageErrors = 0;
            page.PageError += (_, _) => Interlocked.Increment(ref pageErrors);
            var devTools = engine == "chromium" ? await context.NewCDPSessionAsync(page) : null;
            try
            {
                await page.GotoAsync(host.Url);
                await page.Locator("#save-file:enabled").WaitForAsync(new() { State = WaitForSelectorState.Attached, Timeout = StepTimeoutMs });
                build ??= await page.EvaluateAsync<JsonElement>(BootBaseline.ReadBuild);

                var stages = new List<MemoryStage> { await ReadAsync(page, devTools, "Shell ready") };
                await OpenAsync(page, save, "memory-main");
                stages.Add(await ReadAsync(page, devTools, $"Opened ({entities} entities)"));
                await EditAndApplyAsync(page, SlotRef.InBox(0, 1), "Measured");
                stages.Add(await ReadAsync(page, devTools, "Edited, analysed and applied one box slot"));
                await ProofPage.DownloadNamed(page);
                stages.Add(await ReadAsync(page, devTools, "Downloaded (export written and reopened to check it)"));
                await CloseAsync(page);
                stages.Add(await ReadAsync(page, devTools, "Closed"));

                var cycles = new List<MemoryStage>();
                for (var cycle = 1; cycle <= sessions; cycle++)
                {
                    await OpenAsync(page, save, $"memory-{cycle}");
                    await EditAndApplyAsync(page, SlotRef.InBox((cycle - 1) % opened.Working.BoxCount, 2), $"Cycle{cycle}");
                    await ProofPage.DownloadNamed(page);
                    await CloseAsync(page);
                    cycles.Add(await ReadAsync(page, devTools, cycle.ToString(CultureInfo.InvariantCulture)));
                }

                // Last, because WebAssembly memory never shrinks: the high-water mark of a 16 MiB file would hide growth in the sessions above.
                // At the limit the whole file is read and copied before Core refuses it; one byte over, the read stops at the limit.
                await RefuseAsync(page, new byte[SaveLoader.MaxInputBytes]);
                stages.Add(await ReadAsync(page, devTools, "After the sessions, refused a 16 MiB file (read in full, then unrecognised)"));
                await RefuseAsync(page, new byte[SaveLoader.MaxInputBytes + 1]);
                stages.Add(await ReadAsync(page, devTools, "Refused a file over 16 MiB (too large)"));
                if (Volatile.Read(ref pageErrors) != 0)
                {
                    throw new InvalidOperationException($"{pageErrors} page errors.");
                }
                engines.Add(new(name, browser.Version, stages, cycles));
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Memory of {name} could not be measured: {ex.Message}", ex);
            }
        }

        var reported = build!.Value;
        var environment = BootBaseline.DescribeEnvironment(reported.GetProperty("version").GetString()!, reported.GetProperty("commit").GetString()!);
        return new(environment, save.Length, entities, native, engines, publishedSprites is null ? null : MeasureAtlas(publishedSprites));
    }

    /// <summary>Opens <paramref name="save"/> through the picker with nothing open, and waits for the session.</summary>
    private static async Task OpenAsync(IPage page, byte[] save, string fileName)
    {
        await ProofPage.Load(page, save, fileName);
        await Expect(page.Locator("#overview-file")).ToHaveTextAsync(fileName, new() { Timeout = StepTimeoutMs });
    }

    /// <summary>Opens <paramref name="slot"/>, gives it <paramref name="nickname"/>, waits for its legality result and applies it.</summary>
    private static async Task EditAndApplyAsync(IPage page, SlotRef slot, string nickname)
    {
        await ProofPage.Select(page, slot);
        await page.Locator("#nickname").FillAsync(nickname);
        await page.Locator("#nicknamed").CheckAsync();
        await ProofPage.Apply(page);
        await Expect(page.Locator("#session-state")).ToHaveTextAsync("Edited in memory", new() { Timeout = StepTimeoutMs });
    }

    /// <summary>Closes the session after its current revision was downloaded, which asks only for the confirmation that the file was checked.</summary>
    private static async Task CloseAsync(IPage page)
    {
        await page.Locator("#close-session").ClickAsync();
        await page.Locator("#exit-continue").ClickAsync();
        await Expect(page.Locator("#overview-game")).ToHaveCountAsync(0, new() { Timeout = StepTimeoutMs });
    }

    /// <summary>Picks <paramref name="bytes"/> with nothing open and waits for its refusal.</summary>
    private static async Task RefuseAsync(IPage page, byte[] bytes)
    {
        await ProofPage.Load(page, bytes);
        await Expect(page.Locator("#message")).ToContainTextAsync(UserMessages.For(SaveLoader.Load(bytes)), new() { Timeout = StepTimeoutMs });
    }

    /// <summary>Reads the WebAssembly memory and, in Chromium, the JavaScript heap after a forced collection.</summary>
    private static async Task<MemoryStage> ReadAsync(IPage page, ICDPSession? devTools, string stage)
    {
        long? jsHeap = null;
        if (devTools is not null)
        {
            await devTools.SendAsync("HeapProfiler.collectGarbage");
            await devTools.SendAsync("Performance.enable");
            var metrics = await devTools.SendAsync("Performance.getMetrics");
            jsHeap = (long)metrics!.Value.GetProperty("metrics").EnumerateArray()
                .Single(m => m.GetProperty("name").GetString() == "JSHeapUsedSize").GetProperty("value").GetDouble();
        }
        var linear = await page.EvaluateAsync<long>(ReadLinearMemory);
        return new(stage, linear, jsHeap);
    }

    /// <summary>
    /// Runs one session's steps on <paramref name="save"/> natively and counts the managed bytes each allocates on this thread. The steps run
    /// once unrecorded first, so Core's one-time table loading is not counted against the first step.
    /// </summary>
    internal static IReadOnlyList<NativeAllocation> MeasureNative(byte[] save)
    {
        Run(save, null);
        var recorded = new List<NativeAllocation>();
        Run(save, recorded);
        return recorded;

        static void Run(byte[] bytes, List<NativeAllocation>? record)
        {
            var session = Step(record, bytes.Length, "Open: copy, parse and round-trip check", () => SaveLoader.Load(bytes, "main").Session!);
            var draft = Step(record, bytes.Length, "Open a box slot as a draft", () => session.Select(SlotRef.InBox(0, 1)));
            Step(record, bytes.Length, "Edit the nickname", () =>
            {
                draft.EditNickname("Native", true);
                return 0;
            });
            var verdict = Step(record, bytes.Length, "Analyse legality", () => LegalityService.Default.Analyze(session, draft, out _).Verdict);
            Step(record, bytes.Length, "Apply: staged clone, write and checks", () =>
            {
                session.Apply(draft, verdict);
                return 0;
            });
            // As in the page, a download holding a change legality flagged needs the user's acknowledgement first.
            if (session.ExportNeedsAcknowledgement)
            {
                session.AcknowledgeExport(true);
            }
            Step(record, bytes.Length, "Export: write, reopen and checks", () => SaveExporter.Export(session, null));

            // The largest file the read limit lets through, refused by Core: what the page pays for it before any save is recognised.
            // The browser declares the file's size, as it does here. A memory stream completes its reads synchronously, so the read stays on this thread.
            var limit = new byte[SaveLoader.MaxInputBytes];
            var read = Step(record, limit.Length, "Read a 16 MiB file (bounded read)", () => BrowserFileService.ReadBoundedAsync(new MemoryStream(limit), "large", SaveLoader.MaxInputBytes, declaredLength: limit.Length).GetAwaiter().GetResult());
            Step(record, limit.Length, "Refuse it: copy and recognition attempt", () => SaveLoader.Load(read.Bytes, read.FileName).Failure);
        }

        static T Step<T>(List<NativeAllocation>? record, long input, string stage, Func<T> action)
        {
            var before = GC.GetAllocatedBytesForCurrentThread();
            var result = action();
            record?.Add(new(stage, GC.GetAllocatedBytesForCurrentThread() - before, input));
            return result;
        }
    }

    /// <summary>Reads the size of the sprite atlas from its PNG header in the publish made with sprites.</summary>
    internal static AtlasSize MeasureAtlas(string publishedSprites)
    {
        var atlas = Directory.EnumerateFiles(Path.Combine(publishedSprites, "sprites"))
            .Where(f => Regex.IsMatch(Path.GetFileName(f), @"^pokemon\.[0-9a-f]{16}\.png$"))
            .ToList();
        if (atlas.Count != 1)
        {
            throw new InvalidOperationException($"Expected one sprite atlas in {TestEnvironment.PublishedSprites}, found {atlas.Count}.");
        }
        var header = new byte[24];
        using (var stream = File.OpenRead(atlas[0]))
        {
            stream.ReadExactly(header);
        }
        // The PNG signature, then the IHDR chunk: length and type (8 bytes), width and height (big-endian).
        if (!header.AsSpan(12, 4).SequenceEqual("IHDR"u8))
        {
            throw new InvalidDataException("The sprite atlas does not start with an IHDR chunk.");
        }
        var width = BinaryPrimitives.ReadInt32BigEndian(header.AsSpan(16));
        var height = BinaryPrimitives.ReadInt32BigEndian(header.AsSpan(20));
        return new(Path.GetFileName(atlas[0]), new FileInfo(atlas[0]).Length, width, height, 4L * width * height);
    }

    /// <summary>The sessions (1-based) after which the WebAssembly memory in <paramref name="values"/> was larger than after the one before.</summary>
    public static IReadOnlyList<int> GrowthSessions(IReadOnlyList<long> values)
        => [.. Enumerable.Range(1, Math.Max(values.Count - 1, 0)).Where(i => values[i] > values[i - 1]).Select(i => i + 1)];

    /// <summary>
    /// Whether the WebAssembly memory in <paramref name="values"/> may still be growing: it grew after a session in the last quarter of the run,
    /// so it has not been seen to stay flat. Growth before that is the runtime enlarging its heap in steps until it settles.
    /// </summary>
    /// <remarks>
    /// The runtime grows in steps of about 26 MiB, so a small retention per session shows only as a step every so many sessions. A run that
    /// levelled off rules out retention large enough to need a step within its last quarter, not every leak; a longer run
    /// (<see cref="TestEnvironment.PerfSessions"/>) narrows that.
    /// </remarks>
    /// <exception cref="ArgumentException">Fewer than two values, which show no trend.</exception>
    public static bool StillGrowing(IReadOnlyList<long> values)
    {
        if (values.Count < 2)
        {
            throw new ArgumentException("At least two values are needed to see a trend.", nameof(values));
        }
        var growth = GrowthSessions(values);
        return growth.Count > 0 && growth[^1] > values.Count * 3 / 4;
    }

    /// <summary>The report for the CI run summary.</summary>
    public static string ToMarkdown(MemoryBaselineResult result)
    {
        var invariant = CultureInfo.InvariantCulture;
        var env = result.Environment;
        var sb = new StringBuilder();
        sb.AppendLine("## Memory baseline (WEB-PERF-002)");
        sb.AppendLine();
        sb.AppendLine(invariant, $"PKHeX.Web {env.WebVersion} (`{env.SourceCommit}`), measured {env.MeasuredUtc} UTC on {env.Processor} ({env.ProcessorCount} logical CPUs, {(env.MemoryBytes / 1073741824.0).ToString("0.0", invariant)} GiB), {env.OperatingSystem} {env.Architecture}{(env.ContinuousIntegration is { } ci ? $", {ci}" : "")}.");
        sb.AppendLine();
        sb.AppendLine(invariant, $"The largest admitted save: a full ORAS save ({KiB(result.SaveBytes)}, {result.Entities} entities: every box slot and party position). Measured on loopback, with no threshold.");
        sb.AppendLine();
        sb.AppendLine("**WebAssembly memory** is the .NET runtime's memory (managed heap and runtime). It only grows, so each figure is the peak so far. **JS heap** is the page's JavaScript heap after a forced collection (Chromium only). A downloaded Blob is held by the browser process and is in neither.");
        sb.AppendLine();
        sb.AppendLine("### One session, then files at the read limit");
        sb.AppendLine();
        sb.AppendLine("The two refusals come after the repeated sessions below, so their high-water mark cannot hide growth in those sessions.");
        sb.AppendLine();
        var engines = result.Engines;
        sb.AppendLine("| Step | " + string.Join(" | ", engines.Select(e => $"{e.Engine} {e.BrowserVersion}")) + " |");
        sb.AppendLine("|---|" + string.Concat(engines.Select(_ => "---:|")));
        for (var i = 0; i < engines[0].Stages.Count; i++)
        {
            sb.AppendLine($"| {engines[0].Stages[i].Stage} | " + string.Join(" | ", engines.Select(e => Cell(e.Stages[i]))) + " |");
        }
        sb.AppendLine();

        var sessionCount = engines[0].Cycles.Count;
        sb.AppendLine(invariant, $"### {sessionCount} sessions in one page");
        sb.AppendLine();
        sb.AppendLine(invariant, $"Each opens the save, edits, analyses and applies one box slot, downloads and closes; memory is read after each. The runtime enlarges its memory in steps of about 26 MiB until it settles, so early steps are expected. WebAssembly memory is flagged when it still grew in the last quarter of the sessions; a level run rules out retention large enough to need a step there, not every leak. The JS heap moves a little either way after each collection, so it is shown, not flagged.");
        sb.AppendLine();
        sb.AppendLine("| Engine | After session 1 | After session " + sessionCount.ToString(invariant) + " | Grew after sessions | |");
        sb.AppendLine("|---|---:|---:|---|---|");
        foreach (var engine in engines)
        {
            var linear = engine.Cycles.Select(c => c.LinearBytes).ToList();
            var growth = GrowthSessions(linear);
            var flag = StillGrowing(linear) ? "**still growing**" : "levelled off";
            sb.AppendLine($"| {engine.Engine} WebAssembly | {MiB(linear[0])} | {MiB(linear[^1])} | {(growth.Count == 0 ? "none" : string.Join(", ", growth))} | {flag} |");
            if (engine.Cycles.All(c => c.JsHeapBytes is not null))
            {
                var js = engine.Cycles.Select(c => c.JsHeapBytes!.Value).ToList();
                var perSession = (js[^1] - js[0]) / (double)(js.Count - 1) / 1024;
                sb.AppendLine(invariant, $"| {engine.Engine} JS heap | {MiB(js[0])} | {MiB(js[^1])} | {perSession:0.0} KiB per session on average | |");
            }
        }
        sb.AppendLine();

        sb.AppendLine("### Where the memory goes (native .NET, same Web code)");
        sb.AppendLine();
        sb.AppendLine("Managed bytes each step allocates on desktop .NET, after one unrecorded run, and how many copies of its input file that is. Garbage counts too: it stays in WebAssembly memory until a collection, so it drives the peak.");
        sb.AppendLine();
        sb.AppendLine("| Step | Input | Allocated | Copies of the input |");
        sb.AppendLine("|---|---:|---:|---:|");
        foreach (var step in result.Native)
        {
            sb.AppendLine(invariant, $"| {step.Stage} | {Size(step.InputBytes)} | {Size(step.Bytes)} | {(double)step.Bytes / step.InputBytes:0.0} |");
        }
        sb.AppendLine();

        sb.AppendLine("### Sprite atlas");
        sb.AppendLine();
        sb.AppendLine(result.Atlas is { } atlas
            ? $"`{atlas.File}`: {MiB(atlas.Bytes)} on disk, {atlas.Width.ToString(invariant)} × {atlas.Height.ToString(invariant)} px, so about {MiB(atlas.DecodedBytes)} once decoded (RGBA). Computed from the header, not measured in a browser; only the publish made with sprites has it."
            : "Not measured: no publish made with sprites was given.");
        return sb.ToString();
    }

    /// <summary>The full result as indented JSON, for comparing runs.</summary>
    public static string ToJson(MemoryBaselineResult result) => JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true });

    /// <summary>Reads a result written by <see cref="ToJson"/>.</summary>
    public static MemoryBaselineResult FromJson(string json) => JsonSerializer.Deserialize<MemoryBaselineResult>(json)
        ?? throw new InvalidDataException("The memory baseline JSON is empty.");

    private static string Cell(MemoryStage stage) => stage.JsHeapBytes is { } js ? $"{MiB(stage.LinearBytes)} (JS {MiB(js)})" : MiB(stage.LinearBytes);

    private static string MiB(long bytes) => (bytes / 1048576.0).ToString("0.0", CultureInfo.InvariantCulture) + " MiB";

    /// <summary>KiB below 1 MiB, so small steps do not read as nothing.</summary>
    private static string Size(long bytes) => bytes < 1048576 ? KiB(bytes) : MiB(bytes);

    private static string KiB(long bytes) => (bytes / 1024.0).ToString("0", CultureInfo.InvariantCulture) + " KiB";
}
