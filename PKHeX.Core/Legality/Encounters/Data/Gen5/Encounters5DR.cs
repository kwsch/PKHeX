using static PKHeX.Core.AbilityPermission;
using static PKHeX.Core.GameVersion;

namespace PKHeX.Core;

/// <summary>
/// Generation 5 Encounters
/// </summary>
public static class Encounters5DR
{
    #region Dream Radar Tables

    internal static readonly EncounterStatic5Radar[] Encounter_DreamRadar =
    [
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
        new(479, 0, Any12), // Rotom (no HA)
        new(483, 0), // Dialga (Diamond cart)
        new(484, 0), // Palkia (Pearl cart)
        new(487, 0), // Giratina (Platinum cart)
        new(517, 0), // Munna
        new(561, 0), // Sigilyph
        new(641, 1), // Therian Tornadus
        new(642, 1), // Therian Thundurus
        new(645, 1), // Therian Landorus
    ];

    #endregion
    #region DreamWorld Encounter

    public static readonly EncounterStatic5Entree[] DreamWorld_Common = DreamWorldEntry.GetArray(Gen5,
    [
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
    ]);

    #endregion
}
