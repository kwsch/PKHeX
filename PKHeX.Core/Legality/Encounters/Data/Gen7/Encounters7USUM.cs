using static PKHeX.Core.EncounterUtil;
using static PKHeX.Core.GameVersion;
using static PKHeX.Core.AbilityPermission;

namespace PKHeX.Core;

/// <summary>
/// Generation 7 Encounters
/// </summary>
internal static class Encounters7USUM
{
    internal static readonly EncounterArea7[] SlotsUS = EncounterArea7.GetAreas(Get("us", "uu"u8), US);
    internal static readonly EncounterArea7[] SlotsUM = EncounterArea7.GetAreas(Get("um", "uu"u8), UM);

    private const string tradeUSUM = "tradeusum";
    private static readonly string[][] TradeNames = Util.GetLanguageStrings10(tradeUSUM);

    public static readonly EncounterStatic7[] StaticUSUM =
    [
        new(USUM) { FixedBall = Ball.Poke, Species = 722, Level = 05, Location = 008 }, // Rowlet
        new(USUM) { FixedBall = Ball.Poke, Species = 725, Level = 05, Location = 008 }, // Litten
        new(USUM) { FixedBall = Ball.Poke, Species = 728, Level = 05, Location = 008 }, // Popplio
        new(USUM) { FixedBall = Ball.Poke, Species = 138, Level = 15, Location = 058 }, // Omanyte
        new(USUM) { FixedBall = Ball.Poke, Species = 140, Level = 15, Location = 058 }, // Kabuto
        // new(USUM) { FixedBall = Ball.Poke, Species = 142, Level = 15, Location = 058 }, // Aerodactyl
        new(USUM) { FixedBall = Ball.Poke, Species = 345, Level = 15, Location = 058 }, // Lileep
        new(USUM) { FixedBall = Ball.Poke, Species = 347, Level = 15, Location = 058 }, // Anorith
        new(USUM) { FixedBall = Ball.Poke, Species = 408, Level = 15, Location = 058 }, // Cranidos
        new(USUM) { FixedBall = Ball.Poke, Species = 410, Level = 15, Location = 058 }, // Shieldon
        new(USUM) { FixedBall = Ball.Poke, Species = 564, Level = 15, Location = 058 }, // Tirtouga
        new(USUM) { FixedBall = Ball.Poke, Species = 566, Level = 15, Location = 058 }, // Archen
        new(USUM) { FixedBall = Ball.Poke, Species = 696, Level = 15, Location = 058 }, // Tyrunt
        new(USUM) { FixedBall = Ball.Poke, Species = 698, Level = 15, Location = 058 }, // Amaura
        new(USUM) { FixedBall = Ball.Poke, Species = 133, Level = 01, EggLocation = 60002 }, // Eevee @ Nursery helpers
        new(USUM) { FixedBall = Ball.Poke, Species = 137, Level = 30, Location = 116 }, // Porygon @ Route 15
        new(USUM) { FixedBall = Ball.Poke, Species = 772, Level = 60, Location = 188, FlawlessIVCount = 3 }, // Type: Null @ Aether Paradise
        new(USUM) { FixedBall = Ball.Poke, Species = 772, Level = 60, Location = 160, FlawlessIVCount = 3 }, // Type: Null @ Ancient Poni Path
        new(USUM) { FixedBall = Ball.Poke, Species = 142, Level = 40, Location = 172 }, // Aerodactyl @ Seafolk Village
        new(USUM) { FixedBall = Ball.Poke, Species = 025, Level = 40, Location = 070, FlawlessIVCount = 3, Relearn = new(57) }, // Pikachu @ Heahea City
        new(USUM) { FixedBall = Ball.Poke, Species = 803, Level = 40, Location = 208, FlawlessIVCount = 3 }, // Poipole @ Megalo Tower
        new(USUM) { FixedBall = Ball.Poke, Species = 803, Level = 40, Location = 206, FlawlessIVCount = 3 }, // Poipole @ Ultra Megalopolis

        new(USUM) { FixedBall = Ball.Poke, Species = 778, Level = 40, Ability = OnlyFirst,  Location = 202, Form = 2, Shiny = Shiny.Never, FlawlessIVCount = 3 }, // Mimikyu
        new(USUM) { FixedBall = Ball.Poke, Species = 718, Level = 63, Ability = OnlyFirst,  Location = 118, Form = 1, Shiny = Shiny.Never, FlawlessIVCount = 3 }, // Zygarde (10%) @ Route 16

        new(USUM) // Magearna (Bottle Cap)
        {
            FixedBall = Ball.Cherish, Species = 801, Level = 50, Location = 40001, Shiny = Shiny.Never, FlawlessIVCount = 3, Ability = OnlySecond,
            FatefulEncounter = true, Relearn = new(705, 430, 381, 270), // Cherish
        },

        new(USUM) { FixedBall = Ball.Poke, Species = 718, Form = 0, Level = 50, Shiny = Shiny.Never, Location = 118, FlawlessIVCount = 3 }, // Zygarde (50%)
        new(USUM) { FixedBall = Ball.Poke, Species = 718, Form = 1, Level = 50, Shiny = Shiny.Never, Location = 118, FlawlessIVCount = 3 }, // Zygarde (10%)
        new(USUM) { FixedBall = Ball.Poke, Species = 718, Form = 2, Level = 50, Shiny = Shiny.Never, Location = 118, FlawlessIVCount = 3 }, // Zygarde (10%-C)
        new(USUM) { FixedBall = Ball.Poke, Species = 718, Form = 3, Level = 50, Shiny = Shiny.Never, Location = 118, FlawlessIVCount = 3 }, // Zygarde (50%-C)

        // QR Scan: Su/M/Tu/W/Thu/F/Sa
        // Melemele Island
        new(USUM) { Species = 004, Level = 12, Location = 010, Relearn = new(068,108,052,010) }, // Charmander @ Route 3
        new(USUM) { Species = 007, Level = 12, Location = 042, Relearn = new(453,110,055,033) }, // Squirtle @ Seaward Cave
        new(USUM) { Species = 095, Level = 14, Location = 034, Relearn = new(563,099,317,088) }, // Onix @ Ten Carat Hill
        new(USUM) { Species = 116, Level = 18, Location = 014, Relearn = new(352,239,055,043) }, // Horsea @ Kala'e Bay
        new(USUM) { Species = 664, Level = 09, Location = 020, Relearn = new(476,081,078,033), Form = EncounterStatic7.FormVivillon }, // Scatterbug @ Hau'oli City
        new(USUM) { Species = 001, Level = 10, Location = 012, Relearn = new(580,022,045,033) }, // Bulbasaur @ Route 2
        new(USUM) { Species = 607, Level = 09, Location = 038, Relearn = new(203,052,083,123) }, // Litwick @ Hau'oli Cemetery

        // Akala Island
        new(USUM) { Species = 280, Level = 17, Location = 054, Relearn = new(581,345,381,574) }, // Ralts @ Route 6
        new(USUM) { Species = 363, Level = 19, Location = 056, Relearn = new(187,362,301,227) }, // Spheal @ Route 7
        new(USUM) { Species = 256, Level = 20, Location = 058, Relearn = new(067,488,064,028) }, // Combusken @ Route 8
        new(USUM) { Species = 679, Level = 24, Location = 094, Relearn = new(469,332,425,475) }, // Honedge @ Akala Outskirts
        new(USUM) { Species = 015, Level = 14, Location = 050, Relearn = new(099,031,041,000) }, // Beedrill @ Route 4
        new(USUM) { Species = 253, Level = 16, Location = 052, Relearn = new(580,072,098,071) }, // Grovyle @ Route 5
        new(USUM) { Species = 259, Level = 17, Location = 086, Relearn = new(068,193,189,055) }, // Marshtomp @ Brooklet Hill

        // Ula'ula Island
        new(USUM) { Species = 111, Level = 32, Location = 138, Relearn = new(470,350,498,523) }, // Rhyhorn @ Blush Mountain
        new(USUM) { Species = 220, Level = 33, Location = 114, Relearn = new(333,036,420,196) }, // Swinub @ Tapu Village
        new(USUM) { Species = 394, Level = 35, Location = 118, Relearn = new(681,362,031,117) }, // Prinplup @ Route 16
        new(USUM) { Species = 388, Level = 36, Location = 128, Relearn = new(484,073,072,044) }, // Grotle @ Ula'ula Meadow
        new(USUM) { Species = 018, Level = 29, Location = 106, Relearn = new(211,297,239,098) }, // Pidgeot @ Route 10
        new(USUM) { Species = 391, Level = 29, Location = 108, Relearn = new(612,172,154,259) }, // Monferno @ Route 11
        new(USUM) { Species = 610, Level = 30, Location = 136, Relearn = new(068,337,206,163) }, // Axew @ Mount Hokulani

        // Poni Island
        new(USUM) { Species = 604, Level = 55, Location = 164, Relearn = new(242,435,029,306) }, // Eelektross @ Poni Grove
        new(USUM) { Species = 306, Level = 57, Location = 166, Relearn = new(179,484,038,334) }, // Aggron @ Poni Plains
        new(USUM) { Species = 479, Level = 61, Location = 170, Relearn = new(268,506,486,164) }, // Rotom @ Poni Gauntlet
        new(USUM) { Species = 542, Level = 57, Location = 156, Relearn = new(580,437,014,494) }, // Leavanny @ Poni Meadow
        new(USUM) { Species = 652, Level = 45, Location = 184, Relearn = new(191,341,402,596) }, // Chesnaught @ Exeggutor Island
        new(USUM) { Species = 658, Level = 44, Location = 158, Relearn = new(516,164,185,594) }, // Greninja @ Poni Wilds
        new(USUM) { Species = 655, Level = 44, Location = 160, Relearn = new(273,473,113,595) }, // Delphox @ Ancient Poni Path

        new(USUM) { Species = 785, Level = 60, Location = 030, Ability = OnlyFirst,  Shiny = Shiny.Never, FlawlessIVCount = 3 }, // Tapu Koko @ Ruins of Conflict
        new(USUM) { Species = 786, Level = 60, Location = 092, Ability = OnlyFirst,  Shiny = Shiny.Never, FlawlessIVCount = 3 }, // Tapu Lele @ Ruins of Life
        new(USUM) { Species = 787, Level = 60, Location = 140, Ability = OnlyFirst,  Shiny = Shiny.Never, FlawlessIVCount = 3 }, // Tapu Bulu @ Ruins of Abundance
        new(USUM) { Species = 788, Level = 60, Location = 180, Ability = OnlyFirst,  Shiny = Shiny.Never, FlawlessIVCount = 3 }, // Tapu Fini @ Ruins of Hope

        new(USUM) { Species = 023, Level = 10, Location = 012, Ability = OnlyFirst }, // Ekans @ Route 2

        new(USUM) { Species = 127, Level = 42, Location = 184, Shiny = Shiny.Never }, // Pinsir @ Exeggutor Island
        new(USUM) { Species = 127, Level = 43, Location = 184, Shiny = Shiny.Never }, // Pinsir @ Exeggutor Island
        new(USUM) { Species = 800, Level = 65, Location = 146, Ability = OnlyFirst,  Shiny = Shiny.Never, FlawlessIVCount = 3, Relearn = new(722,334,408,400) }, // Necrozma @ Mount Lanakila

        // Legendaries
        new(USUM) { Species = 144, Level = 60, Location = 222, Ability = OnlyFirst,  FlawlessIVCount = 3, Relearn = new(246,573,115,258) }, // Articuno
        new(USUM) { Species = 145, Level = 60, Location = 222, Ability = OnlyFirst,  FlawlessIVCount = 3, Relearn = new(246,435,365,240) }, // Zapdos
        new(USUM) { Species = 146, Level = 60, Location = 222, Ability = OnlyFirst,  FlawlessIVCount = 3, Relearn = new(246,053,403,241) }, // Moltres
        new(USUM) { Species = 150, Level = 60, Location = 222, Ability = OnlyFirst,  FlawlessIVCount = 3, Relearn = new(094,105,129,427) }, // Mewtwo
        new(USUM) { Species = 245, Level = 60, Location = 222, Ability = OnlyFirst,  FlawlessIVCount = 3, Relearn = new(061,062,054,240) }, // Suicune
        new(USUM) { Species = 377, Level = 60, Location = 222, Ability = OnlyFirst,  FlawlessIVCount = 3 }, // Regirock
        new(USUM) { Species = 378, Level = 60, Location = 222, Ability = OnlyFirst,  FlawlessIVCount = 3 }, // Regice
        new(USUM) { Species = 379, Level = 60, Location = 222, Ability = OnlyFirst,  FlawlessIVCount = 3 }, // Registeel
        new(USUM) { Species = 384, Level = 60, Location = 222, Ability = OnlyFirst,  FlawlessIVCount = 3 }, // Rayquaza
        new(USUM) { Species = 480, Level = 60, Location = 222, Ability = OnlyFirst,  FlawlessIVCount = 3, Relearn = new(326,281,133,129) }, // Uxie
        new(USUM) { Species = 481, Level = 60, Location = 222, Ability = OnlyFirst,  FlawlessIVCount = 3, Relearn = new(326,204,248,129) }, // Mesprit
        new(USUM) { Species = 482, Level = 60, Location = 222, Ability = OnlyFirst,  FlawlessIVCount = 3, Relearn = new(326,417,253,129) }, // Azelf
        new(USUM) { Species = 487, Level = 60, Location = 222, Ability = OnlyFirst,  FlawlessIVCount = 3, Relearn = new(467,396,414,337) }, // Giratina
        new(USUM) { Species = 488, Level = 60, Location = 222, Ability = OnlyFirst,  FlawlessIVCount = 3, Gender = 1 }, // Cresselia
        new(USUM) { Species = 638, Level = 60, Location = 222, Ability = OnlyFirst,  FlawlessIVCount = 3, Relearn = new(533,014,098,442) }, // Cobalion
        new(USUM) { Species = 639, Level = 60, Location = 222, Ability = OnlyFirst,  FlawlessIVCount = 3, Relearn = new(533,014,157,444) }, // Terrakion
        new(USUM) { Species = 640, Level = 60, Location = 222, Ability = OnlyFirst,  FlawlessIVCount = 3, Relearn = new(533,014,202,348) }, // Virizion
        new(USUM) { Species = 645, Level = 60, Location = 222, Ability = OnlyFirst,  FlawlessIVCount = 3, Gender = 0 }, // Landorus
        new(USUM) { Species = 646, Level = 60, Location = 222, Ability = OnlyFirst,  FlawlessIVCount = 3 }, // Kyurem
        new(USUM) { Species = 718, Level = 60, Location = 182, Ability = OnlyFirst,  Shiny = Shiny.Never, FlawlessIVCount = 3, Relearn = new(616,137,219,225) }, // Zygarde @ Resolution Cave

        // Ultra Space Wilds
        new(USUM) { Species = 334, Level = 60, Location = 222, FlawlessIVCount = 3 }, // Altaria
        new(USUM) { Species = 469, Level = 60, Location = 222, FlawlessIVCount = 3 }, // Yanmega
        new(USUM) { Species = 561, Level = 60, Location = 222, FlawlessIVCount = 3 }, // Sigilyph
        new(USUM) { Species = 581, Level = 60, Location = 222, FlawlessIVCount = 3 }, // Swanna
        new(USUM) { Species = 277, Level = 60, Location = 222, FlawlessIVCount = 3 }, // Swellow
        new(USUM) { Species = 452, Level = 60, Location = 222, FlawlessIVCount = 3 }, // Drapion
        new(USUM) { Species = 531, Level = 60, Location = 222, FlawlessIVCount = 3 }, // Audino
        new(USUM) { Species = 695, Level = 60, Location = 222, FlawlessIVCount = 3 }, // Heliolisk
        new(USUM) { Species = 274, Level = 60, Location = 222, FlawlessIVCount = 3 }, // Nuzleaf
        new(USUM) { Species = 326, Level = 60, Location = 222, FlawlessIVCount = 3 }, // Grumpig
        new(USUM) { Species = 460, Level = 60, Location = 222, FlawlessIVCount = 3 }, // Abomasnow
        new(USUM) { Species = 308, Level = 60, Location = 222, FlawlessIVCount = 3 }, // Medicham
        new(USUM) { Species = 450, Level = 60, Location = 222, FlawlessIVCount = 3 }, // Hippowdon
        new(USUM) { Species = 558, Level = 60, Location = 222, FlawlessIVCount = 3 }, // Crustle
        new(USUM) { Species = 219, Level = 60, Location = 222, FlawlessIVCount = 3 }, // Magcargo
        new(USUM) { Species = 689, Level = 60, Location = 222, FlawlessIVCount = 3 }, // Barbaracle
        new(USUM) { Species = 271, Level = 60, Location = 222, FlawlessIVCount = 3 }, // Lombre
        new(USUM) { Species = 618, Level = 60, Location = 222, FlawlessIVCount = 3 }, // Stunfisk
        new(USUM) { Species = 419, Level = 60, Location = 222, FlawlessIVCount = 3 }, // Floatzel
        new(USUM) { Species = 195, Level = 60, Location = 222, FlawlessIVCount = 3 }, // Quagsire

        // Ultra Beasts
        new(USUM) { Species = 793, Level = 60, Location = 190, Ability = OnlyFirst,  FlawlessIVCount = 3, Relearn = new(408,491,446,243) }, // Nihilego @ Ultra Deep Sea
        new(USUM) { Species = 796, Level = 60, Location = 210, Ability = OnlyFirst,  FlawlessIVCount = 3 }, // Xurkitree @ Ultra Plant
        new(USUM) { Species = 799, Level = 60, Location = 220, Ability = OnlyFirst,  FlawlessIVCount = 3 }, // Guzzlord @ Ultra Ruin

        // Ditto Five
        new(USUM) { Species = 132, Level = 29, Location = 060, IVs = new(-1,-1,31,00,30,-1), Nature = Nature.Bold }, // Ditto @ Route 9
        new(USUM) { Species = 132, Level = 29, Location = 072, IVs = new(-1,-1,30,31,30,-1), Nature = Nature.Jolly }, // Ditto @ Konikoni City
        new(USUM) { Species = 132, Level = 29, Location = 072, IVs = new(-1,31,30,30,-1,-1), Nature = Nature.Adamant }, // Ditto @ Konikoni City
        new(USUM) { Species = 132, Level = 29, Location = 072, IVs = new(-1,00,-1,-1,31,30), Nature = Nature.Modest }, // Ditto @ Konikoni City
        new(USUM) { Species = 132, Level = 29, Location = 072, IVs = new(-1,30,-1,31,-1,30), Nature = Nature.Timid }, // Ditto @ Konikoni City

        // Miscellaneous Static
        new(USUM) { Species = 760, Level = 28, Location = 020, Shiny = Shiny.Never }, // Bewear @ Hau’oli City (Shopping District)
        new(USUM) { Species = 097, Level = 29, Location = 020, Shiny = Shiny.Never, Relearn = new(095,171,139,029) }, // Hypno @ Hau'oli City Police Station
        new(USUM) { Species = 097, Level = 29, Location = 020, Shiny = Shiny.Never, Relearn = new(417,060,050,139) }, // Hypno @ Hau'oli City Police Station
        new(USUM) { Species = 097, Level = 29, Location = 020, Shiny = Shiny.Never, Relearn = new(093,050,001,096) }, // Hypno @ Hau'oli City Police Station
        new(USUM) { Species = 092, Level = 19, Location = 230, Shiny = Shiny.Never, Relearn = new(174,109,122,101) }, // Gastly @ Route 1 (Trainers’ School)
        new(USUM) { Species = 425, Level = 19, Location = 230, Shiny = Shiny.Never, Relearn = new(310,132,016,371) }, // Drifloon @ Route 1 (Trainers’ School)
        new(USUM) { Species = 592, Level = 34, Location = 126, Shiny = Shiny.Never, Gender = 1 }, // Frillish @ Route 14
        new(USUM) { Species = 101, Level = 60, Location = 224, Ability = OnlyFirst,  Shiny = Shiny.Never }, // Electrode @ Team Rocket's Castle

        // Crabrawler in Berry Piles
        new(USUM) { Species = 739, Level = 25, Location = 106 }, // Route 10
        new(USUM) { Species = 739, Level = 28, Location = 110 }, // Ula'ula Beach
        new(USUM) { Species = 739, Level = 31, Location = 118 }, // Route 16
        new(USUM) { Species = 739, Level = 32, Location = 120 }, // Route 17
    ];

