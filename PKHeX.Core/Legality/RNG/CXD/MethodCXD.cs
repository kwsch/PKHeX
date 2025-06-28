using System;
using System.Runtime.CompilerServices;

namespace PKHeX.Core;

/// <summary>
/// Generator methods for <see cref="GameVersion.CXD"/>.
/// </summary>
public static class MethodCXD
{
    /// <summary>
    /// Tries to set a valid PID/IV with the requested criteria for an <see cref="IShadow3"/> object.
    /// </summary>
    /// <remarks>Only called if <see cref="EncounterCriteria.IsSpecifiedIVsAll"/> is true.</remarks>
    public static bool SetFromIVs<TEnc, TEntity>(this TEnc enc, TEntity pk, in EncounterCriteria criteria, PersonalInfo3 pi, bool noShiny)
        where TEnc : IShadow3
        where TEntity : G3PKM, ISeparateIVs
    {
        var gr = pi.Gender;
        var id32 = pk.ID32;
        criteria.GetCombinedIVs(out var iv1, out var iv2);
        Span<uint> all = stackalloc uint[XDRNG.MaxCountSeedsIV];
        var count = XDRNG.GetSeedsIVs(all, iv1 << 16, iv2 << 16);
        var seeds = all[..count];
        foreach (var seed in seeds)
        {
            // * => IV, IV, ability, PID, PID
            var s = XDRNG.Next3(seed);
            uint pid = GetPID(s, id32, noShiny);
            if (criteria.IsSpecifiedNature() && !criteria.IsSatisfiedNature((Nature)(pid % 25)))
                continue;
            if (criteria.IsSpecifiedGender() && !criteria.IsSatisfiedGender(EntityGender.GetFromPIDAndRatio(pid, gr)))
                continue;
            if (!noShiny && criteria.Shiny.IsShiny() != ShinyUtil.GetIsShiny3(id32, pid))
                continue;

            var result = LockFinder.IsAllShadowLockValid(enc, seed, pk);
            if (!result)
                continue;

            pk.PID = pid;
            SetIVs(pk, iv1, iv2);
            pk.Ability = ((XDRNG.Next2(seed) >> 16) & 1) == 0 ? pi.Ability1 : pi.Ability2;
            return true;
        }

        return false;
    }

    /// <summary>
    /// Tries to set a valid PID/IV with the requested criteria for a non-shadow encounter.
    /// </summary>
    /// <remarks>Only called if <see cref="EncounterCriteria.IsSpecifiedIVsAll"/> is true.</remarks>
    public static bool SetFromIVs<TEntity>(TEntity pk, in EncounterCriteria criteria, PersonalInfo3 pi, bool noShiny)
        where TEntity : G3PKM, ISeparateIVs
    {
        var gr = pi.Gender;
        var id32 = pk.ID32;
        criteria.GetCombinedIVs(out var iv1, out var iv2);
        Span<uint> all = stackalloc uint[XDRNG.MaxCountSeedsIV];
        var count = XDRNG.GetSeedsIVs(all, iv1 << 16, iv2 << 16);
        var seeds = all[..count];
        foreach (var seed in seeds)
        {
            // * => IV, IV, ability, PID, PID
            var s = XDRNG.Next3(seed);
            uint pid = GetPID(s, id32, noShiny);
            if (criteria.IsSpecifiedNature() && !criteria.IsSatisfiedNature((Nature)(pid % 25)))
                continue;
            if (criteria.IsSpecifiedGender() && !criteria.IsSatisfiedGender(EntityGender.GetFromPIDAndRatio(pid, gr)))
                continue;
            if (!noShiny && criteria.Shiny.IsShiny() != ShinyUtil.GetIsShiny3(id32, pid))
                continue;

            pk.PID = pid;
            SetIVs(pk, iv1, iv2);
            pk.Ability = ((XDRNG.Next2(seed) >> 16) & 1) == 0 ? pi.Ability1 : pi.Ability2;
            return true;
        }
        return false;
    }

