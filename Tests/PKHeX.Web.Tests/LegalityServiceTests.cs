using FluentAssertions;
using PKHeX.Core;
using PKHeX.Web.Services;
using PKHeX.Web.State;
using Xunit;

namespace PKHeX.Web.Tests;

/// <summary>
/// The legality service: Core's verdict, reports and severities on a copy of the draft, in the save's and slot's context.
/// </summary>
[Trait(TestCategory.Name, TestCategory.Unit)]
public sealed class LegalityServiceTests
{
    private static readonly IReadOnlyList<(string Name, byte[] Data)> Corpus = SaveFixtures.LegalityCorpus();

    public static TheoryData<int, bool> CorpusRows()
    {
        var rows = new TheoryData<int, bool>();
        for (var i = 0; i < SaveFixtures.LegalityCorpus().Count; i++)
        {
            rows.Add(i, false);
            rows.Add(i, true);
        }
        return rows;
    }

    /// <summary>Core's analysis of the entity at <paramref name="slot"/>, made the way the service makes it.</summary>
    private static NativeLegality Native(SaveSession session, SlotRef slot)
    {
        var save = session.Working;
        return NativeLegality.Of(save, SaveSession.ReadOccupied(save, slot)!, slot.ToSlotInfo(save).Type);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void KnownEntitiesGetCoresVerdictAndReports(bool legal)
    {
        var session = SaveFixtures.Open(SaveFixtures.Synthetic(true, legal));
        var draft = session.Select(SaveFixtures.FirstBoxSlot);
        var native = Native(session, SaveFixtures.FirstBoxSlot);
        native.Valid.Should().Be(legal);

        var result = LegalityService.Default.Analyze(session, draft, out var failure);

        failure.Should().BeNull();
        result.Verdict.Should().Be(legal ? LegalityVerdict.Valid : LegalityVerdict.Invalid);
        result.Report.Should().Be(native.Report);
        result.VerboseReport.Should().Be(native.VerboseReport);
        result.Tag.Should().Be(LegalityTag.Of(draft));
        (result.Problems == 0).Should().Be(legal, "a valid entity has no invalid finding and an invalid one has at least one");
    }

    /// <summary>
    /// Over every PK6 fixture in Core's tests, in XY and ORAS saves: the verdict is Core's, the problems are exactly the lines of Core's short
    /// report in its order, and the warnings are Core's suspicious checks.
    /// </summary>
    [Theory]
    [MemberData(nameof(CorpusRows))]
    public void FindingsMirrorCoreOverTheFixtureCorpus(int index, bool oras)
    {
        var session = SaveFixtures.Open(SaveFixtures.Synthetic(oras, customize: SaveFixtures.WithBoxEntities([Corpus[index].Data])));
        var slot = SlotRef.InBox(0, 1);
        var native = Native(session, slot);

        var result = LegalityService.Default.Analyze(session, session.Select(slot), out var failure);

        failure.Should().BeNull();
        result.Verdict.Should().Be(native.Valid ? LegalityVerdict.Valid : LegalityVerdict.Invalid);
        result.Report.Should().Be(native.Report);
        var problems = result.Findings.Where(f => f.Severity == Severity.Invalid).Select(f => f.Text).ToList();
        if (native.Valid)
        {
            problems.Should().BeEmpty();
        }
        else
        {
            problems.Should().Equal(native.Report.Split(Environment.NewLine), "problems are the lines of Core's short report, in order");
        }
        result.Findings.Should().OnlyContain(f => f.Severity == Severity.Invalid || f.Severity == Severity.Fishy);
        result.Findings.Should().BeInAscendingOrder(f => f.Severity, "problems come before warnings");
        result.Warnings.Should().Be(native.Warnings);
    }

    [Fact]
    public void TheCorpusIncludesProblemsAndWarnings()
    {
        // Guards the corpus test above against becoming vacuous: it must see both severities.
        var findings = Corpus.SelectMany(entity =>
        {
            var session = SaveFixtures.Open(SaveFixtures.Synthetic(true, customize: SaveFixtures.WithBoxEntities([entity.Data])));
            return LegalityService.Default.Analyze(session, session.Select(SlotRef.InBox(0, 1)), out _).Findings;
        }).ToList();
        findings.Should().Contain(f => f.Severity == Severity.Invalid);
        findings.Should().Contain(f => f.Severity == Severity.Fishy);
    }

    /// <summary>
    /// Core's handler checks compare an entity with the save's trainer, and run only with Core's active trainer set, as the desktop editor sets
    /// it for a loaded save. With the current handler flipped on each fixture, at least one verdict must depend on that trainer, and the
    /// service must give the desktop's verdict for every one.
    /// </summary>
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void TheSavesTrainerIsPartOfTheAnalysis(bool oras)
    {
        var flipped = Corpus.Select(c =>
        {
            var entity = new PK6(c.Data.ToArray());
            entity.CurrentHandler ^= 1;
            entity.RefreshChecksum();
            return entity.Data.ToArray();
        }).ToList();
        var session = SaveFixtures.Open(SaveFixtures.Synthetic(oras, customize: SaveFixtures.WithBoxEntities(flipped)));
        var save = session.Working;
        var dependsOnTrainer = 0;
        for (var i = 0; i < flipped.Count; i++)
        {
            var slot = SlotRef.InBox(0, i + 1);
            var entity = SaveSession.ReadOccupied(save, slot)!;
            var desktop = NativeLegality.Of(save, entity, StorageSlotType.Box);
            var withoutTrainer = LegalityService.InTrainerContext(null, () => new LegalityAnalysis(entity.Clone(), save.Personal, StorageSlotType.Box).Valid);
            if (withoutTrainer != desktop.Valid)
            {
                dependsOnTrainer++;
            }
            var result = LegalityService.Default.Analyze(session, session.Select(slot), out _);
            result.Verdict.Should().Be(desktop.Valid ? LegalityVerdict.Valid : LegalityVerdict.Invalid, Corpus[i].Name);
            result.Report.Should().Be(desktop.Report, Corpus[i].Name);
        }
        dependsOnTrainer.Should().BePositive("the case must be one the trainer context decides, or the test proves nothing");
    }

    [Fact]
    public void APartyMemberIsAnalysedAsAPartyMember()
    {
        var session = SaveFixtures.Open(SaveFixtures.Synthetic(false, customize: SaveFixtures.WithPartyMember("Leader")));
        var slot = SlotRef.InParty(0);
        var native = NativeLegality.Of(session.Working, session.Working.GetPartySlotAtIndex(0), StorageSlotType.Party);

        var result = LegalityService.Default.Analyze(session, session.Select(slot), out _);

        result.Verdict.Should().Be(native.Valid ? LegalityVerdict.Valid : LegalityVerdict.Invalid);
        result.Report.Should().Be(native.Report);
        result.VerboseReport.Should().Be(native.VerboseReport);
    }

    [Fact]
    public void AnalysisDoesNotChangeTheDraftOrTheSession()
    {
        var session = SaveFixtures.Open(SaveFixtures.Synthetic(false, legal: false));
        var draft = session.Select(SaveFixtures.FirstBoxSlot);
        draft.EditNickname("Edited", true);
        var drafted = draft.Preview().Data.ToArray();
        var working = session.Working;
        var saved = working.Data.ToArray();

        LegalityService.Default.Analyze(session, draft, out _);

        draft.Preview().Data.ToArray().Should().Equal(drafted);
        draft.EditRevision.Should().Be(1);
        session.Working.Should().BeSameAs(working);
        session.Working.Data.ToArray().Should().Equal(saved);
        session.Revision.Should().Be(0);
    }

    [Fact]
    public void TheDraftAsEditedIsAnalysed()
    {
        var session = SaveFixtures.Open(SaveFixtures.Synthetic(true));
        var draft = session.Select(SaveFixtures.FirstBoxSlot);
        draft.EditNickname("Edited", true);
        var native = NativeLegality.Of(session.Working, draft.Preview(), StorageSlotType.Box);

        var result = LegalityService.Default.Analyze(session, draft, out _);

        result.Report.Should().Be(native.Report);
        result.Tag.EditRevision.Should().Be(1);
    }

    [Fact]
    public void AnExceptionOutsideCoresGuardIsUnavailableAndNeverLegal()
    {
        var session = SaveFixtures.Open(SaveFixtures.Synthetic(true));
        var draft = session.Select(SaveFixtures.FirstBoxSlot);
        var boom = new InvalidOperationException("secret-detail");
        var service = new LegalityService((_, _, _) => throw boom);

        var result = service.Analyze(session, draft, out var failure);

        result.Verdict.Should().Be(LegalityVerdict.Unavailable);
        result.Findings.Should().BeEmpty();
        result.Report.Should().BeEmpty("no exception text or partial report is shown");
        result.Tag.Should().Be(LegalityTag.Of(draft));
        failure.Should().BeSameAs(boom, "the cause goes to the console");
    }

    [Fact]
    public void OutOfMemoryIsNotSwallowed()
    {
        var session = SaveFixtures.Open(SaveFixtures.Synthetic(true));
        var service = new LegalityService((_, _, _) => throw new OutOfMemoryException());
        var act = () => service.Analyze(session, session.Select(SaveFixtures.FirstBoxSlot), out _);
        act.Should().Throw<OutOfMemoryException>();
    }

    [Fact]
    public void ForeignAndStaleDraftsAreRefused()
    {
        var session = SaveFixtures.Open(SaveFixtures.Synthetic(false));
        var other = SaveFixtures.Open(SaveFixtures.Synthetic(false));
        var foreign = () => LegalityService.Default.Analyze(other, session.Select(SaveFixtures.FirstBoxSlot), out _);
        foreign.Should().Throw<SessionException>().Which.Error.Should().Be(SessionError.ForeignDraft);

        var stale = session.Select(SaveFixtures.FirstBoxSlot);
        var current = session.Select(SaveFixtures.FirstBoxSlot);
        current.EditNickname("Applied", true);
        session.Apply(current);
        var analyzeStale = () => LegalityService.Default.Analyze(session, stale, out _);
        analyzeStale.Should().Throw<SessionException>().Which.Error.Should().Be(SessionError.StaleDraft);
    }
}
