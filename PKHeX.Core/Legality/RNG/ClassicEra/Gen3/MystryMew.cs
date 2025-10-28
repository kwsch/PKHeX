using System;

namespace PKHeX.Core;

/// <summary>
/// Logic for MYSTRY Mew origins.
/// </summary>
public static class MystryMew
{
    private static ReadOnlySpan<ushort> Seeds =>
    [
        0x0652, 0x0932, 0x0C13, 0x0D43, 0x0EEE,
        0x1263, 0x13C9, 0x1614, 0x1C09, 0x1EA5,
        0x20BF, 0x2389, 0x2939, 0x302D, 0x306E,
        0x34F3, 0x45F3, 0x46CE, 0x4A0D, 0x4B63,

        0x4C79, 0x508E, 0x50AB, 0x5240, 0x5327,
        0x56BA, 0x56CC, 0x5841, 0x5A60, 0x5BC1,
        0x5E2B, 0x5EF3, 0x6065, 0x643F, 0x6457,
        0x67A3, 0x6944, 0x6E06, 0x6E62, 0x7667,

        0x77EF, 0x78D2, 0x8655, 0x8A92, 0x8B48,
        0x93D0, 0x941D, 0x95A0, 0x967D, 0x9690,
        0x9C37, 0x9C40, 0x9D9C, 0x9DE4, 0x9E86,
        0xA153, 0xA443, 0xA8AC, 0xAC08, 0xAFFB,

        0xB1F2, 0xB831, 0xBE96, 0xC2D4, 0xC385,
        0xC6CE, 0xC92C, 0xC953, 0xC962, 0xCC43,
        0xCD47, 0xCD96, 0xD1E4, 0xDFED, 0xE62C,
        0xE6CC, 0xE90A, 0xE95D, 0xE991, 0xEBB2,

        0xEE7F, 0xEE9F, 0xEFC8, 0xF0E4, 0xFE4E,
        0xFE9D,
    ];

    private const int MewPerRestrictedSeed = 5;
    private const int RestrictedIndex = 0;

    /// <summary> 01_34 of this set were released by the event organizer. </summary>
    private const ushort ReleasedSeed = 0x6065;
    private const byte ReleasedSeedKeptIndex = 2; // 2nd sibling is the only valid one.
    private const uint ReleasedSeedKeptSeed = 0xE8D1A0BF; // Next5(Next5(ReleasedSeed))

    /// <summary>
    /// Checks if the seed was unreleased.
    /// </summary>
    public static bool IsUnreleased(uint seed, int subIndex) => seed is ReleasedSeed && subIndex is not ReleasedSeedKeptIndex;

    /// <summary>
    /// Checks if the seed index (of the returned tuple) is referring to the oldest (first) sibling of the 5-per-seed.
    /// </summary>
    public static bool IsRestrictedIndex0th(int index) => index == RestrictedIndex;

    /// <summary>
    /// Gets a random valid seed based on the input <see cref="random"/> value.
    /// </summary>
    public static uint GetSeed(uint random, PIDType type = PIDType.BACD_U)
    {
        var seeds = Seeds;
        uint restricted = random % (uint)seeds.Length;
        var seed = (uint)seeds[(int)restricted];
        if (seed is ReleasedSeed)
            return ReleasedSeedKeptSeed;
        if (type == PIDType.BACD_R)
            return seed;

        uint position = (random % (MewPerRestrictedSeed - 1)) + 1;
        for (uint i = 0; i < position; i++)
            seed = LCRNG.Next5(seed);

        return seed;
    }

    /// <summary>
    /// Gets the seed at (sorted) <see cref="index"/>.
    /// </summary>
    public static uint GetSeed(int index) => Seeds[(uint)index >= Seeds.Length ? 0 : index];

    /// <summary>
    /// Checks if the seed is a known seed.
    /// </summary>
    /// <returns>True if the seed is known.</returns>
    public static bool IsValidSeed(uint seed) => GetSeedIndex(seed) != -1;

    /// <param name="seed">Origin seed (for the PID/IV)</param>
    /// <param name="subIndex">Sub-index (if not a restricted seed).</param>
    /// <inheritdoc cref="IsValidSeed(uint)"/>
    public static bool IsValidSeed(uint seed, int subIndex) => !IsUnreleased(seed, subIndex);

    /// <summary>
    /// Checks if the seed is a known seed.
    /// </summary>
    /// <param name="seed">Origin seed (for the PID/IV)</param>
    public static int GetSeedIndex(uint seed)
    {
        var seeds = Seeds;
        if (seed <= ushort.MaxValue)
            return seeds.BinarySearch((ushort)seed);
        for (int i = 0; i < MewPerRestrictedSeed; i++)
        {
            seed = LCRNG.Prev5(seed); // BACD{?}
            if (seed <= ushort.MaxValue)
                return seeds.BinarySearch((ushort)seed);
        }

        return -1;
    }

    /// <summary>
    /// Get the index and sub-index of the seed in the known seed list.
    /// </summary>
    /// <param name="seed">Origin seed (for the PID/IV)</param>
    /// <returns>Tuple of indexes; -1 if not found.</returns>
    public static (int Index, int SubIndex) GetSeedIndexes(uint seed)
    {
        var seeds = Seeds;
        if (seed <= ushort.MaxValue)
        {
            var index = seeds.BinarySearch((ushort)seed);
            return (index, RestrictedIndex);
        }
        for (int i = RestrictedIndex + 1; i < RestrictedIndex + MewPerRestrictedSeed; i++)
        {
            seed = LCRNG.Prev5(seed); // BACD{?}
            if (seed <= ushort.MaxValue)
            {
                var index = seeds.BinarySearch((ushort)seed);
                return (index, i);
            }
        }
        return (-1, -1);
    }
}
