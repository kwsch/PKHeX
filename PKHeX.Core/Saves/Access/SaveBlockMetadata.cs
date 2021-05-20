using System.Collections.Generic;
using System.Linq;
using PKHeX.Core;

namespace PKHeX.WinForms
{
    /// <summary>
    /// Utilizes Reflection to obtain all defined accessor property names and object values.
    /// </summary>
    /// <typeparam name="T">Type of accessor</typeparam>
    public sealed class SaveBlockMetadata<T>
    {
        private readonly Dictionary<SaveBlock, string> BlockList;

        public SaveBlockMetadata(ISaveBlockAccessor<T> accessor)
        {
            var aType = accessor.GetType();
            BlockList = aType.GetAllPropertiesOfType<SaveBlock>(accessor);
        }

        public IEnumerable<string> GetSortedBlockList()
        {
            return BlockList.Select(z => z.Value).OrderBy(z => z);
        }

        public SaveBlock GetBlock(string name) => BlockList.First(z => z.Value == name).Key;
    }
}
