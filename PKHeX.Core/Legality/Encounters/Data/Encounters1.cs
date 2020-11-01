using static PKHeX.Core.GameVersion;
using static PKHeX.Core.EncounterGBLanguage;

namespace PKHeX.Core
{
    /// <summary>
    /// Generation 1 Encounters
    /// </summary>
    internal static class Encounters1
    {
        private static readonly EncounterArea1[] SlotsR = Get("red", "g1", RD);
        private static readonly EncounterArea1[] SlotsB = Get("blue", "g1", BU);
        private static readonly EncounterArea1[] SlotsY = Get("yellow", "g1", YW);
        internal static readonly EncounterArea1[] SlotsRBY = ArrayUtil.ConcatAll(SlotsR, SlotsB, SlotsY);

        private static EncounterArea1[] Get(string name, string ident, GameVersion game) =>
            EncounterArea1.GetAreas(BinLinker.Unpack(Util.GetBinaryResource($"encounter_{name}.pkl"), ident), game);

        internal static readonly EncounterStatic1[] StaticRBY =
        {
            // GameVersion is RBY for Pokemon with the same catch rate and initial moves in all games
            // If there are any differences in moves or catch rate, they will be defined as different encounters (GameVersion)
            new EncounterStatic1(001, 05, RBY), // Bulbasaur
            new EncounterStatic1(004, 05, RBY), // Charmander
            new EncounterStatic1(007, 05, RBY), // Squirtle
            new EncounterStatic1(025, 05, YW), // Pikachu

            // Game Corner
            new EncounterStatic1(030, 17, RB), // Nidorina (Red Game Corner)
            new EncounterStatic1(033, 17, BU), // Nidorino (Blue[EN] / Green[JP] Game Corner)
            new EncounterStatic1(035, 08, RBY), // Clefairy (Red Game Corner)
            new EncounterStatic1(036, 24, BU), // Clefable (Blue[JP] Game Corner)
            new EncounterStatic1(037, 18, RBY), // Vulpix (Yellow Game Corner)
            new EncounterStatic1(040, 22, RBY), // Wigglytuff (Yellow Game Corner)
            new EncounterStatic1(063, 06, BU), // Abra (Blue[EN] / Green[JP] Game Corner)
            new EncounterStatic1(116, 18, BU), // Horsea (Blue[JP] Game Corner)
            new EncounterStatic1(123, 25, RBY), // Scyther (Red Game Corner)
            new EncounterStatic1(127, 20, BU), // Pinsir (Blue[EN] / Green[JP] Game Corner)
            new EncounterStatic1(127, 30, YW), // Pinsir (Yellow Game Corner) (Different initial moves)
            new EncounterStatic1(137, 18, BU), // Porygon (Blue[EN] / Green[JP] Game Corner)
            new EncounterStatic1(147, 18, RBY), // Dratini (Red Game Corner)
            new EncounterStatic1(148, 30, BU), // Dragonair (Blue[JP] Game Corner)
            new EncounterStatic1(025, 12, BU), // Pikachu (Blue[JP] Game Corner) (Different catch rate)

            // Lower level less ideal matches; best match is from above.
            // new EncounterStatic1(035, 12), // Clefairy (Blue[EN] / Green[JP] Game Corner)
            // new EncounterStatic1(063, 09), // Abra (Red Game Corner)
            // new EncounterStatic1(063, 08), // Abra (Blue[JP] Game Corner)
            // new EncounterStatic1(063, 15), // Abra (Yellow Game Corner)
            // new EncounterStatic1(123, 30), // Scyther (Yellow Game Corner)
            // new EncounterStatic1(137, 22), // Porygon (Blue[JP] Game Corner)
            // new EncounterStatic1(137, 26), // Porygon (Red Game Corner)
            // new EncounterStatic1(137, 26), // Porygon (Yellow Game Corner)
            // new EncounterStatic1(147, 24), // Dratini (Blue[EN] / Green[JP] Game Corner)

            new EncounterStatic1(129, 05, RBY), // Magikarp
            new EncounterStatic1(143, 30, RBY), // Snorlax
            new EncounterStatic1(106, 30, RBY), // Hitmonlee
            new EncounterStatic1(107, 30, RBY), // Hitmonchan

            new EncounterStatic1(131, 15, RBY), // Lapras
            new EncounterStatic1(138, 30, RBY), // Omanyte
            new EncounterStatic1(140, 30, RBY), // Kabuto
            new EncounterStatic1(142, 30, RBY), // Aerodactyl

            new EncounterStatic1(144, 50, RBY), // Articuno
            new EncounterStatic1(145, 50, RBY), // Zapdos
            new EncounterStatic1(146, 50, RBY), // Moltres

            new EncounterStatic1(150, 70, RBY), // Mewtwo

            new EncounterStatic1(133, 25, RB), // Eevee
            new EncounterStatic1(133, 25, YW), // Eevee (Different initial moves)

            new EncounterStatic1(100, 40, RBY), // Voltorb (Power Plant)
            new EncounterStatic1(101, 43, RBY), // Electrode (Power Plant)

            // Yellow Only -- duplicate encounters with a higher level
            // new EncounterStatic1(001, 10, YW), // Bulbasaur (Cerulean City)
            // new EncounterStatic1(004, 10, YW), // Charmander (Route 24)
            // new EncounterStatic1(007, 10, YW), // Squirtle (Vermillion City)

            new EncounterStatic1(001, 05, Stadium), // Bulbasaur
            new EncounterStatic1(004, 05, Stadium), // Charmander
            new EncounterStatic1(071, 05, Stadium), // Squirtle
            new EncounterStatic1(106, 20, Stadium), // Hitmonlee
            new EncounterStatic1(107, 20, Stadium), // Hitmonchan
            new EncounterStatic1(133, 25, Stadium), // Eevee
            new EncounterStatic1(138, 20, Stadium), // Omanyte
            new EncounterStatic1(140, 20, Stadium), // Kabuto
        };