    // Call structure for Colosseum Starters:
    // u16 TID
    // u16 SID
    //
    // Umbreon:
    // {u16 fakeID, u16 fakeID}
    // u16 iv1
    // u16 iv2
    // u16 ability
    // {u16 PID, u16 PID} repeat until male and not shiny
    //
    // Espeon:
    // Repeat above

    /// <summary>
    /// Tries to set a valid PID/IV with the requested criteria for a Colosseum starter (Umbreon and Espeon), preferring to match Trainer IDs.
    /// </summary>
    public static bool SetStarterFromTrainerID(CK3 pk, in EncounterCriteria criteria, ushort tid, ushort sid)
    {
        var filterIVs = criteria.IsSpecifiedIVs(2);
        var id32 = (uint)sid << 16 | tid;
        var species = pk.Species;
        // * => TID, SID, fakepid*2, [IVs, ability, PID]
        Span<uint> seeds = stackalloc uint[XDRNG.MaxCountSeedsPID];
        var count = XDRNG.GetSeeds(seeds, (uint)tid << 16, (uint)sid << 16);
        foreach (var seed in seeds[..count])
        {
            // Seed is immediately before the TID/SID. Need to jump down to the target.
            var s = XDRNG.Next7(seed); // tid/sid (2), fake pid (2), ivs (2), ability (1)

            // Generate the first starter
            uint prePID = s; // keep track of the ability frame to get IVs later
            uint pid = GetColoStarterPID(ref s, id32);

            // If we're looking for Espeon, generate the next starter
            if (species == (int)Species.Espeon) // After Umbreon
            {
                s = XDRNG.Next5(s); // fake pid (2), ivs (2), ability (1)
                prePID = s;
                pid = GetColoStarterPID(ref s, id32);
            }

            if (criteria.IsSpecifiedNature() && !criteria.IsSatisfiedNature((Nature)(pid % 25)))
                continue;
            // fixed gender & never shiny, ignore

            // prePID is the ability frame, unroll 2x to get the IVs
            var iv2 = XDRNG.Prev15(ref prePID);
            var iv1 = XDRNG.Prev15(ref prePID);
            var iv32 = iv2 << 15 | iv1;
            if (criteria.IsSpecifiedHiddenPower() && !criteria.IsSatisfiedHiddenPower(iv32))
                continue;
            if (filterIVs && !criteria.IsSatisfiedIVs(iv32))
                continue;

            pk.PID = pid;
            SetIVs(pk, iv1, iv2);
            pk.Ability = (ushort)Ability.Synchronize; // Only one ability available, don't bother checking rand().
            return true;
        }

        return false;
    }

    /// <summary>
    /// Umbreon is generated first, so no need for complicated backwards exploration.
    /// </summary>
    public static bool SetStarterFirstFromIVs(CK3 pk, in EncounterCriteria criteria)
    {
        Span<uint> seeds = stackalloc uint[XDRNG.MaxCountSeedsIV];
        criteria.GetCombinedIVs(out var iv1, out var iv2);
        var count = XDRNG.GetSeedsIVs(seeds, iv1 << 16, iv2 << 16);
        foreach (var seed in seeds[..count])
        {
            var origin = XDRNG.Prev4(seed);
            if (!IsValidNameScreenEndSeed(origin, out _))
                continue;
            // * => IV, IV, ability, PID, PID
            var s = XDRNG.Next3(seed);

            // Oh, no! we need to know the Trainer ID to generate the starters correctly.
            var tid = XDRNG.Prev3(seed) >> 16;
            var sid = XDRNG.Prev2(seed) >> 16;
            var id32 = sid << 16 | tid;

            uint pid = GetColoStarterPID(ref s, id32);
            if (criteria.IsSpecifiedNature() && !criteria.IsSatisfiedNature((Nature)(pid % 25)))
                continue;
            // fixed gender & never shiny, ignore

            pk.ID32 = id32;
            pk.PID = pid;
            SetIVs(pk, iv1, iv2);
            pk.Ability = (ushort)Ability.Synchronize; // Only one ability available, don't bother checking rand().
            return true;
        }

        return false;
    }

