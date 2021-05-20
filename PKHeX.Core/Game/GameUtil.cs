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
        public static readonly IReadOnlyList<GameVersion> GameVersions = ((GameVersion[])Enum.GetValues(typeof(GameVersion))).Where(IsValidSavedVersion).Reverse().ToArray();

        /// <summary>
        /// Indicates if the <see cref="GameVersion"/> value is a value used by the games or is an aggregate indicator.
        /// </summary>
        /// <param name="game">Game to check</param>
        public static bool IsValidSavedVersion(this GameVersion game) => game is > 0 and <= HighestGameID;

        /// <summary>
        /// Most recent game ID utilized by official games.
        /// </summary>
        internal const GameVersion HighestGameID = RB - 1;

        /// <summary>Determines the Version Grouping of an input Version ID</summary>
        /// <param name="version">Version of which to determine the group</param>
        /// <returns>Version Group Identifier or Invalid if type cannot be determined.</returns>
        public static GameVersion GetMetLocationVersionGroup(GameVersion version) => version switch
        {
            // Side games
            CXD => CXD,
            GO => GO,

            // VC Transfers
            RD or BU or YW or GN or GD or SV or C => USUM,

            // Gen2 -- PK2
            GS or GSC => GSC,

            // Gen3
            R or S => RS,
            E => E,
            FR or LG => FR,

            // Gen4
            D or P => DP,
            Pt => Pt,
            HG or SS => HGSS,

            // Gen5
            B or W => BW,
            B2 or W2 => B2W2,

            // Gen6
            X or Y => XY,
            OR or AS => ORAS,

            // Gen7
            SN or MN => SM,
            US or UM => USUM,
            GP or GE => GG,

            // Gen8
            SW or SH => SWSH,
            _ => Invalid,
        };

        /// <summary>
        /// Gets a Version ID from the end of that Generation
        /// </summary>
        /// <param name="generation">Generation ID</param>
        /// <returns>Version ID from requested generation. If none, return <see cref="Invalid"/>.</returns>
        public static GameVersion GetVersion(int generation) => generation switch
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
            if (Gen7b.Contains(game)) return 7;
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
            if (Gen7b.Contains(game)) return Legal.MaxSpeciesID_7b;
            if (Gen7.Contains(game))
            {
                if (SM.Contains(game))
                    return Legal.MaxSpeciesID_7;
                if (USUM.Contains(game))
                    return Legal.MaxSpeciesID_7_USUM;
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

            return g1 switch
            {
                RB => g2 is RD or BU or GN,
                RBY or Stadium => RB.Contains(g2) || g2 == YW,
                Gen1 => RBY.Contains(g2) || g2 == Stadium,

                GS => g2 is GD or SV,
                GSC or Stadium2 => GS.Contains(g2) || g2 == C,
                Gen2 => GSC.Contains(g2) || g2 == Stadium2,

                RS => g2 is R or S,
                RSE => RS.Contains(g2) || g2 == E,
                FRLG => g2 is FR or LG,
                COLO or XD => g2 == CXD,
                CXD => g2 is COLO or XD,
                RSBOX => RS.Contains(g2) || g2 == E || FRLG.Contains(g2),
                Gen3 => RSE.Contains(g2) || FRLG.Contains(g2) || CXD.Contains(g2) || g2 == RSBOX,

                DP => g2 is D or P,
                HGSS => g2 is HG or SS,
                DPPt => DP.Contains(g2) || g2 == Pt,
                BATREV => DP.Contains(g2) || g2 == Pt || HGSS.Contains(g2),
                Gen4 => DPPt.Contains(g2) || HGSS.Contains(g2) || g2 == BATREV,

                BW => g2 is B or W,
                B2W2 => g2 is B2 or W2,
                Gen5 => BW.Contains(g2) || B2W2.Contains(g2),

                XY => g2 is X or Y,
                ORAS => g2 is OR or AS,

                Gen6 => XY.Contains(g2) || ORAS.Contains(g2),
                SM => g2 is SN or MN,
                USUM => g2 is US or UM,
                GG => g2 is GP or GE,
                Gen7 => SM.Contains(g2) || USUM.Contains(g2),
                Gen7b => GG.Contains(g2) || GO == g2,

                SWSH => g2 is SW or SH,
                Gen8 => SWSH.Contains(g2),
                _ => false,
            };
        }

        /// <summary>
        /// List of possible <see cref="GameVersion"/> values within the provided <see cref="generation"/>.
        /// </summary>
        /// <param name="generation">Generation to look within</param>
        /// <param name="pkVersion"></param>
        public static GameVersion[] GetVersionsInGeneration(int generation, int pkVersion)
        {
            if (Gen7b.Contains(pkVersion))
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
            var versions = GameVersions
                .Where(version => (GameVersion)obj.MinGameID <= version && version <= (GameVersion)obj.MaxGameID);
            if (generation < 0)
                return versions;
            if (obj.MaxGameID == Legal.MaxGameID_7 && generation == 7)
                versions = versions.Where(version => version != GO);
            return versions.Where(version => version.GetGeneration() <= generation);
        }
    }
}
