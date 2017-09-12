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

            var iIVs = pk.IVs;
            var IVs = new uint[6];
            for (int i = 0; i < 6; i++)
                IVs[i] = (uint)iIVs[i];

            PIDIV pidiv;
            if (GetLCRNGMatch(top, bot, IVs, out pidiv))
                return pidiv;
            if (pk.Species == 201 && GetLCRNGUnownMatch(top, bot, IVs, out pidiv)) // frlg only
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
            return pk.GenNumber == 4
                ? pid <= 0xFF && GetCuteCharmMatch(pk, pid, out pidiv) || GetG5MGShinyMatch(pk, pid, out pidiv)
                : GetG5MGShinyMatch(pk, pid, out pidiv) || pid <= 0xFF && GetCuteCharmMatch(pk, pid, out pidiv);
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
                        pidiv = new PIDIV {OriginSeed = seed, RNG = RNG.LCRNG, Type = PIDType.Method_1};
                        return true;
                    }

                    var E = RNG.LCRNG.Next(D);
                    var ivE = E >> 16 & 0x7FFF;
                    if (iv2 == ivE) // ABCE
                    {
                        pidiv = new PIDIV {OriginSeed = seed, RNG = RNG.LCRNG, Type = PIDType.Method_4};
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
                        pidiv = new PIDIV {OriginSeed = seed, RNG = RNG.LCRNG, Type = PIDType.Method_2};
                        return true;
                    }
                }
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
                        pidiv = new PIDIV {OriginSeed = seed, RNG = RNG.LCRNG, Type = PIDType.Method_1_Unown};
                        return true;
                    }

                    var E = RNG.LCRNG.Next(D);
                    var ivE = E >> 16 & 0x7FFF;
                    if (iv2 == ivE) // BACE
                    {
                        pidiv = new PIDIV {OriginSeed = seed, RNG = RNG.LCRNG, Type = PIDType.Method_4_Unown};
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
                        pidiv = new PIDIV {OriginSeed = seed, RNG = RNG.LCRNG, Type = PIDType.Method_2_Unown};
                        return true;
                    }
                }
            }
            return GetNonMatch(out pidiv);
        }
        private static bool GetLCRNGRoamerMatch(uint top, uint bot, uint[] IVs, out PIDIV pidiv)
        {
            if (IVs.Skip(2).Any(iv => iv != 0) || IVs[1] > 7)
                return GetNonMatch(out pidiv);
            var iv1 = GetIVChunk(IVs, 0);
            var reg = GetSeedsFromPID(RNG.LCRNG, top, bot);
            foreach (var seed in reg)
            {
                // Only the first 8 bits are kept
                var ivC = RNG.LCRNG.Advance(seed, 3) >> 16 & 0x00FF;
                if (iv1 != ivC)
                    continue;

                pidiv = new PIDIV {OriginSeed = seed, RNG = RNG.LCRNG, Type = PIDType.Method_1_Roamer};
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

                if (!GetIVs(A >> 16, B >> 16).SequenceEqual(IVs))
                    continue;

                pidiv = new PIDIV {OriginSeed = RNG.XDRNG.Prev(A), RNG = RNG.XDRNG, Type = PIDType.CXD};
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

                pidiv = new PIDIV {OriginSeed = RNG.XDRNG.Prev(seed), RNG = RNG.XDRNG, Type = PIDType.Channel};
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
                if (!GetIVs(C >> 16, D >> 16).SequenceEqual(IVs))
                    continue;

                pidiv = new PIDIV {OriginSeed = seed, RNG = RNG.LCRNG, Type = PIDType.G4MGAntiShiny};
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
                var high = pid >> 16;
                if (((pk.TID ^ pk.SID ^ low) - high & 0xFFFE) == 0)
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

            int genderValue = pk.Gender;
            switch (genderValue)
            {
                case 2: break; // can't cute charm a genderless pkm
                case 0: // male
                    var gr = pk.PersonalInfo.Gender;
                    if (254 <= gr) // no modification for PID
                        break;
                    var rate = pk.Gender == 1 ? 0 : 25*(gr/25 + 1); // buffered
                    var nature = pid % 25;
                    if (nature + rate != pid)
                        break;

                    pidiv = new PIDIV {NoSeed = true, RNG = RNG.LCRNG, Type = PIDType.CuteCharm};
                    return true;
                case 1: // female
                    if (pid >= 25)
                        break; // nope
                    if (254 <= pk.PersonalInfo.Gender) // no modification for PID
                        break;

                    pidiv = new PIDIV {NoSeed = true, RNG = RNG.LCRNG, Type = PIDType.CuteCharm};
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
                while (true)
                {
                    var bit = s >> 16 & 1;
                    if (bit != (pid >> i & 1))
                        break;
                    s = RNG.LCRNG.Prev(s);
                    if (--i == 2)
                        break;
                }
                if (i != 2) // bit failed
                    continue;
                // Shiny Bits of PID validated
                var upper = s;
                if ((upper >> 16 & 7) != (pid >> 16 & 7))
                    continue;
                var lower = RNG.LCRNG.Prev(upper);
                if ((lower >> 16 & 7) != (pid & 7))
                    continue;

                var upid = ((pid & 0xFFFF) ^ pk.TID ^ pk.SID) & 0xFFF8 | (upper >> 16) & 0x7;
                if (upid != pid >> 16)
                    continue;

                s = RNG.LCRNG.Reverse(lower, 2); // unroll one final time to get the origin seed
                pidiv = new PIDIV {OriginSeed = s, RNG = RNG.LCRNG, Type = PIDType.ChainShiny};
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

                var PID = A & 0xFFFF0000 | low;
                if (PID != pid)
                {
                    uint idxor = (uint)(pk.TID ^ pk.SID);
                    bool isShiny = (idxor ^ PID >> 16 ^ PID & 0xFFFF) < 8;
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
                    pidiv = new PIDIV {OriginSeed = sn, RNG = RNG.LCRNG, Type = --type };
                    return true;
                }
                // no restricted seed found, thus unrestricted
                pidiv = new PIDIV {OriginSeed = s, RNG = RNG.LCRNG, Type = type};
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

            uint pid = (uint)((pk.TID ^ pk.SID) >> 8 ^ 0xFF) << 24; // the most significant byte of the PID is chosen so the Pokémon can never be shiny.
            // Ensure nature is set to required nature without affecting shininess
            pid += nature - pid % 25;

            // Ensure Gender is set to required gender without affecting other properties
            // If Gender is modified, modify the ability if appropriate
            int currentGender = pk.Gender;
            if (currentGender != 2) // either m/f
            {
                var gr = pk.PersonalInfo.Gender;
                var pidGender = (pid & 0xFF) < gr ? 1 : 0;
                if (currentGender != pidGender)
                {
                    if (currentGender == 0) // Male
                    {
                        pid += (uint) (((gr - (pid & 0xFF)) / 25 + 1) * 25);
                        if ((nature & 1) != (pid & 1))
                            pid += 25;
                    }
                    else
                    {
                        pid -= (uint) ((((pid & 0xFF) - gr) / 25 + 1) * 25);
                        if ((nature & 1) != (pid & 1))
                            pid -= 25;
                    }
                }
            }
            if (pid != oldpid)
                return GetNonMatch(out pidiv);
            pidiv = new PIDIV {NoSeed = true, RNG = RNG.LCRNG, Type = PIDType.Pokewalker};
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
            pidiv = null;
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
            uint PID = X & 0xFFFF0000 | idxor ^ X >> 16;
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

        private static PIDIV AnalyzeGB(PKM pk)
        {
            return null;
        }

        private static IEnumerable<uint> GetSeedsFromPID(RNG method, uint a, uint b)
        {
            Debug.Assert(a >> 16 == 0);
            Debug.Assert(b >> 16 == 0);
            uint second = a << 16;
            uint first = b << 16;
            return method.RecoverLower16Bits(first, second);
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
        /// <returns>Array of 6 IVs</returns>
        private static uint[] GetIVs(uint r1, uint r2)
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
                var esv = (seed>>16)%100;
                switch (slot)
                {
                    case 0:
                        if (esv < 50) break; // valid
                        continue;
                    case 1:
                        if (esv >= 50 && esv < 85) break; // valid
                        continue;
                    case 2:
                        if (esv >= 85) break;
                        continue;
                    default:
                        continue;
                }

                // check for valid activation
                var s = RNG.XDRNG.Prev(seed);
                if ((s>>16)%3 != 0)
                {
                    if ((s>>16)%100 < 10) // can't fail a munchlax/bonsly encounter check
                        continue;
                    s = RNG.XDRNG.Prev(s);
                    if ((s>>16)%3 != 0) // can't activate even if generous
                        continue;
                }
                yield return new PIDIV {OriginSeed = s, RNG = RNG.XDRNG, Type = PIDType.PokeSpot};
            }
        }

        public static bool IsCompatible3(this PIDType val, IEncounterable encounter, PKM pkm)
        {
            switch (encounter)
            {
                case WC3 g:
                    return val == g.Method;
                case EncounterStaticShadow d when d.EReader:
                    return val == PIDType.None; // All IVs are 0
                case EncounterStatic s:
                    switch (pkm.Version)
                    {
                        case (int)GameVersion.CXD: return val == PIDType.CXD;
                        case (int)GameVersion.E: return val == PIDType.Method_1; // no roamer glitch

                        case (int)GameVersion.FR:
                        case (int)GameVersion.LG:
                            return s.Roaming ? val == PIDType.Method_1_Roamer : val == PIDType.Method_1; // roamer glitch
                        default: // RS, roamer glitch && RSBox s/w emulation => method 4 available
                            return s.Roaming ? val == PIDType.Method_1_Roamer : MethodH14.Any(z => z == val);
                    }
                case EncounterSlot w:
                    if (pkm.Version == 15)
                        return val == PIDType.PokeSpot;
                    return (w.Species == 201 ? MethodH_Unown : MethodH).Any(z => z == val);
                default:
                    return val == PIDType.None;
            }
        }

        public static bool IsCompatible4(this PIDType val, IEncounterable encounter, PKM pkm)
        {
            switch (encounter)
            {
                case EncounterStatic s:
                    if (s == Encounters4.SpikyEaredPichu // nonshiny forced nature
                     || s.Location == 233 && s.Gift) // Pokewalker
                        return val == PIDType.Pokewalker;
                    return s.Shiny == true ? val == PIDType.ChainShiny : val == PIDType.Method_1;
                case EncounterSlot sl:
                    if (val == PIDType.Method_1)
                        return true;
                    if (val == PIDType.CuteCharm)
                        // Cute charm does not work with swarms pokemon
                        return sl.Type != SlotType.Swarm;
                    if (val != PIDType.ChainShiny)
                        return false;
                    // Chain shiny with poke radar is only possible in DPPt in tall grass, safari zone do not allow pokeradar
                    // TypeEncounter TallGrass discard any cave or city
                    var IsDPPt = GameVersion.DP.Contains((GameVersion)pkm.Version) || (GameVersion)pkm.Version == GameVersion.Pt;
                    return pkm.IsShiny && IsDPPt && sl.TypeEncounter == EncounterType.TallGrass && !Encounters4.SafariZoneLocation_4.Contains(sl.Location);
                case PGT _: // manaphy
                    return IsG4ManaphyPIDValid(val, pkm);
                default:
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

        private static readonly PIDType[] MethodH = { PIDType.Method_1, PIDType.Method_2, PIDType.Method_4 };
        private static readonly PIDType[] MethodH14 = { PIDType.Method_1, PIDType.Method_4 };
        private static readonly PIDType[] MethodH_Unown = { PIDType.Method_1_Unown, PIDType.Method_2_Unown, PIDType.Method_4_Unown };
    }
}
