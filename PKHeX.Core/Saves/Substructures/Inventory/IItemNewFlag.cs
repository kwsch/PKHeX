namespace PKHeX.Core;

public interface IItemNewFlag
{
    /// <summary> Indicates if the item is NEW-ly obtained and not yet viewed. </summary>
    bool IsNew { get; set; }
}
