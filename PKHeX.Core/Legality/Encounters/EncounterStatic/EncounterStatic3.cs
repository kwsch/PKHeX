using System;

namespace PKHeX.Core;

/// <summary>
/// Generation 3 Static Encounter
/// </summary>
/// <inheritdoc cref="EncounterStatic"/>
public sealed record EncounterStatic3 : EncounterStatic
{
    public override int Generation => 3;
    public override EntityContext Context => EntityContext.Gen3;
    public bool Roaming { get; init; }

    public EncounterStatic3(ushort species, byte level, GameVersion game) : base(game)
    {
        Species = species;
        Level = level;
    }

    protected override bool IsMatchEggLocation(PKM pk)
    {
        if (pk.Format == 3)
            return !pk.IsEgg || EggLocation == 0 || EggLocation == pk.Met_Location;
        return base.IsMatchEggLocation(pk);
    }

    protected override bool IsMatchLevel(PKM pk, EvoCriteria evo)
    {
        if (pk.Format != 3) // Met Level lost on PK3=>PK4
            return Level <= evo.LevelMax;

        if (EggEncounter)
            return pk.Met_Level == 0 && pk.CurrentLevel >= 5; // met level 0, origin level 5

        return pk.Met_Level == Level;
    }

    protected override bool IsMatchLocation(PKM pk)
    {
        if (EggEncounter)
            return true;
        if (pk.Format != 3)
            return true; // transfer location verified later

        var met = pk.Met_Location;
        if (!Roaming)
            return Location == met;

        // Route 101-138
        if (Version <= GameVersion.E)
            return met is >= 16 and <= 49;
        // Route 1-25 encounter is possible either in grass or on water
        return met is >= 101 and <= 125;
    }

    protected override bool IsMatchPartial(PKM pk)
    {
        if (Gift && pk.Ball != Ball)
            return true;
        return base.IsMatchPartial(pk);
    }

    protected override void SetMetData(PKM pk, int level, DateTime today)
    {
        pk.Met_Level = level;
        pk.Met_Location = !Roaming ? Location : (Version <= GameVersion.E ? 16 : 101);
    }
}
