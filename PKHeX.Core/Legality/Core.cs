using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static PKHeX.Core.LegalityCheckStrings;

namespace PKHeX.Core
{
    public static partial class Legal
    {
        /// <summary>Event Database for a given Generation</summary>
        public static MysteryGift[] MGDB_G3, MGDB_G4, MGDB_G5, MGDB_G6, MGDB_G7 = new MysteryGift[0];

        /// <summary>Setting to specify if an analysis should permit data sourced from the physical cartridge era of GameBoy games.</summary>
        public static bool AllowGBCartEra = false;
        public static bool AllowGen1Tradeback = false;
        public static bool AllowGen2VCTransfer => AllowGen1Tradeback;

        /// <summary>Setting to specify if the e-berry index item is an eningma berry or a e-reader berry and the name of the e-reader berry</summary>
        public static bool EReaderBerryIsEnigma = true;
        public static string EReaderBerryName = string.Empty;
        public static string EReaderBerryDisplayName => string.Format(V372, Util.toTitleCase(EReaderBerryName.ToLower()));
        public static bool SavegameJapanese = false;
        public static string Savegame_OT = string.Empty;
        public static int Savegame_TID = 0;
        public static int Savegame_SID = 0;
        public static int Savegame_Gender = 0;
        public static GameVersion Savegame_Version = GameVersion.Any;

        // Gen 1
        private static readonly Learnset[] LevelUpRB = Learnset1.getArray(Util.getBinaryResource("lvlmove_rb.pkl"), MaxSpeciesID_1);
        private static readonly Learnset[] LevelUpY = Learnset1.getArray(Util.getBinaryResource("lvlmove_y.pkl"), MaxSpeciesID_1);
        private static readonly EvolutionTree Evolves1;
        private static readonly EncounterArea[] SlotsRBY;
        private static readonly EncounterStatic[] StaticRBY;

        // Gen 2
        private static readonly EggMoves[] EggMovesGS = EggMoves2.getArray(Util.getBinaryResource("eggmove_gs.pkl"), MaxSpeciesID_2);
        private static readonly Learnset[] LevelUpGS = Learnset1.getArray(Util.getBinaryResource("lvlmove_gs.pkl"), MaxSpeciesID_2);
        private static readonly EggMoves[] EggMovesC = EggMoves2.getArray(Util.getBinaryResource("eggmove_c.pkl"), MaxSpeciesID_2);
        private static readonly Learnset[] LevelUpC = Learnset1.getArray(Util.getBinaryResource("lvlmove_c.pkl"), MaxSpeciesID_2);
        private static readonly EvolutionTree Evolves2;
        private static readonly EncounterArea[] SlotsGSC, SlotsGS, SlotsC;
        private static readonly EncounterStatic[] StaticGSC, StaticGS, StaticC;

        // Gen 3
        private static readonly Learnset[] LevelUpE = Learnset6.getArray(Data.unpackMini(Util.getBinaryResource("lvlmove_e.pkl"), "em"));
        private static readonly Learnset[] LevelUpRS = Learnset6.getArray(Data.unpackMini(Util.getBinaryResource("lvlmove_rs.pkl"), "rs"));
        private static readonly Learnset[] LevelUpFR = Learnset6.getArray(Data.unpackMini(Util.getBinaryResource("lvlmove_fr.pkl"), "fr"));
        private static readonly Learnset[] LevelUpLG = Learnset6.getArray(Data.unpackMini(Util.getBinaryResource("lvlmove_lg.pkl"), "lg"));
        private static readonly EggMoves[] EggMovesRS = EggMoves6.getArray(Data.unpackMini(Util.getBinaryResource("eggmove_rs.pkl"), "rs"));
        private static readonly EvolutionTree Evolves3;
        private static readonly EncounterArea[] SlotsR, SlotsS, SlotsE, SlotsFR, SlotsLG;
        private static readonly EncounterStatic[] StaticR, StaticS, StaticE, StaticFR, StaticLG;

        // Gen 4
        private static readonly Learnset[] LevelUpDP = Learnset6.getArray(Data.unpackMini(Util.getBinaryResource("lvlmove_dp.pkl"), "dp"));
        private static readonly Learnset[] LevelUpPt = Learnset6.getArray(Data.unpackMini(Util.getBinaryResource("lvlmove_pt.pkl"), "pt"));
        private static readonly Learnset[] LevelUpHGSS = Learnset6.getArray(Data.unpackMini(Util.getBinaryResource("lvlmove_hgss.pkl"), "hs"));
        private static readonly EggMoves[] EggMovesDPPt = EggMoves6.getArray(Data.unpackMini(Util.getBinaryResource("eggmove_dppt.pkl"), "dp"));
        private static readonly EggMoves[] EggMovesHGSS = EggMoves6.getArray(Data.unpackMini(Util.getBinaryResource("eggmove_hgss.pkl"), "hs"));
        private static readonly EvolutionTree Evolves4;
        private static readonly EncounterArea[] SlotsD, SlotsP, SlotsPt, SlotsHG, SlotsSS;
        private static readonly EncounterStatic[] StaticD, StaticP, StaticPt, StaticHG, StaticSS;

        // Gen 5
        private static readonly Learnset[] LevelUpBW = Learnset6.getArray(Data.unpackMini(Util.getBinaryResource("lvlmove_bw.pkl"), "51"));
        private static readonly Learnset[] LevelUpB2W2 = Learnset6.getArray(Data.unpackMini(Util.getBinaryResource("lvlmove_b2w2.pkl"), "52"));
        private static readonly EggMoves[] EggMovesBW = EggMoves6.getArray(Data.unpackMini(Util.getBinaryResource("eggmove_bw.pkl"), "bw"));
        private static readonly EvolutionTree Evolves5;
        private static readonly EncounterArea[] SlotsB, SlotsW, SlotsB2, SlotsW2;
        private static readonly EncounterStatic[] StaticB, StaticW, StaticB2, StaticW2;

        // Gen 6
        private static readonly EggMoves[] EggMovesXY = EggMoves6.getArray(Data.unpackMini(Util.getBinaryResource("eggmove_xy.pkl"), "xy"));
        private static readonly Learnset[] LevelUpXY = Learnset6.getArray(Data.unpackMini(Util.getBinaryResource("lvlmove_xy.pkl"), "xy"));
        private static readonly EggMoves[] EggMovesAO = EggMoves6.getArray(Data.unpackMini(Util.getBinaryResource("eggmove_ao.pkl"), "ao"));
        private static readonly Learnset[] LevelUpAO = Learnset6.getArray(Data.unpackMini(Util.getBinaryResource("lvlmove_ao.pkl"), "ao"));
        private static readonly EvolutionTree Evolves6;
        private static readonly EncounterArea[] SlotsX, SlotsY, SlotsA, SlotsO;
        private static readonly EncounterStatic[] StaticX, StaticY, StaticA, StaticO;

        // Gen 7
        private static readonly EggMoves[] EggMovesSM = EggMoves7.getArray(Data.unpackMini(Util.getBinaryResource("eggmove_sm.pkl"), "sm"));
        private static readonly Learnset[] LevelUpSM = Learnset7.getArray(Data.unpackMini(Util.getBinaryResource("lvlmove_sm.pkl"), "sm"));
        private static readonly EvolutionTree Evolves7;
        private static readonly EncounterArea[] SlotsSN, SlotsMN;
        private static readonly EncounterStatic[] StaticSN, StaticMN;

        // Setup Help
        private static HashSet<MysteryGift> getPCDDB(byte[] bin)
        {
            var db = new HashSet<MysteryGift>();
            for (int i = 0; i < bin.Length; i += PCD.Size)
            {
                byte[] data = new byte[PCD.Size];
                Buffer.BlockCopy(bin, i, data, 0, PCD.Size);
                db.Add(new PCD(data));
            }
            return db;
        }
        private static HashSet<MysteryGift> getPGFDB(byte[] bin)
        {
            var db = new HashSet<MysteryGift>();
            for (int i = 0; i < bin.Length; i += PGF.Size)
            {
                byte[] data = new byte[PGF.Size];
                Buffer.BlockCopy(bin, i, data, 0, PGF.Size);
                db.Add(new PGF(data));
            }
            return db;
        }
        private static HashSet<MysteryGift> getWC6DB(byte[] wc6bin, byte[] wc6full)
        {
            var db = new HashSet<MysteryGift>();
            for (int i = 0; i < wc6bin.Length; i += WC6.Size)
            {
                byte[] data = new byte[WC6.Size];
                Buffer.BlockCopy(wc6bin, i, data, 0, WC6.Size);
                db.Add(new WC6(data));
            }
            for (int i = 0; i < wc6full.Length; i += WC6.SizeFull)
            {
                byte[] data = new byte[WC6.SizeFull];
                Buffer.BlockCopy(wc6full, i, data, 0, WC6.SizeFull);
                db.Add(new WC6(data));
            }
            return db;
        }
        private static HashSet<MysteryGift> getWC7DB(byte[] wc7bin, byte[] wc7full)
        {
            var db = new HashSet<MysteryGift>();
            for (int i = 0; i < wc7bin.Length; i += WC7.Size)
            {
                byte[] data = new byte[WC7.Size];
                Buffer.BlockCopy(wc7bin, i, data, 0, WC7.Size);
                db.Add(new WC7(data));
            }
            for (int i = 0; i < wc7full.Length; i += WC7.SizeFull)
            {
                byte[] data = new byte[WC7.SizeFull];
                Buffer.BlockCopy(wc7full, i, data, 0, WC7.SizeFull);
                db.Add(new WC7(data));
            }
            return db;
        }
        public static void RefreshMGDB(string localDbPath)
        {
            var g4 = getPCDDB(Util.getBinaryResource("pcd.pkl"));
            var g5 = getPGFDB(Util.getBinaryResource("pgf.pkl"));
            var g6 = getWC6DB(Util.getBinaryResource("wc6.pkl"), Util.getBinaryResource("wc6full.pkl"));
            var g7 = getWC7DB(Util.getBinaryResource("wc7.pkl"), Util.getBinaryResource("wc7full.pkl"));

            if (Directory.Exists(localDbPath))
                foreach (var file in Directory.GetFiles(localDbPath, "*", SearchOption.AllDirectories))
                {
                    var fi = new FileInfo(file);
                    if (!MysteryGift.getIsMysteryGift(fi.Length))
                        continue;

                    var gift = MysteryGift.getMysteryGift(File.ReadAllBytes(file), fi.Extension);
                    switch (gift?.Format)
                    {
                        case 4: g4.Add(gift); continue;
                        case 5: g5.Add(gift); continue;
                        case 6: g6.Add(gift); continue;
                        case 7: g7.Add(gift); continue;
                    }
                }

            MGDB_G3 = Encounter_WC3; // hardcoded
            MGDB_G4 = g4.ToArray();
            MGDB_G5 = g5.ToArray();
            MGDB_G6 = g6.ToArray();
            MGDB_G7 = g7.ToArray();
        }

