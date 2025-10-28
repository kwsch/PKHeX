using static PKHeX.Core.GameVersion;
using static PKHeX.Core.PIDType;
using static PKHeX.Core.Shiny;
using static PKHeX.Core.LanguageID;
using static PKHeX.Core.GiftGender3;

namespace PKHeX.Core;

/// <summary>
/// Generation 3 <see cref="EncounterGift3"/> Gifts
/// </summary>
/// <remarks>
/// Generation 3 has a wide range of PID/IV types and other restrictions, and was never consistently stored in raw bytes.
/// Normally we'd just load the data from a binary, but without raw data... hard-code everything by hand.
/// </remarks>
internal static class EncountersWC3
{
    // Higher frequency of being checked/requested.
    private static readonly EncounterGift3[] Common =
    [
        new(151, 10, R) { Moves = new(001,144,000,000), Method = BACD_M, ID32 = 06930, Shiny = Never, OriginalTrainerName = "MYSTRY", OriginalTrainerGender = RandD3, Language = (int)English, FatefulEncounter = true }, // Mew
        new(385, 05, R) { Moves = new(273,093,156,000), Method = BACD_R, ID32 = 20043, Shiny = Random, OriginalTrainerName = "WISHMKR", OriginalTrainerGender = Only0, Language = (int)English },
        new(385, 05, RS, false, met: 0) { Moves = new(273,093,156,000), Method = Channel, TID16 = 40122, Shiny = Random, OriginalTrainerName = "CHANNEL", OriginalTrainerGender = RandAlgo },
    ];

    private static readonly EncounterGift3[] Japan =
    [
        new(263, 05, S) { Moves = new(033,045,039,000), Language = (int)Japanese, Method = BACD_RBCD, ID32 = 21121, Shiny = Always, OriginalTrainerName = "ルビー",     OriginalTrainerGender = RandD3_1 }, // Berry Fix Ruby
        new(263, 05, S) { Moves = new(033,045,039,000), Language = (int)Japanese, Method = BACD_RBCD, ID32 = 21121, Shiny = Always, OriginalTrainerName = "サファイア", OriginalTrainerGender = RandD3_0 }, // Berry Fix Sapphire

        new(385, 05, R) { Moves = new(273,093,156,000), Language = (int)Japanese, Method = BACD_TA,   ID32 = 30719, Shiny = Never, OriginalTrainerName = "ネガイボシ", OriginalTrainerGender = Only0 }, // Negai Boshi Jirachi
        new(385, 05, RS){ Moves = new(273,093,156,000), Language = (int)Japanese, Method = BACD_U_AX, ID32 = 30719, Shiny = Never, OriginalTrainerName = "ネガイボシ", OriginalTrainerGender = Recipient }, // Negai Boshi Jirachi (Match Recipient)
        new(385, 05, R) { Moves = new(273,093,156,000), Language = (int)Japanese, Method = BACD_R_A, ID32 = 40707, Shiny = Never, OriginalTrainerName = "タナバタ",   OriginalTrainerGender = Only1 }, // Tanabata Jirachi (2004)
        new(025, 10, R) { Moves = new(019,084,039,086), Language = (int)Japanese, Method = BACD_R_A, ID32 = 41205, Shiny = Never, OriginalTrainerName = "ＡＮＡ",     OriginalTrainerGender = Only0 }, // ANA Pikachu
        new(052, 05, R) { Moves = new(010,045,000,000), Language = (int)Japanese, Method = BACD_R_A, ID32 = 50318, Shiny = Never, OriginalTrainerName = "ポケパーク", OriginalTrainerGender = Only0 }, // PokéPark Meowth
        new(025, 10, R) { Moves = new(084,045,086,057), Language = (int)Japanese, Method = BACD_R_A, ID32 = 50319, Shiny = Never, OriginalTrainerName = "ヨコハマ",  OriginalTrainerGender = Only0 }, // Yokohama Pikachu
        new(151, 10, R) { Moves = new(001,144,000,000), Language = (int)Japanese, Method = BACD_R_A, ID32 = 50716, Shiny = Never, OriginalTrainerName = "ハドウ",    OriginalTrainerGender = RandD3, FatefulEncounter = true }, // Hadou Mew
        new(025, 10, R) { Moves = new(045,039,086,019), Language = (int)Japanese, Method = BACD_R_A, ID32 = 50425, Shiny = Never, OriginalTrainerName = "ＧＷ",      OriginalTrainerGender = RandS3 }, // GW Pikachu
        new(025, 10, R) { Moves = new(045,039,086,019), Language = (int)Japanese, Method = BACD_R_A, ID32 = 50701, Shiny = Never, OriginalTrainerName = "サッポロ",  OriginalTrainerGender = Only0 }, // Sapporo Pikachu
        new(385, 05, R) { Moves = new(273,093,156,000), Language = (int)Japanese, Method = BACD_R_A, ID32 = 50707, Shiny = Never, OriginalTrainerName = "タナバタ",  OriginalTrainerGender = Only1 }, // Tanabata Jirachi (2005)
        new(375, 30, R) { Moves = new(036,093,232,287), Language = (int)Japanese, Method = BACD_R_A, ID32 = 02005, Shiny = Never, OriginalTrainerName = "フェスタ",  OriginalTrainerGender = Only0, RibbonNational = true }, // Festa Metang
        new(202, 05, R) { Moves = new(068,243,219,194), Language = (int)Japanese, Method = BACD_R_A, ID32 = 50701, Shiny = Never, OriginalTrainerName = "サンデー",  OriginalTrainerGender = RandS3 }, // Sunday Wobbuffet
        new(377, 40, R) { Moves = new(174,276,246,063), Language = (int)Japanese, Method = BACD_R_A, ID32 = 50901, Shiny = Never, OriginalTrainerName = "ハドウ",    OriginalTrainerGender = RandSG15 }, // Regirock
        new(378, 40, R) { Moves = new(174,276,246,063), Language = (int)Japanese, Method = BACD_R_A, ID32 = 50901, Shiny = Never, OriginalTrainerName = "ハドウ",    OriginalTrainerGender = RandSG15 }, // Regice
        new(379, 40, R) { Moves = new(174,276,246,063), Language = (int)Japanese, Method = BACD_R_A, ID32 = 50901, Shiny = Never, OriginalTrainerName = "ハドウ",    OriginalTrainerGender = RandSG15 }, // Registeel
        new(151, 30, R) { Moves = new(001,144,005,118), Language = (int)Japanese, Method = BACD_R_A, ID32 = 60510, Shiny = Never, OriginalTrainerName = "ポケパーク", OriginalTrainerGender = RandD3, FatefulEncounter = true }, // PokéPark Mew
        new(251, 30, R) { Moves = new(215,219,246,248), Language = (int)Japanese, Method = BACD_R_A, ID32 = 60623, Shiny = Never, OriginalTrainerName = "ポケパーク", OriginalTrainerGender = RandS7 }, // PokéPark Celebi
        new(385, 05, R) { Moves = new(273,093,156,000), Language = (int)Japanese, Method = BACD_R_A, ID32 = 60707, Shiny = Never, OriginalTrainerName = "タナバタ",   OriginalTrainerGender = RandS7 }, // Tanabata Jirachi (2006)
        new(251, 10, R) { Moves = new(073,105,215,219), Language = (int)Japanese, Method = BACD_R_A, ID32 = 60720, Shiny = Never, OriginalTrainerName = "ミツリン",   OriginalTrainerGender = RandS7 }, // Mitsurin Celebi (2006)
        new(385, 30, R) { Moves = new(273,094,270,156), Language = (int)Japanese, Method = BACD_R_A, ID32 = 60731, Shiny = Never, OriginalTrainerName = "ポケパーク", OriginalTrainerGender = RandD3 }, // PokéPark Jirachi (2006)
        new(385, 30, R) { Moves = new(273,094,270,156), Language = (int)Japanese, Method = BACD_R_A, ID32 = 60830, Shiny = Never, OriginalTrainerName = "ポケパーク", OriginalTrainerGender = RandD3 }, // PokéPark Jirachi (2006)
    ];

