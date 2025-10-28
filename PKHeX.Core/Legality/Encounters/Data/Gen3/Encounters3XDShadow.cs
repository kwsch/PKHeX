using static PKHeX.Core.NPCLockDetail;

namespace PKHeX.Core;

/// <summary>
/// Team listings for <see cref="GameVersion.CXD"/> that have a shadow Pokémon afterward.
/// </summary>
public static class Encounters3XDShadow
{
    public static readonly TeamLock XRalts = new(
        280, // Ralts
        [
            new NPCLock(064, 00, 0, 063), // Kadabra (M) (Hardy)
            new NPCLock(180, 06, 1, 127), // Flaaffy (F) (Docile)
            new NPCLock(288, 18, 0, 127), // Vigoroth (M) (Bashful)
        ]);

    public static readonly TeamLock XPoochyena = new(
        261, // Poochyena
        [
            new NPCLock(041, 12, 1, 127), // Zubat (F) (Serious)
        ]);

    public static readonly TeamLock XLedyba = new(
        165, // Ledyba
        [
            new NPCLock(276, 00, 1, 127), // Taillow (F) (Hardy)
        ]);

    public static readonly TeamLock XSphealCipherLab = new(
        363, // Spheal
        CipherLab,
        [
            new NPCLock(116, 24, 0, 063), // Horsea (M) (Quirky)
            new NPCLock(118, 12, 1, 127), // Goldeen (F) (Serious)
        ]);

    public static readonly TeamLock XSphealPhenacCityandPost = new(
        363, // Spheal
        PhenacCityAndPost,
        [
            new NPCLock(116, 24, 0, 063), // Horsea (M) (Quirky)
            new NPCLock(118, 12, 1, 127), // Goldeen (F) (Serious)
            new NPCLock(374, 00, 2, 255), // Beldum (-) (Hardy)
        ]);

    public static readonly TeamLock XGulpin = new(
        316, // Gulpin
        [
            new NPCLock(109, 12, 1, 127), // Koffing (F) (Serious)
            new NPCLock(088, 06, 0, 127), // Grimer (M) (Docile)
        ]);

    public static readonly TeamLock XSeedotCipherLab = new(
        273, // Seedot
        CipherLab,
        [
            new NPCLock(043, 06, 0, 127), // Oddish (M) (Docile)
            new NPCLock(331, 24, 1, 127), // Cacnea (F) (Quirky)
            new NPCLock(285, 18, 1, 127), // Shroomish (F) (Bashful)
            new NPCLock(270, 00, 0, 127), // Lotad (M) (Hardy)
            new NPCLock(204, 12, 0, 127), // Pineco (M) (Serious)
        ]);

    public static readonly TeamLock XSeedotPhenacCity = new(
        273, // Seedot
        PhenacCity,
        [
            new NPCLock(043, 06, 0, 127), // Oddish (M) (Docile)
            new NPCLock(331, 24, 1, 127), // Cacnea (F) (Quirky)
            new NPCLock(285, 00, 1, 127), // Shroomish (F) (Hardy)
            new NPCLock(270, 00, 1, 127), // Lotad (F) (Hardy)
            new NPCLock(204, 06, 0, 127), // Pineco (M) (Docile)
        ]);

    public static readonly TeamLock XSeedotPost = new(
        273, // Seedot
        Post,
        [
            new NPCLock(045, 06, 0, 127), // Vileplume (M) (Docile)
            new NPCLock(332, 24, 1, 127), // Cacturne (F) (Quirky)
            new NPCLock(286, 00, 1, 127), // Breloom (F) (Hardy)
            new NPCLock(271, 00, 0, 127), // Lombre (M) (Hardy)
            new NPCLock(205, 12, 0, 127), // Forretress (M) (Serious)
        ]);

    public static readonly TeamLock XSpinarak = new(
        167, // Spinarak
        [
            new NPCLock(220, 12, 1, 127), // Swinub (F) (Serious)
            new NPCLock(353, 06, 0, 127), // Shuppet (M) (Docile)
        ]);

    public static readonly TeamLock XNumel = new(
        322, // Numel
        [
            new NPCLock(280, 06, 0, 127), // Ralts (M) (Docile)
            new NPCLock(100, 00, 2, 255), // Voltorb (-) (Hardy)
            new NPCLock(371, 24, 1, 127), // Bagon (F) (Quirky)
        ]);

    public static readonly TeamLock XShroomish = new(
        285, // Shroomish
        [
            new NPCLock(209, 24, 1, 191), // Snubbull (F) (Quirky)
            new NPCLock(352, 00, 1, 127), // Kecleon (F) (Hardy)
        ]);

    public static readonly TeamLock XDelcatty = new(
        301, // Delcatty
        [
            new NPCLock(370, 06, 1, 191), // Luvdisc (F) (Docile)
            new NPCLock(267, 00, 0, 127), // Beautifly (M) (Hardy)
            new NPCLock(315, 24, 0, 127), // Roselia (M) (Quirky)
        ]);

    public static readonly TeamLock XVoltorb = new(
        100, // Voltorb
        [
            new NPCLock(271, 00, 0, 127), // Lombre (M) (Hardy)
            new NPCLock(271, 18, 0, 127), // Lombre (M) (Bashful)
            new NPCLock(271, 12, 1, 127), // Lombre (F) (Serious)
        ]);

    public static readonly TeamLock XMakuhita = new(
        296, // Makuhita
        [
            new NPCLock(352, 06, 0, 127), // Kecleon (M) (Docile)
            new NPCLock(283, 18, 1, 127), // Surskit (F) (Bashful)
        ]);

