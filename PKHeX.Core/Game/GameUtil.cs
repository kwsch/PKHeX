namespace PKHeX.Core
{
    public static class GameUtil
    {
        /// <summary>Determines the Version Grouping of an input Version ID</summary>
        /// <param name="Version">Version of which to determine the group</param>
        /// <returns>Version Group Identifier or Invalid if type cannot be determined.</returns>
        public static GameVersion GetMetLocationVersionGroup(GameVersion Version)
        {
            switch (Version)
            {
                case GameVersion.CXD:
                    return GameVersion.CXD;

                case GameVersion.GO:
                    return GameVersion.GO;

                case GameVersion.RBY:
                case GameVersion.RD:
                case GameVersion.BU:
                case GameVersion.YW:
                case GameVersion.GN:
                    return GameVersion.RBY;

                case GameVersion.GS:
                case GameVersion.GD:
                case GameVersion.SV:
                case GameVersion.C:
                    return GameVersion.GSC;

                case GameVersion.R:
                case GameVersion.S:
                    return GameVersion.RS;

                case GameVersion.E:
                    return GameVersion.E;

                case GameVersion.FR:
                case GameVersion.LG:
                    return GameVersion.FR;

                case GameVersion.D:
                case GameVersion.P:
                    return GameVersion.DP;

                case GameVersion.Pt:
                    return GameVersion.Pt;

                case GameVersion.HG:
                case GameVersion.SS:
                    return GameVersion.HGSS;

                case GameVersion.B:
                case GameVersion.W:
                    return GameVersion.BW;

                case GameVersion.B2:
                case GameVersion.W2:
                    return GameVersion.B2W2;

                case GameVersion.X:
                case GameVersion.Y:
                    return GameVersion.XY;

                case GameVersion.OR:
                case GameVersion.AS:
                    return GameVersion.ORAS;

                case GameVersion.SN:
                case GameVersion.MN:
                    return GameVersion.SM;

                default:
                    return GameVersion.Invalid;
            }
        }

        /// <summary>
        /// Gets a Version ID from the end of that Generation
        /// </summary>
        /// <param name="generation">Generation ID</param>
        /// <returns>Version ID from requested generation. If none, return Unknown.</returns>
        public static GameVersion GetVersion(int generation)
        {
            switch (generation)
            {
                case 1: return GameVersion.RBY;
                case 2: return GameVersion.C;
                case 3: return GameVersion.E;
                case 4: return GameVersion.SS;
                case 5: return GameVersion.W2;
                case 6: return GameVersion.AS;
                case 7: return GameVersion.MN;
                default:
                    return GameVersion.Unknown;
            }
        }
    }
}