        private static EncounterStatic[] getStaticEncounters(GameVersion Game)
        {
            EncounterStatic[] table;
            switch (Game)
            {
                case GameVersion.RBY:
                    return Encounter_RBY; // GameVersion filtering not possible, return immediately
                case GameVersion.GS:
                    return Encounter_GS;
                case GameVersion.C:
                    return Encounter_C;
                case GameVersion.GSC:
                    return Encounter_GSC;

                case GameVersion.R: case GameVersion.S: case GameVersion.E:
                    table = Encounter_RSE;
                    break;
                case GameVersion.FR: case GameVersion.LG:
                    table = Encounter_FRLG;
                    break;
                case GameVersion.CXD:
                    table = Encounter_CXD;
                    break;

                case GameVersion.D: case GameVersion.P: case GameVersion.Pt:
                    table = Encounter_DPPt;
                    break;
                case GameVersion.HG: case GameVersion.SS:
                    table = Encounter_HGSS.Concat(Encounter_PokeWalker).ToArray();
                    break;

                case GameVersion.B: case GameVersion.W:
                    table = Encounter_BW.Concat(BW_DreamWorld).ToArray();
                    break;
                case GameVersion.B2: case GameVersion.W2:
                    table = Encounter_B2W2.Concat(B2W2_DreamWorld).ToArray();
                    break;

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
            return table?.Where(s => s.Version.Contains(Game)).ToArray();
        }
        private static EncounterArea[] getEncounterTables(GameVersion Game)
        {
            string ident = null;
            byte[] tables = null;
            switch (Game)
            {
                case GameVersion.R: return EncounterArea.getArray3(Data.unpackMini(Util.getBinaryResource("encounter_r.pkl"), "ru"));
                case GameVersion.S: return EncounterArea.getArray3(Data.unpackMini(Util.getBinaryResource("encounter_s.pkl"), "sa"));
                case GameVersion.E: return EncounterArea.getArray3(Data.unpackMini(Util.getBinaryResource("encounter_e.pkl"), "em"));
                case GameVersion.FR: return EncounterArea.getArray3(Data.unpackMini(Util.getBinaryResource("encounter_fr.pkl"), "fr"));
                case GameVersion.LG: return EncounterArea.getArray3(Data.unpackMini(Util.getBinaryResource("encounter_lg.pkl"), "lg"));
                case GameVersion.D: return EncounterArea.getArray4DPPt(Data.unpackMini(Util.getBinaryResource("encounter_d.pkl"), "da"));
                case GameVersion.P: return EncounterArea.getArray4DPPt(Data.unpackMini(Util.getBinaryResource("encounter_p.pkl"), "pe"));
                case GameVersion.Pt: return EncounterArea.getArray4DPPt(Data.unpackMini(Util.getBinaryResource("encounter_pt.pkl"), "pt"));
                case GameVersion.HG: return EncounterArea.getArray4HGSS(Data.unpackMini(Util.getBinaryResource("encounter_hg.pkl"), "hg"));
                case GameVersion.SS: return EncounterArea.getArray4HGSS(Data.unpackMini(Util.getBinaryResource("encounter_ss.pkl"), "ss"));
                case GameVersion.B: ident = "51"; tables = Util.getBinaryResource("encounter_b.pkl"); break;
                case GameVersion.W: ident = "51"; tables = Util.getBinaryResource("encounter_w.pkl"); break;
                case GameVersion.B2: ident = "52"; tables = Util.getBinaryResource("encounter_b2.pkl"); break;
                case GameVersion.W2: ident = "52"; tables = Util.getBinaryResource("encounter_w2.pkl"); break;
                case GameVersion.X: ident = "xy"; tables = Util.getBinaryResource("encounter_x.pkl"); break;
                case GameVersion.Y: ident = "xy"; tables = Util.getBinaryResource("encounter_y.pkl"); break; 
                case GameVersion.AS: ident = "ao"; tables = Util.getBinaryResource("encounter_a.pkl"); break;
                case GameVersion.OR: ident = "ao"; tables = Util.getBinaryResource("encounter_o.pkl"); break;
                case GameVersion.SN: ident = "sm"; tables = Util.getBinaryResource("encounter_sn.pkl"); break;
                case GameVersion.MN: ident = "sm"; tables = Util.getBinaryResource("encounter_mn.pkl"); break;
            }
            if (ident == null)
                return new EncounterArea[0];

            return getEncounterTables(tables, ident);
        }
        private static EncounterArea[] getEncounterTables(byte[] mini, string ident)
        {
            return EncounterArea.getArray(Data.unpackMini(mini, ident));
        }
        private static EncounterArea[] addExtraTableSlots(params EncounterArea[][] tables)
        {
            return tables.SelectMany(s => s).GroupBy(l => l.Location)
                .Select(t => t.Count() == 1 
                    ? t.First() // only one table, just return the area
                    : new EncounterArea {Location = t.First().Location, Slots = t.SelectMany(s => s.Slots).ToArray()})
                .ToArray();
        }
        private static void MarkEncountersStaticMagnetPull(ref EncounterArea[] Areas, PersonalTable t)
        {
            const int steel = 8;
            const int electric = 12;
            foreach (EncounterArea Area in Areas)
            {
                var s = new List<EncounterSlot>(); // Static
                var m = new List<EncounterSlot>(); // Magnet Pull
                foreach (EncounterSlot Slot in Area.Slots)
                {
                    var types = t[Slot.Species].Types;
                    if (types[0] == steel || types[1] == steel)
                        m.Add(Slot);
                    if (types[0] == electric || types[1] == electric)
                        s.Add(Slot);
                }
                foreach (var slot in s)
                {
                    slot.Static = true;
                    slot.StaticCount = s.Count;
                }
                foreach (var slot in m)
                {
                    slot.MagnetPull = true;
                    slot.MagnetPullCount = s.Count;
                }
            }
        }
        private static void MarkEncountersGeneration(ref EncounterStatic[] Encounters, int Generation)
        {
            foreach (EncounterStatic Encounter in Encounters)
                Encounter.Generation = Generation;
        }
        private static void MarkEncountersGeneration(ref EncounterArea[] Areas, int Generation)
        {
            foreach (EncounterArea Area in Areas)
                foreach (EncounterSlot Slot in Area.Slots)
                    Slot.Generation = Generation;
        }
        private static void ReduceAreasSize(ref EncounterArea[] Areas)
        {
            // Group areas by location id, the raw data have areas with different slots but the same location id
            Areas = Areas.GroupBy(a => a.Location).Select(a => new EncounterArea
            {
                Location = a.First().Location,
                Slots = a.SelectMany(m => m.Slots).ToArray()
            }).ToArray();
        }
        private static void MarkG2Slots(ref EncounterArea[] Areas)
        {
            ReduceAreasSize(ref Areas);
        }
        private static void MarkG3Slots_FRLG(ref EncounterArea[] Areas)
        {
            // Remove slots for unown, those slots does not contains alt form info, it will be added manually in SlotsRFLGAlt
            // Group areas by location id, the raw data have areas with different slots but the same location id
            Areas = Areas.Where(a => a.Location < 188 || a.Location > 194).GroupBy(a => a.Location).Select(a => new EncounterArea
            {
                Location = a.First().Location,
                Slots = a.SelectMany(m => m.Slots).ToArray()
            }).ToArray();
        }
        private static void MarkG3Slots_RSE(ref EncounterArea[] Areas)
        {
            ReduceAreasSize(ref Areas);
        }
        private static void MarkG3SlotsSafariZones(ref EncounterArea[] Areas, int location)
        {
            foreach (EncounterArea Area in Areas.Where(a => a.Location == location))
            {
                foreach (EncounterSlot Slot in Area.Slots)
                {
                    SlotType t;
                    switch (Slot.Type)
                    {
                        case SlotType.Grass: t = SlotType.Grass_Safari; break;
                        case SlotType.Surf: t = SlotType.Surf_Safari; break;
                        case SlotType.Old_Rod: t = SlotType.Old_Rod_Safari; break;
                        case SlotType.Good_Rod: t = SlotType.Good_Rod_Safari; break;
                        case SlotType.Super_Rod: t = SlotType.Super_Rod_Safari; break;
                        case SlotType.Rock_Smash: t = SlotType.Rock_Smash_Safari; break;
                        default: continue;
                    }
                    Slot.Type = t;
                }
            }
        }
        private static void MarkG4PokeWalker(ref EncounterStatic[] t)
        {
            foreach (EncounterStatic s in t)
            {
                s.Location = 233;  //Pokéwalker
                s.Gift = true;    //Pokeball only
            }
        }
        private static void MarkG4SlotsGreatMarsh(ref EncounterArea[] Areas, int location)
        {
            foreach (EncounterArea Area in Areas.Where(a => a.Location == location))
            {
                foreach (EncounterSlot Slot in Area.Slots)
                {
                    SlotType t;
                    switch (Slot.Type)
                    {
                        case SlotType.Grass: t = SlotType.Grass_Safari; break;
                        case SlotType.Surf: t = SlotType.Surf_Safari; break;
                        case SlotType.Old_Rod: t = SlotType.Old_Rod_Safari; break;
                        case SlotType.Good_Rod: t = SlotType.Good_Rod_Safari; break;
                        case SlotType.Super_Rod: t = SlotType.Super_Rod_Safari; break;
                        case SlotType.Pokeradar: t = SlotType.Pokeradar_Safari; break;
                        default: continue;
                    }
                    Slot.Type = t;
                }
            }
        }
        private static void MarkG4SwarmSlots(ref EncounterArea[] Areas, EncounterArea[] SwarmAreas)
        {
            // Grass Swarm slots replace slots 0 and 1 from encounters data
            // for surfing only replace slots 0 from encounters data
            // for fishing replace one or several random slots from encounters data, but all slots have the same level, it's ok to only replace the first
            // Species id are not included in encounter tables but levels can be copied from the encounter raw data
            foreach (EncounterArea Area in Areas)
            {
                var SwarmSlots = SwarmAreas.Where(a => a.Location == Area.Location).SelectMany(s => s.Slots);
                var OutputSlots = new List<EncounterSlot>();
                foreach (EncounterSlot SwarmSlot in SwarmSlots)
                {
                    int slotsnum = SwarmSlot.Type == SlotType.Grass ? 2 : 1;
                    foreach (var swarmSlot in Area.Slots.Where(s => s.Type == SwarmSlot.Type).Take(slotsnum).Select(slot => slot.Clone()))
                    {
                        swarmSlot.Species = SwarmSlot.Species;
                        OutputSlots.Add(swarmSlot);
                    }
                }
                Area.Slots = Area.Slots.Concat(OutputSlots).Where(a => a.Species > 0).ToArray();
            }
        }
        // Gen 4 raw encounter data does not contains info for alt slots
        // Shellos and Gastrodom East Sea form should be modified 
        private static void MarkG4AltFormSlots(ref EncounterArea[] Areas, int Species, int form, int[] Locations)
        {
            foreach(EncounterArea Area in Areas.Where(a => Locations.Contains(a.Location)))
            {
                foreach (EncounterSlot Slot in Area.Slots.Where(s=>s.Species == Species))
                {
                    Slot.Form = form;
                }
            }
        }
        private static void MarkG4Slots(ref EncounterArea[] Areas)
        {
            ReduceAreasSize(ref Areas);
        }
        private static EncounterType GetEncounterTypeBySlotDPPt(SlotType Type, EncounterType GrassType)
        {
            switch (Type)
            {
                case SlotType.Pokeradar:
                case SlotType.Pokeradar_Safari:
                case SlotType.Swarm:
                case SlotType.Grass: return GrassType;
                case SlotType.Surf:
                case SlotType.Old_Rod:
                case SlotType.Good_Rod:
                case SlotType.Super_Rod:
                case SlotType.Surf_Safari:
                case SlotType.Old_Rod_Safari:
                case SlotType.Good_Rod_Safari:
                case SlotType.Super_Rod_Safari: return EncounterType.Surfing_Fishing;
                case SlotType.Grass_Safari: return EncounterType.MarshSafari;
                case SlotType.HoneyTree: return EncounterType.None;
            }
            return EncounterType.None;
        }
        private static EncounterType GetEncounterTypeBySlotHGSS(SlotType Type, EncounterType GrassType, EncounterType HeadbuttType)
        {
            switch (Type)
            {
                // HGSS Safari encounters have normal water/grass encounter type, not safari encounter type
                case SlotType.Grass:
                case SlotType.Grass_Safari:
                case SlotType.BugContest: return GrassType;
                case SlotType.Surf:
                case SlotType.Old_Rod:
                case SlotType.Good_Rod:
                case SlotType.Super_Rod:
                case SlotType.Surf_Safari:
                case SlotType.Old_Rod_Safari:
                case SlotType.Good_Rod_Safari:
                case SlotType.Super_Rod_Safari: return EncounterType.Surfing_Fishing;
                case SlotType.Rock_Smash:
                case SlotType.Rock_Smash_Safari: return EncounterType.RockSmash;
                case SlotType.Headbutt: return HeadbuttType;
                case SlotType.Headbutt_Special: return EncounterType.None;
            }
            return EncounterType.None;
        }
        private static void MarkDPPtEncounterTypeSlots_MultipleTypes(ref EncounterArea[] Areas, int Location, int SpecialEncounterFile, EncounterType NormalEncounterType)
        {
            // Area with two different encounter type for grass encounters
            // SpecialEncounterFile is tall grass encounter type, the other files have the normal encounter type for this location
            var numfile = 0;
            foreach (EncounterArea Area in Areas.Where(x => x.Location == Location))
            {
                numfile++;
                var GrassType = numfile == SpecialEncounterFile ? EncounterType.TallGrass : NormalEncounterType;
                foreach (EncounterSlot Slot in Area.Slots)
                {
                    Slot.TypeEncounter = GetEncounterTypeBySlotDPPt(Slot.Type, GrassType);
                }
            }
        }
        private static void MarkDPPtEncounterTypeSlots_MultipleTypes(ref EncounterArea[] Areas, int Location, int[] SpecialEncounterFiles, EncounterType NormalEncounterType)
        {
            var numfile = 0;
            foreach (EncounterArea Area in Areas.Where(x => x.Location == Location))
            {
                numfile++;
                var GrassType = SpecialEncounterFiles.Contains(numfile) ? EncounterType.TallGrass : NormalEncounterType;
                foreach (EncounterSlot Slot in Area.Slots)
                {
                    Slot.TypeEncounter = GetEncounterTypeBySlotDPPt(Slot.Type, GrassType);
                }
            }
        }
        private static void MarkHGSSEncounterTypeSlots_MultipleTypes(ref EncounterArea[] Areas, int Location, int SpecialEncounterFile, EncounterType NormalEncounterType)
        {
            // Area with two different encounter type for grass encounters
            // SpecialEncounterFile is taall grass encounter type, the other files have the normal encounter type for this location
            var HeadbuttType = getHeadbuttEncounter(Location);
            var numfile = 0;
            foreach (EncounterArea Area in Areas.Where(x => x.Location == Location))
            {
                numfile++;
                var GrassType = numfile == SpecialEncounterFile ? EncounterType.TallGrass : NormalEncounterType;
                foreach (EncounterSlot Slot in Area.Slots)
                {
                    Slot.TypeEncounter = GetEncounterTypeBySlotHGSS(Slot.Type, GrassType, HeadbuttType);
                }
            }
        }
        private static void MarkHGSSEncounterTypeSlots_MultipleTypes(ref EncounterArea[] Areas, int Location, int[] SpecialEncounterFiles, EncounterType NormalEncounterType)
        {
            var HeadbuttType = getHeadbuttEncounter(Location);
            var numfile = 0;
            foreach (EncounterArea Area in Areas.Where(x => x.Location == Location))
            {
                numfile++;
                var GrassType = SpecialEncounterFiles.Contains(numfile) ? EncounterType.TallGrass : NormalEncounterType;
                foreach (EncounterSlot Slot in Area.Slots)
                {
                    Slot.TypeEncounter = GetEncounterTypeBySlotHGSS(Slot.Type, GrassType, HeadbuttType);
                }
            }
        }
        private static void MarkDPPtEncounterTypeSlots(ref EncounterArea[] Areas)
        {
            foreach(EncounterArea Area in Areas)
            {
                if (DPPt_MixInteriorExteriorLocations.Contains(Area.Location))
                    continue;
                var GrassType = (Area.Location == 70) ? EncounterType.Building_EnigmaStone :// Old Chateau
                                DPPt_CaveLocations.Contains(Area.Location) ? EncounterType.Cave_HallOfOrigin :
                                EncounterType.TallGrass;
                foreach (EncounterSlot Slot in Area.Slots)
                {
                    Slot.TypeEncounter = GetEncounterTypeBySlotDPPt(Slot.Type, GrassType);
                }
            }
        }
        private static EncounterType getHeadbuttEncounter(int Location)
        {
            // Routes with trees adjacent to water tiles
            var allowsurf= HGSS_SurfingHeadbutt_Locations.Contains(Location);
            // Cities
            if (HGSS_CityLocations.Contains(Location))
            {
                return allowsurf ? EncounterType.Headbutt_CitySurf : EncounterType.Building_EnigmaStone;
            }
            // Caves with no exterior zones
            if (!HGSS_MixInteriorExteriorLocations.Contains(Location) && HGSS_CaveLocations.Contains(Location))
            {
                return allowsurf ? EncounterType.Headbutt_CaveSurf : EncounterType.Cave_HallOfOrigin;
            }
            // Routes and exterior areas
            // Routes with trees adjacent to grass tiles
            var allowgrass = HGSS_GrassHeadbutt_Locations.Contains(Location);
            return allowgrass && allowsurf ? EncounterType.Headbutt_GrassSurf :
                   allowgrass ? EncounterType.Headbutt_Grass :
                   allowsurf ?  EncounterType.Headbutt_Surf :
                   EncounterType.None;
        }

        private static void MarkHGSSEncounterTypeSlots(ref EncounterArea[] Areas)
        {
            foreach (EncounterArea Area in Areas)
            {
                if (HGSS_MixInteriorExteriorLocations.Contains(Area.Location))
                    continue;
                var GrassType = HGSS_CaveLocations.Contains(Area.Location) ? EncounterType.Cave_HallOfOrigin: EncounterType.TallGrass;
                var HeadbuttType = getHeadbuttEncounter(Area.Location);
                foreach (EncounterSlot Slot in Area.Slots)
                {
                    Slot.TypeEncounter = GetEncounterTypeBySlotHGSS(Slot.Type, GrassType, HeadbuttType);
                }
            }
        }
        private static void MarkBWSwarmSlots(ref EncounterArea[] Areas)
        {
            foreach (EncounterSlot s in Areas.SelectMany(area => area.Slots))
            {
                s.LevelMin = 15; s.LevelMax = 55; s.Type = SlotType.Swarm;
            }
        }
        private static void MarkB2W2SwarmSlots(ref EncounterArea[] Areas)
        {
            foreach (EncounterSlot s in Areas.SelectMany(area => area.Slots))
            {
                s.LevelMin = 40; s.LevelMax = 55; s.Type = SlotType.Swarm;
            }
        }
        private static void MarkG5HiddenGrottoSlots(ref EncounterArea[] Areas)
        {
            foreach (EncounterSlot s in Areas[0].Slots) //Only 1 area
                s.Type = SlotType.HiddenGrotto; 
        }
        private static void MarkG5DreamWorld(ref EncounterStatic[] t)
        {
            foreach (EncounterStatic s in t)
            {
                s.Location = 75;  //Entree Forest
                s.Ability = PersonalTable.B2W2.getAbilities(s.Species, s.Form)[2] == 0 ? 1 : 4; // Check if has HA
            }

            // Split encounters with multiple permitted special moves -- a pkm can only be obtained with 1 of the special moves!
            var list = new List<EncounterStatic>();
            foreach (EncounterStatic s in t)
            {
                if (s.Moves == null || s.Moves.Length <= 1) // no special moves
                {
                    list.Add(s);
                    continue;
                }

                var loc = s.Location;
                for (int i = 0; i < s.Moves.Length; i++)
                {
                    var clone = s.Clone(loc);
                    clone.Moves = new[] {s.Moves[i]};
                    list.Add(clone);
                }
            }
            t = list.ToArray();
        }
        private static void MarkG5Slots(ref EncounterArea[] Areas)
        {
            foreach (var area in Areas)
            {
                int ctr = 0;
                do
                {
                    for (int i = 0; i < 12; i++)
                        area.Slots[ctr++].Type = SlotType.Grass; // Single

                    for (int i = 0; i < 12; i++)
                        area.Slots[ctr++].Type = SlotType.Grass; // Double

                    for (int i = 0; i < 12; i++)
                        area.Slots[ctr++].Type = SlotType.Grass; // Shaking

                    for (int i = 0; i < 5; i++) // 5
                        area.Slots[ctr++].Type = SlotType.Surf; // Surf

                    for (int i = 0; i < 5; i++) // 5
                        area.Slots[ctr++].Type = SlotType.Surf; // Surf Spot

                    for (int i = 0; i < 5; i++) // 5
                        area.Slots[ctr++].Type = SlotType.Super_Rod; // Fish

                    for (int i = 0; i < 5; i++) // 5
                        area.Slots[ctr++].Type = SlotType.Super_Rod; // Fish Spot
                } while (ctr != area.Slots.Length);
                area.Slots = area.Slots.Where(slot => slot.Species != 0).ToArray();
            }
            ReduceAreasSize(ref Areas);
        }
        private static void MarkG6XYSlots(ref EncounterArea[] Areas)
        {
            foreach (var area in Areas)
            {
                int slotct = area.Slots.Length;
                for (int i = slotct - 15; i < slotct; i++)
                    area.Slots[i].Type = SlotType.Horde;
            }
            ReduceAreasSize(ref Areas);
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
            ReduceAreasSize(ref Areas);
        }
        private static void MarkG7REGSlots(ref EncounterArea[] Areas)
        {
            ReduceAreasSize(ref Areas);
        }
        private static void MarkG7SMSlots(ref EncounterArea[] Areas)
        {
            foreach (EncounterSlot s in Areas.SelectMany(area => area.Slots))
                s.Type = SlotType.SOS;
            ReduceAreasSize(ref Areas);
        }
        private static EncounterArea[] getTables1()
        {
            var red_gw = EncounterArea.getArray1_GW(Util.getBinaryResource("encounter_red.pkl"));
            var blu_gw = EncounterArea.getArray1_GW(Util.getBinaryResource("encounter_blue.pkl"));
            var ylw_gw = EncounterArea.getArray1_GW(Util.getBinaryResource("encounter_yellow.pkl"));
            var rb_fish = EncounterArea.getArray1_F(Util.getBinaryResource("encounter_rb_f.pkl"));
            var ylw_fish = EncounterArea.getArray1_FY(Util.getBinaryResource("encounter_yellow_f.pkl"));

            var table = addExtraTableSlots(red_gw, blu_gw, ylw_gw, rb_fish, ylw_fish);
            Array.Resize(ref table, table.Length + 1);
            table[table.Length - 1] = FishOldGood_RBY;

            return table;
        }
        private static EncounterArea[] getTables2(GameVersion Version)
        {
            EncounterArea[] Slots = null;
            // Fishing
            var f = EncounterArea.getArray2_F(Util.getBinaryResource("encounter_gsc_f.pkl"));
            if (Version == GameVersion.GS || Version == GameVersion.GSC)
            {
                // Grass/Water
                var g = EncounterArea.getArray2_GW(Util.getBinaryResource("encounter_gold.pkl"));
                var s = EncounterArea.getArray2_GW(Util.getBinaryResource("encounter_silver.pkl"));
                // Headbutt/Rock Smash
                var h_g = EncounterArea.getArray2_H(Util.getBinaryResource("encounter_gold_h.pkl"));
                var h_s = EncounterArea.getArray2_H(Util.getBinaryResource("encounter_silver_h.pkl"));

                Slots = addExtraTableSlots(g, s, h_g, h_s, f);
            }
            if (Version == GameVersion.C || Version == GameVersion.GSC)
            {
                // Grass/Water
                var c = EncounterArea.getArray2_GW(Util.getBinaryResource("encounter_crystal.pkl"));
                // Headbutt/Rock Smash
                var h_c = EncounterArea.getArray2_H(Util.getBinaryResource("encounter_crystal_h.pkl"));

                var extra = addExtraTableSlots(c, h_c, f);
                return Version == GameVersion.C ? extra : addExtraTableSlots(Slots, extra);
            }

            return Slots;
        }

        static Legal() // Setup
        {
            // Gen 1
            {
                StaticRBY = getStaticEncounters(GameVersion.RBY);
                SlotsRBY = getTables1();
                // Gen 1 is the only gen where ReduceAreasSize is not needed
                MarkEncountersGeneration(ref SlotsRBY, 1);
                MarkEncountersGeneration(ref StaticRBY, 1);
                Evolves1 = new EvolutionTree(new[] { Util.getBinaryResource("evos_rby.pkl") }, GameVersion.RBY, PersonalTable.Y, MaxSpeciesID_1);
                FixPersonalTableY();
            }
            // Gen 2
            {
                StaticGS = getStaticEncounters(GameVersion.GS);
                StaticC = getStaticEncounters(GameVersion.C);
                StaticGSC = getStaticEncounters(GameVersion.GSC);
                SlotsGS = getTables2(GameVersion.GS);
                SlotsC = getTables2(GameVersion.C);
                SlotsGSC = getTables2(GameVersion.GSC);
                MarkG2Slots(ref SlotsGS);
                MarkG2Slots(ref SlotsC);
                MarkG2Slots(ref SlotsGSC);
                MarkEncountersGeneration(ref SlotsGS, 2);
                MarkEncountersGeneration(ref SlotsC, 2);
                MarkEncountersGeneration(ref SlotsGSC, 2);
                MarkEncountersGeneration(ref StaticGS, 2);
                MarkEncountersGeneration(ref StaticC, 2);
                MarkEncountersGeneration(ref StaticGSC, 2);
                Evolves2 = new EvolutionTree(new[] { Util.getBinaryResource("evos_gsc.pkl") }, GameVersion.GSC, PersonalTable.C, MaxSpeciesID_2);
            }
            // Gen3
            {
                StaticR = getStaticEncounters(GameVersion.R);
                StaticS = getStaticEncounters(GameVersion.S);
                StaticE = getStaticEncounters(GameVersion.E);
                StaticFR = getStaticEncounters(GameVersion.FR);
                StaticLG = getStaticEncounters(GameVersion.LG);

                var R_Slots = getEncounterTables(GameVersion.R);
                var S_Slots = getEncounterTables(GameVersion.S);
                var E_Slots = getEncounterTables(GameVersion.E);
                var FR_Slots = getEncounterTables(GameVersion.FR);
                var LG_Slots = getEncounterTables(GameVersion.LG);

                MarkG3Slots_RSE(ref R_Slots);
                MarkG3Slots_RSE(ref S_Slots);
                MarkG3Slots_RSE(ref E_Slots);
                MarkG3Slots_FRLG(ref FR_Slots);
                MarkG3Slots_FRLG(ref LG_Slots);
                MarkG3SlotsSafariZones(ref R_Slots, 57);
                MarkG3SlotsSafariZones(ref S_Slots, 57);
                MarkG3SlotsSafariZones(ref E_Slots, 57);
                MarkG3SlotsSafariZones(ref FR_Slots, 136);
                MarkG3SlotsSafariZones(ref LG_Slots, 136);

                MarkEncountersStaticMagnetPull(ref R_Slots, PersonalTable.SM);
                MarkEncountersStaticMagnetPull(ref S_Slots, PersonalTable.SM);
                MarkEncountersStaticMagnetPull(ref E_Slots, PersonalTable.SM);
                MarkEncountersStaticMagnetPull(ref FR_Slots, PersonalTable.SM);
                MarkEncountersStaticMagnetPull(ref LG_Slots, PersonalTable.SM);

                SlotsR = addExtraTableSlots(R_Slots, SlotsRSEAlt);
                SlotsS = addExtraTableSlots(S_Slots, SlotsRSEAlt);
                SlotsE = addExtraTableSlots(E_Slots, SlotsRSEAlt);
                SlotsFR = addExtraTableSlots(FR_Slots, SlotsFRLGAlt);
                SlotsLG = addExtraTableSlots(LG_Slots, SlotsFRLGAlt);

                Evolves3 = new EvolutionTree(new[] { Util.getBinaryResource("evos_g3.pkl") }, GameVersion.RS, PersonalTable.RS, MaxSpeciesID_3);

                // Update Personal Entries with TM/Tutor Data
                var TMHM = Data.unpackMini(Util.getBinaryResource("hmtm_g3.pkl"), "g3");
                for (int i = 0; i <= MaxSpeciesID_3; i++)
                    PersonalTable.E[i].AddTMHM(TMHM[i]);
                // Tutors g3 contains tutor compatiblity data extracted from emerald, 
                // fire red and leaf green tutors data is a subset of emerald data
                var tutors = Data.unpackMini(Util.getBinaryResource("tutors_g3.pkl"), "g3");
                for (int i = 0; i <= MaxSpeciesID_3; i++)
                    PersonalTable.E[i].AddTypeTutors(tutors[i]);
            }
            // Gen 4
            {
                MarkG4PokeWalker(ref Encounter_PokeWalker);
                StaticD = getStaticEncounters(GameVersion.D);
                StaticP = getStaticEncounters(GameVersion.P);
                StaticPt = getStaticEncounters(GameVersion.Pt);
                StaticHG = getStaticEncounters(GameVersion.HG);
                StaticSS = getStaticEncounters(GameVersion.SS);

                var D_Slots = getEncounterTables(GameVersion.D);
                var P_Slots = getEncounterTables(GameVersion.P);
                var Pt_Slots = getEncounterTables(GameVersion.Pt);
                var HG_Slots = getEncounterTables(GameVersion.HG);
                var SS_Slots = getEncounterTables(GameVersion.SS);

                MarkEncountersStaticMagnetPull(ref D_Slots, PersonalTable.SM);
                MarkEncountersStaticMagnetPull(ref P_Slots, PersonalTable.SM);
                MarkEncountersStaticMagnetPull(ref Pt_Slots, PersonalTable.SM);
                MarkEncountersStaticMagnetPull(ref HG_Slots, PersonalTable.SM);
                MarkEncountersStaticMagnetPull(ref SS_Slots, PersonalTable.SM);

                var DP_Trophy = EncounterArea.getTrophyArea(TrophyDP, new[] {16, 18});
                var Pt_Trophy = EncounterArea.getTrophyArea(TrophyPt, new[] {22, 22});
                var HG_Headbutt_Slots = EncounterArea.getArray4HGSS_Headbutt(Data.unpackMini(Util.getBinaryResource("encunters_hb_hg.pkl"), "hg"));
                var SS_Headbutt_Slots = EncounterArea.getArray4HGSS_Headbutt(Data.unpackMini(Util.getBinaryResource("encunters_hb_ss.pkl"), "ss"));

                var D_HoneyTrees_Slots = SlotsD_HoneyTree.Clone(HoneyTreesLocation);
                var P_HoneyTrees_Slots = SlotsP_HoneyTree.Clone(HoneyTreesLocation);
                var Pt_HoneyTrees_Slots = SlotsPt_HoneyTree.Clone(HoneyTreesLocation);
                
                MarkG4SwarmSlots(ref HG_Slots, SlotsHG_Swarm);
                MarkG4SwarmSlots(ref SS_Slots, SlotsSS_Swarm);

                MarkG4AltFormSlots(ref D_Slots, 422, 1, Shellos_EastSeaLocation_DP);
                MarkG4AltFormSlots(ref D_Slots, 423, 1, Gastrodon_EastSeaLocation_DP);
                MarkG4AltFormSlots(ref P_Slots, 422, 1, Shellos_EastSeaLocation_DP);
                MarkG4AltFormSlots(ref P_Slots, 423, 1, Gastrodon_EastSeaLocation_DP);
                MarkG4AltFormSlots(ref Pt_Slots, 422, 1, Shellos_EastSeaLocation_Pt);
                MarkG4AltFormSlots(ref Pt_Slots, 423, 1, Gastrodon_EastSeaLocation_Pt);

                // Route 209
                MarkDPPtEncounterTypeSlots_MultipleTypes(ref D_Slots, 24, 1, EncounterType.Building_EnigmaStone);
                MarkDPPtEncounterTypeSlots_MultipleTypes(ref P_Slots, 24, 1, EncounterType.Building_EnigmaStone);
                MarkDPPtEncounterTypeSlots_MultipleTypes(ref Pt_Slots, 24, 1, EncounterType.Building_EnigmaStone);

                // Stark Mountain
                MarkDPPtEncounterTypeSlots_MultipleTypes(ref D_Slots, 84, 1, EncounterType.Cave_HallOfOrigin);
                MarkDPPtEncounterTypeSlots_MultipleTypes(ref P_Slots, 84, 1, EncounterType.Cave_HallOfOrigin);
                MarkDPPtEncounterTypeSlots_MultipleTypes(ref Pt_Slots, 84, 1, EncounterType.Cave_HallOfOrigin);
                // Mt Coronet
                MarkDPPtEncounterTypeSlots_MultipleTypes(ref D_Slots, 50, DPPt_MtCoronetExteriorEncounters, EncounterType.Cave_HallOfOrigin);
                MarkDPPtEncounterTypeSlots_MultipleTypes(ref P_Slots, 50, DPPt_MtCoronetExteriorEncounters, EncounterType.Cave_HallOfOrigin);
                MarkDPPtEncounterTypeSlots_MultipleTypes(ref Pt_Slots, 50, DPPt_MtCoronetExteriorEncounters, EncounterType.Cave_HallOfOrigin);

                // Ruins of Alph
                MarkHGSSEncounterTypeSlots_MultipleTypes(ref HG_Slots, 209, 1, EncounterType.Cave_HallOfOrigin);
                MarkHGSSEncounterTypeSlots_MultipleTypes(ref SS_Slots, 209, 1, EncounterType.Cave_HallOfOrigin);
                // Mt Silver Cave
                MarkHGSSEncounterTypeSlots_MultipleTypes(ref HG_Slots, 219, HGSS_MtSilverCaveExteriorEncounters, EncounterType.Cave_HallOfOrigin);
                MarkHGSSEncounterTypeSlots_MultipleTypes(ref SS_Slots, 219, HGSS_MtSilverCaveExteriorEncounters, EncounterType.Cave_HallOfOrigin);

                MarkG4Slots(ref D_Slots);
                MarkG4Slots(ref P_Slots);
                MarkG4Slots(ref Pt_Slots);
                MarkG4Slots(ref HG_Slots);
                MarkG4Slots(ref SS_Slots);
                MarkG4Slots(ref HG_Headbutt_Slots);
                MarkG4Slots(ref SS_Headbutt_Slots);

                MarkG4SlotsGreatMarsh(ref D_Slots, 52);
                MarkG4SlotsGreatMarsh(ref P_Slots, 52);
                MarkG4SlotsGreatMarsh(ref Pt_Slots, 52);

                SlotsD = addExtraTableSlots(D_Slots, D_HoneyTrees_Slots, DP_GreatMarshAlt, SlotsDPPPtAlt, DP_Trophy);
                SlotsP = addExtraTableSlots(P_Slots, P_HoneyTrees_Slots, DP_GreatMarshAlt, SlotsDPPPtAlt, DP_Trophy);
                SlotsPt = addExtraTableSlots(Pt_Slots, Pt_HoneyTrees_Slots, Pt_GreatMarshAlt, SlotsDPPPtAlt, Pt_Trophy);
                SlotsHG = addExtraTableSlots(HG_Slots, HG_Headbutt_Slots, SlotsHGSSAlt);
                SlotsSS = addExtraTableSlots(SS_Slots, SS_Headbutt_Slots, SlotsHGSSAlt);

                MarkDPPtEncounterTypeSlots(ref SlotsD);
                MarkDPPtEncounterTypeSlots(ref SlotsP);
                MarkDPPtEncounterTypeSlots(ref SlotsPt);
                MarkHGSSEncounterTypeSlots(ref SlotsHG);
                MarkHGSSEncounterTypeSlots(ref SlotsSS);
                
                Evolves4 = new EvolutionTree(new[] { Util.getBinaryResource("evos_g4.pkl") }, GameVersion.DP, PersonalTable.DP, MaxSpeciesID_4);

                // Update Personal Entries with Tutor Data
                var tutors = Data.unpackMini(Util.getBinaryResource("tutors_g4.pkl"), "g4");
                for (int i = 0; i <= MaxSpeciesID_4; i++)
                    PersonalTable.HGSS[i].AddTypeTutors(tutors[i]);
            }
            // Gen 5
            {
                MarkG5DreamWorld(ref BW_DreamWorld);
                MarkG5DreamWorld(ref B2W2_DreamWorld);
                StaticB = getStaticEncounters(GameVersion.B);
                StaticW = getStaticEncounters(GameVersion.W);
                StaticB2 = getStaticEncounters(GameVersion.B2);
                StaticW2 = getStaticEncounters(GameVersion.W2);

                var BSlots = getEncounterTables(GameVersion.B);
                var WSlots = getEncounterTables(GameVersion.W);
                MarkG5Slots(ref BSlots);
                MarkG5Slots(ref WSlots);
                MarkBWSwarmSlots(ref SlotsB_Swarm);
                MarkBWSwarmSlots(ref SlotsW_Swarm);
                SlotsB = addExtraTableSlots(BSlots, SlotsB_Swarm);
                SlotsW = addExtraTableSlots(WSlots, SlotsW_Swarm, WhiteForestSlot);

                var B2Slots = getEncounterTables(GameVersion.B2);
                var W2Slots = getEncounterTables(GameVersion.W2);
                MarkG5Slots(ref B2Slots);
                MarkG5Slots(ref W2Slots);
                MarkB2W2SwarmSlots(ref SlotsB2_Swarm);
                MarkB2W2SwarmSlots(ref SlotsW2_Swarm);
                MarkG5HiddenGrottoSlots(ref SlotsB2_HiddenGrotto);
                MarkG5HiddenGrottoSlots(ref SlotsW2_HiddenGrotto);
                SlotsB2 = addExtraTableSlots(B2Slots, SlotsB2_Swarm, SlotsB2_HiddenGrotto);
                SlotsW2 = addExtraTableSlots(W2Slots, SlotsW2_Swarm, SlotsW2_HiddenGrotto);

                Evolves5 = new EvolutionTree(new[] { Util.getBinaryResource("evos_g5.pkl") }, GameVersion.BW, PersonalTable.BW, MaxSpeciesID_5);
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

                Evolves6 = new EvolutionTree(Data.unpackMini(Util.getBinaryResource("evos_ao.pkl"), "ao"), GameVersion.ORAS, PersonalTable.AO, MaxSpeciesID_6);
            }
            // Gen 7
            {
                StaticSN = getStaticEncounters(GameVersion.SN);
                StaticMN = getStaticEncounters(GameVersion.MN);
                var REG_SN = getEncounterTables(GameVersion.SN);
                var REG_MN = getEncounterTables(GameVersion.MN);
                var SOS_SN = getEncounterTables(Util.getBinaryResource("encounter_sn_sos.pkl"), "sm");
                var SOS_MN = getEncounterTables(Util.getBinaryResource("encounter_mn_sos.pkl"), "sm");
                MarkG7REGSlots(ref REG_SN);
                MarkG7REGSlots(ref REG_MN);
                MarkG7SMSlots(ref SOS_SN);
                MarkG7SMSlots(ref SOS_MN);
                SlotsSN = addExtraTableSlots(REG_SN, SOS_SN, Encounter_Pelago_SM, Encounter_Pelago_SN);
                SlotsMN = addExtraTableSlots(REG_MN, SOS_MN, Encounter_Pelago_SM, Encounter_Pelago_MN);

                Evolves7 = new EvolutionTree(Data.unpackMini(Util.getBinaryResource("evos_sm.pkl"), "sm"), GameVersion.SM, PersonalTable.SM, MaxSpeciesID_7);
            }
        }

        private static void FixPersonalTableY()
        {
            // Personal Table from Yellow do not have yellow catch rate for Pikachu and Kadabra, have RedBlue instead
            PersonalTable.Y[25].CatchRate = 163; // Pikachu
            PersonalTable.Y[64].CatchRate = 96; // Kadabra
        }

        // Moves
        internal static int[] getMinLevelLearnMove(int species, int Generation, List<int> moves)
        {
            var r = new int[moves.Count];
            switch (Generation)
            {
                case 1:
                    {
                        int index = PersonalTable.RB.getFormeIndex(species, 0);
                        if (index == 0)
                            return r;

                        var pi_rb = (PersonalInfoG1)PersonalTable.RB[index];
                        var pi_y = (PersonalInfoG1)PersonalTable.Y[index];

                        for (int m = 0; m < moves.Count; m++)
                        {
                            if (pi_rb.Moves.Contains(moves[m]) || pi_y.Moves.Contains(moves[m]))
                                r[m] = 1;
                            else
                            {
                                var rb_level = LevelUpRB[index].getLevelLearnMove(moves[m]);
                                var y_level = LevelUpY[index].getLevelLearnMove(moves[m]);
                                // 0 means it is not learned in that game, select the other game
                                r[m] = rb_level == 0 ? y_level :
                                       y_level == 0 ? rb_level :
                                       Math.Min(rb_level, y_level);
                            }
                        }
                        break;
                    }
            }
            return r;
        }
        internal static int[] getMaxLevelLearnMove(int species, int Generation, List<int> moves)
        {
            var r = new int[moves.Count];
            switch (Generation)
            {
                case 1:
                    {
                        int index = PersonalTable.RB.getFormeIndex(species, 0);
                        if (index == 0)
                            return r;

                        var pi_rb = (PersonalInfoG1)PersonalTable.RB[index];
                        var pi_y = (PersonalInfoG1)PersonalTable.Y[index];

                        for (int m = 0; m < moves.Count; m++)
                        {
                            if (pi_rb.Moves.Contains(moves[m]) && pi_y.Moves.Contains(moves[m]))
                                r[m] = 1;
                            else
                                r[m] = Math.Max(LevelUpRB[index].getLevelLearnMove(moves[m]), LevelUpY[index].getLevelLearnMove(moves[m]));
                        }
                        break;
                    }
            }
            return r;
        }
        internal static List<int>[] getExclusiveMoves(int species1, int species2, int Generation, List<int> tmhm, int[] moves)
        {
            // Return from two species the exclusive moves that only one could learn and also the current pokemon have it in its current moveset
            var moves1 = getLvlMoves(species1, 0, Generation, 1, 100).Distinct().ToList();
            var moves2 = getLvlMoves(species2, 0, Generation, 1, 100).Distinct().ToList();
            var common = moves1.Intersect(moves2).ToList();
            // Remove common moves, remove not learned moves and remove tmhm
            moves1.RemoveAll(x => !moves.Contains(x) || common.Contains(x) || tmhm.Contains(x));
            moves2.RemoveAll(x => !moves.Contains(x) || common.Contains(x) || tmhm.Contains(x));
            return new[] { moves1, moves2 };
        }
        internal static IEnumerable<int> getLvlMoves(int species, int form, int Generation, int minlvl, int lvl, GameVersion Version = GameVersion.Any)
        {
            var r = new List<int>();
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
                        if (minlvl == 1)
                        {
                            r.AddRange(pi_rb.Moves);
                            r.AddRange(pi_y.Moves);
                        }
                        r.AddRange(LevelUpRB[index].getMoves(lvl, minlvl));
                        r.AddRange(LevelUpY[index].getMoves(lvl, minlvl));
                        break;
                    }
                case 2:
                    {
                        int index = PersonalTable.C.getFormeIndex(species, 0);
                        if (index == 0)
                            return r;
                        r.AddRange(LevelUpGS[index].getMoves(lvl));
                        r.AddRange(LevelUpC[index].getMoves(lvl));
                        break;
                    }
                case 3:
                    {
                        int index = PersonalTable.E.getFormeIndex(species, 0);
                        if (index == 0)
                            return r;
                        if (index == 386)
                        {
                            switch (form)
                            {
                                case 0: r.AddRange(LevelUpRS[index].getMoves(lvl)); break;
                                case 1: r.AddRange(LevelUpFR[index].getMoves(lvl)); break;
                                case 2: r.AddRange(LevelUpLG[index].getMoves(lvl)); break;
                                case 3: r.AddRange(LevelUpE[index].getMoves(lvl)); break;
                            }
                        }
                        else
                        {
                            // Emerald level up table are equals to R/S level up tables
                            r.AddRange(LevelUpE[index].getMoves(lvl));
                            // fire red and leaf green are equals between each other but different than RSE
                            // Do not use FR Levelup table. It have 67 moves for charmander but Leaf Green moves table is correct
                            r.AddRange(LevelUpLG[index].getMoves(lvl));
                        }
                        break;
                    }
                case 4:
                    {
                        int index = PersonalTable.HGSS.getFormeIndex(species, 0);
                        if (index == 0)
                            return r;
                        r.AddRange(LevelUpDP[index].getMoves(lvl));
                        r.AddRange(LevelUpPt[index].getMoves(lvl));
                        r.AddRange(LevelUpHGSS[index].getMoves(lvl));
                        break;
                    }
                case 5:
                    {
                        int index = PersonalTable.B2W2.getFormeIndex(species, 0);
                        if (index == 0)
                            return r;
                        r.AddRange(LevelUpBW[index].getMoves(lvl));
                        r.AddRange(LevelUpB2W2[index].getMoves(lvl));
                        break;
                    }
                case 6:
                    switch (ver)
                    {
                        case GameVersion.Any: // Start at the top, hit every table
                        case GameVersion.X:
                        case GameVersion.Y:
                        case GameVersion.XY:
                            {
                                int index = PersonalTable.XY.getFormeIndex(species, form);
                                if (index == 0)
                                    return r;
                                r.AddRange(LevelUpXY[index].getMoves(lvl));
                                if (ver == GameVersion.Any) // Fall Through
                                    goto case GameVersion.ORAS;
                                break;
                            }

                        case GameVersion.AS:
                        case GameVersion.OR:
                        case GameVersion.ORAS:
                            {
                                int index = PersonalTable.AO.getFormeIndex(species, form);
                                if (index == 0)
                                    return r;
                                r.AddRange(LevelUpAO[index].getMoves(lvl));
                                break;
                            }
                    }
                    break;
                case 7:
                    switch (ver)
                    {
                        case GameVersion.Any:
                        case GameVersion.SN:
                        case GameVersion.MN:
                        case GameVersion.SM:
                            {
                                int index = PersonalTable.SM.getFormeIndex(species, form);
                                r.AddRange(LevelUpSM[index].getMoves(lvl));
                                break;
                            }
                    }
                    break;
                default:
                    return r;
            }
            return r;
        } 
        internal static List<int>[] getValidMovesAllGens(PKM pkm, DexLevel[][] evoChains, int minLvLG1 = 1, bool LVL = true, bool Tutor = true, bool Machine = true, bool MoveReminder = true, bool RemoveTransferHM = true)
        {
            List<int>[] Moves = new List<int>[evoChains.Length];
            for (int i = 1; i < evoChains.Length; i++)
                if (evoChains[i].Any())
                    Moves[i] = getValidMoves(pkm, evoChains[i], i, minLvLG1, LVL, Tutor, Machine, MoveReminder, RemoveTransferHM).ToList();
                else
                    Moves[i] = new List<int>();
            return Moves;
        }
        internal static IEnumerable<int> getValidMoves(PKM pkm, DexLevel[][] evoChains, int minLvLG1 = 1, bool LVL = true, bool Tutor = true, bool Machine = true, bool MoveReminder = true, bool RemoveTransferHM = true)
        {
            GameVersion version = (GameVersion)pkm.Version;
            if (!pkm.IsUntraded)
                version = GameVersion.Any;
            return getValidMoves(pkm, version, evoChains, minLvLG1: minLvLG1, LVL: LVL, Relearn: false, Tutor: Tutor, Machine: Machine, MoveReminder: MoveReminder, RemoveTransferHM: RemoveTransferHM);
        }
        internal static IEnumerable<int> getValidMoves(PKM pkm, DexLevel[] evoChain, int generation, int minLvLG1 = 1, bool LVL = true, bool Tutor = true, bool Machine = true, bool MoveReminder = true, bool RemoveTransferHM = true)
        {
            GameVersion version = (GameVersion)pkm.Version;
            if (!pkm.IsUntraded)
                version = GameVersion.Any;
            return getValidMoves(pkm, version, evoChain, generation, minLvLG1: minLvLG1, LVL: LVL, Relearn: false, Tutor: Tutor, Machine: Machine, MoveReminder: MoveReminder, RemoveTransferHM: RemoveTransferHM);
        }
        internal static IEnumerable<int> getValidRelearn(PKM pkm, int species)
        {
            List<int> r = new List<int> { 0 };
            if (pkm.GenNumber < 6 || pkm.VC)
                return r;

            r.AddRange(getRelearnLVLMoves(pkm, species, 1, pkm.AltForm));

            int form = pkm.AltForm;
            if (pkm.Format == 6 && pkm.Species != 678)
                form = 0;

            r.AddRange(getEggMoves(pkm, species, form));
            if (pkm.Species != 489)
                r.AddRange(getRelearnLVLMoves(pkm, species, 100, pkm.AltForm));
            return r.Distinct();
        }
        internal static List<int>[] getShedinjaEvolveMoves(PKM pkm, int lvl = -1, int generation = 0)
        {
            var size = pkm.Format > 3 ? 4 : 3;
            List<int>[] r = new List<int>[size + 1];
            for (int i = 1; i <= size; i++)
                r[i] = new List<int>();
            if (lvl == -1)
                lvl = pkm.CurrentLevel;
            if (pkm.Species != 292 || lvl < 20)
                return r;

            // If nincada evolves into Ninjask an learn in the evolution a move from ninjask learnset pool
            // Shedinja would appear with that move learned. Only one move above level 20 allowed, only in generations 3 and 4
            switch (generation)
            {
                case 0: // Default (both)
                case 3: // Ninjask have the same learnset in every gen 3 games
                    if (pkm.InhabitedGeneration(3))
                        r[3] = LevelUpE[291].getMoves(lvl, 20).ToList();

                    if (generation == 0)
                        goto case 4;
                    break;
                case 4: // Ninjask have the same learnset in every gen 4 games
                    if (pkm.InhabitedGeneration(4))
                        r[4] = LevelUpPt[291].getMoves(lvl, 20).ToList();
                    break;
            }
            return r;
        }
        internal static int[] getBaseEggMoves(PKM pkm, int species, GameVersion gameSource, int lvl)
        {
            if (gameSource == GameVersion.Any)
                gameSource = (GameVersion) pkm.Version;

            switch (gameSource)
            {
                case GameVersion.GS:
                    if (pkm.InhabitedGeneration(2))
                        if (pkm.Format == 1)
                            return LevelUpGS[species].getMoves(lvl).Where(m => m <= MaxMoveID_1).ToArray();
                        else
                            return LevelUpGS[species].getMoves(lvl);
                    break;
                case GameVersion.C:
                    if (pkm.InhabitedGeneration(2))
                        if (pkm.Format == 1)
                            return LevelUpC[species].getMoves(lvl).Where(m => m <= MaxMoveID_1).ToArray();
                        else
                            return LevelUpC[species].getMoves(lvl);
                    break;

                case GameVersion.R:
                case GameVersion.S:
                case GameVersion.RS:
                    if (pkm.InhabitedGeneration(3))
                        return LevelUpRS[species].getMoves(lvl);
                    break;
                case GameVersion.E:
                    if (pkm.InhabitedGeneration(3))
                        return LevelUpE[species].getMoves(lvl);
                    break;
                case GameVersion.FR:
                case GameVersion.LG:
                case GameVersion.FRLG:
                    // only difference in FR/LG is deoxys which doesn't breed.
                    if (pkm.InhabitedGeneration(3))
                        return LevelUpFR[species].getMoves(lvl);
                    break;

                case GameVersion.D:
                case GameVersion.P:
                case GameVersion.DP:
                    if (pkm.InhabitedGeneration(4))
                        return LevelUpDP[species].getMoves(lvl);
                    break;
                case GameVersion.Pt:
                    if (pkm.InhabitedGeneration(4))
                        return LevelUpPt[species].getMoves(lvl);
                    break;
                case GameVersion.HG:
                case GameVersion.SS:
                case GameVersion.HGSS:
                    if (pkm.InhabitedGeneration(4))
                        return LevelUpHGSS[species].getMoves(lvl);
                    break;

                case GameVersion.B:
                case GameVersion.W:
                case GameVersion.BW:
                    if (pkm.InhabitedGeneration(5))
                        return LevelUpBW[species].getMoves(lvl);
                    break;

                case GameVersion.B2:
                case GameVersion.W2:
                case GameVersion.B2W2:
                    if (pkm.InhabitedGeneration(5))
                        return LevelUpBW[species].getMoves(lvl);
                    break;

                case GameVersion.X:
                case GameVersion.Y:
                case GameVersion.XY:
                    if (pkm.InhabitedGeneration(6))
                        return LevelUpXY[species].getMoves(lvl);
                    break;

                case GameVersion.AS:
                case GameVersion.OR:
                case GameVersion.ORAS:
                    if (pkm.InhabitedGeneration(6))
                        return LevelUpAO[species].getMoves(lvl);
                    break;

                case GameVersion.SN:
                case GameVersion.MN:
                case GameVersion.SM:
                    int index = PersonalTable.SM.getFormeIndex(species, pkm.AltForm);
                    if (pkm.InhabitedGeneration(7))
                        return LevelUpSM[index].getMoves(lvl);
                    break;
            }
            return new int[0];
        }
        internal static List<int> getValidPostEvolutionMoves(PKM pkm, int Species, DexLevel[][] evoChains, GameVersion Version)
        {
            // Return moves that the pokemon could learn after evolving 
            var moves = new List<int>();
            for (int i = 1; i < evoChains.Length; i++)
                if (evoChains[i].Any())
                    moves.AddRange(getValidPostEvolutionMoves(pkm, Species, evoChains[i], i, Version));
            if (pkm.GenNumber >= 6)
                moves.AddRange(pkm.RelearnMoves.Where(m => m != 0));
            return moves.Distinct().ToList();
        }
        internal static IEnumerable<int> getValidPostEvolutionMoves(PKM pkm, int Species, DexLevel[] evoChain, int Generation, GameVersion Version)
        {
            var evomoves = new List<int>();
            var index = Array.FindIndex(evoChain, e => e.Species == Species);
            for (int i = 0; i <= index; i++)
            {
                var evo = evoChain[i];
                var moves = getMoves(pkm, evo.Species, 1, evo.Level, pkm.AltForm, moveTutor: true, Version: Version, LVL: true, specialTutors: true, Machine: true, MoveReminder: true, RemoveTransferHM: false, Generation: Generation);
                // Moves from Species or any species after in the evolution phase
                evomoves.AddRange(moves);
            }
            return evomoves;
        }
        internal static List<int>[] getExclusivePreEvolutionMoves(PKM pkm, int Species, DexLevel[][] evoChains, GameVersion Version)
        {
            // Return moves that the pokemon could only learn throught the preevolution Species
            List<int>[] Moves = new List<int>[evoChains.Length];
            for (int i = 1; i < evoChains.Length; i++)
                if (evoChains[i].Any())
                    Moves[i] = getExclusivePreEvolutionMoves(pkm, Species, evoChains[i], i, Version).ToList();
                else
                    Moves[i] = new List<int>();
            return Moves;
        }
        internal static IEnumerable<int> getExclusivePreEvolutionMoves(PKM pkm, int Species, DexLevel[] evoChain, int Generation, GameVersion Version)
        {
            var preevomoves = new List<int>();
            var evomoves = new List<int>();
            var index = Array.FindIndex(evoChain, e => e.Species == Species);
            for (int i = 0; i < evoChain.Length; i++)
            {
                var evo = evoChain[i];
                var moves = getMoves(pkm, evo.Species, 1, evo.Level, pkm.AltForm, moveTutor: true, Version: Version, LVL: true, specialTutors: true, Machine: true, MoveReminder: true, RemoveTransferHM: false, Generation: Generation);
                if (i >= index)
                    // Moves from Species or any species bellow in the evolution phase
                    preevomoves.AddRange(moves);
                else
                    // Moves in phase evolutions after the limit species, this moves should be removed
                    evomoves.AddRange(moves);
            }
            preevomoves.RemoveAll(x => evomoves.Contains(x));
            return preevomoves.Distinct().ToList();
        }

