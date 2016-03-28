using System.Collections.Generic;
using System.Linq;

namespace PKHeX
{
    public static partial class Legal
    {
        // PKHeX master Wonder Card Database
        internal static WC6[] WC6DB;
        // PKHeX master personal.dat
        internal static readonly PersonalInfo[] PersonalAO = PersonalInfo.getPersonalArray(Properties.Resources.personal_ao, PersonalInfo.SizeAO);
        private static readonly PersonalInfo[] PersonalXY = PersonalInfo.getPersonalArray(Properties.Resources.personal_xy, PersonalInfo.SizeXY);
        private static readonly EggMoves[] EggMoveXY = EggMoves.getArray(Util.unpackMini(Properties.Resources.eggmove_xy, "xy"));
        private static readonly Learnset[] LevelUpXY = Learnset.getArray(Util.unpackMini(Properties.Resources.lvlmove_xy, "xy"));
        private static readonly EggMoves[] EggMoveAO = EggMoves.getArray(Util.unpackMini(Properties.Resources.eggmove_ao, "ao"));
        private static readonly Learnset[] LevelUpAO = Learnset.getArray(Util.unpackMini(Properties.Resources.lvlmove_ao, "ao"));
        private static readonly Evolutions[] Evolves = Evolutions.getArray(Util.unpackMini(Properties.Resources.evos_ao, "ao"));
        private static readonly EncounterArea[] SlotsA;
        private static readonly EncounterArea[] SlotsO;
        private static readonly EncounterArea[] SlotsX;
        private static readonly EncounterArea[] SlotsY;
        private static readonly EncounterArea[] DexNavA;
        private static readonly EncounterArea[] DexNavO;
        private static readonly EncounterStatic[] StaticX;
        private static readonly EncounterStatic[] StaticY;
        private static readonly EncounterStatic[] StaticA;
        private static readonly EncounterStatic[] StaticO;
        private static EncounterStatic[] getSpecial(GameVersion Game)
        {
            if (Game == GameVersion.X || Game == GameVersion.Y)
                return Encounter_XY.Where(s => s.Version == GameVersion.Any || s.Version == Game).ToArray();
            // else if (Game == GameVersion.AS || Game == GameVersion.OR)
            return Encounter_AO.Where(s => s.Version == GameVersion.Any || s.Version == Game).ToArray();
        }
        private static EncounterArea[] addXYAltTiles(EncounterArea[] GameSlots, EncounterArea[] SpecialSlots)
        {
            foreach (EncounterArea g in GameSlots)
            {
                EncounterArea slots = SpecialSlots.FirstOrDefault(l => l.Location == g.Location);
                if (slots != null)
                    g.Slots = g.Slots.Concat(slots.Slots).ToArray();
            }
            return GameSlots;
        }
        private static EncounterArea[] getDexNavSlots(EncounterArea[] GameSlots)
        {
            EncounterArea[] eA = new EncounterArea[GameSlots.Length];
            for (int i = 0; i < eA.Length; i++)
            {
                eA[i] = new EncounterArea { Location = GameSlots[i].Location, Slots = GameSlots[i].Slots };
                eA[i].Slots = eA[i].Slots.Where(s => s.Type != SlotType.Rock_Smash).ToArray(); // Skip Rock Smash slots.
                foreach (var slot in eA[i].Slots)
                    slot.DexNav = true;
            }
            return eA;
        }

