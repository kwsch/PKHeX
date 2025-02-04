using System;
using System.Buffers;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace PKHeX.Core;

/// <summary>
/// Verifies whether a particular combination of Egg Moves is possible.
/// </summary>
public static class EggMoveVerifier
{
    private static readonly ConcurrentDictionary<(ushort Species, byte Form, GameVersion Version, ushort Move1, ushort Move2, ushort Move3, ushort Move4), bool> Lookup = new();

    /// <summary>
    /// Flags invalid Egg Move combinations in a Pokémon's current moveset.
    /// </summary>
    /// <param name="result">How each move was learned</param>
    /// <param name="current">Current moves</param>
    /// <param name="enc">Matched encounter</param>
    public static void FlagEggMoveCombination(Span<MoveResult> result, ReadOnlySpan<ushort> current, IEncounterTemplate enc)
    {
        if (enc is not EncounterEgg egg)
            return; // Did not hatch from an Egg
        if (enc.Generation >= 6)
            return; // Can inherit Egg Moves from either parent, so all possible combinations are legal
        
        ushort[] rent = ArrayPool<ushort>.Shared.Rent(4);
        var moves = rent.AsSpan(0, 4);
        int ctr = 0;
        for (var i = 0; i < result.Length; i++)
        {
            if (result[i].Info.Method == LearnMethod.EggMove)
                moves[ctr++] = current[i];
        }
        moves[..ctr].Sort(); // Treat different orderings identically when caching
        moves[ctr..].Clear();

        if (ctr > 0 && !IsPossible(moves, egg.Species, egg.Form, egg.Version))
        {
            // Mark as an unobtainable combination
            for (var i = 0; i < result.Length; i++)
            {
                if (result[i].Info.Method == LearnMethod.EggMove)
                    result[i] = MoveResult.UnobtainableEgg();
            }
        }
        ArrayPool<ushort>.Shared.Return(rent);
    }

    /// <summary>
    /// Checks if an Egg of the requested species and form can have all requested Egg Moves simultaneously.
    /// </summary>
    /// <param name="needs">Wanted moves</param>
    /// <param name="species">Requested species of the Egg</param>
    /// <param name="form">Requested form of the Egg</param>
    /// <param name="version">Version to check</param>
    /// <returns>Whether the combination is valid</returns>
    public static bool IsEggMoveCombinationValid(ReadOnlySpan<ushort> needs, ushort species, byte form, GameVersion version)
    {
        if (version.GetGeneration() >= 6)
            return true; // Can inherit Egg Moves from either parent, so all possible combinations are legal

        var eggMoves = GameData.GetLearnSource(version).GetEggMoves(species, form);
        ushort[] rent = ArrayPool<ushort>.Shared.Rent(4);
        var moves = rent.AsSpan(0, 4);
        int ctr = 0;
        foreach (var move in needs)
        {
            if (eggMoves.Contains(move))
                moves[ctr++] = move;
        }
        moves[..ctr].Sort(); // Treat different orderings identically when caching
        moves[ctr..].Clear();

        var result = (ctr == 0) || IsPossible(moves, species, form, version);
        ArrayPool<ushort>.Shared.Return(rent);
        return result;
    }

    private static bool IsPossible(ReadOnlySpan<ushort> moves, ushort species, byte form, GameVersion version)
    {
        if (Lookup.TryGetValue((species, form, version, moves[0], moves[1], moves[2], moves[3]), out bool found))
            return found;
        found = FindChain(moves, species, form, version, out _);
        Lookup.TryAdd((species, form, version, moves[0], moves[1], moves[2], moves[3]), found);
        return found;
    }

    private static bool FindChain(ReadOnlySpan<ushort> moves, ushort eggSpecies, byte eggForm, GameVersion version, out (ushort Species, byte Form)[]? chain)
    {
        chain = null;

        if (GameData.GetPersonal(version).GetFormEntry(eggSpecies, eggForm).Genderless)
            return false; // cannot inherit Egg Moves

        var nodes = CreateNodes(moves, version);
        if (MarkFathersAndCheckLevelUp(nodes, eggSpecies, eggForm, out chain))
            return true;
        if (CheckEncounters(nodes, moves, version, out chain))
            return true;
        return false;
    }

