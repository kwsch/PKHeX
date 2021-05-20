using static PKHeX.Core.EncounterUtil;
using static PKHeX.Core.Shiny;
using static PKHeX.Core.GameVersion;
using static PKHeX.Core.AreaWeather8;

using static PKHeX.Core.Encounters8Nest;

namespace PKHeX.Core
{
    /// <summary>
    /// Generation 8 Encounters
    /// </summary>
    internal static class Encounters8
    {
        private static readonly EncounterArea8[] SlotsSW_Symbol = EncounterArea8.GetAreas(Get("sw_symbol", "sw"), SW, true);
        private static readonly EncounterArea8[] SlotsSH_Symbol = EncounterArea8.GetAreas(Get("sh_symbol", "sh"), SH, true);
        private static readonly EncounterArea8[] SlotsSW_Hidden = EncounterArea8.GetAreas(Get("sw_hidden", "sw"), SW);
        private static readonly EncounterArea8[] SlotsSH_Hidden = EncounterArea8.GetAreas(Get("sh_hidden", "sh"), SH);
        private static byte[][] Get(string resource, string ident) => BinLinker.Unpack(Util.GetBinaryResource($"encounter_{resource}.pkl"), ident);

        internal static readonly EncounterArea8[] SlotsSW = ArrayUtil.ConcatAll(SlotsSW_Symbol, SlotsSW_Hidden);
        internal static readonly EncounterArea8[] SlotsSH = ArrayUtil.ConcatAll(SlotsSH_Symbol, SlotsSH_Hidden);

        static Encounters8()
        {
            foreach (var t in TradeGift_R1)
                t.TrainerNames = TradeOT_R1;

            MarkEncounterTradeStrings(TradeGift_SWSH, TradeSWSH);
        }

