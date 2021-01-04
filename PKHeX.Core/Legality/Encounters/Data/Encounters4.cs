using System.Linq;
using static PKHeX.Core.EncounterUtil;
using static PKHeX.Core.GameVersion;

namespace PKHeX.Core
{
    /// <summary>
    /// Generation 4 Encounters
    /// </summary>
    internal static class Encounters4
    {
        internal static readonly EncounterArea4[] SlotsD = EncounterArea4.GetAreas(Get("d", "da"), D);
        internal static readonly EncounterArea4[] SlotsP = EncounterArea4.GetAreas(Get("p", "pe"), P);
        internal static readonly EncounterArea4[] SlotsPt = EncounterArea4.GetAreas(Get("pt", "pt"), Pt);
        internal static readonly EncounterArea4[] SlotsHG = EncounterArea4.GetAreas(Get("hg", "hg"), HG);
        internal static readonly EncounterArea4[] SlotsSS = EncounterArea4.GetAreas(Get("ss", "ss"), SS);
        private static byte[][] Get(string resource, string ident) => BinLinker.Unpack(Util.GetBinaryResource($"encounter_{resource}.pkl"), ident);

        static Encounters4()
        {
            MarkEncounterTradeStrings(TradeGift_DPPt, TradeDPPt);
            MarkEncounterTradeStrings(TradeGift_HGSS, TradeHGSS);
        }

