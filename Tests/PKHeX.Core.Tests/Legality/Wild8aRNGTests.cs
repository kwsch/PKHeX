using FluentAssertions;
using Xunit;

namespace PKHeX.Core.Tests.Legality;

public static class Wild8aRNGTests
{
    [Fact]
    public static void TryGenerateShinyOutbreakZorua()
    {
        PA8 test = new() { Species = (int)Species.Zorua, Form = 1 };
        const ulong s0 = 0xDF440DA44EEC4FFB;

        var param = new OverworldParam8a
        {
            FlawlessIVs = 0, IsAlpha = false,
            Shiny = Shiny.Random, RollCount = 30,
            GenderRatio = 0x7F,
        };

        var result = Overworld8aRNG.TryApplyFromSeed(test, EncounterCriteria.Unrestricted, param, s0);
        result.Should().BeTrue();

        test.IV_HP.Should().Be(10);
        test.IV_ATK.Should().Be(13);
        test.IV_DEF.Should().Be(8);
        test.IV_SPA.Should().Be(0);
        test.IV_SPD.Should().Be(17);
        test.IV_SPE.Should().Be(25);
        test.HeightScalar.Should().Be(99);
        test.WeightScalar.Should().Be(153);

        var verify = Overworld8aRNG.Verify(test, s0, param);
        verify.Should().BeTrue();
    }

    [Fact]
    public static void TestMagby()
    {
        const ulong s0 = 0xE12DDECBDFC64AA1ul;
        PA8 test = new() { Species = (int)Species.Magby };

        var param = new OverworldParam8a
        {
            FlawlessIVs = 3,
            IsAlpha = true,
            Shiny = Shiny.Random,
            RollCount = 17,
            GenderRatio = 0x7F,
        };

        var xoro = new Xoroshiro128Plus(s0);
        var (EntitySeed, _) = Overworld8aRNG.ApplyDetails(test, param, true, ref xoro);

        test.IV_HP.Should().Be(31);
        test.IV_ATK.Should().Be(31);
        test.IV_DEF.Should().Be(7);
        test.IV_SPA.Should().Be(31);
        test.IV_SPD.Should().Be(20);
        test.IV_SPE.Should().Be(10);

        test.AlphaMove.Should().Be((ushort)Move.Flamethrower);

        var verify = Overworld8aRNG.Verify(test, EntitySeed, param);
        verify.Should().BeTrue();
    }
}
