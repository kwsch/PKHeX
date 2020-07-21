using System.Collections.Generic;
using System.Linq;
using static PKHeX.Core.EncounterUtil;
using static PKHeX.Core.Encounters3Teams;

namespace PKHeX.Core
{
    /// <summary>
    /// Generation 3 Encounters
    /// </summary>
    internal static class Encounters3
    {
        internal static readonly EncounterArea3[] SlotsR, SlotsS, SlotsE;
        internal static readonly EncounterArea3[] SlotsFR, SlotsLG;
        internal static readonly EncounterStatic[] StaticR, StaticS, StaticE;
        internal static readonly EncounterStatic[] StaticFR, StaticLG;

        private const int SafariLocation_RSE = 57;
        private const int SafariLocation_FRLG = 136;

        static Encounters3()
        {
            StaticR = GetStaticEncounters(Encounter_RSE, GameVersion.R);
            StaticS = GetStaticEncounters(Encounter_RSE, GameVersion.S);
            StaticE = GetStaticEncounters(Encounter_RSE, GameVersion.E);
            StaticFR = GetStaticEncounters(Encounter_FRLG, GameVersion.FR);
            StaticLG = GetStaticEncounters(Encounter_FRLG, GameVersion.LG);

            static EncounterArea3[] get(string resource, string ident)
                => EncounterArea3.GetArray3(BinLinker.Unpack(Util.GetBinaryResource($"encounter_{resource}.pkl"), ident));

            var R_Slots = get("r", "ru");
            var S_Slots = get("s", "sa");
            var E_Slots = get("e", "em");
            var FR_Slots = get("fr", "fr");
            var LG_Slots = get("lg", "lg");

            MarkEncounterAreaArray(SlotsRSEAlt, SlotsFRLGUnown, SlotsXD);

            ReduceAreasSize(ref R_Slots);
            ReduceAreasSize(ref S_Slots);
            ReduceAreasSize(ref E_Slots);
            MarkG3Slots_FRLG(ref FR_Slots);
            MarkG3Slots_FRLG(ref LG_Slots);

            MarkG3SlotsSafariZones(ref R_Slots, SafariLocation_RSE);
            MarkG3SlotsSafariZones(ref S_Slots, SafariLocation_RSE);
            MarkG3SlotsSafariZones(ref E_Slots, SafariLocation_RSE);
            MarkG3SlotsSafariZones(ref FR_Slots, SafariLocation_FRLG);
            MarkG3SlotsSafariZones(ref LG_Slots, SafariLocation_FRLG);

            MarkEncountersStaticMagnetPull(E_Slots, PersonalTable.SM);

            SlotsR = AddExtraTableSlots(R_Slots, SlotsRSEAlt);
            SlotsS = AddExtraTableSlots(S_Slots, SlotsRSEAlt);
            SlotsE = AddExtraTableSlots(E_Slots, SlotsRSEAlt);
            SlotsFR = AddExtraTableSlots(FR_Slots, SlotsFRLGUnown);
            SlotsLG = AddExtraTableSlots(LG_Slots, SlotsFRLGUnown);

            MarkEncountersGeneration(3, SlotsR, SlotsS, SlotsE, SlotsFR, SlotsLG, SlotsXD);
            MarkEncountersGeneration(3, StaticR, StaticS, StaticE, StaticFR, StaticLG, Encounter_CXD, TradeGift_RSE, TradeGift_FRLG);

            MarkEncounterTradeStrings(TradeGift_RSE, TradeRSE);
            MarkEncounterTradeStrings(TradeGift_FRLG, TradeFRLG);

            SlotsRSEAlt.SetVersion(GameVersion.RSE);
            SlotsFRLGUnown.SetVersion(GameVersion.FRLG);
            SlotsR.SetVersion(GameVersion.R);
            SlotsS.SetVersion(GameVersion.S);
            SlotsE.SetVersion(GameVersion.E);
            SlotsFR.SetVersion(GameVersion.FR);
            SlotsLG.SetVersion(GameVersion.LG);
            Encounter_RSE.SetVersion(GameVersion.RSE);
            Encounter_FRLG.SetVersion(GameVersion.FRLG);
            TradeGift_RSE.SetVersion(GameVersion.RSE);
            TradeGift_FRLG.SetVersion(GameVersion.FRLG);
            Encounter_Colo.SetVersion(GameVersion.COLO);
            Encounter_XD.SetVersion(GameVersion.XD);
            SlotsXD.SetVersion(GameVersion.XD);
        }

        private static void MarkG3Slots_FRLG(ref EncounterArea3[] Areas)
        {
            // Remove slots for unown, those slots does not contains alt form info, it will be added manually in SlotsRFLGAlt
            // Group areas by location id, the raw data have areas with different slots but the same location id
            Areas = Areas.Where(a => a.Location < 188 || a.Location > 194).GroupBy(a => a.Location).Select(a => new EncounterArea3
            {
                Location = a.First().Location,
                Slots = a.SelectMany(m => m.Slots).ToArray()
            }).ToArray();
        }

        private static void MarkG3SlotsSafariZones(ref EncounterArea3[] Areas, int location)
        {
            foreach (var Area in Areas.Where(a => a.Location == location))
            {
                foreach (EncounterSlot Slot in Area.Slots)
                    Slot.Type |= SlotType.Safari;
            }
        }

        private static readonly int[] Roaming_MetLocation_FRLG =
        {
            //Route 1-25 encounter is possible either in grass or on water
            101,102,103,104,105,106,107,108,109,110,
            111,112,113,114,115,116,117,118,119,120,
            121,122,123,124,125
        };

        private static readonly int[] Roaming_MetLocation_RSE =
        {
            //Roaming encounter is possible in tall grass and on water
            //Route 101-138
            16, 17, 18, 19, 20, 21, 22, 23, 24, 25,
            26, 27, 28, 29, 30, 31, 32, 33, 34, 35,
            36, 37, 38, 39, 40, 41, 42, 43, 44, 45,
            46, 47, 48, 49,
        };

        private static readonly EncounterStatic[] Encounter_RSE_Roam =
        {
            new EncounterStatic { Species = 380, Level = 40, Version = GameVersion.S, Roaming = true }, // Latias
            new EncounterStatic { Species = 380, Level = 40, Version = GameVersion.E, Roaming = true }, // Latias
            new EncounterStatic { Species = 381, Level = 40, Version = GameVersion.R, Roaming = true }, // Latios
            new EncounterStatic { Species = 381, Level = 40, Version = GameVersion.E, Roaming = true }, // Latios
        };

        private static readonly EncounterStatic[] Encounter_RSE_Regular =
        {
            // Starters
            new EncounterStatic { Gift = true, Species = 152, Level = 05, Location = 000, Version = GameVersion.E, }, // Chikorita @ Littleroot Town
            new EncounterStatic { Gift = true, Species = 155, Level = 05, Location = 000, Version = GameVersion.E, }, // Cyndaquil
            new EncounterStatic { Gift = true, Species = 158, Level = 05, Location = 000, Version = GameVersion.E, }, // Totodile
            new EncounterStatic { Gift = true, Species = 252, Level = 05, Location = 016, }, // Treecko @ Route 101
            new EncounterStatic { Gift = true, Species = 255, Level = 05, Location = 016, }, // Torchic
            new EncounterStatic { Gift = true, Species = 258, Level = 05, Location = 016, }, // Mudkip

            // Fossil @ Rustboro City
            new EncounterStatic { Gift = true, Species = 345, Level = 20, Location = 010, }, // Lileep
            new EncounterStatic { Gift = true, Species = 347, Level = 20, Location = 010, }, // Anorith

            // Gift
            new EncounterStatic { Gift = true, Species = 351, Level = 25, Location = 034, }, // Castform @ Weather Institute
            new EncounterStatic { Gift = true, Species = 374, Level = 05, Location = 013, }, // Beldum @ Mossdeep City
            new EncounterStatic { Gift = true, Species = 360, Level = 05, EggLocation = 253}, // Wynaut Egg

            // Stationary
            new EncounterStatic { Species = 352, Level = 30, Location = 034, }, // Kecleon @ Route 119
            new EncounterStatic { Species = 352, Level = 30, Location = 035, }, // Kecleon @ Route 120
            new EncounterStatic { Species = 101, Level = 30, Location = 066, Version = GameVersion.RS, }, // Electrode @ Hideout (R:Magma Hideout/S:Aqua Hideout)
            new EncounterStatic { Species = 101, Level = 30, Location = 197, Version = GameVersion.E,  }, // Electrode @ Aqua Hideout
            new EncounterStatic { Species = 185, Level = 40, Location = 058, Version = GameVersion.E,  }, // Sudowoodo @ Battle Frontier

            // Stationary Lengendary
            new EncounterStatic { Species = 377, Level = 40, Location = 082, }, // Regirock @ Desert Ruins
            new EncounterStatic { Species = 378, Level = 40, Location = 081, }, // Regice @ Island Cave
            new EncounterStatic { Species = 379, Level = 40, Location = 083, }, // Registeel @ Ancient Tomb
            new EncounterStatic { Species = 380, Level = 50, Location = 073, Version = GameVersion.R }, // Latias @ Southern Island
            new EncounterStatic { Species = 380, Level = 50, Location = 073, Version = GameVersion.E, Fateful = true }, // Latias @ Southern Island
            new EncounterStatic { Species = 381, Level = 50, Location = 073, Version = GameVersion.S }, // Latios @ Southern Island
            new EncounterStatic { Species = 381, Level = 50, Location = 073, Version = GameVersion.E, Fateful = true }, // Latios @ Southern Island
            new EncounterStatic { Species = 382, Level = 45, Location = 072, Version = GameVersion.S, }, // Kyogre @ Cave of Origin
            new EncounterStatic { Species = 382, Level = 70, Location = 203, Version = GameVersion.E, }, // Kyogre @ Marine Cave
            new EncounterStatic { Species = 383, Level = 45, Location = 072, Version = GameVersion.R, }, // Groudon @ Cave of Origin
            new EncounterStatic { Species = 383, Level = 70, Location = 205, Version = GameVersion.E, }, // Groudon @ Terra Cave
            new EncounterStatic { Species = 384, Level = 70, Location = 085, }, // Rayquaza @ Sky Pillar

            // Event
            new EncounterStatic { Species = 151, Level = 30, Location = 201, Version = GameVersion.E, Fateful = true }, // Mew @ Faraway Island (Unreleased outside of Japan)
            new EncounterStatic { Species = 249, Level = 70, Location = 211, Version = GameVersion.E, Fateful = true }, // Lugia @ Navel Rock
            new EncounterStatic { Species = 250, Level = 70, Location = 211, Version = GameVersion.E, Fateful = true }, // Ho-Oh @ Navel Rock
            new EncounterStatic { Species = 386, Level = 30, Location = 200, Version = GameVersion.E, Fateful = true, Form = 3 }, // Deoxys @ Birth Island
        };

