using FluentAssertions;
using PKHeX.Core;
using PKHeX.Web.State;
using Xunit;

namespace PKHeX.Web.Tests;

/// <summary>
/// The PK6 party-stat policy (<c>PKHeX.Web.md</c> §State model): stats are recalculated through Core, the status is kept, and current HP
/// is never raised, so an edit neither heals nor revives.
/// </summary>
[Trait(TestCategory.Name, TestCategory.Unit)]
public sealed class PartyStatPolicyTests
{
    /// <summary>The known legal PK6 in party format, with its stats calculated by Core.</summary>
    private static PK6 Member(Action<PK6>? battle = null)
    {
        var pk = new PK6(SaveFixtures.ReadEntity(true));
        pk.ResetPartyStats();
        battle?.Invoke(pk);
        return pk;
    }

    /// <summary>Core's stats for <paramref name="pk"/> as it is now: the oracle the policy must agree with.</summary>
    private static ushort[] CoreStats(PK6 pk) => pk.GetStats(pk.PersonalInfo);

    private static ushort[] StoredStats(PK6 pk) => [(ushort)pk.Stat_HPMax, (ushort)pk.Stat_ATK, (ushort)pk.Stat_DEF, (ushort)pk.Stat_SPE, (ushort)pk.Stat_SPA, (ushort)pk.Stat_SPD];

    [Fact]
    public void ALowerLevelRecalculatesStatsAndClampsHpToTheNewMaximum()
    {
        var pk = Member();
        var previousHp = pk.Stat_HPCurrent;
        pk.CurrentLevel = 2;

        PartyStatPolicy.Recalculate(pk);

        StoredStats(pk).Should().Equal(CoreStats(pk));
        pk.Stat_Level.Should().Be(2);
        pk.Stat_HPMax.Should().BeLessThan(previousHp, "the fixture must actually lower the maximum");
        pk.Stat_HPCurrent.Should().Be(pk.Stat_HPMax);
    }

    [Fact]
    public void AnInjuredMemberIsNotHealedByAHigherLevel()
    {
        var pk = Member(p => p.Stat_HPCurrent = 3);
        var previousMax = pk.Stat_HPMax;
        pk.CurrentLevel = 100;

        PartyStatPolicy.Recalculate(pk);

        pk.Stat_HPMax.Should().BeGreaterThan(previousMax);
        pk.Stat_HPCurrent.Should().Be(3, "HP is the smaller of the previous HP and the new maximum");
        pk.Stat_Level.Should().Be(100);
        StoredStats(pk).Should().Equal(CoreStats(pk));
    }

    [Fact]
    public void AFaintedMemberStaysFainted()
    {
        var pk = Member(p => p.Stat_HPCurrent = 0);
        pk.CurrentLevel = 100;

        PartyStatPolicy.Recalculate(pk);

        pk.Stat_HPCurrent.Should().Be(0);
        pk.Stat_HPMax.Should().BeGreaterThan(0);
    }

    [Theory]
    [InlineData(1)] // Sleep, one turn left.
    [InlineData(0x10)] // Burn in the Generation 5+ status encoding.
    [InlineData(0x40)] // Paralysis.
    public void TheStatusIsKept(int status)
    {
        var pk = Member(p => p.Status_Condition = status);
        pk.CurrentLevel = 50;

        PartyStatPolicy.Recalculate(pk);

        pk.Status_Condition.Should().Be(status);
    }

    [Theory]
    [InlineData(1, 1)]
    [InlineData(0, 0)]
    public void AOneHpSpeciesKeepsAtMostOneHp(int hp, int expected)
    {
        var pk = Member(p =>
        {
            p.Species = (ushort)Species.Shedinja;
            p.ResetPartyStats();
            p.Stat_HPCurrent = hp;
        });
        pk.CurrentLevel = 80;

        PartyStatPolicy.Recalculate(pk);

        pk.Stat_HPMax.Should().Be(1);
        pk.Stat_HPCurrent.Should().Be(expected);
    }

    [Fact]
    public void MovePpIsNotRefilled()
    {
        var pk = Member(p =>
        {
            p.Move1_PP = 1;
            p.Move2_PP = 0;
        });
        pk.CurrentLevel = 60;

        PartyStatPolicy.Recalculate(pk);

        pk.Move1_PP.Should().Be(1);
        pk.Move2_PP.Should().Be(0);
    }

