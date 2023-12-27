using static PKHeX.Core.EncounterUtil;
using static PKHeX.Core.GameVersion;
using static PKHeX.Core.AbilityPermission;

namespace PKHeX.Core;

/// <summary>
/// Generation 5 Encounters
/// </summary>
public static class Encounters5BW
{
    internal static readonly EncounterArea5[] SlotsB = EncounterArea5.GetAreas(Get("b", "51"u8), B);
    internal static readonly EncounterArea5[] SlotsW = EncounterArea5.GetAreas(Get("w", "51"u8), W);

    private const string tradeBW = "tradebw";
    private static readonly string[][] TradeNames = Util.GetLanguageStrings8(tradeBW);

    #region DreamWorld Encounter

    public static readonly EncounterStatic5Entree[] DreamWorld_BW = DreamWorldEntry.GetArray(BW,
    [
        // Pleasant Forest
        new(029, 10, 010, 389, 162), // Nidoran♀
        new(032, 10, 064, 068, 162), // Nidoran♂
        new(174, 10, 047, 313, 270), // Igglybuff
        new(187, 10, 235, 270, 331), // Hoppip
        new(270, 10, 071, 073, 352), // Lotad
        new(276, 10, 064, 119, 366), // Taillow
        new(309, 10, 086, 423, 324), // Electrike
        new(351, 10, 052, 466, 352), // Castform
        new(417, 10, 098, 343, 351), // Pachirisu

        // Windswept Sky
        new(012, 10, 093, 355, 314), // Butterfree
        new(163, 10, 193, 101, 278), // Hoothoot
        new(278, 10, 055, 239, 351), // Wingull
        new(333, 10, 064, 297, 355), // Swablu
        new(425, 10, 107, 095, 285), // Drifloon
        new(441, 10, 119, 417, 272), // Chatot

        // Sparkling Sea
        new(079, 10, 281, 335, 362), // Slowpoke
        new(098, 10, 011, 133, 290), // Krabby
        new(119, 33, 352, 214, 203), // Seaking
        new(120, 10, 055, 278, 196), // Staryu
        new(222, 10, 145, 109, 446), // Corsola
        new(422, 10, 189, 281, 290, Form: 0), // Shellos-West
        new(422, 10, 189, 281, 290, Form: 1), // Shellos-East

        // Spooky Manor
        new(202, 15, 243, 204, 227), // Wobbuffet
        new(238, 10, 186, 445, 285), // Smoochum
        new(303, 10, 313, 424, 008), // Mawile
        new(307, 10, 096, 409, 203), // Meditite
        new(436, 10, 095, 285, 356), // Bronzor
        new(052, 10, 010, 095, 290), // Meowth
        new(479, 10, 086, 351, 324), // Rotom
        new(280, 10, 093, 194, 270), // Ralts
        new(302, 10, 193, 389, 180), // Sableye
        new(442, 10, 180, 220, 196), // Spiritomb

        // Rugged Mountain
        new(056, 10, 067, 179, 009), // Mankey
        new(111, 10, 030, 068, 038), // Rhyhorn
        new(231, 10, 175, 484, 402), // Phanpy
        new(451, 10, 044, 097, 401), // Skorupi
        new(216, 10, 313, 242, 264), // Teddiursa
        new(296, 10, 292, 270, 008), // Makuhita
        new(327, 10, 383, 252, 276), // Spinda
        new(374, 10, 036, 428, 442), // Beldum
        new(447, 10, 203, 418, 264), // Riolu

        // Icy Cave
        new(173, 10, 227, 312, 214), // Cleffa
        new(213, 10, 227, 270, 504), // Shuckle
        new(299, 10, 033, 446, 246), // Nosepass
        new(363, 10, 181, 090, 401), // Spheal
        new(408, 10, 029, 442, 007), // Cranidos
        new(206, 10, 111, 277, 446), // Dunsparce
        new(410, 10, 182, 068, 090), // Shieldon

        // Dream Park
        new(048, 10, 050, 226, 285), // Venonat
        new(088, 10, 139, 114, 425), // Grimer
        new(415, 10, 016, 366, 314), // Combee
        new(015, 10, 031, 314, 210), // Beedrill
        new(335, 10, 098, 458, 067), // Zangoose
        new(336, 10, 044, 034, 401), // Seviper

        // PGL
        new(134, 10, Gender: 0), // Vaporeon
        new(135, 10, Gender: 0), // Jolteon
        new(136, 10, Gender: 0), // Flareon
        new(196, 10, Gender: 0), // Espeon
        new(197, 10, Gender: 0), // Umbreon
        new(470, 10, Gender: 0), // Leafeon
        new(471, 10, Gender: 0), // Glaceon
        new(001, 10, Gender: 0), // Bulbasaur
        new(004, 10, Gender: 0), // Charmander
        new(007, 10, Gender: 0), // Squirtle
        new(453, 10, Gender: 0), // Croagunk
        new(387, 10, Gender: 0), // Turtwig
        new(390, 10, Gender: 0), // Chimchar
        new(393, 10, Gender: 0), // Piplup
        new(493, 100), // Arceus
        new(252, 10, Gender: 0), // Treecko
        new(255, 10, Gender: 0), // Torchic
        new(258, 10, Gender: 0), // Mudkip
        new(468, 10, 217, Gender: 0), // Togekiss
        new(473, 34, Gender: 0), // Mamoswine
        new(137, 10), // Porygon
        new(384, 50), // Rayquaza
        new(354, 37, 538, Gender: 1), // Banette
        new(453, 10, 398, Gender: 0), // Croagunk
        new(334, 35, 206, Gender: 0),  // Altaria
        new(242, 10), // Blissey
        new(448, 10, 418, Gender: 0), // Lucario
        new(189, 27, 206, Gender: 0), // Jumpluff
    ]);

