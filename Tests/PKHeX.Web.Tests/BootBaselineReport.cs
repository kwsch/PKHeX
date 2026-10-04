using System.Globalization;
using System.Text;
using System.Text.Json;

namespace PKHeX.Web.Tests;

/// <summary>One measured boot of the published app.</summary>
/// <param name="ShellReadyMs">Milliseconds from navigation start until the file input existed and was enabled (the usable shell).</param>
/// <param name="DomContentLoadedMs">Milliseconds from navigation start until <c>DOMContentLoaded</c> handlers finished.</param>
/// <param name="Requests">Responses the host sent for this boot.</param>
/// <param name="NotModified">How many of <paramref name="Requests"/> were 304 revalidations.</param>
/// <param name="BodyBytes">Response body bytes the host wrote, after content coding; headers are not counted.</param>
/// <param name="BytesByEncoding">Body bytes per content coding: <c>br</c>, <c>gzip</c> or <c>identity</c>.</param>
internal sealed record BootSample(double ShellReadyMs, double DomContentLoadedMs, int Requests, int NotModified, long BodyBytes, IReadOnlyDictionary<string, long> BytesByEncoding);

/// <summary>Cold and warm samples of one browser engine on one network profile.</summary>
/// <param name="Engine">Playwright engine name.</param>
/// <param name="BrowserVersion">Version reported by the browser.</param>
/// <param name="Throttled">Whether the <see cref="BootBaseline.ThrottledProfile"/> was applied; otherwise unthrottled loopback.</param>
/// <param name="Cold">Boots in a new browser process on an empty profile.</param>
/// <param name="Warm">Boots in a relaunched browser process on the profile its cold boot left behind.</param>
internal sealed record BootConfiguration(string Engine, string BrowserVersion, bool Throttled, IReadOnlyList<BootSample> Cold, IReadOnlyList<BootSample> Warm);

/// <summary>A published file a cold boot downloaded, with the sizes of its uncompressed asset and precompressed siblings.</summary>
internal sealed record BootSetFile(string Path, long Raw, long? Brotli, long? Gzip);

/// <summary>Resources embedded in PKHeX.Core under one folder of <c>Resources/</c>: how many, and their raw size.</summary>
internal sealed record CoreResourceGroup(string Group, int Count, long Bytes);

/// <summary>Where and on what the baseline was measured.</summary>
/// <param name="MemoryBytes">Memory available to .NET on the machine (<see cref="GCMemoryInfo.TotalAvailableMemoryBytes"/>), which honours container limits.</param>
/// <param name="WebVersion">Version the published app reports.</param>
/// <param name="SourceCommit">Commit the published app reports.</param>
internal sealed record BootEnvironment(string MeasuredUtc, string OperatingSystem, string Architecture, string Processor, int ProcessorCount, long MemoryBytes, string Runtime, string Playwright, string WebVersion, string SourceCommit, string? ContinuousIntegration);

/// <summary>A complete boot baseline: environment, samples per configuration, the boot set and the embedded-resource cost of PKHeX.Core.</summary>
internal sealed record BootBaselineResult(BootEnvironment Environment, int Runs, IReadOnlyList<BootConfiguration> Configurations, IReadOnlyList<BootSetFile> BootSet, IReadOnlyList<CoreResourceGroup> CoreResources);

/// <summary>
/// Summarises a <see cref="BootBaselineResult"/> as Markdown (for the CI run summary) and JSON (for comparing runs).
/// It reports numbers only: the targets from <c>PKHeX.Web.md</c> are shown next to them and are not enforced.
/// </summary>
internal static class BootBaselineReport
{
    /// <summary>Engineering target for a cold usable shell on the throttled profile (<c>PKHeX.Web.md</c>, performance section).</summary>
    public const double ColdTargetMs = 5000;

    /// <summary>Engineering target for a cached usable shell.</summary>
    public const double WarmTargetMs = 2000;

