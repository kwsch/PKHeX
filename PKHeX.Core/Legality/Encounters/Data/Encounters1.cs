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
        private static readonly EncounterArea1[] SlotsG = Get("blue", "g1", GN);
        private static readonly EncounterArea1[] SlotsY = Get("yellow", "g1", YW);
        private static readonly EncounterArea1[] SlotsB = Get("blue_jp", "g1", BU);
        internal static readonly EncounterArea1[] SlotsRBY = ArrayUtil.ConcatAll(SlotsR, SlotsG, SlotsY);
        internal static readonly EncounterArea1[] SlotsRGBY = ArrayUtil.ConcatAll(SlotsRBY, SlotsB);

        private static EncounterArea1[] Get(string name, string ident, GameVersion game) =>
            EncounterArea1.GetAreas(BinLinker.Unpack(Util.GetBinaryResource($"encounter_{name}.pkl"), ident), game);

        static Encounters1() => EncounterUtil.MarkEncounterTradeNicknames(TradeGift_RBY, TradeGift_RBY_OTs);

        internal static readonly EncounterStatic1[] StaticRBY =
        {
            // GameVersion is RBY for Pokemon with the same catch rate and initial moves in all games
            // If there are any differences in moves or catch rate, they will be defined as different encounters (GameVersion)
            new(001, 05, RBY), // Bulbasaur
            new(004, 05, RBY), // Charmander
            new(007, 05, RBY), // Squirtle
            new(025, 05, YW), // Pikachu

            // Game Corner
            new(030, 17, RB), // Nidorina (Red Game Corner)
            new(033, 17, BU), // Nidorino (Blue[EN] / Green[JP] Game Corner)
            new(035, 08, RBY), // Clefairy (Red Game Corner)
            new(036, 24, BU), // Clefable (Blue[JP] Game Corner)
            new(037, 18, RBY), // Vulpix (Yellow Game Corner)
            new(040, 22, RBY), // Wigglytuff (Yellow Game Corner)
            new(063, 06, BU), // Abra (Blue[EN] / Green[JP] Game Corner)
            new(116, 18, BU), // Horsea (Blue[JP] Game Corner)
            new(123, 25, RBY), // Scyther (Red Game Corner)
            new(127, 20, BU), // Pinsir (Blue[EN] / Green[JP] Game Corner)
            new(127, 30, YW), // Pinsir (Yellow Game Corner) (Different initial moves)
            new(137, 18, BU), // Porygon (Blue[EN] / Green[JP] Game Corner)
            new(147, 18, RBY), // Dratini (Red Game Corner)
            new(148, 30, BU), // Dragonair (Blue[JP] Game Corner)
            new(025, 12, BU), // Pikachu (Blue[JP] Game Corner) (Different catch rate)

            // Lower level less ideal matches; best match is from above.
         // new(035, 12), // Clefairy (Blue[EN] / Green[JP] Game Corner)
         // new(063, 09), // Abra (Red Game Corner)
         // new(063, 08), // Abra (Blue[JP] Game Corner)
         // new(063, 15), // Abra (Yellow Game Corner)
         // new(123, 30), // Scyther (Yellow Game Corner)
         // new(137, 22), // Porygon (Blue[JP] Game Corner)
         // new(137, 26), // Porygon (Red Game Corner)
         // new(137, 26), // Porygon (Yellow Game Corner)
         // new(147, 24), // Dratini (Blue[EN] / Green[JP] Game Corner)

            new(129, 05, RBY), // Magikarp
            new(143, 30, RBY), // Snorlax
            new(106, 30, RBY), // Hitmonlee
            new(107, 30, RBY), // Hitmonchan

            new(131, 15, RBY), // Lapras
            new(138, 30, RBY), // Omanyte
            new(140, 30, RBY), // Kabuto
            new(142, 30, RBY), // Aerodactyl

            new(144, 50, RBY), // Articuno
            new(145, 50, RBY), // Zapdos
            new(146, 50, RBY), // Moltres

            new(150, 70, RBY), // Mewtwo

            new(133, 25, RB), // Eevee
            new(133, 25, YW), // Eevee (Different initial moves)

            new(100, 40, RBY), // Voltorb (Power Plant)
            new(101, 43, RBY), // Electrode (Power Plant)

            // Yellow Only -- duplicate encounters with a higher level
         // new(001, 10, YW), // Bulbasaur (Cerulean City)
         // new(004, 10, YW), // Charmander (Route 24)
         // new(007, 10, YW), // Squirtle (Vermillion City)
        };

        internal static readonly EncounterTrade1[] TradeGift_RBY =
        {
            new(122, RB, 06, 05), // Mr. Mime - Abra
            new(032, RB, 02    ), // Nidoran♂ - Nidoran♀
            new(030, RB, 16    ), // Nidorina - Nidorino
            new(108, RB, 15    ), // Lickitung - Slowbro
            new(124, RB, 15, 10), // Jynx - Poliwhirl
            new(083, RB, 02    ), // Farfetch’d - Spearow
            new(101, RB, 03    ), // Electrode - Raichu
            new(114, RB, 13, 05), // Tangela - Venonat
            new(086, RB, 28, 05), // Seel - Ponyta

            new(122, YW, 08, 06), // Mr. Mime - Clefairy
            new(067, YW, 16, 05) { EvolveOnTrade = true }, // Machoke - Cubone
            new(051, YW, 15, 05), // Dugtrio - Lickitung
            new(047, YW, 13, 05), // Parasect - Tangel
            new(112, YW, 15, 10), // Rhydon - Golduck
            new(087, YW, 15, 05), // Dewgong - Growlithe
            new(089, YW, 25, 05), // Muk - Kangaskhan

            new(122, BU, 03    ), // Mr. Mime - Jigglypuff
            new(029, BU, 02    ), // Nidoran♀ - Nidoran♂
            new(060, BU, 02    ), // Poliwag - Rattata
            new(115, BU, 15, 10), // Kangaskhan - Rhydon
            new(128, BU, 28, 18), // Tauros - Persian
            new(093, BU, 28, 14) { EvolveOnTrade = true }, // Haunter - Machop->Machoke
            new(083, BU, 02    ), // Farfetch’d - Wild Pidgey
            new(075, BU, 16, 15) { EvolveOnTrade = true }, // Graveler - Abra->Kadabra
            new(079, BU, 22, 05), // Slowpoke - Seel
            new(098, BU, 15, 05), // Krabby - Growlithe
        };

        private const string tradeRBY = "traderby";
        private static readonly string[][] TradeGift_RBY_OTs = Util.GetLanguageStrings7(tradeRBY);

        private static readonly int[] Flawless15 = { 15, 15, 15, 15, 15, 15 };
        private static readonly int[] Yoshira = { 5, 10, 1, 12, 5, 5 };
        private static readonly string[] YoshiOT = { "YOSHIRA", "YOSHIRB", "YOSHIBA", "YOSHIBB" };
        private static readonly string[] TourOT = { "LINKE", "LINKW", "LUIGE", "LUIGW", "LUIGIC", "YOSHIC" };
        private static readonly string[] StadiumOT_Int = { "STADIUM", "STADE", "STADIO", "ESTADIO" };
        private const string StadiumOT_JPN = "スタジアム";

        internal static readonly EncounterStatic1E[] StaticEventsVC =
        {
            // Event Mew
            new(151, 5, RBY) { IVs = Flawless15, TID = 22796, OT_Name = "GF", Language = International },
            new(151, 5, RBY) { IVs = Flawless15, TID = 22796, OT_Name = "ゲーフリ" },
        };

        internal static readonly EncounterStatic1E[] StaticEventsGB =
        {
            // Stadium 1 (International)
            new(001, 05, Stadium) {Moves = new[] {033, 045}, TID = 2000, OT_Names = StadiumOT_Int, Language = International}, // Bulbasaur
            new(004, 05, Stadium) {Moves = new[] {010, 043}, TID = 2000, OT_Names = StadiumOT_Int, Language = International}, // Charmander
            new(007, 05, Stadium) {Moves = new[] {033, 045}, TID = 2000, OT_Names = StadiumOT_Int, Language = International}, // Squirtle
            new(106, 20, Stadium) {Moves = new[] {024, 096}, TID = 2000, OT_Names = StadiumOT_Int, Language = International}, // Hitmonlee
            new(107, 20, Stadium) {Moves = new[] {004, 097}, TID = 2000, OT_Names = StadiumOT_Int, Language = International}, // Hitmonchan
            new(133, 25, Stadium) {Moves = new[] {033, 039}, TID = 2000, OT_Names = StadiumOT_Int, Language = International}, // Eevee
            new(138, 20, Stadium) {Moves = new[] {055, 110}, TID = 2000, OT_Names = StadiumOT_Int, Language = International}, // Omanyte
            new(140, 20, Stadium) {Moves = new[] {010, 106}, TID = 2000, OT_Names = StadiumOT_Int, Language = International}, // Kabuto
            new(054, 15, Stadium) {Moves = new[] {133, 010}, TID = 2000, OT_Names = StadiumOT_Int, Language = International}, // Psyduck (Amnesia)

            // Stadium 2 (Japan)
            new(001, 05, Stadium) {Moves = new[] {033, 045}, TID = 1999, OT_Name = StadiumOT_JPN}, // Bulbasaur
            new(004, 05, Stadium) {Moves = new[] {010, 043}, TID = 1999, OT_Name = StadiumOT_JPN}, // Charmander
            new(007, 05, Stadium) {Moves = new[] {033, 045}, TID = 1999, OT_Name = StadiumOT_JPN}, // Squirtle
            new(106, 20, Stadium) {Moves = new[] {024, 096}, TID = 1999, OT_Name = StadiumOT_JPN}, // Hitmonlee
            new(107, 20, Stadium) {Moves = new[] {004, 097}, TID = 1999, OT_Name = StadiumOT_JPN}, // Hitmonchan
            new(133, 25, Stadium) {Moves = new[] {033, 039}, TID = 1999, OT_Name = StadiumOT_JPN}, // Eevee
            new(138, 20, Stadium) {Moves = new[] {055, 110}, TID = 1999, OT_Name = StadiumOT_JPN}, // Omanyte
            new(140, 20, Stadium) {Moves = new[] {010, 106}, TID = 1999, OT_Name = StadiumOT_JPN}, // Kabuto
            new(054, 15, Stadium) {Moves = new[] {133, 010}, TID = 1999, OT_Name = StadiumOT_JPN}, // Psyduck (Amnesia)

            new(151, 5, RB) {IVs = Yoshira, OT_Names = YoshiOT, Language = International }, // Yoshira Mew Events
            new(151, 5, RB) {IVs = Yoshira, OT_Names = TourOT, Language = International }, // Pokémon 2000 Stadium Tour Mew
        };
    }
}
