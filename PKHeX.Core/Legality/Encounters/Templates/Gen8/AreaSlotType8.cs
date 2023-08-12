using static PKHeX.Core.AreaSlotType8;
using static PKHeX.Core.AreaWeather8;

namespace PKHeX.Core;

/// <summary>
/// Encounter Slot Types for <see cref="GameVersion.SWSH"/>
/// </summary>
public enum AreaSlotType8 : byte
{
    SymbolMain,
    SymbolMain2,
    SymbolMain3,

    HiddenMain, // Both HiddenMain tables include the tree/fishing slots for the area.
    HiddenMain2,

    Surfing,
    Surfing2,
    Sky,
    Sky2,
    Ground,
    Ground2,
    Sharpedo,

    OnlyFishing, // More restricted hidden table that ignores the weather slots like grass Tentacool.
    Inaccessible, // Shouldn't show up since these tables are not dumped.
}

/// <summary>
/// Extension methods for <see cref="AreaSlotType8"/>.
/// </summary>
public static class AreaSlotType8Extensions
{
    public static bool CanCrossover(this AreaSlotType8 type) => type is not (HiddenMain or HiddenMain2 or OnlyFishing);
    public static bool CanEncounterViaFishing(this AreaSlotType8 type, AreaWeather8 weather) => type is OnlyFishing || weather.HasFlag(Fishing);
    public static bool CanEncounterViaCurry(this AreaSlotType8 type) => type is HiddenMain or HiddenMain2;
}
