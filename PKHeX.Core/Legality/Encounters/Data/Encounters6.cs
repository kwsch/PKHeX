using System.Collections.Generic;
using System.Linq;
using static PKHeX.Core.EncounterUtil;

namespace PKHeX.Core
{
    /// <summary>
    /// Generation 6 Encounters
    /// </summary>
    internal static class Encounters6
    {
        private static readonly EncounterArea6XY FriendSafari = new EncounterArea6XY(Legal.FriendSafari);
        internal static readonly EncounterArea6XY[] SlotsX = ArrayUtil.ConcatAll(EncounterArea6XY.GetAreas(Get("x", "xy"), GameVersion.X), new[] { FriendSafari });
        internal static readonly EncounterArea6XY[] SlotsY = ArrayUtil.ConcatAll(EncounterArea6XY.GetAreas(Get("y", "xy"), GameVersion.Y), new[] { FriendSafari });
        internal static readonly EncounterArea6AO[] SlotsA = EncounterArea6AO.GetAreas(Get("a", "ao"), GameVersion.AS);
        internal static readonly EncounterArea6AO[] SlotsO = EncounterArea6AO.GetAreas(Get("o", "ao"), GameVersion.OR);
        private static byte[][] Get(string resource, string ident) => BinLinker.Unpack(Util.GetBinaryResource($"encounter_{resource}.pkl"), ident);


        static Encounters6()
        {
            MarkEncounterTradeStrings(TradeGift_XY, TradeXY);
            MarkEncounterTradeStrings(TradeGift_AO, TradeAO);

            Encounter_XY.SetVersion(GameVersion.XY);
            Encounter_AO.SetVersion(GameVersion.ORAS);
            TradeGift_XY.SetVersion(GameVersion.XY);
            TradeGift_AO.SetVersion(GameVersion.ORAS);
        }

        private const string tradeXY = "tradexy";
        private const string tradeAO = "tradeao";
        private static readonly string[][] TradeXY = Util.GetLanguageStrings8(tradeXY);
        private static readonly string[][] TradeAO = Util.GetLanguageStrings8(tradeAO);

