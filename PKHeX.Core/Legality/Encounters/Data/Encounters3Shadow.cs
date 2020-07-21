// ReSharper disable StringLiteralTypo
namespace PKHeX.Core
{
    public static class Encounters3Shadow
    {
        #region Colosseum

        public static readonly TeamLock CMakuhita = new TeamLock(
            296, // Makuhita
            new[] {
                new NPCLock(355, 24, 0, 127), // Duskull (M) (Quirky)
                new NPCLock(167, 00, 1, 127), // Spinarak (F) (Hardy)
        });

        public static readonly TeamLock CGligar = new TeamLock(
            207, // Gligar
            new[] {
                new NPCLock(216, 12, 0, 127), // Teddiursa (M) (Serious)
                new NPCLock(039, 06, 1, 191), // Jigglypuff (F) (Docile)
                new NPCLock(285, 18, 0, 127), // Shroomish (M) (Bashful)
        });

        public static readonly TeamLock CMurkrow = new TeamLock(
            198, // Murkrow
            new[] {
                new NPCLock(318, 06, 0, 127), // Carvanha (M) (Docile)
                new NPCLock(274, 12, 1, 127), // Nuzleaf (F) (Serious)
                new NPCLock(228, 18, 0, 127), // Houndour (M) (Bashful)
        });

        public static readonly TeamLock CHeracross = new TeamLock(
            214, // Heracross
            new[] {
                new NPCLock(284, 00, 0, 127), // Masquerain (M) (Hardy)
                new NPCLock(168, 00, 1, 127), // Ariados (F) (Hardy)
        });

        public static readonly TeamLock CUrsaring = new TeamLock(
            217, // Ursaring
            new[] {
                new NPCLock(067, 20, 1, 063), // Machoke (F) (Calm)
                new NPCLock(259, 16, 0, 031), // Marshtomp (M) (Mild)
                new NPCLock(275, 21, 1, 127), // Shiftry (F) (Gentle)
        });

        #endregion

        #region E-Reader

        public static readonly TeamLock ETogepi = new TeamLock(
            175, // Togepi
            new[] {
                new NPCLock(302, 23, 0, 127), // Sableye (M) (Careful)
                new NPCLock(088, 08, 0, 127), // Grimer (M) (Impish)
                new NPCLock(316, 24, 0, 127), // Gulpin (M) (Quirky)
                new NPCLock(175, 22, 1, 031), // Togepi (F) (Sassy) -- itself!
        });

        public static readonly TeamLock EMareep = new TeamLock(
            179, // Mareep
            new[] {
                new NPCLock(300, 04, 1, 191), // Skitty (F) (Naughty)
                new NPCLock(211, 10, 1, 127), // Qwilfish (F) (Timid)
                new NPCLock(355, 12, 1, 127), // Duskull (F) (Serious)
                new NPCLock(179, 16, 1, 127), // Mareep (F) (Mild) -- itself!
        });

        public static readonly TeamLock EScizor = new TeamLock(
            212, // Scizor
            new[] {
                new NPCLock(198, 13, 1, 191), // Murkrow (F) (Jolly)
                new NPCLock(344, 02, 2, 255), // Claydol (-) (Brave)
                new NPCLock(208, 03, 0, 127), // Steelix (M) (Adamant)
                new NPCLock(212, 11, 0, 127), // Scizor (M) (Hasty) -- itself!
        });

        #endregion

        #region XD

        public static readonly TeamLock XRalts = new TeamLock(
            280, // Ralts
            new[] {
                new NPCLock(064, 00, 0, 063), // Kadabra (M) (Hardy)
                new NPCLock(180, 06, 1, 127), // Flaaffy (F) (Docile)
                new NPCLock(288, 18, 0, 127), // Vigoroth (M) (Bashful)
        });

        public static readonly TeamLock XPoochyena = new TeamLock(
            261, // Poochyena
            new[] {
                new NPCLock(041, 12, 1, 127), // Zubat (F) (Serious)
        });

        public static readonly TeamLock XLedyba = new TeamLock(
            165, // Ledyba
            new[] {
                new NPCLock(276, 00, 1, 127), // Taillow (F) (Hardy)
        });

        public static readonly TeamLock XSphealCipherLab = new TeamLock(
            363, // Spheal
            "Cipher Lab",
            new[] {
                new NPCLock(116, 24, 0, 063), // Horsea (M) (Quirky)
                new NPCLock(118, 12, 1, 127), // Goldeen (F) (Serious)
        });

        public static readonly TeamLock XSphealPhenacCityandPost = new TeamLock(
            363, // Spheal
            "Phenac City and Post",
            new[] {
                new NPCLock(116, 24, 0, 063), // Horsea (M) (Quirky)
                new NPCLock(118, 12, 1, 127), // Goldeen (F) (Serious)
                new NPCLock(374, 00, 2, 255), // Beldum (-) (Hardy)
        });

        public static readonly TeamLock XGulpin = new TeamLock(
            316, // Gulpin
            new[] {
                new NPCLock(109, 12, 1, 127), // Koffing (F) (Serious)
                new NPCLock(088, 06, 0, 127), // Grimer (M) (Docile)
        });

        public static readonly TeamLock XSeedotCipherLab = new TeamLock(
            273, // Seedot
            "Cipher Lab",
            new[] {
                new NPCLock(043, 06, 0, 127), // Oddish (M) (Docile)
                new NPCLock(331, 24, 1, 127), // Cacnea (F) (Quirky)
                new NPCLock(285, 18, 1, 127), // Shroomish (F) (Bashful)
                new NPCLock(270, 00, 0, 127), // Lotad (M) (Hardy)
                new NPCLock(204, 12, 0, 127), // Pineco (M) (Serious)
        });

        public static readonly TeamLock XSeedotPhenacCity = new TeamLock(
            273, // Seedot
            "Phenac City",
            new[] {
                new NPCLock(043, 06, 0, 127), // Oddish (M) (Docile)
                new NPCLock(331, 24, 1, 127), // Cacnea (F) (Quirky)
                new NPCLock(285, 00, 1, 127), // Shroomish (F) (Hardy)
                new NPCLock(270, 00, 1, 127), // Lotad (F) (Hardy)
                new NPCLock(204, 06, 0, 127), // Pineco (M) (Docile)
        });

