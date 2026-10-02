using Xunit;
using Xunit.Abstractions;

namespace PKHeX.Web.Tests;

/// <summary>
/// Records the legality analysis timing of the published app (WEB-PERF-004) to <see cref="TestEnvironment.PerfReport"/>. See <see cref="LegalityTiming"/>.
/// It has no timing threshold: it fails only if an analysis cannot be measured or its verdict differs from native Core.
/// </summary>
[Collection(BootBaselineCollection.Name)]
[Trait(TestCategory.Name, TestCategory.Perf)]
public sealed class LegalityTimingTests(ITestOutputHelper output)
{
    /// <summary>Markdown report file name inside <see cref="TestEnvironment.PerfReport"/>.</summary>
    public const string MarkdownFile = "legality-timing.md";

    /// <summary>JSON report file name inside <see cref="TestEnvironment.PerfReport"/>.</summary>
    public const string JsonFile = "legality-timing.json";

    [TierFact(TestCategory.Perf)]
    public async Task RecordsLegalityTiming()
    {
        var published = Path.GetFullPath(TestEnvironment.Required(TestEnvironment.Published));
        var reportDirectory = Path.GetFullPath(TestEnvironment.Required(TestEnvironment.PerfReport));

        var result = await LegalityTiming.MeasureAsync(published);

        var markdown = LegalityTiming.ToMarkdown(result);
        Directory.CreateDirectory(reportDirectory);
        await File.WriteAllTextAsync(Path.Combine(reportDirectory, MarkdownFile), markdown);
        await File.WriteAllTextAsync(Path.Combine(reportDirectory, JsonFile), LegalityTiming.ToJson(result));
        output.WriteLine(markdown);
    }
}
