using System;
using System.Collections.Generic;
using System.Linq;

namespace PKHeX.Core
{
    internal class EggInfoSource
    {
        public EggInfoSource(PKM pkm, IReadOnlyList<int> specialMoves, EncounterEgg e)
        {
            // Eggs with special moves cannot inherit levelup moves as the current moves are predefined.
            Special = specialMoves;
            bool notSpecial = Special.Count == 0;
            AllowInherited = notSpecial && !pkm.WasGiftEgg && pkm.Species != 489 && pkm.Species != 490;

            // Level up moves can only be inherited if ditto is not the mother.
            bool AllowLevelUp = Legal.GetCanInheritMoves(e.Species);
            Base = Legal.GetBaseEggMoves(pkm, e.Species, e.Form, e.Version, e.Level);

            Egg = MoveEgg.GetEggMoves(pkm, e.Species, e.Form, e.Version);
            LevelUp = AllowLevelUp
                ? Legal.GetBaseEggMoves(pkm, e.Species,  e.Form, e.Version, 100).Except(Base).ToList()
                : (IReadOnlyList<int>)Array.Empty<int>();
            Tutor = e.Version == GameVersion.C
                ? MoveTutor.GetTutorMoves(pkm, pkm.Species, pkm.AltForm, false, 2).ToList()
                : (IReadOnlyList<int>)Array.Empty<int>();

            // Only TM/HM moves from the source game of the egg, not any other games from the same generation
            TMHM = MoveTechnicalMachine.GetTMHM(pkm, pkm.Species, pkm.AltForm, pkm.GenNumber, e.Version).ToList();

            // Non-Base moves that can magically appear in the regular movepool
            bool volt = notSpecial && (pkm.GenNumber > 3 || e.Version == GameVersion.E) && Legal.LightBall.Contains(pkm.Species);
            if (volt)
            {
                Egg = Egg.ToList(); // array->list
                Egg.Add(344); // Volt Tackle
            }
        }

        public bool AllowInherited { get; }
        public IReadOnlyList<int> Base { get; }
        public IReadOnlyList<int> Special { get; }
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
            return Special.Contains(m) || Egg.Contains(m) || LevelUp.Contains(m) || TMHM.Contains(m) || Tutor.Contains(m);
        }
    }
}
