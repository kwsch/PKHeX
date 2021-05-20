using static PKHeX.Core.EncounterUtil;
using static PKHeX.Core.GameVersion;

namespace PKHeX.Core
{
    /// <summary>
    /// Generation 5 Encounters
    /// </summary>
    public static class Encounters5
    {
        internal static readonly EncounterArea5[] SlotsB = EncounterArea5.GetAreas(Get("b", "51"), B);
        internal static readonly EncounterArea5[] SlotsW = EncounterArea5.GetAreas(Get("w", "51"), W);
        internal static readonly EncounterArea5[] SlotsB2 = EncounterArea5.GetAreas(Get("b2", "52"), B2);
        internal static readonly EncounterArea5[] SlotsW2 = EncounterArea5.GetAreas(Get("w2", "52"), W2);
        private static byte[][] Get(string resource, string ident) => BinLinker.Unpack(Util.GetBinaryResource($"encounter_{resource}.pkl"), ident);

        static Encounters5()
        {
            MarkEncounterTradeStrings(TradeGift_BW, TradeBW);
            MarkEncounterTradeStrings(TradeGift_B2W2_Regular, TradeB2W2);
        }

        #region Dream Radar Tables

        private static readonly EncounterStatic5DR[] Encounter_DreamRadar =
        {
            new(079, 0), // Slowpoke
            new(120, 0), // Staryu
            new(137, 0), // Porygon
            new(163, 0), // Hoothoot
            new(174, 0), // Igglybuff
            new(175, 0), // Togepi
            new(213, 0), // Shuckle
            new(238, 0), // Smoochum
            new(249, 0), // Lugia (SoulSilver cart)
            new(250, 0), // Ho-Oh (HeartGold cart)
            new(280, 0), // Ralts
            new(333, 0), // Swablu
            new(374, 0), // Beldum
            new(425, 0), // Drifloon
            new(436, 0), // Bronzor
            new(442, 0), // Spiritomb
            new(447, 0), // Riolu
            new(479, 0, 0), // Rotom (no HA)
            new(483, 0), // Dialga (Diamond cart)
            new(484, 0), // Palkia (Pearl cart)
            new(487, 0), // Giratina (Platinum cart)
            new(517, 0), // Munna
            new(561, 0), // Sigilyph
            new(641, 1), // Therian Tornadus
            new(642, 1), // Therian Thundurus
            new(645, 1), // Therian Landorus
        };

        #endregion
        #region DreamWorld Encounter

