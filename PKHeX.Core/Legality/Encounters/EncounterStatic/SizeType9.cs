using System;
using static PKHeX.Core.SizeType9;

namespace PKHeX.Core;

public enum SizeType9
{
    RANDOM = 0,
    XS = 1,
    S = 2,
    M = 3,
    L = 4,
    XL = 5,
    VALUE = 6,
}

public static class SizeType9Extensions
{
    public static byte GetSizeValue(this SizeType9 type, byte value, ref Xoroshiro128Plus rand) => type switch
    {
        RANDOM => (byte)(rand.NextInt(0x81) + rand.NextInt(0x80)),
        XS => (byte)rand.NextInt(0x10),
        S  => (byte)(rand.NextInt(0x20) + 0x10),
        M  => (byte)(rand.NextInt(0xA0) + 0x30),
        L  => (byte)(rand.NextInt(0x20) + 0xD0),
        XL => (byte)(rand.NextInt(0x10) + 0xF0),
        VALUE => value,
        _ => throw new ArgumentOutOfRangeException(nameof(type), type, null),
    };
}
