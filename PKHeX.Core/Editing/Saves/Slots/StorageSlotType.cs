namespace PKHeX.Core;

/// <summary>
/// Extra Slot enumeration to indicate a general type of slot source.
/// </summary>
public enum StorageSlotType
{
    Box,
    Party,

    /// <summary> Battle Box </summary>
    BattleBox,
    /// <summary> Daycare </summary>
    Daycare,
    /// <summary> Global Trade Station (GTS) </summary>
    GTS,
    /// <summary> Fused Legendary Storage </summary>
    Fused,
    /// <summary> Miscellaneous </summary>
    Misc,
    /// <summary> Poké Pelago (Gen7) </summary>
    Resort,
}