        private static readonly EncounterStatic[] Encounter_FRLG_Roam =
        {
            new EncounterStatic { Species = 243, Level = 50, Roaming = true, }, // Raikou
            new EncounterStatic { Species = 244, Level = 50, Roaming = true, }, // Entei
            new EncounterStatic { Species = 245, Level = 50, Roaming = true, }, // Suicune
        };

        private static readonly EncounterStatic[] Encounter_FRLG_Stationary =
        {
            // Starters @ Pallet Town
            new EncounterStatic { Gift = true, Species = 1, Level = 05, Location = 088, }, // Bulbasaur
            new EncounterStatic { Gift = true, Species = 4, Level = 05, Location = 088, }, // Charmander
            new EncounterStatic { Gift = true, Species = 7, Level = 05, Location = 088, }, // Squirtle

            // Fossil @ Cinnabar Island
            new EncounterStatic { Gift = true, Species = 138, Level = 05, Location = 096, }, // Omanyte
            new EncounterStatic { Gift = true, Species = 140, Level = 05, Location = 096, }, // Kabuto
            new EncounterStatic { Gift = true, Species = 142, Level = 05, Location = 096, }, // Aerodactyl

            // Gift
            new EncounterStatic { Gift = true, Species = 106, Level = 25, Location = 098, }, // Hitmonlee @ Saffron City
            new EncounterStatic { Gift = true, Species = 107, Level = 25, Location = 098, }, // Hitmonchan @ Saffron City
            new EncounterStatic { Gift = true, Species = 129, Level = 05, Location = 099, }, // Magikarp @ Route 4
            new EncounterStatic { Gift = true, Species = 131, Level = 25, Location = 134, }, // Lapras @ Silph Co.
            new EncounterStatic { Gift = true, Species = 133, Level = 25, Location = 094, }, // Eevee @ Celadon City
            new EncounterStatic { Gift = true, Species = 175, Level = 05, EggLocation = 253 }, // Togepi Egg

            // Celadon City Game Corner
            new EncounterStatic { Gift = true, Species = 063, Level = 09, Location = 94, Version = GameVersion.FR }, // Abra
            new EncounterStatic { Gift = true, Species = 035, Level = 08, Location = 94, Version = GameVersion.FR }, // Clefairy
            new EncounterStatic { Gift = true, Species = 123, Level = 25, Location = 94, Version = GameVersion.FR }, // Scyther
            new EncounterStatic { Gift = true, Species = 147, Level = 18, Location = 94, Version = GameVersion.FR }, // Dratini
            new EncounterStatic { Gift = true, Species = 137, Level = 26, Location = 94, Version = GameVersion.FR }, // Porygon

            new EncounterStatic { Gift = true, Species = 063, Level = 07, Location = 94, Version = GameVersion.LG }, // Abra
            new EncounterStatic { Gift = true, Species = 035, Level = 12, Location = 94, Version = GameVersion.LG }, // Clefairy
            new EncounterStatic { Gift = true, Species = 127, Level = 18, Location = 94, Version = GameVersion.LG }, // Pinsir
            new EncounterStatic { Gift = true, Species = 147, Level = 24, Location = 94, Version = GameVersion.LG }, // Dratini
            new EncounterStatic { Gift = true, Species = 137, Level = 18, Location = 94, Version = GameVersion.LG }, // Porygon

            // Stationary
            new EncounterStatic { Species = 143, Level = 30, Location = 112, }, // Snorlax @ Route 12
            new EncounterStatic { Species = 143, Level = 30, Location = 116, }, // Snorlax @ Route 16
            new EncounterStatic { Species = 101, Level = 34, Location = 142, }, // Electrode @ Power Plant
            new EncounterStatic { Species = 097, Level = 30, Location = 176, }, // Hypno @ Berry Forest

            // Stationary Legendary
            new EncounterStatic { Species = 144, Level = 50, Location = 139, }, // Articuno @ Seafoam Islands
            new EncounterStatic { Species = 145, Level = 50, Location = 142, }, // Zapdos @ Power Plant
            new EncounterStatic { Species = 146, Level = 50, Location = 175, }, // Moltres @ Mt. Ember.
            new EncounterStatic { Species = 150, Level = 70, Location = 141, }, // Mewtwo @ Cerulean Cave

            // Event
            new EncounterStatic { Species = 249, Level = 70, Location = 174, Fateful = true }, // Lugia @ Navel Rock
            new EncounterStatic { Species = 250, Level = 70, Location = 174, Fateful = true }, // Ho-Oh @ Navel Rock
            new EncounterStatic { Species = 386, Level = 30, Location = 187, Version = GameVersion.FR, Form = 1, Fateful = true }, // Deoxys @ Birth Island
            new EncounterStatic { Species = 386, Level = 30, Location = 187, Version = GameVersion.LG, Form = 2, Fateful = true }, // Deoxys @ Birth Island
        };

        private static readonly EncounterStatic[] Encounter_RSE = Encounter_RSE_Roam.SelectMany(e => e.Clone(Roaming_MetLocation_RSE)).Concat(Encounter_RSE_Regular).ToArray();
        private static readonly EncounterStatic[] Encounter_FRLG = Encounter_FRLG_Roam.SelectMany(e => e.Clone(Roaming_MetLocation_FRLG)).Concat(Encounter_FRLG_Stationary).ToArray();

        private static readonly int[] TradeContest_Cool =   { 30, 05, 05, 05, 05, 10 };
        private static readonly int[] TradeContest_Beauty = { 05, 30, 05, 05, 05, 10 };
        private static readonly int[] TradeContest_Cute =   { 05, 05, 30, 05, 05, 10 };
        private static readonly int[] TradeContest_Clever = { 05, 05, 05, 30, 05, 10 };
        private static readonly int[] TradeContest_Tough =  { 05, 05, 05, 05, 30, 10 };

        internal static readonly EncounterTrade[] TradeGift_RSE =
        {
            new EncounterTradePID(0x00009C40) { Species = 296, Ability = 2, TID = 49562, SID = 00000, OTGender = 0, Gender = 0, IVs = new[] {5,5,4,4,4,4}, Level = 05, Contest = TradeContest_Tough, Version = GameVersion.RS, }, // Slakoth (Level 5 Breeding) -> Makuhita
            new EncounterTradePID(0x498A2E17) { Species = 300, Ability = 1, TID = 02259, SID = 00000, OTGender = 1, Gender = 1, IVs = new[] {5,4,4,5,4,4}, Level = 03, Contest = TradeContest_Cute, Version = GameVersion.RS, }, // Pikachu (Level 3 Viridiam Forest) -> Skitty
            new EncounterTradePID(0x4C970B7F) { Species = 222, Ability = 2, TID = 50183, SID = 00000, OTGender = 1, Gender = 1, IVs = new[] {4,4,5,4,4,5}, Level = 21, Contest = TradeContest_Beauty, Version = GameVersion.RS, }, // Bellossom (Level 21 Odish -> Gloom -> Bellossom) -> Corsola
            new EncounterTradePID(0x00000084) { Species = 273, Ability = 2, TID = 38726, SID = 00000, OTGender = 0, Gender = 0, IVs = new[] {5,4,5,4,4,4}, Level = 04, Contest = TradeContest_Cool, Version = GameVersion.E, }, // Ralts (Level 4 Route 102) -> Seedot
            new EncounterTradePID(0x0000006F) { Species = 311, Ability = 1, TID = 08460, SID = 00001, OTGender = 0, Gender = 1, IVs = new[] {4,4,4,5,5,4}, Level = 05, Contest = TradeContest_Cute, Version = GameVersion.E, }, // Volbeat (Level 5 Breeding) -> Plusle
            new EncounterTradePID(0x0000007F) { Species = 116, Ability = 1, TID = 46285, SID = 00000, OTGender = 0, Gender = 0, IVs = new[] {5,4,4,4,5,4}, Level = 05, Contest = TradeContest_Tough, Version = GameVersion.E, }, // Bagon (Level 5 Breeding) -> Horsea*
            new EncounterTradePID(0x0000008B) { Species = 052, Ability = 1, TID = 25945, SID = 00001, OTGender = 1, Gender = 0, IVs = new[] {4,5,4,5,4,4}, Level = 03, Contest = TradeContest_Clever, Version = GameVersion.E, }, // Skitty (Level 3 Trade)-> Meowth*
            //  If Pokémon with * is evolved in a Generation IV or V game, its Ability will become its second Ability.
        };