        private static readonly EncounterStatic8[] Encounter_SWSH =
        {
            // gifts
            new(SWSH) { Gift = true, Species = 810, Shiny = Never, Level = 05, Location = 006 }, // Grookey
            new(SWSH) { Gift = true, Species = 813, Shiny = Never, Level = 05, Location = 006 }, // Scorbunny
            new(SWSH) { Gift = true, Species = 816, Shiny = Never, Level = 05, Location = 006 }, // Sobble

            new(SWSH) { Gift = true, Species = 772, Shiny = Never, Level = 50, Location = 158, FlawlessIVCount = 3, }, // Type: Null
            new(SWSH) { Gift = true, Species = 848, Shiny = Never, Level = 01, Location = 040, IVs = new[]{-1,31,-1,-1,31,-1}, Ball = 11 }, // Toxel, Attack flawless

            new(SWSH) { Gift = true, Species = 880, FlawlessIVCount = 3, Level = 10, Location = 068 }, // Dracozolt @ Route 6
            new(SWSH) { Gift = true, Species = 881, FlawlessIVCount = 3, Level = 10, Location = 068 }, // Arctozolt @ Route 6
            new(SWSH) { Gift = true, Species = 882, FlawlessIVCount = 3, Level = 10, Location = 068 }, // Dracovish @ Route 6
            new(SWSH) { Gift = true, Species = 883, FlawlessIVCount = 3, Level = 10, Location = 068 }, // Arctovish @ Route 6

            new(SWSH) { Gift = true, Species = 004, Shiny = Never, Level = 05, Location = 006, FlawlessIVCount = 3, CanGigantamax = true, Ability = 1 }, // Charmander
            new(SWSH) { Gift = true, Species = 025, Shiny = Never, Level = 10, Location = 156, FlawlessIVCount = 6, CanGigantamax = true }, // Pikachu
            new(SWSH) { Gift = true, Species = 133, Shiny = Never, Level = 10, Location = 156, FlawlessIVCount = 6, CanGigantamax = true }, // Eevee

            // DLC gifts
            new(SWSH) { Gift = true, Species = 001, Level = 05, Location = 196, Shiny = Never, Ability = 1, FlawlessIVCount = 3, CanGigantamax = true }, // Bulbasaur
            new(SWSH) { Gift = true, Species = 007, Level = 05, Location = 196, Shiny = Never, Ability = 1, FlawlessIVCount = 3, CanGigantamax = true }, // Squirtle
            new(SWSH) { Gift = true, Species = 137, Level = 25, Location = 196, Shiny = Never, Ability = 4, FlawlessIVCount = 3 }, // Porygon
            new(SWSH) { Gift = true, Species = 891, Level = 10, Location = 196, Shiny = Never, FlawlessIVCount = 3 }, // Kubfu

            new(SWSH) { Gift = true, Species = 079, Level = 10, Location = 164, Shiny = Never, Ability = 4, FlawlessIVCount = 3 }, // Slowpoke
            new(SWSH) { Gift = true, Species = 722, Level = 05, Location = 164, Shiny = Never, Ability = 4, FlawlessIVCount = 3 }, // Rowlet
            new(SWSH) { Gift = true, Species = 725, Level = 05, Location = 164, Shiny = Never, Ability = 4, FlawlessIVCount = 3 }, // Litten
            new(SWSH) { Gift = true, Species = 728, Level = 05, Location = 164, Shiny = Never, Ability = 4, FlawlessIVCount = 3 }, // Popplio
            new(SWSH) { Gift = true, Species = 026, Level = 30, Location = 164, Shiny = Never, Ability = 1, FlawlessIVCount = 3, Form = 01 }, // Raichu-1
            new(SWSH) { Gift = true, Species = 027, Level = 05, Location = 164, Shiny = Never, Ability = 4, FlawlessIVCount = 3, Form = 01 }, // Sandshrew-1
            new(SWSH) { Gift = true, Species = 037, Level = 05, Location = 164, Shiny = Never, Ability = 4, FlawlessIVCount = 3, Form = 01 }, // Vulpix-1
            new(SWSH) { Gift = true, Species = 052, Level = 05, Location = 164, Shiny = Never, Ability = 4, FlawlessIVCount = 3, Form = 01 }, // Meowth-1
            new(SWSH) { Gift = true, Species = 103, Level = 30, Location = 164, Shiny = Never, Ability = 4, FlawlessIVCount = 3, Form = 01 }, // Exeggutor-1
            new(SWSH) { Gift = true, Species = 105, Level = 30, Location = 164, Shiny = Never, Ability = 4, FlawlessIVCount = 3, Form = 01 }, // Marowak-1
            new(SWSH) { Gift = true, Species = 050, Level = 20, Location = 164, Shiny = Never, Ability = 4, Gender = 0, Nature = Nature.Jolly, FlawlessIVCount = 6, Form = 01 }, // Diglett-1

            new(SWSH) { Gift = true, Species = 789, Level = 05, Location = 206, FlawlessIVCount = 3, Shiny = Never, Ability = 1 }, // Cosmog
            new(SWSH) { Gift = true, Species = 803, Level = 20, Location = 244, FlawlessIVCount = 3, Shiny = Never, Ability = 1, Ball = 26 }, // Poipole

            // Technically a gift, but copies ball from Calyrex.
            new(SWSH) { Species = 896, Level = 75, Location = 220, ScriptedNoMarks = true, FlawlessIVCount = 3, Shiny = Never, Ability = 1, Relearn = new[] {556,0,0,0} }, // Glastrier
            new(SWSH) { Species = 897, Level = 75, Location = 220, ScriptedNoMarks = true, FlawlessIVCount = 3, Shiny = Never, Ability = 1, Relearn = new[] {247,0,0,0} }, // Spectrier

            #region Static Part 1
            // encounters
            new(SW  ) { Species = 888, Level = 70, Location = 66, ScriptedNoMarks = true, Moves = new[] {533,014,442,242}, Shiny = Never, Ability = 1, FlawlessIVCount = 3 }, // Zacian
            new(  SH) { Species = 889, Level = 70, Location = 66, ScriptedNoMarks = true, Moves = new[] {163,242,442,334}, Shiny = Never, Ability = 1, FlawlessIVCount = 3 }, // Zamazenta
            new(SWSH) { Species = 890, Level = 60, Location = 66, ScriptedNoMarks = true, Moves = new[] {440,406,053,744}, Shiny = Never, Ability = 1, FlawlessIVCount = 3 }, // Eternatus-1 (reverts to form 0)

            // Motostoke Stadium Static Encounters
            new(SWSH) { Species = 037, Level = 24, Location = 24, ScriptedNoMarks = true }, // Vulpix at Motostoke Stadium
          //new(  SH) { Species = 058, Level = 24, Location = 24, ScriptedNoMarks = true }, // Growlithe at Motostoke Stadium (both versions have Vulpix)
            new(SWSH) { Species = 607, Level = 25, Location = 24, ScriptedNoMarks = true }, // Litwick at Motostoke Stadium
            new(SWSH) { Species = 850, Level = 25, Location = 24, ScriptedNoMarks = true, FlawlessIVCount = 3 }, // Sizzlipede at Motostoke Stadium

            new(SWSH) { Species = 618, Level = 25, Location = 054, Moves = new[] {389,319,279,341}, Form = 01, Ability = 1 }, // Stunfisk in Galar Mine No. 2
            new(SWSH) { Species = 618, Level = 48, Location = 008, Moves = new[] {779,330,340,334}, Form = 01 }, // Stunfisk in the Slumbering Weald
            new(SWSH) { Species = 527, Level = 16, Location = 030, Moves = new[] {000,000,000,000} }, // Woobat in Galar Mine
            new(SWSH) { Species = 838, Level = 18, Location = 030, Moves = new[] {488,397,229,033} }, // Carkol in Galar Mine
            new(SWSH) { Species = 834, Level = 24, Location = 054, Moves = new[] {317,029,055,044} }, // Drednaw in Galar Mine No. 2
            new(SWSH) { Species = 423, Level = 50, Location = 054, Moves = new[] {240,414,330,246}, FlawlessIVCount = 3, Form = 01 }, // Gastrodon in Galar Mine No. 2
            new(SWSH) { Species = 859, Level = 31, Location = 076, ScriptedNoMarks = true, Moves = new[] {259,389,207,372} }, // Impidimp in Glimwood Tangle
            new(SWSH) { Species = 860, Level = 38, Location = 076, ScriptedNoMarks = true, Moves = new[] {793,399,259,389} }, // Morgrem in Glimwood Tangle
            new(SWSH) { Species = 835, Level = 08, Location = 018, Moves = new[] {039,033,609,000} }, // Yamper on Route 2
            new(SWSH) { Species = 834, Level = 50, Location = 018, Moves = new[] {710,746,068,317}, FlawlessIVCount = 3 }, // Drednaw on Route 2
            new(SWSH) { Species = 833, Level = 08, Location = 018, Moves = new[] {044,055,000,000} }, // Chewtle on Route 2
            new(SWSH) { Species = 131, Level = 55, Location = 018, Moves = new[] {056,240,058,034}, FlawlessIVCount = 3 }, // Lapras on Route 2
            new(SWSH) { Species = 862, Level = 50, Location = 018, Moves = new[] {269,068,792,184} }, // Obstagoon on Route 2
            new(SWSH) { Species = 822, Level = 18, Location = 028, Moves = new[] {681,468,031,365}, Shiny = Never }, // Corvisquire on Route 3
            new(SWSH) { Species = 050, Level = 17, Location = 032, Moves = new[] {523,189,310,045} }, // Diglett on Route 4
            new(SWSH) { Species = 830, Level = 22, Location = 040, Moves = new[] {178,496,075,047} }, // Eldegoss on Route 5
            new(SWSH) { Species = 558, Level = 40, Location = 086, Moves = new[] {404,350,446,157} }, // Crustle on Route 8
            new(SWSH) { Species = 870, Level = 40, Location = 086, Moves = new[] {748,660,179,203} }, // Falinks on Route 8
            new(SWSH) { Species = 362, Level = 55, Location = 090, Moves = new[] {573,329,104,182}, FlawlessIVCount = 3, Weather = Snowing }, // Glalie on Route 9
            new(SWSH) { Species = 853, Level = 50, Location = 092, Moves = new[] {753,576,276,179}, Weather = Snowing }, // Grapploct on Route 9 (in Circhester Bay)
          //new(SWSH) { Species = 822, Level = 35, Location =  -1, Moves = new[] {065,184,269,365} }, // Corvisquire
            new(SWSH) { Species = 614, Level = 55, Location = 106, Moves = new[] {276,059,156,329}, Weather = Snowstorm }, // Beartic on Route 10
            new(SWSH) { Species = 460, Level = 55, Location = 106, Moves = new[] {008,059,452,275}, Weather = Snowstorm }, // Abomasnow on Route 10
            new(SWSH) { Species = 342, Level = 50, Location = 034, Moves = new[] {242,014,534,400}, FlawlessIVCount = 3 }, // Crawdaunt in the town of Turffield
            #endregion

            #region Static Part 2
            // Some of these may be crossover cases. For now, just log the locations they can show up in and re-categorize later.
            new(SWSH) { Species = 095, Level = 26, Location = 122, Weather = All }, // Onix in the Rolling Fields
            new(SWSH) { Species = 416, Level = 26, Location = 122 }, // Vespiquen in the Rolling Fields
            new(SWSH) { Species = 675, Level = 32, Location = 122, Weather = Normal | Overcast | Stormy | Intense_Sun }, // Pangoro in the Rolling Fields
            new(SWSH) { Species = 675, Level = 32, Location = 124, Weather = Intense_Sun | Icy | Sandstorm }, // Pangoro in the Dappled Grove
            new(SWSH) { Species = 291, Level = 15, Location = 122, Weather = All }, // Ninjask in the Rolling Fields
            new(SWSH) { Species = 315, Level = 15, Location = 122, Weather = Normal | Overcast | Raining | Intense_Sun | Snowing | Sandstorm | Heavy_Fog }, // Roselia in the Rolling Fields
            new(SWSH) { Species = 045, Level = 36, Location = 124, Weather = Normal | Overcast | Heavy_Fog }, // Vileplume in the Dappled Grove
            new(SWSH) { Species = 760, Level = 34, Location = 124, Weather = All }, // Bewear in the Dappled Grove
            new(SW  ) { Species = 275, Level = 34, Location = 124, Weather = Normal | Overcast | Stormy | Heavy_Fog }, // Shiftry in the Dappled Grove
            new(  SH) { Species = 272, Level = 34, Location = 124, Weather = Normal | Overcast | Stormy | Heavy_Fog }, // Ludicolo in the Dappled Grove
            new(SWSH) { Species = 426, Level = 34, Location = 126, Weather = Normal | Intense_Sun | Snowing }, // Drifblim at Watchtower Ruins
            new EncounterStatic8S(SWSH)  { Species = 623, Level = 40, Locations = new[] {126, 130}, Weather = Normal | Intense_Sun | Sandstorm }, // Golurk at Watchtower Ruins, West Lake Axewell 
            new(SWSH) { Species = 195, Level = 15, Location = 130, Weather = Normal | Overcast | Stormy | Heavy_Fog }, // Quagsire at West Lake Axewell
            new(SWSH) { Species = 099, Level = 28, Location = 130 }, // Kingler at West Lake Axewell
            new(SWSH) { Species = 660, Level = 15, Location = 122, Weather = All }, // Diggersby in the Rolling Fields
            new(SWSH) { Species = 660, Level = 15, Location = 130, Weather = Intense_Sun | Icy | Sandstorm }, // Diggersby at West Lake Axewell
            new EncounterStatic8S(SWSH) { Species = 178, Level = 26, Locations = new[] {128, 138}, Weather = Normal | Overcast | Intense_Sun | Icy | Sandstorm | Heavy_Fog }, // Xatu at East Lake Axewell, North Lake Miloch
            new(SWSH) { Species = 569, Level = 36, Location = 128, Weather = Normal | Overcast | Stormy }, // Garbodor at East Lake Axewell
            new(SWSH) { Species = 510, Level = 28, Location = 138, Weather = All }, // Liepard at North Lake Miloch
            new(SWSH) { Species = 750, Level = 31, Location = 122, Weather = Normal | Overcast | Intense_Sun | Sandstorm | Heavy_Fog }, // Mudsdale in the Rolling Fields
            new(SWSH) { Species = 067, Level = 26, Location = 134, Weather = All }, // Machoke at South Lake Miloch
            new(SWSH) { Species = 435, Level = 34, Location = 134, Weather = Normal | Overcast | Icy | Sandstorm | Heavy_Fog }, // Skuntank at South Lake Miloch
            new(SWSH) { Species = 099, Level = 31, Location = 134, Weather = Normal | Thunderstorm }, // Kingler at South Lake Miloch
            new(SWSH) { Species = 342, Level = 31, Location = 134, Weather = Normal | Thunderstorm }, // Crawdaunt at South Lake Miloch
            new(SWSH) { Species = 208, Level = 50, Location = 136, Weather = All }, // Steelix near the Giant’s Seat
            new(SWSH) { Species = 823, Level = 50, Location = 138, Weather = All }, // Corviknight at North Lake Miloch
            new(SWSH) { Species = 448, Level = 36, Location = 138 }, // Lucario at North Lake Miloch
            new(SWSH) { Species = 112, Level = 46, Location = 136 }, // Rhydon near the Giant’s Seat
            new(SWSH) { Species = 625, Level = 52, Location = 136, Weather = All }, // Bisharp near the Giant’s Seat
            new(SWSH) { Species = 625, Level = 52, Location = 140, Weather = Snowstorm }, // Bisharp at the Motostoke Riverbank
            new(SWSH) { Species = 738, Level = 46, Location = 136, Weather = Normal | Overcast | Intense_Sun | Sandstorm }, // Vikavolt near the Giant’s Seat
            new EncounterStatic8S(SWSH) { Species = 091, Level = 46, Locations = new[] {128, 130}, Weather = Normal | Heavy_Fog }, // Cloyster at East/West Lake Axewell
            new EncounterStatic8S(SWSH) { Species = 131, Level = 56, Locations = new[] {130, 134, 138, 154}, Weather = Normal | Stormy | Icy | Heavy_Fog }, // Lapras at North/East/South Lake Miloch/Axewell, the Lake of Outrage
            new(SWSH) { Species = 119, Level = 46, Location = 128, Weather = Normal | Overcast | Intense_Sun | Sandstorm }, // Seaking at East Lake Axewell
            new(SWSH) { Species = 119, Level = 46, Location = 154, Weather = Overcast | Intense_Sun | Sandstorm }, // Seaking at Lake of Outrage
            new EncounterStatic8S(SWSH) { Species = 119, Level = 46, Locations = new[] {130, 142}, Weather = Normal | Overcast | Sandstorm }, // Seaking at West Lake Axewell, Bridge Field
            new(SWSH) { Species = 119, Level = 46, Location = 134, Weather = Normal | Overcast | Stormy | Sandstorm }, // Seaking at South Lake Miloch
            new(SWSH) { Species = 119, Level = 46, Location = 138, Weather = Stormy | Sandstorm }, // Seaking at North Lake Miloch
            new EncounterStatic8S(SWSH) { Species = 130, Level = 56, Locations = new[] {128, 130, 142, 146}, Weather = All }, // Gyarados in East/West Lake Axewell, in Bridge Field, Dusty Bowl
            new EncounterStatic8S(SWSH) { Species = 130, Level = 56, Locations = new[] {138, 154}, Weather = Overcast | Intense_Sun | Sandstorm }, // Gyarados in North Lake Miloch, Lake of Outrage
            new(SWSH) { Species = 279, Level = 46, Location = 128, Weather = Normal | Overcast | Stormy | Intense_Sun }, // Pelipper at East Lake Axewell
            new(SWSH) { Species = 279, Level = 46, Location = 142, Weather = Intense_Sun }, // Pelipper in Bridge Field
            new(SWSH) { Species = 853, Level = 56, Location = 130, Weather = Normal | Overcast | Intense_Sun | Icy | Sandstorm | Heavy_Fog }, // Grapploct at West Lake Axewell
            new EncounterStatic8S(SWSH) { Species = 593, Level = 46, Locations = new[] {128, 138, 154}, Weather = Normal | Overcast | Stormy | Heavy_Fog }, // Jellicent at East Lake Axewell, North Lake Miloch, Lake of Outrage
            new(SWSH) { Species = 593, Level = 46, Location = 130, Weather = Overcast | Raining | Heavy_Fog }, // Jellicent at West Lake Axewell
            new EncounterStatic8S(SWSH) { Species = 593, Level = 46, Locations = new[] {134, 142}, Weather = Raining | Heavy_Fog }, // Jellicent at South Lake Miloch, Bridge Field
            new(SWSH) { Species = 171, Level = 46, Location = 134, Weather = Thunderstorm }, // Lanturn at South Lake Miloch
            new EncounterStatic8S(SWSH) { Species = 171, Level = 46, Locations = new[] {128, 154}, Weather = Normal | Thunderstorm | Heavy_Fog }, // Lanturn at East Lake Axewell, the Lake of Outrage
            new(SWSH) { Species = 340, Level = 46, Location = 134, Weather = Normal | Thunderstorm | Intense_Sun | Sandstorm }, // Whiscash at South Lake Miloch
            new(SWSH) { Species = 340, Level = 46, Location = 138, Weather = Normal | Overcast | Thunderstorm | Intense_Sun | Sandstorm }, // Whiscash at North Lake Miloch
            new EncounterStatic8S(SWSH) { Species = 426, Level = 46, Locations = new[] {134, 138}, Weather = Normal | Overcast | Snowstorm | Heavy_Fog }, // Drifblim at North/South Lake Miloch
            new(SWSH) { Species = 224, Level = 46, Location = 134, Weather = Normal | Intense_Sun | Sandstorm | Heavy_Fog }, // Octillery at South Lake Miloch
            new(SWSH) { Species = 612, Level = 60, Location = 132, Ability = 1, Weather = Normal | Overcast | Raining | Intense_Sun | Sandstorm | Heavy_Fog }, // Haxorus on Axew’s Eye
            new(SWSH) { Species = 143, Level = 36, Location = 140, Weather = Normal | Overcast | Stormy | Intense_Sun | Icy | Sandstorm }, // Snorlax at the Motostoke Riverbank
            new(SWSH) { Species = 452, Level = 40, Location = 140, Weather = Normal | Stormy | Intense_Sun | Sandstorm }, // Drapion at the Motostoke Riverbank
            new(SWSH) { Species = 561, Level = 36, Location = 140, Weather = All }, // Sigilyph at the Motostoke Riverbank
            new(SWSH) { Species = 534, Level = 55, Location = 140, Ability = 1, Weather = Normal | Overcast | Stormy | Snowing | Heavy_Fog }, // Conkeldurr at the Motostoke Riverbank
            new(SWSH) { Species = 320, Level = 56, Location = 140, Weather = All }, // Wailmer at the Motostoke Riverbank
            new(SWSH) { Species = 561, Level = 40, Location = 140, Weather = Normal | Overcast | Heavy_Fog }, // Sigilyph at the Motostoke Riverbank
            new(SWSH) { Species = 569, Level = 40, Location = 142, Weather = All }, // Garbodor in Bridge Field
            new(SWSH) { Species = 743, Level = 40, Location = 142, Weather = Normal | Overcast | Intense_Sun | Heavy_Fog }, // Ribombee in Bridge Field
            new(SWSH) { Species = 475, Level = 60, Location = 142, Weather = Normal | Overcast | Stormy | Intense_Sun | Sandstorm }, // Gallade in Bridge Field
            new EncounterStatic8S(SWSH) { Species = 264, Level = 40, Locations = new[] {140, 142}, Form = 01, Weather = All }, // Linoone at the Motostoke Riverbank, Bridge Field
            new(SWSH) { Species = 606, Level = 42, Location = 142, Weather = Normal | Overcast | Icy | Heavy_Fog }, // Beheeyem in Bridge Field
            new(SWSH) { Species = 715, Level = 50, Location = 142, Weather = Normal | Overcast | Stormy | Intense_Sun | Snowstorm  | Sandstorm | Heavy_Fog }, // Noivern in Bridge Field
            new(SWSH) { Species = 537, Level = 46, Location = 142, Weather = Normal | Overcast | Stormy | Heavy_Fog }, // Seismitoad in Bridge Field
            new(SWSH) { Species = 768, Level = 50, Location = 142, Weather = Normal | Stormy }, // Golisopod in Bridge Field
            new(SWSH) { Species = 760, Level = 42, Location = 142, Weather = Normal | Stormy | Icy | Sandstorm | Heavy_Fog }, // Bewear in Bridge Field
            new(SWSH) { Species = 820, Level = 42, Location = 142, Weather = All }, // Greedent in Bridge Field
            new EncounterStatic8S(SWSH) { Species = 598, Level = 40, Locations = new[] {142, 144}, Weather = All }, // Ferrothorn in Bridge Field, Stony Wilderness
            new(SWSH) { Species = 344, Level = 42, Location = 144, Weather = Normal | Overcast | Intense_Sun | Sandstorm }, // Claydol in the Stony Wilderness
            new(SWSH) { Species = 477, Level = 60, Location = 144, Weather = All }, // Dusknoir in the Stony Wilderness
            new(SWSH) { Species = 623, Level = 43, Location = 144, Weather = All }, // Golurk in the Stony Wilderness
            new(SWSH) { Species = 561, Level = 40, Location = 144, Weather = Normal | Stormy | Intense_Sun | Icy | Sandstorm | Heavy_Fog }, // Sigilyph in the Stony Wilderness
            new(SWSH) { Species = 558, Level = 34, Location = 144, Weather = Normal | Overcast | Intense_Sun | Icy | Sandstorm | Heavy_Fog }, // Crustle in the Stony Wilderness
            new(SWSH) { Species = 112, Level = 41, Location = 144, Weather = All }, // Rhydon in the Stony Wilderness
            new(SWSH) { Species = 763, Level = 36, Location = 144, Weather = Normal | Overcast | Intense_Sun | Icy | Sandstorm | Heavy_Fog }, // Tsareena in the Stony Wilderness
            new(SWSH) { Species = 750, Level = 41, Location = 146, Weather = Normal | Intense_Sun | Sandstorm }, // Mudsdale in Dusty Bowl
            new(SWSH) { Species = 185, Level = 41, Location = 146, Weather = Normal | Overcast | Intense_Sun | Icy | Sandstorm | Heavy_Fog }, // Sudowoodo in Dusty Bowl
            new(SWSH) { Species = 437, Level = 41, Location = 146, Weather = Normal | Stormy | Icy | Heavy_Fog }, // Bronzong in Dusty Bowl
            new(  SH) { Species = 248, Level = 60, Location = 146, Weather = Normal | Intense_Sun | Icy | Sandstorm | Heavy_Fog }, // Tyranitar in Dusty Bowl
            new(SW  ) { Species = 784, Level = 60, Location = 146, Ability = 1, Weather = Normal | Intense_Sun | Icy | Sandstorm | Heavy_Fog }, // Kommo-o in Dusty Bowl
            new(SWSH) { Species = 213, Level = 34, Location = 146, Weather = All }, // Shuckle in Dusty Bowl
            new(SWSH) { Species = 330, Level = 51, Location = 146, Weather = Normal | Sandstorm }, // Flygon in Dusty Bowl
            new(SWSH) { Species = 526, Level = 51, Location = 146, Weather = Normal | Intense_Sun }, // Gigalith in Dusty Bowl
            new EncounterStatic8S(SWSH) { Species = 423, Level = 56, Locations = new[] {146, 148}, Form = 01, Weather = All }, // Gastrodon in Dusty Bowl, Giant’s Mirror
            new(SWSH) { Species = 208, Level = 50, Location = 148, Weather = All }, // Steelix around the Giant’s Mirror
            new(SWSH) { Species = 068, Level = 60, Location = 148, Ability = 1, Weather = All }, // Machamp around the Giant’s Mirror
            new(SWSH) { Species = 182, Level = 41, Location = 148, Weather = Normal | Intense_Sun | Heavy_Fog }, // Bellossom around the Giant’s Mirror
            new(SWSH) { Species = 521, Level = 41, Location = 148, Weather = Normal | Overcast | Intense_Sun }, // Unfezant around the Giant’s Mirror
            new(SWSH) { Species = 701, Level = 36, Location = 150, Weather = All }, // Hawlucha on the Hammerlocke Hills
            new(SWSH) { Species = 094, Level = 60, Location = 152, Weather = Normal | Overcast | Thunderstorm | Intense_Sun | Snowstorm  | Sandstorm | Heavy_Fog }, // Gengar near the Giant’s Cap
            new(SWSH) { Species = 823, Level = 39, Location = 152, Weather = All }, // Corviknight near the Giant’s Cap
            new(SWSH) { Species = 573, Level = 46, Location = 152, Weather = Normal | Overcast | Heavy_Fog }, // Cinccino near the Giant’s Cap
            new(SWSH) { Species = 826, Level = 41, Location = 152, Weather = All }, // Orbeetle near the Giant’s Cap
            new(SWSH) { Species = 834, Level = 36, Location = 152, Weather = All }, // Drednaw near the Giant’s Cap
            new(SWSH) { Species = 680, Level = 56, Location = 152, Weather = Normal | Overcast | Stormy | Intense_Sun | Icy | Sandstorm }, // Doublade near the Giant’s Cap
            new(SWSH) { Species = 711, Level = 41, Location = 150, Weather = All }, // Gourgeist on the Hammerlocke Hills
            new(SWSH) { Species = 600, Level = 46, Location = 150, Weather = Normal | Overcast | Raining | Snowing }, // Klang on the Hammerlocke Hills
            new(SWSH) { Species = 045, Level = 41, Location = 148, Weather = Overcast | Stormy | Icy | Sandstorm }, // Vileplume around the Giant’s Mirror
            new(SWSH) { Species = 823, Level = 38, Location = 150, Weather = All }, // Corviknight on the Hammerlocke Hills
            new(SWSH) { Species = 853, Level = 56, Location = 154, Weather = All }, // Grapploct at the Lake of Outrage
            new(SWSH) { Species = 282, Level = 60, Location = 154, Weather = Normal | Heavy_Fog }, // Gardevoir at the Lake of Outrage
            new(SWSH) { Species = 470, Level = 56, Location = 154, Weather = Normal }, // Leafeon at the Lake of Outrage
          //new(SWSH) { Species = 510, Level = 31, Location =  -1, }, // Liepard
            new(SWSH) { Species = 832, Level = 65, Location = 122, Weather = All }, // Dubwool in the Rolling Fields
            new(SWSH) { Species = 826, Level = 65, Location = 124, Weather = All }, // Orbeetle in the Dappled Grove
            new(SWSH) { Species = 823, Level = 65, Location = 126, Weather = All }, // Corviknight at Watchtower Ruins
            new(SWSH) { Species = 110, Level = 65, Location = 128, Form = 01, Weather = All }, // Weezing at East Lake Axewell
            new(SWSH) { Species = 834, Level = 65, Location = 130, Weather = All }, // Drednaw at West Lake Axewell
            new(SWSH) { Species = 845, Level = 65, Location = 132, Weather = All }, // Cramorant on Axew’s Eye
            new(SWSH) { Species = 828, Level = 65, Location = 134, Weather = All }, // Thievul at South Lake Miloch
            new(SWSH) { Species = 884, Level = 65, Location = 136, Weather = All }, // Duraludon near the Giant’s Seat
            new(SWSH) { Species = 836, Level = 65, Location = 138, Weather = All }, // Boltund at North Lake Miloch
            new(SWSH) { Species = 830, Level = 65, Location = 140, Weather = All }, // Eldegoss at the Motostoke Riverbank
            new(SWSH) { Species = 862, Level = 65, Location = 142, Weather = All }, // Obstagoon in Bridge Field
            new(SWSH) { Species = 861, Level = 65, Location = 144, Gender = 0, Weather = All }, // Grimmsnarl in the Stony Wilderness
            new(SWSH) { Species = 844, Level = 65, Location = 146, Weather = All }, // Sandaconda in Dusty Bowl
            new(SWSH) { Species = 863, Level = 65, Location = 148, Weather = All }, // Perrserker around the Giant’s Mirror
            new(SWSH) { Species = 879, Level = 65, Location = 150, Weather = All }, // Copperajah on the Hammerlocke Hills
            new(SWSH) { Species = 839, Level = 65, Location = 152, Weather = All }, // Coalossal near the Giant’s Cap
            new(SWSH) { Species = 858, Level = 65, Location = 154, Gender = 1, Weather = All }, // Hatterene at the Lake of Outrage
            new EncounterStatic8S(SWSH) { Species = 279, Level = 26, Locations = new[] {122, 128, 138}, Weather = Stormy }, // Pelipper in the Rolling Fields, North Lake Miloch, East Lake Axewell
            new(SWSH) { Species = 310, Level = 26, Location = 122, Weather = Thunderstorm }, // Manectric in the Rolling Fields
            new(SWSH) { Species = 660, Level = 26, Location = 122, Weather = Overcast | Intense_Sun | Icy | Sandstorm}, // Diggersby in the Rolling Fields
            new(SWSH) { Species = 281, Level = 26, Location = 122, Weather = Heavy_Fog }, // Kirlia in the Rolling Fields
            new(SWSH) { Species = 025, Level = 15, Location = 122, Weather = Thunderstorm }, // Pikachu in the Rolling Fields
            new(SWSH) { Species = 439, Level = 15, Location = 122, Weather = Snowstorm }, // Mime Jr. in the Rolling Fields
            new(SWSH) { Species = 221, Level = 33, Location = 122, Weather = Icy }, // Piloswine in the Rolling Fields
            new(SWSH) { Species = 558, Level = 34, Location = 122, Weather = Sandstorm }, // Crustle in the Rolling Fields
            new(SWSH) { Species = 282, Level = 32, Location = 122, Weather = Heavy_Fog }, // Gardevoir in the Rolling Fields
            new(SWSH) { Species = 537, Level = 36, Location = 124, Weather = Stormy }, // Seismitoad in the Dappled Grove
            new(SWSH) { Species = 537, Level = 36, Location = 138, Weather = Overcast | Thunderstorm }, // Seismitoad at North Lake Miloch
            new(SWSH) { Species = 537, Level = 36, Location = 142, Weather = Normal | Overcast | Stormy | Heavy_Fog }, // Seismitoad in Bridge Field
            new(SWSH) { Species = 583, Level = 36, Location = 124, Weather = Icy }, // Vanillish in the Dappled Grove
            new(SWSH) { Species = 344, Level = 36, Location = 124, Weather = Intense_Sun | Sandstorm }, // Claydol in the Dappled Grove
            new(SWSH) { Species = 093, Level = 34, Location = 126, Weather = Overcast | Stormy | Snowstorm | Sandstorm | Heavy_Fog }, // Haunter at Watchtower Ruins
            new EncounterStatic8S(SWSH) { Species = 356, Level = 40, Locations = new[] {126, 130}, Weather = Overcast | Stormy | Heavy_Fog }, // Dusclops at Watchtower Ruins, West Lake Axewell
            new EncounterStatic8S(SWSH) { Species = 362, Level = 40, Locations = new[] {126, 130}, Weather = Icy }, // Glalie at Watchtower Ruins, West Lake Axewell
            new(SWSH) { Species = 279, Level = 28, Location = 130, Weather = Stormy }, // Pelipper at West Lake Axewell
            new(SWSH) { Species = 536, Level = 28, Location = 130, Weather = Overcast | Icy | Sandstorm | Heavy_Fog }, // Palpitoad at West Lake Axewell
            new(SWSH) { Species = 660, Level = 28, Location = 130, Weather = Intense_Sun }, // Diggersby at West Lake Axewell
            new(SWSH) { Species = 221, Level = 36, Location = 128, Weather = Icy }, // Piloswine at East Lake Axewell
            new(SWSH) { Species = 750, Level = 36, Location = 128, Weather = Intense_Sun | Sandstorm }, // Mudsdale at East Lake Axewell
            new(SWSH) { Species = 437, Level = 36, Location = 128, Weather = Heavy_Fog }, // Bronzong at East Lake Axewell
            new(SWSH) { Species = 536, Level = 34, Location = 134, Weather = Stormy }, // Palpitoad at South Lake Miloch
            new(SWSH) { Species = 093, Level = 31, Location = 122, Weather = Stormy }, // Haunter in the Rolling Fields
            new(SWSH) { Species = 221, Level = 33, Location = 122, Weather = Icy }, // Piloswine in the Rolling Fields
            new(SWSH) { Species = 558, Level = 34, Location = 134, Weather = Intense_Sun }, // Crustle at South Lake Miloch
            new(SWSH) { Species = 067, Level = 31, Location = 134, Weather = Overcast | Intense_Sun | Icy | Sandstorm }, // Machoke at South Lake Miloch
            new(SWSH) { Species = 426, Level = 31, Location = 134, Weather = Heavy_Fog }, // Drifblim at South Lake Miloch
            new(SWSH) { Species = 435, Level = 36, Location = 138, Weather = Raining | Intense_Sun | Sandstorm }, // Skuntank at North Lake Miloch
            new(SWSH) { Species = 583, Level = 36, Location = 138, Weather = Icy }, // Vanillish at North Lake Miloch
            new(SWSH) { Species = 426, Level = 36, Location = 138, Weather = Heavy_Fog }, // Drifblim at North Lake Miloch
            new(SWSH) { Species = 437, Level = 46, Location = 136, Weather = Overcast | Raining }, // Bronzong near the Giant’s Seat
            new(SWSH) { Species = 460, Level = 46, Location = 136, Weather = Icy }, // Abomasnow near the Giant’s Seat
            new(SWSH) { Species = 750, Level = 46, Location = 136, Weather = Intense_Sun }, // Mudsdale near the Giant’s Seat
            new(SWSH) { Species = 623, Level = 46, Location = 136, Weather = Sandstorm }, // Golurk near the Giant’s Seat
            new(SWSH) { Species = 356, Level = 46, Location = 136, Weather = Heavy_Fog }, // Dusclops near the Giant’s Seat
            new(SWSH) { Species = 518, Level = 46, Location = 136, Weather = Heavy_Fog }, // Musharna near the Giant’s Seat
            new(SWSH) { Species = 362, Level = 46, Location = 136, Weather = Icy }, // Glalie near the Giant’s Seat
            new(SWSH) { Species = 596, Level = 46, Location = 136, Weather = Raining }, // Galvantula near the Giant’s Seat
            new(SWSH) { Species = 596, Level = 46, Location = 134, Weather = All }, // Galvantula at South Lake Miloch
            new EncounterStatic8S(SWSH) { Species = 584, Level = 47, Locations = new[] {128, 130, 134, 138, 142}, Weather = Icy }, // Vanilluxe at North/East/South/West Lake Miloch/Axewell, Bridge Field
            new(SWSH) { Species = 537, Level = 60, Location = 132, Weather = Thunderstorm }, // Seismitoad on Axew’s Eye
            new(SWSH) { Species = 460, Level = 60, Location = 132, Weather = Icy }, // Abomasnow on Axew’s Eye
            new(SWSH) { Species = 036, Level = 36, Location = 140, Weather = Heavy_Fog }, // Clefable at the Motostoke Riverbank
            new(SWSH) { Species = 743, Level = 40, Location = 140, Weather = Overcast | Icy | Heavy_Fog }, // Ribombee at the Motostoke Riverbank
            new(SWSH) { Species = 112, Level = 55, Location = 140, Weather = Intense_Sun | Sandstorm }, // Rhydon at the Motostoke Riverbank
            new(SWSH) { Species = 823, Level = 40, Location = 140, Weather = Stormy | Intense_Sun | Icy | Sandstorm }, // Corviknight at the Motostoke Riverbank
            new EncounterStatic8S(SWSH) { Species = 760, Level = 40, Locations = new[] {140, 142}, Weather = Thunderstorm | Sandstorm }, // Bewear in Bridge Field, Motostoke Riverbank
            new(SWSH) { Species = 614, Level = 60, Location = 142, Weather = Snowing }, // Beartic in Bridge Field
            new(SWSH) { Species = 461, Level = 60, Location = 142, Weather = Snowstorm }, // Weavile in Bridge Field
            new(SWSH) { Species = 518, Level = 60, Location = 142, Weather = Heavy_Fog }, // Musharna in Bridge Field
            new(SWSH) { Species = 437, Level = 42, Location = 142, Weather = Stormy }, // Bronzong in Bridge Field
            new(SWSH) { Species = 437, Level = 42, Location = 144, Weather = Stormy | Icy | Heavy_Fog }, // Bronzong in Stony Wilderness
            new(SWSH) { Species = 344, Level = 42, Location = 142, Weather = Intense_Sun | Sandstorm }, // Claydol in Bridge Field
            new(SWSH) { Species = 452, Level = 50, Location = 142, Weather = Overcast }, // Drapion in Bridge Field
            new(SWSH) { Species = 164, Level = 50, Location = 142, Weather = Snowing }, // Noctowl in Bridge Field
            new(SWSH) { Species = 760, Level = 46, Location = 142, Weather = Intense_Sun | Sandstorm }, // Bewear in Bridge Field
            new(SWSH) { Species = 675, Level = 42, Location = 142, Weather = Overcast | Intense_Sun }, // Pangoro in Bridge Field
            new(SWSH) { Species = 584, Level = 50, Location = 142, Weather = Icy }, // Vanilluxe in Bridge Field
            new(SWSH) { Species = 112, Level = 50, Location = 142, Weather = Intense_Sun | Sandstorm }, // Rhydon in Bridge Field
            new(SWSH) { Species = 778, Level = 50, Location = 142, Weather = Heavy_Fog }, // Mimikyu in Bridge Field
            new(SWSH) { Species = 521, Level = 40, Location = 144, Weather = Overcast }, // Unfezant in the Stony Wilderness
            new(SWSH) { Species = 752, Level = 34, Location = 144, Weather = Raining }, // Araquanid in the Stony Wilderness
            new(SWSH) { Species = 537, Level = 41, Location = 146, Weather = Stormy }, // Seismitoad in Dusty Bowl
            new(SWSH) { Species = 435, Level = 41, Location = 146, Weather = Overcast }, // Skuntank in Dusty Bowl
            new(SWSH) { Species = 221, Level = 41, Location = 146, Weather = Icy }, // Piloswine in Dusty Bowl
            new(SWSH) { Species = 356, Level = 41, Location = 146, Weather = Heavy_Fog }, // Dusclops in Dusty Bowl
            new(SWSH) { Species = 344, Level = 41, Location = 146, Weather = Overcast | Intense_Sun | Sandstorm }, // Claydol in Dusty Bowl
            new(SWSH) { Species = 689, Level = 60, Location = 146, Weather = Overcast | Stormy }, // Barbaracle in Dusty Bowl
            new(SWSH) { Species = 561, Level = 51, Location = 146, Weather = Overcast | Stormy | Intense_Sun | Icy | Heavy_Fog }, // Sigilyph in Dusty Bowl
            new(SWSH) { Species = 623, Level = 51, Location = 146, Weather = Overcast | Stormy | Icy | Sandstorm | Heavy_Fog }, // Golurk in Dusty Bowl
            new(SWSH) { Species = 537, Level = 60, Location = 148, Weather = Raining }, // Seismitoad around the Giant’s Mirror
            new(SWSH) { Species = 460, Level = 60, Location = 148, Weather = Snowing }, // Abomasnow around the Giant’s Mirror
            new(SWSH) { Species = 045, Level = 41, Location = 150, Weather = Overcast | Stormy | Icy | Sandstorm }, // Vileplume on the Hammerlocke Hills
            new(SWSH) { Species = 178, Level = 41, Location = 148, Weather = Raining | Icy | Sandstorm | Heavy_Fog }, // Xatu around the Giant’s Mirror
            new(SWSH) { Species = 768, Level = 60, Location = 152, Weather = Raining }, // Golisopod near the Giant’s Cap
            new(SWSH) { Species = 614, Level = 60, Location = 152, Weather = Snowing }, // Beartic near the Giant’s Cap
            new(SWSH) { Species = 530, Level = 46, Location = 152, Weather = Intense_Sun | Sandstorm }, // Excadrill near the Giant’s Cap
            new(SWSH) { Species = 362, Level = 46, Location = 152, Weather = Icy }, // Glalie near the Giant’s Cap
            new(SWSH) { Species = 537, Level = 46, Location = 152, Weather = Raining }, // Seismitoad near the Giant’s Cap
            new(SWSH) { Species = 681, Level = 58, Location = 152, Weather = Heavy_Fog }, // Aegislash near the Giant’s Cap
            new(SWSH) { Species = 601, Level = 49, Location = 150, Weather = Thunderstorm | Intense_Sun | Snowstorm  | Sandstorm | Heavy_Fog }, // Klinklang on the Hammerlocke Hills
            new(SWSH) { Species = 407, Level = 41, Location = 150, Weather = Overcast | Heavy_Fog }, // Roserade on the Hammerlocke Hills
            new(SWSH) { Species = 460, Level = 41, Location = 150, Weather = Icy}, // Abomasnow on the Hammerlocke Hills
            new EncounterStatic8S(SWSH) { Species = 350, Level = 60, Locations = new[] {134, 154}, Gender = 0, Ability = 1, Weather = Heavy_Fog }, // Milotic at South Lake Miloch, the Lake of Outrage
            new EncounterStatic8S(SWSH) { Species = 130, Level = 60, Locations = new[] {134, 138, 154 }, Weather = Normal | Overcast | Stormy | Intense_Sun | Icy | Sandstorm }, // Gyarados at North/South Lake Miloch, the Lake of Outrage
            new(SWSH) { Species = 112, Level = 60, Location = 154, Weather = Sandstorm }, // Rhydon at the Lake of Outrage
            new(SWSH) { Species = 609, Level = 60, Location = 154, Weather = Intense_Sun }, // Chandelure at the Lake of Outrage
            new(SWSH) { Species = 713, Level = 60, Location = 154, Weather = Icy }, // Avalugg at the Lake of Outrage
            new(SWSH) { Species = 756, Level = 60, Location = 154, Weather = Overcast | Stormy }, // Shiinotic at the Lake of Outrage
            new(SWSH) { Species = 134, Level = 56, Location = 154, Weather = Raining }, // Vaporeon at the Lake of Outrage
            new(SWSH) { Species = 135, Level = 56, Location = 154, Weather = Thunderstorm }, // Jolteon at the Lake of Outrage
            new(SWSH) { Species = 196, Level = 56, Location = 154, Weather = Overcast }, // Espeon at the Lake of Outrage
            new(SWSH) { Species = 471, Level = 56, Location = 154, Weather = Icy }, // Glaceon at the Lake of Outrage
            new(SWSH) { Species = 136, Level = 56, Location = 154, Weather = Intense_Sun }, // Flareon at the Lake of Outrage
            new(SWSH) { Species = 197, Level = 56, Location = 154, Weather = Sandstorm }, // Umbreon at the Lake of Outrage
            new(SWSH) { Species = 700, Level = 56, Location = 154, Weather = Heavy_Fog }, // Sylveon at the Lake of Outrage
            #endregion

            #region R1 Static Encounters
            new(SWSH) { Species = 079, Level = 12, Location = 016, ScriptedNoMarks = true, Form = 01, Shiny = Never }, // Slowpoke-1 at Wedgehurst Station
            new(SWSH) { Species = 748, Level = 20, Location = 164, Weather = Normal | Heavy_Fog }, // Toxapex in the Fields of Honor
            new(SWSH) { Species = 099, Level = 20, Location = 164, Weather = Normal | Overcast | Stormy | Intense_Sun | Sandstorm }, // Kingler in the Fields of Honor
            new(SWSH) { Species = 834, Level = 20, Location = 164, Weather = Intense_Sun }, // Drednaw in the Fields of Honor
            new(SWSH) { Species = 834, Level = 20, Location = 166, Weather = Normal | Overcast | Intense_Sun | Sandstorm }, // Drednaw in the Soothing Wetlands
            new EncounterStatic8S(SWSH) { Species = 687, Level = 26, Locations = new[] {164, 166}, Weather = Overcast | Raining }, // Malamar in the Fields of Honor, Soothing Wetlands
            new(SWSH) { Species = 764, Level = 15, Location = 164, Weather = Normal | Sandstorm }, // Comfey in the Fields of Honor
            new(SWSH) { Species = 764, Level = 15, Location = 166, Weather = Normal | Intense_Sun | Heavy_Fog }, // Comfey in the Soothing Wetlands
            new EncounterStatic8S(SWSH) { Species = 404, Level = 20, Locations = new[] {164, 166}, Weather = Thunderstorm }, // Luxio in the Fields of Honor, Soothing Wetlands
            new(SWSH) { Species = 744, Level = 15, Location = 164, Weather = Normal | Intense_Sun }, // Rockruff in the Fields of Honor
            new(SWSH) { Species = 744, Level = 15, Location = 166 }, // Rockruff in the Soothing Wetlands
            new(SWSH) { Species = 195, Level = 20, Location = 166, Weather = Normal | Overcast | Raining | Intense_Sun | Sandstorm | Heavy_Fog }, // Quagsire in the Soothing Wetlands
            new EncounterStatic8S(SWSH) { Species = 570, Level = 15, Locations = new[] {164, 166}, Weather = Overcast | Heavy_Fog }, // Zorua in the Fields of Honor, Soothing Wetlands
            new EncounterStatic8S(SWSH) { Species = 040, Level = 27, Locations = new[] {164, 166}, Weather = Heavy_Fog }, // Wigglytuff in the Fields of Honor, Soothing Wetlands
            new(SWSH) { Species = 626, Level = 20, Location = 166, Weather = Normal | Overcast | Stormy | Intense_Sun | Sandstorm }, // Bouffalant in the Soothing Wetlands
            new(SWSH) { Species = 242, Level = 22, Location = 166, Weather = Heavy_Fog }, // Blissey in the Soothing Wetlands
            new(SWSH) { Species = 452, Level = 22, Location = 166, Weather = Normal | Overcast | Raining | Sandstorm }, // Drapion in the Soothing Wetlands
            new(SWSH) { Species = 463, Level = 27, Location = 166, Weather = Normal | Raining | Sandstorm }, // Lickilicky in the Soothing Wetlands
          //new(SWSH) { Species = 834, Level = 21, Location = -1 }, // Drednaw
            new(SWSH) { Species = 405, Level = 32, Location = 166, Weather = Thunderstorm }, // Luxray in the Soothing Wetlands
            new(SWSH) { Species = 121, Level = 20, Location = 164, Weather = All_IoA }, // Starmie in the Fields of Honor
            new(SWSH) { Species = 428, Level = 22, Location = 164, Weather = Normal | Intense_Sun | Sandstorm }, // Lopunny in the Fields of Honor
            new(SWSH) { Species = 428, Level = 22, Location = 166, Weather = Normal | Sandstorm }, // Lopunny in the Soothing Wetlands
            new(SWSH) { Species = 186, Level = 32, Location = 166, Weather = Stormy }, // Politoed in the Soothing Wetlands
            new(SWSH) { Species = 061, Level = 20, Location = 166, Weather = Stormy | Heavy_Fog }, // Poliwhirl in the Soothing Wetlands
            new EncounterStatic8S(SWSH) { Species = 183, Level = 15, Locations = new[] {164, 166}, Weather = Raining }, // Marill in the Fields of Honor, Soothing Wetlands
            new(SWSH) { Species = 183, Level = 15, Location = 170, Weather = Normal | Sandstorm }, // Marill on Challenge Beach
            new EncounterStatic8S(SWSH) { Species = 662, Level = 20, Locations = new[] {164, 166}, Weather = Intense_Sun }, // Fletchinder in the Fields of Honor, in the Soothing Wetlands
          //new(SWSH) { Species = 768, Level = 26, Location = -1 }, // Golisopod
            new(SWSH) { Species = 636, Level = 15, Location = 168, Weather = Intense_Sun }, // Larvesta in the Forest of Focus
            new(SWSH) { Species = 549, Level = 22, Location = 166, Weather = Intense_Sun }, // Lilligant in the Soothing Wetlands
            new(SWSH) { Species = 025, Level = 22, Location = 168, Weather = Normal | Overcast | Stormy }, // Pikachu in the Forest of Focus
            new EncounterStatic8S(SWSH) { Species = 064, Level = 20, Locations = new[] {164, 166}, Weather = Heavy_Fog }, // Kadabra in the Fields of Honor, in the Soothing Wetlands
            new EncounterStatic8S(SWSH) { Species = 026, Level = 26, Locations = new[] {166, 168}, Weather = Thunderstorm }, // Raichu in the Soothing Wetlands, in the Forest of Focus
            new EncounterStatic8S(SWSH) { Species = 025, Level = 15, Locations = new[] {164, 166}, Weather = Thunderstorm }, // Pikachu in the Fields of Honor, in the Soothing Wetlands
            new EncounterStatic8S(SWSH) { Species = 184, Level = 21, Locations = new[] {166, 168}, Weather = Heavy_Fog }, // Azumarill in the Soothing Wetlands, in the Forest of Focus
            new(  SH) { Species = 453, Level = 20, Location = 166, Weather = Overcast }, // Croagunk in the Soothing Wetlands
            new(SW  ) { Species = 559, Level = 20, Location = 166, Weather = Overcast }, // Scraggy in the Soothing Wetlands
            new(SWSH) { Species = 663, Level = 32, Location = 166, Weather = Intense_Sun }, // Talonflame in the Soothing Wetlands
            new(SW  ) { Species = 766, Level = 26, Location = 168 }, // Passimian in the Forest of Focus
            new(  SH) { Species = 765, Level = 26, Location = 168 }, // Oranguru in the Forest of Focus
            new(SWSH) { Species = 342, Level = 26, Location = 168, Weather = Overcast | Stormy }, // Crawdaunt in the Forest of Focus
            new EncounterStatic8S(SWSH) { Species = 754, Level = 27, Locations = new[] {168, 170}, Weather = Intense_Sun }, // Lurantis in the Forest of Focus, on Challenge Beach
            new(SWSH) { Species = 040, Level = 26, Location = 168, Weather = Heavy_Fog }, // Wigglytuff in the Forest of Focus
            new(SWSH) { Species = 028, Level = 26, Location = 168, Weather = Sandstorm }, // Sandslash in the Forest of Focus
            new(SWSH) { Species = 545, Level = 32, Location = 168, Weather = Overcast }, // Scolipede in the Forest of Focus
            new(SW  ) { Species = 127, Level = 26, Location = 168, Weather = Intense_Sun }, // Pinsir in the Forest of Focus
            new(  SH) { Species = 214, Level = 26, Location = 168, Weather = Intense_Sun }, // Heracross in the Forest of Focus
            new(SW  ) { Species = 616, Level = 20, Location = 168, Weather = Stormy }, // Shelmet in the Forest of Focus
            new(  SH) { Species = 704, Level = 20, Location = 168, Weather = Stormy }, // Goomy in the Forest of Focus
            new(SWSH) { Species = 172, Level = 20, Location = 168, Weather = Thunderstorm }, // Pichu in the Forest of Focus
            new(SWSH) { Species = 845, Level = 20, Location = 168, Weather = Normal | Raining | Intense_Sun | Heavy_Fog }, // Cramorant in the Forest of Focus
            new(SWSH) { Species = 465, Level = 36, Location = 168, Weather = Intense_Sun }, // Tangrowth in the Forest of Focus
            new(SWSH) { Species = 589, Level = 32, Location = 168, Weather = Sandstorm }, // Escavalier in the Forest of Focus
            new(SWSH) { Species = 617, Level = 32, Location = 168, Weather = Raining }, // Accelgor in the Forest of Focus
            new(SWSH) { Species = 591, Level = 26, Location = 168, Weather = Normal | Overcast | Raining | Intense_Sun }, // Amoonguss in the Forest of Focus
            new(SWSH) { Species = 764, Level = 22, Location = 168, Weather = Intense_Sun | Heavy_Fog }, // Comfey in the Forest of Focus
            new(SWSH) { Species = 570, Level = 22, Location = 168, Weather = Heavy_Fog }, // Zorua in the Forest of Focus
            new(SWSH) { Species = 104, Level = 20, Location = 168, Weather = Sandstorm }, // Cubone in the Forest of Focus
            new EncounterStatic8S(SWSH) { Species = 282, Level = 36, Locations = new[] {168, 180}, Weather = Heavy_Fog }, // Gardevoir in the Forest of Focus, Training Lowlands
            new(SWSH) { Species = 055, Level = 26, Location = 168, Weather = Raining }, // Golduck in the Forest of Focus
            new(SWSH) { Species = 039, Level = 20, Location = 168, Weather = Heavy_Fog }, // Jigglypuff in the Forest of Focus
            new(SWSH) { Species = 462, Level = 36, Location = 170, Weather = Thunderstorm }, // Magnezone on Challenge Beach
          //new(SWSH) { Species = 475, Level = 20, Location = -1 }, // Gallade
          //new(SWSH) { Species = 625, Level = 20, Location = -1 }, // Bisharp
          //new(SWSH) { Species = 082, Level = 27, Location = -1 }, // Magneton
          //new(SWSH) { Species = 105, Level = 20, Location = -1 }, // Marowak
            new(SWSH) { Species = 637, Level = 42, Location = 170, Weather = Intense_Sun }, // Volcarona on Challenge Beach
            new(SWSH) { Species = 687, Level = 29, Location = 170, Weather = Overcast | Raining }, // Malamar on Challenge Beach
            new(SWSH) { Species = 428, Level = 27, Location = 170, Weather = Normal | Sandstorm | Heavy_Fog }, // Lopunny on Challenge Beach
            new(SWSH) { Species = 452, Level = 27, Location = 170, Weather = Overcast | Raining }, // Drapion on Challenge Beach
            new(SWSH) { Species = 026, Level = 29, Location = 170, Weather = Thunderstorm }, // Raichu on Challenge Beach
          //new(SWSH) { Species = 558, Level = 20, Location = -1 }, // Crustle
            new(SWSH) { Species = 764, Level = 25, Location = 170, Weather = Normal | Intense_Sun | Heavy_Fog }, // Comfey on Challenge Beach
            new(SWSH) { Species = 877, Level = 25, Location = 170, Weather = Normal | Overcast | Stormy }, // Morpeko on Challenge Beach
            new(SWSH) { Species = 834, Level = 26, Location = 170, Weather = Normal | Overcast | Thunderstorm | Intense_Sun | Sandstorm }, // Drednaw on Challenge Beach
            new(SWSH) { Species = 040, Level = 29, Location = 170, Weather = Heavy_Fog }, // Wigglytuff on Challenge Beach
            new(SWSH) { Species = 528, Level = 27, Location = 170, Weather = Overcast }, // Swoobat on Challenge Beach
            new(SWSH) { Species = 279, Level = 26, Location = 170, Weather = All_IoA }, // Pelipper on Challenge Beach
            new(SWSH) { Species = 082, Level = 26, Location = 170, Weather = Thunderstorm }, // Magneton on Challenge Beach
            new(SW  ) { Species = 782, Level = 22, Location = 174, Weather = Intense_Sun | Sandstorm | Heavy_Fog }, // Jangmo-o on Challenge Road
            new(SW  ) { Species = 782, Level = 22, Location = 180, Weather = Sandstorm }, // Jangmo-o in Training Lowlands
            new(SWSH) { Species = 426, Level = 26, Location = 170, Weather = Overcast | Heavy_Fog }, // Drifblim on Challenge Beach
            new(SWSH) { Species = 768, Level = 36, Location = 170, Weather = Raining }, // Golisopod on Challenge Beach
            new(SWSH) { Species = 662, Level = 26, Location = 170, Weather = Intense_Sun }, // Fletchinder on Challenge Beach
            new(SWSH) { Species = 342, Level = 27, Location = 170, Weather = Overcast | Raining }, // Crawdaunt on Challenge Beach
            new(SWSH) { Species = 184, Level = 27, Location = 170, Weather = Heavy_Fog }, // Azumarill on Challenge Beach
            new(SWSH) { Species = 549, Level = 26, Location = 170, Weather = Intense_Sun }, // Lilligant on Challenge Beach
            new(SWSH) { Species = 845, Level = 24, Location = 170, Weather = Normal | Overcast | Raining | Intense_Sun | Sandstorm | Heavy_Fog }, // Cramorant on Challenge Beach
            new(SWSH) { Species = 055, Level = 27, Location = 170, Ability = 2, Weather = Thunderstorm | Intense_Sun }, // Golduck on Challenge Beach
            new(SWSH) { Species = 702, Level = 25, Location = 170, Weather = Normal | Stormy | Sandstorm }, // Dedenne on Challenge Beach
          //new(SWSH) { Species = 113, Level = 27, Location = -1 }, // Chansey
            new(SWSH) { Species = 405, Level = 36, Location = 170, Weather = Thunderstorm }, // Luxray on Challenge Beach
            new(SWSH) { Species = 099, Level = 26, Location = 170, Weather = Normal | Raining | Intense_Sun | Sandstorm }, // Kingler on Challenge Beach
            new(SWSH) { Species = 121, Level = 26, Location = 170, Weather = All_IoA }, // Starmie on Challenge Beach
            new(SWSH) { Species = 748, Level = 26, Location = 170, Weather = Overcast | Stormy | Heavy_Fog }, // Toxapex on Challenge Beach
            new(SWSH) { Species = 224, Level = 45, Location = 170, Weather = Normal | Intense_Sun | Sandstorm }, // Octillery on Challenge Beach
            new(SWSH) { Species = 171, Level = 42, Location = 170, Weather = Thunderstorm | Heavy_Fog }, // Lanturn on Challenge Beach
            new EncounterStatic8S(SWSH) { Species = 342, Level = 42, Locations = new[] {170, 180}, Weather = Overcast }, // Crawdaunt on Challenge Beach, Training Lowlands
            new(SWSH) { Species = 593, Level = 42, Location = 170, Weather = Overcast | Raining | Heavy_Fog }, // Jellicent on Challenge Beach
            new(SWSH) { Species = 593, Level = 42, Location = 178, Weather = Overcast | Heavy_Fog }, // Jellicent in Loop Lagoon
            new(SWSH) { Species = 091, Level = 42, Location = 170, Weather = Raining | Heavy_Fog }, // Cloyster on Challenge Beach
            new(SWSH) { Species = 130, Level = 50, Location = 170, Weather = Normal | Raining | Intense_Sun | Sandstorm }, // Gyarados on Challenge Beach
            new(SWSH) { Species = 062, Level = 36, Location = 172, Weather = All_IoA }, // Poliwrath in Brawlers’ Cave
            new(SWSH) { Species = 294, Level = 26, Location = 172, Weather = All_IoA }, // Loudred in Brawlers’ Cave
            new(SWSH) { Species = 528, Level = 26, Location = 172, Weather = All_IoA }, // Swoobat in Brawlers’ Cave
            new(SWSH) { Species = 621, Level = 36, Location = 172, Weather = All_IoA }, // Druddigon in Brawlers’ Cave
            new(SWSH) { Species = 055, Level = 26, Location = 172, Weather = All_IoA }, // Golduck in Brawlers’ Cave
            new(SWSH) { Species = 526, Level = 42, Location = 172, Weather = All_IoA }, // Gigalith in Brawlers’ Cave
            new(SWSH) { Species = 620, Level = 28, Location = 174, Weather = Normal }, // Mienshao on Challenge Road
            new(SWSH) { Species = 625, Level = 36, Location = 174, Weather = Overcast }, // Bisharp on Challenge Road
            new EncounterStatic8S(SH) { Species = 454, Level = 26, Locations = new[] {172, 174, 180}, Weather = Stormy }, // Toxicroak on Challenge Road, Brawlers’ Cave, Training Lowlands
            new EncounterStatic8S(SW) { Species = 560, Level = 26, Locations = new[] {172, 174, 180}, Weather = Stormy }, // Scrafty on Challenge Road, Brawlers’ Cave, Training Lowlands
            new(SWSH) { Species = 758, Level = 28, Location = 174, Gender = 1, Weather = Intense_Sun }, // Salazzle on Challenge Road
            new EncounterStatic8S(SWSH) { Species = 558, Level = 26, Locations = new[] {172, 174, 180}, Weather = Sandstorm }, // Crustle on Challenge Road, Brawlers’ Cave, Training Lowlands
            new(SWSH) { Species = 475, Level = 32, Location = 174, Weather = Heavy_Fog }, // Gallade on Challenge Road
            new(SWSH) { Species = 745, Level = 32, Location = 174, Weather = Normal }, // Lycanroc on Challenge Road
            new(SWSH) { Species = 745, Level = 32, Location = 174, Form = 01, Weather = Overcast }, // Lycanroc-1 on Challenge Road
            new(SWSH) { Species = 212, Level = 40, Location = 174, Weather = Sandstorm }, // Scizor on Challenge Road
            new(  SH) { Species = 214, Level = 26, Location = 174, Weather = Intense_Sun }, // Heracross on Challenge Road
            new EncounterStatic8S(SW) { Species = 744, Level = 22, Locations = new[] {172,  174}, Weather = Normal | Overcast }, // Rockruff on Challenge Road, Brawlers' Cave
            new EncounterStatic8S(SH) { Species = 744, Level = 22, Locations = new[] {172,  174}, Weather = Normal | Overcast | Intense_Sun | Heavy_Fog }, // Rockruff on Challenge Road, Brawlers' Cave
            new(SW  ) { Species = 127, Level = 26, Location = 174, Weather = Intense_Sun }, // Pinsir on Challenge Road
            new(SWSH) { Species = 227, Level = 26, Location = 174, Weather = Normal | Raining | Intense_Sun | Sandstorm }, // Skarmory on Challenge Road
            new(SWSH) { Species = 227, Level = 26, Location = 180, Weather = Sandstorm }, // Skarmory in the Training Lowlands
            new(SWSH) { Species = 426, Level = 26, Location = 174, Weather = Heavy_Fog }, // Drifblim on Challenge Road
            new(  SH) { Species = 630, Level = 26, Location = 174, Weather = Overcast }, // Mandibuzz on Challenge Road
            new(SW  ) { Species = 628, Level = 26, Location = 174, Weather = Overcast }, // Braviary on Challenge Road
            new(SWSH) { Species = 082, Level = 26, Location = 174, Weather = Thunderstorm }, // Magneton on Challenge Road
            new(SWSH) { Species = 558, Level = 28, Location = 176, Weather = All_IoA }, // Crustle in Courageous Cavern
          //new(SWSH) { Species = 526, Level = 42, Location = -1 }, // Gigalith
            new(SWSH) { Species = 768, Level = 32, Location = 176, Weather = All_IoA }, // Golisopod in Courageous Cavern
            new(SWSH) { Species = 528, Level = 28, Location = 176, Weather = All_IoA }, // Swoobat in Courageous Cavern
            new(SWSH) { Species = 834, Level = 28, Location = 176, Weather = All_IoA }, // Drednaw in Courageous Cavern
            new(SWSH) { Species = 621, Level = 42, Location = 176, Weather = All_IoA }, // Druddigon in Courageous Cavern
          //new(SWSH) { Species = 113, Level = 30, Location = -1 }, // Chansey
            new(SWSH) { Species = 687, Level = 32, Location = 178, Weather = Overcast | Raining }, // Malamar in Loop Lagoon
            new(SWSH) { Species = 040, Level = 32, Location = 178, Weather = Heavy_Fog }, // Wigglytuff in Loop Lagoon
          //new(SWSH) { Species = 768, Level = 32, Location = -1 }, // Golisopod
            new(SWSH) { Species = 404, Level = 30, Location = 178, Weather = Thunderstorm }, // Luxio in Loop Lagoon
            new(SWSH) { Species = 834, Level = 30, Location = 178, Weather = Normal | Intense_Sun | Sandstorm }, // Drednaw in Loop Lagoon
          //new(SWSH) { Species = 558, Level = 30, Location = -1 }, // Crustle
            new(SWSH) { Species = 871, Level = 22, Location = 178, Weather = Normal | Stormy | Heavy_Fog }, // Pincurchin in Loop Lagoon
            new(SWSH) { Species = 748, Level = 26, Location = 178, Weather = Overcast | Raining | Heavy_Fog }, // Toxapex in Loop Lagoon
            new(SWSH) { Species = 853, Level = 32, Location = 178, Weather = Normal | Overcast | Intense_Sun | Sandstorm | Heavy_Fog }, // Grapploct in Loop Lagoon
            new(SWSH) { Species = 770, Level = 32, Location = 178, Weather = Overcast | Sandstorm | Heavy_Fog }, // Palossand in Loop Lagoon
            new(SWSH) { Species = 065, Level = 50, Location = 178, Weather = Normal | Raining | Sandstorm }, // Alakazam in Loop Lagoon
            new(SWSH) { Species = 065, Level = 50, Location = 190, Weather = Heavy_Fog }, // Alakazam in the Insular Sea
            new(SWSH) { Species = 571, Level = 50, Location = 178, Weather = Overcast | Heavy_Fog }, // Zoroark in Loop Lagoon
            new(SWSH) { Species = 462, Level = 50, Location = 178, Weather = Thunderstorm }, // Magnezone in Loop Lagoon
            new(SWSH) { Species = 744, Level = 40, Location = 178 }, // Rockruff in Loop Lagoon
            new(SWSH) { Species = 636, Level = 40, Location = 178, Weather = Intense_Sun | Sandstorm }, // Larvesta in Loop Lagoon
            new(SWSH) { Species = 279, Level = 42, Location = 178, Weather = Raining }, // Pelipper in Loop Lagoon
            new(SWSH) { Species = 405, Level = 50, Location = 178, Weather = Thunderstorm }, // Luxray in Loop Lagoon
            new(SWSH) { Species = 663, Level = 50, Location = 178, Weather = Intense_Sun }, // Talonflame in Loop Lagoon
            new(SWSH) { Species = 508, Level = 42, Location = 180 }, // Stoutland in the Training Lowlands
            new(SWSH) { Species = 625, Level = 36, Location = 180, Weather = Overcast }, // Bisharp in the Training Lowlands
            new(SWSH) { Species = 405, Level = 36, Location = 180, Weather = Thunderstorm }, // Luxray in the Training Lowlands
            new(SWSH) { Species = 663, Level = 36, Location = 180, Weather = Intense_Sun }, // Talonflame in the Training Lowlands
            new(SWSH) { Species = 040, Level = 30, Location = 180, Weather = Heavy_Fog }, // Wigglytuff in the Training Lowlands
            new(SWSH) { Species = 099, Level = 28, Location = 180, Weather = Normal | Overcast | Stormy | Intense_Sun | Sandstorm }, // Kingler in the Training Lowlands
            new(SWSH) { Species = 115, Level = 32, Location = 180, Weather = Normal | Overcast }, // Kangaskhan in the Training Lowlands
            new(SWSH) { Species = 123, Level = 28, Location = 180, Weather = Normal | Intense_Sun }, // Scyther in the Training Lowlands
            new(SWSH) { Species = 404, Level = 28, Location = 180, Weather = Thunderstorm }, // Luxio in the Training Lowlands
            new(SWSH) { Species = 764, Level = 28, Location = 180, Weather = Heavy_Fog }, // Comfey in the Training Lowlands
            new(SWSH) { Species = 452, Level = 28, Location = 180, Weather = Overcast | Intense_Sun }, // Drapion in the Training Lowlands
            new(SWSH) { Species = 279, Level = 28, Location = 180, Weather = Raining }, // Pelipper in the Training Lowlands
            new(SW  ) { Species = 127, Level = 28, Location = 180, Weather = Normal | Intense_Sun }, // Pinsir in the Training Lowlands
            new(  SH) { Species = 214, Level = 28, Location = 180, Weather = Normal | Intense_Sun }, // Heracross in the Training Lowlands
            new(SWSH) { Species = 528, Level = 28, Location = 180, Weather = Overcast }, // Swoobat in the Training Lowlands
            new(SWSH) { Species = 241, Level = 28, Location = 180, Weather = Normal | Overcast | Intense_Sun }, // Miltank in the Training Lowlands
            new(SWSH) { Species = 082, Level = 28, Location = 180, Weather = Thunderstorm }, // Magneton in the Training Lowlands
            new(SWSH) { Species = 662, Level = 28, Location = 180, Weather = Intense_Sun }, // Fletchinder in the Training Lowlands
            new(SWSH) { Species = 128, Level = 28, Location = 180, Weather = All_IoA }, // Tauros in the Training Lowlands
            new(SWSH) { Species = 687, Level = 28, Location = 180, Weather = Overcast | Raining }, // Malamar in the Training Lowlands
            new EncounterStatic8S(SWSH) { Species = 507, Level = 28, Locations = new[] {174, 180}, Weather = Normal | Heavy_Fog }, // Herdier on Challenge Road and in the Training Lowlands
            new(SWSH) { Species = 549, Level = 28, Location = 180, Weather = Intense_Sun }, // Lilligant in the Training Lowlands
            new(SWSH) { Species = 426, Level = 28, Location = 180, Weather = Heavy_Fog }, // Drifblim in the Training Lowlands
            new(SWSH) { Species = 055, Level = 26, Location = 180, Weather = Normal | Overcast | Stormy | Intense_Sun }, // Golduck in the Training Lowlands
            new(SWSH) { Species = 184, Level = 26, Location = 180, Weather = Heavy_Fog }, // Azumarill in the Training Lowlands
            new(SWSH) { Species = 617, Level = 36, Location = 180, Weather = Thunderstorm }, // Accelgor in the Training Lowlands
            new(SWSH) { Species = 212, Level = 42, Location = 180, Weather = Sandstorm }, // Scizor in the Training Lowlands
            new(SWSH) { Species = 589, Level = 36, Location = 180, Weather = Sandstorm }, // Escavalier in the Training Lowlands
            new(SWSH) { Species = 616, Level = 26, Location = 180, Weather = Raining }, // Shelmet in the Training Lowlands
            new(SWSH) { Species = 588, Level = 26, Location = 180, Weather = Overcast }, // Karrablast in the Training Lowlands
            new(SWSH) { Species = 553, Level = 50, Location = 184, Weather = Overcast | Stormy }, // Krookodile in the Potbottom Desert
            new(SWSH) { Species = 464, Level = 50, Location = 184, Weather = Normal | Intense_Sun | Sandstorm | Heavy_Fog }, // Rhyperior in the Potbottom Desert
            new(SWSH) { Species = 105, Level = 42, Location = 184, Weather = Normal | Intense_Sun | Heavy_Fog }, // Marowak in the Potbottom Desert
            new(SWSH) { Species = 552, Level = 42, Location = 184, Weather = Overcast | Stormy }, // Krokorok in the Potbottom Desert
            new(SWSH) { Species = 112, Level = 42, Location = 184, Weather = Normal | Intense_Sun | Sandstorm }, // Rhydon in the Potbottom Desert
            new(SWSH) { Species = 324, Level = 42, Location = 184, Weather = Intense_Sun | Heavy_Fog }, // Torkoal in the Potbottom Desert
            new(SWSH) { Species = 844, Level = 42, Location = 184, Weather = Normal | Intense_Sun | Heavy_Fog }, // Sandaconda in the Potbottom Desert
            new(SWSH) { Species = 637, Level = 50, Location = 184, Weather = Intense_Sun }, // Volcarona in the Potbottom Desert
            new(SWSH) { Species = 028, Level = 42, Location = 184, Weather = Sandstorm }, // Sandslash in the Potbottom Desert
            new(SW  ) { Species = 628, Level = 42, Location = 184, Weather = All_IoA }, // Braviary in the Potbottom Desert
            new(  SH) { Species = 630, Level = 42, Location = 184, Weather = All_IoA }, // Mandibuzz in the Potbottom Desert
            new(SWSH) { Species = 103, Level = 50, Location = 190, Weather = Normal | Raining | Intense_Sun | Sandstorm }, // Exeggutor in the Insular Sea
          //new(SWSH) { Species = 132, Level = 50, Location = 186, FlawlessIVCount = 3 }, // Ditto in the Workout Sea -- collision with wild Ditto in the same area
          //new(SWSH) { Species = 242, Level = 50, Location = -1 }, // Blissey
            new(SWSH) { Species = 571, Level = 50, Location = 190, Weather = Overcast }, // Zoroark in the Insular Sea
            new(SWSH) { Species = 462, Level = 50, Location = 190, Weather = Thunderstorm }, // Magnezone in the Insular Sea
            new(SWSH) { Species = 637, Level = 50, Location = 190, Weather = Intense_Sun }, // Volcarona in the Insular Sea
            new(SWSH) { Species = 279, Level = 45, Location = 190, Weather = Overcast | Stormy }, // Pelipper in the Insular Sea
            new EncounterStatic8S(SWSH) { Species = 764, Level = 50, Locations = new[] {190, 194}, Weather = Heavy_Fog }, // Comfey in the Insular Sea, Honeycalm Sea
            new(SWSH) { Species = 549, Level = 45, Location = 194, Weather = Normal | Intense_Sun | Sandstorm }, // Lilligant on Honeycalm Island
            new(SWSH) { Species = 415, Level = 40, Location = 194, Weather = Overcast | Stormy }, // Combee on Honeycalm Island
            new EncounterStatic8S(SWSH) { Species = 587, Level = 20, Locations = new[] {166, 168}, Weather = All_IoA }, // Emolga in the Soothing Wetlands, Forest of Focus
            new EncounterStatic8S(SWSH) { Species = 847, Level = 42, Locations = new[] {166, 170, 176, 180}, Weather = Normal | Stormy | Intense_Sun | Sandstorm | Heavy_Fog }, // Barraskewda in the Soothing Wetlands, Challenge Beach, Courageous Cavern, Training Lowlands
            new(SWSH) { Species = 073, Level = 42, Location = 176, Weather = All_IoA }, // Tentacruel in Courageous Cavern
            new EncounterStatic8S(SWSH) { Species = 340, Level = 42, Locations = new[] {172, 176}, Weather = All_IoA}, // Whiscash in Courageous Cavern, Brawlers' Cave
            new(SWSH) { Species = 340, Level = 42, Location = 168, Weather = Normal | Overcast | Intense_Sun | Sandstorm }, // Whiscash in the Forest of Focus
          //new(SWSH) { Species = 479, Level = 50, Location = 186, FlawlessIVCount = 3 }, // Rotom in the Workout Sea -- collision with subsequent static Rotom
            new(SWSH) { Species = 479, Level = 50, Location = 186, Moves = new[] {435,506,268}, Form = 01, Weather = Normal | Stormy | Intense_Sun | Sandstorm | Heavy_Fog }, // Rotom-1 in the Workout Sea
            new(SWSH) { Species = 479, Level = 50, Location = 186, Moves = new[] {435,506,268}, Form = 02, Weather = Normal | Stormy | Intense_Sun | Sandstorm | Heavy_Fog }, // Rotom-2 in the Workout Sea
            new(SWSH) { Species = 479, Level = 50, Location = 186, Moves = new[] {435,506,268}, Form = 03, Weather = Normal | Stormy | Intense_Sun | Sandstorm | Heavy_Fog }, // Rotom-3 in the Workout Sea
            new(SWSH) { Species = 479, Level = 50, Location = 186, Moves = new[] {435,506,268}, Form = 04, Weather = Normal | Stormy | Intense_Sun | Sandstorm | Heavy_Fog }, // Rotom-4 in the Workout Sea
            new(SWSH) { Species = 479, Level = 50, Location = 186, Moves = new[] {435,506,268}, Form = 05, Weather = Normal | Stormy | Intense_Sun | Sandstorm | Heavy_Fog }, // Rotom-5 in the Workout Sea
            new(SWSH) { Species = 230, Level = 60, Location = 192, Weather = Thunderstorm }, // Kingdra in the Honeycalm Sea
            new(SWSH) { Species = 117, Level = 45, Location = 192, Weather = Normal | Overcast | Raining | Intense_Sun | Sandstorm | Heavy_Fog }, // Seadra in the Honeycalm Sea
            new(SWSH) { Species = 321, Level = 80, Location = 186, Weather = All_IoA }, // Wailord in the Workout Sea
            #endregion

            #region R2 Static Encounters
            new EncounterStatic8S(SWSH) { Species = 144, Level = 70, Locations = new[] {208, 210, 212, 214}, Moves = new[] {821,542,427,375}, FlawlessIVCount = 3, Shiny = Never, Ability = 1, Form = 01, Weather = All_CT }, // Articuno-1 in the Crown Tundra
            new EncounterStatic8S(SWSH) { Species = 145, Level = 70, Locations = new[] {122, 124, 126, 128, 130}, Moves = new[] {823,065,179,116}, FlawlessIVCount = 3, Shiny = Never, Ability = 1, Form = 01, Weather = All }, // Zapdos-1 in a Wild Area
            new EncounterStatic8S(SWSH) { Species = 146, Level = 70, Locations = new[] {164, 166, 170, 178, 186, 188, 190, 192}, Moves = new[] {822,542,389,417}, FlawlessIVCount = 3, Shiny = Never, Ability = 1, Form = 01, Weather = All_IoA }, // Moltres-1 on the Isle of Armor
            new(SWSH) { Species = 377, Level = 70, Location = 236, ScriptedNoMarks = true, Moves = new[] {276,444,359,174}, FlawlessIVCount = 3, Ability = 1 }, // Regirock
            new(SWSH) { Species = 378, Level = 70, Location = 238, ScriptedNoMarks = true, Moves = new[] {058,192,133,196}, FlawlessIVCount = 3, Ability = 1 }, // Regice
            new(SWSH) { Species = 379, Level = 70, Location = 240, ScriptedNoMarks = true, Moves = new[] {484,430,334,451}, FlawlessIVCount = 3, Ability = 1 }, // Registeel
            new(SWSH) { Species = 894, Level = 70, Location = 242, ScriptedNoMarks = true, Moves = new[] {819,527,245,393}, FlawlessIVCount = 3, Ability = 1 }, // Regieleki
            new(SWSH) { Species = 895, Level = 70, Location = 242, ScriptedNoMarks = true, Moves = new[] {820,337,359,673}, FlawlessIVCount = 3, Ability = 1 }, // Regidrago
            new(SWSH) { Species = 486, Level =100, Location = 210, ScriptedNoMarks = true, Moves = new[] {416,428,359,462}, FlawlessIVCount = 3, Ability = 1, DynamaxLevel = 10 }, // Regigigas in the Giant’s Bed
            new(SWSH) { Species = 638, Level = 70, Location = 226, FlawlessIVCount = 3, Ability = 1, Weather = All_CT }, // Cobalion at the Frigid Sea
            new(SWSH) { Species = 639, Level = 70, Location = 232, FlawlessIVCount = 3, Ability = 1, Weather = Overcast }, // Terrakion in Lakeside Cavern
            new(SWSH) { Species = 640, Level = 70, Location = 210, FlawlessIVCount = 3, Ability = 1, Weather = All_CT }, // Virizion at Giant's Bed
            new(SWSH) { Species = 647, Level = 65, Location = 230, Moves = new[] {548,533,014,056}, FlawlessIVCount = 3, Shiny = Never, Ability = 1, Form = 01, Fateful = true, Weather = All_CT }, // Keldeo-1 at Ballimere Lake
          //new(SWSH) { Species = 896, Level = 75, Location = -1, Moves = new[] {556,037,419,023}, FlawlessIVCount = 3, Shiny = Never, Ability = 1 }, // Glastrier
          //new(SWSH) { Species = 897, Level = 75, Location = -1, Moves = new[] {247,037,506,024}, FlawlessIVCount = 3, Shiny = Never, Ability = 1 }, // Spectrier
            new(SWSH) { Species = 898, Level = 80, Location = 220, Moves = new[] {202,094,473,505}, FlawlessIVCount = 3, Shiny = Never, Ability = 1, ScriptedNoMarks = true }, // Calyrex

            // suspected unused or uncatchable
          //new(SWSH) { Species = 803, Level = 60, Location = -1, FlawlessIVCount = 3, Shiny = Never, Ability = 1 }, // Poipole
          //new(SWSH) { Species = 789, Level = 60, Location = -1, FlawlessIVCount = 3, Shiny = Never, Ability = 1 }, // Cosmog
          //new(SWSH) { Species = 494, Level = 70, Location = -1, FlawlessIVCount = 3, Shiny = Never, Ability = 1 }, // Victini

            new(SWSH) { Species = 473, Level = 65, Location = 204, Weather = Normal | Overcast | Intense_Sun | Icy }, // Mamoswine on Slippery Slope
            new(SWSH) { Species = 698, Level = 60, Location = 204, Weather = Normal | Overcast | Icy }, // Amaura on Slippery Slope
            new(SWSH) { Species = 698, Level = 60, Location = 208, Weather = Normal | Icy }, // Amaura in Frostpoint Field
            new EncounterStatic8S(SWSH) { Species = 832, Level = 63, Locations = new[] {204, 208}, Weather = Normal | Intense_Sun }, // Dubwool on Slippery Slope, Frostpoint Field
            new(SWSH) { Species = 832, Level = 63, Location = 210 }, // Dubwool in the Giant’s Bed
            new(SWSH) { Species = 333, Level = 60, Location = 204, Weather = Overcast | Snowing }, // Swablu on Slippery Slope
            new EncounterStatic8S(SWSH) { Species = 333, Level = 60, Locations = new[] {208, 210}, Weather = Overcast }, // Swablu in Frostpoint Field, Giant’s Bed
            new(SWSH) { Species = 124, Level = 62, Location = 204, Weather = Icy | Heavy_Fog }, // Jynx on Slippery Slope
            new(SWSH) { Species = 124, Level = 62, Location = 208, Weather = Snowing | Heavy_Fog }, // Jynx in Frostpoint Field
            new EncounterStatic8S(SWSH) { Species = 615, Level = 62, Locations = new[] {204, 208, 210}, Weather = Icy }, // Cryogonal on Slippery Slope, Frostpoint Field, Giant’s Bed
            new EncounterStatic8S(SWSH) { Species = 778, Level = 62, Locations = new[] {204, 208, 210, 212}, Weather = Heavy_Fog }, // Mimikyu on Slippery Slope, Frostpoint Field, Giant’s Bed, Old Cemetery
            new(SWSH) { Species = 460, Level = 65, Location = 204, Weather = Snowing }, // Abomasnow on Slippery Slope
            new(SWSH) { Species = 460, Level = 65, Location = 208, Weather = Normal | Overcast | Intense_Sun | Snowing }, // Abomasnow in Frostpoint Field
            new EncounterStatic8S(SWSH) { Species = 584, Level = 65, Locations = new[] {208, 210}, Weather = Icy }, // Vanilluxe in Frostpoint Field, Giant’s Bed
            new(SWSH) { Species = 872, Level = 60, Location = 204, Weather = Normal | Heavy_Fog }, // Snom on Slippery Slope
            new EncounterStatic8S(SW  ) { Species = 576, Level = 65, Locations = new[] {204, 208}, Weather = Heavy_Fog }, // Gothitelle on Slippery Slope, Frostpoint Field
            new(SWSH) { Species = 133, Level = 60, Location = 208, Weather = Snowstorm }, // Eevee in Frostpoint Field
            new(SWSH) { Species = 029, Level = 60, Location = 208, Weather = Normal | Overcast | Intense_Sun | Icy }, // Nidoran♀ in Frostpoint Field
            new(SWSH) { Species = 029, Level = 60, Location = 210, Weather = Normal | Stormy | Intense_Sun }, // Nidoran♀ in the Giant’s Bed
            new(SWSH) { Species = 032, Level = 60, Location = 208, Weather = Normal | Overcast | Intense_Sun | Icy }, // Nidoran♂ in Frostpoint Field
            new EncounterStatic8S(SWSH) { Species = 531, Level = 62, Locations = new[] {204, 208}, Weather = Normal | Overcast | Intense_Sun | Icy | Heavy_Fog }, // Audino on Slippery Slope, Frostpoint Field
            new EncounterStatic8S(SWSH) { Species = 531, Level = 62, Locations = new[] {210, 222, 230}, Weather = All_CT }, // Audino in the Giant’s Bed, Giant’s Foot
            new(SWSH) { Species = 531, Level = 62, Location = 230, Weather = Normal | Overcast | Stormy | Intense_Sun | Snowing }, // Audino at Ballimere Lake
            new(SWSH) { Species = 359, Level = 62, Location = 208, Weather = Snowstorm }, // Absol in Frostpoint Field
            new EncounterStatic8S(SWSH) { Species = 858, Level = 65, Locations = new[] {208, 210}, Weather = Heavy_Fog }, // Hatterene in Frostpoint Field, Giant’s Bed
            new(SWSH) { Species = 461, Level = 63, Location = 208, Weather = Overcast }, // Weavile in Frostpoint Field
            new EncounterStatic8S(  SH) { Species = 579, Level = 65, Locations = new[] {204, 208}, Weather = Heavy_Fog }, // Reuniclus on Slippery Slope, Frostpoint Field
            new(SWSH) { Species = 857, Level = 62, Location = 204, Weather = Heavy_Fog }, // Hattrem on Slippery Slope
            new(SWSH) { Species = 478, Level = 63, Location = 204, Weather = Snowstorm }, // Froslass on Slippery Slope
            new(SWSH) { Species = 362, Level = 63, Location = 204, Weather = Snowstorm }, // Glalie on Slippery Slope
            new(SWSH) { Species = 467, Level = 65, Location = 204, Weather = Intense_Sun }, // Magmortar on Slippery Slope
            new EncounterStatic8S(SWSH) { Species = 126, Level = 62, Locations = new[] {204, 210}, Weather = Intense_Sun }, // Magmar on Slippery Slope, Giant’s Bed
            new(SWSH) { Species = 143, Level = 65, Location = 204, Weather = Normal | Intense_Sun }, // Snorlax on Slippery Slope
            new(SWSH) { Species = 143, Level = 65, Location = 208, Weather = Normal | Intense_Sun | Overcast }, // Snorlax in Frostpoint Field
            new(  SH) { Species = 143, Level = 65, Location = 210, Weather = All_CT }, // Snorlax in the Giant’s Bed
            new EncounterStatic8S(SWSH) { Species = 861, Level = 65, Locations = new[] {204, 210}, Weather = Heavy_Fog }, // Grimmsnarl on Slippery Slope, Giant’s Bed
            new(SWSH) { Species = 709, Level = 63, Location = 212, Weather = Overcast }, // Trevenant in the Old Cemetery
            new(  SH) { Species = 078, Level = 67, Location = 212, Form = 01, Weather = Heavy_Fog }, // Rapidash-1 in the Old Cemetery
            new(SWSH) { Species = 142, Level = 65, Location = 210, Weather = All_CT }, // Aerodactyl in the Giant’s Bed
            new(SWSH) { Species = 133, Level = 60, Location = 210 }, // Eevee in the Giant’s Bed
            new EncounterStatic8S(SWSH) { Species = 437, Level = 63, Locations = new[] {210, 214}, Weather = All_CT }, // Bronzong in the Giant’s Bed, Snowslide Slope
            new(SWSH) { Species = 470, Level = 63, Location = 210 }, // Leafeon in the Giant’s Bed
            new EncounterStatic8S(SWSH) { Species = 034, Level = 65, Locations = new[] {208, 210}, Weather = No_Sun_Sand }, // Nidoking in Frostpoint Field, Giant’s Bed
            new EncounterStatic8S(SWSH) { Species = 030, Level = 63, Locations = new[] {208, 210}, Weather = All_CT }, // Nidorina in Frostpoint Field, in the Giant’s Bed
            new(SWSH) { Species = 033, Level = 63, Location = 210, Weather = All_CT }, // Nidorino in the Giant’s Bed
            new(SWSH) { Species = 534, Level = 65, Location = 210 }, // Conkeldurr in the Giant’s Bed
            new(SW  ) { Species = 874, Level = 63, Location = 210, Weather = All_CT }, // Stonjourner in the Giant’s Bed
            new(SWSH) { Species = 820, Level = 65, Location = 210, Weather = No_Sun_Sand }, // Greedent in the Giant’s Bed
            new(SWSH) { Species = 031, Level = 65, Location = 210, Weather = Normal | Overcast | Raining | Intense_Sun | Icy | Heavy_Fog }, // Nidoqueen in the Giant’s Bed
            new(SWSH) { Species = 862, Level = 65, Location = 210, Weather = Overcast | Raining }, // Obstagoon in the Giant’s Bed
            new(SWSH) { Species = 609, Level = 65, Location = 210, Weather = Overcast | Heavy_Fog }, // Chandelure in the Giant’s Bed
            new(SWSH) { Species = 752, Level = 65, Location = 210, Weather = Stormy }, // Araquanid in the Giant’s Bed
            new EncounterStatic8S(SWSH) { Species = 334, Level = 65, Locations = new[] {210, 218, 222, 226, 230}, Weather = Overcast }, // Altaria in the Giant’s Bed, Path to the Peak, Giant’s Foot, Frigid Sea, Ballimere Lake
            new(SWSH) { Species = 134, Level = 63, Location = 210, Weather = Raining }, // Vaporeon in the Giant’s Bed
            new(SWSH) { Species = 596, Level = 63, Location = 210, Weather = Thunderstorm }, // Galvantula in the Giant’s Bed
            new(SWSH) { Species = 466, Level = 65, Location = 210, Weather = Thunderstorm }, // Electivire in the Giant’s Bed
            new(SWSH) { Species = 135, Level = 63, Location = 210, Weather = Thunderstorm }, // Jolteon in the Giant’s Bed
            new(SWSH) { Species = 125, Level = 63, Location = 210, Weather = Thunderstorm }, // Electabuzz in the Giant’s Bed
            new(SWSH) { Species = 467, Level = 63, Location = 210, Weather = Intense_Sun }, // Magmortar in the Giant’s Bed
            new(SWSH) { Species = 631, Level = 63, Location = 210, Weather = Intense_Sun }, // Heatmor in the Giant’s Bed
            new(SWSH) { Species = 632, Level = 63, Location = 210, Weather = Intense_Sun }, // Durant in the Giant’s Bed
            new(SWSH) { Species = 136, Level = 63, Location = 210, Weather = Intense_Sun }, // Flareon in the Giant’s Bed
            new(SWSH) { Species = 197, Level = 63, Location = 210, Weather = Overcast }, // Umbreon in the Giant’s Bed
            new(SWSH) { Species = 196, Level = 63, Location = 210, Weather = Snowing }, // Espeon in the Giant’s Bed
            new EncounterStatic8S(SWSH) { Species = 478, Level = 65, Locations = new[] {210, 212, 214}, Weather = Icy }, // Froslass in the Giant’s Bed, Old Cemetery, Snowslide Slope
            new(SWSH) { Species = 478, Level = 65, Location = 216, Weather = Overcast }, // Froslass in the Tunnel to the Top
            new EncounterStatic8S(SWSH) { Species = 362, Level = 65, Locations = new[] {210, 214}, Weather = Icy }, // Glalie in the Giant’s Bed, Snowslide Slope
            new(SWSH) { Species = 359, Level = 65, Location = 210, Weather = Snowstorm }, // Absol in the Giant’s Bed
            new(SWSH) { Species = 471, Level = 63, Location = 210, Weather = Snowstorm }, // Glaceon in the Giant’s Bed
            new(SWSH) { Species = 700, Level = 63, Location = 210, Weather = Heavy_Fog }, // Sylveon in the Giant’s Bed
            new(SWSH) { Species = 036, Level = 63, Location = 210, Weather = Heavy_Fog }, // Clefable in the Giant’s Bed
            new(SWSH) { Species = 340, Level = 65, Location = 210, Weather = All_CT }, // Whiscash in the Giant’s Bed
            new EncounterStatic8S(SWSH) { Species = 130, Level = 67, Locations = new[] {210, 230}, Weather = Normal | Overcast | Stormy | Intense_Sun | Icy }, // Gyarados in the Giant’s Bed, Ballimere Lake
            new EncounterStatic8S(SWSH) { Species = 350, Level = 67, Locations = new[] {210, 230}, Weather = Heavy_Fog }, // Milotic in the Giant’s Bed, Ballimere Lake
            new EncounterStatic8S(SWSH) { Species = 855, Level = 63, Locations = new[] {210, 212}, Weather = Normal | Stormy | Intense_Sun | Snowstorm  | Heavy_Fog }, // Polteageist in the Giant’s Bed, Old Cemetery
            new EncounterStatic8S(SWSH) { Species = 887, Level = 65, Locations = new[] {210, 212}, Weather = Normal | Overcast | Stormy | Intense_Sun | Snowing }, // Dragapult in the Giant’s Bed, Old Cemetery
            new(SWSH) { Species = 872, Level = 62, Location = 214, Weather = Normal | Overcast }, // Snom on Snowslide Slope
            new(SWSH) { Species = 698, Level = 62, Location = 214, Weather = Normal | Overcast | Heavy_Fog }, // Amaura on Snowslide Slope
            new(SWSH) { Species = 621, Level = 65, Location = 214, Weather = Normal | Intense_Sun }, // Druddigon on Snowslide Slope
            new(SWSH) { Species = 621, Level = 65, Location = 218 }, // Druddigon on the Path to the Peak
            new(SWSH) { Species = 621, Level = 65, Location = 216, Weather = Overcast }, // Druddigon in the Tunnel to the Top
            new(SWSH) { Species = 832, Level = 65, Location = 214, Weather = Normal | Intense_Sun }, // Dubwool on Snowslide Slope
            new EncounterStatic8S(SWSH) { Species = 375, Level = 63, Locations = new[] {214, 216 }, Weather = Normal | Overcast | Intense_Sun | Icy | Heavy_Fog }, // Metang on Snowslide Slope, Tunnel to the Top
            new(SWSH) { Species = 699, Level = 65, Location = 214, Weather = Normal | Overcast | Icy | Heavy_Fog }, // Aurorus on Snowslide Slope
            new(SWSH) { Species = 376, Level = 68, Location = 214, Weather = Normal | Overcast | Intense_Sun | Icy | Heavy_Fog }, // Metagross on Snowslide Slope
            new(SWSH) { Species = 461, Level = 65, Location = 214, Weather = Overcast }, // Weavile on Snowslide Slope
            new(SWSH) { Species = 709, Level = 65, Location = 214, Weather = Overcast }, // Trevenant on Snowslide Slope
            new EncounterStatic8S(SWSH) { Species = 126, Level = 65, Locations = new[] {214, 230}, Weather = Intense_Sun }, // Magmar on Snowslide Slope, Ballimere Lake
            new(SWSH) { Species = 467, Level = 67, Location = 214, Weather = Intense_Sun }, // Magmortar on Snowslide Slope
            new(SWSH) { Species = 362, Level = 67, Location = 214, Weather = Icy }, // Glalie on Snowslide Slope
            new EncounterStatic8S(SWSH) { Species = 615, Level = 65, Locations = new[] {214, 222, 230}, Weather = Icy }, // Cryogonal on Snowslide Slope, Giant’s Foot, Ballimere Lake
            new EncounterStatic8S(SWSH) { Species = 614, Level = 67, Locations = new[] {214, 226, 228}, Weather = Icy }, // Beartic on Snowslide Slope, Frigid Sea, Three-Point Pass
            new(SWSH) { Species = 584, Level = 67, Location = 214, Weather = Icy }, // Vanilluxe on Snowslide Slope
            new(SWSH) { Species = 584, Level = 67, Location = 230, Weather = Snowing }, // Vanilluxe at Ballimere Lake
            new EncounterStatic8S(SWSH) { Species = 359, Level = 67, Locations = new[] {214, 218, 222}, Weather = Snowstorm }, // Absol on Snowslide Slope, Path to the Peak, Giant’s Foot
            new(SW  ) { Species = 555, Level = 67, Location = 214, Form = 02, Weather = Snowstorm }, // Darmanitan-2 on Snowslide Slope
            new(SWSH) { Species = 861, Level = 67, Location = 214, Weather = Heavy_Fog }, // Grimmsnarl on Snowslide Slope
            new EncounterStatic8S(SWSH) { Species = 778, Level = 65, Locations = new[] {214, 222, 230}, Weather = Heavy_Fog }, // Mimikyu on Snowslide Slope, Giant’s Foot, Ballimere Lake
            new(SWSH) { Species = 036, Level = 65, Location = 214, Weather = Heavy_Fog }, // Clefable on Snowslide Slope
            new(SWSH) { Species = 036, Level = 65, Location = 216, Weather = Overcast }, // Clefable in the Tunnel to the Top
            new EncounterStatic8S(SWSH) { Species = 041, Level = 63, Locations = new[] {216, 224}, Weather = Overcast }, // Zubat in the Tunnel to the Top, Roaring-Sea Caves
            new EncounterStatic8S(SWSH) { Species = 042, Level = 65, Locations = new[] {216, 224}, Weather = Overcast }, // Golbat in the Tunnel to the Top, Roaring-Sea Caves
            new(SW  ) { Species = 371, Level = 65, Location = 216, Weather = Overcast }, // Bagon in the Tunnel to the Top
            new(  SH) { Species = 443, Level = 65, Location = 216, Weather = Overcast }, // Gible in the Tunnel to the Top
            new(SW  ) { Species = 373, Level = 68, Location = 216, Weather = Overcast }, // Salamence in the Tunnel to the Top
            new(SW  ) { Species = 373, Level = 68, Location = 218, Weather = Intense_Sun }, // Salamence on the Path to the Peak
            new(  SH) { Species = 445, Level = 68, Location = 216, Weather = Overcast }, // Garchomp in the Tunnel to the Top
            new(  SH) { Species = 445, Level = 68, Location = 218, Weather = Intense_Sun }, // Garchomp on the Path to the Peak
            new(SWSH) { Species = 703, Level = 65, Location = 216, Weather = Overcast }, // Carbink in the Tunnel to the Top
            new(SWSH) { Species = 873, Level = 65, Location = 218, Weather = Normal | Overcast | Intense_Sun | Icy | Heavy_Fog }, // Frosmoth on the Path to the Peak
            new(SWSH) { Species = 851, Level = 67, Location = 222, Weather = Normal | Intense_Sun }, // Centiskorch at the Giant’s Foot
            new(SWSH) { Species = 879, Level = 67, Location = 222, Weather = Overcast | Stormy | Icy | Heavy_Fog }, // Copperajah at the Giant’s Foot
            new(SWSH) { Species = 534, Level = 67, Location = 222 }, // Conkeldurr at the Giant’s Foot
            new(  SH) { Species = 140, Level = 63, Location = 222, Weather = All_CT }, // Kabuto at the Giant’s Foot
            new(SW  ) { Species = 138, Level = 63, Location = 222, Weather = All_CT }, // Omanyte at the Giant’s Foot
            new(SWSH) { Species = 566, Level = 63, Location = 222, Weather = All_CT }, // Archen at the Giant’s Foot
            new EncounterStatic8S(SWSH) { Species = 344, Level = 65, Locations = new[] {210, 222}, Weather = Overcast | Stormy | Intense_Sun | Icy | Heavy_Fog }, // Claydol in the Giant’s Bed, Giant’s Foot
            new EncounterStatic8S(SWSH) { Species = 437, Level = 65, Locations = new[] {208, 222}, Weather = Normal | Overcast }, // Bronzong at the Giant’s Foot, Frostpoint Field
            new EncounterStatic8S(SWSH) { Species = 752, Level = 67, Locations = new[] {222, 230}, Weather = Raining }, // Araquanid at Ballimere Lake, Giant’s Foot
            new EncounterStatic8S(SWSH) { Species = 125, Level = 65, Locations = new[] {222, 230}, Weather = Thunderstorm }, // Electabuzz at the Giant’s Foot, Ballimere Lake
            new EncounterStatic8S(SWSH) { Species = 466, Level = 68, Locations = new[] {226, 228, 230}, Weather = Thunderstorm }, // Electivire at the Frigid Sea, Three-Point Pass, Ballimere Lake
            new(SWSH) { Species = 126, Level = 65, Location = 222, Weather = Intense_Sun }, // Magmar at the Giant’s Foot
            new EncounterStatic8S(SWSH) { Species = 467, Level = 68, Locations = new[] {226, 230}, Weather = Intense_Sun }, // Magmortar at the Frigid Sea, Ballimere Lake
          //new(SWSH) { Species = 567, Level = 67, Location = -1 }, // Archeops
            new(SW  ) { Species = 635, Level = 68, Location = 224, Weather = Overcast }, // Hydreigon in Roaring-Sea Caves
            new(  SH) { Species = 248, Level = 68, Location = 224, Weather = Overcast }, // Tyranitar in Roaring-Sea Caves
            new(SWSH) { Species = 448, Level = 67, Location = 224, Weather = Overcast }, // Lucario in Roaring-Sea Caves
            new(SWSH) { Species = 363, Level = 63, Location = 226, Weather = No_Sun_Sand }, // Spheal at the Frigid Sea
            new(SWSH) { Species = 364, Level = 65, Location = 226, Weather = No_Sun_Sand }, // Sealeo at the Frigid Sea
            new(SWSH) { Species = 564, Level = 63, Location = 226, Weather = Normal | Overcast | Stormy | Heavy_Fog }, // Tirtouga at the Frigid Sea
            new(SWSH) { Species = 713, Level = 65, Location = 226, Weather = No_Sun_Sand }, // Avalugg at the Frigid Sea
            new EncounterStatic8S(SWSH) { Species = 858, Level = 67, Locations = new[] {226, 230}, Weather = Heavy_Fog }, // Hatterene at the Frigid Sea, Ballimere Lake
            new(SWSH) { Species = 365, Level = 68, Location = 226, Weather = Normal | Overcast | Icy | Heavy_Fog }, // Walrein at the Frigid Sea
            new(SWSH) { Species = 565, Level = 67, Location = 226, Weather = Normal | Stormy }, // Carracosta at the Frigid Sea
            new(SWSH) { Species = 871, Level = 65, Location = 226, Weather = Thunderstorm }, // Pincurchin at the Frigid Sea
            new(  SH) { Species = 875, Level = 65, Location = 226, Weather = No_Sun_Sand }, // Eiscue at the Frigid Sea
            new EncounterStatic8S(SWSH) { Species = 623, Level = 65, Locations = new[] {226, 228}, Weather = All_CT }, // Golurk at the Frigid Sea, Three-Point Pass
            new(SWSH) { Species = 887, Level = 68, Location = 228, Weather = Normal | Overcast | Raining | Intense_Sun | Icy | Heavy_Fog }, // Dragapult in Three-Point Pass
            new(  SH) { Species = 141, Level = 68, Location = 224, Weather = Overcast }, // Kabutops in Roaring-Sea Caves
            new(SW  ) { Species = 139, Level = 68, Location = 224, Weather = Overcast }, // Omastar in Roaring-Sea Caves
            new(SWSH) { Species = 823, Level = 68, Location = 230, Weather = Normal | Snowing }, // Corviknight at Ballimere Lake
            new(SWSH) { Species = 862, Level = 68, Location = 230, Weather = Overcast }, // Obstagoon at Ballimere Lake
            new(SWSH) { Species = 715, Level = 67, Location = 230, Weather = Overcast | Raining }, // Noivern at Ballimere Lake
            new(SWSH) { Species = 715, Level = 67, Location = 232, Weather = Overcast }, // Noivern in Lakeside Cave
            new(SWSH) { Species = 547, Level = 65, Location = 230, Weather = Normal | Raining }, // Whimsicott at Ballimere Lake
            new(SWSH) { Species = 836, Level = 67, Location = 230, Weather = Normal | Stormy | Snowing | Heavy_Fog }, // Boltund at Ballimere Lake
            new(SWSH) { Species = 830, Level = 65, Location = 230, Weather = Raining | Intense_Sun }, // Eldegoss at Ballimere Lake
            new(SW  ) { Species = 876, Level = 65, Location = 230, Weather = Normal | Heavy_Fog }, // Indeedee at Ballimere Lake
            new(  SH) { Species = 876, Level = 65, Location = 230, Form = 01, Weather = Normal | Heavy_Fog }, // Indeedee-1 at Ballimere Lake
            new(SWSH) { Species = 696, Level = 63, Location = 230, Weather = All_Ballimere }, // Tyrunt at Ballimere Lake
            new(SWSH) { Species = 213, Level = 65, Location = 230, Weather = Normal | Intense_Sun }, // Shuckle at Ballimere Lake
            new(SWSH) { Species = 820, Level = 68, Location = 230, Weather = All_Ballimere }, // Greedent at Ballimere Lake
            new(SWSH) { Species = 820, Level = 68, Location = 234, Weather = Normal | Overcast | Stormy | Intense_Sun | Icy | Heavy_Fog }, // Greedent at Dyna Tree Hill
            new(SWSH) { Species = 877, Level = 65, Location = 230, Weather = Overcast | Thunderstorm }, // Morpeko at Ballimere Lake
            new(SWSH) { Species = 596, Level = 67, Location = 230, Weather = Thunderstorm }, // Galvantula at Ballimere Lake
            new(SWSH) { Species = 839, Level = 68, Location = 230, Weather = Normal | Thunderstorm | Intense_Sun | Snowing | Heavy_Fog }, // Coalossal at Ballimere Lake
            new(SWSH) { Species = 839, Level = 68, Location = 232, Weather = Overcast }, // Coalossal in Lakeside Cave
            new(SWSH) { Species = 697, Level = 69, Location = 230, Weather = All_Ballimere }, // Tyrantrum at Ballimere Lake
            new(SWSH) { Species = 531, Level = 65, Location = 230, Weather = Heavy_Fog }, // Audino at Ballimere Lake
            new(SWSH) { Species = 304, Level = 63, Location = 230, Weather = All_Ballimere }, // Aron at Ballimere Lake
            new(SWSH) { Species = 149, Level = 70, Location = 230, Weather = Raining | Thunderstorm }, // Dragonite at Ballimere Lake
            new(SWSH) { Species = 306, Level = 68, Location = 232, Weather = Overcast }, // Aggron in Lakeside Cave
            new(SWSH) { Species = 598, Level = 67, Location = 232, Weather = Overcast }, // Ferrothorn in Lakeside Cave
            new(SWSH) { Species = 305, Level = 63, Location = 232, Weather = Overcast }, // Lairon in Lakeside Cave
            new(SWSH) { Species = 348, Level = 67, Location = 230, Weather = Raining | Thunderstorm }, // Armaldo at Ballimere Lake
            new(SWSH) { Species = 347, Level = 63, Location = 230, Weather = All_Ballimere }, // Anorith at Ballimere Lake
            new(SWSH) { Species = 369, Level = 65, Location = 230, Weather = Normal | Overcast | Stormy | Intense_Sun | Snowing }, // Relicanth at Ballimere Lake
            new(SWSH) { Species = 147, Level = 63, Location = 230, Weather = Raining | Heavy_Fog }, // Dratini at Ballimere Lake
            new(SWSH) { Species = 148, Level = 65, Location = 230, Weather = Thunderstorm | Heavy_Fog }, // Dragonair at Ballimere Lake
            new(SWSH) { Species = 442, Level = 72, Location = 230, FlawlessIVCount = 3, Ability = 4, Weather = All_CT }, // Spiritomb at Ballimere Lake
            #endregion
        };