    /// <summary>
    /// Espeon is generated second, which requires backwards exploration.
    /// </summary>
    public static bool SetStarterSecondFromIVs(CK3 pk, in EncounterCriteria criteria)
    {
        Span<uint> seeds = stackalloc uint[XDRNG.MaxCountSeedsIV];
        criteria.GetCombinedIVs(out var iv1, out var iv2);
        var count = XDRNG.GetSeedsIVs(seeds, iv1 << 16, iv2 << 16);
        foreach (var seed in seeds[..count])
        {
            // The prior generated starter must be Umbreon with a valid PID lock for us to reach here.
            var prePID = XDRNG.Prev(seed) >> 16;
            var second = seed >> 16;
            var first = prePID >> 16;

            var umbreon = GetPIDRegular(first, second);
            if (!IsMaleEevee(umbreon))
                continue;

            // Lock constraint satisfied (1:8 chance), generate the PID for Espeon.
            // * => IV, IV, ability, PID, PID
            var frameEspeonPID = XDRNG.Next3(seed);
            // Oh, no! we need to know the Trainer ID to generate the starters correctly.
            // Look backwards from Umbreon to find all possible trainer IDs for that PID, each step generate an Espeon.
            while (true)
            {
                // Find Trainer ID for this Umbreon frame.
                var s = LCRNG.Prev7(prePID); // hypothetical origin seed
                if (!IsValidNameScreenEndSeed(s, out _))
                    continue;
                var sid = LCRNG.Next16(ref s);
                var tid = LCRNG.Next16(ref s);
                var id32 = sid << 16 | tid;
                if (IsValidTrainerCombination(criteria, id32, umbreon, frameEspeonPID, out var espeonPID))
                {
                    pk.PID = espeonPID;
                    pk.ID32 = id32;
                    break;
                }

                // Try the next possible Trainer ID
                prePID = LCRNG.Prev2(prePID);
                umbreon = GetPID(prePID);
                if (IsMaleEevee(umbreon))
                    return false; // Interrupted.
                // Try again
            }

            pk.Ability = (ushort)Ability.Synchronize; // Only one ability available, don't bother checking rand().
            SetIVs(pk, iv1, iv2);
            return true;
        }

        return false;
    }

    private static bool IsValidTrainerCombination(in EncounterCriteria criteria, uint id32, uint umbreon, uint pidSeed, out uint espeonPID)
    {
        espeonPID = 0;
        if (ShinyUtil.GetIsShiny3(id32, umbreon))
            return false; // Umbreon cannot be shiny

        // Generate Espeon PID
        espeonPID = GetColoStarterPID(ref pidSeed, id32);

        // Check if request constraints satisfied
        if (criteria.IsSpecifiedNature() && !criteria.IsSatisfiedNature((Nature)(espeonPID % 25)))
            return false;
        return false;
    }

    // Call structure for XD Starter:
    // u16 TID
    // u16 SID
    //
    // Eevee:
    // {u16 fakeID, u16 fakeID}
    // u16 iv1
    // u16 iv2
    // u16 ability
    // {u16 PID, u16 PID}

