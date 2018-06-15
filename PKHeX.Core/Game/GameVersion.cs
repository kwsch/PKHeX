namespace PKHeX.Core
{
    /// <summary>
    /// Game Version ID enum shared between actual Version IDs and lumped version groupings.
    /// </summary>
    public enum GameVersion
    {
        // Indicators
        Invalid = -2,
        Any = -1,
        Unknown = 0,

        // Version IDs, also stored in PKM structure
        /*Gen3*/ S = 1, R = 2, E = 3, FR = 4, LG = 5, CXD = 15,
        /*Gen4*/ D = 10, P = 11, Pt = 12, HG = 7, SS = 8,
        /*Gen5*/ W = 20, B = 21, W2 = 22, B2 = 23,
        /*Gen6*/ X = 24, Y = 25, AS = 26, OR = 27,
        /*Gen7*/ SN = 30, MN = 31, US = 32, UM = 33,
        /* GO */ GO = 34,
        /* VC1*/ RD = 35, GN = 36, BU = 37, YW = 38, // GN = Blue for international release
        /* VC2*/ GD = 39, SV = 40, C = 41, // Crystal is unused

        // Not actually stored values, but assigned as properties.

        // Game Groupings (SaveFile type)
        /*SAV1*/ RB, RBY,
        /*SAV2*/ GS, GSC,
        /*SAV3*/ RS, RSE, FRLG, RSBOX, COLO, XD,
        /*SAV4*/ DP, DPPt, HGSS, BATREV,
        /*SAV5*/ BW, B2W2,
        /*SAV6*/ XY, ORASDEMO, ORAS,
        /*SAV7*/ SM, USUM,

        // Extra Game Groupings (Generation)
        Gen1, Gen2, Gen3, Gen4, Gen5, Gen6, Gen7,
        GBCartEraOnly,
        Stadium,
        Stadium2,
        EventsGBGen1,
        EventsGBGen2,
        VCEvents
    }
}
