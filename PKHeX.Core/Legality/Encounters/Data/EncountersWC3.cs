using System.Linq;

namespace PKHeX.Core
{
    internal static class EncountersWC3
    {
        internal static readonly MysteryGift[] Encounter_Event3 =
        {
            new WC3 { Species = 251, Level = 10, TID = 31121, OT_Gender = 1, OT_Name = "アゲト", CardTitle = "Agate Celebi", Method = PIDType.CXD, Shiny = false, Language = 1 },
            new WC3 { Species = 025, Level = 10, TID = 31121, OT_Gender = 0, OT_Name = "コロシアム", CardTitle = "Colosseum Pikachu", Method = PIDType.CXD, Shiny = false, Language = 1 },

            new WC3 { Species = 385, Level = 05, TID = 20043, OT_Gender = 0, OT_Name = "WISHMKR", CardTitle = "Wishmaker Jirachi", Method = PIDType.BACD_R, Language = 2 },
            new WC3 { Species = 251, Level = 10, TID = 31121, OT_Gender = 1, OT_Name = "AGATE", CardTitle = "Agate Celebi", Method = PIDType.CXD, Shiny = false, Language = 2, NotDistributed = true  },
            new WC3 { Species = 025, Level = 10, TID = 31121, OT_Gender = 0, OT_Name = "COLOS", CardTitle = "Colosseum Pikachu", Method = PIDType.CXD, Shiny = false, Language = 2, NotDistributed = true },

            new WC3 { Species = 250, Level = 70, TID = 10048, OT_Gender = 0, OT_Name = "バトルやま", CardTitle = "Mt. Battle Ho-oh", Method = PIDType.CXD, Shiny = false, Language = 1 }, // JPN
            new WC3 { Species = 250, Level = 70, TID = 10048, OT_Gender = 0, OT_Name = "MATTLE", CardTitle = "Mt. Battle Ho-oh", Method = PIDType.CXD, Shiny = false, Language = 2 }, // ENG
            new WC3 { Species = 250, Level = 70, TID = 10048, OT_Gender = 0, OT_Name = "MT BATA", CardTitle = "Mt. Battle Ho-oh", Method = PIDType.CXD, Shiny = false, Language = 3 }, // FRE
            new WC3 { Species = 250, Level = 70, TID = 10048, OT_Gender = 0, OT_Name = "DUELLBE", CardTitle = "Mt. Battle Ho-oh", Method = PIDType.CXD, Shiny = false, Language = 5 }, // GER
            new WC3 { Species = 250, Level = 70, TID = 10048, OT_Gender = 0, OT_Name = "MONTE L", CardTitle = "Mt. Battle Ho-oh", Method = PIDType.CXD, Shiny = false, Language = 4 }, // ITA
            new WC3 { Species = 250, Level = 70, TID = 10048, OT_Gender = 0, OT_Name = "ERNESTO", CardTitle = "Mt. Battle Ho-oh", Method = PIDType.CXD, Shiny = false, Language = 7 }, // SPA

            // CXD
            new WC3 { Species = 239, Level = 20, Language = 2, Fateful = true,  Met_Location = 164, TID = 41400, SID = -1, OT_Gender = 0, OT_Name = "HORDEL", CardTitle = "Trade Togepi", Method = PIDType.CXD, Moves = new[] {8,7,9,238} }, // Elekid @ Snagem Hideout
            new WC3 { Species = 307, Level = 20, Language = 2, Fateful = true,  Met_Location = 116, TID = 37149, SID = -1, OT_Gender = 0, OT_Name = "DUKING", CardTitle = "Trade Trapinch", Method = PIDType.CXD, Moves = new[] {223,93,247,197} }, // Meditite @ Pyrite Town
            new WC3 { Species = 213, Level = 20, Language = 2, Fateful = true,  Met_Location = 116, TID = 37149, SID = -1, OT_Gender = 0, OT_Name = "DUKING", CardTitle = "Trade Surskit", Method = PIDType.CXD, Moves = new[] {92,164,188,277} }, // Shuckle @ Pyrite Town
            new WC3 { Species = 246, Level = 20, Language = 2, Fateful = true,  Met_Location = 116, TID = 37149, SID = -1, OT_Gender = 0, OT_Name = "DUKING", CardTitle = "Trade Wooper", Method = PIDType.CXD, Moves = new[] {201,349,44,200} }, // Larvitar @ Pyrite Town
            new WC3 { Species = 311, Level = 13, Language = 2, Fateful = false, Met_Location = 254, TID = 37149, OT_Gender = 0, OT_Name = "DUKING", CardTitle = "Gift", Method = PIDType.CXD }, // Plusle @ Ingame Trade

            new WC3 { Species = 239, Level = 20, Language = 1, Fateful = true,  Met_Location = 164, TID = 41400, SID = -1, OT_Gender = 0, OT_Name = "ダニー",   CardTitle = "Trade Togepi", Method = PIDType.CXD, Moves = new[] {8,7,9,238} }, // Elekid @ Snagem Hideout
            new WC3 { Species = 307, Level = 20, Language = 1, Fateful = true,  Met_Location = 116, TID = 37149, SID = -1, OT_Gender = 0, OT_Name = "ギンザル", CardTitle = "Trade Trapinch", Method = PIDType.CXD, Moves = new[] {223,93,247,197} }, // Meditite @ Pyrite Town
            new WC3 { Species = 213, Level = 20, Language = 1, Fateful = true,  Met_Location = 116, TID = 37149, SID = -1, OT_Gender = 0, OT_Name = "ギンザル", CardTitle = "Trade Surskit", Method = PIDType.CXD, Moves = new[] {92,164,188,277} }, // Shuckle @ Pyrite Town
            new WC3 { Species = 246, Level = 20, Language = 1, Fateful = true,  Met_Location = 116, TID = 37149, SID = -1, OT_Gender = 0, OT_Name = "ギンザル", CardTitle = "Trade Wooper", Method = PIDType.CXD, Moves = new[] {201,349,44,200} }, // Larvitar @ Pyrite Town
            new WC3 { Species = 311, Level = 13, Language = 1, Fateful = false, Met_Location = 254, TID = 37149, OT_Gender = 0, OT_Name = "ギンザル", CardTitle = "Gift", Method = PIDType.CXD }, // Plusle @ Ingame Trade
        };