        #region Pokéwalker Encounter
        // all pkm are in Poke Ball and have a met location of "PokeWalker"
        private static readonly EncounterStatic4Pokewalker[] Encounter_PokeWalker =
        {
            // Some pkm has a pre-level move, an egg move or even a special move, it might be also available via HM/TM/Tutor
            // Johto/Kanto Courses
            new(084, 1, 08), // Doduo
            new(115, 1, 08), // Kangaskhan
            new(029, 1, 05), // Nidoran♀
            new(032, 0, 05), // Nidoran♂
            new(016, 0, 05), // Pidgey
            new(161, 1, 05), // Sentret
            new(202, 1, 15), // Wobbuffet
            new(069, 1, 08), // Bellsprout
            new(046, 1, 06), // Paras
            new(048, 0, 06), // Venonat
            new(021, 0, 05), // Spearow
            new(043, 1, 05), // Oddish
            new(095, 0, 09), // Onix
            new(240, 0, 09) { Moves = new[]{241},}, // Magby: Sunny Day
            new(066, 1, 07), // Machop
            new(077, 1, 07), // Ponyta
            new(074, 1, 08) { Moves = new[]{189},}, // Geodude: Mud-Slap
            new(163, 1, 06), // Hoothoot
            new(054, 1, 10), // Psyduck
            new(120, 2, 10), // Staryu
            new(060, 0, 08), // Poliwag
            new(079, 0, 08), // Slowpoke
            new(191, 1, 06), // Sunkern
            new(194, 0, 06), // Wooper
            new(081, 2, 11), // Magnemite
            new(239, 0, 11) { Moves = new[]{009},}, // Elekid: Thunder Punch
            new(081, 2, 08), // Magnemite
            new(198, 1, 11), // Murkrow
            new(019, 1, 07), // Rattata
            new(163, 1, 07), // Hoothoot
            new(092, 1, 15) { Moves = new[]{194},}, // Gastly: Destiny Bond
            new(238, 1, 12) { Moves = new[]{419},}, // Smoochum: Avalanche
            new(092, 1, 10), // Gastly
            new(095, 0, 10), // Onix
            new(041, 0, 08), // Zubat
            new(066, 0, 08), // Machop
            new(060, 1, 15) { Moves = new[]{187} }, // Poliwag: Belly Drum
            new(147, 1, 10), // Dratini
            new(090, 1, 12), // Shellder
            new(098, 0, 12) { Moves = new[]{152} }, // Krabby: Crabhammer
            new(072, 1, 09), // Tentacool
            new(118, 1, 09), // Goldeen
            new(063, 1, 15), // Abra
            new(100, 2, 15), // Voltorb
            new(088, 0, 13), // Grimer
            new(109, 1, 13) { Moves = new[]{120} }, // Koffing: Self-Destruct
            new(019, 1, 16), // Rattata
            new(162, 0, 15), // Furret
            // Hoenn Courses
            new(264, 1, 30), // Linoone
            new(300, 1, 30), // Skitty
            new(313, 0, 25), // Volbeat
            new(314, 1, 25), // Illumise
            new(263, 1, 17), // Zigzagoon
            new(265, 1, 15), // Wurmple
            new(298, 1, 20), // Azurill
            new(320, 1, 31), // Wailmer
            new(116, 1, 20), // Horsea
            new(318, 1, 26), // Carvanha
            new(118, 1, 22) { Moves = new[]{401} }, // Goldeen: Aqua Tail
            new(129, 1, 15), // Magikarp
            new(218, 1, 31), // Slugma
            new(307, 0, 32), // Meditite
            new(111, 0, 25), // Rhyhorn
            new(228, 0, 27), // Houndour
            new(074, 0, 29), // Geodude
            new(077, 1, 19), // Ponyta
            new(351, 1, 30), // Castform
            new(352, 0, 30), // Kecleon
            new(203, 1, 28), // Girafarig
            new(234, 1, 28), // Stantler
            new(044, 1, 14), // Gloom
            new(070, 0, 13), // Weepinbell
            new(105, 1, 30) { Moves = new[]{037} }, // Marowak: Thrash
            new(128, 0, 30), // Tauros
            new(042, 0, 33), // Golbat
            new(177, 1, 24), // Natu
            new(066, 0, 13) { Moves = new[]{418} }, // Machop: Bullet Punch
            new(092, 1, 15), // Gastly
            // Sinnoh Courses
            new(415, 0, 30), // Combee
            new(439, 0, 29), // Mime Jr.
            new(403, 1, 33), // Shinx
            new(406, 0, 30), // Budew
            new(399, 1, 13), // Bidoof
            new(401, 0, 15), // Kricketot
            new(361, 1, 28), // Snorunt
            new(459, 0, 31) { Moves = new[]{452} }, // Snover: Wood Hammer
            new(215, 0, 28) { Moves = new[]{306} }, // Sneasel: Crash Claw
            new(436, 2, 20), // Bronzor
            new(179, 1, 15), // Mareep
            new(220, 1, 16), // Swinub
            new(357, 1, 35), // Tropius
            new(438, 0, 30), // Bonsly
            new(114, 1, 30), // Tangela
            new(400, 1, 30), // Bibarel
            new(102, 1, 17), // Exeggcute
            new(179, 0, 19), // Mareep
            new(200, 1, 32) { Moves = new[]{194},}, // Misdreavus: Destiny Bond
            new(433, 0, 22) { Moves = new[]{105},}, // Chingling: Recover
            new(093, 0, 25), // Haunter
            new(418, 0, 28) { Moves = new[]{226},}, // Buizel: Baton Pass
            new(170, 1, 17), // Chinchou
            new(223, 1, 19), // Remoraid
            new(422, 1, 30) { Moves = new[]{243},}, // Shellos: Mirror Coat
            new(456, 1, 26), // Finneon
            new(086, 1, 27), // Seel
            new(129, 1, 30), // Magikarp
            new(054, 1, 22) { Moves = new[]{281},}, // Psyduck: Yawn
            new(090, 0, 20), // Shellder
            new(025, 1, 30), // Pikachu
            new(417, 1, 33) { Moves = new[]{175},}, // Pachirisu: Flail
            new(035, 1, 31), // Clefairy
            new(039, 1, 30), // Jigglypuff
            new(183, 1, 25), // Marill
            new(187, 1, 25), // Hoppip
            new(442, 0, 31), // Spiritomb
            new(446, 0, 33), // Munchlax
            new(349, 0, 30), // Feebas
            new(433, 1, 26), // Chingling
            new(042, 0, 33), // Golbat
            new(164, 1, 30), // Noctowl
            // Special Courses
            new(120, 2, 18) { Moves = new[]{113} }, // Staryu: Light Screen
            new(224, 1, 19) { Moves = new[]{324} }, // Octillery: Signal Beam
            new(116, 0, 15), // Horsea
            new(222, 1, 16), // Corsola
            new(170, 1, 12), // Chinchou
            new(223, 0, 14), // Remoraid
            new(035, 0, 08) { Moves = new[]{236} }, // Clefairy: Moonlight
            new(039, 0, 10), // Jigglypuff
            new(041, 0, 09), // Zubat
            new(163, 1, 06), // Hoothoot
            new(074, 0, 05), // Geodude
            new(095, 1, 05) { Moves = new[]{088} }, // Onix: Rock Throw
            new(025, 0, 15) { Moves = new[]{019} }, // Pikachu: Fly
            new(025, 1, 14) { Moves = new[]{057} }, // Pikachu: Surf
            new(025, 1, 12) { Moves = new[]{344, 252} }, // Pikachu: Volt Tackle, Fake Out
            new(025, 0, 13) { Moves = new[]{175} }, // Pikachu: Flail
            new(025, 0, 10), // Pikachu
            new(025, 1, 10), // Pikachu
            new(302, 1, 15), // Sableye
            new(441, 0, 15), // Chatot
            new(025, 1, 10), // Pikachu
            new(453, 0, 10), // Croagunk
            new(417, 0, 05), // Pachirisu
            new(427, 1, 05), // Buneary
            new(133, 0, 10), // Eevee
            new(255, 0, 10), // Torchic
            new(061, 1, 15) { Moves = new[]{003} }, // Poliwhirl: Double Slap
            new(279, 0, 15), // Pelipper
            new(025, 1, 08), // Pikachu
            new(052, 0, 10), // Meowth
            new(374, 2, 05) { Moves = new[]{428,334,442} }, // Beldum: Zen Headbutt, Iron Defense & Iron Head.
            new(446, 0, 05) { Moves = new[]{120} }, // Munchlax: Self-Destruct
            new(116, 0, 05) { Moves = new[]{330} }, // Horsea: Muddy Water
            new(355, 0, 05) { Moves = new[]{286} }, // Duskull: Imprison
            new(129, 0, 05) { Moves = new[]{340} }, // Magikarp: Bounce
            new(436, 2, 05) { Moves = new[]{433} }, // Bronzor: Trick Room
            new(239, 0, 05) { Moves = new[]{9}}, // Elekid: Thunder Punch (can be tutored)
            new(240, 0, 05) { Moves = new[]{7}}, // Magby: Fire Punch (can be tutored)
            new(238, 1, 05) { Moves = new[]{8}}, // Smoochum: Ice Punch (can be tutored)
            new(440, 1, 05) { Moves = new[]{215}}, // Happiny: Heal Bell
            new(173, 1, 05) { Moves = new[]{118}}, // Cleffa: Metronome
            new(174, 0, 05) { Moves = new[]{273}}, // Igglybuff: Wish
        };
        #endregion
        #region Static Encounter/Gift Tables
        private static readonly EncounterStatic4[] Encounter_DPPt =
        {
            // Starters
            new(DP) { Gift = true, Species = 387, Level = 5, Location = 076, TypeEncounter = EncounterType.Starter_Fossil_Gift_DP }, // Turtwig @ Lake Verity
            new(DP) { Gift = true, Species = 390, Level = 5, Location = 076, TypeEncounter = EncounterType.Starter_Fossil_Gift_DP }, // Chimchar
            new(DP) { Gift = true, Species = 393, Level = 5, Location = 076, TypeEncounter = EncounterType.Starter_Fossil_Gift_DP }, // Piplup
            new(Pt) { Gift = true, Species = 387, Level = 5, Location = 016, TypeEncounter = EncounterType.Starter_Fossil_Gift_Pt_DPTrio }, // Turtwig @ Route 201
            new(Pt) { Gift = true, Species = 390, Level = 5, Location = 016, TypeEncounter = EncounterType.Starter_Fossil_Gift_Pt_DPTrio }, // Chimchar
            new(Pt) { Gift = true, Species = 393, Level = 5, Location = 016, TypeEncounter = EncounterType.Starter_Fossil_Gift_Pt_DPTrio }, // Piplup

            // Fossil @ Mining Museum
            new(DP) { Gift = true, Species = 138, Level = 20, Location = 094, TypeEncounter = EncounterType.Starter_Fossil_Gift_DP }, // Omanyte
            new(DP) { Gift = true, Species = 140, Level = 20, Location = 094, TypeEncounter = EncounterType.Starter_Fossil_Gift_DP }, // Kabuto
            new(DP) { Gift = true, Species = 142, Level = 20, Location = 094, TypeEncounter = EncounterType.Starter_Fossil_Gift_DP }, // Aerodactyl
            new(DP) { Gift = true, Species = 345, Level = 20, Location = 094, TypeEncounter = EncounterType.Starter_Fossil_Gift_DP }, // Lileep
            new(DP) { Gift = true, Species = 347, Level = 20, Location = 094, TypeEncounter = EncounterType.Starter_Fossil_Gift_DP }, // Anorith
            new(DP) { Gift = true, Species = 408, Level = 20, Location = 094, TypeEncounter = EncounterType.Starter_Fossil_Gift_DP }, // Cranidos
            new(DP) { Gift = true, Species = 410, Level = 20, Location = 094, TypeEncounter = EncounterType.Starter_Fossil_Gift_DP }, // Shieldon
            new(Pt) { Gift = true, Species = 138, Level = 20, Location = 094, TypeEncounter = EncounterType.Starter_Fossil_Gift_Pt_DPTrio }, // Omanyte
            new(Pt) { Gift = true, Species = 140, Level = 20, Location = 094, TypeEncounter = EncounterType.Starter_Fossil_Gift_Pt_DPTrio }, // Kabuto
            new(Pt) { Gift = true, Species = 142, Level = 20, Location = 094, TypeEncounter = EncounterType.Starter_Fossil_Gift_Pt_DPTrio }, // Aerodactyl
            new(Pt) { Gift = true, Species = 345, Level = 20, Location = 094, TypeEncounter = EncounterType.Starter_Fossil_Gift_Pt_DPTrio }, // Lileep
            new(Pt) { Gift = true, Species = 347, Level = 20, Location = 094, TypeEncounter = EncounterType.Starter_Fossil_Gift_Pt_DPTrio }, // Anorith
            new(Pt) { Gift = true, Species = 408, Level = 20, Location = 094, TypeEncounter = EncounterType.Starter_Fossil_Gift_Pt_DPTrio }, // Cranidos
            new(Pt) { Gift = true, Species = 410, Level = 20, Location = 094, TypeEncounter = EncounterType.Starter_Fossil_Gift_Pt_DPTrio }, // Shieldon

            // Gift
            new(DP) { Gift = true, Species = 133, Level = 05, Location = 010, TypeEncounter = EncounterType.Starter_Fossil_Gift_DP, }, // Eevee @ Hearthome City
            new(Pt) { Gift = true, Species = 133, Level = 20, Location = 010, TypeEncounter = EncounterType.Starter_Fossil_Gift_Pt_DPTrio, }, // Eevee @ Hearthome City
            new(Pt) { Gift = true, Species = 137, Level = 25, Location = 012, TypeEncounter = EncounterType.Starter_Fossil_Gift_Pt_DPTrio, }, // Porygon @ Veilstone City
            new(Pt) { Gift = true, Species = 175, Level = 01, EggLocation = 2011 }, // Togepi Egg from Cynthia
            new(DP) { Gift = true, Species = 440, Level = 01, EggLocation = 2009 }, // Happiny Egg from Traveling Man
            new(DPPt) { Gift = true, Species = 447, Level = 01, EggLocation = 2010, }, // Riolu Egg from Riley

            // Stationary
            new(DP) { Species = 425, Level = 22, Location = 47, }, // Drifloon @ Valley Windworks
            new(Pt) { Species = 425, Level = 15, Location = 47, }, // Drifloon @ Valley Windworks
            new(DP) { Species = 479, Level = 15, Location = 70, TypeEncounter = EncounterType.Building_EnigmaStone, }, // Rotom @ Old Chateau
            new(Pt) { Species = 479, Level = 20, Location = 70, TypeEncounter = EncounterType.Building_EnigmaStone, }, // Rotom @ Old Chateau
            new(DPPt) { Species = 442, Level = 25, Location = 24 }, // Spiritomb @ Route 209

            // Stationary Legendary
            new(Pt) { Species = 377, Level = 30, Location = 125, TypeEncounter = EncounterType.Cave_HallOfOrigin, }, // Regirock @ Rock Peak Ruins
            new(Pt) { Species = 378, Level = 30, Location = 124, TypeEncounter = EncounterType.Cave_HallOfOrigin, }, // Regice @ Iceberg Ruins
            new(Pt) { Species = 379, Level = 30, Location = 123, TypeEncounter = EncounterType.Cave_HallOfOrigin, }, // Registeel @ Iron Ruins
            new(DPPt) { Species = 480, Level = 50, Location = 089, TypeEncounter = EncounterType.Cave_HallOfOrigin, }, // Uxie @ Acuity Cavern
            new(DPPt) { Species = 482, Level = 50, Location = 088, TypeEncounter = EncounterType.Cave_HallOfOrigin, }, // Azelf @ Valor Cavern
            new(D ) { Species = 483, Level = 47, Location = 051, TypeEncounter = EncounterType.DialgaPalkia }, // Dialga @ Spear Pillar
            new( P) { Species = 484, Level = 47, Location = 051, TypeEncounter = EncounterType.DialgaPalkia }, // Palkia @ Spear Pillar
            new(Pt) { Species = 483, Level = 70, Location = 051, TypeEncounter = EncounterType.DialgaPalkia }, // Dialga @ Spear Pillar
            new(Pt) { Species = 484, Level = 70, Location = 051, TypeEncounter = EncounterType.DialgaPalkia }, // Palkia @ Spear Pillar
            new(DP) { Species = 485, Level = 70, Location = 084, TypeEncounter = EncounterType.Cave_HallOfOrigin }, // Heatran @ Stark Mountain
            new(Pt) { Species = 485, Level = 50, Location = 084, TypeEncounter = EncounterType.Cave_HallOfOrigin }, // Heatran @ Stark Mountain
            new(DP) { Species = 486, Level = 70, Location = 064, TypeEncounter = EncounterType.Cave_HallOfOrigin }, // Regigigas @ Snowpoint Temple
            new(Pt) { Species = 486, Level = 01, Location = 064, TypeEncounter = EncounterType.Cave_HallOfOrigin }, // Regigigas @ Snowpoint Temple
            new(DP) { Species = 487, Level = 70, Location = 062, TypeEncounter = EncounterType.Cave_HallOfOrigin, Form = 0, }, // Giratina @ Turnback Cave
            new(Pt) { Species = 487, Level = 47, Location = 062, TypeEncounter = EncounterType.Cave_HallOfOrigin, Form = 0, }, // Giratina @ Turnback Cave
            new(Pt) { Species = 487, Level = 47, Location = 117, TypeEncounter = EncounterType.DistortionWorld_Pt,Form = 1, HeldItem = 112 }, // Giratina @ Distortion World

            // Event
            new(DP) { Species = 491, Level = 40, Location = 079, TypeEncounter = EncounterType.TallGrass }, // Darkrai @ Newmoon Island (Unreleased in Diamond and Pearl)
            new(Pt) { Species = 491, Level = 50, Location = 079, TypeEncounter = EncounterType.TallGrass }, // Darkrai @ Newmoon Island
            new(Pt) { Species = 492, Form = 0, Level = 30, Location = 063, Fateful = true }, // Shaymin @ Flower Paradise
            new(DP) { Species = 492, Form = 0, Level = 30, Location = 063, Fateful = false }, // Shaymin @ Flower Paradise (Unreleased in Diamond and Pearl)
            new(DPPt) { Species = 493, Form = 0, Level = 80, Location = 086, TypeEncounter = EncounterType.Cave_HallOfOrigin }, // Arceus @ Hall of Origin (Unreleased)

            // Roamers
            new(DPPt) { Roaming = true, Species = 481, Level = 50, TypeEncounter = EncounterType.TallGrass | EncounterType.Surfing_Fishing  }, // Mesprit
            new(DPPt) { Roaming = true, Species = 488, Level = 50, TypeEncounter = EncounterType.TallGrass | EncounterType.Surfing_Fishing }, // Cresselia
            new(Pt) { Roaming = true, Species = 144, Level = 60, TypeEncounter = EncounterType.TallGrass | EncounterType.Surfing_Fishing }, // Articuno
            new(Pt) { Roaming = true, Species = 145, Level = 60, TypeEncounter = EncounterType.TallGrass | EncounterType.Surfing_Fishing }, // Zapdos
            new(Pt) { Roaming = true, Species = 146, Level = 60, TypeEncounter = EncounterType.TallGrass | EncounterType.Surfing_Fishing }, // Moltres
        };

