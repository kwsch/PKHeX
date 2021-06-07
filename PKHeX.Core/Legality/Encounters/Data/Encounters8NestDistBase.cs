using static PKHeX.Core.GameVersion;

namespace PKHeX.Core
{
    // Distribution Nest Encounters (BCAT)
    internal static partial class Encounters8Nest
    {
        /// <summary>
        /// Nest distribution raids for <see cref="GameVersion.SWSH"/> available during base game era.
        /// </summary>
        internal static readonly EncounterStatic8ND[] Dist_Base =
        {
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

            new(17,01,1) { Species = 133, Ability = A4, Moves = new[]{ 098, 270, 608, 028 }, Index = 22, CanGigantamax = true }, // Eevee
            new(30,03,2) { Species = 133, Ability = A4, Moves = new[]{ 129, 270, 608, 044 }, Index = 22, CanGigantamax = true }, // Eevee
            new(40,05,3) { Species = 133, Ability = A4, Moves = new[]{ 036, 270, 608, 044 }, Index = 22, CanGigantamax = true }, // Eevee
            new(40,05,3) { Species = 133, Ability = A4, Moves = new[]{ 036, 270, 608, 231 }, Index = 22, CanGigantamax = true }, // Eevee
            new(50,08,4) { Species = 133, Ability = A4, Moves = new[]{ 038, 270, 204, 044 }, Index = 22, CanGigantamax = true }, // Eevee
            new(50,08,4) { Species = 133, Ability = A4, Moves = new[]{ 038, 203, 204, 231 }, Index = 22, CanGigantamax = true }, // Eevee
            new(60,10,5) { Species = 133, Ability = A4, Moves = new[]{ 038, 270, 204, 231 }, Index = 22, CanGigantamax = true }, // Eevee
            new(60,10,5) { Species = 133, Ability = A4, Moves = new[]{ 387, 203, 204, 231 }, Index = 22, CanGigantamax = true }, // Eevee

            new(17,01,1) { Species = 025, Ability = A4, Moves = new[]{ 084, 104, 486, 364 }, Index = 21, CanGigantamax = true }, // Pikachu
            new(30,03,2) { Species = 025, Ability = A4, Moves = new[]{ 021, 209, 097, 364 }, Index = 21, CanGigantamax = true }, // Pikachu
            new(40,05,3) { Species = 025, Ability = A4, Moves = new[]{ 021, 113, 085, 364 }, Index = 21, CanGigantamax = true }, // Pikachu
            new(50,08,4) { Species = 025, Ability = A4, Moves = new[]{ 087, 113, 085, 364 }, Index = 21, CanGigantamax = true }, // Pikachu
            new(50,08,4) { Species = 025, Ability = A4, Moves = new[]{ 087, 113, 057, 085 }, Index = 21, CanGigantamax = true }, // Pikachu
            new(60,10,5) { Species = 025, Ability = A4, Moves = new[]{ 087, 113, 057, 364 }, Index = 21, CanGigantamax = true }, // Pikachu
            new(60,10,5) { Species = 025, Ability = A4, Moves = new[]{ 087, 113, 057, 344 }, Index = 21, CanGigantamax = true }, // Pikachu

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
            new(17,01,1,SW) { Species = 479, Ability = A4, Moves = new[]{ 104, 315, 084, 109 }, Index = 20, Form = 1 }, // Rotom-1
            new(30,03,2,SW) { Species = 479, Ability = A4, Moves = new[]{ 104, 315, 085, 109 }, Index = 20, Form = 1 }, // Rotom-1
            new(40,05,3,SW) { Species = 479, Ability = A4, Moves = new[]{ 521, 315, 085, 506 }, Index = 20, Form = 1 }, // Rotom-1
            new(50,08,4,SW) { Species = 479, Ability = A4, Moves = new[]{ 521, 315, 085, 261 }, Index = 20, Form = 1 }, // Rotom-1
            new(60,10,5,SW) { Species = 479, Ability = A4, Moves = new[]{ 521, 315, 435, 261 }, Index = 20, Form = 1 }, // Rotom-1
            new(17,01,1,SH) { Species = 479, Ability = A4, Moves = new[]{ 104, 056, 084, 109 }, Index = 20, Form = 2 }, // Rotom-2
            new(30,03,2,SH) { Species = 479, Ability = A4, Moves = new[]{ 104, 056, 085, 109 }, Index = 20, Form = 2 }, // Rotom-2
            new(40,05,3,SH) { Species = 479, Ability = A4, Moves = new[]{ 521, 435, 085, 056 }, Index = 20, Form = 2 }, // Rotom-2
            new(50,08,4,SH) { Species = 479, Ability = A4, Moves = new[]{ 521, 435, 247, 056 }, Index = 20, Form = 2 }, // Rotom-2
            new(60,10,5,SH) { Species = 479, Ability = A4, Moves = new[]{ 521, 056, 247, 261 }, Index = 20, Form = 2 }, // Rotom-2

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
            new(17,01,1,SW) { Species = 479, Ability = A4, Moves = new[]{ 104, 315, 084, 109 }, Index = 19, Form = 1 }, // Rotom-1
            new(17,01,1,SW) { Species = 529, Ability = A4, Moves = new[]{ 189, 232, 010, 468 }, Index = 19 }, // Drilbur
            new(30,03,2,SW) { Species = 479, Ability = A4, Moves = new[]{ 104, 315, 085, 109 }, Index = 19, Form = 1 }, // Rotom-1
            new(40,05,3,SW) { Species = 479, Ability = A4, Moves = new[]{ 521, 315, 085, 506 }, Index = 19, Form = 1 }, // Rotom-1
            new(50,08,4,SW) { Species = 479, Ability = A4, Moves = new[]{ 521, 315, 085, 261 }, Index = 19, Form = 1 }, // Rotom-1
            new(60,10,5,SW) { Species = 479, Ability = A4, Moves = new[]{ 521, 315, 435, 261 }, Index = 19, Form = 1 }, // Rotom-1
            new(17,01,1,SH) { Species = 479, Ability = A4, Moves = new[]{ 104, 435, 084, 109 }, Index = 19, Form = 2 }, // Rotom-2
          //new(17,01,1,SH) { Species = 529, Ability = A4, Moves = new[]{ 189, 232, 056, 468 }, Index = 19 }, // Drilbur - From initial revision: treat this as illegal.
            new(30,03,2,SH) { Species = 479, Ability = A4, Moves = new[]{ 104, 056, 085, 109 }, Index = 19, Form = 2 }, // Rotom-2
            new(40,05,3,SH) { Species = 479, Ability = A4, Moves = new[]{ 521, 435, 085, 056 }, Index = 19, Form = 2 }, // Rotom-2
            new(50,08,4,SH) { Species = 479, Ability = A4, Moves = new[]{ 521, 435, 247, 056 }, Index = 19, Form = 2 }, // Rotom-2
            new(60,10,5,SH) { Species = 479, Ability = A4, Moves = new[]{ 521, 056, 247, 261 }, Index = 19, Form = 2 }, // Rotom-2

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
            new(60,10,5,SW) { Species = 879, Ability = A4, Moves = new[]{ 442, 583, 438, 089 }, Index = 17, CanGigantamax = true }, // Copperajah
            new(60,10,5,SW) { Species = 884, Ability = A4, Moves = new[]{ 430, 406, 085, 334 }, Index = 17, CanGigantamax = true }, // Duraludon
            new(60,10,5,SH) { Species = 569, Ability = A4, Moves = new[]{ 441, 499, 402, 707 }, Index = 17, CanGigantamax = true }, // Garbodor
            new(60,10,5,SH) { Species = 006, Ability = A4, Moves = new[]{ 053, 403, 019, 411 }, Index = 17, CanGigantamax = true }, // Charizard

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
            new(60,10,5,SW) { Species = 879, Ability = A4, Moves = new[]{ 442, 583, 438, 089 }, Index = 16, CanGigantamax = true }, // Copperajah
            new(60,10,5,SW) { Species = 884, Ability = A4, Moves = new[]{ 430, 406, 085, 334 }, Index = 16, CanGigantamax = true }, // Duraludon
            new(60,10,5,SH) { Species = 569, Ability = A4, Moves = new[]{ 441, 409, 402, 707 }, Index = 16, CanGigantamax = true }, // Garbodor
            new(60,10,5,SH) { Species = 006, Ability = A4, Moves = new[]{ 257, 403, 406, 411 }, Index = 16, CanGigantamax = true, Shiny = Shiny.Never }, // Charizard

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
            new(60,10,5,SW) { Species = 068, Ability = A4, Moves = new[]{ 238, 007, 008, 089 }, Index = 14 }, // Machamp
            new(60,10,5,SW) { Species = 068, Ability = A4, Moves = new[]{ 238, 007, 008, 089 }, Index = 14, CanGigantamax = true }, // Machamp
            new(60,10,5,SH) { Species = 094, Ability = A4, Moves = new[]{ 247, 482, 094, 196 }, Index = 14 }, // Gengar
            new(60,10,5,SH) { Species = 094, Ability = A4, Moves = new[]{ 247, 482, 094, 196 }, Index = 14, CanGigantamax = true }, // Gengar

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

            new(17,01,1) { Species = 848, Ability = A4, Moves = new[]{ 609, 051, 496, 715 }, Index = 11 }, // Toxel
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

            new(17,01,1) { Species = 868, Ability = A4, Moves = new[]{ 033, 186, 577, 496 }, Index = 10, CanGigantamax = true }, // Milcery
            new(30,03,2) { Species = 868, Ability = A4, Moves = new[]{ 577, 186, 263, 500 }, Index = 10, CanGigantamax = true }, // Milcery
            new(40,05,3) { Species = 868, Ability = A4, Moves = new[]{ 577, 605, 496, 213 }, Index = 10, CanGigantamax = true }, // Milcery
            new(50,08,4) { Species = 868, Ability = A4, Moves = new[]{ 577, 605, 496, 500 }, Index = 10, CanGigantamax = true }, // Milcery
            new(60,10,5) { Species = 868, Ability = A4, Moves = new[]{ 577, 605, 496, 500 }, Index = 10, CanGigantamax = true }, // Milcery
            new(40,05,3,SW) { Species = 099, Ability = A4, Moves = new[]{ 534, 232, 023, 106 }, Index = 10, CanGigantamax = true }, // Kingler
            new(40,05,3,SW) { Species = 860, Ability = A4, Moves = new[]{ 492, 577, 421, 141 }, Index = 10 }, // Morgrem
            new(40,05,3,SW) { Species = 849, Ability = A4, Moves = new[]{ 085, 474, 496, 103 }, Index = 10, CanGigantamax = true }, // Toxtricity
            new(50,08,4,SW) { Species = 099, Ability = A4, Moves = new[]{ 359, 667, 157, 534 }, Index = 10, CanGigantamax = true }, // Kingler
            new(50,08,4,SW) { Species = 861, Ability = A4, Moves = new[]{ 789, 793, 280, 409 }, Index = 10, CanGigantamax = true }, // Grimmsnarl
            new(50,08,4,SW) { Species = 849, Ability = A4, Moves = new[]{ 786, 506, 474, 409 }, Index = 10, CanGigantamax = true }, // Toxtricity
            new(60,10,5,SW) { Species = 099, Ability = A4, Moves = new[]{ 152, 667, 157, 404 }, Index = 10, CanGigantamax = true }, // Kingler
            new(60,10,5,SW) { Species = 861, Ability = A4, Moves = new[]{ 789, 793, 409, 007 }, Index = 10, CanGigantamax = true }, // Grimmsnarl
            new(60,10,5,SW) { Species = 849, Ability = A4, Moves = new[]{ 786, 586, 482, 506 }, Index = 10, CanGigantamax = true }, // Toxtricity
            new(40,05,3,SH) { Species = 826, Ability = A4, Moves = new[]{ 522, 060, 109, 202 }, Index = 10, CanGigantamax = true }, // Orbeetle
            new(40,05,3,SH) { Species = 857, Ability = A4, Moves = new[]{ 605, 345, 399, 500 }, Index = 10 }, // Hattrem
            new(40,05,3,SH) { Species = 849, Ability = A4, Moves = new[]{ 085, 599, 496, 103 }, Index = 10, Form = 1, CanGigantamax = true }, // Toxtricity-1
            new(50,08,4,SH) { Species = 826, Ability = A4, Moves = new[]{ 405, 094, 202, 247 }, Index = 10, CanGigantamax = true }, // Orbeetle
            new(50,08,4,SH) { Species = 858, Ability = A4, Moves = new[]{ 605, 094, 595, 247 }, Index = 10, CanGigantamax = true }, // Hatterene
            new(50,08,4,SH) { Species = 849, Ability = A4, Moves = new[]{ 786, 506, 599, 409 }, Index = 10, Form = 1, CanGigantamax = true }, // Toxtricity-1
            new(60,10,5,SH) { Species = 826, Ability = A4, Moves = new[]{ 405, 094, 247, 347 }, Index = 10, CanGigantamax = true }, // Orbeetle
            new(60,10,5,SH) { Species = 858, Ability = A4, Moves = new[]{ 605, 094, 595, 500 }, Index = 10, CanGigantamax = true }, // Hatterene
            new(60,10,5,SH) { Species = 849, Ability = A4, Moves = new[]{ 786, 586, 482, 506 }, Index = 10, Form = 1, CanGigantamax = true }, // Toxtricity-1

            new(17,01,1) { Species = 868, Ability = A4, Moves = new[]{ 033, 186, 577, 496 }, Index = 9, CanGigantamax = true }, // Milcery
            new(30,03,2) { Species = 868, Ability = A4, Moves = new[]{ 577, 186, 263, 500 }, Index = 9, CanGigantamax = true }, // Milcery
            new(40,05,3) { Species = 868, Ability = A4, Moves = new[]{ 577, 605, 496, 213 }, Index = 9, CanGigantamax = true }, // Milcery
            new(50,08,4) { Species = 868, Ability = A4, Moves = new[]{ 577, 605, 496, 500 }, Index = 9, CanGigantamax = true }, // Milcery
            new(60,10,5) { Species = 868, Ability = A4, Moves = new[]{ 577, 605, 496, 500 }, Index = 9, CanGigantamax = true }, // Milcery
            new(40,05,3,SW) { Species = 841, Ability = A4, Moves = new[]{ 788, 406, 512, 491 }, Index = 9, CanGigantamax = true }, // Flapple
            new(40,05,3,SW) { Species = 839, Ability = A4, Moves = new[]{ 246, 510, 479, 488 }, Index = 9, CanGigantamax = true }, // Coalossal
            new(50,08,4,SW) { Species = 841, Ability = A4, Moves = new[]{ 788, 407, 491, 334 }, Index = 9, CanGigantamax = true }, // Flapple
            new(50,08,4,SW) { Species = 839, Ability = A4, Moves = new[]{ 246, 053, 157, 261 }, Index = 9, CanGigantamax = true }, // Coalossal
            new(60,10,5,SW) { Species = 841, Ability = A4, Moves = new[]{ 407, 788, 512, 349 }, Index = 9, CanGigantamax = true }, // Flapple
            new(60,10,5,SW) { Species = 839, Ability = A4, Moves = new[]{ 246, 053, 157, 523 }, Index = 9, CanGigantamax = true }, // Coalossal
            new(40,05,3,SH) { Species = 842, Ability = A4, Moves = new[]{ 787, 496, 406, 523 }, Index = 9, CanGigantamax = true }, // Appletun
            new(40,05,3,SH) { Species = 131, Ability = A4, Moves = new[]{ 352, 420, 109, 034 }, Index = 9, CanGigantamax = true }, // Lapras
            new(50,08,4,SH) { Species = 842, Ability = A4, Moves = new[]{ 787, 202, 406, 089 }, Index = 9, CanGigantamax = true }, // Appletun
            new(50,08,4,SH) { Species = 131, Ability = A4, Moves = new[]{ 057, 058, 246, 523 }, Index = 9, CanGigantamax = true }, // Lapras
            new(60,10,5,SH) { Species = 842, Ability = A4, Moves = new[]{ 787, 406, 412, 089 }, Index = 9, CanGigantamax = true }, // Appletun
            new(60,10,5,SH) { Species = 131, Ability = A4, Moves = new[]{ 057, 196, 573, 329 }, Index = 9, CanGigantamax = true }, // Lapras

            new(17,01,1) { Species = 840, Ability = A4, Moves = new[]{ 110, 310, 389, 213 }, Index = 8 }, // Applin
            new(17,01,1) { Species = 840, Ability = A2, Moves = new[]{ 110, 310, 389, 213 }, Index = 8 }, // Applin
            new(17,01,1) { Species = 868, Ability = A4, Moves = new[]{ 577, 033, 186, 263 }, Index = 8 }, // Milcery
            new(17,01,1) { Species = 868, Ability = A2, Moves = new[]{ 577, 033, 186, 263 }, Index = 8 }, // Milcery
            new(30,03,2) { Species = 869, Ability = A4, Moves = new[]{ 577, 213, 033, 186 }, Index = 8, CanGigantamax = true }, // Alcremie
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

            new(40,05,3) { Species = 012, Ability = A4, Moves = new[]{ 676, 403, 202, 527 }, Index = 1 }, // Butterfree
            new(40,05,3) { Species = 012, Ability = A4, Moves = new[]{ 676, 403, 202, 527 }, Index = 1, CanGigantamax = true }, // Butterfree
            new(50,08,4) { Species = 012, Ability = A4, Moves = new[]{ 405, 403, 527, 078 }, Index = 1 }, // Butterfree
            new(50,08,4) { Species = 012, Ability = A4, Moves = new[]{ 405, 403, 527, 078 }, Index = 1, CanGigantamax = true }, // Butterfree
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
        };
    }
}
