using System.Collections.Generic;

namespace PKHeX.Core
{
    public struct SeedInfo
    {
        public uint Seed;
        public bool Charm3;

        public static IEnumerable<SeedInfo> GetSeedsUntilNature(PIDIV pidiv, FrameGenerator info)
        {
            bool reverse = pidiv.Type.IsReversedPID();
            bool charm3 = false;

            var seed = pidiv.OriginSeed;
            yield return new SeedInfo { Seed = seed };

            var s1 = seed;
            var s2 = pidiv.RNG.Prev(s1);
            while (true)
            {
                var a = s2 >> 16;
                var b = s1 >> 16;

                var pid = reverse ? a << 16 | b : b << 16 | a;

                // Process Conditions
                switch (VerifyPIDCriteria(pid, info))
                {
                    case LockInfo.Pass:
                        yield break;
                    case LockInfo.Gender:
                        charm3 = true;
                        break;
                }

                s1 = pidiv.RNG.Prev(s2);
                s2 = pidiv.RNG.Prev(s1);

                yield return new SeedInfo { Seed = s1, Charm3 = charm3 };
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
