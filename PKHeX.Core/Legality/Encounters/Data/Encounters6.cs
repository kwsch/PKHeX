using static PKHeX.Core.EncounterUtil;
using static PKHeX.Core.GameVersion;

namespace PKHeX.Core
{
    /// <summary>
    /// Generation 6 Encounters
    /// </summary>
    internal static class Encounters6
    {
        private static readonly EncounterArea6XY FriendSafari = new(Legal.FriendSafari);
        internal static readonly EncounterArea6XY[] SlotsX = EncounterArea6XY.GetAreas(Get("x", "xy"), GameVersion.X, FriendSafari);
        internal static readonly EncounterArea6XY[] SlotsY = EncounterArea6XY.GetAreas(Get("y", "xy"), GameVersion.Y, FriendSafari);
        internal static readonly EncounterArea6AO[] SlotsA = EncounterArea6AO.GetAreas(Get("a", "ao"), GameVersion.AS);
        internal static readonly EncounterArea6AO[] SlotsO = EncounterArea6AO.GetAreas(Get("o", "ao"), GameVersion.OR);
        private static byte[][] Get(string resource, string ident) => BinLinker.Unpack(Util.GetBinaryResource($"encounter_{resource}.pkl"), ident);

        static Encounters6()
        {
            MarkEncounterTradeStrings(TradeGift_XY, TradeXY);
            MarkEncounterTradeStrings(TradeGift_AO, TradeAO);
        }

        private const string tradeXY = "tradexy";
        private const string tradeAO = "tradeao";
        private static readonly string[][] TradeXY = Util.GetLanguageStrings8(tradeXY);
        private static readonly string[][] TradeAO = Util.GetLanguageStrings8(tradeAO);

        #region Static Encounter/Gift Tables
        private static readonly EncounterStatic6[] Encounter_XY =
        {
            // Kalos Starters @ Aquacorde Town
            new(XY) { Gift = true, Species = 650, Level = 5, Location = 10, }, // Chespin
            new(XY) { Gift = true, Species = 653, Level = 5, Location = 10, }, // Fennekin
            new(XY) { Gift = true, Species = 656, Level = 5, Location = 10, }, // Froakie

            // Kanto Starters @ Lumiose City
            new(XY) { Gift = true, Species = 1, Level = 10, Location = 22, }, // Bulbasaur
            new(XY) { Gift = true, Species = 4, Level = 10, Location = 22, }, // Charmander
            new(XY) { Gift = true, Species = 7, Level = 10, Location = 22, }, // Squirtle

            // Fossils @ Ambrette Town
            new(XY) { Gift = true, Species = 138, Level = 20, Location = 44, }, // Omanyte
            new(XY) { Gift = true, Species = 140, Level = 20, Location = 44, }, // Kabuto
            new(XY) { Gift = true, Species = 142, Level = 20, Location = 44, }, // Aerodactyl
            new(XY) { Gift = true, Species = 345, Level = 20, Location = 44, }, // Lileep
            new(XY) { Gift = true, Species = 347, Level = 20, Location = 44, }, // Anorith
            new(XY) { Gift = true, Species = 408, Level = 20, Location = 44, }, // Cranidos
            new(XY) { Gift = true, Species = 410, Level = 20, Location = 44, }, // Shieldon
            new(XY) { Gift = true, Species = 564, Level = 20, Location = 44, }, // Tirtouga
            new(XY) { Gift = true, Species = 566, Level = 20, Location = 44, }, // Archen
            new(XY) { Gift = true, Species = 696, Level = 20, Location = 44, }, // Tyrunt
            new(XY) { Gift = true, Species = 698, Level = 20, Location = 44, }, // Amaura

            // Gift
            new(XY) { Gift = true, Species = 448, Level = 32, Location = 60, Ability = 1, IVs = new[] {06,25,16,31,25,19}, Nature = Nature.Hasty, Gender = 0, Shiny = Shiny.Never }, // Lucario
            new(XY) { Gift = true, Species = 131, Level = 30, Location = 62, Ability = 1, IVs = new[] {31,20,20,20,20,20}, Nature = Nature.Docile, }, // Lapras

            // Stationary
            new(XY) { Species = 143, Level = 15, Location = 038, Shiny = Shiny.Never, }, // Snorlax

            // Shaking Trash Cans @ Lost Hotel
            new(XY) { Species = 568, Level = 35, Location = 142 }, // Trubbish
            new(XY) { Species = 569, Level = 36, Location = 142 }, // Garbodor
            new(XY) { Species = 569, Level = 37, Location = 142 }, // Garbodor
            new(XY) { Species = 569, Level = 38, Location = 142 }, // Garbodor
            new(XY) { Species = 479, Level = 38, Location = 142 }, // Rotom

            // Shaking Trash Cans @ Pokemon Village
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
            new(X ) { Species = 716, Level = 50, Location = 138, Ability = 1, Shiny = Shiny.Never, FlawlessIVCount = 3 }, // Xerneas
            new( Y) { Species = 717, Level = 50, Location = 138, Ability = 1, Shiny = Shiny.Never, FlawlessIVCount = 3 }, // Yveltal
            new(XY) { Species = 718, Level = 70, Location = 140, Ability = 1, Shiny = Shiny.Never, FlawlessIVCount = 3 }, // Zygarde
            new(XY) { Species = 150, Level = 70, Location = 168, Ability = 1, Shiny = Shiny.Never, FlawlessIVCount = 3 }, // Mewtwo
            new(XY) { Species = 144, Level = 70, Location = 146, Ability = 1, Shiny = Shiny.Never, FlawlessIVCount = 3 }, // Articuno
            new(XY) { Species = 145, Level = 70, Location = 146, Ability = 1, Shiny = Shiny.Never, FlawlessIVCount = 3 }, // Zapdos
            new(XY) { Species = 146, Level = 70, Location = 146, Ability = 1, Shiny = Shiny.Never, FlawlessIVCount = 3 }, // Moltres
        };