        private const string tradeSWSH = "tradeswsh";
        private static readonly string[][] TradeSWSH = Util.GetLanguageStrings10(tradeSWSH, "zh2");
        private static readonly string[] TradeOT_R1 = { string.Empty, "チホコ", "Regina", "Régiona", "Regionalia", "Regine", string.Empty, "Tatiana", "지민", "易蒂", "易蒂" };
        private static readonly int[] TradeIVs = {15, 15, 15, 15, 15, 15};

        private static readonly EncounterTrade8[] TradeGift_Regular =
        {
            new(SWSH, 052,18,08,000,04,5) { Ability = 2, TID7 = 263455, IVs = TradeIVs, DynamaxLevel = 1, OTGender = 0, Gender = 0, Nature = Nature.Timid, Relearn = new[] {387,000,000,000}   }, // Meowth
            new(SWSH, 819,10,01,044,01,2) { Ability = 1, TID7 = 648753, IVs = TradeIVs, DynamaxLevel = 1, OTGender = 1, Gender = 0, Nature = Nature.Mild,                                      }, // Skwovet
            new(SWSH, 546,23,11,000,09,5) { Ability = 1, TID7 = 101154, IVs = TradeIVs, DynamaxLevel = 1, OTGender = 1, Gender = 1, Nature = Nature.Modest,                                    }, // Cottonee
            new(SWSH, 175,25,02,010,10,6) { Ability = 2, TID7 = 109591, IVs = TradeIVs, DynamaxLevel = 1, OTGender = 1, Gender = 0, Nature = Nature.Timid, Relearn = new[] {791,000,000,000}   }, // Togepi
            new(SW  , 856,30,09,859,08,3) { Ability = 2, TID7 = 101101, IVs = TradeIVs, DynamaxLevel = 1, OTGender = 0, Gender = 1, Nature = Nature.Quiet,                                     }, // Hatenna
            new(  SH, 859,30,43,000,07,6) { Ability = 1, TID7 = 256081, IVs = TradeIVs, DynamaxLevel = 1, OTGender = 0, Gender = 0, Nature = Nature.Brave, Relearn = new[] {252,000,000,000}   }, // Impidimp
            new(SWSH, 562,35,16,310,15,5) { Ability = 1, TID7 = 102534, IVs = TradeIVs, DynamaxLevel = 2, OTGender = 1, Gender = 0, Nature = Nature.Bold, Relearn = new[] {261,000,000,000}    }, // Yamask
            new(SW  , 538,37,17,129,20,7) { Ability = 2, TID7 = 768945, IVs = TradeIVs, DynamaxLevel = 2, OTGender = 0, Gender = 0, Nature = Nature.Adamant,                                   }, // Throh
            new(  SH, 539,37,17,129,14,6) { Ability = 1, TID7 = 881426, IVs = TradeIVs, DynamaxLevel = 2, OTGender = 0, Gender = 0, Nature = Nature.Adamant,                                   }, // Sawk
            new(SWSH, 122,40,56,000,12,4) { Ability = 1, TID7 = 891846, IVs = TradeIVs, DynamaxLevel = 1, OTGender = 0, Gender = 0, Nature = Nature.Calm,                                      }, // Mr. Mime
            new(SWSH, 884,50,15,038,06,2) { Ability = 2, TID7 = 101141, IVs = TradeIVs, DynamaxLevel = 3, OTGender = 0, Gender = 0, Nature = Nature.Adamant, Relearn = new[] {400,000,000,000} }, // Duraludon
        };

