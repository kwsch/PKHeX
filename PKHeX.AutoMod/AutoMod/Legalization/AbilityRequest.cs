namespace PKHeX.Core.AutoMod;

public enum AbilityRequest
{
    /// <summary>
    /// Any ability is fine.
    /// </summary>
    Any,

    /// <summary>
    /// Ability that matches ability number 1.
    /// </summary>
    First,

    /// <summary>
    /// Ability that matches ability number 2.
    /// </summary>
    Second,

    /// <summary>
    /// Any ability that is not a hidden ability.
    /// </summary>
    NotHidden,

    /// <summary>
    /// Requested ability may be possibly hidden (Hidden ability matches one of the requested ability).
    /// </summary>
    PossiblyHidden,

    /// <summary>
    /// Requested ability is hidden.
    /// </summary>
    Hidden,
}
