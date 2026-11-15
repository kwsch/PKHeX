using static PKHeX.Core.EncounterUtil;
using static PKHeX.Core.GameVersion;
using static PKHeX.Core.AbilityPermission;

namespace PKHeX.Core;

/// <summary>
/// Generation 5 Encounters
/// </summary>
internal static class Encounters5BW
{
    internal static readonly EncounterArea5[] SlotsB = EncounterArea5.GetAreas(Get("b", "51"u8), B);
    internal static readonly EncounterArea5[] SlotsW = EncounterArea5.GetAreas(Get("w", "51"u8), W);

    private const string tradeBW = "tradebw";
    private static readonly string[][] TradeNames = GetLanguageStrings(tradeBW, 8);

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
        new(B) { IsRoaming = true, Species = 641, Level = 40, Location = 25 }, // Tornadus
    ];
    public static readonly EncounterStatic5[] StaticW =
    [
        new( W) { Species = 644, Level = 50, Location = 045, Shiny = Shiny.Never }, // Zekrom @ N's Castle
        new( W) { Species = 644, Level = 50, Location = 039, Shiny = Shiny.Never }, // Zekrom @ Dragonspiral Tower
        new( W) { IsRoaming = true, Species = 642, Level = 40, Location = 25 }, // Thundurus
    ];

    #endregion
    #region Trade Tables
    internal static readonly EncounterTrade5BW[] TradeGift_BW =
    [
        new(TradeNames, 04, BW, 0xD400007F) { Species = 587, Level = 30, Ability = OnlyFirst,  TID16 = 11195, OTGender = 0, Gender = 0, IVs = new(20,20,31,20,20,20), Nature = Nature.Lax }, // Emolga
        new(TradeNames, 05, BW, 0x2A000000) { Species = 479, Level = 60, Ability = OnlyFirst,  TID16 = 54673, OTGender = 1, Gender = 2, IVs = new(20,20,20,20,20,31), Nature = Nature.Gentle }, // Rotom
        new(TradeNames, 06, BW, 0x6200001F) { Species = 446, Level = 60, Ability = OnlySecond, TID16 = 40217, OTGender = 0, Gender = 0, IVs = new(31,20,20,20,20,20), Nature = Nature.Serious }, // Munchlax
    ];

    internal static readonly EncounterTrade5BW[] TradeGift_B =
    [
        new(TradeNames, 00, B , 0x64000000) { Species = 548, Level = 15, Ability = OnlyFirst,  TID16 = 39922, OTGender = 1, Gender = 1, IVs = new(20,20,20,20,31,20), Nature = Nature.Modest }, // Petilil
        new(TradeNames, 02, B , 0x9400007F) { Species = 550, Level = 25, Ability = OnlyFirst,  TID16 = 27646, OTGender = 0, Gender = 0, IVs = new(20,31,20,20,20,20), Nature = Nature.Adamant, Form = 0 }, // Basculin-Red
    ];

    internal static readonly EncounterTrade5BW[] TradeGift_W =
    [
        new(TradeNames, 01,  W, 0x6400007E) { Species = 546, Level = 15, Ability = OnlyFirst,  TID16 = 39922, OTGender = 1, Gender = 1, IVs = new(20,20,20,20,31,20), Nature = Nature.Modest }, // Cottonee
        new(TradeNames, 03,  W, 0x9400007F) { Species = 550, Level = 25, Ability = OnlyFirst,  TID16 = 27646, OTGender = 0, Gender = 0, IVs = new(20,31,20,20,20,20), Nature = Nature.Adamant, Form = 1 }, // Basculin-Blue
    ];
    #endregion
}
