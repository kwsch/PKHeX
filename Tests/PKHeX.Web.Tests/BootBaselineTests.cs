using System.Globalization;
using Xunit;
using Xunit.Abstractions;

namespace PKHeX.Web.Tests;

/// <summary>
/// Runs the boot baseline on its own: xUnit starts a collection with parallelization disabled only after the parallel ones have finished,
/// so no other browser test competes for CPU while boots are timed, even when tiers are combined in one run.
/// </summary>
[CollectionDefinition(Name, DisableParallelization = true)]
public sealed class BootBaselineCollection
{
    /// <summary>Collection name used by <c>[Collection(BootBaselineCollection.Name)]</c>.</summary>
    public const string Name = "BootBaseline";
}

/// <summary>
/// Records the boot baseline of the published app (WEB-PERF-001) to <see cref="TestEnvironment.PerfReport"/>. See <see cref="BootBaseline"/>.
/// </summary>
[Collection(BootBaselineCollection.Name)]
[Trait(TestCategory.Name, TestCategory.Perf)]
public sealed class BootBaselineTests(ITestOutputHelper output)
{
    /// <summary>Markdown report file name inside <see cref="TestEnvironment.PerfReport"/>.</summary>
    public const string MarkdownFile = "boot-baseline.md";

    /// <summary>JSON report file name inside <see cref="TestEnvironment.PerfReport"/>.</summary>
    public const string JsonFile = "boot-baseline.json";

    [TierFact(TestCategory.Perf)]
    public async Task RecordsBootBaseline()
    {
        var published = Path.GetFullPath(TestEnvironment.Required(TestEnvironment.Published));
        if (!File.Exists(Path.Combine(published, "_framework", "blazor.webassembly.js")))
        {
            throw new InvalidOperationException($"Point {TestEnvironment.Published} to the Release publish wwwroot.");
        }
        var reportDirectory = Path.GetFullPath(TestEnvironment.Required(TestEnvironment.PerfReport));
        var runs = ParseRuns(TestEnvironment.Optional(TestEnvironment.PerfRuns));

        var channel = PerfBrowser.ParseChannel(TestEnvironment.Optional(TestEnvironment.PerfChannel));

        var result = await BootBaseline.MeasureAsync(published, runs, channel);

        var markdown = BootBaselineReport.ToMarkdown(result);
        Directory.CreateDirectory(reportDirectory);
        await File.WriteAllTextAsync(Path.Combine(reportDirectory, MarkdownFile), markdown);
        await File.WriteAllTextAsync(Path.Combine(reportDirectory, JsonFile), BootBaselineReport.ToJson(result));
        output.WriteLine(markdown);
    }

    /// <summary>Reads <see cref="TestEnvironment.PerfRuns"/>: a positive whole number, or <see cref="BootBaseline.DefaultRuns"/> when unset.</summary>
    internal static int ParseRuns(string? value)
    {
        if (value is null)
        {
            return BootBaseline.DefaultRuns;
        }
        if (!int.TryParse(value, NumberStyles.None, CultureInfo.InvariantCulture, out var runs) || runs < 1)
        {
            throw new InvalidOperationException($"{TestEnvironment.PerfRuns} must be a positive whole number; it is '{value}'.");
        }
        return runs;
    }
}
