using FluentAssertions;
using PKHeX.Core;
using PKHeX.Web.Services;
using PKHeX.Web.State;
using Xunit;

namespace PKHeX.Web.Tests;

/// <summary>
/// Session, draft and export rules, run on synthetic saves.
/// </summary>
[Trait(TestCategory.Name, TestCategory.Unit)]
public sealed class SessionTests
{
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void SyntheticDraftCancelApplyAndRoundTrip(bool oras)
    {
        var source = SaveFixtures.Synthetic(oras);
        var before = source.ToArray();
        var session = SaveLoader.Load(source);
        source.Should().Equal(before, "parsing must not mutate caller bytes");
        session.GetOriginalBytes().Should().Equal(before);

        var draft = session.Select(0);
        draft.EditNickname("WASM Proof", true);
        draft.IsDirty.Should().BeTrue();
        var exportDirty = () => SaveExporter.Export(session, draft);
        exportDirty.Should().Throw<InvalidDataException>();

        draft = session.Select(draft.SlotIndex);
        draft.IsDirty.Should().BeFalse();
        session.HasChangesSinceOpen.Should().BeFalse();

        draft.EditNickname("WASM Proof", true);
        session.Apply(draft);
        var output = SaveExporter.Export(session, session.Select(draft.SlotIndex));
        var reloaded = SaveLoader.Load(output).Select(0);
        reloaded.Nickname.Should().Be("WASM Proof");
        reloaded.IsNicknamed.Should().BeTrue();
        source.Should().Equal(before);

        var native = SaveFixtures.Parse(before);
        var pk = native.GetBoxSlotAtIndex(0);
        pk.Nickname = "WASM Proof";
        pk.IsNicknamed = true;
        native.SetBoxSlotAtIndex(pk, 0, EntityImportSettings.None);
        output.Should().Equal(native.Write().ToArray(), "the session must match the native Core edit");
    }

    [Fact]
    public void RejectsInvalidInputsAndInvalidDraftWithoutMutation()
    {
        var loadEmpty = () => SaveLoader.Load([]);
        loadEmpty.Should().Throw<InvalidDataException>();
        var loadTooLarge = () => SaveLoader.Load(new byte[SaveLoader.MaxInputBytes + 1]);
        loadTooLarge.Should().Throw<InvalidDataException>();
        var loadUnrecognized = () => SaveLoader.Load(new byte[512]);
        loadUnrecognized.Should().Throw<InvalidDataException>();
        var corrupt = SaveFixtures.Synthetic(false);
        corrupt[0] ^= 1;
        var loadCorrupt = () => SaveLoader.Load(corrupt);
        loadCorrupt.Should().Throw<InvalidDataException>();

        var session = SaveLoader.Load(SaveFixtures.Synthetic(false));
        var draft = session.Select(0);
        var editTooLong = () => draft.EditNickname(new string('a', 13), true);
        editTooLong.Should().Throw<InvalidDataException>();
        draft.IsDirty.Should().BeFalse();
        session.HasChangesSinceOpen.Should().BeFalse();
    }

    [Fact]
    public void RejectsRecognizedSaveThatIsNotEnabled()
    {
        var bw = new SAV5BW().Write().ToArray();
        SaveUtil.GetSaveFile(bw.ToArray()).Should().BeOfType<SAV5BW>("Core recognises the file");
        var act = () => SaveLoader.Load(bw);
        act.Should().Throw<InvalidDataException>();
    }

