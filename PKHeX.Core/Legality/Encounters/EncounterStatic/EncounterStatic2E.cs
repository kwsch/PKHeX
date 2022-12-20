using System;
using System.Collections.Generic;
using System.Linq;

namespace PKHeX.Core;

/// <summary>
/// Event data for Generation 2
/// </summary>
/// <inheritdoc cref="EncounterStatic2"/>
public sealed record EncounterStatic2E : EncounterStatic2, IFixedGBLanguage
{
    public EncounterGBLanguage Language { get; init; } = EncounterGBLanguage.Japanese;

    /// <summary> Trainer name for the event. </summary>
    public string OT_Name { get; init; } = string.Empty;

    public IReadOnlyList<string> OT_Names { get; init; } = Array.Empty<string>();

    private const ushort UnspecifiedID = 0;

    /// <summary> Trainer ID for the event. </summary>
    public ushort TID16 { get; init; } = UnspecifiedID;

    public bool IsGift => TID16 != UnspecifiedID;

    public int CurrentLevel { get; init; } = -1;

    public EncounterStatic2E(byte species, byte level, GameVersion ver) : base(species, level, ver)
    {
    }

    public override bool IsMatchExact(PKM pk, EvoCriteria evo)
    {
        if (!base.IsMatchExact(pk, evo))
            return false;

        if (Language != EncounterGBLanguage.Any && pk.Japanese != (Language == EncounterGBLanguage.Japanese))
            return false;

        if (CurrentLevel != -1 && CurrentLevel > pk.CurrentLevel)
            return false;

        // EC/PID check doesn't exist for these, so check Shiny state here.
        if (!IsShinyValid(pk))
            return false;

        if (EggEncounter && !pk.IsEgg)
            return true;

        // Check OT Details
        if (TID16 != UnspecifiedID && pk.TID16 != TID16)
            return false;

        if (OT_Name.Length != 0)
        {
            if (pk.OT_Name != OT_Name)
                return false;
        }
        else if (OT_Names.Count != 0)
        {
            if (!OT_Names.Contains(pk.OT_Name))
                return false;
        }

        return true;
    }

    private bool IsShinyValid(PKM pk) => Shiny switch
    {
        Shiny.Never => !pk.IsShiny,
        Shiny.Always => pk.IsShiny,
        _ => true,
    };

    protected override int GetMinimalLevel() => CurrentLevel == -1 ? base.GetMinimalLevel() : CurrentLevel;

    protected override PK2 GetBlank(ITrainerInfo tr) => Language switch
    {
        EncounterGBLanguage.Japanese => new(true),
        EncounterGBLanguage.International => new(),
        _ => new(tr.Language == 1),
    };

    protected override void ApplyDetails(ITrainerInfo tr, EncounterCriteria criteria, PKM pk)
    {
        base.ApplyDetails(tr, criteria, pk);
        if (CurrentLevel != -1) // Restore met level
            pk.Met_Level = LevelMin;

        if (TID16 != UnspecifiedID)
            pk.TID16 = TID16;
        if (IsGift)
            pk.OT_Gender = 0;

        if (OT_Name.Length != 0)
            pk.OT_Name = OT_Name;
        else if (OT_Names.Count != 0)
            pk.OT_Name = OT_Names[Util.Rand.Next(OT_Names.Count)];
    }
}
