using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using static PKHeX.Core.LearnMethod;
using static PKHeX.Core.GameVersion;

namespace PKHeX.Core;

public class LearnSource4HGSS : ILearnSource, IEggSource
{
    public static readonly LearnSource4HGSS Instance = new();
    private static readonly PersonalTable Personal = PersonalTable.HGSS;
    private static readonly Learnset[] Learnsets = Legal.LevelUpHGSS;
    private static readonly EggMoves6[] EggMoves = Legal.EggMovesHGSS;
    private const int MaxSpecies = Legal.MaxSpeciesID_4;
    private const GameVersion Game = HGSS;
    private const int Generation = 4;
    private const int CountTM = 92;

    public Learnset GetLearnset(int species, int form) => Learnsets[species];

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

    public MoveLearnInfo GetCanLearn(PKM pk, PersonalInfo pi, EvoCriteria evo, int move, MoveSourceType types = MoveSourceType.All)
    {
        if (types.HasFlagFast(MoveSourceType.LevelUp))
        {
            var learn = GetLearnset(evo.Species, evo.Form);
            var level = learn.GetLevelLearnMove(move);
            if (level != -1 && level <= evo.LevelMax)
                return new(LevelUp, Game, (byte)level);
        }

        if (types.HasFlagFast(MoveSourceType.Machine))
        {
            if (GetIsTM(pi, move))
                return new(TMHM, Game);
            if (pk.Format == Generation && GetIsHM(pi, move))
                return new(TMHM, Game);
        }

        if (types.HasFlagFast(MoveSourceType.TypeTutor) && GetIsTypeTutor(evo.Species, move))
            return new(Tutor, Game);

        if (types.HasFlagFast(MoveSourceType.SpecialTutor) && GetIsSpecialTutor(pi, move))
            return new(Tutor, Game);

        return default;
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
        var index = Array.IndexOf(Legal.HM_HGSS, move);
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
                moveIDs = Legal.HM_HGSS;
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

            // Elemental Beams
            var arr = Legal.SpecialTutors_Compatibility_4;
            moveIDs = Legal.SpecialTutors_4;
            for (int i = 0; i < arr.Length; i++)
            {
                var index = Array.IndexOf(arr[i], evo.Species);
                if (index != -1)
                    yield return moveIDs[i];
            }
        }
    }
}
