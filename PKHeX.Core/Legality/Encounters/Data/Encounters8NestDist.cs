using static PKHeX.Core.GameVersion;

namespace PKHeX.Core
{
    // Distribution Nest Encounters (BCAT)
    internal static partial class Encounters8Nest
    {
        // For distribution encounters, all commented out entries are duplicate with a prior distribution encounter. Only one encounter is necessary for matching purposes.

        internal static readonly EncounterStatic8ND[] Dist_Common =
        {
            new(40,05,3) { Species = 012, Ability = A4, Moves = new[]{ 676, 403, 202, 527 }, Index = 1 }, // Butterfree
            new(40,05,3) { Species = 012, Ability = A4, Moves = new[]{ 676, 403, 202, 527 }, Index = 1, CanGigantamax = true }, // Butterfree
            new(50,08,4) { Species = 012, Ability = A4, Moves = new[]{ 405, 403, 527, 078 }, Index = 1 }, // Butterfree
            new(50,08,4) { Species = 012, Ability = A4, Moves = new[]{ 405, 403, 527, 078 }, Index = 1, CanGigantamax = true }, // Butterfree

            new(17,01,1) { Species = 446, Ability = A4, Moves = new[]{ 033, 044, 122, 111 }, Index = 2 }, // Munchlax
            new(17,01,1) { Species = 446, Ability = A2, Moves = new[]{ 033, 044, 122, 111 }, Index = 2 }, // Munchlax
            new(30,03,2) { Species = 143, Ability = A4, Moves = new[]{ 034, 242, 118, 111 }, Index = 2 }, // Snorlax
            new(30,03,2) { Species = 143, Ability = A2, Moves = new[]{ 034, 242, 118, 111 }, Index = 2 }, // Snorlax
            new(40,05,3) { Species = 012, Ability = A4, Moves = new[]{ 676, 403, 202, 527 }, Index = 2, CanGigantamax = true }, // Butterfree
            new(40,05,3) { Species = 143, Ability = A4, Moves = new[]{ 034, 667, 242, 281 }, Index = 2 }, // Snorlax
            new(40,05,3) { Species = 143, Ability = A4, Moves = new[]{ 034, 667, 242, 281 }, Index = 2, CanGigantamax = true }, // Snorlax
            new(50,08,4) { Species = 012, Ability = A4, Moves = new[]{ 405, 403, 527, 078 }, Index = 2, CanGigantamax = true }, // Butterfree
            new(50,08,4) { Species = 143, Ability = A4, Moves = new[]{ 034, 667, 280, 523 }, Index = 2 }, // Snorlax
            new(50,08,4) { Species = 143, Ability = A4, Moves = new[]{ 034, 667, 280, 523 }, Index = 2, CanGigantamax = true }, // Snorlax
            new(60,10,5) { Species = 143, Ability = A4, Moves = new[]{ 034, 442, 242, 428 }, Index = 2 }, // Snorlax
            new(70,10,5) { Species = 143, Ability = A4, Moves = new[]{ 034, 442, 242, 428 }, Index = 2, CanGigantamax = true }, // Snorlax

            new(17,01,1) { Species = 446, Ability = A4, Moves = new[]{ 033, 044, 122, 111 }, Index = 3 }, // Munchlax
            new(17,01,1) { Species = 225, Ability = A4, Moves = new[]{ 217, 229, 098, 420 }, Index = 3 }, // Delibird
            new(30,03,2) { Species = 143, Ability = A4, Moves = new[]{ 034, 242, 118, 111 }, Index = 3 }, // Snorlax
            new(30,03,2) { Species = 225, Ability = A4, Moves = new[]{ 217, 065, 034, 693 }, Index = 3 }, // Delibird
            new(40,05,3) { Species = 012, Ability = A4, Moves = new[]{ 676, 403, 202, 527 }, Index = 3, CanGigantamax = true }, // Butterfree
            new(40,05,3) { Species = 225, Ability = A4, Moves = new[]{ 217, 065, 280, 196 }, Index = 3 }, // Delibird
            new(40,05,3) { Species = 143, Ability = A4, Moves = new[]{ 034, 667, 242, 281 }, Index = 3, CanGigantamax = true }, // Snorlax
            new(50,08,4) { Species = 012, Ability = A4, Moves = new[]{ 405, 403, 527, 078 }, Index = 3, CanGigantamax = true }, // Butterfree
            new(50,08,4) { Species = 225, Ability = A4, Moves = new[]{ 217, 059, 034, 280 }, Index = 3 }, // Delibird
            new(50,08,4) { Species = 143, Ability = A4, Moves = new[]{ 034, 667, 280, 523 }, Index = 3, CanGigantamax = true }, // Snorlax
            new(70,10,5) { Species = 225, Ability = A4, Moves = new[]{ 217, 059, 065, 280 }, Index = 3 }, // Delibird
            new(70,10,5) { Species = 143, Ability = A4, Moves = new[]{ 034, 442, 242, 428 }, Index = 3, CanGigantamax = true }, // Snorlax

            new(17,01,1) { Species = 446, Ability = A4, Moves = new[]{ 033, 044, 122, 111 }, Index = 4 }, // Munchlax
            new(17,01,1) { Species = 225, Ability = A4, Moves = new[]{ 217, 229, 098, 420 }, Index = 4 }, // Delibird
            new(30,03,2) { Species = 143, Ability = A4, Moves = new[]{ 034, 242, 118, 111 }, Index = 4 }, // Snorlax
            new(30,03,2) { Species = 225, Ability = A4, Moves = new[]{ 217, 065, 034, 693 }, Index = 4 }, // Delibird
            new(40,05,3) { Species = 012, Ability = A4, Moves = new[]{ 676, 403, 202, 527 }, Index = 4, CanGigantamax = true }, // Butterfree
            new(40,05,3) { Species = 225, Ability = A4, Moves = new[]{ 217, 065, 280, 196 }, Index = 4 }, // Delibird
            new(40,05,3) { Species = 143, Ability = A4, Moves = new[]{ 034, 667, 242, 281 }, Index = 4, CanGigantamax = true }, // Snorlax
            new(50,08,4) { Species = 012, Ability = A4, Moves = new[]{ 405, 403, 527, 078 }, Index = 4, CanGigantamax = true }, // Butterfree
            new(50,08,4) { Species = 225, Ability = A4, Moves = new[]{ 217, 059, 034, 280 }, Index = 4 }, // Delibird
            new(50,08,4) { Species = 143, Ability = A4, Moves = new[]{ 034, 667, 280, 523 }, Index = 4, CanGigantamax = true }, // Snorlax
            new(70,10,5) { Species = 225, Ability = A4, Moves = new[]{ 217, 059, 065, 280 }, Index = 4 }, // Delibird
            new(70,10,5) { Species = 143, Ability = A4, Moves = new[]{ 034, 442, 242, 428 }, Index = 4, CanGigantamax = true }, // Snorlax

            new(17,01,1) { Species = 446, Ability = A4, Moves = new[]{ 033, 044, 122, 111 }, Index = 5 }, // Munchlax
            new(17,01,1) { Species = 446, Ability = A2, Moves = new[]{ 033, 044, 122, 111 }, Index = 5 }, // Munchlax
            new(30,03,2) { Species = 143, Ability = A4, Moves = new[]{ 034, 242, 118, 111 }, Index = 5 }, // Snorlax
            new(30,03,2) { Species = 143, Ability = A2, Moves = new[]{ 034, 242, 118, 111 }, Index = 5 }, // Snorlax
            new(40,05,3) { Species = 012, Ability = A4, Moves = new[]{ 676, 403, 202, 527 }, Index = 5, CanGigantamax = true }, // Butterfree
            new(40,05,3) { Species = 143, Ability = A4, Moves = new[]{ 034, 667, 242, 281 }, Index = 5 }, // Snorlax
            new(40,05,3) { Species = 143, Ability = A4, Moves = new[]{ 034, 667, 242, 281 }, Index = 5, CanGigantamax = true }, // Snorlax
            new(50,08,4) { Species = 012, Ability = A4, Moves = new[]{ 405, 403, 527, 078 }, Index = 5, CanGigantamax = true }, // Butterfree
            new(50,08,4) { Species = 143, Ability = A4, Moves = new[]{ 034, 667, 280, 523 }, Index = 5 }, // Snorlax
            new(50,08,4) { Species = 143, Ability = A4, Moves = new[]{ 034, 667, 280, 523 }, Index = 5, CanGigantamax = true }, // Snorlax
            new(60,10,5) { Species = 143, Ability = A4, Moves = new[]{ 034, 442, 242, 428 }, Index = 5 }, // Snorlax
            new(70,10,5) { Species = 143, Ability = A4, Moves = new[]{ 034, 442, 242, 428 }, Index = 5, CanGigantamax = true }, // Snorlax

            new(17,01,1) { Species = 129, Ability = A4, Moves = new[]{ 150, 033, 000, 000 }, Index = 6, Shiny = Shiny.Always }, // Magikarp
            new(17,01,1) { Species = 446, Ability = A4, Moves = new[]{ 033, 044, 122, 111 }, Index = 6 }, // Munchlax
            new(17,01,1) { Species = 129, Ability = A4, Moves = new[]{ 150, 033, 000, 000 }, Index = 6 }, // Magikarp
            new(30,03,2) { Species = 129, Ability = A4, Moves = new[]{ 150, 033, 175, 000 }, Index = 6, Shiny = Shiny.Always }, // Magikarp
            new(30,03,2) { Species = 143, Ability = A4, Moves = new[]{ 034, 242, 118, 111 }, Index = 6 }, // Snorlax
            new(30,03,2) { Species = 129, Ability = A4, Moves = new[]{ 150, 033, 175, 000 }, Index = 6 }, // Magikarp
            new(40,05,3) { Species = 012, Ability = A4, Moves = new[]{ 676, 403, 202, 527 }, Index = 6, CanGigantamax = true }, // Butterfree
            new(40,05,3) { Species = 129, Ability = A4, Moves = new[]{ 150, 033, 175, 000 }, Index = 6, Shiny = Shiny.Always }, // Magikarp
            new(40,05,3) { Species = 129, Ability = A4, Moves = new[]{ 150, 033, 175, 000 }, Index = 6 }, // Magikarp
            new(40,05,3) { Species = 143, Ability = A4, Moves = new[]{ 034, 667, 242, 281 }, Index = 6, CanGigantamax = true }, // Snorlax
            new(50,08,4) { Species = 012, Ability = A4, Moves = new[]{ 405, 403, 527, 078 }, Index = 6, CanGigantamax = true }, // Butterfree
            new(50,08,4) { Species = 129, Ability = A4, Moves = new[]{ 150, 033, 175, 000 }, Index = 6, Shiny = Shiny.Always }, // Magikarp
            new(50,08,4) { Species = 129, Ability = A4, Moves = new[]{ 150, 033, 175, 000 }, Index = 6 }, // Magikarp
            new(50,08,4) { Species = 143, Ability = A4, Moves = new[]{ 034, 667, 280, 523 }, Index = 6, CanGigantamax = true }, // Snorlax
            new(60,10,5) { Species = 129, Ability = A4, Moves = new[]{ 150, 033, 175, 340 }, Index = 6, Shiny = Shiny.Always }, // Magikarp
            new(70,10,5) { Species = 129, Ability = A4, Moves = new[]{ 150, 033, 175, 340 }, Index = 6 }, // Magikarp
            new(70,10,5) { Species = 143, Ability = A4, Moves = new[]{ 034, 442, 242, 428 }, Index = 6, CanGigantamax = true }, // Snorlax

            new(17,01,1) { Species = 446, Ability = A4, Moves = new[]{ 033, 044, 122, 111 }, Index = 7 }, // Munchlax
            new(17,01,1) { Species = 446, Ability = A2, Moves = new[]{ 033, 044, 122, 111 }, Index = 7 }, // Munchlax
            new(30,03,2) { Species = 143, Ability = A4, Moves = new[]{ 034, 242, 118, 111 }, Index = 7 }, // Snorlax
            new(30,03,2) { Species = 143, Ability = A2, Moves = new[]{ 034, 242, 118, 111 }, Index = 7 }, // Snorlax
            new(40,05,3) { Species = 012, Ability = A4, Moves = new[]{ 676, 403, 202, 527 }, Index = 7, CanGigantamax = true }, // Butterfree
            new(40,05,3) { Species = 143, Ability = A4, Moves = new[]{ 034, 667, 242, 281 }, Index = 7 }, // Snorlax
            new(40,05,3) { Species = 143, Ability = A4, Moves = new[]{ 034, 667, 242, 281 }, Index = 7, CanGigantamax = true }, // Snorlax
            new(50,08,4) { Species = 012, Ability = A4, Moves = new[]{ 405, 403, 527, 078 }, Index = 7, CanGigantamax = true }, // Butterfree
            new(50,08,4) { Species = 143, Ability = A4, Moves = new[]{ 034, 667, 280, 523 }, Index = 7 }, // Snorlax
            new(50,08,4) { Species = 143, Ability = A4, Moves = new[]{ 034, 667, 280, 523 }, Index = 7, CanGigantamax = true }, // Snorlax
            new(60,10,5) { Species = 143, Ability = A4, Moves = new[]{ 034, 442, 242, 428 }, Index = 7 }, // Snorlax
            new(70,10,5) { Species = 143, Ability = A4, Moves = new[]{ 034, 442, 242, 428 }, Index = 7, CanGigantamax = true }, // Snorlax

            new(17,01,1) { Species = 840, Ability = A4, Moves = new[]{ 110, 310, 389, 213 }, Index = 8 }, // Applin
            new(17,01,1) { Species = 840, Ability = A2, Moves = new[]{ 110, 310, 389, 213 }, Index = 8 }, // Applin
            new(17,01,1) { Species = 868, Ability = A4, Moves = new[]{ 577, 033, 186, 263 }, Index = 8 }, // Milcery
            new(17,01,1) { Species = 868, Ability = A2, Moves = new[]{ 577, 033, 186, 263 }, Index = 8 }, // Milcery
            new(30,03,2) { Species = 869, Ability = A4, Moves = new[]{ 577, 213, 033, 186 }, Index = 8, CanGigantamax = true }, // Alcremie

            new(17,01,1) { Species = 868, Ability = A4, Moves = new[]{ 033, 186, 577, 496 }, Index = 9, CanGigantamax = true }, // Milcery
            new(30,03,2) { Species = 868, Ability = A4, Moves = new[]{ 577, 186, 263, 500 }, Index = 9, CanGigantamax = true }, // Milcery
            new(40,05,3) { Species = 868, Ability = A4, Moves = new[]{ 577, 605, 496, 213 }, Index = 9, CanGigantamax = true }, // Milcery
            new(50,08,4) { Species = 868, Ability = A4, Moves = new[]{ 577, 605, 496, 500 }, Index = 9, CanGigantamax = true }, // Milcery
            new(60,10,5) { Species = 868, Ability = A4, Moves = new[]{ 577, 605, 496, 500 }, Index = 9, CanGigantamax = true }, // Milcery

            new(17,01,1) { Species = 868, Ability = A4, Moves = new[]{ 033, 186, 577, 496 }, Index = 10, CanGigantamax = true }, // Milcery
            new(30,03,2) { Species = 868, Ability = A4, Moves = new[]{ 577, 186, 263, 500 }, Index = 10, CanGigantamax = true }, // Milcery
            new(40,05,3) { Species = 868, Ability = A4, Moves = new[]{ 577, 605, 496, 213 }, Index = 10, CanGigantamax = true }, // Milcery
            new(50,08,4) { Species = 868, Ability = A4, Moves = new[]{ 577, 605, 496, 500 }, Index = 10, CanGigantamax = true }, // Milcery
            new(60,10,5) { Species = 868, Ability = A4, Moves = new[]{ 577, 605, 496, 500 }, Index = 10, CanGigantamax = true }, // Milcery

            new(17,01,1) { Species = 848, Ability = A4, Moves = new[]{ 609, 051, 496, 715 }, Index = 11 }, // Toxel

            new(17,01,1) { Species = 001, Ability = A4, Moves = new[]{ 033, 045, 022, 074 }, Index = 12 }, // Bulbasaur
            new(17,01,1) { Species = 004, Ability = A4, Moves = new[]{ 010, 045, 052, 108 }, Index = 12 }, // Charmander
            new(17,01,1) { Species = 007, Ability = A4, Moves = new[]{ 033, 039, 055, 110 }, Index = 12 }, // Squirtle
            new(17,01,1) { Species = 848, Ability = A4, Moves = new[]{ 609, 051, 496, 715 }, Index = 12 }, // Toxel
            new(30,03,2) { Species = 001, Ability = A4, Moves = new[]{ 033, 022, 073, 075 }, Index = 12 }, // Bulbasaur
            new(30,03,2) { Species = 004, Ability = A4, Moves = new[]{ 010, 052, 225, 424 }, Index = 12 }, // Charmander
            new(30,03,2) { Species = 007, Ability = A4, Moves = new[]{ 033, 055, 044, 352 }, Index = 12 }, // Squirtle
            new(40,05,3) { Species = 001, Ability = A4, Moves = new[]{ 073, 075, 077, 402 }, Index = 12 }, // Bulbasaur
            new(40,05,3) { Species = 004, Ability = A4, Moves = new[]{ 424, 225, 163, 108 }, Index = 12 }, // Charmander
            new(40,05,3) { Species = 007, Ability = A4, Moves = new[]{ 055, 229, 044, 352 }, Index = 12 }, // Squirtle
            new(50,08,4) { Species = 002, Ability = A4, Moves = new[]{ 188, 412, 075, 034 }, Index = 12 }, // Ivysaur
            new(50,08,4) { Species = 005, Ability = A4, Moves = new[]{ 257, 242, 009, 053 }, Index = 12 }, // Charmeleon
            new(50,08,4) { Species = 008, Ability = A4, Moves = new[]{ 330, 396, 503, 428 }, Index = 12 }, // Wartortle
          //new(100,10,6) { Species = 150, Ability = A0, Moves = new[]{ 540, 053, 396, 059 }, Index = 12, Shiny = Shiny.Never }, // Mewtwo
          //new(100,10,6) { Species = 150, Ability = A0, Moves = new[]{ 428, 007, 089, 280 }, Index = 12, Shiny = Shiny.Never }, // Mewtwo
          //new(100,10,6) { Species = 150, Ability = A0, Moves = new[]{ 540, 126, 411, 059 }, Index = 12, Shiny = Shiny.Never }, // Mewtwo

            new(17,01,1) { Species = 848, Ability = A4, Moves = new[]{ 609, 051, 496, 715 }, Index = 13 }, // Toxel

            new(17,01,1) { Species = 446, Ability = A4, Moves = new[]{ 033, 044, 122, 111 }, Index = 14 }, // Munchlax
            new(17,01,1) { Species = 092, Ability = A4, Moves = new[]{ 122, 109, 095, 371 }, Index = 14 }, // Gastly
            new(17,01,1) { Species = 066, Ability = A4, Moves = new[]{ 067, 043, 116, 279 }, Index = 14 }, // Machop
            new(30,03,2) { Species = 143, Ability = A4, Moves = new[]{ 034, 242, 118, 111 }, Index = 14 }, // Snorlax
            new(30,03,2) { Species = 093, Ability = A4, Moves = new[]{ 325, 095, 122, 101 }, Index = 14 }, // Haunter
            new(30,03,2) { Species = 067, Ability = A4, Moves = new[]{ 067, 490, 282, 233 }, Index = 14 }, // Machoke
            new(40,05,3) { Species = 143, Ability = A4, Moves = new[]{ 034, 667, 242, 281 }, Index = 14 }, // Snorlax
            new(40,05,3) { Species = 143, Ability = A4, Moves = new[]{ 034, 667, 242, 281 }, Index = 14, CanGigantamax = true }, // Snorlax
            new(40,05,3) { Species = 094, Ability = A4, Moves = new[]{ 506, 188, 085, 261 }, Index = 14 }, // Gengar
            new(40,05,3) { Species = 094, Ability = A4, Moves = new[]{ 506, 188, 085, 261 }, Index = 14, CanGigantamax = true }, // Gengar
            new(40,05,3) { Species = 068, Ability = A4, Moves = new[]{ 279, 667, 008, 157 }, Index = 14 }, // Machamp
            new(40,05,3) { Species = 068, Ability = A4, Moves = new[]{ 279, 667, 008, 157 }, Index = 14, CanGigantamax = true }, // Machamp
            new(50,08,4) { Species = 143, Ability = A4, Moves = new[]{ 034, 667, 280, 523 }, Index = 14 }, // Snorlax
            new(50,08,4) { Species = 143, Ability = A4, Moves = new[]{ 034, 667, 280, 523 }, Index = 14, CanGigantamax = true }, // Snorlax
            new(50,08,4) { Species = 094, Ability = A4, Moves = new[]{ 247, 482, 411, 605 }, Index = 14 }, // Gengar
            new(50,08,4) { Species = 094, Ability = A4, Moves = new[]{ 247, 482, 411, 605 }, Index = 14, CanGigantamax = true }, // Gengar
            new(50,08,4) { Species = 068, Ability = A4, Moves = new[]{ 280, 444, 371, 523 }, Index = 14 }, // Machamp
            new(50,08,4) { Species = 068, Ability = A4, Moves = new[]{ 280, 444, 371, 523 }, Index = 14, CanGigantamax = true }, // Machamp
            new(60,10,5) { Species = 143, Ability = A4, Moves = new[]{ 034, 442, 242, 428 }, Index = 14 }, // Snorlax
            new(60,10,5) { Species = 143, Ability = A4, Moves = new[]{ 034, 442, 242, 428 }, Index = 14, CanGigantamax = true }, // Snorlax

            new(17,01,1) { Species = 588, Ability = A4, Moves = new[]{ 064, 043, 210, 491 }, Index = 15 }, // Karrablast
            new(17,01,1) { Species = 616, Ability = A4, Moves = new[]{ 071, 051, 174, 522 }, Index = 15 }, // Shelmet
            new(17,01,1) { Species = 092, Ability = A4, Moves = new[]{ 122, 109, 095, 371 }, Index = 15 }, // Gastly
            new(17,01,1) { Species = 871, Ability = A4, Moves = new[]{ 084, 064, 055, 031 }, Index = 15 }, // Pincurchin
            new(17,01,1) { Species = 066, Ability = A4, Moves = new[]{ 067, 043, 116, 279 }, Index = 15 }, // Machop
            new(30,03,2) { Species = 143, Ability = A4, Moves = new[]{ 034, 242, 118, 111 }, Index = 15 }, // Snorlax
            new(30,03,2) { Species = 093, Ability = A4, Moves = new[]{ 325, 095, 122, 101 }, Index = 15 }, // Haunter
            new(30,03,2) { Species = 871, Ability = A4, Moves = new[]{ 209, 061, 086, 506 }, Index = 15 }, // Pincurchin
            new(30,03,2) { Species = 067, Ability = A4, Moves = new[]{ 067, 490, 282, 233 }, Index = 15 }, // Machoke
            new(40,05,3) { Species = 143, Ability = A4, Moves = new[]{ 034, 667, 242, 281 }, Index = 15, CanGigantamax = true }, // Snorlax
            new(40,05,3) { Species = 094, Ability = A4, Moves = new[]{ 506, 188, 085, 261 }, Index = 15, CanGigantamax = true }, // Gengar
            new(40,05,3) { Species = 871, Ability = A2, Moves = new[]{ 085, 503, 398, 716 }, Index = 15 }, // Pincurchin
            new(40,05,3) { Species = 068, Ability = A4, Moves = new[]{ 279, 667, 008, 157 }, Index = 15, CanGigantamax = true }, // Machamp
            new(50,08,4) { Species = 617, Ability = A2, Moves = new[]{ 405, 522, 188, 202 }, Index = 15 }, // Accelgor
            new(50,08,4) { Species = 143, Ability = A4, Moves = new[]{ 034, 667, 280, 523 }, Index = 15, CanGigantamax = true }, // Snorlax
            new(50,08,4) { Species = 589, Ability = A2, Moves = new[]{ 442, 224, 529, 398 }, Index = 15 }, // Escavalier
            new(50,08,4) { Species = 094, Ability = A4, Moves = new[]{ 247, 482, 411, 605 }, Index = 15, CanGigantamax = true }, // Gengar
            new(50,08,4) { Species = 871, Ability = A2, Moves = new[]{ 435, 330, 474, 367 }, Index = 15 }, // Pincurchin
            new(50,08,4) { Species = 068, Ability = A4, Moves = new[]{ 280, 444, 371, 523 }, Index = 15, CanGigantamax = true }, // Machamp
            new(60,10,5) { Species = 143, Ability = A4, Moves = new[]{ 034, 442, 242, 428 }, Index = 15, CanGigantamax = true }, // Snorlax

            new(17,01,1) { Species = 878, Ability = A4, Moves = new[]{ 091, 249, 205, 523 }, Index = 16 }, // Cufant
            new(17,01,1) { Species = 568, Ability = A4, Moves = new[]{ 001, 499, 491, 133 }, Index = 16 }, // Trubbish
            new(17,01,1) { Species = 004, Ability = A4, Moves = new[]{ 424, 052, 108, 225 }, Index = 16 }, // Charmander
            new(17,01,1) { Species = 884, Ability = A4, Moves = new[]{ 468, 249, 043, 232 }, Index = 16 }, // Duraludon
            new(30,03,2) { Species = 878, Ability = A4, Moves = new[]{ 334, 091, 205, 523 }, Index = 16 }, // Cufant
            new(30,03,2) { Species = 568, Ability = A4, Moves = new[]{ 036, 499, 124, 133 }, Index = 16 }, // Trubbish
            new(30,03,2) { Species = 005, Ability = A4, Moves = new[]{ 053, 163, 108, 225 }, Index = 16 }, // Charmeleon
            new(30,03,2) { Species = 884, Ability = A4, Moves = new[]{ 468, 249, 784, 232 }, Index = 16 }, // Duraludon
            new(40,05,3) { Species = 879, Ability = A4, Moves = new[]{ 334, 070, 442, 523 }, Index = 16, CanGigantamax = true }, // Copperajah
            new(40,05,3) { Species = 569, Ability = A4, Moves = new[]{ 188, 499, 034, 707 }, Index = 16, CanGigantamax = true }, // Garbodor
            new(40,05,3) { Species = 006, Ability = A4, Moves = new[]{ 053, 403, 108, 225 }, Index = 16, CanGigantamax = true }, // Charizard
            new(40,05,3) { Species = 884, Ability = A4, Moves = new[]{ 442, 555, 784, 334 }, Index = 16, CanGigantamax = true }, // Duraludon
            new(50,08,4) { Species = 879, Ability = A4, Moves = new[]{ 667, 442, 438, 523 }, Index = 16, CanGigantamax = true }, // Copperajah
            new(50,08,4) { Species = 569, Ability = A4, Moves = new[]{ 441, 499, 402, 707 }, Index = 16, CanGigantamax = true }, // Garbodor
            new(50,08,4) { Species = 006, Ability = A4, Moves = new[]{ 053, 403, 076, 257 }, Index = 16, CanGigantamax = true }, // Charizard
            new(50,08,4) { Species = 884, Ability = A4, Moves = new[]{ 337, 430, 784, 776 }, Index = 16, CanGigantamax = true }, // Duraludon

            new(17,01,1) { Species = 878, Ability = A4, Moves = new[]{ 091, 249, 205, 523 }, Index = 17 }, // Cufant
            new(17,01,1) { Species = 568, Ability = A4, Moves = new[]{ 001, 499, 491, 133 }, Index = 17 }, // Trubbish
            new(17,01,1) { Species = 004, Ability = A4, Moves = new[]{ 424, 052, 108, 225 }, Index = 17 }, // Charmander
            new(17,01,1) { Species = 884, Ability = A4, Moves = new[]{ 468, 249, 043, 232 }, Index = 17 }, // Duraludon
            new(30,03,2) { Species = 878, Ability = A4, Moves = new[]{ 334, 091, 205, 523 }, Index = 17 }, // Cufant
            new(30,03,2) { Species = 568, Ability = A4, Moves = new[]{ 036, 499, 124, 133 }, Index = 17 }, // Trubbish
            new(30,03,2) { Species = 884, Ability = A4, Moves = new[]{ 468, 249, 784, 232 }, Index = 17 }, // Duraludon
            new(30,03,2) { Species = 005, Ability = A4, Moves = new[]{ 053, 163, 108, 225 }, Index = 17 }, // Charmeleon
            new(40,05,2) { Species = 848, Ability = A4, Moves = new[]{ 609, 051, 175, 715 }, Index = 17 }, // Toxel
            new(40,05,2) { Species = 458, Ability = A4, Moves = new[]{ 403, 061, 469, 503 }, Index = 17 }, // Mantyke
            new(40,05,3) { Species = 406, Ability = A4, Moves = new[]{ 071, 074, 078, 188 }, Index = 17 }, // Budew
            new(40,05,3) { Species = 236, Ability = A4, Moves = new[]{ 280, 157, 252, 116 }, Index = 17 }, // Tyrogue
            new(40,05,3) { Species = 438, Ability = A4, Moves = new[]{ 317, 389, 157, 313 }, Index = 17 }, // Bonsly
            new(40,05,3) { Species = 447, Ability = A4, Moves = new[]{ 014, 009, 232, 249 }, Index = 17 }, // Riolu
            new(40,05,3) { Species = 446, Ability = A4, Moves = new[]{ 034, 089, 044, 122 }, Index = 17 }, // Munchlax
            new(40,05,3) { Species = 439, Ability = A4, Moves = new[]{ 389, 060, 182, 085 }, Index = 17 }, // Mime Jr.
            new(50,08,4) { Species = 175, Ability = A4, Moves = new[]{ 605, 219, 246, 053 }, Index = 17 }, // Togepi
            new(50,08,4) { Species = 360, Ability = A4, Moves = new[]{ 068, 243, 204, 133 }, Index = 17 }, // Wynaut
            new(50,08,4) { Species = 173, Ability = A4, Moves = new[]{ 574, 005, 113, 034 }, Index = 17 }, // Cleffa
            new(50,08,4) { Species = 006, Ability = A4, Moves = new[]{ 053, 403, 076, 257 }, Index = 17, CanGigantamax = true }, // Charizard
            new(50,08,4) { Species = 172, Ability = A4, Moves = new[]{ 583, 417, 085, 186 }, Index = 17 }, // Pichu
            new(50,08,4) { Species = 884, Ability = A4, Moves = new[]{ 337, 430, 784, 776 }, Index = 17, CanGigantamax = true }, // Duraludon
            new(60,10,5) { Species = 132, Ability = A4, Moves = new[]{ 144, 000, 000, 000 }, Index = 17 }, // Ditto

            new(17,01,1) { Species = 878, Ability = A4, Moves = new[]{ 091, 249, 205, 523 }, Index = 18 }, // Cufant
            new(17,01,1) { Species = 568, Ability = A4, Moves = new[]{ 001, 499, 491, 133 }, Index = 18 }, // Trubbish
            new(17,01,1) { Species = 004, Ability = A4, Moves = new[]{ 424, 052, 108, 225 }, Index = 18 }, // Charmander
            new(17,01,1) { Species = 884, Ability = A4, Moves = new[]{ 468, 249, 043, 232 }, Index = 18 }, // Duraludon
            new(30,03,2) { Species = 878, Ability = A4, Moves = new[]{ 334, 091, 205, 523 }, Index = 18 }, // Cufant
            new(30,03,2) { Species = 568, Ability = A4, Moves = new[]{ 036, 499, 124, 133 }, Index = 18 }, // Trubbish
            new(30,03,2) { Species = 005, Ability = A4, Moves = new[]{ 053, 163, 108, 225 }, Index = 18 }, // Charmeleon
            new(30,03,2) { Species = 884, Ability = A4, Moves = new[]{ 468, 249, 784, 232 }, Index = 18 }, // Duraludon
            new(40,05,3) { Species = 879, Ability = A4, Moves = new[]{ 334, 070, 442, 523 }, Index = 18, CanGigantamax = true }, // Copperajah
            new(40,05,3) { Species = 569, Ability = A4, Moves = new[]{ 188, 499, 034, 707 }, Index = 18, CanGigantamax = true }, // Garbodor
            new(40,05,3) { Species = 006, Ability = A4, Moves = new[]{ 053, 403, 108, 225 }, Index = 18, CanGigantamax = true }, // Charizard
            new(40,05,3) { Species = 884, Ability = A4, Moves = new[]{ 442, 555, 784, 334 }, Index = 18, CanGigantamax = true }, // Duraludon
            new(50,08,4) { Species = 879, Ability = A4, Moves = new[]{ 667, 442, 438, 523 }, Index = 18, CanGigantamax = true }, // Copperajah
            new(50,08,4) { Species = 569, Ability = A4, Moves = new[]{ 441, 499, 402, 707 }, Index = 18, CanGigantamax = true }, // Garbodor
            new(50,08,4) { Species = 006, Ability = A4, Moves = new[]{ 053, 403, 076, 257 }, Index = 18, CanGigantamax = true }, // Charizard
            new(50,08,4) { Species = 884, Ability = A4, Moves = new[]{ 337, 430, 784, 776 }, Index = 18, CanGigantamax = true }, // Duraludon

            new(17,01,1) { Species = 246, Ability = A4, Moves = new[]{ 157, 044, 184, 033 }, Index = 19 }, // Larvitar
            new(17,01,1) { Species = 546, Ability = A4, Moves = new[]{ 584, 078, 075, 071 }, Index = 19 }, // Cottonee
            new(17,01,1) { Species = 885, Ability = A4, Moves = new[]{ 611, 098, 310, 044 }, Index = 19 }, // Dreepy
            new(17,01,1) { Species = 175, Ability = A4, Moves = new[]{ 204, 577, 113, 791 }, Index = 19 }, // Togepi
            new(30,03,2) { Species = 247, Ability = A4, Moves = new[]{ 157, 242, 334, 707 }, Index = 19 }, // Pupitar
            new(30,03,2) { Species = 529, Ability = A4, Moves = new[]{ 189, 232, 157, 306 }, Index = 19 }, // Drilbur
            new(30,03,2) { Species = 546, Ability = A4, Moves = new[]{ 202, 075, 204, 077 }, Index = 19 }, // Cottonee
            new(30,03,2) { Species = 886, Ability = A4, Moves = new[]{ 097, 506, 372, 458 }, Index = 19 }, // Drakloak
            new(30,03,2) { Species = 176, Ability = A4, Moves = new[]{ 584, 038, 113, 791 }, Index = 19 }, // Togetic
            new(40,05,3) { Species = 248, Ability = A4, Moves = new[]{ 444, 242, 334, 089 }, Index = 19 }, // Tyranitar
            new(40,05,3) { Species = 530, Ability = A4, Moves = new[]{ 529, 232, 157, 032 }, Index = 19 }, // Excadrill
            new(40,05,3) { Species = 547, Ability = A4, Moves = new[]{ 283, 585, 077, 366 }, Index = 19 }, // Whimsicott
            new(40,05,3) { Species = 887, Ability = A4, Moves = new[]{ 751, 506, 349, 458 }, Index = 19 }, // Dragapult
            new(40,05,3) { Species = 468, Ability = A4, Moves = new[]{ 605, 038, 246, 403 }, Index = 19 }, // Togekiss
            new(50,08,4) { Species = 248, Ability = A4, Moves = new[]{ 444, 242, 442, 089 }, Index = 19 }, // Tyranitar
            new(50,08,4) { Species = 530, Ability = A4, Moves = new[]{ 529, 442, 157, 032 }, Index = 19 }, // Excadrill
            new(50,08,4) { Species = 547, Ability = A4, Moves = new[]{ 283, 585, 073, 366 }, Index = 19 }, // Whimsicott
            new(50,08,4) { Species = 887, Ability = A4, Moves = new[]{ 751, 506, 349, 211 }, Index = 19 }, // Dragapult
            new(50,08,4) { Species = 468, Ability = A4, Moves = new[]{ 605, 219, 246, 403 }, Index = 19 }, // Togekiss
            new(60,10,5) { Species = 248, Ability = A4, Moves = new[]{ 444, 242, 442, 276 }, Index = 19 }, // Tyranitar
            new(60,10,5) { Species = 530, Ability = A4, Moves = new[]{ 089, 442, 157, 032 }, Index = 19 }, // Excadrill
            new(60,10,5) { Species = 547, Ability = A4, Moves = new[]{ 538, 585, 073, 366 }, Index = 19 }, // Whimsicott
            new(60,10,5) { Species = 887, Ability = A4, Moves = new[]{ 751, 566, 349, 211 }, Index = 19 }, // Dragapult
            new(60,10,5) { Species = 468, Ability = A4, Moves = new[]{ 605, 219, 246, 053 }, Index = 19 }, // Togekiss

            new(17,01,1) { Species = 246, Ability = A4, Moves = new[]{ 157, 044, 184, 033 }, Index = 20 }, // Larvitar
            new(17,01,1) { Species = 529, Ability = A4, Moves = new[]{ 189, 232, 010, 468 }, Index = 20 }, // Drilbur
            new(17,01,1) { Species = 546, Ability = A4, Moves = new[]{ 584, 078, 075, 071 }, Index = 20 }, // Cottonee
            new(17,01,1) { Species = 885, Ability = A4, Moves = new[]{ 611, 098, 310, 044 }, Index = 20 }, // Dreepy
            new(17,01,1) { Species = 175, Ability = A4, Moves = new[]{ 204, 577, 113, 791 }, Index = 20 }, // Togepi
            new(30,03,2) { Species = 247, Ability = A4, Moves = new[]{ 157, 242, 334, 707 }, Index = 20 }, // Pupitar
            new(30,03,2) { Species = 529, Ability = A4, Moves = new[]{ 189, 232, 157, 306 }, Index = 20 }, // Drilbur
            new(30,03,2) { Species = 546, Ability = A4, Moves = new[]{ 202, 075, 204, 077 }, Index = 20 }, // Cottonee
            new(30,03,2) { Species = 886, Ability = A4, Moves = new[]{ 097, 506, 372, 458 }, Index = 20 }, // Drakloak
            new(30,03,2) { Species = 176, Ability = A4, Moves = new[]{ 584, 038, 113, 791 }, Index = 20 }, // Togetic
            new(40,05,3) { Species = 248, Ability = A4, Moves = new[]{ 444, 242, 334, 089 }, Index = 20 }, // Tyranitar
            new(40,05,3) { Species = 530, Ability = A4, Moves = new[]{ 529, 232, 157, 032 }, Index = 20 }, // Excadrill
            new(40,05,3) { Species = 547, Ability = A4, Moves = new[]{ 283, 585, 077, 366 }, Index = 20 }, // Whimsicott
            new(40,05,3) { Species = 887, Ability = A4, Moves = new[]{ 751, 506, 349, 458 }, Index = 20 }, // Dragapult
            new(40,05,3) { Species = 468, Ability = A4, Moves = new[]{ 605, 038, 246, 403 }, Index = 20 }, // Togekiss
            new(50,08,4) { Species = 248, Ability = A4, Moves = new[]{ 444, 242, 442, 089 }, Index = 20 }, // Tyranitar
            new(50,08,4) { Species = 530, Ability = A4, Moves = new[]{ 529, 442, 157, 032 }, Index = 20 }, // Excadrill
            new(50,08,4) { Species = 547, Ability = A4, Moves = new[]{ 283, 585, 073, 366 }, Index = 20 }, // Whimsicott
            new(50,08,4) { Species = 887, Ability = A4, Moves = new[]{ 751, 506, 349, 211 }, Index = 20 }, // Dragapult
            new(50,08,4) { Species = 468, Ability = A4, Moves = new[]{ 605, 219, 246, 403 }, Index = 20 }, // Togekiss
            new(60,10,5) { Species = 248, Ability = A4, Moves = new[]{ 444, 242, 442, 276 }, Index = 20 }, // Tyranitar
            new(60,10,5) { Species = 530, Ability = A4, Moves = new[]{ 089, 442, 157, 032 }, Index = 20 }, // Excadrill
            new(60,10,5) { Species = 547, Ability = A4, Moves = new[]{ 538, 585, 073, 366 }, Index = 20 }, // Whimsicott
            new(60,10,5) { Species = 887, Ability = A4, Moves = new[]{ 751, 566, 349, 211 }, Index = 20 }, // Dragapult
            new(60,10,5) { Species = 468, Ability = A4, Moves = new[]{ 605, 219, 246, 053 }, Index = 20 }, // Togekiss

            new(17,01,1) { Species = 025, Ability = A4, Moves = new[]{ 084, 104, 486, 364 }, Index = 21, CanGigantamax = true }, // Pikachu
            new(30,03,2) { Species = 025, Ability = A4, Moves = new[]{ 021, 209, 097, 364 }, Index = 21, CanGigantamax = true }, // Pikachu
            new(40,05,3) { Species = 025, Ability = A4, Moves = new[]{ 021, 113, 085, 364 }, Index = 21, CanGigantamax = true }, // Pikachu
            new(50,08,4) { Species = 025, Ability = A4, Moves = new[]{ 087, 113, 085, 364 }, Index = 21, CanGigantamax = true }, // Pikachu
            new(50,08,4) { Species = 025, Ability = A4, Moves = new[]{ 087, 113, 057, 085 }, Index = 21, CanGigantamax = true }, // Pikachu
            new(60,10,5) { Species = 025, Ability = A4, Moves = new[]{ 087, 113, 057, 364 }, Index = 21, CanGigantamax = true }, // Pikachu
            new(60,10,5) { Species = 025, Ability = A4, Moves = new[]{ 087, 113, 057, 344 }, Index = 21, CanGigantamax = true }, // Pikachu

            new(17,01,1) { Species = 133, Ability = A4, Moves = new[]{ 098, 270, 608, 028 }, Index = 22, CanGigantamax = true }, // Eevee
            new(30,03,2) { Species = 133, Ability = A4, Moves = new[]{ 129, 270, 608, 044 }, Index = 22, CanGigantamax = true }, // Eevee
            new(40,05,3) { Species = 133, Ability = A4, Moves = new[]{ 036, 270, 608, 044 }, Index = 22, CanGigantamax = true }, // Eevee
            new(40,05,3) { Species = 133, Ability = A4, Moves = new[]{ 036, 270, 608, 231 }, Index = 22, CanGigantamax = true }, // Eevee
            new(50,08,4) { Species = 133, Ability = A4, Moves = new[]{ 038, 270, 204, 044 }, Index = 22, CanGigantamax = true }, // Eevee
            new(50,08,4) { Species = 133, Ability = A4, Moves = new[]{ 038, 203, 204, 231 }, Index = 22, CanGigantamax = true }, // Eevee
            new(60,10,5) { Species = 133, Ability = A4, Moves = new[]{ 038, 270, 204, 231 }, Index = 22, CanGigantamax = true }, // Eevee
            new(60,10,5) { Species = 133, Ability = A4, Moves = new[]{ 387, 203, 204, 231 }, Index = 22, CanGigantamax = true }, // Eevee

            new(17,01,1) { Species = 052, Ability = A4, Moves = new[]{ 252, 044, 010, 364 }, Index = 23, CanGigantamax = true }, // Meowth
            new(17,01,1) { Species = 052, Ability = A4, Moves = new[]{ 006, 044, 010, 364 }, Index = 23, CanGigantamax = true }, // Meowth
            new(30,03,2) { Species = 052, Ability = A4, Moves = new[]{ 252, 044, 269, 154 }, Index = 23, CanGigantamax = true }, // Meowth
            new(30,03,2) { Species = 052, Ability = A4, Moves = new[]{ 006, 044, 269, 154 }, Index = 23, CanGigantamax = true }, // Meowth
            new(40,05,3) { Species = 052, Ability = A4, Moves = new[]{ 252, 044, 417, 163 }, Index = 23, CanGigantamax = true }, // Meowth
            new(40,05,3) { Species = 052, Ability = A4, Moves = new[]{ 006, 044, 417, 163 }, Index = 23, CanGigantamax = true }, // Meowth
            new(50,08,4) { Species = 052, Ability = A4, Moves = new[]{ 252, 583, 417, 163 }, Index = 23, CanGigantamax = true }, // Meowth
            new(50,08,4) { Species = 052, Ability = A4, Moves = new[]{ 006, 583, 417, 163 }, Index = 23, CanGigantamax = true }, // Meowth
            new(60,10,5) { Species = 052, Ability = A4, Moves = new[]{ 252, 583, 417, 034 }, Index = 23, CanGigantamax = true }, // Meowth
            new(60,10,5) { Species = 052, Ability = A4, Moves = new[]{ 006, 583, 417, 034 }, Index = 23, CanGigantamax = true }, // Meowth

            new(30,03,2) { Species = 143, Ability = A4, Moves = new[]{ 034, 044, 280, 523 }, Index = 24, CanGigantamax = true }, // Snorlax
            new(60,10,5) { Species = 143, Ability = A4, Moves = new[]{ 034, 442, 242, 428 }, Index = 24, CanGigantamax = true }, // Snorlax

            new(17,01,1) { Species = 143, Ability = A4, Moves = new[]{ 033, 044, 122, 111 }, Index = 25, CanGigantamax = true }, // Snorlax
          //new(40,05,3) { Species = 807, Ability = A0, Moves = new[]{ 085, 007, 512, 280 }, Index = 25, Shiny = Shiny.Never }, // Zeraora
          //new(50,08,4) { Species = 807, Ability = A0, Moves = new[]{ 085, 007, 200, 370 }, Index = 25, Shiny = Shiny.Never }, // Zeraora
          //new(60,10,5) { Species = 807, Ability = A0, Moves = new[]{ 009, 299, 200, 370 }, Index = 25, Shiny = Shiny.Never }, // Zeraora
          //new(100,10,6) { Species = 807, Ability = A0, Moves = new[]{ 435, 299, 200, 370 }, Index = 25, Shiny = Shiny.Always }, // Zeraora
            new(60,10,5) { Species = 143, Ability = A4, Moves = new[]{ 034, 442, 242, 428 }, Index = 25, CanGigantamax = true }, // Snorlax

            new(30,03,2) { Species = 143, Ability = A4, Moves = new[]{ 034, 044, 280, 523 }, Index = 26, CanGigantamax = true }, // Snorlax
            new(60,10,5) { Species = 143, Ability = A4, Moves = new[]{ 034, 442, 242, 428 }, Index = 26, CanGigantamax = true }, // Snorlax

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

            new(17,01,1) { Species = 878, Ability = A4, Moves = new[]{ 523, 205, 045, 249 }, Index = 28 }, // Cufant
            new(17,01,1) { Species = 208, Ability = A4, Moves = new[]{ 242, 442, 106, 422 }, Index = 28 }, // Steelix
            new(17,01,1) { Species = 052, Ability = A4, Moves = new[]{ 232, 006, 242, 045 }, Index = 28, Form = 2 }, // Meowth-2
            new(17,01,1) { Species = 837, Ability = A4, Moves = new[]{ 229, 261, 479, 108 }, Index = 28 }, // Rolycoly
            new(17,01,1) { Species = 111, Ability = A4, Moves = new[]{ 479, 523, 196, 182 }, Index = 28 }, // Rhyhorn
            new(17,01,1) { Species = 095, Ability = A4, Moves = new[]{ 174, 225, 034, 106 }, Index = 28 }, // Onix
            new(30,03,2) { Species = 878, Ability = A4, Moves = new[]{ 523, 023, 334, 249 }, Index = 28 }, // Cufant
            new(30,03,2) { Species = 208, Ability = A4, Moves = new[]{ 157, 442, 328, 422 }, Index = 28 }, // Steelix
            new(30,03,2) { Species = 863, Ability = A4, Moves = new[]{ 442, 006, 242, 269 }, Index = 28 }, // Perrserker
            new(30,03,2) { Species = 838, Ability = A4, Moves = new[]{ 229, 488, 157, 108 }, Index = 28 }, // Carkol
            new(30,03,2) { Species = 111, Ability = A4, Moves = new[]{ 350, 523, 196, 182 }, Index = 28 }, // Rhyhorn
            new(30,03,2) { Species = 095, Ability = A4, Moves = new[]{ 776, 225, 034, 106 }, Index = 28 }, // Onix
            new(40,05,3) { Species = 879, Ability = A4, Moves = new[]{ 070, 523, 334, 442 }, Index = 28, CanGigantamax = true }, // Copperajah
            new(40,05,3) { Species = 208, Ability = A4, Moves = new[]{ 157, 442, 328, 422 }, Index = 28 }, // Steelix
            new(40,05,3) { Species = 863, Ability = A4, Moves = new[]{ 442, 006, 154, 269 }, Index = 28 }, // Perrserker
            new(40,05,3) { Species = 839, Ability = A4, Moves = new[]{ 025, 488, 157, 108 }, Index = 28, CanGigantamax = true }, // Coalossal
            new(40,05,3) { Species = 112, Ability = A4, Moves = new[]{ 036, 529, 008, 182 }, Index = 28 }, // Rhydon
            new(40,05,3) { Species = 095, Ability = A4, Moves = new[]{ 776, 225, 021, 201 }, Index = 28 }, // Onix
            new(50,08,4) { Species = 879, Ability = A4, Moves = new[]{ 070, 523, 334, 442 }, Index = 28, CanGigantamax = true }, // Copperajah
            new(50,08,4) { Species = 208, Ability = A4, Moves = new[]{ 157, 231, 328, 422 }, Index = 28 }, // Steelix
            new(50,08,4) { Species = 863, Ability = A4, Moves = new[]{ 442, 583, 154, 269 }, Index = 28 }, // Perrserker
            new(50,08,4) { Species = 839, Ability = A4, Moves = new[]{ 025, 488, 157, 115 }, Index = 28, CanGigantamax = true }, // Coalossal
            new(50,08,4) { Species = 464, Ability = A4, Moves = new[]{ 350, 089, 008, 182 }, Index = 28 }, // Rhyperior
            new(50,08,4) { Species = 095, Ability = A4, Moves = new[]{ 776, 225, 784, 201 }, Index = 28 }, // Onix
            new(60,10,5) { Species = 879, Ability = A4, Moves = new[]{ 276, 089, 583, 442 }, Index = 28, CanGigantamax = true }, // Copperajah
            new(60,10,5) { Species = 208, Ability = A4, Moves = new[]{ 038, 231, 529, 422 }, Index = 28 }, // Steelix
            new(60,10,5) { Species = 863, Ability = A4, Moves = new[]{ 442, 583, 370, 269 }, Index = 28 }, // Perrserker
            new(60,10,5) { Species = 839, Ability = A4, Moves = new[]{ 076, 682, 157, 115 }, Index = 28, CanGigantamax = true }, // Coalossal
            new(60,10,5) { Species = 464, Ability = A4, Moves = new[]{ 444, 089, 008, 224 }, Index = 28 }, // Rhyperior
            new(60,10,5) { Species = 095, Ability = A4, Moves = new[]{ 776, 444, 784, 201 }, Index = 28 }, // Onix

            new(17,01,1) { Species = 878, Ability = A4, Moves = new[]{ 523, 205, 045, 249 }, Index = 30 }, // Cufant
            new(17,01,1) { Species = 208, Ability = A4, Moves = new[]{ 242, 442, 106, 422 }, Index = 30 }, // Steelix
            new(17,01,1) { Species = 052, Ability = A4, Moves = new[]{ 232, 006, 242, 045 }, Index = 30, Form = 2 }, // Meowth-2
            new(17,01,1) { Species = 837, Ability = A4, Moves = new[]{ 229, 261, 479, 108 }, Index = 30 }, // Rolycoly
            new(17,01,1) { Species = 111, Ability = A4, Moves = new[]{ 479, 523, 196, 182 }, Index = 30 }, // Rhyhorn
            new(17,01,1) { Species = 095, Ability = A4, Moves = new[]{ 174, 225, 034, 106 }, Index = 30 }, // Onix
            new(30,03,2) { Species = 878, Ability = A4, Moves = new[]{ 523, 023, 334, 249 }, Index = 30 }, // Cufant
            new(30,03,2) { Species = 208, Ability = A4, Moves = new[]{ 157, 442, 328, 422 }, Index = 30 }, // Steelix
            new(30,03,2) { Species = 863, Ability = A4, Moves = new[]{ 442, 006, 242, 269 }, Index = 30 }, // Perrserker
            new(30,03,2) { Species = 838, Ability = A4, Moves = new[]{ 229, 488, 157, 108 }, Index = 30 }, // Carkol
            new(30,03,2) { Species = 111, Ability = A4, Moves = new[]{ 350, 523, 196, 182 }, Index = 30 }, // Rhyhorn
            new(30,03,2) { Species = 095, Ability = A4, Moves = new[]{ 776, 225, 034, 106 }, Index = 30 }, // Onix
            new(40,05,3) { Species = 879, Ability = A4, Moves = new[]{ 070, 523, 334, 442 }, Index = 30, CanGigantamax = true }, // Copperajah
            new(40,05,3) { Species = 208, Ability = A4, Moves = new[]{ 157, 442, 328, 422 }, Index = 30 }, // Steelix
            new(40,05,3) { Species = 863, Ability = A4, Moves = new[]{ 442, 006, 154, 269 }, Index = 30 }, // Perrserker
            new(40,05,3) { Species = 839, Ability = A4, Moves = new[]{ 025, 488, 157, 108 }, Index = 30, CanGigantamax = true }, // Coalossal
            new(40,05,3) { Species = 112, Ability = A4, Moves = new[]{ 036, 529, 008, 182 }, Index = 30 }, // Rhydon
            new(40,05,3) { Species = 095, Ability = A4, Moves = new[]{ 776, 225, 021, 201 }, Index = 30 }, // Onix
            new(50,08,4) { Species = 879, Ability = A4, Moves = new[]{ 070, 523, 334, 442 }, Index = 30, CanGigantamax = true }, // Copperajah
            new(50,08,4) { Species = 208, Ability = A4, Moves = new[]{ 157, 231, 328, 422 }, Index = 30 }, // Steelix
            new(50,08,4) { Species = 863, Ability = A4, Moves = new[]{ 442, 583, 154, 269 }, Index = 30 }, // Perrserker
            new(50,08,4) { Species = 839, Ability = A4, Moves = new[]{ 025, 488, 157, 115 }, Index = 30, CanGigantamax = true }, // Coalossal
            new(50,08,4) { Species = 464, Ability = A4, Moves = new[]{ 350, 089, 008, 182 }, Index = 30 }, // Rhyperior
            new(50,08,4) { Species = 095, Ability = A4, Moves = new[]{ 776, 225, 784, 201 }, Index = 30 }, // Onix
            new(60,10,5) { Species = 879, Ability = A4, Moves = new[]{ 276, 089, 583, 442 }, Index = 30, CanGigantamax = true }, // Copperajah
            new(60,10,5) { Species = 208, Ability = A4, Moves = new[]{ 038, 231, 529, 422 }, Index = 30 }, // Steelix
            new(60,10,5) { Species = 863, Ability = A4, Moves = new[]{ 442, 583, 370, 269 }, Index = 30 }, // Perrserker
            new(60,10,5) { Species = 839, Ability = A4, Moves = new[]{ 076, 682, 157, 115 }, Index = 30, CanGigantamax = true }, // Coalossal
            new(60,10,5) { Species = 464, Ability = A4, Moves = new[]{ 444, 089, 008, 224 }, Index = 30 }, // Rhyperior
            new(60,10,5) { Species = 095, Ability = A4, Moves = new[]{ 776, 444, 784, 201 }, Index = 30 }, // Onix

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

            new(17,01,1) { Species = 878, Ability = A4, Moves = new[]{ 523, 205, 045, 249 }, Index = 32 }, // Cufant
            new(17,01,1) { Species = 208, Ability = A4, Moves = new[]{ 242, 442, 106, 422 }, Index = 32 }, // Steelix
            new(17,01,1) { Species = 052, Ability = A4, Moves = new[]{ 232, 006, 242, 045 }, Index = 32, Form = 2 }, // Meowth-2
            new(17,01,1) { Species = 837, Ability = A4, Moves = new[]{ 229, 261, 479, 108 }, Index = 32 }, // Rolycoly
            new(17,01,1) { Species = 111, Ability = A4, Moves = new[]{ 479, 523, 196, 182 }, Index = 32 }, // Rhyhorn
            new(17,01,1) { Species = 095, Ability = A4, Moves = new[]{ 174, 225, 034, 106 }, Index = 32 }, // Onix
            new(30,03,2) { Species = 878, Ability = A4, Moves = new[]{ 523, 023, 334, 249 }, Index = 32 }, // Cufant
            new(30,03,2) { Species = 208, Ability = A4, Moves = new[]{ 157, 442, 328, 422 }, Index = 32 }, // Steelix
            new(30,03,2) { Species = 863, Ability = A4, Moves = new[]{ 442, 006, 242, 269 }, Index = 32 }, // Perrserker
            new(30,03,2) { Species = 838, Ability = A4, Moves = new[]{ 229, 488, 157, 108 }, Index = 32 }, // Carkol
            new(30,03,2) { Species = 111, Ability = A4, Moves = new[]{ 350, 523, 196, 182 }, Index = 32 }, // Rhyhorn
            new(30,03,2) { Species = 095, Ability = A4, Moves = new[]{ 776, 225, 034, 106 }, Index = 32 }, // Onix
            new(40,05,3) { Species = 879, Ability = A4, Moves = new[]{ 070, 523, 334, 442 }, Index = 32, CanGigantamax = true }, // Copperajah
            new(40,05,3) { Species = 208, Ability = A4, Moves = new[]{ 157, 442, 328, 422 }, Index = 32 }, // Steelix
            new(40,05,3) { Species = 863, Ability = A4, Moves = new[]{ 442, 006, 154, 269 }, Index = 32 }, // Perrserker
            new(40,05,3) { Species = 839, Ability = A4, Moves = new[]{ 025, 488, 157, 108 }, Index = 32, CanGigantamax = true }, // Coalossal
            new(40,05,3) { Species = 112, Ability = A4, Moves = new[]{ 036, 529, 008, 182 }, Index = 32 }, // Rhydon
            new(40,05,3) { Species = 095, Ability = A4, Moves = new[]{ 776, 225, 021, 201 }, Index = 32 }, // Onix
            new(50,08,4) { Species = 879, Ability = A4, Moves = new[]{ 070, 523, 334, 442 }, Index = 32, CanGigantamax = true }, // Copperajah
            new(50,08,4) { Species = 208, Ability = A4, Moves = new[]{ 157, 231, 328, 422 }, Index = 32 }, // Steelix
            new(50,08,4) { Species = 863, Ability = A4, Moves = new[]{ 442, 583, 154, 269 }, Index = 32 }, // Perrserker
            new(50,08,4) { Species = 839, Ability = A4, Moves = new[]{ 025, 488, 157, 115 }, Index = 32, CanGigantamax = true }, // Coalossal
            new(50,08,4) { Species = 464, Ability = A4, Moves = new[]{ 350, 089, 008, 182 }, Index = 32 }, // Rhyperior
            new(50,08,4) { Species = 095, Ability = A4, Moves = new[]{ 776, 225, 784, 201 }, Index = 32 }, // Onix
            new(60,10,5) { Species = 879, Ability = A4, Moves = new[]{ 276, 089, 583, 442 }, Index = 32, CanGigantamax = true }, // Copperajah
            new(60,10,5) { Species = 208, Ability = A4, Moves = new[]{ 038, 231, 529, 422 }, Index = 32 }, // Steelix
            new(60,10,5) { Species = 863, Ability = A4, Moves = new[]{ 442, 583, 370, 269 }, Index = 32 }, // Perrserker
            new(60,10,5) { Species = 839, Ability = A4, Moves = new[]{ 076, 682, 157, 115 }, Index = 32, CanGigantamax = true }, // Coalossal
            new(60,10,5) { Species = 464, Ability = A4, Moves = new[]{ 444, 089, 008, 224 }, Index = 32 }, // Rhyperior
            new(60,10,5) { Species = 095, Ability = A4, Moves = new[]{ 776, 444, 784, 201 }, Index = 32 }, // Onix

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

            new(17,01,1) { Species = 833, Ability = A4, Moves = new[]{ 055, 033, 044, 240 }, Index = 35 }, // Chewtle
            new(17,01,1) { Species = 349, Ability = A4, Moves = new[]{ 150, 033, 175, 057 }, Index = 35 }, // Feebas
            new(17,01,1) { Species = 194, Ability = A4, Moves = new[]{ 341, 021, 039, 055 }, Index = 35 }, // Wooper
            new(17,01,1) { Species = 843, Ability = A4, Moves = new[]{ 028, 035, 523, 693 }, Index = 35 }, // Silicobra
            new(17,01,1) { Species = 449, Ability = A4, Moves = new[]{ 341, 328, 044, 033 }, Index = 35 }, // Hippopotas
            new(17,01,1) { Species = 422, Ability = A4, Moves = new[]{ 352, 106, 189, 055 }, Index = 35, Form = 1 }, // Shellos-1
            new(30,03,2) { Species = 834, Ability = A4, Moves = new[]{ 157, 534, 317, 055 }, Index = 35, CanGigantamax = true }, // Drednaw
            new(30,03,2) { Species = 349, Ability = A4, Moves = new[]{ 057, 033, 175, 150 }, Index = 35 }, // Feebas
            new(30,03,2) { Species = 195, Ability = A4, Moves = new[]{ 341, 021, 401, 055 }, Index = 35 }, // Quagsire
            new(30,03,2) { Species = 843, Ability = A4, Moves = new[]{ 091, 029, 523, 693 }, Index = 35 }, // Silicobra
            new(30,03,2) { Species = 449, Ability = A4, Moves = new[]{ 341, 036, 044, 242 }, Index = 35 }, // Hippopotas
            new(30,03,2) { Species = 423, Ability = A4, Moves = new[]{ 189, 352, 246, 106 }, Index = 35, Form = 1 }, // Gastrodon-1
            new(40,05,3) { Species = 834, Ability = A4, Moves = new[]{ 157, 534, 317, 055 }, Index = 35, CanGigantamax = true }, // Drednaw
            new(40,05,3) { Species = 350, Ability = A4, Moves = new[]{ 057, 239, 034, 574 }, Index = 35 }, // Milotic
            new(40,05,3) { Species = 195, Ability = A4, Moves = new[]{ 341, 021, 401, 005 }, Index = 35 }, // Quagsire
            new(40,05,3) { Species = 844, Ability = A4, Moves = new[]{ 693, 523, 201, 091 }, Index = 35, CanGigantamax = true }, // Sandaconda
            new(40,05,3) { Species = 450, Ability = A4, Moves = new[]{ 341, 422, 036, 242 }, Index = 35 }, // Hippowdon
            new(40,05,3) { Species = 423, Ability = A4, Moves = new[]{ 414, 352, 246, 106 }, Index = 35, Form = 1 }, // Gastrodon-1
            new(50,08,4) { Species = 834, Ability = A4, Moves = new[]{ 157, 710, 317, 334 }, Index = 35, CanGigantamax = true }, // Drednaw
            new(50,08,4) { Species = 350, Ability = A4, Moves = new[]{ 057, 231, 034, 574 }, Index = 35 }, // Milotic
            new(50,08,4) { Species = 195, Ability = A4, Moves = new[]{ 341, 280, 401, 005 }, Index = 35 }, // Quagsire
            new(50,08,4) { Species = 844, Ability = A4, Moves = new[]{ 693, 529, 201, 091 }, Index = 35, CanGigantamax = true }, // Sandaconda
            new(50,08,4) { Species = 450, Ability = A4, Moves = new[]{ 089, 422, 036, 242 }, Index = 35 }, // Hippowdon
            new(50,08,4) { Species = 423, Ability = A4, Moves = new[]{ 414, 503, 311, 106 }, Index = 35, Form = 1 }, // Gastrodon-1
            new(60,10,5) { Species = 834, Ability = A4, Moves = new[]{ 157, 710, 317, 334 }, Index = 35, CanGigantamax = true }, // Drednaw
            new(60,10,5) { Species = 350, Ability = A4, Moves = new[]{ 503, 231, 034, 574 }, Index = 35 }, // Milotic
            new(60,10,5) { Species = 195, Ability = A4, Moves = new[]{ 089, 280, 401, 005 }, Index = 35 }, // Quagsire
            new(60,10,5) { Species = 844, Ability = A4, Moves = new[]{ 693, 529, 201, 091 }, Index = 35, CanGigantamax = true }, // Sandaconda
            new(60,10,5) { Species = 450, Ability = A4, Moves = new[]{ 089, 422, 231, 242 }, Index = 35 }, // Hippowdon
            new(60,10,5) { Species = 423, Ability = A4, Moves = new[]{ 414, 503, 311, 352 }, Index = 35, Form = 1 }, // Gastrodon-1

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

            new(17,01,1) { Species = 848, Ability = A4, Moves = new[]{ 609, 051, 496, 715 }, Index = 38 }, // Toxel
            new(17,01,1) { Species = 835, Ability = A4, Moves = new[]{ 609, 033, 044, 046 }, Index = 38 }, // Yamper
            new(17,01,1) { Species = 695, Ability = A4, Moves = new[]{ 085, 098, 001, 189 }, Index = 38 }, // Heliolisk
            new(17,01,1) { Species = 840, Ability = A4, Moves = new[]{ 110, 310, 000, 000 }, Index = 38 }, // Applin
            new(17,01,1) { Species = 597, Ability = A4, Moves = new[]{ 033, 042, 232, 106 }, Index = 38 }, // Ferroseed
            new(17,01,1) { Species = 829, Ability = A4, Moves = new[]{ 075, 496, 229, 670 }, Index = 38 }, // Gossifleur
            new(30,03,2) { Species = 836, Ability = A4, Moves = new[]{ 609, 209, 204, 706 }, Index = 38 }, // Boltund
            new(30,03,2) { Species = 695, Ability = A4, Moves = new[]{ 085, 098, 001, 189 }, Index = 38 }, // Heliolisk
            new(30,03,2) { Species = 597, Ability = A4, Moves = new[]{ 442, 042, 232, 106 }, Index = 38 }, // Ferroseed
            new(30,03,2) { Species = 830, Ability = A4, Moves = new[]{ 536, 496, 229, 670 }, Index = 38 }, // Eldegoss
            new(40,05,3) { Species = 836, Ability = A4, Moves = new[]{ 609, 209, 424, 706 }, Index = 38 }, // Boltund
            new(40,05,3) { Species = 695, Ability = A4, Moves = new[]{ 085, 098, 447, 189 }, Index = 38 }, // Heliolisk
            new(40,05,3) { Species = 598, Ability = A4, Moves = new[]{ 442, 438, 398, 106 }, Index = 38 }, // Ferrothorn
            new(40,05,3) { Species = 830, Ability = A4, Moves = new[]{ 536, 304, 229, 670 }, Index = 38 }, // Eldegoss
            new(50,08,4) { Species = 836, Ability = A4, Moves = new[]{ 609, 528, 424, 706 }, Index = 38 }, // Boltund
            new(50,08,4) { Species = 695, Ability = A4, Moves = new[]{ 085, 304, 447, 523 }, Index = 38 }, // Heliolisk
            new(50,08,4) { Species = 598, Ability = A4, Moves = new[]{ 360, 438, 398, 014 }, Index = 38 }, // Ferrothorn
            new(50,08,4) { Species = 830, Ability = A4, Moves = new[]{ 536, 304, 229, 437 }, Index = 38 }, // Eldegoss
            new(60,10,5) { Species = 836, Ability = A4, Moves = new[]{ 609, 528, 424, 706 }, Index = 38 }, // Boltund
            new(60,10,5) { Species = 695, Ability = A4, Moves = new[]{ 085, 304, 447, 523 }, Index = 38 }, // Heliolisk
            new(60,10,5) { Species = 598, Ability = A4, Moves = new[]{ 360, 438, 398, 014 }, Index = 38 }, // Ferrothorn
            new(60,10,5) { Species = 830, Ability = A4, Moves = new[]{ 536, 304, 063, 437 }, Index = 38 }, // Eldegoss

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

            new(17,01,1) { Species = 093, Ability = A4, Moves = new[]{ 371, 122, 095, 325 }, Index = 40 }, // Haunter
            new(17,01,1) { Species = 425, Ability = A4, Moves = new[]{ 016, 506, 310, 371 }, Index = 40 }, // Drifloon
            new(17,01,1) { Species = 355, Ability = A4, Moves = new[]{ 310, 425, 043, 506 }, Index = 40 }, // Duskull
            new(17,01,1) { Species = 859, Ability = A4, Moves = new[]{ 372, 313, 260, 044 }, Index = 40 }, // Impidimp
            new(17,01,1) { Species = 633, Ability = A4, Moves = new[]{ 225, 033, 399, 044 }, Index = 40 }, // Deino
            new(17,01,1) { Species = 877, Ability = A4, Moves = new[]{ 084, 098, 681, 043 }, Index = 40 }, // Morpeko
            new(30,03,2) { Species = 094, Ability = A4, Moves = new[]{ 371, 389, 095, 325 }, Index = 40, CanGigantamax = true }, // Gengar
            new(30,03,2) { Species = 426, Ability = A4, Moves = new[]{ 016, 247, 310, 371 }, Index = 40 }, // Drifblim
            new(30,03,2) { Species = 355, Ability = A4, Moves = new[]{ 310, 425, 371, 506 }, Index = 40 }, // Duskull
            new(30,03,2) { Species = 859, Ability = A4, Moves = new[]{ 259, 389, 207, 044 }, Index = 40 }, // Impidimp
            new(30,03,2) { Species = 633, Ability = A4, Moves = new[]{ 225, 021, 399, 029 }, Index = 40 }, // Deino
            new(30,03,2) { Species = 877, Ability = A4, Moves = new[]{ 209, 098, 044, 043 }, Index = 40 }, // Morpeko
            new(40,05,3) { Species = 094, Ability = A4, Moves = new[]{ 506, 389, 095, 325 }, Index = 40, CanGigantamax = true }, // Gengar
            new(40,05,3) { Species = 426, Ability = A4, Moves = new[]{ 016, 247, 360, 371 }, Index = 40 }, // Drifblim
            new(40,05,3) { Species = 477, Ability = A4, Moves = new[]{ 247, 009, 371, 157 }, Index = 40 }, // Dusknoir
            new(40,05,3) { Species = 860, Ability = A4, Moves = new[]{ 417, 793, 421, 399 }, Index = 40 }, // Morgrem
            new(40,05,3) { Species = 633, Ability = A4, Moves = new[]{ 406, 021, 399, 423 }, Index = 40 }, // Deino
            new(40,05,3) { Species = 877, Ability = A4, Moves = new[]{ 209, 098, 044, 402 }, Index = 40 }, // Morpeko
            new(50,08,4) { Species = 094, Ability = A4, Moves = new[]{ 247, 399, 094, 085 }, Index = 40, CanGigantamax = true }, // Gengar
            new(50,08,4) { Species = 426, Ability = A4, Moves = new[]{ 366, 247, 360, 371 }, Index = 40 }, // Drifblim
            new(50,08,4) { Species = 477, Ability = A4, Moves = new[]{ 247, 009, 280, 157 }, Index = 40 }, // Dusknoir
            new(50,08,4) { Species = 861, Ability = A4, Moves = new[]{ 789, 492, 421, 399 }, Index = 40, CanGigantamax = true }, // Grimmsnarl
            new(50,08,4) { Species = 634, Ability = A4, Moves = new[]{ 406, 304, 399, 423 }, Index = 40 }, // Zweilous
            new(50,08,4) { Species = 877, Ability = A4, Moves = new[]{ 783, 098, 242, 402 }, Index = 40 }, // Morpeko
            new(60,10,5) { Species = 094, Ability = A4, Moves = new[]{ 247, 399, 605, 085 }, Index = 40, CanGigantamax = true }, // Gengar
            new(60,10,5) { Species = 426, Ability = A4, Moves = new[]{ 366, 247, 360, 693 }, Index = 40 }, // Drifblim
            new(60,10,5) { Species = 477, Ability = A4, Moves = new[]{ 247, 009, 280, 089 }, Index = 40 }, // Dusknoir
            new(60,10,5) { Species = 861, Ability = A4, Moves = new[]{ 789, 492, 421, 417 }, Index = 40, CanGigantamax = true }, // Grimmsnarl
            new(60,10,5) { Species = 635, Ability = A4, Moves = new[]{ 406, 304, 399, 056 }, Index = 40 }, // Hydreigon
            new(60,10,5) { Species = 877, Ability = A4, Moves = new[]{ 783, 037, 242, 402 }, Index = 40 }, // Morpeko

            new(17,01,1) { Species = 562, Ability = A4, Moves = new[]{ 261, 114, 310, 101 }, Index = 41 }, // Yamask
            new(17,01,1) { Species = 778, Ability = A4, Moves = new[]{ 086, 452, 425, 010 }, Index = 41 }, // Mimikyu
            new(17,01,1) { Species = 709, Ability = A4, Moves = new[]{ 785, 421, 261, 310 }, Index = 41 }, // Trevenant
            new(17,01,1) { Species = 855, Ability = A4, Moves = new[]{ 597, 110, 668, 310 }, Index = 41 }, // Polteageist
            new(30,03,2) { Species = 710, Ability = A4, Moves = new[]{ 567, 425, 310, 331 }, Index = 41 }, // Pumpkaboo
            new(30,03,2) { Species = 563, Ability = A4, Moves = new[]{ 578, 421, 310, 261 }, Index = 41 }, // Cofagrigus
            new(30,03,2) { Species = 778, Ability = A4, Moves = new[]{ 086, 452, 425, 608 }, Index = 41 }, // Mimikyu
            new(30,03,2) { Species = 709, Ability = A4, Moves = new[]{ 785, 506, 261, 310 }, Index = 41 }, // Trevenant
            new(30,03,2) { Species = 855, Ability = A4, Moves = new[]{ 597, 110, 389, 310 }, Index = 41 }, // Polteageist
            new(40,05,3) { Species = 710, Ability = A4, Moves = new[]{ 567, 247, 310, 402 }, Index = 41 }, // Pumpkaboo
            new(40,05,3) { Species = 563, Ability = A4, Moves = new[]{ 578, 421, 310, 261 }, Index = 41 }, // Cofagrigus
            new(40,05,3) { Species = 778, Ability = A4, Moves = new[]{ 085, 452, 421, 608 }, Index = 41 }, // Mimikyu
            new(40,05,3) { Species = 709, Ability = A4, Moves = new[]{ 785, 506, 261, 310 }, Index = 41 }, // Trevenant
            new(40,05,3) { Species = 855, Ability = A4, Moves = new[]{ 597, 110, 389, 310 }, Index = 41 }, // Polteageist
            new(50,08,4) { Species = 711, Ability = A4, Moves = new[]{ 567, 247, 585, 402 }, Index = 41 }, // Gourgeist
            new(50,08,4) { Species = 711, Ability = A4, Moves = new[]{ 567, 247, 585, 402 }, Index = 41, Form = 1 }, // Gourgeist-1
            new(50,08,4) { Species = 711, Ability = A4, Moves = new[]{ 567, 247, 585, 402 }, Index = 41, Form = 2 }, // Gourgeist-2
            new(50,08,4) { Species = 711, Ability = A4, Moves = new[]{ 567, 247, 585, 402 }, Index = 41, Form = 3 }, // Gourgeist-3
            new(50,08,4) { Species = 563, Ability = A4, Moves = new[]{ 578, 247, 399, 261 }, Index = 41 }, // Cofagrigus
            new(50,08,4) { Species = 778, Ability = A4, Moves = new[]{ 085, 452, 261, 204 }, Index = 41 }, // Mimikyu
            new(50,08,4) { Species = 709, Ability = A4, Moves = new[]{ 452, 506, 261, 310 }, Index = 41 }, // Trevenant
            new(50,08,4) { Species = 855, Ability = A4, Moves = new[]{ 247, 417, 389, 310 }, Index = 41 }, // Polteageist
            new(60,10,5) { Species = 711, Ability = A4, Moves = new[]{ 567, 247, 433, 402 }, Index = 41 }, // Gourgeist
            new(60,10,5) { Species = 711, Ability = A4, Moves = new[]{ 567, 247, 433, 402 }, Index = 41, Form = 1, Shiny = Shiny.Always }, // Gourgeist-1
            new(60,10,5) { Species = 711, Ability = A4, Moves = new[]{ 567, 247, 433, 402 }, Index = 41, Form = 2 }, // Gourgeist-2
            new(60,10,5) { Species = 711, Ability = A4, Moves = new[]{ 567, 247, 433, 402 }, Index = 41, Form = 3, Shiny = Shiny.Always }, // Gourgeist-3
            new(60,10,5) { Species = 563, Ability = A4, Moves = new[]{ 578, 247, 399, 261 }, Index = 41 }, // Cofagrigus
            new(60,10,5) { Species = 778, Ability = A4, Moves = new[]{ 087, 452, 261, 583 }, Index = 41 }, // Mimikyu
            new(60,10,5) { Species = 709, Ability = A4, Moves = new[]{ 452, 506, 261, 310 }, Index = 41 }, // Trevenant
            new(60,10,5) { Species = 855, Ability = A4, Moves = new[]{ 247, 417, 389, 310 }, Index = 41 }, // Polteageist

            new(17,01,1) { Species = 012, Ability = A4, Moves = new[]{ 081, 060, 016, 079 }, Index = 42, CanGigantamax = true }, // Butterfree
            new(17,01,1) { Species = 213, Ability = A4, Moves = new[]{ 088, 474, 414, 522 }, Index = 42 }, // Shuckle
            new(17,01,1) { Species = 290, Ability = A4, Moves = new[]{ 189, 206, 010, 106 }, Index = 42 }, // Nincada
            new(17,01,1) { Species = 568, Ability = A4, Moves = new[]{ 390, 133, 491, 001 }, Index = 42 }, // Trubbish
            new(17,01,1) { Species = 043, Ability = A4, Moves = new[]{ 078, 077, 051, 230 }, Index = 42 }, // Oddish
            new(17,01,1) { Species = 453, Ability = A4, Moves = new[]{ 040, 269, 279, 189 }, Index = 42 }, // Croagunk
            new(30,03,2) { Species = 012, Ability = A4, Moves = new[]{ 405, 060, 016, 079 }, Index = 42, CanGigantamax = true }, // Butterfree
            new(30,03,2) { Species = 213, Ability = A4, Moves = new[]{ 088, 474, 414, 522 }, Index = 42 }, // Shuckle
            new(30,03,2) { Species = 291, Ability = A4, Moves = new[]{ 210, 206, 332, 232 }, Index = 42 }, // Ninjask
            new(30,03,2) { Species = 568, Ability = A4, Moves = new[]{ 092, 133, 491, 036 }, Index = 42 }, // Trubbish
            new(30,03,2) { Species = 045, Ability = A4, Moves = new[]{ 080, 585, 051, 230 }, Index = 42 }, // Vileplume
            new(30,03,2) { Species = 453, Ability = A4, Moves = new[]{ 474, 389, 279, 189 }, Index = 42 }, // Croagunk
            new(40,05,3) { Species = 012, Ability = A4, Moves = new[]{ 405, 060, 016, 078 }, Index = 42, CanGigantamax = true }, // Butterfree
            new(40,05,3) { Species = 213, Ability = A4, Moves = new[]{ 157, 188, 089, 522 }, Index = 42 }, // Shuckle
            new(40,05,3) { Species = 291, Ability = A4, Moves = new[]{ 210, 206, 332, 232 }, Index = 42 }, // Ninjask
            new(40,05,3) { Species = 569, Ability = A4, Moves = new[]{ 188, 133, 034, 707 }, Index = 42, CanGigantamax = true }, // Garbodor
            new(40,05,3) { Species = 045, Ability = A4, Moves = new[]{ 080, 585, 051, 230 }, Index = 42 }, // Vileplume
            new(40,05,3) { Species = 454, Ability = A4, Moves = new[]{ 188, 389, 279, 189 }, Index = 42 }, // Toxicroak
            new(50,08,4) { Species = 012, Ability = A4, Moves = new[]{ 405, 403, 093, 078 }, Index = 42, CanGigantamax = true }, // Butterfree
            new(50,08,4) { Species = 213, Ability = A4, Moves = new[]{ 157, 188, 089, 564 }, Index = 42 }, // Shuckle
            new(50,08,4) { Species = 291, Ability = A4, Moves = new[]{ 210, 163, 332, 232 }, Index = 42 }, // Ninjask
            new(50,08,4) { Species = 569, Ability = A4, Moves = new[]{ 188, 499, 034, 707 }, Index = 42, CanGigantamax = true }, // Garbodor
            new(50,08,4) { Species = 045, Ability = A4, Moves = new[]{ 080, 585, 051, 034 }, Index = 42 }, // Vileplume
            new(50,08,4) { Species = 454, Ability = A4, Moves = new[]{ 188, 389, 280, 189 }, Index = 42 }, // Toxicroak
            new(60,10,5) { Species = 012, Ability = A4, Moves = new[]{ 405, 403, 527, 078 }, Index = 42, CanGigantamax = true }, // Butterfree
            new(60,10,5) { Species = 213, Ability = A4, Moves = new[]{ 444, 188, 089, 564 }, Index = 42 }, // Shuckle
            new(60,10,5) { Species = 291, Ability = A4, Moves = new[]{ 404, 163, 332, 232 }, Index = 42 }, // Ninjask
            new(60,10,5) { Species = 569, Ability = A4, Moves = new[]{ 441, 499, 034, 707 }, Index = 42, CanGigantamax = true }, // Garbodor
            new(60,10,5) { Species = 045, Ability = A4, Moves = new[]{ 080, 585, 051, 034 }, Index = 42 }, // Vileplume
            new(60,10,5) { Species = 454, Ability = A4, Moves = new[]{ 188, 389, 280, 523 }, Index = 42 }, // Toxicroak

            new(17,01,1) { Species = 420, Ability = A4, Moves = new[]{ 033, 572, 074, 670 }, Index = 43 }, // Cherubi
            new(17,01,1) { Species = 590, Ability = A4, Moves = new[]{ 078, 492, 310, 412 }, Index = 43 }, // Foongus
            new(17,01,1) { Species = 755, Ability = A4, Moves = new[]{ 402, 109, 605, 310 }, Index = 43 }, // Morelull
            new(17,01,1) { Species = 819, Ability = A4, Moves = new[]{ 747, 231, 371, 033 }, Index = 43 }, // Skwovet
            new(30,03,2) { Species = 420, Ability = A4, Moves = new[]{ 033, 572, 074, 402 }, Index = 43 }, // Cherubi
            new(30,03,2) { Species = 590, Ability = A4, Moves = new[]{ 078, 492, 310, 412 }, Index = 43 }, // Foongus
            new(30,03,2) { Species = 756, Ability = A4, Moves = new[]{ 402, 109, 605, 310 }, Index = 43 }, // Shiinotic
            new(30,03,2) { Species = 819, Ability = A4, Moves = new[]{ 747, 231, 371, 033 }, Index = 43 }, // Skwovet
            new(30,03,2) { Species = 820, Ability = A4, Moves = new[]{ 747, 231, 371, 034 }, Index = 43 }, // Greedent
            new(40,05,3) { Species = 420, Ability = A4, Moves = new[]{ 311, 572, 074, 402 }, Index = 43 }, // Cherubi
            new(40,05,3) { Species = 591, Ability = A4, Moves = new[]{ 078, 492, 092, 412 }, Index = 43 }, // Amoonguss
            new(40,05,3) { Species = 756, Ability = A4, Moves = new[]{ 402, 585, 605, 310 }, Index = 43 }, // Shiinotic
            new(40,05,3) { Species = 819, Ability = A4, Moves = new[]{ 747, 360, 371, 044 }, Index = 43 }, // Skwovet
            new(40,05,3) { Species = 820, Ability = A4, Moves = new[]{ 747, 360, 371, 424 }, Index = 43 }, // Greedent
            new(50,08,4) { Species = 420, Ability = A4, Moves = new[]{ 311, 572, 605, 402 }, Index = 43 }, // Cherubi
            new(50,08,4) { Species = 591, Ability = A4, Moves = new[]{ 188, 492, 092, 412 }, Index = 43 }, // Amoonguss
            new(50,08,4) { Species = 756, Ability = A4, Moves = new[]{ 412, 585, 605, 188 }, Index = 43 }, // Shiinotic
            new(50,08,4) { Species = 819, Ability = A4, Moves = new[]{ 747, 360, 371, 331 }, Index = 43 }, // Skwovet
            new(50,08,4) { Species = 820, Ability = A4, Moves = new[]{ 747, 360, 371, 424 }, Index = 43 }, // Greedent
            new(60,10,5) { Species = 420, Ability = A4, Moves = new[]{ 311, 572, 605, 412 }, Index = 43 }, // Cherubi
            new(60,10,5) { Species = 591, Ability = A4, Moves = new[]{ 147, 492, 092, 412 }, Index = 43 }, // Amoonguss
            new(60,10,5) { Species = 756, Ability = A4, Moves = new[]{ 147, 585, 605, 188 }, Index = 43 }, // Shiinotic
            new(60,10,5) { Species = 819, Ability = A4, Moves = new[]{ 747, 360, 371, 034 }, Index = 43 }, // Skwovet
            new(60,10,5) { Species = 819, Ability = A4, Moves = new[]{ 747, 360, 371, 034 }, Index = 43, Shiny = Shiny.Always }, // Skwovet
            new(60,10,5) { Species = 820, Ability = A4, Moves = new[]{ 747, 360, 371, 089 }, Index = 43 }, // Greedent

            new(17,01,1) { Species = 012, Ability = A4, Moves = new[]{ 081, 060, 016, 079 }, Index = 44, CanGigantamax = true }, // Butterfree
            new(17,01,1) { Species = 213, Ability = A4, Moves = new[]{ 088, 474, 414, 522 }, Index = 44 }, // Shuckle
            new(17,01,1) { Species = 290, Ability = A4, Moves = new[]{ 189, 206, 010, 106 }, Index = 44 }, // Nincada
            new(17,01,1) { Species = 568, Ability = A4, Moves = new[]{ 390, 133, 491, 001 }, Index = 44 }, // Trubbish
            new(17,01,1) { Species = 043, Ability = A4, Moves = new[]{ 078, 077, 051, 230 }, Index = 44 }, // Oddish
            new(17,01,1) { Species = 453, Ability = A4, Moves = new[]{ 040, 269, 279, 189 }, Index = 44 }, // Croagunk
            new(30,03,2) { Species = 012, Ability = A4, Moves = new[]{ 405, 060, 016, 079 }, Index = 44, CanGigantamax = true }, // Butterfree
            new(30,03,2) { Species = 213, Ability = A4, Moves = new[]{ 088, 474, 414, 522 }, Index = 44 }, // Shuckle
            new(30,03,2) { Species = 291, Ability = A4, Moves = new[]{ 210, 206, 332, 232 }, Index = 44 }, // Ninjask
            new(30,03,2) { Species = 568, Ability = A4, Moves = new[]{ 092, 133, 491, 036 }, Index = 44 }, // Trubbish
            new(30,03,2) { Species = 045, Ability = A4, Moves = new[]{ 080, 585, 051, 230 }, Index = 44 }, // Vileplume
            new(30,03,2) { Species = 453, Ability = A4, Moves = new[]{ 474, 389, 279, 189 }, Index = 44 }, // Croagunk
            new(40,05,3) { Species = 012, Ability = A4, Moves = new[]{ 405, 060, 016, 078 }, Index = 44, CanGigantamax = true }, // Butterfree
            new(40,05,3) { Species = 213, Ability = A4, Moves = new[]{ 157, 188, 089, 522 }, Index = 44 }, // Shuckle
            new(40,05,3) { Species = 291, Ability = A4, Moves = new[]{ 210, 206, 332, 232 }, Index = 44 }, // Ninjask
            new(40,05,3) { Species = 569, Ability = A4, Moves = new[]{ 188, 133, 034, 707 }, Index = 44, CanGigantamax = true }, // Garbodor
            new(40,05,3) { Species = 045, Ability = A4, Moves = new[]{ 080, 585, 051, 230 }, Index = 44 }, // Vileplume
            new(40,05,3) { Species = 454, Ability = A4, Moves = new[]{ 188, 389, 279, 189 }, Index = 44 }, // Toxicroak
            new(50,08,4) { Species = 012, Ability = A4, Moves = new[]{ 405, 403, 093, 078 }, Index = 44, CanGigantamax = true }, // Butterfree
            new(50,08,4) { Species = 213, Ability = A4, Moves = new[]{ 157, 188, 089, 564 }, Index = 44 }, // Shuckle
            new(50,08,4) { Species = 291, Ability = A4, Moves = new[]{ 210, 163, 332, 232 }, Index = 44 }, // Ninjask
            new(50,08,4) { Species = 569, Ability = A4, Moves = new[]{ 188, 499, 034, 707 }, Index = 44, CanGigantamax = true }, // Garbodor
            new(50,08,4) { Species = 045, Ability = A4, Moves = new[]{ 080, 585, 051, 034 }, Index = 44 }, // Vileplume
            new(50,08,4) { Species = 454, Ability = A4, Moves = new[]{ 188, 389, 280, 189 }, Index = 44 }, // Toxicroak
            new(60,10,5) { Species = 012, Ability = A4, Moves = new[]{ 405, 403, 527, 078 }, Index = 44, CanGigantamax = true }, // Butterfree
            new(60,10,5) { Species = 213, Ability = A4, Moves = new[]{ 444, 188, 089, 564 }, Index = 44 }, // Shuckle
            new(60,10,5) { Species = 291, Ability = A4, Moves = new[]{ 404, 163, 332, 232 }, Index = 44 }, // Ninjask
            new(60,10,5) { Species = 569, Ability = A4, Moves = new[]{ 441, 499, 034, 707 }, Index = 44, CanGigantamax = true }, // Garbodor
            new(60,10,5) { Species = 045, Ability = A4, Moves = new[]{ 080, 585, 051, 034 }, Index = 44 }, // Vileplume
            new(60,10,5) { Species = 454, Ability = A4, Moves = new[]{ 188, 389, 280, 523 }, Index = 44 }, // Toxicroak

            new(17,01,1) { Species = 131, Ability = A4, Moves = new[]{ 055, 420, 045, 047 }, Index = 45, CanGigantamax = true }, // Lapras
            new(17,01,1) { Species = 712, Ability = A4, Moves = new[]{ 181, 196, 033, 106 }, Index = 45 }, // Bergmite
            new(17,01,1) { Species = 461, Ability = A4, Moves = new[]{ 420, 372, 232, 279 }, Index = 45 }, // Weavile
            new(17,01,1) { Species = 850, Ability = A4, Moves = new[]{ 172, 044, 035, 052 }, Index = 45 }, // Sizzlipede
            new(17,01,1) { Species = 776, Ability = A4, Moves = new[]{ 175, 123, 033, 052 }, Index = 45 }, // Turtonator
            new(17,01,1) { Species = 077, Ability = A4, Moves = new[]{ 488, 045, 039, 052 }, Index = 45 }, // Ponyta
            new(30,03,2) { Species = 131, Ability = A4, Moves = new[]{ 352, 420, 109, 047 }, Index = 45, CanGigantamax = true }, // Lapras
            new(30,03,2) { Species = 712, Ability = A4, Moves = new[]{ 157, 423, 033, 044 }, Index = 45 }, // Bergmite
            new(30,03,2) { Species = 461, Ability = A4, Moves = new[]{ 420, 372, 232, 279 }, Index = 45 }, // Weavile
            new(30,03,2) { Species = 851, Ability = A4, Moves = new[]{ 172, 404, 422, 044 }, Index = 45, CanGigantamax = true }, // Centiskorch
            new(30,03,2) { Species = 776, Ability = A4, Moves = new[]{ 406, 123, 033, 052 }, Index = 45 }, // Turtonator
            new(30,03,2) { Species = 077, Ability = A4, Moves = new[]{ 488, 023, 583, 097 }, Index = 45 }, // Ponyta
            new(40,05,3) { Species = 131, Ability = A4, Moves = new[]{ 352, 196, 109, 047 }, Index = 45, CanGigantamax = true }, // Lapras
            new(40,05,3) { Species = 713, Ability = A4, Moves = new[]{ 157, 423, 036, 044 }, Index = 45 }, // Avalugg
            new(40,05,3) { Species = 461, Ability = A4, Moves = new[]{ 420, 468, 232, 279 }, Index = 45 }, // Weavile
            new(40,05,3) { Species = 851, Ability = A4, Moves = new[]{ 424, 404, 422, 044 }, Index = 45, CanGigantamax = true }, // Centiskorch
            new(40,05,3) { Species = 776, Ability = A4, Moves = new[]{ 406, 776, 034, 053 }, Index = 45 }, // Turtonator
            new(40,05,3) { Species = 078, Ability = A4, Moves = new[]{ 172, 023, 583, 224 }, Index = 45 }, // Rapidash
            new(50,08,4) { Species = 131, Ability = A4, Moves = new[]{ 057, 196, 058, 047 }, Index = 45, CanGigantamax = true }, // Lapras
            new(50,08,4) { Species = 713, Ability = A4, Moves = new[]{ 776, 059, 036, 044 }, Index = 45 }, // Avalugg
            new(50,08,4) { Species = 461, Ability = A4, Moves = new[]{ 420, 468, 232, 279 }, Index = 45 }, // Weavile
            new(50,08,4) { Species = 851, Ability = A4, Moves = new[]{ 680, 404, 422, 044 }, Index = 45, CanGigantamax = true }, // Centiskorch
            new(50,08,4) { Species = 776, Ability = A4, Moves = new[]{ 406, 776, 504, 053 }, Index = 45 }, // Turtonator
            new(50,08,4) { Species = 078, Ability = A4, Moves = new[]{ 517, 528, 583, 224 }, Index = 45 }, // Rapidash
            new(60,10,5) { Species = 131, Ability = A4, Moves = new[]{ 057, 196, 058, 329 }, Index = 45, CanGigantamax = true }, // Lapras
            new(60,10,5) { Species = 713, Ability = A4, Moves = new[]{ 776, 059, 038, 044 }, Index = 45 }, // Avalugg
            new(60,10,5) { Species = 461, Ability = A4, Moves = new[]{ 420, 400, 232, 279 }, Index = 45 }, // Weavile
            new(60,10,5) { Species = 851, Ability = A4, Moves = new[]{ 680, 679, 422, 044 }, Index = 45, CanGigantamax = true }, // Centiskorch
            new(60,10,5) { Species = 776, Ability = A4, Moves = new[]{ 434, 776, 504, 053 }, Index = 45 }, // Turtonator
            new(60,10,5) { Species = 078, Ability = A4, Moves = new[]{ 394, 528, 583, 224 }, Index = 45 }, // Rapidash

            new(17,01,1) { Species = 037, Ability = A4, Moves = new[]{ 420, 196, 039, 577 }, Index = 46, Form = 1 }, // Vulpix-1
            new(17,01,1) { Species = 124, Ability = A4, Moves = new[]{ 181, 001, 093, 122 }, Index = 46 }, // Jynx
            new(17,01,1) { Species = 225, Ability = A4, Moves = new[]{ 217, 229, 098, 420 }, Index = 46 }, // Delibird
            new(17,01,1) { Species = 607, Ability = A4, Moves = new[]{ 310, 052, 506, 123 }, Index = 46 }, // Litwick
            new(17,01,1) { Species = 873, Ability = A4, Moves = new[]{ 522, 078, 181, 432 }, Index = 46 }, // Frosmoth
            new(30,03,2) { Species = 037, Ability = A4, Moves = new[]{ 420, 058, 326, 577 }, Index = 46, Form = 1 }, // Vulpix-1
            new(30,03,2) { Species = 124, Ability = A4, Moves = new[]{ 181, 001, 093, 313 }, Index = 46 }, // Jynx
            new(30,03,2) { Species = 225, Ability = A4, Moves = new[]{ 217, 065, 034, 693 }, Index = 46 }, // Delibird
            new(30,03,2) { Species = 608, Ability = A4, Moves = new[]{ 310, 261, 083, 123 }, Index = 46 }, // Lampent
            new(30,03,2) { Species = 873, Ability = A4, Moves = new[]{ 522, 078, 062, 432 }, Index = 46 }, // Frosmoth
            new(40,05,3) { Species = 037, Ability = A4, Moves = new[]{ 062, 058, 326, 577 }, Index = 46, Form = 1 }, // Vulpix-1
            new(40,05,3) { Species = 124, Ability = A4, Moves = new[]{ 058, 142, 094, 247 }, Index = 46 }, // Jynx
            new(40,05,3) { Species = 225, Ability = A4, Moves = new[]{ 217, 065, 280, 196 }, Index = 46 }, // Delibird
            new(40,05,3) { Species = 609, Ability = A4, Moves = new[]{ 247, 261, 257, 094 }, Index = 46 }, // Chandelure
            new(40,05,3) { Species = 873, Ability = A4, Moves = new[]{ 405, 403, 062, 432 }, Index = 46 }, // Frosmoth
            new(50,08,4) { Species = 037, Ability = A4, Moves = new[]{ 694, 058, 326, 577 }, Index = 46, Form = 1 }, // Vulpix-1
            new(50,08,4) { Species = 124, Ability = A4, Moves = new[]{ 058, 142, 094, 247 }, Index = 46 }, // Jynx
            new(50,08,4) { Species = 225, Ability = A4, Moves = new[]{ 217, 059, 034, 280 }, Index = 46 }, // Delibird
            new(50,08,4) { Species = 609, Ability = A4, Moves = new[]{ 247, 261, 315, 094 }, Index = 46 }, // Chandelure
            new(50,08,4) { Species = 873, Ability = A4, Moves = new[]{ 405, 403, 058, 297 }, Index = 46 }, // Frosmoth
            new(60,10,5) { Species = 037, Ability = A4, Moves = new[]{ 694, 059, 326, 577 }, Index = 46, Form = 1 }, // Vulpix-1
            new(60,10,5) { Species = 037, Ability = A4, Moves = new[]{ 694, 059, 326, 577 }, Index = 46, Form = 1, Shiny = Shiny.Always }, // Vulpix-1
            new(60,10,5) { Species = 124, Ability = A4, Moves = new[]{ 058, 142, 094, 247 }, Index = 46 }, // Jynx
            new(60,10,5) { Species = 225, Ability = A4, Moves = new[]{ 217, 059, 065, 280 }, Index = 46 }, // Delibird
            new(60,10,5) { Species = 609, Ability = A4, Moves = new[]{ 247, 412, 315, 094 }, Index = 46 }, // Chandelure
            new(60,10,5) { Species = 873, Ability = A4, Moves = new[]{ 405, 403, 058, 542 }, Index = 46 }, // Frosmoth

            new(17,01,1) { Species = 131, Ability = A4, Moves = new[]{ 055, 420, 045, 047 }, Index = 47, CanGigantamax = true }, // Lapras
            new(17,01,1) { Species = 712, Ability = A4, Moves = new[]{ 181, 196, 033, 106 }, Index = 47 }, // Bergmite
            new(17,01,1) { Species = 461, Ability = A4, Moves = new[]{ 420, 372, 232, 279 }, Index = 47 }, // Weavile
            new(17,01,1) { Species = 850, Ability = A4, Moves = new[]{ 172, 044, 035, 052 }, Index = 47 }, // Sizzlipede
            new(17,01,1) { Species = 776, Ability = A4, Moves = new[]{ 175, 123, 033, 052 }, Index = 47 }, // Turtonator
            new(17,01,1) { Species = 077, Ability = A4, Moves = new[]{ 488, 045, 039, 052 }, Index = 47 }, // Ponyta
            new(30,03,2) { Species = 131, Ability = A4, Moves = new[]{ 352, 420, 109, 047 }, Index = 47, CanGigantamax = true }, // Lapras
            new(30,03,2) { Species = 712, Ability = A4, Moves = new[]{ 157, 423, 033, 044 }, Index = 47 }, // Bergmite
            new(30,03,2) { Species = 461, Ability = A4, Moves = new[]{ 420, 372, 232, 279 }, Index = 47 }, // Weavile
            new(30,03,2) { Species = 851, Ability = A4, Moves = new[]{ 172, 404, 422, 044 }, Index = 47, CanGigantamax = true }, // Centiskorch
            new(30,03,2) { Species = 776, Ability = A4, Moves = new[]{ 406, 123, 033, 052 }, Index = 47 }, // Turtonator
            new(30,03,2) { Species = 077, Ability = A4, Moves = new[]{ 488, 023, 583, 097 }, Index = 47 }, // Ponyta
            new(40,05,3) { Species = 131, Ability = A4, Moves = new[]{ 352, 196, 109, 047 }, Index = 47, CanGigantamax = true }, // Lapras
            new(40,05,3) { Species = 713, Ability = A4, Moves = new[]{ 157, 423, 036, 044 }, Index = 47 }, // Avalugg
            new(40,05,3) { Species = 461, Ability = A4, Moves = new[]{ 420, 468, 232, 279 }, Index = 47 }, // Weavile
            new(40,05,3) { Species = 851, Ability = A4, Moves = new[]{ 424, 404, 422, 044 }, Index = 47, CanGigantamax = true }, // Centiskorch
            new(40,05,3) { Species = 776, Ability = A4, Moves = new[]{ 406, 776, 034, 053 }, Index = 47 }, // Turtonator
            new(40,05,3) { Species = 078, Ability = A4, Moves = new[]{ 172, 023, 583, 224 }, Index = 47 }, // Rapidash
            new(50,08,4) { Species = 131, Ability = A4, Moves = new[]{ 057, 196, 058, 047 }, Index = 47, CanGigantamax = true }, // Lapras
            new(50,08,4) { Species = 713, Ability = A4, Moves = new[]{ 776, 059, 036, 044 }, Index = 47 }, // Avalugg
            new(50,08,4) { Species = 461, Ability = A4, Moves = new[]{ 420, 468, 232, 279 }, Index = 47 }, // Weavile
            new(50,08,4) { Species = 851, Ability = A4, Moves = new[]{ 680, 404, 422, 044 }, Index = 47, CanGigantamax = true }, // Centiskorch
            new(50,08,4) { Species = 776, Ability = A4, Moves = new[]{ 406, 776, 504, 053 }, Index = 47 }, // Turtonator
            new(50,08,4) { Species = 078, Ability = A4, Moves = new[]{ 517, 528, 583, 224 }, Index = 47 }, // Rapidash
            new(60,10,5) { Species = 131, Ability = A4, Moves = new[]{ 057, 196, 058, 329 }, Index = 47, CanGigantamax = true }, // Lapras
            new(60,10,5) { Species = 713, Ability = A4, Moves = new[]{ 776, 059, 038, 044 }, Index = 47 }, // Avalugg
            new(60,10,5) { Species = 461, Ability = A4, Moves = new[]{ 420, 400, 232, 279 }, Index = 47 }, // Weavile
            new(60,10,5) { Species = 851, Ability = A4, Moves = new[]{ 680, 679, 422, 044 }, Index = 47, CanGigantamax = true }, // Centiskorch
            new(60,10,5) { Species = 776, Ability = A4, Moves = new[]{ 434, 776, 504, 053 }, Index = 47 }, // Turtonator
            new(60,10,5) { Species = 078, Ability = A4, Moves = new[]{ 394, 528, 583, 224 }, Index = 47 }, // Rapidash

            new(17,01,1) { Species = 884, Ability = A4, Moves = new[]{ 232, 043, 468, 249 }, Index = 48, CanGigantamax = true }, // Duraludon
            new(17,01,1) { Species = 610, Ability = A4, Moves = new[]{ 044, 163, 372, 010 }, Index = 48 }, // Axew
            new(17,01,1) { Species = 704, Ability = A4, Moves = new[]{ 225, 352, 033, 175 }, Index = 48 }, // Goomy
            new(17,01,1) { Species = 446, Ability = A4, Moves = new[]{ 033, 044, 122, 111 }, Index = 48 }, // Munchlax
            new(17,01,1) { Species = 759, Ability = A4, Moves = new[]{ 693, 371, 608, 033 }, Index = 48 }, // Stufful
            new(17,01,1) { Species = 572, Ability = A4, Moves = new[]{ 497, 204, 402, 001 }, Index = 48 }, // Minccino
            new(30,03,2) { Species = 884, Ability = A4, Moves = new[]{ 232, 784, 468, 249 }, Index = 48, CanGigantamax = true }, // Duraludon
            new(30,03,2) { Species = 610, Ability = A4, Moves = new[]{ 337, 163, 242, 530 }, Index = 48 }, // Axew
            new(30,03,2) { Species = 704, Ability = A4, Moves = new[]{ 225, 352, 033, 341 }, Index = 48 }, // Goomy
            new(30,03,2) { Species = 143, Ability = A4, Moves = new[]{ 034, 242, 118, 111 }, Index = 48, CanGigantamax = true }, // Snorlax
            new(30,03,2) { Species = 759, Ability = A4, Moves = new[]{ 693, 371, 359, 036 }, Index = 48 }, // Stufful
            new(30,03,2) { Species = 572, Ability = A4, Moves = new[]{ 497, 231, 402, 129 }, Index = 48 }, // Minccino
            new(40,05,3) { Species = 884, Ability = A4, Moves = new[]{ 232, 525, 085, 249 }, Index = 48, CanGigantamax = true }, // Duraludon
            new(40,05,3) { Species = 611, Ability = A4, Moves = new[]{ 406, 231, 242, 530 }, Index = 48 }, // Fraxure
            new(40,05,3) { Species = 705, Ability = A4, Moves = new[]{ 406, 352, 491, 341 }, Index = 48 }, // Sliggoo
            new(40,05,3) { Species = 143, Ability = A4, Moves = new[]{ 034, 667, 242, 281 }, Index = 48, CanGigantamax = true }, // Snorlax
            new(40,05,3) { Species = 760, Ability = A4, Moves = new[]{ 693, 034, 359, 036 }, Index = 48 }, // Bewear
            new(40,05,3) { Species = 573, Ability = A4, Moves = new[]{ 331, 231, 350, 129 }, Index = 48 }, // Cinccino
            new(50,08,4) { Species = 884, Ability = A4, Moves = new[]{ 232, 406, 085, 776 }, Index = 48, CanGigantamax = true }, // Duraludon
            new(50,08,4) { Species = 612, Ability = A4, Moves = new[]{ 406, 231, 370, 530 }, Index = 48 }, // Haxorus
            new(50,08,4) { Species = 706, Ability = A4, Moves = new[]{ 406, 034, 491, 126 }, Index = 48 }, // Goodra
            new(50,08,4) { Species = 143, Ability = A4, Moves = new[]{ 034, 667, 280, 523 }, Index = 48, CanGigantamax = true }, // Snorlax
            new(50,08,4) { Species = 760, Ability = A4, Moves = new[]{ 663, 034, 359, 009 }, Index = 48 }, // Bewear
            new(50,08,4) { Species = 573, Ability = A4, Moves = new[]{ 331, 231, 350, 304 }, Index = 48 }, // Cinccino
            new(60,10,5) { Species = 884, Ability = A4, Moves = new[]{ 430, 406, 085, 776 }, Index = 48, CanGigantamax = true }, // Duraludon
            new(60,10,5) { Species = 612, Ability = A4, Moves = new[]{ 200, 231, 370, 089 }, Index = 48 }, // Haxorus
            new(60,10,5) { Species = 706, Ability = A4, Moves = new[]{ 406, 438, 482, 126 }, Index = 48 }, // Goodra
            new(60,10,5) { Species = 143, Ability = A4, Moves = new[]{ 034, 442, 242, 428 }, Index = 48, CanGigantamax = true }, // Snorlax
            new(60,10,5) { Species = 760, Ability = A4, Moves = new[]{ 663, 038, 276, 009 }, Index = 48 }, // Bewear
            new(60,10,5) { Species = 573, Ability = A4, Moves = new[]{ 402, 231, 350, 304 }, Index = 48 }, // Cinccino

            new(17,01,1) { Species = 420, Ability = A4, Moves = new[]{ 033, 572, 074, 670 }, Index = 49 }, // Cherubi
            new(17,01,1) { Species = 590, Ability = A4, Moves = new[]{ 078, 492, 310, 412 }, Index = 49 }, // Foongus
            new(17,01,1) { Species = 755, Ability = A4, Moves = new[]{ 402, 109, 605, 310 }, Index = 49 }, // Morelull
            new(17,01,1) { Species = 819, Ability = A4, Moves = new[]{ 747, 231, 371, 033 }, Index = 49 }, // Skwovet
            new(30,03,2) { Species = 420, Ability = A4, Moves = new[]{ 033, 572, 074, 402 }, Index = 49 }, // Cherubi
            new(30,03,2) { Species = 590, Ability = A4, Moves = new[]{ 078, 492, 310, 412 }, Index = 49 }, // Foongus
            new(30,03,2) { Species = 756, Ability = A4, Moves = new[]{ 402, 109, 605, 310 }, Index = 49 }, // Shiinotic
            new(30,03,2) { Species = 819, Ability = A4, Moves = new[]{ 747, 231, 371, 033 }, Index = 49 }, // Skwovet
            new(30,03,2) { Species = 820, Ability = A4, Moves = new[]{ 747, 231, 371, 034 }, Index = 49 }, // Greedent
            new(40,05,3) { Species = 420, Ability = A4, Moves = new[]{ 311, 572, 074, 402 }, Index = 49 }, // Cherubi
            new(40,05,3) { Species = 591, Ability = A4, Moves = new[]{ 078, 492, 092, 412 }, Index = 49 }, // Amoonguss
            new(40,05,3) { Species = 756, Ability = A4, Moves = new[]{ 402, 585, 605, 310 }, Index = 49 }, // Shiinotic
            new(40,05,3) { Species = 819, Ability = A4, Moves = new[]{ 747, 360, 371, 044 }, Index = 49 }, // Skwovet
            new(40,05,3) { Species = 820, Ability = A4, Moves = new[]{ 747, 360, 371, 424 }, Index = 49 }, // Greedent
            new(50,08,4) { Species = 420, Ability = A4, Moves = new[]{ 311, 572, 605, 402 }, Index = 49 }, // Cherubi
            new(50,08,4) { Species = 591, Ability = A4, Moves = new[]{ 188, 492, 092, 412 }, Index = 49 }, // Amoonguss
            new(50,08,4) { Species = 756, Ability = A4, Moves = new[]{ 412, 585, 605, 188 }, Index = 49 }, // Shiinotic
            new(50,08,4) { Species = 819, Ability = A4, Moves = new[]{ 747, 360, 371, 331 }, Index = 49 }, // Skwovet
            new(50,08,4) { Species = 820, Ability = A4, Moves = new[]{ 747, 360, 371, 424 }, Index = 49 }, // Greedent
            new(60,10,5) { Species = 420, Ability = A4, Moves = new[]{ 311, 572, 605, 412 }, Index = 49 }, // Cherubi
            new(60,10,5) { Species = 820, Ability = A4, Moves = new[]{ 747, 360, 371, 089 }, Index = 49, Shiny = Shiny.Always }, // Greedent
            new(60,10,5) { Species = 756, Ability = A4, Moves = new[]{ 147, 585, 605, 188 }, Index = 49 }, // Shiinotic
            new(60,10,5) { Species = 819, Ability = A4, Moves = new[]{ 747, 360, 371, 034 }, Index = 49 }, // Skwovet
            new(60,10,5) { Species = 819, Ability = A4, Moves = new[]{ 747, 360, 371, 034 }, Index = 49, Shiny = Shiny.Always }, // Skwovet
            new(60,10,5) { Species = 820, Ability = A4, Moves = new[]{ 747, 360, 371, 089 }, Index = 49 }, // Greedent

            new(17,01,1) { Species = 884, Ability = A4, Moves = new[]{ 232, 043, 468, 249 }, Index = 50, CanGigantamax = true }, // Duraludon
            new(17,01,1) { Species = 610, Ability = A4, Moves = new[]{ 044, 163, 372, 010 }, Index = 50 }, // Axew
            new(17,01,1) { Species = 704, Ability = A4, Moves = new[]{ 225, 352, 033, 175 }, Index = 50 }, // Goomy
            new(17,01,1) { Species = 446, Ability = A4, Moves = new[]{ 033, 044, 122, 111 }, Index = 50 }, // Munchlax
            new(17,01,1) { Species = 759, Ability = A4, Moves = new[]{ 693, 371, 608, 033 }, Index = 50 }, // Stufful
            new(17,01,1) { Species = 572, Ability = A4, Moves = new[]{ 497, 204, 402, 001 }, Index = 50 }, // Minccino
            new(30,03,2) { Species = 884, Ability = A4, Moves = new[]{ 232, 784, 468, 249 }, Index = 50, CanGigantamax = true }, // Duraludon
            new(30,03,2) { Species = 610, Ability = A4, Moves = new[]{ 337, 163, 242, 530 }, Index = 50 }, // Axew
            new(30,03,2) { Species = 704, Ability = A4, Moves = new[]{ 225, 352, 033, 341 }, Index = 50 }, // Goomy
            new(30,03,2) { Species = 143, Ability = A4, Moves = new[]{ 034, 242, 118, 111 }, Index = 50, CanGigantamax = true }, // Snorlax
            new(30,03,2) { Species = 759, Ability = A4, Moves = new[]{ 693, 371, 359, 036 }, Index = 50 }, // Stufful
            new(30,03,2) { Species = 572, Ability = A4, Moves = new[]{ 497, 231, 402, 129 }, Index = 50 }, // Minccino
            new(40,05,3) { Species = 884, Ability = A4, Moves = new[]{ 232, 525, 085, 249 }, Index = 50, CanGigantamax = true }, // Duraludon
            new(40,05,3) { Species = 611, Ability = A4, Moves = new[]{ 406, 231, 242, 530 }, Index = 50 }, // Fraxure
            new(40,05,3) { Species = 705, Ability = A4, Moves = new[]{ 406, 352, 491, 341 }, Index = 50 }, // Sliggoo
            new(40,05,3) { Species = 143, Ability = A4, Moves = new[]{ 034, 667, 242, 281 }, Index = 50, CanGigantamax = true }, // Snorlax
            new(40,05,3) { Species = 760, Ability = A4, Moves = new[]{ 693, 034, 359, 036 }, Index = 50 }, // Bewear
            new(40,05,3) { Species = 573, Ability = A4, Moves = new[]{ 331, 231, 350, 129 }, Index = 50 }, // Cinccino
            new(50,08,4) { Species = 884, Ability = A4, Moves = new[]{ 232, 406, 085, 776 }, Index = 50, CanGigantamax = true }, // Duraludon
            new(50,08,4) { Species = 612, Ability = A4, Moves = new[]{ 406, 231, 370, 530 }, Index = 50 }, // Haxorus
            new(50,08,4) { Species = 706, Ability = A4, Moves = new[]{ 406, 034, 491, 126 }, Index = 50 }, // Goodra
            new(50,08,4) { Species = 143, Ability = A4, Moves = new[]{ 034, 667, 280, 523 }, Index = 50, CanGigantamax = true }, // Snorlax
            new(50,08,4) { Species = 760, Ability = A4, Moves = new[]{ 663, 034, 359, 009 }, Index = 50 }, // Bewear
            new(50,08,4) { Species = 573, Ability = A4, Moves = new[]{ 331, 231, 350, 304 }, Index = 50 }, // Cinccino
            new(60,10,5) { Species = 884, Ability = A4, Moves = new[]{ 430, 406, 085, 776 }, Index = 50, CanGigantamax = true }, // Duraludon
            new(60,10,5) { Species = 612, Ability = A4, Moves = new[]{ 200, 231, 370, 089 }, Index = 50 }, // Haxorus
            new(60,10,5) { Species = 706, Ability = A4, Moves = new[]{ 406, 438, 482, 126 }, Index = 50 }, // Goodra
            new(60,10,5) { Species = 143, Ability = A4, Moves = new[]{ 034, 442, 242, 428 }, Index = 50, CanGigantamax = true }, // Snorlax
            new(60,10,5) { Species = 760, Ability = A4, Moves = new[]{ 663, 038, 276, 009 }, Index = 50 }, // Bewear
            new(60,10,5) { Species = 573, Ability = A4, Moves = new[]{ 402, 231, 350, 304 }, Index = 50 }, // Cinccino

            new(17,01,1) { Species = 128, Ability = A4, Moves = new[]{ 033, 157, 030, 371 }, Index = 51 }, // Tauros
            new(17,01,1) { Species = 626, Ability = A4, Moves = new[]{ 033, 030, 031, 523 }, Index = 51 }, // Bouffalant
            new(17,01,1) { Species = 241, Ability = A4, Moves = new[]{ 707, 033, 023, 205 }, Index = 51 }, // Miltank
            new(30,03,2) { Species = 128, Ability = A4, Moves = new[]{ 033, 157, 030, 370 }, Index = 51 }, // Tauros
            new(30,03,2) { Species = 626, Ability = A4, Moves = new[]{ 279, 030, 675, 523 }, Index = 51 }, // Bouffalant
            new(30,03,2) { Species = 241, Ability = A4, Moves = new[]{ 707, 428, 023, 205 }, Index = 51 }, // Miltank
            new(40,05,3) { Species = 128, Ability = A4, Moves = new[]{ 036, 157, 030, 370 }, Index = 51 }, // Tauros
            new(40,05,3) { Species = 626, Ability = A4, Moves = new[]{ 279, 543, 675, 523 }, Index = 51 }, // Bouffalant
            new(40,05,3) { Species = 241, Ability = A4, Moves = new[]{ 707, 428, 034, 205 }, Index = 51 }, // Miltank
            new(50,08,4) { Species = 128, Ability = A4, Moves = new[]{ 034, 157, 030, 370 }, Index = 51 }, // Tauros
            new(50,08,4) { Species = 626, Ability = A4, Moves = new[]{ 224, 543, 675, 523 }, Index = 51 }, // Bouffalant
            new(50,08,4) { Species = 241, Ability = A4, Moves = new[]{ 707, 428, 034, 583 }, Index = 51 }, // Miltank
            new(60,10,5) { Species = 128, Ability = A4, Moves = new[]{ 034, 157, 372, 370 }, Index = 51 }, // Tauros
            new(60,10,5) { Species = 128, Ability = A4, Moves = new[]{ 034, 157, 372, 370 }, Index = 51, Shiny = Shiny.Always }, // Tauros
            new(60,10,5) { Species = 626, Ability = A4, Moves = new[]{ 224, 543, 675, 089 }, Index = 51 }, // Bouffalant
            new(60,10,5) { Species = 241, Ability = A4, Moves = new[]{ 667, 428, 034, 583 }, Index = 51 }, // Miltank

            new(17,01,1) { Species = 884, Ability = A4, Moves = new[]{ 232, 043, 468, 249 }, Index = 52, CanGigantamax = true }, // Duraludon
            new(17,01,1) { Species = 610, Ability = A4, Moves = new[]{ 044, 163, 372, 010 }, Index = 52 }, // Axew
            new(17,01,1) { Species = 704, Ability = A4, Moves = new[]{ 225, 352, 033, 175 }, Index = 52 }, // Goomy
            new(17,01,1) { Species = 446, Ability = A4, Moves = new[]{ 033, 044, 122, 111 }, Index = 52 }, // Munchlax
            new(17,01,1) { Species = 759, Ability = A4, Moves = new[]{ 693, 371, 608, 033 }, Index = 52 }, // Stufful
            new(17,01,1) { Species = 572, Ability = A4, Moves = new[]{ 497, 204, 402, 001 }, Index = 52 }, // Minccino
            new(30,03,2) { Species = 884, Ability = A4, Moves = new[]{ 232, 784, 468, 249 }, Index = 52, CanGigantamax = true }, // Duraludon
            new(30,03,2) { Species = 610, Ability = A4, Moves = new[]{ 337, 163, 242, 530 }, Index = 52 }, // Axew
            new(30,03,2) { Species = 704, Ability = A4, Moves = new[]{ 225, 352, 033, 341 }, Index = 52 }, // Goomy
            new(30,03,2) { Species = 143, Ability = A4, Moves = new[]{ 034, 242, 118, 111 }, Index = 52, CanGigantamax = true }, // Snorlax
            new(30,03,2) { Species = 759, Ability = A4, Moves = new[]{ 693, 371, 359, 036 }, Index = 52 }, // Stufful
            new(30,03,2) { Species = 572, Ability = A4, Moves = new[]{ 497, 231, 402, 129 }, Index = 52 }, // Minccino
            new(40,05,3) { Species = 884, Ability = A4, Moves = new[]{ 232, 525, 085, 249 }, Index = 52, CanGigantamax = true }, // Duraludon
            new(40,05,3) { Species = 611, Ability = A4, Moves = new[]{ 406, 231, 242, 530 }, Index = 52 }, // Fraxure
            new(40,05,3) { Species = 705, Ability = A4, Moves = new[]{ 406, 352, 491, 341 }, Index = 52 }, // Sliggoo
            new(40,05,3) { Species = 143, Ability = A4, Moves = new[]{ 034, 667, 242, 281 }, Index = 52, CanGigantamax = true }, // Snorlax
            new(40,05,3) { Species = 760, Ability = A4, Moves = new[]{ 693, 034, 359, 036 }, Index = 52 }, // Bewear
            new(40,05,3) { Species = 573, Ability = A4, Moves = new[]{ 331, 231, 350, 129 }, Index = 52 }, // Cinccino
            new(50,08,4) { Species = 884, Ability = A4, Moves = new[]{ 232, 406, 085, 776 }, Index = 52, CanGigantamax = true }, // Duraludon
            new(50,08,4) { Species = 612, Ability = A4, Moves = new[]{ 406, 231, 370, 530 }, Index = 52 }, // Haxorus
            new(50,08,4) { Species = 706, Ability = A4, Moves = new[]{ 406, 034, 491, 126 }, Index = 52 }, // Goodra
            new(50,08,4) { Species = 143, Ability = A4, Moves = new[]{ 034, 667, 280, 523 }, Index = 52, CanGigantamax = true }, // Snorlax
            new(50,08,4) { Species = 760, Ability = A4, Moves = new[]{ 663, 034, 359, 009 }, Index = 52 }, // Bewear
            new(50,08,4) { Species = 573, Ability = A4, Moves = new[]{ 331, 231, 350, 304 }, Index = 52 }, // Cinccino
            new(60,10,5) { Species = 884, Ability = A4, Moves = new[]{ 430, 406, 085, 776 }, Index = 52, CanGigantamax = true }, // Duraludon
            new(60,10,5) { Species = 612, Ability = A4, Moves = new[]{ 200, 231, 370, 089 }, Index = 52 }, // Haxorus
            new(60,10,5) { Species = 706, Ability = A4, Moves = new[]{ 406, 438, 482, 126 }, Index = 52 }, // Goodra
            new(60,10,5) { Species = 143, Ability = A4, Moves = new[]{ 034, 442, 242, 428 }, Index = 52, CanGigantamax = true }, // Snorlax
            new(60,10,5) { Species = 760, Ability = A4, Moves = new[]{ 663, 038, 276, 009 }, Index = 52 }, // Bewear
            new(60,10,5) { Species = 573, Ability = A4, Moves = new[]{ 402, 231, 350, 304 }, Index = 52 }, // Cinccino

            new(17,01,1) { Species = 067, Ability = A4, Moves = new[]{ 279, 009, 490, 067 }, Index = 53 }, // Machoke
            new(17,01,1) { Species = 447, Ability = A4, Moves = new[]{ 280, 098, 317, 523 }, Index = 53 }, // Riolu
            new(17,01,1) { Species = 870, Ability = A4, Moves = new[]{ 442, 029, 249, 157 }, Index = 53 }, // Falinks
            new(17,01,1) { Species = 825, Ability = A4, Moves = new[]{ 522, 263, 371, 247 }, Index = 53 }, // Dottler
            new(17,01,1) { Species = 577, Ability = A4, Moves = new[]{ 060, 086, 360, 283 }, Index = 53 }, // Solosis
            new(17,01,1) { Species = 574, Ability = A4, Moves = new[]{ 060, 086, 412, 157 }, Index = 53 }, // Gothita
            new(30,03,2) { Species = 068, Ability = A4, Moves = new[]{ 279, 009, 233, 372 }, Index = 53, CanGigantamax = true }, // Machamp
            new(30,03,2) { Species = 447, Ability = A4, Moves = new[]{ 280, 232, 317, 523 }, Index = 53 }, // Riolu
            new(30,03,2) { Species = 870, Ability = A4, Moves = new[]{ 442, 029, 179, 157 }, Index = 53 }, // Falinks
            new(30,03,2) { Species = 826, Ability = A4, Moves = new[]{ 405, 277, 371, 109 }, Index = 53, CanGigantamax = true }, // Orbeetle
            new(30,03,2) { Species = 577, Ability = A4, Moves = new[]{ 473, 086, 360, 283 }, Index = 53 }, // Solosis
            new(30,03,2) { Species = 574, Ability = A4, Moves = new[]{ 473, 086, 412, 157 }, Index = 53 }, // Gothita
            new(40,05,3) { Species = 068, Ability = A4, Moves = new[]{ 530, 009, 233, 372 }, Index = 53, CanGigantamax = true }, // Machamp
            new(40,05,3) { Species = 448, Ability = A4, Moves = new[]{ 612, 232, 444, 089 }, Index = 53 }, // Lucario
            new(40,05,3) { Species = 870, Ability = A4, Moves = new[]{ 442, 029, 179, 317 }, Index = 53 }, // Falinks
            new(40,05,3) { Species = 826, Ability = A4, Moves = new[]{ 405, 277, 371, 247 }, Index = 53, CanGigantamax = true }, // Orbeetle
            new(40,05,3) { Species = 578, Ability = A4, Moves = new[]{ 094, 086, 360, 247 }, Index = 53 }, // Duosion
            new(40,05,3) { Species = 575, Ability = A4, Moves = new[]{ 094, 085, 412, 157 }, Index = 53 }, // Gothorita
            new(50,08,4) { Species = 068, Ability = A4, Moves = new[]{ 223, 009, 370, 372 }, Index = 53, CanGigantamax = true }, // Machamp
            new(50,08,4) { Species = 448, Ability = A4, Moves = new[]{ 612, 309, 444, 089 }, Index = 53 }, // Lucario
            new(50,08,4) { Species = 870, Ability = A4, Moves = new[]{ 442, 224, 179, 317 }, Index = 53 }, // Falinks
            new(50,08,4) { Species = 826, Ability = A4, Moves = new[]{ 405, 277, 776, 247 }, Index = 53, CanGigantamax = true }, // Orbeetle
            new(50,08,4) { Species = 579, Ability = A4, Moves = new[]{ 094, 009, 411, 247 }, Index = 53 }, // Reuniclus
            new(50,08,4) { Species = 576, Ability = A4, Moves = new[]{ 094, 085, 412, 322 }, Index = 53 }, // Gothitelle
            new(60,10,5) { Species = 068, Ability = A4, Moves = new[]{ 223, 009, 523, 372 }, Index = 53, CanGigantamax = true }, // Machamp
            new(60,10,5) { Species = 448, Ability = A4, Moves = new[]{ 370, 309, 444, 089 }, Index = 53 }, // Lucario
            new(60,10,5) { Species = 870, Ability = A4, Moves = new[]{ 442, 224, 370, 317 }, Index = 53 }, // Falinks
            new(60,10,5) { Species = 826, Ability = A4, Moves = new[]{ 405, 277, 776, 412 }, Index = 53, CanGigantamax = true }, // Orbeetle
            new(60,10,5) { Species = 579, Ability = A4, Moves = new[]{ 094, 087, 411, 247 }, Index = 53 }, // Reuniclus
            new(60,10,5) { Species = 576, Ability = A4, Moves = new[]{ 094, 085, 412, 322 }, Index = 53 }, // Gothitelle

            new(17,01,1) { Species = 845, Ability = A4, Moves = new[]{ 057, 056, 503, 000 }, Index = 54 }, // Cramorant
            new(17,01,1) { Species = 330, Ability = A4, Moves = new[]{ 225, 129, 693, 048 }, Index = 54 }, // Flygon
            new(17,01,1) { Species = 623, Ability = A4, Moves = new[]{ 089, 157, 707, 009 }, Index = 54 }, // Golurk
            new(17,01,1) { Species = 195, Ability = A4, Moves = new[]{ 055, 157, 281, 034 }, Index = 54 }, // Quagsire
            new(17,01,1) { Species = 876, Ability = A4, Moves = new[]{ 574, 129, 304, 000 }, Index = 54, Form = 1 }, // Indeedee-1
            new(30,03,2) { Species = 845, Ability = A4, Moves = new[]{ 057, 056, 503, 000 }, Index = 54 }, // Cramorant
            new(30,03,2) { Species = 330, Ability = A4, Moves = new[]{ 225, 129, 693, 048 }, Index = 54 }, // Flygon
            new(30,03,2) { Species = 623, Ability = A4, Moves = new[]{ 089, 157, 707, 009 }, Index = 54 }, // Golurk
            new(30,03,2) { Species = 195, Ability = A4, Moves = new[]{ 055, 157, 281, 034 }, Index = 54 }, // Quagsire
            new(30,03,2) { Species = 876, Ability = A4, Moves = new[]{ 574, 129, 304, 000 }, Index = 54, Form = 1 }, // Indeedee-1
            new(40,05,3) { Species = 845, Ability = A4, Moves = new[]{ 057, 056, 503, 000 }, Index = 54 }, // Cramorant
            new(40,05,3) { Species = 330, Ability = A4, Moves = new[]{ 225, 129, 693, 586 }, Index = 54 }, // Flygon
            new(40,05,3) { Species = 623, Ability = A4, Moves = new[]{ 089, 157, 523, 009 }, Index = 54 }, // Golurk
            new(40,05,3) { Species = 195, Ability = A4, Moves = new[]{ 330, 157, 281, 034 }, Index = 54 }, // Quagsire
            new(40,05,3) { Species = 876, Ability = A4, Moves = new[]{ 574, 129, 304, 000 }, Index = 54, Form = 1 }, // Indeedee-1
            new(50,08,4) { Species = 845, Ability = A4, Moves = new[]{ 057, 056, 503, 000 }, Index = 54 }, // Cramorant
            new(50,08,4) { Species = 330, Ability = A4, Moves = new[]{ 225, 129, 693, 586 }, Index = 54 }, // Flygon
            new(50,08,4) { Species = 623, Ability = A4, Moves = new[]{ 089, 157, 523, 009 }, Index = 54 }, // Golurk
            new(50,08,4) { Species = 195, Ability = A4, Moves = new[]{ 330, 157, 281, 034 }, Index = 54 }, // Quagsire
            new(50,08,4) { Species = 876, Ability = A4, Moves = new[]{ 574, 129, 304, 000 }, Index = 54, Form = 1 }, // Indeedee-1
            new(60,10,5) { Species = 845, Ability = A4, Moves = new[]{ 057, 056, 503, 000 }, Index = 54 }, // Cramorant
            new(60,10,5) { Species = 845, Ability = A4, Moves = new[]{ 057, 056, 503, 000 }, Index = 54, Shiny = Shiny.Always }, // Cramorant
            new(60,10,5) { Species = 330, Ability = A4, Moves = new[]{ 225, 129, 693, 586 }, Index = 54 }, // Flygon
            new(60,10,5) { Species = 623, Ability = A4, Moves = new[]{ 089, 157, 523, 009 }, Index = 54 }, // Golurk
            new(60,10,5) { Species = 195, Ability = A4, Moves = new[]{ 330, 157, 281, 034 }, Index = 54 }, // Quagsire
            new(60,10,5) { Species = 876, Ability = A4, Moves = new[]{ 574, 129, 304, 000 }, Index = 54, Form = 1 }, // Indeedee-1

            new(17,01,1) { Species = 067, Ability = A4, Moves = new[]{ 279, 009, 490, 067 }, Index = 55 }, // Machoke
            new(17,01,1) { Species = 447, Ability = A4, Moves = new[]{ 280, 098, 317, 523 }, Index = 55 }, // Riolu
            new(17,01,1) { Species = 870, Ability = A4, Moves = new[]{ 442, 029, 249, 157 }, Index = 55 }, // Falinks
            new(17,01,1) { Species = 825, Ability = A4, Moves = new[]{ 522, 263, 371, 247 }, Index = 55 }, // Dottler
            new(17,01,1) { Species = 577, Ability = A4, Moves = new[]{ 060, 086, 360, 283 }, Index = 55 }, // Solosis
            new(17,01,1) { Species = 574, Ability = A4, Moves = new[]{ 060, 086, 412, 157 }, Index = 55 }, // Gothita
            new(30,03,2) { Species = 068, Ability = A4, Moves = new[]{ 279, 009, 233, 372 }, Index = 55, CanGigantamax = true }, // Machamp
            new(30,03,2) { Species = 447, Ability = A4, Moves = new[]{ 280, 232, 317, 523 }, Index = 55 }, // Riolu
            new(30,03,2) { Species = 870, Ability = A4, Moves = new[]{ 442, 029, 179, 157 }, Index = 55 }, // Falinks
            new(30,03,2) { Species = 826, Ability = A4, Moves = new[]{ 405, 277, 371, 109 }, Index = 55, CanGigantamax = true }, // Orbeetle
            new(30,03,2) { Species = 577, Ability = A4, Moves = new[]{ 473, 086, 360, 283 }, Index = 55 }, // Solosis
            new(30,03,2) { Species = 574, Ability = A4, Moves = new[]{ 473, 086, 412, 157 }, Index = 55 }, // Gothita
            new(40,05,3) { Species = 068, Ability = A4, Moves = new[]{ 530, 009, 233, 372 }, Index = 55, CanGigantamax = true }, // Machamp
            new(40,05,3) { Species = 448, Ability = A4, Moves = new[]{ 612, 232, 444, 089 }, Index = 55 }, // Lucario
            new(40,05,3) { Species = 870, Ability = A4, Moves = new[]{ 442, 029, 179, 317 }, Index = 55 }, // Falinks
            new(40,05,3) { Species = 826, Ability = A4, Moves = new[]{ 405, 277, 371, 247 }, Index = 55, CanGigantamax = true }, // Orbeetle
            new(40,05,3) { Species = 578, Ability = A4, Moves = new[]{ 094, 086, 360, 247 }, Index = 55 }, // Duosion
            new(40,05,3) { Species = 575, Ability = A4, Moves = new[]{ 094, 085, 412, 157 }, Index = 55 }, // Gothorita
            new(50,08,4) { Species = 068, Ability = A4, Moves = new[]{ 223, 009, 370, 372 }, Index = 55, CanGigantamax = true }, // Machamp
            new(50,08,4) { Species = 448, Ability = A4, Moves = new[]{ 612, 309, 444, 089 }, Index = 55 }, // Lucario
            new(50,08,4) { Species = 870, Ability = A4, Moves = new[]{ 442, 224, 179, 317 }, Index = 55 }, // Falinks
            new(50,08,4) { Species = 826, Ability = A4, Moves = new[]{ 405, 277, 776, 247 }, Index = 55, CanGigantamax = true }, // Orbeetle
            new(50,08,4) { Species = 579, Ability = A4, Moves = new[]{ 094, 009, 411, 247 }, Index = 55 }, // Reuniclus
            new(50,08,4) { Species = 576, Ability = A4, Moves = new[]{ 094, 085, 412, 322 }, Index = 55 }, // Gothitelle
            new(60,10,5) { Species = 068, Ability = A4, Moves = new[]{ 223, 009, 523, 372 }, Index = 55, CanGigantamax = true }, // Machamp
            new(60,10,5) { Species = 448, Ability = A4, Moves = new[]{ 370, 309, 444, 089 }, Index = 55 }, // Lucario
            new(60,10,5) { Species = 870, Ability = A4, Moves = new[]{ 442, 224, 370, 317 }, Index = 55 }, // Falinks
            new(60,10,5) { Species = 826, Ability = A4, Moves = new[]{ 405, 277, 776, 412 }, Index = 55, CanGigantamax = true }, // Orbeetle
            new(60,10,5) { Species = 579, Ability = A4, Moves = new[]{ 094, 087, 411, 247 }, Index = 55 }, // Reuniclus
            new(60,10,5) { Species = 576, Ability = A4, Moves = new[]{ 094, 085, 412, 322 }, Index = 55 }, // Gothitelle

            new(17,01,1) { Species = 868, Ability = A4, Moves = new[]{ 033, 186, 577, 496 }, Index = 56, CanGigantamax = true }, // Milcery
            new(30,03,2) { Species = 868, Ability = A4, Moves = new[]{ 577, 186, 263, 500 }, Index = 56, CanGigantamax = true }, // Milcery
            new(40,05,3) { Species = 868, Ability = A4, Moves = new[]{ 577, 605, 496, 213 }, Index = 56, CanGigantamax = true }, // Milcery
            new(50,08,4) { Species = 868, Ability = A4, Moves = new[]{ 577, 605, 496, 500 }, Index = 56, CanGigantamax = true }, // Milcery
            new(60,10,5) { Species = 868, Ability = A4, Moves = new[]{ 577, 605, 496, 500 }, Index = 56, CanGigantamax = true, Shiny = Shiny.Always }, // Milcery
            new(60,10,5) { Species = 868, Ability = A4, Moves = new[]{ 577, 605, 496, 500 }, Index = 56, CanGigantamax = true }, // Milcery

            new(17,01,1) { Species = 067, Ability = A4, Moves = new[]{ 279, 009, 490, 067 }, Index = 57 }, // Machoke
            new(17,01,1) { Species = 447, Ability = A4, Moves = new[]{ 280, 098, 317, 523 }, Index = 57 }, // Riolu
            new(17,01,1) { Species = 870, Ability = A4, Moves = new[]{ 442, 029, 249, 157 }, Index = 57 }, // Falinks
            new(17,01,1) { Species = 825, Ability = A4, Moves = new[]{ 522, 263, 371, 247 }, Index = 57 }, // Dottler
            new(17,01,1) { Species = 577, Ability = A4, Moves = new[]{ 060, 086, 360, 283 }, Index = 57 }, // Solosis
            new(17,01,1) { Species = 574, Ability = A4, Moves = new[]{ 060, 086, 412, 157 }, Index = 57 }, // Gothita
            new(30,03,2) { Species = 068, Ability = A4, Moves = new[]{ 279, 009, 233, 372 }, Index = 57, CanGigantamax = true }, // Machamp
            new(30,03,2) { Species = 447, Ability = A4, Moves = new[]{ 280, 232, 317, 523 }, Index = 57 }, // Riolu
            new(30,03,2) { Species = 870, Ability = A4, Moves = new[]{ 442, 029, 179, 157 }, Index = 57 }, // Falinks
            new(30,03,2) { Species = 826, Ability = A4, Moves = new[]{ 405, 277, 371, 109 }, Index = 57, CanGigantamax = true }, // Orbeetle
            new(30,03,2) { Species = 577, Ability = A4, Moves = new[]{ 473, 086, 360, 283 }, Index = 57 }, // Solosis
            new(30,03,2) { Species = 574, Ability = A4, Moves = new[]{ 473, 086, 412, 157 }, Index = 57 }, // Gothita
            new(40,05,3) { Species = 068, Ability = A4, Moves = new[]{ 530, 009, 233, 372 }, Index = 57, CanGigantamax = true }, // Machamp
            new(40,05,3) { Species = 448, Ability = A4, Moves = new[]{ 612, 232, 444, 089 }, Index = 57 }, // Lucario
            new(40,05,3) { Species = 870, Ability = A4, Moves = new[]{ 442, 029, 179, 317 }, Index = 57 }, // Falinks
            new(40,05,3) { Species = 826, Ability = A4, Moves = new[]{ 405, 277, 371, 247 }, Index = 57, CanGigantamax = true }, // Orbeetle
            new(40,05,3) { Species = 578, Ability = A4, Moves = new[]{ 094, 086, 360, 247 }, Index = 57 }, // Duosion
            new(40,05,3) { Species = 575, Ability = A4, Moves = new[]{ 094, 085, 412, 157 }, Index = 57 }, // Gothorita
            new(50,08,4) { Species = 068, Ability = A4, Moves = new[]{ 223, 009, 370, 372 }, Index = 57, CanGigantamax = true }, // Machamp
            new(50,08,4) { Species = 448, Ability = A4, Moves = new[]{ 612, 309, 444, 089 }, Index = 57 }, // Lucario
            new(50,08,4) { Species = 870, Ability = A4, Moves = new[]{ 442, 224, 179, 317 }, Index = 57 }, // Falinks
            new(50,08,4) { Species = 826, Ability = A4, Moves = new[]{ 405, 277, 776, 247 }, Index = 57, CanGigantamax = true }, // Orbeetle
            new(50,08,4) { Species = 579, Ability = A4, Moves = new[]{ 094, 009, 411, 247 }, Index = 57 }, // Reuniclus
            new(50,08,4) { Species = 576, Ability = A4, Moves = new[]{ 094, 085, 412, 322 }, Index = 57 }, // Gothitelle
            new(60,10,5) { Species = 068, Ability = A4, Moves = new[]{ 223, 009, 523, 372 }, Index = 57, CanGigantamax = true }, // Machamp
            new(60,10,5) { Species = 448, Ability = A4, Moves = new[]{ 370, 309, 444, 089 }, Index = 57 }, // Lucario
            new(60,10,5) { Species = 870, Ability = A4, Moves = new[]{ 442, 224, 370, 317 }, Index = 57 }, // Falinks
            new(60,10,5) { Species = 826, Ability = A4, Moves = new[]{ 405, 277, 776, 412 }, Index = 57, CanGigantamax = true }, // Orbeetle
            new(60,10,5) { Species = 579, Ability = A4, Moves = new[]{ 094, 087, 411, 247 }, Index = 57 }, // Reuniclus
            new(60,10,5) { Species = 576, Ability = A4, Moves = new[]{ 094, 085, 412, 322 }, Index = 57 }, // Gothitelle

            new(17,01,1) { Species = 067, Ability = A4, Moves = new[]{ 279, 009, 490, 067 }, Index = 58 }, // Machoke
            new(17,01,1) { Species = 447, Ability = A4, Moves = new[]{ 280, 098, 317, 523 }, Index = 58 }, // Riolu
            new(17,01,1) { Species = 870, Ability = A4, Moves = new[]{ 442, 029, 249, 157 }, Index = 58 }, // Falinks
            new(17,01,1) { Species = 825, Ability = A4, Moves = new[]{ 522, 263, 371, 247 }, Index = 58 }, // Dottler
            new(17,01,1) { Species = 577, Ability = A4, Moves = new[]{ 060, 086, 360, 283 }, Index = 58 }, // Solosis
            new(17,01,1) { Species = 574, Ability = A4, Moves = new[]{ 060, 086, 412, 157 }, Index = 58 }, // Gothita
            new(30,03,2) { Species = 068, Ability = A4, Moves = new[]{ 279, 009, 233, 372 }, Index = 58, CanGigantamax = true }, // Machamp
            new(30,03,2) { Species = 447, Ability = A4, Moves = new[]{ 280, 232, 317, 523 }, Index = 58 }, // Riolu
            new(30,03,2) { Species = 870, Ability = A4, Moves = new[]{ 442, 029, 179, 157 }, Index = 58 }, // Falinks
            new(30,03,2) { Species = 826, Ability = A4, Moves = new[]{ 405, 277, 371, 109 }, Index = 58, CanGigantamax = true }, // Orbeetle
            new(30,03,2) { Species = 577, Ability = A4, Moves = new[]{ 473, 086, 360, 283 }, Index = 58 }, // Solosis
            new(30,03,2) { Species = 574, Ability = A4, Moves = new[]{ 473, 086, 412, 157 }, Index = 58 }, // Gothita
            new(40,05,3) { Species = 068, Ability = A4, Moves = new[]{ 530, 009, 233, 372 }, Index = 58, CanGigantamax = true }, // Machamp
            new(40,05,3) { Species = 448, Ability = A4, Moves = new[]{ 612, 232, 444, 089 }, Index = 58 }, // Lucario
            new(40,05,3) { Species = 870, Ability = A4, Moves = new[]{ 442, 029, 179, 317 }, Index = 58 }, // Falinks
            new(40,05,3) { Species = 826, Ability = A4, Moves = new[]{ 405, 277, 371, 247 }, Index = 58, CanGigantamax = true }, // Orbeetle
            new(40,05,3) { Species = 578, Ability = A4, Moves = new[]{ 094, 086, 360, 247 }, Index = 58 }, // Duosion
            new(40,05,3) { Species = 575, Ability = A4, Moves = new[]{ 094, 085, 412, 157 }, Index = 58 }, // Gothorita
            new(50,08,4) { Species = 068, Ability = A4, Moves = new[]{ 223, 009, 370, 372 }, Index = 58, CanGigantamax = true }, // Machamp
            new(50,08,4) { Species = 448, Ability = A4, Moves = new[]{ 612, 309, 444, 089 }, Index = 58 }, // Lucario
            new(50,08,4) { Species = 870, Ability = A4, Moves = new[]{ 442, 224, 179, 317 }, Index = 58 }, // Falinks
            new(50,08,4) { Species = 826, Ability = A4, Moves = new[]{ 405, 277, 776, 247 }, Index = 58, CanGigantamax = true }, // Orbeetle
            new(50,08,4) { Species = 579, Ability = A4, Moves = new[]{ 094, 009, 411, 247 }, Index = 58 }, // Reuniclus
            new(50,08,4) { Species = 576, Ability = A4, Moves = new[]{ 094, 085, 412, 322 }, Index = 58 }, // Gothitelle
            new(60,10,5) { Species = 025, Ability = A4, Moves = new[]{ 804, 435, 057, 574 }, Index = 58, CanGigantamax = true, Shiny = Shiny.Always }, // Pikachu
            new(60,10,5) { Species = 025, Ability = A4, Moves = new[]{ 344, 280, 583, 231 }, Index = 58, CanGigantamax = true, Shiny = Shiny.Always }, // Pikachu
            new(60,10,5) { Species = 025, Ability = A4, Moves = new[]{ 804, 435, 057, 574 }, Index = 58, CanGigantamax = true }, // Pikachu
            new(60,10,5) { Species = 025, Ability = A4, Moves = new[]{ 344, 280, 583, 231 }, Index = 58, CanGigantamax = true }, // Pikachu

            new(17,01,1) { Species = 856, Ability = A4, Moves = new[]{ 312, 093, 574, 595 }, Index = 59 }, // Hatenna
            new(17,01,1) { Species = 280, Ability = A4, Moves = new[]{ 574, 093, 104, 045 }, Index = 59 }, // Ralts
            new(17,01,1) { Species = 109, Ability = A4, Moves = new[]{ 499, 053, 124, 372 }, Index = 59 }, // Koffing
            new(17,01,1) { Species = 821, Ability = A4, Moves = new[]{ 403, 031, 043, 681 }, Index = 59 }, // Rookidee
            new(17,01,1) { Species = 627, Ability = A4, Moves = new[]{ 043, 276, 017, 064 }, Index = 59 }, // Rufflet
            new(17,01,1) { Species = 845, Ability = A4, Moves = new[]{ 055, 254, 064, 562 }, Index = 59 }, // Cramorant
            new(30,03,2) { Species = 856, Ability = A4, Moves = new[]{ 605, 060, 574, 595 }, Index = 59 }, // Hatenna
            new(30,03,2) { Species = 281, Ability = A4, Moves = new[]{ 574, 060, 104, 085 }, Index = 59 }, // Kirlia
            new(30,03,2) { Species = 109, Ability = A4, Moves = new[]{ 499, 053, 482, 372 }, Index = 59 }, // Koffing
            new(30,03,2) { Species = 822, Ability = A4, Moves = new[]{ 403, 263, 279, 681 }, Index = 59 }, // Corvisquire
            new(30,03,2) { Species = 627, Ability = A4, Moves = new[]{ 184, 276, 017, 157 }, Index = 59 }, // Rufflet
            new(30,03,2) { Species = 845, Ability = A4, Moves = new[]{ 055, 058, 064, 562 }, Index = 59 }, // Cramorant
            new(40,05,3) { Species = 857, Ability = A4, Moves = new[]{ 605, 060, 595, 574 }, Index = 59, CanGigantamax = true }, // Hattrem
            new(40,05,3) { Species = 282, Ability = A4, Moves = new[]{ 585, 060, 595, 085 }, Index = 59 }, // Gardevoir
            new(40,05,3) { Species = 110, Ability = A4, Moves = new[]{ 790, 053, 482, 372 }, Index = 59, Form = 1 }, // Weezing-1
            new(40,05,3) { Species = 823, Ability = A4, Moves = new[]{ 403, 442, 034, 681 }, Index = 59, CanGigantamax = true }, // Corviknight
            new(40,05,3) { Species = 628, Ability = A4, Moves = new[]{ 403, 276, 163, 157 }, Index = 59 }, // Braviary
            new(40,05,3) { Species = 845, Ability = A4, Moves = new[]{ 503, 058, 403, 065 }, Index = 59 }, // Cramorant
            new(50,08,4) { Species = 858, Ability = A4, Moves = new[]{ 605, 094, 595, 247 }, Index = 59, CanGigantamax = true }, // Hatterene
            new(50,08,4) { Species = 282, Ability = A4, Moves = new[]{ 585, 094, 595, 085 }, Index = 59 }, // Gardevoir
            new(50,08,4) { Species = 110, Ability = A4, Moves = new[]{ 790, 126, 482, 372 }, Index = 59, Form = 1 }, // Weezing-1
            new(50,08,4) { Species = 823, Ability = A4, Moves = new[]{ 413, 442, 034, 681 }, Index = 59, CanGigantamax = true }, // Corviknight
            new(50,08,4) { Species = 628, Ability = A4, Moves = new[]{ 403, 276, 442, 157 }, Index = 59 }, // Braviary
            new(50,08,4) { Species = 845, Ability = A4, Moves = new[]{ 056, 058, 403, 065 }, Index = 59 }, // Cramorant
            new(60,10,5) { Species = 858, Ability = A4, Moves = new[]{ 605, 094, 438, 247 }, Index = 59, CanGigantamax = true }, // Hatterene
            new(60,10,5) { Species = 282, Ability = A4, Moves = new[]{ 585, 094, 261, 085 }, Index = 59 }, // Gardevoir
            new(60,10,5) { Species = 110, Ability = A4, Moves = new[]{ 790, 126, 482, 399 }, Index = 59, Form = 1 }, // Weezing-1
            new(60,10,5) { Species = 823, Ability = A4, Moves = new[]{ 413, 442, 776, 372 }, Index = 59, CanGigantamax = true }, // Corviknight
            new(60,10,5) { Species = 628, Ability = A4, Moves = new[]{ 413, 370, 442, 157 }, Index = 59 }, // Braviary
            new(60,10,5) { Species = 845, Ability = A4, Moves = new[]{ 056, 058, 403, 057 }, Index = 59 }, // Cramorant

            new(17,01,1) { Species = 043, Ability = A4, Moves = new[]{ 331, 236, 051, 074 }, Index = 60 }, // Oddish
            new(17,01,1) { Species = 420, Ability = A4, Moves = new[]{ 234, 572, 670, 033 }, Index = 60 }, // Cherubi
            new(17,01,1) { Species = 549, Ability = A4, Moves = new[]{ 412, 298, 345, 263 }, Index = 60 }, // Lilligant
            new(17,01,1) { Species = 753, Ability = A4, Moves = new[]{ 210, 074, 075, 275 }, Index = 60 }, // Fomantis
            new(17,01,1) { Species = 764, Ability = A4, Moves = new[]{ 345, 579, 035, 074 }, Index = 60 }, // Comfey
            new(30,03,2) { Species = 045, Ability = A4, Moves = new[]{ 572, 236, 051, 496 }, Index = 60 }, // Vileplume
            new(30,03,2) { Species = 182, Ability = A4, Moves = new[]{ 572, 483, 074, 605 }, Index = 60 }, // Bellossom
            new(30,03,2) { Species = 421, Ability = A4, Moves = new[]{ 579, 345, 670, 033 }, Index = 60 }, // Cherrim
            new(30,03,2) { Species = 549, Ability = A4, Moves = new[]{ 412, 298, 345, 263 }, Index = 60 }, // Lilligant
            new(30,03,2) { Species = 753, Ability = A4, Moves = new[]{ 210, 163, 075, 230 }, Index = 60 }, // Fomantis
            new(30,03,2) { Species = 764, Ability = A4, Moves = new[]{ 345, 579, 035, 583 }, Index = 60 }, // Comfey
            new(40,05,3) { Species = 045, Ability = A4, Moves = new[]{ 572, 585, 051, 496 }, Index = 60 }, // Vileplume
            new(40,05,3) { Species = 182, Ability = A4, Moves = new[]{ 572, 483, 077, 605 }, Index = 60 }, // Bellossom
            new(40,05,3) { Species = 421, Ability = A4, Moves = new[]{ 579, 345, 583, 036 }, Index = 60 }, // Cherrim
            new(40,05,3) { Species = 549, Ability = A4, Moves = new[]{ 572, 298, 345, 483 }, Index = 60 }, // Lilligant
            new(40,05,3) { Species = 754, Ability = A4, Moves = new[]{ 210, 572, 400, 530 }, Index = 60 }, // Lurantis
            new(40,05,3) { Species = 764, Ability = A4, Moves = new[]{ 572, 579, 035, 583 }, Index = 60 }, // Comfey
            new(50,08,4) { Species = 045, Ability = A4, Moves = new[]{ 572, 585, 092, 496 }, Index = 60 }, // Vileplume
            new(50,08,4) { Species = 182, Ability = A4, Moves = new[]{ 080, 483, 051, 605 }, Index = 60 }, // Bellossom
            new(50,08,4) { Species = 421, Ability = A4, Moves = new[]{ 579, 572, 583, 676 }, Index = 60 }, // Cherrim
            new(50,08,4) { Species = 549, Ability = A4, Moves = new[]{ 572, 298, 241, 676 }, Index = 60 }, // Lilligant
            new(50,08,4) { Species = 754, Ability = A4, Moves = new[]{ 404, 572, 400, 530 }, Index = 60 }, // Lurantis
            new(50,08,4) { Species = 764, Ability = A4, Moves = new[]{ 572, 579, 035, 583 }, Index = 60 }, // Comfey
            new(60,10,5) { Species = 045, Ability = A4, Moves = new[]{ 572, 585, 092, 034 }, Index = 60 }, // Vileplume
            new(60,10,5) { Species = 549, Ability = A4, Moves = new[]{ 080, 298, 241, 676 }, Index = 60, Shiny = Shiny.Always }, // Lilligant
            new(60,10,5) { Species = 421, Ability = A4, Moves = new[]{ 579, 572, 605, 676 }, Index = 60 }, // Cherrim
            new(60,10,5) { Species = 549, Ability = A4, Moves = new[]{ 080, 298, 241, 676 }, Index = 60 }, // Lilligant
            new(60,10,5) { Species = 754, Ability = A4, Moves = new[]{ 404, 572, 398, 530 }, Index = 60 }, // Lurantis
            new(60,10,5) { Species = 764, Ability = A4, Moves = new[]{ 572, 579, 447, 583 }, Index = 60 }, // Comfey

            new(17,01,1) { Species = 856, Ability = A4, Moves = new[]{ 312, 093, 574, 595 }, Index = 61 }, // Hatenna
            new(17,01,1) { Species = 280, Ability = A4, Moves = new[]{ 574, 093, 104, 045 }, Index = 61 }, // Ralts
            new(17,01,1) { Species = 109, Ability = A4, Moves = new[]{ 499, 053, 124, 372 }, Index = 61 }, // Koffing
            new(17,01,1) { Species = 821, Ability = A4, Moves = new[]{ 403, 031, 043, 681 }, Index = 61 }, // Rookidee
            new(17,01,1) { Species = 627, Ability = A4, Moves = new[]{ 043, 276, 017, 064 }, Index = 61 }, // Rufflet
            new(17,01,1) { Species = 845, Ability = A4, Moves = new[]{ 055, 254, 064, 562 }, Index = 61 }, // Cramorant
            new(30,03,2) { Species = 856, Ability = A4, Moves = new[]{ 605, 060, 574, 595 }, Index = 61 }, // Hatenna
            new(30,03,2) { Species = 281, Ability = A4, Moves = new[]{ 574, 060, 104, 085 }, Index = 61 }, // Kirlia
            new(30,03,2) { Species = 109, Ability = A4, Moves = new[]{ 499, 053, 482, 372 }, Index = 61 }, // Koffing
            new(30,03,2) { Species = 822, Ability = A4, Moves = new[]{ 403, 263, 279, 681 }, Index = 61 }, // Corvisquire
            new(30,03,2) { Species = 627, Ability = A4, Moves = new[]{ 184, 276, 017, 157 }, Index = 61 }, // Rufflet
            new(30,03,2) { Species = 845, Ability = A4, Moves = new[]{ 055, 058, 064, 562 }, Index = 61 }, // Cramorant
            new(40,05,3) { Species = 857, Ability = A4, Moves = new[]{ 605, 060, 595, 574 }, Index = 61, CanGigantamax = true }, // Hattrem
            new(40,05,3) { Species = 282, Ability = A4, Moves = new[]{ 585, 060, 595, 085 }, Index = 61 }, // Gardevoir
            new(40,05,3) { Species = 110, Ability = A4, Moves = new[]{ 790, 053, 482, 372 }, Index = 61, Form = 1 }, // Weezing-1
            new(40,05,3) { Species = 823, Ability = A4, Moves = new[]{ 403, 442, 034, 681 }, Index = 61, CanGigantamax = true }, // Corviknight
            new(40,05,3) { Species = 628, Ability = A4, Moves = new[]{ 403, 276, 163, 157 }, Index = 61 }, // Braviary
            new(40,05,3) { Species = 845, Ability = A4, Moves = new[]{ 503, 058, 403, 065 }, Index = 61 }, // Cramorant
            new(50,08,4) { Species = 858, Ability = A4, Moves = new[]{ 605, 094, 595, 247 }, Index = 61, CanGigantamax = true }, // Hatterene
            new(50,08,4) { Species = 282, Ability = A4, Moves = new[]{ 585, 094, 595, 085 }, Index = 61 }, // Gardevoir
            new(50,08,4) { Species = 110, Ability = A4, Moves = new[]{ 790, 126, 482, 372 }, Index = 61, Form = 1 }, // Weezing-1
            new(50,08,4) { Species = 823, Ability = A4, Moves = new[]{ 413, 442, 034, 681 }, Index = 61, CanGigantamax = true }, // Corviknight
            new(50,08,4) { Species = 628, Ability = A4, Moves = new[]{ 403, 276, 442, 157 }, Index = 61 }, // Braviary
            new(50,08,4) { Species = 845, Ability = A4, Moves = new[]{ 056, 058, 403, 065 }, Index = 61 }, // Cramorant
            new(60,10,5) { Species = 858, Ability = A4, Moves = new[]{ 605, 094, 438, 247 }, Index = 61, CanGigantamax = true }, // Hatterene
            new(60,10,5) { Species = 282, Ability = A4, Moves = new[]{ 585, 094, 261, 085 }, Index = 61 }, // Gardevoir
            new(60,10,5) { Species = 110, Ability = A4, Moves = new[]{ 790, 126, 482, 399 }, Index = 61, Form = 1 }, // Weezing-1
            new(60,10,5) { Species = 823, Ability = A4, Moves = new[]{ 413, 442, 776, 372 }, Index = 61, CanGigantamax = true }, // Corviknight
            new(60,10,5) { Species = 628, Ability = A4, Moves = new[]{ 413, 370, 442, 157 }, Index = 61 }, // Braviary
            new(60,10,5) { Species = 845, Ability = A4, Moves = new[]{ 056, 058, 403, 057 }, Index = 61 }, // Cramorant

          //new(17,01,1) { Species = 129, Ability = A4, Moves = new[]{ 150, 000, 000, 000 }, Index = 62 }, // Magikarp
          //new(30,03,2) { Species = 129, Ability = A4, Moves = new[]{ 150, 000, 000, 000 }, Index = 62 }, // Magikarp
          //new(40,05,3) { Species = 129, Ability = A4, Moves = new[]{ 150, 000, 000, 000 }, Index = 62 }, // Magikarp
          //new(50,08,4) { Species = 129, Ability = A4, Moves = new[]{ 150, 000, 000, 000 }, Index = 62 }, // Magikarp
          //new(60,10,5) { Species = 129, Ability = A4, Moves = new[]{ 150, 000, 000, 000 }, Index = 62 }, // Magikarp

            new(17,01,1) { Species = 183, Ability = A4, Moves = new[]{ 061, 204, 111, 205 }, Index = 63 }, // Marill
            new(17,01,1) { Species = 427, Ability = A4, Moves = new[]{ 098, 608, 150, 111 }, Index = 63 }, // Buneary
            new(17,01,1) { Species = 659, Ability = A4, Moves = new[]{ 098, 189, 280, 341 }, Index = 63 }, // Bunnelby
            new(30,03,2) { Species = 184, Ability = A4, Moves = new[]{ 401, 583, 280, 205 }, Index = 63 }, // Azumarill
            new(30,03,2) { Species = 428, Ability = A4, Moves = new[]{ 024, 204, 029, 111 }, Index = 63 }, // Lopunny
            new(30,03,2) { Species = 660, Ability = A4, Moves = new[]{ 098, 523, 280, 341 }, Index = 63 }, // Diggersby
            new(40,05,3) { Species = 184, Ability = A4, Moves = new[]{ 056, 583, 280, 205 }, Index = 63 }, // Azumarill
            new(40,05,3) { Species = 428, Ability = A4, Moves = new[]{ 024, 204, 029, 129 }, Index = 63 }, // Lopunny
            new(40,05,3) { Species = 660, Ability = A4, Moves = new[]{ 005, 523, 280, 036 }, Index = 63 }, // Diggersby
            new(50,08,4) { Species = 184, Ability = A4, Moves = new[]{ 710, 583, 276, 205 }, Index = 63 }, // Azumarill
            new(50,08,4) { Species = 428, Ability = A4, Moves = new[]{ 024, 583, 025, 129 }, Index = 63 }, // Lopunny
            new(50,08,4) { Species = 660, Ability = A4, Moves = new[]{ 005, 089, 280, 162 }, Index = 63 }, // Diggersby
            new(60,10,5) { Species = 184, Ability = A4, Moves = new[]{ 710, 583, 276, 523 }, Index = 63 }, // Azumarill
            new(60,10,5) { Species = 184, Ability = A4, Moves = new[]{ 710, 583, 276, 523 }, Index = 63, Shiny = Shiny.Always }, // Azumarill
            new(60,10,5) { Species = 428, Ability = A4, Moves = new[]{ 136, 583, 025, 693 }, Index = 63 }, // Lopunny
            new(60,10,5) { Species = 660, Ability = A4, Moves = new[]{ 416, 089, 359, 162 }, Index = 63 }, // Diggersby
          //new(60,10,5) { Species = 815, Ability = A2, Moves = new[]{ 780, 442, 279, 555 }, Index = 63, CanGigantamax = true }, // Cinderace

            new(17,01,1) { Species = 132, Ability = A4, Moves = new[]{ 144, 000, 000, 000 }, Index = 64 }, // Ditto
            new(17,01,1) { Species = 821, Ability = A4, Moves = new[]{ 403, 031, 043, 681 }, Index = 64 }, // Rookidee
            new(17,01,1) { Species = 833, Ability = A4, Moves = new[]{ 055, 033, 044, 240 }, Index = 64 }, // Chewtle
            new(17,01,1) { Species = 824, Ability = A4, Moves = new[]{ 522, 000, 000, 000 }, Index = 64 }, // Blipbug
            new(17,01,1) { Species = 850, Ability = A4, Moves = new[]{ 172, 044, 035, 052 }, Index = 64 }, // Sizzlipede
            new(17,01,1) { Species = 831, Ability = A4, Moves = new[]{ 033, 024, 029, 045 }, Index = 64 }, // Wooloo
            new(30,03,2) { Species = 132, Ability = A4, Moves = new[]{ 144, 000, 000, 000 }, Index = 64 }, // Ditto
            new(30,03,2) { Species = 822, Ability = A4, Moves = new[]{ 403, 263, 279, 681 }, Index = 64 }, // Corvisquire
            new(30,03,2) { Species = 833, Ability = A4, Moves = new[]{ 055, 033, 044, 029 }, Index = 64 }, // Chewtle
            new(30,03,2) { Species = 825, Ability = A4, Moves = new[]{ 522, 263, 371, 247 }, Index = 64 }, // Dottler
            new(30,03,2) { Species = 850, Ability = A4, Moves = new[]{ 172, 044, 035, 052 }, Index = 64 }, // Sizzlipede
            new(30,03,2) { Species = 831, Ability = A4, Moves = new[]{ 036, 024, 029, 086 }, Index = 64 }, // Wooloo
            new(40,05,3) { Species = 132, Ability = A4, Moves = new[]{ 144, 000, 000, 000 }, Index = 64 }, // Ditto
            new(40,05,3) { Species = 823, Ability = A4, Moves = new[]{ 403, 442, 034, 681 }, Index = 64 }, // Corviknight
            new(40,05,3) { Species = 834, Ability = A4, Moves = new[]{ 157, 534, 317, 055 }, Index = 64 }, // Drednaw
            new(40,05,3) { Species = 826, Ability = A4, Moves = new[]{ 405, 277, 371, 247 }, Index = 64 }, // Orbeetle
            new(40,05,3) { Species = 851, Ability = A4, Moves = new[]{ 424, 404, 422, 044 }, Index = 64 }, // Centiskorch
            new(40,05,3) { Species = 832, Ability = A4, Moves = new[]{ 036, 024, 428, 086 }, Index = 64 }, // Dubwool
            new(50,08,4) { Species = 132, Ability = A4, Moves = new[]{ 144, 000, 000, 000 }, Index = 64 }, // Ditto
            new(50,08,4) { Species = 823, Ability = A4, Moves = new[]{ 413, 442, 034, 681 }, Index = 64 }, // Corviknight
            new(50,08,4) { Species = 834, Ability = A4, Moves = new[]{ 157, 710, 317, 334 }, Index = 64 }, // Drednaw
            new(50,08,4) { Species = 826, Ability = A4, Moves = new[]{ 405, 277, 776, 247 }, Index = 64 }, // Orbeetle
            new(50,08,4) { Species = 851, Ability = A4, Moves = new[]{ 680, 404, 422, 044 }, Index = 64 }, // Centiskorch
            new(50,08,4) { Species = 832, Ability = A4, Moves = new[]{ 038, 024, 428, 086 }, Index = 64 }, // Dubwool
            new(60,10,5) { Species = 132, Ability = A4, Moves = new[]{ 144, 000, 000, 000 }, Index = 64 }, // Ditto
            new(60,10,5) { Species = 823, Ability = A4, Moves = new[]{ 413, 442, 776, 372 }, Index = 64 }, // Corviknight
            new(60,10,5) { Species = 834, Ability = A4, Moves = new[]{ 157, 710, 317, 334 }, Index = 64 }, // Drednaw
            new(60,10,5) { Species = 826, Ability = A4, Moves = new[]{ 405, 277, 776, 412 }, Index = 64 }, // Orbeetle
            new(60,10,5) { Species = 851, Ability = A4, Moves = new[]{ 680, 679, 422, 044 }, Index = 64 }, // Centiskorch
            new(60,10,5) { Species = 832, Ability = A4, Moves = new[]{ 038, 776, 428, 086 }, Index = 64 }, // Dubwool
        };