    public static readonly TeamLock XVulpix = new(
        037, // Vulpix
        [
            new NPCLock(167, 00, 0, 127), // Spinarak (M) (Hardy)
            new NPCLock(267, 06, 1, 127), // Beautifly (F) (Docile)
            new NPCLock(269, 18, 0, 127), // Dustox (M) (Bashful)
        ]);

    public static readonly TeamLock XDuskull = new(
        355, // Duskull
        [
            new NPCLock(215, 12, 0, 127), // Sneasel (M) (Serious)
            new NPCLock(193, 18, 1, 127), // Yanma (F) (Bashful)
            new NPCLock(200, 24, 0, 127), // Misdreavus (M) (Quirky)
        ]);

    public static readonly TeamLock XMawile = new(
        303, // Mawile
        [
            new NPCLock(294, 06, 0, 127), // Loudred (M) (Docile)
            new NPCLock(203, 18, 1, 127), // Girafarig (F) (Bashful)
        ]);

    public static readonly TeamLock XSnorunt = new(
        361, // Snorunt
        [
            new NPCLock(336, 06, 1, 127), // Seviper (F) (Docile)
        ]);

    public static readonly TeamLock XPineco = new(
        204, // Pineco
        [
            new NPCLock(198, 06, 0, 127), // Murkrow (M) (Docile)
        ]);

    public static readonly TeamLock XNatu = new(
        177, // Natu
        [
            new NPCLock(281, 00, 0, 127), // Kirlia (M) (Hardy)
            new NPCLock(264, 00, 1, 127), // Linoone (F) (Hardy)
        ]);

    public static readonly TeamLock XRoselia = new(
        315, // Roselia
        [
            new NPCLock(223, 06, 0, 127), // Remoraid (M) (Docile)
            new NPCLock(042, 18, 0, 127), // Golbat (M) (Bashful)
        ]);

    public static readonly TeamLock XMeowth = new(
        052, // Meowth
        [
            new NPCLock(064, 06, 0, 063), // Kadabra (M) (Docile)
            new NPCLock(215, 00, 1, 127), // Sneasel (F) (Hardy)
            new NPCLock(200, 18, 1, 127), // Misdreavus (F) (Bashful)
        ]);

    public static readonly TeamLock XSwinub = new(
        220, // Swinub
        [
            new NPCLock(324, 18, 1, 127), // Torkoal (F) (Bashful)
            new NPCLock(274, 00, 0, 127), // Nuzleaf (M) (Hardy)
        ]);

    public static readonly TeamLock XSpearow = new(
        021, // Spearow
        [
            new NPCLock(279, 18, 0, 127), // Pelipper (M) (Bashful)
            new NPCLock(309, 06, 1, 127), // Electrike (F) (Docile)
        ]);

    public static readonly TeamLock XGrimer = new(
        088, // Grimer
        [
            new NPCLock(358, 12, 0, 127), // Chimecho (M) (Serious)
            new NPCLock(234, 18, 0, 127), // Stantler (M) (Bashful)
        ]);

    public static readonly TeamLock XSeel = new(
        086, // Seel
        [
            new NPCLock(163, 06, 0, 127), // Hoothoot (M) (Docile)
            new NPCLock(075, 18, 0, 127), // Graveler (M) (Bashful)
            new NPCLock(316, 18, 1, 127), // Gulpin (F) (Bashful)
        ]);

    public static readonly TeamLock XLunatone = new(
        337, // Lunatone
        [
            new NPCLock(171, 00, 1, 127), // Lanturn (F) (Hardy)
            new NPCLock(195, 18, 0, 127), // Quagsire (M) (Bashful)
        ]);

    public static readonly TeamLock XNosepass = new(
        299, // Nosepass
        [
            new NPCLock(271, 00, 0, 127), // Lombre (M) (Hardy)
            new NPCLock(271, 18, 0, 127), // Lombre (M) (Bashful)
            new NPCLock(271, 12, 1, 127), // Lombre (F) (Serious)
        ]);

    public static readonly TeamLock XParas = new(
        046, // Paras
        [
            new NPCLock(336, 24, 0, 127), // Seviper (M) (Quirky)
            new NPCLock(198, 06, 1, 127), // Murkrow (F) (Docile)
        ]);

    public static readonly TeamLock XGrowlithe = new(
        058, // Growlithe
        [
            new NPCLock(336, 24, 0, 127), // Seviper (M) (Quirky)
            new NPCLock(198, 06, 1, 127), // Murkrow (F) (Docile)
            new NPCLock(046), // Shadow Paras
        ]);

    public static readonly TeamLock XGrowlitheParasSeen = new(
        058, // Growlithe
        SeenParas,
        [
            new NPCLock(336, 24, 0, 127), // Seviper (M) (Quirky)
            new NPCLock(198, 06, 1, 127), // Murkrow (F) (Docile)
            new NPCLock(046, true), // Shadow Paras (Seen)
        ]);

    public static readonly TeamLock XPidgeotto = new(
        017, // Pidgeotto
        [
            new NPCLock(015), // Shadow Beedrill
            new NPCLock(162, 12, 0, 127), // Furret (M) (Serious)
            new NPCLock(176, 18, 0, 031), // Togetic (M) (Bashful)
        ]);

