using System;
using System.Diagnostics.CodeAnalysis;
using static PKHeX.Core.LearnMethod;
using static PKHeX.Core.LearnEnvironment;
using static PKHeX.Core.LearnSource3;

namespace PKHeX.Core;

/// <summary>
/// Exposes information about how moves are learned in <see cref="FR"/>.
/// </summary>
public sealed class LearnSource3FR : ILearnSource, IEggSource
{
    public static readonly LearnSource3FR Instance = new();
    private static readonly PersonalTable3 Personal = PersonalTable.FR;
    private static readonly Learnset[] Learnsets = Legal.LevelUpFR;
    private static readonly EggMoves6[] EggMoves = Legal.EggMovesRS; // same for all Gen3 games
    private const int MaxSpecies = Legal.MaxSpeciesID_3;
    private const LearnEnvironment Game = FR;
    private const int Generation = 3;
    private const int CountTM = 50;

    public Learnset GetLearnset(int species, int form) => Learnsets[species];
    internal PersonalInfo this[int species] => Personal[species];

    public bool TryGetPersonal(int species, int form, [NotNullWhen(true)] out PersonalInfo? pi)
    {
        pi = null;
        if ((uint)species > MaxSpecies)
            return false;
        pi = Personal[species];
        return true;
    }

    public bool GetIsEggMove(int species, int form, int move)
    {
        if ((uint)species > MaxSpecies)
            return false;
        var moves = EggMoves[species];
        return moves.GetHasEggMove(move);
    }

    public ReadOnlySpan<int> GetEggMoves(int species, int form)
    {
        if ((uint)species > MaxSpecies)
            return ReadOnlySpan<int>.Empty;
        return EggMoves[species].Moves;
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

        if (types.HasFlagFast(MoveSourceType.Machine))
        {
            if (GetIsTM(pi, move))
                return new(TMHM, Game);
            if (pk.Format == Generation && GetIsHM(pi, move))
                return new(TMHM, Game);
        }

        if (types.HasFlagFast(MoveSourceType.SpecialTutor) && GetIsTutor(evo.Species, move))
            return new(Tutor, Game);

        return default;
    }

    private static bool GetIsTutor(int species, int move) => move switch
    {
        (int)Move.BlastBurn   => species == (int)Species.Charizard,
        (int)Move.HydroCannon => species == (int)Species.Blastoise,
        (int)Move.FrenzyPlant => species == (int)Species.Venusaur,
        _ => false,
    };

    private static bool GetIsTM(PersonalInfo info, int move)
    {
        var index = Array.IndexOf(TM_3, move);
        if (index == -1)
            return false;
        return info.TMHM[index];
    }

    private static bool GetIsHM(PersonalInfo info, int move)
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

        if (types.HasFlagFast(MoveSourceType.SpecialTutor))
        {
            if (evo.Species == (int)Species.Charizard)
                result[(int)Move.BlastBurn] = true;
            else if (evo.Species == (int)Species.Blastoise)
                result[(int)Move.HydroCannon] = true;
            else if (evo.Species == (int)Species.Venusaur)
                result[(int)Move.FrenzyPlant] = true;
        }
    }
}