        internal static readonly MysteryGift[] Encounter_Event3_FRLG =
        {
            // PCJP - Egg Pokémon Present Eggs (March 21 to April 4, 2004)
            new WC3 { Species = 043, IsEgg = true, Fateful = true, Level = 05, TID = -1, SID = -1, Version = (int)GameVersion.FRLG, Method = PIDType.Method_2, Moves = new[]{073} }, // Oddish with Leech Seed
            new WC3 { Species = 052, IsEgg = true, Fateful = true, Level = 05, TID = -1, SID = -1, Version = (int)GameVersion.FRLG, Method = PIDType.Method_2, Moves = new[]{080} }, // Meowth with Petal Dance
            new WC3 { Species = 060, IsEgg = true, Fateful = true, Level = 05, TID = -1, SID = -1, Version = (int)GameVersion.FRLG, Method = PIDType.Method_2, Moves = new[]{186} }, // Poliwag with Sweet Kiss
            new WC3 { Species = 069, IsEgg = true, Fateful = true, Level = 05, TID = -1, SID = -1, Version = (int)GameVersion.FRLG, Method = PIDType.Method_2, Moves = new[]{298} }, // Bellsprout with Teeter Dance

            // PCNY - Wish Eggs (December 16, 2004, to January 2, 2005)
            new WC3 { Species = 083, IsEgg = true, Fateful = true, Level = 05, TID = -1, SID = -1, Version = (int)GameVersion.FRLG, Method = PIDType.Method_2, Moves = new[]{273, 281} }, // Farfetch'd with Wish & Yawn
            new WC3 { Species = 096, IsEgg = true, Fateful = true, Level = 05, TID = -1, SID = -1, Version = (int)GameVersion.FRLG, Method = PIDType.Method_2, Moves = new[]{273, 187} }, // Drowzee with Wish & Belly Drum
            new WC3 { Species = 102, IsEgg = true, Fateful = true, Level = 05, TID = -1, SID = -1, Version = (int)GameVersion.FRLG, Method = PIDType.Method_2, Moves = new[]{273, 230} }, // Exeggcute with Wish & Sweet Scent
            new WC3 { Species = 108, IsEgg = true, Fateful = true, Level = 05, TID = -1, SID = -1, Version = (int)GameVersion.FRLG, Method = PIDType.Method_2, Moves = new[]{273, 215} }, // Lickitung with Wish & Heal Bell
            new WC3 { Species = 113, IsEgg = true, Fateful = true, Level = 05, TID = -1, SID = -1, Version = (int)GameVersion.FRLG, Method = PIDType.Method_2, Moves = new[]{273, 230} }, // Chansey with Wish & Sweet Scent
            new WC3 { Species = 115, IsEgg = true, Fateful = true, Level = 05, TID = -1, SID = -1, Version = (int)GameVersion.FRLG, Method = PIDType.Method_2, Moves = new[]{273, 281} }, // Kangaskhan with Wish & Yawn

            // PokePark Eggs - Wondercard
            new WC3 { Species = 054, IsEgg = true, Fateful = true, Level = 05, TID = -1, SID = -1, Version = (int)GameVersion.FRLG, Moves = new[]{300}, Method = PIDType.Method_2 }, // Psyduck with Mud Sport
            new WC3 { Species = 172, IsEgg = true, Fateful = true, Level = 05, TID = -1, SID = -1, Version = (int)GameVersion.FRLG, Moves = new[]{266}, Method = PIDType.Method_2 }, // Pichu with Follow me
            new WC3 { Species = 174, IsEgg = true, Fateful = true, Level = 05, TID = -1, SID = -1, Version = (int)GameVersion.FRLG, Moves = new[]{321}, Method = PIDType.Method_2 }, // Igglybuff with Tickle
            new WC3 { Species = 222, IsEgg = true, Fateful = true, Level = 05, TID = -1, SID = -1, Version = (int)GameVersion.FRLG, Moves = new[]{300}, Method = PIDType.Method_2 }, // Corsola with Mud Sport
            new WC3 { Species = 276, IsEgg = true, Fateful = true, Level = 05, TID = -1, SID = -1, Version = (int)GameVersion.FRLG, Moves = new[]{297}, Method = PIDType.Method_2 }, // Taillow with Feather Dance
            new WC3 { Species = 283, IsEgg = true, Fateful = true, Level = 05, TID = -1, SID = -1, Version = (int)GameVersion.FRLG, Moves = new[]{300}, Method = PIDType.Method_2 }, // Surskit with Mud Sport
            new WC3 { Species = 293, IsEgg = true, Fateful = true, Level = 05, TID = -1, SID = -1, Version = (int)GameVersion.FRLG, Moves = new[]{298}, Method = PIDType.Method_2 }, // Whismur with Teeter Dance
            new WC3 { Species = 300, IsEgg = true, Fateful = true, Level = 05, TID = -1, SID = -1, Version = (int)GameVersion.FRLG, Moves = new[]{205}, Method = PIDType.Method_2 }, // Skitty with Rollout
            new WC3 { Species = 311, IsEgg = true, Fateful = true, Level = 05, TID = -1, SID = -1, Version = (int)GameVersion.FRLG, Moves = new[]{346}, Method = PIDType.Method_2 }, // Plusle with Water Sport
            new WC3 { Species = 312, IsEgg = true, Fateful = true, Level = 05, TID = -1, SID = -1, Version = (int)GameVersion.FRLG, Moves = new[]{300}, Method = PIDType.Method_2 }, // Minun with Mud Sport
            new WC3 { Species = 325, IsEgg = true, Fateful = true, Level = 05, TID = -1, SID = -1, Version = (int)GameVersion.FRLG, Moves = new[]{253}, Method = PIDType.Method_2 }, // Spoink with Uproar
            new WC3 { Species = 327, IsEgg = true, Fateful = true, Level = 05, TID = -1, SID = -1, Version = (int)GameVersion.FRLG, Moves = new[]{047}, Method = PIDType.Method_2 }, // Spinda with Sing
            new WC3 { Species = 331, IsEgg = true, Fateful = true, Level = 05, TID = -1, SID = -1, Version = (int)GameVersion.FRLG, Moves = new[]{227}, Method = PIDType.Method_2 }, // Cacnea with Encore
            new WC3 { Species = 341, IsEgg = true, Fateful = true, Level = 05, TID = -1, SID = -1, Version = (int)GameVersion.FRLG, Moves = new[]{346}, Method = PIDType.Method_2 }, // Corphish with Water Sport
            new WC3 { Species = 360, IsEgg = true, Fateful = true, Level = 05, TID = -1, SID = -1, Version = (int)GameVersion.FRLG, Moves = new[]{321}, Method = PIDType.Method_2 }, // Wynaut with Tickle
        };

