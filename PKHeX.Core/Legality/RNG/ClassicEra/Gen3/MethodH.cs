using System.Runtime.CompilerServices;
using static PKHeX.Core.MethodHCondition;
using static PKHeX.Core.LeadRequired;
using static PKHeX.Core.SlotType3;

namespace PKHeX.Core;

/// <summary>
/// Method H logic used by mainline <see cref="GameVersion.Gen3"/> RNG.
/// </summary>
public static class MethodH
{
    /// <summary>
    /// High-level method to get the first possible encounter conditions.
    /// </summary>
    /// <param name="enc">Encounter template.</param>
    /// <param name="seed">Seed that immediately generates the PID.</param>
    /// <param name="evo">Level range constraints for the capture, if known.</param>
    /// <param name="emerald">Version encountered in (either Emerald or not)</param>
    /// <param name="gender">Gender encountered as</param>
    /// <param name="format">Current format (different from 3)</param>
    public static LeadSeed GetSeed<TEnc, TEvo>(TEnc enc, uint seed, TEvo evo, bool emerald, byte gender, byte format)
        where TEnc : IEncounterSlot3
        where TEvo : ILevelRange
    {
        var pid = enc.Species != (ushort)Species.Unown
            ? ClassicEraRNG.GetSequentialPID(seed)
            : ClassicEraRNG.GetReversePID(seed);
        var nature = (byte)(pid % 25);

        var info = GetReversalWindow(enc, seed, nature, emerald, gender);
        return GetOriginSeed(info, enc, seed, nature, evo.LevelMin, evo.LevelMax, format);
    }

    /// <inheritdoc cref="GetSeed{TEnc,TEvo}"/>
    public static LeadSeed GetSeed<TEnc>(TEnc enc, uint seed, bool emerald, byte gender, byte format)
        where TEnc : IEncounterSlot3 => GetSeed(enc, seed, enc, emerald, gender, format);

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

  //private static bool IsSyncFail(uint rand) => (rand & 1) != 0;
    private static bool IsSyncPass(uint rand) => (rand & 1) == 0;

    private static bool IsStaticMagnetFail(uint rand) => (rand & 1) != 0;
    private static bool IsStaticMagnetPass(uint rand) => (rand & 1) == 0;

    private static bool IsHustleVitalFail(uint rand) => (rand & 1) != 1;
    private static bool IsHustleVitalPass(uint rand) => (rand & 1) == 1;

  //private static bool IsIntimidateKeenEyeFail(uint rand) => (rand & 1) != 1;
    private static bool IsIntimidateKeenEyePass(uint rand) => (rand & 1) == 1;
    private static bool IsSafariBlockProc(uint seed) => (seed >> 16) % 100 < 80;

    private static uint GetNature(uint rand) => rand % 25;
    private static bool IsMatchUnown(uint pid, byte nature, byte form) => pid % 25 == nature && EntityPID.GetUnownForm3(pid) == form;

    private static bool IsMatchGender(uint pid, byte genderMin, byte genderMax)
    {
        var gender = pid & 0xFF;
        return genderMin <= gender && gender <= genderMax;
    }

    /// <summary>
    /// Gets the appropriate <see cref="MethodHCondition"/> for the species and game.
    /// </summary>
    public static MethodHCondition GetCondition(ushort species, GameVersion version)
    {
        if (species == (ushort)Species.Unown)
            return Unown;
        if (version == GameVersion.E)
            return Emerald;
        return Regular;
    }

    /// <summary>
    /// Caches details useful for the rolling look-back allowed for the input seed.
    /// </summary>
    public static MethodHWindowInfo GetReversalWindow<T>(T enc, uint seed, byte nature, bool emerald, byte gender)
        where T : IEncounterSlot3
    {
        if (emerald)
        {
            var pi = PersonalTable.E[enc.Species];
            var ratio = pi.Gender;
            if (ratio == PersonalInfo.RatioMagicGenderless)
                return new((ushort)GetReversalWindow(seed, nature), Emerald);

            var (min, max) = PersonalInfo.GetGenderMinMax(gender, ratio);
            var result = GetReversalWindowCute(seed, nature, min, max);
            return new((ushort)result.NoLead, Emerald, ratio, (ushort)result.Cute);
        }
        if (enc.Species == (int)Species.Unown) // Never Emerald
        {
            var result = GetReversalWindow(seed, nature, enc.Form);
            return new((ushort)result, Unown);
        }
        // Otherwise...
        {
            var result = GetReversalWindow(seed, nature);
            return new((ushort)result, Regular);
        }
    }

