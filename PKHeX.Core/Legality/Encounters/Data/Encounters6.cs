using static PKHeX.Core.EncounterUtil;

namespace PKHeX.Core
{
    internal static class Encounters6
    {
        internal static readonly EncounterArea[] SlotsX, SlotsY, SlotsA, SlotsO;
        internal static readonly EncounterStatic[] StaticX, StaticY, StaticA, StaticO;

        static Encounters6()
        {
            StaticX = GetStaticEncounters(Encounter_XY, GameVersion.X);
            StaticY = GetStaticEncounters(Encounter_XY, GameVersion.Y);
            StaticA = GetStaticEncounters(Encounter_AO, GameVersion.AS);
            StaticO = GetStaticEncounters(Encounter_AO, GameVersion.OR);

            var XSlots = GetEncounterTables(GameVersion.X);
            var YSlots = GetEncounterTables(GameVersion.Y);
            MarkG6XYSlots(ref XSlots);
            MarkG6XYSlots(ref YSlots);
            SlotsX = AddExtraTableSlots(XSlots, SlotsXYAlt);
            SlotsY = AddExtraTableSlots(YSlots, SlotsXYAlt);

            SlotsA = GetEncounterTables(GameVersion.AS);
            SlotsO = GetEncounterTables(GameVersion.OR);
            MarkG6AOSlots(ref SlotsA);
            MarkG6AOSlots(ref SlotsO);
        }
        private static void MarkG6XYSlots(ref EncounterArea[] Areas)
        {
            foreach (var area in Areas)
            {
                int slotct = area.Slots.Length;
                for (int i = slotct - 15; i < slotct; i++)
                    area.Slots[i].Type = SlotType.Horde;
            }
            ReduceAreasSize(ref Areas);
        }
        private static void MarkG6AOSlots(ref EncounterArea[] Areas)
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

        internal static readonly string[][] TradeXY =
        {
            new string[0],                       // 0 - None
            Util.GetStringList("tradexy", "ja"), // 1
            Util.GetStringList("tradexy", "en"), // 2
            Util.GetStringList("tradexy", "fr"), // 3
            Util.GetStringList("tradexy", "it"), // 4
            Util.GetStringList("tradexy", "de"), // 5
            new string[0],                       // 6 - None
            Util.GetStringList("tradexy", "es"), // 7
            Util.GetStringList("tradexy", "ko"), // 8
        };
        internal static readonly string[][] TradeAO =
        {
            new string[0],                       // 0 - None
            Util.GetStringList("tradeao", "ja"), // 1
            Util.GetStringList("tradeao", "en"), // 2
            Util.GetStringList("tradeao", "fr"), // 3
            Util.GetStringList("tradeao", "it"), // 4
            Util.GetStringList("tradeao", "de"), // 5
            new string[0],                       // 6 - None
            Util.GetStringList("tradeao", "es"), // 7
            Util.GetStringList("tradeao", "ko"), // 8
        };

