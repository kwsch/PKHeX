using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace PKHeX.Core;

/// <summary>
/// Logic for Pokéwalker RNG.
/// </summary>
public static class PokewalkerRNG
{
    private const int boxCount = 18;
    private const int boxSize = 30;
    private const int boxCapacity = boxCount * boxSize;
    private const int maxHours = 24;
    private const int maxYears = 100;
    private const int secondsPerDay = 60 * 60 * 24;

    // seeding for [stroll]: 3600 * hour + 60 * minute + second
    // seeding for [no-stroll]: (((month*day + minute + second) & 0xff) << 24) | (hour << 16) | (year)

    /// <summary>
    /// Get the 32-bit RNG seed for a Stroll seeding.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint GetSeedStroll(uint hour, uint minute, uint second) => (3600 * hour) + (60 * minute) + second;

    /// <summary>
    /// Get the 32-bit RNG seed for a No-Stroll seeding.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint GetSeedNoStroll(uint year, uint month, uint day, uint hour, uint minute, uint second) => ((((month * day) + minute + second) & 0xff) << 24) | (hour << 16) | year;

    /// <summary>
    /// Check if the seed is from a Stroll seeding.
    /// </summary>
    public static bool IsSeedFormatStroll(uint seed)
    {
        // XXXS_SSSS
        return seed < secondsPerDay;
    }

    /// <summary>
    /// Check if the seed is from a No-Stroll seeding.
    /// </summary>
    public static bool IsSeedFormatNoStroll(uint seed)
    {
        // the top byte of no-stroll can be any value, so we can skip checking that byte.
        // XX_HH_YYYY
        if ((ushort)seed >= maxYears)
            return false;
        if ((byte)(seed >> 16) >= maxHours)
            return false;
        return true;
    }

    /// <summary> Species slots per course. </summary>
    public const int SlotsPerCourse = 6;
    public const int GroupsPerCourse = 3;
    public const int SlotsPerGroup = 2;

    /// <summary>
    /// All species for all Pokéwalker courses.
    /// </summary>
    /// <remarks>
    /// 6 species per course; each course has 3 groups (A/B/C) of 2 species (0/1).
    /// Data is ripped from Overlay 112's route data, distilled down to just a list of species.
    /// When selecting a slot, the game uses the result of the rand() & 1 == 0, so invert the index.
    /// </remarks>
    private static ReadOnlySpan<ushort> CourseSpecies =>
    [
        115, 084, 029, 032, 016, 161, // 00 Refreshing Field 
        202, 069, 048, 046, 043, 021, // 01 Noisy Forest 
        240, 095, 066, 077, 163, 074, // 02 Rugged Road 
        054, 120, 079, 060, 191, 194, // 03 Beautiful Beach 
        239, 081, 081, 198, 163, 019, // 04 Suburban Area 
        238, 092, 092, 095, 041, 066, // 05 Dim Cave 
        147, 060, 098, 090, 118, 072, // 06 Blue Lake 
        063, 100, 109, 088, 019, 162, // 07 Town Outskirts 
        300, 264, 314, 313, 263, 265, // 08 Hoenn Field 
        320, 298, 116, 318, 118, 129, // 09 Warm Beach 
        218, 307, 228, 111, 077, 074, // 10 Volcano Path 
        352, 351, 203, 234, 044, 070, // 11 Treehouse 
        105, 128, 042, 177, 066, 092, // 12 Scary Cave 
        439, 415, 403, 406, 399, 401, // 13 Sinnoh Field 
        459, 361, 215, 436, 220, 179, // 14 Icy Mountain Road 
        357, 438, 114, 400, 179, 102, // 15 Big Forest 
        433, 200, 093, 418, 223, 170, // 16 White Lake 
        456, 422, 129, 086, 054, 090, // 17 Stormy Beach 
        417, 025, 039, 035, 183, 187, // 18 Resort 
        442, 446, 433, 349, 164, 042, // 19 Quiet Cave 
        120, 224, 116, 222, 223, 170, // 20 Beyond The Sea 
        035, 039, 041, 163, 074, 095, // 21 Night Sky's Edge 
        025, 025, 025, 025, 025, 025, // 22 Yellow Forest 
        441, 302, 025, 453, 427, 417, // 23 Rally 
        255, 133, 279, 061, 052, 025, // 24 Sightseeing 
        446, 374, 116, 355, 129, 436, // 25 Winners Path 
        239, 240, 238, 440, 174, 173, // 26 Amity Meadow 
    ];

