using System;
using System.Diagnostics.CodeAnalysis;
using static PKHeX.Core.LearnMethod;
using static PKHeX.Core.LearnEnvironment;
using static PKHeX.Core.LearnSource3;

namespace PKHeX.Core;

/// <summary>
/// Exposes information about how moves are learned in <see cref="E"/>.
/// </summary>
public sealed class LearnSource3E : ILearnSource<PersonalInfo3>, IEggSource
{
    public static readonly LearnSource3E Instance = new();
    private static readonly PersonalTable3 Personal = PersonalTable.E;
    private static readonly Learnset[] Learnsets = Legal.LevelUpE;
    private static readonly EggMoves6[] EggMoves = Legal.EggMovesRS; // same for all Gen3 games
    private const int MaxSpecies = Legal.MaxSpeciesID_3;
    private const LearnEnvironment Game = E;
    private const int Generation = 3;
    private const int CountTM = 50;

    public Learnset GetLearnset(ushort species, byte form) => Learnsets[species];
    internal PersonalInfo3 this[ushort species] => Personal[species];

    public bool TryGetPersonal(ushort species, byte form, [NotNullWhen(true)] out PersonalInfo3? pi)
    {
        pi = null;
        if (species > MaxSpecies)
            return false;
        pi = Personal[species];
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
            return ReadOnlySpan<ushort>.Empty;
        return EggMoves[species].Moves;
    }

    public MoveLearnInfo GetCanLearn(PKM pk, PersonalInfo3 pi, EvoCriteria evo, ushort move, MoveSourceType types = MoveSourceType.All, LearnOption option = LearnOption.Current)
    {
        if (types.HasFlag(MoveSourceType.LevelUp))
        {
            var learn = GetLearnset(evo.Species, evo.Form);
            var level = learn.GetLevelLearnMove(move);
            if (level != -1 && level <= evo.LevelMax)
                return new(LevelUp, Game, (byte)level);
        }

        if (types.HasFlag(MoveSourceType.Machine))
        {
            if (GetIsTM(pi, move))
                return new(TMHM, Game);
            if (pk.Format == Generation && GetIsHM(pi, move))
                return new(TMHM, Game);
        }

        if (types.HasFlag(MoveSourceType.SpecialTutor) && GetIsSpecialTutor(evo.Species, move))
            return new(Tutor, Game);

        return default;
    }

    private static bool GetIsSpecialTutor(ushort species, ushort move)
    {
        var info = Personal[species];
        var index = Array.IndexOf(Tutor_E, move);
        if (index == -1)
            return false;
        return info.TypeTutors[index];
    }

    private static bool GetIsTM(PersonalInfo3 info, ushort move)
    {
        var index = Array.IndexOf(TM_3, move);
        if (index == -1)
            return false;
        return info.TMHM[index];
    }

    private static bool GetIsHM(PersonalInfo3 info, ushort move)
    {
        var index = Array.IndexOf(HM_3, move);
        if (index == -1)
            return false;
        return info.TMHM[CountTM + index];
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

        if (types.HasFlag(MoveSourceType.Machine))
        {
            var flags = pi.TMHM;
            var moves = TM_3;
            for (int i = 0; i < moves.Length; i++)
            {
                if (flags[i])
                    result[moves[i]] = true;
            }

            if (pk.Format == 3)
            {
                moves = HM_3;
                for (int i = 0; i < moves.Length; i++)
                {
                    if (flags[CountTM + i])
                        result[moves[i]] = true;
                }
            }
        }

        if (types.HasFlag(MoveSourceType.SpecialTutor))
        {
            var flags = pi.TypeTutors;
            var moves = Tutor_E;
            for (int i = 0; i < moves.Length; i++)
            {
                if (flags[i])
                    result[moves[i]] = true;
            }
        }
    }

    private static readonly ushort[] Tutor_E =
    {
        005, 014, 025, 034, 038, 068, 069, 102, 118, 135,
        138, 086, 153, 157, 164, 223, 205, 244, 173, 196,
        203, 189, 008, 207, 214, 129, 111, 009, 007, 210,
    };
}