        public static readonly TeamLock XSeedotPost = new TeamLock(
            273, // Seedot
            "Post",
            new[] {
                new NPCLock(045, 06, 0, 127), // Vileplume (M) (Docile)
                new NPCLock(332, 24, 1, 127), // Cacturne (F) (Quirky)
                new NPCLock(286, 00, 1, 127), // Breloom (F) (Hardy)
                new NPCLock(271, 00, 0, 127), // Lombre (M) (Hardy)
                new NPCLock(205, 12, 0, 127), // Forretress (M) (Serious)
        });

        public static readonly TeamLock XSpinarak = new TeamLock(
            167, // Spinarak
            new[] {
                new NPCLock(220, 12, 1, 127), // Swinub (F) (Serious)
                new NPCLock(353, 06, 0, 127), // Shuppet (M) (Docile)
        });

        public static readonly TeamLock XNumel = new TeamLock(
            322, // Numel
            new[] {
                new NPCLock(280, 06, 0, 127), // Ralts (M) (Docile)
                new NPCLock(100, 00, 2, 255), // Voltorb (-) (Hardy)
                new NPCLock(371, 24, 1, 127), // Bagon (F) (Quirky)
        });

        public static readonly TeamLock XShroomish = new TeamLock(
            285, // Shroomish
            new[] {
                new NPCLock(209, 24, 1, 191), // Snubbull (F) (Quirky)
                new NPCLock(352, 00, 1, 127), // Kecleon (F) (Hardy)
        });

        public static readonly TeamLock XDelcatty = new TeamLock(
            301, // Delcatty
            new[] {
                new NPCLock(370, 06, 1, 191), // Luvdisc (F) (Docile)
                new NPCLock(267, 00, 0, 127), // Beautifly (M) (Hardy)
                new NPCLock(315, 24, 0, 127), // Roselia (M) (Quirky)
        });

        public static readonly TeamLock XVoltorb = new TeamLock(
            100, // Voltorb
            new[] {
                new NPCLock(271, 00, 0, 127), // Lombre (M) (Hardy)
                new NPCLock(271, 18, 0, 127), // Lombre (M) (Bashful)
                new NPCLock(271, 12, 1, 127), // Lombre (F) (Serious)
        });

        public static readonly TeamLock XMakuhita = new TeamLock(
            296, // Makuhita
            new[] {
                new NPCLock(352, 06, 0, 127), // Kecleon (M) (Docile)
                new NPCLock(283, 18, 1, 127), // Surskit (F) (Bashful)
        });

        public static readonly TeamLock XVulpix = new TeamLock(
            037, // Vulpix
            new[] {
                new NPCLock(167, 00, 0, 127), // Spinarak (M) (Hardy)
                new NPCLock(267, 06, 1, 127), // Beautifly (F) (Docile)
                new NPCLock(269, 18, 0, 127), // Dustox (M) (Bashful)
        });

        public static readonly TeamLock XDuskull = new TeamLock(
            355, // Duskull
            new[] {
                new NPCLock(215, 12, 0, 127), // Sneasel (M) (Serious)
                new NPCLock(193, 18, 1, 127), // Yanma (F) (Bashful)
                new NPCLock(200, 24, 0, 127), // Misdreavus (M) (Quirky)
        });

        public static readonly TeamLock XMawile = new TeamLock(
            303, // Mawile
            new[] {
                new NPCLock(294, 06, 0, 127), // Loudred (M) (Docile)
                new NPCLock(203, 18, 1, 127), // Girafarig (F) (Bashful)
        });

        public static readonly TeamLock XSnorunt = new TeamLock(
            361, // Snorunt
            new[] {
                new NPCLock(336, 06, 1, 127), // Seviper (F) (Docile)
        });

        public static readonly TeamLock XPineco = new TeamLock(
            204, // Pineco
            new[] {
                new NPCLock(198, 06, 0, 127), // Murkrow (M) (Docile)
        });

        public static readonly TeamLock XNatu = new TeamLock(
            177, // Natu
            new[] {
                new NPCLock(281, 00, 0, 127), // Kirlia (M) (Hardy)
                new NPCLock(264, 00, 1, 127), // Linoone (F) (Hardy)
        });

        public static readonly TeamLock XRoselia = new TeamLock(
            315, // Roselia
            new[] {
                new NPCLock(223, 06, 0, 127), // Remoraid (M) (Docile)
                new NPCLock(042, 18, 0, 127), // Golbat (M) (Bashful)
        });

        public static readonly TeamLock XMeowth = new TeamLock(
            052, // Meowth
            new[] {
                new NPCLock(064, 06, 0, 063), // Kadabra (M) (Docile)
                new NPCLock(215, 00, 1, 127), // Sneasel (F) (Hardy)
                new NPCLock(200, 18, 1, 127), // Misdreavus (F) (Bashful)
        });

        public static readonly TeamLock XSwinub = new TeamLock(
            220, // Swinub
            new[] {
                new NPCLock(324, 18, 1, 127), // Torkoal (F) (Bashful)
                new NPCLock(274, 00, 0, 127), // Nuzleaf (M) (Hardy)
        });

        public static readonly TeamLock XSpearow = new TeamLock(
            021, // Spearow
            new[] {
                new NPCLock(279, 18, 0, 127), // Pelipper (M) (Bashful)
                new NPCLock(309, 06, 1, 127), // Electrike (F) (Docile)
        });

        public static readonly TeamLock XGrimer = new TeamLock(
            088, // Grimer
            new[] {
                new NPCLock(358, 12, 0, 127), // Chimecho (M) (Serious)
                new NPCLock(234, 18, 0, 127), // Stantler (M) (Bashful)
        });

        public static readonly TeamLock XSeel = new TeamLock(
            086, // Seel
            new[] {
                new NPCLock(163, 06, 0, 127), // Hoothoot (M) (Docile)
                new NPCLock(075, 18, 0, 127), // Graveler (M) (Bashful)
                new NPCLock(316, 18, 1, 127), // Gulpin (F) (Bashful)
        });

