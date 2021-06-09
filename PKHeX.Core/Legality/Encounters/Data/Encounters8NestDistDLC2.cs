namespace PKHeX.Core
{
    // Distribution Nest Encounters (BCAT)
    internal static partial class Encounters8Nest
    {
        /// <summary>
        /// Nest distribution raids for <see cref="GameVersion.SWSH"/> available after Crown Tundra expansion.
        /// </summary>
        internal static readonly EncounterStatic8ND[] Dist_DLC2 =
        {
            new(17,01,1) { Species = 002, Ability = A4, Moves = new[]{ 075, 077, 033, 079 }, Index = 71 }, // Ivysaur
            new(17,01,1) { Species = 060, Ability = A4, Moves = new[]{ 055, 095, 001, 341 }, Index = 71 }, // Poliwag
            new(17,01,1) { Species = 453, Ability = A4, Moves = new[]{ 040, 279, 189, 372 }, Index = 71 }, // Croagunk
            new(17,01,1) { Species = 535, Ability = A4, Moves = new[]{ 497, 341, 045, 051 }, Index = 71 }, // Tympole
            new(30,03,2) { Species = 002, Ability = A4, Moves = new[]{ 402, 077, 033, 036 }, Index = 71 }, // Ivysaur
            new(30,03,2) { Species = 061, Ability = A4, Moves = new[]{ 061, 095, 001, 341 }, Index = 71 }, // Poliwhirl
            new(30,03,2) { Species = 453, Ability = A4, Moves = new[]{ 474, 279, 189, 372 }, Index = 71 }, // Croagunk
            new(30,03,2) { Species = 536, Ability = A4, Moves = new[]{ 061, 341, 175, 051 }, Index = 71 }, // Palpitoad
            new(40,05,3) { Species = 003, Ability = A4, Moves = new[]{ 402, 188, 414, 036 }, Index = 71 }, // Venusaur
            new(40,05,3) { Species = 186, Ability = A4, Moves = new[]{ 056, 411, 034, 341 }, Index = 71 }, // Politoed
            new(40,05,3) { Species = 453, Ability = A4, Moves = new[]{ 092, 279, 404, 372 }, Index = 71 }, // Croagunk
            new(40,05,3) { Species = 537, Ability = A4, Moves = new[]{ 503, 341, 438, 051 }, Index = 71 }, // Seismitoad
            new(50,08,4) { Species = 003, Ability = A4, Moves = new[]{ 438, 188, 414, 036 }, Index = 71 }, // Venusaur
            new(50,08,4) { Species = 186, Ability = A4, Moves = new[]{ 056, 411, 034, 414 }, Index = 71 }, // Politoed
            new(50,08,4) { Species = 453, Ability = A4, Moves = new[]{ 188, 067, 404, 372 }, Index = 71 }, // Croagunk
            new(50,08,4) { Species = 537, Ability = A4, Moves = new[]{ 503, 341, 438, 398 }, Index = 71 }, // Seismitoad
            new(60,10,5) { Species = 003, Ability = A4, Moves = new[]{ 438, 188, 414, 034 }, Index = 71, CanGigantamax = true }, // Venusaur
            new(60,10,5) { Species = 003, Ability = A4, Moves = new[]{ 438, 188, 414, 034 }, Index = 71 }, // Venusaur
            new(60,10,5) { Species = 186, Ability = A4, Moves = new[]{ 056, 311, 034, 414 }, Index = 71, Shiny = Shiny.Always }, // Politoed
            new(60,10,5) { Species = 186, Ability = A4, Moves = new[]{ 056, 311, 034, 414 }, Index = 71 }, // Politoed
            new(60,10,5) { Species = 453, Ability = A4, Moves = new[]{ 188, 067, 404, 247 }, Index = 71 }, // Croagunk
            new(60,10,5) { Species = 537, Ability = A4, Moves = new[]{ 503, 089, 438, 398 }, Index = 71 }, // Seismitoad

            new(17,01,1) { Species = 831, Ability = A4, Moves = new[]{ 029, 024, 045, 033 }, Index = 69 }, // Wooloo
            new(30,03,2) { Species = 831, Ability = A4, Moves = new[]{ 029, 024, 528, 033 }, Index = 69 }, // Wooloo
            new(30,03,2) { Species = 832, Ability = A4, Moves = new[]{ 036, 024, 528, 371 }, Index = 69 }, // Dubwool
            new(40,05,3) { Species = 831, Ability = A4, Moves = new[]{ 029, 179, 528, 024 }, Index = 69 }, // Wooloo
            new(40,05,3) { Species = 832, Ability = A4, Moves = new[]{ 036, 179, 528, 371 }, Index = 69 }, // Dubwool
            new(50,08,4) { Species = 831, Ability = A4, Moves = new[]{ 029, 179, 528, 371 }, Index = 69 }, // Wooloo
            new(50,08,4) { Species = 832, Ability = A4, Moves = new[]{ 036, 179, 528, 024 }, Index = 69 }, // Dubwool
            new(60,10,5) { Species = 831, Ability = A4, Moves = new[]{ 038, 179, 086, 371 }, Index = 69, Shiny = Shiny.Always }, // Wooloo
            new(60,10,5) { Species = 831, Ability = A4, Moves = new[]{ 038, 179, 086, 371 }, Index = 69 }, // Wooloo
            new(60,10,5) { Species = 832, Ability = A4, Moves = new[]{ 776, 038, 086, 371 }, Index = 69 }, // Dubwool

            new(17,01,1) { Species = 052, Ability = A4, Moves = new[]{ 006, 232, 442, 583 }, Index = 67, Form = 2 }, // Meowth-2
            new(17,01,1) { Species = 052, Ability = A4, Moves = new[]{ 006, 583, 196, 675 }, Index = 67, Form = 1 }, // Meowth-1
            new(17,01,1) { Species = 052, Ability = A4, Moves = new[]{ 006, 492, 402, 247 }, Index = 67 }, // Meowth
            new(17,01,1) { Species = 052, Ability = A4, Moves = new[]{ 006, 441, 087, 231 }, Index = 67 }, // Meowth
            new(30,03,2) { Species = 052, Ability = A4, Moves = new[]{ 006, 232, 442, 583 }, Index = 67, Form = 2 }, // Meowth-2
            new(30,03,2) { Species = 052, Ability = A4, Moves = new[]{ 006, 583, 196, 675 }, Index = 67, Form = 1 }, // Meowth-1
            new(30,03,2) { Species = 052, Ability = A4, Moves = new[]{ 006, 492, 402, 247 }, Index = 67 }, // Meowth
            new(30,03,2) { Species = 052, Ability = A4, Moves = new[]{ 006, 441, 087, 231 }, Index = 67 }, // Meowth
            new(40,05,3) { Species = 052, Ability = A4, Moves = new[]{ 006, 232, 442, 583 }, Index = 67, Form = 2 }, // Meowth-2
            new(40,05,3) { Species = 052, Ability = A4, Moves = new[]{ 006, 583, 196, 675 }, Index = 67, Form = 1 }, // Meowth-1
            new(40,05,3) { Species = 052, Ability = A4, Moves = new[]{ 006, 492, 402, 247 }, Index = 67 }, // Meowth
            new(40,05,3) { Species = 052, Ability = A4, Moves = new[]{ 006, 441, 087, 231 }, Index = 67 }, // Meowth
            new(50,08,4) { Species = 052, Ability = A4, Moves = new[]{ 006, 232, 442, 583 }, Index = 67, Form = 2 }, // Meowth-2
            new(50,08,4) { Species = 052, Ability = A4, Moves = new[]{ 006, 583, 196, 675 }, Index = 67, Form = 1 }, // Meowth-1
            new(50,08,4) { Species = 052, Ability = A4, Moves = new[]{ 006, 492, 402, 247 }, Index = 67 }, // Meowth
            new(50,08,4) { Species = 052, Ability = A4, Moves = new[]{ 006, 441, 087, 231 }, Index = 67 }, // Meowth
            new(60,10,5) { Species = 052, Ability = A4, Moves = new[]{ 006, 232, 442, 583 }, Index = 67, Form = 2, Shiny = Shiny.Always }, // Meowth-2
            new(60,10,5) { Species = 052, Ability = A4, Moves = new[]{ 006, 232, 442, 583 }, Index = 67, Form = 2 }, // Meowth-2
            new(60,10,5) { Species = 052, Ability = A4, Moves = new[]{ 006, 583, 196, 675 }, Index = 67, Form = 1 }, // Meowth-1
            new(60,10,5) { Species = 052, Ability = A4, Moves = new[]{ 006, 492, 402, 247 }, Index = 67 }, // Meowth
            new(60,10,5) { Species = 052, Ability = A4, Moves = new[]{ 006, 441, 087, 231 }, Index = 67 }, // Meowth
            new(60,10,5) { Species = 052, Ability = A4, Moves = new[]{ 006, 583, 421, 034 }, Index = 67, CanGigantamax = true }, // Meowth

            new(17,01,1) { Species = 875, Ability = A4, Moves = new[]{ 181, 362, 033, 311 }, Index = 65 }, // Eiscue
            new(30,03,2) { Species = 875, Ability = A4, Moves = new[]{ 196, 362, 033, 029 }, Index = 65 }, // Eiscue
            new(40,05,3) { Species = 875, Ability = A4, Moves = new[]{ 008, 057, 263, 029 }, Index = 65 }, // Eiscue
            new(50,08,4) { Species = 875, Ability = A4, Moves = new[]{ 333, 057, 428, 029 }, Index = 65 }, // Eiscue
            new(60,10,5) { Species = 875, Ability = A4, Moves = new[]{ 333, 710, 442, 029 }, Index = 65, Shiny = Shiny.Always }, // Eiscue
            new(60,10,5) { Species = 875, Ability = A4, Moves = new[]{ 333, 710, 442, 029 }, Index = 65 }, // Eiscue

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

          //new(17,01,1) { Species = 129, Ability = A4, Moves = new[]{ 150, 000, 000, 000 }, Index = 62 }, // Magikarp
          //new(30,03,2) { Species = 129, Ability = A4, Moves = new[]{ 150, 000, 000, 000 }, Index = 62 }, // Magikarp
          //new(40,05,3) { Species = 129, Ability = A4, Moves = new[]{ 150, 000, 000, 000 }, Index = 62 }, // Magikarp
          //new(50,08,4) { Species = 129, Ability = A4, Moves = new[]{ 150, 000, 000, 000 }, Index = 62 }, // Magikarp
          //new(60,10,5) { Species = 129, Ability = A4, Moves = new[]{ 150, 000, 000, 000 }, Index = 62 }, // Magikarp

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

            new(17,01,1) { Species = 868, Ability = A4, Moves = new[]{ 033, 186, 577, 496 }, Index = 56, CanGigantamax = true }, // Milcery
            new(30,03,2) { Species = 868, Ability = A4, Moves = new[]{ 577, 186, 263, 500 }, Index = 56, CanGigantamax = true }, // Milcery
            new(40,05,3) { Species = 868, Ability = A4, Moves = new[]{ 577, 605, 496, 213 }, Index = 56, CanGigantamax = true }, // Milcery
            new(50,08,4) { Species = 868, Ability = A4, Moves = new[]{ 577, 605, 496, 500 }, Index = 56, CanGigantamax = true }, // Milcery
            new(60,10,5) { Species = 868, Ability = A4, Moves = new[]{ 577, 605, 496, 500 }, Index = 56, CanGigantamax = true, Shiny = Shiny.Always }, // Milcery
            new(60,10,5) { Species = 868, Ability = A4, Moves = new[]{ 577, 605, 496, 500 }, Index = 56, CanGigantamax = true }, // Milcery

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
        };
    }
}
