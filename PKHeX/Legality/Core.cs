using System;
using System.Collections.Generic;
using System.Linq;
using PKHeX.Core.Properties;

namespace PKHeX.Core
{
    public static partial class Legal
    {
        /// <summary>Event Database for a given Generation</summary>
        public static MysteryGift[] MGDB_G4, MGDB_G5, MGDB_G6, MGDB_G7 = new MysteryGift[0];

        /// <summary>Setting to specify if an analysis should permit data sourced from the physical cartridge era of GameBoy games.</summary>
        public static bool AllowGBCartEra = false;

        // Gen 1
        private static readonly Learnset[] LevelUpRB = Learnset1.getArray(Resources.lvlmove_rb, MaxSpeciesID_1);
        private static readonly Learnset[] LevelUpY = Learnset1.getArray(Resources.lvlmove_y, MaxSpeciesID_1);
        private static readonly EvolutionTree Evolves1;
        private static readonly EncounterArea[] SlotsRBY;
        private static readonly EncounterStatic[] StaticRBY;

        // Gen 2
        private static readonly EggMoves[] EggMovesGS = EggMoves2.getArray(Resources.eggmove_gs, MaxSpeciesID_2);
        private static readonly Learnset[] LevelUpGS = Learnset1.getArray(Resources.lvlmove_gs, MaxSpeciesID_2);
        private static readonly EggMoves[] EggMovesC = EggMoves2.getArray(Resources.eggmove_c, MaxSpeciesID_2);
        private static readonly Learnset[] LevelUpC = Learnset1.getArray(Resources.lvlmove_c, MaxSpeciesID_2);
        private static readonly EvolutionTree Evolves2;
        private static readonly EncounterArea[] SlotsGSC;
        private static readonly EncounterStatic[] StaticGSC;

        // Gen 6
        private static readonly EggMoves[] EggMovesXY = EggMoves6.getArray(Data.unpackMini(Resources.eggmove_xy, "xy"));
        private static readonly Learnset[] LevelUpXY = Learnset6.getArray(Data.unpackMini(Resources.lvlmove_xy, "xy"));
        private static readonly EggMoves[] EggMovesAO = EggMoves6.getArray(Data.unpackMini(Resources.eggmove_ao, "ao"));
        private static readonly Learnset[] LevelUpAO = Learnset6.getArray(Data.unpackMini(Resources.lvlmove_ao, "ao"));
        private static readonly EvolutionTree Evolves6;
        private static readonly EncounterArea[] SlotsX, SlotsY, SlotsA, SlotsO;
        private static readonly EncounterStatic[] StaticX, StaticY, StaticA, StaticO;

        // Gen 7
        private static readonly EggMoves[] EggMovesSM = EggMoves7.getArray(Data.unpackMini(Resources.eggmove_sm, "sm"));
        private static readonly Learnset[] LevelUpSM = Learnset7.getArray(Data.unpackMini(Resources.lvlmove_sm, "sm"));
        private static readonly EvolutionTree Evolves7;
        private static readonly EncounterArea[] SlotsSN, SlotsMN;
        private static readonly EncounterStatic[] StaticSN, StaticMN;

        // Setup Help
        private static EncounterStatic[] getStaticEncounters(GameVersion Game)
        {
            EncounterStatic[] table;
            switch (Game)
            {
                case GameVersion.RBY:
                    return Encounter_RBY; // GameVersion filtering not possible, return immediately
                case GameVersion.GSC:
                    return Encounter_GSC;

                case GameVersion.X: case GameVersion.Y:
                    table = Encounter_XY;
                    break;
                case GameVersion.AS: case GameVersion.OR:
                    table = Encounter_AO;
                    break;
                case GameVersion.SN: case GameVersion.MN:
                    table = Encounter_SM;
                    break;

                default: return null;
            }
            return table?.Where(s => s.Version == GameVersion.Any || s.Version == Game).ToArray();
        }
        private static EncounterArea[] getEncounterTables(GameVersion Game)
        {
            string ident = null;
            byte[] tables = null;
            switch (Game)
            {
                case GameVersion.X:
                    ident = "xy";
                    tables = Resources.encounter_x;
                    break;
                case GameVersion.Y:
                    ident = "xy";
                    tables = Resources.encounter_y;
                    break;
                case GameVersion.AS:
                    ident = "ao";
                    tables = Resources.encounter_a;
                    break;
                case GameVersion.OR:
                    ident = "ao";
                    tables = Resources.encounter_o;
                    break;
                case GameVersion.SN:
                    ident = "sm";
                    tables = Resources.encounter_sn;
                    break;
                case GameVersion.MN:
                    ident = "sm";
                    tables = Resources.encounter_mn;
                    break;
            }
            if (ident == null)
                return null;

            return getEncounterTables(tables, ident);
        }
        private static EncounterArea[] getEncounterTables(byte[] mini, string ident)
        {
            return EncounterArea.getArray(Data.unpackMini(mini, ident));
        }
        private static EncounterArea[] addExtraTableSlots(EncounterArea[] GameSlots, EncounterArea[] SpecialSlots)
        {
            foreach (EncounterArea g in GameSlots)
            {
                foreach (var slots in SpecialSlots.Where(l => l.Location == g.Location))
                    g.Slots = g.Slots.Concat(slots.Slots).ToArray();
            }
            return GameSlots;
        }
        private static void MarkG6XYSlots(ref EncounterArea[] Areas)
        {
            foreach (var area in Areas)
            {
                int slotct = area.Slots.Length;
                for (int i = slotct - 15; i < slotct; i++)
                    area.Slots[i].Type = SlotType.Horde;
            }
        }
        private static void MarkG6AOSlots(ref EncounterArea[] Areas)
        {
            foreach (var area in Areas)
            {
                for (int i = 32; i < 37; i++)
                    area.Slots[i].Type = SlotType.Rock_Smash;
                int slotct = area.Slots.Length;
                for (int i = slotct - 15; i < slotct; i++)
                    area.Slots[i].Type = SlotType.Horde;

                for (int i = 0; i < slotct; i++)
                    area.Slots[i].AllowDexNav = area.Slots[i].Type != SlotType.Rock_Smash;
            }
        }
        private static void MarkG7SMSlots(ref EncounterArea[] Areas)
        {
            foreach (EncounterSlot s in Areas.SelectMany(area => area.Slots))
                s.Type = SlotType.SOS;
        }

        static Legal() // Setup
        {
            // Gen 1
            {
                StaticRBY = getStaticEncounters(GameVersion.RBY);

                var red         = EncounterArea.getArray1_GW(Resources.encounter_red);
                var blu         = EncounterArea.getArray1_GW(Resources.encounter_blue);
                var ylw         = EncounterArea.getArray1_GW(Resources.encounter_yellow);
                var rb_fish     = EncounterArea.getArray1_F(Resources.encounter_rb_f);
                var ylw_fish    = EncounterArea.getArray1_FY(Resources.encounter_yellow_f);

                red = addExtraTableSlots(red, rb_fish);
                blu = addExtraTableSlots(blu, rb_fish);
                ylw = addExtraTableSlots(ylw, ylw_fish);

                SlotsRBY = addExtraTableSlots(addExtraTableSlots(red, blu), ylw);
                Array.Resize(ref SlotsRBY, SlotsRBY.Length + 1);
                SlotsRBY[SlotsRBY.Length - 1] = FishOldGood_RBY;

                Evolves1 = new EvolutionTree(new[] { Resources.evos_rby }, GameVersion.RBY, PersonalTable.Y, MaxSpeciesID_1);
            }
            // Gen 2
            {
                StaticGSC = getStaticEncounters(GameVersion.GSC);

                // Grass/Water
                var g = EncounterArea.getArray2_GW(Resources.encounter_gold);
                var s = EncounterArea.getArray2_GW(Resources.encounter_silver);
                var c = EncounterArea.getArray2_GW(Resources.encounter_crystal);
                // Fishing
                var f = EncounterArea.getArray2_F(Resources.encounter_gsc_f);
                // Headbutt/Rock Smash
                var h_c = EncounterArea.getArray2_H(Resources.encounter_crystal_h);
                var h_g = EncounterArea.getArray2_H(Resources.encounter_gold_h);
                var h_s = EncounterArea.getArray2_H(Resources.encounter_silver_h);
                var h = h_c.Concat(h_g).Concat(h_s);

                SlotsGSC = addExtraTableSlots(g, s).Concat(c).Concat(f).Concat(h).ToArray();

                Evolves2 = new EvolutionTree(new[] { Resources.evos_gsc }, GameVersion.GSC, PersonalTable.C, MaxSpeciesID_2);
            }
            // Gen 6
            {
                StaticX = getStaticEncounters(GameVersion.X);
                StaticY = getStaticEncounters(GameVersion.Y);
                StaticA = getStaticEncounters(GameVersion.AS);
                StaticO = getStaticEncounters(GameVersion.OR);

                var XSlots = getEncounterTables(GameVersion.X);
                var YSlots = getEncounterTables(GameVersion.Y);
                MarkG6XYSlots(ref XSlots);
                MarkG6XYSlots(ref YSlots);
                SlotsX = addExtraTableSlots(XSlots, SlotsXYAlt);
                SlotsY = addExtraTableSlots(YSlots, SlotsXYAlt);

                SlotsA = getEncounterTables(GameVersion.AS);
                SlotsO = getEncounterTables(GameVersion.OR);
                MarkG6AOSlots(ref SlotsA);
                MarkG6AOSlots(ref SlotsO);

                Evolves6 = new EvolutionTree(Data.unpackMini(Resources.evos_ao, "ao"), GameVersion.ORAS, PersonalTable.AO, MaxSpeciesID_6);
            }
            // Gen 7
            {
                StaticSN = getStaticEncounters(GameVersion.SN);
                StaticMN = getStaticEncounters(GameVersion.MN);
                var REG_SN = getEncounterTables(GameVersion.SN);
                var REG_MN = getEncounterTables(GameVersion.MN);
                var SOS_SN = getEncounterTables(Resources.encounter_sn_sos, "sm");
                var SOS_MN = getEncounterTables(Resources.encounter_mn_sos, "sm");
                MarkG7SMSlots(ref SOS_SN);
                MarkG7SMSlots(ref SOS_MN);
                SlotsSN = addExtraTableSlots(REG_SN, SOS_SN).Concat(Encounter_Pelago_SM).Concat(Encounter_Pelago_SN).ToArray();
                SlotsMN = addExtraTableSlots(REG_MN, SOS_MN).Concat(Encounter_Pelago_SM).Concat(Encounter_Pelago_MN).ToArray();

                Evolves7 = new EvolutionTree(Data.unpackMini(Resources.evos_sm, "sm"), GameVersion.SM, PersonalTable.SM, MaxSpeciesID_7);
            }
        }

