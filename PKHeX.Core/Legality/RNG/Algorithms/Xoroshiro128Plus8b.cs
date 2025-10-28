using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace PKHeX.Core;

/// <summary>
/// Self-modifying RNG structure that implements xoroshiro128+ which split-mixes the initial seed to populate all 128-bits of the initial state, rather than using a fixed 64-bit half.
/// </summary>
/// <remarks>https://en.wikipedia.org/wiki/Xoroshiro128%2B</remarks>
/// <seealso cref="Xoroshiro128Plus"/>
/// <remarks>Used by the Brilliant Diamond &amp; Shining Pearl games; differs in how values are yielded by Next calls.</remarks>
[StructLayout(LayoutKind.Explicit)]
public ref struct Xoroshiro128Plus8b
{
    [FieldOffset(0x0)] private ulong s0;
    [FieldOffset(0x8)] private ulong s1;
    [FieldOffset(0x0)] public readonly UInt128 State;

    public Xoroshiro128Plus8b(ulong s0, ulong s1) => (this.s0, this.s1) = (s0, s1);
    public Xoroshiro128Plus8b(UInt128 state) => State = state;
    public readonly (ulong s0, ulong s1) GetState() => (s0, s1);
    public readonly bool Equals(ulong state0, ulong state1) => s0 == state0 && s1 == state1;

    public Xoroshiro128Plus8b(ulong seed)
    {
        s0 = SplitMix64(seed + 0x9E3779B97F4A7C15);
        s1 = SplitMix64(seed + 0x3C6EF372FE94F82A);
    }

    private static ulong SplitMix64(ulong seed)
    {
        seed = 0xBF58476D1CE4E5B9 * (seed ^ (seed >> 30));
        seed = 0x94D049BB133111EB * (seed ^ (seed >> 27));
        return seed ^ (seed >> 31);
    }

    /// <summary>
    /// Gets the next random <see cref="ulong"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ulong Next()
    {
        var _s0 = s0;
        var _s1 = s1;
        ulong result = _s0 + _s1;

        _s1 ^= _s0;
        // Final calculations and store back to fields
        s0 = BitOperations.RotateLeft(_s0, 24) ^ _s1 ^ (_s1 << 16);
        s1 = BitOperations.RotateLeft(_s1, 37);

        return result;
    }

    /// <summary>
    /// Gets the next previous <see cref="ulong"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ulong Prev()
    {
        var _s0 = s0;
        var _s1 = s1;
        _s1 = BitOperations.RotateLeft(_s1, 27);
        _s0 = _s0 ^ _s1 ^ (_s1 << 16);
        _s0 = BitOperations.RotateLeft(_s0, 40);
        _s1 ^= _s0;
        ulong result = _s0 + _s1;

        s0 = _s0;
        s1 = _s1;
        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool NextBool() => (Next() >> 63) != 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public byte NextByte() => (byte)(Next() >> 56);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ushort UShort() => (ushort)(Next() >> 48);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public uint NextUInt() => (uint)(Next() >> 32);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public uint NextUInt(uint max)
    {
        var rnd = NextUInt();
        return rnd - ((rnd / max) * max);
    }
}