        private static readonly EncounterTrade8[] TradeGift_R1 =
        {
            new(SWSH, 052,15,01,033,04,2) { Ability = 4, TID7 = 101141, FlawlessIVCount = 3, DynamaxLevel = 5, OTGender = 1, Shiny = Random, IsNicknamed = false, Relearn = new[] {387,000,000,000}               }, // Meowth
            new(SW  , 083,15,01,013,10,2) { Ability = 4, TID7 = 101141, FlawlessIVCount = 3, DynamaxLevel = 5, OTGender = 1, Shiny = Random, IsNicknamed = false, Relearn = new[] {098,000,000,000}               }, // Farfetch’d
            new(  SH, 222,15,01,069,12,2) { Ability = 4, TID7 = 101141, FlawlessIVCount = 3, DynamaxLevel = 5, OTGender = 1, Shiny = Random, IsNicknamed = false, Relearn = new[] {457,000,000,000}               }, // Corsola
            new(  SH, 077,15,01,047,06,2) { Ability = 4, TID7 = 101141, FlawlessIVCount = 3, DynamaxLevel = 5, OTGender = 1, Shiny = Random, IsNicknamed = false, Relearn = new[] {234,000,000,000}               }, // Ponyta
            new(SWSH, 122,15,01,005,04,2) { Ability = 4, TID7 = 101141, FlawlessIVCount = 3, DynamaxLevel = 5, OTGender = 1, Shiny = Random, IsNicknamed = false, Relearn = new[] {252,000,000,000}               }, // Mr. Mime
            new(SW  , 554,15,01,040,12,2) { Ability = 4, TID7 = 101141, FlawlessIVCount = 3, DynamaxLevel = 5, OTGender = 1, Shiny = Random, IsNicknamed = false, Relearn = new[] {326,000,000,000}               }, // Darumaka
            new(SWSH, 263,15,01,045,04,2) { Ability = 4, TID7 = 101141, FlawlessIVCount = 3, DynamaxLevel = 5, OTGender = 1, Shiny = Random, IsNicknamed = false, Relearn = new[] {245,000,000,000}               }, // Zigzagoon
            new(SWSH, 618,15,01,050,05,2) { Ability = 4, TID7 = 101141, FlawlessIVCount = 3, DynamaxLevel = 5, OTGender = 1, Shiny = Random, IsNicknamed = false, Relearn = new[] {281,000,000,000}               }, // Stunfisk
            new(SWSH, 110,15,01,040,12,2) { Ability =-1, TID7 = 101141, FlawlessIVCount = 3, DynamaxLevel = 5, OTGender = 1, Shiny = Random, IsNicknamed = false, Relearn = new[] {220,000,000,000}               }, // Weezing
            new(SWSH, 103,15,01,038,06,2) {              TID7 = 101141, FlawlessIVCount = 3, DynamaxLevel = 5, OTGender = 1, Shiny = Random, IsNicknamed = false, Relearn = new[] {246,000,000,000}, Form = 1     }, // Exeggutor-1
            new(SWSH, 105,15,01,038,06,2) {              TID7 = 101141, FlawlessIVCount = 3, DynamaxLevel = 5, OTGender = 1, Shiny = Random, IsNicknamed = false, Relearn = new[] {174,000,000,000}, Form = 1     }, // Marowak-1
        };

        internal static readonly EncounterTrade8[] TradeGift_SWSH = ArrayUtil.ConcatAll(TradeGift_Regular, TradeGift_R1);

        internal static readonly EncounterStatic[] StaticSW = ArrayUtil.ConcatAll<EncounterStatic>(Nest_Common, Nest_SW, Nest_SH, Dist_DLC2, Dist_DLC1, Dist_Base, GetEncounters(Crystal_SWSH, SW), DynAdv_SWSH, GetEncounters(Encounter_SWSH, SW));
        internal static readonly EncounterStatic[] StaticSH = ArrayUtil.ConcatAll<EncounterStatic>(Nest_Common, Nest_SW, Nest_SH, Dist_DLC2, Dist_DLC1, Dist_Base, GetEncounters(Crystal_SWSH, SH), DynAdv_SWSH, GetEncounters(Encounter_SWSH, SH));
    }
}
