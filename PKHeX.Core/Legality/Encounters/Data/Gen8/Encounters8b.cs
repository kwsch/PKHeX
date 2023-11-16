using static PKHeX.Core.EncounterUtil;
using static PKHeX.Core.Shiny;
using static PKHeX.Core.GameVersion;
using static PKHeX.Core.AbilityPermission;

namespace PKHeX.Core;

internal static class Encounters8b
{
    private static readonly EncounterArea8b[] SlotsBD_OW = EncounterArea8b.GetAreas(Get("bd", "bs"u8), BD);
    private static readonly EncounterArea8b[] SlotsSP_OW = EncounterArea8b.GetAreas(Get("sp", "bs"u8), SP);
    private static readonly EncounterArea8b[] SlotsBD_UG = EncounterArea8b.GetAreas(Get("bd_underground", "bs"u8), BD);
    private static readonly EncounterArea8b[] SlotsSP_UG = EncounterArea8b.GetAreas(Get("sp_underground", "bs"u8), SP);

    internal static readonly EncounterArea8b[] SlotsBD = [..SlotsBD_OW, ..SlotsBD_UG];
    internal static readonly EncounterArea8b[] SlotsSP = [..SlotsSP_OW, ..SlotsSP_UG];

    internal static readonly EncounterStatic8b[] Encounter_BDSP =
    [
        // Gifts
        new(BDSP) { FixedBall = Ball.Poke, Species = 387, Level = 05, Location = 323 }, // Turtwig
        new(BDSP) { FixedBall = Ball.Poke, Species = 390, Level = 05, Location = 323 }, // Chimchar
        new(BDSP) { FixedBall = Ball.Poke, Species = 393, Level = 05, Location = 323 }, // Piplup
        new(BDSP) { FixedBall = Ball.Poke, Species = 133, Level = 05, Location = 104 }, // Eevee
        new(BDSP) { FixedBall = Ball.Poke, Species = 440, Level = 01, EggLocation = 60007, Location = Locations.Default8bNone }, // Happiny Egg from Traveling Man
        new(BDSP) { FixedBall = Ball.Poke, Species = 447, Level = 01, EggLocation = 60005, Location = Locations.Default8bNone }, // Riolu Egg from Riley

        // Fossils
        new(BDSP) { FixedBall = Ball.Poke, Species = 138, Level = 01, Location = 049, FlawlessIVCount = 3 }, // Omanyte
        new(BDSP) { FixedBall = Ball.Poke, Species = 140, Level = 01, Location = 049, FlawlessIVCount = 3 }, // Kabuto
        new(BDSP) { FixedBall = Ball.Poke, Species = 142, Level = 01, Location = 049, FlawlessIVCount = 3 }, // Aerodactyl
        new(BDSP) { FixedBall = Ball.Poke, Species = 345, Level = 01, Location = 049, FlawlessIVCount = 3 }, // Lileep
        new(BDSP) { FixedBall = Ball.Poke, Species = 347, Level = 01, Location = 049, FlawlessIVCount = 3 }, // Anorith
        new(BDSP) { FixedBall = Ball.Poke, Species = 408, Level = 01, Location = 049, FlawlessIVCount = 3 }, // Cranidos
        new(BDSP) { FixedBall = Ball.Poke, Species = 410, Level = 01, Location = 049, FlawlessIVCount = 3 }, // Shieldon

        // Game-specific gifts
        new(BDSP) { FixedBall = Ball.Poke, Species = 151, Level = 01, Ability = OnlySecond, Location = 438, Shiny = Never, FlawlessIVCount = 3, FatefulEncounter = true }, // Mew
        new(BDSP) { FixedBall = Ball.Poke, Species = 385, Level = 05, Ability = OnlySecond, Location = 438, Shiny = Never, FlawlessIVCount = 3, FatefulEncounter = true }, // Jirachi

        // Stationary
        new(BDSP) { Species = 425, Level = 22, Location = 197, FlawlessIVCount = 3 }, // Drifloon
        new(BDSP) { Species = 442, Level = 25, Location = 367                      }, // Spiritomb
        new(BDSP) { Species = 479, Level = 15, Location = 311                      }, // Rotom

        // Roamers
        new(BDSP) { Species = 481, Level = 50, Location = 197, FlawlessIVCount = 3, Roaming = true }, // Mesprit
        new(BDSP) { Species = 488, Level = 50, Location = 197, FlawlessIVCount = 3, Roaming = true }, // Cresselia

        // Legendary
        new(BDSP) { Species = 480, Level = 50, Location = 331, FlawlessIVCount = 3 }, // Uxie
        new(BDSP) { Species = 482, Level = 50, Location = 328, FlawlessIVCount = 3 }, // Azelf
        new(BDSP) { Species = 485, Level = 70, Location = 262, FlawlessIVCount = 3 }, // Heatran
        new(BDSP) { Species = 486, Level = 70, Location = 291, FlawlessIVCount = 3 }, // Regigigas
        new(BDSP) { Species = 487, Level = 70, Location = 266, FlawlessIVCount = 3 }, // Giratina

        // Mythical
        new(BDSP) { Species = 491, Level = 50, Location = 333, FlawlessIVCount = 3, FatefulEncounter = true }, // Darkrai
        new(BDSP) { Species = 492, Level = 30, Location = 285, FlawlessIVCount = 3, FatefulEncounter = true }, // Shaymin

        // Ramanas Park (Pure Space)
        new(BDSP) { Species = 377, Level = 70, Location = 506, FlawlessIVCount = 3, Ability = OnlyHidden }, // Regirock
        new(BDSP) { Species = 378, Level = 70, Location = 506, FlawlessIVCount = 3, Ability = OnlyHidden }, // Regice
        new(BDSP) { Species = 379, Level = 70, Location = 506, FlawlessIVCount = 3, Ability = OnlyHidden }, // Registeel
        new(BDSP) { Species = 380, Level = 70, Location = 506, FlawlessIVCount = 3                       }, // Latias
        new(BDSP) { Species = 381, Level = 70, Location = 506, FlawlessIVCount = 3                       }, // Latios

        // Ramanas Park (Strange Space)
        new(BDSP) { Species = 150, Level = 70, Location = 507, FlawlessIVCount = 3, Ability = OnlyHidden }, // Mewtwo
        new(BDSP) { Species = 382, Level = 70, Location = 507, FlawlessIVCount = 3                       }, // Kyogre
        new(BDSP) { Species = 383, Level = 70, Location = 507, FlawlessIVCount = 3                       }, // Groudon
        new(BDSP) { Species = 384, Level = 70, Location = 507, FlawlessIVCount = 3                       }, // Rayquaza
    ];

