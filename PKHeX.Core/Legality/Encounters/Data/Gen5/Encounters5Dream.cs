using System.Collections.Generic;
using static PKHeX.Core.GameVersion;
using static PKHeX.Core.GlobalLinkPromotion;

namespace PKHeX.Core;

/// <summary>
/// Generation 5 Encounters for Dream Radar and Dream World
/// </summary>
public static class Encounters5Dream
{
    /// <summary>
    /// Fetches all possible encounters for the requested version group.
    /// </summary>
    /// <param name="isBlackWhite">Is <see cref="GameVersion.B"/> or <see cref="GameVersion.W"/>.</param>
    /// <param name="includePromo">Include PGL promo encounters.</param>
    /// <param name="allowDemoBypass">Allow demo encounters bypass for B2/W2.</param>
    /// <returns>All possible encounters for the requested version group.</returns>
    public static List<EncounterStatic5Entree> GetAll(bool isBlackWhite, bool includePromo = false, bool allowDemoBypass = false)
    {
        var result = new List<EncounterStatic5Entree>(Common);

        var versionExclusives = isBlackWhite ? BW : B2W2;
        result.AddRange(versionExclusives);

        if (isBlackWhite || allowDemoBypass)
            result.AddRange(Demo);

        if (!includePromo)
            result.RemoveAll(static z => z.Promotion != NotPromotion);

        return result;
    }

    #region DreamWorld Encounters

    internal static readonly EncounterStatic5Entree[] Common = DreamWorldEntry.GetArray(Gen5,
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

        // PGL (Both Versions)
        new(149, 55, 009, Gender: 0, Promotion: Dragonite1), // JPN/KOR ThunderPunch
        new(149, 55, 245, Gender: 0, Promotion: Dragonite2), // INT ExtremeSpeed
        new(212, 10, 211, Gender: 0, Promotion: Scizor), // INT Steel Wing
        new(248, 55, 069, Gender: 0, Promotion: Tyranitar), // INT Seismic Toss
        new(376, 45, 038, Gender: 2, Promotion: Metagross), // INT Double-Edge
        new(445, 48, 242, Gender: 0, Promotion: Garchomp), // INT Crunch
    ]);

    /// <summary>
    /// B/W Dream World encounter data normally exclusive to B/W, but can later link to B2/W2 via Demo accounts.
    /// </summary>
    /// <remarks>
    /// Normally these should be received on B/W, so treat any B2/W2 redemption with suspicion.
    /// See <see cref="BW"/> for the list of encounters that the Dream World Demo accounts could not receive.
    /// </remarks>
    internal static readonly EncounterStatic5Entree[] Demo = DreamWorldEntry.GetArray(Gen5,
    [
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
        
        // PGL: external Pokémon Trainer Club (PTC) or Daisuki Club, no Game Sync ID required.
        // To actually send to your Dream World Friend Board you had to log into the PGL Promotions tab.
        // Trial Mode accounts could access the Promotions tab:
        // https://www.smogon.com/forums/threads/list-of-released-dream-world-pokemon-mark-ii.3454925/post-4429161
        new(134, 10, Gender: 0, Promotion: Eeveelutions), // Vaporeon
        new(135, 10, Gender: 0, Promotion: Eeveelutions), // Jolteon
        new(136, 10, Gender: 0, Promotion: Eeveelutions), // Flareon
        new(196, 10, Gender: 0, Promotion: Eeveelutions), // Espeon
        new(197, 10, Gender: 0, Promotion: Eeveelutions), // Umbreon
        new(470, 10, Gender: 0, Promotion: Eeveelutions), // Leafeon
        new(471, 10, Gender: 0, Promotion: Eeveelutions), // Glaceon
        new(468, 10, 217, Gender: 0, Promotion: Togekiss),

        // PGL: Serial Code
        new(001, 10, Gender: 0, Promotion: StartersKanto), // Bulbasaur
        new(004, 10, Gender: 0, Promotion: StartersKanto), // Charmander
        new(007, 10, Gender: 0, Promotion: StartersKanto), // Squirtle
        new(387, 10, Gender: 0, Promotion: StartersSinnoh), // Turtwig
        new(390, 10, Gender: 0, Promotion: StartersSinnoh), // Chimchar
        new(393, 10, Gender: 0, Promotion: StartersSinnoh), // Piplup
        new(252, 10, Gender: 0, Promotion: StartersHoenn), // Treecko
        new(255, 10, Gender: 0, Promotion: StartersHoenn), // Torchic
        new(258, 10, Gender: 0, Promotion: StartersHoenn), // Mudkip
        new(473, 34, Gender: 0, Promotion: Mamoswine),
        new(453, 10, Gender: 0, Promotion: Croagunk1), // JPN/KOR

        // PGL - Single Gender implied
        new(493, 100, Promotion: Arceus),
        new(137, 10, Promotion: Porygon),
        new(384, 50, Promotion: Rayquaza),
        new(242, 10, Promotion: Blissey),

        // PGL - Now with moves!
        new(453, 10, 398, Gender: 0, Promotion: Croagunk2), // INT
        new(189, 27, 206, Gender: 0, Promotion: Jumpluff),
        new(334, 35, 206, Gender: 0, Promotion: Altaria),
        new(354, 37, 538, Gender: 1, Promotion: Banette),
        new(448, 10, 418, Gender: 0, Promotion: Lucario),
    ]);

    /// <summary>
    /// B/W exclusive (cartridge-linked) Dream World encounter data.
    /// Sparkling Sea and Windswept Sky required badges, thus required a game to be linked.
    /// </summary>
    internal static readonly EncounterStatic5Entree[] BW = DreamWorldEntry.GetArray(GameVersion.BW,
    [
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
    ]);

    /// <summary>
    /// Dream World Encounters exclusive to B2/W2.
    /// </summary>
    internal static readonly EncounterStatic5Entree[] B2W2 = DreamWorldEntry.GetArray(GameVersion.B2W2,
    [
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
        new(025, 10, 029, Gender: 0, Promotion: Pikachu),
        new(387, 10, 254, Gender: 0, Promotion: StartersSinnoh2), // Turtwig
        new(390, 10, 252, Gender: 0, Promotion: StartersSinnoh2), // Chimchar
        new(393, 10, 297, Gender: 0, Promotion: StartersSinnoh2), // Piplup
        new(511, 10, 437, Gender: 0, Promotion: MonkeyUnova), // Pansage
        new(513, 10, 257, Gender: 0, Promotion: MonkeyUnova), // Pansear
        new(515, 10, 056, Gender: 0, Promotion: MonkeyUnova), // Panpour
        new(575, 32, 243, Gender: 0, Promotion: Gothorita1), // Mirror Coat
        new(575, 32, 286, Gender: 0, Promotion: Gothorita2), // Imprison
    ]);
    #endregion
}