        internal static readonly EncounterTrade[] TradeGift_FRLG =
        {
            new EncounterTradePID(0x00009CAE) { Species = 122, Ability = 1, TID = 01985, SID = 00000, OTGender = 0, Gender = 0, IVs = new[] {20,15,17,24,23,22}, Level = 05, Contest = TradeContest_Clever, }, // Abra (Level 5 Breeding) -> Mr. Mime
            new EncounterTradePID(0x4C970B89) { Species = 029, Ability = 1, TID = 63184, SID = 00000, OTGender = 1, Gender = 1, IVs = new[] {22,18,25,19,15,22}, Level = 05, Contest = TradeContest_Tough, Version = GameVersion.FR, }, // Nidoran♀
            new EncounterTradePID(0x4C970B9E) { Species = 032, Ability = 1, TID = 63184, SID = 00000, OTGender = 1, Gender = 0, IVs = new[] {19,25,18,22,22,15}, Level = 05, Contest = TradeContest_Cool, Version = GameVersion.LG, }, // Nidoran♂ *
            new EncounterTradePID(0x00EECA15) { Species = 030, Ability = 1, TID = 13637, SID = 00000, OTGender = 0, Gender = 1, IVs = new[] {22,25,18,19,22,15}, Level = 16, Contest = TradeContest_Cute, Version = GameVersion.FR,}, // Nidorina *
            new EncounterTradePID(0x00EECA19) { Species = 033, Ability = 1, TID = 13637, SID = 00000, OTGender = 0, Gender = 0, IVs = new[] {19,18,25,22,15,22}, Level = 16, Contest = TradeContest_Tough, Version = GameVersion.LG,}, // Nidorino  *
            new EncounterTradePID(0x451308AB) { Species = 108, Ability = 1, TID = 01239, SID = 00000, OTGender = 0, Gender = 0, IVs = new[] {24,19,21,15,23,21}, Level = 25, Contest = TradeContest_Tough, Version = GameVersion.FR, }, // Golduck (Level 25) -> Lickitung  *
            new EncounterTradePID(0x451308AB) { Species = 108, Ability = 1, TID = 01239, SID = 00000, OTGender = 0, Gender = 0, IVs = new[] {24,19,21,15,23,21}, Level = 25, Contest = TradeContest_Tough, Version = GameVersion.LG, }, // Slowbro (Level 25) -> Lickitung  *
            new EncounterTradePID(0x498A2E1D) { Species = 124, Ability = 1, TID = 36728, SID = 00000, OTGender = 0, Gender = 1, IVs = new[] {18,17,18,22,25,21}, Level = 20, Contest = TradeContest_Beauty, }, // Poliwhirl (Level 20) -> Jynx
            new EncounterTradePID(0x151943D7) { Species = 083, Ability = 1, TID = 08810, SID = 00000, OTGender = 0, Gender = 0, IVs = new[] {20,25,21,24,15,20}, Level = 03, Contest = TradeContest_Cool, }, // Spearow (Level 3 Capture) -> Farfetch'd
            new EncounterTradePID(0x06341016) { Species = 101, Ability = 2, TID = 50298, SID = 00000, OTGender = 0, Gender = 2, IVs = new[] {19,16,18,25,25,19}, Level = 03, Contest = TradeContest_Cool, }, // Raichu (Level 3) -> Electrode
            new EncounterTradePID(0x5C77ECFA) { Species = 114, Ability = 1, TID = 60042, SID = 00000, OTGender = 1, Gender = 0, IVs = new[] {22,17,25,16,23,20}, Level = 05, Contest = TradeContest_Cute, }, // Venonat (Level 5 Breeding) -> Tangela
            new EncounterTradePID(0x482CAC89) { Species = 086, Ability = 1, TID = 09853, SID = 00000, OTGender = 0, Gender = 0, IVs = new[] {24,15,22,16,23,22}, Level = 05, Contest = TradeContest_Tough, }, // Ponyta (Level 5 Breeding) -> Seel *
            //  If Pokémon with * is evolved in a Generation IV or V game, its Ability will become its second Ability.
        };

        private const string tradeRSE = "traderse";
        private const string tradeFRLG = "tradefrlg";
        private static readonly string[][] TradeRSE = Util.GetLanguageStrings7(tradeRSE);
        private static readonly string[][] TradeFRLG = Util.GetLanguageStrings7(tradeFRLG);

        private static readonly int[] MoveSwarmSurskit = { 145, 098 }; /* Bubble, Quick Attack */
        private static readonly int[] MoveSwarmSeedot = { 117, 106, 073 };  /* Bide, Harden, Leech Seed */
        private static readonly int[] MoveSwarmNuzleaf = { 106, 074, 267, 073 }; /* Harden, Growth, Nature Power, Leech Seed */
        private static readonly int[] MoveSwarmSeedotF = { 202, 218, 076, 073 }; /* Giga Drain, Frustration, Solar Beam, Leech Seed */
        private static readonly int[] MoveSwarmSkittyRS = { 045, 033 }; /* Growl, Tackle */
        private static readonly int[] MoveSwarmSkittyE = { 045, 033, 039, 213 }; /* Growl, Tackle, Tail Whip, Attract */

        #region AltSlots
        private static readonly EncounterArea3[] SlotsRSEAlt =
        {
            // Swarm can be passed from R/S<->E via mixing records
            // Encounter Percent is a 50% call
            new EncounterArea3 {
                Location = 17, // Route 102
                Slots = new EncounterSlot[]
                {
                    new EncounterSlot3Swarm(MoveSwarmSurskit) { Species = 283, LevelMin = 03, LevelMax = 03, Type = SlotType.Swarm },
                    new EncounterSlot3Swarm(MoveSwarmSeedot) { Species = 273, LevelMin = 03, LevelMax = 03, Type = SlotType.Swarm },
                },},
            new EncounterArea3 {
                Location = 29, // Route 114
                Slots = new EncounterSlot[]
                {
                    new EncounterSlot3Swarm(MoveSwarmSurskit) { Species = 283, LevelMin = 15, LevelMax = 15, Type = SlotType.Swarm },
                    new EncounterSlot3Swarm(MoveSwarmNuzleaf) { Species = 274, LevelMin = 15, LevelMax = 15, Type = SlotType.Swarm },
                },},
            new EncounterArea3 {
                Location = 31, // Route 116
                Slots = new EncounterSlot[]
                {
                    new EncounterSlot3Swarm(MoveSwarmSkittyRS) { Species = 300, LevelMin = 15, LevelMax = 15, Type = SlotType.Swarm },
                    new EncounterSlot3Swarm(MoveSwarmSkittyE) { Species = 300, LevelMin = 08, LevelMax = 08, Type = SlotType.Swarm },
                },},
            new EncounterArea3 {
                Location = 32, // Route 117
                Slots = new EncounterSlot[]
                {
                    new EncounterSlot3Swarm(MoveSwarmSurskit) { Species = 283, LevelMin = 15, LevelMax = 15, Type = SlotType.Swarm },
                    new EncounterSlot3Swarm(MoveSwarmNuzleaf) { Species = 273, LevelMin = 13, LevelMax = 13, Type = SlotType.Swarm }, // Has same moves as Nuzleaf
                },},
            new EncounterArea3 {
                Location = 35, // Route 120
                Slots = new EncounterSlot[]
                {
                    new EncounterSlot3Swarm(MoveSwarmSurskit) { Species = 283, LevelMin = 28, LevelMax = 28, Type = SlotType.Swarm },
                    new EncounterSlot3Swarm(MoveSwarmSeedotF) { Species = 273, LevelMin = 25, LevelMax = 25, Type = SlotType.Swarm },
                },},

            // Feebas fishing spot
            new EncounterArea3 {
                Location = 34, // Route 119
                Slots = new[]
                {
                    new EncounterSlot { Species = 349, LevelMin = 20, LevelMax = 25, Type = SlotType.Swarm } // Feebas with any Rod (50%)
                },},
        };

