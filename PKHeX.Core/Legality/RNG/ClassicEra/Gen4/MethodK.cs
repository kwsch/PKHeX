using System;
using System.Runtime.CompilerServices;
using static PKHeX.Core.LeadRequired;
using static PKHeX.Core.SlotType4;

namespace PKHeX.Core;

/// <summary>
/// Method K logic used by <see cref="GameVersion.HGSS"/> RNG.
/// </summary>
public static class MethodK
{
    /// <summary>
    /// High-level method to get the first possible encounter conditions.
    /// </summary>
    /// <param name="enc">Encounter template.</param>
    /// <param name="seed">Seed that immediately generates the PID.</param>
    /// <param name="evo">Level range constraints for the capture, if known.</param>
    /// <param name="format">Current format (different from 4)</param>
    public static LeadSeed GetSeed<TEnc, TEvo>(TEnc enc, uint seed, TEvo evo, byte format = Format)
        where TEnc : IEncounterSlot4
        where TEvo : ILevelRange
        => GetSeed(enc, seed, evo.LevelMin, evo.LevelMax, format);

    public static LeadSeed GetSeed<TEnc>(TEnc enc, uint seed, byte levelMin, byte levelMax, byte format = Format, int depth = 0)
        where TEnc : IEncounterSlot4
    {
        var pid = ClassicEraRNG.GetSequentialPID(seed);
        var nature = (byte)(pid % 25);

        var frames = GetReversalWindow(seed, nature);
        return GetOriginSeed(enc, seed, nature, frames, levelMin, levelMax, format, depth);
    }

    /// <inheritdoc cref="GetSeed{TEnc, TEvo}(TEnc, uint, TEvo, byte)"/>
    public static LeadSeed GetSeed<TEnc>(TEnc enc, uint seed)
        where TEnc : IEncounterSlot4 => GetSeed(enc, seed, enc);

    /// <inheritdoc cref="MethodJ.GetReversalWindow"/>
    /// <returns>Count of reverses allowed for no specific lead (not cute charm).</returns>
    public static int GetReversalWindow(uint seed, byte nature) => MethodJ.GetReversalWindow(seed, nature);

    // Summary of Random Determinations:
    // Nature:                       rand() % 25 == nature
    // Cute Charm:                   rand() % 3 != 0; (2/3 odds)
    // Sync:                         rand() & 1 == 0; (50% odds)
    // Static/Magnet Pull:           rand() & 1 == 0;
    // Pressure/Hustle/Vital Spirit: rand() & 1 == 1;
    // Intimidate/Keen Eye:          rand() & 1 == 1; -- 0 will reject the encounter.

    private const byte Format = 4;

    private static bool IsCuteCharmFail(uint rand) => (rand % 3) == 0; // 1/3 odds
    private static bool IsCuteCharmPass(uint rand) => (rand % 3) != 0; // 2/3 odds

  //private static bool IsSyncFail(uint rand) => (rand & 1) != 0;
    private static bool IsSyncPass(uint rand) => (rand & 1) == 0;

    private static bool IsStaticMagnetFail(uint rand) => (rand & 1) != 0;
    private static bool IsStaticMagnetPass(uint rand) => (rand & 1) == 0;

    private static bool IsHustleVitalFail(uint rand) => (rand & 1) != 1;
    private static bool IsHustleVitalPass(uint rand) => (rand & 1) == 1;

  //private static bool IsIntimidateKeenEyeFail(uint rand) => (rand & 1) != 1;
    private static bool IsIntimidateKeenEyePass(uint rand) => (rand & 1) == 1;

    private static uint GetNature(uint rand) => rand % 25;

    /// <summary>
    /// Gets the first possible origin seed and lead for the input encounter &amp; constraints.
    /// </summary>
    public static LeadSeed GetOriginSeed<T>(T enc, uint seed, byte nature, int reverseCount, byte levelMin, byte levelMax, byte format = Format, int depth = 0)
        where T : IEncounterSlot4
    {
        LeadSeed prefer = default;
        while (true)
        {
            if (TryGetMatch(enc, levelMin, levelMax, seed, nature, format, out var result, depth))
            {
                if (result.IsNoRequirement())
                    return result;
                if (result.IsBetterThan(prefer))
                    prefer = result;
            }
            if (reverseCount == 0)
                return prefer;
            reverseCount--;
            seed = LCRNG.Prev2(seed);
        }
    }

