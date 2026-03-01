using System;

namespace PKHeX.Core;

/// <summary>
/// Exposes permissions about the Move Shop
/// </summary>
public interface IMasteryInitialMoveShop8
{
    (Learnset Learn, Learnset Mastery) GetLevelUpInfo();
    void LoadInitialMoveset(PA8 pa8, Span<ushort> moves, Learnset learn, byte level);
    bool IsForcedMasteryCorrect(PKM pk);
    void SetInitialMastery<T>(PKM pk, T enc) where T : ISpeciesForm
    {
        if (pk is PA8 pa8)
            SetInitialMastery(pa8, enc);
    }

    void SetInitialMastery<T>(PA8 pk, T enc) where T : ISpeciesForm
    {
        Span<ushort> moves = stackalloc ushort[4];
        var metLevel = pk.MetLevel;
        var (learn, mastery) = LearnSource8LA.GetLearnsetAndMastery(enc.Species, enc.Form);
        LoadInitialMoveset(pk, moves, learn, metLevel);
        pk.SetEncounterMasteryFlags(moves, mastery, metLevel, pk.AlphaMove);
    }
}
