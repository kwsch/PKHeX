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
        return EggMoves[index].AsSpan();
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
        return Reminder[index].AsSpan();
    }

    public MoveLearnInfo GetCanLearn(PKM pk, PersonalInfo9SV pi, EvoCriteria evo, ushort move, MoveSourceType types = MoveSourceType.All, LearnOption option = LearnOption.Current)
    {
        if (types.HasFlagFast(MoveSourceType.LevelUp))
        {
            var learn = GetLearnset(evo.Species, evo.Form);
            var level = learn.GetLevelLearnMove(move);
            if (level != -1 && level <= evo.LevelMax)
                return new(LevelUp, Game, (byte)level);
        }

        if (types.HasFlagFast(MoveSourceType.SharedEggMove) && GetIsEggMove(evo.Species, evo.Form, move))
            return new(Shared, Game);

        if (types.HasFlagFast(MoveSourceType.Machine) && GetIsTM(pi, pk, move, option))
            return new(TMHM, Game);

        if (types.HasFlagFast(MoveSourceType.SpecialTutor) && GetIsReminderMove(evo.Species, evo.Form, move))
            return new(Tutor, Game);

        if (types.HasFlagFast(MoveSourceType.EnhancedTutor) && GetIsEnhancedTutor(evo, pk, move))
            return new(Tutor, Game);

        return default;
    }

    private static bool GetIsTM(PersonalInfo9SV info, PKM pk, ushort move, LearnOption option)
    {
        var index = TM_SV.AsSpan().IndexOf(move);
        if (index == -1)
            return false;
        if (!info.TMHM[index])
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

    public void GetAllMoves(Span<bool> result, PKM pk, EvoCriteria evo, MoveSourceType types = MoveSourceType.All)
    {
        if (!TryGetPersonal(evo.Species, evo.Form, out var pi))
            return;

        if (types.HasFlagFast(MoveSourceType.LevelUp))
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

        if (types.HasFlagFast(MoveSourceType.SharedEggMove))
        {
            var baseSpecies = pi.HatchSpecies;
            var baseForm = pi.HatchFormIndexEverstone;
            var egg = GetEggMoves(baseSpecies, baseForm);
            foreach (var move in egg)
                result[move] = true;
        }

        if (types.HasFlagFast(MoveSourceType.Machine))
        {
            var flags = pi.TMHM;
            var moves = TM_SV;
            for (int i = 0; i < moves.Length; i++)
            {
                if (flags[i])
                    result[moves[i]] = true;
            }
        }

        if (types.HasFlagFast(MoveSourceType.SpecialTutor))
        {
            var reminder = GetReminderMoves(evo.Species, evo.Form);
            foreach (var move in reminder)
                result[move] = true;
        }

        if (types.HasFlagFast(MoveSourceType.EnhancedTutor))
        {
            var species = evo.Species;
            if (species is (int)Species.Rotom && pk.Form is not 0)
                result[MoveTutor.GetRotomFormMove(evo.Form)] = true;
        }
    }

    public static readonly ushort[] TM_SV =
    {
        005, 036, 204, 313, 097, 189, 184, 182, 424, 422,
        423, 352, 067, 491, 512, 522, 060, 109, 168, 574,
        885, 884, 886, 451, 083, 263, 342, 332, 523, 506,
        555, 232, 129, 345, 196, 341, 317, 577, 488, 490,
        314, 500, 101, 374, 525, 474, 419, 203, 521, 241,
        240, 201, 883, 684, 473, 091, 331, 206, 280, 428,
        369, 421, 492, 706, 339, 403, 034, 007, 009, 008,
        214, 402, 486, 409, 115, 113, 350, 127, 337, 605,
        118, 447, 086, 398, 707, 156, 157, 269, 014, 776,
        191, 390, 286, 430, 399, 141, 598, 019, 285, 442,
        349, 408, 441, 164, 334, 404, 529, 261, 242, 271,
        710, 202, 396, 366, 247, 406, 446, 304, 257, 412,
        094, 484, 227, 057, 861, 053, 085, 583, 133, 347,
        270, 676, 226, 414, 179, 058, 604, 580, 678, 581,
        417, 126, 056, 059, 519, 518, 520, 528, 188, 089,
        444, 566, 416, 307, 308, 338, 200, 315, 411, 437,
        542, 433, 405, 063, 413, 394, 087, 370, 076, 434,
        796, 851,
    };
}
