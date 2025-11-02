using System;
using System.Diagnostics.CodeAnalysis;
using static PKHeX.Core.LearnMethod;
using static PKHeX.Core.LearnEnvironment;

namespace PKHeX.Core;

/// <summary>
/// Exposes information about how moves are learned in <see cref="LearnEnvironment.ZA"/>.
/// </summary>
public sealed class LearnSource9ZA : ILearnSource<PersonalInfo9ZA>, IHomeSource, ILearnSourceBonus
{
    public static readonly LearnSource9ZA Instance = new();
    private static readonly PersonalTable9ZA Personal = PersonalTable.ZA;
    private static readonly Learnset[] Learnsets = LearnsetReader.GetArray(BinLinkerAccessor16.Get(Util.GetBinaryResource("lvlmove_za.pkl"), "za"u8));
    private static readonly Learnset[] PlusLevels = LearnsetReader.GetArray(BinLinkerAccessor16.Get(Util.GetBinaryResource("plus_za.pkl"), "za"u8));
    private const int MaxSpecies = Legal.MaxSpeciesID_9a;
    private const LearnEnvironment Game = ZA;

    public Learnset GetLearnset(ushort species, byte form) => Learnsets[Personal.GetFormIndex(species, form)];

    public static (Learnset Learn, Learnset Plus) GetLearnsetAndPlus(ushort species, byte form)
    {
        var index = Personal.GetFormIndex(species, form);
        return (Learnsets[index], PlusLevels[index]);
    }

    public (Learnset Learn, Learnset Other) GetLearnsetAndOther(ushort species, byte form)
    {
        var index = Personal.GetFormIndex(species, form);
        return (Learnsets[index], PlusLevels[index]);
    }

    public bool TryGetPersonal(ushort species, byte form, [NotNullWhen(true)] out PersonalInfo9ZA? pi)
    {
        pi = null;
        if (species > MaxSpecies)
            return false;
        pi = Personal[species, form];
        return true;
    }

    public MoveLearnInfo GetCanLearn(PKM pk, PersonalInfo9ZA pi, EvoCriteria evo, ushort move, MoveSourceType types = MoveSourceType.All, LearnOption option = LearnOption.Current)
    {
        if (types.HasFlag(MoveSourceType.LevelUp))
        {
            var learn = GetLearnset(evo.Species, evo.Form);
            if (learn.TryGetLevelLearnMove(move, out var level) && level <= evo.LevelMax)
                return new(LevelUp, Game, level);
        }

        if (types.HasFlag(MoveSourceType.Machine) && GetIsTM(pi, pk, move, option))
            return new(TMHM, Game);

        return default;
    }

    private static bool GetIsTM(PersonalInfo9ZA info, PKM pk, ushort move, LearnOption option)
    {
        int index = PersonalInfo9ZA.MachineMoves.IndexOf(move);
        if (index == -1)
            return false;
        if (!info.GetIsLearnTM(index))
            return false;

        // Can just use the TM at any time. Does not set record flag.
        return true;
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

        if (types.HasFlag(MoveSourceType.Machine))
            pi.SetAllLearnTM(result);
    }

    public LearnEnvironment Environment => Game;

    public MoveLearnInfo GetCanLearnHOME(PKM pk, EvoCriteria evo, ushort move, MoveSourceType types = MoveSourceType.All)
    {
        var pi = Personal[evo.Species, evo.Form];

        if (types.HasFlag(MoveSourceType.LevelUp))
        {
            var learn = GetLearnset(evo.Species, evo.Form);
            if (learn.TryGetLevelLearnMove(move, out var level))
                return new(LevelUp, Game, level);
        }

        if (types.HasFlag(MoveSourceType.Machine) && GetIsTM(pi, pk, move, LearnOption.HOME))
            return new(TMHM, Game);

        return default;
    }
}
