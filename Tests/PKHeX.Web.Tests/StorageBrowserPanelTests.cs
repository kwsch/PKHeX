using Bunit;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using PKHeX.Web.Components;
using PKHeX.Web.Services;
using PKHeX.Web.State;
using Xunit;

namespace PKHeX.Web.Tests;

/// <summary>
/// The storage browser re-reads its views when the revision or the shown box changes, and only then (WEB-BOX-008).
/// </summary>
[Trait(TestCategory.Name, TestCategory.Unit)]
public sealed class StorageBrowserPanelTests : IDisposable
{
    private readonly BunitContext context = new();
    private readonly WorkspaceState state = SaveFixtures.NewState();

    public StorageBrowserPanelTests()
    {
        context.JSInterop.Mode = JSRuntimeMode.Loose;
        context.Services.AddSingleton(state);
        SpriteFixtures.AddCatalog(context);
    }

    public void Dispose() => context.Dispose();

    [Fact]
    public void ShowsTheNewRevisionAfterAnApply()
    {
        var session = SaveFixtures.Open(SaveFixtures.Synthetic(false));
        state.Open(session);
        state.ShowBox(0);
        var browser = context.Render<StorageBrowser>(p => p.Add(c => c.Session, session));
        var before = browser.Find("#box-grid-0").GetAttribute("aria-label");

        var draft = session.Select(SaveFixtures.FirstBoxSlot);
        draft.EditNickname("Renamed", true);
        session.Apply(draft);
        browser.Render(p => p.Add(c => c.Session, session));

        browser.Find("#box-grid-0").GetAttribute("aria-label").Should().NotBe(before).And.EndWith($"\"{TestText.Isolated("Renamed")}\"");
    }

    [Fact]
    public void NavigatesBoxesAndNamesThemInTheSelector()
    {
        var session = SaveFixtures.Open(SaveFixtures.Synthetic(false, customize: save => ((PKHeX.Core.IBoxDetailName)save).SetBoxName(30, "Last")));
        state.Open(session);
        state.ShowBox(0);
        var browser = context.Render<StorageBrowser>(p => p.Add(c => c.Session, session));

        browser.FindAll("#box-select option").Should().HaveCount(31);
        browser.FindAll("#box-select option")[30].TextContent.Should().Be($"31. {TestText.Isolated("Last")}");
        browser.Find("#box-prev").Click();
        state.CurrentBox.Should().Be(30);
        browser.Find("#box-title").TextContent.Should().Be($"31. {TestText.Isolated("Last")}");
        browser.Find("#box-grid").GetAttribute("aria-label").Should().Be("Last");
        browser.Find("#box-select").Change("4");
        browser.Find("#box-title").TextContent.Should().Be("5. " + SlotText.BoxTitle(4, StorageView.BoxName(session, 4)));
    }

    [Fact]
    public void ShowsANewSessionAtTheSameRevisionAndBox()
    {
        var first = SaveFixtures.Open(SaveFixtures.Synthetic(false));
        var second = SaveFixtures.Open(SaveFixtures.Synthetic(false, legal: false));
        state.Open(first);
        state.ShowBox(0);
        var browser = context.Render<StorageBrowser>(p => p.Add(c => c.Session, first));
        var before = browser.Find("#box-grid-0").GetAttribute("aria-label");

        state.Open(second);
        state.ShowBox(0);
        (second.Revision, state.CurrentBox).Should().Be((0, 0));
        browser.Render(p => p.Add(c => c.Session, second));

        browser.Find("#box-grid-0").GetAttribute("aria-label").Should().NotBe(before).And
            .Be(SlotText.Label(StorageView.Box(second, 0).Slots[0]));
    }

    [Fact]
    public void ReRenderingWithoutANewRevisionDoesNotReadTheSaveAgain()
    {
        var session = SaveFixtures.Open(SaveFixtures.Synthetic(false));
        state.Open(session);
        state.ShowBox(0);
        var browser = context.Render<StorageBrowser>(p => p.Add(c => c.Session, session));
        var before = browser.Find("#box-grid-0").GetAttribute("aria-label");

        // Changed behind the session's back, without an apply: a re-render at the same revision must not read it.
        var pk = session.Working.GetBoxSlotAtIndex(0, 0);
        pk.Nickname = "Unread";
        pk.IsNicknamed = true;
        session.Working.SetBoxSlotAtIndex(pk, 0, 0, PKHeX.Core.EntityImportSettings.None);
        browser.Render(p => p.Add(c => c.Session, session));

        browser.Find("#box-grid-0").GetAttribute("aria-label").Should().Be(before);
    }

    [Fact]
    public void PreviousAndNextAnnounceTheBoxShown()
    {
        var session = SaveFixtures.Open(SaveFixtures.Synthetic(false));
        state.Open(session);
        state.ShowBox(0);
        var browser = context.Render<StorageBrowser>(p => p.Add(c => c.Session, session));
        browser.Find("#box-status").TextContent.Should().BeEmpty();

        browser.Find("#box-next").Click();
        browser.Find("#box-status").TextContent.Should().Be("Showing " + SlotText.BoxOption(1, StorageView.BoxName(session, 1)));
        browser.Find("#box-status").GetAttribute("role").Should().Be("status");
    }
}