        private static readonly EncounterStatic4[] Encounter_HGSS =
        {
            // Starters
            new(HGSS) { Gift = true, Species = 001, Level = 05, Location = 138, TypeEncounter = EncounterType.Starter_Fossil_Gift_Pt_DPTrio }, // Bulbasaur @ Pallet Town
            new(HGSS) { Gift = true, Species = 004, Level = 05, Location = 138, TypeEncounter = EncounterType.Starter_Fossil_Gift_Pt_DPTrio }, // Charmander
            new(HGSS) { Gift = true, Species = 007, Level = 05, Location = 138, TypeEncounter = EncounterType.Starter_Fossil_Gift_Pt_DPTrio }, // Squirtle
            new(HGSS) { Gift = true, Species = 152, Level = 05, Location = 126, TypeEncounter = EncounterType.Starter_Fossil_Gift_DP }, // Chikorita @ New Bark Town
            new(HGSS) { Gift = true, Species = 155, Level = 05, Location = 126, TypeEncounter = EncounterType.Starter_Fossil_Gift_DP }, // Cyndaquil
            new(HGSS) { Gift = true, Species = 158, Level = 05, Location = 126, TypeEncounter = EncounterType.Starter_Fossil_Gift_DP }, // Totodile
            new(HGSS) { Gift = true, Species = 252, Level = 05, Location = 148, TypeEncounter = EncounterType.Starter_Fossil_Gift_Pt_DPTrio }, // Treecko @ Saffron City
            new(HGSS) { Gift = true, Species = 255, Level = 05, Location = 148, TypeEncounter = EncounterType.Starter_Fossil_Gift_Pt_DPTrio }, // Torchic
            new(HGSS) { Gift = true, Species = 258, Level = 05, Location = 148, TypeEncounter = EncounterType.Starter_Fossil_Gift_Pt_DPTrio }, // Mudkip

            // Fossils @ Pewter City
            new(HGSS) { Gift = true, Species = 138, Level = 20, Location = 140, TypeEncounter = EncounterType.Starter_Fossil_Gift_Pt_DPTrio, }, // Omanyte
            new(HGSS) { Gift = true, Species = 140, Level = 20, Location = 140, TypeEncounter = EncounterType.Starter_Fossil_Gift_Pt_DPTrio, }, // Kabuto
            new(HGSS) { Gift = true, Species = 142, Level = 20, Location = 140, TypeEncounter = EncounterType.Starter_Fossil_Gift_Pt_DPTrio, }, // Aerodactyl
            new(HGSS) { Gift = true, Species = 345, Level = 20, Location = 140, TypeEncounter = EncounterType.Starter_Fossil_Gift_Pt_DPTrio, }, // Lileep
            new(HGSS) { Gift = true, Species = 347, Level = 20, Location = 140, TypeEncounter = EncounterType.Starter_Fossil_Gift_Pt_DPTrio, }, // Anorith
            new(HGSS) { Gift = true, Species = 408, Level = 20, Location = 140, TypeEncounter = EncounterType.Starter_Fossil_Gift_Pt_DPTrio, }, // Cranidos
            new(HGSS) { Gift = true, Species = 410, Level = 20, Location = 140, TypeEncounter = EncounterType.Starter_Fossil_Gift_Pt_DPTrio, }, // Shieldon

            // Gift
            new(HGSS) { Gift = true, Species = 072, Level = 15, Location = 130, TypeEncounter = EncounterType.Starter_Fossil_Gift_Pt_DPTrio }, // Tentacool @ Cianwood City
            new(HGSS) { Gift = true, Species = 133, Level = 05, Location = 131, TypeEncounter = EncounterType.Starter_Fossil_Gift_Pt_DPTrio }, // Eevee @ Goldenrod City
            new(HGSS) { Gift = true, Species = 147, Level = 15, Location = 222, TypeEncounter = EncounterType.Starter_Fossil_Gift_Pt_DPTrio, Moves = new[] {245} }, // Dratini @ Dragon's Den (ExtremeSpeed)
            new(HGSS) { Gift = true, Species = 236, Level = 10, Location = 216, TypeEncounter = EncounterType.Starter_Fossil_Gift_Pt_DPTrio, }, // Tyrogue @ Mt. Mortar
            new(HGSS) { Gift = true, Species = 175, Level = 01, EggLocation = 2013, Moves = new[] {326} }, // Togepi Egg from Mr. Pokemon (Extrasensory as Egg move)
            new(HGSS) { Gift = true, Species = 179, Level = 01, EggLocation = 2014, }, // Mareep Egg from Primo
            new(HGSS) { Gift = true, Species = 194, Level = 01, EggLocation = 2014, }, // Wooper Egg from Primo
            new(HGSS) { Gift = true, Species = 218, Level = 01, EggLocation = 2014, }, // Slugma Egg from Primo

            // Celadon City Game Corner
            new(HGSS) { Gift = true, Species = 122, Level = 15, Location = 144, TypeEncounter = EncounterType.Starter_Fossil_Gift_Pt_DPTrio }, // Mr. Mime
            new(HGSS) { Gift = true, Species = 133, Level = 15, Location = 144, TypeEncounter = EncounterType.Starter_Fossil_Gift_Pt_DPTrio }, // Eevee
            new(HGSS) { Gift = true, Species = 137, Level = 15, Location = 144, TypeEncounter = EncounterType.Starter_Fossil_Gift_Pt_DPTrio }, // Porygon

            // Goldenrod City Game Corner
            new(HGSS) { Gift = true, Species = 063, Level = 15, Location = 131, TypeEncounter = EncounterType.Starter_Fossil_Gift_Pt_DPTrio }, // Abra
            new(HG  ) { Gift = true, Species = 023, Level = 15, Location = 131, TypeEncounter = EncounterType.Starter_Fossil_Gift_Pt_DPTrio }, // Ekans
            new(  SS) { Gift = true, Species = 027, Level = 15, Location = 131, TypeEncounter = EncounterType.Starter_Fossil_Gift_Pt_DPTrio }, // Sandshrew
            new(HGSS) { Gift = true, Species = 147, Level = 15, Location = 131, TypeEncounter = EncounterType.Starter_Fossil_Gift_Pt_DPTrio }, // Dratini

            // Team Rocket HQ Trap Floor
            new(HGSS) { Species = 100, Level = 23, Location = 213, TypeEncounter = EncounterType.Building_EnigmaStone, }, // Voltorb
            new(HGSS) { Species = 074, Level = 21, Location = 213, TypeEncounter = EncounterType.Building_EnigmaStone, }, // Geodude
            new(HGSS) { Species = 109, Level = 21, Location = 213, TypeEncounter = EncounterType.Building_EnigmaStone, }, // Koffing

            // Stationary
            new(HGSS) { Species = 130, Level = 30, Location = 135, TypeEncounter = EncounterType.Surfing_Fishing, Shiny = Shiny.Always }, // Gyarados @ Lake of Rage
            new(HGSS) { Species = 131, Level = 20, Location = 210, TypeEncounter = EncounterType.Surfing_Fishing, }, // Lapras @ Union Cave Friday Only
            new(HGSS) { Species = 101, Level = 23, Location = 213, TypeEncounter = EncounterType.Building_EnigmaStone, }, // Electrode @ Team Rocket HQ
            new(HGSS) { Species = 143, Level = 50, Location = 159, }, // Snorlax @ Route 11
            new(HGSS) { Species = 143, Level = 50, Location = 160, }, // Snorlax @ Route 12
            new(HGSS) { Species = 185, Level = 20, Location = 184, }, // Sudowoodo @ Route 36, Encounter does not have type

            new(HGSS) // Spiky-Eared Pichu @ Ilex Forest
            {
                Species = 172,
                Level = 30,
                Gender = 1,
                Form = 1,
                Nature = Nature.Naughty,
                Location = 214,
                Moves = new[] { 344, 270, 207, 220 },
                TypeEncounter = EncounterType.Starter_Fossil_Gift_Pt_DPTrio,
                Shiny = Shiny.Never
            },

            // Stationary Legendary
            new(HGSS) { Species = 144, Level = 50, Location = 203, TypeEncounter = EncounterType.Cave_HallOfOrigin }, // Articuno @ Seafoam Islands
            new(HGSS) { Species = 145, Level = 50, Location = 158, }, // Zapdos @ Route 10
            new(HGSS) { Species = 146, Level = 50, Location = 219, TypeEncounter = EncounterType.Cave_HallOfOrigin }, // Moltres @ Mt. Silver Cave
            new(HGSS) { Species = 150, Level = 70, Location = 199, TypeEncounter = EncounterType.Cave_HallOfOrigin }, // Mewtwo @ Cerulean Cave
            new(HGSS) { Species = 245, Level = 40, Location = 173, }, // Suicune @ Route 25
            new(HGSS) { Species = 245, Level = 40, Location = 206, TypeEncounter = EncounterType.Cave_HallOfOrigin }, // Suicune @ Burned Tower
            new(  SS) { Species = 249, Level = 45, Location = 218, TypeEncounter = EncounterType.Surfing_Fishing }, // Lugia @ Whirl Islands
            new(HG  ) { Species = 249, Level = 70, Location = 218, TypeEncounter = EncounterType.Surfing_Fishing }, // Lugia @ Whirl Islands
            new(HG  ) { Species = 250, Level = 45, Location = 205, TypeEncounter = EncounterType.Building_EnigmaStone }, // Ho-Oh @ Bell Tower
            new(  SS) { Species = 250, Level = 70, Location = 205, TypeEncounter = EncounterType.Building_EnigmaStone }, // Ho-Oh @ Bell Tower
            new(  SS) { Species = 380, Level = 40, Location = 140, TypeEncounter = EncounterType.Building_EnigmaStone }, // Latias @ Pewter City
            new(HG  ) { Species = 381, Level = 40, Location = 140, TypeEncounter = EncounterType.Building_EnigmaStone }, // Latios @ Pewter City
            new(HG  ) { Species = 382, Level = 50, Location = 232, TypeEncounter = EncounterType.Cave_HallOfOrigin }, // Kyogre @ Embedded Tower
            new(  SS) { Species = 383, Level = 50, Location = 232, TypeEncounter = EncounterType.Cave_HallOfOrigin }, // Groudon @ Embedded Tower
            new(HGSS) { Species = 384, Level = 50, Location = 232, TypeEncounter = EncounterType.Cave_HallOfOrigin }, // Rayquaza @ Embedded Tower
            new(HGSS) { Species = 483, Level = 01, Location = 231, Gift = true, TypeEncounter = EncounterType.Starter_Fossil_Gift_Pt_DPTrio }, // Dialga @ Sinjoh Ruins
            new(HGSS) { Species = 484, Level = 01, Location = 231, Gift = true, TypeEncounter = EncounterType.Starter_Fossil_Gift_Pt_DPTrio }, // Palkia @ Sinjoh Ruins
            new(HGSS) { Species = 487, Level = 01, Location = 231, Gift = true, TypeEncounter = EncounterType.Starter_Fossil_Gift_Pt_DPTrio, Form = 1, HeldItem = 112 }, // Giratina @ Sinjoh Ruins

            // Johto Roamers
            new(HGSS) { Roaming = true, Species = 243, Level = 40, TypeEncounter = EncounterType.TallGrass | EncounterType.Surfing_Fishing, }, // Raikou
            new(HGSS) { Roaming = true, Species = 244, Level = 40, TypeEncounter = EncounterType.TallGrass | EncounterType.Surfing_Fishing, }, // Entei

            // Kanto Roamers
            new(HG  ) { Roaming = true, Species = 380, Level = 35, TypeEncounter = EncounterType.TallGrass | EncounterType.Surfing_Fishing, }, // Latias
            new(  SS) { Roaming = true, Species = 381, Level = 35, TypeEncounter = EncounterType.TallGrass | EncounterType.Surfing_Fishing, }, // Latios
        };
        #endregion
        #region Trade Tables

