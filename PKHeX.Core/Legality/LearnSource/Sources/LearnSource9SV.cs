using System;
using System.Diagnostics.CodeAnalysis;
using static PKHeX.Core.LearnMethod;
using static PKHeX.Core.LearnEnvironment;

namespace PKHeX.Core;

/// <summary>
/// Exposes information about how moves are learned in <see cref="SV"/>.
/// </summary>
public sealed class LearnSource9SV : ILearnSource<PersonalInfo9SV>, IEggSource, IReminderSource
{
    public static readonly LearnSource9SV Instance = new();
    private static readonly PersonalTable9SV Personal = PersonalTable.SV;
    private static readonly Learnset[] Learnsets = Legal.LevelUpSV;
    private static readonly ushort[][] EggMoves = Legal.EggMovesSV;
    private static readonly ushort[][] Reminder = Legal.ReminderSV;
    private const int MaxSpecies = Legal.MaxSpeciesID_9;
    private const LearnEnvironment Game = SV;

    public Learnset GetLearnset(ushort species, byte form) => Learnsets[Personal.GetFormIndex(species, form)];

    public bool TryGetPersonal(ushort species, byte form, [NotNullWhen(true)] out PersonalInfo9SV? pi)
    {
        pi = null;
        if (species > MaxSpecies)
            return false;
        pi = Personal[species, form];
        return true;
    }

    public bool GetIsEggMove(ushort species, byte form, ushort move)
    {
        var index = Personal.GetFormIndex(species, form);
        if (index >= EggMoves.Length)
            return false;
        var moves = EggMoves[index].AsSpan();
        return moves.IndexOf(move) != -1;
    }

    public ReadOnlySpan<ushort> GetEggMoves(ushort species, byte form)
    {
        var index = Personal.GetFormIndex(species, form);
        if (index >= EggMoves.Length)
            return ReadOnlySpan<ushort>.Empty;
        return EggMoves[index];
    }

    public bool GetIsReminderMove(ushort species, byte form, ushort move)
    {
        var index = Personal.GetFormIndex(species, form);
        if (index >= Reminder.Length)
            return false;
        var moves = Reminder[index].AsSpan();
        return moves.IndexOf(move) != -1;
    }

    public ReadOnlySpan<ushort> GetReminderMoves(ushort species, byte form)
    {
        var index = Personal.GetFormIndex(species, form);
        if (index >= Reminder.Length)
            return ReadOnlySpan<ushort>.Empty;
        return Reminder[index];
    }

    public MoveLearnInfo GetCanLearn(PKM pk, PersonalInfo9SV pi, EvoCriteria evo, ushort move, MoveSourceType types = MoveSourceType.All, LearnOption option = LearnOption.Current)
    {
        if (types.HasFlag(MoveSourceType.LevelUp))
        {
            var learn = GetLearnset(evo.Species, evo.Form);
            var level = learn.GetLevelLearnMove(move);
            if (level != -1 && level <= evo.LevelMax)
                return new(LevelUp, Game, (byte)level);
        }

        if (types.HasFlag(MoveSourceType.SharedEggMove) && GetIsSharedEggMove(pi, move))
            return new(Shared, Game);

        if (types.HasFlag(MoveSourceType.Machine) && GetIsTM(pi, pk, move, option))
            return new(TMHM, Game);

        if (types.HasFlag(MoveSourceType.SpecialTutor) && GetIsReminderMove(evo.Species, evo.Form, move))
            return new(Tutor, Game);

        if (types.HasFlag(MoveSourceType.EnhancedTutor) && GetIsEnhancedTutor(evo, pk, move))
            return new(Tutor, Game);

        return default;
    }

    private static bool GetIsTM(PersonalInfo9SV info, PKM pk, ushort move, LearnOption option)
    {
        int index = info.RecordPermitIndexes.IndexOf(move);
        if (index == -1)
            return false;
        if (!info.GetIsLearnTM(index))
            return false;
        if (pk is not ITechRecord tr)
            return true;
        if (tr.GetMoveRecordFlag(index))
            return true;
        if (option != LearnOption.Current && !pk.SV && pk.IsOriginalMovesetDeleted())
            return true;
        return false;
    }

    private static bool GetIsEnhancedTutor(EvoCriteria evo, ISpeciesForm current, ushort move) => evo.Species switch
    {
        (int)Species.Rotom => move switch
        {
            (int)Move.Overheat => current.Form == 1,
            (int)Move.HydroPump => current.Form == 2,
            (int)Move.Blizzard => current.Form == 3,
            (int)Move.AirSlash => current.Form == 4,
            (int)Move.LeafStorm => current.Form == 5,
            _ => false,
        },
        _ => false,
    };

    private bool GetIsSharedEggMove(PersonalInfo9SV pi, ushort move)
    {
        var baseSpecies = pi.HatchSpecies;
        var baseForm = pi.HatchFormIndexEverstone;
        return GetEggMoves(baseSpecies, baseForm).IndexOf(move) != -1;
    }

    public void GetAllMoves(Span<bool> result, PKM pk, EvoCriteria evo, MoveSourceType types = MoveSourceType.All)
    {
        if (!TryGetPersonal(evo.Species, evo.Form, out var pi))
            return;

        if (types.HasFlag(MoveSourceType.LevelUp))
        {
            var learn = GetLearnset(evo.Species, evo.Form);
            (bool hasMoves, int start, int end) = learn.GetMoveRange(evo.LevelMax);
            if (hasMoves)
            {
                var moves = learn.Moves;
                for (int i = end; i >= start; i--)
                    result[moves[i]] = true;
            }
        }

        if (types.HasFlag(MoveSourceType.SharedEggMove))
        {
            var baseSpecies = pi.HatchSpecies;
            var baseForm = pi.HatchFormIndexEverstone;
            var egg = GetEggMoves(baseSpecies, baseForm);
            foreach (var move in egg)
                result[move] = true;
        }

        if (types.HasFlag(MoveSourceType.Machine))
            pi.SetAllLearnTM(result);

        if (types.HasFlag(MoveSourceType.SpecialTutor))
        {
            var reminder = GetReminderMoves(evo.Species, evo.Form);
            foreach (var move in reminder)
                result[move] = true;
        }

        if (types.HasFlag(MoveSourceType.EnhancedTutor))
        {
            var species = evo.Species;
            if (species is (int)Species.Rotom && pk.Form is not 0)
                result[MoveTutor.GetRotomFormMove(evo.Form)] = true;
        }
    }
}
