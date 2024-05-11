using System;
using System.Diagnostics.CodeAnalysis;
using static PKHeX.Core.LearnMethod;
using static PKHeX.Core.LearnEnvironment;

namespace PKHeX.Core;

/// <summary>
/// Exposes information about how moves are learned in <see cref="SV"/>.
/// </summary>
public sealed class LearnSource9SV : ILearnSource<PersonalInfo9SV>, IEggSource, IReminderSource, IHomeSource
{
    public static readonly LearnSource9SV Instance = new();
    private static readonly PersonalTable9SV Personal = PersonalTable.SV;
    private static readonly Learnset[] Learnsets = LearnsetReader.GetArray(BinLinkerAccessor.Get(Util.GetBinaryResource("lvlmove_sv.pkl"), "sv"u8));
    private static readonly ushort[][] EggMoves = EggMoves9.GetArray(BinLinkerAccessor.Get(Util.GetBinaryResource("eggmove_sv.pkl"), "sv"u8));
    private static readonly ushort[][] Reminder = EggMoves9.GetArray(BinLinkerAccessor.Get(Util.GetBinaryResource("reminder_sv.pkl"), "sv"u8));
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
        return moves.Contains(move);
    }

    public ReadOnlySpan<ushort> GetEggMoves(ushort species, byte form)
    {
        var index = Personal.GetFormIndex(species, form);
        if (index >= EggMoves.Length)
            return [];
        return EggMoves[index];
    }

    public bool GetIsReminderMove(ushort species, byte form, ushort move)
    {
        var index = Personal.GetFormIndex(species, form);
        if (index >= Reminder.Length)
            return false;
        var moves = Reminder[index].AsSpan();
        return moves.Contains(move);
    }

    public ReadOnlySpan<ushort> GetReminderMoves(ushort species, byte form)
    {
        var index = Personal.GetFormIndex(species, form);
        if (index >= Reminder.Length)
            return [];
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

        if (types.HasFlag(MoveSourceType.EnhancedTutor) && GetIsEnhancedTutor(evo, pk, move, option))
            return new(Tutor, Game);

        // In 2.0.1, the following moves are no longer learned via Level Up.
        // Since they could have been learned via Level Up prior to 2.0.1, we need to check for them.
        // This is double-checked outside of this method -- this is a silly workaround.
        if (types.HasFlag(MoveSourceType.LevelUp))
        {
            if (move == (int)Move.BugBite && evo is { Species: (int)Species.Larvesta, Form: 0, LevelMax: >= 28 })
                return new(LevelUp, Game, 28);
            if (move == (int)Move.Spite   && evo is { Species: (int)Species.Zorua   , Form: 1, LevelMax: >= 24 })
                return new(LevelUp, Game, 28);
        }

        return default;
    }

    private static bool GetIsTM(PersonalInfo9SV info, PKM pk, ushort move, LearnOption option)
    {
        int index = info.RecordPermitIndexes.IndexOf(move);
        if (index == -1)
            return false;
        if (!info.GetIsLearnTM(index))
            return false;

        if (pk is PK9 pk9)
        {
            if (pk9.GetMoveRecordFlag(index))
                return true;
            if (!option.IsFlagCheckRequired())
                return true;
        }
        else
        {
            if (option != LearnOption.Current)
                return true;
        }
        return false;
    }

    private static bool GetIsEnhancedTutor(EvoCriteria evo, ISpeciesForm current, ushort move, LearnOption option) => evo.Species switch
    {
        (int)Species.Necrozma => move switch
        {
            (int)Move.SunsteelStrike => option.IsPast() || current.Form == 1, // Sun w/ Solgaleo
            (int)Move.MoongeistBeam => option.IsPast() || current.Form == 2, // Moon w/ Lunala
            _ => false,
        },
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
        return GetEggMoves(baseSpecies, baseForm).Contains(move);
    }

    public void GetAllMoves(Span<bool> result, PKM pk, EvoCriteria evo, MoveSourceType types = MoveSourceType.All)
    {
        if (!TryGetPersonal(evo.Species, evo.Form, out var pi))
            return;

        if (types.HasFlag(MoveSourceType.LevelUp))
        {
            var learn = GetLearnset(evo.Species, evo.Form);
            var span = learn.GetMoveRange(evo.LevelMax);
            foreach (var move in span)
                result[move] = true;
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
            else if (species is (int)Species.Necrozma && pk.Form is 1) // Sun
                result[(int)Move.SunsteelStrike] = true;
            else if (species is (int)Species.Necrozma && pk.Form is 2) // Moon
                result[(int)Move.MoongeistBeam] = true;
        }
    }

    public LearnEnvironment Environment => Game;

    public MoveLearnInfo GetCanLearnHOME(PKM pk, EvoCriteria evo, ushort move, MoveSourceType types = MoveSourceType.All)
    {
        var pi = Personal[evo.Species, evo.Form];

        if (types.HasFlag(MoveSourceType.LevelUp))
        {
            var learn = GetLearnset(evo.Species, evo.Form);
            var level = learn.GetLevelLearnMove(move);
            if (level != -1)
                return new(LevelUp, Game, (byte)level);
        }

        if (types.HasFlag(MoveSourceType.SharedEggMove) && GetIsSharedEggMove(pi, move))
            return new(Shared, Game);

        if (types.HasFlag(MoveSourceType.Machine) && GetIsTM(pi, pk, move, LearnOption.HOME))
            return new(TMHM, Game);

        if (types.HasFlag(MoveSourceType.SpecialTutor) && GetIsReminderMove(evo.Species, evo.Form, move))
            return new(Tutor, Game);

        return default;
    }
}
