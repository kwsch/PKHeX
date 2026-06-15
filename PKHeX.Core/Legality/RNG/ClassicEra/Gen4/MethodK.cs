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
    /// <param name="format">Current format (different from 4 will use level range instead of exact)</param>
    public static LeadSeed GetSeed<TEnc, TEvo>(TEnc enc, uint seed, TEvo evo, byte format)
        where TEnc : IEncounterSlot4
        where TEvo : ILevelRange
        => GetSeed(enc, seed, evo.LevelMin, evo.LevelMax, format);

    /// <remarks>Used when generating or ignoring level ranges.</remarks>
    /// <inheritdoc cref="GetSeed{TEnc,TEvo}(TEnc, uint, TEvo, byte)"/>
    public static LeadSeed GetSeed<TEnc>(TEnc enc, uint seed) where TEnc : IEncounterSlot4
        => GetSeed(enc, seed, enc, FormatNoLevelCheck);

    /// <remarks>Used when generating with specific level ranges.</remarks>
    /// <inheritdoc cref="GetSeed{TEnc,TEvo}(TEnc, uint, TEvo, byte)"/>
    public static LeadSeed GetSeed<TEnc, TEvo>(TEnc enc, uint seed, TEvo evo)
        where TEnc : IEncounterSlot4
        where TEvo : ILevelRange
        => GetSeed(enc, seed, evo, Format);

    /// <param name="depth">Recursion depth for checking re-rolls. 0 for no recursion, up to 4 for exhausted recursion (default 0).</param>
    /// <param name="forceSyncLead">Force a specific nature for synchronization (default random).</param>
    /// <remarks>
    /// The additional optional parameters are used internally to track the state of recursion solving for repeat IV attempts. Calling this method externally should omit them.
    /// </remarks>
    /// <inheritdoc cref="GetSeed{TEnc,TEvo}(TEnc, uint, TEvo, byte)"/>
    // ReSharper disable InvalidXmlDocComment
    private static LeadSeed GetSeed<TEnc>(TEnc enc, uint seed, byte levelMin, byte levelMax, byte format = Format, int depth = 0, Nature forceSyncLead = LeadSyncAllowed)
        // ReSharper restore InvalidXmlDocComment
        where TEnc : IEncounterSlot4
    {
        var ctx = GetSearchContext(enc, seed, levelMin, levelMax, format);
        return GetSeedCore(ctx, seed, depth, forceSyncLead);
    }

    private static LeadSeed GetSeedCore<TEnc>(in SearchContext<TEnc> ctx, uint seed, int depth = 0, Nature forceSyncLead = LeadSyncAllowed)
        where TEnc : IEncounterSlot4
    {
        var pid = ClassicEraRNG.GetSequentialPID(seed);
        var nature = (byte)(pid % 25);

        var frames = GetReversalWindow(seed, nature);
        return GetOriginSeed(ctx, seed, nature, frames, depth, forceSyncLead);
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
    private const byte FormatNoLevelCheck = 0; // anything but `Format` will ignore met level precision (overwritten via transfer, test logic, etc.)

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

    public static uint GetNature(uint rand) => rand % 25;

    private static SearchContext<T> GetSearchContext<T>(T enc, uint seed, byte levelMin, byte levelMax, byte format)
        where T : IEncounterSlot4
    {
        var isRerollMinimum31 = enc is EncounterSlot4 { IsRerollMinimum31: true };
        var mustFailAllPreviousRerolls = isRerollMinimum31 && !HasAny31IV(seed); // 1-((31/32)^6)^4) = 53.32% for a BCC/Safari to arise with a 31-IV
        return new(enc, levelMin, levelMax, format, isRerollMinimum31, mustFailAllPreviousRerolls);
    }

    /// <summary>
    /// Gets the first possible origin seed and lead for the input encounter &amp; constraints.
    /// </summary>
    public static LeadSeed GetOriginSeed<T>(T enc, uint seed, byte nature, int reverseCount, byte levelMin, byte levelMax, byte format = Format, int depth = 0, Nature forceSyncLead = LeadSyncAllowed)
        where T : IEncounterSlot4
    {
        var ctx = GetSearchContext(enc, seed, levelMin, levelMax, format);
        return GetOriginSeed(ctx, seed, nature, reverseCount, depth, forceSyncLead);
    }

    private static LeadSeed GetOriginSeed<T>(in SearchContext<T> ctx, uint seed, byte nature, int reverseCount, int depth = 0, Nature forceSyncLead = LeadSyncAllowed)
        where T : IEncounterSlot4
    {
        LeadSeed prefer = default;
        while (true)
        {
            if (TryGetMatch(ctx, seed, nature, out var result, depth, forceSyncLead))
            {
                if (result.IsNoRequirement)
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

    /// <summary>
    /// Advances the origin seed to the state that yields the level rand() result.
    /// </summary>
    /// <remarks> If using the rand() result, be sure to >> 16. </remarks>
    public static uint SkipToLevelRand<T>(T enc, uint seed, LeadRequired lead)
        where T : IEncounterSlot4
    {
        if (enc.Type.IsFishingRodType() || enc.Type is Rock_Smash)
            return LCRNG.Next3(seed); // Proc, ESV, level.
        if (enc.Type is BugContest && !lead.IsAbleToSweetScent())
            return LCRNG.Next4(seed); // ProcStep[2], ESV, level.
        return LCRNG.Next2(seed); // ESV, level.
    }

    /// <summary>
    /// Checks an input seed and lead by unrolling to the encounter trigger state and checking the encounter conditions along the way.
    /// </summary>
    /// <param name="enc">Encounter to check against.</param>
    /// <param name="seed">Seed that immediately selects the encounter slot.</param>
    /// <param name="lead">Party lead effect that is active at the moment of encounter slot selection.</param>
    /// <param name="result">Un-rolled seed and lead at the moment of encounter trigger, if the check passes.</param>
    /// <returns><see langword="true"/> if the seed and lead can trigger the encounter; otherwise, <see langword="false"/>.</returns>
    /// <remarks>
    /// It is necessary to check this when exploring possible leads, as different leads (or lack thereof) may consume a different quantity of RNG calls.
    /// </remarks>
    public static bool CheckEncounterActivation<T>(T enc, uint seed, LeadRequired lead, out LeadSeed result)
        where T : IEncounterSlot4
    {
        var pass = CheckEncounterActivation(enc, ref seed, ref lead);
        result = new(seed, lead);
        return pass;
    }

    private static bool CheckEncounterActivation<T>(T enc, ref uint seed, ref LeadRequired lead)
        where T : IEncounterSlot4
    {
        if (enc.Type.IsFishingRodType())
            return IsFishPossible(enc.Type, ref seed, ref lead);
        if (enc.Type is Rock_Smash)
            return IsRockSmashPossible(enc.AreaRate, ref seed, ref lead);

        // Ability & Sweet Scent deadlock for BCC:
        if (enc.Type is BugContest && !lead.IsAbleToSweetScent())
            return IsBugContestPossibleDeadlock(enc.AreaRate, ref seed);
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
            if (!CheckEncounterActivation(enc, s, CuteCharm, out result))
                continue;
            return true;
        }
        result = default; return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool TryGetMatch<T>(in SearchContext<T> ctx, uint seed, byte nature, out LeadSeed result, int depth = 0, Nature forceSyncLead = LeadSyncAllowed)
        where T : IEncounterSlot4
    {
        var p0 = seed >> 16; // 0
        var reg = GetNature(p0) == nature;
        if (reg)
        {
            var frame = ctx.GetFrameRef(seed);
            // Ideally, we don't need to check the recursion. Eagerly check the non-recursive paths.
            if (forceSyncLead is (LeadSyncAllowed or LeadSyncDisallowed))
            {
                if (TryGetMatchNoSync(frame, out result) && ctx.CanAcceptDirectMatch(depth))
                    return true;
            }
            else // Recursion demands a specific/unspecific synchronization nature.
            {
                if (TryGetMatchOnlyFailSync(frame, out result) && ctx.CanAcceptDirectMatch(depth))
                {
                    if (forceSyncLead is not LeadSyncRequiredFailUnspecific) // If we locked in a specific synchronization nature yet, prefer to indicate the end result.
                        result.Lead = Synchronize; // Override back to the end-result synchronize.
                    return true;
                }
            }

            if (ctx.ShouldRecurse(depth))
            {
                // If we haven't locked in to a synchronize lead based on recursion, try earlier frames.
                if (forceSyncLead is (LeadSyncAllowed or LeadSyncDisallowed))
                {
                    if (RecurseReject(ctx, seed, out result, depth + 1, LeadSyncDisallowed))
                        return true;
                }
                var prev = frame.Seed1;
                // If rerolls can reach here from a failed synchronize check (unknown nature), then check that path as well.
                if (forceSyncLead is not LeadSyncDisallowed && !IsSyncPass(prev >> 16) && ctx.LevelMin <= ctx.Encounter.LevelMax) // can't boost level if already using Synchronize
                {
                    var lead = forceSyncLead == LeadSyncAllowed ? LeadSyncRequiredFailUnspecific : forceSyncLead;
                    if (RecurseReject(ctx, prev, out result, depth + 1, lead))
                        return true;
                }
            }
        }

        // Check for a successful sync activation.
        // If recursion demands us to be a specific synchronization nature, ensure it matches. If not, can't be a sync activation.
        if (forceSyncLead is not (LeadSyncAllowed or LeadSyncRequiredFailUnspecific))
        {
            // LeadSyncDisallowed innately passes this check, no need to explicitly check that case.
            if (forceSyncLead != (Nature)nature)
            {
                result = default;
                return false;
            }
        }

        var syncProc = IsSyncPass(p0);
        if (syncProc && ctx.LevelMin <= ctx.Encounter.LevelMax) // can't boost level if already using Synchronize
        {
            var frame = ctx.GetFrameRef(seed);
            if (IsSlotValidRegular(frame, out result, Synchronize) && ctx.CanAcceptDirectMatch(depth))
                return true;

            if (ctx.ShouldRecurse(depth))
            {
                if (RecurseReject(ctx, seed, out result, depth + 1, (Nature)nature))
                    return true;
            }
        }
        result = default;
        return false;
    }

    /// <summary>
    /// Represents the magic number that that allows any lead to be used, enabling the generation target to require or not require a synchronize lead.
    /// </summary>
    private const Nature LeadSyncAllowed = Nature.Random;

    /// <summary>
    /// Represents a special value indicating that synchronize is disallowed for the lead.
    /// </summary>
    /// <remarks>When the generation pattern is traversing upwards under the assumption that it was a regular nature check, this value is used to prevent traversing up synchronize origins.</remarks>
    private const Nature LeadSyncDisallowed = unchecked(LeadSyncAllowed + 1);

    /// <summary>
    /// Represents a special value indicating that synchronize is required, but no specific nature has been required yet.
    /// </summary>
    /// <remarks>
    /// Any other value [0,24] is a specific nature being required as a synchronize lead.
    /// </remarks>
    private const Nature LeadSyncRequiredFailUnspecific = unchecked(LeadSyncAllowed + 2);

    /// <summary>
    /// Recursive method to check if the input seed could have been generated after 1-4 failed attempts at re-rolling for 31-IVs.
    /// </summary>
    private static bool RecurseReject<T>(in SearchContext<T> ctx, uint seed, out LeadSeed result, int depth, Nature forceSyncLead)
        where T : IEncounterSlot4
    {
        // The game will roll the {(sync,)nature..PID/IV} up to 4 times if none of the IVs are at 31 (total of 3 failed attempts at re-rolls).

        // Entering this method, the RNG state (seed) is the nature/sync call that generates the Pokémon we're solving for.
        // Check that the previous 2 frames are a valid rejection for IVs, then recurse to generate that "failed" as our PID/IV origin search.
        result = default;

        // Use the depth to keep track of how many we have already burned.
        // First entry to this method will be 1/4 burned (final result).

        // Ensure if the previous 4 consumed frames {PID,PID,IV,IV} yielded a 31-IV Pokémon. If so, it couldn't have been re-rolled from.
        var iv2 = LCRNG.Prev16(ref seed);
        if (IsAny31(iv2))
            return false;
        var iv1 = LCRNG.Prev16(ref seed);
        if (IsAny31(iv1))
            return false;
        // Cool, the skipped Pokémon was not 31-IV.
        // The skipped Pokémon probably had a different nature, so we need to be sure to use that result when recursing.
        seed = LCRNG.Prev3(seed); // Jump back before the PID frames; origin generates the (PID+badIVs)

        // We need to repeat the entire top-level check with slightly adjusted parameters.
        // The recursion will check that this fail-skipped can be generated (skipped PID/IV could have been landed on via its nature/sync check).
        // The innate recursion will return true if the skipped frame could have been landed on, or if an even-more previous fail-skipped was landed.
        // We pass in the `forceSyncLead` param in the event we just locked into a specific synchronization nature that must be used by all previous fail-skips.
        // The synchronization nature lock-in is required, to prevent the recursion fail-skips from matching a different lead/sync nature.
        result = GetSeedCore(ctx, seed, depth, forceSyncLead);
        if (result.IsValid)
            return true;

        // This rejection target can't be reached from any starting point.
        // The (unreachable) RNG calls that follow will not be able to generate our input target.
        return false;
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

    private static bool HasAny31IV(uint origin)
    {
        var seed = LCRNG.Next3(origin); // hop over pid,pid to get iv1
        var iv1 = seed >> 16;
        if (IsAny31(iv1))
            return true;
        var iv2 = LCRNG.Next16(ref seed);
        return IsAny31(iv2);
    }

    private static bool TryGetMatchCuteCharm<T>(in FrameCheckDetails<T> ctx, out uint result)
        where T : IEncounterSlot4
    {
        if (IsCuteCharmFail(ctx.Prev1))
        { result = 0; return false; }

        return IsSlotValidFrom1Skip(ctx, out result);
    }

    private static bool IsSlotValidCuteCharmFail<T>(in FrameCheckDetails<T> ctx, out uint result)
        where T : IEncounterSlot4
    {
        if (IsCuteCharmPass(ctx.Prev1)) // should have triggered
        { result = 0; return false; }

        return IsSlotValidFrom1Skip(ctx, out result);
    }

    private static bool IsSlotValidSyncFail<T>(in FrameCheckDetails<T> ctx, out uint result)
        where T : IEncounterSlot4
    {
        if (IsSyncPass(ctx.Prev1)) // should have triggered
        { result = 0; return false; }

        return IsSlotValidFrom1Skip(ctx, out result);
    }

    private static bool IsSlotValidIntimidate<T>(in FrameCheckDetails<T> ctx, out uint result)
        where T : IEncounterSlot4
    {
        if (IsIntimidateKeenEyePass(ctx.Prev1)) // encounter routine aborted
        { result = 0; return false; }

        return IsSlotValidFrom1Skip(ctx, out result);
    }

    private static bool IsSlotValidHustleVitalFail<T>(in FrameCheckDetails<T> ctx, out uint result)
        where T : IEncounterSlot4
    {
        if (IsHustleVitalPass(ctx.Prev1)) // should have triggered
        { result = 0; return false; }

        return IsSlotValidFrom1Skip(ctx, out result);
    }

    private static bool TryGetMatchOnlyFailSync<T>(in FrameCheckDetails<T> ctx, out LeadSeed result)
        where T : IEncounterSlot4
    {
        if (ctx.LevelMin > ctx.Encounter.LevelMax)
        {
            // Must be boosted via Pressure/Hustle/Vital Spirit
            result = default; return false;
        }

        if (IsSlotValidSyncFail(ctx, out var seed) && CheckEncounterActivation(ctx.Encounter, seed, SynchronizeFail, out result))
            return true;

        result = default; return false;
    }

    private static bool TryGetMatchNoSync<T>(in FrameCheckDetails<T> ctx, out LeadSeed result)
        where T : IEncounterSlot4
    {
        if (ctx.LevelMin > ctx.Encounter.LevelMax)
        {
            // Must be boosted via Pressure/Hustle/Vital Spirit
            if (IsSlotValidHustleVital(ctx, out var pressure) && CheckEncounterActivation(ctx.Encounter, pressure, PressureHustleSpirit, out result))
                return true;
            result = default; return false;
        }

        if (IsSlotValidRegular(ctx, out result))
            return true;

        if (IsSlotValidSyncFail(ctx, out var seed) && CheckEncounterActivation(ctx.Encounter, seed, SynchronizeFail, out result))
            return true;
        if (IsSlotValidCuteCharmFail(ctx, out seed) && CheckEncounterActivation(ctx.Encounter, seed, CuteCharmFail, out result))
            return true;
        if (IsSlotValidHustleVitalFail(ctx, out seed) && CheckEncounterActivation(ctx.Encounter, seed, PressureHustleSpiritFail, out result))
            return true;
        if (IsSlotValidStaticMagnetFail(ctx, out seed) && CheckEncounterActivation(ctx.Encounter, seed, StaticMagnetFail, out result))
            return true;
        // Intimidate/Keen Eye failing will result in no encounter.

        if (IsSlotValidStaticMagnet(ctx, out seed, out var sm) && CheckEncounterActivation(ctx.Encounter, seed, sm, out result))
            return true;
        if (IsSlotValidIntimidate(ctx, out seed) && CheckEncounterActivation(ctx.Encounter, seed, IntimidateKeenEyeFail, out result))
            return true;
        if (ctx.LevelMax >= ctx.Encounter.PressureLevel) // Can be boosted, or not.
        {
            if (IsSlotValidHustleVital(ctx, out var pressure) && CheckEncounterActivation(ctx.Encounter, pressure, PressureHustleSpirit, out result))
                return true;
        }

        result = default; return false;
    }

    public static bool IsLevelRand<T>(T enc) where T : IEncounterSlot4 => enc.Type.IsLevelRandHGSS;

    private static bool IsSlotValidFrom1Skip<T>(FrameCheckDetails<T> ctx, out uint result)
        where T : IEncounterSlot4
    {
        if (IsLevelRand(ctx.Encounter))
        {
            if (ctx.Encounter.IsFixedLevel || IsLevelValid(ctx.Encounter, ctx.LevelMin, ctx.LevelMax, ctx.Format, ctx.Prev2))
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
        result = 0; return false;
    }

    private static bool IsSlotValidRegular<T>(in FrameCheckDetails<T> ctx, out LeadSeed result, LeadRequired lead = None)
        where T : IEncounterSlot4
    {
        if (IsLevelRand(ctx.Encounter))
        {
            if (ctx.Encounter.IsFixedLevel || IsLevelValid(ctx.Encounter, ctx.LevelMin, ctx.LevelMax, ctx.Format, ctx.Prev1))
            {
                if (IsSlotValid(ctx.Encounter, ctx.Prev2) && CheckEncounterActivation(ctx.Encounter, ctx.Seed3, lead, out result))
                    return true;
            }
        }
        else // Not random level
        {
            if (IsSlotValid(ctx.Encounter, ctx.Prev1) && CheckEncounterActivation(ctx.Encounter, ctx.Seed2, lead, out result))
                return true;
        }
        result = default; return false;
    }

    private static bool IsSlotValidHustleVital<T>(in FrameCheckDetails<T> ctx, out uint result)
        where T : IEncounterSlot4
    {
        if (IsHustleVitalFail(ctx.Prev1)) // should have triggered
        { result = 0; return false; }

        var expectLevel = ctx.Encounter.PressureLevel;
        if (!IsOriginalLevelValid(ctx.LevelMin, ctx.LevelMax, ctx.Format, expectLevel))
        { result = 0; return false; }

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
        result = 0; return false;
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
            { result = 0; return false; }

            if (ctx.Encounter.IsFixedLevel || IsLevelValid(ctx.Encounter, ctx.LevelMin, ctx.LevelMax, ctx.Format, ctx.Prev1))
            {
                if (ctx.Encounter.IsSlotValidStaticMagnet(ctx.Prev2, out lead))
                { result = ctx.Seed4; return true; }
            }
        }
        else // Not random level
        {
            if (IsStaticMagnetFail(ctx.Prev2)) // should have triggered
            { result = 0; return false; }

            if (ctx.Encounter.IsSlotValidStaticMagnet(ctx.Prev1, out lead))
            { result = ctx.Seed3; return true; }
        }
        result = 0; return false;
    }

    private static bool IsSlotValidStaticMagnetFail<T>(in FrameCheckDetails<T> ctx, out uint result)
        where T : IEncounterSlot4
    {
        if (IsLevelRand(ctx.Encounter))
        {
            if (IsStaticMagnetPass(ctx.Prev3)) // should have triggered
            { result = 0; return false; }

            if (ctx.Encounter.IsFixedLevel || IsLevelValid(ctx.Encounter, ctx.LevelMin, ctx.LevelMax, ctx.Format, ctx.Prev1))
            {
                if (IsSlotValid(ctx.Encounter, ctx.Prev2))
                { result = ctx.Seed4; return true; }
            }
        }
        else // Not random level
        {
            if (IsStaticMagnetPass(ctx.Prev2)) // should have triggered
            { result = 0; return false; }

            if (IsSlotValid(ctx.Encounter, ctx.Prev1))
            { result = ctx.Seed2; return true; }
        }
        result = 0; return false;
    }

    private static bool IsSlotValid<T>(T enc, uint u16SlotRand)
        where T : IEncounterSlot4
    {
        var slot = SlotMethodK.GetSlot(enc.Type, u16SlotRand);
        return slot == enc.SlotNumber;
    }

    private static bool IsLevelValid<T>(T enc, byte min, byte max, byte format, uint u16LevelRand) where T : ILevelRange
    {
        var level = GetRandomLevel(enc, u16LevelRand);
        return IsOriginalLevelValid(min, max, format, level);
    }

    private static bool IsOriginalLevelValid(byte min, byte max, byte format, uint level)
    {
        if (format == Format && min > 1)
            return level == min; // Met Level matches
        return LevelRangeExtensions.IsLevelWithinRange((byte)level, min, max);
    }

    public static uint GetRandomLevel<T>(T enc, uint seed, LeadRequired lead) where T : IEncounterSlot4
    {
        if (lead is PressureHustleSpirit)
            return enc.PressureLevel;
        return GetRandomLevel(enc, seed);
    }

    private static uint GetRandomLevel<T>(T enc, uint u16LevelRand) where T : ILevelRange
    {
        var min = enc.LevelMin;
        uint mod = 1u + enc.LevelMax - min;
        return (u16LevelRand % mod) + min;
    }

    private static bool IsBugContestPossibleDeadlock(uint areaRate, ref uint result)
    {
        // BCC only allows one Pokémon to be in the party.
        // Specific lead abilities can learn Sweet Scent, while others cannot.
        // The only entry into this method requires an ability that has no species available with Sweet Scent.
        // Therefore, without Sweet Scent, we need to trigger via turning/walking.
        // With an area rate of 25, this arrangement will succeed 37% of the time.
        // However, White Flute cannot stay active as you cannot use it during a BCC, and entering the BCC has a screen transition (disabling it).

        // The game checks 2 random calls to trigger the encounter: movement -> rate -> generate.
        // HG/SS has an underflow error (via radio) which can pass the first rand call for movement.
        // Only need to check the second call for rate.
        // Rate can be improved by 50% if the White Flute is used.
        // Other abilities can also affect the rate, but we can't use them with our current lead.

        // areaRate += (areaRate >> 1); // +50% White Flute
        var rand = (result >> 16);
        var roll = rand % 100;
        if (roll >= areaRate)
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

    /// <summary>
    /// Wrapper to cache the encounter details for a given seed and level range, to avoid redundant calculations during recursion for Method K's minimum 31-IV re-roll logic.
    /// </summary>
    private readonly record struct SearchContext<T>(T Encounter, byte LevelMin, byte LevelMax, byte CurrentEntityFormat,
        bool IsRerollMinimum31,
        bool MustFailAllPreviousRerolls)
        where T : IEncounterSlot4
    {
        public FrameCheckDetails<T> GetFrameRef(uint seed) => new(Encounter, seed, LevelMin, LevelMax, CurrentEntityFormat);

        /// <summary>
        /// For HG/SS Safari Zone and Bug Catching Contest:
        /// When generating Pokémon's IVs, the game will reroll the numbers up to 4 times if none of the IVs are at 31.
        /// </summary>

        private const int MaxSafariContest = 4;

        public bool CanAcceptDirectMatch(int depth) => !MustFailAllPreviousRerolls || depth == (MaxSafariContest - 1);

        public bool ShouldRecurse(int depth)
        {
            if (!IsRerollMinimum31)
                return false; // No reroll requirement, no recursion.
            if (depth >= MaxSafariContest)
                return false; // Exhausted all possible rerolls.
            return true;
        }
    }
}
