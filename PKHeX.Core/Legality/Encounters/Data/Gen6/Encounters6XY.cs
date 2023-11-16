using static PKHeX.Core.EncounterUtil;
using static PKHeX.Core.GameVersion;
using static PKHeX.Core.AbilityPermission;

namespace PKHeX.Core;

/// <summary>
/// Generation 6 Encounters
/// </summary>
internal static class Encounters6XY
{
    private static readonly EncounterArea6XY FriendSafari = new();
    internal static readonly EncounterArea6XY[] SlotsX = EncounterArea6XY.GetAreas(Get("x", "xy"u8), X, FriendSafari);
    internal static readonly EncounterArea6XY[] SlotsY = EncounterArea6XY.GetAreas(Get("y", "xy"u8), Y, FriendSafari);

    private const string tradeXY = "tradexy";
    private static readonly string[][] TradeNames = Util.GetLanguageStrings8(tradeXY);

    #region Static Encounter/Gift Tables
    internal static readonly EncounterStatic6[] Encounter_XY =
    [
        // Kalos Starters @ Aquacorde Town
        new(XY) { FixedBall = Ball.Poke, Species = 650, Level = 5, Location = 10 }, // Chespin
        new(XY) { FixedBall = Ball.Poke, Species = 653, Level = 5, Location = 10 }, // Fennekin
        new(XY) { FixedBall = Ball.Poke, Species = 656, Level = 5, Location = 10 }, // Froakie

        // Kanto Starters @ Lumiose City
        new(XY) { FixedBall = Ball.Poke, Species = 001, Level = 10, Location = 22 }, // Bulbasaur
        new(XY) { FixedBall = Ball.Poke, Species = 004, Level = 10, Location = 22 }, // Charmander
        new(XY) { FixedBall = Ball.Poke, Species = 007, Level = 10, Location = 22 }, // Squirtle

        // Fossils @ Ambrette Town
        new(XY) { FixedBall = Ball.Poke, Species = 138, Level = 20, Location = 44 }, // Omanyte
        new(XY) { FixedBall = Ball.Poke, Species = 140, Level = 20, Location = 44 }, // Kabuto
        new(XY) { FixedBall = Ball.Poke, Species = 142, Level = 20, Location = 44 }, // Aerodactyl
        new(XY) { FixedBall = Ball.Poke, Species = 345, Level = 20, Location = 44 }, // Lileep
        new(XY) { FixedBall = Ball.Poke, Species = 347, Level = 20, Location = 44 }, // Anorith
        new(XY) { FixedBall = Ball.Poke, Species = 408, Level = 20, Location = 44 }, // Cranidos
        new(XY) { FixedBall = Ball.Poke, Species = 410, Level = 20, Location = 44 }, // Shieldon
        new(XY) { FixedBall = Ball.Poke, Species = 564, Level = 20, Location = 44 }, // Tirtouga
        new(XY) { FixedBall = Ball.Poke, Species = 566, Level = 20, Location = 44 }, // Archen
        new(XY) { FixedBall = Ball.Poke, Species = 696, Level = 20, Location = 44 }, // Tyrunt
        new(XY) { FixedBall = Ball.Poke, Species = 698, Level = 20, Location = 44 }, // Amaura

        // Gift
        new(XY) { FixedBall = Ball.Poke, Species = 448, Level = 32, Location = 60, Ability = OnlyFirst,  IVs = new(06,25,16,31,25,19), Nature = Nature.Hasty, Gender = 0, Shiny = Shiny.Never }, // Lucario
        new(XY) { FixedBall = Ball.Poke, Species = 131, Level = 30, Location = 62, Ability = OnlyFirst,  IVs = new(31,20,20,20,20,20), Nature = Nature.Docile }, // Lapras

        // Stationary
        new(XY) { Species = 143, Level = 15, Location = 038, Shiny = Shiny.Never }, // Snorlax

        // Shaking Trash Cans @ Lost Hotel
        new(XY) { Species = 568, Level = 35, Location = 142 }, // Trubbish
        new(XY) { Species = 569, Level = 36, Location = 142 }, // Garbodor
        new(XY) { Species = 569, Level = 37, Location = 142 }, // Garbodor
        new(XY) { Species = 569, Level = 38, Location = 142 }, // Garbodor
        new(XY) { Species = 479, Level = 38, Location = 142 }, // Rotom

        // Shaking Trash Cans @ Pokémon Village
        new(XY) { Species = 569, Level = 46, Location = 98 }, // Garbodor
        new(XY) { Species = 569, Level = 47, Location = 98 }, // Garbodor
        new(XY) { Species = 569, Level = 48, Location = 98 }, // Garbodor
        new(XY) { Species = 569, Level = 49, Location = 98 }, // Garbodor
        new(XY) { Species = 569, Level = 50, Location = 98 }, // Garbodor
        new(XY) { Species = 354, Level = 46, Location = 98 }, // Banette
        new(XY) { Species = 354, Level = 47, Location = 98 }, // Banette
        new(XY) { Species = 354, Level = 48, Location = 98 }, // Banette
        new(XY) { Species = 354, Level = 49, Location = 98 }, // Banette
        new(XY) { Species = 354, Level = 50, Location = 98 }, // Banette

        // Stationary Legendary
        new(XY) { Species = 718, Level = 70, Location = 140, Ability = OnlyFirst,  Shiny = Shiny.Never, FlawlessIVCount = 3 }, // Zygarde
        new(XY) { Species = 150, Level = 70, Location = 168, Ability = OnlyFirst,  Shiny = Shiny.Never, FlawlessIVCount = 3 }, // Mewtwo
        new(XY) { Species = 144, Level = 70, Location = 146, Ability = OnlyFirst,  Shiny = Shiny.Never, FlawlessIVCount = 3 }, // Articuno
        new(XY) { Species = 145, Level = 70, Location = 146, Ability = OnlyFirst,  Shiny = Shiny.Never, FlawlessIVCount = 3 }, // Zapdos
        new(XY) { Species = 146, Level = 70, Location = 146, Ability = OnlyFirst,  Shiny = Shiny.Never, FlawlessIVCount = 3 }, // Moltres
    ];