        // Encounter
        internal static GameVersion[] getBaseMovesIsEggGames(PKM pkm)
        {
            switch (pkm.GenNumber)
            {
                case 1:
                case 2:
                    return new[] { GameVersion.GS, GameVersion.C };
                case 3:
                    switch ((GameVersion)pkm.Version)
                    {
                        case GameVersion.R:
                        case GameVersion.S:
                            return new[] { GameVersion.RS };
                        case GameVersion.E:
                            return new[] { GameVersion.E };
                        case GameVersion.FR:
                        case GameVersion.LG:
                            return new[] { GameVersion.FRLG };
                        case GameVersion.CXD:
                            return new[] { GameVersion.CXD }; // will yield no moves
                    }
                    break;
                case 4:
                    switch ((GameVersion)pkm.Version)
                    {
                        case GameVersion.D:
                        case GameVersion.P:
                            return new[] { GameVersion.DP };
                        case GameVersion.Pt:
                            return new[] { GameVersion.Pt };
                        case GameVersion.HG:
                        case GameVersion.SS:
                            return new[] { GameVersion.HGSS };
                    }
                    break;
                case 5:
                    switch ((GameVersion)pkm.Version)
                    {
                        case GameVersion.B:
                        case GameVersion.W:
                            return new[] { GameVersion.BW };
                        case GameVersion.B2:
                        case GameVersion.W2:
                            return new[] { GameVersion.B2W2 };
                    }
                    break;
                case 6:
                    if (pkm.WasTradedEgg)
                        return new[] { GameVersion.XY, GameVersion.ORAS };
                    return new[] { (GameVersion)pkm.Version };
                case 7:
                    // if (pkm.WasTradedEgg)
                    //     return new[] { GameVersion.SM };
                    return new[] { (GameVersion)pkm.Version };
            }
            return new GameVersion[] { };
        }
        internal static GameVersion[] getGen1GameEncounter(PKM pk)
        {
            if (pk.Format != 1 || !pk.Gen1_NotTradeback)
                return new[] { GameVersion.RD, GameVersion.YW };
            if (25 <= pk.Species && pk.Species <= 26)
                // Yellow Pikachu detected by its special catch rate
                return new[] { ((pk as PK1).Catch_Rate == 163) ? GameVersion.YW : GameVersion.RD };
            if (64 <= pk.Species && pk.Species <= 65)
                // Yellow Kadabra detected by its special catch rate
                return new[] { ((pk as PK1).Catch_Rate == 96) ? GameVersion.YW : GameVersion.RD };
            if (148 <= pk.Species && pk.Species <= 149 && ((pk as PK1).Catch_Rate == 27))
                // Yellow Dragonair detected by its special catch rate, is have another catch rate could be red/blue dratini or yellow dratini
                return new[] { GameVersion.YW };
            return new[] { GameVersion.RD, GameVersion.YW };
        }
        internal static IEnumerable<int> getInitialMovesGBEncounter(int species, int lvl, GameVersion ver)
        {
            int[] InitialMoves;
            int[] LevelUpMoves;
            int diff;
            switch (ver)
            {
                case GameVersion.YW:
                case GameVersion.RD:
                case GameVersion.BU:
                case GameVersion.GN:
                    {
                        var LevelTable = ver == GameVersion.YW ? LevelUpY : LevelUpRB;
                        int index = PersonalTable.RB.getFormeIndex(species, 0);
                        if (index == 0)
                            return new int[0];
                        LevelUpMoves = LevelTable[species].getEncounterMoves(lvl);
                        diff = 4 - LevelUpMoves.Length;
                        if (diff == 0)
                            return LevelUpMoves.ToArray();
                        InitialMoves = ver == GameVersion.YW ? ((PersonalInfoG1)PersonalTable.Y[index]).Moves : ((PersonalInfoG1)PersonalTable.RB[index]).Moves;
                        break;
                    }
                case GameVersion.C:
                case GameVersion.GS:
                    {
                        if (species == 235)
                            return new[] { 166 }; // Smeargle only learns Sketch, is duplicated in the level up tables
                        var LevelTable = ver == GameVersion.C ? LevelUpC : LevelUpGS;
                        int index = PersonalTable.C.getFormeIndex(species, 0);
                        if (index == 0)
                            return new int[0];
                        LevelUpMoves = LevelTable[species].getEncounterMoves(2, lvl);
                        diff = 4 - LevelUpMoves.Length;
                        if (diff == 0)
                            return LevelUpMoves.ToArray();
                        // Level Up 1 moves are initial moves, it can be duplicated in levels 2-100
                        InitialMoves = LevelTable[species].getEncounterMoves(1, 1);
                        break;
                    }
                default:
                    return new int[0];
            }
            // Initial Moves could be duplicated in the level up table
            // level up table moves have preferences
            var moves = InitialMoves.Where(p => p != 0 && !LevelUpMoves.Any(m => m == p)).ToList();
            // If not all the personal table move cant be included the last moves have preference
            int pop = moves.Count - diff;
            if (pop > 0)
                moves.RemoveRange(0, pop);
            // The order for the pokemon default moves are first moves from personal table and then moves from  level up table
            return moves.Union(LevelUpMoves).ToArray();
        }
        internal static int getMoveMinLevelGBEncounter(int species, int lvl, GameVersion[] versions)
        {
            int movelvl = 100;
            foreach(GameVersion ver in versions)
            {
                movelvl = Math.Min(movelvl, getMoveMinLevelGBEncounter(species, lvl, ver));
            }
            return movelvl;
        }
        internal static int getMoveMinLevelGBEncounter(int species, int lvl, GameVersion ver = GameVersion.Any)
        {
            int movelvl = 100;
            switch (ver)
            {
                case GameVersion.RD:
                case GameVersion.BU:
                case GameVersion.GN:
                case GameVersion.RBY:
                    {
                        movelvl =  LevelUpRB[species].getMinMoveLevel(lvl);
                        if (ver.Contains(GameVersion.YW))
                            goto case GameVersion.YW;
                        return movelvl;
                    }
                case GameVersion.YW:
                    {
                        movelvl = Math.Min(movelvl, LevelUpY[species].getMinMoveLevel(lvl));
                        if (ver.Contains(GameVersion.GS))
                            goto case GameVersion.GS;
                        return movelvl;
                    }
                case GameVersion.GS:
                case GameVersion.GSC:
                    {
                        movelvl = Math.Min(movelvl, LevelUpGS[species].getMinMoveLevel(lvl));
                        if (ver.Contains(GameVersion.C))
                            goto case GameVersion.C;
                        return movelvl;
                    }
                case GameVersion.C:
                    {
                        return Math.Min(movelvl, LevelUpC[species].getMinMoveLevel(lvl));
                    }
            }
            return movelvl;
        }
        internal static int getRequiredMoveCount(PKM pk, int[] moves, LegalInfo info, int[] initialmoves)
        {
            if (pk.Format != 1 || !pk.Gen1_NotTradeback) // No MoveDeleter
                return 1; // Move deleter exits, slots from 2 onwards can allways be empty

            int required = getRequiredMoveCount(pk, moves, info.EncounterMoves.validLevelUpMoves, initialmoves);
            if (required >= 4)
                return 4;

            // tm, hm and tutor moves replace a free slots if the pokemon have less than 4 moves
            // Ignore tm, hm and tutor moves already in the learnset table
            var learn = info.EncounterMoves.validLevelUpMoves;
            var tmhm = info.EncounterMoves.validTMHMMoves;
            var tutor = info.EncounterMoves.validTutorMoves;
            required += moves.Where(m => m != 0 && initialmoves.Union(learn[1]).All(l => l != m) && (tmhm[1].Any(t => t == m) || tutor[1].Any(t => t == m))).Count();

            return Math.Min(4, required);
        }
        private static int getRequiredMoveCount(PKM pk, int[] moves, List<int>[] learn, int[] initialmoves)
        {
            if (SpecialMinMoveSlots.Contains(pk.Species))
                return getRequiredMoveCountSpecial(pk, moves, learn);

            // A pokemon is captured with initial moves and can't forget any until have all 4 slots used
            // If it has learn a move before having 4 it will be in one of the free slots
            int required = getRequiredMoveSlotsRegular(pk, moves, learn, initialmoves);
            return required != 0 ? required : getRequiredMoveCountDecrement(pk, moves, learn, initialmoves);
        }
        private static int getRequiredMoveSlotsRegular(PKM pk, int[] moves, List<int>[] learn, int[] initialmoves)
        {
            int species = pk.Species;
            int catch_rate = (pk as PK1).Catch_Rate;
            // Caterpie and Metapod evolution lines have different count of possible slots available if captured in different evolutionary phases
            // Example: a level 7 caterpie evolved into metapod will have 3 learned moves, a captured metapod will have only 1 move
            if ((species == 011 || species == 012) && catch_rate == 120)
            {
                // Captured as Metapod without Caterpie moves
                return initialmoves.Union(learn[1]).Distinct().Count(lm => lm != 0 && !G1CaterpieMoves.Contains(lm));
                // There is no valid Butterfree encounter in generation 1 games
            }
            if ((species == 014 || species == 015) && (catch_rate == 45 || catch_rate == 120))
            {
                if (species == 15 && catch_rate == 45) // Captured as Beedril without Weedle and Kakuna moves
                    return initialmoves.Union(learn[1]).Distinct().Count(lm => lm != 0 && !G1KakunaMoves.Contains(lm));

                // Captured as Kakuna without Weedle moves
                return initialmoves.Union(learn[1]).Distinct().Count(lm => lm != 0 && !G1WeedleMoves.Contains(lm));
            }

            return getRequiredMoveCountSpecies3(species, pk.CurrentLevel, moves) ? 3 : 0; // no match
        }
        private static bool getRequiredMoveCountSpecies3(int species, int level, int[] moves)
        {
            // Species that evolve and learn the 4th move as evolved species at a greather level than base species
            // The 4th move is included in the level up table set as a preevolution move, 
            // it should be removed from the used slots count if is not the learn move
            switch (species)
            {
                case 017: return level < 21 && !moves.Contains(018); // Pidgeotto without Whirlwind
                case 028: return level < 27 && !moves.Contains(040); // Sandslash without Poison Sting
                case 047: return level < 30 && !moves.Contains(147); // Parasect without Spore
                case 055: return level < 39 && !moves.Contains(093); // Golduck without Confusion
                case 087: return level < 44 && !moves.Contains(156); // Dewgong without Rest
                case 093:
                case 094: return level < 29 && !moves.Contains(095); // Haunter/Gengar without Hypnosis
                case 110: return level < 39 && !moves.Contains(108); // Weezing without Smoke Screen
            }
            return false;
        }
        private static int getRequiredMoveCountDecrement(PKM pk, int[] moves, List<int>[] learn, int[] initialmoves)
        {
            int catch_rate = (pk as PK1).Catch_Rate;
            int usedslots = initialmoves.Union(learn[1]).Where(m => m != 0).Distinct().Count();
            // Yellow optional moves, reduce usedslots if the yellow move is not present
            // The count wont go bellow 1 because the yellow moves were already counted and are not the only initial or level up moves
            if (pk.Species == 031) //Venonat
            {
                // ignore Venomoth, by the time Venonat evolved it will always have 4 moves
                if (pk.CurrentLevel >= 11 && !moves.Contains(48)) // Supersonic
                    usedslots--;
                if (pk.CurrentLevel >= 19 && !moves.Contains(93)) // Confusion
                    usedslots--;
            }
            if (pk.Species == 056 && pk.CurrentLevel >= 9 && !moves.Contains(67)) // Mankey Yellow Low Kick, Primeape will always have 4 moves
                usedslots--;

            if (064 == pk.Species || pk.Species == 065)
            {
                if (catch_rate != 100)// Initial Yellow Kadabra Kinesis (move 134)
                    usedslots--;
                if (catch_rate == 200 && pk.CurrentLevel < 20) // Kadabra Disable, not learned until 20 if captured as Abra (move 50)
                    usedslots--;
            }
            if (104 == pk.Species || pk.Species == 105) // Cubone and Marowak
            {
                if (!moves.Contains(39)) // Initial Yellow Tail Whip 
                    usedslots--;
                if (!moves.Contains(125)) // Initial Yellow Bone Club
                    usedslots--;
                if (pk.Species == 105 && pk.CurrentLevel < 33 && !moves.Contains(116)) // Marowak evolved without Focus Energy
                    usedslots--;
            }
            if (pk.Species == 113) // Chansey 
            {
                if (!moves.Contains(39)) // Yellow Initial Tail Whip 
                    usedslots--;
                if (!moves.Contains(3)) // Yellow Lvl 12 and Initial Red/Blue Double Slap
                    usedslots--;
            }
            if (pk.Species == 130 && pk.CurrentLevel < 32) // Wild Gyarados from yellow do not learn splash, evolved gyarados do not learn tackle 
                usedslots--;
            if (pk.Species == 127 && pk.CurrentLevel >= 21 && !moves.Contains(20)) // Pinsir Yellow Bind
                usedslots--;
            return usedslots;
        }
        private static int getRequiredMoveCountSpecial(PKM pk, int[] moves, List<int>[] learn)
        {
            // Species with few mandatory slots, species with stone evolutions that could evolve at lower level and do not learn any more moves
            // and Pikachu and Nidoran family, those only have mandatory the initial moves and a few have one level up moves, 
            // every other move could be avoided switching game or evolving
            var basespecies = getBaseSpecies(pk);
            var maxlevel = 1;
            var minlevel = 1;
            if (029 <= pk.Species && pk.Species <= 034 && pk.CurrentLevel >= 8)
                maxlevel = 8; // Always lean a third move at level 8
            if (pk.Species == 114)
            {
                //Tangela moves before level 32 are different in red/blue and yellow
                minlevel = 32;
                maxlevel = pk.CurrentLevel;
            }
            var mandatory = minlevel <= pk.CurrentLevel ? getLvlMoves(basespecies, 0, 1, minlevel, maxlevel).Where(m => m != 0).Distinct().ToList() : new List<int>();
            if (pk.Species == 103 && pk.CurrentLevel >= 28) // Exeggutor
            {
                // At level 28 learn different move if is a Exeggute or Exeggutor
                if (moves.Contains(73))
                    mandatory.Add(73); // Leech Seed level 28 Exeggute
                if (moves.Contains(23))
                    mandatory.Add(23); // Stomp level 28 Exeggutor
            }
            if (pk.Species == 25 && pk.CurrentLevel >= 33)
                mandatory.Add(97); // Pikachu always learn Agility
            if (pk.Species == 114)
                mandatory.Add(132); // Tangela always learn Constrict as Initial Move

            // Add to used slots the non-mandatory moves from the learnset table that the pokemon have learned
            return mandatory.Count + moves.Where(m => m != 0 && mandatory.All(l => l != m) && learn[1].Any(t => t == m)).Count();
        }

