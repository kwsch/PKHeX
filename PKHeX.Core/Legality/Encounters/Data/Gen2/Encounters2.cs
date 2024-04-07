using static PKHeX.Core.EncounterUtil;
using static PKHeX.Core.GameVersion;

namespace PKHeX.Core;

/// <summary>
/// Generation 2 Encounters
/// </summary>
internal static class Encounters2
{
    internal static readonly EncounterArea2[] SlotsGD = EncounterArea2.GetAreas(Get("gold", "g2"u8), GD);
    internal static readonly EncounterArea2[] SlotsSI = EncounterArea2.GetAreas(Get("silver", "g2"u8), SI);
    internal static readonly EncounterArea2[] SlotsC = EncounterArea2.GetAreas(Get("crystal", "g2"u8), C);

    private const string tradeGSC = "tradegsc";
    private static readonly string[][] TradeNames = Util.GetLanguageStrings8(tradeGSC);

    public static readonly EncounterStatic2[] StaticGSC =
    [
        new(152, 05, GSC) { Location = 001 }, // Chikorita @ New Bark Town
        new(155, 05, GSC) { Location = 001 }, // Cyndaquil @ New Bark Town
        new(158, 05, GSC) { Location = 001 }, // Totodile @ New Bark Town

        new(175, 05, GSC) { Location = 000, IsEgg = true }, // Togepi
        new(131, 20, GSC) { Location = 010 }, // Lapras @ Union Cave
        new(133, 20, GSC) { Location = 016 }, // Eevee @ Goldenrod City

        new(185, 20, GSC) { Location = 020 }, // Sudowoodo @ Route 36
        new(236, 10, GSC) { Location = 035 }, // Tyrogue @ Mt. Mortar

        new(130, 30, GSC) { Location = 038, Shiny = Shiny.Always, Gender = 0, IVs = new(0, 14, 10, 10, 10, 10) }, // Gyarados @ Lake of Rage (forcing shiny IVs result in always Male)
        new(074, 21, GSC) { Location = 036 }, // Geodude @ Rocket Hideout (Mahogany Town)
        new(109, 21, GSC) { Location = 036 }, // Koffing @ Rocket Hideout (Mahogany Town)
        new(100, 23, GSC) { Location = 036 }, // Voltorb @ Rocket Hideout (Mahogany Town)
        new(101, 23, GSC) { Location = 036 }, // Electrode @ Rocket Hideout (Mahogany Town)
        new(143, 50, GSC) { Location = 061 }, // Snorlax @ Vermillion City

        new(211, 05, GSC) { Location = 008 }, // Qwilfish Swarm @ Route 32 (Old Rod)
        new(211, 20, GSC) { Location = 008 }, // Qwilfish Swarm @ Route 32 (Good Rod)
        new(211, 40, GSC) { Location = 008 }, // Qwilfish Swarm @ Route 32 (Super Rod)

        new(137, 15, GSC) { Location = 071 }, // Porygon @ Celadon Game Corner

        // Roamer
        new(243, 40, GSC) { Location = 002 }, // Raikou
        new(244, 40, GSC) { Location = 002 }, // Entei
    ];

    public static readonly EncounterStatic2[] StaticGS =
    [
        new(133, 15, GS) { Location = 071 }, // Eevee @ Celadon Game Corner
        new(122, 15, GS) { Location = 071 }, // Mr. Mime @ Celadon Game Corner

        new(063, 10, GS) { Location = 016 }, // Abra @ Goldenrod City (Game Corner)
        new(147, 10, GS) { Location = 016 }, // Dratini @ Goldenrod City (Game Corner)
        new(023, 10, GS) { Location = 016 }, // Ekans @ Goldenrod City (Game Corner) (Gold)
        new(027, 10, GS) { Location = 016 }, // Sandshrew @ Goldenrod City (Game Corner) (Silver)

        new(223, 05, GS) { Location = 039 }, // Remoraid Swarm @ Route 44 (Old Rod)
        new(223, 20, GS) { Location = 039 }, // Remoraid Swarm @ Route 44 (Good Rod)
        new(223, 40, GS) { Location = 039 }, // Remoraid Swarm @ Route 44 (Super Rod)

        // Roamer
        new(245, 40, GS) { Location = 002 }, // Suicune
    ];

    public static readonly EncounterStatic2[] StaticGD =
    [
        new(249, 70, GD) { Location = 031 }, // Lugia @ Whirl Islands
        new(250, 40, GD) { Location = 023 }, // Ho-Oh @ Tin Tower
    ];

    public static readonly EncounterStatic2[] StaticSI =
    [
        new(249, 40, SI) { Location = 031 }, // Lugia @ Whirl Islands
        new(250, 70, SI) { Location = 023 }, // Ho-Oh @ Tin Tower
    ];

