using Xunit;

namespace PKHeX.Web.Tests;

/// <summary>Summary statistics, rendering and settings of the boot baseline report; no browser is involved.</summary>
[Trait(TestCategory.Name, TestCategory.Unit)]
public sealed class BootBaselineReportTests
{
    [Theory]
    [InlineData(new[] { 3.0 }, 3.0)]
    [InlineData(new[] { 5.0, 1.0, 3.0 }, 3.0)]
    [InlineData(new[] { 4.0, 1.0, 3.0, 2.0 }, 2.5)]
    public void MedianIsTheMiddleValue(double[] values, double expected) => Assert.Equal(expected, BootBaselineReport.Median(values));

    [Fact]
    public void MedianOfNothingThrows() => Assert.Throws<ArgumentException>(() => BootBaselineReport.Median([]));

    [Fact]
    public void MarkdownShowsTimingsTargetsAndBootSet()
    {
        var markdown = BootBaselineReport.ToMarkdown(Sample());

        Assert.Contains("| chromium | 20 Mbps / 50 ms | cold | 2000 (1000–3000) | 500 (400–600) | 60 (0) | 4.00 MiB (br) | ≤ 5000 |", markdown);
        Assert.Contains("| chromium | 20 Mbps / 50 ms | warm | 800 (700–900) | 200 (100–300) | 60 (58) | 0.00 MiB | ≤ 2000 |", markdown);
        Assert.Contains("| firefox | loopback | cold |", markdown);
        Assert.Contains("| – |", markdown);
        Assert.Contains("chromium 140.0, firefox 142.0", markdown);
        Assert.Contains("| All 2 files | 18.00 MiB | 4.00 MiB | 5.00 MiB |", markdown);
        Assert.Contains("| `_framework/PKHeX.Core.abcdefghij.wasm` | 17.00 MiB | 3.00 MiB | 4.00 MiB |", markdown);
        Assert.Contains("| `index.html` | 1.00 MiB | - | - |", markdown);
        Assert.Contains("| CI | GitHub Actions |", markdown);
        Assert.Contains("| Machine | Test CPU, 8 logical CPUs, 16.0 GiB memory; Test OS (Arm64) |", markdown);
        Assert.Contains("| `text` | 100 | 8.50 MiB | 50% |", markdown);
        Assert.Contains("| `legality` | 40 | 5.00 MiB | 29% |", markdown);
        Assert.Contains("| All | 140 | 13.50 MiB | 79% |", markdown);
        Assert.DoesNotContain("NaN", markdown);
        Assert.DoesNotContain("reused nothing", markdown);
    }

    [Fact]
    public void MarkdownFlagsAWarmBootThatReusedNothing()
    {
        var sample = Sample();
        var cold = sample.Configurations[1].Cold;
        var uncached = sample with { Configurations = [sample.Configurations[0], sample.Configurations[1] with { Warm = cold }] };

        Assert.False(BootBaselineReport.ReusedNothingWhenWarm(sample.Configurations[0]));
        Assert.True(BootBaselineReport.ReusedNothingWhenWarm(uncached.Configurations[1]));
        Assert.Contains("- firefox (loopback): the warm boot reused nothing", BootBaselineReport.ToMarkdown(uncached));
    }

    [Fact]
    public void JsonRoundTrips()
    {
        var result = Sample();
        var copy = BootBaselineReport.FromJson(BootBaselineReport.ToJson(result));

        Assert.Equal(result.Environment, copy.Environment);
        Assert.Equal(result.Runs, copy.Runs);
        Assert.Equal(result.BootSet, copy.BootSet);
        Assert.Equal(result.CoreResources, copy.CoreResources);
        Assert.Equal(BootBaselineReport.ToMarkdown(result), BootBaselineReport.ToMarkdown(copy));
    }

    [Fact]
    public void CoreResourcesAreGroupedByResourceFolder()
    {
        var groups = BootBaseline.MeasureCoreResources();

        Assert.Equal(groups.Count, groups.Select(g => g.Group).Distinct().Count());
        foreach (var expected in new[] { "byte", "legality", "localize", "text" })
        {
            var group = Assert.Single(groups, g => g.Group == expected);
            Assert.True(group.Count > 0 && group.Bytes > 0, $"{expected} has no resources.");
        }
        Assert.Equal(groups.OrderByDescending(g => g.Bytes).Select(g => g.Group), groups.Select(g => g.Group));
    }

    [Theory]
    [InlineData(null, BootBaseline.DefaultRuns)]
    [InlineData("1", 1)]
    [InlineData("12", 12)]
    public void RunsDefaultOrParse(string? value, int expected) => Assert.Equal(expected, BootBaselineTests.ParseRuns(value));

    [Theory]
    [InlineData("0")]
    [InlineData("-1")]
    [InlineData("two")]
    [InlineData("1.5")]
    public void InvalidRunsFail(string value) => Assert.Throws<InvalidOperationException>(() => BootBaselineTests.ParseRuns(value));

    private static BootBaselineResult Sample()
    {
        const long MiB = 1024 * 1024;
        var br = new Dictionary<string, long> { ["br"] = 4 * MiB };
        var none = new Dictionary<string, long> { ["br"] = 0 };
        BootSample Cold(double ready, double dcl) => new(ready, dcl, 60, 0, 4 * MiB, br);
        BootSample Warm(double ready, double dcl) => new(ready, dcl, 60, 58, 2048, none);
        return new(
            new("2026-09-27 12:00", "Test OS", "Arm64", "Test CPU", 8, 16L * 1024 * MiB, ".NET 10.0.12", "1.63.0", "26.8.27", "0123456789abcdef", "GitHub Actions"),
            3,
            [
                new("chromium", "140.0", true, [Cold(1000, 400), Cold(2000, 500), Cold(3000, 600)], [Warm(700, 100), Warm(800, 200), Warm(900, 300)]),
                new("firefox", "142.0", false, [Cold(900, 300), Cold(950, 350), Cold(990, 390)], [Warm(400, 50), Warm(450, 60), Warm(500, 70)]),
            ],
            [
                new("index.html", 1 * MiB, null, null),
                new("_framework/PKHeX.Core.abcdefghij.wasm", 17 * MiB, 3 * MiB, 4 * MiB),
            ],
            [new("text", 100, 8 * MiB + MiB / 2), new("legality", 40, 5 * MiB)]);
    }
}