    public static bool IsEncounterCheckApplicable(SlotType4 type) => type is Rock_Smash or BugContest || type.IsFishingRodType();

    public static bool CheckEncounterActivation<T>(T enc, ref LeadSeed result)
        where T : IEncounterSlot4
    {
        if (enc.Type.IsFishingRodType())
            return IsFishPossible(enc.Type, ref result.Seed, ref result.Lead);
        if (enc.Type is Rock_Smash)
            return IsRockSmashPossible(enc.AreaRate, ref result.Seed, ref result.Lead);

        // Ability & Sweet Scent deadlock for BCC:
        if (enc.Type is BugContest && !result.Lead.IsAbleToSweetScent())
            return IsBugContestPossibleDeadlock(enc.AreaRate, ref result.Seed);

        // Can sweet scent trigger.
        return true;
    }

    private static bool IsAbleToSweetScent(this LeadRequired lead) => lead
        is None // Pretty much anything grass.
        or IntimidateKeenEyeFail // Masquerain & Mawile
        or PressureHustleSpirit or PressureHustleSpiritFail // Vespiquen
        // Synchronize: None
        // Cute Charm: None
        // Static/Magnet Pull: None
    ;

    private static bool CheckEncounterActivationCuteCharm<T>(T enc, ref uint result) where T : IEncounterSlot4
    {
        // Lead is required to be Cute Charm.
        if (enc.Type.IsFishingRodType())
            return IsFishPossible(enc.Type, ref result);
        if (enc.Type is Rock_Smash)
            return IsRockSmashPossible(enc.AreaRate, ref result);

        // Ability & Sweet Scent deadlock for BCC:
        if (enc.Type is BugContest) // No species with Sweet Scent can have Cute Charm.
            return IsBugContestPossibleDeadlock(enc.AreaRate, ref result);

        // Can sweet scent trigger.
        return true;
    }