    /// <summary>
    /// Gets the first possible origin seed and lead for the input encounter &amp; constraints.
    /// </summary>
    public static LeadSeed GetOriginSeed<T>(in MethodHWindowInfo info, T enc, uint seed, byte nature, byte levelMin, byte levelMax, byte format = Format)
        where T : IEncounterSlot3
    {
        return info.Type == Emerald
            ? GetOriginSeedEmerald(enc, seed, nature, info.CountRegular, info.CountCute, levelMin, levelMax, format)
            : GetOriginSeed(enc, seed, nature, info.CountRegular, levelMin, levelMax, format);
    }

    private static LeadSeed GetOriginSeedEmerald<T>(T enc, uint seed, byte nature, int reverseCount, int revCute, byte levelMin, byte levelMax, byte format = Format)
        where T : IEncounterSlot3
    {
        LeadSeed prefer = default;
        while (true)
        {
            if (TryGetMatch(enc, levelMin, levelMax, seed, nature, format, out var result))
            {
                if (CheckEncounterActivationEmerald(enc, ref result))
                {
                    if (result.IsNoAbilityLead())
                        return result;
                    if (result.IsBetterThan(prefer))
                        prefer = result;
                }
            }

            if (reverseCount == 0)
            {
                // If we haven't found a lead, we can try checking for Cute Charm if allowed.
                if (revCute == 0 || prefer.IsValid())
                    return prefer;
                break;
            }
            reverseCount--;
            seed = LCRNG.Prev2(seed);
        }

        while (true)
        {
            if (TryGetMatch(enc, levelMin, levelMax, seed, nature, format, out var result)
                && result.IsNoAbilityLead())
            {
                result.Lead = CuteCharm;
                if (CheckEncounterActivationEmerald(enc, ref result))
                {
                    if (result.IsNoAbilityLead())
                        return result;
                    if (result.IsBetterThan(prefer))
                        prefer = result;
                }
            }
            revCute--;
            if (revCute == 0)
                return prefer;
            seed = LCRNG.Prev2(seed);
        }
    }

    private static bool CheckEncounterActivationEmerald<T>(T enc, ref LeadSeed result)
        where T : IEncounterSlot3
    {
        if (enc.Type is Rock_Smash)
            return IsRockSmashPossible(enc.AreaRate, ref result.Seed);
        if (enc.Type.IsFishingRodType())
            return true; // can just wait and trigger after hooking.

        // Can sweet scent trigger.
        return true;
    }

    private static LeadSeed GetOriginSeed<T>(T enc, uint seed, byte nature, int reverseCount, byte levelMin, byte levelMax, byte format = Format)
        where T : IEncounterSlot3
    {
        while (true)
        {
            if (TryGetMatchNoLead(enc, levelMin, levelMax, seed, nature, format, out var result))
            {
                if (CheckEncounterActivation(enc, ref result))
                    return result;
            }
            if (reverseCount == 0)
                break;
            reverseCount--;
            seed = LCRNG.Prev2(seed);
        }
        return default;
    }

    public static bool IsEncounterCheckApplicable(SlotType3 type) => type is Rock_Smash; // Fishing can use Sticky/Suction along with Friendship boost.

