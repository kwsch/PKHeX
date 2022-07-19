using System;
using System.Diagnostics.CodeAnalysis;
using static PKHeX.Core.LearnMethod;
using static PKHeX.Core.LearnEnvironment;

namespace PKHeX.Core;

/// <summary>
/// Exposes information about how moves are learned in <see cref="PLA"/>.
/// </summary>
public sealed class LearnSource8LA : ILearnSource
{
    public static readonly LearnSource8LA Instance = new();
    private static readonly PersonalTable8LA Personal = PersonalTable.LA;
    private static readonly Learnset[] Learnsets = Legal.LevelUpLA;
    private const int MaxSpecies = Legal.MaxSpeciesID_8a;
    private const LearnEnvironment Game = PLA;

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

        if (types.HasFlagFast(MoveSourceType.Machine) && GetIsMoveShop(pi, move))
            return new(TMHM, Game);

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

    private static bool GetIsMoveShop(PersonalInfo pi, int move)
    {
        var index = Legal.MoveShop8_LA.AsSpan().IndexOf((ushort)move);
        if (index == -1)
            return false;
        return pi.SpecialTutors[0][index];
    }

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

        if (types.HasFlagFast(MoveSourceType.Machine))
        {
            var flags = pi.SpecialTutors[0];
            var moves = Legal.MoveShop8_LA;
            for (int i = 0; i < moves.Length; i++)
            {
                if (flags[i])
                    result[moves[i]] = true;
            }
        }

        if (types.HasFlagFast(MoveSourceType.EnhancedTutor))
        {
            var species = evo.Species;
            if (species is (int)Species.Rotom && evo.Form is not 0)
                result[MoveTutor.GetRotomFormMove(evo.Form)] = true;
        }
    }
}