    /// <summary>
    /// Finds an initial seed for the Pokéwalker IVs with the least amount of capture-IVs advances needed.
    /// </summary>
    /// <param name="ivs">IVs in order (speed last).</param>
    public static PokewalkerSeedResult GetLeastEffortSeed(Span<int> ivs)
    {
        var tmp = MemoryMarshal.Cast<int, uint>(ivs);
        return GetLeastEffortSeed(tmp[0], tmp[1], tmp[2], tmp[4], tmp[5], spe: tmp[3]);
    }

    /// <inheritdoc cref="LCRNGReversal.GetSeedsIVs(Span{uint},uint, uint)"/>
    public static PokewalkerSeedResult GetLeastEffortSeed(uint hp, uint atk, uint def, uint spa, uint spd, uint spe)
    {
        Span<uint> result = stackalloc uint[LCRNG.MaxCountSeedsIV];
        uint first = (hp | (atk << 5) | (def << 10)) << 16;
        uint second = (spe | (spa << 5) | (spd << 10)) << 16;
        return GetLeastEffortSeed(result, first, second);
    }

    /// <inheritdoc cref="GetLeastEffortSeed(Span{int})"/>
    /// <inheritdoc cref="LCRNGReversal.GetSeedsIVs(Span{uint},uint, uint)"/>
    public static PokewalkerSeedResult GetLeastEffortSeed(uint first, uint second)
        => GetLeastEffortSeed(stackalloc uint[LCRNG.MaxCountSeedsIV], first, second);

    /// <inheritdoc cref="GetLeastEffortSeed(Span{int})"/>
    /// <inheritdoc cref="LCRNGReversal.GetSeedsIVs(Span{uint},uint, uint)"/>
    public static PokewalkerSeedResult GetLeastEffortSeed(Span<uint> result, uint first, uint second)
    {
        // When generating a set of Pokéwalker Pokémon (and their IVs), the game does the following logic:
        // If the player begins a stroll, generate an initial seed based on seconds elapsed in the day (< 86400) and 3 slots.
        // Otherwise, generate an initial seed based on the elapsed time and date (similar to Gen4 initial seeding).

        // If the player begins a stroll, the game generates a set of 3 Pokémon to see, with results untraceable to the correlation.
        // Then, the game generates each Pokémon's IVs by calling rand() twice.
        // Since stroll causes 3 RNG advancements, an initial seed [stroll] can be advanced 3+(2*n) times, or [no-stroll] advanced 0+(2*n) times.
        // To determine the first valid initial seed, take advantage of the even-odd nature of the RNG frames (different initial seeding algorithm).

        int ctr = LCRNGReversal.GetSeedsIVs(result, first, second);
        if (ctr == 0)
            return default;

        result = result[..ctr];
        for (ushort priorPoke = 0; priorPoke < boxCapacity; priorPoke++)
        {
            foreach (ref var seed in result)
            {
                var s = seed; // first loop is already unrolled once (immediately generates IVs)

                // Check the [no-stroll] case.
                if (IsSeedFormatNoStroll(s))
                    return new(s, priorPoke, PokewalkerSeedType.NoStroll);
                s = LCRNG.Prev(s);

                // Check the [stroll] case.
                // Due to this backtracking algorithm, the first time we check won't be a valid (needs 3 advancements)
                if (priorPoke != 0 && IsSeedFormatStroll(s)) // don't check species; can be disassociated from slots.
                    return new(s, --priorPoke, PokewalkerSeedType.Stroll); // decrement priorPoke back to 0-indexed
                seed = LCRNG.Prev(s); // prep for next loop
            }
        }

        // The above logic for Stroll checks for [0,n-1) due to the backtracking nature of the algorithm.
        // Check the last-empty-slot for the Stroll case.
        // That's catching 540 'mons... quite unlikely! But still possible, as not all spreads are obtainable.
        foreach (ref var seed in result)
        {
            var s = LCRNG.Prev(seed);
            if (IsSeedFormatStroll(s)) // don't check species; can be disassociated from slots.
                return new(s, boxCapacity - 1, PokewalkerSeedType.Stroll); // decrement priorPoke back to 0-indexed
        }
        return default;
    }