    [Fact]
    public void OnlyBattleStateChanges()
    {
        var pk = Member(p => p.Stat_HPCurrent = 5);
        pk.EV_ATK = 252;
        var before = pk.Clone();

        PartyStatPolicy.Recalculate(pk);

        pk.Data[..pk.SIZE_STORED].ToArray().Should().Equal(before.Data[..pk.SIZE_STORED].ToArray(), "the stored format is never touched");
        pk.Stat_ATK.Should().BeGreaterThan(before.Stat_ATK);
    }

    [Fact]
    public void AMemberWithoutStatsIsNotRecalculated()
    {
        var pk = new PK6(SaveFixtures.ReadEntity(true));
        pk.PartyStatsPresent.Should().BeFalse("the fixture is stored-format data");
        var before = pk.Data.ToArray();

        var act = () => PartyStatPolicy.Recalculate(pk);

        act.Should().Throw<ArgumentException>();
        pk.Data.ToArray().Should().Equal(before);
    }

    [Fact]
    public void HpChangeIsNullWhenNeitherCurrentNorMaximumChanges()
    {
        var pk = Member();
        PartyHpChange.Between(pk, pk.Clone()).Should().BeNull();
    }

    [Fact]
    public void HpChangeReportsAReduction()
    {
        var stored = Member();
        var drafted = (PK6)stored.Clone();
        drafted.CurrentLevel = 2;
        PartyStatPolicy.Recalculate(drafted);

        var change = PartyHpChange.Between(stored, drafted);

        change.Should().Be(new PartyHpChange(stored.Stat_HPCurrent, drafted.Stat_HPCurrent, stored.Stat_HPMax, drafted.Stat_HPMax));
        change!.Value.IsReduction.Should().BeTrue();
        change.Value.IsFainted.Should().BeFalse();
    }

    [Fact]
    public void AHigherMaximumAloneIsNotAReduction()
    {
        var stored = Member(p => p.Stat_HPCurrent = 3);
        var drafted = (PK6)stored.Clone();
        drafted.CurrentLevel = 100;
        PartyStatPolicy.Recalculate(drafted);

        var change = PartyHpChange.Between(stored, drafted);

        change.Should().NotBeNull();
        change!.Value.IsReduction.Should().BeFalse();
        change.Value.NewMax.Should().BeGreaterThan(change.Value.PreviousMax);
    }

    [Fact]
    public void AnEditThatLeavesTheCalculationAsItWasKeepsTheStoredBattleState()
    {
        var stored = Member(p =>
        {
            p.Stat_HPCurrent = 7;
            p.Status_Condition = 0x10;
            p.Stat_ATK = 1;
        });
        var candidate = (PK6)stored.Clone();
        candidate.EXP++;

        PartyStatPolicy.AfterStatEdit(candidate, stored);

        candidate.Data[stored.SIZE_STORED..].ToArray().Should().Equal(stored.Data[stored.SIZE_STORED..].ToArray(), "the stats and level Core calculates are unchanged");
    }

    [Fact]
    public void AnEditThatChangesTheCalculationRecalculates()
    {
        var stored = Member(p =>
        {
            p.Stat_HPCurrent = 7;
            p.Status_Condition = 0x10;
            p.Stat_ATK = 1;
        });
        var candidate = (PK6)stored.Clone();
        candidate.Nature = stored.Nature == Nature.Adamant ? Nature.Modest : Nature.Adamant;

        PartyStatPolicy.AfterStatEdit(candidate, stored);

        candidate.Stat_ATK.Should().Be(candidate.GetStats(candidate.PersonalInfo)[1]);
        candidate.Stat_HPCurrent.Should().Be(7);
        candidate.Status_Condition.Should().Be(0x10);
    }

    [Fact]
    public void ALevelChangeSetsTheStoredPartyLevel()
    {
        var stored = Member(p => p.Stat_ATK = 1);
        var candidate = (PK6)stored.Clone();
        candidate.EXP = Experience.GetEXP((byte)(stored.CurrentLevel + 1), stored.PersonalInfo.EXPGrowth);

        PartyStatPolicy.AfterStatEdit(candidate, stored);

        candidate.Stat_Level.Should().Be((byte)(stored.CurrentLevel + 1));
    }
}