        private static readonly EncounterArea3[] SlotsFRLGUnown =
        {
            GetUnownArea(188, new[] { 00,00,00,00,00,00,00,00,00,00,00,27 }), // 188 = Monean Chamber
            GetUnownArea(189, new[] { 02,02,02,03,03,03,07,07,07,20,20,14 }), // 189 = Liptoo Chamber
            GetUnownArea(190, new[] { 13,13,13,13,18,18,18,18,08,08,04,04 }), // 190 = Weepth Chamber
            GetUnownArea(191, new[] { 15,15,11,11,09,09,17,17,17,16,16,16 }), // 191 = Dilford Chamber
            GetUnownArea(192, new[] { 24,24,19,19,06,06,06,05,05,05,10,10 }), // 192 = Scufib Chamber
            GetUnownArea(193, new[] { 21,21,21,22,22,22,23,23,12,12,01,01 }), // 193 = Rixy Chamber
            GetUnownArea(194, new[] { 25,25,25,25,25,25,25,25,25,25,25,26 }), // 194 = Viapois Chamber
        };

        private static EncounterArea3 GetUnownArea(int location, IReadOnlyList<int> SlotForms)
        {
            return new EncounterArea3
            {
                Location = location,
                Slots = SlotForms.Select((_, i) => new EncounterSlot
                {
                    Species = 201, LevelMin = 25, LevelMax = 25, Type = SlotType.Grass,
                    SlotNumber = i,
                    Form = SlotForms[i]
                }).ToArray()
            };
        }
        #endregion

