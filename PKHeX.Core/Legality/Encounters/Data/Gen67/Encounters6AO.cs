using static PKHeX.Core.EncounterUtil;
using static PKHeX.Core.GameVersion;
using static PKHeX.Core.AbilityPermission;

namespace PKHeX.Core;

/// <summary>
/// Generation 6 Encounters
/// </summary>
internal static class Encounters6AO
{
    internal static readonly EncounterArea6AO[] SlotsA = EncounterArea6AO.GetAreas(Get("as", "ao"), AS);
    internal static readonly EncounterArea6AO[] SlotsO = EncounterArea6AO.GetAreas(Get("or", "ao"), OR);

    static Encounters6AO()
    {
        MarkEncounterTradeStrings(TradeGift_AO, TradeAO);
    }

    private const string tradeAO = "tradeao";
    private static readonly string[][] TradeAO = Util.GetLanguageStrings8(tradeAO);

    #region Static Encounter/Gift Tables
    private static readonly EncounterStatic6 BaseCosplay = new(ORAS)
    {
        Location = 178, // Or 180, 186, 194
        Species = 025,
        Form = 6,
        Level = 20,
        Gender = 1,
        Ability = OnlyHidden,
        FlawlessIVCount = 3,
        CNT_Cool = 70,
        CNT_Beauty = 70,
        CNT_Cute = 70,
        CNT_Tough = 70,
        CNT_Smart = 70,
        Gift = true,
        Shiny = Shiny.Never,
    };