        static Legal() // Setup
        {
            StaticX = getSpecial(GameVersion.X);
            StaticY = getSpecial(GameVersion.Y);
            StaticA = getSpecial(GameVersion.AS);
            StaticO = getSpecial(GameVersion.OR);

            var XSlots = EncounterArea.getArray(Util.unpackMini(Properties.Resources.encounter_x, "xy"));
            var YSlots = EncounterArea.getArray(Util.unpackMini(Properties.Resources.encounter_y, "xy"));

            // Mark Horde Encounters
            foreach (var area in XSlots)
            {
                int slotct = area.Slots.Length;
                for (int i = slotct - 15; i < slotct; i++)
                    area.Slots[i].Type = SlotType.Horde;
            }
            foreach (var area in YSlots)
            {
                int slotct = area.Slots.Length;
                for (int i = slotct - 15; i < slotct; i++)
                    area.Slots[i].Type = SlotType.Horde;
            }
            SlotsX = addXYAltTiles(XSlots, SlotsXYAlt);
            SlotsY = addXYAltTiles(YSlots, SlotsXYAlt);

            SlotsA = EncounterArea.getArray(Util.unpackMini(Properties.Resources.encounter_a, "ao"));
            SlotsO = EncounterArea.getArray(Util.unpackMini(Properties.Resources.encounter_o, "ao"));

            // Mark Rock Smash Encounters
            foreach (var area in SlotsA)
            {
                for (int i = 32; i < 37; i++)
                    area.Slots[i].Type = SlotType.Rock_Smash;
                int slotct = area.Slots.Length;
                for (int i = slotct - 15; i < slotct; i++)
                    area.Slots[i].Type = SlotType.Horde;
            }
            foreach (var area in SlotsO)
            {
                for (int i = 32; i < 37; i++)
                    area.Slots[i].Type = SlotType.Rock_Smash;
                int slotct = area.Slots.Length;
                for (int i = slotct - 15; i < slotct; i++)
                    area.Slots[i].Type = SlotType.Horde;
            }

            DexNavA = getDexNavSlots(SlotsA);
            DexNavO = getDexNavSlots(SlotsO);
        }

        internal static IEnumerable<int> getValidMoves(PK6 pk6)
        {
            List<int> r = new List<int> {0};
            int species = pk6.Species;
            if (FormChangeMoves.Contains(species)) // Deoxys & Shaymin & Giratina (others don't have extra but whatever)
            {
                int formcount = PersonalAO[species].FormeCount;
                for (int i = 0; i < formcount; i++)
                {
                    // Check all Forms
                    r.AddRange(getLVLMoves(species, pk6.CurrentLevel, i));
                    r.AddRange(getTutorMoves(species, i));
                    r.AddRange(getMachineMoves(species, i));
                }
                return r.Distinct().ToArray();
            }
            r.AddRange(getLVLMoves(species, pk6.CurrentLevel, pk6.AltForm));
            r.AddRange(getTutorMoves(species, pk6.AltForm));
            r.AddRange(getMachineMoves(species, pk6.AltForm));
            IEnumerable<DexLevel> vs = getValidPreEvolutions(pk6);
            foreach (DexLevel evo in vs)
            {
                r.AddRange(getLVLMoves(evo.Species, evo.Level, pk6.AltForm));
                r.AddRange(getTutorMoves(evo.Species, pk6.AltForm));
                r.AddRange(getMachineMoves(evo.Species, pk6.AltForm));
            }
            if (species == 479) // Rotom
                r.Add(RotomMoves[pk6.AltForm]);
            if (species == 25) // Pikachu
                r.Add(PikachuMoves[pk6.AltForm]);

            return r.Distinct().ToArray();
        }
        internal static IEnumerable<int> getValidRelearn(PK6 pk6, int skipOption)
        {
            List<int> r = new List<int> { 0 };
            int species = getBaseSpecies(pk6, skipOption);
            r.AddRange(getLVLMoves(species, 1, pk6.AltForm));
            r.AddRange(getEggMoves(species, pk6.Species == 678 ? pk6.AltForm : 0));
            r.AddRange(getLVLMoves(species, 100, pk6.AltForm));
            return r.Distinct();
        }
        internal static IEnumerable<int> getBaseEggMoves(PK6 pk6, int skipOption, int gameSource)
        {
            int species = getBaseSpecies(pk6, skipOption);
            if (gameSource == -1)
            {
                if (pk6.XY)
                    return LevelUpXY[species].getMoves(1);
                // if (pk6.Version == 26 || pk6.Version == 27)
                return LevelUpAO[species].getMoves(1);
            }
            if (gameSource == 0) // XY
                return LevelUpXY[species].getMoves(1);
            // if (gameSource == 1) // ORAS
            return LevelUpAO[species].getMoves(1);
        }