    /// <summary>Largest boot-set files listed in the Markdown report.</summary>
    public const int LargestFiles = 10;

    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };

    /// <summary>Median of <paramref name="values"/>: the middle value, or the mean of the two middle values for an even count.</summary>
    public static double Median(IEnumerable<double> values)
    {
        var sorted = values.Order().ToArray();
        if (sorted.Length == 0)
        {
            throw new ArgumentException("No values to summarise.", nameof(values));
        }
        var middle = sorted.Length / 2;
        return sorted.Length % 2 == 1 ? sorted[middle] : (sorted[middle - 1] + sorted[middle]) / 2;
    }

    /// <summary>Serialises <paramref name="result"/> as indented JSON.</summary>
    public static string ToJson(BootBaselineResult result) => JsonSerializer.Serialize(result, JsonOptions);

    /// <summary>Reads a result written by <see cref="ToJson"/>.</summary>
    public static BootBaselineResult FromJson(string json) => JsonSerializer.Deserialize<BootBaselineResult>(json, JsonOptions)
        ?? throw new InvalidDataException("The boot baseline JSON is empty.");

    /// <summary>Renders <paramref name="result"/> as a Markdown report.</summary>
    public static string ToMarkdown(BootBaselineResult result)
    {
        var env = result.Environment;
        var sb = new StringBuilder();
        sb.AppendLine("## PKHeX.Web boot baseline");
        sb.AppendLine();
        sb.AppendLine($"Measured numbers only; nothing here passes or fails. Targets from `PKHeX.Web.md` are engineering targets, not claims: a usable shell within {Ms(ColdTargetMs)} ms cold on {BootBaseline.ThrottledProfile} and within {Ms(WarmTargetMs)} ms cached, on a named reference desktop. A shared CI runner is not that desktop.");
        sb.AppendLine();
        sb.AppendLine("| | |");
        sb.AppendLine("|---|---|");
        sb.AppendLine($"| Measured (UTC) | {env.MeasuredUtc} |");
        sb.AppendLine($"| Build | PKHeX.Web {env.WebVersion} · `{env.SourceCommit}` |");
        sb.AppendLine($"| Machine | {env.Processor}, {env.ProcessorCount} logical CPUs, {GiB(env.MemoryBytes)} memory; {env.OperatingSystem} ({env.Architecture}) |");
        if (env.ContinuousIntegration is not null)
        {
            sb.AppendLine($"| CI | {env.ContinuousIntegration} |");
        }
        sb.AppendLine($"| Test runtime | {env.Runtime}, Playwright {env.Playwright} |");
        sb.AppendLine($"| Browsers (headless; Playwright builds, or the installed browser when named by its channel, e.g. `chrome`) | {string.Join(", ", result.Configurations.DistinctBy(c => c.Engine).Select(c => $"{c.Engine} {c.BrowserVersion}"))} |");
        sb.AppendLine();

        sb.AppendLine($"### Usable shell (median, min–max of {result.Runs} runs)");
        sb.AppendLine();
        sb.AppendLine("Cold: a new browser process on an empty profile. Warm: the browser is closed and relaunched on that profile, like a returning visit, so only what it stored there (its HTTP cache) carries over. Each configuration first boots once unrecorded. Transfer is the response bodies the host sent, precompressed where the browser accepted it. Throttling adds its latency to each request rather than emulating TCP round trips.");
        sb.AppendLine();
        sb.AppendLine("| Engine | Network | Boot | Shell ready (ms) | DOMContentLoaded (ms) | Requests (304) | Transferred | Target (ms) |");
        sb.AppendLine("|---|---|---|---:|---:|---:|---:|---:|");
        foreach (var configuration in result.Configurations)
        {
            AppendRow(sb, configuration, "cold", configuration.Cold, ColdTargetMs);
            AppendRow(sb, configuration, "warm", configuration.Warm, WarmTargetMs);
        }
        sb.AppendLine();
        foreach (var configuration in result.Configurations.Where(ReusedNothingWhenWarm))
        {
            var network = configuration.Throttled ? BootBaseline.ThrottledProfile : "loopback";
            sb.AppendLine($"- {configuration.Engine} ({network}): the warm boot reused nothing from the HTTP cache (no 304s, and at least 90% of the cold transfer), so its warm row measures a second download, not a cached start.");
        }
        if (result.Configurations.Any(ReusedNothingWhenWarm))
        {
            sb.AppendLine();
        }

        var raw = result.BootSet.Sum(f => f.Raw);
        var brotli = result.BootSet.Sum(f => f.Brotli ?? f.Raw);
        var gzip = result.BootSet.Sum(f => f.Gzip ?? f.Raw);
        sb.AppendLine($"### Boot set ({result.BootSet.Count} files a cold boot downloads)");
        sb.AppendLine();
        sb.AppendLine("Sizes on disk; a file without a precompressed copy counts at its raw size. `PKHeX.Web/tools/size-report.sh` covers the whole publish, including files a boot never fetches.");
        sb.AppendLine();
        sb.AppendLine("| | Raw | Brotli | gzip |");
        sb.AppendLine("|---|---:|---:|---:|");
        sb.AppendLine($"| All {result.BootSet.Count} files | {MiB(raw)} | {MiB(brotli)} | {MiB(gzip)} |");
        foreach (var file in result.BootSet.OrderByDescending(f => f.Raw).ThenBy(f => f.Path, StringComparer.Ordinal).Take(LargestFiles))
        {
            sb.AppendLine($"| `{file.Path}` | {MiB(file.Raw)} | {MiB(file.Brotli)} | {MiB(file.Gzip)} |");
        }

        // The published PKHeX.Core file carries every embedded resource, so their share of it is the resource cost of each boot.
        var core = result.BootSet.FirstOrDefault(f => Path.GetFileName(f.Path).StartsWith("PKHeX.Core.", StringComparison.Ordinal) && f.Path.EndsWith(".wasm", StringComparison.Ordinal));
        var resourceTotal = result.CoreResources.Sum(g => g.Bytes);
        sb.AppendLine();
        sb.AppendLine("### PKHeX.Core embedded resources");
        sb.AppendLine();
        sb.AppendLine("Raw sizes of the data PKHeX.Core embeds (text, legality tables and so on), read from the PKHeX.Core the tests reference. Core loads them by name, so every boot downloads all of them inside the assembly.");
        sb.AppendLine();
        sb.AppendLine("| Group | Resources | Raw | Share of published PKHeX.Core |");
        sb.AppendLine("|---|---:|---:|---:|");
        foreach (var group in result.CoreResources)
        {
            sb.AppendLine($"| `{group.Group}` | {group.Count} | {MiB(group.Bytes)} | {Share(group.Bytes, core?.Raw)} |");
        }
        sb.AppendLine($"| All | {result.CoreResources.Sum(g => g.Count)} | {MiB(resourceTotal)} | {Share(resourceTotal, core?.Raw)} |");
        return sb.ToString();
    }

    /// <summary>Whether the warm boots made no revalidations and transferred about as much as the cold ones, so the cache was not used.</summary>
    public static bool ReusedNothingWhenWarm(BootConfiguration configuration)
        => Median(configuration.Warm.Select(s => (double)s.NotModified)) == 0
        && Median(configuration.Warm.Select(s => (double)s.BodyBytes)) >= 0.9 * Median(configuration.Cold.Select(s => (double)s.BodyBytes));

    private static void AppendRow(StringBuilder sb, BootConfiguration configuration, string boot, IReadOnlyList<BootSample> samples, double target)
    {
        var network = configuration.Throttled ? BootBaseline.ThrottledProfile : "loopback";
        // The targets are stated for the throttled profile only.
        var targetText = configuration.Throttled ? $"≤ {Ms(target)}" : "–";
        var encodings = string.Join("+", samples.SelectMany(s => s.BytesByEncoding).Where(p => p.Value > 0).Select(p => p.Key).Distinct().Order(StringComparer.Ordinal));
        var transferred = MiB((long)Median(samples.Select(s => (double)s.BodyBytes)));
        if (encodings.Length > 0)
        {
            transferred += $" ({encodings})";
        }
        sb.AppendLine($"| {configuration.Engine} | {network} | {boot} | {Spread(samples.Select(s => s.ShellReadyMs))} | {Spread(samples.Select(s => s.DomContentLoadedMs))} | {Ms(Median(samples.Select(s => (double)s.Requests)))} ({Ms(Median(samples.Select(s => (double)s.NotModified)))}) | {transferred} | {targetText} |");
    }

    /// <summary>"median (min–max)" in whole milliseconds.</summary>
    private static string Spread(IEnumerable<double> values)
    {
        var list = values.ToList();
        return $"{Ms(Median(list))} ({Ms(list.Min())}–{Ms(list.Max())})";
    }

    private static string Ms(double value) => Math.Round(value, MidpointRounding.AwayFromZero).ToString("0", CultureInfo.InvariantCulture);

    private static string Share(long part, long? whole) => whole is > 0 ? (100.0 * part / whole.Value).ToString("0", CultureInfo.InvariantCulture) + "%" : "-";

    private static string GiB(long bytes) => (bytes / 1073741824.0).ToString("0.0", CultureInfo.InvariantCulture) + " GiB";

    private static string MiB(long? bytes) => bytes is { } b ? (b / 1048576.0).ToString("0.00", CultureInfo.InvariantCulture) + " MiB" : "-";
}