    internal static readonly EncounterStatic8b[] StaticBD =
    [
        new(BD  ) { Species = 483, Level = 47, Location = 216, FlawlessIVCount = 3 }, // Dialga
        new(BD  ) { Species = 493, Level = 80, Location = 218, FlawlessIVCount = 3, FatefulEncounter = true }, // Arceus (Brilliant Diamond)
        new(BD  ) { Species = 243, Level = 70, Location = 506, FlawlessIVCount = 3, Ability = OnlyHidden }, // Raikou
        new(BD  ) { Species = 244, Level = 70, Location = 506, FlawlessIVCount = 3, Ability = OnlyHidden }, // Entei
        new(BD  ) { Species = 245, Level = 70, Location = 506, FlawlessIVCount = 3, Ability = OnlyHidden }, // Suicune
        new(BD  ) { Species = 250, Level = 70, Location = 507, FlawlessIVCount = 3, Ability = OnlyHidden }, // Ho-Oh
    ];

    internal static readonly EncounterStatic8b[] StaticSP =
    [
        new(  SP) { Species = 484, Level = 47, Location = 217, FlawlessIVCount = 3 }, // Palkia
        new(  SP) { Species = 493, Level = 80, Location = 618, FlawlessIVCount = 3, FatefulEncounter = true }, // Arceus (Shining Pearl)
        new(  SP) { Species = 144, Level = 70, Location = 506, FlawlessIVCount = 3, Ability = OnlyHidden }, // Articuno
        new(  SP) { Species = 145, Level = 70, Location = 506, FlawlessIVCount = 3, Ability = OnlyHidden }, // Zapdos
        new(  SP) { Species = 146, Level = 70, Location = 506, FlawlessIVCount = 3, Ability = OnlyHidden }, // Moltres
        new(  SP) { Species = 249, Level = 70, Location = 507, FlawlessIVCount = 3, Ability = OnlyHidden }, // Lugia
    ];

    private const string tradeBDSP = "tradebdsp";
    private static readonly string[][] TradeNames = Util.GetLanguageStrings10(tradeBDSP, "zh2");

    internal static readonly EncounterTrade8b[] TradeGift_BDSP =
    [
        new(TradeNames, 00, BDSP) { Species = 063, EncryptionConstant = 0x0000008E, PID = 0xFF50A8F5, Level = 09, Ability = OnlyFirst,  Gender = 0, OTGender = 0, ID32 = 25643, IVs = new(28,10,09,31,11,03), Moves = new(100,000,000,000), HeightScalar = 029, WeightScalar = 202, Nature = Nature.Quiet  }, // Abra
        new(TradeNames, 01, BDSP) { Species = 441, EncryptionConstant = 0x00000867, PID = 0x17DAAB19, Level = 15, Ability = OnlySecond, Gender = 1, OTGender = 0, ID32 = 44142, IVs = new(17,08,29,25,17,23), Moves = new(448,047,064,045), HeightScalar = 088, WeightScalar = 091, Nature = Nature.Lonely }, // Chatot
        new(TradeNames, 02, BDSP) { Species = 093, EncryptionConstant = 0x00000088, PID = 0xF60AB5BB, Level = 33, Ability = OnlyFirst,  Gender = 0, OTGender = 0, ID32 = 19248, IVs = new(18,24,28,02,22,30), Moves = new(247,371,389,109), HeightScalar = 096, WeightScalar = 208, Nature = Nature.Hasty  }, // Haunter
        new(TradeNames, 03, BDSP) { Species = 129, EncryptionConstant = 0x0000045C, PID = 0xFCE82F88, Level = 45, Ability = OnlyFirst,  Gender = 1, OTGender = 0, ID32 = 53277, IVs = new(03,03,31,02,11,03), Moves = new(150,000,000,000), HeightScalar = 169, WeightScalar = 068, Nature = Nature.Mild   }, // Magikarp
    ];
}