        // Moves
        internal static IEnumerable<int> getValidMoves(PKM pkm, DexLevel[][] evoChains, bool LVL = true, bool Tutor = true, bool Machine = true, bool MoveReminder = true)
        {
            GameVersion version = (GameVersion)pkm.Version;
            if (!pkm.IsUntraded)
                version = GameVersion.Any;
            return getValidMoves(pkm, version, evoChains, LVL: LVL, Relearn: false, Tutor: Tutor, Machine: Machine, MoveReminder: MoveReminder);
        }
        internal static IEnumerable<int> getValidMoves(PKM pkm, DexLevel[] evoChain, int generation, bool LVL = true, bool Tutor = true, bool Machine = true, bool MoveReminder = true)
        {
            GameVersion version = (GameVersion)pkm.Version;
            if (!pkm.IsUntraded)
                version = GameVersion.Any;
            return getValidMoves(pkm, version, evoChain, generation, LVL: LVL, Relearn: false, Tutor: Tutor, Machine: Machine, MoveReminder: MoveReminder); 
        }
        internal static IEnumerable<int> getValidRelearn(PKM pkm, int skipOption)
        {
            List<int> r = new List<int> { 0 };
            if (pkm.GenNumber < 6)
                return r;

            int species = getBaseSpecies(pkm, skipOption);
            r.AddRange(getLVLMoves(pkm, species, 1, pkm.AltForm));

            int form = pkm.AltForm;
            if (pkm.Format == 6 && pkm.Species != 678)
                form = 0;

            r.AddRange(getEggMoves(pkm, species, form));
            r.AddRange(getLVLMoves(pkm, species, 100, pkm.AltForm));
            return r.Distinct();
        }
        internal static IEnumerable<int> getBaseEggMoves(PKM pkm, int skipOption, GameVersion gameSource)
        {
            int species = getBaseSpecies(pkm, skipOption);

            if (gameSource == GameVersion.Any)
                gameSource = (GameVersion) pkm.Version;

            switch (gameSource)
            {
                case GameVersion.X:
                case GameVersion.Y:
                case GameVersion.XY:
                    if (pkm.InhabitedGeneration(6))
                        return LevelUpXY[species].getMoves(1);
                    break;

                case GameVersion.AS:
                case GameVersion.OR:
                case GameVersion.ORAS:
                    if (pkm.InhabitedGeneration(6))
                        return LevelUpAO[species].getMoves(1);
                    break;

                case GameVersion.SN:
                case GameVersion.MN:
                case GameVersion.SM:
                    int index = PersonalTable.SM.getFormeIndex(species, pkm.AltForm);
                    if (pkm.InhabitedGeneration(7))
                        return LevelUpSM[index].getMoves(1);
                    break;
            }
            return null;
        }
        internal static IEnumerable<int> getEggMoves(PKM pkm, GameVersion Version)
        {
            return getEggMoves(pkm, getBaseSpecies(pkm), 0, Version);
        } 

