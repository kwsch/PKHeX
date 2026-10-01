using Bunit;
using FluentAssertions;
using PKHeX.Web.Components;
using PKHeX.Web.State;
using Xunit;

namespace PKHeX.Web.Tests;

/// <summary>
/// The exit panel (WEB-SESSION-005): one step's choices at a time, focus moved at each step, and the waiting file named as text.
/// </summary>
[Trait(TestCategory.Name, TestCategory.Unit)]
public sealed class SessionExitPanelTests : IDisposable
{
    private readonly BunitContext context = new();
    private readonly WorkspaceState state = new();
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
        .Add(c => c.OnCancel, () => calls.Add("cancel")));

    /// <summary>Passes the state's exit as the workspace does.</summary>
    private ComponentParameterCollectionBuilder<SessionExitPanel> Bind(ComponentParameterCollectionBuilder<SessionExitPanel> p) => p
        .Add(c => c.Exit, state.Exit)
        .Add(c => c.Stage, state.ExitStage)
        .Add(c => c.CanApplyDraft, state.Draft is { CanApply: true } && state.DraftValid);

    /// <summary>Re-renders the panel with the state's current exit, as the workspace does after each step.</summary>
    private void Refresh(IRenderedComponent<SessionExitPanel> panel) => panel.Render(p => Bind(p));

    private static string[] Buttons(IRenderedComponent<SessionExitPanel> panel) => [.. panel.FindAll("button").Select(b => b.Id!)];

    private int FocusCalls() => context.JSInterop.Invocations.Count(i => i.Identifier == "Blazor._internal.domWrapper.focus");

    /// <summary>An open session with one applied change and a dirty draft, plus a waiting candidate.</summary>
    private SaveSession OpenChangedWithDirtyDraft(string candidateName)
    {
        var session = SaveFixtures.Open(SaveFixtures.Synthetic(false));
        state.Open(session);
        var applied = session.Select(SaveFixtures.FirstBoxSlot);
        applied.EditNickname("Applied", true);
        state.SetDraft(applied);
        state.ApplyDraft();
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
    public void EachStepOffersOnlyItsOwnChoicesAndMovesFocus()
    {
        var session = OpenChangedWithDirtyDraft("other-main");
        var panel = RenderPanel();
        panel.Find("#exit-title").TextContent.Should().Be("Open other-main?");
        panel.Find("#exit-name").TextContent.Should().Be("other-main");
        Buttons(panel).Should().Equal("exit-apply-draft", "exit-discard-draft", "exit-cancel");
        FocusCalls().Should().Be(1);

        state.DiscardDraftForExit();
        Refresh(panel);
        Buttons(panel).Should().Equal("exit-export", "exit-discard-session", "exit-cancel");
        panel.Find("#exit-discard-session").TextContent.Should().Be("Discard session and open other-main");
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
    public void ARefusedDraftCanOnlyBeDiscarded()
    {
        OpenChangedWithDirtyDraft("other-main");
        state.SetDraftValid(false);
        var panel = RenderPanel();
        Buttons(panel).Should().Equal("exit-discard-draft", "exit-cancel");
    }

    [Fact]
    public void CloseNamesNoFileAndBusyDisablesEveryChoice()
    {
        var session = SaveFixtures.Open(SaveFixtures.Synthetic(false));
        state.Open(session);
        state.SetDraft(session.Select(SaveFixtures.FirstBoxSlot));
        state.ApplyDraft();
        state.Draft!.EditNickname("Changed", true);
        state.ApplyDraft();
        state.RequestClose();

        var panel = RenderPanel(busy: true);
        panel.Find("#exit-title").TextContent.Should().Be("Close this save?");
        panel.FindAll("#exit-name").Should().BeEmpty();
        panel.Find("#exit-discard-session").TextContent.Should().Be("Discard session and close");
        panel.FindAll("button").Should().OnlyContain(b => b.HasAttribute("disabled"));
    }

    [Fact]
    public void TheWaitingNameIsRenderedAsText()
    {
        OpenChangedWithDirtyDraft("<img src=x onerror=alert(1)>");
        var panel = RenderPanel();
        panel.FindAll("img").Should().BeEmpty();
        panel.Find("#exit-name").TextContent.Should().Be("_img src=x onerror=alert(1)_");
    }
}
