using System.Collections.Generic;
using System.Linq;

namespace PKHeX.Core
{
    public static class FrameFinder
    {
        /// <summary>
        /// Checks a <see cref="PIDIV"/> to see if any encounter frames can generate the spread. Requires further filtering against matched Encounter Slots and generation patterns.
        /// </summary>
        /// <param name="pidiv">Matched <see cref="PIDIV"/> containing info and <see cref="PIDIV.OriginSeed"/>.</param>
        /// <param name="pk"><see cref="PKM"/> object containing various accessible information required for the encounter.</param>
        /// <returns><see cref="IEnumerable{Frame}"/> to yield possible encounter details for further filtering</returns>
        public static IEnumerable<Frame> GetFrames(PIDIV pidiv, PKM pk)
        {
            if (pk.Version == (int)GameVersion.CXD)
                return Enumerable.Empty<Frame>();

            var info = new FrameGenerator(pk) {Nature = pk.EncryptionConstant % 25};

            // gather possible nature determination seeds until a same-nature PID breaks the unrolling
            var seeds = pk.Species == (int)Species.Unown && pk.FRLG // reversed await case
                ? SeedInfo.GetSeedsUntilUnownForm(pidiv, info, pk.Form)
                : SeedInfo.GetSeedsUntilNature(pidiv, info);

            var frames = pidiv.Type == PIDType.CuteCharm
                ? FilterCuteCharm(seeds, pidiv, info)
                : FilterNatureSync(seeds, pidiv, info);

            var refined = RefineFrames(frames, info);
            if (pk.Gen4 && pidiv.Type == PIDType.CuteCharm) // only permit cute charm successful frames
                return refined.Where(z => (z.Lead & ~LeadRequired.UsesLevelCall) == LeadRequired.CuteCharm);
            return refined;
        }

        private static IEnumerable<Frame> RefineFrames(IEnumerable<Frame> frames, FrameGenerator info)
        {
            return info.FrameType == FrameType.MethodH
                ? RefineFrames3(frames, info)
                : RefineFrames4(frames, info);
        }

        private static IEnumerable<Frame> RefineFrames3(IEnumerable<Frame> frames, FrameGenerator info)
        {
            // ESV
            // Level
            // Nature
            // Current Seed of the frame is the Level Calc (frame before nature)
            var list = new List<Frame>();
            foreach (var f in frames)
            {
                bool noLead = !info.AllowLeads && f.Lead != LeadRequired.None;
                if (noLead)
                    continue;

                var prev = info.RNG.Prev(f.Seed); // ESV
                var rand = prev >> 16;
                f.RandESV = rand;
                f.RandLevel = f.Seed >> 16;
                f.OriginSeed = info.RNG.Prev(prev);
                if (f.Lead != LeadRequired.CuteCharm) // needs proc checking
                    yield return f;

                // Generate frames for other slots after the regular slots
                if (info.AllowLeads && (f.Lead is LeadRequired.CuteCharm or LeadRequired.None))
                    list.Add(f);
            }
            foreach (var f in list)
            {
                var leadframes = GenerateLeadSpecificFrames3(f, info);
                foreach (var frame in leadframes)
                    yield return frame;
            }
        }

