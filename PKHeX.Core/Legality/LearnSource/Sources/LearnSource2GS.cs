using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using static PKHeX.Core.LearnMethod;
using static PKHeX.Core.LearnEnvironment;

namespace PKHeX.Core;

/// <summary>
/// Exposes information about how moves are learned in <see cref="GS"/>.
/// </summary>
public sealed class LearnSource2GS : ILearnSource, IEggSource
{
    public static readonly LearnSource2GS Instance = new();
    private static readonly PersonalTable Personal = PersonalTable.GS;
    private static readonly EggMoves2[] EggMoves = Legal.EggMovesGS;
    private static readonly Learnset[] Learnsets = Legal.LevelUpGS;
    private const int MaxSpecies = Legal.MaxSpeciesID_2;
    private const LearnEnvironment Game = GS;

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
        if (types.HasFlagFast(MoveSourceType.Machine) && GetIsTM(pi, move))
            return new(TMHM, Game);

        if (types.HasFlagFast(MoveSourceType.LevelUp))
        {
            var learn = GetLearnset(evo.Species, evo.Form);
            var level = learn.GetLevelLearnMove(move);
            if (level != -1 && evo.LevelMin <= level && level <= evo.LevelMax)
                return new(LevelUp, Game, (byte)level);
        }

        return default;
    }

    private static bool GetIsTM(PersonalInfo info, int move)
    {
        var index = Array.IndexOf(Legal.TMHM_GSC, move);
        if (index == -1)
            return false;
        return info.TMHM[index];
    }

    public IEnumerable<int> GetAllMoves(PKM pk, EvoCriteria evo, MoveSourceType types = MoveSourceType.All)
    {
        if (!TryGetPersonal(evo.Species, evo.Form, out var pi))
            yield break;

        if (types.HasFlagFast(MoveSourceType.LevelUp))
        {
            bool removeVC = pk.Format == 1 || pk.VC1;
            var learn = GetLearnset(evo.Species, evo.Form);
            var min = ParseSettings.AllowGen2MoveReminder(pk) ? 1 : evo.LevelMin;
            (bool hasMoves, int start, int end) = learn.GetMoveRange(min, evo.LevelMax);
            if (hasMoves)
            {
                var moves = learn.Moves;
                for (int i = end; i >= start; i--)
                {
                    var move = moves[i];
                    if (!removeVC || move < Legal.MaxMoveID_1)
                        yield return move;
                }
            }
        }

        if (types.HasFlagFast(MoveSourceType.Machine))
        {
            var permit = pi.TMHM;
            var moveIDs = Legal.TMHM_GSC;
            for (int i = 0; i < moveIDs.Length; i++)
            {
                if (permit[i])
                    yield return moveIDs[i];
            }
        }
    }

    public static void GetEncounterMoves(IEncounterTemplate enc, Span<int> init)
    {
        var species = enc.Species;
        var learn = Learnsets[species];
        learn.SetEncounterMoves(enc.LevelMin, init);
    }
}
