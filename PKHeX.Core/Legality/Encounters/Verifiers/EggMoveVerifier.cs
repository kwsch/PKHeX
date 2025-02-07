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
        var graph = new LearnGraph(moves, version);
        found = graph.FindChain(moves, species, form, version, out var chain, out var enc);
#if DEBUG
        PrintChain(moves, species, form, version, chain, enc);
#endif
        Lookup.TryAdd((species, form, version, moves[0], moves[1], moves[2], moves[3]), found);
        return found;
    }

#if DEBUG
    private static void PrintChain(ReadOnlySpan<ushort> moves, ushort species, byte form, GameVersion version, (ushort Species, byte Form)[]? chain, IEncounterable? enc)
    {
        if (chain is null)
            System.Diagnostics.Debug.WriteLine($"Could not find a breeding chain for {(Species)species}-{form} in {version} with moves {string.Join(", ", Array.ConvertAll(moves.ToArray(), m => (Move)m))}");
        else
            System.Diagnostics.Debug.WriteLine($"Found a breeding chain: ({(enc is null ? "Encounter" : "LevelUp")}) {string.Join(", ", Array.ConvertAll(chain, p => $"{(Species)p.Species}-{p.Form}"))}");
    }
#endif

    /// <summary>
    /// Graph to check whether a particular combination of Egg Moves is possible.
    /// </summary>
    private class LearnGraph
    {
        private readonly LearnNode[] Nodes;
        private readonly LearnEdge[,] Edges; // adjacency matrix
        private readonly ushort MaxSpeciesID;
        private const int UNDISCOVERED = 1 << (int)EggGroup.Undiscovered;

        public LearnGraph(ReadOnlySpan<ushort> moves, GameVersion version)
        {
            // Count how many forms need to be in the graph
            MaxSpeciesID = GameUtil.GetMaxSpeciesID(version);
            var pt = GameData.GetPersonal(version);
            int count = 1; // reserved for species 0
            for (ushort species = 1; species <= MaxSpeciesID; species++)
                count += pt.GetFormEntry(species, 0).FormCount;
            Nodes = new LearnNode[count];
            Edges = new LearnEdge[count, count];

            // Initialize nodes, putting alternate forms at the end
            var source = GameData.GetLearnSource(version);
            var formIndex = MaxSpeciesID + 1;
            for (ushort species = 1; species <= MaxSpeciesID; species++)
            {
                int formCount = pt.GetFormEntry(species, 0).FormCount;
                InitializeNode(species, species, 0, moves, pt, source);
                for (byte form = 1; form < formCount; form++)
                    InitializeNode(formIndex++, species, form, moves, pt, source);
            }

            // Smeargle can pass down almost any move, mark them as level-up moves
            var context = version.GetContext();
            for (int i = 0; i < moves.Length; i++)
            {
                if (MoveInfo.IsSketchValid(moves[i], context))
                    Nodes[(int)Species.Smeargle].Moves |= (byte)(1 << i);
            }

            // Evolutions
            var reverse = EvolutionTree.GetEvolutionTree(context).Reverse;
            for (int index = 0; index < Nodes.Length; index++)
            {
                foreach (var (species, form) in reverse.GetPreEvolutions(Nodes[index].Species, Nodes[index].Form))
                {
                    int pre = GetIndex(species, form);
                    if (pre == -1)
                        continue;

                    // Can evolve to get a level-up move, and pass it down as an Egg Move to make a father of the same species
                    var flags = Nodes[pre].EggMoves() & Nodes[index].LevelUpMoves();
                    if (flags != 0)
                    {
                        Nodes[pre].Moves |= (byte)flags; // add to level-up moves
                        Nodes[pre].Moves &= (byte)(~(flags << 4)); // remove from Egg Moves
                    }

                    // Egg Group compatibility for baby Pokémon is determined by their evolved form
                    if (Nodes[pre].EggGroups == UNDISCOVERED && Nodes[index].EggGroups != UNDISCOVERED)
                        Nodes[pre].EggGroups = Nodes[index].EggGroups;
                }
            }

            var forward = EvolutionTree.GetEvolutionTree(context).Forward;
            for (int index = 0; index < Nodes.Length; index++)
            {
                foreach (var (species, form) in forward.GetEvolutions(Nodes[index].Species, Nodes[index].Form))
                {
                    int evo = GetIndex(species, form);
                    if (evo == -1)
                        continue;

                    // Evolved form can have all moves the pre-evolved form can learn
                    Nodes[evo].Moves |= Nodes[index].Moves;
                }
            }
        }

        private void InitializeNode(int index, ushort species, byte form, ReadOnlySpan<ushort> moves, IPersonalTable pt, ILearnSource source)
        {
            var pi = pt.GetFormEntry(species, form);
            var levelUpMoves = source.GetLearnset(species, form).GetAllMoves();
            var eggMoves = source.GetEggMoves(species, form);
            Nodes[index].Species = species;
            Nodes[index].Form = form;
            Nodes[index].Gender = pi.Gender;
            Nodes[index].EggGroups = (ushort)((1 << pi.EggGroup1) | (1 << pi.EggGroup2));
            Nodes[index].Moves = (byte)(Moveset.BitOverlap(levelUpMoves, moves) | (Moveset.BitOverlap(eggMoves, moves) << 4));
        }

        /// <summary>
        /// Maps species ID and form ID to the corresponding internal index of the node within the graph.
        /// </summary>
        /// <param name="species">Entity species</param>
        /// <param name="form">Entity form</param>
        /// <returns>Index of the corresponding node, or 0 if no such node is found</returns>
        private ushort GetIndex(ushort species, byte form)
        {
            if (form == 0)
                return species;
            for (ushort i = (ushort)(MaxSpeciesID + 1); i < Nodes.Length; i++)
            {
                if (Nodes[i].Species == species && Nodes[i].Form == form)
                    return i;
            }
            return 0;
        }

        /// <summary>
        /// Search for the shortest chain of fathers who can learn all needed moves and pass them on to the target.
        /// </summary>
        /// <param name="moves">Moves to check</param>
        /// <param name="species">Entity species</param>
        /// <param name="form">Entity form</param>
        /// <param name="version">Version to check</param>
        /// <param name="chain">Chain of fathers, or null if no chain was found</param>
        /// <param name="enc">Possible encounter for the first father in the chain, or null if no chain was found or the move is learned by level-up</param>
        /// <returns>Whether a valid chain was found</returns>
        public bool FindChain(ReadOnlySpan<ushort> moves, ushort species, byte form, GameVersion version, out (ushort Species, byte Form)[]? chain, out IEncounterable? enc)
        {
            ushort index = GetIndex(species, form);
            enc = null;
            return FindChainLevelUp(index, out chain) || FindChainEncounter(moves, index, version, out chain, out enc);
        }

        /// <summary>
        /// Search for the shortest chain of fathers who can learn all needed moves via level-up and pass them on to the target.
        /// </summary>
        /// <param name="start">Index of start node</param>
        /// <param name="chain">Chain of fathers, or null if no chain was found</param>
        /// <returns>Whether a valid chain was found</returns>
        /// <remarks>
        /// This is implemented as a breadth-first search along all edges where the father can either learn by level-up or inherit all needed Egg Moves for the child.
        /// If the father can learn all needed moves for the child via level-up, it is immediately returned.
        /// Note that even if the father was explored before, we still need to check if it can learn the moves needed by the child we're checking, since the needed moves might be different.
        /// </remarks>
        private bool FindChainLevelUp(ushort start, out (ushort Species, byte Form)[]? chain)
        {
            Queue<(ushort Previous, ushort Child)> queue = new();
            queue.Enqueue((start, start));
            while (queue.Count > 0)
            {
                var (previous, child) = queue.Dequeue();
                var childNeeds = Nodes[child].EggMoves();
                Nodes[child].Visited = Visited.LevelUp;
                for (ushort father = 0; father < Nodes.Length; father++)
                {
                    if (CanBeFather(child, father))
                    {
                        var fatherHas = Nodes[father].LevelUpMoves();
                        if ((childNeeds & fatherHas) == childNeeds)
                        {
                            // The moves the child needs are a subset of what the father can learn through level-up, start of chain found
                            Edges[child, father].Previous = previous;
                            MakeChain(child, father, out chain);
                            return true;
                        }
                        var fatherNeeds = Nodes[father].EggMoves();
                        if ((childNeeds & (fatherHas | fatherNeeds)) == childNeeds)
                        {
                            // The moves the child needs are a subset of what the father can learn, but it needs to inherit some Egg Moves still
                            Edges[child, father].Previous = previous;
                            if (Nodes[father].Visited != Visited.LevelUp)
                                queue.Enqueue((child, father));
                        }
                    }
                }
            }

            // No match
            chain = null;
            return false;
        }

        /// <summary>
        /// Search for the shortest chain of fathers who can learn all needed moves and pass them on to the target,
        /// where the first father in the chain can only have a needed move as a special move or when transferred from another generation.
        /// </summary>
        /// <param name="moves">Moves to check</param>
        /// <param name="start">Index of start node</param>
        /// <param name="version">Version to check</param>
        /// <param name="chain">Chain of fathers, or null if no chain was found</param>
        /// <param name="enc">Possible encounter for the first father in the chain, or null if no chain was found</param>
        /// <returns>Whether a valid chain was found</returns>
        /// <remarks>
        /// <see cref="FindChainLevelUp"/> must have been run on the graph previously, with no match found.
        /// If the father can learn all needed moves for the child via level-up, it is immediately returned.
        /// Note that even if the father was explored before, we still need to check if it can learn the moves needed by the child we're checking, since the needed moves might be different.
        /// </remarks>
        private bool FindChainEncounter(ReadOnlySpan<ushort> moves, ushort start, GameVersion version, out (ushort Species, byte Form)[]? chain, out IEncounterable? enc)
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

            Queue<ushort> queue = new();
            queue.Enqueue(start);
            while (queue.Count > 0)
            {
                var child = queue.Dequeue();
                var childNeeds = Nodes[child].EggMoves();
                Nodes[child].Visited = Visited.Encounter;
                for (ushort father = 0; father < Nodes.Length; father++)
                {
                    if (Edges[child, father].Previous == 0)
                        continue; // cannot pass on any Egg Moves to the target (never followed this edge during the level-up search)

                    // Could be the father if there's another way to get the needed Egg Moves besides breeding.
                    // Look for any encounter that can learn these moves.
                    for (var i = 0; i < moves.Length; i++)
                        needs[i] = ((childNeeds >> i) & 1) == 1 ? moves[i] : (ushort)0;
                    template.Species = Nodes[father].Species;
                    template.Form = Nodes[father].Form;
                    enc = EncounterMovesetGenerator.GenerateEncountersNonEgg(template, needs).FirstOrDefault();
                    if (enc is not null)
                    {
                        // The moves the child needs are a subset of what the father can learn through this encounter, start of chain found
                        MakeChain(child, father, out chain);
                        return true;
                    }

                    // No matching encounter was found, but it could still be part of a chain that requires this father
                    if (Nodes[father].Visited != Visited.Encounter)
                        queue.Enqueue(father);
                }
            }

            // No match
            chain = null;
            enc = null;
            return false;
        }

        /// <summary>
        /// Follows edges in the graph from the specified father back to the requested Egg, returning a chain of every father along the way.
        /// </summary>
        /// <param name="child">Index of the child node for the starting edge</param>
        /// <param name="father">Index of the father node for the starting edge</param>
        /// <param name="chain">Chain of fathers</param>
        private void MakeChain(ushort child, ushort father, out (ushort Species, byte Form)[] chain)
        {
#if !DEBUG
            chain = [];
#endif
#if DEBUG
            int cnt = 2;
            for (var (c, f) = (child, father); c != Edges[c, f].Previous; cnt++)
                (c, f) = (Edges[c, f].Previous, c);

            chain = new (ushort Species, byte Form)[cnt];
            int i = 0;
            for (var (c, f) = (child, father); i < cnt; i++)
            {
                chain[i] = (Nodes[f].Species, Nodes[f].Form);
                (c, f) = (Edges[c, f].Previous, c);
            }
#endif
        }

        /// <summary>
        /// Checks whether the child node can have the father node as a father.
        /// </summary>
        /// <param name="child">Index of the child node</param>
        /// <param name="father">Index of the father node</param>
        /// <returns></returns>
        private bool CanBeFather(ushort child, ushort father)
        {
            if ((Nodes[father].EggGroups & Nodes[child].EggGroups) == 0)
                return false; // No shared Egg Groups
            if (Nodes[father].EggGroups == UNDISCOVERED)
                return false; // Cannot breed
            if (Nodes[father].Gender is PersonalInfo.RatioMagicFemale or PersonalInfo.RatioMagicGenderless)
                return false; // Cannot be male
            if (Nodes[child].Gender is PersonalInfo.RatioMagicMale && !Breeding.IsGenderSpeciesDetermination(Nodes[child].Species) && !SpeciesCategory.IsFixedGenderFromDual(Nodes[child].Species))
            {
                // Can only have itself or its evolutions as a father (breeding with Ditto)
                Species childSpecies = (Species)Nodes[child].Species;
                Species fatherSpecies = (Species)Nodes[father].Species;
                return (fatherSpecies == childSpecies) || childSpecies switch
                {
                    Species.Tyrogue => fatherSpecies is Species.Hitmonlee or Species.Hitmonchan or Species.Hitmontop,
                    Species.Rufflet => fatherSpecies is Species.Braviary,
                    _ => false,
                };
            }
            return true;
        }
    }

    /// <summary>
    /// Node in the graph, representing a particular species and form.
    /// </summary>
    private record struct LearnNode
    {
        public ushort Species;
        public byte Form;
        public byte Gender;

        /// <summary>
        /// Bitfield of flags for each Egg Group
        /// </summary>
        public ushort EggGroups;

        /// <summary>
        /// Bitfield containing whether each needed move can be learned by level up or as an Egg Move.
        /// </summary>
        /// <remarks>
        /// Lower half is level-up moves, upper half is Egg Moves.
        /// </remarks>
        public byte Moves;

        public Visited Visited;
    }

    /// <summary>
    /// Edge in the graph, representing a father-child relationship.
    /// </summary>
    private record struct LearnEdge
    {
        /// <summary>
        /// Grandchild of the father node of this edge, or child of the child node of this edge.
        /// </summary>
        /// <remarks>
        /// Used for tracing a specific path back to the start node.
        /// </remarks>
        public ushort Previous;
    }

    /// <summary>
    /// Used to mark if a node has been visited during a traversal of the graph.
    /// </summary>
    private enum Visited : byte
    {
        /// <summary>
        /// Not visited yet
        /// </summary>
        None = 0,

        /// <summary>
        /// Visited by <see cref="LearnGraph.FindChainLevelUp"/>
        /// </summary>
        LevelUp = 1,

        /// <summary>
        /// Visited by <see cref="LearnGraph.FindChainEncounter"/>
        /// </summary>
        Encounter = 2,
    }

    private static byte EggMoves(this LearnNode node) => (byte)(node.Moves >> 4);
    private static byte LevelUpMoves(this LearnNode node) => (byte)(node.Moves & 0x0F);
}
