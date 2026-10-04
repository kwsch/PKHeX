using Bunit;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Time.Testing;
using PKHeX.Web.Components;
using PKHeX.Web.State;
using Xunit;

namespace PKHeX.Web.Tests;

/// <summary>
/// Validation in the workspace (WEB-A11Y-002): Apply and Download stay focusable while they cannot act, activating one shows and focuses a
/// summary of why with links to the controls, a refused edit is shown on its control and not in the live status message, and typing never
/// moves focus or brings a summary back.
/// </summary>
[Trait(TestCategory.Name, TestCategory.Unit)]
public sealed class WorkspaceValidationTests : IAsyncLifetime
{
    private readonly BunitContext context = new();
    private readonly WorkspaceState state = SaveFixtures.NewState();
    private readonly BunitJSModuleInterop browser;

    public WorkspaceValidationTests()
    {
        context.JSInterop.Mode = JSRuntimeMode.Loose;
        browser = context.JSInterop.SetupModule("./browser.js");
        browser.Mode = JSRuntimeMode.Loose;
        context.Services.AddSingleton(state);
        context.Services.AddSingleton<TimeProvider>(new FakeTimeProvider());
        DiagnosticFixtures.AddDiagnostics(context.Services, DiagnosticFixtures.NewLog());
        SpriteFixtures.AddCatalog(context);
    }

    public Task InitializeAsync() => Task.CompletedTask;

    /// <summary>The interop services are only asynchronously disposable.</summary>
    public async Task DisposeAsync() => await context.DisposeAsync();

    /// <summary>A workspace with the first box slot open in the editor.</summary>
    private IRenderedComponent<Workspace> Opened()
    {
        var workspace = context.Render<Workspace>();
        state.Open(SaveFixtures.Open(SaveFixtures.Synthetic(true)));
        state.OpenSlot(SaveFixtures.FirstBoxSlot);
        workspace.Render();
        return workspace;
    }

    /// <summary>The ids focused through <c>browser.js</c>, in order.</summary>
    private string[] Focused() => [.. browser.Invocations.Where(i => i.Identifier == "focusElement").Select(i => (string)i.Arguments[0]!)];

    [Fact]
    public void ApplyAndDownloadStayFocusableWhileTheyCannotAct()
    {
        var workspace = Opened();

        foreach (var id in new[] { "apply", "download" })
        {
            var button = workspace.Find($"#{id}");
            button.HasAttribute("disabled").Should().BeFalse($"{id} must stay focusable to say why it cannot act");
            button.GetAttribute("type").Should().Be("button");
        }
        workspace.Find("#apply").GetAttribute("aria-disabled").Should().Be("true", "a clean draft has nothing to apply");
        workspace.Find("#download").GetAttribute("aria-disabled").Should().Be("false", "an unchanged save can be downloaded");
        workspace.FindAll(".error-summary").Should().BeEmpty("nothing was activated");
    }

    [Fact]
    public void ActivatingApplyShowsAndFocusesWhyItCannotApply()
    {
        var workspace = Opened();
        var revision = state.Session!.Revision;

        workspace.Find("#apply").Click();

        workspace.Find("#apply-summary-title").TextContent.Should().Be(ValidationText.ApplyTitle);
        workspace.Find("#apply-summary-list").TextContent.Should().Contain("no changes to apply");
        Focused().Should().EndWith("apply-summary");
        state.Session.Revision.Should().Be(revision, "nothing was applied");
    }

    [Fact]
    public void ARefusedEditIsShownOnItsControlNotInTheStatusMessage()
    {
        var workspace = Opened();
        var message = workspace.Find("#message").TextContent;

        workspace.Find("#level").Input("101");

        state.DraftRefusal.Should().Be(new FieldRefusal("level", SessionError.LevelOutOfRange));
        workspace.Find("#level").GetAttribute("aria-invalid").Should().Be("true");
        workspace.Find("#level-fields-error").TextContent.Should().Be(UserMessages.For(SessionError.LevelOutOfRange));
        workspace.Find("#message").TextContent.Should().Be(message, "the live status message would interrupt every refused keystroke");
        Focused().Should().BeEmpty("typing never moves focus");
        workspace.FindAll(".error-summary").Should().BeEmpty("a summary appears only when Apply or Download is activated");
    }

    [Fact]
    public void TheSummaryLinksToTheRefusedFieldAndFollowsItsCorrection()
    {
        var workspace = Opened();
        workspace.Find("#level").Input("101");

        workspace.Find("#apply").Click();
        var link = workspace.Find("#apply-summary-list a");
        link.GetAttribute("href").Should().Be("#level");
        link.Click();
        Focused().Should().Equal("apply-summary", "level");

        workspace.Find("#level").Input("50");

        // The draft changed, so the summary now says Apply waits for its legality result; the refusal is gone.
        workspace.Find("#level").HasAttribute("aria-invalid").Should().BeFalse();
        workspace.Find("#apply-summary-list").TextContent.Should().Be(LegalityText.ApplyWaiting);
        Focused().Should().Equal("apply-summary", "level");
    }

    [Fact]
    public async Task ASummaryWhoseReasonsAreResolvedDoesNotComeBackUnasked()
    {
        var workspace = Opened();
        workspace.Find("#level").Input("101");
        workspace.Find("#apply").Click();

        workspace.Find("#level").Input("50");
        await state.Legality.RunNowAsync();
        workspace.WaitForAssertion(() => workspace.Find("#apply").GetAttribute("aria-disabled").Should().Be("false"));
        workspace.FindAll("#apply-summary").Should().BeEmpty();

        workspace.Find("#level").Input("101");

        workspace.FindAll("#apply-summary").Should().BeEmpty("the earlier activation was resolved");
    }