        public static readonly TeamLock XLunatone = new TeamLock(
            337, // Lunatone
            new[] {
                new NPCLock(171, 00, 1, 127), // Lanturn (F) (Hardy)
                new NPCLock(195, 18, 0, 127), // Quagsire (M) (Bashful)
        });

        public static readonly TeamLock XNosepass = new TeamLock(
            299, // Nosepass
            new[] {
                new NPCLock(271, 00, 0, 127), // Lombre (M) (Hardy)
                new NPCLock(271, 18, 0, 127), // Lombre (M) (Bashful)
                new NPCLock(271, 12, 1, 127), // Lombre (F) (Serious)
        });

        public static readonly TeamLock XParas = new TeamLock(
            046, // Paras
            new[] {
                new NPCLock(336, 24, 0, 127), // Seviper (M) (Quirky)
                new NPCLock(198, 06, 1, 127), // Murkrow (F) (Docile)
        });

        public static readonly TeamLock XGrowlithe = new TeamLock(
            058, // Growlithe
            new[] {
                new NPCLock(336, 24, 0, 127), // Seviper (M) (Quirky)
                new NPCLock(198, 06, 1, 127), // Murkrow (F) (Docile)
                new NPCLock(046), // Shadow Paras
        });

        public static readonly TeamLock XGrowlitheParasSeen = new TeamLock(
            058, // Growlithe
            "Paras Seen",
            new[] {
                new NPCLock(336, 24, 0, 127), // Seviper (M) (Quirky)
                new NPCLock(198, 06, 1, 127), // Murkrow (F) (Docile)
                new NPCLock(046, true), // Shadow Paras (Seen)
        });

        public static readonly TeamLock XPidgeotto = new TeamLock(
            017, // Pidgeotto
            new[] {
                new NPCLock(015), // Shadow Beedrill
                new NPCLock(162, 12, 0, 127), // Furret (M) (Serious)
                new NPCLock(176, 18, 0, 031), // Togetic (M) (Bashful)
        });

        public static readonly TeamLock XPidgeottoBeedrillSeen = new TeamLock(
            017, // Pidgeotto
            "Beedrill Seen",
            new[] {
                new NPCLock(015, true), // Shadow Beedrill (Seen)
                new NPCLock(162, 12, 0, 127), // Furret (M) (Serious)
                new NPCLock(176, 18, 0, 031), // Togetic (M) (Bashful)
        });

        public static readonly TeamLock XTangela = new TeamLock(
            114, // Tangela
            new[] {
                new NPCLock(038, 12, 1, 191), // Ninetales (F) (Serious)
                new NPCLock(189, 06, 0, 127), // Jumpluff (M) (Docile)
                new NPCLock(184, 00, 1, 127), // Azumarill (F) (Hardy)
        });

        public static readonly TeamLock XButterfree = new TeamLock(
            012, // Butterfree
            new[] {
                new NPCLock(038, 12, 1, 191), // Ninetales (F) (Serious)
                new NPCLock(189, 06, 0, 127), // Jumpluff (M) (Docile)
                new NPCLock(184, 00, 1, 127), // Azumarill (F) (Hardy)
                new NPCLock(114), // Shadow Tangela
        });

        public static readonly TeamLock XButterfreeTangelaSeen = new TeamLock(
            012, // Butterfree
            "Tangela Seen",
            new[] {
                new NPCLock(038, 12, 1, 191), // Ninetales (F) (Serious)
                new NPCLock(189, 06, 0, 127), // Jumpluff (M) (Docile)
                new NPCLock(184, 00, 1, 127), // Azumarill (F) (Hardy)
                new NPCLock(114, true), // Shadow Tangela (Seen)
        });

        public static readonly TeamLock XMagneton = new TeamLock(
            082, // Magneton
            new[] {
                new NPCLock(292, 18, 2, 255), // Shedinja (-) (Bashful)
                new NPCLock(202, 00, 0, 127), // Wobbuffet (M) (Hardy)
                new NPCLock(329, 12, 1, 127), // Vibrava (F) (Serious)
        });

        public static readonly TeamLock XVenomoth = new TeamLock(
            049, // Venomoth
            new[] {
                new NPCLock(055, 18, 1, 127), // Golduck (F) (Bashful)
                new NPCLock(237, 24, 0, 000), // Hitmontop (M) (Quirky)
                new NPCLock(297, 12, 0, 063), // Hariyama (M) (Serious)
        });

        public static readonly TeamLock XWeepinbell = new TeamLock(
            070, // Weepinbell
            new[] {
                new NPCLock(055, 18, 1, 127), // Golduck (F) (Bashful)
                new NPCLock(237, 24, 0, 000), // Hitmontop (M) (Quirky)
                new NPCLock(297, 12, 0, 063), // Hariyama (M) (Serious)
                new NPCLock(049), // Shadow Venomoth
        });

        public static readonly TeamLock XWeepinbellVenomothSeen = new TeamLock(
            070, // Weepinbell
            "Venomoth Seen",
            new[] {
                new NPCLock(055, 18, 1, 127), // Golduck (F) (Bashful)
                new NPCLock(237, 24, 0, 000), // Hitmontop (M) (Quirky)
                new NPCLock(297, 12, 0, 063), // Hariyama (M) (Serious)
                new NPCLock(049, true), // Shadow Venomoth (Seen)
        });

        public static readonly TeamLock XArbok = new TeamLock(
            024, // Arbok
            new[] {
                new NPCLock(367, 06, 0, 127), // Huntail (M) (Docile)
                new NPCLock(332, 00, 1, 127), // Cacturne (F) (Hardy)
                new NPCLock(110, 12, 1, 127), // Weezing (F) (Serious)
                new NPCLock(217, 18, 1, 127), // Ursaring (F) (Bashful)
        });

