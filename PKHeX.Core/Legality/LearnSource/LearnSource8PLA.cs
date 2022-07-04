using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace PKHeX.Core;

public class LearnSource8LA : ILearnSource
{
    public static readonly LearnSource8LA Instance = new();
    private static readonly PersonalTable Personal = PersonalTable.LA;
    private static readonly Learnset[] LevelUp = Legal.LevelUpLA;
    private const int MaxSpecies = Legal.MaxSpeciesID_8a;

    public Learnset GetLearnset(int species, int form) => LevelUp[Personal.GetFormIndex(species, form)];

    public bool TryGetPersonal(int species, int form, [NotNullWhen(true)] out PersonalInfo? pi)
    {
        pi = null;
        if ((uint)species > MaxSpecies)
            return false;
        pi = Personal[species, form];
        return true;
    }

    public LearnMethod GetCanLearn(PKM pk, PersonalInfo pi, EvoCriteria evo, int move, MoveSourceType types = MoveSourceType.All)
    {
        if (types.HasFlagFast(MoveSourceType.LevelUp))
        {
            var learn = GetLearnset(evo.Species, evo.Form);
            var level = learn.GetLevelLearnMove(move);
            if (level != -1 && level <= evo.LevelMax)
                return LearnMethod.LevelUp;
        }

        if (types.HasFlagFast(MoveSourceType.Machine) && GetIsMoveShop(pi, move))
            return LearnMethod.TMHM;

        return LearnMethod.None;
    }

    private static bool GetIsMoveShop(PersonalInfo pi, int move)
    {
        var index = Legal.MoveShop8_LA.AsSpan().IndexOf((ushort)move);
        if (index == -1)
            return false;
        return pi.SpecialTutors[0][index];
    }

    public IEnumerable<int> GetAllMoves(PKM pk, EvoCriteria evo, MoveSourceType types = MoveSourceType.All)
    {
        if (!TryGetPersonal(evo.Species, evo.Form, out var pi))
            yield break;

        if (types.HasFlagFast(MoveSourceType.LevelUp))
        {
            var learn = GetLearnset(evo.Species, evo.Form);
            foreach (var move in learn.GetMoves(evo.LevelMin, evo.LevelMax))
                yield return move;
        }

        if (types.HasFlagFast(MoveSourceType.Machine))
        {
            var permit = pi.SpecialTutors[0];
            var moveIDs = Legal.MoveShop8_LA;
            for (int i = 0; i < moveIDs.Length; i++)
            {
                if (permit[i])
                    yield return moveIDs[i];
            }
        }
    }
}
