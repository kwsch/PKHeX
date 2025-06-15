using System;
using System.Runtime.CompilerServices;
using static PKHeX.Core.GameVersion;
using static PKHeX.Core.PIDType;

namespace PKHeX.Core;

/// <summary>
/// Class containing logic to obtain a PKM's PID/IV method.
/// </summary>
public static class MethodFinder
{
    /// <summary>
    /// Analyzes a <see cref="PKM"/> to find a matching PID/IV method.
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

        uint iv32 = pk.GetIVs();
        uint iv1 = iv32 & 0x7FFF;
        uint iv2 = iv32 >> 15;

        // Between XDRNG and LCRNG, the LCRNG will have the most results.
        // Reuse our temp buffer across all methods.
        Span<uint> seeds = stackalloc uint[LCRNG.MaxCountSeedsIV];

        if (GetLCRNGMatch(seeds, top, bot, iv1, iv2, out var result))
            return result;
        if (pk.Species == (int)Species.Unown && GetLCRNGUnownMatch(seeds, top, bot, iv1, iv2, out result)) // frlg only
            return result;
        if (GetColoStarterMatch(pk, iv1, iv2, out result))
            return result;
        if (GetXDRNGMatch(seeds, pk, top, bot, iv1, iv2, out result))
            return result;

        // Special cases
        if (GetLCRNGRoamerMatch(seeds, top, bot, iv32, out result))
            return result;
        if (GetChannelMatch(seeds, pk, top, bot, iv32, out result))
            return result;
        if (GetMG4Match(seeds, pid, iv1, iv2, out result))
            return result;

        if (GetBACDMatch(seeds, pk, pid, iv1, iv2, out result))
            return result;
        if (GetModifiedPIDMatch(seeds, pk, pid, iv1, iv2, out result))
            return result;

