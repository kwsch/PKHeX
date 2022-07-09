using System;

namespace PKHeX.Core;

/// <summary>
/// Group that checks the source of a move in <see cref="GameVersion.Gen1"/>.
/// </summary>
public sealed class LearnGroup1 : ILearnGroup
{
    public static readonly LearnGroup1 Instance = new();
    private const int Generation = 2;

    public ILearnGroup? GetPrevious(Span<MoveResult> result, PKM pk, EvolutionHistory history, IEncounterTemplate enc)
    {
        if (enc.Generation == 1 && history.Gen2.Length != 0)
            return LearnGroup2.Instance;
        return null;
    }

    public bool HasVisited(PKM pk, EvolutionHistory history) => history.Gen1.Length != 0;

    public bool Check(Span<MoveResult> result, ReadOnlySpan<int> current, PKM pk, EvolutionHistory history, IEncounterTemplate enc)
    {
        if (enc.Generation == 1 && enc is not IMoveset)
            CheckEncounterMoves(result, current, enc);

        var evos = history.Gen1;
        for (var i = 0; i < evos.Length; i++)
            Check(result, current, pk, evos[i], i);

        return MoveResult.AllParsed(result);
    }

    private static void CheckEncounterMoves(Span<MoveResult> result, ReadOnlySpan<int> current, IEncounterTemplate enc)
    {
        Span<int> moves = stackalloc int[4];
        if (enc.Version is GameVersion.YW or GameVersion.RBY)
            LearnSource1YW.Instance.GetEncounterMoves(enc, moves);
        else
            LearnSource1RB.Instance.GetEncounterMoves(enc, moves);

        LearnVerifierHistory.MarkInitialMoves(result, current, moves);
    }

    private static void Check(Span<MoveResult> result, ReadOnlySpan<int> current, PKM pk, EvoCriteria evo, int stage)
    {
        var rb = LearnSource1RB.Instance;
        if (!rb.TryGetPersonal(evo.Species, evo.Form, out var rp))
            return; // should never happen.

        var yw = LearnSource1YW.Instance;
        if (!yw.TryGetPersonal(evo.Species, evo.Form, out var yp))
            return; // should never happen.

        for (int i = result.Length - 1; i >= 0; i--)
        {
            if (result[i].Valid)
                continue;

            var move = current[i];
            var chk = yw.GetCanLearn(pk, yp, evo, move);
            if (chk != default)
            {
                result[i] = new(chk, (byte)stage, Generation);
                continue;
            }
            chk = rb.GetCanLearn(pk, rp, evo, move);
            if (chk != default)
                result[i] = new(chk, (byte)stage, Generation);
        }
    }
}
