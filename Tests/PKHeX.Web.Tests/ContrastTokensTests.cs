using System.Globalization;
using System.Text.RegularExpressions;
using FluentAssertions;
using Xunit;

namespace PKHeX.Web.Tests;

/// <summary>
/// The stylesheet's colour tokens meet WCAG 2.2 AA in both colour schemes (WEB-A11Y-003): text at least 4.5:1 against the page background,
/// and borders, focus rings and the selection mark at least 3:1. Every colour on the page comes from the tokens, so checking them checks
/// the page; axe checks the rendered result in the browser as well.
/// </summary>
[Trait(TestCategory.Name, TestCategory.Unit)]
public sealed partial class ContrastTokensTests
{
    /// <summary>Tokens used for text, including the legality and finding colours.</summary>
    private static readonly string[] TextTokens = ["--fg", "--muted", "--focus", "--valid", "--invalid", "--warning"];

    /// <summary>Tokens used only for non-text marks: borders, the focus ring and the selected slot's outline.</summary>
    private static readonly string[] MarkTokens = ["--border", "--selected"];

    /// <summary>Tokens used for text on a button's --control background.</summary>
    private static readonly string[] ControlTextTokens = ["--fg", "--muted"];

    private static string Stylesheet => File.ReadAllText(Path.Combine(SaveFixtures.RepositoryRoot, "PKHeX.Web", "wwwroot", "app.css"));

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void TokensMeetTheContrastMinimums(bool dark)
    {
        var tokens = Tokens(dark);
        var background = tokens["--bg"];
        foreach (var token in TextTokens)
        {
            Ratio(tokens[token], background).Should().BeGreaterThanOrEqualTo(4.5, $"{token} is text ({(dark ? "dark" : "light")})");
        }
        foreach (var token in MarkTokens)
        {
            Ratio(tokens[token], background).Should().BeGreaterThanOrEqualTo(3, $"{token} marks a control or state ({(dark ? "dark" : "light")})");
        }
        // Buttons are drawn on --control: their text, and the muted text of a disabled one, and their border.
        foreach (var token in ControlTextTokens)
        {
            Ratio(tokens[token], tokens["--control"]).Should().BeGreaterThanOrEqualTo(4.5, $"{token} is text on a button ({(dark ? "dark" : "light")})");
        }
        Ratio(tokens["--border"], tokens["--control"]).Should().BeGreaterThanOrEqualTo(3, $"--border outlines a button ({(dark ? "dark" : "light")})");
    }

    [Fact]
    public void BothSchemesDefineTheSameTokens() => Tokens(true).Keys.Should().BeEquivalentTo(Tokens(false).Keys);

    [Fact]
    public void NoColourIsWrittenOutsideTheTokens()
    {
        var outside = TokenBlock().Replace(Comment().Replace(Stylesheet, ""), "");
        Hex().Matches(outside).Select(m => m.Value).Should().BeEmpty("every colour comes from a token, so the contrast checks cover it");
        outside.Should().NotContain("Canvas", "the system colours vary by browser, so their contrast is not known");
    }

    [Fact]
    public void TheRatioMatchesTheWcagExamples()
    {
        Ratio("#000000", "#ffffff").Should().BeApproximately(21, 0.01);
        Ratio("#767676", "#ffffff").Should().BeApproximately(4.54, 0.01);
        Ratio("#ffffff", "#ffffff").Should().Be(1);
    }

    /// <summary>The custom properties of the light <c>:root</c> block, or of the one inside <c>prefers-color-scheme: dark</c>.</summary>
    private static Dictionary<string, string> Tokens(bool dark)
    {
        var blocks = TokenBlock().Matches(Stylesheet).Select(m => m.Value).ToArray();
        blocks.Should().HaveCount(2, "one light and one dark token block");
        var block = blocks.Single(b => b.Contains("prefers-color-scheme: dark", StringComparison.Ordinal) == dark);
        return Declaration().Matches(block).ToDictionary(m => m.Groups[1].Value, m => m.Groups[2].Value);
    }

    /// <summary>The WCAG contrast ratio of two <c>#rrggbb</c> colours.</summary>
    private static double Ratio(string a, string b)
    {
        var (x, y) = (Luminance(a), Luminance(b));
        return (Math.Max(x, y) + 0.05) / (Math.Min(x, y) + 0.05);
    }

    /// <summary>The WCAG relative luminance of a <c>#rrggbb</c> colour.</summary>
    private static double Luminance(string hex)
    {
        double Channel(int start)
        {
            var c = int.Parse(hex.AsSpan(start, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture) / 255.0;
            return c <= 0.04045 ? c / 12.92 : Math.Pow((c + 0.055) / 1.055, 2.4);
        }
        return (0.2126 * Channel(1)) + (0.7152 * Channel(3)) + (0.0722 * Channel(5));
    }

    /// <summary>A <c>:root</c> block of tokens, with the media query around it when there is one.</summary>
    [GeneratedRegex(@"(@media \(prefers-color-scheme: dark\) \{\s*)?:root \{[^}]*\}(\s*\})?")]
    private static partial Regex TokenBlock();

    [GeneratedRegex(@"(--[a-z-]+):\s*(#[0-9a-f]{6});")]
    private static partial Regex Declaration();

    [GeneratedRegex(@"/\*.*?\*/", RegexOptions.Singleline)]
    private static partial Regex Comment();

    [GeneratedRegex(@"#[0-9a-fA-F]{3,8}\b")]
    private static partial Regex Hex();
}
