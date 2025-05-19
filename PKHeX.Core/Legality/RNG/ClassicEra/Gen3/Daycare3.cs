using System;

namespace PKHeX.Core;

/// <summary>
/// Generation 3 Daycare RNG correlation logic.
/// </summary>
public static class Daycare3
{
    /// <summary>
    /// Checks if the PID is possible to obtain, in isolation.
    /// </summary>
    /// <param name="pid">Obtained PID.</param>
    /// <param name="version">Version the egg was obtained in.</param>
    /// <returns><c>true</c> if the PID is valid, <c>false</c> otherwise.</returns>
    public static bool IsValidProcPID(uint pid, GameVersion version)
    {
        // Gen3 Eggs don't have a zero-value pending PID value.

        // For R/S/FR/LG (not Emerald)
        // LoveCheck: Rand() * 100 / 65535 < compatibility*
        // PID: The game stores the lower 16 bits via (Rand() & 0xFFFE) + 1
        // 0-value for lower 16 bits is never set by egg proc
        if (version is not GameVersion.E)
            return (pid & 0xFFFF) != 0;

        // For Emerald, the PID is generated according to the following pattern:
        // LoveCheck: Rand() * 100 / 65535 < compatibility*
        // PID: The game stores the full 32-bit value to lock in nature; previous games only store the lower 16 bits.
        //   Everstone (inherit nature): 2400 attempts with rand() << 16 | rand() && pid != 0 -- assume never needing 2400 attempts even with vBlank in the mix.
        //   Otherwise (random nature): rand() << 16 | (rand() & 0xFFFE) + 1

        // The PID will never be 0.
        if (pid == 0)
            return false;

        // The LoveCheck is not always immediately before the PID generation (Everstone).
        // Considering vBlank interrupts, it's essentially random enough to get any PID (besides 0).
        return true;
    }

    /// <inheritdoc cref="TryGetOriginSeed(ReadOnlySpan{int}, uint, GameVersion, out Daycare3Origin)"/>
    public static bool TryGetOriginSeed(PKM pk, out Daycare3Origin origin)
    {
        Span<int> actual = stackalloc int[6];
        pk.GetIVs(actual);
        if (pk.Version is GameVersion.E)
            return TryGetOriginSeedEmerald(actual, out origin);
        return TryGetOriginSeed(actual, pk.EncryptionConstant, out origin);
    }

    /// <summary>
    /// Searches for a <see cref="origin"/> state that the player picked up the egg.
    /// </summary>
    /// <param name="ivs">IVs to check for.</param>
    /// <param name="pid">Obtained PID; only high 16 bits needed.</param>
    /// <param name="version">Version of the game.</param>
    /// <param name="origin">Origin info when receiving the egg</param>
    /// <returns><c>true</c> if a valid origin was found, <c>false</c> otherwise.</returns>
    public static bool TryGetOriginSeed(ReadOnlySpan<int> ivs, uint pid, GameVersion version, out Daycare3Origin origin)
    {
        if (version is GameVersion.E)
            return TryGetOriginSeedEmerald(ivs, out origin);
        return TryGetOriginSeed(ivs, pid, out origin);
    }

    /// <inheritdoc cref="TryGetOriginSeed(ReadOnlySpan{int}, uint, GameVersion, out Daycare3Origin)"/>
    public static bool TryGetOriginSeedEmerald(ReadOnlySpan<int> ivs, out Daycare3Origin origin)
    {
        // Frame pattern:
        // PID is already decided. Simply IVs.
        // IVs low (overwritten by inheritance later)
        // IVs high (overwritten by inheritance later)
        // **vBlank
        // Inheritance

        Span<int> tmp = stackalloc int[6];

        // Search forward from Emerald's initial 0 seed.
        // Once we find a result, that's the lowest (best) result possible.
        uint frame = 0xBE34A09C; // 60 frames after initial seed (0x0000_0000); arbitrary minimum frame.

        // Assume the most-frequent vBlank placement in the calculation sequence.
        // (after IVs)
        while (true)
        {
            var seed = frame = LCRNG.Next(frame);

            var iv1 = LCRNG.Next16(ref seed);
            var iv2 = LCRNG.Next16(ref seed);
            Fill(tmp, iv1, iv2);

            var countMatching = GetCountMatch(ivs, tmp);
            if (countMatching < 3)
                continue;

            _ = LCRNG.Next16(ref seed); // vBlank, setting IVs is slow

            // Determine inherited IVs
            ApplyInheritanceEmerald(seed, tmp);
            // Don't care which parent passes the IVs.
            // Done.

            // Check if the IVs are valid
            var count = GetCountMatchInherit(ivs, tmp);
            if (count != 6)
                continue;

            frame = LCRNG.Prev4(frame); // unroll once, and account for interaction lag (vBlank)
            var advances = LCRNG.GetDistance(0, frame);
            origin = new Daycare3Origin(frame, advances, 0);

            return true;
        }
    }