        internal static IEnumerable<WC6> getValidWC6s(PK6 pk6)
        {
            var vs = getValidPreEvolutions(pk6).ToArray();
            List<WC6> validWC6 = new List<WC6>();

            foreach (WC6 wc in WC6DB.Where(wc => vs.Any(dl => dl.Species == wc.Species)))
            {
                if (pk6.Egg_Location == 0) // Not Egg
                {
                    if (wc.CardID != pk6.SID) continue;
                    if (wc.TID != pk6.TID) continue;
                    if (wc.OT != pk6.OT_Name) continue;
                    if (wc.OTGender != pk6.OT_Gender) continue;
                    if (wc.PIDType == 0 && pk6.PID != wc.PID) continue;
                    if (wc.PIDType == 2 && !pk6.IsShiny) continue;
                    if (wc.PIDType == 3 && pk6.IsShiny) continue;
                    if (wc.OriginGame != 0 && wc.OriginGame != pk6.Version) continue;
                    if (wc.EncryptionConstant != 0 && wc.EncryptionConstant != pk6.EncryptionConstant) continue;
                    if (wc.Language != 0 && wc.Language != pk6.Language) continue;
                }
                if (wc.Form != pk6.AltForm && vs.All(dl => !FormChange.Contains(dl.Species))) continue;
                if (wc.MetLocation != pk6.Met_Location) continue;
                if (wc.EggLocation != pk6.Egg_Location) continue;
                if (wc.Level != pk6.Met_Level) continue;
                if (wc.Pokéball != pk6.Ball) continue;
                if (wc.OTGender < 3 && wc.OTGender != pk6.OT_Gender) continue;
                if (wc.Nature != 0xFF && wc.Nature != pk6.Nature) continue;
                if (wc.Gender != 3 && wc.Gender != pk6.Gender) continue;

                if (wc.CNT_Cool > pk6.CNT_Cool) continue;
                if (wc.CNT_Beauty > pk6.CNT_Beauty) continue;
                if (wc.CNT_Cute > pk6.CNT_Cute) continue;
                if (wc.CNT_Smart > pk6.CNT_Smart) continue;
                if (wc.CNT_Tough > pk6.CNT_Tough) continue;
                if (wc.CNT_Sheen > pk6.CNT_Sheen) continue;

                // Some checks are best performed separately as they are caused by users screwing up valid data.
                // if (!wc.RelearnMoves.SequenceEqual(pk6.RelearnMoves)) continue; // Defer to relearn legality
                // if (wc.OT.Length > 0 && pk6.CurrentHandler != 1) continue; // Defer to ownership legality
                // if (wc.OT.Length > 0 && pk6.OT_Friendship != PKX.getBaseFriendship(pk6.Species)) continue; // Friendship
                // if (wc.Level > pk6.CurrentLevel) continue; // Defer to level legality
                // RIBBONS: Defer to ribbon legality

                validWC6.Add(wc);
            }
            return validWC6;
        }
        internal static EncounterLink getValidLinkGifts(PK6 pk6)
        {
            return LinkGifts.FirstOrDefault(g => g.Species == pk6.Species && g.Level == pk6.Met_Level);
        }
        internal static EncounterSlot[] getValidWildEncounters(PK6 pk6)
        {
            List<EncounterSlot> s = new List<EncounterSlot>();

            foreach (var area in getEncounterAreas(pk6))
                s.AddRange(getValidEncounterSlots(pk6, area, DexNav: false));
            if (s.Any()) return s.ToArray();
            if (!pk6.AO) return null;

            // By default, no slots are currently stored. Continue!
            foreach (var area in getDexNavAreas(pk6))
                s.AddRange(getValidEncounterSlots(pk6, area, DexNav: true));
            return s.Any() ? s.ToArray() : null;
        }
        internal static EncounterStatic getValidStaticEncounter(PK6 pk6)
        {
            // Get possible encounters
            IEnumerable<EncounterStatic> poss = getStaticEncounters(pk6);
            // Back Check against pk6
            foreach (EncounterStatic e in poss)
            {
                if (e.Nature != Nature.Random && pk6.Nature != (int)e.Nature)
                    continue;
                if (e.EggLocation != pk6.Egg_Location)
                    continue;
                if (e.Location != 0 && e.Location != pk6.Met_Location)
                    continue;
                if (e.Gender != -1 && e.Gender != pk6.Gender)
                    continue;
                if (e.Level != pk6.Met_Level)
                    continue;
                if (e.Shiny != null && e.Shiny != pk6.IsShiny)
                    continue;
                if (e.Gift && pk6.Ball != 4) // PokéBall
                    continue;

                // Passes all checks, valid encounter
                return e;
            }
            return null;
        }
        internal static EncounterTrade getValidIngameTrade(PK6 pk6)
        {
            if (!pk6.WasIngameTrade)
                return null;
            int lang = pk6.Language;
            if (lang == 0)
                return null;

            // Get valid pre-evolutions
            IEnumerable<DexLevel> p = getValidPreEvolutions(pk6);
            EncounterTrade z = null;
            if (pk6.XY)
                z = lang == 6 ? null : TradeGift_XY.FirstOrDefault(f => p.Any(r => r.Species == f.Species));
            if (pk6.AO)
                z = lang == 6 ? null : TradeGift_AO.FirstOrDefault(f => p.Any(r => r.Species == f.Species));

            if (z == null)
                return null;

            for (int i = 0; i < 6; i++)
                if (z.IVs[i] != -1 && z.IVs[i] != pk6.IVs[i])
                    return null;

            if (z.Shiny ^ pk6.IsShiny) // Are PIDs static?
                return null;
            if (z.TID != pk6.TID)
                return null;
            if (z.SID != pk6.SID)
                return null;
            if (z.Location != pk6.Met_Location)
                return null;
            if (z.Level != pk6.Met_Level)
                return null;
            if (z.Nature != Nature.Random && (int)z.Nature != pk6.Nature)
                return null;
            if (z.Gender != pk6.Gender)
                return null;
            // if (z.Ability == 4 ^ pk6.AbilityNumber == 4) // defer to Ability 
            //    return null;

            return z;
        }
        internal static EncounterSlot[] getValidFriendSafari(PK6 pk6)
        {
            if (!pk6.XY)
                return null;
            if (pk6.Met_Location != 148) // Friend Safari
                return null;
            if (pk6.Met_Level != 30)
                return null;

            IEnumerable<DexLevel> vs = getValidPreEvolutions(pk6);
            List<EncounterSlot> slots = new List<EncounterSlot>();
            foreach (DexLevel d in vs.Where(d => FriendSafari.Contains(d.Species) && d.Level >= 30))
            {
                var slot = new EncounterSlot
                {
                    Species = d.Species,
                    LevelMin = 30,
                    LevelMax = 30,
                    Form = 0,
                    Type = SlotType.FriendSafari,
                };
                slots.Add(slot);
            }

            return slots.Any() ? slots.ToArray() : null;
        }

