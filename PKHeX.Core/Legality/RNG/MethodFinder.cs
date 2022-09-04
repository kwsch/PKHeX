using System;
using System.Runtime.CompilerServices;
using static PKHeX.Core.PIDType;

namespace PKHeX.Core;

/// <summary>
/// Class containing logic to obtain a PKM's PIDIV method.
/// </summary>
public static class MethodFinder
{
    /// <summary>
    /// Analyzes a <see cref="PKM"/> to find a matching PIDIV method.
    /// </summary>
    /// <param name="pk">Input <see cref="PKM"/>.</param>
    /// <returns><see cref="PIDIV"/> object containing seed and method info.</returns>
    public static PIDIV Analyze(PKM pk)
    {
        if (pk.Format < 3)
            return AnalyzeGB(pk);
        var pid = pk.EncryptionConstant;

        var top = pid & 0xFFFF0000;
        var bot = pid << 16;

        Span<uint> temp = stackalloc uint[6];
        for (int i = 0; i < 6; i++)
            temp[i] = (uint)pk.GetIV(i);
        ReadOnlySpan<uint> IVs = temp;

        // Between XDRNG and LCRNG, the LCRNG will have the most results.
        // Reuse our temp buffer across all methods.
        const int maxResults = LCRNG.MaxCountSeedsIV;
        Span<uint> seeds = stackalloc uint[maxResults];

        if (GetLCRNGMatch(seeds, top, bot, IVs, out PIDIV pidiv))
            return pidiv;
        if (pk.Species == (int)Species.Unown && GetLCRNGUnownMatch(seeds, top, bot, IVs, out pidiv)) // frlg only
            return pidiv;
        if (GetColoStarterMatch(seeds, pk, top, bot, IVs, out pidiv))
            return pidiv;
        if (GetXDRNGMatch(seeds, pk, top, bot, IVs, out pidiv))
            return pidiv;

        // Special cases
        if (GetLCRNGRoamerMatch(seeds, top, bot, IVs, out pidiv))
            return pidiv;
        if (GetChannelMatch(seeds, top, bot, IVs, out pidiv, pk))
            return pidiv;
        if (GetMG4Match(seeds, pid, IVs, out pidiv))
            return pidiv;

        if (GetBACDMatch(seeds, pk, pid, IVs, out pidiv))
            return pidiv;
        if (GetModifiedPIDMatch(seeds, pk, pid, IVs, out pidiv))
            return pidiv;

        return PIDIV.None; // no match
    }

    private static bool GetModifiedPIDMatch(Span<uint> seeds, PKM pk, uint pid, ReadOnlySpan<uint> IVs, out PIDIV pidiv)
    {
        if (pk.IsShiny)
        {
            if (GetChainShinyMatch(seeds, pk, pid, IVs, out pidiv))
                return true;
            if (GetModified8BitMatch(pk, pid, out pidiv))
                return true;
        }
        else
        {
            if (pid <= 0xFF && GetCuteCharmMatch(pk, pid, out pidiv))
                return true;
        }

        return GetPokewalkerMatch(pk, pid, out pidiv);
    }

    private static bool GetModified8BitMatch(PKM pk, uint pid, out PIDIV pidiv)
    {
        return pk.Gen4
            ? (pid <= 0xFF && GetCuteCharmMatch(pk, pid, out pidiv)) || GetG5MGShinyMatch(pk, pid, out pidiv)
            : GetG5MGShinyMatch(pk, pid, out pidiv) || (pid <= 0xFF && GetCuteCharmMatch(pk, pid, out pidiv));
    }