    public static bool CheckEncounterActivation<T>(T enc, ref LeadSeed result)
        where T : IEncounterSlot3
    {
        if (enc.Type is Rock_Smash)
            return IsRockSmashPossible(enc.AreaRate, ref result.Seed);
        if (enc.Type.IsFishingRodType())
            return true; // can just wait and trigger after hooking.
        // Can sweet scent trigger.
        return true;
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
    private static bool TryGetMatchNoLead<T>(T enc, byte levelMin, byte levelMax, uint seed, byte nature, byte format, out LeadSeed result)
        where T : IEncounterSlot3
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
                var ctx = new FrameCheckDetails<T>(enc, safariBlockSeed, levelMin, levelMax, format);
                if (IsSlotValidRegular(ctx, out uint origin))
                { result = new(origin, None); return true; }
            }
        }
        var reg = GetNature(p0) == nature;
        if (reg)
        {
            // Still consumes a dummy call between nature and prior.
            // Assume no Safari Block was placed, so that the nature rolls are used.
            if (rseSafari)
                seed = LCRNG.Prev(seed);

            var ctx = new FrameCheckDetails<IEncounterSlot3>(enc, seed, levelMin, levelMax, format);
            if (IsSlotValidRegular(ctx, out uint origin))
            { result = new(origin, None); return true; }
        }
        result = default; return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool TryGetMatch<T>(T enc, byte levelMin, byte levelMax, uint seed, byte nature, byte format, out LeadSeed result)
        where T : IEncounterSlot3
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
                var ctx = new FrameCheckDetails<IEncounterSlot3>(enc, safariBlockSeed, levelMin, levelMax, format);
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
            var ctx = new FrameCheckDetails<IEncounterSlot3>(enc, seed, levelMin, levelMax, format);
            if (IsSlotValidRegular(ctx, out seed))
            {
                result = new(seed, Synchronize);
                return true;
            }
        }
        var reg = GetNature(p0) == nature;
        if (reg)
        {
            var ctx = new FrameCheckDetails<IEncounterSlot3>(enc, seed, levelMin, levelMax, format);
            if (TryGetMatchNoSync(ctx, out result))
                return true;
        }
        result = default; return false;
    }

    private static bool TryGetMatchCuteCharm(in FrameCheckDetails<IEncounterSlot3> ctx, out uint result)
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

    private static bool IsSlotValidCuteCharmFail(in FrameCheckDetails<IEncounterSlot3> ctx, out uint result)
    {
        // Cute Charm
        // -3 ESV
        // -2 Level
        // -1 CC Proc (Random() % 3 == 0)
        //  0 Nature
        if (IsCuteCharmPass(ctx.Prev1)) // should have triggered
        { result = default; return false; }

        return IsSlotValidFrom1Skip(ctx, out result);
    }

    private static bool IsSlotValidSyncFail(in FrameCheckDetails<IEncounterSlot3> ctx, out uint result)
    {
        // Keen Eye, Intimidate (Not compatible with Sweet Scent)
        // -3 ESV
        // -2 Level
        // -1 Sync Proc (Random() % 2) FAIL
        //  0 Nature
        if (IsSyncPass(ctx.Prev1)) // should have triggered
        { result = default; return false; }

        return IsSlotValidFrom1Skip(ctx, out result);
    }

    private static bool IsSlotValidIntimidate(in FrameCheckDetails<IEncounterSlot3> ctx, out uint result)
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

    private static bool IsSlotValidHustleVitalFail(in FrameCheckDetails<IEncounterSlot3> ctx, out uint result)
    {
        // Pressure, Hustle, Vital Spirit = Force Maximum Level from slot
        // -3 ESV
        // -2 Level
        // -1 LevelMax proc (Random() & 1) FAIL
        //  0 Nature
        if (IsHustleVitalPass(ctx.Prev1)) // should have triggered
        { result = default; return false; }

        return IsSlotValidFrom1Skip(ctx, out result);
    }

    private static bool TryGetMatchNoSync(in FrameCheckDetails<IEncounterSlot3> ctx, out LeadSeed result)
    {
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

        if (IsSlotValidStaticMagnet(ctx, out seed, out var lead))
        { result = new(seed, lead); return true; }
        if (IsSlotValidHustleVital(ctx, out seed))
        { result = new(seed, PressureHustleSpirit); return true; }
        if (IsSlotValidIntimidate(ctx, out seed))
        { result = new(seed, IntimidateKeenEyeFail); return true; }
        if (TryGetMatchCuteCharm(ctx, out seed))
        { result = new(seed, CuteCharm); return true; }

        result = default; return false;
    }

    private static bool IsSlotValidFrom1Skip(FrameCheckDetails<IEncounterSlot3> ctx, out uint result)
    {
        // -3 ESV
        // -2 Level
        // -1 (Proc Already Checked)
        //  0 Nature
        if (IsLevelValid(ctx.Encounter, ctx.LevelMin, ctx.LevelMax, ctx.Format, ctx.Prev2))
        {
            if (IsSlotValid(ctx.Encounter, ctx.Prev3))
            { result = ctx.Seed4; return true; }
        }
        result = default; return false;
    }

    private static bool IsSlotValidRegular<T>(in FrameCheckDetails<T> ctx, out uint result)
        where T : IEncounterSlot3
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

    private static bool IsSlotValidHustleVital<T>(in FrameCheckDetails<T> ctx, out uint result)
        where T : IEncounterSlot3
    {
        // Pressure, Hustle, Vital Spirit = Force Maximum Level from slot
        // -3 ESV
        // -2 Level
        // -1 LevelMax proc (Random() & 1)
        //  0 Nature
        if (IsHustleVitalFail(ctx.Prev1)) // should have triggered
        { result = default; return false; }

        var expectLevel = ctx.Encounter.PressureLevel;
        if (!IsOriginalLevelValid(ctx.LevelMin, ctx.LevelMax, ctx.Format, expectLevel))
        { result = default; return false; }

        // Level is always rand(), but...
        {
            // Don't bother evaluating Prev2 for level, as it's always bumped to max after.
            if (IsSlotValid(ctx.Encounter, ctx.Prev3))
            { result = ctx.Seed4; return true; }
        }
        result = default; return false;
    }

    private static bool IsSlotValidStaticMagnet<T>(in FrameCheckDetails<T> ctx, out uint result, out LeadRequired lead)
        where T : IEncounterSlot3
    {
        // Static or Magnet Pull
        // -3 SlotProc (Random % 2 == 0)
        // -2 ESV (select slot)
        // -1 Level
        //  0 Nature
        lead = None;
        if (IsStaticMagnetFail(ctx.Prev3)) // should have triggered
        { result = default; return false; }

        if (IsLevelValid(ctx.Encounter, ctx.LevelMin, ctx.LevelMax, ctx.Format, ctx.Prev1))
        {
            if (ctx.Encounter.IsSlotValidStaticMagnet(ctx.Prev2, out lead))
            { result = ctx.Seed4; return true; }
        }
        result = default; return false;
    }

    private static bool IsSlotValidStaticMagnetFail<T>(in FrameCheckDetails<T> ctx, out uint result)
        where T : IEncounterSlot3
    {
        // Static or Magnet Pull
        // -3 SlotProc (Random % 2 == 1) FAIL
        // -2 ESV (select slot)
        // -1 Level
        //  0 Nature
        if (IsStaticMagnetPass(ctx.Prev3)) // should have triggered
        { result = default; return false; }

        if (IsLevelValid(ctx.Encounter, ctx.LevelMin, ctx.LevelMax, ctx.Format, ctx.Prev1))
        {
            if (IsSlotValid(ctx.Encounter, ctx.Prev2))
            { result = ctx.Seed4; return true; }
        }
        result = default; return false;
    }

    private static bool IsSlotValid<T>(T enc, uint u16SlotRand)
        where T : IEncounterSlot3, INumberedSlot
    {
        var slot = SlotMethodH.GetSlot(enc.Type, u16SlotRand);
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

    private static bool IsRockSmashPossible(byte areaRate, ref uint seed)
    {
        if (IsRatePass(seed, areaRate, None)) // Lead doesn't matter, doesn't influence.
        {
            seed = LCRNG.Prev(seed);
            return true;
        }
        return false;
    }

    private const ushort MaxEncounterRate = 2880; // 0xB40

    private static bool IsRatePass(uint seed, byte areaRate, LeadRequired lead, bool ignoreAbility = true)
    {
        var u16 = seed >> 16;
        var encRate = GetEncounterRate(areaRate, lead, ignoreAbility);
        return u16 % MaxEncounterRate < encRate;
    }

    private static uint GetEncounterRate(byte areaRate, LeadRequired lead, bool ignoreAbility)
    {
        uint encRate = areaRate * 16u;
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

    private static bool IsFishingRodType(this SlotType3 t) => t
        is Old_Rod
        or Good_Rod
        or Super_Rod
        or SwarmFish50;
}
