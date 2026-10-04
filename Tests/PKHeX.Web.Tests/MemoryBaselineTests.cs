using System.Globalization;
using Xunit;
using Xunit.Abstractions;

namespace PKHeX.Web.Tests;

/// <summary>
/// Records the memory baseline of the published app (WEB-PERF-002) to <see cref="TestEnvironment.PerfReport"/>. See <see cref="MemoryBaseline"/>.
/// It has no threshold: it fails only if a step cannot be measured.
/// </summary>
[Collection(BootBaselineCollection.Name)]
[Trait(TestCategory.Name, TestCategory.Perf)]
public sealed class MemoryBaselineTests(ITestOutputHelper output)
{
    /// <summary>Markdown report file name inside <see cref="TestEnvironment.PerfReport"/>.</summary>
    public const string MarkdownFile = "memory-baseline.md";

    /// <summary>JSON report file name inside <see cref="TestEnvironment.PerfReport"/>.</summary>
    public const string JsonFile = "memory-baseline.json";

    [TierFact(TestCategory.Perf)]
    public async Task RecordsMemoryBaseline()
    {
        var published = Path.GetFullPath(TestEnvironment.Required(TestEnvironment.Published));
        var reportDirectory = Path.GetFullPath(TestEnvironment.Required(TestEnvironment.PerfReport));
        var channel = PerfBrowser.ParseChannel(TestEnvironment.Optional(TestEnvironment.PerfChannel));
        // Optional here: the atlas size is reported when the publish made with sprites is given, and said to be unmeasured otherwise.
        var sprites = TestEnvironment.Optional(TestEnvironment.PublishedSprites) is { } path ? Path.GetFullPath(path) : null;
        var sessions = ParseSessions(TestEnvironment.Optional(TestEnvironment.PerfSessions));

        var result = await MemoryBaseline.MeasureAsync(published, channel, sprites, sessions);

        var markdown = MemoryBaseline.ToMarkdown(result);
        Directory.CreateDirectory(reportDirectory);
        await File.WriteAllTextAsync(Path.Combine(reportDirectory, MarkdownFile), markdown);
        await File.WriteAllTextAsync(Path.Combine(reportDirectory, JsonFile), MemoryBaseline.ToJson(result));
        output.WriteLine(markdown);
    }

    /// <summary>Reads <see cref="TestEnvironment.PerfSessions"/>: a whole number of at least 2, or <see cref="MemoryBaseline.DefaultSessions"/> when unset.</summary>
    internal static int ParseSessions(string? value)
    {
        if (value is null)
        {
            return MemoryBaseline.DefaultSessions;
        }
        if (!int.TryParse(value, NumberStyles.None, CultureInfo.InvariantCulture, out var sessions) || sessions < 2)
        {
            throw new InvalidOperationException($"{TestEnvironment.PerfSessions} must be a whole number of at least 2; it is '{value}'.");
        }
        return sessions;
    }
}