    public static readonly TeamLock XPidgeottoBeedrillSeen = new(
        017, // Pidgeotto
        SeenBeedrill,
        [
            new NPCLock(015, true), // Shadow Beedrill (Seen)
            new NPCLock(162, 12, 0, 127), // Furret (M) (Serious)
            new NPCLock(176, 18, 0, 031), // Togetic (M) (Bashful)
        ]);

    public static readonly TeamLock XTangela = new(
        114, // Tangela
        [
            new NPCLock(038, 12, 1, 191), // Ninetales (F) (Serious)
            new NPCLock(189, 06, 0, 127), // Jumpluff (M) (Docile)
            new NPCLock(184, 00, 1, 127), // Azumarill (F) (Hardy)
        ]);

    public static readonly TeamLock XButterfree = new(
        012, // Butterfree
        [
            new NPCLock(038, 12, 1, 191), // Ninetales (F) (Serious)
            new NPCLock(189, 06, 0, 127), // Jumpluff (M) (Docile)
            new NPCLock(184, 00, 1, 127), // Azumarill (F) (Hardy)
            new NPCLock(114), // Shadow Tangela
        ]);

    public static readonly TeamLock XButterfreeTangelaSeen = new(
        012, // Butterfree
        SeenTangela,
        [
            new NPCLock(038, 12, 1, 191), // Ninetales (F) (Serious)
            new NPCLock(189, 06, 0, 127), // Jumpluff (M) (Docile)
            new NPCLock(184, 00, 1, 127), // Azumarill (F) (Hardy)
            new NPCLock(114, true), // Shadow Tangela (Seen)
        ]);

    public static readonly TeamLock XMagneton = new(
        082, // Magneton
        [
            new NPCLock(292, 18, 2, 255), // Shedinja (-) (Bashful)
            new NPCLock(202, 00, 0, 127), // Wobbuffet (M) (Hardy)
            new NPCLock(329, 12, 1, 127), // Vibrava (F) (Serious)
        ]);

    public static readonly TeamLock XVenomoth = new(
        049, // Venomoth
        [
            new NPCLock(055, 18, 1, 127), // Golduck (F) (Bashful)
            new NPCLock(237, 24, 0, 000), // Hitmontop (M) (Quirky)
            new NPCLock(297, 12, 0, 063), // Hariyama (M) (Serious)
        ]);

    public static readonly TeamLock XWeepinbell = new(
        070, // Weepinbell
        [
            new NPCLock(055, 18, 1, 127), // Golduck (F) (Bashful)
            new NPCLock(237, 24, 0, 000), // Hitmontop (M) (Quirky)
            new NPCLock(297, 12, 0, 063), // Hariyama (M) (Serious)
            new NPCLock(049), // Shadow Venomoth
        ]);

    public static readonly TeamLock XWeepinbellVenomothSeen = new(
        070, // Weepinbell
        SeenVenomoth,
        [
            new NPCLock(055, 18, 1, 127), // Golduck (F) (Bashful)
            new NPCLock(237, 24, 0, 000), // Hitmontop (M) (Quirky)
            new NPCLock(297, 12, 0, 063), // Hariyama (M) (Serious)
            new NPCLock(049, true), // Shadow Venomoth (Seen)
        ]);

    public static readonly TeamLock XArbok = new(
        024, // Arbok
        [
            new NPCLock(367, 06, 0, 127), // Huntail (M) (Docile)
            new NPCLock(332, 00, 1, 127), // Cacturne (F) (Hardy)
            new NPCLock(110, 12, 1, 127), // Weezing (F) (Serious)
            new NPCLock(217, 18, 1, 127), // Ursaring (F) (Bashful)
        ]);

    public static readonly TeamLock XPrimeape = new(
        057, // Primeape
        [
            new NPCLock(305, 18, 1, 127), // Lairon (F) (Bashful)
            new NPCLock(364, 12, 1, 127), // Sealeo (F) (Serious)
            new NPCLock(199, 06, 1, 127), // Slowking (F) (Docile)
            new NPCLock(217, 24, 0, 127), // Ursaring (M) (Quirky)
        ]);

    public static readonly TeamLock XHypno = new(
        097, // Hypno
        [
            new NPCLock(305, 18, 1, 127), // Lairon (F) (Bashful)
            new NPCLock(364, 12, 1, 127), // Sealeo (F) (Serious)
            new NPCLock(199, 06, 1, 127), // Slowking (F) (Docile)
            new NPCLock(217, 24, 0, 127), // Ursaring (M) (Quirky)
            new NPCLock(057), // Shadow Primeape
        ]);

    public static readonly TeamLock XHypnoPrimeapeSeen = new(
        097, // Hypno
        SeenPrimeape,
        [
            new NPCLock(305, 18, 1, 127), // Lairon (F) (Bashful)
            new NPCLock(364, 12, 1, 127), // Sealeo (F) (Serious)
            new NPCLock(199, 06, 1, 127), // Slowking (F) (Docile)
            new NPCLock(217, 24, 0, 127), // Ursaring (M) (Quirky)
            new NPCLock(057, true), // Shadow Primeape (Seen)
        ]);

    public static readonly TeamLock XGolduck = new(
        055, // Golduck
        [
            new NPCLock(342, 24, 0, 127), // Crawdaunt (M) (Quirky)
            new NPCLock(279, 06, 1, 127), // Pelipper (F) (Docile)
            new NPCLock(226, 18, 1, 127), // Mantine (F) (Bashful)
        ]);

