using static PKHeX.Core.EncounterUtil;
using static PKHeX.Core.GameVersion;

namespace PKHeX.Core
{
    /// <summary>
    /// Generation 3 Encounters
    /// </summary>
    internal static class Encounters3
    {
        private static readonly EncounterArea3[] SlotsSwarmRSE = GetSwarm("rse_swarm", "rs", RSE);
        internal static readonly EncounterArea3[] SlotsR = ArrayUtil.ConcatAll(Get("r", "ru", R), SlotsSwarmRSE);
        internal static readonly EncounterArea3[] SlotsS = ArrayUtil.ConcatAll(Get("s", "sa", S), SlotsSwarmRSE);
        internal static readonly EncounterArea3[] SlotsE = ArrayUtil.ConcatAll(Get("e", "em", E), SlotsSwarmRSE);
        internal static readonly EncounterArea3[] SlotsFR = Get("fr", "fr", FR);
        internal static readonly EncounterArea3[] SlotsLG = Get("lg", "lg", LG);

        private static byte[][] ReadUnpack(string resource, string ident) => BinLinker.Unpack(Util.GetBinaryResource($"encounter_{resource}.pkl"), ident);
        private static EncounterArea3[] Get(string resource, string ident, GameVersion game) => EncounterArea3.GetAreas(ReadUnpack(resource, ident), game);
        private static EncounterArea3[] GetSwarm(string resource, string ident, GameVersion game) => EncounterArea3.GetAreasSwarm(ReadUnpack(resource, ident), game);

        static Encounters3()
        {
            MarkEncounterTradeStrings(TradeGift_RSE, TradeRSE);
            MarkEncounterTradeStrings(TradeGift_FRLG, TradeFRLG);
        }

        private static readonly EncounterStatic3[] Encounter_RSE_Roam =
        {
            new(380, 40, S) { Roaming = true, Location = 016 }, // Latias
            new(380, 40, E) { Roaming = true, Location = 016 }, // Latias
            new(381, 40, R) { Roaming = true, Location = 016 }, // Latios
            new(381, 40, E) { Roaming = true, Location = 016 }, // Latios
        };

        private static readonly EncounterStatic3[] Encounter_RSE_Regular =
        {
            // Starters
            new(152, 05,  E  ) { Gift = true, Location = 000 }, // Chikorita @ Littleroot Town
            new(155, 05,  E  ) { Gift = true, Location = 000 }, // Cyndaquil
            new(158, 05,  E  ) { Gift = true, Location = 000 }, // Totodile
            new(252, 05,  RSE) { Gift = true, Location = 016 }, // Treecko @ Route 101
            new(255, 05,  RSE) { Gift = true, Location = 016 }, // Torchic
            new(258, 05,  RSE) { Gift = true, Location = 016 }, // Mudkip

            // Fossil @ Rustboro City
            new(345, 20, RSE) { Gift = true, Location = 010, }, // Lileep
            new(347, 20, RSE) { Gift = true, Location = 010, }, // Anorith

            // Gift
            new(351, 25, RSE) { Gift = true, Location = 034,    }, // Castform @ Weather Institute
            new(374, 05, RSE) { Gift = true, Location = 013,    }, // Beldum @ Mossdeep City
            new(360, 05, RSE) { Gift = true, EggLocation = 253, }, // Wynaut Egg

            // Stationary
            new(352, 30, RSE) { Location = 034 }, // Kecleon @ Route 119
            new(352, 30, RSE) { Location = 035 }, // Kecleon @ Route 120
            new(101, 30, RS ) { Location = 066 }, // Electrode @ Hideout (R:Magma Hideout/S:Aqua Hideout)
            new(101, 30, E  ) { Location = 197 }, // Electrode @ Aqua Hideout
            new(185, 40, E  ) { Location = 058 }, // Sudowoodo @ Battle Frontier

            // Stationary Lengendary
            new(377, 40, RSE) { Location = 082, }, // Regirock @ Desert Ruins
            new(378, 40, RSE) { Location = 081, }, // Regice @ Island Cave
            new(379, 40, RSE) { Location = 083, }, // Registeel @ Ancient Tomb
            new(380, 50, R  ) { Location = 073, }, // Latias @ Southern Island
            new(380, 50,   E) { Location = 073, Fateful = true }, // Latias @ Southern Island
            new(381, 50,  S ) { Location = 073, }, // Latios @ Southern Island
            new(381, 50,   E) { Location = 073, Fateful = true }, // Latios @ Southern Island
            new(382, 45,  S ) { Location = 072, }, // Kyogre @ Cave of Origin
            new(382, 70,   E) { Location = 203, }, // Kyogre @ Marine Cave
            new(383, 45, R  ) { Location = 072, }, // Groudon @ Cave of Origin
            new(383, 70,   E) { Location = 205, }, // Groudon @ Terra Cave
            new(384, 70, RSE) { Location = 085, }, // Rayquaza @ Sky Pillar

            // Event
            new(151, 30, E) { Location = 201, Fateful = true }, // Mew @ Faraway Island (Unreleased outside of Japan)
            new(249, 70, E) { Location = 211, Fateful = true }, // Lugia @ Navel Rock
            new(250, 70, E) { Location = 211, Fateful = true }, // Ho-Oh @ Navel Rock
            new(386, 30, E) { Location = 200, Fateful = true, Form = 3 }, // Deoxys @ Birth Island
        };