        // Encounter
        internal static EncounterLink getValidLinkGifts(PKM pkm)
        {
            switch (pkm.GenNumber)
            {
                case 6:
                    return LinkGifts6.FirstOrDefault(g => g.Species == pkm.Species && g.Level == pkm.Met_Level);
                default:
                    return null;
            }
        }
        internal static EncounterSlot[] getValidWildEncounters(PKM pkm, GameVersion gameSource = GameVersion.Any)
        {
            if (gameSource == GameVersion.Any)
                gameSource = (GameVersion)pkm.Version;

            List<EncounterSlot> s = new List<EncounterSlot>();

            foreach (var area in getEncounterAreas(pkm, gameSource))
                s.AddRange(getValidEncounterSlots(pkm, area, DexNav: pkm.AO));
            return s.Any() ? s.ToArray() : null;
        }
        internal static EncounterStatic getValidStaticEncounter(PKM pkm, GameVersion gameSource = GameVersion.Any)
        {
            if (gameSource == GameVersion.Any)
                gameSource = (GameVersion)pkm.Version;

            // Get possible encounters
            IEnumerable<EncounterStatic> poss = getStaticEncounters(pkm, gameSource: gameSource);

            int lvl = getMinLevelEncounter(pkm);
            if (lvl <= 0)
                return null;
            // Back Check against pkm
            foreach (EncounterStatic e in poss)
            {
                if (e.Nature != Nature.Random && pkm.Nature != (int)e.Nature)
                    continue;
                if (e.EggLocation != pkm.Egg_Location)
                    continue;
                if (pkm.HasOriginalMetLocation)
                {
                    if (e.Location != 0 && e.Location != pkm.Met_Location)
                        continue;
                    if (e.Level != lvl)
                        continue;
                }
                else if (e.Level > lvl)
                    continue;
                if (e.Gender != -1 && e.Gender != pkm.Gender)
                    continue;
                if (e.Form != pkm.AltForm && !e.SkipFormCheck && !getCanFormChange(pkm, e.Species))
                    continue;

                // Defer to EC/PID check
                // if (e.Shiny != null && e.Shiny != pkm.IsShiny)
                    // continue;

                // Defer ball check to later
                // if (e.Gift && pkm.Ball != 4) // PokéBall
                    // continue;

                return e;
            }
            return null;
        }
        internal static EncounterTrade getValidIngameTrade(PKM pkm, GameVersion gameSource = GameVersion.Any)
        {
            if (gameSource == GameVersion.Any)
                gameSource = (GameVersion) pkm.Version;
            if (pkm.VC || pkm.Format <= 2)
                return getValidEncounterTradeVC(pkm, gameSource);

            if (!pkm.WasIngameTrade)
            {
                if (pkm.HasOriginalMetLocation)
                    return null;
            }
            int lang = pkm.Language;
            if (lang == 0 || lang == 6)
                return null;

            int lvl = getMinLevelEncounter(pkm);
            if (lvl <= 0)
                return null;
            // Get valid pre-evolutions
            IEnumerable<DexLevel> p = getValidPreEvolutions(pkm);

            EncounterTrade[] table = null;
            if (pkm.XY)
                table = TradeGift_XY;
            else if (pkm.AO)
                table = TradeGift_AO;
            else if (pkm.SM)
                table = TradeGift_SM;

            EncounterTrade z = table?.FirstOrDefault(f => p.Any(r => r.Species == f.Species));

            if (z == null)
                return null;

            for (int i = 0; i < 6; i++)
                if (z.IVs[i] != -1 && z.IVs[i] != pkm.IVs[i])
                    return null;

            if (z.Shiny ^ pkm.IsShiny) // Are PIDs static?
                return null;
            if (z.TID != pkm.TID)
                return null;
            if (z.SID != pkm.SID)
                return null;
            if (pkm.HasOriginalMetLocation && z.Location != pkm.Met_Location)
                return null;
            if (pkm.HasOriginalMetLocation && z.Level != lvl)
                return null;
            if (!pkm.HasOriginalMetLocation && z.Level > lvl)
                return null;
            if (z.Nature != Nature.Random && (int)z.Nature != pkm.Nature)
                return null;
            if (z.Gender != pkm.Gender)
                return null;
            if (z.OTGender != -1 && z.OTGender != pkm.OT_Gender)
                return null;
            // if (z.Ability == 4 ^ pkm.AbilityNumber == 4) // defer to Ability 
            //    return null;

            return z;
        }
        private static EncounterTrade getValidEncounterTradeVC(PKM pkm, GameVersion gameSource)
        {
            var p = getValidPreEvolutions(pkm).ToArray();

            switch (gameSource)
            {
                case GameVersion.RBY:
                    return getValidEncounterTradeVC1(pkm, p, TradeGift_RBY);
                case GameVersion.GSC:
                    return getValidEncounterTradeVC2(pkm, p);
                default:
                    return null;
            }
        }
        private static EncounterTrade getValidEncounterTradeVC2(PKM pkm, DexLevel[] p)
        {
            // Check RBY trades with loosened level criteria.
            var z = getValidEncounterTradeVC1(pkm, p, TradeGift_RBY_2);
            if (z != null)
                return z;

            // Check GSC trades. Reuse generic table fetch-match
            z = getValidEncounterTradeVC1(pkm, p, TradeGift_GSC);

            // Filter Criteria
            if (z?.Gender != pkm.Gender)
                return null;
            if (z.TID != pkm.TID)
                return null;
            if (!z.IVs.SequenceEqual(pkm.IVs))
                return null;
            if (pkm.Met_Location != 0 && pkm.Format == 2 && pkm.Met_Location != z.Location)
                return null;
            
            int index = Array.IndexOf(TradeGift_GSC, z);
            if (TradeGift_GSC_OTs[index].All(ot => ot != pkm.OT_Name))
                return null;
            
            return z;
        }
        private static EncounterTrade getValidEncounterTradeVC1(PKM pkm, DexLevel[] p, EncounterTrade[] table)
        {
            var z = table.FirstOrDefault(f => p.Any(r => r.Species == f.Species));
            if (z == null)
                return null;
            if (z.Level > pkm.CurrentLevel) // minimum required level
                return null;
            return z;
        }
        private static Tuple<object, int, byte> getEncounter12(PKM pkm, GameVersion game)
        {
            // Tuple: Encounter, Level, Preference (higher = more preferred)
            bool WasEgg = game == GameVersion.GSC && getWasEgg23(pkm) && !NoHatchFromEgg.Contains(pkm.Species);
            if (WasEgg)
            {
                // Further Filtering
                if (pkm.Format < 3)
                {
                    WasEgg &= pkm.Met_Location == 0 || pkm.Met_Level == 1; // 2->1->2 clears met info
                    WasEgg &= pkm.CurrentLevel >= 5;
                }
            }

            // Since encounter matching is super weak due to limited stored data in the structure
            // Calculate all 3 at the same time and pick the best result (by species).
            // Favor special event move gifts as Static Encounters when applicable
            var s = getValidStaticEncounter(pkm, game);
            var e = getValidWildEncounters(pkm, game);
            var t = getValidIngameTrade(pkm, game);

            if (s == null && e == null && t == null && !WasEgg)
                return null;

            const byte invalid = 255;
            var sm = s?.Species ?? invalid;
            var em = e?.Min(slot => slot.Species) ?? invalid;
            var tm = t?.Species ?? invalid;

            if (s != null && s.Moves[0] != 0 && pkm.Moves.Contains(s.Moves[0]))
                return new Tuple<object, int, byte>(s, s.Level, 20); // special move 
            if (game == GameVersion.GSC)
            {
                if (t != null && t.TID != 0)
                    return new Tuple<object, int, byte>(t, t.Level, 10); // gen2 trade
                if (WasEgg && new[] { sm, em, tm }.Min(a => a) >= 5)
                    return new Tuple<object, int, byte>(true, 5, 9); // gen2 egg
            }
            if (em <= sm && em <= tm)
                return new Tuple<object, int, byte>(e, e.Where(slot => slot.Species == em).Min(slot => slot.LevelMin), 3);
            if (sm <= em && sm <= tm)
                return new Tuple<object, int, byte>(s, s.Level, 2);
            if (tm <= sm && tm <= em)
                return new Tuple<object, int, byte>(t, t.Level, 1);
            return null;
        }
        internal static Tuple<object, int, byte> getEncounter12(PKM pkm, bool gen2)
        {
            var g1 = getEncounter12(pkm, GameVersion.RBY);
            var g2 = gen2 ? getEncounter12(pkm, GameVersion.GSC) : null;

            if (g1 == null || g2 == null)
                return g1 ?? g2;
            
            var t = g1.Item1 as EncounterTrade;
            if (t != null && getEncounterTrade1Valid(pkm))
                return g1;

            // Both generations can provide an encounter. Return highest preference
            if (g1.Item3 > g2.Item3)
                return g1;
            if (g1.Item3 < g2.Item3)
                return g2;
            // Return lowest level encounter
            return g1.Item2 < g2.Item2 ? g1 : g2;
        }
        internal static bool getEncounterTrade1Valid(PKM pkm)
        {
            string ot = pkm.OT_Name;
            string tr = pkm.Format <= 2 ? "TRAINER" : "Trainer"; // decaps on transfer
            return ot == "トレーナー" || ot == tr;
        }
        internal static EncounterSlot[] getValidFriendSafari(PKM pkm)
        {
            if (!pkm.XY)
                return null;
            if (pkm.Met_Location != 148) // Friend Safari
                return null;
            if (pkm.Met_Level != 30)
                return null;

            IEnumerable<DexLevel> vs = getValidPreEvolutions(pkm);
            List<EncounterSlot> slots = new List<EncounterSlot>();
            foreach (DexLevel d in vs.Where(d => FriendSafari.Contains(d.Species) && d.Level >= 30))
            {
                slots.Add(new EncounterSlot
                {
                    Species = d.Species,
                    LevelMin = 30,
                    LevelMax = 30,
                    Form = 0,
                    Type = SlotType.FriendSafari,
                });
            }

            return slots.Any() ? slots.ToArray() : null;
        }
        private static bool getWasEgg23(PKM pkm)
        {
            if (pkm.Format > 2 && pkm.Ball != 4)
                return false;

            int lvl = pkm.CurrentLevel;
            if (lvl < 5)
                return false;

            return getEvolutionValid(pkm);
        }

        // Generation Specific Fetching
        private static EvolutionTree getEvolutionTable(PKM pkm)
        {
            switch (pkm.Format)
            {
                case 1:
                    return Evolves1;
                case 2:
                    return Evolves2;

                case 6:
                    return Evolves6;
                case 7:
                    return Evolves7;

                default:
                    return Evolves7;
            }
        }

