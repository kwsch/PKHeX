namespace PKHeX.Core;

/// <summary>
/// Exposes details about an encounter with a specific ability index permitted.
/// </summary>
public interface IFixedAbilityNumber
{
    /// <summary>
    /// Specific ability index(es) that can be acquired from this object.
    /// </summary>
    AbilityPermission Ability { get; }
}
