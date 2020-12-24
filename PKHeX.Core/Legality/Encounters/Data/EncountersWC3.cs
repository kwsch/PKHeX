using System.Collections.Generic;
using System.Linq;

namespace PKHeX.Core
{
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
        {
            new() { Species = 251, Level = 10, TID = 31121, OT_Gender = 1, Version = GameVersion.R, Method = PIDType.CXD, OT_Name = "アゲト", CardTitle = "Agate Celebi", Shiny = Shiny.Never, Language = (int)LanguageID.Japanese },
            new() { Species = 025, Level = 10, TID = 31121, OT_Gender = 0, Version = GameVersion.R, Method = PIDType.CXD, OT_Name = "コロシアム", CardTitle = "Colosseum Pikachu", Shiny = Shiny.Never, Language = (int)LanguageID.Japanese },

            new() { Species = 251, Level = 10, TID = 31121, OT_Gender = 1, Version = GameVersion.R, Method = PIDType.CXD, OT_Name = "AGATE", CardTitle = "Agate Celebi", Shiny = Shiny.Never, Language = (int)LanguageID.English, NotDistributed = true },
            new() { Species = 025, Level = 10, TID = 31121, OT_Gender = 0, Version = GameVersion.R, Method = PIDType.CXD, OT_Name = "COLOS", CardTitle = "Colosseum Pikachu", Shiny = Shiny.Never, Language = (int)LanguageID.English, NotDistributed = true },

            new() { Species = 385, Level = 05, TID = 20043, OT_Gender = 0, Version = GameVersion.R, Method = PIDType.BACD_R, OT_Name = "WISHMKR", CardTitle = "Wishmaker Jirachi", Language = (int)LanguageID.English },
        };

        private static IEnumerable<WC3> GetIngameCXDData()
        {
            var langs = new[]{LanguageID.Japanese, LanguageID.English, LanguageID.French, LanguageID.Italian, LanguageID.German, LanguageID.Spanish};
            string[] h = {string.Empty, "ダニー", "HORDEL", "VOLKER", "ODINO", "HORAZ", string.Empty, "HORDEL"};
            string[] d = {string.Empty, "ギンザル", "DUKING", "DOKING", "RODRIGO", "GRAND", string.Empty, "GERMÁN"};
            string[] m = {string.Empty, "バトルやま", "MATTLE", "MT BATAILL", "MONTE LOTT", "DUELLBERG﻿", string.Empty, "ERNESTO"}; // truncated on ck3->pk3 transfer

            return langs.SelectMany(l => GetIngame((int)l));
            IEnumerable<WC3> GetIngame(int l)
            {
                var id = (LanguageID) l;
                return new WC3[]
                {
                    new() { Species = 239, Level = 20, Language = l, Location = 164, TID = 41400, SID = -1, OT_Gender = 0, OT_Name = h[l], Version = GameVersion.XD,   CardTitle = $"Trade Togepi ({id})",     Method = PIDType.CXD, Moves = new[] { 008, 007, 009, 238 }, Fateful = true }, // Elekid @ Snagem Hideout
                    new() { Species = 307, Level = 20, Language = l, Location = 116, TID = 37149, SID = -1, OT_Gender = 0, OT_Name = d[l], Version = GameVersion.XD,   CardTitle = $"Trade Trapinch ({id})",   Method = PIDType.CXD, Moves = new[] { 223, 093, 247, 197 }, Fateful = true }, // Meditite @ Pyrite Town
                    new() { Species = 213, Level = 20, Language = l, Location = 116, TID = 37149, SID = -1, OT_Gender = 0, OT_Name = d[l], Version = GameVersion.XD,   CardTitle = $"Trade Surskit ({id})",    Method = PIDType.CXD, Moves = new[] { 092, 164, 188, 227 }, Fateful = true }, // Shuckle @ Pyrite Town
                    new() { Species = 246, Level = 20, Language = l, Location = 116, TID = 37149, SID = -1, OT_Gender = 0, OT_Name = d[l], Version = GameVersion.XD,   CardTitle = $"Trade Wooper ({id})",     Method = PIDType.CXD, Moves = new[] { 201, 349, 044, 200 }, Fateful = true }, // Larvitar @ Pyrite Town
                    new() { Species = 311, Level = 13, Language = l, Location = 254, TID = 37149, SID =  0, OT_Gender = 0, OT_Name = d[l], Version = GameVersion.COLO, CardTitle = $"Special Gift ({id})",     Method = PIDType.CXD, Moves = new[] { 045, 086, 098, 270 }, Shiny = Shiny.Never }, // Plusle @ Ingame Trade
                    new() { Species = 250, Level = 70, Language = l, Location = 255, TID = 10048, SID =  0, OT_Gender = 0, OT_Name = m[l], Version = GameVersion.S,    CardTitle = $"Mt. Battle Ho-Oh ({id})", Method = PIDType.CXD, Moves = new[] { 105, 126, 241, 129 }, Shiny = Shiny.Never }, // Ho-oh @ Mt. Battle
                };
            }
        }

        internal static readonly WC3[] Encounter_Event3 = Encounter_Event3_Special.Concat(GetIngameCXDData()).ToArray();

