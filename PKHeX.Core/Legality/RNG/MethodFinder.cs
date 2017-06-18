using System.Collections.Generic;
using System.Linq;

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
            if (GetChannelMatch(top, bot, IVs, out pidiv))
                return pidiv;
            if (GetMG4Match(pid, IVs, out pidiv))
                return pidiv;

            if (pk.IsShiny)
            {
                if (GetChainShinyMatch(pk, pid, IVs, out pidiv))
                    return pidiv;
                if (GetModifiedPID(pk, pid, out pidiv))
                    return pidiv;
            }
            if (pid <= 0xFF && GetCuteCharmMatch(pk, pid, out pidiv))
                return pidiv;
            if (GetBACDMatch(pk, pid, IVs, out pidiv))
                return pidiv;

            return new PIDIV {Type=PIDType.None, NoSeed=true}; // no match
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
            pidiv = null;
            return false;
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
            pidiv = null;
            return false;
        }
        private static bool GetLCRNGRoamerMatch(uint top, uint bot, uint[] IVs, out PIDIV pidiv)
        {
            if (IVs.Skip(2).Any(iv => iv != 0) || IVs[1] > 7)
            {
                pidiv = null;
                return false;
            }
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
            pidiv = null;
            return false;
        }
        private static bool GetXDRNGMatch(uint top, uint bot, uint[] IVs, out PIDIV pidiv)
        {
            var xdc = GetSeedsFromPID(RNG.XDRNG, bot, top);
            foreach (var seed in xdc)
            {
                var B = RNG.XDRNG.Prev(seed);
                var A = RNG.XDRNG.Prev(B);

                if (!GetIVs(A >> 16, B >> 16).SequenceEqual(IVs))
                    continue;

                pidiv = new PIDIV {OriginSeed = RNG.XDRNG.Prev(A), RNG = RNG.XDRNG, Type = PIDType.CXD};
                return true;
            }
            pidiv = null;
            return false;
        }
        private static bool GetChannelMatch(uint top, uint bot, uint[] IVs, out PIDIV pidiv)
        {
            var channel = GetSeedsFromPID(RNG.XDRNG, bot, top ^ 0x8000);
            foreach (var seed in channel)
            {
                var E = RNG.XDRNG.Advance(seed, 5);
                if (!GetIVs(RNG.XDRNG, E).SequenceEqual(IVs))
                    continue;

                pidiv = new PIDIV {OriginSeed = RNG.XDRNG.Prev(seed), RNG = RNG.XDRNG, Type = PIDType.Channel};
                return true;
            }
            pidiv = null;
            return false;
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
            pidiv = null;
            return false;
        }
        private static bool GetModifiedPID(PKM pk, uint pid, out PIDIV pidiv)
        {
            var low = pid & 0xFFFF;
            // generation 5 shiny PIDs
            if (low <= 0xFF)
            {
                var high = pid >> 16;
                if ((pk.TID ^ pk.SID ^ low) == high)
                {
                    pidiv = new PIDIV {NoSeed = true, Type = PIDType.G5MGShiny};
                    return true;
                }
            }

            pidiv = null;
            return false;
        }
        private static bool GetCuteCharmMatch(PKM pk, uint pid, out PIDIV pidiv)
        {
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
            pidiv = null;
            return false;
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

            pidiv = null;
            return false;
        }
        private static bool GetBACDMatch(PKM pk, uint pid, uint[] IVs, out PIDIV pidiv)
        {
            var bot = GetIVChunk(IVs, 0);
            var top = GetIVChunk(IVs, 3);
            var reg = GetSeedsFromIVs(RNG.LCRNG, top, bot);
            foreach (var seed in reg)
            {
                var B = seed;
                var A = RNG.LCRNG.Prev(B);

                var PID = A & 0xFFFF0000 | B >> 16;
                bool isShiny = (pk.TID ^ pk.SID ^ PID >> 16 ^ PID & 0xFFFF) < 8;
                bool forceShiny = false;
                bool antiShiny = false;
                if (PID != pid)
                {
                    if (!isShiny)
                    {
                        // check for force shiny pkm
                        if (!pk.IsShiny)
                            continue; // obviously not force shiny
                        
                        // 0-Origin
                        // 1-PIDH
                        // 2-PIDL (ends up unused)
                        // 3-FORCEBITS
                        // PID = PIDH << 16 | (SID ^ TID ^ PIDH)
                       
                        var X = RNG.LCRNG.Prev(A);
                        PID = X & 0xFFFF0000 | (uint)pk.SID ^ (uint)pk.TID ^ X >> 16;
                        PID &= 0xFFFFFFF8;
                        PID |= B >> 16 & 0x7; // lowest 3 bits

                        if (PID != pid)
                            continue;
                        forceShiny = true;
                    }
                    if (!forceShiny)
                    {
                        if ((PID + 8 & 0xFFFFFFF8) != pid)
                            continue;
                        antiShiny = true;
                    }
                }
                var s = RNG.LCRNG.Prev(A);

                // Check for prior Restricted seed
                var sn = s;
                for (int i = 0; i < 3; i++, sn = RNG.LCRNG.Prev(sn))
                {
                    if ((sn & 0xFFFF0000) != 0)
                        continue;
                    var type = forceShiny ? PIDType.BACD_R_S : antiShiny ? PIDType.BACD_R_A : PIDType.BACD_R;
                    pidiv = new PIDIV {OriginSeed = sn, RNG = RNG.LCRNG, Type = type};
                    return true;
                }
                // no restricted seed found, thus unrestricted
                var t = forceShiny ? PIDType.BACD_U_S : antiShiny ? PIDType.BACD_U_A : PIDType.BACD_U;
                pidiv = new PIDIV {OriginSeed = s, RNG = RNG.LCRNG, Type = t};
                return true;
            }
            pidiv = null;
            return false;
        }

        private static PIDIV AnalyzeGB(PKM pk)
        {
            return null;
        }

        private static IEnumerable<uint> GetSeedsFromPID(RNG method, uint a, uint b)
        {
            uint cmp = a << 16;
            uint x = b << 16;
            for (uint i = 0; i <= 0xFFFF; i++)
            {
                var seed = x | i;
                if ((method.Next(seed) & 0xFFFF0000) == cmp)
                    yield return method.Prev(seed);
            }
        }
        private static IEnumerable<uint> GetSeedsFromIVs(RNG method, uint a, uint b)
        {
            uint cmp = a << 16 & 0x7FFF0000;
            uint x = b << 16 & 0x7FFF0000;
            for (uint i = 0; i <= 0xFFFF; i++)
            {
                var seed = x | i;
                if ((method.Next(seed) & 0x7FFF0000) == cmp)
                    yield return method.Prev(seed);
            }

            x |= 0x80000000;
            for (uint i = 0; i <= 0xFFFF; i++)
            {
                var seed = x | i;
                if ((method.Next(seed) & 0x7FFF0000) == cmp)
                    yield return method.Prev(seed);
            }
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
        /// <summary>
        /// Generates an IV for each RNG call using the top 5 bits of frame seeds.
        /// </summary>
        /// <param name="method">RNG advancement method</param>
        /// <param name="seed">RNG seed</param>
        /// <returns>Array of 6 IVs</returns>
        private static uint[] GetIVs(RNG method, uint seed)
        {
            uint[] ivs = new uint[6];
            for (int i = 0; i < 6; i++)
            {
                seed = method.Next(seed);
                ivs[i] = seed >> 27;
            }
            return ivs;
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
            var seeds = GetSeedsFromPID(RNG.XDRNG, bot, top);
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
                    if (s == Legal.SpikyEaredPichu) // nonshiny forced nature, undocumented
                        return val == PIDType.None;
                    return s.Shiny == true ? val == PIDType.ChainShiny : val == PIDType.Method_1;
                case EncounterSlot _:
                    return val == PIDType.Method_1;
                case PGT _: // manaphy
                    return IsG4ManaphyPIDValid(val, pkm);
                default:
                    return val == PIDType.None;
            }
        }
        private static bool IsG4ManaphyPIDValid(PIDType val, PKM pkm)
        {
            if (val == PIDType.Method_1)
                return pkm.WasTradedEgg || !pkm.IsShiny; // can't be shiny on received game
            if (val != PIDType.G4MGAntiShiny)
                return false;
            if (pkm.WasTradedEgg)
                return true;
            var shinyPID = RNG.ARNG.Prev(pkm.PID);
            return (pkm.TID ^ pkm.SID ^ (shinyPID & 0xFFFF) ^ (shinyPID >> 16)) < 8; // shiny proc
        }

        private static readonly PIDType[] MethodH = { PIDType.Method_1, PIDType.Method_2, PIDType.Method_4 };
        private static readonly PIDType[] MethodH14 = { PIDType.Method_1, PIDType.Method_4 };
        private static readonly PIDType[] MethodH_Unown = { PIDType.Method_1_Unown, PIDType.Method_2_Unown, PIDType.Method_4_Unown };
    }
}