    /// <summary>
    /// Generates a null graph consisting of a <see cref="LearnNode"/> for each species and form.
    /// </summary>
    /// <param name="moves">Moves to check</param>
    /// <param name="version">Version to check</param>
    /// <returns>Generated graph</returns>
    private static LearnNode[][] CreateNodes(ReadOnlySpan<ushort> moves, GameVersion version)
    {
        var context = version.GetContext();
        var maxSpecies = GameUtil.GetMaxSpeciesID(version);
        var nodes = new LearnNode[maxSpecies + 1][];

        // Populate graph for every species/form
        var pt = GameData.GetPersonal(version);
        var source = GameData.GetLearnSource(version);
        for (ushort species = 1; species <= maxSpecies; species++)
        {
            byte formCount = pt.GetFormEntry(species, 0).FormCount;
            nodes[species] = new LearnNode[formCount];
            for (byte form = 0; form < formCount; form++)
            {
                var entry = pt.GetFormEntry(species, form);
                var levelUpMoves = source.GetLearnset(species, form).GetAllMoves();
                var eggMoves = source.GetEggMoves(species, form);
                nodes[species][form] = new LearnNode
                {
                    Gender = entry.Gender,
                    EggGroup1 = (EggGroup)entry.EggGroup1,
                    EggGroup2 = (EggGroup)entry.EggGroup2,
                    Moves = (byte)(Moveset.BitOverlap(levelUpMoves, moves) | (Moveset.BitOverlap(eggMoves, moves) << 4))
                };
            }
        }

        // Smeargle can pass down almost any move
        // Mark Smeargle as able to learn them by level-up
        var smeargle = nodes[(ushort)Species.Smeargle][0];
        for (int i = 0; i < moves.Length; i++)
        {
            if (MoveInfo.IsSketchValid(moves[i], context))
                smeargle.Moves |= (byte)(1 << i);
        }

        // Evolutions
        var forward = EvolutionTree.GetEvolutionTree(context).Forward;
        foreach (var (species, form, pre) in Iterate(nodes))
        {
            foreach (var (evoSpecies, evoForm) in forward.GetEvolutions(species, form))
            {
                if (evoForm >= nodes[evoSpecies].Length)
                    continue;
                var evo = nodes[evoSpecies][evoForm];

                // Evolved form can have all moves the pre-evolved form can learn
                evo.Moves |= pre.Moves;

                // Egg Group compatibility for baby Pokémon is determined by their evolved form
                if (pre.EggGroup1 == EggGroup.Undiscovered && evo.EggGroup1 != EggGroup.Undiscovered)
                {
                    pre.EggGroup1 = evo.EggGroup1;
                    pre.EggGroup2 = evo.EggGroup2;
                }
            }
        }
        return nodes;
    }

    /// <summary>
    /// Marks edges on the graph corresponding to possible father-child relationships that can pass on a wanted level-up move as a Egg Move.
    /// If a chain of fathers is found, it is returned immediately.
    /// </summary>
    /// <param name="nodes">Graph to search</param>
    /// <param name="eggSpecies">Species to start from</param>
    /// <param name="eggForm">Form to start from</param>
    /// <param name="chain">Chain of fathers</param>
    /// <returns>Whether a valid chain was found</returns>
    private static bool MarkFathersAndCheckLevelUp(LearnNode[][] nodes, ushort eggSpecies, byte eggForm, out (ushort Species, byte Form)[]? chain)
    {
        Queue<LearnEdge> q = new();
        var egg = nodes[eggSpecies][eggForm];
        egg.Distance = 1;
        q.Enqueue(new LearnEdge(eggSpecies, eggForm, (byte)(egg.Moves >> 4)));

        while (q.Count > 0)
        {
            var child = q.Dequeue();
            var childNode = nodes[child.Species][child.Form];
            foreach (var (species, form, father) in Iterate(nodes))
            {
                if (father.Distance == 0 && father.CanFather(childNode))
                {
                    var levelFlags = father.Moves & 0x0F;
                    if ((child.Flags & levelFlags) == child.Flags)
                    {
                        // Can learn all needed moves for the child through level-up, start of chain found
                        father.Distance = (byte)(childNode.Distance + 1);
                        father.Child = child;
                        MakeChain(nodes, species, form, out chain);
                        return true;
                    }
                    var eggFlags = father.Moves >> 4;
                    if ((child.Flags & (eggFlags | levelFlags)) == child.Flags)
                    {
                        // Can learn all needed moves for the child, but needs to inherit some Egg Moves still
                        father.Distance = (byte)(childNode.Distance + 1);
                        father.Child = child;
                        q.Enqueue(new LearnEdge(species, form, (byte)eggFlags));
                    }
                }
            }
        }
        chain = null;
        return false; // no father found
    }

