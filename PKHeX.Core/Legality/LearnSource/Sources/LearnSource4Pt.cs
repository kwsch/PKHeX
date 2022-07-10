using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using static PKHeX.Core.LearnMethod;
using static PKHeX.Core.LearnEnvironment;

namespace PKHeX.Core;

/// <summary>
/// Exposes information about how moves are learned in <see cref="Pt"/>.
/// </summary>
public sealed class LearnSource4Pt : ILearnSource, IEggSource
{
    public static readonly LearnSource4Pt Instance = new();
    private static readonly PersonalTable Personal = PersonalTable.Pt;
    private static readonly Learnset[] Learnsets = Legal.LevelUpPt;
    private static readonly EggMoves6[] EggMoves = Legal.EggMovesDPPt;
    private const int MaxSpecies = Legal.MaxSpeciesID_4;
    private const LearnEnvironment Game = Pt;
    private const int Generation = 4;
    private const int CountTM = 50;

    public Learnset GetLearnset(int species, int form) => Learnsets[Personal.GetFormIndex(species, form)];

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

    public MoveLearnInfo GetCanLearn(PKM pk, PersonalInfo pi, EvoCriteria evo, int move, MoveSourceType types = MoveSourceType.All, LearnOption option = LearnOption.Current)
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
            if ((move is (int)Move.Defog || pk.Format == Generation) && GetIsHM(pi, move))
                return new(TMHM, Game);
        }

        if (types.HasFlagFast(MoveSourceType.TypeTutor) && GetIsTypeTutor(evo.Species, move))
            return new(Tutor, Game);

        if (types.HasFlagFast(MoveSourceType.SpecialTutor) && GetIsSpecialTutor(pi, move))
            return new(Tutor, Game);

        if (types.HasFlagFast(MoveSourceType.EnhancedTutor) && GetIsEnhancedTutor(evo, pk, move, option))
            return new(Tutor, Game);

        return default;
    }

    private static bool GetIsEnhancedTutor(EvoCriteria evo, ISpeciesForm current, int move, LearnOption option) => evo.Species is (int)Species.Rotom && move switch
    {
        (int)Move.Overheat  => option == LearnOption.AtAnyTime || current.Form == 1,
        (int)Move.HydroPump => option == LearnOption.AtAnyTime || current.Form == 2,
        (int)Move.Blizzard  => option == LearnOption.AtAnyTime || current.Form == 3,
        (int)Move.AirSlash  => option == LearnOption.AtAnyTime || current.Form == 4,
        (int)Move.LeafStorm => option == LearnOption.AtAnyTime || current.Form == 5,
        _ => false,
    };

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
            else
            {
                // Permit Defog to leak through if transferred to Gen5+ (via HG/SS)
                if (permit[CountTM + 4])
                    yield return (int)Move.Defog;
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

        if (types.HasFlagFast(MoveSourceType.EnhancedTutor))
        {
            if (evo.Species is (int)Species.Rotom && evo.Form is not 0)
                yield return MoveTutor.GetRotomFormMove(evo.Form);
        }
    }
}
