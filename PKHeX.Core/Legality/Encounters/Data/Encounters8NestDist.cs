using static PKHeX.Core.GameVersion;

namespace PKHeX.Core
{
    // Distribution Nest Encounters (BCAT)
    internal static partial class Encounters8Nest
    {
        // For distribution encounters, all commented out entries are duplicate with a prior distribution encounter. Only one encounter is necessary for matching purposes.

        internal static readonly EncounterStatic8ND[] Dist_Common =
        {
            // 11/15 - Butterfree
            new(40,05,3) { Species = 012, Ability = A4, Moves = new[]{ 676, 403, 202, 527 } }, // Butterfree
            new(40,05,3) { Species = 012, Ability = A4, Moves = new[]{ 676, 403, 202, 527 }, CanGigantamax = true }, // Butterfree
            new(50,08,4) { Species = 012, Ability = A4, Moves = new[]{ 405, 403, 527, 078 } }, // Butterfree
            new(50,08,4) { Species = 012, Ability = A4, Moves = new[]{ 405, 403, 527, 078 }, CanGigantamax = true }, // Butterfree

            // 12/03 - Snorlax
            new(17,01,1) { Species = 446, Ability = A4, Moves = new[]{ 033, 044, 122, 111 } }, // Munchlax
            new(17,01,1) { Species = 446, Ability = A2, Moves = new[]{ 033, 044, 122, 111 } }, // Munchlax
            new(30,03,2) { Species = 143, Ability = A4, Moves = new[]{ 034, 242, 118, 111 } }, // Snorlax
            new(30,03,2) { Species = 143, Ability = A2, Moves = new[]{ 034, 242, 118, 111 } }, // Snorlax
          //new(40,05,3) { Species = 012, Ability = A4, Moves = new[]{ 676, 403, 202, 527 }, CanGigantamax = true }, // Butterfree
            new(40,05,3) { Species = 143, Ability = A4, Moves = new[]{ 034, 667, 242, 281 } }, // Snorlax
            new(40,05,3) { Species = 143, Ability = A4, Moves = new[]{ 034, 667, 242, 281 }, CanGigantamax = true }, // Snorlax
          //new(50,08,4) { Species = 012, Ability = A4, Moves = new[]{ 405, 403, 527, 078 }, CanGigantamax = true }, // Butterfree
            new(50,08,4) { Species = 143, Ability = A4, Moves = new[]{ 034, 667, 280, 523 } }, // Snorlax
            new(50,08,4) { Species = 143, Ability = A4, Moves = new[]{ 034, 667, 280, 523 }, CanGigantamax = true }, // Snorlax
            new(60,10,5) { Species = 143, Ability = A4, Moves = new[]{ 034, 442, 242, 428 } }, // Snorlax
            new(70,10,5) { Species = 143, Ability = A4, Moves = new[]{ 034, 442, 242, 428 }, CanGigantamax = true }, // Snorlax

            // 12/20 - Delibird
          //new(17,01,1) { Species = 446, Ability = A4, Moves = new[]{ 033, 044, 122, 111 } }, // Munchlax
            new(17,01,1) { Species = 225, Ability = A4, Moves = new[]{ 217, 229, 098, 420 } }, // Delibird
          //new(30,03,2) { Species = 143, Ability = A4, Moves = new[]{ 034, 242, 118, 111 } }, // Snorlax
            new(30,03,2) { Species = 225, Ability = A4, Moves = new[]{ 217, 065, 034, 693 } }, // Delibird
          //new(40,05,3) { Species = 012, Ability = A4, Moves = new[]{ 676, 403, 202, 527 }, CanGigantamax = true }, // Butterfree
            new(40,05,3) { Species = 225, Ability = A4, Moves = new[]{ 217, 065, 280, 196 } }, // Delibird
          //new(40,05,3) { Species = 143, Ability = A4, Moves = new[]{ 034, 667, 242, 281 }, CanGigantamax = true }, // Snorlax
          //new(50,08,4) { Species = 012, Ability = A4, Moves = new[]{ 405, 403, 527, 078 }, CanGigantamax = true }, // Butterfree
            new(50,08,4) { Species = 225, Ability = A4, Moves = new[]{ 217, 059, 034, 280 } }, // Delibird
          //new(50,08,4) { Species = 143, Ability = A4, Moves = new[]{ 034, 667, 280, 523 }, CanGigantamax = true }, // Snorlax
            new(70,10,5) { Species = 225, Ability = A4, Moves = new[]{ 217, 059, 065, 280 } }, // Delibird
          //new(70,10,5) { Species = 143, Ability = A4, Moves = new[]{ 034, 442, 242, 428 }, CanGigantamax = true }, // Snorlax

            // 12/30 - Magikarp
            new(17,01,1) { Species = 129, Ability = A4, Moves = new[]{ 150, 033, 000, 000 } }, // Magikarp
            new(17,01,1) { Species = 446, Ability = A4, Moves = new[]{ 033, 044, 122, 111 } }, // Munchlax
            new(30,03,2) { Species = 129, Ability = A4, Moves = new[]{ 150, 033, 175, 000 } }, // Magikarp
          //new(30,03,2) { Species = 143, Ability = A4, Moves = new[]{ 034, 242, 118, 111 } }, // Snorlax
          //new(40,05,3) { Species = 012, Ability = A4, Moves = new[]{ 676, 403, 202, 527 }, CanGigantamax = true }, // Butterfree
            new(40,05,3) { Species = 129, Ability = A4, Moves = new[]{ 150, 033, 175, 000 } }, // Magikarp
          //new(40,05,3) { Species = 143, Ability = A4, Moves = new[]{ 034, 667, 242, 281 }, CanGigantamax = true }, // Snorlax
          //new(50,08,4) { Species = 012, Ability = A4, Moves = new[]{ 405, 403, 527, 078 }, CanGigantamax = true }, // Butterfree
            new(50,08,4) { Species = 129, Ability = A4, Moves = new[]{ 150, 033, 175, 000 } }, // Magikarp
          //new(50,08,4) { Species = 143, Ability = A4, Moves = new[]{ 034, 667, 280, 523 }, CanGigantamax = true }, // Snorlax
            new(60,10,5) { Species = 129, Ability = A4, Moves = new[]{ 150, 033, 175, 340 } }, // Magikarp
            new(70,10,5) { Species = 129, Ability = A4, Moves = new[]{ 150, 033, 175, 340 } }, // Magikarp
          //new(70,10,5) { Species = 143, Ability = A4, Moves = new[]{ 034, 442, 242, 428 }, CanGigantamax = true }, // Snorlax

            // 1/8 - Applin
            new(17,01,1) { Species = 840, Ability = A4, Moves = new[]{ 110, 310, 389, 213 } }, // Applin
            new(17,01,1) { Species = 840, Ability = A2, Moves = new[]{ 110, 310, 389, 213 } }, // Applin
            new(17,01,1) { Species = 868, Ability = A4, Moves = new[]{ 577, 033, 186, 263 } }, // Milcery
            new(17,01,1) { Species = 868, Ability = A2, Moves = new[]{ 577, 033, 186, 263 } }, // Milcery
            new(30,03,2) { Species = 869, Ability = A4, Moves = new[]{ 577, 213, 033, 186 }, CanGigantamax = true }, // Alcremie

            // 1/31 - Milcery
            new(17,01,1) { Species = 868, Ability = A4, Moves = new[]{ 033, 186, 577, 496 }, CanGigantamax = true }, // Milcery
            new(30,03,2) { Species = 868, Ability = A4, Moves = new[]{ 577, 186, 263, 500 }, CanGigantamax = true }, // Milcery
            new(40,05,3) { Species = 868, Ability = A4, Moves = new[]{ 577, 605, 496, 213 }, CanGigantamax = true }, // Milcery
            new(50,08,4) { Species = 868, Ability = A4, Moves = new[]{ 577, 605, 496, 500 }, CanGigantamax = true }, // Milcery
            new(60,10,5) { Species = 868, Ability = A4, Moves = new[]{ 577, 605, 496, 500 }, CanGigantamax = true }, // Milcery

            // 2/6 - Toxtricity - Same as Milcery 1/31 (above)

            // 2/17 - Toxel
            new(17,01,1) { Species = 848, Ability = A4, Moves = new[]{ 609, 051, 496, 715 } }, // Toxel

            // 2/26 - Mewtwo
            new(17,01,1) { Species = 001, Ability = A4, Moves = new[]{ 033, 045, 022, 074 } }, // Bulbasaur
            new(17,01,1) { Species = 004, Ability = A4, Moves = new[]{ 010, 045, 052, 108 } }, // Charmander
            new(17,01,1) { Species = 007, Ability = A4, Moves = new[]{ 033, 039, 055, 110 } }, // Squirtle
          //new(17,01,1) { Species = 848, Ability = A4, Moves = new[]{ 609, 051, 496, 715 } }, // Toxel
            new(30,03,2) { Species = 001, Ability = A4, Moves = new[]{ 033, 022, 073, 075 } }, // Bulbasaur
            new(30,03,2) { Species = 004, Ability = A4, Moves = new[]{ 010, 052, 225, 424 } }, // Charmander
            new(30,03,2) { Species = 007, Ability = A4, Moves = new[]{ 033, 055, 044, 352 } }, // Squirtle
            new(40,05,3) { Species = 001, Ability = A4, Moves = new[]{ 073, 075, 077, 402 } }, // Bulbasaur
            new(40,05,3) { Species = 004, Ability = A4, Moves = new[]{ 424, 225, 163, 108 } }, // Charmander
            new(40,05,3) { Species = 007, Ability = A4, Moves = new[]{ 055, 229, 044, 352 } }, // Squirtle
            new(50,08,4) { Species = 002, Ability = A4, Moves = new[]{ 188, 412, 075, 034 } }, // Ivysaur
            new(50,08,4) { Species = 005, Ability = A4, Moves = new[]{ 257, 242, 009, 053 } }, // Charmeleon
            new(50,08,4) { Species = 008, Ability = A4, Moves = new[]{ 330, 396, 503, 428 } }, // Wartortle
            // sadly, can't capture Mewtwo.
            // new(100,10,6) { Species = 150, Ability = 1, Moves = new[]{ 540, 053, 396, 059 }, Nature = Nature.Timid, Shiny = Shiny.Never }, // Mewtwo
            // new(100,10,6) { Species = 150, Ability = 1, Moves = new[]{ 428, 007, 089, 280 }, Nature = Nature.Jolly, Shiny = Shiny.Never }, // Mewtwo
            // new(100,10,6) { Species = 150, Ability = 1, Moves = new[]{ 540, 126, 411, 059 }, Nature = Nature.Timid, Shiny = Shiny.Never }, // Mewtwo

            // 3/8 - Gengar/Machamp
          //new(17,01,1) { Species = 446, Ability = A4, Moves = new[]{ 033, 044, 122, 111 } }, // Munchlax
            new(17,01,1) { Species = 092, Ability = A4, Moves = new[]{ 122, 109, 095, 371 } }, // Gastly
            new(17,01,1) { Species = 066, Ability = A4, Moves = new[]{ 067, 043, 116, 279 } }, // Machop
          //new(30,03,2) { Species = 143, Ability = A4, Moves = new[]{ 034, 242, 118, 111 } }, // Snorlax
            new(30,03,2) { Species = 093, Ability = A4, Moves = new[]{ 325, 095, 122, 101 } }, // Haunter
            new(30,03,2) { Species = 067, Ability = A4, Moves = new[]{ 067, 490, 282, 233 } }, // Machoke
          //new(40,05,3) { Species = 143, Ability = A4, Moves = new[]{ 034, 667, 242, 281 } }, // Snorlax
          //new(40,05,3) { Species = 143, Ability = A4, Moves = new[]{ 034, 667, 242, 281 }, CanGigantamax = true }, // Snorlax
            new(40,05,3) { Species = 094, Ability = A4, Moves = new[]{ 506, 188, 085, 261 } }, // Gengar
            new(40,05,3) { Species = 094, Ability = A4, Moves = new[]{ 506, 188, 085, 261 }, CanGigantamax = true }, // Gengar
            new(40,05,3) { Species = 068, Ability = A4, Moves = new[]{ 279, 667, 008, 157 } }, // Machamp
            new(40,05,3) { Species = 068, Ability = A4, Moves = new[]{ 279, 667, 008, 157 }, CanGigantamax = true }, // Machamp
          //new(50,08,4) { Species = 143, Ability = A4, Moves = new[]{ 034, 667, 280, 523 } }, // Snorlax
          //new(50,08,4) { Species = 143, Ability = A4, Moves = new[]{ 034, 667, 280, 523 }, CanGigantamax = true }, // Snorlax
            new(50,08,4) { Species = 094, Ability = A4, Moves = new[]{ 247, 482, 411, 605 } }, // Gengar
            new(50,08,4) { Species = 094, Ability = A4, Moves = new[]{ 247, 482, 411, 605 }, CanGigantamax = true }, // Gengar
            new(50,08,4) { Species = 068, Ability = A4, Moves = new[]{ 280, 444, 371, 523 } }, // Machamp
            new(50,08,4) { Species = 068, Ability = A4, Moves = new[]{ 280, 444, 371, 523 }, CanGigantamax = true }, // Machamp
          //new(60,10,5) { Species = 143, Ability = A4, Moves = new[]{ 034, 442, 242, 428 } }, // Snorlax
            new(60,10,5) { Species = 143, Ability = A4, Moves = new[]{ 034, 442, 242, 428 }, CanGigantamax = true }, // Snorlax

            // 3/18 Exclusives & Food
            new(17,01,1) { Species = 588, Ability = A4, Moves = new[]{ 064, 043, 210, 491 } }, // Karrablast
            new(17,01,1) { Species = 616, Ability = A4, Moves = new[]{ 071, 051, 174, 522 } }, // Shelmet
          //new(17,01,1) { Species = 092, Ability = A4, Moves = new[]{ 122, 109, 095, 371 } }, // Gastly
            new(17,01,1) { Species = 871, Ability = A4, Moves = new[]{ 084, 064, 055, 031 } }, // Pincurchin
          //new(17,01,1) { Species = 066, Ability = A4, Moves = new[]{ 067, 043, 116, 279 } }, // Machop
          //new(30,03,2) { Species = 143, Ability = A4, Moves = new[]{ 034, 242, 118, 111 } }, // Snorlax
          //new(30,03,2) { Species = 093, Ability = A4, Moves = new[]{ 325, 095, 122, 101 } }, // Haunter
            new(30,03,2) { Species = 871, Ability = A4, Moves = new[]{ 209, 061, 086, 506 } }, // Pincurchin
          //new(30,03,2) { Species = 067, Ability = A4, Moves = new[]{ 067, 490, 282, 233 } }, // Machoke
          //new(40,05,3) { Species = 143, Ability = A4, Moves = new[]{ 034, 667, 242, 281 }, CanGigantamax = true }, // Snorlax
          //new(40,05,3) { Species = 094, Ability = A4, Moves = new[]{ 506, 188, 085, 261 }, CanGigantamax = true }, // Gengar
            new(40,05,3) { Species = 871, Ability = A2, Moves = new[]{ 085, 503, 398, 716 } }, // Pincurchin
          //new(40,05,3) { Species = 068, Ability = A4, Moves = new[]{ 279, 667, 008, 157 }, CanGigantamax = true }, // Machamp
            new(50,08,4) { Species = 617, Ability = A2, Moves = new[]{ 405, 522, 188, 202 } }, // Accelgor
          //new(50,08,4) { Species = 143, Ability = A4, Moves = new[]{ 034, 667, 280, 523 }, CanGigantamax = true }, // Snorlax
            new(50,08,4) { Species = 589, Ability = A2, Moves = new[]{ 442, 224, 529, 398 } }, // Escavalier
          //new(50,08,4) { Species = 094, Ability = A4, Moves = new[]{ 247, 482, 411, 605 }, CanGigantamax = true }, // Gengar
            new(50,08,4) { Species = 871, Ability = A2, Moves = new[]{ 435, 330, 474, 367 } }, // Pincurchin
          //new(50,08,4) { Species = 068, Ability = A4, Moves = new[]{ 280, 444, 371, 523 }, CanGigantamax = true }, // Machamp
          //new(60,10,5) { Species = 143, Ability = A4, Moves = new[]{ 034, 442, 242, 428 }, CanGigantamax = true }, // Snorlax

            // 3/25 Charizard
            new(17,01,1) { Species = 878, Ability = A4, Moves = new[]{ 091, 249, 205, 523 } }, // Cufant
            new(17,01,1) { Species = 568, Ability = A4, Moves = new[]{ 001, 499, 491, 133 } }, // Trubbish
            new(17,01,1) { Species = 004, Ability = A4, Moves = new[]{ 424, 052, 108, 225 } }, // Charmander
            new(17,01,1) { Species = 884, Ability = A4, Moves = new[]{ 468, 249, 043, 232 } }, // Duraludon
            new(30,03,2) { Species = 878, Ability = A4, Moves = new[]{ 334, 091, 205, 523 } }, // Cufant
            new(30,03,2) { Species = 568, Ability = A4, Moves = new[]{ 036, 499, 124, 133 } }, // Trubbish
            new(30,03,2) { Species = 005, Ability = A4, Moves = new[]{ 053, 163, 108, 225 } }, // Charmeleon
            new(30,03,2) { Species = 884, Ability = A4, Moves = new[]{ 468, 249, 784, 232 } }, // Duraludon
            new(40,05,3) { Species = 879, Ability = A4, Moves = new[]{ 334, 070, 442, 523 }, CanGigantamax = true }, // Copperajah
            new(40,05,3) { Species = 569, Ability = A4, Moves = new[]{ 188, 499, 034, 707 }, CanGigantamax = true }, // Garbodor
            new(40,05,3) { Species = 006, Ability = A4, Moves = new[]{ 053, 403, 108, 225 }, CanGigantamax = true }, // Charizard
            new(40,05,3) { Species = 884, Ability = A4, Moves = new[]{ 442, 555, 784, 334 }, CanGigantamax = true }, // Duraludon
            new(50,08,4) { Species = 879, Ability = A4, Moves = new[]{ 667, 442, 438, 523 }, CanGigantamax = true }, // Copperajah
            new(50,08,4) { Species = 569, Ability = A4, Moves = new[]{ 441, 499, 402, 707 }, CanGigantamax = true }, // Garbodor
            new(50,08,4) { Species = 006, Ability = A4, Moves = new[]{ 053, 403, 076, 257 }, CanGigantamax = true }, // Charizard
            new(50,08,4) { Species = 884, Ability = A4, Moves = new[]{ 337, 430, 784, 776 }, CanGigantamax = true }, // Duraludon

            // 4/09 - Easter Eggs
            new(40,05,2) { Species = 848, Ability = A4, Moves = new[]{ 609, 051, 175, 715 } }, // Toxel
            new(40,05,2) { Species = 458, Ability = A4, Moves = new[]{ 403, 061, 469, 503 } }, // Mantyke
            new(40,05,3) { Species = 406, Ability = A4, Moves = new[]{ 071, 074, 078, 188 } }, // Budew
            new(40,05,3) { Species = 236, Ability = A4, Moves = new[]{ 280, 157, 252, 116 } }, // Tyrogue
            new(40,05,3) { Species = 438, Ability = A4, Moves = new[]{ 317, 389, 157, 313 } }, // Bonsly
            new(40,05,3) { Species = 447, Ability = A4, Moves = new[]{ 014, 009, 232, 249 } }, // Riolu
            new(40,05,3) { Species = 446, Ability = A4, Moves = new[]{ 034, 089, 044, 122 } }, // Munchlax
            new(40,05,3) { Species = 439, Ability = A4, Moves = new[]{ 389, 060, 182, 085 } }, // Mime Jr.
            new(50,08,4) { Species = 175, Ability = A4, Moves = new[]{ 605, 219, 246, 053 } }, // Togepi
            new(50,08,4) { Species = 360, Ability = A4, Moves = new[]{ 068, 243, 204, 133 } }, // Wynaut
            new(50,08,4) { Species = 173, Ability = A4, Moves = new[]{ 574, 005, 113, 034 } }, // Cleffa
            new(50,08,4) { Species = 172, Ability = A4, Moves = new[]{ 583, 417, 085, 186 } }, // Pichu
            new(60,10,5) { Species = 132, Ability = A4, Moves = new[]{ 144, 000, 000, 000 } }, // Ditto

            // 4/27 - Meta
            new(17,01,1) { Species = 246, Ability = A4, Moves = new[]{ 157, 044, 184, 033 } }, // Larvitar
            new(17,01,1) { Species = 529, Ability = A4, Moves = new[]{ 189, 232, 010, 468 } }, // Drilbur
            new(17,01,1) { Species = 546, Ability = A4, Moves = new[]{ 584, 078, 075, 071 } }, // Cottonee
            new(17,01,1) { Species = 885, Ability = A4, Moves = new[]{ 611, 098, 310, 044 } }, // Dreepy
            new(17,01,1) { Species = 175, Ability = A4, Moves = new[]{ 204, 577, 113, 791 } }, // Togepi
            new(30,03,2) { Species = 247, Ability = A4, Moves = new[]{ 157, 242, 334, 707 } }, // Pupitar
            new(30,03,2) { Species = 529, Ability = A4, Moves = new[]{ 189, 232, 157, 306 } }, // Drilbur
            new(30,03,2) { Species = 546, Ability = A4, Moves = new[]{ 202, 075, 204, 077 } }, // Cottonee
            new(30,03,2) { Species = 886, Ability = A4, Moves = new[]{ 097, 506, 372, 458 } }, // Drakloak
            new(30,03,2) { Species = 176, Ability = A4, Moves = new[]{ 584, 038, 113, 791 } }, // Togetic
            new(40,05,3) { Species = 248, Ability = A4, Moves = new[]{ 444, 242, 334, 089 } }, // Tyranitar
            new(40,05,3) { Species = 530, Ability = A4, Moves = new[]{ 529, 232, 157, 032 } }, // Excadrill
            new(40,05,3) { Species = 547, Ability = A4, Moves = new[]{ 283, 585, 077, 366 } }, // Whimsicott
            new(40,05,3) { Species = 887, Ability = A4, Moves = new[]{ 751, 506, 349, 458 } }, // Dragapult
            new(40,05,3) { Species = 468, Ability = A4, Moves = new[]{ 605, 038, 246, 403 } }, // Togekiss
            new(50,08,4) { Species = 248, Ability = A4, Moves = new[]{ 444, 242, 442, 089 } }, // Tyranitar
            new(50,08,4) { Species = 530, Ability = A4, Moves = new[]{ 529, 442, 157, 032 } }, // Excadrill
            new(50,08,4) { Species = 547, Ability = A4, Moves = new[]{ 283, 585, 073, 366 } }, // Whimsicott
            new(50,08,4) { Species = 887, Ability = A4, Moves = new[]{ 751, 506, 349, 211 } }, // Dragapult
            new(50,08,4) { Species = 468, Ability = A4, Moves = new[]{ 605, 219, 246, 403 } }, // Togekiss
            new(60,10,5) { Species = 248, Ability = A4, Moves = new[]{ 444, 242, 442, 276 } }, // Tyranitar
            new(60,10,5) { Species = 530, Ability = A4, Moves = new[]{ 089, 442, 157, 032 } }, // Excadrill
            new(60,10,5) { Species = 547, Ability = A4, Moves = new[]{ 538, 585, 073, 366 } }, // Whimsicott
            new(60,10,5) { Species = 887, Ability = A4, Moves = new[]{ 751, 566, 349, 211 } }, // Dragapult
            new(60,10,5) { Species = 468, Ability = A4, Moves = new[]{ 605, 219, 246, 053 } }, // Togekiss

            // 5/11 - Gigantamax Pikachu
            new(17,01,1) { Species = 025, Ability = A4, Moves = new[]{ 084, 104, 486, 364 }, CanGigantamax = true }, // Pikachu
            new(30,03,2) { Species = 025, Ability = A4, Moves = new[]{ 021, 209, 097, 364 }, CanGigantamax = true }, // Pikachu
            new(40,05,3) { Species = 025, Ability = A4, Moves = new[]{ 021, 113, 085, 364 }, CanGigantamax = true }, // Pikachu
            new(50,08,4) { Species = 025, Ability = A4, Moves = new[]{ 087, 113, 085, 364 }, CanGigantamax = true }, // Pikachu
            new(50,08,4) { Species = 025, Ability = A4, Moves = new[]{ 087, 113, 057, 085 }, CanGigantamax = true }, // Pikachu
            new(60,10,5) { Species = 025, Ability = A4, Moves = new[]{ 087, 113, 057, 364 }, CanGigantamax = true }, // Pikachu
            new(60,10,5) { Species = 025, Ability = A4, Moves = new[]{ 087, 113, 057, 344 }, CanGigantamax = true }, // Pikachu

            // 5/18 - Gigantamax Eevee
            new(17,01,1) { Species = 133, Ability = A4, Moves = new[]{ 098, 270, 608, 028 }, CanGigantamax = true }, // Eevee
            new(30,03,2) { Species = 133, Ability = A4, Moves = new[]{ 129, 270, 608, 044 }, CanGigantamax = true }, // Eevee
            new(40,05,3) { Species = 133, Ability = A4, Moves = new[]{ 036, 270, 608, 044 }, CanGigantamax = true }, // Eevee
            new(40,05,3) { Species = 133, Ability = A4, Moves = new[]{ 036, 270, 608, 231 }, CanGigantamax = true }, // Eevee
            new(50,08,4) { Species = 133, Ability = A4, Moves = new[]{ 038, 270, 204, 044 }, CanGigantamax = true }, // Eevee
            new(50,08,4) { Species = 133, Ability = A4, Moves = new[]{ 038, 203, 204, 231 }, CanGigantamax = true }, // Eevee
            new(60,10,5) { Species = 133, Ability = A4, Moves = new[]{ 038, 270, 204, 231 }, CanGigantamax = true }, // Eevee
            new(60,10,5) { Species = 133, Ability = A4, Moves = new[]{ 387, 203, 204, 231 }, CanGigantamax = true }, // Eevee

            // 5/25 - Gigantamax Meowth
            new(17,01,1) { Species = 052, Ability = A4, Moves = new[]{ 252, 044, 010, 364 }, CanGigantamax = true }, // Meowth
            new(17,01,1) { Species = 052, Ability = A4, Moves = new[]{ 006, 044, 010, 364 }, CanGigantamax = true }, // Meowth
            new(30,03,2) { Species = 052, Ability = A4, Moves = new[]{ 252, 044, 269, 154 }, CanGigantamax = true }, // Meowth
            new(30,03,2) { Species = 052, Ability = A4, Moves = new[]{ 006, 044, 269, 154 }, CanGigantamax = true }, // Meowth
            new(40,05,3) { Species = 052, Ability = A4, Moves = new[]{ 252, 044, 417, 163 }, CanGigantamax = true }, // Meowth
            new(40,05,3) { Species = 052, Ability = A4, Moves = new[]{ 006, 044, 417, 163 }, CanGigantamax = true }, // Meowth
            new(50,08,4) { Species = 052, Ability = A4, Moves = new[]{ 252, 583, 417, 163 }, CanGigantamax = true }, // Meowth
            new(50,08,4) { Species = 052, Ability = A4, Moves = new[]{ 006, 583, 417, 163 }, CanGigantamax = true }, // Meowth
            new(60,10,5) { Species = 052, Ability = A4, Moves = new[]{ 252, 583, 417, 034 }, CanGigantamax = true }, // Meowth
            new(60,10,5) { Species = 052, Ability = A4, Moves = new[]{ 006, 583, 417, 034 }, CanGigantamax = true }, // Meowth

            // 6/2 - Gigantamax
            new(30,03,2) { Species = 143, Ability = A4, Moves = new[]{ 034, 044, 280, 523 }, CanGigantamax = true }, // Snorlax
          //new(60,10,5) { Species = 143, Ability = A4, Moves = new[]{ 034, 442, 242, 428 }, CanGigantamax = true }, // Snorlax

            new(17,01,1) { Species = 878, Ability = A4, Moves = new[]{ 523, 205, 045, 249 } }, // Cufant
            new(17,01,1) { Species = 208, Ability = A4, Moves = new[]{ 242, 442, 106, 422 } }, // Steelix
            new(17,01,1) { Species = 052, Ability = A4, Moves = new[]{ 232, 006, 242, 045 }, Form = 2 }, // Meowth-2
            new(17,01,1) { Species = 837, Ability = A4, Moves = new[]{ 229, 261, 479, 108 } }, // Rolycoly
            new(17,01,1) { Species = 111, Ability = A4, Moves = new[]{ 479, 523, 196, 182 } }, // Rhyhorn
            new(17,01,1) { Species = 095, Ability = A4, Moves = new[]{ 174, 225, 034, 106 } }, // Onix
            new(30,03,2) { Species = 878, Ability = A4, Moves = new[]{ 523, 023, 334, 249 } }, // Cufant
            new(30,03,2) { Species = 208, Ability = A4, Moves = new[]{ 157, 442, 328, 422 } }, // Steelix
            new(30,03,2) { Species = 863, Ability = A4, Moves = new[]{ 442, 006, 242, 269 } }, // Perrserker
            new(30,03,2) { Species = 838, Ability = A4, Moves = new[]{ 229, 488, 157, 108 } }, // Carkol
            new(30,03,2) { Species = 111, Ability = A4, Moves = new[]{ 350, 523, 196, 182 } }, // Rhyhorn
            new(30,03,2) { Species = 095, Ability = A4, Moves = new[]{ 776, 225, 034, 106 } }, // Onix
            new(40,05,3) { Species = 879, Ability = A4, Moves = new[]{ 070, 523, 334, 442 }, CanGigantamax = true }, // Copperajah
            new(40,05,3) { Species = 208, Ability = A4, Moves = new[]{ 157, 442, 328, 422 } }, // Steelix
            new(40,05,3) { Species = 863, Ability = A4, Moves = new[]{ 442, 006, 154, 269 } }, // Perrserker
            new(40,05,3) { Species = 839, Ability = A4, Moves = new[]{ 025, 488, 157, 108 }, CanGigantamax = true }, // Coalossal
            new(40,05,3) { Species = 112, Ability = A4, Moves = new[]{ 036, 529, 008, 182 } }, // Rhydon
            new(40,05,3) { Species = 095, Ability = A4, Moves = new[]{ 776, 225, 021, 201 } }, // Onix
            new(50,08,4) { Species = 879, Ability = A4, Moves = new[]{ 070, 523, 334, 442 }, CanGigantamax = true }, // Copperajah
            new(50,08,4) { Species = 208, Ability = A4, Moves = new[]{ 157, 231, 328, 422 } }, // Steelix
            new(50,08,4) { Species = 863, Ability = A4, Moves = new[]{ 442, 583, 154, 269 } }, // Perrserker
            new(50,08,4) { Species = 839, Ability = A4, Moves = new[]{ 025, 488, 157, 115 }, CanGigantamax = true }, // Coalossal
            new(50,08,4) { Species = 464, Ability = A4, Moves = new[]{ 350, 089, 008, 182 } }, // Rhyperior
            new(50,08,4) { Species = 095, Ability = A4, Moves = new[]{ 776, 225, 784, 201 } }, // Onix
            new(60,10,5) { Species = 879, Ability = A4, Moves = new[]{ 276, 089, 583, 442 }, CanGigantamax = true }, // Copperajah
            new(60,10,5) { Species = 208, Ability = A4, Moves = new[]{ 038, 231, 529, 422 } }, // Steelix
            new(60,10,5) { Species = 863, Ability = A4, Moves = new[]{ 442, 583, 370, 269 } }, // Perrserker
            new(60,10,5) { Species = 839, Ability = A4, Moves = new[]{ 076, 682, 157, 115 }, CanGigantamax = true }, // Coalossal
            new(60,10,5) { Species = 464, Ability = A4, Moves = new[]{ 444, 089, 008, 224 } }, // Rhyperior
            new(60,10,5) { Species = 095, Ability = A4, Moves = new[]{ 776, 444, 784, 201 } }, // Onix

            // 6/17 Zeraora Challenge
            new(17,01,1) { Species = 143, Ability = A4, Moves = new[]{ 033, 044, 122, 111 }, CanGigantamax = true }, // Snorlax
            //new(60,10,5) { Species = 143, Ability = A4, Moves = new[]{ 034, 442, 242, 428 }, CanGigantamax = true }, // Snorlax

            // 7/16 Sea Pokémon Invasion
            new(17,01,1) { Species = 320, Ability = A4, Moves = new[]{ 362, 034, 310, 054 } }, // Wailmer
            new(17,01,1) { Species = 098, Ability = A4, Moves = new[]{ 055, 341, 043, 232 } }, // Krabby
            new(17,01,1) { Species = 771, Ability = A4, Moves = new[]{ 240, 219, 213, 269 } }, // Pyukumuku
            new(17,01,1) { Species = 592, Ability = A4, Moves = new[]{ 352, 101, 071, 240 } }, // Frillish
            new(17,01,1) { Species = 458, Ability = A4, Moves = new[]{ 033, 017, 352, 469 } }, // Mantyke
            new(17,01,1) { Species = 318, Ability = A4, Moves = new[]{ 453, 305, 116, 044 } }, // Carvanha
            new(30,03,2) { Species = 320, Ability = A4, Moves = new[]{ 362, 034, 310, 054 } }, // Wailmer
            new(30,03,2) { Species = 098, Ability = A4, Moves = new[]{ 061, 341, 023, 232 } }, // Krabby
            new(30,03,2) { Species = 771, Ability = A4, Moves = new[]{ 240, 219, 213, 174 } }, // Pyukumuku
            new(30,03,2) { Species = 592, Ability = A4, Moves = new[]{ 362, 101, 071, 240 } }, // Frillish
            new(30,03,2) { Species = 458, Ability = A4, Moves = new[]{ 029, 017, 061, 469 } }, // Mantyke
            new(30,03,2) { Species = 319, Ability = A4, Moves = new[]{ 453, 400, 423, 044 } }, // Sharpedo
            new(40,05,3) { Species = 321, Ability = A4, Moves = new[]{ 362, 034, 340, 568 } }, // Wailord
            new(40,05,3) { Species = 099, Ability = A4, Moves = new[]{ 534, 341, 021, 014 } }, // Kingler
            new(40,05,3) { Species = 771, Ability = A4, Moves = new[]{ 240, 219, 213, 174 } }, // Pyukumuku
            new(40,05,3) { Species = 593, Ability = A4, Moves = new[]{ 362, 247, 071, 151 } }, // Jellicent
            new(40,05,3) { Species = 226, Ability = A4, Moves = new[]{ 029, 403, 060, 331 } }, // Mantine
            new(40,05,3) { Species = 319, Ability = A4, Moves = new[]{ 453, 242, 423, 044 } }, // Sharpedo
            new(50,08,4) { Species = 321, Ability = A4, Moves = new[]{ 056, 034, 340, 133 } }, // Wailord
            new(50,08,4) { Species = 099, Ability = A4, Moves = new[]{ 534, 341, 359, 014 }, CanGigantamax = true }, // Kingler
            new(50,08,4) { Species = 771, Ability = A4, Moves = new[]{ 240, 219, 213, 174 } }, // Pyukumuku
            new(50,08,4) { Species = 593, Ability = A4, Moves = new[]{ 056, 247, 071, 151 } }, // Jellicent
            new(50,08,4) { Species = 226, Ability = A4, Moves = new[]{ 036, 403, 060, 331 } }, // Mantine
            new(50,08,4) { Species = 319, Ability = A4, Moves = new[]{ 057, 242, 423, 044 } }, // Sharpedo
            new(60,10,5) { Species = 321, Ability = A4, Moves = new[]{ 503, 034, 340, 133 }, Shiny = Shiny.Always }, // Wailord
            new(60,10,5) { Species = 321, Ability = A4, Moves = new[]{ 056, 034, 340, 133 } }, // Wailord
            new(60,10,5) { Species = 771, Ability = A4, Moves = new[]{ 092, 599, 213, 174 } }, // Pyukumuku
            new(60,10,5) { Species = 593, Ability = A4, Moves = new[]{ 056, 058, 605, 433 } }, // Jellicent
            new(60,10,5) { Species = 226, Ability = A4, Moves = new[]{ 036, 403, 060, 331 } }, // Mantine
            new(60,10,5) { Species = 319, Ability = A4, Moves = new[]{ 057, 242, 423, 305 } }, // Sharpedo

            // 7/31 - Water
            new(17,01,1) { Species = 833, Ability = A4, Moves = new[]{ 055, 033, 044, 240 } }, // Chewtle
            new(17,01,1) { Species = 349, Ability = A4, Moves = new[]{ 150, 033, 175, 057 } }, // Feebas
            new(17,01,1) { Species = 194, Ability = A4, Moves = new[]{ 341, 021, 039, 055 } }, // Wooper
            new(17,01,1) { Species = 843, Ability = A4, Moves = new[]{ 028, 035, 523, 693 } }, // Silicobra
            new(17,01,1) { Species = 449, Ability = A4, Moves = new[]{ 341, 328, 044, 033 } }, // Hippopotas
            new(17,01,1) { Species = 422, Ability = A4, Moves = new[]{ 352, 106, 189, 055 }, Form = 1 }, // Shellos-1
            new(30,03,2) { Species = 834, Ability = A4, Moves = new[]{ 157, 534, 317, 055 }, CanGigantamax = true }, // Drednaw
            new(30,03,2) { Species = 349, Ability = A4, Moves = new[]{ 057, 033, 175, 150 } }, // Feebas
            new(30,03,2) { Species = 195, Ability = A4, Moves = new[]{ 341, 021, 401, 055 } }, // Quagsire
            new(30,03,2) { Species = 843, Ability = A4, Moves = new[]{ 091, 029, 523, 693 } }, // Silicobra
            new(30,03,2) { Species = 449, Ability = A4, Moves = new[]{ 341, 036, 044, 242 } }, // Hippopotas
            new(30,03,2) { Species = 423, Ability = A4, Moves = new[]{ 189, 352, 246, 106 }, Form = 1 }, // Gastrodon-1
            new(40,05,3) { Species = 834, Ability = A4, Moves = new[]{ 157, 534, 317, 055 }, CanGigantamax = true }, // Drednaw
            new(40,05,3) { Species = 350, Ability = A4, Moves = new[]{ 057, 239, 034, 574 } }, // Milotic
            new(40,05,3) { Species = 195, Ability = A4, Moves = new[]{ 341, 021, 401, 005 } }, // Quagsire
            new(40,05,3) { Species = 844, Ability = A4, Moves = new[]{ 693, 523, 201, 091 }, CanGigantamax = true }, // Sandaconda
            new(40,05,3) { Species = 450, Ability = A4, Moves = new[]{ 341, 422, 036, 242 } }, // Hippowdon
            new(40,05,3) { Species = 423, Ability = A4, Moves = new[]{ 414, 352, 246, 106 }, Form = 1 }, // Gastrodon-1
            new(50,08,4) { Species = 834, Ability = A4, Moves = new[]{ 157, 710, 317, 334 }, CanGigantamax = true }, // Drednaw
            new(50,08,4) { Species = 350, Ability = A4, Moves = new[]{ 057, 231, 034, 574 } }, // Milotic
            new(50,08,4) { Species = 195, Ability = A4, Moves = new[]{ 341, 280, 401, 005 } }, // Quagsire
            new(50,08,4) { Species = 844, Ability = A4, Moves = new[]{ 693, 529, 201, 091 }, CanGigantamax = true }, // Sandaconda
            new(50,08,4) { Species = 450, Ability = A4, Moves = new[]{ 089, 422, 036, 242 } }, // Hippowdon
            new(50,08,4) { Species = 423, Ability = A4, Moves = new[]{ 414, 503, 311, 106 }, Form = 1 }, // Gastrodon-1
            new(60,10,5) { Species = 834, Ability = A4, Moves = new[]{ 157, 710, 317, 334 }, CanGigantamax = true }, // Drednaw
            new(60,10,5) { Species = 350, Ability = A4, Moves = new[]{ 503, 231, 034, 574 } }, // Milotic
            new(60,10,5) { Species = 195, Ability = A4, Moves = new[]{ 089, 280, 401, 005 } }, // Quagsire
            new(60,10,5) { Species = 844, Ability = A4, Moves = new[]{ 693, 529, 201, 091 }, CanGigantamax = true }, // Sandaconda
            new(60,10,5) { Species = 450, Ability = A4, Moves = new[]{ 089, 422, 231, 242 } }, // Hippowdon
            new(60,10,5) { Species = 423, Ability = A4, Moves = new[]{ 414, 503, 311, 352 }, Form = 1 }, // Gastrodon-1

            // 8/5 - Pikachu Mass Appearance
            new(17,01,1) { Species = 025, Ability = A4, Moves = new[]{ 084, 098, 204, 086 } }, // Pikachu
            new(17,01,1) { Species = 026, Ability = A4, Moves = new[]{ 009, 129, 280, 204 } }, // Raichu
            new(17,01,1) { Species = 026, Ability = A4, Moves = new[]{ 009, 129, 280, 204 }, Form = 1 }, // Raichu-1
            new(17,01,1) { Species = 172, Ability = A4, Moves = new[]{ 589, 609, 085, 186 } }, // Pichu
            new(17,01,1) { Species = 778, Ability = A4, Moves = new[]{ 086, 452, 425, 010 } }, // Mimikyu
            new(30,03,2) { Species = 025, Ability = A4, Moves = new[]{ 209, 097, 204, 086 } }, // Pikachu
            new(30,03,2) { Species = 026, Ability = A4, Moves = new[]{ 009, 129, 280, 204 } }, // Raichu
            new(30,03,2) { Species = 026, Ability = A4, Moves = new[]{ 009, 129, 280, 204 }, Form = 1 }, // Raichu-1
            new(30,03,2) { Species = 172, Ability = A4, Moves = new[]{ 204, 609, 085, 186 } }, // Pichu
            new(30,03,2) { Species = 778, Ability = A4, Moves = new[]{ 086, 452, 425, 608 } }, // Mimikyu
            new(40,05,3) { Species = 025, Ability = A4, Moves = new[]{ 085, 231, 583, 086 } }, // Pikachu
            new(40,05,3) { Species = 026, Ability = A4, Moves = new[]{ 085, 034, 411, 583 } }, // Raichu
            new(40,05,3) { Species = 026, Ability = A4, Moves = new[]{ 085, 034, 057, 583 }, Form = 1 }, // Raichu-1
            new(40,05,3) { Species = 172, Ability = A4, Moves = new[]{ 204, 609, 085, 583 } }, // Pichu
            new(40,05,3) { Species = 778, Ability = A4, Moves = new[]{ 085, 452, 421, 608 } }, // Mimikyu
            new(50,08,4) { Species = 025, Ability = A4, Moves = new[]{ 087, 231, 583, 086 } }, // Pikachu
            new(50,08,4) { Species = 026, Ability = A4, Moves = new[]{ 087, 034, 411, 583 } }, // Raichu
            new(50,08,4) { Species = 026, Ability = A4, Moves = new[]{ 087, 034, 057, 583 }, Form = 1 }, // Raichu-1
            new(50,08,4) { Species = 172, Ability = A4, Moves = new[]{ 253, 609, 085, 583 } }, // Pichu
            new(50,08,4) { Species = 778, Ability = A4, Moves = new[]{ 085, 452, 261, 204 } }, // Mimikyu
            new(60,10,5) { Species = 025, Ability = A4, Moves = new[]{ 344, 231, 583, 086 } }, // Pikachu
            new(60,10,5) { Species = 025, Ability = A4, Moves = new[]{ 344, 231, 583, 086 }, Shiny = Shiny.Always }, // Pikachu
            new(60,10,5) { Species = 026, Ability = A4, Moves = new[]{ 087, 034, 411, 583 } }, // Raichu
            new(60,10,5) { Species = 026, Ability = A4, Moves = new[]{ 087, 034, 057, 583 }, Form = 1 }, // Raichu-1
            new(60,10,5) { Species = 172, Ability = A4, Moves = new[]{ 253, 609, 085, 583 } }, // Pichu
            new(60,10,5) { Species = 778, Ability = A4, Moves = new[]{ 087, 452, 261, 583 } }, // Mimikyu

            // 8/31 Electric Grass 
            new(17,01,1) { Species = 848, Ability = A4, Moves = new[]{ 609, 051, 496, 715 } }, // Toxel
            new(17,01,1) { Species = 835, Ability = A4, Moves = new[]{ 609, 033, 044, 046 } }, // Yamper
            new(17,01,1) { Species = 695, Ability = A4, Moves = new[]{ 085, 098, 001, 189 } }, // Heliolisk
            new(17,01,1) { Species = 840, Ability = A4, Moves = new[]{ 110, 310, 000, 000 } }, // Applin
            new(17,01,1) { Species = 597, Ability = A4, Moves = new[]{ 033, 042, 232, 106 } }, // Ferroseed
            new(17,01,1) { Species = 829, Ability = A4, Moves = new[]{ 075, 496, 229, 670 } }, // Gossifleur
            new(30,03,2) { Species = 836, Ability = A4, Moves = new[]{ 609, 209, 204, 706 } }, // Boltund
            new(30,03,2) { Species = 695, Ability = A4, Moves = new[]{ 085, 098, 001, 189 } }, // Heliolisk
            new(30,03,2) { Species = 597, Ability = A4, Moves = new[]{ 442, 042, 232, 106 } }, // Ferroseed
            new(30,03,2) { Species = 830, Ability = A4, Moves = new[]{ 536, 496, 229, 670 } }, // Eldegoss
            new(40,05,3) { Species = 836, Ability = A4, Moves = new[]{ 609, 209, 424, 706 } }, // Boltund
            new(40,05,3) { Species = 695, Ability = A4, Moves = new[]{ 085, 098, 447, 189 } }, // Heliolisk
            new(40,05,3) { Species = 598, Ability = A4, Moves = new[]{ 442, 438, 398, 106 } }, // Ferrothorn
            new(40,05,3) { Species = 830, Ability = A4, Moves = new[]{ 536, 304, 229, 670 } }, // Eldegoss
            new(50,08,4) { Species = 836, Ability = A4, Moves = new[]{ 609, 528, 424, 706 } }, // Boltund
            new(50,08,4) { Species = 695, Ability = A4, Moves = new[]{ 085, 304, 447, 523 } }, // Heliolisk
            new(50,08,4) { Species = 598, Ability = A4, Moves = new[]{ 360, 438, 398, 014 } }, // Ferrothorn
            new(50,08,4) { Species = 830, Ability = A4, Moves = new[]{ 536, 304, 229, 437 } }, // Eldegoss
            new(60,10,5) { Species = 836, Ability = A4, Moves = new[]{ 609, 528, 424, 706 } }, // Boltund
            new(60,10,5) { Species = 695, Ability = A4, Moves = new[]{ 085, 304, 447, 523 } }, // Heliolisk
            new(60,10,5) { Species = 598, Ability = A4, Moves = new[]{ 360, 438, 398, 014 } }, // Ferrothorn
            new(60,10,5) { Species = 830, Ability = A4, Moves = new[]{ 536, 304, 063, 437 } }, // Eldegoss

            // 9/18 Fairy
            new(17,01,1) { Species = 036, Ability = A4, Moves = new[]{ 574, 001, 204, 045 } }, // Clefable
            new(17,01,1) { Species = 040, Ability = A4, Moves = new[]{ 497, 574, 001, 111 } }, // Wigglytuff
            new(17,01,1) { Species = 044, Ability = A4, Moves = new[]{ 078, 079, 230, 051 } }, // Gloom
            new(17,01,1) { Species = 518, Ability = A4, Moves = new[]{ 060, 236, 111, 000 } }, // Musharna
            new(17,01,1) { Species = 547, Ability = A4, Moves = new[]{ 412, 585, 073, 178 } }, // Whimsicott
            new(17,01,1) { Species = 549, Ability = A4, Moves = new[]{ 412, 241, 437, 263 } }, // Lilligant
            new(30,03,2) { Species = 036, Ability = A4, Moves = new[]{ 574, 001, 236, 045 } }, // Clefable
            new(30,03,2) { Species = 040, Ability = A4, Moves = new[]{ 497, 574, 001, 034 } }, // Wigglytuff
            new(30,03,2) { Species = 182, Ability = A4, Moves = new[]{ 585, 572, 236, 051 } }, // Bellossom
            new(30,03,2) { Species = 518, Ability = A4, Moves = new[]{ 428, 060, 236, 111 } }, // Musharna
            new(30,03,2) { Species = 547, Ability = A4, Moves = new[]{ 412, 585, 073, 178 } }, // Whimsicott
            new(30,03,2) { Species = 549, Ability = A4, Moves = new[]{ 412, 241, 437, 263 } }, // Lilligant
            new(40,05,3) { Species = 036, Ability = A4, Moves = new[]{ 585, 309, 236, 345 } }, // Clefable
            new(40,05,3) { Species = 040, Ability = A4, Moves = new[]{ 304, 583, 360, 034 } }, // Wigglytuff
            new(40,05,3) { Species = 182, Ability = A4, Moves = new[]{ 585, 572, 236, 051 } }, // Bellossom
            new(40,05,3) { Species = 518, Ability = A4, Moves = new[]{ 585, 094, 236, 360 } }, // Musharna
            new(40,05,3) { Species = 547, Ability = A4, Moves = new[]{ 412, 585, 073, 366 } }, // Whimsicott
            new(40,05,3) { Species = 549, Ability = A4, Moves = new[]{ 412, 241, 437, 263 } }, // Lilligant
            new(50,08,4) { Species = 036, Ability = A4, Moves = new[]{ 585, 309, 236, 345 } }, // Clefable
            new(50,08,4) { Species = 040, Ability = A4, Moves = new[]{ 304, 583, 360, 034 } }, // Wigglytuff
            new(50,08,4) { Species = 182, Ability = A4, Moves = new[]{ 585, 572, 236, 051 } }, // Bellossom
            new(50,08,4) { Species = 518, Ability = A4, Moves = new[]{ 585, 094, 236, 360 } }, // Musharna
            new(50,08,4) { Species = 547, Ability = A4, Moves = new[]{ 538, 585, 073, 366 } }, // Whimsicott
            new(50,08,4) { Species = 549, Ability = A4, Moves = new[]{ 412, 241, 437, 263 } }, // Lilligant
            new(60,10,5) { Species = 036, Ability = A4, Moves = new[]{ 585, 309, 236, 345 } }, // Clefable
            new(60,10,5) { Species = 040, Ability = A4, Moves = new[]{ 304, 583, 360, 034 } }, // Wigglytuff
            new(60,10,5) { Species = 182, Ability = A4, Moves = new[]{ 585, 572, 236, 051 } }, // Bellossom
            new(60,10,5) { Species = 518, Ability = A4, Moves = new[]{ 585, 094, 236, 360 } }, // Musharna
            new(60,10,5) { Species = 036, Ability = A4, Moves = new[]{ 585, 309, 236, 345 }, Shiny = Shiny.Always }, // Clefable
            new(60,10,5) { Species = 549, Ability = A4, Moves = new[]{ 412, 241, 437, 263 } }, // Lilligant

            // 10/1 - Spooky
            new(17,01,1) { Species = 093, Ability = A4, Moves = new[]{ 371, 122, 095, 325 } }, // Haunter
            new(17,01,1) { Species = 425, Ability = A4, Moves = new[]{ 016, 506, 310, 371 } }, // Drifloon
            new(17,01,1) { Species = 355, Ability = A4, Moves = new[]{ 310, 425, 043, 506 } }, // Duskull
            new(17,01,1) { Species = 859, Ability = A4, Moves = new[]{ 372, 313, 260, 044 } }, // Impidimp
            new(17,01,1) { Species = 633, Ability = A4, Moves = new[]{ 225, 033, 399, 044 } }, // Deino
            new(17,01,1) { Species = 877, Ability = A4, Moves = new[]{ 084, 098, 681, 043 } }, // Morpeko
            new(30,03,2) { Species = 094, Ability = A4, Moves = new[]{ 371, 389, 095, 325 }, CanGigantamax = true }, // Gengar
            new(30,03,2) { Species = 426, Ability = A4, Moves = new[]{ 016, 247, 310, 371 } }, // Drifblim
            new(30,03,2) { Species = 355, Ability = A4, Moves = new[]{ 310, 425, 371, 506 } }, // Duskull
            new(30,03,2) { Species = 859, Ability = A4, Moves = new[]{ 259, 389, 207, 044 } }, // Impidimp
            new(30,03,2) { Species = 633, Ability = A4, Moves = new[]{ 225, 021, 399, 029 } }, // Deino
            new(30,03,2) { Species = 877, Ability = A4, Moves = new[]{ 209, 098, 044, 043 } }, // Morpeko
            new(40,05,3) { Species = 094, Ability = A4, Moves = new[]{ 506, 389, 095, 325 }, CanGigantamax = true }, // Gengar
            new(40,05,3) { Species = 426, Ability = A4, Moves = new[]{ 016, 247, 360, 371 } }, // Drifblim
            new(40,05,3) { Species = 477, Ability = A4, Moves = new[]{ 247, 009, 371, 157 } }, // Dusknoir
            new(40,05,3) { Species = 860, Ability = A4, Moves = new[]{ 417, 793, 421, 399 } }, // Morgrem
            new(40,05,3) { Species = 633, Ability = A4, Moves = new[]{ 406, 021, 399, 423 } }, // Deino
            new(40,05,3) { Species = 877, Ability = A4, Moves = new[]{ 209, 098, 044, 402 } }, // Morpeko
            new(50,08,4) { Species = 094, Ability = A4, Moves = new[]{ 247, 399, 094, 085 }, CanGigantamax = true }, // Gengar
            new(50,08,4) { Species = 426, Ability = A4, Moves = new[]{ 366, 247, 360, 371 } }, // Drifblim
            new(50,08,4) { Species = 477, Ability = A4, Moves = new[]{ 247, 009, 280, 157 } }, // Dusknoir
            new(50,08,4) { Species = 861, Ability = A4, Moves = new[]{ 789, 492, 421, 399 }, CanGigantamax = true }, // Grimmsnarl
            new(50,08,4) { Species = 634, Ability = A4, Moves = new[]{ 406, 304, 399, 423 } }, // Zweilous
            new(50,08,4) { Species = 877, Ability = A4, Moves = new[]{ 783, 098, 242, 402 } }, // Morpeko
            new(60,10,5) { Species = 094, Ability = A4, Moves = new[]{ 247, 399, 605, 085 }, CanGigantamax = true }, // Gengar
            new(60,10,5) { Species = 426, Ability = A4, Moves = new[]{ 366, 247, 360, 693 } }, // Drifblim
            new(60,10,5) { Species = 477, Ability = A4, Moves = new[]{ 247, 009, 280, 089 } }, // Dusknoir
            new(60,10,5) { Species = 861, Ability = A4, Moves = new[]{ 789, 492, 421, 417 }, CanGigantamax = true }, // Grimmsnarl
            new(60,10,5) { Species = 635, Ability = A4, Moves = new[]{ 406, 304, 399, 056 } }, // Hydreigon
            new(60,10,5) { Species = 877, Ability = A4, Moves = new[]{ 783, 037, 242, 402 } }, // Morpeko

            // 10/30 - Halloween
            new(17,01,1) { Species = 562, Ability = A4, Moves = new[]{ 261, 114, 310, 101 } }, // Yamask
          //new(17,01,1) { Species = 778, Ability = A4, Moves = new[]{ 086, 452, 425, 010 } }, // Mimikyu
            new(17,01,1) { Species = 709, Ability = A4, Moves = new[]{ 785, 421, 261, 310 } }, // Trevenant
            new(17,01,1) { Species = 855, Ability = A4, Moves = new[]{ 597, 110, 668, 310 } }, // Polteageist
            new(30,03,2) { Species = 710, Ability = A4, Moves = new[]{ 567, 425, 310, 331 } }, // Pumpkaboo
            new(30,03,2) { Species = 563, Ability = A4, Moves = new[]{ 578, 421, 310, 261 } }, // Cofagrigus
          //new(30,03,2) { Species = 778, Ability = A4, Moves = new[]{ 086, 452, 425, 608 } }, // Mimikyu
            new(30,03,2) { Species = 709, Ability = A4, Moves = new[]{ 785, 506, 261, 310 } }, // Trevenant
            new(30,03,2) { Species = 855, Ability = A4, Moves = new[]{ 597, 110, 389, 310 } }, // Polteageist
            new(40,05,3) { Species = 710, Ability = A4, Moves = new[]{ 567, 247, 310, 402 } }, // Pumpkaboo
            new(40,05,3) { Species = 563, Ability = A4, Moves = new[]{ 578, 421, 310, 261 } }, // Cofagrigus
          //new(40,05,3) { Species = 778, Ability = A4, Moves = new[]{ 085, 452, 421, 608 } }, // Mimikyu
            new(40,05,3) { Species = 709, Ability = A4, Moves = new[]{ 785, 506, 261, 310 } }, // Trevenant
            new(40,05,3) { Species = 855, Ability = A4, Moves = new[]{ 597, 110, 389, 310 } }, // Polteageist
            new(50,08,4) { Species = 711, Ability = A4, Moves = new[]{ 567, 247, 585, 402 } }, // Gourgeist
            new(50,08,4) { Species = 711, Ability = A4, Moves = new[]{ 567, 247, 585, 402 }, Form = 1 }, // Gourgeist-1
            new(50,08,4) { Species = 711, Ability = A4, Moves = new[]{ 567, 247, 585, 402 }, Form = 2 }, // Gourgeist-2
            new(50,08,4) { Species = 711, Ability = A4, Moves = new[]{ 567, 247, 585, 402 }, Form = 3 }, // Gourgeist-3
            new(50,08,4) { Species = 563, Ability = A4, Moves = new[]{ 578, 247, 399, 261 } }, // Cofagrigus
          //new(50,08,4) { Species = 778, Ability = A4, Moves = new[]{ 085, 452, 261, 204 } }, // Mimikyu
            new(50,08,4) { Species = 709, Ability = A4, Moves = new[]{ 452, 506, 261, 310 } }, // Trevenant
            new(50,08,4) { Species = 855, Ability = A4, Moves = new[]{ 247, 417, 389, 310 } }, // Polteageist
            new(60,10,5) { Species = 711, Ability = A4, Moves = new[]{ 567, 247, 433, 402 } }, // Gourgeist
            new(60,10,5) { Species = 711, Ability = A4, Moves = new[]{ 567, 247, 433, 402 }, Form = 1, Shiny = Shiny.Always }, // Gourgeist-1
            new(60,10,5) { Species = 711, Ability = A4, Moves = new[]{ 567, 247, 433, 402 }, Form = 2 }, // Gourgeist-2
            new(60,10,5) { Species = 711, Ability = A4, Moves = new[]{ 567, 247, 433, 402 }, Form = 3, Shiny = Shiny.Always }, // Gourgeist-3
            new(60,10,5) { Species = 563, Ability = A4, Moves = new[]{ 578, 247, 399, 261 } }, // Cofagrigus
          //new(60,10,5) { Species = 778, Ability = A4, Moves = new[]{ 087, 452, 261, 583 } }, // Mimikyu
            new(60,10,5) { Species = 709, Ability = A4, Moves = new[]{ 452, 506, 261, 310 } }, // Trevenant
            new(60,10,5) { Species = 855, Ability = A4, Moves = new[]{ 247, 417, 389, 310 } }, // Polteageist

            // 11/2 - November
            new(17,01,1) { Species = 012, Ability = A4, Moves = new[]{ 081, 060, 016, 079 }, CanGigantamax = true }, // Butterfree
            new(17,01,1) { Species = 213, Ability = A4, Moves = new[]{ 088, 474, 414, 522 } }, // Shuckle
            new(17,01,1) { Species = 290, Ability = A4, Moves = new[]{ 189, 206, 010, 106 } }, // Nincada
            new(17,01,1) { Species = 568, Ability = A4, Moves = new[]{ 390, 133, 491, 001 } }, // Trubbish
            new(17,01,1) { Species = 043, Ability = A4, Moves = new[]{ 078, 077, 051, 230 } }, // Oddish
            new(17,01,1) { Species = 453, Ability = A4, Moves = new[]{ 040, 269, 279, 189 } }, // Croagunk
            new(30,03,2) { Species = 012, Ability = A4, Moves = new[]{ 405, 060, 016, 079 }, CanGigantamax = true }, // Butterfree
            new(30,03,2) { Species = 213, Ability = A4, Moves = new[]{ 088, 474, 414, 522 } }, // Shuckle
            new(30,03,2) { Species = 291, Ability = A4, Moves = new[]{ 210, 206, 332, 232 } }, // Ninjask
            new(30,03,2) { Species = 568, Ability = A4, Moves = new[]{ 092, 133, 491, 036 } }, // Trubbish
            new(30,03,2) { Species = 045, Ability = A4, Moves = new[]{ 080, 585, 051, 230 } }, // Vileplume
            new(30,03,2) { Species = 453, Ability = A4, Moves = new[]{ 474, 389, 279, 189 } }, // Croagunk
            new(40,05,3) { Species = 012, Ability = A4, Moves = new[]{ 405, 060, 016, 078 }, CanGigantamax = true }, // Butterfree
            new(40,05,3) { Species = 213, Ability = A4, Moves = new[]{ 157, 188, 089, 522 } }, // Shuckle
            new(40,05,3) { Species = 291, Ability = A4, Moves = new[]{ 210, 206, 332, 232 } }, // Ninjask
            new(40,05,3) { Species = 569, Ability = A4, Moves = new[]{ 188, 133, 034, 707 }, CanGigantamax = true }, // Garbodor
            new(40,05,3) { Species = 045, Ability = A4, Moves = new[]{ 080, 585, 051, 230 } }, // Vileplume
            new(40,05,3) { Species = 454, Ability = A4, Moves = new[]{ 188, 389, 279, 189 } }, // Toxicroak
            new(50,08,4) { Species = 012, Ability = A4, Moves = new[]{ 405, 403, 093, 078 }, CanGigantamax = true }, // Butterfree
            new(50,08,4) { Species = 213, Ability = A4, Moves = new[]{ 157, 188, 089, 564 } }, // Shuckle
            new(50,08,4) { Species = 291, Ability = A4, Moves = new[]{ 210, 163, 332, 232 } }, // Ninjask
            new(50,08,4) { Species = 569, Ability = A4, Moves = new[]{ 188, 499, 034, 707 }, CanGigantamax = true }, // Garbodor
            new(50,08,4) { Species = 045, Ability = A4, Moves = new[]{ 080, 585, 051, 034 } }, // Vileplume
            new(50,08,4) { Species = 454, Ability = A4, Moves = new[]{ 188, 389, 280, 189 } }, // Toxicroak
            new(60,10,5) { Species = 012, Ability = A4, Moves = new[]{ 405, 403, 527, 078 }, CanGigantamax = true }, // Butterfree
            new(60,10,5) { Species = 213, Ability = A4, Moves = new[]{ 444, 188, 089, 564 } }, // Shuckle
            new(60,10,5) { Species = 291, Ability = A4, Moves = new[]{ 404, 163, 332, 232 } }, // Ninjask
            new(60,10,5) { Species = 569, Ability = A4, Moves = new[]{ 441, 499, 034, 707 }, CanGigantamax = true }, // Garbodor
            new(60,10,5) { Species = 045, Ability = A4, Moves = new[]{ 080, 585, 051, 034 } }, // Vileplume
            new(60,10,5) { Species = 454, Ability = A4, Moves = new[]{ 188, 389, 280, 523 } }, // Toxicroak

            // 11/20 - Forest
            new(17,01,1) { Species = 420, Ability = A4, Moves = new[]{ 033, 572, 074, 670 } }, // Cherubi
            new(17,01,1) { Species = 590, Ability = A4, Moves = new[]{ 078, 492, 310, 412 } }, // Foongus
            new(17,01,1) { Species = 755, Ability = A4, Moves = new[]{ 402, 109, 605, 310 } }, // Morelull
            new(17,01,1) { Species = 819, Ability = A4, Moves = new[]{ 747, 231, 371, 033 } }, // Skwovet
            new(30,03,2) { Species = 420, Ability = A4, Moves = new[]{ 033, 572, 074, 402 } }, // Cherubi
            new(30,03,2) { Species = 590, Ability = A4, Moves = new[]{ 078, 492, 310, 412 } }, // Foongus
            new(30,03,2) { Species = 756, Ability = A4, Moves = new[]{ 402, 109, 605, 310 } }, // Shiinotic
            new(30,03,2) { Species = 819, Ability = A4, Moves = new[]{ 747, 231, 371, 033 } }, // Skwovet
            new(30,03,2) { Species = 820, Ability = A4, Moves = new[]{ 747, 231, 371, 034 } }, // Greedent
            new(40,05,3) { Species = 420, Ability = A4, Moves = new[]{ 311, 572, 074, 402 } }, // Cherubi
            new(40,05,3) { Species = 591, Ability = A4, Moves = new[]{ 078, 492, 092, 412 } }, // Amoonguss
            new(40,05,3) { Species = 756, Ability = A4, Moves = new[]{ 402, 585, 605, 310 } }, // Shiinotic
            new(40,05,3) { Species = 819, Ability = A4, Moves = new[]{ 747, 360, 371, 044 } }, // Skwovet
            new(40,05,3) { Species = 820, Ability = A4, Moves = new[]{ 747, 360, 371, 424 } }, // Greedent
            new(50,08,4) { Species = 420, Ability = A4, Moves = new[]{ 311, 572, 605, 402 } }, // Cherubi
            new(50,08,4) { Species = 591, Ability = A4, Moves = new[]{ 188, 492, 092, 412 } }, // Amoonguss
            new(50,08,4) { Species = 756, Ability = A4, Moves = new[]{ 412, 585, 605, 188 } }, // Shiinotic
            new(50,08,4) { Species = 819, Ability = A4, Moves = new[]{ 747, 360, 371, 331 } }, // Skwovet
            new(50,08,4) { Species = 820, Ability = A4, Moves = new[]{ 747, 360, 371, 424 } }, // Greedent
            new(60,10,5) { Species = 420, Ability = A4, Moves = new[]{ 311, 572, 605, 412 } }, // Cherubi
            new(60,10,5) { Species = 591, Ability = A4, Moves = new[]{ 147, 492, 092, 412 } }, // Amoonguss
            new(60,10,5) { Species = 756, Ability = A4, Moves = new[]{ 147, 585, 605, 188 } }, // Shiinotic
            new(60,10,5) { Species = 819, Ability = A4, Moves = new[]{ 747, 360, 371, 034 } }, // Skwovet
            new(60,10,5) { Species = 819, Ability = A4, Moves = new[]{ 747, 360, 371, 034 }, Shiny = Shiny.Always }, // Skwovet
            new(60,10,5) { Species = 820, Ability = A4, Moves = new[]{ 747, 360, 371, 089 } }, // Greedent

            // 11/30 - Ice and Fire
            new(17,01,1) { Species = 131, Ability = A4, Moves = new[]{ 055, 420, 045, 047 }, CanGigantamax = true }, // Lapras
            new(17,01,1) { Species = 712, Ability = A4, Moves = new[]{ 181, 196, 033, 106 } }, // Bergmite
            new(17,01,1) { Species = 461, Ability = A4, Moves = new[]{ 420, 372, 232, 279 } }, // Weavile
            new(17,01,1) { Species = 850, Ability = A4, Moves = new[]{ 172, 044, 035, 052 } }, // Sizzlipede
            new(17,01,1) { Species = 776, Ability = A4, Moves = new[]{ 175, 123, 033, 052 } }, // Turtonator
            new(17,01,1) { Species = 077, Ability = A4, Moves = new[]{ 488, 045, 039, 052 } }, // Ponyta
            new(30,03,2) { Species = 131, Ability = A4, Moves = new[]{ 352, 420, 109, 047 }, CanGigantamax = true }, // Lapras
            new(30,03,2) { Species = 712, Ability = A4, Moves = new[]{ 157, 423, 033, 044 } }, // Bergmite
            new(30,03,2) { Species = 461, Ability = A4, Moves = new[]{ 420, 372, 232, 279 } }, // Weavile
            new(30,03,2) { Species = 851, Ability = A4, Moves = new[]{ 172, 404, 422, 044 }, CanGigantamax = true }, // Centiskorch
            new(30,03,2) { Species = 776, Ability = A4, Moves = new[]{ 406, 123, 033, 052 } }, // Turtonator
            new(30,03,2) { Species = 077, Ability = A4, Moves = new[]{ 488, 023, 583, 097 } }, // Ponyta
            new(40,05,3) { Species = 131, Ability = A4, Moves = new[]{ 352, 196, 109, 047 }, CanGigantamax = true }, // Lapras
            new(40,05,3) { Species = 713, Ability = A4, Moves = new[]{ 157, 423, 036, 044 } }, // Avalugg
            new(40,05,3) { Species = 461, Ability = A4, Moves = new[]{ 420, 468, 232, 279 } }, // Weavile
            new(40,05,3) { Species = 851, Ability = A4, Moves = new[]{ 424, 404, 422, 044 }, CanGigantamax = true }, // Centiskorch
            new(40,05,3) { Species = 776, Ability = A4, Moves = new[]{ 406, 776, 034, 053 } }, // Turtonator
            new(40,05,3) { Species = 078, Ability = A4, Moves = new[]{ 172, 023, 583, 224 } }, // Rapidash
            new(50,08,4) { Species = 131, Ability = A4, Moves = new[]{ 057, 196, 058, 047 }, CanGigantamax = true }, // Lapras
            new(50,08,4) { Species = 713, Ability = A4, Moves = new[]{ 776, 059, 036, 044 } }, // Avalugg
            new(50,08,4) { Species = 461, Ability = A4, Moves = new[]{ 420, 468, 232, 279 } }, // Weavile
            new(50,08,4) { Species = 851, Ability = A4, Moves = new[]{ 680, 404, 422, 044 }, CanGigantamax = true }, // Centiskorch
            new(50,08,4) { Species = 776, Ability = A4, Moves = new[]{ 406, 776, 504, 053 } }, // Turtonator
            new(50,08,4) { Species = 078, Ability = A4, Moves = new[]{ 517, 528, 583, 224 } }, // Rapidash
            new(60,10,5) { Species = 131, Ability = A4, Moves = new[]{ 057, 196, 058, 329 }, CanGigantamax = true }, // Lapras
            new(60,10,5) { Species = 713, Ability = A4, Moves = new[]{ 776, 059, 038, 044 } }, // Avalugg
            new(60,10,5) { Species = 461, Ability = A4, Moves = new[]{ 420, 400, 232, 279 } }, // Weavile
            new(60,10,5) { Species = 851, Ability = A4, Moves = new[]{ 680, 679, 422, 044 }, CanGigantamax = true }, // Centiskorch
            new(60,10,5) { Species = 776, Ability = A4, Moves = new[]{ 434, 776, 504, 053 } }, // Turtonator
            new(60,10,5) { Species = 078, Ability = A4, Moves = new[]{ 394, 528, 583, 224 } }, // Rapidash

            // 12/25 - Winter Holiday
            new(17,01,1) { Species = 037, Ability = A4, Moves = new[]{ 420, 196, 039, 577 }, Form = 1 }, // Vulpix-1
            new(17,01,1) { Species = 124, Ability = A4, Moves = new[]{ 181, 001, 093, 122 } }, // Jynx
            new(17,01,1) { Species = 225, Ability = A4, Moves = new[]{ 217, 229, 098, 420 } }, // Delibird
            new(17,01,1) { Species = 607, Ability = A4, Moves = new[]{ 310, 052, 506, 123 } }, // Litwick
            new(17,01,1) { Species = 873, Ability = A4, Moves = new[]{ 522, 078, 181, 432 } }, // Frosmoth
            new(30,03,2) { Species = 037, Ability = A4, Moves = new[]{ 420, 058, 326, 577 }, Form = 1 }, // Vulpix-1
            new(30,03,2) { Species = 124, Ability = A4, Moves = new[]{ 181, 001, 093, 313 } }, // Jynx
            new(30,03,2) { Species = 225, Ability = A4, Moves = new[]{ 217, 065, 034, 693 } }, // Delibird
            new(30,03,2) { Species = 608, Ability = A4, Moves = new[]{ 310, 261, 083, 123 } }, // Lampent
            new(30,03,2) { Species = 873, Ability = A4, Moves = new[]{ 522, 078, 062, 432 } }, // Frosmoth
            new(40,05,3) { Species = 037, Ability = A4, Moves = new[]{ 062, 058, 326, 577 }, Form = 1 }, // Vulpix-1
            new(40,05,3) { Species = 124, Ability = A4, Moves = new[]{ 058, 142, 094, 247 } }, // Jynx
            new(40,05,3) { Species = 225, Ability = A4, Moves = new[]{ 217, 065, 280, 196 } }, // Delibird
            new(40,05,3) { Species = 609, Ability = A4, Moves = new[]{ 247, 261, 257, 094 } }, // Chandelure
            new(40,05,3) { Species = 873, Ability = A4, Moves = new[]{ 405, 403, 062, 432 } }, // Frosmoth
            new(50,08,4) { Species = 037, Ability = A4, Moves = new[]{ 694, 058, 326, 577 }, Form = 1 }, // Vulpix-1
            new(50,08,4) { Species = 124, Ability = A4, Moves = new[]{ 058, 142, 094, 247 } }, // Jynx
            new(50,08,4) { Species = 225, Ability = A4, Moves = new[]{ 217, 059, 034, 280 } }, // Delibird
            new(50,08,4) { Species = 609, Ability = A4, Moves = new[]{ 247, 261, 315, 094 } }, // Chandelure
            new(50,08,4) { Species = 873, Ability = A4, Moves = new[]{ 405, 403, 058, 297 } }, // Frosmoth
            new(60,10,5) { Species = 037, Ability = A4, Moves = new[]{ 694, 059, 326, 577 }, Form = 1 }, // Vulpix-1
            new(60,10,5) { Species = 037, Ability = A4, Moves = new[]{ 694, 059, 326, 577 }, Form = 1, Shiny = Shiny.Always }, // Vulpix-1
            new(60,10,5) { Species = 124, Ability = A4, Moves = new[]{ 058, 142, 094, 247 } }, // Jynx
            new(60,10,5) { Species = 225, Ability = A4, Moves = new[]{ 217, 059, 065, 280 } }, // Delibird
            new(60,10,5) { Species = 609, Ability = A4, Moves = new[]{ 247, 412, 315, 094 } }, // Chandelure
            new(60,10,5) { Species = 873, Ability = A4, Moves = new[]{ 405, 403, 058, 542 } }, // Frosmoth

            // 1/1 - Year of the Dragon
            new(17,01,1) { Species = 884, Ability = A4, Moves = new[]{ 232, 043, 468, 249 }, CanGigantamax = true }, // Duraludon
            new(17,01,1) { Species = 610, Ability = A4, Moves = new[]{ 044, 163, 372, 010 } }, // Axew
            new(17,01,1) { Species = 704, Ability = A4, Moves = new[]{ 225, 352, 033, 175 } }, // Goomy
          //new(17,01,1) { Species = 446, Ability = A4, Moves = new[]{ 033, 044, 122, 111 } }, // Munchlax
            new(17,01,1) { Species = 759, Ability = A4, Moves = new[]{ 693, 371, 608, 033 } }, // Stufful
            new(17,01,1) { Species = 572, Ability = A4, Moves = new[]{ 497, 204, 402, 001 } }, // Minccino
            new(30,03,2) { Species = 884, Ability = A4, Moves = new[]{ 232, 784, 468, 249 }, CanGigantamax = true }, // Duraludon
            new(30,03,2) { Species = 610, Ability = A4, Moves = new[]{ 337, 163, 242, 530 } }, // Axew
            new(30,03,2) { Species = 704, Ability = A4, Moves = new[]{ 225, 352, 033, 341 } }, // Goomy
            new(30,03,2) { Species = 143, Ability = A4, Moves = new[]{ 034, 242, 118, 111 }, CanGigantamax = true }, // Snorlax
            new(30,03,2) { Species = 759, Ability = A4, Moves = new[]{ 693, 371, 359, 036 } }, // Stufful
            new(30,03,2) { Species = 572, Ability = A4, Moves = new[]{ 497, 231, 402, 129 } }, // Minccino
            new(40,05,3) { Species = 884, Ability = A4, Moves = new[]{ 232, 525, 085, 249 }, CanGigantamax = true }, // Duraludon
            new(40,05,3) { Species = 611, Ability = A4, Moves = new[]{ 406, 231, 242, 530 } }, // Fraxure
            new(40,05,3) { Species = 705, Ability = A4, Moves = new[]{ 406, 352, 491, 341 } }, // Sliggoo
          //new(40,05,3) { Species = 143, Ability = A4, Moves = new[]{ 034, 667, 242, 281 }, CanGigantamax = true }, // Snorlax
            new(40,05,3) { Species = 760, Ability = A4, Moves = new[]{ 693, 034, 359, 036 } }, // Bewear
            new(40,05,3) { Species = 573, Ability = A4, Moves = new[]{ 331, 231, 350, 129 } }, // Cinccino
            new(50,08,4) { Species = 884, Ability = A4, Moves = new[]{ 232, 406, 085, 776 }, CanGigantamax = true }, // Duraludon
            new(50,08,4) { Species = 612, Ability = A4, Moves = new[]{ 406, 231, 370, 530 } }, // Haxorus
            new(50,08,4) { Species = 706, Ability = A4, Moves = new[]{ 406, 034, 491, 126 } }, // Goodra
          //new(50,08,4) { Species = 143, Ability = A4, Moves = new[]{ 034, 667, 280, 523 }, CanGigantamax = true }, // Snorlax
            new(50,08,4) { Species = 760, Ability = A4, Moves = new[]{ 663, 034, 359, 009 } }, // Bewear
            new(50,08,4) { Species = 573, Ability = A4, Moves = new[]{ 331, 231, 350, 304 } }, // Cinccino
            new(60,10,5) { Species = 884, Ability = A4, Moves = new[]{ 430, 406, 085, 776 }, CanGigantamax = true }, // Duraludon
            new(60,10,5) { Species = 612, Ability = A4, Moves = new[]{ 200, 231, 370, 089 } }, // Haxorus
            new(60,10,5) { Species = 706, Ability = A4, Moves = new[]{ 406, 438, 482, 126 } }, // Goodra
          //new(60,10,5) { Species = 143, Ability = A4, Moves = new[]{ 034, 442, 242, 428 }, CanGigantamax = true }, // Snorlax
            new(60,10,5) { Species = 760, Ability = A4, Moves = new[]{ 663, 038, 276, 009 } }, // Bewear
            new(60,10,5) { Species = 573, Ability = A4, Moves = new[]{ 402, 231, 350, 304 } }, // Cinccino

            // 1/8 - Forest Returns (no changes besides shiny Greedent replacing regular R5 Amoonguss)
            new(60,10,5) { Species = 820, Ability = A4, Moves = new[]{ 747, 360, 371, 089 }, Shiny = Shiny.Always }, // Greedent

            // 1/21 - Normal Bovine
            new(17,01,1) { Species = 128, Ability = A4, Moves = new[]{ 033, 157, 030, 371 } }, // Tauros
            new(17,01,1) { Species = 626, Ability = A4, Moves = new[]{ 033, 030, 031, 523 } }, // Bouffalant
            new(17,01,1) { Species = 241, Ability = A4, Moves = new[]{ 707, 033, 023, 205 } }, // Miltank
            new(30,03,2) { Species = 128, Ability = A4, Moves = new[]{ 033, 157, 030, 370 } }, // Tauros
            new(30,03,2) { Species = 626, Ability = A4, Moves = new[]{ 279, 030, 675, 523 } }, // Bouffalant
            new(30,03,2) { Species = 241, Ability = A4, Moves = new[]{ 707, 428, 023, 205 } }, // Miltank
            new(40,05,3) { Species = 128, Ability = A4, Moves = new[]{ 036, 157, 030, 370 } }, // Tauros
            new(40,05,3) { Species = 626, Ability = A4, Moves = new[]{ 279, 543, 675, 523 } }, // Bouffalant
            new(40,05,3) { Species = 241, Ability = A4, Moves = new[]{ 707, 428, 034, 205 } }, // Miltank
            new(50,08,4) { Species = 128, Ability = A4, Moves = new[]{ 034, 157, 030, 370 } }, // Tauros
            new(50,08,4) { Species = 626, Ability = A4, Moves = new[]{ 224, 543, 675, 523 } }, // Bouffalant
            new(50,08,4) { Species = 241, Ability = A4, Moves = new[]{ 707, 428, 034, 583 } }, // Miltank
            new(60,10,5) { Species = 128, Ability = A4, Moves = new[]{ 034, 157, 372, 370 } }, // Tauros
            new(60,10,5) { Species = 128, Ability = A4, Moves = new[]{ 034, 157, 372, 370 }, Shiny = Shiny.Always }, // Tauros
            new(60,10,5) { Species = 626, Ability = A4, Moves = new[]{ 224, 543, 675, 089 } }, // Bouffalant
            new(60,10,5) { Species = 241, Ability = A4, Moves = new[]{ 667, 428, 034, 583 } }, // Miltank
        };

