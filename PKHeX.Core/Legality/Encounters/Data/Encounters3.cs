using static PKHeX.Core.EncounterUtil;
using static PKHeX.Core.Encounters3Teams;
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

        private static readonly int[] TradeContest_Cool =   { 30, 05, 05, 05, 05, 10 };
        private static readonly int[] TradeContest_Beauty = { 05, 30, 05, 05, 05, 10 };
        private static readonly int[] TradeContest_Cute =   { 05, 05, 30, 05, 05, 10 };
        private static readonly int[] TradeContest_Clever = { 05, 05, 05, 30, 05, 10 };
        private static readonly int[] TradeContest_Tough =  { 05, 05, 05, 05, 30, 10 };

        internal static readonly EncounterTrade3[] TradeGift_RSE =
        {
            new(0x00009C40) { Species = 296, Ability = 2, TID = 49562, SID = 00000, OTGender = 0, Gender = 0, IVs = new[] {5,5,4,4,4,4}, Level = 05, Contest = TradeContest_Tough, Version = RS, }, // Slakoth (Level 5 Breeding) -> Makuhita
            new(0x498A2E17) { Species = 300, Ability = 1, TID = 02259, SID = 00000, OTGender = 1, Gender = 1, IVs = new[] {5,4,4,5,4,4}, Level = 03, Contest = TradeContest_Cute, Version = RS, }, // Pikachu (Level 3 Viridiam Forest) -> Skitty
            new(0x4C970B7F) { Species = 222, Ability = 2, TID = 50183, SID = 00000, OTGender = 1, Gender = 1, IVs = new[] {4,4,5,4,4,5}, Level = 21, Contest = TradeContest_Beauty, Version = RS, }, // Bellossom (Level 21 Odish -> Gloom -> Bellossom) -> Corsola
            new(0x00000084) { Species = 273, Ability = 2, TID = 38726, SID = 00000, OTGender = 0, Gender = 0, IVs = new[] {5,4,5,4,4,4}, Level = 04, Contest = TradeContest_Cool, Version = E, }, // Ralts (Level 4 Route 102) -> Seedot
            new(0x0000006F) { Species = 311, Ability = 1, TID = 08460, SID = 00001, OTGender = 0, Gender = 1, IVs = new[] {4,4,4,5,5,4}, Level = 05, Contest = TradeContest_Cute, Version = E, }, // Volbeat (Level 5 Breeding) -> Plusle
            new(0x0000007F) { Species = 116, Ability = 1, TID = 46285, SID = 00000, OTGender = 0, Gender = 0, IVs = new[] {5,4,4,4,5,4}, Level = 05, Contest = TradeContest_Tough, Version = E, }, // Bagon (Level 5 Breeding) -> Horsea*
            new(0x0000008B) { Species = 052, Ability = 1, TID = 25945, SID = 00001, OTGender = 1, Gender = 0, IVs = new[] {4,5,4,5,4,4}, Level = 03, Contest = TradeContest_Clever, Version = E, }, // Skitty (Level 3 Trade)-> Meowth*
            //  If Pokémon with * is evolved in a Generation IV or V game, its Ability will become its second Ability.
        };

        internal static readonly EncounterTrade3[] TradeGift_FRLG =
        {
            new(0x00009CAE) { Species = 122, Ability = 1, TID = 01985, SID = 00000, OTGender = 0, Gender = 0, IVs = new[] {20,15,17,24,23,22}, Level = 05, Contest = TradeContest_Clever, Version = FRLG, }, // Abra (Level 5 Breeding) -> Mr. Mime
            new(0x4C970B89) { Species = 029, Ability = 1, TID = 63184, SID = 00000, OTGender = 1, Gender = 1, IVs = new[] {22,18,25,19,15,22}, Level = 05, Contest = TradeContest_Tough, Version = FR, }, // Nidoran♀
            new(0x4C970B9E) { Species = 032, Ability = 1, TID = 63184, SID = 00000, OTGender = 1, Gender = 0, IVs = new[] {19,25,18,22,22,15}, Level = 05, Contest = TradeContest_Cool, Version = LG, }, // Nidoran♂ *
            new(0x00EECA15) { Species = 030, Ability = 1, TID = 13637, SID = 00000, OTGender = 0, Gender = 1, IVs = new[] {22,25,18,19,22,15}, Level = 16, Contest = TradeContest_Cute, Version = FR,}, // Nidorina *
            new(0x00EECA19) { Species = 033, Ability = 1, TID = 13637, SID = 00000, OTGender = 0, Gender = 0, IVs = new[] {19,18,25,22,15,22}, Level = 16, Contest = TradeContest_Tough, Version = LG,}, // Nidorino  *
            new(0x451308AB) { Species = 108, Ability = 1, TID = 01239, SID = 00000, OTGender = 0, Gender = 0, IVs = new[] {24,19,21,15,23,21}, Level = 25, Contest = TradeContest_Tough, Version = FR, }, // Golduck (Level 25) -> Lickitung  *
            new(0x451308AB) { Species = 108, Ability = 1, TID = 01239, SID = 00000, OTGender = 0, Gender = 0, IVs = new[] {24,19,21,15,23,21}, Level = 25, Contest = TradeContest_Tough, Version = LG, }, // Slowbro (Level 25) -> Lickitung  *
            new(0x498A2E1D) { Species = 124, Ability = 1, TID = 36728, SID = 00000, OTGender = 0, Gender = 1, IVs = new[] {18,17,18,22,25,21}, Level = 20, Contest = TradeContest_Beauty, Version = FRLG, }, // Poliwhirl (Level 20) -> Jynx
            new(0x151943D7) { Species = 083, Ability = 1, TID = 08810, SID = 00000, OTGender = 0, Gender = 0, IVs = new[] {20,25,21,24,15,20}, Level = 03, Contest = TradeContest_Cool, Version = FRLG, }, // Spearow (Level 3 Capture) -> Farfetch'd
            new(0x06341016) { Species = 101, Ability = 2, TID = 50298, SID = 00000, OTGender = 0, Gender = 2, IVs = new[] {19,16,18,25,25,19}, Level = 03, Contest = TradeContest_Cool, Version = FRLG, }, // Raichu (Level 3) -> Electrode
            new(0x5C77ECFA) { Species = 114, Ability = 1, TID = 60042, SID = 00000, OTGender = 1, Gender = 0, IVs = new[] {22,17,25,16,23,20}, Level = 05, Contest = TradeContest_Cute, Version = FRLG, }, // Venonat (Level 5 Breeding) -> Tangela
            new(0x482CAC89) { Species = 086, Ability = 1, TID = 09853, SID = 00000, OTGender = 0, Gender = 0, IVs = new[] {24,15,22,16,23,22}, Level = 05, Contest = TradeContest_Tough, Version = FRLG, }, // Ponyta (Level 5 Breeding) -> Seel *
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

    internal static class Encounters3GC
    {
        static Encounters3GC()
        {
            Encounter_Colo.SetVersion(COLO);
            Encounter_XD.SetVersion(XD);
        }

        #region Colosseum

        private static readonly EncounterStatic3[] Encounter_ColoGift =
        {
            // Colosseum Starters: Gender locked to male
            new(196, 25, COLO) { Gift = true, Location = 254, Gender = 0 }, // Espeon
            new(197, 26, COLO) { Gift = true, Location = 254, Gender = 0, Moves = new[] {044} }, // Umbreon (Bite)
        };

        private static readonly EncounterStaticShadow[] Encounter_Colo =
        {
            new(ColoMakuhita) { Species = 296, Level = 30, Gauge = 03000, Moves = new[] {193,116,233,238}, Location = 005 }, // Makuhita: Miror B.Peon Trudly @ Phenac City

            new() { Species = 153, Level = 30, Gauge = 03000, Moves = new[] {241,235,075,034}, Location = 003 }, // Bayleef: Cipher Peon Verde @ Phenac City
            new() { Species = 156, Level = 30, Gauge = 03000, Moves = new[] {241,108,091,172}, Location = 003 }, // Quilava: Cipher Peon Rosso @ Phenac City
            new() { Species = 159, Level = 30, Gauge = 03000, Moves = new[] {240,184,044,057}, Location = 003 }, // Croconaw: Cipher Peon Bluno @ Phenac City
            new() { Species = 153, Level = 30, Gauge = 03000, Moves = new[] {241,235,075,034}, Location = 069 }, // Bayleef: Cipher Peon Verde @ Shadow PKMN Lab
            new() { Species = 156, Level = 30, Gauge = 03000, Moves = new[] {241,108,091,172}, Location = 069 }, // Quilava: Cipher Peon Rosso @ Shadow PKMN Lab
            new() { Species = 159, Level = 30, Gauge = 03000, Moves = new[] {240,184,044,057}, Location = 069 }, // Croconaw: Cipher Peon Bluno @ Shadow PKMN Lab
            new() { Species = 153, Level = 30, Gauge = 03000, Moves = new[] {241,235,075,034}, Location = 115 }, // Bayleef: Cipher Peon Verde @ Realgam Tower
            new() { Species = 156, Level = 30, Gauge = 03000, Moves = new[] {241,108,091,172}, Location = 115 }, // Quilava: Cipher Peon Rosso @ Realgam Tower
            new() { Species = 159, Level = 30, Gauge = 03000, Moves = new[] {240,184,044,057}, Location = 115 }, // Croconaw: Cipher Peon Bluno @ Realgam Tower
            new() { Species = 153, Level = 30, Gauge = 03000, Moves = new[] {241,235,075,034}, Location = 132 }, // Bayleef: Cipher Peon Verde @ Snagem Hideout
            new() { Species = 156, Level = 30, Gauge = 03000, Moves = new[] {241,108,091,172}, Location = 132 }, // Quilava: Cipher Peon Rosso @ Snagem Hideout
            new() { Species = 159, Level = 30, Gauge = 03000, Moves = new[] {240,184,044,057}, Location = 132 }, // Croconaw: Cipher Peon Bluno @ Snagem Hideout

            new() { Species = 218, Level = 30, Gauge = 04000, Moves = new[] {241,281,088,053}, Location = 015 }, // Slugma: Roller Boy Lon @ Pyrite Town
            new() { Species = 164, Level = 30, Gauge = 03000, Moves = new[] {211,095,115,019}, Location = 015 }, // Noctowl: Rider Nover @ Pyrite Town
            new() { Species = 180, Level = 30, Gauge = 03000, Moves = new[] {085,086,178,084}, Location = 015 }, // Flaaffy: St.Performer Diogo @ Pyrite Town
            new() { Species = 188, Level = 30, Gauge = 03000, Moves = new[] {235,079,178,072}, Location = 015 }, // Skiploom: Rider Leba @ Pyrite Town
            new() { Species = 195, Level = 30, Gauge = 04000, Moves = new[] {341,133,021,057}, Location = 015 }, // Quagsire: Bandana Guy Divel @ Pyrite Town
            new() { Species = 200, Level = 30, Gauge = 04000, Moves = new[] {060,109,212,247}, Location = 015 }, // Misdreavus: Rider Vant @ Pyrite Town
            new() { Species = 162, Level = 33, Gauge = 05000, Moves = new[] {231,270,098,070}, Location = 015 }, // Furret: Rogue Cail @ Pyrite Town

            new() { Species = 193, Level = 33, Gauge = 05000, Moves = new[] {197,048,049,253}, Location = 025 }, // Yanma: Cipher Peon Nore @ Pyrite Bldg
            new() { Species = 193, Level = 33, Gauge = 05000, Moves = new[] {197,048,049,253}, Location = 132 }, // Yanma: Cipher Peon Nore @ Snagem Hideout

            new() { Species = 223, Level = 20, Gauge = 04000, Moves = new[] {061,199,060,062}, Location = 028 }, // Remoraid: Miror B.Peon Reath @ Pyrite Bldg
            new() { Species = 223, Level = 20, Gauge = 04000, Moves = new[] {061,199,060,062}, Location = 030 }, // Remoraid: Miror B.Peon Reath @ Pyrite Cave
            new() { Species = 226, Level = 33, Gauge = 05000, Moves = new[] {017,048,061,036}, Location = 028 }, // Mantine: Miror B.Peon Ferma @ Pyrite Bldg
            new() { Species = 226, Level = 33, Gauge = 05000, Moves = new[] {017,048,061,036}, Location = 030 }, // Mantine: Miror B.Peon Ferma @ Pyrite Cave

            new() { Species = 211, Level = 33, Gauge = 05000, Moves = new[] {042,107,040,057}, Location = 015 }, // Qwilfish: Hunter Doken @ Pyrite Bldg
            new() { Species = 307, Level = 33, Gauge = 05000, Moves = new[] {197,347,093,136}, Location = 031 }, // Meditite: Rider Twan @ Pyrite Cave
            new() { Species = 206, Level = 33, Gauge = 05000, Moves = new[] {180,137,281,036}, Location = 029 }, // Dunsparce: Rider Sosh @ Pyrite Cave
            new() { Species = 333, Level = 33, Gauge = 05000, Moves = new[] {119,047,219,019}, Location = 032 }, // Swablu: Hunter Zalo @ Pyrite Cave

            new() { Species = 185, Level = 35, Gauge = 10000, Moves = new[] {175,335,067,157}, Location = 104 }, // Sudowoodo: Cipher Admin Miror B. @ Realgam Tower
            new() { Species = 185, Level = 35, Gauge = 10000, Moves = new[] {175,335,067,157}, Location = 125 }, // Sudowoodo: Cipher Admin Miror B. @ Deep Colosseum
            new() { Species = 185, Level = 35, Gauge = 10000, Moves = new[] {175,335,067,157}, Location = 030 }, // Sudowoodo: Cipher Admin Miror B. @ Pyrite Cave

            new() { Species = 237, Level = 38, Gauge = 06000, Moves = new[] {097,116,167,229}, Location = 039 }, // Hitmontop: Cipher Peon Skrub @ Agate Village
            new() { Species = 237, Level = 38, Gauge = 06000, Moves = new[] {097,116,167,229}, Location = 132 }, // Hitmontop: Cipher Peon Skrub @ Snagem Hideout
            new() { Species = 237, Level = 38, Gauge = 06000, Moves = new[] {097,116,167,229}, Location = 068 }, // Hitmontop: Cipher Peon Skrub @ Shadow PKMN Lab

            new() { Species = 166, Level = 40, Gauge = 06000, Moves = new[] {226,219,048,004}, Location = 047 }, // Ledian: Cipher Peon Kloak @ The Under
            new() { Species = 166, Level = 40, Gauge = 06000, Moves = new[] {226,219,048,004}, Location = 132 }, // Ledian: Cipher Peon Kloak @ Snagem Hideout

            new() { Species = 244, Level = 40, Gauge = 13000, Moves = new[] {241,043,044,126}, Location = 106 }, // Entei: Cipher Admin Dakim @ Realgam Tower
            new() { Species = 244, Level = 40, Gauge = 13000, Moves = new[] {241,043,044,126}, Location = 125 }, // Entei: Cipher Admin Dakim @ Deep Colosseum
            new() { Species = 244, Level = 40, Gauge = 13000, Moves = new[] {241,043,044,126}, Location = 076 }, // Entei: Cipher Admin Dakim @ Mt. Battle

            new() { Species = 245, Level = 40, Gauge = 13000, Moves = new[] {240,043,016,057}, Location = 110 }, // Suicune (Surf): Cipher Admin Venus @ Realgam Tower
            new() { Species = 245, Level = 40, Gauge = 13000, Moves = new[] {240,043,016,056}, Location = 125 }, // Suicune (Hydro Pump): Cipher Admin Venus @ Deep Colosseum
            new() { Species = 245, Level = 40, Gauge = 13000, Moves = new[] {240,043,016,057}, Location = 055 }, // Suicune (Surf): Cipher Admin Venus @ The Under

            new() { Species = 243, Level = 40, Gauge = 13000, Moves = new[] {240,043,098,087}, Location = 113 }, // Raikou: Cipher Admin Ein @ Realgam Tower
            new() { Species = 243, Level = 40, Gauge = 13000, Moves = new[] {240,043,098,087}, Location = 125 }, // Raikou: Cipher Admin Ein @ Deep Colosseum
            new() { Species = 243, Level = 40, Gauge = 13000, Moves = new[] {240,043,098,087}, Location = 069 }, // Raikou: Cipher Admin Ein @ Shadow PKMN Lab

            new() { Species = 234, Level = 43, Gauge = 06000, Moves = new[] {310,095,043,036}, Location = 058 }, // Stantler: Chaser Liaks @ The Under Subway
            new() { Species = 234, Level = 43, Gauge = 06000, Moves = new[] {310,095,043,036}, Location = 133 }, // Stantler: Chaser Liaks @ Snagem Hideout
            new() { Species = 221, Level = 43, Gauge = 06000, Moves = new[] {203,316,091,059}, Location = 058 }, // Piloswine: Bodybuilder Lonia @ The Under Subway
            new() { Species = 221, Level = 43, Gauge = 06000, Moves = new[] {203,316,091,059}, Location = 134 }, // Piloswine: Bodybuilder Lonia @ Snagem Hideout
            new() { Species = 215, Level = 43, Gauge = 06000, Moves = new[] {185,103,154,196}, Location = 058 }, // Sneasel: Rider Nelis @ The Under Subway
            new() { Species = 215, Level = 43, Gauge = 06000, Moves = new[] {185,103,154,196}, Location = 134 }, // Sneasel: Rider Nelis @ Snagem Hideout
            new() { Species = 190, Level = 43, Gauge = 06000, Moves = new[] {226,321,154,129}, Location = 067 }, // Aipom: Cipher Peon Cole @ Shadow PKMN Lab
            new() { Species = 205, Level = 43, Gauge = 06000, Moves = new[] {153,182,117,229}, Location = 067 }, // Forretress: Cipher Peon Vana @ Shadow PKMN Lab
            new() { Species = 168, Level = 43, Gauge = 06000, Moves = new[] {169,184,141,188}, Location = 069 }, // Ariados: Cipher Peon Lesar @ Shadow PKMN Lab
            new() { Species = 210, Level = 43, Gauge = 06000, Moves = new[] {044,184,046,070}, Location = 069 }, // Granbull: Cipher Peon Tanie @ Shadow PKMN Lab
            new() { Species = 329, Level = 43, Gauge = 06000, Moves = new[] {242,103,328,225}, Location = 068 }, // Vibrava: Cipher Peon Remil @ Shadow PKMN Lab

            new() { Species = 192, Level = 45, Gauge = 07000, Moves = new[] {241,074,275,076}, Location = 109 }, // Sunflora: Cipher Peon Baila @ Realgam Tower
            new() { Species = 225, Level = 45, Gauge = 07000, Moves = new[] {059,213,217,019}, Location = 109 }, // Delibird: Cipher Peon Arton @ Realgam Tower
            new() { Species = 227, Level = 47, Gauge = 13000, Moves = new[] {065,319,314,211}, Location = 117 }, // Skarmory: Snagem Head Gonzap @ Realgam Tower
            new() { Species = 192, Level = 45, Gauge = 07000, Moves = new[] {241,074,275,076}, Location = 132 }, // Sunflora: Cipher Peon Baila @ Snagem Hideout
            new() { Species = 225, Level = 45, Gauge = 07000, Moves = new[] {059,213,217,019}, Location = 132 }, // Delibird: Cipher Peon Arton @ Snagem Hideout
            new() { Species = 227, Level = 47, Gauge = 13000, Moves = new[] {065,319,314,211}, Location = 133 }, // Skarmory: Snagem Head Gonzap @ Snagem Hideout

            new() { Species = 241, Level = 48, Gauge = 07000, Moves = new[] {208,111,205,034}, Location = 118 }, // Miltank: Bodybuilder Jomas @ Tower Colosseum
            new() { Species = 359, Level = 48, Gauge = 07000, Moves = new[] {195,014,163,185}, Location = 118 }, // Absol: Rider Delan @ Tower Colosseum
            new() { Species = 229, Level = 48, Gauge = 07000, Moves = new[] {185,336,123,053}, Location = 118 }, // Houndoom: Cipher Peon Nella @ Tower Colosseum
            new() { Species = 357, Level = 49, Gauge = 07000, Moves = new[] {076,235,345,019}, Location = 118 }, // Tropius: Cipher Peon Ston @ Tower Colosseum
            new() { Species = 376, Level = 50, Gauge = 15000, Moves = new[] {063,334,232,094}, Location = 118 }, // Metagross: Cipher Nascour @ Tower Colosseum
            new() { Species = 248, Level = 55, Gauge = 20000, Moves = new[] {242,087,157,059}, Location = 118 }, // Tyranitar: Cipher Head Evice @ Tower Colosseum
            new() { Species = 235, Level = 45, Gauge = 07000, Moves = new[] {166,039,003,231}, Location = 132 }, // Smeargle: Team Snagem Biden @ Snagem Hideout
            new() { Species = 213, Level = 45, Gauge = 07000, Moves = new[] {219,227,156,117}, Location = 125 }, // Shuckle: Deep King Agnol @ Deep Colosseum
            new() { Species = 176, Level = 20, Gauge = 05000, Moves = new[] {118,204,186,281}, Location = 001 }, // Togetic: Cipher Peon Fein @ Outskirt Stand

            new(Gligar)    { Species = 207, Level = 43, Gauge = 06000, Moves = new[] {185,028,040,163}, Location = 058 }, // Gligar: Hunter Frena @ The Under Subway
            new(Gligar)    { Species = 207, Level = 43, Gauge = 06000, Moves = new[] {185,028,040,163}, Location = 133 }, // Gligar: Hunter Frena @ Snagem Hideout
            new(Murkrow)   { Species = 198, Level = 43, Gauge = 06000, Moves = new[] {185,212,101,019}, Location = 067 }, // Murkrow: Cipher Peon Lare @ Shadow PKMN Lab
            new(Heracross) { Species = 214, Level = 45, Gauge = 07000, Moves = new[] {179,203,068,280}, Location = 111 }, // Heracross: Cipher Peon Dioge @ Realgam Tower
            new(Heracross) { Species = 214, Level = 45, Gauge = 07000, Moves = new[] {179,203,068,280}, Location = 132 }, // Heracross: Cipher Peon Dioge @ Snagem Hideout
            new(Ursaring)  { Species = 217, Level = 45, Gauge = 07000, Moves = new[] {185,313,122,163}, Location = 132 }, // Ursaring: Team Snagem Agrev @ Snagem Hideout
            new(CTogepi)   { Species = 175, Level = 20, Gauge = 00000, Moves = new[] {118,204,186,281}, IVs = EncounterStaticShadow.EReaderEmpty }, // Togepi: Chaser ボデス @ Card e Room (Japanese games only)
            new(CMareep)   { Species = 179, Level = 37, Gauge = 00000, Moves = new[] {087,084,086,178}, IVs = EncounterStaticShadow.EReaderEmpty }, // Mareep: Hunter ホル @ Card e Room (Japanese games only)
            new(CScizor)   { Species = 212, Level = 50, Gauge = 00000, Moves = new[] {210,232,014,163}, IVs = EncounterStaticShadow.EReaderEmpty }, // Scizor: Bodybuilder ワーバン @ Card e Room (Japanese games only)
        };
        #endregion

        #region XD

        private static readonly EncounterStatic3[] Encounter_XDGift =
        {
            new(133, 10, XD) { Fateful = true, Gift = true, Location = 000, Moves = new[] {044} }, // Eevee (Bite)
            new(152, 05, XD) { Fateful = true, Gift = true, Location = 016, Moves = new[] {246,033,045,338} }, // Chikoritascverifier.fate
            new(155, 05, XD) { Fateful = true, Gift = true, Location = 016, Moves = new[] {179,033,043,307} }, // Cyndaquil
            new(158, 05, XD) { Fateful = true, Gift = true, Location = 016, Moves = new[] {242,010,043,308} }, // Totodile
        };

        private static readonly EncounterStaticShadow[] Encounter_XD =
        {
            new() { Fateful = true, Species = 216, Level = 11, Gauge = 03000, Moves = new[] {216,287,122,232}, Location = 143, Gift = true }, // Teddiursa: Cipher Peon Naps @ Pokémon HQ Lab -- treat as Gift as it can only be captured in a Poké Ball
            new() { Fateful = true, Species = 228, Level = 17, Gauge = 01500, Moves = new[] {185,204,052,046}, Location = 011, }, // Houndour: Cipher Peon Resix @ Cipher Lab
            new() { Fateful = true, Species = 343, Level = 17, Gauge = 01500, Moves = new[] {317,287,189,060}, Location = 011, }, // Baltoy: Cipher Peon Browsix @ Cipher Lab
            new() { Fateful = true, Species = 179, Level = 17, Gauge = 01500, Moves = new[] {034,215,084,086}, Location = 011, }, // Mareep: Cipher Peon Yellosix @ Cipher Lab
            new() { Fateful = true, Species = 318, Level = 15, Gauge = 01700, Moves = new[] {352,287,184,044}, Location = 008, }, // Carvanha: Cipher Peon Cabol @ Cipher Lab
            new() { Fateful = true, Species = 175, Level = 25, Gauge = 04500, Moves = new[] {266,161,246,270}, Location = 164, Gift = true }, // Togepi: Pokémon Trainer Hordel @ Outskirt Stand

            // Phenac City Hexagon Brothers
            new() { Fateful = true, Species = 228, Level = 17, Gauge = 01500, Moves = new[] {185,204,052,046}, Location = 096, }, // Houndour: Cipher Peon Resix  @ Phenac City
            new() { Fateful = true, Species = 343, Level = 17, Gauge = 01500, Moves = new[] {317,287,189,060}, Location = 096, }, // Baltoy: Cipher Peon Browsix  @ Phenac City
            new() { Fateful = true, Species = 179, Level = 17, Gauge = 01500, Moves = new[] {034,215,084,086}, Location = 096, }, // Mareep: Cipher Peon Yellosix @ Phenac City
            new(Seedot)   { Fateful = true, Species = 273, Level = 17, Gauge = 01500, Moves = new[] {202,287,331,290}, Location = 096 }, // Seedot: Cipher Peon Greesix @ Phenac City
            new(Spheal)   { Fateful = true, Species = 363, Level = 17, Gauge = 01500, Moves = new[] {062,204,055,189}, Location = 096 }, // Spheal: Cipher Peon Blusix  @ Phenac City
            new(Gulpin)   { Fateful = true, Species = 316, Level = 17, Gauge = 01500, Moves = new[] {351,047,124,092}, Location = 096 }, // Gulpin: Cipher Peon Purpsix @ Phenac City

            new() { Fateful = true, Species = 335, Level = 28, Gauge = 05000, Moves = new[] {280,287,068,306}, Location = 071 }, // Zangoose: Thug Zook @ Cipher Key Lair
            new() { Fateful = true, Species = 335, Level = 28, Gauge = 05000, Moves = new[] {280,287,068,306}, Location = 090 }, // Zangoose: Thug Zook @ Cipher Key Lair
            new() { Fateful = true, Species = 015, Level = 30, Gauge = 04500, Moves = new[] {188,226,041,014}, Location = 059 }, // Beedrill: Cipher Peon Lok @ Cipher Key Lair
            new() { Fateful = true, Species = 090, Level = 29, Gauge = 04000, Moves = new[] {036,287,057,062}, Location = 065 }, // Shellder: Cipher Peon Gorog @ Cipher Key Lair
            new() { Fateful = true, Species = 015, Level = 30, Gauge = 04500, Moves = new[] {188,226,041,014}, Location = 066 }, // Beedrill: Cipher Peon Lok @ Cipher Key Lair
            new() { Fateful = true, Species = 249, Level = 50, Gauge = 12000, Moves = new[] {354,297,089,056}, Location = 074 }, // Lugia: Grand Master Greevil @ Citadark Isle
            new() { Fateful = true, Species = 112, Level = 46, Gauge = 07000, Moves = new[] {224,270,184,089}, Location = 074 }, // Rhydon: Grand Master Greevil @ Citadark Isle
            new() { Fateful = true, Species = 277, Level = 43, Gauge = 07000, Moves = new[] {143,226,097,263}, Location = 087 }, // Swellow: Cipher Admin Ardos @ Citadark Isle

            new(Ledyba)   { Fateful = true, Species = 165, Level = 10, Gauge = 02500, Moves = new[] {060,287,332,048}, Location = 153 }, // Ledyba: Casual Guy Cyle @ Gateon Port
            new(Poochyena){ Fateful = true, Species = 261, Level = 10, Gauge = 02500, Moves = new[] {091,215,305,336}, Location = 162 }, // Poochyena: Bodybuilder Kilen @ Gateon Port
            new(Seedot)   { Fateful = true, Species = 273, Level = 17, Gauge = 01500, Moves = new[] {202,287,331,290}, Location = 011 }, // Seedot: Cipher Peon Greesix @ Cipher Lab
            new(Spheal)   { Fateful = true, Species = 363, Level = 17, Gauge = 01500, Moves = new[] {062,204,055,189}, Location = 011 }, // Spheal: Cipher Peon Blusix @ Cipher Lab
            new(Gulpin)   { Fateful = true, Species = 316, Level = 17, Gauge = 01500, Moves = new[] {351,047,124,092}, Location = 011 }, // Gulpin: Cipher Peon Purpsix @ Cipher Lab
            new(Spinarak) { Fateful = true, Species = 167, Level = 14, Gauge = 01500, Moves = new[] {091,287,324,101}, Location = 010 }, // Spinarak: Cipher Peon Nexir @ Cipher Lab
            new(Numel)    { Fateful = true, Species = 322, Level = 14, Gauge = 01500, Moves = new[] {036,204,091,052}, Location = 009 }, // Numel: Cipher Peon Solox @ Cipher Lab
            new(Shroomish){ Fateful = true, Species = 285, Level = 15, Gauge = 01800, Moves = new[] {206,287,072,078}, Location = 008 }, // Shroomish: Cipher R&D Klots @ Cipher Lab
            new(Delcatty) { Fateful = true, Species = 301, Level = 18, Gauge = 02500, Moves = new[] {290,186,213,351}, Location = 008 }, // Delcatty: Cipher Admin Lovrina @ Cipher Lab
            new(Voltorb)  { Fateful = true, Species = 100, Level = 19, Gauge = 02500, Moves = new[] {243,287,209,129}, Location = 092 }, // Voltorb: Wanderer Miror B. @ Cave Poké Spot
            new(Makuhita) { Fateful = true, Species = 296, Level = 18, Gauge = 02000, Moves = new[] {280,287,292,317}, Location = 109 }, // Makuhita: Cipher Peon Torkin @ ONBS Building
            new(Vulpix)   { Fateful = true, Species = 037, Level = 18, Gauge = 02000, Moves = new[] {257,204,052,091}, Location = 109 }, // Vulpix: Cipher Peon Mesin @ ONBS Building
            new(Duskull)  { Fateful = true, Species = 355, Level = 19, Gauge = 02200, Moves = new[] {247,270,310,109}, Location = 110 }, // Duskull: Cipher Peon Lobar @ ONBS Building
            new(Ralts)    { Fateful = true, Species = 280, Level = 20, Gauge = 02200, Moves = new[] {351,047,115,093}, Location = 119 }, // Ralts: Cipher Peon Feldas @ ONBS Building
            new(Mawile)   { Fateful = true, Species = 303, Level = 22, Gauge = 02500, Moves = new[] {206,047,011,334}, Location = 111 }, // Mawile: Cipher Cmdr Exol @ ONBS Building
            new(Snorunt)  { Fateful = true, Species = 361, Level = 20, Gauge = 02500, Moves = new[] {352,047,044,196}, Location = 097 }, // Snorunt: Cipher Peon Exinn @ Phenac City
            new(Pineco)   { Fateful = true, Species = 204, Level = 20, Gauge = 02500, Moves = new[] {042,287,191,068}, Location = 096 }, // Pineco: Cipher Peon Gonrap @ Phenac City
            new(Natu)     { Fateful = true, Species = 177, Level = 22, Gauge = 02500, Moves = new[] {248,226,101,332}, Location = 094 }, // Natu: Cipher Peon Eloin @ Phenac City

            new(Roselia)  { Fateful = true, Species = 315, Level = 22, Gauge = 03000, Moves = new[] {345,186,320,073}, Location = 113, }, // Roselia: Cipher Peon Fasin @ Phenac City
            new(Roselia)  { Fateful = true, Species = 315, Level = 22, Gauge = 03000, Moves = new[] {345,186,320,073}, Location = 094, }, // Roselia: Cipher Peon Fasin @ Phenac City
            new(Meowth)   { Fateful = true, Species = 052, Level = 22, Gauge = 03500, Moves = new[] {163,047,006,044}, Location = 113, }, // Meowth: Cipher Peon Fostin @ Phenac City
            new(Meowth)   { Fateful = true, Species = 052, Level = 22, Gauge = 03500, Moves = new[] {163,047,006,044}, Location = 094, }, // Meowth: Cipher Peon Fostin @ Phenac City

            new(Swinub)   { Fateful = true, Species = 220, Level = 22, Gauge = 02500, Moves = new[] {246,204,054,341}, Location = 100, }, // Swinub: Cipher Peon Greck @ Phenac City

            new(Spearow)  { Fateful = true, Species = 021, Level = 22, Gauge = 04500, Moves = new[] {206,226,043,332}, Location = 059, }, // Spearow: Cipher Peon Ezin @ Phenac Stadium
            new(Spearow)  { Fateful = true, Species = 021, Level = 22, Gauge = 04500, Moves = new[] {206,226,043,332}, Location = 107, }, // Spearow: Cipher Peon Ezin @ Phenac Stadium
            new(Grimer)   { Fateful = true, Species = 088, Level = 23, Gauge = 03000, Moves = new[] {188,270,325,107}, Location = 059, }, // Grimer: Cipher Peon Faltly @ Phenac Stadium
            new(Grimer)   { Fateful = true, Species = 088, Level = 23, Gauge = 03000, Moves = new[] {188,270,325,107}, Location = 107, }, // Grimer: Cipher Peon Faltly @ Phenac Stadium

            new(Seel)     { Fateful = true, Species = 086, Level = 23, Gauge = 03500, Moves = new[] {057,270,219,058}, Location = 107, }, // Seel: Cipher Peon Egrog @ Phenac Stadium
            new(Lunatone) { Fateful = true, Species = 337, Level = 25, Gauge = 05000, Moves = new[] {094,226,240,317}, Location = 107, }, // Lunatone: Cipher Admin Snattle @ Phenac Stadium

            new(Nosepass) { Fateful = true, Species = 299, Level = 26, Gauge = 04000, Moves = new[] {085,270,086,157}, Location = 090, }, // Nosepass: Wanderer Miror B. @ Pyrite Colosseum/Realgam Colosseum/Poké Spots
            new(Nosepass) { Fateful = true, Species = 299, Level = 26, Gauge = 04000, Moves = new[] {085,270,086,157}, Location = 113, }, // Nosepass: Wanderer Miror B. @ Pyrite Colosseum/Realgam Colosseum/Poké Spots

            new(Paras)    { Fateful = true, Species = 046, Level = 28, Gauge = 04000, Moves = new[] {147,287,163,206}, Location = 064, }, // Paras: Cipher Peon Humah @ Cipher Key Lair

            new(Growlithe) { Fateful = true, Species = 058, Level = 28, Gauge = 04000, Moves = new[] {053,204,044,036}, Location = 064 }, // Growlithe: Cipher Peon Humah @ Cipher Key Lair
            new(Growlithe) { Fateful = true, Species = 058, Level = 28, Gauge = 04000, Moves = new[] {053,204,044,036}, Location = 113 }, // Growlithe: Cipher Peon Humah @ Cipher Key Lair
            new(Butterfree){ Fateful = true, Species = 012, Level = 30, Gauge = 04000, Moves = new[] {094,234,079,332}, Location = 059, }, // Butterfree: Cipher Peon Targ @ Cipher Key Lair
            new(Venomoth)  { Fateful = true, Species = 049, Level = 32, Gauge = 04000, Moves = new[] {318,287,164,094}, Location = 059, }, // Venomoth: Cipher Peon Angic @ Cipher Key Lair
            new(Hypno)     { Fateful = true, Species = 097, Level = 34, Gauge = 05500, Moves = new[] {094,226,096,247}, Location = 059, }, // Hypno: Cipher Admin Gorigan @ Cipher Key Lair
            new(Banette)   { Fateful = true, Species = 354, Level = 37, Gauge = 07000, Moves = new[] {185,270,247,174}, Location = 059, }, // Banette: Cipher Peon Litnar @ Citadark Isle

            new(Pidgeotto) { Fateful = true, Species = 017, Level = 30, Gauge = 04000, Moves = new[] {017,287,211,297}, Location = 066, }, // Pidgeotto: Cipher Peon Lok @ Cipher Key Lair
            new(Tangela)   { Fateful = true, Species = 114, Level = 30, Gauge = 04000, Moves = new[] {076,234,241,275}, Location = 067, }, // Tangela: Cipher Peon Targ @ Cipher Key Lair
            new(Butterfree){ Fateful = true, Species = 012, Level = 30, Gauge = 04000, Moves = new[] {094,234,079,332}, Location = 067, }, // Butterfree: Cipher Peon Targ @ Cipher Key Lair
            new(Magneton)  { Fateful = true, Species = 082, Level = 30, Gauge = 04500, Moves = new[] {038,287,240,087}, Location = 067, }, // Magneton: Cipher Peon Snidle @ Cipher Key Lair
            new(Venomoth)  { Fateful = true, Species = 049, Level = 32, Gauge = 04000, Moves = new[] {318,287,164,094}, Location = 070, }, // Venomoth: Cipher Peon Angic @ Cipher Key Lair
            new(Weepinbell){ Fateful = true, Species = 070, Level = 32, Gauge = 04000, Moves = new[] {345,234,188,230}, Location = 070, }, // Weepinbell: Cipher Peon Angic @ Cipher Key Lair
            new(Arbok)     { Fateful = true, Species = 024, Level = 33, Gauge = 05000, Moves = new[] {188,287,137,044}, Location = 070, }, // Arbok: Cipher Peon Smarton @ Cipher Key Lair
            new(Primeape)  { Fateful = true, Species = 057, Level = 34, Gauge = 06000, Moves = new[] {238,270,116,179}, Location = 069, }, // Primeape: Cipher Admin Gorigan @ Cipher Key Lair
            new(Hypno)     { Fateful = true, Species = 097, Level = 34, Gauge = 05500, Moves = new[] {094,226,096,247}, Location = 069, }, // Hypno: Cipher Admin Gorigan @ Cipher Key Lair
            new(Golduck)   { Fateful = true, Species = 055, Level = 33, Gauge = 06500, Moves = new[] {127,204,244,280}, Location = 088, }, // Golduck: Navigator Abson @ Citadark Isle
            new(Sableye)   { Fateful = true, Species = 302, Level = 33, Gauge = 07000, Moves = new[] {247,270,185,105}, Location = 088, }, // Sableye: Navigator Abson @ Citadark Isle
            new(Dodrio)    { Fateful = true, Species = 085, Level = 34, Gauge = 08000, Moves = new[] {065,226,097,161}, Location = 076, }, // Dodrio: Chaser Furgy @ Citadark Isle
            new(Raticate)  { Fateful = true, Species = 020, Level = 34, Gauge = 06000, Moves = new[] {162,287,184,158}, Location = 076, }, // Raticate: Chaser Furgy @ Citadark Isle
            new(Farfetchd) { Fateful = true, Species = 083, Level = 36, Gauge = 05500, Moves = new[] {163,226,014,332}, Location = 076, }, // Farfetch'd: Cipher Admin Lovrina @ Citadark Isle
            new(Altaria)   { Fateful = true, Species = 334, Level = 36, Gauge = 06500, Moves = new[] {225,215,076,332}, Location = 076, }, // Altaria: Cipher Admin Lovrina @ Citadark Isle
            new(Kangaskhan){ Fateful = true, Species = 115, Level = 35, Gauge = 06000, Moves = new[] {089,047,039,146}, Location = 085, }, // Kangaskhan: Cipher Peon Litnar @ Citadark Isle
            new(Banette)   { Fateful = true, Species = 354, Level = 37, Gauge = 07000, Moves = new[] {185,270,247,174}, Location = 085, }, // Banette: Cipher Peon Litnar @ Citadark Isle
            new(Magmar)    { Fateful = true, Species = 126, Level = 36, Gauge = 07000, Moves = new[] {126,266,238,009}, Location = 077, }, // Magmar: Cipher Peon Grupel @ Citadark Isle
            new(Pinsir)    { Fateful = true, Species = 127, Level = 35, Gauge = 07000, Moves = new[] {012,270,206,066}, Location = 077, }, // Pinsir: Cipher Peon Grupel @ Citadark Isle
            new(Rapidash)  { Fateful = true, Species = 078, Level = 40, Gauge = 06000, Moves = new[] {076,226,241,053}, Location = 080, }, // Rapidash: Cipher Peon Kolest @ Citadark Isle
            new(Magcargo)  { Fateful = true, Species = 219, Level = 38, Gauge = 05500, Moves = new[] {257,287,089,053}, Location = 080, }, // Magcargo: Cipher Peon Kolest @ Citadark Isle
            new(Hitmonchan){ Fateful = true, Species = 107, Level = 38, Gauge = 06000, Moves = new[] {005,270,170,327}, Location = 081, }, // Hitmonchan: Cipher Peon Karbon @ Citadark Isle
            new(Hitmonlee) { Fateful = true, Species = 106, Level = 38, Gauge = 07000, Moves = new[] {136,287,170,025}, Location = 081, }, // Hitmonlee: Cipher Peon Petro @ Citadark Isle
            new(Lickitung) { Fateful = true, Species = 108, Level = 38, Gauge = 05000, Moves = new[] {038,270,111,205}, Location = 084, }, // Lickitung: Cipher Peon Geftal @ Citadark Isle
            new(Scyther)   { Fateful = true, Species = 123, Level = 40, Gauge = 08000, Moves = new[] {013,234,318,163}, Location = 084, }, // Scyther: Cipher Peon Leden @ Citadark Isle

            new(Chansey)   { Fateful = true, Species = 113, Level = 39, Gauge = 04000, Moves = new[] {085,186,135,285}, Location = 084, }, // Chansey: Cipher Peon Leden @ Citadark Isle
            new(Chansey)   { Fateful = true, Species = 113, Level = 39, Gauge = 04000, Moves = new[] {085,186,135,285}, Location = 087, }, // Chansey: Cipher Peon Leden @ Citadark Isle

            new(Solrock)   { Fateful = true, Species = 338, Level = 41, Gauge = 07500, Moves = new[] {094,226,241,322}, Location = 087, }, // Solrock: Cipher Admin Snattle @ Citadark Isle
            new(Starmie)   { Fateful = true, Species = 121, Level = 41, Gauge = 07500, Moves = new[] {127,287,058,105}, Location = 087, }, // Starmie: Cipher Admin Snattle @ Citadark Isle
            new(Electabuzz){ Fateful = true, Species = 125, Level = 43, Gauge = 07000, Moves = new[] {238,266,086,085}, Location = 087, }, // Electabuzz: Cipher Admin Ardos @ Citadark Isle
            new(Snorlax)   { Fateful = true, Species = 143, Level = 43, Gauge = 09000, Moves = new[] {090,287,174,034}, Location = 087, }, // Snorlax: Cipher Admin Ardos @ Citadark Isle
            new(Poliwrath) { Fateful = true, Species = 062, Level = 42, Gauge = 07500, Moves = new[] {056,270,240,280}, Location = 087, }, // Poliwrath: Cipher Admin Gorigan @ Citadark Isle
            new(MrMime)    { Fateful = true, Species = 122, Level = 42, Gauge = 06500, Moves = new[] {094,266,227,009}, Location = 087, }, // Mr. Mime: Cipher Admin Gorigan @ Citadark Isle
            new(Dugtrio)   { Fateful = true, Species = 051, Level = 40, Gauge = 05000, Moves = new[] {089,204,201,161}, Location = 075, }, // Dugtrio: Cipher Peon Kolax @ Citadark Isle
            new(Manectric) { Fateful = true, Species = 310, Level = 44, Gauge = 07000, Moves = new[] {087,287,240,044}, Location = 073, }, // Manectric: Cipher Admin Eldes @ Citadark Isle
            new(Salamence) { Fateful = true, Species = 373, Level = 50, Gauge = 09000, Moves = new[] {337,287,349,332}, Location = 073, }, // Salamence: Cipher Admin Eldes @ Citadark Isle
            new(Marowak)   { Fateful = true, Species = 105, Level = 44, Gauge = 06500, Moves = new[] {089,047,014,157}, Location = 073, }, // Marowak: Cipher Admin Eldes @ Citadark Isle
            new(Lapras)    { Fateful = true, Species = 131, Level = 44, Gauge = 06000, Moves = new[] {056,215,240,059}, Location = 073, }, // Lapras: Cipher Admin Eldes @ Citadark Isle
            new(Moltres)   { Fateful = true, Species = 146, Level = 50, Gauge = 10000, Moves = new[] {326,234,261,053}, Location = 074, }, // Moltres: Grand Master Greevil @ Citadark Isle
            new(Exeggutor) { Fateful = true, Species = 103, Level = 46, Gauge = 09000, Moves = new[] {094,287,095,246}, Location = 074, }, // Exeggutor: Grand Master Greevil @ Citadark Isle
            new(Tauros)    { Fateful = true, Species = 128, Level = 46, Gauge = 09000, Moves = new[] {089,287,039,034}, Location = 074, }, // Tauros: Grand Master Greevil @ Citadark Isle
            new(Articuno)  { Fateful = true, Species = 144, Level = 50, Gauge = 10000, Moves = new[] {326,215,114,058}, Location = 074, }, // Articuno: Grand Master Greevil @ Citadark Isle
            new(Zapdos)    { Fateful = true, Species = 145, Level = 50, Gauge = 10000, Moves = new[] {326,226,319,085}, Location = 074, }, // Zapdos: Grand Master Greevil @ Citadark Isle
            new(Dragonite) { Fateful = true, Species = 149, Level = 55, Gauge = 09000, Moves = new[] {063,215,349,089}, Location = 162, }, // Dragonite: Wanderer Miror B. @ Gateon Port
        };

        internal static readonly EncounterArea3XD[] SlotsXD =
        {
            new(90, 027, 23, 207, 20, 328, 20), // Rock (Sandshrew, Gligar, Trapinch)
            new(91, 187, 20, 231, 20, 283, 20), // Oasis (Hoppip, Phanpy, Surskit)
            new(92, 041, 21, 304, 21, 194, 21), // Cave (Zubat, Aron, Wooper)
        };

        internal static readonly EncounterStatic[] Encounter_CXD = ArrayUtil.ConcatAll<EncounterStatic>(Encounter_ColoGift, Encounter_Colo, Encounter_XDGift, Encounter_XD);

        #endregion
    }
}
