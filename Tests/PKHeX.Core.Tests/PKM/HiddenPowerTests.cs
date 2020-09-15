using System;
using FluentAssertions;
using PKHeX.Core;
using Xunit;

namespace PKHeX.Tests.PKM
{
    public class HiddenPowerTests
    {
        [Theory]
        [InlineData(14, 15, 15, 14, 14, 15, MoveType.Dark, 69, typeof(PK2))]
        [InlineData(30, 31, 31, 30, 31, 31, MoveType.Grass, 70, typeof(PK3))]
        [InlineData(26, 31, 31, 30, 31, 31, MoveType.Grass, 70, typeof(PK3))]
        public void HiddenPowerTest(int h, int a, int b, int c, int d, int s, MoveType type, int power, Type pkmType)
        {
            var pkm = PKMConverter.GetBlank(pkmType);
            pkm.IV_HP = h;
            pkm.IV_ATK = a;
            pkm.IV_DEF = b;
            pkm.IV_SPA = c;
            pkm.IV_SPD = d;
            pkm.IV_SPE = s;

            pkm.HPType.Should().Be((int)type - 1); // no normal type, down-shift by 1
            pkm.HPPower.Should().Be(power);
        }
    }
}
