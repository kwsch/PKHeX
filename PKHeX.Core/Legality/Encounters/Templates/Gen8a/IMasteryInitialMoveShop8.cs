using System;

namespace PKHeX.Core;

/// <summary>
/// Exposes permissions about the Move Shop
/// </summary>
public interface IMasteryInitialMoveShop8
{
    (Learnset Learn, Learnset Mastery) GetLevelUpInfo();
    void LoadInitialMoveset(PA8 pa8, Span<ushort> moves, Learnset learn, int level);
    bool IsForcedMasteryCorrect(PKM pk);
    void SetInitialMastery(PKM pk)
    {
        if (pk is PA8 pa8)
            SetInitialMastery(pa8);
    }

    void SetInitialMastery(PA8 pk)
    {
        Span<ushort> moves = stackalloc ushort[4];
        var level = pk.MetLevel;
        var (learn, mastery) = LearnSource8LA.GetLearnsetAndMastery(pk.Species, pk.Form);
        LoadInitialMoveset(pk, moves, learn, level);
        pk.SetEncounterMasteryFlags(moves, mastery, level);
    }
}