    public static readonly TeamLock XSableye = new(
        302, // Sableye
        [
            new NPCLock(342, 24, 0, 127), // Crawdaunt (M) (Quirky)
            new NPCLock(279, 06, 1, 127), // Pelipper (F) (Docile)
            new NPCLock(226, 18, 1, 127), // Mantine (F) (Bashful)
            new NPCLock(055), // Shadow Golduck
        ]);

    public static readonly TeamLock XSableyeGolduckSeen = new(
        302, // Sableye
        SeenGolduck,
        [
            new NPCLock(342, 24, 0, 127), // Crawdaunt (M) (Quirky)
            new NPCLock(279, 06, 1, 127), // Pelipper (F) (Docile)
            new NPCLock(226, 18, 1, 127), // Mantine (F) (Bashful)
            new NPCLock(055, true), // Shadow Golduck (Seen)
        ]);

    public static readonly TeamLock XDodrio = new(
        085, // Dodrio
        [
            new NPCLock(178, 18, 1, 127), // Xatu (F) (Bashful)
        ]);

    public static readonly TeamLock XRaticate = new(
        020, // Raticate
        [
            new NPCLock(178, 18, 1, 127), // Xatu (F) (Bashful)
            new NPCLock(085), // Shadow Dodrio
            new NPCLock(340, 18, 0, 127), // Whiscash (M) (Bashful)
        ]);

    public static readonly TeamLock XRaticateDodrioSeen = new(
        020, // Raticate
        SeenDodrio,
        [
            new NPCLock(178, 18, 1, 127), // Xatu (F) (Bashful)
            new NPCLock(085, true), // Shadow Dodrio (Seen)
            new NPCLock(340, 18, 0, 127), // Whiscash (M) (Bashful)
        ]);

    public static readonly TeamLock XFarfetchd = new(
        083, // Farfetch’d
        [
            new NPCLock(282, 12, 0, 127), // Gardevoir (M) (Serious)
            new NPCLock(368, 00, 1, 127), // Gorebyss (F) (Hardy)
            new NPCLock(315, 24, 0, 127), // Roselia (M) (Quirky)
        ]);

    public static readonly TeamLock XAltaria = new(
        334, // Altaria
        [
            new NPCLock(282, 12, 0, 127), // Gardevoir (M) (Serious)
            new NPCLock(368, 00, 1, 127), // Gorebyss (F) (Hardy)
            new NPCLock(315, 24, 0, 127), // Roselia (M) (Quirky)
            new NPCLock(083), // Shadow Farfetch’d
        ]);

    public static readonly TeamLock XAltariaFarfetchdSeen = new(
        334, // Altaria
        SeenFarfetchd,
        [
            new NPCLock(282, 12, 0, 127), // Gardevoir (M) (Serious)
            new NPCLock(368, 00, 1, 127), // Gorebyss (F) (Hardy)
            new NPCLock(315, 24, 0, 127), // Roselia (M) (Quirky)
            new NPCLock(083, true), // Shadow Farfetch’d (Seen)
        ]);

    public static readonly TeamLock XKangaskhan = new(
        115, // Kangaskhan
        [
            new NPCLock(101, 00, 2, 255), // Electrode (-) (Hardy)
            new NPCLock(200, 18, 1, 127), // Misdreavus (F) (Bashful)
            new NPCLock(344, 12, 2, 255), // Claydol (-) (Serious)
        ]);

    public static readonly TeamLock XBanette = new(
        354, // Banette
        [
            new NPCLock(101, 00, 2, 255), // Electrode (-) (Hardy)
            new NPCLock(200, 18, 1, 127), // Misdreavus (F) (Bashful)
            new NPCLock(344, 12, 2, 255), // Claydol (-) (Serious)
            new NPCLock(115), // Shadow Kangaskhan
        ]);

    public static readonly TeamLock XBanetteKangaskhanSeen = new(
        354, // Banette
        SeenKangaskhan,
        [
            new NPCLock(101, 00, 2, 255), // Electrode (-) (Hardy)
            new NPCLock(200, 18, 1, 127), // Misdreavus (F) (Bashful)
            new NPCLock(344, 12, 2, 255), // Claydol (-) (Serious)
            new NPCLock(115, true), // Shadow Kangaskhan (Seen)
        ]);

    public static readonly TeamLock XMagmar = new(
        126, // Magmar
        [
            new NPCLock(229, 18, 0, 127), // Houndoom (M) (Bashful)
            new NPCLock(038, 18, 0, 191), // Ninetales (M) (Bashful)
            new NPCLock(045, 00, 1, 127), // Vileplume (F) (Hardy)
        ]);

    public static readonly TeamLock XPinsir = new(
        127, // Pinsir
        [
            new NPCLock(229, 18, 0, 127), // Houndoom (M) (Bashful)
            new NPCLock(038, 18, 0, 191), // Ninetales (M) (Bashful)
            new NPCLock(045, 00, 1, 127), // Vileplume (F) (Hardy)
            new NPCLock(126), // Shadow Magmar
        ]);

    public static readonly TeamLock XPinsirMagmarSeen = new(
        127, // Pinsir
        SeenMagmar,
        [
            new NPCLock(229, 18, 0, 127), // Houndoom (M) (Bashful)
            new NPCLock(038, 18, 0, 191), // Ninetales (M) (Bashful)
            new NPCLock(045, 00, 1, 127), // Vileplume (F) (Hardy)
            new NPCLock(126, true), // Shadow Magmar (Seen)
        ]);

