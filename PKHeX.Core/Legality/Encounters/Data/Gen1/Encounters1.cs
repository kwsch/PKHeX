using static PKHeX.Core.GameVersion;
using static PKHeX.Core.EncounterUtil;

namespace PKHeX.Core;

/// <summary>
/// Generation 1 Encounters
/// </summary>
internal static class Encounters1
{
    internal static readonly EncounterArea1[] SlotsRD = EncounterArea1.GetAreas(Get("red", "g1"u8), RD);
    internal static readonly EncounterArea1[] SlotsGN = EncounterArea1.GetAreas(Get("blue", "g1"u8), GN);
    internal static readonly EncounterArea1[] SlotsYW = EncounterArea1.GetAreas(Get("yellow", "g1"u8), YW);
    internal static readonly EncounterArea1[] SlotsBU = EncounterArea1.GetAreas(Get("blue_jp", "g1"u8), BU);

    private const string tradeRBY = "traderby";
    private static readonly string[][] TradeNames = Util.GetLanguageStrings7(tradeRBY);

    internal static readonly EncounterStatic1[] StaticRBY =
    [
        // GameVersion is RBY for Pokémon with the same catch rate and initial moves in all games
        // If there are any differences in moves or catch rate, they will be defined as different encounters (GameVersion)
        new(001, 05, RBY), // Bulbasaur
        new(004, 05, RBY), // Charmander
        new(007, 05, RBY), // Squirtle

        // Game Corner
        new(035, 08, RBY), // Clefairy (Red Game Corner)
        new(037, 18, RBY), // Vulpix (Yellow Game Corner)
        new(040, 22, RBY), // Wigglytuff (Yellow Game Corner)
        new(123, 25, RBY), // Scyther (Red Game Corner)
        new(147, 18, RBY), // Dratini (Red Game Corner)

        // Lower level less ideal matches; best match is from above.
        // new(035, 12), // Clefairy (Blue[EN] / Green[JP] Game Corner)
        // new(063, 09), // Abra (Red Game Corner)
        // new(063, 08), // Abra (Blue[JP] Game Corner)
        // new(063, 15), // Abra (Yellow Game Corner)
        // new(123, 30), // Scyther (Yellow Game Corner)
        // new(137, 22), // Porygon (Blue[JP] Game Corner)
        // new(137, 26), // Porygon (Red Game Corner)
        // new(137, 26), // Porygon (Yellow Game Corner)
        // new(147, 24), // Dratini (Blue[EN] / Green[JP] Game Corner)

        new(129, 05, RBY), // Magikarp
        new(143, 30, RBY), // Snorlax
        new(106, 30, RBY), // Hitmonlee
        new(107, 30, RBY), // Hitmonchan

        new(131, 15, RBY), // Lapras
        new(138, 30, RBY), // Omanyte
        new(140, 30, RBY), // Kabuto
        new(142, 30, RBY), // Aerodactyl

        new(144, 50, RBY), // Articuno
        new(145, 50, RBY), // Zapdos
        new(146, 50, RBY), // Moltres

        new(150, 70, RBY), // Mewtwo

        new(100, 40, RBY), // Voltorb (Power Plant)
        new(101, 43, RBY), // Electrode (Power Plant)

        // Yellow Only -- duplicate encounters with a higher level
        // new(001, 10, YW), // Bulbasaur (Cerulean City)
        // new(004, 10, YW), // Charmander (Route 24)
        // new(007, 10, YW), // Squirtle (Vermillion City)
    ];

    internal static readonly EncounterStatic1[] StaticRB =
    [
        new(030, 17, RB), // Nidorina (Red Game Corner)
        new(033, 17, RB), // Nidorino (Blue[EN] / Green[JP] Game Corner)
        new(063, 06, RB), // Abra (Blue[EN] / Green[JP] Game Corner)
        new(133, 25, RB), // Eevee
        new(127, 20, RB), // Pinsir (Blue[EN] / Green[JP] Game Corner)
        new(137, 18, RB), // Porygon (Blue[EN] / Green[JP] Game Corner)
    ];

    internal static readonly EncounterStatic1[] StaticYW =
    [
        new(025, 05, YW), // Pikachu
        new(127, 30, YW), // Pinsir (Yellow Game Corner) (Different initial moves)
        new(133, 25, YW), // Eevee (Different initial moves)
    ];

    internal static readonly EncounterStatic1[] StaticBU =
    [
        new(116, 18, BU), // Horsea (Blue[JP] Game Corner)
        new(036, 24, BU), // Clefable (Blue[JP] Game Corner)
        new(148, 30, BU), // Dragonair (Blue[JP] Game Corner)
        new(025, 12, BU), // Pikachu (Blue[JP] Game Corner) (Different catch rate)
    ];

    internal static readonly EncounterTrade1[] TradeGift_RB =
    [
        new(TradeNames, 00, 122, RB, 06, 05), // Mr. Mime - Abra
        new(TradeNames, 01, 032, RB, 02    ), // Nidoran♂ - Nidoran♀
        new(TradeNames, 02, 030, RB, 16    ), // Nidorina - Nidorino
        new(TradeNames, 03, 108, RB, 15    ), // Lickitung - Slowbro
        new(TradeNames, 04, 124, RB, 15, 10), // Jynx - Poliwhirl
        new(TradeNames, 05, 083, RB, 02    ), // Farfetch’d - Spearow
        new(TradeNames, 06, 101, RB, 03    ), // Electrode - Raichu
        new(TradeNames, 07, 114, RB, 13, 05), // Tangela - Venonat
        new(TradeNames, 08, 086, RB, 28, 05), // Seel - Ponyta
    ];

    public static readonly EncounterTrade1[] TradeGift_YW =
    [
        new(TradeNames, 09, 122, YW, 08, 06), // Mr. Mime - Clefairy
        new(TradeNames, 10, 067, YW, 16, 05) { EvolveOnTrade = true }, // Machoke - Cubone
        new(TradeNames, 11, 051, YW, 15, 05), // Dugtrio - Lickitung
        new(TradeNames, 12, 047, YW, 13, 05), // Parasect - Tangel
        new(TradeNames, 13, 112, YW, 15, 10), // Rhydon - Golduck
        new(TradeNames, 14, 087, YW, 15, 05), // Dewgong - Growlithe
        new(TradeNames, 15, 089, YW, 25, 05), // Muk - Kangaskhan
    ];

    public static readonly EncounterTrade1[] TradeGift_BU =
    [
        new(TradeNames, 16, 122, BU, 03    ), // Mr. Mime - Jigglypuff
        new(TradeNames, 17, 029, BU, 02    ), // Nidoran♀ - Nidoran♂
        new(TradeNames, 18, 060, BU, 02    ), // Poliwag - Rattata
        new(TradeNames, 19, 115, BU, 15, 10), // Kangaskhan - Rhydon
        new(TradeNames, 20, 128, BU, 28, 18), // Tauros - Persian
        new(TradeNames, 21, 093, BU, 28, 14) { EvolveOnTrade = true }, // Haunter - Machop->Machoke
        new(TradeNames, 22, 083, BU, 02    ), // Farfetch’d - Wild Pidgey
        new(TradeNames, 23, 075, BU, 16, 15) { EvolveOnTrade = true }, // Graveler - Abra->Kadabra
        new(TradeNames, 24, 079, BU, 22, 05), // Slowpoke - Seel
        new(TradeNames, 25, 098, BU, 15, 05), // Krabby - Growlithe
    ];
}
