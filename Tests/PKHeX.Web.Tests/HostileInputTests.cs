using FluentAssertions;
using PKHeX.Web.Components;
using PKHeX.Web.Services;
using PKHeX.Web.State;
using Xunit;

namespace PKHeX.Web.Tests;

/// <summary>
/// The hostile-input fixture holds what it claims, and the app's text for it is inert and cannot reorder the page.
/// The browser half is <see cref="HostileInputBrowserTests"/>.
/// </summary>
[Trait(TestCategory.Name, TestCategory.Unit)]
public sealed class HostileInputTests
{
    [Fact]
    public void CoreStoresTheHostileNames()
    {
        var session = SaveFixtures.Open(HostileFixtures.Save(), HostileFixtures.FileName);

        SaveOverview.From(session).TrainerName.Should().Be(HostileFixtures.TrainerName);
        StorageView.BoxName(session, 0).Should().Be(HostileFixtures.BoxName);
        var pk = session.Select(SaveFixtures.FirstBoxSlot);
        pk.Nickname.Should().Be(HostileFixtures.Nickname, "the override is stored as written");
        pk.Inspect().Origin.TrainerName.Should().Be(HostileFixtures.TrainerName);
        session.FileName.Should().Be(HostileFixtures.ShownFileName);
    }

    [Fact]
    public void TheAppsTextForThemHasNoBidiControlsOutsideItsIsolates()
    {
        var session = SaveFixtures.Open(HostileFixtures.Save(), HostileFixtures.FileName);
        var slot = StorageView.Box(session, 0).Slots[0];
        var draft = session.Select(SaveFixtures.FirstBoxSlot);
        var inspection = draft.Inspect();

        OverviewText.Trainer(SaveOverview.From(session)).Should().Be(HostileFixtures.TrainerName);
        SlotText.BoxOption(0, StorageView.BoxName(session, 0)).Should().Be($"1. {TestText.Isolated(HostileFixtures.BoxName)}");
        SlotText.Label(slot).Should().EndWith($"\"{TestText.Isolated(HostileFixtures.ShownNickname)}\"");
        var identity = InspectorText.Sections(inspection)[0].Rows.Single(r => r.Id == "inspect-nickname");
        identity.Value.Should().Be(HostileFixtures.ShownNickname);
        draft.Nickname.Should().Be(HostileFixtures.Nickname, "the draft, and so the nickname box, keeps the stored name");
    }
}
