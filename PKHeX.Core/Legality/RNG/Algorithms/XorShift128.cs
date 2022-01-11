using System.Runtime.CompilerServices;

namespace PKHeX.Core;

/// <summary>
/// Self-modifying RNG structure that implements xorshift.
/// </summary>
/// <remarks>https://en.wikipedia.org/wiki/Xorshift</remarks>
/// <seealso cref="Xoroshiro128Plus"/>
public ref struct XorShift128
{
    private uint x, y, z, w;

    public XorShift128(uint seed)
    {
        x = seed << 13;
        y = (seed >> 9) ^ (x << 6);
        z = y >> 7;
        w = seed;
    }

    public XorShift128(ulong s0, ulong s1) : this((uint)s0, (uint)(s0 >> 32), (uint)s1, (uint)(s1 >> 32))
    {
    }

    public XorShift128(uint x, uint y, uint z, uint w)
    {
        this.x = x;
        this.y = y;
        this.z = z;
        this.w = w;
    }

    public (uint x, uint y, uint z, uint w) GetState32() => (x, y, z, w);
    public (ulong s0, ulong s1) GetState64() => (((ulong)y << 32) | x, ((ulong)w << 32) | z);
    public string GetState128 => $"{w:X8}{z:X8}{y:X8}{x:X8}";

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