        #region Static Encounter/Gift Tables
        private static readonly EncounterStatic6[] Encounter_XY =
        {
            // Kalos Starters @ Aquacorde Town
            new EncounterStatic6 { Gift = true, Species = 650, Level = 5, Location = 10, }, // Chespin
            new EncounterStatic6 { Gift = true, Species = 653, Level = 5, Location = 10, }, // Fennekin
            new EncounterStatic6 { Gift = true, Species = 656, Level = 5, Location = 10, }, // Froakie

            // Kanto Starters @ Lumiose City
            new EncounterStatic6 { Gift = true, Species = 1, Level = 10, Location = 22, }, // Bulbasaur
            new EncounterStatic6 { Gift = true, Species = 4, Level = 10, Location = 22, }, // Charmander
            new EncounterStatic6 { Gift = true, Species = 7, Level = 10, Location = 22, }, // Squirtle

            // Fossils @ Ambrette Town
            new EncounterStatic6 { Gift = true, Species = 138, Level = 20, Location = 44, }, // Omanyte
            new EncounterStatic6 { Gift = true, Species = 140, Level = 20, Location = 44, }, // Kabuto
            new EncounterStatic6 { Gift = true, Species = 142, Level = 20, Location = 44, }, // Aerodactyl
            new EncounterStatic6 { Gift = true, Species = 345, Level = 20, Location = 44, }, // Lileep
            new EncounterStatic6 { Gift = true, Species = 347, Level = 20, Location = 44, }, // Anorith
            new EncounterStatic6 { Gift = true, Species = 408, Level = 20, Location = 44, }, // Cranidos
            new EncounterStatic6 { Gift = true, Species = 410, Level = 20, Location = 44, }, // Shieldon
            new EncounterStatic6 { Gift = true, Species = 564, Level = 20, Location = 44, }, // Tirtouga
            new EncounterStatic6 { Gift = true, Species = 566, Level = 20, Location = 44, }, // Archen
            new EncounterStatic6 { Gift = true, Species = 696, Level = 20, Location = 44, }, // Tyrunt
            new EncounterStatic6 { Gift = true, Species = 698, Level = 20, Location = 44, }, // Amaura

            // Gift
            new EncounterStatic6 { Gift = true, Species = 448, Level = 32, Location = 60, Ability = 1, IVs = new[] {06,25,16,31,25,19}, Nature = Nature.Hasty, Gender = 0, Shiny = Shiny.Never }, // Lucario
            new EncounterStatic6 { Gift = true, Species = 131, Level = 30, Location = 62, Ability = 1, IVs = new[] {31,20,20,20,20,20}, Nature = Nature.Docile, }, // Lapras

            // Stationary
            new EncounterStatic6 { Species = 143, Level = 15, Location = 038, Shiny = Shiny.Never, }, // Snorlax

            // Shaking Trash Cans @ Lost Hotel
            new EncounterStatic6 { Species = 568, Level = 35, Location = 142 }, // Trubbish
            new EncounterStatic6 { Species = 569, Level = 36, Location = 142 }, // Garbodor
            new EncounterStatic6 { Species = 569, Level = 37, Location = 142 }, // Garbodor
            new EncounterStatic6 { Species = 569, Level = 38, Location = 142 }, // Garbodor
            new EncounterStatic6 { Species = 479, Level = 38, Location = 142 }, // Rotom

            // Shaking Trash Cans @ Pokemon Village
            new EncounterStatic6 { Species = 569, Level = 46, Location = 98 }, // Garbodor
            new EncounterStatic6 { Species = 569, Level = 47, Location = 98 }, // Garbodor
            new EncounterStatic6 { Species = 569, Level = 48, Location = 98 }, // Garbodor
            new EncounterStatic6 { Species = 569, Level = 49, Location = 98 }, // Garbodor
            new EncounterStatic6 { Species = 569, Level = 50, Location = 98 }, // Garbodor
            new EncounterStatic6 { Species = 354, Level = 46, Location = 98 }, // Banette
            new EncounterStatic6 { Species = 354, Level = 47, Location = 98 }, // Banette
            new EncounterStatic6 { Species = 354, Level = 48, Location = 98 }, // Banette
            new EncounterStatic6 { Species = 354, Level = 49, Location = 98 }, // Banette
            new EncounterStatic6 { Species = 354, Level = 50, Location = 98 }, // Banette

            // Stationary Legendary
            new EncounterStatic6 { Species = 716, Level = 50, Location = 138, Ability = 1, Version = GameVersion.X, Shiny = Shiny.Never, FlawlessIVCount = 3 }, // Xerneas
            new EncounterStatic6 { Species = 717, Level = 50, Location = 138, Ability = 1, Version = GameVersion.Y, Shiny = Shiny.Never, FlawlessIVCount = 3 }, // Yveltal
            new EncounterStatic6 { Species = 718, Level = 70, Location = 140, Ability = 1, Shiny = Shiny.Never, FlawlessIVCount = 3 }, // Zygarde
            new EncounterStatic6 { Species = 150, Level = 70, Location = 168, Ability = 1, Shiny = Shiny.Never, FlawlessIVCount = 3 }, // Mewtwo
            new EncounterStatic6 { Species = 144, Level = 70, Location = 146, Ability = 1, Shiny = Shiny.Never, FlawlessIVCount = 3 }, // Articuno
            new EncounterStatic6 { Species = 145, Level = 70, Location = 146, Ability = 1, Shiny = Shiny.Never, FlawlessIVCount = 3 }, // Zapdos
            new EncounterStatic6 { Species = 146, Level = 70, Location = 146, Ability = 1, Shiny = Shiny.Never, FlawlessIVCount = 3 }, // Moltres
        };