    /// <summary>
    /// Tries to set a valid PID/IV for the requested criteria for the XD starter (Eevee), preferring to match Trainer IDs.
    /// </summary>
    public static bool SetStarterFromTrainerID(XK3 pk, in EncounterCriteria criteria, ushort tid, ushort sid)
    {
        // * => TID, SID, fakepid*2, [IVs, ability, PID]
        var filterIVs = criteria.IsSpecifiedIVs(2);
        Span<uint> all = stackalloc uint[XDRNG.MaxCountSeedsPID];
        var count = XDRNG.GetSeeds(all, (uint)tid << 16, (uint)sid << 16);
        var seeds = all[..count];
        foreach (ref var seed in seeds)
        {
            // Seed is immediately before the TID/SID. Need to jump down to the target.
            var s = XDRNG.Next7(seed); // tid/sid (2), fake pid (2), ivs (2), ability (1)

            uint pid = GetPID(s);
            if (criteria.IsSpecifiedNature() && !criteria.IsSatisfiedNature((Nature)(pid % 25)))
                continue;
            if (criteria.IsSpecifiedGender() && !criteria.IsSatisfiedGender(EntityGender.GetFromPIDAndRatio(pid, 0x1F)))
                continue;

            var ivSeed = XDRNG.Next4(seed);
            var iv1 = XDRNG.Next15(ref ivSeed);
            var iv2 = XDRNG.Next15(ref ivSeed);
            var iv32 = iv2 << 15 | iv1;
            if (criteria.IsSpecifiedHiddenPower() && !criteria.IsSatisfiedHiddenPower(iv32))
                continue;
            if (filterIVs && !criteria.IsSatisfiedIVs(iv32))
                continue;

            pk.PID = pid;
            SetIVs(pk, iv1, iv2);
            pk.Ability = (ushort)Ability.RunAway; // Only one ability available, don't bother checking rand().
            return true;
        }

        return false;
    }

    /// <summary>
    /// Tries to set a valid PID/IV with the requested criteria for the XD starter (Eevee), preferring to match the IVs requested.
    /// </summary>
    public static bool SetStarterFromIVs(XK3 pk, in EncounterCriteria criteria)
    {
        Span<uint> seeds = stackalloc uint[XDRNG.MaxCountSeedsIV];
        criteria.GetCombinedIVs(out var iv1, out var iv2);
        var count = XDRNG.GetSeedsIVs(seeds, iv1 << 16, iv2 << 16);
        foreach (var seed in seeds[..count])
        {
            // * => IV, IV, ability, PID, PID
            var s = XDRNG.Next3(seed);

            uint pid = GetPID(s);
            if (criteria.IsSpecifiedNature() && !criteria.IsSatisfiedNature((Nature)(pid % 25)))
                continue;

            var tid = XDRNG.Prev3(seed) >> 16;
            var sid = XDRNG.Prev2(seed) >> 16;
            var id32 = sid << 16 | tid;
            if (criteria.Shiny.IsShiny() != ShinyUtil.GetIsShiny3(id32, pid))
                continue;

            pk.ID32 = id32;
            pk.PID = pid;
            SetIVs(pk, iv1, iv2);
            pk.Ability = (ushort)Ability.Synchronize; // Only one ability available, don't bother checking rand().
            return true;
        }
        return false;
    }

    /// <summary>
    /// Sets a random PID/IV with the requested criteria for the XD starter (Eevee).
    /// </summary>
    public static void SetStarterRandom(XK3 pk, in EncounterCriteria criteria, uint seed)
    {
        bool filterIVs = criteria.IsSpecifiedIVs(2);
        bool checkNameScreen = pk.Language is (int)LanguageID.Japanese;
        while (true)
        {
            if (checkNameScreen)
            {
                if (!IsValidNameScreenEndSeed(XDRNG.Prev1000(seed), out _))
                {
                    seed = XDRNG.Next(seed);
                    continue;
                }
            }
            var start = seed;

            // Get PID
            seed = XDRNG.Next7(seed); // tid, sid, fakePID x2, IVs x2, ability* => pid1, pid2
            var pid = GetPIDRegular(ref seed);
            if (criteria.IsSpecifiedNature() && !criteria.IsSatisfiedNature((Nature)(pid % 25)))
                continue;
            if (criteria.IsSpecifiedGender() && !criteria.IsSatisfiedGender(EntityGender.GetFromPIDAndRatio(pid, GenderRatioMale87_5)))
                continue;

            var tid = XDRNG.Next16(ref start);
            var sid = XDRNG.Next16(ref start);
            var id32 = sid << 16 | tid;
            if (criteria.Shiny.IsShiny() != ShinyUtil.GetIsShiny3(id32, pid))
                continue;

            // Get IVs
            seed = XDRNG.Next2(start);
            var iv1 = XDRNG.Next15(ref seed);
            var iv2 = XDRNG.Next15(ref seed);
            var iv32 = iv2 << 15 | iv1;
            if (criteria.IsSpecifiedHiddenPower() && !criteria.IsSatisfiedHiddenPower(iv32))
                continue;
            if (filterIVs && !criteria.IsSatisfiedIVs(iv32))
                continue;

            pk.ID32 = id32;
            pk.PID = pid;
            SetIVs(pk, iv1, iv2);
            pk.Ability = (ushort)Ability.RunAway; // Only one ability available, don't bother checking rand().
            return;
        }
    }

