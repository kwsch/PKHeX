using System;
using System.Collections.Generic;
using System.Linq;

namespace PKHeX.Core;

/// <summary>
/// Object which stores information useful for analyzing a moveset relative to the encounter data.
/// </summary>
public sealed class ValidEncounterMoves
{
    public readonly IReadOnlyList<int>[] LevelUpMoves;
    public readonly IReadOnlyList<int>[] TMHMMoves = Empty;
    public readonly IReadOnlyList<int>[] TutorMoves = Empty;
    public int[] Relearn = Array.Empty<int>();

    private const int EmptyCount = PKX.Generation + 1; // one for each generation index (and 0th)
    private static readonly IReadOnlyList<int>[] Empty = Enumerable.Repeat((IReadOnlyList<int>)new List<int>(), EmptyCount).ToArray();

    public ValidEncounterMoves(PKM pk, IEncounterTemplate encounter, EvolutionHistory chains)
    {
        var level = MoveList.GetValidMovesAllGens(pk, chains, types: MoveSourceType.Encounter, RemoveTransferHM: false);

        int gen = encounter.Generation;
        if ((uint)gen < level.Length && level[gen] is List<int> x)
            AddEdgeCaseMoves(x, encounter, pk);

        LevelUpMoves = level;
        TMHMMoves = MoveList.GetValidMovesAllGens(pk, chains, types: MoveSourceType.AllMachines);
        TutorMoves = MoveList.GetValidMovesAllGens(pk, chains, types: MoveSourceType.AllTutors);
    }

    private static void AddEdgeCaseMoves(List<int> moves, IEncounterTemplate encounter, PKM pk)
    {
        if (pk is IBattleVersion {BattleVersion: not 0})
            return;

        switch (encounter)
        {
            case EncounterStatic8N r when r.IsDownLeveled(pk): // Downleveled Raid can happen for shared raids and self-hosted raids.
                moves.AddRange(MoveLevelUp.GetMovesLevelUp(pk, r.Species, r.Form, r.LevelMax, 0, 0, GameVersion.SW, false, 8));
                break;
            case EncounterSlot8GO g:
                moves.AddRange(MoveLevelUp.GetEncounterMoves(g.Species, g.Form, pk.Met_Level, g.OriginGroup));
                break;
        }
    }

    public ValidEncounterMoves(IReadOnlyList<int>[] levelup)
    {
        LevelUpMoves = levelup;
    }

    public ValidEncounterMoves() : this(Empty) { }
}
