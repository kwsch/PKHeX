using System.Collections.Generic;

namespace PKHeX.Core
{
    /// <summary>
    /// Locks associated to a given NPC PKM that appears before a <see cref="EncounterStaticShadow"/>.
    /// </summary>
    public sealed class NPCLock
    {
        public int Species;
        public uint? Nature = null;
        public uint? Gender = null;
        public bool Shadow = false;
    }

    /// <summary>
    /// Contains various Colosseum/XD 'wait for value' logic related to PKM generation.
    /// </summary>
    /// <remarks>
    /// "Locks" are referring to the <see cref="IEncounterable"/> being "locked" to a certain value, e.g. requiring Nature to be neutral.
    /// These locks cause the <see cref="PKM.PID"/> of the current <see cref="PKM"/> to be rerolled until the requisite lock is satisfied.
    /// <see cref="PKM.Nature"/> locks require a certain <see cref="Nature"/>, which is derived from the <see cref="PKM.PID"/>.
    /// <see cref="PKM.Gender"/> locks require a certain gender value, which is derived from the <see cref="PKM.PID"/> and <see cref="PersonalInfo.Gender"/> ratio.
    /// Not sure if Abilities are locked for the encounter, assume not. When this code is eventually utilized, our understanding can be tested!
    /// </remarks>
    public static class LockFinder
    {
        // Message Passing
        private struct SeedFrame
        {
            public uint PID;
            public int FrameID;
        }

        public static bool FindLockSeed(uint originSeed, IEnumerable<NPCLock> lockList, bool XD, out uint origin)
        {
            var locks = new Stack<NPCLock>(lockList);
            var pids = new Stack<uint>();
            var cache = new FrameCache(RNG.XDRNG.Reverse(originSeed, 2), RNG.XDRNG.Prev);
            var result = FindLockSeed(cache, 0, locks, null, pids, XD, out var originFrame);
            origin = cache.GetSeed(originFrame);
            return result;
        }

        // Recursively iterates to visit possible locks until all locks (or none) are satisfied.
        private static bool FindLockSeed(FrameCache cache, int ctr, Stack<NPCLock> Locks, NPCLock prior, Stack<uint> PIDs, bool XD, out int originFrame)
        {
            if (Locks.Count == 0)
                return VerifyNPC(cache, ctr, PIDs, XD, out originFrame);

            var l = Locks.Pop();
            foreach (var poss in FindPossibleLockFrames(cache, ctr, l, prior))
            {
                PIDs.Push(poss.PID); // possible match
                if (FindLockSeed(cache, poss.FrameID, Locks, l, PIDs, XD, out originFrame))
                    return true; // all locks are satisfied
                PIDs.Pop(); // no match, remove
            }

            Locks.Push(l); // return the lock, lock is impossible
            originFrame = 0;
            return false;
        }

        private static IEnumerable<SeedFrame> FindPossibleLockFrames(FrameCache cache, int ctr, NPCLock l, NPCLock prior)
        {
            if (prior == null || prior.Shadow)
                return GetSingleLockFrame(cache, ctr, l);

            return GetComplexLockFrame(cache, ctr, l, prior);
        }
        private static IEnumerable<SeedFrame> GetSingleLockFrame(FrameCache cache, int ctr, NPCLock l)
        {
            uint pid = cache[ctr + 1] << 16 | cache[ctr];
            if (MatchesLock(l, pid, PKX.GetGenderFromPID(l.Species, pid)))
                yield return new SeedFrame { FrameID = ctr + 6, PID = pid };
        }
        private static IEnumerable<SeedFrame> GetComplexLockFrame(FrameCache cache, int ctr, NPCLock l, NPCLock prior)
        {
            // Since the prior(next) lock is generated 7+2*n frames after, the worst case break is 7 frames after the PID.
            // Continue reversing until a sequential generation case is found.

            // Check 

            int start = ctr;
            do
            {
                int p7 = ctr - 7;
                if (p7 > start)
                {
                    uint cid = cache[p7 + 1] << 16 | cache[p7];
                    if (MatchesLock(prior, cid, PKX.GetGenderFromPID(prior.Species, cid)))
                        yield break;
                }
                uint pid = cache[ctr + 1] << 16 | cache[ctr];
                if (MatchesLock(l, pid, PKX.GetGenderFromPID(l.Species, pid)))
                    yield return new SeedFrame { FrameID = ctr + 6, PID = pid};

                ctr += 2;
            } while (true);
        }

