using FluentAssertions;
using PKHeX.Web.Services;
using Xunit;

namespace PKHeX.Web.Tests;

/// <summary>
/// File name sanitising for display and download names.
/// </summary>
[Trait(TestCategory.Name, TestCategory.Unit)]
public sealed class FileNamingTests
{
    [Theory]
    [InlineData("main")]
    [InlineData("main.bak")]
    [InlineData("save.sav")]
    [InlineData("Pokémon Y (1)")]
    public void LeavesSafeNamesAndExtensionsUnchanged(string name)
    {
        FileNaming.Sanitize(name).Should().Be(name);
    }

    [Theory]
    [InlineData("a/b\\c.sav", "c.sav")]
    [InlineData("C:\\Users\\me\\main", "main")]
    [InlineData("../../main", "main")]
    public void KeepsOnlyTheLastPathSegment(string raw, string expected)
    {
        FileNaming.Sanitize(raw).Should().Be(expected);
    }

    [Theory]
    [InlineData("backup\u0007 copy.sav", "backup copy.sav")]
    [InlineData("line\nbreak", "linebreak")]
    [InlineData("evil\u202Evas.exe", "evilvas.exe")] // Right-to-left override would disguise the extension.
    [InlineData("\u200Bmain\uFEFF", "main")]
    [InlineData("a\u2028b\u2029c", "abc")]
    public void RemovesControlAndFormatCharacters(string raw, string expected)
    {
        FileNaming.Sanitize(raw).Should().Be(expected);
    }

    [Fact]
    public void RemovesUnpairedSurrogates()
    {
        // Built here rather than in InlineData, which cannot carry unpaired surrogates through test discovery.
        // Browsers replace them, so the downloaded name would differ from the one shown.
        FileNaming.Sanitize("\uD800abc\uDC00").Should().Be("abc");
        FileNaming.Sanitize("a\uD83D\uDE00b").Should().Be("a\uD83D\uDE00b");
    }

    [Fact]
    public void ReplacesReservedCharacters()
    {
        FileNaming.Sanitize("a<b>:c\"d|e?f*").Should().Be("a_b__c_d_e_f_");
    }

    [Theory]
    [InlineData(" main ", "main")]
    [InlineData("main. . ", "main")]
    [InlineData("main...", "main")]
    public void TrimsWhitespaceAndTrailingDots(string raw, string expected)
    {
        FileNaming.Sanitize(raw).Should().Be(expected);
    }

    [Theory]
    [InlineData("CON", "_CON")]
    [InlineData("nul.txt", "_nul.txt")]
    [InlineData("com1.sav", "_com1.sav")]
    [InlineData("CONIN$", "_CONIN$")]
    [InlineData("com\u00B9.sav", "_com\u00B9.sav")]
    [InlineData("CON .txt", "_CON .txt")]
    [InlineData("console", "console")]
    public void PrefixesWindowsDeviceNames(string raw, string expected)
    {
        FileNaming.Sanitize(raw).Should().Be(expected);
    }

    [Fact]
    public void CapsLengthAndKeepsShortExtension()
    {
        var result = FileNaming.Sanitize(new string('a', 200) + ".sav");
        result.Should().HaveLength(FileNaming.MaxLength);
        result.Should().EndWith(".sav");
    }

    [Fact]
    public void CapsLengthWithoutSplittingSurrogatePairs()
    {
        var result = FileNaming.Sanitize(new string('a', FileNaming.MaxLength - 5) + string.Concat(Enumerable.Repeat("😀", 10)) + ".sav");
        result.Length.Should().BeLessThanOrEqualTo(FileNaming.MaxLength);
        result.Should().EndWith(".sav");
        for (int i = 0; i < result.Length; i++)
        {
            if (char.IsHighSurrogate(result[i]))
            {
                char.IsLowSurrogate(result[i + 1]).Should().BeTrue("a surrogate pair must not be split");
            }
            else
            {
                char.IsLowSurrogate(result[i]).Should().BeFalse("a surrogate pair must not be split");
            }
        }
    }

    [Fact]
    public void DevicePrefixStaysWithinLength()
    {
        var result = FileNaming.Sanitize("CON." + new string('x', FileNaming.MaxLength - 4));
        result.Should().StartWith("_CON.");
        result.Length.Should().BeLessThanOrEqualTo(FileNaming.MaxLength);
    }

    [Fact]
    public void CutThatExposesDeviceNameIsPrefixed()
    {
        var result = FileNaming.Sanitize("CON" + new string(' ', 200) + "x");
        result.Should().Be("_CON");
    }

    [Theory]
    [InlineData(".sav", "sav")]
    [InlineData("..sav", "sav")]
    [InlineData(". .main", "main")]
    public void RemovesLeadingDotsThatBrowsersWouldStrip(string raw, string expected)
    {
        FileNaming.Sanitize(raw).Should().Be(expected);
    }

    [Fact]
    public void CapsLengthAfterRemovingLeadingDots()
    {
        FileNaming.Sanitize(new string('.', 200) + "x" + new string('y', 200) + ".sav").Should().Be("x" + new string('y', FileNaming.MaxLength - 5) + ".sav");
    }

