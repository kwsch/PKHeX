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

        static Encounters1() => EncounterUtil.MarkEncounterTradeNicknames(TradeGift_RBY, TradeGift_RBY_OTs);

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

        internal static readonly EncounterTrade1[] TradeGift_RBY =
        {
            new EncounterTrade1(122, RB, 06    ), // Mr. Mime - Abra
            new EncounterTrade1(032, RB, 02    ), // Nidoran♂ - Nidoran♀
            new EncounterTrade1(030, RB, 16    ), // Nidorina - Nidorino
            new EncounterTrade1(108, RB, 15    ), // Lickitung - Slowbro
            new EncounterTrade1(124, RB, 15, 10), // Jynx - Poliwhirl
            new EncounterTrade1(114, RB, 13, 05), // Tangela - Venonat
            new EncounterTrade1(083, RB, 02    ), // Farfetch’d - Spearow
            new EncounterTrade1(101, RB, 03    ), // Electrode - Raichu
            new EncounterTrade1(086, RB, 28, 05), // Seel - Ponyta

            new EncounterTrade1(122, YW, 08, 06), // Mr. Mime - Clefairy
            new EncounterTrade1(067, YW, 16, 05) { EvolveOnTrade = true }, // Machoke - Cubone
            new EncounterTrade1(051, YW, 15, 05), // Dugtrio - Lickitung
            new EncounterTrade1(047, YW, 13, 05), // Parasect - Tangel
            new EncounterTrade1(112, YW, 15, 10), // Rhydon - Golduck
            new EncounterTrade1(087, YW, 15, 05), // Dewgong - Growlithe
            new EncounterTrade1(089, YW, 25, 05), // Muk - Kangaskhan
            
            new EncounterTrade1(122, BU, 03    ), // Mr. Mime - Jigglypuff
            new EncounterTrade1(029, BU, 02    ), // Nidoran♀ - Nidoran♂
            new EncounterTrade1(060, BU, 02    ), // Poliwag - Rattata
            new EncounterTrade1(115, BU, 15, 10), // Kangaskhan - Rhydon
            new EncounterTrade1(128, BU, 28, 18), // Tauros - Persian
            new EncounterTrade1(093, BU, 28, 14) { EvolveOnTrade = true }, // Haunter - Machop->Machoke
            new EncounterTrade1(083, BU, 02    ), // Farfetch’d - Wild Pidgey
            new EncounterTrade1(075, BU, 16, 15) { EvolveOnTrade = true }, // Graveler - Abra->Kadabra
            new EncounterTrade1(079, BU, 22, 05), // Slowpoke - Seel
            new EncounterTrade1(098, BU, 15, 05), // Krabby - Growlithe
        };

        private const string tradeRBY = "traderby";
        private static readonly string[][] TradeGift_RBY_OTs = Util.GetLanguageStrings7(tradeRBY);

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
            new EncounterStatic1E(054, 15, Stadium) {Moves = new[] {133, 10}, TID = 2000, OT_Names = new[]{"STADIUM", "STADE", "STADIO", "ESTADIO"}, Language = International }, // Stadium Psyduck (Amnesia)

            new EncounterStatic1E(151, 5, RB) {IVs = Yoshira, OT_Names = YoshiOT, Language = International }, // Yoshira Mew Events
            new EncounterStatic1E(151, 5, RB) {IVs = Yoshira, OT_Names = TourOT, Language = International }, // Pokémon 2000 Stadium Tour Mew
        };
    }
}
