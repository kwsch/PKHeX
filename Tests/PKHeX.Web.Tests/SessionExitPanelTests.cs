using Bunit;
using FluentAssertions;
using PKHeX.Web.Components;
using PKHeX.Web.State;
using Xunit;

namespace PKHeX.Web.Tests;

/// <summary>
/// The exit panel (WEB-SESSION-004/005, WEB-LEGAL-003): one step's choices at a time, focus moved at each step, the waiting file named as text,
/// a discard's single confirmation, and the legality acknowledgements its apply and download steps wait for.
/// </summary>
[Trait(TestCategory.Name, TestCategory.Unit)]
public sealed class SessionExitPanelTests : IDisposable
{
    private readonly BunitContext context = new();
    private readonly WorkspaceState state = SaveFixtures.NewState();
    private readonly List<string> calls = [];

    public SessionExitPanelTests()
    {
        context.JSInterop.Mode = JSRuntimeMode.Loose;
    }

    public void Dispose() => context.Dispose();

    private IRenderedComponent<SessionExitPanel> RenderPanel(bool busy = false) => context.Render<SessionExitPanel>(p => Bind(p)
        .Add(c => c.Busy, busy)
        .Add(c => c.OnApplyDraft, () => calls.Add("apply"))
        .Add(c => c.OnDiscardDraft, () => calls.Add("discard-draft"))
        .Add(c => c.OnExport, () => calls.Add("export"))
        .Add(c => c.OnContinue, () => calls.Add("continue"))
        .Add(c => c.OnDiscardSession, () => calls.Add("discard-session"))
        .Add(c => c.OnCancel, () => calls.Add("cancel"))
        .Add(c => c.OnAcknowledgeApply, (bool value) => calls.Add($"ack-apply:{value}"))
        .Add(c => c.OnAcknowledgeExport, (bool value) => calls.Add($"ack-export:{value}")));

    /// <summary>Passes the state's exit as the workspace does.</summary>
    private ComponentParameterCollectionBuilder<SessionExitPanel> Bind(ComponentParameterCollectionBuilder<SessionExitPanel> p) => p
        .Add(c => c.Exit, state.Exit)
        .Add(c => c.Stage, state.ExitStage)
        .Add(c => c.CanApplyDraft, state.Draft is { CanApply: true } && state.DraftValid)
        .Add(c => c.ApplyGate, state.Legality.Gate)
        .Add(c => c.ApplyVerdict, state.Legality.Current?.Verdict ?? LegalityVerdict.Unavailable)
        .Add(c => c.Flagged, state.Session?.FlaggedChanges ?? new Dictionary<SlotRef, LegalityVerdict>())
        .Add(c => c.ExportAcknowledged, !(state.Session?.ExportNeedsAcknowledgement ?? false))
        .Add(c => c.DraftPending, state.DraftDirty || !state.DraftValid)
        .Add(c => c.ExportStatus, state.Session?.ExportStatus ?? ExportStatus.Unchanged);

    /// <summary>Re-renders the panel with the state's current exit, as the workspace does after each step.</summary>
    private void Refresh(IRenderedComponent<SessionExitPanel> panel) => panel.Render(p => Bind(p));

    private static string[] Buttons(IRenderedComponent<SessionExitPanel> panel) => [.. panel.FindAll("button").Select(b => b.Id!)];

    private int FocusCalls() => context.JSInterop.Invocations.Count(i => i.Identifier == "Blazor._internal.domWrapper.focus");

    /// <summary>An open session with one applied change and a dirty draft, plus a waiting candidate.</summary>
    private async Task<SaveSession> OpenChangedWithDirtyDraft(string candidateName)
    {
        var session = SaveFixtures.Open(SaveFixtures.Synthetic(false));
        state.Open(session);
        var applied = session.Select(SaveFixtures.FirstBoxSlot);
        applied.EditNickname("Applied", true);
        state.SetDraft(applied);
        await SaveFixtures.ApplyAsync(state);
        state.Draft!.EditNickname("Dirty", true);
        state.RequestReplace(SaveFixtures.Open(SaveFixtures.Synthetic(true), candidateName));
        return session;
    }

    [Fact]
    public void RendersNothingWithoutAnExit()
    {
        RenderPanel().Markup.Trim().Should().BeEmpty();
    }

