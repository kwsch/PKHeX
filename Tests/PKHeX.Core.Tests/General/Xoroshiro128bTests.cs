using FluentAssertions;
using Xunit;

namespace PKHeX.Core.Tests;

public static class Xoroshiro128bTests
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
        var rand = new Xoroshiro128Plus8b(n0, n1);
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
        var rand = new Xoroshiro128Plus8b(s0, s1);
        for (int i = 0; i < loop; i++)
        {
            _ = rand.Next();
            if (rand.Equals(n0, n1))
                return i;
        }
        return -1;
    }
}
