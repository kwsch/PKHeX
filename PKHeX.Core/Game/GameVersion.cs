namespace PKHeX.Core
{
    public enum GameVersion
    {
        // Not actually stored values, but assigned as properties.
        XD = -11,
        COLO = -10,
        BATREV = -7,
        RSBOX = -5,
        GS = -4,

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
        /* GO */ GO = 34,
        /* VC */ RD = 35, GN = 36, BU = 37, YW = 38, // GN = Blue for international release
        GD, SV, C,

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
        Gen1, Gen2, Gen3, Gen4, Gen5, Gen6, Gen7,
        GBCartEraOnly,
        Stadium,
        Stadium2,
        EventsGBGen1,
        EventsGBGen2,
        VCEvents
    }

    public static class Extension
    {
        public static bool Contains(this GameVersion g1, GameVersion g2)
        {
            if (g1 == g2 || g1 == GameVersion.Any)
                return true;

            switch (g1)
            {
                case GameVersion.RBY:
                    return g2 == GameVersion.RD || g2 == GameVersion.BU || g2 == GameVersion.YW || g2 == GameVersion.GN;
                case GameVersion.Gen1:
                    return GameVersion.RBY.Contains(g2) || g2 == GameVersion.Stadium || g2 == GameVersion.EventsGBGen1 || g2 == GameVersion.VCEvents;
                case GameVersion.Stadium:
                case GameVersion.EventsGBGen1:
                case GameVersion.VCEvents:
                    return GameVersion.RBY.Contains(g2);

                case GameVersion.GS: return g2 == GameVersion.GD || g2 == GameVersion.SV;
                case GameVersion.GSC:
                    return GameVersion.GS.Contains(g2) || g2 == GameVersion.C;
                case GameVersion.Gen2:
                    return GameVersion.GSC.Contains(g2) || g2 == GameVersion.Stadium2 || g2 == GameVersion.EventsGBGen2;
                case GameVersion.Stadium2:
                case GameVersion.EventsGBGen2:
                    return GameVersion.GSC.Contains(g2);
                case GameVersion.GBCartEraOnly:
                    return g2 == GameVersion.Stadium || g2 == GameVersion.Stadium2 || g2 == GameVersion.EventsGBGen1 || g2 == GameVersion.EventsGBGen2;

                case GameVersion.RS: return g2 == GameVersion.R || g2 == GameVersion.S;
                case GameVersion.FRLG: return g2 == GameVersion.FR || g2 == GameVersion.LG;
                case GameVersion.CXD: return g2 == GameVersion.COLO || g2 == GameVersion.XD;
                case GameVersion.RSBOX: return GameVersion.RS.Contains(g2) || g2 == GameVersion.E || GameVersion.FRLG.Contains(g2);
                case GameVersion.Gen3:
                    return GameVersion.RS.Contains(g2) || g2 == GameVersion.E || GameVersion.FRLG.Contains(g2) || GameVersion.CXD.Contains(g2) || g2 == GameVersion.RSBOX;

                case GameVersion.DP: return g2 == GameVersion.D || g2 == GameVersion.P;
                case GameVersion.HGSS: return g2 == GameVersion.HG || g2 == GameVersion.SS;
                case GameVersion.BATREV: return GameVersion.DP.Contains(g2) || g2 == GameVersion.Pt || GameVersion.HGSS.Contains(g2);
                case GameVersion.Gen4:
                    return GameVersion.DP.Contains(g2) || g2 == GameVersion.Pt || GameVersion.HGSS.Contains(g2) || g2 == GameVersion.BATREV;

                case GameVersion.BW: return g2 == GameVersion.B || g2 == GameVersion.W;
                case GameVersion.B2W2: return g2 == GameVersion.B2 || g2 == GameVersion.W2;
                case GameVersion.Gen5:
                    return GameVersion.BW.Contains(g2) || GameVersion.B2W2.Contains(g2);

                case GameVersion.XY: return g2 == GameVersion.X || g2 == GameVersion.Y;
                case GameVersion.ORAS: return g2 == GameVersion.OR || g2 == GameVersion.AS;
                case GameVersion.Gen6:
                    return GameVersion.XY.Contains(g2) || GameVersion.ORAS.Contains(g2);

                case GameVersion.SM:
                case GameVersion.Gen7:
                    return g2 == GameVersion.SN || g2 == GameVersion.MN;

                default: return false;
            }
        }
    }
}
