using System;
using System.Runtime.CompilerServices;

namespace PKHeX.Core;

/// <summary>
/// Method J logic used by <see cref="GameVersion.DPPt"/> RNG.
/// </summary>
public static class MethodJ
{
    // Summary of Random Determinations:
    // Nature:                       rand() / 0xA3E == nature (1/25 odds)
    // Cute Charm:                   rand() / 0x5556 != 0; (2/3 odds)
    // Sync:                         rand() >> 15 == 0; (50% odds)
    // Static/Magnet Pull:           rand() >> 15 == 0;
    // Pressure/Hustle/Vital Spirit: rand() >> 15 == 1;
    // Intimidate/Keen Eye:          rand() >> 15 == 1; -- 0 will reject the encounter.

    private const byte Format = 4;

    private static bool IsCuteCharmFail(uint rand) => (rand / 0x5556) == 0; // 1/3 odds
    private static bool IsCuteCharmPass(uint rand) => (rand / 0x5556) != 0; // 2/3 odds

    private static bool IsSyncFail(uint rand) => (rand >> 15) != 0;
    private static bool IsSyncPass(uint rand) => (rand >> 15) == 0;

    private static bool IsStaticMagnetFail(uint rand) => (rand >> 15) != 0;
    private static bool IsStaticMagnetPass(uint rand) => (rand >> 15) == 0;

    private static bool IsHustleVitalFail(uint rand) => (rand >> 15) != 1;
    private static bool IsHustleVitalPass(uint rand) => (rand >> 15) == 1;

    private static bool IsIntimidateKeenEyeFail(uint rand) => (rand >> 15) != 1;
    private static bool IsIntimidateKeenEyePass(uint rand) => (rand >> 15) == 1;

    private static uint GetNature(uint rand) => rand / 0xA3Eu;

    public static (uint Origin, LeadRequired Lead) GetOriginSeed(EncounterSlot4 enc, uint seed, byte nature, int reverseCount, byte levelMin, byte levelMax, byte format = Format)
    {
        while (true)
        {
            if (TryGetMatch(enc, levelMin, levelMax, seed, nature, format, out var result))
                return result;
            if (reverseCount == 0)
                break;
            reverseCount--;
            seed = LCRNG.Prev2(seed);
        }
        return (default, LeadRequired.Fail);
    }

