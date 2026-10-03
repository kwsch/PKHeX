using Bunit;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Time.Testing;
using PKHeX.Web.Components;
using PKHeX.Web.State;
using Xunit;

namespace PKHeX.Web.Tests;

/// <summary>
/// The responsive workspace's layout state (WEB-APP-003): which pane a narrow screen shows and whether a medium one folds the storage pane
/// away, how both follow the draft and the session, and the markup and focus targets the page builds from them.
/// </summary>
[Trait(TestCategory.Name, TestCategory.Unit)]
public sealed class WorkspaceViewTests : IAsyncLifetime
{
    private readonly WorkspaceState state = SaveFixtures.NewState();
    private readonly BunitContext context = new();
    private readonly BunitJSModuleInterop browser;

    public WorkspaceViewTests()
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

    private SaveSession OpenSession()
    {
        var session = SaveFixtures.Open(SaveFixtures.Synthetic(false, customize: SaveFixtures.WithPartyMember()));
        state.Open(session);
        return session;
    }

    [Fact]
    public void OpeningASlotShowsItsEditorAndTheReturnActionKeepsTheDraft()
    {
        OpenSession();
        state.View.Pane.Should().Be(WorkspacePane.Storage);
        state.View.HasDraft.Should().BeFalse();

        state.OpenSlot(SaveFixtures.FirstBoxSlot).Should().Be(SlotOpening.Opened);
        state.View.Pane.Should().Be(WorkspacePane.Editor);
        state.Draft!.EditNickname("Kept", true);

        state.View.ShowStorage();
        state.View.Pane.Should().Be(WorkspacePane.Storage);
        state.Draft.Should().NotBeNull("the return action only changes the pane");
        state.Draft!.Nickname.Should().Be("Kept");

        // The selected slot again: nothing reopens, but its editor is shown.
        state.OpenSlot(SaveFixtures.FirstBoxSlot).Should().Be(SlotOpening.AlreadyOpen);
        state.View.Pane.Should().Be(WorkspacePane.Editor);
    }

    [Fact]
    public void ARefusedOpenKeepsThePane()
    {
        OpenSession();
        state.OpenSlot(SaveFixtures.FirstBoxSlot);
        state.Draft!.EditNickname("Pending", true);
        state.View.ShowStorage();

        state.OpenSlot(SlotRef.InParty(0)).Should().Be(SlotOpening.DraftPending);
        state.View.Pane.Should().Be(WorkspacePane.Storage, "nothing was opened, so the storage the user is browsing stays");
    }

    [Fact]
    public void TheFoldNeedsADraftAndEndsWithIt()
    {
        OpenSession();
        state.View.ToggleStorage();
        state.View.StorageCollapsed.Should().BeFalse("without a draft the party and boxes are all there is to show");

        state.OpenSlot(SaveFixtures.FirstBoxSlot);
        state.View.ToggleStorage();
        state.View.StorageCollapsed.Should().BeTrue();

        // An empty slot closes the draft, so storage comes back on every width.
        state.OpenSlot(SlotRef.InBox(0, 1)).Should().Be(SlotOpening.Empty);
        state.Draft.Should().BeNull();
        state.View.StorageCollapsed.Should().BeFalse();
        state.View.Pane.Should().Be(WorkspacePane.Storage);
        state.View.HasDraft.Should().BeFalse();
    }

    [Fact]
    public async Task ApplyAndCancelKeepTheEditorShown()
    {
        OpenSession();
        state.OpenSlot(SaveFixtures.FirstBoxSlot);
        state.View.ToggleStorage();
        state.Draft!.EditNickname("Applied", true);
        await SaveFixtures.ApplyAsync(state);
        state.View.Pane.Should().Be(WorkspacePane.Editor);
        state.View.StorageCollapsed.Should().BeTrue();

        state.SetDraft(state.Session!.Select(SaveFixtures.FirstBoxSlot));
        state.View.Pane.Should().Be(WorkspacePane.Editor, "a cancelled draft is reopened clean on the same slot");
        state.View.StorageCollapsed.Should().BeTrue();
    }

    [Fact]
    public void ANewSessionAFaultAndADiscardedDraftResetTheLayout()
    {
        OpenSession();
        state.OpenSlot(SaveFixtures.FirstBoxSlot);
        state.View.ToggleStorage();
        OpenSession();
        AssertReset();

        state.OpenSlot(SaveFixtures.FirstBoxSlot);
        state.View.ToggleStorage();
        state.RecoverAfterFault();
        AssertReset();

        state.OpenSlot(SaveFixtures.FirstBoxSlot);
        state.View.ToggleStorage();
        state.Draft!.EditNickname("Dropped", true);
        state.NotifyChanged();
        state.RequestClose();
        state.DiscardDraftForExit();
        AssertReset();

        void AssertReset()
        {
            state.View.Pane.Should().Be(WorkspacePane.Storage);
            state.View.StorageCollapsed.Should().BeFalse();
            state.View.HasDraft.Should().BeFalse();
        }
    }

