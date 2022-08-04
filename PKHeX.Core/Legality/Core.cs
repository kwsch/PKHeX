using System;
using static PKHeX.Core.BinLinkerAccessor;

namespace PKHeX.Core;

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

    internal static int GetMaxSpeciesOrigin(PKM pk)
    {
        if (pk.Format == 1)
            return GetMaxSpeciesOrigin(1);
        if (pk.Format == 2 || pk.VC)
            return GetMaxSpeciesOrigin(2);
        return GetMaxSpeciesOrigin(pk.Generation);
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

    /// <summary>
    /// Checks if the relearn moves should be wiped.
    /// </summary>
    /// <remarks>Already checked for generations &lt; 8.</remarks>
    /// <param name="pk">Entity to check</param>
    internal static bool IsOriginalMovesetDeleted(this PKM pk)
    {
        if (pk is PA8 {LA: false} or PB8 {BDSP: false})
            return true;
        if (pk.IsNative)
        {
            if (pk is PK8 {LA: true} or PK8 {BDSP: true})
                return true;
            return false;
        }

        if (pk is IBattleVersion { BattleVersion: not 0 })
            return true;

        return false;
    }

    /// <summary>
    /// Indicates if PP Ups are available for use.
    /// </summary>
    /// <param name="pk">Entity to check</param>
    public static bool IsPPUpAvailable(PKM pk)
    {
        return pk is not PA8;
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

    public static bool GetIsFixedIVSequenceValidSkipRand(ReadOnlySpan<int> IVs, PKM pk, int max = 31)
    {
        for (int i = 0; i < 6; i++)
        {
            if ((uint) IVs[i] > max) // random
                continue;
            if (IVs[i] != pk.GetIV(i))
                return false;
        }
        return true;
    }

    public static bool GetIsFixedIVSequenceValidNoRand(ReadOnlySpan<int> IVs, PKM pk)
    {
        for (int i = 0; i < 6; i++)
        {
            if (IVs[i] != pk.GetIV(i))
                return false;
        }
        return true;
    }

    public static bool IsMetAsEgg(PKM pk) => pk switch
    {
        PA8 or PK8 => pk.Egg_Location is not 0 || (pk.BDSP && pk.Egg_Day is not 0),
        PB8 pb8 => pb8.Egg_Location is not Locations.Default8bNone,
        _ => pk.Egg_Location is not 0,
    };
}
