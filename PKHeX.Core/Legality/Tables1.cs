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

        internal static readonly int[] G1CaterpieMoves = { 33, 81 };
        internal static readonly int[] G1WeedleMoves = { 40, 81 };
        internal static readonly int[] G1MetapodMoves = G1CaterpieMoves.Concat(new[] { 106 }).ToArray();
        internal static readonly int[] G1KakunaMoves = G1WeedleMoves.Concat(new[] { 106 }).ToArray();
        internal static readonly int[] G1Exeggcute_IncompatibleMoves = { 78, 77, 79 };

        internal static readonly int[] WildPokeBalls1 = {4};

        internal static readonly EncounterStatic[] Encounter_RBY =
        {
            // Gameversion is RBY for pokemon with the same catch rate and initial moves in all games
            // If there are differents in moves or catch rate they will have different encounters defined
            new EncounterStatic { Species = 001, Level = 05, Version = GameVersion.RBY }, // Bulbasaur
            new EncounterStatic { Species = 004, Level = 05, Version = GameVersion.RBY }, // Charmander
            new EncounterStatic { Species = 007, Level = 05, Version = GameVersion.RBY }, // Squirtle
            new EncounterStatic { Species = 025, Level = 05, Version = GameVersion.YW }, // Pikachu
            
            // Game Corner
            new EncounterStatic { Species = 030, Level = 17, Version = GameVersion.RB }, // Nidorina (Red Game Corner)
            new EncounterStatic { Species = 033, Level = 17, Version = GameVersion.BU }, // Nidorino (Blue[EN] / Green[JP] Game Corner)
            new EncounterStatic { Species = 035, Level = 08, Version = GameVersion.RBY }, // Clefairy (Red Game Corner)
            new EncounterStatic { Species = 036, Level = 24, Version = GameVersion.RBY }, // Clefable (Blue[JP] Game Corner)
            new EncounterStatic { Species = 037, Level = 18, Version = GameVersion.RBY }, // Vulpix (Yellow Game Corner)
            new EncounterStatic { Species = 040, Level = 22, Version = GameVersion.RBY }, // Wigglytuff (Yellow Game Corner)
            new EncounterStatic { Species = 063, Level = 06, Version = GameVersion.RBY }, // Abra (Blue[EN] / Green[JP] Game Corner)
            new EncounterStatic { Species = 116, Level = 18, Version = GameVersion.RBY }, // Horsea (Blue[JP] Game Corner)
            new EncounterStatic { Species = 123, Level = 25, Version = GameVersion.RBY }, // Scyther (Red Game Corner)
            new EncounterStatic { Species = 127, Level = 20, Version = GameVersion.BU }, // Pinsir (Blue[EN] / Green[JP] Game Corner)
            new EncounterStatic { Species = 127, Level = 30, Version = GameVersion.YW }, // Pinsir (Yellow Game Corner) (Different initial moves)
            new EncounterStatic { Species = 137, Level = 18, Version = GameVersion.RBY }, // Porygon (Blue[EN] / Green[JP] Game Corner)
            new EncounterStatic { Species = 147, Level = 18, Version = GameVersion.RBY }, // Dratini (Red Game Corner)
            new EncounterStatic { Species = 148, Level = 30, Version = GameVersion.BU }, // Dragonair (Blue[JP] Game Corner)
            new EncounterStatic { Species = 025, Level = 12, Version = GameVersion.BU }, // Pikachu (Blue[JP] Game Corner) (Different catch rate)

           
            // Lower level less ideal matches; best match is from above.
         // new EncounterStatic { Species = 035, Level = 12 }, // Clefairy (Blue[EN] / Green[JP] Game Corner)
         // new EncounterStatic { Species = 063, Level = 09 }, // Abra (Red Game Corner)
         // new EncounterStatic { Species = 063, Level = 08 }, // Abra (Blue[JP] Game Corner)
         // new EncounterStatic { Species = 063, Level = 15 }, // Abra (Yellow Game Corner)
         // new EncounterStatic { Species = 123, Level = 30 }, // Scyther (Yellow Game Corner)
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
            
            new EncounterStatic { Species = 133, Level = 25, Version = GameVersion.RB }, // Eevee
            new EncounterStatic { Species = 133, Level = 25, Version = GameVersion.YW }, // Eevee (Different initial moves)

            // Yellow Only -- duplicate encounters with a higher level
         // new EncounterStatic { Species = 001, Level = 10, Version = GameVersion.YW }, // Bulbasaur (Cerulean City)
         // new EncounterStatic { Species = 004, Level = 10, Version = GameVersion.YW }, // Charmander (Route 24)
         // new EncounterStatic { Species = 007, Level = 10, Version = GameVersion.YW }, // Squirtle (Vermillion City)

            new EncounterStatic { Species = 054, Level = 15, Moves = new [] { 133, 10 }, Version = GameVersion.Stadium }, // Stadium Psyduck (Amnesia)
            new EncounterStatic { Species = 001, Level = 05, Version = GameVersion.Stadium }, // Bulbasaur
            new EncounterStatic { Species = 004, Level = 05, Version = GameVersion.Stadium }, // Charmander
            new EncounterStatic { Species = 071, Level = 05, Version = GameVersion.Stadium }, // Squirtle
            new EncounterStatic { Species = 106, Level = 20, Version = GameVersion.Stadium }, // Hitmonlee
            new EncounterStatic { Species = 107, Level = 20, Version = GameVersion.Stadium }, // Hitmonchan
            new EncounterStatic { Species = 133, Level = 25, Version = GameVersion.Stadium }, // Eevee
            new EncounterStatic { Species = 138, Level = 20, Version = GameVersion.Stadium }, // Omanyte
            new EncounterStatic { Species = 140, Level = 20, Version = GameVersion.Stadium }, // Kabuto
            new EncounterStatic { Species = 151, Level = 5, IVs = new [] {15,15,15,15,15,15}, Version = GameVersion.VCEvents }, // Event Mew
        };
        internal static readonly EncounterTrade[] TradeGift_RBY_Common =
          {
            // Species & Minimum level (legal) possible to acquire at.
          //new EncounterTrade { Species = 122, Generation = 1, Level = 06 }, // Mr. Mime - Game Corner Abra
            new EncounterTrade { Species = 032, Generation = 1, Level = 02, Version = GameVersion.RD }, // Nidoran♂ - Wild Nidoran♀
            new EncounterTrade { Species = 029, Generation = 1, Level = 02, Version = GameVersion.BU }, // Nidoran♀ - Wild Nidoran♂
            new EncounterTrade { Species = 030, Generation = 1, Level = 16, Version = GameVersion.RB  }, // Nidorina - Evolve Nidorino
            new EncounterTrade { Species = 030, Generation = 1, Level = 16, Version = GameVersion.YW }, // Nidorina - Evolve Nidorino (Different initial moves)
            new EncounterTrade { Species = 108, Generation = 1, Level = 15, Version = GameVersion.RBY }, // Lickitung - Surf Slowbro
            new EncounterTrade { Species = 083, Generation = 1, Level = 02, Version = GameVersion.RBY }, // Farfetch’d - Wild Spearow
            new EncounterTrade { Species = 101, Generation = 1, Level = 03, Version = GameVersion.RBY }, // Electrode - Wild Raichu
            
            new EncounterTrade { Species = 122, Generation = 1, Level = 03, Version = GameVersion.RBY }, // Mr. Mime - Wild Jigglypuff
            new EncounterTrade { Species = 060, Generation = 1, Level = 02, Version = GameVersion.RBY }, // Poliwag - Wild Rattata
          //new EncounterTrade { Species = 083, Generation = 1, Level = 02 }, // Farfetch’d - Wild Pidgey
            new EncounterTrade { Species = 079, Generation = 1, Level = 22, Version = GameVersion.RBY }, // Slowpoke - Wild Seel
            
            new EncounterTrade { Species = 051, Generation = 1, Level = 15, Version = GameVersion.RBY }, // Dugtrio - Trade Lickitung
            new EncounterTrade { Species = 047, Generation = 1, Level = 13, Version = GameVersion.RBY }, // Parasect - Trade Tangela
        };
        internal static readonly EncounterTrade[] TradeGift_RBY_NoTradeback = TradeGift_RBY_Common.Concat(new[]
        {
            // Species & Minimum level (legal) possible to acquire at.
            new EncounterTrade { Species = 124, Generation = 1, Level = 15, Version = GameVersion.RBY }, // Jynx - Fish Poliwhirl (GSC: 10)
            new EncounterTrade { Species = 114, Generation = 1, Level = 13, Version = GameVersion.RBY }, // Tangela - Wild Venonat (GSC: 5) No different moves at level 13
            new EncounterTrade { Species = 086, Generation = 1, Level = 28, Version = GameVersion.RBY }, // Seel - Wild Ponyta (GSC: 6)
            
            new EncounterTrade { Species = 115, Generation = 1, Level = 42, Version = GameVersion.RBY }, // Kangaskhan - Evolve Rhydon (GSC: 30)
            new EncounterTrade { Species = 128, Generation = 1, Level = 28, Version = GameVersion.RBY }, // Tauros - Evolve Persian (GSC: 18)
            new EncounterTradeCatchRate { Species = 093, Generation = 1, Level = 20, Catch_Rate = 45, EvolveOnTrade = true, Version = GameVersion.RBY }, // Haunter - Trade Machoke (GSC: 10)
            new EncounterTradeCatchRate { Species = 075, Generation = 1, Level = 16, Catch_Rate = 45, EvolveOnTrade = true, Version = GameVersion.RBY }, // Graveler - Evolve Kadabra (GSC: 15)
            new EncounterTradeCatchRate { Species = 098, Generation = 1, Level = 15, Catch_Rate = 204, Version = GameVersion.RBY }, // Krabby - Wild Growlithe (GSC: 5)
            
          //new EncounterTrade { Species = 122, Generation = 1, Level = 08 }, // Mr. Mime - Wild Clefairy (GSC: 6)
            new EncounterTradeCatchRate { Species = 067, Generation = 1, Level = 20, Catch_Rate = 180, EvolveOnTrade = true, Version = GameVersion.RBY }, // Machoke - Wild Cubone (GSC: 10)
            new EncounterTrade { Species = 112, Generation = 1, Level = 15, Version = GameVersion.RBY }, // Rhydon - Surf Golduck (GSC: 10)
            new EncounterTrade { Species = 087, Generation = 1, Level = 15, Version = GameVersion.RBY }, // Dewgong - Wild Growlithe (GSC: 5)
            new EncounterTrade { Species = 089, Generation = 1, Level = 25, Version = GameVersion.RBY }, // Muk - Wild Kangaskhan (GSC: 14)
        }).ToArray();
        internal static readonly EncounterTrade[] TradeGift_RBY_Tradeback = TradeGift_RBY_Common.Concat(new[]
        {
            // Trade gifts that can be obtained at a lower level due to the requested Pokémon being a lower level in GSC
            new EncounterTrade { Species = 124, Generation = 1, Level = 10, Version = GameVersion.RBY }, // Jynx - Fish Poliwhirl (RBY: 15)
            new EncounterTrade { Species = 114, Generation = 1, Level = 05, Version = GameVersion.RBY }, // Tangela - Wild Venonat (RBY: 13)
            new EncounterTrade { Species = 086, Generation = 1, Level = 05, Version = GameVersion.RBY }, // Seel - Egg Ponyta (RBY: 28)
            
            new EncounterTrade { Species = 115, Generation = 1, Level = 30, Version = GameVersion.RBY }, // Kangaskhan - Evolve Rhydon (RBY: 42)
            new EncounterTrade { Species = 128, Generation = 1, Level = 18, Version = GameVersion.RBY }, // Tauros - Evolve Persian (RBY: 28)
            new EncounterTradeCatchRate { Species = 093, Generation = 1, Level = 10, Catch_Rate = 45, EvolveOnTrade = true, Version = GameVersion.RBY }, // Haunter - Trade Machoke (RBY: 20)
            new EncounterTradeCatchRate { Species = 075, Generation = 1, Level = 15, Catch_Rate = 45, EvolveOnTrade = true, Version = GameVersion.RBY }, // Graveler - Evolve Kadabra (RBY: 16)
            new EncounterTradeCatchRate { Species = 098, Generation = 1, Level = 05, Catch_Rate = 204, Version = GameVersion.RBY }, // Krabby - Egg Growlithe (RBY: 15)
            
          //new EncounterTrade { Species = 122, Generation = 1, Level = 08 }, // Mr. Mime - Wild Clefairy (RBY: 6)
            new EncounterTradeCatchRate { Species = 067, Generation = 1, Level = 05, Catch_Rate = 180, EvolveOnTrade = true, Version = GameVersion.RBY }, // Machoke - Egg Cubone (RBY: 20)
            new EncounterTrade { Species = 112, Generation = 1, Level = 10, Version = GameVersion.RBY }, // Rhydon - Surf Golduck (RBY: 15)
            new EncounterTrade { Species = 087, Generation = 1, Level = 05, Version = GameVersion.RBY }, // Dewgong - Egg Growlithe (RBY: 15)
            new EncounterTrade { Species = 089, Generation = 1, Level = 05, Version = GameVersion.RBY }, // Muk - Egg Kangaskhan (RBY: 25)
        }).ToArray();
        internal static readonly EncounterArea FishOldGood_RBY = new EncounterArea { Location = -1, Slots = new EncounterSlot[]
        {
            new EncounterSlot1 {Species = 129, LevelMin = 05, LevelMax = 05, Type = SlotType.Old_Rod,  Rate = -1, Version = GameVersion.RBY }, // Magikarp
            new EncounterSlot1 {Species = 118, LevelMin = 10, LevelMax = 10, Type = SlotType.Good_Rod, Rate = -1, Version = GameVersion.RBY }, // Goldeen
            new EncounterSlot1 {Species = 060, LevelMin = 10, LevelMax = 10, Type = SlotType.Good_Rod, Rate = -1, Version = GameVersion.RBY }, // Poliwag
        }};

        internal static readonly int[] FutureEvolutionsGen1 =
        {
            169,182,186,196,197,199,208,212,230,233,242,462,463,464,465,466,467,470,471,474,700
        };

        internal static readonly int[] FutureEvolutionsGen1_Gen2LevelUp =
        {
            // Crobat Espeon Umbreon Blissey
            169,196,197,242
        };
        internal static readonly int[] SpecialMinMoveSlots =
        {
            25, 26, 29, 30, 31, 32, 33, 34, 36, 38, 40, 59, 91, 103, 114, 121,
        };
        internal static readonly int[] Types_Gen1 =
        {
            0, 1, 2, 3, 4, 5, 7, 8, 20, 21, 22, 23, 24, 25, 26
        };
        internal static readonly int[] Species_NotAvailable_CatchRate =
        {
            12, 18, 31, 34, 36, 38, 45, 53, 59, 62, 65, 68, 71, 78, 91, 103, 121
        };
        internal static readonly int[] Stadium_CatchRate =
        {
            167, // Normal Box
            168, // Gorgeous Box
        };
        internal static readonly int[] Stadium_GiftSpecies =
        {
            001, // Bulbasaur
            004, // Charmander
            007, // Squirtle
            054, // Psyduck (Amnesia)
            106, // Hitmonlee
            107, // Hitmonchan
            133, // Eevee
            138, // Omanyte
            140, // Kabuto
        };
        internal static readonly int[] Trade_Evolution1 =
        {
            064,
            067,
            075,
            093
        };
    }
}
