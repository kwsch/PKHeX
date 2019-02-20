using System;
using System.Collections.Generic;
using System.Linq;

namespace PKHeX.Core
{
    public class MemoryStrings
    {
        private readonly GameStrings s;

        public MemoryStrings(GameStrings strings)
        {
            s = strings;
            memories = new Lazy<List<ComboItem>>(GetMemories);
            none = new Lazy<List<ComboItem>>(() => Util.GetCBList(new[] {string.Empty}));
            species = new Lazy<List<ComboItem>>(() => Util.GetCBList(s.specieslist.Take(Legal.MaxSpeciesID_6 + 1).ToArray()));
            item = new Lazy<List<ComboItem>>(() => Util.GetCBList(s.itemlist, Memories.MemoryItems));
            genloc = new Lazy<List<ComboItem>>(() => Util.GetCBList(s.genloc));
            moves = new Lazy<List<ComboItem>>(() => Util.GetCBList(s.movelist.Take(622).ToArray())); // Hyperspace Fury
            specific = new Lazy<List<ComboItem>>(() => Util.GetCBList(s.metXY_00000, Legal.Met_XY_0));
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

        public string[] GetMemoryQualities()
        {
            var list = new string[7];
            for (int i = 0; i < list.Length; i++)
                list[i] = s.memories[2 + i];
            return list;
        }

        public string[] GetMemoryFeelings()
        {
            var list = new string[24];
            for (int i = 0; i < 24; i++)
                list[i] = s.memories[10 + i];
            return list;
        }

        public List<ComboItem> GetArgumentStrings(MemoryArgType memIndex)
        {
            switch (memIndex)
            {
                default:
                    return None;
                case MemoryArgType.Species:
                    return Species;
                case MemoryArgType.GeneralLocation:
                    return GeneralLocations;
                case MemoryArgType.Item:
                    return Items;
                case MemoryArgType.Move:
                    return Moves;
                case MemoryArgType.SpecificLocation:
                    return SpecificLocations;
            }
        }
    }
}