        internal static bool getWasEgg23(PKM pkm)
        {
            if (pkm.IsEgg)
                return true;
            if (pkm.Format > 2 && pkm.Ball != 4)
                return false;
            if (pkm.Format == 3)
                return pkm.WasEgg;

            int lvl = pkm.CurrentLevel;
            if (lvl < 5)
                return false;

            if (pkm.Format > 3 && pkm.Met_Level <5)
                return false;
            if (pkm.Format > 3 && pkm.FatefulEncounter)
                return false;

            return getEvolutionValid(pkm);
        }

        // Generation Specific Fetching
        private static EvolutionTree getEvolutionTable(PKM pkm)
        {
            return getEvolutionTable(pkm.Format);
        }
        private static EvolutionTree getEvolutionTable(int generation)
        {
            switch (generation)
            {
                case 1:
                    return Evolves1;
                case 2:
                    return Evolves2;
                case 3:
                    return Evolves3;
                case 4:
                    return Evolves4;
                case 5:
                    return Evolves5;
                case 6:
                    return Evolves6;
                default:
                    return Evolves7;
            }
        }
        internal static EncounterStatic[] getEncounterStaticTable(PKM pkm, GameVersion gameSource = GameVersion.Any)
        {
            if (gameSource == GameVersion.Any)
                gameSource = (GameVersion)pkm.Version;

            switch (gameSource)
            {
                case GameVersion.RBY:
                case GameVersion.RD:
                case GameVersion.BU:
                case GameVersion.GN:
                case GameVersion.YW:
                    return StaticRBY;

                case GameVersion.GSC:
                case GameVersion.GD:
                case GameVersion.SV:
                case GameVersion.C:
                    if (pkm.Format != 2)
                        return StaticGSC;

                    if (pkm.HasOriginalMetLocation)
                        return StaticC;
                    if (pkm.Species > 151 && !FutureEvolutionsGen1.Contains(pkm.Species))
                        return StaticGS;

                    return StaticGSC;

                case GameVersion.R: return StaticR;
                case GameVersion.S: return StaticS;
                case GameVersion.E: return StaticE;
                case GameVersion.FR: return StaticFR;
                case GameVersion.LG: return StaticLG;
                case GameVersion.CXD: return Encounter_CXD;

                case GameVersion.D: return StaticD;
                case GameVersion.P: return StaticP;
                case GameVersion.Pt: return StaticPt;
                case GameVersion.HG: return StaticHG;
                case GameVersion.SS: return StaticSS;

                case GameVersion.B: return StaticB;
                case GameVersion.W: return StaticW;
                case GameVersion.B2: return StaticB2;
                case GameVersion.W2: return StaticW2;

                case GameVersion.X: return StaticX;
                case GameVersion.Y: return StaticY;
                case GameVersion.AS: return StaticA;
                case GameVersion.OR: return StaticO;

                case GameVersion.SN: return StaticSN;
                case GameVersion.MN: return StaticMN;

                default: return new EncounterStatic[0];
            }
        }
        internal static EncounterArea[] getEncounterTable(PKM pkm, GameVersion gameSource = GameVersion.Any)
        {
            if (gameSource == GameVersion.Any)
                gameSource = (GameVersion)pkm.Version;

            switch (gameSource)
            {
                case GameVersion.RBY:
                case GameVersion.RD:
                case GameVersion.BU:
                case GameVersion.GN:
                case GameVersion.YW:
                    return SlotsRBY;

                case GameVersion.GSC:
                case GameVersion.GD:
                case GameVersion.SV:
                case GameVersion.C:
                    if (pkm.Format != 2)
                        // Gen 2 met location is lost outside gen 2 games
                        return SlotsGSC;

                    if (pkm.HasOriginalMetLocation)
                        // Format 2 with met location, encounter should be from crystal
                        return SlotsC;

                    if (pkm.Species > 151 && !FutureEvolutionsGen1.Contains(pkm.Species))
                        // Format 2 without met location but pokemon could not be tradeback to gen 1, 
                        // encounter should be from gold or silver
                        return SlotsGS;

                    // Encounter could be any gen 2 game, it can have empty met location for have a g/s origin
                    // or it can be a crystal pokemon that lost met location after being tradeback to gen 1 games
                    return SlotsGSC;

                case GameVersion.R: return SlotsR;
                case GameVersion.S: return SlotsS;
                case GameVersion.E: return SlotsE;
                case GameVersion.FR: return SlotsFR;
                case GameVersion.LG: return SlotsLG;
                case GameVersion.CXD: return SlotsXD;

                case GameVersion.D: return SlotsD;
                case GameVersion.P: return SlotsP;
                case GameVersion.Pt: return SlotsPt;
                case GameVersion.HG: return SlotsHG;
                case GameVersion.SS: return SlotsSS;

                case GameVersion.B: return SlotsB;
                case GameVersion.W: return SlotsW;
                case GameVersion.B2: return SlotsB2;
                case GameVersion.W2: return SlotsW2;

                case GameVersion.X: return SlotsX;
                case GameVersion.Y: return SlotsY;
                case GameVersion.AS: return SlotsA;
                case GameVersion.OR: return SlotsO;

                case GameVersion.SN: return SlotsSN;
                case GameVersion.MN: return SlotsMN;

                default: return new EncounterArea[0];
            }
        }
        internal static IEnumerable<EncounterArea> getDexNavAreas(PKM pkm)
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

