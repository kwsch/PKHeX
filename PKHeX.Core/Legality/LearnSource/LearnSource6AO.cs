using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace PKHeX.Core;

public class LearnSource6AO : ILearnSource, IEggSource
{
    public static readonly LearnSource6AO Instance = new();
    private static readonly PersonalTable Personal = PersonalTable.AO;
    private static readonly Learnset[] LevelUp = Legal.LevelUpAO;
    private static readonly EggMoves6[] EggMoves = Legal.EggMovesAO;
    private const int MaxSpecies = Legal.MaxSpeciesID_6;

    public Learnset GetLearnset(int species, int form) => LevelUp[species];

    public bool TryGetPersonal(int species, int form, [NotNullWhen(true)] out PersonalInfo? pi)
    {
        pi = null;
        if ((uint)species > MaxSpecies)
            return false;
        pi = Personal[species];
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

        if (types.HasFlagFast(MoveSourceType.Machine) && GetIsTM(pi, move))
            return LearnMethod.TMHM;

        if (types.HasFlagFast(MoveSourceType.TypeTutor) && GetIsTypeTutor(pi, move))
            return LearnMethod.Tutor;

        if (types.HasFlagFast(MoveSourceType.SpecialTutor) && GetIsSpecialTutor(pi, move))
            return LearnMethod.Tutor;

        return LearnMethod.None;
    }

    private static bool GetIsTypeTutor(PersonalInfo pi, int move)
    {
        var tutors = Legal.Tutors_AO;
        for (int i = 0; i < tutors.Length; i++)
        {
            var tutor = Array.IndexOf(tutors[i], move);
            if (tutor == -1)
                continue;
            if (pi.SpecialTutors[i][tutor])
                return true;
            break;
        }

        var index = Array.IndexOf(Legal.TypeTutor6, move);
        if (index == -1)
            return false;
        return pi.TypeTutors[index];
    }

    private static bool GetIsSpecialTutor(PersonalInfo pi, int move)
    {
        var tutors = Legal.Tutors_AO;
        for (int i = 0; i < tutors.Length; i++)
        {
            var index = Array.IndexOf(tutors[i], move);
            if (index == -1)
                continue;
            return pi.SpecialTutors[i][index];
        }
        return false;
    }

    private static bool GetIsTM(PersonalInfo info, int move)
    {
        var index = Array.IndexOf(Legal.TMHM_AO, move);
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
            var moveIDs = Legal.TMHM_AO;
            for (int i = 0; i < moveIDs.Length; i++)
            {
                if (permit[i])
                    yield return moveIDs[i];
            }
        }

        if (types.HasFlagFast(MoveSourceType.SpecialTutor))
        {
            // Beams
            var permit = pi.TypeTutors;
            var moveIDs = Legal.TypeTutor6;
            for (int i = 0; i < moveIDs.Length; i++)
            {
                if (permit[i])
                    yield return moveIDs[i];
            }

            // OR/AS Tutors
            var tutors = Legal.Tutors_AO;
            for (int i = 0; i < tutors.Length; i++)
            {
                permit = pi.SpecialTutors[i];
                moveIDs = tutors[i];
                for (int m = 0; m < moveIDs.Length; m++)
                {
                    if (permit[m])
                        yield return moveIDs[m];
                }
            }
        }
    }
}