    public static readonly TeamLock XRapidash = new(
        078, // Rapidash
        [
            new NPCLock(323, 24, 0, 127), // Camerupt (M) (Quirky)
            new NPCLock(110, 06, 0, 127), // Weezing (M) (Docile)
            new NPCLock(089, 12, 1, 127), // Muk (F) (Serious)
        ]);

    public static readonly TeamLock XMagcargo = new(
        219, // Magcargo
        [
            new NPCLock(323, 24, 0, 127), // Camerupt (M) (Quirky)
            new NPCLock(110, 06, 0, 127), // Weezing (M) (Docile)
            new NPCLock(089, 12, 1, 127), // Muk (F) (Serious)
            new NPCLock(078), // Shadow Rapidash
        ]);

    public static readonly TeamLock XMagcargoRapidashSeen = new(
        219, // Magcargo
        SeenRapidash,
        [
            new NPCLock(323, 24, 0, 127), // Camerupt (M) (Quirky)
            new NPCLock(110, 06, 0, 127), // Weezing (M) (Docile)
            new NPCLock(089, 12, 1, 127), // Muk (F) (Serious)
            new NPCLock(078, true), // Shadow Rapidash (Seen)
        ]);

    public static readonly TeamLock XHitmonchan = new(
        107, // Hitmonchan
        [
            new NPCLock(308, 24, 0, 127), // Medicham (M) (Quirky)
            new NPCLock(076, 06, 1, 127), // Golem (F) (Docile)
            new NPCLock(178, 18, 1, 127), // Xatu (F) (Bashful)
        ]);

    public static readonly TeamLock XHitmonlee = new(
        106, // Hitmonlee
        [
            new NPCLock(326, 18, 0, 127), // Grumpig (M) (Bashful)
            new NPCLock(227, 12, 1, 127), // Skarmory (F) (Serious)
            new NPCLock(375, 06, 2, 255), // Metang (-) (Docile)
            new NPCLock(297, 24, 1, 063), // Hariyama (F) (Quirky)
        ]);

    public static readonly TeamLock XLickitung = new(
        108, // Lickitung
        [
            new NPCLock(171, 24, 0, 127), // Lanturn (M) (Quirky)
            new NPCLock(082, 06, 2, 255), // Magneton (-) (Docile)
        ]);

    public static readonly TeamLock XScyther = new(
        123, // Scyther
        [
            new NPCLock(234, 06, 1, 127), // Stantler (F) (Docile)
            new NPCLock(295, 24, 0, 127), // Exploud (M) (Quirky)
        ]);

    public static readonly TeamLock XChansey = new(
        113, // Chansey
        [
            new NPCLock(234, 06, 1, 127), // Stantler (F) (Docile)
            new NPCLock(295, 24, 0, 127), // Exploud (M) (Quirky)
            new NPCLock(123), // Shadow Scyther
        ]);

    public static readonly TeamLock XChanseyScytherSeen = new(
        113, // Chansey
        SeenScyther,
        [
            new NPCLock(234, 06, 1, 127), // Stantler (F) (Docile)
            new NPCLock(295, 24, 0, 127), // Exploud (M) (Quirky)
            new NPCLock(123, true), // Shadow Scyther (Seen)
        ]);

    public static readonly TeamLock XSolrock = new(
        338, // Solrock
        [
            new NPCLock(375, 24, 2, 255), // Metang (-) (Quirky)
            new NPCLock(195, 06, 0, 127), // Quagsire (M) (Docile)
            new NPCLock(212, 00, 1, 127), // Scizor (F) (Hardy)
        ]);

    public static readonly TeamLock XStarmie = new(
        121, // Starmie
        [
            new NPCLock(375, 24, 2, 255), // Metang (-) (Quirky)
            new NPCLock(195, 06, 0, 127), // Quagsire (M) (Docile)
            new NPCLock(212, 00, 1, 127), // Scizor (F) (Hardy)
            new NPCLock(338), // Shadow Solrock
            new NPCLock(351, 18, 0, 127), // Castform (M) (Bashful)
        ]);

    public static readonly TeamLock XStarmieSolrockSeen = new(
        121, // Starmie
        SeenSolrock,
        [
            new NPCLock(375, 24, 2, 255), // Metang (-) (Quirky)
            new NPCLock(195, 06, 0, 127), // Quagsire (M) (Docile)
            new NPCLock(212, 00, 1, 127), // Scizor (F) (Hardy)
            new NPCLock(338, true), // Shadow Solrock (Seen)
            new NPCLock(351, 18, 0, 127), // Castform (M) (Bashful)
        ]);

    public static readonly TeamLock XElectabuzz = new(
        125, // Electabuzz
        [
            new NPCLock(277), // Shadow Swellow
            new NPCLock(065, 24, 0, 063), // Alakazam (M) (Quirky)
            new NPCLock(230, 6, 1, 127), // Kingdra (F) (Docile)
            new NPCLock(214, 18, 1, 127), // Heracross (F) (Bashful)
        ]);

    public static readonly TeamLock XElectabuzzSwellowSeen = new(
        125, // Electabuzz
        SeenSwellow,
        [
            new NPCLock(277, true), // Shadow Swellow (Seen)
            new NPCLock(065, 24, 0, 063), // Alakazam (M) (Quirky)
            new NPCLock(230, 6, 1, 127), // Kingdra (F) (Docile)
            new NPCLock(214, 18, 1, 127), // Heracross (F) (Bashful)
        ]);