    public static readonly EncounterStatic7[] StaticUS =
    [
        new(US  ) { FixedBall = Ball.Poke, Species = 789, Level = 05, Location = 142, FlawlessIVCount = 3, Shiny = Shiny.Never, Ability = OnlySecond }, // Cosmog @ Lake of the Sunne

        // Totem-Sized Gifts @ Heahea Beach
        new(US  ) { FixedBall = Ball.Poke, Species = 735, Level = 20, Ability = OnlyHidden, Location = 202, Form = 1, Shiny = Shiny.Never, FlawlessIVCount = 3 }, // Gumshoos
        new(US  ) { FixedBall = Ball.Poke, Species = 105, Level = 25, Ability = OnlyHidden, Location = 202, Form = 2, Shiny = Shiny.Never, FlawlessIVCount = 3 }, // Marowak
        new(US  ) { FixedBall = Ball.Poke, Species = 754, Level = 30, Ability = OnlySecond, Location = 202, Form = 1, Shiny = Shiny.Never, FlawlessIVCount = 3 }, // Lurantis
        new(US  ) { FixedBall = Ball.Poke, Species = 738, Level = 35, Ability = OnlyFirst,  Location = 202, Form = 1, Shiny = Shiny.Never, FlawlessIVCount = 3 }, // Vikavolt
        new(US  ) { FixedBall = Ball.Poke, Species = 743, Level = 50, Ability = OnlyHidden, Location = 202, Form = 1, Shiny = Shiny.Never, FlawlessIVCount = 3 }, // Ribombee

        new(US  ) { Species = 791, Level = 60, Location = 028, Ability = OnlyFirst,  Shiny = Shiny.Never, FlawlessIVCount = 3, Relearn = new(713,322,242,428) }, // Solgaleo @ Mahalo Trail (Plank Bridge)
        new(US  ) { Species = 243, Level = 60, Location = 222, Ability = OnlyFirst,  FlawlessIVCount = 3 }, // Raikou
        new(US  ) { Species = 250, Level = 60, Location = 222, Ability = OnlyFirst,  FlawlessIVCount = 3, Relearn = new(682,221,326,246) }, // Ho-Oh
        new(US  ) { Species = 381, Level = 60, Location = 222, Ability = OnlyFirst,  FlawlessIVCount = 3, Relearn = new(295,406,375,225), Gender = 0 }, // Latios
        new(US  ) { Species = 383, Level = 60, Location = 222, Ability = OnlyFirst,  FlawlessIVCount = 3, Relearn = new(089,619,339,076) }, // Groudon
        new(US  ) { Species = 483, Level = 60, Location = 222, Ability = OnlyFirst,  FlawlessIVCount = 3 }, // Dialga
        new(US  ) { Species = 485, Level = 60, Location = 222, Ability = OnlyFirst,  FlawlessIVCount = 3 }, // Heatran
        new(US  ) { Species = 641, Level = 60, Location = 222, Ability = OnlyFirst,  FlawlessIVCount = 3, Gender = 0 }, // Tornadus
        new(US  ) { Species = 643, Level = 60, Location = 222, Ability = OnlyFirst,  FlawlessIVCount = 3 }, // Reshiram
        new(US  ) { Species = 716, Level = 60, Location = 222, Ability = OnlyFirst,  FlawlessIVCount = 3, Relearn = new(601,532,400,585) }, // Xerneas

        new(US  ) { Species = 794, Level = 60, Location = 218, Ability = OnlyFirst,  FlawlessIVCount = 3 }, // Buzzwole @ Ultra Jungle
        new(US  ) { Species = 798, Level = 60, Location = 216, Ability = OnlyFirst,  FlawlessIVCount = 3 }, // Kartana @ Ultra Forest
        new(US  ) { Species = 806, Level = 60, Location = 164, Ability = OnlyFirst,  FlawlessIVCount = 3 }, // Blacephalon @ Poni Grove
    ];

