using FluentAssertions;
using PKHeX.Web.Components;
using PKHeX.Web.Services;
using PKHeX.Web.State;
using Xunit;

namespace PKHeX.Web.Tests;

/// <summary>
/// The exact text for change and download state, the exit steps and the download name (WEB-SESSION-003/005/007, WEB-EXP-002).
/// </summary>
/// <remarks>The E2E tier builds its expected text from these pinned strings, not from the mapping under test.</remarks>
[Trait(TestCategory.Name, TestCategory.Unit)]
public sealed class SessionStatusTextTests
{
    [Fact]
    public void ExportStatusTextIsPinnedAndNeverClaimsTheFileWasSaved()
    {
        SessionStatusText.For(ExportStatus.Unchanged).Should().Be("No changes to download.");
        SessionStatusText.For(ExportStatus.NotExported).Should().Be("Changes not downloaded yet.");
        SessionStatusText.For(ExportStatus.ExportedCurrent).Should().Be("Download started for the current changes — verify your file.");
        SessionStatusText.For(ExportStatus.ChangedSinceExport).Should().Be("Changed since the last download.");
        SessionStatusText.DownloadStarted.Should().Be("Download started — verify your file. The session remains temporary.");
        foreach (var status in Enum.GetValues<ExportStatus>())
        {
            SessionStatusText.For(status).Should().NotContainEquivalentOf("saved");
        }
    }

    [Fact]
    public void ChangesStayEditedAfterADownload()
    {
        var session = SaveFixtures.Open(SaveFixtures.Synthetic(false));
        SessionStatusText.Changes(session).Should().Be("Unmodified session");
        var draft = session.Select(SaveFixtures.FirstBoxSlot);
        draft.EditNickname("Changed", true);
        session.Apply(draft);
        session.MarkExported(session.Revision);
        SessionStatusText.Changes(session).Should().Be("Edited in memory");
    }

    [Fact]
    public void ExitTextNamesTheWaitingFile()
    {
        var candidate = SaveFixtures.Open(SaveFixtures.Synthetic(true), "other-main");
        var replace = SessionExit.Replace(candidate);
        SessionStatusText.ExitTitle(replace).Should().Be("Open other-main?");
        SessionStatusText.ExitTitle(SessionExit.Close).Should().Be("Close this save?");
        SessionStatusText.DiscardSession(replace).Should().Be("Discard session and open other-main");
        SessionStatusText.DiscardSession(SessionExit.Close).Should().Be("Discard session and close");
        SessionStatusText.Continue(replace, ExitStage.ConfirmExport).Should().Be("Continue; I have checked my export");
        SessionStatusText.Continue(SessionExit.Close, ExitStage.ConfirmExport).Should().Be("Continue; I have checked my export");
        SessionStatusText.Continue(replace, ExitStage.Ready).Should().Be("Open other-main");
        SessionStatusText.Continue(SessionExit.Close, ExitStage.Ready).Should().Be("Close save");

        SessionStatusText.ExitPrompt(ExitStage.ResolveDraft).Should().Be("The editor has changes that are not applied. Apply them to the save, or discard them.");
        SessionStatusText.ExitPrompt(ExitStage.ResolveSession).Should().Be("This save has changes that have not been downloaded. Download it first, or discard the session and lose them.");
        SessionStatusText.ExitPrompt(ExitStage.ConfirmExport).Should().Be("A download of the current changes was started. Check that the file was saved before you continue; the session will be gone.");
        SessionStatusText.ExitPrompt(ExitStage.Ready).Should().Be("Nothing will be lost.");
    }

    [Fact]
    public void NameTextAndRenameRule()
    {
        SessionStatusText.NameOption(ExportNameChoice.Edited, "main-modified-2026-10-01-143205").Should().Be("Edited name: main-modified-2026-10-01-143205 (stamped with the time of the download)");
        SessionStatusText.NameOption(ExportNameChoice.Original, "main").Should().Be("Original name: main");
        SessionStatusText.RenameToRestore.Should().Be("To restore this file with a save manager on the console, or in an emulator, rename it to main first.");
        ExportNaming.NeedsRenameToRestore("main").Should().BeFalse();
        ExportNaming.NeedsRenameToRestore("Main").Should().BeTrue("consoles match the name exactly");
        ExportNaming.NeedsRenameToRestore("main-modified-2026-10-01-143205").Should().BeTrue();
    }

    [Fact]
    public void TheDefaultNameIsTheEditedOneOnceChangesAreApplied()
    {
        var stamp = new DateTime(2026, 10, 1, 14, 32, 5);
        var session = SaveFixtures.Open(SaveFixtures.Synthetic(false));
        ExportNaming.DefaultFor(session).Should().Be(ExportNameChoice.Original, "an unchanged download is the original's bytes");
        ExportNaming.NameFor(session, ExportNameChoice.Original, stamp).Should().Be("main");
        ExportNaming.NameFor(session, ExportNameChoice.Edited, stamp).Should().Be("main-modified-2026-10-01-143205");

        var draft = session.Select(SaveFixtures.FirstBoxSlot);
        draft.EditNickname("Changed", true);
        session.Apply(draft);
        ExportNaming.DefaultFor(session).Should().Be(ExportNameChoice.Edited);
    }
}
