using static PKHeX.Core.EncounterUtil;
using static PKHeX.Core.Shiny;
using static PKHeX.Core.GameVersion;

namespace PKHeX.Core
{
    internal static class Encounters8b
    {
        private static readonly EncounterArea8b[] SlotsBD_OW = EncounterArea8b.GetAreas(Get("encounter_bd", "bs"), BD);
        private static readonly EncounterArea8b[] SlotsSP_OW = EncounterArea8b.GetAreas(Get("encounter_sp", "bs"), SP);
        private static readonly EncounterArea8b[] SlotsBD_UG = EncounterArea8b.GetAreas(Get("underground_bd", "bs"), BD);
        private static readonly EncounterArea8b[] SlotsSP_UG = EncounterArea8b.GetAreas(Get("underground_sp", "bs"), SP);

        internal static readonly EncounterArea8b[] SlotsBD = ArrayUtil.ConcatAll(SlotsBD_OW, SlotsBD_UG);
        internal static readonly EncounterArea8b[] SlotsSP = ArrayUtil.ConcatAll(SlotsSP_OW, SlotsSP_UG);

        private static byte[][] Get(string resource, string ident) => BinLinker.Unpack(Util.GetBinaryResource($"{resource}.pkl"), ident);

        static Encounters8b() => MarkEncounterTradeStrings(TradeGift_BDSP, TradeBDSP);

        private static readonly EncounterStatic8b[] Encounter_BDSP =
        {
            // Gifts
            new (BDSP) { Gift = true, Species = 387, Level = 05, Location = 323 }, // Turtwig
            new (BDSP) { Gift = true, Species = 390, Level = 05, Location = 323 }, // Chimchar
            new (BDSP) { Gift = true, Species = 393, Level = 05, Location = 323 }, // Piplup
            new (BDSP) { Gift = true, Species = 133, Level = 05, Location = 104 }, // Eevee
            new (BDSP) { Gift = true, Species = 440, Level = 01, EggLocation = 60007, EggCycles = 40 }, // Happiny Egg from Traveling Man
            new (BDSP) { Gift = true, Species = 447, Level = 01, EggLocation = 60005, EggCycles = 25 }, // Riolu Egg from Riley

            // Fossils
            new (BDSP) { Gift = true, Species = 138, Level = 01, Location = 049, FlawlessIVCount = 3 }, // Omanyte
            new (BDSP) { Gift = true, Species = 140, Level = 01, Location = 049, FlawlessIVCount = 3 }, // Kabuto
            new (BDSP) { Gift = true, Species = 142, Level = 01, Location = 049, FlawlessIVCount = 3 }, // Aerodactyl
            new (BDSP) { Gift = true, Species = 345, Level = 01, Location = 049, FlawlessIVCount = 3 }, // Lileep
            new (BDSP) { Gift = true, Species = 347, Level = 01, Location = 049, FlawlessIVCount = 3 }, // Anorith
            new (BDSP) { Gift = true, Species = 408, Level = 01, Location = 049, FlawlessIVCount = 3 }, // Cranidos
            new (BDSP) { Gift = true, Species = 410, Level = 01, Location = 049, FlawlessIVCount = 3 }, // Shieldon

            // Game-specific gifts
            new (BDSP) { Gift = true, Species = 151, Level = 01, Ability = 2, Location = 438, Shiny = Never, FlawlessIVCount = 3, Fateful = true }, // Mew
            new (BDSP) { Gift = true, Species = 385, Level = 05, Ability = 2, Location = 438, Shiny = Never, FlawlessIVCount = 3, Fateful = true }, // Jirachi

            // Stationary
            new (BDSP) { Species = 425, Level = 22, Location = 197 }, // Drifloon
            new (BDSP) { Species = 442, Level = 25, Location = 367 }, // Spiritomb
            new (BDSP) { Species = 479, Level = 15, Location = 311 }, // Rotom

            // Roamers
            new (BDSP) { Species = 481, Level = 50, FlawlessIVCount = 3, Roaming = true }, // Mesprit
            new (BDSP) { Species = 488, Level = 50, FlawlessIVCount = 3, Roaming = true }, // Cresselia

            // Legendary
            new (BDSP) { Species = 480, Level = 50, Location = 331, FlawlessIVCount = 3 }, // Uxie
            new (BDSP) { Species = 482, Level = 50, Location = 328, FlawlessIVCount = 3 }, // Azelf
            new (BD  ) { Species = 483, Level = 47, Location = 216, FlawlessIVCount = 3 }, // Dialga
            new (  SP) { Species = 484, Level = 47, Location = 217, FlawlessIVCount = 3 }, // Palkia
            new (BDSP) { Species = 485, Level = 70, Location = 262, FlawlessIVCount = 3 }, // Heatran
            new (BDSP) { Species = 486, Level = 70, Location = 291, FlawlessIVCount = 3 }, // Regigigas
            new (BDSP) { Species = 487, Level = 70, Location = 266, FlawlessIVCount = 3 }, // Giratina

            // Mythical
          //new (BDSP) { Species = 491, Level = 50, Location = 333, FlawlessIVCount = 3, Fateful = true }, // Darkrai
          //new (BDSP) { Species = 492, Level = 30, Location = 285, FlawlessIVCount = 3, Fateful = true }, // Shaymin
          //new (BDSP) { Species = 493, Level = 80, Location = 218, FlawlessIVCount = 3, Fateful = true }, // Arceus

            // Ramanas Park (Pure Space)
            new (  SP) { Species = 144, Level = 70, Ability = 4, Location = 506, FlawlessIVCount = 3 }, // Articuno
            new (  SP) { Species = 145, Level = 70, Ability = 4, Location = 506, FlawlessIVCount = 3 }, // Zapdos
            new (  SP) { Species = 146, Level = 70, Ability = 4, Location = 506, FlawlessIVCount = 3 }, // Moltres
            new (BD  ) { Species = 243, Level = 70, Ability = 4, Location = 506, FlawlessIVCount = 3 }, // Raikou
            new (BD  ) { Species = 244, Level = 70, Ability = 4, Location = 506, FlawlessIVCount = 3 }, // Entei
            new (BD  ) { Species = 245, Level = 70, Ability = 4, Location = 506, FlawlessIVCount = 3 }, // Suicune
            new (BDSP) { Species = 377, Level = 70, Ability = 4, Location = 506, FlawlessIVCount = 3 }, // Regirock
            new (BDSP) { Species = 378, Level = 70, Ability = 4, Location = 506, FlawlessIVCount = 3 }, // Regice
            new (BDSP) { Species = 379, Level = 70, Ability = 4, Location = 506, FlawlessIVCount = 3 }, // Registeel
            new (BDSP) { Species = 380, Level = 70,              Location = 506, FlawlessIVCount = 3 }, // Latias
            new (BDSP) { Species = 381, Level = 70,              Location = 506, FlawlessIVCount = 3 }, // Latios

            // Ramanas Park (Deep Space)
            new (BDSP) { Species = 150, Level = 70, Ability = 4, Location = 507, FlawlessIVCount = 3 }, // Mewtwo
            new (  SP) { Species = 249, Level = 70, Ability = 4, Location = 507, FlawlessIVCount = 3 }, // Lugia
            new (BD  ) { Species = 250, Level = 70, Ability = 4, Location = 507, FlawlessIVCount = 3 }, // Ho-Oh
            new (BDSP) { Species = 382, Level = 70,              Location = 507, FlawlessIVCount = 3 }, // Kyogre
            new (BDSP) { Species = 383, Level = 70,              Location = 507, FlawlessIVCount = 3 }, // Groudon
            new (BDSP) { Species = 384, Level = 70,              Location = 507, FlawlessIVCount = 3 }, // Rayquaza
        };

