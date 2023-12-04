using static PKHeX.Core.EncounterUtil;
using static PKHeX.Core.GameVersion;
using static PKHeX.Core.AbilityPermission;

namespace PKHeX.Core;

/// <summary>
/// Generation 7 Encounters
/// </summary>
internal static class Encounters7SM
{
    internal static readonly EncounterArea7[] SlotsSN = EncounterArea7.GetAreas(Get("sn", "sm"u8), SN);
    internal static readonly EncounterArea7[] SlotsMN = EncounterArea7.GetAreas(Get("mn", "sm"u8), MN);

    private const string tradeSM = "tradesm";
    private static readonly string[][] TradeNames = Util.GetLanguageStrings10(tradeSM);

    public static readonly EncounterStatic7[] StaticSM = // @ a\1\5\5
    [
        // Gifts - 0.bin
        new(SM) { FixedBall = Ball.Poke, Species = 722, Level = 5,  Location = 024 }, // Rowlet
        new(SM) { FixedBall = Ball.Poke, Species = 725, Level = 5,  Location = 024 }, // Litten
        new(SM) { FixedBall = Ball.Poke, Species = 728, Level = 5,  Location = 024 }, // Popplio
        new(SM) { FixedBall = Ball.Poke, Species = 138, Level = 15, Location = 058 }, // Omanyte
        new(SM) { FixedBall = Ball.Poke, Species = 140, Level = 15, Location = 058 }, // Kabuto
        // new(SM) { FixedBall = Ball.Poke, Species = 142, Level = 15, Location = 058 }, // Aerodactyl
        new(SM) { FixedBall = Ball.Poke, Species = 345, Level = 15, Location = 058 }, // Lileep
        new(SM) { FixedBall = Ball.Poke, Species = 347, Level = 15, Location = 058 }, // Anorith
        new(SM) { FixedBall = Ball.Poke, Species = 408, Level = 15, Location = 058 }, // Cranidos
        new(SM) { FixedBall = Ball.Poke, Species = 410, Level = 15, Location = 058 }, // Shieldon
        new(SM) { FixedBall = Ball.Poke, Species = 564, Level = 15, Location = 058 }, // Tirtouga
        new(SM) { FixedBall = Ball.Poke, Species = 566, Level = 15, Location = 058 }, // Archen
        new(SM) { FixedBall = Ball.Poke, Species = 696, Level = 15, Location = 058 }, // Tyrunt
        new(SM) { FixedBall = Ball.Poke, Species = 698, Level = 15, Location = 058 }, // Amaura
        new(SM) { FixedBall = Ball.Poke, Species = 137, Level = 30, Location = 116 }, // Porygon @ Route 15
        new(SM) { FixedBall = Ball.Poke, Species = 133, Level = 1,  EggLocation = 60002 }, // Eevee @ Nursery helpers
        new(SM) { FixedBall = Ball.Poke, Species = 772, Level = 40, Location = 188, FlawlessIVCount = 3 }, // Type: Null
        new(SN) { FixedBall = Ball.Poke, Species = 789, Level = 5,  Location = 142, Shiny = Shiny.Never, Ability = OnlySecond, FlawlessIVCount = 3 }, // Cosmog
        new(MN) { FixedBall = Ball.Poke, Species = 789, Level = 5,  Location = 144, Shiny = Shiny.Never, Ability = OnlySecond, FlawlessIVCount = 3 }, // Cosmog
        new(SM) { FixedBall = Ball.Poke, Species = 142, Level = 40, Location = 172 }, // Aerodactyl @ Seafolk Village

        new(SM) { FixedBall = Ball.Poke, Species = 718, Form = 0, Level = 30, Location = 118, Shiny = Shiny.Never, FlawlessIVCount = 3 }, // Zygarde
        new(SM) { FixedBall = Ball.Poke, Species = 718, Form = 1, Level = 30, Location = 118, Shiny = Shiny.Never, FlawlessIVCount = 3 }, // Zygarde
        new(SM) { FixedBall = Ball.Poke, Species = 718, Form = 2, Level = 30, Location = 118, Shiny = Shiny.Never, FlawlessIVCount = 3 }, // Zygarde
        new(SM) { FixedBall = Ball.Poke, Species = 718, Form = 3, Level = 30, Location = 118, Shiny = Shiny.Never, FlawlessIVCount = 3 }, // Zygarde

        new(SM) { FixedBall = Ball.Poke, Species = 718, Form = 0, Level = 50, Location = 118, Shiny = Shiny.Never, FlawlessIVCount = 3 }, // Zygarde
        new(SM) { FixedBall = Ball.Poke, Species = 718, Form = 1, Level = 50, Location = 118, Shiny = Shiny.Never, FlawlessIVCount = 3 }, // Zygarde
        new(SM) { FixedBall = Ball.Poke, Species = 718, Form = 2, Level = 50, Location = 118, Shiny = Shiny.Never, FlawlessIVCount = 3 }, // Zygarde
        new(SM) { FixedBall = Ball.Poke, Species = 718, Form = 3, Level = 50, Location = 118, Shiny = Shiny.Never, FlawlessIVCount = 3 }, // Zygarde

        new(SM) // Magearna (Bottle Cap) 00 FF
        {
            FixedBall = Ball.Cherish, Species = 801, Level = 50, Location = 40001, Shiny = Shiny.Never, FlawlessIVCount = 3, Ability = OnlySecond,
            FatefulEncounter = true, Relearn = new(705, 430, 381, 270), // Cherish
        },

        // Static Encounters - 1.bin
        new(SM) { Species = 746, Level = 17, Location = 086, Shiny = Shiny.Never, Ability = OnlyFirst }, // Wishiwashi
        new(SM) { Species = 746, Level = 18, Location = 086, Shiny = Shiny.Never, Ability = OnlyFirst }, // Wishiwashi

        new(SM) { Species = 785, Level = 60, Location = 030, Shiny = Shiny.Never, Ability = OnlyFirst,  FlawlessIVCount = 3 }, // Tapu Koko
        new(SM) { Species = 786, Level = 60, Location = 092, Shiny = Shiny.Never, Ability = OnlyFirst,  FlawlessIVCount = 3 }, // Tapu Lele
        new(SM) { Species = 787, Level = 60, Location = 140, Shiny = Shiny.Never, Ability = OnlyFirst,  FlawlessIVCount = 3 }, // Tapu Bulu
        new(SM) { Species = 788, Level = 60, Location = 180, Shiny = Shiny.Never, Ability = OnlyFirst,  FlawlessIVCount = 3 }, // Tapu Fini

        new(SM) { Species = 793, Level = 55, Location = 082, Shiny = Shiny.Never, Ability = OnlyFirst,  FlawlessIVCount = 3 }, // Nihilego @ Wela Volcano Park
        new(SM) { Species = 793, Level = 55, Location = 100, Shiny = Shiny.Never, Ability = OnlyFirst,  FlawlessIVCount = 3 }, // Nihilego @ Diglett’s Tunnel
        new(SM) { Species = 796, Level = 65, Location = 090, Shiny = Shiny.Never, Ability = OnlyFirst,  FlawlessIVCount = 3 }, // Xurkitree @ Lush Jungle
        new(SM) { Species = 796, Level = 65, Location = 076, Shiny = Shiny.Never, Ability = OnlyFirst,  FlawlessIVCount = 3 }, // Xurkitree @ Memorial Hill
        new(SM) { Species = 799, Level = 70, Location = 182, Shiny = Shiny.Never, Ability = OnlyFirst,  FlawlessIVCount = 3 }, // Guzzlord @ Resolution Cave
        new(SM) { Species = 800, Level = 75, Location = 036, Shiny = Shiny.Never, Ability = OnlyFirst,  FlawlessIVCount = 3 }, // Necrozma @ Ten Carat Hill (Farthest Hollow)

        // QR Scan: Su/M/Tu/W/Thu/F/Sa
        // Melemele Island
        new(SM) { Species = 155, Level = 12, Location = 010, Relearn = new(024, 052, 108, 043) }, // Cyndaquil @ Route 3
        new(SM) { Species = 158, Level = 12, Location = 042, Relearn = new(232, 099, 055, 043) }, // Totodile @ Seaward Cave
        new(SM) { Species = 633, Level = 13, Location = 034, Relearn = new(372, 029, 044, 116) }, // Deino @ Ten Carat Hill
        new(SM) { Species = 116, Level = 18, Location = 014, Relearn = new(225, 239, 055, 043) }, // Horsea @ Kala'e Bay
        new(SM) { Species = 599, Level = 08, Location = 020, Relearn = new(268, 011, 000, 000) }, // Klink @ Hau'oli City
        new(SM) { Species = 152, Level = 10, Location = 012, Relearn = new(073, 077, 075, 045) }, // Chikorita @ Route 2
        new(SM) { Species = 607, Level = 10, Location = 038, Relearn = new(051, 109, 083, 123) }, // Litwick @ Hau'oli Cemetery

        // Akala Island
        new(SM) { Species = 574, Level = 17, Location = 054, Relearn = new(399, 060, 003, 313) }, // Gothita @ Route 6
        new(SM) { Species = 363, Level = 19, Location = 056, Relearn = new(392, 362, 301, 227) }, // Spheal @ Route 7
        new(SM) { Species = 404, Level = 20, Location = 058, Relearn = new(598, 044, 209, 268) }, // Luxio @ Route 8
        new(SM) { Species = 679, Level = 23, Location = 094, Relearn = new(194, 332, 425, 475) }, // Honedge @ Akala Outskirts
        new(SM) { Species = 543, Level = 14, Location = 050, Relearn = new(390, 228, 103, 040) }, // Venipede @ Route 4
        new(SM) { Species = 069, Level = 16, Location = 052, Relearn = new(491, 077, 079, 035) }, // Bellsprout @ Route 5
        new(SM) { Species = 183, Level = 17, Location = 086, Relearn = new(453, 270, 061, 205) }, // Marill @ Brooklet Hill

        // Ula'ula Island
        new(SM) { Species = 111, Level = 30, Location = 138, Relearn = new(130, 350, 498, 523) }, // Rhyhorn @ Blush Mountain
        new(SM) { Species = 220, Level = 31, Location = 114, Relearn = new(573, 036, 420, 196) }, // Swinub @ Tapu Village
        new(SM) { Species = 578, Level = 33, Location = 118, Relearn = new(101, 248, 283, 473) }, // Duosion @ Route 16
        new(SM) { Species = 315, Level = 34, Location = 128, Relearn = new(437, 275, 230, 390) }, // Roselia @ Ula'ula Meadow
        new(SM) { Species = 397, Level = 27, Location = 106, Relearn = new(355, 018, 283, 104) }, // Staravia @ Route 10
        new(SM) { Species = 288, Level = 27, Location = 108, Relearn = new(359, 498, 163, 203) }, // Vigoroth @ Route 11
        new(SM) { Species = 610, Level = 28, Location = 136, Relearn = new(231, 337, 206, 163) }, // Axew @ Mount Hokulani

        // Poni Island
        new(SM) { Species = 604, Level = 55, Location = 164, Relearn = new(435, 051, 029, 306) }, // Eelektross @ Poni Grove
        new(SM) { Species = 534, Level = 57, Location = 166, Relearn = new(409, 276, 264, 444) }, // Conkeldurr @ Poni Plains
        new(SM) { Species = 468, Level = 59, Location = 170, Relearn = new(248, 403, 396, 245) }, // Togekiss @ Poni Gauntlet
        new(SM) { Species = 542, Level = 57, Location = 156, Relearn = new(382, 437, 014, 494) }, // Leavanny @ Poni Meadow
        new(SM) { Species = 497, Level = 43, Location = 184, Relearn = new(137, 489, 348, 021) }, // Serperior @ Exeggutor Island
        new(SM) { Species = 503, Level = 43, Location = 158, Relearn = new(362, 227, 453, 279) }, // Samurott @ Poni Wilds
        new(SM) { Species = 500, Level = 43, Location = 160, Relearn = new(276, 053, 372, 535) }, // Emboar @ Ancient Poni Path
    ];

