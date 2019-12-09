using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;

namespace PKHeX.Core
{
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

            var top = pid >> 16;
            var bot = pid & 0xFFFF;

            var IVs = new uint[6];
            for (int i = 0; i < 6; i++)
                IVs[i] = (uint)pk.GetIV(i);

            if (GetLCRNGMatch(top, bot, IVs, out PIDIV pidiv))
                return pidiv;
            if (pk.Species == (int)Species.Unown && GetLCRNGUnownMatch(top, bot, IVs, out pidiv)) // frlg only
                return pidiv;
            if (GetColoStarterMatch(pk, top, bot, IVs, out pidiv))
                return pidiv;
            if (GetXDRNGMatch(top, bot, IVs, out pidiv))
                return pidiv;

            // Special cases
            if (GetLCRNGRoamerMatch(top, bot, IVs, out pidiv))
                return pidiv;
            if (GetChannelMatch(top, bot, IVs, out pidiv, pk))
                return pidiv;
            if (GetMG4Match(pid, IVs, out pidiv))
                return pidiv;

            if (GetBACDMatch(pk, pid, IVs, out pidiv))
                return pidiv;
            if (GetModifiedPIDMatch(pk, pid, IVs, out pidiv))
                return pidiv;

            return new PIDIV {Type=PIDType.None, NoSeed=true}; // no match
        }

        private static bool GetModifiedPIDMatch(PKM pk, uint pid, uint[] IVs, out PIDIV pidiv)
        {
            if (pk.IsShiny)
            {
                if (GetChainShinyMatch(pk, pid, IVs, out pidiv))
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

        private static bool GetLCRNGMatch(uint top, uint bot, uint[] IVs, out PIDIV pidiv)
        {
            var reg = GetSeedsFromPID(RNG.LCRNG, top, bot);
            var iv1 = GetIVChunk(IVs, 0);
            var iv2 = GetIVChunk(IVs, 3);
            foreach (var seed in reg)
            {
                // A and B are already used by PID
                var B = RNG.LCRNG.Advance(seed, 2);

                // Method 1/2/4 can use 3 different RNG frames
                var C = RNG.LCRNG.Next(B);
                var ivC = C >> 16 & 0x7FFF;
                if (iv1 == ivC)
                {
                    var D = RNG.LCRNG.Next(C);
                    var ivD = D >> 16 & 0x7FFF;
                    if (iv2 == ivD) // ABCD
                    {
                        pidiv = new PIDIV {OriginSeed = seed, RNG = RNGType.LCRNG, Type = PIDType.Method_1};
                        return true;
                    }

                    var E = RNG.LCRNG.Next(D);
                    var ivE = E >> 16 & 0x7FFF;
                    if (iv2 == ivE) // ABCE
                    {
                        pidiv = new PIDIV {OriginSeed = seed, RNG = RNGType.LCRNG, Type = PIDType.Method_4};
                        return true;
                    }
                }
                else
                {
                    var D = RNG.LCRNG.Next(C);
                    var ivD = D >> 16 & 0x7FFF;
                    if (iv1 != ivD)
                        continue;

                    var E = RNG.LCRNG.Next(D);
                    var ivE = E >> 16 & 0x7FFF;
                    if (iv2 == ivE) // ABDE
                    {
                        pidiv = new PIDIV {OriginSeed = seed, RNG = RNGType.LCRNG, Type = PIDType.Method_2};
                        return true;
                    }
                }
            }
            reg = GetSeedsFromPIDSkip(RNG.LCRNG, top, bot);
            foreach (var seed in reg)
            {
                // A and B are already used by PID
                var C = RNG.LCRNG.Advance(seed, 3);

                // Method 3
                var D = RNG.LCRNG.Next(C);
                var ivD = D >> 16 & 0x7FFF;
                if (iv1 != ivD)
                    continue;
                var E = RNG.LCRNG.Next(D);
                var ivE = E >> 16 & 0x7FFF;
                if (iv2 != ivE)
                    continue;
                pidiv = new PIDIV {OriginSeed = seed, RNG = RNGType.LCRNG, Type = PIDType.Method_3};
                return true;
            }
            return GetNonMatch(out pidiv);
        }

        private static bool GetLCRNGUnownMatch(uint top, uint bot, uint[] IVs, out PIDIV pidiv)
        {
            // this is an exact copy of LCRNG 1,2,4 matching, except the PID has its halves switched (BACD, BADE, BACE)
            var reg = GetSeedsFromPID(RNG.LCRNG, bot, top); // reversed!
            var iv1 = GetIVChunk(IVs, 0);
            var iv2 = GetIVChunk(IVs, 3);
            foreach (var seed in reg)
            {
                // A and B are already used by PID
                var B = RNG.LCRNG.Advance(seed, 2);

                // Method 1/2/4 can use 3 different RNG frames
                var C = RNG.LCRNG.Next(B);
                var ivC = C >> 16 & 0x7FFF;
                if (iv1 == ivC)
                {
                    var D = RNG.LCRNG.Next(C);
                    var ivD = D >> 16 & 0x7FFF;
                    if (iv2 == ivD) // BACD
                    {
                        pidiv = new PIDIV {OriginSeed = seed, RNG = RNGType.LCRNG, Type = PIDType.Method_1_Unown};
                        return true;
                    }

                    var E = RNG.LCRNG.Next(D);
                    var ivE = E >> 16 & 0x7FFF;
                    if (iv2 == ivE) // BACE
                    {
                        pidiv = new PIDIV {OriginSeed = seed, RNG = RNGType.LCRNG, Type = PIDType.Method_4_Unown};
                        return true;
                    }
                }
                else
                {
                    var D = RNG.LCRNG.Next(C);
                    var ivD = D >> 16 & 0x7FFF;
                    if (iv1 != ivD)
                        continue;

                    var E = RNG.LCRNG.Next(D);
                    var ivE = E >> 16 & 0x7FFF;
                    if (iv2 == ivE) // BADE
                    {
                        pidiv = new PIDIV {OriginSeed = seed, RNG = RNGType.LCRNG, Type = PIDType.Method_2_Unown};
                        return true;
                    }
                }
            }
            reg = GetSeedsFromPIDSkip(RNG.LCRNG, bot, top); // reversed!
            foreach (var seed in reg)
            {
                // A and B are already used by PID
                var C = RNG.LCRNG.Advance(seed, 3);

                // Method 3
                var D = RNG.LCRNG.Next(C);
                var ivD = D >> 16 & 0x7FFF;
                if (iv1 != ivD)
                    continue;
                var E = RNG.LCRNG.Next(D);
                var ivE = E >> 16 & 0x7FFF;
                if (iv2 != ivE)
                    continue;
                pidiv = new PIDIV {OriginSeed = seed, RNG = RNGType.LCRNG, Type = PIDType.Method_3_Unown};
                return true;
            }
            return GetNonMatch(out pidiv);
        }

        private static bool GetLCRNGRoamerMatch(uint top, uint bot, uint[] IVs, out PIDIV pidiv)
        {
            if (IVs[2] != 0 || IVs[3] != 0 || IVs[4] != 0 || IVs[5] != 0 || IVs[1] > 7)
                return GetNonMatch(out pidiv);
            var iv1 = GetIVChunk(IVs, 0);
            var reg = GetSeedsFromPID(RNG.LCRNG, top, bot);
            foreach (var seed in reg)
            {
                // Only the first 8 bits are kept
                var ivC = RNG.LCRNG.Advance(seed, 3) >> 16 & 0x00FF;
                if (iv1 != ivC)
                    continue;

                pidiv = new PIDIV {OriginSeed = seed, RNG = RNGType.LCRNG, Type = PIDType.Method_1_Roamer};
                return true;
            }
            return GetNonMatch(out pidiv);
        }

        private static bool GetXDRNGMatch(uint top, uint bot, uint[] IVs, out PIDIV pidiv)
        {
            var xdc = GetSeedsFromPIDEuclid(RNG.XDRNG, top, bot);
            foreach (var seed in xdc)
            {
                var B = RNG.XDRNG.Prev(seed);
                var A = RNG.XDRNG.Prev(B);

                var hi = A >> 16;
                var lo = B >> 16;
                if (!IVsMatch(hi, lo, IVs))
                {
                    // check for antishiny
                    // allow 2 different TSVs to proc antishiny for XD
                    var tsv1 = (int)((hi ^ lo) >> 3);
                    var tsv2 = -1;
                    while (true)
                    {
                        B = RNG.XDRNG.Prev(A);
                        A = RNG.XDRNG.Prev(B);
                        hi = A >> 16;
                        lo = B >> 16;
                        if (!IVsMatch(hi, lo, IVs))
                        {
                            var anti = (int)(hi ^ lo) >> 3;
                            if (anti == tsv1)
                                continue;
                            if (anti == tsv2)
                                continue;
                            if (tsv2 >= 0) // already set
                                break; // can't have this many shiny TSVs
                            tsv2 = anti;
                            continue;
                        }
                        pidiv = new PIDIVTSV
                        {
                            OriginSeed = RNG.XDRNG.Prev(A), RNG = RNGType.XDRNG, Type = PIDType.CXDAnti,
                            TSV1 = tsv1, TSV2 = tsv2,
                        };
                        return true;
                    }
                    continue;
                }

                pidiv = new PIDIV {OriginSeed = RNG.XDRNG.Prev(A), RNG = RNGType.XDRNG, Type = PIDType.CXD};
                return true;
            }
            return GetNonMatch(out pidiv);
        }

        private static bool GetChannelMatch(uint top, uint bot, uint[] IVs, out PIDIV pidiv, PKM pk)
        {
            var ver = pk.Version;
            if (ver != (int) GameVersion.R && ver != (int) GameVersion.S)
                return GetNonMatch(out pidiv);

            var undo = top ^ 0x8000;
            if ((undo > 7 ? 0 : 1) != (bot ^ pk.SID ^ 40122))
                top = undo;
            var channel = GetSeedsFromPIDEuclid(RNG.XDRNG, top, bot);
            foreach (var seed in channel)
            {
                var C = RNG.XDRNG.Advance(seed, 3); // held item
                // no checks, held item can be swapped

                var D = RNG.XDRNG.Next(C); // Version
                if ((D >> 31) + 1 != ver) // (0-Sapphire, 1-Ruby)
                    continue;

                var E = RNG.XDRNG.Next(D); // OT Gender
                if (E >> 31 != pk.OT_Gender)
                    continue;

                if (!RNG.XDRNG.GetSequentialIVsUInt32(E).SequenceEqual(IVs))
                    continue;

                if (seed >> 16 != pk.SID)
                    continue;

                pidiv = new PIDIV {OriginSeed = RNG.XDRNG.Prev(seed), RNG = RNGType.XDRNG, Type = PIDType.Channel};
                return true;
            }
            return GetNonMatch(out pidiv);
        }

        private static bool GetMG4Match(uint pid, uint[] IVs, out PIDIV pidiv)
        {
            uint mg4Rev = RNG.ARNG.Prev(pid);
            var mg4 = GetSeedsFromPID(RNG.LCRNG, mg4Rev >> 16, mg4Rev & 0xFFFF);
            foreach (var seed in mg4)
            {
                var B = RNG.LCRNG.Advance(seed, 2);
                var C = RNG.LCRNG.Next(B);
                var D = RNG.LCRNG.Next(C);
                if (!IVsMatch(C >> 16, D >> 16, IVs))
                    continue;

                pidiv = new PIDIV {OriginSeed = seed, RNG = RNGType.LCRNG, Type = PIDType.G4MGAntiShiny};
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
                    pidiv = new PIDIV {NoSeed = true, Type = PIDType.G5MGShiny};
                    return true;
                }
            }
            return GetNonMatch(out pidiv);
        }

        private static bool GetCuteCharmMatch(PKM pk, uint pid, out PIDIV pidiv)
        {
            if (pid > 0xFF)
                return GetNonMatch(out pidiv);

            GetCuteCharmGenderSpecies(pk, pid, out int genderValue, out int species);
            int getRatio() => PersonalTable.HGSS[species].Gender;
            switch (genderValue)
            {
                case 2: break; // can't cute charm a genderless pkm
                case 0: // male
                    var gr = getRatio();
                    if (254 <= gr) // no modification for PID
                        break;
                    var rate = 25*((gr / 25) + 1); // buffered
                    var nature = pid % 25;
                    if (nature + rate != pid)
                        break;

                    pidiv = new PIDIV {NoSeed = true, RNG = RNGType.LCRNG, Type = PIDType.CuteCharm};
                    return true;
                case 1: // female
                    if (pid >= 25)
                        break; // nope, this isn't a valid nature
                    if (254 <= getRatio()) // no modification for PID
                        break;

                    pidiv = new PIDIV {NoSeed = true, RNG = RNGType.LCRNG, Type = PIDType.CuteCharm};
                    return true;
            }
            return GetNonMatch(out pidiv);
        }

        private static bool GetChainShinyMatch(PKM pk, uint pid, uint[] IVs, out PIDIV pidiv)
        {
            // 13 shiny bits
            // PIDH & 7
            // PIDL & 7
            // IVs
            var bot = GetIVChunk(IVs, 0);
            var top = GetIVChunk(IVs, 3);
            var reg = GetSeedsFromIVs(RNG.LCRNG, top, bot);
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
                    s = RNG.LCRNG.Prev(s);
                }
                while (--i != 2);
                if (i != 2) // bit failed
                    continue;
                // Shiny Bits of PID validated
                var upper = s;
                if ((upper >> 16 & 7) != (pid >> 16 & 7))
                    continue;
                var lower = RNG.LCRNG.Prev(upper);
                if ((lower >> 16 & 7) != (pid & 7))
                    continue;

                var upid = (((pid & 0xFFFF) ^ pk.TID ^ pk.SID) & 0xFFF8) | ((upper >> 16) & 0x7);
                if (upid != pid >> 16)
                    continue;

                s = RNG.LCRNG.Reverse(lower, 2); // unroll one final time to get the origin seed
                pidiv = new PIDIV {OriginSeed = s, RNG = RNGType.LCRNG, Type = PIDType.ChainShiny};
                return true;
            }
            return GetNonMatch(out pidiv);
        }

        private static bool GetBACDMatch(PKM pk, uint pid, uint[] IVs, out PIDIV pidiv)
        {
            var bot = GetIVChunk(IVs, 0);
            var top = GetIVChunk(IVs, 3);
            var reg = GetSeedsFromIVs(RNG.LCRNG, top, bot);
            PIDType type = PIDType.BACD_U;
            foreach (var seed in reg)
            {
                var B = seed;
                var A = RNG.LCRNG.Prev(B);
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
                        else // check for force shiny pkm
                        {
                            if (!IsBACD_U_S(idxor, pid, low, ref A, ref type))
                                continue;
                        }
                    }
                    else if (!IsBACD_U_AX(idxor, pid, low, A, ref type))
                    {
                        if ((PID + 8 & 0xFFFFFFF8) != pid)
                            continue;
                        type = PIDType.BACD_U_A;
                    }
                }
                var s = RNG.LCRNG.Prev(A);

                // Check for prior Restricted seed
                var sn = s;
                for (int i = 0; i < 3; i++, sn = RNG.LCRNG.Prev(sn))
                {
                    if ((sn & 0xFFFF0000) != 0)
                        continue;
                    // shift from unrestricted enum val to restricted enum val
                    pidiv = new PIDIV {OriginSeed = sn, RNG = RNGType.LCRNG, Type = --type };
                    return true;
                }
                // no restricted seed found, thus unrestricted
                pidiv = new PIDIV {OriginSeed = s, RNG = RNGType.LCRNG, Type = type};
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
            pidiv = new PIDIV {NoSeed = true, RNG = RNGType.LCRNG, Type = PIDType.Pokewalker};
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsAzurillEdgeCaseM(PKM pk, uint nature, uint oldpid)
        {
            // check for Azurill evolution edge case... 75% F-M is now 50% F-M; was this a F->M bend?
            int spec = pk.Species;
            if (spec != (int)Species.Marill && spec != (int)Species.Azumarill)
                return false;

            const int AzurillGenderRatio = 0xBF;
            var gender = PKX.GetGenderFromPIDAndRatio(pk.PID, AzurillGenderRatio);
            if (gender != 1)
                return false;

            var pid = PIDGenerator.GetPokeWalkerPID(pk.TID, pk.SID, nature, 1, AzurillGenderRatio);
            return pid == oldpid;
        }

        private static bool GetColoStarterMatch(PKM pk, uint top, uint bot, uint[] IVs, out PIDIV pidiv)
        {
            if (pk.Version != 15 || (pk.Species != 196 && pk.Species != 197))
                return GetNonMatch(out pidiv);

            var iv1 = GetIVChunk(IVs, 0);
            var iv2 = GetIVChunk(IVs, 3);
            var xdc = GetSeedsFromPIDEuclid(RNG.XDRNG, top, bot);
            foreach (var seed in xdc)
            {
                uint origin = seed;
                if (!LockFinder.IsColoStarterValid(pk.Species, ref origin, pk.TID, pk.SID, pk.PID, iv1, iv2))
                    continue;

                pidiv = new PIDIV { OriginSeed = origin, RNG = RNGType.XDRNG, Type = PIDType.CXD_ColoStarter };
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

            var X = RNG.LCRNG.Prev(A); // unroll once as there's 3 calls instead of 2
            uint PID = (X & 0xFFFF0000) | (idxor ^ X >> 16);
            PID &= 0xFFFFFFF8;
            PID |= low & 0x7; // lowest 3 bits

            if (PID != pid)
                return false;
            A = X; // keep the unrolled seed
            type = PIDType.BACD_U_S;
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
            type = PIDType.BACD_U_AX;
            return true;
        }

        private static PIDIV AnalyzeGB(PKM _)
        {
            // not implemented; correlation between IVs and RNG hasn't been converted to code.
            return PIDIV.None;
        }

        private static IEnumerable<uint> GetSeedsFromPID(RNG method, uint a, uint b)
        {
            Debug.Assert(a >> 16 == 0);
            Debug.Assert(b >> 16 == 0);
            uint second = a << 16;
            uint first = b << 16;
            return method.RecoverLower16Bits(first, second);
        }

        private static IEnumerable<uint> GetSeedsFromPIDSkip(RNG method, uint a, uint b)
        {
            Debug.Assert(a >> 16 == 0);
            Debug.Assert(b >> 16 == 0);
            uint third = a << 16;
            uint first = b << 16;
            return method.RecoverLower16BitsGap(first, third);
        }

        private static IEnumerable<uint> GetSeedsFromIVs(RNG method, uint a, uint b)
        {
            Debug.Assert(a >> 15 == 0);
            Debug.Assert(b >> 15 == 0);
            uint second = a << 16;
            uint first = b << 16;
            var pairs = method.RecoverLower16Bits(first, second)
                .Concat(method.RecoverLower16Bits(first, second ^ 0x80000000));
            foreach (var z in pairs)
            {
                yield return z;
                yield return z ^ 0x80000000; // sister bitflip
            }
        }

        public static IEnumerable<uint> GetSeedsFromIVsSkip(RNG method, uint rand1, uint rand3)
        {
            Debug.Assert(rand1 >> 15 == 0);
            Debug.Assert(rand3 >> 15 == 0);
            rand1 <<= 16;
            rand3 <<= 16;
            var seeds = method.RecoverLower16BitsGap(rand1, rand3)
                .Concat(method.RecoverLower16BitsGap(rand1, rand3 ^ 0x80000000));
            foreach (var z in seeds)
            {
                yield return z;
                yield return z ^ 0x80000000; // sister bitflip
            }
        }

        public static IEnumerable<uint> GetSeedsFromPIDEuclid(RNG method, uint rand1, uint rand2)
        {
            return method.RecoverLower16BitsEuclid16(rand1 << 16, rand2 << 16);
        }

        public static IEnumerable<uint> GetSeedsFromIVsEuclid(RNG method, uint rand1, uint rand2)
        {
            return method.RecoverLower16BitsEuclid15(rand1 << 16, rand2 << 16);
        }

        /// <summary>
        /// Generates IVs from 2 RNG calls using 15 bits of each to generate 6 IVs (5bits each).
        /// </summary>
        /// <param name="r1">First rand frame</param>
        /// <param name="r2">Second rand frame</param>
        /// <param name="IVs">IVs that should be the result</param>
        /// <returns>IVs match random number IVs</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IVsMatch(uint r1, uint r2, IReadOnlyList<uint> IVs)
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
        /// <param name="r1">First rand frame</param>
        /// <param name="r2">Second rand frame</param>
        /// <returns>Array of 6 IVs</returns>
        internal static uint[] GetIVs(uint r1, uint r2)
        {
            return new[]
            {
                r1 & 31,
                r1 >> 5 & 31,
                r1 >> 10 & 31,
                r2 & 31,
                r2 >> 5 & 31,
                r2 >> 10 & 31,
            };
        }

        internal static int[] GetIVsInt32(uint r1, uint r2)
        {
            return new[]
            {
                (int)r1 & 31,
                (int)r1 >> 5 & 31,
                (int)r1 >> 10 & 31,
                (int)r2 & 31,
                (int)r2 >> 5 & 31,
                (int)r2 >> 10 & 31,
            };
        }

        private static uint GetIVChunk(uint[] IVs, int start)
        {
            uint val = 0;
            for (int i = 0; i < 3; i++)
                val |= IVs[i+start] << (5*i);
            return val;
        }

        public static IEnumerable<PIDIV> GetColoEReaderMatches(uint PID)
        {
            var top = PID >> 16;
            var bot = (ushort)PID;
            var xdc = GetSeedsFromPIDEuclid(RNG.XDRNG, top, bot);
            foreach (var seed in xdc)
            {
                var B = RNG.XDRNG.Prev(seed);
                var A = RNG.XDRNG.Prev(B);

                var C = RNG.XDRNG.Advance(A, 7);

                yield return new PIDIV { OriginSeed = RNG.XDRNG.Prev(C), RNG = RNGType.XDRNG, Type = PIDType.CXD };
            }
        }

        public static IEnumerable<PIDIV> GetPokeSpotSeeds(PKM pkm, int slot)
        {
            // Activate (rand % 3)
            // Munchlax / Bonsly (10%/30%)
            // Encounter Slot Value (ESV) = 50%/35%/15% rarity (0-49, 50-84, 85-99)
            var pid = pkm.PID;
            var top = pid >> 16;
            var bot = pid & 0xFFFF;
            var seeds = GetSeedsFromPIDEuclid(RNG.XDRNG, top, bot);
            foreach (var seed in seeds)
            {
                // check for valid encounter slot info
                if (!IsPokeSpotActivation(slot, seed, out uint s))
                    continue;
                yield return new PIDIV {OriginSeed = s, RNG = RNGType.XDRNG, Type = PIDType.PokeSpot};
            }
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
            s = RNG.XDRNG.Prev(seed);
            if ((s >> 16) % 3 != 0)
            {
                if ((s >> 16) % 100 < 10) // can't fail a munchlax/bonsly encounter check
                {
                    // todo
                }
                s = RNG.XDRNG.Prev(s);
                if ((s >> 16) % 3 != 0) // can't activate even if generous
                {
                    // todo
                }
            }
            return true;
        }

        private static bool IsPokeSpotSlotValid(int slot, uint esv)
        {
            return slot switch
            {
                0 when esv < 50 => true,
                1 when 50 <= esv && esv < 85 => true,
                2 when 85 <= esv => true,
                _ => false
            };
        }

        public static bool IsCompatible3(this PIDType val, IEncounterable encounter, PKM pkm)
        {
            switch (encounter)
            {
                case WC3 g:
                    if (val == g.Method)
                        return true;
                    // forced shiny eggs, when hatched, can lose their detectable correlation.
                    return g.IsEgg && !pkm.IsEgg && val == PIDType.None && (g.Method == PIDType.BACD_R_S || g.Method == PIDType.BACD_U_S);
                case EncounterStatic s:
                    switch (pkm.Version)
                    {
                        case (int)GameVersion.CXD: return val == PIDType.CXD || val == PIDType.CXD_ColoStarter || val == PIDType.CXDAnti;
                        case (int)GameVersion.E: return val == PIDType.Method_1; // no roamer glitch

                        case (int)GameVersion.FR:
                        case (int)GameVersion.LG:
                            return s.Roaming ? val.IsRoamerPIDIV(pkm) : val == PIDType.Method_1; // roamer glitch
                        default: // RS, roamer glitch && RSBox s/w emulation => method 4 available
                            return s.Roaming ? val.IsRoamerPIDIV(pkm) : MethodH14.Contains(val);
                    }
                case EncounterSlot w:
                    if (pkm.Version == 15)
                        return val == PIDType.PokeSpot;
                    return (w.Species == (int)Species.Unown ? MethodH_Unown : MethodH).Contains(val);
                default:
                    return val == PIDType.None;
            }
        }

        private static bool IsRoamerPIDIV(this PIDType val, PKM pkm)
        {
            // Roamer PIDIV is always Method 1.
            // M1 is checked before M1R. A M1R PIDIV can also be a M1 PIDIV, so check that collision.
            if (PIDType.Method_1_Roamer == val)
                return true;
            if (PIDType.Method_1 != val)
                return false;

            // only 8 bits are stored instead of 32 -- 5 bits HP, 3 bits for ATK.
            return !(pkm.IV_DEF != 0 || pkm.IV_SPE != 0 || pkm.IV_SPA != 0 || pkm.IV_SPD != 0 || pkm.IV_ATK > 7);
        }

        public static bool IsCompatible4(this PIDType val, IEncounterable encounter, PKM pkm)
        {
            switch (encounter)
            {
                case EncounterStatic s:
                    if (s == Encounters4.SpikyEaredPichu || (s.Location == Locations.PokeWalker4 && s.Gift)) // Pokewalker
                        return val == PIDType.Pokewalker;
                    if (s.Shiny == Shiny.Always)
                        return val == PIDType.ChainShiny;
                    if (val == PIDType.CuteCharm && IsCuteCharm4Valid(encounter, pkm))
                        return true;
                    return val == PIDType.Method_1;
                case EncounterSlot sl:
                    if (val == PIDType.Method_1)
                        return true;
                    if (val == PIDType.CuteCharm && IsCuteCharm4Valid(encounter, pkm))
                        return true;
                    if (val != PIDType.ChainShiny)
                        return false;
                    // Chain shiny with poke radar is only possible in DPPt in tall grass, safari zone do not allow pokeradar
                    // TypeEncounter TallGrass discard any cave or city
                    var ver = (GameVersion)pkm.Version;
                    var IsDPPt = ver == GameVersion.D || ver == GameVersion.P || ver == GameVersion.Pt;
                    return pkm.IsShiny && IsDPPt && sl.TypeEncounter == EncounterType.TallGrass && !Encounters4.SafariZoneLocation_4.Contains(sl.Location);
                case PGT _: // manaphy
                    return IsG4ManaphyPIDValid(val, pkm);
                case PCD d when d.Gift.PK.PID != 1:
                    return true; // already matches PCD's fixed PID requirement
                default: // eggs
                    return val == PIDType.None;
            }
        }

        private static bool IsG4ManaphyPIDValid(PIDType val, PKM pkm)
        {
            if (pkm.IsEgg)
            {
                if (pkm.IsShiny)
                    return false;
                if (val == PIDType.Method_1)
                    return true;
                return val == PIDType.G4MGAntiShiny && IsAntiShinyARNG();
            }

            if (val == PIDType.Method_1)
                return pkm.WasTradedEgg || !pkm.IsShiny; // can't be shiny on received game
            return val == PIDType.G4MGAntiShiny && (pkm.WasTradedEgg || IsAntiShinyARNG());

            bool IsAntiShinyARNG()
            {
                var shinyPID = RNG.ARNG.Prev(pkm.PID);
                return (pkm.TID ^ pkm.SID ^ (shinyPID & 0xFFFF) ^ (shinyPID >> 16)) < 8; // shiny proc
            }
        }

        private static bool IsCuteCharm4Valid(IEncounterable encounter, PKM pkm)
        {
            if (pkm.Species == (int)Species.Marill || pkm.Species == (int)Species.Azumarill)
            {
                return !IsCuteCharmAzurillMale(pkm.PID) // recognized as not Azurill
                      || encounter.Species == (int)Species.Azurill; // encounter must be male Azurill
            }

            return true;
        }

        private static bool IsCuteCharmAzurillMale(uint pid) => pid >= 0xC8 && pid <= 0xE0;

        private static void GetCuteCharmGenderSpecies(PKM pk, uint pid, out int genderValue, out int species)
        {
            // There are some edge cases when the gender ratio changes across evolutions.
            species = pk.Species;
            if (species == (int)Species.Ninjask)
            {
                species = (int)Species.Nincada; // Nincada evo chain travels from M/F -> Genderless Shedinja
                genderValue = PKX.GetGenderFromPID(species, pid);
                return;
            }

            switch (species)
            {
                // These evolved species cannot be encountered with cute charm.
                // 100% fixed gender does not modify PID; override this with the encounter species for correct calculation.
                // We can assume the re-mapped species's [gender ratio] is what was encountered.

                case (int)Species.Wormadam: species = (int)Species.Burmy; break; // Wormadam -> Burmy
                case (int)Species.Mothim: species = (int)Species.Burmy; break; // Mothim -> Burmy
                case (int)Species.Vespiquen: species = (int)Species.Combee; break; // Vespiquen -> Combee
                case (int)Species.Gallade: species = (int)Species.Kirlia; break; // Gallade -> Kirlia/Ralts
                case (int)Species.Froslass: species = (int)Species.Snorunt; break; // Froslass -> Snorunt

                // Changed gender ratio (25% M -> 50% M) needs special treatment.
                // Double check the encounter species with IsCuteCharm4Valid afterwards.
                case (int)Species.Marill: case (int)Species.Azumarill: // Azurill & Marill/Azumarill collision
                    if (IsCuteCharmAzurillMale(pid))
                    {
                        species = (int)Species.Azurill;
                        genderValue = 0;
                        return;
                    }
                    break;
            }
            genderValue = pk.Gender;
        }

        private static readonly PIDType[] MethodH = { PIDType.Method_1, PIDType.Method_2, PIDType.Method_3, PIDType.Method_4 };
        private static readonly PIDType[] MethodH14 = { PIDType.Method_1, PIDType.Method_4 };
        private static readonly PIDType[] MethodH_Unown = { PIDType.Method_1_Unown, PIDType.Method_2_Unown, PIDType.Method_3_Unown, PIDType.Method_4_Unown };
    }
}
