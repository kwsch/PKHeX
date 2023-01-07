using FluentAssertions;
using Xunit;

namespace PKHeX.Core.Tests.Legality;

public static class Wild8bRNGTests
{
    [Fact]
    public static void TryGenerateLatias()
    {
        PB8 test = new() { Species = (int)Species.Latias};
        const ulong s0 = 0xdf9cf5c73e4a160b;
        const ulong s1 = 0xd0b8383103a7f201;
        Wild8bRNG.TryApplyFromSeed(test, EncounterCriteria.Unrestricted, Shiny.Random, 3, new XorShift128(s0, s1), AbilityPermission.Any12);
        test.IV_HP.Should().Be(31);
        test.IV_ATK.Should().Be(4);
        test.IV_DEF.Should().Be(31);
        test.IV_SPA.Should().Be(31);
        test.IV_SPD.Should().Be(6);
        test.IV_SPE.Should().Be(8);
        test.HeightScalar.Should().Be(123);
        test.WeightScalar.Should().Be(115);
    }
}