    public static readonly EncounterStatic7[] StaticUM =
    [
        new(  UM) { FixedBall = Ball.Poke, Species = 789, Level = 05, Location = 144, FlawlessIVCount = 3, Shiny = Shiny.Never, Ability = OnlySecond }, // Cosmog @ Lake of the Moone

        // Totem-Sized Gifts @ Heahea Beach
        new(  UM) { FixedBall = Ball.Poke, Species = 020, Level = 20, Ability = OnlyHidden, Location = 202, Form = 2, Shiny = Shiny.Never, FlawlessIVCount = 3 }, // Raticate
        new(  UM) { FixedBall = Ball.Poke, Species = 752, Level = 25, Ability = OnlyFirst,  Location = 202, Form = 1, Shiny = Shiny.Never, FlawlessIVCount = 3 }, // Araquanid
        new(  UM) { FixedBall = Ball.Poke, Species = 758, Level = 30, Ability = OnlyFirst,  Location = 202, Form = 1, Shiny = Shiny.Never, FlawlessIVCount = 3 }, // Salazzle
        new(  UM) { FixedBall = Ball.Poke, Species = 777, Level = 35, Ability = OnlyHidden, Location = 202, Form = 1, Shiny = Shiny.Never, FlawlessIVCount = 3 }, // Togedemaru
        new(  UM) { FixedBall = Ball.Poke, Species = 784, Level = 50, Ability = OnlyHidden, Location = 202, Form = 1, Shiny = Shiny.Never, FlawlessIVCount = 3 }, // Kommo-o

        new(  UM) { Species = 792, Level = 60, Location = 028, Ability = OnlyFirst,  Shiny = Shiny.Never, FlawlessIVCount = 3, Relearn = new(714,322,539,585) }, // Lunala @ Mahalo Trail (Plank Bridge)
        new(  UM) { Species = 244, Level = 60, Location = 222, Ability = OnlyFirst,  FlawlessIVCount = 3, Relearn = new(023,044,207,436) }, // Entei
        new(  UM) { Species = 249, Level = 60, Location = 222, Ability = OnlyFirst,  FlawlessIVCount = 3, Relearn = new(285,177,326,246) }, // Lugia
        new(  UM) { Species = 380, Level = 60, Location = 222, Ability = OnlyFirst,  FlawlessIVCount = 3, Relearn = new(296,406,375,273), Gender = 1 }, // Latias
        new(  UM) { Species = 382, Level = 60, Location = 222, Ability = OnlyFirst,  FlawlessIVCount = 3, Relearn = new(058,618,347,330) }, // Kyogre
        new(  UM) { Species = 484, Level = 60, Location = 222, Ability = OnlyFirst,  FlawlessIVCount = 3 }, // Palkia
        new(  UM) { Species = 486, Level = 60, Location = 222, Ability = OnlyFirst,  FlawlessIVCount = 3, Relearn = new(428,279,146,109) }, // Regigigas
        new(  UM) { Species = 642, Level = 60, Location = 222, Ability = OnlyFirst,  FlawlessIVCount = 3, Gender = 0 }, // Thundurus
        new(  UM) { Species = 644, Level = 60, Location = 222, Ability = OnlyFirst,  FlawlessIVCount = 3 }, // Zekrom
        new(  UM) { Species = 717, Level = 60, Location = 222, Ability = OnlyFirst,  FlawlessIVCount = 3, Relearn = new(613,399,566,094) }, // Yveltal

        new(  UM) { Species = 795, Level = 60, Location = 214, Ability = OnlyFirst,  FlawlessIVCount = 3 }, // Pheromosa @ Ultra Desert
        new(  UM) { Species = 797, Level = 60, Location = 212, Ability = OnlyFirst,  FlawlessIVCount = 3 }, // Celesteela @ Ultra Crater
        new(  UM) { Species = 805, Level = 60, Location = 164, Ability = OnlyFirst,  FlawlessIVCount = 3 }, // Stakataka @ Poni Grove

        new(  UM) { Species = 769, Level = 30, Location = 116, Shiny = Shiny.Never, Relearn = new(310,523,072,328) }, // Sandygast @ Route 15
    ];