        internal static IEnumerable<MysteryGift> getValidGifts(PKM pkm)
        {
            switch (pkm.GenNumber)
            {
                case 6:
                    return getMatchingWC6(pkm, MGDB_G6);
                case 7:
                    return getMatchingWC7(pkm, MGDB_G7);
                default:
                    return new List<MysteryGift>();
            }
        }
        private static IEnumerable<MysteryGift> getMatchingWC6(PKM pkm, IEnumerable<MysteryGift> DB)
        {
            List<MysteryGift> validWC6 = new List<MysteryGift>();
            if (DB == null)
                return validWC6;
            var vs = getValidPreEvolutions(pkm).ToArray();
            foreach (WC6 wc in DB.OfType<WC6>().Where(wc => vs.Any(dl => dl.Species == wc.Species)))
            {
                if (pkm.Egg_Location == 0) // Not Egg
                {
                    if (wc.CardID != pkm.SID) continue;
                    if (wc.TID != pkm.TID) continue;
                    if (wc.OT != pkm.OT_Name) continue;
                    if (wc.OTGender != pkm.OT_Gender) continue;
                    if (wc.PIDType == 0 && pkm.PID != wc.PID) continue;
                    if (wc.PIDType == 2 && !pkm.IsShiny) continue;
                    if (wc.PIDType == 3 && pkm.IsShiny) continue;
                    if (wc.OriginGame != 0 && wc.OriginGame != pkm.Version) continue;
                    if (wc.EncryptionConstant != 0 && wc.EncryptionConstant != pkm.EncryptionConstant) continue;
                    if (wc.Language != 0 && wc.Language != pkm.Language) continue;
                }
                if (wc.Form != pkm.AltForm && vs.All(dl => !getCanFormChange(pkm, dl.Species))) continue;
                if (wc.MetLocation != pkm.Met_Location) continue;
                if (wc.EggLocation != pkm.Egg_Location) continue;
                if (wc.Level != pkm.Met_Level) continue;
                if (wc.Ball != pkm.Ball) continue;
                if (wc.OTGender < 3 && wc.OTGender != pkm.OT_Gender) continue;
                if (wc.Nature != 0xFF && wc.Nature != pkm.Nature) continue;
                if (wc.Gender != 3 && wc.Gender != pkm.Gender) continue;

                if (wc.CNT_Cool > pkm.CNT_Cool) continue;
                if (wc.CNT_Beauty > pkm.CNT_Beauty) continue;
                if (wc.CNT_Cute > pkm.CNT_Cute) continue;
                if (wc.CNT_Smart > pkm.CNT_Smart) continue;
                if (wc.CNT_Tough > pkm.CNT_Tough) continue;
                if (wc.CNT_Sheen > pkm.CNT_Sheen) continue;

                // Some checks are best performed separately as they are caused by users screwing up valid data.
                // if (!wc.RelearnMoves.SequenceEqual(pkm.RelearnMoves)) continue; // Defer to relearn legality
                // if (wc.OT.Length > 0 && pkm.CurrentHandler != 1) continue; // Defer to ownership legality
                // if (wc.OT.Length > 0 && pkm.OT_Friendship != PKX.getBaseFriendship(pkm.Species)) continue; // Friendship
                // if (wc.Level > pkm.CurrentLevel) continue; // Defer to level legality
                // RIBBONS: Defer to ribbon legality

                validWC6.Add(wc);
            }
            return validWC6;
        }
        private static IEnumerable<MysteryGift> getMatchingWC7(PKM pkm, IEnumerable<MysteryGift> DB)
        {
            List<MysteryGift> validWC7 = new List<MysteryGift>();
            if (DB == null)
                return validWC7;
            var vs = getValidPreEvolutions(pkm).ToArray();
            foreach (WC7 wc in DB.OfType<WC7>().Where(wc => vs.Any(dl => dl.Species == wc.Species)))
            {
                if (pkm.Egg_Location == 0) // Not Egg
                {
                    if (wc.OTGender != 3)
                    {
                        if (wc.SID != pkm.SID) continue;
                        if (wc.TID != pkm.TID) continue;
                        if (wc.OTGender != pkm.OT_Gender) continue;
                    }
                    if (!string.IsNullOrEmpty(wc.OT) && wc.OT != pkm.OT_Name) continue;
                    if (wc.OriginGame != 0 && wc.OriginGame != pkm.Version) continue;
                    if (wc.EncryptionConstant != 0 && wc.EncryptionConstant != pkm.EncryptionConstant) continue;
                    if (wc.Language != 0 && wc.Language != pkm.Language) continue;
                }
                if (wc.Form != pkm.AltForm && vs.All(dl => !getCanFormChange(pkm, dl.Species))) continue;
                if (wc.MetLocation != pkm.Met_Location) continue;
                if (wc.EggLocation != pkm.Egg_Location) continue;
                if (wc.MetLevel != pkm.Met_Level) continue;
                if (wc.Ball != pkm.Ball) continue;
                if (wc.OTGender < 3 && wc.OTGender != pkm.OT_Gender) continue;
                if (wc.Nature != 0xFF && wc.Nature != pkm.Nature) continue;
                if (wc.Gender != 3 && wc.Gender != pkm.Gender) continue;

                if (wc.CNT_Cool > pkm.CNT_Cool) continue;
                if (wc.CNT_Beauty > pkm.CNT_Beauty) continue;
                if (wc.CNT_Cute > pkm.CNT_Cute) continue;
                if (wc.CNT_Smart > pkm.CNT_Smart) continue;
                if (wc.CNT_Tough > pkm.CNT_Tough) continue;
                if (wc.CNT_Sheen > pkm.CNT_Sheen) continue;

                if (wc.PIDType == 2 && !pkm.IsShiny) continue;
                if (wc.PIDType == 3 && pkm.IsShiny) continue;
                
                if ((pkm.SID << 16 | pkm.TID) == 0x79F57B49) // Greninja WC has variant PID and can arrive @ 36 or 37
                {
                    if (!pkm.IsShiny)
                        validWC7.Add(wc);
                    continue;
                }
                if (wc.PIDType == 0 && pkm.PID != wc.PID) continue;

                // Some checks are best performed separately as they are caused by users screwing up valid data.
                // if (!wc.RelearnMoves.SequenceEqual(pkm.RelearnMoves)) continue; // Defer to relearn legality
                // if (wc.OT.Length > 0 && pkm.CurrentHandler != 1) continue; // Defer to ownership legality
                // if (wc.OT.Length > 0 && pkm.OT_Friendship != PKX.getBaseFriendship(pkm.Species)) continue; // Friendship
                // if (wc.Level > pkm.CurrentLevel) continue; // Defer to level legality
                // RIBBONS: Defer to ribbon legality

                validWC7.Add(wc);
            }
            return validWC7;
        }
        internal static IEnumerable<int> getLineage(PKM pkm)
        {
            if (pkm.IsEgg)
                return new[] {pkm.Species};

            var table = getEvolutionTable(pkm);
            var lineage = table.getValidPreEvolutions(pkm, pkm.CurrentLevel);
            return lineage.Select(evolution => evolution.Species);
        }
        internal static IEnumerable<int> getWildBalls(PKM pkm)
        {
            switch (pkm.GenNumber)
            {
                case 1:
                    return WildPokeBalls1;
                case 2:
                    return WildPokeBalls2;
                case 3:
                    return WildPokeBalls3;
                case 4:
                    return pkm.HGSS ? WildPokeBalls4_HGSS : WildPokeBalls4_DPPt;
                case 5:
                    return WildPokeBalls5;
                case 6:
                    return WildPokeballs6;
                case 7:
                    return WildPokeballs7;

                default:
                    return null;
            }
        }
        internal static int getMaxSpeciesOrigin(PKM pkm)
        {
            if (pkm.Format == 1 || pkm.VC1) // Gen1 VC could not trade with gen 2 yet
                return getMaxSpeciesOrigin(1);
            if (pkm.Format == 2 || pkm.VC2)
                return getMaxSpeciesOrigin(2);
            return getMaxSpeciesOrigin(pkm.GenNumber);
        }
        internal static int getMaxSpeciesOrigin(int generation)
        {
            switch (generation)
            {
                case 1: return MaxSpeciesID_1;
                case 2: return MaxSpeciesID_2;
                case 3: return MaxSpeciesID_3;
                case 4: return MaxSpeciesID_4;
                case 5: return MaxSpeciesID_5;
                case 6: return MaxSpeciesID_6;
                case 7: return MaxSpeciesID_7;
                default: return -1;
            }
        }
        internal static IEnumerable<int> getFutureGenEvolutions(int generation)
        {
            switch (generation)
            {
                case 1: return FutureEvolutionsGen1;
                case 2: return FutureEvolutionsGen2;
                case 3: return FutureEvolutionsGen3;
                case 4: return FutureEvolutionsGen4;
                case 5: return FutureEvolutionsGen5;
                default: return new int[0];
            }
        }

        internal static bool[] getReleasedHeldItems(int generation)
        {
            switch (generation)
            {
                case 2: return ReleasedHeldItems_2;
                case 3: return ReleasedHeldItems_3;
                case 4: return ReleasedHeldItems_4;
                case 5: return ReleasedHeldItems_5;
                case 6: return ReleasedHeldItems_6;
                case 7: return ReleasedHeldItems_7;
                default: return new bool[0];
            }
        }
        internal static bool getHeldItemAllowed(int generation, int item)
        {
            if (item < 0)
                return false;
            if (item == 0)
                return true;

            var items = getReleasedHeldItems(generation);
            return items.Length > item && items[item];
        }

        internal static bool getDexNavValid(PKM pkm)
        {
            if (!pkm.AO || !pkm.InhabitedGeneration(6))
                return false;

            IEnumerable<EncounterArea> locs = getDexNavAreas(pkm);
            return locs.Select(loc => getValidEncounterSlots(pkm, loc, DexNav: true)).Any(slots => slots.Any(slot => slot.AllowDexNav && slot.DexNav));
        }
        internal static bool getHasEvolved(PKM pkm)
        {
            if (pkm.IsEgg)
                return false;

            return getValidPreEvolutions(pkm).Count() > 1;
        }
        internal static bool getHasEvolvedFormChange(PKM pkm)
        {
            if (pkm.IsEgg)
                return false;

            if (pkm.Format >= 7 && EvolveToAlolanForms.Contains(pkm.Species))
                return pkm.AltForm == 1;
            if (pkm.Species == 678 && pkm.Gender == 1)
                return pkm.AltForm == 1;
            if (pkm.Species == 773)
                return true;
            return false;
        }
        internal static bool getHasTradeEvolved(PKM pkm)
        {
            if (pkm.IsEgg)
                return false;

            var table = getEvolutionTable(pkm);
            var lineage = table.getValidPreEvolutions(pkm, 100, skipChecks:true);
            return lineage.Any(evolution => EvolutionMethod.TradeMethods.Any(method => method == evolution.Flag)); // Trade Evolutions
        }
        internal static bool getEvolutionValid(PKM pkm)
        {
            var curr = getValidPreEvolutions(pkm);
            var poss = getValidPreEvolutions(pkm, 100, skipChecks: true);

            if (SplitBreed.Contains(getBaseSpecies(pkm, 1)))
                return curr.Count() >= poss.Count() - 1;
            return curr.Count() >= poss.Count();
        }
        internal static bool getCanFormChange(PKM pkm, int species)
        {
            if (FormChange.Contains(species))
                return true;
            if (getHasEvolvedFormChange(pkm))
                return true;
            if (pkm.Species == 718 && pkm.InhabitedGeneration(7) && pkm.AltForm == 3)
                return true;
            return false;
        }

