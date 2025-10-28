namespace PKHeX.Core;

/// <summary>
/// Enumeration representing the legality of hidden abilities in encounters.
/// </summary>
/// <remarks>
/// Only used for Encounter Slots to help indicate if the Type of slot can possibly originate with a hidden ability.
/// </remarks>
public enum HiddenAbilityPermission : byte
{
    /// <summary>
    /// Never originates with a hidden ability.
    /// </summary>
    /// <remarks>Default, as coercing a hidden ability to appear requires special encounter conditions.</remarks>
    Never,

    /// <summary> Always appears with a hidden ability. </summary>
    /// <remarks>
    /// <see cref="EncounterSlot5.IsHiddenGrotto"/>
    /// </remarks>
    Always,

    /// <summary> Random, but not 100% guaranteed. </summary>
    /// <remarks>
    /// <see cref="EncounterSlot6XY.IsFriendSafari"/>
    /// <see cref="EncounterSlot6XY.IsHorde"/>
    /// <see cref="EncounterSlot6AO.CanDexNav"/>
    /// <see cref="EncounterSlot6AO.IsHorde"/>
    /// <see cref="EncounterSlot7.IsSOS"/>
    /// </remarks>
    Possible,
}