        internal static IEnumerable<int> getLineage(PKM pkm)
        {
            if (pkm.IsEgg)
                return new[] {pkm.Species};

            var table = getEvolutionTable(pkm);
            var lineage = table.getValidPreEvolutions(pkm, pkm.CurrentLevel);
            return lineage.Select(evolution => evolution.Species);
        }
        internal static int[] getWildBalls(PKM pkm)
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

        internal static int[] getSplitBreedGeneration(PKM pkm)
        {
            return getSplitBreedGeneration(pkm.GenNumber);
        }
        internal static int[] getSplitBreedGeneration(int generation)
        {
            switch (generation)
            {
                case 1: return new int[0];
                case 2: return new int[0];
                case 3: return SplitBreed_3;
                case 4: return SplitBreed;
                case 5: return SplitBreed;
                case 6: return SplitBreed;
                case 7: return SplitBreed;
                default: return new int[0];
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
        internal static bool getEvolutionValid(PKM pkm, int minSpecies = -1)
        {
            var curr = getValidPreEvolutions(pkm);
            var poss = getValidPreEvolutions(pkm, lvl: 100, skipChecks: true);

            if (minSpecies != -1)
                poss = poss.Reverse().SkipWhile(z => z.Species != minSpecies); // collection is reversed, we only care about count
            else if (getSplitBreedGeneration(pkm).Contains(getBaseSpecies(pkm, 1)))
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
                // Capture Memory only obtainable via Gen 6.
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
                    }
                    break;
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
            return generations.Any(generation => getCanLearnMachineMove(pkm, move, generation, version));
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
        internal static int getBaseEggSpecies(PKM pkm, int skipOption = 0)
        {
            if (pkm.Format == 1)
                return getBaseSpecies(pkm, skipOption : skipOption, generation : 2);
            return getBaseSpecies(pkm, skipOption);
        }
        internal static int getBaseSpecies(PKM pkm, int skipOption = 0, int generation = -1)
        {
            if (pkm.Species == 292)
                return 290;
            if (pkm.Species == 242 && pkm.CurrentLevel < 3) // Never Cleffa
                return 113;

            var table = generation != -1 ? getEvolutionTable(generation): getEvolutionTable(pkm);
            int maxSpeciesOrigin = generation != -1 ? getMaxSpeciesOrigin(generation) : - 1;
            var evos = table.getValidPreEvolutions(pkm, 100, maxSpeciesOrigin: maxSpeciesOrigin, skipChecks:true).ToArray();

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
                return -1;

            if (pkm.Format <= 2)
            {
                if (generation == 1 && FutureEvolutionsGen1_Gen2LevelUp.Contains(pkm.Species))
                    return pkm.CurrentLevel - 1;
                return pkm.CurrentLevel;
            }

            if (pkm.Species == 700 && generation == 5)
                return pkm.CurrentLevel - 1;

            if (pkm.Gen3 && pkm.Format > 4 && pkm.Met_Level == pkm.CurrentLevel && FutureEvolutionsGen3_LevelUpGen4.Contains(pkm.Species))
                return pkm.Met_Level - 1;

            if (!pkm.HasOriginalMetLocation)
                return pkm.Met_Level;
            
            return pkm.CurrentLevel;
        }
        internal static int getMinLevelEncounter(PKM pkm)
        {
            if (pkm.Format == 3 && pkm.WasEgg)
                // Only for gen 3 pokemon in format 3, after transfer to gen 4 it should return transfer level
                return 5;
            if (pkm.Format == 4 && pkm.GenNumber == 4 && pkm.WasEgg) 
                // Only for gen 4 pokemon in format 4, after transfer to gen 5 it should return transfer level
                return 1;
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
            
            if (!pkm.HasOriginalMetLocation && generation != pkm.GenNumber)
                return pkm.Met_Level;

            if (pkm.GenNumber <= 3)
                return 2;

            return 1;
        }

        internal static DexLevel[][] getEvolutionChainsAllGens(PKM pkm, IEncounterable Encounter)
        {
            var CompleteEvoChain = getEvolutionChain(pkm, Encounter).ToArray();
            int maxgen = pkm.Format == 1 && !pkm.Gen1_NotTradeback ? 2 : pkm.Format;
            int mingen = pkm.Format == 2 && !pkm.Gen2_NotTradeback ? 1 : pkm.GenNumber;
            DexLevel[][] GensEvoChains = new DexLevel[maxgen + 1][];
            for (int i = 0; i <= maxgen; i++)
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

            // Iterate generations backwards because level will be decreased from current level in each generation
            for (int gen = maxgen; gen >= mingen; gen--)
            {
                if (pkm.GenNumber == 1 && pkm.Gen1_NotTradeback && gen == 2)
                    continue;
                if (pkm.GenNumber <= 2 && 3 <= gen && gen <= 6)
                    continue;
                if (!pkm.HasOriginalMetLocation && pkm.Format > 2 && gen < pkm.Format && gen <= 4 && lvl > pkm.Met_Level)
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
                    GensEvoChains[gen] = GensEvoChains[gen].Where(e => e.Level >= getMinLevelGeneration(pkm, gen)).ToArray();

                if (gen == 1 && GensEvoChains[gen].LastOrDefault()?.Species > MaxSpeciesID_1)
                    // Remove generation 2 pre-evolutions
                    GensEvoChains[gen] = GensEvoChains[gen].Take(GensEvoChains[gen].Length - 1).ToArray();
            }
            return GensEvoChains;
        }
        internal static DexLevel[] getEvolutionChain(PKM pkm, IEncounterable Encounter)
        {
            return getEvolutionChain(pkm, Encounter, pkm.Species, 100);
        }

        private static DexLevel[] getEvolutionChain(PKM pkm, IEncounterable Encounter, int maxspec, int maxlevel)
        {
            DexLevel[] vs = getValidPreEvolutions(pkm).ToArray();

            // Evolution chain is in reverse order (devolution)
            int minspec = Encounter.Species;

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
        internal static string getEncounterTypeName(IEncounterable Encounter)
        {
            if (Encounter == null)
                return "Unknown";
            return Encounter.Name;
        }
        private static IEnumerable<int> getRelearnLVLMoves(PKM pkm, int species, int lvl, int formnum)
        {
            List<int> moves = new List<int>();
            switch (pkm.GenNumber)
            {
                case 6:
                    if (pkm.InhabitedGeneration(6))
                    {
                        int ind_XY = PersonalTable.XY.getFormeIndex(species, formnum);
                        moves.AddRange(LevelUpXY[ind_XY].getMoves(lvl));
                        int ind_AO = PersonalTable.AO.getFormeIndex(species, formnum);
                        moves.AddRange(LevelUpAO[ind_AO].getMoves(lvl));
                    }
                    break;
                case 7:
                    if (pkm.InhabitedGeneration(7))
                    {
                        int ind_SM = PersonalTable.SM.getFormeIndex(species, formnum);
                        moves.AddRange(LevelUpSM[ind_SM].getMoves(lvl));
                    }
                    break;
            }
            return moves;
        }
        internal static IEnumerable<DexLevel> getValidPreEvolutions(PKM pkm, int maxspeciesorigin = -1, int lvl = -1, bool skipChecks = false)
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
                    new DexLevel { Species = 292, Level = lvl, MinLevel = 20 },
                    new DexLevel { Species = 290, Level = lvl - 1, MinLevel = 1 }
                };
            if (maxspeciesorigin == -1 && pkm.InhabitedGeneration(2) && pkm.GenNumber == 1)
                maxspeciesorigin = MaxSpeciesID_2;

            var et = maxspeciesorigin == MaxSpeciesID_2 ? getEvolutionTable(2) : getEvolutionTable(pkm);
            return et.getValidPreEvolutions(pkm, lvl: lvl, maxSpeciesOrigin: maxspeciesorigin, skipChecks: skipChecks);
        }
        private static IEnumerable<int> getValidMoves(PKM pkm, GameVersion Version, IReadOnlyList<DexLevel[]> vs, int minLvLG1 = 1, bool LVL = false, bool Relearn = false, bool Tutor = false, bool Machine = false, bool MoveReminder = true, bool RemoveTransferHM = true)
        {
            List<int> r = new List<int> { 0 };
            if (Relearn && pkm.Format >= 6)
                r.AddRange(pkm.RelearnMoves);

            for (int gen = pkm.GenNumber; gen <= pkm.Format; gen++)
                if (vs[gen].Any())
                    r.AddRange(getValidMoves(pkm, Version, vs[gen], gen, minLvLG1:minLvLG1, LVL: LVL, Relearn: false, Tutor: Tutor, Machine: Machine, MoveReminder: MoveReminder, RemoveTransferHM: RemoveTransferHM));

            return r.Distinct().ToArray();
        }
        private static IEnumerable<int> getValidMoves(PKM pkm, GameVersion Version, DexLevel[] vs, int Generation, int minLvLG1 = 1, bool LVL = false, bool Relearn = false, bool Tutor = false, bool Machine = false, bool MoveReminder = true, bool RemoveTransferHM = true)
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
                if (species == 386 && pkm.Format == 3)
                    // In gen 3 deoxys has different forms depending on the current game, in personal info there is no alter form info
                    formcount = 4;
                for (int i = 0; i < formcount; i++)
                    r.AddRange(getMoves(pkm, species, minLvLG1, vs.First().Level, i, moveTutor, Version, LVL, Tutor, Machine, MoveReminder, RemoveTransferHM, Generation));
                if (Relearn) r.AddRange(pkm.RelearnMoves);
                return r.Distinct();
            }

