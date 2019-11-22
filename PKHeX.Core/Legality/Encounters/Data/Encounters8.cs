using static PKHeX.Core.EncounterUtil;
using static PKHeX.Core.Shiny;
using static PKHeX.Core.GameVersion;

using static PKHeX.Core.Encounters8Nest;

namespace PKHeX.Core
{
    internal static class Encounters8
    {
        internal static readonly EncounterArea8[] SlotsSW = GetEncounterTables<EncounterArea8>("sw", "sw");
        internal static readonly EncounterArea8[] SlotsSH = GetEncounterTables<EncounterArea8>("sh", "sh");
        internal static readonly EncounterStatic[] StaticSW, StaticSH;

        static Encounters8()
        {
            SlotsSW.SetVersion(SW);
            SlotsSH.SetVersion(SH);
            Encounter_SWSH.SetVersion(SWSH);
            TradeGift_SWSH.SetVersion(SWSH);
            Nest_Common.SetVersion(SWSH);
            Nest_SW.SetVersion(SW);
            Nest_SH.SetVersion(SH);
            Crystal_SWSH.SetVersion(SWSH);
            MarkEncounterTradeStrings(TradeGift_SWSH, TradeSWSH);

            StaticSW = GetStaticEncounters(Encounter_SWSH, SW);
            StaticSH = GetStaticEncounters(Encounter_SWSH, SH);
            StaticSW = ArrayUtil.ConcatAll(StaticSW, Nest_Common, Nest_SW, GetStaticEncounters(Crystal_SWSH, SW));
            StaticSH = ArrayUtil.ConcatAll(StaticSH, Nest_Common, Nest_SH, GetStaticEncounters(Crystal_SWSH, SH));

            MarkEncountersGeneration(8, SlotsSW, SlotsSH);
            MarkEncountersGeneration(8, StaticSW, StaticSH, TradeGift_SWSH);
        }

