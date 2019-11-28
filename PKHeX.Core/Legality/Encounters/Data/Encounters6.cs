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
        internal static readonly EncounterArea6XY[] SlotsX, SlotsY;
        internal static readonly EncounterArea6AO[] SlotsA, SlotsO;
        internal static readonly EncounterStatic[] StaticX, StaticY, StaticA, StaticO;
        internal static readonly ILookup<int, EncounterSlot> FriendSafari;

        static Encounters6()
        {
            StaticX = GetStaticEncounters(Encounter_XY, GameVersion.X);
            StaticY = GetStaticEncounters(Encounter_XY, GameVersion.Y);
            StaticA = GetStaticEncounters(Encounter_AO, GameVersion.AS);
            StaticO = GetStaticEncounters(Encounter_AO, GameVersion.OR);

            var XSlots = GetEncounterTables<EncounterArea6XY>("xy", "x");
            var YSlots = GetEncounterTables<EncounterArea6XY>("xy", "y");
            MarkG6XYSlots(ref XSlots);
            MarkG6XYSlots(ref YSlots);
            MarkEncounterAreaArray(SlotsXYAlt);
            SlotsX = AddExtraTableSlots(XSlots, SlotsXYAlt);
            SlotsY = AddExtraTableSlots(YSlots, SlotsXYAlt);

            SlotsA = GetEncounterTables<EncounterArea6AO>("ao", "a");
            SlotsO = GetEncounterTables<EncounterArea6AO>("ao", "o");
            MarkG6AOSlots(ref SlotsA);
            MarkG6AOSlots(ref SlotsO);

            MarkEncountersGeneration(6, SlotsX, SlotsY, SlotsA, SlotsO);
            MarkEncountersGeneration(6, StaticX, StaticY, StaticA, StaticO, TradeGift_XY, TradeGift_AO);

            FriendSafari = GetFriendSafariArea();
            MarkEncounterTradeStrings(TradeGift_XY, TradeXY);
            MarkEncounterTradeStrings(TradeGift_AO, TradeAO);

            SlotsXYAlt.SetVersion(GameVersion.XY);
            SlotsX.SetVersion(GameVersion.X);
            SlotsY.SetVersion(GameVersion.Y);
            SlotsA.SetVersion(GameVersion.AS);
            SlotsO.SetVersion(GameVersion.OR);
            Encounter_XY.SetVersion(GameVersion.XY);
            Encounter_AO.SetVersion(GameVersion.ORAS);
            TradeGift_XY.SetVersion(GameVersion.XY);
            TradeGift_AO.SetVersion(GameVersion.ORAS);
        }

        private static ILookup<int, EncounterSlot> GetFriendSafariArea()
        {
            var area = new EncounterAreaFake { Location = 148 };
            EncounterSlot FriendSafariSlot(int d)
            {
                return new EncounterSlot
                {
                    Area = area,
                    Generation = 6,
                    Species = d,
                    LevelMin = 30,
                    LevelMax = 30,
                    Form = 0,
                    Type = SlotType.FriendSafari,
                    Version = GameVersion.XY,
                };
            }
            area.Slots = Legal.FriendSafari.Select(FriendSafariSlot).ToArray();
            return area.Slots.ToLookup(s => s.Species);
        }

        private static void MarkG6XYSlots(ref EncounterArea6XY[] Areas)
        {
            foreach (var area in Areas)
            {
                int slotct = area.Slots.Length;
                for (int i = slotct - 15; i < slotct; i++)
                    area.Slots[i].Type = SlotType.Horde;
            }
            ReduceAreasSize(ref Areas);
        }

        private static void MarkG6AOSlots(ref EncounterArea6AO[] Areas)
        {
            foreach (var area in Areas)
            {
                for (int i = 32; i < 37; i++)
                    area.Slots[i].Type = SlotType.Rock_Smash;
                int slotct = area.Slots.Length;
                for (int i = slotct - 15; i < slotct; i++)
                    area.Slots[i].Type = SlotType.Horde;

                for (int i = 0; i < slotct; i++)
                    area.Slots[i].Permissions.AllowDexNav = area.Slots[i].Type != SlotType.Rock_Smash;
            }
            ReduceAreasSize(ref Areas);
        }

        private const string tradeXY = "tradexy";
        private const string tradeAO = "tradeao";
        private static readonly string[][] TradeXY = Util.GetLanguageStrings8(tradeXY);
        private static readonly string[][] TradeAO = Util.GetLanguageStrings8(tradeAO);

        #region XY Alt Slots
        private static readonly EncounterArea6XY[] SlotsXYAlt =
        {
            new EncounterArea6XY {
                Location = 104, // Victory Road
                Slots = new[]
                {
                    // Drops
                    new EncounterSlot { Species = 075, LevelMin = 57, LevelMax = 57, Form = 0 }, // Graveler
                    new EncounterSlot { Species = 168, LevelMin = 58, LevelMax = 59, Form = 0 }, // Ariados
                    new EncounterSlot { Species = 714, LevelMin = 57, LevelMax = 59, Form = 0 }, // Noibat

                    // Swoops
                    new EncounterSlot { Species = 022, LevelMin = 57, LevelMax = 59, Form = 0 }, // Fearow
                    new EncounterSlot { Species = 227, LevelMin = 57, LevelMax = 59, Form = 0 }, // Skarmory
                    new EncounterSlot { Species = 635, LevelMin = 59, LevelMax = 59, Form = 0 }, // Hydreigon
                },},
            new EncounterArea6XY {
                Location = 34, // Route 6
                Slots = new[]
                {
                    // Rustling Bush
                    new EncounterSlot { Species = 543, LevelMin = 10, LevelMax = 12, Form = 0 }, // Venipede
                    new EncounterSlot { Species = 531, LevelMin = 10, LevelMax = 12, Form = 0 }, // Audino
                },},

            new EncounterArea6XY { Location = 38, // Route 7
                Slots = new[]
                {
                    // Berry Field
                    new EncounterSlot { Species = 165, LevelMin = 14, LevelMax = 15, Form = 0 }, // Ledyba
                    new EncounterSlot { Species = 313, LevelMin = 14, LevelMax = 15, Form = 0 }, // Volbeat
                    new EncounterSlot { Species = 314, LevelMin = 14, LevelMax = 15, Form = 0 }, // Illumise
                    new EncounterSlot { Species = 412, LevelMin = 14, LevelMax = 15, Form = 0 }, // Burmy
                    new EncounterSlot { Species = 415, LevelMin = 14, LevelMax = 15, Form = 0 }, // Combee
                    new EncounterSlot { Species = 665, LevelMin = 14, LevelMax = 15, Form = 0 }, // Spewpa
                },},

            new EncounterArea6XY { Location = 88, // Route 18
                Slots = new[]
                {
                    // Rustling Bush
                    new EncounterSlot { Species = 632, LevelMin = 44, LevelMax = 46, Form = 0 }, // Durant
                    new EncounterSlot { Species = 631, LevelMin = 45, LevelMax = 45, Form = 0 }, // Heatmor
                },},

            new EncounterArea6XY { Location = 132, // Glittering Cave
                Slots = new[]
                {
                    // Drops
                    new EncounterSlot { Species = 527, LevelMin = 15, LevelMax = 17, Form = 0 }, // Woobat
                    new EncounterSlot { Species = 597, LevelMin = 15, LevelMax = 17, Form = 0 }, // Ferroseed
                },},

            new EncounterArea6XY { Location = 56, // Reflection Cave
                Slots = new[]
                {
                    // Drops
                    new EncounterSlot { Species = 527, LevelMin = 21, LevelMax = 23, Form = 0 }, // Woobat
                    new EncounterSlot { Species = 597, LevelMin = 21, LevelMax = 23, Form = 0 }, // Ferroseed
                },},

            new EncounterArea6XY { Location = 140, // Terminus Cave
                Slots = new[]
                {
                    // Drops
                    new EncounterSlot { Species = 168, LevelMin = 44, LevelMax = 46, Form = 0 }, // Ariados
                    new EncounterSlot { Species = 714, LevelMin = 44, LevelMax = 46, Form = 0 }, // Noibat
                },},
        };
        #endregion
        #region Static Encounter/Gift Tables
        private static readonly EncounterStatic[] Encounter_XY =
        {
            // Kalos Starters @ Aquacorde Town
            new EncounterStatic { Gift = true, Species = 650, Level = 5, Location = 10, }, // Chespin
            new EncounterStatic { Gift = true, Species = 653, Level = 5, Location = 10, }, // Fennekin
            new EncounterStatic { Gift = true, Species = 656, Level = 5, Location = 10, }, // Froakie

            // Kanto Starters @ Lumiose City
            new EncounterStatic { Gift = true, Species = 1, Level = 10, Location = 22, }, // Bulbasaur
            new EncounterStatic { Gift = true, Species = 4, Level = 10, Location = 22, }, // Charmander
            new EncounterStatic { Gift = true, Species = 7, Level = 10, Location = 22, }, // Squirtle

            // Fossils @ Ambrette Town
            new EncounterStatic { Gift = true, Species = 138, Level = 20, Location = 44, }, // Omanyte
            new EncounterStatic { Gift = true, Species = 140, Level = 20, Location = 44, }, // Kabuto
            new EncounterStatic { Gift = true, Species = 142, Level = 20, Location = 44, }, // Aerodactyl
            new EncounterStatic { Gift = true, Species = 345, Level = 20, Location = 44, }, // Lileep
            new EncounterStatic { Gift = true, Species = 347, Level = 20, Location = 44, }, // Anorith
            new EncounterStatic { Gift = true, Species = 408, Level = 20, Location = 44, }, // Cranidos
            new EncounterStatic { Gift = true, Species = 410, Level = 20, Location = 44, }, // Shieldon
            new EncounterStatic { Gift = true, Species = 564, Level = 20, Location = 44, }, // Tirtouga
            new EncounterStatic { Gift = true, Species = 566, Level = 20, Location = 44, }, // Archen
            new EncounterStatic { Gift = true, Species = 696, Level = 20, Location = 44, }, // Tyrunt
            new EncounterStatic { Gift = true, Species = 698, Level = 20, Location = 44, }, // Amaura

            // Gift
            new EncounterStatic { Species = 448, Level = 32, Location = 60, Ability = 1, IVs = new[] {06,25,16,31,25,19}, Nature = Nature.Hasty, Gender = 0, Gift = true, Shiny = Shiny.Never }, // Lucario
            new EncounterStatic { Species = 131, Level = 30, Location = 62, Ability = 1, IVs = new[] {31,20,20,20,20,20}, Nature = Nature.Docile, Gift = true }, // Lapras

            // Stationary
            new EncounterStatic { Species = 143, Level = 15, Location = 038, Shiny = Shiny.Never, }, // Snorlax

            // Shaking Trash Cans @ Lost Hotel
            new EncounterStatic { Species = 568, Level = 35, Location = 142 }, // Trubbish
            new EncounterStatic { Species = 569, Level = 36, Location = 142 }, // Garbodor
            new EncounterStatic { Species = 569, Level = 37, Location = 142 }, // Garbodor
            new EncounterStatic { Species = 569, Level = 38, Location = 142 }, // Garbodor
            new EncounterStatic { Species = 479, Level = 38, Location = 142 }, // Rotom

            // Shaking Trash Cans @ Pokemon Village
            new EncounterStatic { Species = 569, Level = 46, Location = 98 }, // Garbodor
            new EncounterStatic { Species = 569, Level = 47, Location = 98 }, // Garbodor
            new EncounterStatic { Species = 569, Level = 48, Location = 98 }, // Garbodor
            new EncounterStatic { Species = 569, Level = 49, Location = 98 }, // Garbodor
            new EncounterStatic { Species = 569, Level = 50, Location = 98 }, // Garbodor
            new EncounterStatic { Species = 354, Level = 46, Location = 98 }, // Banette
            new EncounterStatic { Species = 354, Level = 47, Location = 98 }, // Banette
            new EncounterStatic { Species = 354, Level = 48, Location = 98 }, // Banette
            new EncounterStatic { Species = 354, Level = 49, Location = 98 }, // Banette
            new EncounterStatic { Species = 354, Level = 50, Location = 98 }, // Banette

            // Stationary Legendary
            new EncounterStatic { Species = 716, Level = 50, Location = 138, Ability = 1, Version = GameVersion.X, Shiny = Shiny.Never, FlawlessIVCount = 3 }, // Xerneas
            new EncounterStatic { Species = 717, Level = 50, Location = 138, Ability = 1, Version = GameVersion.Y, Shiny = Shiny.Never, FlawlessIVCount = 3 }, // Yveltal
            new EncounterStatic { Species = 718, Level = 70, Location = 140, Ability = 1, Shiny = Shiny.Never, FlawlessIVCount = 3 }, // Zygarde
            new EncounterStatic { Species = 150, Level = 70, Location = 168, Ability = 1, Shiny = Shiny.Never, FlawlessIVCount = 3 }, // Mewtwo
            new EncounterStatic { Species = 144, Level = 70, Location = 146, Ability = 1, Shiny = Shiny.Never, FlawlessIVCount = 3 }, // Articuno
            new EncounterStatic { Species = 145, Level = 70, Location = 146, Ability = 1, Shiny = Shiny.Never, FlawlessIVCount = 3 }, // Zapdos
            new EncounterStatic { Species = 146, Level = 70, Location = 146, Ability = 1, Shiny = Shiny.Never, FlawlessIVCount = 3 }, // Moltres
        };

        private static readonly EncounterStatic[] Encounter_AO_Regular =
        {
            // Starters @ Route 101
            new EncounterStatic { Gift = true, Species = 252, Level = 5, Location = 204, }, // Treeko
            new EncounterStatic { Gift = true, Species = 255, Level = 5, Location = 204, }, // Torchic
            new EncounterStatic { Gift = true, Species = 258, Level = 5, Location = 204, }, // Mudkip

            new EncounterStatic { Gift = true, Species = 152, Level = 5, Location = 204, }, // Chikorita
            new EncounterStatic { Gift = true, Species = 155, Level = 5, Location = 204, }, // Cyndaquil
            new EncounterStatic { Gift = true, Species = 158, Level = 5, Location = 204, }, // Totodile

            new EncounterStatic { Gift = true, Species = 387, Level = 5, Location = 204, }, // Turtwig
            new EncounterStatic { Gift = true, Species = 390, Level = 5, Location = 204, }, // Chimchar
            new EncounterStatic { Gift = true, Species = 393, Level = 5, Location = 204, }, // Piplup

            new EncounterStatic { Gift = true, Species = 495, Level = 5, Location = 204, }, // Snivy
            new EncounterStatic { Gift = true, Species = 498, Level = 5, Location = 204, }, // Tepig
            new EncounterStatic { Gift = true, Species = 501, Level = 5, Location = 204, }, // Oshawott

            // Fossils @ Rustboro City
            new EncounterStatic { Gift = true, Species = 138, Level = 20, Location = 190, }, // Omanyte
            new EncounterStatic { Gift = true, Species = 140, Level = 20, Location = 190, }, // Kabuto
            new EncounterStatic { Gift = true, Species = 142, Level = 20, Location = 190, }, // Aerodactyl
            new EncounterStatic { Gift = true, Species = 345, Level = 20, Location = 190, }, // Lileep
            new EncounterStatic { Gift = true, Species = 347, Level = 20, Location = 190, }, // Anorith
            new EncounterStatic { Gift = true, Species = 408, Level = 20, Location = 190, }, // Cranidos
            new EncounterStatic { Gift = true, Species = 410, Level = 20, Location = 190, }, // Shieldon
            new EncounterStatic { Gift = true, Species = 564, Level = 20, Location = 190, }, // Tirtouga
            new EncounterStatic { Gift = true, Species = 566, Level = 20, Location = 190, }, // Archen
            new EncounterStatic { Gift = true, Species = 696, Level = 20, Location = 190, }, // Tyrunt
            new EncounterStatic { Gift = true, Species = 698, Level = 20, Location = 190, }, // Amaura

            // Hot Springs Eggs
            new EncounterStatic { Species = 360, Level = 1, EggLocation = 60004, Ability = 1, Gift = true, EggCycles = 70 }, // Wynaut
            new EncounterStatic { Species = 175, Level = 1, EggLocation = 60004, Ability = 1, Gift = true, EggCycles = 70 }, // Togepi

            // Gift
            new EncounterStatic { Species = 374, Level = 01, Location = 196, Ability = 1, IVs = new[] {-1,-1,31,-1,-1,31}, Gift = true }, // Beldum
            new EncounterStatic { Species = 351, Level = 30, Location = 240, Ability = 1, IVs = new[] {-1,-1,-1,-1,31,-1}, Contest = new[] {0,100,0,0,0,0}, Gender = 1, Nature = Nature.Lax, Gift = true }, // Castform
            new EncounterStatic { Species = 319, Level = 40, Location = 318, Ability = 1, Gender = 1, Nature = Nature.Adamant, Gift = true }, // Sharpedo
            new EncounterStatic { Species = 323, Level = 40, Location = 318, Ability = 1, Gender = 1, Nature = Nature.Quiet, Gift = true }, // Camerupt
            new EncounterStatic { Species = 380, Level = 30, Location = 320, Ability = 1, Version = GameVersion.AS, Gift = true, FlawlessIVCount = 3 }, // Latias
            new EncounterStatic { Species = 381, Level = 30, Location = 320, Ability = 1, Version = GameVersion.OR, Gift = true, FlawlessIVCount = 3 }, // Latios

            // Stationary Legendary
            new EncounterStatic { Species = 377, Level = 40, Location = 278, FlawlessIVCount = 3 }, // Regirock
            new EncounterStatic { Species = 378, Level = 40, Location = 306, FlawlessIVCount = 3 }, // Regice
            new EncounterStatic { Species = 379, Level = 40, Location = 308, FlawlessIVCount = 3 }, // Registeel
            new EncounterStatic { Species = 486, Level = 50, Location = 306, FlawlessIVCount = 3 }, // Regigigas
            new EncounterStatic { Species = 382, Level = 45, Location = 296, Version = GameVersion.AS, Shiny = Shiny.Never, FlawlessIVCount = 3 }, // Kyogre
            new EncounterStatic { Species = 383, Level = 45, Location = 296, Version = GameVersion.OR, Shiny = Shiny.Never, FlawlessIVCount = 3 }, // Groudon
            new EncounterStatic { Species = 384, Level = 70, Location = 316, Shiny = Shiny.Never, FlawlessIVCount = 3 }, // Rayquaza
            new EncounterStatic { Species = 386, Level = 80, Location = 316, Shiny = Shiny.Never, FlawlessIVCount = 3, Fateful = true }, // Deoxys

            // Hoopa Rings
            new EncounterStatic { Species = 249, Level = 50, Location = 304, Version = GameVersion.AS, FlawlessIVCount = 3 }, // Lugia
            new EncounterStatic { Species = 250, Level = 50, Location = 304, Version = GameVersion.OR, FlawlessIVCount = 3 }, // Ho-Oh
            new EncounterStatic { Species = 483, Level = 50, Location = 348, Version = GameVersion.AS, FlawlessIVCount = 3 }, // Dialga
            new EncounterStatic { Species = 484, Level = 50, Location = 348, Version = GameVersion.OR, FlawlessIVCount = 3 }, // Palkia
            new EncounterStatic { Species = 644, Level = 50, Location = 340, Version = GameVersion.AS, FlawlessIVCount = 3 }, // Zekrom
            new EncounterStatic { Species = 643, Level = 50, Location = 340, Version = GameVersion.OR, FlawlessIVCount = 3 }, // Reshiram
            new EncounterStatic { Species = 642, Level = 50, Location = 348, Version = GameVersion.AS, FlawlessIVCount = 3 }, // Thundurus
            new EncounterStatic { Species = 641, Level = 50, Location = 348, Version = GameVersion.OR, FlawlessIVCount = 3 }, // Tornadus
            new EncounterStatic { Species = 243, Level = 50, Location = 334, FlawlessIVCount = 3 }, // Raikou
            new EncounterStatic { Species = 244, Level = 50, Location = 334, FlawlessIVCount = 3 }, // Entei
            new EncounterStatic { Species = 245, Level = 50, Location = 334, FlawlessIVCount = 3 }, // Suicune
            new EncounterStatic { Species = 480, Level = 50, Location = 338, FlawlessIVCount = 3 }, // Uxie
            new EncounterStatic { Species = 481, Level = 50, Location = 338, FlawlessIVCount = 3 }, // Mesprit
            new EncounterStatic { Species = 482, Level = 50, Location = 338, FlawlessIVCount = 3 }, // Azelf
            new EncounterStatic { Species = 485, Level = 50, Location = 312, FlawlessIVCount = 3 }, // Heatran
            new EncounterStatic { Species = 487, Level = 50, Location = 348, FlawlessIVCount = 3 }, // Giratina
            new EncounterStatic { Species = 488, Level = 50, Location = 344, FlawlessIVCount = 3 }, // Cresselia
            new EncounterStatic { Species = 638, Level = 50, Location = 336, FlawlessIVCount = 3 }, // Cobalion
            new EncounterStatic { Species = 639, Level = 50, Location = 336, FlawlessIVCount = 3 }, // Terrakion
            new EncounterStatic { Species = 640, Level = 50, Location = 336, FlawlessIVCount = 3 }, // Virizion
            new EncounterStatic { Species = 645, Level = 50, Location = 348, FlawlessIVCount = 3 }, // Landorus
            new EncounterStatic { Species = 646, Level = 50, Location = 342, FlawlessIVCount = 3 }, // Kyurem

            // Devon Scope Kecleon
            new EncounterStatic { Species = 352, Level = 30, Location = 240 }, // Kecleon @ Route 119
            new EncounterStatic { Species = 352, Level = 30, Location = 242 }, // Kecleon @ Route 120
            new EncounterStatic { Species = 352, Level = 40, Location = 176, Gender = 1, }, // Kecleon @ Lavaridge
            new EncounterStatic { Species = 352, Level = 45, Location = 196, Ability = 4, }, // Kecleon @ Mossdeep City

            // Eon Ticket Lati@s
            new EncounterStatic { Species = 381, Level = 30, Location = 320, Version = GameVersion.AS, FlawlessIVCount = 3 }, // Latios
            new EncounterStatic { Species = 380, Level = 30, Location = 320, Version = GameVersion.OR, FlawlessIVCount = 3 }, // Latias

            // Stationary
            new EncounterStatic { Species = 101, Level = 40, Location = 292, Version = GameVersion.AS }, // Electrode
            new EncounterStatic { Species = 101, Level = 40, Location = 314, Version = GameVersion.OR }, // Electrode
            new EncounterStatic { Species = 100, Level = 20, Location = 302 }, // Voltorb @ Route 119
            new EncounterStatic { Species = 442, Level = 50, Location = 304 }, // Spiritomb @ Route 120

            // Soaring in the Sky
            new EncounterStatic { Species = 198, Level = 45, Location = 348 }, // Murkrow
            new EncounterStatic { Species = 276, Level = 40, Location = 348 }, // Taillow
            new EncounterStatic { Species = 278, Level = 40, Location = 348 }, // Wingull
            new EncounterStatic { Species = 279, Level = 40, Location = 348 }, // Pelipper
            new EncounterStatic { Species = 333, Level = 40, Location = 348 }, // Swablu
            new EncounterStatic { Species = 425, Level = 45, Location = 348 }, // Drifloon
            new EncounterStatic { Species = 628, Level = 45, Location = 348 }, // Braviary
        };

        private static readonly EncounterStatic[] Encounter_AO = ArrayUtil.ConcatAll(Encounter_AO_Regular, PermuteCosplayPikachu().ToArray());

        private static IEnumerable<EncounterStatic> PermuteCosplayPikachu()
        {
            var CosplayPikachu = new EncounterStatic
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
    }
}
