using static PKHeX.Core.GameVersion;
using static PKHeX.Core.PIDType;
using static PKHeX.Core.Shiny;
using static PKHeX.Core.LanguageID;

namespace PKHeX.Core;

/// <summary>
/// Generation 3 <see cref="WC3"/> Gifts
/// </summary>
/// <remarks>
/// Generation 3 has a wide range of PIDIV types and other restrictions, and was never consistently stored in raw bytes.
/// Normally we'd just load the data from a binary, but without raw data... hard-code everything by hand.
/// </remarks>
internal static class EncountersWC3
{
    internal static readonly WC3[] Encounter_Event3_Special =
    [
        new(R) { Species = 385, Level = 05, ID32 = 20043, OriginalTrainerGender = 0, Method = BACD_R, OriginalTrainerName = "WISHMKR", CardTitle = "Wishmaker Jirachi", Language = (int)English },
    ];

    internal static readonly WC3[] Encounter_Event3 = Encounter_Event3_Special;

    internal static readonly WC3[] Encounter_Event3_FRLG =
    [
        // PCJP - Egg Pokémon Present Eggs (March 21 to April 4, 2004)
        new(FRLG, true) { Species = 043, IsEgg = true, Level = 05, Moves = new(071,073,000,000), Method = Method_2 }, // Oddish with Leech Seed
        new(FRLG, true) { Species = 052, IsEgg = true, Level = 05, Moves = new(010,045,080,000), Method = Method_2 }, // Meowth with Petal Dance
        new(FRLG, true) { Species = 060, IsEgg = true, Level = 05, Moves = new(145,186,000,000), Method = Method_2 }, // Poliwag with Sweet Kiss
        new(FRLG, true) { Species = 069, IsEgg = true, Level = 05, Moves = new(022,298,000,000), Method = Method_2 }, // Bellsprout with Teeter Dance

        // PCNY - Wish Eggs (December 16, 2004, to January 2, 2005)
        new(FRLG, true) { Species = 083, IsEgg = true, Level = 05, Moves = new(281,273,000,000), Method = Method_2 }, // Farfetch'd with Wish & Yawn
        new(FRLG, true) { Species = 096, IsEgg = true, Level = 05, Moves = new(187,273,000,000), Method = Method_2 }, // Drowzee with Wish & Belly Drum
        new(FRLG, true) { Species = 102, IsEgg = true, Level = 05, Moves = new(230,273,000,000), Method = Method_2 }, // Exeggcute with Wish & Sweet Scent
        new(FRLG, true) { Species = 108, IsEgg = true, Level = 05, Moves = new(215,273,000,000), Method = Method_2 }, // Lickitung with Wish & Heal Bell
        new(FRLG, true) { Species = 113, IsEgg = true, Level = 05, Moves = new(230,273,000,000), Method = Method_2 }, // Chansey with Wish & Sweet Scent
        new(FRLG, true) { Species = 115, IsEgg = true, Level = 05, Moves = new(281,273,000,000), Method = Method_2 }, // Kangaskhan with Wish & Yawn

        // PokePark Eggs - Wondercard
        new(FRLG, true) { Species = 054, IsEgg = true, Level = 05, Moves = new(346,010,039,300), Method = Method_2 }, // Psyduck with Mud Sport
        new(FRLG, true) { Species = 172, IsEgg = true, Level = 05, Moves = new(084,204,266,000), Method = Method_2 }, // Pichu with Follow me
        new(FRLG, true) { Species = 174, IsEgg = true, Level = 05, Moves = new(047,204,111,321), Method = Method_2 }, // Igglybuff with Tickle
        new(FRLG, true) { Species = 222, IsEgg = true, Level = 05, Moves = new(033,300,000,000), Method = Method_2 }, // Corsola with Mud Sport
        new(FRLG, true) { Species = 276, IsEgg = true, Level = 05, Moves = new(064,045,116,297), Method = Method_2 }, // Taillow with Feather Dance
        new(FRLG, true) { Species = 283, IsEgg = true, Level = 05, Moves = new(145,300,000,000), Method = Method_2 }, // Surskit with Mud Sport
        new(FRLG, true) { Species = 293, IsEgg = true, Level = 05, Moves = new(001,253,298,000), Method = Method_2 }, // Whismur with Teeter Dance
        new(FRLG, true) { Species = 300, IsEgg = true, Level = 05, Moves = new(045,033,039,205), Method = Method_2 }, // Skitty with Rollout
        new(FRLG, true) { Species = 311, IsEgg = true, Level = 05, Moves = new(045,086,346,000), Method = Method_2 }, // Plusle with Water Sport
        new(FRLG, true) { Species = 312, IsEgg = true, Level = 05, Moves = new(045,086,300,000), Method = Method_2 }, // Minun with Mud Sport
        new(FRLG, true) { Species = 325, IsEgg = true, Level = 05, Moves = new(150,253,000,000), Method = Method_2 }, // Spoink with Uproar
        new(FRLG, true) { Species = 327, IsEgg = true, Level = 05, Moves = new(033,253,047,000), Method = Method_2 }, // Spinda with Sing
        new(FRLG, true) { Species = 331, IsEgg = true, Level = 05, Moves = new(040,043,071,227), Method = Method_2 }, // Cacnea with Encore
        new(FRLG, true) { Species = 341, IsEgg = true, Level = 05, Moves = new(145,346,000,000), Method = Method_2 }, // Corphish with Water Sport
        new(FRLG, true) { Species = 360, IsEgg = true, Level = 05, Moves = new(150,204,227,321), Method = Method_2 }, // Wynaut with Tickle
    ];

