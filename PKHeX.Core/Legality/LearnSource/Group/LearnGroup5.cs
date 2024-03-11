using System;

namespace PKHeX.Core;

/// <summary>
/// Group that checks the source of a move in <see cref="GameVersion.Gen5"/>.
/// </summary>
public sealed class LearnGroup5 : ILearnGroup
{
    public static readonly LearnGroup5 Instance = new();
    private const byte Generation = 5;
    public ushort MaxMoveID => Legal.MaxMoveID_5;

    public ILearnGroup? GetPrevious(PKM pk, EvolutionHistory history, IEncounterTemplate enc, LearnOption option) => enc.Generation is Generation ? null : LearnGroup4.Instance;
    public bool HasVisited(PKM pk, EvolutionHistory history) => history.HasVisitedGen5;

    public bool Check(Span<MoveResult> result, ReadOnlySpan<ushort> current, PKM pk, EvolutionHistory history,
        IEncounterTemplate enc, MoveSourceType types = MoveSourceType.All, LearnOption option = LearnOption.Current)
    {
        var evos = history.Gen5;
        for (var i = 0; i < evos.Length; i++)
            Check(result, current, pk, evos[i], i, types, option);

        if (types.HasFlag(MoveSourceType.Encounter) && enc is EncounterEgg { Generation: Generation } egg)
            CheckEncounterMoves(result, current, egg);

        return MoveResult.AllParsed(result);
    }

    private static void CheckEncounterMoves(Span<MoveResult> result, ReadOnlySpan<ushort> current, EncounterEgg egg)
    {
        ILearnSource inst = egg.Version > GameVersion.B ? LearnSource5B2W2.Instance : LearnSource5BW.Instance;
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
            else if (move is (ushort)Move.VoltTackle && egg.CanHaveVoltTackle)
                result[i] = new(LearnMethod.SpecialEgg, inst.Environment);
        }
    }

    private static void Check(Span<MoveResult> result, ReadOnlySpan<ushort> current, PKM pk, EvoCriteria evo, int stage,
        MoveSourceType types, LearnOption option)
    {
        if (evo.Species is not ((int)Species.Deoxys or (int)Species.Giratina or (int)Species.Shaymin))
        {
            CheckInternal(result, current, pk, evo, stage, types, option);
            return;
        }

        // Check all forms
        var inst = LearnSource5B2W2.Instance;
        if (!inst.TryGetPersonal(evo.Species, evo.Form, out var pi))
            return;

        var fc = pi.FormCount;
        for (int i = 0; i < fc; i++)
            CheckInternal(result, current, pk, evo with {Form = (byte)i}, stage, types, option);
    }

    private static void CheckInternal(Span<MoveResult> result, ReadOnlySpan<ushort> current, PKM pk, EvoCriteria evo, int stage, MoveSourceType types, LearnOption option)
    {
        var b2w2 = LearnSource5B2W2.Instance;
        var species = evo.Species;
        if (!b2w2.TryGetPersonal(species, evo.Form, out var b2w2_pi))
            return;

        // Some forms don't exist in B/W (Kyurem)
        var bw = LearnSource5BW.Instance;
        _ = bw.TryGetPersonal(species, evo.Form, out var bw_pi);

        for (int i = result.Length - 1; i >= 0; i--)
        {
            if (result[i].Valid)
                continue;

            // Level Up moves are different for each game, but others (TM/Tutor) are same.
            var move = current[i];
            var chk = b2w2.GetCanLearn(pk, b2w2_pi, evo, move, types, option);
            if (chk != default)
                result[i] = new(chk, (byte)stage, Generation);

            if (bw_pi is null)
                continue;

            // B2/W2 is the same besides some level up moves.
            chk = LearnSource5BW.Instance.GetCanLearn(pk, bw_pi, evo, move, types & MoveSourceType.LevelUp, option);
            if (chk != default)
                result[i] = new(chk, (byte)stage, Generation);
        }
    }

    public void GetAllMoves(Span<bool> result, PKM pk, EvolutionHistory history, IEncounterTemplate enc, MoveSourceType types = MoveSourceType.All, LearnOption option = LearnOption.Current)
    {
        if (types.HasFlag(MoveSourceType.Encounter) && enc.Generation == Generation)
            FlagEncounterMoves(enc, result);

        foreach (var evo in history.Gen5)
            GetAllMoves(result, pk, evo, types);
    }

    private static void GetAllMoves(Span<bool> result, PKM pk, EvoCriteria evo, MoveSourceType types)
    {
        if (evo.Species is not ((int)Species.Deoxys or (int)Species.Giratina or (int)Species.Shaymin))
        {
            LearnSource5B2W2.Instance.GetAllMoves(result, pk, evo, types);
            LearnSource5BW.Instance.GetAllMoves(result, pk, evo, types & MoveSourceType.LevelUp);
            return;
        }

        // Check all forms
        var inst = LearnSource5B2W2.Instance;
        if (!inst.TryGetPersonal(evo.Species, evo.Form, out var pi))
            return;

        // Above species have same level up moves on B/W & B2/W2; just check B2/W2.
        var fc = pi.FormCount;
        for (int i = 0; i < fc; i++)
            LearnSource5B2W2.Instance.GetAllMoves(result, pk, evo with { Form = (byte)i }, types);
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
