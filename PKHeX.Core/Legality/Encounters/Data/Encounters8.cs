using System;
using static PKHeX.Core.EncounterUtil;
using static PKHeX.Core.Shiny;
using static PKHeX.Core.GameVersion;

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
            StaticSW = ConcatAll(StaticSW, Nest_Common, Nest_SW, GetStaticEncounters(Crystal_SWSH, SW));
            StaticSH = ConcatAll(StaticSH, Nest_Common, Nest_SH, GetStaticEncounters(Crystal_SWSH, SH));

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
            new EncounterStatic { Species = 037, Level = 24, Location = 24, Version = SW }, // Vulpix at Motostoke Stadium
            new EncounterStatic { Species = 058, Level = 24, Location = 24, Version = SH }, // Growlithe at Motostoke Stadium
            new EncounterStatic { Species = 607, Level = 25, Location = 24, }, // Litwick at Motostoke Stadium
            new EncounterStatic { Species = 850, Level = 25, Location = 24, FlawlessIVCount = 3 }, // Sizzlipede at Motostoke Stadium
            
            new EncounterStatic  { Species = 618, Level = 25, Location = 054, Moves = new[] {389,319,279,341}, Form = 01, Ability = 1 }, // Stunfisk in Galar Mine No. 2
            new EncounterStatic8 { Species = 618, Level = 48, Location =  -1, Moves = new[] {779,330,340,334}, Form = 01 }, // Stunfisk
            new EncounterStatic8 { Species = 527, Level = 16, Location =  -1, Moves = new[] {000,000,000,000} }, // Woobat
            new EncounterStatic8 { Species = 838, Level = 18, Location =  -1, Moves = new[] {488,397,229,033} }, // Carkol
            new EncounterStatic8 { Species = 834, Level = 24, Location = 054, Moves = new[] {317,029,055,044} }, // Drednaw in Galar Mine No. 2
            new EncounterStatic8 { Species = 423, Level = 50, Location =  -1, Moves = new[] {240,414,330,246}, FlawlessIVCount = 3, Form = 01 }, // Gastrodon
            new EncounterStatic8 { Species = 859, Level = 31, Location = 076, Moves = new[] {259,389,207,372} }, // Impidimp
            new EncounterStatic8 { Species = 860, Level = 38, Location =  -1, Moves = new[] {793,399,259,389} }, // Morgrem
            new EncounterStatic8 { Species = 835, Level = 08, Location = 018, Moves = new[] {039,033,609,000} }, // Yamper on Route 2
            new EncounterStatic8 { Species = 834, Level = 50, Location =  -1, Moves = new[] {710,746,068,317}, FlawlessIVCount = 3 }, // Drednaw
            new EncounterStatic8 { Species = 833, Level = 08, Location =  -1, Moves = new[] {044,055,000,000} }, // Chewtle
            new EncounterStatic8 { Species = 131, Level = 55, Location =  -1, Moves = new[] {056,240,058,034}, FlawlessIVCount = 3 }, // Lapras
            new EncounterStatic8 { Species = 862, Level = 50, Location =  -1, Moves = new[] {269,068,792,184} }, // Obstagoon
            new EncounterStatic8 { Species = 822, Level = 18, Location =  -1, Moves = new[] {681,468,031,365}, Shiny = Never }, // Corvisquire
            new EncounterStatic8 { Species = 050, Level = 17, Location =  -1, Moves = new[] {523,189,310,045} }, // Diglett
            new EncounterStatic8 { Species = 830, Level = 22, Location = 040, Moves = new[] {178,496,075,047} }, // Eldegoss on Route 5
            new EncounterStatic8 { Species = 558, Level = 40, Location = 086, Moves = new[] {404,350,446,157} }, // Crustle on Route 8
            new EncounterStatic8 { Species = 870, Level = 40, Location =  -1, Moves = new[] {748,660,179,203} }, // Falinks
            new EncounterStatic8 { Species = 362, Level = 55, Location =  -1, Moves = new[] {573,329,104,182}, FlawlessIVCount = 3 }, // Glalie
            new EncounterStatic8 { Species = 853, Level = 50, Location = 092, Moves = new[] {753,576,276,179} }, // Grapploct
            new EncounterStatic8 { Species = 822, Level = 35, Location =  -1, Moves = new[] {065,184,269,365} }, // Corvisquire
            new EncounterStatic8 { Species = 614, Level = 55, Location = 106, Moves = new[] {276,059,156,329} }, // Beartic
            new EncounterStatic8 { Species = 460, Level = 55, Location = 106, Moves = new[] {008,059,452,275} }, // Abomasnow
            new EncounterStatic8 { Species = 342, Level = 50, Location =  -1, Moves = new[] {242,014,534,400}, FlawlessIVCount = 3 }, // Crawdaunt
            #endregion

            #region Static Part 2
            new EncounterStatic8 { Species = 095, Level = 26, Location = 122, }, // Onix in the Rolling Fields (in a Wild Area)
            new EncounterStatic8 { Species = 416, Level = 26, Location =  -1, }, // Vespiquen
            new EncounterStatic8 { Species = 675, Level = 32, Location =  -1, }, // Pangoro
            new EncounterStatic8 { Species = 291, Level = 15, Location =  -1, }, // Ninjask
            new EncounterStatic8 { Species = 315, Level = 15, Location = 122, }, // Roselia in the Rolling Fields (in a Wild Area)
            new EncounterStatic8 { Species = 045, Level = 36, Location =  -1, }, // Vileplume
            new EncounterStatic8 { Species = 760, Level = 34, Location =  -1, }, // Bewear
            new EncounterStatic8 { Species = 275, Level = 34, Location =  -1, }, // Shiftry
            new EncounterStatic8 { Species = 272, Level = 34, Location =  -1, }, // Ludicolo
            new EncounterStatic8 { Species = 426, Level = 34, Location =  -1, }, // Drifblim
            new EncounterStatic8 { Species = 623, Level = 40, Location =  -1, }, // Golurk
            new EncounterStatic8 { Species = 195, Level = 15, Location =  -1, }, // Quagsire
            new EncounterStatic8 { Species = 099, Level = 28, Location =  -1, }, // Kingler
            new EncounterStatic8 { Species = 660, Level = 15, Location = 122, }, // Diggersby in the Rolling Fields (in a Wild Area)
            new EncounterStatic8 { Species = 178, Level = 26, Location =  -1, }, // Xatu
            new EncounterStatic8 { Species = 569, Level = 36, Location =  -1, }, // Garbodor
            new EncounterStatic8 { Species = 510, Level = 28, Location = 138, }, // Liepard at North Lake Miloch (in a Wild Area)
            new EncounterStatic8 { Species = 750, Level = 31, Location =  -1, }, // Mudsdale
            new EncounterStatic8 { Species = 067, Level = 26, Location =  -1, }, // Machoke
            new EncounterStatic8 { Species = 435, Level = 34, Location =  -1, }, // Skuntank
            new EncounterStatic8 { Species = 099, Level = 31, Location =  -1, }, // Kingler
            new EncounterStatic8 { Species = 342, Level = 31, Location =  -1, }, // Crawdaunt
            new EncounterStatic8 { Species = 208, Level = 50, Location =  -1, }, // Steelix
            new EncounterStatic8 { Species = 823, Level = 50, Location =  -1, }, // Corviknight
            new EncounterStatic8 { Species = 448, Level = 36, Location =  -1, }, // Lucario
            new EncounterStatic8 { Species = 112, Level = 46, Location =  -1, }, // Rhydon
            new EncounterStatic8 { Species = 625, Level = 52, Location =  -1, }, // Bisharp
            new EncounterStatic8 { Species = 738, Level = 46, Location =  -1, }, // Vikavolt
            new EncounterStatic8 { Species = 091, Level = 46, Location =  -1, }, // Cloyster
            new EncounterStatic8 { Species = 131, Level = 56, Location =  -1, }, // Lapras
            new EncounterStatic8 { Species = 119, Level = 46, Location =  -1, }, // Seaking
            new EncounterStatic8 { Species = 130, Level = 56, Location =  -1, }, // Gyarados
            new EncounterStatic8 { Species = 279, Level = 46, Location =  -1, }, // Pelipper
            new EncounterStatic8 { Species = 853, Level = 56, Location =  -1, }, // Grapploct
            new EncounterStatic8 { Species = 593, Level = 46, Location =  -1, }, // Jellicent
            new EncounterStatic8 { Species = 171, Level = 46, Location =  -1, }, // Lanturn
            new EncounterStatic8 { Species = 340, Level = 46, Location =  -1, }, // Whiscash
            new EncounterStatic8 { Species = 426, Level = 46, Location =  -1, }, // Drifblim
            new EncounterStatic8 { Species = 224, Level = 46, Location =  -1, }, // Octillery
            new EncounterStatic8 { Species = 612, Level = 60, Location =  -1, Ability = 1, }, // Haxorus
            new EncounterStatic8 { Species = 143, Level = 36, Location =  -1, }, // Snorlax
            new EncounterStatic8 { Species = 452, Level = 40, Location =  -1, }, // Drapion
            new EncounterStatic8 { Species = 561, Level = 36, Location =  -1, }, // Sigilyph
            new EncounterStatic8 { Species = 534, Level = 55, Location =  -1, Ability = 1, }, // Conkeldurr
            new EncounterStatic8 { Species = 320, Level = 56, Location =  -1, }, // Wailmer
            new EncounterStatic8 { Species = 561, Level = 40, Location =  -1, }, // Sigilyph
            new EncounterStatic8 { Species = 569, Level = 40, Location =  -1, }, // Garbodor
            new EncounterStatic8 { Species = 743, Level = 40, Location =  -1, }, // Ribombee
            new EncounterStatic8 { Species = 475, Level = 60, Location =  -1, }, // Gallade
            new EncounterStatic8 { Species = 264, Level = 40, Location =  -1,     Form = 01, }, // Linoone
            new EncounterStatic8 { Species = 606, Level = 42, Location =  -1, }, // Beheeyem
            new EncounterStatic8 { Species = 715, Level = 50, Location =  -1, }, // Noivern
            new EncounterStatic8 { Species = 537, Level = 46, Location =  -1, }, // Seismitoad
            new EncounterStatic8 { Species = 768, Level = 50, Location =  -1, }, // Golisopod
            new EncounterStatic8 { Species = 760, Level = 42, Location =  -1, }, // Bewear
            new EncounterStatic8 { Species = 820, Level = 42, Location =  -1, }, // Greedent
            new EncounterStatic8 { Species = 598, Level = 40, Location =  -1, }, // Ferrothorn
            new EncounterStatic8 { Species = 344, Level = 42, Location =  -1, }, // Claydol
            new EncounterStatic8 { Species = 477, Level = 60, Location =  -1, }, // Dusknoir
            new EncounterStatic8 { Species = 623, Level = 43, Location =  -1, }, // Golurk
            new EncounterStatic8 { Species = 561, Level = 40, Location =  -1, }, // Sigilyph
            new EncounterStatic8 { Species = 558, Level = 34, Location =  -1, }, // Crustle
            new EncounterStatic8 { Species = 112, Level = 41, Location =  -1, }, // Rhydon
            new EncounterStatic8 { Species = 763, Level = 36, Location =  -1, }, // Tsareena
            new EncounterStatic8 { Species = 750, Level = 41, Location =  -1, }, // Mudsdale
            new EncounterStatic8 { Species = 185, Level = 41, Location =  -1, }, // Sudowoodo
            new EncounterStatic8 { Species = 437, Level = 41, Location =  -1, }, // Bronzong
            new EncounterStatic8 { Species = 248, Level = 60, Location =  -1, }, // Tyranitar
            new EncounterStatic8 { Species = 784, Level = 60, Location =  -1, Ability = 1, }, // Kommo-o
            new EncounterStatic8 { Species = 213, Level = 34, Location =  -1, }, // Shuckle
            new EncounterStatic8 { Species = 330, Level = 51, Location =  -1, }, // Flygon
            new EncounterStatic8 { Species = 526, Level = 51, Location =  -1, }, // Gigalith
            new EncounterStatic8 { Species = 423, Level = 56, Location =  -1,     Form = 01, }, // Gastrodon
            new EncounterStatic8 { Species = 208, Level = 50, Location =  -1, }, // Steelix
            new EncounterStatic8 { Species = 068, Level = 60, Location =  -1, Ability = 1, }, // Machamp
            new EncounterStatic8 { Species = 182, Level = 41, Location =  -1, }, // Bellossom
            new EncounterStatic8 { Species = 521, Level = 41, Location =  -1, }, // Unfezant
            new EncounterStatic8 { Species = 701, Level = 36, Location =  -1, }, // Hawlucha
            new EncounterStatic8 { Species = 094, Level = 60, Location =  -1, }, // Gengar
            new EncounterStatic8 { Species = 823, Level = 39, Location =  -1, }, // Corviknight
            new EncounterStatic8 { Species = 573, Level = 46, Location =  -1, }, // Cinccino
            new EncounterStatic8 { Species = 826, Level = 41, Location =  -1, }, // Orbeetle
            new EncounterStatic8 { Species = 834, Level = 36, Location =  -1, }, // Drednaw
            new EncounterStatic8 { Species = 680, Level = 56, Location =  -1, }, // Doublade
            new EncounterStatic8 { Species = 711, Level = 41, Location =  -1, }, // Gourgeist
            new EncounterStatic8 { Species = 600, Level = 46, Location =  -1, }, // Klang
            new EncounterStatic8 { Species = 045, Level = 41, Location =  -1, }, // Vileplume
            new EncounterStatic8 { Species = 823, Level = 38, Location =  -1, }, // Corviknight
            new EncounterStatic8 { Species = 130, Level = 60, Location =  -1, }, // Gyarados
            new EncounterStatic8 { Species = 853, Level = 56, Location =  -1, }, // Grapploct
            new EncounterStatic8 { Species = 282, Level = 60, Location =  -1, }, // Gardevoir
            new EncounterStatic8 { Species = 470, Level = 56, Location =  -1, }, // Leafeon
            new EncounterStatic8 { Species = 510, Level = 31, Location =  -1, }, // Liepard
            new EncounterStatic8 { Species = 832, Level = 65, Location =  -1, }, // Dubwool
            new EncounterStatic8 { Species = 826, Level = 65, Location =  -1, }, // Orbeetle
            new EncounterStatic8 { Species = 823, Level = 65, Location =  -1, }, // Corviknight
            new EncounterStatic8 { Species = 110, Level = 65, Location =  -1,     Form = 01, }, // Weezing
            new EncounterStatic8 { Species = 834, Level = 65, Location =  -1, }, // Drednaw
            new EncounterStatic8 { Species = 845, Level = 65, Location =  -1, }, // Cramorant
            new EncounterStatic8 { Species = 828, Level = 65, Location =  -1, }, // Thievul
            new EncounterStatic8 { Species = 884, Level = 65, Location =  -1, }, // Duraludon
            new EncounterStatic8 { Species = 836, Level = 65, Location =  -1, }, // Boltund
            new EncounterStatic8 { Species = 830, Level = 65, Location =  -1, }, // Eldegoss
            new EncounterStatic8 { Species = 862, Level = 65, Location =  -1, }, // Obstagoon
            new EncounterStatic8 { Species = 861, Level = 65, Location =  -1, Gender = 0, }, // Grimmsnarl
            new EncounterStatic8 { Species = 844, Level = 65, Location =  -1, }, // Sandaconda
            new EncounterStatic8 { Species = 863, Level = 65, Location =  -1, }, // Perrserker
            new EncounterStatic8 { Species = 879, Level = 65, Location =  -1, }, // Copperajah
            new EncounterStatic8 { Species = 839, Level = 65, Location =  -1, }, // Coalossal
            new EncounterStatic8 { Species = 858, Level = 65, Location =  -1, Gender = 1 }, // Hatterene
            new EncounterStatic8 { Species = 279, Level = 26, Location =  -1, }, // Pelipper
            new EncounterStatic8 { Species = 310, Level = 26, Location =  -1, }, // Manectric
            new EncounterStatic8 { Species = 660, Level = 26, Location =  -1, }, // Diggersby
            new EncounterStatic8 { Species = 281, Level = 26, Location =  -1, }, // Kirlia
            new EncounterStatic8 { Species = 025, Level = 15, Location =  -1, }, // Pikachu
            new EncounterStatic8 { Species = 439, Level = 15, Location =  -1, }, // Mime Jr.
            new EncounterStatic8 { Species = 221, Level = 33, Location =  -1, }, // Piloswine
            new EncounterStatic8 { Species = 558, Level = 34, Location =  -1, }, // Crustle
            new EncounterStatic8 { Species = 282, Level = 32, Location =  -1, }, // Gardevoir
            new EncounterStatic8 { Species = 537, Level = 36, Location =  -1, }, // Seismitoad
            new EncounterStatic8 { Species = 583, Level = 36, Location =  -1, }, // Vanillish
            new EncounterStatic8 { Species = 344, Level = 36, Location =  -1, }, // Claydol
            new EncounterStatic8 { Species = 093, Level = 34, Location =  -1, }, // Haunter
            new EncounterStatic8 { Species = 356, Level = 40, Location =  -1, }, // Dusclops
            new EncounterStatic8 { Species = 362, Level = 40, Location =  -1, }, // Glalie
            new EncounterStatic8 { Species = 279, Level = 28, Location =  -1, }, // Pelipper
            new EncounterStatic8 { Species = 536, Level = 28, Location =  -1, }, // Palpitoad
            new EncounterStatic8 { Species = 660, Level = 28, Location =  -1, }, // Diggersby
            new EncounterStatic8 { Species = 221, Level = 36, Location =  -1, }, // Piloswine
            new EncounterStatic8 { Species = 750, Level = 36, Location =  -1, }, // Mudsdale
            new EncounterStatic8 { Species = 437, Level = 36, Location =  -1, }, // Bronzong
            new EncounterStatic8 { Species = 536, Level = 34, Location =  -1, }, // Palpitoad
            new EncounterStatic8 { Species = 279, Level = 26, Location =  -1, }, // Pelipper
            new EncounterStatic8 { Species = 093, Level = 31, Location =  -1, }, // Haunter
            new EncounterStatic8 { Species = 221, Level = 33, Location =  -1, }, // Piloswine
            new EncounterStatic8 { Species = 558, Level = 34, Location =  -1, }, // Crustle
            new EncounterStatic8 { Species = 067, Level = 31, Location =  -1, }, // Machoke
            new EncounterStatic8 { Species = 426, Level = 31, Location =  -1, }, // Drifblim
            new EncounterStatic8 { Species = 435, Level = 36, Location =  -1, }, // Skuntank
            new EncounterStatic8 { Species = 537, Level = 36, Location =  -1, }, // Seismitoad
            new EncounterStatic8 { Species = 583, Level = 36, Location =  -1, }, // Vanillish
            new EncounterStatic8 { Species = 426, Level = 36, Location =  -1, }, // Drifblim
            new EncounterStatic8 { Species = 437, Level = 46, Location =  -1, }, // Bronzong
            new EncounterStatic8 { Species = 460, Level = 46, Location =  -1, }, // Abomasnow
            new EncounterStatic8 { Species = 750, Level = 46, Location =  -1, }, // Mudsdale
            new EncounterStatic8 { Species = 623, Level = 46, Location =  -1, }, // Golurk
            new EncounterStatic8 { Species = 356, Level = 46, Location =  -1, }, // Dusclops
            new EncounterStatic8 { Species = 518, Level = 46, Location =  -1, }, // Musharna
            new EncounterStatic8 { Species = 362, Level = 46, Location =  -1, }, // Glalie
            new EncounterStatic8 { Species = 596, Level = 46, Location =  -1, }, // Galvantula
            new EncounterStatic8 { Species = 584, Level = 47, Location =  -1, }, // Vanilluxe
            new EncounterStatic8 { Species = 537, Level = 60, Location =  -1, }, // Seismitoad
            new EncounterStatic8 { Species = 460, Level = 60, Location =  -1, }, // Abomasnow
            new EncounterStatic8 { Species = 036, Level = 36, Location =  -1, }, // Clefable
            new EncounterStatic8 { Species = 743, Level = 40, Location =  -1, }, // Ribombee
            new EncounterStatic8 { Species = 112, Level = 55, Location =  -1, }, // Rhydon
            new EncounterStatic8 { Species = 823, Level = 40, Location =  -1, }, // Corviknight
            new EncounterStatic8 { Species = 760, Level = 40, Location =  -1, }, // Bewear
            new EncounterStatic8 { Species = 614, Level = 60, Location =  -1, }, // Beartic
            new EncounterStatic8 { Species = 461, Level = 60, Location =  -1, }, // Weavile
            new EncounterStatic8 { Species = 518, Level = 60, Location =  -1, }, // Musharna
            new EncounterStatic8 { Species = 437, Level = 42, Location =  -1, }, // Bronzong
            new EncounterStatic8 { Species = 344, Level = 42, Location =  -1, }, // Claydol
            new EncounterStatic8 { Species = 452, Level = 50, Location =  -1, }, // Drapion
            new EncounterStatic8 { Species = 164, Level = 50, Location =  -1, }, // Noctowl
            new EncounterStatic8 { Species = 760, Level = 46, Location =  -1, }, // Bewear
            new EncounterStatic8 { Species = 675, Level = 42, Location =  -1, }, // Pangoro
            new EncounterStatic8 { Species = 584, Level = 50, Location =  -1, }, // Vanilluxe
            new EncounterStatic8 { Species = 112, Level = 50, Location =  -1, }, // Rhydon
            new EncounterStatic8 { Species = 778, Level = 50, Location =  -1, }, // Mimikyu
            new EncounterStatic8 { Species = 521, Level = 40, Location =  -1, }, // Unfezant
            new EncounterStatic8 { Species = 752, Level = 34, Location =  -1, }, // Araquanid
            new EncounterStatic8 { Species = 537, Level = 41, Location =  -1, }, // Seismitoad
            new EncounterStatic8 { Species = 435, Level = 41, Location =  -1, }, // Skuntank
            new EncounterStatic8 { Species = 221, Level = 41, Location =  -1, }, // Piloswine
            new EncounterStatic8 { Species = 356, Level = 41, Location =  -1, }, // Dusclops
            new EncounterStatic8 { Species = 344, Level = 41, Location =  -1, }, // Claydol
            new EncounterStatic8 { Species = 689, Level = 60, Location =  -1, }, // Barbaracle
            new EncounterStatic8 { Species = 561, Level = 51, Location =  -1, }, // Sigilyph
            new EncounterStatic8 { Species = 623, Level = 51, Location =  -1, }, // Golurk
            new EncounterStatic8 { Species = 537, Level = 60, Location =  -1, }, // Seismitoad
            new EncounterStatic8 { Species = 460, Level = 60, Location =  -1, }, // Abomasnow
            new EncounterStatic8 { Species = 045, Level = 41, Location =  -1, }, // Vileplume
            new EncounterStatic8 { Species = 178, Level = 41, Location =  -1, }, // Xatu
            new EncounterStatic8 { Species = 768, Level = 60, Location =  -1, }, // Golisopod
            new EncounterStatic8 { Species = 614, Level = 60, Location =  -1, }, // Beartic
            new EncounterStatic8 { Species = 530, Level = 46, Location =  -1, }, // Excadrill
            new EncounterStatic8 { Species = 362, Level = 46, Location =  -1, }, // Glalie
            new EncounterStatic8 { Species = 537, Level = 46, Location =  -1, }, // Seismitoad
            new EncounterStatic8 { Species = 681, Level = 58, Location =  -1, }, // Aegislash
            new EncounterStatic8 { Species = 601, Level = 49, Location =  -1, }, // Klinklang
            new EncounterStatic8 { Species = 407, Level = 41, Location =  -1, }, // Roserade
            new EncounterStatic8 { Species = 460, Level = 41, Location =  -1, }, // Abomasnow
            new EncounterStatic8 { Species = 350, Level = 60, Location =  -1, Gender = 0, Ability = 1, }, // Milotic
            new EncounterStatic8 { Species = 112, Level = 60, Location =  -1, }, // Rhydon
            new EncounterStatic8 { Species = 609, Level = 60, Location =  -1, }, // Chandelure
            new EncounterStatic8 { Species = 713, Level = 60, Location =  -1, }, // Avalugg
            new EncounterStatic8 { Species = 756, Level = 60, Location =  -1, }, // Shiinotic
            new EncounterStatic8 { Species = 134, Level = 56, Location =  -1, }, // Vaporeon
            new EncounterStatic8 { Species = 135, Level = 56, Location =  -1, }, // Jolteon
            new EncounterStatic8 { Species = 196, Level = 56, Location =  -1, }, // Espeon
            new EncounterStatic8 { Species = 471, Level = 56, Location =  -1, }, // Glaceon
            new EncounterStatic8 { Species = 136, Level = 56, Location =  -1, }, // Flareon
            new EncounterStatic8 { Species = 197, Level = 56, Location =  -1, }, // Umbreon
            new EncounterStatic8 { Species = 700, Level = 56, Location =  -1, }, // Sylveon
            #endregion
        };

        #region Nest
        private const int Nest00 = -100000;
        private const int Nest01 = -100001;
        private const int Nest02 = -100002;
        private const int Nest03 = -100003;
        private const int Nest04 = -100004;
        private const int Nest05 = -100005;
        private const int Nest06 = -100006;
        private const int Nest07 = -100007;
        private const int Nest08 = -100008;
        private const int Nest09 = -100009;
        private const int Nest10 = -100010;
        private const int Nest11 = -100011;
        private const int Nest12 = -100012;
        private const int Nest13 = -100013;
        private const int Nest14 = -100014;
        private const int Nest15 = -100015;
        private const int Nest16 = -100016;
        private const int Nest17 = -100017;
        private const int Nest18 = -100018;
        private const int Nest19 = -100019;
        private const int Nest20 = -100020;
        private const int Nest21 = -100021;
        private const int Nest22 = -100022;
        private const int Nest23 = -100023;
        private const int Nest24 = -100024;
        private const int Nest25 = -100025;
        private const int Nest26 = -100026;
        private const int Nest27 = -100027;
        private const int Nest28 = -100028;
        private const int Nest29 = -100029;
        private const int Nest30 = -100030;
        private const int Nest31 = -100031;
        private const int Nest32 = -100032;
        private const int Nest33 = -100033;
        private const int Nest34 = -100034;
        private const int Nest35 = -100035;
        private const int Nest36 = -100036;
        private const int Nest37 = -100037;
        private const int Nest38 = -100038;
        private const int Nest39 = -100039;
        private const int Nest40 = -100040;
        private const int Nest41 = -100041;
        private const int Nest42 = -100042;
        private const int Nest43 = -100043;
        private const int Nest44 = -100044;
        private const int Nest45 = -100045;
        private const int Nest46 = -100046;
        private const int Nest47 = -100047;
        private const int Nest48 = -100048;
        private const int Nest49 = -100049;
        private const int Nest50 = -100050;
        private const int Nest51 = -100051;
        private const int Nest52 = -100052;
        private const int Nest53 = -100053;
        private const int Nest54 = -100054;
        private const int Nest55 = -100055;
        private const int Nest56 = -100056;
        private const int Nest57 = -100057;
        private const int Nest58 = -100058;
        private const int Nest59 = -100059;
        private const int Nest60 = -100060;
        private const int Nest61 = -100061;
        private const int Nest62 = -100062;
        private const int Nest63 = -100063;
        private const int Nest64 = -100064;
        private const int Nest65 = -100065;
        private const int Nest66 = -100066;
        private const int Nest67 = -100067;
        private const int Nest68 = -100068;
        private const int Nest69 = -100069;
        private const int Nest70 = -100070;
        private const int Nest71 = -100071;
        private const int Nest72 = -100072;
        private const int Nest73 = -100073;
        private const int Nest74 = -100074;
        private const int Nest75 = -100075;
        private const int Nest76 = -100076;
        private const int Nest77 = -100077;
        private const int Nest78 = -100078;
        private const int Nest79 = -100079;
        private const int Nest80 = -100080;
        private const int Nest81 = -100081;
        private const int Nest82 = -100082;
        private const int Nest83 = -100083;
        private const int Nest84 = -100084;
        private const int Nest85 = -100085;
        private const int Nest86 = -100086;
        private const int Nest87 = -100087;
        private const int Nest88 = -100088;
        private const int Nest89 = -100089;
        private const int Nest90 = -100090;
        private const int Nest91 = -100091;
        private const int Nest92 = -100092;

        internal static readonly EncounterStatic8N[] Nest_Common =
        {
            new EncounterStatic8N(Nest00,0,0,1) { Species = 236, Ability =  4 }, // Tyrogue
            new EncounterStatic8N(Nest00,0,0,1) { Species = 066, Ability =  4 }, // Machop
            new EncounterStatic8N(Nest00,0,1,1) { Species = 532, Ability =  4 }, // Timburr
            new EncounterStatic8N(Nest00,1,2,2) { Species = 067, Ability =  4 }, // Machoke
            new EncounterStatic8N(Nest00,1,2,2) { Species = 533, Ability =  4 }, // Gurdurr
            new EncounterStatic8N(Nest00,4,4,4) { Species = 068, Ability = -1 }, // Machamp
            new EncounterStatic8N(Nest01,0,0,1) { Species = 280, Ability =  4 }, // Ralts
            new EncounterStatic8N(Nest01,0,0,1) { Species = 517, Ability =  4 }, // Munna
            new EncounterStatic8N(Nest01,0,1,1) { Species = 677, Ability =  4 }, // Espurr
            new EncounterStatic8N(Nest01,0,1,1) { Species = 605, Ability =  4 }, // Elgyem
            new EncounterStatic8N(Nest01,1,2,2) { Species = 281, Ability =  4 }, // Kirlia
            new EncounterStatic8N(Nest01,2,4,4) { Species = 518, Ability =  4 }, // Musharna
            new EncounterStatic8N(Nest01,4,4,4) { Species = 282, Ability = -1 }, // Gardevoir
            new EncounterStatic8N(Nest02,0,0,1) { Species = 438, Ability =  4 }, // Bonsly
            new EncounterStatic8N(Nest02,0,1,1) { Species = 557, Ability =  4 }, // Dwebble
            new EncounterStatic8N(Nest02,1,2,2) { Species = 111, Ability =  4 }, // Rhyhorn
            new EncounterStatic8N(Nest02,1,2,2) { Species = 525, Ability =  4 }, // Boldore
            new EncounterStatic8N(Nest02,2,3,3) { Species = 689, Ability =  4 }, // Barbaracle
            new EncounterStatic8N(Nest02,2,4,4) { Species = 112, Ability =  4 }, // Rhydon
            new EncounterStatic8N(Nest02,2,4,4) { Species = 185, Ability =  4 }, // Sudowoodo
            new EncounterStatic8N(Nest02,4,4,4) { Species = 213, Ability = -1 }, // Shuckle
            new EncounterStatic8N(Nest03,0,0,1) { Species = 010, Ability =  4 }, // Caterpie
            new EncounterStatic8N(Nest03,0,0,1) { Species = 736, Ability =  4 }, // Grubbin
            new EncounterStatic8N(Nest03,0,1,1) { Species = 290, Ability =  4 }, // Nincada
            new EncounterStatic8N(Nest03,0,1,1) { Species = 595, Ability =  4 }, // Joltik
            new EncounterStatic8N(Nest03,1,2,2) { Species = 011, Ability =  4 }, // Metapod
            new EncounterStatic8N(Nest03,1,2,2) { Species = 632, Ability =  4 }, // Durant
            new EncounterStatic8N(Nest03,2,3,3) { Species = 737, Ability =  4 }, // Charjabug
            new EncounterStatic8N(Nest03,2,4,4) { Species = 291, Ability =  4 }, // Ninjask
            new EncounterStatic8N(Nest03,2,4,4) { Species = 012, Ability =  4 }, // Butterfree
            new EncounterStatic8N(Nest03,3,4,4) { Species = 596, Ability = -1 }, // Galvantula
            new EncounterStatic8N(Nest03,4,4,4) { Species = 738, Ability = -1 }, // Vikavolt
            new EncounterStatic8N(Nest03,4,4,4) { Species = 632, Ability = -1 }, // Durant
            new EncounterStatic8N(Nest04,0,0,1) { Species = 010, Ability =  4 }, // Caterpie
            new EncounterStatic8N(Nest04,0,0,1) { Species = 415, Ability =  4 }, // Combee
            new EncounterStatic8N(Nest04,0,1,1) { Species = 742, Ability =  4 }, // Cutiefly
            new EncounterStatic8N(Nest04,0,1,1) { Species = 824, Ability =  4 }, // Blipbug
            new EncounterStatic8N(Nest04,1,2,2) { Species = 595, Ability =  4 }, // Joltik
            new EncounterStatic8N(Nest04,1,2,2) { Species = 011, Ability =  4 }, // Metapod
            new EncounterStatic8N(Nest04,2,3,3) { Species = 825, Ability =  4 }, // Dottler
            new EncounterStatic8N(Nest04,2,4,4) { Species = 596, Ability =  4 }, // Galvantula
            new EncounterStatic8N(Nest04,2,4,4) { Species = 012, Ability =  4 }, // Butterfree
            new EncounterStatic8N(Nest04,3,4,4) { Species = 743, Ability = -1 }, // Ribombee
            new EncounterStatic8N(Nest04,4,4,4) { Species = 416, Ability = -1 }, // Vespiquen
            new EncounterStatic8N(Nest04,4,4,4) { Species = 826, Ability = -1 }, // Orbeetle
            new EncounterStatic8N(Nest05,0,0,1) { Species = 092, Ability =  4 }, // Gastly
            new EncounterStatic8N(Nest05,0,0,1) { Species = 355, Ability =  4 }, // Duskull
            new EncounterStatic8N(Nest05,0,1,1) { Species = 425, Ability =  4 }, // Drifloon
            new EncounterStatic8N(Nest05,0,1,1) { Species = 708, Ability =  4 }, // Phantump
            new EncounterStatic8N(Nest05,0,1,1) { Species = 592, Ability =  4 }, // Frillish
            new EncounterStatic8N(Nest05,1,2,2) { Species = 710, Ability =  4 }, // Pumpkaboo
            new EncounterStatic8N(Nest05,2,3,3) { Species = 093, Ability =  4 }, // Haunter
            new EncounterStatic8N(Nest05,2,4,4) { Species = 356, Ability =  4 }, // Dusclops
            new EncounterStatic8N(Nest05,2,4,4) { Species = 426, Ability =  4 }, // Drifblim
            new EncounterStatic8N(Nest05,3,4,4) { Species = 709, Ability = -1 }, // Trevenant
            new EncounterStatic8N(Nest05,4,4,4) { Species = 711, Ability = -1 }, // Gourgeist
            new EncounterStatic8N(Nest05,4,4,4) { Species = 593, Ability = -1 }, // Jellicent
            new EncounterStatic8N(Nest06,0,0,1) { Species = 129, Ability =  4 }, // Magikarp
            new EncounterStatic8N(Nest06,0,0,1) { Species = 458, Ability =  4 }, // Mantyke
            new EncounterStatic8N(Nest06,1,2,2) { Species = 320, Ability =  4 }, // Wailmer
            new EncounterStatic8N(Nest06,2,3,3) { Species = 224, Ability =  4 }, // Octillery
            new EncounterStatic8N(Nest06,2,4,4) { Species = 226, Ability =  4 }, // Mantine
            new EncounterStatic8N(Nest06,2,4,4) { Species = 171, Ability =  4 }, // Lanturn
            new EncounterStatic8N(Nest06,3,4,4) { Species = 321, Ability = -1 }, // Wailord
            new EncounterStatic8N(Nest06,4,4,4) { Species = 746, Ability = -1 }, // Wishiwashi
            new EncounterStatic8N(Nest06,4,4,4) { Species = 130, Ability = -1 }, // Gyarados
            new EncounterStatic8N(Nest07,0,0,1) { Species = 833, Ability =  4 }, // Chewtle
            new EncounterStatic8N(Nest07,0,0,1) { Species = 846, Ability =  4 }, // Arrokuda
            new EncounterStatic8N(Nest07,0,1,1) { Species = 422, Ability =  4, Form = 1 }, // Shellos-1
            new EncounterStatic8N(Nest07,0,1,1) { Species = 751, Ability =  4 }, // Dewpider
            new EncounterStatic8N(Nest07,1,2,2) { Species = 320, Ability =  4 }, // Wailmer
            new EncounterStatic8N(Nest07,2,3,3) { Species = 746, Ability =  4 }, // Wishiwashi
            new EncounterStatic8N(Nest07,2,4,4) { Species = 834, Ability =  4 }, // Drednaw
            new EncounterStatic8N(Nest07,2,4,4) { Species = 847, Ability =  4 }, // Barraskewda
            new EncounterStatic8N(Nest07,3,4,4) { Species = 752, Ability = -1 }, // Araquanid
            new EncounterStatic8N(Nest07,4,4,4) { Species = 423, Ability = -1, Form = 1 }, // Gastrodon-1
            new EncounterStatic8N(Nest07,4,4,4) { Species = 321, Ability = -1 }, // Wailord
            new EncounterStatic8N(Nest08,0,0,1) { Species = 833, Ability =  4 }, // Chewtle
            new EncounterStatic8N(Nest08,0,0,1) { Species = 194, Ability =  4 }, // Wooper
            new EncounterStatic8N(Nest08,0,1,1) { Species = 535, Ability =  4 }, // Tympole
            new EncounterStatic8N(Nest08,0,1,1) { Species = 341, Ability =  4 }, // Corphish
            new EncounterStatic8N(Nest08,1,2,2) { Species = 536, Ability =  4 }, // Palpitoad
            new EncounterStatic8N(Nest08,2,3,3) { Species = 834, Ability =  4 }, // Drednaw
            new EncounterStatic8N(Nest08,2,4,4) { Species = 195, Ability =  4 }, // Quagsire
            new EncounterStatic8N(Nest08,2,4,4) { Species = 771, Ability =  4 }, // Pyukumuku
            new EncounterStatic8N(Nest08,3,4,4) { Species = 091, Ability = -1 }, // Cloyster
            new EncounterStatic8N(Nest08,4,4,4) { Species = 537, Ability = -1 }, // Seismitoad
            new EncounterStatic8N(Nest08,4,4,4) { Species = 342, Ability = -1 }, // Crawdaunt
            new EncounterStatic8N(Nest09,0,0,1) { Species = 236, Ability =  4 }, // Tyrogue
            new EncounterStatic8N(Nest09,0,0,1) { Species = 759, Ability =  4 }, // Stufful
            new EncounterStatic8N(Nest09,0,1,1) { Species = 852, Ability =  4 }, // Clobbopus
            new EncounterStatic8N(Nest09,0,1,1) { Species = 674, Ability =  4 }, // Pancham
            new EncounterStatic8N(Nest09,2,4,4) { Species = 760, Ability =  4 }, // Bewear
            new EncounterStatic8N(Nest09,2,4,4) { Species = 675, Ability =  4 }, // Pangoro
            new EncounterStatic8N(Nest09,2,4,4) { Species = 701, Ability =  4 }, // Hawlucha
            new EncounterStatic8N(Nest09,4,4,4) { Species = 853, Ability = -1 }, // Grapploct
            new EncounterStatic8N(Nest09,4,4,4) { Species = 870, Ability = -1 }, // Falinks
            new EncounterStatic8N(Nest10,0,0,1) { Species = 599, Ability =  4 }, // Klink
            new EncounterStatic8N(Nest10,0,0,1) { Species = 052, Ability =  4, Form = 2 }, // Meowth-2
            new EncounterStatic8N(Nest10,0,1,1) { Species = 436, Ability =  4 }, // Bronzor
            new EncounterStatic8N(Nest10,0,1,1) { Species = 597, Ability =  4 }, // Ferroseed
            new EncounterStatic8N(Nest10,1,1,2) { Species = 624, Ability =  4 }, // Pawniard
            new EncounterStatic8N(Nest10,1,2,2) { Species = 878, Ability =  4 }, // Cufant
            new EncounterStatic8N(Nest10,2,4,4) { Species = 600, Ability =  4 }, // Klang
            new EncounterStatic8N(Nest10,2,4,4) { Species = 863, Ability =  4 }, // Perrserker
            new EncounterStatic8N(Nest10,2,4,4) { Species = 437, Ability =  4 }, // Bronzong
            new EncounterStatic8N(Nest10,3,4,4) { Species = 625, Ability = -1 }, // Bisharp
            new EncounterStatic8N(Nest10,4,4,4) { Species = 601, Ability = -1 }, // Klinklang
            new EncounterStatic8N(Nest10,4,4,4) { Species = 879, Ability = -1 }, // Copperajah
            new EncounterStatic8N(Nest11,0,0,1) { Species = 599, Ability =  4 }, // Klink
            new EncounterStatic8N(Nest11,0,0,1) { Species = 436, Ability =  4 }, // Bronzor
            new EncounterStatic8N(Nest11,0,1,1) { Species = 597, Ability =  4 }, // Ferroseed
            new EncounterStatic8N(Nest11,0,1,1) { Species = 624, Ability =  4 }, // Pawniard
            new EncounterStatic8N(Nest11,1,1,2) { Species = 599, Ability =  4 }, // Klink
            new EncounterStatic8N(Nest11,1,2,2) { Species = 436, Ability =  4 }, // Bronzor
            new EncounterStatic8N(Nest11,2,4,4) { Species = 208, Ability =  4 }, // Steelix
            new EncounterStatic8N(Nest11,2,4,4) { Species = 598, Ability =  4 }, // Ferrothorn
            new EncounterStatic8N(Nest11,2,4,4) { Species = 437, Ability =  4 }, // Bronzong
            new EncounterStatic8N(Nest11,3,4,4) { Species = 625, Ability = -1 }, // Bisharp
            new EncounterStatic8N(Nest11,4,4,4) { Species = 777, Ability = -1 }, // Togedemaru
            new EncounterStatic8N(Nest12,0,0,1) { Species = 439, Ability =  4 }, // Mime Jr.
            new EncounterStatic8N(Nest12,0,0,1) { Species = 824, Ability =  4 }, // Blipbug
            new EncounterStatic8N(Nest12,2,3,3) { Species = 561, Ability =  4 }, // Sigilyph
            new EncounterStatic8N(Nest12,2,3,3) { Species = 178, Ability =  4 }, // Xatu
            new EncounterStatic8N(Nest12,4,4,4) { Species = 858, Ability = -1 }, // Hatterene
            new EncounterStatic8N(Nest13,0,0,1) { Species = 439, Ability =  4 }, // Mime Jr.
            new EncounterStatic8N(Nest13,0,0,1) { Species = 360, Ability =  4 }, // Wynaut
            new EncounterStatic8N(Nest13,0,1,1) { Species = 177, Ability =  4 }, // Natu
            new EncounterStatic8N(Nest13,0,1,1) { Species = 343, Ability =  4 }, // Baltoy
            new EncounterStatic8N(Nest13,1,1,1) { Species = 436, Ability =  4 }, // Bronzor
            new EncounterStatic8N(Nest13,1,3,3) { Species = 122, Ability =  4, Form = 1 }, // Mr. Mime-1
            new EncounterStatic8N(Nest13,2,3,3) { Species = 561, Ability =  4 }, // Sigilyph
            new EncounterStatic8N(Nest13,2,3,3) { Species = 178, Ability =  4 }, // Xatu
            new EncounterStatic8N(Nest13,3,4,4) { Species = 344, Ability = -1 }, // Claydol
            new EncounterStatic8N(Nest13,4,4,4) { Species = 866, Ability = -1 }, // Mr. Rime
            new EncounterStatic8N(Nest13,4,4,4) { Species = 202, Ability = -1 }, // Wobbuffet
            new EncounterStatic8N(Nest14,0,0,1) { Species = 837, Ability =  4 }, // Rolycoly
            new EncounterStatic8N(Nest14,0,1,1) { Species = 688, Ability =  4 }, // Binacle
            new EncounterStatic8N(Nest14,1,1,1) { Species = 838, Ability =  4 }, // Carkol
            new EncounterStatic8N(Nest14,1,2,2) { Species = 525, Ability =  4 }, // Boldore
            new EncounterStatic8N(Nest14,2,3,3) { Species = 558, Ability =  4 }, // Crustle
            new EncounterStatic8N(Nest14,2,4,4) { Species = 689, Ability =  4 }, // Barbaracle
            new EncounterStatic8N(Nest14,4,4,4) { Species = 464, Ability = -1 }, // Rhyperior
            new EncounterStatic8N(Nest15,0,0,1) { Species = 050, Ability =  4 }, // Diglett
            new EncounterStatic8N(Nest15,0,0,1) { Species = 749, Ability =  4 }, // Mudbray
            new EncounterStatic8N(Nest15,0,1,1) { Species = 290, Ability =  4 }, // Nincada
            new EncounterStatic8N(Nest15,0,1,1) { Species = 529, Ability =  4 }, // Drilbur
            new EncounterStatic8N(Nest15,1,1,1) { Species = 095, Ability =  4 }, // Onix
            new EncounterStatic8N(Nest15,1,2,2) { Species = 339, Ability =  4 }, // Barboach
            new EncounterStatic8N(Nest15,2,3,3) { Species = 208, Ability =  4 }, // Steelix
            new EncounterStatic8N(Nest15,2,4,4) { Species = 340, Ability =  4 }, // Whiscash
            new EncounterStatic8N(Nest15,2,4,4) { Species = 660, Ability =  4 }, // Diggersby
            new EncounterStatic8N(Nest15,3,4,4) { Species = 051, Ability = -1 }, // Dugtrio
            new EncounterStatic8N(Nest15,4,4,4) { Species = 530, Ability = -1 }, // Excadrill
            new EncounterStatic8N(Nest15,4,4,4) { Species = 750, Ability = -1 }, // Mudsdale
            new EncounterStatic8N(Nest16,0,0,1) { Species = 843, Ability =  4 }, // Silicobra
            new EncounterStatic8N(Nest16,0,0,1) { Species = 562, Ability =  4, Form = 1 }, // Yamask-1
            new EncounterStatic8N(Nest16,0,1,1) { Species = 449, Ability =  4 }, // Hippopotas
            new EncounterStatic8N(Nest16,1,2,2) { Species = 221, Ability =  4 }, // Piloswine
            new EncounterStatic8N(Nest16,4,4,4) { Species = 867, Ability =  4 }, // Runerigus
            new EncounterStatic8N(Nest16,4,4,4) { Species = 844, Ability = -1 }, // Sandaconda
            new EncounterStatic8N(Nest17,0,0,1) { Species = 850, Ability =  4 }, // Sizzlipede
            new EncounterStatic8N(Nest17,0,1,1) { Species = 757, Ability =  4 }, // Salandit
            new EncounterStatic8N(Nest17,0,1,1) { Species = 607, Ability =  4 }, // Litwick
            new EncounterStatic8N(Nest17,2,4,4) { Species = 324, Ability =  4 }, // Torkoal
            new EncounterStatic8N(Nest18,0,0,1) { Species = 757, Ability =  4 }, // Salandit
            new EncounterStatic8N(Nest18,0,1,1) { Species = 607, Ability =  4 }, // Litwick
            new EncounterStatic8N(Nest18,1,1,1) { Species = 757, Ability =  4 }, // Salandit
            new EncounterStatic8N(Nest18,4,4,4) { Species = 609, Ability = -1 }, // Chandelure
            new EncounterStatic8N(Nest19,0,0,1) { Species = 850, Ability =  4 }, // Sizzlipede
            new EncounterStatic8N(Nest19,0,1,1) { Species = 757, Ability =  4 }, // Salandit
            new EncounterStatic8N(Nest19,0,1,1) { Species = 607, Ability =  4 }, // Litwick
            new EncounterStatic8N(Nest19,2,4,4) { Species = 324, Ability =  4 }, // Torkoal
            new EncounterStatic8N(Nest19,3,4,4) { Species = 851, Ability = -1 }, // Centiskorch
            new EncounterStatic8N(Nest19,4,4,4) { Species = 839, Ability = -1 }, // Coalossal
            new EncounterStatic8N(Nest20,0,0,1) { Species = 582, Ability =  4 }, // Vanillite
            new EncounterStatic8N(Nest20,0,0,1) { Species = 220, Ability =  4 }, // Swinub
            new EncounterStatic8N(Nest20,0,1,1) { Species = 459, Ability =  4 }, // Snover
            new EncounterStatic8N(Nest20,0,1,1) { Species = 712, Ability =  4 }, // Bergmite
            new EncounterStatic8N(Nest20,1,1,1) { Species = 225, Ability =  4 }, // Delibird
            new EncounterStatic8N(Nest20,1,2,2) { Species = 583, Ability =  4 }, // Vanillish
            new EncounterStatic8N(Nest20,2,3,3) { Species = 221, Ability =  4 }, // Piloswine
            new EncounterStatic8N(Nest20,2,4,4) { Species = 713, Ability =  4 }, // Avalugg
            new EncounterStatic8N(Nest20,2,4,4) { Species = 460, Ability =  4 }, // Abomasnow
            new EncounterStatic8N(Nest20,3,4,4) { Species = 091, Ability = -1 }, // Cloyster
            new EncounterStatic8N(Nest20,4,4,4) { Species = 584, Ability = -1 }, // Vanilluxe
            new EncounterStatic8N(Nest20,4,4,4) { Species = 131, Ability = -1 }, // Lapras
            new EncounterStatic8N(Nest21,0,0,1) { Species = 220, Ability =  4 }, // Swinub
            new EncounterStatic8N(Nest21,0,0,1) { Species = 613, Ability =  4 }, // Cubchoo
            new EncounterStatic8N(Nest21,0,1,1) { Species = 872, Ability =  4 }, // Snom
            new EncounterStatic8N(Nest21,0,1,1) { Species = 215, Ability =  4 }, // Sneasel
            new EncounterStatic8N(Nest21,1,1,1) { Species = 122, Ability =  4, Form = 1 }, // Mr. Mime-1
            new EncounterStatic8N(Nest21,1,2,2) { Species = 221, Ability =  4 }, // Piloswine
            new EncounterStatic8N(Nest21,2,3,3) { Species = 091, Ability =  4 }, // Cloyster
            new EncounterStatic8N(Nest21,2,4,4) { Species = 614, Ability =  4 }, // Beartic
            new EncounterStatic8N(Nest21,2,4,4) { Species = 866, Ability =  4 }, // Mr. Rime
            new EncounterStatic8N(Nest21,3,4,4) { Species = 473, Ability = -1 }, // Mamoswine
            new EncounterStatic8N(Nest21,4,4,4) { Species = 873, Ability = -1 }, // Frosmoth
            new EncounterStatic8N(Nest21,4,4,4) { Species = 461, Ability = -1 }, // Weavile
            new EncounterStatic8N(Nest22,0,0,1) { Species = 361, Ability =  4 }, // Snorunt
            new EncounterStatic8N(Nest22,0,0,1) { Species = 872, Ability =  4 }, // Snom
            new EncounterStatic8N(Nest22,0,1,1) { Species = 215, Ability =  4 }, // Sneasel
            new EncounterStatic8N(Nest22,1,1,2) { Species = 122, Ability =  4, Form = 1 }, // Mr. Mime-1
            new EncounterStatic8N(Nest22,1,2,3) { Species = 459, Ability =  4 }, // Snover
            new EncounterStatic8N(Nest22,2,3,3) { Species = 460, Ability =  4 }, // Abomasnow
            new EncounterStatic8N(Nest22,2,4,4) { Species = 362, Ability =  4 }, // Glalie
            new EncounterStatic8N(Nest22,2,4,4) { Species = 866, Ability =  4 }, // Mr. Rime
            new EncounterStatic8N(Nest22,3,4,4) { Species = 873, Ability = -1 }, // Frosmoth
            new EncounterStatic8N(Nest22,4,4,4) { Species = 478, Ability = -1 }, // Froslass
            new EncounterStatic8N(Nest23,0,0,1) { Species = 172, Ability =  4 }, // Pichu
            new EncounterStatic8N(Nest23,0,0,1) { Species = 309, Ability =  4 }, // Electrike
            new EncounterStatic8N(Nest23,0,1,1) { Species = 595, Ability =  4 }, // Joltik
            new EncounterStatic8N(Nest23,0,1,1) { Species = 170, Ability =  4 }, // Chinchou
            new EncounterStatic8N(Nest23,1,1,2) { Species = 737, Ability =  4 }, // Charjabug
            new EncounterStatic8N(Nest23,1,2,3) { Species = 025, Ability =  4 }, // Pikachu
            new EncounterStatic8N(Nest23,2,3,3) { Species = 025, Ability =  4 }, // Pikachu
            new EncounterStatic8N(Nest23,2,4,4) { Species = 310, Ability =  4 }, // Manectric
            new EncounterStatic8N(Nest23,2,4,4) { Species = 171, Ability =  4 }, // Lanturn
            new EncounterStatic8N(Nest23,3,4,4) { Species = 596, Ability = -1 }, // Galvantula
            new EncounterStatic8N(Nest23,4,4,4) { Species = 738, Ability = -1 }, // Vikavolt
            new EncounterStatic8N(Nest23,4,4,4) { Species = 026, Ability = -1 }, // Raichu
            new EncounterStatic8N(Nest24,0,0,1) { Species = 835, Ability =  4 }, // Yamper
            new EncounterStatic8N(Nest24,0,0,1) { Species = 694, Ability =  4 }, // Helioptile
            new EncounterStatic8N(Nest24,0,1,1) { Species = 848, Ability =  4 }, // Toxel
            new EncounterStatic8N(Nest24,0,1,1) { Species = 170, Ability =  4 }, // Chinchou
            new EncounterStatic8N(Nest24,1,1,2) { Species = 025, Ability =  4 }, // Pikachu
            new EncounterStatic8N(Nest24,1,2,3) { Species = 171, Ability =  4 }, // Lanturn
            new EncounterStatic8N(Nest24,2,3,3) { Species = 836, Ability =  4 }, // Boltund
            new EncounterStatic8N(Nest24,2,4,4) { Species = 695, Ability =  4 }, // Heliolisk
            new EncounterStatic8N(Nest24,2,4,4) { Species = 849, Ability =  4 }, // Toxtricity
            new EncounterStatic8N(Nest24,3,4,4) { Species = 871, Ability = -1 }, // Pincurchin
            new EncounterStatic8N(Nest24,4,4,4) { Species = 777, Ability = -1 }, // Togedemaru
            new EncounterStatic8N(Nest24,4,4,4) { Species = 877, Ability = -1 }, // Morpeko
            new EncounterStatic8N(Nest25,0,0,1) { Species = 406, Ability =  4 }, // Budew
            new EncounterStatic8N(Nest25,0,1,1) { Species = 761, Ability =  4 }, // Bounsweet
            new EncounterStatic8N(Nest25,0,1,1) { Species = 043, Ability =  4 }, // Oddish
            new EncounterStatic8N(Nest25,1,2,3) { Species = 315, Ability =  4 }, // Roselia
            new EncounterStatic8N(Nest25,2,3,3) { Species = 044, Ability =  4 }, // Gloom
            new EncounterStatic8N(Nest25,2,4,4) { Species = 762, Ability =  4 }, // Steenee
            new EncounterStatic8N(Nest25,3,4,4) { Species = 763, Ability = -1 }, // Tsareena
            new EncounterStatic8N(Nest25,4,4,4) { Species = 045, Ability = -1 }, // Vileplume
            new EncounterStatic8N(Nest25,4,4,4) { Species = 182, Ability = -1 }, // Bellossom
            new EncounterStatic8N(Nest26,0,0,1) { Species = 406, Ability =  4 }, // Budew
            new EncounterStatic8N(Nest26,0,0,1) { Species = 829, Ability =  4 }, // Gossifleur
            new EncounterStatic8N(Nest26,0,1,1) { Species = 546, Ability =  4 }, // Cottonee
            new EncounterStatic8N(Nest26,0,1,1) { Species = 840, Ability =  4 }, // Applin
            new EncounterStatic8N(Nest26,1,1,2) { Species = 420, Ability =  4 }, // Cherubi
            new EncounterStatic8N(Nest26,1,2,2) { Species = 315, Ability =  4 }, // Roselia
            new EncounterStatic8N(Nest26,2,3,3) { Species = 597, Ability =  4 }, // Ferroseed
            new EncounterStatic8N(Nest26,2,4,4) { Species = 598, Ability =  4 }, // Ferrothorn
            new EncounterStatic8N(Nest26,2,4,4) { Species = 421, Ability =  4 }, // Cherrim
            new EncounterStatic8N(Nest26,3,4,4) { Species = 830, Ability = -1 }, // Eldegoss
            new EncounterStatic8N(Nest26,4,4,4) { Species = 547, Ability = -1 }, // Whimsicott
            new EncounterStatic8N(Nest27,0,0,1) { Species = 710, Ability =  4, Form = 1 }, // Pumpkaboo-1
            new EncounterStatic8N(Nest27,0,0,1) { Species = 708, Ability =  4 }, // Phantump
            new EncounterStatic8N(Nest27,0,1,1) { Species = 710, Ability =  4 }, // Pumpkaboo
            new EncounterStatic8N(Nest27,0,1,1) { Species = 755, Ability =  4 }, // Morelull
            new EncounterStatic8N(Nest27,1,1,2) { Species = 710, Ability =  4, Form = 2 }, // Pumpkaboo-2
            new EncounterStatic8N(Nest27,1,2,2) { Species = 315, Ability =  4 }, // Roselia
            new EncounterStatic8N(Nest27,2,3,3) { Species = 756, Ability =  4 }, // Shiinotic
            new EncounterStatic8N(Nest27,2,4,4) { Species = 556, Ability =  4 }, // Maractus
            new EncounterStatic8N(Nest27,2,4,4) { Species = 709, Ability =  4 }, // Trevenant
            new EncounterStatic8N(Nest27,3,4,4) { Species = 711, Ability = -1 }, // Gourgeist
            new EncounterStatic8N(Nest27,4,4,4) { Species = 781, Ability = -1 }, // Dhelmise
            new EncounterStatic8N(Nest27,4,4,4) { Species = 710, Ability = -1, Form = 3 }, // Pumpkaboo-3
            new EncounterStatic8N(Nest28,0,0,1) { Species = 434, Ability =  4 }, // Stunky
            new EncounterStatic8N(Nest28,0,0,1) { Species = 568, Ability =  4 }, // Trubbish
            new EncounterStatic8N(Nest28,0,1,1) { Species = 451, Ability =  4 }, // Skorupi
            new EncounterStatic8N(Nest28,1,2,2) { Species = 315, Ability =  4 }, // Roselia
            new EncounterStatic8N(Nest28,2,3,3) { Species = 211, Ability =  4 }, // Qwilfish
            new EncounterStatic8N(Nest28,2,4,4) { Species = 452, Ability =  4 }, // Drapion
            new EncounterStatic8N(Nest28,2,4,4) { Species = 045, Ability =  4 }, // Vileplume
            new EncounterStatic8N(Nest28,4,4,4) { Species = 569, Ability = -1 }, // Garbodor
            new EncounterStatic8N(Nest29,0,0,1) { Species = 848, Ability =  4 }, // Toxel
            new EncounterStatic8N(Nest29,0,0,1) { Species = 092, Ability =  4 }, // Gastly
            new EncounterStatic8N(Nest29,0,1,1) { Species = 451, Ability =  4 }, // Skorupi
            new EncounterStatic8N(Nest29,0,1,1) { Species = 043, Ability =  4 }, // Oddish
            new EncounterStatic8N(Nest29,1,1,2) { Species = 044, Ability =  4 }, // Gloom
            new EncounterStatic8N(Nest29,1,2,2) { Species = 093, Ability =  4 }, // Haunter
            new EncounterStatic8N(Nest29,2,3,3) { Species = 109, Ability =  4 }, // Koffing
            new EncounterStatic8N(Nest29,2,4,4) { Species = 211, Ability =  4 }, // Qwilfish
            new EncounterStatic8N(Nest29,2,4,4) { Species = 045, Ability =  4 }, // Vileplume
            new EncounterStatic8N(Nest29,3,4,4) { Species = 315, Ability = -1 }, // Roselia
            new EncounterStatic8N(Nest29,4,4,4) { Species = 849, Ability = -1 }, // Toxtricity
            new EncounterStatic8N(Nest29,4,4,4) { Species = 110, Ability = -1, Form = 1 }, // Weezing-1
            new EncounterStatic8N(Nest30,0,0,1) { Species = 519, Ability =  4 }, // Pidove
            new EncounterStatic8N(Nest30,0,0,1) { Species = 163, Ability =  4 }, // Hoothoot
            new EncounterStatic8N(Nest30,0,1,1) { Species = 177, Ability =  4 }, // Natu
            new EncounterStatic8N(Nest30,1,1,2) { Species = 527, Ability =  4 }, // Woobat
            new EncounterStatic8N(Nest30,1,2,2) { Species = 520, Ability =  4 }, // Tranquill
            new EncounterStatic8N(Nest30,2,3,3) { Species = 521, Ability =  4 }, // Unfezant
            new EncounterStatic8N(Nest30,2,4,4) { Species = 164, Ability =  4 }, // Noctowl
            new EncounterStatic8N(Nest30,2,4,4) { Species = 528, Ability =  4 }, // Swoobat
            new EncounterStatic8N(Nest30,3,4,4) { Species = 178, Ability = -1 }, // Xatu
            new EncounterStatic8N(Nest30,4,4,4) { Species = 561, Ability = -1 }, // Sigilyph
            new EncounterStatic8N(Nest31,0,0,1) { Species = 821, Ability =  4 }, // Rookidee
            new EncounterStatic8N(Nest31,0,0,1) { Species = 714, Ability =  4 }, // Noibat
            new EncounterStatic8N(Nest31,0,1,1) { Species = 278, Ability =  4 }, // Wingull
            new EncounterStatic8N(Nest31,0,1,1) { Species = 177, Ability =  4 }, // Natu
            new EncounterStatic8N(Nest31,1,1,2) { Species = 425, Ability =  4 }, // Drifloon
            new EncounterStatic8N(Nest31,1,2,2) { Species = 822, Ability =  4 }, // Corvisquire
            new EncounterStatic8N(Nest31,2,3,3) { Species = 426, Ability =  4 }, // Drifblim
            new EncounterStatic8N(Nest31,2,4,4) { Species = 279, Ability =  4 }, // Pelipper
            new EncounterStatic8N(Nest31,2,4,4) { Species = 178, Ability =  4 }, // Xatu
            new EncounterStatic8N(Nest31,3,4,4) { Species = 823, Ability = -1 }, // Corviknight
            new EncounterStatic8N(Nest31,4,4,4) { Species = 701, Ability = -1 }, // Hawlucha
            new EncounterStatic8N(Nest31,4,4,4) { Species = 845, Ability = -1 }, // Cramorant
            new EncounterStatic8N(Nest32,0,0,1) { Species = 173, Ability =  4 }, // Cleffa
            new EncounterStatic8N(Nest32,0,0,1) { Species = 175, Ability =  4 }, // Togepi
            new EncounterStatic8N(Nest32,0,1,1) { Species = 742, Ability =  4 }, // Cutiefly
            new EncounterStatic8N(Nest32,1,1,2) { Species = 035, Ability =  4 }, // Clefairy
            new EncounterStatic8N(Nest32,1,2,2) { Species = 755, Ability =  4 }, // Morelull
            new EncounterStatic8N(Nest32,2,3,3) { Species = 176, Ability =  4 }, // Togetic
            new EncounterStatic8N(Nest32,2,4,4) { Species = 036, Ability =  4 }, // Clefable
            new EncounterStatic8N(Nest32,2,4,4) { Species = 743, Ability =  4 }, // Ribombee
            new EncounterStatic8N(Nest32,3,4,4) { Species = 756, Ability = -1 }, // Shiinotic
            new EncounterStatic8N(Nest32,4,4,4) { Species = 468, Ability = -1 }, // Togekiss
            new EncounterStatic8N(Nest33,0,0,1) { Species = 439, Ability =  4 }, // Mime Jr.
            new EncounterStatic8N(Nest33,0,0,1) { Species = 868, Ability =  4 }, // Milcery
            new EncounterStatic8N(Nest33,0,1,1) { Species = 859, Ability =  4 }, // Impidimp
            new EncounterStatic8N(Nest33,0,1,1) { Species = 280, Ability =  4 }, // Ralts
            new EncounterStatic8N(Nest33,1,1,2) { Species = 035, Ability =  4 }, // Clefairy
            new EncounterStatic8N(Nest33,1,2,2) { Species = 281, Ability =  4 }, // Kirlia
            new EncounterStatic8N(Nest33,2,3,3) { Species = 860, Ability =  4 }, // Morgrem
            new EncounterStatic8N(Nest33,2,4,4) { Species = 036, Ability =  4 }, // Clefable
            new EncounterStatic8N(Nest33,2,4,4) { Species = 282, Ability =  4 }, // Gardevoir
            new EncounterStatic8N(Nest33,3,4,4) { Species = 869, Ability = -1 }, // Alcremie
            new EncounterStatic8N(Nest33,4,4,4) { Species = 861, Ability = -1 }, // Grimmsnarl
            new EncounterStatic8N(Nest34,0,0,1) { Species = 509, Ability =  4 }, // Purrloin
            new EncounterStatic8N(Nest34,0,0,1) { Species = 434, Ability =  4 }, // Stunky
            new EncounterStatic8N(Nest34,0,1,1) { Species = 215, Ability =  4 }, // Sneasel
            new EncounterStatic8N(Nest34,0,1,1) { Species = 686, Ability =  4 }, // Inkay
            new EncounterStatic8N(Nest34,1,1,2) { Species = 624, Ability =  4 }, // Pawniard
            new EncounterStatic8N(Nest34,1,2,2) { Species = 510, Ability =  4 }, // Liepard
            new EncounterStatic8N(Nest34,2,3,3) { Species = 435, Ability =  4 }, // Skuntank
            new EncounterStatic8N(Nest34,2,4,4) { Species = 461, Ability =  4 }, // Weavile
            new EncounterStatic8N(Nest34,2,4,4) { Species = 687, Ability =  4 }, // Malamar
            new EncounterStatic8N(Nest34,3,4,4) { Species = 625, Ability = -1 }, // Bisharp
            new EncounterStatic8N(Nest34,4,4,4) { Species = 342, Ability = -1 }, // Crawdaunt
            new EncounterStatic8N(Nest35,0,0,1) { Species = 827, Ability =  4 }, // Nickit
            new EncounterStatic8N(Nest35,0,0,1) { Species = 263, Ability =  4, Form = 1 }, // Zigzagoon-1
            new EncounterStatic8N(Nest35,0,1,1) { Species = 509, Ability =  4 }, // Purrloin
            new EncounterStatic8N(Nest35,0,1,1) { Species = 859, Ability =  4 }, // Impidimp
            new EncounterStatic8N(Nest35,1,2,2) { Species = 828, Ability =  4 }, // Thievul
            new EncounterStatic8N(Nest35,2,3,3) { Species = 264, Ability =  4, Form = 1 }, // Linoone-1
            new EncounterStatic8N(Nest35,2,4,4) { Species = 860, Ability =  4 }, // Morgrem
            new EncounterStatic8N(Nest35,2,4,4) { Species = 861, Ability =  4 }, // Grimmsnarl
            new EncounterStatic8N(Nest35,4,4,4) { Species = 862, Ability = -1 }, // Obstagoon
            new EncounterStatic8N(Nest36,0,0,1) { Species = 714, Ability =  4 }, // Noibat
            new EncounterStatic8N(Nest36,0,1,1) { Species = 714, Ability =  4 }, // Noibat
            new EncounterStatic8N(Nest36,1,2,2) { Species = 329, Ability =  4 }, // Vibrava
            new EncounterStatic8N(Nest37,0,0,1) { Species = 714, Ability =  4 }, // Noibat
            new EncounterStatic8N(Nest37,0,0,1) { Species = 840, Ability =  4 }, // Applin
            new EncounterStatic8N(Nest37,0,1,1) { Species = 885, Ability =  4 }, // Dreepy
            new EncounterStatic8N(Nest37,1,1,2) { Species = 714, Ability =  4 }, // Noibat
            new EncounterStatic8N(Nest37,1,2,2) { Species = 840, Ability =  4 }, // Applin
            new EncounterStatic8N(Nest37,2,3,3) { Species = 886, Ability =  4 }, // Drakloak
            new EncounterStatic8N(Nest37,2,4,4) { Species = 715, Ability =  4 }, // Noivern
            new EncounterStatic8N(Nest37,4,4,4) { Species = 887, Ability = -1 }, // Dragapult
            new EncounterStatic8N(Nest38,0,0,1) { Species = 659, Ability =  4 }, // Bunnelby
            new EncounterStatic8N(Nest38,0,0,1) { Species = 163, Ability =  4 }, // Hoothoot
            new EncounterStatic8N(Nest38,0,1,1) { Species = 519, Ability =  4 }, // Pidove
            new EncounterStatic8N(Nest38,0,1,1) { Species = 572, Ability =  4 }, // Minccino
            new EncounterStatic8N(Nest38,1,1,2) { Species = 694, Ability =  4 }, // Helioptile
            new EncounterStatic8N(Nest38,1,2,2) { Species = 759, Ability =  4 }, // Stufful
            new EncounterStatic8N(Nest38,2,3,3) { Species = 660, Ability =  4 }, // Diggersby
            new EncounterStatic8N(Nest38,2,4,4) { Species = 164, Ability =  4 }, // Noctowl
            new EncounterStatic8N(Nest38,2,4,4) { Species = 521, Ability =  4 }, // Unfezant
            new EncounterStatic8N(Nest38,3,4,4) { Species = 695, Ability = -1 }, // Heliolisk
            new EncounterStatic8N(Nest38,4,4,4) { Species = 573, Ability = -1 }, // Cinccino
            new EncounterStatic8N(Nest38,4,4,4) { Species = 760, Ability = -1 }, // Bewear
            new EncounterStatic8N(Nest39,0,0,1) { Species = 819, Ability =  4 }, // Skwovet
            new EncounterStatic8N(Nest39,0,0,1) { Species = 831, Ability =  4 }, // Wooloo
            new EncounterStatic8N(Nest39,0,1,1) { Species = 263, Ability =  4, Form = 1 }, // Zigzagoon-1
            new EncounterStatic8N(Nest39,0,1,1) { Species = 446, Ability =  4 }, // Munchlax
            new EncounterStatic8N(Nest39,1,2,2) { Species = 820, Ability =  4 }, // Greedent
            new EncounterStatic8N(Nest39,2,3,3) { Species = 264, Ability =  4, Form = 1 }, // Linoone-1
            new EncounterStatic8N(Nest39,2,4,4) { Species = 820, Ability =  4 }, // Greedent
            new EncounterStatic8N(Nest39,2,4,4) { Species = 832, Ability =  4 }, // Dubwool
            new EncounterStatic8N(Nest39,3,4,4) { Species = 660, Ability = -1 }, // Diggersby
            new EncounterStatic8N(Nest39,4,4,4) { Species = 143, Ability = -1 }, // Snorlax
            new EncounterStatic8N(Nest40,0,0,1) { Species = 535, Ability =  4 }, // Tympole
            new EncounterStatic8N(Nest40,0,0,1) { Species = 090, Ability =  4 }, // Shellder
            new EncounterStatic8N(Nest40,0,1,1) { Species = 170, Ability =  4 }, // Chinchou
            new EncounterStatic8N(Nest40,1,2,2) { Species = 846, Ability =  4 }, // Arrokuda
            new EncounterStatic8N(Nest40,2,4,4) { Species = 171, Ability =  4 }, // Lanturn
            new EncounterStatic8N(Nest40,4,4,4) { Species = 847, Ability = -1 }, // Barraskewda
            new EncounterStatic8N(Nest41,0,0,1) { Species = 422, Ability =  4, Form = 1 }, // Shellos-1
            new EncounterStatic8N(Nest41,0,0,1) { Species = 098, Ability =  4 }, // Krabby
            new EncounterStatic8N(Nest41,0,1,1) { Species = 341, Ability =  4 }, // Corphish
            new EncounterStatic8N(Nest41,0,1,1) { Species = 833, Ability =  4 }, // Chewtle
            new EncounterStatic8N(Nest41,1,1,2) { Species = 688, Ability =  4 }, // Binacle
            new EncounterStatic8N(Nest41,1,2,2) { Species = 771, Ability =  4 }, // Pyukumuku
            new EncounterStatic8N(Nest41,2,3,3) { Species = 099, Ability =  4 }, // Kingler
            new EncounterStatic8N(Nest41,2,4,4) { Species = 342, Ability =  4 }, // Crawdaunt
            new EncounterStatic8N(Nest41,2,4,4) { Species = 689, Ability =  4 }, // Barbaracle
            new EncounterStatic8N(Nest41,3,4,4) { Species = 423, Ability = -1, Form = 1 }, // Gastrodon-1
            new EncounterStatic8N(Nest41,4,4,4) { Species = 593, Ability = -1 }, // Jellicent
            new EncounterStatic8N(Nest41,4,4,4) { Species = 834, Ability = -1 }, // Drednaw
            new EncounterStatic8N(Nest42,0,0,1) { Species = 092, Ability =  4 }, // Gastly
            new EncounterStatic8N(Nest42,0,0,1) { Species = 562, Ability =  4, Form = 1 }, // Yamask-1
            new EncounterStatic8N(Nest42,0,1,1) { Species = 854, Ability =  4 }, // Sinistea
            new EncounterStatic8N(Nest42,0,1,1) { Species = 355, Ability =  4 }, // Duskull
            new EncounterStatic8N(Nest42,1,2,2) { Species = 093, Ability =  4 }, // Haunter
            new EncounterStatic8N(Nest42,2,3,3) { Species = 356, Ability =  4 }, // Dusclops
            new EncounterStatic8N(Nest42,4,4,4) { Species = 477, Ability = -1 }, // Dusknoir
            new EncounterStatic8N(Nest42,4,4,4) { Species = 094, Ability = -1 }, // Gengar
            new EncounterStatic8N(Nest43,0,0,1) { Species = 129, Ability =  4 }, // Magikarp
            new EncounterStatic8N(Nest43,0,0,1) { Species = 349, Ability =  4 }, // Feebas
            new EncounterStatic8N(Nest43,0,1,1) { Species = 846, Ability =  4 }, // Arrokuda
            new EncounterStatic8N(Nest43,0,1,1) { Species = 833, Ability =  4 }, // Chewtle
            new EncounterStatic8N(Nest43,1,2,2) { Species = 747, Ability =  4 }, // Mareanie
            new EncounterStatic8N(Nest43,2,3,3) { Species = 211, Ability =  4 }, // Qwilfish
            new EncounterStatic8N(Nest43,2,4,4) { Species = 748, Ability =  4 }, // Toxapex
            new EncounterStatic8N(Nest43,3,4,4) { Species = 771, Ability = -1 }, // Pyukumuku
            new EncounterStatic8N(Nest43,3,4,4) { Species = 130, Ability = -1 }, // Gyarados
            new EncounterStatic8N(Nest43,4,4,4) { Species = 131, Ability = -1 }, // Lapras
            new EncounterStatic8N(Nest43,4,4,4) { Species = 350, Ability = -1 }, // Milotic
            new EncounterStatic8N(Nest44,0,0,1) { Species = 447, Ability =  4 }, // Riolu
            new EncounterStatic8N(Nest44,0,0,1) { Species = 436, Ability =  4 }, // Bronzor
            new EncounterStatic8N(Nest44,0,1,1) { Species = 624, Ability =  4 }, // Pawniard
            new EncounterStatic8N(Nest44,0,1,1) { Species = 599, Ability =  4 }, // Klink
            new EncounterStatic8N(Nest44,1,2,2) { Species = 095, Ability =  4 }, // Onix
            new EncounterStatic8N(Nest44,2,4,4) { Species = 437, Ability =  4 }, // Bronzong
            new EncounterStatic8N(Nest44,3,4,4) { Species = 625, Ability = -1 }, // Bisharp
            new EncounterStatic8N(Nest44,3,4,4) { Species = 208, Ability = -1 }, // Steelix
            new EncounterStatic8N(Nest44,4,4,4) { Species = 601, Ability = -1 }, // Klinklang
            new EncounterStatic8N(Nest44,4,4,4) { Species = 448, Ability = -1 }, // Lucario
            new EncounterStatic8N(Nest45,0,0,1) { Species = 767, Ability =  4 }, // Wimpod
            new EncounterStatic8N(Nest45,0,0,1) { Species = 824, Ability =  4 }, // Blipbug
            new EncounterStatic8N(Nest45,0,1,1) { Species = 751, Ability =  4 }, // Dewpider
            new EncounterStatic8N(Nest45,1,2,2) { Species = 557, Ability =  4 }, // Dwebble
            new EncounterStatic8N(Nest45,2,3,3) { Species = 825, Ability =  4 }, // Dottler
            new EncounterStatic8N(Nest45,2,4,4) { Species = 826, Ability =  4 }, // Orbeetle
            new EncounterStatic8N(Nest45,3,4,4) { Species = 752, Ability = -1 }, // Araquanid
            new EncounterStatic8N(Nest45,3,4,4) { Species = 768, Ability = -1 }, // Golisopod
            new EncounterStatic8N(Nest45,4,4,4) { Species = 292, Ability = -1 }, // Shedinja
            new EncounterStatic8N(Nest46,0,0,1) { Species = 679, Ability =  4 }, // Honedge
            new EncounterStatic8N(Nest46,0,0,1) { Species = 562, Ability =  4, Form = 1 }, // Yamask-1
            new EncounterStatic8N(Nest46,0,1,1) { Species = 854, Ability =  4 }, // Sinistea
            new EncounterStatic8N(Nest46,0,1,1) { Species = 425, Ability =  4 }, // Drifloon
            new EncounterStatic8N(Nest46,1,2,2) { Species = 680, Ability =  4 }, // Doublade
            new EncounterStatic8N(Nest46,2,3,3) { Species = 426, Ability =  4 }, // Drifblim
            new EncounterStatic8N(Nest46,3,4,4) { Species = 855, Ability = -1 }, // Polteageist
            new EncounterStatic8N(Nest46,4,4,4) { Species = 867, Ability = -1 }, // Runerigus
            new EncounterStatic8N(Nest46,4,4,4) { Species = 681, Ability = -1 }, // Aegislash
            new EncounterStatic8N(Nest47,0,0,1) { Species = 447, Ability =  4 }, // Riolu
            new EncounterStatic8N(Nest47,0,0,1) { Species = 066, Ability =  4 }, // Machop
            new EncounterStatic8N(Nest47,0,1,1) { Species = 759, Ability =  4 }, // Stufful
            new EncounterStatic8N(Nest47,1,2,2) { Species = 760, Ability =  4 }, // Bewear
            new EncounterStatic8N(Nest47,1,3,3) { Species = 870, Ability =  4 }, // Falinks
            new EncounterStatic8N(Nest47,2,3,3) { Species = 067, Ability =  4 }, // Machoke
            new EncounterStatic8N(Nest47,3,4,4) { Species = 068, Ability = -1 }, // Machamp
            new EncounterStatic8N(Nest47,4,4,4) { Species = 448, Ability = -1 }, // Lucario
            new EncounterStatic8N(Nest47,4,4,4) { Species = 475, Ability = -1 }, // Gallade
            new EncounterStatic8N(Nest48,0,0,1) { Species = 052, Ability =  4, Form = 2 }, // Meowth-2
            new EncounterStatic8N(Nest48,0,0,1) { Species = 436, Ability =  4 }, // Bronzor
            new EncounterStatic8N(Nest48,0,1,1) { Species = 624, Ability =  4 }, // Pawniard
            new EncounterStatic8N(Nest48,0,1,1) { Species = 597, Ability =  4 }, // Ferroseed
            new EncounterStatic8N(Nest48,1,2,2) { Species = 679, Ability =  4 }, // Honedge
            new EncounterStatic8N(Nest48,1,2,2) { Species = 437, Ability =  4 }, // Bronzong
            new EncounterStatic8N(Nest48,3,4,4) { Species = 863, Ability = -1 }, // Perrserker
            new EncounterStatic8N(Nest48,2,4,4) { Species = 598, Ability =  4 }, // Ferrothorn
            new EncounterStatic8N(Nest48,3,4,4) { Species = 625, Ability = -1 }, // Bisharp
            new EncounterStatic8N(Nest48,3,4,4) { Species = 618, Ability = -1, Form = 1 }, // Stunfisk-1
            new EncounterStatic8N(Nest48,4,4,4) { Species = 879, Ability = -1 }, // Copperajah
            new EncounterStatic8N(Nest48,4,4,4) { Species = 884, Ability = -1 }, // Duraludon
            new EncounterStatic8N(Nest49,0,0,1) { Species = 686, Ability =  4 }, // Inkay
            new EncounterStatic8N(Nest49,0,0,1) { Species = 280, Ability =  4 }, // Ralts
            new EncounterStatic8N(Nest49,0,1,1) { Species = 122, Ability =  4, Form = 1 }, // Mr. Mime-1
            new EncounterStatic8N(Nest49,0,1,1) { Species = 527, Ability =  4 }, // Woobat
            new EncounterStatic8N(Nest49,1,2,2) { Species = 856, Ability =  4 }, // Hatenna
            new EncounterStatic8N(Nest49,1,2,2) { Species = 857, Ability =  4 }, // Hattrem
            new EncounterStatic8N(Nest49,2,3,3) { Species = 281, Ability =  4 }, // Kirlia
            new EncounterStatic8N(Nest49,2,4,4) { Species = 528, Ability =  4 }, // Swoobat
            new EncounterStatic8N(Nest49,3,4,4) { Species = 858, Ability = -1 }, // Hatterene
            new EncounterStatic8N(Nest49,3,4,4) { Species = 866, Ability = -1 }, // Mr. Rime
            new EncounterStatic8N(Nest49,4,4,4) { Species = 687, Ability = -1 }, // Malamar
            new EncounterStatic8N(Nest49,4,4,4) { Species = 282, Ability = -1 }, // Gardevoir
            new EncounterStatic8N(Nest50,0,0,1) { Species = 557, Ability =  4 }, // Dwebble
            new EncounterStatic8N(Nest50,0,0,1) { Species = 438, Ability =  4 }, // Bonsly
            new EncounterStatic8N(Nest50,0,1,1) { Species = 837, Ability =  4 }, // Rolycoly
            new EncounterStatic8N(Nest50,1,2,2) { Species = 838, Ability =  4 }, // Carkol
            new EncounterStatic8N(Nest50,2,4,4) { Species = 095, Ability =  4 }, // Onix
            new EncounterStatic8N(Nest50,3,4,4) { Species = 558, Ability = -1 }, // Crustle
            new EncounterStatic8N(Nest50,3,4,4) { Species = 839, Ability = -1 }, // Coalossal
            new EncounterStatic8N(Nest50,4,4,4) { Species = 208, Ability = -1 }, // Steelix
            new EncounterStatic8N(Nest51,0,0,1) { Species = 194, Ability =  4 }, // Wooper
            new EncounterStatic8N(Nest51,0,0,1) { Species = 339, Ability =  4 }, // Barboach
            new EncounterStatic8N(Nest51,0,1,1) { Species = 562, Ability =  4, Form = 1 }, // Yamask-1
            new EncounterStatic8N(Nest51,0,1,1) { Species = 622, Ability =  4 }, // Golett
            new EncounterStatic8N(Nest51,1,2,2) { Species = 536, Ability =  4 }, // Palpitoad
            new EncounterStatic8N(Nest51,1,2,2) { Species = 195, Ability =  4 }, // Quagsire
            new EncounterStatic8N(Nest51,2,3,3) { Species = 618, Ability =  4, Form = 1 }, // Stunfisk-1
            new EncounterStatic8N(Nest51,2,4,4) { Species = 623, Ability =  4 }, // Golurk
            new EncounterStatic8N(Nest51,3,4,4) { Species = 423, Ability = -1, Form = 1 }, // Gastrodon-1
            new EncounterStatic8N(Nest51,3,4,4) { Species = 537, Ability = -1 }, // Seismitoad
            new EncounterStatic8N(Nest51,4,4,4) { Species = 867, Ability = -1 }, // Runerigus
            new EncounterStatic8N(Nest51,4,4,4) { Species = 464, Ability = -1 }, // Rhyperior
            new EncounterStatic8N(Nest52,0,0,1) { Species = 850, Ability =  4 }, // Sizzlipede
            new EncounterStatic8N(Nest52,0,1,1) { Species = 607, Ability =  4 }, // Litwick
            new EncounterStatic8N(Nest52,0,1,1) { Species = 004, Ability =  4 }, // Charmander
            new EncounterStatic8N(Nest52,1,2,2) { Species = 005, Ability =  4 }, // Charmeleon
            new EncounterStatic8N(Nest52,2,3,3) { Species = 631, Ability =  4 }, // Heatmor
            new EncounterStatic8N(Nest52,2,4,4) { Species = 324, Ability =  4 }, // Torkoal
            new EncounterStatic8N(Nest52,3,4,4) { Species = 758, Ability = -1, Gender = 1 }, // Salazzle
            new EncounterStatic8N(Nest52,4,4,4) { Species = 851, Ability = -1 }, // Centiskorch
            new EncounterStatic8N(Nest52,4,4,4) { Species = 006, Ability = -1 }, // Charizard
            new EncounterStatic8N(Nest53,0,0,1) { Species = 850, Ability =  4 }, // Sizzlipede
            new EncounterStatic8N(Nest53,0,1,1) { Species = 607, Ability =  4 }, // Litwick
            new EncounterStatic8N(Nest53,0,1,1) { Species = 757, Ability =  4 }, // Salandit
            new EncounterStatic8N(Nest53,1,2,2) { Species = 838, Ability =  4 }, // Carkol
            new EncounterStatic8N(Nest53,2,4,4) { Species = 324, Ability =  4 }, // Torkoal
            new EncounterStatic8N(Nest53,3,4,4) { Species = 609, Ability = -1 }, // Chandelure
            new EncounterStatic8N(Nest53,4,4,4) { Species = 839, Ability = -1 }, // Coalossal
            new EncounterStatic8N(Nest54,0,0,1) { Species = 582, Ability =  4 }, // Vanillite
            new EncounterStatic8N(Nest54,0,1,1) { Species = 122, Ability =  4, Form = 1 }, // Mr. Mime-1
            new EncounterStatic8N(Nest54,0,1,1) { Species = 712, Ability =  4 }, // Bergmite
            new EncounterStatic8N(Nest54,1,2,2) { Species = 361, Ability =  4 }, // Snorunt
            new EncounterStatic8N(Nest54,1,2,2) { Species = 225, Ability =  4 }, // Delibird
            new EncounterStatic8N(Nest54,2,3,3) { Species = 713, Ability =  4 }, // Avalugg
            new EncounterStatic8N(Nest54,2,4,4) { Species = 362, Ability =  4 }, // Glalie
            new EncounterStatic8N(Nest54,3,4,4) { Species = 584, Ability = -1 }, // Vanilluxe
            new EncounterStatic8N(Nest54,3,4,4) { Species = 866, Ability = -1 }, // Mr. Rime
            new EncounterStatic8N(Nest54,4,4,4) { Species = 131, Ability = -1 }, // Lapras
            new EncounterStatic8N(Nest55,0,0,1) { Species = 835, Ability =  4 }, // Yamper
            new EncounterStatic8N(Nest55,0,0,1) { Species = 848, Ability =  4 }, // Toxel
            new EncounterStatic8N(Nest55,0,1,1) { Species = 025, Ability =  4 }, // Pikachu
            new EncounterStatic8N(Nest55,0,1,1) { Species = 595, Ability =  4 }, // Joltik
            new EncounterStatic8N(Nest55,1,2,2) { Species = 170, Ability =  4 }, // Chinchou
            new EncounterStatic8N(Nest55,1,2,2) { Species = 171, Ability =  4 }, // Lanturn
            new EncounterStatic8N(Nest55,2,4,4) { Species = 836, Ability =  4 }, // Boltund
            new EncounterStatic8N(Nest55,2,4,4) { Species = 849, Ability =  4 }, // Toxtricity
            new EncounterStatic8N(Nest55,3,4,4) { Species = 871, Ability = -1 }, // Pincurchin
            new EncounterStatic8N(Nest55,3,4,4) { Species = 596, Ability = -1 }, // Galvantula
            new EncounterStatic8N(Nest55,4,4,4) { Species = 777, Ability = -1 }, // Togedemaru
            new EncounterStatic8N(Nest55,4,4,4) { Species = 877, Ability = -1 }, // Morpeko
            new EncounterStatic8N(Nest56,0,0,1) { Species = 172, Ability =  4 }, // Pichu
            new EncounterStatic8N(Nest56,0,0,1) { Species = 309, Ability =  4 }, // Electrike
            new EncounterStatic8N(Nest56,0,1,1) { Species = 848, Ability =  4 }, // Toxel
            new EncounterStatic8N(Nest56,0,1,1) { Species = 694, Ability =  4 }, // Helioptile
            new EncounterStatic8N(Nest56,1,2,2) { Species = 595, Ability =  4 }, // Joltik
            new EncounterStatic8N(Nest56,1,2,2) { Species = 025, Ability =  4 }, // Pikachu
            new EncounterStatic8N(Nest56,2,4,4) { Species = 025, Ability =  4 }, // Pikachu
            new EncounterStatic8N(Nest56,2,4,4) { Species = 479, Ability =  4, Form = 5 }, // Rotom-5
            new EncounterStatic8N(Nest56,3,4,4) { Species = 479, Ability = -1, Form = 4 }, // Rotom-4
            new EncounterStatic8N(Nest56,3,4,4) { Species = 479, Ability = -1, Form = 3 }, // Rotom-3
            new EncounterStatic8N(Nest56,4,4,4) { Species = 479, Ability = -1, Form = 2 }, // Rotom-2
            new EncounterStatic8N(Nest56,4,4,4) { Species = 479, Ability = -1, Form = 1 }, // Rotom-1
            new EncounterStatic8N(Nest57,0,0,1) { Species = 406, Ability =  4 }, // Budew
            new EncounterStatic8N(Nest57,0,1,1) { Species = 829, Ability =  4 }, // Gossifleur
            new EncounterStatic8N(Nest57,0,1,1) { Species = 597, Ability =  4 }, // Ferroseed
            new EncounterStatic8N(Nest57,1,2,2) { Species = 840, Ability =  4 }, // Applin
            new EncounterStatic8N(Nest57,2,4,4) { Species = 315, Ability =  4 }, // Roselia
            new EncounterStatic8N(Nest57,3,4,4) { Species = 830, Ability = -1 }, // Eldegoss
            new EncounterStatic8N(Nest57,3,4,4) { Species = 598, Ability = -1 }, // Ferrothorn
            new EncounterStatic8N(Nest57,4,4,4) { Species = 407, Ability = -1 }, // Roserade
            new EncounterStatic8N(Nest58,0,0,1) { Species = 420, Ability =  4 }, // Cherubi
            new EncounterStatic8N(Nest58,0,1,1) { Species = 829, Ability =  4 }, // Gossifleur
            new EncounterStatic8N(Nest58,0,1,1) { Species = 546, Ability =  4 }, // Cottonee
            new EncounterStatic8N(Nest58,1,2,2) { Species = 755, Ability =  4 }, // Morelull
            new EncounterStatic8N(Nest58,2,4,4) { Species = 421, Ability =  4 }, // Cherrim
            new EncounterStatic8N(Nest58,2,4,4) { Species = 756, Ability =  4 }, // Shiinotic
            new EncounterStatic8N(Nest58,3,4,4) { Species = 830, Ability = -1 }, // Eldegoss
            new EncounterStatic8N(Nest58,3,4,4) { Species = 547, Ability = -1 }, // Whimsicott
            new EncounterStatic8N(Nest58,4,4,4) { Species = 781, Ability = -1 }, // Dhelmise
            new EncounterStatic8N(Nest59,0,0,1) { Species = 434, Ability =  4 }, // Stunky
            new EncounterStatic8N(Nest59,0,0,1) { Species = 568, Ability =  4 }, // Trubbish
            new EncounterStatic8N(Nest59,0,1,1) { Species = 451, Ability =  4 }, // Skorupi
            new EncounterStatic8N(Nest59,0,1,1) { Species = 109, Ability =  4 }, // Koffing
            new EncounterStatic8N(Nest59,1,2,2) { Species = 848, Ability =  4 }, // Toxel
            new EncounterStatic8N(Nest59,2,4,4) { Species = 569, Ability =  4 }, // Garbodor
            new EncounterStatic8N(Nest59,2,4,4) { Species = 452, Ability =  4 }, // Drapion
            new EncounterStatic8N(Nest59,3,4,4) { Species = 849, Ability = -1 }, // Toxtricity
            new EncounterStatic8N(Nest59,3,4,4) { Species = 435, Ability = -1 }, // Skuntank
            new EncounterStatic8N(Nest59,4,4,4) { Species = 110, Ability = -1, Form = 1 }, // Weezing-1
            new EncounterStatic8N(Nest60,0,0,1) { Species = 177, Ability =  4 }, // Natu
            new EncounterStatic8N(Nest60,0,0,1) { Species = 163, Ability =  4 }, // Hoothoot
            new EncounterStatic8N(Nest60,0,1,1) { Species = 821, Ability =  4 }, // Rookidee
            new EncounterStatic8N(Nest60,0,1,1) { Species = 278, Ability =  4 }, // Wingull
            new EncounterStatic8N(Nest60,1,2,2) { Species = 012, Ability =  4 }, // Butterfree
            new EncounterStatic8N(Nest60,1,2,2) { Species = 822, Ability =  4 }, // Corvisquire
            new EncounterStatic8N(Nest60,2,4,4) { Species = 164, Ability =  4 }, // Noctowl
            new EncounterStatic8N(Nest60,2,4,4) { Species = 279, Ability =  4 }, // Pelipper
            new EncounterStatic8N(Nest60,3,4,4) { Species = 178, Ability = -1 }, // Xatu
            new EncounterStatic8N(Nest60,3,4,4) { Species = 701, Ability = -1 }, // Hawlucha
            new EncounterStatic8N(Nest60,4,4,4) { Species = 823, Ability = -1 }, // Corviknight
            new EncounterStatic8N(Nest60,4,4,4) { Species = 225, Ability = -1 }, // Delibird
            new EncounterStatic8N(Nest61,0,0,1) { Species = 175, Ability =  4 }, // Togepi
            new EncounterStatic8N(Nest61,0,0,1) { Species = 755, Ability =  4 }, // Morelull
            new EncounterStatic8N(Nest61,0,1,1) { Species = 859, Ability =  4 }, // Impidimp
            new EncounterStatic8N(Nest61,0,1,1) { Species = 280, Ability =  4 }, // Ralts
            new EncounterStatic8N(Nest61,1,2,2) { Species = 176, Ability =  4 }, // Togetic
            new EncounterStatic8N(Nest61,1,2,2) { Species = 756, Ability =  4 }, // Shiinotic
            new EncounterStatic8N(Nest61,2,4,4) { Species = 860, Ability =  4 }, // Morgrem
            new EncounterStatic8N(Nest61,3,4,4) { Species = 282, Ability = -1 }, // Gardevoir
            new EncounterStatic8N(Nest61,3,4,4) { Species = 468, Ability = -1 }, // Togekiss
            new EncounterStatic8N(Nest61,4,4,4) { Species = 861, Ability = -1 }, // Grimmsnarl
            new EncounterStatic8N(Nest61,4,4,4) { Species = 778, Ability = -1 }, // Mimikyu
            new EncounterStatic8N(Nest62,0,0,1) { Species = 827, Ability =  4 }, // Nickit
            new EncounterStatic8N(Nest62,0,0,1) { Species = 263, Ability =  4, Form = 1 }, // Zigzagoon-1
            new EncounterStatic8N(Nest62,0,1,1) { Species = 215, Ability =  4 }, // Sneasel
            new EncounterStatic8N(Nest62,1,2,2) { Species = 510, Ability =  4 }, // Liepard
            new EncounterStatic8N(Nest62,1,2,2) { Species = 264, Ability =  4, Form = 1 }, // Linoone-1
            new EncounterStatic8N(Nest62,2,4,4) { Species = 828, Ability =  4 }, // Thievul
            new EncounterStatic8N(Nest62,2,4,4) { Species = 675, Ability =  4 }, // Pangoro
            new EncounterStatic8N(Nest62,3,4,4) { Species = 461, Ability = -1 }, // Weavile
            new EncounterStatic8N(Nest62,4,4,4) { Species = 862, Ability = -1 }, // Obstagoon
            new EncounterStatic8N(Nest63,0,0,1) { Species = 840, Ability =  4 }, // Applin
            new EncounterStatic8N(Nest63,1,2,2) { Species = 885, Ability =  4 }, // Dreepy
            new EncounterStatic8N(Nest63,3,4,4) { Species = 886, Ability = -1 }, // Drakloak
            new EncounterStatic8N(Nest63,4,4,4) { Species = 887, Ability = -1 }, // Dragapult
            new EncounterStatic8N(Nest64,0,0,1) { Species = 659, Ability =  4 }, // Bunnelby
            new EncounterStatic8N(Nest64,0,0,1) { Species = 519, Ability =  4 }, // Pidove
            new EncounterStatic8N(Nest64,0,1,1) { Species = 819, Ability =  4 }, // Skwovet
            new EncounterStatic8N(Nest64,0,1,1) { Species = 133, Ability =  4 }, // Eevee
            new EncounterStatic8N(Nest64,1,2,2) { Species = 520, Ability =  4 }, // Tranquill
            new EncounterStatic8N(Nest64,1,2,2) { Species = 831, Ability =  4 }, // Wooloo
            new EncounterStatic8N(Nest64,2,4,4) { Species = 521, Ability =  4 }, // Unfezant
            new EncounterStatic8N(Nest64,2,4,4) { Species = 832, Ability =  4 }, // Dubwool
            new EncounterStatic8N(Nest64,4,4,4) { Species = 133, Ability = -1 }, // Eevee
            new EncounterStatic8N(Nest64,4,4,4) { Species = 143, Ability = -1 }, // Snorlax
            new EncounterStatic8N(Nest65,0,0,1) { Species = 132, Ability =  4 }, // Ditto
            new EncounterStatic8N(Nest65,0,1,2) { Species = 132, Ability =  4 }, // Ditto
            new EncounterStatic8N(Nest65,1,2,3) { Species = 132, Ability =  4 }, // Ditto
            new EncounterStatic8N(Nest65,2,3,3) { Species = 132, Ability =  4 }, // Ditto
            new EncounterStatic8N(Nest65,3,4,4) { Species = 132, Ability = -1 }, // Ditto
            new EncounterStatic8N(Nest65,4,4,4) { Species = 132, Ability = -1 }, // Ditto
            new EncounterStatic8N(Nest66,0,0,1) { Species = 458, Ability =  4 }, // Mantyke
            new EncounterStatic8N(Nest66,0,0,1) { Species = 341, Ability =  4 }, // Corphish
            new EncounterStatic8N(Nest66,0,1,1) { Species = 846, Ability =  4 }, // Arrokuda
            new EncounterStatic8N(Nest66,0,1,1) { Species = 833, Ability =  4 }, // Chewtle
            new EncounterStatic8N(Nest66,1,2,2) { Species = 747, Ability =  4 }, // Mareanie
            new EncounterStatic8N(Nest66,2,3,3) { Species = 342, Ability =  4 }, // Crawdaunt
            new EncounterStatic8N(Nest66,2,4,4) { Species = 748, Ability =  4 }, // Toxapex
            new EncounterStatic8N(Nest66,3,4,4) { Species = 771, Ability = -1 }, // Pyukumuku
            new EncounterStatic8N(Nest66,3,4,4) { Species = 226, Ability = -1 }, // Mantine
            new EncounterStatic8N(Nest66,4,4,4) { Species = 131, Ability = -1 }, // Lapras
            new EncounterStatic8N(Nest66,4,4,4) { Species = 134, Ability = -1 }, // Vaporeon
            new EncounterStatic8N(Nest67,0,0,1) { Species = 686, Ability =  4 }, // Inkay
            new EncounterStatic8N(Nest67,0,0,1) { Species = 436, Ability =  4 }, // Bronzor
            new EncounterStatic8N(Nest67,0,1,1) { Species = 122, Ability =  4, Form = 1 }, // Mr. Mime-1
            new EncounterStatic8N(Nest67,0,1,1) { Species = 527, Ability =  4 }, // Woobat
            new EncounterStatic8N(Nest67,1,2,2) { Species = 856, Ability =  4 }, // Hatenna
            new EncounterStatic8N(Nest67,1,2,2) { Species = 857, Ability =  4 }, // Hattrem
            new EncounterStatic8N(Nest67,2,3,3) { Species = 437, Ability =  4 }, // Bronzong
            new EncounterStatic8N(Nest67,2,4,4) { Species = 528, Ability =  4 }, // Swoobat
            new EncounterStatic8N(Nest67,3,4,4) { Species = 687, Ability = -1 }, // Malamar
            new EncounterStatic8N(Nest67,3,4,4) { Species = 866, Ability = -1 }, // Mr. Rime
            new EncounterStatic8N(Nest67,4,4,4) { Species = 858, Ability = -1 }, // Hatterene
            new EncounterStatic8N(Nest67,4,4,4) { Species = 196, Ability = -1 }, // Espeon
            new EncounterStatic8N(Nest68,0,0,1) { Species = 827, Ability =  4 }, // Nickit
            new EncounterStatic8N(Nest68,0,0,1) { Species = 263, Ability =  4, Form = 1 }, // Zigzagoon-1
            new EncounterStatic8N(Nest68,0,1,1) { Species = 686, Ability =  4 }, // Inkay
            new EncounterStatic8N(Nest68,0,1,1) { Species = 624, Ability =  4 }, // Pawniard
            new EncounterStatic8N(Nest68,1,2,2) { Species = 510, Ability =  4 }, // Liepard
            new EncounterStatic8N(Nest68,1,2,2) { Species = 264, Ability =  4, Form = 1 }, // Linoone-1
            new EncounterStatic8N(Nest68,2,4,4) { Species = 828, Ability =  4 }, // Thievul
            new EncounterStatic8N(Nest68,2,4,4) { Species = 675, Ability =  4 }, // Pangoro
            new EncounterStatic8N(Nest68,3,4,4) { Species = 625, Ability = -1 }, // Bisharp
            new EncounterStatic8N(Nest68,3,4,4) { Species = 687, Ability = -1 }, // Malamar
            new EncounterStatic8N(Nest68,4,4,4) { Species = 862, Ability = -1 }, // Obstagoon
            new EncounterStatic8N(Nest68,4,4,4) { Species = 197, Ability = -1 }, // Umbreon
            new EncounterStatic8N(Nest69,0,0,1) { Species = 420, Ability =  4 }, // Cherubi
            new EncounterStatic8N(Nest69,0,0,1) { Species = 761, Ability =  4 }, // Bounsweet
            new EncounterStatic8N(Nest69,0,1,1) { Species = 829, Ability =  4 }, // Gossifleur
            new EncounterStatic8N(Nest69,0,1,1) { Species = 546, Ability =  4 }, // Cottonee
            new EncounterStatic8N(Nest69,1,2,2) { Species = 762, Ability =  4 }, // Steenee
            new EncounterStatic8N(Nest69,1,2,2) { Species = 597, Ability =  4 }, // Ferroseed
            new EncounterStatic8N(Nest69,2,4,4) { Species = 421, Ability =  4 }, // Cherrim
            new EncounterStatic8N(Nest69,2,4,4) { Species = 598, Ability =  4 }, // Ferrothorn
            new EncounterStatic8N(Nest69,3,4,4) { Species = 830, Ability = -1 }, // Eldegoss
            new EncounterStatic8N(Nest69,3,4,4) { Species = 763, Ability = -1 }, // Tsareena
            new EncounterStatic8N(Nest69,4,4,4) { Species = 547, Ability = -1 }, // Whimsicott
            new EncounterStatic8N(Nest69,4,4,4) { Species = 470, Ability = -1 }, // Leafeon
            new EncounterStatic8N(Nest70,0,0,1) { Species = 850, Ability =  4 }, // Sizzlipede
            new EncounterStatic8N(Nest70,0,1,1) { Species = 607, Ability =  4 }, // Litwick
            new EncounterStatic8N(Nest70,1,2,2) { Species = 838, Ability =  4 }, // Carkol
            new EncounterStatic8N(Nest70,2,4,4) { Species = 324, Ability =  4 }, // Torkoal
            new EncounterStatic8N(Nest70,3,4,4) { Species = 059, Ability = -1 }, // Arcanine
            new EncounterStatic8N(Nest70,3,4,4) { Species = 038, Ability = -1 }, // Ninetales
            new EncounterStatic8N(Nest70,4,4,4) { Species = 609, Ability = -1 }, // Chandelure
            new EncounterStatic8N(Nest70,4,4,4) { Species = 136, Ability = -1 }, // Flareon
            new EncounterStatic8N(Nest71,0,0,1) { Species = 835, Ability =  4 }, // Yamper
            new EncounterStatic8N(Nest71,0,0,1) { Species = 848, Ability =  4 }, // Toxel
            new EncounterStatic8N(Nest71,0,1,1) { Species = 025, Ability =  4 }, // Pikachu
            new EncounterStatic8N(Nest71,0,1,1) { Species = 694, Ability =  4 }, // Helioptile
            new EncounterStatic8N(Nest71,1,2,2) { Species = 170, Ability =  4 }, // Chinchou
            new EncounterStatic8N(Nest71,1,2,2) { Species = 171, Ability =  4 }, // Lanturn
            new EncounterStatic8N(Nest71,2,4,4) { Species = 836, Ability =  4 }, // Boltund
            new EncounterStatic8N(Nest71,2,4,4) { Species = 849, Ability =  4 }, // Toxtricity
            new EncounterStatic8N(Nest71,3,4,4) { Species = 695, Ability = -1 }, // Heliolisk
            new EncounterStatic8N(Nest71,3,4,4) { Species = 738, Ability = -1 }, // Vikavolt
            new EncounterStatic8N(Nest71,4,4,4) { Species = 025, Ability = -1 }, // Pikachu
            new EncounterStatic8N(Nest71,4,4,4) { Species = 135, Ability = -1 }, // Jolteon
            new EncounterStatic8N(Nest72,0,0,1) { Species = 582, Ability =  4 }, // Vanillite
            new EncounterStatic8N(Nest72,0,0,1) { Species = 872, Ability =  4 }, // Snom
            new EncounterStatic8N(Nest72,0,1,1) { Species = 122, Ability =  4, Form = 1 }, // Mr. Mime-1
            new EncounterStatic8N(Nest72,0,1,1) { Species = 712, Ability =  4 }, // Bergmite
            new EncounterStatic8N(Nest72,1,2,2) { Species = 361, Ability =  4 }, // Snorunt
            new EncounterStatic8N(Nest72,1,2,2) { Species = 583, Ability =  4 }, // Vanillish
            new EncounterStatic8N(Nest72,2,3,3) { Species = 713, Ability =  4 }, // Avalugg
            new EncounterStatic8N(Nest72,2,4,4) { Species = 873, Ability =  4 }, // Frosmoth
            new EncounterStatic8N(Nest72,3,4,4) { Species = 584, Ability = -1 }, // Vanilluxe
            new EncounterStatic8N(Nest72,3,4,4) { Species = 866, Ability = -1 }, // Mr. Rime
            new EncounterStatic8N(Nest72,4,4,4) { Species = 478, Ability = -1 }, // Froslass
            new EncounterStatic8N(Nest72,4,4,4) { Species = 471, Ability = -1 }, // Glaceon
            new EncounterStatic8N(Nest73,0,0,1) { Species = 175, Ability =  4 }, // Togepi
            new EncounterStatic8N(Nest73,0,1,1) { Species = 859, Ability =  4 }, // Impidimp
            new EncounterStatic8N(Nest73,0,1,1) { Species = 280, Ability =  4 }, // Ralts
            new EncounterStatic8N(Nest73,1,2,2) { Species = 176, Ability =  4 }, // Togetic
            new EncounterStatic8N(Nest73,1,2,2) { Species = 860, Ability =  4 }, // Morgrem
            new EncounterStatic8N(Nest73,2,4,4) { Species = 868, Ability =  4 }, // Milcery
            new EncounterStatic8N(Nest73,3,4,4) { Species = 282, Ability = -1 }, // Gardevoir
            new EncounterStatic8N(Nest73,3,4,4) { Species = 861, Ability = -1 }, // Grimmsnarl
            new EncounterStatic8N(Nest73,4,4,4) { Species = 468, Ability = -1 }, // Togekiss
            new EncounterStatic8N(Nest73,4,4,4) { Species = 700, Ability = -1 }, // Sylveon
            new EncounterStatic8N(Nest74,0,0,1) { Species = 129, Ability =  4 }, // Magikarp
            new EncounterStatic8N(Nest74,0,0,1) { Species = 751, Ability =  4 }, // Dewpider
            new EncounterStatic8N(Nest74,0,1,1) { Species = 194, Ability =  4 }, // Wooper
            new EncounterStatic8N(Nest74,0,1,1) { Species = 339, Ability =  4 }, // Barboach
            new EncounterStatic8N(Nest74,1,2,2) { Species = 098, Ability =  4 }, // Krabby
            new EncounterStatic8N(Nest74,1,2,2) { Species = 746, Ability =  4 }, // Wishiwashi
            new EncounterStatic8N(Nest74,2,3,3) { Species = 099, Ability =  4 }, // Kingler
            new EncounterStatic8N(Nest74,2,4,4) { Species = 340, Ability =  4 }, // Whiscash
            new EncounterStatic8N(Nest74,3,4,4) { Species = 211, Ability = -1 }, // Qwilfish
            new EncounterStatic8N(Nest74,3,4,4) { Species = 195, Ability = -1 }, // Quagsire
            new EncounterStatic8N(Nest74,4,4,4) { Species = 752, Ability = -1 }, // Araquanid
            new EncounterStatic8N(Nest74,4,4,4) { Species = 130, Ability = -1 }, // Gyarados
            new EncounterStatic8N(Nest75,0,0,1) { Species = 458, Ability =  4 }, // Mantyke
            new EncounterStatic8N(Nest75,0,0,1) { Species = 223, Ability =  4 }, // Remoraid
            new EncounterStatic8N(Nest75,0,1,1) { Species = 320, Ability =  4 }, // Wailmer
            new EncounterStatic8N(Nest75,0,1,1) { Species = 688, Ability =  4 }, // Binacle
            new EncounterStatic8N(Nest75,1,2,2) { Species = 098, Ability =  4 }, // Krabby
            new EncounterStatic8N(Nest75,1,2,2) { Species = 771, Ability =  4 }, // Pyukumuku
            new EncounterStatic8N(Nest75,2,3,3) { Species = 099, Ability =  4 }, // Kingler
            new EncounterStatic8N(Nest75,3,4,4) { Species = 211, Ability = -1 }, // Qwilfish
            new EncounterStatic8N(Nest75,3,4,4) { Species = 224, Ability = -1 }, // Octillery
            new EncounterStatic8N(Nest75,4,4,4) { Species = 321, Ability = -1 }, // Wailord
            new EncounterStatic8N(Nest75,4,4,4) { Species = 226, Ability = -1 }, // Mantine
            new EncounterStatic8N(Nest76,0,0,1) { Species = 850, Ability =  4 }, // Sizzlipede
            new EncounterStatic8N(Nest76,0,1,1) { Species = 607, Ability =  4 }, // Litwick
            new EncounterStatic8N(Nest76,0,1,1) { Species = 004, Ability =  4 }, // Charmander
            new EncounterStatic8N(Nest76,1,2,2) { Species = 005, Ability =  4 }, // Charmeleon
            new EncounterStatic8N(Nest76,2,3,3) { Species = 631, Ability =  4 }, // Heatmor
            new EncounterStatic8N(Nest76,2,4,4) { Species = 324, Ability =  4 }, // Torkoal
            new EncounterStatic8N(Nest76,3,4,4) { Species = 758, Ability = -1, Gender = 1 }, // Salazzle
            new EncounterStatic8N(Nest76,4,4,4) { Species = 851, Ability = -1 }, // Centiskorch
            new EncounterStatic8N(Nest76,4,4,4) { Species = 006, Ability = -1, CanGigantamax = true }, // Charizard
            new EncounterStatic8N(Nest77,0,0,1) { Species = 129, Ability =  4 }, // Magikarp
            new EncounterStatic8N(Nest77,0,0,1) { Species = 846, Ability =  4 }, // Arrokuda
            new EncounterStatic8N(Nest77,0,1,1) { Species = 833, Ability =  4 }, // Chewtle
            new EncounterStatic8N(Nest77,0,1,1) { Species = 098, Ability =  4 }, // Krabby
            new EncounterStatic8N(Nest77,1,2,2) { Species = 771, Ability =  4 }, // Pyukumuku
            new EncounterStatic8N(Nest77,2,3,3) { Species = 211, Ability =  4 }, // Qwilfish
            new EncounterStatic8N(Nest77,2,4,4) { Species = 099, Ability =  4 }, // Kingler
            new EncounterStatic8N(Nest77,3,4,4) { Species = 746, Ability = -1 }, // Wishiwashi
            new EncounterStatic8N(Nest77,3,4,4) { Species = 130, Ability = -1 }, // Gyarados
            new EncounterStatic8N(Nest77,4,4,4) { Species = 423, Ability = -1, Form = 1 }, // Gastrodon-1
            new EncounterStatic8N(Nest77,4,4,4) { Species = 834, Ability = -1, CanGigantamax = true }, // Drednaw
            new EncounterStatic8N(Nest78,0,0,1) { Species = 406, Ability =  4 }, // Budew
            new EncounterStatic8N(Nest78,0,1,1) { Species = 829, Ability =  4 }, // Gossifleur
            new EncounterStatic8N(Nest78,0,1,1) { Species = 597, Ability =  4 }, // Ferroseed
            new EncounterStatic8N(Nest78,1,2,2) { Species = 840, Ability =  4 }, // Applin
            new EncounterStatic8N(Nest78,2,4,4) { Species = 315, Ability =  4 }, // Roselia
            new EncounterStatic8N(Nest78,3,4,4) { Species = 830, Ability = -1 }, // Eldegoss
            new EncounterStatic8N(Nest78,3,4,4) { Species = 598, Ability = -1 }, // Ferrothorn
            new EncounterStatic8N(Nest78,4,4,4) { Species = 407, Ability = -1 }, // Roserade
            new EncounterStatic8N(Nest79,0,0,1) { Species = 850, Ability =  4 }, // Sizzlipede
            new EncounterStatic8N(Nest79,0,1,1) { Species = 607, Ability =  4 }, // Litwick
            new EncounterStatic8N(Nest79,0,1,1) { Species = 757, Ability =  4 }, // Salandit
            new EncounterStatic8N(Nest79,1,2,2) { Species = 838, Ability =  4 }, // Carkol
            new EncounterStatic8N(Nest79,1,2,2) { Species = 608, Ability =  4 }, // Lampent
            new EncounterStatic8N(Nest79,2,3,3) { Species = 631, Ability =  4 }, // Heatmor
            new EncounterStatic8N(Nest79,2,4,4) { Species = 324, Ability =  4 }, // Torkoal
            new EncounterStatic8N(Nest79,3,4,4) { Species = 609, Ability = -1 }, // Chandelure
            new EncounterStatic8N(Nest79,4,4,4) { Species = 839, Ability = -1 }, // Coalossal
            new EncounterStatic8N(Nest79,4,4,4) { Species = 851, Ability = -1, CanGigantamax = true }, // Centiskorch
            new EncounterStatic8N(Nest81,0,0,1) { Species = 175, Ability =  4 }, // Togepi
            new EncounterStatic8N(Nest81,0,1,1) { Species = 859, Ability =  4 }, // Impidimp
            new EncounterStatic8N(Nest81,0,1,1) { Species = 280, Ability =  4 }, // Ralts
            new EncounterStatic8N(Nest81,1,2,2) { Species = 176, Ability =  4 }, // Togetic
            new EncounterStatic8N(Nest81,1,2,2) { Species = 756, Ability =  4 }, // Shiinotic
            new EncounterStatic8N(Nest81,2,3,3) { Species = 860, Ability =  4 }, // Morgrem
            new EncounterStatic8N(Nest81,3,4,4) { Species = 282, Ability = -1 }, // Gardevoir
            new EncounterStatic8N(Nest81,3,4,4) { Species = 468, Ability = -1 }, // Togekiss
            new EncounterStatic8N(Nest81,4,4,4) { Species = 861, Ability = -1 }, // Grimmsnarl
            new EncounterStatic8N(Nest81,4,4,4) { Species = 869, Ability = -1, CanGigantamax = true }, // Alcremie
            new EncounterStatic8N(Nest83,0,0,1) { Species = 447, Ability =  4 }, // Riolu
            new EncounterStatic8N(Nest83,0,0,1) { Species = 436, Ability =  4 }, // Bronzor
            new EncounterStatic8N(Nest83,0,1,1) { Species = 624, Ability =  4 }, // Pawniard
            new EncounterStatic8N(Nest83,0,1,1) { Species = 599, Ability =  4 }, // Klink
            new EncounterStatic8N(Nest83,1,2,2) { Species = 095, Ability =  4 }, // Onix
            new EncounterStatic8N(Nest83,2,4,4) { Species = 437, Ability =  4 }, // Bronzong
            new EncounterStatic8N(Nest83,3,4,4) { Species = 625, Ability = -1 }, // Bisharp
            new EncounterStatic8N(Nest83,3,4,4) { Species = 208, Ability = -1 }, // Steelix
            new EncounterStatic8N(Nest83,4,4,4) { Species = 601, Ability = -1 }, // Klinklang
            new EncounterStatic8N(Nest83,4,4,4) { Species = 884, Ability = -1, CanGigantamax = true }, // Duraludon
            new EncounterStatic8N(Nest84,0,0,1) { Species = 052, Ability =  4, Form = 2 }, // Meowth-2
            new EncounterStatic8N(Nest84,0,0,1) { Species = 436, Ability =  4 }, // Bronzor
            new EncounterStatic8N(Nest84,0,1,1) { Species = 624, Ability =  4 }, // Pawniard
            new EncounterStatic8N(Nest84,0,1,1) { Species = 597, Ability =  4 }, // Ferroseed
            new EncounterStatic8N(Nest84,1,2,2) { Species = 679, Ability =  4 }, // Honedge
            new EncounterStatic8N(Nest84,1,2,2) { Species = 437, Ability =  4 }, // Bronzong
            new EncounterStatic8N(Nest84,2,3,3) { Species = 863, Ability =  4 }, // Perrserker
            new EncounterStatic8N(Nest84,2,4,4) { Species = 598, Ability =  4 }, // Ferrothorn
            new EncounterStatic8N(Nest84,3,4,4) { Species = 625, Ability = -1 }, // Bisharp
            new EncounterStatic8N(Nest84,3,4,4) { Species = 618, Ability = -1, Form = 1 }, // Stunfisk-1
            new EncounterStatic8N(Nest84,4,4,4) { Species = 884, Ability = -1 }, // Duraludon
            new EncounterStatic8N(Nest84,4,4,4) { Species = 879, Ability = -1, CanGigantamax = true }, // Copperajah
            new EncounterStatic8N(Nest85,0,0,1) { Species = 434, Ability =  4 }, // Stunky
            new EncounterStatic8N(Nest85,0,0,1) { Species = 568, Ability =  4 }, // Trubbish
            new EncounterStatic8N(Nest85,0,1,1) { Species = 451, Ability =  4 }, // Skorupi
            new EncounterStatic8N(Nest85,0,1,1) { Species = 109, Ability =  4 }, // Koffing
            new EncounterStatic8N(Nest85,1,2,2) { Species = 848, Ability =  4 }, // Toxel
            new EncounterStatic8N(Nest85,2,3,3) { Species = 452, Ability =  4 }, // Drapion
            new EncounterStatic8N(Nest85,2,4,4) { Species = 849, Ability =  4 }, // Toxtricity
            new EncounterStatic8N(Nest85,3,4,4) { Species = 435, Ability = -1 }, // Skuntank
            new EncounterStatic8N(Nest85,3,4,4) { Species = 110, Ability = -1, Form = 1 }, // Weezing-1
            new EncounterStatic8N(Nest85,4,4,4) { Species = 569, Ability = -1, CanGigantamax = true }, // Garbodor
            new EncounterStatic8N(Nest86,0,0,1) { Species = 175, Ability =  4 }, // Togepi
            new EncounterStatic8N(Nest86,0,1,1) { Species = 859, Ability =  4 }, // Impidimp
            new EncounterStatic8N(Nest86,0,1,1) { Species = 280, Ability =  4 }, // Ralts
            new EncounterStatic8N(Nest86,1,2,2) { Species = 176, Ability =  4 }, // Togetic
            new EncounterStatic8N(Nest86,1,2,2) { Species = 860, Ability =  4 }, // Morgrem
            new EncounterStatic8N(Nest86,2,4,4) { Species = 868, Ability =  4 }, // Milcery
            new EncounterStatic8N(Nest86,3,4,4) { Species = 282, Ability = -1 }, // Gardevoir
            new EncounterStatic8N(Nest86,3,4,4) { Species = 861, Ability = -1 }, // Grimmsnarl
            new EncounterStatic8N(Nest86,4,4,4) { Species = 468, Ability = -1 }, // Togekiss
            new EncounterStatic8N(Nest86,4,4,4) { Species = 858, Ability = -1, CanGigantamax = true }, // Hatterene
            new EncounterStatic8N(Nest87,0,0,1) { Species = 827, Ability =  4 }, // Nickit
            new EncounterStatic8N(Nest87,0,0,1) { Species = 263, Ability =  4, Form = 1 }, // Zigzagoon-1
            new EncounterStatic8N(Nest87,0,1,1) { Species = 859, Ability =  4 }, // Impidimp
            new EncounterStatic8N(Nest87,1,2,2) { Species = 510, Ability =  4 }, // Liepard
            new EncounterStatic8N(Nest87,1,2,2) { Species = 264, Ability =  4, Form = 1 }, // Linoone-1
            new EncounterStatic8N(Nest87,2,3,3) { Species = 860, Ability =  4 }, // Morgrem
            new EncounterStatic8N(Nest87,2,4,4) { Species = 828, Ability =  4 }, // Thievul
            new EncounterStatic8N(Nest87,3,4,4) { Species = 675, Ability = -1 }, // Pangoro
            new EncounterStatic8N(Nest87,4,4,4) { Species = 861, Ability = -1, CanGigantamax = true }, // Grimmsnarl
            new EncounterStatic8N(Nest88,0,0,1) { Species = 177, Ability =  4 }, // Natu
            new EncounterStatic8N(Nest88,0,0,1) { Species = 163, Ability =  4 }, // Hoothoot
            new EncounterStatic8N(Nest88,0,1,1) { Species = 821, Ability =  4 }, // Rookidee
            new EncounterStatic8N(Nest88,0,1,1) { Species = 278, Ability =  4 }, // Wingull
            new EncounterStatic8N(Nest88,1,2,2) { Species = 012, Ability =  4 }, // Butterfree
            new EncounterStatic8N(Nest88,1,2,2) { Species = 822, Ability =  4 }, // Corvisquire
            new EncounterStatic8N(Nest88,2,3,3) { Species = 164, Ability =  4 }, // Noctowl
            new EncounterStatic8N(Nest88,2,4,4) { Species = 279, Ability =  4 }, // Pelipper
            new EncounterStatic8N(Nest88,3,4,4) { Species = 178, Ability = -1 }, // Xatu
            new EncounterStatic8N(Nest88,3,4,4) { Species = 701, Ability = -1 }, // Hawlucha
            new EncounterStatic8N(Nest88,4,4,4) { Species = 561, Ability = -1 }, // Sigilyph
            new EncounterStatic8N(Nest88,4,4,4) { Species = 823, Ability = -1, CanGigantamax = true }, // Corviknight
            new EncounterStatic8N(Nest89,0,0,1) { Species = 767, Ability =  4 }, // Wimpod
            new EncounterStatic8N(Nest89,0,0,1) { Species = 824, Ability =  4 }, // Blipbug
            new EncounterStatic8N(Nest89,0,1,1) { Species = 751, Ability =  4 }, // Dewpider
            new EncounterStatic8N(Nest89,1,2,2) { Species = 557, Ability =  4 }, // Dwebble
            new EncounterStatic8N(Nest89,2,3,3) { Species = 825, Ability =  4 }, // Dottler
            new EncounterStatic8N(Nest89,2,4,4) { Species = 826, Ability =  4 }, // Orbeetle
            new EncounterStatic8N(Nest89,3,4,4) { Species = 752, Ability = -1 }, // Araquanid
            new EncounterStatic8N(Nest89,3,4,4) { Species = 768, Ability = -1 }, // Golisopod
            new EncounterStatic8N(Nest89,0,4,4) { Species = 012, Ability = -1, CanGigantamax = true }, // Butterfree
            new EncounterStatic8N(Nest90,0,0,1) { Species = 341, Ability =  4 }, // Corphish
            new EncounterStatic8N(Nest90,0,0,1) { Species = 098, Ability =  4 }, // Krabby
            new EncounterStatic8N(Nest90,0,1,1) { Species = 846, Ability =  4 }, // Arrokuda
            new EncounterStatic8N(Nest90,0,1,1) { Species = 833, Ability =  4 }, // Chewtle
            new EncounterStatic8N(Nest90,1,2,2) { Species = 747, Ability =  4 }, // Mareanie
            new EncounterStatic8N(Nest90,2,3,3) { Species = 342, Ability =  4 }, // Crawdaunt
            new EncounterStatic8N(Nest90,2,4,4) { Species = 748, Ability =  4 }, // Toxapex
            new EncounterStatic8N(Nest90,3,4,4) { Species = 771, Ability = -1 }, // Pyukumuku
            new EncounterStatic8N(Nest90,3,4,4) { Species = 130, Ability = -1 }, // Gyarados
            new EncounterStatic8N(Nest90,4,4,4) { Species = 131, Ability = -1 }, // Lapras
            new EncounterStatic8N(Nest90,1,4,4) { Species = 099, Ability = -1, CanGigantamax = true }, // Kingler
            new EncounterStatic8N(Nest91,0,0,1) { Species = 767, Ability =  4 }, // Wimpod
            new EncounterStatic8N(Nest91,0,0,1) { Species = 824, Ability =  4 }, // Blipbug
            new EncounterStatic8N(Nest91,0,1,1) { Species = 751, Ability =  4 }, // Dewpider
            new EncounterStatic8N(Nest91,1,2,2) { Species = 557, Ability =  4 }, // Dwebble
            new EncounterStatic8N(Nest91,2,3,3) { Species = 825, Ability =  4 }, // Dottler
            new EncounterStatic8N(Nest91,2,4,4) { Species = 826, Ability =  4 }, // Orbeetle
            new EncounterStatic8N(Nest91,3,4,4) { Species = 752, Ability = -1 }, // Araquanid
            new EncounterStatic8N(Nest91,3,4,4) { Species = 768, Ability = -1 }, // Golisopod
            new EncounterStatic8N(Nest91,2,4,4) { Species = 826, Ability = -1, CanGigantamax = true }, // Orbeetle
            new EncounterStatic8N(Nest92,0,0,1) { Species = 194, Ability =  4 }, // Wooper
            new EncounterStatic8N(Nest92,0,0,1) { Species = 339, Ability =  4 }, // Barboach
            new EncounterStatic8N(Nest92,0,1,1) { Species = 562, Ability =  4, Form = 1 }, // Yamask-1
            new EncounterStatic8N(Nest92,0,1,1) { Species = 622, Ability =  4 }, // Golett
            new EncounterStatic8N(Nest92,1,2,2) { Species = 536, Ability =  4 }, // Palpitoad
            new EncounterStatic8N(Nest92,1,2,2) { Species = 195, Ability =  4 }, // Quagsire
            new EncounterStatic8N(Nest92,2,3,3) { Species = 618, Ability =  4, Form = 1 }, // Stunfisk-1
            new EncounterStatic8N(Nest92,2,4,4) { Species = 623, Ability =  4 }, // Golurk
            new EncounterStatic8N(Nest92,3,4,4) { Species = 423, Ability = -1, Form = 1 }, // Gastrodon-1
            new EncounterStatic8N(Nest92,3,4,4) { Species = 537, Ability = -1 }, // Seismitoad
            new EncounterStatic8N(Nest92,4,4,4) { Species = 464, Ability = -1 }, // Rhyperior
            new EncounterStatic8N(Nest92,3,4,4) { Species = 844, Ability = -1, CanGigantamax = true }, // Sandaconda
        };

        internal static readonly EncounterStatic8N[] Nest_SW =
        {
            new EncounterStatic8N(Nest00,0,1,1) { Species = 559, Ability =  4 }, // Scraggy
            new EncounterStatic8N(Nest00,2,3,3) { Species = 106, Ability =  4 }, // Hitmonlee
            new EncounterStatic8N(Nest00,2,4,4) { Species = 107, Ability =  4 }, // Hitmonchan
            new EncounterStatic8N(Nest00,2,4,4) { Species = 560, Ability =  4 }, // Scrafty
            new EncounterStatic8N(Nest00,3,4,4) { Species = 534, Ability = -1 }, // Conkeldurr
            new EncounterStatic8N(Nest00,4,4,4) { Species = 237, Ability = -1 }, // Hitmontop
            new EncounterStatic8N(Nest01,0,1,1) { Species = 574, Ability =  4 }, // Gothita
            new EncounterStatic8N(Nest01,2,3,3) { Species = 678, Ability =  4, Gender = 0 }, // Meowstic
            new EncounterStatic8N(Nest01,2,3,3) { Species = 575, Ability =  4 }, // Gothorita
            new EncounterStatic8N(Nest01,3,4,4) { Species = 576, Ability = -1 }, // Gothitelle
            new EncounterStatic8N(Nest01,4,4,4) { Species = 338, Ability = -1 }, // Solrock
            new EncounterStatic8N(Nest02,0,0,1) { Species = 524, Ability =  4 }, // Roggenrola
            new EncounterStatic8N(Nest02,0,1,1) { Species = 688, Ability =  4 }, // Binacle
            new EncounterStatic8N(Nest02,3,4,4) { Species = 558, Ability = -1 }, // Crustle
            new EncounterStatic8N(Nest02,4,4,4) { Species = 526, Ability = -1 }, // Gigalith
            new EncounterStatic8N(Nest06,0,1,1) { Species = 223, Ability =  4 }, // Remoraid
            new EncounterStatic8N(Nest06,0,1,1) { Species = 170, Ability =  4 }, // Chinchou
            new EncounterStatic8N(Nest06,1,2,2) { Species = 550, Ability =  4 }, // Basculin
            new EncounterStatic8N(Nest07,1,2,2) { Species = 550, Ability =  4 }, // Basculin
            new EncounterStatic8N(Nest08,1,1,2) { Species = 090, Ability =  4 }, // Shellder
            new EncounterStatic8N(Nest09,1,1,2) { Species = 083, Ability =  4, Form = 1 }, // Farfetch’d-1
            new EncounterStatic8N(Nest09,1,2,2) { Species = 539, Ability =  4 }, // Sawk
            new EncounterStatic8N(Nest09,3,4,4) { Species = 865, Ability = -1 }, // Sirfetch’d
            new EncounterStatic8N(Nest11,4,4,4) { Species = 303, Ability = -1 }, // Mawile
            new EncounterStatic8N(Nest12,0,1,1) { Species = 177, Ability =  4 }, // Natu
            new EncounterStatic8N(Nest12,0,1,1) { Species = 856, Ability =  4 }, // Hatenna
            new EncounterStatic8N(Nest12,1,1,2) { Species = 825, Ability =  4 }, // Dottler
            new EncounterStatic8N(Nest12,1,3,2) { Species = 857, Ability =  4 }, // Hattrem
            new EncounterStatic8N(Nest12,2,4,4) { Species = 876, Ability =  4, Gender = 0 }, // Indeedee
            new EncounterStatic8N(Nest12,3,4,4) { Species = 561, Ability = -1 }, // Sigilyph
            new EncounterStatic8N(Nest12,4,4,4) { Species = 826, Ability = -1 }, // Orbeetle
            new EncounterStatic8N(Nest13,2,4,4) { Species = 876, Ability =  4, Gender = 0 }, // Indeedee
            new EncounterStatic8N(Nest14,0,0,1) { Species = 524, Ability =  4 }, // Roggenrola
            new EncounterStatic8N(Nest14,0,1,1) { Species = 557, Ability =  4 }, // Dwebble
            new EncounterStatic8N(Nest14,2,4,4) { Species = 095, Ability =  4 }, // Onix
            new EncounterStatic8N(Nest14,3,4,4) { Species = 839, Ability = -1 }, // Coalossal
            new EncounterStatic8N(Nest14,4,4,4) { Species = 526, Ability = -1 }, // Gigalith
            new EncounterStatic8N(Nest16,0,1,1) { Species = 220, Ability =  4 }, // Swinub
            new EncounterStatic8N(Nest16,1,1,1) { Species = 328, Ability =  4 }, // Trapinch
            new EncounterStatic8N(Nest16,2,3,3) { Species = 329, Ability =  4 }, // Vibrava
            new EncounterStatic8N(Nest16,2,4,4) { Species = 618, Ability =  4, Form = 1 }, // Stunfisk-1
            new EncounterStatic8N(Nest16,3,4,4) { Species = 450, Ability = -1 }, // Hippowdon
            new EncounterStatic8N(Nest16,4,4,4) { Species = 330, Ability = -1 }, // Flygon
            new EncounterStatic8N(Nest17,0,0,1) { Species = 037, Ability =  4 }, // Vulpix
            new EncounterStatic8N(Nest17,1,1,1) { Species = 554, Ability =  4, Form = 1 }, // Darumaka-1
            new EncounterStatic8N(Nest17,1,2,2) { Species = 758, Ability =  4, Gender = 1 }, // Salazzle
            new EncounterStatic8N(Nest17,2,3,3) { Species = 608, Ability =  4 }, // Lampent
            new EncounterStatic8N(Nest17,2,4,4) { Species = 038, Ability =  4 }, // Ninetales
            new EncounterStatic8N(Nest17,3,4,4) { Species = 851, Ability = -1 }, // Centiskorch
            new EncounterStatic8N(Nest17,4,4,4) { Species = 631, Ability = -1 }, // Heatmor
            new EncounterStatic8N(Nest17,4,4,4) { Species = 555, Ability = -1, Form = 2 }, // Darmanitan-2
            new EncounterStatic8N(Nest18,0,0,1) { Species = 037, Ability =  4 }, // Vulpix
            new EncounterStatic8N(Nest18,0,1,1) { Species = 037, Ability =  4 }, // Vulpix
            new EncounterStatic8N(Nest18,1,2,2) { Species = 758, Ability =  4, Gender = 1 }, // Salazzle
            new EncounterStatic8N(Nest18,2,3,3) { Species = 608, Ability =  4 }, // Lampent
            new EncounterStatic8N(Nest18,2,4,4) { Species = 038, Ability =  4 }, // Ninetales
            new EncounterStatic8N(Nest18,2,4,4) { Species = 324, Ability =  4 }, // Torkoal
            new EncounterStatic8N(Nest18,3,4,4) { Species = 758, Ability = -1, Gender = 1 }, // Salazzle
            new EncounterStatic8N(Nest18,4,4,4) { Species = 776, Ability = -1 }, // Turtonator
            new EncounterStatic8N(Nest19,0,0,1) { Species = 037, Ability =  4 }, // Vulpix
            new EncounterStatic8N(Nest19,1,1,1) { Species = 554, Ability =  4, Form = 1 }, // Darumaka-1
            new EncounterStatic8N(Nest19,1,2,2) { Species = 758, Ability =  4, Gender = 1 }, // Salazzle
            new EncounterStatic8N(Nest19,2,3,3) { Species = 838, Ability =  4 }, // Carkol
            new EncounterStatic8N(Nest19,2,4,4) { Species = 038, Ability =  4 }, // Ninetales
            new EncounterStatic8N(Nest19,4,4,4) { Species = 555, Ability = -1, Form = 2 }, // Darmanitan-2
            new EncounterStatic8N(Nest22,0,1,1) { Species = 554, Ability =  4, Form = 1 }, // Darumaka-1
            new EncounterStatic8N(Nest22,4,4,4) { Species = 555, Ability = -1, Form = 2 }, // Darmanitan-2
            new EncounterStatic8N(Nest25,0,0,1) { Species = 273, Ability =  4 }, // Seedot
            new EncounterStatic8N(Nest25,1,1,2) { Species = 274, Ability =  4 }, // Nuzleaf
            new EncounterStatic8N(Nest25,2,4,4) { Species = 275, Ability =  4 }, // Shiftry
            new EncounterStatic8N(Nest26,4,4,4) { Species = 841, Ability = -1 }, // Flapple
            new EncounterStatic8N(Nest28,0,1,1) { Species = 747, Ability =  4 }, // Mareanie
            new EncounterStatic8N(Nest28,1,1,2) { Species = 043, Ability =  4 }, // Oddish
            new EncounterStatic8N(Nest28,3,4,4) { Species = 748, Ability = -1 }, // Toxapex
            new EncounterStatic8N(Nest28,4,4,4) { Species = 435, Ability = -1 }, // Skuntank
            new EncounterStatic8N(Nest30,0,1,1) { Species = 627, Ability =  4 }, // Rufflet
            new EncounterStatic8N(Nest30,4,4,4) { Species = 628, Ability = -1 }, // Braviary
            new EncounterStatic8N(Nest32,0,1,1) { Species = 684, Ability =  4 }, // Swirlix
            new EncounterStatic8N(Nest32,4,4,4) { Species = 685, Ability = -1 }, // Slurpuff
            new EncounterStatic8N(Nest33,4,4,4) { Species = 303, Ability = -1 }, // Mawile
            new EncounterStatic8N(Nest34,4,4,4) { Species = 275, Ability = -1 }, // Shiftry
            new EncounterStatic8N(Nest35,1,1,2) { Species = 633, Ability =  4 }, // Deino
            new EncounterStatic8N(Nest35,3,4,4) { Species = 634, Ability = -1 }, // Zweilous
            new EncounterStatic8N(Nest35,4,4,4) { Species = 635, Ability = -1 }, // Hydreigon
            new EncounterStatic8N(Nest36,0,0,1) { Species = 328, Ability =  4 }, // Trapinch
            new EncounterStatic8N(Nest36,0,1,1) { Species = 610, Ability =  4 }, // Axew
            new EncounterStatic8N(Nest36,1,1,2) { Species = 782, Ability =  4 }, // Jangmo-o
            new EncounterStatic8N(Nest36,2,3,3) { Species = 783, Ability =  4 }, // Hakamo-o
            new EncounterStatic8N(Nest36,2,4,4) { Species = 611, Ability =  4 }, // Fraxure
            new EncounterStatic8N(Nest36,2,4,4) { Species = 612, Ability =  4 }, // Haxorus
            new EncounterStatic8N(Nest36,3,4,4) { Species = 330, Ability = -1 }, // Flygon
            new EncounterStatic8N(Nest36,4,4,4) { Species = 776, Ability = -1 }, // Turtonator
            new EncounterStatic8N(Nest36,4,4,4) { Species = 784, Ability = -1 }, // Kommo-o
            new EncounterStatic8N(Nest37,0,1,1) { Species = 782, Ability =  4 }, // Jangmo-o
            new EncounterStatic8N(Nest37,2,4,4) { Species = 783, Ability =  4 }, // Hakamo-o
            new EncounterStatic8N(Nest37,3,4,4) { Species = 784, Ability = -1 }, // Kommo-o
            new EncounterStatic8N(Nest37,4,4,4) { Species = 841, Ability = -1 }, // Flapple
            new EncounterStatic8N(Nest39,1,1,2) { Species = 876, Ability =  4, Gender = 0 }, // Indeedee
            new EncounterStatic8N(Nest39,4,4,4) { Species = 628, Ability = -1 }, // Braviary
            new EncounterStatic8N(Nest40,0,1,1) { Species = 747, Ability =  4 }, // Mareanie
            new EncounterStatic8N(Nest40,1,1,2) { Species = 536, Ability =  4 }, // Palpitoad
            new EncounterStatic8N(Nest40,2,3,3) { Species = 091, Ability =  4 }, // Cloyster
            new EncounterStatic8N(Nest40,2,4,4) { Species = 746, Ability =  4 }, // Wishiwashi
            new EncounterStatic8N(Nest40,3,4,4) { Species = 537, Ability = -1 }, // Seismitoad
            new EncounterStatic8N(Nest40,4,4,4) { Species = 748, Ability = -1 }, // Toxapex
            new EncounterStatic8N(Nest42,1,3,2) { Species = 710, Ability =  4 }, // Pumpkaboo
            new EncounterStatic8N(Nest42,4,4,4) { Species = 867, Ability =  4 }, // Runerigus
            new EncounterStatic8N(Nest42,3,4,4) { Species = 855, Ability = -1 }, // Polteageist
            new EncounterStatic8N(Nest42,3,4,4) { Species = 711, Ability = -1 }, // Gourgeist
            new EncounterStatic8N(Nest43,1,2,2) { Species = 550, Ability =  4 }, // Basculin
            new EncounterStatic8N(Nest44,1,2,2) { Species = 632, Ability =  4 }, // Durant
            new EncounterStatic8N(Nest44,2,3,3) { Species = 600, Ability =  4 }, // Klang
            new EncounterStatic8N(Nest45,0,1,1) { Species = 588, Ability =  4 }, // Karrablast
            new EncounterStatic8N(Nest45,1,2,2) { Species = 616, Ability =  4 }, // Shelmet
            new EncounterStatic8N(Nest45,4,4,4) { Species = 589, Ability = -1 }, // Escavalier
            new EncounterStatic8N(Nest46,1,3,3) { Species = 710, Ability =  4 }, // Pumpkaboo
            new EncounterStatic8N(Nest46,2,4,4) { Species = 711, Ability =  4 }, // Gourgeist
            new EncounterStatic8N(Nest46,3,4,4) { Species = 711, Ability = -1 }, // Gourgeist
            new EncounterStatic8N(Nest47,0,1,1) { Species = 559, Ability =  4 }, // Scraggy
            new EncounterStatic8N(Nest47,2,4,4) { Species = 560, Ability =  4 }, // Scrafty
            new EncounterStatic8N(Nest47,3,4,4) { Species = 766, Ability = -1 }, // Passimian
            new EncounterStatic8N(Nest50,0,1,1) { Species = 688, Ability =  4 }, // Binacle
            new EncounterStatic8N(Nest50,1,2,2) { Species = 185, Ability =  4 }, // Sudowoodo
            new EncounterStatic8N(Nest50,2,3,3) { Species = 689, Ability =  4 }, // Barbaracle
            new EncounterStatic8N(Nest50,4,4,4) { Species = 874, Ability = -1 }, // Stonjourner
            new EncounterStatic8N(Nest52,0,0,1) { Species = 037, Ability =  4 }, // Vulpix
            new EncounterStatic8N(Nest52,1,2,2) { Species = 038, Ability =  4 }, // Ninetales
            new EncounterStatic8N(Nest52,3,4,4) { Species = 038, Ability = -1 }, // Ninetales
            new EncounterStatic8N(Nest53,0,0,1) { Species = 037, Ability =  4 }, // Vulpix
            new EncounterStatic8N(Nest53,1,2,2) { Species = 608, Ability =  4 }, // Lampent
            new EncounterStatic8N(Nest53,2,3,3) { Species = 631, Ability =  4 }, // Heatmor
            new EncounterStatic8N(Nest53,3,4,4) { Species = 038, Ability = -1 }, // Ninetales
            new EncounterStatic8N(Nest53,4,4,4) { Species = 776, Ability = -1 }, // Turtonator
            new EncounterStatic8N(Nest54,0,0,1) { Species = 554, Ability =  4, Form = 1 }, // Darumaka-1
            new EncounterStatic8N(Nest54,4,4,4) { Species = 555, Ability = -1, Form = 2 }, // Darmanitan-2
            new EncounterStatic8N(Nest57,0,0,1) { Species = 273, Ability =  4 }, // Seedot
            new EncounterStatic8N(Nest57,1,2,2) { Species = 274, Ability =  4 }, // Nuzleaf
            new EncounterStatic8N(Nest57,2,4,4) { Species = 275, Ability =  4 }, // Shiftry
            new EncounterStatic8N(Nest57,4,4,4) { Species = 841, Ability = -1 }, // Flapple
            new EncounterStatic8N(Nest58,0,0,1) { Species = 273, Ability =  4 }, // Seedot
            new EncounterStatic8N(Nest58,1,2,2) { Species = 274, Ability =  4 }, // Nuzleaf
            new EncounterStatic8N(Nest58,4,4,4) { Species = 275, Ability = -1 }, // Shiftry
            new EncounterStatic8N(Nest59,1,2,2) { Species = 747, Ability =  4 }, // Mareanie
            new EncounterStatic8N(Nest59,4,4,4) { Species = 748, Ability = -1 }, // Toxapex
            new EncounterStatic8N(Nest61,2,4,4) { Species = 303, Ability =  4 }, // Mawile
            new EncounterStatic8N(Nest62,0,1,1) { Species = 559, Ability =  4 }, // Scraggy
            new EncounterStatic8N(Nest62,3,4,4) { Species = 560, Ability = -1 }, // Scrafty
            new EncounterStatic8N(Nest62,4,4,4) { Species = 635, Ability = -1 }, // Hydreigon
            new EncounterStatic8N(Nest63,0,0,1) { Species = 328, Ability =  4 }, // Trapinch
            new EncounterStatic8N(Nest63,0,1,1) { Species = 610, Ability =  4 }, // Axew
            new EncounterStatic8N(Nest63,0,1,1) { Species = 782, Ability =  4 }, // Jangmo-o
            new EncounterStatic8N(Nest63,1,2,2) { Species = 611, Ability =  4 }, // Fraxure
            new EncounterStatic8N(Nest63,2,4,4) { Species = 783, Ability =  4 }, // Hakamo-o
            new EncounterStatic8N(Nest63,2,4,4) { Species = 776, Ability =  4 }, // Turtonator
            new EncounterStatic8N(Nest63,3,4,4) { Species = 784, Ability = -1 }, // Kommo-o
            new EncounterStatic8N(Nest63,4,4,4) { Species = 612, Ability = -1 }, // Haxorus
            new EncounterStatic8N(Nest64,3,4,4) { Species = 628, Ability = -1 }, // Braviary
            new EncounterStatic8N(Nest64,3,4,4) { Species = 876, Ability = -1, Gender = 0 }, // Indeedee
            new EncounterStatic8N(Nest66,1,2,2) { Species = 550, Ability =  4 }, // Basculin
            new EncounterStatic8N(Nest70,0,0,1) { Species = 037, Ability =  4 }, // Vulpix
            new EncounterStatic8N(Nest70,0,1,1) { Species = 037, Ability =  4 }, // Vulpix
            new EncounterStatic8N(Nest70,1,2,2) { Species = 608, Ability =  4 }, // Lampent
            new EncounterStatic8N(Nest70,2,3,3) { Species = 631, Ability =  4 }, // Heatmor
            new EncounterStatic8N(Nest73,0,0,1) { Species = 684, Ability =  4 }, // Swirlix
            new EncounterStatic8N(Nest73,2,4,4) { Species = 685, Ability =  4 }, // Slurpuff
            new EncounterStatic8N(Nest75,2,4,4) { Species = 550, Ability =  4 }, // Basculin
            new EncounterStatic8N(Nest76,0,0,1) { Species = 037, Ability =  4 }, // Vulpix
            new EncounterStatic8N(Nest76,1,2,2) { Species = 038, Ability =  4 }, // Ninetales
            new EncounterStatic8N(Nest76,3,4,4) { Species = 038, Ability = -1 }, // Ninetales
            new EncounterStatic8N(Nest77,1,2,2) { Species = 550, Ability =  4 }, // Basculin
            new EncounterStatic8N(Nest78,0,0,1) { Species = 273, Ability =  4 }, // Seedot
            new EncounterStatic8N(Nest78,1,2,2) { Species = 274, Ability =  4 }, // Nuzleaf
            new EncounterStatic8N(Nest78,2,4,4) { Species = 275, Ability =  4 }, // Shiftry
            new EncounterStatic8N(Nest78,4,4,4) { Species = 841, Ability = -1, CanGigantamax = true }, // Flapple
            new EncounterStatic8N(Nest79,0,0,1) { Species = 037, Ability =  4 }, // Vulpix
            new EncounterStatic8N(Nest79,3,4,4) { Species = 038, Ability = -1 }, // Ninetales
            new EncounterStatic8N(Nest80,0,0,1) { Species = 447, Ability =  4 }, // Riolu
            new EncounterStatic8N(Nest80,0,0,1) { Species = 066, Ability =  4 }, // Machop
            new EncounterStatic8N(Nest80,0,1,1) { Species = 759, Ability =  4 }, // Stufful
            new EncounterStatic8N(Nest80,0,1,1) { Species = 083, Ability =  4, Form = 1 }, // Farfetch’d-1
            new EncounterStatic8N(Nest80,1,2,2) { Species = 760, Ability =  4 }, // Bewear
            new EncounterStatic8N(Nest80,1,3,3) { Species = 067, Ability =  4 }, // Machoke
            new EncounterStatic8N(Nest80,2,3,3) { Species = 870, Ability =  4 }, // Falinks
            new EncounterStatic8N(Nest80,2,4,4) { Species = 701, Ability =  4 }, // Hawlucha
            new EncounterStatic8N(Nest80,3,4,4) { Species = 448, Ability = -1 }, // Lucario
            new EncounterStatic8N(Nest80,3,4,4) { Species = 475, Ability = -1 }, // Gallade
            new EncounterStatic8N(Nest80,4,4,4) { Species = 865, Ability = -1 }, // Sirfetch’d
            new EncounterStatic8N(Nest80,4,4,4) { Species = 068, Ability = -1, CanGigantamax = true }, // Machamp
            new EncounterStatic8N(Nest81,0,0,1) { Species = 755, Ability =  4 }, // Morelull
            new EncounterStatic8N(Nest81,2,4,4) { Species = 303, Ability =  4 }, // Mawile
            new EncounterStatic8N(Nest82,0,0,1) { Species = 557, Ability =  4 }, // Dwebble
            new EncounterStatic8N(Nest82,0,0,1) { Species = 438, Ability =  4 }, // Bonsly
            new EncounterStatic8N(Nest82,0,1,1) { Species = 837, Ability =  4 }, // Rolycoly
            new EncounterStatic8N(Nest82,0,1,1) { Species = 688, Ability =  4 }, // Binacle
            new EncounterStatic8N(Nest82,1,2,2) { Species = 838, Ability =  4 }, // Carkol
            new EncounterStatic8N(Nest82,1,2,2) { Species = 185, Ability =  4 }, // Sudowoodo
            new EncounterStatic8N(Nest82,2,3,3) { Species = 689, Ability =  4 }, // Barbaracle
            new EncounterStatic8N(Nest82,2,4,4) { Species = 095, Ability =  4 }, // Onix
            new EncounterStatic8N(Nest82,3,4,4) { Species = 558, Ability = -1 }, // Crustle
            new EncounterStatic8N(Nest82,3,4,4) { Species = 208, Ability = -1 }, // Steelix
            new EncounterStatic8N(Nest82,4,4,4) { Species = 874, Ability = -1 }, // Stonjourner
            new EncounterStatic8N(Nest82,4,4,4) { Species = 839, Ability = -1, CanGigantamax = true }, // Coalossal
            new EncounterStatic8N(Nest83,1,2,2) { Species = 632, Ability =  4 }, // Durant
            new EncounterStatic8N(Nest83,2,3,3) { Species = 600, Ability =  4 }, // Klang
            new EncounterStatic8N(Nest85,1,2,2) { Species = 747, Ability =  4 }, // Mareanie
            new EncounterStatic8N(Nest85,4,4,4) { Species = 748, Ability = -1 }, // Toxapex
            new EncounterStatic8N(Nest86,0,0,1) { Species = 684, Ability =  4 }, // Swirlix
            new EncounterStatic8N(Nest86,2,3,3) { Species = 685, Ability =  4 }, // Slurpuff
            new EncounterStatic8N(Nest87,0,1,1) { Species = 559, Ability =  4 }, // Scraggy
            new EncounterStatic8N(Nest87,3,4,4) { Species = 560, Ability = -1 }, // Scrafty
            new EncounterStatic8N(Nest87,4,4,4) { Species = 635, Ability = -1 }, // Hydreigon
            new EncounterStatic8N(Nest89,0,1,1) { Species = 588, Ability =  4 }, // Karrablast
            new EncounterStatic8N(Nest89,1,2,2) { Species = 616, Ability =  4 }, // Shelmet
            new EncounterStatic8N(Nest89,4,4,4) { Species = 589, Ability = -1 }, // Escavalier
            new EncounterStatic8N(Nest90,1,2,2) { Species = 550, Ability =  4 }, // Basculin
            new EncounterStatic8N(Nest91,0,1,1) { Species = 588, Ability =  4 }, // Karrablast
            new EncounterStatic8N(Nest91,1,2,2) { Species = 616, Ability =  4 }, // Shelmet
            new EncounterStatic8N(Nest91,4,4,4) { Species = 589, Ability = -1 }, // Escavalier

        };

        internal static readonly EncounterStatic8N[] Nest_SH =
        {
            new EncounterStatic8N(Nest00,0,1,1) { Species = 453, Ability =  4 }, // Croagunk
            new EncounterStatic8N(Nest00,2,3,3) { Species = 107, Ability =  4 }, // Hitmonchan
            new EncounterStatic8N(Nest00,2,4,4) { Species = 106, Ability =  4 }, // Hitmonlee
            new EncounterStatic8N(Nest00,2,4,4) { Species = 454, Ability =  4 }, // Toxicroak
            new EncounterStatic8N(Nest00,3,4,4) { Species = 237, Ability = -1 }, // Hitmontop
            new EncounterStatic8N(Nest00,4,4,4) { Species = 534, Ability = -1 }, // Conkeldurr
            new EncounterStatic8N(Nest01,0,1,1) { Species = 577, Ability =  4 }, // Solosis
            new EncounterStatic8N(Nest01,2,3,3) { Species = 678, Ability =  4, Gender = 1, Form = 1 }, // Meowstic-1
            new EncounterStatic8N(Nest01,2,3,3) { Species = 578, Ability =  4 }, // Duosion
            new EncounterStatic8N(Nest01,3,4,4) { Species = 579, Ability = -1 }, // Reuniclus
            new EncounterStatic8N(Nest01,4,4,4) { Species = 337, Ability = -1 }, // Lunatone
            new EncounterStatic8N(Nest02,0,0,1) { Species = 688, Ability =  4 }, // Binacle
            new EncounterStatic8N(Nest02,0,1,1) { Species = 524, Ability =  4 }, // Roggenrola
            new EncounterStatic8N(Nest02,3,4,4) { Species = 526, Ability = -1 }, // Gigalith
            new EncounterStatic8N(Nest02,4,4,4) { Species = 558, Ability = -1 }, // Crustle
            new EncounterStatic8N(Nest06,0,1,2) { Species = 223, Ability =  4 }, // Remoraid
            new EncounterStatic8N(Nest06,0,1,2) { Species = 170, Ability =  4 }, // Chinchou
            new EncounterStatic8N(Nest06,1,2,2) { Species = 550, Ability =  4, Form = 1 }, // Basculin-1
            new EncounterStatic8N(Nest07,1,2,2) { Species = 550, Ability =  4, Form = 1 }, // Basculin-1
            new EncounterStatic8N(Nest08,1,1,1) { Species = 090, Ability =  4 }, // Shellder
            new EncounterStatic8N(Nest09,1,1,2) { Species = 759, Ability =  4 }, // Stufful
            new EncounterStatic8N(Nest09,1,2,2) { Species = 538, Ability =  4 }, // Throh
            new EncounterStatic8N(Nest09,3,4,4) { Species = 760, Ability = -1 }, // Bewear
            new EncounterStatic8N(Nest11,4,4,4) { Species = 208, Ability = -1 }, // Steelix
            new EncounterStatic8N(Nest12,0,1,2) { Species = 177, Ability =  4 }, // Natu
            new EncounterStatic8N(Nest12,0,1,2) { Species = 856, Ability =  4 }, // Hatenna
            new EncounterStatic8N(Nest12,1,1,2) { Species = 077, Ability =  4, Form = 1 }, // Ponyta-1
            new EncounterStatic8N(Nest12,1,3,3) { Species = 857, Ability =  4 }, // Hattrem
            new EncounterStatic8N(Nest12,2,4,4) { Species = 876, Ability =  4, Gender = 1, Form = 1 }, // Indeedee-1
            new EncounterStatic8N(Nest12,3,4,4) { Species = 765, Ability = -1 }, // Oranguru
            new EncounterStatic8N(Nest12,4,4,4) { Species = 078, Ability = -1, Form = 1 }, // Rapidash-1
            new EncounterStatic8N(Nest13,2,4,4) { Species = 876, Ability =  4, Gender = 1, Form = 1 }, // Indeedee-1
            new EncounterStatic8N(Nest14,0,0,1) { Species = 557, Ability =  4 }, // Dwebble
            new EncounterStatic8N(Nest14,0,1,1) { Species = 524, Ability =  4 }, // Roggenrola
            new EncounterStatic8N(Nest14,2,4,4) { Species = 839, Ability =  4 }, // Coalossal
            new EncounterStatic8N(Nest14,3,4,4) { Species = 526, Ability = -1 }, // Gigalith
            new EncounterStatic8N(Nest14,4,4,4) { Species = 095, Ability = -1 }, // Onix
            new EncounterStatic8N(Nest16,0,1,1) { Species = 328, Ability =  4 }, // Trapinch
            new EncounterStatic8N(Nest16,1,1,1) { Species = 220, Ability =  4 }, // Swinub
            new EncounterStatic8N(Nest16,2,3,3) { Species = 618, Ability =  4, Form = 1 }, // Stunfisk-1
            new EncounterStatic8N(Nest16,2,4,4) { Species = 329, Ability =  4 }, // Vibrava
            new EncounterStatic8N(Nest16,3,4,4) { Species = 330, Ability = -1 }, // Flygon
            new EncounterStatic8N(Nest16,4,4,4) { Species = 450, Ability = -1 }, // Hippowdon
            new EncounterStatic8N(Nest17,0,0,1) { Species = 058, Ability =  4 }, // Growlithe
            new EncounterStatic8N(Nest17,1,1,1) { Species = 631, Ability =  4 }, // Heatmor
            new EncounterStatic8N(Nest17,1,2,2) { Species = 608, Ability =  4 }, // Lampent
            new EncounterStatic8N(Nest17,2,3,3) { Species = 758, Ability =  4, Gender = 1 }, // Salazzle
            new EncounterStatic8N(Nest17,2,4,4) { Species = 059, Ability =  4 }, // Arcanine
            new EncounterStatic8N(Nest17,3,4,4) { Species = 631, Ability = -1 }, // Heatmor
            new EncounterStatic8N(Nest17,4,4,4) { Species = 851, Ability = -1 }, // Centiskorch
            new EncounterStatic8N(Nest17,4,4,4) { Species = 059, Ability = -1 }, // Arcanine
            new EncounterStatic8N(Nest18,0,0,1) { Species = 058, Ability =  4 }, // Growlithe
            new EncounterStatic8N(Nest18,0,1,1) { Species = 058, Ability =  4 }, // Growlithe
            new EncounterStatic8N(Nest18,1,2,2) { Species = 608, Ability =  4 }, // Lampent
            new EncounterStatic8N(Nest18,2,3,3) { Species = 758, Ability =  4, Gender = 1 }, // Salazzle
            new EncounterStatic8N(Nest18,2,4,4) { Species = 059, Ability =  4 }, // Arcanine
            new EncounterStatic8N(Nest18,2,4,4) { Species = 758, Ability =  4, Gender = 1 }, // Salazzle
            new EncounterStatic8N(Nest18,3,4,4) { Species = 324, Ability = -1 }, // Torkoal
            new EncounterStatic8N(Nest18,4,4,4) { Species = 059, Ability = -1 }, // Arcanine
            new EncounterStatic8N(Nest19,0,0,1) { Species = 058, Ability =  4 }, // Growlithe
            new EncounterStatic8N(Nest19,1,1,1) { Species = 324, Ability =  4 }, // Torkoal
            new EncounterStatic8N(Nest19,1,2,2) { Species = 838, Ability =  4 }, // Carkol
            new EncounterStatic8N(Nest19,2,3,3) { Species = 758, Ability =  4, Gender = 1 }, // Salazzle
            new EncounterStatic8N(Nest19,2,4,4) { Species = 059, Ability =  4 }, // Arcanine
            new EncounterStatic8N(Nest19,4,4,4) { Species = 059, Ability = -1 }, // Arcanine
            new EncounterStatic8N(Nest22,0,1,1) { Species = 225, Ability =  4 }, // Delibird
            new EncounterStatic8N(Nest22,4,4,4) { Species = 875, Ability = -1 }, // Eiscue
            new EncounterStatic8N(Nest25,0,0,1) { Species = 270, Ability =  4 }, // Lotad
            new EncounterStatic8N(Nest25,1,1,2) { Species = 271, Ability =  4 }, // Lombre
            new EncounterStatic8N(Nest25,2,4,4) { Species = 272, Ability =  4 }, // Ludicolo
            new EncounterStatic8N(Nest26,4,4,4) { Species = 842, Ability = -1 }, // Appletun
            new EncounterStatic8N(Nest28,0,1,1) { Species = 043, Ability =  4 }, // Oddish
            new EncounterStatic8N(Nest28,1,1,1) { Species = 747, Ability =  4 }, // Mareanie
            new EncounterStatic8N(Nest28,3,4,4) { Species = 435, Ability = -1 }, // Skuntank
            new EncounterStatic8N(Nest28,4,4,4) { Species = 748, Ability = -1 }, // Toxapex
            new EncounterStatic8N(Nest30,0,1,1) { Species = 629, Ability =  4 }, // Vullaby
            new EncounterStatic8N(Nest30,4,4,4) { Species = 630, Ability = -1 }, // Mandibuzz
            new EncounterStatic8N(Nest32,0,1,1) { Species = 682, Ability =  4 }, // Spritzee
            new EncounterStatic8N(Nest32,4,4,4) { Species = 683, Ability = -1 }, // Aromatisse
            new EncounterStatic8N(Nest33,4,4,4) { Species = 078, Ability = -1, Form = 1 }, // Rapidash-1
            new EncounterStatic8N(Nest34,4,4,4) { Species = 302, Ability = -1 }, // Sableye
            new EncounterStatic8N(Nest35,1,1,2) { Species = 629, Ability =  4 }, // Vullaby
            new EncounterStatic8N(Nest35,3,4,4) { Species = 630, Ability = -1 }, // Mandibuzz
            new EncounterStatic8N(Nest35,4,4,4) { Species = 248, Ability = -1 }, // Tyranitar
            new EncounterStatic8N(Nest36,0,0,1) { Species = 610, Ability =  4 }, // Axew
            new EncounterStatic8N(Nest36,0,1,1) { Species = 328, Ability =  4 }, // Trapinch
            new EncounterStatic8N(Nest36,1,1,2) { Species = 704, Ability =  4 }, // Goomy
            new EncounterStatic8N(Nest36,2,3,3) { Species = 611, Ability =  4 }, // Fraxure
            new EncounterStatic8N(Nest36,2,4,4) { Species = 705, Ability =  4 }, // Sliggoo
            new EncounterStatic8N(Nest36,2,4,4) { Species = 330, Ability =  4 }, // Flygon
            new EncounterStatic8N(Nest36,3,4,4) { Species = 612, Ability = -1 }, // Haxorus
            new EncounterStatic8N(Nest36,4,4,4) { Species = 780, Ability = -1 }, // Drampa
            new EncounterStatic8N(Nest36,4,4,4) { Species = 706, Ability = -1 }, // Goodra
            new EncounterStatic8N(Nest37,0,1,1) { Species = 704, Ability =  4 }, // Goomy
            new EncounterStatic8N(Nest37,2,4,4) { Species = 705, Ability =  4 }, // Sliggoo
            new EncounterStatic8N(Nest37,3,4,4) { Species = 706, Ability = -1 }, // Goodra
            new EncounterStatic8N(Nest37,4,4,4) { Species = 842, Ability = -1 }, // Appletun
            new EncounterStatic8N(Nest39,1,1,2) { Species = 876, Ability =  4, Gender = 1, Form = 1 }, // Indeedee-1
            new EncounterStatic8N(Nest39,4,4,4) { Species = 765, Ability = -1 }, // Oranguru
            new EncounterStatic8N(Nest40,0,1,1) { Species = 536, Ability =  4 }, // Palpitoad
            new EncounterStatic8N(Nest40,1,1,2) { Species = 747, Ability =  4 }, // Mareanie
            new EncounterStatic8N(Nest40,2,3,3) { Species = 748, Ability =  4 }, // Toxapex
            new EncounterStatic8N(Nest40,2,4,4) { Species = 091, Ability =  4 }, // Cloyster
            new EncounterStatic8N(Nest40,3,4,4) { Species = 746, Ability = -1 }, // Wishiwashi
            new EncounterStatic8N(Nest40,4,4,4) { Species = 537, Ability = -1 }, // Seismitoad
            new EncounterStatic8N(Nest42,1,3,3) { Species = 222, Ability =  4, Form = 1 }, // Corsola-1
            new EncounterStatic8N(Nest42,4,4,4) { Species = 302, Ability =  4 }, // Sableye
            new EncounterStatic8N(Nest42,3,4,4) { Species = 867, Ability = -1 }, // Runerigus
            new EncounterStatic8N(Nest42,3,4,4) { Species = 864, Ability = -1 }, // Cursola
            new EncounterStatic8N(Nest43,1,2,2) { Species = 550, Ability =  4, Form = 1 }, // Basculin-1
            new EncounterStatic8N(Nest44,1,2,2) { Species = 600, Ability =  4 }, // Klang
            new EncounterStatic8N(Nest44,2,3,3) { Species = 632, Ability =  4 }, // Durant
            new EncounterStatic8N(Nest45,0,1,1) { Species = 616, Ability =  4 }, // Shelmet
            new EncounterStatic8N(Nest45,1,2,2) { Species = 588, Ability =  4 }, // Karrablast
            new EncounterStatic8N(Nest45,4,4,4) { Species = 617, Ability = -1 }, // Accelgor
            new EncounterStatic8N(Nest46,1,3,3) { Species = 222, Ability =  4, Form = 1 }, // Corsola-1
            new EncounterStatic8N(Nest46,2,4,4) { Species = 302, Ability =  4 }, // Sableye
            new EncounterStatic8N(Nest46,3,4,4) { Species = 864, Ability = -1 }, // Cursola
            new EncounterStatic8N(Nest47,0,1,1) { Species = 453, Ability =  4 }, // Croagunk
            new EncounterStatic8N(Nest47,2,4,4) { Species = 454, Ability =  4 }, // Toxicroak
            new EncounterStatic8N(Nest47,3,4,4) { Species = 701, Ability = -1 }, // Hawlucha
            new EncounterStatic8N(Nest50,0,1,1) { Species = 524, Ability =  4 }, // Roggenrola
            new EncounterStatic8N(Nest50,1,2,2) { Species = 246, Ability =  4 }, // Larvitar
            new EncounterStatic8N(Nest50,2,3,3) { Species = 247, Ability =  4 }, // Pupitar
            new EncounterStatic8N(Nest50,4,4,4) { Species = 248, Ability = -1 }, // Tyranitar
            new EncounterStatic8N(Nest52,0,0,1) { Species = 058, Ability =  4 }, // Growlithe
            new EncounterStatic8N(Nest52,1,2,2) { Species = 631, Ability =  4 }, // Heatmor
            new EncounterStatic8N(Nest52,3,4,4) { Species = 059, Ability = -1 }, // Arcanine
            new EncounterStatic8N(Nest53,0,0,1) { Species = 058, Ability =  4 }, // Growlithe
            new EncounterStatic8N(Nest53,1,2,2) { Species = 631, Ability =  4 }, // Heatmor
            new EncounterStatic8N(Nest53,2,3,3) { Species = 608, Ability =  4 }, // Lampent
            new EncounterStatic8N(Nest53,3,4,4) { Species = 059, Ability = -1 }, // Arcanine
            new EncounterStatic8N(Nest53,4,4,4) { Species = 758, Ability = -1, Gender = 1 }, // Salazzle
            new EncounterStatic8N(Nest54,0,0,1) { Species = 613, Ability =  4 }, // Cubchoo
            new EncounterStatic8N(Nest54,4,4,4) { Species = 875, Ability = -1 }, // Eiscue
            new EncounterStatic8N(Nest57,0,0,1) { Species = 270, Ability =  4 }, // Lotad
            new EncounterStatic8N(Nest57,1,2,2) { Species = 271, Ability =  4 }, // Lombre
            new EncounterStatic8N(Nest57,2,4,4) { Species = 272, Ability =  4 }, // Ludicolo
            new EncounterStatic8N(Nest57,4,4,4) { Species = 842, Ability = -1 }, // Appletun
            new EncounterStatic8N(Nest58,0,0,1) { Species = 270, Ability =  4 }, // Lotad
            new EncounterStatic8N(Nest58,1,2,2) { Species = 271, Ability =  4 }, // Lombre
            new EncounterStatic8N(Nest58,4,4,4) { Species = 272, Ability = -1 }, // Ludicolo
            new EncounterStatic8N(Nest59,1,2,2) { Species = 757, Ability =  4 }, // Salandit
            new EncounterStatic8N(Nest59,4,4,4) { Species = 758, Ability = -1, Gender = 1 }, // Salazzle
            new EncounterStatic8N(Nest61,2,4,4) { Species = 078, Ability =  4, Form = 1 }, // Rapidash-1
            new EncounterStatic8N(Nest62,0,1,1) { Species = 629, Ability =  4 }, // Vullaby
            new EncounterStatic8N(Nest62,3,4,4) { Species = 630, Ability = -1 }, // Mandibuzz
            new EncounterStatic8N(Nest62,4,4,4) { Species = 248, Ability = -1 }, // Tyranitar
            new EncounterStatic8N(Nest63,0,0,1) { Species = 610, Ability =  4 }, // Axew
            new EncounterStatic8N(Nest63,0,1,1) { Species = 328, Ability =  4 }, // Trapinch
            new EncounterStatic8N(Nest63,0,1,1) { Species = 704, Ability =  4 }, // Goomy
            new EncounterStatic8N(Nest63,1,2,2) { Species = 329, Ability =  4 }, // Vibrava
            new EncounterStatic8N(Nest63,2,4,4) { Species = 705, Ability =  4 }, // Sliggoo
            new EncounterStatic8N(Nest63,2,4,4) { Species = 780, Ability =  4 }, // Drampa
            new EncounterStatic8N(Nest63,3,4,4) { Species = 706, Ability = -1 }, // Goodra
            new EncounterStatic8N(Nest63,4,4,4) { Species = 330, Ability = -1 }, // Flygon
            new EncounterStatic8N(Nest64,3,4,4) { Species = 765, Ability = -1 }, // Oranguru
            new EncounterStatic8N(Nest64,3,4,4) { Species = 876, Ability = -1, Gender = 1, Form = 1 }, // Indeedee-1
            new EncounterStatic8N(Nest66,1,2,2) { Species = 550, Ability =  4, Form = 1 }, // Basculin-1
            new EncounterStatic8N(Nest70,0,0,1) { Species = 058, Ability =  4 }, // Growlithe
            new EncounterStatic8N(Nest70,0,1,1) { Species = 058, Ability =  4 }, // Growlithe
            new EncounterStatic8N(Nest70,1,2,2) { Species = 631, Ability =  4 }, // Heatmor
            new EncounterStatic8N(Nest70,2,3,3) { Species = 608, Ability =  4 }, // Lampent
            new EncounterStatic8N(Nest73,0,0,1) { Species = 682, Ability =  4 }, // Spritzee
            new EncounterStatic8N(Nest73,2,4,4) { Species = 683, Ability =  4 }, // Aromatisse
            new EncounterStatic8N(Nest75,2,4,4) { Species = 550, Ability =  4, Form = 1 }, // Basculin-1
            new EncounterStatic8N(Nest76,0,0,1) { Species = 058, Ability =  4 }, // Growlithe
            new EncounterStatic8N(Nest76,1,2,2) { Species = 631, Ability =  4 }, // Heatmor
            new EncounterStatic8N(Nest76,3,4,4) { Species = 059, Ability = -1 }, // Arcanine
            new EncounterStatic8N(Nest77,1,2,2) { Species = 550, Ability =  4, Form = 1 }, // Basculin-1
            new EncounterStatic8N(Nest78,0,0,1) { Species = 270, Ability =  4 }, // Lotad
            new EncounterStatic8N(Nest78,1,2,2) { Species = 271, Ability =  4 }, // Lombre
            new EncounterStatic8N(Nest78,2,4,4) { Species = 272, Ability =  4 }, // Ludicolo
            new EncounterStatic8N(Nest78,4,4,4) { Species = 842, Ability = -1, CanGigantamax = true }, // Appletun
            new EncounterStatic8N(Nest79,0,0,1) { Species = 058, Ability =  4 }, // Growlithe
            new EncounterStatic8N(Nest79,3,4,4) { Species = 059, Ability = -1 }, // Arcanine
            new EncounterStatic8N(Nest80,0,0,1) { Species = 679, Ability =  4 }, // Honedge
            new EncounterStatic8N(Nest80,0,0,1) { Species = 562, Ability =  4, Form = 1 }, // Yamask-1
            new EncounterStatic8N(Nest80,0,1,1) { Species = 854, Ability =  4 }, // Sinistea
            new EncounterStatic8N(Nest80,0,1,1) { Species = 092, Ability =  4 }, // Gastly
            new EncounterStatic8N(Nest80,1,2,2) { Species = 680, Ability =  4 }, // Doublade
            new EncounterStatic8N(Nest80,1,3,3) { Species = 222, Ability =  4, Form = 1 }, // Corsola-1
            new EncounterStatic8N(Nest80,2,3,3) { Species = 093, Ability =  4 }, // Haunter
            new EncounterStatic8N(Nest80,2,4,4) { Species = 302, Ability =  4 }, // Sableye
            new EncounterStatic8N(Nest80,3,4,4) { Species = 855, Ability = -1 }, // Polteageist
            new EncounterStatic8N(Nest80,3,4,4) { Species = 864, Ability = -1 }, // Cursola
            new EncounterStatic8N(Nest80,4,4,4) { Species = 867, Ability = -1 }, // Runerigus
            new EncounterStatic8N(Nest80,4,4,4) { Species = 094, Ability = -1, CanGigantamax = true }, // Gengar
            new EncounterStatic8N(Nest81,0,0,1) { Species = 077, Ability =  4, Form = 1 }, // Ponyta-1
            new EncounterStatic8N(Nest81,2,4,4) { Species = 078, Ability =  4, Form = 1 }, // Rapidash-1
            new EncounterStatic8N(Nest82,0,0,1) { Species = 582, Ability =  4 }, // Vanillite
            new EncounterStatic8N(Nest82,0,0,1) { Species = 613, Ability =  4 }, // Cubchoo
            new EncounterStatic8N(Nest82,0,1,1) { Species = 122, Ability =  4, Form = 1 }, // Mr. Mime-1
            new EncounterStatic8N(Nest82,0,1,1) { Species = 712, Ability =  4 }, // Bergmite
            new EncounterStatic8N(Nest82,1,2,2) { Species = 361, Ability =  4 }, // Snorunt
            new EncounterStatic8N(Nest82,1,2,2) { Species = 225, Ability =  4 }, // Delibird
            new EncounterStatic8N(Nest82,2,3,3) { Species = 713, Ability =  4 }, // Avalugg
            new EncounterStatic8N(Nest82,2,4,4) { Species = 362, Ability =  4 }, // Glalie
            new EncounterStatic8N(Nest82,3,4,4) { Species = 584, Ability = -1 }, // Vanilluxe
            new EncounterStatic8N(Nest82,3,4,4) { Species = 866, Ability = -1 }, // Mr. Rime
            new EncounterStatic8N(Nest82,4,4,4) { Species = 875, Ability = -1 }, // Eiscue
            new EncounterStatic8N(Nest82,4,4,4) { Species = 131, Ability = -1, CanGigantamax = true }, // Lapras
            new EncounterStatic8N(Nest83,1,2,2) { Species = 600, Ability =  4 }, // Klang
            new EncounterStatic8N(Nest83,2,3,3) { Species = 632, Ability =  4 }, // Durant
            new EncounterStatic8N(Nest85,1,2,2) { Species = 757, Ability =  4 }, // Salandit
            new EncounterStatic8N(Nest85,4,4,4) { Species = 758, Ability = -1, Gender = 1 }, // Salazzle
            new EncounterStatic8N(Nest86,0,0,1) { Species = 682, Ability =  4 }, // Spritzee
            new EncounterStatic8N(Nest86,2,3,3) { Species = 683, Ability =  4 }, // Aromatisse
            new EncounterStatic8N(Nest87,0,1,1) { Species = 629, Ability =  4 }, // Vullaby
            new EncounterStatic8N(Nest87,3,4,4) { Species = 630, Ability = -1 }, // Mandibuzz
            new EncounterStatic8N(Nest87,4,4,4) { Species = 248, Ability = -1 }, // Tyranitar
            new EncounterStatic8N(Nest89,0,1,1) { Species = 616, Ability =  4 }, // Shelmet
            new EncounterStatic8N(Nest89,1,2,2) { Species = 588, Ability =  4 }, // Karrablast
            new EncounterStatic8N(Nest89,4,4,4) { Species = 617, Ability = -1 }, // Accelgor
            new EncounterStatic8N(Nest90,1,2,2) { Species = 550, Ability =  4, Form = 1 }, // Basculin-1
            new EncounterStatic8N(Nest91,0,1,1) { Species = 616, Ability =  4 }, // Shelmet
            new EncounterStatic8N(Nest91,1,2,2) { Species = 588, Ability =  4 }, // Karrablast
            new EncounterStatic8N(Nest91,4,4,4) { Species = 617, Ability = -1 }, // Accelgor
        };

        private const string tradeSWSH = "tradeswsh";
        private static readonly string[][] TradeSWSH = Util.GetLanguageStrings10(tradeSWSH);
        private static readonly int[] TradeIVs = {15, 15, 15, 15, 15, 15};

        internal static readonly EncounterTrade8[] TradeGift_SWSH =
        {
            new EncounterTrade8(052,18) { Ability = 2, TID7 = 263455, IVs = TradeIVs, DynamaxLevel = 1            , Relearn = new[] {387,000,000,000} }, // Meowth
            new EncounterTrade8(819,10) { Ability = 1, TID7 = 648753, IVs = TradeIVs, DynamaxLevel = 1, Gender = 0                                    }, // Skwovet
            new EncounterTrade8(546,23) { Ability = 1, TID7 = 101154, IVs = TradeIVs, DynamaxLevel = 1, Gender = 0                                    }, // Cottonee
            new EncounterTrade8(175,25) { Ability = 2, TID7 = 109591, IVs = TradeIVs, DynamaxLevel = 1, Gender = 0, Relearn = new[] {791,000,000,000} }, // Togepi
            new EncounterTrade8(856,30) { Ability = 2, TID7 = 101101, IVs = TradeIVs, DynamaxLevel = 1                                                }, // Hatenna
            new EncounterTrade8(859,30) { Ability = 1, TID7 = 256081, IVs = TradeIVs, DynamaxLevel = 1            , Relearn = new[] {252,000,000,000} }, // Impidimp
            new EncounterTrade8(562,35) { Ability = 1, TID7 = 102534, IVs = TradeIVs, DynamaxLevel = 2, Gender = 0, Relearn = new[] {261,000,000,000} }, // Yamask
            new EncounterTrade8(538,37) { Ability = 2, TID7 = 768945, IVs = TradeIVs, DynamaxLevel = 2                                                }, // Throh
            new EncounterTrade8(539,37) { Ability = 1, TID7 = 881426, IVs = TradeIVs, DynamaxLevel = 2                                                }, // Sawk
            new EncounterTrade8(122,40) { Ability = 1, TID7 = 891846, IVs = TradeIVs, DynamaxLevel = 1                                                }, // Mr. Mime
            new EncounterTrade8(884,50) { Ability = 2, TID7 = 101141, IVs = TradeIVs, DynamaxLevel = 3            , Relearn = new[] {400,000,000,000} }, // Duraludon

        };
        #endregion

        #region Dynamax Crystal Distributions

        internal static readonly EncounterStatic8ND[] Crystal_SWSH =
        {
            new EncounterStatic8ND { Species = 782, Level = 16, Ability = -1, Location = 126, IVs = new []{31,31,31,-1,-1,-1}, DynamaxLevel = 2, Moves = new[] {033,029,525,043}, }, // ★And458 Jangmo-o
            new EncounterStatic8ND { Species = 246, Level = 16, Ability = -1, Location = 126, IVs = new []{31,31,31,-1,-1,-1}, DynamaxLevel = 2, Moves = new[] {033,157,371,044}, }, // ★And15 Larvitar
        };

        #endregion
    }

    public class EncounterGift8 : EncounterStatic, IGigantamax, IDynamaxLevel
    {
        public bool CanGigantamax { get; set; }
        public byte DynamaxLevel { get; set; }
    }

    public class EncounterTrade8 : EncounterTrade, IDynamaxLevel, IRelearn
    {
        public byte DynamaxLevel { get; set; }
        public int[] Relearn { get; set; } = Array.Empty<int>();

        public EncounterTrade8(int species, int level)
        {
            Species = species;
            Level = level;
        }

        public override bool IsMatch(PKM pkm, int lvl)
        {
            if (pkm is IDynamaxLevel d && d.DynamaxLevel < DynamaxLevel)
                return false;
            return base.IsMatch(pkm, lvl);
        }

        protected override void ApplyDetails(ITrainerInfo SAV, EncounterCriteria criteria, PKM pk)
        {
            if (pk is IDynamaxLevel d)
                d.DynamaxLevel = DynamaxLevel;
            pk.SetRelearnMoves(Relearn);
            ((PK8)pk).HT_Language = SAV.Language;
            base.ApplyDetails(SAV, criteria, pk);
        }
    }

    public sealed class EncounterStatic8 : EncounterStatic
    {
        protected override bool IsMatchLevel(PKM pkm, int lvl)
        {
            if (lvl == Level)
                return true;
            if (EncounterArea8.IsWildArea8(Location))
                return lvl == 60;
            return false;
        }
    }

    public sealed class EncounterStatic8N : EncounterStatic, IGigantamax, IDynamaxLevel
    {
        public bool CanGigantamax { get; set; }
        public byte DynamaxLevel { get; set; }

        private readonly int MinRank;
        private readonly int MaxRank;

        public EncounterStatic8N(int loc, int minRank, int maxRank, byte val)
        {
            Location = loc;
            MinRank = minRank;
            MaxRank = maxRank;
            DynamaxLevel = val;
        }

        private readonly int[] LevelCaps =
        {
            15, 20, // 0
            25, 30, // 1
            35, 40, // 2
            45, 50, // 3
            55, 60, // 4
        };

        protected override int GetMinimalLevel() => LevelCaps[MinRank * 2];

        protected override bool IsMatchLevel(PKM pkm, int lvl)
        {
            var metLevel = pkm.Met_Level - 15;
            var rank = metLevel / 10;
            if ((uint)rank > 4u)
                return false;
            if (rank < MinRank || MaxRank < rank)
                return false;

            return metLevel % 10 <= 5;
        }

        public override bool IsMatch(PKM pkm, int lvl)
        {
            if (Ability != -1 && pkm.AbilityNumber != 4)
                return false;
            if (pkm is IDynamaxLevel d && d.DynamaxLevel < DynamaxLevel)
                return false;

            if (pkm.GetFlawlessIVCount() < DynamaxLevel)
                return false;

            return base.IsMatch(pkm, lvl);
        }
    }

    public sealed class EncounterStatic8ND : EncounterStatic, IGigantamax, IDynamaxLevel
    {
        public bool CanGigantamax { get; set; }
        public byte DynamaxLevel { get; set; }

        public override bool IsMatch(PKM pkm, int lvl)
        {
            if (Ability != -1 && pkm.AbilityNumber != 4)
                return false;
            if (pkm is IDynamaxLevel d && d.DynamaxLevel < DynamaxLevel)
                return false;

            return base.IsMatch(pkm, lvl);
        }
    }
}