    internal static readonly WC3[] Encounter_Event3_RS =
    [
        // PCJP - Pokémon Center 5th Anniversary Eggs (April 25 to May 18, 2003)
        new(R) { Species = 172, IsEgg = true, Level = 05, OriginalTrainerName = "オヤＮＡＭＥ", Moves = new(084,204,298,000), Method = BACD_R }, // Pichu with Teeter Dance
        new(R) { Species = 172, IsEgg = true, Level = 05, OriginalTrainerName = "オヤＮＡＭＥ", Moves = new(084,204,273,000), Method = BACD_R }, // Pichu with Wish
        new(R) { Species = 172, IsEgg = true, Level = 05, OriginalTrainerName = "オヤＮＡＭＥ", Moves = new(084,204,298,000), Method = BACD_R_S }, // Pichu with Teeter Dance
        new(R) { Species = 172, IsEgg = true, Level = 05, OriginalTrainerName = "オヤＮＡＭＥ", Moves = new(084,204,273,000), Method = BACD_R_S }, // Pichu with Wish
        new(R) { Species = 280, IsEgg = true, Level = 05, OriginalTrainerName = "オヤＮＡＭＥ", Moves = new(045,204,000,000), Method = BACD_R }, // Ralts with Charm
        new(R) { Species = 280, IsEgg = true, Level = 05, OriginalTrainerName = "オヤＮＡＭＥ", Moves = new(045,273,000,000), Method = BACD_R }, // Ralts with Wish
        new(R) { Species = 359, IsEgg = true, Level = 05, OriginalTrainerName = "オヤＮＡＭＥ", Moves = new(010,043,180,000), Method = BACD_R }, // Absol with Spite
        new(R) { Species = 359, IsEgg = true, Level = 05, OriginalTrainerName = "オヤＮＡＭＥ", Moves = new(010,043,273,000), Method = BACD_R }, // Absol with Wish
        new(R) { Species = 371, IsEgg = true, Level = 05, OriginalTrainerName = "オヤＮＡＭＥ", Moves = new(099,044,334,000), Method = BACD_R }, // Bagon with Iron Defense
        new(R) { Species = 371, IsEgg = true, Level = 05, OriginalTrainerName = "オヤＮＡＭＥ", Moves = new(099,044,273,000), Method = BACD_R }, // Bagon with Wish

        // Negai Boshi Jirachi
        new(R) { Species = 385, Level = 05, ID32 = 30719, OriginalTrainerGender = 0, OriginalTrainerName = "ネガイボシ", Method = BACD_R, Language = (int)Japanese, Shiny = Never },
        new(RS) { Species = 385, Level = 05, ID32 = 30719, OriginalTrainerName = "ネガイボシ", Method = BACD_U_AX, Language = (int)Japanese, Shiny = Never },

        // Berry Glitch Fix
        // PCJP - (December 29, 2003, to March 31, 2004)
        new(S) { Species = 263, Level = 5, Language = (int)Japanese, Method = BACD_R_S, ID32 = 21121, OriginalTrainerName = "ルビー", OriginalTrainerGender = 1, Shiny = Always },
        new(S) { Species = 263, Level = 5, Language = (int)Japanese, Method = BACD_R_S, ID32 = 21121, OriginalTrainerName = "サファイア", OriginalTrainerGender = 0, Shiny = Always },

        // EBGames/GameStop (March 1, 2004, to April 22, 2007), also via multi-game discs
        new(S) { Species = 263, Level = 5, Language = (int)English, Method = BACD_R_S, ID32 = 30317, OriginalTrainerName = "RUBY", OriginalTrainerGender = 1 },
        new(S) { Species = 263, Level = 5, Language = (int)English, Method = BACD_R_S, ID32 = 30317, OriginalTrainerName = "SAPHIRE", OriginalTrainerGender = 0 },

        // Channel Jirachi
        new(RS) { Species = 385, Level = 5, Method = Channel, TID16 = 40122, OriginalTrainerGender = 3, OriginalTrainerName = "CHANNEL", CardTitle = "Channel Jirachi", MetLevel = 0 },

        // Aura Mew
        new(R, true) { Species = 151, Level = 10, Language = (int)English, Method = BACD_R, ID32 = 20078, OriginalTrainerName = "Aura", Shiny = Never }, // Mew
        new(R, true) { Species = 151, Level = 10, Language = (int)French,  Method = BACD_R, ID32 = 20078, OriginalTrainerName = "Aura", Shiny = Never }, // Mew
        new(R, true) { Species = 151, Level = 10, Language = (int)Italian, Method = BACD_R, ID32 = 20078, OriginalTrainerName = "Aura", Shiny = Never }, // Mew
        new(R, true) { Species = 151, Level = 10, Language = (int)German,  Method = BACD_R, ID32 = 20078, OriginalTrainerName = "Aura", Shiny = Never }, // Mew
        new(R, true) { Species = 151, Level = 10, Language = (int)Spanish, Method = BACD_R, ID32 = 20078, OriginalTrainerName = "Aura", Shiny = Never }, // Mew

        // English Events
        new(R) { Species = 006, Level = 70, Moves = new(017,163,082,083), Language = (int)English, Method = BACD_R, ID32 = 06227, OriginalTrainerName = "10ANNIV", Shiny = Never }, // Charizard
        new(R) { Species = 025, Level = 70, Moves = new(085,097,087,113), Language = (int)English, Method = BACD_R, ID32 = 06227, OriginalTrainerName = "10ANNIV", Shiny = Never }, // Pikachu
        new(R) { Species = 144, Level = 70, Moves = new(097,170,058,115), Language = (int)English, Method = BACD_R, ID32 = 06227, OriginalTrainerName = "10ANNIV", Shiny = Never }, // Articuno
        new(R) { Species = 243, Level = 70, Moves = new(098,209,115,242), Language = (int)English, Method = BACD_R, ID32 = 06227, OriginalTrainerName = "10ANNIV", Shiny = Never }, // Raikou
        new(R) { Species = 244, Level = 70, Moves = new(083,023,053,207), Language = (int)English, Method = BACD_R, ID32 = 06227, OriginalTrainerName = "10ANNIV", Shiny = Never }, // Entei
        new(R) { Species = 245, Level = 70, Moves = new(016,062,054,243), Language = (int)English, Method = BACD_R, ID32 = 06227, OriginalTrainerName = "10ANNIV", Shiny = Never }, // Suicune
        new(R) { Species = 249, Level = 70, Moves = new(105,056,240,129), Language = (int)English, Method = BACD_R, ID32 = 06227, OriginalTrainerName = "10ANNIV", Shiny = Never }, // Lugia
        new(R) { Species = 250, Level = 70, Moves = new(105,126,241,129), Language = (int)English, Method = BACD_R, ID32 = 06227, OriginalTrainerName = "10ANNIV", Shiny = Never }, // Ho-Oh
        new(R) { Species = 380, Level = 70, Moves = new(296,094,105,204), Language = (int)English, Method = BACD_R, ID32 = 06227, OriginalTrainerName = "10ANNIV", Shiny = Never }, // Latias
        new(R) { Species = 381, Level = 70, Moves = new(295,094,105,349), Language = (int)English, Method = BACD_R, ID32 = 06227, OriginalTrainerName = "10ANNIV", Shiny = Never }, // Latios

        // French
        new(R) { Species = 006, Level = 70, Moves = new(017,163,082,083), Language = (int)French, Method = BACD_R, ID32 = 06227, OriginalTrainerName = "10ANNIV", Shiny = Never }, // Charizard
        new(R) { Species = 025, Level = 70, Moves = new(085,097,087,113), Language = (int)French, Method = BACD_R, ID32 = 06227, OriginalTrainerName = "10ANNIV", Shiny = Never }, // Pikachu
        new(R) { Species = 144, Level = 70, Moves = new(097,170,058,115), Language = (int)French, Method = BACD_R, ID32 = 06227, OriginalTrainerName = "10ANNIV", Shiny = Never }, // Articuno
        new(R) { Species = 243, Level = 70, Moves = new(098,209,115,242), Language = (int)French, Method = BACD_R, ID32 = 06227, OriginalTrainerName = "10ANNIV", Shiny = Never }, // Raikou
        new(R) { Species = 244, Level = 70, Moves = new(083,023,053,207), Language = (int)French, Method = BACD_R, ID32 = 06227, OriginalTrainerName = "10ANNIV", Shiny = Never }, // Entei
        new(R) { Species = 245, Level = 70, Moves = new(016,062,054,243), Language = (int)French, Method = BACD_R, ID32 = 06227, OriginalTrainerName = "10ANNIV", Shiny = Never }, // Suicune
        new(R) { Species = 249, Level = 70, Moves = new(105,056,240,129), Language = (int)French, Method = BACD_R, ID32 = 06227, OriginalTrainerName = "10ANNIV", Shiny = Never }, // Lugia
        new(R) { Species = 250, Level = 70, Moves = new(105,126,241,129), Language = (int)French, Method = BACD_R, ID32 = 06227, OriginalTrainerName = "10ANNIV", Shiny = Never }, // Ho-Oh
        new(R) { Species = 380, Level = 70, Moves = new(296,094,105,204), Language = (int)French, Method = BACD_R, ID32 = 06227, OriginalTrainerName = "10ANNIV", Shiny = Never }, // Latias
        new(R) { Species = 381, Level = 70, Moves = new(295,094,105,349), Language = (int)French, Method = BACD_R, ID32 = 06227, OriginalTrainerName = "10ANNIV", Shiny = Never }, // Latios

        // Italian
        new(R) { Species = 006, Level = 70, Moves = new(017,163,082,083), Language = (int)Italian, Method = BACD_R, ID32 = 06227, OriginalTrainerName = "10ANNI", Shiny = Never }, // Charizard
        new(R) { Species = 025, Level = 70, Moves = new(085,097,087,113), Language = (int)Italian, Method = BACD_R, ID32 = 06227, OriginalTrainerName = "10ANNI", Shiny = Never }, // Pikachu
        new(R) { Species = 144, Level = 70, Moves = new(097,170,058,115), Language = (int)Italian, Method = BACD_R, ID32 = 06227, OriginalTrainerName = "10ANNI", Shiny = Never }, // Articuno
        new(R) { Species = 243, Level = 70, Moves = new(098,209,115,242), Language = (int)Italian, Method = BACD_R, ID32 = 06227, OriginalTrainerName = "10ANNI", Shiny = Never }, // Raikou
        new(R) { Species = 244, Level = 70, Moves = new(083,023,053,207), Language = (int)Italian, Method = BACD_R, ID32 = 06227, OriginalTrainerName = "10ANNI", Shiny = Never }, // Entei
        new(R) { Species = 245, Level = 70, Moves = new(016,062,054,243), Language = (int)Italian, Method = BACD_R, ID32 = 06227, OriginalTrainerName = "10ANNI", Shiny = Never }, // Suicune
        new(R) { Species = 249, Level = 70, Moves = new(105,056,240,129), Language = (int)Italian, Method = BACD_R, ID32 = 06227, OriginalTrainerName = "10ANNI", Shiny = Never }, // Lugia
        new(R) { Species = 250, Level = 70, Moves = new(105,126,241,129), Language = (int)Italian, Method = BACD_R, ID32 = 06227, OriginalTrainerName = "10ANNI", Shiny = Never }, // Ho-Oh
        new(R) { Species = 380, Level = 70, Moves = new(296,094,105,204), Language = (int)Italian, Method = BACD_R, ID32 = 06227, OriginalTrainerName = "10ANNI", Shiny = Never }, // Latias
        new(R) { Species = 381, Level = 70, Moves = new(295,094,105,349), Language = (int)Italian, Method = BACD_R, ID32 = 06227, OriginalTrainerName = "10ANNI", Shiny = Never }, // Latios

        // German
        new(R) { Species = 006, Level = 70, Moves = new(017,163,082,083), Language = (int)German, Method = BACD_R, ID32 = 06227, OriginalTrainerName = "10JAHRE", Shiny = Never }, // Charizard
        new(R) { Species = 025, Level = 70, Moves = new(085,097,087,113), Language = (int)German, Method = BACD_R, ID32 = 06227, OriginalTrainerName = "10JAHRE", Shiny = Never }, // Pikachu
        new(R) { Species = 144, Level = 70, Moves = new(097,170,058,115), Language = (int)German, Method = BACD_R, ID32 = 06227, OriginalTrainerName = "10JAHRE", Shiny = Never }, // Articuno
        new(R) { Species = 243, Level = 70, Moves = new(098,209,115,242), Language = (int)German, Method = BACD_R, ID32 = 06227, OriginalTrainerName = "10JAHRE", Shiny = Never }, // Raikou
        new(R) { Species = 244, Level = 70, Moves = new(083,023,053,207), Language = (int)German, Method = BACD_R, ID32 = 06227, OriginalTrainerName = "10JAHRE", Shiny = Never }, // Entei
        new(R) { Species = 245, Level = 70, Moves = new(016,062,054,243), Language = (int)German, Method = BACD_R, ID32 = 06227, OriginalTrainerName = "10JAHRE", Shiny = Never }, // Suicune
        new(R) { Species = 249, Level = 70, Moves = new(105,056,240,129), Language = (int)German, Method = BACD_R, ID32 = 06227, OriginalTrainerName = "10JAHRE", Shiny = Never }, // Lugia
        new(R) { Species = 250, Level = 70, Moves = new(105,126,241,129), Language = (int)German, Method = BACD_R, ID32 = 06227, OriginalTrainerName = "10JAHRE", Shiny = Never }, // Ho-Oh
        new(R) { Species = 380, Level = 70, Moves = new(296,094,105,204), Language = (int)German, Method = BACD_R, ID32 = 06227, OriginalTrainerName = "10JAHRE", Shiny = Never }, // Latias
        new(R) { Species = 381, Level = 70, Moves = new(295,094,105,349), Language = (int)German, Method = BACD_R, ID32 = 06227, OriginalTrainerName = "10JAHRE", Shiny = Never }, // Latios

        // Spanish
        new(R) { Species = 006, Level = 70, Moves = new(017,163,082,083), Language = (int)Spanish, Method = BACD_R, ID32 = 06227, OriginalTrainerName = "10ANIV", Shiny = Never }, // Charizard
        new(R) { Species = 025, Level = 70, Moves = new(085,097,087,113), Language = (int)Spanish, Method = BACD_R, ID32 = 06227, OriginalTrainerName = "10ANIV", Shiny = Never }, // Pikachu
        new(R) { Species = 144, Level = 70, Moves = new(097,170,058,115), Language = (int)Spanish, Method = BACD_R, ID32 = 06227, OriginalTrainerName = "10ANIV", Shiny = Never }, // Articuno
        new(R) { Species = 243, Level = 70, Moves = new(098,209,115,242), Language = (int)Spanish, Method = BACD_R, ID32 = 06227, OriginalTrainerName = "10ANIV", Shiny = Never }, // Raikou
        new(R) { Species = 244, Level = 70, Moves = new(083,023,053,207), Language = (int)Spanish, Method = BACD_R, ID32 = 06227, OriginalTrainerName = "10ANIV", Shiny = Never }, // Entei
        new(R) { Species = 245, Level = 70, Moves = new(016,062,054,243), Language = (int)Spanish, Method = BACD_R, ID32 = 06227, OriginalTrainerName = "10ANIV", Shiny = Never }, // Suicune
        new(R) { Species = 249, Level = 70, Moves = new(105,056,240,129), Language = (int)Spanish, Method = BACD_R, ID32 = 06227, OriginalTrainerName = "10ANIV", Shiny = Never }, // Lugia
        new(R) { Species = 250, Level = 70, Moves = new(105,126,241,129), Language = (int)Spanish, Method = BACD_R, ID32 = 06227, OriginalTrainerName = "10ANIV", Shiny = Never }, // Ho-Oh
        new(R) { Species = 380, Level = 70, Moves = new(296,094,105,204), Language = (int)Spanish, Method = BACD_R, ID32 = 06227, OriginalTrainerName = "10ANIV", Shiny = Never }, // Latias
        new(R) { Species = 381, Level = 70, Moves = new(295,094,105,349), Language = (int)Spanish, Method = BACD_R, ID32 = 06227, OriginalTrainerName = "10ANIV", Shiny = Never }, // Latios

        new(R) { Species = 375, Level = 30, Moves = new(036,093,232,287), Language = (int)English, Method = BACD_R, ID32 = 02005, OriginalTrainerName = "ROCKS", OriginalTrainerGender = 0, RibbonNational = true, Shiny = Never }, // Metang
        new(R, true) { Species = 386, Level = 70,  Moves = new(322,105,354,063), Language = (int)English, Method = BACD_R, ID32 = 28606, OriginalTrainerName = "DOEL", Shiny = Never }, // Deoxys
        new(R, true) { Species = 386, Level = 70,  Moves = new(322,105,354,063), Language = (int)English, Method = BACD_R, ID32 = 00010, OriginalTrainerName = "SPACE C", Shiny = Never }, // Deoxys
        new(R, true) { Species = 151, Level = 10,  Language = (int)English, Method = BACD_U, ID32 = 06930, OriginalTrainerName = "MYSTRY", Shiny = Never }, // Mew
        new(R, true) { Species = 151, Level = 10,  Language = (int)English, Method = BACD_R, ID32 = 06930, OriginalTrainerName = "MYSTRY", Shiny = Never }, // Mew

        // Party of the Decade
        new(R) { Species = 001, Level = 70, Moves = new(230,074,076,235), Language = (int)English, Method = BACD_R, ID32 = 06808, OriginalTrainerName = "10 ANIV", Shiny = Never }, // Bulbasaur
        new(R) { Species = 006, Level = 70, Moves = new(017,163,082,083), Language = (int)English, Method = BACD_R, ID32 = 06808, OriginalTrainerName = "10 ANIV", Shiny = Never }, // Charizard
        new(R) { Species = 009, Level = 70, Moves = new(182,240,130,056), Language = (int)English, Method = BACD_R, ID32 = 06808, OriginalTrainerName = "10 ANIV", Shiny = Never }, // Blastoise
        new(R) { Species = 025, Level = 70, Moves = new(085,087,113,019), Language = (int)English, Method = BACD_R, ID32 = 06808, OriginalTrainerName = "10 ANIV", HeldItem = 202, Shiny = Never }, // Pikachu (Fly)
        new(R) { Species = 065, Level = 70, Moves = new(248,347,094,271), Language = (int)English, Method = BACD_R, ID32 = 06808, OriginalTrainerName = "10 ANIV", Shiny = Never }, // Alakazam
        new(R) { Species = 144, Level = 70, Moves = new(097,170,058,115), Language = (int)English, Method = BACD_R, ID32 = 06808, OriginalTrainerName = "10 ANIV", Shiny = Never }, // Articuno
        new(R) { Species = 145, Level = 70, Moves = new(097,197,065,268), Language = (int)English, Method = BACD_R, ID32 = 06808, OriginalTrainerName = "10 ANIV", Shiny = Never }, // Zapdos
        new(R) { Species = 146, Level = 70, Moves = new(097,203,053,219), Language = (int)English, Method = BACD_R, ID32 = 06808, OriginalTrainerName = "10 ANIV", Shiny = Never }, // Moltres
        new(R) { Species = 149, Level = 70, Moves = new(097,219,017,200), Language = (int)English, Method = BACD_R, ID32 = 06808, OriginalTrainerName = "10 ANIV", Shiny = Never }, // Dragonite
        new(R) { Species = 157, Level = 70, Moves = new(098,172,129,053), Language = (int)English, Method = BACD_R, ID32 = 06808, OriginalTrainerName = "10 ANIV", Shiny = Never }, // Typhlosion
        new(R) { Species = 196, Level = 70, Moves = new(060,244,094,234), Language = (int)English, Method = BACD_R, ID32 = 06808, OriginalTrainerName = "10 ANIV", Shiny = Never }, // Espeon
        new(R) { Species = 197, Level = 70, Moves = new(185,212,103,236), Language = (int)English, Method = BACD_R, ID32 = 06808, OriginalTrainerName = "10 ANIV", Shiny = Never }, // Umbreon
        new(R) { Species = 243, Level = 70, Moves = new(098,209,115,242), Language = (int)English, Method = BACD_R, ID32 = 06808, OriginalTrainerName = "10 ANIV", Shiny = Never }, // Raikou
        new(R) { Species = 244, Level = 70, Moves = new(083,023,053,207), Language = (int)English, Method = BACD_R, ID32 = 06808, OriginalTrainerName = "10 ANIV", Shiny = Never }, // Entei
        new(R) { Species = 245, Level = 70, Moves = new(016,062,054,243), Language = (int)English, Method = BACD_R, ID32 = 06808, OriginalTrainerName = "10 ANIV", Shiny = Never }, // Suicune
        new(R) { Species = 248, Level = 70, Moves = new(037,184,242,089), Language = (int)English, Method = BACD_R, ID32 = 06808, OriginalTrainerName = "10 ANIV", Shiny = Never }, // Tyranitar
        new(R) { Species = 257, Level = 70, Moves = new(299,163,119,327), Language = (int)English, Method = BACD_R, ID32 = 06808, OriginalTrainerName = "10 ANIV", Shiny = Never }, // Blaziken
        new(R) { Species = 359, Level = 70, Moves = new(104,163,248,195), Language = (int)English, Method = BACD_R, ID32 = 06808, OriginalTrainerName = "10 ANIV", Shiny = Never }, // Absol
        new(R) { Species = 380, Level = 70, Moves = new(296,094,105,204), Language = (int)English, Method = BACD_R, ID32 = 06808, OriginalTrainerName = "10 ANIV", HeldItem = 191, Shiny = Never }, // Latias
        new(R) { Species = 381, Level = 70, Moves = new(295,094,105,349), Language = (int)English, Method = BACD_R, ID32 = 06808, OriginalTrainerName = "10 ANIV", HeldItem = 191, Shiny = Never }, // Latios

        // Journey Across America
        new(R) { Species = 001, Level = 70, Moves = new(230,074,076,235), Language = (int)English, Method = BACD_R, ID32 = 00010, OriginalTrainerName = "10 ANIV", Shiny = Never }, // Bulbasaur
        new(R) { Species = 006, Level = 70, Moves = new(017,163,082,083), Language = (int)English, Method = BACD_R, ID32 = 00010, OriginalTrainerName = "10 ANIV", Shiny = Never }, // Charizard
        new(R) { Species = 009, Level = 70, Moves = new(182,240,130,056), Language = (int)English, Method = BACD_R, ID32 = 00010, OriginalTrainerName = "10 ANIV", Shiny = Never }, // Blastoise
        new(R) { Species = 025, Level = 70, Moves = new(085,097,087,113), Language = (int)English, Method = BACD_R, ID32 = 00010, OriginalTrainerName = "10 ANIV", HeldItem = 202, Shiny = Never }, // Pikachu (No Fly)
        new(R) { Species = 065, Level = 70, Moves = new(248,347,094,271), Language = (int)English, Method = BACD_R, ID32 = 00010, OriginalTrainerName = "10 ANIV", Shiny = Never }, // Alakazam
        new(R) { Species = 144, Level = 70, Moves = new(097,170,058,115), Language = (int)English, Method = BACD_R, ID32 = 00010, OriginalTrainerName = "10 ANIV", Shiny = Never }, // Articuno
        new(R) { Species = 145, Level = 70, Moves = new(097,197,065,268), Language = (int)English, Method = BACD_R, ID32 = 00010, OriginalTrainerName = "10 ANIV", Shiny = Never }, // Zapdos
        new(R) { Species = 146, Level = 70, Moves = new(097,203,053,219), Language = (int)English, Method = BACD_R, ID32 = 00010, OriginalTrainerName = "10 ANIV", Shiny = Never }, // Moltres
        new(R) { Species = 149, Level = 70, Moves = new(097,219,017,200), Language = (int)English, Method = BACD_R, ID32 = 00010, OriginalTrainerName = "10 ANIV", Shiny = Never }, // Dragonite
        new(R) { Species = 157, Level = 70, Moves = new(098,172,129,053), Language = (int)English, Method = BACD_R, ID32 = 00010, OriginalTrainerName = "10 ANIV", Shiny = Never }, // Typhlosion
        new(R) { Species = 196, Level = 70, Moves = new(060,244,094,234), Language = (int)English, Method = BACD_R, ID32 = 00010, OriginalTrainerName = "10 ANIV", Shiny = Never }, // Espeon
        new(R) { Species = 197, Level = 70, Moves = new(185,212,103,236), Language = (int)English, Method = BACD_R, ID32 = 00010, OriginalTrainerName = "10 ANIV", Shiny = Never }, // Umbreon
        new(R) { Species = 243, Level = 70, Moves = new(098,209,115,242), Language = (int)English, Method = BACD_R, ID32 = 00010, OriginalTrainerName = "10 ANIV", Shiny = Never }, // Raikou
        new(R) { Species = 244, Level = 70, Moves = new(083,023,053,207), Language = (int)English, Method = BACD_R, ID32 = 00010, OriginalTrainerName = "10 ANIV", Shiny = Never }, // Entei
        new(R) { Species = 245, Level = 70, Moves = new(016,062,054,243), Language = (int)English, Method = BACD_R, ID32 = 00010, OriginalTrainerName = "10 ANIV", Shiny = Never }, // Suicune
        new(R) { Species = 248, Level = 70, Moves = new(037,184,242,089), Language = (int)English, Method = BACD_R, ID32 = 00010, OriginalTrainerName = "10 ANIV", Shiny = Never }, // Tyranitar
        new(R) { Species = 251, Level = 70, Moves = new(246,248,226,195), Language = (int)English, Method = BACD_R, ID32 = 00010, OriginalTrainerName = "10 ANIV", Shiny = Never }, // Celebi
        new(R) { Species = 257, Level = 70, Moves = new(299,163,119,327), Language = (int)English, Method = BACD_R, ID32 = 00010, OriginalTrainerName = "10 ANIV", Shiny = Never }, // Blaziken
        new(R) { Species = 359, Level = 70, Moves = new(104,163,248,195), Language = (int)English, Method = BACD_R, ID32 = 00010, OriginalTrainerName = "10 ANIV", Shiny = Never }, // Absol
        new(R) { Species = 380, Level = 70, Moves = new(296,094,105,204), Language = (int)English, Method = BACD_R, ID32 = 00010, OriginalTrainerName = "10 ANIV", HeldItem = 191, Shiny = Never }, // Latias
        new(R) { Species = 381, Level = 70, Moves = new(295,094,105,349), Language = (int)English, Method = BACD_R, ID32 = 00010, OriginalTrainerName = "10 ANIV", HeldItem = 191, Shiny = Never }, // Latios
    ];

