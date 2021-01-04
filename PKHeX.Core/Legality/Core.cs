using System;
using System.Collections.Generic;
using System.Linq;

namespace PKHeX.Core
{
    public static partial class Legal
    {
        // Gen 1
        internal static readonly Learnset[] LevelUpRB = LearnsetReader.GetArray(Util.GetBinaryResource("lvlmove_rb.pkl"), MaxSpeciesID_1);
        internal static readonly Learnset[] LevelUpY = LearnsetReader.GetArray(Util.GetBinaryResource("lvlmove_y.pkl"), MaxSpeciesID_1);

        // Gen 2
        internal static readonly EggMoves[] EggMovesGS = EggMoves2.GetArray(Util.GetBinaryResource("eggmove_gs.pkl"), MaxSpeciesID_2);
        internal static readonly Learnset[] LevelUpGS = LearnsetReader.GetArray(Util.GetBinaryResource("lvlmove_gs.pkl"), MaxSpeciesID_2);
        internal static readonly EggMoves[] EggMovesC = EggMoves2.GetArray(Util.GetBinaryResource("eggmove_c.pkl"), MaxSpeciesID_2);
        internal static readonly Learnset[] LevelUpC = LearnsetReader.GetArray(Util.GetBinaryResource("lvlmove_c.pkl"), MaxSpeciesID_2);

        // Gen 3
        internal static readonly Learnset[] LevelUpE = LearnsetReader.GetArray(BinLinker.Unpack(Util.GetBinaryResource("lvlmove_e.pkl"), "em"));
        internal static readonly Learnset[] LevelUpRS = LearnsetReader.GetArray(BinLinker.Unpack(Util.GetBinaryResource("lvlmove_rs.pkl"), "rs"));
        internal static readonly Learnset[] LevelUpFR = LearnsetReader.GetArray(BinLinker.Unpack(Util.GetBinaryResource("lvlmove_fr.pkl"), "fr"));
        internal static readonly Learnset[] LevelUpLG = LearnsetReader.GetArray(BinLinker.Unpack(Util.GetBinaryResource("lvlmove_lg.pkl"), "lg"));
        internal static readonly EggMoves6[] EggMovesRS = EggMoves6.GetArray(BinLinker.Unpack(Util.GetBinaryResource("eggmove_rs.pkl"), "rs"));

        // Gen 4
        internal static readonly Learnset[] LevelUpDP = LearnsetReader.GetArray(BinLinker.Unpack(Util.GetBinaryResource("lvlmove_dp.pkl"), "dp"));
        internal static readonly Learnset[] LevelUpPt = LearnsetReader.GetArray(BinLinker.Unpack(Util.GetBinaryResource("lvlmove_pt.pkl"), "pt"));
        internal static readonly Learnset[] LevelUpHGSS = LearnsetReader.GetArray(BinLinker.Unpack(Util.GetBinaryResource("lvlmove_hgss.pkl"), "hs"));
        internal static readonly EggMoves6[] EggMovesDPPt = EggMoves6.GetArray(BinLinker.Unpack(Util.GetBinaryResource("eggmove_dppt.pkl"), "dp"));
        internal static readonly EggMoves6[] EggMovesHGSS = EggMoves6.GetArray(BinLinker.Unpack(Util.GetBinaryResource("eggmove_hgss.pkl"), "hs"));

        // Gen 5
        internal static readonly Learnset[] LevelUpBW = LearnsetReader.GetArray(BinLinker.Unpack(Util.GetBinaryResource("lvlmove_bw.pkl"), "51"));
        internal static readonly Learnset[] LevelUpB2W2 = LearnsetReader.GetArray(BinLinker.Unpack(Util.GetBinaryResource("lvlmove_b2w2.pkl"), "52"));
        internal static readonly EggMoves6[] EggMovesBW = EggMoves6.GetArray(BinLinker.Unpack(Util.GetBinaryResource("eggmove_bw.pkl"), "bw"));