    /// <summary>
    /// Indicates if the given seed is a valid Stroll seed for the given species on the given course.
    /// </summary>
    /// <param name="seed">Seed generated for a Pokéwalker Stroll.</param>
    /// <param name="species">Species expected to be encountered.</param>
    /// <param name="course">Course the Stroll is taking place on.</param>
    /// <returns>True if the seed is valid, false otherwise.</returns>
    /// <remarks>
    /// By immediately cancelling a Stroll, the next frames are not used to generate IVs, which makes these results irrelevant for checking IVs->Slot.
    /// </remarks>
    public static bool IsValidSeedStrollSlots(uint seed, ushort species, PokewalkerCourse4 course)
    {
        // initial seed
        // rand() & 1 => slot A
        // rand() & 1 => slot B
        // rand() & 1 => slot C
        // To pick the actual index, it is the result of the rand() & 1 == 0, so invert the index.
        var span = GetSpecies(course);
        var slotA = (int)(LCRNG.Next16(ref seed) & 1) ^ 1;
        if (span[slotA] == species)
            return true;
        var slotB = (int)(LCRNG.Next16(ref seed) & 1) ^ 1;
        if (span[slotB + 2] == species)
            return true;
        var slotC = (int)(LCRNG.Next16(ref seed) & 1) ^ 1;
        if (span[slotC + 4] == species)
            return true;
        return false;
    }

    /// <summary>
    /// Gets the slot indexes for referencing the overlay data.
    /// </summary>
    public static (int A, int B, int C) GetSlotsStroll(ref uint seed)
    {
        // initial seed
        // rand() & 1 => slot A
        // rand() & 1 => slot B
        // rand() & 1 => slot C
        // To pick the actual index, it is the result of the rand() & 1 == 0, so invert the index.

        var slotA = (int)(LCRNG.Next16(ref seed) & 1) ^ 1;
        var slotB = (int)(LCRNG.Next16(ref seed) & 1) ^ 1;
        var slotC = (int)(LCRNG.Next16(ref seed) & 1) ^ 1;
        return (slotA, slotB, slotC);
    }

    /// <summary>
    /// Gets all 6 species for the given course.
    /// </summary>
    /// <param name="course">Course to get species for.</param>
    /// <returns>Span of all 6 species.</returns>
    public static ReadOnlySpan<ushort> GetSpecies(PokewalkerCourse4 course) => CourseSpecies.Slice((int)course * SlotsPerCourse, SlotsPerCourse);

