namespace PKHeX.Core
{
    /// <summary>
    /// Alternate form data has an associated value.
    /// </summary>
    /// <remarks>
    /// <see cref="Species.Furfrou"/> How long (days) the form can last before reverting to AltForm-0 (5 days max)
    /// <see cref="Species.Hoopa"/>: How long (days) the form can last before reverting to AltForm-0 (3 days max)
    /// <see cref="Species.Alcremie"/>: Topping (Strawberry, Star, etc); [0,7]
    /// </remarks>
    public interface IFormArgument
    {
        /// <summary>
        /// Argument for the associated <see cref="PKM.AltForm"/>
        /// </summary>
        uint FormArgument { get; set; }
    }
}