        internal static readonly MysteryGift[] Encounter_Event3_RS =
        {   
            // PCJP - Pokémon Center 5th Anniversary Eggs (April 25 to May 18, 2003)
            new WC3 { Species = 172, IsEgg = true, Level = 05, OT_Name = "オヤＮＡＭＥ", TID = -1, SID = -1, Version = (int)GameVersion.R, Method = PIDType.BACD_R, Moves = new[]{298} }, // Pichu with Teeter Dance
            new WC3 { Species = 172, IsEgg = true, Level = 05, OT_Name = "オヤＮＡＭＥ", TID = -1, SID = -1, Version = (int)GameVersion.R, Method = PIDType.BACD_R, Moves = new[]{273} }, // Pichu with Wish
            new WC3 { Species = 172, IsEgg = true, Level = 05, OT_Name = "オヤＮＡＭＥ", TID = -1, SID = -1, Version = (int)GameVersion.R, Method = PIDType.BACD_R_S, Moves = new[]{298} }, // Pichu with Teeter Dance
            new WC3 { Species = 172, IsEgg = true, Level = 05, OT_Name = "オヤＮＡＭＥ", TID = -1, SID = -1, Version = (int)GameVersion.R, Method = PIDType.BACD_R_S, Moves = new[]{273} }, // Pichu with Wish
            new WC3 { Species = 280, IsEgg = true, Level = 05, OT_Name = "オヤＮＡＭＥ", TID = -1, SID = -1, Version = (int)GameVersion.R, Method = PIDType.BACD_R, Moves = new[]{204 } }, // Ralts with Charm
            new WC3 { Species = 280, IsEgg = true, Level = 05, OT_Name = "オヤＮＡＭＥ", TID = -1, SID = -1, Version = (int)GameVersion.R, Method = PIDType.BACD_R, Moves = new[]{273} }, // Ralts with Wish
            new WC3 { Species = 359, IsEgg = true, Level = 05, OT_Name = "オヤＮＡＭＥ", TID = -1, SID = -1, Version = (int)GameVersion.R, Method = PIDType.BACD_R, Moves = new[]{180} }, // Absol with Spite
            new WC3 { Species = 359, IsEgg = true, Level = 05, OT_Name = "オヤＮＡＭＥ", TID = -1, SID = -1, Version = (int)GameVersion.R, Method = PIDType.BACD_R, Moves = new[]{273} }, // Absol with Wish
            new WC3 { Species = 371, IsEgg = true, Level = 05, OT_Name = "オヤＮＡＭＥ", TID = -1, SID = -1, Version = (int)GameVersion.R, Method = PIDType.BACD_R, Moves = new[]{334} }, // Bagon with Iron Defense
            new WC3 { Species = 371, IsEgg = true, Level = 05, OT_Name = "オヤＮＡＭＥ", TID = -1, SID = -1, Version = (int)GameVersion.R, Method = PIDType.BACD_R, Moves = new[]{273} }, // Bagon with Wish
            
            // Negai Boshi Jirachi
            new WC3 { Species = 385, Level = 05, TID = 30719, OT_Gender = 0, OT_Name = "ネガイボシ", Method = PIDType.BACD_R, Language = 1, Shiny = false },

            // Berry Glitch Fix
            // PCJP - (December 29, 2003 to March 31, 2004)
            new WC3 { Species = 263, Level = 5, Version = (int)GameVersion.S, Language = 1, Method = PIDType.BACD_R_S, TID = 21121, OT_Name = "ルビー", OT_Gender = 1, Shiny = true },
            new WC3 { Species = 263, Level = 5, Version = (int)GameVersion.S, Language = 1, Method = PIDType.BACD_R_S, TID = 21121, OT_Name = "サファイア", OT_Gender = 0, Shiny = true },
            
            // EBGames/GameStop (March 1, 2004 to April 22, 2007), also via multi-game discs
            new WC3 { Species = 263, Level = 5, Version = (int)GameVersion.S, Language = 2, Method = PIDType.BACD_R_S, TID = 30317, OT_Name = "RUBY", OT_Gender = 1 },
            new WC3 { Species = 263, Level = 5, Version = (int)GameVersion.S, Language = 2, Method = PIDType.BACD_R_S, TID = 30317, OT_Name = "SAPHIRE", OT_Gender = 0 },

            // Channel Jirachi
            new WC3 { Species = 385, Level = 5, Version = (int)GameVersion.RS, Method = PIDType.Channel, TID = 40122, OT_Gender = 3,SID = -1, OT_Name = "CHANNEL", CardTitle = "Channel Jirachi", Met_Level = 0 },

            // Aura Mew
            new WC3 { Species = 151, Level = 10, Version = (int)GameVersion.R, Language = 2, Method = PIDType.BACD_R, TID = 20078, OT_Name = "Aura", Fateful = true }, // Mew
            new WC3 { Species = 151, Level = 10, Version = (int)GameVersion.R, Language = 3, Method = PIDType.BACD_R, TID = 20078, OT_Name = "Aura", Fateful = true }, // Mew
            new WC3 { Species = 151, Level = 10, Version = (int)GameVersion.R, Language = 4, Method = PIDType.BACD_R, TID = 20078, OT_Name = "Aura", Fateful = true }, // Mew
            new WC3 { Species = 151, Level = 10, Version = (int)GameVersion.R, Language = 5, Method = PIDType.BACD_R, TID = 20078, OT_Name = "Aura", Fateful = true }, // Mew
            new WC3 { Species = 151, Level = 10, Version = (int)GameVersion.R, Language = 7, Method = PIDType.BACD_R, TID = 20078, OT_Name = "Aura", Fateful = true }, // Mew

            // English Events
            new WC3 { Species = 006, Level = 70, Version = (int)GameVersion.R, Moves = new[] {017,163,082,083}, Language = 2, Method = PIDType.BACD_R, TID = 06227, OT_Name = "10ANNIV", Shiny = false }, // Charizard
            new WC3 { Species = 025, Level = 70, Version = (int)GameVersion.R, Moves = new[] {085,097,087,113}, Language = 2, Method = PIDType.BACD_R, TID = 06227, OT_Name = "10ANNIV", Shiny = false }, // Pikachu
            new WC3 { Species = 144, Level = 70, Version = (int)GameVersion.R, Moves = new[] {097,170,058,115}, Language = 2, Method = PIDType.BACD_R, TID = 06227, OT_Name = "10ANNIV", Shiny = false }, // Articuno
            new WC3 { Species = 243, Level = 70, Version = (int)GameVersion.R, Moves = new[] {098,209,115,242}, Language = 2, Method = PIDType.BACD_R, TID = 06227, OT_Name = "10ANNIV", Shiny = false }, // Raikou
            new WC3 { Species = 244, Level = 70, Version = (int)GameVersion.R, Moves = new[] {083,023,053,207}, Language = 2, Method = PIDType.BACD_R, TID = 06227, OT_Name = "10ANNIV", Shiny = false }, // Entei
            new WC3 { Species = 245, Level = 70, Version = (int)GameVersion.R, Moves = new[] {016,062,054,243}, Language = 2, Method = PIDType.BACD_R, TID = 06227, OT_Name = "10ANNIV", Shiny = false }, // Suicune
            new WC3 { Species = 249, Level = 70, Version = (int)GameVersion.R, Moves = new[] {105,056,240,129}, Language = 2, Method = PIDType.BACD_R, TID = 06227, OT_Name = "10ANNIV", Shiny = false }, // Lugia
            new WC3 { Species = 250, Level = 70, Version = (int)GameVersion.R, Moves = new[] {105,126,241,129}, Language = 2, Method = PIDType.BACD_R, TID = 06227, OT_Name = "10ANNIV", Shiny = false }, // Ho-Oh
            new WC3 { Species = 380, Level = 70, Version = (int)GameVersion.R, Moves = new[] {296,094,105,204}, Language = 2, Method = PIDType.BACD_R, TID = 06227, OT_Name = "10ANNIV", Shiny = false }, // Latias
            new WC3 { Species = 381, Level = 70, Version = (int)GameVersion.R, Moves = new[] {295,094,105,349}, Language = 2, Method = PIDType.BACD_R, TID = 06227, OT_Name = "10ANNIV", Shiny = false }, // Latios

            // French
            new WC3 { Species = 006, Level = 70, Version = (int)GameVersion.R, Moves = new[] {017,163,082,083}, Language = 3, Method = PIDType.BACD_R, TID = 06227, OT_Name = "10ANNIV", Shiny = false }, // Charizard
            new WC3 { Species = 025, Level = 70, Version = (int)GameVersion.R, Moves = new[] {085,097,087,113}, Language = 3, Method = PIDType.BACD_R, TID = 06227, OT_Name = "10ANNIV", Shiny = false }, // Pikachu
            new WC3 { Species = 144, Level = 70, Version = (int)GameVersion.R, Moves = new[] {097,170,058,115}, Language = 3, Method = PIDType.BACD_R, TID = 06227, OT_Name = "10ANNIV", Shiny = false }, // Articuno
            new WC3 { Species = 243, Level = 70, Version = (int)GameVersion.R, Moves = new[] {098,209,115,242}, Language = 3, Method = PIDType.BACD_R, TID = 06227, OT_Name = "10ANNIV", Shiny = false }, // Raikou
            new WC3 { Species = 244, Level = 70, Version = (int)GameVersion.R, Moves = new[] {083,023,053,207}, Language = 3, Method = PIDType.BACD_R, TID = 06227, OT_Name = "10ANNIV", Shiny = false }, // Entei
            new WC3 { Species = 245, Level = 70, Version = (int)GameVersion.R, Moves = new[] {016,062,054,243}, Language = 3, Method = PIDType.BACD_R, TID = 06227, OT_Name = "10ANNIV", Shiny = false }, // Suicune
            new WC3 { Species = 249, Level = 70, Version = (int)GameVersion.R, Moves = new[] {105,056,240,129}, Language = 3, Method = PIDType.BACD_R, TID = 06227, OT_Name = "10ANNIV", Shiny = false }, // Lugia
            new WC3 { Species = 250, Level = 70, Version = (int)GameVersion.R, Moves = new[] {105,126,241,129}, Language = 3, Method = PIDType.BACD_R, TID = 06227, OT_Name = "10ANNIV", Shiny = false }, // Ho-Oh
            new WC3 { Species = 380, Level = 70, Version = (int)GameVersion.R, Moves = new[] {296,094,105,204}, Language = 3, Method = PIDType.BACD_R, TID = 06227, OT_Name = "10ANNIV", Shiny = false }, // Latias
            new WC3 { Species = 381, Level = 70, Version = (int)GameVersion.R, Moves = new[] {295,094,105,349}, Language = 3, Method = PIDType.BACD_R, TID = 06227, OT_Name = "10ANNIV", Shiny = false }, // Latios

            // Italian
            new WC3 { Species = 006, Level = 70, Version = (int)GameVersion.R, Moves = new[] {017,163,082,083}, Language = 4, Method = PIDType.BACD_R, TID = 06227, OT_Name = "10ANNI", Shiny = false }, // Charizard
            new WC3 { Species = 025, Level = 70, Version = (int)GameVersion.R, Moves = new[] {085,097,087,113}, Language = 4, Method = PIDType.BACD_R, TID = 06227, OT_Name = "10ANNI", Shiny = false }, // Pikachu
            new WC3 { Species = 144, Level = 70, Version = (int)GameVersion.R, Moves = new[] {097,170,058,115}, Language = 4, Method = PIDType.BACD_R, TID = 06227, OT_Name = "10ANNI", Shiny = false }, // Articuno
            new WC3 { Species = 243, Level = 70, Version = (int)GameVersion.R, Moves = new[] {098,209,115,242}, Language = 4, Method = PIDType.BACD_R, TID = 06227, OT_Name = "10ANNI", Shiny = false }, // Raikou
            new WC3 { Species = 244, Level = 70, Version = (int)GameVersion.R, Moves = new[] {083,023,053,207}, Language = 4, Method = PIDType.BACD_R, TID = 06227, OT_Name = "10ANNI", Shiny = false }, // Entei
            new WC3 { Species = 245, Level = 70, Version = (int)GameVersion.R, Moves = new[] {016,062,054,243}, Language = 4, Method = PIDType.BACD_R, TID = 06227, OT_Name = "10ANNI", Shiny = false }, // Suicune
            new WC3 { Species = 249, Level = 70, Version = (int)GameVersion.R, Moves = new[] {105,056,240,129}, Language = 4, Method = PIDType.BACD_R, TID = 06227, OT_Name = "10ANNI", Shiny = false }, // Lugia
            new WC3 { Species = 250, Level = 70, Version = (int)GameVersion.R, Moves = new[] {105,126,241,129}, Language = 4, Method = PIDType.BACD_R, TID = 06227, OT_Name = "10ANNI", Shiny = false }, // Ho-Oh
            new WC3 { Species = 380, Level = 70, Version = (int)GameVersion.R, Moves = new[] {296,094,105,204}, Language = 4, Method = PIDType.BACD_R, TID = 06227, OT_Name = "10ANNI", Shiny = false }, // Latias
            new WC3 { Species = 381, Level = 70, Version = (int)GameVersion.R, Moves = new[] {295,094,105,349}, Language = 4, Method = PIDType.BACD_R, TID = 06227, OT_Name = "10ANNI", Shiny = false }, // Latios
            
            // German
            new WC3 { Species = 006, Level = 70, Version = (int)GameVersion.R, Moves = new[] {017,163,082,083}, Language = 5, Method = PIDType.BACD_R, TID = 06227, OT_Name = "10JAHRE", Shiny = false }, // Charizard
            new WC3 { Species = 025, Level = 70, Version = (int)GameVersion.R, Moves = new[] {085,097,087,113}, Language = 5, Method = PIDType.BACD_R, TID = 06227, OT_Name = "10JAHRE", Shiny = false }, // Pikachu
            new WC3 { Species = 144, Level = 70, Version = (int)GameVersion.R, Moves = new[] {097,170,058,115}, Language = 5, Method = PIDType.BACD_R, TID = 06227, OT_Name = "10JAHRE", Shiny = false }, // Articuno
            new WC3 { Species = 243, Level = 70, Version = (int)GameVersion.R, Moves = new[] {098,209,115,242}, Language = 5, Method = PIDType.BACD_R, TID = 06227, OT_Name = "10JAHRE", Shiny = false }, // Raikou
            new WC3 { Species = 244, Level = 70, Version = (int)GameVersion.R, Moves = new[] {083,023,053,207}, Language = 5, Method = PIDType.BACD_R, TID = 06227, OT_Name = "10JAHRE", Shiny = false }, // Entei
            new WC3 { Species = 245, Level = 70, Version = (int)GameVersion.R, Moves = new[] {016,062,054,243}, Language = 5, Method = PIDType.BACD_R, TID = 06227, OT_Name = "10JAHRE", Shiny = false }, // Suicune
            new WC3 { Species = 249, Level = 70, Version = (int)GameVersion.R, Moves = new[] {105,056,240,129}, Language = 5, Method = PIDType.BACD_R, TID = 06227, OT_Name = "10JAHRE", Shiny = false }, // Lugia
            new WC3 { Species = 250, Level = 70, Version = (int)GameVersion.R, Moves = new[] {105,126,241,129}, Language = 5, Method = PIDType.BACD_R, TID = 06227, OT_Name = "10JAHRE", Shiny = false }, // Ho-Oh
            new WC3 { Species = 380, Level = 70, Version = (int)GameVersion.R, Moves = new[] {296,094,105,204}, Language = 5, Method = PIDType.BACD_R, TID = 06227, OT_Name = "10JAHRE", Shiny = false }, // Latias
            new WC3 { Species = 381, Level = 70, Version = (int)GameVersion.R, Moves = new[] {295,094,105,349}, Language = 5, Method = PIDType.BACD_R, TID = 06227, OT_Name = "10JAHRE", Shiny = false }, // Latios

            // Spanish
            new WC3 { Species = 006, Level = 70, Version = (int)GameVersion.R, Moves = new[] {017,163,082,083}, Language = 7, Method = PIDType.BACD_R, TID = 06227, OT_Name = "10ANIV", Shiny = false }, // Charizard
            new WC3 { Species = 025, Level = 70, Version = (int)GameVersion.R, Moves = new[] {085,097,087,113}, Language = 7, Method = PIDType.BACD_R, TID = 06227, OT_Name = "10ANIV", Shiny = false }, // Pikachu
            new WC3 { Species = 144, Level = 70, Version = (int)GameVersion.R, Moves = new[] {097,170,058,115}, Language = 7, Method = PIDType.BACD_R, TID = 06227, OT_Name = "10ANIV", Shiny = false }, // Articuno
            new WC3 { Species = 243, Level = 70, Version = (int)GameVersion.R, Moves = new[] {098,209,115,242}, Language = 7, Method = PIDType.BACD_R, TID = 06227, OT_Name = "10ANIV", Shiny = false }, // Raikou
            new WC3 { Species = 244, Level = 70, Version = (int)GameVersion.R, Moves = new[] {083,023,053,207}, Language = 7, Method = PIDType.BACD_R, TID = 06227, OT_Name = "10ANIV", Shiny = false }, // Entei
            new WC3 { Species = 245, Level = 70, Version = (int)GameVersion.R, Moves = new[] {016,062,054,243}, Language = 7, Method = PIDType.BACD_R, TID = 06227, OT_Name = "10ANIV", Shiny = false }, // Suicune
            new WC3 { Species = 249, Level = 70, Version = (int)GameVersion.R, Moves = new[] {105,056,240,129}, Language = 7, Method = PIDType.BACD_R, TID = 06227, OT_Name = "10ANIV", Shiny = false }, // Lugia
            new WC3 { Species = 250, Level = 70, Version = (int)GameVersion.R, Moves = new[] {105,126,241,129}, Language = 7, Method = PIDType.BACD_R, TID = 06227, OT_Name = "10ANIV", Shiny = false }, // Ho-Oh
            new WC3 { Species = 380, Level = 70, Version = (int)GameVersion.R, Moves = new[] {296,094,105,204}, Language = 7, Method = PIDType.BACD_R, TID = 06227, OT_Name = "10ANIV", Shiny = false }, // Latias
            new WC3 { Species = 381, Level = 70, Version = (int)GameVersion.R, Moves = new[] {295,094,105,349}, Language = 7, Method = PIDType.BACD_R, TID = 06227, OT_Name = "10ANIV", Shiny = false }, // Latios

            new WC3 { Species = 375, Level = 30, Version = (int)GameVersion.R, Moves = new[] {036,093,232,287}, Language = 2, Method = PIDType.BACD_R, TID = 02005, OT_Name = "ROCKS", OT_Gender = 0, RibbonNational = true, Shiny = false }, // Metang
            new WC3 { Species = 386, Level = 70, Version = (int)GameVersion.R, Moves = new[] {322,105,354,063}, Language = 2, Method = PIDType.BACD_R, TID = 28606, OT_Name = "DOEL", Fateful = true, Shiny = false }, // Deoxys
            new WC3 { Species = 386, Level = 70, Version = (int)GameVersion.R, Moves = new[] {322,105,354,063}, Language = 2, Method = PIDType.BACD_R, TID = 00010, OT_Name = "SPACE C", Fateful = true, Shiny = false }, // Deoxys
            new WC3 { Species = 151, Level = 10, Version = (int)GameVersion.R, Language = 2, Method = PIDType.BACD_U, TID = 06930, OT_Name = "MYSTRY", Fateful = true, Shiny = false }, // Mew
            new WC3 { Species = 151, Level = 10, Version = (int)GameVersion.R, Language = 2, Method = PIDType.BACD_R, TID = 06930, OT_Name = "MYSTRY", Fateful = true, Shiny = false }, // Mew

            // Party of the Decade
            new WC3 { Species = 001, Level = 70, Version = (int)GameVersion.R, Moves = new[] {230,074,076,235}, Language = 2, Method = PIDType.BACD_R, TID = 06808, OT_Name = "10 ANIV", Shiny = false }, // Bulbasaur
            new WC3 { Species = 006, Level = 70, Version = (int)GameVersion.R, Moves = new[] {017,163,082,083}, Language = 2, Method = PIDType.BACD_R, TID = 06808, OT_Name = "10 ANIV", Shiny = false }, // Charizard
            new WC3 { Species = 009, Level = 70, Version = (int)GameVersion.R, Moves = new[] {182,240,130,056}, Language = 2, Method = PIDType.BACD_R, TID = 06808, OT_Name = "10 ANIV", Shiny = false }, // Blastoise
            new WC3 { Species = 025, Level = 70, Version = (int)GameVersion.R, Moves = new[] {085,087,113,019}, Language = 2, Method = PIDType.BACD_R, TID = 06808, OT_Name = "10 ANIV", HeldItem = 202, Shiny = false }, // Pikachu (Fly)
            new WC3 { Species = 065, Level = 70, Version = (int)GameVersion.R, Moves = new[] {248,347,094,271}, Language = 2, Method = PIDType.BACD_R, TID = 06808, OT_Name = "10 ANIV", Shiny = false }, // Alakazam
            new WC3 { Species = 144, Level = 70, Version = (int)GameVersion.R, Moves = new[] {097,170,058,115}, Language = 2, Method = PIDType.BACD_R, TID = 06808, OT_Name = "10 ANIV", Shiny = false }, // Articuno
            new WC3 { Species = 145, Level = 70, Version = (int)GameVersion.R, Moves = new[] {097,197,065,268}, Language = 2, Method = PIDType.BACD_R, TID = 06808, OT_Name = "10 ANIV", Shiny = false }, // Zapdos
            new WC3 { Species = 146, Level = 70, Version = (int)GameVersion.R, Moves = new[] {097,203,053,219}, Language = 2, Method = PIDType.BACD_R, TID = 06808, OT_Name = "10 ANIV", Shiny = false }, // Moltres
            new WC3 { Species = 149, Level = 70, Version = (int)GameVersion.R, Moves = new[] {097,219,017,200}, Language = 2, Method = PIDType.BACD_R, TID = 06808, OT_Name = "10 ANIV", Shiny = false }, // Dragonite
            new WC3 { Species = 157, Level = 70, Version = (int)GameVersion.R, Moves = new[] {098,172,129,053}, Language = 2, Method = PIDType.BACD_R, TID = 06808, OT_Name = "10 ANIV", Shiny = false }, // Typhlosion
            new WC3 { Species = 196, Level = 70, Version = (int)GameVersion.R, Moves = new[] {060,244,094,234}, Language = 2, Method = PIDType.BACD_R, TID = 06808, OT_Name = "10 ANIV", Shiny = false }, // Espeon
            new WC3 { Species = 197, Level = 70, Version = (int)GameVersion.R, Moves = new[] {185,212,103,236}, Language = 2, Method = PIDType.BACD_R, TID = 06808, OT_Name = "10 ANIV", Shiny = false }, // Umbreon
            new WC3 { Species = 243, Level = 70, Version = (int)GameVersion.R, Moves = new[] {098,209,115,242}, Language = 2, Method = PIDType.BACD_R, TID = 06808, OT_Name = "10 ANIV", Shiny = false }, // Raikou
            new WC3 { Species = 244, Level = 70, Version = (int)GameVersion.R, Moves = new[] {083,023,053,207}, Language = 2, Method = PIDType.BACD_R, TID = 06808, OT_Name = "10 ANIV", Shiny = false }, // Entei
            new WC3 { Species = 245, Level = 70, Version = (int)GameVersion.R, Moves = new[] {016,062,054,243}, Language = 2, Method = PIDType.BACD_R, TID = 06808, OT_Name = "10 ANIV", Shiny = false }, // Suicune
            new WC3 { Species = 248, Level = 70, Version = (int)GameVersion.R, Moves = new[] {037,184,242,089}, Language = 2, Method = PIDType.BACD_R, TID = 06808, OT_Name = "10 ANIV", Shiny = false }, // Tyranitar
            new WC3 { Species = 257, Level = 70, Version = (int)GameVersion.R, Moves = new[] {299,163,119,327}, Language = 2, Method = PIDType.BACD_R, TID = 06808, OT_Name = "10 ANIV", Shiny = false }, // Blaziken
            new WC3 { Species = 359, Level = 70, Version = (int)GameVersion.R, Moves = new[] {104,163,248,195}, Language = 2, Method = PIDType.BACD_R, TID = 06808, OT_Name = "10 ANIV", Shiny = false }, // Absol
            new WC3 { Species = 380, Level = 70, Version = (int)GameVersion.R, Moves = new[] {296,094,105,204}, Language = 2, Method = PIDType.BACD_R, TID = 06808, OT_Name = "10 ANIV", HeldItem = 191, Shiny = false }, // Latias
            new WC3 { Species = 381, Level = 70, Version = (int)GameVersion.R, Moves = new[] {295,094,105,349}, Language = 2, Method = PIDType.BACD_R, TID = 06808, OT_Name = "10 ANIV", HeldItem = 191, Shiny = false }, // Latios
            
            // Journey Across America
            new WC3 { Species = 001, Level = 70, Version = (int)GameVersion.R, Moves = new[] {230,074,076,235}, Language = 2, Method = PIDType.BACD_R, TID = 00010, OT_Name = "10 ANIV", Shiny = false }, // Bulbasaur
            new WC3 { Species = 006, Level = 70, Version = (int)GameVersion.R, Moves = new[] {017,163,082,083}, Language = 2, Method = PIDType.BACD_R, TID = 00010, OT_Name = "10 ANIV", Shiny = false }, // Charizard
            new WC3 { Species = 009, Level = 70, Version = (int)GameVersion.R, Moves = new[] {182,240,130,056}, Language = 2, Method = PIDType.BACD_R, TID = 00010, OT_Name = "10 ANIV", Shiny = false }, // Blastoise
            new WC3 { Species = 025, Level = 70, Version = (int)GameVersion.R, Moves = new[] {085,097,087,113}, Language = 2, Method = PIDType.BACD_R, TID = 00010, OT_Name = "10 ANIV", HeldItem = 202, Shiny = false }, // Pikachu (No Fly)
            new WC3 { Species = 065, Level = 70, Version = (int)GameVersion.R, Moves = new[] {248,347,094,271}, Language = 2, Method = PIDType.BACD_R, TID = 00010, OT_Name = "10 ANIV", Shiny = false }, // Alakazam
            new WC3 { Species = 144, Level = 70, Version = (int)GameVersion.R, Moves = new[] {097,170,058,115}, Language = 2, Method = PIDType.BACD_R, TID = 00010, OT_Name = "10 ANIV", Shiny = false }, // Articuno
            new WC3 { Species = 145, Level = 70, Version = (int)GameVersion.R, Moves = new[] {097,197,065,268}, Language = 2, Method = PIDType.BACD_R, TID = 00010, OT_Name = "10 ANIV", Shiny = false }, // Zapdos
            new WC3 { Species = 146, Level = 70, Version = (int)GameVersion.R, Moves = new[] {097,203,053,219}, Language = 2, Method = PIDType.BACD_R, TID = 00010, OT_Name = "10 ANIV", Shiny = false }, // Moltres
            new WC3 { Species = 149, Level = 70, Version = (int)GameVersion.R, Moves = new[] {097,219,017,200}, Language = 2, Method = PIDType.BACD_R, TID = 00010, OT_Name = "10 ANIV", Shiny = false }, // Dragonite
            new WC3 { Species = 157, Level = 70, Version = (int)GameVersion.R, Moves = new[] {098,172,129,053}, Language = 2, Method = PIDType.BACD_R, TID = 00010, OT_Name = "10 ANIV", Shiny = false }, // Typhlosion
            new WC3 { Species = 196, Level = 70, Version = (int)GameVersion.R, Moves = new[] {060,244,094,234}, Language = 2, Method = PIDType.BACD_R, TID = 00010, OT_Name = "10 ANIV", Shiny = false }, // Espeon
            new WC3 { Species = 197, Level = 70, Version = (int)GameVersion.R, Moves = new[] {185,212,103,236}, Language = 2, Method = PIDType.BACD_R, TID = 00010, OT_Name = "10 ANIV", Shiny = false }, // Umbreon
            new WC3 { Species = 243, Level = 70, Version = (int)GameVersion.R, Moves = new[] {098,209,115,242}, Language = 2, Method = PIDType.BACD_R, TID = 00010, OT_Name = "10 ANIV", Shiny = false }, // Raikou
            new WC3 { Species = 244, Level = 70, Version = (int)GameVersion.R, Moves = new[] {083,023,053,207}, Language = 2, Method = PIDType.BACD_R, TID = 00010, OT_Name = "10 ANIV", Shiny = false }, // Entei
            new WC3 { Species = 245, Level = 70, Version = (int)GameVersion.R, Moves = new[] {016,062,054,243}, Language = 2, Method = PIDType.BACD_R, TID = 00010, OT_Name = "10 ANIV", Shiny = false }, // Suicune
            new WC3 { Species = 248, Level = 70, Version = (int)GameVersion.R, Moves = new[] {037,184,242,089}, Language = 2, Method = PIDType.BACD_R, TID = 00010, OT_Name = "10 ANIV", Shiny = false }, // Tyranitar
            new WC3 { Species = 251, Level = 70, Version = (int)GameVersion.R, Moves = new[] {246,248,226,195}, Language = 2, Method = PIDType.BACD_R, TID = 00010, OT_Name = "10 ANIV", Shiny = false }, // Celebi
            new WC3 { Species = 257, Level = 70, Version = (int)GameVersion.R, Moves = new[] {299,163,119,327}, Language = 2, Method = PIDType.BACD_R, TID = 00010, OT_Name = "10 ANIV", Shiny = false }, // Blaziken
            new WC3 { Species = 359, Level = 70, Version = (int)GameVersion.R, Moves = new[] {104,163,248,195}, Language = 2, Method = PIDType.BACD_R, TID = 00010, OT_Name = "10 ANIV", Shiny = false }, // Absol
            new WC3 { Species = 380, Level = 70, Version = (int)GameVersion.R, Moves = new[] {296,094,105,204}, Language = 2, Method = PIDType.BACD_R, TID = 00010, OT_Name = "10 ANIV", HeldItem = 191, Shiny = false }, // Latias
            new WC3 { Species = 381, Level = 70, Version = (int)GameVersion.R, Moves = new[] {295,094,105,349}, Language = 2, Method = PIDType.BACD_R, TID = 00010, OT_Name = "10 ANIV", HeldItem = 191, Shiny = false }, // Latios
        };

