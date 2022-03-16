namespace PKHeX.Core;

public interface IItemNew
{
    /// <summary> Indicates if the item is "NEW"ly obtained and not yet viewed. </summary>
    bool IsNew { get; set; }
}
