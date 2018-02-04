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
        public uint? Ability = null;
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
        private sealed class SeedPID
        {
            public uint PID;
            public uint Seed;
        }

        public static bool FindLockSeed(uint originSeed, IEnumerable<NPCLock> lockList, bool XD, out uint origin)
        {
            var locks = new Stack<NPCLock>(lockList);
            var pids = new Stack<uint>();
            originSeed = RNG.XDRNG.Reverse(originSeed, 2);
            return FindLockSeed(originSeed, locks, null, pids, XD, out origin);
        }

        // Recursively iterates to visit possible locks until all locks (or none) are satisfied.
        private static bool FindLockSeed(uint seed, Stack<NPCLock> Locks, NPCLock prior, Stack<uint> PIDs, bool XD, out uint origin)
        {
            if (Locks.Count == 0)
                return VerifyNPC(seed, PIDs, XD, out origin);

            var l = Locks.Pop();
            foreach (var poss in FindPossibleLockFrames(seed, l, prior))
            {
                PIDs.Push(poss.PID); // possible match
                if (FindLockSeed(poss.Seed, Locks, l, PIDs, XD, out origin))
                    return true; // all locks are satisfied
                PIDs.Pop(); // no match, remove
            }
            Locks.Push(l); // return the lock, lock is impossible

            origin = seed;
            return false;
        }

        private static IEnumerable<SeedPID> FindPossibleLockFrames(uint seed, NPCLock l, NPCLock prior)
        {
            uint prev0 = seed;
            uint prev1 = RNG.XDRNG.Prev(prev0);
            uint pid = (prev1 & 0xFFFF0000) | (prev0 >> 16);

            if (prior == null)
            {
                if (MatchesLock(l, pid, PKX.GetGenderFromPID(l.Species, pid)))
                    yield return new SeedPID { Seed = RNG.XDRNG.Reverse(prev1, 6), PID = pid };
                yield break;
            }
            do
            {
                if (MatchesLock(prior, pid, PKX.GetGenderFromPID(prior.Species, pid)))
                    yield break; // prior lock breaks our chain!
                if (MatchesLock(l, pid, PKX.GetGenderFromPID(l.Species, pid)))
                    yield return new SeedPID { Seed = RNG.XDRNG.Reverse(prev1, 6), PID = pid };

                prev0 = RNG.XDRNG.Prev(prev1);
                prev1 = RNG.XDRNG.Prev(prev0);
                pid = (prev1 & 0xFFFF0000) | (prev0 >> 16);
            } while (true);

        }
        private static bool VerifyNPC(uint seed, IEnumerable<uint> PIDs, bool XD, out uint origin)
        {
            var seed1 = RNG.XDRNG.Prev(seed);
            origin = RNG.XDRNG.Prev(seed1);
            var tid = seed1 >> 16;
            var sid = seed >> 16;

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
