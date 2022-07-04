using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using static PKHeX.Core.LearnMethod;
using static PKHeX.Core.GameVersion;

namespace PKHeX.Core;

public class LearnSource7USUM : ILearnSource, IEggSource
{
    public static readonly LearnSource7USUM Instance = new();
    private static readonly PersonalTable Personal = PersonalTable.USUM;
    private static readonly Learnset[] Learnsets = Legal.LevelUpUSUM;
    private static readonly EggMoves7[] EggMoves = Legal.EggMovesUSUM;
    private const int MaxSpecies = Legal.MaxSpeciesID_7_USUM;
    private const GameVersion Game = USUM;

    public Learnset GetLearnset(int species, int form) => Learnsets[Personal.GetFormIndex(species, form)];

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
        var moves = MoveEgg.GetFormEggMoves(species, form, EggMoves).AsSpan();
        return moves.IndexOf(move) != -1;
    }

    public ReadOnlySpan<int> GetEggMoves(int species, int form)
    {
        if ((uint)species > MaxSpecies)
            return ReadOnlySpan<int>.Empty;
        return MoveEgg.GetFormEggMoves(species, form, EggMoves).AsSpan();
    }

    public MoveLearnInfo GetCanLearn(PKM pk, PersonalInfo pi, EvoCriteria evo, int move, MoveSourceType types = MoveSourceType.All)
    {
        if (types.HasFlagFast(MoveSourceType.LevelUp))
        {
            var learn = GetLearnset(evo.Species, evo.Form);
            var level = learn.GetLevelLearnMove(move);
            if (level != -1) // Can relearn at any level!
                return new(LevelUp, Game, (byte)level);
        }

        if (types.HasFlagFast(MoveSourceType.Machine) && GetIsTM(pi, move))
            return new(TMHM, Game);

        if (types.HasFlagFast(MoveSourceType.TypeTutor) && GetIsTypeTutor(pi, move))
            return new(Tutor, Game);

        if (types.HasFlagFast(MoveSourceType.SpecialTutor) && GetIsSpecialTutor(pi, move))
            return new(Tutor, Game);

        return default;
    }

    private static bool GetIsTypeTutor(PersonalInfo pi, int move)
    {
        var index = Array.IndexOf(Legal.TypeTutor6, move);
        if (index == -1)
            return false;
        return pi.TypeTutors[index];
    }

    private static bool GetIsSpecialTutor(PersonalInfo pi, int move)
    {
        // US/UM Tutors
        var tutors = Legal.Tutors_USUM;
        var tutor = Array.IndexOf(tutors, move);
        if (tutor == -1)
            return false;
        return pi.SpecialTutors[0][tutor];
    }

    private static bool GetIsTM(PersonalInfo info, int move)
    {
        var index = Array.IndexOf(Legal.TMHM_SM, move);
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
            foreach (var move in learn.GetMoves(evo.LevelMin, 100))
                yield return move;
        }

        if (types.HasFlagFast(MoveSourceType.Machine))
        {
            var permit = pi.TMHM;
            var moveIDs = Legal.TMHM_SM;
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

            // US/UM Tutors
            permit = pi.SpecialTutors[0];
            moveIDs = Legal.Tutors_USUM;
            for (int i = 0; i < permit.Length; i++)
            {
                if (permit[i])
                    yield return moveIDs[i];
            }
        }
    }
}
