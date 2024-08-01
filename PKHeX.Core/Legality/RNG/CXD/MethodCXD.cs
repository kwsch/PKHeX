using System;

namespace PKHeX.Core;

/// <summary>
/// Generator methods for <see cref="GameVersion.CXD"/>.
/// </summary>
public static class MethodCXD
{
    /// <summary>
    /// Tries to set a valid PID/IV for the requested criteria for an <see cref="IShadow3"/> object.
    /// </summary>
    /// <remarks>Only called if <see cref="EncounterCriteria.IsSpecifiedIVs"/> is true.</remarks>
    public static bool SetFromIVs<T>(this T enc, G3PKM pk, EncounterCriteria criteria, PersonalInfo3 pi, bool noShiny = false) where T : IShadow3
    {
        var gr = pi.Gender;
        (uint iv1, uint iv2) = GetCombinedIVs(criteria);
        Span<uint> all = stackalloc uint[XDRNG.MaxCountSeedsIV];
        var count = XDRNG.GetSeedsIVs(all, iv1 << 16, iv2 << 16);
        var seeds = all[..count];
        foreach (var seed in seeds)
        {
            // * => IV, IV, ability, PID, PID
            var s = XDRNG.Next3(seed);
            uint pid = GetPID(pk, s, noShiny);
            if (criteria.IsSpecifiedNature() && (Nature)(pid % 25) != criteria.Nature)
                continue;

            var gender = EntityGender.GetFromPIDAndRatio(pid, gr);
            if (!criteria.IsGenderSatisfied(gender))
                continue;

            var origin = XDRNG.Prev(seed);
            var pidiv = new PIDIV(PIDType.CXD, origin);
            var result = LockFinder.IsAllShadowLockValid(enc, pidiv, pk);
            if (!result)
                continue;

            pk.PID = pid;
            pk.Ability = ((XDRNG.Next2(seed) >> 16) & 1) == 0 ? pi.Ability1 : pi.Ability2;
            criteria.SetRandomIVs(pk);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Tries to set a valid PID/IV for the requested criteria for a non-shadow encounter.
    /// </summary>
    /// <remarks>Only called if <see cref="EncounterCriteria.IsSpecifiedIVs"/> is true.</remarks>
    public static bool SetFromIVsCXD(G3PKM pk, EncounterCriteria criteria, PersonalInfo3 pi, bool noShiny = true)
    {
        var gr = pi.Gender;
        (uint iv1, uint iv2) = GetCombinedIVs(criteria);
        Span<uint> all = stackalloc uint[XDRNG.MaxCountSeedsIV];
        var count = XDRNG.GetSeedsIVs(all, iv1 << 16, iv2 << 16);
        var seeds = all[..count];
        foreach (var seed in seeds)
        {
            // * => IV, IV, ability, PID, PID
            var s = XDRNG.Next3(seed);
            uint pid = GetPID(pk, s, noShiny);
            if (criteria.IsSpecifiedNature() && (Nature)(pid % 25) != criteria.Nature)
                continue;

            var gender = EntityGender.GetFromPIDAndRatio(pid, gr);
            if (!criteria.IsGenderSatisfied(gender))
                continue;

            pk.PID = pid;
            pk.Ability = ((XDRNG.Next2(seed) >> 16) & 1) == 0 ? pi.Ability1 : pi.Ability2;
            criteria.SetRandomIVs(pk);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Tries to set a valid PID/IV for the requested criteria for a Colosseum starter (Umbreon and Espeon), preferring to match Trainer IDs.
    /// </summary>
    public static bool SetFromTrainerIDStarter(CK3 pk, EncounterCriteria criteria, PersonalInfo3 pi, ushort tid, ushort sid)
    {
        // * => TID, SID, fakepid*2, [IVs, ability, PID]
        Span<uint> all = stackalloc uint[XDRNG.MaxCountSeedsPID];
        var count = XDRNG.GetSeeds(all, (uint)tid << 16, (uint)sid << 16);
        var seeds = all[..count];
        foreach (ref var seed in seeds)
        {
            var s = XDRNG.Next7(seed);
            uint pid = GetPIDStarterMale(ref s, pk.ID32);
            if (pk.Species == (int)Species.Espeon) // After Umbreon
            {
                s = XDRNG.Next2(s);
                pid = GetPIDStarterMale(ref s, pk.ID32);
            }
            if (criteria.IsSpecifiedNature() && (Nature)(pid % 25) != criteria.Nature)
                continue;

            var ivSeed = XDRNG.Next4(seed);
            var iv1 = XDRNG.Next15(ref ivSeed);
            var iv2 = XDRNG.Next15(ref ivSeed);

            SetIVs(pk, iv1, iv2);
            pk.PID = pid;
            pk.Ability = ((XDRNG.Next2(seed) >> 16) & 1) == 0 ? pi.Ability1 : pi.Ability2;
            return true;
        }

        return false;
    }

    /// <summary>
    /// Tries to set a valid PID/IV for the requested criteria for the XD starter (Eevee), preferring to match Trainer IDs.
    /// </summary>
    public static bool SetFromTrainerIDStarter(XK3 pk, EncounterCriteria criteria, PersonalInfo3 pi, ushort tid, ushort sid)
    {
        // * => TID, SID, fakepid*2, [IVs, ability, PID]
        Span<uint> all = stackalloc uint[XDRNG.MaxCountSeedsPID];
        var count = XDRNG.GetSeeds(all, (uint)tid << 16, (uint)sid << 16);
        var seeds = all[..count];
        foreach (ref var seed in seeds)
        {
            var s = XDRNG.Next7(seed);
            uint pid = GetPID(s);
            if (criteria.IsSpecifiedNature() && (Nature)(pid % 25) != criteria.Nature)
                continue;

            var ivSeed = XDRNG.Next4(seed);
            var iv1 = XDRNG.Next15(ref ivSeed);
            var iv2 = XDRNG.Next15(ref ivSeed);

            SetIVs(pk, iv1, iv2);
            pk.PID = pid;
            pk.Ability = ((XDRNG.Next2(seed) >> 16) & 1) == 0 ? pi.Ability1 : pi.Ability2;
            return true;
        }

        return false;
    }

    private static uint GetPID(uint seed)
    {
        var a = XDRNG.Next16(ref seed);
        var b = XDRNG.Next16(ref seed);
        return GetPIDRegular(a, b);
    }

    private static uint GetPID(G3PKM pk, uint seed, bool noShiny)
    {
        while (true)
        {
            var a = XDRNG.Next16(ref seed);
            var b = XDRNG.Next16(ref seed);
            var pid = GetPIDRegular(a, b);
            if (!noShiny || !ShinyUtil.GetIsShiny(pk.ID32, pid))
                return pid;
        }
    }

    private static uint GetPIDStarterMale(ref uint seed, uint id32)
    {
        const byte ratio = 0x1F; // 12.5% F (can't be female)
        while (true)
        {
            var a = XDRNG.Next16(ref seed);
            var b = XDRNG.Next16(ref seed);
            var pid = GetPIDRegular(a, b);
            if ((pid & 0xFF) >= ratio && !ShinyUtil.GetIsShiny(id32, pid))
                return pid;
        }
    }

    private static uint GetPIDRegular(uint a, uint b) => a << 16 | b;

    private static (uint iv1, uint iv2) GetCombinedIVs(EncounterCriteria criteria)
    {
        uint iv1 = (uint)criteria.IV_HP | (uint)criteria.IV_ATK << 5 | (uint)criteria.IV_DEF << 10;
        uint iv2 = (uint)criteria.IV_SPE | (uint)criteria.IV_SPA << 5 | (uint)criteria.IV_SPD << 10;
        return (iv1, iv2);
    }

    private static void SetIVs(G3PKM pk, uint iv1, uint iv2)
    {
        Span<int> ivs = stackalloc int[6];
        MethodFinder.GetIVsInt32(ivs, iv1, iv2);
        pk.SetIVs(ivs);
    }
}
