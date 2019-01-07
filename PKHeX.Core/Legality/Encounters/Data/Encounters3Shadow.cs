namespace PKHeX.Core
{
    public static class Encounters3Shadow
    {
        #region Colosseum

        public static readonly TeamLock CMakuhita = new TeamLock {
            Species = 296, // Makuhita
            Locks = new[] {
                new NPCLock(355, 24, 0, 127), // Duskull (M) (Quirky)
                new NPCLock(167, 00, 1, 127), // Spinarak (F) (Hardy)
        }};

        public static readonly TeamLock CGligar = new TeamLock {
            Species = 207, // Gligar
            Locks = new[] {
                new NPCLock(216, 12, 0, 127), // Teddiursa (M) (Serious)
                new NPCLock(039, 06, 1, 191), // Jigglypuff (F) (Docile)
                new NPCLock(285, 18, 0, 127), // Shroomish (M) (Bashful)
        }};

        public static readonly TeamLock CMurkrow = new TeamLock {
            Species = 198, // Murkrow
            Locks = new[] {
                new NPCLock(318, 06, 0, 127), // Carvanha (M) (Docile)
                new NPCLock(274, 12, 1, 127), // Nuzleaf (F) (Serious)
                new NPCLock(228, 18, 0, 127), // Houndour (M) (Bashful)
        }};

        public static readonly TeamLock CHeracross = new TeamLock {
            Species = 214, // Heracross
            Locks = new[] {
                new NPCLock(284, 00, 0, 127), // Masquerain (M) (Hardy)
                new NPCLock(168, 00, 1, 127), // Ariados (F) (Hardy)
        }};

        public static readonly TeamLock CUrsaring = new TeamLock {
            Species = 217, // Ursaring
            Locks = new[] {
                new NPCLock(067, 20, 1, 063), // Machoke (F) (Calm)
                new NPCLock(259, 16, 0, 031), // Marshtomp (M) (Mild)
                new NPCLock(275, 21, 1, 127), // Shiftry (F) (Gentle)
        }};

        #endregion

        #region E-Reader

        public static readonly TeamLock ETogepi = new TeamLock {
            Species = 175, // Togepi
            Locks = new[] {
                new NPCLock(302, 23, 0, 127), // Sableye (M) (Careful)
                new NPCLock(088, 08, 0, 127), // Grimer (M) (Impish)
                new NPCLock(316, 24, 0, 127), // Gulpin (M) (Quirky)
                new NPCLock(175, 22, 1, 031), // Togepi (F) (Sassy) -- itself!
        }};

        public static readonly TeamLock EMareep = new TeamLock {
            Species = 179, // Mareep
            Locks = new[] {
                new NPCLock(300, 04, 1, 191), // Skitty (F) (Naughty)
                new NPCLock(211, 10, 1, 127), // Qwilfish (F) (Timid)
                new NPCLock(355, 12, 1, 127), // Duskull (F) (Serious)
                new NPCLock(179, 16, 1, 127), // Mareep (F) (Mild) -- itself!
        }};

        public static readonly TeamLock EScizor = new TeamLock {
            Species = 212, // Scizor
            Locks = new[] {
                new NPCLock(198, 13, 1, 191), // Murkrow (F) (Jolly)
                new NPCLock(344, 02, 2, 255), // Claydol (-) (Brave)
                new NPCLock(208, 03, 0, 127), // Steelix (M) (Adamant)
                new NPCLock(212, 11, 0, 127), // Scizor (M) (Hasty) -- itself!
        }};

        #endregion

        #region XD

        public static readonly TeamLock XRalts = new TeamLock {
            Species = 280, // Ralts
            Locks = new[] {
                new NPCLock(064, 00, 0, 063), // Kadabra (M) (Hardy)
                new NPCLock(180, 06, 1, 127), // Flaaffy (F) (Docile)
                new NPCLock(288, 18, 0, 127), // Vigoroth (M) (Bashful)
        }};

        public static readonly TeamLock XPoochyena = new TeamLock {
            Species = 261, // Poochyena
            Locks = new[] {
                new NPCLock(041, 12, 1, 127), // Zubat (F) (Serious)
        }};

        public static readonly TeamLock XLedyba = new TeamLock {
            Species = 165, // Ledyba
            Locks = new[] {
                new NPCLock(276, 00, 1, 127), // Taillow (F) (Hardy)
        }};

        public static readonly TeamLock XSphealCipherLab = new TeamLock {
            Species = 363, // Spheal
            Comment = "Cipher Lab",
            Locks = new[] {
                new NPCLock(116, 24, 0, 063), // Horsea (M) (Quirky)
                new NPCLock(118, 12, 1, 127), // Goldeen (F) (Serious)
        }};

        public static readonly TeamLock XSphealPhenacCityandPost = new TeamLock {
            Species = 363, // Spheal
            Comment = "Phenac City and Post",
            Locks = new[] {
                new NPCLock(116, 24, 0, 063), // Horsea (M) (Quirky)
                new NPCLock(118, 12, 1, 127), // Goldeen (F) (Serious)
                new NPCLock(374, 00, 2, 255), // Beldum (-) (Hardy)
        }};

        public static readonly TeamLock XGulpin = new TeamLock {
            Species = 316, // Gulpin
            Locks = new[] {
                new NPCLock(109, 12, 1, 127), // Koffing (F) (Serious)
                new NPCLock(088, 06, 0, 127), // Grimer (M) (Docile)
        }};

        public static readonly TeamLock XSeedotCipherLab = new TeamLock {
            Species = 273, // Seedot
            Comment = "Cipher Lab",
            Locks = new[] {
                new NPCLock(043, 06, 0, 127), // Oddish (M) (Docile)
                new NPCLock(331, 24, 1, 127), // Cacnea (F) (Quirky)
                new NPCLock(285, 18, 1, 127), // Shroomish (F) (Bashful)
                new NPCLock(270, 00, 0, 127), // Lotad (M) (Hardy)
                new NPCLock(204, 12, 0, 127), // Pineco (M) (Serious)
        }};

        public static readonly TeamLock XSeedotPhenacCity = new TeamLock {
            Species = 273, // Seedot
            Comment = "Phenac City",
            Locks = new[] {
                new NPCLock(043, 06, 0, 127), // Oddish (M) (Docile)
                new NPCLock(331, 24, 1, 127), // Cacnea (F) (Quirky)
                new NPCLock(285, 00, 1, 127), // Shroomish (F) (Hardy)
                new NPCLock(270, 00, 1, 127), // Lotad (F) (Hardy)
                new NPCLock(204, 06, 0, 127), // Pineco (M) (Docile)
        }};

        public static readonly TeamLock XSeedotPost = new TeamLock {
            Species = 273, // Seedot
            Comment = "Post",
            Locks = new[] {
                new NPCLock(045, 06, 0, 127), // Vileplume (M) (Docile)
                new NPCLock(332, 24, 1, 127), // Cacturne (F) (Quirky)
                new NPCLock(286, 00, 1, 127), // Breloom (F) (Hardy)
                new NPCLock(271, 00, 0, 127), // Lombre (M) (Hardy)
                new NPCLock(205, 12, 0, 127), // Forretress (M) (Serious)
        }};

        public static readonly TeamLock XSpinarak = new TeamLock {
            Species = 167, // Spinarak
            Locks = new[] {
                new NPCLock(220, 12, 1, 127), // Swinub (F) (Serious)
                new NPCLock(353, 06, 0, 127), // Shuppet (M) (Docile)
        }};

        public static readonly TeamLock XNumel = new TeamLock {
            Species = 322, // Numel
            Locks = new[] {
                new NPCLock(280, 06, 0, 127), // Ralts (M) (Docile)
                new NPCLock(100, 00, 2, 255), // Voltorb (-) (Hardy)
                new NPCLock(371, 24, 1, 127), // Bagon (F) (Quirky)
        }};

        public static readonly TeamLock XShroomish = new TeamLock {
            Species = 285, // Shroomish
            Locks = new[] {
                new NPCLock(209, 24, 1, 191), // Snubbull (F) (Quirky)
                new NPCLock(352, 00, 1, 127), // Kecleon (F) (Hardy)
        }};

        public static readonly TeamLock XDelcatty = new TeamLock {
            Species = 301, // Delcatty
            Locks = new[] {
                new NPCLock(370, 06, 1, 191), // Luvdisc (F) (Docile)
                new NPCLock(267, 00, 0, 127), // Beautifly (M) (Hardy)
                new NPCLock(315, 24, 0, 127), // Roselia (M) (Quirky)
        }};

        public static readonly TeamLock XVoltorb = new TeamLock {
            Species = 100, // Voltorb
            Locks = new[] {
                new NPCLock(271, 00, 0, 127), // Lombre (M) (Hardy)
                new NPCLock(271, 18, 0, 127), // Lombre (M) (Bashful)
                new NPCLock(271, 12, 1, 127), // Lombre (F) (Serious)
        }};

        public static readonly TeamLock XMakuhita = new TeamLock {
            Species = 296, // Makuhita
            Locks = new[] {
                new NPCLock(352, 06, 0, 127), // Kecleon (M) (Docile)
                new NPCLock(283, 18, 1, 127), // Surskit (F) (Bashful)
        }};

        public static readonly TeamLock XVulpix = new TeamLock {
            Species = 037, // Vulpix
            Locks = new[] {
                new NPCLock(167, 00, 0, 127), // Spinarak (M) (Hardy)
                new NPCLock(267, 06, 1, 127), // Beautifly (F) (Docile)
                new NPCLock(269, 18, 0, 127), // Dustox (M) (Bashful)
        }};

        public static readonly TeamLock XDuskull = new TeamLock {
            Species = 355, // Duskull
            Locks = new[] {
                new NPCLock(215, 12, 0, 127), // Sneasel (M) (Serious)
                new NPCLock(193, 18, 1, 127), // Yanma (F) (Bashful)
                new NPCLock(200, 24, 0, 127), // Misdreavus (M) (Quirky)
        }};

        public static readonly TeamLock XMawile = new TeamLock {
            Species = 303, // Mawile
            Locks = new[] {
                new NPCLock(294, 06, 0, 127), // Loudred (M) (Docile)
                new NPCLock(203, 18, 1, 127), // Girafarig (F) (Bashful)
        }};

        public static readonly TeamLock XSnorunt = new TeamLock {
            Species = 361, // Snorunt
            Locks = new[] {
                new NPCLock(336, 06, 1, 127), // Seviper (F) (Docile)
        }};

        public static readonly TeamLock XPineco = new TeamLock {
            Species = 204, // Pineco
            Locks = new[] {
                new NPCLock(198, 06, 0, 127), // Murkrow (M) (Docile)
        }};

        public static readonly TeamLock XNatu = new TeamLock {
            Species = 177, // Natu
            Locks = new[] {
                new NPCLock(281, 00, 0, 127), // Kirlia (M) (Hardy)
                new NPCLock(264, 00, 1, 127), // Linoone (F) (Hardy)
        }};

        public static readonly TeamLock XRoselia = new TeamLock {
            Species = 315, // Roselia
            Locks = new[] {
                new NPCLock(223, 06, 0, 127), // Remoraid (M) (Docile)
                new NPCLock(042, 18, 0, 127), // Golbat (M) (Bashful)
        }};

        public static readonly TeamLock XMeowth = new TeamLock {
            Species = 052, // Meowth
            Locks = new[] {
                new NPCLock(064, 06, 0, 063), // Kadabra (M) (Docile)
                new NPCLock(215, 00, 1, 127), // Sneasel (F) (Hardy)
                new NPCLock(200, 18, 1, 127), // Misdreavus (F) (Bashful)
        }};

        public static readonly TeamLock XSwinub = new TeamLock {
            Species = 220, // Swinub
            Locks = new[] {
                new NPCLock(324, 18, 1, 127), // Torkoal (F) (Bashful)
                new NPCLock(274, 00, 0, 127), // Nuzleaf (M) (Hardy)
        }};

        public static readonly TeamLock XSpearow = new TeamLock {
            Species = 021, // Spearow
            Locks = new[] {
                new NPCLock(279, 18, 0, 127), // Pelipper (M) (Bashful)
                new NPCLock(309, 06, 1, 127), // Electrike (F) (Docile)
        }};

        public static readonly TeamLock XGrimer = new TeamLock {
            Species = 088, // Grimer
            Locks = new[] {
                new NPCLock(358, 12, 0, 127), // Chimecho (M) (Serious)
                new NPCLock(234, 18, 0, 127), // Stantler (M) (Bashful)
        }};

        public static readonly TeamLock XSeel = new TeamLock {
            Species = 086, // Seel
            Locks = new[] {
                new NPCLock(163, 06, 0, 127), // Hoothoot (M) (Docile)
                new NPCLock(075, 18, 0, 127), // Graveler (M) (Bashful)
                new NPCLock(316, 18, 1, 127), // Gulpin (F) (Bashful)
        }};

        public static readonly TeamLock XLunatone = new TeamLock {
            Species = 337, // Lunatone
            Locks = new[] {
                new NPCLock(171, 00, 1, 127), // Lanturn (F) (Hardy)
                new NPCLock(195, 18, 0, 127), // Quagsire (M) (Bashful)
        }};

        public static readonly TeamLock XNosepass = new TeamLock {
            Species = 299, // Nosepass
            Locks = new[] {
                new NPCLock(271, 00, 0, 127), // Lombre (M) (Hardy)
                new NPCLock(271, 18, 0, 127), // Lombre (M) (Bashful)
                new NPCLock(271, 12, 1, 127), // Lombre (F) (Serious)
        }};

        public static readonly TeamLock XParas = new TeamLock {
            Species = 046, // Paras
            Locks = new[] {
                new NPCLock(336, 24, 0, 127), // Seviper (M) (Quirky)
                new NPCLock(198, 06, 1, 127), // Murkrow (F) (Docile)
        }};

        public static readonly TeamLock XGrowlithe = new TeamLock {
            Species = 058, // Growlithe
            Locks = new[] {
                new NPCLock(336, 24, 0, 127), // Seviper (M) (Quirky)
                new NPCLock(198, 06, 1, 127), // Murkrow (F) (Docile)
                new NPCLock(046), // Shadow Paras
        }};

        public static readonly TeamLock XGrowlitheParasSeen = new TeamLock {
            Species = 058, // Growlithe
            Comment = "Paras Seen",
            Locks = new[] {
                new NPCLock(336, 24, 0, 127), // Seviper (M) (Quirky)
                new NPCLock(198, 06, 1, 127), // Murkrow (F) (Docile)
                new NPCLock(046, true), // Shadow Paras (Seen)
        }};

        public static readonly TeamLock XPidgeotto = new TeamLock {
            Species = 017, // Pidgeotto
            Locks = new[] {
                new NPCLock(015), // Shadow Beedrill
                new NPCLock(162, 12, 0, 127), // Furret (M) (Serious)
                new NPCLock(176, 18, 0, 031), // Togetic (M) (Bashful)
        }};

        public static readonly TeamLock XPidgeottoBeedrillSeen = new TeamLock {
            Species = 017, // Pidgeotto
            Comment = "Beedrill Seen",
            Locks = new[] {
                new NPCLock(015, true), // Shadow Beedrill (Seen)
                new NPCLock(162, 12, 0, 127), // Furret (M) (Serious)
                new NPCLock(176, 18, 0, 031), // Togetic (M) (Bashful)
        }};

        public static readonly TeamLock XTangela = new TeamLock {
            Species = 114, // Tangela
            Locks = new[] {
                new NPCLock(038, 12, 1, 191), // Ninetales (F) (Serious)
                new NPCLock(189, 06, 0, 127), // Jumpluff (M) (Docile)
                new NPCLock(184, 00, 1, 127), // Azumarill (F) (Hardy)
        }};

        public static readonly TeamLock XButterfree = new TeamLock {
            Species = 012, // Butterfree
            Locks = new[] {
                new NPCLock(038, 12, 1, 191), // Ninetales (F) (Serious)
                new NPCLock(189, 06, 0, 127), // Jumpluff (M) (Docile)
                new NPCLock(184, 00, 1, 127), // Azumarill (F) (Hardy)
                new NPCLock(114), // Shadow Tangela
        }};

        public static readonly TeamLock XButterfreeTangelaSeen = new TeamLock {
            Species = 012, // Butterfree
            Comment = "Tangela Seen",
            Locks = new[] {
                new NPCLock(038, 12, 1, 191), // Ninetales (F) (Serious)
                new NPCLock(189, 06, 0, 127), // Jumpluff (M) (Docile)
                new NPCLock(184, 00, 1, 127), // Azumarill (F) (Hardy)
                new NPCLock(114, true), // Shadow Tangela (Seen)
        }};

        public static readonly TeamLock XMagneton = new TeamLock {
            Species = 082, // Magneton
            Locks = new[] {
                new NPCLock(292, 18, 2, 255), // Shedinja (-) (Bashful)
                new NPCLock(202, 00, 0, 127), // Wobbuffet (M) (Hardy)
                new NPCLock(329, 12, 1, 127), // Vibrava (F) (Serious)
        }};

        public static readonly TeamLock XVenomoth = new TeamLock {
            Species = 049, // Venomoth
            Locks = new[] {
                new NPCLock(055, 18, 1, 127), // Golduck (F) (Bashful)
                new NPCLock(237, 24, 0, 000), // Hitmontop (M) (Quirky)
                new NPCLock(297, 12, 0, 063), // Hariyama (M) (Serious)
        }};

        public static readonly TeamLock XWeepinbell = new TeamLock {
            Species = 070, // Weepinbell
            Locks = new[] {
                new NPCLock(055, 18, 1, 127), // Golduck (F) (Bashful)
                new NPCLock(237, 24, 0, 000), // Hitmontop (M) (Quirky)
                new NPCLock(297, 12, 0, 063), // Hariyama (M) (Serious)
                new NPCLock(049), // Shadow Venomoth
        }};

        public static readonly TeamLock XWeepinbellVenomothSeen = new TeamLock {
            Species = 070, // Weepinbell
            Comment = "Venomoth Seen",
            Locks = new[] {
                new NPCLock(055, 18, 1, 127), // Golduck (F) (Bashful)
                new NPCLock(237, 24, 0, 000), // Hitmontop (M) (Quirky)
                new NPCLock(297, 12, 0, 063), // Hariyama (M) (Serious)
                new NPCLock(049, true), // Shadow Venomoth (Seen)
        }};

        public static readonly TeamLock XArbok = new TeamLock {
            Species = 024, // Arbok
            Locks = new[] {
                new NPCLock(367, 06, 0, 127), // Huntail (M) (Docile)
                new NPCLock(332, 00, 1, 127), // Cacturne (F) (Hardy)
                new NPCLock(110, 12, 1, 127), // Weezing (F) (Serious)
                new NPCLock(217, 18, 1, 127), // Ursaring (F) (Bashful)
        }};

        public static readonly TeamLock XPrimeape = new TeamLock {
            Species = 057, // Primeape
            Locks = new[] {
                new NPCLock(305, 18, 1, 127), // Lairon (F) (Bashful)
                new NPCLock(364, 12, 1, 127), // Sealeo (F) (Serious)
                new NPCLock(199, 06, 1, 127), // Slowking (F) (Docile)
                new NPCLock(217, 24, 0, 127), // Ursaring (M) (Quirky)
        }};

        public static readonly TeamLock XHypno = new TeamLock {
            Species = 097, // Hypno
            Locks = new[] {
                new NPCLock(305, 18, 1, 127), // Lairon (F) (Bashful)
                new NPCLock(364, 12, 1, 127), // Sealeo (F) (Serious)
                new NPCLock(199, 06, 1, 127), // Slowking (F) (Docile)
                new NPCLock(217, 24, 0, 127), // Ursaring (M) (Quirky)
                new NPCLock(057), // Shadow Primeape
        }};

        public static readonly TeamLock XHypnoPrimeapeSeen = new TeamLock {
            Species = 097, // Hypno
            Comment = "Primeape Seen",
            Locks = new[] {
                new NPCLock(305, 18, 1, 127), // Lairon (F) (Bashful)
                new NPCLock(364, 12, 1, 127), // Sealeo (F) (Serious)
                new NPCLock(199, 06, 1, 127), // Slowking (F) (Docile)
                new NPCLock(217, 24, 0, 127), // Ursaring (M) (Quirky)
                new NPCLock(057, true), // Shadow Primeape (Seen)
        }};

        public static readonly TeamLock XGolduck = new TeamLock {
            Species = 055, // Golduck
            Locks = new[] {
                new NPCLock(342, 24, 0, 127), // Crawdaunt (M) (Quirky)
                new NPCLock(279, 06, 1, 127), // Pelipper (F) (Docile)
                new NPCLock(226, 18, 1, 127), // Mantine (F) (Bashful)
        }};

        public static readonly TeamLock XSableye = new TeamLock {
            Species = 302, // Sableye
            Locks = new[] {
                new NPCLock(342, 24, 0, 127), // Crawdaunt (M) (Quirky)
                new NPCLock(279, 06, 1, 127), // Pelipper (F) (Docile)
                new NPCLock(226, 18, 1, 127), // Mantine (F) (Bashful)
                new NPCLock(055), // Shadow Golduck
        }};

        public static readonly TeamLock XSableyeGolduckSeen = new TeamLock {
            Species = 302, // Sableye
            Comment = "Golduck Seen",
            Locks = new[] {
                new NPCLock(342, 24, 0, 127), // Crawdaunt (M) (Quirky)
                new NPCLock(279, 06, 1, 127), // Pelipper (F) (Docile)
                new NPCLock(226, 18, 1, 127), // Mantine (F) (Bashful)
                new NPCLock(055, true), // Shadow Golduck (Seen)
        }};

        public static readonly TeamLock XDodrio = new TeamLock {
            Species = 085, // Dodrio
            Locks = new[] {
                new NPCLock(178, 18, 1, 127), // Xatu (F) (Bashful)
        }};

        public static readonly TeamLock XRaticate = new TeamLock {
            Species = 020, // Raticate
            Locks = new[] {
                new NPCLock(178, 18, 1, 127), // Xatu (F) (Bashful)
                new NPCLock(085), // Shadow Dodrio
                new NPCLock(340, 18, 0, 127), // Whiscash (M) (Bashful)
        }};

        public static readonly TeamLock XRaticateDodrioSeen = new TeamLock {
            Species = 020, // Raticate
            Comment = "Dodrio Seen",
            Locks = new[] {
                new NPCLock(178, 18, 1, 127), // Xatu (F) (Bashful)
                new NPCLock(085, true), // Shadow Dodrio (Seen)
                new NPCLock(340, 18, 0, 127), // Whiscash (M) (Bashful)
        }};

        public static readonly TeamLock XFarfetchd = new TeamLock {
            Species = 083, // Farfetch’d
            Locks = new[] {
                new NPCLock(282, 12, 0, 127), // Gardevoir (M) (Serious)
                new NPCLock(368, 00, 1, 127), // Gorebyss (F) (Hardy)
                new NPCLock(315, 24, 0, 127), // Roselia (M) (Quirky)
        }};

        public static readonly TeamLock XAltaria = new TeamLock {
            Species = 334, // Altaria
            Locks = new[] {
                new NPCLock(282, 12, 0, 127), // Gardevoir (M) (Serious)
                new NPCLock(368, 00, 1, 127), // Gorebyss (F) (Hardy)
                new NPCLock(315, 24, 0, 127), // Roselia (M) (Quirky)
                new NPCLock(083), // Shadow Farfetch’d
        }};

        public static readonly TeamLock XAltariaFarfetchdSeen = new TeamLock {
            Species = 334, // Altaria
            Comment = "Farfetch'd Seen",
            Locks = new[] {
                new NPCLock(282, 12, 0, 127), // Gardevoir (M) (Serious)
                new NPCLock(368, 00, 1, 127), // Gorebyss (F) (Hardy)
                new NPCLock(315, 24, 0, 127), // Roselia (M) (Quirky)
                new NPCLock(083, true), // Shadow Farfetch’d (Seen)
        }};

        public static readonly TeamLock XKangaskhan = new TeamLock {
            Species = 115, // Kangaskhan
            Locks = new[] {
                new NPCLock(101, 00, 2, 255), // Electrode (-) (Hardy)
                new NPCLock(200, 18, 1, 127), // Misdreavus (F) (Bashful)
                new NPCLock(344, 12, 2, 255), // Claydol (-) (Serious)
        }};

        public static readonly TeamLock XBanette = new TeamLock {
            Species = 354, // Banette
            Locks = new[] {
                new NPCLock(101, 00, 2, 255), // Electrode (-) (Hardy)
                new NPCLock(200, 18, 1, 127), // Misdreavus (F) (Bashful)
                new NPCLock(344, 12, 2, 255), // Claydol (-) (Serious)
                new NPCLock(115), // Shadow Kangaskhan
        }};

        public static readonly TeamLock XBanetteKangaskhanSeen = new TeamLock {
            Species = 354, // Banette
            Comment = "Kangaskhan Seen",
            Locks = new[] {
                new NPCLock(101, 00, 2, 255), // Electrode (-) (Hardy)
                new NPCLock(200, 18, 1, 127), // Misdreavus (F) (Bashful)
                new NPCLock(344, 12, 2, 255), // Claydol (-) (Serious)
                new NPCLock(115, true), // Shadow Kangaskhan (Seen)
        }};

        public static readonly TeamLock XMagmar = new TeamLock {
            Species = 126, // Magmar
            Locks = new[] {
                new NPCLock(229, 18, 0, 127), // Houndoom (M) (Bashful)
                new NPCLock(038, 18, 0, 191), // Ninetales (M) (Bashful)
                new NPCLock(045, 00, 1, 127), // Vileplume (F) (Hardy)
        }};

        public static readonly TeamLock XPinsir = new TeamLock {
            Species = 127, // Pinsir
            Locks = new[] {
                new NPCLock(229, 18, 0, 127), // Houndoom (M) (Bashful)
                new NPCLock(038, 18, 0, 191), // Ninetales (M) (Bashful)
                new NPCLock(045, 00, 1, 127), // Vileplume (F) (Hardy)
                new NPCLock(126), // Shadow Magmar
        }};

        public static readonly TeamLock XPinsirMagmarSeen = new TeamLock {
            Species = 127, // Pinsir
            Comment = "Magmar Seen",
            Locks = new[] {
                new NPCLock(229, 18, 0, 127), // Houndoom (M) (Bashful)
                new NPCLock(038, 18, 0, 191), // Ninetales (M) (Bashful)
                new NPCLock(045, 00, 1, 127), // Vileplume (F) (Hardy)
                new NPCLock(126, true), // Shadow Magmar (Seen)
        }};

        public static readonly TeamLock XRapidash = new TeamLock {
            Species = 078, // Rapidash
            Locks = new[] {
                new NPCLock(323, 24, 0, 127), // Camerupt (M) (Quirky)
                new NPCLock(110, 06, 0, 127), // Weezing (M) (Docile)
                new NPCLock(089, 12, 1, 127), // Muk (F) (Serious)
        }};

        public static readonly TeamLock XMagcargo = new TeamLock {
            Species = 219, // Magcargo
            Locks = new[] {
                new NPCLock(323, 24, 0, 127), // Camerupt (M) (Quirky)
                new NPCLock(110, 06, 0, 127), // Weezing (M) (Docile)
                new NPCLock(089, 12, 1, 127), // Muk (F) (Serious)
                new NPCLock(078), // Shadow Rapidash
        }};

        public static readonly TeamLock XMagcargoRapidashSeen = new TeamLock {
            Species = 219, // Magcargo
            Comment = "Rapidash Seen",
            Locks = new[] {
                new NPCLock(323, 24, 0, 127), // Camerupt (M) (Quirky)
                new NPCLock(110, 06, 0, 127), // Weezing (M) (Docile)
                new NPCLock(089, 12, 1, 127), // Muk (F) (Serious)
                new NPCLock(078, true), // Shadow Rapidash (Seen)
        }};

        public static readonly TeamLock XHitmonchan = new TeamLock {
            Species = 107, // Hitmonchan
            Locks = new[] {
                new NPCLock(308, 24, 0, 127), // Medicham (M) (Quirky)
                new NPCLock(076, 06, 1, 127), // Golem (F) (Docile)
                new NPCLock(178, 18, 1, 127), // Xatu (F) (Bashful)
        }};

        public static readonly TeamLock XHitmonlee = new TeamLock {
            Species = 106, // Hitmonlee
            Locks = new[] {
                new NPCLock(326, 18, 0, 127), // Grumpig (M) (Bashful)
                new NPCLock(227, 12, 1, 127), // Skarmory (F) (Serious)
                new NPCLock(375, 06, 2, 255), // Metang (-) (Docile)
                new NPCLock(297, 24, 1, 063), // Hariyama (F) (Quirky)
        }};

        public static readonly TeamLock XLickitung = new TeamLock {
            Species = 108, // Lickitung
            Locks = new[] {
                new NPCLock(171, 24, 0, 127), // Lanturn (M) (Quirky)
                new NPCLock(082, 06, 2, 255), // Magneton (-) (Docile)
        }};

        public static readonly TeamLock XScyther = new TeamLock {
            Species = 123, // Scyther
            Locks = new[]
            {
                new NPCLock(234, 06, 1, 127), // Stantler (F) (Docile)
                new NPCLock(295, 24, 0, 127), // Exploud (M) (Quirky)
        }};

        public static readonly TeamLock XChansey = new TeamLock {
            Species = 113, // Chansey
            Locks = new[] {
                new NPCLock(234, 06, 1, 127), // Stantler (F) (Docile)
                new NPCLock(295, 24, 0, 127), // Exploud (M) (Quirky)
                new NPCLock(123), // Shadow Scyther
        }};

        public static readonly TeamLock XChanseyScytherSeen = new TeamLock {
            Species = 113, // Chansey
            Comment = "Scyther Seen",
            Locks = new[] {
                new NPCLock(234, 06, 1, 127), // Stantler (F) (Docile)
                new NPCLock(295, 24, 0, 127), // Exploud (M) (Quirky)
                new NPCLock(123, true), // Shadow Scyther (Seen)
        }};

        public static readonly TeamLock XSolrock = new TeamLock {
            Species = 338, // Solrock
            Locks = new[] {
                new NPCLock(375, 24, 2, 255), // Metang (-) (Quirky)
                new NPCLock(195, 06, 0, 127), // Quagsire (M) (Docile)
                new NPCLock(212, 00, 1, 127), // Scizor (F) (Hardy)
        }};

        public static readonly TeamLock XStarmie = new TeamLock {
            Species = 121, // Starmie
            Locks = new[] {
                new NPCLock(375, 24, 2, 255), // Metang (-) (Quirky)
                new NPCLock(195, 06, 0, 127), // Quagsire (M) (Docile)
                new NPCLock(212, 00, 1, 127), // Scizor (F) (Hardy)
                new NPCLock(338), // Shadow Solrock
                new NPCLock(351, 18, 0, 127), // Castform (M) (Bashful)
        }};

        public static readonly TeamLock XStarmieSolrockSeen = new TeamLock {
            Species = 121, // Starmie
            Comment = "Solrock Seen",
            Locks = new[] {
                new NPCLock(375, 24, 2, 255), // Metang (-) (Quirky)
                new NPCLock(195, 06, 0, 127), // Quagsire (M) (Docile)
                new NPCLock(212, 00, 1, 127), // Scizor (F) (Hardy)
                new NPCLock(338, true), // Shadow Solrock (Seen)
                new NPCLock(351, 18, 0, 127), // Castform (M) (Bashful)
        }};

        public static readonly TeamLock XElectabuzz = new TeamLock {
            Species = 125, // Electabuzz
            Locks = new[] {
                new NPCLock(277), // Shadow Swellow
                new NPCLock(065, 24, 0, 063), // Alakazam (M) (Quirky)
                new NPCLock(230, 6, 1, 127), // Kingdra (F) (Docile)
                new NPCLock(214, 18, 1, 127), // Heracross (F) (Bashful)
        }};

        public static readonly TeamLock XElectabuzzSwellowSeen = new TeamLock {
            Species = 125, // Electabuzz
            Comment = "Swellow Seen",
            Locks = new[] {
                new NPCLock(277, true), // Shadow Swellow (Seen)
                new NPCLock(065, 24, 0, 063), // Alakazam (M) (Quirky)
                new NPCLock(230, 6, 1, 127), // Kingdra (F) (Docile)
                new NPCLock(214, 18, 1, 127), // Heracross (F) (Bashful)
        }};

        public static readonly TeamLock XSnorlax = new TeamLock {
            Species = 143, // Snorlax
            Locks = new[] {
                new NPCLock(277), // Shadow Swellow
                new NPCLock(065, 24, 0, 063), // Alakazam (M) (Quirky)
                new NPCLock(230, 6, 1, 127), // Kingdra (F) (Docile)
                new NPCLock(214, 18, 1, 127), // Heracross (F) (Bashful)
                new NPCLock(125), // Shadow Electabuzz
        }};

        public static readonly TeamLock XSnorlaxSwellowSeen = new TeamLock {
            Species = 143, // Snorlax
            Comment = "Swellow Seen",
            Locks = new[] {
                new NPCLock(277, true), // Shadow Swellow (Seen)
                new NPCLock(065, 24, 0, 063), // Alakazam (M) (Quirky)
                new NPCLock(230, 6, 1, 127), // Kingdra (F) (Docile)
                new NPCLock(214, 18, 1, 127), // Heracross (F) (Bashful)
                new NPCLock(125), // Shadow Electabuzz
        }};

        public static readonly TeamLock XSnorlaxSwellowElectabuzzSeen = new TeamLock {
            Species = 143, // Snorlax
            Comment = "Swellow & Electabuzz Seen",
            Locks = new[] {
                new NPCLock(277, true), // Shadow Swellow (Seen)
                new NPCLock(065, 24, 0, 063), // Alakazam (M) (Quirky)
                new NPCLock(230, 6, 1, 127), // Kingdra (F) (Docile)
                new NPCLock(214, 18, 1, 127), // Heracross (F) (Bashful)
                new NPCLock(125, true), // Shadow Electabuzz
        }};

        public static readonly TeamLock XPoliwrath = new TeamLock {
            Species = 062, // Poliwrath
            Locks = new[] {
                new NPCLock(199, 18, 0, 127), // Slowking (M) (Bashful)
                new NPCLock(217, 18, 0, 127), // Ursaring (M) (Bashful)
                new NPCLock(306, 24, 0, 127), // Aggron (M) (Quirky)
                new NPCLock(365, 06, 1, 127), // Walrein (F) (Docile)
        }};

        public static readonly TeamLock XMrMime = new TeamLock {
            Species = 122, // Mr. Mime
            Locks = new[] {
                new NPCLock(199, 18, 0, 127), // Slowking (M) (Bashful)
                new NPCLock(217, 18, 0, 127), // Ursaring (M) (Bashful)
                new NPCLock(306, 24, 0, 127), // Aggron (M) (Quirky)
                new NPCLock(365, 06, 1, 127), // Walrein (F) (Docile)
                new NPCLock(062), // Shadow Poliwrath
        }};

        public static readonly TeamLock XMrMimePoliwrathSeen = new TeamLock {
            Species = 122, // Mr. Mime
            Comment = "Poliwrath Seen",
            Locks = new[] {
                new NPCLock(199, 18, 0, 127), // Slowking (M) (Bashful)
                new NPCLock(217, 18, 0, 127), // Ursaring (M) (Bashful)
                new NPCLock(306, 24, 0, 127), // Aggron (M) (Quirky)
                new NPCLock(365, 06, 1, 127), // Walrein (F) (Docile)
                new NPCLock(062, true), // Shadow Poliwrath (Seen)
        }};

        public static readonly TeamLock XDugtrio = new TeamLock {
            Species = 051, // Dugtrio
            Locks = new[] {
                new NPCLock(362, 00, 0, 127), // Glalie (M) (Hardy)
                new NPCLock(181, 18, 0, 127), // Ampharos (M) (Bashful)
                new NPCLock(286, 06, 1, 127), // Breloom (F) (Docile)
                new NPCLock(232, 12, 0, 127), // Donphan (M) (Serious)
        }};

        public static readonly TeamLock XManectric = new TeamLock {
            Species = 310, // Manectric
            Locks = new[] {
                new NPCLock(291, 06, 1, 127), // Ninjask (F) (Docile)
        }};

        public static readonly TeamLock XSalamence = new TeamLock {
            Species = 373, // Salamence
            Locks = new[] {
                new NPCLock(291, 06, 1, 127), // Ninjask (F) (Docile)
                new NPCLock(310), // Shadow Manectric
        }};

        public static readonly TeamLock XMarowak = new TeamLock {
            Species = 105, // Marowak
            Locks = new[] {
                new NPCLock(291, 06, 1, 127), // Ninjask (F) (Docile)
                new NPCLock(310), // Shadow Manectric
                new NPCLock(373), // Shadow Salamence
                new NPCLock(330, 24, 0, 127), // Flygon (M) (Quirky)
        }};

        public static readonly TeamLock XLapras = new TeamLock {
            Species = 131, // Lapras
            Locks = new[] {
                new NPCLock(291, 06, 1, 127), // Ninjask (F) (Docile)
                new NPCLock(310), // Shadow Manectric
                new NPCLock(373), // Shadow Salamence
                new NPCLock(330, 24, 0, 127), // Flygon (M) (Quirky)
                new NPCLock(105), // Shadow Marowak
        }};

        public static readonly TeamLock XSalamenceManectricSeen = new TeamLock {
            Species = 373, // Salamence
            Comment = "Manectric Seen",
            Locks = new[] {
                new NPCLock(291, 06, 1, 127), // Ninjask (F) (Docile)
                new NPCLock(310, true), // Shadow Manectric (Seen)
        }};

        public static readonly TeamLock XMarowakManectricSeen = new TeamLock {
            Species = 105, // Marowak
            Comment = "Manectric Seen",
            Locks = new[] {
                new NPCLock(291, 06, 1, 127), // Ninjask (F) (Docile)
                new NPCLock(310, true), // Shadow Manectric (Seen)
                new NPCLock(373), // Shadow Salamence
                new NPCLock(330, 24, 0, 127), // Flygon (M) (Quirky)
        }};

        public static readonly TeamLock XMarowakManectricSalamenceSeen = new TeamLock {
            Species = 105, // Marowak
            Comment = "Manectric & Salamence Seen",
            Locks = new[] {
                new NPCLock(291, 06, 1, 127), // Ninjask (F) (Docile)
                new NPCLock(310, true), // Shadow Manectric (Seen)
                new NPCLock(373, true), // Shadow Salamence (Seen)
                new NPCLock(330, 24, 0, 127), // Flygon (M) (Quirky)
        }};

        public static readonly TeamLock XLaprasManectricSeen = new TeamLock {
            Species = 131, // Lapras
            Comment = "Manectric Seen",
            Locks = new[] {
                new NPCLock(291, 06, 1, 127), // Ninjask (F) (Docile)
                new NPCLock(310, true), // Shadow Manectric (Seen)
                new NPCLock(373), // Shadow Salamence
                new NPCLock(330, 24, 0, 127), // Flygon (M) (Quirky)
                new NPCLock(105), // Shadow Marowak
        }};

        public static readonly TeamLock XLaprasManectricSalamenceSeen = new TeamLock {
            Species = 131, // Lapras
            Comment = "Manectric & Salamence Seen",
            Locks = new[] {
                new NPCLock(291, 06, 1, 127), // Ninjask (F) (Docile)
                new NPCLock(310, true), // Shadow Manectric (Seen)
                new NPCLock(373, true), // Shadow Salamence (Seen)
                new NPCLock(330, 24, 0, 127), // Flygon (M) (Quirky)
                new NPCLock(105), // Shadow Marowak
        }};

        public static readonly TeamLock XLaprasManectricMarowakSeen = new TeamLock {
            Species = 131, // Lapras
            Comment = "Manectric & Marowak Seen",
            Locks = new[] {
                new NPCLock(291, 06, 1, 127), // Ninjask (F) (Docile)
                new NPCLock(310, true), // Shadow Manectric (Seen)
                new NPCLock(373), // Shadow Salamence
                new NPCLock(330, 24, 0, 127), // Flygon (M) (Quirky)
                new NPCLock(105, true), // Shadow Marowak (Seen)
        }};

        public static readonly TeamLock XLaprasManectricSalamenceMarowakSeen = new TeamLock {
            Species = 131, // Lapras
            Comment = "Manectric & Salamence & Marowak Seen",
            Locks = new[] {
                new NPCLock(291, 06, 1, 127), // Ninjask (F) (Docile)
                new NPCLock(310, true), // Shadow Manectric (Seen)
                new NPCLock(373, true), // Shadow Salamence (Seen)
                new NPCLock(330, 24, 0, 127), // Flygon (M) (Quirky)
                new NPCLock(105, true), // Shadow Marowak (Seen)
        }};

        public static readonly TeamLock XMoltres = new TeamLock {
            Species = 146, // Moltres
            Locks = new[] {
                new NPCLock(112), // Shadow Rhydon
        }};

        public static readonly TeamLock XExeggutor = new TeamLock {
            Species = 103, // Exeggutor
            Locks = new[] {
                new NPCLock(112), // Shadow Rhydon
                new NPCLock(146), // Shadow Moltres
        }};

        public static readonly TeamLock XTauros = new TeamLock {
            Species = 128, // Tauros
            Locks = new[] {
                new NPCLock(112), // Shadow Rhydon
                new NPCLock(146), // Shadow Moltres
                new NPCLock(103), // Shadow Exeggutor
        }};

        public static readonly TeamLock XArticuno = new TeamLock {
            Species = 144, // Articuno
            Locks = new[] {
                new NPCLock(112), // Shadow Rhydon
                new NPCLock(146), // Shadow Moltres
                new NPCLock(103), // Shadow Exeggutor
                new NPCLock(128), // Shadow Tauros
        }};

        public static readonly TeamLock XZapdos = new TeamLock {
            Species = 145, // Zapdos
            Locks = new[] {
                new NPCLock(112), // Shadow Rhydon
                new NPCLock(146), // Shadow Moltres
                new NPCLock(103), // Shadow Exeggutor
                new NPCLock(128), // Shadow Tauros
                new NPCLock(144), // Shadow Articuno
        }};

        public static readonly TeamLock XExeggutorRhydonMoltresSeen = new TeamLock {
            Species = 103, // Exeggutor
            Comment = "Rhydon & Moltres Seen",
            Locks = new[] {
                new NPCLock(112, true), // Shadow Rhydon (Seen)
                new NPCLock(146, true), // Shadow Moltres (Seen)
        }};

        public static readonly TeamLock XTaurosRhydonMoltresSeen = new TeamLock {
            Species = 128, // Tauros
            Comment = "Rhydon & Moltres Seen",
            Locks = new[] {
                new NPCLock(112, true), // Shadow Rhydon (Seen)
                new NPCLock(146, true), // Shadow Moltres (Seen)
                new NPCLock(103), // Shadow Exeggutor
        }};

        public static readonly TeamLock XTaurosRhydonMoltresExeggutorSeen = new TeamLock {
            Species = 128, // Tauros
            Comment = "Rhydon & Moltres & Exeggutor Seen",
            Locks = new[] {
                new NPCLock(112, true), // Shadow Rhydon (Seen)
                new NPCLock(146, true), // Shadow Moltres (Seen)
                new NPCLock(103, true), // Shadow Exeggutor (Seen)
        }};

        public static readonly TeamLock XArticunoRhydonMoltresSeen = new TeamLock {
            Species = 144, // Articuno
            Comment = "Rhydon & Moltres Seen",
            Locks = new[] {
                new NPCLock(112, true), // Shadow Rhydon (Seen)
                new NPCLock(146, true), // Shadow Moltres (Seen)
                new NPCLock(103), // Shadow Exeggutor
                new NPCLock(128), // Shadow Tauros
        }};

        public static readonly TeamLock XArticunoRhydonMoltresTaurosSeen = new TeamLock {
            Species = 144, // Articuno
            Comment = "Rhydon & Moltres & Tauros Seen",
            Locks = new[] {
                new NPCLock(112, true), // Shadow Rhydon (Seen)
                new NPCLock(146, true), // Shadow Moltres (Seen)
                new NPCLock(103), // Shadow Exeggutor
                new NPCLock(128, true), // Shadow Tauros (Seen)
        }};

        public static readonly TeamLock XArticunoRhydonMoltresExeggutorSeen = new TeamLock {
            Species = 144, // Articuno
            Comment = "Rhydon & Moltres & Exeggutor Seen",
            Locks = new[] {
                new NPCLock(112, true), // Shadow Rhydon (Seen)
                new NPCLock(146, true), // Shadow Moltres (Seen)
                new NPCLock(103, true), // Shadow Exeggutor (Seen)
                new NPCLock(128), // Shadow Tauros
        }};

        public static readonly TeamLock XArticunoRhydonMoltresExeggutorTaurosSeen = new TeamLock {
            Species = 144, // Articuno
            Comment = "Rhydon & Moltres & Exeggutor & Tauros Seen",
            Locks = new[] {
                new NPCLock(112, true), // Shadow Rhydon (Seen)
                new NPCLock(146, true), // Shadow Moltres (Seen)
                new NPCLock(103, true), // Shadow Exeggutor (Seen)
                new NPCLock(128, true), // Shadow Tauros (Seen)
        }};

        public static readonly TeamLock XZapdosRhydonMoltresSeen = new TeamLock {
            Species = 145, // Zapdos
            Comment = "Rhydon & Moltres Seen",
            Locks = new[] {
                new NPCLock(112, true), // Shadow Rhydon (Seen)
                new NPCLock(146, true), // Shadow Moltres (Seen)
                new NPCLock(103), // Shadow Exeggutor
                new NPCLock(128), // Shadow Tauros
                new NPCLock(144), // Shadow Articuno
        }};

        public static readonly TeamLock XZapdosRhydonMoltresTaurosSeen = new TeamLock {
            Species = 145, // Zapdos
            Comment = "Rhydon & Moltres & Tauros Seen",
            Locks = new[] {
                new NPCLock(112, true), // Shadow Rhydon (Seen)
                new NPCLock(146, true), // Shadow Moltres (Seen)
                new NPCLock(103), // Shadow Exeggutor
                new NPCLock(128, true), // Shadow Tauros (Seen)
                new NPCLock(144), // Shadow Articuno
        }};

        public static readonly TeamLock XZapdosRhydonMoltresArticunoSeen = new TeamLock {
            Species = 145, // Zapdos
            Comment = "Rhydon & Moltres & Articuno Seen",
            Locks = new[] {
                new NPCLock(112, true), // Shadow Rhydon (Seen)
                new NPCLock(146, true), // Shadow Moltres (Seen)
                new NPCLock(103), // Shadow Exeggutor
                new NPCLock(128), // Shadow Tauros
                new NPCLock(144, true), // Shadow Articuno (Seen)
        }};

        public static readonly TeamLock XZapdosRhydonMoltresExeggutorSeen = new TeamLock {
            Species = 145, // Zapdos
            Comment = "Rhydon & Moltres & Exeggutor Seen",
            Locks = new[] {
                new NPCLock(112, true), // Shadow Rhydon (Seen)
                new NPCLock(146, true), // Shadow Moltres (Seen)
                new NPCLock(103, true), // Shadow Exeggutor (Seen)
                new NPCLock(128), // Shadow Tauros
                new NPCLock(144), // Shadow Articuno
        }};

        public static readonly TeamLock XZapdosRhydonMoltresTaurosArticunoSeen = new TeamLock {
            Species = 145, // Zapdos
            Comment = "Rhydon & Moltres & Tauros & Articuno Seen",
            Locks = new[] {
                new NPCLock(112, true), // Shadow Rhydon (Seen)
                new NPCLock(146, true), // Shadow Moltres (Seen)
                new NPCLock(103), // Shadow Exeggutor
                new NPCLock(128, true), // Shadow Tauros (Seen)
                new NPCLock(144, true), // Shadow Articuno (Seen)
        }};

        public static readonly TeamLock XZapdosRhydonMoltresExeggutorTaurosSeen = new TeamLock {
            Species = 145, // Zapdos
            Comment = "Rhydon & Moltres & Exeggutor & Tauros Seen",
            Locks = new[] {
                new NPCLock(112, true), // Shadow Rhydon (Seen)
                new NPCLock(146, true), // Shadow Moltres (Seen)
                new NPCLock(103, true), // Shadow Exeggutor (Seen)
                new NPCLock(128, true), // Shadow Tauros (Seen)
                new NPCLock(144), // Shadow Articuno
        }};

        public static readonly TeamLock XZapdosRhydonMoltresExeggutorArticunoSeen = new TeamLock {
            Species = 145, // Zapdos
            Comment = "Rhydon & Moltres & Exeggutor & Articuno Seen",
            Locks = new[] {
                new NPCLock(112, true), // Shadow Rhydon (Seen)
                new NPCLock(146, true), // Shadow Moltres (Seen)
                new NPCLock(103, true), // Shadow Exeggutor (Seen)
                new NPCLock(128), // Shadow Tauros
                new NPCLock(144, true), // Shadow Articuno (Seen)
        }};

        public static readonly TeamLock XZapdosRhydonMoltresExeggutorTaurosArticunoSeen = new TeamLock {
            Species = 145, // Zapdos
            Comment = "Rhydon & Moltres & Exeggutor & Tauros & Articuno Seen",
            Locks = new[] {
                new NPCLock(112, true), // Shadow Rhydon (Seen)
                new NPCLock(146, true), // Shadow Moltres (Seen)
                new NPCLock(103, true), // Shadow Exeggutor (Seen)
                new NPCLock(128, true), // Shadow Tauros (Seen)
                new NPCLock(144, true), // Shadow Articuno (Seen)
        }};

        public static readonly TeamLock XDragonite = new TeamLock {
            Species = 149, // Dragonite
            Locks = new[] {
                new NPCLock(272, 00, 0, 127), // Ludicolo (M) (Hardy)
                new NPCLock(272, 18, 0, 127), // Ludicolo (M) (Bashful)
                new NPCLock(272, 12, 1, 127), // Ludicolo (F) (Serious)
                new NPCLock(272, 12, 1, 127), // Ludicolo (F) (Serious)
                new NPCLock(272, 00, 0, 127), // Ludicolo (M) (Hardy)
        }};

        #endregion
    }
}
