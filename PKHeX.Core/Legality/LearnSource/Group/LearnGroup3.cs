using System;

namespace PKHeX.Core;

/// <summary>
/// Group that checks the source of a move in <see cref="GameVersion.Gen3"/>.
/// </summary>
public sealed class LearnGroup3 : ILearnGroup
{
    public static readonly LearnGroup3 Instance = new();
    private const int Generation = 3;

    public ILearnGroup? GetPrevious(Span<MoveResult> result, PKM pk, EvolutionHistory history, IEncounterTemplate enc) => null; // Gen3 is the end of the line!
    public bool HasVisited(PKM pk, EvolutionHistory history) => history.Gen3.Length != 0;

    public bool Check(Span<MoveResult> result, ReadOnlySpan<int> current, PKM pk, EvolutionHistory history, IEncounterTemplate enc, LearnOption option = LearnOption.Current)
    {
        var evos = history.Gen3;
        for (var i = 0; i < evos.Length; i++)
            Check(result, current, pk, evos[i], i);

        if (enc is EncounterEgg { Generation: Generation } egg)
            CheckEncounterMoves(result, current, egg);

        return MoveResult.AllParsed(result);
    }

    private static void CheckEncounterMoves(Span<MoveResult> result, ReadOnlySpan<int> current, EncounterEgg egg)
    {
        ReadOnlySpan<int> eggMoves, levelMoves;
        if (egg.Version is GameVersion.C)
        {
            var inst = LearnSource2C.Instance;
            eggMoves = inst.GetEggMoves(egg.Species, egg.Form);
            levelMoves = egg.CanInheritMoves
                ? inst.GetLearnset(egg.Species, egg.Form).GetAllMoves()
                : ReadOnlySpan<int>.Empty;
        }
        else
        {
            var inst = LearnSource2GS.Instance;
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
        var rs = LearnSource3RS.Instance;
        var species = evo.Species;
        if (!rs.TryGetPersonal(species, evo.Form, out var rp))
            return; // should never happen.

        var e = LearnSource3E.Instance;
        var fr = LearnSource3FR.Instance;
        var lg = LearnSource3LG.Instance;
        var ep = e[species];
        var fp = fr[species];
        var lp = lg[species];

        for (int i = result.Length - 1; i >= 0; i--)
        {
            if (result[i].Valid)
                continue;

            // Level Up moves are different for each game, but TM/HM is shared (use Emerald).
            var move = current[i];
            var chk = e.GetCanLearn(pk, ep, evo, move);
            if (chk != default)
            {
                result[i] = new(chk, (byte)stage, Generation);
                continue;
            }
            chk = rs.GetCanLearn(pk, rp, evo, move, MoveSourceType.LevelUp | MoveSourceType.AllTutors);
            if (chk != default)
            {
                result[i] = new(chk, (byte)stage, Generation);
                continue;
            }
            chk = fr.GetCanLearn(pk, fp, evo, move, MoveSourceType.LevelUp | MoveSourceType.AllTutors);
            if (chk != default)
            {
                result[i] = new(chk, (byte)stage, Generation);
                continue;
            }
            chk = lg.GetCanLearn(pk, lp, evo, move, MoveSourceType.LevelUp); // Tutors same as FR
            if (chk != default)
                result[i] = new(chk, (byte)stage, Generation);
        }
    }
}
