using System;

namespace PKHeX.Core
{
    /// <summary>
    /// Value passing object to simplify some initialization.
    /// </summary>
    /// <typeparam name="T">Egg Move source type enumeration.</typeparam>
    internal readonly ref struct BreedInfo<T> where T : Enum
    {
        /// <summary> Indicates the analyzed source of each move. </summary>
        public readonly T[] Actual;

        /// <summary> Indicates all possible sources of each move. </summary>
        public readonly byte[] Possible;

        /// <summary> Level Up entry for the egg. </summary>
        public readonly Learnset Learnset;

        /// <summary> Moves the egg knows after it is finalized. </summary>
        public readonly int[] Moves;

        /// <summary> Level the egg originated at. </summary>
        public readonly int Level;

        public BreedInfo(int count, Learnset learnset, int[] moves, int level)
        {
            Possible = new byte[count];
            Actual = new T[count];
            Learnset = learnset;
            Moves = moves;
            Level = level;
        }
    }
}