        private static readonly EncounterStatic6[] Encounter_AO_Regular =
        {
            // Starters @ Route 101
            new EncounterStatic6 { Gift = true, Species = 252, Level = 5, Location = 204, }, // Treeko
            new EncounterStatic6 { Gift = true, Species = 255, Level = 5, Location = 204, }, // Torchic
            new EncounterStatic6 { Gift = true, Species = 258, Level = 5, Location = 204, }, // Mudkip

            new EncounterStatic6 { Gift = true, Species = 152, Level = 5, Location = 204, }, // Chikorita
            new EncounterStatic6 { Gift = true, Species = 155, Level = 5, Location = 204, }, // Cyndaquil
            new EncounterStatic6 { Gift = true, Species = 158, Level = 5, Location = 204, }, // Totodile

            new EncounterStatic6 { Gift = true, Species = 387, Level = 5, Location = 204, }, // Turtwig
            new EncounterStatic6 { Gift = true, Species = 390, Level = 5, Location = 204, }, // Chimchar
            new EncounterStatic6 { Gift = true, Species = 393, Level = 5, Location = 204, }, // Piplup

            new EncounterStatic6 { Gift = true, Species = 495, Level = 5, Location = 204, }, // Snivy
            new EncounterStatic6 { Gift = true, Species = 498, Level = 5, Location = 204, }, // Tepig
            new EncounterStatic6 { Gift = true, Species = 501, Level = 5, Location = 204, }, // Oshawott

            // Fossils @ Rustboro City
            new EncounterStatic6 { Gift = true, Species = 138, Level = 20, Location = 190, }, // Omanyte
            new EncounterStatic6 { Gift = true, Species = 140, Level = 20, Location = 190, }, // Kabuto
            new EncounterStatic6 { Gift = true, Species = 142, Level = 20, Location = 190, }, // Aerodactyl
            new EncounterStatic6 { Gift = true, Species = 345, Level = 20, Location = 190, }, // Lileep
            new EncounterStatic6 { Gift = true, Species = 347, Level = 20, Location = 190, }, // Anorith
            new EncounterStatic6 { Gift = true, Species = 408, Level = 20, Location = 190, }, // Cranidos
            new EncounterStatic6 { Gift = true, Species = 410, Level = 20, Location = 190, }, // Shieldon
            new EncounterStatic6 { Gift = true, Species = 564, Level = 20, Location = 190, }, // Tirtouga
            new EncounterStatic6 { Gift = true, Species = 566, Level = 20, Location = 190, }, // Archen
            new EncounterStatic6 { Gift = true, Species = 696, Level = 20, Location = 190, }, // Tyrunt
            new EncounterStatic6 { Gift = true, Species = 698, Level = 20, Location = 190, }, // Amaura

            // Hot Springs Eggs
            new EncounterStatic6 { Gift = true, Species = 360, Level = 1, EggLocation = 60004, Ability = 1, EggCycles = 70 }, // Wynaut
            new EncounterStatic6 { Gift = true, Species = 175, Level = 1, EggLocation = 60004, Ability = 1, EggCycles = 70 }, // Togepi

            // Gift
            new EncounterStatic6 { Species = 374, Level = 01, Location = 196, Ability = 1, IVs = new[] {-1,-1,31,-1,-1,31}, Gift = true }, // Beldum
            new EncounterStatic6 { Species = 351, Level = 30, Location = 240, Ability = 1, IVs = new[] {-1,-1,-1,-1,31,-1}, Contest = new[] {0,100,0,0,0,0}, Gender = 1, Nature = Nature.Lax, Gift = true }, // Castform
            new EncounterStatic6 { Species = 319, Level = 40, Location = 318, Ability = 1, Gender = 1, Nature = Nature.Adamant, Gift = true }, // Sharpedo
            new EncounterStatic6 { Species = 323, Level = 40, Location = 318, Ability = 1, Gender = 1, Nature = Nature.Quiet, Gift = true }, // Camerupt
            new EncounterStatic6 { Species = 380, Level = 30, Location = 320, Ability = 1, Version = GameVersion.AS, Gift = true, FlawlessIVCount = 3 }, // Latias
            new EncounterStatic6 { Species = 381, Level = 30, Location = 320, Ability = 1, Version = GameVersion.OR, Gift = true, FlawlessIVCount = 3 }, // Latios

            // Stationary Legendary
            new EncounterStatic6 { Species = 377, Level = 40, Location = 278, FlawlessIVCount = 3 }, // Regirock
            new EncounterStatic6 { Species = 378, Level = 40, Location = 306, FlawlessIVCount = 3 }, // Regice
            new EncounterStatic6 { Species = 379, Level = 40, Location = 308, FlawlessIVCount = 3 }, // Registeel
            new EncounterStatic6 { Species = 486, Level = 50, Location = 306, FlawlessIVCount = 3 }, // Regigigas
            new EncounterStatic6 { Species = 382, Level = 45, Location = 296, Version = GameVersion.AS, Shiny = Shiny.Never, FlawlessIVCount = 3 }, // Kyogre
            new EncounterStatic6 { Species = 383, Level = 45, Location = 296, Version = GameVersion.OR, Shiny = Shiny.Never, FlawlessIVCount = 3 }, // Groudon
            new EncounterStatic6 { Species = 384, Level = 70, Location = 316, Shiny = Shiny.Never, FlawlessIVCount = 3 }, // Rayquaza
            new EncounterStatic6 { Species = 386, Level = 80, Location = 316, Shiny = Shiny.Never, FlawlessIVCount = 3, Fateful = true }, // Deoxys

            // Hoopa Rings
            new EncounterStatic6 { Species = 249, Level = 50, Location = 304, Version = GameVersion.AS, FlawlessIVCount = 3 }, // Lugia
            new EncounterStatic6 { Species = 250, Level = 50, Location = 304, Version = GameVersion.OR, FlawlessIVCount = 3 }, // Ho-Oh
            new EncounterStatic6 { Species = 483, Level = 50, Location = 348, Version = GameVersion.AS, FlawlessIVCount = 3 }, // Dialga
            new EncounterStatic6 { Species = 484, Level = 50, Location = 348, Version = GameVersion.OR, FlawlessIVCount = 3 }, // Palkia
            new EncounterStatic6 { Species = 644, Level = 50, Location = 340, Version = GameVersion.AS, FlawlessIVCount = 3 }, // Zekrom
            new EncounterStatic6 { Species = 643, Level = 50, Location = 340, Version = GameVersion.OR, FlawlessIVCount = 3 }, // Reshiram
            new EncounterStatic6 { Species = 642, Level = 50, Location = 348, Version = GameVersion.AS, FlawlessIVCount = 3 }, // Thundurus
            new EncounterStatic6 { Species = 641, Level = 50, Location = 348, Version = GameVersion.OR, FlawlessIVCount = 3 }, // Tornadus
            new EncounterStatic6 { Species = 243, Level = 50, Location = 334, FlawlessIVCount = 3 }, // Raikou
            new EncounterStatic6 { Species = 244, Level = 50, Location = 334, FlawlessIVCount = 3 }, // Entei
            new EncounterStatic6 { Species = 245, Level = 50, Location = 334, FlawlessIVCount = 3 }, // Suicune
            new EncounterStatic6 { Species = 480, Level = 50, Location = 338, FlawlessIVCount = 3 }, // Uxie
            new EncounterStatic6 { Species = 481, Level = 50, Location = 338, FlawlessIVCount = 3 }, // Mesprit
            new EncounterStatic6 { Species = 482, Level = 50, Location = 338, FlawlessIVCount = 3 }, // Azelf
            new EncounterStatic6 { Species = 485, Level = 50, Location = 312, FlawlessIVCount = 3 }, // Heatran
            new EncounterStatic6 { Species = 487, Level = 50, Location = 348, FlawlessIVCount = 3 }, // Giratina
            new EncounterStatic6 { Species = 488, Level = 50, Location = 344, FlawlessIVCount = 3 }, // Cresselia
            new EncounterStatic6 { Species = 638, Level = 50, Location = 336, FlawlessIVCount = 3 }, // Cobalion
            new EncounterStatic6 { Species = 639, Level = 50, Location = 336, FlawlessIVCount = 3 }, // Terrakion
            new EncounterStatic6 { Species = 640, Level = 50, Location = 336, FlawlessIVCount = 3 }, // Virizion
            new EncounterStatic6 { Species = 645, Level = 50, Location = 348, FlawlessIVCount = 3 }, // Landorus
            new EncounterStatic6 { Species = 646, Level = 50, Location = 342, FlawlessIVCount = 3 }, // Kyurem

            // Devon Scope Kecleon
            //new EncounterStatic { Species = 352, Level = 30, Location = 240 }, // Kecleon @ Route 119 -- dexnav encounter slot collision; prefer EncounterSlot
            //new EncounterStatic { Species = 352, Level = 30, Location = 242 }, // Kecleon @ Route 120 -- dexnav encounter slot collision; prefer EncounterSlot
            new EncounterStatic6 { Species = 352, Level = 40, Location = 176, Gender = 1, }, // Kecleon @ Lavaridge
            new EncounterStatic6 { Species = 352, Level = 45, Location = 196, Ability = 4, }, // Kecleon @ Mossdeep City

            // Eon Ticket Lati@s
            new EncounterStatic6 { Species = 381, Level = 30, Location = 320, Version = GameVersion.AS, FlawlessIVCount = 3 }, // Latios
            new EncounterStatic6 { Species = 380, Level = 30, Location = 320, Version = GameVersion.OR, FlawlessIVCount = 3 }, // Latias

            // Stationary
            new EncounterStatic6 { Species = 101, Level = 40, Location = 292, Version = GameVersion.AS }, // Electrode
            new EncounterStatic6 { Species = 101, Level = 40, Location = 314, Version = GameVersion.OR }, // Electrode
            new EncounterStatic6 { Species = 100, Level = 20, Location = 302 }, // Voltorb @ Route 119
            new EncounterStatic6 { Species = 442, Level = 50, Location = 304 }, // Spiritomb @ Route 120

            // Soaring in the Sky
            new EncounterStatic6 { Species = 198, Level = 45, Location = 348 }, // Murkrow
            new EncounterStatic6 { Species = 276, Level = 40, Location = 348 }, // Taillow
            new EncounterStatic6 { Species = 278, Level = 40, Location = 348 }, // Wingull
            new EncounterStatic6 { Species = 279, Level = 40, Location = 348 }, // Pelipper
            new EncounterStatic6 { Species = 333, Level = 40, Location = 348 }, // Swablu
            new EncounterStatic6 { Species = 425, Level = 45, Location = 348 }, // Drifloon
            new EncounterStatic6 { Species = 628, Level = 45, Location = 348 }, // Braviary
        };

