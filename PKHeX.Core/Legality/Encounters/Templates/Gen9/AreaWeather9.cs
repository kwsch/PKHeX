using System;
using static PKHeX.Core.RibbonIndex;
using static PKHeX.Core.AreaWeather9;

namespace PKHeX.Core;

/// <summary>
/// Encounter Conditions for <see cref="GameVersion.SV"/>
/// </summary>
[Flags]
public enum AreaWeather9 : ushort
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

    Standard = Normal | Overcast | Raining | Thunderstorm,
    Sand = Normal | Overcast | Raining | Sandstorm,
    Snow = Normal | Overcast | Snowing | Snowstorm,
    Inside = Normal | Overcast,
}

/// <summary>
/// Extension methods for <see cref="AreaWeather9"/>
/// </summary>
public static class AreaWeather9Extensions
{
    public static bool IsMarkCompatible(this AreaWeather9 weather, RibbonIndex m) => m switch
    {
        MarkCloudy => (weather & Overcast) != 0,
        MarkRainy => (weather & Raining) != 0,
        MarkStormy => (weather & Thunderstorm) != 0,
        MarkSnowy => (weather & Snowing) != 0,
        MarkBlizzard => (weather & Snowstorm) != 0,
        MarkSandstorm => (weather & Sandstorm) != 0,
        _ => false,
    };
}