    public static readonly TeamLock XSnorlax = new(
        143, // Snorlax
        [
            new NPCLock(277), // Shadow Swellow
            new NPCLock(065, 24, 0, 063), // Alakazam (M) (Quirky)
            new NPCLock(230, 6, 1, 127), // Kingdra (F) (Docile)
            new NPCLock(214, 18, 1, 127), // Heracross (F) (Bashful)
            new NPCLock(125), // Shadow Electabuzz
        ]);

    public static readonly TeamLock XSnorlaxSwellowSeen = new(
        143, // Snorlax
        SeenSwellow,
        [
            new NPCLock(277, true), // Shadow Swellow (Seen)
            new NPCLock(065, 24, 0, 063), // Alakazam (M) (Quirky)
            new NPCLock(230, 6, 1, 127), // Kingdra (F) (Docile)
            new NPCLock(214, 18, 1, 127), // Heracross (F) (Bashful)
            new NPCLock(125), // Shadow Electabuzz
        ]);

    public static readonly TeamLock XSnorlaxSwellowElectabuzzSeen = new(
        143, // Snorlax
        SeenSwellowElectabuzz,
        [
            new NPCLock(277, true), // Shadow Swellow (Seen)
            new NPCLock(065, 24, 0, 063), // Alakazam (M) (Quirky)
            new NPCLock(230, 6, 1, 127), // Kingdra (F) (Docile)
            new NPCLock(214, 18, 1, 127), // Heracross (F) (Bashful)
            new NPCLock(125, true), // Shadow Electabuzz
        ]);

    public static readonly TeamLock XPoliwrath = new(
        062, // Poliwrath
        [
            new NPCLock(199, 18, 0, 127), // Slowking (M) (Bashful)
            new NPCLock(217, 18, 0, 127), // Ursaring (M) (Bashful)
            new NPCLock(306, 24, 0, 127), // Aggron (M) (Quirky)
            new NPCLock(365, 06, 1, 127), // Walrein (F) (Docile)
        ]);

    public static readonly TeamLock XMrMime = new(
        122, // Mr. Mime
        [
            new NPCLock(199, 18, 0, 127), // Slowking (M) (Bashful)
            new NPCLock(217, 18, 0, 127), // Ursaring (M) (Bashful)
            new NPCLock(306, 24, 0, 127), // Aggron (M) (Quirky)
            new NPCLock(365, 06, 1, 127), // Walrein (F) (Docile)
            new NPCLock(062), // Shadow Poliwrath
        ]);

    public static readonly TeamLock XMrMimePoliwrathSeen = new(
        122, // Mr. Mime
        SeenPoliwrath,
        [
            new NPCLock(199, 18, 0, 127), // Slowking (M) (Bashful)
            new NPCLock(217, 18, 0, 127), // Ursaring (M) (Bashful)
            new NPCLock(306, 24, 0, 127), // Aggron (M) (Quirky)
            new NPCLock(365, 06, 1, 127), // Walrein (F) (Docile)
            new NPCLock(062, true), // Shadow Poliwrath (Seen)
        ]);

    public static readonly TeamLock XDugtrio = new(
        051, // Dugtrio
        [
            new NPCLock(362, 00, 0, 127), // Glalie (M) (Hardy)
            new NPCLock(181, 18, 0, 127), // Ampharos (M) (Bashful)
            new NPCLock(286, 06, 1, 127), // Breloom (F) (Docile)
            new NPCLock(232, 12, 0, 127), // Donphan (M) (Serious)
        ]);

    public static readonly TeamLock XManectric = new(
        310, // Manectric
        [
            new NPCLock(291, 06, 1, 127), // Ninjask (F) (Docile)
        ]);

    public static readonly TeamLock XSalamence = new(
        373, // Salamence
        [
            new NPCLock(291, 06, 1, 127), // Ninjask (F) (Docile)
            new NPCLock(310), // Shadow Manectric
        ]);

    public static readonly TeamLock XMarowak = new(
        105, // Marowak
        [
            new NPCLock(291, 06, 1, 127), // Ninjask (F) (Docile)
            new NPCLock(310), // Shadow Manectric
            new NPCLock(373), // Shadow Salamence
            new NPCLock(330, 24, 0, 127), // Flygon (M) (Quirky)
        ]);

    public static readonly TeamLock XLapras = new(
        131, // Lapras
        [
            new NPCLock(291, 06, 1, 127), // Ninjask (F) (Docile)
            new NPCLock(310), // Shadow Manectric
            new NPCLock(373), // Shadow Salamence
            new NPCLock(330, 24, 0, 127), // Flygon (M) (Quirky)
            new NPCLock(105), // Shadow Marowak
        ]);

    public static readonly TeamLock XSalamenceManectricSeen = new(
        373, // Salamence
        SeenManectric,
        [
            new NPCLock(291, 06, 1, 127), // Ninjask (F) (Docile)
            new NPCLock(310, true), // Shadow Manectric (Seen)
        ]);

    public static readonly TeamLock XMarowakManectricSeen = new(
        105, // Marowak
        SeenManectric,
        [
            new NPCLock(291, 06, 1, 127), // Ninjask (F) (Docile)
            new NPCLock(310, true), // Shadow Manectric (Seen)
            new NPCLock(373), // Shadow Salamence
            new NPCLock(330, 24, 0, 127), // Flygon (M) (Quirky)
        ]);