        public static readonly EncounterStatic5[] DreamWorld_Common = DreamWorldEntry.GetArray(Gen5, new DreamWorldEntry[]
        {
            // Pleasant Forest
            new(019, 10, 098, 382, 231), // Rattata
            new(043, 10, 230, 298, 202), // Oddish
            new(069, 10, 022, 235, 402), // Bellsprout
            new(077, 10, 033, 037, 257), // Ponyta
            new(083, 10, 210, 355, 348), // Farfetch'd
            new(084, 10, 045, 175, 355), // Doduo
            new(102, 10, 140, 235, 202), // Exeggcute
            new(108, 10, 122, 214, 431), // Lickitung
            new(114, 10, 079, 073, 402), // Tangela
            new(115, 10, 252, 068, 409), // Kangaskhan
            new(161, 10, 010, 203, 343), // Sentret
            new(179, 10, 084, 115, 351), // Mareep
            new(191, 10, 072, 230, 414), // Sunkern
            new(234, 10, 033, 050, 285), // Stantler
            new(261, 10, 336, 305, 399), // Poochyena
            new(283, 10, 145, 056, 202), // Surskit
            new(399, 10, 033, 401, 290), // Bidoof
            new(403, 10, 268, 393, 400), // Shinx
            new(431, 10, 252, 372, 290), // Glameow
            new(054, 10, 346, 227, 362), // Psyduck
            new(058, 10, 044, 034, 203), // Growlithe
            new(123, 10, 098, 226, 366), // Scyther
            new(128, 10, 099, 231, 431), // Tauros
            new(183, 10, 111, 453, 008), // Marill
            new(185, 10, 175, 205, 272), // Sudowoodo
            new(203, 10, 093, 243, 285), // Girafarig
            new(241, 10, 111, 174, 231), // Miltank
            new(263, 10, 033, 271, 387), // Zigzagoon
            new(427, 10, 193, 252, 409), // Buneary
            new(037, 10, 046, 257, 399), // Vulpix
            new(060, 10, 095, 054, 214), // Poliwag
            new(177, 10, 101, 297, 202), // Natu
            new(239, 10, 084, 238, 393), // Elekid
            new(300, 10, 193, 321, 445), // Skitty

            // Windswept Sky
            new(016, 10, 016, 211, 290), // Pidgey
            new(021, 10, 064, 185, 211), // Spearow
            new(041, 10, 048, 095, 162), // Zubat
            new(142, 10, 044, 372, 446), // Aerodactyl
            new(165, 10, 004, 450, 009), // Ledyba
            new(187, 10, 235, 227, 340), // Hoppip
            new(193, 10, 098, 364, 202), // Yanma
            new(198, 10, 064, 109, 355), // Murkrow
            new(207, 10, 028, 364, 366), // Gligar
            new(225, 10, 217, 420, 264), // Delibird
            new(276, 10, 064, 203, 413), // Taillow
            new(397, 14, 017, 297, 366), // Staravia
            new(227, 10, 064, 065, 355), // Skarmory
            new(357, 10, 016, 073, 318), // Tropius

            // Sparkling Sea
            new(086, 10, 029, 333, 214), // Seel
            new(090, 10, 110, 112, 196), // Shellder
            new(116, 10, 145, 190, 362), // Horsea
            new(118, 10, 064, 060, 352), // Goldeen
            new(129, 10, 150, 175, 340), // Magikarp
            new(138, 10, 044, 330, 196), // Omanyte
            new(140, 10, 071, 175, 446), // Kabuto
            new(170, 10, 086, 133, 351), // Chinchou
            new(194, 10, 055, 034, 401), // Wooper
            new(211, 10, 040, 453, 290), // Qwilfish
            new(223, 10, 199, 350, 362), // Remoraid
            new(226, 10, 048, 243, 314), // Mantine
            new(320, 10, 055, 214, 340), // Wailmer
            new(339, 10, 189, 214, 209), // Barboach
            new(366, 10, 250, 445, 392), // Clamperl
            new(369, 10, 055, 214, 414), // Relicanth
            new(370, 10, 204, 300, 196), // Luvdisc
            new(418, 10, 346, 163, 352), // Buizel
            new(456, 10, 213, 186, 352), // Finneon
            new(072, 10, 048, 367, 202), // Tentacool
            new(318, 10, 044, 037, 399), // Carvanha
            new(341, 10, 106, 232, 283), // Corphish
            new(345, 10, 051, 243, 202), // Lileep
            new(347, 10, 010, 446, 440), // Anorith
            new(349, 10, 150, 445, 243), // Feebas
            new(131, 10, 109, 032, 196), // Lapras
            new(147, 10, 086, 352, 225), // Dratini

            // Spooky Manor
            new(092, 10, 095, 050, 482), // Gastly
            new(096, 10, 095, 427, 409), // Drowzee
            new(122, 10, 112, 298, 285), // Mr. Mime
            new(167, 10, 040, 527, 450), // Spinarak
            new(200, 10, 149, 194, 517), // Misdreavus
            new(228, 10, 336, 364, 399), // Houndour
            new(325, 10, 149, 285, 278), // Spoink
            new(353, 10, 101, 194, 220), // Shuppet
            new(355, 10, 050, 220, 271), // Duskull
            new(358, 10, 035, 095, 304), // Chimecho
            new(434, 10, 103, 492, 389), // Stunky
            new(209, 10, 204, 370, 038), // Snubbull
            new(235, 10, 166, 445, 214), // Smeargle
            new(313, 10, 148, 271, 366), // Volbeat
            new(314, 10, 204, 313, 366), // Illumise
            new(063, 10, 100, 285, 356), // Abra

            // Rugged Mountain
            new(066, 10, 067, 418, 270), // Machop
            new(081, 10, 319, 278, 356), // Magnemite
            new(109, 10, 123, 399, 482), // Koffing
            new(218, 10, 052, 517, 257), // Slugma
            new(246, 10, 044, 399, 446), // Larvitar
            new(324, 10, 052, 090, 446), // Torkoal
            new(328, 10, 044, 324, 202), // Trapinch
            new(331, 10, 071, 298, 009), // Cacnea
            new(412, 10, 182, 450, 173), // Burmy
            new(449, 10, 044, 254, 276), // Hippopotas
            new(240, 10, 052, 009, 257), // Magby
            new(322, 10, 052, 034, 257), // Numel
            new(359, 10, 364, 224, 276), // Absol
            new(453, 10, 040, 409, 441), // Croagunk
            new(236, 10, 252, 364, 183), // Tyrogue
            new(371, 10, 044, 349, 200), // Bagon

            // Icy Cave
            new(027, 10, 028, 068, 162), // Sandshrew
            new(074, 10, 111, 446, 431), // Geodude
            new(095, 10, 020, 446, 431), // Onix
            new(100, 10, 268, 324, 363), // Voltorb
            new(104, 10, 125, 195, 067), // Cubone
            new(293, 10, 253, 283, 428), // Whismur
            new(304, 10, 106, 283, 457), // Aron
            new(337, 10, 093, 414, 236), // Lunatone
            new(338, 10, 093, 428, 234), // Solrock
            new(343, 10, 229, 356, 428), // Baltoy
            new(459, 10, 075, 419, 202), // Snover
            new(050, 10, 028, 251, 446), // Diglett
            new(215, 10, 269, 008, 067), // Sneasel
            new(361, 10, 181, 311, 352), // Snorunt
            new(220, 10, 316, 246, 333), // Swinub
            new(443, 10, 082, 200, 203), // Gible

            // Dream Park
            new(046, 10, 078, 440, 235), // Paras
            new(204, 10, 120, 390, 356), // Pineco
            new(265, 10, 040, 450, 173), // Wurmple
            new(273, 10, 074, 331, 492), // Seedot
            new(287, 10, 281, 400, 389), // Slakoth
            new(290, 10, 141, 203, 400), // Nincada
            new(311, 10, 086, 435, 324), // Plusle
            new(312, 10, 086, 435, 324), // Minun
            new(316, 10, 139, 151, 202), // Gulpin
            new(352, 10, 185, 285, 513), // Kecleon
            new(401, 10, 522, 283, 253), // Kricketot
            new(420, 10, 073, 505, 331), // Cherubi
            new(455, 10, 044, 476, 380), // Carnivine
            new(023, 10, 040, 251, 399), // Ekans
            new(175, 10, 118, 381, 253), // Togepi
            new(190, 10, 010, 252, 007), // Aipom
            new(285, 10, 078, 331, 264), // Shroomish
            new(315, 10, 074, 079, 129), // Roselia
            new(113, 10, 045, 068, 270), // Chansey
            new(127, 10, 011, 370, 382), // Pinsir
            new(133, 10, 028, 204, 129), // Eevee
            new(143, 10, 133, 007, 278), // Snorlax
            new(214, 10, 030, 175, 264), // Heracross

            // Pokémon Café Forest
            new(061, 25, 240, 114, 352), // Poliwhirl
            new(133, 10, 270, 204, 129), // Eevee
            new(235, 10, 166, 445, 214), // Smeargle
            new(412, 10, 182, 450, 173), // Burmy

            // PGL
            new(212, 10, 211, Gender: 0), // Scizor
            new(445, 48, Gender: 0), // Garchomp
            new(149, 55, 245, Gender: 0), // Dragonite
            new(248, 55, 069, Gender: 0), // Tyranitar
            new(376, 45, 038, Gender: 2), // Metagross
        });

