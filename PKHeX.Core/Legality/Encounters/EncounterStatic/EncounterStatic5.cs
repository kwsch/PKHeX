namespace PKHeX.Core;

/// <summary>
/// Generation 5 Static Encounter
/// </summary>
/// <inheritdoc cref="EncounterStatic"/>
public record EncounterStatic5(GameVersion Version) : EncounterStatic(Version)
{
    public sealed override int Generation => 5;
    public override EntityContext Context => EntityContext.Gen5;
    public bool Roaming { get; init; }
    public bool IsWildCorrelationPID => !Roaming && Shiny == Shiny.Random && Species != (int)Core.Species.Crustle;

    protected sealed override bool IsMatchPartial(PKM pk)
    {
        // BW/2 Jellicent collision with wild surf slot, resolved by duplicating the encounter with any abil
        if (Ability == AbilityPermission.OnlyHidden && pk.AbilityNumber != 4 && pk.Format <= 7)
            return true;
        return base.IsMatchPartial(pk);
    }

    protected sealed override bool IsMatchLocation(PKM pk)
    {
        if (!Roaming)
            return base.IsMatchLocation(pk);
        return IsRoamerMet(pk.Met_Location);
    }

    protected override bool IsMatchEggLocation(PKM pk)
    {
        if (!EggEncounter)
            return base.IsMatchEggLocation(pk);

        var eggloc = pk.Egg_Location;
        if (!pk.IsEgg) // hatched
            return eggloc == EggLocation || eggloc == Locations.LinkTrade5;

        // Unhatched:
        if (eggloc != EggLocation)
            return false;
        if (pk.Met_Location is not (0 or Locations.LinkTrade5))
            return false;
        return true;
    }

    // 25,26,27,28, // Route 12, 13, 14, 15 Night latter half
    // 15,16,31,    // Route 2, 3, 18 Morning
    // 17,18,29,    // Route 4, 5, 16 Daytime
    // 19,20,21,    // Route 6, 7, 8 Evening
    // 22,23,24,    // Route 9, 10, 11 Night former half
    private static bool IsRoamerMet(int location)
    {
        if ((uint)location >= 32)
            return false;
        return (0b10111111111111111000000000000000 & (1 << location)) != 0;
    }
}
