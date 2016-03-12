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
            IEnumerable<EncounterArea> locs = (alpha ? DexNavA : DexNavO).Where(l => l.Location == pk6.Met_Location);
            return locs.Select(loc => getValidEncounterSlots(pk6, loc, DexNav: true)).Any(slots => slots.Any());
        }
        internal static int[] getValidMoves(PK6 pk6)
        {
            List<int> r = new List<int> {0};
            
            r.AddRange(getLVLMoves(pk6.Species, pk6.CurrentLevel));
            r.AddRange(getTutorMoves(pk6.Species));
            r.AddRange(getMachineMoves(pk6.Species));
            IEnumerable<DexLevel> vs = getValidPreEvolutions(pk6);
            foreach (DexLevel evo in vs)
            {
                r.AddRange(getLVLMoves(evo.Species, evo.Level));
                r.AddRange(getTutorMoves(evo.Species));
                r.AddRange(getMachineMoves(evo.Species));
            }
            return r.Distinct().ToArray();
        }
        internal static int[] getValidRelearn(PK6 pk6, int skipOption)
        {
            List<int> r = new List<int> { 0 };
            int species = getBaseSpecies(pk6, skipOption);
            r.AddRange(getLVLMoves(species, 1));
            r.AddRange(getEggMoves(species));
            r.AddRange(getLVLMoves(species, 100));
            return r.Distinct().ToArray();
        }
        internal static int[] getLinkMoves(PK6 pk6)
        {
            if (pk6.Species == 251 && pk6.Met_Level == 10) // Celebi
                return new[] {610, 0, 0, 0};
            if (pk6.Species == 154 && pk6.Met_Level == 50 && pk6.AbilityNumber == 4) // Meganium Hidden
                return new[] {0, 0, 0, 0};
            if (pk6.Species == 157 && pk6.Met_Level == 50 && pk6.AbilityNumber == 4) // Typhlosion Hidden
                return new[] {0, 0, 0, 0};
            if (pk6.Species == 160 && pk6.Met_Level == 50 && pk6.AbilityNumber == 4) // Feraligatr Hidden
                return new[] {0, 0, 0, 0};
            if (pk6.Species == 377 && pk6.Met_Level == 50 && pk6.AbilityNumber == 4) // Regirock Hidden
                return new[] {153, 8, 444, 359};
            if (pk6.Species == 378 && pk6.Met_Level == 50 && pk6.AbilityNumber == 4) // Regice Hidden
                return new[] {85, 133, 58, 258};
            if (pk6.Species == 379 && pk6.Met_Level == 50 && pk6.AbilityNumber == 4) // Registeel Hidden
                return new[] {442, 157, 356, 334};
            return new int[0];
        }
        internal static IEnumerable<WC6> getValidWC6s(PK6 pk6)
        {
            IEnumerable<DexLevel> vs = getValidPreEvolutions(pk6);
            if (pk6.Egg_Location > 0) // Was Egg, can't search TID/SID/OT
                return WC6DB.Where(wc6 => vs.Any(dl => dl.Species == wc6.Species));

            // Not Egg
            return WC6DB.Where(wc6 =>
                wc6.CardID == pk6.SID &&
                wc6.TID == pk6.TID &&
                vs.Any(dl => dl.Species == wc6.Species) &&
                wc6.OT == pk6.OT_Name);
        }
        internal static IEnumerable<EncounterArea> getEncounterSlots(PK6 pk6)
        {
            switch (pk6.Version)
            {
                case 24: // X
                    return getSlots(pk6, SlotsX);
                case 25: // Y
                    return getSlots(pk6, SlotsY);
                case 26: // AS
                    return getSlots(pk6, SlotsA);
                case 27: // OR
                    return getSlots(pk6, SlotsO);
                default: return new List<EncounterArea>();
            }
        }
        
        private static int getBaseSpecies(PK6 pk6, int skipOption)
        {
            DexLevel[] evos = Evolves[pk6.Species].Evos;
            switch (skipOption)
            {
                case -1: return pk6.Species;
                case 1: return evos.Length <= 1 ? pk6.Species : evos[evos.Length - 1].Species;
                default: return evos.Length <= 0 ? pk6.Species : evos.Last().Species;
            }
        }

        internal static bool getWildEncounterValid(PK6 pk6)
        {
            var areas = getEncounterAreas(pk6);
            bool dexNav = pk6.RelearnMove1 != 0;
            return areas.Any(a => getValidEncounterSlots(pk6, a, dexNav).Any());
        }
        internal static IEnumerable<EncounterArea> getEncounterAreas(PK6 pk6)
        {
            return getEncounterSlots(pk6).Where(l => l.Location == pk6.Met_Location);
        }
        internal static IEnumerable<EncounterSlot> getValidEncounterSlots(PK6 pk6, EncounterArea loc, bool DexNav)
        {
            // Get Valid levels
            IEnumerable<DexLevel> vs = getValidPreEvolutions(pk6);
            // Get slots where pokemon can exist
            IEnumerable<EncounterSlot> slots = loc.Slots.Where(slot => vs.Any(evo => evo.Species == slot.Species && evo.Level >= slot.LevelMin));

            // Filter for Met Level
            slots = DexNav
                ? slots.Where(slot => slot.LevelMin <= pk6.Met_Level && pk6.Met_Level <= slot.LevelMin + 10) // DexNav Boost Range
                : slots.Where(slot => slot.LevelMin == pk6.Met_Level); // Non-boosted Level

            // Filter for Form Specific
            if (WildForms.Contains(pk6.Species))
                slots = slots.Where(slot => slot.Form == pk6.AltForm);
            return slots;
        }
        private static IEnumerable<EncounterArea> getSlots(PK6 pk6, IEnumerable<EncounterArea> tables)
        {
            int species = pk6.Species;
            IEnumerable<DexLevel> vs = getValidPreEvolutions(pk6);
            List<EncounterArea> slotLocations = new List<EncounterArea>();
            foreach (var loc in tables)
            {
                IEnumerable<EncounterSlot> slots = loc.Slots.Where(slot => vs.Any(evo => evo.Species == slot.Species && evo.Level >= slot.LevelMin));
                if (WildForms.Contains(species))
                    slots = slots.Where(slot => slot.Form == pk6.AltForm);

                EncounterSlot[] es = slots.ToArray();
                if (es.Length > 0)
                    slotLocations.Add(new EncounterArea { Location = loc.Location, Slots = es });
            }
            return slotLocations;
        }
        private static IEnumerable<DexLevel> getValidPreEvolutions(PK6 pk6)
        {
            var evos = Evolves[pk6.Species].Evos;
            int dec = 0;
            List<DexLevel> dl = new List<DexLevel> { new DexLevel { Species = pk6.Species, Level = pk6.CurrentLevel } };
            foreach (DexLevel evo in evos)
            {
                if (evo.Level > 0) // Level Up (from previous level)
                    dec++;
                int lvl = pk6.CurrentLevel - dec;
                if (lvl >= pk6.Met_Level && lvl > evo.Level)
                    dl.Add(new DexLevel { Species = evo.Species, Level = lvl });
            }
            return dl;
        }
        private static IEnumerable<int> getEggMoves(int species)
        {
            return EggMoveAO[species].Moves.Concat(EggMoveXY[species].Moves);
        }
        private static IEnumerable<int> getLVLMoves(int species, int lvl)
        {
            return LevelUpXY[species].getMoves(lvl).Concat(LevelUpAO[species].getMoves(lvl));
        }
        private static IEnumerable<int> getTutorMoves(int species)
        {
            PersonalInfo pkAO = PersonalAO[species];

            // Type Tutor
            List<int> moves = TypeTutor.Where((t, i) => pkAO.Tutors[i]).ToList();

            // Varied Tutors
            for (int i = 0; i < Tutors_AO.Length; i++)
                for (int b = 0; b < Tutors_AO[i].Length; b++)
                    if (pkAO.ORASTutors[i][b])
                        moves.Add(Tutors_AO[i][b]);

            return moves;
        }
        private static IEnumerable<int> getMachineMoves(int species)
        {
            PersonalInfo pkXY = PersonalXY[species];
            PersonalInfo pkAO = PersonalAO[species];
            List<int> moves = new List<int>();
            moves.AddRange(TMHM_XY.Where((t, i) => pkXY.TMHM[i]));
            moves.AddRange(TMHM_AO.Where((t, i) => pkAO.TMHM[i]));
            return moves;
        }
    }
}
