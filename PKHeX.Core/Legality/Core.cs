using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static PKHeX.Core.Encounters1;
using static PKHeX.Core.Encounters2;
using static PKHeX.Core.Encounters3;
using static PKHeX.Core.Encounters4;
using static PKHeX.Core.Encounters5;
using static PKHeX.Core.Encounters6;
using static PKHeX.Core.Encounters7;
using static PKHeX.Core.EncountersWC3;
using static PKHeX.Core.LegalityCheckStrings;

namespace PKHeX.Core
{
    public static partial class Legal
    {
        /// <summary>Event Database for Generation 3</summary>
        public static MysteryGift[] MGDB_G3 { get; private set; } = new MysteryGift[0];

        /// <summary>Event Database for Generation 4</summary>
        public static MysteryGift[] MGDB_G4 { get; private set; } = new MysteryGift[0];

        /// <summary>Event Database for Generation 5</summary>
        public static MysteryGift[] MGDB_G5 { get; private set; } = new MysteryGift[0];

        /// <summary>Event Database for Generation 6</summary>
        public static MysteryGift[] MGDB_G6 { get; private set; } = new MysteryGift[0];

        /// <summary>Event Database for Generation 7</summary>
        public static MysteryGift[] MGDB_G7 { get; private set; } = new MysteryGift[0];

        /// <summary>Setting to specify if an analysis should permit data sourced from the physical cartridge era of GameBoy games.</summary>
        public static bool AllowGBCartEra { get; set; }
        public static bool AllowGen1Tradeback { get; set; }
        public static bool AllowGen2VCTransfer => true;
        public static bool AllowGen2VCCrystal => false;
        public static bool AllowGen2Crystal(bool Korean) => !Korean && (AllowGBCartEra || AllowGen2VCCrystal); // Pokemon Crystal was never released in Korea
        public static bool AllowGen2Crystal(PKM pkm) => AllowGen2Crystal(pkm.Korean);
        public static bool AllowGen2MoveReminder(PKM pkm) => !pkm.Korean && AllowGBCartEra; // Pokemon Stadium 2 was never released in Korea

        public static bool CheckWordFilter { get; set; } = true;

        public static int SavegameLanguage { get; set; }
        /// <summary> e-Reader Berry originates from a Japanese SaveFile </summary>
        public static bool SavegameJapanese { get; set; }
        /// <summary> e-Reader Berry is Enigma or special berry </summary>
        public static bool EReaderBerryIsEnigma { get; set; } = true;
        /// <summary> e-Reader Berry Name </summary>
        public static string EReaderBerryName { get; set; } = string.Empty;
        /// <summary> e-Reader Berry Name formatted in Title Case </summary>
        public static string EReaderBerryDisplayName => string.Format(V372, Util.ToTitleCase(EReaderBerryName.ToLower()));

        public static string Savegame_OT { private get; set; } = string.Empty;
        public static int Savegame_TID { private get; set; }
        public static int Savegame_SID { private get; set; }
        public static int Savegame_Gender { private get; set; }
        public static GameVersion Savegame_Version { private get; set; } = GameVersion.Any;

        // Gen 1
        private static readonly Learnset[] LevelUpRB = Learnset1.GetArray(Util.GetBinaryResource("lvlmove_rb.pkl"), MaxSpeciesID_1);
        private static readonly Learnset[] LevelUpY = Learnset1.GetArray(Util.GetBinaryResource("lvlmove_y.pkl"), MaxSpeciesID_1);

        // Gen 2
        private static readonly EggMoves[] EggMovesGS = EggMoves2.GetArray(Util.GetBinaryResource("eggmove_gs.pkl"), MaxSpeciesID_2);
        private static readonly Learnset[] LevelUpGS = Learnset1.GetArray(Util.GetBinaryResource("lvlmove_gs.pkl"), MaxSpeciesID_2);
        private static readonly EggMoves[] EggMovesC = EggMoves2.GetArray(Util.GetBinaryResource("eggmove_c.pkl"), MaxSpeciesID_2);
        private static readonly Learnset[] LevelUpC = Learnset1.GetArray(Util.GetBinaryResource("lvlmove_c.pkl"), MaxSpeciesID_2);

        // Gen 3
        private static readonly Learnset[] LevelUpE = Learnset6.GetArray(Data.UnpackMini(Util.GetBinaryResource("lvlmove_e.pkl"), "em"));
        private static readonly Learnset[] LevelUpRS = Learnset6.GetArray(Data.UnpackMini(Util.GetBinaryResource("lvlmove_rs.pkl"), "rs"));
        private static readonly Learnset[] LevelUpFR = Learnset6.GetArray(Data.UnpackMini(Util.GetBinaryResource("lvlmove_fr.pkl"), "fr"));
        private static readonly Learnset[] LevelUpLG = Learnset6.GetArray(Data.UnpackMini(Util.GetBinaryResource("lvlmove_lg.pkl"), "lg"));
        private static readonly EggMoves[] EggMovesRS = EggMoves6.GetArray(Data.UnpackMini(Util.GetBinaryResource("eggmove_rs.pkl"), "rs"));

        // Gen 4
        private static readonly Learnset[] LevelUpDP = Learnset6.GetArray(Data.UnpackMini(Util.GetBinaryResource("lvlmove_dp.pkl"), "dp"));
        private static readonly Learnset[] LevelUpPt = Learnset6.GetArray(Data.UnpackMini(Util.GetBinaryResource("lvlmove_pt.pkl"), "pt"));
        private static readonly Learnset[] LevelUpHGSS = Learnset6.GetArray(Data.UnpackMini(Util.GetBinaryResource("lvlmove_hgss.pkl"), "hs"));
        private static readonly EggMoves[] EggMovesDPPt = EggMoves6.GetArray(Data.UnpackMini(Util.GetBinaryResource("eggmove_dppt.pkl"), "dp"));
        private static readonly EggMoves[] EggMovesHGSS = EggMoves6.GetArray(Data.UnpackMini(Util.GetBinaryResource("eggmove_hgss.pkl"), "hs"));

        // Gen 5
        private static readonly Learnset[] LevelUpBW = Learnset6.GetArray(Data.UnpackMini(Util.GetBinaryResource("lvlmove_bw.pkl"), "51"));
        private static readonly Learnset[] LevelUpB2W2 = Learnset6.GetArray(Data.UnpackMini(Util.GetBinaryResource("lvlmove_b2w2.pkl"), "52"));
        private static readonly EggMoves[] EggMovesBW = EggMoves6.GetArray(Data.UnpackMini(Util.GetBinaryResource("eggmove_bw.pkl"), "bw"));

        // Gen 6
        private static readonly EggMoves[] EggMovesXY = EggMoves6.GetArray(Data.UnpackMini(Util.GetBinaryResource("eggmove_xy.pkl"), "xy"));
        private static readonly Learnset[] LevelUpXY = Learnset6.GetArray(Data.UnpackMini(Util.GetBinaryResource("lvlmove_xy.pkl"), "xy"));
        private static readonly EggMoves[] EggMovesAO = EggMoves6.GetArray(Data.UnpackMini(Util.GetBinaryResource("eggmove_ao.pkl"), "ao"));
        private static readonly Learnset[] LevelUpAO = Learnset6.GetArray(Data.UnpackMini(Util.GetBinaryResource("lvlmove_ao.pkl"), "ao"));

        // Gen 7
        private static readonly EggMoves[] EggMovesSM = EggMoves7.GetArray(Data.UnpackMini(Util.GetBinaryResource("eggmove_sm.pkl"), "sm"));
        private static readonly Learnset[] LevelUpSM = Learnset7.GetArray(Data.UnpackMini(Util.GetBinaryResource("lvlmove_sm.pkl"), "sm"));
        private static readonly EggMoves[] EggMovesUSUM = EggMoves7.GetArray(Data.UnpackMini(Util.GetBinaryResource("eggmove_uu.pkl"), "uu"));
        private static readonly Learnset[] LevelUpUSUM = Learnset7.GetArray(Data.UnpackMini(Util.GetBinaryResource("lvlmove_uu.pkl"), "uu"));

        // Setup Help
        private static HashSet<MysteryGift> GetPCDDB(byte[] bin)
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
        private static HashSet<MysteryGift> GetPGFDB(byte[] bin)
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
        private static HashSet<MysteryGift> GetWC6DB(byte[] wc6bin, byte[] wc6full)
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
        private static HashSet<MysteryGift> GetWC7DB(byte[] wc7bin, byte[] wc7full)
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
            var g4 = GetPCDDB(Util.GetBinaryResource("pcd.pkl"));
            var g5 = GetPGFDB(Util.GetBinaryResource("pgf.pkl"));
            var g6 = GetWC6DB(Util.GetBinaryResource("wc6.pkl"), Util.GetBinaryResource("wc6full.pkl"));
            var g7 = GetWC7DB(Util.GetBinaryResource("wc7.pkl"), Util.GetBinaryResource("wc7full.pkl"));

