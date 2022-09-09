namespace PKHeX.Core;

// Distribution Nest Encounters (BCAT)
internal static partial class Encounters8Nest
{
    /// <summary>
    /// Nest distribution raids for <see cref="GameVersion.SWSH"/> available after Crown Tundra expansion.
    /// </summary>
    internal static readonly EncounterStatic8ND[] Dist_DLC2 =
    {
        new(17,01,1) { Species = 872, Ability = A4, Moves = new(496, 196, 263, 181), Index = 113 }, // Snom
        new(17,01,1) { Species = 771, Ability = A4, Moves = new(092, 220, 599, 000), Index = 113 }, // Pyukumuku
        new(17,01,1) { Species = 871, Ability = A4, Moves = new(084, 061, 064, 031), Index = 113 }, // Pincurchin
        new(30,03,2) { Species = 872, Ability = A4, Moves = new(496, 196, 263, 181), Index = 113 }, // Snom
        new(30,03,2) { Species = 771, Ability = A4, Moves = new(092, 220, 599, 000), Index = 113 }, // Pyukumuku
        new(30,03,2) { Species = 871, Ability = A4, Moves = new(716, 061, 398, 675), Index = 113 }, // Pincurchin
        new(40,05,3) { Species = 872, Ability = A4, Moves = new(405, 196, 263, 181), Index = 113 }, // Snom
        new(40,05,3) { Species = 771, Ability = A4, Moves = new(092, 220, 599, 000), Index = 113 }, // Pyukumuku
        new(40,05,3) { Species = 871, Ability = A4, Moves = new(716, 710, 398, 675), Index = 113 }, // Pincurchin
        new(50,08,4) { Species = 872, Ability = A4, Moves = new(405, 196, 263, 522), Index = 113 }, // Snom
        new(50,08,4) { Species = 771, Ability = A4, Moves = new(092, 220, 599, 000), Index = 113 }, // Pyukumuku
        new(50,08,4) { Species = 871, Ability = A4, Moves = new(716, 056, 398, 675), Index = 113 }, // Pincurchin
        new(60,10,5) { Species = 872, Ability = A4, Moves = new(405, 196, 263, 522), Index = 113, Shiny = Shiny.Always }, // Snom
        new(60,10,5) { Species = 872, Ability = A4, Moves = new(405, 196, 263, 522), Index = 113 }, // Snom
        new(60,10,5) { Species = 771, Ability = A4, Moves = new(092, 220, 599, 213), Index = 113 }, // Pyukumuku
        new(60,10,5) { Species = 871, Ability = A4, Moves = new(435, 056, 398, 675), Index = 113 }, // Pincurchin

        new(17,01,1) { Species = 859, Ability = A4, Moves = new(372, 044, 252, 590), Index = 111 }, // Impidimp
        new(30,03,2) { Species = 859, Ability = A4, Moves = new(399, 583, 252, 005), Index = 111 }, // Impidimp
        new(40,05,3) { Species = 860, Ability = A4, Moves = new(793, 583, 421, 025), Index = 111 }, // Morgrem
        new(40,05,3) { Species = 859, Ability = A4, Moves = new(399, 583, 252, 005), Index = 111 }, // Impidimp
        new(50,08,4) { Species = 861, Ability = A4, Moves = new(789, 492, 359, 025), Index = 111 }, // Grimmsnarl
        new(50,08,4) { Species = 860, Ability = A4, Moves = new(793, 583, 421, 025), Index = 111 }, // Morgrem
        new(50,08,4) { Species = 859, Ability = A4, Moves = new(399, 583, 252, 005), Index = 111 }, // Impidimp
        new(60,10,5) { Species = 861, Ability = A4, Moves = new(789, 663, 359, 025), Index = 111, Shiny = Shiny.Always }, // Grimmsnarl
        new(60,10,5) { Species = 861, Ability = A4, Moves = new(789, 663, 359, 025), Index = 111 }, // Grimmsnarl

        new(17,01,1) { Species = 183, Ability = A4, Moves = new(583, 392, 205, 021), Index = 109 }, // Marill
        new(17,01,1) { Species = 060, Ability = A4, Moves = new(055, 001, 341, 061), Index = 109 }, // Poliwag
        new(17,01,1) { Species = 363, Ability = A4, Moves = new(181, 055, 205, 111), Index = 109 }, // Spheal
        new(30,03,2) { Species = 183, Ability = A4, Moves = new(583, 392, 231, 021), Index = 109 }, // Marill
        new(30,03,2) { Species = 060, Ability = A4, Moves = new(061, 001, 341, 094), Index = 109 }, // Poliwag
        new(30,03,2) { Species = 363, Ability = A4, Moves = new(062, 362, 205, 231), Index = 109 }, // Spheal
        new(40,05,3) { Species = 183, Ability = A4, Moves = new(583, 056, 231, 021), Index = 109 }, // Marill
        new(40,05,3) { Species = 060, Ability = A4, Moves = new(056, 058, 341, 094), Index = 109 }, // Poliwag
        new(40,05,3) { Species = 363, Ability = A4, Moves = new(062, 057, 523, 231), Index = 109 }, // Spheal
        new(50,08,4) { Species = 183, Ability = A4, Moves = new(583, 056, 276, 021), Index = 109 }, // Marill
        new(50,08,4) { Species = 060, Ability = A4, Moves = new(056, 058, 414, 094), Index = 109 }, // Poliwag
        new(50,08,4) { Species = 363, Ability = A4, Moves = new(058, 057, 523, 231), Index = 109 }, // Spheal
        new(60,10,5) { Species = 183, Ability = A4, Moves = new(583, 056, 276, 059), Index = 109, Shiny = Shiny.Always }, // Marill
        new(60,10,5) { Species = 183, Ability = A4, Moves = new(583, 056, 276, 059), Index = 109 }, // Marill
        new(60,10,5) { Species = 060, Ability = A4, Moves = new(503, 058, 414, 094), Index = 109 }, // Poliwag
        new(60,10,5) { Species = 363, Ability = A4, Moves = new(329, 057, 523, 231), Index = 109 }, // Spheal

        new(17,01,1) { Species = 824, Ability = A4, Moves = new(522, 000, 000, 000), Index = 106 }, // Blipbug
        new(30,03,2) { Species = 833, Ability = A4, Moves = new(055, 033, 044, 029), Index = 106 }, // Chewtle
        new(40,05,3) { Species = 832, Ability = A4, Moves = new(036, 024, 428, 086), Index = 106 }, // Dubwool
        new(50,08,4) { Species = 823, Ability = A4, Moves = new(413, 442, 034, 681), Index = 106 }, // Corviknight
        //new(60,10,5) { Species = 892, Ability = A0, Moves = new(555, 370, 389, 398), Index = 106, CanGigantamax = true }, // Urshifu
        //new(60,10,5) { Species = 892, Ability = A0, Moves = new(710, 370, 009, 512), Index = 106, Form = 1, CanGigantamax = true }, // Urshifu-1

        new(17,01,1) { Species = 090, Ability = A4, Moves = new(420, 056, 033, 250), Index = 104 }, // Shellder
        new(17,01,1) { Species = 090, Ability = A4, Moves = new(420, 057, 033, 710), Index = 104 }, // Shellder
        new(30,03,2) { Species = 090, Ability = A4, Moves = new(062, 056, 033, 250), Index = 104 }, // Shellder
        new(30,03,2) { Species = 090, Ability = A4, Moves = new(062, 057, 033, 710), Index = 104 }, // Shellder
        new(40,05,3) { Species = 090, Ability = A4, Moves = new(062, 056, 504, 534), Index = 104 }, // Shellder
        new(40,05,3) { Species = 090, Ability = A4, Moves = new(062, 057, 161, 710), Index = 104 }, // Shellder
        new(50,08,4) { Species = 090, Ability = A4, Moves = new(058, 057, 504, 534), Index = 104 }, // Shellder
        new(50,08,4) { Species = 090, Ability = A4, Moves = new(058, 057, 161, 710), Index = 104 }, // Shellder
        new(60,10,5) { Species = 090, Ability = A4, Moves = new(058, 057, 504, 534), Index = 104, Shiny = Shiny.Always }, // Shellder
        new(60,10,5) { Species = 090, Ability = A4, Moves = new(058, 057, 161, 710), Index = 104 }, // Shellder
        new(60,10,5) { Species = 090, Ability = A4, Moves = new(058, 057, 504, 534), Index = 104 }, // Shellder

        new(17,01,1) { Species = 438, Ability = A4, Moves = new(088, 383, 175, 313), Index = 102 }, // Bonsly
        new(30,03,2) { Species = 438, Ability = A4, Moves = new(088, 317, 175, 313), Index = 102 }, // Bonsly
        new(40,05,3) { Species = 438, Ability = A4, Moves = new(317, 389, 157, 313), Index = 102 }, // Bonsly
        new(50,08,4) { Species = 185, Ability = A4, Moves = new(452, 359, 157, 389), Index = 102 }, // Sudowoodo
        new(60,10,5) { Species = 185, Ability = A4, Moves = new(452, 444, 038, 389), Index = 102, Shiny = Shiny.Always }, // Sudowoodo
        new(60,10,5) { Species = 185, Ability = A4, Moves = new(452, 444, 038, 389), Index = 102 }, // Sudowoodo

        new(17,01,1) { Species = 696, Ability = A4, Moves = new(246, 033, 525, 046), Index = 100 }, // Tyrunt
        new(17,01,1) { Species = 564, Ability = A4, Moves = new(453, 414, 246, 044), Index = 100 }, // Tirtouga
        new(17,01,1) { Species = 566, Ability = A4, Moves = new(017, 246, 225, 414), Index = 100 }, // Archen
        new(17,01,1) { Species = 698, Ability = A4, Moves = new(181, 086, 246, 196), Index = 100 }, // Amaura
        new(30,03,2) { Species = 696, Ability = A4, Moves = new(246, 523, 525, 044), Index = 100 }, // Tyrunt
        new(30,03,2) { Species = 564, Ability = A4, Moves = new(453, 414, 246, 242), Index = 100 }, // Tirtouga
        new(30,03,2) { Species = 566, Ability = A4, Moves = new(017, 246, 225, 414), Index = 100 }, // Archen
        new(30,03,2) { Species = 698, Ability = A4, Moves = new(062, 086, 246, 196), Index = 100 }, // Amaura
        new(40,05,3) { Species = 697, Ability = A4, Moves = new(444, 523, 337, 231), Index = 100 }, // Tyrantrum
        new(40,05,3) { Species = 565, Ability = A4, Moves = new(453, 414, 246, 231), Index = 100 }, // Carracosta
        new(40,05,3) { Species = 567, Ability = A4, Moves = new(403, 157, 337, 414), Index = 100 }, // Archeops
        new(40,05,3) { Species = 699, Ability = A4, Moves = new(059, 086, 444, 304), Index = 100 }, // Aurorus
        new(50,08,4) { Species = 697, Ability = A4, Moves = new(444, 089, 337, 231), Index = 100 }, // Tyrantrum
        new(50,08,4) { Species = 565, Ability = A4, Moves = new(056, 414, 246, 231), Index = 100 }, // Carracosta
        new(50,08,4) { Species = 567, Ability = A4, Moves = new(403, 444, 337, 414), Index = 100 }, // Archeops
        new(50,08,4) { Species = 699, Ability = A4, Moves = new(059, 094, 444, 304), Index = 100 }, // Aurorus
        new(60,10,5) { Species = 697, Ability = A4, Moves = new(457, 089, 406, 231), Index = 100, Shiny = Shiny.Always }, // Tyrantrum
        new(60,10,5) { Species = 697, Ability = A4, Moves = new(444, 089, 406, 231), Index = 100 }, // Tyrantrum
        new(60,10,5) { Species = 565, Ability = A4, Moves = new(056, 444, 089, 231), Index = 100 }, // Carracosta
        new(60,10,5) { Species = 567, Ability = A4, Moves = new(403, 444, 406, 414), Index = 100 }, // Archeops
        new(60,10,5) { Species = 699, Ability = A4, Moves = new(059, 573, 444, 304), Index = 100 }, // Aurorus

        new(50,08,4) { Species = 003, Ability = A4, Moves = new(572, 188, 414, 200), Index = 98, CanGigantamax = true }, // Venusaur
        new(50,08,4) { Species = 006, Ability = A4, Moves = new(257, 076, 542, 406), Index = 98, CanGigantamax = true }, // Charizard
        new(50,08,4) { Species = 009, Ability = A4, Moves = new(057, 059, 430, 089), Index = 98, CanGigantamax = true }, // Blastoise
        new(80,10,5) { Species = 003, Ability = A4, Moves = new(572, 188, 414, 200), Index = 98, CanGigantamax = true }, // Venusaur
        new(80,10,5) { Species = 006, Ability = A4, Moves = new(257, 076, 542, 406), Index = 98, CanGigantamax = true }, // Charizard
        new(80,10,5) { Species = 009, Ability = A4, Moves = new(057, 059, 430, 089), Index = 98, CanGigantamax = true }, // Blastoise

        new(17,01,1) { Species = 129, Ability = A4, Moves = new(150, 033, 000, 000), Index = 95 }, // Magikarp
        new(17,01,1) { Species = 052, Ability = A4, Moves = new(006, 492, 402, 247), Index = 95 }, // Meowth
        new(17,01,1) { Species = 052, Ability = A4, Moves = new(006, 441, 087, 231), Index = 95 }, // Meowth
        new(17,01,1) { Species = 438, Ability = A4, Moves = new(088, 383, 175, 313), Index = 95 }, // Bonsly
        new(17,01,1) { Species = 554, Ability = A4, Moves = new(052, 044, 033, 526), Index = 95 }, // Darumaka
        new(30,03,2) { Species = 129, Ability = A4, Moves = new(150, 033, 000, 000), Index = 95 }, // Magikarp
        new(30,03,2) { Species = 052, Ability = A4, Moves = new(006, 492, 402, 247), Index = 95 }, // Meowth
        new(30,03,2) { Species = 052, Ability = A4, Moves = new(006, 441, 087, 231), Index = 95 }, // Meowth
        new(30,03,2) { Species = 438, Ability = A4, Moves = new(088, 317, 175, 313), Index = 95 }, // Bonsly
        new(30,03,2) { Species = 554, Ability = A4, Moves = new(007, 044, 157, 029), Index = 95 }, // Darumaka
        new(40,05,3) { Species = 129, Ability = A4, Moves = new(150, 033, 175, 000), Index = 95 }, // Magikarp
        new(40,05,3) { Species = 052, Ability = A4, Moves = new(006, 492, 402, 247), Index = 95 }, // Meowth
        new(40,05,3) { Species = 052, Ability = A4, Moves = new(006, 441, 087, 231), Index = 95 }, // Meowth
        new(40,05,3) { Species = 438, Ability = A4, Moves = new(317, 389, 157, 313), Index = 95 }, // Bonsly
        new(40,05,3) { Species = 555, Ability = A4, Moves = new(359, 276, 157, 442), Index = 95 }, // Darmanitan
        new(50,08,4) { Species = 129, Ability = A4, Moves = new(150, 033, 175, 000), Index = 95 }, // Magikarp
        new(50,08,4) { Species = 052, Ability = A4, Moves = new(006, 492, 402, 247), Index = 95 }, // Meowth
        new(50,08,4) { Species = 052, Ability = A4, Moves = new(006, 441, 087, 231), Index = 95 }, // Meowth
        new(50,08,4) { Species = 185, Ability = A4, Moves = new(452, 359, 157, 389), Index = 95 }, // Sudowoodo
        new(50,08,4) { Species = 555, Ability = A4, Moves = new(394, 276, 157, 442), Index = 95 }, // Darmanitan
        new(60,10,5) { Species = 129, Ability = A4, Moves = new(150, 033, 175, 340), Index = 95, Shiny = Shiny.Always }, // Magikarp
        new(60,10,5) { Species = 129, Ability = A4, Moves = new(150, 033, 175, 340), Index = 95 }, // Magikarp
        new(60,10,5) { Species = 052, Ability = A4, Moves = new(006, 492, 402, 247), Index = 95 }, // Meowth
        new(60,10,5) { Species = 052, Ability = A4, Moves = new(006, 441, 087, 231), Index = 95 }, // Meowth
        new(60,10,5) { Species = 185, Ability = A4, Moves = new(452, 444, 038, 389), Index = 95 }, // Sudowoodo
        new(60,10,5) { Species = 555, Ability = A4, Moves = new(394, 276, 089, 442), Index = 95 }, // Darmanitan

        new(17,01,1) { Species = 225, Ability = A4, Moves = new(217, 065, 034, 372), Index = 93 }, // Delibird
        new(17,01,1) { Species = 121, Ability = A4, Moves = new(057, 408, 055, 129), Index = 93 }, // Starmie
        new(17,01,1) { Species = 615, Ability = A4, Moves = new(196, 020, 229, 420), Index = 93 }, // Cryogonal
        new(30,03,2) { Species = 225, Ability = A4, Moves = new(217, 065, 034, 693), Index = 93 }, // Delibird
        new(30,03,2) { Species = 121, Ability = A4, Moves = new(057, 408, 094, 129), Index = 93 }, // Starmie
        new(30,03,2) { Species = 615, Ability = A4, Moves = new(400, 062, 229, 246), Index = 93 }, // Cryogonal
        new(40,05,3) { Species = 225, Ability = A4, Moves = new(217, 065, 280, 196), Index = 93 }, // Delibird
        new(40,05,3) { Species = 121, Ability = A4, Moves = new(056, 408, 094, 129), Index = 93 }, // Starmie
        new(40,05,3) { Species = 615, Ability = A4, Moves = new(400, 062, 573, 246), Index = 93 }, // Cryogonal
        new(50,08,4) { Species = 225, Ability = A4, Moves = new(217, 059, 034, 280), Index = 93 }, // Delibird
        new(50,08,4) { Species = 121, Ability = A4, Moves = new(056, 408, 094, 605), Index = 93 }, // Starmie
        new(50,08,4) { Species = 615, Ability = A4, Moves = new(400, 058, 573, 430), Index = 93 }, // Cryogonal
        new(60,10,5) { Species = 225, Ability = A4, Moves = new(217, 059, 065, 280), Index = 93, Shiny = Shiny.Always }, // Delibird
        new(60,10,5) { Species = 225, Ability = A4, Moves = new(217, 059, 065, 280), Index = 93 }, // Delibird
        new(60,10,5) { Species = 121, Ability = A4, Moves = new(056, 800, 094, 605), Index = 93 }, // Starmie
        new(60,10,5) { Species = 615, Ability = A4, Moves = new(400, 329, 573, 430), Index = 93 }, // Cryogonal

        new(17,01,1) { Species = 133, Ability = A4, Moves = new(033, 098, 039, 608), Index = 91 }, // Eevee
        new(30,03,2) { Species = 133, Ability = A4, Moves = new(129, 098, 039, 608), Index = 91 }, // Eevee
        new(40,05,3) { Species = 133, Ability = A4, Moves = new(129, 098, 231, 608), Index = 91 }, // Eevee
        new(40,05,3) { Species = 134, Ability = A4, Moves = new(352, 058, 330, 304), Index = 91 }, // Vaporeon
        new(40,05,3) { Species = 135, Ability = A4, Moves = new(422, 086, 247, 129), Index = 91 }, // Jolteon
        new(40,05,3) { Species = 136, Ability = A4, Moves = new(436, 098, 276, 044), Index = 91 }, // Flareon
        new(40,05,3) { Species = 196, Ability = A4, Moves = new(060, 605, 231, 098), Index = 91 }, // Espeon
        new(40,05,3) { Species = 197, Ability = A4, Moves = new(555, 044, 247, 098), Index = 91 }, // Umbreon
        new(40,05,3) { Species = 470, Ability = A4, Moves = new(202, 098, 073, 231), Index = 91 }, // Leafeon
        new(40,05,3) { Species = 471, Ability = A4, Moves = new(573, 059, 247, 129), Index = 91 }, // Glaceon
        new(40,05,3) { Species = 700, Ability = A4, Moves = new(574, 595, 129, 605), Index = 91 }, // Sylveon
        new(50,08,4) { Species = 133, Ability = A4, Moves = new(129, 500, 231, 204), Index = 91 }, // Eevee
        new(50,08,4) { Species = 134, Ability = A4, Moves = new(056, 058, 330, 304), Index = 91 }, // Vaporeon
        new(50,08,4) { Species = 135, Ability = A4, Moves = new(087, 086, 247, 129), Index = 91 }, // Jolteon
        new(50,08,4) { Species = 136, Ability = A4, Moves = new(394, 098, 276, 044), Index = 91 }, // Flareon
        new(50,08,4) { Species = 196, Ability = A4, Moves = new(094, 605, 231, 098), Index = 91 }, // Espeon
        new(50,08,4) { Species = 197, Ability = A4, Moves = new(555, 492, 247, 098), Index = 91 }, // Umbreon
        new(50,08,4) { Species = 470, Ability = A4, Moves = new(348, 098, 073, 231), Index = 91 }, // Leafeon
        new(50,08,4) { Species = 471, Ability = A4, Moves = new(573, 059, 247, 311), Index = 91 }, // Glaceon
        new(50,08,4) { Species = 700, Ability = A4, Moves = new(585, 595, 129, 605), Index = 91 }, // Sylveon
        new(60,10,5) { Species = 133, Ability = A4, Moves = new(387, 500, 231, 204), Index = 91, Shiny = Shiny.Always }, // Eevee
        new(60,10,5) { Species = 134, Ability = A4, Moves = new(056, 058, 503, 304), Index = 91 }, // Vaporeon
        new(60,10,5) { Species = 135, Ability = A4, Moves = new(087, 085, 247, 129), Index = 91 }, // Jolteon
        new(60,10,5) { Species = 136, Ability = A4, Moves = new(394, 231, 276, 044), Index = 91 }, // Flareon
        new(60,10,5) { Species = 196, Ability = A4, Moves = new(094, 605, 231, 129), Index = 91 }, // Espeon
        new(60,10,5) { Species = 197, Ability = A4, Moves = new(555, 492, 247, 304), Index = 91 }, // Umbreon
        new(60,10,5) { Species = 470, Ability = A4, Moves = new(348, 311, 073, 231), Index = 91 }, // Leafeon
        new(60,10,5) { Species = 471, Ability = A4, Moves = new(573, 059, 247, 304), Index = 91 }, // Glaceon
        new(60,10,5) { Species = 700, Ability = A4, Moves = new(585, 595, 304, 605), Index = 91 }, // Sylveon
        new(60,10,5) { Species = 133, Ability = A4, Moves = new(387, 500, 231, 204), Index = 91, CanGigantamax = true }, // Eevee

        new(17,01,1) { Species = 570, Ability = A4, Moves = new(468, 247, 010, 043), Index = 89 }, // Zorua
        new(17,01,1) { Species = 302, Ability = A4, Moves = new(252, 010, 425, 555), Index = 89 }, // Sableye
        new(17,01,1) { Species = 355, Ability = A4, Moves = new(310, 425, 043, 506), Index = 89 }, // Duskull
        new(17,01,1) { Species = 821, Ability = A4, Moves = new(403, 031, 043, 681), Index = 89 }, // Rookidee
        new(17,01,1) { Species = 827, Ability = A4, Moves = new(555, 098, 251, 468), Index = 89 }, // Nickit
        new(30,03,2) { Species = 571, Ability = A4, Moves = new(400, 247, 279, 304), Index = 89 }, // Zoroark
        new(30,03,2) { Species = 302, Ability = A4, Moves = new(252, 094, 425, 555), Index = 89 }, // Sableye
        new(30,03,2) { Species = 355, Ability = A4, Moves = new(310, 425, 371, 506), Index = 89 }, // Duskull
        new(30,03,2) { Species = 822, Ability = A4, Moves = new(403, 263, 279, 681), Index = 89 }, // Corvisquire
        new(30,03,2) { Species = 828, Ability = A4, Moves = new(555, 098, 251, 583), Index = 89 }, // Thievul
        new(40,05,3) { Species = 571, Ability = A4, Moves = new(400, 247, 411, 304), Index = 89 }, // Zoroark
        new(40,05,3) { Species = 302, Ability = A4, Moves = new(252, 261, 247, 555), Index = 89 }, // Sableye
        new(40,05,3) { Species = 477, Ability = A4, Moves = new(247, 009, 371, 157), Index = 89 }, // Dusknoir
        new(40,05,3) { Species = 823, Ability = A4, Moves = new(403, 442, 034, 681), Index = 89 }, // Corviknight
        new(40,05,3) { Species = 828, Ability = A4, Moves = new(555, 098, 094, 583), Index = 89 }, // Thievul
        new(50,08,4) { Species = 571, Ability = A4, Moves = new(539, 247, 411, 304), Index = 89 }, // Zoroark
        new(50,08,4) { Species = 302, Ability = A4, Moves = new(605, 261, 247, 555), Index = 89 }, // Sableye
        new(50,08,4) { Species = 477, Ability = A4, Moves = new(247, 009, 280, 157), Index = 89 }, // Dusknoir
        new(50,08,4) { Species = 823, Ability = A4, Moves = new(413, 442, 034, 681), Index = 89 }, // Corviknight
        new(50,08,4) { Species = 828, Ability = A4, Moves = new(555, 341, 094, 583), Index = 89 }, // Thievul
        new(60,10,5) { Species = 571, Ability = A4, Moves = new(539, 247, 411, 492), Index = 89, Shiny = Shiny.Always }, // Zoroark
        new(60,10,5) { Species = 571, Ability = A4, Moves = new(539, 247, 411, 492), Index = 89 }, // Zoroark
        new(60,10,5) { Species = 302, Ability = A4, Moves = new(605, 261, 247, 492), Index = 89 }, // Sableye
        new(60,10,5) { Species = 477, Ability = A4, Moves = new(247, 009, 280, 089), Index = 89 }, // Dusknoir
        new(60,10,5) { Species = 823, Ability = A4, Moves = new(413, 442, 776, 372), Index = 89 }, // Corviknight
        new(60,10,5) { Species = 828, Ability = A4, Moves = new(555, 492, 094, 583), Index = 89 }, // Thievul

        new(17,01,1) { Species = 722, Ability = A4, Moves = new(064, 075, 389, 129), Index = 87 }, // Rowlet
        new(17,01,1) { Species = 725, Ability = A4, Moves = new(052, 006, 044, 421), Index = 87 }, // Litten
        new(17,01,1) { Species = 728, Ability = A4, Moves = new(574, 001, 453, 196), Index = 87 }, // Popplio
        new(30,03,2) { Species = 722, Ability = A4, Moves = new(064, 348, 389, 129), Index = 87 }, // Rowlet
        new(30,03,2) { Species = 725, Ability = A4, Moves = new(053, 006, 044, 421), Index = 87 }, // Litten
        new(30,03,2) { Species = 728, Ability = A4, Moves = new(574, 061, 453, 196), Index = 87 }, // Popplio
        new(40,05,3) { Species = 722, Ability = A4, Moves = new(413, 348, 389, 129), Index = 87 }, // Rowlet
        new(40,05,3) { Species = 725, Ability = A4, Moves = new(394, 006, 044, 421), Index = 87 }, // Litten
        new(40,05,3) { Species = 728, Ability = A4, Moves = new(585, 056, 453, 196), Index = 87 }, // Popplio
        new(50,08,4) { Species = 722, Ability = A4, Moves = new(413, 348, 389, 412), Index = 87 }, // Rowlet
        new(50,08,4) { Species = 725, Ability = A4, Moves = new(394, 006, 279, 421), Index = 87 }, // Litten
        new(50,08,4) { Species = 728, Ability = A4, Moves = new(585, 056, 453, 059), Index = 87 }, // Popplio
        new(60,10,5) { Species = 722, Ability = A4, Moves = new(413, 348, 421, 412), Index = 87 }, // Rowlet
        new(60,10,5) { Species = 725, Ability = A4, Moves = new(394, 006, 279, 242), Index = 87 }, // Litten
        new(60,10,5) { Species = 728, Ability = A4, Moves = new(585, 056, 453, 058), Index = 87 }, // Popplio

        new(17,01,1) { Species = 337, Ability = A4, Moves = new(585, 033, 093, 088), Index = 85 }, // Lunatone
        new(17,01,1) { Species = 338, Ability = A4, Moves = new(394, 033, 093, 088), Index = 85 }, // Solrock
        new(30,03,2) { Species = 337, Ability = A4, Moves = new(585, 129, 094, 088), Index = 85 }, // Lunatone
        new(30,03,2) { Species = 338, Ability = A4, Moves = new(394, 129, 094, 088), Index = 85 }, // Solrock
        new(40,05,3) { Species = 337, Ability = A4, Moves = new(585, 129, 094, 157), Index = 85 }, // Lunatone
        new(40,05,3) { Species = 338, Ability = A4, Moves = new(394, 129, 094, 157), Index = 85 }, // Solrock
        new(50,08,4) { Species = 337, Ability = A4, Moves = new(585, 058, 094, 157), Index = 85 }, // Lunatone
        new(50,08,4) { Species = 338, Ability = A4, Moves = new(394, 076, 094, 157), Index = 85 }, // Solrock
        new(60,10,5) { Species = 337, Ability = A4, Moves = new(585, 058, 094, 444), Index = 85, Shiny = Shiny.Always }, // Lunatone
        new(60,10,5) { Species = 337, Ability = A4, Moves = new(585, 058, 094, 444), Index = 85 }, // Lunatone
        new(60,10,5) { Species = 338, Ability = A4, Moves = new(394, 076, 094, 444), Index = 85, Shiny = Shiny.Always }, // Solrock
        new(60,10,5) { Species = 338, Ability = A4, Moves = new(394, 076, 094, 444), Index = 85 }, // Solrock

        new(17,01,1) { Species = 573, Ability = A4, Moves = new(350, 541, 331, 001), Index = 83 }, // Cinccino
        new(17,01,1) { Species = 333, Ability = A4, Moves = new(574, 064, 257, 031), Index = 83 }, // Swablu
        new(17,01,1) { Species = 479, Ability = A4, Moves = new(437, 104, 310, 084), Index = 83, Form = 5 }, // Rotom-5
        new(17,01,1) { Species = 767, Ability = A4, Moves = new(522, 057, 111, 028), Index = 83 }, // Wimpod
        new(30,03,2) { Species = 573, Ability = A4, Moves = new(350, 541, 331, 583), Index = 83 }, // Cinccino
        new(30,03,2) { Species = 333, Ability = A4, Moves = new(583, 225, 257, 058), Index = 83 }, // Swablu
        new(30,03,2) { Species = 479, Ability = A4, Moves = new(437, 399, 310, 084), Index = 83, Form = 5 }, // Rotom-5
        new(30,03,2) { Species = 767, Ability = A4, Moves = new(522, 057, 111, 028), Index = 83 }, // Wimpod
        new(40,05,3) { Species = 573, Ability = A4, Moves = new(350, 541, 331, 441), Index = 83 }, // Cinccino
        new(40,05,3) { Species = 334, Ability = A4, Moves = new(583, 784, 083, 058), Index = 83 }, // Altaria
        new(40,05,3) { Species = 479, Ability = A4, Moves = new(437, 399, 506, 351), Index = 83, Form = 5 }, // Rotom-5
        new(40,05,3) { Species = 767, Ability = A4, Moves = new(522, 057, 372, 028), Index = 83 }, // Wimpod
        new(50,08,4) { Species = 573, Ability = A4, Moves = new(350, 541, 331, 086), Index = 83 }, // Cinccino
        new(50,08,4) { Species = 334, Ability = A4, Moves = new(585, 784, 083, 058), Index = 83 }, // Altaria
        new(50,08,4) { Species = 479, Ability = A4, Moves = new(437, 399, 247, 085), Index = 83, Form = 5 }, // Rotom-5
        new(50,08,4) { Species = 767, Ability = A4, Moves = new(522, 057, 372, 341), Index = 83 }, // Wimpod
        new(60,10,5) { Species = 573, Ability = A4, Moves = new(350, 541, 331, 813), Index = 83, Shiny = Shiny.Always }, // Cinccino
        new(60,10,5) { Species = 573, Ability = A4, Moves = new(350, 541, 331, 813), Index = 83 }, // Cinccino
        new(60,10,5) { Species = 334, Ability = A4, Moves = new(585, 784, 542, 058), Index = 83 }, // Altaria
        new(60,10,5) { Species = 479, Ability = A4, Moves = new(437, 399, 261, 085), Index = 83, Form = 5 }, // Rotom-5
        new(60,10,5) { Species = 767, Ability = A4, Moves = new(806, 057, 372, 341), Index = 83 }, // Wimpod

        new(17,01,1) { Species = 092, Ability = A4, Moves = new(122, 605, 474, 009), Index = 81 }, // Gastly
        new(17,01,1) { Species = 607, Ability = A4, Moves = new(052, 506, 123, 109), Index = 81 }, // Litwick
        new(17,01,1) { Species = 425, Ability = A4, Moves = new(310, 016, 107, 506), Index = 81 }, // Drifloon
        new(30,03,2) { Species = 093, Ability = A4, Moves = new(325, 605, 474, 009), Index = 81 }, // Haunter
        new(30,03,2) { Species = 607, Ability = A4, Moves = new(083, 506, 123, 261), Index = 81 }, // Litwick
        new(30,03,2) { Species = 426, Ability = A4, Moves = new(668, 016, 261, 506), Index = 81 }, // Drifblim
        new(40,05,3) { Species = 094, Ability = A4, Moves = new(325, 605, 474, 087), Index = 81 }, // Gengar
        new(40,05,3) { Species = 607, Ability = A4, Moves = new(517, 247, 123, 094), Index = 81 }, // Litwick
        new(40,05,3) { Species = 426, Ability = A4, Moves = new(668, 086, 261, 506), Index = 81 }, // Drifblim
        new(50,08,4) { Species = 094, Ability = A4, Moves = new(247, 605, 482, 087), Index = 81 }, // Gengar
        new(50,08,4) { Species = 609, Ability = A4, Moves = new(315, 247, 123, 094), Index = 81 }, // Chandelure
        new(50,08,4) { Species = 426, Ability = A4, Moves = new(668, 086, 261, 247), Index = 81 }, // Drifblim
        new(60,10,5) { Species = 094, Ability = A4, Moves = new(247, 605, 482, 261), Index = 81, CanGigantamax = true }, // Gengar
        new(60,10,5) { Species = 094, Ability = A4, Moves = new(247, 605, 482, 261), Index = 81 }, // Gengar
        new(60,10,5) { Species = 609, Ability = A4, Moves = new(315, 247, 399, 094), Index = 81, Shiny = Shiny.Always }, // Chandelure
        new(60,10,5) { Species = 609, Ability = A4, Moves = new(315, 247, 399, 094), Index = 81 }, // Chandelure
        new(60,10,5) { Species = 426, Ability = A4, Moves = new(668, 371, 261, 247), Index = 81 }, // Drifblim

        new(17,01,1) { Species = 582, Ability = A4, Moves = new(419, 106, 263, 310), Index = 79 }, // Vanillite
        new(17,01,1) { Species = 118, Ability = A4, Moves = new(030, 039, 352, 064), Index = 79 }, // Goldeen
        new(17,01,1) { Species = 127, Ability = A4, Moves = new(458, 693, 157, 069), Index = 79 }, // Pinsir
        new(17,01,1) { Species = 214, Ability = A4, Moves = new(280, 031, 089, 332), Index = 79 }, // Heracross
        new(17,01,1) { Species = 290, Ability = A4, Moves = new(189, 206, 028, 010), Index = 79 }, // Nincada
        new(17,01,1) { Species = 479, Ability = A4, Moves = new(403, 084, 310, 104), Index = 79, Form = 4 }, // Rotom-4
        new(30,03,2) { Species = 582, Ability = A4, Moves = new(419, 430, 263, 310), Index = 79 }, // Vanillite
        new(30,03,2) { Species = 118, Ability = A4, Moves = new(030, 398, 352, 064), Index = 79 }, // Goldeen
        new(30,03,2) { Species = 127, Ability = A4, Moves = new(458, 675, 157, 069), Index = 79 }, // Pinsir
        new(30,03,2) { Species = 214, Ability = A4, Moves = new(280, 030, 089, 332), Index = 79 }, // Heracross
        new(30,03,2) { Species = 291, Ability = A4, Moves = new(232, 210, 403, 010), Index = 79 }, // Ninjask
        new(30,03,2) { Species = 479, Ability = A4, Moves = new(403, 351, 310, 104), Index = 79, Form = 4 }, // Rotom-4
        new(40,05,3) { Species = 583, Ability = A4, Moves = new(419, 430, 304, 310), Index = 79 }, // Vanillish
        new(40,05,3) { Species = 119, Ability = A4, Moves = new(030, 224, 352, 529), Index = 79 }, // Seaking
        new(40,05,3) { Species = 127, Ability = A4, Moves = new(404, 675, 157, 280), Index = 79 }, // Pinsir
        new(40,05,3) { Species = 214, Ability = A4, Moves = new(280, 042, 089, 157), Index = 79 }, // Heracross
        new(40,05,3) { Species = 291, Ability = A4, Moves = new(232, 210, 403, 104), Index = 79 }, // Ninjask
        new(40,05,3) { Species = 479, Ability = A4, Moves = new(403, 085, 310, 399), Index = 79, Form = 4 }, // Rotom-4
        new(50,08,4) { Species = 584, Ability = A4, Moves = new(573, 430, 304, 058), Index = 79 }, // Vanilluxe
        new(50,08,4) { Species = 119, Ability = A4, Moves = new(030, 224, 503, 529), Index = 79 }, // Seaking
        new(50,08,4) { Species = 127, Ability = A4, Moves = new(404, 675, 157, 276), Index = 79 }, // Pinsir
        new(50,08,4) { Species = 214, Ability = A4, Moves = new(280, 331, 089, 157), Index = 79 }, // Heracross
        new(50,08,4) { Species = 291, Ability = A4, Moves = new(232, 210, 403, 163), Index = 79 }, // Ninjask
        new(50,08,4) { Species = 479, Ability = A4, Moves = new(403, 085, 506, 399), Index = 79, Form = 4 }, // Rotom-4
        new(60,10,5) { Species = 584, Ability = A4, Moves = new(573, 430, 304, 059), Index = 79, Shiny = Shiny.Always }, // Vanilluxe
        new(60,10,5) { Species = 119, Ability = A4, Moves = new(032, 224, 503, 529), Index = 79 }, // Seaking
        new(60,10,5) { Species = 127, Ability = A4, Moves = new(404, 675, 317, 276), Index = 79 }, // Pinsir
        new(60,10,5) { Species = 214, Ability = A4, Moves = new(370, 331, 089, 157), Index = 79 }, // Heracross
        new(60,10,5) { Species = 291, Ability = A4, Moves = new(232, 404, 403, 163), Index = 79 }, // Ninjask
        new(60,10,5) { Species = 479, Ability = A4, Moves = new(403, 085, 247, 399), Index = 79, Form = 4 }, // Rotom-4

        new(17,01,1) { Species = 138, Ability = A4, Moves = new(055, 028, 205, 020), Index = 77 }, // Omanyte
        new(17,01,1) { Species = 140, Ability = A4, Moves = new(453, 028, 263, 010), Index = 77 }, // Kabuto
        new(17,01,1) { Species = 142, Ability = A4, Moves = new(246, 414, 044, 017), Index = 77 }, // Aerodactyl
        new(30,03,2) { Species = 138, Ability = A4, Moves = new(055, 341, 246, 020), Index = 77 }, // Omanyte
        new(30,03,2) { Species = 140, Ability = A4, Moves = new(453, 246, 263, 058), Index = 77 }, // Kabuto
        new(30,03,2) { Species = 142, Ability = A4, Moves = new(157, 414, 242, 017), Index = 77 }, // Aerodactyl
        new(40,05,3) { Species = 138, Ability = A4, Moves = new(362, 414, 246, 196), Index = 77 }, // Omanyte
        new(40,05,3) { Species = 140, Ability = A4, Moves = new(362, 246, 263, 059), Index = 77 }, // Kabuto
        new(40,05,3) { Species = 142, Ability = A4, Moves = new(157, 414, 422, 017), Index = 77 }, // Aerodactyl
        new(50,08,4) { Species = 138, Ability = A4, Moves = new(057, 414, 246, 058), Index = 77 }, // Omanyte
        new(50,08,4) { Species = 140, Ability = A4, Moves = new(710, 246, 141, 059), Index = 77 }, // Kabuto
        new(50,08,4) { Species = 142, Ability = A4, Moves = new(444, 414, 422, 542), Index = 77 }, // Aerodactyl
        new(60,10,5) { Species = 138, Ability = A4, Moves = new(056, 414, 246, 058), Index = 77, Shiny = Shiny.Always }, // Omanyte
        new(60,10,5) { Species = 138, Ability = A4, Moves = new(056, 414, 246, 058), Index = 77 }, // Omanyte
        new(60,10,5) { Species = 140, Ability = A4, Moves = new(710, 444, 141, 059), Index = 77 }, // Kabuto
        new(60,10,5) { Species = 142, Ability = A4, Moves = new(444, 089, 422, 542), Index = 77 }, // Aerodactyl

        new(17,01,1) { Species = 878, Ability = A4, Moves = new(523, 249, 033, 045), Index = 75 }, // Cufant
        new(17,01,1) { Species = 109, Ability = A4, Moves = new(188, 372, 139, 108), Index = 75 }, // Koffing
        new(17,01,1) { Species = 202, Ability = A4, Moves = new(243, 227, 068, 204), Index = 75 }, // Wobbuffet
        new(17,01,1) { Species = 868, Ability = A4, Moves = new(033, 186, 577, 496), Index = 75 }, // Milcery
        new(17,01,1) { Species = 004, Ability = A4, Moves = new(052, 225, 010, 108), Index = 75 }, // Charmander
        new(30,03,2) { Species = 878, Ability = A4, Moves = new(523, 249, 023, 045), Index = 75 }, // Cufant
        new(30,03,2) { Species = 109, Ability = A4, Moves = new(188, 372, 123, 108), Index = 75 }, // Koffing
        new(30,03,2) { Species = 202, Ability = A4, Moves = new(243, 227, 068, 204), Index = 75 }, // Wobbuffet
        new(30,03,2) { Species = 868, Ability = A4, Moves = new(577, 186, 263, 500), Index = 75 }, // Milcery
        new(30,03,2) { Species = 005, Ability = A4, Moves = new(225, 512, 242, 053), Index = 75 }, // Charmeleon
        new(40,05,3) { Species = 879, Ability = A4, Moves = new(484, 583, 070, 249), Index = 75, CanGigantamax = true }, // Copperajah
        new(40,05,3) { Species = 109, Ability = A4, Moves = new(482, 372, 053, 085), Index = 75 }, // Koffing
        new(40,05,3) { Species = 202, Ability = A4, Moves = new(243, 227, 068, 204), Index = 75 }, // Wobbuffet
        new(40,05,3) { Species = 868, Ability = A4, Moves = new(577, 605, 496, 213), Index = 75 }, // Milcery
        new(40,05,3) { Species = 006, Ability = A4, Moves = new(337, 403, 280, 257), Index = 75, CanGigantamax = true }, // Charizard
        new(50,08,4) { Species = 879, Ability = A4, Moves = new(484, 583, 070, 438), Index = 75, CanGigantamax = true }, // Copperajah
        new(50,08,4) { Species = 109, Ability = A4, Moves = new(482, 399, 053, 085), Index = 75 }, // Koffing
        new(50,08,4) { Species = 202, Ability = A4, Moves = new(243, 227, 068, 204), Index = 75 }, // Wobbuffet
        new(50,08,4) { Species = 868, Ability = A4, Moves = new(577, 605, 496, 500), Index = 75 }, // Milcery
        new(50,08,4) { Species = 006, Ability = A4, Moves = new(337, 403, 411, 257), Index = 75, CanGigantamax = true }, // Charizard
        new(60,10,5) { Species = 879, Ability = A4, Moves = new(484, 583, 089, 438), Index = 75, CanGigantamax = true, Shiny = Shiny.Always }, // Copperajah
        new(60,10,5) { Species = 879, Ability = A4, Moves = new(484, 583, 089, 438), Index = 75, CanGigantamax = true }, // Copperajah
        new(60,10,5) { Species = 109, Ability = A4, Moves = new(482, 399, 053, 087), Index = 75 }, // Koffing
        new(60,10,5) { Species = 202, Ability = A4, Moves = new(243, 227, 068, 204), Index = 75 }, // Wobbuffet
        new(60,10,5) { Species = 868, Ability = A4, Moves = new(577, 605, 496, 500), Index = 75 }, // Milcery
        new(60,10,5) { Species = 006, Ability = A4, Moves = new(406, 403, 411, 257), Index = 75, CanGigantamax = true }, // Charizard

        new(17,01,1) { Species = 852, Ability = A4, Moves = new(371, 249, 362, 364), Index = 73 }, // Clobbopus
        new(17,01,1) { Species = 223, Ability = A4, Moves = new(055, 062, 060, 129), Index = 73 }, // Remoraid
        new(17,01,1) { Species = 686, Ability = A4, Moves = new(371, 060, 035, 095), Index = 73 }, // Inkay
        new(30,03,2) { Species = 852, Ability = A4, Moves = new(371, 066, 362, 364), Index = 73 }, // Clobbopus
        new(30,03,2) { Species = 223, Ability = A4, Moves = new(061, 062, 060, 129), Index = 73 }, // Remoraid
        new(30,03,2) { Species = 686, Ability = A4, Moves = new(400, 060, 035, 095), Index = 73 }, // Inkay
        new(40,05,3) { Species = 853, Ability = A4, Moves = new(693, 276, 190, 034), Index = 73 }, // Grapploct
        new(40,05,3) { Species = 224, Ability = A4, Moves = new(503, 058, 086, 129), Index = 73 }, // Octillery
        new(40,05,3) { Species = 687, Ability = A4, Moves = new(492, 060, 035, 095), Index = 73 }, // Malamar
        new(50,08,4) { Species = 853, Ability = A4, Moves = new(576, 276, 008, 034), Index = 73 }, // Grapploct
        new(50,08,4) { Species = 224, Ability = A4, Moves = new(056, 058, 086, 129), Index = 73 }, // Octillery
        new(50,08,4) { Species = 687, Ability = A4, Moves = new(492, 427, 163, 085), Index = 73 }, // Malamar
        new(60,10,5) { Species = 853, Ability = A4, Moves = new(576, 276, 008, 707), Index = 73, Shiny = Shiny.Always }, // Grapploct
        new(60,10,5) { Species = 853, Ability = A4, Moves = new(576, 276, 008, 707), Index = 73 }, // Grapploct
        new(60,10,5) { Species = 224, Ability = A4, Moves = new(056, 058, 086, 053), Index = 73 }, // Octillery
        new(60,10,5) { Species = 687, Ability = A4, Moves = new(492, 094, 157, 085), Index = 73 }, // Malamar

        new(17,01,1) { Species = 002, Ability = A4, Moves = new(075, 077, 033, 079), Index = 71 }, // Ivysaur
        new(17,01,1) { Species = 060, Ability = A4, Moves = new(055, 095, 001, 341), Index = 71 }, // Poliwag
        new(17,01,1) { Species = 453, Ability = A4, Moves = new(040, 279, 189, 372), Index = 71 }, // Croagunk
        new(17,01,1) { Species = 535, Ability = A4, Moves = new(497, 341, 045, 051), Index = 71 }, // Tympole
        new(30,03,2) { Species = 002, Ability = A4, Moves = new(402, 077, 033, 036), Index = 71 }, // Ivysaur
        new(30,03,2) { Species = 061, Ability = A4, Moves = new(061, 095, 001, 341), Index = 71 }, // Poliwhirl
        new(30,03,2) { Species = 453, Ability = A4, Moves = new(474, 279, 189, 372), Index = 71 }, // Croagunk
        new(30,03,2) { Species = 536, Ability = A4, Moves = new(061, 341, 175, 051), Index = 71 }, // Palpitoad
        new(40,05,3) { Species = 003, Ability = A4, Moves = new(402, 188, 414, 036), Index = 71 }, // Venusaur
        new(40,05,3) { Species = 186, Ability = A4, Moves = new(056, 411, 034, 341), Index = 71 }, // Politoed
        new(40,05,3) { Species = 453, Ability = A4, Moves = new(092, 279, 404, 372), Index = 71 }, // Croagunk
        new(40,05,3) { Species = 537, Ability = A4, Moves = new(503, 341, 438, 051), Index = 71 }, // Seismitoad
        new(50,08,4) { Species = 003, Ability = A4, Moves = new(438, 188, 414, 036), Index = 71 }, // Venusaur
        new(50,08,4) { Species = 186, Ability = A4, Moves = new(056, 411, 034, 414), Index = 71 }, // Politoed
        new(50,08,4) { Species = 453, Ability = A4, Moves = new(188, 067, 404, 372), Index = 71 }, // Croagunk
        new(50,08,4) { Species = 537, Ability = A4, Moves = new(503, 341, 438, 398), Index = 71 }, // Seismitoad
        new(60,10,5) { Species = 003, Ability = A4, Moves = new(438, 188, 414, 034), Index = 71, CanGigantamax = true }, // Venusaur
        new(60,10,5) { Species = 003, Ability = A4, Moves = new(438, 188, 414, 034), Index = 71 }, // Venusaur
        new(60,10,5) { Species = 186, Ability = A4, Moves = new(056, 311, 034, 414), Index = 71, Shiny = Shiny.Always }, // Politoed
        new(60,10,5) { Species = 186, Ability = A4, Moves = new(056, 311, 034, 414), Index = 71 }, // Politoed
        new(60,10,5) { Species = 453, Ability = A4, Moves = new(188, 067, 404, 247), Index = 71 }, // Croagunk
        new(60,10,5) { Species = 537, Ability = A4, Moves = new(503, 089, 438, 398), Index = 71 }, // Seismitoad

        new(17,01,1) { Species = 831, Ability = A4, Moves = new(029, 024, 045, 033), Index = 69 }, // Wooloo
        new(30,03,2) { Species = 831, Ability = A4, Moves = new(029, 024, 528, 033), Index = 69 }, // Wooloo
        new(30,03,2) { Species = 832, Ability = A4, Moves = new(036, 024, 528, 371), Index = 69 }, // Dubwool
        new(40,05,3) { Species = 831, Ability = A4, Moves = new(029, 179, 528, 024), Index = 69 }, // Wooloo
        new(40,05,3) { Species = 832, Ability = A4, Moves = new(036, 179, 528, 371), Index = 69 }, // Dubwool
        new(50,08,4) { Species = 831, Ability = A4, Moves = new(029, 179, 528, 371), Index = 69 }, // Wooloo
        new(50,08,4) { Species = 832, Ability = A4, Moves = new(036, 179, 528, 024), Index = 69 }, // Dubwool
        new(60,10,5) { Species = 831, Ability = A4, Moves = new(038, 179, 086, 371), Index = 69, Shiny = Shiny.Always }, // Wooloo
        new(60,10,5) { Species = 831, Ability = A4, Moves = new(038, 179, 086, 371), Index = 69 }, // Wooloo
        new(60,10,5) { Species = 832, Ability = A4, Moves = new(776, 038, 086, 371), Index = 69 }, // Dubwool

        new(17,01,1) { Species = 052, Ability = A4, Moves = new(006, 232, 442, 583), Index = 67, Form = 2 }, // Meowth-2
        new(17,01,1) { Species = 052, Ability = A4, Moves = new(006, 583, 196, 675), Index = 67, Form = 1 }, // Meowth-1
        new(17,01,1) { Species = 052, Ability = A4, Moves = new(006, 492, 402, 247), Index = 67 }, // Meowth
        new(17,01,1) { Species = 052, Ability = A4, Moves = new(006, 441, 087, 231), Index = 67 }, // Meowth
        new(30,03,2) { Species = 052, Ability = A4, Moves = new(006, 232, 442, 583), Index = 67, Form = 2 }, // Meowth-2
        new(30,03,2) { Species = 052, Ability = A4, Moves = new(006, 583, 196, 675), Index = 67, Form = 1 }, // Meowth-1
        new(30,03,2) { Species = 052, Ability = A4, Moves = new(006, 492, 402, 247), Index = 67 }, // Meowth
        new(30,03,2) { Species = 052, Ability = A4, Moves = new(006, 441, 087, 231), Index = 67 }, // Meowth
        new(40,05,3) { Species = 052, Ability = A4, Moves = new(006, 232, 442, 583), Index = 67, Form = 2 }, // Meowth-2
        new(40,05,3) { Species = 052, Ability = A4, Moves = new(006, 583, 196, 675), Index = 67, Form = 1 }, // Meowth-1
        new(40,05,3) { Species = 052, Ability = A4, Moves = new(006, 492, 402, 247), Index = 67 }, // Meowth
        new(40,05,3) { Species = 052, Ability = A4, Moves = new(006, 441, 087, 231), Index = 67 }, // Meowth
        new(50,08,4) { Species = 052, Ability = A4, Moves = new(006, 232, 442, 583), Index = 67, Form = 2 }, // Meowth-2
        new(50,08,4) { Species = 052, Ability = A4, Moves = new(006, 583, 196, 675), Index = 67, Form = 1 }, // Meowth-1
        new(50,08,4) { Species = 052, Ability = A4, Moves = new(006, 492, 402, 247), Index = 67 }, // Meowth
        new(50,08,4) { Species = 052, Ability = A4, Moves = new(006, 441, 087, 231), Index = 67 }, // Meowth
        new(60,10,5) { Species = 052, Ability = A4, Moves = new(006, 232, 442, 583), Index = 67, Form = 2, Shiny = Shiny.Always }, // Meowth-2
        new(60,10,5) { Species = 052, Ability = A4, Moves = new(006, 232, 442, 583), Index = 67, Form = 2 }, // Meowth-2
        new(60,10,5) { Species = 052, Ability = A4, Moves = new(006, 583, 196, 675), Index = 67, Form = 1 }, // Meowth-1
        new(60,10,5) { Species = 052, Ability = A4, Moves = new(006, 492, 402, 247), Index = 67 }, // Meowth
        new(60,10,5) { Species = 052, Ability = A4, Moves = new(006, 441, 087, 231), Index = 67 }, // Meowth
        new(60,10,5) { Species = 052, Ability = A4, Moves = new(006, 583, 421, 034), Index = 67, CanGigantamax = true }, // Meowth

        new(17,01,1) { Species = 875, Ability = A4, Moves = new(181, 362, 033, 311), Index = 65 }, // Eiscue
        new(30,03,2) { Species = 875, Ability = A4, Moves = new(196, 362, 033, 029), Index = 65 }, // Eiscue
        new(40,05,3) { Species = 875, Ability = A4, Moves = new(008, 057, 263, 029), Index = 65 }, // Eiscue
        new(50,08,4) { Species = 875, Ability = A4, Moves = new(333, 057, 428, 029), Index = 65 }, // Eiscue
        new(60,10,5) { Species = 875, Ability = A4, Moves = new(333, 710, 442, 029), Index = 65, Shiny = Shiny.Always }, // Eiscue
        new(60,10,5) { Species = 875, Ability = A4, Moves = new(333, 710, 442, 029), Index = 65 }, // Eiscue

        new(17,01,1) { Species = 132, Ability = A4, Moves = new(144, 000, 000, 000), Index = 64 }, // Ditto
        new(17,01,1) { Species = 821, Ability = A4, Moves = new(403, 031, 043, 681), Index = 64 }, // Rookidee
        new(17,01,1) { Species = 833, Ability = A4, Moves = new(055, 033, 044, 240), Index = 64 }, // Chewtle
        new(17,01,1) { Species = 824, Ability = A4, Moves = new(522, 000, 000, 000), Index = 64 }, // Blipbug
        new(17,01,1) { Species = 850, Ability = A4, Moves = new(172, 044, 035, 052), Index = 64 }, // Sizzlipede
        new(17,01,1) { Species = 831, Ability = A4, Moves = new(033, 024, 029, 045), Index = 64 }, // Wooloo
        new(30,03,2) { Species = 132, Ability = A4, Moves = new(144, 000, 000, 000), Index = 64 }, // Ditto
        new(30,03,2) { Species = 822, Ability = A4, Moves = new(403, 263, 279, 681), Index = 64 }, // Corvisquire
        new(30,03,2) { Species = 833, Ability = A4, Moves = new(055, 033, 044, 029), Index = 64 }, // Chewtle
        new(30,03,2) { Species = 825, Ability = A4, Moves = new(522, 263, 371, 247), Index = 64 }, // Dottler
        new(30,03,2) { Species = 850, Ability = A4, Moves = new(172, 044, 035, 052), Index = 64 }, // Sizzlipede
        new(30,03,2) { Species = 831, Ability = A4, Moves = new(036, 024, 029, 086), Index = 64 }, // Wooloo
        new(40,05,3) { Species = 132, Ability = A4, Moves = new(144, 000, 000, 000), Index = 64 }, // Ditto
        new(40,05,3) { Species = 823, Ability = A4, Moves = new(403, 442, 034, 681), Index = 64 }, // Corviknight
        new(40,05,3) { Species = 834, Ability = A4, Moves = new(157, 534, 317, 055), Index = 64 }, // Drednaw
        new(40,05,3) { Species = 826, Ability = A4, Moves = new(405, 277, 371, 247), Index = 64 }, // Orbeetle
        new(40,05,3) { Species = 851, Ability = A4, Moves = new(424, 404, 422, 044), Index = 64 }, // Centiskorch
        new(40,05,3) { Species = 832, Ability = A4, Moves = new(036, 024, 428, 086), Index = 64 }, // Dubwool
        new(50,08,4) { Species = 132, Ability = A4, Moves = new(144, 000, 000, 000), Index = 64 }, // Ditto
        new(50,08,4) { Species = 823, Ability = A4, Moves = new(413, 442, 034, 681), Index = 64 }, // Corviknight
        new(50,08,4) { Species = 834, Ability = A4, Moves = new(157, 710, 317, 334), Index = 64 }, // Drednaw
        new(50,08,4) { Species = 826, Ability = A4, Moves = new(405, 277, 776, 247), Index = 64 }, // Orbeetle
        new(50,08,4) { Species = 851, Ability = A4, Moves = new(680, 404, 422, 044), Index = 64 }, // Centiskorch
        new(50,08,4) { Species = 832, Ability = A4, Moves = new(038, 024, 428, 086), Index = 64 }, // Dubwool
        new(60,10,5) { Species = 132, Ability = A4, Moves = new(144, 000, 000, 000), Index = 64 }, // Ditto
        new(60,10,5) { Species = 823, Ability = A4, Moves = new(413, 442, 776, 372), Index = 64 }, // Corviknight
        new(60,10,5) { Species = 834, Ability = A4, Moves = new(157, 710, 317, 334), Index = 64 }, // Drednaw
        new(60,10,5) { Species = 826, Ability = A4, Moves = new(405, 277, 776, 412), Index = 64 }, // Orbeetle
        new(60,10,5) { Species = 851, Ability = A4, Moves = new(680, 679, 422, 044), Index = 64 }, // Centiskorch
        new(60,10,5) { Species = 832, Ability = A4, Moves = new(038, 776, 428, 086), Index = 64 }, // Dubwool

        new(17,01,1) { Species = 183, Ability = A4, Moves = new(061, 204, 111, 205), Index = 63 }, // Marill
        new(17,01,1) { Species = 427, Ability = A4, Moves = new(098, 608, 150, 111), Index = 63 }, // Buneary
        new(17,01,1) { Species = 659, Ability = A4, Moves = new(098, 189, 280, 341), Index = 63 }, // Bunnelby
        new(30,03,2) { Species = 184, Ability = A4, Moves = new(401, 583, 280, 205), Index = 63 }, // Azumarill
        new(30,03,2) { Species = 428, Ability = A4, Moves = new(024, 204, 029, 111), Index = 63 }, // Lopunny
        new(30,03,2) { Species = 660, Ability = A4, Moves = new(098, 523, 280, 341), Index = 63 }, // Diggersby
        new(40,05,3) { Species = 184, Ability = A4, Moves = new(056, 583, 280, 205), Index = 63 }, // Azumarill
        new(40,05,3) { Species = 428, Ability = A4, Moves = new(024, 204, 029, 129), Index = 63 }, // Lopunny
        new(40,05,3) { Species = 660, Ability = A4, Moves = new(005, 523, 280, 036), Index = 63 }, // Diggersby
        new(50,08,4) { Species = 184, Ability = A4, Moves = new(710, 583, 276, 205), Index = 63 }, // Azumarill
        new(50,08,4) { Species = 428, Ability = A4, Moves = new(024, 583, 025, 129), Index = 63 }, // Lopunny
        new(50,08,4) { Species = 660, Ability = A4, Moves = new(005, 089, 280, 162), Index = 63 }, // Diggersby
        new(60,10,5) { Species = 184, Ability = A4, Moves = new(710, 583, 276, 523), Index = 63 }, // Azumarill
        new(60,10,5) { Species = 184, Ability = A4, Moves = new(710, 583, 276, 523), Index = 63, Shiny = Shiny.Always }, // Azumarill
        new(60,10,5) { Species = 428, Ability = A4, Moves = new(136, 583, 025, 693), Index = 63 }, // Lopunny
        new(60,10,5) { Species = 660, Ability = A4, Moves = new(416, 089, 359, 162), Index = 63 }, // Diggersby
        //new(60,10,5) { Species = 815, Ability = A2, Moves = new(780, 442, 279, 555), Index = 63, CanGigantamax = true }, // Cinderace

        //new(17,01,1) { Species = 129, Ability = A4, Moves = new(150, 000, 000, 000), Index = 62 }, // Magikarp
        //new(30,03,2) { Species = 129, Ability = A4, Moves = new(150, 000, 000, 000), Index = 62 }, // Magikarp
        //new(40,05,3) { Species = 129, Ability = A4, Moves = new(150, 000, 000, 000), Index = 62 }, // Magikarp
        //new(50,08,4) { Species = 129, Ability = A4, Moves = new(150, 000, 000, 000), Index = 62 }, // Magikarp
        //new(60,10,5) { Species = 129, Ability = A4, Moves = new(150, 000, 000, 000), Index = 62 }, // Magikarp

        new(17,01,1) { Species = 043, Ability = A4, Moves = new(331, 236, 051, 074), Index = 60 }, // Oddish
        new(17,01,1) { Species = 420, Ability = A4, Moves = new(234, 572, 670, 033), Index = 60 }, // Cherubi
        new(17,01,1) { Species = 549, Ability = A4, Moves = new(412, 298, 345, 263), Index = 60 }, // Lilligant
        new(17,01,1) { Species = 753, Ability = A4, Moves = new(210, 074, 075, 275), Index = 60 }, // Fomantis
        new(17,01,1) { Species = 764, Ability = A4, Moves = new(345, 579, 035, 074), Index = 60 }, // Comfey
        new(30,03,2) { Species = 045, Ability = A4, Moves = new(572, 236, 051, 496), Index = 60 }, // Vileplume
        new(30,03,2) { Species = 182, Ability = A4, Moves = new(572, 483, 074, 605), Index = 60 }, // Bellossom
        new(30,03,2) { Species = 421, Ability = A4, Moves = new(579, 345, 670, 033), Index = 60 }, // Cherrim
        new(30,03,2) { Species = 549, Ability = A4, Moves = new(412, 298, 345, 263), Index = 60 }, // Lilligant
        new(30,03,2) { Species = 753, Ability = A4, Moves = new(210, 163, 075, 230), Index = 60 }, // Fomantis
        new(30,03,2) { Species = 764, Ability = A4, Moves = new(345, 579, 035, 583), Index = 60 }, // Comfey
        new(40,05,3) { Species = 045, Ability = A4, Moves = new(572, 585, 051, 496), Index = 60 }, // Vileplume
        new(40,05,3) { Species = 182, Ability = A4, Moves = new(572, 483, 077, 605), Index = 60 }, // Bellossom
        new(40,05,3) { Species = 421, Ability = A4, Moves = new(579, 345, 583, 036), Index = 60 }, // Cherrim
        new(40,05,3) { Species = 549, Ability = A4, Moves = new(572, 298, 345, 483), Index = 60 }, // Lilligant
        new(40,05,3) { Species = 754, Ability = A4, Moves = new(210, 572, 400, 530), Index = 60 }, // Lurantis
        new(40,05,3) { Species = 764, Ability = A4, Moves = new(572, 579, 035, 583), Index = 60 }, // Comfey
        new(50,08,4) { Species = 045, Ability = A4, Moves = new(572, 585, 092, 496), Index = 60 }, // Vileplume
        new(50,08,4) { Species = 182, Ability = A4, Moves = new(080, 483, 051, 605), Index = 60 }, // Bellossom
        new(50,08,4) { Species = 421, Ability = A4, Moves = new(579, 572, 583, 676), Index = 60 }, // Cherrim
        new(50,08,4) { Species = 549, Ability = A4, Moves = new(572, 298, 241, 676), Index = 60 }, // Lilligant
        new(50,08,4) { Species = 754, Ability = A4, Moves = new(404, 572, 400, 530), Index = 60 }, // Lurantis
        new(50,08,4) { Species = 764, Ability = A4, Moves = new(572, 579, 035, 583), Index = 60 }, // Comfey
        new(60,10,5) { Species = 045, Ability = A4, Moves = new(572, 585, 092, 034), Index = 60 }, // Vileplume
        new(60,10,5) { Species = 549, Ability = A4, Moves = new(080, 298, 241, 676), Index = 60, Shiny = Shiny.Always }, // Lilligant
        new(60,10,5) { Species = 421, Ability = A4, Moves = new(579, 572, 605, 676), Index = 60 }, // Cherrim
        new(60,10,5) { Species = 549, Ability = A4, Moves = new(080, 298, 241, 676), Index = 60 }, // Lilligant
        new(60,10,5) { Species = 754, Ability = A4, Moves = new(404, 572, 398, 530), Index = 60 }, // Lurantis
        new(60,10,5) { Species = 764, Ability = A4, Moves = new(572, 579, 447, 583), Index = 60 }, // Comfey

        new(17,01,1) { Species = 856, Ability = A4, Moves = new(312, 093, 574, 595), Index = 59 }, // Hatenna
        new(17,01,1) { Species = 280, Ability = A4, Moves = new(574, 093, 104, 045), Index = 59 }, // Ralts
        new(17,01,1) { Species = 109, Ability = A4, Moves = new(499, 053, 124, 372), Index = 59 }, // Koffing
        new(17,01,1) { Species = 821, Ability = A4, Moves = new(403, 031, 043, 681), Index = 59 }, // Rookidee
        new(17,01,1) { Species = 627, Ability = A4, Moves = new(043, 276, 017, 064), Index = 59 }, // Rufflet
        new(17,01,1) { Species = 845, Ability = A4, Moves = new(055, 254, 064, 562), Index = 59 }, // Cramorant
        new(30,03,2) { Species = 856, Ability = A4, Moves = new(605, 060, 574, 595), Index = 59 }, // Hatenna
        new(30,03,2) { Species = 281, Ability = A4, Moves = new(574, 060, 104, 085), Index = 59 }, // Kirlia
        new(30,03,2) { Species = 109, Ability = A4, Moves = new(499, 053, 482, 372), Index = 59 }, // Koffing
        new(30,03,2) { Species = 822, Ability = A4, Moves = new(403, 263, 279, 681), Index = 59 }, // Corvisquire
        new(30,03,2) { Species = 627, Ability = A4, Moves = new(184, 276, 017, 157), Index = 59 }, // Rufflet
        new(30,03,2) { Species = 845, Ability = A4, Moves = new(055, 058, 064, 562), Index = 59 }, // Cramorant
        new(40,05,3) { Species = 857, Ability = A4, Moves = new(605, 060, 595, 574), Index = 59, CanGigantamax = true }, // Hattrem
        new(40,05,3) { Species = 282, Ability = A4, Moves = new(585, 060, 595, 085), Index = 59 }, // Gardevoir
        new(40,05,3) { Species = 110, Ability = A4, Moves = new(790, 053, 482, 372), Index = 59, Form = 1 }, // Weezing-1
        new(40,05,3) { Species = 823, Ability = A4, Moves = new(403, 442, 034, 681), Index = 59, CanGigantamax = true }, // Corviknight
        new(40,05,3) { Species = 628, Ability = A4, Moves = new(403, 276, 163, 157), Index = 59 }, // Braviary
        new(40,05,3) { Species = 845, Ability = A4, Moves = new(503, 058, 403, 065), Index = 59 }, // Cramorant
        new(50,08,4) { Species = 858, Ability = A4, Moves = new(605, 094, 595, 247), Index = 59, CanGigantamax = true }, // Hatterene
        new(50,08,4) { Species = 282, Ability = A4, Moves = new(585, 094, 595, 085), Index = 59 }, // Gardevoir
        new(50,08,4) { Species = 110, Ability = A4, Moves = new(790, 126, 482, 372), Index = 59, Form = 1 }, // Weezing-1
        new(50,08,4) { Species = 823, Ability = A4, Moves = new(413, 442, 034, 681), Index = 59, CanGigantamax = true }, // Corviknight
        new(50,08,4) { Species = 628, Ability = A4, Moves = new(403, 276, 442, 157), Index = 59 }, // Braviary
        new(50,08,4) { Species = 845, Ability = A4, Moves = new(056, 058, 403, 065), Index = 59 }, // Cramorant
        new(60,10,5) { Species = 858, Ability = A4, Moves = new(605, 094, 438, 247), Index = 59, CanGigantamax = true }, // Hatterene
        new(60,10,5) { Species = 282, Ability = A4, Moves = new(585, 094, 261, 085), Index = 59 }, // Gardevoir
        new(60,10,5) { Species = 110, Ability = A4, Moves = new(790, 126, 482, 399), Index = 59, Form = 1 }, // Weezing-1
        new(60,10,5) { Species = 823, Ability = A4, Moves = new(413, 442, 776, 372), Index = 59, CanGigantamax = true }, // Corviknight
        new(60,10,5) { Species = 628, Ability = A4, Moves = new(413, 370, 442, 157), Index = 59 }, // Braviary
        new(60,10,5) { Species = 845, Ability = A4, Moves = new(056, 058, 403, 057), Index = 59 }, // Cramorant

        new(17,01,1) { Species = 067, Ability = A4, Moves = new(279, 009, 490, 067), Index = 58 }, // Machoke
        new(17,01,1) { Species = 447, Ability = A4, Moves = new(280, 098, 317, 523), Index = 58 }, // Riolu
        new(17,01,1) { Species = 870, Ability = A4, Moves = new(442, 029, 249, 157), Index = 58 }, // Falinks
        new(17,01,1) { Species = 825, Ability = A4, Moves = new(522, 263, 371, 247), Index = 58 }, // Dottler
        new(17,01,1) { Species = 577, Ability = A4, Moves = new(060, 086, 360, 283), Index = 58 }, // Solosis
        new(17,01,1) { Species = 574, Ability = A4, Moves = new(060, 086, 412, 157), Index = 58 }, // Gothita
        new(30,03,2) { Species = 068, Ability = A4, Moves = new(279, 009, 233, 372), Index = 58, CanGigantamax = true }, // Machamp
        new(30,03,2) { Species = 447, Ability = A4, Moves = new(280, 232, 317, 523), Index = 58 }, // Riolu
        new(30,03,2) { Species = 870, Ability = A4, Moves = new(442, 029, 179, 157), Index = 58 }, // Falinks
        new(30,03,2) { Species = 826, Ability = A4, Moves = new(405, 277, 371, 109), Index = 58, CanGigantamax = true }, // Orbeetle
        new(30,03,2) { Species = 577, Ability = A4, Moves = new(473, 086, 360, 283), Index = 58 }, // Solosis
        new(30,03,2) { Species = 574, Ability = A4, Moves = new(473, 086, 412, 157), Index = 58 }, // Gothita
        new(40,05,3) { Species = 068, Ability = A4, Moves = new(530, 009, 233, 372), Index = 58, CanGigantamax = true }, // Machamp
        new(40,05,3) { Species = 448, Ability = A4, Moves = new(612, 232, 444, 089), Index = 58 }, // Lucario
        new(40,05,3) { Species = 870, Ability = A4, Moves = new(442, 029, 179, 317), Index = 58 }, // Falinks
        new(40,05,3) { Species = 826, Ability = A4, Moves = new(405, 277, 371, 247), Index = 58, CanGigantamax = true }, // Orbeetle
        new(40,05,3) { Species = 578, Ability = A4, Moves = new(094, 086, 360, 247), Index = 58 }, // Duosion
        new(40,05,3) { Species = 575, Ability = A4, Moves = new(094, 085, 412, 157), Index = 58 }, // Gothorita
        new(50,08,4) { Species = 068, Ability = A4, Moves = new(223, 009, 370, 372), Index = 58, CanGigantamax = true }, // Machamp
        new(50,08,4) { Species = 448, Ability = A4, Moves = new(612, 309, 444, 089), Index = 58 }, // Lucario
        new(50,08,4) { Species = 870, Ability = A4, Moves = new(442, 224, 179, 317), Index = 58 }, // Falinks
        new(50,08,4) { Species = 826, Ability = A4, Moves = new(405, 277, 776, 247), Index = 58, CanGigantamax = true }, // Orbeetle
        new(50,08,4) { Species = 579, Ability = A4, Moves = new(094, 009, 411, 247), Index = 58 }, // Reuniclus
        new(50,08,4) { Species = 576, Ability = A4, Moves = new(094, 085, 412, 322), Index = 58 }, // Gothitelle
        new(60,10,5) { Species = 025, Ability = A4, Moves = new(804, 435, 057, 574), Index = 58, CanGigantamax = true, Shiny = Shiny.Always }, // Pikachu
        new(60,10,5) { Species = 025, Ability = A4, Moves = new(344, 280, 583, 231), Index = 58, CanGigantamax = true, Shiny = Shiny.Always }, // Pikachu
        new(60,10,5) { Species = 025, Ability = A4, Moves = new(804, 435, 057, 574), Index = 58, CanGigantamax = true }, // Pikachu
        new(60,10,5) { Species = 025, Ability = A4, Moves = new(344, 280, 583, 231), Index = 58, CanGigantamax = true }, // Pikachu

        new(17,01,1) { Species = 868, Ability = A4, Moves = new(033, 186, 577, 496), Index = 56, CanGigantamax = true }, // Milcery
        new(30,03,2) { Species = 868, Ability = A4, Moves = new(577, 186, 263, 500), Index = 56, CanGigantamax = true }, // Milcery
        new(40,05,3) { Species = 868, Ability = A4, Moves = new(577, 605, 496, 213), Index = 56, CanGigantamax = true }, // Milcery
        new(50,08,4) { Species = 868, Ability = A4, Moves = new(577, 605, 496, 500), Index = 56, CanGigantamax = true }, // Milcery
        new(60,10,5) { Species = 868, Ability = A4, Moves = new(577, 605, 496, 500), Index = 56, CanGigantamax = true, Shiny = Shiny.Always }, // Milcery
        new(60,10,5) { Species = 868, Ability = A4, Moves = new(577, 605, 496, 500), Index = 56, CanGigantamax = true }, // Milcery

        new(17,01,1) { Species = 845, Ability = A4, Moves = new(057, 056, 503, 000), Index = 54 }, // Cramorant
        new(17,01,1) { Species = 330, Ability = A4, Moves = new(225, 129, 693, 048), Index = 54 }, // Flygon
        new(17,01,1) { Species = 623, Ability = A4, Moves = new(089, 157, 707, 009), Index = 54 }, // Golurk
        new(17,01,1) { Species = 195, Ability = A4, Moves = new(055, 157, 281, 034), Index = 54 }, // Quagsire
        new(17,01,1) { Species = 876, Ability = A4, Moves = new(574, 129, 304, 000), Index = 54, Form = 1 }, // Indeedee-1
        new(30,03,2) { Species = 845, Ability = A4, Moves = new(057, 056, 503, 000), Index = 54 }, // Cramorant
        new(30,03,2) { Species = 330, Ability = A4, Moves = new(225, 129, 693, 048), Index = 54 }, // Flygon
        new(30,03,2) { Species = 623, Ability = A4, Moves = new(089, 157, 707, 009), Index = 54 }, // Golurk
        new(30,03,2) { Species = 195, Ability = A4, Moves = new(055, 157, 281, 034), Index = 54 }, // Quagsire
        new(30,03,2) { Species = 876, Ability = A4, Moves = new(574, 129, 304, 000), Index = 54, Form = 1 }, // Indeedee-1
        new(40,05,3) { Species = 845, Ability = A4, Moves = new(057, 056, 503, 000), Index = 54 }, // Cramorant
        new(40,05,3) { Species = 330, Ability = A4, Moves = new(225, 129, 693, 586), Index = 54 }, // Flygon
        new(40,05,3) { Species = 623, Ability = A4, Moves = new(089, 157, 523, 009), Index = 54 }, // Golurk
        new(40,05,3) { Species = 195, Ability = A4, Moves = new(330, 157, 281, 034), Index = 54 }, // Quagsire
        new(40,05,3) { Species = 876, Ability = A4, Moves = new(574, 129, 304, 000), Index = 54, Form = 1 }, // Indeedee-1
        new(50,08,4) { Species = 845, Ability = A4, Moves = new(057, 056, 503, 000), Index = 54 }, // Cramorant
        new(50,08,4) { Species = 330, Ability = A4, Moves = new(225, 129, 693, 586), Index = 54 }, // Flygon
        new(50,08,4) { Species = 623, Ability = A4, Moves = new(089, 157, 523, 009), Index = 54 }, // Golurk
        new(50,08,4) { Species = 195, Ability = A4, Moves = new(330, 157, 281, 034), Index = 54 }, // Quagsire
        new(50,08,4) { Species = 876, Ability = A4, Moves = new(574, 129, 304, 000), Index = 54, Form = 1 }, // Indeedee-1
        new(60,10,5) { Species = 845, Ability = A4, Moves = new(057, 056, 503, 000), Index = 54 }, // Cramorant
        new(60,10,5) { Species = 845, Ability = A4, Moves = new(057, 056, 503, 000), Index = 54, Shiny = Shiny.Always }, // Cramorant
        new(60,10,5) { Species = 330, Ability = A4, Moves = new(225, 129, 693, 586), Index = 54 }, // Flygon
        new(60,10,5) { Species = 623, Ability = A4, Moves = new(089, 157, 523, 009), Index = 54 }, // Golurk
        new(60,10,5) { Species = 195, Ability = A4, Moves = new(330, 157, 281, 034), Index = 54 }, // Quagsire
        new(60,10,5) { Species = 876, Ability = A4, Moves = new(574, 129, 304, 000), Index = 54, Form = 1 }, // Indeedee-1

        new(17,01,1) { Species = 067, Ability = A4, Moves = new(279, 009, 490, 067), Index = 53 }, // Machoke
        new(17,01,1) { Species = 447, Ability = A4, Moves = new(280, 098, 317, 523), Index = 53 }, // Riolu
        new(17,01,1) { Species = 870, Ability = A4, Moves = new(442, 029, 249, 157), Index = 53 }, // Falinks
        new(17,01,1) { Species = 825, Ability = A4, Moves = new(522, 263, 371, 247), Index = 53 }, // Dottler
        new(17,01,1) { Species = 577, Ability = A4, Moves = new(060, 086, 360, 283), Index = 53 }, // Solosis
        new(17,01,1) { Species = 574, Ability = A4, Moves = new(060, 086, 412, 157), Index = 53 }, // Gothita
        new(30,03,2) { Species = 068, Ability = A4, Moves = new(279, 009, 233, 372), Index = 53, CanGigantamax = true }, // Machamp
        new(30,03,2) { Species = 447, Ability = A4, Moves = new(280, 232, 317, 523), Index = 53 }, // Riolu
        new(30,03,2) { Species = 870, Ability = A4, Moves = new(442, 029, 179, 157), Index = 53 }, // Falinks
        new(30,03,2) { Species = 826, Ability = A4, Moves = new(405, 277, 371, 109), Index = 53, CanGigantamax = true }, // Orbeetle
        new(30,03,2) { Species = 577, Ability = A4, Moves = new(473, 086, 360, 283), Index = 53 }, // Solosis
        new(30,03,2) { Species = 574, Ability = A4, Moves = new(473, 086, 412, 157), Index = 53 }, // Gothita
        new(40,05,3) { Species = 068, Ability = A4, Moves = new(530, 009, 233, 372), Index = 53, CanGigantamax = true }, // Machamp
        new(40,05,3) { Species = 448, Ability = A4, Moves = new(612, 232, 444, 089), Index = 53 }, // Lucario
        new(40,05,3) { Species = 870, Ability = A4, Moves = new(442, 029, 179, 317), Index = 53 }, // Falinks
        new(40,05,3) { Species = 826, Ability = A4, Moves = new(405, 277, 371, 247), Index = 53, CanGigantamax = true }, // Orbeetle
        new(40,05,3) { Species = 578, Ability = A4, Moves = new(094, 086, 360, 247), Index = 53 }, // Duosion
        new(40,05,3) { Species = 575, Ability = A4, Moves = new(094, 085, 412, 157), Index = 53 }, // Gothorita
        new(50,08,4) { Species = 068, Ability = A4, Moves = new(223, 009, 370, 372), Index = 53, CanGigantamax = true }, // Machamp
        new(50,08,4) { Species = 448, Ability = A4, Moves = new(612, 309, 444, 089), Index = 53 }, // Lucario
        new(50,08,4) { Species = 870, Ability = A4, Moves = new(442, 224, 179, 317), Index = 53 }, // Falinks
        new(50,08,4) { Species = 826, Ability = A4, Moves = new(405, 277, 776, 247), Index = 53, CanGigantamax = true }, // Orbeetle
        new(50,08,4) { Species = 579, Ability = A4, Moves = new(094, 009, 411, 247), Index = 53 }, // Reuniclus
        new(50,08,4) { Species = 576, Ability = A4, Moves = new(094, 085, 412, 322), Index = 53 }, // Gothitelle
        new(60,10,5) { Species = 068, Ability = A4, Moves = new(223, 009, 523, 372), Index = 53, CanGigantamax = true }, // Machamp
        new(60,10,5) { Species = 448, Ability = A4, Moves = new(370, 309, 444, 089), Index = 53 }, // Lucario
        new(60,10,5) { Species = 870, Ability = A4, Moves = new(442, 224, 370, 317), Index = 53 }, // Falinks
        new(60,10,5) { Species = 826, Ability = A4, Moves = new(405, 277, 776, 412), Index = 53, CanGigantamax = true }, // Orbeetle
        new(60,10,5) { Species = 579, Ability = A4, Moves = new(094, 087, 411, 247), Index = 53 }, // Reuniclus
        new(60,10,5) { Species = 576, Ability = A4, Moves = new(094, 085, 412, 322), Index = 53 }, // Gothitelle

        new(17,01,1) { Species = 128, Ability = A4, Moves = new(033, 157, 030, 371), Index = 51 }, // Tauros
        new(17,01,1) { Species = 626, Ability = A4, Moves = new(033, 030, 031, 523), Index = 51 }, // Bouffalant
        new(17,01,1) { Species = 241, Ability = A4, Moves = new(707, 033, 023, 205), Index = 51 }, // Miltank
        new(30,03,2) { Species = 128, Ability = A4, Moves = new(033, 157, 030, 370), Index = 51 }, // Tauros
        new(30,03,2) { Species = 626, Ability = A4, Moves = new(279, 030, 675, 523), Index = 51 }, // Bouffalant
        new(30,03,2) { Species = 241, Ability = A4, Moves = new(707, 428, 023, 205), Index = 51 }, // Miltank
        new(40,05,3) { Species = 128, Ability = A4, Moves = new(036, 157, 030, 370), Index = 51 }, // Tauros
        new(40,05,3) { Species = 626, Ability = A4, Moves = new(279, 543, 675, 523), Index = 51 }, // Bouffalant
        new(40,05,3) { Species = 241, Ability = A4, Moves = new(707, 428, 034, 205), Index = 51 }, // Miltank
        new(50,08,4) { Species = 128, Ability = A4, Moves = new(034, 157, 030, 370), Index = 51 }, // Tauros
        new(50,08,4) { Species = 626, Ability = A4, Moves = new(224, 543, 675, 523), Index = 51 }, // Bouffalant
        new(50,08,4) { Species = 241, Ability = A4, Moves = new(707, 428, 034, 583), Index = 51 }, // Miltank
        new(60,10,5) { Species = 128, Ability = A4, Moves = new(034, 157, 372, 370), Index = 51 }, // Tauros
        new(60,10,5) { Species = 128, Ability = A4, Moves = new(034, 157, 372, 370), Index = 51, Shiny = Shiny.Always }, // Tauros
        new(60,10,5) { Species = 626, Ability = A4, Moves = new(224, 543, 675, 089), Index = 51 }, // Bouffalant
        new(60,10,5) { Species = 241, Ability = A4, Moves = new(667, 428, 034, 583), Index = 51 }, // Miltank

        new(17,01,1) { Species = 420, Ability = A4, Moves = new(033, 572, 074, 670), Index = 49 }, // Cherubi
        new(17,01,1) { Species = 590, Ability = A4, Moves = new(078, 492, 310, 412), Index = 49 }, // Foongus
        new(17,01,1) { Species = 755, Ability = A4, Moves = new(402, 109, 605, 310), Index = 49 }, // Morelull
        new(17,01,1) { Species = 819, Ability = A4, Moves = new(747, 231, 371, 033), Index = 49 }, // Skwovet
        new(30,03,2) { Species = 420, Ability = A4, Moves = new(033, 572, 074, 402), Index = 49 }, // Cherubi
        new(30,03,2) { Species = 590, Ability = A4, Moves = new(078, 492, 310, 412), Index = 49 }, // Foongus
        new(30,03,2) { Species = 756, Ability = A4, Moves = new(402, 109, 605, 310), Index = 49 }, // Shiinotic
        new(30,03,2) { Species = 819, Ability = A4, Moves = new(747, 231, 371, 033), Index = 49 }, // Skwovet
        new(30,03,2) { Species = 820, Ability = A4, Moves = new(747, 231, 371, 034), Index = 49 }, // Greedent
        new(40,05,3) { Species = 420, Ability = A4, Moves = new(311, 572, 074, 402), Index = 49 }, // Cherubi
        new(40,05,3) { Species = 591, Ability = A4, Moves = new(078, 492, 092, 412), Index = 49 }, // Amoonguss
        new(40,05,3) { Species = 756, Ability = A4, Moves = new(402, 585, 605, 310), Index = 49 }, // Shiinotic
        new(40,05,3) { Species = 819, Ability = A4, Moves = new(747, 360, 371, 044), Index = 49 }, // Skwovet
        new(40,05,3) { Species = 820, Ability = A4, Moves = new(747, 360, 371, 424), Index = 49 }, // Greedent
        new(50,08,4) { Species = 420, Ability = A4, Moves = new(311, 572, 605, 402), Index = 49 }, // Cherubi
        new(50,08,4) { Species = 591, Ability = A4, Moves = new(188, 492, 092, 412), Index = 49 }, // Amoonguss
        new(50,08,4) { Species = 756, Ability = A4, Moves = new(412, 585, 605, 188), Index = 49 }, // Shiinotic
        new(50,08,4) { Species = 819, Ability = A4, Moves = new(747, 360, 371, 331), Index = 49 }, // Skwovet
        new(50,08,4) { Species = 820, Ability = A4, Moves = new(747, 360, 371, 424), Index = 49 }, // Greedent
        new(60,10,5) { Species = 420, Ability = A4, Moves = new(311, 572, 605, 412), Index = 49 }, // Cherubi
        new(60,10,5) { Species = 820, Ability = A4, Moves = new(747, 360, 371, 089), Index = 49, Shiny = Shiny.Always }, // Greedent
        new(60,10,5) { Species = 756, Ability = A4, Moves = new(147, 585, 605, 188), Index = 49 }, // Shiinotic
        new(60,10,5) { Species = 819, Ability = A4, Moves = new(747, 360, 371, 034), Index = 49 }, // Skwovet
        new(60,10,5) { Species = 819, Ability = A4, Moves = new(747, 360, 371, 034), Index = 49, Shiny = Shiny.Always }, // Skwovet
        new(60,10,5) { Species = 820, Ability = A4, Moves = new(747, 360, 371, 089), Index = 49 }, // Greedent

        new(17,01,1) { Species = 884, Ability = A4, Moves = new(232, 043, 468, 249), Index = 48, CanGigantamax = true }, // Duraludon
        new(17,01,1) { Species = 610, Ability = A4, Moves = new(044, 163, 372, 010), Index = 48 }, // Axew
        new(17,01,1) { Species = 704, Ability = A4, Moves = new(225, 352, 033, 175), Index = 48 }, // Goomy
        new(17,01,1) { Species = 446, Ability = A4, Moves = new(033, 044, 122, 111), Index = 48 }, // Munchlax
        new(17,01,1) { Species = 759, Ability = A4, Moves = new(693, 371, 608, 033), Index = 48 }, // Stufful
        new(17,01,1) { Species = 572, Ability = A4, Moves = new(497, 204, 402, 001), Index = 48 }, // Minccino
        new(30,03,2) { Species = 884, Ability = A4, Moves = new(232, 784, 468, 249), Index = 48, CanGigantamax = true }, // Duraludon
        new(30,03,2) { Species = 610, Ability = A4, Moves = new(337, 163, 242, 530), Index = 48 }, // Axew
        new(30,03,2) { Species = 704, Ability = A4, Moves = new(225, 352, 033, 341), Index = 48 }, // Goomy
        new(30,03,2) { Species = 143, Ability = A4, Moves = new(034, 242, 118, 111), Index = 48, CanGigantamax = true }, // Snorlax
        new(30,03,2) { Species = 759, Ability = A4, Moves = new(693, 371, 359, 036), Index = 48 }, // Stufful
        new(30,03,2) { Species = 572, Ability = A4, Moves = new(497, 231, 402, 129), Index = 48 }, // Minccino
        new(40,05,3) { Species = 884, Ability = A4, Moves = new(232, 525, 085, 249), Index = 48, CanGigantamax = true }, // Duraludon
        new(40,05,3) { Species = 611, Ability = A4, Moves = new(406, 231, 242, 530), Index = 48 }, // Fraxure
        new(40,05,3) { Species = 705, Ability = A4, Moves = new(406, 352, 491, 341), Index = 48 }, // Sliggoo
        new(40,05,3) { Species = 143, Ability = A4, Moves = new(034, 667, 242, 281), Index = 48, CanGigantamax = true }, // Snorlax
        new(40,05,3) { Species = 760, Ability = A4, Moves = new(693, 034, 359, 036), Index = 48 }, // Bewear
        new(40,05,3) { Species = 573, Ability = A4, Moves = new(331, 231, 350, 129), Index = 48 }, // Cinccino
        new(50,08,4) { Species = 884, Ability = A4, Moves = new(232, 406, 085, 776), Index = 48, CanGigantamax = true }, // Duraludon
        new(50,08,4) { Species = 612, Ability = A4, Moves = new(406, 231, 370, 530), Index = 48 }, // Haxorus
        new(50,08,4) { Species = 706, Ability = A4, Moves = new(406, 034, 491, 126), Index = 48 }, // Goodra
        new(50,08,4) { Species = 143, Ability = A4, Moves = new(034, 667, 280, 523), Index = 48, CanGigantamax = true }, // Snorlax
        new(50,08,4) { Species = 760, Ability = A4, Moves = new(663, 034, 359, 009), Index = 48 }, // Bewear
        new(50,08,4) { Species = 573, Ability = A4, Moves = new(331, 231, 350, 304), Index = 48 }, // Cinccino
        new(60,10,5) { Species = 884, Ability = A4, Moves = new(430, 406, 085, 776), Index = 48, CanGigantamax = true }, // Duraludon
        new(60,10,5) { Species = 612, Ability = A4, Moves = new(200, 231, 370, 089), Index = 48 }, // Haxorus
        new(60,10,5) { Species = 706, Ability = A4, Moves = new(406, 438, 482, 126), Index = 48 }, // Goodra
        new(60,10,5) { Species = 143, Ability = A4, Moves = new(034, 442, 242, 428), Index = 48, CanGigantamax = true }, // Snorlax
        new(60,10,5) { Species = 760, Ability = A4, Moves = new(663, 038, 276, 009), Index = 48 }, // Bewear
        new(60,10,5) { Species = 573, Ability = A4, Moves = new(402, 231, 350, 304), Index = 48 }, // Cinccino

        new(17,01,1) { Species = 037, Ability = A4, Moves = new(420, 196, 039, 577), Index = 46, Form = 1 }, // Vulpix-1
        new(17,01,1) { Species = 124, Ability = A4, Moves = new(181, 001, 093, 122), Index = 46 }, // Jynx
        new(17,01,1) { Species = 225, Ability = A4, Moves = new(217, 229, 098, 420), Index = 46 }, // Delibird
        new(17,01,1) { Species = 607, Ability = A4, Moves = new(310, 052, 506, 123), Index = 46 }, // Litwick
        new(17,01,1) { Species = 873, Ability = A4, Moves = new(522, 078, 181, 432), Index = 46 }, // Frosmoth
        new(30,03,2) { Species = 037, Ability = A4, Moves = new(420, 058, 326, 577), Index = 46, Form = 1 }, // Vulpix-1
        new(30,03,2) { Species = 124, Ability = A4, Moves = new(181, 001, 093, 313), Index = 46 }, // Jynx
        new(30,03,2) { Species = 225, Ability = A4, Moves = new(217, 065, 034, 693), Index = 46 }, // Delibird
        new(30,03,2) { Species = 608, Ability = A4, Moves = new(310, 261, 083, 123), Index = 46 }, // Lampent
        new(30,03,2) { Species = 873, Ability = A4, Moves = new(522, 078, 062, 432), Index = 46 }, // Frosmoth
        new(40,05,3) { Species = 037, Ability = A4, Moves = new(062, 058, 326, 577), Index = 46, Form = 1 }, // Vulpix-1
        new(40,05,3) { Species = 124, Ability = A4, Moves = new(058, 142, 094, 247), Index = 46 }, // Jynx
        new(40,05,3) { Species = 225, Ability = A4, Moves = new(217, 065, 280, 196), Index = 46 }, // Delibird
        new(40,05,3) { Species = 609, Ability = A4, Moves = new(247, 261, 257, 094), Index = 46 }, // Chandelure
        new(40,05,3) { Species = 873, Ability = A4, Moves = new(405, 403, 062, 432), Index = 46 }, // Frosmoth
        new(50,08,4) { Species = 037, Ability = A4, Moves = new(694, 058, 326, 577), Index = 46, Form = 1 }, // Vulpix-1
        new(50,08,4) { Species = 124, Ability = A4, Moves = new(058, 142, 094, 247), Index = 46 }, // Jynx
        new(50,08,4) { Species = 225, Ability = A4, Moves = new(217, 059, 034, 280), Index = 46 }, // Delibird
        new(50,08,4) { Species = 609, Ability = A4, Moves = new(247, 261, 315, 094), Index = 46 }, // Chandelure
        new(50,08,4) { Species = 873, Ability = A4, Moves = new(405, 403, 058, 297), Index = 46 }, // Frosmoth
        new(60,10,5) { Species = 037, Ability = A4, Moves = new(694, 059, 326, 577), Index = 46, Form = 1 }, // Vulpix-1
        new(60,10,5) { Species = 037, Ability = A4, Moves = new(694, 059, 326, 577), Index = 46, Form = 1, Shiny = Shiny.Always }, // Vulpix-1
        new(60,10,5) { Species = 124, Ability = A4, Moves = new(058, 142, 094, 247), Index = 46 }, // Jynx
        new(60,10,5) { Species = 225, Ability = A4, Moves = new(217, 059, 065, 280), Index = 46 }, // Delibird
        new(60,10,5) { Species = 609, Ability = A4, Moves = new(247, 412, 315, 094), Index = 46 }, // Chandelure
        new(60,10,5) { Species = 873, Ability = A4, Moves = new(405, 403, 058, 542), Index = 46 }, // Frosmoth

        new(17,01,1) { Species = 131, Ability = A4, Moves = new(055, 420, 045, 047), Index = 45, CanGigantamax = true }, // Lapras
        new(17,01,1) { Species = 712, Ability = A4, Moves = new(181, 196, 033, 106), Index = 45 }, // Bergmite
        new(17,01,1) { Species = 461, Ability = A4, Moves = new(420, 372, 232, 279), Index = 45 }, // Weavile
        new(17,01,1) { Species = 850, Ability = A4, Moves = new(172, 044, 035, 052), Index = 45 }, // Sizzlipede
        new(17,01,1) { Species = 776, Ability = A4, Moves = new(175, 123, 033, 052), Index = 45 }, // Turtonator
        new(17,01,1) { Species = 077, Ability = A4, Moves = new(488, 045, 039, 052), Index = 45 }, // Ponyta
        new(30,03,2) { Species = 131, Ability = A4, Moves = new(352, 420, 109, 047), Index = 45, CanGigantamax = true }, // Lapras
        new(30,03,2) { Species = 712, Ability = A4, Moves = new(157, 423, 033, 044), Index = 45 }, // Bergmite
        new(30,03,2) { Species = 461, Ability = A4, Moves = new(420, 372, 232, 279), Index = 45 }, // Weavile
        new(30,03,2) { Species = 851, Ability = A4, Moves = new(172, 404, 422, 044), Index = 45, CanGigantamax = true }, // Centiskorch
        new(30,03,2) { Species = 776, Ability = A4, Moves = new(406, 123, 033, 052), Index = 45 }, // Turtonator
        new(30,03,2) { Species = 077, Ability = A4, Moves = new(488, 023, 583, 097), Index = 45 }, // Ponyta
        new(40,05,3) { Species = 131, Ability = A4, Moves = new(352, 196, 109, 047), Index = 45, CanGigantamax = true }, // Lapras
        new(40,05,3) { Species = 713, Ability = A4, Moves = new(157, 423, 036, 044), Index = 45 }, // Avalugg
        new(40,05,3) { Species = 461, Ability = A4, Moves = new(420, 468, 232, 279), Index = 45 }, // Weavile
        new(40,05,3) { Species = 851, Ability = A4, Moves = new(424, 404, 422, 044), Index = 45, CanGigantamax = true }, // Centiskorch
        new(40,05,3) { Species = 776, Ability = A4, Moves = new(406, 776, 034, 053), Index = 45 }, // Turtonator
        new(40,05,3) { Species = 078, Ability = A4, Moves = new(172, 023, 583, 224), Index = 45 }, // Rapidash
        new(50,08,4) { Species = 131, Ability = A4, Moves = new(057, 196, 058, 047), Index = 45, CanGigantamax = true }, // Lapras
        new(50,08,4) { Species = 713, Ability = A4, Moves = new(776, 059, 036, 044), Index = 45 }, // Avalugg
        new(50,08,4) { Species = 461, Ability = A4, Moves = new(420, 468, 232, 279), Index = 45 }, // Weavile
        new(50,08,4) { Species = 851, Ability = A4, Moves = new(680, 404, 422, 044), Index = 45, CanGigantamax = true }, // Centiskorch
        new(50,08,4) { Species = 776, Ability = A4, Moves = new(406, 776, 504, 053), Index = 45 }, // Turtonator
        new(50,08,4) { Species = 078, Ability = A4, Moves = new(517, 528, 583, 224), Index = 45 }, // Rapidash
        new(60,10,5) { Species = 131, Ability = A4, Moves = new(057, 196, 058, 329), Index = 45, CanGigantamax = true }, // Lapras
        new(60,10,5) { Species = 713, Ability = A4, Moves = new(776, 059, 038, 044), Index = 45 }, // Avalugg
        new(60,10,5) { Species = 461, Ability = A4, Moves = new(420, 400, 232, 279), Index = 45 }, // Weavile
        new(60,10,5) { Species = 851, Ability = A4, Moves = new(680, 679, 422, 044), Index = 45, CanGigantamax = true }, // Centiskorch
        new(60,10,5) { Species = 776, Ability = A4, Moves = new(434, 776, 504, 053), Index = 45 }, // Turtonator
        new(60,10,5) { Species = 078, Ability = A4, Moves = new(394, 528, 583, 224), Index = 45 }, // Rapidash

        new(17,01,1) { Species = 420, Ability = A4, Moves = new(033, 572, 074, 670), Index = 43 }, // Cherubi
        new(17,01,1) { Species = 590, Ability = A4, Moves = new(078, 492, 310, 412), Index = 43 }, // Foongus
        new(17,01,1) { Species = 755, Ability = A4, Moves = new(402, 109, 605, 310), Index = 43 }, // Morelull
        new(17,01,1) { Species = 819, Ability = A4, Moves = new(747, 231, 371, 033), Index = 43 }, // Skwovet
        new(30,03,2) { Species = 420, Ability = A4, Moves = new(033, 572, 074, 402), Index = 43 }, // Cherubi
        new(30,03,2) { Species = 590, Ability = A4, Moves = new(078, 492, 310, 412), Index = 43 }, // Foongus
        new(30,03,2) { Species = 756, Ability = A4, Moves = new(402, 109, 605, 310), Index = 43 }, // Shiinotic
        new(30,03,2) { Species = 819, Ability = A4, Moves = new(747, 231, 371, 033), Index = 43 }, // Skwovet
        new(30,03,2) { Species = 820, Ability = A4, Moves = new(747, 231, 371, 034), Index = 43 }, // Greedent
        new(40,05,3) { Species = 420, Ability = A4, Moves = new(311, 572, 074, 402), Index = 43 }, // Cherubi
        new(40,05,3) { Species = 591, Ability = A4, Moves = new(078, 492, 092, 412), Index = 43 }, // Amoonguss
        new(40,05,3) { Species = 756, Ability = A4, Moves = new(402, 585, 605, 310), Index = 43 }, // Shiinotic
        new(40,05,3) { Species = 819, Ability = A4, Moves = new(747, 360, 371, 044), Index = 43 }, // Skwovet
        new(40,05,3) { Species = 820, Ability = A4, Moves = new(747, 360, 371, 424), Index = 43 }, // Greedent
        new(50,08,4) { Species = 420, Ability = A4, Moves = new(311, 572, 605, 402), Index = 43 }, // Cherubi
        new(50,08,4) { Species = 591, Ability = A4, Moves = new(188, 492, 092, 412), Index = 43 }, // Amoonguss
        new(50,08,4) { Species = 756, Ability = A4, Moves = new(412, 585, 605, 188), Index = 43 }, // Shiinotic
        new(50,08,4) { Species = 819, Ability = A4, Moves = new(747, 360, 371, 331), Index = 43 }, // Skwovet
        new(50,08,4) { Species = 820, Ability = A4, Moves = new(747, 360, 371, 424), Index = 43 }, // Greedent
        new(60,10,5) { Species = 420, Ability = A4, Moves = new(311, 572, 605, 412), Index = 43 }, // Cherubi
        new(60,10,5) { Species = 591, Ability = A4, Moves = new(147, 492, 092, 412), Index = 43 }, // Amoonguss
        new(60,10,5) { Species = 756, Ability = A4, Moves = new(147, 585, 605, 188), Index = 43 }, // Shiinotic
        new(60,10,5) { Species = 819, Ability = A4, Moves = new(747, 360, 371, 034), Index = 43 }, // Skwovet
        new(60,10,5) { Species = 819, Ability = A4, Moves = new(747, 360, 371, 034), Index = 43, Shiny = Shiny.Always }, // Skwovet
        new(60,10,5) { Species = 820, Ability = A4, Moves = new(747, 360, 371, 089), Index = 43 }, // Greedent

        new(17,01,1) { Species = 012, Ability = A4, Moves = new(081, 060, 016, 079), Index = 42, CanGigantamax = true }, // Butterfree
        new(17,01,1) { Species = 213, Ability = A4, Moves = new(088, 474, 414, 522), Index = 42 }, // Shuckle
        new(17,01,1) { Species = 290, Ability = A4, Moves = new(189, 206, 010, 106), Index = 42 }, // Nincada
        new(17,01,1) { Species = 568, Ability = A4, Moves = new(390, 133, 491, 001), Index = 42 }, // Trubbish
        new(17,01,1) { Species = 043, Ability = A4, Moves = new(078, 077, 051, 230), Index = 42 }, // Oddish
        new(17,01,1) { Species = 453, Ability = A4, Moves = new(040, 269, 279, 189), Index = 42 }, // Croagunk
        new(30,03,2) { Species = 012, Ability = A4, Moves = new(405, 060, 016, 079), Index = 42, CanGigantamax = true }, // Butterfree
        new(30,03,2) { Species = 213, Ability = A4, Moves = new(088, 474, 414, 522), Index = 42 }, // Shuckle
        new(30,03,2) { Species = 291, Ability = A4, Moves = new(210, 206, 332, 232), Index = 42 }, // Ninjask
        new(30,03,2) { Species = 568, Ability = A4, Moves = new(092, 133, 491, 036), Index = 42 }, // Trubbish
        new(30,03,2) { Species = 045, Ability = A4, Moves = new(080, 585, 051, 230), Index = 42 }, // Vileplume
        new(30,03,2) { Species = 453, Ability = A4, Moves = new(474, 389, 279, 189), Index = 42 }, // Croagunk
        new(40,05,3) { Species = 012, Ability = A4, Moves = new(405, 060, 016, 078), Index = 42, CanGigantamax = true }, // Butterfree
        new(40,05,3) { Species = 213, Ability = A4, Moves = new(157, 188, 089, 522), Index = 42 }, // Shuckle
        new(40,05,3) { Species = 291, Ability = A4, Moves = new(210, 206, 332, 232), Index = 42 }, // Ninjask
        new(40,05,3) { Species = 569, Ability = A4, Moves = new(188, 133, 034, 707), Index = 42, CanGigantamax = true }, // Garbodor
        new(40,05,3) { Species = 045, Ability = A4, Moves = new(080, 585, 051, 230), Index = 42 }, // Vileplume
        new(40,05,3) { Species = 454, Ability = A4, Moves = new(188, 389, 279, 189), Index = 42 }, // Toxicroak
        new(50,08,4) { Species = 012, Ability = A4, Moves = new(405, 403, 093, 078), Index = 42, CanGigantamax = true }, // Butterfree
        new(50,08,4) { Species = 213, Ability = A4, Moves = new(157, 188, 089, 564), Index = 42 }, // Shuckle
        new(50,08,4) { Species = 291, Ability = A4, Moves = new(210, 163, 332, 232), Index = 42 }, // Ninjask
        new(50,08,4) { Species = 569, Ability = A4, Moves = new(188, 499, 034, 707), Index = 42, CanGigantamax = true }, // Garbodor
        new(50,08,4) { Species = 045, Ability = A4, Moves = new(080, 585, 051, 034), Index = 42 }, // Vileplume
        new(50,08,4) { Species = 454, Ability = A4, Moves = new(188, 389, 280, 189), Index = 42 }, // Toxicroak
        new(60,10,5) { Species = 012, Ability = A4, Moves = new(405, 403, 527, 078), Index = 42, CanGigantamax = true }, // Butterfree
        new(60,10,5) { Species = 213, Ability = A4, Moves = new(444, 188, 089, 564), Index = 42 }, // Shuckle
        new(60,10,5) { Species = 291, Ability = A4, Moves = new(404, 163, 332, 232), Index = 42 }, // Ninjask
        new(60,10,5) { Species = 569, Ability = A4, Moves = new(441, 499, 034, 707), Index = 42, CanGigantamax = true }, // Garbodor
        new(60,10,5) { Species = 045, Ability = A4, Moves = new(080, 585, 051, 034), Index = 42 }, // Vileplume
        new(60,10,5) { Species = 454, Ability = A4, Moves = new(188, 389, 280, 523), Index = 42 }, // Toxicroak

        new(17,01,1) { Species = 562, Ability = A4, Moves = new(261, 114, 310, 101), Index = 41 }, // Yamask
        new(17,01,1) { Species = 778, Ability = A4, Moves = new(086, 452, 425, 010), Index = 41 }, // Mimikyu
        new(17,01,1) { Species = 709, Ability = A4, Moves = new(785, 421, 261, 310), Index = 41 }, // Trevenant
        new(17,01,1) { Species = 855, Ability = A4, Moves = new(597, 110, 668, 310), Index = 41 }, // Polteageist
        new(30,03,2) { Species = 710, Ability = A4, Moves = new(567, 425, 310, 331), Index = 41 }, // Pumpkaboo
        new(30,03,2) { Species = 563, Ability = A4, Moves = new(578, 421, 310, 261), Index = 41 }, // Cofagrigus
        new(30,03,2) { Species = 778, Ability = A4, Moves = new(086, 452, 425, 608), Index = 41 }, // Mimikyu
        new(30,03,2) { Species = 709, Ability = A4, Moves = new(785, 506, 261, 310), Index = 41 }, // Trevenant
        new(30,03,2) { Species = 855, Ability = A4, Moves = new(597, 110, 389, 310), Index = 41 }, // Polteageist
        new(40,05,3) { Species = 710, Ability = A4, Moves = new(567, 247, 310, 402), Index = 41 }, // Pumpkaboo
        new(40,05,3) { Species = 563, Ability = A4, Moves = new(578, 421, 310, 261), Index = 41 }, // Cofagrigus
        new(40,05,3) { Species = 778, Ability = A4, Moves = new(085, 452, 421, 608), Index = 41 }, // Mimikyu
        new(40,05,3) { Species = 709, Ability = A4, Moves = new(785, 506, 261, 310), Index = 41 }, // Trevenant
        new(40,05,3) { Species = 855, Ability = A4, Moves = new(597, 110, 389, 310), Index = 41 }, // Polteageist
        new(50,08,4) { Species = 711, Ability = A4, Moves = new(567, 247, 585, 402), Index = 41 }, // Gourgeist
        new(50,08,4) { Species = 711, Ability = A4, Moves = new(567, 247, 585, 402), Index = 41, Form = 1 }, // Gourgeist-1
        new(50,08,4) { Species = 711, Ability = A4, Moves = new(567, 247, 585, 402), Index = 41, Form = 2 }, // Gourgeist-2
        new(50,08,4) { Species = 711, Ability = A4, Moves = new(567, 247, 585, 402), Index = 41, Form = 3 }, // Gourgeist-3
        new(50,08,4) { Species = 563, Ability = A4, Moves = new(578, 247, 399, 261), Index = 41 }, // Cofagrigus
        new(50,08,4) { Species = 778, Ability = A4, Moves = new(085, 452, 261, 204), Index = 41 }, // Mimikyu
        new(50,08,4) { Species = 709, Ability = A4, Moves = new(452, 506, 261, 310), Index = 41 }, // Trevenant
        new(50,08,4) { Species = 855, Ability = A4, Moves = new(247, 417, 389, 310), Index = 41 }, // Polteageist
        new(60,10,5) { Species = 711, Ability = A4, Moves = new(567, 247, 433, 402), Index = 41 }, // Gourgeist
        new(60,10,5) { Species = 711, Ability = A4, Moves = new(567, 247, 433, 402), Index = 41, Form = 1, Shiny = Shiny.Always }, // Gourgeist-1
        new(60,10,5) { Species = 711, Ability = A4, Moves = new(567, 247, 433, 402), Index = 41, Form = 2 }, // Gourgeist-2
        new(60,10,5) { Species = 711, Ability = A4, Moves = new(567, 247, 433, 402), Index = 41, Form = 3, Shiny = Shiny.Always }, // Gourgeist-3
        new(60,10,5) { Species = 563, Ability = A4, Moves = new(578, 247, 399, 261), Index = 41 }, // Cofagrigus
        new(60,10,5) { Species = 778, Ability = A4, Moves = new(087, 452, 261, 583), Index = 41 }, // Mimikyu
        new(60,10,5) { Species = 709, Ability = A4, Moves = new(452, 506, 261, 310), Index = 41 }, // Trevenant
        new(60,10,5) { Species = 855, Ability = A4, Moves = new(247, 417, 389, 310), Index = 41 }, // Polteageist

        new(17,01,1) { Species = 093, Ability = A4, Moves = new(371, 122, 095, 325), Index = 40 }, // Haunter
        new(17,01,1) { Species = 425, Ability = A4, Moves = new(016, 506, 310, 371), Index = 40 }, // Drifloon
        new(17,01,1) { Species = 355, Ability = A4, Moves = new(310, 425, 043, 506), Index = 40 }, // Duskull
        new(17,01,1) { Species = 859, Ability = A4, Moves = new(372, 313, 260, 044), Index = 40 }, // Impidimp
        new(17,01,1) { Species = 633, Ability = A4, Moves = new(225, 033, 399, 044), Index = 40 }, // Deino
        new(17,01,1) { Species = 877, Ability = A4, Moves = new(084, 098, 681, 043), Index = 40 }, // Morpeko
        new(30,03,2) { Species = 094, Ability = A4, Moves = new(371, 389, 095, 325), Index = 40, CanGigantamax = true }, // Gengar
        new(30,03,2) { Species = 426, Ability = A4, Moves = new(016, 247, 310, 371), Index = 40 }, // Drifblim
        new(30,03,2) { Species = 355, Ability = A4, Moves = new(310, 425, 371, 506), Index = 40 }, // Duskull
        new(30,03,2) { Species = 859, Ability = A4, Moves = new(259, 389, 207, 044), Index = 40 }, // Impidimp
        new(30,03,2) { Species = 633, Ability = A4, Moves = new(225, 021, 399, 029), Index = 40 }, // Deino
        new(30,03,2) { Species = 877, Ability = A4, Moves = new(209, 098, 044, 043), Index = 40 }, // Morpeko
        new(40,05,3) { Species = 094, Ability = A4, Moves = new(506, 389, 095, 325), Index = 40, CanGigantamax = true }, // Gengar
        new(40,05,3) { Species = 426, Ability = A4, Moves = new(016, 247, 360, 371), Index = 40 }, // Drifblim
        new(40,05,3) { Species = 477, Ability = A4, Moves = new(247, 009, 371, 157), Index = 40 }, // Dusknoir
        new(40,05,3) { Species = 860, Ability = A4, Moves = new(417, 793, 421, 399), Index = 40 }, // Morgrem
        new(40,05,3) { Species = 633, Ability = A4, Moves = new(406, 021, 399, 423), Index = 40 }, // Deino
        new(40,05,3) { Species = 877, Ability = A4, Moves = new(209, 098, 044, 402), Index = 40 }, // Morpeko
        new(50,08,4) { Species = 094, Ability = A4, Moves = new(247, 399, 094, 085), Index = 40, CanGigantamax = true }, // Gengar
        new(50,08,4) { Species = 426, Ability = A4, Moves = new(366, 247, 360, 371), Index = 40 }, // Drifblim
        new(50,08,4) { Species = 477, Ability = A4, Moves = new(247, 009, 280, 157), Index = 40 }, // Dusknoir
        new(50,08,4) { Species = 861, Ability = A4, Moves = new(789, 492, 421, 399), Index = 40, CanGigantamax = true }, // Grimmsnarl
        new(50,08,4) { Species = 634, Ability = A4, Moves = new(406, 304, 399, 423), Index = 40 }, // Zweilous
        new(50,08,4) { Species = 877, Ability = A4, Moves = new(783, 098, 242, 402), Index = 40 }, // Morpeko
        new(60,10,5) { Species = 094, Ability = A4, Moves = new(247, 399, 605, 085), Index = 40, CanGigantamax = true }, // Gengar
        new(60,10,5) { Species = 426, Ability = A4, Moves = new(366, 247, 360, 693), Index = 40 }, // Drifblim
        new(60,10,5) { Species = 477, Ability = A4, Moves = new(247, 009, 280, 089), Index = 40 }, // Dusknoir
        new(60,10,5) { Species = 861, Ability = A4, Moves = new(789, 492, 421, 417), Index = 40, CanGigantamax = true }, // Grimmsnarl
        new(60,10,5) { Species = 635, Ability = A4, Moves = new(406, 304, 399, 056), Index = 40 }, // Hydreigon
        new(60,10,5) { Species = 877, Ability = A4, Moves = new(783, 037, 242, 402), Index = 40 }, // Morpeko
    };
}