    public static readonly TeamLock XMarowakManectricSalamenceSeen = new(
        105, // Marowak
        SeenManectricSalamence,
        [
            new NPCLock(291, 06, 1, 127), // Ninjask (F) (Docile)
            new NPCLock(310, true), // Shadow Manectric (Seen)
            new NPCLock(373, true), // Shadow Salamence (Seen)
            new NPCLock(330, 24, 0, 127), // Flygon (M) (Quirky)
        ]);

    public static readonly TeamLock XLaprasManectricSeen = new(
        131, // Lapras
        SeenManectric,
        [
            new NPCLock(291, 06, 1, 127), // Ninjask (F) (Docile)
            new NPCLock(310, true), // Shadow Manectric (Seen)
            new NPCLock(373), // Shadow Salamence
            new NPCLock(330, 24, 0, 127), // Flygon (M) (Quirky)
            new NPCLock(105), // Shadow Marowak
        ]);

    public static readonly TeamLock XLaprasManectricSalamenceSeen = new(
        131, // Lapras
        SeenManectricSalamence,
        [
            new NPCLock(291, 06, 1, 127), // Ninjask (F) (Docile)
            new NPCLock(310, true), // Shadow Manectric (Seen)
            new NPCLock(373, true), // Shadow Salamence (Seen)
            new NPCLock(330, 24, 0, 127), // Flygon (M) (Quirky)
            new NPCLock(105), // Shadow Marowak
        ]);

    public static readonly TeamLock XLaprasManectricMarowakSeen = new(
        131, // Lapras
        SeenManectricMarowak,
        [
            new NPCLock(291, 06, 1, 127), // Ninjask (F) (Docile)
            new NPCLock(310, true), // Shadow Manectric (Seen)
            new NPCLock(373), // Shadow Salamence
            new NPCLock(330, 24, 0, 127), // Flygon (M) (Quirky)
            new NPCLock(105, true), // Shadow Marowak (Seen)
        ]);

    public static readonly TeamLock XLaprasManectricSalamenceMarowakSeen = new(
        131, // Lapras
        SeenManectricMarowakSalamence,
        [
            new NPCLock(291, 06, 1, 127), // Ninjask (F) (Docile)
            new NPCLock(310, true), // Shadow Manectric (Seen)
            new NPCLock(373, true), // Shadow Salamence (Seen)
            new NPCLock(330, 24, 0, 127), // Flygon (M) (Quirky)
            new NPCLock(105, true), // Shadow Marowak (Seen)
        ]);

    public static readonly TeamLock XMoltres = new(
        146, // Moltres
        [
            new NPCLock(112), // Shadow Rhydon
        ]);

    public static readonly TeamLock XExeggutor = new(
        103, // Exeggutor
        [
            new NPCLock(112), // Shadow Rhydon
            new NPCLock(146), // Shadow Moltres
        ]);

    public static readonly TeamLock XTauros = new(
        128, // Tauros
        [
            new NPCLock(112), // Shadow Rhydon
            new NPCLock(146), // Shadow Moltres
            new NPCLock(103), // Shadow Exeggutor
        ]);

    public static readonly TeamLock XArticuno = new(
        144, // Articuno
        [
            new NPCLock(112), // Shadow Rhydon
            new NPCLock(146), // Shadow Moltres
            new NPCLock(103), // Shadow Exeggutor
            new NPCLock(128), // Shadow Tauros
        ]);

    public static readonly TeamLock XZapdos = new(
        145, // Zapdos
        [
            new NPCLock(112), // Shadow Rhydon
            new NPCLock(146), // Shadow Moltres
            new NPCLock(103), // Shadow Exeggutor
            new NPCLock(128), // Shadow Tauros
            new NPCLock(144), // Shadow Articuno
        ]);

    public static readonly TeamLock XExeggutorRhydonMoltresSeen = new(
        103, // Exeggutor
        SeenRhydonMoltres,
        [
            new NPCLock(112, true), // Shadow Rhydon (Seen)
            new NPCLock(146, true), // Shadow Moltres (Seen)
        ]);

    public static readonly TeamLock XTaurosRhydonMoltresSeen = new(
        128, // Tauros
        SeenRhydonMoltres,
        [
            new NPCLock(112, true), // Shadow Rhydon (Seen)
            new NPCLock(146, true), // Shadow Moltres (Seen)
            new NPCLock(103), // Shadow Exeggutor
        ]);

    public static readonly TeamLock XTaurosRhydonMoltresExeggutorSeen = new(
        128, // Tauros
        SeenRhydonMoltresExeggutor,
        [
            new NPCLock(112, true), // Shadow Rhydon (Seen)
            new NPCLock(146, true), // Shadow Moltres (Seen)
            new NPCLock(103, true), // Shadow Exeggutor (Seen)
        ]);

    public static readonly TeamLock XArticunoRhydonMoltresSeen = new(
        144, // Articuno
        SeenRhydonMoltres,
        [
            new NPCLock(112, true), // Shadow Rhydon (Seen)
            new NPCLock(146, true), // Shadow Moltres (Seen)
            new NPCLock(103), // Shadow Exeggutor
            new NPCLock(128), // Shadow Tauros
        ]);

    public static readonly TeamLock XArticunoRhydonMoltresTaurosSeen = new(
        144, // Articuno
        SeenRhydonMoltresTauros,
        [
            new NPCLock(112, true), // Shadow Rhydon (Seen)
            new NPCLock(146, true), // Shadow Moltres (Seen)
            new NPCLock(103), // Shadow Exeggutor
            new NPCLock(128, true), // Shadow Tauros (Seen)
        ]);

