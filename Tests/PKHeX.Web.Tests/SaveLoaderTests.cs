using System.Buffers.Binary;
using FluentAssertions;
using PKHeX.Core;
using PKHeX.Web.Interop;
using PKHeX.Web.Services;
using Xunit;

namespace PKHeX.Web.Tests;

/// <summary>
/// The load pipeline's typed outcomes, one per failure, and the unchanged round trip required at open.
/// </summary>
[Trait(TestCategory.Name, TestCategory.Unit)]
public sealed class SaveLoaderTests
{
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void OpensAnEnabledSaveThatWritesBackUnchanged(bool oras)
    {
        var bytes = SaveFixtures.Synthetic(oras);
        var before = bytes.ToArray();
        var outcome = SaveLoader.Load(bytes, "main");
        outcome.Succeeded.Should().BeTrue();
        outcome.Failure.Should().BeNull();
        outcome.Integrity.Should().BeNull();
        outcome.Recognized.Should().BeNull();
        outcome.Session!.GetOriginalBytes().Should().Equal(before);
        bytes.Should().Equal(before, "parsing must not mutate caller bytes");
    }

    [Fact]
    public void EmptyAndOversizedInputAreRefusedBeforeParsing()
    {
        var parsed = false;
        SaveFile? Parse(byte[] _)
        {
            parsed = true;
            return null;
        }

        Load([], Parse).Failure.Should().Be(LoadFailure.Empty);
        Load(new byte[SaveLoader.MaxInputBytes + 1], Parse).Failure.Should().Be(LoadFailure.TooLarge);
        parsed.Should().BeFalse();
    }

    public static TheoryData<string> UnrecognizedInputs => ["zeros", "zero-filled ORAS size", "truncated XY"];

    [Theory]
    [MemberData(nameof(UnrecognizedInputs))]
    public void UnknownDataIsUnrecognizedNotCorrupt(string name)
    {
        var bytes = name switch
        {
            "zeros" => new byte[512],
            "zero-filled ORAS size" => new byte[SaveUtil.SIZE_G6ORAS],
            _ => SaveFixtures.Synthetic(false)[..^1],
        };
        var outcome = SaveLoader.Load(bytes);
        outcome.Failure.Should().Be(LoadFailure.Unrecognized);
        outcome.Recognized.Should().BeNull("nothing was recognised, so nothing may be claimed about the file");
        outcome.Integrity.Should().BeNull();
    }

    [Fact]
    public void RecognizedSaveOfAnotherFamilyNamesWhatItWasRecognisedAs()
    {
        var outcome = SaveLoader.Load(new SAV5BW().Write().ToArray());
        outcome.Failure.Should().Be(LoadFailure.RecognizedNotEnabled);
        outcome.Recognized.Should().Be(new RecognizedSave(typeof(SAV5BW), outcome.Recognized!.Version, 5));
        outcome.Session.Should().BeNull();
    }

    [Fact]
    public void OrasDemoIsRecognizedButNotEnabled()
    {
        var bytes = new SAV6AODemo().Write().ToArray();
        // The same test-only container marker as SaveFixtures.Synthetic, so Core recognises the blank demo save.
        BinaryPrimitives.WriteUInt32LittleEndian(bytes.AsSpan(bytes.Length - 0x1F0), 0x42454546);
        SaveUtil.GetSaveFile(bytes.ToArray()).Should().BeOfType<SAV6AODemo>("Core recognises the file");
        var outcome = SaveLoader.Load(bytes);
        outcome.Failure.Should().Be(LoadFailure.RecognizedNotEnabled);
        outcome.Recognized!.SaveType.Should().Be<SAV6AODemo>();
    }

    [Fact]
    public void BrokenChecksumIsAnIntegrityFailure()
    {
        var outcome = SaveLoader.Load(ProofPage.Corrupt(SaveFixtures.Synthetic(false)));
        outcome.Failure.Should().Be(LoadFailure.IntegrityFailed);
        outcome.Integrity.Should().Be(IntegrityProblem.ChecksumsInvalid);
        outcome.Recognized!.SaveType.Should().Be<SAV6XY>();
    }

