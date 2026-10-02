using FluentAssertions;
using PKHeX.Core;
using PKHeX.Web.Services;
using Xunit;

namespace PKHeX.Web.Tests;

/// <summary>
/// The overview reads each value from Core, and reports a missing or unrecognised value as such (WEB-OVERVIEW-001/002).
/// </summary>
[Trait(TestCategory.Name, TestCategory.Unit)]
public sealed class SaveOverviewTests
{
    /// <summary>A save date Core stores and reads back exactly (minute precision).</summary>
    internal static readonly DateTime SavedAt = new(2024, 5, 6, 7, 8, 0);

    /// <summary>Distinct, recognisable trainer values, so a swapped or misread field cannot pass.</summary>
    internal static void SetKnownTrainer(SaveFile save)
    {
        save.OT = "Serena";
        save.TID16 = 42;
        save.SID16 = 54321;
        save.Language = (int)LanguageID.French;
        save.PlayedHours = 123;
        save.PlayedMinutes = 4;
        save.PlayedSeconds = 5;
        save.Money = 1234567;
        ((SAV6)save).Played.LastSavedDate = SavedAt;
    }

    private static SaveOverview Overview(bool oras, Action<SaveFile> customize, string? fileName = null) =>
        SaveOverview.From(SaveFixtures.Open(SaveFixtures.Synthetic(oras, customize: customize), fileName));

    [Theory]
    [InlineData(false, GameVersion.X, typeof(SAV6XY), SaveUtil.SIZE_G6XY)]
    [InlineData(true, GameVersion.AS, typeof(SAV6AO), SaveUtil.SIZE_G6ORAS)]
    public void EveryValueComesFromTheSave(bool oras, GameVersion version, Type type, int size)
    {
        var overview = Overview(oras, SetKnownTrainer, "backup/main");

        overview.Version.Should().Be(version);
        overview.VersionValid.Should().BeTrue();
        overview.Family.SaveType.Should().Be(type);
        overview.Generation.Should().Be(6);
        overview.TrainerName.Should().Be("Serena");
        overview.Language.Should().Be((int)LanguageID.French);
        overview.LanguageName.Should().Be("FRA (Français)", "Core's own label, as WinForms shows it");
        overview.IdFormat.Should().Be(TrainerIDFormat.SixteenBit);
        overview.DisplayTid.Should().Be(42);
        overview.DisplaySid.Should().Be(54321);
        (overview.PlayedHours, overview.PlayedMinutes, overview.PlayedSeconds).Should().Be((123, 4, 5));
        overview.Money.Should().Be(1234567);
        overview.LastSaved.Should().Be(SavedAt);
        overview.FileName.Should().Be("main", "the overview shows the sanitised name");
        overview.SizeBytes.Should().Be(size);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void BlankTrainerNameIsNotInvented(string name)
    {
        Overview(false, s => s.OT = name).TrainerName.Should().BeNull();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(6)]
    [InlineData(42)]
    public void UnlistedLanguageKeepsItsRawValue(int language)
    {
        var overview = Overview(false, s => s.Language = language);
        overview.LanguageName.Should().BeNull();
        overview.Language.Should().Be(language);
    }

    [Fact]
    public void InvalidLastSavedDateIsNotInvented()
    {
        Overview(false, s => ((SAV6)s).Played.LastSavedDate = null).LastSaved.Should().BeNull();
    }

    [Theory]
    [InlineData(false, GameVersion.OR)]
    [InlineData(true, GameVersion.X)]
    [InlineData(false, (GameVersion)0)]
    public void VersionOutsideTheFamilyIsReportedInvalid(bool oras, GameVersion stored)
    {
        // The loader opens the save (it matches the family by layout), but the stored game is not one of the family's.
        var overview = Overview(oras, s => s.Version = stored);
        overview.Version.Should().Be(stored);
        overview.VersionValid.Should().BeFalse();
    }

    [Fact]
    public void ApplyingAnEntityEditLeavesTheTrainerSummaryUnchanged()
    {
        var session = SaveFixtures.Open(SaveFixtures.Synthetic(false, customize: SetKnownTrainer));
        var before = SaveOverview.From(session);

        var draft = session.Select(SaveFixtures.FirstBoxSlot);
        draft.EditNickname("Overview", true);
        session.Apply(draft);

        session.Revision.Should().Be(1);
        SaveOverview.From(session).Should().Be(before, "entity edits use EntityImportSettings.None, which leaves trainer data alone");
    }
}