    public static readonly EncounterStatic7[] StaticSN =
    [
        new(SN) { Species = 791, Level = 55, Location = 176, Shiny = Shiny.Never, Ability = OnlyFirst,  FlawlessIVCount = 3, Relearn = new(713, 322, 242, 428) }, // Solgaleo
        new(SN) { Species = 794, Level = 65, Location = 040, Shiny = Shiny.Never, Ability = OnlyFirst,  FlawlessIVCount = 3 }, // Buzzwole @ Melemele Meadow
        new(SN) { Species = 798, Level = 60, Location = 134, Shiny = Shiny.Never, Ability = OnlyFirst,  FlawlessIVCount = 3 }, // Kartana @ Malie Garden
        new(SN) { Species = 798, Level = 60, Location = 120, Shiny = Shiny.Never, Ability = OnlyFirst,  FlawlessIVCount = 3 }, // Kartana @ Route 17
    ];

    public static readonly EncounterStatic7[] StaticMN =
    [
        new(MN) { Species = 792, Level = 55, Location = 178, Shiny = Shiny.Never, Ability = OnlyFirst,  FlawlessIVCount = 3, Relearn = new(714, 322, 539, 247) }, // Lunala
        new(MN) { Species = 795, Level = 60, Location = 046, Shiny = Shiny.Never, Ability = OnlyFirst,  FlawlessIVCount = 3 }, // Pheromosa @ Verdant Cavern (Trial Site)
        new(MN) { Species = 797, Level = 65, Location = 124, Shiny = Shiny.Never, Ability = OnlyFirst,  FlawlessIVCount = 3 }, // Celesteela @ Haina Desert
        new(MN) { Species = 797, Level = 65, Location = 134, Shiny = Shiny.Never, Ability = OnlyFirst,  FlawlessIVCount = 3 }, // Celesteela @ Malie Garden
    ];