    [Theory]
    [InlineData("main")]
    [InlineData("a\u00A0\u00A0.")]
    [InlineData("a .  . ")]
    [InlineData("CON.xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx")]
    [InlineData("CON                                                                                                                                                  x")]
    [InlineData("backup\u0007 \u202Ecopy.sav")]
    [InlineData("😀😀😀😀😀😀😀😀😀😀😀😀😀😀😀😀😀😀😀😀😀😀😀😀😀😀😀😀😀😀😀😀😀😀😀😀😀😀😀😀😀😀😀😀😀😀😀😀😀😀😀😀😀😀😀😀😀😀😀😀😀😀😀😀😀😀😀😀😀😀.sav")]
    public void SanitisingIsStable(string raw)
    {
        // Names are sanitised on read, on load and again on download, so a second pass must not change them.
        var once = FileNaming.Sanitize(raw);
        FileNaming.Sanitize(once).Should().Be(once);
        once.Length.Should().BeLessThanOrEqualTo(FileNaming.MaxLength);
    }

    [Fact]
    public void CapsLengthWithoutKeepingALongExtension()
    {
        var result = FileNaming.Sanitize("a." + new string('b', 200));
        result.Should().HaveLength(FileNaming.MaxLength);
        result.Should().StartWith("a.b");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("\u0007")]
    [InlineData("folder/")]
    [InlineData("...")]
    [InlineData("\u2028")]
    public void FallsBackWhenNothingUsableRemains(string? raw)
    {
        FileNaming.Sanitize(raw).Should().Be(FileNaming.DefaultSaveName);
        FileNaming.Sanitize(raw, "fallback").Should().Be("fallback");
    }

    private static readonly DateTime Stamp = new(2026, 10, 1, 14, 32, 5);

    [Theory]
    [InlineData("main", "main-modified-2026-10-01-143205")]
    [InlineData("backup.sav", "backup-modified-2026-10-01-143205.sav")]
    [InlineData("my save.dsv", "my save-modified-2026-10-01-143205.dsv")]
    [InlineData("folder/main", "main-modified-2026-10-01-143205")]
    [InlineData("a<b>.sav", "a_b_-modified-2026-10-01-143205.sav")]
    [InlineData("CON", "_CON-modified-2026-10-01-143205")]
    [InlineData("", "main-modified-2026-10-01-143205")]
    [InlineData("...", "main-modified-2026-10-01-143205")]
    [InlineData("main-modified-2025-01-02-030405", "main-modified-2026-10-01-143205")]
    [InlineData("backup-modified-2025-01-02-030405.sav", "backup-modified-2026-10-01-143205.sav")]
    [InlineData("main-modified-2025-01-02-030405-copy", "main-modified-2025-01-02-030405-copy-modified-2026-10-01-143205")]
    [InlineData("main-modified-today", "main-modified-today-modified-2026-10-01-143205")]
    public void EditedNameStampsTheStemAndKeepsTheExtension(string original, string expected)
    {
        FileNaming.EditedName(original, Stamp).Should().Be(expected);
    }

    [Theory]
    [InlineData("de-DE")]
    [InlineData("ar-SA")]
    [InlineData("hi-IN")]
    [InlineData("th-TH")]
    public void EditedNameUsesInvariantDigitsAndCalendar(string culture)
    {
        var previous = System.Globalization.CultureInfo.CurrentCulture;
        try
        {
            System.Globalization.CultureInfo.CurrentCulture = new System.Globalization.CultureInfo(culture);
            FileNaming.EditedName("main", Stamp).Should().Be("main-modified-2026-10-01-143205");
        }
        finally
        {
            System.Globalization.CultureInfo.CurrentCulture = previous;
        }
    }

    [Theory]
    [InlineData(".sav")]
    [InlineData("")]
    [InlineData(".abcdefghijklmno")]
    public void EditedNameKeepsTheStampAndExtensionWithinTheCap(string extension)
    {
        var original = FileNaming.Sanitize(new string('s', 200) + extension);
        var result = FileNaming.EditedName(original, Stamp);
        result.Length.Should().BeLessThanOrEqualTo(FileNaming.MaxLength);
        result.Should().EndWith("-modified-2026-10-01-143205" + extension);
        result.Should().StartWith("sss");
        FileNaming.Sanitize(result).Should().Be(result, "the download name is sanitised again");
    }

    [Fact]
    public void EditedNameNeverSplitsASurrogatePair()
    {
        var result = FileNaming.EditedName(string.Concat(Enumerable.Repeat("😀", 100)) + ".sav", Stamp);
        result.Length.Should().BeLessThanOrEqualTo(FileNaming.MaxLength);
        result.Should().EndWith("-modified-2026-10-01-143205.sav");
        result.Where(char.IsHighSurrogate).Count().Should().Be(result.Count(char.IsLowSurrogate));
        FileNaming.Sanitize(result).Should().Be(result);
    }
}