        public static readonly EncounterStatic5[] DreamWorld_BW = DreamWorldEntry.GetArray(BW, new DreamWorldEntry[]
        {
            // Pleasant Forest
            new(029, 10, 010, 389, 162), // Nidoran♀
            new(032, 10, 064, 068, 162), // Nidoran♂
            new(174, 10, 047, 313, 270), // Igglybuff
            new(187, 10, 235, 270, 331), // Hoppip
            new(270, 10, 071, 073, 352), // Lotad
            new(276, 10, 064, 119, 366), // Taillow
            new(309, 10, 086, 423, 324), // Electrike
            new(351, 10, 052, 466, 352), // Castform
            new(417, 10, 098, 343, 351), // Pachirisu

            // Windswept Sky
            new(012, 10, 093, 355, 314), // Butterfree
            new(163, 10, 193, 101, 278), // Hoothoot
            new(278, 10, 055, 239, 351), // Wingull
            new(333, 10, 064, 297, 355), // Swablu
            new(425, 10, 107, 095, 285), // Drifloon
            new(441, 10, 119, 417, 272), // Chatot

            // Sparkling Sea
            new(079, 10, 281, 335, 362), // Slowpoke
            new(098, 10, 011, 133, 290), // Krabby
            new(119, 33, 352, 214, 203), // Seaking
            new(120, 10, 055, 278, 196), // Staryu
            new(222, 10, 145, 109, 446), // Corsola
            new(422, 10, 189, 281, 290, Form: 0), // Shellos-West
            new(422, 10, 189, 281, 290, Form: 1), // Shellos-East

            // Spooky Manor
            new(202, 15, 243, 204, 227), // Wobbuffet
            new(238, 10, 186, 445, 285), // Smoochum
            new(303, 10, 313, 424, 008), // Mawile
            new(307, 10, 096, 409, 203), // Meditite
            new(436, 10, 095, 285, 356), // Bronzor
            new(052, 10, 010, 095, 290), // Meowth
            new(479, 10, 086, 351, 324), // Rotom
            new(280, 10, 093, 194, 270), // Ralts
            new(302, 10, 193, 389, 180), // Sableye
            new(442, 10, 180, 220, 196), // Spiritomb

            // Rugged Mountain
            new(056, 10, 067, 179, 009), // Mankey
            new(111, 10, 030, 068, 038), // Rhyhorn
            new(231, 10, 175, 484, 402), // Phanpy
            new(451, 10, 044, 097, 401), // Skorupi
            new(216, 10, 313, 242, 264), // Teddiursa
            new(296, 10, 292, 270, 008), // Makuhita
            new(327, 10, 383, 252, 276), // Spinda
            new(374, 10, 036, 428, 442), // Beldum
            new(447, 10, 203, 418, 264), // Riolu

            // Icy Cave
            new(173, 10, 227, 312, 214), // Cleffa
            new(213, 10, 227, 270, 504), // Shuckle
            new(299, 10, 033, 446, 246), // Nosepass
            new(363, 10, 181, 090, 401), // Spheal
            new(408, 10, 029, 442, 007), // Cranidos
            new(206, 10, 111, 277, 446), // Dunsparce
            new(410, 10, 182, 068, 090), // Shieldon

            // Dream Park
            new(048, 10, 050, 226, 285), // Venonat
            new(088, 10, 139, 114, 425), // Grimer
            new(415, 10, 016, 366, 314), // Combee
            new(015, 10, 031, 314, 210), // Beedrill
            new(335, 10, 098, 458, 067), // Zangoose
            new(336, 10, 044, 034, 401), // Seviper

            // PGL
            new(134, 10, Gender: 0), // Vaporeon
            new(135, 10, Gender: 0), // Jolteon
            new(136, 10, Gender: 0), // Flareon
            new(196, 10, Gender: 0), // Espeon
            new(197, 10, Gender: 0), // Umbreon
            new(470, 10, Gender: 0), // Leafeon
            new(471, 10, Gender: 0), // Glaceon
            new(001, 10, Gender: 0), // Bulbasaur
            new(004, 10, Gender: 0), // Charmander
            new(007, 10, Gender: 0), // Squirtle
            new(453, 10, Gender: 0), // Croagunk
            new(387, 10, Gender: 0), // Turtwig
            new(390, 10, Gender: 0), // Chimchar
            new(393, 10, Gender: 0), // Piplup
            new(493, 100), // Arceus
            new(252, 10, Gender: 0), // Treecko
            new(255, 10, Gender: 0), // Torchic
            new(258, 10, Gender: 0), // Mudkip
            new(468, 10, 217, Gender: 0), // Togekiss
            new(473, 34, Gender: 0), // Mamoswine
            new(137, 10), // Porygon
            new(384, 50), // Rayquaza
            new(354, 37, 538, Gender: 1), // Banette
            new(453, 10, 398, Gender: 0), // Croagunk
            new(334, 35, 206, Gender: 0),  // Altaria
            new(242, 10), // Blissey
            new(448, 10, 418, Gender: 0), // Lucario
            new(189, 27, 206, Gender: 0), // Jumpluff
        });