            foreach (DexLevel evo in vs)
            {
                var minlvlevo = 1;
                if (Generation == 1)
                {
                    // Return moves from minLvLG1 if species if the species encounters
                    // For evolutions return moves using evolution min level as min level
                    minlvlevo = minLvLG1;
                    if (evo.MinLevel > 1)
                        minlvlevo = Math.Min(pkm.CurrentLevel, evo.MinLevel);
                }
                r.AddRange(getMoves(pkm, evo.Species, minlvlevo, evo.Level, pkm.AltForm, moveTutor, Version, LVL, Tutor, Machine, MoveReminder, RemoveTransferHM, Generation));
            }

            if (pkm.Format <= 3)
                return r.Distinct();
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
            return r.Distinct();
        }
        private static IEnumerable<int> getMoves(PKM pkm, int species, int minlvlG1, int lvl, int form, bool moveTutor, GameVersion Version, bool LVL, bool specialTutors, bool Machine, bool MoveReminder, bool RemoveTransferHM, int Generation)
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
                            if (minlvlG1 == 1)
                            {
                                r.AddRange(pi_rb.Moves);
                                r.AddRange(pi_y.Moves);
                            }
                            r.AddRange(LevelUpRB[index].getMoves(lvl, minlvlG1));
                            r.AddRange(LevelUpY[index].getMoves(lvl, minlvlG1));
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
                        if (index == 0)
                            return r;
                        if (LVL)
                        {
                            r.AddRange(LevelUpGS[index].getMoves(lvl));
                            r.AddRange(LevelUpC[index].getMoves(lvl));
                        }
                        if (Machine)
                        {
                            var pi_c = (PersonalInfoG2)PersonalTable.C[index];
                            r.AddRange(TMHM_GSC.Where((t, m) => pi_c.TMHM[m]));
                        }
                        if (moveTutor)
                            r.AddRange(getTutorMoves(pkm, species, form, specialTutors, Generation));
                        if (pkm.Format == 1) //tradeback gen 2 -> gen 1
                            r = r.Where(m => m <= MaxMoveID_1).ToList();
                        break;
                    }
                case 3:
                    {
                        int index = PersonalTable.E.getFormeIndex(species, 0);
                        if (index == 0)
                            return r;
                        if (LVL)
                        {
                            if (index == 386)
                            {
                                switch(form)
                                {
                                    case 0: r.AddRange(LevelUpRS[index].getMoves(lvl)); break;
                                    case 1: r.AddRange(LevelUpFR[index].getMoves(lvl)); break;
                                    case 2: r.AddRange(LevelUpLG[index].getMoves(lvl)); break;
                                    case 3: r.AddRange(LevelUpE[index].getMoves(lvl)); break;
                                }
                            }
                            else
                            {
                                // Emerald level up table are equals to R/S level up tables
                                r.AddRange(LevelUpE[index].getMoves(lvl));
                                // fire red and leaf green are equals between each other but different than RSE
                                // Do not use FR Levelup table. It have 67 moves for charmander but Leaf Green moves table is correct
                                r.AddRange(LevelUpLG[index].getMoves(lvl));
                            }
                        }
                        if (Machine)
                        {
                            var pi_c = PersonalTable.E[index];
                            r.AddRange(TM_3.Where((t, m) => pi_c.TMHM[m]));
                            if (!RemoveTransferHM || pkm.Format == 3) // HM moves must be removed for 3->4, only give if current format.
                                r.AddRange(HM_3.Where((t, m) => pi_c.TMHM[m+50]));
                        }
                        if (moveTutor)
                            r.AddRange(getTutorMoves(pkm, species, form, specialTutors, Generation));
                        if (RemoveTransferHM && pkm.Format > 3) //Remove HM
                            r = r.Except(HM_3).ToList();
                        break;
                    }
                case 4:
                    {
                        int index = PersonalTable.HGSS.getFormeIndex(species, 0);
                        if (index == 0)
                            return r;
                        if (LVL)
                        {
                            r.AddRange(LevelUpDP[index].getMoves(lvl));
                            r.AddRange(LevelUpPt[index].getMoves(lvl));
                            r.AddRange(LevelUpHGSS[index].getMoves(lvl));
                        }
                        if (Machine)
                        {
                            var pi_hgss = PersonalTable.HGSS[index];
                            var pi_dppt = PersonalTable.Pt[index];
                            r.AddRange(TM_4.Where((t, m) => pi_hgss.TMHM[m]));
                            if (RemoveTransferHM && pkm.Format > 4)
                            {
                                // The combination of both these moves is illegal, it should be checked that the pokemon only learn one
                                // except if it can learn any of these moves in gen 5 or later
                                if (pi_hgss.TMHM[96])
                                    r.Add(250); // Whirlpool
                                if (pi_dppt.TMHM[96])
                                    r.Add(432); // Defog
                            }
                            else
                            {
                                r.AddRange(HM_DPPt.Where((t, m) => pi_dppt.TMHM[m + 92]));
                                r.AddRange(HM_HGSS.Where((t, m) => pi_hgss.TMHM[m + 92]));
                            }
                        }
                        if (moveTutor)
                            r.AddRange(getTutorMoves(pkm, species, form, specialTutors, Generation));
                        if (RemoveTransferHM && pkm.Format > 4) //Remove HM
                            r = r.Except(HM_4_RemovePokeTransfer).ToList();
                        break;
                    }
                case 5:
                    {
                        int index = PersonalTable.B2W2.getFormeIndex(species, 0);
                        if (index == 0)
                            return r;
                        if (LVL)
                        {
                            r.AddRange(LevelUpBW[index].getMoves(lvl));
                            r.AddRange(LevelUpB2W2[index].getMoves(lvl));
                        }
                        if (Machine)
                        {
                            var pi_c = PersonalTable.B2W2[index];
                            r.AddRange(TMHM_BW.Where((t, m) => pi_c.TMHM[m]));
                        }
                        if (moveTutor)
                            r.AddRange(getTutorMoves(pkm, species, form, specialTutors, Generation));
                        break;
                    }
                case 6:
                    switch (ver)
                    {
                        case GameVersion.Any: // Start at the top, hit every table
                        case GameVersion.X: case GameVersion.Y: case GameVersion.XY:
                        {
                            int index = PersonalTable.XY.getFormeIndex(species, form);
                            if (index == 0)
                                return r;

                            if (LVL)
                                r.AddRange(LevelUpXY[index].getMoves(lvl));
                            if (moveTutor)
                                 r.AddRange(getTutorMoves(pkm, species, form, specialTutors, Generation));
                            if (Machine)
                            {
                                PersonalInfo pi = PersonalTable.XY[index];
                                r.AddRange(TMHM_XY.Where((t, m) => pi.TMHM[m]));
                            }

                            if (ver == GameVersion.Any) // Fall Through
                                goto case GameVersion.ORAS;
                            break;
                        }

                        case GameVersion.AS: case GameVersion.OR: case GameVersion.ORAS:
                        {
                            int index = PersonalTable.AO.getFormeIndex(species, form);
                            if (index == 0)
                                return r;

                            if (LVL)
                                r.AddRange(LevelUpAO[index].getMoves(lvl));
                            if (moveTutor)
                                r.AddRange(getTutorMoves(pkm, species, form, specialTutors, Generation));
                            if (Machine)
                            {
                                PersonalInfo pi = PersonalTable.AO[index];
                                r.AddRange(TMHM_AO.Where((t, m) => pi.TMHM[m]));
                            }
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
                            if (MoveReminder)
                                lvl = 100; // Move reminder can teach any level in movepool now!

                            if (LVL)
                                r.AddRange(LevelUpSM[index].getMoves(lvl));
                            if (moveTutor) 
                                r.AddRange(getTutorMoves(pkm, species, form, specialTutors, Generation));
                            if (Machine)
                            {
                                PersonalInfo pi = PersonalTable.SM.getFormeEntry(species, form);
                                r.AddRange(TMHM_SM.Where((t, m) => pi.TMHM[m]));
                            }
                            break;
                        }
                    }
                    break;

                default:
                    return r;
            }
            return r;
        }

        internal static int[] getEggMoves(PKM pkm, int species, int formnum, GameVersion Version = GameVersion.Any)
        {
            if (!pkm.InhabitedGeneration(pkm.GenNumber, species))
                return new int[0];

            switch (pkm.GenNumber)
            {
                case 1:
                case 2:
                    if (pkm.Format != 2)
                        return EggMovesC[species].Moves;
                    if (pkm.HasOriginalMetLocation)
                        return EggMovesC[species].Moves;
                    if (pkm.Species > 151 && !FutureEvolutionsGen1.Contains(pkm.Species))
                        return EggMovesGS[species].Moves;
                    return EggMovesC[species].Moves;
                case 3:
                    return EggMovesRS[species].Moves;
                case 4:
                    switch (Version)
                    {
                        case GameVersion.DP:
                        case GameVersion.Pt:
                            return EggMovesDPPt[species].Moves;
                        case GameVersion.HGSS:
                            return EggMovesHGSS[species].Moves;
                        default:
                            return new int[0];
                    }
                case 5:
                    return EggMovesBW[species].Moves;
                case 6: // entries per species
                    return EggMovesAO[species].Moves.Concat(EggMovesXY[species].Moves).ToArray();

                case 7: // entries per form if required
                    var entry = EggMovesSM[species];
                    if (formnum > 0 && AlolanOriginForms.Contains(species))
                        entry = EggMovesSM[entry.FormTableIndex + formnum - 1];
                    return entry.Moves;

                default:
                    return new int[0];
            }
        }
        internal static IEnumerable<int> getTMHM(PKM pkm, int species, int form, int generation, GameVersion Version = GameVersion.Any, bool RemoveTransferHM = true)
        {
            List<int> moves = new List<int>();
            int index;
            switch (generation)
            {
                case 1:
                    index = PersonalTable.RB.getFormeIndex(species, 0);
                    if (index == 0)
                        return moves;
                    var pi_rb = (PersonalInfoG1)PersonalTable.RB[index];
                    var pi_y = (PersonalInfoG1)PersonalTable.Y[index];
                    moves.AddRange(TMHM_RBY.Where((t, m) => pi_rb.TMHM[m]));
                    moves.AddRange(TMHM_RBY.Where((t, m) => pi_y.TMHM[m]));
                    break;
                case 2:
                    index = PersonalTable.C.getFormeIndex(species, 0);
                    if (index == 0)
                        return moves;
                    var pi_c = (PersonalInfoG2)PersonalTable.C[index];
                    moves.AddRange(TMHM_GSC.Where((t, m) => pi_c.TMHM[m]));
                    if (Version == GameVersion.Any)
                        goto case 1; // rby
                    break;
                case 3:
                    index = PersonalTable.E.getFormeIndex(species, 0);
                    var pi_e = PersonalTable.E[index];
                    moves.AddRange(TM_3.Where((t, m) => pi_e.TMHM[m]));
                    if (!RemoveTransferHM || pkm.Format == 3) // HM moves must be removed for 3->4, only give if current format.
                        moves.AddRange(HM_3.Where((t, m) => pi_e.TMHM[m + 50]));
                    break;
                case 4:
                    index = PersonalTable.HGSS.getFormeIndex(species, 0);
                    if (index == 0)
                        return moves;
                    var pi_hgss = PersonalTable.HGSS[index];
                    var pi_dppt = PersonalTable.Pt[index];
                    moves.AddRange(TM_4.Where((t, m) => pi_hgss.TMHM[m]));
                    // The combination of both these moves is illegal, it should be checked that the pokemon only learn one
                    // except if it can learn any of these moves in gen 5 or later
                    if (Version == GameVersion.Any || Version == GameVersion.DP || Version == GameVersion.D || Version == GameVersion.P || Version == GameVersion.Pt)
                    {
                        if (RemoveTransferHM && pkm.Format > 4)
                        {
                            if (pi_dppt.TMHM[96])
                                moves.Add(432); // Defog
                        }
                        else
                        {
                            moves.AddRange(HM_DPPt.Where((t, m) => pi_dppt.TMHM[m + 92]));
                        }
                    }
                    if (Version == GameVersion.Any || Version == GameVersion.HGSS || Version == GameVersion.HG || Version == GameVersion.SS)
                    {
                        if (RemoveTransferHM && pkm.Format > 4)
                        {
                            if (pi_hgss.TMHM[96])
                                moves.Add(432); // Defog
                        }
                        else
                        {
                            moves.AddRange(HM_HGSS.Where((t, m) => pi_dppt.TMHM[m + 92]));
                        }
                    }
                    break;
                case 5:
                    index = PersonalTable.B2W2.getFormeIndex(species, 0);
                    if (index == 0)
                        return moves;

                    var pi_bw = PersonalTable.B2W2[index];
                    moves.AddRange(TMHM_BW.Where((t, m) => pi_bw.TMHM[m]));
                    break;
                case 6:
                    switch (Version)
                    {
                        case GameVersion.Any: // Start at the top, hit every table
                        case GameVersion.X:
                        case GameVersion.Y:
                        case GameVersion.XY:
                            {
                                index = PersonalTable.XY.getFormeIndex(species, form);
                                if (index == 0)
                                    return moves;

                                PersonalInfo pi_xy = PersonalTable.XY[index];
                                moves.AddRange(TMHM_XY.Where((t, m) => pi_xy.TMHM[m]));
                                
                                if (Version == GameVersion.Any) // Fall Through
                                    goto case GameVersion.ORAS;
                                break;
                            }
                        case GameVersion.AS:
                        case GameVersion.OR:
                        case GameVersion.ORAS:
                            {
                                index = PersonalTable.AO.getFormeIndex(species, form);
                                if (index == 0)
                                    return moves;

                                PersonalInfo pi_ao = PersonalTable.AO[index];
                                moves.AddRange(TMHM_AO.Where((t, m) => pi_ao.TMHM[m]));
                                break;
                            }
                    }
                    break;
                case 7:
                    index = PersonalTable.SM.getFormeIndex(species, form);
                    if (index == 0)
                        return moves;

                    PersonalInfo pi_sm = PersonalTable.SM[index];
                    moves.AddRange(TMHM_SM.Where((t, m) => pi_sm.TMHM[m]));
                    break;
            }
            return moves.Distinct();
        }
        internal static IEnumerable<int> getTutorMoves(PKM pkm, int species, int form, bool specialTutors, int generation)
        {
            List<int> moves = new List<int>();
            PersonalInfo info;
            switch (generation)
            {
                case 1:
                    if (AllowGBCartEra && pkm.Format < 3 && (pkm.Species == 25 || pkm.Species == 26)) // Surf Pikachu via Stadium
                        moves.Add(57);
                    break;
                case 2:
                    info = PersonalTable.C[species];
                    moves.AddRange(Tutors_GSC.Where((t, i) => info.TMHM[57 + i]));
                    goto case 1;
                case 3:
                    // E Tutors (Free)
                    // E Tutors (BP)
                    info = PersonalTable.E[species];
                    moves.AddRange(Tutor_E.Where((t, i) => info.TypeTutors[i]));
                    // FRLG Tutors
                    // Only special tutor moves, normal tutor moves are already included in Emerald data
                    moves.AddRange(SpecialTutors_FRLG.Where((t, i) => SpecialTutors_Compatibility_FRLG[i].Any(e => e == species)));
                    // XD
                    moves.AddRange(SpecialTutors_XD_Exclusive.Where((t, i) => SpecialTutors_Compatibility_XD_Exclusive[i].Any(e => e == species)));
                    // XD (Mew)
                    if (species == 151)
                        moves.AddRange(Tutor_3Mew);

                    break;
                case 4:
                    info = PersonalTable.HGSS[species];
                    moves.AddRange(Tutors_4.Where((t, i) => info.TypeTutors[i]));
                    moves.AddRange(SpecialTutors_4.Where((t, i) => SpecialTutors_Compatibility_4[i].Any(e => e == species)));
                    break;
                case 5:
                    info = PersonalTable.B2W2[species];
                    moves.AddRange(TypeTutor6.Where((t, i) => info.TypeTutors[i]));
                    if (pkm.InhabitedGeneration(5) && specialTutors)
                    {
                        PersonalInfo pi = PersonalTable.B2W2.getFormeEntry(species, form);
                        for (int i = 0; i < Tutors_B2W2.Length; i++)
                            for (int b = 0; b < Tutors_B2W2[i].Length; b++)
                                if (pi.SpecialTutors[i][b])
                                    moves.Add(Tutors_B2W2[i][b]);
                    }
                    break;
                case 6:
                    info = PersonalTable.AO[species];
                    moves.AddRange(TypeTutor6.Where((t, i) => info.TypeTutors[i]));
                    if (pkm.InhabitedGeneration(6) && specialTutors && (pkm.AO || !pkm.IsUntraded))
                    {
                        PersonalInfo pi = PersonalTable.AO.getFormeEntry(species, form);
                        for (int i = 0; i < Tutors_AO.Length; i++)
                            for (int b = 0; b < Tutors_AO[i].Length; b++)
                                if (pi.SpecialTutors[i][b])
                                    moves.Add(Tutors_AO[i][b]);
                    }
                    break;
                case 7:
                    info = PersonalTable.SM.getFormeEntry(species, form);
                    moves.AddRange(TypeTutor6.Where((t, i) => info.TypeTutors[i]));
                    // No special tutors in G7
                    break;
            }
            return moves.Distinct();
        }
        internal static List<int>[] GetEmptyMovesList(DexLevel[][] EvoChainsAllGens)
        {
            var empty = new List<int>[EvoChainsAllGens.Length];
            for (int i = 0; i < empty.Length; i++)
                empty[i] = new List<int>();
            return empty;
        }
        internal static bool IsOutsider(PKM pkm)
        {
            var Outsider = Savegame_TID != pkm.TID || Savegame_OT != pkm.OT_Name;
            if (pkm.Format <= 2)
                return Outsider;
            Outsider |= Savegame_SID != pkm.SID;
            if (pkm.Format == 3) // Generation 3 does not check ot geneder nor pokemon version
                return Outsider;
            Outsider |= Savegame_Gender != pkm.OT_Gender || Savegame_Version != (GameVersion) pkm.Version;
            return Outsider;
        }
    }
}
