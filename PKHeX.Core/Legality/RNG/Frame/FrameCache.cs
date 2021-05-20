using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace PKHeX.Core
{
    /// <summary>
    /// Frame List used to cache <see cref="RNG"/> results.
    /// </summary>
    public sealed class FrameCache
    {
        private const int DefaultSize = 32;
        private readonly List<uint> Seeds = new(DefaultSize);
        private readonly List<uint> Values = new(DefaultSize);
        private readonly Func<uint, uint> Advance;

        /// <summary>
        /// Creates a new instance of a <see cref="FrameCache"/>.
        /// </summary>
        /// <param name="origin">Seed at frame 0.</param>
        /// <param name="advance"><see cref="RNG"/> method used to get the next seed. Can use <see cref="RNG.Next"/> or <see cref="RNG.Prev"/>.</param>
        public FrameCache(uint origin, Func<uint, uint> advance)
        {
            Advance = advance;
            Add(origin);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Add(uint seed)
        {
            Seeds.Add(seed);
            Values.Add(seed >> 16);
        }

        /// <summary>
        /// Gets the 16 bit value from <see cref="Values"/> at a given <see cref="index"/>.
        /// </summary>
        /// <param name="index">Index to grab the value from</param>
        /// <returns></returns>
        public uint this[int index]
        {
            get
            {
                while (index >= Seeds.Count)
                    Add(Advance(Seeds[^1]));
                return Values[index];
            }
        }

        /// <summary>
        /// Gets the Seed at a specified frame index.
        /// </summary>
        /// <param name="index">Frame number</param>
        /// <returns>Seed at index</returns>
        public uint GetSeed(int index)
        {
            while (index >= Seeds.Count)
                Add(Advance(Seeds[^1]));
            return Seeds[index];
        }
    }
}
