namespace PKHeX
{
    public enum GameVersion
    {
        // Not actually stored values, but assigned as properties.
        XD = -11,
        COLO = -10,
        BATREV = -7,
        RSBOX = -5,
        GS = -4,
        C = -3,

        // Indicators
        Invalid = -2,
        Any = -1,
        Unknown = 0,

        // Version IDs, also stored in PKM structure
        /*Gen3*/ S = 1, R = 2, E = 3, FR = 4, LG = 5, CXD = 15,
        /*Gen4*/ D = 10, P = 11, Pt = 12, HG = 7, SS = 8,
        /*Gen5*/ W = 20, B = 21, W2 = 22, B2 = 23,
        /*Gen6*/ X = 24, Y = 25, AS = 26, OR = 27,
        /*Gen7*/ SN = 30, MN = 31,

        // Game Groupings (SaveFile type)
        RBY = 98,
        GSC = 99,
        RS = 100,
        FRLG = 101,
        DP = 102,
        HGSS = 103,
        BW = 104,
        B2W2 = 105,
        XY = 106,
        ORASDEMO = 107,
        ORAS = 108,
        SM = 109,

        // Extra Game Groupings (Generation)
        Gen1, Gen2, Gen3, Gen4, Gen5, Gen6, Gen7
    }
}