        public static readonly TeamLock XPrimeape = new TeamLock(
            057, // Primeape
            new[] {
                new NPCLock(305, 18, 1, 127), // Lairon (F) (Bashful)
                new NPCLock(364, 12, 1, 127), // Sealeo (F) (Serious)
                new NPCLock(199, 06, 1, 127), // Slowking (F) (Docile)
                new NPCLock(217, 24, 0, 127), // Ursaring (M) (Quirky)
        });

        public static readonly TeamLock XHypno = new TeamLock(
            097, // Hypno
            new[] {
                new NPCLock(305, 18, 1, 127), // Lairon (F) (Bashful)
                new NPCLock(364, 12, 1, 127), // Sealeo (F) (Serious)
                new NPCLock(199, 06, 1, 127), // Slowking (F) (Docile)
                new NPCLock(217, 24, 0, 127), // Ursaring (M) (Quirky)
                new NPCLock(057), // Shadow Primeape
        });

        public static readonly TeamLock XHypnoPrimeapeSeen = new TeamLock(
            097, // Hypno
            "Primeape Seen",
            new[] {
                new NPCLock(305, 18, 1, 127), // Lairon (F) (Bashful)
                new NPCLock(364, 12, 1, 127), // Sealeo (F) (Serious)
                new NPCLock(199, 06, 1, 127), // Slowking (F) (Docile)
                new NPCLock(217, 24, 0, 127), // Ursaring (M) (Quirky)
                new NPCLock(057, true), // Shadow Primeape (Seen)
        });

        public static readonly TeamLock XGolduck = new TeamLock(
            055, // Golduck
            new[] {
                new NPCLock(342, 24, 0, 127), // Crawdaunt (M) (Quirky)
                new NPCLock(279, 06, 1, 127), // Pelipper (F) (Docile)
                new NPCLock(226, 18, 1, 127), // Mantine (F) (Bashful)
        });

        public static readonly TeamLock XSableye = new TeamLock(
            302, // Sableye
            new[] {
                new NPCLock(342, 24, 0, 127), // Crawdaunt (M) (Quirky)
                new NPCLock(279, 06, 1, 127), // Pelipper (F) (Docile)
                new NPCLock(226, 18, 1, 127), // Mantine (F) (Bashful)
                new NPCLock(055), // Shadow Golduck
        });

        public static readonly TeamLock XSableyeGolduckSeen = new TeamLock(
            302, // Sableye
            "Golduck Seen",
            new[] {
                new NPCLock(342, 24, 0, 127), // Crawdaunt (M) (Quirky)
                new NPCLock(279, 06, 1, 127), // Pelipper (F) (Docile)
                new NPCLock(226, 18, 1, 127), // Mantine (F) (Bashful)
                new NPCLock(055, true), // Shadow Golduck (Seen)
        });

        public static readonly TeamLock XDodrio = new TeamLock(
            085, // Dodrio
            new[] {
                new NPCLock(178, 18, 1, 127), // Xatu (F) (Bashful)
        });

        public static readonly TeamLock XRaticate = new TeamLock(
            020, // Raticate
            new[] {
                new NPCLock(178, 18, 1, 127), // Xatu (F) (Bashful)
                new NPCLock(085), // Shadow Dodrio
                new NPCLock(340, 18, 0, 127), // Whiscash (M) (Bashful)
        });

        public static readonly TeamLock XRaticateDodrioSeen = new TeamLock(
            020, // Raticate
            "Dodrio Seen",
            new[] {
                new NPCLock(178, 18, 1, 127), // Xatu (F) (Bashful)
                new NPCLock(085, true), // Shadow Dodrio (Seen)
                new NPCLock(340, 18, 0, 127), // Whiscash (M) (Bashful)
        });

        public static readonly TeamLock XFarfetchd = new TeamLock(
            083, // Farfetch’d
            new[] {
                new NPCLock(282, 12, 0, 127), // Gardevoir (M) (Serious)
                new NPCLock(368, 00, 1, 127), // Gorebyss (F) (Hardy)
                new NPCLock(315, 24, 0, 127), // Roselia (M) (Quirky)
        });

        public static readonly TeamLock XAltaria = new TeamLock(
            334, // Altaria
            new[] {
                new NPCLock(282, 12, 0, 127), // Gardevoir (M) (Serious)
                new NPCLock(368, 00, 1, 127), // Gorebyss (F) (Hardy)
                new NPCLock(315, 24, 0, 127), // Roselia (M) (Quirky)
                new NPCLock(083), // Shadow Farfetch’d
        });

        public static readonly TeamLock XAltariaFarfetchdSeen = new TeamLock(
            334, // Altaria
            "Farfetch'd Seen",
            new[] {
                new NPCLock(282, 12, 0, 127), // Gardevoir (M) (Serious)
                new NPCLock(368, 00, 1, 127), // Gorebyss (F) (Hardy)
                new NPCLock(315, 24, 0, 127), // Roselia (M) (Quirky)
                new NPCLock(083, true), // Shadow Farfetch’d (Seen)
        });

        public static readonly TeamLock XKangaskhan = new TeamLock(
            115, // Kangaskhan
            new[] {
                new NPCLock(101, 00, 2, 255), // Electrode (-) (Hardy)
                new NPCLock(200, 18, 1, 127), // Misdreavus (F) (Bashful)
                new NPCLock(344, 12, 2, 255), // Claydol (-) (Serious)
        });

        public static readonly TeamLock XBanette = new TeamLock(
            354, // Banette
            new[] {
                new NPCLock(101, 00, 2, 255), // Electrode (-) (Hardy)
                new NPCLock(200, 18, 1, 127), // Misdreavus (F) (Bashful)
                new NPCLock(344, 12, 2, 255), // Claydol (-) (Serious)
                new NPCLock(115), // Shadow Kangaskhan
        });

        public static readonly TeamLock XBanetteKangaskhanSeen = new TeamLock(
            354, // Banette
            "Kangaskhan Seen",
            new[] {
                new NPCLock(101, 00, 2, 255), // Electrode (-) (Hardy)
                new NPCLock(200, 18, 1, 127), // Misdreavus (F) (Bashful)
                new NPCLock(344, 12, 2, 255), // Claydol (-) (Serious)
                new NPCLock(115, true), // Shadow Kangaskhan (Seen)
        });

