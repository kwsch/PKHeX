using static PKHeX.Core.Encounters3XDTeams;

namespace PKHeX.Core;

internal static class Encounters3XD
{
    private static readonly EncounterStatic3[] Encounter_XDGift =
    {
        new(133, 10, GameVersion.XD) { FatefulEncounter = true, Gift = true, Location = 000, Moves = new(044) }, // Eevee (Bite)
        new(152, 05, GameVersion.XD) { FatefulEncounter = true, Gift = true, Location = 016, Moves = new(246,033,045,338) }, // Chikorita
        new(155, 05, GameVersion.XD) { FatefulEncounter = true, Gift = true, Location = 016, Moves = new(179,033,043,307) }, // Cyndaquil
        new(158, 05, GameVersion.XD) { FatefulEncounter = true, Gift = true, Location = 016, Moves = new(242,010,043,308) }, // Totodile
    };

    private static readonly EncounterStaticShadow[] Encounter_XD =
    {
        new(GameVersion.XD, 01, 03000, First)     { FatefulEncounter = true, Species = 216, Level = 11, Moves = new(216,287,122,232), Location = 143, Gift = true }, // Teddiursa: Cipher Peon Naps @ Pokémon HQ Lab -- treat as Gift as it can only be captured in a Poké Ball
        new(GameVersion.XD, 02, 02000, Vulpix)    { FatefulEncounter = true, Species = 037, Level = 18, Moves = new(257,204,052,091), Location = 109 }, // Vulpix: Cipher Peon Mesin @ ONBS Building
        new(GameVersion.XD, 03, 01500, Spheal)    { FatefulEncounter = true, Species = 363, Level = 17, Moves = new(062,204,055,189), Location = 011 }, // Spheal: Cipher Peon Blusix @ Cipher Lab
        new(GameVersion.XD, 03, 01500, Spheal)    { FatefulEncounter = true, Species = 363, Level = 17, Moves = new(062,204,055,189), Location = 100 }, // Spheal: Cipher Peon Blusix  @ Phenac City
        new(GameVersion.XD, 04, 01500, First)     { FatefulEncounter = true, Species = 343, Level = 17, Moves = new(317,287,189,060), Location = 011 }, // Baltoy: Cipher Peon Browsix @ Cipher Lab
        new(GameVersion.XD, 04, 01500, First)     { FatefulEncounter = true, Species = 343, Level = 17, Moves = new(317,287,189,060), Location = 096 }, // Baltoy: Cipher Peon Browsix  @ Phenac City
        new(GameVersion.XD, 05, 01500, First)     { FatefulEncounter = true, Species = 179, Level = 17, Moves = new(034,215,084,086), Location = 011 }, // Mareep: Cipher Peon Yellosix @ Cipher Lab
        new(GameVersion.XD, 05, 01500, First)     { FatefulEncounter = true, Species = 179, Level = 17, Moves = new(034,215,084,086), Location = 096 }, // Mareep: Cipher Peon Yellosix @ Phenac City
        new(GameVersion.XD, 06, 01500, Gulpin)    { FatefulEncounter = true, Species = 316, Level = 17, Moves = new(351,047,124,092), Location = 011 }, // Gulpin: Cipher Peon Purpsix @ Cipher Lab
        new(GameVersion.XD, 06, 01500, Gulpin)    { FatefulEncounter = true, Species = 316, Level = 17, Moves = new(351,047,124,092), Location = 100 }, // Gulpin: Cipher Peon Purpsix @ Phenac City
        new(GameVersion.XD, 07, 01500, Seedot)    { FatefulEncounter = true, Species = 273, Level = 17, Moves = new(202,287,331,290), Location = 011 }, // Seedot: Cipher Peon Greesix @ Cipher Lab
        new(GameVersion.XD, 07, 01500, Seedot)    { FatefulEncounter = true, Species = 273, Level = 17, Moves = new(202,287,331,290), Location = 100 }, // Seedot: Cipher Peon Greesix @ Phenac City
        new(GameVersion.XD, 08, 01500, Spinarak)  { FatefulEncounter = true, Species = 167, Level = 14, Moves = new(091,287,324,101), Location = 010 }, // Spinarak: Cipher Peon Nexir @ Cipher Lab
        new(GameVersion.XD, 09, 01500, Numel)     { FatefulEncounter = true, Species = 322, Level = 14, Moves = new(036,204,091,052), Location = 009 }, // Numel: Cipher Peon Solox @ Cipher Lab
        new(GameVersion.XD, 10, 01700, First)     { FatefulEncounter = true, Species = 318, Level = 15, Moves = new(352,287,184,044), Location = 008 }, // Carvanha: Cipher Peon Cabol @ Cipher Lab
        new(GameVersion.XD, 11, 03000, Roselia)   { FatefulEncounter = true, Species = 315, Level = 22, Moves = new(345,186,320,073), Location = 094 }, // Roselia: Cipher Peon Fasin @ Phenac City
        new(GameVersion.XD, 12, 02500, Delcatty)  { FatefulEncounter = true, Species = 301, Level = 18, Moves = new(290,186,213,351), Location = 008 }, // Delcatty: Cipher Admin Lovrina @ Cipher Lab
        new(GameVersion.XD, 13, 04000, Nosepass)  { FatefulEncounter = true, Species = 299, Level = 26, Moves = new(085,270,086,157), Location = 090 }, // Nosepass: Wanderer Miror B. @ Poké Spots
        new(GameVersion.XD, 14, 01500, First)     { FatefulEncounter = true, Species = 228, Level = 17, Moves = new(185,204,052,046), Location = 100 }, // Houndour: Cipher Peon Resix  @ Phenac City
        new(GameVersion.XD, 14, 01500, First)     { FatefulEncounter = true, Species = 228, Level = 17, Moves = new(185,204,052,046), Location = 011 }, // Houndour: Cipher Peon Resix @ Cipher Lab
        new(GameVersion.XD, 15, 02000, Makuhita)  { FatefulEncounter = true, Species = 296, Level = 18, Moves = new(280,287,292,317), Location = 109 }, // Makuhita: Cipher Peon Torkin @ ONBS Building
        new(GameVersion.XD, 16, 02200, Duskull)   { FatefulEncounter = true, Species = 355, Level = 19, Moves = new(247,270,310,109), Location = 110 }, // Duskull: Cipher Peon Lobar @ ONBS Building
        new(GameVersion.XD, 17, 02200, Ralts)     { FatefulEncounter = true, Species = 280, Level = 20, Moves = new(351,047,115,093), Location = 119 }, // Ralts: Cipher Peon Feldas @ ONBS Building
        new(GameVersion.XD, 18, 02500, Mawile)    { FatefulEncounter = true, Species = 303, Level = 22, Moves = new(206,047,011,334), Location = 111 }, // Mawile: Cipher Cmdr Exol @ ONBS Building
        new(GameVersion.XD, 19, 02500, Snorunt)   { FatefulEncounter = true, Species = 361, Level = 20, Moves = new(352,047,044,196), Location = 097 }, // Snorunt: Cipher Peon Exinn @ Phenac City
        new(GameVersion.XD, 20, 02500, Pineco)    { FatefulEncounter = true, Species = 204, Level = 20, Moves = new(042,287,191,068), Location = 096 }, // Pineco: Cipher Peon Gonrap @ Phenac City
        new(GameVersion.XD, 21, 02500, Swinub)    { FatefulEncounter = true, Species = 220, Level = 22, Moves = new(246,204,054,341), Location = 100 }, // Swinub: Cipher Peon Greck @ Phenac City
        new(GameVersion.XD, 22, 02500, Natu)      { FatefulEncounter = true, Species = 177, Level = 22, Moves = new(248,226,101,332), Location = 094 }, // Natu: Cipher Peon Eloin @ Phenac City
        new(GameVersion.XD, 23, 01800, Shroomish) { FatefulEncounter = true, Species = 285, Level = 15, Moves = new(206,287,072,078), Location = 008 }, // Shroomish: Cipher R&D Klots @ Cipher Lab
        new(GameVersion.XD, 24, 03500, Meowth)    { FatefulEncounter = true, Species = 052, Level = 22, Moves = new(163,047,006,044), Location = 094 }, // Meowth: Cipher Peon Fostin @ Phenac City
        new(GameVersion.XD, 25, 04500, Spearow)   { FatefulEncounter = true, Species = 021, Level = 22, Moves = new(206,226,043,332), Location = 107 }, // Spearow: Cipher Peon Ezin @ Phenac Stadium
        new(GameVersion.XD, 26, 03000, Grimer)    { FatefulEncounter = true, Species = 088, Level = 23, Moves = new(188,270,325,107), Location = 107 }, // Grimer: Cipher Peon Faltly @ Phenac Stadium
        new(GameVersion.XD, 27, 03500, Seel)      { FatefulEncounter = true, Species = 086, Level = 23, Moves = new(057,270,219,058), Location = 107 }, // Seel: Cipher Peon Egrog @ Phenac Stadium
        new(GameVersion.XD, 28, 05000, Lunatone)  { FatefulEncounter = true, Species = 337, Level = 25, Moves = new(094,226,240,317), Location = 107 }, // Lunatone: Cipher Admin Snattle @ Phenac Stadium
        new(GameVersion.XD, 29, 02500, Voltorb)   { FatefulEncounter = true, Species = 100, Level = 19, Moves = new(243,287,209,129), Location = 092 }, // Voltorb: Wanderer Miror B. @ Cave Poké Spot
        new(GameVersion.XD, 30, 05000, First)     { FatefulEncounter = true, Species = 335, Level = 28, Moves = new(280,287,068,306), Location = 071 }, // Zangoose: Thug Zook @ Cipher Key Lair
        new(GameVersion.XD, 31, 04000, Growlithe) { FatefulEncounter = true, Species = 058, Level = 28, Moves = new(053,204,044,036), Location = 064 }, // Growlithe: Cipher Peon Humah @ Cipher Key Lair
        new(GameVersion.XD, 32, 04000, Paras)     { FatefulEncounter = true, Species = 046, Level = 28, Moves = new(147,287,163,206), Location = 064 }, // Paras: Cipher Peon Humah @ Cipher Key Lair
        new(GameVersion.XD, 33, 04000, First)     { FatefulEncounter = true, Species = 090, Level = 29, Moves = new(036,287,057,062), Location = 065 }, // Shellder: Cipher Peon Gorog @ Cipher Key Lair
        new(GameVersion.XD, 34, 04500, First)     { FatefulEncounter = true, Species = 015, Level = 30, Moves = new(188,226,041,014), Location = 066 }, // Beedrill: Cipher Peon Lok @ Cipher Key Lair
        new(GameVersion.XD, 35, 04000, Pidgeotto) { FatefulEncounter = true, Species = 017, Level = 30, Moves = new(017,287,211,297), Location = 066 }, // Pidgeotto: Cipher Peon Lok @ Cipher Key Lair
        new(GameVersion.XD, 36, 04000, Butterfree){ FatefulEncounter = true, Species = 012, Level = 30, Moves = new(094,234,079,332), Location = 067 }, // Butterfree: Cipher Peon Targ @ Cipher Key Lair
        new(GameVersion.XD, 37, 04000, Tangela)   { FatefulEncounter = true, Species = 114, Level = 30, Moves = new(076,234,241,275), Location = 067 }, // Tangela: Cipher Peon Targ @ Cipher Key Lair
        new(GameVersion.XD, 38, 06000, Raticate)  { FatefulEncounter = true, Species = 020, Level = 34, Moves = new(162,287,184,158), Location = 076 }, // Raticate: Chaser Furgy @ Citadark Isle
        new(GameVersion.XD, 39, 04000, Venomoth)  { FatefulEncounter = true, Species = 049, Level = 32, Moves = new(318,287,164,094), Location = 070 }, // Venomoth: Cipher Peon Angic @ Cipher Key Lair
        new(GameVersion.XD, 40, 04000, Weepinbell){ FatefulEncounter = true, Species = 070, Level = 32, Moves = new(345,234,188,230), Location = 070 }, // Weepinbell: Cipher Peon Angic @ Cipher Key Lair
        new(GameVersion.XD, 41, 05000, Arbok)     { FatefulEncounter = true, Species = 024, Level = 33, Moves = new(188,287,137,044), Location = 070 }, // Arbok: Cipher Peon Smarton @ Cipher Key Lair
        new(GameVersion.XD, 42, 06000, Primeape)  { FatefulEncounter = true, Species = 057, Level = 34, Moves = new(238,270,116,179), Location = 069 }, // Primeape: Cipher Admin Gorigan @ Cipher Key Lair
        new(GameVersion.XD, 43, 05500, Hypno)     { FatefulEncounter = true, Species = 097, Level = 34, Moves = new(094,226,096,247), Location = 069 }, // Hypno: Cipher Admin Gorigan @ Cipher Key Lair
        new(GameVersion.XD, 44, 06500, Golduck)   { FatefulEncounter = true, Species = 055, Level = 33, Moves = new(127,204,244,280), Location = 088 }, // Golduck: Navigator Abson @ Citadark Isle
        new(GameVersion.XD, 45, 07000, Sableye)   { FatefulEncounter = true, Species = 302, Level = 33, Moves = new(247,270,185,105), Location = 088 }, // Sableye: Navigator Abson @ Citadark Isle
        new(GameVersion.XD, 46, 04500, Magneton)  { FatefulEncounter = true, Species = 082, Level = 30, Moves = new(038,287,240,087), Location = 067 }, // Magneton: Cipher Peon Snidle @ Cipher Key Lair
        new(GameVersion.XD, 47, 08000, Dodrio)    { FatefulEncounter = true, Species = 085, Level = 34, Moves = new(065,226,097,161), Location = 076 }, // Dodrio: Chaser Furgy @ Citadark Isle
        new(GameVersion.XD, 48, 05500, Farfetchd) { FatefulEncounter = true, Species = 083, Level = 36, Moves = new(163,226,014,332), Location = 076 }, // Farfetch'd: Cipher Admin Lovrina @ Citadark Isle
        new(GameVersion.XD, 49, 06500, Altaria)   { FatefulEncounter = true, Species = 334, Level = 36, Moves = new(225,215,076,332), Location = 076 }, // Altaria: Cipher Admin Lovrina @ Citadark Isle
        new(GameVersion.XD, 50, 06000, Kangaskhan){ FatefulEncounter = true, Species = 115, Level = 35, Moves = new(089,047,039,146), Location = 085 }, // Kangaskhan: Cipher Peon Litnar @ Citadark Isle
        new(GameVersion.XD, 51, 07000, Banette)   { FatefulEncounter = true, Species = 354, Level = 37, Moves = new(185,270,247,174), Location = 085 }, // Banette: Cipher Peon Litnar @ Citadark Isle
        new(GameVersion.XD, 52, 07000, Magmar)    { FatefulEncounter = true, Species = 126, Level = 36, Moves = new(126,266,238,009), Location = 077 }, // Magmar: Cipher Peon Grupel @ Citadark Isle
        new(GameVersion.XD, 53, 07000, Pinsir)    { FatefulEncounter = true, Species = 127, Level = 35, Moves = new(012,270,206,066), Location = 077 }, // Pinsir: Cipher Peon Grupel @ Citadark Isle
        new(GameVersion.XD, 54, 05500, Magcargo)  { FatefulEncounter = true, Species = 219, Level = 38, Moves = new(257,287,089,053), Location = 080 }, // Magcargo: Cipher Peon Kolest @ Citadark Isle
        new(GameVersion.XD, 55, 06000, Rapidash)  { FatefulEncounter = true, Species = 078, Level = 40, Moves = new(076,226,241,053), Location = 080 }, // Rapidash: Cipher Peon Kolest @ Citadark Isle
        new(GameVersion.XD, 56, 06000, Hitmonchan){ FatefulEncounter = true, Species = 107, Level = 38, Moves = new(005,270,170,327), Location = 081 }, // Hitmonchan: Cipher Peon Karbon @ Citadark Isle
        new(GameVersion.XD, 57, 07000, Hitmonlee) { FatefulEncounter = true, Species = 106, Level = 38, Moves = new(136,287,170,025), Location = 081 }, // Hitmonlee: Cipher Peon Petro @ Citadark Isle
        new(GameVersion.XD, 58, 05000, Lickitung) { FatefulEncounter = true, Species = 108, Level = 38, Moves = new(038,270,111,205), Location = 084 }, // Lickitung: Cipher Peon Geftal @ Citadark Isle
        new(GameVersion.XD, 59, 08000, Scyther)   { FatefulEncounter = true, Species = 123, Level = 40, Moves = new(013,234,318,163), Location = 084 }, // Scyther: Cipher Peon Leden @ Citadark Isle
        new(GameVersion.XD, 60, 04000, Chansey)   { FatefulEncounter = true, Species = 113, Level = 39, Moves = new(085,186,135,285), Location = 084 }, // Chansey: Cipher Peon Leden @ Citadark Isle
        new(GameVersion.XD, 60, 04000, Chansey)   { FatefulEncounter = true, Species = 113, Level = 39, Moves = new(085,186,135,285), Location = 087 }, // Chansey: Cipher Peon Leden @ Citadark Isle
        new(GameVersion.XD, 61, 07500, Solrock)   { FatefulEncounter = true, Species = 338, Level = 41, Moves = new(094,226,241,322), Location = 087 }, // Solrock: Cipher Admin Snattle @ Citadark Isle
        new(GameVersion.XD, 62, 07500, Starmie)   { FatefulEncounter = true, Species = 121, Level = 41, Moves = new(127,287,058,105), Location = 087 }, // Starmie: Cipher Admin Snattle @ Citadark Isle
        new(GameVersion.XD, 63, 07000, Electabuzz){ FatefulEncounter = true, Species = 125, Level = 43, Moves = new(238,266,086,085), Location = 087 }, // Electabuzz: Cipher Admin Ardos @ Citadark Isle
        new(GameVersion.XD, 64, 07000, First)     { FatefulEncounter = true, Species = 277, Level = 43, Moves = new(143,226,097,263), Location = 087 }, // Swellow: Cipher Admin Ardos @ Citadark Isle
        new(GameVersion.XD, 65, 09000, Snorlax)   { FatefulEncounter = true, Species = 143, Level = 43, Moves = new(090,287,174,034), Location = 087 }, // Snorlax: Cipher Admin Ardos @ Citadark Isle
        new(GameVersion.XD, 66, 07500, Poliwrath) { FatefulEncounter = true, Species = 062, Level = 42, Moves = new(056,270,240,280), Location = 087 }, // Poliwrath: Cipher Admin Gorigan @ Citadark Isle
        new(GameVersion.XD, 67, 06500, MrMime)    { FatefulEncounter = true, Species = 122, Level = 42, Moves = new(094,266,227,009), Location = 087 }, // Mr. Mime: Cipher Admin Gorigan @ Citadark Isle
        new(GameVersion.XD, 68, 05000, Dugtrio)   { FatefulEncounter = true, Species = 051, Level = 40, Moves = new(089,204,201,161), Location = 075 }, // Dugtrio: Cipher Peon Kolax @ Citadark Isle
        new(GameVersion.XD, 69, 07000, Manectric) { FatefulEncounter = true, Species = 310, Level = 44, Moves = new(087,287,240,044), Location = 073 }, // Manectric: Cipher Admin Eldes @ Citadark Isle
        new(GameVersion.XD, 70, 09000, Salamence) { FatefulEncounter = true, Species = 373, Level = 50, Moves = new(337,287,349,332), Location = 073 }, // Salamence: Cipher Admin Eldes @ Citadark Isle
        new(GameVersion.XD, 71, 06500, Marowak)   { FatefulEncounter = true, Species = 105, Level = 44, Moves = new(089,047,014,157), Location = 073 }, // Marowak: Cipher Admin Eldes @ Citadark Isle
        new(GameVersion.XD, 72, 06000, Lapras)    { FatefulEncounter = true, Species = 131, Level = 44, Moves = new(056,215,240,059), Location = 073 }, // Lapras: Cipher Admin Eldes @ Citadark Isle
        new(GameVersion.XD, 73, 12000, First)     { FatefulEncounter = true, Species = 249, Level = 50, Moves = new(354,297,089,056), Location = 074 }, // Lugia: Grand Master Greevil @ Citadark Isle
        new(GameVersion.XD, 74, 10000, Zapdos)    { FatefulEncounter = true, Species = 145, Level = 50, Moves = new(326,226,319,085), Location = 074 }, // Zapdos: Grand Master Greevil @ Citadark Isle
        new(GameVersion.XD, 75, 10000, Moltres)   { FatefulEncounter = true, Species = 146, Level = 50, Moves = new(326,234,261,053), Location = 074 }, // Moltres: Grand Master Greevil @ Citadark Isle
        new(GameVersion.XD, 76, 10000, Articuno)  { FatefulEncounter = true, Species = 144, Level = 50, Moves = new(326,215,114,058), Location = 074 }, // Articuno: Grand Master Greevil @ Citadark Isle
        new(GameVersion.XD, 77, 09000, Tauros)    { FatefulEncounter = true, Species = 128, Level = 46, Moves = new(089,287,039,034), Location = 074 }, // Tauros: Grand Master Greevil @ Citadark Isle
        new(GameVersion.XD, 78, 07000, First)     { FatefulEncounter = true, Species = 112, Level = 46, Moves = new(224,270,184,089), Location = 074 }, // Rhydon: Grand Master Greevil @ Citadark Isle
        new(GameVersion.XD, 79, 09000, Exeggutor) { FatefulEncounter = true, Species = 103, Level = 46, Moves = new(094,287,095,246), Location = 074 }, // Exeggutor: Grand Master Greevil @ Citadark Isle
        new(GameVersion.XD, 80, 09000, Dragonite) { FatefulEncounter = true, Species = 149, Level = 55, Moves = new(063,215,349,089), Location = 162 }, // Dragonite: Wanderer Miror B. @ Gateon Port
        new(GameVersion.XD, 81, 04500, First)     { FatefulEncounter = true, Species = 175, Level = 25, Moves = new(266,161,246,270), Location = 164, Gift = true }, // Togepi: Pokémon Trainer Hordel @ Outskirt Stand
        new(GameVersion.XD, 82, 02500, Poochyena) { FatefulEncounter = true, Species = 261, Level = 10, Moves = new(091,215,305,336), Location = 162 }, // Poochyena: Bodybuilder Kilen @ Gateon Port
        new(GameVersion.XD, 83, 02500, Ledyba)    { FatefulEncounter = true, Species = 165, Level = 10, Moves = new(060,287,332,048), Location = 153 }, // Ledyba: Casual Guy Cyle @ Gateon Port
    };

    internal static readonly EncounterArea3XD[] SlotsXD =
    {
        new(90, 027, 23, 207, 20, 328, 20), // Rock (Sandshrew, Gligar, Trapinch)
        new(91, 187, 20, 231, 20, 283, 20), // Oasis (Hoppip, Phanpy, Surskit)
        new(92, 041, 21, 304, 21, 194, 21), // Cave (Zubat, Aron, Wooper)
    };

    internal static readonly EncounterStatic3[] Encounter_CXDGift   = ArrayUtil.ConcatAll(Encounters3Colo.Encounter_ColoGift, Encounter_XDGift);
    internal static readonly EncounterStaticShadow[] Encounter_CXDShadow = ArrayUtil.ConcatAll(Encounters3Colo.Encounter_Colo, Encounter_XD);
}