        #region Colosseum
        private static readonly EncounterStatic[] Encounter_Colo =
        {
            // Colosseum Starters: Gender locked to male
            new EncounterStatic { Gift = true, Species = 196, Level = 25, Location = 254, Gender = 0 }, // Espeon
            new EncounterStatic { Gift = true, Species = 197, Level = 26, Location = 254, Gender = 0, Moves = new[] {044} }, // Umbreon (Bite)

            new EncounterStaticShadow(ColoMakuhita) { Species = 296, Level = 30, Gauge = 03000, Moves = new[] {193,116,233,238}, Location = 005 }, // Makuhita: Miror B.Peon Trudly @ Phenac City

            new EncounterStaticShadow { Species = 153, Level = 30, Gauge = 03000, Moves = new[] {241,235,075,034}, Location = 003 }, // Bayleef: Cipher Peon Verde @ Phenac City
            new EncounterStaticShadow { Species = 156, Level = 30, Gauge = 03000, Moves = new[] {241,108,091,172}, Location = 003 }, // Quilava: Cipher Peon Rosso @ Phenac City
            new EncounterStaticShadow { Species = 159, Level = 30, Gauge = 03000, Moves = new[] {240,184,044,057}, Location = 003 }, // Croconaw: Cipher Peon Bluno @ Phenac City
            new EncounterStaticShadow { Species = 153, Level = 30, Gauge = 03000, Moves = new[] {241,235,075,034}, Location = 069 }, // Bayleef: Cipher Peon Verde @ Shadow PKMN Lab
            new EncounterStaticShadow { Species = 156, Level = 30, Gauge = 03000, Moves = new[] {241,108,091,172}, Location = 069 }, // Quilava: Cipher Peon Rosso @ Shadow PKMN Lab
            new EncounterStaticShadow { Species = 159, Level = 30, Gauge = 03000, Moves = new[] {240,184,044,057}, Location = 069 }, // Croconaw: Cipher Peon Bluno @ Shadow PKMN Lab
            new EncounterStaticShadow { Species = 153, Level = 30, Gauge = 03000, Moves = new[] {241,235,075,034}, Location = 115 }, // Bayleef: Cipher Peon Verde @ Realgam Tower
            new EncounterStaticShadow { Species = 156, Level = 30, Gauge = 03000, Moves = new[] {241,108,091,172}, Location = 115 }, // Quilava: Cipher Peon Rosso @ Realgam Tower
            new EncounterStaticShadow { Species = 159, Level = 30, Gauge = 03000, Moves = new[] {240,184,044,057}, Location = 115 }, // Croconaw: Cipher Peon Bluno @ Realgam Tower
            new EncounterStaticShadow { Species = 153, Level = 30, Gauge = 03000, Moves = new[] {241,235,075,034}, Location = 132 }, // Bayleef: Cipher Peon Verde @ Snagem Hideout
            new EncounterStaticShadow { Species = 156, Level = 30, Gauge = 03000, Moves = new[] {241,108,091,172}, Location = 132 }, // Quilava: Cipher Peon Rosso @ Snagem Hideout
            new EncounterStaticShadow { Species = 159, Level = 30, Gauge = 03000, Moves = new[] {240,184,044,057}, Location = 132 }, // Croconaw: Cipher Peon Bluno @ Snagem Hideout

            new EncounterStaticShadow { Species = 218, Level = 30, Gauge = 04000, Moves = new[] {241,281,088,053}, Location = 015 }, // Slugma: Roller Boy Lon @ Pyrite Town
            new EncounterStaticShadow { Species = 164, Level = 30, Gauge = 03000, Moves = new[] {211,095,115,019}, Location = 015 }, // Noctowl: Rider Nover @ Pyrite Town
            new EncounterStaticShadow { Species = 180, Level = 30, Gauge = 03000, Moves = new[] {085,086,178,084}, Location = 015 }, // Flaaffy: St.Performer Diogo @ Pyrite Town
            new EncounterStaticShadow { Species = 188, Level = 30, Gauge = 03000, Moves = new[] {235,079,178,072}, Location = 015 }, // Skiploom: Rider Leba @ Pyrite Town
            new EncounterStaticShadow { Species = 195, Level = 30, Gauge = 04000, Moves = new[] {341,133,021,057}, Location = 015 }, // Quagsire: Bandana Guy Divel @ Pyrite Town
            new EncounterStaticShadow { Species = 200, Level = 30, Gauge = 04000, Moves = new[] {060,109,212,247}, Location = 015 }, // Misdreavus: Rider Vant @ Pyrite Town
            new EncounterStaticShadow { Species = 162, Level = 33, Gauge = 05000, Moves = new[] {231,270,098,070}, Location = 015 }, // Furret: Rogue Cail @ Pyrite Town

            new EncounterStaticShadow { Species = 193, Level = 33, Gauge = 05000, Moves = new[] {197,048,049,253}, Location = 025 }, // Yanma: Cipher Peon Nore @ Pyrite Bldg
            new EncounterStaticShadow { Species = 193, Level = 33, Gauge = 05000, Moves = new[] {197,048,049,253}, Location = 132 }, // Yanma: Cipher Peon Nore @ Snagem Hideout

            new EncounterStaticShadow { Species = 223, Level = 20, Gauge = 04000, Moves = new[] {061,199,060,062}, Location = 028 }, // Remoraid: Miror B.Peon Reath @ Pyrite Bldg
            new EncounterStaticShadow { Species = 223, Level = 20, Gauge = 04000, Moves = new[] {061,199,060,062}, Location = 030 }, // Remoraid: Miror B.Peon Reath @ Pyrite Cave
            new EncounterStaticShadow { Species = 226, Level = 33, Gauge = 05000, Moves = new[] {017,048,061,036}, Location = 028 }, // Mantine: Miror B.Peon Ferma @ Pyrite Bldg
            new EncounterStaticShadow { Species = 226, Level = 33, Gauge = 05000, Moves = new[] {017,048,061,036}, Location = 030 }, // Mantine: Miror B.Peon Ferma @ Pyrite Cave

            new EncounterStaticShadow { Species = 211, Level = 33, Gauge = 05000, Moves = new[] {042,107,040,057}, Location = 015 }, // Qwilfish: Hunter Doken @ Pyrite Bldg
            new EncounterStaticShadow { Species = 307, Level = 33, Gauge = 05000, Moves = new[] {197,347,093,136}, Location = 031 }, // Meditite: Rider Twan @ Pyrite Cave
            new EncounterStaticShadow { Species = 206, Level = 33, Gauge = 05000, Moves = new[] {180,137,281,036}, Location = 029 }, // Dunsparce: Rider Sosh @ Pyrite Cave
            new EncounterStaticShadow { Species = 333, Level = 33, Gauge = 05000, Moves = new[] {119,047,219,019}, Location = 032 }, // Swablu: Hunter Zalo @ Pyrite Cave

            new EncounterStaticShadow { Species = 185, Level = 35, Gauge = 10000, Moves = new[] {175,335,067,157}, Location = 104 }, // Sudowoodo: Cipher Admin Miror B. @ Realgam Tower
            new EncounterStaticShadow { Species = 185, Level = 35, Gauge = 10000, Moves = new[] {175,335,067,157}, Location = 125 }, // Sudowoodo: Cipher Admin Miror B. @ Deep Colosseum
            new EncounterStaticShadow { Species = 185, Level = 35, Gauge = 10000, Moves = new[] {175,335,067,157}, Location = 030 }, // Sudowoodo: Cipher Admin Miror B. @ Pyrite Cave

            new EncounterStaticShadow { Species = 237, Level = 38, Gauge = 06000, Moves = new[] {097,116,167,229}, Location = 039 }, // Hitmontop: Cipher Peon Skrub @ Agate Village
            new EncounterStaticShadow { Species = 237, Level = 38, Gauge = 06000, Moves = new[] {097,116,167,229}, Location = 132 }, // Hitmontop: Cipher Peon Skrub @ Snagem Hideout
            new EncounterStaticShadow { Species = 237, Level = 38, Gauge = 06000, Moves = new[] {097,116,167,229}, Location = 068 }, // Hitmontop: Cipher Peon Skrub @ Shadow PKMN Lab

            new EncounterStaticShadow { Species = 166, Level = 40, Gauge = 06000, Moves = new[] {226,219,048,004}, Location = 047 }, // Ledian: Cipher Peon Kloak @ The Under
            new EncounterStaticShadow { Species = 166, Level = 40, Gauge = 06000, Moves = new[] {226,219,048,004}, Location = 132 }, // Ledian: Cipher Peon Kloak @ Snagem Hideout

            new EncounterStaticShadow { Species = 244, Level = 40, Gauge = 13000, Moves = new[] {241,043,044,126}, Location = 106 }, // Entei: Cipher Admin Dakim @ Realgam Tower
            new EncounterStaticShadow { Species = 244, Level = 40, Gauge = 13000, Moves = new[] {241,043,044,126}, Location = 125 }, // Entei: Cipher Admin Dakim @ Deep Colosseum
            new EncounterStaticShadow { Species = 244, Level = 40, Gauge = 13000, Moves = new[] {241,043,044,126}, Location = 076 }, // Entei: Cipher Admin Dakim @ Mt. Battle

            new EncounterStaticShadow { Species = 245, Level = 40, Gauge = 13000, Moves = new[] {240,043,016,057}, Location = 110 }, // Suicune (Surf): Cipher Admin Venus @ Realgam Tower
            new EncounterStaticShadow { Species = 245, Level = 40, Gauge = 13000, Moves = new[] {240,043,016,056}, Location = 125 }, // Suicune (Hydro Pump): Cipher Admin Venus @ Deep Colosseum
            new EncounterStaticShadow { Species = 245, Level = 40, Gauge = 13000, Moves = new[] {240,043,016,057}, Location = 055 }, // Suicune (Surf): Cipher Admin Venus @ The Under

            new EncounterStaticShadow { Species = 243, Level = 40, Gauge = 13000, Moves = new[] {240,043,098,087}, Location = 113 }, // Raikou: Cipher Admin Ein @ Realgam Tower
            new EncounterStaticShadow { Species = 243, Level = 40, Gauge = 13000, Moves = new[] {240,043,098,087}, Location = 125 }, // Raikou: Cipher Admin Ein @ Deep Colosseum
            new EncounterStaticShadow { Species = 243, Level = 40, Gauge = 13000, Moves = new[] {240,043,098,087}, Location = 069 }, // Raikou: Cipher Admin Ein @ Shadow PKMN Lab

            new EncounterStaticShadow { Species = 234, Level = 43, Gauge = 06000, Moves = new[] {310,095,043,036}, Location = 058 }, // Stantler: Chaser Liaks @ The Under Subway
            new EncounterStaticShadow { Species = 234, Level = 43, Gauge = 06000, Moves = new[] {310,095,043,036}, Location = 133 }, // Stantler: Chaser Liaks @ Snagem Hideout
            new EncounterStaticShadow { Species = 221, Level = 43, Gauge = 06000, Moves = new[] {203,316,091,059}, Location = 058 }, // Piloswine: Bodybuilder Lonia @ The Under Subway
            new EncounterStaticShadow { Species = 221, Level = 43, Gauge = 06000, Moves = new[] {203,316,091,059}, Location = 134 }, // Piloswine: Bodybuilder Lonia @ Snagem Hideout
            new EncounterStaticShadow { Species = 215, Level = 43, Gauge = 06000, Moves = new[] {185,103,154,196}, Location = 058 }, // Sneasel: Rider Nelis @ The Under Subway
            new EncounterStaticShadow { Species = 215, Level = 43, Gauge = 06000, Moves = new[] {185,103,154,196}, Location = 134 }, // Sneasel: Rider Nelis @ Snagem Hideout
            new EncounterStaticShadow { Species = 190, Level = 43, Gauge = 06000, Moves = new[] {226,321,154,129}, Location = 067 }, // Aipom: Cipher Peon Cole @ Shadow PKMN Lab
            new EncounterStaticShadow { Species = 205, Level = 43, Gauge = 06000, Moves = new[] {153,182,117,229}, Location = 067 }, // Forretress: Cipher Peon Vana @ Shadow PKMN Lab
            new EncounterStaticShadow { Species = 168, Level = 43, Gauge = 06000, Moves = new[] {169,184,141,188}, Location = 069 }, // Ariados: Cipher Peon Lesar @ Shadow PKMN Lab
            new EncounterStaticShadow { Species = 210, Level = 43, Gauge = 06000, Moves = new[] {044,184,046,070}, Location = 069 }, // Granbull: Cipher Peon Tanie @ Shadow PKMN Lab
            new EncounterStaticShadow { Species = 329, Level = 43, Gauge = 06000, Moves = new[] {242,103,328,225}, Location = 068 }, // Vibrava: Cipher Peon Remil @ Shadow PKMN Lab

            new EncounterStaticShadow { Species = 192, Level = 45, Gauge = 07000, Moves = new[] {241,074,275,076}, Location = 109 }, // Sunflora: Cipher Peon Baila @ Realgam Tower
            new EncounterStaticShadow { Species = 225, Level = 45, Gauge = 07000, Moves = new[] {059,213,217,019}, Location = 109 }, // Delibird: Cipher Peon Arton @ Realgam Tower
            new EncounterStaticShadow { Species = 227, Level = 47, Gauge = 13000, Moves = new[] {065,319,314,211}, Location = 117 }, // Skarmory: Snagem Head Gonzap @ Realgam Tower
            new EncounterStaticShadow { Species = 192, Level = 45, Gauge = 07000, Moves = new[] {241,074,275,076}, Location = 132 }, // Sunflora: Cipher Peon Baila @ Snagem Hideout
            new EncounterStaticShadow { Species = 225, Level = 45, Gauge = 07000, Moves = new[] {059,213,217,019}, Location = 132 }, // Delibird: Cipher Peon Arton @ Snagem Hideout
            new EncounterStaticShadow { Species = 227, Level = 47, Gauge = 13000, Moves = new[] {065,319,314,211}, Location = 133 }, // Skarmory: Snagem Head Gonzap @ Snagem Hideout

            new EncounterStaticShadow { Species = 241, Level = 48, Gauge = 07000, Moves = new[] {208,111,205,034}, Location = 118 }, // Miltank: Bodybuilder Jomas @ Tower Colosseum
            new EncounterStaticShadow { Species = 359, Level = 48, Gauge = 07000, Moves = new[] {195,014,163,185}, Location = 118 }, // Absol: Rider Delan @ Tower Colosseum
            new EncounterStaticShadow { Species = 229, Level = 48, Gauge = 07000, Moves = new[] {185,336,123,053}, Location = 118 }, // Houndoom: Cipher Peon Nella @ Tower Colosseum
            new EncounterStaticShadow { Species = 357, Level = 49, Gauge = 07000, Moves = new[] {076,235,345,019}, Location = 118 }, // Tropius: Cipher Peon Ston @ Tower Colosseum
            new EncounterStaticShadow { Species = 376, Level = 50, Gauge = 15000, Moves = new[] {063,334,232,094}, Location = 118 }, // Metagross: Cipher Nascour @ Tower Colosseum
            new EncounterStaticShadow { Species = 248, Level = 55, Gauge = 20000, Moves = new[] {242,087,157,059}, Location = 118 }, // Tyranitar: Cipher Head Evice @ Tower Colosseum
            new EncounterStaticShadow { Species = 235, Level = 45, Gauge = 07000, Moves = new[] {166,039,003,231}, Location = 132 }, // Smeargle: Team Snagem Biden @ Snagem Hideout
            new EncounterStaticShadow { Species = 213, Level = 45, Gauge = 07000, Moves = new[] {219,227,156,117}, Location = 125 }, // Shuckle: Deep King Agnol @ Deep Colosseum
            new EncounterStaticShadow { Species = 176, Level = 20, Gauge = 05000, Moves = new[] {118,204,186,281}, Location = 001 }, // Togetic: Cipher Peon Fein @ Outskirt Stand

            new EncounterStaticShadow(Gligar)    { Species = 207, Level = 43, Gauge = 06000, Moves = new[] {185,028,040,163}, Location = 058 }, // Gligar: Hunter Frena @ The Under Subway
            new EncounterStaticShadow(Gligar)    { Species = 207, Level = 43, Gauge = 06000, Moves = new[] {185,028,040,163}, Location = 133 }, // Gligar: Hunter Frena @ Snagem Hideout
            new EncounterStaticShadow(Murkrow)   { Species = 198, Level = 43, Gauge = 06000, Moves = new[] {185,212,101,019}, Location = 067 }, // Murkrow: Cipher Peon Lare @ Shadow PKMN Lab
            new EncounterStaticShadow(Heracross) { Species = 214, Level = 45, Gauge = 07000, Moves = new[] {179,203,068,280}, Location = 111 }, // Heracross: Cipher Peon Dioge @ Realgam Tower
            new EncounterStaticShadow(Heracross) { Species = 214, Level = 45, Gauge = 07000, Moves = new[] {179,203,068,280}, Location = 132 }, // Heracross: Cipher Peon Dioge @ Snagem Hideout
            new EncounterStaticShadow(Ursaring)  { Species = 217, Level = 45, Gauge = 07000, Moves = new[] {185,313,122,163}, Location = 132 }, // Ursaring: Team Snagem Agrev @ Snagem Hideout
            new EncounterStaticShadow(CTogepi)   { Species = 175, Level = 20, Gauge = 00000, Moves = new[] {118,204,186,281}, IVs = new[] {0,0,0,0,0,0}, EReader = true }, // Togepi: Chaser ボデス @ Card e Room (Japanese games only)
            new EncounterStaticShadow(CMareep)   { Species = 179, Level = 37, Gauge = 00000, Moves = new[] {087,084,086,178}, IVs = new[] {0,0,0,0,0,0}, EReader = true }, // Mareep: Hunter ホル @ Card e Room (Japanese games only)
            new EncounterStaticShadow(CScizor)   { Species = 212, Level = 50, Gauge = 00000, Moves = new[] {210,232,014,163}, IVs = new[] {0,0,0,0,0,0}, EReader = true }, // Scizor: Bodybuilder ワーバン @ Card e Room (Japanese games only)
        };
        #endregion

