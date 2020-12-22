using System;
using System.Collections.Generic;

namespace PKHeX.Core
{
    internal sealed class MoveParseSource
    {
        private static readonly int[] Empty = Array.Empty<int>();
        public IReadOnlyList<int> CurrentMoves { get; init; } = Empty;
        public IReadOnlyList<int> SpecialSource { get; set; } = Empty;
        public int[] NonTradeBackLevelUpMoves { get; set; } = Empty;

        /// <summary>
        /// Base moves from a standard encounter
        /// </summary>
        public int[] Base { get; set; } = Empty;

        public int[] EggLevelUpSource { get; set; } = Empty;
        public int[] EggMoveSource { get; set; } = Empty;
        public IReadOnlyList<int> EggEventSource { get; set; } = Empty;

        /// <summary>
        /// Clears all sources except for the <see cref="CurrentMoves"/>.
        /// </summary>
        public void ResetSources()
        {
            EggEventSource = Empty;
            Base = Empty;
            EggLevelUpSource = Empty;
            EggMoveSource = Empty;
            NonTradeBackLevelUpMoves = Empty;
            SpecialSource = Empty;
        }
    }
}
