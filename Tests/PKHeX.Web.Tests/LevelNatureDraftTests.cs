using FluentAssertions;
using PKHeX.Core;
using PKHeX.Web.Services;
using PKHeX.Web.State;
using Xunit;

namespace PKHeX.Web.Tests;

/// <summary>
/// Drafting the level, experience points and nature (WEB-PKM-005, WEB-PKM-007, WEB-PKM-014): level and experience stay in step through
/// Core's growth curves, the Generation 6 nature changes nothing but itself, refused values are never clamped, and a party member's stats
/// follow the PK6 party-stat policy.
/// </summary>
[Trait(TestCategory.Name, TestCategory.Unit)]
public sealed class LevelNatureDraftTests
{
    private const int Burn = 0x10;

    /// <summary>The known legal Zigzagoon in box 1, slot 1.</summary>
    private static EditorDraft Zigzagoon(bool oras = true) => SaveFixtures.Open(SaveFixtures.Synthetic(oras)).Select(SaveFixtures.FirstBoxSlot);

    /// <summary>A party member at 7 HP with a burn and a stored Attack Core would not calculate, so a heal, a cleared status or a recalculation would show.</summary>
    private static byte[] InjuredAndBurned(bool oras) => SaveFixtures.Synthetic(oras, customize: SaveFixtures.WithPartyMember("Leader", p =>
    {
        p.Stat_HPCurrent = 7;
        p.Status_Condition = Burn;
        p.Stat_ATK = 1;
    }));

