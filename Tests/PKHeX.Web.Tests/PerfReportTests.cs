using System.Buffers.Binary;
using PKHeX.Web.Services;
using Xunit;

namespace PKHeX.Web.Tests;

/// <summary>
/// The Perf tier's browser choice and the memory, box navigation and legality timing reports: their maths, rendering and settings. No browser is involved.
/// </summary>
[Trait(TestCategory.Name, TestCategory.Unit)]
public sealed class PerfReportTests
{
    private static readonly BootEnvironment Environment = new("2026-10-04 12:00", "Test OS", "Arm64", "Test CPU", 8, 16L << 30, ".NET 10", "1.63.0", "1.2.3", "abc123", null);

    [Theory]
    [InlineData(null, null)]
    [InlineData("chrome", "chrome")]
    [InlineData(" Chrome ", "chrome")]
    [InlineData("msedge", "msedge")]
    public void ChannelIsAKnownChromiumChannelOrNone(string? value, string? expected) => Assert.Equal(expected, PerfBrowser.ParseChannel(value));

    [Theory]
    [InlineData("chromium")]
    [InlineData("firefox")]
    [InlineData("chrome-stable")]
    public void AnUnknownChannelIsRefused(string value)
    {
        var refused = Assert.Throws<InvalidOperationException>(() => PerfBrowser.ParseChannel(value));
        Assert.Contains(TestEnvironment.PerfChannel, refused.Message);
    }

    [Fact]
    public void OnlyChromiumRunsFromTheChannel()
    {
        Assert.Equal("chrome", PerfBrowser.ChannelFor("chromium", "chrome"));
        Assert.Null(PerfBrowser.ChannelFor("firefox", "chrome"));
        Assert.Null(PerfBrowser.ChannelFor("webkit", "chrome"));
        Assert.Equal("chrome", PerfBrowser.Label("chromium", "chrome"));
        Assert.Equal("chromium", PerfBrowser.Label("chromium", null));
        Assert.Equal("webkit", PerfBrowser.Label("webkit", "chrome"));
    }

    [Theory]
    [InlineData(new long[] { 10, 10, 10 }, new int[0])]
    [InlineData(new long[] { 10, 20, 20, 30 }, new[] { 2, 4 })]
    [InlineData(new long[] { 30, 20, 20 }, new int[0])]
    public void GrowthSessionsAreTheSessionsAfterWhichMemoryGrew(long[] values, int[] expected) => Assert.Equal(expected, MemoryBaseline.GrowthSessions(values));

    [Theory]
    [InlineData(null, MemoryBaseline.DefaultSessions)]
    [InlineData("2", 2)]
    [InlineData("100", 100)]
    public void SessionsDefaultOrParse(string? value, int expected) => Assert.Equal(expected, MemoryBaselineTests.ParseSessions(value));

    [Theory]
    [InlineData("1")]
    [InlineData("0")]
    [InlineData("-5")]
    [InlineData("ten")]
    [InlineData("4.5")]
    public void SessionsBelowTwoOrNotWholeAreRefused(string value)
        => Assert.Contains(TestEnvironment.PerfSessions, Assert.Throws<InvalidOperationException>(() => MemoryBaselineTests.ParseSessions(value)).Message);

    [Fact]
    public void ATrendNeedsTwoValues() => Assert.Throws<ArgumentException>(() => MemoryBaseline.StillGrowing([1]));

    [Theory]
    [InlineData(new long[] { 1, 1, 1, 1, 1, 1, 1, 1 }, false)] // flat
    [InlineData(new long[] { 1, 2, 2, 2, 2, 2, 2, 2 }, false)] // one early step: the heap enlarged once
    [InlineData(new long[] { 1, 2, 2, 2, 3, 3, 3, 3 }, false)] // two steps, then flat for the last quarter: settled
    [InlineData(new long[] { 1, 1, 1, 1, 1, 2, 2, 2 }, false)] // the last step at session 6 of 8, before the last quarter
    [InlineData(new long[] { 1, 1, 1, 1, 1, 1, 1, 2 }, true)] // a step in the last quarter: not seen to settle
    [InlineData(new long[] { 1, 2, 2, 2, 2, 2, 3, 3 }, true)] // an early step and another in the last quarter
    public void ContinuedGrowthIsFlagged(long[] values, bool expected) => Assert.Equal(expected, MemoryBaseline.StillGrowing(values));