        internal static readonly EncounterStatic8b[] StaticBD = GetEncounters(Encounter_BDSP, BD);
        internal static readonly EncounterStatic8b[] StaticSP = GetEncounters(Encounter_BDSP, SP);

        private const string tradeBDSP = "tradebdsp";
        private static readonly string[][] TradeBDSP = Util.GetLanguageStrings10(tradeBDSP, "zh2");

        internal static readonly EncounterTrade8b[] TradeGift_BDSP =
        {
            new (BDSP) { Species = 063, Level = 09, Ability = 1, Gender = 0, OTGender = 0, TID = 25643, IVs = new[] {28,10,09,31,11,03}, Moves = new[] {100,000,000,000}, HeightScalar = 029, WeightScalar = 202, Nature = Nature.Quiet  }, // Abra
            new (BDSP) { Species = 441, Level = 15, Ability = 2, Gender = 1, OTGender = 0, TID = 44142, IVs = new[] {17,08,29,25,17,23}, Moves = new[] {448,047,064,045}, HeightScalar = 088, WeightScalar = 091, Nature = Nature.Lonely }, // Chatot
            new (BDSP) { Species = 093, Level = 33, Ability = 1, Gender = 0, OTGender = 0, TID = 19248, IVs = new[] {18,24,28,02,22,30}, Moves = new[] {247,371,389,109}, HeightScalar = 096, WeightScalar = 208, Nature = Nature.Hasty  }, // Haunter
            new (BDSP) { Species = 129, Level = 45, Ability = 1, Gender = 1, OTGender = 0, TID = 53277, IVs = new[] {03,03,31,02,11,03}, Moves = new[] {150,000,000,000}, HeightScalar = 169, WeightScalar = 068, Nature = Nature.Mild   }, // Magikarp
        };
    }
}
