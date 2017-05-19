using System.Collections.Generic;

namespace PKHeX.Core
{
    public class NPCLock
    {
        public int Species;
        public uint? Nature = null;
        public uint? Gender = null;
        public uint? Ability = null;
    }
    public static class LockFinder
    {
        // Message Passing
        private class SeedPID
        {
            public uint PID;
            public uint Seed;
        }

        // Recursively iterates to visit possible locks until all locks (or none) are satisfied.
        public static bool FindLockSeed(uint seed, RNG RNG, Stack<NPCLock> Locks, NPCLock prior, Stack<uint> PIDs, bool XD, out uint origin)
        {
            if (Locks.Count == 0)
                return VerifyNPC(seed, RNG, PIDs, XD, out origin);

            var l = Locks.Pop();
            foreach (var poss in FindPossibleLockFrames(seed, RNG, l, prior))
            {
                PIDs.Push(poss.PID); // possible match
                if (FindLockSeed(poss.Seed, RNG, Locks, l, PIDs, XD, out origin))
                    return true; // all locks are satisfied
                PIDs.Pop(); // no match, remove
            }
            Locks.Push(l); // return the lock, lock is impossible

            origin = seed;
            return false;
        }

        // Restriction Checking
        private static IEnumerable<SeedPID> FindPossibleLockFrames(uint seed, RNG RNG, NPCLock l, NPCLock prior)
        {
            // todo: check for premature breaks
            do
            {
                // todo: generate PKM for checking
                uint pid = 0;
                int gender = 0;
                int abil = 0;
                uint origin = 0; // possible to defer calc to yield?

                if (prior == null)
                {
                    if (MatchesLock(l, pid, gender, abil))
                        yield return new SeedPID { Seed = origin, PID = pid };
                    yield break;
                }
                if (MatchesLock(prior, pid, gender, abil))
                    yield break; // prior lock breaks our chain!
                if (MatchesLock(l, pid, gender, abil))
                    yield return new SeedPID { Seed = origin, PID = pid };

            } while (true);

        }
        private static bool VerifyNPC(uint seed, RNG RNG, Stack<uint> PIDs, bool XD, out uint origin)
        {
            // todo: get trainer TID/SID/Origin Seed
            origin = 0;
            var tid = 0;
            var sid = 0;

            // verify none are shiny
            var arr = PIDs.ToArray();
            for (int i = 0; i < PIDs.Count; i++)
                if (IsShiny(tid, sid, arr[i]))
                    return false;
            return true;
        }

        // Helpers
        private static bool IsShiny(int TID, int SID, uint PID) => (TID ^ SID ^ (PID >> 16) ^ (PID & 0xFFFF)) < 8;
        private static bool MatchesLock(NPCLock k, uint PID, int Gender, int AbilityNumber)
        {
            if (k.Nature != null && k.Nature != PID % 25)
                return false;
            if (k.Gender != null && k.Gender != Gender)
                return false;
            if (k.Ability != null && k.Ability != AbilityNumber)
                return false;
            return true;
        }
    }
}
