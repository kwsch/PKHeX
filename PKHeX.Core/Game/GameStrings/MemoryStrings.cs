using System;
using System.Collections.Generic;
using System.Linq;

namespace PKHeX.Core
{
    /// <summary>
    /// Localizing Memory strings
    /// </summary>
    /// <remarks>
    /// <see cref="IMemoryOT"/> and <see cref="IMemoryHT"/> parameters build strings.
    /// </remarks>
    public sealed class MemoryStrings
    {
        private readonly GameStrings s;

        public MemoryStrings(GameStrings strings, int format)
        {
            s = strings;
            memories = new Lazy<List<ComboItem>>(GetMemories);
            none = new Lazy<List<ComboItem>>(() => Util.GetCBList(new[] {string.Empty}));
            species = new Lazy<List<ComboItem>>(() => Util.GetCBList(s.specieslist));
            item = new Lazy<List<ComboItem>>(() => GetItems(format));
            genloc = new Lazy<List<ComboItem>>(() => Util.GetCBList(s.genloc));
            moves = new Lazy<List<ComboItem>>(() => Util.GetCBList(s.movelist)); // Hyperspace Fury
            specific = new Lazy<List<ComboItem>>(() => Util.GetCBList(s.metXY_00000, Legal.Met_XY_0));
        }

        private List<ComboItem> GetItems(int format)
        {
            var permit = Memories.GetMemoryItemParams(format);
            var asInt = permit.Select(z => (int) z).ToArray();
            return Util.GetCBList(s.itemlist, asInt);
        }

        private readonly Lazy<List<ComboItem>> memories;
        private readonly Lazy<List<ComboItem>> none, species, item, genloc, moves, specific;

        public List<ComboItem> Memory => memories.Value;
        public List<ComboItem> None => none.Value;
        public List<ComboItem> Moves => moves.Value;
        public List<ComboItem> Items => item.Value;
        public List<ComboItem> GeneralLocations => genloc.Value;
        public List<ComboItem> SpecificLocations => specific.Value;
        public List<ComboItem> Species => species.Value;

        private List<ComboItem> GetMemories()
        {
            // Memory Chooser
            int memorycount = s.memories.Length - 38;
            string[] mems = new string[memorycount];
            int[] allowed = new int[memorycount];
            for (int i = 0; i < memorycount; i++)
            {
                mems[i] = s.memories[38 + i];
                allowed[i] = i + 1;
            }
            Array.Resize(ref allowed, allowed.Length - 1);
            var memory_list1 = Util.GetCBList(new[] { mems[0] });
            Util.AddCBWithOffset(memory_list1, mems, 0, allowed);
            return memory_list1;
        }

        public ReadOnlySpan<string> GetMemoryQualities() => s.memories.AsSpan(2, 7);
        public ReadOnlySpan<string> GetMemoryFeelings(int format) => format >= 8 ? s.memories.AsSpan(9, 25) : s.memories.AsSpan(10, 24); // empty line for 0 in gen8+

        public List<ComboItem> GetArgumentStrings(MemoryArgType type) => type switch
        {
            MemoryArgType.Species => Species,
            MemoryArgType.GeneralLocation => GeneralLocations,
            MemoryArgType.Item => Items,
            MemoryArgType.Move => Moves,
            MemoryArgType.SpecificLocation => SpecificLocations,
            _ => None
        };
    }
}
