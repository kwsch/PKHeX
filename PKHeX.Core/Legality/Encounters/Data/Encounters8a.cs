using static PKHeX.Core.EncounterUtil;
using static PKHeX.Core.Shiny;
using static PKHeX.Core.GameVersion;

namespace PKHeX.Core;

internal static class Encounters8a
{
    internal static readonly EncounterArea8a[] SlotsLA = EncounterArea8a.GetAreas(Get("la", "la"), PLA);

    private const byte M = 127; // Middle Height/Weight
    private const byte A = 255; // Max Height/Weight for Alphas
    private const byte U = 128; // Middle Height - Unown

    internal static readonly EncounterStatic8a[] StaticLA =
    {
        // Gifts
        new(722,000,05,M,M) { Location = 006,                      Gift = true, Ball = (int)Ball.LAPoke   }, // Rowlet
        new(155,000,05,M,M) { Location = 006,                      Gift = true, Ball = (int)Ball.LAPoke   }, // Cyndaquil
        new(501,000,05,M,M) { Location = 006,                      Gift = true, Ball = (int)Ball.LAPoke   }, // Oshawott
        new(037,001,40,M,M) { Location = 088,                      Gift = true, Ball = (int)Ball.LAPoke   }, // Vulpix-1
        new(483,000,65,M,M) { Location = 109, FlawlessIVCount = 3, Gift = true, Ball = (int)Ball.LAOrigin }, // Dialga
        new(484,000,65,M,M) { Location = 109, FlawlessIVCount = 3, Gift = true, Ball = (int)Ball.LAOrigin }, // Palkia
        new(493,000,75,M,M) { Location = 109, FlawlessIVCount = 3, Gift = true, Ball = (int)Ball.LAPoke, Fateful = true }, // Arceus

        // Static Encounters - Scripted Table Slots
        new(480,000,70,M,M) { Location = 111, FlawlessIVCount = 3 }, // Uxie
        new(481,000,70,M,M) { Location = 104, FlawlessIVCount = 3 }, // Mesprit
        new(482,000,70,M,M) { Location = 105, FlawlessIVCount = 3 }, // Azelf
        new(485,000,70,M,M) { Location = 068, FlawlessIVCount = 3 }, // Heatran
        new(488,000,70,M,M) { Location = 082, FlawlessIVCount = 3 }, // Cresselia

        new(641,000,70,M,M) { Location = 090, FlawlessIVCount = 3 }, // Tornadus
        new(642,000,70,M,M) { Location = 009, FlawlessIVCount = 3 }, // Thundurus
        new(645,000,70,M,M) { Location = 027, FlawlessIVCount = 3 }, // Landorus
        new(905,000,70,M,M) { Location = 038, FlawlessIVCount = 3 }, // Enamorus

        new(077,000,15    ) { Location = 014, Shiny = Always}, // Ponyta*
        new(442,000,60,M,M) { Location = 043, FlawlessIVCount = 3 }, // Spiritomb
        new(570,001,27    ) { Location = 027, FlawlessIVCount = 3 }, // Zorua
        new(570,001,28    ) { Location = 027, FlawlessIVCount = 3 }, // Zorua
        new(570,001,29    ) { Location = 027, FlawlessIVCount = 3 }, // Zorua

        new(489,000,33    ) { Location = 064, Fateful = true }, // Phione
        new(489,000,34    ) { Location = 064, Fateful = true }, // Phione
        new(489,000,35    ) { Location = 064, Fateful = true }, // Phione
        new(489,000,36    ) { Location = 064, Fateful = true }, // Phione
        new(490,000,50,M,M) { Location = 064, FlawlessIVCount = 3, Fateful = true }, // Manaphy
        new(491,000,70,M,M) { Location = 010, FlawlessIVCount = 3, Fateful = true }, // Darkrai
        new(492,000,70,M,M) { Location = 026, FlawlessIVCount = 3, Fateful = true }, // Shaymin

        // Unown Notes
        new(201,000,25,U) { Location = 040 }, // Unown A
        new(201,001,25,U) { Location = 056 }, // Unown B
        new(201,002,25,U) { Location = 081 }, // Unown C
        new(201,003,25,U) { Location = 008 }, // Unown D
        new(201,004,25,U) { Location = 022 }, // Unown E
        new(201,005,25,U) { Location = 010 }, // Unown F
        new(201,006,25,U) { Location = 017 }, // Unown G
        new(201,007,25,U) { Location = 006 }, // Unown H
        new(201,008,25,U) { Location = 023 }, // Unown I
        new(201,009,25,U) { Location = 072 }, // Unown J
        new(201,010,25,U) { Location = 043 }, // Unown K
        new(201,011,25,U) { Location = 086 }, // Unown L
        new(201,012,25,U) { Location = 037 }, // Unown M
        new(201,013,25,U) { Location = 009 }, // Unown N
        new(201,014,25,U) { Location = 102 }, // Unown O
        new(201,015,25,U) { Location = 075 }, // Unown P
        new(201,016,25,U) { Location = 058 }, // Unown Q
        new(201,017,25,U) { Location = 059 }, // Unown R
        new(201,018,25,U) { Location = 025 }, // Unown S
        new(201,019,25,U) { Location = 092 }, // Unown T
        new(201,020,25,U) { Location = 011 }, // Unown U
        new(201,021,25,U) { Location = 038 }, // Unown V
        new(201,022,25,U) { Location = 006 }, // Unown W
        new(201,023,25,U) { Location = 021 }, // Unown X
        new(201,024,25,U) { Location = 097 }, // Unown Y
        new(201,025,25,U) { Location = 051 }, // Unown Z
        new(201,026,25,U) { Location = 142 }, // Unown ! at Snowfall Hot Spring
        new(201,027,25,U) { Location = 006 }, // Unown ?

        // Future updates will handle crossovers better.
        new(201,017,25,U) { Location = 009 }, // Unown R (Cobalt Coastlands)
        new(201,026,25,U) { Location = 099 }, // Unown ! (Arena’s Approach)
        new(201,026,25,U) { Location = 141 }, // Unown ! (Icepeak Arena)
        new(201,023,25,U) { Location = 007 }, // Unown X
        new(201,024,25,U) { Location = 097 }, // Unown Y
        new(201,006,25,U) { Location = 007 }, // Unown G

        // Static Encounters
        new(046,000,50,M,M) { Location = 019 }, // paras01: Paras
        new(390,000,12,M,M) { Location = 007 }, // hikozaru_01: Chimchar
        new(434,000,20,M,M) { Location = 008 }, // skunpuu01: Stunky
        new(441,000,34,M,M) { Location = 129 }, // perap01: Chatot
        new(450,000,34,M,M) { Location = 036, Gender = 0 }, // kabaldon01: Hippowdon
        new(459,000,50,M,M) { Location = 101, Gender = 1 }, // yukikaburi01: Snover

        new(483,000,65,M,M) { Location = 109, FlawlessIVCount = 3 }, // dialga01: Dialga
        new(484,000,65,M,M) { Location = 109, FlawlessIVCount = 3 }, // palkia01: Palkia
        new(486,000,70,M,M) { Location = 095, FlawlessIVCount = 3 }, // regigigas01: Regigigas
        new(487,001,70,M,M) { Location = 067, FlawlessIVCount = 3 }, // giratina02: Giratina-1

        new(362,000,64,A,A) { Location = 011, IsAlpha = true,                                  Moves = new[] {442,059,556,242}, Mastery = new[] {true,true,true, true } }, // onigohri01: Glalie
        new(402,000,12,A,A) { Location = 007, IsAlpha = true, Gender = 0,                      Moves = new[] {206,071,033,332}, Mastery = new[] {true,true,false,false} }, // mev002: Kricketune
        new(416,000,60,A,A) { Location = 022, IsAlpha = true, Gender = 1, FlawlessIVCount = 3, Moves = new[] {188,403,408,405}, Mastery = new[] {true,true,true ,true } }, // beequen01: Vespiquen
        new(571,001,58,M,M) { Location = 111, IsAlpha = true,                                  Moves = new[] {555,421,841,417}, Mastery = new[] {true,true,true ,true } }, // zoroark01: Zoroark-1
        new(706,001,58,M,M) { Location = 104, IsAlpha = true,                                  Moves = new[] {231,406,842,056}, Mastery = new[] {true,true,true ,true } }, // numelgon01: Goodra-1
        new(904,000,58,M,M) { Location = 105, IsAlpha = true,                                  Moves = new[] {301,398,401,038}, Mastery = new[] {true,true,true ,false} }, // harysen01: Overqwil
    };
}
