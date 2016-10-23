using System.Collections.Generic;
using System.Linq;

namespace PKHeX
{
    public static partial class Legal
    {
        // PKHeX master Wonder Card Database
        internal static WC6[] WC6DB;
        // PKHeX master personal.dat

        private static readonly EggMoves[] EggMoveXY = EggMoves.getArray(Data.unpackMini(Properties.Resources.eggmove_xy, "xy"));
        private static readonly Learnset[] LevelUpXY = Learnset.getArray(Data.unpackMini(Properties.Resources.lvlmove_xy, "xy"));
        private static readonly EggMoves[] EggMoveAO = EggMoves.getArray(Data.unpackMini(Properties.Resources.eggmove_ao, "ao"));
        private static readonly Learnset[] LevelUpAO = Learnset.getArray(Data.unpackMini(Properties.Resources.lvlmove_ao, "ao"));
        private static readonly Evolutions[] Evolves = Evolutions.getArray(Data.unpackMini(Properties.Resources.evos_ao, "ao"));
        private static readonly EncounterArea[] SlotsA;
        private static readonly EncounterArea[] SlotsO;
        private static readonly EncounterArea[] SlotsX;
        private static readonly EncounterArea[] SlotsY;
        private static readonly EncounterStatic[] StaticX;
        private static readonly EncounterStatic[] StaticY;
        private static readonly EncounterStatic[] StaticA;
        private static readonly EncounterStatic[] StaticO;
        private static EncounterStatic[] getSpecial(GameVersion Game)
        {
            EncounterStatic[] table = null;
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
            }
            return table?.Where(s => s.Version == GameVersion.Any || s.Version == Game).ToArray();
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

        static Legal() // Setup
        {
            #region Gen6: XY & ORAS
            StaticX = getSpecial(GameVersion.X);
            StaticY = getSpecial(GameVersion.Y);
            StaticA = getSpecial(GameVersion.AS);
            StaticO = getSpecial(GameVersion.OR);

            var XSlots = EncounterArea.getArray(Data.unpackMini(Properties.Resources.encounter_x, "xy"));
            var YSlots = EncounterArea.getArray(Data.unpackMini(Properties.Resources.encounter_y, "xy"));

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

            SlotsA = EncounterArea.getArray(Data.unpackMini(Properties.Resources.encounter_a, "ao"));
            SlotsO = EncounterArea.getArray(Data.unpackMini(Properties.Resources.encounter_o, "ao"));

            // Mark Encounters
            foreach (var area in SlotsA)
            {
                for (int i = 32; i < 37; i++)
                    area.Slots[i].Type = SlotType.Rock_Smash;
                int slotct = area.Slots.Length;
                for (int i = slotct - 15; i < slotct; i++)
                    area.Slots[i].Type = SlotType.Horde;

                for (int i = 0; i < slotct; i++)
                    area.Slots[i].AllowDexNav = area.Slots[i].Type != SlotType.Rock_Smash;
            }
            foreach (var area in SlotsO)
            {
                for (int i = 32; i < 37; i++)
                    area.Slots[i].Type = SlotType.Rock_Smash;
                int slotct = area.Slots.Length;
                for (int i = slotct - 15; i < slotct; i++)
                    area.Slots[i].Type = SlotType.Horde;

                for (int i = 0; i < slotct; i++)
                    area.Slots[i].AllowDexNav = area.Slots[i].Type != SlotType.Rock_Smash;
            }
            #endregion
        }

        internal static IEnumerable<int> getValidMoves(PKM pkm)
        { return getValidMoves(pkm, -1, LVL: true, Relearn: false, Tutor: true, Machine: true); }
        internal static IEnumerable<int> getValidRelearn(PKM pkm, int skipOption)
        {
            List<int> r = new List<int> { 0 };
            int species = getBaseSpecies(pkm, skipOption);
            r.AddRange(getLVLMoves(species, 1, pkm.AltForm));
            r.AddRange(getEggMoves(species, pkm.Species == 678 ? pkm.AltForm : 0));
            r.AddRange(getLVLMoves(species, 100, pkm.AltForm));
            return r.Distinct();
        }
        internal static IEnumerable<int> getBaseEggMoves(PKM pkm, int skipOption, int gameSource)
        {
            int species = getBaseSpecies(pkm, skipOption);
            if (gameSource == -1)
            {
                if (pkm.XY)
                    return LevelUpXY[species].getMoves(1);
                if (pkm.AO)
                    return LevelUpAO[species].getMoves(1);
                return null;
            }
            if (gameSource == 0) // XY
                return LevelUpXY[species].getMoves(1);
            // if (gameSource == 1) // ORAS
            return LevelUpAO[species].getMoves(1);
        }