        private static readonly EncounterStatic3[] Encounter_FRLG_Roam =
        {
            new(243, 50, FRLG) { Roaming = true, Location = 16 }, // Raikou
            new(244, 50, FRLG) { Roaming = true, Location = 16 }, // Entei
            new(245, 50, FRLG) { Roaming = true, Location = 16 }, // Suicune
        };

        private static readonly EncounterStatic3[] Encounter_FRLG_Stationary =
        {
            // Starters @ Pallet Town
            new(001, 05, FRLG) { Gift = true, Location = 088, }, // Bulbasaur
            new(004, 05, FRLG) { Gift = true, Location = 088, }, // Charmander
            new(007, 05, FRLG) { Gift = true, Location = 088, }, // Squirtle

            // Fossil @ Cinnabar Island
            new(138, 05, FRLG) { Gift = true, Location = 096, }, // Omanyte
            new(140, 05, FRLG) { Gift = true, Location = 096, }, // Kabuto
            new(142, 05, FRLG) { Gift = true, Location = 096, }, // Aerodactyl

            // Gift
            new(106, 25, FRLG) { Gift = true, Location = 098, }, // Hitmonlee @ Saffron City
            new(107, 25, FRLG) { Gift = true, Location = 098, }, // Hitmonchan @ Saffron City
            new(129, 05, FRLG) { Gift = true, Location = 099, }, // Magikarp @ Route 4
            new(131, 25, FRLG) { Gift = true, Location = 134, }, // Lapras @ Silph Co.
            new(133, 25, FRLG) { Gift = true, Location = 094, }, // Eevee @ Celadon City
            new(175, 05, FRLG) { Gift = true, EggLocation = 253 }, // Togepi Egg

            // Celadon City Game Corner
            new(063, 09, FR) { Gift = true, Location = 94 }, // Abra
            new(035, 08, FR) { Gift = true, Location = 94 }, // Clefairy
            new(123, 25, FR) { Gift = true, Location = 94 }, // Scyther
            new(147, 18, FR) { Gift = true, Location = 94 }, // Dratini
            new(137, 26, FR) { Gift = true, Location = 94 }, // Porygon

            new(063, 07, LG) { Gift = true, Location = 94 }, // Abra
            new(035, 12, LG) { Gift = true, Location = 94 }, // Clefairy
            new(127, 18, LG) { Gift = true, Location = 94 }, // Pinsir
            new(147, 24, LG) { Gift = true, Location = 94 }, // Dratini
            new(137, 18, LG) { Gift = true, Location = 94 }, // Porygon

            // Stationary
            new(143, 30, FRLG) { Location = 112, }, // Snorlax @ Route 12
            new(143, 30, FRLG) { Location = 116, }, // Snorlax @ Route 16
            new(101, 34, FRLG) { Location = 142, }, // Electrode @ Power Plant
            new(097, 30, FRLG) { Location = 176, }, // Hypno @ Berry Forest

            // Stationary Legendary
            new(144, 50, FRLG) { Location = 139, }, // Articuno @ Seafoam Islands
            new(145, 50, FRLG) { Location = 142, }, // Zapdos @ Power Plant
            new(146, 50, FRLG) { Location = 175, }, // Moltres @ Mt. Ember.
            new(150, 70, FRLG) { Location = 141, }, // Mewtwo @ Cerulean Cave

            // Event
            new(249, 70, FRLG) { Location = 174, Fateful = true }, // Lugia @ Navel Rock
            new(250, 70, FRLG) { Location = 174, Fateful = true }, // Ho-Oh @ Navel Rock
            new(386, 30, FR  ) { Location = 187, Fateful = true, Form = 1, }, // Deoxys @ Birth Island
            new(386, 30,   LG) { Location = 187, Fateful = true, Form = 2, }, // Deoxys @ Birth Island
        };

