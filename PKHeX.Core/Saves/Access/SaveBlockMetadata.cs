using System.Collections.Generic;
using System.Linq;

namespace PKHeX.Core;

/// <summary>
/// Utilizes Reflection to obtain all defined accessor property names and object values.
/// </summary>
/// <typeparam name="T">Type of accessor</typeparam>
public sealed class SaveBlockMetadata<T>
{
    private readonly Dictionary<string, IDataIndirect> BlockList;

    public SaveBlockMetadata(ISaveBlockAccessor<T> accessor)
    {
        var aType = accessor.GetType();
        BlockList = aType.GetAllPropertiesOfType<IDataIndirect>(accessor);
    }

    public IEnumerable<string> GetSortedBlockList()
    {
        return BlockList.Select(z => z.Key).OrderBy(z => z);
    }

    public IDataIndirect GetBlock(string name) => BlockList.First(z => z.Key == name).Value;
}