    [Theory]
    [InlineData("Bad\nName")]
    [InlineData("\u0007")]
    [InlineData("\uE08E")] // Stored as-is but read back as '♂', so the text would change.
    public void RejectsNicknameThatCannotBeStoredUnchanged(string nickname)
    {
        var draft = SaveLoader.Load(SaveFixtures.Synthetic(false)).Select(0);
        var before = draft.Nickname;
        var act = () => draft.EditNickname(nickname, true);
        act.Should().Throw<InvalidDataException>();
        draft.Nickname.Should().Be(before);
        draft.IsDirty.Should().BeFalse();
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void KnownEntityLegalityIsPreserved(bool legal)
    {
        var native = new LegalityAnalysis(new PK6(SaveFixtures.ReadEntity(legal)));
        native.Parsed.Should().BeTrue();
        native.Valid.Should().Be(legal);

        var session = SaveLoader.Load(SaveFixtures.Synthetic(true, legal));
        session.Select(0).Analyze(session).Verdict.Should().Be(legal ? "Valid" : "Invalid");
    }

    [Fact]
    public void RevisionAdvancesOnlyOnRealApply()
    {
        var session = SaveLoader.Load(SaveFixtures.Synthetic(false));
        session.Revision.Should().Be(0);

        session.Apply(session.Select(0));
        session.Revision.Should().Be(0);
        session.HasChangesSinceOpen.Should().BeFalse();

        var draft = session.Select(0);
        draft.EditNickname("Changed", true);
        session.Apply(draft);
        session.Revision.Should().Be(1);
        session.HasChangesSinceOpen.Should().BeTrue();
    }

    [Fact]
    public void ApplyWritesOnlyEditedFields()
    {
        var session = SaveLoader.Load(SaveFixtures.Synthetic(false));
        var expected = (PK6)session.Working.GetBoxSlotAtIndex(0);
        var draft = session.Select(0);
        draft.EditNickname("Changed", true);
        session.Apply(draft);

        expected.Nickname = "Changed";
        expected.IsNicknamed = true;
        expected.RefreshChecksum();
        session.Working.GetBoxSlotAtIndex(0).Data.ToArray().Should().Equal(expected.Data.ToArray());
    }

    [Fact]
    public void MarkExportedRecordsOnlyRevisionsOfTheSession()
    {
        var session = SaveLoader.Load(SaveFixtures.Synthetic(false));
        SaveExporter.Export(session, null);
        session.ExportedRevision.Should().BeNull("producing the bytes is not a download");

        session.MarkExported(session.Revision);
        session.ExportedRevision.Should().Be(0);

        var draft = session.Select(0);
        draft.EditNickname("Changed", true);
        session.Apply(draft);
        session.ExportedRevision.Should().Be(0);
        session.MarkExported(session.Revision);
        session.ExportedRevision.Should().Be(1);

        var markFuture = () => session.MarkExported(2);
        markFuture.Should().Throw<ArgumentOutOfRangeException>();
        var markNegative = () => session.MarkExported(-1);
        markNegative.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void StaleDraftIsRejectedUnlessClean()
    {
        var session = SaveLoader.Load(SaveFixtures.Synthetic(false));
        var stale = session.Select(0);
        var current = session.Select(0);
        current.EditNickname("First", true);
        session.Apply(current);
        var working = session.Working;

        var exportStale = () => SaveExporter.Export(session, stale);
        exportStale.Should().Throw<InvalidDataException>();
        var analyzeStale = () => stale.Analyze(session);
        analyzeStale.Should().Throw<InvalidDataException>();
        session.Apply(stale);
        session.Revision.Should().Be(1, "a clean stale draft has nothing to apply");

        stale.EditNickname("Second", true);
        var applyStale = () => session.Apply(stale);
        applyStale.Should().Throw<InvalidDataException>();
        session.Working.Should().BeSameAs(working);
        session.Revision.Should().Be(1);
        session.Select(0).Nickname.Should().Be("First");
    }

    [Fact]
    public void ForeignSessionDraftIsRejectedWithoutMutation()
    {
        var owner = SaveLoader.Load(SaveFixtures.Synthetic(false));
        var other = SaveLoader.Load(SaveFixtures.Synthetic(false, legal: false));
        other.SessionId.Should().NotBe(owner.SessionId);
        var foreign = owner.Select(0);
        foreign.EditNickname("Foreign", true);
        var working = other.Working;
        var nickname = other.Select(0).Nickname;

        var applyForeign = () => other.Apply(foreign);
        applyForeign.Should().Throw<InvalidDataException>();
        var analyzeForeign = () => foreign.Analyze(other);
        analyzeForeign.Should().Throw<InvalidDataException>();
        var exportForeign = () => SaveExporter.Export(other, owner.Select(0));
        exportForeign.Should().Throw<InvalidDataException>();
        other.Working.Should().BeSameAs(working);
        other.Revision.Should().Be(0);
        other.HasChangesSinceOpen.Should().BeFalse();
        other.Select(0).Nickname.Should().Be(nickname);
    }
}
