namespace PKHeX.Core;

public interface IItemFreeSpaceIndex
{
    /// <summary> Indicates if the item should be shown in the Free Space pouch instead (Generation 7). </summary>
    uint FreeSpaceIndex { get; set; }
}