    private const int MaxIterationsShadowSearch = 1_000_000;

    /// <summary>
    /// Sets a random PID/IV with the requested criteria for a shadow encounter.
    /// </summary>
    public static bool SetRandom<TEnc, TEntity>(this TEnc enc, TEntity pk, in EncounterCriteria criteria, PersonalInfo3 pi, bool noShiny, uint seed)
        where TEnc : IShadow3
        where TEntity : G3PKM, ISeparateIVs
    {
        var id32 = pk.ID32;
        var gender = pi.Gender;

        bool hasPrior = enc.PartyPrior.Length != 0;
        bool filterIVs = criteria.IsSpecifiedIVs(2);
        int ctr = 0;
        while (ctr++ < MaxIterationsShadowSearch)
        {
            // fakePID x2, IVs x2, ability, pid1*, pid2
            var pid = GetPIDReuse(ref seed, id32, noShiny);
            if (criteria.IsSpecifiedNature() && !criteria.IsSatisfiedNature((Nature)(pid % 25)))
                continue;
            if (criteria.IsSpecifiedGender() && !criteria.IsSatisfiedGender(EntityGender.GetFromPIDAndRatio(pid, gender)))
                continue;
            if (!noShiny && criteria.Shiny.IsShiny() != ShinyUtil.GetIsShiny3(id32, pid))
                continue;

            // Get IVs -- separately from the single-stepping seed to avoid deadlock (processing same frames each loop)
            var s = XDRNG.Prev5(seed); // origin*, iv1, iv2, ability, pid1, pid2 {seed}
            var origin = s;
            var iv1 = XDRNG.Next15(ref s);
            var iv2 = XDRNG.Next15(ref s);
            var iv32 = iv2 << 15 | iv1;
            if (criteria.IsSpecifiedHiddenPower() && !criteria.IsSatisfiedHiddenPower(iv32))
                continue;
            if (filterIVs && !criteria.IsSatisfiedIVs(iv32))
                continue;

            if (hasPrior && !LockFinder.IsAllShadowLockValid(enc, origin, pk))
                continue;

            var abit = XDRNG.Next16(ref s);
            pk.PID = pid;
            SetIVs(pk, iv1, iv2);
            pk.Ability = (int)(abit & 1) == 0 ? pi.Ability1 : pi.Ability2;
            return true;
        }
        return false;
    }