        private static EncounterStatic6 GetCosplayPikachu(int form) => new(ORAS)
        {
            Location = 178, // Or 180, 186, 194
            Form = form,
            Species = 025,
            Level = 20,
            Gender = 1,
            Ability = 4,
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
            new(ORAS) { Gift = true, Species = 252, Level = 5, Location = 204, }, // Treeko
            new(ORAS) { Gift = true, Species = 255, Level = 5, Location = 204, }, // Torchic
            new(ORAS) { Gift = true, Species = 258, Level = 5, Location = 204, }, // Mudkip

            new(ORAS) { Gift = true, Species = 152, Level = 5, Location = 204, }, // Chikorita
            new(ORAS) { Gift = true, Species = 155, Level = 5, Location = 204, }, // Cyndaquil
            new(ORAS) { Gift = true, Species = 158, Level = 5, Location = 204, }, // Totodile

            new(ORAS) { Gift = true, Species = 387, Level = 5, Location = 204, }, // Turtwig
            new(ORAS) { Gift = true, Species = 390, Level = 5, Location = 204, }, // Chimchar
            new(ORAS) { Gift = true, Species = 393, Level = 5, Location = 204, }, // Piplup

            new(ORAS) { Gift = true, Species = 495, Level = 5, Location = 204, }, // Snivy
            new(ORAS) { Gift = true, Species = 498, Level = 5, Location = 204, }, // Tepig
            new(ORAS) { Gift = true, Species = 501, Level = 5, Location = 204, }, // Oshawott

            // Fossils @ Rustboro City
            new(ORAS) { Gift = true, Species = 138, Level = 20, Location = 190, }, // Omanyte
            new(ORAS) { Gift = true, Species = 140, Level = 20, Location = 190, }, // Kabuto
            new(ORAS) { Gift = true, Species = 142, Level = 20, Location = 190, }, // Aerodactyl
            new(ORAS) { Gift = true, Species = 345, Level = 20, Location = 190, }, // Lileep
            new(ORAS) { Gift = true, Species = 347, Level = 20, Location = 190, }, // Anorith
            new(ORAS) { Gift = true, Species = 408, Level = 20, Location = 190, }, // Cranidos
            new(ORAS) { Gift = true, Species = 410, Level = 20, Location = 190, }, // Shieldon
            new(ORAS) { Gift = true, Species = 564, Level = 20, Location = 190, }, // Tirtouga
            new(ORAS) { Gift = true, Species = 566, Level = 20, Location = 190, }, // Archen
            new(ORAS) { Gift = true, Species = 696, Level = 20, Location = 190, }, // Tyrunt
            new(ORAS) { Gift = true, Species = 698, Level = 20, Location = 190, }, // Amaura

            // Hot Springs Eggs
            new(ORAS) { Gift = true, Species = 360, Level = 1, EggLocation = 60004, Ability = 1, EggCycles = 70 }, // Wynaut
            new(ORAS) { Gift = true, Species = 175, Level = 1, EggLocation = 60004, Ability = 1, EggCycles = 70 }, // Togepi

            // Gift
            new(ORAS) { Species = 374, Level = 01, Location = 196, Ability = 1, Gift = true, IVs = new[] {-1,-1,31,-1,-1,31} }, // Beldum
            new(ORAS) { Species = 351, Level = 30, Location = 240, Ability = 1, Gift = true, IVs = new[] {-1,-1,-1,-1,31,-1}, CNT_Beauty = 100, Gender = 1, Nature = Nature.Lax }, // Castform
            new(ORAS) { Species = 319, Level = 40, Location = 318, Ability = 1, Gift = true, Gender = 1, Nature = Nature.Adamant }, // Sharpedo
            new(ORAS) { Species = 323, Level = 40, Location = 318, Ability = 1, Gift = true, Gender = 1, Nature = Nature.Quiet }, // Camerupt
            new(  AS) { Species = 380, Level = 30, Location = 320, Ability = 1, Gift = true, FlawlessIVCount = 3 }, // Latias
            new(OR  ) { Species = 381, Level = 30, Location = 320, Ability = 1, Gift = true, FlawlessIVCount = 3 }, // Latios

            // Stationary Legendary
            new(ORAS) { Species = 377, Level = 40, Location = 278, FlawlessIVCount = 3 }, // Regirock
            new(ORAS) { Species = 378, Level = 40, Location = 306, FlawlessIVCount = 3 }, // Regice
            new(ORAS) { Species = 379, Level = 40, Location = 308, FlawlessIVCount = 3 }, // Registeel
            new(ORAS) { Species = 486, Level = 50, Location = 306, FlawlessIVCount = 3 }, // Regigigas
            new(  AS) { Species = 382, Level = 45, Location = 296, Shiny = Shiny.Never, FlawlessIVCount = 3 }, // Kyogre
            new(OR  ) { Species = 383, Level = 45, Location = 296, Shiny = Shiny.Never, FlawlessIVCount = 3 }, // Groudon
            new(ORAS) { Species = 384, Level = 70, Location = 316, Shiny = Shiny.Never, FlawlessIVCount = 3 }, // Rayquaza
            new(ORAS) { Species = 386, Level = 80, Location = 316, Shiny = Shiny.Never, FlawlessIVCount = 3, Fateful = true }, // Deoxys

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
            //new EncounterStatic { Species = 352, Level = 30, Location = 240 }, // Kecleon @ Route 119 -- dexnav encounter slot collision; prefer EncounterSlot
            //new EncounterStatic { Species = 352, Level = 30, Location = 242 }, // Kecleon @ Route 120 -- dexnav encounter slot collision; prefer EncounterSlot
            new(ORAS) { Species = 352, Level = 40, Location = 176, Gender = 1, }, // Kecleon @ Lavaridge
            new(ORAS) { Species = 352, Level = 45, Location = 196, Ability = 4, }, // Kecleon @ Mossdeep City

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

            GetCosplayPikachu(1),
            GetCosplayPikachu(2),
            GetCosplayPikachu(3),
            GetCosplayPikachu(4),
            GetCosplayPikachu(5),
            GetCosplayPikachu(6),
        };