        internal static bool getDexNavValid(PK6 pk6)
        {
            IEnumerable<EncounterArea> locs = getDexNavAreas(pk6);
            return locs.Select(loc => getValidEncounterSlots(pk6, loc, DexNav: true)).Any(slots => slots.Any());
        }
        internal static bool getHasEvolved(PK6 pk6)
        {
            return getValidPreEvolutions(pk6).Count() > 1;
        }
        internal static bool getHasTradeEvolved(PK6 pk6)
        {
            return Evolves[pk6.Species].Evos.Any(evo => evo.Level == 1); // 1: Trade, 0: Item, >=2: Levelup
        }
        internal static bool getIsFossil(PK6 pk6)
        {
            if (pk6.Met_Level != 20)
                return false;
            if (pk6.Egg_Location != 0)
                return false;
            if (pk6.XY && pk6.Met_Location == 44)
                return Fossils.Contains(getBaseSpecies(pk6));
            if (pk6.AO && pk6.Met_Location == 190)
                return Fossils.Contains(getBaseSpecies(pk6));

            return false;
        }
        internal static IEnumerable<int> getLineage(PK6 pk6)
        {
            List<int> res = new List<int>();
            for (int i = -1; i < 2; i++)
                res.Add(getBaseSpecies(pk6, i));
            return res.Distinct();
        }

