using System;
using System.Diagnostics.CodeAnalysis;
using static PKHeX.Core.LearnMethod;
using static PKHeX.Core.LearnEnvironment;
using static PKHeX.Core.PersonalInfo5BW;

namespace PKHeX.Core;

/// <summary>
/// Exposes information about how moves are learned in <see cref="BW"/>.
/// </summary>
public sealed class LearnSource5BW : LearnSource5, ILearnSource<PersonalInfo5BW>, IEggSource
{
    public static readonly LearnSource5BW Instance = new();
    private static readonly PersonalTable5BW Personal = PersonalTable.BW;
    private static readonly Learnset[] Learnsets = LearnsetReader.GetArray(BinLinkerAccessor.Get(Util.GetBinaryResource("lvlmove_bw.pkl"), "51"u8));
    private const int MaxSpecies = Legal.MaxSpeciesID_5;
    private const LearnEnvironment Game = BW;

    public LearnEnvironment Environment => Game;

    public Learnset GetLearnset(ushort species, byte form) => Learnsets[Personal.GetFormIndex(species, form)];

    public bool TryGetPersonal(ushort species, byte form, [NotNullWhen(true)] out PersonalInfo5BW? pi)
    {
        pi = null;
        if (!Personal.IsPresentInGame(species, form))
            return false;
        pi = Personal[species, form];
        return true;
    }

    public bool GetIsEggMove(ushort species, byte form, ushort move)
    {
        if (species > MaxSpecies)
            return false;
        var moves = EggMoves[species];
        return moves.GetHasEggMove(move);
    }

    public ReadOnlySpan<ushort> GetEggMoves(ushort species, byte form)
    {
        if (species > MaxSpecies)
            return [];
        return EggMoves[species].Moves;
    }

    public MoveLearnInfo GetCanLearn(PKM pk, PersonalInfo5BW pi, EvoCriteria evo, ushort move, MoveSourceType types = MoveSourceType.All, LearnOption option = LearnOption.Current)
    {
        if (types.HasFlag(MoveSourceType.LevelUp))
        {
            var learn = GetLearnset(evo.Species, evo.Form);
            if (learn.TryGetLevelLearnMove(move, out var level) && level <= evo.LevelMax)
                return new(LevelUp, Game, level);
        }

        if (types.HasFlag(MoveSourceType.Machine) && GetIsTM(pi, move))
            return new(TMHM, Game);

        if (types.HasFlag(MoveSourceType.TypeTutor) && GetIsTypeTutor(pi, move))
            return new(Tutor, Game);

        if (types.HasFlag(MoveSourceType.EnhancedTutor) && GetIsEnhancedTutor(evo, pk, move, option))
            return new(Tutor, Game);

        return default;
    }

    private static bool GetIsEnhancedTutor<T1, T2>(T1 evo, T2 current, ushort move, LearnOption option)
        where T1 : ISpeciesForm
        where T2 : ISpeciesForm
        => evo.Species switch
    {
        (int)Species.Keldeo => move is (int)Move.SecretSword,
        (int)Species.Meloetta => move is (int)Move.RelicSong,
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

    private static bool GetIsTypeTutor(PersonalInfo5BW pi, ushort move)
    {
        var index = TypeTutorMoves.IndexOf(move);
        if (index == -1)
            return false;
        return pi.GetIsLearnTutorType(index);
    }

    private static bool GetIsTM(PersonalInfo5BW info, ushort move)
    {
        var index = MachineMoves.IndexOf(move);
        if (index == -1)
            return false;
        return info.GetIsLearnTM(index) && index != 94; // TM95 not available in this game
    }

    public void GetAllMoves(Span<bool> result, PKM _, EvoCriteria evo, MoveSourceType types = MoveSourceType.All)
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

        if (types.HasFlag(MoveSourceType.Machine))
        {
            // TM95 is unavailable (Snarl - Lock Capsule)
            // Cache the current value to clear it if so.
            var knowSnarlOtherSource = result[(int)Move.Snarl];
            pi.SetAllLearnTM(result, MachineMoves);
            if (!knowSnarlOtherSource)
                result[(int)Move.Snarl] = false;
        }

        if (types.HasFlag(MoveSourceType.TypeTutor))
        {
            pi.SetAllLearnTutorType(result, TypeTutorMoves);
        }

        if (types.HasFlag(MoveSourceType.EnhancedTutor))
        {
            var species = evo.Species;
            if (species is (int)Species.Rotom && evo.Form is not 0)
                result[MoveTutor.GetRotomFormMove(evo.Form)] = true;
            else if (species is (int)Species.Keldeo)
                result[(int)Move.SecretSword] = true;
            else if (species is (int)Species.Meloetta)
                result[(int)Move.RelicSong] = true;
        }
    }
}
