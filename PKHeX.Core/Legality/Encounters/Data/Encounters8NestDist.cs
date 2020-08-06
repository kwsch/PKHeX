namespace PKHeX.Core
{
    // Distribution Nest Encounters (BCAT)
    internal static partial class Encounters8Nest
    {
        // For distribution encounters, all commented out entries are duplicate with a prior distribution encounter. Only one encounter is necessary for matching purposes.

        internal static readonly EncounterStatic8ND[] Dist_Common =
        {
            // 11/15 - Butterfree
            new EncounterStatic8ND(40,05,3) { Species = 012, Ability = A4, Moves = new[]{ 676, 403, 202, 527 } }, // Butterfree
            new EncounterStatic8ND(40,05,3) { Species = 012, Ability = A4, Moves = new[]{ 676, 403, 202, 527 }, CanGigantamax = true }, // Butterfree
            new EncounterStatic8ND(50,08,4) { Species = 012, Ability = A4, Moves = new[]{ 405, 403, 527, 078 } }, // Butterfree
            new EncounterStatic8ND(50,08,4) { Species = 012, Ability = A4, Moves = new[]{ 405, 403, 527, 078 }, CanGigantamax = true }, // Butterfree

            // 12/03 - Snorlax
            new EncounterStatic8ND(17,01,1) { Species = 446, Ability = A4, Moves = new[]{ 033, 044, 122, 111 } }, // Munchlax
            new EncounterStatic8ND(17,01,1) { Species = 446, Ability = A2, Moves = new[]{ 033, 044, 122, 111 } }, // Munchlax
            new EncounterStatic8ND(30,03,2) { Species = 143, Ability = A4, Moves = new[]{ 034, 242, 118, 111 } }, // Snorlax
            new EncounterStatic8ND(30,03,2) { Species = 143, Ability = A2, Moves = new[]{ 034, 242, 118, 111 } }, // Snorlax
            //new EncounterStatic8ND(40,05,3) { Species = 012, Ability = A4, Moves = new[]{ 676, 403, 202, 527 }, CanGigantamax = true }, // Butterfree
            new EncounterStatic8ND(40,05,3) { Species = 143, Ability = A4, Moves = new[]{ 034, 667, 242, 281 } }, // Snorlax
            new EncounterStatic8ND(40,05,3) { Species = 143, Ability = A4, Moves = new[]{ 034, 667, 242, 281 }, CanGigantamax = true }, // Snorlax
            //new EncounterStatic8ND(50,08,4) { Species = 012, Ability = A4, Moves = new[]{ 405, 403, 527, 078 }, CanGigantamax = true }, // Butterfree
            new EncounterStatic8ND(50,08,4) { Species = 143, Ability = A4, Moves = new[]{ 034, 667, 280, 523 } }, // Snorlax
            new EncounterStatic8ND(50,08,4) { Species = 143, Ability = A4, Moves = new[]{ 034, 667, 280, 523 }, CanGigantamax = true }, // Snorlax
            new EncounterStatic8ND(60,10,5) { Species = 143, Ability = A4, Moves = new[]{ 034, 442, 242, 428 } }, // Snorlax
            new EncounterStatic8ND(70,10,5) { Species = 143, Ability = A4, Moves = new[]{ 034, 442, 242, 428 }, CanGigantamax = true }, // Snorlax

            // 12/20 - Delibird
            //new EncounterStatic8ND(17,01,1) { Species = 446, Ability = A4, Moves = new[]{ 033, 044, 122, 111 } }, // Munchlax
            new EncounterStatic8ND(17,01,1) { Species = 225, Ability = A4, Moves = new[]{ 217, 229, 098, 420 } }, // Delibird
            //new EncounterStatic8ND(30,03,2) { Species = 143, Ability = A4, Moves = new[]{ 034, 242, 118, 111 } }, // Snorlax
            new EncounterStatic8ND(30,03,2) { Species = 225, Ability = A4, Moves = new[]{ 217, 065, 034, 693 } }, // Delibird
            //new EncounterStatic8ND(40,05,3) { Species = 012, Ability = A4, Moves = new[]{ 676, 403, 202, 527 }, CanGigantamax = true }, // Butterfree
            new EncounterStatic8ND(40,05,3) { Species = 225, Ability = A4, Moves = new[]{ 217, 065, 280, 196 } }, // Delibird
            //new EncounterStatic8ND(40,05,3) { Species = 143, Ability = A4, Moves = new[]{ 034, 667, 242, 281 }, CanGigantamax = true }, // Snorlax
            //new EncounterStatic8ND(50,08,4) { Species = 012, Ability = A4, Moves = new[]{ 405, 403, 527, 078 }, CanGigantamax = true }, // Butterfree
            new EncounterStatic8ND(50,08,4) { Species = 225, Ability = A4, Moves = new[]{ 217, 059, 034, 280 } }, // Delibird
            //new EncounterStatic8ND(50,08,4) { Species = 143, Ability = A4, Moves = new[]{ 034, 667, 280, 523 }, CanGigantamax = true }, // Snorlax
            new EncounterStatic8ND(70,10,5) { Species = 225, Ability = A4, Moves = new[]{ 217, 059, 065, 280 } }, // Delibird
            //new EncounterStatic8ND(70,10,5) { Species = 143, Ability = A4, Moves = new[]{ 034, 442, 242, 428 }, CanGigantamax = true }, // Snorlax

            // 12/30 - Magikarp
            new EncounterStatic8ND(17,01,1) { Species = 129, Ability = A4, Moves = new[]{ 150, 033, 000, 000 } }, // Magikarp
            new EncounterStatic8ND(17,01,1) { Species = 446, Ability = A4, Moves = new[]{ 033, 044, 122, 111 } }, // Munchlax
            new EncounterStatic8ND(30,03,2) { Species = 129, Ability = A4, Moves = new[]{ 150, 033, 175, 000 } }, // Magikarp
            //new EncounterStatic8ND(30,03,2) { Species = 143, Ability = A4, Moves = new[]{ 034, 242, 118, 111 } }, // Snorlax
            //new EncounterStatic8ND(40,05,3) { Species = 012, Ability = A4, Moves = new[]{ 676, 403, 202, 527 }, CanGigantamax = true }, // Butterfree
            new EncounterStatic8ND(40,05,3) { Species = 129, Ability = A4, Moves = new[]{ 150, 033, 175, 000 } }, // Magikarp
            //new EncounterStatic8ND(40,05,3) { Species = 143, Ability = A4, Moves = new[]{ 034, 667, 242, 281 }, CanGigantamax = true }, // Snorlax
            //new EncounterStatic8ND(50,08,4) { Species = 012, Ability = A4, Moves = new[]{ 405, 403, 527, 078 }, CanGigantamax = true }, // Butterfree
            new EncounterStatic8ND(50,08,4) { Species = 129, Ability = A4, Moves = new[]{ 150, 033, 175, 000 } }, // Magikarp
            //new EncounterStatic8ND(50,08,4) { Species = 143, Ability = A4, Moves = new[]{ 034, 667, 280, 523 }, CanGigantamax = true }, // Snorlax
            new EncounterStatic8ND(60,10,5) { Species = 129, Ability = A4, Moves = new[]{ 150, 033, 175, 340 } }, // Magikarp
            new EncounterStatic8ND(70,10,5) { Species = 129, Ability = A4, Moves = new[]{ 150, 033, 175, 340 } }, // Magikarp
            //new EncounterStatic8ND(70,10,5) { Species = 143, Ability = A4, Moves = new[]{ 034, 442, 242, 428 }, CanGigantamax = true }, // Snorlax

            // 1/8 - Applin
            new EncounterStatic8ND(17,01,1) { Species = 840, Ability = A4, Moves = new[]{ 110, 310, 389, 213 } }, // Applin
            new EncounterStatic8ND(17,01,1) { Species = 840, Ability = A2, Moves = new[]{ 110, 310, 389, 213 } }, // Applin
            new EncounterStatic8ND(17,01,1) { Species = 868, Ability = A4, Moves = new[]{ 577, 033, 186, 263 } }, // Milcery
            new EncounterStatic8ND(17,01,1) { Species = 868, Ability = A2, Moves = new[]{ 577, 033, 186, 263 } }, // Milcery
            new EncounterStatic8ND(30,03,2) { Species = 869, Ability = A4, Moves = new[]{ 577, 213, 033, 186 }, CanGigantamax = true }, // Alcremie

            // 1/31 - Milcery
            new EncounterStatic8ND(17,01,1) { Species = 868, Ability = A4, Moves = new[]{ 033, 186, 577, 496 }, CanGigantamax = true }, // Milcery
            new EncounterStatic8ND(30,03,2) { Species = 868, Ability = A4, Moves = new[]{ 577, 186, 263, 500 }, CanGigantamax = true }, // Milcery
            new EncounterStatic8ND(40,05,3) { Species = 868, Ability = A4, Moves = new[]{ 577, 605, 496, 213 }, CanGigantamax = true }, // Milcery
            new EncounterStatic8ND(50,08,4) { Species = 868, Ability = A4, Moves = new[]{ 577, 605, 496, 500 }, CanGigantamax = true }, // Milcery
            new EncounterStatic8ND(60,10,5) { Species = 868, Ability = A4, Moves = new[]{ 577, 605, 496, 500 }, CanGigantamax = true }, // Milcery

            // 2/6 - Toxtricity - Same as Milcery 1/31 (above)

            // 2/17 - Toxel
            new EncounterStatic8ND(17,01,1) { Species = 848, Ability = A4, Moves = new[]{ 609, 051, 496, 715 } }, // Toxel

            // 2/26 - Mewtwo
            new EncounterStatic8ND(17,01,1) { Species = 001, Ability = A4, Moves = new[]{ 033, 045, 022, 074 } }, // Bulbasaur
            new EncounterStatic8ND(17,01,1) { Species = 004, Ability = A4, Moves = new[]{ 010, 045, 052, 108 } }, // Charmander
            new EncounterStatic8ND(17,01,1) { Species = 007, Ability = A4, Moves = new[]{ 033, 039, 055, 110 } }, // Squirtle
            //new EncounterStatic8ND(17,01,1) { Species = 848, Ability = A4, Moves = new[]{ 609, 051, 496, 715 } }, // Toxel
            new EncounterStatic8ND(30,03,2) { Species = 001, Ability = A4, Moves = new[]{ 033, 022, 073, 075 } }, // Bulbasaur
            new EncounterStatic8ND(30,03,2) { Species = 004, Ability = A4, Moves = new[]{ 010, 052, 225, 424 } }, // Charmander
            new EncounterStatic8ND(30,03,2) { Species = 007, Ability = A4, Moves = new[]{ 033, 055, 044, 352 } }, // Squirtle
            new EncounterStatic8ND(40,05,3) { Species = 001, Ability = A4, Moves = new[]{ 073, 075, 077, 402 } }, // Bulbasaur
            new EncounterStatic8ND(40,05,3) { Species = 004, Ability = A4, Moves = new[]{ 424, 225, 163, 108 } }, // Charmander
            new EncounterStatic8ND(40,05,3) { Species = 007, Ability = A4, Moves = new[]{ 055, 229, 044, 352 } }, // Squirtle
            new EncounterStatic8ND(50,08,4) { Species = 002, Ability = A4, Moves = new[]{ 188, 412, 075, 034 } }, // Ivysaur
            new EncounterStatic8ND(50,08,4) { Species = 005, Ability = A4, Moves = new[]{ 257, 242, 009, 053 } }, // Charmeleon
            new EncounterStatic8ND(50,08,4) { Species = 008, Ability = A4, Moves = new[]{ 330, 396, 503, 428 } }, // Wartortle
            // sadly, can't capture Mewtwo.
            // new EncounterStatic8ND(100,10,6) { Species = 150, Ability = 1, Moves = new[]{ 540, 053, 396, 059 }, Nature = Nature.Timid, Shiny = Shiny.Never }, // Mewtwo
            // new EncounterStatic8ND(100,10,6) { Species = 150, Ability = 1, Moves = new[]{ 428, 007, 089, 280 }, Nature = Nature.Jolly, Shiny = Shiny.Never }, // Mewtwo
            // new EncounterStatic8ND(100,10,6) { Species = 150, Ability = 1, Moves = new[]{ 540, 126, 411, 059 }, Nature = Nature.Timid, Shiny = Shiny.Never }, // Mewtwo

            // 3/8 - Gengar/Machamp
            //new EncounterStatic8ND(17,01,1) { Species = 446, Ability = A4, Moves = new[]{ 033, 044, 122, 111 } }, // Munchlax
            new EncounterStatic8ND(17,01,1) { Species = 092, Ability = A4, Moves = new[]{ 122, 109, 095, 371 } }, // Gastly
            new EncounterStatic8ND(17,01,1) { Species = 066, Ability = A4, Moves = new[]{ 067, 043, 116, 279 } }, // Machop
            //new EncounterStatic8ND(30,03,2) { Species = 143, Ability = A4, Moves = new[]{ 034, 242, 118, 111 } }, // Snorlax
            new EncounterStatic8ND(30,03,2) { Species = 093, Ability = A4, Moves = new[]{ 325, 095, 122, 101 } }, // Haunter
            new EncounterStatic8ND(30,03,2) { Species = 067, Ability = A4, Moves = new[]{ 067, 490, 282, 233 } }, // Machoke
            //new EncounterStatic8ND(40,05,3) { Species = 143, Ability = A4, Moves = new[]{ 034, 667, 242, 281 } }, // Snorlax
            //new EncounterStatic8ND(40,05,3) { Species = 143, Ability = A4, Moves = new[]{ 034, 667, 242, 281 }, CanGigantamax = true }, // Snorlax
            new EncounterStatic8ND(40,05,3) { Species = 094, Ability = A4, Moves = new[]{ 506, 188, 085, 261 } }, // Gengar
            new EncounterStatic8ND(40,05,3) { Species = 094, Ability = A4, Moves = new[]{ 506, 188, 085, 261 }, CanGigantamax = true }, // Gengar
            new EncounterStatic8ND(40,05,3) { Species = 068, Ability = A4, Moves = new[]{ 279, 667, 008, 157 } }, // Machamp
            new EncounterStatic8ND(40,05,3) { Species = 068, Ability = A4, Moves = new[]{ 279, 667, 008, 157 }, CanGigantamax = true }, // Machamp
            //new EncounterStatic8ND(50,08,4) { Species = 143, Ability = A4, Moves = new[]{ 034, 667, 280, 523 } }, // Snorlax
            //new EncounterStatic8ND(50,08,4) { Species = 143, Ability = A4, Moves = new[]{ 034, 667, 280, 523 }, CanGigantamax = true }, // Snorlax
            new EncounterStatic8ND(50,08,4) { Species = 094, Ability = A4, Moves = new[]{ 247, 482, 411, 605 } }, // Gengar
            new EncounterStatic8ND(50,08,4) { Species = 094, Ability = A4, Moves = new[]{ 247, 482, 411, 605 }, CanGigantamax = true }, // Gengar
            new EncounterStatic8ND(50,08,4) { Species = 068, Ability = A4, Moves = new[]{ 280, 444, 371, 523 } }, // Machamp
            new EncounterStatic8ND(50,08,4) { Species = 068, Ability = A4, Moves = new[]{ 280, 444, 371, 523 }, CanGigantamax = true }, // Machamp
            //new EncounterStatic8ND(60,10,5) { Species = 143, Ability = A4, Moves = new[]{ 034, 442, 242, 428 } }, // Snorlax
            new EncounterStatic8ND(60,10,5) { Species = 143, Ability = A4, Moves = new[]{ 034, 442, 242, 428 }, CanGigantamax = true }, // Snorlax

            // 3/18 Exclusives & Food
            new EncounterStatic8ND(17,01,1) { Species = 588, Ability = A4, Moves = new[]{ 064, 043, 210, 491 } }, // Karrablast
            new EncounterStatic8ND(17,01,1) { Species = 616, Ability = A4, Moves = new[]{ 071, 051, 174, 522 } }, // Shelmet
            //new EncounterStatic8ND(17,01,1) { Species = 092, Ability = A4, Moves = new[]{ 122, 109, 095, 371 } }, // Gastly
            new EncounterStatic8ND(17,01,1) { Species = 871, Ability = A4, Moves = new[]{ 084, 064, 055, 031 } }, // Pincurchin
            //new EncounterStatic8ND(17,01,1) { Species = 066, Ability = A4, Moves = new[]{ 067, 043, 116, 279 } }, // Machop
            //new EncounterStatic8ND(30,03,2) { Species = 143, Ability = A4, Moves = new[]{ 034, 242, 118, 111 } }, // Snorlax
            //new EncounterStatic8ND(30,03,2) { Species = 093, Ability = A4, Moves = new[]{ 325, 095, 122, 101 } }, // Haunter
            new EncounterStatic8ND(30,03,2) { Species = 871, Ability = A4, Moves = new[]{ 209, 061, 086, 506 } }, // Pincurchin
            //new EncounterStatic8ND(30,03,2) { Species = 067, Ability = A4, Moves = new[]{ 067, 490, 282, 233 } }, // Machoke
            //new EncounterStatic8ND(40,05,3) { Species = 143, Ability = A4, Moves = new[]{ 034, 667, 242, 281 }, CanGigantamax = true }, // Snorlax
            //new EncounterStatic8ND(40,05,3) { Species = 094, Ability = A4, Moves = new[]{ 506, 188, 085, 261 }, CanGigantamax = true }, // Gengar
            new EncounterStatic8ND(40,05,3) { Species = 871, Ability = A2, Moves = new[]{ 085, 503, 398, 716 } }, // Pincurchin
            //new EncounterStatic8ND(40,05,3) { Species = 068, Ability = A4, Moves = new[]{ 279, 667, 008, 157 }, CanGigantamax = true }, // Machamp
            new EncounterStatic8ND(50,08,4) { Species = 617, Ability = A2, Moves = new[]{ 405, 522, 188, 202 } }, // Accelgor
            //new EncounterStatic8ND(50,08,4) { Species = 143, Ability = A4, Moves = new[]{ 034, 667, 280, 523 }, CanGigantamax = true }, // Snorlax
            new EncounterStatic8ND(50,08,4) { Species = 589, Ability = A2, Moves = new[]{ 442, 224, 529, 398 } }, // Escavalier
            //new EncounterStatic8ND(50,08,4) { Species = 094, Ability = A4, Moves = new[]{ 247, 482, 411, 605 }, CanGigantamax = true }, // Gengar
            new EncounterStatic8ND(50,08,4) { Species = 871, Ability = A2, Moves = new[]{ 435, 330, 474, 367 } }, // Pincurchin
            //new EncounterStatic8ND(50,08,4) { Species = 068, Ability = A4, Moves = new[]{ 280, 444, 371, 523 }, CanGigantamax = true }, // Machamp
            //new EncounterStatic8ND(60,10,5) { Species = 143, Ability = A4, Moves = new[]{ 034, 442, 242, 428 }, CanGigantamax = true }, // Snorlax

            // 3/25 Charizard
            new EncounterStatic8ND(17,01,1) { Species = 878, Ability = A4, Moves = new[]{ 091, 249, 205, 523 } }, // Cufant
            new EncounterStatic8ND(17,01,1) { Species = 568, Ability = A4, Moves = new[]{ 001, 499, 491, 133 } }, // Trubbish
            new EncounterStatic8ND(17,01,1) { Species = 004, Ability = A4, Moves = new[]{ 424, 052, 108, 225 } }, // Charmander
            new EncounterStatic8ND(17,01,1) { Species = 884, Ability = A4, Moves = new[]{ 468, 249, 043, 232 } }, // Duraludon
            new EncounterStatic8ND(30,03,2) { Species = 878, Ability = A4, Moves = new[]{ 334, 091, 205, 523 } }, // Cufant
            new EncounterStatic8ND(30,03,2) { Species = 568, Ability = A4, Moves = new[]{ 036, 499, 124, 133 } }, // Trubbish
            new EncounterStatic8ND(30,03,2) { Species = 005, Ability = A4, Moves = new[]{ 053, 163, 108, 225 } }, // Charmeleon
            new EncounterStatic8ND(30,03,2) { Species = 884, Ability = A4, Moves = new[]{ 468, 249, 784, 232 } }, // Duraludon
            new EncounterStatic8ND(40,05,3) { Species = 879, Ability = A4, Moves = new[]{ 334, 070, 442, 523 }, CanGigantamax = true }, // Copperajah
            new EncounterStatic8ND(40,05,3) { Species = 569, Ability = A4, Moves = new[]{ 188, 499, 034, 707 }, CanGigantamax = true }, // Garbodor
            new EncounterStatic8ND(40,05,3) { Species = 006, Ability = A4, Moves = new[]{ 053, 403, 108, 225 }, CanGigantamax = true }, // Charizard
            new EncounterStatic8ND(40,05,3) { Species = 884, Ability = A4, Moves = new[]{ 442, 555, 784, 334 }, CanGigantamax = true }, // Duraludon
            new EncounterStatic8ND(50,08,4) { Species = 879, Ability = A4, Moves = new[]{ 667, 442, 438, 523 }, CanGigantamax = true }, // Copperajah
            new EncounterStatic8ND(50,08,4) { Species = 569, Ability = A4, Moves = new[]{ 441, 499, 402, 707 }, CanGigantamax = true }, // Garbodor
            new EncounterStatic8ND(50,08,4) { Species = 006, Ability = A4, Moves = new[]{ 053, 403, 076, 257 }, CanGigantamax = true }, // Charizard
            new EncounterStatic8ND(50,08,4) { Species = 884, Ability = A4, Moves = new[]{ 337, 430, 784, 776 }, CanGigantamax = true }, // Duraludon

            // 4/09 - Easter Eggs
            new EncounterStatic8ND(40,05,2) { Species = 848, Ability = A4, Moves = new[]{ 609, 051, 175, 715 } }, // Toxel
            new EncounterStatic8ND(40,05,2) { Species = 458, Ability = A4, Moves = new[]{ 403, 061, 469, 503 } }, // Mantyke
            new EncounterStatic8ND(40,05,3) { Species = 406, Ability = A4, Moves = new[]{ 071, 074, 078, 188 } }, // Budew
            new EncounterStatic8ND(40,05,3) { Species = 236, Ability = A4, Moves = new[]{ 280, 157, 252, 116 } }, // Tyrogue
            new EncounterStatic8ND(40,05,3) { Species = 438, Ability = A4, Moves = new[]{ 317, 389, 157, 313 } }, // Bonsly
            new EncounterStatic8ND(40,05,3) { Species = 447, Ability = A4, Moves = new[]{ 014, 009, 232, 249 } }, // Riolu
            new EncounterStatic8ND(40,05,3) { Species = 446, Ability = A4, Moves = new[]{ 034, 089, 044, 122 } }, // Munchlax
            new EncounterStatic8ND(40,05,3) { Species = 439, Ability = A4, Moves = new[]{ 389, 060, 182, 085 } }, // Mime Jr.
            new EncounterStatic8ND(50,08,4) { Species = 175, Ability = A4, Moves = new[]{ 605, 219, 246, 053 } }, // Togepi
            new EncounterStatic8ND(50,08,4) { Species = 360, Ability = A4, Moves = new[]{ 068, 243, 204, 133 } }, // Wynaut
            new EncounterStatic8ND(50,08,4) { Species = 173, Ability = A4, Moves = new[]{ 574, 005, 113, 034 } }, // Cleffa
            new EncounterStatic8ND(50,08,4) { Species = 172, Ability = A4, Moves = new[]{ 583, 417, 085, 186 } }, // Pichu
            new EncounterStatic8ND(60,10,5) { Species = 132, Ability = A4, Moves = new[]{ 144, 000, 000, 000 } }, // Ditto

            // 4/27 - Meta
            new EncounterStatic8ND(17,01,1) { Species = 246, Ability = A4, Moves = new[]{ 157, 044, 184, 033 } }, // Larvitar
            new EncounterStatic8ND(17,01,1) { Species = 529, Ability = A4, Moves = new[]{ 189, 232, 010, 468 } }, // Drilbur
            new EncounterStatic8ND(17,01,1) { Species = 546, Ability = A4, Moves = new[]{ 584, 078, 075, 071 } }, // Cottonee
            new EncounterStatic8ND(17,01,1) { Species = 885, Ability = A4, Moves = new[]{ 611, 098, 310, 044 } }, // Dreepy
            new EncounterStatic8ND(17,01,1) { Species = 175, Ability = A4, Moves = new[]{ 204, 577, 113, 791 } }, // Togepi
            new EncounterStatic8ND(30,03,2) { Species = 247, Ability = A4, Moves = new[]{ 157, 242, 334, 707 } }, // Pupitar
            new EncounterStatic8ND(30,03,2) { Species = 529, Ability = A4, Moves = new[]{ 189, 232, 157, 306 } }, // Drilbur
            new EncounterStatic8ND(30,03,2) { Species = 546, Ability = A4, Moves = new[]{ 202, 075, 204, 077 } }, // Cottonee
            new EncounterStatic8ND(30,03,2) { Species = 886, Ability = A4, Moves = new[]{ 097, 506, 372, 458 } }, // Drakloak
            new EncounterStatic8ND(30,03,2) { Species = 176, Ability = A4, Moves = new[]{ 584, 038, 113, 791 } }, // Togetic
            new EncounterStatic8ND(40,05,3) { Species = 248, Ability = A4, Moves = new[]{ 444, 242, 334, 089 } }, // Tyranitar
            new EncounterStatic8ND(40,05,3) { Species = 530, Ability = A4, Moves = new[]{ 529, 232, 157, 032 } }, // Excadrill
            new EncounterStatic8ND(40,05,3) { Species = 547, Ability = A4, Moves = new[]{ 283, 585, 077, 366 } }, // Whimsicott
            new EncounterStatic8ND(40,05,3) { Species = 887, Ability = A4, Moves = new[]{ 751, 506, 349, 458 } }, // Dragapult
            new EncounterStatic8ND(40,05,3) { Species = 468, Ability = A4, Moves = new[]{ 605, 038, 246, 403 } }, // Togekiss
            new EncounterStatic8ND(50,08,4) { Species = 248, Ability = A4, Moves = new[]{ 444, 242, 442, 089 } }, // Tyranitar
            new EncounterStatic8ND(50,08,4) { Species = 530, Ability = A4, Moves = new[]{ 529, 442, 157, 032 } }, // Excadrill
            new EncounterStatic8ND(50,08,4) { Species = 547, Ability = A4, Moves = new[]{ 283, 585, 073, 366 } }, // Whimsicott
            new EncounterStatic8ND(50,08,4) { Species = 887, Ability = A4, Moves = new[]{ 751, 506, 349, 211 } }, // Dragapult
            new EncounterStatic8ND(50,08,4) { Species = 468, Ability = A4, Moves = new[]{ 605, 219, 246, 403 } }, // Togekiss
            new EncounterStatic8ND(60,10,5) { Species = 248, Ability = A4, Moves = new[]{ 444, 242, 442, 276 } }, // Tyranitar
            new EncounterStatic8ND(60,10,5) { Species = 530, Ability = A4, Moves = new[]{ 089, 442, 157, 032 } }, // Excadrill
            new EncounterStatic8ND(60,10,5) { Species = 547, Ability = A4, Moves = new[]{ 538, 585, 073, 366 } }, // Whimsicott
            new EncounterStatic8ND(60,10,5) { Species = 887, Ability = A4, Moves = new[]{ 751, 566, 349, 211 } }, // Dragapult
            new EncounterStatic8ND(60,10,5) { Species = 468, Ability = A4, Moves = new[]{ 605, 219, 246, 053 } }, // Togekiss

            // 5/11 - Gigantamax Pikachu
            new EncounterStatic8ND(17,01,1) { Species = 025, Ability = A4, Moves = new[]{ 084, 104, 486, 364 }, CanGigantamax = true }, // Pikachu
            new EncounterStatic8ND(30,03,2) { Species = 025, Ability = A4, Moves = new[]{ 021, 209, 097, 364 }, CanGigantamax = true }, // Pikachu
            new EncounterStatic8ND(40,05,3) { Species = 025, Ability = A4, Moves = new[]{ 021, 113, 085, 364 }, CanGigantamax = true }, // Pikachu
            new EncounterStatic8ND(50,08,4) { Species = 025, Ability = A4, Moves = new[]{ 087, 113, 085, 364 }, CanGigantamax = true }, // Pikachu
            new EncounterStatic8ND(50,08,4) { Species = 025, Ability = A4, Moves = new[]{ 087, 113, 057, 085 }, CanGigantamax = true }, // Pikachu
            new EncounterStatic8ND(60,10,5) { Species = 025, Ability = A4, Moves = new[]{ 087, 113, 057, 364 }, CanGigantamax = true }, // Pikachu
            new EncounterStatic8ND(60,10,5) { Species = 025, Ability = A4, Moves = new[]{ 087, 113, 057, 344 }, CanGigantamax = true }, // Pikachu

            // 5/18 - Gigantamax Eevee
            new EncounterStatic8ND(17,01,1) { Species = 133, Ability = A4, Moves = new[]{ 098, 270, 608, 028 }, CanGigantamax = true }, // Eevee
            new EncounterStatic8ND(30,03,2) { Species = 133, Ability = A4, Moves = new[]{ 129, 270, 608, 044 }, CanGigantamax = true }, // Eevee
            new EncounterStatic8ND(40,05,3) { Species = 133, Ability = A4, Moves = new[]{ 036, 270, 608, 044 }, CanGigantamax = true }, // Eevee
            new EncounterStatic8ND(40,05,3) { Species = 133, Ability = A4, Moves = new[]{ 036, 270, 608, 231 }, CanGigantamax = true }, // Eevee
            new EncounterStatic8ND(50,08,4) { Species = 133, Ability = A4, Moves = new[]{ 038, 270, 204, 044 }, CanGigantamax = true }, // Eevee
            new EncounterStatic8ND(50,08,4) { Species = 133, Ability = A4, Moves = new[]{ 038, 203, 204, 231 }, CanGigantamax = true }, // Eevee
            new EncounterStatic8ND(60,10,5) { Species = 133, Ability = A4, Moves = new[]{ 038, 270, 204, 231 }, CanGigantamax = true }, // Eevee
            new EncounterStatic8ND(60,10,5) { Species = 133, Ability = A4, Moves = new[]{ 387, 203, 204, 231 }, CanGigantamax = true }, // Eevee

            // 5/25 - Gigantamax Meowth
            new EncounterStatic8ND(17,01,1) { Species = 052, Ability = A4, Moves = new[]{ 252, 044, 010, 364 }, CanGigantamax = true }, // Meowth
            new EncounterStatic8ND(17,01,1) { Species = 052, Ability = A4, Moves = new[]{ 006, 044, 010, 364 }, CanGigantamax = true }, // Meowth
            new EncounterStatic8ND(30,03,2) { Species = 052, Ability = A4, Moves = new[]{ 252, 044, 269, 154 }, CanGigantamax = true }, // Meowth
            new EncounterStatic8ND(30,03,2) { Species = 052, Ability = A4, Moves = new[]{ 006, 044, 269, 154 }, CanGigantamax = true }, // Meowth
            new EncounterStatic8ND(40,05,3) { Species = 052, Ability = A4, Moves = new[]{ 252, 044, 417, 163 }, CanGigantamax = true }, // Meowth
            new EncounterStatic8ND(40,05,3) { Species = 052, Ability = A4, Moves = new[]{ 006, 044, 417, 163 }, CanGigantamax = true }, // Meowth
            new EncounterStatic8ND(50,08,4) { Species = 052, Ability = A4, Moves = new[]{ 252, 583, 417, 163 }, CanGigantamax = true }, // Meowth
            new EncounterStatic8ND(50,08,4) { Species = 052, Ability = A4, Moves = new[]{ 006, 583, 417, 163 }, CanGigantamax = true }, // Meowth
            new EncounterStatic8ND(60,10,5) { Species = 052, Ability = A4, Moves = new[]{ 252, 583, 417, 034 }, CanGigantamax = true }, // Meowth
            new EncounterStatic8ND(60,10,5) { Species = 052, Ability = A4, Moves = new[]{ 006, 583, 417, 034 }, CanGigantamax = true }, // Meowth

            // 6/2 - Gigantamax
            new EncounterStatic8ND(30,03,2) { Species = 143, Ability = A4, Moves = new[]{ 034, 044, 280, 523 }, CanGigantamax = true }, // Snorlax
          //new EncounterStatic8ND(60,10,5) { Species = 143, Ability = A4, Moves = new[]{ 034, 442, 242, 428 }, CanGigantamax = true }, // Snorlax

            new EncounterStatic8ND(17,01,1) { Species = 878, Ability = A4, Moves = new[]{ 523, 205, 045, 249 } }, // Cufant
            new EncounterStatic8ND(17,01,1) { Species = 208, Ability = A4, Moves = new[]{ 242, 442, 106, 422 } }, // Steelix
            new EncounterStatic8ND(17,01,1) { Species = 052, Ability = A4, Moves = new[]{ 232, 006, 242, 045 }, Form = 2 }, // Meowth-2
            new EncounterStatic8ND(17,01,1) { Species = 837, Ability = A4, Moves = new[]{ 229, 261, 479, 108 } }, // Rolycoly
            new EncounterStatic8ND(17,01,1) { Species = 111, Ability = A4, Moves = new[]{ 479, 523, 196, 182 } }, // Rhyhorn
            new EncounterStatic8ND(17,01,1) { Species = 095, Ability = A4, Moves = new[]{ 174, 225, 034, 106 } }, // Onix
            new EncounterStatic8ND(30,03,2) { Species = 878, Ability = A4, Moves = new[]{ 523, 023, 334, 249 } }, // Cufant
            new EncounterStatic8ND(30,03,2) { Species = 208, Ability = A4, Moves = new[]{ 157, 442, 328, 422 } }, // Steelix
            new EncounterStatic8ND(30,03,2) { Species = 863, Ability = A4, Moves = new[]{ 442, 006, 242, 269 } }, // Perrserker
            new EncounterStatic8ND(30,03,2) { Species = 838, Ability = A4, Moves = new[]{ 229, 488, 157, 108 } }, // Carkol
            new EncounterStatic8ND(30,03,2) { Species = 111, Ability = A4, Moves = new[]{ 350, 523, 196, 182 } }, // Rhyhorn
            new EncounterStatic8ND(30,03,2) { Species = 095, Ability = A4, Moves = new[]{ 776, 225, 034, 106 } }, // Onix
            new EncounterStatic8ND(40,05,3) { Species = 879, Ability = A4, Moves = new[]{ 070, 523, 334, 442 }, CanGigantamax = true }, // Copperajah
            new EncounterStatic8ND(40,05,3) { Species = 208, Ability = A4, Moves = new[]{ 157, 442, 328, 422 } }, // Steelix
            new EncounterStatic8ND(40,05,3) { Species = 863, Ability = A4, Moves = new[]{ 442, 006, 154, 269 } }, // Perrserker
            new EncounterStatic8ND(40,05,3) { Species = 839, Ability = A4, Moves = new[]{ 025, 488, 157, 108 }, CanGigantamax = true }, // Coalossal
            new EncounterStatic8ND(40,05,3) { Species = 112, Ability = A4, Moves = new[]{ 036, 529, 008, 182 } }, // Rhydon
            new EncounterStatic8ND(40,05,3) { Species = 095, Ability = A4, Moves = new[]{ 776, 225, 021, 201 } }, // Onix
            new EncounterStatic8ND(50,08,4) { Species = 879, Ability = A4, Moves = new[]{ 070, 523, 334, 442 }, CanGigantamax = true }, // Copperajah
            new EncounterStatic8ND(50,08,4) { Species = 208, Ability = A4, Moves = new[]{ 157, 231, 328, 422 } }, // Steelix
            new EncounterStatic8ND(50,08,4) { Species = 863, Ability = A4, Moves = new[]{ 442, 583, 154, 269 } }, // Perrserker
            new EncounterStatic8ND(50,08,4) { Species = 839, Ability = A4, Moves = new[]{ 025, 488, 157, 115 }, CanGigantamax = true }, // Coalossal
            new EncounterStatic8ND(50,08,4) { Species = 464, Ability = A4, Moves = new[]{ 350, 089, 008, 182 } }, // Rhyperior
            new EncounterStatic8ND(50,08,4) { Species = 095, Ability = A4, Moves = new[]{ 776, 225, 784, 201 } }, // Onix
            new EncounterStatic8ND(60,10,5) { Species = 879, Ability = A4, Moves = new[]{ 276, 089, 583, 442 }, CanGigantamax = true }, // Copperajah
            new EncounterStatic8ND(60,10,5) { Species = 208, Ability = A4, Moves = new[]{ 038, 231, 529, 422 } }, // Steelix
            new EncounterStatic8ND(60,10,5) { Species = 863, Ability = A4, Moves = new[]{ 442, 583, 370, 269 } }, // Perrserker
            new EncounterStatic8ND(60,10,5) { Species = 839, Ability = A4, Moves = new[]{ 076, 682, 157, 115 }, CanGigantamax = true }, // Coalossal
            new EncounterStatic8ND(60,10,5) { Species = 464, Ability = A4, Moves = new[]{ 444, 089, 008, 224 } }, // Rhyperior
            new EncounterStatic8ND(60,10,5) { Species = 095, Ability = A4, Moves = new[]{ 776, 444, 784, 201 } }, // Onix

            // 7/16 Sea Pokémon Invasion
            new EncounterStatic8ND(17,01,1) { Species = 320, Ability = A4, Moves = new[]{ 362, 034, 310, 054 } }, // Wailmer
            new EncounterStatic8ND(17,01,1) { Species = 098, Ability = A4, Moves = new[]{ 055, 341, 043, 232 } }, // Krabby
            new EncounterStatic8ND(17,01,1) { Species = 771, Ability = A4, Moves = new[]{ 240, 219, 213, 269 } }, // Pyukumuku
            new EncounterStatic8ND(17,01,1) { Species = 592, Ability = A4, Moves = new[]{ 352, 101, 071, 240 } }, // Frillish
            new EncounterStatic8ND(17,01,1) { Species = 458, Ability = A4, Moves = new[]{ 033, 017, 352, 469 } }, // Mantyke
            new EncounterStatic8ND(17,01,1) { Species = 318, Ability = A4, Moves = new[]{ 453, 305, 116, 044 } }, // Carvanha
            new EncounterStatic8ND(30,03,2) { Species = 320, Ability = A4, Moves = new[]{ 362, 034, 310, 054 } }, // Wailmer
            new EncounterStatic8ND(30,03,2) { Species = 098, Ability = A4, Moves = new[]{ 061, 341, 023, 232 } }, // Krabby
            new EncounterStatic8ND(30,03,2) { Species = 771, Ability = A4, Moves = new[]{ 240, 219, 213, 174 } }, // Pyukumuku
            new EncounterStatic8ND(30,03,2) { Species = 592, Ability = A4, Moves = new[]{ 362, 101, 071, 240 } }, // Frillish
            new EncounterStatic8ND(30,03,2) { Species = 458, Ability = A4, Moves = new[]{ 029, 017, 061, 469 } }, // Mantyke
            new EncounterStatic8ND(30,03,2) { Species = 319, Ability = A4, Moves = new[]{ 453, 400, 423, 044 } }, // Sharpedo
            new EncounterStatic8ND(40,05,3) { Species = 321, Ability = A4, Moves = new[]{ 362, 034, 340, 568 } }, // Wailord
            new EncounterStatic8ND(40,05,3) { Species = 099, Ability = A4, Moves = new[]{ 534, 341, 021, 014 } }, // Kingler
            new EncounterStatic8ND(40,05,3) { Species = 771, Ability = A4, Moves = new[]{ 240, 219, 213, 174 } }, // Pyukumuku
            new EncounterStatic8ND(40,05,3) { Species = 593, Ability = A4, Moves = new[]{ 362, 247, 071, 151 } }, // Jellicent
            new EncounterStatic8ND(40,05,3) { Species = 226, Ability = A4, Moves = new[]{ 029, 403, 060, 331 } }, // Mantine
            new EncounterStatic8ND(40,05,3) { Species = 319, Ability = A4, Moves = new[]{ 453, 242, 423, 044 } }, // Sharpedo
            new EncounterStatic8ND(50,08,4) { Species = 321, Ability = A4, Moves = new[]{ 056, 034, 340, 133 } }, // Wailord
            new EncounterStatic8ND(50,08,4) { Species = 099, Ability = A4, Moves = new[]{ 534, 341, 359, 014 }, CanGigantamax = true }, // Kingler
            new EncounterStatic8ND(50,08,4) { Species = 771, Ability = A4, Moves = new[]{ 240, 219, 213, 174 } }, // Pyukumuku
            new EncounterStatic8ND(50,08,4) { Species = 593, Ability = A4, Moves = new[]{ 056, 247, 071, 151 } }, // Jellicent
            new EncounterStatic8ND(50,08,4) { Species = 226, Ability = A4, Moves = new[]{ 036, 403, 060, 331 } }, // Mantine
            new EncounterStatic8ND(50,08,4) { Species = 319, Ability = A4, Moves = new[]{ 057, 242, 423, 044 } }, // Sharpedo
            new EncounterStatic8ND(60,10,5) { Species = 321, Ability = A4, Moves = new[]{ 503, 034, 340, 133 }, Shiny = Shiny.Always }, // Wailord
            new EncounterStatic8ND(60,10,5) { Species = 321, Ability = A4, Moves = new[]{ 056, 034, 340, 133 } }, // Wailord
            new EncounterStatic8ND(60,10,5) { Species = 771, Ability = A4, Moves = new[]{ 092, 599, 213, 174 } }, // Pyukumuku
            new EncounterStatic8ND(60,10,5) { Species = 593, Ability = A4, Moves = new[]{ 056, 058, 605, 433 } }, // Jellicent
            new EncounterStatic8ND(60,10,5) { Species = 226, Ability = A4, Moves = new[]{ 036, 403, 060, 331 } }, // Mantine
            new EncounterStatic8ND(60,10,5) { Species = 319, Ability = A4, Moves = new[]{ 057, 242, 423, 305 } }, // Sharpedo

            // 7/31 - Water
            new EncounterStatic8ND(17,01,1) { Species = 833, Ability = A4, Moves = new[]{ 055, 033, 044, 240 } }, // Chewtle
            new EncounterStatic8ND(17,01,1) { Species = 349, Ability = A4, Moves = new[]{ 150, 033, 175, 057 } }, // Feebas
            new EncounterStatic8ND(17,01,1) { Species = 194, Ability = A4, Moves = new[]{ 341, 021, 039, 055 } }, // Wooper
            new EncounterStatic8ND(17,01,1) { Species = 843, Ability = A4, Moves = new[]{ 028, 035, 523, 693 } }, // Silicobra
            new EncounterStatic8ND(17,01,1) { Species = 449, Ability = A4, Moves = new[]{ 341, 328, 044, 033 } }, // Hippopotas
            new EncounterStatic8ND(17,01,1) { Species = 422, Ability = A4, Moves = new[]{ 352, 106, 189, 055 }, Form = 1 }, // Shellos-1
            new EncounterStatic8ND(30,03,2) { Species = 834, Ability = A4, Moves = new[]{ 157, 534, 317, 055 }, CanGigantamax = true }, // Drednaw
            new EncounterStatic8ND(30,03,2) { Species = 349, Ability = A4, Moves = new[]{ 057, 033, 175, 150 } }, // Feebas
            new EncounterStatic8ND(30,03,2) { Species = 195, Ability = A4, Moves = new[]{ 341, 021, 401, 055 } }, // Quagsire
            new EncounterStatic8ND(30,03,2) { Species = 843, Ability = A4, Moves = new[]{ 091, 029, 523, 693 } }, // Silicobra
            new EncounterStatic8ND(30,03,2) { Species = 449, Ability = A4, Moves = new[]{ 341, 036, 044, 242 } }, // Hippopotas
            new EncounterStatic8ND(30,03,2) { Species = 423, Ability = A4, Moves = new[]{ 189, 352, 246, 106 }, Form = 1 }, // Gastrodon-1
            new EncounterStatic8ND(40,05,3) { Species = 834, Ability = A4, Moves = new[]{ 157, 534, 317, 055 }, CanGigantamax = true }, // Drednaw
            new EncounterStatic8ND(40,05,3) { Species = 350, Ability = A4, Moves = new[]{ 057, 239, 034, 574 } }, // Milotic
            new EncounterStatic8ND(40,05,3) { Species = 195, Ability = A4, Moves = new[]{ 341, 021, 401, 005 } }, // Quagsire
            new EncounterStatic8ND(40,05,3) { Species = 844, Ability = A4, Moves = new[]{ 693, 523, 201, 091 }, CanGigantamax = true }, // Sandaconda
            new EncounterStatic8ND(40,05,3) { Species = 450, Ability = A4, Moves = new[]{ 341, 422, 036, 242 } }, // Hippowdon
            new EncounterStatic8ND(40,05,3) { Species = 423, Ability = A4, Moves = new[]{ 414, 352, 246, 106 }, Form = 1 }, // Gastrodon-1
            new EncounterStatic8ND(50,08,4) { Species = 834, Ability = A4, Moves = new[]{ 157, 710, 317, 334 }, CanGigantamax = true }, // Drednaw
            new EncounterStatic8ND(50,08,4) { Species = 350, Ability = A4, Moves = new[]{ 057, 231, 034, 574 } }, // Milotic
            new EncounterStatic8ND(50,08,4) { Species = 195, Ability = A4, Moves = new[]{ 341, 280, 401, 005 } }, // Quagsire
            new EncounterStatic8ND(50,08,4) { Species = 844, Ability = A4, Moves = new[]{ 693, 529, 201, 091 }, CanGigantamax = true }, // Sandaconda
            new EncounterStatic8ND(50,08,4) { Species = 450, Ability = A4, Moves = new[]{ 089, 422, 036, 242 } }, // Hippowdon
            new EncounterStatic8ND(50,08,4) { Species = 423, Ability = A4, Moves = new[]{ 414, 503, 311, 106 }, Form = 1 }, // Gastrodon-1
            new EncounterStatic8ND(60,10,5) { Species = 834, Ability = A4, Moves = new[]{ 157, 710, 317, 334 }, CanGigantamax = true }, // Drednaw
            new EncounterStatic8ND(60,10,5) { Species = 350, Ability = A4, Moves = new[]{ 503, 231, 034, 574 } }, // Milotic
            new EncounterStatic8ND(60,10,5) { Species = 195, Ability = A4, Moves = new[]{ 089, 280, 401, 005 } }, // Quagsire
            new EncounterStatic8ND(60,10,5) { Species = 844, Ability = A4, Moves = new[]{ 693, 529, 201, 091 }, CanGigantamax = true }, // Sandaconda
            new EncounterStatic8ND(60,10,5) { Species = 450, Ability = A4, Moves = new[]{ 089, 422, 231, 242 } }, // Hippowdon
            new EncounterStatic8ND(60,10,5) { Species = 423, Ability = A4, Moves = new[]{ 414, 503, 311, 352 }, Form = 1 }, // Gastrodon-1

            // 8/5 - Pikachu Mass Appearance
            new EncounterStatic8ND(17,01,1) { Species = 025, Ability = A4, Moves = new[]{ 084, 098, 204, 086 } }, // Pikachu
            new EncounterStatic8ND(17,01,1) { Species = 026, Ability = A4, Moves = new[]{ 009, 129, 280, 204 } }, // Raichu
            new EncounterStatic8ND(17,01,1) { Species = 026, Ability = A4, Moves = new[]{ 009, 129, 280, 204 }, Form = 1 }, // Raichu-1
            new EncounterStatic8ND(17,01,1) { Species = 172, Ability = A4, Moves = new[]{ 589, 609, 085, 186 } }, // Pichu
            new EncounterStatic8ND(17,01,1) { Species = 778, Ability = A4, Moves = new[]{ 086, 452, 425, 010 } }, // Mimikyu
            new EncounterStatic8ND(30,03,2) { Species = 025, Ability = A4, Moves = new[]{ 209, 097, 204, 086 } }, // Pikachu
            new EncounterStatic8ND(30,03,2) { Species = 026, Ability = A4, Moves = new[]{ 009, 129, 280, 204 } }, // Raichu
            new EncounterStatic8ND(30,03,2) { Species = 026, Ability = A4, Moves = new[]{ 009, 129, 280, 204 }, Form = 1 }, // Raichu-1
            new EncounterStatic8ND(30,03,2) { Species = 172, Ability = A4, Moves = new[]{ 204, 609, 085, 186 } }, // Pichu
            new EncounterStatic8ND(30,03,2) { Species = 778, Ability = A4, Moves = new[]{ 086, 452, 425, 608 } }, // Mimikyu
            new EncounterStatic8ND(40,05,3) { Species = 025, Ability = A4, Moves = new[]{ 085, 231, 583, 086 } }, // Pikachu
            new EncounterStatic8ND(40,05,3) { Species = 026, Ability = A4, Moves = new[]{ 085, 034, 411, 583 } }, // Raichu
            new EncounterStatic8ND(40,05,3) { Species = 026, Ability = A4, Moves = new[]{ 085, 034, 057, 583 }, Form = 1 }, // Raichu-1
            new EncounterStatic8ND(40,05,3) { Species = 172, Ability = A4, Moves = new[]{ 204, 609, 085, 583 } }, // Pichu
            new EncounterStatic8ND(40,05,3) { Species = 778, Ability = A4, Moves = new[]{ 085, 452, 421, 608 } }, // Mimikyu
            new EncounterStatic8ND(50,08,4) { Species = 025, Ability = A4, Moves = new[]{ 087, 231, 583, 086 } }, // Pikachu
            new EncounterStatic8ND(50,08,4) { Species = 026, Ability = A4, Moves = new[]{ 087, 034, 411, 583 } }, // Raichu
            new EncounterStatic8ND(50,08,4) { Species = 026, Ability = A4, Moves = new[]{ 087, 034, 057, 583 }, Form = 1 }, // Raichu-1
            new EncounterStatic8ND(50,08,4) { Species = 172, Ability = A4, Moves = new[]{ 253, 609, 085, 583 } }, // Pichu
            new EncounterStatic8ND(50,08,4) { Species = 778, Ability = A4, Moves = new[]{ 085, 452, 261, 204 } }, // Mimikyu
            new EncounterStatic8ND(60,10,5) { Species = 025, Ability = A4, Moves = new[]{ 344, 231, 583, 086 } }, // Pikachu
            new EncounterStatic8ND(60,10,5) { Species = 025, Ability = A4, Moves = new[]{ 344, 231, 583, 086 }, Shiny = Shiny.Always }, // Pikachu
            new EncounterStatic8ND(60,10,5) { Species = 026, Ability = A4, Moves = new[]{ 087, 034, 411, 583 } }, // Raichu
            new EncounterStatic8ND(60,10,5) { Species = 026, Ability = A4, Moves = new[]{ 087, 034, 057, 583 }, Form = 1 }, // Raichu-1
            new EncounterStatic8ND(60,10,5) { Species = 172, Ability = A4, Moves = new[]{ 253, 609, 085, 583 } }, // Pichu
            new EncounterStatic8ND(60,10,5) { Species = 778, Ability = A4, Moves = new[]{ 087, 452, 261, 583 } }, // Mimikyu
        };

        internal static readonly EncounterStatic8ND[] Dist_SW =
        {
            // 11/15 - Butterfree
            new EncounterStatic8ND(17,01,1) { Species = 012, Ability = A4, Moves = new[]{ 405, 060, 016, 078 } }, // Butterfree
            new EncounterStatic8ND(17,01,1) { Species = 012, Ability = A4, Moves = new[]{ 405, 060, 016, 078 }, CanGigantamax = true }, // Butterfree
            new EncounterStatic8ND(17,01,1) { Species = 843, Ability = A2, Moves = new[]{ 693, 523, 189, 103 } }, // Silicobra
            new EncounterStatic8ND(17,01,1) { Species = 843, Ability = A4, Moves = new[]{ 693, 523, 189, 103 } }, // Silicobra
            new EncounterStatic8ND(17,01,1) { Species = 833, Ability = A2, Moves = new[]{ 055, 044, 033, 213 } }, // Chewtle
            new EncounterStatic8ND(17,01,1) { Species = 833, Ability = A4, Moves = new[]{ 055, 044, 033, 213 } }, // Chewtle
            new EncounterStatic8ND(30,03,2) { Species = 012, Ability = A4, Moves = new[]{ 405, 403, 081, 079 } }, // Butterfree
            new EncounterStatic8ND(30,03,2) { Species = 012, Ability = A4, Moves = new[]{ 405, 403, 081, 079 }, CanGigantamax = true }, // Butterfree
            new EncounterStatic8ND(30,03,2) { Species = 843, Ability = A2, Moves = new[]{ 693, 523, 029, 137 } }, // Silicobra
            new EncounterStatic8ND(30,03,2) { Species = 843, Ability = A4, Moves = new[]{ 693, 523, 029, 137 } }, // Silicobra
            new EncounterStatic8ND(30,03,2) { Species = 834, Ability = A2, Moves = new[]{ 317, 242, 055, 334 } }, // Drednaw
            new EncounterStatic8ND(30,03,2) { Species = 834, Ability = A4, Moves = new[]{ 317, 242, 055, 334 } }, // Drednaw
            new EncounterStatic8ND(40,05,3) { Species = 844, Ability = A4, Moves = new[]{ 693, 529, 407, 424 } }, // Sandaconda
            new EncounterStatic8ND(40,05,3) { Species = 844, Ability = A4, Moves = new[]{ 693, 529, 407, 424 }, CanGigantamax = true }, // Sandaconda
            new EncounterStatic8ND(40,05,3) { Species = 834, Ability = A4, Moves = new[]{ 157, 534, 034, 317 } }, // Drednaw
            new EncounterStatic8ND(40,05,3) { Species = 834, Ability = A4, Moves = new[]{ 157, 534, 034, 317 }, CanGigantamax = true }, // Drednaw
            new EncounterStatic8ND(50,08,4) { Species = 844, Ability = A4, Moves = new[]{ 693, 529, 342, 328 } }, // Sandaconda
            new EncounterStatic8ND(50,08,4) { Species = 844, Ability = A4, Moves = new[]{ 693, 529, 342, 328 }, CanGigantamax = true }, // Sandaconda
            new EncounterStatic8ND(50,08,4) { Species = 834, Ability = A4, Moves = new[]{ 157, 710, 350, 523 } }, // Drednaw
            new EncounterStatic8ND(50,08,4) { Species = 834, Ability = A4, Moves = new[]{ 157, 710, 350, 523 }, CanGigantamax = true }, // Drednaw
            new EncounterStatic8ND(60,10,5) { Species = 012, Ability = A4, Moves = new[]{ 405, 094, 542, 202 } }, // Butterfree
            new EncounterStatic8ND(70,10,5) { Species = 012, Ability = A4, Moves = new[]{ 405, 094, 542, 202 }, CanGigantamax = true }, // Butterfree
            new EncounterStatic8ND(60,10,5) { Species = 844, Ability = A4, Moves = new[]{ 693, 529, 342, 328 } }, // Sandaconda
            new EncounterStatic8ND(70,10,5) { Species = 844, Ability = A4, Moves = new[]{ 693, 529, 342, 328 }, CanGigantamax = true }, // Sandaconda
            new EncounterStatic8ND(60,10,5) { Species = 834, Ability = A4, Moves = new[]{ 157, 710, 317, 334 } }, // Drednaw
            new EncounterStatic8ND(70,10,5) { Species = 834, Ability = A4, Moves = new[]{ 157, 710, 317, 334 }, CanGigantamax = true }, // Drednaw

            // 12/03 - Snorlax
            //new EncounterStatic8ND(17,01,1) { Species = 012, Ability = A4, Moves = new[]{ 405, 060, 016, 078 } }, // Butterfree
            //new EncounterStatic8ND(17,01,1) { Species = 012, Ability = A4, Moves = new[]{ 405, 060, 016, 078 }, CanGigantamax = true }, // Butterfree
            //new EncounterStatic8ND(17,01,1) { Species = 843, Ability = A4, Moves = new[]{ 693, 523, 189, 103 } }, // Silicobra
            //new EncounterStatic8ND(17,01,1) { Species = 833, Ability = A4, Moves = new[]{ 055, 044, 033, 213 } }, // Chewtle
            //new EncounterStatic8ND(30,03,2) { Species = 012, Ability = A4, Moves = new[]{ 405, 403, 081, 079 }, CanGigantamax = true }, // Butterfree
            //new EncounterStatic8ND(30,03,2) { Species = 843, Ability = A4, Moves = new[]{ 693, 523, 029, 137 } }, // Silicobra
            //new EncounterStatic8ND(30,03,2) { Species = 834, Ability = A4, Moves = new[]{ 317, 242, 055, 334 } }, // Drednaw
            //new EncounterStatic8ND(30,03,2) { Species = 834, Ability = A2, Moves = new[]{ 317, 242, 055, 334 } }, // Drednaw
            //new EncounterStatic8ND(40,05,3) { Species = 844, Ability = A4, Moves = new[]{ 693, 529, 407, 424 }, CanGigantamax = true }, // Sandaconda
            //new EncounterStatic8ND(40,05,3) { Species = 834, Ability = A4, Moves = new[]{ 157, 534, 034, 317 } }, // Drednaw
            //new EncounterStatic8ND(40,05,3) { Species = 834, Ability = A4, Moves = new[]{ 157, 534, 034, 317 }, CanGigantamax = true }, // Drednaw
            //new EncounterStatic8ND(50,08,4) { Species = 844, Ability = A4, Moves = new[]{ 693, 529, 342, 328 }, CanGigantamax = true }, // Sandaconda
            //new EncounterStatic8ND(50,08,4) { Species = 834, Ability = A4, Moves = new[]{ 157, 710, 350, 523 } }, // Drednaw
            //new EncounterStatic8ND(50,08,4) { Species = 834, Ability = A4, Moves = new[]{ 157, 710, 350, 523 }, CanGigantamax = true }, // Drednaw
            new EncounterStatic8ND(60,10,5) { Species = 012, Ability = A4, Moves = new[]{ 405, 094, 542, 202 }, CanGigantamax = true }, // Butterfree
            //new EncounterStatic8ND(70,10,5) { Species = 844, Ability = A4, Moves = new[]{ 693, 529, 342, 328 }, CanGigantamax = true }, // Sandaconda
            //new EncounterStatic8ND(60,10,5) { Species = 834, Ability = A4, Moves = new[]{ 157, 710, 317, 334 } }, // Drednaw
            //new EncounterStatic8ND(70,10,5) { Species = 834, Ability = A4, Moves = new[]{ 157, 710, 317, 334 }, CanGigantamax = true }, // Drednaw

            // 12/20 - Delibird
            //new EncounterStatic8ND(17,01,1) { Species = 012, Ability = A4, Moves = new[]{ 405, 060, 016, 078 } }, // Butterfree
            //new EncounterStatic8ND(17,01,1) { Species = 012, Ability = A4, Moves = new[]{ 405, 060, 016, 078 }, CanGigantamax = true }, // Butterfree
            //new EncounterStatic8ND(17,01,1) { Species = 843, Ability = A4, Moves = new[]{ 693, 523, 189, 103 } }, // Silicobra
            //new EncounterStatic8ND(17,01,1) { Species = 833, Ability = A4, Moves = new[]{ 055, 044, 033, 213 } }, // Chewtle
            //new EncounterStatic8ND(30,03,2) { Species = 012, Ability = A4, Moves = new[]{ 405, 403, 081, 079 }, CanGigantamax = true }, // Butterfree
            //new EncounterStatic8ND(30,03,2) { Species = 843, Ability = A4, Moves = new[]{ 693, 523, 029, 137 } }, // Silicobra
            //new EncounterStatic8ND(30,03,2) { Species = 834, Ability = A4, Moves = new[]{ 317, 242, 055, 334 } }, // Drednaw
            //new EncounterStatic8ND(40,05,3) { Species = 844, Ability = A4, Moves = new[]{ 693, 529, 407, 424 }, CanGigantamax = true }, // Sandaconda
            //new EncounterStatic8ND(40,05,3) { Species = 834, Ability = A4, Moves = new[]{ 157, 534, 034, 317 } }, // Drednaw
            //new EncounterStatic8ND(40,05,3) { Species = 834, Ability = A4, Moves = new[]{ 157, 534, 034, 317 }, CanGigantamax = true }, // Drednaw
            new EncounterStatic8ND(50,08,3) { Species = 844, Ability = A4, Moves = new[]{ 693, 529, 407, 424 }, CanGigantamax = true }, // Sandaconda
            //new EncounterStatic8ND(50,08,4) { Species = 834, Ability = A4, Moves = new[]{ 157, 710, 350, 523 } }, // Drednaw
            //new EncounterStatic8ND(50,08,4) { Species = 834, Ability = A4, Moves = new[]{ 157, 710, 350, 523 }, CanGigantamax = true }, // Drednaw
            //new EncounterStatic8ND(60,10,5) { Species = 012, Ability = A4, Moves = new[]{ 405, 094, 542, 202 }, CanGigantamax = true }, // Butterfree
            //new EncounterStatic8ND(70,10,5) { Species = 844, Ability = A4, Moves = new[]{ 693, 529, 342, 328 }, CanGigantamax = true }, // Sandaconda
            //new EncounterStatic8ND(60,10,5) { Species = 834, Ability = A4, Moves = new[]{ 157, 710, 317, 334 } }, // Drednaw
            //new EncounterStatic8ND(70,10,5) { Species = 834, Ability = A4, Moves = new[]{ 157, 710, 317, 334 }, CanGigantamax = true }, // Drednaw

            // 12/30 - Magikarp
            //new EncounterStatic8ND(17,01,1) { Species = 012, Ability = A4, Moves = new[]{ 405, 060, 016, 078 }, CanGigantamax = true }, // Butterfree
            //new EncounterStatic8ND(17,01,1) { Species = 843, Ability = A4, Moves = new[]{ 693, 523, 189, 103 } }, // Silicobra
            //new EncounterStatic8ND(17,01,1) { Species = 833, Ability = A4, Moves = new[]{ 055, 044, 033, 213 } }, // Chewtle
            //new EncounterStatic8ND(30,03,2) { Species = 012, Ability = A4, Moves = new[]{ 405, 403, 081, 079 }, CanGigantamax = true }, // Butterfree
            //new EncounterStatic8ND(30,03,2) { Species = 843, Ability = A4, Moves = new[]{ 693, 523, 029, 137 } }, // Silicobra
            //new EncounterStatic8ND(30,03,2) { Species = 834, Ability = A4, Moves = new[]{ 317, 242, 055, 334 } }, // Drednaw
            //new EncounterStatic8ND(40,05,3) { Species = 844, Ability = A4, Moves = new[]{ 693, 529, 407, 424 }, CanGigantamax = true }, // Sandaconda
            //new EncounterStatic8ND(40,05,3) { Species = 834, Ability = A4, Moves = new[]{ 157, 534, 034, 317 }, CanGigantamax = true }, // Drednaw
            //new EncounterStatic8ND(50,08,4) { Species = 844, Ability = A4, Moves = new[]{ 693, 529, 342, 328 }, CanGigantamax = true }, // Sandaconda
            //new EncounterStatic8ND(50,08,4) { Species = 834, Ability = A4, Moves = new[]{ 157, 710, 350, 523 }, CanGigantamax = true }, // Drednaw
            //new EncounterStatic8ND(60,10,5) { Species = 012, Ability = A4, Moves = new[]{ 405, 094, 542, 202 }, CanGigantamax = true }, // Butterfree
            //new EncounterStatic8ND(70,10,5) { Species = 844, Ability = A4, Moves = new[]{ 693, 529, 342, 328 }, CanGigantamax = true }, // Sandaconda
            //new EncounterStatic8ND(70,10,5) { Species = 834, Ability = A4, Moves = new[]{ 157, 710, 317, 334 }, CanGigantamax = true }, // Drednaw

            // 1/8 - Applin
            new EncounterStatic8ND(17,01,1) { Species = 837, Ability = A4, Moves = new[]{ 479, 033, 108, 189 } }, // Rolycoly
            new EncounterStatic8ND(17,01,1) { Species = 837, Ability = A2, Moves = new[]{ 479, 033, 108, 189 } }, // Rolycoly
            new EncounterStatic8ND(30,03,2) { Species = 841, Ability = A4, Moves = new[]{ 406, 491, 017, 225 } }, // Flapple
            new EncounterStatic8ND(30,03,2) { Species = 841, Ability = A2, Moves = new[]{ 406, 491, 017, 225 } }, // Flapple
            new EncounterStatic8ND(30,03,2) { Species = 838, Ability = A4, Moves = new[]{ 246, 510, 479, 189 } }, // Carkol
            new EncounterStatic8ND(30,03,2) { Species = 838, Ability = A2, Moves = new[]{ 246, 510, 479, 189 } }, // Carkol
            new EncounterStatic8ND(40,05,3) { Species = 841, Ability = A4, Moves = new[]{ 788, 406, 512, 491 } }, // Flapple
            new EncounterStatic8ND(40,05,3) { Species = 841, Ability = A4, Moves = new[]{ 788, 406, 512, 491 }, CanGigantamax = true }, // Flapple
            new EncounterStatic8ND(40,05,3) { Species = 869, Ability = A4, Moves = new[]{ 577, 605, 202, 186 }, Form = 3, CanGigantamax = true }, // Alcremie-3
            new EncounterStatic8ND(40,05,3) { Species = 869, Ability = A4, Moves = new[]{ 577, 605, 202, 186 }, Form = 4, CanGigantamax = true }, // Alcremie-4
            new EncounterStatic8ND(40,05,3) { Species = 839, Ability = A4, Moves = new[]{ 246, 510, 479, 488 } }, // Coalossal
            new EncounterStatic8ND(40,05,3) { Species = 839, Ability = A4, Moves = new[]{ 246, 510, 479, 488 }, CanGigantamax = true }, // Coalossal
            new EncounterStatic8ND(50,08,4) { Species = 841, Ability = A4, Moves = new[]{ 407, 788, 491, 334 } }, // Flapple
            new EncounterStatic8ND(50,08,4) { Species = 841, Ability = A4, Moves = new[]{ 407, 788, 491, 334 }, CanGigantamax = true }, // Flapple
            new EncounterStatic8ND(50,08,4) { Species = 869, Ability = A4, Moves = new[]{ 605, 202, 094, 151 }, Form = 1, CanGigantamax = true }, // Alcremie-1
            new EncounterStatic8ND(50,08,4) { Species = 869, Ability = A4, Moves = new[]{ 605, 202, 094, 151 }, Form = 2, CanGigantamax = true }, // Alcremie-2
            new EncounterStatic8ND(50,08,4) { Species = 839, Ability = A4, Moves = new[]{ 246, 053, 157, 261 } }, // Coalossal
            new EncounterStatic8ND(50,08,4) { Species = 839, Ability = A4, Moves = new[]{ 246, 053, 157, 261 }, CanGigantamax = true }, // Coalossal
            new EncounterStatic8ND(60,10,5) { Species = 841, Ability = A4, Moves = new[]{ 407, 788, 512, 349 } }, // Flapple
            new EncounterStatic8ND(60,10,5) { Species = 841, Ability = A4, Moves = new[]{ 407, 788, 512, 349 }, CanGigantamax = true }, // Flapple
            new EncounterStatic8ND(60,10,5) { Species = 869, Ability = A4, Moves = new[]{ 605, 202, 595, 500 }, Form = 5, CanGigantamax = true }, // Alcremie-5
            new EncounterStatic8ND(60,10,5) { Species = 869, Ability = A4, Moves = new[]{ 605, 202, 595, 500 }, Form = 6, CanGigantamax = true }, // Alcremie-6
            new EncounterStatic8ND(60,10,5) { Species = 839, Ability = A4, Moves = new[]{ 246, 053, 157, 523 } }, // Coalossal
            new EncounterStatic8ND(60,10,5) { Species = 839, Ability = A4, Moves = new[]{ 246, 053, 157, 523 }, CanGigantamax = true }, // Coalossal

            // 1/31 - Milcery
            //new EncounterStatic8ND(40,05,3) { Species = 841, Ability = A4, Moves = new[]{ 788, 406, 512, 491 }, CanGigantamax = true }, // Flapple
            //new EncounterStatic8ND(40,05,3) { Species = 839, Ability = A4, Moves = new[]{ 246, 510, 479, 488 }, CanGigantamax = true }, // Coalossal
            //new EncounterStatic8ND(50,08,4) { Species = 841, Ability = A4, Moves = new[]{ 788, 407, 491, 334 }, CanGigantamax = true }, // Flapple -- first two moves swapped match a prior distribution
            //new EncounterStatic8ND(50,08,4) { Species = 839, Ability = A4, Moves = new[]{ 246, 053, 157, 261 }, CanGigantamax = true }, // Coalossal
            //new EncounterStatic8ND(60,10,5) { Species = 841, Ability = A4, Moves = new[]{ 407, 788, 512, 349 }, CanGigantamax = true }, // Flapple
            //new EncounterStatic8ND(60,10,5) { Species = 839, Ability = A4, Moves = new[]{ 246, 053, 157, 523 }, CanGigantamax = true }, // Coalossal

            // 2/6 - Toxtricity
            new EncounterStatic8ND(40,05,3) { Species = 099, Ability = A4, Moves = new[]{ 534, 232, 023, 106 }, CanGigantamax = true }, // Kingler
            new EncounterStatic8ND(40,05,3) { Species = 860, Ability = A4, Moves = new[]{ 492, 577, 421, 141 } }, // Morgrem
            new EncounterStatic8ND(40,05,3) { Species = 849, Ability = A4, Moves = new[]{ 085, 474, 496, 103 }, CanGigantamax = true }, // Toxtricity
            new EncounterStatic8ND(50,08,4) { Species = 099, Ability = A4, Moves = new[]{ 359, 667, 157, 534 }, CanGigantamax = true }, // Kingler
            new EncounterStatic8ND(50,08,4) { Species = 861, Ability = A4, Moves = new[]{ 789, 793, 280, 409 }, CanGigantamax = true }, // Grimmsnarl
            new EncounterStatic8ND(50,08,4) { Species = 849, Ability = A4, Moves = new[]{ 786, 506, 474, 409 }, CanGigantamax = true }, // Toxtricity
            new EncounterStatic8ND(60,10,5) { Species = 099, Ability = A4, Moves = new[]{ 152, 667, 157, 404 }, CanGigantamax = true }, // Kingler
            new EncounterStatic8ND(60,10,5) { Species = 861, Ability = A4, Moves = new[]{ 789, 793, 409, 007 }, CanGigantamax = true }, // Grimmsnarl
            new EncounterStatic8ND(60,10,5) { Species = 849, Ability = A4, Moves = new[]{ 786, 586, 482, 506 }, CanGigantamax = true }, // Toxtricity

            // 2/17 - Toxel
            new EncounterStatic8ND(17,01,1) { Species = 098, Ability = A4, Moves = new[]{ 055, 043, 106, 232 } }, // Krabby
            new EncounterStatic8ND(17,01,1) { Species = 859, Ability = A4, Moves = new[]{ 044, 260, 590, 372 } }, // Impidimp
            new EncounterStatic8ND(30,03,2) { Species = 099, Ability = A4, Moves = new[]{ 232, 341, 061, 023 } }, // Kingler
            new EncounterStatic8ND(30,03,2) { Species = 099, Ability = A4, Moves = new[]{ 232, 341, 061, 023 }, CanGigantamax = true }, // Kingler
            new EncounterStatic8ND(30,03,2) { Species = 859, Ability = A4, Moves = new[]{ 389, 577, 260, 279 } }, // Impidimp
            new EncounterStatic8ND(30,03,2) { Species = 849, Ability = A4, Moves = new[]{ 474, 209, 268, 175 } }, // Toxtricity
            new EncounterStatic8ND(40,05,3) { Species = 099, Ability = A4, Moves = new[]{ 534, 232, 023, 106 } }, // Kingler
            //new EncounterStatic8ND(40,05,3) { Species = 099, Ability = A4, Moves = new[]{ 534, 232, 023, 106 }, CanGigantamax = true }, // Kingler
            //new EncounterStatic8ND(40,05,3) { Species = 860, Ability = A4, Moves = new[]{ 492, 577, 421, 141 } }, // Morgrem
            new EncounterStatic8ND(40,05,3) { Species = 849, Ability = A4, Moves = new[]{ 085, 474, 496, 103 } }, // Toxtricity
            //new EncounterStatic8ND(40,05,3) { Species = 849, Ability = A4, Moves = new[]{ 085, 474, 496, 103 }, CanGigantamax = true }, // Toxtricity
            new EncounterStatic8ND(50,08,4) { Species = 099, Ability = A4, Moves = new[]{ 359, 667, 157, 534 } }, // Kingler
            //new EncounterStatic8ND(50,08,4) { Species = 099, Ability = A4, Moves = new[]{ 359, 667, 157, 534 }, CanGigantamax = true }, // Kingler
            new EncounterStatic8ND(50,08,4) { Species = 861, Ability = A4, Moves = new[]{ 789, 793, 280, 409 } }, // Grimmsnarl
            //new EncounterStatic8ND(50,08,4) { Species = 861, Ability = A4, Moves = new[]{ 789, 793, 280, 409 }, CanGigantamax = true }, // Grimmsnarl
            new EncounterStatic8ND(50,08,4) { Species = 849, Ability = A4, Moves = new[]{ 786, 506, 474, 409 } }, // Toxtricity
            //new EncounterStatic8ND(50,08,4) { Species = 849, Ability = A4, Moves = new[]{ 786, 506, 474, 409 }, CanGigantamax = true }, // Toxtricity
            new EncounterStatic8ND(60,10,5) { Species = 099, Ability = A4, Moves = new[]{ 152, 667, 157, 404 } }, // Kingler
            //new EncounterStatic8ND(60,10,5) { Species = 099, Ability = A4, Moves = new[]{ 152, 667, 157, 404 }, CanGigantamax = true }, // Kingler
            new EncounterStatic8ND(60,10,5) { Species = 861, Ability = A4, Moves = new[]{ 789, 793, 409, 007 } }, // Grimmsnarl
            //new EncounterStatic8ND(60,10,5) { Species = 861, Ability = A4, Moves = new[]{ 789, 793, 409, 007 }, CanGigantamax = true }, // Grimmsnarl
            new EncounterStatic8ND(60,10,5) { Species = 849, Ability = A4, Moves = new[]{ 786, 586, 188, 506 } }, // Toxtricity
            new EncounterStatic8ND(60,10,5) { Species = 849, Ability = A4, Moves = new[]{ 786, 586, 188, 506 }, CanGigantamax = true }, // Toxtricity

            // 2/26 - Mewtwo
            //new EncounterStatic8ND(17,01,1) { Species = 098, Ability = A4, Moves = new[]{ 055, 043, 106, 232 } }, // Krabby
            //new EncounterStatic8ND(17,01,1) { Species = 859, Ability = A4, Moves = new[]{ 044, 260, 590, 372 } }, // Impidimp
            //new EncounterStatic8ND(30,03,2) { Species = 099, Ability = A4, Moves = new[]{ 232, 341, 061, 023 }, CanGigantamax = true }, // Kingler
            //new EncounterStatic8ND(30,03,2) { Species = 859, Ability = A4, Moves = new[]{ 389, 577, 260, 279 } }, // Impidimp
            new EncounterStatic8ND(30,03,2) { Species = 849, Ability = A4, Moves = new[]{ 084, 209, 268, 175 } }, // Toxtricity
            //new EncounterStatic8ND(40,05,3) { Species = 099, Ability = A4, Moves = new[]{ 534, 232, 023, 106 }, CanGigantamax = true }, // Kingler
            //new EncounterStatic8ND(40,05,3) { Species = 860, Ability = A4, Moves = new[]{ 492, 577, 421, 141 } }, // Morgrem
            //new EncounterStatic8ND(40,05,3) { Species = 849, Ability = A4, Moves = new[]{ 085, 474, 496, 103 }, CanGigantamax = true }, // Toxtricity
            //new EncounterStatic8ND(50,08,4) { Species = 099, Ability = A4, Moves = new[]{ 359, 667, 157, 534 }, CanGigantamax = true }, // Kingler
            //new EncounterStatic8ND(50,08,4) { Species = 861, Ability = A4, Moves = new[]{ 789, 793, 280, 409 }, CanGigantamax = true }, // Grimmsnarl
            //new EncounterStatic8ND(50,08,4) { Species = 849, Ability = A4, Moves = new[]{ 786, 506, 474, 409 }, CanGigantamax = true }, // Toxtricity
            //new EncounterStatic8ND(60,10,5) { Species = 099, Ability = A4, Moves = new[]{ 152, 667, 157, 404 }, CanGigantamax = true }, // Kingler
            //new EncounterStatic8ND(60,10,5) { Species = 861, Ability = A4, Moves = new[]{ 789, 793, 409, 007 }, CanGigantamax = true }, // Grimmsnarl
            //new EncounterStatic8ND(60,10,5) { Species = 849, Ability = A4, Moves = new[]{ 786, 586, 188, 506 }, CanGigantamax = true }, // Toxtricity

            // 3/8 - Gengar/Machamp
            new EncounterStatic8ND(60,10,5) { Species = 068, Ability = A4, Moves = new[]{ 238, 007, 008, 089 } }, // Machamp
            new EncounterStatic8ND(60,10,5) { Species = 068, Ability = A4, Moves = new[]{ 238, 007, 008, 089 }, CanGigantamax = true }, // Machamp

            // 3/18 Exclusives & Food
            new EncounterStatic8ND(17,01,1) { Species = 222, Ability = A4, Moves = new[]{ 033, 106, 310, 050 }, Form = 1 }, // Corsola-1
            new EncounterStatic8ND(30,03,2) { Species = 077, Ability = A4, Moves = new[]{ 093, 584, 060, 023 }, Form = 1 }, // Ponyta-1
            new EncounterStatic8ND(30,03,2) { Species = 222, Ability = A4, Moves = new[]{ 310, 050, 246, 506 }, Form = 1 }, // Corsola-1
            new EncounterStatic8ND(40,05,3) { Species = 077, Ability = A2, Moves = new[]{ 340, 023, 428, 583 }, Form = 1 }, // Ponyta-1
            new EncounterStatic8ND(40,05,3) { Species = 222, Ability = A2, Moves = new[]{ 506, 408, 503, 261 }, Form = 1 }, // Corsola-1
            new EncounterStatic8ND(60,10,5) { Species = 765, Ability = A2, Moves = new[]{ 492, 094, 085, 247 } }, // Oranguru
            new EncounterStatic8ND(60,10,5) { Species = 876, Ability = A2, Moves = new[]{ 094, 595, 605, 304 }, Form = 1 }, // Indeedee-1
            new EncounterStatic8ND(60,10,5) { Species = 630, Ability = A2, Moves = new[]{ 403, 555, 492, 211 } }, // Mandibuzz
            new EncounterStatic8ND(60,10,5) { Species = 078, Ability = A2, Moves = new[]{ 428, 583, 224, 340 }, Form = 1 }, // Rapidash-1
            //new EncounterStatic8ND(60,10,5) { Species = 068, Ability = A4, Moves = new[]{ 238, 007, 008, 089 }, CanGigantamax = true }, // Machamp

            // 3/25 - Charizard
            new EncounterStatic8ND(60,10,5) { Species = 879, Ability = A4, Moves = new[]{ 442, 583, 438, 089 }, CanGigantamax = true }, // Copperajah
            new EncounterStatic8ND(60,10,5) { Species = 884, Ability = A4, Moves = new[]{ 430, 406, 085, 334 }, CanGigantamax = true }, // Duraludon

            // 4/27 - Meta
            new EncounterStatic8ND(17,01,1) { Species = 479, Ability = A4, Moves = new[]{ 104, 084, 109 }, Form = 1 }, // Rotom-1
            new EncounterStatic8ND(30,03,2) { Species = 479, Ability = A4, Moves = new[]{ 104, 085, 109 }, Form = 1 }, // Rotom-1
            new EncounterStatic8ND(40,05,3) { Species = 479, Ability = A4, Moves = new[]{ 521, 085, 506 }, Form = 1 }, // Rotom-1
            new EncounterStatic8ND(50,08,4) { Species = 479, Ability = A4, Moves = new[]{ 521, 085, 261 }, Form = 1 }, // Rotom-1
            new EncounterStatic8ND(60,10,5) { Species = 479, Ability = A4, Moves = new[]{ 521, 435, 261 }, Form = 1 }, // Rotom-1

            // 6/2 - Gigantamax
            new EncounterStatic8ND(17,01,1) { Species = 869, Ability = A4, Moves = new[]{ 033, 186, 577, 230 }, CanGigantamax = true }, // Alcremie
            new EncounterStatic8ND(30,03,2) { Species = 851, Ability = A4, Moves = new[]{ 044, 172, 489, 693 }, CanGigantamax = true }, // Centiskorch
            new EncounterStatic8ND(30,03,2) { Species = 131, Ability = A4, Moves = new[]{ 352, 420, 109, 047 }, CanGigantamax = true }, // Lapras
          //new EncounterStatic8ND(40,05,3) { Species = 099, Ability = A4, Moves = new[]{ 534, 232, 023, 106 }, CanGigantamax = true }, // Kingler
            new EncounterStatic8ND(40,05,3) { Species = 842, Ability = A4, Moves = new[]{ 787, 496, 406, 523 }, CanGigantamax = true }, // Appletun
            new EncounterStatic8ND(40,05,3) { Species = 851, Ability = A4, Moves = new[]{ 141, 424, 422, 044 }, CanGigantamax = true }, // Centiskorch
            new EncounterStatic8ND(50,08,4) { Species = 823, Ability = A4, Moves = new[]{ 413, 442, 269, 103 }, CanGigantamax = true }, // Corviknight
          //new EncounterStatic8ND(50,08,4) { Species = 861, Ability = A4, Moves = new[]{ 789, 793, 280, 409 }, CanGigantamax = true }, // Grimmsnarl
            new EncounterStatic8ND(50,08,4) { Species = 569, Ability = A4, Moves = new[]{ 188, 499, 034, 707 }, CanGigantamax = true }, // Garbodor
            new EncounterStatic8ND(50,08,4) { Species = 869, Ability = A4, Moves = new[]{ 577, 605, 105, 500 }, CanGigantamax = true }, // Alcremie
            new EncounterStatic8ND(60,10,5) { Species = 131, Ability = A4, Moves = new[]{ 057, 196, 058, 329 }, CanGigantamax = true }, // Lapras
            new EncounterStatic8ND(60,10,5) { Species = 849, Ability = A4, Moves = new[]{ 786, 506, 474, 409 }, CanGigantamax = true }, // Toxtricity
            new EncounterStatic8ND(60,10,5) { Species = 094, Ability = A4, Moves = new[]{ 247, 482, 094, 196 }, CanGigantamax = true }, // Gengar
          //new EncounterStatic8ND(60,10,5) { Species = 884, Ability = A4, Moves = new[]{ 430, 406, 085, 334 }, CanGigantamax = true }, // Duraludon
        };

        internal static readonly EncounterStatic8ND[] Dist_SH =
        {
            // 11/15 - Butterfree
            new EncounterStatic8ND(17,01,1) { Species = 012, Ability = A4, Moves = new[]{ 405, 060, 016, 079 } }, // Butterfree
            new EncounterStatic8ND(17,01,1) { Species = 012, Ability = A4, Moves = new[]{ 405, 060, 016, 079 }, CanGigantamax = true }, // Butterfree
            new EncounterStatic8ND(17,01,1) { Species = 821, Ability = A2, Moves = new[]{ 365, 031, 526, 064 } }, // Rookidee
            new EncounterStatic8ND(17,01,1) { Species = 821, Ability = A4, Moves = new[]{ 365, 031, 526, 064 } }, // Rookidee
            new EncounterStatic8ND(17,01,1) { Species = 850, Ability = A2, Moves = new[]{ 044, 172, 450, 693 } }, // Sizzlipede
            new EncounterStatic8ND(17,01,1) { Species = 850, Ability = A4, Moves = new[]{ 044, 172, 450, 693 } }, // Sizzlipede
            new EncounterStatic8ND(30,03,2) { Species = 012, Ability = A4, Moves = new[]{ 405, 403, 081, 077 } }, // Butterfree
            new EncounterStatic8ND(30,03,2) { Species = 012, Ability = A4, Moves = new[]{ 405, 403, 081, 077 }, CanGigantamax = true }, // Butterfree
            new EncounterStatic8ND(30,03,2) { Species = 822, Ability = A2, Moves = new[]{ 365, 263, 179, 468 } }, // Corvisquire
            new EncounterStatic8ND(30,03,2) { Species = 822, Ability = A4, Moves = new[]{ 365, 263, 179, 468 } }, // Corvisquire
            new EncounterStatic8ND(30,03,2) { Species = 851, Ability = A2, Moves = new[]{ 172, 242, 450, 257 } }, // Centiskorch
            new EncounterStatic8ND(30,03,2) { Species = 851, Ability = A4, Moves = new[]{ 172, 242, 450, 257 } }, // Centiskorch
            new EncounterStatic8ND(40,05,3) { Species = 823, Ability = A4, Moves = new[]{ 065, 211, 034, 103 } }, // Corviknight
            new EncounterStatic8ND(40,05,3) { Species = 823, Ability = A4, Moves = new[]{ 065, 211, 034, 103 }, CanGigantamax = true }, // Corviknight
            new EncounterStatic8ND(40,05,3) { Species = 851, Ability = A4, Moves = new[]{ 141, 438, 053, 489 } }, // Centiskorch
            new EncounterStatic8ND(40,05,3) { Species = 851, Ability = A4, Moves = new[]{ 141, 438, 053, 489 }, CanGigantamax = true }, // Centiskorch
            new EncounterStatic8ND(50,08,4) { Species = 823, Ability = A4, Moves = new[]{ 065, 442, 034, 334 } }, // Corviknight
            new EncounterStatic8ND(50,08,4) { Species = 823, Ability = A4, Moves = new[]{ 065, 442, 034, 334 }, CanGigantamax = true }, // Corviknight
            new EncounterStatic8ND(50,08,4) { Species = 851, Ability = A4, Moves = new[]{ 141, 424, 422, 242 } }, // Centiskorch
            new EncounterStatic8ND(50,08,4) { Species = 851, Ability = A4, Moves = new[]{ 141, 424, 422, 242 }, CanGigantamax = true }, // Centiskorch
            new EncounterStatic8ND(60,10,5) { Species = 012, Ability = A4, Moves = new[]{ 405, 094, 403, 079 } }, // Butterfree
            new EncounterStatic8ND(70,10,5) { Species = 012, Ability = A4, Moves = new[]{ 405, 094, 403, 079 }, CanGigantamax = true }, // Butterfree
            new EncounterStatic8ND(60,10,5) { Species = 823, Ability = A4, Moves = new[]{ 413, 442, 249, 103 } }, // Corviknight
            new EncounterStatic8ND(70,10,5) { Species = 823, Ability = A4, Moves = new[]{ 413, 442, 249, 103 }, CanGigantamax = true }, // Corviknight
            new EncounterStatic8ND(60,10,5) { Species = 851, Ability = A4, Moves = new[]{ 679, 257, 083, 438 } }, // Centiskorch
            new EncounterStatic8ND(70,10,5) { Species = 851, Ability = A4, Moves = new[]{ 679, 257, 083, 438 }, CanGigantamax = true }, // Centiskorch

            // 12/03 - Snorlax
            //new EncounterStatic8ND(17,01,1) { Species = 012, Ability = A4, Moves = new[]{ 405, 060, 016, 079 } }, // Butterfree
            //new EncounterStatic8ND(17,01,1) { Species = 012, Ability = A4, Moves = new[]{ 405, 060, 016, 079 }, CanGigantamax = true }, // Butterfree
            new EncounterStatic8ND(17,01,1) { Species = 821, Ability = A4, Moves = new[]{ 403, 031, 116, 064 } }, // Rookidee
            new EncounterStatic8ND(17,01,1) { Species = 850, Ability = A4, Moves = new[]{ 044, 172, 404, 693 } }, // Sizzlipede
            //new EncounterStatic8ND(30,03,2) { Species = 012, Ability = A4, Moves = new[]{ 405, 403, 081, 077 }, CanGigantamax = true }, // Butterfree
            new EncounterStatic8ND(30,03,2) { Species = 822, Ability = A4, Moves = new[]{ 403, 263, 179, 468 } }, // Corvisquire
            new EncounterStatic8ND(30,03,2) { Species = 851, Ability = A4, Moves = new[]{ 172, 242, 404, 257 } }, // Centiskorch
            new EncounterStatic8ND(30,03,2) { Species = 851, Ability = A2, Moves = new[]{ 172, 242, 404, 257 } }, // Centiskorch
            //new EncounterStatic8ND(40,05,3) { Species = 823, Ability = A4, Moves = new[]{ 065, 211, 034, 103 }, CanGigantamax = true }, // Corviknight
            //new EncounterStatic8ND(40,05,3) { Species = 851, Ability = A4, Moves = new[]{ 141, 438, 053, 489 } }, // Centiskorch
            //new EncounterStatic8ND(40,05,3) { Species = 851, Ability = A4, Moves = new[]{ 141, 438, 053, 489 }, CanGigantamax = true }, // Centiskorch
            //new EncounterStatic8ND(50,08,4) { Species = 823, Ability = A4, Moves = new[]{ 065, 442, 034, 334 }, CanGigantamax = true }, // Corviknight
            //new EncounterStatic8ND(50,08,4) { Species = 851, Ability = A4, Moves = new[]{ 141, 424, 422, 242 } }, // Centiskorch
            //new EncounterStatic8ND(50,08,4) { Species = 851, Ability = A4, Moves = new[]{ 141, 424, 422, 242 }, CanGigantamax = true }, // Centiskorch
            new EncounterStatic8ND(60,10,5) { Species = 012, Ability = A4, Moves = new[]{ 405, 094, 403, 079 }, CanGigantamax = true }, // Butterfree
            //new EncounterStatic8ND(70,10,5) { Species = 823, Ability = A4, Moves = new[]{ 413, 442, 249, 103 }, CanGigantamax = true }, // Corviknight
            //new EncounterStatic8ND(60,10,5) { Species = 851, Ability = A4, Moves = new[]{ 679, 257, 083, 438 } }, // Centiskorch
            //new EncounterStatic8ND(70,10,5) { Species = 851, Ability = A4, Moves = new[]{ 679, 257, 083, 438 }, CanGigantamax = true }, // Centiskorch

            // 12/20 - Delibird
            //new EncounterStatic8ND(17,01,1) { Species = 012, Ability = A4, Moves = new[]{ 405, 060, 016, 079 } }, // Butterfree
            //new EncounterStatic8ND(17,01,1) { Species = 012, Ability = A4, Moves = new[]{ 405, 060, 016, 079 }, CanGigantamax = true }, // Butterfree
            //new EncounterStatic8ND(17,01,1) { Species = 821, Ability = A4, Moves = new[]{ 403, 031, 116, 064 } }, // Rookidee
            new EncounterStatic8ND(17,01,1) { Species = 850, Ability = A4, Moves = new[]{ 044, 172, 141, 693 } }, // Sizzlipede
            //new EncounterStatic8ND(30,03,2) { Species = 012, Ability = A4, Moves = new[]{ 405, 403, 081, 077 }, CanGigantamax = true }, // Butterfree
            //new EncounterStatic8ND(30,03,2) { Species = 822, Ability = A4, Moves = new[]{ 403, 263, 179, 468 } }, // Corvisquire
            //new EncounterStatic8ND(30,03,2) { Species = 851, Ability = A4, Moves = new[]{ 172, 242, 404, 257 } }, // Centiskorch
            //new EncounterStatic8ND(40,05,3) { Species = 823, Ability = A4, Moves = new[]{ 065, 211, 034, 103 }, CanGigantamax = true }, // Corviknight
            //new EncounterStatic8ND(40,05,3) { Species = 851, Ability = A4, Moves = new[]{ 141, 438, 053, 489 } }, // Centiskorch
            //new EncounterStatic8ND(40,05,3) { Species = 851, Ability = A4, Moves = new[]{ 141, 438, 053, 489 }, CanGigantamax = true }, // Centiskorch
            //new EncounterStatic8ND(50,08,4) { Species = 823, Ability = A4, Moves = new[]{ 065, 442, 034, 334 }, CanGigantamax = true }, // Corviknight
            //new EncounterStatic8ND(50,08,4) { Species = 851, Ability = A4, Moves = new[]{ 141, 424, 422, 242 } }, // Centiskorch
            //new EncounterStatic8ND(50,08,4) { Species = 851, Ability = A4, Moves = new[]{ 141, 424, 422, 242 }, CanGigantamax = true }, // Centiskorch
            //new EncounterStatic8ND(60,10,5) { Species = 012, Ability = A4, Moves = new[]{ 405, 094, 403, 079 }, CanGigantamax = true }, // Butterfree
            //new EncounterStatic8ND(70,10,5) { Species = 823, Ability = A4, Moves = new[]{ 413, 442, 249, 103 }, CanGigantamax = true }, // Corviknight
            //new EncounterStatic8ND(60,10,5) { Species = 851, Ability = A4, Moves = new[]{ 679, 257, 083, 438 } }, // Centiskorch
            //new EncounterStatic8ND(70,10,5) { Species = 851, Ability = A4, Moves = new[]{ 679, 257, 083, 438 }, CanGigantamax = true }, // Centiskorch

            // 12/30 - Magikarp
            //new EncounterStatic8ND(17,01,1) { Species = 012, Ability = A4, Moves = new[]{ 405, 060, 016, 079 }, CanGigantamax = true }, // Butterfree
            //new EncounterStatic8ND(17,01,1) { Species = 821, Ability = A4, Moves = new[]{ 403, 031, 116, 064 } }, // Rookidee
            //new EncounterStatic8ND(17,01,1) { Species = 850, Ability = A4, Moves = new[]{ 044, 172, 141, 693 } }, // Sizzlipede
            //new EncounterStatic8ND(30,03,2) { Species = 012, Ability = A4, Moves = new[]{ 405, 403, 081, 077 }, CanGigantamax = true }, // Butterfree
            //new EncounterStatic8ND(30,03,2) { Species = 822, Ability = A4, Moves = new[]{ 403, 263, 179, 468 } }, // Corvisquire
            //new EncounterStatic8ND(30,03,2) { Species = 851, Ability = A4, Moves = new[]{ 172, 242, 404, 257 } }, // Centiskorch
            //new EncounterStatic8ND(40,05,3) { Species = 823, Ability = A4, Moves = new[]{ 065, 211, 034, 103 }, CanGigantamax = true }, // Corviknight
            //new EncounterStatic8ND(40,05,3) { Species = 851, Ability = A4, Moves = new[]{ 141, 438, 053, 489 }, CanGigantamax = true }, // Centiskorch
            //new EncounterStatic8ND(50,08,4) { Species = 823, Ability = A4, Moves = new[]{ 065, 442, 034, 334 }, CanGigantamax = true }, // Corviknight
            //new EncounterStatic8ND(50,08,4) { Species = 851, Ability = A4, Moves = new[]{ 141, 424, 422, 242 }, CanGigantamax = true }, // Centiskorch
            //new EncounterStatic8ND(60,10,5) { Species = 012, Ability = A4, Moves = new[]{ 405, 094, 403, 079 }, CanGigantamax = true }, // Butterfree
            //new EncounterStatic8ND(70,10,5) { Species = 823, Ability = A4, Moves = new[]{ 413, 442, 249, 103 }, CanGigantamax = true }, // Corviknight
            //new EncounterStatic8ND(70,10,5) { Species = 851, Ability = A4, Moves = new[]{ 679, 257, 083, 438 }, CanGigantamax = true }, // Centiskorch

            // 1/8 - Applin
            new EncounterStatic8ND(17,01,1) { Species = 131, Ability = A4, Moves = new[]{ 055, 496, 045, 047 } }, // Lapras
            new EncounterStatic8ND(17,01,1) { Species = 131, Ability = A2, Moves = new[]{ 055, 496, 045, 047 } }, // Lapras
            new EncounterStatic8ND(30,03,2) { Species = 842, Ability = A4, Moves = new[]{ 787, 029, 389, 073 } }, // Appletun
            new EncounterStatic8ND(30,03,2) { Species = 842, Ability = A2, Moves = new[]{ 787, 029, 389, 073 } }, // Appletun
            new EncounterStatic8ND(30,03,2) { Species = 131, Ability = A4, Moves = new[]{ 352, 420, 109, 047 } }, // Lapras
            new EncounterStatic8ND(30,03,2) { Species = 131, Ability = A2, Moves = new[]{ 352, 420, 109, 047 } }, // Lapras
            new EncounterStatic8ND(40,05,3) { Species = 842, Ability = A4, Moves = new[]{ 787, 496, 406, 523 } }, // Appletun
            new EncounterStatic8ND(40,05,3) { Species = 842, Ability = A4, Moves = new[]{ 787, 496, 406, 523 }, CanGigantamax = true }, // Appletun
            new EncounterStatic8ND(40,05,3) { Species = 869, Ability = A4, Moves = new[]{ 577, 605, 202, 186 }, Form = 1, CanGigantamax = true }, // Alcremie-1
            new EncounterStatic8ND(40,05,3) { Species = 869, Ability = A4, Moves = new[]{ 577, 605, 202, 186 }, Form = 2, CanGigantamax = true }, // Alcremie-2
            new EncounterStatic8ND(40,05,3) { Species = 131, Ability = A4, Moves = new[]{ 352, 420, 109, 034 } }, // Lapras
            new EncounterStatic8ND(40,05,3) { Species = 131, Ability = A4, Moves = new[]{ 352, 420, 109, 034 }, CanGigantamax = true }, // Lapras
            new EncounterStatic8ND(50,08,4) { Species = 842, Ability = A4, Moves = new[]{ 787, 202, 406, 089 } }, // Appletun
            new EncounterStatic8ND(50,08,4) { Species = 842, Ability = A4, Moves = new[]{ 787, 202, 406, 089 }, CanGigantamax = true }, // Appletun
            new EncounterStatic8ND(50,08,4) { Species = 869, Ability = A4, Moves = new[]{ 605, 202, 094, 151 }, Form = 7, CanGigantamax = true }, // Alcremie-7
            new EncounterStatic8ND(50,08,4) { Species = 869, Ability = A4, Moves = new[]{ 605, 202, 094, 151 }, Form = 8, CanGigantamax = true }, // Alcremie-8
            new EncounterStatic8ND(50,08,4) { Species = 131, Ability = A4, Moves = new[]{ 057, 058, 246, 523 } }, // Lapras
            new EncounterStatic8ND(50,08,4) { Species = 131, Ability = A4, Moves = new[]{ 057, 058, 246, 523 }, CanGigantamax = true }, // Lapras
            new EncounterStatic8ND(60,10,5) { Species = 842, Ability = A4, Moves = new[]{ 787, 406, 412, 089 } }, // Appletun
            new EncounterStatic8ND(60,10,5) { Species = 842, Ability = A4, Moves = new[]{ 787, 406, 412, 089 }, CanGigantamax = true }, // Appletun
            new EncounterStatic8ND(60,10,5) { Species = 869, Ability = A4, Moves = new[]{ 605, 202, 595, 500 }, Form = 3, CanGigantamax = true }, // Alcremie-3
            new EncounterStatic8ND(60,10,5) { Species = 869, Ability = A4, Moves = new[]{ 605, 202, 595, 500 }, Form = 4, CanGigantamax = true }, // Alcremie-4
            new EncounterStatic8ND(60,10,5) { Species = 131, Ability = A4, Moves = new[]{ 057, 196, 573, 329 } }, // Lapras
            new EncounterStatic8ND(60,10,5) { Species = 131, Ability = A4, Moves = new[]{ 057, 196, 573, 329 }, CanGigantamax = true }, // Lapras

            // 1/31 - Milcery
            //new EncounterStatic8ND(40,05,3) { Species = 842, Ability = A4, Moves = new[]{ 787, 496, 406, 523 }, CanGigantamax = true }, // Appletun
            //new EncounterStatic8ND(40,05,3) { Species = 131, Ability = A4, Moves = new[]{ 352, 420, 109, 034 }, CanGigantamax = true }, // Lapras
            //new EncounterStatic8ND(50,08,4) { Species = 842, Ability = A4, Moves = new[]{ 787, 202, 406, 089 }, CanGigantamax = true }, // Appletun
            //new EncounterStatic8ND(50,08,4) { Species = 131, Ability = A4, Moves = new[]{ 057, 058, 246, 523 }, CanGigantamax = true }, // Lapras
            //new EncounterStatic8ND(60,10,5) { Species = 842, Ability = A4, Moves = new[]{ 787, 406, 412, 089 }, CanGigantamax = true }, // Appletun
            //new EncounterStatic8ND(60,10,5) { Species = 131, Ability = A4, Moves = new[]{ 057, 196, 573, 329 }, CanGigantamax = true }, // Lapras

            // 2/6 - Toxtricity
            new EncounterStatic8ND(40,05,3) { Species = 826, Ability = A4, Moves = new[]{ 522, 060, 109, 202 }, CanGigantamax = true }, // Orbeetle
            new EncounterStatic8ND(40,05,3) { Species = 857, Ability = A4, Moves = new[]{ 605, 345, 399, 500 } }, // Hattrem
            new EncounterStatic8ND(40,05,3) { Species = 849, Ability = A4, Moves = new[]{ 085, 599, 496, 103 }, Form = 1, CanGigantamax = true }, // Toxtricity-1
            new EncounterStatic8ND(50,08,4) { Species = 826, Ability = A4, Moves = new[]{ 405, 094, 202, 247 }, CanGigantamax = true }, // Orbeetle
            new EncounterStatic8ND(50,08,4) { Species = 858, Ability = A4, Moves = new[]{ 605, 094, 595, 247 }, CanGigantamax = true }, // Hatterene
            new EncounterStatic8ND(50,08,4) { Species = 849, Ability = A4, Moves = new[]{ 786, 506, 599, 409 }, Form = 1, CanGigantamax = true }, // Toxtricity-1
            new EncounterStatic8ND(60,10,5) { Species = 826, Ability = A4, Moves = new[]{ 405, 094, 247, 347 }, CanGigantamax = true }, // Orbeetle
            new EncounterStatic8ND(60,10,5) { Species = 858, Ability = A4, Moves = new[]{ 605, 094, 595, 500 }, CanGigantamax = true }, // Hatterene
            new EncounterStatic8ND(60,10,5) { Species = 849, Ability = A4, Moves = new[]{ 786, 586, 482, 506 }, Form = 1, CanGigantamax = true }, // Toxtricity-1

            // 2/17 - Toxel
            new EncounterStatic8ND(17,01,1) { Species = 825, Ability = A4, Moves = new[]{ 093, 522, 113, 115 } }, // Dottler
            new EncounterStatic8ND(17,01,1) { Species = 856, Ability = A4, Moves = new[]{ 093, 589, 791, 574 } }, // Hatenna
            new EncounterStatic8ND(30,03,2) { Species = 826, Ability = A4, Moves = new[]{ 405, 060, 496, 095 } }, // Orbeetle
            new EncounterStatic8ND(30,03,2) { Species = 826, Ability = A4, Moves = new[]{ 405, 060, 496, 095 }, CanGigantamax = true }, // Orbeetle
            new EncounterStatic8ND(30,03,2) { Species = 856, Ability = A4, Moves = new[]{ 605, 060, 345, 347 } }, // Hatenna
            new EncounterStatic8ND(30,03,2) { Species = 849, Ability = A4, Moves = new[]{ 599, 209, 268, 175 }, Form = 1 }, // Toxtricity-1
            new EncounterStatic8ND(40,05,3) { Species = 826, Ability = A4, Moves = new[]{ 405, 060, 202, 109 } }, // Orbeetle
            new EncounterStatic8ND(40,05,3) { Species = 826, Ability = A4, Moves = new[]{ 405, 060, 202, 109 }, CanGigantamax = true }, // Orbeetle
            //new EncounterStatic8ND(40,05,3) { Species = 857, Ability = A4, Moves = new[]{ 605, 345, 399, 500 } }, // Hattrem
            new EncounterStatic8ND(40,05,3) { Species = 849, Ability = A4, Moves = new[]{ 085, 599, 496, 103 }, Form = 1 }, // Toxtricity-1
            //new EncounterStatic8ND(40,05,3) { Species = 849, Ability = A4, Moves = new[]{ 085, 599, 496, 103 }, Form = 1, CanGigantamax = true }, // Toxtricity-1
            new EncounterStatic8ND(50,08,4) { Species = 826, Ability = A4, Moves = new[]{ 405, 094, 202, 247 } }, // Orbeetle
            //new EncounterStatic8ND(50,08,4) { Species = 826, Ability = A4, Moves = new[]{ 405, 094, 202, 247 }, CanGigantamax = true }, // Orbeetle
            new EncounterStatic8ND(50,08,4) { Species = 858, Ability = A4, Moves = new[]{ 605, 094, 595, 247 } }, // Hatterene
            //new EncounterStatic8ND(50,08,4) { Species = 858, Ability = A4, Moves = new[]{ 605, 094, 595, 247 }, CanGigantamax = true }, // Hatterene
            new EncounterStatic8ND(50,08,4) { Species = 849, Ability = A4, Moves = new[]{ 786, 506, 599, 409 }, Form = 1 }, // Toxtricity-1
            //new EncounterStatic8ND(50,08,4) { Species = 849, Ability = A4, Moves = new[]{ 786, 506, 599, 409 }, Form = 1, CanGigantamax = true }, // Toxtricity-1
            new EncounterStatic8ND(60,10,5) { Species = 826, Ability = A4, Moves = new[]{ 405, 094, 247, 347 } }, // Orbeetle
            //new EncounterStatic8ND(60,10,5) { Species = 826, Ability = A4, Moves = new[]{ 405, 094, 247, 347 }, CanGigantamax = true }, // Orbeetle
            new EncounterStatic8ND(60,10,5) { Species = 858, Ability = A4, Moves = new[]{ 605, 094, 595, 500 } }, // Hatterene
            //new EncounterStatic8ND(60,10,5) { Species = 858, Ability = A4, Moves = new[]{ 605, 094, 595, 500 }, CanGigantamax = true }, // Hatterene
            new EncounterStatic8ND(60,10,5) { Species = 849, Ability = A4, Moves = new[]{ 786, 586, 188, 506 }, Form = 1 }, // Toxtricity-1
            new EncounterStatic8ND(60,10,5) { Species = 849, Ability = A4, Moves = new[]{ 786, 586, 188, 506 }, Form = 1, CanGigantamax = true }, // Toxtricity-1

            // 2/26 - Mewtwo
            //new EncounterStatic8ND(17,01,1) { Species = 825, Ability = A4, Moves = new[]{ 093, 522, 113, 115 } }, // Dottler
            //new EncounterStatic8ND(17,01,1) { Species = 856, Ability = A4, Moves = new[]{ 093, 589, 791, 574 } }, // Hatenna
            //new EncounterStatic8ND(30,03,2) { Species = 826, Ability = A4, Moves = new[]{ 405, 060, 496, 095 }, CanGigantamax = true }, // Orbeetle
            //new EncounterStatic8ND(30,03,2) { Species = 856, Ability = A4, Moves = new[]{ 605, 060, 345, 347 } }, // Hatenna
            //new EncounterStatic8ND(30,03,2) { Species = 849, Ability = A4, Moves = new[]{ 599, 209, 268, 175 }, Form = 1 }, // Toxtricity-1
            //new EncounterStatic8ND(40,05,3) { Species = 826, Ability = A4, Moves = new[]{ 405, 060, 202, 109 }, CanGigantamax = true }, // Orbeetle
            //new EncounterStatic8ND(40,05,3) { Species = 857, Ability = A4, Moves = new[]{ 605, 345, 399, 500 } }, // Hattrem
            //new EncounterStatic8ND(40,05,3) { Species = 849, Ability = A4, Moves = new[]{ 085, 599, 496, 103 }, Form = 1, CanGigantamax = true }, // Toxtricity-1
            //new EncounterStatic8ND(50,08,4) { Species = 826, Ability = A4, Moves = new[]{ 405, 094, 202, 247 }, CanGigantamax = true }, // Orbeetle
            //new EncounterStatic8ND(50,08,4) { Species = 858, Ability = A4, Moves = new[]{ 605, 094, 595, 247 }, CanGigantamax = true }, // Hatterene
            //new EncounterStatic8ND(50,08,4) { Species = 849, Ability = A4, Moves = new[]{ 786, 506, 599, 409 }, Form = 1, CanGigantamax = true }, // Toxtricity-1
            //new EncounterStatic8ND(60,10,5) { Species = 826, Ability = A4, Moves = new[]{ 405, 094, 247, 347 }, CanGigantamax = true }, // Orbeetle
            //new EncounterStatic8ND(60,10,5) { Species = 858, Ability = A4, Moves = new[]{ 605, 094, 595, 500 }, CanGigantamax = true }, // Hatterene
            //new EncounterStatic8ND(60,10,5) { Species = 849, Ability = A4, Moves = new[]{ 786, 586, 188, 506 }, Form = 1, CanGigantamax = true }, // Toxtricity-1

            // 3/8 - Gengar/Machamp
            new EncounterStatic8ND(60,10,5) { Species = 094, Ability = A4, Moves = new[]{ 247, 482, 094, 196 } }, // Gengar
            new EncounterStatic8ND(60,10,5) { Species = 094, Ability = A4, Moves = new[]{ 247, 482, 094, 196 }, CanGigantamax = true }, // Gengar

            // 3/18 - Exclusives & Food
            new EncounterStatic8ND(17,01,1) { Species = 554, Ability = A4, Moves = new[]{ 033, 181, 044, 419 }, Form = 1 }, // Darumaka-1
            new EncounterStatic8ND(30,03,2) { Species = 083, Ability = A4, Moves = new[]{ 064, 028, 249, 693 }, Form = 1 }, // Farfetch’d-1
            new EncounterStatic8ND(30,03,2) { Species = 554, Ability = A4, Moves = new[]{ 423, 029, 424, 280 }, Form = 1 }, // Darumaka-1
            new EncounterStatic8ND(40,05,3) { Species = 083, Ability = A2, Moves = new[]{ 280, 693, 348, 413 }, Form = 1 }, // Farfetch’d-1
            new EncounterStatic8ND(40,05,3) { Species = 554, Ability = A2, Moves = new[]{ 008, 007, 428, 276 }, Form = 1 }, // Darumaka-1
            new EncounterStatic8ND(60,10,5) { Species = 766, Ability = A2, Moves = new[]{ 370, 157, 523, 231 } }, // Passimian
            new EncounterStatic8ND(60,10,5) { Species = 876, Ability = A2, Moves = new[]{ 094, 595, 605, 247 } }, // Indeedee
            new EncounterStatic8ND(60,10,5) { Species = 628, Ability = A2, Moves = new[]{ 413, 276, 442, 157 } }, // Braviary
            new EncounterStatic8ND(60,10,5) { Species = 865, Ability = A2, Moves = new[]{ 370, 413, 211, 675 } }, // Sirfetch’d
            //new EncounterStatic8ND(60,10,5) { Species = 094, Ability = A4, Moves = new[]{ 247, 482, 094, 196 }, CanGigantamax = true }, // Gengar

            // 3/25 - Charizard
            new EncounterStatic8ND(60,10,5) { Species = 569, Ability = A4, Moves = new[]{ 441, 409, 402, 707 }, CanGigantamax = true }, // Garbodor
            new EncounterStatic8ND(60,10,5) { Species = 006, Ability = A4, Moves = new[]{ 257, 403, 406, 411 }, CanGigantamax = true, Shiny = Shiny.Never }, // Charizard

            // 4/09 - Easter Eggs (and fixed Charizard)
            new EncounterStatic8ND(60,10,5) { Species = 569, Ability = A4, Moves = new[]{ 441, 499, 402, 707 }, CanGigantamax = true }, // Garbodor (Drain Punch -> Clear Smog)
            new EncounterStatic8ND(60,10,5) { Species = 006, Ability = A4, Moves = new[]{ 053, 403, 019, 411 }, CanGigantamax = true }, // Charizard (Heat Wave, Dragon Pulse -> Flamethrower/Fly)

            // 4/27 - Meta
            new EncounterStatic8ND(17,01,1) { Species = 479, Ability = A4, Moves = new[]{ 104, 435, 084, 109 }, Form = 2 }, // Rotom-2 // From initial revision: Discharge @lv17 isn't legal, but they distributed it!
            // new EncounterStatic8ND(17,01,1) { Species = 529, Ability = A4, Moves = new[]{ 189, 232, 056, 468 } }, // Drilbur // From initial revision: treat this as illegal.
            new EncounterStatic8ND(17,01,1) { Species = 479, Ability = A4, Moves = new[]{ 104, 084, 109 }, Form = 2 }, // Rotom-2
            new EncounterStatic8ND(30,03,2) { Species = 479, Ability = A4, Moves = new[]{ 104, 085, 109 }, Form = 2 }, // Rotom-2
            new EncounterStatic8ND(40,05,3) { Species = 479, Ability = A4, Moves = new[]{ 521, 435, 085 }, Form = 2 }, // Rotom-2
            new EncounterStatic8ND(50,08,4) { Species = 479, Ability = A4, Moves = new[]{ 521, 435, 247 }, Form = 2 }, // Rotom-2
            new EncounterStatic8ND(60,10,5) { Species = 479, Ability = A4, Moves = new[]{ 521, 247, 261 }, Form = 2 }, // Rotom-2

            // 6/2 - Gigantamax
          //new EncounterStatic8ND(17,01,1) { Species = 012, Ability = A4, Moves = new[]{ 405, 060, 016, 079 }, CanGigantamax = true }, // Butterfree
          //new EncounterStatic8ND(30,03,2) { Species = 826, Ability = A4, Moves = new[]{ 405, 060, 496, 095 }, CanGigantamax = true }, // Orbeetle
            new EncounterStatic8ND(30,03,2) { Species = 068, Ability = A4, Moves = new[]{ 523, 490, 279, 233 }, CanGigantamax = true }, // Machamp
            new EncounterStatic8ND(40,05,3) { Species = 826, Ability = A4, Moves = new[]{ 405, 094, 202, 247 }, CanGigantamax = true }, // Orbeetle
            new EncounterStatic8ND(40,05,3) { Species = 841, Ability = A4, Moves = new[]{ 406, 788, 491, 334 }, CanGigantamax = true }, // Flapple
            new EncounterStatic8ND(40,05,3) { Species = 844, Ability = A4, Moves = new[]{ 693, 529, 201, 091 }, CanGigantamax = true }, // Sandaconda
          //new EncounterStatic8ND(50,08,4) { Species = 834, Ability = A4, Moves = new[]{ 157, 710, 317, 334 }, CanGigantamax = true }, // Drednaw -- duplicate with raid on 7/31 (SW & SH)
          //new EncounterStatic8ND(50,08,4) { Species = 858, Ability = A4, Moves = new[]{ 605, 094, 595, 247 }, CanGigantamax = true }, // Hatterene
          //new EncounterStatic8ND(50,08,4) { Species = 006, Ability = A4, Moves = new[]{ 053, 403, 076, 257 }, CanGigantamax = true }, // Charizard
          //new EncounterStatic8ND(50,08,4) { Species = 012, Ability = A4, Moves = new[]{ 405, 403, 527, 078 }, CanGigantamax = true }, // Butterfree
            new EncounterStatic8ND(60,10,5) { Species = 849, Ability = A4, Moves = new[]{ 786, 506, 599, 409 }, Form = 1, CanGigantamax = true }, // Toxtricity-1
            new EncounterStatic8ND(60,10,5) { Species = 839, Ability = A4, Moves = new[]{ 246, 053, 157, 523 }, CanGigantamax = true }, // Coalossal
            new EncounterStatic8ND(60,10,5) { Species = 068, Ability = A4, Moves = new[]{ 238, 007, 008, 089 }, CanGigantamax = true }, // Machamp
            new EncounterStatic8ND(60,10,5) { Species = 879, Ability = A4, Moves = new[]{ 442, 583, 438, 089 }, CanGigantamax = true }, // Copperajah
        };
    }
}