    #endregion
    #region Static Encounter/Gift Tables
    public static readonly EncounterStatic5[] Encounter_BW =
    [
        // Starters @ Nuvema Town
        new(BW) { FixedBall = Ball.Poke, Species = 495, Level = 05, Location = 004 }, // Snivy
        new(BW) { FixedBall = Ball.Poke, Species = 498, Level = 05, Location = 004 }, // Tepig
        new(BW) { FixedBall = Ball.Poke, Species = 501, Level = 05, Location = 004 }, // Oshawott

        // Fossils @ Nacrene City
        new(BW) { FixedBall = Ball.Poke, Species = 138, Level = 25, Location = 007 }, // Omanyte
        new(BW) { FixedBall = Ball.Poke, Species = 140, Level = 25, Location = 007 }, // Kabuto
        new(BW) { FixedBall = Ball.Poke, Species = 142, Level = 25, Location = 007 }, // Aerodactyl
        new(BW) { FixedBall = Ball.Poke, Species = 345, Level = 25, Location = 007 }, // Lileep
        new(BW) { FixedBall = Ball.Poke, Species = 347, Level = 25, Location = 007 }, // Anorith
        new(BW) { FixedBall = Ball.Poke, Species = 408, Level = 25, Location = 007 }, // Cranidos
        new(BW) { FixedBall = Ball.Poke, Species = 410, Level = 25, Location = 007 }, // Shieldon
        new(BW) { FixedBall = Ball.Poke, Species = 564, Level = 25, Location = 007 }, // Tirtouga
        new(BW) { FixedBall = Ball.Poke, Species = 566, Level = 25, Location = 007 }, // Archen

        // Gift
        new(BW) { FixedBall = Ball.Poke, Species = 511, Level = 10, Location = 032 }, // Pansage @ Dreamyard
        new(BW) { FixedBall = Ball.Poke, Species = 513, Level = 10, Location = 032 }, // Pansear
        new(BW) { FixedBall = Ball.Poke, Species = 515, Level = 10, Location = 032 }, // Panpour
        new(BW) { FixedBall = Ball.Poke, Species = 129, Level = 05, Location = 068 }, // Magikarp @ Marvelous Bridge
        new(BW) { FixedBall = Ball.Poke, Species = 636, Level = 01, EggLocation = 60003, Location = 0 }, // Larvesta Egg from Treasure Hunter

        // Stationary
        new(BW) { Species = 518, Level = 50, Location = 032, Ability = OnlyHidden }, // Musharna @ Dreamyard Friday Only
        new(BW) { Species = 590, Level = 20, Location = 019 }, // Foongus @ Route 6
        new(BW) { Species = 590, Level = 30, Location = 023 }, // Foongus @ Route 10
        new(BW) { Species = 591, Level = 40, Location = 023 }, // Amoonguss @ Route 10
        new(BW) { Species = 555, Level = 35, Location = 034, Ability = OnlyHidden }, // HA Darmanitan @ Desert Resort
        new(BW) { Species = 637, Level = 70, Location = 035 }, // Volcarona @ Relic Castle

        // Stationary Legendary
        new(BW) { Species = 638, Level = 42, Location = 074 }, // Cobalion @ Guidance Chamber
        new(BW) { Species = 639, Level = 42, Location = 073 }, // Terrakion @ Trial Chamber
        new(BW) { Species = 640, Level = 42, Location = 055 }, // Virizion @ Rumination Field
        new(BW) { Species = 645, Level = 70, Location = 070 }, // Landorus @ Abundant Shrine
        new(BW) { Species = 646, Level = 75, Location = 061 }, // Kyurem @ Giant Chasm

        // Event
        new(BW) { Species = 494, Level = 15, Location = 062, Shiny = Shiny.Never}, // Victini @ Liberty Garden
        new(BW) { Species = 570, Level = 10, Location = 008, Shiny = Shiny.Never, Gender = 0 }, // Zorua @ Castelia City
        new(BW) { Species = 571, Level = 25, Location = 072, Shiny = Shiny.Never, Gender = 1 }, // Zoroark @ Lostlorn Forest
    ];