    [Fact]
    public void MemoryMarkdownShowsStagesTrendsCopiesAndAtlas()
    {
        var markdown = MemoryBaseline.ToMarkdown(MemorySample());

        Assert.Contains("| Step | chrome 154.0 | firefox 155.0 |", markdown);
        Assert.Contains("| Opened (930 entities) | 60.0 MiB (JS 2.0 MiB) | 70.0 MiB |", markdown);
        Assert.Contains("| chrome WebAssembly | 80.0 MiB | 80.0 MiB | none | levelled off |", markdown);
        Assert.Contains("| chrome JS heap | 2.0 MiB | 3.9 MiB | 100.0 KiB per session on average | |", markdown);
        Assert.Contains("| firefox WebAssembly | 80.0 MiB | 90.0 MiB | 10, 11, 20 | **still growing** |", markdown);
        Assert.DoesNotContain("firefox JS heap", markdown);
        Assert.Contains("| Open: copy, parse and round-trip check | 472 KiB | 2.0 MiB | 4.3 |", markdown);
        Assert.Contains("| Edit the nickname | 472 KiB | 2 KiB | 0.0 |", markdown);
        Assert.Contains("about 28.0 MiB once decoded", markdown);
        Assert.Contains("a full ORAS save (472 KiB, 930 entities", markdown);
        Assert.DoesNotContain("NaN", markdown);
    }

    [Fact]
    public void MemoryMarkdownSaysWhenTheAtlasWasNotMeasured()
        => Assert.Contains("Not measured: no publish made with sprites was given.", MemoryBaseline.ToMarkdown(MemorySample() with { Atlas = null }));

    [Fact]
    public void MemoryJsonRoundTrips()
    {
        var result = MemorySample();
        Assert.Equal(MemoryBaseline.ToMarkdown(result), MemoryBaseline.ToMarkdown(MemoryBaseline.FromJson(MemoryBaseline.ToJson(result))));
    }

    [Fact]
    public void NativeAccountingCountsEveryStepOfTheLargestSave()
    {
        var save = SaveFixtures.Full(true);
        var steps = MemoryBaseline.MeasureNative(save);

        Assert.Equal(["Open: copy, parse and round-trip check", "Open a box slot as a draft", "Edit the nickname", "Analyse legality", "Apply: staged clone, write and checks", "Export: write, reopen and checks", "Read a 16 MiB file (bounded read)", "Refuse it: copy and recognition attempt"], steps.Select(s => s.Stage));
        // Opening keeps the original, parses a copy and writes one to compare, so it holds at least three copies of the save; export writes and reopens it.
        Assert.True(steps[0].Bytes >= 3L * save.Length, "Opening allocated less than three copies of the save.");
        Assert.True(steps[5].Bytes >= 3L * save.Length, "Exporting allocated less than three copies of the save.");
        // The bounded read keeps the whole file at least once.
        Assert.True(steps[6].Bytes >= SaveLoader.MaxInputBytes, "The read allocated less than the file.");
    }

    [Fact]
    public void TheFullFixtureFillsEveryBoxSlotAndPartyPosition()
    {
        var session = SaveFixtures.Open(SaveFixtures.Full(true));

        Assert.All(StorageView.Party(session), s => Assert.True(s.CanOpen));
        for (var box = 0; box < StorageView.BoxCount(session); box++)
        {
            Assert.All(StorageView.Box(session, box).Slots, s => Assert.True(s.CanOpen));
        }
    }

