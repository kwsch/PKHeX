using System.Linq;

namespace PKHeX.Core
{
    public static partial class Legal
    {
        internal const int MaxSpeciesID_1 = 151;
        internal const int MaxMoveID_1 = 165;
        internal const int MaxItemID_1 = 255; 
        internal const int MaxAbilityID_1 = 0;
        
        internal static readonly ushort[] Pouch_Items_RBY = Enumerable.Range(0, 7)     // 0-6
           .Concat(Enumerable.Range(10, 11))  // 10-20
           .Concat(Enumerable.Range(29, 15))  // 29-43
           .Concat(Enumerable.Range(45, 5))   // 45-49
           .Concat(Enumerable.Range(51, 8))   // 51-58
           .Concat(Enumerable.Range(60, 24))  // 60-83
           .Concat(Enumerable.Range(196, 55)) // 196-250
           .Select(i => (ushort)i).ToArray();

        internal static readonly int[] MovePP_RBY =
        {
            0,
            35, 25, 10, 15, 20, 20, 15, 15, 15, 35, 30, 05, 10, 30, 30, 35, 35, 20, 15, 20, 20, 10, 20, 30, 05, 25, 15, 15, 15, 25, 20, 05, 35, 15, 20, 20, 20, 15, 30, 35, 20, 20, 30, 25, 40, 20, 15, 20, 20, 20,
            30, 25, 15, 30, 25, 05, 15, 10, 05, 20, 20, 20, 05, 35, 20, 25, 20, 20, 20, 15, 20, 10, 10, 40, 25, 10, 35, 30, 15, 20, 40, 10, 15, 30, 15, 20, 10, 15, 10, 05, 10, 10, 25, 10, 20, 40, 30, 30, 20, 20,
            15, 10, 40, 15, 20, 30, 20, 20, 10, 40, 40, 30, 30, 30, 20, 30, 10, 10, 20, 05, 10, 30, 20, 20, 20, 05, 15, 10, 20, 15, 15, 35, 20, 15, 10, 20, 30, 15, 40, 20, 15, 10, 05, 10, 30, 10, 15, 20, 15, 40,
            40, 10, 05, 15, 10, 10, 10, 15, 30, 30, 10, 10, 20, 10, 10, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00,
            00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00,
            00, 00, 00, 00, 00, 00
        };

        internal static readonly int[] TransferSpeciesDefaultAbility_1 = {92, 93, 94, 109, 110, 151};

        internal static readonly int[] TMHM_RBY =
        {
            005, 013, 014, 018, 025, 092, 032, 034, 036, 038,
            061, 055, 058, 059, 063, 006, 066, 068, 069, 099,
            072, 076, 082, 085, 087, 089, 090, 091, 094, 100,
            102, 104, 115, 117, 118, 120, 121, 126, 129, 130,
            135, 138, 143, 156, 086, 149, 153, 157, 161, 164,

            015, 019, 057, 070, 148
        };
        internal static readonly int[] WildPokeBalls1 = {4};