    public static readonly EncounterStatic2[] StaticC =
    [
        new(245, 40, C) { Location = 023 }, // Suicune @ Tin Tower

        // Dragon Master's Quiz Dratini @ Dragon's Den
        new(147, 15, C) { Location = 042, Moves = new((int)Move.Wrap, (int)Move.ThunderWave, (int)Move.Twister, (int)Move.ExtremeSpeed) }, // Dratini ExtremeSpeed
        new(147, 15, C) { Location = 042 }, // Dratini with regular moves

        new(249, 60, C) { Location = 031 }, // Lugia @ Whirl Islands
        new(250, 60, C) { Location = 023 }, // Ho-Oh @ Tin Tower

        new(025, 25, C) { Location = 071 }, // Pikachu @ Celadon Game Corner
        new(246, 40, C) { Location = 071 }, // Larvitar @ Celadon Game Corner

        new(063, 05, C) { Location = 016 }, // Abra @ Goldenrod City (Game Corner)
        new(104, 15, C) { Location = 016 }, // Cubone @ Goldenrod City (Game Corner)
        new(202, 15, C) { Location = 016 }, // Wobbuffet @ Goldenrod City (Game Corner)
    ];

    public static readonly EncounterStatic2[] StaticOddEggC =
    [
        new(172, 05, C) { IsEgg = true, Moves = new((int)Move.ThunderShock,(int)Move.Charm, (int)Move.DizzyPunch)}, // Pichu
        new(173, 05, C) { IsEgg = true, Moves = new((int)Move.Pound,       (int)Move.Charm, (int)Move.DizzyPunch)}, // Cleffa
        new(174, 05, C) { IsEgg = true, Moves = new((int)Move.Sing,        (int)Move.Charm, (int)Move.DizzyPunch)}, // Igglybuff
        new(236, 05, C) { IsEgg = true, Moves = new((int)Move.Tackle,                       (int)Move.DizzyPunch)}, // Tyrogue
        new(238, 05, C) { IsEgg = true, Moves = new((int)Move.Pound,       (int)Move.Lick,  (int)Move.DizzyPunch)}, // Smoochum
        new(239, 05, C) { IsEgg = true, Moves = new((int)Move.QuickAttack, (int)Move.Leer,  (int)Move.DizzyPunch)}, // Elekid
        new(240, 05, C) { IsEgg = true, Moves = new((int)Move.Ember,                        (int)Move.DizzyPunch)}, // Magby
    ];

    internal static readonly EncounterStatic2 CelebiVC = new(251, 30, C) { Location = 014 }; // Celebi @ Ilex Forest (VC)

    internal static readonly EncounterTrade2[] TradeGift_GSC =
    [
        new(TradeNames, 0, 095, 03, 48926) { Gender = 0, IVs = new(08, 09, 06, 06, 06, 06) }, // Onix @ Violet City for Bellsprout [wild]
        new(TradeNames, 1, 066, 05, 37460) { Gender = 1, IVs = new(12, 03, 07, 06, 06, 06) }, // Machop @ Goldenrod City for Drowzee [wild 9, hatched egg 5]
        new(TradeNames, 2, 100, 05, 29189) { Gender = 2, IVs = new(08, 09, 08, 08, 08, 08) }, // Voltorb @ Olivine City for Krabby [egg]
        new(TradeNames, 3, 112, 10, 00283) { Gender = 1, IVs = new(12, 07, 07, 06, 06, 06) }, // Rhydon @ Blackthorn City for Dragonair [wild]
        new(TradeNames, 4, 142, 05, 26491) { Gender = 0, IVs = new(08, 09, 06, 06, 06, 06), OTGender = 1}, // Aerodactyl @ Route 14 for Chansey [egg]
        new(TradeNames, 5, 078, 14, 15616) { Gender = 0, IVs = new(08, 09, 06, 06, 06, 06) }, // Rapidash @ Pewter City for Gloom [wild]

        new(TradeNames, 6, 085, 10, 00283) { Gender = 1, IVs = new(12, 07, 07, 06, 06, 06), OTGender = 1}, // Dodrio @ Blackthorn City for Dragonair [wild]
        new(TradeNames, 7, 178, 15, 15616) { Gender = 0, IVs = new(08, 09, 06, 08, 06, 06) }, // Xatu @ Pewter City for Haunter [wild]
        new(TradeNames, 8, 082, 05, 50082) { Gender = 2, IVs = new(08, 09, 06, 06, 06, 06) }, // Magneton @ Power Plant for Dugtrio [traded for Lickitung]

        new(TradeNames,  9, 021, 10, 01001), // Spearow @ Goldenrod City for free
        new(TradeNames, 10, 213, 15, 00518), // Shuckle @ Cianwood City for free
    ];
}
