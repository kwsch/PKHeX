using System.Collections.Generic;

namespace PKHeX.Core
{
    public readonly struct SeedInfo
    {
        public readonly uint Seed;
        public readonly bool Charm3;

        private SeedInfo(uint seed, bool charm3 = false)
        {
            Seed = seed;
            Charm3 = charm3;
        }

        public override bool Equals(object obj) => obj is SeedInfo s && Equals(s);
        public bool Equals(SeedInfo s) => s.Charm3 == Charm3 && s.Seed == Seed;
        public override int GetHashCode() => -1;
        public static bool operator ==(SeedInfo left, SeedInfo right) => left.Equals(right);
        public static bool operator !=(SeedInfo left, SeedInfo right) => !(left == right);

        /// <summary>
        /// Yields an enumerable list of seeds until another valid PID breaks the chain.
        /// </summary>
        /// <param name="pidiv">Seed and RNG data</param>
        /// <param name="info">Verification information</param>
        /// <returns>Seed information data, which needs to be unrolled once for the nature call.</returns>
        public static IEnumerable<SeedInfo> GetSeedsUntilNature(PIDIV pidiv, FrameGenerator info)
        {
            bool charm3 = false;

            var seed = pidiv.OriginSeed;
            yield return new SeedInfo(seed);

            var s1 = seed;
            var s2 = RNG.LCRNG.Prev(s1);
            while (true)
            {
                var a = s2 >> 16;
                var b = s1 >> 16;

                var pid = b << 16 | a;

                // Process Conditions
                switch (VerifyPIDCriteria(pid, info))
                {
                    case LockInfo.Pass:
                        yield break;
                    case LockInfo.Gender:
                        charm3 = true;
                        break;
                }

                s1 = RNG.LCRNG.Prev(s2);
                s2 = RNG.LCRNG.Prev(s1);

                yield return new SeedInfo(s1, charm3);
            }
        }

        /// <summary>
        /// Yields an enumerable list of seeds until another valid PID breaks the chain.
        /// </summary>
        /// <param name="pidiv">Seed and RNG data</param>
        /// <param name="info">Verification information</param>
        /// <param name="form">Unown Form lock value</param>
        /// <returns>Seed information data, which needs to be unrolled once for the nature call.</returns>
        public static IEnumerable<SeedInfo> GetSeedsUntilUnownForm(PIDIV pidiv, FrameGenerator info, int form)
        {
            var seed = pidiv.OriginSeed;
            yield return new SeedInfo(seed);

            var s1 = seed;
            var s2 = RNG.LCRNG.Prev(s1);
            while (true)
            {
                var a = s2 >> 16;
                var b = s1 >> 16;
                // PID is in reverse for FRLG Unown
                var pid = a << 16 | b;

                // Process Conditions
                if (PKX.GetUnownForm(pid) == form) // matches form, does it match nature?
                {
                    switch (VerifyPIDCriteria(pid, info))
                    {
                        case LockInfo.Pass: // yes
                            yield break;
                    }
                }

                s1 = RNG.LCRNG.Prev(s2);
                s2 = RNG.LCRNG.Prev(s1);

                yield return new SeedInfo(s1);
            }
        }

        private static LockInfo VerifyPIDCriteria(uint pid, FrameGenerator info)
        {
            // Nature locks are always a given
            var nval = pid % 25;
            if (nval != info.Nature)
                return LockInfo.Nature;

            if (!info.Gendered)
                return LockInfo.Pass;

            var gender = pid & 0xFF;
            if (info.GenderLow > gender || gender > info.GenderHigh)
                return LockInfo.Gender;

            return LockInfo.Pass;
        }
    }
}
