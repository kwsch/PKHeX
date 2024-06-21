namespace PKHeX.Core;

/// <summary>
/// Extra Slot enumeration to indicate a general type of slot source.
/// </summary>
public enum StorageSlotType : byte
{
    None = 0,

    Box,
    Party,

    /// <summary> Battle Box </summary>
    BattleBox,
    /// <summary> Daycare </summary>
    Daycare,
    /// <summary> Global Trade Station (GTS) </summary>
    GTS,

    /// <summary> Fused Legendary Storage </summary>
    FusedKyurem,
    FusedNecrozmaS,
    FusedNecrozmaM,
    FusedCalyrex,

    /// <summary> Miscellaneous </summary>
    Misc,
    /// <summary> Poké Pelago (Gen7) </summary>
    Resort,
    /// <summary> Ride Legendary Slot (S/V) </summary>
    Ride,
}