        private static readonly EncounterStatic[] Encounter_AO = ArrayUtil.ConcatAll(Encounter_AO_Regular, PermuteCosplayPikachu().ToArray());

        private static IEnumerable<EncounterStatic> PermuteCosplayPikachu()
        {
            var CosplayPikachu = new EncounterStatic6
            {
                Species = 25, Level = 20, Gender = 1, Ability = 4, FlawlessIVCount = 3,
                Contest = new[] { 70, 70, 70, 70, 70, 0 }, Gift = true, Shiny = Shiny.Never
            };
            foreach (int loc in new[] { 178, 180, 186, 194 })
            {
                for (int f = 1; f <= 6; f++)
                {
                    var pk = CosplayPikachu.Clone(loc); pk.Form = f;
                    yield return pk;
                }
            }
        }
        #endregion
        #region Trade Tables
        internal static readonly EncounterTrade[] TradeGift_XY =
        {
            new EncounterTrade6(01,3,23,049) { Species = 129, Level = 05, Ability = 1, TID = 44285, IVs = new[] {-1,31,-1,-1,31,-1}, Gender = 0, Nature = Nature.Adamant, }, // Magikarp
            new EncounterTrade6(10,3,00,000) { Species = 133, Level = 05, Ability = 1, TID = 29294, Gender = 1, Nature = Nature.Docile, }, // Eevee

            new EncounterTrade6(15,4,13,017) { Species = 083, Level = 10, Ability = 1, TID = 00185, IVs = new[] {-1,-1,-1,31,-1,-1}, Gender = 0, Nature = Nature.Jolly, }, // Farfetch'd
            new EncounterTrade6(17,5,08,025) { Species = 208, Level = 20, Ability = 1, TID = 19250, IVs = new[] {-1,-1,31,-1,-1,-1}, Gender = 1, Nature = Nature.Impish, }, // Steelix
            new EncounterTrade6(18,7,20,709) { Species = 625, Level = 50, Ability = 1, TID = 03447, IVs = new[] {-1,31,-1,-1,-1,-1}, Gender = 0, Nature = Nature.Adamant, }, // Bisharp

            new EncounterTrade6(02,3,11,005) { Species = 656, Level = 05, Ability = 1, TID = 00037, IVs = new[] {20,20,20,31,20,20}, Gender = 0, Nature = Nature.Jolly, }, // Froakie
            new EncounterTrade6(02,3,09,005) { Species = 650, Level = 05, Ability = 1, TID = 00037, IVs = new[] {20,31,20,20,20,20}, Gender = 0, Nature = Nature.Adamant, }, // Chespin
            new EncounterTrade6(02,3,18,005) { Species = 653, Level = 05, Ability = 1, TID = 00037, IVs = new[] {20,20,20,20,31,20}, Gender = 0, Nature = Nature.Modest, }, // Fennekin
            new EncounterTrade6(51,4,04,033) { Species = 280, Level = 05, Ability = 1, TID = 37110, IVs = new[] {20,20,20,31,31,20}, Gender = 1, Nature = Nature.Modest, IsNicknamed = false, }, // Ralts
        };

