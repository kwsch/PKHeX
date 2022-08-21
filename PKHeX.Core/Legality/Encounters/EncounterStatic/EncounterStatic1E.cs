using System;
using System.Collections.Generic;
using System.Linq;

namespace PKHeX.Core;

/// <summary>
/// Event data for Generation 1
/// </summary>
/// <inheritdoc cref="EncounterStatic1"/>
public sealed record EncounterStatic1E : EncounterStatic1, IFixedGBLanguage
{
    public EncounterGBLanguage Language { get; init; } = EncounterGBLanguage.Japanese;

    /// <summary> Trainer name for the event. </summary>
    public string OT_Name { get; init; } = string.Empty;

    public IReadOnlyList<string> OT_Names { get; init; } = Array.Empty<string>();

    /// <summary> Trainer ID for the event. </summary>
    public int TID { get; init; } = -1;

    public EncounterStatic1E(byte species, byte level, GameVersion game) : base(species, level, game)
    {
    }

    public override bool IsMatchExact(PKM pk, EvoCriteria evo)
    {
        if (!base.IsMatchExact(pk, evo))
            return false;

        if (Language != EncounterGBLanguage.Any && pk.Japanese != (Language == EncounterGBLanguage.Japanese))
            return false;

        // EC/PID check doesn't exist for these, so check Shiny state here.
        if (!IsShinyValid(pk))
            return false;

        if (TID != -1 && pk.TID != TID)
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

    protected override PKM GetBlank(ITrainerInfo tr) => Language switch
    {
        EncounterGBLanguage.Japanese => new PK1(true),
        EncounterGBLanguage.International => new PK1(),
        _ => new PK1(tr.Language == 1),
    };

    protected override void ApplyDetails(ITrainerInfo tr, EncounterCriteria criteria, PKM pk)
    {
        base.ApplyDetails(tr, criteria, pk);

        if (Version == GameVersion.Stadium)
        {
            var pk1 = (PK1)pk;
            // Amnesia Psyduck has different catch rates depending on language
            if (Species == (int)Core.Species.Psyduck)
                pk1.Catch_Rate = pk1.Japanese ? (byte)167 : (byte)168;
            else
                pk1.Catch_Rate = Util.Rand.Next(2) == 0 ? (byte)167 : (byte)168;
        }

        if (TID != -1)
            pk.TID = TID;

        if (OT_Name.Length != 0)
            pk.OT_Name = OT_Name;
        else if (OT_Names.Count != 0)
            pk.OT_Name = OT_Names[Util.Rand.Next(OT_Names.Count)];
    }
}

/// <summary>
/// Exposes info on language restriction for Gen1/2.
/// </summary>
public interface IFixedGBLanguage
{
    /// <summary>
    /// Language restriction for the encounter template.
    /// </summary>
    EncounterGBLanguage Language { get; }
}

/// <summary>
/// Generations 1 &amp; 2 cannot communicate between Japanese &amp; International versions.
/// </summary>
public enum EncounterGBLanguage
{
    /// <summary> Can only be obtained in Japanese games. </summary>
    Japanese,

    /// <summary> Can only be obtained in International (not Japanese) games. </summary>
    International,

    /// <summary> Can be obtained in any localization. </summary>
    Any,
}