    /// <summary>
    /// Gets the species for the given course, group, and rand bit.
    /// </summary>
    /// <param name="course">Course to get species for.</param>
    /// <param name="group">Group to get species for (A/B/C).</param>
    /// <param name="rare">Rand bit to get species for (0/1).</param>
    /// <returns>Species for the given course, group, and rand bit.</returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static ushort GetSpecies(PokewalkerCourse4 course, int group, int rare)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual<uint>((uint)group, GroupsPerCourse);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual<uint>((uint)rare, SlotsPerGroup);
        var span = GetSpecies(course);
        return span[(group * 2) + rare];
    }

    /// <inheritdoc cref="GetPID(ushort, ushort, uint, byte, byte)"/>
    public static uint GetPID(uint id32, uint nature, byte gender, byte genderRatio) =>
        GetPID((ushort)id32, (ushort)(id32 >> 16), nature, gender, genderRatio);

    /// <summary>
    /// Calculates a Pokewalker PID based on the given parameters.
    /// </summary>
    /// <param name="TID16">16-bit Trainer ID.</param>
    /// <param name="SID16">16-bit Secret ID.</param>
    /// <param name="nature">Nature to set PID to.</param>
    /// <param name="gender">Gender to set PID to.</param>
    /// <param name="genderRatio">Gender ratio of the species.</param>
    /// <returns>PID for the given parameters.</returns>
    public static uint GetPID(ushort TID16, ushort SID16, uint nature, byte gender, byte genderRatio)
    {
        if (nature >= 24)
            nature = 0;
        uint pid = ((((uint)TID16 ^ SID16) >> 8) ^ 0xFF) << 24; // the most significant byte of the PID is chosen so the Pokémon can never be shiny.
        // Ensure nature is set to required nature without affecting shininess
        pid += nature - (pid % 25);

        if (genderRatio is 0 or >= 0xFE) // non-dual gender
            return pid;

        // Ensure Gender is set to required gender without affecting other properties
        // If Gender is modified, modify the ability if appropriate

        // either m/f
        var pidGender = (pid & 0xFF) < genderRatio ? 1 : 0;
        if (gender == pidGender)
            return pid;

        if (gender == 0) // Male
        {
            pid += (((genderRatio - (pid & 0xFF)) / 25) + 1) * 25;
            if ((nature & 1) != (pid & 1))
                pid += 25;
        }
        else
        {
            pid -= ((((pid & 0xFF) - genderRatio) / 25) + 1) * 25;
            if ((nature & 1) != (pid & 1))
                pid -= 25;
        }
        return pid;
    }

    /// <summary>
    /// Gets the IVs to a valid Pokewalker IV spread.
    /// </summary>
    /// <param name="criteria">Criteria to set IVs with.</param>
    /// <param name="iv32">Result IVs</param>
    public static bool GetRandomIVs(EncounterCriteria criteria, out uint iv32)
    {
        // Try to find a seed that works for the given criteria.
        // Don't waste too much time iterating, try around 100k.
        // 256 * 24 * 2 * 10 = 122,880
        for (uint year = 0; year < 2; year++)
        {
            uint seed = year;
            for (uint hour = 0; hour < maxHours; hour++)
            {
                for (uint i = 0; i <= 0xFF; i++)
                {
                    var iterSeed = seed;
                    for (int p = 0; p < 10; p++)
                    {
                        if (TryApply(ref iterSeed, out iv32, criteria))
                            return true;
                    }
                    seed += 0x01_000000;
                }
                seed += 0x01_0000;
            }
        }

        var randByte = (uint)Util.Rand.Next(256) << 24;
        TryApply(ref randByte, out iv32, EncounterCriteria.Unrestricted);
        return false;
    }

    private static bool TryApply(ref uint seed, out uint iv32, in EncounterCriteria criteria)
    {
        // Act like a Non-Stroll encounter, generate IV rand() results immediately.
        iv32 = PIDGenerator.GetIVsFromSeedSequentialLCRNG(ref seed);
        return criteria.IsSatisfiedIVs(iv32);
    }
}

/// <summary>
/// Wrapper for Pokewalker Seed Results
/// </summary>
/// <param name="Seed">32-bit seed</param>
/// <param name="PriorPoke">Count of Pokémon generated prior to the checked Pokémon</param>
/// <param name="Type">Type of seed</param>
public readonly record struct PokewalkerSeedResult(uint Seed, ushort PriorPoke, PokewalkerSeedType Type);

/// <summary>
/// Type of Pokewalker Seed
/// </summary>
public enum PokewalkerSeedType : byte
{
    /// <summary> Invalid </summary>
    None = 0,
    /// <summary> Stroll Seed </summary>
    Stroll = 1,
    /// <summary> No Stroll Seed </summary>
    NoStroll = 2,
}