        private static readonly EncounterStatic3[] Encounter_RSE = ArrayUtil.ConcatAll(Encounter_RSE_Roam, Encounter_RSE_Regular);
        private static readonly EncounterStatic3[] Encounter_FRLG = ArrayUtil.ConcatAll(Encounter_FRLG_Roam, Encounter_FRLG_Stationary);

        private static readonly byte[] TradeContest_Cool =   { 30, 05, 05, 05, 05, 10 };
        private static readonly byte[] TradeContest_Beauty = { 05, 30, 05, 05, 05, 10 };
        private static readonly byte[] TradeContest_Cute =   { 05, 05, 30, 05, 05, 10 };
        private static readonly byte[] TradeContest_Clever = { 05, 05, 05, 30, 05, 10 };
        private static readonly byte[] TradeContest_Tough =  { 05, 05, 05, 05, 30, 10 };

        internal static readonly EncounterTrade3[] TradeGift_RSE =
        {
            new(RS, 0x00009C40, 296, 05) { Ability = 2, TID = 49562, SID = 00000, OTGender = 0, Gender = 0, IVs = new[] {5,5,4,4,4,4}, Contest = TradeContest_Tough }, // Slakoth (Level 5 Breeding) -> Makuhita
            new(RS, 0x498A2E17, 300, 03) { Ability = 1, TID = 02259, SID = 00000, OTGender = 1, Gender = 1, IVs = new[] {5,4,4,5,4,4}, Contest = TradeContest_Cute }, // Pikachu (Level 3 Viridian Forest) -> Skitty
            new(RS, 0x4C970B7F, 222, 21) { Ability = 2, TID = 50183, SID = 00000, OTGender = 1, Gender = 1, IVs = new[] {4,4,5,4,4,5}, Contest = TradeContest_Beauty }, // Bellossom (Level 21 Oddish -> Gloom -> Bellossom) -> Corsola
            new(E , 0x00000084, 273, 04) { Ability = 2, TID = 38726, SID = 00000, OTGender = 0, Gender = 0, IVs = new[] {5,4,5,4,4,4}, Contest = TradeContest_Cool }, // Ralts (Level 4 Route 102) -> Seedot
            new(E , 0x0000006F, 311, 05) { Ability = 1, TID = 08460, SID = 00001, OTGender = 0, Gender = 1, IVs = new[] {4,4,4,5,5,4}, Contest = TradeContest_Cute }, // Volbeat (Level 5 Breeding) -> Plusle
            new(E , 0x0000007F, 116, 05) { Ability = 1, TID = 46285, SID = 00000, OTGender = 0, Gender = 0, IVs = new[] {5,4,4,4,5,4}, Contest = TradeContest_Tough }, // Bagon (Level 5 Breeding) -> Horsea*
            new(E , 0x0000008B, 052, 03) { Ability = 1, TID = 25945, SID = 00001, OTGender = 1, Gender = 0, IVs = new[] {4,5,4,5,4,4}, Contest = TradeContest_Clever }, // Skitty (Level 3 Trade)-> Meowth*
            //  If Pokémon with * is evolved in a Generation IV or V game, its Ability will become its second Ability.
        };