        return PIDIV.None; // no match
    }

    private static bool GetModifiedPIDMatch(Span<uint> seeds, PKM pk, uint pid, uint iv1, uint iv2, out PIDIV pidiv)
    {
        if (pk.IsShiny)
        {
            if (GetChainShinyMatch(seeds, pk, pid, iv1, iv2, out pidiv))
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

    public static bool GetLCRNGMethod1Match(PKM pk, out uint result)
    {
        var iv32 = pk.GetIVs();
        var pid = pk.EncryptionConstant;
        return GetLCRNGMethod1Match(pid, iv32, out result);
    }

    public static bool GetLCRNGMethod1Match(uint pid, uint iv32, out uint result)
    {
        var iv1 = iv32 & 0x7FFF;
        var iv2 = iv32 >> 15;
        return GetLCRNGMethod1Match(pid, iv1, iv2, out result);
    }

    public static bool GetLCRNGMethod1Match(uint pid, uint iv1, uint iv2, out uint result)
    {
        var bot = pid << 16;
        var top = pid & 0xFFFF0000;
        return GetLCRNGMethod1Match(top, bot, iv1, iv2, out result);
    }

    private static bool GetLCRNGMethod1Match(uint top, uint bot, uint iv1, uint iv2, out uint result)
    {
        const int maxResults = LCRNG.MaxCountSeedsIV;
        Span<uint> seeds = stackalloc uint[maxResults];
        var count = LCRNGReversal.GetSeeds(seeds, bot, top);
        var reg = seeds[..count];

        foreach (var seed in reg)
        {
            var s = LCRNG.Next2(seed);
            if (iv1 != LCRNG.Next15(ref s))
                continue;
            if (iv2 != LCRNG.Next15(ref s))
                continue;
            // ABCD
            result = seed;
            return true;
        }

        result = 0;
        return false;
    }

    private static bool GetLCRNGMatch(Span<uint> seeds, uint top, uint bot, uint iv1, uint iv2, out PIDIV pidiv)
    {
        var count = LCRNGReversal.GetSeeds(seeds, bot, top);
        var reg = seeds[..count];
        foreach (var seed in reg)
        {
            // A and B are already used by PID
            var s = LCRNG.Next2(seed);

            // Method 1/2/4 can use 3 different RNG frames
            if (iv1 == LCRNG.Next15(ref s))
            {
                if (iv2 == LCRNG.Next15(ref s)) // ABCD
                {
                    pidiv = new PIDIV(Method_1, seed);
                    return true;
                }
                if (iv2 == LCRNG.Next15(ref s)) // ABC_E
                {
                    pidiv = new PIDIV(Method_4, seed);
                    return true;
                }
            }
            else
            {
                if (iv1 != LCRNG.Next15(ref s))
                    continue;
                if (iv2 == LCRNG.Next15(ref s)) // AB_DE
                {
                    pidiv = new PIDIV(Method_2, seed);
                    return true;
                }
            }
        }

        // Method 3 (A_CDE)
        count = LCRNGReversalSkip.GetSeeds(seeds, bot, top);
        reg = seeds[..count];
        foreach (var seed in reg)
        {
            // 3 frames are already used by PID; 2 + 1 frame from vblank
            var s = LCRNG.Next3(seed);

            if (iv1 != LCRNG.Next15(ref s))
                continue;
            if (iv2 != LCRNG.Next15(ref s))
                continue;
            pidiv = new PIDIV(Method_3, seed);
            return true;
        }
        return GetNonMatch(out pidiv);
    }

    private static bool GetLCRNGUnownMatch(Span<uint> seeds, uint top, uint bot, uint iv1, uint iv2, out PIDIV pidiv)
    {
        // this is an exact copy of LCRNG 1,2,4 matching, except the PID has its halves switched (BACD, BADE, BACE)
        var count = LCRNGReversal.GetSeeds(seeds, top, bot); // reversed!
        var reg = seeds[..count];
        foreach (var seed in reg)
        {
            // A and B are already used by PID
            var s = LCRNG.Next2(seed);
            if (iv1 == LCRNG.Next15(ref s))
            {
                if (iv2 == LCRNG.Next15(ref s)) // BACD
                {
                    pidiv = new PIDIV(Method_1_Unown, seed);
                    return true;
                }
                if (iv2 == LCRNG.Next15(ref s)) // BAC_E
                {
                    pidiv = new PIDIV(Method_4_Unown, seed);
                    return true;
                }
            }
            else
            {
                if (iv1 != LCRNG.Next15(ref s))
                    continue;
                if (iv2 == LCRNG.Next15(ref s)) // BA_DE
                {
                    pidiv = new PIDIV(Method_2_Unown, seed);
                    return true;
                }
            }
        }

        // Method 3 (C_ADE)
        count = LCRNGReversalSkip.GetSeeds(seeds, top, bot); // reversed!
        reg = seeds[..count];
        foreach (var seed in reg)
        {
            // 3 frames are already used by PID; 2 + 1 frame from vblank
            var s = LCRNG.Next3(seed);

            if (iv1 != LCRNG.Next15(ref s))
                continue;
            if (iv2 != LCRNG.Next15(ref s))
                continue;
            pidiv = new PIDIV(Method_3_Unown, seed);
            return true;
        }
        return GetNonMatch(out pidiv);
    }

    private static bool GetLCRNGRoamerMatch(Span<uint> seeds, uint top, uint bot, uint iv32, out PIDIV pidiv)
    {
        if (iv32 > 0xFF)
            return GetNonMatch(out pidiv);

        var count = LCRNGReversal.GetSeeds(seeds, bot, top);
        var reg = seeds[..count];
        foreach (var seed in reg)
        {
            // Only the first 8 bits are kept
            var ivC = LCRNG.Next3(seed) >> 16 & 0x00FF;
            if (iv32 != ivC)
                continue;

            pidiv = new PIDIV(Method_1_Roamer, seed);
            return true;
        }
        return GetNonMatch(out pidiv);
    }

    private static bool GetXDRNGMatch(Span<uint> seeds, PKM pk, uint top, uint bot, uint iv1, uint iv2, out PIDIV pidiv)
    {
        var count = XDRNG.GetSeeds(seeds, top, bot);
        var xdc = seeds[..count];
        foreach (var seed in xdc)
        {
            var B = XDRNG.Prev(seed);
            var A = XDRNG.Prev(B);

            var hi = (A >> 16) & 0x7FFF;
            var lo = (B >> 16) & 0x7FFF;
            if (hi == iv1 && lo == iv2)
            {
                pidiv = new PIDIV(PIDType.CXD, XDRNG.Prev(A));
                return true;
            }

            // Check for anti-shiny against player TSV
            var tsv = (uint)(pk.TID16 ^ pk.SID16) >> 3;
            var psv = (top ^ bot) >> (16 + 3); // inputs are << 16, account for that
            if (psv == tsv) // Already shiny, wouldn't be made anti-shiny
                continue;

            var p2 = seed;
            var p1 = B;
            psv = ((p2 ^ p1) >> 19);
            if (psv != tsv) // The prior PID must be shiny!
                continue;

            do
            {
                B = XDRNG.Prev(A);
                A = XDRNG.Prev(B);
                hi = (A >> 16) & 0x7FFF;
                lo = (B >> 16) & 0x7FFF;
                if (hi == iv1 && lo == iv2)
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

    private static bool GetChannelMatch(Span<uint> seeds, PKM pk, uint top, uint bot, uint iv32, out PIDIV pidiv)
    {
        var version = pk.Version;
        if (version is not (R or S))
            return GetNonMatch(out pidiv);

        var undo = (top >> 16) ^ 0x8000;
        if ((undo > 7 ? 0 : 1) != ((bot >> 16) ^ pk.SID16 ^ 40122))
            top = (undo << 16);

        var count = XDRNG.GetSeeds(seeds, top, bot);
        var channel = seeds[..count];
        foreach (var seed in channel)
        {
            var C = XDRNG.Next3(seed); // held item
            // no checks, held item can be swapped

            var D = XDRNG.Next(C); // Version
            if ((D >> 31) + 1 != (byte)version) // (0-Sapphire, 1-Ruby)
                continue;

            var E = XDRNG.Next(D); // OT Gender
            if (E >> 31 != pk.OriginalTrainerGender)
                continue;

            var ivs = XDRNG.GetSequentialIV32(E);
            if (ivs != iv32)
                continue;

            if (seed >> 16 != pk.SID16)
                continue;

            pidiv = new PIDIV(Channel, XDRNG.Prev(seed));
            return true;
        }
        return GetNonMatch(out pidiv);
    }

    private static bool GetMG4Match(Span<uint> seeds, uint pid, uint iv1, uint iv2, out PIDIV pidiv)
    {
        var currentPSV = getPSV(pid);
        pid = ARNG.Prev(pid);
        var originalPSV = getPSV(pid);
        // ARNG shiny value must be different from the original shiny
        // if we have a multi-rerolled PID, each re-roll must be from the same shiny value
        if (originalPSV == currentPSV)
            return GetNonMatch(out pidiv);

        // ARNG can happen at most 3 times (checked all 2^32 seeds)
        for (int i = 0; i < 3; i++)
        {
            var count = LCRNGReversal.GetSeeds(seeds, pid << 16, pid & 0xFFFF0000);
            var mg4 = seeds[..count];
            foreach (var seed in mg4)
            {
                var s = LCRNG.Next2(seed);
                if (iv1 != LCRNG.Next15(ref s))
                    continue;
                if (iv2 != LCRNG.Next15(ref s))
                    continue;

                pidiv = new PIDIV(G4MGAntiShiny, seed);
                return true;
            }

            // Continue checking for multi-rerolls
            pid = ARNG.Prev(pid);
            var prevPSV = getPSV(pid);
            if (prevPSV != originalPSV)
                break;
        }
        return GetNonMatch(out pidiv);

        static uint getPSV(uint u32) => ((u32 >> 16) ^ (u32 & 0xFFFF)) >> 3;
    }

    private static bool GetG5MGShinyMatch(PKM pk, uint pid, out PIDIV pidiv)
    {
        var low = pid & 0xFFFF;
        // generation 5 shiny PIDs
        if (low <= 0xFF)
        {
            var abilBit = (pid >> 16) & 1;
            var expect = MonochromeRNG.GetShinyPID(low, abilBit, pk.TID16, pk.SID16);
            if (pid == expect)
            {
                pidiv = PIDIV.G5MGShiny;
                return true;
            }
        }
        return GetNonMatch(out pidiv);
    }

    private static bool GetCuteCharmMatch(PKM pk, uint pid, out PIDIV pidiv)
    {
        if (!CuteCharm4.IsCuteCharm(pk, pid))
            return GetNonMatch(out pidiv);
        pidiv = PIDIV.CuteCharm;
        return true;
    }

    private static bool GetChainShinyMatch(Span<uint> seeds, ITrainerID32 pk, uint pid, uint iv1, uint iv2, out PIDIV pidiv)
    {
        // 13 shiny bits
        // PIDH & 7
        // PIDL & 7
        // IVs
        var count = LCRNGReversal.GetSeedsIVs(seeds, iv1 << 16, iv2 << 16);
        var reg = seeds[..count];
        foreach (var seed in reg)
        {
            if (!IsChainShinyValid(pk, pid, seed, out uint s))
                continue;
            pidiv = new PIDIV(ChainShiny, s);
            return true;
        }
        return GetNonMatch(out pidiv);
    }

    public static bool IsChainShinyValid<T>(T pk, uint pid, uint seed, out uint s) where T : ITrainerID32
    {
        // check the individual bits
        s = seed;
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
            return false;
        // Shiny Bits of PID validated
        var upper = s;
        if ((upper >> 16 & 7) != (pid >> 16 & 7))
            return false;
        var lower = LCRNG.Prev(upper);
        if ((lower >> 16 & 7) != (pid & 7))
            return false;

        var upid = (((pid & 0xFFFF) ^ pk.TID16 ^ pk.SID16) & 0xFFF8) | ((upper >> 16) & 0x7);
        if (upid != pid >> 16)
            return false;

        s = LCRNG.Prev2(lower); // unroll one final time to get the origin seed
        return true;
    }

    private static bool GetBACDMatch<T>(Span<uint> seeds, T pk, uint actualPID, uint iv1, uint iv2, out PIDIV pidiv)
        where T : ITrainerID32
    {
        var bot = iv1 << 16;
        var top = iv2 << 16;

        var count = LCRNGReversal.GetSeedsIVs(seeds, bot, top);
        var reg = seeds[..count];
        PIDType type = BACD;
        uint idXor = (uint)(pk.TID16 ^ pk.SID16);
        foreach (var x in reg)
        {
            // Check for the expected BA(CD) pattern -- the expected PID.
            var seed = x;
            var b16 = seed >> 16;
            var a16 = LCRNG.Prev16(ref seed);

            var expectPID = CommonEvent3.GetRegular(a16, b16);
            if (expectPID != actualPID)
            {
                // Check for the alternate event types that force shiny state.
                bool isShiny = ShinyUtil.GetIsShiny(idXor, actualPID, 8);
                if (!isShiny) // most likely, branch prediction!
                {
                    if (CommonEvent3.IsRegularAntishiny(actualPID, expectPID))
                        type = BACD_A;
                    else if (CommonEvent3.IsForceAntishiny(actualPID, a16, b16, idXor))
                        type = BACD_AX;
                    else if (CommonEvent3.IsForceShinyDifferentOT(actualPID, LCRNG.Prev16(ref seed), b16, idXor))
                        type = BACD_ES; // was shiny, hatched by different OT.
                    else
                        continue;
                }
                else // Shiny
                {
                    if (CommonEvent3.IsRegularAntishinyDifferentOT(actualPID, expectPID))
                        type = BACD_EA; // was not-shiny, hatched by different OT.
                    else if (CommonEvent3.IsForceAntishinyDifferentOT(actualPID, a16, b16, idXor))
                        type = BACD_EAX; // was not-shiny, hatched by different OT.
                    else if (CommonEvent3.IsForceShiny(actualPID, LCRNG.Prev16(ref seed), b16, idXor))
                        type = BACD_S;
                    else
                        continue;
                }
            }

            // Unroll one final time to get the origin seed.
            var origin = LCRNG.Prev(seed);
            pidiv = new PIDIV(type, origin);
            return true;
        }
        return GetNonMatch(out pidiv);
    }

    private static bool GetPokewalkerMatch(PKM pk, uint actualPID, out PIDIV pidiv)
    {
        // check surface compatibility
        // Bits 8-24 must all be zero or all be one.
        const uint midMask = 0x00FFFF00;
        var mid = actualPID & midMask;
        if (mid is not (0 or midMask))
            return GetNonMatch(out pidiv);

        var nature = actualPID % 25;
        // Quirky Nature is not possible with the algorithm.
        if (nature == 24)
            return GetNonMatch(out pidiv);

        // No Pokewalker Pokémon evolves into a different gender-ratio species.
        // Besides Azurill, and Froslass
        var gender = pk.Gender;
        var gr = pk.PersonalInfo.Gender;
        if (pk.Species == (int)Species.Froslass)
            gr = 0x7F; // Snorunt
        var expectPID = PokewalkerRNG.GetPID(pk.TID16, pk.SID16, nature, gender, gr);
        if (expectPID != actualPID)
        {
            if (!(gender == 0 && IsAzurillEdgeCaseM(pk, nature, actualPID)))
                return GetNonMatch(out pidiv);
        }
        pidiv = PIDIV.Pokewalker;
        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsAzurillEdgeCaseM(PKM pk, uint nature, uint actualPID)
    {
        // check for Azurill evolution edge case... 75% F-M is now 50% F-M; was this a Female->Male bend?
        ushort species = pk.Species;
        if (species is not ((int)Species.Marill or (int)Species.Azumarill))
            return false;

        const byte AzurillGenderRatio = 0xBF;
        var gender = EntityGender.GetFromPIDAndRatio(actualPID, AzurillGenderRatio);
        if (gender != 1)
            return false;

        var pid = PokewalkerRNG.GetPID(pk.TID16, pk.SID16, nature, 1, AzurillGenderRatio);
        return pid == actualPID;
    }

    private static bool GetColoStarterMatch(PKM pk, uint iv1, uint iv2, out PIDIV pidiv)
    {
        var species = pk.Species;
        bool starter = pk.Version == GameVersion.CXD && species switch
        {
            (int)Species.Espeon when pk.MetLevel >= 25 => true,
            (int)Species.Umbreon when pk.MetLevel >= 26 => true,
            _ => false,
        };
        if (!starter)
            return GetNonMatch(out pidiv);

        if (!MethodCXD.TryGetSeedStarterColo(iv1, iv2, pk.EncryptionConstant, pk.TID16, pk.SID16, species, out var result))
            return GetNonMatch(out pidiv);

        pidiv = new PIDIV(CXD_ColoStarter, result);
        return true;
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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static PIDIV AnalyzeGB(PKM _)
    {
        // not implemented; correlation between IVs and RNG hasn't been converted to code.
        return PIDIV.None;
    }
}
