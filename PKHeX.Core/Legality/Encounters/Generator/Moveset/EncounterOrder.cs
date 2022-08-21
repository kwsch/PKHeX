namespace PKHeX.Core;

/// <summary>
/// Enumeration for indicating the various "base type" category of encounter templates.
/// </summary>
public enum EncounterOrder
{
    /// <summary> Bred egg from the Daycare/etc. </summary>
    Egg,
    /// <summary> Mystery Gift data from external distributions. </summary>
    Mystery,
    /// <summary> Static encounter / gift in-game. </summary>
    Static,
    /// <summary> Trade encounter from an in-game NPC. </summary>
    Trade,
    /// <summary> Wild encounter </summary>
    Slot,
}
