using static PKHeX.Core.Encounters8Nest;

namespace PKHeX.Core;

/// <summary>
/// Generation 8 Nest Encounter (Distributed Crystal Data)
/// </summary>
/// <inheritdoc cref="EncounterStatic8Nest{T}"/>
public sealed record EncounterStatic8NC(GameVersion Version) : EncounterStatic8Nest<EncounterStatic8NC>(Version)
{
    protected override bool IsMatchLocation(PKM pk)
    {
        var loc = pk.Met_Location;
        return loc is SharedNest or Watchtower;
    }

    protected override bool IsMatchLevel(PKM pk, EvoCriteria evo)
    {
        var lvl = pk.Met_Level;
        if (lvl == Level)
            return true;

        // Check downleveled (20-55)
        if (lvl > Level)
            return false;
        if (lvl is < 20 or > 55)
            return false;
        if (pk is { Met_Location: Watchtower, IsShiny: true })
            return false; // host cannot downlevel and be shiny
        return lvl % 5 == 0;
    }
}
