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
        private static bool VerifyNPC(uint seed, RNG RNG, IEnumerable<uint> PIDs, bool XD, out uint origin)
        {
            // todo: get trainer TID/SID/Origin Seed
            origin = 0;
            var tid = 0;
            var sid = 0;

            // verify none are shiny
            foreach (var pid in PIDs)
                if (IsShiny(tid, sid, pid))
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
