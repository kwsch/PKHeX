using System;

namespace PKHeX.Core;

/// <summary>
/// Group that checks the source of a move in <see cref="GameVersion.Gen8"/>.
/// </summary>
public sealed class LearnGroup8 : ILearnGroup
{
    public static readonly LearnGroup8 Instance = new();
    private const int Generation = 8;

    public ILearnGroup? GetPrevious(PKM pk, EvolutionHistory history, IEncounterTemplate enc, LearnOption option)
    {
        if (enc.Generation >= Generation)
            return null;
        if (option == LearnOption.Current && pk.IsOriginalMovesetDeleted())
            return null;
        if (history.HasVisitedLGPE)
            return LearnGroup7b.Instance;
        return LearnGroup7.Instance;
    }

    public bool HasVisited(PKM pk, EvolutionHistory history) => history.HasVisitedSWSH;

    public bool Check(Span<MoveResult> result, ReadOnlySpan<int> current, PKM pk, EvolutionHistory history,
        IEncounterTemplate enc, MoveSourceType types = MoveSourceType.All, LearnOption option = LearnOption.Current)
    {
        var evos = history.Gen8;
        if (evos.Length == 0)
            return false;

        for (var i = 0; i < evos.Length; i++)
            Check(result, current, pk, evos[i], i, types, option);

        if (enc is EncounterStatic8N r && r.IsDownLeveled(pk))
        {
            // If the encounter was reduced in level for the OT that joined the encounter, check for the original moveset range.
            var i = evos.Length - 1;
            var exist = evos[i];
            var original = exist with { LevelMax = r.LevelMax, LevelMin = exist.LevelMax };
            Check(result, current, pk, original, i, types & MoveSourceType.LevelUp);
        }

        CheckSharedMoves(result, current, evos[0]);

        if (option is not LearnOption.Current && pk.IsOriginalMovesetDeleted() && enc is EncounterEgg { Generation: Generation } egg)
            CheckEncounterMoves(result, current, egg);

        return MoveResult.AllParsed(result);
    }

    private static void CheckSharedMoves(Span<MoveResult> result, ReadOnlySpan<int> current, EvoCriteria evo)
    {
        var game = LearnSource8SWSH.Instance;
        var entry = PersonalTable.SWSH.GetFormEntry(evo.Species, evo.Form);
        var baseSpecies = entry.HatchSpecies;
        var baseForm = entry.HatchFormIndexEverstone;
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

    private static void CheckEncounterMoves(Span<MoveResult> result, ReadOnlySpan<int> current, EncounterEgg egg)
    {
        var game = LearnSource8SWSH.Instance;
        ReadOnlySpan<int> eggMoves = game.GetEggMoves(egg.Species, egg.Form);
        ReadOnlySpan<int> levelMoves = egg.CanInheritMoves
            ? game.GetLearnset(egg.Species, egg.Form).Moves
            : ReadOnlySpan<int>.Empty;

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

    private static void Check(Span<MoveResult> result, ReadOnlySpan<int> current, PKM pk, EvoCriteria evo, int stage, MoveSourceType type = MoveSourceType.All, LearnOption option = LearnOption.Current)
    {
        if (!FormChangeUtil.ShouldIterateForms(evo.Species, evo.Form, Generation, option))
        {
            CheckInternal(result, current, pk, evo, stage, type, option);
            return;
        }

        // Check all forms
        var inst = LearnSource8SWSH.Instance;
        if (!inst.TryGetPersonal(evo.Species, evo.Form, out var pi))
            return;

        var fc = pi.FormCount;
        for (int i = 0; i < fc; i++)
            CheckInternal(result, current, pk, evo with { Form = (byte)i }, stage, type, option);
    }

    private static void CheckInternal(Span<MoveResult> result, ReadOnlySpan<int> current, PKM pk, EvoCriteria evo, int stage, MoveSourceType type, LearnOption option)
    {
        var game = LearnSource8SWSH.Instance;
        if (!game.TryGetPersonal(evo.Species, evo.Form, out var pi))
            return;

        for (int i = result.Length - 1; i >= 0; i--)
        {
            if (result[i].Valid)
                continue;

            var move = current[i];
            var chk = game.GetCanLearn(pk, pi, evo, move, type, option);
            if (chk != default)
                result[i] = new(chk, (byte)stage, Generation);
        }
    }

    public void GetAllMoves(Span<bool> result, PKM pk, EvolutionHistory history, IEncounterTemplate enc, MoveSourceType types = MoveSourceType.All, LearnOption option = LearnOption.Current)
    {
        var evos = history.Gen8;
        if (types.HasFlagFast(MoveSourceType.Encounter) && enc.Generation == Generation)
        {
            FlagEncounterMoves(enc, result);
            if (enc is EncounterStatic8N r && r.IsDownLeveled(pk))
            {
                // If the encounter was reduced in level for the OT that joined the encounter, check for the original moveset range.
                var i = evos.Length - 1;
                var exist = evos[i];
                var original = exist with { LevelMax = r.LevelMax, LevelMin = exist.LevelMax };
                LearnSource8SWSH.Instance.GetAllMoves(result, pk, original, types);
            }
        }

        foreach (var evo in evos)
            GetAllMoves(result, pk, evo, types, option);
    }

    private static void GetAllMoves(Span<bool> result, PKM pk, EvoCriteria evo, MoveSourceType types, LearnOption option)
    {
        if (!FormChangeUtil.ShouldIterateForms(evo.Species, evo.Form, Generation, option))
        {
            GetAllMovesInternal(result, pk, evo, types);
            return;
        }

        // Check all forms
        var inst = LearnSource6AO.Instance;
        if (!inst.TryGetPersonal(evo.Species, evo.Form, out var pi))
            return;

        var fc = pi.FormCount;
        for (int i = 0; i < fc; i++)
            GetAllMovesInternal(result, pk, evo with { Form = (byte)i }, types);
    }

    private static void GetAllMovesInternal(Span<bool> result, PKM pk, EvoCriteria evo, MoveSourceType types)
    {
        LearnSource8SWSH.Instance.GetAllMoves(result, pk, evo, types);
    }

    private static void FlagEncounterMoves(IEncounterTemplate enc, Span<bool> result)
    {
        if (enc is IMoveset { Moves: int[] { Length: not 0 } m })
        {
            foreach (var move in m)
                result[move] = true;
        }
        if (enc is IRelearn { Relearn: int[] { Length: not 0 } r })
        {
            foreach (var move in r)
                result[move] = true;
        }
    }
}
