using System;
using FluentAssertions;
using Xunit;

namespace PKHeX.Core.Tests.PKM;

public class HiddenPowerTests
{
    [Theory]
    [InlineData(14, 15, 15, 14, 14, 15, MoveType.Dark, 69, typeof(PK2))]
    [InlineData(30, 31, 31, 30, 31, 31, MoveType.Grass, 70, typeof(PK3))]
    [InlineData(26, 31, 31, 30, 31, 31, MoveType.Grass, 70, typeof(PK3))]
    public void HiddenPowerTest(int h, int a, int b, int c, int d, int s, MoveType type, int power, Type pkmType)
    {
        var pk = EntityBlank.GetBlank(pkmType);
        pk.IV_HP = h;
        pk.IV_ATK = a;
        pk.IV_DEF = b;
        pk.IV_SPA = c;
        pk.IV_SPD = d;
        pk.IV_SPE = s;

        pk.HPType.Should().Be((int)type - 1); // no normal type, down-shift by 1
        pk.HPPower.Should().Be(power);
    }
}