        internal static readonly EncounterStatic8ND[] Dist_SW =
        {
            new(17,01,1,SW) { Species = 012, Ability = A4, Moves = new[]{ 405, 060, 016, 078 }, Index = 1 }, // Butterfree
            new(17,01,1,SW) { Species = 012, Ability = A4, Moves = new[]{ 405, 060, 016, 078 }, Index = 1, CanGigantamax = true }, // Butterfree
            new(17,01,1,SW) { Species = 843, Ability = A2, Moves = new[]{ 693, 523, 189, 103 }, Index = 1 }, // Silicobra
            new(17,01,1,SW) { Species = 843, Ability = A4, Moves = new[]{ 693, 523, 189, 103 }, Index = 1 }, // Silicobra
            new(17,01,1,SW) { Species = 833, Ability = A2, Moves = new[]{ 055, 044, 033, 213 }, Index = 1 }, // Chewtle
            new(17,01,1,SW) { Species = 833, Ability = A4, Moves = new[]{ 055, 044, 033, 213 }, Index = 1 }, // Chewtle
            new(30,03,2,SW) { Species = 012, Ability = A4, Moves = new[]{ 405, 403, 081, 079 }, Index = 1 }, // Butterfree
            new(30,03,2,SW) { Species = 012, Ability = A4, Moves = new[]{ 405, 403, 081, 079 }, Index = 1, CanGigantamax = true }, // Butterfree
            new(30,03,2,SW) { Species = 843, Ability = A2, Moves = new[]{ 693, 523, 029, 137 }, Index = 1 }, // Silicobra
            new(30,03,2,SW) { Species = 843, Ability = A4, Moves = new[]{ 693, 523, 029, 137 }, Index = 1 }, // Silicobra
            new(30,03,2,SW) { Species = 834, Ability = A2, Moves = new[]{ 317, 242, 055, 334 }, Index = 1 }, // Drednaw
            new(30,03,2,SW) { Species = 834, Ability = A4, Moves = new[]{ 317, 242, 055, 334 }, Index = 1 }, // Drednaw
            new(40,05,3,SW) { Species = 844, Ability = A4, Moves = new[]{ 693, 529, 407, 424 }, Index = 1 }, // Sandaconda
            new(40,05,3,SW) { Species = 844, Ability = A4, Moves = new[]{ 693, 529, 407, 424 }, Index = 1, CanGigantamax = true }, // Sandaconda
            new(40,05,3,SW) { Species = 834, Ability = A4, Moves = new[]{ 157, 534, 034, 317 }, Index = 1 }, // Drednaw
            new(40,05,3,SW) { Species = 834, Ability = A4, Moves = new[]{ 157, 534, 034, 317 }, Index = 1, CanGigantamax = true }, // Drednaw
            new(50,08,4,SW) { Species = 844, Ability = A4, Moves = new[]{ 693, 529, 342, 328 }, Index = 1 }, // Sandaconda
            new(50,08,4,SW) { Species = 844, Ability = A4, Moves = new[]{ 693, 529, 342, 328 }, Index = 1, CanGigantamax = true }, // Sandaconda
            new(50,08,4,SW) { Species = 834, Ability = A4, Moves = new[]{ 157, 710, 350, 523 }, Index = 1 }, // Drednaw
            new(50,08,4,SW) { Species = 834, Ability = A4, Moves = new[]{ 157, 710, 350, 523 }, Index = 1, CanGigantamax = true }, // Drednaw
            new(60,10,5,SW) { Species = 012, Ability = A4, Moves = new[]{ 405, 094, 542, 202 }, Index = 1 }, // Butterfree
            new(70,10,5,SW) { Species = 012, Ability = A4, Moves = new[]{ 405, 094, 542, 202 }, Index = 1, CanGigantamax = true }, // Butterfree
            new(60,10,5,SW) { Species = 844, Ability = A4, Moves = new[]{ 693, 529, 342, 328 }, Index = 1 }, // Sandaconda
            new(70,10,5,SW) { Species = 844, Ability = A4, Moves = new[]{ 693, 529, 342, 328 }, Index = 1, CanGigantamax = true }, // Sandaconda
            new(60,10,5,SW) { Species = 834, Ability = A4, Moves = new[]{ 157, 710, 317, 334 }, Index = 1 }, // Drednaw
            new(70,10,5,SW) { Species = 834, Ability = A4, Moves = new[]{ 157, 710, 317, 334 }, Index = 1, CanGigantamax = true }, // Drednaw

            new(17,01,1,SW) { Species = 012, Ability = A4, Moves = new[]{ 405, 060, 016, 078 }, Index = 2 }, // Butterfree
            new(17,01,1,SW) { Species = 012, Ability = A4, Moves = new[]{ 405, 060, 016, 078 }, Index = 2, CanGigantamax = true }, // Butterfree
            new(17,01,1,SW) { Species = 843, Ability = A4, Moves = new[]{ 693, 523, 189, 103 }, Index = 2 }, // Silicobra
            new(17,01,1,SW) { Species = 833, Ability = A4, Moves = new[]{ 055, 044, 033, 213 }, Index = 2 }, // Chewtle
            new(30,03,2,SW) { Species = 012, Ability = A4, Moves = new[]{ 405, 403, 081, 079 }, Index = 2, CanGigantamax = true }, // Butterfree
            new(30,03,2,SW) { Species = 843, Ability = A4, Moves = new[]{ 693, 523, 029, 137 }, Index = 2 }, // Silicobra
            new(30,03,2,SW) { Species = 834, Ability = A4, Moves = new[]{ 317, 242, 055, 334 }, Index = 2 }, // Drednaw
            new(30,03,2,SW) { Species = 834, Ability = A2, Moves = new[]{ 317, 242, 055, 334 }, Index = 2 }, // Drednaw
            new(40,05,3,SW) { Species = 844, Ability = A4, Moves = new[]{ 693, 529, 407, 424 }, Index = 2, CanGigantamax = true }, // Sandaconda
            new(40,05,3,SW) { Species = 834, Ability = A4, Moves = new[]{ 157, 534, 034, 317 }, Index = 2 }, // Drednaw
            new(40,05,3,SW) { Species = 834, Ability = A4, Moves = new[]{ 157, 534, 034, 317 }, Index = 2, CanGigantamax = true }, // Drednaw
            new(50,08,4,SW) { Species = 844, Ability = A4, Moves = new[]{ 693, 529, 342, 328 }, Index = 2, CanGigantamax = true }, // Sandaconda
            new(50,08,4,SW) { Species = 834, Ability = A4, Moves = new[]{ 157, 710, 350, 523 }, Index = 2 }, // Drednaw
            new(50,08,4,SW) { Species = 834, Ability = A4, Moves = new[]{ 157, 710, 350, 523 }, Index = 2, CanGigantamax = true }, // Drednaw
            new(60,10,5,SW) { Species = 012, Ability = A4, Moves = new[]{ 405, 094, 542, 202 }, Index = 2, CanGigantamax = true }, // Butterfree
            new(70,10,5,SW) { Species = 844, Ability = A4, Moves = new[]{ 693, 529, 342, 328 }, Index = 2, CanGigantamax = true }, // Sandaconda
            new(60,10,5,SW) { Species = 834, Ability = A4, Moves = new[]{ 157, 710, 317, 334 }, Index = 2 }, // Drednaw
            new(70,10,5,SW) { Species = 834, Ability = A4, Moves = new[]{ 157, 710, 317, 334 }, Index = 2, CanGigantamax = true }, // Drednaw

            new(17,01,1,SW) { Species = 012, Ability = A4, Moves = new[]{ 405, 060, 016, 078 }, Index = 3 }, // Butterfree
            new(17,01,1,SW) { Species = 012, Ability = A4, Moves = new[]{ 405, 060, 016, 078 }, Index = 3, CanGigantamax = true }, // Butterfree
            new(17,01,1,SW) { Species = 843, Ability = A4, Moves = new[]{ 693, 523, 189, 103 }, Index = 3 }, // Silicobra
            new(17,01,1,SW) { Species = 833, Ability = A4, Moves = new[]{ 055, 044, 033, 213 }, Index = 3 }, // Chewtle
            new(30,03,2,SW) { Species = 012, Ability = A4, Moves = new[]{ 405, 403, 081, 079 }, Index = 3, CanGigantamax = true }, // Butterfree
            new(30,03,2,SW) { Species = 843, Ability = A4, Moves = new[]{ 693, 523, 029, 137 }, Index = 3 }, // Silicobra
            new(30,03,2,SW) { Species = 834, Ability = A4, Moves = new[]{ 317, 242, 055, 334 }, Index = 3 }, // Drednaw
            new(40,05,3,SW) { Species = 844, Ability = A4, Moves = new[]{ 693, 529, 407, 424 }, Index = 3, CanGigantamax = true }, // Sandaconda
            new(40,05,3,SW) { Species = 834, Ability = A4, Moves = new[]{ 157, 534, 034, 317 }, Index = 3 }, // Drednaw
            new(40,05,3,SW) { Species = 834, Ability = A4, Moves = new[]{ 157, 534, 034, 317 }, Index = 3, CanGigantamax = true }, // Drednaw
            new(50,08,3,SW) { Species = 844, Ability = A4, Moves = new[]{ 693, 529, 407, 424 }, Index = 3, CanGigantamax = true }, // Sandaconda
            new(50,08,4,SW) { Species = 834, Ability = A4, Moves = new[]{ 157, 710, 350, 523 }, Index = 3 }, // Drednaw
            new(50,08,4,SW) { Species = 834, Ability = A4, Moves = new[]{ 157, 710, 350, 523 }, Index = 3, CanGigantamax = true }, // Drednaw
            new(60,10,5,SW) { Species = 012, Ability = A4, Moves = new[]{ 405, 094, 542, 202 }, Index = 3, CanGigantamax = true }, // Butterfree
            new(70,10,5,SW) { Species = 844, Ability = A4, Moves = new[]{ 693, 529, 342, 328 }, Index = 3, CanGigantamax = true }, // Sandaconda
            new(60,10,5,SW) { Species = 834, Ability = A4, Moves = new[]{ 157, 710, 317, 334 }, Index = 3 }, // Drednaw
            new(70,10,5,SW) { Species = 834, Ability = A4, Moves = new[]{ 157, 710, 317, 334 }, Index = 3, CanGigantamax = true }, // Drednaw

            new(17,01,1,SW) { Species = 012, Ability = A4, Moves = new[]{ 405, 060, 016, 078 }, Index = 4 }, // Butterfree
            new(17,01,1,SW) { Species = 012, Ability = A4, Moves = new[]{ 405, 060, 016, 078 }, Index = 4, CanGigantamax = true }, // Butterfree
            new(17,01,1,SW) { Species = 843, Ability = A4, Moves = new[]{ 693, 523, 189, 103 }, Index = 4 }, // Silicobra
            new(17,01,1,SW) { Species = 833, Ability = A4, Moves = new[]{ 055, 044, 033, 213 }, Index = 4 }, // Chewtle
            new(30,03,2,SW) { Species = 012, Ability = A4, Moves = new[]{ 405, 403, 081, 079 }, Index = 4, CanGigantamax = true }, // Butterfree
            new(30,03,2,SW) { Species = 843, Ability = A4, Moves = new[]{ 693, 523, 029, 137 }, Index = 4 }, // Silicobra
            new(30,03,2,SW) { Species = 834, Ability = A4, Moves = new[]{ 317, 242, 055, 334 }, Index = 4 }, // Drednaw
            new(40,05,3,SW) { Species = 844, Ability = A4, Moves = new[]{ 693, 529, 407, 424 }, Index = 4, CanGigantamax = true }, // Sandaconda
            new(40,05,3,SW) { Species = 834, Ability = A4, Moves = new[]{ 157, 534, 034, 317 }, Index = 4 }, // Drednaw
            new(40,05,3,SW) { Species = 834, Ability = A4, Moves = new[]{ 157, 534, 034, 317 }, Index = 4, CanGigantamax = true }, // Drednaw
            new(50,08,3,SW) { Species = 844, Ability = A4, Moves = new[]{ 693, 529, 407, 424 }, Index = 4, CanGigantamax = true }, // Sandaconda
            new(50,08,4,SW) { Species = 834, Ability = A4, Moves = new[]{ 157, 710, 350, 523 }, Index = 4 }, // Drednaw
            new(50,08,4,SW) { Species = 834, Ability = A4, Moves = new[]{ 157, 710, 350, 523 }, Index = 4, CanGigantamax = true }, // Drednaw
            new(60,10,5,SW) { Species = 012, Ability = A4, Moves = new[]{ 405, 094, 542, 202 }, Index = 4, CanGigantamax = true }, // Butterfree
            new(70,10,5,SW) { Species = 844, Ability = A4, Moves = new[]{ 693, 529, 342, 328 }, Index = 4, CanGigantamax = true }, // Sandaconda
            new(60,10,5,SW) { Species = 834, Ability = A4, Moves = new[]{ 157, 710, 317, 334 }, Index = 4 }, // Drednaw
            new(70,10,5,SW) { Species = 834, Ability = A4, Moves = new[]{ 157, 710, 317, 334 }, Index = 4, CanGigantamax = true }, // Drednaw

            new(17,01,1,SW) { Species = 012, Ability = A4, Moves = new[]{ 405, 060, 016, 078 }, Index = 5 }, // Butterfree
            new(17,01,1,SW) { Species = 012, Ability = A4, Moves = new[]{ 405, 060, 016, 078 }, Index = 5, CanGigantamax = true }, // Butterfree
            new(17,01,1,SW) { Species = 843, Ability = A4, Moves = new[]{ 693, 523, 189, 103 }, Index = 5 }, // Silicobra
            new(17,01,1,SW) { Species = 833, Ability = A4, Moves = new[]{ 055, 044, 033, 213 }, Index = 5 }, // Chewtle
            new(30,03,2,SW) { Species = 012, Ability = A4, Moves = new[]{ 405, 403, 081, 079 }, Index = 5, CanGigantamax = true }, // Butterfree
            new(30,03,2,SW) { Species = 843, Ability = A4, Moves = new[]{ 693, 523, 029, 137 }, Index = 5 }, // Silicobra
            new(30,03,2,SW) { Species = 834, Ability = A4, Moves = new[]{ 317, 242, 055, 334 }, Index = 5 }, // Drednaw
            new(30,03,2,SW) { Species = 834, Ability = A2, Moves = new[]{ 317, 242, 055, 334 }, Index = 5 }, // Drednaw
            new(40,05,3,SW) { Species = 844, Ability = A4, Moves = new[]{ 693, 529, 407, 424 }, Index = 5, CanGigantamax = true }, // Sandaconda
            new(40,05,3,SW) { Species = 834, Ability = A4, Moves = new[]{ 157, 534, 034, 317 }, Index = 5 }, // Drednaw
            new(40,05,3,SW) { Species = 834, Ability = A4, Moves = new[]{ 157, 534, 034, 317 }, Index = 5, CanGigantamax = true }, // Drednaw
            new(50,08,4,SW) { Species = 844, Ability = A4, Moves = new[]{ 693, 529, 342, 328 }, Index = 5, CanGigantamax = true }, // Sandaconda
            new(50,08,4,SW) { Species = 834, Ability = A4, Moves = new[]{ 157, 710, 350, 523 }, Index = 5 }, // Drednaw
            new(50,08,4,SW) { Species = 834, Ability = A4, Moves = new[]{ 157, 710, 350, 523 }, Index = 5, CanGigantamax = true }, // Drednaw
            new(60,10,5,SW) { Species = 012, Ability = A4, Moves = new[]{ 405, 094, 542, 202 }, Index = 5, CanGigantamax = true }, // Butterfree
            new(70,10,5,SW) { Species = 844, Ability = A4, Moves = new[]{ 693, 529, 342, 328 }, Index = 5, CanGigantamax = true }, // Sandaconda
            new(60,10,5,SW) { Species = 834, Ability = A4, Moves = new[]{ 157, 710, 317, 334 }, Index = 5 }, // Drednaw
            new(70,10,5,SW) { Species = 834, Ability = A4, Moves = new[]{ 157, 710, 317, 334 }, Index = 5, CanGigantamax = true }, // Drednaw

            new(17,01,1,SW) { Species = 012, Ability = A4, Moves = new[]{ 405, 060, 016, 078 }, Index = 6, CanGigantamax = true }, // Butterfree
            new(17,01,1,SW) { Species = 843, Ability = A4, Moves = new[]{ 693, 523, 189, 103 }, Index = 6 }, // Silicobra
            new(17,01,1,SW) { Species = 833, Ability = A4, Moves = new[]{ 055, 044, 033, 213 }, Index = 6 }, // Chewtle
            new(30,03,2,SW) { Species = 012, Ability = A4, Moves = new[]{ 405, 403, 081, 079 }, Index = 6, CanGigantamax = true }, // Butterfree
            new(30,03,2,SW) { Species = 843, Ability = A4, Moves = new[]{ 693, 523, 029, 137 }, Index = 6 }, // Silicobra
            new(30,03,2,SW) { Species = 834, Ability = A4, Moves = new[]{ 317, 242, 055, 334 }, Index = 6 }, // Drednaw
            new(40,05,3,SW) { Species = 844, Ability = A4, Moves = new[]{ 693, 529, 407, 424 }, Index = 6, CanGigantamax = true }, // Sandaconda
            new(40,05,3,SW) { Species = 834, Ability = A4, Moves = new[]{ 157, 534, 034, 317 }, Index = 6, CanGigantamax = true }, // Drednaw
            new(50,08,4,SW) { Species = 844, Ability = A4, Moves = new[]{ 693, 529, 342, 328 }, Index = 6, CanGigantamax = true }, // Sandaconda
            new(50,08,4,SW) { Species = 834, Ability = A4, Moves = new[]{ 157, 710, 350, 523 }, Index = 6, CanGigantamax = true }, // Drednaw
            new(60,10,5,SW) { Species = 012, Ability = A4, Moves = new[]{ 405, 094, 542, 202 }, Index = 6, CanGigantamax = true }, // Butterfree
            new(70,10,5,SW) { Species = 844, Ability = A4, Moves = new[]{ 693, 529, 342, 328 }, Index = 6, CanGigantamax = true }, // Sandaconda
            new(70,10,5,SW) { Species = 834, Ability = A4, Moves = new[]{ 157, 710, 317, 334 }, Index = 6, CanGigantamax = true }, // Drednaw

            new(17,01,1,SW) { Species = 012, Ability = A4, Moves = new[]{ 405, 060, 016, 078 }, Index = 7 }, // Butterfree
            new(17,01,1,SW) { Species = 012, Ability = A4, Moves = new[]{ 405, 060, 016, 078 }, Index = 7, CanGigantamax = true }, // Butterfree
            new(17,01,1,SW) { Species = 843, Ability = A4, Moves = new[]{ 693, 523, 189, 103 }, Index = 7 }, // Silicobra
            new(17,01,1,SW) { Species = 833, Ability = A4, Moves = new[]{ 055, 044, 033, 213 }, Index = 7 }, // Chewtle
            new(30,03,2,SW) { Species = 012, Ability = A4, Moves = new[]{ 405, 403, 081, 079 }, Index = 7, CanGigantamax = true }, // Butterfree
            new(30,03,2,SW) { Species = 843, Ability = A4, Moves = new[]{ 693, 523, 029, 137 }, Index = 7 }, // Silicobra
            new(30,03,2,SW) { Species = 834, Ability = A4, Moves = new[]{ 317, 242, 055, 334 }, Index = 7 }, // Drednaw
            new(30,03,2,SW) { Species = 834, Ability = A2, Moves = new[]{ 317, 242, 055, 334 }, Index = 7 }, // Drednaw
            new(40,05,3,SW) { Species = 844, Ability = A4, Moves = new[]{ 693, 529, 407, 424 }, Index = 7, CanGigantamax = true }, // Sandaconda
            new(40,05,3,SW) { Species = 834, Ability = A4, Moves = new[]{ 157, 534, 034, 317 }, Index = 7 }, // Drednaw
            new(40,05,3,SW) { Species = 834, Ability = A4, Moves = new[]{ 157, 534, 034, 317 }, Index = 7, CanGigantamax = true }, // Drednaw
            new(50,08,4,SW) { Species = 844, Ability = A4, Moves = new[]{ 693, 529, 342, 328 }, Index = 7, CanGigantamax = true }, // Sandaconda
            new(50,08,4,SW) { Species = 834, Ability = A4, Moves = new[]{ 157, 710, 350, 523 }, Index = 7 }, // Drednaw
            new(50,08,4,SW) { Species = 834, Ability = A4, Moves = new[]{ 157, 710, 350, 523 }, Index = 7, CanGigantamax = true }, // Drednaw
            new(60,10,5,SW) { Species = 012, Ability = A4, Moves = new[]{ 405, 094, 542, 202 }, Index = 7, CanGigantamax = true }, // Butterfree
            new(70,10,5,SW) { Species = 844, Ability = A4, Moves = new[]{ 693, 529, 342, 328 }, Index = 7, CanGigantamax = true }, // Sandaconda
            new(60,10,5,SW) { Species = 834, Ability = A4, Moves = new[]{ 157, 710, 317, 334 }, Index = 7 }, // Drednaw
            new(70,10,5,SW) { Species = 834, Ability = A4, Moves = new[]{ 157, 710, 317, 334 }, Index = 7, CanGigantamax = true }, // Drednaw

            new(17,01,1,SW) { Species = 837, Ability = A4, Moves = new[]{ 479, 033, 108, 189 }, Index = 8 }, // Rolycoly
            new(17,01,1,SW) { Species = 837, Ability = A2, Moves = new[]{ 479, 033, 108, 189 }, Index = 8 }, // Rolycoly
            new(30,03,2,SW) { Species = 841, Ability = A4, Moves = new[]{ 406, 491, 017, 225 }, Index = 8 }, // Flapple
            new(30,03,2,SW) { Species = 841, Ability = A2, Moves = new[]{ 406, 491, 017, 225 }, Index = 8 }, // Flapple
            new(30,03,2,SW) { Species = 838, Ability = A4, Moves = new[]{ 246, 510, 479, 189 }, Index = 8 }, // Carkol
            new(30,03,2,SW) { Species = 838, Ability = A2, Moves = new[]{ 246, 510, 479, 189 }, Index = 8 }, // Carkol
            new(40,05,3,SW) { Species = 841, Ability = A4, Moves = new[]{ 788, 406, 512, 491 }, Index = 8 }, // Flapple
            new(40,05,3,SW) { Species = 841, Ability = A4, Moves = new[]{ 788, 406, 512, 491 }, Index = 8, CanGigantamax = true }, // Flapple
            new(40,05,3,SW) { Species = 869, Ability = A4, Moves = new[]{ 577, 605, 202, 186 }, Index = 8, Form = 3, CanGigantamax = true }, // Alcremie-3
            new(40,05,3,SW) { Species = 869, Ability = A4, Moves = new[]{ 577, 605, 202, 186 }, Index = 8, Form = 4, CanGigantamax = true }, // Alcremie-4
            new(40,05,3,SW) { Species = 839, Ability = A4, Moves = new[]{ 246, 510, 479, 488 }, Index = 8 }, // Coalossal
            new(40,05,3,SW) { Species = 839, Ability = A4, Moves = new[]{ 246, 510, 479, 488 }, Index = 8, CanGigantamax = true }, // Coalossal
            new(50,08,4,SW) { Species = 841, Ability = A4, Moves = new[]{ 407, 788, 491, 334 }, Index = 8 }, // Flapple
            new(50,08,4,SW) { Species = 841, Ability = A4, Moves = new[]{ 407, 788, 491, 334 }, Index = 8, CanGigantamax = true }, // Flapple
            new(50,08,4,SW) { Species = 869, Ability = A4, Moves = new[]{ 605, 202, 094, 151 }, Index = 8, Form = 1, CanGigantamax = true }, // Alcremie-1
            new(50,08,4,SW) { Species = 869, Ability = A4, Moves = new[]{ 605, 202, 094, 151 }, Index = 8, Form = 2, CanGigantamax = true }, // Alcremie-2
            new(50,08,4,SW) { Species = 839, Ability = A4, Moves = new[]{ 246, 053, 157, 261 }, Index = 8 }, // Coalossal
            new(50,08,4,SW) { Species = 839, Ability = A4, Moves = new[]{ 246, 053, 157, 261 }, Index = 8, CanGigantamax = true }, // Coalossal
            new(60,10,5,SW) { Species = 841, Ability = A4, Moves = new[]{ 407, 788, 512, 349 }, Index = 8 }, // Flapple
            new(60,10,5,SW) { Species = 841, Ability = A4, Moves = new[]{ 407, 788, 512, 349 }, Index = 8, CanGigantamax = true }, // Flapple
            new(60,10,5,SW) { Species = 869, Ability = A4, Moves = new[]{ 605, 202, 595, 500 }, Index = 8, Form = 5, CanGigantamax = true }, // Alcremie-5
            new(60,10,5,SW) { Species = 869, Ability = A4, Moves = new[]{ 605, 202, 595, 500 }, Index = 8, Form = 6, CanGigantamax = true }, // Alcremie-6
            new(60,10,5,SW) { Species = 839, Ability = A4, Moves = new[]{ 246, 053, 157, 523 }, Index = 8 }, // Coalossal
            new(60,10,5,SW) { Species = 839, Ability = A4, Moves = new[]{ 246, 053, 157, 523 }, Index = 8, CanGigantamax = true }, // Coalossal

            new(40,05,3,SW) { Species = 841, Ability = A4, Moves = new[]{ 788, 406, 512, 491 }, Index = 9, CanGigantamax = true }, // Flapple
            new(40,05,3,SW) { Species = 839, Ability = A4, Moves = new[]{ 246, 510, 479, 488 }, Index = 9, CanGigantamax = true }, // Coalossal
            new(50,08,4,SW) { Species = 841, Ability = A4, Moves = new[]{ 788, 407, 491, 334 }, Index = 9, CanGigantamax = true }, // Flapple
            new(50,08,4,SW) { Species = 839, Ability = A4, Moves = new[]{ 246, 053, 157, 261 }, Index = 9, CanGigantamax = true }, // Coalossal
            new(60,10,5,SW) { Species = 841, Ability = A4, Moves = new[]{ 407, 788, 512, 349 }, Index = 9, CanGigantamax = true }, // Flapple
            new(60,10,5,SW) { Species = 839, Ability = A4, Moves = new[]{ 246, 053, 157, 523 }, Index = 9, CanGigantamax = true }, // Coalossal

            new(40,05,3,SW) { Species = 099, Ability = A4, Moves = new[]{ 534, 232, 023, 106 }, Index = 10, CanGigantamax = true }, // Kingler
            new(40,05,3,SW) { Species = 860, Ability = A4, Moves = new[]{ 492, 577, 421, 141 }, Index = 10 }, // Morgrem
            new(40,05,3,SW) { Species = 849, Ability = A4, Moves = new[]{ 085, 474, 496, 103 }, Index = 10, CanGigantamax = true }, // Toxtricity
            new(50,08,4,SW) { Species = 099, Ability = A4, Moves = new[]{ 359, 667, 157, 534 }, Index = 10, CanGigantamax = true }, // Kingler
            new(50,08,4,SW) { Species = 861, Ability = A4, Moves = new[]{ 789, 793, 280, 409 }, Index = 10, CanGigantamax = true }, // Grimmsnarl
            new(50,08,4,SW) { Species = 849, Ability = A4, Moves = new[]{ 786, 506, 474, 409 }, Index = 10, CanGigantamax = true }, // Toxtricity
            new(60,10,5,SW) { Species = 099, Ability = A4, Moves = new[]{ 152, 667, 157, 404 }, Index = 10, CanGigantamax = true }, // Kingler
            new(60,10,5,SW) { Species = 861, Ability = A4, Moves = new[]{ 789, 793, 409, 007 }, Index = 10, CanGigantamax = true }, // Grimmsnarl
            new(60,10,5,SW) { Species = 849, Ability = A4, Moves = new[]{ 786, 586, 482, 506 }, Index = 10, CanGigantamax = true }, // Toxtricity

            new(17,01,1,SW) { Species = 098, Ability = A4, Moves = new[]{ 055, 043, 106, 232 }, Index = 11 }, // Krabby
            new(17,01,1,SW) { Species = 859, Ability = A4, Moves = new[]{ 044, 260, 590, 372 }, Index = 11 }, // Impidimp
            new(30,03,2,SW) { Species = 099, Ability = A4, Moves = new[]{ 232, 341, 061, 023 }, Index = 11 }, // Kingler
            new(30,03,2,SW) { Species = 099, Ability = A4, Moves = new[]{ 232, 341, 061, 023 }, Index = 11, CanGigantamax = true }, // Kingler
            new(30,03,2,SW) { Species = 859, Ability = A4, Moves = new[]{ 389, 577, 260, 279 }, Index = 11 }, // Impidimp
            new(30,03,2,SW) { Species = 849, Ability = A4, Moves = new[]{ 474, 209, 268, 175 }, Index = 11 }, // Toxtricity
            new(40,05,3,SW) { Species = 099, Ability = A4, Moves = new[]{ 534, 232, 023, 106 }, Index = 11 }, // Kingler
            new(40,05,3,SW) { Species = 099, Ability = A4, Moves = new[]{ 534, 232, 023, 106 }, Index = 11, CanGigantamax = true }, // Kingler
            new(40,05,3,SW) { Species = 860, Ability = A4, Moves = new[]{ 492, 577, 421, 141 }, Index = 11 }, // Morgrem
            new(40,05,3,SW) { Species = 849, Ability = A4, Moves = new[]{ 085, 474, 496, 103 }, Index = 11 }, // Toxtricity
            new(40,05,3,SW) { Species = 849, Ability = A4, Moves = new[]{ 085, 474, 496, 103 }, Index = 11, CanGigantamax = true }, // Toxtricity
            new(50,08,4,SW) { Species = 099, Ability = A4, Moves = new[]{ 359, 667, 157, 534 }, Index = 11 }, // Kingler
            new(50,08,4,SW) { Species = 099, Ability = A4, Moves = new[]{ 359, 667, 157, 534 }, Index = 11, CanGigantamax = true }, // Kingler
            new(50,08,4,SW) { Species = 861, Ability = A4, Moves = new[]{ 789, 793, 280, 409 }, Index = 11 }, // Grimmsnarl
            new(50,08,4,SW) { Species = 861, Ability = A4, Moves = new[]{ 789, 793, 280, 409 }, Index = 11, CanGigantamax = true }, // Grimmsnarl
            new(50,08,4,SW) { Species = 849, Ability = A4, Moves = new[]{ 786, 506, 474, 409 }, Index = 11 }, // Toxtricity
            new(50,08,4,SW) { Species = 849, Ability = A4, Moves = new[]{ 786, 506, 474, 409 }, Index = 11, CanGigantamax = true }, // Toxtricity
            new(60,10,5,SW) { Species = 099, Ability = A4, Moves = new[]{ 152, 667, 157, 404 }, Index = 11 }, // Kingler
            new(60,10,5,SW) { Species = 099, Ability = A4, Moves = new[]{ 152, 667, 157, 404 }, Index = 11, CanGigantamax = true }, // Kingler
            new(60,10,5,SW) { Species = 861, Ability = A4, Moves = new[]{ 789, 793, 409, 007 }, Index = 11 }, // Grimmsnarl
            new(60,10,5,SW) { Species = 861, Ability = A4, Moves = new[]{ 789, 793, 409, 007 }, Index = 11, CanGigantamax = true }, // Grimmsnarl
            new(60,10,5,SW) { Species = 849, Ability = A4, Moves = new[]{ 786, 586, 188, 506 }, Index = 11 }, // Toxtricity
            new(60,10,5,SW) { Species = 849, Ability = A4, Moves = new[]{ 786, 586, 188, 506 }, Index = 11, CanGigantamax = true }, // Toxtricity

            new(17,01,1,SW) { Species = 098, Ability = A4, Moves = new[]{ 055, 043, 106, 232 }, Index = 12 }, // Krabby
            new(17,01,1,SW) { Species = 859, Ability = A4, Moves = new[]{ 044, 260, 590, 372 }, Index = 12 }, // Impidimp
            new(30,03,2,SW) { Species = 099, Ability = A4, Moves = new[]{ 232, 341, 061, 023 }, Index = 12, CanGigantamax = true }, // Kingler
            new(30,03,2,SW) { Species = 859, Ability = A4, Moves = new[]{ 389, 577, 260, 279 }, Index = 12 }, // Impidimp
            new(30,03,2,SW) { Species = 849, Ability = A4, Moves = new[]{ 084, 209, 268, 175 }, Index = 12 }, // Toxtricity
            new(40,05,3,SW) { Species = 099, Ability = A4, Moves = new[]{ 534, 232, 023, 106 }, Index = 12, CanGigantamax = true }, // Kingler
            new(40,05,3,SW) { Species = 860, Ability = A4, Moves = new[]{ 492, 577, 421, 141 }, Index = 12 }, // Morgrem
            new(40,05,3,SW) { Species = 849, Ability = A4, Moves = new[]{ 085, 474, 496, 103 }, Index = 12, CanGigantamax = true }, // Toxtricity
            new(50,08,4,SW) { Species = 099, Ability = A4, Moves = new[]{ 359, 667, 157, 534 }, Index = 12, CanGigantamax = true }, // Kingler
            new(50,08,4,SW) { Species = 861, Ability = A4, Moves = new[]{ 789, 793, 280, 409 }, Index = 12, CanGigantamax = true }, // Grimmsnarl
            new(50,08,4,SW) { Species = 849, Ability = A4, Moves = new[]{ 786, 506, 474, 409 }, Index = 12, CanGigantamax = true }, // Toxtricity
            new(60,10,5,SW) { Species = 099, Ability = A4, Moves = new[]{ 152, 667, 157, 404 }, Index = 12, CanGigantamax = true }, // Kingler
            new(60,10,5,SW) { Species = 861, Ability = A4, Moves = new[]{ 789, 793, 409, 007 }, Index = 12, CanGigantamax = true }, // Grimmsnarl
            new(60,10,5,SW) { Species = 849, Ability = A4, Moves = new[]{ 786, 586, 188, 506 }, Index = 12, CanGigantamax = true }, // Toxtricity

            new(17,01,1,SW) { Species = 098, Ability = A4, Moves = new[]{ 055, 043, 106, 232 }, Index = 13 }, // Krabby
            new(17,01,1,SW) { Species = 859, Ability = A4, Moves = new[]{ 044, 260, 590, 372 }, Index = 13 }, // Impidimp
            new(30,03,2,SW) { Species = 099, Ability = A4, Moves = new[]{ 232, 341, 061, 023 }, Index = 13 }, // Kingler
            new(30,03,2,SW) { Species = 099, Ability = A4, Moves = new[]{ 232, 341, 061, 023 }, Index = 13, CanGigantamax = true }, // Kingler
            new(30,03,2,SW) { Species = 859, Ability = A4, Moves = new[]{ 389, 577, 260, 279 }, Index = 13 }, // Impidimp
            new(30,03,2,SW) { Species = 849, Ability = A4, Moves = new[]{ 474, 209, 268, 175 }, Index = 13 }, // Toxtricity
            new(40,05,3,SW) { Species = 099, Ability = A4, Moves = new[]{ 534, 232, 023, 106 }, Index = 13 }, // Kingler
            new(40,05,3,SW) { Species = 099, Ability = A4, Moves = new[]{ 534, 232, 023, 106 }, Index = 13, CanGigantamax = true }, // Kingler
            new(40,05,3,SW) { Species = 860, Ability = A4, Moves = new[]{ 492, 577, 421, 141 }, Index = 13 }, // Morgrem
            new(40,05,3,SW) { Species = 849, Ability = A4, Moves = new[]{ 085, 474, 496, 103 }, Index = 13 }, // Toxtricity
            new(40,05,3,SW) { Species = 849, Ability = A4, Moves = new[]{ 085, 474, 496, 103 }, Index = 13, CanGigantamax = true }, // Toxtricity
            new(50,08,4,SW) { Species = 099, Ability = A4, Moves = new[]{ 359, 667, 157, 534 }, Index = 13 }, // Kingler
            new(50,08,4,SW) { Species = 099, Ability = A4, Moves = new[]{ 359, 667, 157, 534 }, Index = 13, CanGigantamax = true }, // Kingler
            new(50,08,4,SW) { Species = 861, Ability = A4, Moves = new[]{ 789, 793, 280, 409 }, Index = 13 }, // Grimmsnarl
            new(50,08,4,SW) { Species = 861, Ability = A4, Moves = new[]{ 789, 793, 280, 409 }, Index = 13, CanGigantamax = true }, // Grimmsnarl
            new(50,08,4,SW) { Species = 849, Ability = A4, Moves = new[]{ 786, 506, 474, 409 }, Index = 13 }, // Toxtricity
            new(50,08,4,SW) { Species = 849, Ability = A4, Moves = new[]{ 786, 506, 474, 409 }, Index = 13, CanGigantamax = true }, // Toxtricity
            new(60,10,5,SW) { Species = 099, Ability = A4, Moves = new[]{ 152, 667, 157, 404 }, Index = 13 }, // Kingler
            new(60,10,5,SW) { Species = 099, Ability = A4, Moves = new[]{ 152, 667, 157, 404 }, Index = 13, CanGigantamax = true }, // Kingler
            new(60,10,5,SW) { Species = 861, Ability = A4, Moves = new[]{ 789, 793, 409, 007 }, Index = 13 }, // Grimmsnarl
            new(60,10,5,SW) { Species = 861, Ability = A4, Moves = new[]{ 789, 793, 409, 007 }, Index = 13, CanGigantamax = true }, // Grimmsnarl
            new(60,10,5,SW) { Species = 849, Ability = A4, Moves = new[]{ 786, 586, 188, 506 }, Index = 13 }, // Toxtricity
            new(60,10,5,SW) { Species = 849, Ability = A4, Moves = new[]{ 786, 586, 188, 506 }, Index = 13, CanGigantamax = true }, // Toxtricity

            new(60,10,5,SW) { Species = 068, Ability = A4, Moves = new[]{ 238, 007, 008, 089 }, Index = 14 }, // Machamp
            new(60,10,5,SW) { Species = 068, Ability = A4, Moves = new[]{ 238, 007, 008, 089 }, Index = 14, CanGigantamax = true }, // Machamp

            new(17,01,1,SW) { Species = 222, Ability = A4, Moves = new[]{ 033, 106, 310, 050 }, Index = 15, Form = 1 }, // Corsola-1
            new(30,03,2,SW) { Species = 077, Ability = A4, Moves = new[]{ 093, 584, 060, 023 }, Index = 15, Form = 1 }, // Ponyta-1
            new(30,03,2,SW) { Species = 222, Ability = A4, Moves = new[]{ 310, 050, 246, 506 }, Index = 15, Form = 1 }, // Corsola-1
            new(40,05,3,SW) { Species = 077, Ability = A2, Moves = new[]{ 340, 023, 428, 583 }, Index = 15, Form = 1 }, // Ponyta-1
            new(40,05,3,SW) { Species = 222, Ability = A2, Moves = new[]{ 506, 408, 503, 261 }, Index = 15, Form = 1 }, // Corsola-1
            new(60,10,5,SW) { Species = 765, Ability = A2, Moves = new[]{ 492, 094, 085, 247 }, Index = 15 }, // Oranguru
            new(60,10,5,SW) { Species = 876, Ability = A2, Moves = new[]{ 094, 595, 605, 304 }, Index = 15, Form = 1 }, // Indeedee-1
            new(60,10,5,SW) { Species = 630, Ability = A2, Moves = new[]{ 403, 555, 492, 211 }, Index = 15 }, // Mandibuzz
            new(60,10,5,SW) { Species = 078, Ability = A2, Moves = new[]{ 428, 583, 224, 340 }, Index = 15, Form = 1 }, // Rapidash-1
            new(60,10,5,SW) { Species = 068, Ability = A4, Moves = new[]{ 238, 007, 008, 089 }, Index = 15, CanGigantamax = true }, // Machamp

            new(60,10,5,SW) { Species = 879, Ability = A4, Moves = new[]{ 442, 583, 438, 089 }, Index = 16, CanGigantamax = true }, // Copperajah
            new(60,10,5,SW) { Species = 884, Ability = A4, Moves = new[]{ 430, 406, 085, 334 }, Index = 16, CanGigantamax = true }, // Duraludon

            new(60,10,5,SW) { Species = 879, Ability = A4, Moves = new[]{ 442, 583, 438, 089 }, Index = 17, CanGigantamax = true }, // Copperajah
            new(60,10,5,SW) { Species = 884, Ability = A4, Moves = new[]{ 430, 406, 085, 334 }, Index = 17, CanGigantamax = true }, // Duraludon

            new(60,10,5,SW) { Species = 879, Ability = A4, Moves = new[]{ 442, 583, 438, 089 }, Index = 18, CanGigantamax = true }, // Copperajah
            new(60,10,5,SW) { Species = 884, Ability = A4, Moves = new[]{ 430, 406, 085, 334 }, Index = 18, CanGigantamax = true }, // Duraludon

            new(17,01,1,SW) { Species = 479, Ability = A4, Moves = new[]{ 104, 315, 084, 109 }, Index = 19, Form = 1 }, // Rotom-1
            new(17,01,1,SW) { Species = 529, Ability = A4, Moves = new[]{ 189, 232, 010, 468 }, Index = 19 }, // Drilbur
            new(30,03,2,SW) { Species = 479, Ability = A4, Moves = new[]{ 104, 315, 085, 109 }, Index = 19, Form = 1 }, // Rotom-1
            new(40,05,3,SW) { Species = 479, Ability = A4, Moves = new[]{ 521, 315, 085, 506 }, Index = 19, Form = 1 }, // Rotom-1
            new(50,08,4,SW) { Species = 479, Ability = A4, Moves = new[]{ 521, 315, 085, 261 }, Index = 19, Form = 1 }, // Rotom-1
            new(60,10,5,SW) { Species = 479, Ability = A4, Moves = new[]{ 521, 315, 435, 261 }, Index = 19, Form = 1 }, // Rotom-1

            new(17,01,1,SW) { Species = 479, Ability = A4, Moves = new[]{ 104, 315, 084, 109 }, Index = 20, Form = 1 }, // Rotom-1
            new(30,03,2,SW) { Species = 479, Ability = A4, Moves = new[]{ 104, 315, 085, 109 }, Index = 20, Form = 1 }, // Rotom-1
            new(40,05,3,SW) { Species = 479, Ability = A4, Moves = new[]{ 521, 315, 085, 506 }, Index = 20, Form = 1 }, // Rotom-1
            new(50,08,4,SW) { Species = 479, Ability = A4, Moves = new[]{ 521, 315, 085, 261 }, Index = 20, Form = 1 }, // Rotom-1
            new(60,10,5,SW) { Species = 479, Ability = A4, Moves = new[]{ 521, 315, 435, 261 }, Index = 20, Form = 1 }, // Rotom-1

            new(17,01,1,SW) { Species = 869, Ability = A4, Moves = new[]{ 033, 186, 577, 230 }, Index = 24, CanGigantamax = true }, // Alcremie
            new(30,03,2,SW) { Species = 851, Ability = A4, Moves = new[]{ 044, 172, 489, 693 }, Index = 24, CanGigantamax = true }, // Centiskorch
            new(30,03,2,SW) { Species = 131, Ability = A4, Moves = new[]{ 352, 420, 109, 047 }, Index = 24, CanGigantamax = true }, // Lapras
            new(40,05,3,SW) { Species = 099, Ability = A4, Moves = new[]{ 534, 232, 023, 106 }, Index = 24, CanGigantamax = true }, // Kingler
            new(40,05,3,SW) { Species = 842, Ability = A4, Moves = new[]{ 787, 496, 406, 523 }, Index = 24, CanGigantamax = true }, // Appletun
            new(40,05,3,SW) { Species = 851, Ability = A4, Moves = new[]{ 141, 424, 422, 044 }, Index = 24, CanGigantamax = true }, // Centiskorch
            new(50,08,4,SW) { Species = 823, Ability = A4, Moves = new[]{ 413, 442, 269, 103 }, Index = 24, CanGigantamax = true }, // Corviknight
            new(50,08,4,SW) { Species = 861, Ability = A4, Moves = new[]{ 789, 793, 280, 409 }, Index = 24, CanGigantamax = true }, // Grimmsnarl
            new(50,08,4,SW) { Species = 569, Ability = A4, Moves = new[]{ 188, 499, 034, 707 }, Index = 24, CanGigantamax = true }, // Garbodor
            new(50,08,4,SW) { Species = 869, Ability = A4, Moves = new[]{ 577, 605, 105, 500 }, Index = 24, CanGigantamax = true }, // Alcremie
            new(60,10,5,SW) { Species = 131, Ability = A4, Moves = new[]{ 057, 196, 058, 329 }, Index = 24, CanGigantamax = true }, // Lapras
            new(60,10,5,SW) { Species = 849, Ability = A4, Moves = new[]{ 786, 506, 474, 409 }, Index = 24, CanGigantamax = true }, // Toxtricity
            new(60,10,5,SW) { Species = 094, Ability = A4, Moves = new[]{ 247, 482, 094, 196 }, Index = 24, CanGigantamax = true }, // Gengar
            new(60,10,5,SW) { Species = 884, Ability = A4, Moves = new[]{ 430, 406, 085, 334 }, Index = 24, CanGigantamax = true }, // Duraludon

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

            new(30,03,2,SW) { Species = 849, Ability = A4, Moves = new[]{ 351, 506, 491, 103 }, Index = 36, Form = 1, CanGigantamax = true }, // Toxtricity-1
            new(30,03,2,SW) { Species = 842, Ability = A4, Moves = new[]{ 787, 496, 310, 029 }, Index = 36, CanGigantamax = true }, // Appletun
            new(40,05,3,SW) { Species = 849, Ability = A4, Moves = new[]{ 435, 506, 398, 103 }, Index = 36, Form = 1, CanGigantamax = true }, // Toxtricity-1
            new(40,05,3,SW) { Species = 842, Ability = A4, Moves = new[]{ 787, 496, 406, 029 }, Index = 36, CanGigantamax = true }, // Appletun
            new(50,08,4,SW) { Species = 849, Ability = A4, Moves = new[]{ 786, 506, 398, 586 }, Index = 36, Form = 1, CanGigantamax = true }, // Toxtricity-1
            new(50,08,4,SW) { Species = 842, Ability = A4, Moves = new[]{ 787, 496, 406, 523 }, Index = 36, CanGigantamax = true }, // Appletun
            new(60,10,5,SW) { Species = 849, Ability = A4, Moves = new[]{ 786, 506, 599, 586 }, Index = 36, Form = 1, CanGigantamax = true }, // Toxtricity-1
            new(60,10,5,SW) { Species = 842, Ability = A4, Moves = new[]{ 787, 034, 406, 523 }, Index = 36, CanGigantamax = true }, // Appletun

            new(30,03,2,SW) { Species = 849, Ability = A4, Moves = new[]{ 351, 506, 491, 103 }, Index = 38, Form = 1, CanGigantamax = true }, // Toxtricity-1
            new(30,03,2,SW) { Species = 842, Ability = A4, Moves = new[]{ 787, 496, 310, 029 }, Index = 38, CanGigantamax = true }, // Appletun
            new(40,05,3,SW) { Species = 849, Ability = A4, Moves = new[]{ 435, 506, 398, 103 }, Index = 38, Form = 1, CanGigantamax = true }, // Toxtricity-1
            new(40,05,3,SW) { Species = 842, Ability = A4, Moves = new[]{ 787, 496, 406, 029 }, Index = 38, CanGigantamax = true }, // Appletun
            new(50,08,4,SW) { Species = 849, Ability = A4, Moves = new[]{ 786, 506, 398, 586 }, Index = 38, Form = 1, CanGigantamax = true }, // Toxtricity-1
            new(50,08,4,SW) { Species = 842, Ability = A4, Moves = new[]{ 787, 496, 406, 523 }, Index = 38, CanGigantamax = true }, // Appletun
            new(60,10,5,SW) { Species = 849, Ability = A4, Moves = new[]{ 786, 506, 599, 586 }, Index = 38, Form = 1, CanGigantamax = true }, // Toxtricity-1
            new(60,10,5,SW) { Species = 842, Ability = A4, Moves = new[]{ 787, 034, 406, 523 }, Index = 38, CanGigantamax = true }, // Appletun
        };