        internal static readonly EncounterStatic[] Encounter_RBY =
        {
            new EncounterStatic { Species = 001, Level = 05, Version = GameVersion.RBY }, // Bulbasaur
            new EncounterStatic { Species = 004, Level = 05, Version = GameVersion.RBY }, // Charmander
            new EncounterStatic { Species = 007, Level = 05, Version = GameVersion.RBY }, // Squirtle
            new EncounterStatic { Species = 025, Level = 05, Version = GameVersion.YW }, // Pikachu
            
            // Game Corner
            new EncounterStatic { Species = 030, Level = 17, Version = GameVersion.RBY }, // Nidorina (Red Game Corner)
            new EncounterStatic { Species = 033, Level = 17, Version = GameVersion.RBY }, // Nidorino (Blue[EN] / Green[JP] Game Corner)
            new EncounterStatic { Species = 035, Level = 08, Version = GameVersion.RBY }, // Clefairy (Red Game Corner)
            new EncounterStatic { Species = 036, Level = 24, Version = GameVersion.RBY }, // Clefable (Blue[JP] Game Corner)
            new EncounterStatic { Species = 037, Level = 18, Version = GameVersion.RBY }, // Vulpix (Yellow Game Corner)
            new EncounterStatic { Species = 040, Level = 22, Version = GameVersion.RBY }, // Wigglytuff (Yellow Game Corner)
            new EncounterStatic { Species = 063, Level = 06, Version = GameVersion.RBY }, // Abra (Blue[EN] / Green[JP] Game Corner)
            new EncounterStatic { Species = 116, Level = 18, Version = GameVersion.RBY }, // Horsea (Blue[JP] Game Corner)
            new EncounterStatic { Species = 123, Level = 25, Version = GameVersion.RBY }, // Scyther (Red Game Corner)
            new EncounterStatic { Species = 127, Level = 20, Version = GameVersion.RBY }, // Pinsir (Blue[EN] / Green[JP] Game Corner)
            new EncounterStatic { Species = 137, Level = 18, Version = GameVersion.RBY }, // Porygon (Blue[EN] / Green[JP] Game Corner)
            new EncounterStatic { Species = 147, Level = 18, Version = GameVersion.RBY }, // Dratini (Red Game Corner)
            new EncounterStatic { Species = 148, Level = 30, Version = GameVersion.RBY }, // Dragonair (Blue[JP] Game Corner)
           
            // Lower level less ideal matches; best match is from above.
         // new EncounterStatic { Species = 025, Level = 12 }, // Pikachu (Blue[JP] Game Corner) 
         // new EncounterStatic { Species = 035, Level = 12 }, // Clefairy (Blue[EN] / Green[JP] Game Corner)
         // new EncounterStatic { Species = 063, Level = 09 }, // Abra (Red Game Corner)
         // new EncounterStatic { Species = 063, Level = 08 }, // Abra (Blue[JP] Game Corner)
         // new EncounterStatic { Species = 063, Level = 15 }, // Abra (Yellow Game Corner)
         // new EncounterStatic { Species = 123, Level = 30 }, // Scyther (Yellow Game Corner)
         // new EncounterStatic { Species = 127, Level = 30 }, // Pinsir (Yellow Game Corner)
         // new EncounterStatic { Species = 137, Level = 22 }, // Porygon (Blue[JP] Game Corner)
         // new EncounterStatic { Species = 137, Level = 26 }, // Porygon (Red Game Corner)
         // new EncounterStatic { Species = 137, Level = 26 }, // Porygon (Yellow Game Corner)
         // new EncounterStatic { Species = 147, Level = 24 }, // Dratini (Blue[EN] / Green[JP] Game Corner)

            new EncounterStatic { Species = 129, Level = 05, Version = GameVersion.RBY }, // Magikarp
            new EncounterStatic { Species = 143, Level = 30, Version = GameVersion.RBY }, // Snorlax
            new EncounterStatic { Species = 106, Level = 30, Version = GameVersion.RBY }, // Hitmonlee
            new EncounterStatic { Species = 107, Level = 30, Version = GameVersion.RBY }, // Hitmonchan

            new EncounterStatic { Species = 131, Level = 15, Version = GameVersion.RBY }, // Lapras
            new EncounterStatic { Species = 138, Level = 30, Version = GameVersion.RBY }, // Omanyte
            new EncounterStatic { Species = 140, Level = 30, Version = GameVersion.RBY }, // Kabuto
            new EncounterStatic { Species = 142, Level = 30, Version = GameVersion.RBY }, // Aerodactyl
            
            new EncounterStatic { Species = 144, Level = 50, Version = GameVersion.RBY }, // Articuno
            new EncounterStatic { Species = 145, Level = 50, Version = GameVersion.RBY }, // Zapdos
            new EncounterStatic { Species = 146, Level = 50, Version = GameVersion.RBY }, // Moltres

            new EncounterStatic { Species = 150, Level = 70, Version = GameVersion.RBY }, // Mewtwo
            
            new EncounterStatic { Species = 133, Level = 25, Version = GameVersion.RBY }, // Eevee

            // Yellow Only -- duplicate encounters with a higher level
         // new EncounterStatic { Species = 133, Level = 25, Version = GameVersion.YW }, // Eevee (Celadon City)
         // new EncounterStatic { Species = 001, Level = 10, Version = GameVersion.YW }, // Bulbasaur (Cerulean City)
         // new EncounterStatic { Species = 004, Level = 10, Version = GameVersion.YW }, // Charmander (Route 24)
         // new EncounterStatic { Species = 007, Level = 10, Version = GameVersion.YW }, // Squirtle (Vermillion City)

            new EncounterStatic { Species = 054, Level = 15, Moves = new [] { 133, 10 }, Version = GameVersion.SPECIAL }, // Stadium Psyduck (Amnesia)
            new EncounterStatic { Species = 151, Level = 5, IVs = new [] {15,15,15,15,15,15}, Version = GameVersion.SPECIAL }, // Event Mew
        };
        internal static readonly EncounterTrade[] TradeGift_RBY =
        {
        };
        internal static readonly EncounterArea FishOldGood_RBY = new EncounterArea { Location = -1, Slots = new EncounterSlot[]
        {
            new EncounterSlot1 {Species = 129, LevelMin = 05, LevelMax = 05, Type = SlotType.Old_Rod,  Rate = -1, }, // Magikarp
            new EncounterSlot1 {Species = 118, LevelMin = 10, LevelMax = 10, Type = SlotType.Good_Rod, Rate = -1, }, // Goldeen
            new EncounterSlot1 {Species = 060, LevelMin = 10, LevelMax = 10, Type = SlotType.Good_Rod, Rate = -1, }, // Poliwag
        }};
    }
}
