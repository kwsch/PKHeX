using System;

namespace PKHeX.Core;

/// <summary>
/// Group that checks the source of a move in <see cref="GameVersion.BDSP"/>.
/// </summary>
public sealed class LearnGroup8b : ILearnGroup
{
    public static readonly LearnGroup8b Instance = new();
    private const int Generation = 8;

    public ILearnGroup? GetPrevious(Span<MoveResult> result, PKM pk, EvolutionHistory history, IEncounterTemplate enc) => null;
    public bool HasVisited(PKM pk, EvolutionHistory history) => history.Gen8b.Length != 0;

    public bool Check(Span<MoveResult> result, ReadOnlySpan<int> current, PKM pk, EvolutionHistory history, IEncounterTemplate enc, LearnOption option = LearnOption.Current)
    {
        var evos = history.Gen8b;
        for (var i = 0; i < evos.Length; i++)
            Check(result, current, pk, evos[i], i);

        CheckSharedMoves(result, current, pk);

        return MoveResult.AllParsed(result);
    }

    private static void CheckSharedMoves(Span<MoveResult> result, ReadOnlySpan<int> current, PKM pk)
    {
        var game = LearnSource8BDSP.Instance;
        var entry = (PersonalInfoBDSP)pk.PersonalInfo;
        var baseSpecies = entry.HatchSpecies;
        var baseForm = entry.HatchFormIndex;
        var eggMoves = game.GetEggMoves(baseSpecies, baseForm);

        for (var i = result.Length - 1; i >= 0; i--)
        {
            if (result[i].Valid)
                continue;
            var move = current[i];
            if (eggMoves.Contains(move))
                result[i] = new(LearnMethod.Shared);
        }
    }

    private static void Check(Span<MoveResult> result, ReadOnlySpan<int> current, PKM pk, EvoCriteria evo, int stage)
    {
        var game = LearnSource8BDSP.Instance;
        if (!game.TryGetPersonal(evo.Species, evo.Form, out var pi))
            return; // should never happen.

        for (int i = result.Length - 1; i >= 0; i--)
        {
            if (result[i].Valid)
                continue;

            var move = current[i];
            var chk = game.GetCanLearn(pk, pi, evo, move);
            if (chk != default)
                result[i] = new(chk, (byte)stage, Generation);
        }
    }
}
