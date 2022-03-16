namespace PKHeX.Core;

public interface IItemFreeSpace
{
    /// <summary> Indicates if the item should be shown in the Free Space pouch instead (Generation 5). </summary>
    bool IsFreeSpace { get; set; }
}