        #region XY Alt Slots
        private static readonly EncounterArea[] SlotsXYAlt =
        {
            new EncounterArea {
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
            new EncounterArea {
                Location = 34, // Route 6
                Slots = new[]
                {
                    // Rustling Bush
                    new EncounterSlot { Species = 543, LevelMin = 10, LevelMax = 12, Form = 0 }, // Venipede
                    new EncounterSlot { Species = 531, LevelMin = 10, LevelMax = 12, Form = 0 }, // Audino
                },},

            new EncounterArea { Location = 38, // Route 7
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

            new EncounterArea { Location = 88, // Route 18
                Slots = new[]
                {
                    // Rustling Bush
                    new EncounterSlot { Species = 632, LevelMin = 44, LevelMax = 46, Form = 0 }, // Durant
                    new EncounterSlot { Species = 631, LevelMin = 45, LevelMax = 45, Form = 0 }, // Heatmor
                },},

            new EncounterArea { Location = 132, // Glittering Cave
                Slots = new[]
                {
                    // Drops
                    new EncounterSlot { Species = 527, LevelMin = 15, LevelMax = 17, Form = 0 }, // Woobat
                    new EncounterSlot { Species = 597, LevelMin = 15, LevelMax = 17, Form = 0 }, // Ferroseed
                },},

            new EncounterArea { Location = 56, // Reflection Cave
                Slots = new[]
                {
                    // Drops
                    new EncounterSlot { Species = 527, LevelMin = 21, LevelMax = 23, Form = 0 }, // Woobat
                    new EncounterSlot { Species = 597, LevelMin = 21, LevelMax = 23, Form = 0 }, // Ferroseed
                },},

            new EncounterArea { Location = 140, // Terminus Cave
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
            new EncounterStatic { Gift = true, Species = 650, Level = 5, Location = 10, }, // Chespin
            new EncounterStatic { Gift = true, Species = 653, Level = 5, Location = 10, }, // Fennekin
            new EncounterStatic { Gift = true, Species = 656, Level = 5, Location = 10, }, // Froakie

            new EncounterStatic { Gift = true, Species = 1, Level = 10, Location = 22, }, // Bulbasaur
            new EncounterStatic { Gift = true, Species = 4, Level = 10, Location = 22, }, // Charmander
            new EncounterStatic { Gift = true, Species = 7, Level = 10, Location = 22, }, // Squirtle

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

            new EncounterStatic { Species = 448, Level = 32, Location = 60, Ability = 1, Nature = Nature.Hasty, Gender = 0, IVs = new[] {6, 25, 16, 31, 25, 19}, Gift = true, Shiny = false }, // Lucario
            new EncounterStatic { Species = 131, Level = 30, Location = 62, Nature = Nature.Docile, IVs = new[] {31, 20, 20, 20, 20, 20}, Gift = true }, // Lapras
            
            new EncounterStatic { Species = 143, Level = 15, Location = 038, Shiny = false, }, // Snorlax
            new EncounterStatic { Species = 568, Level = 35, Location = 142 }, // Trubbish
            new EncounterStatic { Species = 569, Level = 36, Location = 142 }, // Garbodor
            new EncounterStatic { Species = 569, Level = 37, Location = 142 }, // Garbodor
            new EncounterStatic { Species = 569, Level = 38, Location = 142 }, // Garbodor
            new EncounterStatic { Species = 479, Level = 38, Location = 142 }, // Rotom

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
            
            new EncounterStatic { Species = 716, Level = 50, Location = 138, Ability = 1, Version = GameVersion.X, Shiny = false, IV3 = true }, // Xerneas
            new EncounterStatic { Species = 717, Level = 50, Location = 138, Ability = 1, Version = GameVersion.Y, Shiny = false, IV3 = true }, // Yveltal
            new EncounterStatic { Species = 718, Level = 70, Location = 140, Ability = 1, Shiny = false, IV3 = true }, // Zygarde
            
            new EncounterStatic { Species = 150, Level = 70, Location = 168, Ability = 1, Shiny = false, IV3 = true }, // Mewtwo

            new EncounterStatic { Species = 144, Level = 70, Location = 146, Ability = 1, Shiny = false, IV3 = true }, // Articuno
            new EncounterStatic { Species = 145, Level = 70, Location = 146, Ability = 1, Shiny = false, IV3 = true }, // Zapdos
            new EncounterStatic { Species = 146, Level = 70, Location = 146, Ability = 1, Shiny = false, IV3 = true }, // Moltres
        };
        private static readonly EncounterStatic[] Encounter_AO =
        {
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
            
            // Fossil
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

            new EncounterStatic { Species = 25, Level = 20, Location = 178, Gender = 1, Ability = 4, IVs = new[] {-1, -1, -1, 31, -1, -1}, Contest = new[] {70,70,70,70,70,0}, Gift = true, Shiny = false, SkipFormCheck = true }, // Pikachu
            new EncounterStatic { Species = 25, Level = 20, Location = 180, Gender = 1, Ability = 4, IVs = new[] {-1, -1, -1, 31, -1, -1}, Contest = new[] {70,70,70,70,70,0}, Gift = true, Shiny = false, SkipFormCheck = true }, // Pikachu
            new EncounterStatic { Species = 25, Level = 20, Location = 186, Gender = 1, Ability = 4, IVs = new[] {-1, -1, -1, 31, -1, -1}, Contest = new[] {70,70,70,70,70,0}, Gift = true, Shiny = false, SkipFormCheck = true }, // Pikachu
            new EncounterStatic { Species = 25, Level = 20, Location = 194, Gender = 1, Ability = 4, IVs = new[] {-1, -1, -1, 31, -1, -1}, Contest = new[] {70,70,70,70,70,0}, Gift = true, Shiny = false, SkipFormCheck = true }, // Pikachu

            new EncounterStatic { Species = 360, Level = 1, EggLocation = 60004, Ability = 1, Gift = true, EggCycles = 70 }, // Wynaut
            new EncounterStatic { Species = 175, Level = 1, EggLocation = 60004, Ability = 1, Gift = true, EggCycles = 70 }, // Togepi
            new EncounterStatic { Species = 374, Level = 1, Location = 196, Ability = 1, IVs = new[] {-1, -1, 31, -1, -1, 31}, Gift = true }, // Beldum

            new EncounterStatic { Species = 351, Level = 30, Location = 240, Gender = 1, Ability = 1, Nature = Nature.Lax, IVs = new[] {-1, -1, -1, -1, 31, -1}, Contest = new[] {0,100,0,0,0,0}, Gift = true }, // Castform
            new EncounterStatic { Species = 319, Level = 40, Location = 318, Gender = 1, Ability = 1, Nature = Nature.Adamant, Gift = true }, // Sharpedo
            new EncounterStatic { Species = 323, Level = 40, Location = 318, Gender = 1, Ability = 1, Nature = Nature.Quiet, Gift = true }, // Camerupt
            
            new EncounterStatic { Species = 380, Level = 30, Location = 320, Version = GameVersion.AS, Ability = 1, Gift = true, IV3 = true }, // Latias
            new EncounterStatic { Species = 381, Level = 30, Location = 320, Version = GameVersion.OR, Ability = 1, Gift = true, IV3 = true }, // Latios

            new EncounterStatic { Species = 382, Level = 45, Location = 296, Version = GameVersion.AS, Shiny = false, IV3 = true }, // Kyogre
            new EncounterStatic { Species = 383, Level = 45, Location = 296, Version = GameVersion.OR, Shiny = false, IV3 = true }, // Groudon
            new EncounterStatic { Species = 384, Level = 70, Location = 316, Shiny = false, IV3 = true }, // Rayquaza
            new EncounterStatic { Species = 386, Level = 80, Location = 316, Shiny = false, IV3 = true, Fateful = true }, // Deoxys

            new EncounterStatic { Species = 377, Level = 40, Location = 278, IV3 = true }, // Regirock
            new EncounterStatic { Species = 378, Level = 40, Location = 306, IV3 = true }, // Regice
            new EncounterStatic { Species = 379, Level = 40, Location = 308, IV3 = true }, // Registeel
            new EncounterStatic { Species = 486, Level = 50, Location = 306, IV3 = true }, // Regigigas
            
            new EncounterStatic { Species = 249, Level = 50, Location = 304, Version = GameVersion.AS, IV3 = true }, // Lugia
            new EncounterStatic { Species = 250, Level = 50, Location = 304, Version = GameVersion.OR, IV3 = true }, // Ho-oh

            new EncounterStatic { Species = 483, Level = 50, Location = 348, Version = GameVersion.AS, IV3 = true }, // Dialga
            new EncounterStatic { Species = 484, Level = 50, Location = 348, Version = GameVersion.OR, IV3 = true }, // Palkia

            new EncounterStatic { Species = 644, Level = 50, Location = 340, Version = GameVersion.AS, IV3 = true }, // Zekrom
            new EncounterStatic { Species = 643, Level = 50, Location = 340, Version = GameVersion.OR, IV3 = true }, // Reshiram

            new EncounterStatic { Species = 642, Level = 50, Location = 348, Version = GameVersion.AS, IV3 = true }, // Thundurus
            new EncounterStatic { Species = 641, Level = 50, Location = 348, Version = GameVersion.OR, IV3 = true }, // Tornadus

            new EncounterStatic { Species = 485, Level = 50, Location = 312, IV3 = true }, // Heatran
            new EncounterStatic { Species = 487, Level = 50, Location = 348, IV3 = true }, // Giratina
            new EncounterStatic { Species = 488, Level = 50, Location = 344, IV3 = true }, // Cresselia
            new EncounterStatic { Species = 645, Level = 50, Location = 348, IV3 = true }, // Landorus
            new EncounterStatic { Species = 646, Level = 50, Location = 342, IV3 = true }, // Kyurem
            
            new EncounterStatic { Species = 243, Level = 50, Location = 334, IV3 = true }, // Raikou
            new EncounterStatic { Species = 244, Level = 50, Location = 334, IV3 = true }, // Entei
            new EncounterStatic { Species = 245, Level = 50, Location = 334, IV3 = true }, // Suicune

            new EncounterStatic { Species = 480, Level = 50, Location = 338, IV3 = true }, // Uxie
            new EncounterStatic { Species = 481, Level = 50, Location = 338, IV3 = true }, // Mesprit
            new EncounterStatic { Species = 482, Level = 50, Location = 338, IV3 = true }, // Azelf

            new EncounterStatic { Species = 638, Level = 50, Location = 336, IV3 = true }, // Cobalion
            new EncounterStatic { Species = 639, Level = 50, Location = 336, IV3 = true }, // Terrakion
            new EncounterStatic { Species = 640, Level = 50, Location = 336, IV3 = true }, // Virizion
            
            new EncounterStatic { Species = 352, Level = 30, Location = 240 }, // Kecleon @ Route 119
            new EncounterStatic { Species = 352, Level = 30, Location = 242 }, // Kecleon @ Route 120
            new EncounterStatic { Species = 352, Level = 40, Location = 176, Gender = 1, }, // Kecleon @ Lavaridge
            new EncounterStatic { Species = 352, Level = 45, Location = 196, Ability = 4, }, // Kecleon @ Mossdeep City

            new EncounterStatic { Species = 381, Level = 30, Location = 320, Version = GameVersion.AS, IV3 = true }, // Latios
            new EncounterStatic { Species = 380, Level = 30, Location = 320, Version = GameVersion.OR, IV3 = true }, // Latias
            
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
        #endregion
        #region Trade Tables
        internal static readonly EncounterTrade[] TradeGift_XY =
        {
            new EncounterTrade { Species = 129, Level = 5, Ability = 1, Gender = 0, TID = 44285, Nature = Nature.Adamant, }, // Magikarp
            new EncounterTrade { Species = 133, Level = 5, Ability = 1, Gender = 1, TID = 29294, Nature = Nature.Docile, }, // Eevee

            new EncounterTrade { Species = 83, Level = 10, Ability = 1, Gender = 0, TID = 00185, Nature = Nature.Jolly, IVs = new[] {-1, -1, -1, 31, -1, -1}, }, // Farfetch'd
            new EncounterTrade { Species = 208, Level = 20, Ability = 1, Gender = 1, TID = 19250, Nature = Nature.Impish, IVs = new[] {-1, -1, 31, -1, -1, -1}, }, // Steelix
            new EncounterTrade { Species = 625, Level = 50, Ability = 1, Gender = 0, TID = 03447, Nature = Nature.Adamant, IVs = new[] {-1, 31, -1, -1, -1, -1}, }, // Bisharp

            new EncounterTrade { Species = 656, Level = 5, Ability = 1, Gender = 0, TID = 00037, Nature = Nature.Jolly, IVs = new[] {20, 20, 20, 31, 20, 20}, }, // Froakie
            new EncounterTrade { Species = 650, Level = 5, Ability = 1, Gender = 0, TID = 00037, Nature = Nature.Adamant, IVs = new[] {20, 31, 20, 20, 20, 20}, }, // Chespin
            new EncounterTrade { Species = 653, Level = 5, Ability = 1, Gender = 0, TID = 00037, Nature = Nature.Modest, IVs = new[] {20, 20, 20, 20, 31, 20}, }, // Fennekin

            new EncounterTrade { Species = 280, Level = 5, Ability = 1, Gender = 1, TID = 37110, Nature = Nature.Modest, IVs = new[] {20, 20, 20, 31, 31, 20}, }, // Ralts
        };
        internal static readonly EncounterTrade[] TradeGift_AO =
        {
            new EncounterTrade { Species = 296, Level = 9, Ability = 2, Gender = 0, TID = 30724, Nature = Nature.Brave, IVs = new[] {-1, 31, -1, -1, -1, -1}, }, // Makuhita
            new EncounterTrade { Species = 300, Level = 30, Ability = 1, Gender = 1, TID = 03239, Nature = Nature.Naughty, IVs = new[] {-1, -1, -1, 31, -1, -1}, }, // Skitty
            new EncounterTrade { Species = 222, Level = 50, Ability = 4, Gender = 1, TID = 00325, Nature = Nature.Calm, IVs = new[] {31, -1, -1, -1, -1, 31}, }, // Corsola
        };
        #endregion
        #region Pokémon Link Gifts

        internal static readonly EncounterLink[] LinkGifts6 =
        {
            new EncounterLink { Species = 154, Level = 50, Ability = 4, XY = true, ORAS = true }, // Meganium
            new EncounterLink { Species = 157, Level = 50, Ability = 4, XY = true, ORAS = true }, // Typhlosion
            new EncounterLink { Species = 160, Level = 50, Ability = 4, XY = true, ORAS = true, Moves = new [] {8} }, // Feraligatr with Ice Punch (not relearn)

            new EncounterLink { Species = 251, Level = 10, Ability = 1, RelearnMoves = new[] {610, 0, 0, 0}, Ball = 11, XY = true }, // Celebi

            new EncounterLink { Species = 377, Level = 50, Ability = 4, RelearnMoves = new[] {153, 8, 444, 359}, XY = true, ORAS = true }, // Regirock
            new EncounterLink { Species = 378, Level = 50, Ability = 4, RelearnMoves = new[] {85, 133, 58, 258}, XY = true, ORAS = true }, // Regice
            new EncounterLink { Species = 379, Level = 50, Ability = 4, RelearnMoves = new[] {442, 157, 356, 334}, XY = true, ORAS = true }, // Registeel

            new EncounterLink { Species = 208, Level = 40, Ability = 1, RibbonClassic = false, ORAS = true, OT = false }, // Steelix
            new EncounterLink { Species = 362, Level = 40, Ability = 1, RibbonClassic = false, ORAS = true, OT = false }, // Glalie
        };
        #endregion
    }
}