        internal static readonly WC3[] Encounter_Event3_FRLG =
        {
            // PCJP - Egg Pokémon Present Eggs (March 21 to April 4, 2004)
            new() { Species = 043, IsEgg = true, Fateful = true, Level = 05, TID = -1, SID = -1, Version = GameVersion.FRLG, Method = PIDType.Method_2, Moves = new[]{073} }, // Oddish with Leech Seed
            new() { Species = 052, IsEgg = true, Fateful = true, Level = 05, TID = -1, SID = -1, Version = GameVersion.FRLG, Method = PIDType.Method_2, Moves = new[]{080} }, // Meowth with Petal Dance
            new() { Species = 060, IsEgg = true, Fateful = true, Level = 05, TID = -1, SID = -1, Version = GameVersion.FRLG, Method = PIDType.Method_2, Moves = new[]{186} }, // Poliwag with Sweet Kiss
            new() { Species = 069, IsEgg = true, Fateful = true, Level = 05, TID = -1, SID = -1, Version = GameVersion.FRLG, Method = PIDType.Method_2, Moves = new[]{298} }, // Bellsprout with Teeter Dance

            // PCNY - Wish Eggs (December 16, 2004, to January 2, 2005)
            new() { Species = 083, IsEgg = true, Fateful = true, Level = 05, TID = -1, SID = -1, Version = GameVersion.FRLG, Method = PIDType.Method_2, Moves = new[]{273, 281} }, // Farfetch'd with Wish & Yawn
            new() { Species = 096, IsEgg = true, Fateful = true, Level = 05, TID = -1, SID = -1, Version = GameVersion.FRLG, Method = PIDType.Method_2, Moves = new[]{273, 187} }, // Drowzee with Wish & Belly Drum
            new() { Species = 102, IsEgg = true, Fateful = true, Level = 05, TID = -1, SID = -1, Version = GameVersion.FRLG, Method = PIDType.Method_2, Moves = new[]{273, 230} }, // Exeggcute with Wish & Sweet Scent
            new() { Species = 108, IsEgg = true, Fateful = true, Level = 05, TID = -1, SID = -1, Version = GameVersion.FRLG, Method = PIDType.Method_2, Moves = new[]{273, 215} }, // Lickitung with Wish & Heal Bell
            new() { Species = 113, IsEgg = true, Fateful = true, Level = 05, TID = -1, SID = -1, Version = GameVersion.FRLG, Method = PIDType.Method_2, Moves = new[]{273, 230} }, // Chansey with Wish & Sweet Scent
            new() { Species = 115, IsEgg = true, Fateful = true, Level = 05, TID = -1, SID = -1, Version = GameVersion.FRLG, Method = PIDType.Method_2, Moves = new[]{273, 281} }, // Kangaskhan with Wish & Yawn

            // PokePark Eggs - Wondercard
            new() { Species = 054, IsEgg = true, Fateful = true, Level = 05, TID = -1, SID = -1, Version = GameVersion.FRLG, Moves = new[]{300}, Method = PIDType.Method_2 }, // Psyduck with Mud Sport
            new() { Species = 172, IsEgg = true, Fateful = true, Level = 05, TID = -1, SID = -1, Version = GameVersion.FRLG, Moves = new[]{266}, Method = PIDType.Method_2 }, // Pichu with Follow me
            new() { Species = 174, IsEgg = true, Fateful = true, Level = 05, TID = -1, SID = -1, Version = GameVersion.FRLG, Moves = new[]{321}, Method = PIDType.Method_2 }, // Igglybuff with Tickle
            new() { Species = 222, IsEgg = true, Fateful = true, Level = 05, TID = -1, SID = -1, Version = GameVersion.FRLG, Moves = new[]{300}, Method = PIDType.Method_2 }, // Corsola with Mud Sport
            new() { Species = 276, IsEgg = true, Fateful = true, Level = 05, TID = -1, SID = -1, Version = GameVersion.FRLG, Moves = new[]{297}, Method = PIDType.Method_2 }, // Taillow with Feather Dance
            new() { Species = 283, IsEgg = true, Fateful = true, Level = 05, TID = -1, SID = -1, Version = GameVersion.FRLG, Moves = new[]{300}, Method = PIDType.Method_2 }, // Surskit with Mud Sport
            new() { Species = 293, IsEgg = true, Fateful = true, Level = 05, TID = -1, SID = -1, Version = GameVersion.FRLG, Moves = new[]{298}, Method = PIDType.Method_2 }, // Whismur with Teeter Dance
            new() { Species = 300, IsEgg = true, Fateful = true, Level = 05, TID = -1, SID = -1, Version = GameVersion.FRLG, Moves = new[]{205}, Method = PIDType.Method_2 }, // Skitty with Rollout
            new() { Species = 311, IsEgg = true, Fateful = true, Level = 05, TID = -1, SID = -1, Version = GameVersion.FRLG, Moves = new[]{346}, Method = PIDType.Method_2 }, // Plusle with Water Sport
            new() { Species = 312, IsEgg = true, Fateful = true, Level = 05, TID = -1, SID = -1, Version = GameVersion.FRLG, Moves = new[]{300}, Method = PIDType.Method_2 }, // Minun with Mud Sport
            new() { Species = 325, IsEgg = true, Fateful = true, Level = 05, TID = -1, SID = -1, Version = GameVersion.FRLG, Moves = new[]{253}, Method = PIDType.Method_2 }, // Spoink with Uproar
            new() { Species = 327, IsEgg = true, Fateful = true, Level = 05, TID = -1, SID = -1, Version = GameVersion.FRLG, Moves = new[]{047}, Method = PIDType.Method_2 }, // Spinda with Sing
            new() { Species = 331, IsEgg = true, Fateful = true, Level = 05, TID = -1, SID = -1, Version = GameVersion.FRLG, Moves = new[]{227}, Method = PIDType.Method_2 }, // Cacnea with Encore
            new() { Species = 341, IsEgg = true, Fateful = true, Level = 05, TID = -1, SID = -1, Version = GameVersion.FRLG, Moves = new[]{346}, Method = PIDType.Method_2 }, // Corphish with Water Sport
            new() { Species = 360, IsEgg = true, Fateful = true, Level = 05, TID = -1, SID = -1, Version = GameVersion.FRLG, Moves = new[]{321}, Method = PIDType.Method_2 }, // Wynaut with Tickle
        };

