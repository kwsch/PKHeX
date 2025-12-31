namespace PKHeX.Core;

/// <summary>
/// Exposes info about <see cref="MoveType"/> that an entity has.
/// </summary>
public interface IPersonalType
{
    /// <summary>
    /// Primary Type
    /// </summary>
    byte Type1 { get; set; }

    /// <summary>
    /// Secondary Type
    /// </summary>
    byte Type2 { get; set; }
}

public static class PersonalTypeExtensions
{
    extension(IPersonalType detail)
    {
        /// <summary>
        /// Checks if the entry has either type equal to the input type.
        /// </summary>
        /// <param name="type1">Type</param>
        /// <returns>Typing is present in entry</returns>
        public bool IsType(byte type1)
        {
            return detail.Type1 == type1 || detail.Type2 == type1;
        }

        /// <summary>
        /// Checks if the entry has either type equal to both input types.
        /// </summary>
        /// <remarks>Input order does not matter.</remarks>
        /// <param name="type1">Type 1</param>
        /// <param name="type2">Type 2</param>
        /// <returns>Typing is present in entry</returns>
        public bool IsType(byte type1, byte type2)
        {
            return (detail.Type1 == type1 && detail.Type2 == type2) || (detail.Type1 == type2 && detail.Type2 == type1);
        }

        /// <summary>
        /// Checks to see if the provided Types match the entry's types.
        /// </summary>
        /// <remarks>Input order matters! If input order does not matter, use <see cref="IsType(IPersonalType,byte,byte)"/> instead.</remarks>
        /// <param name="type1">First type</param>
        /// <param name="type2">Second type</param>
        /// <returns>Typing is an exact match</returns>
        public bool IsValidTypeCombination(byte type1, byte type2) => detail.Type1 == type1 && detail.Type2 == type2;
    }
}
