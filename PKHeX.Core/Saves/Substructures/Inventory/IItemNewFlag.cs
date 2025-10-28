namespace PKHeX.Core;

public interface IItemNewFlag
{
    /// <summary> Indicates if the item is NEW-ly obtained and not yet viewed. </summary>
    bool IsNew { get; set; }
}

public interface IItemNewShopFlag
{
    /// <summary> Indicates if the item is NEW-ly available in the shop. </summary>
    bool IsNewShop { get; set; }
}

public interface IItemHeldFlag
{
    /// <summary> Indicates if the item is given to a Pokémon and cannot be given to another. </summary>
    /// <remarks>
    /// This is how the game leases out Mega Stones to one Pokémon at a time.
    /// In Legends: Z-A, the game will show a green checkmark above the item sprite in the bag.
    /// </remarks>
    bool IsHeld { get; set; }
}
