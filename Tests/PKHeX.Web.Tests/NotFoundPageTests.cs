using FluentAssertions;
using System.Text.RegularExpressions;
using Xunit;

namespace PKHeX.Web.Tests;

/// <summary>
/// The not-found page a host serves at any missing path: nothing in it may run, load or point anywhere, so it is the same at every depth and under a subpath.
/// </summary>
[Trait(TestCategory.Name, TestCategory.Unit)]
public sealed partial class NotFoundPageTests
{
    private static readonly string Html = File.ReadAllText(Path.Combine(SaveFixtures.RepositoryRoot, "PKHeX.Web", "wwwroot", StaticHost.NotFoundPage));

    [Fact]
    public void ItRunsAndLoadsNothing()
    {
        Html.Should().StartWith("<!DOCTYPE html>").And.Contain("<html lang=\"en\">").And.Contain("<title>");
        Html.Should().NotContainAny(["<script", "<link", "<style", "<img", "<iframe", "<base", "href=", "src=", "style=", "javascript:"]);
        EventHandlerAttribute().IsMatch(Html).Should().BeFalse("no inline event handler attributes");
    }

    [Fact]
    public void ItSaysTheAddressIsNotPartOfTheApp()
        => Html.Should().Contain("<h1>Not found</h1>").And.Contain("This address is not part of PKHeX Web.");

    [GeneratedRegex(@"<[^>]*\son[a-z]+\s*=", RegexOptions.IgnoreCase)]
    private static partial Regex EventHandlerAttribute();
}
