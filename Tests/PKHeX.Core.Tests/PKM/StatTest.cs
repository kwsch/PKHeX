using FluentAssertions;
using PKHeX.Core;
using Xunit;

namespace PKHeX.Tests.PKM
{
    public class StatTest
    {
        [Fact]
        public void CalcStatsGB()
        {
            var pk = new PK2
            {
                Species = (int) Species.Gyarados,
                CurrentLevel = 38,
                IV_HP = 0,
                IV_ATK = 14,
                IV_DEF = 10,
                IV_SPC = 10,
                IV_SPE = 10,
                EV_HP = 5120, // 2 HP Ups

                Move1 = 45, // Growl (PP 40)
                Move2 = 45, // Growl (PP 40)
                Move3 = 45, // Growl (PP 40)
                Move1_PPUps = 1,
                Move2_PPUps = 2,
                Move3_PPUps = 3,
            };

            pk.ResetPartyStats();
            pk.Stat_Level.Should().Be(pk.CurrentLevel, "stat level");
            pk.Stat_HPCurrent.Should().Be(127, "stat re-calculation");

            pk.HealPP();
            pk.Move1_PP.Should().Be(47, "pp calc oddity");
            pk.Move2_PP.Should().Be(54, "pp calc oddity");
            pk.Move3_PP.Should().Be(61, "pp calc oddity");
        }
    }

    public class BelugaTests
    {
        [Theory]
        [InlineData(41, 25, 91)]
        public void CalculateCP(int level, int statSum, int expect)
        {
            var result1 = (((level * 4.0f / 100.0f) + 2.0f) * (statSum & 0xFFFF));
            var result2 = (int)result1;

            result2.Should().Be(expect);
        }
    }
}