    private static bool GetLCRNGMatch(Span<uint> seeds, uint top, uint bot, ReadOnlySpan<uint> IVs, out PIDIV pidiv)
    {
        var count = LCRNGReversal.GetSeeds(seeds, bot, top);
        var reg = seeds[..count];
        var iv1 = GetIVChunk(IVs, 0);
        var iv2 = GetIVChunk(IVs, 3);
        foreach (var seed in reg)
        {
            // A and B are already used by PID
            var B = LCRNG.Next2(seed);

            // Method 1/2/4 can use 3 different RNG frames
            var C = LCRNG.Next(B);
            var ivC = C >> 16 & 0x7FFF;
            if (iv1 == ivC)
            {
                var D = LCRNG.Next(C);
                var ivD = D >> 16 & 0x7FFF;
                if (iv2 == ivD) // ABCD
                {
                    pidiv = new PIDIV(Method_1, seed);
                    return true;
                }

                var E = LCRNG.Next(D);
                var ivE = E >> 16 & 0x7FFF;
                if (iv2 == ivE) // ABCE
                {
                    pidiv = new PIDIV(Method_4, seed);
                    return true;
                }
            }
            else
            {
                var D = LCRNG.Next(C);
                var ivD = D >> 16 & 0x7FFF;
                if (iv1 != ivD)
                    continue;

                var E = LCRNG.Next(D);
                var ivE = E >> 16 & 0x7FFF;
                if (iv2 == ivE) // ABDE
                {
                    pidiv = new PIDIV(Method_2, seed);
                    return true;
                }
            }
        }
        count = LCRNGReversalSkip.GetSeeds(seeds, bot, top);
        reg = seeds[..count];
        foreach (var seed in reg)
        {
            // A and B are already used by PID
            var C = LCRNG.Next3(seed);

            // Method 3
            var D = LCRNG.Next(C);
            var ivD = D >> 16 & 0x7FFF;
            if (iv1 != ivD)
                continue;
            var E = LCRNG.Next(D);
            var ivE = E >> 16 & 0x7FFF;
            if (iv2 != ivE)
                continue;
            pidiv = new PIDIV(Method_3, seed);
            return true;
        }
        return GetNonMatch(out pidiv);
    }

    private static bool GetLCRNGUnownMatch(Span<uint> seeds, uint top, uint bot, ReadOnlySpan<uint> IVs, out PIDIV pidiv)
    {
        // this is an exact copy of LCRNG 1,2,4 matching, except the PID has its halves switched (BACD, BADE, BACE)
        var count = LCRNGReversal.GetSeeds(seeds, top, bot); // reversed!
        var reg = seeds[..count];
        var iv1 = GetIVChunk(IVs, 0);
        var iv2 = GetIVChunk(IVs, 3);
        foreach (var seed in reg)
        {
            // A and B are already used by PID
            var B = LCRNG.Next2(seed);

            // Method 1/2/4 can use 3 different RNG frames
            var C = LCRNG.Next(B);
            var ivC = C >> 16 & 0x7FFF;
            if (iv1 == ivC)
            {
                var D = LCRNG.Next(C);
                var ivD = D >> 16 & 0x7FFF;
                if (iv2 == ivD) // BACD
                {
                    pidiv = new PIDIV(Method_1_Unown, seed);
                    return true;
                }

                var E = LCRNG.Next(D);
                var ivE = E >> 16 & 0x7FFF;
                if (iv2 == ivE) // BACE
                {
                    pidiv = new PIDIV(Method_4_Unown, seed);
                    return true;
                }
            }
            else
            {
                var D = LCRNG.Next(C);
                var ivD = D >> 16 & 0x7FFF;
                if (iv1 != ivD)
                    continue;

                var E = LCRNG.Next(D);
                var ivE = E >> 16 & 0x7FFF;
                if (iv2 == ivE) // BADE
                {
                    pidiv = new PIDIV(Method_2_Unown, seed);
                    return true;
                }
            }
        }
        count = LCRNGReversalSkip.GetSeeds(seeds, top, bot); // reversed!
        reg = seeds[..count];
        foreach (var seed in reg)
        {
            // A and B are already used by PID
            var C = LCRNG.Next3(seed);

            // Method 3
            var D = LCRNG.Next(C);
            var ivD = D >> 16 & 0x7FFF;
            if (iv1 != ivD)
                continue;
            var E = LCRNG.Next(D);
            var ivE = E >> 16 & 0x7FFF;
            if (iv2 != ivE)
                continue;
            pidiv = new PIDIV(Method_3_Unown, seed);
            return true;
        }
        return GetNonMatch(out pidiv);
    }

