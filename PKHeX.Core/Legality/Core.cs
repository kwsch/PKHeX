using System;

namespace PKHeX.Core;

/// <summary>
/// Loosely aggregated legality logic.
/// </summary>
public static class Legal
{
    internal const int MaxSpeciesID_1 = 151;
    internal const int MaxMoveID_1 = 165;
    internal const int MaxItemID_1 = 255;
    internal const int MaxAbilityID_1 = 0;

    internal const int MaxSpeciesID_2 = 251;
    internal const int MaxMoveID_2 = 251;
    internal const int MaxItemID_2 = 255;
    internal const int MaxAbilityID_2 = 0;

    internal const int MaxSpeciesID_3 = 386;
    internal const int MaxMoveID_3 = 354;
    internal const int MaxItemID_3 = 374;
    internal const int MaxItemID_3_E = 376;
    internal const int MaxItemID_3_COLO = 547;
    internal const int MaxItemID_3_XD = 593;
    internal const int MaxAbilityID_3 = 77;
    internal const int MaxBallID_3 = 0xC;
    internal const GameVersion MaxGameID_3 = GameVersion.CXD;

    internal const int MaxSpeciesID_4 = 493;
    internal const int MaxMoveID_4 = 467;
    internal const int MaxItemID_4_DP = 464;
    internal const int MaxItemID_4_Pt = 467;
    internal const int MaxItemID_4_HGSS = 536;
    internal const int MaxAbilityID_4 = 123;
    internal const int MaxBallID_4 = 0x18;
    internal const GameVersion MaxGameID_4 = GameVersion.CXD;

    internal const int MaxSpeciesID_5 = 649;
    internal const int MaxMoveID_5 = 559;
    internal const int MaxItemID_5_BW = 632;
    internal const int MaxItemID_5_B2W2 = 638;
    internal const int MaxAbilityID_5 = 164;
    internal const int MaxBallID_5 = 0x19;
    internal const GameVersion MaxGameID_5 = GameVersion.B2;

    internal const int MaxSpeciesID_6 = 721;
    internal const int MaxMoveID_6_XY = 617;
    internal const int MaxMoveID_6_AO = 621;
    internal const int MaxItemID_6_XY = 717;
    internal const int MaxItemID_6_AO = 775;
    internal const int MaxAbilityID_6_XY = 188;
    internal const int MaxAbilityID_6_AO = 191;
    internal const int MaxBallID_6 = 0x19;
    internal const GameVersion MaxGameID_6 = GameVersion.OR;

    internal const int MaxSpeciesID_7 = 802;
    internal const int MaxMoveID_7 = 719;
    internal const int MaxItemID_7 = 920;
    internal const int MaxAbilityID_7 = 232;
    internal const int MaxBallID_7 = 0x1A; // 26
    internal const GameVersion MaxGameID_7 = GameVersion.C;

    internal const int MaxSpeciesID_7_USUM = 807;
    internal const int MaxMoveID_7_USUM = 728;
    internal const int MaxItemID_7_USUM = 959;
    internal const int MaxAbilityID_7_USUM = 233;

    internal const int MaxSpeciesID_7b = 809; // Melmetal
    internal const int MaxMoveID_7b = 742; // Double Iron Bash
    internal const int MaxItemID_7b = 1057; // Magmar Candy
    internal const int MaxBallID_7b = (int)Ball.Beast;
    internal const GameVersion MaxGameID_7b = GameVersion.GE;
    internal const int MaxAbilityID_7b = MaxAbilityID_7_USUM;

    // Current Binaries
    internal const int MaxSpeciesID_8 = MaxSpeciesID_8_R2;
    internal const int MaxMoveID_8 = MaxMoveID_8_R2;
    internal const int MaxItemID_8 = MaxItemID_8_R2;
    internal const int MaxAbilityID_8 = MaxAbilityID_8_R2;

    // Orion (No DLC)
    internal const int MaxSpeciesID_8_O0 = 890; // Eternatus
    internal const int MaxMoveID_8_O0 = 796; // Steel Beam
    internal const int MaxItemID_8_O0 = 1278; // Rotom Catalog, ignore all catalog parts
    internal const int MaxAbilityID_8_O0 = 258; // Hunger Switch

    // Rigel 1 (DLC 1: Isle of Armor)
    internal const int MaxSpeciesID_8_R1 = 893; // Zarude
    internal const int MaxMoveID_8_R1 = 818; // Surging Strikes
    internal const int MaxItemID_8_R1 = 1589; // Mark Charm
    internal const int MaxAbilityID_8_R1 = 260; // Unseen Fist

