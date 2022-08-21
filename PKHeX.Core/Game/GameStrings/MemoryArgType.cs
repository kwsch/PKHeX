namespace PKHeX.Core;

/// <summary>
/// Type of memory argument
/// </summary>
public enum MemoryArgType : byte
{
    /// <summary> Memory does not have arguments. </summary>
    None,
    /// <summary> Memory argument is a general location type. </summary>
    GeneralLocation,
    /// <summary> Memory argument is a specific Location ID. </summary>
    SpecificLocation,
    /// <summary> Memory argument is a specific Species ID. </summary>
    Species,
    /// <summary> Memory argument is a specific Move ID. </summary>
    Move,
    /// <summary> Memory argument is a specific Item ID. </summary>
    Item,
}