        // Gen 6
        internal static readonly EggMoves6[] EggMovesXY = EggMoves6.GetArray(BinLinker.Unpack(Util.GetBinaryResource("eggmove_xy.pkl"), "xy"));
        internal static readonly Learnset[] LevelUpXY = LearnsetReader.GetArray(BinLinker.Unpack(Util.GetBinaryResource("lvlmove_xy.pkl"), "xy"));
        internal static readonly EggMoves6[] EggMovesAO = EggMoves6.GetArray(BinLinker.Unpack(Util.GetBinaryResource("eggmove_ao.pkl"), "ao"));
        internal static readonly Learnset[] LevelUpAO = LearnsetReader.GetArray(BinLinker.Unpack(Util.GetBinaryResource("lvlmove_ao.pkl"), "ao"));

        // Gen 7
        internal static readonly EggMoves7[] EggMovesSM = EggMoves7.GetArray(BinLinker.Unpack(Util.GetBinaryResource("eggmove_sm.pkl"), "sm"));
        internal static readonly Learnset[] LevelUpSM = LearnsetReader.GetArray(BinLinker.Unpack(Util.GetBinaryResource("lvlmove_sm.pkl"), "sm"));
        internal static readonly EggMoves7[] EggMovesUSUM = EggMoves7.GetArray(BinLinker.Unpack(Util.GetBinaryResource("eggmove_uu.pkl"), "uu"));
        internal static readonly Learnset[] LevelUpUSUM = LearnsetReader.GetArray(BinLinker.Unpack(Util.GetBinaryResource("lvlmove_uu.pkl"), "uu"));
        internal static readonly Learnset[] LevelUpGG = LearnsetReader.GetArray(BinLinker.Unpack(Util.GetBinaryResource("lvlmove_gg.pkl"), "gg"));

        // Gen 8
        internal static readonly EggMoves7[] EggMovesSWSH = EggMoves7.GetArray(BinLinker.Unpack(Util.GetBinaryResource("eggmove_swsh.pkl"), "ss"));
        internal static readonly Learnset[] LevelUpSWSH = LearnsetReader.GetArray(BinLinker.Unpack(Util.GetBinaryResource("lvlmove_swsh.pkl"), "ss"));

        public static IReadOnlyList<byte> GetPPTable(PKM pkm, int format)
        {
            if (format != 7)
                return GetPPTable(format);
            var lgpe = pkm.Version is (int) GameVersion.GO or (int) GameVersion.GP or (int) GameVersion.GE;
            return lgpe ? MovePP_GG : MovePP_SM;
        }

        public static IReadOnlyList<byte> GetPPTable(int format) => format switch
        {
            1 => MovePP_RBY,
            2 => MovePP_GSC,
            3 => MovePP_RS,
            4 => MovePP_DP,
            5 => MovePP_BW,
            6 => MovePP_XY,
            7 => MovePP_SM,
            8 => MovePP_SWSH,
            _ => Array.Empty<byte>()
        };

        internal static int GetMaxSpeciesOrigin(PKM pkm)
        {
            if (pkm.Format == 1)
                return GetMaxSpeciesOrigin(1);
            if (pkm.Format == 2 || pkm.VC)
                return GetMaxSpeciesOrigin(2);
            return GetMaxSpeciesOrigin(pkm.Generation);
        }

        internal static int GetMaxSpeciesOrigin(int generation) => generation switch
        {
            1 => MaxSpeciesID_1,
            2 => MaxSpeciesID_2,
            3 => MaxSpeciesID_3,
            4 => MaxSpeciesID_4,
            5 => MaxSpeciesID_5,
            6 => MaxSpeciesID_6,
            7 => MaxSpeciesID_7b,
            8 => MaxSpeciesID_8,
            _ => -1
        };

        internal static int GetDebutGeneration(int species) => species switch
        {
            <= MaxSpeciesID_1 => 1,
            <= MaxSpeciesID_2 => 2,
            <= MaxSpeciesID_3 => 3,
            <= MaxSpeciesID_4 => 4,
            <= MaxSpeciesID_5 => 5,
            <= MaxSpeciesID_6 => 6,
            <= MaxSpeciesID_7b => 7,
            <= MaxSpeciesID_8 => 8,
            _ => -1
        };

