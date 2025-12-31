using System;

namespace PKHeX.Core;
using static LearnEnvironment;

/// <summary>
/// Indicates the group of game(s) that the move was learned in.
/// </summary>
/// <remarks>
/// Each unique set of learnsets has a unique <see cref="LearnEnvironment"/> value.
/// </remarks>
public enum LearnEnvironment : byte
{
    /// <summary>
    /// Sentinel value indicating no environment specified/initial environment.
    /// </summary>
    None,

    /* Gen1 */ RB, YW,
    /* Gen2 */ GS, C, Stadium2,
    /* Gen3 */ RS, E, FR, LG,
    /* Gen4 */ DP, Pt, HGSS,
    /* Gen5 */ BW, B2W2,
    /* Gen6 */ XY, ORAS,
    /* Gen7 */ SM, USUM, GG,
    /* Gen8 */ SWSH, BDSP, PLA,
    /* Gen9 */ SV, ZA,
    HOME,
}

/// <summary>
/// Extension methods for <see cref="LearnEnvironment"/>.
/// </summary>
public static class LearnEnvironmentExtensions
{
    extension(LearnEnvironment value)
    {
        /// <summary>
        /// Indicates whether the <see cref="LearnEnvironment"/> is specified (not <see cref="None"/>), and thus worth indicating.
        /// </summary>
        public bool IsSpecified => value is not None;

        /// <summary>
        /// Gets the generation number [1-n] for the given <see cref="LearnEnvironment"/>.
        /// </summary>
        public byte Generation => value switch
        {
            RB or YW => 1,
            GS or C or Stadium2 => 2,
            RS or E or FR or LG => 3,
            DP or Pt or HGSS => 4,
            BW or B2W2 => 5,
            XY or ORAS => 6,
            SM or USUM or GG => 7,
            SWSH or BDSP or PLA => 8,
            SV or ZA => 9,
            _ => 0,
        };

        /// <summary>
        /// Retrieves the evolution criteria for the given <see cref="LearnEnvironment"/> from the provided <see cref="EvolutionHistory"/>.
        /// </summary>
        public ReadOnlySpan<EvoCriteria> GetEvolutions(EvolutionHistory history) => value switch
        {
            RB or YW => history.Gen1,
            GS or C or Stadium2 => history.Gen2,
            RS or E or FR or LG => history.Gen3,
            DP or Pt or HGSS => history.Gen4,
            BW or B2W2 => history.Gen5,
            XY or ORAS => history.Gen6,
            SM or USUM => history.Gen7,
            GG => history.Gen7b,
            SWSH => history.Gen8,
            PLA => history.Gen8a,
            BDSP => history.Gen8b,
            SV => history.Gen9,
            ZA => history.Gen9a,
            _ => [],
        };
    }
}