    /// <remarks>
    /// Usable by all versions (R/S/FR/LG), excluding Emerald.
    /// </remarks>
    /// <inheritdoc cref="TryGetOriginSeed(ReadOnlySpan{int}, uint, GameVersion, out Daycare3Origin)"/>
    public static bool TryGetOriginSeed(ReadOnlySpan<int> ivs, uint pid, out Daycare3Origin origin)
    {
        // Frame pattern:
        // PID high
        // **vBlank
        // IVs low (overwritten by inheritance later)
        // IVs high (overwritten by inheritance later)
        // **vBlank
        // Inheritance

        Span<int> tmp = stackalloc int[6];
        origin = default;

        // PID is the first Rand() call. We don't know the lower bits of the seed, so try all.
        // The seed that produces the IV pattern with the lowest amount of advances from a 16-bit seed is our result.
        // For Ruby Sapphire RNG abused eggs, ideally we discover 0x05A0 is the initial seed.
        var high = pid & 0xFFFF0000;

        // Assume the most-frequent vBlank placement in the calculation sequence.
        // (after PID, after IVs)
        for (uint i = 0; i < 0x10000; i++)
        {
            var seed = high | i; // PID high; unknown lower portion of seed.

            _ = LCRNG.Next16(ref seed); // vBlank, set PID and all misc PKM properties before IVs
            var iv1 = LCRNG.Next16(ref seed);
            var iv2 = LCRNG.Next16(ref seed);
            Fill(tmp, iv1, iv2);

            var countMatching = GetCountMatch(ivs, tmp);
            if (countMatching < 3)
                continue;

            _ = LCRNG.Next16(ref seed); // vBlank, setting IVs is slow

            // Determine inherited IVs
            ApplyInheritance(seed, tmp);
            // Don't care which parent passes the IVs.
            // Done.

            // Check if the IVs are valid
            var count = GetCountMatchInherit(ivs, tmp);
            if (count != 6)
                continue;

            var generate = LCRNG.Prev4(high | i); // unroll once, and account for interaction lag (vBlank)
            UpdateIfBetter(ref origin, generate);
        }

        return origin.Pattern != Daycare3Correlation.None;
    }

    private static void ApplyInheritance(uint seed, Span<int> tmp)
    {
        Span<int> statIndexes = stackalloc int[6];
        for (int i = 0; i < 6; i++)
            statIndexes[i] = i;

        for (int i = 0; i < 3; i++)
        {
            var index = (int)LCRNG.Next16(ref seed) % (6 - i);
            var inherit = statIndexes[index];
            for (int j = index + 1; j < 6; j++)
                statIndexes[j - 1] = statIndexes[j];

            tmp[inherit] = -1; // Inherit this stat
        }
    }

    private static void ApplyInheritanceEmerald(uint seed, Span<int> tmp)
    {
        // Game Bug: Instead of removing the IV that was just picked, this
        // removes position 0 (HP) then position 1 (DEF), then position 2.
        Span<int> statIndexes = stackalloc int[6];
        for (int i = 0; i < 6; i++)
            statIndexes[i] = i;

        for (int i = 0; i < 3; i++)
        {
            var index = (int)LCRNG.Next16(ref seed) % (6 - i);
            var inherit = statIndexes[index];
            for (int j = /* index */ i + 1; j < 6; j++)
                statIndexes[j - 1] = statIndexes[j];

            tmp[inherit] = -1; // Inherit this stat
        }
    }

    private static void Fill(Span<int> tmp, uint iv1, uint iv2)
    {
        tmp[0] = (byte)(iv1 & 0x1F);
        tmp[1] = (byte)((iv1 >> 5) & 0x1F);
        tmp[2] = (byte)((iv1 >> 10) & 0x1F);
        tmp[3] = (byte)(iv2 & 0x1F);
        tmp[4] = (byte)((iv2 >> 5) & 0x1F);
        tmp[5] = (byte)((iv2 >> 10) & 0x1F);
    }

    private static int GetCountMatch(ReadOnlySpan<int> actual, ReadOnlySpan<int> tmp)
    {
        int count = 0;
        for (int i = 0; i < actual.Length; i++)
        {
            if (actual[i] == tmp[i])
                count++;
        }
        return count;
    }

    private static int GetCountMatchInherit(ReadOnlySpan<int> actual, ReadOnlySpan<int> tmp)
    {
        int count = 0;
        for (int i = 0; i < actual.Length; i++)
        {
            var iv = tmp[i];
            if (iv == -1 || actual[i] == iv)
                count++;
        }
        return count;
    }

    private static void UpdateIfBetter(ref Daycare3Origin best, uint origin, Daycare3Correlation pattern = Daycare3Correlation.Regular)
    {
        // Determine initial seed
        var seed = LCRNG.Prev9(origin); // arbitrary un-hittable frames
        while (seed >> 16 != 0)
            seed = LCRNG.Prev(seed);

        var advances = LCRNG.GetDistance(seed, origin);

        // Check if the seed is better
        if (best.Advances >= advances || best.Pattern is Daycare3Correlation.None)
            best = new Daycare3Origin(origin, advances, (ushort)seed, pattern);
    }
}

/// <summary>
/// Stores initial seed information for Gen3 daycare seeds.
/// </summary>
/// <param name="Origin">Seed that originates the egg on pickup from Daycare.</param>
/// <param name="Advances">Advances from the initial seed to the origin seed.</param>
/// <param name="Initial">Initial seed from the start of the game.</param>
/// <param name="Pattern">Generation pattern of the egg.</param>
public readonly record struct Daycare3Origin(uint Origin, uint Advances, ushort Initial, Daycare3Correlation Pattern = Daycare3Correlation.Regular);

public enum Daycare3Correlation
{
    /// <summary>
    /// None detected, usually just a sentinel value.
    /// </summary>
    None,

    /// <summary>
    /// Standard vBlank pattern.
    /// </summary>
    Regular,

    // Other patterns may be added in the future if other examples necessitate it.
}
