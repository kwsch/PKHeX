using System;
using System.Collections.Generic;
using System.Linq;
using static PKHeX.Core.GameVersion;

namespace PKHeX.Core
{
    /// <summary>
    /// Utility class for <see cref="GameVersion"/> logic.
    /// </summary>
    public static class GameUtil
    {
        /// <summary>
        /// List of possible <see cref="GameVersion"/> values a <see cref="PKM.Version"/> can have.
        /// </summary>
        /// <remarks>Ordered roughly by most recent games first.</remarks>
        public static readonly IReadOnlyList<GameVersion> GameVersions = ((GameVersion[])Enum.GetValues(typeof(GameVersion))).Where(z => z < RB && z > 0).Reverse().ToArray();

        /// <summary>
        /// Indicates if the <see cref="GameVersion"/> value is a value used by the games or is an aggregate indicator.
        /// </summary>
        /// <param name="game">Game to check</param>
        public static bool IsValidSavedVersion(this GameVersion game) => 0 < game && game <= RB;

        /// <summary>
        /// Most recent game ID utilized by official games.
        /// </summary>
        public const GameVersion HighestGameID = RB - 1;

        /// <summary>Determines the Version Grouping of an input Version ID</summary>
        /// <param name="Version">Version of which to determine the group</param>
        /// <returns>Version Group Identifier or Invalid if type cannot be determined.</returns>
        public static GameVersion GetMetLocationVersionGroup(GameVersion Version)
        {
            switch (Version)
            {
                // Sidegame
                case CXD:
                    return CXD;
                case GO:
                    return GO;

                // VC Transfers
                case RD: case BU: case YW: case GN:
                case GD: case SV: case C:
                    return USUM;

                // Gen2 -- PK2
                case GS: case GSC:
                    return GSC;

                // Gen3
                case R: case S:
                    return RS;
                case E:
                    return E;
                case FR: case LG:
                    return FR;

                // Gen4
                case D: case P:
                    return DP;
                case Pt:
                    return Pt;
                case HG: case SS:
                    return HGSS;

                // Gen5
                case B: case W:
                    return BW;
                case B2: case W2:
                    return B2W2;

                // Gen6
                case X: case Y:
                    return XY;
                case OR: case AS:
                    return ORAS;

                // Gen7
                case SN: case MN:
                    return SM;
                case US: case UM:
                    return USUM;
                case GP: case GE:
                    return GG;

                // Gen8
                case SW: case SH:
                    return SWSH;

                default:
                    return Invalid;
            }
        }

        /// <summary>
        /// Gets a Version ID from the end of that Generation
        /// </summary>
        /// <param name="generation">Generation ID</param>
        /// <returns>Version ID from requested generation. If none, return <see cref="Invalid"/>.</returns>
        public static GameVersion GetVersion(int generation)
        {
            return generation switch
            {
                1 => RBY,
                2 => C,
                3 => E,
                4 => SS,
                5 => W2,
                6 => AS,
                7 => UM,
                8 => SH,
                _ => Invalid
            };
        }

        /// <summary>
        /// Gets the Generation the <see cref="GameVersion"/> belongs to.
        /// </summary>
        /// <param name="game">Game to retrieve the generation for</param>
        /// <returns>Generation ID</returns>
        public static int GetGeneration(this GameVersion game)
        {
            if (Gen1.Contains(game)) return 1;
            if (Gen2.Contains(game)) return 2;
            if (Gen3.Contains(game)) return 3;
            if (Gen4.Contains(game)) return 4;
            if (Gen5.Contains(game)) return 5;
            if (Gen6.Contains(game)) return 6;
            if (Gen7.Contains(game)) return 7;
            if (Gen8.Contains(game)) return 8;
            return -1;
        }

        /// <summary>
        /// Gets the Generation the <see cref="GameVersion"/> belongs to.
        /// </summary>
        /// <param name="game">Game to retrieve the generation for</param>
        /// <returns>Generation ID</returns>
        public static int GetMaxSpeciesID(this GameVersion game)
        {
            if (Gen1.Contains(game)) return Legal.MaxSpeciesID_1;
            if (Gen2.Contains(game)) return Legal.MaxSpeciesID_2;
            if (Gen3.Contains(game)) return Legal.MaxSpeciesID_3;
            if (Gen4.Contains(game)) return Legal.MaxSpeciesID_4;
            if (Gen5.Contains(game)) return Legal.MaxSpeciesID_5;
            if (Gen6.Contains(game)) return Legal.MaxSpeciesID_6;
            if (Gen7.Contains(game))
            {
                if (SM.Contains(game))
                    return Legal.MaxSpeciesID_7;
                if (USUM.Contains(game))
                    return Legal.MaxSpeciesID_7_USUM;
                if (GG.Contains(game))
                    return Legal.MaxSpeciesID_7b;
                return Legal.MaxSpeciesID_7_USUM;
            }
            if (Gen8.Contains(game)) return Legal.MaxSpeciesID_8;
            return -1;
        }

