using FluentAssertions;
using PKHeX.Core;
using PKHeX.Web.Components;
using PKHeX.Web.Interop;
using PKHeX.Web.Services;
using PKHeX.Web.State;
using Xunit;

namespace PKHeX.Web.Tests;

/// <summary>
/// Every typed outcome and refusal has its own text, and none of it overclaims.
/// </summary>
[Trait(TestCategory.Name, TestCategory.Unit)]
public sealed class UserMessagesTests
{
    private static readonly RecognizedSave XY = new(typeof(SAV6XY), GameVersion.X, 6);

    /// <summary>One outcome for every failure and integrity problem.</summary>
    private static IEnumerable<SaveLoadOutcome> Failures()
    {
        foreach (var failure in Enum.GetValues<LoadFailure>())
        {
            if (failure == LoadFailure.RecognizedNotEnabled)
            {
                yield return SaveLoadOutcome.NotEnabled(new RecognizedSave(typeof(SAV5BW), GameVersion.BW, 5));
            }
            else if (failure != LoadFailure.IntegrityFailed)
            {
                yield return SaveLoadOutcome.Failed(failure);
            }
        }
        foreach (var problem in Enum.GetValues<IntegrityProblem>())
        {
            yield return SaveLoadOutcome.IntegrityFailed(XY, problem);
        }
    }

    [Fact]
    public void EveryLoadFailureHasDistinctText()
    {
        var texts = Failures().Select(UserMessages.For).ToList();
        texts.Should().HaveCount(Enum.GetValues<LoadFailure>().Length - 1 + Enum.GetValues<IntegrityProblem>().Length);
        texts.Should().OnlyHaveUniqueItems().And.AllSatisfy(AssertHonest);
    }

    [Fact]
    public void EverySessionErrorAndDropRejectionHasDistinctText()
    {
        var errors = Enum.GetValues<SessionError>().Select(UserMessages.For).ToList();
        errors.Should().OnlyHaveUniqueItems().And.AllSatisfy(AssertHonest).And.NotContain(UserMessages.OperationFailed);
        var drops = Enum.GetValues<DropRejection>().Select(UserMessages.For).ToList();
        drops.Should().OnlyHaveUniqueItems().And.AllSatisfy(AssertHonest);
    }

    public static TheoryData<string, string> ExactLoadTexts => new()
    {
        { "Empty", "The file is empty." },
        { "TooLarge", "The file exceeds the 16 MiB limit." },
        { "ReadFailed", "The file could not be read. Choose it again." },
        { "Unrecognized", "This file was not recognised as a save this release can open. This release opens only raw Pokémon X and Y or Pokémon Omega Ruby and Alpha Sapphire saves. Use the decrypted main file exported with a save manager on the console, or taken from an emulator." },
        { "BW", "This looks like a Generation 5 save. This release opens only raw Pokémon X and Y or Pokémon Omega Ruby and Alpha Sapphire saves." },
        { "Demo", "This looks like a save from the Omega Ruby/Alpha Sapphire special demo. This release opens only raw Pokémon X and Y or Pokémon Omega Ruby and Alpha Sapphire saves." },
        { "NotExportable", "This Pokémon X and Y save is marked by PKHeX as not exportable, so it was not opened. Nothing was repaired or changed." },
        { "ChecksumsInvalid", "This Pokémon X and Y save failed its checksum validation, so it was not opened. Nothing was repaired or changed." },
        { "RoundTripMismatch", "This Pokémon X and Y save would not be reproduced exactly when written back, even without edits, so it was not opened. Nothing was repaired or changed." },
        { "ParserFault", "The file could not be processed. It was not opened, and nothing was changed." },
    };

    /// <summary>Pins the wording itself, so the E2E tier, which derives its expected text from this mapping, cannot pass on a wrong mapping.</summary>
    [Theory]
    [MemberData(nameof(ExactLoadTexts))]
    public void LoadTextIsExact(string name, string expected)
    {
        var outcome = name switch
        {
            "BW" => SaveLoadOutcome.NotEnabled(new RecognizedSave(typeof(SAV5BW), GameVersion.BW, 5)),
            "Demo" => SaveLoadOutcome.NotEnabled(new RecognizedSave(typeof(SAV6AODemo), GameVersion.AS, 6)),
            _ when Enum.TryParse<IntegrityProblem>(name, out var problem) => SaveLoadOutcome.IntegrityFailed(XY, problem),
            _ => SaveLoadOutcome.Failed(Enum.Parse<LoadFailure>(name)),
        };
        UserMessages.For(outcome).Should().Be(expected);
    }

    [Fact]
    public void RefusalsSayOnlyWhatWasEstablished()
    {
        UserMessages.For(SaveLoadOutcome.Failed(LoadFailure.Unrecognized)).Should().NotContainAny("corrupt", "damaged", "Generation");
        UserMessages.For(SaveLoadOutcome.NotEnabled(new RecognizedSave(typeof(SAV5BW), GameVersion.BW, 5))).Should().StartWith("This looks like a Generation 5 save.");
        UserMessages.For(SaveLoadOutcome.NotEnabled(new RecognizedSave(typeof(SAV6AODemo), GameVersion.AS, 6))).Should().Contain("special demo");
        UserMessages.For(SaveLoadOutcome.IntegrityFailed(XY, IntegrityProblem.ChecksumsInvalid)).Should().Contain("Pokémon X and Y").And.Contain("Nothing was repaired");
    }

    [Fact]
    public void SuccessHasNoFailureText()
    {
        var act = () => UserMessages.For(SaveLoader.Load(SaveFixtures.Synthetic(false)));
        act.Should().Throw<ArgumentException>();
    }

    private static void AssertHonest(string text)
    {
        text.Should().NotBeNullOrWhiteSpace();
        text.Should().NotContainEquivalentOf("supported", "no family is qualified as supported yet");
    }
}
