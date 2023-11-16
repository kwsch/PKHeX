using System;
using static PKHeX.Core.AreaWeather8;

namespace PKHeX.Core;

/// <summary>
/// Encounter Conditions for <see cref="GameVersion.SWSH"/>
/// </summary>
/// <remarks>Values above <see cref="AreaWeather8.All"/> are for Shaking/Fishing hidden encounters only.</remarks>
[Flags]
public enum AreaWeather8 : ushort
{
    None,
    Normal = 1,
    Overcast = 1 << 1,
    Raining = 1 << 2,
    Thunderstorm = 1 << 3,
    Intense_Sun = 1 << 4,
    Snowing = 1 << 5,
    Snowstorm = 1 << 6,
    Sandstorm = 1 << 7,
    Heavy_Fog = 1 << 8,

    All = Normal | Overcast | Raining | Thunderstorm | Intense_Sun | Snowing | Snowstorm | Sandstorm | Heavy_Fog,
    Stormy = Raining | Thunderstorm,
    Icy = Snowing | Snowstorm,
    All_IoA = Normal | Overcast | Stormy | Intense_Sun | Sandstorm | Heavy_Fog,         // IoA can have everything but snow
    All_CT = Normal | Overcast | Stormy | Intense_Sun | Icy | Heavy_Fog,                // CT can have everything but sand
    No_Sun_Sand = Normal | Overcast | Stormy | Icy | Heavy_Fog,                         // Everything but sand and sun
    All_Ballimere = Normal | Overcast | Stormy | Intense_Sun | Snowing | Heavy_Fog,     // All Ballimere Lake weather

    Shaking_Trees = 1 << 9,
    Fishing = 1 << 10,

    NotWeather = Shaking_Trees | Fishing,
}

/// <summary>
/// Extension methods for <see cref="AreaWeather8"/>.
/// </summary>
public static class AreaWeather8Extensions
{
    public static bool IsMarkCompatible(this AreaWeather8 weather, IRibbonSetMark8 m)
    {
        if (m.RibbonMarkCloudy) return (weather & Overcast) != 0;
        if (m.RibbonMarkRainy) return (weather & Raining) != 0;
        if (m.RibbonMarkStormy) return (weather & Thunderstorm) != 0;
        if (m.RibbonMarkSnowy) return (weather & Snowing) != 0;
        if (m.RibbonMarkBlizzard) return (weather & Snowstorm) != 0;
        if (m.RibbonMarkDry) return (weather & Intense_Sun) != 0;
        if (m.RibbonMarkSandstorm) return (weather & Sandstorm) != 0;
        if (m.RibbonMarkMisty) return (weather & Heavy_Fog) != 0;
        return true; // no mark / etc. is fine; check later.
    }
}