    internal static readonly EncounterStatic6[] StaticX =
    [
        new(X ) { Species = 716, Level = 50, Location = 138, Ability = OnlyFirst,  Shiny = Shiny.Never, FlawlessIVCount = 3 }, // Xerneas
    ];

    internal static readonly EncounterStatic6[] StaticY =
    [
        new( Y) { Species = 717, Level = 50, Location = 138, Ability = OnlyFirst,  Shiny = Shiny.Never, FlawlessIVCount = 3 }, // Yveltal
    ];

    #endregion
    #region Trade Tables
    internal static readonly EncounterTrade6[] TradeGift_XY =
    [
        new(TradeNames, 00, XY, 01,3,23,049) { Species = 129, Level = 05, Ability = OnlyFirst,  ID32 = 44285, Gender = 0, OTGender = 0, IVs = new(-1,31,-1,-1,31,-1), Nature = Nature.Adamant }, // Magikarp
        new(TradeNames, 01, XY, 10,3,00,000) { Species = 133, Level = 05, Ability = OnlyFirst,  ID32 = 29294, Gender = 1, OTGender = 1, IVs = default, Nature = Nature.Docile }, // Eevee

        new(TradeNames, 02, XY, 15,4,13,017) { Species = 083, Level = 10, Ability = OnlyFirst,  ID32 = 00185, Gender = 0, OTGender = 0, IVs = new(-1,-1,-1,31,-1,-1), Nature = Nature.Jolly }, // Farfetch'd
        new(TradeNames, 03, XY, 17,5,08,025) { Species = 208, Level = 20, Ability = OnlyFirst,  ID32 = 19250, Gender = 1, OTGender = 0, IVs = new(-1,-1,31,-1,-1,-1), Nature = Nature.Impish }, // Steelix
        new(TradeNames, 04, XY, 18,7,20,709) { Species = 625, Level = 50, Ability = OnlyFirst,  ID32 = 03447, Gender = 0, OTGender = 1, IVs = new(-1,31,-1,-1,-1,-1), Nature = Nature.Adamant }, // Bisharp

        new(TradeNames, 05, XY, 02,3,11,005) { Species = 656, Level = 05, Ability = OnlyFirst,  ID32 = 00037, Gender = 0, OTGender = 1, IVs = new(20,20,20,31,20,20), Nature = Nature.Jolly }, // Froakie
        new(TradeNames, 06, XY, 02,3,09,005) { Species = 650, Level = 05, Ability = OnlyFirst,  ID32 = 00037, Gender = 0, OTGender = 1, IVs = new(20,31,20,20,20,20), Nature = Nature.Adamant }, // Chespin
        new(TradeNames, 07, XY, 02,3,18,005) { Species = 653, Level = 05, Ability = OnlyFirst,  ID32 = 00037, Gender = 0, OTGender = 1, IVs = new(20,20,20,20,31,20), Nature = Nature.Modest }, // Fennekin
        new(TradeNames, 08, XY, 51,4,04,033) { Species = 280, Level = 05, Ability = OnlyFirst,  ID32 = 37110, Gender = 1, OTGender = 1, IVs = new(20,20,20,31,31,20), Nature = Nature.Modest, IsFixedNickname = false }, // Ralts
    ];
    #endregion
}