        public static readonly TeamLock XMagmar = new TeamLock(
            126, // Magmar
            new[] {
                new NPCLock(229, 18, 0, 127), // Houndoom (M) (Bashful)
                new NPCLock(038, 18, 0, 191), // Ninetales (M) (Bashful)
                new NPCLock(045, 00, 1, 127), // Vileplume (F) (Hardy)
        });

        public static readonly TeamLock XPinsir = new TeamLock(
            127, // Pinsir
            new[] {
                new NPCLock(229, 18, 0, 127), // Houndoom (M) (Bashful)
                new NPCLock(038, 18, 0, 191), // Ninetales (M) (Bashful)
                new NPCLock(045, 00, 1, 127), // Vileplume (F) (Hardy)
                new NPCLock(126), // Shadow Magmar
        });

        public static readonly TeamLock XPinsirMagmarSeen = new TeamLock(
            127, // Pinsir
            "Magmar Seen",
            new[] {
                new NPCLock(229, 18, 0, 127), // Houndoom (M) (Bashful)
                new NPCLock(038, 18, 0, 191), // Ninetales (M) (Bashful)
                new NPCLock(045, 00, 1, 127), // Vileplume (F) (Hardy)
                new NPCLock(126, true), // Shadow Magmar (Seen)
        });

        public static readonly TeamLock XRapidash = new TeamLock(
            078, // Rapidash
            new[] {
                new NPCLock(323, 24, 0, 127), // Camerupt (M) (Quirky)
                new NPCLock(110, 06, 0, 127), // Weezing (M) (Docile)
                new NPCLock(089, 12, 1, 127), // Muk (F) (Serious)
        });

        public static readonly TeamLock XMagcargo = new TeamLock(
            219, // Magcargo
            new[] {
                new NPCLock(323, 24, 0, 127), // Camerupt (M) (Quirky)
                new NPCLock(110, 06, 0, 127), // Weezing (M) (Docile)
                new NPCLock(089, 12, 1, 127), // Muk (F) (Serious)
                new NPCLock(078), // Shadow Rapidash
        });

        public static readonly TeamLock XMagcargoRapidashSeen = new TeamLock(
            219, // Magcargo
            "Rapidash Seen",
            new[] {
                new NPCLock(323, 24, 0, 127), // Camerupt (M) (Quirky)
                new NPCLock(110, 06, 0, 127), // Weezing (M) (Docile)
                new NPCLock(089, 12, 1, 127), // Muk (F) (Serious)
                new NPCLock(078, true), // Shadow Rapidash (Seen)
        });

        public static readonly TeamLock XHitmonchan = new TeamLock(
            107, // Hitmonchan
            new[] {
                new NPCLock(308, 24, 0, 127), // Medicham (M) (Quirky)
                new NPCLock(076, 06, 1, 127), // Golem (F) (Docile)
                new NPCLock(178, 18, 1, 127), // Xatu (F) (Bashful)
        });

        public static readonly TeamLock XHitmonlee = new TeamLock(
            106, // Hitmonlee
            new[] {
                new NPCLock(326, 18, 0, 127), // Grumpig (M) (Bashful)
                new NPCLock(227, 12, 1, 127), // Skarmory (F) (Serious)
                new NPCLock(375, 06, 2, 255), // Metang (-) (Docile)
                new NPCLock(297, 24, 1, 063), // Hariyama (F) (Quirky)
        });

        public static readonly TeamLock XLickitung = new TeamLock(
            108, // Lickitung
            new[] {
                new NPCLock(171, 24, 0, 127), // Lanturn (M) (Quirky)
                new NPCLock(082, 06, 2, 255), // Magneton (-) (Docile)
        });

        public static readonly TeamLock XScyther = new TeamLock(
            123, // Scyther
            new[]
            {
                new NPCLock(234, 06, 1, 127), // Stantler (F) (Docile)
                new NPCLock(295, 24, 0, 127), // Exploud (M) (Quirky)
        });

        public static readonly TeamLock XChansey = new TeamLock(
            113, // Chansey
            new[] {
                new NPCLock(234, 06, 1, 127), // Stantler (F) (Docile)
                new NPCLock(295, 24, 0, 127), // Exploud (M) (Quirky)
                new NPCLock(123), // Shadow Scyther
        });

        public static readonly TeamLock XChanseyScytherSeen = new TeamLock(
            113, // Chansey
            "Scyther Seen",
            new[] {
                new NPCLock(234, 06, 1, 127), // Stantler (F) (Docile)
                new NPCLock(295, 24, 0, 127), // Exploud (M) (Quirky)
                new NPCLock(123, true), // Shadow Scyther (Seen)
        });

        public static readonly TeamLock XSolrock = new TeamLock(
            338, // Solrock
            new[] {
                new NPCLock(375, 24, 2, 255), // Metang (-) (Quirky)
                new NPCLock(195, 06, 0, 127), // Quagsire (M) (Docile)
                new NPCLock(212, 00, 1, 127), // Scizor (F) (Hardy)
        });

        public static readonly TeamLock XStarmie = new TeamLock(
            121, // Starmie
            new[] {
                new NPCLock(375, 24, 2, 255), // Metang (-) (Quirky)
                new NPCLock(195, 06, 0, 127), // Quagsire (M) (Docile)
                new NPCLock(212, 00, 1, 127), // Scizor (F) (Hardy)
                new NPCLock(338), // Shadow Solrock
                new NPCLock(351, 18, 0, 127), // Castform (M) (Bashful)
        });

        public static readonly TeamLock XStarmieSolrockSeen = new TeamLock(
            121, // Starmie
            "Solrock Seen",
            new[] {
                new NPCLock(375, 24, 2, 255), // Metang (-) (Quirky)
                new NPCLock(195, 06, 0, 127), // Quagsire (M) (Docile)
                new NPCLock(212, 00, 1, 127), // Scizor (F) (Hardy)
                new NPCLock(338, true), // Shadow Solrock (Seen)
                new NPCLock(351, 18, 0, 127), // Castform (M) (Bashful)
        });

