using System;
using System.Collections.Generic;
using static PKHeX.Core.BinLinkerAccessor;

namespace PKHeX.Core
{
    public static partial class Legal
    {
        // Gen 1
        internal static readonly Learnset[] LevelUpRB = LearnsetReader.GetArray(Util.GetBinaryResource("lvlmove_rb.pkl"), MaxSpeciesID_1);
        internal static readonly Learnset[] LevelUpY = LearnsetReader.GetArray(Util.GetBinaryResource("lvlmove_y.pkl"), MaxSpeciesID_1);

        // Gen 2
        internal static readonly EggMoves2[] EggMovesGS = EggMoves2.GetArray(Util.GetBinaryResource("eggmove_gs.pkl"), MaxSpeciesID_2);
        internal static readonly Learnset[] LevelUpGS = LearnsetReader.GetArray(Util.GetBinaryResource("lvlmove_gs.pkl"), MaxSpeciesID_2);
        internal static readonly EggMoves2[] EggMovesC = EggMoves2.GetArray(Util.GetBinaryResource("eggmove_c.pkl"), MaxSpeciesID_2);
        internal static readonly Learnset[] LevelUpC = LearnsetReader.GetArray(Util.GetBinaryResource("lvlmove_c.pkl"), MaxSpeciesID_2);

        // Gen 3
        internal static readonly Learnset[] LevelUpE = LearnsetReader.GetArray(Get(Util.GetBinaryResource("lvlmove_e.pkl"), "em"));
        internal static readonly Learnset[] LevelUpRS = LearnsetReader.GetArray(Get(Util.GetBinaryResource("lvlmove_rs.pkl"), "rs"));
        internal static readonly Learnset[] LevelUpFR = LearnsetReader.GetArray(Get(Util.GetBinaryResource("lvlmove_fr.pkl"), "fr"));
        internal static readonly Learnset[] LevelUpLG = LearnsetReader.GetArray(Get(Util.GetBinaryResource("lvlmove_lg.pkl"), "lg"));
        internal static readonly EggMoves6[] EggMovesRS = EggMoves6.GetArray(Get(Util.GetBinaryResource("eggmove_rs.pkl"), "rs"));

        // Gen 4
        internal static readonly Learnset[] LevelUpDP = LearnsetReader.GetArray(Get(Util.GetBinaryResource("lvlmove_dp.pkl"), "dp"));
        internal static readonly Learnset[] LevelUpPt = LearnsetReader.GetArray(Get(Util.GetBinaryResource("lvlmove_pt.pkl"), "pt"));
        internal static readonly Learnset[] LevelUpHGSS = LearnsetReader.GetArray(Get(Util.GetBinaryResource("lvlmove_hgss.pkl"), "hs"));
        internal static readonly EggMoves6[] EggMovesDPPt = EggMoves6.GetArray(Get(Util.GetBinaryResource("eggmove_dppt.pkl"), "dp"));
        internal static readonly EggMoves6[] EggMovesHGSS = EggMoves6.GetArray(Get(Util.GetBinaryResource("eggmove_hgss.pkl"), "hs"));

        // Gen 5
        internal static readonly Learnset[] LevelUpBW = LearnsetReader.GetArray(Get(Util.GetBinaryResource("lvlmove_bw.pkl"), "51"));
        internal static readonly Learnset[] LevelUpB2W2 = LearnsetReader.GetArray(Get(Util.GetBinaryResource("lvlmove_b2w2.pkl"), "52"));
        internal static readonly EggMoves6[] EggMovesBW = EggMoves6.GetArray(Get(Util.GetBinaryResource("eggmove_bw.pkl"), "bw"));

        // Gen 6
        internal static readonly EggMoves6[] EggMovesXY = EggMoves6.GetArray(Get(Util.GetBinaryResource("eggmove_xy.pkl"), "xy"));
        internal static readonly Learnset[] LevelUpXY = LearnsetReader.GetArray(Get(Util.GetBinaryResource("lvlmove_xy.pkl"), "xy"));
        internal static readonly EggMoves6[] EggMovesAO = EggMoves6.GetArray(Get(Util.GetBinaryResource("eggmove_ao.pkl"), "ao"));
        internal static readonly Learnset[] LevelUpAO = LearnsetReader.GetArray(Get(Util.GetBinaryResource("lvlmove_ao.pkl"), "ao"));

        // Gen 7
        internal static readonly EggMoves7[] EggMovesSM = EggMoves7.GetArray(Get(Util.GetBinaryResource("eggmove_sm.pkl"), "sm"));
        internal static readonly Learnset[] LevelUpSM = LearnsetReader.GetArray(Get(Util.GetBinaryResource("lvlmove_sm.pkl"), "sm"));
        internal static readonly EggMoves7[] EggMovesUSUM = EggMoves7.GetArray(Get(Util.GetBinaryResource("eggmove_uu.pkl"), "uu"));
        internal static readonly Learnset[] LevelUpUSUM = LearnsetReader.GetArray(Get(Util.GetBinaryResource("lvlmove_uu.pkl"), "uu"));
        internal static readonly Learnset[] LevelUpGG = LearnsetReader.GetArray(Get(Util.GetBinaryResource("lvlmove_gg.pkl"), "gg"));

