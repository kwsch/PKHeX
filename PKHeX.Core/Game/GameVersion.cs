namespace PKHeX.Core
{
    /// <summary>
    /// Game Version ID enum shared between actual Version IDs and lumped version groupings.
    /// </summary>
    public enum GameVersion
    {
        #region Indicators
        Invalid = -2,
        Any = -1,
        Unknown = 0,
        #endregion

        #region Gen3
        /// <summary>
        /// Sapphire (GBA)
        /// </summary>
        S = 1,

        /// <summary>
        /// Ruby (GBA)
        /// </summary>
        R = 2,

        /// <summary>
        /// Emerald (GBA)
        /// </summary>
        E = 3,

        /// <summary>
        /// FireRed (GBA)
        /// </summary>
        FR = 4,

        /// <summary>
        /// LeafGreen (GBA)
        /// </summary>
        LG = 5,

        /// <summary>
        /// Colosseum/XD (GameCube)
        /// </summary>
        CXD = 15,
        #endregion

        #region Gen4
        /// <summary>
        /// Diamond (NDS)
        /// </summary>
        D = 10,

        /// <summary>
        /// Pearl (NDS)
        /// </summary>
        P = 11,

        /// <summary>
        /// Platinum (NDS)
        /// </summary>
        Pt = 12,

        /// <summary>
        /// Heart Gold (NDS)
        /// </summary>
        HG = 7,

        /// <summary>
        /// Soul Silver (NDS)
        /// </summary>
        SS = 8,
        #endregion

        #region Gen5
        /// <summary>
        /// White (NDS)
        /// </summary>
        W = 20,

        /// <summary>
        /// Black (NDS)
        /// </summary>
        B = 21,

        /// <summary>
        /// White 2 (NDS)
        /// </summary>
        W2 = 22,

        /// <summary>
        /// Black 2 (NDS)
        /// </summary>
        B2 = 23,
        #endregion

        #region Gen6
        /// <summary>
        /// X (3DS)
        /// </summary>
        X = 24,

        /// <summary>
        /// Y (3DS)
        /// </summary>
        Y = 25,

        /// <summary>
        /// Alpha Sapphire (3DS)
        /// </summary>
        AS = 26,

        /// <summary>
        /// Omega Ruby (3DS)
        /// </summary>
        OR = 27,
        #endregion

        #region Gen7
        /// <summary>
        /// Sun (3DS)
        /// </summary>
        SN = 30,

        /// <summary>
        /// Moon (3DS)
        /// </summary>
        MN = 31,

        /// <summary>
        /// Ultra Sun (3DS)
        /// </summary>
        US = 32,

        /// <summary>
        /// Ultra Moon (3DS)
        /// </summary>
        UM = 33,
        #endregion

        /// <summary>
        /// Pokémon GO (Unused)
        /// </summary>
        GO = 34,

        #region Virtual Console (3DS) Gen1
        /// <summary>
        /// Red (3DS Virtual Console)
        /// </summary>
        RD = 35,

        /// <summary>
        /// Green[JP]/Blue[INT] (3DS Virtual Console)
        /// </summary>
        GN = 36,

        /// <summary>
        /// Blue[JP] (3DS Virtual Console)
        /// </summary>
        BU = 37,

        /// <summary>
        /// Yellow [JP] (3DS Virtual Console)
        /// </summary>
        YW = 38,
        #endregion

        #region Virtual Console (3DS) Gen2
        /// <summary>
        /// Gold (3DS Virtual Console)
        /// </summary>
        GD = 39,

        /// <summary>
        /// Silver (3DS Virtual Console)
        /// </summary>
        SV = 40,

        /// <summary>
        /// Crystal (3DS Virtual Console)
        /// </summary>
        C = 41,
        #endregion

        #region Nintendo Switch
        /// <summary>
        /// Let's Go Pikachu (NX)
        /// </summary>
        GP = 42,

        /// <summary>
        /// Lets Go Eevee (NX)
        /// </summary>
        GE = 43,
        #endregion

        // Not actually stored values, but assigned as properties.

        // Game Groupings (SaveFile type)
        /*SAV1*/ RB, RBY,
        /*SAV2*/ GS, GSC,
        /*SAV3*/ RS, RSE, FRLG, RSBOX, COLO, XD,
        /*SAV4*/ DP, DPPt, HGSS, BATREV,
        /*SAV5*/ BW, B2W2,
        /*SAV6*/ XY, ORASDEMO, ORAS,
        /*SAV7*/ SM, USUM, GG,

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