    private static readonly EncounterStatic6[] Encounter_AO_Regular =
    {
        // Starters @ Route 101
        new(ORAS) { Gift = true, Species = 252, Level = 5, Location = 204 }, // Treeko
        new(ORAS) { Gift = true, Species = 255, Level = 5, Location = 204 }, // Torchic
        new(ORAS) { Gift = true, Species = 258, Level = 5, Location = 204 }, // Mudkip

        new(ORAS) { Gift = true, Species = 152, Level = 5, Location = 204 }, // Chikorita
        new(ORAS) { Gift = true, Species = 155, Level = 5, Location = 204 }, // Cyndaquil
        new(ORAS) { Gift = true, Species = 158, Level = 5, Location = 204 }, // Totodile

        new(ORAS) { Gift = true, Species = 387, Level = 5, Location = 204 }, // Turtwig
        new(ORAS) { Gift = true, Species = 390, Level = 5, Location = 204 }, // Chimchar
        new(ORAS) { Gift = true, Species = 393, Level = 5, Location = 204 }, // Piplup

        new(ORAS) { Gift = true, Species = 495, Level = 5, Location = 204 }, // Snivy
        new(ORAS) { Gift = true, Species = 498, Level = 5, Location = 204 }, // Tepig
        new(ORAS) { Gift = true, Species = 501, Level = 5, Location = 204 }, // Oshawott

        // Fossils @ Rustboro City
        new(ORAS) { Gift = true, Species = 138, Level = 20, Location = 190 }, // Omanyte
        new(ORAS) { Gift = true, Species = 140, Level = 20, Location = 190 }, // Kabuto
        new(ORAS) { Gift = true, Species = 142, Level = 20, Location = 190 }, // Aerodactyl
        new(ORAS) { Gift = true, Species = 345, Level = 20, Location = 190 }, // Lileep
        new(ORAS) { Gift = true, Species = 347, Level = 20, Location = 190 }, // Anorith
        new(ORAS) { Gift = true, Species = 408, Level = 20, Location = 190 }, // Cranidos
        new(ORAS) { Gift = true, Species = 410, Level = 20, Location = 190 }, // Shieldon
        new(ORAS) { Gift = true, Species = 564, Level = 20, Location = 190 }, // Tirtouga
        new(ORAS) { Gift = true, Species = 566, Level = 20, Location = 190 }, // Archen
        new(ORAS) { Gift = true, Species = 696, Level = 20, Location = 190 }, // Tyrunt
        new(ORAS) { Gift = true, Species = 698, Level = 20, Location = 190 }, // Amaura

        // Hot Springs Eggs
        new(ORAS) { Gift = true, Species = 360, Level = 1, EggLocation = 60004, Ability = OnlyFirst,  EggCycles = 70 }, // Wynaut
        new(ORAS) { Gift = true, Species = 175, Level = 1, EggLocation = 60004, Ability = OnlyFirst,  EggCycles = 70 }, // Togepi

        // Gift
        new(ORAS) { Species = 374, Level = 01, Location = 196, Ability = OnlyFirst,  Gift = true, IVs = new(-1,-1,31,-1,-1,31) }, // Beldum
        new(ORAS) { Species = 351, Level = 30, Location = 240, Ability = OnlyFirst,  Gift = true, IVs = new(-1,-1,-1,-1,31,-1), CNT_Beauty = 100, Gender = 1, Nature = Nature.Lax }, // Castform
        new(ORAS) { Species = 319, Level = 40, Location = 318, Ability = OnlyFirst,  Gift = true, Gender = 1, Nature = Nature.Adamant }, // Sharpedo
        new(ORAS) { Species = 323, Level = 40, Location = 318, Ability = OnlyFirst,  Gift = true, Gender = 1, Nature = Nature.Quiet }, // Camerupt
        new(  AS) { Species = 380, Level = 30, Location = 320, Ability = OnlyFirst,  Gift = true, FlawlessIVCount = 3 }, // Latias
        new(OR  ) { Species = 381, Level = 30, Location = 320, Ability = OnlyFirst,  Gift = true, FlawlessIVCount = 3 }, // Latios

        // Stationary Legendary
        new(ORAS) { Species = 377, Level = 40, Location = 278, FlawlessIVCount = 3 }, // Regirock
        new(ORAS) { Species = 378, Level = 40, Location = 306, FlawlessIVCount = 3 }, // Regice
        new(ORAS) { Species = 379, Level = 40, Location = 308, FlawlessIVCount = 3 }, // Registeel
        new(ORAS) { Species = 486, Level = 50, Location = 306, FlawlessIVCount = 3 }, // Regigigas
        new(  AS) { Species = 382, Level = 45, Location = 296, Shiny = Shiny.Never, FlawlessIVCount = 3 }, // Kyogre
        new(OR  ) { Species = 383, Level = 45, Location = 296, Shiny = Shiny.Never, FlawlessIVCount = 3 }, // Groudon
        new(ORAS) { Species = 384, Level = 70, Location = 316, Shiny = Shiny.Never, FlawlessIVCount = 3 }, // Rayquaza
        new(ORAS) { Species = 386, Level = 80, Location = 316, Shiny = Shiny.Never, FlawlessIVCount = 3, FatefulEncounter = true }, // Deoxys

        // Hoopa Rings
        new(  AS) { Species = 249, Level = 50, Location = 304, FlawlessIVCount = 3 }, // Lugia
        new(OR  ) { Species = 250, Level = 50, Location = 304, FlawlessIVCount = 3 }, // Ho-Oh
        new(  AS) { Species = 483, Level = 50, Location = 348, FlawlessIVCount = 3 }, // Dialga
        new(OR  ) { Species = 484, Level = 50, Location = 348, FlawlessIVCount = 3 }, // Palkia
        new(  AS) { Species = 644, Level = 50, Location = 340, FlawlessIVCount = 3 }, // Zekrom
        new(OR  ) { Species = 643, Level = 50, Location = 340, FlawlessIVCount = 3 }, // Reshiram
        new(  AS) { Species = 642, Level = 50, Location = 348, FlawlessIVCount = 3 }, // Thundurus
        new(OR  ) { Species = 641, Level = 50, Location = 348, FlawlessIVCount = 3 }, // Tornadus
        new(ORAS) { Species = 243, Level = 50, Location = 334, FlawlessIVCount = 3 }, // Raikou
        new(ORAS) { Species = 244, Level = 50, Location = 334, FlawlessIVCount = 3 }, // Entei
        new(ORAS) { Species = 245, Level = 50, Location = 334, FlawlessIVCount = 3 }, // Suicune
        new(ORAS) { Species = 480, Level = 50, Location = 338, FlawlessIVCount = 3 }, // Uxie
        new(ORAS) { Species = 481, Level = 50, Location = 338, FlawlessIVCount = 3 }, // Mesprit
        new(ORAS) { Species = 482, Level = 50, Location = 338, FlawlessIVCount = 3 }, // Azelf
        new(ORAS) { Species = 485, Level = 50, Location = 312, FlawlessIVCount = 3 }, // Heatran
        new(ORAS) { Species = 487, Level = 50, Location = 348, FlawlessIVCount = 3 }, // Giratina
        new(ORAS) { Species = 488, Level = 50, Location = 344, FlawlessIVCount = 3 }, // Cresselia
        new(ORAS) { Species = 638, Level = 50, Location = 336, FlawlessIVCount = 3 }, // Cobalion
        new(ORAS) { Species = 639, Level = 50, Location = 336, FlawlessIVCount = 3 }, // Terrakion
        new(ORAS) { Species = 640, Level = 50, Location = 336, FlawlessIVCount = 3 }, // Virizion
        new(ORAS) { Species = 645, Level = 50, Location = 348, FlawlessIVCount = 3 }, // Landorus
        new(ORAS) { Species = 646, Level = 50, Location = 342, FlawlessIVCount = 3 }, // Kyurem

        // Devon Scope Kecleon
        //new(ORAS) { Species = 352, Level = 30, Location = 240 }, // Kecleon @ Route 119 -- dexnav encounter slot collision; prefer EncounterSlot
        //new(ORAS) { Species = 352, Level = 30, Location = 242 }, // Kecleon @ Route 120 -- dexnav encounter slot collision; prefer EncounterSlot
        new(ORAS) { Species = 352, Level = 40, Location = 176, Gender = 1 }, // Kecleon @ Lavaridge
        new(ORAS) { Species = 352, Level = 45, Location = 196, Ability = OnlyHidden }, // Kecleon @ Mossdeep City

        // Eon Ticket Lati@s
        new(  AS) { Species = 381, Level = 30, Location = 320, FlawlessIVCount = 3 }, // Latios
        new(OR  ) { Species = 380, Level = 30, Location = 320, FlawlessIVCount = 3 }, // Latias

        // Stationary
        new(  AS) { Species = 101, Level = 40, Location = 292 }, // Electrode
        new(OR  ) { Species = 101, Level = 40, Location = 314 }, // Electrode
        new(ORAS) { Species = 100, Level = 20, Location = 302 }, // Voltorb @ Route 119
        new(ORAS) { Species = 442, Level = 50, Location = 304 }, // Spiritomb @ Route 120

        // Soaring in the Sky
        new(ORAS) { Species = 198, Level = 45, Location = 348 }, // Murkrow
        new(ORAS) { Species = 276, Level = 40, Location = 348 }, // Taillow
        new(ORAS) { Species = 278, Level = 40, Location = 348 }, // Wingull
        new(ORAS) { Species = 279, Level = 40, Location = 348 }, // Pelipper
        new(ORAS) { Species = 333, Level = 40, Location = 348 }, // Swablu
        new(ORAS) { Species = 425, Level = 45, Location = 348 }, // Drifloon
        new(ORAS) { Species = 628, Level = 45, Location = 348 }, // Braviary

        BaseCosplay with {Form = 1, Moves = new(098, 486, 086, (int)Move.MeteorMash)}, // Rock Star
        BaseCosplay with {Form = 2, Moves = new(098, 486, 086, (int)Move.IcicleCrash)}, // Belle
        BaseCosplay with {Form = 3, Moves = new(098, 486, 086, (int)Move.DrainingKiss)}, // Pop Star
        BaseCosplay with {Form = 4, Moves = new(098, 486, 086, (int)Move.ElectricTerrain)}, // Ph.D.
        BaseCosplay with {Form = 5, Moves = new(098, 486, 086, (int)Move.FlyingPress)}, // Libre
        BaseCosplay, // Cosplay, same 3 level up moves.
    };

    private static readonly EncounterStatic6[] Encounter_AO = Encounter_AO_Regular;

    #endregion
    #region Trade Tables
    internal static readonly EncounterTrade6[] TradeGift_AO =
    {
        new(ORAS, 01,3,05,040) { Species = 296, Level = 09, Ability = OnlySecond, TID16 = 30724, IVs = new(-1,31,-1,-1,-1,-1), Gender = 0, Nature = Nature.Brave }, // Makuhita
        new(ORAS, 34,3,13,176) { Species = 300, Level = 30, Ability = OnlyFirst,  TID16 = 03239, IVs = new(-1,-1,-1,31,-1,-1), Gender = 1, Nature = Nature.Naughty }, // Skitty
        new(ORAS, 07,4,10,319) { Species = 222, Level = 50, Ability = OnlyHidden, TID16 = 00325, IVs = new(31,-1,-1,-1,-1,31), Gender = 1, Nature = Nature.Calm }, // Corsola
    };
    #endregion

    internal static readonly EncounterStatic6[] StaticA = GetEncounters(Encounter_AO, AS);
    internal static readonly EncounterStatic6[] StaticO = GetEncounters(Encounter_AO, OR);
}