        public static readonly TeamLock XElectabuzz = new TeamLock(
            125, // Electabuzz
            new[] {
                new NPCLock(277), // Shadow Swellow
                new NPCLock(065, 24, 0, 063), // Alakazam (M) (Quirky)
                new NPCLock(230, 6, 1, 127), // Kingdra (F) (Docile)
                new NPCLock(214, 18, 1, 127), // Heracross (F) (Bashful)
        });

        public static readonly TeamLock XElectabuzzSwellowSeen = new TeamLock(
            125, // Electabuzz
            "Swellow Seen",
            new[] {
                new NPCLock(277, true), // Shadow Swellow (Seen)
                new NPCLock(065, 24, 0, 063), // Alakazam (M) (Quirky)
                new NPCLock(230, 6, 1, 127), // Kingdra (F) (Docile)
                new NPCLock(214, 18, 1, 127), // Heracross (F) (Bashful)
        });

        public static readonly TeamLock XSnorlax = new TeamLock(
            143, // Snorlax
            new[] {
                new NPCLock(277), // Shadow Swellow
                new NPCLock(065, 24, 0, 063), // Alakazam (M) (Quirky)
                new NPCLock(230, 6, 1, 127), // Kingdra (F) (Docile)
                new NPCLock(214, 18, 1, 127), // Heracross (F) (Bashful)
                new NPCLock(125), // Shadow Electabuzz
        });

        public static readonly TeamLock XSnorlaxSwellowSeen = new TeamLock(
            143, // Snorlax
            "Swellow Seen",
            new[] {
                new NPCLock(277, true), // Shadow Swellow (Seen)
                new NPCLock(065, 24, 0, 063), // Alakazam (M) (Quirky)
                new NPCLock(230, 6, 1, 127), // Kingdra (F) (Docile)
                new NPCLock(214, 18, 1, 127), // Heracross (F) (Bashful)
                new NPCLock(125), // Shadow Electabuzz
        });

        public static readonly TeamLock XSnorlaxSwellowElectabuzzSeen = new TeamLock(
            143, // Snorlax
            "Swellow & Electabuzz Seen",
            new[] {
                new NPCLock(277, true), // Shadow Swellow (Seen)
                new NPCLock(065, 24, 0, 063), // Alakazam (M) (Quirky)
                new NPCLock(230, 6, 1, 127), // Kingdra (F) (Docile)
                new NPCLock(214, 18, 1, 127), // Heracross (F) (Bashful)
                new NPCLock(125, true), // Shadow Electabuzz
        });

        public static readonly TeamLock XPoliwrath = new TeamLock(
            062, // Poliwrath
            new[] {
                new NPCLock(199, 18, 0, 127), // Slowking (M) (Bashful)
                new NPCLock(217, 18, 0, 127), // Ursaring (M) (Bashful)
                new NPCLock(306, 24, 0, 127), // Aggron (M) (Quirky)
                new NPCLock(365, 06, 1, 127), // Walrein (F) (Docile)
        });

        public static readonly TeamLock XMrMime = new TeamLock(
            122, // Mr. Mime
            new[] {
                new NPCLock(199, 18, 0, 127), // Slowking (M) (Bashful)
                new NPCLock(217, 18, 0, 127), // Ursaring (M) (Bashful)
                new NPCLock(306, 24, 0, 127), // Aggron (M) (Quirky)
                new NPCLock(365, 06, 1, 127), // Walrein (F) (Docile)
                new NPCLock(062), // Shadow Poliwrath
        });

        public static readonly TeamLock XMrMimePoliwrathSeen = new TeamLock(
            122, // Mr. Mime
            "Poliwrath Seen",
            new[] {
                new NPCLock(199, 18, 0, 127), // Slowking (M) (Bashful)
                new NPCLock(217, 18, 0, 127), // Ursaring (M) (Bashful)
                new NPCLock(306, 24, 0, 127), // Aggron (M) (Quirky)
                new NPCLock(365, 06, 1, 127), // Walrein (F) (Docile)
                new NPCLock(062, true), // Shadow Poliwrath (Seen)
        });

        public static readonly TeamLock XDugtrio = new TeamLock(
            051, // Dugtrio
            new[] {
                new NPCLock(362, 00, 0, 127), // Glalie (M) (Hardy)
                new NPCLock(181, 18, 0, 127), // Ampharos (M) (Bashful)
                new NPCLock(286, 06, 1, 127), // Breloom (F) (Docile)
                new NPCLock(232, 12, 0, 127), // Donphan (M) (Serious)
        });

        public static readonly TeamLock XManectric = new TeamLock(
            310, // Manectric
            new[] {
                new NPCLock(291, 06, 1, 127), // Ninjask (F) (Docile)
        });

        public static readonly TeamLock XSalamence = new TeamLock(
            373, // Salamence
            new[] {
                new NPCLock(291, 06, 1, 127), // Ninjask (F) (Docile)
                new NPCLock(310), // Shadow Manectric
        });

        public static readonly TeamLock XMarowak = new TeamLock(
            105, // Marowak
            new[] {
                new NPCLock(291, 06, 1, 127), // Ninjask (F) (Docile)
                new NPCLock(310), // Shadow Manectric
                new NPCLock(373), // Shadow Salamence
                new NPCLock(330, 24, 0, 127), // Flygon (M) (Quirky)
        });

        public static readonly TeamLock XLapras = new TeamLock(
            131, // Lapras
            new[] {
                new NPCLock(291, 06, 1, 127), // Ninjask (F) (Docile)
                new NPCLock(310), // Shadow Manectric
                new NPCLock(373), // Shadow Salamence
                new NPCLock(330, 24, 0, 127), // Flygon (M) (Quirky)
                new NPCLock(105), // Shadow Marowak
        });

        public static readonly TeamLock XSalamenceManectricSeen = new TeamLock(
            373, // Salamence
            "Manectric Seen",
            new[] {
                new NPCLock(291, 06, 1, 127), // Ninjask (F) (Docile)
                new NPCLock(310, true), // Shadow Manectric (Seen)
        });