        private static IEnumerable<Frame> GenerateLeadSpecificFrames3(Frame f, FrameGenerator info)
        {
            // Check leads -- none in list if leads are not allowed
            // Certain leads inject a RNG call
            // 3 different rand places
            LeadRequired lead;
            var prev0 = f.Seed; // 0
            var prev1 = info.RNG.Prev(f.Seed); // -1
            var prev2 = info.RNG.Prev(prev1); // -2
            var prev3 = info.RNG.Prev(prev2); // -3

            // Rand call raw values
            var p0 = prev0 >> 16;
            var p1 = prev1 >> 16;
            var p2 = prev2 >> 16;

            // Cute Charm
            // -2 ESV
            // -1 Level
            //  0 CC Proc (Random() % 3 != 0)
            //  1 Nature
            if (info.Gendered)
            {
                bool cc = p0 % 3 != 0;
                if (f.Lead == LeadRequired.CuteCharm) // 100% required for frame base
                {
                    if (cc)
                        yield return info.GetFrame(prev2, LeadRequired.CuteCharm, p2, p1, prev3);
                    yield break;
                }
                lead = cc ? LeadRequired.CuteCharm : LeadRequired.CuteCharmFail;
                yield return info.GetFrame(prev2, lead, p2, p1, prev3);
            }
            if (f.Lead == LeadRequired.CuteCharm)
                yield break;

            // Pressure, Hustle, Vital Spirit = Force Maximum Level from slot
            // -2 ESV
            // -1 Level
            //  0 LevelMax proc (Random() & 1)
            //  1 Nature
            bool max = p0 % 2 == 1;
            lead = max ? LeadRequired.PressureHustleSpirit : LeadRequired.PressureHustleSpiritFail;
            yield return info.GetFrame(prev2, lead, p2, p1, prev3);

            // Keen Eye, Intimidate (Not compatible with Sweet Scent)
            // -2 ESV
            // -1 Level
            //  0 Level Adequate Check !(Random() % 2 == 1) rejects --  rand%2==1 is adequate
            //  1 Nature
            // Note: if this check fails, the encounter generation routine is aborted.
            if (max) // same result as above, no need to recalculate
            {
                lead = LeadRequired.IntimidateKeenEye;
                yield return info.GetFrame(prev2, lead, p2, p1, prev3);
            }

            // Static or Magnet Pull
            // -2 SlotProc (Random % 2 == 0)
            // -1 ESV (select slot)
            //  0 Level
            //  1 Nature
            bool force = p2 % 2 == 0;
            if (force)
            {
                // Since a failed proc is indistinguishable from the default frame calls, only generate if it succeeds.
                lead = LeadRequired.StaticMagnet;
                yield return info.GetFrame(prev2, lead, p1, p0, prev3);
            }
        }

        private static IEnumerable<Frame> RefineFrames4(IEnumerable<Frame> frames, FrameGenerator info)
        {
            var list = new List<Frame>();
            foreach (var f in frames)
            {
                // Current Seed of the frame is the ESV.
                var rand = f.Seed >> 16;
                f.RandESV = rand;
                f.RandLevel = rand; // unused
                f.OriginSeed = info.RNG.Prev(f.Seed);
                yield return f;

                // Create a copy for level; shift ESV and origin back
                var esv = f.OriginSeed >> 16;
                var origin = info.RNG.Prev(f.OriginSeed);
                var withLevel = info.GetFrame(f.Seed, f.Lead | LeadRequired.UsesLevelCall, esv, f.RandLevel, origin);
                yield return withLevel;

                if (f.Lead != LeadRequired.None)
                    continue;

                // Generate frames for other slots after the regular slots
                list.Add(f);
            }
            foreach (var f in list)
            {
                var leadframes = GenerateLeadSpecificFrames4(f, info);
                foreach (var frame in leadframes)
                    yield return frame;
            }
        }

