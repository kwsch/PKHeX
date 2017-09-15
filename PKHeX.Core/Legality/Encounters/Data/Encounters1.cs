using System;
using System.Linq;
using static PKHeX.Core.EncounterUtil;

namespace PKHeX.Core
{
    internal static class Encounters1
    {
        internal static readonly EncounterArea[] SlotsRBY;
        internal static readonly EncounterStatic[] StaticRBY;

        static Encounters1()
        {
            StaticRBY = Encounter_RBY;
            SlotsRBY = GetAreas();
            MarkEncountersGeneration(SlotsRBY, 1);
            MarkEncountersGeneration(StaticRBY, 1);
        }

        private static EncounterArea[] GetAreas()
        {
            var red_gw = EncounterArea.GetArray1_GW(Util.GetBinaryResource("encounter_red.pkl"));
            var blu_gw = EncounterArea.GetArray1_GW(Util.GetBinaryResource("encounter_blue.pkl"));
            var ylw_gw = EncounterArea.GetArray1_GW(Util.GetBinaryResource("encounter_yellow.pkl"));
            var rb_fish = EncounterArea.GetArray1_F(Util.GetBinaryResource("encounter_rb_f.pkl"));
            var ylw_fish = EncounterArea.GetArray1_FY(Util.GetBinaryResource("encounter_yellow_f.pkl"));

            MarkEncountersVersion(red_gw, GameVersion.RD);
            MarkEncountersVersion(blu_gw, GameVersion.BW);
            MarkEncountersVersion(ylw_gw, GameVersion.YW);
            MarkEncountersVersion(rb_fish, GameVersion.RB);
            MarkEncountersVersion(ylw_fish, GameVersion.YW);

            var table = AddExtraTableSlots(red_gw, blu_gw, ylw_gw, rb_fish, ylw_fish);
            Array.Resize(ref table, table.Length + 1);
            table[table.Length - 1] = FishOldGood_RBY;

            return table;
        }

        private static readonly EncounterStatic[] Encounter_RBY =
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
            new EncounterTradeCatchRate { Species = 067, Generation = 1, Level = 16, Catch_Rate = 180, EvolveOnTrade = true, Version = GameVersion.RBY }, // Machoke - Wild Cubone (GSC: 10)
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
        private static readonly EncounterArea FishOldGood_RBY = new EncounterArea
        {
            Location = -1,
            Slots = new EncounterSlot[]
            {
                new EncounterSlot1 {Species = 129, LevelMin = 05, LevelMax = 05, Type = SlotType.Old_Rod,  Rate = -1, Version = GameVersion.RBY }, // Magikarp
                new EncounterSlot1 {Species = 118, LevelMin = 10, LevelMax = 10, Type = SlotType.Good_Rod, Rate = -1, Version = GameVersion.RBY }, // Goldeen
                new EncounterSlot1 {Species = 060, LevelMin = 10, LevelMax = 10, Type = SlotType.Good_Rod, Rate = -1, Version = GameVersion.RBY }, // Poliwag
            }
        };
    }
}
