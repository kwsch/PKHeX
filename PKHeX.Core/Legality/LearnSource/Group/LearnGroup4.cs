using System;

namespace PKHeX.Core;

/// <summary>
/// Group that checks the source of a move in <see cref="GameVersion.Gen4"/>.
/// </summary>
public sealed class LearnGroup4 : ILearnGroup
{
    public static readonly LearnGroup4 Instance = new();
    private const byte Generation = 4;
    public ushort MaxMoveID => Legal.MaxMoveID_4;

    public ILearnGroup? GetPrevious(PKM pk, EvolutionHistory history, IEncounterTemplate enc, LearnOption option) => enc.Generation is Generation ? null : LearnGroup3.Instance;
    public bool HasVisited(PKM pk, EvolutionHistory history) => history.HasVisitedGen4;

    public bool Check(Span<MoveResult> result, ReadOnlySpan<ushort> current, PKM pk, EvolutionHistory history,
        IEncounterTemplate enc, MoveSourceType types = MoveSourceType.All, LearnOption option = LearnOption.Current)
    {
        var evos = history.Gen4;
        for (var i = 0; i < evos.Length; i++)
            Check(result, current, pk, evos[i], i);

        if (types.HasFlag(MoveSourceType.Encounter) && enc is EncounterEgg { Generation: Generation } egg)
            CheckEncounterMoves(result, current, egg);

        if (types.HasFlag(MoveSourceType.LevelUp) && enc.Species is (int)Species.Nincada && evos is [{ Species: (int)Species.Shedinja }, _])
            CheckNincadaMoves(result, current, evos[^1]);

        return MoveResult.AllParsed(result);
    }

    private static void CheckNincadaMoves(Span<MoveResult> result, ReadOnlySpan<ushort> current, EvoCriteria nincada)
    {
        if (MoveResult.AllParsed(result))
            return;

        // If a result is not valid, check to see if it is a Shedinja move.
        // Ninjask's Gen4 learnset is a superset of Gen3, so don't bother re-checking in that group.
        var shedinja = LearnSource4Pt.Instance;
        var moves = shedinja.GetLearnset((int)Species.Ninjask, 0);
        for (var i = 0; i < result.Length; i++)
        {
            if (result[i].Valid)
                continue;
            var move = current[i];
            if (move == 0)
                break;

            var level = moves.GetLevelLearnMove(move);
            if (level == -1 || !nincada.InsideLevelRange(level))
                continue;

            var info = new MoveLearnInfo(LearnMethod.ShedinjaEvo, LearnEnvironment.Pt, (byte)level);
            result[i] = new MoveResult(info, 0, Generation);
            break; // Can only have one Ninjask move.
        }
    }

    private static void CheckEncounterMoves(Span<MoveResult> result, ReadOnlySpan<ushort> current, EncounterEgg egg)
    {
        ILearnSource inst = egg.Version switch
        {
            // HG/SS
            <= GameVersion.SS => LearnSource4HGSS.Instance,
            GameVersion.Pt => LearnSource4Pt.Instance,
            _ => LearnSource4DP.Instance,
        };
        var eggMoves = inst.GetEggMoves(egg.Species, egg.Form);
        var levelMoves = inst.GetInheritMoves(egg.Species, egg.Form);

        for (var i = result.Length - 1; i >= 0; i--)
        {
            if (result[i].Valid)
                continue;
            var move = current[i];
            if (eggMoves.Contains(move))
                result[i] = new(LearnMethod.EggMove, inst.Environment);
            else if (levelMoves.Contains(move))
                result[i] = new(LearnMethod.InheritLevelUp, inst.Environment);
            else if (move is (int)Move.VoltTackle && egg.CanHaveVoltTackle)
                result[i] = new(LearnMethod.SpecialEgg, inst.Environment);
        }
    }

    private static void Check(Span<MoveResult> result, ReadOnlySpan<ushort> current, PKM pk, EvoCriteria evo, int stage)
    {
        if (evo.Species is not ((int)Species.Deoxys or (int)Species.Giratina or (int)Species.Shaymin))
        {
            CheckInternal(result, current, pk, evo, stage);
            return;
        }

        // Check all forms
        var inst = LearnSource4HGSS.Instance;
        if (!inst.TryGetPersonal(evo.Species, evo.Form, out var pi))
            return;

        var fc = pi.FormCount;
        for (int i = 0; i < fc; i++)
            CheckInternal(result, current, pk, evo with { Form = (byte)i }, stage);
    }

    private static void CheckInternal(Span<MoveResult> result, ReadOnlySpan<ushort> current, PKM pk, EvoCriteria evo, int stage)
    {
        var hgss = LearnSource4HGSS.Instance;
        var species = evo.Species;
        if (!hgss.TryGetPersonal(species, evo.Form, out var hgss_pi))
            return; // should never happen.

        var pt = LearnSource4Pt.Instance;
        if (!pt.TryGetPersonal(species, evo.Form, out var pt_pi))
            return; // should never happen.

        var dp = LearnSource4DP.Instance;

        for (int i = result.Length - 1; i >= 0; i--)
        {
            if (result[i].Valid)
                continue;

            // Level Up moves are different for each game, but others (TM/Tutor) are same (except Pt Defog).
            var move = current[i];
            var chk = hgss.GetCanLearn(pk, hgss_pi, evo, move);
            if (chk != default)
            {
                result[i] = new(chk, (byte)stage, Generation);
                continue;
            }

            chk = pt.GetCanLearn(pk, pt_pi, evo, move, MoveSourceType.LevelUp | MoveSourceType.AllMachines);
            if (chk != default)
            {
                result[i] = new(chk, (byte)stage, Generation);
                continue;
            }
            chk = dp.GetCanLearn(pk, pt_pi, evo, move, MoveSourceType.LevelUp);
            if (chk != default)
            {
                result[i] = new(chk, (byte)stage, Generation);
            }
        }
    }

    public void GetAllMoves(Span<bool> result, PKM pk, EvolutionHistory history, IEncounterTemplate enc, MoveSourceType types = MoveSourceType.All, LearnOption option = LearnOption.Current)
    {
        if (types.HasFlag(MoveSourceType.Encounter) && enc.Generation == Generation)
            FlagEncounterMoves(enc, result);

        var evos = history.Gen4;
        foreach (var evo in evos)
            GetAllMoves(result, pk, evo, types);

        if (evos is [{ Species: (int)Species.Shedinja }, _])
        {
            var shedinja = LearnSource4Pt.Instance;
            var moves = shedinja.GetLearnset((int)Species.Ninjask, 0);
            var span = moves.GetMoveRange(evos[0].LevelMax, 20);
            foreach (var move in span)
                result[move] = true;
        }
    }

    private static void GetAllMoves(Span<bool> result, PKM pk, EvoCriteria evo, MoveSourceType types)
    {
        LearnSource4HGSS.Instance.GetAllMoves(result, pk, evo, types);
        LearnSource4Pt.Instance.GetAllMoves(result, pk, evo, types & (MoveSourceType.LevelUp | MoveSourceType.AllMachines));
        LearnSource4DP.Instance.GetAllMoves(result, pk, evo, types & (MoveSourceType.LevelUp));
    }

    private static void FlagEncounterMoves(IEncounterTemplate enc, Span<bool> result)
    {
        if (enc is IMoveset { Moves: { HasMoves: true } x })
        {
            result[x.Move4] = true;
            result[x.Move3] = true;
            result[x.Move2] = true;
            result[x.Move1] = true;
        }
    }
}
