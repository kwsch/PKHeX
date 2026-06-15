using System;
using FluentAssertions;
using Xunit;
using TR = PKHeX.Core.SimpleTrainerInfo;

namespace PKHeX.Core.Tests.Simulator;

public class MysteryGiftIVTests
{
    [Fact]
    public void FlawlessMysteryGiftTemplatePrefersTemplateOverConflictingCriteria()
    {
        var gift = new WA9
        {
            IsEntity = true,
            Species = (ushort)Species.Bulbasaur,
            Level = 5,
            MetLevel = 5,
            CardID = 9001,
            IV_HP = 0xFE,
            IV_ATK = 0xFF,
            IV_DEF = 0xFF,
            IV_SPE = 0xFF,
            IV_SPA = 0xFF,
            IV_SPD = 0xFF,
        };

        var criteria = EncounterCriteria.Unrestricted with
        {
            IV_HP = 1,
            IV_ATK = 2,
            IV_DEF = 3,
            IV_SPE = 4,
            IV_SPA = 5,
            IV_SPD = 6,
        };

        var trainer = new TR(GameVersion.ZA);
        var pk = gift.ConvertToPKM(trainer, criteria);

        pk.FlawlessIVCount.Should().BeGreaterThanOrEqualTo(3);
        Span<int> ivs = stackalloc int[6];
        pk.GetIVs(ivs);
        ivs.ToArray().Should().Contain(31);
    }

    [Fact]
    public void RandomMysteryGiftTemplateUsesRequestedCriteriaIVs()
    {
        var gift = new WC9
        {
            IsEntity = true,
            Species = (ushort)Species.Bulbasaur,
            Level = 5,
            MetLevel = 5,
            IV_HP = 0xFF,
            IV_ATK = 0xFF,
            IV_DEF = 0xFF,
            IV_SPE = 0xFF,
            IV_SPA = 0xFF,
            IV_SPD = 0xFF,
        };

        var criteria = EncounterCriteria.Unrestricted with
        {
            IV_HP = 1,
            IV_ATK = 2,
            IV_DEF = 3,
            IV_SPE = 4,
            IV_SPA = 5,
            IV_SPD = 6,
        };

        var trainer = new TR(GameVersion.SL);
        var pk = gift.ConvertToPKM(trainer, criteria);

        pk.IV_HP.Should().Be(1);
        pk.IV_ATK.Should().Be(2);
        pk.IV_DEF.Should().Be(3);
        pk.IV_SPE.Should().Be(4);
        pk.IV_SPA.Should().Be(5);
        pk.IV_SPD.Should().Be(6);
    }
}