        public static readonly TeamLock XMarowakManectricSeen = new TeamLock(
            105, // Marowak
            "Manectric Seen",
            new[] {
                new NPCLock(291, 06, 1, 127), // Ninjask (F) (Docile)
                new NPCLock(310, true), // Shadow Manectric (Seen)
                new NPCLock(373), // Shadow Salamence
                new NPCLock(330, 24, 0, 127), // Flygon (M) (Quirky)
        });

        public static readonly TeamLock XMarowakManectricSalamenceSeen = new TeamLock(
            105, // Marowak
            "Manectric & Salamence Seen",
            new[] {
                new NPCLock(291, 06, 1, 127), // Ninjask (F) (Docile)
                new NPCLock(310, true), // Shadow Manectric (Seen)
                new NPCLock(373, true), // Shadow Salamence (Seen)
                new NPCLock(330, 24, 0, 127), // Flygon (M) (Quirky)
        });

        public static readonly TeamLock XLaprasManectricSeen = new TeamLock(
            131, // Lapras
            "Manectric Seen",
            new[] {
                new NPCLock(291, 06, 1, 127), // Ninjask (F) (Docile)
                new NPCLock(310, true), // Shadow Manectric (Seen)
                new NPCLock(373), // Shadow Salamence
                new NPCLock(330, 24, 0, 127), // Flygon (M) (Quirky)
                new NPCLock(105), // Shadow Marowak
        });

        public static readonly TeamLock XLaprasManectricSalamenceSeen = new TeamLock(
            131, // Lapras
            "Manectric & Salamence Seen",
            new[] {
                new NPCLock(291, 06, 1, 127), // Ninjask (F) (Docile)
                new NPCLock(310, true), // Shadow Manectric (Seen)
                new NPCLock(373, true), // Shadow Salamence (Seen)
                new NPCLock(330, 24, 0, 127), // Flygon (M) (Quirky)
                new NPCLock(105), // Shadow Marowak
        });

        public static readonly TeamLock XLaprasManectricMarowakSeen = new TeamLock(
            131, // Lapras
            "Manectric & Marowak Seen",
            new[] {
                new NPCLock(291, 06, 1, 127), // Ninjask (F) (Docile)
                new NPCLock(310, true), // Shadow Manectric (Seen)
                new NPCLock(373), // Shadow Salamence
                new NPCLock(330, 24, 0, 127), // Flygon (M) (Quirky)
                new NPCLock(105, true), // Shadow Marowak (Seen)
        });

        public static readonly TeamLock XLaprasManectricSalamenceMarowakSeen = new TeamLock(
            131, // Lapras
            "Manectric & Salamence & Marowak Seen",
            new[] {
                new NPCLock(291, 06, 1, 127), // Ninjask (F) (Docile)
                new NPCLock(310, true), // Shadow Manectric (Seen)
                new NPCLock(373, true), // Shadow Salamence (Seen)
                new NPCLock(330, 24, 0, 127), // Flygon (M) (Quirky)
                new NPCLock(105, true), // Shadow Marowak (Seen)
        });

        public static readonly TeamLock XMoltres = new TeamLock(
            146, // Moltres
            new[] {
                new NPCLock(112), // Shadow Rhydon
        });

        public static readonly TeamLock XExeggutor = new TeamLock(
            103, // Exeggutor
            new[] {
                new NPCLock(112), // Shadow Rhydon
                new NPCLock(146), // Shadow Moltres
        });

        public static readonly TeamLock XTauros = new TeamLock(
            128, // Tauros
            new[] {
                new NPCLock(112), // Shadow Rhydon
                new NPCLock(146), // Shadow Moltres
                new NPCLock(103), // Shadow Exeggutor
        });

        public static readonly TeamLock XArticuno = new TeamLock(
            144, // Articuno
            new[] {
                new NPCLock(112), // Shadow Rhydon
                new NPCLock(146), // Shadow Moltres
                new NPCLock(103), // Shadow Exeggutor
                new NPCLock(128), // Shadow Tauros
        });

        public static readonly TeamLock XZapdos = new TeamLock(
            145, // Zapdos
            new[] {
                new NPCLock(112), // Shadow Rhydon
                new NPCLock(146), // Shadow Moltres
                new NPCLock(103), // Shadow Exeggutor
                new NPCLock(128), // Shadow Tauros
                new NPCLock(144), // Shadow Articuno
        });

        public static readonly TeamLock XExeggutorRhydonMoltresSeen = new TeamLock(
            103, // Exeggutor
            "Rhydon & Moltres Seen",
            new[] {
                new NPCLock(112, true), // Shadow Rhydon (Seen)
                new NPCLock(146, true), // Shadow Moltres (Seen)
        });

        public static readonly TeamLock XTaurosRhydonMoltresSeen = new TeamLock(
            128, // Tauros
            "Rhydon & Moltres Seen",
            new[] {
                new NPCLock(112, true), // Shadow Rhydon (Seen)
                new NPCLock(146, true), // Shadow Moltres (Seen)
                new NPCLock(103), // Shadow Exeggutor
        });

        public static readonly TeamLock XTaurosRhydonMoltresExeggutorSeen = new TeamLock(
            128, // Tauros
            "Rhydon & Moltres & Exeggutor Seen",
            new[] {
                new NPCLock(112, true), // Shadow Rhydon (Seen)
                new NPCLock(146, true), // Shadow Moltres (Seen)
                new NPCLock(103, true), // Shadow Exeggutor (Seen)
        });

        public static readonly TeamLock XArticunoRhydonMoltresSeen = new TeamLock(
            144, // Articuno
            "Rhydon & Moltres Seen",
            new[] {
                new NPCLock(112, true), // Shadow Rhydon (Seen)
                new NPCLock(146, true), // Shadow Moltres (Seen)
                new NPCLock(103), // Shadow Exeggutor
                new NPCLock(128), // Shadow Tauros
        });

