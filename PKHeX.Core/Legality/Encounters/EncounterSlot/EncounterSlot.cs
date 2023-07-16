namespace PKHeX.Core;

/// <summary>
/// Wild Encounter Slot data
/// </summary>
/// <remarks>Wild encounter slots are found as random encounters in-game.</remarks>
public interface EncounterSlot : IEncounterable, IEncounterMatch
{
    string GetConditionString() => LegalityCheckStrings.LEncCondition;
}

public enum HiddenAbilityPermission : byte
{
    Never,
    Always,
    Possible,
}
