namespace PKHeX.Core;

/// <summary>
/// Generation 6 Static Encounter
/// </summary>
/// <inheritdoc cref="EncounterStatic"/>
public sealed record EncounterStatic6(GameVersion Version) : EncounterStatic(Version), IContestStats
{
    public override int Generation => 6;
    public override EntityContext Context => EntityContext.Gen6;

    public byte CNT_Cool   { get; init; }
    public byte CNT_Beauty { get; init; }
    public byte CNT_Cute   { get; init; }
    public byte CNT_Smart  { get; init; }
    public byte CNT_Tough  { get; init; }
    public byte CNT_Sheen  { get; init; }

    protected override bool IsMatchLocation(PKM pk)
    {
        if (base.IsMatchLocation(pk))
            return true;

        if (Species != (int) Core.Species.Pikachu)
            return false;

        // Cosplay Pikachu is given from multiple locations
        var loc = pk.Met_Location;
        return loc is 180 or 186 or 194;
    }

    protected override bool IsMatchEggLocation(PKM pk)
    {
        if (!EggEncounter)
            return base.IsMatchEggLocation(pk);

        var eggloc = pk.Egg_Location;
        if (!pk.IsEgg) // hatched
            return eggloc == EggLocation || eggloc == Locations.LinkTrade6;

        // Unhatched:
        if (eggloc != EggLocation)
            return false;
        if (pk.Met_Location is not (0 or Locations.LinkTrade6))
            return false;
        return true;
    }

    protected override void ApplyDetails(ITrainerInfo tr, EncounterCriteria criteria, PKM pk)
    {
        base.ApplyDetails(tr, criteria, pk);
        var pk6 = (PK6)pk;
        this.CopyContestStatsTo(pk6);
        pk6.SetRandomMemory6();
        pk6.SetRandomEC();
    }
}