        internal static readonly EncounterTrade3[] TradeGift_FRLG =
        {
            new(FRLG, 0x00009CAE, 122, 05) { Ability = 1, TID = 01985, SID = 00000, OTGender = 0, Gender = 0, IVs = new[] {20,15,17,24,23,22}, Contest = TradeContest_Clever }, // Abra (Level 5 Breeding) -> Mr. Mime
            new(FR  , 0x4C970B89, 029, 05) { Ability = 1, TID = 63184, SID = 00000, OTGender = 1, Gender = 1, IVs = new[] {22,18,25,19,15,22}, Contest = TradeContest_Tough }, // Nidoran♀
            new(  LG, 0x4C970B9E, 032, 05) { Ability = 1, TID = 63184, SID = 00000, OTGender = 1, Gender = 0, IVs = new[] {19,25,18,22,22,15}, Contest = TradeContest_Cool }, // Nidoran♂ *
            new(FR  , 0x00EECA15, 030, 16) { Ability = 1, TID = 13637, SID = 00000, OTGender = 0, Gender = 1, IVs = new[] {22,25,18,19,22,15}, Contest = TradeContest_Cute }, // Nidorina *
            new(  LG, 0x00EECA19, 033, 16) { Ability = 1, TID = 13637, SID = 00000, OTGender = 0, Gender = 0, IVs = new[] {19,18,25,22,15,22}, Contest = TradeContest_Tough }, // Nidorino  *
            new(FR  , 0x451308AB, 108, 25) { Ability = 1, TID = 01239, SID = 00000, OTGender = 0, Gender = 0, IVs = new[] {24,19,21,15,23,21}, Contest = TradeContest_Tough }, // Golduck (Level 25) -> Lickitung  *
            new(  LG, 0x451308AB, 108, 25) { Ability = 1, TID = 01239, SID = 00000, OTGender = 0, Gender = 0, IVs = new[] {24,19,21,15,23,21}, Contest = TradeContest_Tough }, // Slowbro (Level 25) -> Lickitung  *
            new(FRLG, 0x498A2E1D, 124, 20) { Ability = 1, TID = 36728, SID = 00000, OTGender = 0, Gender = 1, IVs = new[] {18,17,18,22,25,21}, Contest = TradeContest_Beauty }, // Poliwhirl (Level 20) -> Jynx
            new(FRLG, 0x151943D7, 083, 03) { Ability = 1, TID = 08810, SID = 00000, OTGender = 0, Gender = 0, IVs = new[] {20,25,21,24,15,20}, Contest = TradeContest_Cool }, // Spearow (Level 3 Capture) -> Farfetch'd
            new(FRLG, 0x06341016, 101, 03) { Ability = 2, TID = 50298, SID = 00000, OTGender = 0, Gender = 2, IVs = new[] {19,16,18,25,25,19}, Contest = TradeContest_Cool }, // Raichu (Level 3) -> Electrode
            new(FRLG, 0x5C77ECFA, 114, 05) { Ability = 1, TID = 60042, SID = 00000, OTGender = 1, Gender = 0, IVs = new[] {22,17,25,16,23,20}, Contest = TradeContest_Cute }, // Venonat (Level 5 Breeding) -> Tangela
            new(FRLG, 0x482CAC89, 086, 05) { Ability = 1, TID = 09853, SID = 00000, OTGender = 0, Gender = 0, IVs = new[] {24,15,22,16,23,22}, Contest = TradeContest_Tough }, // Ponyta (Level 5 Breeding) -> Seel *
            //  If Pokémon with * is evolved in a Generation IV or V game, its Ability will become its second Ability.
        };

        private const string tradeRSE = "traderse";
        private const string tradeFRLG = "tradefrlg";
        private static readonly string[][] TradeRSE = Util.GetLanguageStrings7(tradeRSE);
        private static readonly string[][] TradeFRLG = Util.GetLanguageStrings7(tradeFRLG);

        internal static readonly EncounterStatic3[] StaticR = GetEncounters(Encounter_RSE, R);
        internal static readonly EncounterStatic3[] StaticS = GetEncounters(Encounter_RSE, S);
        internal static readonly EncounterStatic3[] StaticE = GetEncounters(Encounter_RSE, E);
        internal static readonly EncounterStatic3[] StaticFR = GetEncounters(Encounter_FRLG, FR);
        internal static readonly EncounterStatic3[] StaticLG = GetEncounters(Encounter_FRLG, LG);
    }
}