    /// <summary>
    /// Sets a random PID/IV with the requested criteria for a non-shadow encounter.
    /// </summary>
    public static void SetRandom<T>(T pk, in EncounterCriteria criteria, PersonalInfo3 pi, bool noShiny, uint seed)
        where T : G3PKM, ISeparateIVs
    {
        var id32 = pk.ID32;
        var gender = pi.Gender;

        bool filterIVs = criteria.IsSpecifiedIVs(2);
        while (true)
        {
            // fakePID x2, IVs x2, ability, pid1*, pid2
            var pid = GetPIDReuse(ref seed, id32, noShiny);
            if (criteria.IsSpecifiedNature() && !criteria.IsSatisfiedNature((Nature)(pid % 25)))
                continue;
            if (criteria.IsSpecifiedGender() && !criteria.IsSatisfiedGender(EntityGender.GetFromPIDAndRatio(pid, gender)))
                continue;
            if (!noShiny && criteria.Shiny.IsShiny() != ShinyUtil.GetIsShiny3(id32, pid))
                continue;

            // Get IVs -- separately from the single-stepping seed to avoid deadlock (processing same frames each loop)
            var s = XDRNG.Prev5(seed); // origin*, iv1, iv2, ability, pid1, pid2 {seed}
            var iv1 = XDRNG.Next15(ref s);
            var iv2 = XDRNG.Next15(ref s);
            var iv32 = iv2 << 15 | iv1;
            if (criteria.IsSpecifiedHiddenPower() && !criteria.IsSatisfiedHiddenPower(iv32))
                continue;
            if (filterIVs && !criteria.IsSatisfiedIVs(iv32))
                continue;

            var abit = XDRNG.Next16(ref s);
            pk.PID = pid;
            SetIVs(pk, iv1, iv2);
            pk.Ability = (int)(abit & 1) == 0 ? pi.Ability1 : pi.Ability2;
            return;
        }
    }

    private static uint GetPID(uint seed) => GetPIDRegular(ref seed);

