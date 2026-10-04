using PKHeX.Web.Components;
using PKHeX.Web.Services;
using PKHeX.Web.State;
using Xunit;
using Xunit.Abstractions;

namespace PKHeX.Web.Tests;

/// <summary>
/// Records the legality analysis timing of the published app (WEB-PERF-004) over every slot the private XY and ORAS saves let the user open.
/// It complements <see cref="LegalityTimingTests"/>, whose synthetic corpus is small, with the
/// entities of real play. See <see cref="LegalityTiming"/>.
/// </summary>
/// <remarks>
/// It is a <see cref="TestCategory.RealSave"/> test, because it reads the private saves, and runs in the non-parallel collection so no other
/// browser test competes with its timings. Nearly a thousand analyses take about six minutes per engine, so it runs in Chromium only (or the
/// <see cref="TestEnvironment.PerfChannel"/> browser); the synthetic corpus covers every engine. The report goes to the test output, and
/// also to <see cref="TestEnvironment.PerfReport"/> when that is set, so a RealSave run needs nothing more.
/// The report names slot positions and verdicts only: never a species, nickname or trainer value.
/// It fails only if an analysis cannot be measured or its verdict differs from native Core; it has no timing threshold.
/// </remarks>
[Collection(BootBaselineCollection.Name)]
[Trait(TestCategory.Name, TestCategory.RealSave)]
public sealed class RealSaveLegalityTimingTests(ITestOutputHelper output)
{
    /// <summary>Markdown report file name inside <see cref="TestEnvironment.PerfReport"/>.</summary>
    public const string MarkdownFile = "legality-timing-realsave.md";

    /// <summary>JSON report file name inside <see cref="TestEnvironment.PerfReport"/>.</summary>
    public const string JsonFile = "legality-timing-realsave.json";

    [TierFact(TestCategory.RealSave)]
    public async Task RecordsLegalityTimingOverTheRealSaves()
    {
        var published = Path.GetFullPath(TestEnvironment.Required(TestEnvironment.Published));
        var reportDirectory = TestEnvironment.Optional(TestEnvironment.PerfReport);
        var channel = PerfBrowser.ParseChannel(TestEnvironment.Optional(TestEnvironment.PerfChannel));
        var saves = RealSaves.Families.Select(Open).ToList();

        var corpus = $"Every slot the private saves let the user open ({string.Join(" and ", saves.Select(s => $"{s.Slots.Count} in {s.Family}"))}), analysed once each";
        var result = await LegalityTiming.MeasureAsync(published, channel, saves, corpus, ["chromium"]);

        var markdown = LegalityTiming.ToMarkdown(result);
        output.WriteLine(markdown);
        if (reportDirectory is not null)
        {
            var directory = Path.GetFullPath(reportDirectory);
            Directory.CreateDirectory(directory);
            await File.WriteAllTextAsync(Path.Combine(directory, MarkdownFile), markdown);
            await File.WriteAllTextAsync(Path.Combine(directory, JsonFile), LegalityTiming.ToJson(result));
        }
    }

    /// <summary>Opens the private save of <paramref name="family"/> natively and lists every party position and box slot the app would open.</summary>
    private static TimedSave Open(string family)
    {
        var fixture = RealSaves.Read(family);
        var session = SaveFixtures.Open(fixture.Bytes);
        var slots = StorageView.Party(session)
            .Concat(Enumerable.Range(0, StorageView.BoxCount(session)).SelectMany(box => StorageView.Box(session, box).Slots))
            .Where(s => s.CanOpen)
            .Select(s => (SlotText.Position(s.Ref), s.Ref))
            .ToList();
        if (slots.Count == 0)
        {
            throw new InvalidOperationException($"The {family} fixture has no slot to analyse.");
        }
        return new TimedSave(family, fixture.Bytes, session, slots);
    }
}
