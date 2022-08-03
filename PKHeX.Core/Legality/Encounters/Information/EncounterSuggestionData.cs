using System;
using System.Collections.Generic;

namespace PKHeX.Core;

public sealed class EncounterSuggestionData : ISpeciesForm, IRelearn
{
    private readonly IEncounterable? Encounter;

    public IReadOnlyList<int> Relearn => Encounter is IRelearn { Relearn: int[] { Length: not 0 } r } ? r : Array.Empty<int>();

    public EncounterSuggestionData(PKM pk, IEncounterable enc, int met)
    {
        Encounter = enc;
        Species = pk.Species;
        Form = pk.Form;
        Location = met;

        LevelMin = enc.LevelMin;
        LevelMax = enc.LevelMax;
    }

    public EncounterSuggestionData(PKM pk, int met, byte lvl)
    {
        Species = pk.Species;
        Form = pk.Form;
        Location = met;

        LevelMin = lvl;
        LevelMax = lvl;
    }

    public int Species { get; }
    public int Form { get; }
    public int Location { get; }

    public byte LevelMin { get; }
    public byte LevelMax { get; }

    public int GetSuggestedMetLevel(PKM pk) => EncounterSuggestion.GetSuggestedMetLevel(pk, LevelMin);
    public GroundTileType GetSuggestedGroundTile() => Encounter is IGroundTypeTile t ? t.GroundTile.GetIndex() : 0;
    public bool HasGroundTile(int format) => Encounter is IGroundTypeTile t && t.HasGroundTile(format);
}
