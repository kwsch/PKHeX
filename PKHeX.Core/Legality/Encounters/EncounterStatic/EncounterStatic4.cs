using System;
using static PKHeX.Core.GroundTileAllowed;

namespace PKHeX.Core;

/// <summary>
/// Generation 4 Static Encounter
/// </summary>
/// <inheritdoc cref="EncounterStatic"/>
public sealed record EncounterStatic4(GameVersion Version) : EncounterStatic(Version), IGroundTypeTile
{
    public override int Generation => 4;
    public override EntityContext Context => EntityContext.Gen4;

    /// <summary> Indicates if the encounter is a Roamer (variable met location) </summary>
    public bool Roaming { get; init; }

    /// <summary> <see cref="PK4.GroundTile"/> values permitted for the encounter. </summary>
    public GroundTileAllowed GroundTile { get; init; } = None;

    protected override bool IsMatchLocation(PKM pk)
    {
        if (!Roaming)
            return base.IsMatchLocation(pk);

        // Met location is lost on transfer
        if (pk is not G4PKM pk4)
            return true;

        return pk4.GroundTile switch
        {
            GroundTileType.Grass => IsMatchLocationGrass(Location, pk4.Met_Location),
            GroundTileType.Water => IsMatchLocationWater(Location, pk4.Met_Location),
            _ => false,
        };
    }

    private static bool IsMatchLocationGrass(int location, int met) => location switch
    {
        FirstS => IsMatchRoamerLocation(PermitGrassS, met, FirstS),
        FirstJ => IsMatchRoamerLocation(PermitGrassJ, met, FirstJ),
        FirstH => IsMatchRoamerLocation(PermitGrassH, met, FirstH),
        _ => false,
    };

    private static bool IsMatchLocationWater(int location, int met) => location switch
    {
        FirstS => IsMatchRoamerLocation(PermitWaterS, met, FirstS),
        FirstJ => IsMatchRoamerLocation(PermitWaterJ, met, FirstJ),
        FirstH => IsMatchRoamerLocation(PermitWaterH, met, FirstH),
        _ => false,
    };

    protected override bool IsMatchEggLocation(PKM pk)
    {
        if (!EggEncounter)
            return base.IsMatchEggLocation(pk);

        var eggloc = pk.Egg_Location;
        // Transferring 4->5 clears Pt/HG/SS location value and keeps Faraway Place
        if (pk is not G4PKM pk4)
        {
            if (eggloc == Locations.LinkTrade4)
                return true;
            var cmp = Locations.IsPtHGSSLocationEgg(EggLocation) ? Locations.Faraway4 : EggLocation;
            return eggloc == cmp;
        }

        if (!pk4.IsEgg) // hatched
            return eggloc == EggLocation || eggloc == Locations.LinkTrade4;

        // Unhatched:
        if (eggloc != EggLocation)
            return false;
        if (pk4.Met_Location is not (0 or Locations.LinkTrade4))
            return false;
        return true;
    }

    protected override void ApplyDetails(ITrainerInfo tr, EncounterCriteria criteria, PKM pk)
    {
        base.ApplyDetails(tr, criteria, pk);
        SanityCheckVersion(pk);
    }

    private void SanityCheckVersion(PKM pk)
    {
        // Unavailable encounters in DP, morph them to Pt so they're legal.
        switch (Species)
        {
            case (int)Core.Species.Darkrai when Location == 079: // DP Darkrai
            case (int)Core.Species.Shaymin when Location == 063: // DP Shaymin
                pk.Version = (int)GameVersion.Pt;
                return;
        }
    }

    protected override bool IsMatchLevel(PKM pk, EvoCriteria evo)
    {
        if (pk.Format != 4) // Met Level lost on PK4=>PK5
            return Level <= evo.LevelMax;

        return pk.Met_Level == (EggEncounter ? 0 : Level);
    }

    protected override bool IsMatchPartial(PKM pk)
    {
        if (Gift && pk.Ball != Ball)
            return true;
        return base.IsMatchPartial(pk);
    }

    protected override void SetMetData(PKM pk, int level, DateTime today)
    {
        var pk4 = (PK4)pk;
        pk4.GroundTile = Roaming ? GroundTileType.Grass : GroundTile.GetIndex();
        pk.Met_Location = Location;
        pk.Met_Level = level;
        pk.MetDate = today;
    }

    public static bool IsMatchRoamerLocation(uint permit, int location, int first)
    {
        var value = location - first;
        if ((uint)value >= 64)
            return false;
        return (permit & (1u << value)) != 0;
    }

    // Merged all locations into a bitmask for quick computation.
    private const int FirstS = 16;
    private const uint PermitGrassS = 0x8033FFFF; // 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33,         36, 37, 47, 49,
    private const uint PermitWaterS = 0x803E3B9E; //         18, 19, 20,         23, 24, 25,     27, 28, 29,             33, 34, 35, 36, 37, 47, 49,

    private const int FirstJ = 177;
    private const uint PermitGrassJ = 0x0003E7FF; // 177, 178, 179, 180, 181, 182, 183, 184, 185, 186, 187,                     190, 191, 192, 193, 194,
    private const uint PermitWaterJ = 0x0001E06E; //      178, 179, 180,      182, 183,                                         190, 191, 192, 193,

    private const int FirstH = 149;
    private const uint PermitGrassH = 0x0AB3FFFF; // 149, 150, 151, 152, 153, 154, 155, 156, 157, 158, 159, 160, 161, 162, 163, 164, 165, 166,           169, 170, 172,      174,      176, 
    private const uint PermitWaterH = 0x0ABC1B28; //                152,      154,           157, 158,      160, 161,                          167, 168, 169, 170, 172,      174,      176,
}