        internal static IEnumerable<MysteryGift> getValidWC6s(PKM pkm)
        {
            var vs = getValidPreEvolutions(pkm).ToArray();
            List<MysteryGift> validWC6 = new List<MysteryGift>();

            foreach (WC6 wc in WC6DB.Where(wc => vs.Any(dl => dl.Species == wc.Species)))
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
                if (wc.Form != pkm.AltForm && vs.All(dl => !FormChange.Contains(dl.Species))) continue;
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
        internal static EncounterLink getValidLinkGifts(PKM pkm)
        {
            return LinkGifts.FirstOrDefault(g => g.Species == pkm.Species && g.Level == pkm.Met_Level);
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
            if (lang == 0)
                return null;

            // Get valid pre-evolutions
            IEnumerable<DexLevel> p = getValidPreEvolutions(pkm);
            EncounterTrade z = null;
            if (pkm.XY)
                z = lang == 6 ? null : TradeGift_XY.FirstOrDefault(f => p.Any(r => r.Species == f.Species));
            if (pkm.AO)
                z = lang == 6 ? null : TradeGift_AO.FirstOrDefault(f => p.Any(r => r.Species == f.Species));

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

        internal static bool getDexNavValid(PKM pkm)
        {
            IEnumerable<EncounterArea> locs = getDexNavAreas(pkm);
            return locs.Select(loc => getValidEncounterSlots(pkm, loc, DexNav: true)).Any(slots => slots.Any(slot => slot.AllowDexNav && slot.DexNav));
        }
        internal static bool getHasEvolved(PKM pkm)
        {
            return getValidPreEvolutions(pkm).Count() > 1;
        }
        internal static bool getHasTradeEvolved(PKM pkm)
        {
            return Evolves[pkm.Species].Evos.Any(evo => evo.Level == 1); // 1: Trade, 0: Item, >=2: Levelup
        }
        internal static bool getIsFossil(PKM pkm)
        {
            if (pkm.Met_Level != 20)
                return false;
            if (pkm.Egg_Location != 0)
                return false;
            if (pkm.XY && pkm.Met_Location == 44)
                return Fossils.Contains(getBaseSpecies(pkm));
            if (pkm.AO && pkm.Met_Location == 190)
                return Fossils.Contains(getBaseSpecies(pkm));

            return false;
        }
        internal static bool getEvolutionValid(PKM pkm)
        {
            var curr = getValidPreEvolutions(pkm);
            var poss = getValidPreEvolutions(pkm, 100);

            if (SplitBreed.Contains(getBaseSpecies(pkm, 1)))
                return curr.Count() >= poss.Count() - 1;
            return curr.Count() >= poss.Count();
        }
        internal static IEnumerable<int> getLineage(PKM pkm)
        {
            int species = pkm.Species;
            List<int> res = new List<int>{species};
            for (int i = 0; i < Evolves.Length; i++)
                if (Evolves[i].Evos.Any(pk => pk.Species == species))
                    res.Add(i);
            for (int i = -1; i < 2; i++)
                res.Add(getBaseSpecies(pkm, i));
            return res.Distinct();
        }

        internal static bool getCanBeCaptured(int species, int version = -1)
        {
            if (version < 0 || version == (int)GameVersion.X)
            {
                if (SlotsX.Any(loc => loc.Slots.Any(slot => slot.Species == species)))
                    return true;
                if (FriendSafari.Contains(species))
                    return true;
                if (StaticX.Any(enc => enc.Species == species && !enc.Gift))
                    return true;
            }
            if (version < 0 || version == (int)GameVersion.Y)
            {
                if (SlotsY.Any(loc => loc.Slots.Any(slot => slot.Species == species)))
                    return true;
                if (FriendSafari.Contains(species))
                    return true;
                if (StaticY.Any(enc => enc.Species == species && !enc.Gift))
                    return true;
            }
            if (version < 0 || version == (int)GameVersion.AS)
            {
                if (SlotsA.Any(loc => loc.Slots.Any(slot => slot.Species == species)))
                    return true;
                if (StaticA.Any(enc => enc.Species == species && !enc.Gift))
                    return true;
            }
            if (version < 0 || version == (int)GameVersion.OR)
            {
                if (SlotsO.Any(loc => loc.Slots.Any(slot => slot.Species == species)))
                    return true;
                if (StaticO.Any(enc => enc.Species == species && !enc.Gift))
                    return true;
            }
            return false;
        }
        internal static bool getCanLearnMachineMove(PKM pkm, int move, int version = -1)
        {
            return getValidMoves(pkm, version, Machine: true).Contains(move);
        }
        internal static bool getCanRelearnMove(PKM pkm, int move, int version = -1)
        {
            return getValidMoves(pkm, version, LVL: true, Relearn: true).Contains(move);
        }
        internal static bool getCanLearnMove(PKM pkm, int move, int version = -1)
        {
            return getValidMoves(pkm, version, Tutor: true, Machine: true).Contains(move);
        }
        internal static bool getCanKnowMove(PKM pkm, int move, int version = -1)
        {
            if (pkm.Species == 235 && !InvalidSketch.Contains(move))
                return true;
            return getValidMoves(pkm, Version: version, LVL: true, Relearn: true, Tutor: true, Machine: true).Contains(move);
        }

        private static int getBaseSpecies(PKM pkm, int skipOption = 0)
        {
            if (pkm.Species == 292)
                return 290;
            if (pkm.Species == 242 && pkm.CurrentLevel < 3) // Never Cleffa
                return 113;
            DexLevel[] evos = Evolves[pkm.Species].Evos;
            switch (skipOption)
            {
                case -1: return pkm.Species;
                case 1: return evos.Length <= 1 ? pkm.Species : evos[evos.Length - 2].Species;
                default: return evos.Length <= 0 ? pkm.Species : evos.Last().Species;
            }
        }
        private static IEnumerable<EncounterArea> getDexNavAreas(PKM pkm)
        {
            bool alpha = pkm.Version == 26;
            if (!alpha && pkm.Version != 27)
                return new EncounterArea[0];
            return (alpha ? SlotsA : SlotsO).Where(l => l.Location == pkm.Met_Location);
        }
        private static IEnumerable<int> getLVLMoves(int species, int lvl, int formnum)
        {
            int ind_XY = PersonalTable.XY.getFormeIndex(species, formnum);
            int ind_AO = PersonalTable.AO.getFormeIndex(species, formnum);
            return LevelUpXY[ind_XY].getMoves(lvl).Concat(LevelUpAO[ind_AO].getMoves(lvl));
        }
        private static IEnumerable<EncounterArea> getEncounterSlots(PKM pkm)
        {
            switch (pkm.Version)
            {
                case 24: // X
                    return getSlots(pkm, SlotsX);
                case 25: // Y
                    return getSlots(pkm, SlotsY);
                case 26: // AS
                    return getSlots(pkm, SlotsA);
                case 27: // OR
                    return getSlots(pkm, SlotsO);
                default: return new List<EncounterArea>();
            }
        }
        private static IEnumerable<EncounterStatic> getStaticEncounters(PKM pkm)
        {
            switch (pkm.Version)
            {
                case 24: // X
                    return getStatic(pkm, StaticX);
                case 25: // Y
                    return getStatic(pkm, StaticY);
                case 26: // AS
                    return getStatic(pkm, StaticA);
                case 27: // OR
                    return getStatic(pkm, StaticO);
                default: return new List<EncounterStatic>();
            }
        }
        private static IEnumerable<EncounterArea> getEncounterAreas(PKM pkm)
        {
            return getEncounterSlots(pkm).Where(l => l.Location == pkm.Met_Location);
        }
        private static IEnumerable<EncounterSlot> getValidEncounterSlots(PKM pkm, EncounterArea loc, bool DexNav)
        {
            const int fluteBoost = 4;
            const int dexnavBoost = 30;
            int df = DexNav ? fluteBoost : 0;
            int dn = DexNav ? fluteBoost + dexnavBoost : 0;
            List<EncounterSlot> slotdata = new List<EncounterSlot>();

            // Get Valid levels
            IEnumerable<DexLevel> vs = getValidPreEvolutions(pkm);
            // Get slots where pokemon can exist
            IEnumerable<EncounterSlot> slots = loc.Slots.Where(slot => vs.Any(evo => evo.Species == slot.Species && evo.Level >= slot.LevelMin - df));

            // Filter for Met Level
            int lvl = pkm.Met_Level;
            var encounterSlots = slots.Where(slot => slot.LevelMin - df <= lvl && lvl <= slot.LevelMax + (slot.AllowDexNav ? dn : df)).ToList();

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
        private static IEnumerable<EncounterArea> getSlots(PKM pkm, IEnumerable<EncounterArea> tables)
        {
            IEnumerable<DexLevel> vs = getValidPreEvolutions(pkm);
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
        private static IEnumerable<DexLevel> getValidPreEvolutions(PKM pkm, int lvl = -1)
        {
            if (lvl < 0)
                lvl = pkm.CurrentLevel;
            if (pkm.Species == 292 && pkm.Met_Level + 1 <= lvl && lvl >= 20)
                return new List<DexLevel>
                {
                    new DexLevel { Species = 292, Level = lvl },
                    new DexLevel { Species = 290, Level = lvl-1 }
                };
            var evos = Evolves[pkm.Species].Evos;
            List<DexLevel> dl = new List<DexLevel> { new DexLevel { Species = pkm.Species, Level = lvl } };
            foreach (DexLevel evo in evos)
            {
                if (lvl >= pkm.Met_Level && lvl >= evo.Level)
                    dl.Add(new DexLevel {Species = evo.Species, Level = lvl});
                else break;
                if (evo.Level > 2) // Level Up (from previous level)
                    lvl--;
            }
            return dl;
        }
        private static IEnumerable<EncounterStatic> getStatic(PKM pkm, IEnumerable<EncounterStatic> table)
        {
            IEnumerable<DexLevel> dl = getValidPreEvolutions(pkm);
            return table.Where(e => dl.Any(d => d.Species == e.Species));
        }
        private static IEnumerable<int> getValidMoves(PKM pkm, int Version, bool LVL = false, bool Relearn = false, bool Tutor = false, bool Machine = false)
        {
            List<int> r = new List<int> { 0 };
            int species = pkm.Species;
            int lvl = pkm.CurrentLevel;
            bool ORASTutors = Version == -1 || pkm.AO || !pkm.IsUntraded;
            if (FormChangeMoves.Contains(species)) // Deoxys & Shaymin & Giratina (others don't have extra but whatever)
            {
                int formcount = PersonalTable.AO[species].FormeCount;
                for (int i = 0; i < formcount; i++)
                    r.AddRange(getMoves(species, lvl, i, ORASTutors, Version, LVL, Tutor, Machine));
                if (Relearn) r.AddRange(pkm.RelearnMoves);
                return r.Distinct().ToArray();
            }

            r.AddRange(getMoves(species, lvl, pkm.AltForm, ORASTutors, Version, LVL, Tutor, Machine));
            IEnumerable<DexLevel> vs = getValidPreEvolutions(pkm);

            foreach (DexLevel evo in vs)
                r.AddRange(getMoves(evo.Species, evo.Level, pkm.AltForm, ORASTutors, Version, LVL, Tutor, Machine));
            if (species == 479) // Rotom
                r.Add(RotomMoves[pkm.AltForm]);
            if (species == 25) // Pikachu
                r.Add(PikachuMoves[pkm.AltForm]);

            if (Relearn) r.AddRange(pkm.RelearnMoves);
            return r.Distinct().ToArray();
        }
        private static IEnumerable<int> getMoves(int species, int lvl, int form, bool ORASTutors, int Version, bool LVL, bool Tutor, bool Machine)
        {
            List<int> r = new List<int> { 0 };
            if (Version < 0 || Version == 0)
            {
                int index = PersonalTable.XY.getFormeIndex(species, form);
                PersonalInfo pi = PersonalTable.XY.getFormeEntry(species, form);

                if (LVL) r.AddRange(LevelUpXY[index].getMoves(lvl));
                if (Tutor) r.AddRange(getTutorMoves(species, form, ORASTutors));
                if (Machine) r.AddRange(TMHM_XY.Where((t, m) => pi.TMHM[m]));
            }
            if (Version < 0 || Version == 1)
            {
                int index = PersonalTable.AO.getFormeIndex(species, form);
                PersonalInfo pi = PersonalTable.AO.getFormeEntry(species, form);

                if (LVL) r.AddRange(LevelUpAO[index].getMoves(lvl));
                if (Tutor) r.AddRange(getTutorMoves(species, form, ORASTutors));
                if (Machine) r.AddRange(TMHM_AO.Where((t, m) => pi.TMHM[m]));
            }
            return r;
        }
        private static IEnumerable<int> getEggMoves(int species, int formnum)
        {
            int ind_XY = PersonalTable.XY.getFormeIndex(species, formnum);
            int ind_AO = PersonalTable.AO.getFormeIndex(species, formnum);
            return EggMoveAO[ind_AO].Moves.Concat(EggMoveXY[ind_XY].Moves);
        }
        private static IEnumerable<int> getTutorMoves(int species, int formnum, bool ORASTutors)
        {
            PersonalInfo pkAO = PersonalTable.AO.getFormeEntry(species, formnum);

            // Type Tutor
            List<int> moves = TypeTutor.Where((t, i) => pkAO.TypeTutors[i]).ToList();

            // Varied Tutors
            if (ORASTutors)
            for (int i = 0; i < Tutors_AO.Length; i++)
                for (int b = 0; b < Tutors_AO[i].Length; b++)
                    if (pkAO.SpecialTutors[i][b])
                        moves.Add(Tutors_AO[i][b]);

            // Keldeo - Secret Sword
            if (species == 647)
                moves.Add(548);
            return moves;
        }
    }
}
