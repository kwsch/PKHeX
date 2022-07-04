using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace PKHeX.Core;

public class LearnSource4Pt : ILearnSource, IEggSource
{
    public static readonly LearnSource4Pt Instance = new();
    private static readonly PersonalTable Personal = PersonalTable.Pt;
    private static readonly Learnset[] LevelUp = Legal.LevelUpPt;
    private static readonly EggMoves6[] EggMoves = Legal.EggMovesDPPt;
    private const int MaxSpecies = Legal.MaxSpeciesID_4;
    private const int Generation = 4;
    private const int CountTM = 50;

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

        if (types.HasFlagFast(MoveSourceType.Machine))
        {
            if (GetIsTM(pi, move))
                return LearnMethod.TMHM;
            if (pk.Format == Generation && GetIsHM(pi, move))
                return LearnMethod.TMHM;
        }

        if (types.HasFlagFast(MoveSourceType.TypeTutor) && GetIsTypeTutor(evo.Species, move))
            return LearnMethod.Tutor;

        if (types.HasFlagFast(MoveSourceType.SpecialTutor) && GetIsSpecialTutor(pi, move))
            return LearnMethod.Tutor;

        return LearnMethod.None;
    }

    private static bool GetIsTypeTutor(int species, int move)
    {
        var index = Array.IndexOf(Legal.SpecialTutors_4, move);
        if (index == -1)
            return false;
        var list = Legal.SpecialTutors_Compatibility_4[index].AsSpan();
        return list.IndexOf(species) != -1;
    }

    private static bool GetIsSpecialTutor(PersonalInfo pi, int move)
    {
        var index = Array.IndexOf(Legal.Tutors_4, move);
        if (index == -1)
            return false;
        return pi.TypeTutors[index];
    }

    private static bool GetIsTM(PersonalInfo info, int move)
    {
        var index = Array.IndexOf(Legal.TM_4, move);
        if (index == -1)
            return false;
        return info.TMHM[index];
    }

    private static bool GetIsHM(PersonalInfo info, int move)
    {
        var index = Array.IndexOf(Legal.HM_DPPt, move);
        if (index == -1)
            return false;
        return info.TMHM[CountTM + index];
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
            var moveIDs = Legal.TM_4;
            for (int i = 0; i < moveIDs.Length; i++)
            {
                if (permit[i])
                    yield return moveIDs[i];
            }

            if (pk.Format == Generation)
            {
                moveIDs = Legal.HM_DPPt;
                for (int i = 0; i < moveIDs.Length; i++)
                {
                    if (permit[CountTM + i])
                        yield return moveIDs[i];
                }
            }
        }

        if (types.HasFlagFast(MoveSourceType.SpecialTutor))
        {
            var permit = pi.TypeTutors;
            var moveIDs = Legal.Tutors_4;
            for (int i = 0; i < moveIDs.Length; i++)
            {
                if (permit[i])
                    yield return moveIDs[i];
            }
        }
    }
}
