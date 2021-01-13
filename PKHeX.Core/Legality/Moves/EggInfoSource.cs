using System;
using System.Collections.Generic;
using System.Linq;

namespace PKHeX.Core
{
    internal sealed class EggInfoSource
    {
        public EggInfoSource(PKM pkm, EncounterEgg e)
        {
            // Eggs with special moves cannot inherit levelup moves as the current moves are predefined.
            AllowInherited = e.Species != 489 && e.Species != 490;

            // Level up moves can only be inherited if ditto is not the mother.
            bool AllowLevelUp = Breeding.GetCanInheritMoves(e.Species);
            Base = MoveList.GetBaseEggMoves(pkm, e.Species, e.Form, e.Version, e.Level);

            Egg = MoveEgg.GetEggMoves(pkm.PersonalInfo, e.Species, e.Form, e.Version, e.Generation);
            LevelUp = AllowLevelUp
                ? MoveList.GetBaseEggMoves(pkm, e.Species,  e.Form, e.Version, 100).Except(Base).ToList()
                : (IReadOnlyList<int>)Array.Empty<int>();
            Tutor = e.Version == GameVersion.C
                ? MoveTutor.GetTutorMoves(pkm, e.Species, 0, false, 2).ToList()
                : (IReadOnlyList<int>)Array.Empty<int>();

            // Only TM/HM moves from the source game of the egg, not any other games from the same generation
            TMHM = MoveTechnicalMachine.GetTMHM(pkm, pkm.Species, pkm.Form, e.Generation, e.Version).ToList();

            // Non-Base moves that can magically appear in the regular movepool
            bool volt = (e.Generation > 3 || e.Version == GameVersion.E) && Legal.LightBall.Contains(pkm.Species);
            if (volt)
            {
                Egg = Egg.ToList(); // array->list
                Egg.Add((int)Move.VoltTackle); // Volt Tackle
            }
        }

        public bool AllowInherited { get; }
        public IReadOnlyList<int> Base { get; }
        public IList<int> Egg { get; }
        public IReadOnlyList<int> Tutor { get; }
        public IReadOnlyList<int> TMHM { get; }
        public IReadOnlyList<int> LevelUp { get; }

        public bool IsInherited(int m)
        {
            if (m == 0)
                return false;
            if (Base.Contains(m))
                return false;
            return Egg.Contains(m) || LevelUp.Contains(m) || TMHM.Contains(m) || Tutor.Contains(m);
        }
    }
}