        internal static readonly EncounterTrade[] TradeGift_AO =
        {
            new EncounterTrade6(01,3,05,040) { Species = 296, Level = 09, Ability = 2, TID = 30724, IVs = new[] {-1,31,-1,-1,-1,-1}, Gender = 0, Nature = Nature.Brave, }, // Makuhita
            new EncounterTrade6(34,3,13,176) { Species = 300, Level = 30, Ability = 1, TID = 03239, IVs = new[] {-1,-1,-1,31,-1,-1}, Gender = 1, Nature = Nature.Naughty, }, // Skitty
            new EncounterTrade6(07,4,10,319) { Species = 222, Level = 50, Ability = 4, TID = 00325, IVs = new[] {31,-1,-1,-1,-1,31}, Gender = 1, Nature = Nature.Calm, }, // Corsola
        };
        #endregion

        internal static readonly EncounterStatic[] StaticX = GetEncounters(Encounter_XY, GameVersion.X);
        internal static readonly EncounterStatic[] StaticY = GetEncounters(Encounter_XY, GameVersion.Y);
        internal static readonly EncounterStatic[] StaticA = GetEncounters(Encounter_AO, GameVersion.AS);
        internal static readonly EncounterStatic[] StaticO = GetEncounters(Encounter_AO, GameVersion.OR);
    }
}
