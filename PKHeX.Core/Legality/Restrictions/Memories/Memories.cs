using System.Collections.Generic;

namespace PKHeX.Core
{
    /// <summary>
    /// Generation 6 Memory parameters &amp; validation
    /// </summary>
    public static class Memories
    {
        public static readonly MemoryContext6 Memory6 = new();
        public static readonly MemoryContext8 Memory8 = new();

        internal static readonly HashSet<int> MemoryGeneral = new() { 1, 2, 3, 4, 19, 24, 31, 32, 33, 35, 36, 37, 38, 39, 42, 52, 59, 70, 86 };
        private static readonly HashSet<int> MemorySpecific = new() { 6 };
        private static readonly HashSet<int> MemoryMove = new() { 12, 16, 48, 49, 80, 81, 89 };
        private static readonly HashSet<int> MemoryItem = new() { 5, 15, 26, 34, 40, 51, 84, 88 };
        private static readonly HashSet<int> MemorySpecies = new() { 7, 9, 13, 14, 17, 21, 18, 25, 29, 44, 45, 50, 60, 70, 71, 72, 75, 82, 83, 87 };

        public static MemoryArgType GetMemoryArgType(int memory, int format)
        {
            if (MemoryGeneral.Contains(memory)) return MemoryArgType.GeneralLocation;
            if (MemorySpecific.Contains(memory))
            {
                if (format == 6)
                    return MemoryArgType.SpecificLocation;
                return MemoryArgType.GeneralLocation;
            }

            if (MemoryItem.Contains(memory)) return MemoryArgType.Item;
            if (MemoryMove.Contains(memory)) return MemoryArgType.Move;
            if (MemorySpecies.Contains(memory)) return MemoryArgType.Species;

            return MemoryArgType.None;
        }

        public static MemoryContext GetContext(int format) => format switch
        {
            6 or 7 => Memory6,
            _ => Memory8,
        };

        public static IEnumerable<ushort> GetMemoryItemParams(int format) => GetContext(format).GetMemoryItemParams();
    }
}