    [Theory]
    [InlineData(true, 3, false, 0, "party-grid-3")]
    [InlineData(true, 3, true, 7, "party-list-3")]
    [InlineData(false, 29, false, 2, "box-grid-29")]
    [InlineData(false, 0, true, 2, "box-list-0")]
    public void TheSelectedSlotIsFoundInTheViewShown(bool party, int slot, bool asList, int box, string id)
    {
        var at = party ? SlotRef.InParty(slot) : SlotRef.InBox(2, slot);
        WorkspaceLayout.SlotElementId(at, asList, box).Should().Be(id);
    }

    [Fact]
    public void ASlotInABoxNotShownHasNoElement() =>
        WorkspaceLayout.SlotElementId(SlotRef.InBox(4, 0), asList: false, currentBox: 3).Should().BeNull();

    [Fact]
    public void TheLayoutQueriesAreTheSameInTheStylesheetAndTheScript()
    {
        var root = Path.Combine(SaveFixtures.RepositoryRoot, "PKHeX.Web", "wwwroot");
        var css = File.ReadAllText(Path.Combine(root, "app.css"));
        var script = File.ReadAllText(Path.Combine(root, "browser.js"));
        css.Should().Contain($"@media {WorkspaceLayout.NarrowQuery}");
        script.Should().Contain($"const narrowQuery = '{WorkspaceLayout.NarrowQuery}';");
        css.Should().Contain($"@media {WorkspaceLayout.WideQuery}");
        script.Should().Contain($"const wideQuery = '{WorkspaceLayout.WideQuery}';");
    }

    [Fact]
    public void ThePanesAreMarkedForTheirWidthAndFollowTheView()
    {
        var workspace = context.Render<Workspace>();
        OpenSession();
        workspace.Render();
        var panes = workspace.Find("#workspace-panes");
        panes.GetAttribute("data-pane").Should().Be("storage");
        panes.HasAttribute("data-storage-collapsed").Should().BeFalse();
        workspace.FindAll("#pane-editor").Should().BeEmpty();
        workspace.FindAll("#editor-return, #storage-toggle").Should().BeEmpty("both act on an open editor");

        workspace.Find("#box-grid-0").Click();
        panes = workspace.Find("#workspace-panes");
        panes.GetAttribute("data-pane").Should().Be("editor");
        workspace.Find("#editor-return").ClassList.Should().Contain("narrow-only");
        workspace.Find("#editor-return").TextContent.Should().Be(WorkspaceLayout.ReturnToStorage);
        var fold = workspace.Find("#storage-toggle");
        fold.ClassList.Should().Contain("medium-only");
        fold.GetAttribute("aria-controls").Should().Be("pane-storage");
        fold.GetAttribute("aria-expanded").Should().Be("true");
        browser.Invocations.Should().Contain(i => i.Identifier == "focusIfNarrow" && (string?)i.Arguments[0] == "draft-title",
            "a narrow screen hides the slot that had focus");

        fold.Click();
        workspace.Find("#storage-toggle").GetAttribute("aria-expanded").Should().Be("false");
        workspace.Find("#workspace-panes").GetAttribute("data-storage-collapsed").Should().Be("true");
        // Folded storage stays rendered, so the box, the list mode and the selection are kept.
        workspace.Find("#pane-storage #box-grid [aria-selected=true] button").Id.Should().Be("box-grid-0");

        workspace.Find("#editor-return").Click();
        workspace.Find("#workspace-panes").GetAttribute("data-pane").Should().Be("storage");
        browser.Invocations.Should().Contain(i => i.Identifier == "focusElement" && (string?)i.Arguments[0] == "box-grid-0");

        // The way back to the draft, from the storage pane.
        var resume = workspace.Find("#pane-storage #editor-resume");
        resume.ClassList.Should().Contain("narrow-only");
        resume.TextContent.Should().Be(WorkspaceLayout.ResumeEditor);
        var focusCalls = browser.Invocations.Count(i => i.Identifier == "focusIfNarrow");
        resume.Click();
        workspace.Find("#workspace-panes").GetAttribute("data-pane").Should().Be("editor");
        browser.Invocations.Count(i => i.Identifier == "focusIfNarrow").Should().Be(focusCalls + 1);

        // Without a draft there is nothing to go back to.
        workspace.Find("#cancel-draft").Click();
        state.SetDraft(null);
        workspace.Render();
        workspace.FindAll("#editor-resume").Should().BeEmpty();
    }

    [Fact]
    public void ReturningToABoxNotShownFocusesTheStorageHeading()
    {
        var workspace = context.Render<Workspace>();
        OpenSession();
        workspace.Render();
        workspace.Find("#box-grid-0").Click();
        state.ShowBox(5);

        workspace.Find("#editor-return").Click();

        browser.Invocations.Should().Contain(i => i.Identifier == "focusElement" && (string?)i.Arguments[0] == WorkspaceLayout.StorageHeadingId);
        workspace.Find($"#{WorkspaceLayout.StorageHeadingId}").GetAttribute("tabindex").Should().Be("-1");
    }
}
