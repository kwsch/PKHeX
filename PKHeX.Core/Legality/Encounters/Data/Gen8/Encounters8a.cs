using static PKHeX.Core.EncounterUtil;
using static PKHeX.Core.Shiny;
using static PKHeX.Core.EncounterStatic8aCorrelation;

namespace PKHeX.Core;

internal static class Encounters8a
{
    internal static readonly EncounterArea8a[] SlotsLA = EncounterArea8a.GetAreas(Get("la", "la"u8));

    private const byte M = 127; // Middle Height/Weight
    private const byte A = 255; // Max Height/Weight for Alphas
    private const byte U = 128; // Middle Height - Unown

    internal static readonly EncounterStatic8a[] StaticLA =
    [
        // Gifts
        new(722,000,05,M,M) { Location = 006,                      FixedBall = Ball.LAPoke,   Method = Fixed }, // Rowlet
        new(155,000,05,M,M) { Location = 006,                      FixedBall = Ball.LAPoke,   Method = Fixed }, // Cyndaquil
        new(501,000,05,M,M) { Location = 006,                      FixedBall = Ball.LAPoke,   Method = Fixed }, // Oshawott
        new(037,001,40,M,M) { Location = 088,                      FixedBall = Ball.LAPoke,   Method = Fixed }, // Vulpix-1
        new(483,000,65,M,M) { Location = 109, FlawlessIVCount = 3, FixedBall = Ball.LAOrigin, Method = Fixed }, // Dialga
        new(484,000,65,M,M) { Location = 109, FlawlessIVCount = 3, FixedBall = Ball.LAOrigin, Method = Fixed }, // Palkia
        new(493,000,75,M,M) { Location = 109, FlawlessIVCount = 3, FixedBall = Ball.LAPoke,   Method = Fixed, FatefulEncounter = true }, // Arceus

        // Static Encounters - Scripted Table Slots
        new(480,000,70,M,M) { Location = 111, FlawlessIVCount = 3, Moves = new(129,326,832,095) }, // Uxie
        new(481,000,70,M,M) { Location = 104, FlawlessIVCount = 3, Moves = new(129,326,832,105) }, // Mesprit
        new(482,000,70,M,M) { Location = 105, FlawlessIVCount = 3, Moves = new(129,458,326,832) }, // Azelf
        new(485,000,70,M,M) { Location = 068, FlawlessIVCount = 3, Moves = new(442,242,414,463) }, // Heatran
        new(488,000,70,M,M) { Location = 082, FlawlessIVCount = 3, Moves = new(427,094,585,849) }, // Cresselia

        new(641,000,70,M,M) { Location = 090, FlawlessIVCount = 3, Moves = new(326,242,542,846) }, // Tornadus
        new(642,000,70,M,M) { Location = 009, FlawlessIVCount = 3, Moves = new(326,242,087,847) }, // Thundurus
        new(645,000,70,M,M) { Location = 027, FlawlessIVCount = 3, Moves = new(326,242,414,848) }, // Landorus
        new(905,000,70,M,M) { Location = 038, FlawlessIVCount = 3, Moves = new(326,242,585,831) }, // Enamorus

        new(077,000,15    ) { Location = 014, Shiny = Always}, // Ponyta*
        new(442,000,60,M,M) { Location = 043, FlawlessIVCount = 3 }, // Spiritomb
        new(570,001,27    ) { Location = 027, LevelMax = 29 }, // Zorua

        new(489,000,33    ) { Location = 064, FatefulEncounter = true, LevelMax = 36, Moves = new(145,352,151,428) }, // Phione
        new(490,000,50,M,M) { Location = 064, FlawlessIVCount = 3, FatefulEncounter = true, Moves = new(352,428,585,145) }, // Manaphy
        new(491,000,70,M,M) { Location = 010, FlawlessIVCount = 3, FatefulEncounter = true, Moves = new(506,399,094,464) }, // Darkrai
        new(492,000,70,M,M) { Location = 026, FlawlessIVCount = 3, FatefulEncounter = true, Moves = new(403,412,414,465) }, // Shaymin

        // Unown Notes
        new(201,000,25,U) { Location = 040, Method = Fixed }, // Unown A
        new(201,001,25,U) { Location = 056, Method = Fixed }, // Unown B
        new(201,002,25,U) { Location = 081, Method = Fixed }, // Unown C
        new(201,003,25,U) { Location = 008, Method = Fixed }, // Unown D
        new(201,004,25,U) { Location = 022, Method = Fixed }, // Unown E
        new(201,005,25,U) { Location = 010, Method = Fixed }, // Unown F
        new(201,006,25,U) { Location = 017, Method = Fixed }, // Unown G
        new(201,007,25,U) { Location = 006, Method = Fixed }, // Unown H
        new(201,008,25,U) { Location = 023, Method = Fixed }, // Unown I
        new(201,009,25,U) { Location = 072, Method = Fixed }, // Unown J
        new(201,010,25,U) { Location = 043, Method = Fixed }, // Unown K
        new(201,011,25,U) { Location = 086, Method = Fixed }, // Unown L
        new(201,012,25,U) { Location = 037, Method = Fixed }, // Unown M
        new(201,013,25,U) { Location = 009, Method = Fixed }, // Unown N
        new(201,014,25,U) { Location = 102, Method = Fixed }, // Unown O
        new(201,015,25,U) { Location = 075, Method = Fixed }, // Unown P
        new(201,016,25,U) { Location = 058, Method = Fixed }, // Unown Q
        new(201,017,25,U) { Location = 059, Method = Fixed }, // Unown R
        new(201,018,25,U) { Location = 025, Method = Fixed }, // Unown S
        new(201,019,25,U) { Location = 092, Method = Fixed }, // Unown T
        new(201,020,25,U) { Location = 011, Method = Fixed }, // Unown U
        new(201,021,25,U) { Location = 038, Method = Fixed }, // Unown V
        new(201,022,25,U) { Location = 006, Method = Fixed }, // Unown W
        new(201,023,25,U) { Location = 021, Method = Fixed }, // Unown X
        new(201,024,25,U) { Location = 097, Method = Fixed }, // Unown Y
        new(201,025,25,U) { Location = 051, Method = Fixed }, // Unown Z
        new(201,026,25,U) { Location = 142, Method = Fixed }, // Unown ! at Snowfall Hot Spring
        new(201,027,25,U) { Location = 006, Method = Fixed }, // Unown ?

        // Future updates will handle crossovers better.
        new(201,002,25,U) { Location = 010, Method = Fixed }, // Unown C (Coronet Highlands)
        new(201,017,25,U) { Location = 009, Method = Fixed }, // Unown R (Cobalt Coastlands)
        new(201,026,25,U) { Location = 099, Method = Fixed }, // Unown ! (Arena’s Approach)
        new(201,026,25,U) { Location = 141, Method = Fixed }, // Unown ! (Icepeak Arena)
        new(201,023,25,U) { Location = 007, Method = Fixed }, // Unown X
        new(201,024,25,U) { Location = 097, Method = Fixed }, // Unown Y
        new(201,006,25,U) { Location = 007, Method = Fixed }, // Unown G
        new(201,013,25,U) { Location = 129, Method = Fixed }, // Unown N (Sand's Reach)

        new(642,000,70,M,M) { Location = 059, FlawlessIVCount = 3,                 Moves = new(326,242,087,847) }, // Thundurus (Lunker’s Lair)
        new(642,000,70,M,M) { Location = 129, FlawlessIVCount = 3,                 Moves = new(326,242,087,847) }, // Thundurus (Sand’s Reach)
        new(488,000,70,M,M) { Location = 010, FlawlessIVCount = 3,                 Moves = new(427,094,585,849) }, // Cresselia (Coronet Highlands)
        new(491,000,70,M,M) { Location = 074, FlawlessIVCount = 3, FatefulEncounter = true, Moves = new(506,399,094,464) }, // Darkrai (Lonely Spring)

        // Static Encounters
        new(046,000,50,M,M) { Location = 019, Method = Fixed }, // paras01: Paras
        new(390,000,12,M,M) { Location = 007, Method = Fixed }, // hikozaru_01: Chimchar
        new(434,000,20,M,M) { Location = 008, Method = Fixed }, // skunpuu01: Stunky
        new(441,000,34,M,M) { Location = 129, Method = Fixed }, // perap01: Chatot
        new(450,000,34,M,M) { Location = 036, Method = Fixed, Gender = 0 }, // kabaldon01: Hippowdon
        new(459,000,50,M,M) { Location = 101, Method = Fixed, Gender = 1 }, // yukikaburi01: Snover

        new(483,000,65,M,M) { Location = 109, Method = Fixed, FlawlessIVCount = 3 }, // dialga01: Dialga
        new(484,000,65,M,M) { Location = 109, Method = Fixed, FlawlessIVCount = 3 }, // palkia01: Palkia
        new(486,000,70,M,M) { Location = 095, Method = Fixed, FlawlessIVCount = 3 }, // regigigas01: Regigigas
        new(487,001,70,M,M) { Location = 067, Method = Fixed, FlawlessIVCount = 3 }, // giratina02: Giratina-1

        new(362,000,64,A,A) { Location = 011, Method = Fixed, IsAlpha = true,                                  Moves = new(442,059,556,242) }, // onigohri01: Glalie
        new(402,000,12,A,A) { Location = 007, Method = Fixed, IsAlpha = true, Gender = 0,                      Moves = new(206,071,033,332) }, // mev002: Kricketune
        new(416,000,60,A,A) { Location = 022, Method = Fixed, IsAlpha = true, Gender = 1, FlawlessIVCount = 3, Moves = new(188,403,408,405) }, // beequen01: Vespiquen
        new(571,001,58,M,M) { Location = 111, Method = Fixed, IsAlpha = true,                                  Moves = new(555,421,841,417) }, // zoroark01: Zoroark-1
        new(706,001,58,M,M) { Location = 104, Method = Fixed, IsAlpha = true,                                  Moves = new(231,406,842,056) }, // numelgon01: Goodra-1
        new(904,000,58,M,M) { Location = 105, Method = Fixed, IsAlpha = true,                                  Moves = new(301,398,401,038) }, // harysen01: Overqwil
    ];
}