        internal static EncounterArea getCaptureLocation(PKM pkm)
        {
            return (from area in getEncounterSlots(pkm, 100)
                let slots = getValidEncounterSlots(pkm, area, pkm.AO, ignoreLevel:true).ToArray()
                where slots.Any()
                select new EncounterArea
                {
                    Location = area.Location, Slots = slots,
                }).OrderBy(area => area.Slots.Min(x => x.LevelMin)).FirstOrDefault();
        }
        internal static EncounterStatic getRBYStaticTransfer(int species)
        {
            return new EncounterStatic
            {
                Species = species,
                Gift = true, // Forces Poké Ball
                Ability = TransferSpeciesDefaultAbility_1.Contains(species) ? 1 : 4, // Hidden by default, else first
                Shiny = species == 151 ? (bool?)false : null,
                Fateful = species == 151,
                Location = 30013,
                EggLocation = 0,
                IV3 = true,
                Version = GameVersion.RBY
            };
        }
        internal static EncounterStatic getStaticLocation(PKM pkm, int species = -1)
        {
            switch (pkm.GenNumber)
            {
                case 1:
                    return getRBYStaticTransfer(species);
                default:
                    return getStaticEncounters(pkm, 100).OrderBy(s => s.Level).FirstOrDefault();
            }
        }

        public static int getLowestLevel(PKM pkm, int refSpecies = -1)
        {
            if (refSpecies == -1)
                refSpecies = getBaseSpecies(pkm);
            for (int i = 0; i < 100; i++)
            {
                var table = getEvolutionTable(pkm);
                var evos = table.getValidPreEvolutions(pkm, i, skipChecks:true).ToArray();
                if (evos.Any(evo => evo.Species == refSpecies))
                    return evos.OrderByDescending(evo => evo.Level).First().Level;
            }
            return 100;
        }
        internal static bool getCanBeCaptured(int species, int gen, GameVersion version = GameVersion.Any)
        {
            switch (gen)
            {
                case 1:
                    return getCanBeCaptured(species, SlotsRBY, StaticRBY);

                case 6:
                    switch (version)
                    {
                        case GameVersion.Any:
                            return getCanBeCaptured(species, SlotsX, StaticX, XY:true)
                                || getCanBeCaptured(species, SlotsY, StaticY, XY:true)
                                || getCanBeCaptured(species, SlotsA, StaticA)
                                || getCanBeCaptured(species, SlotsO, StaticO);
                        case GameVersion.X:
                            return getCanBeCaptured(species, SlotsX, StaticX, XY:true);
                        case GameVersion.Y:
                            return getCanBeCaptured(species, SlotsY, StaticY, XY:true);
                        case GameVersion.AS:
                            return getCanBeCaptured(species, SlotsA, StaticA);
                        case GameVersion.OR:
                            return getCanBeCaptured(species, SlotsO, StaticO);

                        default:
                            return false;
                    }
                case 7:
                    switch (version)
                    {
                        case GameVersion.Any:
                            return getCanBeCaptured(species, SlotsSN, StaticSN)
                                || getCanBeCaptured(species, SlotsMN, StaticMN);
                        case GameVersion.SN:
                            return getCanBeCaptured(species, SlotsSN, StaticSN);
                        case GameVersion.MN:
                            return getCanBeCaptured(species, SlotsMN, StaticMN);

                        default:
                            return false;
                    }
            }
            return false;
        }
        private static bool getCanBeCaptured(int species, IEnumerable<EncounterArea> area, IEnumerable<EncounterStatic> statics, bool XY = false)
        {
            if (XY && FriendSafari.Contains(species))
                return true;

            if (area.Any(loc => loc.Slots.Any(slot => slot.Species == species)))
                return true;
            if (statics.Any(enc => enc.Species == species && !enc.Gift))
                return true;
            return false;
        }

        internal static bool getCanLearnMachineMove(PKM pkm, int move, IEnumerable<int> generations, GameVersion version = GameVersion.Any)
        {
            return generations.Any(generation => getCanLearnMachineMove(pkm, generation, move, version));
        }
        internal static bool getCanRelearnMove(PKM pkm, int move, IEnumerable<int> generations, GameVersion version = GameVersion.Any)
        {
            return generations.Any(generation => getCanRelearnMove(pkm, move, generation, version));
        }
        internal static bool getCanLearnMove(PKM pkm, int move, IEnumerable<int> generations, GameVersion version = GameVersion.Any)
        {
            return generations.Any(generation => getCanLearnMove(pkm, move, generation, version));
        }
        internal static bool getCanKnowMove(PKM pkm, int move, IEnumerable<int> generations, GameVersion version = GameVersion.Any)
        {
            return generations.Any(generation => getCanKnowMove(pkm, move, generation, version));
        }
        internal static bool getCanLearnMachineMove(PKM pkm, int move, int generation, GameVersion version = GameVersion.Any)
        {
            return getValidMoves(pkm, version, getValidPreEvolutions(pkm).ToArray(), generation, Machine: true).Contains(move);
        }
        internal static bool getCanRelearnMove(PKM pkm, int move, int generation, GameVersion version = GameVersion.Any)
        {
            return getValidMoves(pkm, version, getValidPreEvolutions(pkm).ToArray(), generation, LVL: true, Relearn: true).Contains(move);
        }
        internal static bool getCanLearnMove(PKM pkm, int move, int generation, GameVersion version = GameVersion.Any)
        {
            return getValidMoves(pkm, version, getValidPreEvolutions(pkm).ToArray(), generation, Tutor: true, Machine: true).Contains(move);
        }
        internal static bool getCanKnowMove(PKM pkm, int move, int generation, GameVersion version = GameVersion.Any)
        {
            if (pkm.Species == 235 && !InvalidSketch.Contains(move))
                return true;
            return getValidMoves(pkm, version, getValidPreEvolutions(pkm).ToArray(), generation, LVL: true, Relearn: true, Tutor: true, Machine: true).Contains(move);
        }

        internal static int getBaseSpecies(PKM pkm, int skipOption = 0)
        {
            if (pkm.Species == 292)
                return 290;
            if (pkm.Species == 242 && pkm.CurrentLevel < 3) // Never Cleffa
                return 113;

            var table = getEvolutionTable(pkm);
            var evos = table.getValidPreEvolutions(pkm, 100, skipChecks:true).ToArray();

            switch (skipOption)
            {
                case -1: return pkm.Species;
                case 1: return evos.Length <= 1 ? pkm.Species : evos[evos.Length - 2].Species;
                default: return evos.Length <= 0 ? pkm.Species : evos.Last().Species;
            }
        }
        internal static int getMaxLevelGeneration(PKM pkm)
        {
            return getMaxLevelGeneration(pkm, pkm.GenNumber);
        }
        internal static int getMaxLevelGeneration(PKM pkm, int generation)
        {
            if (!pkm.InhabitedGeneration(generation))
                return 0;

            if (pkm.Format <= 2)
            {
                if (generation == 1 && FutureEvolutionsGen1_Gen2LevelUp.Contains(pkm.Species))
                    return pkm.CurrentLevel - 1;
                return pkm.CurrentLevel;
            }

            if (pkm.Species == 700 && generation == 5)
                return pkm.CurrentLevel - 1;

            if (pkm.Gen3 && pkm.Format > 4 && pkm.Met_Level == pkm.CurrentLevel && FutureEvolutionsGen3_LevelUp.Contains(pkm.Species))
                return pkm.Met_Level - 1;

            if (!pkm.HasOriginalMetLocation)
                return pkm.Met_Level;
            
            return pkm.CurrentLevel;
        }
        internal static int getMinLevelEncounter(PKM pkm)
        {
            return pkm.HasOriginalMetLocation ? pkm.Met_Level : getMaxLevelGeneration(pkm);
        }
        internal static int getMinLevelGeneration(PKM pkm)
        {
            return getMinLevelGeneration(pkm, pkm.GenNumber);
        }
        internal static int getMinLevelGeneration(PKM pkm, int generation)
        {
            if (!pkm.InhabitedGeneration(generation))
                return 0;

            if (pkm.Format <= 2)
                return 2;
            
            if (!pkm.HasOriginalMetLocation)
                return pkm.Met_Level;

            if (pkm.GenNumber <= 3)
                return 2;

            return 1;
        }

