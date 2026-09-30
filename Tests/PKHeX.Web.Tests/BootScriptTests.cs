using System.Text.RegularExpressions;
using Xunit;

namespace PKHeX.Web.Tests;

/// <summary>
/// <c>boot.js</c> must parse in the old browsers it exists to turn away, so it keeps to ES2015 syntax.
/// </summary>
[Trait(TestCategory.Name, TestCategory.Unit)]
public sealed partial class BootScriptTests
{
    /// <summary>Syntax newer than ES2015 that a browser such as Safari 12 cannot parse, which would leave the page on its loading message.</summary>
    [Theory]
    [InlineData(@"\?\.(?!\d)", "optional chaining")]
    [InlineData(@"\?\?", "nullish coalescing")]
    [InlineData(@"\bcatch\s*\{", "optional catch binding")]
    [InlineData(@"\basync\b|\bawait\b", "async functions")]
    [InlineData(@"\*\*", "exponentiation")]
    [InlineData(@"\.\.\.", "spread or rest")]
    public void UsesOnlyES2015Syntax(string pattern, string feature)
    {
        var path = Path.Combine(SaveFixtures.RepositoryRoot, "PKHeX.Web", "wwwroot", "boot.js");
        var code = Comments().Replace(File.ReadAllText(path), "");
        Assert.False(Regex.IsMatch(code, pattern), $"boot.js uses {feature}, which older browsers cannot parse.");
    }

    /// <summary>Block and line comments, which may mention the forbidden syntax in prose.</summary>
    [GeneratedRegex(@"/\*[\s\S]*?\*/|//[^\n]*")]
    private static partial Regex Comments();
}