        private static readonly EncounterStatic6[] Encounter_AO = Encounter_AO_Regular;

        #endregion
        #region Trade Tables
        internal static readonly EncounterTrade6[] TradeGift_XY =
        {
            new(XY, 01,3,23,049) { Species = 129, Level = 05, Ability = 1, TID = 44285, IVs = new[] {-1,31,-1,-1,31,-1}, Gender = 0, Nature = Nature.Adamant, }, // Magikarp
            new(XY, 10,3,00,000) { Species = 133, Level = 05, Ability = 1, TID = 29294, Gender = 1, Nature = Nature.Docile, }, // Eevee

            new(XY, 15,4,13,017) { Species = 083, Level = 10, Ability = 1, TID = 00185, IVs = new[] {-1,-1,-1,31,-1,-1}, Gender = 0, Nature = Nature.Jolly, }, // Farfetch'd
            new(XY, 17,5,08,025) { Species = 208, Level = 20, Ability = 1, TID = 19250, IVs = new[] {-1,-1,31,-1,-1,-1}, Gender = 1, Nature = Nature.Impish, }, // Steelix
            new(XY, 18,7,20,709) { Species = 625, Level = 50, Ability = 1, TID = 03447, IVs = new[] {-1,31,-1,-1,-1,-1}, Gender = 0, Nature = Nature.Adamant, }, // Bisharp

            new(XY, 02,3,11,005) { Species = 656, Level = 05, Ability = 1, TID = 00037, IVs = new[] {20,20,20,31,20,20}, Gender = 0, Nature = Nature.Jolly, }, // Froakie
            new(XY, 02,3,09,005) { Species = 650, Level = 05, Ability = 1, TID = 00037, IVs = new[] {20,31,20,20,20,20}, Gender = 0, Nature = Nature.Adamant, }, // Chespin
            new(XY, 02,3,18,005) { Species = 653, Level = 05, Ability = 1, TID = 00037, IVs = new[] {20,20,20,20,31,20}, Gender = 0, Nature = Nature.Modest, }, // Fennekin
            new(XY, 51,4,04,033) { Species = 280, Level = 05, Ability = 1, TID = 37110, IVs = new[] {20,20,20,31,31,20}, Gender = 1, Nature = Nature.Modest, IsNicknamed = false, }, // Ralts
        };

        internal static readonly EncounterTrade6[] TradeGift_AO =
        {
            new(ORAS, 01,3,05,040) { Species = 296, Level = 09, Ability = 2, TID = 30724, IVs = new[] {-1,31,-1,-1,-1,-1}, Gender = 0, Nature = Nature.Brave, }, // Makuhita
            new(ORAS, 34,3,13,176) { Species = 300, Level = 30, Ability = 1, TID = 03239, IVs = new[] {-1,-1,-1,31,-1,-1}, Gender = 1, Nature = Nature.Naughty, }, // Skitty
            new(ORAS, 07,4,10,319) { Species = 222, Level = 50, Ability = 4, TID = 00325, IVs = new[] {31,-1,-1,-1,-1,31}, Gender = 1, Nature = Nature.Calm, }, // Corsola
        };
        #endregion

        internal static readonly EncounterStatic6[] StaticX = GetEncounters(Encounter_XY, X);
        internal static readonly EncounterStatic6[] StaticY = GetEncounters(Encounter_XY, Y);
        internal static readonly EncounterStatic6[] StaticA = GetEncounters(Encounter_AO, AS);
        internal static readonly EncounterStatic6[] StaticO = GetEncounters(Encounter_AO, OR);
    }
}