        // Gen 8
        internal static readonly EggMoves7[] EggMovesSWSH = EggMoves7.GetArray(Get(Util.GetBinaryResource("eggmove_swsh.pkl"), "ss"));
        internal static readonly Learnset[] LevelUpSWSH = LearnsetReader.GetArray(Get(Util.GetBinaryResource("lvlmove_swsh.pkl"), "ss"));
        internal static readonly EggMoves6[] EggMovesBDSP = EggMoves6.GetArray(Get(Util.GetBinaryResource("eggmove_bdsp.pkl"), "bs"));
        internal static readonly Learnset[] LevelUpBDSP = LearnsetReader.GetArray(Get(Util.GetBinaryResource("lvlmove_bdsp.pkl"), "bs"));
        internal static readonly Learnset[] LevelUpLA = LearnsetReader.GetArray(Get(Util.GetBinaryResource("lvlmove_la.pkl"), "la"));
        internal static readonly Learnset[] MasteryLA = LearnsetReader.GetArray(Get(Util.GetBinaryResource("mastery_la.pkl"), "la"));

        public static IReadOnlyList<byte> GetPPTable(PKM pkm, int format) => format switch
        {
            7 when pkm is PB7 => MovePP_GG,
            8 when pkm is PA8 => MovePP_LA,
            _ => GetPPTable(format),
        };

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
            _ => Array.Empty<byte>(),
        };

        public static ICollection<int> GetDummiedMovesHashSet(PKM pkm) => pkm switch
        {
            PK8 => DummiedMoves_SWSH,
            PB8 => DummiedMoves_BDSP,
            PA8 => DummiedMoves_LA,
            _ => Array.Empty<int>(),
        };

        internal static int GetMaxSpeciesOrigin(PKM pkm)
        {
            if (pkm.Format == 1)
                return GetMaxSpeciesOrigin(1);
            if (pkm.Format == 2 || pkm.VC)
                return GetMaxSpeciesOrigin(2);
            return GetMaxSpeciesOrigin(pkm.Generation);
        }

        internal static int GetMaxSpeciesOrigin(int generation, GameVersion version) => generation switch
        {
            1 => MaxSpeciesID_1,
            2 => MaxSpeciesID_2,
            3 => MaxSpeciesID_3,
            4 => MaxSpeciesID_4,
            5 => MaxSpeciesID_5,
            6 => MaxSpeciesID_6,
            7 when GameVersion.GG.Contains(version) => MaxSpeciesID_7b,
            7 when GameVersion.USUM.Contains(version) => MaxSpeciesID_7_USUM,
            7 => MaxSpeciesID_7,
            8 when version is GameVersion.PLA => MaxSpeciesID_8a,
            8 when GameVersion.BDSP.Contains(version) => MaxSpeciesID_8b,
            8 => MaxSpeciesID_8_R2,
            _ => -1,
        };

        internal static int GetMaxSpeciesOrigin(EntityContext context) => context switch
        {
            EntityContext.Gen1 => MaxSpeciesID_1,
            EntityContext.Gen2 => MaxSpeciesID_2,
            EntityContext.Gen3 => MaxSpeciesID_3,
            EntityContext.Gen4 => MaxSpeciesID_4,
            EntityContext.Gen5 => MaxSpeciesID_5,
            EntityContext.Gen6 => MaxSpeciesID_6,
            EntityContext.Gen7 => MaxSpeciesID_7_USUM,
            EntityContext.Gen8 => MaxSpeciesID_8_R2,

            EntityContext.Gen7b => MaxSpeciesID_7b,
            EntityContext.Gen8a => MaxSpeciesID_8a,
            EntityContext.Gen8b => MaxSpeciesID_8b,
            _ => -1,
        };

        internal static int GetMaxSpeciesOrigin(int generation) => generation switch
        {
            1 => MaxSpeciesID_1,
            2 => MaxSpeciesID_2,
            3 => MaxSpeciesID_3,
            4 => MaxSpeciesID_4,
            5 => MaxSpeciesID_5,
            6 => MaxSpeciesID_6,
            7 => MaxSpeciesID_7b,
            8 => MaxSpeciesID_8a,
            _ => -1,
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
            <= MaxSpeciesID_8a => 8,
            _ => -1,
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
            _ => -1,
        };