    [Fact]
    public void SaveCoreCannotWriteBackIsAnIntegrityFailure()
    {
        // Core marks only blank saves built without data as not exportable, never a parsed one, so the check is reached through the parse seam.
        var blank = new SAV6XY();
        blank.State.Exportable.Should().BeFalse();
        var outcome = Load(SaveFixtures.Synthetic(false), _ => blank);
        outcome.Failure.Should().Be(LoadFailure.IntegrityFailed);
        outcome.Integrity.Should().Be(IntegrityProblem.NotExportable);
    }

    [Fact]
    public void SaveThatDoesNotWriteBackUnchangedIsAnIntegrityFailure()
    {
        // Core writes valid XY/ORAS saves back exactly, so the mismatch is injected through the write step.
        var bytes = SaveFixtures.Synthetic(false);
        var outcome = SaveLoader.Load(bytes, null, data => SaveUtil.GetSaveFile(data), save =>
        {
            var written = save.Clone().Write().ToArray();
            written[^1] ^= 1;
            return written;
        });
        outcome.Failure.Should().Be(LoadFailure.IntegrityFailed);
        outcome.Integrity.Should().Be(IntegrityProblem.RoundTripMismatch);
        outcome.Recognized!.SaveType.Should().Be<SAV6XY>();
    }

    [Fact]
    public void RoundTripWriteDoesNotChangeTheSave()
    {
        // Break the checksums first: writing the save itself would then refresh them in its buffer, which the comparison must not do.
        var save = SaveFixtures.Parse(ProofPage.Corrupt(SaveFixtures.Synthetic(false)));
        save.ChecksumsValid.Should().BeFalse();
        var before = save.Data.ToArray();

        var written = SaveLoader.WriteForComparison(save);
        save.Data.ToArray().Should().Equal(before, "the comparison writes a clone, not the save");
        save.ChecksumsValid.Should().BeFalse();
        written.ToArray().Should().Equal(save.Clone().Write().ToArray(), "it is exactly what Core would export");
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void CoreExceptionsAreParserFaultsWithoutTheirDetails(bool inParse)
    {
        const string secret = "private detail";
        var bytes = SaveFixtures.Synthetic(false);
        var before = bytes.ToArray();
        var outcome = SaveLoader.Load(bytes, null,
            data => inParse ? throw new InvalidOperationException(secret) : SaveUtil.GetSaveFile(data),
            _ => throw new IndexOutOfRangeException(secret));
        outcome.Failure.Should().Be(LoadFailure.ParserFault);
        outcome.Session.Should().BeNull();
        outcome.ToString().Should().NotContain(secret);
        bytes.Should().Equal(before);
    }

    [Theory]
    [InlineData(FileReadStatus.Empty, LoadFailure.Empty)]
    [InlineData(FileReadStatus.TooLarge, LoadFailure.TooLarge)]
    [InlineData(FileReadStatus.ReadFailed, LoadFailure.ReadFailed)]
    public void FailedReadsKeepTheirReason(FileReadStatus status, LoadFailure expected)
    {
        FileReadResult.Failed(status, "main").Open().Failure.Should().Be(expected);
    }

    [Fact]
    public void SuccessfulReadIsLoadedUnderItsName()
    {
        var outcome = new FileReadResult(FileReadStatus.Ok, "backup", SaveFixtures.Synthetic(true)).Open();
        outcome.Session!.FileName.Should().Be("backup");
        outcome.Session.Working.Should().BeOfType<SAV6AO>();
    }

    [Theory]
    [InlineData(LoadFailure.RecognizedNotEnabled)]
    [InlineData(LoadFailure.IntegrityFailed)]
    public void FailuresThatDescribeASaveNeedIt(LoadFailure failure)
    {
        var act = () => SaveLoadOutcome.Failed(failure);
        act.Should().Throw<ArgumentException>();
    }

    private static SaveLoadOutcome Load(byte[] bytes, Func<byte[], SaveFile?> parse) => SaveLoader.Load(bytes, null, parse, SaveLoader.WriteForComparison);
}