        private static readonly EncounterStatic[] Encounter_SWSH =
        {
            // gifts
            new EncounterStatic { Gift = true, Species = 810, Shiny = Never, Level = 05, Location = 006, }, // Grookey
            new EncounterStatic { Gift = true, Species = 813, Shiny = Never, Level = 05, Location = 006, }, // Scorbunny
            new EncounterStatic { Gift = true, Species = 816, Shiny = Never, Level = 05, Location = 006, }, // Sobble

            new EncounterStatic { Gift = true, Species = 772, Shiny = Never, Level = 50, Location = 158, FlawlessIVCount = 3, }, // Type: Null
            new EncounterStatic { Gift = true, Species = 848, Shiny = Never, Level = 01, Location = 040, IVs = new[]{-1,31,-1,-1,31,-1}, Ball = 11 }, // Toxel, Attack flawless

            new EncounterStatic { Gift = true, Species = 880, FlawlessIVCount = 3, Level = 10, Location = 068, }, // Dracozolt @ Route 6
            new EncounterStatic { Gift = true, Species = 881, FlawlessIVCount = 3, Level = 10, Location = 068, }, // Arctozolt @ Route 6
            new EncounterStatic { Gift = true, Species = 882, FlawlessIVCount = 3, Level = 10, Location = 068, }, // Dracovish @ Route 6
            new EncounterStatic { Gift = true, Species = 883, FlawlessIVCount = 3, Level = 10, Location = 068, }, // Arctovish @ Route 6

            new EncounterGift8 { Gift = true, Species = 004, Shiny = Never, Level = 05, Location = 006, FlawlessIVCount = 3, CanGigantamax = true }, // Charmander
            new EncounterGift8 { Gift = true, Species = 025, Shiny = Never, Level = 10, Location = 156, FlawlessIVCount = 6, CanGigantamax = true }, // Pikachu
            new EncounterGift8 { Gift = true, Species = 133, Shiny = Never, Level = 10, Location = 156, FlawlessIVCount = 6, CanGigantamax = true }, // Eevee

            #region Static Part 1
            // encounters
            new EncounterStatic { Species = 888, Level = 70, Location = 66, Moves = new[] {533,014,442,242}, Shiny = Never, Ability = 1, FlawlessIVCount = 3, Version = SW }, // Zacian
            new EncounterStatic { Species = 889, Level = 70, Location = 66, Moves = new[] {163,242,442,334}, Shiny = Never, Ability = 1, FlawlessIVCount = 3, Version = SH }, // Zamazenta
            new EncounterStatic { Species = 890, Level = 60, Location = 66, Moves = new[] {440,406,053,744}, Shiny = Never, Ability = 1, FlawlessIVCount = 3 }, // Eternatus-1 (reverts to form 0)

            // Motostoke Stadium Static Encounters
            new EncounterStatic { Species = 037, Level = 24, Location = 24, }, // Vulpix at Motostoke Stadium
            new EncounterStatic { Species = 058, Level = 24, Location = 24, Version = SH }, // Growlithe at Motostoke Stadium
            new EncounterStatic { Species = 607, Level = 25, Location = 24, }, // Litwick at Motostoke Stadium
            new EncounterStatic { Species = 850, Level = 25, Location = 24, FlawlessIVCount = 3 }, // Sizzlipede at Motostoke Stadium

            new EncounterStatic  { Species = 618, Level = 25, Location = 054, Moves = new[] {389,319,279,341}, Form = 01, Ability = 1 }, // Stunfisk in Galar Mine No. 2
            new EncounterStatic8 { Species = 618, Level = 48, Location =  -1, Moves = new[] {779,330,340,334}, Form = 01 }, // Stunfisk
            new EncounterStatic8 { Species = 527, Level = 16, Location =  -1, Moves = new[] {000,000,000,000} }, // Woobat
            new EncounterStatic8 { Species = 838, Level = 18, Location = 030, Moves = new[] {488,397,229,033} }, // Carkol in Galar Mine
            new EncounterStatic8 { Species = 834, Level = 24, Location = 054, Moves = new[] {317,029,055,044} }, // Drednaw in Galar Mine No. 2
            new EncounterStatic8 { Species = 423, Level = 50, Location = 054, Moves = new[] {240,414,330,246}, FlawlessIVCount = 3, Form = 01 }, // Gastrodon in Galar Mine No. 2
            new EncounterStatic8 { Species = 859, Level = 31, Location = 076, Moves = new[] {259,389,207,372} }, // Impidimp in Glimwood Tangle
            new EncounterStatic8 { Species = 860, Level = 38, Location = 076, Moves = new[] {793,399,259,389} }, // Morgrem in Glimwood Tangle
            new EncounterStatic8 { Species = 835, Level = 08, Location = 018, Moves = new[] {039,033,609,000} }, // Yamper on Route 2
            new EncounterStatic8 { Species = 834, Level = 50, Location = 018, Moves = new[] {710,746,068,317}, FlawlessIVCount = 3 }, // Drednaw on Route 2
            new EncounterStatic8 { Species = 833, Level = 08, Location = 018, Moves = new[] {044,055,000,000} }, // Chewtle on Route 2
            new EncounterStatic8 { Species = 131, Level = 55, Location = 138, Moves = new[] {056,240,058,034}, FlawlessIVCount = 3 }, // Lapras at North Lake Miloch (in a Wild Area)
            new EncounterStatic8 { Species = 862, Level = 50, Location = 018, Moves = new[] {269,068,792,184} }, // Obstagoon on Route 2
            new EncounterStatic8 { Species = 822, Level = 18, Location =  -1, Moves = new[] {681,468,031,365}, Shiny = Never }, // Corvisquire
            new EncounterStatic8 { Species = 050, Level = 17, Location =  -1, Moves = new[] {523,189,310,045} }, // Diglett
            new EncounterStatic8 { Species = 830, Level = 22, Location = 040, Moves = new[] {178,496,075,047} }, // Eldegoss on Route 5
            new EncounterStatic8 { Species = 558, Level = 40, Location = 086, Moves = new[] {404,350,446,157} }, // Crustle on Route 8
            new EncounterStatic8 { Species = 870, Level = 40, Location =  -1, Moves = new[] {748,660,179,203} }, // Falinks
            new EncounterStatic8 { Species = 362, Level = 55, Location = 090, Moves = new[] {573,329,104,182}, FlawlessIVCount = 3 }, // Glalie on Route 9
            new EncounterStatic8 { Species = 853, Level = 50, Location = 092, Moves = new[] {753,576,276,179} }, // Grapploct
            new EncounterStatic8 { Species = 822, Level = 35, Location =  -1, Moves = new[] {065,184,269,365} }, // Corvisquire
            new EncounterStatic8 { Species = 614, Level = 55, Location = 106, Moves = new[] {276,059,156,329} }, // Beartic
            new EncounterStatic8 { Species = 460, Level = 55, Location = 106, Moves = new[] {008,059,452,275} }, // Abomasnow
            new EncounterStatic8 { Species = 342, Level = 50, Location = 034, Moves = new[] {242,014,534,400}, FlawlessIVCount = 3 }, // Crawdaunt in the town of Turffield
            #endregion

            #region Static Part 2
            new EncounterStatic8 { Species = 095, Level = 26, Location = 122, }, // Onix in the Rolling Fields (in a Wild Area)
            new EncounterStatic8 { Species = 416, Level = 26, Location = 122, }, // Vespiquen in the Rolling Fields (in a Wild Area)
            new EncounterStatic8 { Species = 675, Level = 32, Location = 122, }, // Pangoro in the Rolling Fields (in a Wild Area)
            new EncounterStatic8 { Species = 291, Level = 15, Location = 122, }, // Ninjask in the Rolling Fields (in a Wild Area)
            new EncounterStatic8 { Species = 315, Level = 15, Location = 122, }, // Roselia in the Rolling Fields (in a Wild Area)
            new EncounterStatic8 { Species = 045, Level = 36, Location = 124, }, // Vileplume in the Dappled Grove (in a Wild Area)
            new EncounterStatic8 { Species = 760, Level = 34, Location = 124, }, // Bewear in the Dappled Grove (in a Wild Area)
            new EncounterStatic8 { Species = 275, Level = 34, Location = 124, }, // Shiftry in the Dappled Grove (in a Wild Area)
            new EncounterStatic8 { Species = 272, Level = 34, Location = 124, }, // Ludicolo in the Dappled Grove (in a Wild Area)
            new EncounterStatic8 { Species = 426, Level = 34, Location = 126, }, // Drifblim at Watchtower Ruins (in a Wild Area)
            new EncounterStatic8 { Species = 623, Level = 40, Location = 126, }, // Golurk at Watchtower Ruins (in a Wild Area)
            new EncounterStatic8 { Species = 195, Level = 15, Location = 130, }, // Quagsire at West Lake Axewell (in a Wild Area)
            new EncounterStatic8 { Species = 099, Level = 28, Location = 130, }, // Kingler at West Lake Axewell (in a Wild Area)
            new EncounterStatic8 { Species = 660, Level = 15, Location = 122, }, // Diggersby in the Rolling Fields (in a Wild Area)
            new EncounterStatic8 { Species = 178, Level = 26, Location = 128, }, // Xatu at East Lake Axewell (in a Wild Area)
            new EncounterStatic8 { Species = 569, Level = 36, Location = 128, }, // Garbodor at East Lake Axewell (in a Wild Area)
            new EncounterStatic8 { Species = 510, Level = 28, Location = 138, }, // Liepard at North Lake Miloch (in a Wild Area)
            new EncounterStatic8 { Species = 750, Level = 31, Location = 122, }, // Mudsdale in the Rolling Fields (in a Wild Area)
            new EncounterStatic8 { Species = 067, Level = 26, Location = 134, }, // Machoke at South Lake Miloch (in a Wild Area)
            new EncounterStatic8 { Species = 435, Level = 34, Location = 134, }, // Skuntank at South Lake Miloch (in a Wild Area)
            new EncounterStatic8 { Species = 099, Level = 31, Location = 134, }, // Kingler at South Lake Miloch (in a Wild Area)
            new EncounterStatic8 { Species = 342, Level = 31, Location = 134, }, // Crawdaunt at South Lake Miloch (in a Wild Area)
            new EncounterStatic8 { Species = 208, Level = 50, Location = 136, }, // Steelix near the Giant’s Seat (in a Wild Area)
            new EncounterStatic8 { Species = 823, Level = 50, Location = 150, }, // Corviknight on the Hammerlocke Hills (in a Wild Area)
            new EncounterStatic8 { Species = 448, Level = 36, Location = 138, }, // Lucario at North Lake Miloch (in a Wild Area)
            new EncounterStatic8 { Species = 112, Level = 46, Location = 142, }, // Rhydon in Bridge Field (in a Wild Area)
            new EncounterStatic8 { Species = 625, Level = 52, Location = 136, }, // Bisharp near the Giant’s Seat (in a Wild Area)
            new EncounterStatic8 { Species = 738, Level = 46, Location = 136, }, // Vikavolt near the Giant’s Seat (in a Wild Area)
            new EncounterStatic8 { Species = 091, Level = 46, Location = 130, }, // Cloyster at West Lake Axewell (in a Wild Area)
            new EncounterStatic8 { Species = 131, Level = 56, Location = 154, }, // Lapras at the Lake of Outrage (in a Wild Area)
            new EncounterStatic8 { Species = 119, Level = 46, Location = 142, }, // Seaking in Bridge Field (in a Wild Area)
            new EncounterStatic8 { Species = 130, Level = 56, Location = 146, }, // Gyarados in Dusty Bowl (in a Wild Area)
            new EncounterStatic8 { Species = 279, Level = 46, Location = 142, }, // Pelipper in Bridge Field (in a Wild Area)
            new EncounterStatic8 { Species = 853, Level = 56, Location = 130, }, // Grapploct at West Lake Axewell (in a Wild Area)
            new EncounterStatic8 { Species = 593, Level = 46, Location = 128, }, // Jellicent at East Lake Axewell (in a Wild Area)
            new EncounterStatic8 { Species = 171, Level = 46, Location =  -1, }, // Lanturn
            new EncounterStatic8 { Species = 340, Level = 46, Location = 134, }, // Whiscash at South Lake Miloch (in a Wild Area)
            new EncounterStatic8 { Species = 426, Level = 46, Location = 134, }, // Drifblim at South Lake Miloch (in a Wild Area)
            new EncounterStatic8 { Species = 224, Level = 46, Location = 134, }, // Octillery at South Lake Miloch (in a Wild Area)
            new EncounterStatic8 { Species = 612, Level = 60, Location = 132, Ability = 1, }, // Haxorus on Axew’s Eye (in a Wild Area)
            new EncounterStatic8 { Species = 143, Level = 36, Location = 140, }, // Snorlax at the Motostoke Riverbank (in a Wild Area)
            new EncounterStatic8 { Species = 452, Level = 40, Location = 140, }, // Drapion at the Motostoke Riverbank (in a Wild Area)
            new EncounterStatic8 { Species = 561, Level = 36, Location = 140, }, // Sigilyph at the Motostoke Riverbank (in a Wild Area)
            new EncounterStatic8 { Species = 534, Level = 55, Location = 140, Ability = 1, }, // Conkeldurr at the Motostoke Riverbank (in a Wild Area)
            new EncounterStatic8 { Species = 320, Level = 56, Location = 140, }, // Wailmer at the Motostoke Riverbank (in a Wild Area)
            new EncounterStatic8 { Species = 561, Level = 40, Location = 140, }, // Sigilyph at the Motostoke Riverbank (in a Wild Area)
            new EncounterStatic8 { Species = 569, Level = 40, Location = 142, }, // Garbodor in Bridge Field (in a Wild Area)
            new EncounterStatic8 { Species = 743, Level = 40, Location = 142, }, // Ribombee in Bridge Field (in a Wild Area)
            new EncounterStatic8 { Species = 475, Level = 60, Location = 142, }, // Gallade in Bridge Field (in a Wild Area)
            new EncounterStatic8 { Species = 264, Level = 40, Location = 142, Form = 01, }, // Linoone in Bridge Field (in a Wild Area)
            new EncounterStatic8 { Species = 606, Level = 42, Location = 142, }, // Beheeyem in Bridge Field (in a Wild Area)
            new EncounterStatic8 { Species = 715, Level = 50, Location = 142, }, // Noivern in Bridge Field (in a Wild Area)
            new EncounterStatic8 { Species = 537, Level = 46, Location = 142, }, // Seismitoad in Bridge Field (in a Wild Area)
            new EncounterStatic8 { Species = 768, Level = 50, Location = 142, }, // Golisopod in Bridge Field (in a Wild Area)
            new EncounterStatic8 { Species = 760, Level = 42, Location = 142, }, // Bewear in Bridge Field (in a Wild Area)
            new EncounterStatic8 { Species = 820, Level = 42, Location = 142, }, // Greedent in Bridge Field (in a Wild Area)
            new EncounterStatic8 { Species = 598, Level = 40, Location = 142, }, // Ferrothorn in Bridge Field (in a Wild Area)
            new EncounterStatic8 { Species = 344, Level = 42, Location = 144, }, // Claydol in the Stony Wilderness (in a Wild Area)
            new EncounterStatic8 { Species = 477, Level = 60, Location = 144, }, // Dusknoir in the Stony Wilderness (in a Wild Area)
            new EncounterStatic8 { Species = 623, Level = 43, Location = 144, }, // Golurk in the Stony Wilderness (in a Wild Area)
            new EncounterStatic8 { Species = 561, Level = 40, Location = 144, }, // Sigilyph in the Stony Wilderness (in a Wild Area)
            new EncounterStatic8 { Species = 558, Level = 34, Location = 144, }, // Crustle in the Stony Wilderness (in a Wild Area)
            new EncounterStatic8 { Species = 112, Level = 41, Location = 144, }, // Rhydon in the Stony Wilderness (in a Wild Area)
            new EncounterStatic8 { Species = 763, Level = 36, Location = 144, }, // Tsareena in the Stony Wilderness (in a Wild Area)
            new EncounterStatic8 { Species = 750, Level = 41, Location = 146, }, // Mudsdale in Dusty Bowl (in a Wild Area)
            new EncounterStatic8 { Species = 185, Level = 41, Location = 146, }, // Sudowoodo in Dusty Bowl (in a Wild Area)
            new EncounterStatic8 { Species = 437, Level = 41, Location = 146, }, // Bronzong in Dusty Bowl (in a Wild Area)
            new EncounterStatic8 { Species = 248, Level = 60, Location = 146, }, // Tyranitar in Dusty Bowl (in a Wild Area)
            new EncounterStatic8 { Species = 784, Level = 60, Location = 146, Ability = 1, }, // Kommo-o in Dusty Bowl (in a Wild Area)
            new EncounterStatic8 { Species = 213, Level = 34, Location = 146, }, // Shuckle in Dusty Bowl (in a Wild Area)
            new EncounterStatic8 { Species = 330, Level = 51, Location = 146, }, // Flygon in Dusty Bowl (in a Wild Area)
            new EncounterStatic8 { Species = 526, Level = 51, Location = 146, }, // Gigalith in Dusty Bowl (in a Wild Area)
            new EncounterStatic8 { Species = 423, Level = 56, Location = 146, Form = 01, }, // Gastrodon in Dusty Bowl (in a Wild Area)
            new EncounterStatic8 { Species = 208, Level = 50, Location = 148, }, // Steelix around the Giant’s Mirror (in a Wild Area)
            new EncounterStatic8 { Species = 068, Level = 60, Location = 148, Ability = 1, }, // Machamp around the Giant’s Mirror (in a Wild Area)
            new EncounterStatic8 { Species = 182, Level = 41, Location = 148, }, // Bellossom around the Giant’s Mirror (in a Wild Area)
            new EncounterStatic8 { Species = 521, Level = 41, Location = 148, }, // Unfezant around the Giant’s Mirror (in a Wild Area)
            new EncounterStatic8 { Species = 701, Level = 36, Location = 150, }, // Hawlucha on the Hammerlocke Hills (in a Wild Area)
            new EncounterStatic8 { Species = 094, Level = 60, Location = 152, }, // Gengar near the Giant’s Cap (in a Wild Area)
            new EncounterStatic8 { Species = 823, Level = 39, Location =  -1, }, // Corviknight
            new EncounterStatic8 { Species = 573, Level = 46, Location = 152, }, // Cinccino near the Giant’s Cap (in a Wild Area)
            new EncounterStatic8 { Species = 826, Level = 41, Location = 152, }, // Orbeetle near the Giant’s Cap (in a Wild Area)
            new EncounterStatic8 { Species = 834, Level = 36, Location =  -1, }, // Drednaw
            new EncounterStatic8 { Species = 680, Level = 56, Location = 152, }, // Doublade near the Giant’s Cap (in a Wild Area)
            new EncounterStatic8 { Species = 711, Level = 41, Location = 150, }, // Gourgeist on the Hammerlocke Hills (in a Wild Area)
            new EncounterStatic8 { Species = 600, Level = 46, Location = 150, }, // Klang on the Hammerlocke Hills (in a Wild Area)
            new EncounterStatic8 { Species = 045, Level = 41, Location = 148, }, // Vileplume around the Giant’s Mirror (in a Wild Area)
            new EncounterStatic8 { Species = 823, Level = 38, Location =  -1, }, // Corviknight
            new EncounterStatic8 { Species = 130, Level = 60, Location = 154, }, // Gyarados at the Lake of Outrage (in a Wild Area)
            new EncounterStatic8 { Species = 853, Level = 56, Location = 154, }, // Grapploct at the Lake of Outrage (in a Wild Area)
            new EncounterStatic8 { Species = 282, Level = 60, Location =  -1, }, // Gardevoir
            new EncounterStatic8 { Species = 470, Level = 56, Location = 154, }, // Leafeon at the Lake of Outrage (in a Wild Area)
            new EncounterStatic8 { Species = 510, Level = 31, Location =  -1, }, // Liepard
            new EncounterStatic8 { Species = 832, Level = 65, Location =  -1, }, // Dubwool
            new EncounterStatic8 { Species = 826, Level = 65, Location =  -1, }, // Orbeetle
            new EncounterStatic8 { Species = 823, Level = 65, Location = 126, }, // Corviknight at Watchtower Ruins (in a Wild Area)
            new EncounterStatic8 { Species = 110, Level = 65, Location = 128, Form = 01, }, // Weezing at East Lake Axewell (in a Wild Area)
            new EncounterStatic8 { Species = 834, Level = 65, Location = 130, }, // Drednaw at West Lake Axewell (in a Wild Area)
            new EncounterStatic8 { Species = 845, Level = 65, Location = 132, }, // Cramorant on Axew’s Eye (in a Wild Area)
            new EncounterStatic8 { Species = 828, Level = 65, Location = 134, }, // Thievul at South Lake Miloch (in a Wild Area)
            new EncounterStatic8 { Species = 884, Level = 65, Location = 136, }, // Duraludon near the Giant’s Seat (in a Wild Area)
            new EncounterStatic8 { Species = 836, Level = 65, Location = 138, }, // Boltund at North Lake Miloch (in a Wild Area)
            new EncounterStatic8 { Species = 830, Level = 65, Location = 140, }, // Eldegoss at the Motostoke Riverbank (in a Wild Area)
            new EncounterStatic8 { Species = 862, Level = 65, Location = 142, }, // Obstagoon in Bridge Field (in a Wild Area)
            new EncounterStatic8 { Species = 861, Level = 65, Location = 144, Gender = 0, }, // Grimmsnarl in the Stony Wilderness (in a Wild Area)
            new EncounterStatic8 { Species = 844, Level = 65, Location =  -1, }, // Sandaconda
            new EncounterStatic8 { Species = 863, Level = 65, Location =  -1, }, // Perrserker
            new EncounterStatic8 { Species = 879, Level = 65, Location = 150, }, // Copperajah on the Hammerlocke Hills (in a Wild Area)
            new EncounterStatic8 { Species = 839, Level = 65, Location = 152, }, // Coalossal near the Giant’s Cap (in a Wild Area)
            new EncounterStatic8 { Species = 858, Level = 65, Location = 154, Gender = 1 }, // Hatterene at the Lake of Outrage (in a Wild Area)
            new EncounterStatic8 { Species = 279, Level = 26, Location = 138, }, // Pelipper at North Lake Miloch (in a Wild Area)
            new EncounterStatic8 { Species = 310, Level = 26, Location = 122, }, // Manectric in the Rolling Fields (in a Wild Area)
            new EncounterStatic8 { Species = 660, Level = 26, Location = 122, }, // Diggersby in the Rolling Fields (in a Wild Area)
            new EncounterStatic8 { Species = 281, Level = 26, Location = 122, }, // Kirlia in the Rolling Fields (in a Wild Area)
            new EncounterStatic8 { Species = 025, Level = 15, Location = 122, }, // Pikachu in the Rolling Fields (in a Wild Area)
            new EncounterStatic8 { Species = 439, Level = 15, Location = 122, }, // Mime Jr. in the Rolling Fields (in a Wild Area)
            new EncounterStatic8 { Species = 221, Level = 33, Location = 122, }, // Piloswine in the Rolling Fields (in a Wild Area)
            new EncounterStatic8 { Species = 558, Level = 34, Location = 122, }, // Crustle in the Rolling Fields (in a Wild Area)
            new EncounterStatic8 { Species = 282, Level = 32, Location = 122, }, // Gardevoir in the Rolling Fields (in a Wild Area)
            new EncounterStatic8 { Species = 537, Level = 36, Location = 142, }, // Seismitoad in Bridge Field (in a Wild Area)
            new EncounterStatic8 { Species = 583, Level = 36, Location = 124, }, // Vanillish in the Dappled Grove (in a Wild Area)
            new EncounterStatic8 { Species = 344, Level = 36, Location = 124, }, // Claydol in the Dappled Grove (in a Wild Area)
            new EncounterStatic8 { Species = 093, Level = 34, Location = 126, }, // Haunter at Watchtower Ruins (in a Wild Area)
            new EncounterStatic8 { Species = 356, Level = 40, Location = 126, }, // Dusclops at Watchtower Ruins (in a Wild Area)
            new EncounterStatic8 { Species = 362, Level = 40, Location = 126, }, // Glalie at Watchtower Ruins (in a Wild Area)
            new EncounterStatic8 { Species = 279, Level = 28, Location = 138, }, // Pelipper at North Lake Miloch (in a Wild Area)
            new EncounterStatic8 { Species = 536, Level = 28, Location = 130, }, // Palpitoad at West Lake Axewell (in a Wild Area)
            new EncounterStatic8 { Species = 660, Level = 28, Location =  -1, }, // Diggersby
            new EncounterStatic8 { Species = 221, Level = 36, Location =  -1, }, // Piloswine
            new EncounterStatic8 { Species = 750, Level = 36, Location = 128, }, // Mudsdale at East Lake Axewell (in a Wild Area)
            new EncounterStatic8 { Species = 437, Level = 36, Location =  -1, }, // Bronzong
            new EncounterStatic8 { Species = 536, Level = 34, Location =  -1, }, // Palpitoad
            new EncounterStatic8 { Species = 279, Level = 26, Location = 122, }, // Pelipper in the Rolling Fields (in a Wild Area)
            new EncounterStatic8 { Species = 093, Level = 31, Location = 122, }, // Haunter in the Rolling Fields (in a Wild Area)
            new EncounterStatic8 { Species = 221, Level = 33, Location = 128, }, // Piloswine at East Lake Axewell (in a Wild Area)
            new EncounterStatic8 { Species = 558, Level = 34, Location = 134, }, // Crustle at South Lake Miloch (in a Wild Area)
            new EncounterStatic8 { Species = 067, Level = 31, Location = 134, }, // Machoke at South Lake Miloch (in a Wild Area)
            new EncounterStatic8 { Species = 426, Level = 31, Location =  -1, }, // Drifblim
            new EncounterStatic8 { Species = 435, Level = 36, Location =  -1, }, // Skuntank
            new EncounterStatic8 { Species = 537, Level = 36, Location = 124, }, // Seismitoad in the Dappled Grove (in a Wild Area)
            new EncounterStatic8 { Species = 583, Level = 36, Location =  -1, }, // Vanillish
            new EncounterStatic8 { Species = 426, Level = 36, Location =  -1, }, // Drifblim
            new EncounterStatic8 { Species = 437, Level = 46, Location = 136, }, // Bronzong near the Giant’s Seat (in a Wild Area)
            new EncounterStatic8 { Species = 460, Level = 46, Location = 136, }, // Abomasnow near the Giant’s Seat (in a Wild Area)
            new EncounterStatic8 { Species = 750, Level = 46, Location = 136, }, // Mudsdale near the Giant’s Seat (in a Wild Area)
            new EncounterStatic8 { Species = 623, Level = 46, Location = 136, }, // Golurk near the Giant’s Seat (in a Wild Area)
            new EncounterStatic8 { Species = 356, Level = 46, Location = 136, }, // Dusclops near the Giant’s Seat (in a Wild Area)
            new EncounterStatic8 { Species = 518, Level = 46, Location = 136, }, // Musharna near the Giant’s Seat (in a Wild Area)
            new EncounterStatic8 { Species = 362, Level = 46, Location = 136, }, // Glalie near the Giant’s Seat (in a Wild Area)
            new EncounterStatic8 { Species = 596, Level = 46, Location =  -1, }, // Galvantula
            new EncounterStatic8 { Species = 584, Level = 47, Location = 134, }, // Vanilluxe at South Lake Miloch (in a Wild Area)
            new EncounterStatic8 { Species = 537, Level = 60, Location = 132, }, // Seismitoad on Axew’s Eye (in a Wild Area)
            new EncounterStatic8 { Species = 460, Level = 60, Location = 132, }, // Abomasnow on Axew’s Eye (in a Wild Area)
            new EncounterStatic8 { Species = 036, Level = 36, Location = 140, }, // Clefable at the Motostoke Riverbank (in a Wild Area)
            new EncounterStatic8 { Species = 743, Level = 40, Location = 140, }, // Ribombee at the Motostoke Riverbank (in a Wild Area)
            new EncounterStatic8 { Species = 112, Level = 55, Location = 140, }, // Rhydon at the Motostoke Riverbank (in a Wild Area)
            new EncounterStatic8 { Species = 823, Level = 40, Location =  -1, }, // Corviknight
            new EncounterStatic8 { Species = 760, Level = 40, Location = 142, }, // Bewear in Bridge Field (in a Wild Area)
            new EncounterStatic8 { Species = 614, Level = 60, Location = 142, }, // Beartic in Bridge Field (in a Wild Area)
            new EncounterStatic8 { Species = 461, Level = 60, Location = 142, }, // Weavile in Bridge Field (in a Wild Area)
            new EncounterStatic8 { Species = 518, Level = 60, Location = 142, }, // Musharna in Bridge Field (in a Wild Area)
            new EncounterStatic8 { Species = 437, Level = 42, Location = 144, }, // Bronzong in the Stony Wilderness (in a Wild Area)
            new EncounterStatic8 { Species = 344, Level = 42, Location = 142, }, // Claydol in Bridge Field (in a Wild Area)
            new EncounterStatic8 { Species = 452, Level = 50, Location = 142, }, // Drapion in Bridge Field (in a Wild Area)
            new EncounterStatic8 { Species = 164, Level = 50, Location = 142, }, // Noctowl in Bridge Field (in a Wild Area)
            new EncounterStatic8 { Species = 760, Level = 46, Location = 130, }, // Bewear at West Lake Axewell (in a Wild Area)
            new EncounterStatic8 { Species = 675, Level = 42, Location = 142, }, // Pangoro in Bridge Field (in a Wild Area)
            new EncounterStatic8 { Species = 584, Level = 50, Location = 142, }, // Vanilluxe in Bridge Field (in a Wild Area)
            new EncounterStatic8 { Species = 112, Level = 50, Location = 142, }, // Rhydon in Bridge Field (in a Wild Area)
            new EncounterStatic8 { Species = 778, Level = 50, Location = 142, }, // Mimikyu in Bridge Field (in a Wild Area)
            new EncounterStatic8 { Species = 521, Level = 40, Location = 144, }, // Unfezant in the Stony Wilderness (in a Wild Area)
            new EncounterStatic8 { Species = 752, Level = 34, Location = 144, }, // Araquanid in the Stony Wilderness (in a Wild Area)
            new EncounterStatic8 { Species = 537, Level = 41, Location = 146, }, // Seismitoad in Dusty Bowl (in a Wild Area)
            new EncounterStatic8 { Species = 435, Level = 41, Location = 146, }, // Skuntank in Dusty Bowl (in a Wild Area)
            new EncounterStatic8 { Species = 221, Level = 41, Location = 146, }, // Piloswine in Dusty Bowl (in a Wild Area)
            new EncounterStatic8 { Species = 356, Level = 41, Location =  -1, }, // Dusclops
            new EncounterStatic8 { Species = 344, Level = 41, Location = 146, }, // Claydol in Dusty Bowl (in a Wild Area)
            new EncounterStatic8 { Species = 689, Level = 60, Location = 146, }, // Barbaracle in Dusty Bowl (in a Wild Area)
            new EncounterStatic8 { Species = 561, Level = 51, Location = 146, }, // Sigilyph in Dusty Bowl (in a Wild Area)
            new EncounterStatic8 { Species = 623, Level = 51, Location = 146, }, // Golurk in Dusty Bowl (in a Wild Area)
            new EncounterStatic8 { Species = 537, Level = 60, Location = 148, }, // Seismitoad around the Giant’s Mirror (in a Wild Area)
            new EncounterStatic8 { Species = 460, Level = 60, Location = 148, }, // Abomasnow around the Giant’s Mirror (in a Wild Area)
            new EncounterStatic8 { Species = 045, Level = 41, Location = 150, }, // Vileplume on the Hammerlocke Hills (in a Wild Area)
            new EncounterStatic8 { Species = 178, Level = 41, Location = 148, }, // Xatu around the Giant’s Mirror (in a Wild Area)
            new EncounterStatic8 { Species = 768, Level = 60, Location = 152, }, // Golisopod near the Giant’s Cap (in a Wild Area)
            new EncounterStatic8 { Species = 614, Level = 60, Location = 152, }, // Beartic near the Giant’s Cap (in a Wild Area)
            new EncounterStatic8 { Species = 530, Level = 46, Location = 152, }, // Excadrill near the Giant’s Cap (in a Wild Area)
            new EncounterStatic8 { Species = 362, Level = 46, Location = 152, }, // Glalie near the Giant’s Cap (in a Wild Area)
            new EncounterStatic8 { Species = 537, Level = 46, Location = 152, }, // Seismitoad near the Giant’s Cap (in a Wild Area)
            new EncounterStatic8 { Species = 681, Level = 58, Location = 152, }, // Aegislash near the Giant’s Cap (in a Wild Area)
            new EncounterStatic8 { Species = 601, Level = 49, Location = 150, }, // Klinklang on the Hammerlocke Hills (in a Wild Area)
            new EncounterStatic8 { Species = 407, Level = 41, Location = 150, }, // Roserade on the Hammerlocke Hills (in a Wild Area)
            new EncounterStatic8 { Species = 460, Level = 41, Location = 150, }, // Abomasnow on the Hammerlocke Hills (in a Wild Area)
            new EncounterStatic8 { Species = 350, Level = 60, Location = 154, Gender = 0, Ability = 1, }, // Milotic at the Lake of Outrage (in a Wild Area)
            new EncounterStatic8 { Species = 112, Level = 60, Location = 154, }, // Rhydon at the Lake of Outrage (in a Wild Area)
            new EncounterStatic8 { Species = 609, Level = 60, Location = 154, }, // Chandelure at the Lake of Outrage (in a Wild Area)
            new EncounterStatic8 { Species = 713, Level = 60, Location = 154, }, // Avalugg at the Lake of Outrage (in a Wild Area)
            new EncounterStatic8 { Species = 756, Level = 60, Location = 154, }, // Shiinotic at the Lake of Outrage (in a Wild Area)
            new EncounterStatic8 { Species = 134, Level = 56, Location = 154, }, // Vaporeon at the Lake of Outrage (in a Wild Area)
            new EncounterStatic8 { Species = 135, Level = 56, Location = 154, }, // Jolteon at the Lake of Outrage (in a Wild Area)
            new EncounterStatic8 { Species = 196, Level = 56, Location = 154, }, // Espeon at the Lake of Outrage (in a Wild Area)
            new EncounterStatic8 { Species = 471, Level = 56, Location = 154, }, // Glaceon at the Lake of Outrage (in a Wild Area)
            new EncounterStatic8 { Species = 136, Level = 56, Location = 154, }, // Flareon at the Lake of Outrage (in a Wild Area)
            new EncounterStatic8 { Species = 197, Level = 56, Location = 154, }, // Umbreon at the Lake of Outrage (in a Wild Area)
            new EncounterStatic8 { Species = 700, Level = 56, Location = 154, }, // Sylveon at the Lake of Outrage (in a Wild Area)
            #endregion
        };