        internal static int GetMaxMoveID(int generation) => generation switch
        {
            1 => MaxMoveID_1,
            2 => MaxMoveID_2,
            3 => MaxMoveID_3,
            4 => MaxMoveID_4,
            5 => MaxMoveID_5,
            6 => MaxMoveID_6_AO,
            7 => MaxMoveID_7b,
            8 => MaxMoveID_8a,
            _ => -1,
        };

        internal const GameVersion NONE = GameVersion.Invalid;
        internal static readonly LearnVersion LearnNONE = new(-1);

        internal static bool HasVisitedB2W2(this PKM pkm, int species) => pkm.InhabitedGeneration(5, species);
        internal static bool HasVisitedORAS(this PKM pkm, int species) => pkm.InhabitedGeneration(6, species) && (pkm.AO || !pkm.IsUntraded);
        internal static bool HasVisitedUSUM(this PKM pkm, int species) => pkm.InhabitedGeneration(7, species) && (pkm.USUM || !pkm.IsUntraded);

        internal static bool HasVisitedSWSH(this PKM pkm, EvoCriteria[] evos)
        {
            if (pkm.SWSH)
                return true;
            if (pkm.IsUntraded)
                return false;
            if (pkm.BDSP && pkm.Species is (int)Species.Spinda or (int)Species.Nincada)
                return false;

            var pt = PersonalTable.SWSH;
            foreach (var evo in evos)
            {
                var pi = pt.GetFormEntry(evo.Species, evo.Form);
                if (pi.IsPresentInGame)
                    return true;
            }
            return false;
        }

        internal static bool HasVisitedBDSP(this PKM pkm, EvoCriteria[] evos)
        {
            if (pkm.BDSP)
                return true;
            if (pkm.IsUntraded)
                return false;
            if (pkm.Species is (int)Species.Spinda or (int)Species.Nincada)
                return false;

            var pt = PersonalTable.BDSP;
            foreach (var evo in evos)
            {
                var pi = pt.GetFormEntry(evo.Species, evo.Form);
                if (pi.IsPresentInGame)
                    return true;
            }
            return false;
        }

        internal static bool HasVisitedLA(this PKM pkm, EvoCriteria[] evos)
        {
            if (pkm.LA)
                return true;
            if (pkm.IsUntraded)
                return false;

            var pt = PersonalTable.LA;
            foreach (var evo in evos)
            {
                var pi = pt.GetFormEntry(evo.Species, evo.Form);
                if (pi.IsPresentInGame)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Checks if the moveset is restricted to only a specific version.
        /// </summary>
        /// <param name="pkm">Entity to check</param>
        internal static (bool IsRestricted, GameVersion Game) IsMovesetRestricted(this PKM pkm) => pkm switch
        {
            PB7 => (true, GameVersion.GP),
            PA8 => (true, GameVersion.PLA),
            PB8 => (true, GameVersion.BD),
            PK8 when pkm.Version > (int)GameVersion.SH => (true, GameVersion.SH), // Permit past generation moves.

            IBattleVersion { BattleVersion: not 0 } bv => (true, (GameVersion)bv.BattleVersion),
            _ when pkm.IsUntraded => (true, (GameVersion)pkm.Version),
            _ => (false, GameVersion.Any),
        };

        /// <summary>
        /// Checks if the relearn moves should be wiped.
        /// </summary>
        /// <remarks>Already checked for generations &lt; 8.</remarks>
        /// <param name="pkm">Entity to check</param>
        internal static bool IsOriginalMovesetDeleted(this PKM pkm)
        {
            if (pkm is PA8 {LA: false} or PB8 {BDSP: false})
                return true;
            if (pkm.IsNative)
            {
                if (pkm is PK8 {LA: true} or PK8 {BDSP: true})
                    return true;
                return false;
            }

            if (pkm is IBattleVersion { BattleVersion: not 0 })
                return true;

            return false;
        }

        /// <summary>
        /// Indicates if PP Ups are available for use.
        /// </summary>
        /// <param name="pkm">Entity to check</param>
        public static bool IsPPUpAvailable(PKM pkm)
        {
            return pkm is not PA8;
        }

        public static int GetMaxLengthOT(int generation, LanguageID language) => language switch
        {
            LanguageID.ChineseS or LanguageID.ChineseT => 6,
            LanguageID.Japanese or LanguageID.Korean => generation >= 6 ? 6 : 5,
            _ => generation >= 6 ? 12 : 7,
        };

        public static int GetMaxLengthNickname(int generation, LanguageID language) => language switch
        {
            LanguageID.ChineseS or LanguageID.ChineseT => 6,
            LanguageID.Japanese or LanguageID.Korean => generation >= 6 ? 6 : 5,
            _ => generation >= 6 ? 12 : 10,
        };

        public static bool GetIsFixedIVSequenceValidSkipRand(ReadOnlySpan<int> IVs, PKM pkm, int max = 31)
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

        public static bool GetIsFixedIVSequenceValidNoRand(ReadOnlySpan<int> IVs, PKM pkm)
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
