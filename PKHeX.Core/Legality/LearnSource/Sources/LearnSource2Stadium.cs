using System;
using System.Diagnostics.CodeAnalysis;
using static PKHeX.Core.LearnMethod;
using static PKHeX.Core.LearnEnvironment;

namespace PKHeX.Core;

public sealed class LearnSource2Stadium : ILearnSource<PersonalInfo2>, IEggSource
{
    public static readonly LearnSource2Stadium Instance = new();
    private static readonly LearnsetStadium[] Learnsets = LearnsetStadium.GetArray(BinLinkerAccessor16.Get(Util.GetBinaryResource("reminder_stad2.pkl"), "s2"u8));

    private const int MaxSpecies = Legal.MaxSpeciesID_2;
    private const LearnEnvironment Game = Stadium2;
    public LearnEnvironment Environment => Game;

    public Learnset GetLearnset(ushort species, byte form) => throw new InvalidOperationException("Not supported.");
    public bool TryGetPersonal(ushort species, byte form, [NotNullWhen(true)] out PersonalInfo2? pi) => throw new InvalidOperationException("Not supported.");
    public bool GetIsEggMove(ushort species, byte form, ushort move) => false;
    public ReadOnlySpan<ushort> GetEggMoves(ushort species, byte form) => [];
    private static bool IsSpeciesInGame(ushort species, byte form) => form is 0 && species is <= MaxSpecies and not 0;
    public LearnsetStadium GetLearnsetStadium(ushort species, byte form) => Learnsets[species < Learnsets.Length ? species : 0];

    public MoveLearnInfo GetCanLearn(PKM pk, PersonalInfo2 pi, EvoCriteria evo, ushort move, MoveSourceType types = MoveSourceType.All, LearnOption option = LearnOption.Current)
    {
        return GetCanRelearn(evo, move, types);
    }

    public MoveLearnInfo GetCanRelearn(EvoCriteria evo, ushort move, MoveSourceType types)
    {
        if (move > Legal.MaxMoveID_2) // byte
            return default;

        if (types.HasFlag(MoveSourceType.LevelUp))
        {
            var learn = GetLearnsetStadium(evo.Species, evo.Form);
            if (learn.CanRelearn(move, evo.LevelMax, out var level))
                return new(LevelUp, Game, level);
        }

        return default;
    }

    public void GetAllMoves(Span<bool> result, PKM pk, EvoCriteria evo, MoveSourceType types = MoveSourceType.All)
    {
        if (!IsSpeciesInGame(evo.Species, evo.Form))
            return;

        if (!types.HasFlag(MoveSourceType.LevelUp))
            return;

        bool removeVC = pk.Format == 1 || pk.VC1;
        var learn = GetLearnsetStadium(evo.Species, evo.Form);
        var span = learn.GetMoveRange(evo.LevelMax);
        foreach (var tuple in span)
        {
            if (!tuple.Source.IsAbleToBeRelearned())
                continue;
            var move = tuple.Move;
            if (!removeVC || move <= Legal.MaxMoveID_1)
                result[move] = true;
        }
    }
}