            if (Directory.Exists(localDbPath))
                foreach (var file in Directory.EnumerateFiles(localDbPath, "*", SearchOption.AllDirectories))
                {
                    var fi = new FileInfo(file);
                    if (!MysteryGift.IsMysteryGift(fi.Length))
                        continue;

                    var gift = MysteryGift.GetMysteryGift(File.ReadAllBytes(file), fi.Extension);
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

        // Moves
        internal static int[] GetMinLevelLearnMove(int species, int Generation, List<int> moves)
        {
            var r = new int[moves.Count];
            switch (Generation)
            {
                case 1:
                    {
                        int index = PersonalTable.RB.GetFormeIndex(species, 0);
                        if (index == 0)
                            return r;

                        var pi_rb = ((PersonalInfoG1)PersonalTable.RB[index]).Moves;
                        var pi_y = ((PersonalInfoG1)PersonalTable.Y[index]).Moves;

                        for (int m = 0; m < moves.Count; m++)
                        {
                            if (pi_rb.Contains(moves[m]) || pi_y.Contains(moves[m]))
                                r[m] = 1;
                            else
                            {
                                var rb_level = LevelUpRB[index].GetLevelLearnMove(moves[m]);
                                var y_level = LevelUpY[index].GetLevelLearnMove(moves[m]);
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
        internal static int[] GetMaxLevelLearnMove(int species, int Generation, List<int> moves)
        {
            var r = new int[moves.Count];
            switch (Generation)
            {
                case 1:
                    {
                        int index = PersonalTable.RB.GetFormeIndex(species, 0);
                        if (index == 0)
                            return r;

                        var pi_rb = ((PersonalInfoG1)PersonalTable.RB[index]).Moves;
                        var pi_y = ((PersonalInfoG1)PersonalTable.Y[index]).Moves;

                        for (int m = 0; m < moves.Count; m++)
                        {
                            bool start = pi_rb.Contains(moves[m]) && pi_y.Contains(moves[m]);
                            r[m] = start ? 1 : Math.Max(LevelUpRB[index].GetLevelLearnMove(moves[m]), LevelUpY[index].GetLevelLearnMove(moves[m]));
                        }
                        break;
                    }
            }
            return r;
        }
        internal static List<int>[] GetExclusiveMoves(int species1, int species2, int Generation, IEnumerable<int> tmhm, IEnumerable<int> moves, bool korean)
        {
            // Return from two species the exclusive moves that only one could learn and also the current pokemon have it in its current moveset
            var moves1 = GetLvlMoves(species1, 0, Generation, 1, 100, korean).Distinct().ToList();
            var moves2 = GetLvlMoves(species2, 0, Generation, 1, 100, korean).Distinct().ToList();

            // Remove common moves and remove tmhm, remove not learned moves
            var common = new HashSet<int>(moves1.Intersect(moves2).Concat(tmhm));
            var hashMoves = new HashSet<int>(moves);
            moves1.RemoveAll(x => !hashMoves.Contains(x) || common.Contains(x));
            moves2.RemoveAll(x => !hashMoves.Contains(x) || common.Contains(x));
            return new[] { moves1, moves2 };
        }
        private static IEnumerable<int> GetLvlMoves(int species, int form, int Generation, int minlvl, int lvl, bool korean = true, GameVersion Version = GameVersion.Any)
        {
            var r = new List<int>();
            var ver = Version;
            switch (Generation)
            {
                case 1:
                    {
                        int index = PersonalTable.RB.GetFormeIndex(species, 0);
                        if (index == 0)
                            return r;

                        var pi_rb = (PersonalInfoG1)PersonalTable.RB[index];
                        var pi_y = (PersonalInfoG1)PersonalTable.Y[index];
                        if (minlvl == 1)
                        {
                            r.AddRange(pi_rb.Moves);
                            r.AddRange(pi_y.Moves);
                        }
                        r.AddRange(LevelUpRB[index].GetMoves(lvl, minlvl));
                        r.AddRange(LevelUpY[index].GetMoves(lvl, minlvl));
                        break;
                    }
                case 2:
                    {
                        int index = PersonalTable.C.GetFormeIndex(species, 0);
                        if (index == 0)
                            return r;
                        r.AddRange(LevelUpGS[index].GetMoves(lvl));
                        if (AllowGen2Crystal(korean))
                            r.AddRange(LevelUpC[index].GetMoves(lvl));
                        break;
                    }
                case 3:
                    {
                        int index = PersonalTable.E.GetFormeIndex(species, 0);
                        if (index == 0)
                            return r;
                        if (index == 386)
                        {
                            switch (form)
                            {
                                case 0: r.AddRange(LevelUpRS[index].GetMoves(lvl)); break;
                                case 1: r.AddRange(LevelUpFR[index].GetMoves(lvl)); break;
                                case 2: r.AddRange(LevelUpLG[index].GetMoves(lvl)); break;
                                case 3: r.AddRange(LevelUpE[index].GetMoves(lvl)); break;
                            }
                        }
                        else
                        {
                            // Emerald level up table are equals to R/S level up tables
                            r.AddRange(LevelUpE[index].GetMoves(lvl));
                            // fire red and leaf green are equals between each other but different than RSE
                            // Do not use FR Levelup table. It have 67 moves for charmander but Leaf Green moves table is correct
                            r.AddRange(LevelUpLG[index].GetMoves(lvl));
                        }
                        break;
                    }
                case 4:
                    {
                        int index = PersonalTable.HGSS.GetFormeIndex(species, 0);
                        if (index == 0)
                            return r;
                        r.AddRange(LevelUpDP[index].GetMoves(lvl));
                        r.AddRange(LevelUpPt[index].GetMoves(lvl));
                        r.AddRange(LevelUpHGSS[index].GetMoves(lvl));
                        break;
                    }
                case 5:
                    {
                        int index = PersonalTable.B2W2.GetFormeIndex(species, 0);
                        if (index == 0)
                            return r;
                        r.AddRange(LevelUpBW[index].GetMoves(lvl));
                        r.AddRange(LevelUpB2W2[index].GetMoves(lvl));
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
                                int index = PersonalTable.XY.GetFormeIndex(species, form);
                                if (index == 0)
                                    return r;
                                r.AddRange(LevelUpXY[index].GetMoves(lvl));
                                if (ver == GameVersion.Any) // Fall Through
                                    goto case GameVersion.ORAS;
                                break;
                            }

                        case GameVersion.AS:
                        case GameVersion.OR:
                        case GameVersion.ORAS:
                            {
                                int index = PersonalTable.AO.GetFormeIndex(species, form);
                                if (index == 0)
                                    return r;
                                r.AddRange(LevelUpAO[index].GetMoves(lvl));
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
                                int index = PersonalTable.SM.GetFormeIndex(species, form);
                                r.AddRange(LevelUpSM[index].GetMoves(lvl));
                                if (ver == GameVersion.Any) // Fall Through
                                    goto case GameVersion.USUM;
                                break;
                            }
                        case GameVersion.US:
                        case GameVersion.UM:
                        case GameVersion.USUM:
                        {
                            int index = PersonalTable.USUM.GetFormeIndex(species, form);
                            if (index == 0)
                                return r;
                            r.AddRange(LevelUpUSUM[index].GetMoves(lvl));
                            break;
                        }
                    }
                    break;
                default:
                    return r;
            }
            return r;
        } 
        internal static List<int>[] GetValidMovesAllGens(PKM pkm, DexLevel[][] evoChains, int minLvLG1 = 1, int minLvLG2 = 1, bool LVL = true, bool Tutor = true, bool Machine = true, bool MoveReminder = true, bool RemoveTransferHM = true)
        {
            List<int>[] Moves = new List<int>[evoChains.Length];
            for (int i = 1; i < evoChains.Length; i++)
                if (evoChains[i].Any())
                    Moves[i] = GetValidMoves(pkm, evoChains[i], i, minLvLG1, minLvLG2, LVL, Tutor, Machine, MoveReminder, RemoveTransferHM).ToList();
                else
                    Moves[i] = new List<int>();
            return Moves;
        }
        internal static IEnumerable<int> GetValidMoves(PKM pkm, DexLevel[][] evoChains, int minLvLG1 = 1, int minLvLG2 = 1, bool LVL = true, bool Tutor = true, bool Machine = true, bool MoveReminder = true, bool RemoveTransferHM = true)
        {
            GameVersion version = (GameVersion)pkm.Version;
            if (!pkm.IsUntraded)
                version = GameVersion.Any;
            return GetValidMoves(pkm, version, evoChains, minLvLG1: minLvLG1, minLvLG2: minLvLG2, LVL: LVL, Relearn: false, Tutor: Tutor, Machine: Machine, MoveReminder: MoveReminder, RemoveTransferHM: RemoveTransferHM);
        }
        internal static IEnumerable<int> GetValidMoves(PKM pkm, DexLevel[] evoChain, int generation, int minLvLG1 = 1, int minLvLG2 = 1, bool LVL = true, bool Tutor = true, bool Machine = true, bool MoveReminder = true, bool RemoveTransferHM = true)
        {
            GameVersion version = (GameVersion)pkm.Version;
            if (!pkm.IsUntraded)
                version = GameVersion.Any;
            return GetValidMoves(pkm, version, evoChain, generation, minLvLG1: minLvLG1, minLvLG2: minLvLG2, LVL: LVL, Relearn: false, Tutor: Tutor, Machine: Machine, MoveReminder: MoveReminder, RemoveTransferHM: RemoveTransferHM);
        }
        internal static IEnumerable<int> GetValidRelearn(PKM pkm, int species, bool inheritlvlmoves, GameVersion version = GameVersion.Any)
        {
            List<int> r = new List<int> { 0 };
            if (pkm.GenNumber < 6 || pkm.VC)
                return r;

            r.AddRange(GetRelearnLVLMoves(pkm, species, 1, pkm.AltForm, version));

            int form = pkm.AltForm;
            if (pkm.Format == 6 && pkm.Species != 678)
                form = 0;

            r.AddRange(GetEggMoves(pkm, species, form));
            if (inheritlvlmoves)
                r.AddRange(GetRelearnLVLMoves(pkm, species, 100, pkm.AltForm, version));
            return r.Distinct();
        }
        internal static List<int>[] GetShedinjaEvolveMoves(PKM pkm, int lvl = -1, int generation = 0)
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
                        r[3] = LevelUpE[291].GetMoves(lvl, 20).ToList();

                    if (generation == 0)
                        goto case 4;
                    break;
                case 4: // Ninjask have the same learnset in every gen 4 games
                    if (pkm.InhabitedGeneration(4))
                        r[4] = LevelUpPt[291].GetMoves(lvl, 20).ToList();
                    break;
            }
            return r;
        }
        internal static int[] GetBaseEggMoves(PKM pkm, int species, GameVersion gameSource, int lvl)
        {
            if (gameSource == GameVersion.Any)
                gameSource = (GameVersion) pkm.Version;

            switch (gameSource)
            {
                case GameVersion.GS:
                    // If checking back-transfer specimen (GSC->RBY), remove moves that must be deleted prior to transfer
                    int[] getRBYCompatibleMoves(int[] moves) => pkm.Format == 1 ? moves.Where(m => m <= MaxMoveID_1).ToArray() : moves;
                    if (pkm.InhabitedGeneration(2))
                        return getRBYCompatibleMoves(LevelUpGS[species].GetMoves(lvl));
                    break;
                case GameVersion.C:
                    if (pkm.InhabitedGeneration(2))
                        return getRBYCompatibleMoves(LevelUpC[species].GetMoves(lvl));
                    break;

                case GameVersion.R:
                case GameVersion.S:
                case GameVersion.RS:
                    if (pkm.InhabitedGeneration(3))
                        return LevelUpRS[species].GetMoves(lvl);
                    break;
                case GameVersion.E:
                    if (pkm.InhabitedGeneration(3))
                        return LevelUpE[species].GetMoves(lvl);
                    break;
                case GameVersion.FR:
                case GameVersion.LG:
                case GameVersion.FRLG:
                    // only difference in FR/LG is deoxys which doesn't breed.
                    if (pkm.InhabitedGeneration(3))
                        return LevelUpFR[species].GetMoves(lvl);
                    break;

                case GameVersion.D:
                case GameVersion.P:
                case GameVersion.DP:
                    if (pkm.InhabitedGeneration(4))
                        return LevelUpDP[species].GetMoves(lvl);
                    break;
                case GameVersion.Pt:
                    if (pkm.InhabitedGeneration(4))
                        return LevelUpPt[species].GetMoves(lvl);
                    break;
                case GameVersion.HG:
                case GameVersion.SS:
                case GameVersion.HGSS:
                    if (pkm.InhabitedGeneration(4))
                        return LevelUpHGSS[species].GetMoves(lvl);
                    break;

                case GameVersion.B:
                case GameVersion.W:
                case GameVersion.BW:
                    if (pkm.InhabitedGeneration(5))
                        return LevelUpBW[species].GetMoves(lvl);
                    break;

                case GameVersion.B2:
                case GameVersion.W2:
                case GameVersion.B2W2:
                    if (pkm.InhabitedGeneration(5))
                        return LevelUpBW[species].GetMoves(lvl);
                    break;

                case GameVersion.X:
                case GameVersion.Y:
                case GameVersion.XY:
                    if (pkm.InhabitedGeneration(6))
                        return LevelUpXY[species].GetMoves(lvl);
                    break;

                case GameVersion.AS:
                case GameVersion.OR:
                case GameVersion.ORAS:
                    if (pkm.InhabitedGeneration(6))
                        return LevelUpAO[species].GetMoves(lvl);
                    break;

                case GameVersion.SN:
                case GameVersion.MN:
                case GameVersion.SM:
                    if (pkm.InhabitedGeneration(7))
                    {
                        int index = PersonalTable.SM.GetFormeIndex(species, pkm.AltForm);
                        return LevelUpSM[index].GetMoves(lvl);
                    }
                    break;

                case GameVersion.US:
                case GameVersion.UM:
                case GameVersion.USUM:
                    if (pkm.InhabitedGeneration(7))
                    {
                        int index = PersonalTable.USUM.GetFormeIndex(species, pkm.AltForm);
                        return LevelUpUSUM[index].GetMoves(lvl);
                    }
                    break;
            }
            return new int[0];
        }
        internal static List<int> GetValidPostEvolutionMoves(PKM pkm, int Species, DexLevel[][] evoChains, GameVersion Version)
        {
            // Return moves that the pokemon could learn after evolving 
            var moves = new List<int>();
            for (int i = 1; i < evoChains.Length; i++)
                if (evoChains[i].Any())
                    moves.AddRange(GetValidPostEvolutionMoves(pkm, Species, evoChains[i], i, Version));
            if (pkm.GenNumber >= 6)
                moves.AddRange(pkm.RelearnMoves.Where(m => m != 0));
            return moves.Distinct().ToList();
        }
        private static IEnumerable<int> GetValidPostEvolutionMoves(PKM pkm, int Species, DexLevel[] evoChain, int Generation, GameVersion Version)
        {
            var evomoves = new List<int>();
            var index = Array.FindIndex(evoChain, e => e.Species == Species);
            for (int i = 0; i <= index; i++)
            {
                var evo = evoChain[i];
                var moves = GetMoves(pkm, evo.Species, 1, 1, evo.Level, pkm.AltForm, moveTutor: true, Version: Version, LVL: true, specialTutors: true, Machine: true, MoveReminder: true, RemoveTransferHM: false, Generation: Generation);
                // Moves from Species or any species after in the evolution phase
                evomoves.AddRange(moves);
            }
            return evomoves;
        }
        internal static IEnumerable<int> GetExclusivePreEvolutionMoves(PKM pkm, int Species, DexLevel[] evoChain, int Generation, GameVersion Version)
        {
            var preevomoves = new List<int>();
            var evomoves = new List<int>();
            var index = Array.FindIndex(evoChain, e => e.Species == Species);
            for (int i = 0; i < evoChain.Length; i++)
            {
                var evo = evoChain[i];
                var moves = GetMoves(pkm, evo.Species, 1, 1, evo.Level, pkm.AltForm, moveTutor: true, Version: Version, LVL: true, specialTutors: true, Machine: true, MoveReminder: true, RemoveTransferHM: false, Generation: Generation);
                var list = i >= index ? preevomoves : evomoves;
                list.AddRange(moves);
            }
            return preevomoves.Where(z => !evomoves.Contains(z)).Distinct().ToList();
        }

        // Encounter
        internal static IEnumerable<GameVersion> GetGen2Versions(LegalInfo Info)
        {
            if (AllowGen2Crystal(Info.Korean) && Info.Game == GameVersion.C)
                yield return GameVersion.C;

            // Any encounter marked with version GSC is for pokemon with the same moves in GS and C
            // it is sufficient to check just GS's case
            yield return GameVersion.GS;
        }
        internal static IEnumerable<GameVersion> GetGen1Versions(LegalInfo Info)
        {
            if (Info.EncounterMatch.Species == 133 && Info.Game == GameVersion.Stadium)
            { 
                // Stadium Eevee; check for RB and yellow initial moves
                yield return GameVersion.RB;
                yield return GameVersion.YW;
            }
            else if (Info.Game == GameVersion.YW)
                yield return GameVersion.YW;

            // Any encounter marked with version RBY is for pokemon with the same moves and catch rate in RB and Y, 
            // it is sufficient to check just RB's case
            yield return GameVersion.RB;
        }
        internal static IEnumerable<int> GetInitialMovesGBEncounter(int species, int lvl, GameVersion ver)
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
                case GameVersion.RB:
                    {
                        var LevelTable = ver == GameVersion.YW ? LevelUpY : LevelUpRB;
                        int index = PersonalTable.RB.GetFormeIndex(species, 0);
                        if (index == 0)
                            return new int[0];
                        LevelUpMoves = LevelTable[species].GetEncounterMoves(lvl);
                        diff = 4 - LevelUpMoves.Count(z => z != 0);
                        if (diff == 0)
                            return LevelUpMoves;
                        var table = ver == GameVersion.YW ? PersonalTable.Y : PersonalTable.RB;
                        InitialMoves = ((PersonalInfoG1)table[index]).Moves;
                        break;
                    }
                case GameVersion.C:
                case GameVersion.GD:
                case GameVersion.SV:
                case GameVersion.GS:
                    {
                        if (species == 235)
                            return new[] { 166 }; // Smeargle only learns Sketch, is duplicated in the level up tables
                        var LevelTable = ver == GameVersion.C ? LevelUpC : LevelUpGS;
                        int index = PersonalTable.C.GetFormeIndex(species, 0);
                        if (index == 0)
                            return new int[0];
                        LevelUpMoves = LevelTable[species].GetEncounterMoves(lvl);
                        diff = 4 - LevelUpMoves.Count(z => z != 0);
                        if (diff == 0)
                            return LevelUpMoves;
                        // Level Up 1 moves are initial moves, it can be duplicated in levels 2-100
                        InitialMoves = LevelTable[species].GetEncounterMoves(1);
                        break;
                    }
                default:
                    return new int[0];
            }
            // Initial Moves could be duplicated in the level up table
            // level up table moves have preference
            var moves = InitialMoves.Where(p => p != 0 && !LevelUpMoves.Any(m => m == p)).ToList();
            // If all of the personal table moves can't be included, the last moves have preference.
            int pop = moves.Count - diff;
            if (pop > 0)
                moves.RemoveRange(0, pop);
            // The order for the pokemon default moves are first moves from personal table and then moves from  level up table
            return moves.Union(LevelUpMoves).ToArray();
        }
        internal static int GetRequiredMoveCount(PKM pk, int[] moves, LegalInfo info, int[] initialmoves)
        {
            if (pk.Format != 1 || !pk.Gen1_NotTradeback) // No Move Deleter in Gen 1
                return 1; // Move Deleter exits, slots from 2 onwards can allways be empty

            int required = GetRequiredMoveCount(pk, moves, info.EncounterMoves.LevelUpMoves, initialmoves);
            if (required >= 4)
                return 4;

            // tm, hm and tutor moves replace a free slots if the pokemon have less than 4 moves
            // Ignore tm, hm and tutor moves already in the learnset table
            var learn = info.EncounterMoves.LevelUpMoves;
            var tmhm = info.EncounterMoves.TMHMMoves;
            var tutor = info.EncounterMoves.TutorMoves;
            var union = initialmoves.Union(learn[1]);
            required += moves.Count(m => m != 0 && union.All(t => t != m) && (tmhm[1].Any(t => t == m) || tutor[1].Any(t => t == m)));

            return Math.Min(4, required);
        }
        private static int GetRequiredMoveCount(PKM pk, int[] moves, List<int>[] learn, int[] initialmoves)
        {
            if (SpecialMinMoveSlots.Contains(pk.Species))
                return GetRequiredMoveCountSpecial(pk, moves, learn);

            // A pokemon is captured with initial moves and can't forget any until have all 4 slots used
            // If it has learn a move before having 4 it will be in one of the free slots
            int required = GetRequiredMoveSlotsRegular(pk, moves, learn, initialmoves);
            return required != 0 ? required : GetRequiredMoveCountDecrement(pk, moves, learn, initialmoves);
        }
        private static int GetRequiredMoveSlotsRegular(PKM pk, int[] moves, List<int>[] learn, int[] initialmoves)
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

            return IsMoveCountRequired3(species, pk.CurrentLevel, moves) ? 3 : 0; // no match
        }
        private static bool IsMoveCountRequired3(int species, int level, int[] moves)
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
        private static int GetRequiredMoveCountDecrement(PKM pk, int[] moves, List<int>[] learn, int[] initialmoves)
        {
            int usedslots = initialmoves.Union(learn[1]).Where(m => m != 0).Distinct().Count();
            switch (pk.Species)
            {
                case 031: // Venonat; ignore Venomoth (by the time Venonat evolves it will always have 4 moves)
                    if (pk.CurrentLevel >= 11 && !moves.Contains(48)) // Supersonic
                        usedslots--;
                    if (pk.CurrentLevel >= 19 && !moves.Contains(93)) // Confusion
                        usedslots--;
                    break;
                case 064: case 065: // Abra & Kadabra
                    int catch_rate = ((PK1) pk).Catch_Rate;
                    if (catch_rate != 100)// Initial Yellow Kadabra Kinesis (move 134)
                        usedslots--;
                    if (catch_rate == 200 && pk.CurrentLevel < 20) // Kadabra Disable, not learned until 20 if captured as Abra (move 50)
                        usedslots--;
                    break;
                case 104: case 105: // Cubone & Marowak
                    if (!moves.Contains(39)) // Initial Yellow Tail Whip 
                        usedslots--;
                    if (!moves.Contains(125)) // Initial Yellow Bone Club
                        usedslots--;
                    if (pk.Species == 105 && pk.CurrentLevel < 33 && !moves.Contains(116)) // Marowak evolved without Focus Energy
                        usedslots--;
                    break;
                case 113:
                    if (!moves.Contains(39)) // Yellow Initial Tail Whip 
                        usedslots--;
                    if (!moves.Contains(3)) // Yellow Lvl 12 and Initial Red/Blue Double Slap
                        usedslots--;
                    break;
                case 056 when pk.CurrentLevel >= 9 && !moves.Contains(67): // Mankey (Low Kick)
                case 127 when pk.CurrentLevel >= 21 && !moves.Contains(20): // Pinsir (Bind)
                case 130 when pk.CurrentLevel < 32: // Gyarados
                    usedslots--;
                    break;
            }
            return usedslots;
        }
        private static int GetRequiredMoveCountSpecial(PKM pk, int[] moves, List<int>[] learn)
        {
            // Species with few mandatory slots, species with stone evolutions that could evolve at lower level and do not learn any more moves
            // and Pikachu and Nidoran family, those only have mandatory the initial moves and a few have one level up moves, 
            // every other move could be avoided switching game or evolving
            var mandatory = GetRequiredMoveCountLevel(pk);
            switch (pk.Species)
            {
                case 103 when pk.CurrentLevel >= 28: // Exeggutor
                    // At level 28 learn different move if is a Exeggute or Exeggutor
                    if (moves.Contains(73))
                        mandatory.Add(73); // Leech Seed level 28 Exeggute
                    if (moves.Contains(23))
                        mandatory.Add(23); // Stomp level 28 Exeggutor
                    break;
                case 25 when pk.CurrentLevel >= 33:
                    mandatory.Add(97); // Pikachu always learns Agility
                    break;
                case 114:
                    mandatory.Add(132); // Tangela always has Constrict as Initial Move
                    break;
            }

            // Add to used slots the non-mandatory moves from the learnset table that the pokemon have learned
            return mandatory.Count + moves.Count(m => m != 0 && mandatory.All(l => l != m) && learn[1].Any(t => t == m));
        }
        private static List<int> GetRequiredMoveCountLevel(PKM pk)
        {
            int species = pk.Species;
            int basespecies = GetBaseSpecies(pk);
            int maxlevel = 1;
            int minlevel = 1;

            if (species == 114) // Tangela moves before level 32 are different in RB vs Y
            {
                minlevel = 32;
                maxlevel = pk.CurrentLevel;
            }
            else if (029 <= species && species <= 034 && pk.CurrentLevel >= 8)
            {
                maxlevel = 8; // Always learns a third move at level 8
            }

            return minlevel <= pk.CurrentLevel
                ? GetLvlMoves(basespecies, 0, 1, minlevel, maxlevel, pk.Korean).Where(m => m != 0).Distinct().ToList()
                : new List<int>();
        }

