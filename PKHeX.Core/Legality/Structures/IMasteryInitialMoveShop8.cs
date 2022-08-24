using System;

namespace PKHeX.Core;

/// <summary>
/// Exposes permissions about the Move Shop
/// </summary>
public interface IMasteryInitialMoveShop8
{
    (Learnset Learn, Learnset Mastery) GetLevelUpInfo();
    void LoadInitialMoveset(PA8 pa8, Span<int> moves, Learnset learn, int level);
    bool IsForcedMasteryCorrect(PKM pk);
}

public static class MasteryInitialMoveShop8Extensions
{
    public static void SetInitialMastery(this IMasteryInitialMoveShop8 enc, PKM pk)
    {
        if (pk is PA8 pa8)
        {
            Span<int> moves = stackalloc int[4];
            var level = pa8.Met_Level;
            var (learn, mastery) = enc.GetLevelUpInfo();
            enc.LoadInitialMoveset(pa8, moves, learn, level);
            pa8.SetEncounterMasteryFlags(moves, mastery, level);
        }
    }
}
