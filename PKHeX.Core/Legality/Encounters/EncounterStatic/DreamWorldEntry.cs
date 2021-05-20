using System.Collections.Generic;
using System.Linq;

namespace PKHeX.Core
{
    /// <summary>
    /// Intermediary Representation of Dream World Data
    /// </summary>
    internal record DreamWorldEntry(int Species, int Level, int Move1 = 0, int Move2 = 0, int Move3 = 0, int Form = 0, int Gender = -1)
    {
        private int EntryCount => Move1 == 0 ? 1 : Move2 == 0 ? 1 : Move3 == 0 ? 2 : 3;

        private void AddTo(GameVersion game, EncounterStatic5[] result, ref int ctr)
        {
            var p = (PersonalInfoBW)PersonalTable.B2W2[Species];
            var a = p.HasHiddenAbility ? 4 : 1;
            if (Move1 == 0)
            {
                result[ctr++] = new EncounterStatic5(game)
                {
                    Species = Species,
                    Form = Form,
                    Gender = Gender,
                    Level = Level,
                    Ability = a,
                    Location = 075,
                    Shiny = Shiny.Never,
                };
                return;
            }
            result[ctr++] = Create(game, a, Move1);
            if (Move2 == 0)
                return;
            result[ctr++] = Create(game, a, Move2);
            if (Move3 == 0)
                return;
            result[ctr++] = Create(game, a, Move3);
        }

        private EncounterStatic5 Create(GameVersion game, int ability, int move)
        {
            return new(game)
            {
                Species = Species,
                Form = Form,
                Level = Level,
                Ability = ability,
                Location = 075,
                Shiny = Shiny.Never,
                Moves = new[] { move }
            };
        }

        public static EncounterStatic5[] GetArray(GameVersion game, IReadOnlyList<DreamWorldEntry> t)
        {
            // Split encounters with multiple permitted special moves -- a pkm can only be obtained with 1 of the special moves!
            var count = t.Sum(z => z.EntryCount);
            var result = new EncounterStatic5[count];

            int ctr = 0;
            foreach (var s in t)
                s.AddTo(game, result, ref ctr);
            return result;
        }
    }
}
