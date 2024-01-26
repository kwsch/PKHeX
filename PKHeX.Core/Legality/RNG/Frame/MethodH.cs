using System.Runtime.CompilerServices;
using static PKHeX.Core.MethodHCondition;
using static PKHeX.Core.LeadRequired;

namespace PKHeX.Core;

public enum MethodHCondition : byte
{
    Empty = 0, // Invalid sentinel value
    Regular,
    Emerald,
    Unown,
}

/// <summary>
/// Cache-able representation of the Method H window information.
/// </summary>
/// <param name="CountRegular">Count of reversals allowed for no specific lead (not requiring cute charm).</param>
/// <param name="Type">Type of Method H logic.</param>
/// <param name="Gender">Gender Ratio of the encountered Pokémon.</param>
/// <param name="CountCute">Count of reversals allowed for a matching cute charm lead.</param>
public readonly record struct MethodHWindowInfo(ushort CountRegular, MethodHCondition Type, byte Gender = 0, ushort CountCute = 0)
{
    private bool IsUninitialized => Type == Empty;
    private bool IsGenderRatioDifferentCuteCharm(ushort Species) => Type == Emerald && PersonalTable.E[Species].Gender != Gender;

    public bool ShouldRevise(ushort Species) => IsUninitialized || IsGenderRatioDifferentCuteCharm(Species);
}

/// <summary>
/// Method H logic used by mainline <see cref="GameVersion.Gen3"/> RNG.
/// </summary>
public static class MethodH
{
    // Summary of Random Determinations:
    // Nature:                       rand() % 25 == nature
    // Cute Charm:                   rand() % 3 != 0; (2/3 odds)
    // Sync:                         rand() & 1 == 0; (50% odds)
    // Static/Magnet Pull:           rand() & 1 == 0;
    // Pressure/Hustle/Vital Spirit: rand() & 1 == 1;
    // Intimidate/Keen Eye:          rand() & 1 == 1; -- 0 will reject the encounter.

    private const byte Format = 3;

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
    private static bool IsSafariBlockProc(uint seed) => (seed >> 16) % 100 < 80;

    private static uint GetNature(uint rand) => rand % 25;
    private static bool IsMatchUnown(uint pid, byte nature, byte form) => pid % 25 == nature && EntityPID.GetUnownForm3(pid) == form;

    private static bool IsMatchGender(uint pid, byte genderMin, byte genderMax)
    {
        var gender = pid & 0xFF;
        return genderMin <= gender && gender <= genderMax;
    }

    public static MethodHCondition GetCondition(ushort species, GameVersion version)
    {
        if (species == (ushort)Species.Unown)
            return Unown;
        if (version == GameVersion.E)
            return Emerald;
        return Regular;
    }

    public static MethodHWindowInfo GetReversalWindow(EncounterSlot3 enc, uint seed, byte nature, byte version, int gender)
    {
        if (enc.Species == (int)Species.Unown)
        {
            var result = GetReversalWindow(seed, nature, enc.Form);
            return new((ushort)result, Unown);
        }

        if (version == (int)GameVersion.E)
        {
            var pi = PersonalTable.E[enc.Species];
            var ratio = pi.Gender;
            if (ratio == PersonalInfo.RatioMagicGenderless)
                return new((ushort)GetReversalWindow(seed, nature), Emerald);

            (byte min, byte max) = PersonalInfo.GetGenderMinMax(gender, ratio);
            var result = GetReversalWindowCute(seed, nature, min, max);
            return new((ushort)result.NoLead, Emerald, ratio, (ushort)result.Cute);
        }
        // Otherwise...
        {
            var result = GetReversalWindow(seed, nature);
            return new((ushort)result, Regular);
        }
    }

    public static (uint Origin, LeadRequired Lead) GetOriginSeed(in MethodHWindowInfo info, EncounterSlot3 enc, uint seed, byte nature, byte levelMin, byte levelMax, byte format = Format)
    {
        var result = info.Type == Emerald
            ? GetOriginSeedEmerald(enc, seed, nature, info.CountRegular, info.CountCute, levelMin, levelMax, format)
            : GetOriginSeed(enc, seed, nature, info.CountRegular, levelMin, levelMax, format);
        if (result.Lead == Fail)
            return result;

        if (enc.Type == SlotType.Rock_Smash)
        {
            // Check proc; every other type can be Sweet Scent triggered (fishing can wait at dialog dismissal).
            var areaRate = enc.Parent.Rate;
            if (!IsRatePass(result.Origin, areaRate, result.Lead, true))
                return (default, Fail); // failed to pass the rate check
            result = result with { Origin = LCRNG.Prev(result.Origin) };
        }

        return result;
    }

    private const ushort MaxEncounterRate = 2880; // 0xB40