        /// <summary>
        /// Checks if the <see cref="g1"/> version (or subset versions) is equivalent to <see cref="g2"/>.
        /// </summary>
        /// <param name="g1">Version (set)</param>
        /// <param name="g2">Individual version</param>
        public static bool Contains(this GameVersion g1, int g2) => g1.Contains((GameVersion) g2);

        /// <summary>
        /// Checks if the <see cref="g1"/> version (or subset versions) is equivalent to <see cref="g2"/>.
        /// </summary>
        /// <param name="g1">Version (set)</param>
        /// <param name="g2">Individual version</param>
        public static bool Contains(this GameVersion g1, GameVersion g2)
        {
            if (g1 == g2 || g1 == Any)
                return true;

            switch (g1)
            {
                case RB:
                    return g2 == RD || g2 == BU || g2 == GN;
                case Stadium:
                case EventsGBGen1:
                case VCEvents:
                case RBY:
                    return RB.Contains(g2) || g2 == YW;
                case Gen1:
                    return RBY.Contains(g2) || g2 == Stadium || g2 == EventsGBGen1 || g2 == VCEvents;

                case GS: return g2 == GD || g2 == SV;
                case Stadium2:
                case EventsGBGen2:
                case GSC:
                    return GS.Contains(g2) || g2 == C;
                case Gen2:
                    return GSC.Contains(g2) || g2 == Stadium2 || g2 == EventsGBGen2;
                case GBCartEraOnly:
                    return g2 == Stadium || g2 == Stadium2 || g2 == EventsGBGen1 || g2 == EventsGBGen2;

                case RS: return g2 == R || g2 == S;
                case RSE:
                    return RS.Contains(g2) || g2 == E;
                case FRLG: return g2 == FR || g2 == LG;
                case COLO:
                case XD: return g2 == CXD;
                case CXD: return g2 == COLO || g2 == XD;
                case RSBOX: return RS.Contains(g2) || g2 == E || FRLG.Contains(g2);
                case Gen3:
                    return RSE.Contains(g2) || FRLG.Contains(g2) || CXD.Contains(g2) || g2 == RSBOX;

                case DP: return g2 == D || g2 == P;
                case HGSS: return g2 == HG || g2 == SS;
                case DPPt:
                    return DP.Contains(g2) || g2 == Pt;
                case BATREV: return DP.Contains(g2) || g2 == Pt || HGSS.Contains(g2);
                case Gen4:
                    return DPPt.Contains(g2) || HGSS.Contains(g2) || g2 == BATREV;

                case BW: return g2 == B || g2 == W;
                case B2W2: return g2 == B2 || g2 == W2;
                case Gen5:
                    return BW.Contains(g2) || B2W2.Contains(g2);

                case XY: return g2 == X || g2 == Y;
                case ORAS: return g2 == OR || g2 == AS;
                case Gen6:
                    return XY.Contains(g2) || ORAS.Contains(g2);

                case SM:
                    return g2 == SN || g2 == MN;
                case USUM:
                    return g2 == US || g2 == UM;
                case GG:
                    return g2 == GP || g2 == GE || g2 == GO;
                case Gen7:
                    return SM.Contains(g2) || USUM.Contains(g2) || GG.Contains(g2);

                case SWSH:
                    return g2 == SW || g2 == SH;
                case Gen8:
                    return SWSH.Contains(g2);

                default: return false;
            }
        }

        /// <summary>
        /// List of possible <see cref="GameVersion"/> values within the provided <see cref="generation"/>.
        /// </summary>
        /// <param name="generation">Generation to look within</param>
        /// <param name="pkVersion"></param>
        public static GameVersion[] GetVersionsInGeneration(int generation, int pkVersion)
        {
            if (GG.Contains(pkVersion))
                return new[] {GO, GP, GE};
            return GameVersions.Where(z => z.GetGeneration() == generation).ToArray();
        }

        /// <summary>
        /// List of possible <see cref="GameVersion"/> values within the provided <see cref="IGameValueLimit"/> criteria.
        /// </summary>
        /// <param name="obj">Criteria for retrieving versions</param>
        /// <param name="generation">Generation format minimum (necessary for the CXD/Gen4 swap etc)</param>
        public static IEnumerable<GameVersion> GetVersionsWithinRange(IGameValueLimit obj, int generation = -1)
        {
            if (obj.MaxGameID == Legal.MaxGameID_7b) // edge case
                return new[] {GO, GP, GE};
            var vers = GameVersions.Where(z => z >= (GameVersion)obj.MinGameID && z <= (GameVersion)obj.MaxGameID);
            if (generation < 0)
                return vers;
            if (obj.MaxGameID == Legal.MaxGameID_7 && generation == 7)
                vers = vers.Where(z => z != GO);
            return vers.Where(z => z.GetGeneration() <= generation);
        }
    }
}
