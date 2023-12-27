using static PKHeX.Core.Encounters3XDTeams;

namespace PKHeX.Core;

internal static class Encounters3XD
{
    internal static readonly EncounterStatic3XD[] Gifts =
    [
        new(133, 10) { FixedBall = Ball.Poke, FatefulEncounter = true, Location = 000, Moves = new(033,039,044,028) }, // Eevee (Bite)
        new(152, 05) { FixedBall = Ball.Poke, FatefulEncounter = true, Location = 016, Moves = new(246,033,045,338) }, // Chikorita
        new(155, 05) { FixedBall = Ball.Poke, FatefulEncounter = true, Location = 016, Moves = new(179,033,043,307) }, // Cyndaquil
        new(158, 05) { FixedBall = Ball.Poke, FatefulEncounter = true, Location = 016, Moves = new(242,010,043,308) }, // Totodile
    ];

    private static readonly string[] Hordel = [string.Empty, "ダニー", "HORDEL", "VOLKER", "ODINO", "HORAZ", string.Empty, "HORDEL"];
    private static readonly string[] Zaprong = [string.Empty, "コンセント", "ZAPRONG", "ZAPRONG", "ZAPRONG", "ZAPRONG", string.Empty, "ZAPRONG"];
    private static string[] Duking => Encounters3Colo.TrainerNameDuking;

    internal static readonly EncounterTrade3XD[] Trades =
    [
        new(239, 20, Hordel, Zaprong) { Location = 164, TID16 = 41400, Moves = new(008, 007, 009, 238) }, // Elekid @ Snagem Hideout
        new(307, 20, Duking)          { Location = 116, TID16 = 37149, Moves = new(223, 093, 247, 197) }, // Meditite @ Pyrite Town
        new(213, 20, Duking)          { Location = 116, TID16 = 37149, Moves = new(092, 164, 188, 227) }, // Shuckle @ Pyrite Town
        new(246, 20, Duking)          { Location = 116, TID16 = 37149, Moves = new(201, 349, 044, 200) }, // Larvitar @ Pyrite Town
    ];