    private static bool IsRatePass(uint seed, byte rate, LeadRequired lead, bool ignoreAbility = true)
    {
        var u16 = seed >> 16;
        var encRate = GetEncounterRate(rate, lead, ignoreAbility);
        return u16 % MaxEncounterRate < encRate;
    }

    private static uint GetEncounterRate(byte areaRateByte, LeadRequired lead, bool ignoreAbility)
    {
        uint encRate = areaRateByte * 16u;
        // We intend to pass the encounter, as we want an encounter to trigger.
        // Player on a Bike adjusts by *80 /100. We assume the player is not on a bike.
        // Cleanse Tag adjusts by *2 /3. We assume the player is not using a Cleanse Tag.
        // Black Flute adjusts by /2. We assume the player is not using a Black Flute.
        // White Flute adjusts by += /2. We assume the player is using a White Flute.***
        encRate += (encRate >> 1); // +50%

        if (!ignoreAbility && lead == None)
        {
            // Stench, White Smoke, and Sand Veil (in Sandstorm) halve the rate. We assume the player is not using any of these.
            // Illuminate and Arena Trap double the rate.
            encRate <<= 1; // *2
        }
        return encRate;
    }

    private static (uint Origin, LeadRequired Lead) GetOriginSeedEmerald(EncounterSlot3 enc, uint seed, byte nature, int reverseCount, int revCute, byte levelMin, byte levelMax, byte format = Format)
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
        while (true)
        {
            if (TryGetMatch(enc, levelMin, levelMax, seed, nature, format, out var result))
                return result;
            if (revCute == 0)
                break;
            revCute--;
            seed = LCRNG.Prev2(seed);
        }
        return (default, Fail);
    }

    private static (uint Origin, LeadRequired Lead) GetOriginSeed(EncounterSlot3 enc, uint seed, byte nature, int reverseCount, byte levelMin, byte levelMax, byte format = Format)
    {
        while (true)
        {
            if (TryGetMatchNoLead(enc, levelMin, levelMax, seed, nature, format, out var result))
                return result;
            if (reverseCount == 0)
                break;
            reverseCount--;
            seed = LCRNG.Prev2(seed);
        }
        return (default, Fail);
    }

    /// <inheritdoc cref="MethodJ.GetReversalWindow"/>
    private static int GetReversalWindow(uint seed, byte nature) => MethodJ.GetReversalWindow(seed, nature);

    /// <inheritdoc cref="MethodJ.GetReversalWindow"/>
    /// <param name="seed">Seed that generates the expected resulting PID.</param>
    /// <param name="nature">Nature of the resulting PID.</param>
    /// <param name="genderMin">Inclusive low-bound for the gender portion of the PID.</param>
    /// <param name="genderMax">Inclusive high-bound for the gender portion of the PID.</param>
    /// <returns>Count of reverses allowed for no specific lead (not requiring cute charm) and a matching cute charm lead.</returns>
    /// <remarks>Only accessible in Emerald for gendered Pokémon.</remarks>
    private static (int NoLead, int Cute) GetReversalWindowCute(uint seed, byte nature, byte genderMin, byte genderMax)
    {
        int noLead = 0;
        int ctr = 0;
        uint b = seed >> 16;
        while (true)
        {
            var a = LCRNG.Prev16(ref seed);
            var pid = b << 16 | a;
            if (pid % 25 == nature)
            {
                if (noLead == 0)
                    noLead = ctr;
                if (IsMatchGender(pid, genderMin, genderMax))
                    return (noLead, ctr - noLead);
            }
            b = LCRNG.Prev16(ref seed);
            ctr++;
        }
    }

    /// <inheritdoc cref="MethodJ.GetReversalWindow"/>
    /// <param name="seed">Seed that generates the expected resulting PID.</param>
    /// <param name="nature">Nature of the resulting PID.</param>
    /// <param name="form">Form of the resulting PID.</param>
    /// <remarks>Only used for FR/LG <see cref="Species.Unown"/>.</remarks>
    private static int GetReversalWindow(uint seed, byte nature, byte form)
    {
        // Reverses the PID calls, and ensures the form matches.
        int ctr = 0;
        uint b = seed >> 16;
        while (true)
        {
            var a = LCRNG.Prev16(ref seed);
            var pid = a << 16 | b;
            if (IsMatchUnown(pid, nature, form))
                break; // Correct Nature and Form.
            b = LCRNG.Prev16(ref seed);
            ctr++;
        }
        return ctr;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool TryGetMatchNoLead(EncounterSlot3 enc, byte levelMin, byte levelMax, uint seed, byte nature, byte format, out (uint Origin, LeadRequired Lead) result)
    {
        var p0 = seed >> 16; // 0

        var rseSafari = enc.IsSafariHoenn;
        if (rseSafari)
        {
            // Seek backwards to the activation check (80%).
            // The nature preference table consumes 300 RNG calls, so we need to jump back that far.
            // Below are the magic RNG multiply & add for 300 prev calls.
            var safariBlockSeed = (0xC048C851u * seed) + 0x196302B4u;
            if (IsSafariBlockProc(safariBlockSeed))
            {
                var ctx = new FrameCheckDetails<EncounterSlot3>(enc, safariBlockSeed, levelMin, levelMax, format);
                if (IsSlotValidRegular(ctx, out uint origin))
                { result = (origin, None); return true; }
            }
        }
        var reg = GetNature(p0) == nature;
        if (reg)
        {
            // Still consumes a dummy call between nature and prior.
            // Assume no Safari Block was placed, so that the nature rolls are used.
            if (rseSafari)
                seed = LCRNG.Prev(seed);

            var ctx = new FrameCheckDetails<EncounterSlot3>(enc, seed, levelMin, levelMax, format);
            if (IsSlotValidRegular(ctx, out uint origin))
            { result = (origin, None); return true; }
        }
        result = default; return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool TryGetMatch(EncounterSlot3 enc, byte levelMin, byte levelMax, uint seed, byte nature, byte format, out (uint Origin, LeadRequired Lead) result)
    {
        var p0 = seed >> 16; // 0

        if (enc.IsSafariHoenn)
        {
            // Seek backwards to the activation check (80%).
            // The nature preference table consumes 300 RNG calls, so we need to jump back that far.
            // Below are the magic RNG multiply & add for 300 prev calls.
            var safariBlockSeed = (0xC048C851u * seed) + 0x196302B4u;
            if (IsSafariBlockProc(safariBlockSeed))
            {
                var ctx = new FrameCheckDetails<EncounterSlot3>(enc, safariBlockSeed, levelMin, levelMax, format);
                if (TryGetMatchNoSync(ctx, out result))
                    return true;
            }

            // Still consumes a dummy call between nature and prior.
            // Assume no Safari Block was placed, so that the nature rolls are used.
            // Do this before checking either; it's a >50% chance of being a Sync or regular nature call.
            seed = LCRNG.Prev(seed);
        }

        var syncProc = IsSyncPass(p0);
        if (syncProc)
        {
            var ctx = new FrameCheckDetails<EncounterSlot3>(enc, seed, levelMin, levelMax, format);
            if (IsSlotValidFrom1Skip(ctx, out seed))
            {
                result = (seed, Synchronize);
                return true;
            }
        }
        var reg = GetNature(p0) == nature;
        if (reg)
        {
            var ctx = new FrameCheckDetails<EncounterSlot3>(enc, seed, levelMin, levelMax, format);
            if (TryGetMatchNoSync(ctx, out result))
                return true;
        }
        result = default; return false;
    }

    private static bool TryGetMatchCuteCharm(in FrameCheckDetails<EncounterSlot3> ctx, out uint result)
    {
        // Cute Charm
        // -3 ESV
        // -2 Level
        // -1 CC Proc (Random() % 3 != 0)
        //  0 Nature
        if (IsCuteCharmFail(ctx.Prev1))
        { result = default; return false; }

        return IsSlotValidFrom1Skip(ctx, out result);
    }

    private static bool IsSlotValidCuteCharmFail(in FrameCheckDetails<EncounterSlot3> ctx, out uint result)
    {
        // Cute Charm
        // -3 ESV
        // -2 Level
        // -1 CC Proc (Random() % 3 == 0)
        //  0 Nature
        if (IsCuteCharmPass(ctx.Prev1)) // should have proc'd
        { result = default; return false; }

        return IsSlotValidFrom1Skip(ctx, out result);
    }

    private static bool IsSlotValidSyncFail(in FrameCheckDetails<EncounterSlot3> ctx, out uint result)
    {
        // Keen Eye, Intimidate (Not compatible with Sweet Scent)
        // -3 ESV
        // -2 Level
        // -1 Sync Proc (Random() % 2) FAIL
        //  0 Nature
        if (IsSyncPass(ctx.Prev1)) // should have proc'd
        { result = default; return false; }

        return IsSlotValidFrom1Skip(ctx, out result);
    }

    private static bool IsSlotValidInitimidate(in FrameCheckDetails<EncounterSlot3> ctx, out uint result)
    {
        // Keen Eye, Intimidate (Not compatible with Sweet Scent)
        // -3 ESV
        // -2 Level
        // -1 Level Adequate Check !(Random() % 2 == 1) rejects --  rand%2==1 is adequate
        //  0 Nature
        // Note: if this check fails, the encounter generation routine is aborted.
        if (IsIntimidateKeenEyePass(ctx.Prev1)) // encounter routine aborted
        { result = default; return false; }

        return IsSlotValidFrom1Skip(ctx, out result);
    }

    private static bool IsSlotValidHustleVitalFail(in FrameCheckDetails<EncounterSlot3> ctx, out uint result)
    {
        // Pressure, Hustle, Vital Spirit = Force Maximum Level from slot
        // -3 ESV
        // -2 Level
        // -1 LevelMax proc (Random() & 1) FAIL
        //  0 Nature
        if (IsHustleVitalPass(ctx.Prev1)) // should have proc'd
        { result = default; return false; }

        return IsSlotValidFrom1Skip(ctx, out result);
    }

    private static bool TryGetMatchNoSync(in FrameCheckDetails<EncounterSlot3> ctx, out (uint Origin, LeadRequired Lead) result)
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
        if (IsSlotValidInitimidate(ctx, out seed))
        { result = (seed, IntimidateKeenEye); return true; }
        if (TryGetMatchCuteCharm(ctx, out seed))
        { result = (seed, CuteCharm); return true; }

        result = default; return false;
    }

    private static bool IsSlotValidFrom1Skip(FrameCheckDetails<EncounterSlot3> ctx, out uint result)
    {
        // -3 ESV
        // -2 Level
        // -1 (Proc Already Checked)
        //  0 Nature
        if (IsLevelValid(ctx.Encounter, ctx.LevelMin, ctx.LevelMax, ctx.Format, ctx.Prev3))
        {
            if (IsSlotValid(ctx.Encounter, ctx.Prev2))
            { result = ctx.Seed4; return true; }
        }
        result = default; return false;
    }

    private static bool IsSlotValidRegular(in FrameCheckDetails<EncounterSlot3> ctx, out uint result)
    {
        // -2 ESV
        // -1 Level
        //  0 Nature
        if (IsLevelValid(ctx.Encounter, ctx.LevelMin, ctx.LevelMax, ctx.Format, ctx.Prev1))
        {
            if (IsSlotValid(ctx.Encounter, ctx.Prev2))
            { result = ctx.Seed3; return true; }
        }
        result = default; return false;
    }

    private static bool IsSlotValidHustleVital(in FrameCheckDetails<EncounterSlot3> ctx, out uint result)
    {
        // Pressure, Hustle, Vital Spirit = Force Maximum Level from slot
        // -3 ESV
        // -2 Level
        // -1 LevelMax proc (Random() & 1)
        //  0 Nature
        if (IsHustleVitalFail(ctx.Prev1)) // should have proc'd
        { result = default; return false; }

        if (!IsOriginalLevelValid(ctx.LevelMin, ctx.LevelMax, ctx.Format, ctx.Encounter.LevelMax))
        { result = default; return false; }

        if (!ctx.Encounter.IsFixedLevel())
        {
            // Don't bother evaluating Prev2 for level, as it's always bumped to max after.
            if (IsSlotValid(ctx.Encounter, ctx.Prev3))
            { result = ctx.Seed4; return true; }
        }
        result = default; return false;
    }

    private static bool IsSlotValidStaticMagnet(in FrameCheckDetails<EncounterSlot3> ctx, out uint result)
    {
        // Static or Magnet Pull
        // -3 SlotProc (Random % 2 == 0)
        // -2 ESV (select slot)
        // -1 Level
        //  0 Nature
        if (IsStaticMagnetFail(ctx.Prev3)) // should have proc'd
        { result = default; return false; }

        if (IsLevelValid(ctx.Encounter, ctx.LevelMin, ctx.LevelMax, ctx.Format, ctx.Prev1))
        {
            if (IsSlotValidStaticMagnet(ctx.Encounter, ctx.Prev2))
            { result = ctx.Seed4; return true; }
        }
        result = default; return false;
    }

    private static bool IsSlotValidStaticMagnetFail(in FrameCheckDetails<EncounterSlot3> ctx, out uint result)
    {
        // Static or Magnet Pull
        // -3 SlotProc (Random % 2 == 1) FAIL
        // -2 ESV (select slot)
        // -1 Level
        //  0 Nature
        if (IsStaticMagnetPass(ctx.Prev3)) // should have proc'd
        { result = default; return false; }

        if (IsLevelValid(ctx.Encounter, ctx.LevelMin, ctx.LevelMax, ctx.Format, ctx.Prev1))
        {
            if (IsSlotValid(ctx.Encounter, ctx.Prev2))
            { result = ctx.Seed4; return true; }
        }
        result = default; return false;
    }

    private static bool IsSlotValid(EncounterSlot3 enc, uint u16SlotRand)
    {
        var slot = SlotRange.HSlot(enc.Type, u16SlotRand);
        return slot == enc.SlotNumber;
    }

    private static bool IsSlotValidStaticMagnet<T>(T enc, uint u16SlotRand) where T : IMagnetStatic
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
}
