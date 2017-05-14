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
        public static IEnumerable<Frame> getFrames(PIDIV pidiv, PKM pk)
        {
            // gather possible nature determination seeds until a same-nature PID breaks the unrolling
            FrameGenerator criteria = new FrameGenerator(pk, pidiv.Type);
            if (criteria.FrameType == FrameType.None)
                yield break;

            criteria.Nature = pk.EncryptionConstant % 25;

            IEnumerable<SeedInfo> seeds = SeedInfo.getSeedsUntilNature(pidiv, criteria);
            // get game generation criteria
            IEnumerable<Frame> info;
            switch (pidiv.Type)
            {
                case PIDType.CuteCharm:
                    info = filterCuteCharm(seeds, pidiv, criteria);
                    break;
                default:
                    info = filterNatureSync(seeds, pidiv, criteria);
                    break;
            }

            // games need to map 0-65535 to 0-99
            // dppt use /656, hgss&gen3 use %100
            foreach (var z in info)
                yield return z;
        }

        /// <summary>
        /// Filters the input <see cref="SeedInfo"/> according to a Nature Lock frame generation pattern.
        /// </summary>
        /// <param name="seeds">Seed Information for the frame</param>
        /// <param name="pidiv">PIDIV Info for the frame</param>
        /// <param name="info">Search Info for the frame</param>
        /// <returns>Possible matches to the Nature Lock frame generation pattern</returns>
        private static IEnumerable<Frame> filterNatureSync(IEnumerable<SeedInfo> seeds, PIDIV pidiv, FrameGenerator info)
        {
            foreach (var seed in seeds)
            {
                var s = seed.Seed;
                var rand = s >> 16;
                bool sync = info.CanSync && !seed.Charm3 && (info.DPPt ? rand >> 15 : rand & 1) == 0;
                bool reg = (info.DPPt ? rand / 0xA3E : rand % 25) == info.Nature;
                if (!sync && !reg) // doesn't generate nature frame
                    continue;

                uint prev = pidiv.RNG.Prev(s);
                if (info.CanSync && reg) // check for failed sync
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
        private static IEnumerable<Frame> filterCuteCharm(IEnumerable<SeedInfo> seeds, PIDIV pidiv, FrameGenerator info)
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