        private static IEnumerable<Frame> GenerateLeadSpecificFrames4(Frame f, FrameGenerator info)
        {
            LeadRequired lead;
            var prev0 = f.Seed; // 0
            var prev1 = info.RNG.Prev(f.Seed); // -1
            var prev2 = info.RNG.Prev(prev1); // -2
            var prev3 = info.RNG.Prev(prev2); // -3

            // Rand call raw values
            var p0 = prev0 >> 16;
            var p1 = prev1 >> 16;
            var p2 = prev2 >> 16;

            // Cute Charm
            // -2 ESV
            // -1 Level (Optional)
            //  0 CC Proc (Random() % 3 != 0)
            //  1 Nature
            if (info.Gendered)
            {
                bool cc = (info.DPPt ? p0 / 0x5556 : p0 % 3) != 0;
                if (f.Lead == LeadRequired.CuteCharm) // 100% required for frame base
                {
                    if (!cc) yield break;
                    yield return info.GetFrame(prev2, LeadRequired.CuteCharm, p1, p1, prev2);
                    yield return info.GetFrame(prev2, LeadRequired.CuteCharm | LeadRequired.UsesLevelCall, p2, p1, prev3);
                    yield break;
                }
                lead = cc ? LeadRequired.CuteCharm : LeadRequired.CuteCharmFail;
                yield return info.GetFrame(prev2, lead, p1, p1, prev2);
                yield return info.GetFrame(prev2, lead | LeadRequired.UsesLevelCall, p2, p1, prev3);
            }
            if (f.Lead == LeadRequired.CuteCharm)
                yield break;

            // Pressure, Hustle, Vital Spirit = Force Maximum Level from slot
            // -2 ESV
            // -1 Level (Optional)
            //  0 LevelMax proc (Random() & 1)
            //  1 Nature
            bool max = (info.DPPt ? p0 >> 15 : p0 & 1) == 1;
            lead = max ? LeadRequired.PressureHustleSpirit : LeadRequired.PressureHustleSpiritFail;
            yield return info.GetFrame(prev2, lead, p1, p1, prev2);
            yield return info.GetFrame(prev2, lead | LeadRequired.UsesLevelCall, p2, p1, prev3);

            // Keen Eye, Intimidate (Not compatible with Sweet Scent)
            // -2 ESV
            // -1 Level (Optional)
            //  0 Level Adequate Check !(Random() % 2 == 1) rejects --  rand%2==1 is adequate
            //  1 Nature
            // Note: if this check fails, the encounter generation routine is aborted.
            if (max) // same result as above, no need to recalculate
            {
                lead = LeadRequired.IntimidateKeenEye;
                yield return info.GetFrame(prev2, lead, p1, p1, prev2);
                yield return info.GetFrame(prev2, lead | LeadRequired.UsesLevelCall, p2, p1, prev3);
            }

            // Static or Magnet Pull
            // -2 SlotProc (Random % 2 == 0)
            // -1 ESV (select slot)
            //  0 Level (Optional)
            //  1 Nature
            var force1 = (info.DPPt ? p1 >> 15 : p1 & 1) == 1;
            lead = force1 ? LeadRequired.StaticMagnet : LeadRequired.StaticMagnetFail;
            yield return info.GetFrame(prev2, lead, p0, p0, prev3);

            var force2 = (info.DPPt ? p2 >> 15 : p2 & 1) == 1;
            lead = (force2 ? LeadRequired.StaticMagnet : LeadRequired.StaticMagnetFail) | LeadRequired.UsesLevelCall;
            yield return info.GetFrame(prev2, lead, p1, p0, prev3);
        }

        /// <summary>
        /// Filters the input <see cref="SeedInfo"/> according to a Nature Lock frame generation pattern.
        /// </summary>
        /// <param name="seeds">Seed Information for the frame</param>
        /// <param name="pidiv">PIDIV Info for the frame</param>
        /// <param name="info">Search Info for the frame</param>
        /// <returns>Possible matches to the Nature Lock frame generation pattern</returns>
        private static IEnumerable<Frame> FilterNatureSync(IEnumerable<SeedInfo> seeds, PIDIV pidiv, FrameGenerator info)
        {
            foreach (var seed in seeds)
            {
                var s = seed.Seed;

                if (info.Safari3)
                {
                    // successful pokeblock activation
                    bool result = IsValidPokeBlockNature(s, info.Nature, out uint blockSeed);
                    if (result)
                        yield return info.GetFrame(blockSeed, seed.Charm3 ? LeadRequired.CuteCharm : LeadRequired.None);

                    // if no pokeblocks present (or failed call), fall out of the safari specific code and generate via the other scenarios
                }

                var rand = s >> 16;
                bool sync = info.AllowLeads && !seed.Charm3 && (info.DPPt ? rand >> 15 : rand & 1) == 0;
                bool reg = (info.DPPt ? rand / 0xA3E : rand % 25) == info.Nature;
                if (!sync && !reg) // doesn't generate nature frame
                    continue;

                uint prev = RNG.LCRNG.Prev(s);
                if (info.AllowLeads && reg) // check for failed sync
                {
                    var failsync = (info.DPPt ? prev >> 31 : (prev >> 16) & 1) != 1;
                    if (failsync)
                        yield return info.GetFrame(RNG.LCRNG.Prev(prev), LeadRequired.SynchronizeFail);
                }
                if (sync)
                    yield return info.GetFrame(prev, LeadRequired.Synchronize);
                if (reg)
                {
                    if (seed.Charm3)
                    {
                        yield return info.GetFrame(prev, LeadRequired.CuteCharm);
                    }
                    else
                    {
                        if (info.Safari3)
                            prev = RNG.LCRNG.Prev(prev); // wasted RNG call
                        yield return info.GetFrame(prev, LeadRequired.None);
                    }
                }
            }
        }

