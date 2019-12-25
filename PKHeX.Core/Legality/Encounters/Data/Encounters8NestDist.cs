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
        };
    }
}