    private static readonly EncounterGift3[] International =
    [
        // EBGames/GameStop (March 1, 2004, to April 22, 2007), also via multi-game discs
        new(263, 5, S) { Moves = new(033,045,039,000), Method = BACD_RBCD, ID32 = 30317, Shiny = Always, OriginalTrainerName = "RUBY",    OriginalTrainerGender = RandD3_1, Language = (int)English }, // Berry Fix Ruby
        new(263, 5, S) { Moves = new(033,045,039,000), Method = BACD_RBCD, ID32 = 30317, Shiny = Always, OriginalTrainerName = "SAPHIRE", OriginalTrainerGender = RandD3_0, Language = (int)English }, // Berry Fix Sapphire

        // English
        new(006, 70, R) { Moves = new(017,163,082,083), Method = BACD_R_A, ID32 = 06227, Shiny = Never, OriginalTrainerName = "10ANNIV", OriginalTrainerGender = RandS7, Language = (int)English }, // Charizard
        new(025, 70, R) { Moves = new(085,097,087,113), Method = BACD_R_A, ID32 = 06227, Shiny = Never, OriginalTrainerName = "10ANNIV", OriginalTrainerGender = RandS7, Language = (int)English }, // Pikachu
        new(144, 70, R) { Moves = new(097,170,058,115), Method = BACD_R_A, ID32 = 06227, Shiny = Never, OriginalTrainerName = "10ANNIV", OriginalTrainerGender = RandS7, Language = (int)English }, // Articuno
        new(243, 70, R) { Moves = new(098,209,115,242), Method = BACD_R_A, ID32 = 06227, Shiny = Never, OriginalTrainerName = "10ANNIV", OriginalTrainerGender = RandS7, Language = (int)English }, // Raikou
        new(244, 70, R) { Moves = new(083,023,053,207), Method = BACD_R_A, ID32 = 06227, Shiny = Never, OriginalTrainerName = "10ANNIV", OriginalTrainerGender = RandS7, Language = (int)English }, // Entei
        new(245, 70, R) { Moves = new(016,062,054,243), Method = BACD_R_A, ID32 = 06227, Shiny = Never, OriginalTrainerName = "10ANNIV", OriginalTrainerGender = RandS7, Language = (int)English }, // Suicune
        new(249, 70, R) { Moves = new(105,056,240,129), Method = BACD_R_A, ID32 = 06227, Shiny = Never, OriginalTrainerName = "10ANNIV", OriginalTrainerGender = RandS7, Language = (int)English }, // Lugia
        new(250, 70, R) { Moves = new(105,126,241,129), Method = BACD_R_A, ID32 = 06227, Shiny = Never, OriginalTrainerName = "10ANNIV", OriginalTrainerGender = RandS7, Language = (int)English }, // Ho-Oh
        new(380, 70, R) { Moves = new(296,094,105,204), Method = BACD_R_A, ID32 = 06227, Shiny = Never, OriginalTrainerName = "10ANNIV", OriginalTrainerGender = RandS7, Language = (int)English }, // Latias
        new(381, 70, R) { Moves = new(295,094,105,349), Method = BACD_R_A, ID32 = 06227, Shiny = Never, OriginalTrainerName = "10ANNIV", OriginalTrainerGender = RandS7, Language = (int)English }, // Latios

        // French
        new(006, 70, R) { Moves = new(017,163,082,083), Method = BACD_R_A, ID32 = 06227, Shiny = Never, OriginalTrainerName = "10ANNIV", OriginalTrainerGender = RandS7, Language = (int)French }, // Charizard
        new(025, 70, R) { Moves = new(085,097,087,113), Method = BACD_R_A, ID32 = 06227, Shiny = Never, OriginalTrainerName = "10ANNIV", OriginalTrainerGender = RandS7, Language = (int)French }, // Pikachu
        new(144, 70, R) { Moves = new(097,170,058,115), Method = BACD_R_A, ID32 = 06227, Shiny = Never, OriginalTrainerName = "10ANNIV", OriginalTrainerGender = RandS7, Language = (int)French }, // Articuno
        new(243, 70, R) { Moves = new(098,209,115,242), Method = BACD_R_A, ID32 = 06227, Shiny = Never, OriginalTrainerName = "10ANNIV", OriginalTrainerGender = RandS7, Language = (int)French }, // Raikou
        new(244, 70, R) { Moves = new(083,023,053,207), Method = BACD_R_A, ID32 = 06227, Shiny = Never, OriginalTrainerName = "10ANNIV", OriginalTrainerGender = RandS7, Language = (int)French }, // Entei
        new(245, 70, R) { Moves = new(016,062,054,243), Method = BACD_R_A, ID32 = 06227, Shiny = Never, OriginalTrainerName = "10ANNIV", OriginalTrainerGender = RandS7, Language = (int)French }, // Suicune
        new(249, 70, R) { Moves = new(105,056,240,129), Method = BACD_R_A, ID32 = 06227, Shiny = Never, OriginalTrainerName = "10ANNIV", OriginalTrainerGender = RandS7, Language = (int)French }, // Lugia
        new(250, 70, R) { Moves = new(105,126,241,129), Method = BACD_R_A, ID32 = 06227, Shiny = Never, OriginalTrainerName = "10ANNIV", OriginalTrainerGender = RandS7, Language = (int)French }, // Ho-Oh
        new(380, 70, R) { Moves = new(296,094,105,204), Method = BACD_R_A, ID32 = 06227, Shiny = Never, OriginalTrainerName = "10ANNIV", OriginalTrainerGender = RandS7, Language = (int)French }, // Latias
        new(381, 70, R) { Moves = new(295,094,105,349), Method = BACD_R_A, ID32 = 06227, Shiny = Never, OriginalTrainerName = "10ANNIV", OriginalTrainerGender = RandS7, Language = (int)French }, // Latios

        // German
        new(006, 70, R) { Moves = new(017,163,082,083), Method = BACD_R_A, ID32 = 06227, Shiny = Never, OriginalTrainerName = "10JAHRE", OriginalTrainerGender = RandS7, Language = (int)German }, // Charizard
        new(025, 70, R) { Moves = new(085,097,087,113), Method = BACD_R_A, ID32 = 06227, Shiny = Never, OriginalTrainerName = "10JAHRE", OriginalTrainerGender = RandS7, Language = (int)German }, // Pikachu
        new(144, 70, R) { Moves = new(097,170,058,115), Method = BACD_R_A, ID32 = 06227, Shiny = Never, OriginalTrainerName = "10JAHRE", OriginalTrainerGender = RandS7, Language = (int)German }, // Articuno
        new(243, 70, R) { Moves = new(098,209,115,242), Method = BACD_R_A, ID32 = 06227, Shiny = Never, OriginalTrainerName = "10JAHRE", OriginalTrainerGender = RandS7, Language = (int)German }, // Raikou
        new(244, 70, R) { Moves = new(083,023,053,207), Method = BACD_R_A, ID32 = 06227, Shiny = Never, OriginalTrainerName = "10JAHRE", OriginalTrainerGender = RandS7, Language = (int)German }, // Entei
        new(245, 70, R) { Moves = new(016,062,054,243), Method = BACD_R_A, ID32 = 06227, Shiny = Never, OriginalTrainerName = "10JAHRE", OriginalTrainerGender = RandS7, Language = (int)German }, // Suicune
        new(249, 70, R) { Moves = new(105,056,240,129), Method = BACD_R_A, ID32 = 06227, Shiny = Never, OriginalTrainerName = "10JAHRE", OriginalTrainerGender = RandS7, Language = (int)German }, // Lugia
        new(250, 70, R) { Moves = new(105,126,241,129), Method = BACD_R_A, ID32 = 06227, Shiny = Never, OriginalTrainerName = "10JAHRE", OriginalTrainerGender = RandS7, Language = (int)German }, // Ho-Oh
        new(380, 70, R) { Moves = new(296,094,105,204), Method = BACD_R_A, ID32 = 06227, Shiny = Never, OriginalTrainerName = "10JAHRE", OriginalTrainerGender = RandS7, Language = (int)German }, // Latias
        new(381, 70, R) { Moves = new(295,094,105,349), Method = BACD_R_A, ID32 = 06227, Shiny = Never, OriginalTrainerName = "10JAHRE", OriginalTrainerGender = RandS7, Language = (int)German }, // Latios

        // Italian
        new(006, 70, R) { Moves = new(017,163,082,083), Method = BACD_R_A, ID32 = 06227, Shiny = Never, OriginalTrainerName = "10ANNI", OriginalTrainerGender = RandS7, Language = (int)Italian }, // Charizard
        new(025, 70, R) { Moves = new(085,097,087,113), Method = BACD_R_A, ID32 = 06227, Shiny = Never, OriginalTrainerName = "10ANNI", OriginalTrainerGender = RandS7, Language = (int)Italian }, // Pikachu
        new(144, 70, R) { Moves = new(097,170,058,115), Method = BACD_R_A, ID32 = 06227, Shiny = Never, OriginalTrainerName = "10ANNI", OriginalTrainerGender = RandS7, Language = (int)Italian }, // Articuno
        new(243, 70, R) { Moves = new(098,209,115,242), Method = BACD_R_A, ID32 = 06227, Shiny = Never, OriginalTrainerName = "10ANNI", OriginalTrainerGender = RandS7, Language = (int)Italian }, // Raikou
        new(244, 70, R) { Moves = new(083,023,053,207), Method = BACD_R_A, ID32 = 06227, Shiny = Never, OriginalTrainerName = "10ANNI", OriginalTrainerGender = RandS7, Language = (int)Italian }, // Entei
        new(245, 70, R) { Moves = new(016,062,054,243), Method = BACD_R_A, ID32 = 06227, Shiny = Never, OriginalTrainerName = "10ANNI", OriginalTrainerGender = RandS7, Language = (int)Italian }, // Suicune
        new(249, 70, R) { Moves = new(105,056,240,129), Method = BACD_R_A, ID32 = 06227, Shiny = Never, OriginalTrainerName = "10ANNI", OriginalTrainerGender = RandS7, Language = (int)Italian }, // Lugia
        new(250, 70, R) { Moves = new(105,126,241,129), Method = BACD_R_A, ID32 = 06227, Shiny = Never, OriginalTrainerName = "10ANNI", OriginalTrainerGender = RandS7, Language = (int)Italian }, // Ho-Oh
        new(380, 70, R) { Moves = new(296,094,105,204), Method = BACD_R_A, ID32 = 06227, Shiny = Never, OriginalTrainerName = "10ANNI", OriginalTrainerGender = RandS7, Language = (int)Italian }, // Latias
        new(381, 70, R) { Moves = new(295,094,105,349), Method = BACD_R_A, ID32 = 06227, Shiny = Never, OriginalTrainerName = "10ANNI", OriginalTrainerGender = RandS7, Language = (int)Italian }, // Latios

        // Spanish
        new(006, 70, R) { Moves = new(017,163,082,083), Method = BACD_R_A, ID32 = 06227, Shiny = Never, OriginalTrainerName = "10ANIV", OriginalTrainerGender = RandS7, Language = (int)Spanish }, // Charizard
        new(025, 70, R) { Moves = new(085,097,087,113), Method = BACD_R_A, ID32 = 06227, Shiny = Never, OriginalTrainerName = "10ANIV", OriginalTrainerGender = RandS7, Language = (int)Spanish }, // Pikachu
        new(144, 70, R) { Moves = new(097,170,058,115), Method = BACD_R_A, ID32 = 06227, Shiny = Never, OriginalTrainerName = "10ANIV", OriginalTrainerGender = RandS7, Language = (int)Spanish }, // Articuno
        new(243, 70, R) { Moves = new(098,209,115,242), Method = BACD_R_A, ID32 = 06227, Shiny = Never, OriginalTrainerName = "10ANIV", OriginalTrainerGender = RandS7, Language = (int)Spanish }, // Raikou
        new(244, 70, R) { Moves = new(083,023,053,207), Method = BACD_R_A, ID32 = 06227, Shiny = Never, OriginalTrainerName = "10ANIV", OriginalTrainerGender = RandS7, Language = (int)Spanish }, // Entei
        new(245, 70, R) { Moves = new(016,062,054,243), Method = BACD_R_A, ID32 = 06227, Shiny = Never, OriginalTrainerName = "10ANIV", OriginalTrainerGender = RandS7, Language = (int)Spanish }, // Suicune
        new(249, 70, R) { Moves = new(105,056,240,129), Method = BACD_R_A, ID32 = 06227, Shiny = Never, OriginalTrainerName = "10ANIV", OriginalTrainerGender = RandS7, Language = (int)Spanish }, // Lugia
        new(250, 70, R) { Moves = new(105,126,241,129), Method = BACD_R_A, ID32 = 06227, Shiny = Never, OriginalTrainerName = "10ANIV", OriginalTrainerGender = RandS7, Language = (int)Spanish }, // Ho-Oh
        new(380, 70, R) { Moves = new(296,094,105,204), Method = BACD_R_A, ID32 = 06227, Shiny = Never, OriginalTrainerName = "10ANIV", OriginalTrainerGender = RandS7, Language = (int)Spanish }, // Latias
        new(381, 70, R) { Moves = new(295,094,105,349), Method = BACD_R_A, ID32 = 06227, Shiny = Never, OriginalTrainerName = "10ANIV", OriginalTrainerGender = RandS7, Language = (int)Spanish }, // Latios

        // Aura Mew
        new(151, 10, R) { Moves = new(001,144,000,000), Method = BACD_R_A, ID32 = 20078, Shiny = Never, OriginalTrainerName = "Aura", OriginalTrainerGender = RandS7, FatefulEncounter = true }, // Mew

        // English Events
        new(375, 30, R) { Moves = new(036,093,232,287), Method = BACD_R_A, ID32 = 02005, Shiny = Never, OriginalTrainerName = "ROCKS",   OriginalTrainerGender = Only0, Language = (int)English, RibbonNational = true }, // Metang
        new(386, 70, R) { Moves = new(322,105,354,063), Method = BACD_R_A, ID32 = 28606, Shiny = Never, OriginalTrainerName = "DOEL",    OriginalTrainerGender = RandS7, Language = (int)English, FatefulEncounter = true }, // Deoxys
        new(386, 70, R) { Moves = new(322,105,354,063), Method = BACD_R_A, ID32 = 00010, Shiny = Never, OriginalTrainerName = "SPACE C", OriginalTrainerGender = RandS7, Language = (int)English, FatefulEncounter = true }, // Deoxys

        // Party of the Decade
        new(001, 70, R) { Moves = new(230,074,076,235), Method = BACD_R_A, ID32 = 06808, Shiny = Never, OriginalTrainerName = "10 ANIV", OriginalTrainerGender = RandS7, Language = (int)English }, // Bulbasaur
        new(006, 70, R) { Moves = new(017,163,082,083), Method = BACD_R_A, ID32 = 06808, Shiny = Never, OriginalTrainerName = "10 ANIV", OriginalTrainerGender = RandS7, Language = (int)English }, // Charizard
        new(009, 70, R) { Moves = new(182,240,130,056), Method = BACD_R_A, ID32 = 06808, Shiny = Never, OriginalTrainerName = "10 ANIV", OriginalTrainerGender = RandS7, Language = (int)English }, // Blastoise
        new(025, 70, R) { Moves = new(085,087,113,019), Method = BACD_R_A, ID32 = 06808, Shiny = Never, OriginalTrainerName = "10 ANIV", OriginalTrainerGender = RandS7, Language = (int)English }, // Pikachu (Fly)
        new(065, 70, R) { Moves = new(248,347,094,271), Method = BACD_R_A, ID32 = 06808, Shiny = Never, OriginalTrainerName = "10 ANIV", OriginalTrainerGender = RandS7, Language = (int)English }, // Alakazam
        new(144, 70, R) { Moves = new(097,170,058,115), Method = BACD_R_A, ID32 = 06808, Shiny = Never, OriginalTrainerName = "10 ANIV", OriginalTrainerGender = RandS7, Language = (int)English }, // Articuno
        new(145, 70, R) { Moves = new(097,197,065,268), Method = BACD_R_A, ID32 = 06808, Shiny = Never, OriginalTrainerName = "10 ANIV", OriginalTrainerGender = RandS7, Language = (int)English }, // Zapdos
        new(146, 70, R) { Moves = new(097,203,053,219), Method = BACD_R_A, ID32 = 06808, Shiny = Never, OriginalTrainerName = "10 ANIV", OriginalTrainerGender = RandS7, Language = (int)English }, // Moltres
        new(149, 70, R) { Moves = new(097,219,017,200), Method = BACD_R_A, ID32 = 06808, Shiny = Never, OriginalTrainerName = "10 ANIV", OriginalTrainerGender = RandS7, Language = (int)English }, // Dragonite
        new(157, 70, R) { Moves = new(098,172,129,053), Method = BACD_R_A, ID32 = 06808, Shiny = Never, OriginalTrainerName = "10 ANIV", OriginalTrainerGender = RandS7, Language = (int)English }, // Typhlosion
        new(196, 70, R) { Moves = new(060,244,094,234), Method = BACD_R_A, ID32 = 06808, Shiny = Never, OriginalTrainerName = "10 ANIV", OriginalTrainerGender = RandS7, Language = (int)English }, // Espeon
        new(197, 70, R) { Moves = new(185,212,103,236), Method = BACD_R_A, ID32 = 06808, Shiny = Never, OriginalTrainerName = "10 ANIV", OriginalTrainerGender = RandS7, Language = (int)English }, // Umbreon
        new(243, 70, R) { Moves = new(098,209,115,242), Method = BACD_R_A, ID32 = 06808, Shiny = Never, OriginalTrainerName = "10 ANIV", OriginalTrainerGender = RandS7, Language = (int)English }, // Raikou
        new(244, 70, R) { Moves = new(083,023,053,207), Method = BACD_R_A, ID32 = 06808, Shiny = Never, OriginalTrainerName = "10 ANIV", OriginalTrainerGender = RandS7, Language = (int)English }, // Entei
        new(245, 70, R) { Moves = new(016,062,054,243), Method = BACD_R_A, ID32 = 06808, Shiny = Never, OriginalTrainerName = "10 ANIV", OriginalTrainerGender = RandS7, Language = (int)English }, // Suicune
        new(248, 70, R) { Moves = new(037,184,242,089), Method = BACD_R_A, ID32 = 06808, Shiny = Never, OriginalTrainerName = "10 ANIV", OriginalTrainerGender = RandS7, Language = (int)English }, // Tyranitar
        new(257, 70, R) { Moves = new(299,163,119,327), Method = BACD_R_A, ID32 = 06808, Shiny = Never, OriginalTrainerName = "10 ANIV", OriginalTrainerGender = RandS7, Language = (int)English }, // Blaziken
        new(359, 70, R) { Moves = new(104,163,248,195), Method = BACD_R_A, ID32 = 06808, Shiny = Never, OriginalTrainerName = "10 ANIV", OriginalTrainerGender = RandS7, Language = (int)English }, // Absol
        new(380, 70, R) { Moves = new(296,094,105,204), Method = BACD_R_A, ID32 = 06808, Shiny = Never, OriginalTrainerName = "10 ANIV", OriginalTrainerGender = RandS7, Language = (int)English }, // Latias
        new(381, 70, R) { Moves = new(295,094,105,349), Method = BACD_R_A, ID32 = 06808, Shiny = Never, OriginalTrainerName = "10 ANIV", OriginalTrainerGender = RandS7, Language = (int)English }, // Latios

        // Journey Across America
        new(001, 70, R) { Moves = new(230,074,076,235), Method = BACD_R_A, ID32 = 00010, OriginalTrainerName = "10 ANIV", Shiny = Never, OriginalTrainerGender = RandS7, Language = (int)English }, // Bulbasaur
        new(006, 70, R) { Moves = new(017,163,082,083), Method = BACD_R_A, ID32 = 00010, OriginalTrainerName = "10 ANIV", Shiny = Never, OriginalTrainerGender = RandS7, Language = (int)English }, // Charizard
        new(009, 70, R) { Moves = new(182,240,130,056), Method = BACD_R_A, ID32 = 00010, OriginalTrainerName = "10 ANIV", Shiny = Never, OriginalTrainerGender = RandS7, Language = (int)English }, // Blastoise
        new(025, 70, R) { Moves = new(085,097,087,113), Method = BACD_R_A, ID32 = 00010, OriginalTrainerName = "10 ANIV", Shiny = Never, OriginalTrainerGender = RandS7, Language = (int)English }, // Pikachu (No Fly)
        new(065, 70, R) { Moves = new(248,347,094,271), Method = BACD_R_A, ID32 = 00010, OriginalTrainerName = "10 ANIV", Shiny = Never, OriginalTrainerGender = RandS7, Language = (int)English }, // Alakazam
        new(144, 70, R) { Moves = new(097,170,058,115), Method = BACD_R_A, ID32 = 00010, OriginalTrainerName = "10 ANIV", Shiny = Never, OriginalTrainerGender = RandS7, Language = (int)English }, // Articuno
        new(145, 70, R) { Moves = new(097,197,065,268), Method = BACD_R_A, ID32 = 00010, OriginalTrainerName = "10 ANIV", Shiny = Never, OriginalTrainerGender = RandS7, Language = (int)English }, // Zapdos
        new(146, 70, R) { Moves = new(097,203,053,219), Method = BACD_R_A, ID32 = 00010, OriginalTrainerName = "10 ANIV", Shiny = Never, OriginalTrainerGender = RandS7, Language = (int)English }, // Moltres
        new(149, 70, R) { Moves = new(097,219,017,200), Method = BACD_R_A, ID32 = 00010, OriginalTrainerName = "10 ANIV", Shiny = Never, OriginalTrainerGender = RandS7, Language = (int)English }, // Dragonite
        new(157, 70, R) { Moves = new(098,172,129,053), Method = BACD_R_A, ID32 = 00010, OriginalTrainerName = "10 ANIV", Shiny = Never, OriginalTrainerGender = RandS7, Language = (int)English }, // Typhlosion
        new(196, 70, R) { Moves = new(060,244,094,234), Method = BACD_R_A, ID32 = 00010, OriginalTrainerName = "10 ANIV", Shiny = Never, OriginalTrainerGender = RandS7, Language = (int)English }, // Espeon
        new(197, 70, R) { Moves = new(185,212,103,236), Method = BACD_R_A, ID32 = 00010, OriginalTrainerName = "10 ANIV", Shiny = Never, OriginalTrainerGender = RandS7, Language = (int)English }, // Umbreon
        new(243, 70, R) { Moves = new(098,209,115,242), Method = BACD_R_A, ID32 = 00010, OriginalTrainerName = "10 ANIV", Shiny = Never, OriginalTrainerGender = RandS7, Language = (int)English }, // Raikou
        new(244, 70, R) { Moves = new(083,023,053,207), Method = BACD_R_A, ID32 = 00010, OriginalTrainerName = "10 ANIV", Shiny = Never, OriginalTrainerGender = RandS7, Language = (int)English }, // Entei
        new(245, 70, R) { Moves = new(016,062,054,243), Method = BACD_R_A, ID32 = 00010, OriginalTrainerName = "10 ANIV", Shiny = Never, OriginalTrainerGender = RandS7, Language = (int)English }, // Suicune
        new(248, 70, R) { Moves = new(037,184,242,089), Method = BACD_R_A, ID32 = 00010, OriginalTrainerName = "10 ANIV", Shiny = Never, OriginalTrainerGender = RandS7, Language = (int)English }, // Tyranitar
        new(251, 70, R) { Moves = new(246,248,226,195), Method = BACD_R_A, ID32 = 00010, OriginalTrainerName = "10 ANIV", Shiny = Never, OriginalTrainerGender = RandS7, Language = (int)English }, // Celebi
        new(257, 70, R) { Moves = new(299,163,119,327), Method = BACD_R_A, ID32 = 00010, OriginalTrainerName = "10 ANIV", Shiny = Never, OriginalTrainerGender = RandS7, Language = (int)English }, // Blaziken
        new(359, 70, R) { Moves = new(104,163,248,195), Method = BACD_R_A, ID32 = 00010, OriginalTrainerName = "10 ANIV", Shiny = Never, OriginalTrainerGender = RandS7, Language = (int)English }, // Absol
        new(380, 70, R) { Moves = new(296,094,105,204), Method = BACD_R_A, ID32 = 00010, OriginalTrainerName = "10 ANIV", Shiny = Never, OriginalTrainerGender = RandS7, Language = (int)English }, // Latias
        new(381, 70, R) { Moves = new(295,094,105,349), Method = BACD_R_A, ID32 = 00010, OriginalTrainerName = "10 ANIV", Shiny = Never, OriginalTrainerGender = RandS7, Language = (int)English }, // Latios
    ];