        internal static readonly MysteryGift[] Encounter_Event3_Common =
        {
            // Pokémon Box
            new WC3 { Species = 333, IsEgg = true, Level = 05, Moves = new[]{206}, Method = PIDType.BACD_U, OT_Gender = 1, OT_Name = "ＡＺＵＳＡ" }, // Swablu Egg with False Swipe
            new WC3 { Species = 263, IsEgg = true, Level = 05, Moves = new[]{245}, Method = PIDType.BACD_U, OT_Gender = 1, OT_Name = "ＡＺＵＳＡ" }, // Zigzagoon Egg with Extreme Speed
            new WC3 { Species = 300, IsEgg = true, Level = 05, Moves = new[]{006}, Method = PIDType.BACD_U, OT_Gender = 1, OT_Name = "ＡＺＵＳＡ" }, // Skitty Egg with Pay Day
            new WC3 { Species = 172, IsEgg = true, Level = 05, Moves = new[]{057}, Method = PIDType.BACD_U, OT_Gender = 1, OT_Name = "ＡＺＵＳＡ" }, // Pichu Egg with Surf
 
            // PokePark Eggs - DS Download Play
            new WC3 { Species = 054, IsEgg = true, Level = 05, Met_Level = 05, TID = 50318, OT_Gender = 0, OT_Name = "ポケパーク", Version = (int)GameVersion.R, Moves = new[]{300}, Method = PIDType.BACD_R }, // Psyduck with Mud Sport
            new WC3 { Species = 172, IsEgg = true, Level = 05, Met_Level = 05, TID = 50318, OT_Gender = 0, OT_Name = "ポケパーク", Version = (int)GameVersion.R, Moves = new[]{266}, Method = PIDType.BACD_R }, // Pichu with Follow me
            new WC3 { Species = 174, IsEgg = true, Level = 05, Met_Level = 05, TID = 50318, OT_Gender = 0, OT_Name = "ポケパーク", Version = (int)GameVersion.R, Moves = new[]{321}, Method = PIDType.BACD_R }, // Igglybuff with Tickle
            new WC3 { Species = 222, IsEgg = true, Level = 05, Met_Level = 05, TID = 50318, OT_Gender = 0, OT_Name = "ポケパーク", Version = (int)GameVersion.R, Moves = new[]{300}, Method = PIDType.BACD_R }, // Corsola with Mud Sport
            new WC3 { Species = 276, IsEgg = true, Level = 05, Met_Level = 05, TID = 50318, OT_Gender = 0, OT_Name = "ポケパーク", Version = (int)GameVersion.R, Moves = new[]{297}, Method = PIDType.BACD_R }, // Taillow with Feather Dance
            new WC3 { Species = 283, IsEgg = true, Level = 05, Met_Level = 05, TID = 50318, OT_Gender = 0, OT_Name = "ポケパーク", Version = (int)GameVersion.R, Moves = new[]{300}, Method = PIDType.BACD_R }, // Surskit with Mud Sport
            new WC3 { Species = 293, IsEgg = true, Level = 05, Met_Level = 05, TID = 50318, OT_Gender = 0, OT_Name = "ポケパーク", Version = (int)GameVersion.R, Moves = new[]{298}, Method = PIDType.BACD_R }, // Whismur with Teeter Dance
            new WC3 { Species = 300, IsEgg = true, Level = 05, Met_Level = 05, TID = 50318, OT_Gender = 0, OT_Name = "ポケパーク", Version = (int)GameVersion.R, Moves = new[]{205}, Method = PIDType.BACD_R }, // Skitty with Rollout
            new WC3 { Species = 311, IsEgg = true, Level = 05, Met_Level = 05, TID = 50318, OT_Gender = 0, OT_Name = "ポケパーク", Version = (int)GameVersion.R, Moves = new[]{346}, Method = PIDType.BACD_R }, // Plusle with Water Sport
            new WC3 { Species = 312, IsEgg = true, Level = 05, Met_Level = 05, TID = 50318, OT_Gender = 0, OT_Name = "ポケパーク", Version = (int)GameVersion.R, Moves = new[]{300}, Method = PIDType.BACD_R }, // Minun with Mud Sport
            new WC3 { Species = 325, IsEgg = true, Level = 05, Met_Level = 05, TID = 50318, OT_Gender = 0, OT_Name = "ポケパーク", Version = (int)GameVersion.R, Moves = new[]{253}, Method = PIDType.BACD_R }, // Spoink with Uproar
            new WC3 { Species = 327, IsEgg = true, Level = 05, Met_Level = 05, TID = 50318, OT_Gender = 0, OT_Name = "ポケパーク", Version = (int)GameVersion.R, Moves = new[]{047}, Method = PIDType.BACD_R }, // Spinda with Sing
            new WC3 { Species = 331, IsEgg = true, Level = 05, Met_Level = 05, TID = 50318, OT_Gender = 0, OT_Name = "ポケパーク", Version = (int)GameVersion.R, Moves = new[]{227}, Method = PIDType.BACD_R }, // Cacnea with Encore
            new WC3 { Species = 341, IsEgg = true, Level = 05, Met_Level = 05, TID = 50318, OT_Gender = 0, OT_Name = "ポケパーク", Version = (int)GameVersion.R, Moves = new[]{346}, Method = PIDType.BACD_R }, // Corphish with Water Sport
            new WC3 { Species = 360, IsEgg = true, Level = 05, Met_Level = 05, TID = 50318, OT_Gender = 0, OT_Name = "ポケパーク", Version = (int)GameVersion.R, Moves = new[]{321}, Method = PIDType.BACD_R }, // Wynaut with Tickle
        };

        internal static readonly MysteryGift[] Encounter_WC3 = Encounter_Event3.Concat(Encounter_Event3_RS).Concat(Encounter_Event3_FRLG.Concat(Encounter_Event3_Common)).ToArray();

    }
}
