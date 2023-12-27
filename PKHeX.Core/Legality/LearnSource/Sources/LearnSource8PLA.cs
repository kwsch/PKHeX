using System;
using System.Diagnostics.CodeAnalysis;
using static PKHeX.Core.LearnMethod;
using static PKHeX.Core.LearnEnvironment;

namespace PKHeX.Core;

/// <summary>
/// Exposes information about how moves are learned in <see cref="PLA"/>.
/// </summary>
public sealed class LearnSource8LA : ILearnSource<PersonalInfo8LA>, IHomeSource
{
    public static readonly LearnSource8LA Instance = new();
    private static readonly PersonalTable8LA Personal = PersonalTable.LA;
    private static readonly Learnset[] Learnsets = LearnsetReader.GetArray(BinLinkerAccessor.Get(Util.GetBinaryResource("lvlmove_la.pkl"), "la"u8));
    private static readonly Learnset[] Mastery = LearnsetReader.GetArray(BinLinkerAccessor.Get(Util.GetBinaryResource("mastery_la.pkl"), "la"u8));
    private const int MaxSpecies = Legal.MaxSpeciesID_8a;
    private const LearnEnvironment Game = PLA;

    public Learnset GetLearnset(ushort species, byte form) => Learnsets[Personal.GetFormIndex(species, form)];

    public static (Learnset Learn, Learnset Mastery) GetLearnsetAndMastery(ushort species, byte form)
    {
        var index = Personal.GetFormIndex(species, form);
        return (Learnsets[index], Mastery[index]);
    }

    public bool TryGetPersonal(ushort species, byte form, [NotNullWhen(true)] out PersonalInfo8LA? pi)
    {
        pi = null;
        if (species > MaxSpecies)
            return false;
        pi = Personal[species, form];
        return true;
    }

    public MoveLearnInfo GetCanLearn(PKM pk, PersonalInfo8LA pi, EvoCriteria evo, ushort move, MoveSourceType types = MoveSourceType.All, LearnOption option = LearnOption.Current)
    {
        if (types.HasFlag(MoveSourceType.LevelUp))
        {
            var learn = GetLearnset(evo.Species, evo.Form);
            var level = learn.GetLevelLearnMove(move);
            if (level != -1 && level <= evo.LevelMax)
                return new(LevelUp, Game, (byte)level);
        }

        if (types.HasFlag(MoveSourceType.Machine) && pi.GetIsLearnMoveShop(move))
            return new(TMHM, Game);

        if (types.HasFlag(MoveSourceType.EnhancedTutor) && GetIsEnhancedTutor(evo, pk, move, option))
            return new(Tutor, Game);

        return default;
    }

    private static bool GetIsEnhancedTutor(EvoCriteria evo, ISpeciesForm current, ushort move, LearnOption option) => evo.Species is (int)Species.Rotom && move switch
    {
        (int)Move.Overheat  => option.IsPast() || current.Form == 1,
        (int)Move.HydroPump => option.IsPast() || current.Form == 2,
        (int)Move.Blizzard  => option.IsPast() || current.Form == 3,
        (int)Move.AirSlash  => option.IsPast() || current.Form == 4,
        (int)Move.LeafStorm => option.IsPast() || current.Form == 5,
        _ => false,
    };

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

        if (types.HasFlag(MoveSourceType.Machine))
            pi.SetAllLearnMoveShop(result);

        if (types.HasFlag(MoveSourceType.EnhancedTutor))
        {
            var species = evo.Species;
            if (species is (int)Species.Rotom && evo.Form is not 0)
                result[MoveTutor.GetRotomFormMove(evo.Form)] = true;
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

        if (types.HasFlag(MoveSourceType.Machine) && pi.GetIsLearnMoveShop(move))
            return new(TMHM, Game);

        if (types.HasFlag(MoveSourceType.EnhancedTutor) && GetIsEnhancedTutor(evo, pk, move, LearnOption.HOME))
            return new(Tutor, Game);

        return default;
    }
}
