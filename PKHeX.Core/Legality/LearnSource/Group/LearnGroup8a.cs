using System;

namespace PKHeX.Core;

/// <summary>
/// Group that checks the source of a move in <see cref="GameVersion.PLA"/>.
/// </summary>
public sealed class LearnGroup8a : ILearnGroup
{
    public static readonly LearnGroup8a Instance = new();
    private const int Generation = 8;

    public ILearnGroup? GetPrevious(Span<MoveResult> result, PKM pk, EvolutionHistory history, IEncounterTemplate enc) => null;
    public bool HasVisited(PKM pk, EvolutionHistory history) => history.Gen8a.Length != 0;

    public bool Check(Span<MoveResult> result, ReadOnlySpan<int> current, PKM pk, EvolutionHistory history, IEncounterTemplate enc, LearnOption option = LearnOption.Current)
    {
        var evos = history.Gen8a;
        for (var i = 0; i < evos.Length; i++)
            Check(result, current, pk, evos[i], i, option);

        return MoveResult.AllParsed(result);
    }

    private static void Check(Span<MoveResult> result, ReadOnlySpan<int> current, PKM pk, EvoCriteria evo, int stage, LearnOption option)
    {
        var game = LearnSource8LA.Instance;
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