    // Rigel 2 (DLC 2: Crown Tundra)
    internal const int MaxSpeciesID_8_R2 = 898; // Calyrex
    internal const int MaxMoveID_8_R2 = 826; // Eerie Spell
    internal const int MaxItemID_8_R2 = 1607; // Reins of Unity
    internal const int MaxAbilityID_8_R2 = 267; // As One (Glastrier)

    internal const int MaxBallID_8 = 0x1A; // 26 Beast
    internal const GameVersion MaxGameID_8 = GameVersion.SH;

    internal const int MaxSpeciesID_8a = (int)Species.Enamorus;
    internal const int MaxMoveID_8a = (int)Move.TakeHeart;
    internal const int MaxItemID_8a = 1828; // Legend Plate
    internal const int MaxBallID_8a = (int)Ball.LAOrigin;
  //internal const GameVersion MaxGameID_8a = GameVersion.SP;
    internal const int MaxAbilityID_8a = MaxAbilityID_8_R2;

    internal const int MaxSpeciesID_8b = MaxSpeciesID_4; // Arceus-493
    internal const int MaxMoveID_8b = MaxMoveID_8_R2;
    internal const int MaxItemID_8b = 1822; // DS Sounds
    internal const int MaxBallID_8b = (int)Ball.LAOrigin;
  //internal const GameVersion MaxGameID_8b = GameVersion.SP;
    internal const int MaxAbilityID_8b = MaxAbilityID_8_R2;

    internal const int MaxSpeciesID_9 = MaxSpeciesID_9_T2;
    internal const int MaxMoveID_9 = MaxMoveID_9_T2;
    internal const int MaxItemID_9 = MaxItemID_9_T2;
    internal const int MaxAbilityID_9 = MaxAbilityID_9_T2;

    internal const int MaxSpeciesID_9_T0 = (int)Species.IronLeaves;
    internal const int MaxMoveID_9_T0 = (int)Move.MagicalTorque;
    internal const int MaxItemID_9_T0 = 2400; // Yellow Dish
    internal const int MaxAbilityID_9_T0 = (int)Ability.MyceliumMight;

    internal const int MaxSpeciesID_9_T1 = (int)Species.Ogerpon;
    internal const int MaxMoveID_9_T1 = (int)Move.IvyCudgel;
    internal const int MaxItemID_9_T1 = 2481; // Glimmering Charm
    internal const int MaxAbilityID_9_T1 = (int)Ability.SupersweetSyrup;

    internal const int MaxSpeciesID_9_T2 = (int)Species.Pecharunt;
    internal const int MaxMoveID_9_T2 = (int)Move.MalignantChain;
    internal const int MaxItemID_9_T2 = 2557; // Briar’s Book
    internal const int MaxAbilityID_9_T2 = (int)Ability.PoisonPuppeteer;

    internal const int MaxBallID_9 = (int)Ball.LAOrigin;
    internal const GameVersion MaxGameID_9 = GameVersion.VL;
    internal const GameVersion MaxGameID_HOME = MaxGameID_9;

    internal static readonly ushort[] HeldItems_GSC = ItemStorage2.GetAllHeld();
    internal static readonly ushort[] HeldItems_RS = ItemStorage3RS.GetAllHeld();
    internal static readonly ushort[] HeldItems_DP = ItemStorage4DP.GetAllHeld();
    internal static readonly ushort[] HeldItems_Pt = ItemStorage4Pt.GetAllHeld(); // Griseous Orb Added
    internal static readonly ushort[] HeldItems_HGSS = HeldItems_Pt;
    internal static readonly ushort[] HeldItems_BW = ItemStorage5.GetAllHeld();
    internal static readonly ushort[] HeldItems_XY = ItemStorage6XY.GetAllHeld();
    internal static readonly ushort[] HeldItems_AO = ItemStorage6AO.GetAllHeld();
    internal static readonly ushort[] HeldItems_SM = ItemStorage7SM.GetAllHeld();
    internal static readonly ushort[] HeldItems_USUM = ItemStorage7USUM.GetAllHeld();
    internal static readonly ushort[] HeldItems_GG = [];
    internal static readonly ushort[] HeldItems_SWSH = ItemStorage8SWSH.GetAllHeld();
    internal static readonly ushort[] HeldItems_BS = ItemStorage8BDSP.GetAllHeld();
    internal static readonly ushort[] HeldItems_LA = [];
    internal static readonly ushort[] HeldItems_SV = ItemStorage9SV.GetAllHeld();

    internal static int GetMaxLanguageID(byte generation) => generation switch
    {
        1 => (int) LanguageID.Spanish, // 1-7 except 6
        3 => (int) LanguageID.Spanish, // 1-7 except 6
        2 => (int) LanguageID.Korean,
        4 => (int) LanguageID.Korean,
        5 => (int) LanguageID.Korean,
        6 => (int) LanguageID.Korean,
        7 => (int) LanguageID.ChineseT,
        8 => (int) LanguageID.ChineseT,
        9 => (int) LanguageID.ChineseT,
        _ => -1,
    };

