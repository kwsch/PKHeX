using FluentAssertions;
using PKHeX.Core;
using PKHeX.Web.Services;
using PKHeX.Web.Services.Diagnostics;
using Xunit;

namespace PKHeX.Web.Tests;

/// <summary>
/// The diagnostic report text (WEB-SEC-005): build, browser, open family and redacted failures, and nothing from the save.
/// </summary>
[Trait(TestCategory.Name, TestCategory.Unit)]
public sealed class DiagnosticReportTests
{
    private const string TrainerSentinel = "ZZTRAINER";
    private const string NicknameSentinel = "ZZNICK";
    private const string BoxSentinel = "ZZBOX";
    private const string FileSentinel = "ZZFILE-main";

    private static readonly DiagnosticContext Context = new("26.10.01", "0123456789abcdef0123456789abcdef01234567", "26.10.1", false,
        "Mozilla/5.0 (Test) Browser/1.0", "Pokémon X and Y", DiagnosticFixtures.Start);

    [Fact]
    public void WithoutEntriesTheReportNamesTheBuildAndSaysNothingWasRecorded()
    {
        var report = DiagnosticReport.Build(Context, []);
        report.Should().Be(
            "PKHeX Web diagnostic report\n"
            + DiagnosticReport.Scope + "\n"
            + "\n"
            + "Web version: 26.10.01\n"
            + "Source commit: 0123456789abcdef0123456789abcdef01234567\n"
            + "PKHeX.Core version: 26.10.1\n"
            + "Sprites: not included\n"
            + "Browser: Mozilla/5.0 (Test) Browser/1.0\n"
            + "Open save: Pokémon X and Y\n"
            + "Created (UTC): 2026-10-03T12:00:00Z\n"
            + "\n"
            + DiagnosticReport.NoEntries + "\n");
    }

    [Fact]
    public void EntriesListTheOperationCodeTypesAndFrames()
    {
        var log = DiagnosticFixtures.NewLog();
        log.Record(DiagnosticOperation.Open, DiagnosticCode.For(SaveLoadOutcome.NotEnabled(new(typeof(SAV5BW), GameVersion.B, 5))));
        log.Record(DiagnosticOperation.Export, new InvalidOperationException(NicknameSentinel));

        var report = DiagnosticReport.Build(Context with { OpenFamily = null, SpritesIncluded = true }, log.Entries);

        report.Should().Contain("Open save: none\n").And.Contain("Sprites: included\n");
        report.Should().Contain("Recorded problems, oldest first (2; at most 20 are kept):\n"
            + "1. 2026-10-03T12:00:00Z Open: open.recognized-not-enabled\n"
            + "   Recognised as: SAV5BW\n"
            + "2. 2026-10-03T12:00:00Z Export: unexpected\n"
            + "   Exception: System.InvalidOperationException\n");
        report.Should().NotContain(NicknameSentinel);
    }

    [Theory]
    [InlineData(null, "unknown")]
    [InlineData("   ", "unknown")]
    [InlineData("A\u0000B\nC\u001b", "ABC")]
    public void TheBrowserStringIsCleaned(string? userAgent, string expected)
    {
        DiagnosticReport.Browser(userAgent).Should().Be(expected);
        DiagnosticReport.Browser(new string('a', 2000)).Should().HaveLength(DiagnosticReport.MaxBrowserLength);
    }

    [Fact]
    public void NothingFromAnOpenedSaveReachesTheReport()
    {
        // A save whose names are sentinels, opened under a sentinel file name, with every failure kind the workspace records.
        var bytes = SaveFixtures.Synthetic(false, customize: save =>
        {
            save.OT = TrainerSentinel[..Math.Min(TrainerSentinel.Length, 12)];
            ((IBoxDetailName)save).SetBoxName(0, BoxSentinel);
            var pk = save.GetBoxSlotAtIndex(0, 0);
            pk.SetNickname(NicknameSentinel);
            save.SetBoxSlotAtIndex(pk, 0, 0);
        });
        var session = SaveFixtures.Open(bytes, FileSentinel);
        var log = DiagnosticFixtures.NewLog();
        log.Record(DiagnosticOperation.Open, DiagnosticCode.For(SaveLoader.Load(new byte[3], FileSentinel)));
        log.Record(DiagnosticOperation.Apply, new PKHeX.Web.State.SessionException(PKHeX.Web.State.SessionError.StagedEditMismatch));
        log.Record(DiagnosticOperation.Render, new InvalidOperationException($"{TrainerSentinel} {NicknameSentinel} {FileSentinel}"));

        var report = DiagnosticReport.Build(DiagnosticContext.ForBuild("UA", session.Capabilities.Family.Games, DiagnosticFixtures.Start), log.Entries);

        report.Should().NotContain(TrainerSentinel).And.NotContain(NicknameSentinel).And.NotContain(BoxSentinel).And.NotContain(FileSentinel);
        report.Should().NotContain(session.SessionId.ToString()).And.NotContain(bytes.Length.ToString(System.Globalization.CultureInfo.InvariantCulture));
        report.Should().Contain("Open save: Pokémon X and Y").And.Contain($"Source commit: {BuildInfo.SourceCommit}");
    }
}