        internal static bool GetWasEgg23(PKM pkm)
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

            if (pkm.Format > 3 && pkm.Met_Level < 5)
                return false;
            if (pkm.Format > 3 && pkm.FatefulEncounter)
                return false;

            return IsEvolutionValid(pkm);
        }

        // Generation Specific Fetching
        internal static IEnumerable<EncounterStatic> GetEncounterStaticTable(PKM pkm, GameVersion gameSource = GameVersion.Any)
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
                    return GetEncounterStaticTableGSC(pkm);

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
                case GameVersion.US: return StaticUS;
                case GameVersion.UM: return StaticUM;

                default: return new EncounterStatic[0];
            }
        }
        internal static IEnumerable<EncounterArea> GetEncounterTable(PKM pkm, GameVersion gameSource = GameVersion.Any)
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
                    return GetEncounterTableGSC(pkm);

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
                case GameVersion.US: return SlotsUS;
                case GameVersion.UM: return SlotsUM;

                default: return new EncounterArea[0];
            }
        }
        private static IEnumerable<EncounterStatic> GetEncounterStaticTableGSC(PKM pkm)
        {
            if (!AllowGen2Crystal(pkm))
                return StaticGS;
            if (pkm.Format != 2)
                return StaticGSC;

            if (pkm.HasOriginalMetLocation)
                return StaticC;
            return StaticGSC;
        }
        private static IEnumerable<EncounterArea> GetEncounterTableGSC(PKM pkm)
        {
            if (!AllowGen2Crystal(pkm))
                return SlotsGS;

            if (pkm.Format != 2)
                // Gen 2 met location is lost outside gen 2 games
                return SlotsGSC;

            if (pkm.HasOriginalMetLocation)
                // Format 2 with met location, encounter should be from Crystal
                return SlotsC;

            if (pkm.Species > 151 && !FutureEvolutionsGen1.Contains(pkm.Species))
                // Format 2 without met location but pokemon could not be tradeback to gen 1, 
                // encounter should be from gold or silver
                return SlotsGS;

            // Encounter could be any gen 2 game, it can have empty met location for have a g/s origin
            // or it can be a Crystal pokemon that lost met location after being tradeback to gen 1 games
            return SlotsGSC;
        }
        internal static IEnumerable<EncounterArea> GetDexNavAreas(PKM pkm)
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

        internal static IEnumerable<int> GetLineage(PKM pkm)
        {
            if (pkm.IsEgg)
                return new[] {pkm.Species};

            var table = EvolutionTree.GetEvolutionTree(pkm.Format);
            var lineage = table.GetValidPreEvolutions(pkm, maxLevel: pkm.CurrentLevel);
            return lineage.Select(evolution => evolution.Species);
        }
        internal static ICollection<int> GetWildBalls(PKM pkm)
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
        internal static int GetEggHatchLevel(PKM pkm) => pkm.Format <= 3 ? 5 : 1;
        internal static ICollection<int> GetSplitBreedGeneration(PKM pkm)
        {
            return GetSplitBreedGeneration(pkm.GenNumber);
        }
        private static ICollection<int> GetSplitBreedGeneration(int generation)
        {
            switch (generation)
            {
                case 1: return Empty;
                case 2: return Empty;
                case 3: return SplitBreed_3;
                case 4: return SplitBreed;
                case 5: return SplitBreed;
                case 6: return SplitBreed;
                case 7: return SplitBreed;
                default: return Empty;
            }
        }
        internal static int GetMaxSpeciesOrigin(PKM pkm)
        {
            if (pkm.Format == 1 || pkm.VC1) // Gen1 VC could not trade with gen 2 yet
                return GetMaxSpeciesOrigin(1);
            if (pkm.Format == 2 || pkm.VC2)
                return GetMaxSpeciesOrigin(2);
            return GetMaxSpeciesOrigin(pkm.GenNumber);
        }
        internal static int GetMaxSpeciesOrigin(int generation)
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
        internal static IEnumerable<int> GetFutureGenEvolutions(int generation)
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

        internal static int GetMaxLanguageID(int generation)
        {
            switch (generation)
            {
                case 1:
                case 3:
                    return 7; // 1-7 except 6
                case 2:
                case 4:
                case 5:
                case 6:
                    return 8;
                case 7:
                    return 10;
            }
            return -1;
        }
        private static bool[] GetReleasedHeldItems(int generation)
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
        internal static bool IsHeldItemAllowed(int item, int generation)
        {
            if (item < 0)
                return false;
            if (item == 0)
                return true;

            var items = GetReleasedHeldItems(generation);
            return items.Length > item && items[item];
        }

        internal static bool IsNotBaseSpecies(PKM pkm)
        {
            if (pkm.IsEgg)
                return false;

            return GetValidPreEvolutions(pkm).Count() > 1;
        }
        private static bool IsEvolvedFormChange(PKM pkm)
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
        internal static bool IsTradeEvolved(PKM pkm)
        {
            if (pkm.IsEgg)
                return false;

            var table = EvolutionTree.GetEvolutionTree(pkm.Format);
            var lineage = table.GetValidPreEvolutions(pkm, maxLevel: 100, skipChecks:true);
            return lineage.Any(evolution => EvolutionMethod.TradeMethods.Any(method => method == evolution.Flag)); // Trade Evolutions
        }
        internal static bool IsEvolutionValid(PKM pkm, int minSpecies = -1)
        {
            var curr = GetValidPreEvolutions(pkm);
            var poss = GetValidPreEvolutions(pkm, lvl: 100, skipChecks: true);

            if (minSpecies != -1)
                poss = poss.Reverse().SkipWhile(z => z.Species != minSpecies); // collection is reversed, we only care about count
            else if (GetSplitBreedGeneration(pkm).Contains(GetBaseSpecies(pkm, 1)))
                return curr.Count() >= poss.Count() - 1;
            return curr.Count() >= poss.Count();
        }
        internal static bool IsEvolutionValidWithMove(PKM pkm, LegalInfo info)
        {
            // Exclude species that do not evolve leveling with a move
            // Exclude gen 1-3 formats
            // Exclude Mr Mime and Snorlax for gen 1-3 games
            if (!SpeciesEvolutionWithMove.Contains(pkm.Species) || pkm.Format <= 3 || (BabyEvolutionWithMove.Contains(pkm.Species) && pkm.GenNumber <= 3))
                return true;

            var index = Array.FindIndex(SpeciesEvolutionWithMove, p => p == pkm.Species);
            var levels = MinLevelEvolutionWithMove[index];
            var moves = MoveEvolutionWithMove[index];
            var allowegg = EggMoveEvolutionWithMove[index][pkm.GenNumber];

            // Get the minimum level in any generation when the pokemon could learn the evolve move
            var LearnLevel = 101;
            for (int g = pkm.GenNumber; g <= pkm.Format; g++)
                if (pkm.InhabitedGeneration(g) && levels[g] > 0)
                    LearnLevel = Math.Min(LearnLevel, levels[g]);

            // Check also if the current encounter include the evolve move as an special move
            // That means the pokemon have the move from the encounter level
            int[] SpecialMoves = (info.EncounterMatch as IMoveset)?.Moves ?? new int[0];
            if (SpecialMoves.Any(m => moves.Contains(m)))
                LearnLevel = Math.Min(LearnLevel, info.EncounterMatch.LevelMin);

            // If the encounter is a player hatched egg check if the move could be an egg move or inherited level up move
            if (info.EncounterMatch.EggEncounter && !pkm.WasGiftEgg && !pkm.WasEventEgg && allowegg)
            {
                if (IsMoveInherited(pkm, info, moves))
                    LearnLevel = Math.Min(LearnLevel, pkm.GenNumber < 4 ? 6 : 2);
            }

            // If has original met location the minimum evolution level is one level after met level
            // Gen 3 pokemon in gen 4 games: minimum level is one level after transfer to generation 4
            // VC pokemon: minimum level is one level after transfer to generation 7
            // Sylveon: always one level after met level, for gen 4 and 5 eevees in gen 6 games minimum for evolution is one level after transfer to generation 5 
            if (pkm.HasOriginalMetLocation || pkm.Format == 4 && pkm.Gen3 || pkm.VC || pkm.Species == 700)
                LearnLevel = Math.Max(pkm.Met_Level + 1, LearnLevel);

            // Current level must be at least one the minimum learn level
            // the level-up event that triggers the learning of the move also triggers evolution with no further level-up required
            return pkm.CurrentLevel >= LearnLevel;
        }
        private static bool IsMoveInherited(PKM pkm, LegalInfo info, int[] moves)
        {
            // In 3DS games, the inherited move must be in the relearn moves.
            if (pkm.GenNumber >= 6)
                return pkm.RelearnMoves.Any(moves.Contains);

            // In Pre-3DS games, the move is inherited if it has the move and it can be hatched with the move.
            if (pkm.Moves.Any(moves.Contains))
                return true;

            // If the pokemon does not have the move, it still could be an egg move that was forgotten.
            // This requires the pokemon to not have 4 other moves identified as egg moves or inherited level up moves.
            return 4 > info.Moves.Count(m => m.Source == MoveSource.EggMove || m.Source == MoveSource.InheritLevelUp);
        }
        internal static bool IsFormChangeable(PKM pkm, int species)
        {
            if (FormChange.Contains(species))
                return true;
            if (IsEvolvedFormChange(pkm))
                return true;
            if (pkm.Species == 718 && pkm.InhabitedGeneration(7) && pkm.AltForm != 1)
            {
                return true;
            }
            return false;
        }
        
        internal static bool GetCanInheritMoves(PKM pkm, IEncounterable e)
        {
            if (FixedGenderFromBiGender.Contains(e.Species)) // Nincada -> Shedinja loses gender causing 'false', edge case
                return true;
            int ratio = pkm.PersonalInfo.Gender;
            if (ratio > 0 && ratio < 255)
                return true;
            if (MixedGenderBreeding.Contains(e.Species))
                return true;
            return false;
        }
        public static int GetLowestLevel(PKM pkm, int startLevel)
        {
            if (startLevel == -1)
                startLevel = 100;

            var table = EvolutionTree.GetEvolutionTree(pkm.Format);
            int count = 1;
            for (int i = 100; i >= startLevel; i--)
            {
                var evos = table.GetValidPreEvolutions(pkm, maxLevel: i, minLevel: startLevel, skipChecks:true).ToArray();
                if (evos.Length < count) // lost an evolution, prior level was minimum current level
                    return evos.Max(evo => evo.Level) + 1;
                count = evos.Length;
            }
            return startLevel;
        }
        internal static bool GetCanBeCaptured(int species, int gen, GameVersion version = GameVersion.Any)
        {
            switch (gen)
            {
                // Capture Memory only obtainable via Gen 6.
                case 6:
                    switch (version)
                    {
                        case GameVersion.Any:
                            return GetCanBeCaptured(species, SlotsX, StaticX, XY:true)
                                || GetCanBeCaptured(species, SlotsY, StaticY, XY:true)
                                || GetCanBeCaptured(species, SlotsA, StaticA)
                                || GetCanBeCaptured(species, SlotsO, StaticO);
                        case GameVersion.X:
                            return GetCanBeCaptured(species, SlotsX, StaticX, XY:true);
                        case GameVersion.Y:
                            return GetCanBeCaptured(species, SlotsY, StaticY, XY:true);
                        case GameVersion.AS:
                            return GetCanBeCaptured(species, SlotsA, StaticA);
                        case GameVersion.OR:
                            return GetCanBeCaptured(species, SlotsO, StaticO);
                    }
                    break;
            }
            return false;
        }
        private static bool GetCanBeCaptured(int species, IEnumerable<EncounterArea> area, IEnumerable<EncounterStatic> statics, bool XY = false)
        {
            if (XY && FriendSafari.Contains(species))
                return true;

            if (area.Any(loc => loc.Slots.Any(slot => slot.Species == species)))
                return true;
            if (statics.Any(enc => enc.Species == species && !enc.Gift))
                return true;
            return false;
        }
        internal static bool GetCanLearnMachineMove(PKM pkm, int move, int generation, GameVersion version = GameVersion.Any)
        {
            return GetValidMoves(pkm, version, GetValidPreEvolutions(pkm).ToArray(), generation, Machine: true).Contains(move);
        }
        internal static bool GetCanRelearnMove(PKM pkm, int move, int generation, GameVersion version = GameVersion.Any)
        {
            return GetValidMoves(pkm, version, GetValidPreEvolutions(pkm).ToArray(), generation, LVL: true, Relearn: true).Contains(move);
        }
        internal static bool GetCanLearnMove(PKM pkm, int move, int generation, GameVersion version = GameVersion.Any)
        {
            return GetValidMoves(pkm, version, GetValidPreEvolutions(pkm).ToArray(), generation, Tutor: true, Machine: true).Contains(move);
        }
        internal static bool GetCanKnowMove(PKM pkm, int move, int generation, GameVersion version = GameVersion.Any)
        {
            if (pkm.Species == 235 && !InvalidSketch.Contains(move))
                return true;
            return GetValidMoves(pkm, version, GetValidPreEvolutions(pkm).ToArray(), generation, LVL: true, Relearn: true, Tutor: true, Machine: true).Contains(move);
        }
        internal static int GetBaseEggSpecies(PKM pkm, int skipOption = 0)
        {
            if (pkm.Format == 1)
                return GetBaseSpecies(pkm, skipOption : skipOption, generation : 2);
            return GetBaseSpecies(pkm, skipOption);
        }
        internal static int GetBaseSpecies(PKM pkm, int skipOption = 0, int generation = -1)
        {
            if (pkm.Species == 292)
                return 290;
            if (pkm.Species == 242 && pkm.CurrentLevel < 3) // Never Cleffa
                return 113;

            int tree = generation != -1 ? generation : pkm.Format;
            var table = EvolutionTree.GetEvolutionTree(tree);
            int maxSpeciesOrigin = generation != -1 ? GetMaxSpeciesOrigin(generation) : - 1;
            var evos = table.GetValidPreEvolutions(pkm, maxLevel: 100, maxSpeciesOrigin: maxSpeciesOrigin, skipChecks:true).ToArray();

            switch (skipOption)
            {
                case -1: return pkm.Species;
                case 1: return evos.Length <= 1 ? pkm.Species : evos[evos.Length - 2].Species;
                default: return evos.Length <= 0 ? pkm.Species : evos.Last().Species;
            }
        }
        private static int GetMaxLevelGeneration(PKM pkm)
        {
            return GetMaxLevelGeneration(pkm, pkm.GenNumber);
        }
        private static int GetMaxLevelGeneration(PKM pkm, int generation)
        {
            if (!pkm.InhabitedGeneration(generation))
                return pkm.Met_Level;

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
        internal static int GetMinLevelEncounter(PKM pkm)
        {
            if (pkm.Format == 3 && pkm.WasEgg)
                // Only for gen 3 pokemon in format 3, after transfer to gen 4 it should return transfer level
                return 5;
            if (pkm.Format == 4 && pkm.GenNumber == 4 && pkm.WasEgg)
                // Only for gen 4 pokemon in format 4, after transfer to gen 5 it should return transfer level
                return 1;
            return pkm.HasOriginalMetLocation ? pkm.Met_Level : GetMaxLevelGeneration(pkm);
        }
        internal static int GetMinLevelGeneration(PKM pkm)
        {
            return GetMinLevelGeneration(pkm, pkm.GenNumber);
        }
        private static int GetMinLevelGeneration(PKM pkm, int generation)
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
        private static bool GetCatchRateMatchesPreEvolution(PKM pkm, int catch_rate)
        {
            // For species catch rate, discard any species that has no valid encounters and a different catch rate than their pre-evolutions
            var Lineage = GetLineage(pkm).Where(s => !Species_NotAvailable_CatchRate.Contains(s)).ToList();
            return IsCatchRateRB(Lineage) || IsCatchRateY(Lineage) || IsCatchRateTrade() || IsCatchRateStadium();

            // Dragonite's Catch Rate is different than Dragonair's in Yellow, but there is no Dragonite encounter.
            bool IsCatchRateRB(List<int> ds) => ds.Any(s => catch_rate == PersonalTable.RB[s].CatchRate);
            bool IsCatchRateY(List<int> ds) => ds.Any(s => s != 149 && catch_rate == PersonalTable.Y[s].CatchRate);
            // Krabby encounter trade special catch rate
            bool IsCatchRateTrade() => (pkm.Species == 098 || pkm.Species == 099) && catch_rate == 204;
            bool IsCatchRateStadium() => Stadium_GiftSpecies.Contains(pkm.Species) && Stadium_CatchRate.Contains(catch_rate);
        }
        internal static void SetTradebackStatusRBY(PKM pkm)
        {
            if (!AllowGen1Tradeback)
            {
                pkm.TradebackStatus = TradebackType.Gen1_NotTradeback;
                ((PK1)pkm).CatchRateIsItem = false;
                return;
            }

            // Detect tradeback status by comparing the catch rate(Gen1)/held item(Gen2) to the species in the pkm's evolution chain.
            var catch_rate = ((PK1)pkm).Catch_Rate;
            bool matchAny = GetCatchRateMatchesPreEvolution(pkm, catch_rate);

            // If the catch rate value has been modified, the item has either been removed or swapped in Generation 2.
            var HeldItemCatchRate = catch_rate == 0 || HeldItems_GSC.Any(h => h == catch_rate);
            if (HeldItemCatchRate && !matchAny)
                pkm.TradebackStatus = TradebackType.WasTradeback;
            else if (!HeldItemCatchRate && matchAny)
                pkm.TradebackStatus = TradebackType.Gen1_NotTradeback;
            else
                pkm.TradebackStatus = TradebackType.Any;

            // Update the editing settings for the PKM to acknowledge the tradeback status if the species is changed.
            ((PK1)pkm).CatchRateIsItem = !pkm.Gen1_NotTradeback && HeldItemCatchRate && !matchAny;
        }

        internal static DexLevel[][] GetEvolutionChainsAllGens(PKM pkm, IEncounterable Encounter)
        {
            var CompleteEvoChain = GetEvolutionChain(pkm, Encounter).ToArray();
            int maxgen = pkm.Format == 1 && !pkm.Gen1_NotTradeback ? 2 : pkm.Format;
            int mingen = (pkm.Format == 2 || pkm.VC2) && !pkm.Gen2_NotTradeback ? 1 : pkm.GenNumber;
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
                if (GetMaxSpeciesOrigin(pkm.GenNumber) >= pkm.Species)
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

                int maxspeciesgen = GetMaxSpeciesOrigin(gen);
                if (gen == 2 && pkm.VC1)
                    maxspeciesgen = MaxSpeciesID_1;

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

                GensEvoChains[gen] = GetEvolutionChain(pkm, Encounter, CompleteEvoChain.First().Species, lvl);
                if (gen > 2 && !pkm.HasOriginalMetLocation && gen >= pkm.GenNumber)
                    //Remove previous evolutions bellow transfer level
                    //For example a gen3 charizar in format 7 with current level 36 and met level 36
                    //chain level for charmander is 35, is bellow met level
                    GensEvoChains[gen] = GensEvoChains[gen].Where(e => e.Level >= GetMinLevelGeneration(pkm, gen)).ToArray();

                if (gen == 1 && GensEvoChains[gen].LastOrDefault()?.Species > MaxSpeciesID_1)
                {
                    // Remove generation 2 pre-evolutions
                    GensEvoChains[gen] = GensEvoChains[gen].Take(GensEvoChains[gen].Length - 1).ToArray();
                    if (pkm.VC1)
                    {
                        // Remove generation 2 pre-evolutions from gen 7 and future generations
                        for ( int fgen = 7; fgen <= maxgen; fgen++)
                            GensEvoChains[fgen] = GensEvoChains[fgen].Take(GensEvoChains[fgen].Length - 1).ToArray();
                    }
                }
            }
            return GensEvoChains;
        }
        private static DexLevel[] GetEvolutionChain(PKM pkm, IEncounterable Encounter)
        {
            return GetEvolutionChain(pkm, Encounter, pkm.Species, 100);
        }

        private static DexLevel[] GetEvolutionChain(PKM pkm, IEncounterable Encounter, int maxspec, int maxlevel)
        {
            DexLevel[] vs = GetValidPreEvolutions(pkm).ToArray();

            // Evolution chain is in reverse order (devolution)
            int minspec = Encounter.Species;

            int minindex = Math.Max(0, Array.FindIndex(vs, p => p.Species == minspec));
            Array.Resize(ref vs, minindex + 1);
            var last = vs.Last();
            if (last.MinLevel > 1) // Last entry from vs is removed, turn next entry into the wild/hatched pokemon
            {
                last.MinLevel = 1;
                last.RequiresLvlUp = false;
                var first = vs.First();
                if (first.MinLevel == 2 && !first.RequiresLvlUp)
                {
                    // Example Raichu in gen 2 or later, 
                    // because Pichu requires level up Minimum level of Raichu would be 2
                    // but after removing Pichu because the origin species is Pikachu, Raichu min level should be 1
                    first.MinLevel = 1;
                    first.RequiresLvlUp = false;
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
        private static IEnumerable<int> GetRelearnLVLMoves(PKM pkm, int species, int lvl, int formnum, GameVersion version = GameVersion.Any)
        {
            if (version == GameVersion.Any)
                version = (GameVersion)pkm.Version;
            // A pkm can only have levelup relearn moves from the game it originated on
            // eg Plusle/Minun have Charm/Fake Tears (respectively) only in OR/AS, not X/Y
            switch (version)
            {
                case GameVersion.X: case GameVersion.Y:
                    return getMoves(LevelUpXY, PersonalTable.XY);
                case GameVersion.AS: case GameVersion.OR:
                    return getMoves(LevelUpAO, PersonalTable.AO);

                case GameVersion.SN: case GameVersion.MN:
                    return getMoves(LevelUpSM, PersonalTable.SM);
                case GameVersion.US: case GameVersion.UM:
                    return getMoves(LevelUpUSUM, PersonalTable.USUM);
            }
            return new int[0];

            int[] getMoves(Learnset[] moves, PersonalTable table) => moves[table.GetFormeIndex(species, formnum)].GetMoves(lvl);
        }
        internal static IEnumerable<DexLevel> GetValidPreEvolutions(PKM pkm, int maxspeciesorigin = -1, int lvl = -1, bool skipChecks = false)
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

            int tree = maxspeciesorigin == MaxSpeciesID_2 ? 2 : pkm.Format;
            var et = EvolutionTree.GetEvolutionTree(tree);
            return et.GetValidPreEvolutions(pkm, maxLevel: lvl, maxSpeciesOrigin: maxspeciesorigin, skipChecks: skipChecks);
        }
        private static IEnumerable<int> GetValidMoves(PKM pkm, GameVersion Version, IReadOnlyList<DexLevel[]> vs, int minLvLG1 = 1, int minLvLG2 = 1, bool LVL = false, bool Relearn = false, bool Tutor = false, bool Machine = false, bool MoveReminder = true, bool RemoveTransferHM = true)
        {
            List<int> r = new List<int> { 0 };
            if (Relearn && pkm.Format >= 6)
                r.AddRange(pkm.RelearnMoves);

            for (int gen = pkm.GenNumber; gen <= pkm.Format; gen++)
                if (vs[gen].Any())
                    r.AddRange(GetValidMoves(pkm, Version, vs[gen], gen, minLvLG1: minLvLG1, minLvLG2: minLvLG2, LVL: LVL, Relearn: false, Tutor: Tutor, Machine: Machine, MoveReminder: MoveReminder, RemoveTransferHM: RemoveTransferHM));

            return r.Distinct().ToArray();
        }
        private static IEnumerable<int> GetValidMoves(PKM pkm, GameVersion Version, DexLevel[] vs, int Generation, int minLvLG1 = 1, int minLvLG2 = 1, bool LVL = false, bool Relearn = false, bool Tutor = false, bool Machine = false, bool MoveReminder = true, bool RemoveTransferHM = true)
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
                    r.AddRange(GetMoves(pkm, species, minLvLG1, minLvLG2, vs.First().Level, i, moveTutor, Version, LVL, Tutor, Machine, MoveReminder, RemoveTransferHM, Generation));
                if (Relearn) r.AddRange(pkm.RelearnMoves);
                return r.Distinct();
            }

            foreach (DexLevel evo in vs)
            {
                var minlvlevo1 = 1;
                var minlvlevo2 = 1;
                if (Generation == 1)
                {
                    // Return moves from minLvLG1 if species if the species encounters
                    // For evolutions return moves using evolution min level as min level
                    minlvlevo1 = minLvLG1;
                    if (evo.MinLevel > 1)
                        minlvlevo1 = Math.Min(pkm.CurrentLevel, evo.MinLevel);
                }
                if (Generation == 2 && !AllowGen2MoveReminder(pkm))
                {
                    minlvlevo2 = minLvLG2;
                    if (evo.MinLevel > 1)
                        minlvlevo2 = Math.Min(pkm.CurrentLevel, evo.MinLevel);
                }
                r.AddRange(GetMoves(pkm, evo.Species, minlvlevo1, minlvlevo2, evo.Level, pkm.AltForm, moveTutor, Version, LVL, Tutor, Machine, MoveReminder, RemoveTransferHM, Generation));
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
                {
                    int index = pkm.AltForm - 1;
                    if (index >= 0 && index < CosplayPikachuMoves.Length)
                        r.Add(CosplayPikachuMoves[index]);
                }

                if ((species == 25 || species == 26) && Generation == 7) // Pikachu/Raichu Tutor
                    r.Add(344); // Volt Tackle
            }
            if (Relearn && Generation >= 6)
                r.AddRange(pkm.RelearnMoves);
            return r.Distinct();
        }
        private static IEnumerable<int> GetMoves(PKM pkm, int species, int minlvlG1, int minlvlG2, int lvl, int form, bool moveTutor, GameVersion Version, bool LVL, bool specialTutors, bool Machine, bool MoveReminder, bool RemoveTransferHM, int Generation)
        {
            List<int> r = new List<int>();

            var ver = Version;
            switch (Generation)
            {
                case 1:
                    {
                        int index = PersonalTable.RB.GetFormeIndex(species, 0);
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
                            r.AddRange(LevelUpRB[index].GetMoves(lvl, minlvlG1));
                            r.AddRange(LevelUpY[index].GetMoves(lvl, minlvlG1));
                        }
                        if (Machine)
                        {
                            r.AddRange(TMHM_RBY.Where((t, m) => pi_rb.TMHM[m]));
                            r.AddRange(TMHM_RBY.Where((t, m) => pi_y.TMHM[m]));
                        }
                        if (moveTutor)
                            r.AddRange(GetTutorMoves(pkm, species, form, specialTutors, Generation));
                        break;
                    }
                case 2:
                    {
                        int index = PersonalTable.C.GetFormeIndex(species, 0);
                        if (index == 0)
                            return r;
                        if (LVL)
                        {
                            r.AddRange(LevelUpGS[index].GetMoves(lvl, minlvlG2));
                            if (AllowGen2Crystal(pkm))
                                r.AddRange(LevelUpC[index].GetMoves(lvl, minlvlG2));
                        }
                        if (Machine)
                        {
                            var pi_c = (PersonalInfoG2)PersonalTable.C[index];
                            r.AddRange(TMHM_GSC.Where((t, m) => pi_c.TMHM[m]));
                        }
                        if (moveTutor)
                            r.AddRange(GetTutorMoves(pkm, species, form, specialTutors, Generation));
                        if (pkm.Format == 1) //tradeback gen 2 -> gen 1
                            r = r.Where(m => m <= MaxMoveID_1).ToList();
                        break;
                    }
                case 3:
                    {
                        int index = PersonalTable.E.GetFormeIndex(species, 0);
                        if (index == 0)
                            return r;
                        if (LVL)
                        {
                            if (index == 386)
                            {
                                switch(form)
                                {
                                    case 0: r.AddRange(LevelUpRS[index].GetMoves(lvl)); break;
                                    case 1: r.AddRange(LevelUpFR[index].GetMoves(lvl)); break;
                                    case 2: r.AddRange(LevelUpLG[index].GetMoves(lvl)); break;
                                    case 3: r.AddRange(LevelUpE[index].GetMoves(lvl)); break;
                                }
                            }
                            else
                            {
                                // Emerald level up table are equals to R/S level up tables
                                r.AddRange(LevelUpE[index].GetMoves(lvl));
                                // fire red and leaf green are equals between each other but different than RSE
                                // Do not use FR Levelup table. It have 67 moves for charmander but Leaf Green moves table is correct
                                r.AddRange(LevelUpLG[index].GetMoves(lvl));
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
                            r.AddRange(GetTutorMoves(pkm, species, form, specialTutors, Generation));
                        if (RemoveTransferHM && pkm.Format > 3) //Remove HM
                            r = r.Except(HM_3).ToList();
                        break;
                    }
                case 4:
                    {
                        int index = PersonalTable.HGSS.GetFormeIndex(species, form);
                        if (index == 0)
                            return r;
                        if (LVL)
                        {
                            if (index < LevelUpDP.Length)
                                r.AddRange(LevelUpDP[index].GetMoves(lvl));
                            r.AddRange(LevelUpPt[index].GetMoves(lvl));
                            r.AddRange(LevelUpHGSS[index].GetMoves(lvl));
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
                            r.AddRange(GetTutorMoves(pkm, species, form, specialTutors, Generation));
                        if (RemoveTransferHM && pkm.Format > 4) //Remove HM
                            r = r.Except(HM_4_RemovePokeTransfer).ToList();
                        break;
                    }
                case 5:
                    {
                        int index1 = PersonalTable.BW.GetFormeIndex(species, form);
                        int index2 = PersonalTable.B2W2.GetFormeIndex(species, form);
                        if (index1 == 0 && index2 == 0)
                            return r;
                        if (LVL)
                        {
                            if (index1 != 0)
                                r.AddRange(LevelUpBW[index1].GetMoves(lvl));
                            if (index2 != 0)
                                r.AddRange(LevelUpB2W2[index2].GetMoves(lvl));
                        }
                        if (Machine)
                        {
                            var pi_c = PersonalTable.B2W2[index2];
                            r.AddRange(TMHM_BW.Where((t, m) => pi_c.TMHM[m]));
                        }
                        if (moveTutor)
                            r.AddRange(GetTutorMoves(pkm, species, form, specialTutors, Generation));
                        break;
                    }
                case 6:
                    switch (ver)
                    {
                        case GameVersion.Any: // Start at the top, hit every table
                        case GameVersion.X: case GameVersion.Y: case GameVersion.XY:
                        {
                            int index = PersonalTable.XY.GetFormeIndex(species, form);
                            if (index == 0)
                                return r;

                            if (LVL)
                                r.AddRange(LevelUpXY[index].GetMoves(lvl));
                            if (moveTutor)
                                 r.AddRange(GetTutorMoves(pkm, species, form, specialTutors, Generation));
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
                            int index = PersonalTable.AO.GetFormeIndex(species, form);
                            if (index == 0)
                                return r;

                            if (LVL)
                                r.AddRange(LevelUpAO[index].GetMoves(lvl));
                            if (moveTutor)
                                r.AddRange(GetTutorMoves(pkm, species, form, specialTutors, Generation));
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
                            int index = PersonalTable.SM.GetFormeIndex(species, form);
                            if (MoveReminder)
                                lvl = 100; // Move reminder can teach any level in movepool now!

                            if (LVL)
                                r.AddRange(LevelUpSM[index].GetMoves(lvl));
                            if (moveTutor) 
                                r.AddRange(GetTutorMoves(pkm, species, form, specialTutors, Generation));
                            if (Machine)
                            {
                                PersonalInfo pi = PersonalTable.SM.GetFormeEntry(species, form);
                                r.AddRange(TMHM_SM.Where((t, m) => pi.TMHM[m]));
                            }
                            if (ver == GameVersion.Any) // Fall Through
                                goto case GameVersion.USUM;
                            break;
                        }
                        case GameVersion.US: case GameVersion.UM: case GameVersion.USUM:
                        {
                            int index = PersonalTable.USUM.GetFormeIndex(species, form);
                            if (MoveReminder)
                                lvl = 100; // Move reminder can teach any level in movepool now!

                            if (LVL)
                                r.AddRange(LevelUpUSUM[index].GetMoves(lvl));
                            if (moveTutor)
                                r.AddRange(GetTutorMoves(pkm, species, form, specialTutors, Generation));
                            if (Machine)
                            {
                                PersonalInfo pi = PersonalTable.USUM.GetFormeEntry(species, form);
                                r.AddRange(TMHM_SM.Where((t, m) => pi.TMHM[m]));
                            }
                            break;
                        }
                    }
                    break;
            }
            return r;
        }

        internal static int[] GetEggMoves(PKM pkm, int species, int formnum)
        {
            if (!pkm.InhabitedGeneration(pkm.GenNumber, species) || pkm.PersonalInfo.Gender == 255 && !FixedGenderFromBiGender.Contains(species))
                return new int[0];

            switch (pkm.GenNumber)
            {
                case 1:
                case 2:
                    if (!AllowGen2Crystal(pkm))
                        return EggMovesGS[species].Moves;
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
                    switch (pkm.Version)
                    {
                        case (int)GameVersion.HG:
                        case (int)GameVersion.SS:
                            return EggMovesHGSS[species].Moves;
                        default:
                            return EggMovesDPPt[species].Moves;
                    }
                case 5:
                    return EggMovesBW[species].Moves;
                case 6: // entries per species
                    switch (pkm.Version)
                    {
                        case (int)GameVersion.OR:
                        case (int)GameVersion.AS:
                            return EggMovesAO[species].Moves;
                        default:
                            return EggMovesXY[species].Moves;
                    }

                case 7: // entries per form if required
                    EggMoves[] table;
                    switch (pkm.Version)
                    {
                        case (int)GameVersion.US:
                        case (int)GameVersion.UM:
                            table = EggMovesUSUM;
                            break;
                        default:
                            table = EggMovesSM;
                            break;
                    }

                    var entry = table[species];
                    if (formnum > 0 && AlolanOriginForms.Contains(species))
                        entry = table[entry.FormTableIndex + formnum - 1];
                    return entry.Moves;

                default:
                    return new int[0];
            }
        }
        internal static IEnumerable<int> GetTMHM(PKM pkm, int species, int form, int generation, GameVersion Version = GameVersion.Any, bool RemoveTransferHM = true)
        {
            List<int> moves = new List<int>();
            int index;
            switch (generation)
            {
                case 1:
                    index = PersonalTable.RB.GetFormeIndex(species, 0);
                    if (index == 0)
                        return moves;
                    var pi_rb = (PersonalInfoG1)PersonalTable.RB[index];
                    var pi_y = (PersonalInfoG1)PersonalTable.Y[index];
                    moves.AddRange(TMHM_RBY.Where((t, m) => pi_rb.TMHM[m]));
                    moves.AddRange(TMHM_RBY.Where((t, m) => pi_y.TMHM[m]));
                    break;
                case 2:
                    index = PersonalTable.C.GetFormeIndex(species, 0);
                    if (index == 0)
                        return moves;
                    var pi_c = (PersonalInfoG2)PersonalTable.C[index];
                    moves.AddRange(TMHM_GSC.Where((t, m) => pi_c.TMHM[m]));
                    if (Version == GameVersion.Any)
                        goto case 1; // rby
                    break;
                case 3:
                    index = PersonalTable.E.GetFormeIndex(species, 0);
                    var pi_e = PersonalTable.E[index];
                    moves.AddRange(TM_3.Where((t, m) => pi_e.TMHM[m]));
                    if (!RemoveTransferHM || pkm.Format == 3) // HM moves must be removed for 3->4, only give if current format.
                        moves.AddRange(HM_3.Where((t, m) => pi_e.TMHM[m + 50]));
                    break;
                case 4:
                    index = PersonalTable.HGSS.GetFormeIndex(species, 0);
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
                    index = PersonalTable.B2W2.GetFormeIndex(species, 0);
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
                                index = PersonalTable.XY.GetFormeIndex(species, form);
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
                                index = PersonalTable.AO.GetFormeIndex(species, form);
                                if (index == 0)
                                    return moves;

                                PersonalInfo pi_ao = PersonalTable.AO[index];
                                moves.AddRange(TMHM_AO.Where((t, m) => pi_ao.TMHM[m]));
                                break;
                            }
                    }
                    break;
                case 7:
                    index = PersonalTable.USUM.GetFormeIndex(species, form);
                    if (index == 0)
                        return moves;

                    PersonalInfo pi_sm = PersonalTable.USUM[index];
                    moves.AddRange(TMHM_SM.Where((t, m) => pi_sm.TMHM[m]));
                    break;
            }
            return moves.Distinct();
        }
        internal static IEnumerable<int> GetTutorMoves(PKM pkm, int species, int form, bool specialTutors, int generation)
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
                    info = PersonalTable.HGSS.GetFormeEntry(species, form);
                    moves.AddRange(Tutors_4.Where((t, i) => info.TypeTutors[i]));
                    moves.AddRange(SpecialTutors_4.Where((t, i) => SpecialTutors_Compatibility_4[i].Any(e => e == species)));
                    break;
                case 5:
                    info = PersonalTable.B2W2[species];
                    moves.AddRange(TypeTutor6.Where((t, i) => info.TypeTutors[i]));
                    if (pkm.InhabitedGeneration(5) && specialTutors)
                    {
                        PersonalInfo pi = PersonalTable.B2W2.GetFormeEntry(species, form);
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
                        PersonalInfo pi = PersonalTable.AO.GetFormeEntry(species, form);
                        for (int i = 0; i < Tutors_AO.Length; i++)
                            for (int b = 0; b < Tutors_AO[i].Length; b++)
                                if (pi.SpecialTutors[i][b])
                                    moves.Add(Tutors_AO[i][b]);
                    }
                    break;
                case 7:
                    info = PersonalTable.USUM.GetFormeEntry(species, form);
                    moves.AddRange(TypeTutor6.Where((t, i) => info.TypeTutors[i]));
                    // No special tutors in G7
                    break;
            }
            return moves.Distinct();
        }
        internal static bool IsTradedKadabraG1(PKM pkm)
        {
            if (!(pkm is PK1 pk1) || pk1.Species != 64)
                return false;
            if (pk1.TradebackStatus == TradebackType.WasTradeback)
                return true;
            var IsYellow = Savegame_Version == GameVersion.Y;
            if (pk1.TradebackStatus == TradebackType.Gen1_NotTradeback)
            {
                // If catch rate is Abra catch rate it wont trigger as invalid trade without evolution, it could be traded as Abra
                var catch_rate = pk1.Catch_Rate;
                // Yellow Kadabra catch rate in Red/Blue game, must be Alakazam
                if (!IsYellow && catch_rate == PersonalTable.Y[64].CatchRate)
                    return true;
                // Red/Blue Kadabra catch rate in Yellow game, must be Alakazam
                if (IsYellow && catch_rate == PersonalTable.RB[64].CatchRate)
                    return true;
            }
            if (IsYellow)
                return false;
            // Yellow only moves in Red/Blue game, must be Allakazham
            if (pk1.Moves.Contains(134)) // Kinesis, yellow only move 
                return true;
            if (pk1.CurrentLevel < 20 && pkm.Moves.Contains(50)) // Obtaining Disable below level 20 implies a yellow only move
                return true;

            return false;
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

        internal static TreesArea GetCrystalTreeArea(EncounterSlot Slot)
        {
            return HeadbuttTreesC.FirstOrDefault(a => a.Location == Slot.Location);
        }
    }
}
