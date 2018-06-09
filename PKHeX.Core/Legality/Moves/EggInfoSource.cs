using System.Collections.Generic;
using System.Linq;

namespace PKHeX.Core
{
    internal class EggInfoSource
    {
        public EggInfoSource(PKM pkm, IEnumerable<int> specialMoves, EncounterEgg e)
        {
            // Eggs with special moves cannot inherit levelup moves as the current moves are predefined.
            Special = specialMoves.Where(m => m != 0).ToList();
            bool notSpecial = Special.Count == 0;
            AllowInherited = notSpecial && !pkm.WasGiftEgg && pkm.Species != 489 && pkm.Species != 490;

            // Level up moves can only be inherited if ditto is not the mother.
            bool AllowLevelUp = Legal.GetCanInheritMoves(pkm, e);
            Base = Legal.GetBaseEggMoves(pkm, e.Species, e.Version, e.Level);

            Egg = Legal.GetEggMoves(pkm, e.Species, pkm.AltForm, e.Version);
            LevelUp = AllowLevelUp
                ? Legal.GetBaseEggMoves(pkm, e.Species, e.Version, 100).Except(Base).ToList()
                : new List<int>();
            Tutor = e.Version == GameVersion.C
                ? Legal.GetTutorMoves(pkm, pkm.Species, pkm.AltForm, false, 2).ToList()
                : new List<int>();

            // Only TM/HM moves from the source game of the egg, not any other games from the same generation
            TMHM = Legal.GetTMHM(pkm, pkm.Species, pkm.AltForm, pkm.GenNumber, e.Version, false).ToList();

            // Non-Base moves that can magically appear in the regular movepool
            bool volt = notSpecial && (pkm.GenNumber > 3 || e.Version == GameVersion.E) && Legal.LightBall.Contains(pkm.Species);
            if (volt)
                Egg.Add(344); // Volt Tackle
        }

        public bool AllowInherited { get; }
        public IList<int> Base { get; }
        public List<int> Special { get; }
        public IList<int> Egg { get; }
        public List<int> Tutor { get; }
        public List<int> TMHM { get; }
        public List<int> LevelUp { get; }

        public bool IsInherited(int m) => !Base.Contains(m) || Special.Contains(m) ||
                                          Egg.Contains(m) || LevelUp.Contains(m) ||
                                          TMHM.Contains(m) || Tutor.Contains(m);
    }
}