        internal static readonly WC3[] Encounter_Event3_RS =
        {
            // PCJP - Pokémon Center 5th Anniversary Eggs (April 25 to May 18, 2003)
            new() { Species = 172, IsEgg = true, Level = 05, OT_Name = "オヤＮＡＭＥ", TID = -1, SID = -1, Version = GameVersion.R, Method = PIDType.BACD_R, Moves = new[]{298} }, // Pichu with Teeter Dance
            new() { Species = 172, IsEgg = true, Level = 05, OT_Name = "オヤＮＡＭＥ", TID = -1, SID = -1, Version = GameVersion.R, Method = PIDType.BACD_R, Moves = new[]{273} }, // Pichu with Wish
            new() { Species = 172, IsEgg = true, Level = 05, OT_Name = "オヤＮＡＭＥ", TID = -1, SID = -1, Version = GameVersion.R, Method = PIDType.BACD_R_S, Moves = new[]{298} }, // Pichu with Teeter Dance
            new() { Species = 172, IsEgg = true, Level = 05, OT_Name = "オヤＮＡＭＥ", TID = -1, SID = -1, Version = GameVersion.R, Method = PIDType.BACD_R_S, Moves = new[]{273} }, // Pichu with Wish
            new() { Species = 280, IsEgg = true, Level = 05, OT_Name = "オヤＮＡＭＥ", TID = -1, SID = -1, Version = GameVersion.R, Method = PIDType.BACD_R, Moves = new[]{204 } }, // Ralts with Charm
            new() { Species = 280, IsEgg = true, Level = 05, OT_Name = "オヤＮＡＭＥ", TID = -1, SID = -1, Version = GameVersion.R, Method = PIDType.BACD_R, Moves = new[]{273} }, // Ralts with Wish
            new() { Species = 359, IsEgg = true, Level = 05, OT_Name = "オヤＮＡＭＥ", TID = -1, SID = -1, Version = GameVersion.R, Method = PIDType.BACD_R, Moves = new[]{180} }, // Absol with Spite
            new() { Species = 359, IsEgg = true, Level = 05, OT_Name = "オヤＮＡＭＥ", TID = -1, SID = -1, Version = GameVersion.R, Method = PIDType.BACD_R, Moves = new[]{273} }, // Absol with Wish
            new() { Species = 371, IsEgg = true, Level = 05, OT_Name = "オヤＮＡＭＥ", TID = -1, SID = -1, Version = GameVersion.R, Method = PIDType.BACD_R, Moves = new[]{334} }, // Bagon with Iron Defense
            new() { Species = 371, IsEgg = true, Level = 05, OT_Name = "オヤＮＡＭＥ", TID = -1, SID = -1, Version = GameVersion.R, Method = PIDType.BACD_R, Moves = new[]{273} }, // Bagon with Wish

            // Negai Boshi Jirachi
            new() { Species = 385, Level = 05, TID = 30719, OT_Gender = 0, OT_Name = "ネガイボシ", Version = GameVersion.R, Method = PIDType.BACD_R, Language = (int)LanguageID.Japanese, Shiny = Shiny.Never },
            new() { Species = 385, Level = 05, TID = 30719, OT_Name = "ネガイボシ", Version = GameVersion.RS, Method = PIDType.BACD_U_AX, Language = (int)LanguageID.Japanese, Shiny = Shiny.Never },

            // Berry Glitch Fix
            // PCJP - (December 29, 2003 to March 31, 2004)
            new() { Species = 263, Level = 5, Version = GameVersion.S, Language = (int)LanguageID.Japanese, Method = PIDType.BACD_R_S, TID = 21121, OT_Name = "ルビー", OT_Gender = 1, Shiny = Shiny.Always },
            new() { Species = 263, Level = 5, Version = GameVersion.S, Language = (int)LanguageID.Japanese, Method = PIDType.BACD_R_S, TID = 21121, OT_Name = "サファイア", OT_Gender = 0, Shiny = Shiny.Always },

            // EBGames/GameStop (March 1, 2004 to April 22, 2007), also via multi-game discs
            new() { Species = 263, Level = 5, Version = GameVersion.S, Language = (int)LanguageID.English, Method = PIDType.BACD_R_S, TID = 30317, OT_Name = "RUBY", OT_Gender = 1 },
            new() { Species = 263, Level = 5, Version = GameVersion.S, Language = (int)LanguageID.English, Method = PIDType.BACD_R_S, TID = 30317, OT_Name = "SAPHIRE", OT_Gender = 0 },

            // Channel Jirachi
            new() { Species = 385, Level = 5, Version = GameVersion.RS, Method = PIDType.Channel, TID = 40122, OT_Gender = 3,SID = -1, OT_Name = "CHANNEL", CardTitle = "Channel Jirachi", Met_Level = 0 },

            // Aura Mew
            new() { Species = 151, Level = 10, Version = GameVersion.R, Language = (int)LanguageID.English, Method = PIDType.BACD_R, TID = 20078, OT_Name = "Aura", Fateful = true, Shiny = Shiny.Never }, // Mew
            new() { Species = 151, Level = 10, Version = GameVersion.R, Language = (int)LanguageID.French,  Method = PIDType.BACD_R, TID = 20078, OT_Name = "Aura", Fateful = true, Shiny = Shiny.Never }, // Mew
            new() { Species = 151, Level = 10, Version = GameVersion.R, Language = (int)LanguageID.Italian, Method = PIDType.BACD_R, TID = 20078, OT_Name = "Aura", Fateful = true, Shiny = Shiny.Never }, // Mew
            new() { Species = 151, Level = 10, Version = GameVersion.R, Language = (int)LanguageID.German,  Method = PIDType.BACD_R, TID = 20078, OT_Name = "Aura", Fateful = true, Shiny = Shiny.Never }, // Mew
            new() { Species = 151, Level = 10, Version = GameVersion.R, Language = (int)LanguageID.Spanish, Method = PIDType.BACD_R, TID = 20078, OT_Name = "Aura", Fateful = true, Shiny = Shiny.Never }, // Mew

            // English Events
            new() { Species = 006, Level = 70, Version = GameVersion.R, Moves = new[] {017,163,082,083}, Language = (int)LanguageID.English, Method = PIDType.BACD_R, TID = 06227, OT_Name = "10ANNIV", Shiny = Shiny.Never }, // Charizard
            new() { Species = 025, Level = 70, Version = GameVersion.R, Moves = new[] {085,097,087,113}, Language = (int)LanguageID.English, Method = PIDType.BACD_R, TID = 06227, OT_Name = "10ANNIV", Shiny = Shiny.Never }, // Pikachu
            new() { Species = 144, Level = 70, Version = GameVersion.R, Moves = new[] {097,170,058,115}, Language = (int)LanguageID.English, Method = PIDType.BACD_R, TID = 06227, OT_Name = "10ANNIV", Shiny = Shiny.Never }, // Articuno
            new() { Species = 243, Level = 70, Version = GameVersion.R, Moves = new[] {098,209,115,242}, Language = (int)LanguageID.English, Method = PIDType.BACD_R, TID = 06227, OT_Name = "10ANNIV", Shiny = Shiny.Never }, // Raikou
            new() { Species = 244, Level = 70, Version = GameVersion.R, Moves = new[] {083,023,053,207}, Language = (int)LanguageID.English, Method = PIDType.BACD_R, TID = 06227, OT_Name = "10ANNIV", Shiny = Shiny.Never }, // Entei
            new() { Species = 245, Level = 70, Version = GameVersion.R, Moves = new[] {016,062,054,243}, Language = (int)LanguageID.English, Method = PIDType.BACD_R, TID = 06227, OT_Name = "10ANNIV", Shiny = Shiny.Never }, // Suicune
            new() { Species = 249, Level = 70, Version = GameVersion.R, Moves = new[] {105,056,240,129}, Language = (int)LanguageID.English, Method = PIDType.BACD_R, TID = 06227, OT_Name = "10ANNIV", Shiny = Shiny.Never }, // Lugia
            new() { Species = 250, Level = 70, Version = GameVersion.R, Moves = new[] {105,126,241,129}, Language = (int)LanguageID.English, Method = PIDType.BACD_R, TID = 06227, OT_Name = "10ANNIV", Shiny = Shiny.Never }, // Ho-Oh
            new() { Species = 380, Level = 70, Version = GameVersion.R, Moves = new[] {296,094,105,204}, Language = (int)LanguageID.English, Method = PIDType.BACD_R, TID = 06227, OT_Name = "10ANNIV", Shiny = Shiny.Never }, // Latias
            new() { Species = 381, Level = 70, Version = GameVersion.R, Moves = new[] {295,094,105,349}, Language = (int)LanguageID.English, Method = PIDType.BACD_R, TID = 06227, OT_Name = "10ANNIV", Shiny = Shiny.Never }, // Latios

            // French
            new() { Species = 006, Level = 70, Version = GameVersion.R, Moves = new[] {017,163,082,083}, Language = (int)LanguageID.French, Method = PIDType.BACD_R, TID = 06227, OT_Name = "10ANNIV", Shiny = Shiny.Never }, // Charizard
            new() { Species = 025, Level = 70, Version = GameVersion.R, Moves = new[] {085,097,087,113}, Language = (int)LanguageID.French, Method = PIDType.BACD_R, TID = 06227, OT_Name = "10ANNIV", Shiny = Shiny.Never }, // Pikachu
            new() { Species = 144, Level = 70, Version = GameVersion.R, Moves = new[] {097,170,058,115}, Language = (int)LanguageID.French, Method = PIDType.BACD_R, TID = 06227, OT_Name = "10ANNIV", Shiny = Shiny.Never }, // Articuno
            new() { Species = 243, Level = 70, Version = GameVersion.R, Moves = new[] {098,209,115,242}, Language = (int)LanguageID.French, Method = PIDType.BACD_R, TID = 06227, OT_Name = "10ANNIV", Shiny = Shiny.Never }, // Raikou
            new() { Species = 244, Level = 70, Version = GameVersion.R, Moves = new[] {083,023,053,207}, Language = (int)LanguageID.French, Method = PIDType.BACD_R, TID = 06227, OT_Name = "10ANNIV", Shiny = Shiny.Never }, // Entei
            new() { Species = 245, Level = 70, Version = GameVersion.R, Moves = new[] {016,062,054,243}, Language = (int)LanguageID.French, Method = PIDType.BACD_R, TID = 06227, OT_Name = "10ANNIV", Shiny = Shiny.Never }, // Suicune
            new() { Species = 249, Level = 70, Version = GameVersion.R, Moves = new[] {105,056,240,129}, Language = (int)LanguageID.French, Method = PIDType.BACD_R, TID = 06227, OT_Name = "10ANNIV", Shiny = Shiny.Never }, // Lugia
            new() { Species = 250, Level = 70, Version = GameVersion.R, Moves = new[] {105,126,241,129}, Language = (int)LanguageID.French, Method = PIDType.BACD_R, TID = 06227, OT_Name = "10ANNIV", Shiny = Shiny.Never }, // Ho-Oh
            new() { Species = 380, Level = 70, Version = GameVersion.R, Moves = new[] {296,094,105,204}, Language = (int)LanguageID.French, Method = PIDType.BACD_R, TID = 06227, OT_Name = "10ANNIV", Shiny = Shiny.Never }, // Latias
            new() { Species = 381, Level = 70, Version = GameVersion.R, Moves = new[] {295,094,105,349}, Language = (int)LanguageID.French, Method = PIDType.BACD_R, TID = 06227, OT_Name = "10ANNIV", Shiny = Shiny.Never }, // Latios

            // Italian
            new() { Species = 006, Level = 70, Version = GameVersion.R, Moves = new[] {017,163,082,083}, Language = (int)LanguageID.Italian, Method = PIDType.BACD_R, TID = 06227, OT_Name = "10ANNI", Shiny = Shiny.Never }, // Charizard
            new() { Species = 025, Level = 70, Version = GameVersion.R, Moves = new[] {085,097,087,113}, Language = (int)LanguageID.Italian, Method = PIDType.BACD_R, TID = 06227, OT_Name = "10ANNI", Shiny = Shiny.Never }, // Pikachu
            new() { Species = 144, Level = 70, Version = GameVersion.R, Moves = new[] {097,170,058,115}, Language = (int)LanguageID.Italian, Method = PIDType.BACD_R, TID = 06227, OT_Name = "10ANNI", Shiny = Shiny.Never }, // Articuno
            new() { Species = 243, Level = 70, Version = GameVersion.R, Moves = new[] {098,209,115,242}, Language = (int)LanguageID.Italian, Method = PIDType.BACD_R, TID = 06227, OT_Name = "10ANNI", Shiny = Shiny.Never }, // Raikou
            new() { Species = 244, Level = 70, Version = GameVersion.R, Moves = new[] {083,023,053,207}, Language = (int)LanguageID.Italian, Method = PIDType.BACD_R, TID = 06227, OT_Name = "10ANNI", Shiny = Shiny.Never }, // Entei
            new() { Species = 245, Level = 70, Version = GameVersion.R, Moves = new[] {016,062,054,243}, Language = (int)LanguageID.Italian, Method = PIDType.BACD_R, TID = 06227, OT_Name = "10ANNI", Shiny = Shiny.Never }, // Suicune
            new() { Species = 249, Level = 70, Version = GameVersion.R, Moves = new[] {105,056,240,129}, Language = (int)LanguageID.Italian, Method = PIDType.BACD_R, TID = 06227, OT_Name = "10ANNI", Shiny = Shiny.Never }, // Lugia
            new() { Species = 250, Level = 70, Version = GameVersion.R, Moves = new[] {105,126,241,129}, Language = (int)LanguageID.Italian, Method = PIDType.BACD_R, TID = 06227, OT_Name = "10ANNI", Shiny = Shiny.Never }, // Ho-Oh
            new() { Species = 380, Level = 70, Version = GameVersion.R, Moves = new[] {296,094,105,204}, Language = (int)LanguageID.Italian, Method = PIDType.BACD_R, TID = 06227, OT_Name = "10ANNI", Shiny = Shiny.Never }, // Latias
            new() { Species = 381, Level = 70, Version = GameVersion.R, Moves = new[] {295,094,105,349}, Language = (int)LanguageID.Italian, Method = PIDType.BACD_R, TID = 06227, OT_Name = "10ANNI", Shiny = Shiny.Never }, // Latios

            // German
            new() { Species = 006, Level = 70, Version = GameVersion.R, Moves = new[] {017,163,082,083}, Language = (int)LanguageID.German, Method = PIDType.BACD_R, TID = 06227, OT_Name = "10JAHRE", Shiny = Shiny.Never }, // Charizard
            new() { Species = 025, Level = 70, Version = GameVersion.R, Moves = new[] {085,097,087,113}, Language = (int)LanguageID.German, Method = PIDType.BACD_R, TID = 06227, OT_Name = "10JAHRE", Shiny = Shiny.Never }, // Pikachu
            new() { Species = 144, Level = 70, Version = GameVersion.R, Moves = new[] {097,170,058,115}, Language = (int)LanguageID.German, Method = PIDType.BACD_R, TID = 06227, OT_Name = "10JAHRE", Shiny = Shiny.Never }, // Articuno
            new() { Species = 243, Level = 70, Version = GameVersion.R, Moves = new[] {098,209,115,242}, Language = (int)LanguageID.German, Method = PIDType.BACD_R, TID = 06227, OT_Name = "10JAHRE", Shiny = Shiny.Never }, // Raikou
            new() { Species = 244, Level = 70, Version = GameVersion.R, Moves = new[] {083,023,053,207}, Language = (int)LanguageID.German, Method = PIDType.BACD_R, TID = 06227, OT_Name = "10JAHRE", Shiny = Shiny.Never }, // Entei
            new() { Species = 245, Level = 70, Version = GameVersion.R, Moves = new[] {016,062,054,243}, Language = (int)LanguageID.German, Method = PIDType.BACD_R, TID = 06227, OT_Name = "10JAHRE", Shiny = Shiny.Never }, // Suicune
            new() { Species = 249, Level = 70, Version = GameVersion.R, Moves = new[] {105,056,240,129}, Language = (int)LanguageID.German, Method = PIDType.BACD_R, TID = 06227, OT_Name = "10JAHRE", Shiny = Shiny.Never }, // Lugia
            new() { Species = 250, Level = 70, Version = GameVersion.R, Moves = new[] {105,126,241,129}, Language = (int)LanguageID.German, Method = PIDType.BACD_R, TID = 06227, OT_Name = "10JAHRE", Shiny = Shiny.Never }, // Ho-Oh
            new() { Species = 380, Level = 70, Version = GameVersion.R, Moves = new[] {296,094,105,204}, Language = (int)LanguageID.German, Method = PIDType.BACD_R, TID = 06227, OT_Name = "10JAHRE", Shiny = Shiny.Never }, // Latias
            new() { Species = 381, Level = 70, Version = GameVersion.R, Moves = new[] {295,094,105,349}, Language = (int)LanguageID.German, Method = PIDType.BACD_R, TID = 06227, OT_Name = "10JAHRE", Shiny = Shiny.Never }, // Latios

            // Spanish
            new() { Species = 006, Level = 70, Version = GameVersion.R, Moves = new[] {017,163,082,083}, Language = (int)LanguageID.Spanish, Method = PIDType.BACD_R, TID = 06227, OT_Name = "10ANIV", Shiny = Shiny.Never }, // Charizard
            new() { Species = 025, Level = 70, Version = GameVersion.R, Moves = new[] {085,097,087,113}, Language = (int)LanguageID.Spanish, Method = PIDType.BACD_R, TID = 06227, OT_Name = "10ANIV", Shiny = Shiny.Never }, // Pikachu
            new() { Species = 144, Level = 70, Version = GameVersion.R, Moves = new[] {097,170,058,115}, Language = (int)LanguageID.Spanish, Method = PIDType.BACD_R, TID = 06227, OT_Name = "10ANIV", Shiny = Shiny.Never }, // Articuno
            new() { Species = 243, Level = 70, Version = GameVersion.R, Moves = new[] {098,209,115,242}, Language = (int)LanguageID.Spanish, Method = PIDType.BACD_R, TID = 06227, OT_Name = "10ANIV", Shiny = Shiny.Never }, // Raikou
            new() { Species = 244, Level = 70, Version = GameVersion.R, Moves = new[] {083,023,053,207}, Language = (int)LanguageID.Spanish, Method = PIDType.BACD_R, TID = 06227, OT_Name = "10ANIV", Shiny = Shiny.Never }, // Entei
            new() { Species = 245, Level = 70, Version = GameVersion.R, Moves = new[] {016,062,054,243}, Language = (int)LanguageID.Spanish, Method = PIDType.BACD_R, TID = 06227, OT_Name = "10ANIV", Shiny = Shiny.Never }, // Suicune
            new() { Species = 249, Level = 70, Version = GameVersion.R, Moves = new[] {105,056,240,129}, Language = (int)LanguageID.Spanish, Method = PIDType.BACD_R, TID = 06227, OT_Name = "10ANIV", Shiny = Shiny.Never }, // Lugia
            new() { Species = 250, Level = 70, Version = GameVersion.R, Moves = new[] {105,126,241,129}, Language = (int)LanguageID.Spanish, Method = PIDType.BACD_R, TID = 06227, OT_Name = "10ANIV", Shiny = Shiny.Never }, // Ho-Oh
            new() { Species = 380, Level = 70, Version = GameVersion.R, Moves = new[] {296,094,105,204}, Language = (int)LanguageID.Spanish, Method = PIDType.BACD_R, TID = 06227, OT_Name = "10ANIV", Shiny = Shiny.Never }, // Latias
            new() { Species = 381, Level = 70, Version = GameVersion.R, Moves = new[] {295,094,105,349}, Language = (int)LanguageID.Spanish, Method = PIDType.BACD_R, TID = 06227, OT_Name = "10ANIV", Shiny = Shiny.Never }, // Latios

            new() { Species = 375, Level = 30, Version = GameVersion.R, Moves = new[] {036,093,232,287}, Language = (int)LanguageID.English, Method = PIDType.BACD_R, TID = 02005, OT_Name = "ROCKS", OT_Gender = 0, RibbonNational = true, Shiny = Shiny.Never }, // Metang
            new() { Species = 386, Level = 70, Version = GameVersion.R, Moves = new[] {322,105,354,063}, Language = (int)LanguageID.English, Method = PIDType.BACD_R, TID = 28606, OT_Name = "DOEL", Fateful = true, Shiny = Shiny.Never }, // Deoxys
            new() { Species = 386, Level = 70, Version = GameVersion.R, Moves = new[] {322,105,354,063}, Language = (int)LanguageID.English, Method = PIDType.BACD_R, TID = 00010, OT_Name = "SPACE C", Fateful = true, Shiny = Shiny.Never }, // Deoxys
            new() { Species = 151, Level = 10, Version = GameVersion.R, Language = (int)LanguageID.English, Method = PIDType.BACD_U, TID = 06930, OT_Name = "MYSTRY", Fateful = true, Shiny = Shiny.Never }, // Mew
            new() { Species = 151, Level = 10, Version = GameVersion.R, Language = (int)LanguageID.English, Method = PIDType.BACD_R, TID = 06930, OT_Name = "MYSTRY", Fateful = true, Shiny = Shiny.Never }, // Mew

            // Party of the Decade
            new() { Species = 001, Level = 70, Version = GameVersion.R, Moves = new[] {230,074,076,235}, Language = (int)LanguageID.English, Method = PIDType.BACD_R, TID = 06808, OT_Name = "10 ANIV", Shiny = Shiny.Never }, // Bulbasaur
            new() { Species = 006, Level = 70, Version = GameVersion.R, Moves = new[] {017,163,082,083}, Language = (int)LanguageID.English, Method = PIDType.BACD_R, TID = 06808, OT_Name = "10 ANIV", Shiny = Shiny.Never }, // Charizard
            new() { Species = 009, Level = 70, Version = GameVersion.R, Moves = new[] {182,240,130,056}, Language = (int)LanguageID.English, Method = PIDType.BACD_R, TID = 06808, OT_Name = "10 ANIV", Shiny = Shiny.Never }, // Blastoise
            new() { Species = 025, Level = 70, Version = GameVersion.R, Moves = new[] {085,087,113,019}, Language = (int)LanguageID.English, Method = PIDType.BACD_R, TID = 06808, OT_Name = "10 ANIV", HeldItem = 202, Shiny = Shiny.Never }, // Pikachu (Fly)
            new() { Species = 065, Level = 70, Version = GameVersion.R, Moves = new[] {248,347,094,271}, Language = (int)LanguageID.English, Method = PIDType.BACD_R, TID = 06808, OT_Name = "10 ANIV", Shiny = Shiny.Never }, // Alakazam
            new() { Species = 144, Level = 70, Version = GameVersion.R, Moves = new[] {097,170,058,115}, Language = (int)LanguageID.English, Method = PIDType.BACD_R, TID = 06808, OT_Name = "10 ANIV", Shiny = Shiny.Never }, // Articuno
            new() { Species = 145, Level = 70, Version = GameVersion.R, Moves = new[] {097,197,065,268}, Language = (int)LanguageID.English, Method = PIDType.BACD_R, TID = 06808, OT_Name = "10 ANIV", Shiny = Shiny.Never }, // Zapdos
            new() { Species = 146, Level = 70, Version = GameVersion.R, Moves = new[] {097,203,053,219}, Language = (int)LanguageID.English, Method = PIDType.BACD_R, TID = 06808, OT_Name = "10 ANIV", Shiny = Shiny.Never }, // Moltres
            new() { Species = 149, Level = 70, Version = GameVersion.R, Moves = new[] {097,219,017,200}, Language = (int)LanguageID.English, Method = PIDType.BACD_R, TID = 06808, OT_Name = "10 ANIV", Shiny = Shiny.Never }, // Dragonite
            new() { Species = 157, Level = 70, Version = GameVersion.R, Moves = new[] {098,172,129,053}, Language = (int)LanguageID.English, Method = PIDType.BACD_R, TID = 06808, OT_Name = "10 ANIV", Shiny = Shiny.Never }, // Typhlosion
            new() { Species = 196, Level = 70, Version = GameVersion.R, Moves = new[] {060,244,094,234}, Language = (int)LanguageID.English, Method = PIDType.BACD_R, TID = 06808, OT_Name = "10 ANIV", Shiny = Shiny.Never }, // Espeon
            new() { Species = 197, Level = 70, Version = GameVersion.R, Moves = new[] {185,212,103,236}, Language = (int)LanguageID.English, Method = PIDType.BACD_R, TID = 06808, OT_Name = "10 ANIV", Shiny = Shiny.Never }, // Umbreon
            new() { Species = 243, Level = 70, Version = GameVersion.R, Moves = new[] {098,209,115,242}, Language = (int)LanguageID.English, Method = PIDType.BACD_R, TID = 06808, OT_Name = "10 ANIV", Shiny = Shiny.Never }, // Raikou
            new() { Species = 244, Level = 70, Version = GameVersion.R, Moves = new[] {083,023,053,207}, Language = (int)LanguageID.English, Method = PIDType.BACD_R, TID = 06808, OT_Name = "10 ANIV", Shiny = Shiny.Never }, // Entei
            new() { Species = 245, Level = 70, Version = GameVersion.R, Moves = new[] {016,062,054,243}, Language = (int)LanguageID.English, Method = PIDType.BACD_R, TID = 06808, OT_Name = "10 ANIV", Shiny = Shiny.Never }, // Suicune
            new() { Species = 248, Level = 70, Version = GameVersion.R, Moves = new[] {037,184,242,089}, Language = (int)LanguageID.English, Method = PIDType.BACD_R, TID = 06808, OT_Name = "10 ANIV", Shiny = Shiny.Never }, // Tyranitar
            new() { Species = 257, Level = 70, Version = GameVersion.R, Moves = new[] {299,163,119,327}, Language = (int)LanguageID.English, Method = PIDType.BACD_R, TID = 06808, OT_Name = "10 ANIV", Shiny = Shiny.Never }, // Blaziken
            new() { Species = 359, Level = 70, Version = GameVersion.R, Moves = new[] {104,163,248,195}, Language = (int)LanguageID.English, Method = PIDType.BACD_R, TID = 06808, OT_Name = "10 ANIV", Shiny = Shiny.Never }, // Absol
            new() { Species = 380, Level = 70, Version = GameVersion.R, Moves = new[] {296,094,105,204}, Language = (int)LanguageID.English, Method = PIDType.BACD_R, TID = 06808, OT_Name = "10 ANIV", HeldItem = 191, Shiny = Shiny.Never }, // Latias
            new() { Species = 381, Level = 70, Version = GameVersion.R, Moves = new[] {295,094,105,349}, Language = (int)LanguageID.English, Method = PIDType.BACD_R, TID = 06808, OT_Name = "10 ANIV", HeldItem = 191, Shiny = Shiny.Never }, // Latios

            // Journey Across America
            new() { Species = 001, Level = 70, Version = GameVersion.R, Moves = new[] {230,074,076,235}, Language = (int)LanguageID.English, Method = PIDType.BACD_R, TID = 00010, OT_Name = "10 ANIV", Shiny = Shiny.Never }, // Bulbasaur
            new() { Species = 006, Level = 70, Version = GameVersion.R, Moves = new[] {017,163,082,083}, Language = (int)LanguageID.English, Method = PIDType.BACD_R, TID = 00010, OT_Name = "10 ANIV", Shiny = Shiny.Never }, // Charizard
            new() { Species = 009, Level = 70, Version = GameVersion.R, Moves = new[] {182,240,130,056}, Language = (int)LanguageID.English, Method = PIDType.BACD_R, TID = 00010, OT_Name = "10 ANIV", Shiny = Shiny.Never }, // Blastoise
            new() { Species = 025, Level = 70, Version = GameVersion.R, Moves = new[] {085,097,087,113}, Language = (int)LanguageID.English, Method = PIDType.BACD_R, TID = 00010, OT_Name = "10 ANIV", HeldItem = 202, Shiny = Shiny.Never }, // Pikachu (No Fly)
            new() { Species = 065, Level = 70, Version = GameVersion.R, Moves = new[] {248,347,094,271}, Language = (int)LanguageID.English, Method = PIDType.BACD_R, TID = 00010, OT_Name = "10 ANIV", Shiny = Shiny.Never }, // Alakazam
            new() { Species = 144, Level = 70, Version = GameVersion.R, Moves = new[] {097,170,058,115}, Language = (int)LanguageID.English, Method = PIDType.BACD_R, TID = 00010, OT_Name = "10 ANIV", Shiny = Shiny.Never }, // Articuno
            new() { Species = 145, Level = 70, Version = GameVersion.R, Moves = new[] {097,197,065,268}, Language = (int)LanguageID.English, Method = PIDType.BACD_R, TID = 00010, OT_Name = "10 ANIV", Shiny = Shiny.Never }, // Zapdos
            new() { Species = 146, Level = 70, Version = GameVersion.R, Moves = new[] {097,203,053,219}, Language = (int)LanguageID.English, Method = PIDType.BACD_R, TID = 00010, OT_Name = "10 ANIV", Shiny = Shiny.Never }, // Moltres
            new() { Species = 149, Level = 70, Version = GameVersion.R, Moves = new[] {097,219,017,200}, Language = (int)LanguageID.English, Method = PIDType.BACD_R, TID = 00010, OT_Name = "10 ANIV", Shiny = Shiny.Never }, // Dragonite
            new() { Species = 157, Level = 70, Version = GameVersion.R, Moves = new[] {098,172,129,053}, Language = (int)LanguageID.English, Method = PIDType.BACD_R, TID = 00010, OT_Name = "10 ANIV", Shiny = Shiny.Never }, // Typhlosion
            new() { Species = 196, Level = 70, Version = GameVersion.R, Moves = new[] {060,244,094,234}, Language = (int)LanguageID.English, Method = PIDType.BACD_R, TID = 00010, OT_Name = "10 ANIV", Shiny = Shiny.Never }, // Espeon
            new() { Species = 197, Level = 70, Version = GameVersion.R, Moves = new[] {185,212,103,236}, Language = (int)LanguageID.English, Method = PIDType.BACD_R, TID = 00010, OT_Name = "10 ANIV", Shiny = Shiny.Never }, // Umbreon
            new() { Species = 243, Level = 70, Version = GameVersion.R, Moves = new[] {098,209,115,242}, Language = (int)LanguageID.English, Method = PIDType.BACD_R, TID = 00010, OT_Name = "10 ANIV", Shiny = Shiny.Never }, // Raikou
            new() { Species = 244, Level = 70, Version = GameVersion.R, Moves = new[] {083,023,053,207}, Language = (int)LanguageID.English, Method = PIDType.BACD_R, TID = 00010, OT_Name = "10 ANIV", Shiny = Shiny.Never }, // Entei
            new() { Species = 245, Level = 70, Version = GameVersion.R, Moves = new[] {016,062,054,243}, Language = (int)LanguageID.English, Method = PIDType.BACD_R, TID = 00010, OT_Name = "10 ANIV", Shiny = Shiny.Never }, // Suicune
            new() { Species = 248, Level = 70, Version = GameVersion.R, Moves = new[] {037,184,242,089}, Language = (int)LanguageID.English, Method = PIDType.BACD_R, TID = 00010, OT_Name = "10 ANIV", Shiny = Shiny.Never }, // Tyranitar
            new() { Species = 251, Level = 70, Version = GameVersion.R, Moves = new[] {246,248,226,195}, Language = (int)LanguageID.English, Method = PIDType.BACD_R, TID = 00010, OT_Name = "10 ANIV", Shiny = Shiny.Never }, // Celebi
            new() { Species = 257, Level = 70, Version = GameVersion.R, Moves = new[] {299,163,119,327}, Language = (int)LanguageID.English, Method = PIDType.BACD_R, TID = 00010, OT_Name = "10 ANIV", Shiny = Shiny.Never }, // Blaziken
            new() { Species = 359, Level = 70, Version = GameVersion.R, Moves = new[] {104,163,248,195}, Language = (int)LanguageID.English, Method = PIDType.BACD_R, TID = 00010, OT_Name = "10 ANIV", Shiny = Shiny.Never }, // Absol
            new() { Species = 380, Level = 70, Version = GameVersion.R, Moves = new[] {296,094,105,204}, Language = (int)LanguageID.English, Method = PIDType.BACD_R, TID = 00010, OT_Name = "10 ANIV", HeldItem = 191, Shiny = Shiny.Never }, // Latias
            new() { Species = 381, Level = 70, Version = GameVersion.R, Moves = new[] {295,094,105,349}, Language = (int)LanguageID.English, Method = PIDType.BACD_R, TID = 00010, OT_Name = "10 ANIV", HeldItem = 191, Shiny = Shiny.Never }, // Latios
        };

