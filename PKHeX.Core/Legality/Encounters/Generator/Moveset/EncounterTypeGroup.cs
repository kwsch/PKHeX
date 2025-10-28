using System;

namespace PKHeX.Core;

/// <summary>
/// Enumeration for indicating the various "base type" category of encounter templates.
/// </summary>
[Flags]
public enum EncounterTypeGroup
{
    /// <summary> Bred egg from the Daycare/etc. </summary>
    Egg = 1 << 0,
    /// <summary> Mystery Gift data from external distributions. </summary>
    Mystery = 1 << 1,
    /// <summary> Static encounter / gift in-game. </summary>
    Static = 1 << 2,
    /// <summary> Trade encounter from an in-game NPC. </summary>
    Trade = 1 << 3,
    /// <summary> Wild encounter </summary>
    Slot = 1 << 4,
}