    private static bool GetLCRNGRoamerMatch(Span<uint> seeds, uint top, uint bot, ReadOnlySpan<uint> IVs, out PIDIV pidiv)
    {
        if (IVs[2] != 0 || IVs[3] != 0 || IVs[4] != 0 || IVs[5] != 0 || IVs[1] > 7)
            return GetNonMatch(out pidiv);

        var iv1 = GetIVChunk(IVs, 0);
        var count = LCRNGReversal.GetSeeds(seeds, bot, top);
        var reg = seeds[..count];
        foreach (var seed in reg)
        {
            // Only the first 8 bits are kept
            var ivC = LCRNG.Next3(seed) >> 16 & 0x00FF;
            if (iv1 != ivC)
                continue;

            pidiv = new PIDIV(Method_1_Roamer, seed);
            return true;
        }
        return GetNonMatch(out pidiv);
    }

    private static bool GetXDRNGMatch(Span<uint> seeds, PKM pk, uint top, uint bot, ReadOnlySpan<uint> IVs, out PIDIV pidiv)
    {
        var count = XDRNG.GetSeeds(seeds, top, bot);
        var xdc = seeds[..count];
        foreach (var seed in xdc)
        {
            var B = XDRNG.Prev(seed);
            var A = XDRNG.Prev(B);

            var hi = A >> 16;
            var lo = B >> 16;
            if (IVsMatch(hi, lo, IVs))
            {
                pidiv = new PIDIV(CXD, XDRNG.Prev(A));
                return true;
            }

            // check for anti-shiny against player TSV
            var tsv = (uint)(pk.TID ^ pk.SID) >> 3;
            var psv = (top ^ bot) >> 3;
            if (psv == tsv) // already shiny, wouldn't be anti-shiny
                continue;

            var p2 = seed;
            var p1 = B;
            psv = ((p2 ^ p1) >> 19);
            if (psv != tsv) // prior PID must be shiny
                continue;

            do
            {
                B = XDRNG.Prev(A);
                A = XDRNG.Prev(B);
                hi = A >> 16;
                lo = B >> 16;
                if (IVsMatch(hi, lo, IVs))
                {
                    pidiv = new PIDIV(CXDAnti, XDRNG.Prev(A));
                    return true;
                }

                p2 = XDRNG.Prev(p1);
                p1 = XDRNG.Prev(p2);
                psv = (p2 ^ p1) >> 19;
            }
            while (psv == tsv);
        }
        return GetNonMatch(out pidiv);
    }

    private static bool GetChannelMatch(Span<uint> seeds, uint top, uint bot, ReadOnlySpan<uint> IVs, out PIDIV pidiv, PKM pk)
    {
        var ver = pk.Version;
        if (ver is not ((int)GameVersion.R or (int)GameVersion.S))
            return GetNonMatch(out pidiv);

        var undo = (top >> 16) ^ 0x8000;
        if ((undo > 7 ? 0 : 1) != ((bot >> 16) ^ pk.SID ^ 40122))
            top = (undo << 16);

        var count = XDRNG.GetSeeds(seeds, top, bot);
        var channel = seeds[..count];
        foreach (var seed in channel)
        {
            var C = XDRNG.Next3(seed); // held item
            // no checks, held item can be swapped

            var D = XDRNG.Next(C); // Version
            if ((D >> 31) + 1 != ver) // (0-Sapphire, 1-Ruby)
                continue;

            var E = XDRNG.Next(D); // OT Gender
            if (E >> 31 != pk.OT_Gender)
                continue;

            if (!XDRNG.GetSequentialIVsUInt32(E, IVs))
                continue;

            if (seed >> 16 != pk.SID)
                continue;

            pidiv = new PIDIV(Channel, XDRNG.Prev(seed));
            return true;
        }
        return GetNonMatch(out pidiv);
    }

    private static bool GetMG4Match(Span<uint> seeds, uint pid, ReadOnlySpan<uint> IVs, out PIDIV pidiv)
    {
        uint mg4Rev = ARNG.Prev(pid);

        var count = LCRNGReversal.GetSeeds(seeds, mg4Rev << 16, mg4Rev & 0xFFFF0000);
        var mg4 = seeds[..count];
        foreach (var seed in mg4)
        {
            var B = LCRNG.Next2(seed);
            var C = LCRNG.Next(B);
            var D = LCRNG.Next(C);
            if (!IVsMatch(C >> 16, D >> 16, IVs))
                continue;

            pidiv = new PIDIV(G4MGAntiShiny, seed);
            return true;
        }
        return GetNonMatch(out pidiv);
    }