        private static readonly EncounterTrade4[] RanchGifts =
        {
            new EncounterTrade4RanchGift(323975838, 025, 18) { Moves = new[] {447,085,148,104}, TID = 1000, SID = 19840, OTGender = 1, Location = 0068, Gender = 0, Ability = 1, CurrentLevel = 20, }, // Pikachu
            new EncounterTrade4RanchGift(323977664, 037, 16) { Moves = new[] {412,109,053,219}, TID = 1000, SID = 21150, OTGender = 1, Location = 3000, Gender = 0, Ability = 1, CurrentLevel = 30, }, // Vulpix
            new EncounterTrade4RanchGift(323975579, 077, 13) { Moves = new[] {036,033,039,052}, TID = 1000, SID = 01123, OTGender = 1, Location = 3000, Gender = 0, Ability = 2, CurrentLevel = 16, }, // Ponyta
            new EncounterTrade4RanchGift(323975564, 108, 34) { Moves = new[] {076,111,014,205}, TID = 1000, SID = 03050, OTGender = 1, Location = 0077, Gender = 0, Ability = 1, CurrentLevel = 40, }, // Lickitung
            new EncounterTrade4RanchGift(323977579, 114, 01) { Moves = new[] {437,438,079,246}, TID = 1000, SID = 49497, OTGender = 1, Location = 3000, Gender = 1, Ability = 2, }, // Tangela
            new EncounterTrade4RanchGift(323977675, 133, 16) { Moves = new[] {363,270,098,247}, TID = 1000, SID = 47710, OTGender = 1, Location = 0068, Gender = 0, Ability = 2, CurrentLevel = 30, }, // Eevee
            new EncounterTrade4RanchGift(323977588, 142, 20) { Moves = new[] {363,089,444,332}, TID = 1000, SID = 43066, OTGender = 1, Location = 0094, Gender = 0, Ability = 1, CurrentLevel = 50, }, // Aerodactyl
            new EncounterTrade4RanchGift(232975554, 193, 22) { Moves = new[] {318,095,246,138}, TID = 1000, SID = 42301, OTGender = 1, Location = 0052, Gender = 0, Ability = 1, CurrentLevel = 45, Ball = 0x05, }, // Yanma
            new EncounterTrade4RanchGift(323975570, 241, 16) { Moves = new[] {208,215,360,359}, TID = 1000, SID = 02707, OTGender = 1, Location = 3000, Gender = 1, Ability = 1, CurrentLevel = 48, }, // Miltank
            new EncounterTrade4RanchGift(323975563, 285, 22) { Moves = new[] {402,147,206,078}, TID = 1000, SID = 02788, OTGender = 1, Location = 3000, Gender = 0, Ability = 2, CurrentLevel = 45, Ball = 0x05, }, // Shroomish
            new EncounterTrade4RanchGift(323975559, 320, 30) { Moves = new[] {156,323,133,058}, TID = 1000, SID = 27046, OTGender = 1, Location = 0038, Gender = 0, Ability = 2, CurrentLevel = 45, }, // Wailmer
            new EncounterTrade4RanchGift(323977657, 360, 01) { Moves = new[] {204,150,227,000}, TID = 1000, SID = 01788, OTGender = 1, Location = 0004, Gender = 0, Ability = 2, EggLocation = 2000, }, // Wynaut
            new EncounterTrade4RanchGift(323975563, 397, 02) { Moves = new[] {355,017,283,018}, TID = 1000, SID = 59298, OTGender = 1, Location = 0016, Gender = 0, Ability = 2, CurrentLevel = 23, }, // Staravia
            new EncounterTrade4RanchGift(323970584, 415, 05) { Moves = new[] {230,016,000,000}, TID = 1000, SID = 54140, OTGender = 1, Location = 0020, Gender = 1, Ability = 1, CurrentLevel = 20, }, // Combee
            new EncounterTrade4RanchGift(323977539, 417, 09) { Moves = new[] {447,045,351,098}, TID = 1000, SID = 18830, OTGender = 1, Location = 0020, Gender = 1, Ability = 2, CurrentLevel = 10, }, // Pachirisu
            new EncounterTrade4RanchGift(323974107, 422, 20) { Moves = new[] {363,352,426,104}, TID = 1000, SID = 39272, OTGender = 1, Location = 0028, Gender = 0, Ability = 2, CurrentLevel = 25, Form = 1 }, // Shellos
            new EncounterTrade4RanchGift(323977566, 427, 10) { Moves = new[] {204,193,409,098}, TID = 1000, SID = 31045, OTGender = 1, Location = 3000, Gender = 1, Ability = 1, CurrentLevel = 16, }, // Buneary
            new EncounterTrade4RanchGift(323975579, 453, 22) { Moves = new[] {310,207,426,389}, TID = 1000, SID = 41342, OTGender = 1, Location = 0052, Gender = 0, Ability = 2, CurrentLevel = 31, Ball = 0x05, }, // Croagunk
            new EncounterTrade4RanchGift(323977566, 456, 15) { Moves = new[] {213,352,219,392}, TID = 1000, SID = 48348, OTGender = 1, Location = 0020, Gender = 1, Ability = 1, CurrentLevel = 35, }, // Finneon
            new EncounterTrade4RanchGift(323975582, 459, 32) { Moves = new[] {452,420,275,059}, TID = 1000, SID = 23360, OTGender = 1, Location = 0031, Gender = 0, Ability = 1, CurrentLevel = 41, }, // Snover
            new EncounterTrade4RanchSpecial(151, 50) { Moves = new[] {235,216,095,100}, TID = 1000, SID = 59228, OTGender = 1, Location = 3000, Ball = 0x10, Gender = 2, }, // Mew
            new EncounterTrade4RanchSpecial(489, 01) { Moves = new[] {447,240,156,057}, TID = 1000, SID = 09248, OTGender = 1, Location = 3000, Ball = 0x10, Gender = 2, CurrentLevel = 50, EggLocation = 3000, }, // Phione
        };

