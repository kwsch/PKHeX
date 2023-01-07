using System;
using System.Diagnostics.CodeAnalysis;
using static PKHeX.Core.LearnMethod;
using static PKHeX.Core.LearnEnvironment;
using static PKHeX.Core.LearnSource2;

namespace PKHeX.Core;

/// <summary>
/// Exposes information about how moves are learned in <see cref="GS"/>.
/// </summary>
public sealed class LearnSource2GS : ILearnSource<PersonalInfo2>, IEggSource
{
    public static readonly LearnSource2GS Instance = new();
    private static readonly PersonalTable2 Personal = PersonalTable.GS;
    private static readonly EggMoves2[] EggMoves = Legal.EggMovesGS;
    private static readonly Learnset[] Learnsets = Legal.LevelUpGS;
    private const int MaxSpecies = Legal.MaxSpeciesID_2;
    private const LearnEnvironment Game = GS;

    public Learnset GetLearnset(ushort species, byte form) => Learnsets[species];

    public bool TryGetPersonal(ushort species, byte form, [NotNullWhen(true)] out PersonalInfo2? pi)
    {
        if (form is not 0 || species > MaxSpecies)
        {
            pi = null;
            return false;
        }
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

    public MoveLearnInfo GetCanLearn(PKM pk, PersonalInfo2 pi, EvoCriteria evo, ushort move, MoveSourceType types = MoveSourceType.All, LearnOption option = LearnOption.Current)
    {
        if (move > Legal.MaxMoveID_2) // byte
            return default;

        if (types.HasFlag(MoveSourceType.Machine) && GetIsTM(pi, (byte)move))
            return new(TMHM, Game);

        if (types.HasFlag(MoveSourceType.LevelUp))
        {
            var learn = Learnsets[evo.Species];
            var level = learn.GetLevelLearnMove(move);
            if (level != -1 && evo.LevelMin <= level && level <= evo.LevelMax)
                return new(LevelUp, Game, (byte)level);
        }

        return default;
    }

    private static bool GetIsTM(PersonalInfo2 info, byte move)
    {
        var index = TMHM_GSC.IndexOf(move);
        if (index == -1)
            return false;
        return info.GetIsLearnTM(index);
    }

    public void GetAllMoves(Span<bool> result, PKM pk, EvoCriteria evo, MoveSourceType types = MoveSourceType.All)
    {
        if (!TryGetPersonal(evo.Species, evo.Form, out var pi))
            return;

        bool removeVC = pk.Format == 1 || pk.VC1;
        if (types.HasFlag(MoveSourceType.LevelUp))
        {
            var learn = Learnsets[evo.Species];
            var min = ParseSettings.AllowGen2MoveReminder(pk) ? 1 : evo.LevelMin;
            (bool hasMoves, int start, int end) = learn.GetMoveRange(evo.LevelMax, min);
            if (hasMoves)
            {
                var moves = learn.Moves;
                for (int i = end; i >= start; i--)
                {
                    var move = moves[i];
                    if (!removeVC || move <= Legal.MaxMoveID_1)
                        result[move] = true;
                }
            }
        }

        if (types.HasFlag(MoveSourceType.Machine))
            pi.SetAllLearnTM(result, TMHM_GSC);
    }

    public static void GetEncounterMoves(IEncounterTemplate enc, Span<ushort> init)
    {
        var species = enc.Species;
        var learn = Learnsets[species];
        learn.SetEncounterMoves(enc.LevelMin, init);
    }
}