    [Fact]
    public void AtlasSizeIsReadFromThePngHeader()
    {
        var root = Directory.CreateTempSubdirectory("pkhex-atlas-");
        try
        {
            var sprites = Directory.CreateDirectory(Path.Combine(root.FullName, "sprites"));
            var header = new byte[33];
            new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A }.CopyTo(header, 0);
            BinaryPrimitives.WriteInt32BigEndian(header.AsSpan(8), 13);
            "IHDR"u8.CopyTo(header.AsSpan(12));
            BinaryPrimitives.WriteInt32BigEndian(header.AsSpan(16), 2048);
            BinaryPrimitives.WriteInt32BigEndian(header.AsSpan(20), 3584);
            File.WriteAllBytes(Path.Combine(sprites.FullName, "pokemon.0123456789abcdef.png"), header);
            File.WriteAllBytes(Path.Combine(sprites.FullName, "manifest.json"), []);

            var atlas = MemoryBaseline.MeasureAtlas(root.FullName);

            Assert.Equal(new AtlasSize("pokemon.0123456789abcdef.png", 33, 2048, 3584, 4L * 2048 * 3584), atlas);
        }
        finally
        {
            root.Delete(true);
        }
    }

    [Fact]
    public void AnAtlasIsRequiredOnce()
    {
        var root = Directory.CreateTempSubdirectory("pkhex-atlas-");
        try
        {
            Directory.CreateDirectory(Path.Combine(root.FullName, "sprites"));
            Assert.Throws<InvalidOperationException>(() => MemoryBaseline.MeasureAtlas(root.FullName));
        }
        finally
        {
            root.Delete(true);
        }
    }

    [Fact]
    public void BoxNavigationMarkdownShowsPercentilesAgainstTheTarget()
    {
        var steps = Enumerable.Range(1, 20).Select(i => new BoxStepSample(0, 1, i * 10)).ToList();
        var markdown = BoxNavigationTiming.ToMarkdown(new(Environment, 31, [new("chrome", "154.0", steps)]));

        Assert.Contains("| chrome 154.0 | 100 ms | 190 ms | 200 ms | 10 | 20 |", markdown);
        Assert.Contains("A full ORAS save (31 full boxes)", markdown);
    }

    [Fact]
    public void LegalityMarkdownDescribesItsCorpus()
    {
        var first = new LegalitySample("Box 1, slot 1", "XY", "Valid", 300);
        var warm = new[] { new LegalitySample("Box 1, slot 2", "XY", "Invalid", 20), new LegalitySample("Party 1", "ORAS", "Valid", 250) };
        var markdown = LegalityTiming.ToMarkdown(new(Environment, "Every slot the private saves let the user open (2 in XY and 1 in ORAS), analysed once each", 3, [new("chrome", "154.0", first, warm)]));

        Assert.Contains("Every slot the private saves let the user open (2 in XY and 1 in ORAS), analysed once each, through the app's idle-delay path", markdown);
        Assert.Contains("| chrome 154.0 | 300 ms | 20 ms | 250 ms | 250 ms | 1 | 3 |", markdown);
        Assert.Contains("| chrome | ORAS | `Party 1` | Valid | 250 ms |", markdown);
    }

    private static MemoryBaselineResult MemorySample()
    {
        const long MiB = 1 << 20;
        MemoryStage[] Stages(long linear, long? js) => [new("Shell ready", 50 * MiB, js), new("Opened (930 entities)", linear, js)];
        // Flat WebAssembly memory, and a JS heap that ends 1.9 MiB up: 100 KiB after each of the 19 later sessions.
        var flat = Enumerable.Range(1, 20).Select(i => new MemoryStage($"{i}", 80 * MiB, (2 * MiB) + (i == 20 ? 1945600 : 0))).ToList();
        // 80 MiB, then steps after sessions 10, 11 and 20.
        var growing = Enumerable.Range(1, 20).Select(i => new MemoryStage($"{i}", (i < 10 ? 80 : i == 10 ? 81 : i == 20 ? 90 : 85) * MiB, null)).ToList();
        return new(
            Environment,
            483_328,
            930,
            [new("Open: copy, parse and round-trip check", 2 * MiB, 483_328), new("Edit the nickname", 2048, 483_328)],
            [new("chrome", "154.0", Stages(60 * MiB, 2 * MiB), flat), new("firefox", "155.0", Stages(70 * MiB, null), growing)],
            new("pokemon.0123456789abcdef.png", 1300000, 2048, 3584, 4L * 2048 * 3584));
    }
}