        internal static readonly EncounterTrade4[] TradeGift_DPPt = new EncounterTrade4PID[]
        {
            new(DPPt, 0x0000008E, 063, 01) { Ability = 1, TID = 25643, SID = 00000, OTGender = 1, Gender = 0, IVs = new[] {15,15,15,20,25,25} }, // Machop -> Abra
            new(DPPt, 0x00000867, 441, 01) { Ability = 2, TID = 44142, SID = 00000, OTGender = 0, Gender = 1, IVs = new[] {15,20,15,25,25,15}, Contest = 20 }, // Buizel -> Chatot
            new(DPPt, 0x00000088, 093, 35) { Ability = 1, TID = 19248, SID = 00000, OTGender = 1, Gender = 0, IVs = new[] {20,25,15,25,15,15} }, // Medicham (35 from Route 217) -> Haunter
            new(DPPt, 0x0000045C, 129, 01) { Ability = 1, TID = 53277, SID = 00000, OTGender = 0, Gender = 1, IVs = new[] {15,25,15,20,25,15} }, // Finneon -> Magikarp
        }.Concat(RanchGifts).ToArray();

        internal static readonly EncounterTrade4PID[] TradeGift_HGSS =
        {
            new(HGSS, 0x000025EF, 095, 01) { Ability = 2, TID = 48926, SID = 00000, OTGender = 0, Gender = 0, IVs = new[] {25,20,25,15,15,15} }, // Bellsprout -> Onix
            new(HGSS, 0x00002310, 066, 01) { Ability = 1, TID = 37460, SID = 00000, OTGender = 0, Gender = 1, IVs = new[] {15,25,20,20,15,15} }, // Drowzee -> Machop
            new(HGSS, 0x000001DB, 100, 01) { Ability = 2, TID = 29189, SID = 00000, OTGender = 0, Gender = 2, IVs = new[] {15,20,15,25,25,15} }, // Krabby -> Voltorb
            new(HGSS, 0x0001FC0A, 085, 15) { Ability = 1, TID = 00283, SID = 00000, OTGender = 1, Gender = 1, IVs = new[] {20,20,20,15,15,15} }, // Dragonair (15 from DPPt) -> Dodrio
            new(HGSS, 0x0000D136, 082, 19) { Ability = 1, TID = 50082, SID = 00000, OTGender = 0, Gender = 2, IVs = new[] {15,20,15,20,20,20} }, // Dugtrio (19 from Diglett's Cave) -> Magneton
            new(HGSS, 0x000034E4, 178, 16) { Ability = 1, TID = 15616, SID = 00000, OTGender = 0, Gender = 0, IVs = new[] {15,20,15,20,20,20} }, // Haunter (16 from Old Chateau) -> Xatu
            new(HGSS, 0x00485876, 025, 02) { Ability = 1, TID = 33038, SID = 00000, OTGender = 0, Gender = 1, IVs = new[] {20,25,18,31,25,13} }, // Pikachu
            new(HGSS, 0x0012B6D4, 374, 31) { Ability = 1, TID = 23478, SID = 00000, OTGender = 0, Gender = 2, IVs = new[] {28,29,24,23,24,25} }, // Forretress -> Beldum
            new(HGSS, 0x0012971C, 111, 01) { Ability = 1, TID = 06845, SID = 00000, OTGender = 0, Gender = 1, IVs = new[] {22,31,13,00,22,09}, Moves = new[]{422} }, // Bonsly -> Rhyhorn
            new(HGSS, 0x00101596, 208, 01) { Ability = 1, TID = 26491, SID = 00000, OTGender = 1, Gender = 0, IVs = new[] {08,30,28,06,18,20}}, // Any -> Steelix

            //Gift
            new(HGSS, 0x00006B5E, 021, 20) { Ability = 1, TID = 01001, SID = 00000, OTGender = 0, Gender = 1, IVs = new[] {15,20,15,20,20,20}, Location = 183, Moves = new[]{043,031,228,332} },// Webster's Spearow
            new(HGSS, 0x000214D7, 213, 20) { Ability = 2, TID = 04336, SID = 00001, OTGender = 0, Gender = 0, IVs = new[] {15,20,15,20,20,20}, Location = 130, Moves = new[]{132,117,227,219} },// Kirk's Shuckle
        };

        private const string tradeDPPt = "tradedppt";
        private const string tradeHGSS = "tradehgss";
        private static readonly string[][] TradeDPPt = Util.GetLanguageStrings8(tradeDPPt);
        private static readonly string[][] TradeHGSS = Util.GetLanguageStrings8(tradeHGSS);
        #endregion

        internal static readonly EncounterStatic4[] StaticD = GetEncounters(Encounter_DPPt, D);
        internal static readonly EncounterStatic4[] StaticP = GetEncounters(Encounter_DPPt, P);
        internal static readonly EncounterStatic4[] StaticPt = GetEncounters(Encounter_DPPt, Pt);
        internal static readonly EncounterStatic[] StaticHG = GetEncounters(ArrayUtil.ConcatAll<EncounterStatic>(Encounter_HGSS, Encounter_PokeWalker), HG);
        internal static readonly EncounterStatic[] StaticSS = GetEncounters(ArrayUtil.ConcatAll<EncounterStatic>(Encounter_HGSS, Encounter_PokeWalker), SS);
    }
}