    internal static readonly EncounterTrade7[] TradeGift_USUM =
    [
        // Trades - 4.bin
        new(TradeNames, 00, USUM) { Species = 701, Form = 0, Level = 08, Ability = OnlySecond, ID32 = 000410, IVs = new(-1,31,-1,-1,-1,-1), OTGender = 1, Gender = 0, Nature = Nature.Brave }, // Hawlucha
        new(TradeNames, 01, USUM) { Species = 714, Form = 0, Level = 19, Ability = OnlyFirst,  ID32 = 610507, IVs = new(-1,-1,-1,-1,31,-1), OTGender = 0, Gender = 0, Nature = Nature.Modest }, // Noibat
        new(TradeNames, 02, USUM) { Species = 339, Form = 0, Level = 21, Ability = OnlySecond, ID32 = 590916, IVs = new(31,-1,-1,-1,-1,-1), OTGender = 0, Gender = 1, Nature = Nature.Naughty }, // Barboach
        new(TradeNames, 03, USUM) { Species = 024, Form = 0, Level = 22, Ability = OnlyFirst,  ID32 = 010913, IVs = new(-1,-1,31,-1,-1,-1), OTGender = 1, Gender = 1, Nature = Nature.Impish }, // Arbok
        new(TradeNames, 04, USUM) { Species = 708, Form = 0, Level = 33, Ability = OnlyFirst,  ID32 = 610602, IVs = new(-1,-1,-1,-1,-1,31), OTGender = 0, Gender = 0, Nature = Nature.Calm, EvolveOnTrade = true }, // Phantump
        new(TradeNames, 05, USUM) { Species = 422, Form = 0, Level = 44, Ability = OnlySecond, ID32 = 610503, IVs = new(-1,-1,31,-1,-1,-1), OTGender = 1, Gender = 1, Nature = Nature.Quiet }, // Shellos
        new(TradeNames, 06, USUM) { Species = 128, Form = 0, Level = 59, Ability = OnlyFirst,  ID32 = 581022, IVs = new(-1,-1,-1,31,-1,-1), OTGender = 0, Gender = 0, Nature = Nature.Jolly }, // Tauros
    ];
}