    internal static readonly WC3[] Encounter_Event3_Common =
    [
        // Pokémon Box -- RSE Recipient
        new(RSE) { Species = 333, IsEgg = true, Level = 05, Moves = new(064,045,206,000), Method = BACD_U, OriginalTrainerGender = 1, OriginalTrainerName = "ＡＺＵＳＡ" }, // Swablu Egg with False Swipe
        new(RSE) { Species = 263, IsEgg = true, Level = 05, Moves = new(033,045,039,245), Method = BACD_U, OriginalTrainerGender = 1, OriginalTrainerName = "ＡＺＵＳＡ" }, // Zigzagoon Egg with Extreme Speed
        new(RSE) { Species = 300, IsEgg = true, Level = 05, Moves = new(045,033,039,006), Method = BACD_U, OriginalTrainerGender = 1, OriginalTrainerName = "ＡＺＵＳＡ" }, // Skitty Egg with Pay Day
        new(RSE) { Species = 172, IsEgg = true, Level = 05, Moves = new(084,204,057,000), Method = BACD_U, OriginalTrainerGender = 1, OriginalTrainerName = "ＡＺＵＳＡ" }, // Pichu Egg with Surf
        // Pokémon Box -- FRLG Recipient
        new(FRLG) { Species = 333, IsEgg = true, Level = 05, Moves = new(064,045,206,000), Method = BACD_U, OriginalTrainerGender = 1, OriginalTrainerName = "ＡＺＵＳＡ" }, // Swablu Egg with False Swipe
        new(FRLG) { Species = 263, IsEgg = true, Level = 05, Moves = new(033,045,039,245), Method = BACD_U, OriginalTrainerGender = 1, OriginalTrainerName = "ＡＺＵＳＡ" }, // Zigzagoon Egg with Extreme Speed
        new(FRLG) { Species = 300, IsEgg = true, Level = 05, Moves = new(045,033,039,006), Method = BACD_U, OriginalTrainerGender = 1, OriginalTrainerName = "ＡＺＵＳＡ" }, // Skitty Egg with Pay Day
        new(FRLG) { Species = 172, IsEgg = true, Level = 05, Moves = new(084,204,057,000), Method = BACD_U, OriginalTrainerGender = 1, OriginalTrainerName = "ＡＺＵＳＡ" }, // Pichu Egg with Surf

        // PokePark Eggs - DS Download Play
        new(R) { Species = 054, IsEgg = true, Level = 05, MetLevel = 05, TID16 = 50318, OriginalTrainerGender = 0, OriginalTrainerName = "ポケパーク", Moves = new(300), Method = BACD_R }, // Psyduck with Mud Sport
        new(R) { Species = 172, IsEgg = true, Level = 05, MetLevel = 05, TID16 = 50318, OriginalTrainerGender = 0, OriginalTrainerName = "ポケパーク", Moves = new(266), Method = BACD_R }, // Pichu with Follow Me
        new(R) { Species = 174, IsEgg = true, Level = 05, MetLevel = 05, TID16 = 50318, OriginalTrainerGender = 0, OriginalTrainerName = "ポケパーク", Moves = new(321), Method = BACD_R }, // Igglybuff with Tickle
        new(R) { Species = 222, IsEgg = true, Level = 05, MetLevel = 05, TID16 = 50318, OriginalTrainerGender = 0, OriginalTrainerName = "ポケパーク", Moves = new(300), Method = BACD_R }, // Corsola with Mud Sport
        new(R) { Species = 276, IsEgg = true, Level = 05, MetLevel = 05, TID16 = 50318, OriginalTrainerGender = 0, OriginalTrainerName = "ポケパーク", Moves = new(297), Method = BACD_R }, // Taillow with Feather Dance
        new(R) { Species = 283, IsEgg = true, Level = 05, MetLevel = 05, TID16 = 50318, OriginalTrainerGender = 0, OriginalTrainerName = "ポケパーク", Moves = new(300), Method = BACD_R }, // Surskit with Mud Sport
        new(R) { Species = 293, IsEgg = true, Level = 05, MetLevel = 05, TID16 = 50318, OriginalTrainerGender = 0, OriginalTrainerName = "ポケパーク", Moves = new(298), Method = BACD_R }, // Whismur with Teeter Dance
        new(R) { Species = 300, IsEgg = true, Level = 05, MetLevel = 05, TID16 = 50318, OriginalTrainerGender = 0, OriginalTrainerName = "ポケパーク", Moves = new(205), Method = BACD_R }, // Skitty with Rollout
        new(R) { Species = 311, IsEgg = true, Level = 05, MetLevel = 05, TID16 = 50318, OriginalTrainerGender = 0, OriginalTrainerName = "ポケパーク", Moves = new(346), Method = BACD_R }, // Plusle with Water Sport
        new(R) { Species = 312, IsEgg = true, Level = 05, MetLevel = 05, TID16 = 50318, OriginalTrainerGender = 0, OriginalTrainerName = "ポケパーク", Moves = new(300), Method = BACD_R }, // Minun with Mud Sport
        new(R) { Species = 325, IsEgg = true, Level = 05, MetLevel = 05, TID16 = 50318, OriginalTrainerGender = 0, OriginalTrainerName = "ポケパーク", Moves = new(253), Method = BACD_R }, // Spoink with Uproar
        new(R) { Species = 327, IsEgg = true, Level = 05, MetLevel = 05, TID16 = 50318, OriginalTrainerGender = 0, OriginalTrainerName = "ポケパーク", Moves = new(047), Method = BACD_R }, // Spinda with Sing
        new(R) { Species = 331, IsEgg = true, Level = 05, MetLevel = 05, TID16 = 50318, OriginalTrainerGender = 0, OriginalTrainerName = "ポケパーク", Moves = new(227), Method = BACD_R }, // Cacnea with Encore
        new(R) { Species = 341, IsEgg = true, Level = 05, MetLevel = 05, TID16 = 50318, OriginalTrainerGender = 0, OriginalTrainerName = "ポケパーク", Moves = new(346), Method = BACD_R }, // Corphish with Water Sport
        new(R) { Species = 360, IsEgg = true, Level = 05, MetLevel = 05, TID16 = 50318, OriginalTrainerGender = 0, OriginalTrainerName = "ポケパーク", Moves = new(321), Method = BACD_R }, // Wynaut with Tickle
    ];

    internal static readonly WC3[] Encounter_WC3 = [..Encounter_Event3, ..Encounter_Event3_RS, ..Encounter_Event3_FRLG, ..Encounter_Event3_Common];
}
