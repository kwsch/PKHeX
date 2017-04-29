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
            var pid = pk.PID;
            var top = pid >> 16;
            var bot = pid & 0xFFFF;

            var iIVs = pk.IVs;
            var IVs = new uint[6];
            for (int i = 0; i < 6; i++)
                IVs[i] = (uint)iIVs[i];

            PIDIV pidiv;
            if (getLCRNGMatch(top, bot, IVs, out pidiv))
                return pidiv;
            if (getXDRNGMatch(top, bot, IVs, out pidiv))
                return pidiv;

            // Special cases
            if (getChannelMatch(top, bot, IVs, out pidiv))
                return pidiv;
            if (getMG4Match(pid, IVs, out pidiv))
                return pidiv;
            if (getModifiedPID(pid, out pidiv))
                return pidiv;
            if (pid <= 0xFF && getCuteCharmMatch(pk, pid, out pidiv))
                return pidiv;

            return pidiv; // no match
        }

        private static bool getLCRNGMatch(uint top, uint bot, uint[] IVs, out PIDIV pidiv)
        {
            var reg = getSeedsFromPID(RNG.LCRNG, top, bot);
            foreach (var seed in reg)
            {
                // A and B are already used by PID
                var B = RNG.LCRNG.Advance(seed, 2);

                // Method 1/2/4 can use 3 different RNG frames
                var C = RNG.LCRNG.Next(B);
                var D = RNG.LCRNG.Next(C);

                if (getIVs(C >> 16, D >> 16).SequenceEqual(IVs)) // ABCD
                {
                    pidiv = new PIDIV {OriginSeed = seed, RNG = RNG.LCRNG, Type = PIDType.Method_1};
                    return true;
                }

                var E = RNG.LCRNG.Next(D);
                if (getIVs(D >> 16, E >> 16).SequenceEqual(IVs)) // ABDE
                {
                    pidiv = new PIDIV {OriginSeed = seed, RNG = RNG.LCRNG, Type = PIDType.Method_2};
                    return true;
                }

                if (getIVs(C >> 16, E >> 16).SequenceEqual(IVs)) // ABCE
                {
                    pidiv = new PIDIV {OriginSeed = seed, RNG = RNG.LCRNG, Type = PIDType.Method_4};
                    return true;
                }
            }
            pidiv = null;
            return false;
        }
        private static bool getXDRNGMatch(uint top, uint bot, uint[] IVs, out PIDIV pidiv)
        {
            var xdc = getSeedsFromPID(RNG.XDRNG, bot, top);
            foreach (var seed in xdc)
            {
                var C = RNG.XDRNG.Reverse(seed, 3);

                var D = RNG.XDRNG.Next(C);
                var E = RNG.XDRNG.Next(D);

                if (!getIVs(D >> 16, E >> 16).SequenceEqual(IVs))
                    continue;

                pidiv = new PIDIV {OriginSeed = seed, RNG = RNG.XDRNG, Type = PIDType.XDC};
                return true;
            }
            pidiv = null;
            return false;
        }
        private static bool getChannelMatch(uint top, uint bot, uint[] IVs, out PIDIV pidiv)
        {
            var channel = getSeedsFromPID(RNG.XDRNG, bot, top ^ 0x8000);
            foreach (var seed in channel)
            {
                var E = RNG.XDRNG.Advance(seed, 5);
                if (!getIVs(RNG.XDRNG, E).SequenceEqual(IVs))
                    continue;

                pidiv = new PIDIV {OriginSeed = RNG.XDRNG.Prev(seed), RNG = RNG.XDRNG, Type = PIDType.Channel};
                return true;
            }
            pidiv = null;
            return false;
        }
        private static bool getMG4Match(uint pid, uint[] IVs, out PIDIV pidiv)
        {
            uint mg4Rev = RNG.ARNG.Prev(pid);
            var mg4 = getSeedsFromPID(RNG.LCRNG, mg4Rev >> 16, mg4Rev & 0xFFFF);
            foreach (var seed in mg4)
            {
                var B = RNG.LCRNG.Advance(seed, 2);
                var C = RNG.LCRNG.Next(B);
                var D = RNG.LCRNG.Next(C);
                if (!getIVs(C >> 16, D >> 16).SequenceEqual(IVs))
                    continue;

                pidiv = new PIDIV {OriginSeed = seed, RNG = RNG.LCRNG, Type = PIDType.G4AntiShiny};
                return true;
            }
            pidiv = null;
            return false;
        }
        private static bool getModifiedPID(uint pid, out PIDIV pidiv)
        {
            // generation 5 shiny PIDs
            // todo
            pidiv = null;
            return false;
        }
        private static bool getCuteCharmMatch(PKM pk, uint pid, out PIDIV pidiv)
        {
            int genderValue = pk.Gender;
            switch (genderValue)
            {
                case 2: break; // can't cute charm a genderless pkm
                case 0: // male
                    var gr = pk.PersonalInfo.Gender;
                    if (254 <= gr) // no modification for PID
                        break;
                    if (pk.PID < gr)
                        break;
                    if (pk.PID >= gr + 25)
                        break;

                    pidiv = new PIDIV { OriginSeed = 0, RNG = RNG.LCRNG, Type = PIDType.CuteCharm };
                    return true;
                case 1: // female
                    if (pk.PID >= 25)
                        break; // nope
                    if (254 <= pk.PersonalInfo.Gender) // no modification for PID
                        break;

                    pidiv = new PIDIV { OriginSeed = 0, RNG = RNG.LCRNG, Type = PIDType.CuteCharm };
                    return true;
            }
            pidiv = null;
            return false;
        }

        private static PIDIV AnalyzeGB(PKM pk)
        {
            return null;
        }

        private static IEnumerable<uint> getSeedsFromPID(RNG method, uint top, uint bot)
        {
            uint cmp = top << 16;
            uint start = bot << 16;
            uint end = start | 0xFFFF;
            for (uint i = start; i <= end; i++)
                if ((method.Next(i) & 0xFFFF0000) == cmp)
                    yield return method.Prev(i);
        }
        private static IEnumerable<uint> getSeedsFromIVs(RNG method, uint top, uint bot)
        {
            uint cmp = top << 16 & 0x7FFF0000;
            uint start = bot << 16 & 0x7FFF0000;
            uint end = start | 0xFFFF;
            for (uint i = start; i <= end; i++)
                if ((method.Next(i) & 0x7FFF0000) == cmp)
                    yield return method.Prev(i);

            start |= 0x80000000;
            end |= 0x80000000;
            for (uint i = start; i <= end; i++)
                if ((method.Next(i) & 0x7FFF0000) == cmp)
                    yield return method.Prev(i);
        }

        /// <summary>
        /// Generates IVs from 2 RNG calls using 15 bits of each to generate 6 IVs (5bits each).
        /// </summary>
        /// <param name="r1">First rand frame</param>
        /// <param name="r2">Second rand frame</param>
        /// <returns>Array of 6 IVs</returns>
        private static uint[] getIVs(uint r1, uint r2)
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
        private static uint[] getIVs(RNG method, uint seed)
        {
            uint[] ivs = new uint[6];
            for (int i = 0; i < 6; i++)
            {
                seed = method.Next(seed);
                ivs[i] = seed >> 27;
            }
            return ivs;
        }
    }
}