    public static readonly EncounterStatic5[] StaticB =
    [
        new(B) { Species = 643, Level = 50, Location = 045, Shiny = Shiny.Never }, // Reshiram @ N's Castle
        new(B) { Species = 643, Level = 50, Location = 039, Shiny = Shiny.Never }, // Reshiram @ Dragonspiral Tower
        new(B) { Roaming = true, Species = 641, Level = 40, Location = 25 }, // Tornadus
    ];
    public static readonly EncounterStatic5[] StaticW =
    [
        new( W) { Species = 644, Level = 50, Location = 045, Shiny = Shiny.Never }, // Zekrom @ N's Castle
        new( W) { Species = 644, Level = 50, Location = 039, Shiny = Shiny.Never }, // Zekrom @ Dragonspiral Tower
        new( W) { Roaming = true, Species = 642, Level = 40, Location = 25 }, // Thundurus
    ];

    #endregion
    #region Trade Tables
    internal static readonly EncounterTrade5BW[] TradeGift_BW =
    [
        new(TradeNames, 04, BW, 0xD400007F) { Species = 587, Level = 30, Ability = OnlyFirst,  ID32 = 11195, OTGender = 0, Gender = 0, IVs = new(20,20,31,20,20,20), Nature = Nature.Lax }, // Emolga
        new(TradeNames, 05, BW, 0x2A000000) { Species = 479, Level = 60, Ability = OnlyFirst,  ID32 = 54673, OTGender = 1, Gender = 2, IVs = new(20,20,20,20,20,31), Nature = Nature.Gentle }, // Rotom
        new(TradeNames, 06, BW, 0x6200001F) { Species = 446, Level = 60, Ability = OnlySecond, ID32 = 40217, OTGender = 0, Gender = 0, IVs = new(31,20,20,20,20,20), Nature = Nature.Serious }, // Munchlax
    ];

    internal static readonly EncounterTrade5BW[] TradeGift_B =
    [
        new(TradeNames, 00, B , 0x64000000) { Species = 548, Level = 15, Ability = OnlyFirst,  ID32 = 39922, OTGender = 1, Gender = 1, IVs = new(20,20,20,20,31,20), Nature = Nature.Modest }, // Petilil
        new(TradeNames, 02, B , 0x9400007F) { Species = 550, Level = 25, Ability = OnlyFirst,  ID32 = 27646, OTGender = 0, Gender = 0, IVs = new(20,31,20,20,20,20), Nature = Nature.Adamant, Form = 0 }, // Basculin-Red
    ];

    internal static readonly EncounterTrade5BW[] TradeGift_W =
    [
        new(TradeNames, 01,  W, 0x6400007E) { Species = 546, Level = 15, Ability = OnlyFirst,  ID32 = 39922, OTGender = 1, Gender = 1, IVs = new(20,20,20,20,31,20), Nature = Nature.Modest }, // Cottonee
        new(TradeNames, 03,  W, 0x9400007F) { Species = 550, Level = 25, Ability = OnlyFirst,  ID32 = 27646, OTGender = 0, Gender = 0, IVs = new(20,31,20,20,20,20), Nature = Nature.Adamant, Form = 1 }, // Basculin-Blue
    ];
    #endregion
}