        internal static readonly EncounterStatic8ND[] Dist_SW =
        {
            // 11/15 - Butterfree
            new(17,01,1,SW) { Species = 012, Ability = A4, Moves = new[]{ 405, 060, 016, 078 } }, // Butterfree
            new(17,01,1,SW) { Species = 012, Ability = A4, Moves = new[]{ 405, 060, 016, 078 }, CanGigantamax = true }, // Butterfree
            new(17,01,1,SW) { Species = 843, Ability = A2, Moves = new[]{ 693, 523, 189, 103 } }, // Silicobra
            new(17,01,1,SW) { Species = 843, Ability = A4, Moves = new[]{ 693, 523, 189, 103 } }, // Silicobra
            new(17,01,1,SW) { Species = 833, Ability = A2, Moves = new[]{ 055, 044, 033, 213 } }, // Chewtle
            new(17,01,1,SW) { Species = 833, Ability = A4, Moves = new[]{ 055, 044, 033, 213 } }, // Chewtle
            new(30,03,2,SW) { Species = 012, Ability = A4, Moves = new[]{ 405, 403, 081, 079 } }, // Butterfree
            new(30,03,2,SW) { Species = 012, Ability = A4, Moves = new[]{ 405, 403, 081, 079 }, CanGigantamax = true }, // Butterfree
            new(30,03,2,SW) { Species = 843, Ability = A2, Moves = new[]{ 693, 523, 029, 137 } }, // Silicobra
            new(30,03,2,SW) { Species = 843, Ability = A4, Moves = new[]{ 693, 523, 029, 137 } }, // Silicobra
            new(30,03,2,SW) { Species = 834, Ability = A2, Moves = new[]{ 317, 242, 055, 334 } }, // Drednaw
            new(30,03,2,SW) { Species = 834, Ability = A4, Moves = new[]{ 317, 242, 055, 334 } }, // Drednaw
            new(40,05,3,SW) { Species = 844, Ability = A4, Moves = new[]{ 693, 529, 407, 424 } }, // Sandaconda
            new(40,05,3,SW) { Species = 844, Ability = A4, Moves = new[]{ 693, 529, 407, 424 }, CanGigantamax = true }, // Sandaconda
            new(40,05,3,SW) { Species = 834, Ability = A4, Moves = new[]{ 157, 534, 034, 317 } }, // Drednaw
            new(40,05,3,SW) { Species = 834, Ability = A4, Moves = new[]{ 157, 534, 034, 317 }, CanGigantamax = true }, // Drednaw
            new(50,08,4,SW) { Species = 844, Ability = A4, Moves = new[]{ 693, 529, 342, 328 } }, // Sandaconda
            new(50,08,4,SW) { Species = 844, Ability = A4, Moves = new[]{ 693, 529, 342, 328 }, CanGigantamax = true }, // Sandaconda
            new(50,08,4,SW) { Species = 834, Ability = A4, Moves = new[]{ 157, 710, 350, 523 } }, // Drednaw
            new(50,08,4,SW) { Species = 834, Ability = A4, Moves = new[]{ 157, 710, 350, 523 }, CanGigantamax = true }, // Drednaw
            new(60,10,5,SW) { Species = 012, Ability = A4, Moves = new[]{ 405, 094, 542, 202 } }, // Butterfree
            new(70,10,5,SW) { Species = 012, Ability = A4, Moves = new[]{ 405, 094, 542, 202 }, CanGigantamax = true }, // Butterfree
            new(60,10,5,SW) { Species = 844, Ability = A4, Moves = new[]{ 693, 529, 342, 328 } }, // Sandaconda
            new(70,10,5,SW) { Species = 844, Ability = A4, Moves = new[]{ 693, 529, 342, 328 }, CanGigantamax = true }, // Sandaconda
            new(60,10,5,SW) { Species = 834, Ability = A4, Moves = new[]{ 157, 710, 317, 334 } }, // Drednaw
            new(70,10,5,SW) { Species = 834, Ability = A4, Moves = new[]{ 157, 710, 317, 334 }, CanGigantamax = true }, // Drednaw

            // 12/03 - Snorlax
          //new(17,01,1,SW) { Species = 012, Ability = A4, Moves = new[]{ 405, 060, 016, 078 } }, // Butterfree
          //new(17,01,1,SW) { Species = 012, Ability = A4, Moves = new[]{ 405, 060, 016, 078 }, CanGigantamax = true }, // Butterfree
          //new(17,01,1,SW) { Species = 843, Ability = A4, Moves = new[]{ 693, 523, 189, 103 } }, // Silicobra
          //new(17,01,1,SW) { Species = 833, Ability = A4, Moves = new[]{ 055, 044, 033, 213 } }, // Chewtle
          //new(30,03,2,SW) { Species = 012, Ability = A4, Moves = new[]{ 405, 403, 081, 079 }, CanGigantamax = true }, // Butterfree
          //new(30,03,2,SW) { Species = 843, Ability = A4, Moves = new[]{ 693, 523, 029, 137 } }, // Silicobra
          //new(30,03,2,SW) { Species = 834, Ability = A4, Moves = new[]{ 317, 242, 055, 334 } }, // Drednaw
          //new(30,03,2,SW) { Species = 834, Ability = A2, Moves = new[]{ 317, 242, 055, 334 } }, // Drednaw
          //new(40,05,3,SW) { Species = 844, Ability = A4, Moves = new[]{ 693, 529, 407, 424 }, CanGigantamax = true }, // Sandaconda
          //new(40,05,3,SW) { Species = 834, Ability = A4, Moves = new[]{ 157, 534, 034, 317 } }, // Drednaw
          //new(40,05,3,SW) { Species = 834, Ability = A4, Moves = new[]{ 157, 534, 034, 317 }, CanGigantamax = true }, // Drednaw
          //new(50,08,4,SW) { Species = 844, Ability = A4, Moves = new[]{ 693, 529, 342, 328 }, CanGigantamax = true }, // Sandaconda
          //new(50,08,4,SW) { Species = 834, Ability = A4, Moves = new[]{ 157, 710, 350, 523 } }, // Drednaw
          //new(50,08,4,SW) { Species = 834, Ability = A4, Moves = new[]{ 157, 710, 350, 523 }, CanGigantamax = true }, // Drednaw
            new(60,10,5,SW) { Species = 012, Ability = A4, Moves = new[]{ 405, 094, 542, 202 }, CanGigantamax = true }, // Butterfree
          //new(70,10,5,SW) { Species = 844, Ability = A4, Moves = new[]{ 693, 529, 342, 328 }, CanGigantamax = true }, // Sandaconda
          //new(60,10,5,SW) { Species = 834, Ability = A4, Moves = new[]{ 157, 710, 317, 334 } }, // Drednaw
          //new(70,10,5,SW) { Species = 834, Ability = A4, Moves = new[]{ 157, 710, 317, 334 }, CanGigantamax = true }, // Drednaw

            // 12/20 - Delibird
          //new(17,01,1,SW) { Species = 012, Ability = A4, Moves = new[]{ 405, 060, 016, 078 } }, // Butterfree
          //new(17,01,1,SW) { Species = 012, Ability = A4, Moves = new[]{ 405, 060, 016, 078 }, CanGigantamax = true }, // Butterfree
          //new(17,01,1,SW) { Species = 843, Ability = A4, Moves = new[]{ 693, 523, 189, 103 } }, // Silicobra
          //new(17,01,1,SW) { Species = 833, Ability = A4, Moves = new[]{ 055, 044, 033, 213 } }, // Chewtle
          //new(30,03,2,SW) { Species = 012, Ability = A4, Moves = new[]{ 405, 403, 081, 079 }, CanGigantamax = true }, // Butterfree
          //new(30,03,2,SW) { Species = 843, Ability = A4, Moves = new[]{ 693, 523, 029, 137 } }, // Silicobra
          //new(30,03,2,SW) { Species = 834, Ability = A4, Moves = new[]{ 317, 242, 055, 334 } }, // Drednaw
          //new(40,05,3,SW) { Species = 844, Ability = A4, Moves = new[]{ 693, 529, 407, 424 }, CanGigantamax = true }, // Sandaconda
          //new(40,05,3,SW) { Species = 834, Ability = A4, Moves = new[]{ 157, 534, 034, 317 } }, // Drednaw
          //new(40,05,3,SW) { Species = 834, Ability = A4, Moves = new[]{ 157, 534, 034, 317 }, CanGigantamax = true }, // Drednaw
            new(50,08,3,SW) { Species = 844, Ability = A4, Moves = new[]{ 693, 529, 407, 424 }, CanGigantamax = true }, // Sandaconda
          //new(50,08,4,SW) { Species = 834, Ability = A4, Moves = new[]{ 157, 710, 350, 523 } }, // Drednaw
          //new(50,08,4,SW) { Species = 834, Ability = A4, Moves = new[]{ 157, 710, 350, 523 }, CanGigantamax = true }, // Drednaw
          //new(60,10,5,SW) { Species = 012, Ability = A4, Moves = new[]{ 405, 094, 542, 202 }, CanGigantamax = true }, // Butterfree
          //new(70,10,5,SW) { Species = 844, Ability = A4, Moves = new[]{ 693, 529, 342, 328 }, CanGigantamax = true }, // Sandaconda
          //new(60,10,5,SW) { Species = 834, Ability = A4, Moves = new[]{ 157, 710, 317, 334 } }, // Drednaw
          //new(70,10,5,SW) { Species = 834, Ability = A4, Moves = new[]{ 157, 710, 317, 334 }, CanGigantamax = true }, // Drednaw

            // 12/30 - Magikarp
          //new(17,01,1,SW) { Species = 012, Ability = A4, Moves = new[]{ 405, 060, 016, 078 }, CanGigantamax = true }, // Butterfree
          //new(17,01,1,SW) { Species = 843, Ability = A4, Moves = new[]{ 693, 523, 189, 103 } }, // Silicobra
          //new(17,01,1,SW) { Species = 833, Ability = A4, Moves = new[]{ 055, 044, 033, 213 } }, // Chewtle
          //new(30,03,2,SW) { Species = 012, Ability = A4, Moves = new[]{ 405, 403, 081, 079 }, CanGigantamax = true }, // Butterfree
          //new(30,03,2,SW) { Species = 843, Ability = A4, Moves = new[]{ 693, 523, 029, 137 } }, // Silicobra
          //new(30,03,2,SW) { Species = 834, Ability = A4, Moves = new[]{ 317, 242, 055, 334 } }, // Drednaw
          //new(40,05,3,SW) { Species = 844, Ability = A4, Moves = new[]{ 693, 529, 407, 424 }, CanGigantamax = true }, // Sandaconda
          //new(40,05,3,SW) { Species = 834, Ability = A4, Moves = new[]{ 157, 534, 034, 317 }, CanGigantamax = true }, // Drednaw
          //new(50,08,4,SW) { Species = 844, Ability = A4, Moves = new[]{ 693, 529, 342, 328 }, CanGigantamax = true }, // Sandaconda
          //new(50,08,4,SW) { Species = 834, Ability = A4, Moves = new[]{ 157, 710, 350, 523 }, CanGigantamax = true }, // Drednaw
          //new(60,10,5,SW) { Species = 012, Ability = A4, Moves = new[]{ 405, 094, 542, 202 }, CanGigantamax = true }, // Butterfree
          //new(70,10,5,SW) { Species = 844, Ability = A4, Moves = new[]{ 693, 529, 342, 328 }, CanGigantamax = true }, // Sandaconda
          //new(70,10,5,SW) { Species = 834, Ability = A4, Moves = new[]{ 157, 710, 317, 334 }, CanGigantamax = true }, // Drednaw

            // 1/8 - Applin
            new(17,01,1,SW) { Species = 837, Ability = A4, Moves = new[]{ 479, 033, 108, 189 } }, // Rolycoly
            new(17,01,1,SW) { Species = 837, Ability = A2, Moves = new[]{ 479, 033, 108, 189 } }, // Rolycoly
            new(30,03,2,SW) { Species = 841, Ability = A4, Moves = new[]{ 406, 491, 017, 225 } }, // Flapple
            new(30,03,2,SW) { Species = 841, Ability = A2, Moves = new[]{ 406, 491, 017, 225 } }, // Flapple
            new(30,03,2,SW) { Species = 838, Ability = A4, Moves = new[]{ 246, 510, 479, 189 } }, // Carkol
            new(30,03,2,SW) { Species = 838, Ability = A2, Moves = new[]{ 246, 510, 479, 189 } }, // Carkol
            new(40,05,3,SW) { Species = 841, Ability = A4, Moves = new[]{ 788, 406, 512, 491 } }, // Flapple
            new(40,05,3,SW) { Species = 841, Ability = A4, Moves = new[]{ 788, 406, 512, 491 }, CanGigantamax = true }, // Flapple
            new(40,05,3,SW) { Species = 869, Ability = A4, Moves = new[]{ 577, 605, 202, 186 }, Form = 3, CanGigantamax = true }, // Alcremie-3
            new(40,05,3,SW) { Species = 869, Ability = A4, Moves = new[]{ 577, 605, 202, 186 }, Form = 4, CanGigantamax = true }, // Alcremie-4
            new(40,05,3,SW) { Species = 839, Ability = A4, Moves = new[]{ 246, 510, 479, 488 } }, // Coalossal
            new(40,05,3,SW) { Species = 839, Ability = A4, Moves = new[]{ 246, 510, 479, 488 }, CanGigantamax = true }, // Coalossal
            new(50,08,4,SW) { Species = 841, Ability = A4, Moves = new[]{ 407, 788, 491, 334 } }, // Flapple
            new(50,08,4,SW) { Species = 841, Ability = A4, Moves = new[]{ 407, 788, 491, 334 }, CanGigantamax = true }, // Flapple
            new(50,08,4,SW) { Species = 869, Ability = A4, Moves = new[]{ 605, 202, 094, 151 }, Form = 1, CanGigantamax = true }, // Alcremie-1
            new(50,08,4,SW) { Species = 869, Ability = A4, Moves = new[]{ 605, 202, 094, 151 }, Form = 2, CanGigantamax = true }, // Alcremie-2
            new(50,08,4,SW) { Species = 839, Ability = A4, Moves = new[]{ 246, 053, 157, 261 } }, // Coalossal
            new(50,08,4,SW) { Species = 839, Ability = A4, Moves = new[]{ 246, 053, 157, 261 }, CanGigantamax = true }, // Coalossal
            new(60,10,5,SW) { Species = 841, Ability = A4, Moves = new[]{ 407, 788, 512, 349 } }, // Flapple
            new(60,10,5,SW) { Species = 841, Ability = A4, Moves = new[]{ 407, 788, 512, 349 }, CanGigantamax = true }, // Flapple
            new(60,10,5,SW) { Species = 869, Ability = A4, Moves = new[]{ 605, 202, 595, 500 }, Form = 5, CanGigantamax = true }, // Alcremie-5
            new(60,10,5,SW) { Species = 869, Ability = A4, Moves = new[]{ 605, 202, 595, 500 }, Form = 6, CanGigantamax = true }, // Alcremie-6
            new(60,10,5,SW) { Species = 839, Ability = A4, Moves = new[]{ 246, 053, 157, 523 } }, // Coalossal
            new(60,10,5,SW) { Species = 839, Ability = A4, Moves = new[]{ 246, 053, 157, 523 }, CanGigantamax = true }, // Coalossal

            // 1/31 - Milcery
          //new(40,05,3,SW) { Species = 841, Ability = A4, Moves = new[]{ 788, 406, 512, 491 }, CanGigantamax = true }, // Flapple
          //new(40,05,3,SW) { Species = 839, Ability = A4, Moves = new[]{ 246, 510, 479, 488 }, CanGigantamax = true }, // Coalossal
          //new(50,08,4,SW) { Species = 841, Ability = A4, Moves = new[]{ 788, 407, 491, 334 }, CanGigantamax = true }, // Flapple -- first two moves swapped match a prior distribution
          //new(50,08,4,SW) { Species = 839, Ability = A4, Moves = new[]{ 246, 053, 157, 261 }, CanGigantamax = true }, // Coalossal
          //new(60,10,5,SW) { Species = 841, Ability = A4, Moves = new[]{ 407, 788, 512, 349 }, CanGigantamax = true }, // Flapple
          //new(60,10,5,SW) { Species = 839, Ability = A4, Moves = new[]{ 246, 053, 157, 523 }, CanGigantamax = true }, // Coalossal

            // 2/6 - Toxtricity
            new(40,05,3,SW) { Species = 099, Ability = A4, Moves = new[]{ 534, 232, 023, 106 }, CanGigantamax = true }, // Kingler
            new(40,05,3,SW) { Species = 860, Ability = A4, Moves = new[]{ 492, 577, 421, 141 } }, // Morgrem
            new(40,05,3,SW) { Species = 849, Ability = A4, Moves = new[]{ 085, 474, 496, 103 }, CanGigantamax = true }, // Toxtricity
            new(50,08,4,SW) { Species = 099, Ability = A4, Moves = new[]{ 359, 667, 157, 534 }, CanGigantamax = true }, // Kingler
            new(50,08,4,SW) { Species = 861, Ability = A4, Moves = new[]{ 789, 793, 280, 409 }, CanGigantamax = true }, // Grimmsnarl
            new(50,08,4,SW) { Species = 849, Ability = A4, Moves = new[]{ 786, 506, 474, 409 }, CanGigantamax = true }, // Toxtricity
            new(60,10,5,SW) { Species = 099, Ability = A4, Moves = new[]{ 152, 667, 157, 404 }, CanGigantamax = true }, // Kingler
            new(60,10,5,SW) { Species = 861, Ability = A4, Moves = new[]{ 789, 793, 409, 007 }, CanGigantamax = true }, // Grimmsnarl
            new(60,10,5,SW) { Species = 849, Ability = A4, Moves = new[]{ 786, 586, 482, 506 }, CanGigantamax = true }, // Toxtricity

            // 2/17 - Toxel
            new(17,01,1,SW) { Species = 098, Ability = A4, Moves = new[]{ 055, 043, 106, 232 } }, // Krabby
            new(17,01,1,SW) { Species = 859, Ability = A4, Moves = new[]{ 044, 260, 590, 372 } }, // Impidimp
            new(30,03,2,SW) { Species = 099, Ability = A4, Moves = new[]{ 232, 341, 061, 023 } }, // Kingler
            new(30,03,2,SW) { Species = 099, Ability = A4, Moves = new[]{ 232, 341, 061, 023 }, CanGigantamax = true }, // Kingler
            new(30,03,2,SW) { Species = 859, Ability = A4, Moves = new[]{ 389, 577, 260, 279 } }, // Impidimp
            new(30,03,2,SW) { Species = 849, Ability = A4, Moves = new[]{ 474, 209, 268, 175 } }, // Toxtricity
            new(40,05,3,SW) { Species = 099, Ability = A4, Moves = new[]{ 534, 232, 023, 106 } }, // Kingler
          //new(40,05,3,SW) { Species = 099, Ability = A4, Moves = new[]{ 534, 232, 023, 106 }, CanGigantamax = true }, // Kingler
          //new(40,05,3,SW) { Species = 860, Ability = A4, Moves = new[]{ 492, 577, 421, 141 } }, // Morgrem
            new(40,05,3,SW) { Species = 849, Ability = A4, Moves = new[]{ 085, 474, 496, 103 } }, // Toxtricity
          //new(40,05,3,SW) { Species = 849, Ability = A4, Moves = new[]{ 085, 474, 496, 103 }, CanGigantamax = true }, // Toxtricity
            new(50,08,4,SW) { Species = 099, Ability = A4, Moves = new[]{ 359, 667, 157, 534 } }, // Kingler
          //new(50,08,4,SW) { Species = 099, Ability = A4, Moves = new[]{ 359, 667, 157, 534 }, CanGigantamax = true }, // Kingler
            new(50,08,4,SW) { Species = 861, Ability = A4, Moves = new[]{ 789, 793, 280, 409 } }, // Grimmsnarl
          //new(50,08,4,SW) { Species = 861, Ability = A4, Moves = new[]{ 789, 793, 280, 409 }, CanGigantamax = true }, // Grimmsnarl
            new(50,08,4,SW) { Species = 849, Ability = A4, Moves = new[]{ 786, 506, 474, 409 } }, // Toxtricity
          //new(50,08,4,SW) { Species = 849, Ability = A4, Moves = new[]{ 786, 506, 474, 409 }, CanGigantamax = true }, // Toxtricity
            new(60,10,5,SW) { Species = 099, Ability = A4, Moves = new[]{ 152, 667, 157, 404 } }, // Kingler
          //new(60,10,5,SW) { Species = 099, Ability = A4, Moves = new[]{ 152, 667, 157, 404 }, CanGigantamax = true }, // Kingler
            new(60,10,5,SW) { Species = 861, Ability = A4, Moves = new[]{ 789, 793, 409, 007 } }, // Grimmsnarl
          //new(60,10,5,SW) { Species = 861, Ability = A4, Moves = new[]{ 789, 793, 409, 007 }, CanGigantamax = true }, // Grimmsnarl
            new(60,10,5,SW) { Species = 849, Ability = A4, Moves = new[]{ 786, 586, 188, 506 } }, // Toxtricity
            new(60,10,5,SW) { Species = 849, Ability = A4, Moves = new[]{ 786, 586, 188, 506 }, CanGigantamax = true }, // Toxtricity

            // 2/26 - Mewtwo
          //new(17,01,1,SW) { Species = 098, Ability = A4, Moves = new[]{ 055, 043, 106, 232 } }, // Krabby
          //new(17,01,1,SW) { Species = 859, Ability = A4, Moves = new[]{ 044, 260, 590, 372 } }, // Impidimp
          //new(30,03,2,SW) { Species = 099, Ability = A4, Moves = new[]{ 232, 341, 061, 023 }, CanGigantamax = true }, // Kingler
          //new(30,03,2,SW) { Species = 859, Ability = A4, Moves = new[]{ 389, 577, 260, 279 } }, // Impidimp
            new(30,03,2,SW) { Species = 849, Ability = A4, Moves = new[]{ 084, 209, 268, 175 } }, // Toxtricity
          //new(40,05,3,SW) { Species = 099, Ability = A4, Moves = new[]{ 534, 232, 023, 106 }, CanGigantamax = true }, // Kingler
          //new(40,05,3,SW) { Species = 860, Ability = A4, Moves = new[]{ 492, 577, 421, 141 } }, // Morgrem
          //new(40,05,3,SW) { Species = 849, Ability = A4, Moves = new[]{ 085, 474, 496, 103 }, CanGigantamax = true }, // Toxtricity
          //new(50,08,4,SW) { Species = 099, Ability = A4, Moves = new[]{ 359, 667, 157, 534 }, CanGigantamax = true }, // Kingler
          //new(50,08,4,SW) { Species = 861, Ability = A4, Moves = new[]{ 789, 793, 280, 409 }, CanGigantamax = true }, // Grimmsnarl
          //new(50,08,4,SW) { Species = 849, Ability = A4, Moves = new[]{ 786, 506, 474, 409 }, CanGigantamax = true }, // Toxtricity
          //new(60,10,5,SW) { Species = 099, Ability = A4, Moves = new[]{ 152, 667, 157, 404 }, CanGigantamax = true }, // Kingler
          //new(60,10,5,SW) { Species = 861, Ability = A4, Moves = new[]{ 789, 793, 409, 007 }, CanGigantamax = true }, // Grimmsnarl
          //new(60,10,5,SW) { Species = 849, Ability = A4, Moves = new[]{ 786, 586, 188, 506 }, CanGigantamax = true }, // Toxtricity

            // 3/8 - Gengar/Machamp
            new(60,10,5,SW) { Species = 068, Ability = A4, Moves = new[]{ 238, 007, 008, 089 } }, // Machamp
            new(60,10,5,SW) { Species = 068, Ability = A4, Moves = new[]{ 238, 007, 008, 089 }, CanGigantamax = true }, // Machamp

            // 3/18 Exclusives & Food
            new(17,01,1,SW) { Species = 222, Ability = A4, Moves = new[]{ 033, 106, 310, 050 }, Form = 1 }, // Corsola-1
            new(30,03,2,SW) { Species = 077, Ability = A4, Moves = new[]{ 093, 584, 060, 023 }, Form = 1 }, // Ponyta-1
            new(30,03,2,SW) { Species = 222, Ability = A4, Moves = new[]{ 310, 050, 246, 506 }, Form = 1 }, // Corsola-1
            new(40,05,3,SW) { Species = 077, Ability = A2, Moves = new[]{ 340, 023, 428, 583 }, Form = 1 }, // Ponyta-1
            new(40,05,3,SW) { Species = 222, Ability = A2, Moves = new[]{ 506, 408, 503, 261 }, Form = 1 }, // Corsola-1
            new(60,10,5,SW) { Species = 765, Ability = A2, Moves = new[]{ 492, 094, 085, 247 } }, // Oranguru
            new(60,10,5,SW) { Species = 876, Ability = A2, Moves = new[]{ 094, 595, 605, 304 }, Form = 1 }, // Indeedee-1
            new(60,10,5,SW) { Species = 630, Ability = A2, Moves = new[]{ 403, 555, 492, 211 } }, // Mandibuzz
            new(60,10,5,SW) { Species = 078, Ability = A2, Moves = new[]{ 428, 583, 224, 340 }, Form = 1 }, // Rapidash-1
          //new(60,10,5,SW) { Species = 068, Ability = A4, Moves = new[]{ 238, 007, 008, 089 }, CanGigantamax = true }, // Machamp

            // 3/25 - Charizard
            new(60,10,5,SW) { Species = 879, Ability = A4, Moves = new[]{ 442, 583, 438, 089 }, CanGigantamax = true }, // Copperajah
            new(60,10,5,SW) { Species = 884, Ability = A4, Moves = new[]{ 430, 406, 085, 334 }, CanGigantamax = true }, // Duraludon

            // 4/27 - Meta
            new(17,01,1,SW) { Species = 479, Ability = A4, Moves = new[]{ 104, 084, 109 }, Form = 1 }, // Rotom-1
            new(30,03,2,SW) { Species = 479, Ability = A4, Moves = new[]{ 104, 085, 109 }, Form = 1 }, // Rotom-1
            new(40,05,3,SW) { Species = 479, Ability = A4, Moves = new[]{ 521, 085, 506 }, Form = 1 }, // Rotom-1
            new(50,08,4,SW) { Species = 479, Ability = A4, Moves = new[]{ 521, 085, 261 }, Form = 1 }, // Rotom-1
            new(60,10,5,SW) { Species = 479, Ability = A4, Moves = new[]{ 521, 435, 261 }, Form = 1 }, // Rotom-1

            // 6/2 - Gigantamax
            new(17,01,1,SW) { Species = 869, Ability = A4, Moves = new[]{ 033, 186, 577, 230 }, CanGigantamax = true }, // Alcremie
            new(30,03,2,SW) { Species = 851, Ability = A4, Moves = new[]{ 044, 172, 489, 693 }, CanGigantamax = true }, // Centiskorch
            new(30,03,2,SW) { Species = 131, Ability = A4, Moves = new[]{ 352, 420, 109, 047 }, CanGigantamax = true }, // Lapras
          //new(40,05,3,SW) { Species = 099, Ability = A4, Moves = new[]{ 534, 232, 023, 106 }, CanGigantamax = true }, // Kingler
            new(40,05,3,SW) { Species = 842, Ability = A4, Moves = new[]{ 787, 496, 406, 523 }, CanGigantamax = true }, // Appletun
            new(40,05,3,SW) { Species = 851, Ability = A4, Moves = new[]{ 141, 424, 422, 044 }, CanGigantamax = true }, // Centiskorch
            new(50,08,4,SW) { Species = 823, Ability = A4, Moves = new[]{ 413, 442, 269, 103 }, CanGigantamax = true }, // Corviknight
          //new(50,08,4,SW) { Species = 861, Ability = A4, Moves = new[]{ 789, 793, 280, 409 }, CanGigantamax = true }, // Grimmsnarl
            new(50,08,4,SW) { Species = 569, Ability = A4, Moves = new[]{ 188, 499, 034, 707 }, CanGigantamax = true }, // Garbodor
            new(50,08,4,SW) { Species = 869, Ability = A4, Moves = new[]{ 577, 605, 105, 500 }, CanGigantamax = true }, // Alcremie
            new(60,10,5,SW) { Species = 131, Ability = A4, Moves = new[]{ 057, 196, 058, 329 }, CanGigantamax = true }, // Lapras
            new(60,10,5,SW) { Species = 849, Ability = A4, Moves = new[]{ 786, 506, 474, 409 }, CanGigantamax = true }, // Toxtricity
            new(60,10,5,SW) { Species = 094, Ability = A4, Moves = new[]{ 247, 482, 094, 196 }, CanGigantamax = true }, // Gengar
          //new(60,10,5,SW) { Species = 884, Ability = A4, Moves = new[]{ 430, 406, 085, 334 }, CanGigantamax = true }, // Duraludon

            // 8/31 - Electric Grass
            new(30,03,2,SW) { Species = 849, Ability = A4, Moves = new[]{ 351, 506, 491, 103 }, Form = 1, CanGigantamax = true }, // Toxtricity-1
            new(30,03,2,SW) { Species = 842, Ability = A4, Moves = new[]{ 787, 496, 310, 029 }, CanGigantamax = true }, // Appletun
            new(40,05,3,SW) { Species = 849, Ability = A4, Moves = new[]{ 435, 506, 398, 103 }, Form = 1, CanGigantamax = true }, // Toxtricity-1
            new(40,05,3,SW) { Species = 842, Ability = A4, Moves = new[]{ 787, 496, 406, 029 }, CanGigantamax = true }, // Appletun
            new(50,08,4,SW) { Species = 849, Ability = A4, Moves = new[]{ 786, 506, 398, 586 }, Form = 1, CanGigantamax = true }, // Toxtricity-1
            new(50,08,4,SW) { Species = 842, Ability = A4, Moves = new[]{ 787, 496, 406, 523 }, CanGigantamax = true }, // Appletun
            new(60,10,5,SW) { Species = 849, Ability = A4, Moves = new[]{ 786, 506, 599, 586 }, Form = 1, CanGigantamax = true }, // Toxtricity-1
            new(60,10,5,SW) { Species = 842, Ability = A4, Moves = new[]{ 787, 034, 406, 523 }, CanGigantamax = true }, // Appletun
        };

