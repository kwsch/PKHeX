using Xunit;
using Xunit.Abstractions;

namespace PKHeX.Web.Tests;

/// <summary>
/// Records the box navigation timing of the published app to <see cref="TestEnvironment.PerfReport"/>. See <see cref="BoxNavigationTiming"/>.
/// It has no timing threshold: it fails only if a step cannot be measured, shows the wrong box, or makes a request.
/// </summary>
[Collection(BootBaselineCollection.Name)]
[Trait(TestCategory.Name, TestCategory.Perf)]
public sealed class BoxNavigationTimingTests(ITestOutputHelper output)
{
    /// <summary>Markdown report file name inside <see cref="TestEnvironment.PerfReport"/>.</summary>
    public const string MarkdownFile = "box-navigation.md";

    /// <summary>JSON report file name inside <see cref="TestEnvironment.PerfReport"/>.</summary>
    public const string JsonFile = "box-navigation.json";

    [TierFact(TestCategory.Perf)]
    public async Task RecordsBoxNavigationTiming()
    {
        var published = Path.GetFullPath(TestEnvironment.Required(TestEnvironment.Published));
        var reportDirectory = Path.GetFullPath(TestEnvironment.Required(TestEnvironment.PerfReport));
        var channel = PerfBrowser.ParseChannel(TestEnvironment.Optional(TestEnvironment.PerfChannel));

        var result = await BoxNavigationTiming.MeasureAsync(published, channel);

        var markdown = BoxNavigationTiming.ToMarkdown(result);
        Directory.CreateDirectory(reportDirectory);
        await File.WriteAllTextAsync(Path.Combine(reportDirectory, MarkdownFile), markdown);
        await File.WriteAllTextAsync(Path.Combine(reportDirectory, JsonFile), BoxNavigationTiming.ToJson(result));
        output.WriteLine(markdown);
    }
}