        public static readonly EncounterStatic5[] DreamWorld_B2W2 = DreamWorldEntry.GetArray(B2W2, new DreamWorldEntry[]
        {
            // Pleasant Forest
            new(535, 10, 496, 414, 352), // Tympole
            new(546, 10, 073, 227, 388), // Cottonee
            new(548, 10, 079, 204, 230), // Petilil
            new(588, 10, 203, 224, 450), // Karrablast
            new(616, 10, 051, 226, 227), // Shelmet
            new(545, 30, 342, 390, 276), // Scolipede

            // Windswept Sky
            new(519, 10, 016, 095, 234), // Pidove
            new(561, 10, 095, 500, 257), // Sigilyph
            new(580, 10, 432, 362, 382), // Ducklett
            new(587, 10, 098, 403, 204), // Emolga

            // Sparkling Sea
            new(550, 10, 029, 097, 428, Form: 0), // Basculin-Red
            new(550, 10, 029, 097, 428, Form: 1), // Basculin-Blue
            new(594, 10, 392, 243, 220), // Alomomola
            new(618, 10, 189, 174, 281), // Stunfisk
            new(564, 10, 205, 175, 334), // Tirtouga

            // Spooky Manor
            new(605, 10, 377, 112, 417), // Elgyem
            new(624, 10, 210, 427, 389), // Pawniard
            new(596, 36, 486, 050, 228), // Galvantula
            new(578, 32, 105, 286, 271), // Duosion
            new(622, 10, 205, 007, 009), // Golett

            // Rugged Mountain
            new(631, 10, 510, 257, 202), // Heatmor
            new(632, 10, 210, 203, 422), // Durant
            new(556, 10, 042, 073, 191), // Maractus
            new(558, 34, 157, 068, 400), // Crustle
            new(553, 40, 242, 068, 212), // Krookodile

            // Icy Cave
            new(529, 10, 229, 319, 431), // Drilbur
            new(621, 10, 044, 424, 389), // Druddigon
            new(525, 25, 479, 174, 484), // Boldore
            new(583, 35, 429, 420, 286), // Vanillish
            new(600, 38, 451, 356, 393), // Klang
            new(610, 10, 082, 068, 400), // Axew

            // Dream Park
            new(531, 10, 270, 227, 281), // Audino
            new(538, 10, 020, 008, 276), // Throh
            new(539, 10, 249, 009, 530), // Sawk
            new(559, 10, 067, 252, 409), // Scraggy
            new(533, 25, 067, 183, 409), // Gurdurr

            // PGL
            new(575, 32, 243, Gender: 0), // Gothorita
            new(025, 10, 029, Gender: 0), // Pikachu
            new(511, 10, 437, Gender: 0), // Pansage
            new(513, 10, 257, Gender: 0), // Pansear
            new(515, 10, 056, Gender: 0), // Panpour
            new(387, 10, 254, Gender: 0), // Turtwig
            new(390, 10, 252, Gender: 0), // Chimchar
            new(393, 10, 297, Gender: 0), // Piplup
            new(575, 32, 286, Gender: 0), // Gothorita
        });

        #endregion
        #region Static Encounter/Gift Tables
        private static readonly EncounterStatic5[] Encounter_BW =
        {
            // Starters @ Nuvema Town
            new(BW) { Gift = true, Species = 495, Level = 05, Location = 004 }, // Snivy
            new(BW) { Gift = true, Species = 498, Level = 05, Location = 004 }, // Tepig
            new(BW) { Gift = true, Species = 501, Level = 05, Location = 004 }, // Oshawott

            // Fossils @ Nacrene City
            new(BW) { Gift = true, Species = 138, Level = 25, Location = 007 }, // Omanyte
            new(BW) { Gift = true, Species = 140, Level = 25, Location = 007 }, // Kabuto
            new(BW) { Gift = true, Species = 142, Level = 25, Location = 007 }, // Aerodactyl
            new(BW) { Gift = true, Species = 345, Level = 25, Location = 007 }, // Lileep
            new(BW) { Gift = true, Species = 347, Level = 25, Location = 007 }, // Anorith
            new(BW) { Gift = true, Species = 408, Level = 25, Location = 007 }, // Cranidos
            new(BW) { Gift = true, Species = 410, Level = 25, Location = 007 }, // Shieldon
            new(BW) { Gift = true, Species = 564, Level = 25, Location = 007 }, // Tirtouga
            new(BW) { Gift = true, Species = 566, Level = 25, Location = 007 }, // Archen

            // Gift
            new(BW) { Gift = true, Species = 511, Level = 10, Location = 032 }, // Pansage @ Dreamyard
            new(BW) { Gift = true, Species = 513, Level = 10, Location = 032 }, // Pansear
            new(BW) { Gift = true, Species = 515, Level = 10, Location = 032 }, // Panpour
            new(BW) { Gift = true, Species = 129, Level = 05, Location = 068 }, // Magikarp @ Marvelous Bridge
            new(BW) { Gift = true, Species = 636, Level = 01, EggLocation = 60003 }, // Larvesta Egg from Treasure Hunter

            // Stationary
            new(BW) { Species = 518, Level = 50, Location = 032, Ability = 4 }, // Musharna @ Dreamyard Friday Only
            new(BW) { Species = 590, Level = 20, Location = 019, }, // Foongus @ Route 6
            new(BW) { Species = 590, Level = 30, Location = 023, }, // Foongus @ Route 10
            new(BW) { Species = 591, Level = 40, Location = 023, }, // Amoonguss @ Route 10
            new(BW) { Species = 555, Level = 35, Location = 034, Ability = 4 }, // HA Darmanitan @ Desert Resort
            new(BW) { Species = 637, Level = 70, Location = 035, }, // Volcarona @ Relic Castle

            // Stationary Legendary
            new(BW) { Species = 638, Level = 42, Location = 074, }, // Cobalion @ Guidance Chamber
            new(BW) { Species = 639, Level = 42, Location = 073, }, // Terrakion @ Trial Chamber
            new(BW) { Species = 640, Level = 42, Location = 055, }, // Virizion @ Rumination Field
            new(B ) { Species = 643, Level = 50, Location = 045, Shiny = Shiny.Never }, // Reshiram @ N's Castle
            new(B ) { Species = 643, Level = 50, Location = 039, Shiny = Shiny.Never }, // Reshiram @ Dragonspiral Tower
            new( W) { Species = 644, Level = 50, Location = 045, Shiny = Shiny.Never }, // Zekrom @ N's Castle
            new( W) { Species = 644, Level = 50, Location = 039, Shiny = Shiny.Never }, // Zekrom @ Dragonspiral Tower
            new(BW) { Species = 645, Level = 70, Location = 070, }, // Landorus @ Abundant Shrine
            new(BW) { Species = 646, Level = 75, Location = 061, }, // Kyurem @ Giant Chasm

            // Event
            new(BW) { Species = 494, Level = 15, Location = 062, Shiny = Shiny.Never}, // Victini @ Liberty Garden
            new(BW) { Species = 570, Level = 10, Location = 008, Shiny = Shiny.Never, Gender = 0, }, // Zorua @ Castelia City
            new(BW) { Species = 571, Level = 25, Location = 072, Shiny = Shiny.Never, Gender = 1, }, // Zoroark @ Lostlorn Forest

            // Roamer
            new(B ) { Roaming = true, Species = 641, Level = 40, Location = 25, }, // Tornadus
            new( W) { Roaming = true, Species = 642, Level = 40, Location = 25, }, // Thundurus
        };

