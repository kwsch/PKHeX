using System;

namespace PKHeX.Core;

public static partial class Util
{
    /// <inheritdoc cref="Random.Shared"/>
    public static Random Rand => Random.Shared;

    public static uint Rand32() => Rand32(Rand);
    public static uint Rand32(this Random rnd) => ((uint)rnd.Next(1 << 30) << 2) | (uint)rnd.Next(1 << 2);
    public static ulong Rand64(this Random rnd) => rnd.Rand32() | ((ulong)rnd.Rand32() << 32);
}