    public static bool TryGetMatchCuteCharm(EncounterSlot4 enc, ReadOnlySpan<uint> seeds, byte nature, byte levelMin, byte levelMax, out uint result)
    {
        foreach (uint seed in seeds)
        {
            var p0 = seed >> 16; // 0
            var reg = GetNature(p0) == nature;
            if (!reg)
                continue;
            var ctx = new FrameCheckDetails<EncounterSlot4>(enc, seed, levelMin, levelMax, 4);
            if (TryGetMatchCuteCharm(ctx, out result))
                return true;
        }
        result = default; return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool TryGetMatch(EncounterSlot4 enc, byte levelMin, byte levelMax, uint seed, byte nature, byte format, out (uint Origin, LeadRequired Lead) result)
    {
        var p0 = seed >> 16; // 0
        var reg = GetNature(p0) == nature;
        if (reg)
        {
            var ctx = new FrameCheckDetails<EncounterSlot4>(enc, seed, levelMin, levelMax, format);
            return TryGetMatchNoSync(ctx, out result);
        }
        var syncProc = IsSyncPass(p0);
        if (syncProc)
        {
            var ctx = new FrameCheckDetails<EncounterSlot4>(enc, seed, levelMin, levelMax, format);
            if (IsSlotValidFrom1Skip(ctx, out seed))
            {
                result = (seed, LeadRequired.Synchronize);
                return true;
            }
        }
        result = default;
        return false;
    }

    private static bool TryGetMatchCuteCharm(in FrameCheckDetails<EncounterSlot4> ctx, out uint result)
    {
        if (IsCuteCharmFail(ctx.Prev1))
        { result = default; return false; }

        return IsSlotValidFrom1Skip(ctx, out result);
    }

    private static bool IsSlotValidCuteCharmFail(in FrameCheckDetails<EncounterSlot4> ctx, out uint result)
    {
        if (IsCuteCharmPass(ctx.Prev1)) // should have proc'd
        { result = default; return false; }

        return IsSlotValidFrom1Skip(ctx, out result);
    }

    private static bool IsSlotValidSyncFail(in FrameCheckDetails<EncounterSlot4> ctx, out uint result)
    {
        if (IsSyncPass(ctx.Prev1)) // should have proc'd
        { result = default; return false; }

        return IsSlotValidFrom1Skip(ctx, out result);
    }

    private static bool IsSlotValidInitimidate(in FrameCheckDetails<EncounterSlot4> ctx, out uint result)
    {
        if (IsIntimidateKeenEyePass(ctx.Prev1)) // encounter routine aborted
        { result = default; return false; }

        return IsSlotValidFrom1Skip(ctx, out result);
    }

    private static bool IsSlotValidHustleVitalFail(in FrameCheckDetails<EncounterSlot4> ctx, out uint result)
    {
        if (IsHustleVitalPass(ctx.Prev1)) // should have proc'd
        { result = default; return false; }

        return IsSlotValidFrom1Skip(ctx, out result);
    }

    private static bool TryGetMatchNoSync(in FrameCheckDetails<EncounterSlot4> ctx, out (uint Origin, LeadRequired Lead) result)
    {
        if (IsSlotValidRegular(ctx, out uint seed))
        { result = (seed, LeadRequired.None); return true; }

        if (IsSlotValidSyncFail(ctx, out seed))
        { result = (seed, LeadRequired.SynchronizeFail); return true; }
        if (IsSlotValidCuteCharmFail(ctx, out seed))
        { result = (seed, LeadRequired.CuteCharmFail); return true; }
        if (IsSlotValidHustleVitalFail(ctx, out seed))
        { result = (seed, LeadRequired.PressureHustleSpiritFail); return true; }
        if (IsSlotValidStaticMagnetFail(ctx, out seed))
        { result = (seed, LeadRequired.StaticMagnetFail); return true; }
        // Intimidate/Keen Eye failing will result in no encounter.

        if (IsSlotValidStaticMagnet(ctx, out seed))
        { result = (seed, LeadRequired.StaticMagnet); return true; }
        if (IsSlotValidHustleVital(ctx, out seed))
        { result = (seed, LeadRequired.PressureHustleSpirit); return true; }
        if (IsSlotValidInitimidate(ctx, out seed))
        { result = (seed, LeadRequired.IntimidateKeenEye); return true; }

        result = default; return false;
    }

    private static bool IsSlotValidFrom1Skip(FrameCheckDetails<EncounterSlot4> ctx, out uint result)
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

    private static bool IsSlotValidRegular(in FrameCheckDetails<EncounterSlot4> ctx, out uint result)
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

    private static bool IsSlotValidHustleVital(in FrameCheckDetails<EncounterSlot4> ctx, out uint result)
    {
        if (IsHustleVitalFail(ctx.Prev1)) // should have proc'd
        { result = default; return false; }

        if (!IsOriginalLevelValid(ctx.LevelMin, ctx.LevelMax, ctx.Format, ctx.Encounter.LevelMax))
        { result = default; return false; }

        if (!ctx.Encounter.IsFixedLevel())
        {
            // Don't bother evaluating Prev1 for level, as it's always bumped to max after.
            if (IsSlotValid(ctx.Encounter, ctx.Prev2))
            { result = ctx.Seed3; return true; }
        }
        else // Not random level
        {
            if (IsSlotValid(ctx.Encounter, ctx.Prev3))
            { result = ctx.Seed4; return true; }
        }
        result = default; return false;
    }

    private static bool IsSlotValidStaticMagnet(in FrameCheckDetails<EncounterSlot4> ctx, out uint result)
    {
        if (!ctx.Encounter.IsFixedLevel())
        {
            if (IsStaticMagnetFail(ctx.Prev3)) // should have proc'd
            { result = default; return false; }

            if (IsLevelValid(ctx.Encounter, ctx.LevelMin, ctx.LevelMax, ctx.Format, ctx.Prev1))
            {
                if (IsSlotValidStaticMagnet(ctx.Encounter, ctx.Prev2))
                { result = ctx.Seed4; return true; }
            }
        }
        else // Not random level
        {
            if (IsStaticMagnetFail(ctx.Prev3)) // should have proc'd
            { result = default; return false; }

            if (IsSlotValidStaticMagnet(ctx.Encounter, ctx.Prev1))
            { result = ctx.Seed3; return true; }
        }
        result = default; return false;
    }

    private static bool IsSlotValidStaticMagnetFail(in FrameCheckDetails<EncounterSlot4> ctx, out uint result)
    {
        if (!ctx.Encounter.IsFixedLevel())
        {
            if (IsStaticMagnetPass(ctx.Prev3)) // should have proc'd
            { result = default; return false; }

            if (IsLevelValid(ctx.Encounter, ctx.LevelMin, ctx.LevelMax, ctx.Format, ctx.Prev1))
            {
                if (IsSlotValid(ctx.Encounter, ctx.Prev2))
                { result = ctx.Seed4; return true; }
            }
        }
        else // Not random level
        {
            if (IsStaticMagnetPass(ctx.Prev2)) // should have proc'd
            { result = default; return false; }

            if (IsSlotValid(ctx.Encounter, ctx.Prev1))
            { result = ctx.Seed3; return true; }
        }
        result = default; return false;
    }

    private static bool IsSlotValid(EncounterSlot4 enc, uint u16SlotRand)
    {
        var slot = SlotRange.JSlot(enc.Type, u16SlotRand);
        return slot == enc.SlotNumber;
    }

    public static bool IsSlotValidStaticMagnet<T>(T enc, uint u16SlotRand) where T : IMagnetStatic
    {
        if (enc.IsStaticSlot && u16SlotRand % enc.StaticCount == enc.StaticIndex)
            return true;
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

    /// <summary>
    /// Gets the amount of reverses allowed prior to another satisfactory PID being generated.
    /// </summary>
    /// <param name="seed">Seed that generates the expected resulting PID.</param>
    /// <param name="nature">Nature of the resulting PID.</param>
    /// <returns>Count of reverses allowed for no specific lead (not cute charm).</returns>
    public static int GetReversalWindow(uint seed, byte nature)
    {
        int ctr = 0;
        // Seed is currently the second RNG call. Unroll to the first.
        uint b = seed >> 16;
        while (true)
        {
            var a = LCRNG.Prev16(ref seed);
            var pid = b << 16 | a;
            if (pid % 25 == nature)
                break;
            b = LCRNG.Prev16(ref seed);
            ctr++;
        }
        return ctr;
    }
}