    /// <summary>
    /// Checks if the relearn moves should be wiped.
    /// </summary>
    /// <remarks>Already checked for generations &lt; 8.</remarks>
    /// <param name="pk">Entity to check</param>
    internal static bool IsOriginalMovesetDeleted(this PKM pk) => pk switch
    {
        PA8 pa8 => !pa8.LA,
        PB8 pb8 => !pb8.BDSP,
        PK8 pk8 => pk8.IsSideTransfer || pk8.BattleVersion != 0,
        PK9 pk9 => !(pk9.SV || pk9 is { IsEgg: true, Version: 0 }),
        _ => false,
    };

    /// <summary>
    /// Indicates if PP Ups are available for use.
    /// </summary>
    /// <param name="pk">Entity to check</param>
    public static bool IsPPUpAvailable(PKM pk)
    {
        return pk is not PA8;
    }

    /// <summary>
    /// Indicate if PP Ups are available for use.
    /// </summary>
    /// <param name="moveID">Move ID</param>
    public static bool IsPPUpAvailable(ushort moveID) => moveID switch
    {
        0 => false,
        (int)Move.Sketch => false, // BD/SP v1.0 could use PP Ups on Sketch, but not in later versions. Disallow anyway.
        (int)Move.RevivalBlessing => false,
        _ => true,
    };

    /// <summary>
    /// Gets the maximum length of a Trainer Name for the input <see cref="generation"/> and <see cref="language"/>.
    /// </summary>
    /// <param name="generation">Generation of the Trainer</param>
    /// <param name="language">Language of the Trainer</param>
    public static int GetMaxLengthOT(byte generation, LanguageID language) => language switch
    {
        LanguageID.ChineseS or LanguageID.ChineseT => 6,
        LanguageID.Japanese or LanguageID.Korean => generation >= 6 ? 6 : 5,
        _ => generation >= 6 ? 12 : 7,
    };

    /// <summary>
    /// Gets the maximum length of a Nickname for the input <see cref="generation"/> and <see cref="language"/>.
    /// </summary>
    /// <param name="generation">Generation of the Trainer</param>
    /// <param name="language">Language of the Trainer</param>
    public static int GetMaxLengthNickname(byte generation, LanguageID language) => language switch
    {
        LanguageID.ChineseS or LanguageID.ChineseT => 6,
        LanguageID.Japanese or LanguageID.Korean => generation >= 6 ? 6 : 5,
        _ => generation >= 6 ? 12 : 10,
    };

    /// <summary>
    /// Checks if the input <see cref="pk"/> has IVs that match the template <see cref="IVs"/>.
    /// </summary>
    public static bool GetIsFixedIVSequenceValidSkipRand(ReadOnlySpan<int> IVs, PKM pk, uint max = 31)
    {
        for (int i = 5; i >= 0; i--)
        {
            var iv = IVs[i];
            if ((uint)iv > max) // random
                continue;
            if (iv != pk.GetIV(i))
                return false;
        }
        return true;
    }

    /// <summary>
    /// Checks if the input <see cref="pk"/> has IVs that match the template <see cref="IVs"/>.
    /// </summary>
    public static bool GetIsFixedIVSequenceValidSkipRand(in IndividualValueSet IVs, PKM pk, int max = 31)
    {
        // Template IVs not in the [0,max] range are random. Only check for IVs within the "specified" range.
        if ((uint)IVs.HP  <= max && IVs.HP  != pk.IV_HP ) return false;
        if ((uint)IVs.ATK <= max && IVs.ATK != pk.IV_ATK) return false;
        if ((uint)IVs.DEF <= max && IVs.DEF != pk.IV_DEF) return false;
        if ((uint)IVs.SPE <= max && IVs.SPE != pk.IV_SPE) return false;
        if ((uint)IVs.SPA <= max && IVs.SPA != pk.IV_SPA) return false;
        if ((uint)IVs.SPD <= max && IVs.SPD != pk.IV_SPD) return false;
        return true;
    }

    /// <summary>
    /// Checks if the input <see cref="pk"/> has IVs that match the template <see cref="IVs"/>.
    /// </summary>
    public static bool GetIsFixedIVSequenceValidNoRand(in IndividualValueSet IVs, PKM pk)
    {
        if (IVs.HP  != pk.IV_HP ) return false;
        if (IVs.ATK != pk.IV_ATK) return false;
        if (IVs.DEF != pk.IV_DEF) return false;
        if (IVs.SPE != pk.IV_SPE) return false;
        if (IVs.SPA != pk.IV_SPA) return false;
        if (IVs.SPD != pk.IV_SPD) return false;
        return true;
    }
}