    public static readonly TeamLock XArticunoRhydonMoltresExeggutorSeen = new(
        144, // Articuno
        SeenRhydonMoltresExeggutor,
        [
            new NPCLock(112, true), // Shadow Rhydon (Seen)
            new NPCLock(146, true), // Shadow Moltres (Seen)
            new NPCLock(103, true), // Shadow Exeggutor (Seen)
            new NPCLock(128), // Shadow Tauros
        ]);

    public static readonly TeamLock XArticunoRhydonMoltresExeggutorTaurosSeen = new(
        144, // Articuno
        SeenRhydonMoltresExeggutorTauros,
        [
            new NPCLock(112, true), // Shadow Rhydon (Seen)
            new NPCLock(146, true), // Shadow Moltres (Seen)
            new NPCLock(103, true), // Shadow Exeggutor (Seen)
            new NPCLock(128, true), // Shadow Tauros (Seen)
        ]);

    public static readonly TeamLock XZapdosRhydonMoltresSeen = new(
        145, // Zapdos
        SeenRhydonMoltres,
        [
            new NPCLock(112, true), // Shadow Rhydon (Seen)
            new NPCLock(146, true), // Shadow Moltres (Seen)
            new NPCLock(103), // Shadow Exeggutor
            new NPCLock(128), // Shadow Tauros
            new NPCLock(144), // Shadow Articuno
        ]);

    public static readonly TeamLock XZapdosRhydonMoltresTaurosSeen = new(
        145, // Zapdos
        SeenRhydonMoltresTauros,
        [
            new NPCLock(112, true), // Shadow Rhydon (Seen)
            new NPCLock(146, true), // Shadow Moltres (Seen)
            new NPCLock(103), // Shadow Exeggutor
            new NPCLock(128, true), // Shadow Tauros (Seen)
            new NPCLock(144), // Shadow Articuno
        ]);

    public static readonly TeamLock XZapdosRhydonMoltresArticunoSeen = new(
        145, // Zapdos
        SeenRhydonMoltresArticuno,
        [
            new NPCLock(112, true), // Shadow Rhydon (Seen)
            new NPCLock(146, true), // Shadow Moltres (Seen)
            new NPCLock(103), // Shadow Exeggutor
            new NPCLock(128), // Shadow Tauros
            new NPCLock(144, true), // Shadow Articuno (Seen)
        ]);

    public static readonly TeamLock XZapdosRhydonMoltresExeggutorSeen = new(
        145, // Zapdos
        SeenRhydonMoltresExeggutor,
        [
            new NPCLock(112, true), // Shadow Rhydon (Seen)
            new NPCLock(146, true), // Shadow Moltres (Seen)
            new NPCLock(103, true), // Shadow Exeggutor (Seen)
            new NPCLock(128), // Shadow Tauros
            new NPCLock(144), // Shadow Articuno
        ]);

    public static readonly TeamLock XZapdosRhydonMoltresTaurosArticunoSeen = new(
        145, // Zapdos
        SeenRhydonMoltresTaurosArticuno,
        [
            new NPCLock(112, true), // Shadow Rhydon (Seen)
            new NPCLock(146, true), // Shadow Moltres (Seen)
            new NPCLock(103), // Shadow Exeggutor
            new NPCLock(128, true), // Shadow Tauros (Seen)
            new NPCLock(144, true), // Shadow Articuno (Seen)
        ]);

    public static readonly TeamLock XZapdosRhydonMoltresExeggutorTaurosSeen = new(
        145, // Zapdos
        SeenRhydonMoltresExeggutorTauros,
        [
            new NPCLock(112, true), // Shadow Rhydon (Seen)
            new NPCLock(146, true), // Shadow Moltres (Seen)
            new NPCLock(103, true), // Shadow Exeggutor (Seen)
            new NPCLock(128, true), // Shadow Tauros (Seen)
            new NPCLock(144), // Shadow Articuno
        ]);

    public static readonly TeamLock XZapdosRhydonMoltresExeggutorArticunoSeen = new(
        145, // Zapdos
        SeenRhydonMoltresExeggutorArticuno,
        [
            new NPCLock(112, true), // Shadow Rhydon (Seen)
            new NPCLock(146, true), // Shadow Moltres (Seen)
            new NPCLock(103, true), // Shadow Exeggutor (Seen)
            new NPCLock(128), // Shadow Tauros
            new NPCLock(144, true), // Shadow Articuno (Seen)
        ]);

    public static readonly TeamLock XZapdosRhydonMoltresExeggutorTaurosArticunoSeen = new(
        145, // Zapdos
        SeenRhydonMoltresExeggutorTaurosArticuno,
        [
            new NPCLock(112, true), // Shadow Rhydon (Seen)
            new NPCLock(146, true), // Shadow Moltres (Seen)
            new NPCLock(103, true), // Shadow Exeggutor (Seen)
            new NPCLock(128, true), // Shadow Tauros (Seen)
            new NPCLock(144, true), // Shadow Articuno (Seen)
        ]);

    public static readonly TeamLock XDragonite = new(
        149, // Dragonite
        [
            new NPCLock(272, 00, 0, 127), // Ludicolo (M) (Hardy)
            new NPCLock(272, 18, 0, 127), // Ludicolo (M) (Bashful)
            new NPCLock(272, 12, 1, 127), // Ludicolo (F) (Serious)
            new NPCLock(272, 12, 1, 127), // Ludicolo (F) (Serious)
            new NPCLock(272, 00, 0, 127), // Ludicolo (M) (Hardy)
        ]);
}