        private static bool VerifyNPC(FrameCache cache, int ctr, IEnumerable<uint> PIDs, bool XD, out int originFrame)
        {
            originFrame = ctr+2;
            var tid = cache[ctr+1];
            var sid = cache[ctr];

            // verify none are shiny
            foreach (var pid in PIDs)
                if (IsShiny(tid, sid, pid))
                    return true; // todo
            return true;
        }

        // Helpers
        private static bool IsShiny(uint TID, uint SID, uint PID) => (TID ^ SID ^ (PID >> 16) ^ (PID & 0xFFFF)) < 8;
        private static bool IsShiny(int TID, int SID, uint PID) => (TID ^ SID ^ (PID >> 16) ^ (PID & 0xFFFF)) < 8;
        private static bool MatchesLock(NPCLock k, uint PID, int Gender)
        {
            if (k.Nature != null && k.Nature != PID % 25)
                return false;
            if (k.Gender != null && k.Gender != Gender)
                return false;
            return true;
        }

        // Colosseum/XD Starters
        public static bool IsXDStarterValid(uint seed, int TID, int SID)
        {
            // pidiv reversed 2x yields SID, 3x yields TID. shift by 7 if another PKM is generated prior
            var SIDf = RNG.XDRNG.Reverse(seed, 2);
            var TIDf = RNG.XDRNG.Prev(SIDf);
            return SIDf >> 16 == SID && TIDf >> 16 == TID;
        }
        public static bool IsColoStarterValid(int species, ref uint seed, int TID, int SID, uint pkPID, uint IV1, uint IV2)
        {
            // reverse the seed the bare minimum
            int rev = 2;
            if (species == 196)
                rev += 7;

            var rng = RNG.XDRNG;
            var SIDf = rng.Reverse(seed, rev);
            int ctr = 0;
            while (true)
            {
                if (SIDf >> 16 == SID && rng.Prev(SIDf) >> 16 == TID)
                    break;
                SIDf = rng.Prev(SIDf);
                if (ctr > 32) // arbitrary
                    return false;
                ctr++;
            }

            var next = rng.Next(SIDf);

            // generate Umbreon
            var PIDIV = GenerateValidColoStarterPID(ref next, TID, SID);
            if (species == 196) // need espeon, which is immediately next
                PIDIV = GenerateValidColoStarterPID(ref next, TID, SID);

            if (!PIDIV.Equals(pkPID, IV1, IV2))
                return false;
            seed = rng.Reverse(SIDf, 2);
            return true;
        }

        private struct PIDIVGroup
        {
            public uint PID;
            public uint IV1;
            public uint IV2;

            public bool Equals(uint pid, uint iv1, uint iv2) => PID == pid && IV1 == iv1 && IV2 == iv2;
        }

        private static PIDIVGroup GenerateValidColoStarterPID(ref uint uSeed, int TID, int SID)
        {
            var rng = RNG.XDRNG;
            PIDIVGroup group = new PIDIVGroup();

            uSeed = rng.Advance(uSeed, 2); // skip fakePID
            group.IV1 = (uSeed >> 16) & 0x7FFF;
            uSeed = rng.Next(uSeed);
            group.IV2 = (uSeed >> 16) & 0x7FFF;
            uSeed = rng.Next(uSeed);
            uSeed = rng.Advance(uSeed, 1); // skip ability call
            group.PID = GenerateStarterPID(ref uSeed, TID, SID);

            uSeed = rng.Advance(uSeed, 2); // PID calls consumed

            return group;
        }
        private static uint GenerateStarterPID(ref uint uSeed, int TID, int SID)
        {
            uint PID;
            const byte ratio = 0x20; // 12.5% F (can't be female)
            while (true)
            {
                var next = RNG.XDRNG.Next(uSeed);
                PID = (uSeed & 0xFFFF0000) | (next >> 16);
                if ((PID & 0xFF) > ratio && !IsShiny(TID, SID, PID))
                    break;
                uSeed = RNG.XDRNG.Next(next);
            }

            return PID;
        }
    }
}