        private const string tradeSWSH = "tradeswsh";
        private static readonly string[][] TradeSWSH = Util.GetLanguageStrings10(tradeSWSH);
        private static readonly int[] TradeIVs = {15, 15, 15, 15, 15, 15};

        internal static readonly EncounterTrade8[] TradeGift_SWSH =
        {
            new EncounterTrade8(052,18) { Ability = 2, TID7 = 263455, IVs = TradeIVs, DynamaxLevel = 1, OTGender = 0, Relearn = new[] {387,000,000,000} }, // Meowth
            new EncounterTrade8(819,10) { Ability = 1, TID7 = 648753, IVs = TradeIVs, DynamaxLevel = 1, OTGender = 1,                                   }, // Skwovet
            new EncounterTrade8(546,23) { Ability = 1, TID7 = 101154, IVs = TradeIVs, DynamaxLevel = 1, OTGender = 1,                                   }, // Cottonee
            new EncounterTrade8(175,25) { Ability = 2, TID7 = 109591, IVs = TradeIVs, DynamaxLevel = 1, OTGender = 1, Relearn = new[] {791,000,000,000} }, // Togepi
            new EncounterTrade8(856,30) { Ability = 2, TID7 = 101101, IVs = TradeIVs, DynamaxLevel = 1, OTGender = 0, Version = SW                      }, // Hatenna
            new EncounterTrade8(859,30) { Ability = 1, TID7 = 256081, IVs = TradeIVs, DynamaxLevel = 1, OTGender = 0, Relearn = new[] {252,000,000,000}, Version = SH }, // Impidimp
            new EncounterTrade8(562,35) { Ability = 1, TID7 = 102534, IVs = TradeIVs, DynamaxLevel = 2, OTGender = 1, Relearn = new[] {261,000,000,000} }, // Yamask
            new EncounterTrade8(538,37) { Ability = 2, TID7 = 768945, IVs = TradeIVs, DynamaxLevel = 2, OTGender = 0, Version = SW                      }, // Throh
            new EncounterTrade8(539,37) { Ability = 1, TID7 = 881426, IVs = TradeIVs, DynamaxLevel = 2, OTGender = 0, Version = SH                      }, // Sawk
            new EncounterTrade8(122,40) { Ability = 1, TID7 = 891846, IVs = TradeIVs, DynamaxLevel = 1, OTGender = 0,                                   }, // Mr. Mime
            new EncounterTrade8(884,50) { Ability = 2, TID7 = 101141, IVs = TradeIVs, DynamaxLevel = 3, OTGender = 0, Relearn = new[] {400,000,000,000} }, // Duraludon
        };
    }
}