        internal static readonly WC3[] Encounter_Event3_Common =
        {
            // Pokémon Box -- RSE Recipient
            new() { Species = 333, IsEgg = true, Level = 05, Moves = new[]{206}, Method = PIDType.BACD_U, OT_Gender = 1, OT_Name = "ＡＺＵＳＡ", Version = GameVersion.RSE }, // Swablu Egg with False Swipe
            new() { Species = 263, IsEgg = true, Level = 05, Moves = new[]{245}, Method = PIDType.BACD_U, OT_Gender = 1, OT_Name = "ＡＺＵＳＡ", Version = GameVersion.RSE }, // Zigzagoon Egg with Extreme Speed
            new() { Species = 300, IsEgg = true, Level = 05, Moves = new[]{006}, Method = PIDType.BACD_U, OT_Gender = 1, OT_Name = "ＡＺＵＳＡ", Version = GameVersion.RSE }, // Skitty Egg with Pay Day
            new() { Species = 172, IsEgg = true, Level = 05, Moves = new[]{057}, Method = PIDType.BACD_U, OT_Gender = 1, OT_Name = "ＡＺＵＳＡ", Version = GameVersion.RSE }, // Pichu Egg with Surf
            // Pokémon Box -- FRLG Recipient
            new() { Species = 333, IsEgg = true, Level = 05, Moves = new[]{206}, Method = PIDType.BACD_U, OT_Gender = 1, OT_Name = "ＡＺＵＳＡ", Version = GameVersion.FRLG }, // Swablu Egg with False Swipe
            new() { Species = 263, IsEgg = true, Level = 05, Moves = new[]{245}, Method = PIDType.BACD_U, OT_Gender = 1, OT_Name = "ＡＺＵＳＡ", Version = GameVersion.FRLG }, // Zigzagoon Egg with Extreme Speed
            new() { Species = 300, IsEgg = true, Level = 05, Moves = new[]{006}, Method = PIDType.BACD_U, OT_Gender = 1, OT_Name = "ＡＺＵＳＡ", Version = GameVersion.FRLG }, // Skitty Egg with Pay Day
            new() { Species = 172, IsEgg = true, Level = 05, Moves = new[]{057}, Method = PIDType.BACD_U, OT_Gender = 1, OT_Name = "ＡＺＵＳＡ", Version = GameVersion.FRLG }, // Pichu Egg with Surf

            // PokePark Eggs - DS Download Play
            new() { Species = 054, IsEgg = true, Level = 05, Met_Level = 05, TID = 50318, OT_Gender = 0, OT_Name = "ポケパーク", Version = GameVersion.R, Moves = new[]{300}, Method = PIDType.BACD_R }, // Psyduck with Mud Sport
            new() { Species = 172, IsEgg = true, Level = 05, Met_Level = 05, TID = 50318, OT_Gender = 0, OT_Name = "ポケパーク", Version = GameVersion.R, Moves = new[]{266}, Method = PIDType.BACD_R }, // Pichu with Follow me
            new() { Species = 174, IsEgg = true, Level = 05, Met_Level = 05, TID = 50318, OT_Gender = 0, OT_Name = "ポケパーク", Version = GameVersion.R, Moves = new[]{321}, Method = PIDType.BACD_R }, // Igglybuff with Tickle
            new() { Species = 222, IsEgg = true, Level = 05, Met_Level = 05, TID = 50318, OT_Gender = 0, OT_Name = "ポケパーク", Version = GameVersion.R, Moves = new[]{300}, Method = PIDType.BACD_R }, // Corsola with Mud Sport
            new() { Species = 276, IsEgg = true, Level = 05, Met_Level = 05, TID = 50318, OT_Gender = 0, OT_Name = "ポケパーク", Version = GameVersion.R, Moves = new[]{297}, Method = PIDType.BACD_R }, // Taillow with Feather Dance
            new() { Species = 283, IsEgg = true, Level = 05, Met_Level = 05, TID = 50318, OT_Gender = 0, OT_Name = "ポケパーク", Version = GameVersion.R, Moves = new[]{300}, Method = PIDType.BACD_R }, // Surskit with Mud Sport
            new() { Species = 293, IsEgg = true, Level = 05, Met_Level = 05, TID = 50318, OT_Gender = 0, OT_Name = "ポケパーク", Version = GameVersion.R, Moves = new[]{298}, Method = PIDType.BACD_R }, // Whismur with Teeter Dance
            new() { Species = 300, IsEgg = true, Level = 05, Met_Level = 05, TID = 50318, OT_Gender = 0, OT_Name = "ポケパーク", Version = GameVersion.R, Moves = new[]{205}, Method = PIDType.BACD_R }, // Skitty with Rollout
            new() { Species = 311, IsEgg = true, Level = 05, Met_Level = 05, TID = 50318, OT_Gender = 0, OT_Name = "ポケパーク", Version = GameVersion.R, Moves = new[]{346}, Method = PIDType.BACD_R }, // Plusle with Water Sport
            new() { Species = 312, IsEgg = true, Level = 05, Met_Level = 05, TID = 50318, OT_Gender = 0, OT_Name = "ポケパーク", Version = GameVersion.R, Moves = new[]{300}, Method = PIDType.BACD_R }, // Minun with Mud Sport
            new() { Species = 325, IsEgg = true, Level = 05, Met_Level = 05, TID = 50318, OT_Gender = 0, OT_Name = "ポケパーク", Version = GameVersion.R, Moves = new[]{253}, Method = PIDType.BACD_R }, // Spoink with Uproar
            new() { Species = 327, IsEgg = true, Level = 05, Met_Level = 05, TID = 50318, OT_Gender = 0, OT_Name = "ポケパーク", Version = GameVersion.R, Moves = new[]{047}, Method = PIDType.BACD_R }, // Spinda with Sing
            new() { Species = 331, IsEgg = true, Level = 05, Met_Level = 05, TID = 50318, OT_Gender = 0, OT_Name = "ポケパーク", Version = GameVersion.R, Moves = new[]{227}, Method = PIDType.BACD_R }, // Cacnea with Encore
            new() { Species = 341, IsEgg = true, Level = 05, Met_Level = 05, TID = 50318, OT_Gender = 0, OT_Name = "ポケパーク", Version = GameVersion.R, Moves = new[]{346}, Method = PIDType.BACD_R }, // Corphish with Water Sport
            new() { Species = 360, IsEgg = true, Level = 05, Met_Level = 05, TID = 50318, OT_Gender = 0, OT_Name = "ポケパーク", Version = GameVersion.R, Moves = new[]{321}, Method = PIDType.BACD_R }, // Wynaut with Tickle
        };

        internal static readonly WC3[] Encounter_WC3 = ArrayUtil.ConcatAll(Encounter_Event3, Encounter_Event3_RS, Encounter_Event3_FRLG, Encounter_Event3_Common);
    }
}