        internal static int GetMaxLanguageID(int generation) => generation switch
        {
            1 => (int) LanguageID.Spanish, // 1-7 except 6
            3 => (int) LanguageID.Spanish, // 1-7 except 6
            2 => (int) LanguageID.Korean,
            4 => (int) LanguageID.Korean,
            5 => (int) LanguageID.Korean,
            6 => (int) LanguageID.Korean,
            7 => (int) LanguageID.ChineseT,
            8 => (int) LanguageID.ChineseT,
            _ => -1
        };

        internal static bool GetCanLearnMachineMove(PKM pkm, int move, int generation, GameVersion version = GameVersion.Any)
        {
            var evos = EvolutionChain.GetValidPreEvolutions(pkm);
            return MoveList.GetValidMoves(pkm, version, evos, generation, types: MoveSourceType.AllMachines).Contains(move);
        }

        internal static bool GetCanRelearnMove(PKM pkm, int move, int generation, IReadOnlyList<EvoCriteria> evos, GameVersion version = GameVersion.Any)
        {
            return MoveList.GetValidMoves(pkm, version, evos, generation, types: MoveSourceType.Reminder).Contains(move);
        }

        internal static bool GetCanKnowMove(PKM pkm, int move, int generation, IReadOnlyList<IReadOnlyList<EvoCriteria>> evos, GameVersion version = GameVersion.Any)
        {
            if (pkm.Species == (int)Species.Smeargle)
                return !InvalidSketch.Contains(move);

            if (generation >= 8 && MoveEgg.GetIsSharedEggMove(pkm, generation, move))
                return true;

            if (evos.Count <= generation)
                return false;
            for (int i = 1; i <= generation; i++)
            {
                var moves = MoveList.GetValidMoves(pkm, version, evos[i], i, types: MoveSourceType.All);
                if (moves.Contains(move))
                    return true;
            }
            return false;
        }

        internal static bool IsCatchRateHeldItem(int rate) => ParseSettings.AllowGen1Tradeback && HeldItems_GSC.Contains((ushort) rate);

        internal const GameVersion NONE = GameVersion.Invalid;
        internal static readonly LearnVersion LearnNONE = new(-1);

        internal static bool HasVisitedB2W2(this PKM pkm, int species) => pkm.InhabitedGeneration(5, species);
        internal static bool HasVisitedORAS(this PKM pkm, int species) => pkm.InhabitedGeneration(6, species) && (pkm.AO || !pkm.IsUntraded);
        internal static bool HasVisitedUSUM(this PKM pkm, int species) => pkm.InhabitedGeneration(7, species) && (pkm.USUM || !pkm.IsUntraded);
        internal static bool IsMovesetRestricted(this PKM pkm, int gen) => (gen == 7 && pkm.Version is (int)GameVersion.GO or (int)GameVersion.GP or (int)GameVersion.GE) || pkm.IsUntraded;

        public static bool IsValidMissingLanguage(PKM pkm)
        {
            return pkm.Format == 5 && pkm.BW;
        }

        public static int GetMaxLengthOT(int generation, LanguageID language) => language switch
        {
            LanguageID.ChineseS or LanguageID.ChineseT => 6,
            LanguageID.Japanese or LanguageID.Korean => generation >= 6 ? 6 : 5,
            _ => generation >= 6 ? 12 : 7
        };

        public static int GetMaxLengthNickname(int generation, LanguageID language) => language switch
        {
            LanguageID.ChineseS or LanguageID.ChineseT => 6,
            LanguageID.Japanese or LanguageID.Korean => generation >= 6 ? 6 : 5,
            _ => generation >= 6 ? 12 : 10
        };

        public static bool GetIsFixedIVSequenceValidSkipRand(IReadOnlyList<int> IVs, PKM pkm, int max = 31)
        {
            for (int i = 0; i < 6; i++)
            {
                if ((uint) IVs[i] > max) // random
                    continue;
                if (IVs[i] != pkm.GetIV(i))
                    return false;
            }
            return true;
        }

        public static bool GetIsFixedIVSequenceValidNoRand(IReadOnlyList<int> IVs, PKM pkm)
        {
            for (int i = 0; i < 6; i++)
            {
                if (IVs[i] != pkm.GetIV(i))
                    return false;
            }
            return true;
        }
    }
}