    public const string PCJPEggTrainerName = "オヤＮＡＭＥ";
    private const string M2WishEggOT = "";

    private static readonly EncounterGift3[] Eggs =
    [
        // Pokémon Box -- Recipient
        new(333, 5, Gen3, true) { Moves = new(064,045,206,000), Method = BACD_U, OriginalTrainerName = "ＡＺＵＳＡ", OriginalTrainerGender = Only1 }, // Swablu Egg with False Swipe
        new(263, 5, Gen3, true) { Moves = new(033,045,039,245), Method = BACD_U, OriginalTrainerName = "ＡＺＵＳＡ", OriginalTrainerGender = Only1 }, // Zigzagoon Egg with Extreme Speed
        new(300, 5, Gen3, true) { Moves = new(045,033,039,006), Method = BACD_U, OriginalTrainerName = "ＡＺＵＳＡ", OriginalTrainerGender = Only1 }, // Skitty Egg with Pay Day
        new(172, 5, Gen3, true) { Moves = new(084,204,057,000), Method = BACD_U, OriginalTrainerName = "ＡＺＵＳＡ", OriginalTrainerGender = Only1 }, // Pichu Egg with Surf

        // PCJP - Pokémon Center 5th Anniversary Eggs (April 25 to May 18, 2003)
        new(172, 5, R, true) { Moves = new(084,204,298,000), Method = BACD_TS, OriginalTrainerName = PCJPEggTrainerName, OriginalTrainerGender = Only0, Shiny = Always }, // Pichu with Teeter Dance
        new(172, 5, R, true) { Moves = new(084,204,273,000), Method = BACD_TS, OriginalTrainerName = PCJPEggTrainerName, OriginalTrainerGender = Only0, Shiny = Always }, // Pichu with Wish
        new(172, 5, R, true) { Moves = new(084,204,298,000), Method = BACD_TA, OriginalTrainerName = PCJPEggTrainerName, OriginalTrainerGender = Only0 }, // Pichu with Teeter Dance
        new(172, 5, R, true) { Moves = new(084,204,273,000), Method = BACD_TA, OriginalTrainerName = PCJPEggTrainerName, OriginalTrainerGender = Only0 }, // Pichu with Wish
        new(280, 5, R, true) { Moves = new(045,204,000,000), Method = BACD_TA, OriginalTrainerName = PCJPEggTrainerName, OriginalTrainerGender = Only0 }, // Ralts with Charm
        new(280, 5, R, true) { Moves = new(045,273,000,000), Method = BACD_TA, OriginalTrainerName = PCJPEggTrainerName, OriginalTrainerGender = Only0 }, // Ralts with Wish
        new(359, 5, R, true) { Moves = new(010,043,180,000), Method = BACD_TA, OriginalTrainerName = PCJPEggTrainerName, OriginalTrainerGender = Only0 }, // Absol with Spite
        new(359, 5, R, true) { Moves = new(010,043,273,000), Method = BACD_TA, OriginalTrainerName = PCJPEggTrainerName, OriginalTrainerGender = Only0 }, // Absol with Wish
        new(371, 5, R, true) { Moves = new(099,044,334,000), Method = BACD_TA, OriginalTrainerName = PCJPEggTrainerName, OriginalTrainerGender = Only0 }, // Bagon with Iron Defense
        new(371, 5, R, true) { Moves = new(099,044,273,000), Method = BACD_TA, OriginalTrainerName = PCJPEggTrainerName, OriginalTrainerGender = Only0 }, // Bagon with Wish

        // Distributed event gifts for receipt in the Pokémon Center 2F call the same give egg script as the Hot Springs Wynaut.
        // Most often, the game has a VBlank interrupt between PID and IVs (potentially because the script does not lock when starting).
        // Removing the VBlank interrupt via ROM patch results in the scripts yielding a Method 1 PID/IV.
        // Specific setups & seeds can yield Method 1 or Method 4. Check method compatibility separately (based on resulting PID value and correlation type).

        // PCJP Egg Pokémon Present Eggs - Wondercard (March 21 to April 4, 2004)
        new(043, 5, FRLG, true) { Moves = new(071,073,000,000), Method = Method_2, FatefulEncounter = true, OriginalTrainerName = M2WishEggOT }, // Oddish with Leech Seed
        new(052, 5, FRLG, true) { Moves = new(010,045,080,000), Method = Method_2, FatefulEncounter = true, OriginalTrainerName = M2WishEggOT }, // Meowth with Petal Dance
        new(060, 5, FRLG, true) { Moves = new(145,186,000,000), Method = Method_2, FatefulEncounter = true, OriginalTrainerName = M2WishEggOT }, // Poliwag with Sweet Kiss
        new(069, 5, FRLG, true) { Moves = new(022,298,000,000), Method = Method_2, FatefulEncounter = true, OriginalTrainerName = M2WishEggOT }, // Bellsprout with Teeter Dance

        // PCNY Wish Eggs - Wondercard (December 16, 2004, to January 2, 2005)
        new(083, 5, FRLG, true) { Moves = new(281,273,000,000), Method = Method_2, FatefulEncounter = true, OriginalTrainerName = M2WishEggOT }, // Farfetch'd with Wish & Yawn
        new(096, 5, FRLG, true) { Moves = new(187,273,000,000), Method = Method_2, FatefulEncounter = true, OriginalTrainerName = M2WishEggOT }, // Drowzee with Wish & Belly Drum
        new(102, 5, FRLG, true) { Moves = new(230,273,000,000), Method = Method_2, FatefulEncounter = true, OriginalTrainerName = M2WishEggOT }, // Exeggcute with Wish & Sweet Scent
        new(108, 5, FRLG, true) { Moves = new(215,273,000,000), Method = Method_2, FatefulEncounter = true, OriginalTrainerName = M2WishEggOT }, // Lickitung with Wish & Heal Bell
        new(113, 5, FRLG, true) { Moves = new(230,273,000,000), Method = Method_2, FatefulEncounter = true, OriginalTrainerName = M2WishEggOT }, // Chansey with Wish & Sweet Scent
        new(115, 5, FRLG, true) { Moves = new(281,273,000,000), Method = Method_2, FatefulEncounter = true, OriginalTrainerName = M2WishEggOT }, // Kangaskhan with Wish & Yawn

        // PokéPark Eggs - Wondercard (March 12 to May 8, 2005)
        new(054, 5, EFL, true) { Moves = new(346,010,039,300), Method = Method_2, FatefulEncounter = true, OriginalTrainerName = M2WishEggOT }, // Psyduck with Mud Sport
        new(172, 5, EFL, true) { Moves = new(084,204,266,000), Method = Method_2, FatefulEncounter = true, OriginalTrainerName = M2WishEggOT }, // Pichu with Follow me
        new(174, 5, EFL, true) { Moves = new(047,204,111,321), Method = Method_2, FatefulEncounter = true, OriginalTrainerName = M2WishEggOT }, // Igglybuff with Tickle
        new(222, 5, EFL, true) { Moves = new(033,300,000,000), Method = Method_2, FatefulEncounter = true, OriginalTrainerName = M2WishEggOT }, // Corsola with Mud Sport
        new(276, 5, EFL, true) { Moves = new(064,045,116,297), Method = Method_2, FatefulEncounter = true, OriginalTrainerName = M2WishEggOT }, // Taillow with Feather Dance
        new(283, 5, EFL, true) { Moves = new(145,300,000,000), Method = Method_2, FatefulEncounter = true, OriginalTrainerName = M2WishEggOT }, // Surskit with Mud Sport
        new(293, 5, EFL, true) { Moves = new(001,253,298,000), Method = Method_2, FatefulEncounter = true, OriginalTrainerName = M2WishEggOT }, // Whismur with Teeter Dance
        new(300, 5, EFL, true) { Moves = new(045,033,039,205), Method = Method_2, FatefulEncounter = true, OriginalTrainerName = M2WishEggOT }, // Skitty with Rollout
        new(311, 5, EFL, true) { Moves = new(045,086,346,000), Method = Method_2, FatefulEncounter = true, OriginalTrainerName = M2WishEggOT }, // Plusle with Water Sport
        new(312, 5, EFL, true) { Moves = new(045,086,300,000), Method = Method_2, FatefulEncounter = true, OriginalTrainerName = M2WishEggOT }, // Minun with Mud Sport
        new(325, 5, EFL, true) { Moves = new(150,253,000,000), Method = Method_2, FatefulEncounter = true, OriginalTrainerName = M2WishEggOT }, // Spoink with Uproar
        new(327, 5, EFL, true) { Moves = new(033,253,047,000), Method = Method_2, FatefulEncounter = true, OriginalTrainerName = M2WishEggOT }, // Spinda with Sing
        new(331, 5, EFL, true) { Moves = new(040,043,071,227), Method = Method_2, FatefulEncounter = true, OriginalTrainerName = M2WishEggOT }, // Cacnea with Encore
        new(341, 5, EFL, true) { Moves = new(145,346,000,000), Method = Method_2, FatefulEncounter = true, OriginalTrainerName = M2WishEggOT }, // Corphish with Water Sport
        new(360, 5, EFL, true) { Moves = new(150,204,227,321), Method = Method_2, FatefulEncounter = true, OriginalTrainerName = M2WishEggOT }, // Wynaut with Tickle

        // PokéPark  Eggs - DS Download Play (March 12 to May 8, 2005)
        new(054, 5, R, true, 5) { Moves = new(346,010,039,300), Method = BACD_R, OriginalTrainerName = "ポケパーク", OriginalTrainerGender = Only0, ID32 = 50318 }, // Psyduck with Mud Sport
        new(172, 5, R, true, 5) { Moves = new(084,204,266,000), Method = BACD_R, OriginalTrainerName = "ポケパーク", OriginalTrainerGender = Only0, ID32 = 50318 }, // Pichu with Follow Me
        new(174, 5, R, true, 5) { Moves = new(047,204,111,321), Method = BACD_R, OriginalTrainerName = "ポケパーク", OriginalTrainerGender = Only0, ID32 = 50318 }, // Igglybuff with Tickle
        new(222, 5, R, true, 5) { Moves = new(033,300,000,000), Method = BACD_R, OriginalTrainerName = "ポケパーク", OriginalTrainerGender = Only0, ID32 = 50318 }, // Corsola with Mud Sport
        new(276, 5, R, true, 5) { Moves = new(064,045,116,297), Method = BACD_R, OriginalTrainerName = "ポケパーク", OriginalTrainerGender = Only0, ID32 = 50318 }, // Taillow with Feather Dance
        new(283, 5, R, true, 5) { Moves = new(145,300,000,000), Method = BACD_R, OriginalTrainerName = "ポケパーク", OriginalTrainerGender = Only0, ID32 = 50318 }, // Surskit with Mud Sport
        new(293, 5, R, true, 5) { Moves = new(001,253,298,000), Method = BACD_R, OriginalTrainerName = "ポケパーク", OriginalTrainerGender = Only0, ID32 = 50318 }, // Whismur with Teeter Dance
        new(300, 5, R, true, 5) { Moves = new(045,033,039,205), Method = BACD_R, OriginalTrainerName = "ポケパーク", OriginalTrainerGender = Only0, ID32 = 50318 }, // Skitty with Rollout
        new(311, 5, R, true, 5) { Moves = new(045,086,346,000), Method = BACD_R, OriginalTrainerName = "ポケパーク", OriginalTrainerGender = Only0, ID32 = 50318 }, // Plusle with Water Sport
        new(312, 5, R, true, 5) { Moves = new(045,086,300,000), Method = BACD_R, OriginalTrainerName = "ポケパーク", OriginalTrainerGender = Only0, ID32 = 50318 }, // Minun with Mud Sport
        new(325, 5, R, true, 5) { Moves = new(150,253,000,000), Method = BACD_R, OriginalTrainerName = "ポケパーク", OriginalTrainerGender = Only0, ID32 = 50318 }, // Spoink with Uproar
        new(327, 5, R, true, 5) { Moves = new(033,253,047,000), Method = BACD_R, OriginalTrainerName = "ポケパーク", OriginalTrainerGender = Only0, ID32 = 50318 }, // Spinda with Sing
        new(331, 5, R, true, 5) { Moves = new(040,043,071,227), Method = BACD_R, OriginalTrainerName = "ポケパーク", OriginalTrainerGender = Only0, ID32 = 50318 }, // Cacnea with Encore
        new(341, 5, R, true, 5) { Moves = new(145,346,000,000), Method = BACD_R, OriginalTrainerName = "ポケパーク", OriginalTrainerGender = Only0, ID32 = 50318 }, // Corphish with Water Sport
        new(360, 5, R, true, 5) { Moves = new(150,204,227,321), Method = BACD_R, OriginalTrainerName = "ポケパーク", OriginalTrainerGender = Only0, ID32 = 50318 }, // Wynaut with Tickle
    ];

    internal static readonly EncounterGift3[] Encounter_WC3 = [..Common, ..International, .. Japan, ..Eggs];

    internal static readonly EncounterGift3NY[] PCNY = EncounterGift3NY.GetArray(EncounterUtil.Get("pcny"));

    internal static readonly EncounterGift3JPN[] PCJP = Gen3PCJP.GetArray();
}
