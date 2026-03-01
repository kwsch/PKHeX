namespace PKHeX.Core;

/// <summary>
/// Extra Slot enumeration to indicate a general type of slot source.
/// </summary>
public enum StorageSlotType : byte
{
    None = 0,

    /// <summary>
    /// Originated from Box
    /// </summary>
    Box,

    /// <summary>
    /// Originated from Party
    /// </summary>
    Party,

    /// <summary>
    /// Battle Box
    /// </summary>
    BattleBox,

    /// <summary>
    /// Daycare
    /// </summary>
    Daycare,

    /// <summary>
    /// Miscellaneous Origin (usually in-game scripted event recollection)
    /// </summary>
    Scripted,

    /// <summary>
    /// Global Trade Station (GTS)
    /// </summary>
    GTS,

    /// <summary>
    /// Pokémon Global Link (PGL)
    /// </summary>
    PGL,

    /// <summary>
    /// Surprise Trade Upload/Download
    /// </summary>
    SurpriseTrade,

    /// <summary>
    /// Shiny Overworld Cache
    /// </summary>
    /// <remarks>
    /// <see cref="GameVersion.ZA"/> 
    /// </remarks>
    Shiny,

    /// <summary>
    /// Underground area wild Pokémon cache
    /// </summary>
    /// <remarks>
    /// <see cref="GameVersion.BD"/>
    /// <see cref="GameVersion.SP"/>
    /// </remarks>
    Underground,

    /// <summary>
    /// Fused Legendary Storage
    /// </summary>
    Fused,

    /// <summary>
    /// Sub-tag for <see cref="Species.Kyurem"/> differentiation.
    /// </summary>
    FusedKyurem,
    /// <summary>
    /// Sub-tag for <see cref="Species.Solgaleo"/> differentiation.
    /// </summary>
    FusedNecrozmaS,
    /// <summary>
    /// Sub-tag for <see cref="Species.Lunala"/> differentiation.
    /// </summary>
    FusedNecrozmaM,
    /// <summary>
    /// Sub-tag for <see cref="Species.Calyrex"/> differentiation.
    /// </summary>
    FusedCalyrex,

    /// <summary>
    /// Poké Pelago (Gen7)
    /// </summary>
    Resort,

    /// <summary>
    /// Ride Legendary Slot (S/V)
    /// </summary>
    /// <remarks>
    /// <see cref="GameVersion.SL"/>
    /// <see cref="GameVersion.VL"/>
    /// </remarks>
    Ride,

    /// <summary>
    /// Battle Agency (Gen7)
    /// </summary>
    BattleAgency,

    /// <summary>
    /// Gen4 HeartGold/SoulSilver pedometer accessory upload
    /// </summary>
    Pokéwalker,
}