    /// <summary>
    /// Check encounters for possible fathers to see if they can learn needed Egg Moves in some other way besides level up (transfer, static, etc.).
    /// </summary>
    /// <param name="nodes">Graph to search</param>
    /// <param name="moves">Moves to check</param>
    /// <param name="version">Version to check</param>
    /// <param name="chain">Chain of fathers</param>
    /// <returns>Whether a valid chain was found</returns>
    private static bool CheckEncounters(LearnNode[][] nodes, ReadOnlySpan<ushort> moves, GameVersion version, out (ushort Species, byte Form)[]? chain)
    {
        PKM template = version.GetGeneration() switch
        {
            1 => new PK1() { Gender = 0 },
            2 => new PK2() { Gender = 0 },
            3 => new PK3() { Gender = 0 },
            4 => new PK4() { Gender = 0 },
            5 => new PK5() { Gender = 0 },
            _ => throw new ArgumentOutOfRangeException(nameof(version)),
        };
        ushort[] needs = moves.ToArray();
        foreach (var (species, form, father) in Iterate(nodes))
        {
            if (nodes[species][form].Distance == 0)
                continue; // never visited, cannot pass on any Egg Moves to the target

            // This species could be the father if there's another way to get the needed Egg Moves besides breeding.
            // Look for any encounter that can learn this move.
            for (var i = 0; i < moves.Length; i++)
                needs[i] = ((nodes[species][form].Moves >> (i + 4)) & 1) == 1 ? moves[i] : (ushort)0;

            template.Species = species;
            template.Form = form;
            var enc = EncounterMovesetGenerator.GenerateEncountersNonEgg(template, needs).FirstOrDefault();
            if (enc is not null)
            {
                MakeChain(nodes, species, form, out chain); // start of chain found
                return true;
            }
        }
        chain = null;
        return false;
    }

    /// <summary>
    /// Follows edges in the graph from the specified father back to the requested Egg, returning a chain of every father along the way.
    /// </summary>
    /// <param name="nodes">Graph to search</param>
    /// <param name="species">Species to start from</param>
    /// <param name="form">Form to start from</param>
    /// <param name="chain">Chain of fathers</param>
    private static void MakeChain(LearnNode[][] nodes, ushort species, byte form, out (ushort Species, byte Form)[] chain)
    {
        chain = new (ushort Species, byte Form)[nodes[species][form].Distance + 1];
        var cnt = 0;
        var node = nodes[species][form];
        while (node.Child is not null)
        {
            chain[cnt++] = (species, form);
            (species, form) = (node.Child.Species, node.Child.Form);
            node = nodes[species][form];
        }
        chain[cnt] = (species, form);

    }

    private record LearnNode
    {
        public byte Distance;
        public byte Gender;
        public EggGroup EggGroup1;
        public EggGroup EggGroup2;

        /// <summary>
        /// Bitfield containing whether each needed move can be learned by level up or as an Egg Move.
        /// </summary>
        /// <remarks>
        /// Lower half is level-up moves, upper half is Egg Moves.
        /// </remarks>
        public byte Moves;

        public LearnEdge? Child = null;
    }
    private record LearnEdge(ushort Species, byte Form, byte Flags);

    /// <summary>
    /// Iterate through all species and forms in the graph.
    /// </summary>
    private static IEnumerable<(ushort Species, byte Form, LearnNode Node)> Iterate(this LearnNode[][] nodes)
    {
        for (ushort species = 1; species < nodes.Length; species++)
        {
            for (byte form = 0; form < nodes[species].Length; form++)
            {
                yield return (species, form, nodes[species][form]);
            }
        }
    }

    /// <summary>
    /// Checks if this <see cref="LearnNode"/> can father the child node.
    /// </summary>
    /// <param name="father">Father node</param>
    /// <param name="child">Child node</param>
    /// <returns></returns>
    private static bool CanFather(this LearnNode father, LearnNode child)
    {
        if (father.Gender is PersonalInfo.RatioMagicFemale or PersonalInfo.RatioMagicGenderless)
            return false;
        return father.EggGroup1 == child.EggGroup1 || father.EggGroup1 == child.EggGroup2 || father.EggGroup2 == child.EggGroup1 || father.EggGroup2 == child.EggGroup2;
    }
}
