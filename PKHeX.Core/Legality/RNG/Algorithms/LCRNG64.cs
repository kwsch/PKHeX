using System.Runtime.CompilerServices;

namespace PKHeX.Core;

/// <summary>
/// 64 Bit Linear Congruential Random Number Generator
/// </summary>
public static class LCRNG64
{
    public const ulong Mult  = 0x5D588B656C078965;
    public const ulong Add   = 0x0000000000269EC3;
    public const ulong rMult = 0xDEDCEDAE9638806D;
    public const ulong rAdd  = 0x9B1AE6E9A384E6F9;

    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static ulong Next(ulong seed) => (seed * Mult) + Add;
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static ulong Prev(ulong seed) => (seed * rMult) + rAdd;

    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static ulong Next(ref ulong seed) => seed = Next(seed);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static ulong Prev(ref ulong seed) => seed = Prev(seed);

    public static uint Next32(ref ulong seed) => (uint)(seed = Next(seed) >> 32);
    public static uint NextRange(ulong seed, uint exclusiveMax) => (uint)(((Next(seed) >> 32) * exclusiveMax) >> 32);
    public static uint NextRange(ref ulong seed, uint exclusiveMax) => (uint)((((seed = Next(seed)) >> 32) * exclusiveMax) >> 32);
}
