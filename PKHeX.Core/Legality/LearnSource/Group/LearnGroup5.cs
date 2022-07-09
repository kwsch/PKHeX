using System;

namespace PKHeX.Core;

/// <summary>
/// Group that checks the source of a move in <see cref="GameVersion.Gen5"/>.
/// </summary>
public sealed class LearnGroup5 : ILearnGroup
{
    public static readonly LearnGroup5 Instance = new();
    private const int Generation = 5;

    public ILearnGroup? GetPrevious(Span<MoveResult> result, PKM pk, EvolutionHistory history, IEncounterTemplate enc) => enc.Generation is Generation ? null : LearnGroup4.Instance;
    public bool HasVisited(PKM pk, EvolutionHistory history) => history.Gen5.Length != 0;

    public bool Check(Span<MoveResult> result, ReadOnlySpan<int> current, PKM pk, EvolutionHistory history, IEncounterTemplate enc)
    {
        var evos = history.Gen5;
        for (var i = 0; i < evos.Length; i++)
            Check(result, current, pk, evos[i], i);

        if (enc is EncounterEgg { Generation: Generation } egg)
            CheckEncounterMoves(result, current, egg);

        return MoveResult.AllParsed(result);
    }

    private static void CheckEncounterMoves(Span<MoveResult> result, ReadOnlySpan<int> current, EncounterEgg egg)
    {
        ReadOnlySpan<int> eggMoves, levelMoves;
        if (egg.Version > GameVersion.B) // B2/W2
        {
            var inst = LearnSource5B2W2.Instance;
            eggMoves = inst.GetEggMoves(egg.Species, egg.Form);
            levelMoves = egg.CanInheritMoves
                ? inst.GetLearnset(egg.Species, egg.Form).GetAllMoves()
                : ReadOnlySpan<int>.Empty;
        }
        else
        {
            var inst = LearnSource5BW.Instance;
            eggMoves = inst.GetEggMoves(egg.Species, egg.Form);
            levelMoves = egg.CanInheritMoves
                ? inst.GetLearnset(egg.Species, egg.Form).GetAllMoves()
                : ReadOnlySpan<int>.Empty;
        }

        for (var i = result.Length - 1; i >= 0; i--)
        {
            if (result[i].Valid)
                continue;
            var move = current[i];
            if (eggMoves.Contains(move))
                result[i] = new(LearnMethod.EggMove);
            else if (levelMoves.Contains(move))
                result[i] = new(LearnMethod.InheritLevelUp);
            else if (move is (int)Move.VoltTackle && egg.CanHaveVoltTackle)
                result[i] = new(LearnMethod.SpecialEgg);
        }
    }

    private static void Check(Span<MoveResult> result, ReadOnlySpan<int> current, PKM pk, EvoCriteria evo, int stage)
    {
        var b2w2 = LearnSource5B2W2.Instance;
        var species = evo.Species;
        if (!b2w2.TryGetPersonal(species, evo.Form, out var b2w2_pi))
            return; // should never happen.

        var bw = LearnSource5BW.Instance;

        for (int i = result.Length - 1; i >= 0; i--)
        {
            if (result[i].Valid)
                continue;

            // Level Up moves are different for each game, but others (TM/Tutor) are same.
            var move = current[i];
            var chk = b2w2.GetCanLearn(pk, b2w2_pi, evo, move);
            if (chk != default)
            {
                result[i] = new(chk, (byte)stage, Generation);
                continue;
            }

            chk = bw.GetCanLearn(pk, b2w2_pi, evo, move, MoveSourceType.LevelUp);
            if (chk != default)
                result[i] = new(chk, (byte)stage, Generation);
        }
    }
}
