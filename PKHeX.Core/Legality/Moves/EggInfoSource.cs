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
            var ratio = pkm.PersonalInfo.Gender; // Genderless/Male Only (except a few) cannot inherit.
            bool AllowLevelUp = ratio > 0 && ratio < 255 || Legal.MixedGenderBreeding.Contains(e.Species);
            Base = Legal.GetBaseEggMoves(pkm, e.Species, e.Game, e.LevelMin).ToList();

            Egg = Legal.GetEggMoves(pkm, e.Species, pkm.AltForm).ToList();
            LevelUp = AllowLevelUp
                ? Legal.GetBaseEggMoves(pkm, e.Species, e.Game, 100).Where(x => !Base.Contains(x)).ToList()
                : new List<int>();
            Tutor = e.Game == GameVersion.C
                ? Legal.GetTutorMoves(pkm, pkm.Species, pkm.AltForm, false, 2).ToList()
                : new List<int>();

            // Only TM/HM moves from the source game of the egg, not any other games from the same generation
            TMHM = Legal.GetTMHM(pkm, pkm.Species, pkm.AltForm, pkm.GenNumber, e.Game, false).ToList();

            // Non-Base moves that can magically appear in the regular movepool
            bool volt = notSpecial && (pkm.GenNumber > 3 || e.Game == GameVersion.E) && Legal.LightBall.Contains(pkm.Species);
            if (volt)
                Egg.Add(344); // Volt Tackle
        }

        public bool AllowInherited { get; }
        public List<int> Base { get; }
        public List<int> Special { get; }
        public List<int> Egg { get; }
        public List<int> Tutor { get; }
        public List<int> TMHM { get; }
        public List<int> LevelUp { get; }

        public bool IsInherited(int m) => !Base.Contains(m) || Special.Contains(m) ||
                                          Egg.Contains(m) || LevelUp.Contains(m) ||
                                          TMHM.Contains(m) || Tutor.Contains(m);
    }
}