        internal static readonly EncounterStatic8ND[] Dist_SH =
        {
            // 11/15 - Butterfree
            new(17,01,1,SH) { Species = 012, Ability = A4, Moves = new[]{ 405, 060, 016, 079 } }, // Butterfree
            new(17,01,1,SH) { Species = 012, Ability = A4, Moves = new[]{ 405, 060, 016, 079 }, CanGigantamax = true }, // Butterfree
            new(17,01,1,SH) { Species = 821, Ability = A2, Moves = new[]{ 365, 031, 526, 064 } }, // Rookidee
            new(17,01,1,SH) { Species = 821, Ability = A4, Moves = new[]{ 365, 031, 526, 064 } }, // Rookidee
            new(17,01,1,SH) { Species = 850, Ability = A2, Moves = new[]{ 044, 172, 450, 693 } }, // Sizzlipede
            new(17,01,1,SH) { Species = 850, Ability = A4, Moves = new[]{ 044, 172, 450, 693 } }, // Sizzlipede
            new(30,03,2,SH) { Species = 012, Ability = A4, Moves = new[]{ 405, 403, 081, 077 } }, // Butterfree
            new(30,03,2,SH) { Species = 012, Ability = A4, Moves = new[]{ 405, 403, 081, 077 }, CanGigantamax = true }, // Butterfree
            new(30,03,2,SH) { Species = 822, Ability = A2, Moves = new[]{ 365, 263, 179, 468 } }, // Corvisquire
            new(30,03,2,SH) { Species = 822, Ability = A4, Moves = new[]{ 365, 263, 179, 468 } }, // Corvisquire
            new(30,03,2,SH) { Species = 851, Ability = A2, Moves = new[]{ 172, 242, 450, 257 } }, // Centiskorch
            new(30,03,2,SH) { Species = 851, Ability = A4, Moves = new[]{ 172, 242, 450, 257 } }, // Centiskorch
            new(40,05,3,SH) { Species = 823, Ability = A4, Moves = new[]{ 065, 211, 034, 103 } }, // Corviknight
            new(40,05,3,SH) { Species = 823, Ability = A4, Moves = new[]{ 065, 211, 034, 103 }, CanGigantamax = true }, // Corviknight
            new(40,05,3,SH) { Species = 851, Ability = A4, Moves = new[]{ 141, 438, 053, 489 } }, // Centiskorch
            new(40,05,3,SH) { Species = 851, Ability = A4, Moves = new[]{ 141, 438, 053, 489 }, CanGigantamax = true }, // Centiskorch
            new(50,08,4,SH) { Species = 823, Ability = A4, Moves = new[]{ 065, 442, 034, 334 } }, // Corviknight
            new(50,08,4,SH) { Species = 823, Ability = A4, Moves = new[]{ 065, 442, 034, 334 }, CanGigantamax = true }, // Corviknight
            new(50,08,4,SH) { Species = 851, Ability = A4, Moves = new[]{ 141, 424, 422, 242 } }, // Centiskorch
            new(50,08,4,SH) { Species = 851, Ability = A4, Moves = new[]{ 141, 424, 422, 242 }, CanGigantamax = true }, // Centiskorch
            new(60,10,5,SH) { Species = 012, Ability = A4, Moves = new[]{ 405, 094, 403, 079 } }, // Butterfree
            new(70,10,5,SH) { Species = 012, Ability = A4, Moves = new[]{ 405, 094, 403, 079 }, CanGigantamax = true }, // Butterfree
            new(60,10,5,SH) { Species = 823, Ability = A4, Moves = new[]{ 413, 442, 249, 103 } }, // Corviknight
            new(70,10,5,SH) { Species = 823, Ability = A4, Moves = new[]{ 413, 442, 249, 103 }, CanGigantamax = true }, // Corviknight
            new(60,10,5,SH) { Species = 851, Ability = A4, Moves = new[]{ 679, 257, 083, 438 } }, // Centiskorch
            new(70,10,5,SH) { Species = 851, Ability = A4, Moves = new[]{ 679, 257, 083, 438 }, CanGigantamax = true }, // Centiskorch

            // 12/03 - Snorlax
          //new(17,01,1,SH) { Species = 012, Ability = A4, Moves = new[]{ 405, 060, 016, 079 } }, // Butterfree
          //new(17,01,1,SH) { Species = 012, Ability = A4, Moves = new[]{ 405, 060, 016, 079 }, CanGigantamax = true }, // Butterfree
            new(17,01,1,SH) { Species = 821, Ability = A4, Moves = new[]{ 403, 031, 116, 064 } }, // Rookidee
            new(17,01,1,SH) { Species = 850, Ability = A4, Moves = new[]{ 044, 172, 404, 693 } }, // Sizzlipede
          //new(30,03,2,SH) { Species = 012, Ability = A4, Moves = new[]{ 405, 403, 081, 077 }, CanGigantamax = true }, // Butterfree
            new(30,03,2,SH) { Species = 822, Ability = A4, Moves = new[]{ 403, 263, 179, 468 } }, // Corvisquire
            new(30,03,2,SH) { Species = 851, Ability = A4, Moves = new[]{ 172, 242, 404, 257 } }, // Centiskorch
            new(30,03,2,SH) { Species = 851, Ability = A2, Moves = new[]{ 172, 242, 404, 257 } }, // Centiskorch
          //new(40,05,3,SH) { Species = 823, Ability = A4, Moves = new[]{ 065, 211, 034, 103 }, CanGigantamax = true }, // Corviknight
          //new(40,05,3,SH) { Species = 851, Ability = A4, Moves = new[]{ 141, 438, 053, 489 } }, // Centiskorch
          //new(40,05,3,SH) { Species = 851, Ability = A4, Moves = new[]{ 141, 438, 053, 489 }, CanGigantamax = true }, // Centiskorch
          //new(50,08,4,SH) { Species = 823, Ability = A4, Moves = new[]{ 065, 442, 034, 334 }, CanGigantamax = true }, // Corviknight
          //new(50,08,4,SH) { Species = 851, Ability = A4, Moves = new[]{ 141, 424, 422, 242 } }, // Centiskorch
          //new(50,08,4,SH) { Species = 851, Ability = A4, Moves = new[]{ 141, 424, 422, 242 }, CanGigantamax = true }, // Centiskorch
            new(60,10,5,SH) { Species = 012, Ability = A4, Moves = new[]{ 405, 094, 403, 079 }, CanGigantamax = true }, // Butterfree
          //new(70,10,5,SH) { Species = 823, Ability = A4, Moves = new[]{ 413, 442, 249, 103 }, CanGigantamax = true }, // Corviknight
          //new(60,10,5,SH) { Species = 851, Ability = A4, Moves = new[]{ 679, 257, 083, 438 } }, // Centiskorch
          //new(70,10,5,SH) { Species = 851, Ability = A4, Moves = new[]{ 679, 257, 083, 438 }, CanGigantamax = true }, // Centiskorch

            // 12/20 - Delibird
          //new(17,01,1,SH) { Species = 012, Ability = A4, Moves = new[]{ 405, 060, 016, 079 } }, // Butterfree
          //new(17,01,1,SH) { Species = 012, Ability = A4, Moves = new[]{ 405, 060, 016, 079 }, CanGigantamax = true }, // Butterfree
          //new(17,01,1,SH) { Species = 821, Ability = A4, Moves = new[]{ 403, 031, 116, 064 } }, // Rookidee
            new(17,01,1,SH) { Species = 850, Ability = A4, Moves = new[]{ 044, 172, 141, 693 } }, // Sizzlipede
          //new(30,03,2,SH) { Species = 012, Ability = A4, Moves = new[]{ 405, 403, 081, 077 }, CanGigantamax = true }, // Butterfree
          //new(30,03,2,SH) { Species = 822, Ability = A4, Moves = new[]{ 403, 263, 179, 468 } }, // Corvisquire
          //new(30,03,2,SH) { Species = 851, Ability = A4, Moves = new[]{ 172, 242, 404, 257 } }, // Centiskorch
          //new(40,05,3,SH) { Species = 823, Ability = A4, Moves = new[]{ 065, 211, 034, 103 }, CanGigantamax = true }, // Corviknight
          //new(40,05,3,SH) { Species = 851, Ability = A4, Moves = new[]{ 141, 438, 053, 489 } }, // Centiskorch
          //new(40,05,3,SH) { Species = 851, Ability = A4, Moves = new[]{ 141, 438, 053, 489 }, CanGigantamax = true }, // Centiskorch
          //new(50,08,4,SH) { Species = 823, Ability = A4, Moves = new[]{ 065, 442, 034, 334 }, CanGigantamax = true }, // Corviknight
          //new(50,08,4,SH) { Species = 851, Ability = A4, Moves = new[]{ 141, 424, 422, 242 } }, // Centiskorch
          //new(50,08,4,SH) { Species = 851, Ability = A4, Moves = new[]{ 141, 424, 422, 242 }, CanGigantamax = true }, // Centiskorch
          //new(60,10,5,SH) { Species = 012, Ability = A4, Moves = new[]{ 405, 094, 403, 079 }, CanGigantamax = true }, // Butterfree
          //new(70,10,5,SH) { Species = 823, Ability = A4, Moves = new[]{ 413, 442, 249, 103 }, CanGigantamax = true }, // Corviknight
          //new(60,10,5,SH) { Species = 851, Ability = A4, Moves = new[]{ 679, 257, 083, 438 } }, // Centiskorch
          //new(70,10,5,SH) { Species = 851, Ability = A4, Moves = new[]{ 679, 257, 083, 438 }, CanGigantamax = true }, // Centiskorch

            // 12/30 - Magikarp
          //new(17,01,1,SH) { Species = 012, Ability = A4, Moves = new[]{ 405, 060, 016, 079 }, CanGigantamax = true }, // Butterfree
          //new(17,01,1,SH) { Species = 821, Ability = A4, Moves = new[]{ 403, 031, 116, 064 } }, // Rookidee
          //new(17,01,1,SH) { Species = 850, Ability = A4, Moves = new[]{ 044, 172, 141, 693 } }, // Sizzlipede
          //new(30,03,2,SH) { Species = 012, Ability = A4, Moves = new[]{ 405, 403, 081, 077 }, CanGigantamax = true }, // Butterfree
          //new(30,03,2,SH) { Species = 822, Ability = A4, Moves = new[]{ 403, 263, 179, 468 } }, // Corvisquire
          //new(30,03,2,SH) { Species = 851, Ability = A4, Moves = new[]{ 172, 242, 404, 257 } }, // Centiskorch
          //new(40,05,3,SH) { Species = 823, Ability = A4, Moves = new[]{ 065, 211, 034, 103 }, CanGigantamax = true }, // Corviknight
          //new(40,05,3,SH) { Species = 851, Ability = A4, Moves = new[]{ 141, 438, 053, 489 }, CanGigantamax = true }, // Centiskorch
          //new(50,08,4,SH) { Species = 823, Ability = A4, Moves = new[]{ 065, 442, 034, 334 }, CanGigantamax = true }, // Corviknight
          //new(50,08,4,SH) { Species = 851, Ability = A4, Moves = new[]{ 141, 424, 422, 242 }, CanGigantamax = true }, // Centiskorch
          //new(60,10,5,SH) { Species = 012, Ability = A4, Moves = new[]{ 405, 094, 403, 079 }, CanGigantamax = true }, // Butterfree
          //new(70,10,5,SH) { Species = 823, Ability = A4, Moves = new[]{ 413, 442, 249, 103 }, CanGigantamax = true }, // Corviknight
          //new(70,10,5,SH) { Species = 851, Ability = A4, Moves = new[]{ 679, 257, 083, 438 }, CanGigantamax = true }, // Centiskorch

            // 1/8 - Applin
            new(17,01,1,SH) { Species = 131, Ability = A4, Moves = new[]{ 055, 496, 045, 047 } }, // Lapras
            new(17,01,1,SH) { Species = 131, Ability = A2, Moves = new[]{ 055, 496, 045, 047 } }, // Lapras
            new(30,03,2,SH) { Species = 842, Ability = A4, Moves = new[]{ 787, 029, 389, 073 } }, // Appletun
            new(30,03,2,SH) { Species = 842, Ability = A2, Moves = new[]{ 787, 029, 389, 073 } }, // Appletun
            new(30,03,2,SH) { Species = 131, Ability = A4, Moves = new[]{ 352, 420, 109, 047 } }, // Lapras
            new(30,03,2,SH) { Species = 131, Ability = A2, Moves = new[]{ 352, 420, 109, 047 } }, // Lapras
            new(40,05,3,SH) { Species = 842, Ability = A4, Moves = new[]{ 787, 496, 406, 523 } }, // Appletun
            new(40,05,3,SH) { Species = 842, Ability = A4, Moves = new[]{ 787, 496, 406, 523 }, CanGigantamax = true }, // Appletun
            new(40,05,3,SH) { Species = 869, Ability = A4, Moves = new[]{ 577, 605, 202, 186 }, Form = 1, CanGigantamax = true }, // Alcremie-1
            new(40,05,3,SH) { Species = 869, Ability = A4, Moves = new[]{ 577, 605, 202, 186 }, Form = 2, CanGigantamax = true }, // Alcremie-2
            new(40,05,3,SH) { Species = 131, Ability = A4, Moves = new[]{ 352, 420, 109, 034 } }, // Lapras
            new(40,05,3,SH) { Species = 131, Ability = A4, Moves = new[]{ 352, 420, 109, 034 }, CanGigantamax = true }, // Lapras
            new(50,08,4,SH) { Species = 842, Ability = A4, Moves = new[]{ 787, 202, 406, 089 } }, // Appletun
            new(50,08,4,SH) { Species = 842, Ability = A4, Moves = new[]{ 787, 202, 406, 089 }, CanGigantamax = true }, // Appletun
            new(50,08,4,SH) { Species = 869, Ability = A4, Moves = new[]{ 605, 202, 094, 151 }, Form = 7, CanGigantamax = true }, // Alcremie-7
            new(50,08,4,SH) { Species = 869, Ability = A4, Moves = new[]{ 605, 202, 094, 151 }, Form = 8, CanGigantamax = true }, // Alcremie-8
            new(50,08,4,SH) { Species = 131, Ability = A4, Moves = new[]{ 057, 058, 246, 523 } }, // Lapras
            new(50,08,4,SH) { Species = 131, Ability = A4, Moves = new[]{ 057, 058, 246, 523 }, CanGigantamax = true }, // Lapras
            new(60,10,5,SH) { Species = 842, Ability = A4, Moves = new[]{ 787, 406, 412, 089 } }, // Appletun
            new(60,10,5,SH) { Species = 842, Ability = A4, Moves = new[]{ 787, 406, 412, 089 }, CanGigantamax = true }, // Appletun
            new(60,10,5,SH) { Species = 869, Ability = A4, Moves = new[]{ 605, 202, 595, 500 }, Form = 3, CanGigantamax = true }, // Alcremie-3
            new(60,10,5,SH) { Species = 869, Ability = A4, Moves = new[]{ 605, 202, 595, 500 }, Form = 4, CanGigantamax = true }, // Alcremie-4
            new(60,10,5,SH) { Species = 131, Ability = A4, Moves = new[]{ 057, 196, 573, 329 } }, // Lapras
            new(60,10,5,SH) { Species = 131, Ability = A4, Moves = new[]{ 057, 196, 573, 329 }, CanGigantamax = true }, // Lapras

            // 1/31 - Milcery
          //new(40,05,3,SH) { Species = 842, Ability = A4, Moves = new[]{ 787, 496, 406, 523 }, CanGigantamax = true }, // Appletun
          //new(40,05,3,SH) { Species = 131, Ability = A4, Moves = new[]{ 352, 420, 109, 034 }, CanGigantamax = true }, // Lapras
          //new(50,08,4,SH) { Species = 842, Ability = A4, Moves = new[]{ 787, 202, 406, 089 }, CanGigantamax = true }, // Appletun
          //new(50,08,4,SH) { Species = 131, Ability = A4, Moves = new[]{ 057, 058, 246, 523 }, CanGigantamax = true }, // Lapras
          //new(60,10,5,SH) { Species = 842, Ability = A4, Moves = new[]{ 787, 406, 412, 089 }, CanGigantamax = true }, // Appletun
          //new(60,10,5,SH) { Species = 131, Ability = A4, Moves = new[]{ 057, 196, 573, 329 }, CanGigantamax = true }, // Lapras

            // 2/6 - Toxtricity
            new(40,05,3,SH) { Species = 826, Ability = A4, Moves = new[]{ 522, 060, 109, 202 }, CanGigantamax = true }, // Orbeetle
            new(40,05,3,SH) { Species = 857, Ability = A4, Moves = new[]{ 605, 345, 399, 500 } }, // Hattrem
            new(40,05,3,SH) { Species = 849, Ability = A4, Moves = new[]{ 085, 599, 496, 103 }, Form = 1, CanGigantamax = true }, // Toxtricity-1
            new(50,08,4,SH) { Species = 826, Ability = A4, Moves = new[]{ 405, 094, 202, 247 }, CanGigantamax = true }, // Orbeetle
            new(50,08,4,SH) { Species = 858, Ability = A4, Moves = new[]{ 605, 094, 595, 247 }, CanGigantamax = true }, // Hatterene
            new(50,08,4,SH) { Species = 849, Ability = A4, Moves = new[]{ 786, 506, 599, 409 }, Form = 1, CanGigantamax = true }, // Toxtricity-1
            new(60,10,5,SH) { Species = 826, Ability = A4, Moves = new[]{ 405, 094, 247, 347 }, CanGigantamax = true }, // Orbeetle
            new(60,10,5,SH) { Species = 858, Ability = A4, Moves = new[]{ 605, 094, 595, 500 }, CanGigantamax = true }, // Hatterene
            new(60,10,5,SH) { Species = 849, Ability = A4, Moves = new[]{ 786, 586, 482, 506 }, Form = 1, CanGigantamax = true }, // Toxtricity-1

            // 2/17 - Toxel
            new(17,01,1,SH) { Species = 825, Ability = A4, Moves = new[]{ 093, 522, 113, 115 } }, // Dottler
            new(17,01,1,SH) { Species = 856, Ability = A4, Moves = new[]{ 093, 589, 791, 574 } }, // Hatenna
            new(30,03,2,SH) { Species = 826, Ability = A4, Moves = new[]{ 405, 060, 496, 095 } }, // Orbeetle
            new(30,03,2,SH) { Species = 826, Ability = A4, Moves = new[]{ 405, 060, 496, 095 }, CanGigantamax = true }, // Orbeetle
            new(30,03,2,SH) { Species = 856, Ability = A4, Moves = new[]{ 605, 060, 345, 347 } }, // Hatenna
            new(30,03,2,SH) { Species = 849, Ability = A4, Moves = new[]{ 599, 209, 268, 175 }, Form = 1 }, // Toxtricity-1
            new(40,05,3,SH) { Species = 826, Ability = A4, Moves = new[]{ 405, 060, 202, 109 } }, // Orbeetle
            new(40,05,3,SH) { Species = 826, Ability = A4, Moves = new[]{ 405, 060, 202, 109 }, CanGigantamax = true }, // Orbeetle
          //new(40,05,3,SH) { Species = 857, Ability = A4, Moves = new[]{ 605, 345, 399, 500 } }, // Hattrem
            new(40,05,3,SH) { Species = 849, Ability = A4, Moves = new[]{ 085, 599, 496, 103 }, Form = 1 }, // Toxtricity-1
          //new(40,05,3,SH) { Species = 849, Ability = A4, Moves = new[]{ 085, 599, 496, 103 }, Form = 1, CanGigantamax = true }, // Toxtricity-1
            new(50,08,4,SH) { Species = 826, Ability = A4, Moves = new[]{ 405, 094, 202, 247 } }, // Orbeetle
          //new(50,08,4,SH) { Species = 826, Ability = A4, Moves = new[]{ 405, 094, 202, 247 }, CanGigantamax = true }, // Orbeetle
            new(50,08,4,SH) { Species = 858, Ability = A4, Moves = new[]{ 605, 094, 595, 247 } }, // Hatterene
          //new(50,08,4,SH) { Species = 858, Ability = A4, Moves = new[]{ 605, 094, 595, 247 }, CanGigantamax = true }, // Hatterene
            new(50,08,4,SH) { Species = 849, Ability = A4, Moves = new[]{ 786, 506, 599, 409 }, Form = 1 }, // Toxtricity-1
          //new(50,08,4,SH) { Species = 849, Ability = A4, Moves = new[]{ 786, 506, 599, 409 }, Form = 1, CanGigantamax = true }, // Toxtricity-1
            new(60,10,5,SH) { Species = 826, Ability = A4, Moves = new[]{ 405, 094, 247, 347 } }, // Orbeetle
          //new(60,10,5,SH) { Species = 826, Ability = A4, Moves = new[]{ 405, 094, 247, 347 }, CanGigantamax = true }, // Orbeetle
            new(60,10,5,SH) { Species = 858, Ability = A4, Moves = new[]{ 605, 094, 595, 500 } }, // Hatterene
          //new(60,10,5,SH) { Species = 858, Ability = A4, Moves = new[]{ 605, 094, 595, 500 }, CanGigantamax = true }, // Hatterene
            new(60,10,5,SH) { Species = 849, Ability = A4, Moves = new[]{ 786, 586, 188, 506 }, Form = 1 }, // Toxtricity-1
            new(60,10,5,SH) { Species = 849, Ability = A4, Moves = new[]{ 786, 586, 188, 506 }, Form = 1, CanGigantamax = true }, // Toxtricity-1

            // 2/26 - Mewtwo
          //new(17,01,1,SH) { Species = 825, Ability = A4, Moves = new[]{ 093, 522, 113, 115 } }, // Dottler
          //new(17,01,1,SH) { Species = 856, Ability = A4, Moves = new[]{ 093, 589, 791, 574 } }, // Hatenna
          //new(30,03,2,SH) { Species = 826, Ability = A4, Moves = new[]{ 405, 060, 496, 095 }, CanGigantamax = true }, // Orbeetle
          //new(30,03,2,SH) { Species = 856, Ability = A4, Moves = new[]{ 605, 060, 345, 347 } }, // Hatenna
          //new(30,03,2,SH) { Species = 849, Ability = A4, Moves = new[]{ 599, 209, 268, 175 }, Form = 1 }, // Toxtricity-1
          //new(40,05,3,SH) { Species = 826, Ability = A4, Moves = new[]{ 405, 060, 202, 109 }, CanGigantamax = true }, // Orbeetle
          //new(40,05,3,SH) { Species = 857, Ability = A4, Moves = new[]{ 605, 345, 399, 500 } }, // Hattrem
          //new(40,05,3,SH) { Species = 849, Ability = A4, Moves = new[]{ 085, 599, 496, 103 }, Form = 1, CanGigantamax = true }, // Toxtricity-1
          //new(50,08,4,SH) { Species = 826, Ability = A4, Moves = new[]{ 405, 094, 202, 247 }, CanGigantamax = true }, // Orbeetle
          //new(50,08,4,SH) { Species = 858, Ability = A4, Moves = new[]{ 605, 094, 595, 247 }, CanGigantamax = true }, // Hatterene
          //new(50,08,4,SH) { Species = 849, Ability = A4, Moves = new[]{ 786, 506, 599, 409 }, Form = 1, CanGigantamax = true }, // Toxtricity-1
          //new(60,10,5,SH) { Species = 826, Ability = A4, Moves = new[]{ 405, 094, 247, 347 }, CanGigantamax = true }, // Orbeetle
          //new(60,10,5,SH) { Species = 858, Ability = A4, Moves = new[]{ 605, 094, 595, 500 }, CanGigantamax = true }, // Hatterene
          //new(60,10,5,SH) { Species = 849, Ability = A4, Moves = new[]{ 786, 586, 188, 506 }, Form = 1, CanGigantamax = true }, // Toxtricity-1

            // 3/8 - Gengar/Machamp
            new(60,10,5,SH) { Species = 094, Ability = A4, Moves = new[]{ 247, 482, 094, 196 } }, // Gengar
            new(60,10,5,SH) { Species = 094, Ability = A4, Moves = new[]{ 247, 482, 094, 196 }, CanGigantamax = true }, // Gengar

            // 3/18 - Exclusives & Food
            new(17,01,1,SH) { Species = 554, Ability = A4, Moves = new[]{ 033, 181, 044, 419 }, Form = 1 }, // Darumaka-1
            new(30,03,2,SH) { Species = 083, Ability = A4, Moves = new[]{ 064, 028, 249, 693 }, Form = 1 }, // Farfetch’d-1
            new(30,03,2,SH) { Species = 554, Ability = A4, Moves = new[]{ 423, 029, 424, 280 }, Form = 1 }, // Darumaka-1
            new(40,05,3,SH) { Species = 083, Ability = A2, Moves = new[]{ 280, 693, 348, 413 }, Form = 1 }, // Farfetch’d-1
            new(40,05,3,SH) { Species = 554, Ability = A2, Moves = new[]{ 008, 007, 428, 276 }, Form = 1 }, // Darumaka-1
            new(60,10,5,SH) { Species = 766, Ability = A2, Moves = new[]{ 370, 157, 523, 231 } }, // Passimian
            new(60,10,5,SH) { Species = 876, Ability = A2, Moves = new[]{ 094, 595, 605, 247 } }, // Indeedee
            new(60,10,5,SH) { Species = 628, Ability = A2, Moves = new[]{ 413, 276, 442, 157 } }, // Braviary
            new(60,10,5,SH) { Species = 865, Ability = A2, Moves = new[]{ 370, 413, 211, 675 } }, // Sirfetch’d
          //new(60,10,5,SH) { Species = 094, Ability = A4, Moves = new[]{ 247, 482, 094, 196 }, CanGigantamax = true }, // Gengar

            // 3/25 - Charizard
            new(60,10,5,SH) { Species = 569, Ability = A4, Moves = new[]{ 441, 409, 402, 707 }, CanGigantamax = true }, // Garbodor
            new(60,10,5,SH) { Species = 006, Ability = A4, Moves = new[]{ 257, 403, 406, 411 }, CanGigantamax = true, Shiny = Shiny.Never }, // Charizard

            // 4/09 - Easter Eggs (and fixed Charizard)
            new(60,10,5,SH) { Species = 569, Ability = A4, Moves = new[]{ 441, 499, 402, 707 }, CanGigantamax = true }, // Garbodor (Drain Punch -> Clear Smog)
            new(60,10,5,SH) { Species = 006, Ability = A4, Moves = new[]{ 053, 403, 019, 411 }, CanGigantamax = true }, // Charizard (Heat Wave, Dragon Pulse -> Flamethrower/Fly)

            // 4/27 - Meta
            new(17,01,1,SH) { Species = 479, Ability = A4, Moves = new[]{ 104, 435, 084, 109 }, Form = 2 }, // Rotom-2 // From initial revision: Discharge @lv17 isn't legal, but they distributed it!
            // new(17,01,1,SH) { Species = 529, Ability = A4, Moves = new[]{ 189, 232, 056, 468 } }, // Drilbur // From initial revision: treat this as illegal.
            new(17,01,1,SH) { Species = 479, Ability = A4, Moves = new[]{ 104, 084, 109 }, Form = 2 }, // Rotom-2
            new(30,03,2,SH) { Species = 479, Ability = A4, Moves = new[]{ 104, 085, 109 }, Form = 2 }, // Rotom-2
            new(40,05,3,SH) { Species = 479, Ability = A4, Moves = new[]{ 521, 435, 085 }, Form = 2 }, // Rotom-2
            new(50,08,4,SH) { Species = 479, Ability = A4, Moves = new[]{ 521, 435, 247 }, Form = 2 }, // Rotom-2
            new(60,10,5,SH) { Species = 479, Ability = A4, Moves = new[]{ 521, 247, 261 }, Form = 2 }, // Rotom-2

            // 6/2 - Gigantamax
          //new(17,01,1,SH) { Species = 012, Ability = A4, Moves = new[]{ 405, 060, 016, 079 }, CanGigantamax = true }, // Butterfree
          //new(30,03,2,SH) { Species = 826, Ability = A4, Moves = new[]{ 405, 060, 496, 095 }, CanGigantamax = true }, // Orbeetle
            new(30,03,2,SH) { Species = 068, Ability = A4, Moves = new[]{ 523, 490, 279, 233 }, CanGigantamax = true }, // Machamp
            new(40,05,3,SH) { Species = 826, Ability = A4, Moves = new[]{ 405, 094, 202, 247 }, CanGigantamax = true }, // Orbeetle
            new(40,05,3,SH) { Species = 841, Ability = A4, Moves = new[]{ 406, 788, 491, 334 }, CanGigantamax = true }, // Flapple
            new(40,05,3,SH) { Species = 844, Ability = A4, Moves = new[]{ 693, 529, 201, 091 }, CanGigantamax = true }, // Sandaconda
          //new(50,08,4,SH) { Species = 834, Ability = A4, Moves = new[]{ 157, 710, 317, 334 }, CanGigantamax = true }, // Drednaw -- duplicate with raid on 7/31 (SW & SH)
          //new(50,08,4,SH) { Species = 858, Ability = A4, Moves = new[]{ 605, 094, 595, 247 }, CanGigantamax = true }, // Hatterene
          //new(50,08,4,SH) { Species = 006, Ability = A4, Moves = new[]{ 053, 403, 076, 257 }, CanGigantamax = true }, // Charizard
          //new(50,08,4,SH) { Species = 012, Ability = A4, Moves = new[]{ 405, 403, 527, 078 }, CanGigantamax = true }, // Butterfree
            new(60,10,5,SH) { Species = 849, Ability = A4, Moves = new[]{ 786, 506, 599, 409 }, Form = 1, CanGigantamax = true }, // Toxtricity-1
            new(60,10,5,SH) { Species = 839, Ability = A4, Moves = new[]{ 246, 053, 157, 523 }, CanGigantamax = true }, // Coalossal
            new(60,10,5,SH) { Species = 068, Ability = A4, Moves = new[]{ 238, 007, 008, 089 }, CanGigantamax = true }, // Machamp
            new(60,10,5,SH) { Species = 879, Ability = A4, Moves = new[]{ 442, 583, 438, 089 }, CanGigantamax = true }, // Copperajah

            // 8/31 - Electric Grass
            new(30,03,2,SH) { Species = 849, Ability = A4, Moves = new[]{ 351, 506, 491, 103 }, CanGigantamax = true }, // Toxtricity
            new(30,03,2,SH) { Species = 841, Ability = A4, Moves = new[]{ 406, 073, 491, 184 }, CanGigantamax = true }, // Flapple
            new(40,05,3,SH) { Species = 849, Ability = A4, Moves = new[]{ 435, 506, 474, 103 }, CanGigantamax = true }, // Toxtricity
            new(40,05,3,SH) { Species = 841, Ability = A4, Moves = new[]{ 406, 788, 491, 184 }, CanGigantamax = true }, // Flapple
            new(50,08,4,SH) { Species = 849, Ability = A4, Moves = new[]{ 786, 506, 474, 586 }, CanGigantamax = true }, // Toxtricity
            new(50,08,4,SH) { Species = 841, Ability = A4, Moves = new[]{ 406, 788, 491, 263 }, CanGigantamax = true }, // Flapple
            new(60,10,5,SH) { Species = 849, Ability = A4, Moves = new[]{ 786, 506, 474, 586 }, CanGigantamax = true }, // Toxtricity
            new(60,10,5,SH) { Species = 841, Ability = A4, Moves = new[]{ 406, 788, 491, 263 }, CanGigantamax = true }, // Flapple
        };
    }
}
