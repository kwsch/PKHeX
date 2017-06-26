using System.Collections.Generic;

namespace PKHeX.Core
{
    public static class FrameFinder
    {
        /// <summary>
        /// Checks a <see cref="PIDIV"/> to see if any encounter frames can generate the spread. Requires further filtering against matched Encounter Slots and generation patterns.
        /// </summary>
        /// <param name="pidiv">Matched <see cref="PIDIV"/> containing <see cref="PIDIV.RNG"/> info and <see cref="PIDIV.OriginSeed"/>.</param>
        /// <param name="pk"><see cref="PKM"/> object containing various accessible information required for the encounter.</param>
        /// <returns><see cref="IEnumerable{Frame}"/> to yield possible encounter details for further filtering</returns>
        public static IEnumerable<Frame> GetFrames(PIDIV pidiv, PKM pk)
        {
            FrameGenerator info = new FrameGenerator(pidiv, pk);
            if (info.FrameType == FrameType.None)
                yield break;

            info.Nature = pk.EncryptionConstant % 25;

            // gather possible nature determination seeds until a same-nature PID breaks the unrolling
            IEnumerable<SeedInfo> seeds = SeedInfo.GetSeedsUntilNature(pidiv, info);

            var frames = pidiv.Type == PIDType.CuteCharm 
                ? FilterCuteCharm(seeds, pidiv, info) 
                : FilterNatureSync(seeds, pidiv, info);

            var refined = RefineFrames(frames, info);
            foreach (var z in refined)
                yield return z;
        }

        private static IEnumerable<Frame> RefineFrames(IEnumerable<Frame> frames, FrameGenerator info)
        {
            return info.FrameType == FrameType.MethodH
                ? RefineFrames3(frames, info)
                : RefineFrames4(frames, info);
        }

        private static IEnumerable<Frame> RefineFrames3(IEnumerable<Frame> frames, FrameGenerator info)
        {
            var list = new List<Frame>();
            foreach (var f in frames)
            {
                // Current Seed of the frame is the Level Calc
                var prev = info.RNG.Prev(f.Seed); // ESV
                var rand = prev >> 16;
                {
                    f.ESV = rand;
                    yield return f;
                }

                if (f.Lead != LeadRequired.None || !info.AllowLeads) // Emerald
                    continue;

                // Generate frames for other slots after the regular slots
                list.Add(f);
            }
            foreach (var f in list)
            {
                // Level Modifiers between ESV and Nature
                var prev = info.RNG.Prev(f.Seed); // Level
                prev = info.RNG.Prev(prev); // Level Proc
                var p16 = prev >> 16;

                yield return info.GetFrame(prev, LeadRequired.Intimidate, p16);
                yield return info.GetFrame(prev, LeadRequired.VitalSpirit, p16);

                // Slot Modifiers before ESV
                var force = (info.DPPt ? p16 >> 15 : p16 & 1) == 1;
                if (!force)
                    continue;

                var rand = f.Seed >> 16;
                yield return info.GetFrame(prev, LeadRequired.Static, rand);
                yield return info.GetFrame(prev, LeadRequired.MagnetPull, rand);
            }
        }
        private static IEnumerable<Frame> RefineFrames4(IEnumerable<Frame> frames, FrameGenerator info)
        {
            var list = new List<Frame>();
            foreach (var f in frames)
            {
                // Current Seed of the frame is the ESV.
                var rand = f.Seed >> 16;
                {
                    f.ESV = rand;
                    yield return f;
                }

                if (f.Lead != LeadRequired.None)
                    continue;

                // Generate frames for other slots after the regular slots
                list.Add(f);
            }
            foreach (var f in list)
            {
                // Level Modifiers between ESV and Nature
                var prev = info.RNG.Prev(f.Seed);
                var p16 = prev >> 16;

                yield return info.GetFrame(prev, LeadRequired.Intimidate, p16);
                yield return info.GetFrame(prev, LeadRequired.VitalSpirit, p16);

                // Slot Modifiers before ESV
                var force = (info.DPPt ? p16 >> 15 : p16 & 1) == 1;
                if (!force)
                    continue;

                var rand = f.Seed >> 16;
                yield return info.GetFrame(prev, LeadRequired.Static, rand);
                yield return info.GetFrame(prev, LeadRequired.MagnetPull, rand);
            }
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
                var rand = s >> 16;
                bool sync = info.AllowLeads && !seed.Charm3 && (info.DPPt ? rand >> 15 : rand & 1) == 0;
                bool reg = (info.DPPt ? rand / 0xA3E : rand % 25) == info.Nature;
                if (!sync && !reg) // doesn't generate nature frame
                    continue;

                uint prev = pidiv.RNG.Prev(s);
                if (info.AllowLeads && reg) // check for failed sync
                {
                    var failsync = (info.DPPt ? prev >> 31 : (prev >> 16) & 1) != 1;
                    if (failsync)
                        yield return info.GetFrame(pidiv.RNG.Prev(prev), LeadRequired.SynchronizeFail);
                }
                if (sync)
                    yield return info.GetFrame(prev, LeadRequired.Synchronize);
                if (reg)
                {
                    if (seed.Charm3)
                        yield return info.GetFrame(prev, LeadRequired.CuteCharm);
                    else
                        yield return info.GetFrame(prev, LeadRequired.None);
                }
            }
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

                var prev = pidiv.RNG.Prev(s);
                var proc = prev >> 16;
                bool charmProc = (info.DPPt ? proc / 0x5556 : proc % 3) == 0;
                if (!charmProc)
                    continue;

                yield return info.GetFrame(prev, LeadRequired.CuteCharm);
            }
        }
    }
}