    [Fact]
    public void ActivatingDownloadWithAnUnappliedDraftLinksToApply()
    {
        var workspace = Opened();
        workspace.Find("#nickname").Input("Pending");

        workspace.Find("#download").Click();

        workspace.Find("#download-summary-title").TextContent.Should().Be(ValidationText.DownloadTitle);
        workspace.Find("#download-summary-list a").GetAttribute("href").Should().Be("#apply");
        Focused().Should().EndWith("download-summary");
        state.Session!.ExportStatus.Should().Be(ExportStatus.Unchanged, "nothing was downloaded");
    }

    [Fact]
    public void ALinkToTheEditorShowsItsPaneFirst()
    {
        // On a phone the storage pane can be shown instead of the editor, which hides every control a download reason links to but its
        // acknowledgement.
        var workspace = Opened();
        workspace.Find("#nickname").Input("Pending");
        workspace.Find("#editor-return").Click();
        state.View.Pane.Should().Be(WorkspacePane.Storage);
        workspace.Find("#download").Click();

        workspace.Find("#download-summary-list a").Click();

        state.View.Pane.Should().Be(WorkspacePane.Editor);
        Focused().Should().EndWith("apply");
    }

    [Fact]
    public void ANewDraftDropsTheSummary()
    {
        var workspace = Opened();
        workspace.Find("#apply").Click();
        workspace.FindAll("#apply-summary").Should().HaveCount(1);

        workspace.Find("#cancel-draft").Click();

        workspace.FindAll("#apply-summary").Should().BeEmpty();
    }

    [Fact]
    public async Task PreviousAndNextStayFocusableAndSayWhyTheyCannotStep()
    {
        var workspace = Opened();
        foreach (var id in new[] { "draft-prev", "draft-next" })
        {
            var button = workspace.Find($"#{id}");
            button.HasAttribute("disabled").Should().BeFalse();
            button.GetAttribute("type").Should().Be("button");
            button.GetAttribute("aria-disabled").Should().Be("false", "a clean draft can step");
        }
        workspace.Find("#draft-steps").GetAttribute("aria-label").Should().Be(WorkspaceLayout.StepsLabel);
        var draft = state.Draft;

        workspace.Find("#level").Input("51");
        workspace.Find("#draft-next").GetAttribute("aria-disabled").Should().Be("true");
        await workspace.Find("#draft-next").ClickAsync(new());

        state.Draft.Should().BeSameAs(draft, "unapplied work is never replaced");
        workspace.Find("#step-summary-title").TextContent.Should().Be(ValidationText.StepTitle);
        workspace.Find("#step-summary-list a").GetAttribute("href").Should().Be("#apply");
        Focused().Should().EndWith("step-summary");

        workspace.Find("#step-summary-list a").Click();
        Focused().Should().EndWith("apply");

        workspace.Find("#cancel-draft").Click();
        workspace.FindAll("#step-summary").Should().BeEmpty("a new draft drops the summary");
    }

    [Fact]
    public async Task ARefusedFieldIsAStepReasonLinkingToTheField()
    {
        var workspace = Opened();
        workspace.Find("#level").Input("101");

        await workspace.Find("#draft-prev").ClickAsync(new());

        workspace.Find("#step-summary-list a").GetAttribute("href").Should().Be("#level");
        state.Draft!.Slot.Should().Be(SaveFixtures.FirstBoxSlot);
    }

    [Fact]
    public async Task StepOpensTheNextPokemonAndKeepsFocusOnTheButton()
    {
        var workspace = context.Render<Workspace>();
        state.Open(SaveFixtures.Open(SaveFixtures.Synthetic(true, customize: SaveFixtures.All(
            SaveFixtures.WithPartyMember(), SaveFixtures.WithBoxEntity(3, 4, _ => { })))));
        state.OpenSlot(SlotRef.InParty(0));
        workspace.Render();
        var before = browser.Invocations.Count(i => i.Identifier == "focusIfNarrow");

        // Each step cancels the last draft's legality timer, whose continuation can hold the renderer's dispatcher; a synchronous Click is
        // then queued behind it and returns before the step is made, so every step is awaited.
        await workspace.Find("#draft-next").ClickAsync(new());
        state.Draft!.Slot.Should().Be(SaveFixtures.FirstBoxSlot);
        workspace.Find("#draft-slot").TextContent.Should().Be(SlotText.Position(SaveFixtures.FirstBoxSlot));
        workspace.Find("#message").TextContent.Should().StartWith($"Opened {SlotText.Position(SaveFixtures.FirstBoxSlot)}: ");

        await workspace.Find("#draft-next").ClickAsync(new());
        state.Draft!.Slot.Should().Be(SlotRef.InBox(3, 4));
        state.CurrentBox.Should().Be(3);
        workspace.Find("#box-select").GetAttribute("value").Should().Be("3", "the storage browser follows into the box");
        workspace.Find("#box-grid-4").GetAttribute("tabindex").Should().Be("0", "the stepped-to slot is the grid's tab stop");

        await workspace.Find("#draft-next").ClickAsync(new());
        state.Draft!.Slot.Should().Be(SlotRef.InParty(0), "the last Pokémon wraps to the first");
        browser.Invocations.Count(i => i.Identifier == "focusIfNarrow").Should().Be(before, "focus stays on the button, not the heading");
        state.View.Pane.Should().Be(WorkspacePane.Editor);
    }

    [Fact]
    public async Task StepWithNoOtherPokemonSaysSo()
    {
        var workspace = Opened();
        await workspace.Find("#draft-prev").ClickAsync(new());
        workspace.Find("#message").TextContent.Should().Be("No other Pokémon in this save can be opened.");
        state.Draft!.Slot.Should().Be(SaveFixtures.FirstBoxSlot);
    }
}
