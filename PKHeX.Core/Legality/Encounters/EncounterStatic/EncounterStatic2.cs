using System;

namespace PKHeX.Core;

/// <summary>
/// Generation 2 Static Encounter
/// </summary>
/// <inheritdoc cref="EncounterStatic"/>
public record EncounterStatic2 : EncounterStatic
{
    public sealed override int Generation => 2;
    public override EntityContext Context => EntityContext.Gen2;
    public sealed override byte Level { get; init; }

    public EncounterStatic2(byte species, byte level, GameVersion game) : base(game)
    {
        Species = species;
        Level = level;
    }

    public override bool IsMatchExact(PKM pk, EvoCriteria evo)
    {
        if (Shiny == Shiny.Always && !pk.IsShiny)
            return false;
        return base.IsMatchExact(pk, evo);
    }

    protected override bool IsMatchEggLocation(PKM pk)
    {
        if (pk.Format > 2)
            return true;

        if (pk.IsEgg)
        {
            if (!EggEncounter)
                return false;
            if (pk.Met_Location != 0 && pk.Met_Level != 0)
                return false;
            if (pk.OT_Friendship > EggCycles) // Dizzy Punch eggs start with below-normal hatch counters.
                return false;
        }
        else
        {
            switch (pk.Met_Level)
            {
                case 0 when pk.Met_Location != 0:
                    return false;
                case 1: // 0 = second floor of every Pokémon Center, valid
                    return true;
                default:
                    if (pk.Met_Location == 0 && pk.Met_Level != 0)
                        return false;
                    break;
            }
        }

        return true;
    }

    protected override bool IsMatchLevel(PKM pk, EvoCriteria evo)
    {
        if (pk is ICaughtData2 {CaughtData: not 0})
            return pk.Met_Level == (EggEncounter ? 1 : Level);

        return Level <= evo.LevelMax;
    }

    protected override bool IsMatchLocation(PKM pk)
    {
        if (EggEncounter)
            return true;
        if (Location == 0)
            return true;
        if (pk is ICaughtData2 {CaughtData: not 0})
            return Location == pk.Met_Location;
        return true;
    }

    protected override bool IsMatchPartial(PKM pk) => false;

    protected override void ApplyDetails(ITrainerInfo tr, EncounterCriteria criteria, PKM pk)
    {
        base.ApplyDetails(tr, criteria, pk);
        var pk2 = (PK2)pk;
        if (Shiny == Shiny.Always)
            pk2.SetShiny();
    }

    protected override void SetMetData(PKM pk, int level, DateTime today)
    {
        if (Version != GameVersion.C && pk.OT_Gender != 1)
            return;
        var pk2 = (PK2)pk;
        pk2.Met_Location = Location;
        pk2.Met_Level = level;
        pk2.Met_TimeOfDay = EncounterTime.Any.RandomValidTime();
    }
}

public sealed record EncounterStatic2Odd : EncounterStatic2
{
    public EncounterStatic2Odd(byte species) : base(species, 5, GameVersion.C)
    {
        EggLocation = 256;
        EggCycles = 20;
    }

    public override bool IsMatchExact(PKM pk, EvoCriteria evo)
    {
        // Let it get picked up as regular EncounterEgg under other conditions.
        if (pk.Format > 2)
            return false;
        if (!pk.HasMove((int)Move.DizzyPunch))
            return false;
        if (pk.IsEgg && pk.EXP != 125)
            return false;
        return base.IsMatchExact(pk, evo);
    }
}

public sealed record EncounterStatic2Roam : EncounterStatic2
{
    // Routes 29-46, except 40 & 41; total 16.
    // 02, 04, 05, 08, 11, 15, 18, 20,
    // 21, 25, 26, 34, 37, 39, 43, 45,
    private const ulong RoamLocations = 0b10_1000_1010_0100_0000_0110_0011_0100_1000_1001_0011_0100;
    public override int Location => 2;

    public EncounterStatic2Roam(byte species, byte level, GameVersion ver) : base(species, level, ver) { }

    protected override bool IsMatchLocation(PKM pk)
    {
        if (!pk.HasOriginalMetLocation)
            return true;
        // Gen2 met location is always u8
        var loc = pk.Met_Location;
        return loc <= 45 && ((RoamLocations & (1UL << loc)) != 0);
    }
}