        private static readonly EncounterStatic5[] Encounter_B2W2_Regular =
        {
            // Starters @ Aspertia City
            new(B2W2) { Gift = true, Species = 495, Level = 05, Location = 117 }, // Snivy
            new(B2W2) { Gift = true, Species = 498, Level = 05, Location = 117 }, // Tepig
            new(B2W2) { Gift = true, Species = 501, Level = 05, Location = 117 }, // Oshawott

            // Fossils @ Nacrene City
            new(B2W2) { Gift = true, Species = 138, Level = 25, Location = 007 }, // Omanyte
            new(B2W2) { Gift = true, Species = 140, Level = 25, Location = 007 }, // Kabuto
            new(B2W2) { Gift = true, Species = 142, Level = 25, Location = 007 }, // Aerodactyl
            new(B2W2) { Gift = true, Species = 345, Level = 25, Location = 007 }, // Lileep
            new(B2W2) { Gift = true, Species = 347, Level = 25, Location = 007 }, // Anorith
            new(B2W2) { Gift = true, Species = 408, Level = 25, Location = 007 }, // Cranidos
            new(B2W2) { Gift = true, Species = 410, Level = 25, Location = 007 }, // Shieldon
            new(B2W2) { Gift = true, Species = 564, Level = 25, Location = 007 }, // Tirtouga
            new(B2W2) { Gift = true, Species = 566, Level = 25, Location = 007 }, // Archen

            // Gift
            new(B2W2) { Gift = true, Species = 133, Level = 10, Location = 008, Ability = 4, }, // HA Eevee @ Castelia City
            new(B2W2) { Gift = true, Species = 585, Level = 30, Location = 019, Ability = 4, Form = 0, }, // HA Deerling @ Route 6
            new(B2W2) { Gift = true, Species = 585, Level = 30, Location = 019, Ability = 4, Form = 1, }, // HA Deerling @ Route 6
            new(B2W2) { Gift = true, Species = 585, Level = 30, Location = 019, Ability = 4, Form = 2, }, // HA Deerling @ Route 6
            new(B2W2) { Gift = true, Species = 585, Level = 30, Location = 019, Ability = 4, Form = 3, }, // HA Deerling @ Route 6
            new(B2  ) { Gift = true, Species = 443, Level = 01, Location = 122, Shiny = Shiny.Always, Gender = 0, }, // Shiny Gible @ Floccesy Town
            new(  W2) { Gift = true, Species = 147, Level = 01, Location = 122, Shiny = Shiny.Always, Gender = 0, }, // Shiny Dratini @ Floccesy Town
            new(B2W2) { Gift = true, Species = 129, Level = 05, Location = 068,} , // Magikarp @ Marvelous Bridge
            new(B2W2) { Gift = true, Species = 440, Level = 01, EggLocation = 60003 }, // Happiny Egg from PKMN Breeder

            // Stationary
            new(B2W2) { Species = 590, Level = 29, Location = 019, }, // Foongus @ Route 6
            new(B2W2) { Species = 591, Level = 43, Location = 024, }, // Amoonguss @ Route 11
            new(B2W2) { Species = 591, Level = 47, Location = 127, }, // Amoonguss @ Route 22
            new(B2W2) { Species = 591, Level = 56, Location = 128, }, // Amoonguss @ Route 23
            new(B2  ) { Species = 593, Level = 40, Location = 071, Ability = 4, Gender = 0, }, // HA Jellicent @ Undella Bay Mon Only
            new(  W2) { Species = 593, Level = 40, Location = 071, Ability = 4, Gender = 1, }, // HA Jellicent @ Undella Bay Thurs Only
            new(B2W2) { Species = 593, Level = 40, Location = 071, }, // HA Jellicent @ Undella Bay EncounterSlot collision
            new(  W2) { Species = 628, Level = 25, Location = 017, Ability = 4, Gender = 0, }, // HA Braviary @ Route 4 Mon Only
            new(B2  ) { Species = 630, Level = 25, Location = 017, Ability = 4, Gender = 1, }, // HA Mandibuzz @ Route 4 Thurs Only
            new(B2W2) { Species = 637, Level = 35, Location = 035,  }, // Volcarona @ Relic Castle
            new(B2W2) { Species = 637, Level = 65, Location = 035,  }, // Volcarona @ Relic Castle
            new(B2W2) { Species = 558, Level = 42, Location = 141,  }, // Crustle @ Seaside Cave
            new(B2W2) { Species = 612, Level = 60, Location = 147, Shiny = Shiny.Always}, // Haxorus @ Nature Preserve

            // Stationary Legendary
            new(B2W2) { Species = 377, Level = 65, Location = 150, }, // Regirock @ Rock Peak Chamber
            new(B2W2) { Species = 378, Level = 65, Location = 151, }, // Regice @ Iceberg Chamber
            new(B2W2) { Species = 379, Level = 65, Location = 152, }, // Registeel @ Iron Chamber
            new(  W2) { Species = 380, Level = 68, Location = 032, }, // Latias @ Dreamyard
            new(B2  ) { Species = 381, Level = 68, Location = 032, }, // Latios @ Dreamyard
            new(B2W2) { Species = 480, Level = 65, Location = 007, }, // Uxie @ Nacrene City
            new(B2W2) { Species = 481, Level = 65, Location = 056, }, // Mesprit @ Celestial Tower
            new(B2W2) { Species = 482, Level = 65, Location = 128, }, // Azelf @ Route 23
            new(B2W2) { Species = 485, Level = 68, Location = 132, }, // Heatran @ Reversal Mountain
            new(B2W2) { Species = 486, Level = 68, Location = 038, }, // Regigigas @ Twist Mountain
            new(B2W2) { Species = 488, Level = 68, Location = 068, }, // Cresselia @ Marvelous Bridge
            new(B2W2) { Species = 638, Level = 45, Location = 026, }, // Cobalion @ Route 13
            new(B2W2) { Species = 638, Level = 65, Location = 026, }, // Cobalion @ Route 13
            new(B2W2) { Species = 639, Level = 45, Location = 127, }, // Terrakion @ Route 22
            new(B2W2) { Species = 639, Level = 65, Location = 127, }, // Terrakion @ Route 22
            new(B2W2) { Species = 640, Level = 45, Location = 024, }, // Virizion @ Route 11
            new(B2W2) { Species = 640, Level = 65, Location = 024, }, // Virizion @ Route 11
            new(  W2) { Species = 643, Level = 70, Location = 039, Shiny = Shiny.Never }, // Reshiram @ Dragonspiral Tower
            new(B2  ) { Species = 644, Level = 70, Location = 039, Shiny = Shiny.Never }, // Zekrom @ Dragonspiral Tower
            new(B2W2) { Species = 646, Level = 70, Location = 061, Form = 0 }, // Kyurem @ Giant Chasm
        };