        #region XD

        private static readonly int[] MirorBXDLocations =
        {
            090, // Rock
            091, // Oasis
            092, // Cave
            113, // Pyrite Town
            059, // Realgam Tower
        };

        private static readonly EncounterStatic[] Encounter_XD = new[]
        {
            new EncounterStatic { Fateful = true, Gift = true, Species = 133, Level = 10, Location = 000, Moves = new[] {044} }, // Eevee (Bite)
            new EncounterStatic { Fateful = true, Gift = true, Species = 152, Level = 05, Location = 016, Moves = new[] {246,033,045,338} }, // Chikorita
            new EncounterStatic { Fateful = true, Gift = true, Species = 155, Level = 05, Location = 016, Moves = new[] {179,033,043,307} }, // Cyndaquil
            new EncounterStatic { Fateful = true, Gift = true, Species = 158, Level = 05, Location = 016, Moves = new[] {242,010,043,308} }, // Totodile

            new EncounterStaticShadow { Fateful = true, Species = 216, Level = 11, Gauge = 03000, Moves = new[] {216,287,122,232}, Location = 143 }, // Teddiursa: Cipher Peon Naps @ Pokémon HQ Lab
            new EncounterStaticShadow { Fateful = true, Species = 228, Level = 17, Gauge = 01500, Moves = new[] {185,204,052,046}, Location = 011, }, // Houndour: Cipher Peon Resix @ Cipher Lab
            new EncounterStaticShadow { Fateful = true, Species = 343, Level = 17, Gauge = 01500, Moves = new[] {317,287,189,060}, Location = 011, }, // Baltoy: Cipher Peon Browsix @ Cipher Lab
            new EncounterStaticShadow { Fateful = true, Species = 179, Level = 17, Gauge = 01500, Moves = new[] {034,215,084,086}, Location = 011, }, // Mareep: Cipher Peon Yellosix @ Cipher Lab
            new EncounterStaticShadow { Fateful = true, Species = 318, Level = 15, Gauge = 01700, Moves = new[] {352,287,184,044}, Location = 008, }, // Carvanha: Cipher Peon Cabol @ Cipher Lab
            new EncounterStaticShadow { Fateful = true, Species = 175, Level = 25, Gauge = 04500, Moves = new[] {266,161,246,270}, Location = 164, Gift = true }, // Togepi: Pokémon Trainer Hordel @ Outskirt Stand

            new EncounterStaticShadow { Fateful = true, Species = 335, Level = 28, Gauge = 05000, Moves = new[] {280,287,068,306}, Location = 071 }, // Zangoose: Thug Zook @ Cipher Key Lair
            new EncounterStaticShadow { Fateful = true, Species = 335, Level = 28, Gauge = 05000, Moves = new[] {280,287,068,306}, Location = 090 }, // Zangoose: Thug Zook @ Cipher Key Lair
            new EncounterStaticShadow { Fateful = true, Species = 015, Level = 30, Gauge = 04500, Moves = new[] {188,226,041,014}, Location = 059 }, // Beedrill: Cipher Peon Lok @ Cipher Key Lair
            new EncounterStaticShadow { Fateful = true, Species = 090, Level = 29, Gauge = 04000, Moves = new[] {036,287,057,062}, Location = 065 }, // Shellder: Cipher Peon Gorog @ Cipher Key Lair
            new EncounterStaticShadow { Fateful = true, Species = 015, Level = 30, Gauge = 04500, Moves = new[] {188,226,041,014}, Location = 066 }, // Beedrill: Cipher Peon Lok @ Cipher Key Lair
            new EncounterStaticShadow { Fateful = true, Species = 249, Level = 50, Gauge = 12000, Moves = new[] {354,297,089,056}, Location = 074 }, // Lugia: Grand Master Greevil @ Citadark Isle
            new EncounterStaticShadow { Fateful = true, Species = 112, Level = 46, Gauge = 07000, Moves = new[] {224,270,184,089}, Location = 074 }, // Rhydon: Grand Master Greevil @ Citadark Isle
            new EncounterStaticShadow { Fateful = true, Species = 277, Level = 43, Gauge = 07000, Moves = new[] {143,226,097,263}, Location = 087 }, // Swellow: Cipher Admin Ardos @ Citadark Isle

            new EncounterStaticShadow(Ledyba)   { Fateful = true, Species = 165, Level = 10, Gauge = 02500, Moves = new[] {060,287,332,048}, Location = 153 }, // Ledyba: Casual Guy Cyle @ Gateon Port
            new EncounterStaticShadow(Poochyena){ Fateful = true, Species = 261, Level = 10, Gauge = 02500, Moves = new[] {091,215,305,336}, Location = 162 }, // Poochyena: Bodybuilder Kilen @ Gateon Port
            new EncounterStaticShadow(Seedot)   { Fateful = true, Species = 273, Level = 17, Gauge = 01500, Moves = new[] {202,287,331,290}, Location = 011 }, // Seedot: Cipher Peon Greesix @ Cipher Lab
            new EncounterStaticShadow(Spheal)   { Fateful = true, Species = 363, Level = 17, Gauge = 01500, Moves = new[] {062,204,055,189}, Location = 011 }, // Spheal: Cipher Peon Blusix @ Cipher Lab
            new EncounterStaticShadow(Gulpin)   { Fateful = true, Species = 316, Level = 17, Gauge = 01500, Moves = new[] {351,047,124,092}, Location = 011 }, // Gulpin: Cipher Peon Purpsix @ Cipher Lab
            new EncounterStaticShadow(Spinarak) { Fateful = true, Species = 167, Level = 14, Gauge = 01500, Moves = new[] {091,287,324,101}, Location = 010 }, // Spinarak: Cipher Peon Nexir @ Cipher Lab
            new EncounterStaticShadow(Numel)    { Fateful = true, Species = 322, Level = 14, Gauge = 01500, Moves = new[] {036,204,091,052}, Location = 009 }, // Numel: Cipher Peon Solox @ Cipher Lab
            new EncounterStaticShadow(Shroomish){ Fateful = true, Species = 285, Level = 15, Gauge = 01800, Moves = new[] {206,287,072,078}, Location = 008 }, // Shroomish: Cipher R&D Klots @ Cipher Lab
            new EncounterStaticShadow(Delcatty) { Fateful = true, Species = 301, Level = 18, Gauge = 02500, Moves = new[] {290,186,213,351}, Location = 008 }, // Delcatty: Cipher Admin Lovrina @ Cipher Lab
            new EncounterStaticShadow(Voltorb)  { Fateful = true, Species = 100, Level = 19, Gauge = 02500, Moves = new[] {243,287,209,129}, Location = 092 }, // Voltorb: Wanderer Miror B. @ Cave Poké Spot
            new EncounterStaticShadow(Makuhita) { Fateful = true, Species = 296, Level = 18, Gauge = 02000, Moves = new[] {280,287,292,317}, Location = 109 }, // Makuhita: Cipher Peon Torkin @ ONBS Building
            new EncounterStaticShadow(Vulpix)   { Fateful = true, Species = 037, Level = 18, Gauge = 02000, Moves = new[] {257,204,052,091}, Location = 109 }, // Vulpix: Cipher Peon Mesin @ ONBS Building
            new EncounterStaticShadow(Duskull)  { Fateful = true, Species = 355, Level = 19, Gauge = 02200, Moves = new[] {247,270,310,109}, Location = 110 }, // Duskull: Cipher Peon Lobar @ ONBS Building
            new EncounterStaticShadow(Ralts)    { Fateful = true, Species = 280, Level = 20, Gauge = 02200, Moves = new[] {351,047,115,093}, Location = 119 }, // Ralts: Cipher Peon Feldas @ ONBS Building
            new EncounterStaticShadow(Mawile)   { Fateful = true, Species = 303, Level = 22, Gauge = 02500, Moves = new[] {206,047,011,334}, Location = 111 }, // Mawile: Cipher Cmdr Exol @ ONBS Building
            new EncounterStaticShadow(Snorunt)  { Fateful = true, Species = 361, Level = 20, Gauge = 02500, Moves = new[] {352,047,044,196}, Location = 097 }, // Snorunt: Cipher Peon Exinn @ Phenac City
            new EncounterStaticShadow(Pineco)   { Fateful = true, Species = 204, Level = 20, Gauge = 02500, Moves = new[] {042,287,191,068}, Location = 096 }, // Pineco: Cipher Peon Gonrap @ Phenac City
            new EncounterStaticShadow(Natu)     { Fateful = true, Species = 177, Level = 22, Gauge = 02500, Moves = new[] {248,226,101,332}, Location = 094 }, // Natu: Cipher Peon Eloin @ Phenac City

            new EncounterStaticShadow(Roselia)  { Fateful = true, Species = 315, Level = 22, Gauge = 03000, Moves = new[] {345,186,320,073}, Location = 113, }, // Roselia: Cipher Peon Fasin @ Phenac City
            new EncounterStaticShadow(Roselia)  { Fateful = true, Species = 315, Level = 22, Gauge = 03000, Moves = new[] {345,186,320,073}, Location = 094, }, // Roselia: Cipher Peon Fasin @ Phenac City
            new EncounterStaticShadow(Meowth)   { Fateful = true, Species = 052, Level = 22, Gauge = 03500, Moves = new[] {163,047,006,044}, Location = 113, }, // Meowth: Cipher Peon Fostin @ Phenac City
            new EncounterStaticShadow(Meowth)   { Fateful = true, Species = 052, Level = 22, Gauge = 03500, Moves = new[] {163,047,006,044}, Location = 094, }, // Meowth: Cipher Peon Fostin @ Phenac City

            new EncounterStaticShadow(Swinub)   { Fateful = true, Species = 220, Level = 22, Gauge = 02500, Moves = new[] {246,204,054,341}, Location = 100, }, // Swinub: Cipher Peon Greck @ Phenac City

            new EncounterStaticShadow(Spearow)  { Fateful = true, Species = 021, Level = 22, Gauge = 04500, Moves = new[] {206,226,043,332}, Location = 059, }, // Spearow: Cipher Peon Ezin @ Phenac Stadium
            new EncounterStaticShadow(Spearow)  { Fateful = true, Species = 021, Level = 22, Gauge = 04500, Moves = new[] {206,226,043,332}, Location = 107, }, // Spearow: Cipher Peon Ezin @ Phenac Stadium
            new EncounterStaticShadow(Grimer)   { Fateful = true, Species = 088, Level = 23, Gauge = 03000, Moves = new[] {188,270,325,107}, Location = 059, }, // Grimer: Cipher Peon Faltly @ Phenac Stadium
            new EncounterStaticShadow(Grimer)   { Fateful = true, Species = 088, Level = 23, Gauge = 03000, Moves = new[] {188,270,325,107}, Location = 107, }, // Grimer: Cipher Peon Faltly @ Phenac Stadium

            new EncounterStaticShadow(Seel)     { Fateful = true, Species = 086, Level = 23, Gauge = 03500, Moves = new[] {057,270,219,058}, Location = 107, }, // Seel: Cipher Peon Egrog @ Phenac Stadium
            new EncounterStaticShadow(Lunatone) { Fateful = true, Species = 337, Level = 25, Gauge = 05000, Moves = new[] {094,226,240,317}, Location = 107, }, // Lunatone: Cipher Admin Snattle @ Phenac Stadium

            new EncounterStaticShadow(Nosepass) { Fateful = true, Species = 299, Level = 26, Gauge = 04000, Moves = new[] {085,270,086,157}, Location = 090, }, // Nosepass: Wanderer Miror B. @ Pyrite Colosseum/Realgam Colosseum/Poké Spots
            new EncounterStaticShadow(Nosepass) { Fateful = true, Species = 299, Level = 26, Gauge = 04000, Moves = new[] {085,270,086,157}, Location = 113, }, // Nosepass: Wanderer Miror B. @ Pyrite Colosseum/Realgam Colosseum/Poké Spots

            new EncounterStaticShadow(Paras)    { Fateful = true, Species = 046, Level = 28, Gauge = 04000, Moves = new[] {147,287,163,206}, Location = 064, }, // Paras: Cipher Peon Humah @ Cipher Key Lair

            new EncounterStaticShadow(Growlithe) { Fateful = true, Species = 058, Level = 28, Gauge = 04000, Moves = new[] {053,204,044,036}, Location = 064 }, // Growlithe: Cipher Peon Humah @ Cipher Key Lair
            new EncounterStaticShadow(Growlithe) { Fateful = true, Species = 058, Level = 28, Gauge = 04000, Moves = new[] {053,204,044,036}, Location = 113 }, // Growlithe: Cipher Peon Humah @ Cipher Key Lair
            new EncounterStaticShadow(Butterfree){ Fateful = true, Species = 012, Level = 30, Gauge = 04000, Moves = new[] {094,234,079,332}, Location = 059, }, // Butterfree: Cipher Peon Targ @ Cipher Key Lair
            new EncounterStaticShadow(Venomoth)  { Fateful = true, Species = 049, Level = 32, Gauge = 04000, Moves = new[] {318,287,164,094}, Location = 059, }, // Venomoth: Cipher Peon Angic @ Cipher Key Lair
            new EncounterStaticShadow(Hypno)     { Fateful = true, Species = 097, Level = 34, Gauge = 05500, Moves = new[] {094,226,096,247}, Location = 059, }, // Hypno: Cipher Admin Gorigan @ Cipher Key Lair
            new EncounterStaticShadow(Banette)   { Fateful = true, Species = 354, Level = 37, Gauge = 07000, Moves = new[] {185,270,247,174}, Location = 059, }, // Banette: Cipher Peon Litnar @ Citadark Isle

            new EncounterStaticShadow(Pidgeotto) { Fateful = true, Species = 017, Level = 30, Gauge = 04000, Moves = new[] {017,287,211,297}, Location = 066, }, // Pidgeotto: Cipher Peon Lok @ Cipher Key Lair
            new EncounterStaticShadow(Tangela)   { Fateful = true, Species = 114, Level = 30, Gauge = 04000, Moves = new[] {076,234,241,275}, Location = 067, }, // Tangela: Cipher Peon Targ @ Cipher Key Lair
            new EncounterStaticShadow(Butterfree){ Fateful = true, Species = 012, Level = 30, Gauge = 04000, Moves = new[] {094,234,079,332}, Location = 067, }, // Butterfree: Cipher Peon Targ @ Cipher Key Lair
            new EncounterStaticShadow(Magneton)  { Fateful = true, Species = 082, Level = 30, Gauge = 04500, Moves = new[] {038,287,240,087}, Location = 067, }, // Magneton: Cipher Peon Snidle @ Cipher Key Lair
            new EncounterStaticShadow(Venomoth)  { Fateful = true, Species = 049, Level = 32, Gauge = 04000, Moves = new[] {318,287,164,094}, Location = 070, }, // Venomoth: Cipher Peon Angic @ Cipher Key Lair
            new EncounterStaticShadow(Weepinbell){ Fateful = true, Species = 070, Level = 32, Gauge = 04000, Moves = new[] {345,234,188,230}, Location = 070, }, // Weepinbell: Cipher Peon Angic @ Cipher Key Lair
            new EncounterStaticShadow(Arbok)     { Fateful = true, Species = 024, Level = 33, Gauge = 05000, Moves = new[] {188,287,137,044}, Location = 070, }, // Arbok: Cipher Peon Smarton @ Cipher Key Lair
            new EncounterStaticShadow(Primeape)  { Fateful = true, Species = 057, Level = 34, Gauge = 06000, Moves = new[] {238,270,116,179}, Location = 069, }, // Primeape: Cipher Admin Gorigan @ Cipher Key Lair
            new EncounterStaticShadow(Hypno)     { Fateful = true, Species = 097, Level = 34, Gauge = 05500, Moves = new[] {094,226,096,247}, Location = 069, }, // Hypno: Cipher Admin Gorigan @ Cipher Key Lair
            new EncounterStaticShadow(Golduck)   { Fateful = true, Species = 055, Level = 33, Gauge = 06500, Moves = new[] {127,204,244,280}, Location = 088, }, // Golduck: Navigator Abson @ Citadark Isle
            new EncounterStaticShadow(Sableye)   { Fateful = true, Species = 302, Level = 33, Gauge = 07000, Moves = new[] {247,270,185,105}, Location = 088, }, // Sableye: Navigator Abson @ Citadark Isle
            new EncounterStaticShadow(Dodrio)    { Fateful = true, Species = 085, Level = 34, Gauge = 08000, Moves = new[] {065,226,097,161}, Location = 076, }, // Dodrio: Chaser Furgy @ Citadark Isle
            new EncounterStaticShadow(Raticate)  { Fateful = true, Species = 020, Level = 34, Gauge = 06000, Moves = new[] {162,287,184,158}, Location = 076, }, // Raticate: Chaser Furgy @ Citadark Isle
            new EncounterStaticShadow(Farfetchd) { Fateful = true, Species = 083, Level = 36, Gauge = 05500, Moves = new[] {163,226,014,332}, Location = 076, }, // Farfetch'd: Cipher Admin Lovrina @ Citadark Isle
            new EncounterStaticShadow(Altaria)   { Fateful = true, Species = 334, Level = 36, Gauge = 06500, Moves = new[] {225,215,076,332}, Location = 076, }, // Altaria: Cipher Admin Lovrina @ Citadark Isle
            new EncounterStaticShadow(Kangaskhan){ Fateful = true, Species = 115, Level = 35, Gauge = 06000, Moves = new[] {089,047,039,146}, Location = 085, }, // Kangaskhan: Cipher Peon Litnar @ Citadark Isle
            new EncounterStaticShadow(Banette)   { Fateful = true, Species = 354, Level = 37, Gauge = 07000, Moves = new[] {185,270,247,174}, Location = 085, }, // Banette: Cipher Peon Litnar @ Citadark Isle
            new EncounterStaticShadow(Magmar)    { Fateful = true, Species = 126, Level = 36, Gauge = 07000, Moves = new[] {126,266,238,009}, Location = 077, }, // Magmar: Cipher Peon Grupel @ Citadark Isle
            new EncounterStaticShadow(Pinsir)    { Fateful = true, Species = 127, Level = 35, Gauge = 07000, Moves = new[] {012,270,206,066}, Location = 077, }, // Pinsir: Cipher Peon Grupel @ Citadark Isle
            new EncounterStaticShadow(Rapidash)  { Fateful = true, Species = 078, Level = 40, Gauge = 06000, Moves = new[] {076,226,241,053}, Location = 080, }, // Rapidash: Cipher Peon Kolest @ Citadark Isle
            new EncounterStaticShadow(Magcargo)  { Fateful = true, Species = 219, Level = 38, Gauge = 05500, Moves = new[] {257,287,089,053}, Location = 080, }, // Magcargo: Cipher Peon Kolest @ Citadark Isle
            new EncounterStaticShadow(Hitmonchan){ Fateful = true, Species = 107, Level = 38, Gauge = 06000, Moves = new[] {005,270,170,327}, Location = 081, }, // Hitmonchan: Cipher Peon Karbon @ Citadark Isle
            new EncounterStaticShadow(Hitmonlee) { Fateful = true, Species = 106, Level = 38, Gauge = 07000, Moves = new[] {136,287,170,025}, Location = 081, }, // Hitmonlee: Cipher Peon Petro @ Citadark Isle
            new EncounterStaticShadow(Lickitung) { Fateful = true, Species = 108, Level = 38, Gauge = 05000, Moves = new[] {038,270,111,205}, Location = 084, }, // Lickitung: Cipher Peon Geftal @ Citadark Isle
            new EncounterStaticShadow(Scyther)   { Fateful = true, Species = 123, Level = 40, Gauge = 08000, Moves = new[] {013,234,318,163}, Location = 084, }, // Scyther: Cipher Peon Leden @ Citadark Isle

            new EncounterStaticShadow(Chansey)   { Fateful = true, Species = 113, Level = 39, Gauge = 04000, Moves = new[] {085,186,135,285}, Location = 084, }, // Chansey: Cipher Peon Leden @ Citadark Isle
            new EncounterStaticShadow(Chansey)   { Fateful = true, Species = 113, Level = 39, Gauge = 04000, Moves = new[] {085,186,135,285}, Location = 087, }, // Chansey: Cipher Peon Leden @ Citadark Isle

            new EncounterStaticShadow(Solrock)   { Fateful = true, Species = 338, Level = 41, Gauge = 07500, Moves = new[] {094,226,241,322}, Location = 087, }, // Solrock: Cipher Admin Snattle @ Citadark Isle
            new EncounterStaticShadow(Starmie)   { Fateful = true, Species = 121, Level = 41, Gauge = 07500, Moves = new[] {127,287,058,105}, Location = 087, }, // Starmie: Cipher Admin Snattle @ Citadark Isle
            new EncounterStaticShadow(Electabuzz){ Fateful = true, Species = 125, Level = 43, Gauge = 07000, Moves = new[] {238,266,086,085}, Location = 087, }, // Electabuzz: Cipher Admin Ardos @ Citadark Isle
            new EncounterStaticShadow(Snorlax)   { Fateful = true, Species = 143, Level = 43, Gauge = 09000, Moves = new[] {090,287,174,034}, Location = 087, }, // Snorlax: Cipher Admin Ardos @ Citadark Isle
            new EncounterStaticShadow(Poliwrath) { Fateful = true, Species = 062, Level = 42, Gauge = 07500, Moves = new[] {056,270,240,280}, Location = 087, }, // Poliwrath: Cipher Admin Gorigan @ Citadark Isle
            new EncounterStaticShadow(MrMime)    { Fateful = true, Species = 122, Level = 42, Gauge = 06500, Moves = new[] {094,266,227,009}, Location = 087, }, // Mr. Mime: Cipher Admin Gorigan @ Citadark Isle
            new EncounterStaticShadow(Dugtrio)   { Fateful = true, Species = 051, Level = 40, Gauge = 05000, Moves = new[] {089,204,201,161}, Location = 075, }, // Dugtrio: Cipher Peon Kolax @ Citadark Isle
            new EncounterStaticShadow(Manectric) { Fateful = true, Species = 310, Level = 44, Gauge = 07000, Moves = new[] {087,287,240,044}, Location = 073, }, // Manectric: Cipher Admin Eldes @ Citadark Isle
            new EncounterStaticShadow(Salamence) { Fateful = true, Species = 373, Level = 50, Gauge = 09000, Moves = new[] {337,287,349,332}, Location = 073, }, // Salamence: Cipher Admin Eldes @ Citadark Isle
            new EncounterStaticShadow(Marowak)   { Fateful = true, Species = 105, Level = 44, Gauge = 06500, Moves = new[] {089,047,014,157}, Location = 073, }, // Marowak: Cipher Admin Eldes @ Citadark Isle
            new EncounterStaticShadow(Lapras)    { Fateful = true, Species = 131, Level = 44, Gauge = 06000, Moves = new[] {056,215,240,059}, Location = 073, }, // Lapras: Cipher Admin Eldes @ Citadark Isle
            new EncounterStaticShadow(Moltres)   { Fateful = true, Species = 146, Level = 50, Gauge = 10000, Moves = new[] {326,234,261,053}, Location = 074, }, // Moltres: Grand Master Greevil @ Citadark Isle
            new EncounterStaticShadow(Exeggutor) { Fateful = true, Species = 103, Level = 46, Gauge = 09000, Moves = new[] {094,287,095,246}, Location = 074, }, // Exeggutor: Grand Master Greevil @ Citadark Isle
            new EncounterStaticShadow(Tauros)    { Fateful = true, Species = 128, Level = 46, Gauge = 09000, Moves = new[] {089,287,039,034}, Location = 074, }, // Tauros: Grand Master Greevil @ Citadark Isle
            new EncounterStaticShadow(Articuno)  { Fateful = true, Species = 144, Level = 50, Gauge = 10000, Moves = new[] {326,215,114,058}, Location = 074, }, // Articuno: Grand Master Greevil @ Citadark Isle
            new EncounterStaticShadow(Zapdos)    { Fateful = true, Species = 145, Level = 50, Gauge = 10000, Moves = new[] {326,226,319,085}, Location = 074, }, // Zapdos: Grand Master Greevil @ Citadark Isle
            new EncounterStaticShadow(Dragonite) { Fateful = true, Species = 149, Level = 55, Gauge = 09000, Moves = new[] {063,215,349,089}, Location = 162, }, // Dragonite: Wanderer Miror B. @ Gateon Port
        }.SelectMany(CloneMirorB).ToArray();