    private static bool GetG5MGShinyMatch(PKM pk, uint pid, out PIDIV pidiv)
    {
        var low = pid & 0xFFFF;
        // generation 5 shiny PIDs
        if (low <= 0xFF)
        {
            var av = (pid >> 16) & 1;
            var genPID = PIDGenerator.GetMG5ShinyPID(low, av, pk.TID, pk.SID);
            if (genPID == pid)
            {
                pidiv = PIDIV.G5MGShiny;
                return true;
            }
        }
        return GetNonMatch(out pidiv);
    }

    private static bool GetCuteCharmMatch(PKM pk, uint pid, out PIDIV pidiv)
    {
        if (pid > 0xFF)
            return GetNonMatch(out pidiv);

        (var species, int genderValue) = GetCuteCharmGenderSpecies(pk, pid, pk.Species);
        static int getRatio(ushort species)
        {
            return species <= Legal.MaxSpeciesID_4
                ? PersonalTable.HGSS[species].Gender
                : PKX.Personal[species].Gender;
        }

        switch (genderValue)
        {
            case 2: break; // can't cute charm a genderless pk
            case 0: // male
                var gr = getRatio(species);
                if (gr >= PersonalInfo.RatioMagicFemale) // no modification for PID
                    break;
                var rate = 25*((gr / 25) + 1); // buffered
                var nature = pid % 25;
                if (nature + rate != pid)
                    break;

                pidiv = PIDIV.CuteCharm;
                return true;
            case 1: // female
                if (pid >= 25)
                    break; // nope, this isn't a valid nature
                if (getRatio(species) >= PersonalInfo.RatioMagicFemale) // no modification for PID
                    break;

                pidiv = PIDIV.CuteCharm;
                return true;
        }
        return GetNonMatch(out pidiv);
    }

    private static bool GetChainShinyMatch(Span<uint> seeds, PKM pk, uint pid, ReadOnlySpan<uint> IVs, out PIDIV pidiv)
    {
        // 13 shiny bits
        // PIDH & 7
        // PIDL & 7
        // IVs
        var bot = GetIVChunk(IVs, 0) << 16;
        var top = GetIVChunk(IVs, 3) << 16;

        var count = LCRNGReversal.GetSeedsIVs(seeds, bot, top);
        var reg = seeds[..count];
        foreach (var seed in reg)
        {
            // check the individual bits
            var s = seed;
            int i = 15;
            do
            {
                var bit = s >> 16 & 1;
                if (bit != (pid >> i & 1))
                    break;
                s = LCRNG.Prev(s);
            }
            while (--i != 2);
            if (i != 2) // bit failed
                continue;
            // Shiny Bits of PID validated
            var upper = s;
            if ((upper >> 16 & 7) != (pid >> 16 & 7))
                continue;
            var lower = LCRNG.Prev(upper);
            if ((lower >> 16 & 7) != (pid & 7))
                continue;

            var upid = (((pid & 0xFFFF) ^ pk.TID ^ pk.SID) & 0xFFF8) | ((upper >> 16) & 0x7);
            if (upid != pid >> 16)
                continue;

            s = LCRNG.Prev2(lower); // unroll one final time to get the origin seed
            pidiv = new PIDIV(ChainShiny, s);
            return true;
        }
        return GetNonMatch(out pidiv);
    }