        internal static readonly EncounterTrade1[] TradeGift_RBY_Common =
        {
            // Species & Minimum level (legal) possible to acquire at.
          //new EncounterTrade1(122, 06, RBY), // Mr. Mime - Game Corner Abra
            new EncounterTrade1(032, 02, RD), // Nidoran♂ - Wild Nidoran♀
            new EncounterTrade1(029, 02, BU), // Nidoran♀ - Wild Nidoran♂
            new EncounterTrade1(030, 16, RB), // Nidorina - Evolve Nidorino
            new EncounterTrade1(030, 16, YW), // Nidorina - Evolve Nidorino (Different initial moves)
            new EncounterTrade1(108, 15, RBY), // Lickitung - Surf Slowbro
            new EncounterTrade1(083, 02, RBY), // Farfetch’d - Wild Spearow
            new EncounterTrade1(101, 03, RBY), // Electrode - Wild Raichu

            new EncounterTrade1(122, 03, RBY), // Mr. Mime - Wild Jigglypuff
            new EncounterTrade1(060, 02, RBY), // Poliwag - Wild Rattata
          //new EncounterTrade1(083, 02, RBY), // Farfetch’d - Wild Pidgey

            new EncounterTrade1(093, 28, RBY, 90) { EvolveOnTrade = true }, // Haunter - Evolve Machop->Machoke
            new EncounterTrade1(075, 16, RBY, 120) { EvolveOnTrade = true }, // Graveler - Evolve Abra->Kadabra
        };

        internal static readonly EncounterTrade1[] TradeGift_RBY_NoTradeback = ArrayUtil.ConcatAll(TradeGift_RBY_Common, new[]
        {
            new EncounterTrade1(124, 15, RBY), // Jynx - Fish Poliwhirl (GSC: 10)
            new EncounterTrade1(114, 13, RBY), // Tangela - Wild Venonat (GSC: 5) No different moves at level 13
            new EncounterTrade1(086, 28, RBY), // Seel - Wild Ponyta (GSC: 5)

            new EncounterTrade1(115, 15, RBY), // Kangaskhan - Trade Rhydon (GSC: 10)
            new EncounterTrade1(128, 28, RBY), // Tauros - Evolve Persian (GSC: 18)
            new EncounterTrade1(098, 15, RBY, 204), // Krabby - Wild Growlithe (GSC: 5)

          //new EncounterTrade1(122, 08, RBY), // Mr. Mime - Wild Clefairy (GSC: 6)
            new EncounterTrade1(067, 16, RBY) { EvolveOnTrade = true }, // Machoke - Wild Cubone (GSC: 5)
            new EncounterTrade1(112, 15, RBY), // Rhydon - Surf Golduck (GSC: 10)
            new EncounterTrade1(087, 15, RBY), // Dewgong - Wild Growlithe (GSC: 5)
            new EncounterTrade1(089, 25, RBY), // Muk - Wild Kangaskhan (GSC: 5)
            new EncounterTrade1(079, 22, RBY), // Slowpoke - Wild Seel (GSC 5)
            new EncounterTrade1(051, 15, RBY), // Dugtrio - Trade Lickitung (GSC 5)
            new EncounterTrade1(047, 13, RBY), // Parasect - Trade Tangela (GSC 5)
        });