        private static bool IsValidPokeBlockNature(uint seed, uint nature, out uint natureOrigin)
        {
            if (nature % 6 == 0) // neutral
            {
                natureOrigin = 0;
                return false;
            }

            // unroll the RNG to a stack of seeds
            var stack = new Stack<uint>();
            for (uint i = 0; i < 25; i++)
            {
                for (uint j = 1 + i; j < 25; j++)
                    stack.Push(seed = RNG.LCRNG.Prev(seed));
            }

            natureOrigin = RNG.LCRNG.Prev(stack.Peek());
            if (natureOrigin >> 16 % 100 >= 80) // failed proc
                return false;

            // init natures
            uint[] natures = new uint[25];
            for (uint i = 0; i < 25; i++)
                natures[i] = i;

            // shuffle nature list
            for (uint i = 0; i < 25; i++)
            {
                for (uint j = 1 + i; j < 25; j++)
                {
                    var s = stack.Pop();
                    if ((s >> 16 & 1) == 0)
                        continue; // only swap if 1

                    var temp = natures[i];
                    natures[i] = natures[j];
                    natures[j] = temp;
                }
            }

            var likes = Pokeblock.GetLikedBlockFlavor(nature);
            // best case scenario is a perfect flavored pokeblock for the nature.
            // has liked flavor, and all other non-disliked flavors are present.
            // is it possible to skip this step?
            for (int i = 0; i < 25; i++)
            {
                var n = natures[i];
                if (n == nature)
                    break;

                var nl = Pokeblock.GetLikedBlockFlavor(natures[i]);
                if (nl == likes) // next random nature likes the same block as the desired nature
                    return false; // current nature is chosen instead, fail!
            }
            // unroll once more to hit the level calc (origin with respect for beginning the nature calcs)
            natureOrigin = RNG.LCRNG.Prev(natureOrigin);
            return true;
        }

        /// <summary>
        /// Filters the input <see cref="SeedInfo"/> according to a Cute Charm frame generation pattern.
        /// </summary>
        /// <param name="seeds">Seed Information for the frame</param>
        /// <param name="pidiv">PIDIV Info for the frame</param>
        /// <param name="info">Search Info for the frame</param>
        /// <returns>Possible matches to the Cute Charm frame generation pattern</returns>
        private static IEnumerable<Frame> FilterCuteCharm(IEnumerable<SeedInfo> seeds, PIDIV pidiv, FrameGenerator info)
        {
            foreach (var seed in seeds)
            {
                var s = seed.Seed;

                var rand = s >> 16;
                var nature = info.DPPt ? rand / 0xA3E : rand % 25;
                if (nature != info.Nature)
                    continue;

                var prev = RNG.LCRNG.Prev(s);
                var proc = prev >> 16;
                bool charmProc = (info.DPPt ? proc / 0x5556 : proc % 3) != 0; // 2/3 odds
                if (!charmProc)
                    continue;

                yield return info.GetFrame(prev, LeadRequired.CuteCharm);
            }
        }
    }
}
