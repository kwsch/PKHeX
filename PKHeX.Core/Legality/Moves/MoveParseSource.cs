using System;

namespace PKHeX.Core
{
    internal class MoveParseSource
    {
        private static readonly int[] Empty = Array.Empty<int>();
        public int[] CurrentMoves { get; set; } = Empty;
        public int[] SpecialSource { get; set; } = Empty;
        public int[] NonTradeBackLevelUpMoves { get; set; } = Empty;

        /// <summary>
        /// Base moves from a standard encounter
        /// </summary>
        public int[] Base { get; set; } = Empty;

        public int[] EggLevelUpSource { get; set; } = Empty;
        public int[] EggMoveSource { get; set; } = Empty;
        public int[] EggEventSource { get; set; } = Empty;
    }
}
