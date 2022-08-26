using System;
using static PKHeX.Core.StaticCorrelation8bRequirement;

namespace PKHeX.Core;

/// <summary>
/// Generation 7 Static Encounter
/// </summary>
/// <inheritdoc cref="EncounterStatic"/>
public sealed record EncounterStatic8b : EncounterStatic, IStaticCorrelation8b
{
    public override int Generation => 8;
    public override EntityContext Context => EntityContext.Gen8b;

    public bool Roaming { get; init; }
    public override bool EggEncounter => EggLocation != Locations.Default8bNone;

    public EncounterStatic8b(GameVersion game) : base(game) => EggLocation = Locations.Default8bNone;

    protected override bool IsMatchLocation(PKM pk)
    {
        if (pk is PK8)
            return Locations.IsValidMetBDSP(pk.Met_Location, pk.Version);
        if (!Roaming)
            return base.IsMatchLocation(pk);
        return IsRoamingLocation(pk);
    }

    private static bool IsRoamingLocation(PKM pk)
    {
        var location = pk.Met_Location;
        foreach (var value in Roaming_MetLocation_BDSP)
        {
            if (value == location)
                return true;
        }
        return false;
    }

    public StaticCorrelation8bRequirement GetRequirement(PKM pk) => Roaming
        ? MustHave
        : MustNotHave;

    public bool IsStaticCorrelationCorrect(PKM pk)
    {
        return Roaming8bRNG.ValidateRoamingEncounter(pk, Shiny == Shiny.Random ? Shiny.FixedValue : Shiny, FlawlessIVCount);
    }

    protected override bool IsMatchEggLocation(PKM pk)
    {
        if (pk is not PB8)
        {
            if (!EggEncounter)
                return pk.Egg_Location == 0;

            if (pk is PK8)
            {
                if (EggLocation > 60000 && pk.Egg_Location == Locations.HOME_SWSHBDSPEgg)
                    return true;
                // >60000 can be reset to Link Trade (30001), then altered differently.
                return Locations.IsValidMetBDSP(pk.Egg_Location, pk.Version) && pk.Egg_Location == pk.Met_Location;
            }

            // Hatched
            return pk.Egg_Location == EggLocation || pk.Egg_Location == Locations.LinkTrade6NPC;
        }

        var eggloc = pk.Egg_Location;
        if (!EggEncounter)
            return eggloc == EggLocation;

        if (!pk.IsEgg) // hatched
            return eggloc == EggLocation || eggloc == Locations.LinkTrade6NPC;

        // Unhatched:
        if (eggloc != EggLocation)
            return false;
        if (pk.Met_Location is not (Locations.Default8bNone or Locations.LinkTrade6NPC))
            return false;
        return true;
    }

    protected override void ApplyDetails(ITrainerInfo tr, EncounterCriteria criteria, PKM pk)
    {
        pk.Met_Location = pk.Egg_Location = Locations.Default8bNone;
        base.ApplyDetails(tr, criteria, pk);
        var req = GetRequirement(pk);
        if (req == MustHave) // Roamers
        {
            var shiny = Shiny == Shiny.Random ? Shiny.FixedValue : Shiny;
            Roaming8bRNG.ApplyDetails(pk, criteria, shiny, FlawlessIVCount);
        }
        else
        {
            var shiny = Shiny == Shiny.Never ? Shiny.Never : Shiny.Random;
            Wild8bRNG.ApplyDetails(pk, criteria, shiny, FlawlessIVCount, Ability);
        }
    }

    protected override void SetMetData(PKM pk, int level, DateTime today)
    {
        pk.Met_Level = level;
        pk.Met_Location = !Roaming ? Location : Roaming_MetLocation_BDSP[0];
        pk.MetDate = today;
    }

    // defined by mvpoke in encounter data
    private static readonly ushort[] Roaming_MetLocation_BDSP =
    {
        197, 201, 354, 355, 356, 357, 358, 359, 361, 362, 364, 365, 367, 373, 375, 377,
        378, 379, 383, 385, 392, 394, 395, 397, 400, 403, 404, 407,
        485,
    };
}

public interface IStaticCorrelation8b
{
    StaticCorrelation8bRequirement GetRequirement(PKM pk);
    bool IsStaticCorrelationCorrect(PKM pk);
}

public enum StaticCorrelation8bRequirement
{
    CanBeEither,
    MustHave,
    MustNotHave,
}