        internal static readonly EncounterTrade1[] TradeGift_RBY_Tradeback = ArrayUtil.ConcatAll(TradeGift_RBY_Common, new[]
        {
            // Trade gifts that can be obtained at a lower level due to the requested Pokémon being a lower level in GSC
            new EncounterTrade1(124, 10, RBY), // Jynx - Fish Poliwhirl (RBY: 15)
            new EncounterTrade1(114, 05, RBY), // Tangela - Wild Venonat (RBY: 13)
            new EncounterTrade1(086, 05, RBY), // Seel - Egg Ponyta (RBY: 28)

            new EncounterTrade1(115, 10, RBY), // Kangaskhan - Trade Rhydon (RBY: 42)
            new EncounterTrade1(128, 18, RBY), // Tauros - Evolve Persian (RBY: 28)
            new EncounterTrade1(098, 05, RBY, 204), // Krabby - Egg Growlithe (RBY: 15)

          //new EncounterTrade1(122, 08), // Mr. Mime - Wild Clefairy (RBY: 6)
            new EncounterTrade1(067, 05, RBY) { EvolveOnTrade = true }, // Machoke - Egg Cubone (RBY: 20)
            new EncounterTrade1(112, 10, RBY), // Rhydon - Surf Golduck (RBY: 15)
            new EncounterTrade1(087, 05, RBY), // Dewgong - Egg Growlithe (RBY: 15)
            new EncounterTrade1(089, 05, RBY), // Muk - Egg Kangaskhan (RBY: 25)

            new EncounterTrade1(079, 05, RBY), // Slowpoke - Wild Seel (GSC 5)
            new EncounterTrade1(051, 05, RBY), // Dugtrio - Trade Lickitung (GSC 5)
            new EncounterTrade1(047, 05, RBY), // Parasect - Trade Tangela (GSC 5)
        });

        private static readonly int[] Flawless15 = { 15, 15, 15, 15, 15, 15 };
        private static readonly int[] Yoshira = { 5, 10, 1, 12, 5, 5 };
        private static readonly string[] YoshiOT = new[] { "YOSHIRA", "YOSHIRB", "YOSHIBA", "YOSHIBB" };
        private static readonly string[] TourOT = new[] { "LINKE", "LINKW", "LUIGE", "LUIGW", "LUIGIC", "YOSHIC" };

        internal static readonly EncounterStatic1E[] StaticEventsVC =
        {
            // Event Mew
            new EncounterStatic1E(151, 5, RBY) { IVs = Flawless15, TID = 22796, OT_Name = "GF", Language = EncounterGBLanguage.International },
            new EncounterStatic1E(151, 5, RBY) { IVs = Flawless15, TID = 22796, OT_Name = "ゲーフリ" },
        };

        internal static readonly EncounterStatic1E[] StaticEventsGB =
        {
            // Stadium 1: Psyduck
            new EncounterStatic1E(054, 15, Stadium) {Moves = new[] {133, 10}, TID = 1999, OT_Name = "スタジアム" }, // Stadium Psyduck (Amnesia)
            new EncounterStatic1E(054, 15, Stadium) {Moves = new[] {133, 10}, TID = 2000, OT_Names = new[]{"STADIUM", "Stade", "Stadio", "Estadio"}, Language = International }, // Stadium Psyduck (Amnesia)

            new EncounterStatic1E(151, 5, RB) {IVs = Yoshira, OT_Names = YoshiOT, Language = International }, // Yoshira Mew Events
            new EncounterStatic1E(151, 5, RB) {IVs = Yoshira, OT_Names = TourOT, Language = International }, // Pokémon 2000 Stadium Tour Mew
        };
    }
}
