using static PKHeX.Core.Encounters3Teams;

namespace PKHeX.Core;

internal static class Encounters3GC
{
    #region Colosseum

    private static readonly EncounterStatic3[] Encounter_ColoGift =
    {
        // Colosseum Starters: Gender locked to male
        new(196, 25, GameVersion.COLO) { Gift = true, Location = 254, Gender = 0 }, // Espeon
        new(197, 26, GameVersion.COLO) { Gift = true, Location = 254, Gender = 0, Moves = new(044) }, // Umbreon (Bite)
    };

    private static readonly EncounterStaticShadow[] Encounter_Colo =
    {
        new(GameVersion.COLO, 01, 03000, ColoMakuhita) { Species = 296, Level = 30, Moves = new(193,116,233,238), Location = 005 }, // Makuhita: Miror B.Peon Trudly @ Phenac City

        new(GameVersion.COLO, 02, 03000, First)     { Species = 153, Level = 30, Moves = new(241,235,075,034), Location = 003  }, // Bayleef: Cipher Peon Verde @ Phenac City
        new(GameVersion.COLO, 02, 03000, First)     { Species = 153, Level = 30, Moves = new(241,235,075,034), Location = 069  }, // Bayleef: Cipher Peon Verde @ Shadow PKMN Lab
        new(GameVersion.COLO, 02, 03000, First)     { Species = 153, Level = 30, Moves = new(241,235,075,034), Location = 115  }, // Bayleef: Cipher Peon Verde @ Realgam Tower
        new(GameVersion.COLO, 02, 03000, First)     { Species = 153, Level = 30, Moves = new(241,235,075,034), Location = 132  }, // Bayleef: Cipher Peon Verde @ Snagem Hideout
        new(GameVersion.COLO, 03, 03000, First)     { Species = 156, Level = 30, Moves = new(241,108,091,172), Location = 003  }, // Quilava: Cipher Peon Rosso @ Phenac City
        new(GameVersion.COLO, 03, 03000, First)     { Species = 156, Level = 30, Moves = new(241,108,091,172), Location = 069  }, // Quilava: Cipher Peon Rosso @ Shadow PKMN Lab
        new(GameVersion.COLO, 03, 03000, First)     { Species = 156, Level = 30, Moves = new(241,108,091,172), Location = 115  }, // Quilava: Cipher Peon Rosso @ Realgam Tower
        new(GameVersion.COLO, 03, 03000, First)     { Species = 156, Level = 30, Moves = new(241,108,091,172), Location = 132  }, // Quilava: Cipher Peon Rosso @ Snagem Hideout
        new(GameVersion.COLO, 04, 03000, First)     { Species = 159, Level = 30, Moves = new(240,184,044,057), Location = 003  }, // Croconaw: Cipher Peon Bluno @ Phenac City
        new(GameVersion.COLO, 04, 03000, First)     { Species = 159, Level = 30, Moves = new(240,184,044,057), Location = 069  }, // Croconaw: Cipher Peon Bluno @ Shadow PKMN Lab
        new(GameVersion.COLO, 04, 03000, First)     { Species = 159, Level = 30, Moves = new(240,184,044,057), Location = 115  }, // Croconaw: Cipher Peon Bluno @ Realgam Tower
        new(GameVersion.COLO, 04, 03000, First)     { Species = 159, Level = 30, Moves = new(240,184,044,057), Location = 132  }, // Croconaw: Cipher Peon Bluno @ Snagem Hideout
        new(GameVersion.COLO, 05, 03000, First)     { Species = 164, Level = 30, Moves = new(211,095,115,019), Location = 015 }, // Noctowl: Rider Nover @ Pyrite Town
        new(GameVersion.COLO, 06, 03000, First)     { Species = 180, Level = 30, Moves = new(085,086,178,084), Location = 015 }, // Flaaffy: St.Performer Diogo @ Pyrite Town
        new(GameVersion.COLO, 07, 03000, First)     { Species = 188, Level = 30, Moves = new(235,079,178,072), Location = 015 }, // Skiploom: Rider Leba @ Pyrite Town
        new(GameVersion.COLO, 08, 04000, First)     { Species = 195, Level = 30, Moves = new(341,133,021,057), Location = 015 }, // Quagsire: Bandana Guy Divel @ Pyrite Town
        new(GameVersion.COLO, 09, 04000, First)     { Species = 200, Level = 30, Moves = new(060,109,212,247), Location = 015 }, // Misdreavus: Rider Vant @ Pyrite Town
        new(GameVersion.COLO, 10, 05000, First)     { Species = 193, Level = 33, Moves = new(197,048,049,253), Location = 025  }, // Yanma: Cipher Peon Nore @ Pyrite Bldg
        new(GameVersion.COLO, 10, 05000, First)     { Species = 193, Level = 33, Moves = new(197,048,049,253), Location = 132  }, // Yanma: Cipher Peon Nore @ Snagem Hideout
        new(GameVersion.COLO, 11, 05000, First)     { Species = 162, Level = 33, Moves = new(231,270,098,070), Location = 015  }, // Furret: Rogue Cail @ Pyrite Town
        new(GameVersion.COLO, 12, 04000, First)     { Species = 218, Level = 30, Moves = new(241,281,088,053), Location = 015  }, // Slugma: Roller Boy Lon @ Pyrite Town
        new(GameVersion.COLO, 13, 04000, First)     { Species = 223, Level = 20, Moves = new(061,199,060,062), Location = 028 }, // Remoraid: Miror B.Peon Reath @ Pyrite Bldg
        new(GameVersion.COLO, 13, 04000, First)     { Species = 223, Level = 20, Moves = new(061,199,060,062), Location = 030 }, // Remoraid: Miror B.Peon Reath @ Pyrite Cave
        new(GameVersion.COLO, 14, 05000, First)     { Species = 226, Level = 33, Moves = new(017,048,061,036), Location = 028 }, // Mantine: Miror B.Peon Ferma @ Pyrite Bldg
        new(GameVersion.COLO, 14, 05000, First)     { Species = 226, Level = 33, Moves = new(017,048,061,036), Location = 030 }, // Mantine: Miror B.Peon Ferma @ Pyrite Cave
        new(GameVersion.COLO, 15, 05000, First)     { Species = 211, Level = 33, Moves = new(042,107,040,057), Location = 015 }, // Qwilfish: Hunter Doken @ Pyrite Bldg
        new(GameVersion.COLO, 16, 05000, First)     { Species = 307, Level = 33, Moves = new(197,347,093,136), Location = 031 }, // Meditite: Rider Twan @ Pyrite Cave
        new(GameVersion.COLO, 17, 05000, First)     { Species = 206, Level = 33, Moves = new(180,137,281,036), Location = 029 }, // Dunsparce: Rider Sosh @ Pyrite Cave
        new(GameVersion.COLO, 18, 05000, First)     { Species = 333, Level = 33, Moves = new(119,047,219,019), Location = 032 }, // Swablu: Hunter Zalo @ Pyrite Cave
        new(GameVersion.COLO, 19, 10000, First)     { Species = 185, Level = 35, Moves = new(175,335,067,157), Location = 104 }, // Sudowoodo: Cipher Admin Miror B. @ Realgam Tower
        new(GameVersion.COLO, 19, 10000, First)     { Species = 185, Level = 35, Moves = new(175,335,067,157), Location = 125 }, // Sudowoodo: Cipher Admin Miror B. @ Deep Colosseum
        new(GameVersion.COLO, 19, 10000, First)     { Species = 185, Level = 35, Moves = new(175,335,067,157), Location = 030 }, // Sudowoodo: Cipher Admin Miror B. @ Pyrite Cave
        new(GameVersion.COLO, 20, 06000, First)     { Species = 237, Level = 38, Moves = new(097,116,167,229), Location = 039 }, // Hitmontop: Cipher Peon Skrub @ Agate Village
        new(GameVersion.COLO, 20, 06000, First)     { Species = 237, Level = 38, Moves = new(097,116,167,229), Location = 132 }, // Hitmontop: Cipher Peon Skrub @ Snagem Hideout
        new(GameVersion.COLO, 20, 06000, First)     { Species = 237, Level = 38, Moves = new(097,116,167,229), Location = 068 }, // Hitmontop: Cipher Peon Skrub @ Shadow PKMN Lab
        new(GameVersion.COLO, 21, 13000, First)     { Species = 244, Level = 40, Moves = new(241,043,044,126), Location = 106 }, // Entei: Cipher Admin Dakim @ Realgam Tower
        new(GameVersion.COLO, 21, 13000, First)     { Species = 244, Level = 40, Moves = new(241,043,044,126), Location = 125 }, // Entei: Cipher Admin Dakim @ Deep Colosseum
        new(GameVersion.COLO, 21, 13000, First)     { Species = 244, Level = 40, Moves = new(241,043,044,126), Location = 076 }, // Entei: Cipher Admin Dakim @ Mt. Battle
        new(GameVersion.COLO, 22, 06000, First)     { Species = 166, Level = 40, Moves = new(226,219,048,004), Location = 047 }, // Ledian: Cipher Peon Kloak @ The Under
        new(GameVersion.COLO, 22, 06000, First)     { Species = 166, Level = 40, Moves = new(226,219,048,004), Location = 132 }, // Ledian: Cipher Peon Kloak @ Snagem Hideout
        new(GameVersion.COLO, 23, 13000, First)     { Species = 245, Level = 40, Moves = new(240,043,016,057), Location = 110 }, // Suicune (Surf): Cipher Admin Venus @ Realgam Tower
        new(GameVersion.COLO, 23, 13000, First)     { Species = 245, Level = 40, Moves = new(240,043,016,056), Location = 125 }, // Suicune (Hydro Pump): Cipher Admin Venus @ Deep Colosseum
        new(GameVersion.COLO, 23, 13000, First)     { Species = 245, Level = 40, Moves = new(240,043,016,057), Location = 055 }, // Suicune (Surf): Cipher Admin Venus @ The Under
        new(GameVersion.COLO, 24, 06000, Gligar)    { Species = 207, Level = 43, Moves = new(185,028,040,163), Location = 058 }, // Gligar: Hunter Frena @ The Under Subway
        new(GameVersion.COLO, 24, 06000, Gligar)    { Species = 207, Level = 43, Moves = new(185,028,040,163), Location = 133 }, // Gligar: Hunter Frena @ Snagem Hideout
        new(GameVersion.COLO, 25, 06000, First)     { Species = 234, Level = 43, Moves = new(310,095,043,036), Location = 058 }, // Stantler: Chaser Liaks @ The Under Subway
        new(GameVersion.COLO, 25, 06000, First)     { Species = 234, Level = 43, Moves = new(310,095,043,036), Location = 133 }, // Stantler: Chaser Liaks @ Snagem Hideout
        new(GameVersion.COLO, 25, 06000, First)     { Species = 221, Level = 43, Moves = new(203,316,091,059), Location = 058 }, // Piloswine: Bodybuilder Lonia @ The Under Subway
        new(GameVersion.COLO, 26, 06000, First)     { Species = 221, Level = 43, Moves = new(203,316,091,059), Location = 134 }, // Piloswine: Bodybuilder Lonia @ Snagem Hideout
        new(GameVersion.COLO, 27, 06000, First)     { Species = 215, Level = 43, Moves = new(185,103,154,196), Location = 058 }, // Sneasel: Rider Nelis @ The Under Subway
        new(GameVersion.COLO, 27, 06000, First)     { Species = 215, Level = 43, Moves = new(185,103,154,196), Location = 134 }, // Sneasel: Rider Nelis @ Snagem Hideout
        new(GameVersion.COLO, 28, 06000, First)     { Species = 190, Level = 43, Moves = new(226,321,154,129), Location = 067 }, // Aipom: Cipher Peon Cole @ Shadow PKMN Lab
        new(GameVersion.COLO, 29, 06000, Murkrow)   { Species = 198, Level = 43, Moves = new(185,212,101,019), Location = 067 }, // Murkrow: Cipher Peon Lare @ Shadow PKMN Lab
        new(GameVersion.COLO, 30, 06000, First)     { Species = 205, Level = 43, Moves = new(153,182,117,229), Location = 067 }, // Forretress: Cipher Peon Vana @ Shadow PKMN Lab
        new(GameVersion.COLO, 31, 06000, First)     { Species = 210, Level = 43, Moves = new(044,184,046,070), Location = 069 }, // Granbull: Cipher Peon Tanie @ Shadow PKMN Lab
        new(GameVersion.COLO, 32, 06000, First)     { Species = 329, Level = 43, Moves = new(242,103,328,225), Location = 068 }, // Vibrava: Cipher Peon Remil @ Shadow PKMN Lab
        new(GameVersion.COLO, 33, 06000, First)     { Species = 168, Level = 43, Moves = new(169,184,141,188), Location = 069 }, // Ariados: Cipher Peon Lesar @ Shadow PKMN Lab

        new(GameVersion.COLO, 34, 13000, First)     { Species = 243, Level = 40, Moves = new(240,043,098,087), Location = 113 }, // Raikou: Cipher Admin Ein @ Realgam Tower
        new(GameVersion.COLO, 34, 13000, First)     { Species = 243, Level = 40, Moves = new(240,043,098,087), Location = 125 }, // Raikou: Cipher Admin Ein @ Deep Colosseum
        new(GameVersion.COLO, 34, 13000, First)     { Species = 243, Level = 40, Moves = new(240,043,098,087), Location = 069 }, // Raikou: Cipher Admin Ein @ Shadow PKMN Lab

        new(GameVersion.COLO, 35, 07000, First)     { Species = 192, Level = 45, Moves = new(241,074,275,076), Location = 109 }, // Sunflora: Cipher Peon Baila @ Realgam Tower
        new(GameVersion.COLO, 35, 07000, First)     { Species = 192, Level = 45, Moves = new(241,074,275,076), Location = 132 }, // Sunflora: Cipher Peon Baila @ Snagem Hideout
        new(GameVersion.COLO, 36, 07000, First)     { Species = 225, Level = 45, Moves = new(059,213,217,019), Location = 109 }, // Delibird: Cipher Peon Arton @ Realgam Tower
        new(GameVersion.COLO, 36, 07000, First)     { Species = 225, Level = 45, Moves = new(059,213,217,019), Location = 132 }, // Delibird: Cipher Peon Arton @ Snagem Hideout
        new(GameVersion.COLO, 37, 07000, Heracross) { Species = 214, Level = 45, Moves = new(179,203,068,280), Location = 111 }, // Heracross: Cipher Peon Dioge @ Realgam Tower
        new(GameVersion.COLO, 37, 07000, Heracross) { Species = 214, Level = 45, Moves = new(179,203,068,280), Location = 132 }, // Heracross: Cipher Peon Dioge @ Snagem Hideout
        new(GameVersion.COLO, 38, 13000, First)     { Species = 227, Level = 47, Moves = new(065,319,314,211), Location = 117 }, // Skarmory: Snagem Head Gonzap @ Realgam Tower
        new(GameVersion.COLO, 38, 13000, First)     { Species = 227, Level = 47, Moves = new(065,319,314,211), Location = 133 }, // Skarmory: Snagem Head Gonzap @ Snagem Hideout

        new(GameVersion.COLO, 39, 07000, First)     { Species = 241, Level = 48, Moves = new(208,111,205,034), Location = 118 }, // Miltank: Bodybuilder Jomas @ Tower Colosseum
        new(GameVersion.COLO, 40, 07000, First)     { Species = 359, Level = 48, Moves = new(195,014,163,185), Location = 118 }, // Absol: Rider Delan @ Tower Colosseum
        new(GameVersion.COLO, 41, 07000, First)     { Species = 229, Level = 48, Moves = new(185,336,123,053), Location = 118 }, // Houndoom: Cipher Peon Nella @ Tower Colosseum
        new(GameVersion.COLO, 42, 07000, First)     { Species = 357, Level = 49, Moves = new(076,235,345,019), Location = 118 }, // Tropius: Cipher Peon Ston @ Tower Colosseum
        new(GameVersion.COLO, 43, 15000, First)     { Species = 376, Level = 50, Moves = new(063,334,232,094), Location = 118 }, // Metagross: Cipher Nascour @ Tower Colosseum
        new(GameVersion.COLO, 44, 20000, First)     { Species = 248, Level = 55, Moves = new(242,087,157,059), Location = 118 }, // Tyranitar: Cipher Head Evice @ Tower Colosseum

        new(GameVersion.COLO, 55, 07000, First)     { Species = 235, Level = 45, Moves = new(166,039,003,231), Location = 132 }, // Smeargle: Team Snagem Biden @ Snagem Hideout
        new(GameVersion.COLO, 56, 07000, Ursaring)  { Species = 217, Level = 45, Moves = new(185,313,122,163), Location = 132 }, // Ursaring: Team Snagem Agrev @ Snagem Hideout
        new(GameVersion.COLO, 57, 07000, First)     { Species = 213, Level = 45, Moves = new(219,227,156,117), Location = 125 }, // Shuckle: Deep King Agnol @ Deep Colosseum

        new(GameVersion.COLO, 67, 05000, First)     { Species = 176, Level = 20, Moves = new(118,204,186,281), Location = 001 }, // Togetic: Cipher Peon Fein @ Outskirt Stand

        new(GameVersion.COLO, 00, 00000, CTogepi)   { Species = 175, Level = 20, Moves = new(118,204,186,281), Location = 128, IVs = new(0, 0, 0, 0, 0, 0) }, // Togepi: Chaser ボデス @ Card e Room (Japanese games only)
        new(GameVersion.COLO, 00, 00000, CMareep)   { Species = 179, Level = 37, Moves = new(087,084,086,178), Location = 128, IVs = new(0, 0, 0, 0, 0, 0) }, // Mareep: Hunter ホル @ Card e Room (Japanese games only)
        new(GameVersion.COLO, 00, 00000, CScizor)   { Species = 212, Level = 50, Moves = new(210,232,014,163), Location = 128, IVs = new(0, 0, 0, 0, 0, 0) }, // Scizor: Bodybuilder ワーバン @ Card e Room (Japanese games only)
    };
    #endregion