    internal static readonly EncounterTrade7[] TradeGift_SM = // @ a\1\5\5
    [
        // Trades - 4.bin
        new(TradeNames, 00, SM) { Species = 066, Form = 0, Level = 09, Ability = OnlySecond, ID32 = 000410, IVs = new(-1,31,-1,-1,-1,-1), OTGender = 1, Gender = 0, Nature = Nature.Brave }, // Machop
        new(TradeNames, 01, SM) { Species = 761, Form = 0, Level = 16, Ability = OnlyFirst,  ID32 = 610507, IVs = new(-1,31,-1,-1,-1,-1), OTGender = 0, Gender = 1, Nature = Nature.Adamant }, // Bounsweet
        new(TradeNames, 02, SM) { Species = 061, Form = 0, Level = 22, Ability = OnlySecond, ID32 = 590916, IVs = new(31,-1,-1,-1,-1,-1), OTGender = 1, Gender = 1, Nature = Nature.Naughty }, // Poliwhirl
        new(TradeNames, 03, SM) { Species = 440, Form = 0, Level = 27, Ability = OnlySecond, ID32 = 010913, IVs = new(-1,-1,-1,-1,31,-1), OTGender = 1, Gender = 1, Nature = Nature.Calm }, // Happiny
        new(TradeNames, 04, SM) { Species = 075, Form = 1, Level = 32, Ability = OnlyFirst,  ID32 = 610602, IVs = new(-1,-1,31,-1,-1,-1), OTGender = 0, Gender = 0, Nature = Nature.Impish, EvolveOnTrade = true }, // Graveler-1
        new(TradeNames, 05, SM) { Species = 762, Form = 0, Level = 43, Ability = OnlyFirst,  ID32 = 610503, IVs = new(-1,-1,-1,-1,-1,31), OTGender = 1, Gender = 1, Nature = Nature.Careful }, // Steenee
        new(TradeNames, 06, SM) { Species = 663, Form = 0, Level = 59, Ability = OnlyHidden, ID32 = 581022, IVs = new(-1,-1,-1,31,-1,-1), OTGender = 0, Gender = 0, Nature = Nature.Jolly }, // Talonflame
    ];
}
