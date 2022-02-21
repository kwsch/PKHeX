using System.Runtime.CompilerServices;

namespace PKHeX.Core;

/// <summary>
/// Self-modifying RNG structure that implements xoroshiro128+
/// </summary>
/// <remarks>https://en.wikipedia.org/wiki/Xoroshiro128%2B</remarks>
public ref struct Xoroshiro128Plus
{
    public const ulong XOROSHIRO_CONST0= 0x0F4B17A579F18960;
    public const ulong XOROSHIRO_CONST = 0x82A2B175229D6A5B;

    private ulong s0, s1;

    public Xoroshiro128Plus(ulong s0 = XOROSHIRO_CONST0, ulong s1 = XOROSHIRO_CONST) => (this.s0, this.s1) = (s0, s1);

    public (ulong s0, ulong s1) GetState() => (s0, s1);
    public string FullState => $"{s1:X16}{s0:X16}";

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static ulong RotateLeft(ulong x, int k) => (x << k) | (x >> -k);

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
        s0 = RotateLeft(_s0, 24) ^ _s1 ^ (_s1 << 16);
        s1 = RotateLeft(_s1, 37);

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
        _s1 = RotateLeft(_s1, 27);
        _s0 = _s0 ^ _s1 ^ (_s1 << 16);
        _s0 = RotateLeft(_s0, 40);
        _s1 ^= _s0;
        ulong result = _s0 + _s1;

        s0 = _s0;
        s1 = _s1;
        return result;
    }

    /// <summary>
    /// Gets a random value that is less than <see cref="MOD"/>
    /// </summary>
    /// <param name="MOD">Maximum value (exclusive). Generates a bitmask for the loop.</param>
    /// <returns>Random value</returns>
    public ulong NextInt(ulong MOD = 0xFFFFFFFF)
    {
        ulong mask = GetBitmask(MOD);
        ulong res;
        do
        {
            res = Next() & mask;
        } while (res >= MOD);
        return res;
    }

    /// <summary>
    /// Next Power of Two
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static ulong GetBitmask(ulong x)
    {
        x--; // comment out to always take the next biggest power of two, even if x is already a power of two
        x |= x >> 1;
        x |= x >> 2;
        x |= x >> 4;
        x |= x >> 8;
        x |= x >> 16;
        return x;
    }

    /// <summary>
    /// Gets a random float value.
    /// </summary>
    /// <param name="range">Range of values permitted [0,n)</param>
    /// <param name="bias">Number to add to the resultant amplified range.</param>
    /// <returns>Random value</returns>
    public float NextFloat(float range, float bias = 0f)
    {
        const float inv_64_f = 5.421e-20f; // 1/(2^64) as a float; 0x1F800000u moved into a float register.
        ulong next = Next();
        return (range * (next * inv_64_f)) + bias;
    }
}
