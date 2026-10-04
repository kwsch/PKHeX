using Bunit;
using FluentAssertions;
using PKHeX.Core;
using PKHeX.Web.Components;
using PKHeX.Web.Services;
using PKHeX.Web.State;
using Xunit;

namespace PKHeX.Web.Tests;

/// <summary>
/// The legality panel: a status word with an icon, a summary, Core's findings with links, and Core's reports in a disclosure.
/// </summary>
[Trait(TestCategory.Name, TestCategory.Unit)]
public sealed class LegalityPanelTests : IDisposable
{
    private const string Markup = "<img id=\"injected\" src=\"x\">";

    private readonly BunitContext context = new();
    private readonly EditorDraft draft = SaveFixtures.Open(SaveFixtures.Synthetic(false)).Select(SaveFixtures.FirstBoxSlot);
    private readonly List<string> calls = [];

    public void Dispose() => context.Dispose();

    private LegalityResult Result(LegalityVerdict verdict, params LegalityFinding[] findings) =>
        new(LegalityTag.Of(draft), verdict, findings, "short " + Markup, "verbose " + Markup);

    private IRenderedComponent<LegalityPanel> Render(LegalityStatus status, LegalityResult? current = null, bool autoRun = true, bool draftValid = true,
        bool running = false) =>
        context.Render<LegalityPanel>(p => p
            .Add(c => c.Status, status)
            .Add(c => c.Running, running)
            .Add(c => c.Current, current)
            .Add(c => c.AutoRun, autoRun)
            .Add(c => c.DraftValid, draftValid)
            .Add(c => c.OnAnalyze, () => calls.Add("analyze"))
            .Add(c => c.OnShowSection, id => calls.Add(id)));

    [Theory]
    [InlineData(LegalityStatus.NotAnalyzed, "Not analyzed", "legality-notanalyzed")]
    [InlineData(LegalityStatus.Pending, "Pending", "legality-pending")]
    [InlineData(LegalityStatus.Stale, "Stale", "legality-stale")]
    [InlineData(LegalityStatus.Valid, "Valid", "legality-valid")]
    [InlineData(LegalityStatus.Invalid, "Invalid", "legality-invalid")]
    [InlineData(LegalityStatus.Unavailable, "Unavailable", "legality-unavailable")]
    public void EveryStatusHasItsWordAndIcon(LegalityStatus status, string word, string iconClass)
    {
        var verdict = status switch
        {
            LegalityStatus.Valid => LegalityVerdict.Valid,
            LegalityStatus.Invalid => LegalityVerdict.Invalid,
            _ => LegalityVerdict.Unavailable,
        };
        var current = status is LegalityStatus.Valid or LegalityStatus.Invalid or LegalityStatus.Unavailable ? Result(verdict) : null;
        var panel = Render(status, current);

        panel.Find("#legality-status").TextContent.Should().Be(word);
        // The status follows typing (stale, pending), so it is not announced itself; the announcement holds only a final verdict.
        panel.Find("#legality-status").HasAttribute("role").Should().BeFalse();
        var announcement = panel.Find("#legality-announce");
        announcement.GetAttribute("role").Should().Be("status");
        announcement.TextContent.Should().Be(current is null ? "" : $"Legality: {word}");
        var icon = panel.Find(".legality-icon");
        icon.ClassList.Should().Contain(iconClass);
        icon.GetAttribute("aria-hidden").Should().Be("true", "the word carries the meaning; the icon only reinforces it");
        panel.Find("#legality-summary").TextContent.Should().NotBeEmpty();
    }

    [Fact]
    public void FindingsShowSeverityTextAndASectionLink()
    {
        var current = Result(LegalityVerdict.Invalid,
            new(Severity.Invalid, CheckIdentifier.CurrentMove, "Move problem"),
            new(Severity.Invalid, CheckIdentifier.Memory, "Memory problem"),
            new(Severity.Fishy, CheckIdentifier.Ball, "Ball warning"));
        var panel = Render(LegalityStatus.Invalid, current);

        panel.Find("#legality-summary").TextContent.Should().Be("2 problems, 1 warning.");
        var items = panel.FindAll("#legality-findings li");
        items.Should().HaveCount(3);
        items[0].TextContent.Should().Contain("Move problem").And.Contain("Show in Moves");
        items[0].ClassList.Should().Contain("finding-invalid");
        items[2].ClassList.Should().Contain("finding-warning");
        items[1].QuerySelectorAll("a").Should().BeEmpty("the inspector does not show memories");
        items[2].TextContent.Should().Contain("Ball warning").And.Contain("Show in Origin and trainer");
        items[0].QuerySelector("a")!.GetAttribute("href").Should().Be("#inspect-moves");

        items[0].QuerySelector("a")!.Click();
        calls.Should().Equal("inspect-moves");
    }

