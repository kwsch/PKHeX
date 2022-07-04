using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace PKHeX.Core;

public class LearnSource8BDSP : ILearnSource, IEggSource
{
    public static readonly LearnSource8BDSP Instance = new();
    private static readonly PersonalTable Personal = PersonalTable.BDSP;
    private static readonly Learnset[] LevelUp = Legal.LevelUpBDSP;
    private static readonly EggMoves6[] EggMoves = Legal.EggMovesBDSP;
    private const int MaxSpecies = Legal.MaxSpeciesID_8b;

    public Learnset GetLearnset(int species, int form) => LevelUp[Personal.GetFormIndex(species, form)];

    public bool TryGetPersonal(int species, int form, [NotNullWhen(true)] out PersonalInfo? pi)
    {
        pi = null;
        if ((uint)species > MaxSpecies)
            return false;
        pi = Personal[species, form];
        return true;
    }

    public bool GetIsEggMove(int species, int form, int move)
    {
        if ((uint)species > MaxSpecies)
            return false;
        var moves = EggMoves[species];
        return moves.GetHasEggMove(move);
    }

    public ReadOnlySpan<int> GetEggMoves(int species, int form)
    {
        if ((uint)species > MaxSpecies)
            return ReadOnlySpan<int>.Empty;
        return EggMoves[species].Moves;
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

        if (types.HasFlagFast(MoveSourceType.SharedEggMove) && GetIsSharedEggMove(pi, move))
            return LearnMethod.Shared;

        if (types.HasFlagFast(MoveSourceType.Machine) && GetIsTM(pi, move))
            return LearnMethod.TMHM;

        if (types.HasFlagFast(MoveSourceType.TypeTutor) && GetIsTypeTutor(pi, move))
            return LearnMethod.Tutor;

        return LearnMethod.None;
    }

    private bool GetIsSharedEggMove(PersonalInfo pi, int move)
    {
        var entry = (PersonalInfoBDSP)pi;
        var baseSpecies = entry.HatchSpecies;
        var baseForm = entry.HatchFormIndex;
        return GetEggMoves(baseSpecies, baseForm).IndexOf(move) != -1;
    }

    private static bool GetIsTypeTutor(PersonalInfo pi, int move)
    {
        var index = Array.IndexOf(Legal.TypeTutor8b, move);
        if (index == -1)
            return false;
        return pi.TypeTutors[index];
    }

    private static bool GetIsTM(PersonalInfo info, int move)
    {
        var index = Array.IndexOf(Legal.TMHM_BDSP, move);
        if (index == -1)
            return false;
        return info.TMHM[index];
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
            var permit = pi.TMHM;
            var moveIDs = Legal.TMHM_BDSP;
            for (int i = 0; i < moveIDs.Length; i++)
            {
                if (permit[i])
                    yield return moveIDs[i];
            }
        }

        if (types.HasFlagFast(MoveSourceType.TypeTutor))
        {
            var permit = pi.TypeTutors;
            var moveIDs = Legal.TypeTutor8b;
            for (int i = 0; i < moveIDs.Length; i++)
            {
                if (permit[i])
                    yield return moveIDs[i];
            }
        }
    }
}