        private static int getBaseSpecies(PK6 pk6, int skipOption = 0)
        {
            if (pk6.Species == 292)
                return 290;
            DexLevel[] evos = Evolves[pk6.Species].Evos;
            switch (skipOption)
            {
                case -1: return pk6.Species;
                case 1: return evos.Length <= 1 ? pk6.Species : evos[evos.Length - 2].Species;
                default: return evos.Length <= 0 ? pk6.Species : evos.Last().Species;
            }
        }
        private static IEnumerable<EncounterArea> getDexNavAreas(PK6 pk6)
        {
            bool alpha = pk6.Version == 26;
            if (!alpha && pk6.Version != 27)
                return new EncounterArea[0];
            return (alpha ? DexNavA : DexNavO).Where(l => l.Location == pk6.Met_Location);
        }
        private static IEnumerable<int> getLVLMoves(int species, int lvl, int formnum)
        {
            int ind_XY = PersonalXY[species].FormeIndex(species, formnum);
            int ind_AO = PersonalAO[species].FormeIndex(species, formnum);
            return LevelUpXY[ind_XY].getMoves(lvl).Concat(LevelUpAO[ind_AO].getMoves(lvl));
        }
        private static IEnumerable<EncounterArea> getEncounterSlots(PK6 pk6)
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
        private static IEnumerable<EncounterStatic> getStaticEncounters(PK6 pk6)
        {
            switch (pk6.Version)
            {
                case 24: // X
                    return getStatic(pk6, StaticX);
                case 25: // Y
                    return getStatic(pk6, StaticY);
                case 26: // AS
                    return getStatic(pk6, StaticA);
                case 27: // OR
                    return getStatic(pk6, StaticO);
                default: return new List<EncounterStatic>();
            }
        }
        private static IEnumerable<EncounterArea> getEncounterAreas(PK6 pk6)
        {
            return getEncounterSlots(pk6).Where(l => l.Location == pk6.Met_Location);
        }
        private static IEnumerable<EncounterSlot> getValidEncounterSlots(PK6 pk6, EncounterArea loc, bool DexNav)
        {
            // Get Valid levels
            IEnumerable<DexLevel> vs = getValidPreEvolutions(pk6);
            // Get slots where pokemon can exist
            IEnumerable<EncounterSlot> slots = loc.Slots.Where(slot => vs.Any(evo => evo.Species == slot.Species && evo.Level >= slot.LevelMin));

            // Filter for Met Level
            int lvl = pk6.Met_Level;
            slots = DexNav
                ? slots.Where(slot => slot.LevelMin <= lvl && lvl <= slot.LevelMax + 30) // DexNav Boost Range ??
                : slots.Where(slot => slot.LevelMin <= lvl && lvl <= slot.LevelMax); // Non-boosted Level

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
            int lvl = pk6.CurrentLevel;
            if (pk6.Species == 292 && pk6.Met_Level + 1 <= lvl && lvl >= 20)
                return new List<DexLevel>
                {
                    new DexLevel { Species = 292, Level = lvl },
                    new DexLevel { Species = 290, Level = lvl-1 }
                };
            var evos = Evolves[pk6.Species].Evos;
            List<DexLevel> dl = new List<DexLevel> { new DexLevel { Species = pk6.Species, Level = lvl } };
            foreach (DexLevel evo in evos)
            {
                if (lvl >= pk6.Met_Level && lvl >= evo.Level)
                    dl.Add(new DexLevel {Species = evo.Species, Level = lvl});
                else break;
                if (evo.Level > 1) // Level Up (from previous level)
                    lvl--;
            }
            return dl;
        }
        private static IEnumerable<EncounterStatic> getStatic(PK6 pk6, IEnumerable<EncounterStatic> table)
        {
            IEnumerable<DexLevel> dl = getValidPreEvolutions(pk6);
            return table.Where(e => dl.Any(d => d.Species == e.Species));
        }
        private static IEnumerable<int> getEggMoves(int species, int formnum)
        {
            int ind_XY = PersonalXY[species].FormeIndex(species, formnum);
            int ind_AO = PersonalAO[species].FormeIndex(species, formnum);
            return EggMoveAO[ind_AO].Moves.Concat(EggMoveXY[ind_XY].Moves);
        }
        private static IEnumerable<int> getTutorMoves(int species, int formnum)
        {
            PersonalInfo pkAO = PersonalAO[PersonalAO[species].FormeIndex(species, formnum)];

            // Type Tutor
            List<int> moves = TypeTutor.Where((t, i) => pkAO.Tutors[i]).ToList();

            // Varied Tutors
            for (int i = 0; i < Tutors_AO.Length; i++)
                for (int b = 0; b < Tutors_AO[i].Length; b++)
                    if (pkAO.ORASTutors[i][b])
                        moves.Add(Tutors_AO[i][b]);

            // Keldeo - Secret Sword
            if (species == 647)
                moves.Add(548);
            return moves;
        }
        private static IEnumerable<int> getMachineMoves(int species, int formnum)
        {
            PersonalInfo pkXY = PersonalXY[PersonalXY[species].FormeIndex(species, formnum)];
            PersonalInfo pkAO = PersonalAO[PersonalAO[species].FormeIndex(species, formnum)];
            List<int> moves = new List<int>();
            moves.AddRange(TMHM_XY.Where((t, i) => pkXY.TMHM[i]));
            moves.AddRange(TMHM_AO.Where((t, i) => pkAO.TMHM[i]));
            return moves;
        }
    }
}