    [Fact]
    public void WarningsAloneSayNoProblems() =>
        LegalityText.Counts(Result(LegalityVerdict.Valid, new LegalityFinding(Severity.Fishy, CheckIdentifier.Form, "w")))
            .Should().Be("No problems, 1 warning.");

    [Fact]
    public void AValidResultSaysNoProblemsAndKeepsTheReports()
    {
        var panel = Render(LegalityStatus.Valid, Result(LegalityVerdict.Valid));
        panel.Find("#legality-summary").TextContent.Should().Be("No problems found.");
        panel.FindAll("#legality-findings").Should().BeEmpty();
        panel.Find("#legality-details").HasAttribute("open").Should().BeFalse("the detail is collapsed until asked for");
        panel.Find("#legality-report").TextContent.Should().Be("short " + Markup);
        panel.Find("#legality-report-verbose").TextContent.Should().Be("verbose " + Markup);
    }

    [Theory]
    [InlineData(LegalityStatus.Stale)]
    [InlineData(LegalityStatus.Pending)]
    [InlineData(LegalityStatus.NotAnalyzed)]
    public void NothingOutOfDateIsListed(LegalityStatus status)
    {
        // The workspace passes no current result unless it matches the draft; the panel shows no findings or reports without one.
        var panel = Render(status);
        panel.FindAll("#legality-findings").Should().BeEmpty();
        panel.FindAll("#legality-details").Should().BeEmpty();
    }

    [Fact]
    public void UnavailableNeverReadsAsLegal()
    {
        var panel = Render(LegalityStatus.Unavailable, LegalityResult.Unavailable(LegalityTag.Of(draft)));
        panel.Find("#legality-summary").TextContent.Should().Contain("could not complete").And.Contain("does not mean the Pokémon is legal");
        panel.FindAll("#legality-details").Should().BeEmpty();
    }

    [Fact]
    public void StaleSaysHowItWillBeRefreshed()
    {
        Render(LegalityStatus.Stale).Find("#legality-summary").TextContent.Should().Contain("once you stop typing");
        Render(LegalityStatus.Stale, autoRun: false).Find("#legality-summary").TextContent.Should().Contain("Analyze now");
        Render(LegalityStatus.Stale, draftValid: false).Find("#legality-summary").TextContent.Should().Contain("refused");
    }

    [Fact]
    public void AnalyzeNowIsOfferedUnlessRunningOrRefused()
    {
        var panel = Render(LegalityStatus.Stale);
        panel.Find("#analyze").Click();
        calls.Should().Equal("analyze");
        Render(LegalityStatus.Pending).Find("#analyze").HasAttribute("disabled").Should().BeFalse("a waiting analysis can be started at once");
        Render(LegalityStatus.Pending, running: true).Find("#analyze").HasAttribute("disabled").Should().BeTrue();
        Render(LegalityStatus.Stale, draftValid: false).Find("#analyze").HasAttribute("disabled").Should().BeTrue();
    }

    [Fact]
    public void ThePanelIsBusyOnlyWhileRunning()
    {
        Render(LegalityStatus.Pending).Find("#legality").GetAttribute("aria-busy").Should().Be("false");
        Render(LegalityStatus.Pending, running: true).Find("#legality").GetAttribute("aria-busy").Should().Be("true");
    }

    [Fact]
    public void CoreTextIsRenderedAsText()
    {
        var current = Result(LegalityVerdict.Invalid, new LegalityFinding(Severity.Invalid, CheckIdentifier.Nickname, Markup));
        var panel = Render(LegalityStatus.Invalid, current);
        panel.FindAll("#injected").Should().BeEmpty();
        panel.Find(".finding-text").TextContent.Should().Be(Markup);
    }

    [Fact]
    public void TheNotesNameTheCoreBuildAndTheLimits()
    {
        var panel = Render(LegalityStatus.NotAnalyzed);
        panel.Find("#legality-engine").TextContent.Should().Contain("PKHeX.Core " + BuildInfo.CoreVersion);
        panel.Find("#legality-guarantee").TextContent.Should().Contain("not an online acceptance guarantee");
    }
}
