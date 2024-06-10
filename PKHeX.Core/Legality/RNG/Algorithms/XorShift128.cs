using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace PKHeX.Core;

/// <summary>
/// Self-modifying RNG structure that implements xorshift.
/// </summary>
/// <remarks>https://en.wikipedia.org/wiki/Xorshift</remarks>
/// <seealso cref="Xoroshiro128Plus"/>
[StructLayout(LayoutKind.Explicit)]
public ref struct XorShift128
{
    [FieldOffset(0x0)] private uint x;
    [FieldOffset(0x4)] private uint y;
    [FieldOffset(0x8)] private uint z;
    [FieldOffset(0xC)] private uint w;

    // not really readonly! just prevents future updates from touching this.
    [FieldOffset(0x0)] private readonly ulong s0;
    [FieldOffset(0x8)] private readonly ulong s1;
    [FieldOffset(0x0)] public readonly UInt128 State;

    /// <summary>
    /// Uses the <see cref="ARNG"/> to advance the seed for each 32-bit input.
    /// </summary>
    /// <param name="seed">32 bit seed</param>
    /// <remarks>sub_E0F5E0 in v1.1.3</remarks>
    public XorShift128(uint seed) : this()
    {
        x = seed;
        y = (0x6C078965 * x) + 1;
        z = (0x6C078965 * y) + 1;
        w = (0x6C078965 * z) + 1;
    }

    public XorShift128(ulong s0, ulong s1) : this()
    {
        this.s0 = s0;
        this.s1 = s1;
    }

    public XorShift128(uint x, uint y, uint z, uint w) : this()
    {
        this.x = x;
        this.y = y;
        this.z = z;
        this.w = w;
    }

    public XorShift128(UInt128 state) => State = state;

    public readonly (uint x, uint y, uint z, uint w) GetState32() => (x, y, z, w);
    public readonly (ulong s0, ulong s1) GetState64() => (s0, s1);
    public readonly bool Equals(ulong state0, ulong state1) => s0 == state0 && s1 == state1;

    /// <summary>
    /// Gets the next random <see cref="ulong"/>.
    /// </summary>
    public uint Next()
    {
        var t = x ^ (x << 11);
        x = y;
        y = z;
        z = w;
        return w = w ^ (w >> 19) ^ t ^ (t >> 8);
    }

    /// <summary>
    /// Gets the previous random <see cref="ulong"/>.
    /// </summary>
    public uint Prev()
    {
        var t = w ^ z ^ (z >> 19);
        t ^= t >> 8;
        t ^= t >> 16;

        w = z;
        z = y;
        y = x;

        t ^= t << 11;
        t ^= t << 22;

        x = t;
        return w;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public uint NextUInt() => (uint)NextInt();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public uint NextUInt(uint max) => NextUInt() % max;

    /// <summary>
    /// Gets the next random <see cref="ulong"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int NextInt(int start = int.MinValue, int end = int.MaxValue)
    {
        var next = Next();
        var delta = unchecked((uint)(end - start));
        return start + (int)(next % delta);
    }
}
