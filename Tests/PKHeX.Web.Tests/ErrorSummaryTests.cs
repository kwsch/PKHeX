using Bunit;
using FluentAssertions;
using PKHeX.Web.Components;
using PKHeX.Web.State;
using Xunit;

namespace PKHeX.Web.Tests;

/// <summary>
/// The error summary and its text (WEB-A11Y-002): one sentence per reason, a link to the control that resolves it where there is one, nothing
/// rendered without a reason; and where a refused edit is shown in the editor.
/// </summary>
[Trait(TestCategory.Name, TestCategory.Unit)]
public sealed class ErrorSummaryTests : IDisposable
{
    private readonly BunitContext context = new();
    private readonly List<string> shown = [];

    public void Dispose() => context.Dispose();

    private IRenderedComponent<ErrorSummary> Render(params SummaryItem[] items) => context.Render<ErrorSummary>(p => p
        .Add(c => c.Id, "apply-summary")
        .Add(c => c.Title, ValidationText.ApplyTitle)
        .Add(c => c.Items, items)
        .Add(c => c.OnShow, (string id) => shown.Add(id)));

    [Fact]
    public void NothingIsRenderedWithoutAReason()
    {
        Render().Markup.Trim().Should().BeEmpty();
    }

    [Fact]
    public void EachReasonIsASentenceWithItsLink()
    {
        var summary = Render(new SummaryItem("Wait."), new SummaryItem("Fix it.", "Go to the field", "level"));

        var region = summary.Find("#apply-summary");
        region.GetAttribute("role").Should().Be("region");
        region.GetAttribute("tabindex").Should().Be("-1", "the workspace focuses it when Apply is activated");
        region.GetAttribute("aria-labelledby").Should().Be("apply-summary-title");
        summary.Find("#apply-summary-title").TextContent.Should().Be(ValidationText.ApplyTitle);
        summary.Find("#apply-summary-title").TagName.Should().Be("H3", "heading navigation finds it under the editor's or Download's heading");
        var items = summary.FindAll("#apply-summary-list li");
        items.Should().HaveCount(2);
        items[0].TextContent.Trim().Should().Be("Wait.");
        items[0].QuerySelector("a").Should().BeNull("a reason with nowhere to go has no link");
        var link = items[1].QuerySelector("a")!;
        link.TextContent.Should().Be("Go to the field");
        link.GetAttribute("href").Should().Be("#level");
    }

    [Fact]
    public void ALinkAsksForFocusWithoutNavigating()
    {
        var summary = Render(new SummaryItem("Fix it.", "Go to the field", "iv-3"));

        summary.Find("a").Click();

        shown.Should().Equal("iv-3");
        // Under /PKHeX/ the browser would resolve "#iv-3" against the base path and navigate away.
        summary.Find("a").HasAttribute("blazor:onclick:preventDefault").Should().BeTrue();
    }

    [Fact]
    public void AFieldErrorUsesTheRefusalOrTheFailureText()
    {
        ValidationText.FieldError(new FieldRefusal("level", SessionError.LevelOutOfRange)).Should().Be(UserMessages.For(SessionError.LevelOutOfRange));
        ValidationText.FieldError(new FieldRefusal("level", null)).Should().Be(ValidationText.FieldFailed);
    }

    [Fact]
    public void EveryReasonHasText()
    {
        var refusal = new FieldRefusal("ev-2", SessionError.EvTotalAboveLimit);
        foreach (var blocker in Enum.GetValues<ApplyBlocker>())
        {
            ValidationText.For(blocker, refusal, LegalityVerdict.Invalid).Text.Should().NotBeNullOrWhiteSpace();
        }
        foreach (var blocker in Enum.GetValues<DownloadBlocker>())
        {
            ValidationText.For(blocker, refusal).Text.Should().NotBeNullOrWhiteSpace();
        }
    }

    [Fact]
    public void ReasonsLinkToTheControlThatResolvesThem()
    {
        var refusal = new FieldRefusal("ev-2", SessionError.EvTotalAboveLimit);

        var refused = ValidationText.For(ApplyBlocker.FieldRefused, refusal, LegalityVerdict.Valid);
        refused.TargetId.Should().Be("ev-2");
        refused.Text.Should().Contain(UserMessages.For(SessionError.EvTotalAboveLimit));
        ValidationText.For(DownloadBlocker.FieldRefused, refusal).TargetId.Should().Be("ev-2");
        ValidationText.For(ApplyBlocker.SpeciesPreviewPending, null, LegalityVerdict.Valid).TargetId.Should().Be("species-confirm");
        ValidationText.For(ApplyBlocker.LegalityNotAcknowledged, null, LegalityVerdict.Invalid).TargetId.Should().Be("apply-ack");
        ValidationText.For(ApplyBlocker.LegalityNotAcknowledged, null, LegalityVerdict.Unavailable).Text.Should().Contain("could not analyse");
        ValidationText.For(DownloadBlocker.DraftNotApplied, null).TargetId.Should().Be("apply");
        ValidationText.For(DownloadBlocker.ExportNotAcknowledged, null).TargetId.Should().Be("export-ack");
        // These resolve themselves, or cannot be resolved here.
        ValidationText.For(ApplyBlocker.LegalityWaiting, null, LegalityVerdict.Valid).TargetId.Should().BeNull();
        ValidationText.For(ApplyBlocker.NoChanges, null, LegalityVerdict.Valid).TargetId.Should().BeNull();
        ValidationText.For(ApplyBlocker.NotWritable, null, LegalityVerdict.Valid).TargetId.Should().BeNull();
        ValidationText.For(DownloadBlocker.Busy, null).TargetId.Should().BeNull();
    }

    [Theory]
    [InlineData("species", "species-fields")]
    [InlineData("form", "species-fields")]
    [InlineData("nickname", "name-fields")]
    [InlineData("nicknamed", "name-fields")]
    [InlineData("language", "name-fields")]
    [InlineData("ot-friendship", "friendship-fields")]
    [InlineData("ht-friendship", "friendship-fields")]
    [InlineData("level", "level-fields")]
    [InlineData("exp", "level-fields")]
    [InlineData("nature", "nature-fields")]
    [InlineData("ability", "ability-fields")]
    [InlineData("gender", "ability-fields")]
    [InlineData("iv-0", "stat-fields")]
    [InlineData("ev-5", "stat-fields")]
    [InlineData("held-item", "item-fields")]
    [InlineData("move-3", "move-fields")]
    [InlineData("pp-0", "move-fields")]
    [InlineData("ppups-2", "move-fields")]
    public void EveryControlBelongsToItsFieldset(string field, string fieldset)
    {
        EditorFields.FieldsetOf(field).Should().Be(fieldset);
        EditorFields.ErrorIdFor(new FieldRefusal(field, null)).Should().Be($"{fieldset}-error");
    }

    [Theory]
    [InlineData("apply")]
    [InlineData("iv-")]
    [InlineData("iv-10")]
    [InlineData("iv-x")]
    [InlineData("moves-1")]
    public void AnUnknownControlHasNoFieldset(string field)
    {
        EditorFields.FieldsetOf(field).Should().BeNull();
    }
}
