using static PKHeX.Core.GameVersion;
using static PKHeX.Core.EncounterGBLanguage;

namespace PKHeX.Core;

internal static class Encounters2GBEra
{
    private static readonly string[] PCNYx = ["PCNYa", "PCNYb", "PCNYc", "PCNYd"];
    private static readonly string[] Stadium2 = ["Stade", "Stadion", "Stadio", "Estadio"];

    internal static readonly EncounterGift2[] StaticEventsGB =
    [
        // Stadium 2 Baton Pass Farfetch'd
        new(083, 05, C) {Moves = new(226, 14, 97, 163), Location = 127, TID16 = 2000, OT_Name = "スタジアム"},
        new(083, 05, C) {Moves = new(226, 14, 97, 163), Location = 127, TID16 = 2000, OT_Name = "Stadium", Language = International},
        new(083, 05, C) {Moves = new(226, 14, 97, 163), Location = 127, TID16 = 2001, OT_Names = Stadium2, Language = International},

        // Stadium 2 Earthquake Gligar
        new(207, 05, C) {Moves = new(89, 68, 17), Location = 127, TID16 = 2000, OT_Name = "スタジアム"},
        new(207, 05, C) {Moves = new(89, 68, 17), Location = 127, TID16 = 2000, OT_Name = "Stadium", Language = International},
        new(207, 05, C) {Moves = new(89, 68, 17), Location = 127, TID16 = 2001, OT_Names = Stadium2, Language = International},

        //New York Pokémon Center Events

        // Legendary Beasts (November 22 to 29, 2001)
        new(243, 05, C) {OT_Names = PCNYx, CurrentLevel = 40, Shiny = Shiny.Always, Location = 127, Language = International}, // Shiny Raikou
        new(244, 05, C) {OT_Names = PCNYx, CurrentLevel = 40, Shiny = Shiny.Always, Location = 127, Language = International}, // Shiny Entei
        new(245, 05, C) {OT_Names = PCNYx, CurrentLevel = 40, Shiny = Shiny.Always, Location = 127, Language = International}, // Shiny Suicune

        // Legendary Birds (November 30 to December 6, 2001)
        new(144, 05, C) {OT_Names = PCNYx, CurrentLevel = 50, Shiny = Shiny.Always, Location = 127, Language = International}, // Shiny Articuno
        new(145, 05, C) {OT_Names = PCNYx, CurrentLevel = 50, Shiny = Shiny.Always, Location = 127, Language = International}, // Shiny Zapdos
        new(146, 05, C) {OT_Names = PCNYx, CurrentLevel = 50, Shiny = Shiny.Always, Location = 127, Language = International}, // Shiny Moltres

        // Christmas Week (December 21 to 27, 2001)
        new(225, 05, GS) {OT_Names = PCNYx, Moves = new(006), EggCycles = 10, Language = International}, // Pay Day Delibird
        new(251, 05, C) {OT_Names = PCNYx, Location = 127, Language = International}, // Celebi

        // The Initial Three Set (December 28, 2001, to January 31, 2002)
        new(001, 05, GS) {OT_Names = PCNYx, Moves = new(246), EggCycles = 10, Language = International}, // Bulbasaur Ancientpower
        new(004, 05, GS) {OT_Names = PCNYx, Moves = new(242), EggCycles = 10, Language = International}, // Charmander Crunch
        new(007, 05, GS) {OT_Names = PCNYx, Moves = new(192), EggCycles = 10, Language = International}, // Squirtle Zap Cannon
        new(152, 05, GS) {OT_Names = PCNYx, Moves = new(080), EggCycles = 10, Language = International}, // Chikorita Petal Dance
        new(155, 05, GS) {OT_Names = PCNYx, Moves = new(038), EggCycles = 10, Language = International}, // Cyndaquil Double-Edge
        new(158, 05, GS) {OT_Names = PCNYx, Moves = new(066), EggCycles = 10, Language = International}, // Totodile Submission

        // Valentine Week (February 1 to 14, 2002)
        new(029, 05, GS) {OT_Names = PCNYx, Moves = new(142), EggCycles = 10, Language = International}, // Nidoran (F) Lovely Kiss
        new(029, 05, GS) {OT_Names = PCNYx, Moves = new(186), EggCycles = 10, Language = International}, // Nidoran (F) Sweet Kiss
        new(032, 05, GS) {OT_Names = PCNYx, Moves = new(142), EggCycles = 10, Language = International}, // Nidoran (M) Lovely Kiss
        new(032, 05, GS) {OT_Names = PCNYx, Moves = new(186), EggCycles = 10, Language = International}, // Nidoran (M) Sweet Kiss
        new(069, 05, GS) {OT_Names = PCNYx, Moves = new(142), EggCycles = 10, Language = International}, // Bellsprout Lovely Kiss
        new(069, 05, GS) {OT_Names = PCNYx, Moves = new(186), EggCycles = 10, Language = International}, // Bellsprout Sweet Kiss

        // Swarm Week (February 22 to March 14, 2002)
        new(183, 05, GS) {OT_Names = PCNYx, Moves = new(056), EggCycles = 10, Language = International}, // Marill Hydro Pump
        new(193, 05, GS) {OT_Names = PCNYx, Moves = new(211), EggCycles = 10, Language = International}, // Yanma Steel Wing
        new(206, 05, GS) {OT_Names = PCNYx, Moves = new(032), EggCycles = 10, Language = International}, // Dunsparce Horn Drill
        new(209, 05, GS) {OT_Names = PCNYx, Moves = new(142), EggCycles = 10, Language = International}, // Snubbull Lovely Kiss
        new(211, 05, GS) {OT_Names = PCNYx, Moves = new(038), EggCycles = 10, Language = International}, // Qwilfish Double-Edge
        new(223, 05, GS) {OT_Names = PCNYx, Moves = new(133), EggCycles = 10, Language = International}, // Remoraid Amnesia

        // Shiny RBY Starters (March 15 to 21, 2002)
        new(003, 05, C) {OT_Names = PCNYx, CurrentLevel = 40, Shiny = Shiny.Always, Location = 127, Language = International}, // Shiny Venusaur
        new(006, 05, C) {OT_Names = PCNYx, CurrentLevel = 40, Shiny = Shiny.Always, Location = 127, Language = International}, // Shiny Charizard
        new(009, 05, C) {OT_Names = PCNYx, CurrentLevel = 40, Shiny = Shiny.Always, Location = 127, Language = International}, // Shiny Blastoise

        // Babies Week (March 22 to April 11, 2002)
        new(172, 05, GS) {OT_Names = PCNYx, Moves = new(047), EggCycles = 10, Language = International}, // Pichu Sing
        new(173, 05, GS) {OT_Names = PCNYx, Moves = new(129), EggCycles = 10, Language = International}, // Cleffa Swift
        new(174, 05, GS) {OT_Names = PCNYx, Moves = new(102), EggCycles = 10, Language = International}, // Igglybuff Mimic
        new(238, 05, GS) {OT_Names = PCNYx, Moves = new(118), EggCycles = 10, Language = International}, // Smoochum Metronome
        new(239, 05, GS) {OT_Names = PCNYx, Moves = new(228), EggCycles = 10, Language = International}, // Elekid Pursuit
        new(240, 05, GS) {OT_Names = PCNYx, Moves = new(185), EggCycles = 10, Language = International}, // Magby Faint Attack

        // Spring Into Spring (April 12 to May 4, 2002)
        new(054, 05, GS) {OT_Names = PCNYx, Moves = new(080), EggCycles = 10, Language = International}, // Psyduck Petal Dance
        new(152, 05, GS) {OT_Names = PCNYx, Moves = new(080), EggCycles = 10, Language = International}, // Chikorita Petal Dance
        new(172, 05, GS) {OT_Names = PCNYx, Moves = new(080), EggCycles = 10, Language = International}, // Pichu Petal Dance
        new(173, 05, GS) {OT_Names = PCNYx, Moves = new(080), EggCycles = 10, Language = International}, // Cleffa Petal Dance
        new(174, 05, GS) {OT_Names = PCNYx, Moves = new(080), EggCycles = 10, Language = International}, // Igglybuff Petal Dance
        new(238, 05, GS) {OT_Names = PCNYx, Moves = new(080), EggCycles = 10, Language = International}, // Smoochum Petal Dance

        // Baby Weeks (May 5 to June 7, 2002)
        new(194, 05, GS) {Moves = new(187), EggCycles = 10, Language = International}, // Wooper Belly Drum

        // Tropical Promotion to Summer Festival 1 (June 8 to 21, 2002)
        new(060, 05, GS) {OT_Names = PCNYx, Moves = new(074), EggCycles = 10, Language = International}, // Poliwag Growth
        new(116, 05, GS) {OT_Names = PCNYx, Moves = new(114), EggCycles = 10, Language = International}, // Horsea Haze
        new(118, 05, GS) {OT_Names = PCNYx, Moves = new(014), EggCycles = 10, Language = International}, // Goldeen Swords Dance
        new(129, 05, GS) {OT_Names = PCNYx, Moves = new(179), EggCycles = 10, Language = International}, // Magikarp Reversal
        new(183, 05, GS) {OT_Names = PCNYx, Moves = new(146), EggCycles = 10, Language = International}, // Marill Dizzy Punch

        // Tropical Promotion to Summer Festival 2 (July 12 to August 8, 2002)
        new(054, 05, GS) {OT_Names = PCNYx, Moves = new(161), EggCycles = 10, Language = International}, // Psyduck Tri Attack
        new(072, 05, GS) {OT_Names = PCNYx, Moves = new(109), EggCycles = 10, Language = International}, // Tentacool Confuse Ray
        new(131, 05, GS) {OT_Names = PCNYx, Moves = new(044), EggCycles = 10, Language = International}, // Lapras Bite
        new(170, 05, GS) {OT_Names = PCNYx, Moves = new(113), EggCycles = 10, Language = International}, // Chinchou Light Screen
        new(223, 05, GS) {OT_Names = PCNYx, Moves = new(054), EggCycles = 10, Language = International}, // Remoraid Mist
        new(226, 05, GS) {OT_Names = PCNYx, Moves = new(016), EggCycles = 10, Language = International}, // Mantine Gust

        // Safari Week (August 9 to 29, 2002)
        new(029, 05, GS) {OT_Names = PCNYx, Moves = new(236), EggCycles = 10, Language = International}, // Nidoran (F) Moonlight
        new(032, 05, GS) {OT_Names = PCNYx, Moves = new(234), EggCycles = 10, Language = International}, // Nidoran (M) Morning Sun
        new(113, 05, GS) {OT_Names = PCNYx, Moves = new(230), EggCycles = 10, Language = International}, // Chansey Sweet Scent
        new(115, 05, GS) {OT_Names = PCNYx, Moves = new(185), EggCycles = 10, Language = International}, // Kangaskhan Faint Attack
        new(128, 05, GS) {OT_Names = PCNYx, Moves = new(098), EggCycles = 10, Language = International}, // Tauros Quick Attack
        new(147, 05, GS) {OT_Names = PCNYx, Moves = new(056), EggCycles = 10, Language = International}, // Dratini Hydro Pump

        // Sky Week (August 30 to September 26, 2002)
        new(021, 05, GS) {OT_Names = PCNYx, Moves = new(049), EggCycles = 10, Language = International}, // Spearow SonicBoom
        new(083, 05, GS) {OT_Names = PCNYx, Moves = new(210), EggCycles = 10, Language = International}, // Farfetch'd Fury Cutter
        new(084, 05, GS) {OT_Names = PCNYx, Moves = new(067), EggCycles = 10, Language = International}, // Doduo Low Kick
        new(177, 05, GS) {OT_Names = PCNYx, Moves = new(219), EggCycles = 10, Language = International}, // Natu Safeguard
        new(198, 05, GS) {OT_Names = PCNYx, Moves = new(251), EggCycles = 10, Language = International}, // Murkrow Beat Up
        new(227, 05, GS) {OT_Names = PCNYx, Moves = new(210), EggCycles = 10, Language = International}, // Skarmory Fury Cutter

        // The Kanto Initial Three Pokémon (September 27 to October 3, 2002)
        new(150, 05, C) {OT_Names = PCNYx, CurrentLevel = 70, Shiny = Shiny.Always, Location = 127, Language = International}, // Shiny Mewtwo

        // Power Plant Pokémon (October 4 to October 10, 2002)
        new(172, 05, GS) {OT_Names = PCNYx, Moves = new(146), EggCycles = 10, Language = International}, // Pichu Dizzy Punch
        new(081, 05, GS) {OT_Names = PCNYx, Moves = new(097), EggCycles = 10, Language = International}, // Magnemite Agility
        new(239, 05, GS) {OT_Names = PCNYx, Moves = new(146), EggCycles = 10, Language = International}, // Elekid Dizzy Punch
        new(100, 05, GS) {OT_Names = PCNYx, Moves = new(097), EggCycles = 10, Language = International}, // Voltorb Agility

        // Scary Face Pokémon (October 25 to October 31, 2002)
        new(173, 05, GS) {OT_Names = PCNYx, Moves = new(184), EggCycles = 10, Language = International}, // Cleffa Scary Face
        new(174, 05, GS) {OT_Names = PCNYx, Moves = new(184), EggCycles = 10, Language = International}, // Igglybuff Scary Face
        new(183, 05, GS) {OT_Names = PCNYx, Moves = new(184), EggCycles = 10, Language = International}, // Marill Scary Face
        new(172, 05, GS) {OT_Names = PCNYx, Moves = new(184), EggCycles = 10, Language = International}, // Pichu Scary Face
        new(194, 05, GS) {OT_Names = PCNYx, Moves = new(184), EggCycles = 10, Language = International}, // Wooper Scary Face

        // Silver Cave (November 1 to November 7, 2002)
        new(114, 05, GS) {OT_Names = PCNYx, Moves = new(235), EggCycles = 10, Language = International}, // Tangela Synthesis
        new(077, 05, GS) {OT_Names = PCNYx, Moves = new(067), EggCycles = 10, Language = International}, // Ponyta Low Kick
        new(200, 05, GS) {OT_Names = PCNYx, Moves = new(095), EggCycles = 10, Language = International}, // Misdreavus Hypnosis
        new(246, 05, GS) {OT_Names = PCNYx, Moves = new(099), EggCycles = 10, Language = International}, // Larvitar Rage

        // Union Cave Pokémon (November 8 to 14, 2002)
        new(120, 05, GS) {OT_Names = PCNYx, Moves = new(239), EggCycles = 10, Language = International}, // Staryu Twister
        new(098, 05, GS) {OT_Names = PCNYx, Moves = new(232), EggCycles = 10, Language = International}, // Krabby Metal Claw
        new(095, 05, GS) {OT_Names = PCNYx, Moves = new(159), EggCycles = 10, Language = International}, // Onix Sharpen
        new(131, 05, GS) {OT_Names = PCNYx, Moves = new(248), EggCycles = 10, Language = International}, // Lapras Future Sight

        // Johto Legendary (November 15 to 21, 2002)
        new(250, 05, C) {OT_Names = PCNYx, CurrentLevel = 40, Shiny = Shiny.Always, Location = 127, Language = International}, // Shiny Ho-Oh
        new(249, 05, C) {OT_Names = PCNYx, CurrentLevel = 40, Shiny = Shiny.Always, Location = 127, Language = International}, // Shiny Lugia

        // Celebi Present SP (November 22 to 28, 2002)
        new(151, 05, C) {OT_Names = PCNYx, Shiny = Shiny.Always, Location = 127, Language = International}, // Shiny Mew

        // Psychic Type Pokémon (November 29 to December 5, 2002)
        new(063, 05, GS) {OT_Names = PCNYx, Moves = new(193), EggCycles = 10, Language = International}, // Abra Foresight
        new(096, 05, GS) {OT_Names = PCNYx, Moves = new(133), EggCycles = 10, Language = International}, // Drowzee Amnesia
        new(102, 05, GS) {OT_Names = PCNYx, Moves = new(230), EggCycles = 10, Language = International}, // Exeggcute Sweet Scent
        new(122, 05, GS) {OT_Names = PCNYx, Moves = new(170), EggCycles = 10, Language = International}, // Mr. Mime Mind Reader

        // The Johto Initial Three Pokémon (December 6 to 12, 2002)
        new(154, 05, C) {OT_Names = PCNYx, CurrentLevel = 40, Shiny = Shiny.Always, Location = 127, Language = International}, // Shiny Meganium
        new(157, 05, C) {OT_Names = PCNYx, CurrentLevel = 40, Shiny = Shiny.Always, Location = 127, Language = International}, // Shiny Typhlosion
        new(160, 05, C) {OT_Names = PCNYx, CurrentLevel = 40, Shiny = Shiny.Always, Location = 127, Language = International}, // Shiny Feraligatr

        // Rock Tunnel Pokémon (December 13 to December 19, 2002)
        new(074, 05, GS) {OT_Names = PCNYx, Moves = new(229), EggCycles = 10, Language = International}, // Geodude Rapid Spin
        new(041, 05, GS) {OT_Names = PCNYx, Moves = new(175), EggCycles = 10, Language = International}, // Zubat Flail
        new(066, 05, GS) {OT_Names = PCNYx, Moves = new(037), EggCycles = 10, Language = International}, // Machop Thrash
        new(104, 05, GS) {OT_Names = PCNYx, Moves = new(031), EggCycles = 10, Language = International}, // Cubone Fury Attack

        // Ice Type Pokémon (December 20 to 26, 2002)
        new(225, 05, GS) {OT_Names = PCNYx, Moves = new(191), EggCycles = 10, Language = International}, // Delibird Spikes
        new(086, 05, GS) {OT_Names = PCNYx, Moves = new(175), EggCycles = 10, Language = International}, // Seel Flail
        new(220, 05, GS) {OT_Names = PCNYx, Moves = new(018), EggCycles = 10, Language = International}, // Swinub Whirlwind

        // Pokémon that Appear at Night only (December 27, 2002, to January 2, 2003)
        new(163, 05, GS) {OT_Names = PCNYx, Moves = new(101), EggCycles = 10, Language = International}, // Hoothoot Night Shade
        new(215, 05, GS) {OT_Names = PCNYx, Moves = new(236), EggCycles = 10, Language = International}, // Sneasel Moonlight

        // Grass Type ( January 3 to 9, 2003)
        new(191, 05, GS) {OT_Names = PCNYx, Moves = new(150), EggCycles = 10, Language = International}, // Sunkern Splash
        new(046, 05, GS) {OT_Names = PCNYx, Moves = new(235), EggCycles = 10, Language = International}, // Paras Synthesis
        new(187, 05, GS) {OT_Names = PCNYx, Moves = new(097), EggCycles = 10, Language = International}, // Hoppip Agility
        new(043, 05, GS) {OT_Names = PCNYx, Moves = new(073), EggCycles = 10, Language = International}, // Oddish Leech Seed

        // Normal Pokémon (January 10 to 16, 2003)
        new(161, 05, GS) {OT_Names = PCNYx, Moves = new(146), EggCycles = 10, Language = International}, // Sentret Dizzy Punch
        new(234, 05, GS) {OT_Names = PCNYx, Moves = new(219), EggCycles = 10, Language = International}, // Stantler Safeguard
        new(241, 05, GS) {OT_Names = PCNYx, Moves = new(025), EggCycles = 10, Language = International}, // Miltank Mega Kick
        new(190, 05, GS) {OT_Names = PCNYx, Moves = new(102), EggCycles = 10, Language = International}, // Aipom Mimic
        new(108, 05, GS) {OT_Names = PCNYx, Moves = new(003), EggCycles = 10, Language = International}, // Lickitung DoubleSlap
        new(143, 05, GS) {OT_Names = PCNYx, Moves = new(150), EggCycles = 10, Language = International}, // Snorlax Splash

        // Mt. Mortar (January 24 to 30, 2003)
        new(066, 05, GS) {OT_Names = PCNYx, Moves = new(206), EggCycles = 10, Language = International}, // Machop False Swipe
        new(129, 05, GS) {OT_Names = PCNYx, Moves = new(145), EggCycles = 10, Language = International}, // Magikarp Bubble
        new(236, 05, GS) {OT_Names = PCNYx, Moves = new(099), EggCycles = 10, Language = International}, // Tyrogue Rage

        // Dark Cave Pokémon (January 31 to February 6, 2003)
        new(206, 05, GS) {OT_Names = PCNYx, Moves = new(031), EggCycles = 10, Language = International}, // Dunsparce Fury Attack
        new(202, 05, GS) {OT_Names = PCNYx, Moves = new(102), EggCycles = 10, Language = International}, // Wobbuffet Mimic
        new(231, 05, GS) {OT_Names = PCNYx, Moves = new(071), EggCycles = 10, Language = International}, // Phanpy Absorb
        new(216, 05, GS) {OT_Names = PCNYx, Moves = new(230), EggCycles = 10, Language = International}, // Teddiursa Sweet Scent

        // Valentine's Day Special (February 7 to 13, 2003)
        new(060, 05, GS) {OT_Names = PCNYx, Moves = new(186), EggCycles = 10, Language = International}, // Poliwag Sweet Kiss
        new(060, 05, GS) {OT_Names = PCNYx, Moves = new(142), EggCycles = 10, Language = International}, // Poliwag Lovely Kiss
        new(143, 05, GS) {OT_Names = PCNYx, Moves = new(186), EggCycles = 10, Language = International}, // Snorlax Sweet Kiss
        new(143, 05, GS) {OT_Names = PCNYx, Moves = new(142), EggCycles = 10, Language = International}, // Snorlax Lovely Kiss

        // Rare Pokémon (February 21 to 27, 2003)
        new(140, 05, GS) {OT_Names = PCNYx, Moves = new(088), EggCycles = 10, Language = International}, // Kabuto Rock Throw
        new(138, 05, GS) {OT_Names = PCNYx, Moves = new(088), EggCycles = 10, Language = International}, // Omanyte Rock Throw
        new(142, 05, GS) {OT_Names = PCNYx, Moves = new(088), EggCycles = 10, Language = International}, // Aerodactyl Rock Throw
        new(137, 05, GS) {OT_Names = PCNYx, Moves = new(112), EggCycles = 10, Language = International}, // Porygon Barrier
        new(133, 05, GS) {OT_Names = PCNYx, Moves = new(074), EggCycles = 10, Language = International}, // Eevee Growth
        new(185, 05, GS) {OT_Names = PCNYx, Moves = new(164), EggCycles = 10, Language = International}, // Sudowoodo Substitute

        // Bug Type Pokémon (February 28 to March 6, 2003)
        new(123, 05, GS) {OT_Names = PCNYx, Moves = new(049), EggCycles = 10, Language = International}, // Scyther SonicBoom
        new(214, 05, GS) {OT_Names = PCNYx, Moves = new(069), EggCycles = 10, Language = International}, // Heracross Seismic Toss
        new(127, 05, GS) {OT_Names = PCNYx, Moves = new(088), EggCycles = 10, Language = International}, // Pinsir Rock Throw
        new(165, 05, GS) {OT_Names = PCNYx, Moves = new(112), EggCycles = 10, Language = International}, // Ledyba Barrier
        new(167, 05, GS) {OT_Names = PCNYx, Moves = new(074), EggCycles = 10, Language = International}, // Spinarak Growth
        new(193, 05, GS) {OT_Names = PCNYx, Moves = new(186), EggCycles = 10, Language = International}, // Yanma Sweet Kiss
        new(204, 05, GS) {OT_Names = PCNYx, Moves = new(164), EggCycles = 10, Language = International}, // Pineco Substitute

        // Japanese Only (all below)
        new(251, 30, GSC) {Location = 014}, // Celebi @ Ilex Forest (GBC)

        // Gen2 Events
        // Egg Cycles Subject to Change. OTs for Eggs are unknown.
        // Pokémon Center Mystery Egg #1 (December 15, 2001, to January 14, 2002)
        new(152, 05, GSC) {Moves = new(080), EggCycles = 10}, // Chikorita Petal Dance
        new(172, 05, GSC) {Moves = new(047), EggCycles = 10}, // Pichu Sing
        new(173, 05, GSC) {Moves = new(129), EggCycles = 10}, // Cleffa Swift
        new(194, 05, GSC) {Moves = new(187), EggCycles = 10}, // Wooper Belly Drum
        new(231, 05, GSC) {Moves = new(227), EggCycles = 10}, // Phanpy Encore
        new(238, 05, GSC) {Moves = new(118), EggCycles = 10}, // Smoochum Metronome

        // Pokémon Center Mystery Egg #2 (March 16 to April 7, 2002)
        new(054, 05, GSC) {Moves = new(080), EggCycles = 10}, // Psyduck Petal Dance
        new(152, 05, GSC) {Moves = new(080), EggCycles = 10}, // Chikorita Petal Dance
        new(172, 05, GSC) {Moves = new(080), EggCycles = 10}, // Pichu Petal Dance
        new(173, 05, GSC) {Moves = new(080), EggCycles = 10}, // Cleffa Petal Dance
        new(174, 05, GSC) {Moves = new(080), EggCycles = 10}, // Igglybuff Petal Dance
        new(238, 05, GSC) {Moves = new(080), EggCycles = 10}, // Smoochum Petal Dance

        // Pokémon Center Mystery Egg #3 (April 27 to May 12, 2002)
        new(001, 05, GSC) {Moves = new(246), EggCycles = 10}, // Bulbasaur Ancientpower
        new(004, 05, GSC) {Moves = new(242), EggCycles = 10}, // Charmander Crunch
        new(158, 05, GSC) {Moves = new(066), EggCycles = 10}, // Totodile Submission
        new(163, 05, GSC) {Moves = new(101), EggCycles = 10}, // Hoot-Hoot Night Shade
    ];
}
