namespace PKHeX.Core;

public interface IItemFavorite
{
    /// <summary> Indicates if the item should be indicated as a favorite item. </summary>
    bool IsFavorite { get; set; }
}
