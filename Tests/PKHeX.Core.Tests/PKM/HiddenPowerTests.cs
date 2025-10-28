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

    [Theory]
    [InlineData(15, 15, MoveType.Dark)]
    [InlineData(15, 10, MoveType.Dragon)]
    public void HiddenPowerTestGen2(int atk, int def, MoveType type)
    {
        int expect = (int)type - 1; // no normal type, down-shift by 1
        var pk2 = new PK2 { IV_ATK = atk, IV_DEF = def };
        pk2.HPType.Should().Be(expect);
        HiddenPower.GetTypeGB(pk2.DV16).Should().Be(expect);

        Span<int> ivs = stackalloc int[6];
        pk2.GetIVs(ivs);
        HiddenPower.GetTypeGB(ivs).Should().Be(expect);
    }
}