        internal static readonly EncounterStatic8ND[] Dist_SH =
        {
            new(17,01,1,SH) { Species = 012, Ability = A4, Moves = new[]{ 405, 060, 016, 079 }, Index = 1 }, // Butterfree
            new(17,01,1,SH) { Species = 012, Ability = A4, Moves = new[]{ 405, 060, 016, 079 }, Index = 1, CanGigantamax = true }, // Butterfree
            new(17,01,1,SH) { Species = 821, Ability = A2, Moves = new[]{ 365, 031, 526, 064 }, Index = 1 }, // Rookidee
            new(17,01,1,SH) { Species = 821, Ability = A4, Moves = new[]{ 365, 031, 526, 064 }, Index = 1 }, // Rookidee
            new(17,01,1,SH) { Species = 850, Ability = A2, Moves = new[]{ 044, 172, 450, 693 }, Index = 1 }, // Sizzlipede
            new(17,01,1,SH) { Species = 850, Ability = A4, Moves = new[]{ 044, 172, 450, 693 }, Index = 1 }, // Sizzlipede
            new(30,03,2,SH) { Species = 012, Ability = A4, Moves = new[]{ 405, 403, 081, 077 }, Index = 1 }, // Butterfree
            new(30,03,2,SH) { Species = 012, Ability = A4, Moves = new[]{ 405, 403, 081, 077 }, Index = 1, CanGigantamax = true }, // Butterfree
            new(30,03,2,SH) { Species = 822, Ability = A2, Moves = new[]{ 365, 263, 179, 468 }, Index = 1 }, // Corvisquire
            new(30,03,2,SH) { Species = 822, Ability = A4, Moves = new[]{ 365, 263, 179, 468 }, Index = 1 }, // Corvisquire
            new(30,03,2,SH) { Species = 851, Ability = A2, Moves = new[]{ 172, 242, 450, 257 }, Index = 1 }, // Centiskorch
            new(30,03,2,SH) { Species = 851, Ability = A4, Moves = new[]{ 172, 242, 450, 257 }, Index = 1 }, // Centiskorch
            new(40,05,3,SH) { Species = 823, Ability = A4, Moves = new[]{ 065, 211, 034, 103 }, Index = 1 }, // Corviknight
            new(40,05,3,SH) { Species = 823, Ability = A4, Moves = new[]{ 065, 211, 034, 103 }, Index = 1, CanGigantamax = true }, // Corviknight
            new(40,05,3,SH) { Species = 851, Ability = A4, Moves = new[]{ 141, 438, 053, 489 }, Index = 1 }, // Centiskorch
            new(40,05,3,SH) { Species = 851, Ability = A4, Moves = new[]{ 141, 438, 053, 489 }, Index = 1, CanGigantamax = true }, // Centiskorch
            new(50,08,4,SH) { Species = 823, Ability = A4, Moves = new[]{ 065, 442, 034, 334 }, Index = 1 }, // Corviknight
            new(50,08,4,SH) { Species = 823, Ability = A4, Moves = new[]{ 065, 442, 034, 334 }, Index = 1, CanGigantamax = true }, // Corviknight
            new(50,08,4,SH) { Species = 851, Ability = A4, Moves = new[]{ 141, 424, 422, 242 }, Index = 1 }, // Centiskorch
            new(50,08,4,SH) { Species = 851, Ability = A4, Moves = new[]{ 141, 424, 422, 242 }, Index = 1, CanGigantamax = true }, // Centiskorch
            new(60,10,5,SH) { Species = 012, Ability = A4, Moves = new[]{ 405, 094, 403, 079 }, Index = 1 }, // Butterfree
            new(70,10,5,SH) { Species = 012, Ability = A4, Moves = new[]{ 405, 094, 403, 079 }, Index = 1, CanGigantamax = true }, // Butterfree
            new(60,10,5,SH) { Species = 823, Ability = A4, Moves = new[]{ 413, 442, 249, 103 }, Index = 1 }, // Corviknight
            new(70,10,5,SH) { Species = 823, Ability = A4, Moves = new[]{ 413, 442, 249, 103 }, Index = 1, CanGigantamax = true }, // Corviknight
            new(60,10,5,SH) { Species = 851, Ability = A4, Moves = new[]{ 679, 257, 083, 438 }, Index = 1 }, // Centiskorch
            new(70,10,5,SH) { Species = 851, Ability = A4, Moves = new[]{ 679, 257, 083, 438 }, Index = 1, CanGigantamax = true }, // Centiskorch

            new(17,01,1,SH) { Species = 012, Ability = A4, Moves = new[]{ 405, 060, 016, 079 }, Index = 2 }, // Butterfree
            new(17,01,1,SH) { Species = 012, Ability = A4, Moves = new[]{ 405, 060, 016, 079 }, Index = 2, CanGigantamax = true }, // Butterfree
            new(17,01,1,SH) { Species = 821, Ability = A4, Moves = new[]{ 403, 031, 116, 064 }, Index = 2 }, // Rookidee
            new(17,01,1,SH) { Species = 850, Ability = A4, Moves = new[]{ 044, 172, 404, 693 }, Index = 2 }, // Sizzlipede
            new(30,03,2,SH) { Species = 012, Ability = A4, Moves = new[]{ 405, 403, 081, 077 }, Index = 2, CanGigantamax = true }, // Butterfree
            new(30,03,2,SH) { Species = 822, Ability = A4, Moves = new[]{ 403, 263, 179, 468 }, Index = 2 }, // Corvisquire
            new(30,03,2,SH) { Species = 851, Ability = A4, Moves = new[]{ 172, 242, 404, 257 }, Index = 2 }, // Centiskorch
            new(30,03,2,SH) { Species = 851, Ability = A2, Moves = new[]{ 172, 242, 404, 257 }, Index = 2 }, // Centiskorch
            new(40,05,3,SH) { Species = 823, Ability = A4, Moves = new[]{ 065, 211, 034, 103 }, Index = 2, CanGigantamax = true }, // Corviknight
            new(40,05,3,SH) { Species = 851, Ability = A4, Moves = new[]{ 141, 438, 053, 489 }, Index = 2 }, // Centiskorch
            new(40,05,3,SH) { Species = 851, Ability = A4, Moves = new[]{ 141, 438, 053, 489 }, Index = 2, CanGigantamax = true }, // Centiskorch
            new(50,08,4,SH) { Species = 823, Ability = A4, Moves = new[]{ 065, 442, 034, 334 }, Index = 2, CanGigantamax = true }, // Corviknight
            new(50,08,4,SH) { Species = 851, Ability = A4, Moves = new[]{ 141, 424, 422, 242 }, Index = 2 }, // Centiskorch
            new(50,08,4,SH) { Species = 851, Ability = A4, Moves = new[]{ 141, 424, 422, 242 }, Index = 2, CanGigantamax = true }, // Centiskorch
            new(60,10,5,SH) { Species = 012, Ability = A4, Moves = new[]{ 405, 094, 403, 079 }, Index = 2, CanGigantamax = true }, // Butterfree
            new(70,10,5,SH) { Species = 823, Ability = A4, Moves = new[]{ 413, 442, 249, 103 }, Index = 2, CanGigantamax = true }, // Corviknight
            new(60,10,5,SH) { Species = 851, Ability = A4, Moves = new[]{ 679, 257, 083, 438 }, Index = 2 }, // Centiskorch
            new(70,10,5,SH) { Species = 851, Ability = A4, Moves = new[]{ 679, 257, 083, 438 }, Index = 2, CanGigantamax = true }, // Centiskorch

            new(17,01,1,SH) { Species = 012, Ability = A4, Moves = new[]{ 405, 060, 016, 079 }, Index = 3 }, // Butterfree
            new(17,01,1,SH) { Species = 012, Ability = A4, Moves = new[]{ 405, 060, 016, 079 }, Index = 3, CanGigantamax = true }, // Butterfree
            new(17,01,1,SH) { Species = 821, Ability = A4, Moves = new[]{ 403, 031, 116, 064 }, Index = 3 }, // Rookidee
            new(17,01,1,SH) { Species = 850, Ability = A4, Moves = new[]{ 044, 172, 141, 693 }, Index = 3 }, // Sizzlipede
            new(30,03,2,SH) { Species = 012, Ability = A4, Moves = new[]{ 405, 403, 081, 077 }, Index = 3, CanGigantamax = true }, // Butterfree
            new(30,03,2,SH) { Species = 822, Ability = A4, Moves = new[]{ 403, 263, 179, 468 }, Index = 3 }, // Corvisquire
            new(30,03,2,SH) { Species = 851, Ability = A4, Moves = new[]{ 172, 242, 404, 257 }, Index = 3 }, // Centiskorch
            new(40,05,3,SH) { Species = 823, Ability = A4, Moves = new[]{ 065, 211, 034, 103 }, Index = 3, CanGigantamax = true }, // Corviknight
            new(40,05,3,SH) { Species = 851, Ability = A4, Moves = new[]{ 141, 438, 053, 489 }, Index = 3 }, // Centiskorch
            new(40,05,3,SH) { Species = 851, Ability = A4, Moves = new[]{ 141, 438, 053, 489 }, Index = 3, CanGigantamax = true }, // Centiskorch
            new(50,08,4,SH) { Species = 823, Ability = A4, Moves = new[]{ 065, 442, 034, 334 }, Index = 3, CanGigantamax = true }, // Corviknight
            new(50,08,4,SH) { Species = 851, Ability = A4, Moves = new[]{ 141, 424, 422, 242 }, Index = 3 }, // Centiskorch
            new(50,08,4,SH) { Species = 851, Ability = A4, Moves = new[]{ 141, 424, 422, 242 }, Index = 3, CanGigantamax = true }, // Centiskorch
            new(60,10,5,SH) { Species = 012, Ability = A4, Moves = new[]{ 405, 094, 403, 079 }, Index = 3, CanGigantamax = true }, // Butterfree
            new(70,10,5,SH) { Species = 823, Ability = A4, Moves = new[]{ 413, 442, 249, 103 }, Index = 3, CanGigantamax = true }, // Corviknight
            new(60,10,5,SH) { Species = 851, Ability = A4, Moves = new[]{ 679, 257, 083, 438 }, Index = 3 }, // Centiskorch
            new(70,10,5,SH) { Species = 851, Ability = A4, Moves = new[]{ 679, 257, 083, 438 }, Index = 3, CanGigantamax = true }, // Centiskorch

            new(17,01,1,SH) { Species = 012, Ability = A4, Moves = new[]{ 405, 060, 016, 079 }, Index = 4 }, // Butterfree
            new(17,01,1,SH) { Species = 012, Ability = A4, Moves = new[]{ 405, 060, 016, 079 }, Index = 4, CanGigantamax = true }, // Butterfree
            new(17,01,1,SH) { Species = 821, Ability = A4, Moves = new[]{ 403, 031, 116, 064 }, Index = 4 }, // Rookidee
            new(17,01,1,SH) { Species = 850, Ability = A4, Moves = new[]{ 044, 172, 141, 693 }, Index = 4 }, // Sizzlipede
            new(30,03,2,SH) { Species = 012, Ability = A4, Moves = new[]{ 405, 403, 081, 077 }, Index = 4, CanGigantamax = true }, // Butterfree
            new(30,03,2,SH) { Species = 822, Ability = A4, Moves = new[]{ 403, 263, 179, 468 }, Index = 4 }, // Corvisquire
            new(30,03,2,SH) { Species = 851, Ability = A4, Moves = new[]{ 172, 242, 404, 257 }, Index = 4 }, // Centiskorch
            new(40,05,3,SH) { Species = 823, Ability = A4, Moves = new[]{ 065, 211, 034, 103 }, Index = 4, CanGigantamax = true }, // Corviknight
            new(40,05,3,SH) { Species = 851, Ability = A4, Moves = new[]{ 141, 438, 053, 489 }, Index = 4 }, // Centiskorch
            new(40,05,3,SH) { Species = 851, Ability = A4, Moves = new[]{ 141, 438, 053, 489 }, Index = 4, CanGigantamax = true }, // Centiskorch
            new(50,08,4,SH) { Species = 823, Ability = A4, Moves = new[]{ 065, 442, 034, 334 }, Index = 4, CanGigantamax = true }, // Corviknight
            new(50,08,4,SH) { Species = 851, Ability = A4, Moves = new[]{ 141, 424, 422, 242 }, Index = 4 }, // Centiskorch
            new(50,08,4,SH) { Species = 851, Ability = A4, Moves = new[]{ 141, 424, 422, 242 }, Index = 4, CanGigantamax = true }, // Centiskorch
            new(60,10,5,SH) { Species = 012, Ability = A4, Moves = new[]{ 405, 094, 403, 079 }, Index = 4, CanGigantamax = true }, // Butterfree
            new(70,10,5,SH) { Species = 823, Ability = A4, Moves = new[]{ 413, 442, 249, 103 }, Index = 4, CanGigantamax = true }, // Corviknight
            new(60,10,5,SH) { Species = 851, Ability = A4, Moves = new[]{ 679, 257, 083, 438 }, Index = 4 }, // Centiskorch
            new(70,10,5,SH) { Species = 851, Ability = A4, Moves = new[]{ 679, 257, 083, 438 }, Index = 4, CanGigantamax = true }, // Centiskorch

            new(17,01,1,SH) { Species = 012, Ability = A4, Moves = new[]{ 405, 060, 016, 079 }, Index = 5 }, // Butterfree
            new(17,01,1,SH) { Species = 012, Ability = A4, Moves = new[]{ 405, 060, 016, 079 }, Index = 5, CanGigantamax = true }, // Butterfree
            new(17,01,1,SH) { Species = 821, Ability = A4, Moves = new[]{ 403, 031, 116, 064 }, Index = 5 }, // Rookidee
            new(17,01,1,SH) { Species = 850, Ability = A4, Moves = new[]{ 044, 172, 141, 693 }, Index = 5 }, // Sizzlipede
            new(30,03,2,SH) { Species = 012, Ability = A4, Moves = new[]{ 405, 403, 081, 077 }, Index = 5, CanGigantamax = true }, // Butterfree
            new(30,03,2,SH) { Species = 822, Ability = A4, Moves = new[]{ 403, 263, 179, 468 }, Index = 5 }, // Corvisquire
            new(30,03,2,SH) { Species = 851, Ability = A4, Moves = new[]{ 172, 242, 404, 257 }, Index = 5 }, // Centiskorch
            new(30,03,2,SH) { Species = 851, Ability = A2, Moves = new[]{ 172, 242, 404, 257 }, Index = 5 }, // Centiskorch
            new(40,05,3,SH) { Species = 823, Ability = A4, Moves = new[]{ 065, 211, 034, 103 }, Index = 5, CanGigantamax = true }, // Corviknight
            new(40,05,3,SH) { Species = 851, Ability = A4, Moves = new[]{ 141, 438, 053, 489 }, Index = 5 }, // Centiskorch
            new(40,05,3,SH) { Species = 851, Ability = A4, Moves = new[]{ 141, 438, 053, 489 }, Index = 5, CanGigantamax = true }, // Centiskorch
            new(50,08,4,SH) { Species = 823, Ability = A4, Moves = new[]{ 065, 442, 034, 334 }, Index = 5, CanGigantamax = true }, // Corviknight
            new(50,08,4,SH) { Species = 851, Ability = A4, Moves = new[]{ 141, 424, 422, 242 }, Index = 5 }, // Centiskorch
            new(50,08,4,SH) { Species = 851, Ability = A4, Moves = new[]{ 141, 424, 422, 242 }, Index = 5, CanGigantamax = true }, // Centiskorch
            new(60,10,5,SH) { Species = 012, Ability = A4, Moves = new[]{ 405, 094, 403, 079 }, Index = 5, CanGigantamax = true }, // Butterfree
            new(70,10,5,SH) { Species = 823, Ability = A4, Moves = new[]{ 413, 442, 249, 103 }, Index = 5, CanGigantamax = true }, // Corviknight
            new(60,10,5,SH) { Species = 851, Ability = A4, Moves = new[]{ 679, 257, 083, 438 }, Index = 5 }, // Centiskorch
            new(70,10,5,SH) { Species = 851, Ability = A4, Moves = new[]{ 679, 257, 083, 438 }, Index = 5, CanGigantamax = true }, // Centiskorch

            new(17,01,1,SH) { Species = 012, Ability = A4, Moves = new[]{ 405, 060, 016, 079 }, Index = 6, CanGigantamax = true }, // Butterfree
            new(17,01,1,SH) { Species = 821, Ability = A4, Moves = new[]{ 403, 031, 116, 064 }, Index = 6 }, // Rookidee
            new(17,01,1,SH) { Species = 850, Ability = A4, Moves = new[]{ 044, 172, 141, 693 }, Index = 6 }, // Sizzlipede
            new(30,03,2,SH) { Species = 012, Ability = A4, Moves = new[]{ 405, 403, 081, 077 }, Index = 6, CanGigantamax = true }, // Butterfree
            new(30,03,2,SH) { Species = 822, Ability = A4, Moves = new[]{ 403, 263, 179, 468 }, Index = 6 }, // Corvisquire
            new(30,03,2,SH) { Species = 851, Ability = A4, Moves = new[]{ 172, 242, 404, 257 }, Index = 6 }, // Centiskorch
            new(40,05,3,SH) { Species = 823, Ability = A4, Moves = new[]{ 065, 211, 034, 103 }, Index = 6, CanGigantamax = true }, // Corviknight
            new(40,05,3,SH) { Species = 851, Ability = A4, Moves = new[]{ 141, 438, 053, 489 }, Index = 6, CanGigantamax = true }, // Centiskorch
            new(50,08,4,SH) { Species = 823, Ability = A4, Moves = new[]{ 065, 442, 034, 334 }, Index = 6, CanGigantamax = true }, // Corviknight
            new(50,08,4,SH) { Species = 851, Ability = A4, Moves = new[]{ 141, 424, 422, 242 }, Index = 6, CanGigantamax = true }, // Centiskorch
            new(60,10,5,SH) { Species = 012, Ability = A4, Moves = new[]{ 405, 094, 403, 079 }, Index = 6, CanGigantamax = true }, // Butterfree
            new(70,10,5,SH) { Species = 823, Ability = A4, Moves = new[]{ 413, 442, 249, 103 }, Index = 6, CanGigantamax = true }, // Corviknight
            new(70,10,5,SH) { Species = 851, Ability = A4, Moves = new[]{ 679, 257, 083, 438 }, Index = 6, CanGigantamax = true }, // Centiskorch

            new(17,01,1,SH) { Species = 012, Ability = A4, Moves = new[]{ 405, 060, 016, 079 }, Index = 7 }, // Butterfree
            new(17,01,1,SH) { Species = 012, Ability = A4, Moves = new[]{ 405, 060, 016, 079 }, Index = 7, CanGigantamax = true }, // Butterfree
            new(17,01,1,SH) { Species = 821, Ability = A4, Moves = new[]{ 403, 031, 116, 064 }, Index = 7 }, // Rookidee
            new(17,01,1,SH) { Species = 850, Ability = A4, Moves = new[]{ 044, 172, 141, 693 }, Index = 7 }, // Sizzlipede
            new(30,03,2,SH) { Species = 012, Ability = A4, Moves = new[]{ 405, 403, 081, 077 }, Index = 7, CanGigantamax = true }, // Butterfree
            new(30,03,2,SH) { Species = 822, Ability = A4, Moves = new[]{ 403, 263, 179, 468 }, Index = 7 }, // Corvisquire
            new(30,03,2,SH) { Species = 851, Ability = A4, Moves = new[]{ 172, 242, 404, 257 }, Index = 7 }, // Centiskorch
            new(30,03,2,SH) { Species = 851, Ability = A2, Moves = new[]{ 172, 242, 404, 257 }, Index = 7 }, // Centiskorch
            new(40,05,3,SH) { Species = 823, Ability = A4, Moves = new[]{ 065, 211, 034, 103 }, Index = 7, CanGigantamax = true }, // Corviknight
            new(40,05,3,SH) { Species = 851, Ability = A4, Moves = new[]{ 141, 438, 053, 489 }, Index = 7 }, // Centiskorch
            new(40,05,3,SH) { Species = 851, Ability = A4, Moves = new[]{ 141, 438, 053, 489 }, Index = 7, CanGigantamax = true }, // Centiskorch
            new(50,08,4,SH) { Species = 823, Ability = A4, Moves = new[]{ 065, 442, 034, 334 }, Index = 7, CanGigantamax = true }, // Corviknight
            new(50,08,4,SH) { Species = 851, Ability = A4, Moves = new[]{ 141, 424, 422, 242 }, Index = 7 }, // Centiskorch
            new(50,08,4,SH) { Species = 851, Ability = A4, Moves = new[]{ 141, 424, 422, 242 }, Index = 7, CanGigantamax = true }, // Centiskorch
            new(60,10,5,SH) { Species = 012, Ability = A4, Moves = new[]{ 405, 094, 403, 079 }, Index = 7, CanGigantamax = true }, // Butterfree
            new(70,10,5,SH) { Species = 823, Ability = A4, Moves = new[]{ 413, 442, 249, 103 }, Index = 7, CanGigantamax = true }, // Corviknight
            new(60,10,5,SH) { Species = 851, Ability = A4, Moves = new[]{ 679, 257, 083, 438 }, Index = 7 }, // Centiskorch
            new(70,10,5,SH) { Species = 851, Ability = A4, Moves = new[]{ 679, 257, 083, 438 }, Index = 7, CanGigantamax = true }, // Centiskorch

            new(17,01,1,SH) { Species = 131, Ability = A4, Moves = new[]{ 055, 496, 045, 047 }, Index = 8 }, // Lapras
            new(17,01,1,SH) { Species = 131, Ability = A2, Moves = new[]{ 055, 496, 045, 047 }, Index = 8 }, // Lapras
            new(30,03,2,SH) { Species = 842, Ability = A4, Moves = new[]{ 787, 029, 389, 073 }, Index = 8 }, // Appletun
            new(30,03,2,SH) { Species = 842, Ability = A2, Moves = new[]{ 787, 029, 389, 073 }, Index = 8 }, // Appletun
            new(30,03,2,SH) { Species = 131, Ability = A4, Moves = new[]{ 352, 420, 109, 047 }, Index = 8 }, // Lapras
            new(30,03,2,SH) { Species = 131, Ability = A2, Moves = new[]{ 352, 420, 109, 047 }, Index = 8 }, // Lapras
            new(40,05,3,SH) { Species = 842, Ability = A4, Moves = new[]{ 787, 496, 406, 523 }, Index = 8 }, // Appletun
            new(40,05,3,SH) { Species = 842, Ability = A4, Moves = new[]{ 787, 496, 406, 523 }, Index = 8, CanGigantamax = true }, // Appletun
            new(40,05,3,SH) { Species = 869, Ability = A4, Moves = new[]{ 577, 605, 202, 186 }, Index = 8, Form = 1, CanGigantamax = true }, // Alcremie-1
            new(40,05,3,SH) { Species = 869, Ability = A4, Moves = new[]{ 577, 605, 202, 186 }, Index = 8, Form = 2, CanGigantamax = true }, // Alcremie-2
            new(40,05,3,SH) { Species = 131, Ability = A4, Moves = new[]{ 352, 420, 109, 034 }, Index = 8 }, // Lapras
            new(40,05,3,SH) { Species = 131, Ability = A4, Moves = new[]{ 352, 420, 109, 034 }, Index = 8, CanGigantamax = true }, // Lapras
            new(50,08,4,SH) { Species = 842, Ability = A4, Moves = new[]{ 787, 202, 406, 089 }, Index = 8 }, // Appletun
            new(50,08,4,SH) { Species = 842, Ability = A4, Moves = new[]{ 787, 202, 406, 089 }, Index = 8, CanGigantamax = true }, // Appletun
            new(50,08,4,SH) { Species = 869, Ability = A4, Moves = new[]{ 605, 202, 094, 151 }, Index = 8, Form = 7, CanGigantamax = true }, // Alcremie-7
            new(50,08,4,SH) { Species = 869, Ability = A4, Moves = new[]{ 605, 202, 094, 151 }, Index = 8, Form = 8, CanGigantamax = true }, // Alcremie-8
            new(50,08,4,SH) { Species = 131, Ability = A4, Moves = new[]{ 057, 058, 246, 523 }, Index = 8 }, // Lapras
            new(50,08,4,SH) { Species = 131, Ability = A4, Moves = new[]{ 057, 058, 246, 523 }, Index = 8, CanGigantamax = true }, // Lapras
            new(60,10,5,SH) { Species = 842, Ability = A4, Moves = new[]{ 787, 406, 412, 089 }, Index = 8 }, // Appletun
            new(60,10,5,SH) { Species = 842, Ability = A4, Moves = new[]{ 787, 406, 412, 089 }, Index = 8, CanGigantamax = true }, // Appletun
            new(60,10,5,SH) { Species = 869, Ability = A4, Moves = new[]{ 605, 202, 595, 500 }, Index = 8, Form = 3, CanGigantamax = true }, // Alcremie-3
            new(60,10,5,SH) { Species = 869, Ability = A4, Moves = new[]{ 605, 202, 595, 500 }, Index = 8, Form = 4, CanGigantamax = true }, // Alcremie-4
            new(60,10,5,SH) { Species = 131, Ability = A4, Moves = new[]{ 057, 196, 573, 329 }, Index = 8 }, // Lapras
            new(60,10,5,SH) { Species = 131, Ability = A4, Moves = new[]{ 057, 196, 573, 329 }, Index = 8, CanGigantamax = true }, // Lapras

            new(40,05,3,SH) { Species = 842, Ability = A4, Moves = new[]{ 787, 496, 406, 523 }, Index = 9, CanGigantamax = true }, // Appletun
            new(40,05,3,SH) { Species = 131, Ability = A4, Moves = new[]{ 352, 420, 109, 034 }, Index = 9, CanGigantamax = true }, // Lapras
            new(50,08,4,SH) { Species = 842, Ability = A4, Moves = new[]{ 787, 202, 406, 089 }, Index = 9, CanGigantamax = true }, // Appletun
            new(50,08,4,SH) { Species = 131, Ability = A4, Moves = new[]{ 057, 058, 246, 523 }, Index = 9, CanGigantamax = true }, // Lapras
            new(60,10,5,SH) { Species = 842, Ability = A4, Moves = new[]{ 787, 406, 412, 089 }, Index = 9, CanGigantamax = true }, // Appletun
            new(60,10,5,SH) { Species = 131, Ability = A4, Moves = new[]{ 057, 196, 573, 329 }, Index = 9, CanGigantamax = true }, // Lapras

            new(40,05,3,SH) { Species = 826, Ability = A4, Moves = new[]{ 522, 060, 109, 202 }, Index = 10, CanGigantamax = true }, // Orbeetle
            new(40,05,3,SH) { Species = 857, Ability = A4, Moves = new[]{ 605, 345, 399, 500 }, Index = 10 }, // Hattrem
            new(40,05,3,SH) { Species = 849, Ability = A4, Moves = new[]{ 085, 599, 496, 103 }, Index = 10, Form = 1, CanGigantamax = true }, // Toxtricity-1
            new(50,08,4,SH) { Species = 826, Ability = A4, Moves = new[]{ 405, 094, 202, 247 }, Index = 10, CanGigantamax = true }, // Orbeetle
            new(50,08,4,SH) { Species = 858, Ability = A4, Moves = new[]{ 605, 094, 595, 247 }, Index = 10, CanGigantamax = true }, // Hatterene
            new(50,08,4,SH) { Species = 849, Ability = A4, Moves = new[]{ 786, 506, 599, 409 }, Index = 10, Form = 1, CanGigantamax = true }, // Toxtricity-1
            new(60,10,5,SH) { Species = 826, Ability = A4, Moves = new[]{ 405, 094, 247, 347 }, Index = 10, CanGigantamax = true }, // Orbeetle
            new(60,10,5,SH) { Species = 858, Ability = A4, Moves = new[]{ 605, 094, 595, 500 }, Index = 10, CanGigantamax = true }, // Hatterene
            new(60,10,5,SH) { Species = 849, Ability = A4, Moves = new[]{ 786, 586, 482, 506 }, Index = 10, Form = 1, CanGigantamax = true }, // Toxtricity-1

            new(17,01,1,SH) { Species = 825, Ability = A4, Moves = new[]{ 093, 522, 113, 115 }, Index = 11 }, // Dottler
            new(17,01,1,SH) { Species = 856, Ability = A4, Moves = new[]{ 093, 589, 791, 574 }, Index = 11 }, // Hatenna
            new(30,03,2,SH) { Species = 826, Ability = A4, Moves = new[]{ 405, 060, 496, 095 }, Index = 11 }, // Orbeetle
            new(30,03,2,SH) { Species = 826, Ability = A4, Moves = new[]{ 405, 060, 496, 095 }, Index = 11, CanGigantamax = true }, // Orbeetle
            new(30,03,2,SH) { Species = 856, Ability = A4, Moves = new[]{ 605, 060, 345, 347 }, Index = 11 }, // Hatenna
            new(30,03,2,SH) { Species = 849, Ability = A4, Moves = new[]{ 599, 209, 268, 175 }, Index = 11, Form = 1 }, // Toxtricity-1
            new(40,05,3,SH) { Species = 826, Ability = A4, Moves = new[]{ 405, 060, 202, 109 }, Index = 11 }, // Orbeetle
            new(40,05,3,SH) { Species = 826, Ability = A4, Moves = new[]{ 405, 060, 202, 109 }, Index = 11, CanGigantamax = true }, // Orbeetle
            new(40,05,3,SH) { Species = 857, Ability = A4, Moves = new[]{ 605, 345, 399, 500 }, Index = 11 }, // Hattrem
            new(40,05,3,SH) { Species = 849, Ability = A4, Moves = new[]{ 085, 599, 496, 103 }, Index = 11, Form = 1 }, // Toxtricity-1
            new(40,05,3,SH) { Species = 849, Ability = A4, Moves = new[]{ 085, 599, 496, 103 }, Index = 11, Form = 1, CanGigantamax = true }, // Toxtricity-1
            new(50,08,4,SH) { Species = 826, Ability = A4, Moves = new[]{ 405, 094, 202, 247 }, Index = 11 }, // Orbeetle
            new(50,08,4,SH) { Species = 826, Ability = A4, Moves = new[]{ 405, 094, 202, 247 }, Index = 11, CanGigantamax = true }, // Orbeetle
            new(50,08,4,SH) { Species = 858, Ability = A4, Moves = new[]{ 605, 094, 595, 247 }, Index = 11 }, // Hatterene
            new(50,08,4,SH) { Species = 858, Ability = A4, Moves = new[]{ 605, 094, 595, 247 }, Index = 11, CanGigantamax = true }, // Hatterene
            new(50,08,4,SH) { Species = 849, Ability = A4, Moves = new[]{ 786, 506, 599, 409 }, Index = 11, Form = 1 }, // Toxtricity-1
            new(50,08,4,SH) { Species = 849, Ability = A4, Moves = new[]{ 786, 506, 599, 409 }, Index = 11, Form = 1, CanGigantamax = true }, // Toxtricity-1
            new(60,10,5,SH) { Species = 826, Ability = A4, Moves = new[]{ 405, 094, 247, 347 }, Index = 11 }, // Orbeetle
            new(60,10,5,SH) { Species = 826, Ability = A4, Moves = new[]{ 405, 094, 247, 347 }, Index = 11, CanGigantamax = true }, // Orbeetle
            new(60,10,5,SH) { Species = 858, Ability = A4, Moves = new[]{ 605, 094, 595, 500 }, Index = 11 }, // Hatterene
            new(60,10,5,SH) { Species = 858, Ability = A4, Moves = new[]{ 605, 094, 595, 500 }, Index = 11, CanGigantamax = true }, // Hatterene
            new(60,10,5,SH) { Species = 849, Ability = A4, Moves = new[]{ 786, 586, 188, 506 }, Index = 11, Form = 1 }, // Toxtricity-1
            new(60,10,5,SH) { Species = 849, Ability = A4, Moves = new[]{ 786, 586, 188, 506 }, Index = 11, Form = 1, CanGigantamax = true }, // Toxtricity-1

            new(17,01,1,SH) { Species = 825, Ability = A4, Moves = new[]{ 093, 522, 113, 115 }, Index = 12 }, // Dottler
            new(17,01,1,SH) { Species = 856, Ability = A4, Moves = new[]{ 093, 589, 791, 574 }, Index = 12 }, // Hatenna
            new(30,03,2,SH) { Species = 826, Ability = A4, Moves = new[]{ 405, 060, 496, 095 }, Index = 12, CanGigantamax = true }, // Orbeetle
            new(30,03,2,SH) { Species = 856, Ability = A4, Moves = new[]{ 605, 060, 345, 347 }, Index = 12 }, // Hatenna
            new(30,03,2,SH) { Species = 849, Ability = A4, Moves = new[]{ 599, 209, 268, 175 }, Index = 12, Form = 1 }, // Toxtricity-1
            new(40,05,3,SH) { Species = 826, Ability = A4, Moves = new[]{ 405, 060, 202, 109 }, Index = 12, CanGigantamax = true }, // Orbeetle
            new(40,05,3,SH) { Species = 857, Ability = A4, Moves = new[]{ 605, 345, 399, 500 }, Index = 12 }, // Hattrem
            new(40,05,3,SH) { Species = 849, Ability = A4, Moves = new[]{ 085, 599, 496, 103 }, Index = 12, Form = 1, CanGigantamax = true }, // Toxtricity-1
            new(50,08,4,SH) { Species = 826, Ability = A4, Moves = new[]{ 405, 094, 202, 247 }, Index = 12, CanGigantamax = true }, // Orbeetle
            new(50,08,4,SH) { Species = 858, Ability = A4, Moves = new[]{ 605, 094, 595, 247 }, Index = 12, CanGigantamax = true }, // Hatterene
            new(50,08,4,SH) { Species = 849, Ability = A4, Moves = new[]{ 786, 506, 599, 409 }, Index = 12, Form = 1, CanGigantamax = true }, // Toxtricity-1
            new(60,10,5,SH) { Species = 826, Ability = A4, Moves = new[]{ 405, 094, 247, 347 }, Index = 12, CanGigantamax = true }, // Orbeetle
            new(60,10,5,SH) { Species = 858, Ability = A4, Moves = new[]{ 605, 094, 595, 500 }, Index = 12, CanGigantamax = true }, // Hatterene
            new(60,10,5,SH) { Species = 849, Ability = A4, Moves = new[]{ 786, 586, 188, 506 }, Index = 12, Form = 1, CanGigantamax = true }, // Toxtricity-1

            new(17,01,1,SH) { Species = 825, Ability = A4, Moves = new[]{ 093, 522, 113, 115 }, Index = 13 }, // Dottler
            new(17,01,1,SH) { Species = 856, Ability = A4, Moves = new[]{ 093, 589, 791, 574 }, Index = 13 }, // Hatenna
            new(30,03,2,SH) { Species = 826, Ability = A4, Moves = new[]{ 405, 060, 496, 095 }, Index = 13 }, // Orbeetle
            new(30,03,2,SH) { Species = 826, Ability = A4, Moves = new[]{ 405, 060, 496, 095 }, Index = 13, CanGigantamax = true }, // Orbeetle
            new(30,03,2,SH) { Species = 856, Ability = A4, Moves = new[]{ 605, 060, 345, 347 }, Index = 13 }, // Hatenna
            new(30,03,2,SH) { Species = 849, Ability = A4, Moves = new[]{ 599, 209, 268, 175 }, Index = 13, Form = 1 }, // Toxtricity-1
            new(40,05,3,SH) { Species = 826, Ability = A4, Moves = new[]{ 405, 060, 202, 109 }, Index = 13 }, // Orbeetle
            new(40,05,3,SH) { Species = 826, Ability = A4, Moves = new[]{ 405, 060, 202, 109 }, Index = 13, CanGigantamax = true }, // Orbeetle
            new(40,05,3,SH) { Species = 857, Ability = A4, Moves = new[]{ 605, 345, 399, 500 }, Index = 13 }, // Hattrem
            new(40,05,3,SH) { Species = 849, Ability = A4, Moves = new[]{ 085, 599, 496, 103 }, Index = 13, Form = 1 }, // Toxtricity-1
            new(40,05,3,SH) { Species = 849, Ability = A4, Moves = new[]{ 085, 599, 496, 103 }, Index = 13, Form = 1, CanGigantamax = true }, // Toxtricity-1
            new(50,08,4,SH) { Species = 826, Ability = A4, Moves = new[]{ 405, 094, 202, 247 }, Index = 13 }, // Orbeetle
            new(50,08,4,SH) { Species = 826, Ability = A4, Moves = new[]{ 405, 094, 202, 247 }, Index = 13, CanGigantamax = true }, // Orbeetle
            new(50,08,4,SH) { Species = 858, Ability = A4, Moves = new[]{ 605, 094, 595, 247 }, Index = 13 }, // Hatterene
            new(50,08,4,SH) { Species = 858, Ability = A4, Moves = new[]{ 605, 094, 595, 247 }, Index = 13, CanGigantamax = true }, // Hatterene
            new(50,08,4,SH) { Species = 849, Ability = A4, Moves = new[]{ 786, 506, 599, 409 }, Index = 13, Form = 1 }, // Toxtricity-1
            new(50,08,4,SH) { Species = 849, Ability = A4, Moves = new[]{ 786, 506, 599, 409 }, Index = 13, Form = 1, CanGigantamax = true }, // Toxtricity-1
            new(60,10,5,SH) { Species = 826, Ability = A4, Moves = new[]{ 405, 094, 247, 347 }, Index = 13 }, // Orbeetle
            new(60,10,5,SH) { Species = 826, Ability = A4, Moves = new[]{ 405, 094, 247, 347 }, Index = 13, CanGigantamax = true }, // Orbeetle
            new(60,10,5,SH) { Species = 858, Ability = A4, Moves = new[]{ 605, 094, 595, 500 }, Index = 13 }, // Hatterene
            new(60,10,5,SH) { Species = 858, Ability = A4, Moves = new[]{ 605, 094, 595, 500 }, Index = 13, CanGigantamax = true }, // Hatterene
            new(60,10,5,SH) { Species = 849, Ability = A4, Moves = new[]{ 786, 586, 188, 506 }, Index = 13, Form = 1 }, // Toxtricity-1
            new(60,10,5,SH) { Species = 849, Ability = A4, Moves = new[]{ 786, 586, 188, 506 }, Index = 13, Form = 1, CanGigantamax = true }, // Toxtricity-1

            new(60,10,5,SH) { Species = 094, Ability = A4, Moves = new[]{ 247, 482, 094, 196 }, Index = 14 }, // Gengar
            new(60,10,5,SH) { Species = 094, Ability = A4, Moves = new[]{ 247, 482, 094, 196 }, Index = 14, CanGigantamax = true }, // Gengar

            new(17,01,1,SH) { Species = 554, Ability = A4, Moves = new[]{ 033, 181, 044, 419 }, Index = 15, Form = 1 }, // Darumaka-1
            new(30,03,2,SH) { Species = 083, Ability = A4, Moves = new[]{ 064, 028, 249, 693 }, Index = 15, Form = 1 }, // Farfetch’d-1
            new(30,03,2,SH) { Species = 554, Ability = A4, Moves = new[]{ 423, 029, 424, 280 }, Index = 15, Form = 1 }, // Darumaka-1
            new(40,05,3,SH) { Species = 083, Ability = A2, Moves = new[]{ 280, 693, 348, 413 }, Index = 15, Form = 1 }, // Farfetch’d-1
            new(40,05,3,SH) { Species = 554, Ability = A2, Moves = new[]{ 008, 007, 428, 276 }, Index = 15, Form = 1 }, // Darumaka-1
            new(60,10,5,SH) { Species = 766, Ability = A2, Moves = new[]{ 370, 157, 523, 231 }, Index = 15 }, // Passimian
            new(60,10,5,SH) { Species = 876, Ability = A2, Moves = new[]{ 094, 595, 605, 247 }, Index = 15 }, // Indeedee
            new(60,10,5,SH) { Species = 628, Ability = A2, Moves = new[]{ 413, 276, 442, 157 }, Index = 15 }, // Braviary
            new(60,10,5,SH) { Species = 865, Ability = A2, Moves = new[]{ 370, 413, 211, 675 }, Index = 15 }, // Sirfetch’d
            new(60,10,5,SH) { Species = 094, Ability = A4, Moves = new[]{ 247, 482, 094, 196 }, Index = 15, CanGigantamax = true }, // Gengar

            new(60,10,5,SH) { Species = 569, Ability = A4, Moves = new[]{ 441, 409, 402, 707 }, Index = 16, CanGigantamax = true }, // Garbodor
            new(60,10,5,SH) { Species = 006, Ability = A4, Moves = new[]{ 257, 403, 406, 411 }, Index = 16, CanGigantamax = true, Shiny = Shiny.Never }, // Charizard

            new(60,10,5,SH) { Species = 569, Ability = A4, Moves = new[]{ 441, 499, 402, 707 }, Index = 17, CanGigantamax = true }, // Garbodor
            new(60,10,5,SH) { Species = 006, Ability = A4, Moves = new[]{ 053, 403, 019, 411 }, Index = 17, CanGigantamax = true }, // Charizard

            new(60,10,5,SH) { Species = 569, Ability = A4, Moves = new[]{ 441, 409, 402, 707 }, Index = 18, CanGigantamax = true }, // Garbodor
            new(60,10,5,SH) { Species = 006, Ability = A4, Moves = new[]{ 257, 403, 406, 411 }, Index = 18, CanGigantamax = true, Shiny = Shiny.Never }, // Charizard

            new(17,01,1,SH) { Species = 479, Ability = A4, Moves = new[]{ 104, 435, 084, 109 }, Index = 19, Form = 2 }, // Rotom-2
          //new(17,01,1,SH) { Species = 529, Ability = A4, Moves = new[]{ 189, 232, 056, 468 }, Index = 19 }, // Drilbur - From initial revision: treat this as illegal.
            new(30,03,2,SH) { Species = 479, Ability = A4, Moves = new[]{ 104, 056, 085, 109 }, Index = 19, Form = 2 }, // Rotom-2
            new(40,05,3,SH) { Species = 479, Ability = A4, Moves = new[]{ 521, 435, 085, 056 }, Index = 19, Form = 2 }, // Rotom-2
            new(50,08,4,SH) { Species = 479, Ability = A4, Moves = new[]{ 521, 435, 247, 056 }, Index = 19, Form = 2 }, // Rotom-2
            new(60,10,5,SH) { Species = 479, Ability = A4, Moves = new[]{ 521, 056, 247, 261 }, Index = 19, Form = 2 }, // Rotom-2

            new(17,01,1,SH) { Species = 479, Ability = A4, Moves = new[]{ 104, 056, 084, 109 }, Index = 20, Form = 2 }, // Rotom-2
            new(30,03,2,SH) { Species = 479, Ability = A4, Moves = new[]{ 104, 056, 085, 109 }, Index = 20, Form = 2 }, // Rotom-2
            new(40,05,3,SH) { Species = 479, Ability = A4, Moves = new[]{ 521, 435, 085, 056 }, Index = 20, Form = 2 }, // Rotom-2
            new(50,08,4,SH) { Species = 479, Ability = A4, Moves = new[]{ 521, 435, 247, 056 }, Index = 20, Form = 2 }, // Rotom-2
            new(60,10,5,SH) { Species = 479, Ability = A4, Moves = new[]{ 521, 056, 247, 261 }, Index = 20, Form = 2 }, // Rotom-2

            new(17,01,1,SH) { Species = 012, Ability = A4, Moves = new[]{ 405, 060, 016, 079 }, Index = 24, CanGigantamax = true }, // Butterfree
            new(30,03,2,SH) { Species = 826, Ability = A4, Moves = new[]{ 405, 060, 496, 095 }, Index = 24, CanGigantamax = true }, // Orbeetle
            new(30,03,2,SH) { Species = 068, Ability = A4, Moves = new[]{ 523, 490, 279, 233 }, Index = 24, CanGigantamax = true }, // Machamp
            new(40,05,3,SH) { Species = 826, Ability = A4, Moves = new[]{ 405, 094, 202, 247 }, Index = 24, CanGigantamax = true }, // Orbeetle
            new(40,05,3,SH) { Species = 841, Ability = A4, Moves = new[]{ 406, 788, 491, 334 }, Index = 24, CanGigantamax = true }, // Flapple
            new(40,05,3,SH) { Species = 844, Ability = A4, Moves = new[]{ 693, 529, 201, 091 }, Index = 24, CanGigantamax = true }, // Sandaconda
            new(50,08,4,SH) { Species = 834, Ability = A4, Moves = new[]{ 157, 710, 317, 334 }, Index = 24, CanGigantamax = true }, // Drednaw
            new(50,08,4,SH) { Species = 858, Ability = A4, Moves = new[]{ 605, 094, 595, 247 }, Index = 24, CanGigantamax = true }, // Hatterene
            new(50,08,4,SH) { Species = 006, Ability = A4, Moves = new[]{ 053, 403, 076, 257 }, Index = 24, CanGigantamax = true }, // Charizard
            new(50,08,4,SH) { Species = 012, Ability = A4, Moves = new[]{ 405, 403, 527, 078 }, Index = 24, CanGigantamax = true }, // Butterfree
            new(60,10,5,SH) { Species = 849, Ability = A4, Moves = new[]{ 786, 506, 599, 409 }, Index = 24, Form = 1, CanGigantamax = true }, // Toxtricity-1
            new(60,10,5,SH) { Species = 839, Ability = A4, Moves = new[]{ 246, 053, 157, 523 }, Index = 24, CanGigantamax = true }, // Coalossal
            new(60,10,5,SH) { Species = 068, Ability = A4, Moves = new[]{ 238, 007, 008, 089 }, Index = 24, CanGigantamax = true }, // Machamp
            new(60,10,5,SH) { Species = 879, Ability = A4, Moves = new[]{ 442, 583, 438, 089 }, Index = 24, CanGigantamax = true }, // Copperajah

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

            new(30,03,2,SH) { Species = 849, Ability = A4, Moves = new[]{ 351, 506, 491, 103 }, Index = 36, CanGigantamax = true }, // Toxtricity
            new(30,03,2,SH) { Species = 841, Ability = A4, Moves = new[]{ 406, 073, 491, 184 }, Index = 36, CanGigantamax = true }, // Flapple
            new(40,05,3,SH) { Species = 849, Ability = A4, Moves = new[]{ 435, 506, 474, 103 }, Index = 36, CanGigantamax = true }, // Toxtricity
            new(40,05,3,SH) { Species = 841, Ability = A4, Moves = new[]{ 406, 788, 491, 184 }, Index = 36, CanGigantamax = true }, // Flapple
            new(50,08,4,SH) { Species = 849, Ability = A4, Moves = new[]{ 786, 506, 474, 586 }, Index = 36, CanGigantamax = true }, // Toxtricity
            new(50,08,4,SH) { Species = 841, Ability = A4, Moves = new[]{ 406, 788, 491, 263 }, Index = 36, CanGigantamax = true }, // Flapple
            new(60,10,5,SH) { Species = 849, Ability = A4, Moves = new[]{ 786, 506, 474, 586 }, Index = 36, CanGigantamax = true }, // Toxtricity
            new(60,10,5,SH) { Species = 841, Ability = A4, Moves = new[]{ 406, 788, 491, 263 }, Index = 36, CanGigantamax = true }, // Flapple

            new(30,03,2,SH) { Species = 849, Ability = A4, Moves = new[]{ 351, 506, 491, 103 }, Index = 38, CanGigantamax = true }, // Toxtricity
            new(30,03,2,SH) { Species = 841, Ability = A4, Moves = new[]{ 406, 073, 491, 184 }, Index = 38, CanGigantamax = true }, // Flapple
            new(40,05,3,SH) { Species = 849, Ability = A4, Moves = new[]{ 435, 506, 474, 103 }, Index = 38, CanGigantamax = true }, // Toxtricity
            new(40,05,3,SH) { Species = 841, Ability = A4, Moves = new[]{ 406, 788, 491, 184 }, Index = 38, CanGigantamax = true }, // Flapple
            new(50,08,4,SH) { Species = 849, Ability = A4, Moves = new[]{ 786, 506, 474, 586 }, Index = 38, CanGigantamax = true }, // Toxtricity
            new(50,08,4,SH) { Species = 841, Ability = A4, Moves = new[]{ 406, 788, 491, 263 }, Index = 38, CanGigantamax = true }, // Flapple
            new(60,10,5,SH) { Species = 849, Ability = A4, Moves = new[]{ 786, 506, 474, 586 }, Index = 38, CanGigantamax = true }, // Toxtricity
            new(60,10,5,SH) { Species = 841, Ability = A4, Moves = new[]{ 406, 788, 491, 263 }, Index = 38, CanGigantamax = true }, // Flapple
        };
    }
}