    /// <summary>
    /// Attempts to find a matching seed for the given encounter and constraints for Cute Charm buffered PIDs.
    /// </summary>
    public static bool TryGetMatchCuteCharm<T>(T enc, ReadOnlySpan<uint> seeds, byte nature, byte levelMin, byte levelMax, byte format, out LeadSeed result)
        where T : IEncounterSlot4
    {
        foreach (uint seed in seeds)
        {
            var p0 = seed >> 16; // 0
            var reg = GetNature(p0) == nature;
            if (!reg)
                continue;
            var ctx = new FrameCheckDetails<T>(enc, seed, levelMin, levelMax, format);
            if (!TryGetMatchCuteCharm(ctx, out var s))
                continue;
            if (!CheckEncounterActivationCuteCharm(enc, ref s))
                continue;

            result = new(s, CuteCharm);
            return true;
        }
        result = default; return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool TryGetMatch<T>(T enc, byte levelMin, byte levelMax, uint seed, byte nature, byte format, out LeadSeed result, int depth = 0)
        where T : IEncounterSlot4
    {
        var p0 = seed >> 16; // 0
        var reg = GetNature(p0) == nature;
        if (reg)
        {
            var ctx = new FrameCheckDetails<T>(enc, seed, levelMin, levelMax, format);
            if (TryGetMatchNoSync(ctx, out result) && CheckEncounterActivation(enc, ref result))
                return true;
            if (depth != 4 && enc is EncounterSlot4 s && (s.IsBugContest || s.IsSafariHGSS))
                return Recurse4x(enc, levelMin, levelMax, seed, nature, format, out result, ++depth);
        }
        else if (IsSyncPass(p0) && !(enc.Type is Grass && enc.LevelMax < levelMin))
        {
            var ctx = new FrameCheckDetails<T>(enc, seed, levelMin, levelMax, format);
            if (IsSlotValidRegular(ctx, out seed))
            {
                result = new(seed, Synchronize);
                if (CheckEncounterActivation(enc, ref result))
                    return true;
            }
            if (depth != 4 && enc is EncounterSlot4 s && (s.IsBugContest || s.IsSafariHGSS))
                return Recurse4x(enc, levelMin, levelMax, seed, nature, format, out result, ++depth);
        }
        result = default;
        return false;
    }

    private const int MaxSafariContest = 4;

    private static bool Recurse4x<T>(T enc, byte levelMin, byte levelMax, uint seed, byte nature, byte format,
        out LeadSeed result, int depth)
        where T : IEncounterSlot4
    {
        // When generating Pokémon's IVs, the game tries to give at least one flawless IV.
        // The game will roll the {nature,PID/IV} up to 4 times if none of the IVs are at 31 (total of 3 re-rolls)
        result = default;

        // Use the depth to keep track of how many we have already burned.
        // First entry to this method will be 1/4 burned (final result).
        if (depth == MaxSafariContest)
            return false;

        // Check if the previous 4 frames were a 31-IV Pokémon. If so, it couldn't have been re-rolled from.
        // If it was, we can adjust our parameters and try the entire search again (expensive!)

        // First we need to determine how far back we can permit the previous skipped encounter to be originating from.
        // Our only requirement is that the nature PID we currently have not been generated.
        // This is the same "solved" problem as the regular reversal window.

        // Each loop we have a ~18.75% chance to hit a >=1 31 IV setup and return false.
        // On average, we look back 5-6 times.
        bool breakNext = false;
        while (true)
        {
            var iv2 = seed >> 16;
            if (IsAny31(iv2))
                return false;

            var iv1 = LCRNG.Prev16(ref seed);
            if (IsAny31(iv1))
                return false;

            // Since we're looking backwards and doing recursion in the same method, we need to ensure we don't exceed our previous nature window.
            var sanityNature = ClassicEraRNG.GetSequentialPID(seed) % 25;
            if (sanityNature == nature)
                breakNext = true;

            // Cool, the skipped Pokémon was not 31-IV. Let's try again.
            // The skipped Pokémon probably had a different nature, so we need to use that value instead.
            // We basically need to repeat the entire top-level check with slightly adjusted parameters.
            var origin = LCRNG.Prev2(seed);

            // We need to double-check that this skipped PID/IV could have been landed on via the nature/sync check.
            // The innate recursion will return true if the skipped frame could have been landed on, or if an even-more previous skipped was landed.
            result = GetSeed(enc, origin, levelMin, levelMax, format, depth);
            if (result.IsValid())
                return true;

            // If the forwards window no longer lets us land on the frame we entered this method from, the window is exhausted.
            // We need to do at least one recursion as the 4 calls we look backwards from will hide a double-nature PID/frame pair.
            if (breakNext)
                return false; // Window exhausted, this nature would have been chosen instead of the frame we entered this method from.
        }
    }

    /// <summary>
    /// Checks if any IV component value is 31.
    /// </summary>
    /// <remarks>BCC re-rolls 3x if none are 31.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsAny31(uint iv16)
           => IsLow5Bits31(iv16)
           || IsLow5Bits31(iv16 >> 5)
           || IsLow5Bits31(iv16 >> 10);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsLow5Bits31(uint iv16) => (iv16 & 0x1F) == 0x1F;

    private static bool TryGetMatchCuteCharm<T>(in FrameCheckDetails<T> ctx, out uint result)
        where T : IEncounterSlot4
    {
        if (IsCuteCharmFail(ctx.Prev1))
        { result = default; return false; }

        return IsSlotValidFrom1Skip(ctx, out result);
    }

    private static bool IsSlotValidCuteCharmFail<T>(in FrameCheckDetails<T> ctx, out uint result)
        where T : IEncounterSlot4
    {
        if (IsCuteCharmPass(ctx.Prev1)) // should have triggered
        { result = default; return false; }

        return IsSlotValidFrom1Skip(ctx, out result);
    }

    private static bool IsSlotValidSyncFail<T>(in FrameCheckDetails<T> ctx, out uint result)
        where T : IEncounterSlot4
    {
        if (IsSyncPass(ctx.Prev1)) // should have triggered
        { result = default; return false; }

        return IsSlotValidFrom1Skip(ctx, out result);
    }

    private static bool IsSlotValidIntimidate<T>(in FrameCheckDetails<T> ctx, out uint result)
        where T : IEncounterSlot4
    {
        if (IsIntimidateKeenEyePass(ctx.Prev1)) // encounter routine aborted
        { result = default; return false; }

        return IsSlotValidFrom1Skip(ctx, out result);
    }

    private static bool IsSlotValidHustleVitalFail<T>(in FrameCheckDetails<T> ctx, out uint result)
        where T : IEncounterSlot4
    {
        if (IsHustleVitalPass(ctx.Prev1)) // should have triggered
        { result = default; return false; }

        return IsSlotValidFrom1Skip(ctx, out result);
    }

    private static bool TryGetMatchNoSync<T>(in FrameCheckDetails<T> ctx, out LeadSeed result)
        where T : IEncounterSlot4
    {
        if (ctx.Encounter.Type is Grass)
        {
            if (ctx.Encounter.LevelMax < ctx.LevelMin) // Must be boosted via Pressure/Hustle/Vital Spirit
            {
                if (IsSlotValidHustleVital(ctx, out var pressure))
                { result = new(pressure, PressureHustleSpirit); return true; }
                result = default; return false;
            }
        }

        if (IsSlotValidRegular(ctx, out uint seed))
        { result = new(seed, None); return true; }

        if (IsSlotValidSyncFail(ctx, out seed))
        { result = new(seed, SynchronizeFail); return true; }
        if (IsSlotValidCuteCharmFail(ctx, out seed))
        { result = new(seed, CuteCharmFail); return true; }
        if (IsSlotValidHustleVitalFail(ctx, out seed))
        { result = new(seed, PressureHustleSpiritFail); return true; }
        if (IsSlotValidStaticMagnetFail(ctx, out seed))
        { result = new(seed, StaticMagnetFail); return true; }
        // Intimidate/Keen Eye failing will result in no encounter.

        if (IsSlotValidStaticMagnet(ctx, out seed, out var sm))
        { result = new(seed, sm); return true; }
        if (IsSlotValidIntimidate(ctx, out seed))
        { result = new(seed, IntimidateKeenEyeFail); return true; }
        if (ctx.Encounter.PressureLevel <= ctx.LevelMax) // Can be boosted, or not.
        {
            if (IsSlotValidHustleVital(ctx, out var pressure))
            { result = new(pressure, PressureHustleSpirit); return true; }
        }

        result = default; return false;
    }

    public static bool IsLevelRand<T>(T enc) where T : IEncounterSlot4 => enc.Type.IsLevelRandHGSS();

    private static bool IsSlotValidFrom1Skip<T>(FrameCheckDetails<T> ctx, out uint result)
        where T : IEncounterSlot4
    {
        if (IsLevelRand(ctx.Encounter))
        {
            if (ctx.Encounter.IsFixedLevel() || IsLevelValid(ctx.Encounter, ctx.LevelMin, ctx.LevelMax, ctx.Format, ctx.Prev2))
            {
                if (IsSlotValid(ctx.Encounter, ctx.Prev3))
                { result = ctx.Seed4; return true; }
            }
        }
        else // Not random level
        {
            if (IsSlotValid(ctx.Encounter, ctx.Prev2))
            { result = ctx.Seed3; return true; }
        }
        result = default; return false;
    }

    private static bool IsSlotValidRegular<T>(in FrameCheckDetails<T> ctx, out uint result)
        where T : IEncounterSlot4
    {
        if (IsLevelRand(ctx.Encounter))
        {
            if (ctx.Encounter.IsFixedLevel() || IsLevelValid(ctx.Encounter, ctx.LevelMin, ctx.LevelMax, ctx.Format, ctx.Prev1))
            {
                if (IsSlotValid(ctx.Encounter, ctx.Prev2))
                { result = ctx.Seed3; return true; }
            }
        }
        else // Not random level
        {
            if (IsSlotValid(ctx.Encounter, ctx.Prev1))
            { result = ctx.Seed2; return true; }
        }
        result = default; return false;
    }

    private static bool IsSlotValidHustleVital<T>(in FrameCheckDetails<T> ctx, out uint result)
        where T : IEncounterSlot4
    {
        if (IsHustleVitalFail(ctx.Prev1)) // should have triggered
        { result = default; return false; }

        var expectLevel = ctx.Encounter.PressureLevel;
        if (!IsOriginalLevelValid(ctx.LevelMin, ctx.LevelMax, ctx.Format, expectLevel))
        { result = default; return false; }

        if (IsLevelRand(ctx.Encounter))
        {
            // Don't bother evaluating Prev1 for level, as it's always bumped to max after.
            if (IsSlotValid(ctx.Encounter, ctx.Prev3))
            { result = ctx.Seed4; return true; }
        }
        else // Not random level
        {
            if (IsSlotValid(ctx.Encounter, ctx.Prev2))
            { result = ctx.Seed3; return true; }
        }
        result = default; return false;
    }

    private static bool IsSlotValidStaticMagnet<T>(in FrameCheckDetails<T> ctx, out uint result, out LeadRequired lead)
        where T : IEncounterSlot4
    {
        // Static or Magnet Pull
        // -3 SlotProc (Random % 2 == 0)
        // -2 ESV (select slot)
        // -1 Level
        //  0 Nature
        lead = None;
        if (IsLevelRand(ctx.Encounter))
        {
            if (IsStaticMagnetFail(ctx.Prev3)) // should have triggered
            { result = default; return false; }

            if (ctx.Encounter.IsFixedLevel() || IsLevelValid(ctx.Encounter, ctx.LevelMin, ctx.LevelMax, ctx.Format, ctx.Prev1))
            {
                if (ctx.Encounter.IsSlotValidStaticMagnet(ctx.Prev2, out lead))
                { result = ctx.Seed4; return true; }
            }
        }
        else // Not random level
        {
            if (IsStaticMagnetFail(ctx.Prev2)) // should have triggered
            { result = default; return false; }

            if (ctx.Encounter.IsSlotValidStaticMagnet(ctx.Prev1, out lead))
            { result = ctx.Seed3; return true; }
        }
        result = default; return false;
    }

    private static bool IsSlotValidStaticMagnetFail<T>(in FrameCheckDetails<T> ctx, out uint result)
        where T : IEncounterSlot4
    {
        if (IsLevelRand(ctx.Encounter))
        {
            if (IsStaticMagnetPass(ctx.Prev3)) // should have triggered
            { result = default; return false; }

            if (ctx.Encounter.IsFixedLevel() || IsLevelValid(ctx.Encounter, ctx.LevelMin, ctx.LevelMax, ctx.Format, ctx.Prev1))
            {
                if (IsSlotValid(ctx.Encounter, ctx.Prev2))
                { result = ctx.Seed4; return true; }
            }
        }
        else // Not random level
        {
            if (IsStaticMagnetPass(ctx.Prev2)) // should have triggered
            { result = default; return false; }

            if (IsSlotValid(ctx.Encounter, ctx.Prev1))
            { result = ctx.Seed2; return true; }
        }
        result = default; return false;
    }

    private static bool IsSlotValid<T>(T enc, uint u16SlotRand)
        where T : IEncounterSlot4
    {
        var slot = SlotMethodK.GetSlot(enc.Type, u16SlotRand);
        return slot == enc.SlotNumber;
    }

    private static bool IsLevelValid<T>(T enc, byte min, byte max, byte format, uint u16LevelRand) where T : ILevelRange
    {
        var level = GetExpectedLevel(enc, u16LevelRand);
        return IsOriginalLevelValid(min, max, format, level);
    }

    private static bool IsOriginalLevelValid(byte min, byte max, byte format, uint level)
    {
        if (format == Format)
            return level == min; // Met Level matches
        return LevelRangeExtensions.IsLevelWithinRange((int)level, min, max);
    }

    private static uint GetExpectedLevel(ILevelRange enc, uint u16LevelRand)
    {
        uint mod = 1u + enc.LevelMax - enc.LevelMin;
        return (u16LevelRand % mod) + enc.LevelMin;
    }

    private static bool IsBugContestPossibleDeadlock(uint areaRate, ref uint result)
    {
        // BCC only allows one Pokémon to be in the party.
        // Specific lead abilities can learn Sweet Scent, while others cannot.
        // The only entry into this method requires an ability that has no species available with Sweet Scent.
        // Therefore, without Sweet Scent, we need to trigger via turning/walking.
        // With an area rate of 25, this arrangement will succeed 37% of the time.

        // The game checks 2 random calls to trigger the encounter: movement -> rate -> generate.
        // HG/SS has an underflow error (via radio) which can pass the first rand call for movement.
        // Only need to check the second call for rate.
        // Rate can be improved by 50% if the White Flute is used.
        // Other abilities can also affect the rate, but we can't use them with our current lead.
        var rate = areaRate + (areaRate >> 1); // +50% White Flute
        var rand = (result >> 16);
        var roll = rand % 100;
        if (roll >= rate)
            return false;

        // Skip backwards before the two calls. Valid encounter seed found.
        result = LCRNG.Prev2(result);
        return true;
    }

    private static bool IsFishPossible(SlotType4 encType, ref uint seed, ref LeadRequired lead)
    {
        var rodRate = GetRodRate(encType);
        var u16 = seed >> 16;
        var roll = u16 % 100;

        // HG/SS: Lead (Following) Pokémon with >= 250 adds +50 to the rate. Assume the best case.
        rodRate += 50; // This happens before Suction Cups / Sticky Hold, can be compounded.
        if (roll < rodRate) // This will always succeed for Good/Super rod due to the base+bonus being >=100
        {
            seed = LCRNG.Prev(seed);
            return true;
        }

        // Old Rod might reach here (75% < 100%)
        if (lead != None)
            return false;

        // Suction Cups / Sticky Hold
        if (roll < rodRate * 2)
        {
            seed = LCRNG.Prev(seed);
            lead = SuctionCups;
            return true;
        }
        return false;
    }

    // Lead is something else, and cannot be changed. Does the same as the above method without a ref LeadRequired.
    private static bool IsFishPossible(SlotType4 encType, ref uint seed)
    {
        var rodRate = GetRodRate(encType);
        var u16 = seed >> 16;
        var roll = u16 % 100;

        // HG/SS: Lead (Following) Pokémon with >= 250 adds +50 to the rate. Assume the best case.
        rodRate += 50; // This happens before Suction Cups / Sticky Hold, can be compounded.
        if (roll < rodRate) // This will always succeed for Good/Super rod due to the base+bonus being >=100
        {
            seed = LCRNG.Prev(seed);
            return true;
        }

        // Old Rod might reach here (75% < 100%)
        return false;
    }

    private static bool IsRockSmashPossible(byte areaRate, ref uint seed)
    {
        var u16 = seed >> 16;
        var roll = u16 % 100;
        if (roll < areaRate)
        {
            seed = LCRNG.Prev(seed);
            return true;
        }
        return false;
    }

    private static bool IsRockSmashPossible(byte areaRate, ref uint seed, ref LeadRequired lead)
    {
        // No flute boost.
        var u16 = seed >> 16;
        var roll = u16 % 100;
        if (roll < areaRate)
        {
            seed = LCRNG.Prev(seed);
            return true;
        }
        if (lead != None)
            return false;
        if (roll < areaRate * 2u)
        {
            seed = LCRNG.Prev(seed);
            lead = Illuminate;
            return true;
        }
        return false;
    }

    private static byte GetRodRate(SlotType4 type) => type switch
    {
        Old_Rod => 25,
        Good_Rod => 50,
        Super_Rod => 75,

        Safari_Old_Rod => 25,
        Safari_Good_Rod => 50,
        Safari_Super_Rod => 75,

        _ => 0,
    };

    private static bool IsFishingRodType(this SlotType4 t) => t
        is Old_Rod
        or Good_Rod
        or Super_Rod

        or Safari_Old_Rod
        or Safari_Good_Rod
        or Safari_Super_Rod;
}