        public static readonly TeamLock XArticunoRhydonMoltresTaurosSeen = new TeamLock(
            144, // Articuno
            "Rhydon & Moltres & Tauros Seen",
            new[] {
                new NPCLock(112, true), // Shadow Rhydon (Seen)
                new NPCLock(146, true), // Shadow Moltres (Seen)
                new NPCLock(103), // Shadow Exeggutor
                new NPCLock(128, true), // Shadow Tauros (Seen)
        });

        public static readonly TeamLock XArticunoRhydonMoltresExeggutorSeen = new TeamLock(
            144, // Articuno
            "Rhydon & Moltres & Exeggutor Seen",
            new[] {
                new NPCLock(112, true), // Shadow Rhydon (Seen)
                new NPCLock(146, true), // Shadow Moltres (Seen)
                new NPCLock(103, true), // Shadow Exeggutor (Seen)
                new NPCLock(128), // Shadow Tauros
        });

        public static readonly TeamLock XArticunoRhydonMoltresExeggutorTaurosSeen = new TeamLock(
            144, // Articuno
            "Rhydon & Moltres & Exeggutor & Tauros Seen",
            new[] {
                new NPCLock(112, true), // Shadow Rhydon (Seen)
                new NPCLock(146, true), // Shadow Moltres (Seen)
                new NPCLock(103, true), // Shadow Exeggutor (Seen)
                new NPCLock(128, true), // Shadow Tauros (Seen)
        });

        public static readonly TeamLock XZapdosRhydonMoltresSeen = new TeamLock(
            145, // Zapdos
            "Rhydon & Moltres Seen",
            new[] {
                new NPCLock(112, true), // Shadow Rhydon (Seen)
                new NPCLock(146, true), // Shadow Moltres (Seen)
                new NPCLock(103), // Shadow Exeggutor
                new NPCLock(128), // Shadow Tauros
                new NPCLock(144), // Shadow Articuno
        });

        public static readonly TeamLock XZapdosRhydonMoltresTaurosSeen = new TeamLock(
            145, // Zapdos
            "Rhydon & Moltres & Tauros Seen",
            new[] {
                new NPCLock(112, true), // Shadow Rhydon (Seen)
                new NPCLock(146, true), // Shadow Moltres (Seen)
                new NPCLock(103), // Shadow Exeggutor
                new NPCLock(128, true), // Shadow Tauros (Seen)
                new NPCLock(144), // Shadow Articuno
        });

        public static readonly TeamLock XZapdosRhydonMoltresArticunoSeen = new TeamLock(
            145, // Zapdos
            "Rhydon & Moltres & Articuno Seen",
            new[] {
                new NPCLock(112, true), // Shadow Rhydon (Seen)
                new NPCLock(146, true), // Shadow Moltres (Seen)
                new NPCLock(103), // Shadow Exeggutor
                new NPCLock(128), // Shadow Tauros
                new NPCLock(144, true), // Shadow Articuno (Seen)
        });

        public static readonly TeamLock XZapdosRhydonMoltresExeggutorSeen = new TeamLock(
            145, // Zapdos
            "Rhydon & Moltres & Exeggutor Seen",
            new[] {
                new NPCLock(112, true), // Shadow Rhydon (Seen)
                new NPCLock(146, true), // Shadow Moltres (Seen)
                new NPCLock(103, true), // Shadow Exeggutor (Seen)
                new NPCLock(128), // Shadow Tauros
                new NPCLock(144), // Shadow Articuno
        });

        public static readonly TeamLock XZapdosRhydonMoltresTaurosArticunoSeen = new TeamLock(
            145, // Zapdos
            "Rhydon & Moltres & Tauros & Articuno Seen",
            new[] {
                new NPCLock(112, true), // Shadow Rhydon (Seen)
                new NPCLock(146, true), // Shadow Moltres (Seen)
                new NPCLock(103), // Shadow Exeggutor
                new NPCLock(128, true), // Shadow Tauros (Seen)
                new NPCLock(144, true), // Shadow Articuno (Seen)
        });

        public static readonly TeamLock XZapdosRhydonMoltresExeggutorTaurosSeen = new TeamLock(
            145, // Zapdos
            "Rhydon & Moltres & Exeggutor & Tauros Seen",
            new[] {
                new NPCLock(112, true), // Shadow Rhydon (Seen)
                new NPCLock(146, true), // Shadow Moltres (Seen)
                new NPCLock(103, true), // Shadow Exeggutor (Seen)
                new NPCLock(128, true), // Shadow Tauros (Seen)
                new NPCLock(144), // Shadow Articuno
        });

        public static readonly TeamLock XZapdosRhydonMoltresExeggutorArticunoSeen = new TeamLock(
            145, // Zapdos
            "Rhydon & Moltres & Exeggutor & Articuno Seen",
            new[] {
                new NPCLock(112, true), // Shadow Rhydon (Seen)
                new NPCLock(146, true), // Shadow Moltres (Seen)
                new NPCLock(103, true), // Shadow Exeggutor (Seen)
                new NPCLock(128), // Shadow Tauros
                new NPCLock(144, true), // Shadow Articuno (Seen)
        });

        public static readonly TeamLock XZapdosRhydonMoltresExeggutorTaurosArticunoSeen = new TeamLock(
            145, // Zapdos
            "Rhydon & Moltres & Exeggutor & Tauros & Articuno Seen",
            new[] {
                new NPCLock(112, true), // Shadow Rhydon (Seen)
                new NPCLock(146, true), // Shadow Moltres (Seen)
                new NPCLock(103, true), // Shadow Exeggutor (Seen)
                new NPCLock(128, true), // Shadow Tauros (Seen)
                new NPCLock(144, true), // Shadow Articuno (Seen)
        });

        public static readonly TeamLock XDragonite = new TeamLock(
            149, // Dragonite
            new[] {
                new NPCLock(272, 00, 0, 127), // Ludicolo (M) (Hardy)
                new NPCLock(272, 18, 0, 127), // Ludicolo (M) (Bashful)
                new NPCLock(272, 12, 1, 127), // Ludicolo (F) (Serious)
                new NPCLock(272, 12, 1, 127), // Ludicolo (F) (Serious)
                new NPCLock(272, 00, 0, 127), // Ludicolo (M) (Hardy)
        });

        #endregion
    }
}
