namespace PKHeX.Core;

/// <summary>
/// Time of Day enumeration used for <see cref="GameVersion.PLA"/> Pokédex entries.
/// </summary>
public enum PokedexTimeOfDay8a : byte
{
    Morning = 0,
    Midday = 1,
    Evening = 2,
    Night = 3,
    DaylightHours = 4,
    Invalid = 5,
}
