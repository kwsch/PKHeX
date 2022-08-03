using System;
using System.Diagnostics.CodeAnalysis;
using static PKHeX.Core.LearnMethod;
using static PKHeX.Core.LearnEnvironment;
using static PKHeX.Core.LearnSource2;

namespace PKHeX.Core;

/// <summary>
/// Exposes information about how moves are learned in <see cref="GS"/>.
/// </summary>
public sealed class LearnSource2GS : ILearnSource, IEggSource
{
    public static readonly LearnSource2GS Instance = new();
    private static readonly PersonalTable2 Personal = PersonalTable.GS;
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
        var index = Array.IndexOf(TMHM_GSC, move);
        if (index == -1)
            return false;
        return info.TMHM[index];
    }

    public void GetAllMoves(Span<bool> result, PKM pk, EvoCriteria evo, MoveSourceType types = MoveSourceType.All)
    {
        if (!TryGetPersonal(evo.Species, evo.Form, out var pi))
            return;

        if (types.HasFlagFast(MoveSourceType.LevelUp))
        {
            bool removeVC = pk.Format == 1 || pk.VC1;
            var learn = GetLearnset(evo.Species, evo.Form);
            var min = ParseSettings.AllowGen2MoveReminder(pk) ? 1 : evo.LevelMin;
            (bool hasMoves, int start, int end) = learn.GetMoveRange(evo.LevelMax, min);
            if (hasMoves)
            {
                var moves = learn.Moves;
                for (int i = end; i >= start; i--)
                {
                    var move = moves[i];
                    if (!removeVC || move < Legal.MaxMoveID_1)
                        result[move] = true;
                }
            }
        }

        if (types.HasFlagFast(MoveSourceType.Machine))
        {
            var flags = pi.TMHM;
            var moves = TMHM_GSC;
            for (int i = 0; i < moves.Length; i++)
            {
                if (flags[i])
                    result[moves[i]] = true;
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