        private static readonly EncounterStatic5N[] Encounter_B2W2_N =
        {
            // N's Pokemon
            new(0xFF01007F) { Species = 509, Level = 07, Location = 015, Ability = 2, Nature = Nature.Timid }, // Purloin @ Route 2
            new(0xFF01007F) { Species = 519, Level = 13, Location = 033, Ability = 2, Nature = Nature.Sassy }, // Pidove @ Pinwheel Forest
            new(0xFF00003F) { Species = 532, Level = 13, Location = 033, Ability = 1, Nature = Nature.Rash }, // Timburr @ Pinwheel Forest
            new(0xFF01007F) { Species = 535, Level = 13, Location = 033, Ability = 2, Nature = Nature.Modest }, // Tympole @ Pinwheel Forest
            new(0xFF00007F) { Species = 527, Level = 55, Location = 053, Ability = 1, Nature = Nature.Timid }, // Woobat @ Wellspring Cave
            new(0xFF01007F) { Species = 551, Level = 22, Location = 034, Ability = 2, Nature = Nature.Docile }, // Sandile @ Desert Resort
            new(0xFF00007F) { Species = 554, Level = 22, Location = 034, Ability = 1, Nature = Nature.Naive }, // Darumaka @ Desert Resort
            new(0xFF00007F) { Species = 555, Level = 35, Location = 034, Ability = 4, Nature = Nature.Calm }, // Darmanitan @ Desert Resort
            new(0xFF00007F) { Species = 559, Level = 22, Location = 034, Ability = 1, Nature = Nature.Lax }, // Scraggy @ Desert Resort
            new(0xFF01007F) { Species = 561, Level = 22, Location = 034, Ability = 2, Nature = Nature.Gentle }, // Sigilyph @ Desert Resort
            new(0xFF00007F) { Species = 525, Level = 28, Location = 037, Ability = 1, Nature = Nature.Naive }, // Boldore @ Chargestone Cave
            new(0xFF01007F) { Species = 595, Level = 28, Location = 037, Ability = 2, Nature = Nature.Docile }, // Joltik @ Chargestone Cave
            new(0xFF00007F) { Species = 597, Level = 28, Location = 037, Ability = 1, Nature = Nature.Bashful }, // Ferroseed @ Chargestone Cave
            new(0xFF000000) { Species = 599, Level = 28, Location = 037, Ability = 1, Nature = Nature.Rash }, // Klink @ Chargestone Cave
            new(0xFF00001F) { Species = 570, Level = 25, Location = 010, Ability = 1, Nature = Nature.Hasty, Gift = true } // N's Zorua @ Driftveil City
        };

