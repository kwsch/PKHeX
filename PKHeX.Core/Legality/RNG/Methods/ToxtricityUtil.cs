using System;

namespace PKHeX.Core;

public static class ToxtricityUtil
{
    private static ReadOnlySpan<byte> Nature0 => new byte[] { 3, 4, 2, 8, 9, 19, 22, 11, 13, 14, 0, 6, 24 };
    private static ReadOnlySpan<byte> Nature1 => new byte[] { 1, 5, 7, 10, 12, 15, 16, 17, 18, 20, 21, 23 };

    public static byte GetRandomNature(ref Xoroshiro128Plus rand, byte form)
    {
        var table = form == 0 ? Nature0 : Nature1;
        return table[(int)rand.NextInt((uint)table.Length)];
    }
}