        internal static DexLevel[][] getEvolutionChainsAllGens(PKM pkm, object Encounter)
        {
            var CompleteEvoChain = getEvolutionChain(pkm, Encounter).ToArray();
            int size = Math.Max(pkm.Format, 2);
            DexLevel[][] GensEvoChains = new DexLevel[size + 1][];
            for (int i = 0; i <= size; i++)
                GensEvoChains[i] = new DexLevel[0];

            if (pkm.Species == 0 || pkm.Format > 2 && pkm.GenU) // Illegal origin or empty pokemon, return only chain for current format
            {
                GensEvoChains[pkm.Format] = CompleteEvoChain;
                return GensEvoChains;
            }
            // If is egg skip the other checks and just return the evo chain for GenNumber, that will contains only the pokemon inside the egg
            // Empty list returned if is an impossible egg (like a gen 3 infernape inside an egg)
            if (pkm.IsEgg)
            {
                if (getMaxSpeciesOrigin(pkm.GenNumber) >= pkm.Species)
                    GensEvoChains[pkm.GenNumber] = CompleteEvoChain;
                return GensEvoChains;
            }

            int lvl = pkm.CurrentLevel;
            int maxgen = pkm.Format <= 2 ? 2 : pkm.Format;
            int mingen = pkm.VC2 || pkm.Format <= 2 ? 1 : pkm.GenNumber;

            // Iterate generations backwards because level will be decreased from current level in each generation
            for (int gen = maxgen; gen >= mingen; gen--)
            {
                if ((pkm.Gen1 || pkm.VC1) && pkm.Format > 2 && 2 <= gen && gen <= 6)
                    continue;
                if ((pkm.Gen2 || pkm.VC2) && 3 <= gen && gen <= 6)
                    continue;
                if (!pkm.HasOriginalMetLocation && pkm.Format > 2 && gen <= 4 && lvl > pkm.Met_Level)
                {
                    // Met location was lost at this point but it also means the pokemon existed in generations 1 to 4 with maximum level equals to met level
                    lvl = pkm.Met_Level;
                }

                int maxspeciesgen = getMaxSpeciesOrigin(gen);

                // Remove future gen evolutions after a few special considerations, 
                // it the pokemon origin is illegal like a "gen 3" Infernape the list will be emptied, it didnt existed in gen 3 in any evolution phase
                while (CompleteEvoChain.Any() && CompleteEvoChain.First().Species > maxspeciesgen)
                {   
                    // Eevee requires to level one time to be Sylveon, it can be deduced in gen 5 and before it existed with maximum one level bellow current
                    if (CompleteEvoChain.First().Species == 700 && gen == 5)
                        lvl--;
                    // This is a gen 3 pokemon in a gen 4 phase evolution that requieres level up and then transfered to gen 5+
                    // We can deduce that it existed in gen 4 until met level,
                    // but if current level is met level we can also deduce it existed in gen 3 until maximum met level -1
                    if (gen == 3 && pkm.Format > 4 && lvl == pkm.CurrentLevel && CompleteEvoChain.First().Species > MaxSpeciesID_3 && CompleteEvoChain.First().RequiresLvlUp)
                        lvl--;
                    // The same condition for gen2 evolution of gen 1 pokemon, level of the pokemon in gen 1 games would be CurrentLevel -1 one level bellow gen 2 level
                    if (gen == 1 && pkm.Format == 2 && lvl == pkm.CurrentLevel && CompleteEvoChain.First().Species > MaxSpeciesID_1 && CompleteEvoChain.First().RequiresLvlUp)
                        lvl--;
                    CompleteEvoChain = CompleteEvoChain.Skip(1).ToArray();
                }

                // Alolan form evolutions, remove from gens 1-6 chains
                if (gen < 7 && pkm.Format >= 7 && CompleteEvoChain.Any() && CompleteEvoChain.First().Form > 0 && EvolveToAlolanForms.Contains(CompleteEvoChain.First().Species))
                    CompleteEvoChain = CompleteEvoChain.Skip(1).ToArray();

                if (!CompleteEvoChain.Any())
                    continue;

                GensEvoChains[gen] = getEvolutionChain(pkm, Encounter, CompleteEvoChain.First().Species, lvl);
                if (gen > 2 && !pkm.HasOriginalMetLocation && gen >= pkm.GenNumber)
                    //Remove previous evolutions bellow transfer level
                    //For example a gen3 charizar in format 7 with current level 36 and met level 36
                    //chain level for charmander is 35, is bellow met level
                    GensEvoChains[gen] = GensEvoChains[gen].Where(e => e.Level >= lvl).ToArray();
            }
            return GensEvoChains;
        }
        internal static DexLevel[] getEvolutionChain(PKM pkm, object Encounter)
        {
            return getEvolutionChain(pkm, Encounter, pkm.Species, 100);
        }
        internal static DexLevel[] getEvolutionChain(PKM pkm, object Encounter, int maxspec, int maxlevel)
        {
            int minspec;
            DexLevel[] vs = getValidPreEvolutions(pkm).ToArray();

            // Evolution chain is in reverse order (devolution)

            if (Encounter is int)
                minspec = (int)Encounter;
            else if (Encounter is IEncounterable[])
                minspec = vs.Reverse().First(s => ((IEncounterable[]) Encounter).Any(slot => slot.Species == s.Species)).Species;
            else if (Encounter is IEncounterable)
                minspec = vs.Reverse().First(s => ((IEncounterable) Encounter).Species == s.Species).Species;
            else
                minspec = vs.Last().Species;

            int minindex = Math.Max(0, Array.FindIndex(vs, p => p.Species == minspec));
            Array.Resize(ref vs, minindex + 1);
            if (vs.Last().MinLevel > 1) // Last entry from vs is removed, turn next entry into the wild/hatched pokemon
            {
                vs.Last().MinLevel = 1;
                vs.Last().RequiresLvlUp = false;
                if (vs.First().MinLevel == 2 && !vs.First().RequiresLvlUp)
                {
                    // Example Raichu in gen 2 or later, 
                    // because Pichu requires level up Minimum level of Raichu would be 2
                    // but after removing Pichu because the origin species is Pikachu, Raichu min level should be 1
                    vs.First().MinLevel = 1;
                    vs.First().RequiresLvlUp = false;
                }
            }
            // Maxspec is used to remove future gen evolutions, to gather evolution chain of a pokemon in previous generations
            int skip = Math.Max(0, Array.FindIndex(vs, p => p.Species == maxspec));
            // Maxlevel is also used for previous generations, it removes evolutions imposible before the transfer level
            // For example a fire red charizard whose current level in XY is 50 but met level is 20, it couldnt be a Charizard in gen 3 and 4 games
            vs = vs.Skip(skip).Where(e => e.MinLevel <= maxlevel).ToArray();
            // Reduce the evolution chain levels to max level, because met level is the last one when the pokemon could be and learn moves in that generation
            foreach (DexLevel d in vs)
                d.Level = Math.Min(d.Level, maxlevel);
            return vs;
        }
        internal static string getEncounterTypeName(PKM pkm, object Encounter)
        {
            var t = Encounter;
            if (pkm.WasEgg)
                return "Egg";
            if (t is IEncounterable)
                return ((IEncounterable)t).Name;
            if (t is IEncounterable[])
            {
                var arr = (IEncounterable[])t;
                if (arr.Any())
                    return arr.First().Name;
            }
            if (t is int)
                return "Unknown";
            return t?.GetType().Name ?? "Unknown";
        }
        private static IEnumerable<EncounterArea> getDexNavAreas(PKM pkm)
        {
            switch (pkm.Version)
            {
                case (int)GameVersion.AS:
                    return SlotsA.Where(l => l.Location == pkm.Met_Location);
                case (int)GameVersion.OR:
                    return SlotsO.Where(l => l.Location == pkm.Met_Location);
                default:
                    return new EncounterArea[0];
            }
        }
        private static IEnumerable<int> getLVLMoves(PKM pkm, int species, int lvl, int formnum)
        {
            List<int> moves = new List<int>();
            if (pkm.InhabitedGeneration(1))
            {
                moves.AddRange(((PersonalInfoG1)PersonalTable.RB[species]).Moves);
                moves.AddRange(((PersonalInfoG1)PersonalTable.Y[species]).Moves);
                moves.AddRange(LevelUpRB[species].getMoves(lvl));
                moves.AddRange(LevelUpY[species].getMoves(lvl));
            }
            if (pkm.InhabitedGeneration(6))
            {
                int ind_XY = PersonalTable.XY.getFormeIndex(species, formnum);
                moves.AddRange(LevelUpXY[ind_XY].getMoves(lvl));
                int ind_AO = PersonalTable.AO.getFormeIndex(species, formnum);
                moves.AddRange(LevelUpAO[ind_AO].getMoves(lvl));
            }
            if (pkm.InhabitedGeneration(7))
            {
                int ind_SM = PersonalTable.SM.getFormeIndex(species, formnum);
                moves.AddRange(LevelUpSM[ind_SM].getMoves(lvl));
            }
            return moves;
        }
        private static IEnumerable<EncounterArea> getEncounterSlots(PKM pkm, int lvl = -1, GameVersion gameSource = GameVersion.Any)
        {
            if (gameSource == GameVersion.Any)
                gameSource = (GameVersion)pkm.Version;

            switch (gameSource)
            {
                case GameVersion.RBY:
                case GameVersion.RD: case GameVersion.BU:
                case GameVersion.GN: case GameVersion.YW:
                    return getSlots(pkm, SlotsRBY, lvl);

                case GameVersion.GSC:
                case GameVersion.GD: case GameVersion.SV:
                case GameVersion.C:
                    return getSlots(pkm, SlotsGSC, lvl);

                case GameVersion.X:
                    return getSlots(pkm, SlotsX, lvl);
                case GameVersion.Y:
                    return getSlots(pkm, SlotsY, lvl);
                case GameVersion.AS:
                    return getSlots(pkm, SlotsA, lvl);
                case GameVersion.OR:
                    return getSlots(pkm, SlotsO, lvl);

                case GameVersion.SN:
                    return getSlots(pkm, SlotsSN, lvl);
                case GameVersion.MN:
                    return getSlots(pkm, SlotsMN, lvl);
                default: return new List<EncounterArea>();
            }
        }
        private static IEnumerable<EncounterStatic> getStaticEncounters(PKM pkm, int lvl = -1, GameVersion gameSource = GameVersion.Any)
        {
            if (gameSource == GameVersion.Any)
                gameSource = (GameVersion) pkm.Version;

            switch (gameSource)
            {
                case GameVersion.RBY:
                case GameVersion.RD: case GameVersion.BU:
                case GameVersion.GN: case GameVersion.YW:
                    return getStatic(pkm, StaticRBY, lvl);

                case GameVersion.GSC:
                case GameVersion.GD: case GameVersion.SV:
                case GameVersion.C:
                    return getStatic(pkm, StaticGSC, lvl);

                case GameVersion.X:
                    return getStatic(pkm, StaticX, lvl);
                case GameVersion.Y:
                    return getStatic(pkm, StaticY, lvl);
                case GameVersion.AS:
                    return getStatic(pkm, StaticA, lvl);
                case GameVersion.OR:
                    return getStatic(pkm, StaticO, lvl);

                case GameVersion.SN:
                    return getStatic(pkm, StaticSN, lvl);
                case GameVersion.MN:
                    return getStatic(pkm, StaticMN, lvl);

                default: return new List<EncounterStatic>();
            }
        }
        private static IEnumerable<EncounterArea> getEncounterAreas(PKM pkm, GameVersion gameSource = GameVersion.Any)
        {
            if (gameSource == GameVersion.Any)
                gameSource = (GameVersion)pkm.Version;

            var slots = getEncounterSlots(pkm, gameSource: gameSource);
            bool noMet = !pkm.HasOriginalMetLocation;
            return noMet ? slots : slots.Where(area => area.Location == pkm.Met_Location);
        }
        private static IEnumerable<EncounterSlot> getValidEncounterSlots(PKM pkm, EncounterArea loc, bool DexNav, bool ignoreLevel = false)
        {
            int fluteBoost = pkm.Format < 3 ? 0 : 4;
            const int dexnavBoost = 30;

            int df = DexNav ? fluteBoost : 0;
            int dn = DexNav ? fluteBoost + dexnavBoost : 0;
            List<EncounterSlot> slotdata = new List<EncounterSlot>();

            // Get Valid levels
            IEnumerable<DexLevel> vs = getValidPreEvolutions(pkm, ignoreLevel ? 100 : -1, ignoreLevel);

            // Get slots where pokemon can exist
            bool ignoreSlotLevel = ignoreLevel;
            IEnumerable<EncounterSlot> slots = loc.Slots.Where(slot => vs.Any(evo => evo.Species == slot.Species && (ignoreSlotLevel || evo.Level >= slot.LevelMin - df)));

            int lvl = getMinLevelEncounter(pkm);
            if (lvl <= 0)
                return slotdata;
            int gen = pkm.GenNumber;

            List<EncounterSlot> encounterSlots;
            if (ignoreLevel)
                encounterSlots = slots.ToList();
            else if (pkm.HasOriginalMetLocation)
                encounterSlots = slots.Where(slot => slot.LevelMin - df <= lvl && lvl <= slot.LevelMax + (slot.AllowDexNav ? dn : df)).ToList();
            else // check for any less than current level
                encounterSlots = slots.Where(slot => slot.LevelMin <= lvl).ToList();

            if (gen <= 2)
            {   
                // For gen 1 and 2 return Minimum level slot
                // Minimum level is needed to check available moves, because there is no move reminder in gen 1,
                // There are moves in the level up table that cant be legally obtained
                EncounterSlot slotMin = encounterSlots.OrderBy(slot => slot.LevelMin).FirstOrDefault();
                if (slotMin != null)
                    slotdata.Add(slotMin);
                return slotdata;
            }

            // Pressure Slot
            EncounterSlot slotMax = encounterSlots.OrderByDescending(slot => slot.LevelMax).FirstOrDefault();
            if (slotMax != null)
            {
                slotMax = slotMax.Clone();
                slotMax.Pressure = true;
                slotMax.Form = pkm.AltForm;
            }

            if (gen >= 6 && !DexNav)
            {
                // Filter for Form Specific
                slotdata.AddRange(WildForms.Contains(pkm.Species)
                    ? encounterSlots.Where(slot => slot.Form == pkm.AltForm)
                    : encounterSlots);
                if (slotMax != null)
                    slotdata.Add(slotMax);
                return slotdata;
            }

            List<EncounterSlot> eslots = encounterSlots.Where(slot => !WildForms.Contains(pkm.Species) || slot.Form == pkm.AltForm).ToList();
            if (gen <= 5)
            {
                slotdata.AddRange(eslots);
                return slotdata;
            }
            if (slotMax != null)
                eslots.Add(slotMax);
            foreach (EncounterSlot s in eslots)
            {
                bool nav = s.AllowDexNav && (pkm.RelearnMove1 != 0 || pkm.AbilityNumber == 4);
                EncounterSlot slot = s.Clone();
                slot.DexNav = nav;

                if (slot.LevelMin > lvl)
                    slot.WhiteFlute = true;
                if (slot.LevelMax + 1 <= lvl && lvl <= slot.LevelMax + fluteBoost)
                    slot.BlackFlute = true;
                if (slot.LevelMax != lvl && slot.AllowDexNav)
                    slot.DexNav = true;
                slotdata.Add(slot);
            }
            return slotdata;
        }
        private static IEnumerable<EncounterArea> getSlots(PKM pkm, IEnumerable<EncounterArea> tables, int lvl = -1)
        {
            IEnumerable<DexLevel> vs = getValidPreEvolutions(pkm, lvl);
            List<EncounterArea> slotLocations = new List<EncounterArea>();
            foreach (var loc in tables)
            {
                IEnumerable<EncounterSlot> slots = loc.Slots.Where(slot => vs.Any(evo => evo.Species == slot.Species));

                EncounterSlot[] es = slots.ToArray();
                if (es.Length > 0)
                    slotLocations.Add(new EncounterArea { Location = loc.Location, Slots = es });
            }
            return slotLocations;
        }
        private static IEnumerable<DexLevel> getValidPreEvolutions(PKM pkm, int lvl = -1, bool skipChecks = false)
        {
            if (lvl < 0)
                lvl = pkm.CurrentLevel;
            if (lvl == 1 && pkm.IsEgg)
                return new List<DexLevel>
                {
                    new DexLevel { Species = pkm.Species, Level = 1, MinLevel = 1 },
                };
            if (pkm.Species == 292 && lvl >= 20 && (!pkm.HasOriginalMetLocation || pkm.Met_Level + 1 <= lvl))
                return new List<DexLevel>
                {
                    new DexLevel { Species = 292, Level = lvl, MinLevel =20 },
                    new DexLevel { Species = 290, Level = lvl-1, MinLevel = 1 }
                };

            var et = getEvolutionTable(pkm);
            return et.getValidPreEvolutions(pkm, lvl, skipChecks: skipChecks);
        }
        private static IEnumerable<EncounterStatic> getStatic(PKM pkm, IEnumerable<EncounterStatic> table, int lvl = -1)
        {
            IEnumerable<DexLevel> dl = getValidPreEvolutions(pkm, lvl);
            return table.Where(e => dl.Any(d => d.Species == e.Species));
        }
        private static IEnumerable<int> getValidMoves(PKM pkm, GameVersion Version, IReadOnlyList<DexLevel[]> vs, bool LVL = false, bool Relearn = false, bool Tutor = false, bool Machine = false, bool MoveReminder = true)
        {
            List<int> r = new List<int> { 0 };
            if (Relearn && pkm.Format >= 6)
                r.AddRange(pkm.RelearnMoves);

            for (int gen = pkm.GenNumber; gen <= pkm.Format; gen++)
                if (vs[gen].Any())
                    r.AddRange(getValidMoves(pkm, Version, vs[gen], gen, LVL, Tutor, Machine, MoveReminder));

            return r.Distinct().ToArray();
        }
        private static IEnumerable<int> getValidMoves(PKM pkm, GameVersion Version, DexLevel[] vs, int Generation, bool LVL = false, bool Relearn = false, bool Tutor = false, bool Machine = false, bool MoveReminder = true)
        {
            List<int> r = new List<int> { 0 };
            if (!vs.Any())
                return r;
            int species = pkm.Species;

            // Special Type Tutors Availability
            bool moveTutor = Tutor || MoveReminder; // Usually true, except when called for move suggestions (no tutored moves)
            
            if (FormChangeMoves.Contains(species)) // Deoxys & Shaymin & Giratina (others don't have extra but whatever)
            {
                int formcount = pkm.PersonalInfo.FormeCount;
                for (int i = 0; i < formcount; i++)
                    r.AddRange(getMoves(pkm, species, vs.First().Level, i, moveTutor, Version, LVL, Tutor, Machine, MoveReminder, Generation));
                if (Relearn) r.AddRange(pkm.RelearnMoves);
                return r.Distinct().ToArray();
            }

            foreach (DexLevel evo in vs)
                r.AddRange(getMoves(pkm, evo.Species, evo.Level, pkm.AltForm, moveTutor, Version, LVL, Tutor, Machine, MoveReminder, Generation));

            if (pkm.Format <= 3)
                return r.Distinct().ToArray();
            if (LVL)
            { 
                if (species == 479 && Generation >= 4) // Rotom
                    r.Add(RotomMoves[pkm.AltForm]);

                if (species == 718 && Generation == 7) // Zygarde
                    r.AddRange(ZygardeMoves);
            }
            if (Tutor)
            {
                if (species == 647) // Keldeo
                    r.Add(548); // Secret Sword
                if (species == 648) // Meloetta
                    r.Add(547); // Relic Song

                if (species == 25 && pkm.Format == 6 && Generation == 6) // Pikachu
                    r.Add(PikachuMoves[pkm.AltForm]);

                if ((species == 25 || species == 26) && Generation == 7) // Pikachu/Raichu Tutor
                    r.Add(344); // Volt Tackle
            }
            if (Relearn && Generation >= 6)
                r.AddRange(pkm.RelearnMoves);
            return r.Distinct().ToArray();
        }
        private static IEnumerable<int> getMoves(PKM pkm, int species, int lvl, int form, bool moveTutor, GameVersion Version, bool LVL, bool specialTutors, bool Machine, bool MoveReminder)
        {
            List<int> r = new List<int> { 0 };
            int gen = pkm.GenNumber;
            if (pkm.GenNumber < 3)
            {
                int max = pkm.Format < 3 ? 2 : 1;
                for (; gen <= max; gen++)
                    if (pkm.InhabitedGeneration(gen, species))
                        r.AddRange(getMoves(pkm, species, lvl, form, moveTutor, Version, LVL, specialTutors, Machine, MoveReminder, gen));
                gen = 7;
            }

            for (; gen <= pkm.Format; gen++)
                if (pkm.InhabitedGeneration(gen))
                    r.AddRange(getMoves(pkm, species, lvl, form, moveTutor, Version, LVL, specialTutors, Machine, MoveReminder, gen));
            return r.Distinct();
        }
        private static IEnumerable<int> getMoves(PKM pkm, int species, int lvl, int form, bool moveTutor, GameVersion Version, bool LVL, bool specialTutors, bool Machine, bool MoveReminder, int Generation)
        {
            List<int> r = new List<int>();

            var ver = Version;
            switch (Generation)
            {
                case 1:
                    {
                        int index = PersonalTable.RB.getFormeIndex(species, 0);
                        if (index == 0)
                            return r;
                        var pi_rb = (PersonalInfoG1)PersonalTable.RB[index];
                        var pi_y = (PersonalInfoG1)PersonalTable.Y[index];
                        if (LVL)
                        {
                            r.AddRange(pi_rb.Moves);
                            r.AddRange(pi_y.Moves);
                            r.AddRange(LevelUpRB[index].getMoves(lvl));
                            r.AddRange(LevelUpY[index].getMoves(lvl));
                        }
                        if (Machine)
                        {
                            r.AddRange(TMHM_RBY.Where((t, m) => pi_rb.TMHM[m]));
                            r.AddRange(TMHM_RBY.Where((t, m) => pi_y.TMHM[m]));
                        }
                        if (moveTutor)
                            r.AddRange(getTutorMoves(pkm, species, form, specialTutors, Generation));
                        break;
                    }
                case 2:
                    {
                        int index = PersonalTable.C.getFormeIndex(species, 0);
                        var pi_c = (PersonalInfoG2)PersonalTable.C[index];
                        if (LVL)
                        {
                            r.AddRange(LevelUpGS[index].getMoves(lvl));
                            r.AddRange(LevelUpC[index].getMoves(lvl));
                        }
                        if (Machine)
                        {
                            r.AddRange(TMHM_GSC.Where((t, m) => pi_c.TMHM[m]));
                        }
                        if (moveTutor)
                            r.AddRange(getTutorMoves(pkm, species, form, specialTutors, Generation));
                        if (pkm.Format == 1) //tradeback gen 2 -> gen 1
                            r = r.Where(m => m <= MaxMoveID_1).ToList();
                        break;
                    }
                case 6:
                    switch (ver)
                    {
                        case GameVersion.Any: // Start at the top, hit every table
                        case GameVersion.X: case GameVersion.Y: case GameVersion.XY:
                        {
                            int index = PersonalTable.XY.getFormeIndex(species, form);
                            PersonalInfo pi = PersonalTable.XY[index];

                            if (LVL) r.AddRange(LevelUpXY[index].getMoves(lvl));
                            if (moveTutor) r.AddRange(getTutorMoves(pkm, species, form, specialTutors, Generation));
                            if (Machine) r.AddRange(TMHM_XY.Where((t, m) => pi.TMHM[m]));

                            if (ver == GameVersion.Any) // Fall Through
                                goto case GameVersion.ORAS;
                            break;
                        }

                        case GameVersion.AS: case GameVersion.OR: case GameVersion.ORAS:
                        {
                            int index = PersonalTable.AO.getFormeIndex(species, form);
                            PersonalInfo pi = PersonalTable.AO[index];

                            if (LVL) r.AddRange(LevelUpAO[index].getMoves(lvl));
                            if (moveTutor) r.AddRange(getTutorMoves(pkm, species, form, specialTutors, Generation));
                            if (Machine) r.AddRange(TMHM_AO.Where((t, m) => pi.TMHM[m]));
                            break;
                        }
                    }
                    break;
                case 7:
                    switch (ver)
                    {
                        case GameVersion.Any:
                        case GameVersion.SN: case GameVersion.MN: case GameVersion.SM:
                        {
                            int index = PersonalTable.SM.getFormeIndex(species, form);
                            PersonalInfo pi = PersonalTable.SM.getFormeEntry(species, form);
                            if (MoveReminder)
                                lvl = 100; // Move reminder can teach any level in movepool now!

                            if (LVL) r.AddRange(LevelUpSM[index].getMoves(lvl));
                            if (moveTutor) r.AddRange(getTutorMoves(pkm, species, form, specialTutors, Generation));
                            if (Machine) r.AddRange(TMHM_SM.Where((t, m) => pi.TMHM[m]));
                            break;
                        }
                    }
                    break;

                default:
                    return r;
            }
            return r;
        }
        private static IEnumerable<int> getEggMoves(PKM pkm, int species, int formnum, GameVersion Version = GameVersion.Any)
        {
            if (!pkm.InhabitedGeneration(pkm.GenNumber, species))
                return new List<int>();

            switch (pkm.GenNumber)
            {
                case 1:
                case 2:
                    switch (Version)
                    {
                        case GameVersion.GS:
                            return EggMovesGS[species].Moves;
                        case GameVersion.C:
                            return EggMovesC[species].Moves;
                        default:
                            return new List<int>();

                    }
                case 6: // entries per species
                    return EggMovesAO[species].Moves.Concat(EggMovesXY[species].Moves);

                case 7: // entries per form if required
                    var entry = EggMovesSM[species];
                    if (formnum > 0 && AlolanOriginForms.Contains(species))
                        entry = EggMovesSM[entry.FormTableIndex + formnum - 1];
                    return entry.Moves;

                default:
                    return new List<int>();
            }
        }
        private static IEnumerable<int> getTutorMoves(PKM pkm, int species, int form, bool specialTutors, int generation)
        {
            List<int> moves = new List<int>();
            PersonalInfo info;
            switch (generation)
            {
                case 2:
                    moves.AddRange(Tutors_GSC.Where((t, i) => PersonalTable.C[species].TMHM[57 + i]));
                    goto case 1;
                case 1:
                    if (pkm.Format < 3 && (pkm.Species == 25 || pkm.Species == 26)) // Surf Pikachu via Stadium
                        moves.Add(57);
                    break;
                //case 5:
                // Varied Tutors
                //if (pkm.InhabitedGeneration(5) && Tutors)
                //{
                //    //PersonalInfo pi = PersonalTable.B2W2.getFormeEntry(species, form);
                //    //for (int i = 0; i < Tutors_B2W2.Length; i++)
                //    //    for (int b = 0; b < Tutors_B2W2[i].Length; b++)
                //    //        if (pi.SpecialTutors[i][b])
                //    //            moves.Add(Tutors_B2W2[i][b]);
                //}
                case 6:
                    info = PersonalTable.AO[species];
                    moves.AddRange(TypeTutor6.Where((t, i) => info.TypeTutors[i]));
                    if ( pkm.InhabitedGeneration(6) && specialTutors && (pkm.AO || !pkm.IsUntraded))
                    {
                        PersonalInfo pi = PersonalTable.AO.getFormeEntry(species, form);
                        for (int i = 0; i < Tutors_AO.Length; i++)
                            for (int b = 0; b < Tutors_AO[i].Length; b++)
                                if (pi.SpecialTutors[i][b])
                                    moves.Add(Tutors_AO[i][b]);
                    }
                    break;
                case 7:
                    info = PersonalTable.SM[species];
                    // Type Tutors -- Pledge moves and High BP moves switched places in G7+
                    moves.AddRange(TypeTutor7.Where((t, i) => info.TypeTutors[i]));
                    // No special tutors in G7
                    break;

            }
            return moves.Distinct();
        }
    }
}
