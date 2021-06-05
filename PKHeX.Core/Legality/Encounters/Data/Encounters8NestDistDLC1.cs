using static PKHeX.Core.GameVersion;

namespace PKHeX.Core
{
    // Distribution Nest Encounters (BCAT)
    internal static partial class Encounters8Nest
    {
        /// <summary>
        /// Nest distribution raids for <see cref="GameVersion.SWSH"/> available after Isle of Armor expansion and before Crown Tundra.
        /// </summary>
        internal static readonly EncounterStatic8ND[] Dist_DLC1 =
        {
            new(17,01,1) { Species = 093, Ability = A4, Moves = new[]{ 371, 122, 095, 325 }, Index = 39 }, // Haunter
            new(17,01,1) { Species = 425, Ability = A4, Moves = new[]{ 016, 506, 310, 371 }, Index = 39 }, // Drifloon
            new(17,01,1) { Species = 355, Ability = A4, Moves = new[]{ 310, 425, 043, 506 }, Index = 39 }, // Duskull
            new(17,01,1) { Species = 859, Ability = A4, Moves = new[]{ 372, 313, 260, 044 }, Index = 39 }, // Impidimp
            new(17,01,1) { Species = 633, Ability = A4, Moves = new[]{ 225, 033, 399, 044 }, Index = 39 }, // Deino
            new(17,01,1) { Species = 877, Ability = A4, Moves = new[]{ 084, 098, 681, 043 }, Index = 39 }, // Morpeko
            new(30,03,2) { Species = 094, Ability = A4, Moves = new[]{ 371, 389, 095, 325 }, Index = 39, CanGigantamax = true }, // Gengar
            new(30,03,2) { Species = 426, Ability = A4, Moves = new[]{ 016, 247, 310, 371 }, Index = 39 }, // Drifblim
            new(30,03,2) { Species = 355, Ability = A4, Moves = new[]{ 310, 425, 371, 506 }, Index = 39 }, // Duskull
            new(30,03,2) { Species = 859, Ability = A4, Moves = new[]{ 259, 389, 207, 044 }, Index = 39 }, // Impidimp
            new(30,03,2) { Species = 633, Ability = A4, Moves = new[]{ 225, 021, 399, 029 }, Index = 39 }, // Deino
            new(30,03,2) { Species = 877, Ability = A4, Moves = new[]{ 209, 098, 044, 043 }, Index = 39 }, // Morpeko
            new(40,05,3) { Species = 094, Ability = A4, Moves = new[]{ 506, 389, 095, 325 }, Index = 39, CanGigantamax = true }, // Gengar
            new(40,05,3) { Species = 426, Ability = A4, Moves = new[]{ 016, 247, 360, 371 }, Index = 39 }, // Drifblim
            new(40,05,3) { Species = 477, Ability = A4, Moves = new[]{ 247, 009, 371, 157 }, Index = 39 }, // Dusknoir
            new(40,05,3) { Species = 860, Ability = A4, Moves = new[]{ 417, 793, 421, 399 }, Index = 39 }, // Morgrem
            new(40,05,3) { Species = 633, Ability = A4, Moves = new[]{ 406, 021, 399, 423 }, Index = 39 }, // Deino
            new(40,05,3) { Species = 877, Ability = A4, Moves = new[]{ 209, 098, 044, 402 }, Index = 39 }, // Morpeko
            new(50,08,4) { Species = 094, Ability = A4, Moves = new[]{ 247, 399, 094, 085 }, Index = 39, CanGigantamax = true }, // Gengar
            new(50,08,4) { Species = 426, Ability = A4, Moves = new[]{ 366, 247, 360, 371 }, Index = 39 }, // Drifblim
            new(50,08,4) { Species = 477, Ability = A4, Moves = new[]{ 247, 009, 280, 157 }, Index = 39 }, // Dusknoir
            new(50,08,4) { Species = 861, Ability = A4, Moves = new[]{ 789, 492, 421, 399 }, Index = 39, CanGigantamax = true }, // Grimmsnarl
            new(50,08,4) { Species = 634, Ability = A4, Moves = new[]{ 406, 304, 399, 423 }, Index = 39 }, // Zweilous
            new(50,08,4) { Species = 877, Ability = A4, Moves = new[]{ 783, 098, 242, 402 }, Index = 39 }, // Morpeko
            new(60,10,5) { Species = 094, Ability = A4, Moves = new[]{ 247, 399, 605, 085 }, Index = 39, CanGigantamax = true }, // Gengar
            new(60,10,5) { Species = 426, Ability = A4, Moves = new[]{ 366, 247, 360, 693 }, Index = 39 }, // Drifblim
            new(60,10,5) { Species = 477, Ability = A4, Moves = new[]{ 247, 009, 280, 089 }, Index = 39 }, // Dusknoir
            new(60,10,5) { Species = 861, Ability = A4, Moves = new[]{ 789, 492, 421, 417 }, Index = 39, CanGigantamax = true }, // Grimmsnarl
            new(60,10,5) { Species = 635, Ability = A4, Moves = new[]{ 406, 304, 399, 056 }, Index = 39 }, // Hydreigon
            new(60,10,5) { Species = 877, Ability = A4, Moves = new[]{ 783, 037, 242, 402 }, Index = 39 }, // Morpeko

            new(17,01,1) { Species = 036, Ability = A4, Moves = new[]{ 574, 001, 204, 045 }, Index = 37 }, // Clefable
            new(17,01,1) { Species = 040, Ability = A4, Moves = new[]{ 497, 574, 001, 111 }, Index = 37 }, // Wigglytuff
            new(17,01,1) { Species = 044, Ability = A4, Moves = new[]{ 078, 079, 230, 051 }, Index = 37 }, // Gloom
            new(17,01,1) { Species = 518, Ability = A4, Moves = new[]{ 060, 236, 111, 000 }, Index = 37 }, // Musharna
            new(17,01,1) { Species = 547, Ability = A4, Moves = new[]{ 412, 585, 073, 178 }, Index = 37 }, // Whimsicott
            new(17,01,1) { Species = 549, Ability = A4, Moves = new[]{ 412, 241, 437, 263 }, Index = 37 }, // Lilligant
            new(30,03,2) { Species = 036, Ability = A4, Moves = new[]{ 574, 001, 236, 045 }, Index = 37 }, // Clefable
            new(30,03,2) { Species = 040, Ability = A4, Moves = new[]{ 497, 574, 001, 034 }, Index = 37 }, // Wigglytuff
            new(30,03,2) { Species = 182, Ability = A4, Moves = new[]{ 585, 572, 236, 051 }, Index = 37 }, // Bellossom
            new(30,03,2) { Species = 518, Ability = A4, Moves = new[]{ 428, 060, 236, 111 }, Index = 37 }, // Musharna
            new(30,03,2) { Species = 547, Ability = A4, Moves = new[]{ 412, 585, 073, 178 }, Index = 37 }, // Whimsicott
            new(30,03,2) { Species = 549, Ability = A4, Moves = new[]{ 412, 241, 437, 263 }, Index = 37 }, // Lilligant
            new(40,05,3) { Species = 036, Ability = A4, Moves = new[]{ 585, 309, 236, 345 }, Index = 37 }, // Clefable
            new(40,05,3) { Species = 040, Ability = A4, Moves = new[]{ 304, 583, 360, 034 }, Index = 37 }, // Wigglytuff
            new(40,05,3) { Species = 182, Ability = A4, Moves = new[]{ 585, 572, 236, 051 }, Index = 37 }, // Bellossom
            new(40,05,3) { Species = 518, Ability = A4, Moves = new[]{ 585, 094, 236, 360 }, Index = 37 }, // Musharna
            new(40,05,3) { Species = 547, Ability = A4, Moves = new[]{ 412, 585, 073, 366 }, Index = 37 }, // Whimsicott
            new(40,05,3) { Species = 549, Ability = A4, Moves = new[]{ 412, 241, 437, 263 }, Index = 37 }, // Lilligant
            new(50,08,4) { Species = 036, Ability = A4, Moves = new[]{ 585, 309, 236, 345 }, Index = 37 }, // Clefable
            new(50,08,4) { Species = 040, Ability = A4, Moves = new[]{ 304, 583, 360, 034 }, Index = 37 }, // Wigglytuff
            new(50,08,4) { Species = 182, Ability = A4, Moves = new[]{ 585, 572, 236, 051 }, Index = 37 }, // Bellossom
            new(50,08,4) { Species = 518, Ability = A4, Moves = new[]{ 585, 094, 236, 360 }, Index = 37 }, // Musharna
            new(50,08,4) { Species = 547, Ability = A4, Moves = new[]{ 538, 585, 073, 366 }, Index = 37 }, // Whimsicott
            new(50,08,4) { Species = 549, Ability = A4, Moves = new[]{ 412, 241, 437, 263 }, Index = 37 }, // Lilligant
            new(60,10,5) { Species = 036, Ability = A4, Moves = new[]{ 585, 309, 236, 345 }, Index = 37 }, // Clefable
            new(60,10,5) { Species = 040, Ability = A4, Moves = new[]{ 304, 583, 360, 034 }, Index = 37 }, // Wigglytuff
            new(60,10,5) { Species = 182, Ability = A4, Moves = new[]{ 585, 572, 236, 051 }, Index = 37 }, // Bellossom
            new(60,10,5) { Species = 518, Ability = A4, Moves = new[]{ 585, 094, 236, 360 }, Index = 37 }, // Musharna
            new(60,10,5) { Species = 036, Ability = A4, Moves = new[]{ 585, 309, 236, 345 }, Index = 37, Shiny = Shiny.Always }, // Clefable
            new(60,10,5) { Species = 549, Ability = A4, Moves = new[]{ 412, 241, 437, 263 }, Index = 37 }, // Lilligant

            new(17,01,1) { Species = 848, Ability = A4, Moves = new[]{ 609, 051, 496, 715 }, Index = 36 }, // Toxel
            new(17,01,1) { Species = 835, Ability = A4, Moves = new[]{ 609, 033, 044, 046 }, Index = 36 }, // Yamper
            new(17,01,1) { Species = 695, Ability = A4, Moves = new[]{ 085, 098, 001, 189 }, Index = 36 }, // Heliolisk
            new(17,01,1) { Species = 840, Ability = A4, Moves = new[]{ 110, 310, 000, 000 }, Index = 36 }, // Applin
            new(17,01,1) { Species = 597, Ability = A4, Moves = new[]{ 033, 042, 232, 106 }, Index = 36 }, // Ferroseed
            new(17,01,1) { Species = 829, Ability = A4, Moves = new[]{ 075, 496, 229, 670 }, Index = 36 }, // Gossifleur
            new(30,03,2) { Species = 836, Ability = A4, Moves = new[]{ 609, 209, 204, 706 }, Index = 36 }, // Boltund
            new(30,03,2) { Species = 695, Ability = A4, Moves = new[]{ 085, 098, 001, 189 }, Index = 36 }, // Heliolisk
            new(30,03,2) { Species = 597, Ability = A4, Moves = new[]{ 442, 042, 232, 106 }, Index = 36 }, // Ferroseed
            new(30,03,2) { Species = 830, Ability = A4, Moves = new[]{ 536, 496, 229, 670 }, Index = 36 }, // Eldegoss
            new(40,05,3) { Species = 836, Ability = A4, Moves = new[]{ 609, 209, 424, 706 }, Index = 36 }, // Boltund
            new(40,05,3) { Species = 695, Ability = A4, Moves = new[]{ 085, 098, 447, 189 }, Index = 36 }, // Heliolisk
            new(40,05,3) { Species = 598, Ability = A4, Moves = new[]{ 442, 438, 398, 106 }, Index = 36 }, // Ferrothorn
            new(40,05,3) { Species = 830, Ability = A4, Moves = new[]{ 536, 304, 229, 670 }, Index = 36 }, // Eldegoss
            new(50,08,4) { Species = 836, Ability = A4, Moves = new[]{ 609, 528, 424, 706 }, Index = 36 }, // Boltund
            new(50,08,4) { Species = 695, Ability = A4, Moves = new[]{ 085, 304, 447, 523 }, Index = 36 }, // Heliolisk
            new(50,08,4) { Species = 598, Ability = A4, Moves = new[]{ 360, 438, 398, 014 }, Index = 36 }, // Ferrothorn
            new(50,08,4) { Species = 830, Ability = A4, Moves = new[]{ 536, 304, 229, 437 }, Index = 36 }, // Eldegoss
            new(60,10,5) { Species = 836, Ability = A4, Moves = new[]{ 609, 528, 424, 706 }, Index = 36 }, // Boltund
            new(60,10,5) { Species = 695, Ability = A4, Moves = new[]{ 085, 304, 447, 523 }, Index = 36 }, // Heliolisk
            new(60,10,5) { Species = 598, Ability = A4, Moves = new[]{ 360, 438, 398, 014 }, Index = 36 }, // Ferrothorn
            new(60,10,5) { Species = 830, Ability = A4, Moves = new[]{ 536, 304, 063, 437 }, Index = 36 }, // Eldegoss
            new(30,03,2,SW) { Species = 849, Ability = A4, Moves = new[]{ 351, 506, 491, 103 }, Index = 36, Form = 1, CanGigantamax = true }, // Toxtricity-1
            new(30,03,2,SW) { Species = 842, Ability = A4, Moves = new[]{ 787, 496, 310, 029 }, Index = 36, CanGigantamax = true }, // Appletun
            new(40,05,3,SW) { Species = 849, Ability = A4, Moves = new[]{ 435, 506, 398, 103 }, Index = 36, Form = 1, CanGigantamax = true }, // Toxtricity-1
            new(40,05,3,SW) { Species = 842, Ability = A4, Moves = new[]{ 787, 496, 406, 029 }, Index = 36, CanGigantamax = true }, // Appletun
            new(50,08,4,SW) { Species = 849, Ability = A4, Moves = new[]{ 786, 506, 398, 586 }, Index = 36, Form = 1, CanGigantamax = true }, // Toxtricity-1
            new(50,08,4,SW) { Species = 842, Ability = A4, Moves = new[]{ 787, 496, 406, 523 }, Index = 36, CanGigantamax = true }, // Appletun
            new(60,10,5,SW) { Species = 849, Ability = A4, Moves = new[]{ 786, 506, 599, 586 }, Index = 36, Form = 1, CanGigantamax = true }, // Toxtricity-1
            new(60,10,5,SW) { Species = 842, Ability = A4, Moves = new[]{ 787, 034, 406, 523 }, Index = 36, CanGigantamax = true }, // Appletun
            new(30,03,2,SH) { Species = 849, Ability = A4, Moves = new[]{ 351, 506, 491, 103 }, Index = 36, CanGigantamax = true }, // Toxtricity
            new(30,03,2,SH) { Species = 841, Ability = A4, Moves = new[]{ 406, 073, 491, 184 }, Index = 36, CanGigantamax = true }, // Flapple
            new(40,05,3,SH) { Species = 849, Ability = A4, Moves = new[]{ 435, 506, 474, 103 }, Index = 36, CanGigantamax = true }, // Toxtricity
            new(40,05,3,SH) { Species = 841, Ability = A4, Moves = new[]{ 406, 788, 491, 184 }, Index = 36, CanGigantamax = true }, // Flapple
            new(50,08,4,SH) { Species = 849, Ability = A4, Moves = new[]{ 786, 506, 474, 586 }, Index = 36, CanGigantamax = true }, // Toxtricity
            new(50,08,4,SH) { Species = 841, Ability = A4, Moves = new[]{ 406, 788, 491, 263 }, Index = 36, CanGigantamax = true }, // Flapple
            new(60,10,5,SH) { Species = 849, Ability = A4, Moves = new[]{ 786, 506, 474, 586 }, Index = 36, CanGigantamax = true }, // Toxtricity
            new(60,10,5,SH) { Species = 841, Ability = A4, Moves = new[]{ 406, 788, 491, 263 }, Index = 36, CanGigantamax = true }, // Flapple

            new(17,01,1) { Species = 025, Ability = A4, Moves = new[]{ 084, 098, 204, 086 }, Index = 34 }, // Pikachu
            new(17,01,1) { Species = 026, Ability = A4, Moves = new[]{ 009, 129, 280, 204 }, Index = 34 }, // Raichu
            new(17,01,1) { Species = 026, Ability = A4, Moves = new[]{ 009, 129, 280, 204 }, Index = 34, Form = 1 }, // Raichu-1
            new(17,01,1) { Species = 172, Ability = A4, Moves = new[]{ 589, 609, 085, 186 }, Index = 34 }, // Pichu
            new(17,01,1) { Species = 778, Ability = A4, Moves = new[]{ 086, 452, 425, 010 }, Index = 34 }, // Mimikyu
            new(30,03,2) { Species = 025, Ability = A4, Moves = new[]{ 209, 097, 204, 086 }, Index = 34 }, // Pikachu
            new(30,03,2) { Species = 026, Ability = A4, Moves = new[]{ 009, 129, 280, 204 }, Index = 34 }, // Raichu
            new(30,03,2) { Species = 026, Ability = A4, Moves = new[]{ 009, 129, 280, 204 }, Index = 34, Form = 1 }, // Raichu-1
            new(30,03,2) { Species = 172, Ability = A4, Moves = new[]{ 204, 609, 085, 186 }, Index = 34 }, // Pichu
            new(30,03,2) { Species = 778, Ability = A4, Moves = new[]{ 086, 452, 425, 608 }, Index = 34 }, // Mimikyu
            new(40,05,3) { Species = 025, Ability = A4, Moves = new[]{ 085, 231, 583, 086 }, Index = 34 }, // Pikachu
            new(40,05,3) { Species = 026, Ability = A4, Moves = new[]{ 085, 034, 411, 583 }, Index = 34 }, // Raichu
            new(40,05,3) { Species = 026, Ability = A4, Moves = new[]{ 085, 034, 057, 583 }, Index = 34, Form = 1 }, // Raichu-1
            new(40,05,3) { Species = 172, Ability = A4, Moves = new[]{ 204, 609, 085, 583 }, Index = 34 }, // Pichu
            new(40,05,3) { Species = 778, Ability = A4, Moves = new[]{ 085, 452, 421, 608 }, Index = 34 }, // Mimikyu
            new(50,08,4) { Species = 025, Ability = A4, Moves = new[]{ 087, 231, 583, 086 }, Index = 34 }, // Pikachu
            new(50,08,4) { Species = 026, Ability = A4, Moves = new[]{ 087, 034, 411, 583 }, Index = 34 }, // Raichu
            new(50,08,4) { Species = 026, Ability = A4, Moves = new[]{ 087, 034, 057, 583 }, Index = 34, Form = 1 }, // Raichu-1
            new(50,08,4) { Species = 172, Ability = A4, Moves = new[]{ 253, 609, 085, 583 }, Index = 34 }, // Pichu
            new(50,08,4) { Species = 778, Ability = A4, Moves = new[]{ 085, 452, 261, 204 }, Index = 34 }, // Mimikyu
            new(60,10,5) { Species = 025, Ability = A4, Moves = new[]{ 344, 231, 583, 086 }, Index = 34 }, // Pikachu
            new(60,10,5) { Species = 025, Ability = A4, Moves = new[]{ 344, 231, 583, 086 }, Index = 34, Shiny = Shiny.Always }, // Pikachu
            new(60,10,5) { Species = 026, Ability = A4, Moves = new[]{ 087, 034, 411, 583 }, Index = 34 }, // Raichu
            new(60,10,5) { Species = 026, Ability = A4, Moves = new[]{ 087, 034, 057, 583 }, Index = 34, Form = 1 }, // Raichu-1
            new(60,10,5) { Species = 172, Ability = A4, Moves = new[]{ 253, 609, 085, 583 }, Index = 34 }, // Pichu
            new(60,10,5) { Species = 778, Ability = A4, Moves = new[]{ 087, 452, 261, 583 }, Index = 34 }, // Mimikyu

            new(17,01,1) { Species = 833, Ability = A4, Moves = new[]{ 055, 033, 044, 240 }, Index = 33 }, // Chewtle
            new(17,01,1) { Species = 349, Ability = A4, Moves = new[]{ 150, 033, 175, 057 }, Index = 33 }, // Feebas
            new(17,01,1) { Species = 194, Ability = A4, Moves = new[]{ 341, 021, 039, 055 }, Index = 33 }, // Wooper
            new(17,01,1) { Species = 843, Ability = A4, Moves = new[]{ 028, 035, 523, 693 }, Index = 33 }, // Silicobra
            new(17,01,1) { Species = 449, Ability = A4, Moves = new[]{ 341, 328, 044, 033 }, Index = 33 }, // Hippopotas
            new(17,01,1) { Species = 422, Ability = A4, Moves = new[]{ 352, 106, 189, 055 }, Index = 33, Form = 1 }, // Shellos-1
            new(30,03,2) { Species = 834, Ability = A4, Moves = new[]{ 157, 534, 317, 055 }, Index = 33, CanGigantamax = true }, // Drednaw
            new(30,03,2) { Species = 349, Ability = A4, Moves = new[]{ 057, 033, 175, 150 }, Index = 33 }, // Feebas
            new(30,03,2) { Species = 195, Ability = A4, Moves = new[]{ 341, 021, 401, 055 }, Index = 33 }, // Quagsire
            new(30,03,2) { Species = 843, Ability = A4, Moves = new[]{ 091, 029, 523, 693 }, Index = 33 }, // Silicobra
            new(30,03,2) { Species = 449, Ability = A4, Moves = new[]{ 341, 036, 044, 242 }, Index = 33 }, // Hippopotas
            new(30,03,2) { Species = 423, Ability = A4, Moves = new[]{ 189, 352, 246, 106 }, Index = 33, Form = 1 }, // Gastrodon-1
            new(40,05,3) { Species = 834, Ability = A4, Moves = new[]{ 157, 534, 317, 055 }, Index = 33, CanGigantamax = true }, // Drednaw
            new(40,05,3) { Species = 350, Ability = A4, Moves = new[]{ 057, 239, 034, 574 }, Index = 33 }, // Milotic
            new(40,05,3) { Species = 195, Ability = A4, Moves = new[]{ 341, 021, 401, 005 }, Index = 33 }, // Quagsire
            new(40,05,3) { Species = 844, Ability = A4, Moves = new[]{ 693, 523, 201, 091 }, Index = 33, CanGigantamax = true }, // Sandaconda
            new(40,05,3) { Species = 450, Ability = A4, Moves = new[]{ 341, 422, 036, 242 }, Index = 33 }, // Hippowdon
            new(40,05,3) { Species = 423, Ability = A4, Moves = new[]{ 414, 352, 246, 106 }, Index = 33, Form = 1 }, // Gastrodon-1
            new(50,08,4) { Species = 834, Ability = A4, Moves = new[]{ 157, 710, 317, 334 }, Index = 33, CanGigantamax = true }, // Drednaw
            new(50,08,4) { Species = 350, Ability = A4, Moves = new[]{ 057, 231, 034, 574 }, Index = 33 }, // Milotic
            new(50,08,4) { Species = 195, Ability = A4, Moves = new[]{ 341, 280, 401, 005 }, Index = 33 }, // Quagsire
            new(50,08,4) { Species = 844, Ability = A4, Moves = new[]{ 693, 529, 201, 091 }, Index = 33, CanGigantamax = true }, // Sandaconda
            new(50,08,4) { Species = 450, Ability = A4, Moves = new[]{ 089, 422, 036, 242 }, Index = 33 }, // Hippowdon
            new(50,08,4) { Species = 423, Ability = A4, Moves = new[]{ 414, 503, 311, 106 }, Index = 33, Form = 1 }, // Gastrodon-1
            new(60,10,5) { Species = 834, Ability = A4, Moves = new[]{ 157, 710, 317, 334 }, Index = 33, CanGigantamax = true }, // Drednaw
            new(60,10,5) { Species = 350, Ability = A4, Moves = new[]{ 503, 231, 034, 574 }, Index = 33 }, // Milotic
            new(60,10,5) { Species = 195, Ability = A4, Moves = new[]{ 089, 280, 401, 005 }, Index = 33 }, // Quagsire
            new(60,10,5) { Species = 844, Ability = A4, Moves = new[]{ 693, 529, 201, 091 }, Index = 33, CanGigantamax = true }, // Sandaconda
            new(60,10,5) { Species = 450, Ability = A4, Moves = new[]{ 089, 422, 231, 242 }, Index = 33 }, // Hippowdon
            new(60,10,5) { Species = 423, Ability = A4, Moves = new[]{ 414, 503, 311, 352 }, Index = 33, Form = 1 }, // Gastrodon-1

            new(17,01,1) { Species = 320, Ability = A4, Moves = new[]{ 362, 034, 310, 054 }, Index = 31 }, // Wailmer
            new(17,01,1) { Species = 098, Ability = A4, Moves = new[]{ 055, 341, 043, 232 }, Index = 31 }, // Krabby
            new(17,01,1) { Species = 771, Ability = A4, Moves = new[]{ 240, 219, 213, 269 }, Index = 31 }, // Pyukumuku
            new(17,01,1) { Species = 592, Ability = A4, Moves = new[]{ 352, 101, 071, 240 }, Index = 31 }, // Frillish
            new(17,01,1) { Species = 458, Ability = A4, Moves = new[]{ 033, 017, 352, 469 }, Index = 31 }, // Mantyke
            new(17,01,1) { Species = 318, Ability = A4, Moves = new[]{ 453, 305, 116, 044 }, Index = 31 }, // Carvanha
            new(30,03,2) { Species = 320, Ability = A4, Moves = new[]{ 362, 034, 310, 054 }, Index = 31 }, // Wailmer
            new(30,03,2) { Species = 098, Ability = A4, Moves = new[]{ 061, 341, 023, 232 }, Index = 31 }, // Krabby
            new(30,03,2) { Species = 771, Ability = A4, Moves = new[]{ 240, 219, 213, 174 }, Index = 31 }, // Pyukumuku
            new(30,03,2) { Species = 592, Ability = A4, Moves = new[]{ 362, 101, 071, 240 }, Index = 31 }, // Frillish
            new(30,03,2) { Species = 458, Ability = A4, Moves = new[]{ 029, 017, 061, 469 }, Index = 31 }, // Mantyke
            new(30,03,2) { Species = 319, Ability = A4, Moves = new[]{ 453, 400, 423, 044 }, Index = 31 }, // Sharpedo
            new(40,05,3) { Species = 321, Ability = A4, Moves = new[]{ 362, 034, 340, 568 }, Index = 31 }, // Wailord
            new(40,05,3) { Species = 099, Ability = A4, Moves = new[]{ 534, 341, 021, 014 }, Index = 31 }, // Kingler
            new(40,05,3) { Species = 771, Ability = A4, Moves = new[]{ 240, 219, 213, 174 }, Index = 31 }, // Pyukumuku
            new(40,05,3) { Species = 593, Ability = A4, Moves = new[]{ 362, 247, 071, 151 }, Index = 31 }, // Jellicent
            new(40,05,3) { Species = 226, Ability = A4, Moves = new[]{ 029, 403, 060, 331 }, Index = 31 }, // Mantine
            new(40,05,3) { Species = 319, Ability = A4, Moves = new[]{ 453, 242, 423, 044 }, Index = 31 }, // Sharpedo
            new(50,08,4) { Species = 321, Ability = A4, Moves = new[]{ 056, 034, 340, 133 }, Index = 31 }, // Wailord
            new(50,08,4) { Species = 099, Ability = A4, Moves = new[]{ 534, 341, 359, 014 }, Index = 31, CanGigantamax = true }, // Kingler
            new(50,08,4) { Species = 771, Ability = A4, Moves = new[]{ 240, 219, 213, 174 }, Index = 31 }, // Pyukumuku
            new(50,08,4) { Species = 593, Ability = A4, Moves = new[]{ 056, 247, 071, 151 }, Index = 31 }, // Jellicent
            new(50,08,4) { Species = 226, Ability = A4, Moves = new[]{ 036, 403, 060, 331 }, Index = 31 }, // Mantine
            new(50,08,4) { Species = 319, Ability = A4, Moves = new[]{ 057, 242, 423, 044 }, Index = 31 }, // Sharpedo
            new(60,10,5) { Species = 321, Ability = A4, Moves = new[]{ 503, 034, 340, 133 }, Index = 31, Shiny = Shiny.Always }, // Wailord
            new(60,10,5) { Species = 321, Ability = A4, Moves = new[]{ 056, 034, 340, 133 }, Index = 31 }, // Wailord
            new(60,10,5) { Species = 771, Ability = A4, Moves = new[]{ 092, 599, 213, 174 }, Index = 31 }, // Pyukumuku
            new(60,10,5) { Species = 593, Ability = A4, Moves = new[]{ 056, 058, 605, 433 }, Index = 31 }, // Jellicent
            new(60,10,5) { Species = 226, Ability = A4, Moves = new[]{ 036, 403, 060, 331 }, Index = 31 }, // Mantine
            new(60,10,5) { Species = 319, Ability = A4, Moves = new[]{ 057, 242, 423, 305 }, Index = 31 }, // Sharpedo

            new(17,01,1) { Species = 878, Ability = A4, Moves = new[]{ 523, 205, 045, 249 }, Index = 27 }, // Cufant
            new(17,01,1) { Species = 208, Ability = A4, Moves = new[]{ 242, 442, 106, 422 }, Index = 27 }, // Steelix
            new(17,01,1) { Species = 052, Ability = A4, Moves = new[]{ 232, 006, 242, 045 }, Index = 27, Form = 2 }, // Meowth-2
            new(17,01,1) { Species = 837, Ability = A4, Moves = new[]{ 229, 261, 479, 108 }, Index = 27 }, // Rolycoly
            new(17,01,1) { Species = 111, Ability = A4, Moves = new[]{ 479, 523, 196, 182 }, Index = 27 }, // Rhyhorn
            new(17,01,1) { Species = 095, Ability = A4, Moves = new[]{ 174, 225, 034, 106 }, Index = 27 }, // Onix
            new(30,03,2) { Species = 878, Ability = A4, Moves = new[]{ 523, 023, 334, 249 }, Index = 27 }, // Cufant
            new(30,03,2) { Species = 208, Ability = A4, Moves = new[]{ 157, 442, 328, 422 }, Index = 27 }, // Steelix
            new(30,03,2) { Species = 863, Ability = A4, Moves = new[]{ 442, 006, 242, 269 }, Index = 27 }, // Perrserker
            new(30,03,2) { Species = 838, Ability = A4, Moves = new[]{ 229, 488, 157, 108 }, Index = 27 }, // Carkol
            new(30,03,2) { Species = 111, Ability = A4, Moves = new[]{ 350, 523, 196, 182 }, Index = 27 }, // Rhyhorn
            new(30,03,2) { Species = 095, Ability = A4, Moves = new[]{ 776, 225, 034, 106 }, Index = 27 }, // Onix
            new(40,05,3) { Species = 879, Ability = A4, Moves = new[]{ 070, 523, 334, 442 }, Index = 27, CanGigantamax = true }, // Copperajah
            new(40,05,3) { Species = 208, Ability = A4, Moves = new[]{ 157, 442, 328, 422 }, Index = 27 }, // Steelix
            new(40,05,3) { Species = 863, Ability = A4, Moves = new[]{ 442, 006, 154, 269 }, Index = 27 }, // Perrserker
            new(40,05,3) { Species = 839, Ability = A4, Moves = new[]{ 025, 488, 157, 108 }, Index = 27, CanGigantamax = true }, // Coalossal
            new(40,05,3) { Species = 112, Ability = A4, Moves = new[]{ 036, 529, 008, 182 }, Index = 27 }, // Rhydon
            new(40,05,3) { Species = 095, Ability = A4, Moves = new[]{ 776, 225, 021, 201 }, Index = 27 }, // Onix
            new(50,08,4) { Species = 879, Ability = A4, Moves = new[]{ 070, 523, 334, 442 }, Index = 27, CanGigantamax = true }, // Copperajah
            new(50,08,4) { Species = 208, Ability = A4, Moves = new[]{ 157, 231, 328, 422 }, Index = 27 }, // Steelix
            new(50,08,4) { Species = 863, Ability = A4, Moves = new[]{ 442, 583, 154, 269 }, Index = 27 }, // Perrserker
            new(50,08,4) { Species = 839, Ability = A4, Moves = new[]{ 025, 488, 157, 115 }, Index = 27, CanGigantamax = true }, // Coalossal
            new(50,08,4) { Species = 464, Ability = A4, Moves = new[]{ 350, 089, 008, 182 }, Index = 27 }, // Rhyperior
            new(50,08,4) { Species = 095, Ability = A4, Moves = new[]{ 776, 225, 784, 201 }, Index = 27 }, // Onix
            new(60,10,5) { Species = 879, Ability = A4, Moves = new[]{ 276, 089, 583, 442 }, Index = 27, CanGigantamax = true }, // Copperajah
            new(60,10,5) { Species = 208, Ability = A4, Moves = new[]{ 038, 231, 529, 422 }, Index = 27 }, // Steelix
            new(60,10,5) { Species = 863, Ability = A4, Moves = new[]{ 442, 583, 370, 269 }, Index = 27 }, // Perrserker
            new(60,10,5) { Species = 839, Ability = A4, Moves = new[]{ 076, 682, 157, 115 }, Index = 27, CanGigantamax = true }, // Coalossal
            new(60,10,5) { Species = 464, Ability = A4, Moves = new[]{ 444, 089, 008, 224 }, Index = 27 }, // Rhyperior
            new(60,10,5) { Species = 095, Ability = A4, Moves = new[]{ 776, 444, 784, 201 }, Index = 27 }, // Onix

            new(30,03,2) { Species = 143, Ability = A4, Moves = new[]{ 034, 044, 280, 523 }, Index = 26, CanGigantamax = true }, // Snorlax
            new(60,10,5) { Species = 143, Ability = A4, Moves = new[]{ 034, 442, 242, 428 }, Index = 26, CanGigantamax = true }, // Snorlax
            new(17,01,1,SW) { Species = 869, Ability = A4, Moves = new[]{ 033, 186, 577, 230 }, Index = 26, CanGigantamax = true }, // Alcremie
            new(30,03,2,SW) { Species = 851, Ability = A4, Moves = new[]{ 044, 172, 489, 693 }, Index = 26, CanGigantamax = true }, // Centiskorch
            new(30,03,2,SW) { Species = 131, Ability = A4, Moves = new[]{ 352, 420, 109, 047 }, Index = 26, CanGigantamax = true }, // Lapras
            new(40,05,3,SW) { Species = 099, Ability = A4, Moves = new[]{ 534, 232, 023, 106 }, Index = 26, CanGigantamax = true }, // Kingler
            new(40,05,3,SW) { Species = 842, Ability = A4, Moves = new[]{ 787, 496, 406, 523 }, Index = 26, CanGigantamax = true }, // Appletun
            new(40,05,3,SW) { Species = 851, Ability = A4, Moves = new[]{ 141, 424, 422, 044 }, Index = 26, CanGigantamax = true }, // Centiskorch
            new(50,08,4,SW) { Species = 823, Ability = A4, Moves = new[]{ 413, 442, 269, 103 }, Index = 26, CanGigantamax = true }, // Corviknight
            new(50,08,4,SW) { Species = 861, Ability = A4, Moves = new[]{ 789, 793, 280, 409 }, Index = 26, CanGigantamax = true }, // Grimmsnarl
            new(50,08,4,SW) { Species = 569, Ability = A4, Moves = new[]{ 188, 499, 034, 707 }, Index = 26, CanGigantamax = true }, // Garbodor
            new(50,08,4,SW) { Species = 869, Ability = A4, Moves = new[]{ 577, 605, 105, 500 }, Index = 26, CanGigantamax = true }, // Alcremie
            new(60,10,5,SW) { Species = 131, Ability = A4, Moves = new[]{ 057, 196, 058, 329 }, Index = 26, CanGigantamax = true }, // Lapras
            new(60,10,5,SW) { Species = 849, Ability = A4, Moves = new[]{ 786, 506, 474, 409 }, Index = 26, CanGigantamax = true }, // Toxtricity
            new(60,10,5,SW) { Species = 094, Ability = A4, Moves = new[]{ 247, 482, 094, 196 }, Index = 26, CanGigantamax = true }, // Gengar
            new(60,10,5,SW) { Species = 884, Ability = A4, Moves = new[]{ 430, 406, 085, 334 }, Index = 26, CanGigantamax = true }, // Duraludon
            new(17,01,1,SH) { Species = 012, Ability = A4, Moves = new[]{ 405, 060, 016, 079 }, Index = 26, CanGigantamax = true }, // Butterfree
            new(30,03,2,SH) { Species = 826, Ability = A4, Moves = new[]{ 405, 060, 496, 095 }, Index = 26, CanGigantamax = true }, // Orbeetle
            new(30,03,2,SH) { Species = 068, Ability = A4, Moves = new[]{ 523, 490, 279, 233 }, Index = 26, CanGigantamax = true }, // Machamp
            new(40,05,3,SH) { Species = 826, Ability = A4, Moves = new[]{ 405, 094, 202, 247 }, Index = 26, CanGigantamax = true }, // Orbeetle
            new(40,05,3,SH) { Species = 841, Ability = A4, Moves = new[]{ 406, 788, 491, 334 }, Index = 26, CanGigantamax = true }, // Flapple
            new(40,05,3,SH) { Species = 844, Ability = A4, Moves = new[]{ 693, 529, 201, 091 }, Index = 26, CanGigantamax = true }, // Sandaconda
            new(50,08,4,SH) { Species = 834, Ability = A4, Moves = new[]{ 157, 710, 317, 334 }, Index = 26, CanGigantamax = true }, // Drednaw
            new(50,08,4,SH) { Species = 858, Ability = A4, Moves = new[]{ 605, 094, 595, 247 }, Index = 26, CanGigantamax = true }, // Hatterene
            new(50,08,4,SH) { Species = 006, Ability = A4, Moves = new[]{ 053, 403, 076, 257 }, Index = 26, CanGigantamax = true }, // Charizard
            new(50,08,4,SH) { Species = 012, Ability = A4, Moves = new[]{ 405, 403, 527, 078 }, Index = 26, CanGigantamax = true }, // Butterfree
            new(60,10,5,SH) { Species = 849, Ability = A4, Moves = new[]{ 786, 506, 599, 409 }, Index = 26, Form = 1, CanGigantamax = true }, // Toxtricity-1
            new(60,10,5,SH) { Species = 839, Ability = A4, Moves = new[]{ 246, 053, 157, 523 }, Index = 26, CanGigantamax = true }, // Coalossal
            new(60,10,5,SH) { Species = 068, Ability = A4, Moves = new[]{ 238, 007, 008, 089 }, Index = 26, CanGigantamax = true }, // Machamp
            new(60,10,5,SH) { Species = 879, Ability = A4, Moves = new[]{ 442, 583, 438, 089 }, Index = 26, CanGigantamax = true }, // Copperajah

            new(17,01,1) { Species = 143, Ability = A4, Moves = new[]{ 033, 044, 122, 111 }, Index = 25, CanGigantamax = true }, // Snorlax
          //new(40,05,3) { Species = 807, Ability = A0, Moves = new[]{ 085, 007, 512, 280 }, Index = 25, Shiny = Shiny.Never }, // Zeraora
          //new(50,08,4) { Species = 807, Ability = A0, Moves = new[]{ 085, 007, 200, 370 }, Index = 25, Shiny = Shiny.Never }, // Zeraora
          //new(60,10,5) { Species = 807, Ability = A0, Moves = new[]{ 009, 299, 200, 370 }, Index = 25, Shiny = Shiny.Never }, // Zeraora
          //new(100,10,6) { Species = 807, Ability = A0, Moves = new[]{ 435, 299, 200, 370 }, Index = 25, Shiny = Shiny.Always }, // Zeraora
            new(60,10,5) { Species = 143, Ability = A4, Moves = new[]{ 034, 442, 242, 428 }, Index = 25, CanGigantamax = true }, // Snorlax
            new(30,03,2,SW) { Species = 131, Ability = A4, Moves = new[]{ 352, 420, 109, 047 }, Index = 25, CanGigantamax = true }, // Lapras
            new(40,05,3,SW) { Species = 099, Ability = A4, Moves = new[]{ 534, 232, 023, 106 }, Index = 25, CanGigantamax = true }, // Kingler
            new(40,05,3,SW) { Species = 842, Ability = A4, Moves = new[]{ 787, 496, 406, 523 }, Index = 25, CanGigantamax = true }, // Appletun
            new(40,05,3,SW) { Species = 851, Ability = A4, Moves = new[]{ 141, 424, 422, 044 }, Index = 25, CanGigantamax = true }, // Centiskorch
            new(50,08,4,SW) { Species = 823, Ability = A4, Moves = new[]{ 413, 442, 269, 103 }, Index = 25, CanGigantamax = true }, // Corviknight
            new(50,08,4,SW) { Species = 861, Ability = A4, Moves = new[]{ 789, 793, 280, 409 }, Index = 25, CanGigantamax = true }, // Grimmsnarl
            new(50,08,4,SW) { Species = 569, Ability = A4, Moves = new[]{ 188, 499, 034, 707 }, Index = 25, CanGigantamax = true }, // Garbodor
            new(50,08,4,SW) { Species = 869, Ability = A4, Moves = new[]{ 577, 605, 105, 500 }, Index = 25, CanGigantamax = true }, // Alcremie
            new(60,10,5,SW) { Species = 131, Ability = A4, Moves = new[]{ 057, 196, 058, 329 }, Index = 25, CanGigantamax = true }, // Lapras
            new(60,10,5,SW) { Species = 849, Ability = A4, Moves = new[]{ 786, 506, 474, 409 }, Index = 25, CanGigantamax = true }, // Toxtricity
            new(60,10,5,SW) { Species = 094, Ability = A4, Moves = new[]{ 247, 482, 094, 196 }, Index = 25, CanGigantamax = true }, // Gengar
            new(60,10,5,SW) { Species = 884, Ability = A4, Moves = new[]{ 430, 406, 085, 334 }, Index = 25, CanGigantamax = true }, // Duraludon
            new(30,03,2,SH) { Species = 068, Ability = A4, Moves = new[]{ 523, 490, 279, 233 }, Index = 25, CanGigantamax = true }, // Machamp
            new(40,05,3,SH) { Species = 826, Ability = A4, Moves = new[]{ 405, 094, 202, 247 }, Index = 25, CanGigantamax = true }, // Orbeetle
            new(40,05,3,SH) { Species = 841, Ability = A4, Moves = new[]{ 406, 788, 491, 334 }, Index = 25, CanGigantamax = true }, // Flapple
            new(40,05,3,SH) { Species = 844, Ability = A4, Moves = new[]{ 693, 529, 201, 091 }, Index = 25, CanGigantamax = true }, // Sandaconda
            new(50,08,4,SH) { Species = 834, Ability = A4, Moves = new[]{ 157, 710, 317, 334 }, Index = 25, CanGigantamax = true }, // Drednaw
            new(50,08,4,SH) { Species = 858, Ability = A4, Moves = new[]{ 605, 094, 595, 247 }, Index = 25, CanGigantamax = true }, // Hatterene
            new(50,08,4,SH) { Species = 006, Ability = A4, Moves = new[]{ 053, 403, 076, 257 }, Index = 25, CanGigantamax = true }, // Charizard
            new(50,08,4,SH) { Species = 012, Ability = A4, Moves = new[]{ 405, 403, 527, 078 }, Index = 25, CanGigantamax = true }, // Butterfree
            new(60,10,5,SH) { Species = 849, Ability = A4, Moves = new[]{ 786, 506, 599, 409 }, Index = 25, Form = 1, CanGigantamax = true }, // Toxtricity-1
            new(60,10,5,SH) { Species = 839, Ability = A4, Moves = new[]{ 246, 053, 157, 523 }, Index = 25, CanGigantamax = true }, // Coalossal
            new(60,10,5,SH) { Species = 068, Ability = A4, Moves = new[]{ 238, 007, 008, 089 }, Index = 25, CanGigantamax = true }, // Machamp
            new(60,10,5,SH) { Species = 879, Ability = A4, Moves = new[]{ 442, 583, 438, 089 }, Index = 25, CanGigantamax = true }, // Copperajah
        };
    }
}