    /// <summary>Asserts the draft holds exactly the stored entity changed by <paramref name="nativeEdit"/>, so the edit wrote nothing else.</summary>
    private static void AssertMatchesNative(EditorDraft draft, PK6 stored, Action<PK6> nativeEdit)
    {
        var expected = (PK6)stored.Clone();
        nativeEdit(expected);
        draft.Preview().Data.ToArray().Should().Equal(expected.Data.ToArray());
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void ALevelEditSetsTheFewestExperiencePointsForTheLevel(bool oras)
    {
        var draft = Zigzagoon(oras);
        var stored = draft.Preview();

        draft.EditLevel(50);

        draft.Level.Should().Be(50);
        draft.Experience.Should().Be(125_000, "Zigzagoon grows Medium Fast, which reaches level 50 at 125,000");
        // The native edit sets the experience points only: the party level byte is not stored for a boxed Pokémon.
        AssertMatchesNative(draft, stored, p => p.EXP = 125_000);
    }

    [Fact]
    public void ALevelEditLeavesTheUnstoredPartyBytesOfABoxedPokemonAlone()
    {
        // Core's CurrentLevel setter also writes the party level byte, which would make an unchanged box draft look edited.
        var draft = Zigzagoon();
        var stored = draft.Preview();

        draft.EditLevel(stored.CurrentLevel + 1);
        draft.EditLevel(stored.CurrentLevel);

        draft.Preview().Data[stored.SIZE_STORED..].ToArray().Should().Equal(stored.Data[stored.SIZE_STORED..].ToArray());
    }

    [Theory]
    [InlineData(Species.Zigzagoon, 125_000u, 1_000_000u)]
    [InlineData(Species.Zangoose, 125_000u, 600_000u)]
    [InlineData(Species.Seviper, 142_500u, 1_640_000u)]
    [InlineData(Species.Bulbasaur, 117_360u, 1_059_860u)]
    [InlineData(Species.Clefairy, 100_000u, 800_000u)]
    [InlineData(Species.Dratini, 156_250u, 1_250_000u)]
    public void EachGrowthRateGivesItsOwnExperience(Species species, uint level50, uint maximum)
    {
        var bytes = SaveFixtures.Synthetic(true, customize: SaveFixtures.WithBoxEntity(0, 1, p => p.Species = (ushort)species));
        var draft = SaveFixtures.Open(bytes).Select(SlotRef.InBox(0, 1));

        draft.EditLevel(50);
        draft.Experience.Should().Be(level50);
        draft.Progress.Maximum.Should().Be(maximum);

        draft.EditLevel(100);
        draft.Experience.Should().Be(maximum);
        draft.Progress.NextLevel.Should().BeNull();

        draft.EditLevel(1);
        draft.Experience.Should().Be(0);
        draft.Progress.NextLevel.Should().NotBeNull();
    }

    [Fact]
    public void ReEnteringTheCurrentLevelKeepsTheExperiencePoints()
    {
        var draft = Zigzagoon();
        draft.EditLevel(50);
        draft.EditExperience(125_500);

        draft.EditLevel(50);

        draft.Experience.Should().Be(125_500, "only a changed level resets the experience points to the start of the level");
        draft.Level.Should().Be(50);
    }

    [Fact]
    public void AnExperienceEditMovesTheLevelAlongTheCurve()
    {
        var draft = Zigzagoon();
        var stored = draft.Preview();

        draft.EditExperience(124_999);
        draft.Level.Should().Be(49);
        draft.EditExperience(125_000);
        draft.Level.Should().Be(50);

        AssertMatchesNative(draft, stored, p => p.EXP = 125_000);
        draft.Progress.Should().Be(new LevelProgress(50, 125_000, 125_000, 132_651, 1_000_000));
    }

    [Fact]
    public void TheMaximumExperienceIsAcceptedAndOneMoreIsRefused()
    {
        var draft = Zigzagoon();
        draft.EditExperience(1_000_000);
        draft.Level.Should().Be(100);

        var over = () => draft.EditExperience(1_000_001);

        over.Should().Throw<SessionException>().Which.Error.Should().Be(SessionError.ExperienceOutOfRange);
        draft.Experience.Should().Be(1_000_000, "a refused value is never clamped and leaves the draft unchanged");
        draft.EditRevision.Should().Be(1);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(101)]
    [InlineData(-1)]
    [InlineData(int.MaxValue)]
    public void ALevelOutsideOneToAHundredIsRefusedNotClamped(int level)
    {
        var draft = Zigzagoon();
        var before = draft.Preview().Data.ToArray();

        var edit = () => draft.EditLevel(level);

        edit.Should().Throw<SessionException>().Which.Error.Should().Be(SessionError.LevelOutOfRange);
        draft.Preview().Data.ToArray().Should().Equal(before);
        draft.EditRevision.Should().Be(0);
    }

    [Theory]
    [InlineData(-1L)]
    [InlineData(long.MaxValue)]
    [InlineData(uint.MaxValue + 1L)]
    public void ExperienceOutOfRangeIsRefused(long experience)
    {
        var draft = Zigzagoon();

        var edit = () => draft.EditExperience(experience);

        edit.Should().Throw<SessionException>().Which.Error.Should().Be(SessionError.ExperienceOutOfRange);
        draft.IsDirty.Should().BeFalse();
    }

    [Fact]
    public void StoredExperienceAboveTheCurveReadsAsTheMaximumLevel()
    {
        var bytes = SaveFixtures.Synthetic(true, customize: SaveFixtures.WithBoxEntity(0, 1, p => p.EXP = 5_000_000));
        var draft = SaveFixtures.Open(bytes).Select(SlotRef.InBox(0, 1));

        draft.Level.Should().Be(100);
        draft.Experience.Should().Be(5_000_000, "the stored value is shown as stored");
        draft.Progress.NextLevel.Should().BeNull();

        draft.EditLevel(100);
        draft.Experience.Should().Be(5_000_000, "re-entering the current level keeps the stored value");
        draft.EditLevel(99);
        draft.Level.Should().Be(99);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void ANatureEditChangesOnlyTheNature(bool oras)
    {
        var draft = Zigzagoon(oras);
        var stored = draft.Preview();
        var nature = stored.Nature == Nature.Adamant ? Nature.Modest : Nature.Adamant;

        draft.EditNature((int)nature);

        draft.Nature.Should().Be(nature);
        AssertMatchesNative(draft, stored, p => p.Nature = nature);
        var preview = draft.Preview();
        (preview.PID, preview.EncryptionConstant, preview.Gender, preview.AbilityNumber, preview.IsShiny)
            .Should().Be((stored.PID, stored.EncryptionConstant, stored.Gender, stored.AbilityNumber, stored.IsShiny));
    }

    [Fact]
    public void EveryListedNatureIsAccepted()
    {
        var draft = Zigzagoon();

        foreach (var nature in Enumerable.Range(0, 25))
        {
            draft.EditNature(nature);
            draft.Nature.Should().Be((Nature)nature);
        }
    }

    [Theory]
    [InlineData(25)]
    [InlineData(-1)]
    [InlineData(255)]
    public void ANatureOutsideTheGamesListIsRefused(int nature)
    {
        var draft = Zigzagoon();

        var edit = () => draft.EditNature(nature);

        edit.Should().Throw<SessionException>().Which.Error.Should().Be(SessionError.NatureNotAvailable);
        draft.IsDirty.Should().BeFalse();
    }

    [Fact]
    public void ANatureChangedAndChangedBackLeavesTheDraftClean()
    {
        var draft = Zigzagoon();
        var stored = draft.Nature;

        draft.EditNature((int)(stored == Nature.Adamant ? Nature.Modest : Nature.Adamant));
        draft.EditNature((int)stored);

        draft.IsDirty.Should().BeFalse();
    }

    [Fact]
    public void TheEditsAreGatedOnTheFamilyAndRefusedForEggs()
    {
        var bytes = SaveFixtures.Synthetic(true, customize: SaveFixtures.WithBoxEntity(0, 1, p => p.IsEgg = true));
        var save = SaveFixtures.Parse(bytes);
        var gated = new SaveSession(bytes.ToArray(), save, "fixture.sav", SaveCapabilities.For(save, SupportMatrix.Find(save)! with { Editable = EditableFields.Nickname }))
            .Select(SaveFixtures.FirstBoxSlot);
        var egg = SaveFixtures.Open(bytes).Select(SlotRef.InBox(0, 1));

        foreach (var (draft, error) in new[] { (gated, SessionError.FieldNotEditable), (egg, SessionError.EggNotEditable) })
        {
            Action[] edits = [() => draft.EditLevel(5), () => draft.EditExperience(5), () => draft.EditNature(3)];
            foreach (var edit in edits)
            {
                edit.Should().Throw<SessionException>().Which.Error.Should().Be(error);
            }
            draft.IsDirty.Should().BeFalse();
        }
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void APartyLevelEditFollowsThePolicyAndMatchesNativeCore(bool oras)
    {
        var source = InjuredAndBurned(oras);
        var session = SaveFixtures.Open(source);
        var draft = session.Select(SlotRef.InParty(0));

        draft.EditLevel(2);

        var preview = draft.Preview();
        var expectedStats = preview.GetStats(preview.PersonalInfo);
        new[] { preview.Stat_HPMax, preview.Stat_ATK, preview.Stat_DEF, preview.Stat_SPE, preview.Stat_SPA, preview.Stat_SPD }
            .Should().Equal(expectedStats.Select(s => (int)s), "the stats are recalculated for the new level, the odd stored Attack included");
        preview.Stat_Level.Should().Be(2);
        preview.Stat_HPCurrent.Should().Be(Math.Min(7, (int)expectedStats[0]));
        preview.Status_Condition.Should().Be(Burn);
        session.Apply(draft);

        var output = SaveExporter.Export(session, session.Select(SlotRef.InParty(0)));
        var native = SaveFixtures.Parse(source);
        var pk = native.GetPartySlotAtIndex(0);
        pk.EXP = Experience.GetEXP(2, pk.PersonalInfo.EXPGrowth);
        pk.SetStats(expectedStats);
        pk.Stat_Level = 2;
        pk.Stat_HPCurrent = Math.Min(7, (int)expectedStats[0]);
        native.SetPartySlotAtIndex(pk, 0, EntityImportSettings.None);
        output.Should().Equal(native.Write().ToArray(), "the session must match the native Core edit");
    }

    [Fact]
    public void APartyNatureEditRecalculatesWithoutHealing()
    {
        var session = SaveFixtures.Open(InjuredAndBurned(true));
        var draft = session.Select(SlotRef.InParty(0));
        var stored = draft.Preview();
        var nature = stored.Nature == Nature.Adamant ? Nature.Modest : Nature.Adamant;

        draft.EditNature((int)nature);

        var preview = draft.Preview();
        preview.Stat_ATK.Should().Be(preview.GetStats(preview.PersonalInfo)[1]);
        preview.Stat_HPCurrent.Should().Be(7);
        preview.Status_Condition.Should().Be(Burn);
        draft.HpChange.Should().BeNull("a nature does not change HP");
        draft.Inspect().Stats.Source.Should().Be(StatsSource.Recalculated);
    }

    [Fact]
    public void APartyEditThatLeavesTheCalculationAsItWasKeepsTheStoredBattleState()
    {
        var session = SaveFixtures.Open(InjuredAndBurned(true));
        var draft = session.Select(SlotRef.InParty(0));
        var stored = draft.Preview();
        var nature = stored.Nature == Nature.Adamant ? Nature.Modest : Nature.Adamant;

        // Experience points within the same level change no stat.
        draft.EditExperience(stored.EXP + 1);
        draft.Preview().Stat_ATK.Should().Be(1, "the stored Attack is kept while the calculation is unchanged");
        draft.Inspect().Stats.Source.Should().Be(StatsSource.Stored);

        // A nature changed and changed back returns to the stored calculation, so the stored battle state comes back.
        draft.EditExperience(stored.EXP);
        draft.EditNature((int)nature);
        draft.Preview().Stat_ATK.Should().NotBe(1);
        draft.EditNature((int)stored.Nature);

        draft.IsDirty.Should().BeFalse();
        draft.Preview().Data.ToArray().Should().Equal(stored.Data.ToArray());
    }

    [Fact]
    public void APartyLevelDropIsPreviewedAndAFaintedMemberStaysFainted()
    {
        var healthy = SaveFixtures.Open(SaveFixtures.Synthetic(true, customize: SaveFixtures.WithPartyMember())).Select(SlotRef.InParty(0));
        healthy.EditLevel(2);
        healthy.HpChange!.Value.IsReduction.Should().BeTrue();

        var fainted = SaveFixtures.Open(SaveFixtures.Synthetic(true, customize: SaveFixtures.WithPartyMember(battle: p => p.Stat_HPCurrent = 0)))
            .Select(SlotRef.InParty(0));
        fainted.EditLevel(100);
        fainted.Preview().Stat_HPCurrent.Should().Be(0, "a level gain never revives a fainted member");
    }

    [Fact]
    public void ShedinjaKeepsItsOneHp()
    {
        var draft = SaveFixtures.Open(SaveFixtures.Synthetic(true, customize: SaveFixtures.WithPartyMember(battle: p =>
        {
            p.Species = (ushort)Species.Shedinja;
            p.ResetPartyStats();
        }))).Select(SlotRef.InParty(0));

        draft.EditLevel(100);

        var preview = draft.Preview();
        (preview.Stat_HPCurrent, preview.Stat_HPMax).Should().Be((1, 1));
        draft.HpChange.Should().BeNull();
    }

    [Fact]
    public void TypingThroughALowerLevelDoesNotLowerHpForGood()
    {
        // Retyping 90 as 95 passes through 9 (Backspace, then 5); each keystroke is an edit, and HP clamped at level 9 must not stick.
        var draft = SaveFixtures.Open(SaveFixtures.Synthetic(true, customize: SaveFixtures.WithPartyMember(battle: p =>
        {
            p.EXP = Experience.GetEXP(90, p.PersonalInfo.EXPGrowth) + 5;
            p.ResetPartyStats();
        }))).Select(SlotRef.InParty(0));
        var stored = draft.Preview();
        draft.EditLevel(9);
        draft.EditLevel(95);
        draft.Preview().Stat_HPCurrent.Should().Be(stored.Stat_HPCurrent, "HP is the smaller of the stored HP and the level 95 maximum");
        draft.HpChange!.Value.IsReduction.Should().BeFalse();
    }

    [Fact]
    public void TypingBackToTheStoredLevelRestoresTheStoredExperience()
    {
        // Retyping 90 passes through 9; arriving at 90 again must not reset the experience points to the start of level 90.
        var draft = SaveFixtures.Open(SaveFixtures.Synthetic(true, customize: SaveFixtures.WithBoxEntity(0, 1, p => p.EXP = Experience.GetEXP(90, p.PersonalInfo.EXPGrowth) + 5)))
            .Select(SlotRef.InBox(0, 1));
        draft.EditLevel(9);
        draft.EditLevel(90);
        draft.IsDirty.Should().BeFalse("the stored experience points are restored");
    }
}