        internal static readonly EncounterArea3[] SlotsXD =
        {
            new EncounterArea3 { Location = 090, Slots = new[] // Rock
                {
                    new EncounterSlot {Species = 027, LevelMin = 10, LevelMax = 23, SlotNumber = 0}, // Sandshrew
                    new EncounterSlot {Species = 207, LevelMin = 10, LevelMax = 20, SlotNumber = 1}, // Gligar
                    new EncounterSlot {Species = 328, LevelMin = 10, LevelMax = 20, SlotNumber = 2}, // Trapinch
                }
            },
            new EncounterArea3 { Location = 091, Slots = new[] // Oasis
                {
                    new EncounterSlot {Species = 187, LevelMin = 10, LevelMax = 20, SlotNumber = 0}, // Hoppip
                    new EncounterSlot {Species = 231, LevelMin = 10, LevelMax = 20, SlotNumber = 1}, // Phanpy
                    new EncounterSlot {Species = 283, LevelMin = 10, LevelMax = 20, SlotNumber = 2}, // Surskit
                }
            },
            new EncounterArea3 { Location = 092, Slots = new[] // Cave
                {
                    new EncounterSlot {Species = 041, LevelMin = 10, LevelMax = 21, SlotNumber = 0}, // Zubat
                    new EncounterSlot {Species = 304, LevelMin = 10, LevelMax = 21, SlotNumber = 1}, // Aron
                    new EncounterSlot {Species = 194, LevelMin = 10, LevelMax = 21, SlotNumber = 2}, // Wooper
                }
            },
        };

        internal static readonly EncounterStatic[] Encounter_CXD = ArrayUtil.ConcatAll(Encounter_Colo, Encounter_XD);

        private static IEnumerable<EncounterStatic> CloneMirorB(EncounterStatic arg)
        {
            yield return arg;
            foreach (int loc in MirorBXDLocations)
                yield return arg.Clone(loc);
        }

        #endregion
    }
}
