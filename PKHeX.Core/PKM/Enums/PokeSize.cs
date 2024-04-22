using System;
using static PKHeX.Core.PokeSize;

namespace PKHeX.Core;

/// <summary>
/// Size Groupings for an entity's Height and Weight
/// </summary>
public enum PokeSize
{
    XS,
    S,
    AV,
    L,
    XL,
}

public static class PokeSizeUtil
{
    /// <summary>
    /// Compares the sizing scalar to different thresholds to determine the size rating.
    /// </summary>
    /// <param name="scalar">Sizing scalar (0-255)</param>
    /// <returns>0-4 rating</returns>
    public static PokeSize GetSizeRating(byte scalar) => scalar switch
    {
        < 0x10 => XS, // 1/16 = XS
        < 0x30 => S,  // 2/16 = S
        < 0xD0 => AV, // average (10/16)
        < 0xF0 => L,  // 2/16 = L
        _ => XL, // 1/16 = XL
    };

    public static byte GetRandomScalar(this PokeSize size) => GetRandomScalar(size, Util.Rand);

    public static byte GetRandomScalar(this PokeSize size, Random rnd) => size switch
    {
        XS => (byte)(rnd.Next(0x10)),
        S  => (byte)(rnd.Next(0x20) + 0x10),
        AV => (byte)(rnd.Next(0xA0) + 0x30),
        L  => (byte)(rnd.Next(0x20) + 0xD0),
        XL => (byte)(rnd.Next(0x10) + 0xF0),
        _ => GetRandomScalar(rnd),
    };

    /// <summary>
    /// Gets a random size scalar with a triangular distribution (copying official implementation).
    /// </summary>
    public static byte GetRandomScalar() => GetRandomScalar(Util.Rand);

    public static byte GetRandomScalar(Random rnd) => (byte)(rnd.Next(0x81) + rnd.Next(0x80));
}
