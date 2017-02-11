using System;
using System.Collections.Generic;
using System.Linq;
using PKHeX.Core.Properties;

namespace PKHeX.Core
{
    public static partial class Legal
    {
        // Event Database(s)
        public static MysteryGift[] MGDB_G6, MGDB_G7 = new MysteryGift[0];

        // Gen 1
        private static readonly Learnset[] LevelUpRB = Learnset1.getArray(Resources.lvlmove_rby);
        private static readonly EvolutionTree Evolves1;

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
                case GameVersion.X:
                case GameVersion.Y:
                    table = Encounter_XY;
                    break;
                case GameVersion.AS:
                case GameVersion.OR:
                    table = Encounter_AO;
                    break;
                case GameVersion.SN:
                case GameVersion.MN:
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
                Evolves1 = new EvolutionTree(new[] { Resources.evos_rby }, GameVersion.RBY, PersonalTable.RBY, MaxSpeciesID_1);
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
        internal static IEnumerable<int> getValidMoves(PKM pkm, IEnumerable<DexLevel> evoChain, bool Tutor = true, bool Machine = true, bool MoveReminder = true)
        {
            GameVersion version = (GameVersion)pkm.Version;
            if (!pkm.IsUntraded)
                version = GameVersion.Any;
            return getValidMoves(pkm, version, evoChain, LVL: true, Relearn: false, Tutor: Tutor, Machine: Machine, MoveReminder: MoveReminder); 
        }
        internal static IEnumerable<int> getValidRelearn(PKM pkm, int skipOption)
        {
            List<int> r = new List<int> { 0 };
            int species = getBaseSpecies(pkm, skipOption);
            r.AddRange(getLVLMoves(pkm, species, 1, pkm.AltForm));

            int form = pkm.AltForm;
            if (pkm.Format < 6)
                form = 0;
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

        // Encounter
        internal static EncounterLink getValidLinkGifts(PKM pkm)
        {
            switch (pkm.GenNumber)
            {
                case 6:
                    return LinkGifts6.FirstOrDefault(g => g.Species == pkm.Species && g.Level == pkm.Met_Level);
                default:
                    return LinkGifts7.FirstOrDefault(g => g.Species == pkm.Species && g.Level == pkm.Met_Level);
            }
        }
        internal static EncounterSlot[] getValidWildEncounters(PKM pkm)
        {
            List<EncounterSlot> s = new List<EncounterSlot>();

            foreach (var area in getEncounterAreas(pkm))
                s.AddRange(getValidEncounterSlots(pkm, area, DexNav: pkm.AO));
            return s.Any() ? s.ToArray() : null;
        }
        internal static EncounterStatic getValidStaticEncounter(PKM pkm)
        {
            // Get possible encounters
            IEnumerable<EncounterStatic> poss = getStaticEncounters(pkm);
            // Back Check against pkm
            foreach (EncounterStatic e in poss)
            {
                if (e.Nature != Nature.Random && pkm.Nature != (int)e.Nature)
                    continue;
                if (e.EggLocation != pkm.Egg_Location)
                    continue;
                if (e.Location != 0 && e.Location != pkm.Met_Location)
                    continue;
                if (e.Gender != -1 && e.Gender != pkm.Gender)
                    continue;
                if (e.Level != pkm.Met_Level)
                    continue;
                if (e.Form != pkm.AltForm && !FormChange.Contains(pkm.Species) && !e.SkipFormCheck)
                    continue;

                // Defer to EC/PID check
                // if (e.Shiny != null && e.Shiny != pkm.IsShiny)
                    // continue;

                // Defer ball check to later
                // if (e.Gift && pkm.Ball != 4) // PokéBall
                    // continue;

                // Passes all checks, valid encounter
                return e;
            }
            return null;
        }
        internal static EncounterTrade getValidIngameTrade(PKM pkm)
        {
            if (!pkm.WasIngameTrade)
                return null;
            int lang = pkm.Language;
            if (lang == 0 || lang == 6)
                return null;

            // Get valid pre-evolutions
            IEnumerable<DexLevel> p = getValidPreEvolutions(pkm);

            EncounterTrade[] table = null;
            if (pkm.XY)
                table = TradeGift_XY;
            if (pkm.AO)
                table = TradeGift_AO;
            if (pkm.SM)
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
            if (z.Location != pkm.Met_Location)
                return null;
            if (z.Level != pkm.Met_Level)
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

        // Generation Specific Fetching
        private static EvolutionTree getEvolutionTable(PKM pkm)
        {
            switch (pkm.Format)
            {
                case 6:
                    return Evolves6;
                case 7:
                    return Evolves7;

                default:
                    return Evolves6;
            }
        }

        private static int getMaxSpeciesOrigin(int generation)
        {
            switch (generation)
            {
                case 1:
                    return MaxSpeciesID_1;
                case 2:
                    return MaxSpeciesID_2;
                case 3:
                    return MaxSpeciesID_3;
                case 4:
                    return MaxSpeciesID_4;
                case 5:
                    return MaxSpeciesID_5;
                case 6:
                    return MaxSpeciesID_6;
                case 7:
                    return MaxSpeciesID_7;
                default:
                    return MaxSpeciesID_7;
            }
        }

        internal static int getMaxSpeciesOrigin(PKM pkm)
        {
            if (pkm.Format == 1 || pkm.VC1) //Gen1 VC could not trade with gen 2 yet
                return getMaxSpeciesOrigin(1);
            else if (pkm.Format == 2 || pkm.VC2)
                return getMaxSpeciesOrigin(2);
            else
                return getMaxSpeciesOrigin(pkm.GenNumber);
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
                if (wc.Form != pkm.AltForm && vs.All(dl => !FormChange.Contains(dl.Species)) && !getHasEvolvedFormChange(pkm)) continue;
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
                if (wc.Form != pkm.AltForm && vs.All(dl => !FormChange.Contains(dl.Species)) && getHasEvolvedFormChange(pkm)) continue;
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
                case 7:
                    return WildPokeballs7;
                default:
                    return WildPokeballs6;
            }
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

        internal static EncounterArea getCaptureLocation(PKM pkm)
        {
            return (from area in getEncounterSlots(pkm, 100)
                let slots = getValidEncounterSlots(pkm, area, pkm.AO, ignoreLevel:true).ToArray()
                where slots.Any()
                select new EncounterArea
                {
                    Location = area.Location, Slots = slots,
                }).FirstOrDefault();
        }
        internal static EncounterStatic getStaticLocation(PKM pkm)
        {
            return getStaticEncounters(pkm).FirstOrDefault();
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

        internal static bool getCanLearnMachineMove(PKM pkm, int move, GameVersion version = GameVersion.Any)
        {
            return getValidMoves(pkm, version, getValidPreEvolutions(pkm), Machine: true).Contains(move);
        }
        internal static bool getCanRelearnMove(PKM pkm, int move, GameVersion version = GameVersion.Any)
        {
            return getValidMoves(pkm, version, getValidPreEvolutions(pkm), LVL: true, Relearn: true).Contains(move);
        }
        internal static bool getCanLearnMove(PKM pkm, int move, GameVersion version = GameVersion.Any)
        {
            return getValidMoves(pkm, version, getValidPreEvolutions(pkm), Tutor: true, Machine: true).Contains(move);
        }
        internal static bool getCanKnowMove(PKM pkm, int move, GameVersion version = GameVersion.Any)
        {
            if (pkm.Species == 235 && !InvalidSketch.Contains(move))
                return true;
            return getValidMoves(pkm, Version: version, vs: getValidPreEvolutions(pkm), LVL: true, Relearn: true, Tutor: true, Machine: true).Contains(move);
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
        internal static DexLevel[] getEvolutionChain(PKM pkm, object Encounter)
        {
            int minspec;
            var vs = getValidPreEvolutions(pkm).ToArray();

            // Evolution chain is in reverse order (devolution)

            if (Encounter is int)
                minspec = (int)Encounter;
            else if (Encounter is IEncounterable[])
                minspec = vs.Reverse().First(s => ((IEncounterable[]) Encounter).Any(slot => slot.Species == s.Species)).Species;
            else if (Encounter is IEncounterable)
                minspec = vs.Reverse().First(s => ((IEncounterable) Encounter).Species == s.Species).Species;
            else
                minspec = vs.Last().Species;

            int index = Math.Max(0, Array.FindIndex(vs, p => p.Species == minspec));
            Array.Resize(ref vs, index + 1);
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
            return t.GetType().Name;
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
        private static IEnumerable<EncounterArea> getEncounterSlots(PKM pkm, int lvl = -1)
        {
            switch (pkm.Version)
            {
                case (int)GameVersion.X:
                    return getSlots(pkm, SlotsX, lvl);
                case (int)GameVersion.Y:
                    return getSlots(pkm, SlotsY, lvl);
                case (int)GameVersion.AS:
                    return getSlots(pkm, SlotsA, lvl);
                case (int)GameVersion.OR:
                    return getSlots(pkm, SlotsO, lvl);
                case (int)GameVersion.SN:
                    return getSlots(pkm, SlotsSN, lvl);
                case (int)GameVersion.MN:
                    return getSlots(pkm, SlotsMN, lvl);
                default: return new List<EncounterArea>();
            }
        }
        private static IEnumerable<EncounterStatic> getStaticEncounters(PKM pkm, int lvl = -1)
        {
            switch (pkm.Version)
            {
                case (int)GameVersion.X:
                    return getStatic(pkm, StaticX, lvl);
                case (int)GameVersion.Y:
                    return getStatic(pkm, StaticY, lvl);
                case (int)GameVersion.AS:
                    return getStatic(pkm, StaticA, lvl);
                case (int)GameVersion.OR:
                    return getStatic(pkm, StaticO, lvl);
                case (int)GameVersion.SN:
                    return getStatic(pkm, StaticSN, lvl);
                case (int)GameVersion.MN:
                    return getStatic(pkm, StaticMN, lvl);
                default: return new List<EncounterStatic>();
            }
        }
        private static IEnumerable<EncounterArea> getEncounterAreas(PKM pkm)
        {
            return getEncounterSlots(pkm).Where(l => l.Location == pkm.Met_Location);
        }
        private static IEnumerable<EncounterSlot> getValidEncounterSlots(PKM pkm, EncounterArea loc, bool DexNav, bool ignoreLevel = false)
        {
            const int fluteBoost = 4;
            const int dexnavBoost = 30;

            int df = DexNav ? fluteBoost : 0;
            int dn = DexNav ? fluteBoost + dexnavBoost : 0;
            List<EncounterSlot> slotdata = new List<EncounterSlot>();

            // Get Valid levels
            IEnumerable<DexLevel> vs = getValidPreEvolutions(pkm);
            // Get slots where pokemon can exist
            IEnumerable<EncounterSlot> slots = loc.Slots.Where(slot => vs.Any(evo => evo.Species == slot.Species && evo.Level >= slot.LevelMin - df) || ignoreLevel);

            // Filter for Met Level
            int lvl = pkm.Met_Level;
            var encounterSlots = slots.Where(slot => slot.LevelMin - df <= lvl && lvl <= slot.LevelMax + (slot.AllowDexNav ? dn : df) || ignoreLevel).ToList();

            // Pressure Slot
            EncounterSlot slotMax = encounterSlots.OrderByDescending(slot => slot.LevelMax).FirstOrDefault();
            if (slotMax != null)
                slotMax = new EncounterSlot(slotMax) { Pressure = true, Form = pkm.AltForm };

            if (!DexNav)
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
            if (slotMax != null)
                eslots.Add(slotMax);
            foreach (EncounterSlot s in eslots)
            {
                bool nav = s.AllowDexNav && (pkm.RelearnMove1 != 0 || pkm.AbilityNumber == 4);
                EncounterSlot slot = new EncounterSlot(s) { DexNav = nav };

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
                    new DexLevel { Species = pkm.Species, Level = 1 },
                };
            if (pkm.Species == 292 && pkm.Met_Level + 1 <= lvl && lvl >= 20)
                return new List<DexLevel>
                {
                    new DexLevel { Species = 292, Level = lvl },
                    new DexLevel { Species = 290, Level = lvl-1 }
                };

            var et = getEvolutionTable(pkm);
            return et.getValidPreEvolutions(pkm, lvl, skipChecks: skipChecks);
        }
        private static IEnumerable<EncounterStatic> getStatic(PKM pkm, IEnumerable<EncounterStatic> table, int lvl = -1)
        {
            IEnumerable<DexLevel> dl = getValidPreEvolutions(pkm, lvl);
            return table.Where(e => dl.Any(d => d.Species == e.Species));
        }
        private static IEnumerable<int> getValidMoves(PKM pkm, GameVersion Version, IEnumerable<DexLevel> vs, bool LVL = false, bool Relearn = false, bool Tutor = false, bool Machine = false, bool MoveReminder = true)
        {
            List<int> r = new List<int> { 0 };
            int species = pkm.Species;
            int lvl = pkm.CurrentLevel;

            // Special Type Tutors Availability
            bool moveTutor = Tutor || MoveReminder; // Usually true, except when called for move suggestions (no tutored moves)
            
            if (FormChangeMoves.Contains(species)) // Deoxys & Shaymin & Giratina (others don't have extra but whatever)
            {
                int formcount = pkm.PersonalInfo.FormeCount;
                for (int i = 0; i < formcount; i++)
                    r.AddRange(getMoves(pkm, species, lvl, i, moveTutor, Version, LVL, Tutor, Machine, MoveReminder));
                if (Relearn) r.AddRange(pkm.RelearnMoves);
                return r.Distinct().ToArray();
            }

            r.AddRange(getMoves(pkm, species, lvl, pkm.AltForm, moveTutor, Version, LVL, Tutor, Machine, MoveReminder));

            foreach (DexLevel evo in vs)
                r.AddRange(getMoves(pkm, evo.Species, evo.Level, pkm.AltForm, moveTutor, Version, LVL, Tutor, Machine, MoveReminder));

            if (species == 479) // Rotom
                r.Add(RotomMoves[pkm.AltForm]);
            if (species == 648) // Meloetta
                r.Add(547); // Relic Song

            if (species == 25 && pkm.Format == 6 && pkm.GenNumber == 6) // Pikachu
                r.Add(PikachuMoves[pkm.AltForm]);

            if (species == 718 && pkm.GenNumber == 7) // Zygarde
                r.AddRange(ZygardeMoves);
            if (species == 25 || species == 26 && pkm.Format == 7) // Pikachu/Raichu Tutor
                r.Add(344); // Volt Tackle

            if (Relearn) r.AddRange(pkm.RelearnMoves);
            return r.Distinct().ToArray();
        }
        private static IEnumerable<int> getMoves(PKM pkm, int species, int lvl, int form, bool moveTutor, GameVersion Version, bool LVL, bool specialTutors, bool Machine, bool MoveReminder)
        {
            List<int> r = new List<int> { 0 };
            for (int gen = pkm.GenNumber; gen <= pkm.Format; gen++)
               r.AddRange(getMoves(pkm, species, lvl, form, moveTutor, Version, LVL, specialTutors, Machine, gen, MoveReminder));
            return r.Distinct();
        }
        private static IEnumerable<int> getMoves(PKM pkm, int species, int lvl, int form, bool moveTutor, GameVersion Version, bool LVL, bool specialTutors, bool Machine, int Generation, bool MoveReminder)
        {
            List<int> r = new List<int>();

            var ver = Version;
            switch (Generation)
            {
                case 6:
                    switch (ver)
                    {
                        case GameVersion.Any: // Start at the top, hit every table
                        case GameVersion.X: case GameVersion.Y: case GameVersion.XY:
                        {
                            int index = PersonalTable.XY.getFormeIndex(species, form);
                            PersonalInfo pi = PersonalTable.XY.getFormeEntry(species, form);

                            if (LVL) r.AddRange(LevelUpXY[index].getMoves(lvl));
                            if (moveTutor) r.AddRange(getTutorMoves(pkm, species, form, specialTutors));
                            if (Machine) r.AddRange(TMHM_XY.Where((t, m) => pi.TMHM[m]));

                            if (ver == GameVersion.Any) // Fall Through
                                goto case GameVersion.ORAS;
                            break;
                        }

                        case GameVersion.AS: case GameVersion.OR: case GameVersion.ORAS:
                        {
                            int index = PersonalTable.AO.getFormeIndex(species, form);
                            PersonalInfo pi = PersonalTable.AO.getFormeEntry(species, form);

                            if (LVL) r.AddRange(LevelUpAO[index].getMoves(lvl));
                            if (moveTutor) r.AddRange(getTutorMoves(pkm, species, form, specialTutors));
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
                            if (moveTutor) r.AddRange(getTutorMoves(pkm, species, form, specialTutors));
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
        private static IEnumerable<int> getEggMoves(PKM pkm, int species, int formnum)
        {
            switch (pkm.GenNumber)
            {
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
        private static IEnumerable<int> getTutorMoves(PKM pkm, int species, int form, bool specialTutors)
        {
            PersonalInfo info = pkm.PersonalInfo;
            List<int> moves = new List<int>();

            // Type Tutors -- Pledge moves and High BP moves switched places in G7+
            if (pkm.Format <= 6)
                moves.AddRange(TypeTutor6.Where((t, i) => info.TypeTutors[i]));
            else if (pkm.Format >= 7)
                moves.AddRange(TypeTutor7.Where((t, i) => info.TypeTutors[i]));

            // Varied Tutors
            //if (pkm.InhabitedGeneration(5) && Tutors)
            //{
            //    //PersonalInfo pi = PersonalTable.B2W2.getFormeEntry(species, form);
            //    //for (int i = 0; i < Tutors_B2W2.Length; i++)
            //    //    for (int b = 0; b < Tutors_B2W2[i].Length; b++)
            //    //        if (pi.SpecialTutors[i][b])
            //    //            moves.Add(Tutors_B2W2[i][b]);
            //}
            if (pkm.InhabitedGeneration(6) && specialTutors && (pkm.AO || !pkm.IsUntraded))
            {
                PersonalInfo pi = PersonalTable.AO.getFormeEntry(species, form);
                for (int i = 0; i < Tutors_AO.Length; i++)
                    for (int b = 0; b < Tutors_AO[i].Length; b++)
                        if (pi.SpecialTutors[i][b])
                            moves.Add(Tutors_AO[i][b]);
            }
            // No tutors in G7

            // Keldeo - Secret Sword
            if (species == 647)
                moves.Add(548);
            return moves.Distinct();
        }
    }
}