    #region XD

    private static readonly EncounterStatic3[] Encounter_XDGift =
    {
        new(133, 10, GameVersion.XD) { Fateful = true, Gift = true, Location = 000, Moves = new(044) }, // Eevee (Bite)
        new(152, 05, GameVersion.XD) { Fateful = true, Gift = true, Location = 016, Moves = new(246,033,045,338) }, // Chikorita
        new(155, 05, GameVersion.XD) { Fateful = true, Gift = true, Location = 016, Moves = new(179,033,043,307) }, // Cyndaquil
        new(158, 05, GameVersion.XD) { Fateful = true, Gift = true, Location = 016, Moves = new(242,010,043,308) }, // Totodile
    };

    private static readonly EncounterStaticShadow[] Encounter_XD =
    {
        new(GameVersion.XD, 01, 03000, First)     { Fateful = true, Species = 216, Level = 11, Moves = new(216,287,122,232), Location = 143, Gift = true }, // Teddiursa: Cipher Peon Naps @ Pokémon HQ Lab -- treat as Gift as it can only be captured in a Poké Ball
        new(GameVersion.XD, 02, 02000, Vulpix)    { Fateful = true, Species = 037, Level = 18, Moves = new(257,204,052,091), Location = 109 }, // Vulpix: Cipher Peon Mesin @ ONBS Building
        new(GameVersion.XD, 03, 01500, Spheal)    { Fateful = true, Species = 363, Level = 17, Moves = new(062,204,055,189), Location = 011 }, // Spheal: Cipher Peon Blusix @ Cipher Lab
        new(GameVersion.XD, 03, 01500, Spheal)    { Fateful = true, Species = 363, Level = 17, Moves = new(062,204,055,189), Location = 100 }, // Spheal: Cipher Peon Blusix  @ Phenac City
        new(GameVersion.XD, 04, 01500, First)     { Fateful = true, Species = 343, Level = 17, Moves = new(317,287,189,060), Location = 011 }, // Baltoy: Cipher Peon Browsix @ Cipher Lab
        new(GameVersion.XD, 04, 01500, First)     { Fateful = true, Species = 343, Level = 17, Moves = new(317,287,189,060), Location = 096 }, // Baltoy: Cipher Peon Browsix  @ Phenac City
        new(GameVersion.XD, 05, 01500, First)     { Fateful = true, Species = 179, Level = 17, Moves = new(034,215,084,086), Location = 011 }, // Mareep: Cipher Peon Yellosix @ Cipher Lab
        new(GameVersion.XD, 05, 01500, First)     { Fateful = true, Species = 179, Level = 17, Moves = new(034,215,084,086), Location = 096 }, // Mareep: Cipher Peon Yellosix @ Phenac City
        new(GameVersion.XD, 06, 01500, Gulpin)    { Fateful = true, Species = 316, Level = 17, Moves = new(351,047,124,092), Location = 011 }, // Gulpin: Cipher Peon Purpsix @ Cipher Lab
        new(GameVersion.XD, 06, 01500, Gulpin)    { Fateful = true, Species = 316, Level = 17, Moves = new(351,047,124,092), Location = 100 }, // Gulpin: Cipher Peon Purpsix @ Phenac City
        new(GameVersion.XD, 07, 01500, Seedot)    { Fateful = true, Species = 273, Level = 17, Moves = new(202,287,331,290), Location = 011 }, // Seedot: Cipher Peon Greesix @ Cipher Lab
        new(GameVersion.XD, 07, 01500, Seedot)    { Fateful = true, Species = 273, Level = 17, Moves = new(202,287,331,290), Location = 100 }, // Seedot: Cipher Peon Greesix @ Phenac City
        new(GameVersion.XD, 08, 01500, Spinarak)  { Fateful = true, Species = 167, Level = 14, Moves = new(091,287,324,101), Location = 010 }, // Spinarak: Cipher Peon Nexir @ Cipher Lab
        new(GameVersion.XD, 09, 01500, Numel)     { Fateful = true, Species = 322, Level = 14, Moves = new(036,204,091,052), Location = 009 }, // Numel: Cipher Peon Solox @ Cipher Lab
        new(GameVersion.XD, 10, 01700, First)     { Fateful = true, Species = 318, Level = 15, Moves = new(352,287,184,044), Location = 008 }, // Carvanha: Cipher Peon Cabol @ Cipher Lab
        new(GameVersion.XD, 11, 03000, Roselia)   { Fateful = true, Species = 315, Level = 22, Moves = new(345,186,320,073), Location = 094 }, // Roselia: Cipher Peon Fasin @ Phenac City
        new(GameVersion.XD, 12, 02500, Delcatty)  { Fateful = true, Species = 301, Level = 18, Moves = new(290,186,213,351), Location = 008 }, // Delcatty: Cipher Admin Lovrina @ Cipher Lab
        new(GameVersion.XD, 13, 04000, Nosepass)  { Fateful = true, Species = 299, Level = 26, Moves = new(085,270,086,157), Location = 090 }, // Nosepass: Wanderer Miror B. @ Poké Spots
        new(GameVersion.XD, 14, 01500, First)     { Fateful = true, Species = 228, Level = 17, Moves = new(185,204,052,046), Location = 100 }, // Houndour: Cipher Peon Resix  @ Phenac City
        new(GameVersion.XD, 14, 01500, First)     { Fateful = true, Species = 228, Level = 17, Moves = new(185,204,052,046), Location = 011 }, // Houndour: Cipher Peon Resix @ Cipher Lab
        new(GameVersion.XD, 15, 02000, Makuhita)  { Fateful = true, Species = 296, Level = 18, Moves = new(280,287,292,317), Location = 109 }, // Makuhita: Cipher Peon Torkin @ ONBS Building
        new(GameVersion.XD, 16, 02200, Duskull)   { Fateful = true, Species = 355, Level = 19, Moves = new(247,270,310,109), Location = 110 }, // Duskull: Cipher Peon Lobar @ ONBS Building
        new(GameVersion.XD, 17, 02200, Ralts)     { Fateful = true, Species = 280, Level = 20, Moves = new(351,047,115,093), Location = 119 }, // Ralts: Cipher Peon Feldas @ ONBS Building
        new(GameVersion.XD, 18, 02500, Mawile)    { Fateful = true, Species = 303, Level = 22, Moves = new(206,047,011,334), Location = 111 }, // Mawile: Cipher Cmdr Exol @ ONBS Building
        new(GameVersion.XD, 19, 02500, Snorunt)   { Fateful = true, Species = 361, Level = 20, Moves = new(352,047,044,196), Location = 097 }, // Snorunt: Cipher Peon Exinn @ Phenac City
        new(GameVersion.XD, 20, 02500, Pineco)    { Fateful = true, Species = 204, Level = 20, Moves = new(042,287,191,068), Location = 096 }, // Pineco: Cipher Peon Gonrap @ Phenac City
        new(GameVersion.XD, 21, 02500, Swinub)    { Fateful = true, Species = 220, Level = 22, Moves = new(246,204,054,341), Location = 100 }, // Swinub: Cipher Peon Greck @ Phenac City
        new(GameVersion.XD, 22, 02500, Natu)      { Fateful = true, Species = 177, Level = 22, Moves = new(248,226,101,332), Location = 094 }, // Natu: Cipher Peon Eloin @ Phenac City
        new(GameVersion.XD, 23, 01800, Shroomish) { Fateful = true, Species = 285, Level = 15, Moves = new(206,287,072,078), Location = 008 }, // Shroomish: Cipher R&D Klots @ Cipher Lab
        new(GameVersion.XD, 24, 03500, Meowth)    { Fateful = true, Species = 052, Level = 22, Moves = new(163,047,006,044), Location = 094 }, // Meowth: Cipher Peon Fostin @ Phenac City
        new(GameVersion.XD, 25, 04500, Spearow)   { Fateful = true, Species = 021, Level = 22, Moves = new(206,226,043,332), Location = 107 }, // Spearow: Cipher Peon Ezin @ Phenac Stadium
        new(GameVersion.XD, 26, 03000, Grimer)    { Fateful = true, Species = 088, Level = 23, Moves = new(188,270,325,107), Location = 107 }, // Grimer: Cipher Peon Faltly @ Phenac Stadium
        new(GameVersion.XD, 27, 03500, Seel)      { Fateful = true, Species = 086, Level = 23, Moves = new(057,270,219,058), Location = 107 }, // Seel: Cipher Peon Egrog @ Phenac Stadium
        new(GameVersion.XD, 28, 05000, Lunatone)  { Fateful = true, Species = 337, Level = 25, Moves = new(094,226,240,317), Location = 107 }, // Lunatone: Cipher Admin Snattle @ Phenac Stadium
        new(GameVersion.XD, 29, 02500, Voltorb)   { Fateful = true, Species = 100, Level = 19, Moves = new(243,287,209,129), Location = 092 }, // Voltorb: Wanderer Miror B. @ Cave Poké Spot
        new(GameVersion.XD, 30, 05000, First)     { Fateful = true, Species = 335, Level = 28, Moves = new(280,287,068,306), Location = 071 }, // Zangoose: Thug Zook @ Cipher Key Lair
        new(GameVersion.XD, 31, 04000, Growlithe) { Fateful = true, Species = 058, Level = 28, Moves = new(053,204,044,036), Location = 064 }, // Growlithe: Cipher Peon Humah @ Cipher Key Lair
        new(GameVersion.XD, 32, 04000, Paras)     { Fateful = true, Species = 046, Level = 28, Moves = new(147,287,163,206), Location = 064 }, // Paras: Cipher Peon Humah @ Cipher Key Lair
        new(GameVersion.XD, 33, 04000, First)     { Fateful = true, Species = 090, Level = 29, Moves = new(036,287,057,062), Location = 065 }, // Shellder: Cipher Peon Gorog @ Cipher Key Lair
        new(GameVersion.XD, 34, 04500, First)     { Fateful = true, Species = 015, Level = 30, Moves = new(188,226,041,014), Location = 066 }, // Beedrill: Cipher Peon Lok @ Cipher Key Lair
        new(GameVersion.XD, 35, 04000, Pidgeotto) { Fateful = true, Species = 017, Level = 30, Moves = new(017,287,211,297), Location = 066 }, // Pidgeotto: Cipher Peon Lok @ Cipher Key Lair
        new(GameVersion.XD, 36, 04000, Butterfree){ Fateful = true, Species = 012, Level = 30, Moves = new(094,234,079,332), Location = 067 }, // Butterfree: Cipher Peon Targ @ Cipher Key Lair
        new(GameVersion.XD, 37, 04000, Tangela)   { Fateful = true, Species = 114, Level = 30, Moves = new(076,234,241,275), Location = 067 }, // Tangela: Cipher Peon Targ @ Cipher Key Lair
        new(GameVersion.XD, 38, 06000, Raticate)  { Fateful = true, Species = 020, Level = 34, Moves = new(162,287,184,158), Location = 076 }, // Raticate: Chaser Furgy @ Citadark Isle
        new(GameVersion.XD, 39, 04000, Venomoth)  { Fateful = true, Species = 049, Level = 32, Moves = new(318,287,164,094), Location = 070 }, // Venomoth: Cipher Peon Angic @ Cipher Key Lair
        new(GameVersion.XD, 40, 04000, Weepinbell){ Fateful = true, Species = 070, Level = 32, Moves = new(345,234,188,230), Location = 070 }, // Weepinbell: Cipher Peon Angic @ Cipher Key Lair
        new(GameVersion.XD, 41, 05000, Arbok)     { Fateful = true, Species = 024, Level = 33, Moves = new(188,287,137,044), Location = 070 }, // Arbok: Cipher Peon Smarton @ Cipher Key Lair
        new(GameVersion.XD, 42, 06000, Primeape)  { Fateful = true, Species = 057, Level = 34, Moves = new(238,270,116,179), Location = 069 }, // Primeape: Cipher Admin Gorigan @ Cipher Key Lair
        new(GameVersion.XD, 43, 05500, Hypno)     { Fateful = true, Species = 097, Level = 34, Moves = new(094,226,096,247), Location = 069 }, // Hypno: Cipher Admin Gorigan @ Cipher Key Lair
        new(GameVersion.XD, 44, 06500, Golduck)   { Fateful = true, Species = 055, Level = 33, Moves = new(127,204,244,280), Location = 088 }, // Golduck: Navigator Abson @ Citadark Isle
        new(GameVersion.XD, 45, 07000, Sableye)   { Fateful = true, Species = 302, Level = 33, Moves = new(247,270,185,105), Location = 088 }, // Sableye: Navigator Abson @ Citadark Isle
        new(GameVersion.XD, 46, 04500, Magneton)  { Fateful = true, Species = 082, Level = 30, Moves = new(038,287,240,087), Location = 067 }, // Magneton: Cipher Peon Snidle @ Cipher Key Lair
        new(GameVersion.XD, 47, 08000, Dodrio)    { Fateful = true, Species = 085, Level = 34, Moves = new(065,226,097,161), Location = 076 }, // Dodrio: Chaser Furgy @ Citadark Isle
        new(GameVersion.XD, 48, 05500, Farfetchd) { Fateful = true, Species = 083, Level = 36, Moves = new(163,226,014,332), Location = 076 }, // Farfetch'd: Cipher Admin Lovrina @ Citadark Isle
        new(GameVersion.XD, 49, 06500, Altaria)   { Fateful = true, Species = 334, Level = 36, Moves = new(225,215,076,332), Location = 076 }, // Altaria: Cipher Admin Lovrina @ Citadark Isle
        new(GameVersion.XD, 50, 06000, Kangaskhan){ Fateful = true, Species = 115, Level = 35, Moves = new(089,047,039,146), Location = 085 }, // Kangaskhan: Cipher Peon Litnar @ Citadark Isle
        new(GameVersion.XD, 51, 07000, Banette)   { Fateful = true, Species = 354, Level = 37, Moves = new(185,270,247,174), Location = 085 }, // Banette: Cipher Peon Litnar @ Citadark Isle
        new(GameVersion.XD, 52, 07000, Magmar)    { Fateful = true, Species = 126, Level = 36, Moves = new(126,266,238,009), Location = 077 }, // Magmar: Cipher Peon Grupel @ Citadark Isle
        new(GameVersion.XD, 53, 07000, Pinsir)    { Fateful = true, Species = 127, Level = 35, Moves = new(012,270,206,066), Location = 077 }, // Pinsir: Cipher Peon Grupel @ Citadark Isle
        new(GameVersion.XD, 54, 05500, Magcargo)  { Fateful = true, Species = 219, Level = 38, Moves = new(257,287,089,053), Location = 080 }, // Magcargo: Cipher Peon Kolest @ Citadark Isle
        new(GameVersion.XD, 55, 06000, Rapidash)  { Fateful = true, Species = 078, Level = 40, Moves = new(076,226,241,053), Location = 080 }, // Rapidash: Cipher Peon Kolest @ Citadark Isle
        new(GameVersion.XD, 56, 06000, Hitmonchan){ Fateful = true, Species = 107, Level = 38, Moves = new(005,270,170,327), Location = 081 }, // Hitmonchan: Cipher Peon Karbon @ Citadark Isle
        new(GameVersion.XD, 57, 07000, Hitmonlee) { Fateful = true, Species = 106, Level = 38, Moves = new(136,287,170,025), Location = 081 }, // Hitmonlee: Cipher Peon Petro @ Citadark Isle
        new(GameVersion.XD, 58, 05000, Lickitung) { Fateful = true, Species = 108, Level = 38, Moves = new(038,270,111,205), Location = 084 }, // Lickitung: Cipher Peon Geftal @ Citadark Isle
        new(GameVersion.XD, 59, 08000, Scyther)   { Fateful = true, Species = 123, Level = 40, Moves = new(013,234,318,163), Location = 084 }, // Scyther: Cipher Peon Leden @ Citadark Isle
        new(GameVersion.XD, 60, 04000, Chansey)   { Fateful = true, Species = 113, Level = 39, Moves = new(085,186,135,285), Location = 084 }, // Chansey: Cipher Peon Leden @ Citadark Isle
        new(GameVersion.XD, 60, 04000, Chansey)   { Fateful = true, Species = 113, Level = 39, Moves = new(085,186,135,285), Location = 087 }, // Chansey: Cipher Peon Leden @ Citadark Isle
        new(GameVersion.XD, 61, 07500, Solrock)   { Fateful = true, Species = 338, Level = 41, Moves = new(094,226,241,322), Location = 087 }, // Solrock: Cipher Admin Snattle @ Citadark Isle
        new(GameVersion.XD, 62, 07500, Starmie)   { Fateful = true, Species = 121, Level = 41, Moves = new(127,287,058,105), Location = 087 }, // Starmie: Cipher Admin Snattle @ Citadark Isle
        new(GameVersion.XD, 63, 07000, Electabuzz){ Fateful = true, Species = 125, Level = 43, Moves = new(238,266,086,085), Location = 087 }, // Electabuzz: Cipher Admin Ardos @ Citadark Isle
        new(GameVersion.XD, 64, 07000, First)     { Fateful = true, Species = 277, Level = 43, Moves = new(143,226,097,263), Location = 087 }, // Swellow: Cipher Admin Ardos @ Citadark Isle
        new(GameVersion.XD, 65, 09000, Snorlax)   { Fateful = true, Species = 143, Level = 43, Moves = new(090,287,174,034), Location = 087 }, // Snorlax: Cipher Admin Ardos @ Citadark Isle
        new(GameVersion.XD, 66, 07500, Poliwrath) { Fateful = true, Species = 062, Level = 42, Moves = new(056,270,240,280), Location = 087 }, // Poliwrath: Cipher Admin Gorigan @ Citadark Isle
        new(GameVersion.XD, 67, 06500, MrMime)    { Fateful = true, Species = 122, Level = 42, Moves = new(094,266,227,009), Location = 087 }, // Mr. Mime: Cipher Admin Gorigan @ Citadark Isle
        new(GameVersion.XD, 68, 05000, Dugtrio)   { Fateful = true, Species = 051, Level = 40, Moves = new(089,204,201,161), Location = 075 }, // Dugtrio: Cipher Peon Kolax @ Citadark Isle
        new(GameVersion.XD, 69, 07000, Manectric) { Fateful = true, Species = 310, Level = 44, Moves = new(087,287,240,044), Location = 073 }, // Manectric: Cipher Admin Eldes @ Citadark Isle
        new(GameVersion.XD, 70, 09000, Salamence) { Fateful = true, Species = 373, Level = 50, Moves = new(337,287,349,332), Location = 073 }, // Salamence: Cipher Admin Eldes @ Citadark Isle
        new(GameVersion.XD, 71, 06500, Marowak)   { Fateful = true, Species = 105, Level = 44, Moves = new(089,047,014,157), Location = 073 }, // Marowak: Cipher Admin Eldes @ Citadark Isle
        new(GameVersion.XD, 72, 06000, Lapras)    { Fateful = true, Species = 131, Level = 44, Moves = new(056,215,240,059), Location = 073 }, // Lapras: Cipher Admin Eldes @ Citadark Isle
        new(GameVersion.XD, 73, 12000, First)     { Fateful = true, Species = 249, Level = 50, Moves = new(354,297,089,056), Location = 074 }, // Lugia: Grand Master Greevil @ Citadark Isle
        new(GameVersion.XD, 74, 10000, Zapdos)    { Fateful = true, Species = 145, Level = 50, Moves = new(326,226,319,085), Location = 074 }, // Zapdos: Grand Master Greevil @ Citadark Isle
        new(GameVersion.XD, 75, 10000, Moltres)   { Fateful = true, Species = 146, Level = 50, Moves = new(326,234,261,053), Location = 074 }, // Moltres: Grand Master Greevil @ Citadark Isle
        new(GameVersion.XD, 76, 10000, Articuno)  { Fateful = true, Species = 144, Level = 50, Moves = new(326,215,114,058), Location = 074 }, // Articuno: Grand Master Greevil @ Citadark Isle
        new(GameVersion.XD, 77, 09000, Tauros)    { Fateful = true, Species = 128, Level = 46, Moves = new(089,287,039,034), Location = 074 }, // Tauros: Grand Master Greevil @ Citadark Isle
        new(GameVersion.XD, 78, 07000, First)     { Fateful = true, Species = 112, Level = 46, Moves = new(224,270,184,089), Location = 074 }, // Rhydon: Grand Master Greevil @ Citadark Isle
        new(GameVersion.XD, 79, 09000, Exeggutor) { Fateful = true, Species = 103, Level = 46, Moves = new(094,287,095,246), Location = 074 }, // Exeggutor: Grand Master Greevil @ Citadark Isle
        new(GameVersion.XD, 80, 09000, Dragonite) { Fateful = true, Species = 149, Level = 55, Moves = new(063,215,349,089), Location = 162 }, // Dragonite: Wanderer Miror B. @ Gateon Port
        new(GameVersion.XD, 81, 04500, First)     { Fateful = true, Species = 175, Level = 25, Moves = new(266,161,246,270), Location = 164, Gift = true }, // Togepi: Pokémon Trainer Hordel @ Outskirt Stand
        new(GameVersion.XD, 82, 02500, Poochyena) { Fateful = true, Species = 261, Level = 10, Moves = new(091,215,305,336), Location = 162 }, // Poochyena: Bodybuilder Kilen @ Gateon Port
        new(GameVersion.XD, 83, 02500, Ledyba)    { Fateful = true, Species = 165, Level = 10, Moves = new(060,287,332,048), Location = 153 }, // Ledyba: Casual Guy Cyle @ Gateon Port
    };

    internal static readonly EncounterArea3XD[] SlotsXD =
    {
        new(90, 027, 23, 207, 20, 328, 20), // Rock (Sandshrew, Gligar, Trapinch)
        new(91, 187, 20, 231, 20, 283, 20), // Oasis (Hoppip, Phanpy, Surskit)
        new(92, 041, 21, 304, 21, 194, 21), // Cave (Zubat, Aron, Wooper)
    };

    internal static readonly EncounterStatic[] Encounter_CXD = ArrayUtil.ConcatAll<EncounterStatic>(Encounter_ColoGift, Encounter_Colo, Encounter_XDGift, Encounter_XD);

    #endregion
}