        private static readonly EncounterStatic5[] Encounter_B2W2 = ArrayUtil.ConcatAll(Encounter_B2W2_Regular, Encounter_B2W2_N, Encounter_DreamRadar);

        #endregion
        #region Trade Tables

        internal static readonly EncounterTrade5PID[] TradeGift_BW =
        {
            new(B , 0x64000000) { Species = 548, Level = 15, Ability = 1, TID = 39922, SID = 00000, OTGender = 1, Gender = 1, IVs = new[] {20,20,20,20,31,20}, Nature = Nature.Modest,   }, // Petilil
            new( W, 0x6400007E) { Species = 546, Level = 15, Ability = 1, TID = 39922, SID = 00000, OTGender = 1, Gender = 1, IVs = new[] {20,20,20,20,31,20}, Nature = Nature.Modest,   }, // Cottonee
            new(B , 0x9400007F) { Species = 550, Level = 25, Ability = 1, TID = 27646, SID = 00000, OTGender = 0, Gender = 0, IVs = new[] {20,31,20,20,20,20}, Nature = Nature.Adamant, Form = 0, }, // Basculin-Red
            new( W, 0x9400007F) { Species = 550, Level = 25, Ability = 1, TID = 27646, SID = 00000, OTGender = 0, Gender = 0, IVs = new[] {20,31,20,20,20,20}, Nature = Nature.Adamant, Form = 1, }, // Basculin-Blue
            new(BW, 0xD400007F) { Species = 587, Level = 30, Ability = 1, TID = 11195, SID = 00000, OTGender = 0, Gender = 0, IVs = new[] {20,20,31,20,20,20}, Nature = Nature.Lax,      }, // Emolga
            new(BW, 0x2A000000) { Species = 479, Level = 60, Ability = 1, TID = 54673, SID = 00000, OTGender = 1, Gender = 2, IVs = new[] {20,20,20,20,20,31}, Nature = Nature.Gentle,   }, // Rotom
            new(BW, 0x6200001F) { Species = 446, Level = 60, Ability = 2, TID = 40217, SID = 00000, OTGender = 0, Gender = 0, IVs = new[] {31,20,20,20,20,20}, Nature = Nature.Serious,  }, // Munchlax
        };

        internal static readonly EncounterTrade5[] TradeGift_B2W2_Regular =
        {
            new(B2  ) { Species = 548, Level = 20, Ability = 2, TID = 65217, SID = 00000, OTGender = 1, Gender = 1, IVs = new[] {20,20,20,20,31,20}, Nature = Nature.Timid,   }, // Petilil
            new(  W2) { Species = 546, Level = 20, Ability = 1, TID = 05720, SID = 00001, OTGender = 0, Gender = 0, IVs = new[] {20,20,20,20,31,20}, Nature = Nature.Modest,  }, // Cottonee
            new(B2W2) { Species = 526, Level = 35, Ability = 1, TID = 11195, SID = 00000, OTGender = 0, Gender = 0, IVs = new[] {20,31,20,20,20,20}, Nature = Nature.Adamant, IsNicknamed = false }, // Gigalith
            new(B2W2) { Species = 465, Level = 45, Ability = 1, TID = 27658, SID = 00001, OTGender = 0, Gender = 0, IVs = new[] {31,20,20,20,20,20}, Nature = Nature.Hardy,   }, // Tangrowth
            new(B2W2) { Species = 479, Level = 60, Ability = 1, TID = 54673, SID = 00000, OTGender = 1, Gender = 2, IVs = new[] {20,20,20,20,20,31}, Nature = Nature.Calm,    }, // Rotom
            new(B2W2) { Species = 424, Level = 40, Ability = 2, TID = 17074, SID = 00001, OTGender = 1, Gender = 0, IVs = new[] {20,20,20,31,20,20}, Nature = Nature.Jolly,   }, // Ambipom
            new(B2W2) { Species = 065, Level = 40, Ability = 1, TID = 17074, SID = 00001, OTGender = 1, Gender = 0, IVs = new[] {20,20,20,31,20,20}, Nature = Nature.Timid,   }, // Alakazam
        };

        internal const int YancyTID = 10303;
        internal const int CurtisTID = 54118;
        private static readonly string[] TradeOT_B2W2_F = { string.Empty, "ルリ", "Yancy", "Brenda", "Lilì", "Sabine", string.Empty, "Belinda", "루리" };
        private static readonly string[] TradeOT_B2W2_M = { string.Empty, "テツ", "Curtis", "Julien", "Dadi", "Markus", string.Empty, "Julián", "철권" };

