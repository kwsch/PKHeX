using System;
using static PKHeX.Core.SizeType9;

namespace PKHeX.Core;

/// <summary>
/// Generation 9 Size Type used by encounters to force a specific size range or value.
/// </summary>
public enum SizeType9 : byte
{
    /// <summary> Size value is randomly distributed. </summary>
    RANDOM = 0,

    /// <summary> Size value is very small. </summary>
    XS = 1,

    /// <summary> Size value is small. </summary>
    S = 2,

    /// <summary> Size value is average. </summary>
    M = 3,

    /// <summary> Size value is large. </summary>
    L = 4,

    /// <summary> Size value is very large. </summary>
    XL = 5,

    /// <summary> Size value is a specific value. </summary>
    VALUE = 6,
}

/// <summary>
/// Extension methods for <see cref="SizeType9"/>.
/// </summary>
public static class SizeType9Extensions
{
    /// <summary>
    /// Gets a random size value for the specified <see cref="SizeType9"/>.
    /// </summary>
    /// <param name="type">Size Type</param>
    /// <param name="value">Value if <see cref="VALUE"/>, unused otherwise.</param>
    /// <param name="rand">RNG to generate value with.</param>
    /// <returns>Size Value</returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
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

    /// <summary>
    /// Checks if the specified value is within the range of the <see cref="SizeType9"/>.
    /// </summary>
    /// <param name="type">Size Type</param>
    /// <param name="value">Value to check</param>
    /// <returns>True if the value is within the range of the <see cref="SizeType9"/>.</returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static bool IsWithinRange(this SizeType9 type, byte value) => type switch
    {
        RANDOM => true,
        XS => value < 0x10,
        S => value is >= 0x10 and < 0x30,
        M => value is >= 0x30 and < 0xD0,
        L => value is >= 0xD0 and < 0xF0,
        XL => value >= 0xF0,
        _ => throw new ArgumentOutOfRangeException(nameof(type), type, null),
    };
}
