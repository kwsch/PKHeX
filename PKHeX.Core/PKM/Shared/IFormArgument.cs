namespace PKHeX.Core
{
    /// <summary>
    /// Alternate form data has an associated value.
    /// </summary>
    /// <remarks>
    /// <see cref="Species.Furfrou"/> How long (days) the form can last before reverting to Form-0 (5 days max)
    /// <see cref="Species.Hoopa"/>: How long (days) the form can last before reverting to Form-0 (3 days max)
    /// <see cref="Species.Alcremie"/>: Topping (Strawberry, Star, etc); [0,7]
    /// <see cref="Species.Yamask"/> How much damage the Pokémon has taken as Yamask-1 [0,9999].
    /// <see cref="Species.Runerigus"/> How much damage the Pokémon has taken as Yamask-1 [0,9999].
    /// </remarks>
    public interface IFormArgument
    {
        /// <summary>
        /// Argument for the associated <see cref="PKM.Form"/>
        /// </summary>
        uint FormArgument { get; set; }
    }
}