        private static readonly EncounterTrade5[] TradeGift_B2W2_YancyCurtis =
        {
            // Player is Male
            new(B2W2) { Species = 052, Level = 50, Ability = 4, TID = 10303, SID = 00000, OTGender = 1, TrainerNames = TradeOT_B2W2_F, }, // Meowth
            new(B2W2) { Species = 202, Level = 50, Ability = 4, TID = 10303, SID = 00000, OTGender = 1, TrainerNames = TradeOT_B2W2_F, }, // Wobbuffet
            new(B2W2) { Species = 280, Level = 50, Ability = 4, TID = 10303, SID = 00000, OTGender = 1, TrainerNames = TradeOT_B2W2_F, }, // Ralts
            new(B2W2) { Species = 410, Level = 50, Ability = 4, TID = 10303, SID = 00000, OTGender = 1, TrainerNames = TradeOT_B2W2_F, }, // Shieldon
            new(B2W2) { Species = 111, Level = 50, Ability = 4, TID = 10303, SID = 00000, OTGender = 1, TrainerNames = TradeOT_B2W2_F, }, // Rhyhorn
            new(B2W2) { Species = 422, Level = 50, Ability = 4, TID = 10303, SID = 00000, OTGender = 1, TrainerNames = TradeOT_B2W2_F, Form = 0, }, // Shellos-West
            new(B2W2) { Species = 303, Level = 50, Ability = 4, TID = 10303, SID = 00000, OTGender = 1, TrainerNames = TradeOT_B2W2_F, }, // Mawile
            new(B2W2) { Species = 442, Level = 50, Ability = 4, TID = 10303, SID = 00000, OTGender = 1, TrainerNames = TradeOT_B2W2_F, }, // Spiritomb
            new(B2W2) { Species = 143, Level = 50, Ability = 4, TID = 10303, SID = 00000, OTGender = 1, TrainerNames = TradeOT_B2W2_F, }, // Snorlax
            new(B2W2) { Species = 216, Level = 50, Ability = 4, TID = 10303, SID = 00000, OTGender = 1, TrainerNames = TradeOT_B2W2_F, }, // Teddiursa
            new(B2W2) { Species = 327, Level = 50, Ability = 4, TID = 10303, SID = 00000, OTGender = 1, TrainerNames = TradeOT_B2W2_F, }, // Spinda
            new(B2W2) { Species = 175, Level = 50, Ability = 4, TID = 10303, SID = 00000, OTGender = 1, TrainerNames = TradeOT_B2W2_F, }, // Togepi

            // Player is Female
            new(B2W2) { Species = 056, Level = 50, Ability = 4, TID = 54118, SID = 00000, OTGender = 0, TrainerNames = TradeOT_B2W2_M, }, // Mankey
            new(B2W2) { Species = 202, Level = 50, Ability = 4, TID = 54118, SID = 00000, OTGender = 0, TrainerNames = TradeOT_B2W2_M, }, // Wobbuffet
            new(B2W2) { Species = 280, Level = 50, Ability = 4, TID = 54118, SID = 00000, OTGender = 0, TrainerNames = TradeOT_B2W2_M, }, // Ralts
            new(B2W2) { Species = 408, Level = 50, Ability = 4, TID = 54118, SID = 00000, OTGender = 0, TrainerNames = TradeOT_B2W2_M, }, // Cranidos
            new(B2W2) { Species = 111, Level = 50, Ability = 4, TID = 54118, SID = 00000, OTGender = 0, TrainerNames = TradeOT_B2W2_M, }, // Rhyhorn
            new(B2W2) { Species = 422, Level = 50, Ability = 4, TID = 54118, SID = 00000, OTGender = 0, TrainerNames = TradeOT_B2W2_M, Form = 1, }, // Shellos-East
            new(B2W2) { Species = 302, Level = 50, Ability = 4, TID = 54118, SID = 00000, OTGender = 0, TrainerNames = TradeOT_B2W2_M, }, // Sableye
            new(B2W2) { Species = 442, Level = 50, Ability = 4, TID = 54118, SID = 00000, OTGender = 0, TrainerNames = TradeOT_B2W2_M, }, // Spiritomb
            new(B2W2) { Species = 143, Level = 50, Ability = 4, TID = 54118, SID = 00000, OTGender = 0, TrainerNames = TradeOT_B2W2_M, }, // Snorlax
            new(B2W2) { Species = 231, Level = 50, Ability = 4, TID = 54118, SID = 00000, OTGender = 0, TrainerNames = TradeOT_B2W2_M, }, // Phanpy
            new(B2W2) { Species = 327, Level = 50, Ability = 4, TID = 54118, SID = 00000, OTGender = 0, TrainerNames = TradeOT_B2W2_M, }, // Spinda
            new(B2W2) { Species = 175, Level = 50, Ability = 4, TID = 54118, SID = 00000, OTGender = 0, TrainerNames = TradeOT_B2W2_M, }, // Togepi
        };

        private const string tradeBW = "tradebw";
        private const string tradeB2W2 = "tradeb2w2";
        private static readonly string[][] TradeBW = Util.GetLanguageStrings8(tradeBW);
        private static readonly string[][] TradeB2W2 = Util.GetLanguageStrings8(tradeB2W2);

        internal static readonly EncounterTrade5[] TradeGift_B2W2 = ArrayUtil.ConcatAll(TradeGift_B2W2_Regular, TradeGift_B2W2_YancyCurtis);

        #endregion

        internal static readonly EncounterStatic5[] StaticB = ArrayUtil.ConcatAll(GetEncounters(Encounter_BW, B), DreamWorld_Common, DreamWorld_BW);
        internal static readonly EncounterStatic5[] StaticW = ArrayUtil.ConcatAll(GetEncounters(Encounter_BW, W), DreamWorld_Common, DreamWorld_BW);
        internal static readonly EncounterStatic5[] StaticB2 = ArrayUtil.ConcatAll(GetEncounters(Encounter_B2W2, B2), DreamWorld_Common, DreamWorld_B2W2);
        internal static readonly EncounterStatic5[] StaticW2 = ArrayUtil.ConcatAll(GetEncounters(Encounter_B2W2, W2), DreamWorld_Common, DreamWorld_B2W2);
    }
}