    [Fact]
    public async Task EachStepOffersOnlyItsOwnChoicesAndMovesFocus()
    {
        var session = await OpenChangedWithDirtyDraft("other-main");
        var panel = RenderPanel();
        panel.Find("#exit-title").TextContent.Should().Be($"Open {TestText.Isolated("other-main")}?");
        panel.Find("#exit-name").TextContent.Should().Be("other-main");
        Buttons(panel).Should().Equal("exit-apply-draft", "exit-discard-draft", "exit-cancel");
        FocusCalls().Should().Be(1);

        state.DiscardDraftForExit();
        Refresh(panel);
        Buttons(panel).Should().Equal("exit-export", "exit-discard-session", "exit-cancel");
        panel.Find("#exit-discard-session").TextContent.Should().Be($"Discard session and open {TestText.Isolated("other-main")}");
        FocusCalls().Should().Be(2, "a new step moves focus to the heading again");
        Refresh(panel);
        FocusCalls().Should().Be(2, "re-rendering the same step leaves focus alone");

        session.MarkExported(session.Revision);
        Refresh(panel);
        Buttons(panel).Should().Equal("exit-continue", "exit-export", "exit-cancel");
        panel.Find("#exit-continue").TextContent.Should().Be("Continue; I have checked my export");
        panel.Find("#exit-export").TextContent.Should().Be("Download again");
        FocusCalls().Should().Be(3);

        panel.Find("#exit-continue").Click();
        panel.Find("#exit-export").Click();
        panel.Find("#exit-cancel").Click();
        calls.Should().Equal("continue", "export", "cancel");
    }

    [Fact]
    public async Task ARefusedDraftCanOnlyBeDiscarded()
    {
        await OpenChangedWithDirtyDraft("other-main");
        state.SetDraftValid(false);
        var panel = RenderPanel();
        Buttons(panel).Should().Equal("exit-discard-draft", "exit-cancel");
    }

    [Fact]
    public async Task CloseNamesNoFileAndBusyDisablesEveryChoice()
    {
        var session = SaveFixtures.Open(SaveFixtures.Synthetic(false));
        state.Open(session);
        state.SetDraft(session.Select(SaveFixtures.FirstBoxSlot));
        state.ApplyDraft();
        state.Draft!.EditNickname("Changed", true);
        await SaveFixtures.ApplyAsync(state);
        state.RequestClose();

        var panel = RenderPanel(busy: true);
        panel.Find("#exit-title").TextContent.Should().Be("Close this save?");
        panel.FindAll("#exit-name").Should().BeEmpty();
        panel.Find("#exit-discard-session").TextContent.Should().Be("Discard session and close");
        panel.FindAll("button").Should().OnlyContain(b => b.HasAttribute("disabled"));
    }

    [Fact]
    public async Task TheWaitingNameIsRenderedAsText()
    {
        await OpenChangedWithDirtyDraft("<img src=x onerror=alert(1)>");
        var panel = RenderPanel();
        panel.FindAll("img").Should().BeEmpty();
        panel.Find("#exit-name").TextContent.Should().Be("_img src=x onerror=alert(1)_");
    }

    [Fact]
    public async Task ADiscardAsksOnceAndNamesWhatIsLost()
    {
        var session = await OpenChangedWithDirtyDraft("other-main");
        state.CancelExit();
        state.RequestDiscard();
        var panel = RenderPanel();
        panel.Find("#exit-title").TextContent.Should().Be("Discard this session?");
        panel.Find("#exit-prompt").TextContent.Should().Be(SessionStatusText.DiscardPrompt(true, ExportStatus.NotExported));
        Buttons(panel).Should().Equal("exit-discard-session", "exit-cancel");
        panel.Find("#exit-discard-session").TextContent.Should().Be("Discard session");
        FocusCalls().Should().Be(1);

        panel.Find("#exit-discard-session").Click();
        calls.Should().Equal("discard-session");
        session.Revision.Should().Be(1, "the panel only reports the choice; the workspace carries it out");
    }