    private static bool GetBACDMatch(Span<uint> seeds, PKM pk, uint pid, ReadOnlySpan<uint> IVs, out PIDIV pidiv)
    {
        var bot = GetIVChunk(IVs, 0) << 16;
        var top = GetIVChunk(IVs, 3) << 16;

        var count = LCRNGReversal.GetSeedsIVs(seeds, bot, top);
        var reg = seeds[..count];
        PIDType type = BACD_U;
        foreach (var seed in reg)
        {
            var B = seed;
            var A = LCRNG.Prev(B);
            var low = B >> 16;

            var PID = (A & 0xFFFF0000) | low;
            if (PID != pid)
            {
                uint idxor = (uint)(pk.TID ^ pk.SID);
                bool isShiny = (idxor ^ PID >> 16 ^ (PID & 0xFFFF)) < 8;
                if (!isShiny)
                {
                    if (!pk.IsShiny) // check for nyx antishiny
                    {
                        if (!IsBACD_U_AX(idxor, pid, low, A, ref type))
                            continue;
                    }
                    else // check for force shiny pk
                    {
                        if (!IsBACD_U_S(idxor, pid, low, ref A, ref type))
                            continue;
                    }
                }
                else if (!IsBACD_U_AX(idxor, pid, low, A, ref type))
                {
                    if ((PID + 8 & 0xFFFFFFF8) != pid)
                        continue;
                    type = BACD_U_A;
                }
            }
            var s = LCRNG.Prev(A);

            // Check for prior Restricted seed
            var sn = s;
            for (int i = 0; i < 3; i++, sn = LCRNG.Prev(sn))
            {
                if ((sn & 0xFFFF0000) != 0)
                    continue;
                // shift from unrestricted enum val to restricted enum val
                pidiv = new PIDIV(--type, sn);
                return true;
            }
            // no restricted seed found, thus unrestricted
            pidiv = new PIDIV(type, s);
            return true;
        }
        return GetNonMatch(out pidiv);
    }

