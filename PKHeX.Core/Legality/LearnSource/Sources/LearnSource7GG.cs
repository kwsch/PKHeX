using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using static PKHeX.Core.LearnMethod;
using static PKHeX.Core.LearnEnvironment;

namespace PKHeX.Core;

/// <summary>
/// Exposes information about how moves are learned in <see cref="USUM"/>.
/// </summary>
public sealed class LearnSource7GG : ILearnSource
{
    public static readonly LearnSource7GG Instance = new();
    private static readonly PersonalTable Personal = PersonalTable.GG;
    private static readonly Learnset[] Learnsets = Legal.LevelUpGG;
    private const int MaxSpecies = Legal.MaxSpeciesID_7b;
    private const LearnEnvironment Game = GG;

    public Learnset GetLearnset(int species, int form) => Learnsets[Personal.GetFormIndex(species, form)];

    public bool TryGetPersonal(int species, int form, [NotNullWhen(true)] out PersonalInfo? pi)
    {
        pi = null;
        if ((uint)species > MaxSpecies)
            return false;
        pi = Personal[species, form];
        return true;
    }

    public MoveLearnInfo GetCanLearn(PKM pk, PersonalInfo pi, EvoCriteria evo, int move, MoveSourceType types = MoveSourceType.All, LearnOption option = LearnOption.Current)
    {
        if (types.HasFlagFast(MoveSourceType.LevelUp))
        {
            var learn = GetLearnset(evo.Species, evo.Form);
            var level = learn.GetLevelLearnMove(move);
            if (level != -1 && level <= evo.LevelMax)
                return new(LevelUp, Game, (byte)level);
        }

        if (types.HasFlagFast(MoveSourceType.Machine) && GetIsTM(pi, move))
            return new(TMHM, Game);

        if (types.HasFlagFast(MoveSourceType.EnhancedTutor) && GetIsEnhancedTutor(evo.Species, evo.Form, move))
            return new(Tutor, Game);

        return default;
    }

    private static bool GetIsEnhancedTutor(int species, int form, int move)
    {
        if (species == (int)Species.Pikachu && form == 8) // Partner
            return Legal.Tutor_StarterPikachu.AsSpan().Contains(move);
        if (species == (int)Species.Eevee && form == 1) // Partner
            return Legal.Tutor_StarterEevee.AsSpan().Contains(move);
        return false;
    }

    private static bool GetIsTM(PersonalInfo info, int move)
    {
        var index = Array.IndexOf(Legal.TMHM_GG, move);
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
            (bool hasMoves, int start, int end) = learn.GetMoveRange(evo.LevelMax);
            if (hasMoves)
            {
                var moves = learn.Moves;
                for (int i = end; i >= start; i--)
                    yield return moves[i];
            }
        }

        if (types.HasFlagFast(MoveSourceType.Machine))
        {
            var permit = pi.TMHM;
            var moveIDs = Legal.TMHM_GG;
            for (int i = 0; i < moveIDs.Length; i++)
            {
                if (permit[i])
                    yield return moveIDs[i];
            }
        }

        if (types.HasFlagFast(MoveSourceType.SpecialTutor))
        {
            if (evo.Species == (int)Species.Pikachu && evo.Form == 8) // Partner
            {
                foreach (var move in Legal.Tutor_StarterPikachu)
                    yield return move;
            }
            else if (evo.Species == (int)Species.Eevee && evo.Form == 1) // Partner
            {
                foreach (var move in Legal.Tutor_StarterEevee)
                    yield return move;
            }
        }
    }
}
