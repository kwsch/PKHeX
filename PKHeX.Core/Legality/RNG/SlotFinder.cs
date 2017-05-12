using System.Collections.Generic;

namespace PKHeX.Core
{
    public static class SlotFinder
    {
        public static List<uint> getSlotSeeds(PIDIV pidiv, uint nature, GameVersion v)
        {
            // gather possible nature determination seeds until a same-nature PID breaks the unrolling
            var seeds = getSeedsUntilNature(pidiv, nature);

            // get game generation criteria
            bool dppt = v == GameVersion.D || v == GameVersion.D || v == GameVersion.Pt;
            IEnumerable<SlotResult> info;
            switch (pidiv.Type)
            {
                case PIDType.CuteCharm:
                    info = filterCuteCharm(seeds, pidiv, dppt);
                    break;
                default:
                    bool canSync = v == GameVersion.E || (int)v > 5; // Emerald and Gen4
                    info = filterNatureSync(seeds, pidiv, nature, dppt, canSync);
                    break;
            }

            // games need to map 0-65535 to 0-99
            // dppt use /656, hgss&gen3 use %100
            foreach (var z in info)
            {

            }

            return null;
        }

        private static IEnumerable<uint> getSeedsUntilNature(PIDIV pidiv, uint nature)
        {
            bool reverse = pidiv.Type.IsReversedPID();

            var seed = pidiv.OriginSeed;
            yield return seed;

            var s1 = pidiv.RNG.Prev(seed);
            var s2 = pidiv.RNG.Prev(s1);
            while (true)
            {
                var a = s2 >> 16;
                var b = s1 >> 16;

                var pid = reverse ? b << 16 | a : a << 16 | b;
                if (pid % 25 == nature)
                    break;

                s1 = pidiv.RNG.Prev(s2);
                s2 = pidiv.RNG.Prev(s1);

                yield return s1;
            }
        }
        private static IEnumerable<SlotResult> filterNatureSync(IEnumerable<uint> seeds, PIDIV pidiv, uint nature, bool dppt, bool canSync)
        {
            foreach (var s in seeds)
            {
                var rand = s >> 16;
                bool sync = canSync && (rand & 1) == 0;
                bool reg = (dppt ? rand / 0xA3E : rand % 25) == nature;
                if (!sync && !reg) // doesn't generate nature frame
                    continue;

                uint prev = pidiv.RNG.Prev(s);
                if (canSync && reg) // check for failed sync
                {
                    var failsync = prev >> 31 != 0;
                    if (failsync)
                        yield return new SlotResult {Seed = pidiv.RNG.Prev(prev), Sync = false};
                }
                if (sync)
                    yield return new SlotResult {Seed = prev, Sync = true};
                if (reg)
                    yield return new SlotResult {Seed = prev, Sync = false};
            }
        }
        private static IEnumerable<SlotResult> filterCuteCharm(IEnumerable<uint> seeds, PIDIV pidiv, bool dppt)
        {
            foreach (var s in seeds)
            {
                var rand = s >> 16;
                bool charmProc = (dppt ? rand / 0x5556 : rand%3) == 0;
                if (charmProc)
                    yield return new SlotResult {Seed = pidiv.RNG.Prev(s)};
            }
        }
        
        public class SlotResult
        {
            public uint Seed { get; set; }
            public bool Sync { get; set; }    
        }
    }
}