    private static bool GetPokewalkerMatch(PKM pk, uint oldpid, out PIDIV pidiv)
    {
        // check surface compatibility
        var mid = oldpid & 0x00FFFF00;
        if (mid != 0 && mid != 0x00FFFF00) // not expected bits
            return GetNonMatch(out pidiv);
        var nature = oldpid % 25;
        if (nature == 24) // impossible nature
            return GetNonMatch(out pidiv);

        var gender = pk.Gender;
        uint pid = PIDGenerator.GetPokeWalkerPID(pk.TID, pk.SID, nature, gender, pk.PersonalInfo.Gender);

        if (pid != oldpid)
        {
            if (!(gender == 0 && IsAzurillEdgeCaseM(pk, nature, oldpid)))
                return GetNonMatch(out pidiv);
        }
        pidiv = PIDIV.Pokewalker;
        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsAzurillEdgeCaseM(PKM pk, uint nature, uint oldpid)
    {
        // check for Azurill evolution edge case... 75% F-M is now 50% F-M; was this a F->M bend?
        ushort species = pk.Species;
        if (species is not ((int)Species.Marill or (int)Species.Azumarill))
            return false;

        const int AzurillGenderRatio = 0xBF;
        var gender = EntityGender.GetFromPIDAndRatio(pk.PID, AzurillGenderRatio);
        if (gender != 1)
            return false;

        var pid = PIDGenerator.GetPokeWalkerPID(pk.TID, pk.SID, nature, 1, AzurillGenderRatio);
        return pid == oldpid;
    }

    private static bool GetColoStarterMatch(Span<uint> seeds, PKM pk, uint top, uint bot, ReadOnlySpan<uint> IVs, out PIDIV pidiv)
    {
        bool starter = pk.Version == (int)GameVersion.CXD && pk.Species switch
        {
            (int)Species.Espeon when pk.Met_Level >= 25 => true,
            (int)Species.Umbreon when pk.Met_Level >= 26 => true,
            _ => false,
        };
        if (!starter)
            return GetNonMatch(out pidiv);

        var iv1 = GetIVChunk(IVs, 0);
        var iv2 = GetIVChunk(IVs, 3);

        var count = XDRNG.GetSeeds(seeds, top, bot);
        var xdc = seeds[..count];
        foreach (var seed in xdc)
        {
            uint origin = seed;
            if (!LockFinder.IsColoStarterValid(pk.Species, ref origin, pk.TID, pk.SID, pk.PID, iv1, iv2))
                continue;

            pidiv = new PIDIV(CXD_ColoStarter, origin);
            return true;
        }
        return GetNonMatch(out pidiv);
    }

    /// <summary>
    /// Returns false and no <see cref="PIDIV"/>.
    /// </summary>
    /// <param name="pidiv">Null</param>
    /// <returns>False</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool GetNonMatch(out PIDIV pidiv)
    {
        pidiv = PIDIV.None;
        return false;
    }

    /// <summary>
    /// Checks if the PID is a <see cref="PIDType.BACD_U_S"></see> match.
    /// </summary>
    /// <param name="idxor"><see cref="PKM.TID"/> ^ <see cref="PKM.SID"/></param>
    /// <param name="pid">Full actual PID</param>
    /// <param name="low">Low portion of PID (B)</param>
    /// <param name="A">First RNG call</param>
    /// <param name="type">PID Type is updated if successful</param>
    /// <returns>True/False if the PID matches</returns>
    /// <remarks>First RNG call is unrolled once if the PID is valid with this correlation</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsBACD_U_S(uint idxor, uint pid, uint low, ref uint A, ref PIDType type)
    {
        // 0-Origin
        // 1-PIDH
        // 2-PIDL (ends up unused)
        // 3-FORCEBITS
        // PID = PIDH << 16 | (SID ^ TID ^ PIDH)

        var X = LCRNG.Prev(A); // unroll once as there's 3 calls instead of 2
        uint PID = (X & 0xFFFF0000) | (idxor ^ X >> 16);
        PID &= 0xFFFFFFF8;
        PID |= low & 0x7; // lowest 3 bits

        if (PID != pid)
            return false;
        A = X; // keep the unrolled seed
        type = BACD_U_S;
        return true;
    }

    /// <summary>
    /// Checks if the PID is a <see cref="PIDType.BACD_U_AX"></see> match.
    /// </summary>
    /// <param name="idxor"><see cref="PKM.TID"/> ^ <see cref="PKM.SID"/></param>
    /// <param name="pid">Full actual PID</param>
    /// <param name="low">Low portion of PID (B)</param>
    /// <param name="A">First RNG call</param>
    /// <param name="type">PID Type is updated if successful</param>
    /// <returns>True/False if the PID matches</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsBACD_U_AX(uint idxor, uint pid, uint low, uint A, ref PIDType type)
    {
        if ((pid & 0xFFFF) != low)
            return false;

        // 0-Origin
        // 1-ushort rnd, do until >8
        // 2-PIDL

        uint rnd = A >> 16;
        if (rnd < 8)
            return false;
        uint PID = ((rnd ^ idxor ^ low) << 16) | low;
        if (PID != pid)
            return false;
        type = BACD_U_AX;
        return true;
    }

    private static PIDIV AnalyzeGB(PKM _)
    {
        // not implemented; correlation between IVs and RNG hasn't been converted to code.
        return PIDIV.None;
    }

    /// <summary>
    /// Generates IVs from 2 RNG calls using 15 bits of each to generate 6 IVs (5bits each).
    /// </summary>
    /// <param name="r1">First rand frame</param>
    /// <param name="r2">Second rand frame</param>
    /// <param name="IVs">IVs that should be the result</param>
    /// <returns>IVs match random number IVs</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IVsMatch(uint r1, uint r2, ReadOnlySpan<uint> IVs)
    {
        if (IVs[0] != (r1 & 31))
            return false;
        if (IVs[1] != (r1 >> 5 & 31))
            return false;
        if (IVs[2] != (r1 >> 10 & 31))
            return false;
        if (IVs[3] != (r2 & 31))
            return false;
        if (IVs[4] != (r2 >> 5 & 31))
            return false;
        if (IVs[5] != (r2 >> 10 & 31))
            return false;
        return true;
    }

    /// <summary>
    /// Generates IVs from 2 RNG calls using 15 bits of each to generate 6 IVs (5bits each).
    /// </summary>
    /// <param name="result">Result storage</param>
    /// <param name="r1">First rand frame</param>
    /// <param name="r2">Second rand frame</param>
    /// <returns>Array of 6 IVs</returns>
    internal static void GetIVsInt32(Span<int> result, uint r1, uint r2)
    {
        result[5] = (int)r2 >> 10 & 31;
        result[4] = (int)r2 >> 5 & 31;
        result[3] = (int)r2 & 31;
        result[2] = (int)r1 >> 10 & 31;
        result[1] = (int)r1 >> 5 & 31;
        result[0] = (int)r1 & 31;
    }

    private static uint GetIVChunk(ReadOnlySpan<uint> IVs, int start)
    {
        uint val = 0;
        for (int i = 0; i < 3; i++)
            val |= IVs[i+start] << (5*i);
        return val;
    }
    
    public static bool IsPokeSpotActivation(int slot, uint seed, out uint s)
    {
        s = seed;
        var esv = (seed >> 16) % 100;
        if (!IsPokeSpotSlotValid(slot, esv))
        {
            // todo
        }
        // check for valid activation
        s = XDRNG.Prev(seed);
        if ((s >> 16) % 3 != 0)
        {
            if ((s >> 16) % 100 < 10) // can't fail a munchlax/bonsly encounter check
            {
                // todo
            }
            s = XDRNG.Prev(s);
            if ((s >> 16) % 3 != 0) // can't activate even if generous
            {
                // todo
            }
        }
        return true;
    }

    private static bool IsPokeSpotSlotValid(int slot, uint esv) => slot switch
    {
        0 => esv < 50 , // [0,50)
        1 => esv - 50 < 35, // [50,85)
        2 => esv >= 85, // [85,100)
        _ => false,
    };

    public static bool IsCompatible3(this PIDType val, IEncounterTemplate encounter, PKM pk) => encounter switch
    {
        WC3 g                  => IsCompatible3Mystery(val, pk, g),
        EncounterStatic3 s     => IsCompatible3Static(val, pk, s),
        EncounterStaticShadow  => val is (CXD or CXDAnti),
        EncounterSlot3PokeSpot => val is PokeSpot,
        EncounterSlot3 w       => w.Species != (int)Species.Unown
            ? val is (Method_1       or Method_2       or Method_3       or Method_4)
            : val is (Method_1_Unown or Method_2_Unown or Method_3_Unown or Method_4_Unown),
        _  => val is None,
    };

    private static bool IsCompatible3Static(PIDType val, PKM pk, EncounterStatic3 s) => pk.Version switch
    {
        (int)GameVersion.CXD                        => val is (CXD or CXD_ColoStarter or CXDAnti),
        (int)GameVersion.E                          => val is Method_1, // no roamer glitch
        (int)GameVersion.FR or (int) GameVersion.LG => s.Roaming ? val.IsRoamerPIDIV(pk) : val is Method_1, // roamer glitch
        _ => s.Roaming ? val.IsRoamerPIDIV(pk) : val is (Method_1 or Method_4), // RS, roamer glitch && RSBox s/w emulation => method 4 available
    };

    private static bool IsCompatible3Mystery(PIDType val, PKM pk, WC3 g) => val == g.Method || val switch
    {
        // forced shiny eggs, when hatched, can lose their detectable correlation.
        None    => (g.Method is (BACD_R_S or BACD_U_S)) && g.IsEgg && !pk.IsEgg,
        CXDAnti => g.Method is CXD && g.Shiny == Shiny.Never,
        _       => false,
    };

    private static bool IsRoamerPIDIV(this PIDType val, PKM pk)
    {
        // Roamer PIDIV is always Method 1.
        // M1 is checked before M1R. A M1R PIDIV can also be a M1 PIDIV, so check that collision.
        if (Method_1_Roamer == val)
            return true;
        if (Method_1 != val)
            return false;

        // only 8 bits are stored instead of 32 -- 5 bits HP, 3 bits for ATK.
        // return pk.IV32 <= 0xFF;
        return pk.IV_DEF == 0 && pk.IV_SPE == 0 && pk.IV_SPA == 0 && pk.IV_SPD == 0 && pk.IV_ATK <= 7;
    }

    public static bool IsCompatible4(this PIDType val, IEncounterTemplate encounter, PKM pk) => encounter switch
    {
        // Pokewalker can sometimes be confused with CuteCharm due to the PID creation routine. Double check if it is okay.
        EncounterStatic4Pokewalker when val is CuteCharm => GetCuteCharmMatch(pk, pk.EncryptionConstant, out _) && IsCuteCharm4Valid(encounter, pk),
        EncounterStatic4Pokewalker => val is Pokewalker,

        EncounterStatic4 {Species: (int)Species.Pichu} => val is Pokewalker,
        EncounterStatic4 {Shiny: Shiny.Always} => val is ChainShiny,
        EncounterStatic4 when val is CuteCharm => IsCuteCharm4Valid(encounter, pk),
        EncounterStatic4 => val is Method_1,

        EncounterSlot4 w => val switch
        {
            // Chain shiny with Poké Radar is only possible in DPPt, in grass. Safari Zone does not allow using the Poké Radar
            ChainShiny => pk.IsShiny && !pk.HGSS && (w.GroundTile & GroundTileAllowed.Grass) != 0 && !Locations.IsSafariZoneLocation4(w.Location),
            CuteCharm => IsCuteCharm4Valid(encounter, pk),
            _ => val is Method_1,
        },

        PGT => IsG4ManaphyPIDValid(val, pk), // Manaphy is the only PGT in the database
        PCD d when d.Gift.PK.PID != 1 => true, // Already matches PCD's fixed PID requirement
        _ => val is None,
    };

    private static bool IsG4ManaphyPIDValid(PIDType val, PKM pk)
    {
        if (pk.IsEgg)
        {
            if (pk.IsShiny)
                return false;
            if (val == Method_1)
                return true;
            return val == G4MGAntiShiny && IsAntiShinyARNG();
        }

        if (val == Method_1)
            return pk.WasTradedEgg || !pk.IsShiny; // can't be shiny on received game
        return val == G4MGAntiShiny && (pk.WasTradedEgg || IsAntiShinyARNG());

        bool IsAntiShinyARNG()
        {
            var shinyPID = ARNG.Prev(pk.PID);
            return (pk.TID ^ pk.SID ^ (shinyPID & 0xFFFF) ^ (shinyPID >> 16)) < 8; // shiny proc
        }
    }

    private static bool IsCuteCharm4Valid(ISpeciesForm encounter, PKM pk)
    {
        if (pk.Species is (int)Species.Marill or (int)Species.Azumarill)
        {
            return !IsCuteCharmAzurillMale(pk.PID) // recognized as not Azurill
                   || encounter.Species == (int)Species.Azurill; // encounter must be male Azurill
        }

        return true;
    }

    private static bool IsCuteCharmAzurillMale(uint pid) => pid is >= 0xC8 and <= 0xE0;

    /// <summary>
    /// There are some edge cases when the gender ratio changes across evolutions.
    /// </summary>
    private static (ushort Species, int Gender) GetCuteCharmGenderSpecies(PKM pk, uint pid, ushort currentSpecies) => currentSpecies switch
    {
        // Nincada evo chain travels from M/F -> Genderless Shedinja
        (int)Species.Shedinja  => ((int)Species.Nincada, EntityGender.GetFromPID((int)Species.Nincada, pid)),

        // These evolved species cannot be encountered with cute charm.
        // 100% fixed gender does not modify PID; override this with the encounter species for correct calculation.
        // We can assume the re-mapped species's [gender ratio] is what was encountered.
        (int)Species.Wormadam  => ((int)Species.Burmy,   1),
        (int)Species.Mothim    => ((int)Species.Burmy,   0),
        (int)Species.Vespiquen => ((int)Species.Combee,  1),
        (int)Species.Gallade   => ((int)Species.Kirlia,  0),
        (int)Species.Froslass  => ((int)Species.Snorunt, 1),
        // Azurill & Marill/Azumarill collision
        // Changed gender ratio (25% M -> 50% M) needs special treatment.
        // Double check the encounter species with IsCuteCharm4Valid afterwards.
        (int)Species.Marill or (int)Species.Azumarill when IsCuteCharmAzurillMale(pid) => ((int)Species.Azurill, 0),

        // Future evolutions
        (int)Species.Sylveon   => ((int)Species.Eevee, pk.Gender),
        (int)Species.MrRime    => ((int)Species.MimeJr, pk.Gender),
        (int)Species.Kleavor   => ((int)Species.Scyther, pk.Gender),

        _ => (currentSpecies, pk.Gender),
    };

    public static PIDIV GetPokeSpotSeedFirst(PKM pk, byte slot)
    {
        // Activate (rand % 3)
        // Munchlax / Bonsly (10%/30%)
        // Encounter Slot Value (ESV) = 50%/35%/15% rarity (0-49, 50-84, 85-99)

        Span<uint> seeds = stackalloc uint[XDRNG.MaxCountSeedsPID];
        int count = XDRNG.GetSeeds(seeds, pk.EncryptionConstant);
        var reg = seeds[..count];
        foreach (var seed in reg)
        {
            // check for valid encounter slot info
            if (IsPokeSpotActivation(slot, seed, out uint s))
                return new PIDIV(PokeSpot, s);
        }
        return default;
    }
}
