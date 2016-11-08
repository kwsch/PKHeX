namespace PKHeX
{
    public static class GameUtil
    {
        /// <summary>Determines the Version Grouping of an input Version ID</summary>
        /// <param name="Version">Version of which to determine the group</param>
        /// <returns>Version Group Identifier or Invalid if type cannot be determined.</returns>
        public static GameVersion getMetLocationVersionGroup(GameVersion Version)
        {
            switch (Version)
            {
                case GameVersion.CXD:
                    return GameVersion.CXD;

                case GameVersion.RBY:
                    return GameVersion.RBY;

                case GameVersion.GS:
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
    }
}