    internal static readonly EncounterShadow3XD[] Shadow =
    [
        new(01, 03000, First)     { Species = 216, Level = 11, Moves = new(216,287,122,232), Location = 143, FixedBall = Ball.Poke }, // Teddiursa: Cipher Peon Naps @ Pokémon HQ Lab -- treat as Gift as it can only be captured in a Poké Ball
        new(02, 02000, Vulpix)    { Species = 037, Level = 18, Moves = new(257,204,052,091), Location = 109 }, // Vulpix: Cipher Peon Mesin @ ONBS Building
        new(03, 01500, Spheal)    { Species = 363, Level = 17, Moves = new(062,204,055,189), Location = 011 }, // Spheal: Cipher Peon Blusix @ Cipher Lab
        new(03, 01500, Spheal)    { Species = 363, Level = 17, Moves = new(062,204,055,189), Location = 100 }, // Spheal: Cipher Peon Blusix  @ Phenac City
        new(04, 01500, First)     { Species = 343, Level = 17, Moves = new(317,287,189,060), Location = 011 }, // Baltoy: Cipher Peon Browsix @ Cipher Lab
        new(04, 01500, First)     { Species = 343, Level = 17, Moves = new(317,287,189,060), Location = 096 }, // Baltoy: Cipher Peon Browsix  @ Phenac City
        new(05, 01500, First)     { Species = 179, Level = 17, Moves = new(034,215,084,086), Location = 011 }, // Mareep: Cipher Peon Yellosix @ Cipher Lab
        new(05, 01500, First)     { Species = 179, Level = 17, Moves = new(034,215,084,086), Location = 096 }, // Mareep: Cipher Peon Yellosix @ Phenac City
        new(06, 01500, Gulpin)    { Species = 316, Level = 17, Moves = new(351,047,124,092), Location = 011 }, // Gulpin: Cipher Peon Purpsix @ Cipher Lab
        new(06, 01500, Gulpin)    { Species = 316, Level = 17, Moves = new(351,047,124,092), Location = 100 }, // Gulpin: Cipher Peon Purpsix @ Phenac City
        new(07, 01500, Seedot)    { Species = 273, Level = 17, Moves = new(202,287,331,290), Location = 011 }, // Seedot: Cipher Peon Greesix @ Cipher Lab
        new(07, 01500, Seedot)    { Species = 273, Level = 17, Moves = new(202,287,331,290), Location = 100 }, // Seedot: Cipher Peon Greesix @ Phenac City
        new(08, 01500, Spinarak)  { Species = 167, Level = 14, Moves = new(091,287,324,101), Location = 010 }, // Spinarak: Cipher Peon Nexir @ Cipher Lab
        new(09, 01500, Numel)     { Species = 322, Level = 14, Moves = new(036,204,091,052), Location = 009 }, // Numel: Cipher Peon Solox @ Cipher Lab
        new(10, 01700, First)     { Species = 318, Level = 15, Moves = new(352,287,184,044), Location = 008 }, // Carvanha: Cipher Peon Cabol @ Cipher Lab
        new(11, 03000, Roselia)   { Species = 315, Level = 22, Moves = new(345,186,320,073), Location = 094 }, // Roselia: Cipher Peon Fasin @ Phenac City
        new(12, 02500, Delcatty)  { Species = 301, Level = 18, Moves = new(290,186,213,351), Location = 008 }, // Delcatty: Cipher Admin Lovrina @ Cipher Lab
        new(13, 04000, Nosepass)  { Species = 299, Level = 26, Moves = new(085,270,086,157), Location = 090 }, // Nosepass: Wanderer Miror B. @ Poké Spots
        new(14, 01500, First)     { Species = 228, Level = 17, Moves = new(185,204,052,046), Location = 100 }, // Houndour: Cipher Peon Resix  @ Phenac City
        new(14, 01500, First)     { Species = 228, Level = 17, Moves = new(185,204,052,046), Location = 011 }, // Houndour: Cipher Peon Resix @ Cipher Lab
        new(15, 02000, Makuhita)  { Species = 296, Level = 18, Moves = new(280,287,292,317), Location = 109 }, // Makuhita: Cipher Peon Torkin @ ONBS Building
        new(16, 02200, Duskull)   { Species = 355, Level = 19, Moves = new(247,270,310,109), Location = 110 }, // Duskull: Cipher Peon Lobar @ ONBS Building
        new(17, 02200, Ralts)     { Species = 280, Level = 20, Moves = new(351,047,115,093), Location = 119 }, // Ralts: Cipher Peon Feldas @ ONBS Building
        new(18, 02500, Mawile)    { Species = 303, Level = 22, Moves = new(206,047,011,334), Location = 111 }, // Mawile: Cipher Cmdr Exol @ ONBS Building
        new(19, 02500, Snorunt)   { Species = 361, Level = 20, Moves = new(352,047,044,196), Location = 097 }, // Snorunt: Cipher Peon Exinn @ Phenac City
        new(20, 02500, Pineco)    { Species = 204, Level = 20, Moves = new(042,287,191,068), Location = 096 }, // Pineco: Cipher Peon Gonrap @ Phenac City
        new(21, 02500, Swinub)    { Species = 220, Level = 22, Moves = new(246,204,054,341), Location = 100 }, // Swinub: Cipher Peon Greck @ Phenac City
        new(22, 02500, Natu)      { Species = 177, Level = 22, Moves = new(248,226,101,332), Location = 094 }, // Natu: Cipher Peon Eloin @ Phenac City
        new(23, 01800, Shroomish) { Species = 285, Level = 15, Moves = new(206,287,072,078), Location = 008 }, // Shroomish: Cipher R&D Klots @ Cipher Lab
        new(24, 03500, Meowth)    { Species = 052, Level = 22, Moves = new(163,047,006,044), Location = 094 }, // Meowth: Cipher Peon Fostin @ Phenac City
        new(25, 04500, Spearow)   { Species = 021, Level = 22, Moves = new(206,226,043,332), Location = 107 }, // Spearow: Cipher Peon Ezin @ Phenac Stadium
        new(26, 03000, Grimer)    { Species = 088, Level = 23, Moves = new(188,270,325,107), Location = 107 }, // Grimer: Cipher Peon Faltly @ Phenac Stadium
        new(27, 03500, Seel)      { Species = 086, Level = 23, Moves = new(057,270,219,058), Location = 107 }, // Seel: Cipher Peon Egrog @ Phenac Stadium
        new(28, 05000, Lunatone)  { Species = 337, Level = 25, Moves = new(094,226,240,317), Location = 107 }, // Lunatone: Cipher Admin Snattle @ Phenac Stadium
        new(29, 02500, Voltorb)   { Species = 100, Level = 19, Moves = new(243,287,209,129), Location = 092 }, // Voltorb: Wanderer Miror B. @ Cave Poké Spot
        new(30, 05000, First)     { Species = 335, Level = 28, Moves = new(280,287,068,306), Location = 071 }, // Zangoose: Thug Zook @ Cipher Key Lair
        new(31, 04000, Growlithe) { Species = 058, Level = 28, Moves = new(053,204,044,036), Location = 064 }, // Growlithe: Cipher Peon Humah @ Cipher Key Lair
        new(32, 04000, Paras)     { Species = 046, Level = 28, Moves = new(147,287,163,206), Location = 064 }, // Paras: Cipher Peon Humah @ Cipher Key Lair
        new(33, 04000, First)     { Species = 090, Level = 29, Moves = new(036,287,057,062), Location = 065 }, // Shellder: Cipher Peon Gorog @ Cipher Key Lair
        new(34, 04500, First)     { Species = 015, Level = 30, Moves = new(188,226,041,014), Location = 066 }, // Beedrill: Cipher Peon Lok @ Cipher Key Lair
        new(35, 04000, Pidgeotto) { Species = 017, Level = 30, Moves = new(017,287,211,297), Location = 066 }, // Pidgeotto: Cipher Peon Lok @ Cipher Key Lair
        new(36, 04000, Butterfree){ Species = 012, Level = 30, Moves = new(094,234,079,332), Location = 067 }, // Butterfree: Cipher Peon Targ @ Cipher Key Lair
        new(37, 04000, Tangela)   { Species = 114, Level = 30, Moves = new(076,234,241,275), Location = 067 }, // Tangela: Cipher Peon Targ @ Cipher Key Lair
        new(38, 06000, Raticate)  { Species = 020, Level = 34, Moves = new(162,287,184,158), Location = 076 }, // Raticate: Chaser Furgy @ Citadark Isle
        new(39, 04000, Venomoth)  { Species = 049, Level = 32, Moves = new(318,287,164,094), Location = 070 }, // Venomoth: Cipher Peon Angic @ Cipher Key Lair
        new(40, 04000, Weepinbell){ Species = 070, Level = 32, Moves = new(345,234,188,230), Location = 070 }, // Weepinbell: Cipher Peon Angic @ Cipher Key Lair
        new(41, 05000, Arbok)     { Species = 024, Level = 33, Moves = new(188,287,137,044), Location = 070 }, // Arbok: Cipher Peon Smarton @ Cipher Key Lair
        new(42, 06000, Primeape)  { Species = 057, Level = 34, Moves = new(238,270,116,179), Location = 069 }, // Primeape: Cipher Admin Gorigan @ Cipher Key Lair
        new(43, 05500, Hypno)     { Species = 097, Level = 34, Moves = new(094,226,096,247), Location = 069 }, // Hypno: Cipher Admin Gorigan @ Cipher Key Lair
        new(44, 06500, Golduck)   { Species = 055, Level = 33, Moves = new(127,204,244,280), Location = 088 }, // Golduck: Navigator Abson @ Citadark Isle
        new(45, 07000, Sableye)   { Species = 302, Level = 33, Moves = new(247,270,185,105), Location = 088 }, // Sableye: Navigator Abson @ Citadark Isle
        new(46, 04500, Magneton)  { Species = 082, Level = 30, Moves = new(038,287,240,087), Location = 067 }, // Magneton: Cipher Peon Snidle @ Cipher Key Lair
        new(47, 08000, Dodrio)    { Species = 085, Level = 34, Moves = new(065,226,097,161), Location = 076 }, // Dodrio: Chaser Furgy @ Citadark Isle
        new(48, 05500, Farfetchd) { Species = 083, Level = 36, Moves = new(163,226,014,332), Location = 076 }, // Farfetch'd: Cipher Admin Lovrina @ Citadark Isle
        new(49, 06500, Altaria)   { Species = 334, Level = 36, Moves = new(225,215,076,332), Location = 076 }, // Altaria: Cipher Admin Lovrina @ Citadark Isle
        new(50, 06000, Kangaskhan){ Species = 115, Level = 35, Moves = new(089,047,039,146), Location = 085 }, // Kangaskhan: Cipher Peon Litnar @ Citadark Isle
        new(51, 07000, Banette)   { Species = 354, Level = 37, Moves = new(185,270,247,174), Location = 085 }, // Banette: Cipher Peon Litnar @ Citadark Isle
        new(52, 07000, Magmar)    { Species = 126, Level = 36, Moves = new(126,266,238,009), Location = 077 }, // Magmar: Cipher Peon Grupel @ Citadark Isle
        new(53, 07000, Pinsir)    { Species = 127, Level = 35, Moves = new(012,270,206,066), Location = 077 }, // Pinsir: Cipher Peon Grupel @ Citadark Isle
        new(54, 05500, Magcargo)  { Species = 219, Level = 38, Moves = new(257,287,089,053), Location = 080 }, // Magcargo: Cipher Peon Kolest @ Citadark Isle
        new(55, 06000, Rapidash)  { Species = 078, Level = 40, Moves = new(076,226,241,053), Location = 080 }, // Rapidash: Cipher Peon Kolest @ Citadark Isle
        new(56, 06000, Hitmonchan){ Species = 107, Level = 38, Moves = new(005,270,170,327), Location = 081 }, // Hitmonchan: Cipher Peon Karbon @ Citadark Isle
        new(57, 07000, Hitmonlee) { Species = 106, Level = 38, Moves = new(136,287,170,025), Location = 081 }, // Hitmonlee: Cipher Peon Petro @ Citadark Isle
        new(58, 05000, Lickitung) { Species = 108, Level = 38, Moves = new(038,270,111,205), Location = 084 }, // Lickitung: Cipher Peon Geftal @ Citadark Isle
        new(59, 08000, Scyther)   { Species = 123, Level = 40, Moves = new(013,234,318,163), Location = 084 }, // Scyther: Cipher Peon Leden @ Citadark Isle
        new(60, 04000, Chansey)   { Species = 113, Level = 39, Moves = new(085,186,135,285), Location = 084 }, // Chansey: Cipher Peon Leden @ Citadark Isle
        new(60, 04000, Chansey)   { Species = 113, Level = 39, Moves = new(085,186,135,285), Location = 087 }, // Chansey: Cipher Peon Leden @ Citadark Isle
        new(61, 07500, Solrock)   { Species = 338, Level = 41, Moves = new(094,226,241,322), Location = 087 }, // Solrock: Cipher Admin Snattle @ Citadark Isle
        new(62, 07500, Starmie)   { Species = 121, Level = 41, Moves = new(127,287,058,105), Location = 087 }, // Starmie: Cipher Admin Snattle @ Citadark Isle
        new(63, 07000, Electabuzz){ Species = 125, Level = 43, Moves = new(238,266,086,085), Location = 087 }, // Electabuzz: Cipher Admin Ardos @ Citadark Isle
        new(64, 07000, First)     { Species = 277, Level = 43, Moves = new(143,226,097,263), Location = 087 }, // Swellow: Cipher Admin Ardos @ Citadark Isle
        new(65, 09000, Snorlax)   { Species = 143, Level = 43, Moves = new(090,287,174,034), Location = 087 }, // Snorlax: Cipher Admin Ardos @ Citadark Isle
        new(66, 07500, Poliwrath) { Species = 062, Level = 42, Moves = new(056,270,240,280), Location = 087 }, // Poliwrath: Cipher Admin Gorigan @ Citadark Isle
        new(67, 06500, MrMime)    { Species = 122, Level = 42, Moves = new(094,266,227,009), Location = 087 }, // Mr. Mime: Cipher Admin Gorigan @ Citadark Isle
        new(68, 05000, Dugtrio)   { Species = 051, Level = 40, Moves = new(089,204,201,161), Location = 075 }, // Dugtrio: Cipher Peon Kolax @ Citadark Isle
        new(69, 07000, Manectric) { Species = 310, Level = 44, Moves = new(087,287,240,044), Location = 073 }, // Manectric: Cipher Admin Eldes @ Citadark Isle
        new(70, 09000, Salamence) { Species = 373, Level = 50, Moves = new(337,287,349,332), Location = 073 }, // Salamence: Cipher Admin Eldes @ Citadark Isle
        new(71, 06500, Marowak)   { Species = 105, Level = 44, Moves = new(089,047,014,157), Location = 073 }, // Marowak: Cipher Admin Eldes @ Citadark Isle
        new(72, 06000, Lapras)    { Species = 131, Level = 44, Moves = new(056,215,240,059), Location = 073 }, // Lapras: Cipher Admin Eldes @ Citadark Isle
        new(73, 12000, First)     { Species = 249, Level = 50, Moves = new(354,297,089,056), Location = 074 }, // Lugia: Grand Master Greevil @ Citadark Isle
        new(74, 10000, Zapdos)    { Species = 145, Level = 50, Moves = new(326,226,319,085), Location = 074 }, // Zapdos: Grand Master Greevil @ Citadark Isle
        new(75, 10000, Moltres)   { Species = 146, Level = 50, Moves = new(326,234,261,053), Location = 074 }, // Moltres: Grand Master Greevil @ Citadark Isle
        new(76, 10000, Articuno)  { Species = 144, Level = 50, Moves = new(326,215,114,058), Location = 074 }, // Articuno: Grand Master Greevil @ Citadark Isle
        new(77, 09000, Tauros)    { Species = 128, Level = 46, Moves = new(089,287,039,034), Location = 074 }, // Tauros: Grand Master Greevil @ Citadark Isle
        new(78, 07000, First)     { Species = 112, Level = 46, Moves = new(224,270,184,089), Location = 074 }, // Rhydon: Grand Master Greevil @ Citadark Isle
        new(79, 09000, Exeggutor) { Species = 103, Level = 46, Moves = new(094,287,095,246), Location = 074 }, // Exeggutor: Grand Master Greevil @ Citadark Isle
        new(80, 09000, Dragonite) { Species = 149, Level = 55, Moves = new(063,215,349,089), Location = 162 }, // Dragonite: Wanderer Miror B. @ Gateon Port
        new(81, 04500, First)     { Species = 175, Level = 25, Moves = new(266,161,246,270), Location = 164, FixedBall = Ball.Poke }, // Togepi: Pokémon Trainer Hordel @ Outskirt Stand
        new(82, 02500, Poochyena) { Species = 261, Level = 10, Moves = new(091,215,305,336), Location = 162 }, // Poochyena: Bodybuilder Kilen @ Gateon Port
        new(83, 02500, Ledyba)    { Species = 165, Level = 10, Moves = new(060,287,332,048), Location = 153 }, // Ledyba: Casual Guy Cyle @ Gateon Port
    ];

    internal static readonly EncounterArea3XD[] Slots =
    [
        new(90, 027, 23, 207, 20, 328, 20), // Rock (Sandshrew, Gligar, Trapinch)
        new(91, 187, 20, 231, 20, 283, 20), // Oasis (Hoppip, Phanpy, Surskit)
        new(92, 041, 21, 304, 21, 194, 21), // Cave (Zubat, Aron, Wooper)
    ];
}
