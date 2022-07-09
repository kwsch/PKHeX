using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using static PKHeX.Core.LearnMethod;
using static PKHeX.Core.GameVersion;

namespace PKHeX.Core;

/// <summary>
/// Exposes information about how moves are learned in <see cref="RS"/>.
/// </summary>
public sealed class LearnSource3RS : ILearnSource, IEggSource
{
    public static readonly LearnSource3RS Instance = new();
    private static readonly PersonalTable Personal = PersonalTable.RS;
    private static readonly Learnset[] Learnsets = Legal.LevelUpRS;
    private static readonly EggMoves6[] EggMoves = Legal.EggMovesRS; // same for all Gen3 games
    private const int MaxSpecies = Legal.MaxSpeciesID_3;
    private const GameVersion Game = RS;
    private const int Generation = 3;
    private const int CountTM = 50;

    public Learnset GetLearnset(int species, int form) => Learnsets[species];

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

    private static bool GetIsTutor(int species, int move)
    {
        // XD (Mew)
        if (species == (int)Species.Mew && Legal.Tutor_3Mew.AsSpan().IndexOf(move) != -1)
            return true;

        return move switch
        {
            (int)Move.SelfDestruct => Array.BinarySearch(Legal.SpecialTutors_XD_SelfDestruct, (ushort)species) != -1,
            (int)Move.SkyAttack => Array.BinarySearch(Legal.SpecialTutors_XD_SkyAttack, (ushort)species) != -1,
            (int)Move.Nightmare => Array.BinarySearch(Legal.SpecialTutors_XD_Nightmare, (ushort)species) != -1,
            _ => false,
        };
    }

    private static bool GetIsTM(PersonalInfo info, int move)
    {
        var index = Array.IndexOf(Legal.TM_3, move);
        if (index == -1)
            return false;
        return info.TMHM[index];
    }

    private static bool GetIsHM(PersonalInfo info, int move)
    {
        var index = Array.IndexOf(Legal.HM_3, move);
        if (index == -1)
            return false;
        return info.TMHM[CountTM + index];
    }

    public IEnumerable<int> GetAllMoves(PKM pk, EvoCriteria evo, MoveSourceType types = MoveSourceType.All)
    {
        if (!TryGetPersonal(evo.Species, evo.Form, out var pi))
            yield break;

        if (types.HasFlagFast(MoveSourceType.LevelUp))
        {
            var learn = GetLearnset(evo.Species, evo.Form);
            foreach (var move in learn.GetMoves(evo.LevelMin, evo.LevelMax))
                yield return move;
        }

        if (types.HasFlagFast(MoveSourceType.Machine))
        {
            var permit = pi.TMHM;
            var moveIDs = Legal.TM_3;
            for (int i = 0; i < moveIDs.Length; i++)
            {
                if (permit[i])
                    yield return moveIDs[i];
            }

            if (pk.Format == Generation)
            {
                moveIDs = Legal.HM_3;
                for (int i = 0; i < moveIDs.Length; i++)
                {
                    if (permit[CountTM + i])
                        yield return moveIDs[i];
                }
            }
        }

        if (types.HasFlagFast(MoveSourceType.SpecialTutor))
        {
            if (evo.Species == (int)Species.Mew)
            {
                foreach (var m in Legal.Tutor_3Mew)
                    yield return m;
            }
            if (Array.BinarySearch(Legal.SpecialTutors_XD_SelfDestruct, evo.Species) != -1)
                yield return (int)Move.SelfDestruct;
            if (Array.BinarySearch(Legal.SpecialTutors_XD_SkyAttack, evo.Species) != -1)
                yield return (int)Move.SkyAttack;
            if (Array.BinarySearch(Legal.SpecialTutors_XD_Nightmare, evo.Species) != -1)
                yield return (int)Move.Nightmare;
        }
    }
}
