using System.Collections.Generic;

namespace PKHeX.Core
{
    public sealed class TeamLockResult
    {
        public readonly uint OriginSeed;
        public readonly bool Valid;

        internal readonly TeamLock Spec;

        private int OriginFrame;
        private int RCSV;

        private readonly int TSV;
        private readonly Stack<NPCLock> Locks;
        private readonly FrameCache Cache;
        private readonly Stack<SeedFrame> Team;

        private const int NOT_FORCED = -1;

        internal TeamLockResult(TeamLock teamSpec, uint originSeed, int tsv)
        {
            Locks = new Stack<NPCLock>((Spec = teamSpec).Locks);
            Team = new Stack<SeedFrame>();
            Cache = new FrameCache(RNG.XDRNG.Reverse(originSeed, 2), RNG.XDRNG.Prev);
            TSV = tsv;

            Valid = FindLockSeed();
            if (Valid)
                OriginSeed = Cache.GetSeed(OriginFrame);
        }

        // Recursively iterates to visit possible locks until all locks (or none) are satisfied.
        private bool FindLockSeed(int frame = 0, NPCLock prior = null)
        {
            if (Locks.Count == 0) // full team reverse-generated
                return VerifyNPC(frame);

            var current = Locks.Pop();
            var locks = GetPossibleLocks(frame, current, prior);
            foreach (var l in locks)
            {
                Team.Push(l); // possible match
                if (FindLockSeed(l.FrameID, current))
                    return true; // all locks are satisfied
                Team.Pop(); // no match, remove
            }

            Locks.Push(current); // return the lock, lock is impossible
            return false;
        }

        private IEnumerable<SeedFrame> GetPossibleLocks(int ctr, NPCLock current, NPCLock prior)
        {
            if (prior?.Shadow != false)
                return GetSingleLock(ctr, current);

            return GetAllLocks(ctr, current, prior);
        }

        private IEnumerable<SeedFrame> GetSingleLock(int ctr, NPCLock current)
        {
            uint pid = Cache[ctr + 1] << 16 | Cache[ctr];
            if (current.MatchesLock(pid))
                yield return new SeedFrame { FrameID = ctr + (current.Seen ? 5 : 7), PID = pid };
        }

        private IEnumerable<SeedFrame> GetAllLocks(int ctr, NPCLock l, NPCLock prior)
        {
            // Since the prior(next) lock is generated 7+2*n frames after, the worst case break is 7 frames after the PID.
            // Continue reversing until a sequential generation case is found.

            int start = ctr;
            bool forcedOT = false;
            while (true)
            {
                int p7 = ctr - 7;
                if (p7 > start)
                {
                    // check for interrupting cpu team cases
                    var upper = Cache[p7 + 1];
                    var lower = Cache[p7];
                    uint cid = upper << 16 | lower;
                    var sv = (upper ^ lower) >> 3;
                    if (l.Shadow && sv == TSV) // shiny shadow mon, only ever true for XD.
                    {
                        // This interrupt is ignored! The result is shiny.
                    }
                    else if (prior.MatchesLock(cid)) // lock matched cpu mon
                    {
                        if (RCSV != NOT_FORCED) // CPU shiny value is required for a previous lock
                        {
                            if (sv != RCSV)
                            {
                                if (forcedOT) // current call to this method had forced the OT; clear the forced OT before breaking.
                                    RCSV = NOT_FORCED;
                                yield break; // Since we can't skip this interrupt, we're done.
                            }
                        }
                        else // No CPU shiny value forced yet. Lets try to skip this lock by requiring the eventual OT to get this shiny.
                        {
                            RCSV = (int)sv;
                            forcedOT = true;
                            // don't break
                        }
                    }
                }
                uint pid = Cache[ctr + 1] << 16 | Cache[ctr];
                if (l.MatchesLock(pid))
                    yield return new SeedFrame { FrameID = ctr + (l.Seen ? 5 : 7), PID = pid };

                ctr += 2;
            }
        }

        private bool VerifyNPC(int ctr)
        {
            var TID = Cache[ctr + 1];
            var SID = Cache[ctr];
            var CPUSV = (TID ^ SID) >> 3;
            if (RCSV != NOT_FORCED && RCSV != CPUSV)
                return false; // required CPU Trainer's shiny value did not match the required value.

            int pos = Team.Count - 1; // stack can't do a for loop :(
            foreach (var member in Team)
            {
                var pid = member.PID;
                var psv = ((pid & 0xFFFF) ^ (pid >> 16)) >> 3;
                bool shadow = Spec.Locks[pos].Shadow;
                if (!shadow && psv == CPUSV)
                    return false;
                if (psv == TSV) // always false for Colo, which doesn't set the TSV field.
                    return false;
                pos--;
            }

            OriginFrame = ctr + 2;
            return true;
        }
    }
}