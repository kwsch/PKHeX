using System;
using System.Runtime.CompilerServices;
using static PKHeX.Core.LeadRequired;

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
    public static (uint Seed, LeadRequired Lead) GetSeed<TEnc, TEvo>(TEnc enc, uint seed, TEvo evo)
        where TEnc : IEncounterSlot34
        where TEvo : ILevelRange
    {
        var pid = GetPID(seed);
        var nature = (byte)(pid % 25);

        var frames = GetReversalWindow(seed, nature);
        return GetOriginSeed(enc, seed, nature, frames, evo.LevelMin, evo.LevelMax);
    }

    /// <inheritdoc cref="GetSeed{TEnc, TEvo}(TEnc, uint, TEvo)"/>
    public static (uint Seed, LeadRequired Lead) GetSeed<TEnc>(TEnc enc, uint seed)
        where TEnc : IEncounterSlot34 => GetSeed(enc, seed, enc);

    private static uint GetPID(uint seed)
    {
        var a = LCRNG.Next16(ref seed);
        var b = LCRNG.Next16(ref seed);
        return b << 16 | a;
    }

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

    private static bool IsSyncFail(uint rand) => (rand & 1) != 0;
    private static bool IsSyncPass(uint rand) => (rand & 1) == 0;

    private static bool IsStaticMagnetFail(uint rand) => (rand & 1) != 0;
    private static bool IsStaticMagnetPass(uint rand) => (rand & 1) == 0;

    private static bool IsHustleVitalFail(uint rand) => (rand & 1) != 1;
    private static bool IsHustleVitalPass(uint rand) => (rand & 1) == 1;

    private static bool IsIntimidateKeenEyeFail(uint rand) => (rand & 1) != 1;
    private static bool IsIntimidateKeenEyePass(uint rand) => (rand & 1) == 1;

    private static uint GetNature(uint rand) => rand % 25;

    /// <summary>
    /// Gets the first possible origin seed and lead for the input encounter &amp; constraints.
    /// </summary>
    public static (uint Origin, LeadRequired Lead) GetOriginSeed<T>(T enc, uint seed, byte nature, int reverseCount, byte levelMin, byte levelMax, byte format = Format)
        where T : IEncounterSlot34
    {
        (uint Origin, LeadRequired Lead) prefer = (default, Fail);
        while (true)
        {
            if (TryGetMatch(enc, levelMin, levelMax, seed, nature, format, out var result))
            {
                if (CheckEncounterActivation(enc, ref result))
                {
                    if (result.Lead == None)
                        return result;
                    if (prefer.Lead == Fail || result.Lead < prefer.Lead)
                        prefer = result;
                }
            }
            if (reverseCount == 0)
                return prefer;
            reverseCount--;
            seed = LCRNG.Prev2(seed);
        }
    }

    private static bool CheckEncounterActivation<T>(T enc, ref (uint Origin, LeadRequired Lead) result)
        where T : IEncounterSlot34
    {
        if (enc.Type.IsFishingRodType())
            return IsFishPossible(enc.Type, ref result.Origin, ref result.Lead);
        if (enc.Type is SlotType.Rock_Smash)
            return IsRockSmashPossible(enc.AreaRate, ref result.Origin, ref result.Lead);
        // Can sweet scent trigger.
        return true;
    }

    private static bool CheckEncounterActivation<T>(T enc, ref uint result) where T : IEncounterSlot34
    {
        // Lead is required to be Cute Charm.
        if (enc.Type.IsFishingRodType())
            return IsFishPossible(enc.Type, ref result);
        if (enc.Type is SlotType.Rock_Smash)
            return IsRockSmashPossible(enc.AreaRate, ref result);
        // Can sweet scent trigger.
        return true;
    }

    /// <summary>
    /// Attempts to find a matching seed for the given encounter and constraints for Cute Charm buffered PIDs.
    /// </summary>
    public static bool TryGetMatchCuteCharm<T>(T enc, ReadOnlySpan<uint> seeds, byte nature, byte levelMin, byte levelMax, out uint result)
        where T : IEncounterSlot34
    {
        foreach (uint seed in seeds)
        {
            var p0 = seed >> 16; // 0
            var reg = GetNature(p0) == nature;
            if (!reg)
                continue;
            var ctx = new FrameCheckDetails<T>(enc, seed, levelMin, levelMax, 4);
            if (!TryGetMatchCuteCharm(ctx, out result))
                continue;
            if (CheckEncounterActivation(enc, ref result))
                return true;
        }
        result = default; return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool TryGetMatch<T>(T enc, byte levelMin, byte levelMax, uint seed, byte nature, byte format, out (uint Origin, LeadRequired Lead) result)
        where T : IEncounterSlot34
    {
        var p0 = seed >> 16; // 0
        var reg = GetNature(p0) == nature;
        if (reg)
        {
            var ctx = new FrameCheckDetails<T>(enc, seed, levelMin, levelMax, format);
            return TryGetMatchNoSync(ctx, out result);
        }
        var syncProc = IsSyncPass(p0);
        if (syncProc)
        {
            var ctx = new FrameCheckDetails<T>(enc, seed, levelMin, levelMax, format);
            if (IsSlotValidRegular(ctx, out seed))
            {
                result = (seed, Synchronize);
                return true;
            }
        }
        result = default;
        return false;
    }

    private static bool TryGetMatchCuteCharm<T>(in FrameCheckDetails<T> ctx, out uint result)
        where T : IEncounterSlot34
    {
        if (IsCuteCharmFail(ctx.Prev1))
        { result = default; return false; }

        return IsSlotValidFrom1Skip(ctx, out result);
    }

    private static bool IsSlotValidCuteCharmFail<T>(in FrameCheckDetails<T> ctx, out uint result)
        where T : IEncounterSlot34
    {
        if (IsCuteCharmPass(ctx.Prev1)) // should have triggered
        { result = default; return false; }

        return IsSlotValidFrom1Skip(ctx, out result);
    }

    private static bool IsSlotValidSyncFail<T>(in FrameCheckDetails<T> ctx, out uint result)
        where T : IEncounterSlot34
    {
        if (IsSyncPass(ctx.Prev1)) // should have triggered
        { result = default; return false; }

        return IsSlotValidFrom1Skip(ctx, out result);
    }

    private static bool IsSlotValidIntimidate<T>(in FrameCheckDetails<T> ctx, out uint result)
        where T : IEncounterSlot34
    {
        if (IsIntimidateKeenEyePass(ctx.Prev1)) // encounter routine aborted
        { result = default; return false; }

        return IsSlotValidFrom1Skip(ctx, out result);
    }

    private static bool IsSlotValidHustleVitalFail<T>(in FrameCheckDetails<T> ctx, out uint result)
        where T : IEncounterSlot34
    {
        if (IsHustleVitalPass(ctx.Prev1)) // should have triggered
        { result = default; return false; }

        return IsSlotValidFrom1Skip(ctx, out result);
    }

    private static bool TryGetMatchNoSync<T>(in FrameCheckDetails<T> ctx, out (uint Origin, LeadRequired Lead) result)
        where T : IEncounterSlot34
    {
        if (IsSlotValidRegular(ctx, out uint seed))
        { result = (seed, None); return true; }

        if (IsSlotValidSyncFail(ctx, out seed))
        { result = (seed, SynchronizeFail); return true; }
        if (IsSlotValidCuteCharmFail(ctx, out seed))
        { result = (seed, CuteCharmFail); return true; }
        if (IsSlotValidHustleVitalFail(ctx, out seed))
        { result = (seed, PressureHustleSpiritFail); return true; }
        if (IsSlotValidStaticMagnetFail(ctx, out seed))
        { result = (seed, StaticMagnetFail); return true; }
        // Intimidate/Keen Eye failing will result in no encounter.

        if (IsSlotValidStaticMagnet(ctx, out seed))
        { result = (seed, StaticMagnet); return true; }
        if (IsSlotValidHustleVital(ctx, out seed))
        { result = (seed, PressureHustleSpirit); return true; }
        if (IsSlotValidIntimidate(ctx, out seed))
        { result = (seed, IntimidateKeenEye); return true; }

        result = default; return false;
    }

    private static bool IsSlotValidFrom1Skip<T>(FrameCheckDetails<T> ctx, out uint result)
        where T : IEncounterSlot34
    {
        if (!ctx.Encounter.IsFixedLevel())
        {
            if (IsLevelValid(ctx.Encounter, ctx.LevelMin, ctx.LevelMax, ctx.Format, ctx.Prev2))
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
        where T : IEncounterSlot34
    {
        if (!ctx.Encounter.IsFixedLevel())
        {
            if (IsLevelValid(ctx.Encounter, ctx.LevelMin, ctx.LevelMax, ctx.Format, ctx.Prev1))
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
        where T : IEncounterSlot34
    {
        if (IsHustleVitalFail(ctx.Prev1)) // should have triggered
        { result = default; return false; }

        var expectLevel = ctx.Encounter.PressureLevel;
        if (!IsOriginalLevelValid(ctx.LevelMin, ctx.LevelMax, ctx.Format, expectLevel))
        { result = default; return false; }

        if (!ctx.Encounter.IsFixedLevel())
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

    private static bool IsSlotValidStaticMagnet<T>(in FrameCheckDetails<T> ctx, out uint result)
        where T : IEncounterSlot34
    {
        // Static or Magnet Pull
        // -3 SlotProc (Random % 2 == 0)
        // -2 ESV (select slot)
        // -1 Level
        //  0 Nature
        if (!ctx.Encounter.IsFixedLevel())
        {
            if (IsStaticMagnetFail(ctx.Prev3)) // should have triggered
            { result = default; return false; }

            if (IsLevelValid(ctx.Encounter, ctx.LevelMin, ctx.LevelMax, ctx.Format, ctx.Prev1))
            {
                if (IsSlotValidStaticMagnet(ctx.Encounter, ctx.Prev2))
                { result = ctx.Seed4; return true; }
            }
        }
        else // Not random level
        {
            if (IsStaticMagnetFail(ctx.Prev2)) // should have triggered
            { result = default; return false; }

            if (IsSlotValidStaticMagnet(ctx.Encounter, ctx.Prev1))
            { result = ctx.Seed3; return true; }
        }
        result = default; return false;
    }

    private static bool IsSlotValidStaticMagnetFail<T>(in FrameCheckDetails<T> ctx, out uint result)
        where T : IEncounterSlot34
    {
        if (!ctx.Encounter.IsFixedLevel())
        {
            if (IsStaticMagnetPass(ctx.Prev3)) // should have triggered
            { result = default; return false; }

            if (IsLevelValid(ctx.Encounter, ctx.LevelMin, ctx.LevelMax, ctx.Format, ctx.Prev1))
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
        where T : IEncounterSlot34
    {
        var slot = SlotRange.KSlot(enc.Type, u16SlotRand);
        return slot == enc.SlotNumber;
    }

    private static bool IsSlotValidStaticMagnet<T>(T enc, uint u16SlotRand) where T : IMagnetStatic
    {
        if (enc.IsStaticSlot && u16SlotRand % enc.StaticCount == enc.StaticIndex)
            return true;
        // Isn't checked for Fishing slots, but no fishing slots are steel type -- always false.
        if (enc.IsMagnetSlot && u16SlotRand % enc.MagnetPullCount == enc.MagnetPullIndex)
            return true;
        return false;
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

    private static bool IsFishPossible(SlotType encType, ref uint seed, ref LeadRequired lead)
    {
        var rate = GetFishingThreshold(encType);
        var u16 = seed >> 16;
        var roll = u16 % 100;
        if (roll < rate)
        {
            seed = LCRNG.Prev(seed);
            return true;
        }

        if (lead != None)
            return false;

        // Suction Cups / Sticky Hold
        if (roll < rate * 2)
        {
            seed = LCRNG.Prev(seed);
            lead = SuctionCups;
            return true;
        }

        return false;
    }

    private static bool IsFishPossible(SlotType encType, ref uint seed)
    {
        var rate = GetFishingThreshold(encType);
        var u16 = seed >> 16;
        var roll = u16 % 100;
        if (roll < rate)
        {
            seed = LCRNG.Prev(seed);
            return true;
        }
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
        var u16 = seed >> 16;
        var roll = u16 % 100;
        if (roll < areaRate)
        {
            seed = LCRNG.Prev(seed);
            return true;
        }
        if (lead != None)
            return false;
        if (roll < areaRate * 2)
        {
            seed = LCRNG.Prev(seed);
            lead = SuctionCups;
            return true;
        }
        return false;
    }

    private static byte GetFishingThreshold(SlotType type) => type switch
    {
        SlotType.Old_Rod => 25,
        SlotType.Good_Rod => 50,
        SlotType.Super_Rod => 75,
        _ => 0,
    };
}
