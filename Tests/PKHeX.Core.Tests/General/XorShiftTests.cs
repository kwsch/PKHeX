using FluentAssertions;
using Xunit;

namespace PKHeX.Core.Tests;

public static class XorShiftTests
{
    [Theory]
    [InlineData(0x7F996375F5A791FC, 0xF3A995138EC18148, 0x50188547081C7BEC, 0x0DA245AA536BEF36, 99, 1000)]
    public static void Forward(ulong s0, ulong s1, ulong n0, ulong n1, int frames, int loop)
    {
        var adv = GetFramesForward(s0, s1, n0, n1, loop);
        adv.Should().Be(frames);
    }

    [Theory]
    [InlineData(0x7F996375F5A791FC, 0xF3A995138EC18148, 0x50188547081C7BEC, 0x0DA245AA536BEF36, 99, 1000)]
    public static void Reverse(ulong s0, ulong s1, ulong n0, ulong n1, int frames, int loop)
    {
        var adv = GetFramesReverse(s0, s1, n0, n1, loop);
        adv.Should().Be(frames);
    }

    private static int GetFramesReverse(ulong s0, ulong s1, ulong n0, ulong n1, int loop)
    {
        var rand = new XorShift128(n0, n1);
        for (int i = 0; i < loop; i++)
        {
            _ = rand.Prev();
            if (rand.GetState64() == (s0, s1))
                return i;
        }
        return -1;
    }

    private static int GetFramesForward(ulong s0, ulong s1, ulong n0, ulong n1, int loop)
    {
        var rand = new XorShift128(s0, s1);
        for (int i = 0; i < loop; i++)
        {
            _ = rand.Next();
            if (rand.GetState64() == (n0, n1))
                return i;
        }
        return -1;
    }
}
