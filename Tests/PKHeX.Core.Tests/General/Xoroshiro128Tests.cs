using FluentAssertions;
using Xunit;

namespace PKHeX.Core.Tests;

public static class Xoroshiro128Tests
{
    [Theory]
    [InlineData(1, 1, 0x339C61939607D435, 0xF4DD6E2E9698D1B0, 98, 1000)]
    public static void Forward(ulong s0, ulong s1, ulong n0, ulong n1, int frames, int loop)
    {
        var adv = GetFramesForward(s0, s1, n0, n1, loop);
        adv.Should().Be(frames);
    }

    [Theory]
    [InlineData(1, 1, 0x339C61939607D435, 0xF4DD6E2E9698D1B0, 98, 1000)]
    public static void Reverse(ulong s0, ulong s1, ulong n0, ulong n1, int frames, int loop)
    {
        var adv = GetFramesReverse(s0, s1, n0, n1, loop);
        adv.Should().Be(frames);
    }

    private static int GetFramesReverse(ulong s0, ulong s1, ulong n0, ulong n1, int loop)
    {
        var rand = new Xoroshiro128Plus(n0, n1);
        for (int i = 0; i < loop; i++)
        {
            _ = rand.Prev();
            if (rand.Equals(s0, s1))
                return i;
        }
        return -1;
    }

    private static int GetFramesForward(ulong s0, ulong s1, ulong n0, ulong n1, int loop)
    {
        var rand = new Xoroshiro128Plus(s0, s1);
        for (int i = 0; i < loop; i++)
        {
            _ = rand.Next();
            if (rand.Equals(n0, n1))
                return i;
        }
        return -1;
    }

    [Theory]
    [InlineData(0x5a62b550aa55aa55, 0xCCF314B0, 0x359D276C, 0x43157D53)]
    [InlineData(0x5a63b450aa55aa55, 0xCCF314B0, 0x35BD486B, 0x43157D53)]
    [InlineData(0x5a6cb550aa55aa55, 0xCCF314B0, 0x375D316C, 0x43157D53)]
    [InlineData(0x5a6db450aa55aa55, 0xCCF314B0, 0x377D526B, 0x43157D53)]
    [InlineData(0x9a508f54aa55aa55, 0xCCF314B0, 0x3BA454B2, 0x43157D53)]
    [InlineData(0x9a518e54aa55aa55, 0xCCF314B0, 0x3BC475B1, 0x43157D53)]
    [InlineData(0x9a5a8754aa55aa55, 0xCCF314B0, 0x3C635EBA, 0x43157D53)]
    [InlineData(0x9a5b8654aa55aa55, 0xCCF314B0, 0x3C837FB9, 0x43157D53)]
    [InlineData(0x9a70a754aa55aa55, 0xCCF314B0, 0x379F74DA, 0x43157D53)]
    [InlineData(0x9a71a654aa55aa55, 0xCCF314B0, 0x37BF95D9, 0x43157D53)]
    [InlineData(0x9a7aaf54aa55aa55, 0xCCF314B0, 0x38607ED2, 0x43157D53)]
    [InlineData(0x9a7bae54aa55aa55, 0xCCF314B0, 0x38809FD1, 0x43157D53)]
    [InlineData(0xaa54ab55aa55aa55, 0xCCF314B0, 0x3C2FD8B6, 0x43157D53)]
    [InlineData(0xaa55aa55aa55aa55, 0xCCF314B0, 0x3C4FF9B5, 0x43157D53)]
    [InlineData(0xaa7e8b55aa55aa55, 0xCCF314B0, 0x38F40296, 0x43157D53)]
    [InlineData(0xaa7f8a55aa55aa55, 0xCCF314B0, 0x39142395, 0x43157D53)]
    public static void TestConsecutive(ulong seed, uint first, uint second, uint third)
    {
        bool found = false;
        var machine = new XoroMachineConsecutive(first, second);
        foreach (var result in machine)
        {
            if (result == seed)
                found = true;

            var xoro = new Xoroshiro128Plus(seed);
            ((uint)xoro.Next()).Should().Be(first);
            ((uint)xoro.Next()).Should().Be(second);
            ((uint)xoro.Next()).Should().Be(third);
        }
        found.Should().BeTrue();
    }

    [Theory]
    [InlineData(0x5a62b550aa55aa55, 0xCCF314B0, 0x359D276C, 0x43157D53)]
    [InlineData(0x5a63b450aa55aa55, 0xCCF314B0, 0x35BD486B, 0x43157D53)]
    [InlineData(0x5a6cb550aa55aa55, 0xCCF314B0, 0x375D316C, 0x43157D53)]
    [InlineData(0x5a6db450aa55aa55, 0xCCF314B0, 0x377D526B, 0x43157D53)]
    [InlineData(0x9a508f54aa55aa55, 0xCCF314B0, 0x3BA454B2, 0x43157D53)]
    [InlineData(0x9a518e54aa55aa55, 0xCCF314B0, 0x3BC475B1, 0x43157D53)]
    [InlineData(0x9a5a8754aa55aa55, 0xCCF314B0, 0x3C635EBA, 0x43157D53)]
    [InlineData(0x9a5b8654aa55aa55, 0xCCF314B0, 0x3C837FB9, 0x43157D53)]
    [InlineData(0x9a70a754aa55aa55, 0xCCF314B0, 0x379F74DA, 0x43157D53)]
    [InlineData(0x9a71a654aa55aa55, 0xCCF314B0, 0x37BF95D9, 0x43157D53)]
    [InlineData(0x9a7aaf54aa55aa55, 0xCCF314B0, 0x38607ED2, 0x43157D53)]
    [InlineData(0x9a7bae54aa55aa55, 0xCCF314B0, 0x38809FD1, 0x43157D53)]
    [InlineData(0xaa54ab55aa55aa55, 0xCCF314B0, 0x3C2FD8B6, 0x43157D53)]
    [InlineData(0xaa55aa55aa55aa55, 0xCCF314B0, 0x3C4FF9B5, 0x43157D53)]
    [InlineData(0xaa7e8b55aa55aa55, 0xCCF314B0, 0x38F40296, 0x43157D53)]
    [InlineData(0xaa7f8a55aa55aa55, 0xCCF314B0, 0x39142395, 0x43157D53)]
    public static void TestSkip(ulong seed, uint first, uint second, uint third)
    {
        bool found = false;
        var machine = new XoroMachineSkip(first, third);
        foreach (var result in machine)
        {
            if (result == seed)
                found = true;

            var xoro = new Xoroshiro128Plus(seed);
            ((uint)xoro.Next()).Should().Be(first);
            ((uint)xoro.Next()).Should().Be(second);
            ((uint)xoro.Next()).Should().Be(third);
        }
        found.Should().BeTrue();
    }
}