    /// <summary>
    /// Get the PID based on the input seed frame already being the first half of the PID.
    /// </summary>
    /// <param name="seed">Random seed to (re)use</param>
    /// <param name="id32">Trainer ID</param>
    /// <param name="noShiny">Re-roll if shiny</param>
    /// <returns>Generated PID</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static uint GetPIDReuse(ref uint seed, uint id32, bool noShiny)
    {
        while (true)
        {
            uint pid = GetPIDReuseFirst(ref seed);
            if (!noShiny || !ShinyUtil.GetIsShiny3(id32, pid))
                return pid;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static uint GetPID(uint seed, uint id32, bool noShiny)
    {
        while (true)
        {
            uint pid = GetPIDRegular(ref seed);
            if (!noShiny || !ShinyUtil.GetIsShiny3(id32, pid))
                return pid;
        }
    }

    private static uint GetColoStarterPID(ref uint seed, uint id32)
    {
        while (true)
        {
            uint pid = GetPIDRegular(ref seed);
            if (IsValidStarterColo(id32, pid))
                return pid;
        }
    }

    private const byte GenderRatioMale87_5 = 0x1F; // 87.5%

    private static bool IsMaleEevee(uint pid) => (pid & 0xFF) >= GenderRatioMale87_5;

    private static bool IsValidStarterColo(uint id32, uint pid)
        => IsMaleEevee(pid) && !ShinyUtil.GetIsShiny3(id32, pid);

    private static bool IsMatchIVs(uint iv1, uint iv2, uint seed)
    {
        if (iv1 != XDRNG.Next15(ref seed))
            return false;
        if (iv2 != XDRNG.Next15(ref seed))
            return false;
        return true;
    }

    public static uint GetColoStarterPID(ref uint seed, ushort tid, ushort sid)
    {
        var id32 = (uint)sid << 16 | tid;
        return GetColoStarterPID(ref seed, id32);
    }

    private static uint GetPIDRegular(ref uint seed)
    {
        var a = XDRNG.Next16(ref seed);
        var b = XDRNG.Next16(ref seed);
        return GetPIDRegular(a, b);
    }

    /// <summary>
    /// Get the PID based on the input seed frame already being the first half of the PID.
    /// </summary>
    private static uint GetPIDReuseFirst(ref uint seed)
    {
        var a = seed >> 16;
        var b = XDRNG.Next16(ref seed);
        return GetPIDRegular(a, b);
    }

    private static uint GetPIDRegular(uint a, uint b) => a << 16 | b;

    private static void SetIVs<TEntity>(TEntity pk, uint iv1, uint iv2)
        where TEntity : ISeparateIVs
    {
        // CXD store IVs in separate bytes.
        pk.IV_HP  = (byte)(iv1 & 0x1F);
        pk.IV_ATK = (byte)((iv1 >> 5) & 0x1F);
        pk.IV_DEF = (byte)((iv1 >> 10) & 0x1F);
        pk.IV_SPE = (byte)(iv2 & 0x1F);
        pk.IV_SPA = (byte)((iv2 >> 5) & 0x1F);
        pk.IV_SPD = (byte)((iv2 >> 10) & 0x1F);
    }

    public static bool TryGetSeedStarterColo(uint iv1, uint iv2,
        uint expectPID,
        ushort tid, ushort sid,
        ushort species, out uint result)
    {
        var id32 = (uint)sid << 16 | tid;
        // * => TID, SID, fakepid*2, [IVs, ability, PID]
        Span<uint> seeds = stackalloc uint[XDRNG.MaxCountSeedsPID];
        var count = XDRNG.GetSeeds(seeds, (uint)tid << 16, (uint)sid << 16);
        foreach (var seed in seeds[..count])
        {
            if (species is (ushort)Species.Umbreon)
            {
                // Check IVs
                var ivSeed = XDRNG.Next4(seed);
                if (!IsMatchIVs(iv1, iv2, ivSeed))
                    continue;
            }
            var pidSeedU = XDRNG.Next7(seed);
            var pid = GetColoStarterPID(ref pidSeedU, id32);
            if (species is (ushort)Species.Umbreon)
            {
                if (pid != expectPID)
                    continue;
                result = seed;
                return true;
            }

            // Espeon
            var afterFake = XDRNG.Next2(pidSeedU);
            if (!IsMatchIVs(iv1, iv2, afterFake))
                continue;
            var pidSeedE = XDRNG.Next3(afterFake);
            pid = GetColoStarterPID(ref pidSeedE, id32);
            if (pid != expectPID)
                continue;
            result = seed;
            return true;
        }
        result = 0;
        return false;
    }

    public static bool TryGetSeedStarterXD(uint iv1, uint iv2,
        uint expectPID, ushort tid, ushort sid,
        out uint result)
    {
        // * => TID, SID, fakepid*2, [IVs, ability, PID]
        Span<uint> seeds = stackalloc uint[XDRNG.MaxCountSeedsPID];
        var count = XDRNG.GetSeeds(seeds, (uint)tid << 16, (uint)sid << 16);
        foreach (var seed in seeds[..count])
        {
            // Check IVs
            var ivSeed = XDRNG.Next4(seed);
            if (!IsMatchIVs(iv1, iv2, ivSeed))
                continue;
            var pidSeed = XDRNG.Next7(seed);
            var pid = GetPID(pidSeed);
            if (pid != expectPID)
                continue;
            result = seed;
            return true;
        }
        result = 0;
        return false;
    }

    /// <summary>
    /// Checks if the provided <see cref="seed"/> can be landed at after confirming the player name.
    /// </summary>
    /// <param name="seed">Seed that goes on to generate the player TID/SID (after advancing 1000 frames)</param>
    /// <param name="origin">Current seed that confirms the player name</param>
    /// <returns>True if <see cref="seed"/> can be reached via <see cref="origin"/></returns>
    /// <remarks>https://github.com/yatsuna827/PokemonCoRNGLibrary/blob/e5255b13134ab3a2119788c40b5e71ee48d849b0/PokemonCoRNGLibrary/Util/LCGExtensions.cs#L15</remarks>
    public static bool IsValidNameScreenEndSeed(uint seed, out uint origin)
    {
        // On the character name screen, the game has a 10% chance to spawn a ball, and if passes, in a random position (using 4 RNG calls).
        // Ensure the seed can be landed on, with rough criteria of 4 fadeout frames in a row.
        // rand() / 10 == 0 is a skip
        const ushort SEED_THRESHOLD = 0x1999; // Only need the upper 16 bits

        var p1 =           (seed >> 16) > SEED_THRESHOLD;
        var p2 = XDRNG.Prev16(ref seed) > SEED_THRESHOLD;
        var p3 = XDRNG.Prev16(ref seed) > SEED_THRESHOLD;
        var p4 = XDRNG.Prev16(ref seed) > SEED_THRESHOLD;

        if (p1 && p2 && p3 && p4)
        {
            origin = seed;
            return true;
        }

        // Couldn't land on the input seed naturally, try to find an earlier seed that catapults us to the input seed by spawning a ball.
        if (XDRNG.Prev16(ref seed) <= SEED_THRESHOLD && IsValidNameScreenEndSeed(XDRNG.Prev(seed), out origin)) return true;
        if (XDRNG.Prev16(ref seed) <= SEED_THRESHOLD && p1 && IsValidNameScreenEndSeed(XDRNG.Prev(seed), out origin)) return true;
        if (XDRNG.Prev16(ref seed) <= SEED_THRESHOLD && p1 && p2 && IsValidNameScreenEndSeed(XDRNG.Prev(seed), out origin)) return true;
        if (XDRNG.Prev16(ref seed) <= SEED_THRESHOLD && p1 && p2 && p3 && IsValidNameScreenEndSeed(XDRNG.Prev(seed), out origin)) return true;

        origin = 0;
        return false;
    }

    /// <summary>
    /// Get the first possible starting seed that generates the given trainer ID and secret ID.
    /// </summary>
    /// <param name="tid">Generation 3 Trainer ID</param>
    /// <param name="sid">Generation 3 Secret ID</param>
    /// <param name="origin">Possible starting seed</param>
    /// <returns>True if a seed was found, false if no seed was found</returns>
    public static bool TryGetSeedTrainerID(ushort tid, ushort sid, out uint origin)
    {
        Span<uint> seeds = stackalloc uint[XDRNG.MaxCountSeedsPID];
        var count = XDRNG.GetSeeds(seeds, (uint)tid << 16, (uint)sid << 16);
        foreach (var s in seeds[..count])
        {
            var prev1k = XDRNG.Prev1000(s);
            if (IsValidNameScreenEndSeed(prev1k, out origin))
                return true;
        }
        origin = 0;
        return false;
    }

    public static bool TryGetShinySID(ushort tid, out ushort sid, uint xor, uint bits)
    {
        for (int i = 0; i < 8; i++, bits++)
        {
            var newSID = (ushort)(xor ^ (bits & 7));
            if (!TryGetSeedTrainerID(tid, newSID, out _))
                continue;
            sid = newSID;
            return true;
        }
        sid = 0;
        return false;
    }

    public static bool TryGetSeedStarterColo(PKM pk, out uint result)
    {
        (uint iv1, uint iv2) = GetIVs(pk);
        var tid = pk.TID16;
        var sid = pk.SID16;
        var species = pk.Species;
        var expectPID = pk.EncryptionConstant;
        return TryGetSeedStarterColo(iv1, iv2, expectPID, tid, sid, species, out result);
    }

    public static bool TryGetSeedStarterXD(PKM pk, out uint result)
    {
        (uint iv1, uint iv2) = GetIVs(pk);
        var tid = pk.TID16;
        var sid = pk.SID16;
        var expectPID = pk.EncryptionConstant;
        return TryGetSeedStarterXD(iv1, iv2, expectPID, tid, sid, out result);
    }

    private static (uint iv1, uint iv2) GetIVs(PKM pk)
    {
        uint iv32 = pk.GetIVs();
        var iv1 = iv32 & 0x7FFF;
        var iv2 = iv32 >> 15;
        return (iv1, iv2);
    }
}
