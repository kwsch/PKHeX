using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using static PKHeX.Core.LearnMethod;
using static PKHeX.Core.LearnEnvironment;

namespace PKHeX.Core;

/// <summary>
/// Exposes information about how moves are learned in <see cref="SWSH"/>.
/// </summary>
public sealed class LearnSource8SWSH : ILearnSource, IEggSource
{
    public static readonly LearnSource8SWSH Instance = new();
    private static readonly PersonalTable Personal = PersonalTable.SWSH;
    private static readonly Learnset[] Learnsets = Legal.LevelUpSWSH;
    private static readonly EggMoves7[] EggMoves = Legal.EggMovesSWSH;
    private const int MaxSpecies = Legal.MaxSpeciesID_8_R2;
    private const LearnEnvironment Game = SWSH;

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

    public MoveLearnInfo GetCanLearn(PKM pk, PersonalInfo pi, EvoCriteria evo, int move, MoveSourceType types = MoveSourceType.All, LearnOption option = LearnOption.Current)
    {
        if (types.HasFlagFast(MoveSourceType.LevelUp))
        {
            var learn = GetLearnset(evo.Species, evo.Form);
            var level = learn.GetLevelLearnMove(move);
            if (level != -1 && level <= evo.LevelMax)
                return new(LevelUp, Game, (byte)level);
        }

        if (types.HasFlagFast(MoveSourceType.SharedEggMove) && GetIsSharedEggMove(pi, move))
            return new(Shared, Game);

        if (types.HasFlagFast(MoveSourceType.Machine) && GetIsTM(pi, move))
            return new(TMHM, Game);

        if (types.HasFlagFast(MoveSourceType.TechnicalRecord) && pk is ITechRecord8 tr && GetIsTR(pi, tr, evo, move))
            return new(TMHM, Game);

        if (types.HasFlagFast(MoveSourceType.TypeTutor) && GetIsTypeTutor(pi, move))
            return new(Tutor, Game);

        if (types.HasFlagFast(MoveSourceType.EnhancedTutor) && GetIsEnhancedTutor(evo, pk, move, option))
            return new(Tutor, Game);

        return default;
    }

    private static bool GetIsEnhancedTutor(EvoCriteria evo, ISpeciesForm current, int move, LearnOption option) => evo.Species switch
    {
        (int)Species.Necrozma => move switch
        {
            (int)Move.SunsteelStrike => (option == LearnOption.AtAnyTime || current.Form == 1), // Sun w/ Solgaleo
            (int)Move.MoongeistBeam => (option == LearnOption.AtAnyTime || current.Form == 2), // Moon w/ Lunala
            _ => false,
        },
        (int)Species.Rotom => move switch
        {
            (int)Move.Overheat  => option == LearnOption.AtAnyTime || current.Form == 1,
            (int)Move.HydroPump => option == LearnOption.AtAnyTime || current.Form == 2,
            (int)Move.Blizzard  => option == LearnOption.AtAnyTime || current.Form == 3,
            (int)Move.AirSlash  => option == LearnOption.AtAnyTime || current.Form == 4,
            (int)Move.LeafStorm => option == LearnOption.AtAnyTime || current.Form == 5,
            _ => false,
        },
        _ => false,
    };

    private bool GetIsSharedEggMove(PersonalInfo pi, int move)
    {
        var entry = (PersonalInfoSWSH)pi;
        var baseSpecies = entry.HatchSpecies;
        var baseForm = entry.HatchFormIndexEverstone;
        return GetEggMoves(baseSpecies, baseForm).IndexOf(move) != -1;
    }

    private static bool GetIsTypeTutor(PersonalInfo pi, int move)
    {
        var index = Array.IndexOf(Legal.TypeTutor8, move);
        if (index == -1)
            return false;
        return pi.TypeTutors[index];
    }

    private static bool GetIsTM(PersonalInfo info, int move)
    {
        var index = Legal.TMHM_SWSH.AsSpan(0, PersonalInfoSWSH.CountTM).IndexOf(move);
        if (index == -1)
            return false;
        return info.TMHM[index];
    }

    private static bool GetIsTR(PersonalInfo info, ITechRecord8 tr, EvoCriteria evo, int move)
    {
        var index = Legal.TMHM_SWSH.AsSpan(PersonalInfoSWSH.CountTM).IndexOf(move);
        if (index == -1)
            return false;
        if (!info.TMHM[PersonalInfoSWSH.CountTM + index])
            return false;

        if (tr.GetMoveRecordFlag(index))
            return true;
        if (index == 12 && evo.Species == (int)Species.Calyrex && evo.Form == 0) // TR12
            return true; // Agility Calyrex without TR glitch.

        return false;
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
            var moveIDs = Legal.TMHM_SWSH;
            for (int i = 0; i < PersonalInfoSWSH.CountTM; i++)
            {
                if (permit[i])
                    yield return moveIDs[i];
            }

            if (pk is ITechRecord8 tr)
            {
                for (int index = 0; index < moveIDs.Length; index++)
                {
                    var move = moveIDs[PersonalInfoSWSH.CountTM + index];
                    if (!permit[PersonalInfoSWSH.CountTM + index])
                        yield return move;
                    if (tr.GetMoveRecordFlag(index))
                        yield return move;
                    else if (index == 12 && evo.Species == (int)Species.Calyrex && evo.Form == 0) // TR12
                        yield return move; // Agility Calyrex without TR glitch.
                }
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

        if (types.HasFlagFast(MoveSourceType.EnhancedTutor))
        {
            var species = evo.Species;
            if (species is (int)Species.Rotom && pk.Form is not 0)
                yield return MoveTutor.GetRotomFormMove(evo.Form);
            else if (species is (int)Species.Necrozma && pk.Form is 1) // Sun
                yield return (int)Move.SunsteelStrike;
            else if (species is (int)Species.Necrozma && pk.Form is 2) // Moon
                yield return (int)Move.MoongeistBeam;
        }
    }
}