    [Fact]
    public async Task AResetIsResolvedWithItsOwnWording()
    {
        await OpenChangedWithDirtyDraft("other-main");
        state.CancelExit();
        state.SetDraft(state.Session!.Select(SaveFixtures.FirstBoxSlot));
        state.RequestReset();
        var panel = RenderPanel();
        panel.Find("#exit-title").TextContent.Should().Be("Reset to the file as opened?");
        panel.FindAll("#exit-name").Should().BeEmpty("the file being reopened is the open one");
        Buttons(panel).Should().Equal("exit-export", "exit-discard-session", "exit-cancel");
        panel.Find("#exit-discard-session").TextContent.Should().Be("Discard changes and reset");
    }

    [Fact]
    public async Task ApplyDraftWaitsForTheResultAndItsAcknowledgement()
    {
        await OpenChangedWithDirtyDraft("other-main");
        var panel = RenderPanel();
        panel.Find("#exit-apply-ack-waiting").TextContent.Should().Be(LegalityText.ApplyWaiting);
        panel.Find("#exit-apply-draft").HasAttribute("disabled").Should().BeTrue("the dirty draft has no result yet");

        // The Zigzagoon is not its trainer's own in an X/Y save, so it is Invalid.
        await state.Legality.RunNowAsync();
        Refresh(panel);
        panel.FindAll("#exit-apply-ack-waiting").Should().BeEmpty();
        var box = panel.Find("#exit-apply-ack");
        box.HasAttribute("checked").Should().BeFalse();
        panel.Find("label[for=exit-apply-ack]").TextContent.Trim().Should().Be(LegalityText.ApplyAcknowledgement(LegalityVerdict.Invalid));
        panel.Find("#exit-apply-draft").HasAttribute("disabled").Should().BeTrue();
        box.Change(true);
        calls.Should().Equal("ack-apply:True");

        state.AcknowledgeLegality(true);
        Refresh(panel);
        panel.Find("#exit-apply-ack").HasAttribute("checked").Should().BeTrue();
        panel.Find("#exit-apply-draft").HasAttribute("disabled").Should().BeFalse();
    }

    [Fact]
    public async Task TheDownloadStepListsFlaggedChangesAndWaitsForTheirAcknowledgement()
    {
        var session = SaveFixtures.Open(SaveFixtures.Synthetic(false));
        state.Open(session);
        state.SetDraft(session.Select(SaveFixtures.FirstBoxSlot));
        state.Draft!.EditNickname("Flagged", true);
        await SaveFixtures.ApplyAsync(state);
        state.RequestClose();
        state.ExitStage.Should().Be(ExitStage.ResolveSession);

        var panel = RenderPanel();
        panel.Find("#exit-export-ack-list").TextContent.Should().Be(SessionStatusText.Flagged(SaveFixtures.FirstBoxSlot, LegalityVerdict.Invalid));
        panel.Find("#exit-export").HasAttribute("disabled").Should().BeTrue();
        panel.Find("#exit-discard-session").HasAttribute("disabled").Should().BeFalse("discarding never needs the acknowledgement");
        panel.Find("#exit-export-ack").Change(true);
        calls.Should().Equal("ack-export:True");

        state.AcknowledgeExport(true);
        Refresh(panel);
        panel.Find("#exit-export-ack").HasAttribute("checked").Should().BeTrue();
        panel.Find("#exit-export").HasAttribute("disabled").Should().BeFalse();
    }

    [Fact]
    public async Task DownloadAgainOffersTheAcknowledgementWhenItWasWithdrawn()
    {
        var session = SaveFixtures.Open(SaveFixtures.Synthetic(false));
        state.Open(session);
        state.SetDraft(session.Select(SaveFixtures.FirstBoxSlot));
        state.Draft!.EditNickname("Flagged", true);
        await SaveFixtures.ApplyAsync(state);
        state.AcknowledgeExport(true);
        session.MarkExported(session.Revision);
        // The user unticks the acknowledgement in the Download section after the download, then closes.
        state.AcknowledgeExport(false);
        state.RequestClose();
        state.ExitStage.Should().Be(ExitStage.ConfirmExport);

        var panel = RenderPanel();
        panel.Find("#exit-export").HasAttribute("disabled").Should().BeTrue();
        panel.FindAll("#exit-export-ack").Should().ContainSingle("Download again must not be disabled without a way to enable it");
    }
}
