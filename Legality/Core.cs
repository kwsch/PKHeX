using System.Collections.Generic;
using System.Linq;

namespace PKHeX
{
    public static partial class Legal
    {
        internal static WC6[] WC6DB;
        // PKHeX master personal.dat
        internal static readonly PersonalInfo[] PersonalAO = PersonalInfo.getPersonalArray(Properties.Resources.personal_ao, PersonalInfo.SizeAO);

        private static readonly PersonalInfo[] PersonalXY = PersonalInfo.getPersonalArray(Properties.Resources.personal_xy, PersonalInfo.SizeXY);
        private static readonly EggMoves[] EggMoveXY = EggMoves.getArray(Util.unpackMini(Properties.Resources.eggmove_xy, "xy"));
        private static readonly Learnset[] LevelUpXY = Learnset.getArray(Util.unpackMini(Properties.Resources.lvlmove_xy, "xy"));
        private static readonly EggMoves[] EggMoveAO = EggMoves.getArray(Util.unpackMini(Properties.Resources.eggmove_ao, "ao"));
        private static readonly Learnset[] LevelUpAO = Learnset.getArray(Util.unpackMini(Properties.Resources.lvlmove_ao, "ao"));
        private static readonly Evolutions[] Evolves = Evolutions.getArray(Util.unpackMini(Properties.Resources.evos_ao, "ao"));
        private static readonly EncounterArea[] SlotsA = EncounterArea.getArray(Util.unpackMini(Properties.Resources.encounter_a, "ao"));
        private static readonly EncounterArea[] SlotsO = EncounterArea.getArray(Util.unpackMini(Properties.Resources.encounter_o, "ao"));
        private static readonly EncounterArea[] SlotsX = EncounterArea.getArray(Util.unpackMini(Properties.Resources.encounter_x, "xy"));
        private static readonly EncounterArea[] SlotsY = EncounterArea.getArray(Util.unpackMini(Properties.Resources.encounter_y, "xy"));
        private static readonly EncounterArea[] DexNavA = getDexNavSlots(SlotsA);
        private static readonly EncounterArea[] DexNavO = getDexNavSlots(SlotsO);
        private static EncounterArea[] getDexNavSlots(EncounterArea[] GameSlots)
        {
            EncounterArea[] eA = new EncounterArea[GameSlots.Length];
            for (int i = 0; i < eA.Length; i++)
            {
                eA[i] = GameSlots[i];
                eA[i].Slots = eA[i].Slots.Take(20).Concat(eA[i].Slots.Skip(25)).ToArray(); // Skip 5 Rock Smash slots.
            }
            return eA;
        }

        internal static bool getDexNavValid(PK6 pk6)
        {
            bool alpha = pk6.Version == 26;
            if (!alpha && pk6.Version != 27)
                return false;
            EncounterArea[] locs = (alpha ? DexNavA : DexNavO).Where(l => l.Location == pk6.Met_Location).ToArray();
            return locs.Select(loc => getValidEncounterSlots(pk6, loc)).Any(slots => slots.Length > 0);
        }

        internal static EncounterSlot[] getValidEncounterSlots(PK6 pk6, EncounterArea loc)
        {
            // Get Valid levels
            DexLevel[] vs = getValidPreEvolutions(pk6);
            // Get slots where pokemon can exist
            EncounterSlot[] slots = loc.Slots.Where(slot => vs.Any(evo => evo.Species == slot.Species && evo.Level >= slot.LevelMin)).ToArray();

            // Filter for Form Specific
            if (WildForms.Contains(pk6.Species))
                slots = slots.Where(slot => slot.Form == pk6.AltForm).ToArray();
            return slots;
        }
        internal static DexLevel[] getValidPreEvolutions(PK6 pk6)
        {
            var evos = Evolves[pk6.Species].Evos;
            int dec = 0;
            List<DexLevel> dl = new List<DexLevel> {new DexLevel {Species = pk6.Species, Level = pk6.CurrentLevel}};
            foreach (DexLevel evo in evos)
            {
                if (evo.Level == 1) // Level Up (from previous level)
                    dec++;
                int lvl = pk6.CurrentLevel - dec;
                if (lvl >= pk6.Met_Level && lvl > evo.Level)
                    dl.Add(new DexLevel {Species = evo.Species, Level = lvl});
            }
            return dl.ToArray();
        }

        internal static int[] getValidMoves(int species, int level)
        {
            int[] r = new int[1];

            // r = r.Concat(getEggMoves(species)).ToArray();
            r = r.Concat(getLVLMoves(species, level)).ToArray();
            r = r.Concat(getTutorMoves(species)).ToArray();
            r = r.Concat(getMachineMoves(species)).ToArray();
            var e = Evolves[species].Evos;
            int dec = 0;
            foreach (DexLevel evo in e)
            {
                if (evo.Level == 1) // In order to level up evolve, the list of available moves is for one level previous.
                    dec++;
                r = r.Concat(getLVLMoves(evo.Species, level - dec)).ToArray();
                r = r.Concat(getTutorMoves(evo.Species)).ToArray();
                r = r.Concat(getMachineMoves(evo.Species)).ToArray();
            }
            return r.Distinct().ToArray();
        }

        internal static int getBaseSpecies(PK6 pk6, int skipOption)
        {
            DexLevel[] evos = Evolves[pk6.Species].Evos;
            switch (skipOption)
            {
                case -1: return pk6.Species;
                case 1: return evos.Length <= 1 ? pk6.Species : evos[evos.Length - 1].Species;
                default: return evos.Length <= 0 ? pk6.Species : evos.Last().Species;
            }
        }

        internal static int[] getDexNavRelearn(PK6 pk6, int skipOption)
        {
            int species = getBaseSpecies(pk6, skipOption);

            var moves = new int[1];
            moves = moves.Concat(getEggMoves(species)).ToArray();
            return moves.Distinct().ToArray();
        }
        internal static int[] getValidRelearn(PK6 pk6, int skipOption)
        {
            int[] moves = new int[1];
            int species = getBaseSpecies(pk6, skipOption);
            moves = moves.Concat(getLVLMoves(species, 1)).ToArray();
            moves = moves.Concat(getEggMoves(species)).ToArray();
            moves = moves.Concat(getLVLMoves(species, 100)).ToArray();
            return moves.Distinct().ToArray();
        }

        private static int[] getEggMoves(int species)
        {
            return EggMoveAO[species].Moves.Concat(EggMoveXY[species].Moves).ToArray();
        }
        private static int[] getLVLMoves(int species, int lvl)
        {
            return LevelUpXY[species].getMoves(lvl).Concat(LevelUpAO[species].getMoves(lvl)).ToArray();
        }
        private static int[] getTutorMoves(int species)
        {
            PersonalInfo pkAO = PersonalAO[species];

            // Type Tutor
            List<int> moves = TypeTutor.Where((t, i) => pkAO.Tutors[i]).ToList();

            // Varied Tutors
            for (int i = 0; i < Tutors_AO.Length; i++)
                for (int b = 0; b < Tutors_AO[i].Length; b++)
                    if (pkAO.ORASTutors[i][b])
                        moves.Add(Tutors_AO[i][b]);

            return moves.ToArray();
        }
        private static int[] getMachineMoves(int species)
        {
            PersonalInfo pkXY = PersonalXY[species];
            PersonalInfo pkAO = PersonalAO[species];
            List<int> moves = new List<int>();
            moves.AddRange(TMHM_XY.Where((t, i) => pkXY.TMHM[i]));
            moves.AddRange(TMHM_AO.Where((t, i) => pkAO.TMHM[i]));
            return moves.ToArray();
        }
    }
}
