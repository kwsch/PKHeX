using System;
using System.Diagnostics.CodeAnalysis;
using static PKHeX.Core.LearnMethod;
using static PKHeX.Core.LearnEnvironment;

namespace PKHeX.Core;

/// <summary>
/// Exposes information about how moves are learned in <see cref="SWSH"/>.
/// </summary>
public sealed class LearnSource8SWSH : ILearnSource<PersonalInfo8SWSH>, IEggSource, IHomeSource
{
    public static readonly LearnSource8SWSH Instance = new();
    private static readonly PersonalTable8SWSH Personal = PersonalTable.SWSH;
    private static readonly Learnset[] Learnsets = LearnsetReader.GetArray(BinLinkerAccessor.Get(Util.GetBinaryResource("lvlmove_swsh.pkl"), "ss"u8));
    private static readonly EggMoves7[] EggMoves = EggMoves7.GetArray(BinLinkerAccessor.Get(Util.GetBinaryResource("eggmove_swsh.pkl"), "ss"u8));
    private const int MaxSpecies = Legal.MaxSpeciesID_8_R2;
    private const LearnEnvironment Game = SWSH;

    public Learnset GetLearnset(ushort species, byte form) => Learnsets[Personal.GetFormIndex(species, form)];

    public bool TryGetPersonal(ushort species, byte form, [NotNullWhen(true)] out PersonalInfo8SWSH? pi)
    {
        pi = null;
        if (species > MaxSpecies)
            return false;
        pi = Personal[species, form];
        return true;
    }

    public bool GetIsEggMove(ushort species, byte form, ushort move)
    {
        if (species > MaxSpecies)
            return false;
        var moves = EggMoves.GetFormEggMoves(species, form);
        return moves.Contains(move);
    }

    public ReadOnlySpan<ushort> GetEggMoves(ushort species, byte form)
    {
        if (species > MaxSpecies)
            return [];
        return EggMoves.GetFormEggMoves(species, form);
    }

    public MoveLearnInfo GetCanLearn(PKM pk, PersonalInfo8SWSH pi, EvoCriteria evo, ushort move, MoveSourceType types = MoveSourceType.All, LearnOption option = LearnOption.Current)
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

        if (types.HasFlag(MoveSourceType.Machine) && pi.GetIsLearnTM(move))
            return new(TMHM, Game);

        if (types.HasFlag(MoveSourceType.TechnicalRecord) && GetIsTR(pi, pk, evo, move, option))
            return new(TMHM, Game);

        if (types.HasFlag(MoveSourceType.TypeTutor) && pi.GetIsLearnTutorType(move))
            return new(Tutor, Game);

        if (types.HasFlag(MoveSourceType.SpecialTutor) && pi.GetIsLearnTutorSpecial(move))
            return new(Tutor, Game);

        if (types.HasFlag(MoveSourceType.EnhancedTutor) && GetIsEnhancedTutor(evo, pk, move, option))
            return new(Tutor, Game);

        return default;
    }

    private static bool GetIsEnhancedTutor(EvoCriteria evo, ISpeciesForm current, ushort move, LearnOption option) => evo.Species switch
    {
        (int)Species.Necrozma => move switch
        {
            (int)Move.SunsteelStrike => option.IsPast() || current.Form == 1, // Sun w/ Solgaleo
            (int)Move.MoongeistBeam =>  option.IsPast() || current.Form == 2, // Moon w/ Lunala
            _ => false,
        },
        (int)Species.Rotom => move switch
        {
            (int)Move.Overheat  => option.IsPast() || current.Form == 1,
            (int)Move.HydroPump => option.IsPast() || current.Form == 2,
            (int)Move.Blizzard  => option.IsPast() || current.Form == 3,
            (int)Move.AirSlash  => option.IsPast() || current.Form == 4,
            (int)Move.LeafStorm => option.IsPast() || current.Form == 5,
            _ => false,
        },
        _ => false,
    };

    private bool GetIsSharedEggMove(PersonalInfo8SWSH pi, ushort move)
    {
        var baseSpecies = pi.HatchSpecies;
        var baseForm = pi.HatchFormIndexEverstone;
        return GetEggMoves(baseSpecies, baseForm).Contains(move);
    }

    private static bool GetIsTR(PersonalInfo8SWSH info, PKM pk, EvoCriteria evo, ushort move, LearnOption option)
    {
        var index = info.RecordPermitIndexes.IndexOf(move);
        if (index == -1)
            return false;
        if (!info.GetIsLearnTR(index))
            return false;

        if (pk is PK8 pk8)
        {
            if (pk8.GetMoveRecordFlag(index))
                return true;
            if (!option.IsFlagCheckRequired())
                return true;
        }
        else
        {
            if (option != LearnOption.Current)
                return true;
        }

        if (index == 12 && evo is { Species: (int)Species.Calyrex, Form: 0 }) // TR12
            return true; // Agility Calyrex without TR glitch.

        return false;
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
        {
            pi.SetAllLearnTM(result);
            pi.SetAllLearnTR(result);
            if (evo is { Species: (int)Species.Calyrex, Form: 0 })
                result[(int)Move.Agility] = true; // Agility Calyrex without TR glitch.
        }

        if (types.HasFlag(MoveSourceType.TypeTutor))
            pi.SetAllLearnTutorType(result);

        if (types.HasFlag(MoveSourceType.SpecialTutor))
            pi.SetAllLearnTutorSpecial(result);

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
        if (!TryGetPersonal(evo.Species, evo.Form, out var pi))
            return default;

        if (types.HasFlag(MoveSourceType.LevelUp))
        {
            var learn = GetLearnset(evo.Species, evo.Form);
            var level = learn.GetLevelLearnMove(move);
            if (level != -1)
                return new(LevelUp, Game, (byte)level);
        }

        if (types.HasFlag(MoveSourceType.SharedEggMove) && GetIsSharedEggMove(pi, move))
            return new(Shared, Game);

        if (types.HasFlag(MoveSourceType.Machine) && pi.GetIsLearnTM(move))
            return new(TMHM, Game);

        if (types.HasFlag(MoveSourceType.TechnicalRecord) && GetIsTR(pi, pk, evo, move, LearnOption.HOME))
            return new(TMHM, Game);

        return default;
    }
}
