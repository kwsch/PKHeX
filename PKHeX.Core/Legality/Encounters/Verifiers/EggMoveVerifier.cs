using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace PKHeX.Core;

/// <summary>
/// Verifies whether a particular combination of Egg Moves is possible.
/// </summary>
public static class EggMoveVerifier
{
    private static readonly ConcurrentDictionary<(ushort Species, GameVersion Version, ushort Move1, ushort Move2, ushort Move3, ushort Move4), bool> Lookup = [];

    /// <summary>
    /// Flags invalid Egg Move combinations in a Pokémon's current moveset.
    /// This includes inherited level-up moves for female-only species.
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

        // If Smeargle can be a direct father, all Egg Move combinations are possible.
        var pi = GameData.GetPersonal(egg.Version)[egg.Species];
        if (CanSmeargleBeFather(current, pi, egg.Version.GetContext()))
            return;

        bool checkInheritLevelUp = pi.OnlyFemale && !IsDualGenderParent(egg.Species);
        Span<ushort> moves = stackalloc ushort[4];
        int ctr = 0;
        for (int i = 0; i < result.Length; i++)
        {
            var method = result[i].Info.Method;
            if (method == LearnMethod.EggMove || (checkInheritLevelUp && method == LearnMethod.InheritLevelUp))
                moves[ctr++] = current[i];
        }
        if (ctr == 0)
            return; // none to flag.
        if (ctr == 1 && !checkInheritLevelUp)
            return;

        moves[..ctr].Sort(); // Treat different orderings identically when caching
        if (IsPossible(moves, egg.Species, egg.Version))
            return;

        // Mark as an unobtainable combination
        foreach (ref var m in result)
        {
            var method = m.Info.Method;
            if (method == LearnMethod.EggMove || (checkInheritLevelUp && method == LearnMethod.InheritLevelUp))
                m = MoveResult.UnobtainableEgg();
        }
    }

    /// <summary>
    /// Checks if an Egg of the requested species can have all requested Egg Moves in the moveset simultaneously.
    /// This includes inherited level-up moves for female-only species.
    /// </summary>
    /// <param name="needs">Wanted moves</param>
    /// <param name="species">Requested species of the Egg</param>
    /// <param name="version">Version to check</param>
    /// <returns>Whether the combination is valid</returns>
    public static bool IsEggMoveCombinationValid(ReadOnlySpan<ushort> needs, ushort species, GameVersion version)
    {
        if (version.GetGeneration() >= 6)
            return true; // Can inherit Egg Moves from either parent, so all possible combinations are legal

        // If Smeargle can be a direct father, all Egg Move combinations are possible.
        var pi = GameData.GetPersonal(version)[species];
        if (CanSmeargleBeFather(needs, pi, version.GetContext()))
            return true;

        var source = GameData.GetLearnSource(version);
        var eggMoves = source.GetEggMoves(species, 0);
        var learnset = source.GetLearnset(species, 0);
        var eggLevel = EggStateLegality.GetEggLevel(version.GetGeneration());
        var baseMoves = learnset.GetBaseEggMoves(eggLevel);
        var levelUpMoves = learnset.GetAllMoves();

        bool checkInheritLevelUp = pi.OnlyFemale && !IsDualGenderParent(species);
        Span<ushort> moves = stackalloc ushort[4];
        int ctr = 0;
        foreach (var move in needs)
        {
            if (eggMoves.Contains(move) || (checkInheritLevelUp && levelUpMoves.Contains(move) && !baseMoves.Contains(move)))
                moves[ctr++] = move;
        }
        if (ctr == 0)
            return true;
        if (ctr == 1 && !checkInheritLevelUp)
            return true;

        moves[..ctr].Sort(); // Treat different orderings identically when caching
        if (IsPossible(moves, species, version))
            return true;
        return false;
    }

    private static bool CanSmeargleBeFather(ReadOnlySpan<ushort> moves, PersonalInfo pi, EntityContext context)
    {
        if ((pi.EggGroup1 != (int)EggGroup.Field && pi.EggGroup2 != (int)EggGroup.Field) || pi.OnlyMale || pi.Genderless)
            return false;

        foreach (var move in moves)
        {
            if (!MoveInfo.IsSketchValid(move, context))
                return false;
        }
        return true;
    }

    private static bool IsDualGenderParent(ushort species) => Breeding.IsGenderSpeciesDetermination(species) || SpeciesCategory.IsFixedGenderFromDual(species);

    /// <summary>
    /// Checks if an Egg of the requested species can inherit all requested moves simultaneously.
    /// </summary>
    /// <param name="moves">Wanted Egg Moves (or inherited level-up moves)</param>
    /// <param name="species">Requested species of the Egg</param>
    /// <param name="version">Version to check</param>
    /// <returns>Whether the combination is valid</returns>
    public static bool IsPossible(ReadOnlySpan<ushort> moves, ushort species, GameVersion version)
    {
        if (Lookup.TryGetValue((species, version, moves[0], moves[1], moves[2], moves[3]), out bool found))
            return found;
        found = IsPossible(moves, species, version, out _, out _);
        Lookup.TryAdd((species, version, moves[0], moves[1], moves[2], moves[3]), found);
        return found;
    }

    /// <inheritdoc cref="IsPossible(ReadOnlySpan{ushort}, ushort, GameVersion)"/>
    /// <param name="version"></param>
    /// <param name="chain">Chain of fathers, or null if no chain was found</param>
    /// <param name="enc">Possible encounter for the first father in the chain, or null if no chain was found</param>
    /// <param name="moves"></param>
    /// <param name="species"></param>
    public static bool IsPossible(ReadOnlySpan<ushort> moves, ushort species, GameVersion version, out ushort[]? chain, out IEncounterTemplate? enc)
    {
        var graph = new LearnGraph(moves, version);
        var found = graph.FindChain(moves, species, version, out chain, out enc);
#if DEBUG
        PrintChain(moves, species, version, chain, enc);
#endif
        return found;
    }

#if DEBUG
    private static void PrintChain(ReadOnlySpan<ushort> moves, ushort species, GameVersion version, ushort[]? chain, IEncounterTemplate? enc)
    {
        if (chain is null)
            System.Diagnostics.Debug.WriteLine($"Could not find a breeding chain for {(Species)species} in {version} with moves {string.Join(", ", moves.ToArray().Select(m => (Move)m))}");
        else
            System.Diagnostics.Debug.WriteLine($"Found a breeding chain for {(Species)species} in {version}: ({(enc is null ? "LevelUp" : "Encounter")}) {string.Join(", ", Array.ConvertAll(chain, p => $"{(Species)p}"))}");
    }
#endif

    /// <summary>
    /// Graph to check whether a particular combination of Egg Moves is possible.
    /// </summary>
    private sealed class LearnGraph
    {
        private readonly LearnNode[] Nodes;
        private readonly Dictionary<LearnEdge, LearnEdge> Edges;
        private const int UNDISCOVERED = 1 << (int)EggGroup.Undiscovered;

        public LearnGraph(ReadOnlySpan<ushort> moves, GameVersion version)
        {
            // Initialize nodes
            var maxSpeciesID = version.GetMaxSpeciesID();
            var pt = GameData.GetPersonal(version);
            var source = GameData.GetLearnSource(version);
            var eggLevel = EggStateLegality.GetEggLevel(version.GetGeneration());
            Nodes = new LearnNode[maxSpeciesID + 1]; // reserve 0 for species 0
            for (ushort species = 1; species <= maxSpeciesID; species++)
                InitializeNode(species, moves, eggLevel, pt, source);
            Edges = [];

            // Smeargle can pass down almost any move, mark them as level-up moves
            var context = version.GetContext();
            for (int i = 0; i < moves.Length; i++)
            {
                if (MoveInfo.IsSketchValid(moves[i], context))
                    Nodes[(int)Species.Smeargle].Moves |= (byte)(1 << i);
            }

            // Evolutions
            var reverse = EvolutionTree.GetEvolutionTree(context).Reverse;
            for (ushort evo = 0; evo < Nodes.Length; evo++)
            {
                var ne = Nodes[evo];
                foreach (var (pre, _) in reverse.GetPreEvolutions(ne.Species, 0))
                {
                    var np = Nodes[pre];
                    // Egg Group compatibility for baby Pokémon is determined by their evolved form
                    if (np.EggGroups == UNDISCOVERED && ne.EggGroups != UNDISCOVERED)
                        Nodes[pre].EggGroups = ne.EggGroups;

                    // Can evolve to get a level-up move, and pass it down as an Egg Move to make a father of the same species
                    if (!CanBeFather(pre, evo))
                        continue;

                    var flags = np.EggMoves() & ne.LevelUpMoves();
                    if (flags != 0)
                    {
                        Nodes[pre].Moves |= (byte)flags; // add to level-up moves
                        Nodes[pre].Moves &= (byte)(~(flags << 4)); // remove from Egg Moves
                    }
                }
            }

            var forward = EvolutionTree.GetEvolutionTree(context).Forward;
            for (ushort pre = 0; pre < Nodes.Length; pre++)
            {
                var np = Nodes[pre];
                foreach (var (evo, _) in forward.GetEvolutions(np.Species, 0))
                {
                    // Evolved form can have all moves the pre-evolved form can learn
                    Nodes[evo].Moves |= np.Moves;
                }
            }
        }

        private void InitializeNode(ushort species, ReadOnlySpan<ushort> moves, byte eggLevel, IPersonalTable pt, ILearnSource source)
        {
            // In Gen 2-5, no species has a different gender ratio or Egg Groups between forms.
            // Only Wormadam has a different learnset between forms, but it can't be a father to pass on any moves it learns and Burmy has no Egg Moves.
            const byte form = 0;
            var pi = pt.GetFormEntry(species, form);
            var eggMoves = source.GetEggMoves(species, form);
            var learnset = source.GetLearnset(species, form);
            var levelUpMoves = learnset.GetAllMoves();

            var childHas = Moveset.BitOverlap(levelUpMoves, moves);
            var childNeeds = Moveset.BitOverlap(eggMoves, moves);

            // For species that can be both genders, you can breed parents with both the needed Egg Moves and level-up moves.
            // For female-only species, you can't do this, so we also need to make sure that the father can learn any needed inherited level-up moves.
            if (pi.OnlyFemale && !Breeding.IsGenderSpeciesDetermination(species) && !SpeciesCategory.IsFixedGenderFromDual(species))
            {
                var baseMoves = learnset.GetBaseEggMoves(eggLevel);
                childHas = Moveset.BitOverlap(baseMoves, moves);
                childNeeds |= Moveset.BitOverlap(levelUpMoves, moves) & ~childHas;
            }

            Nodes[species] = new LearnNode
            {
                Species = species,
                Gender = pi.Gender,
                EggGroups = (ushort)((1 << pi.EggGroup1) | (1 << pi.EggGroup2)),
                Moves = (byte)(childHas | (childNeeds << 4)),
            };
        }

        /// <summary>
        /// Search for the shortest chain of fathers who can learn all needed moves and pass them on to the target.
        /// </summary>
        /// <param name="moves">Moves to check</param>
        /// <param name="species">Entity species</param>
        /// <param name="version">Version to check</param>
        /// <param name="chain">Chain of fathers, or null if no chain was found</param>
        /// <param name="enc">Possible encounter for the first father in the chain, or null if no chain was found or the move is learned by level-up</param>
        /// <returns>Whether a valid chain was found</returns>
        public bool FindChain(ReadOnlySpan<ushort> moves, ushort species, GameVersion version, out ushort[]? chain, out IEncounterTemplate? enc)
        {
            enc = null;
            return FindChainLevelUp(species, out chain) || FindChainEncounter(moves, species, version, out chain, out enc);
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
        /// Note that the same node or edge can be reached multiple times, since the needed moves for the child might be different.
        /// </remarks>
        private bool FindChainLevelUp(ushort start, out ushort[]? chain)
        {
            var startEdge = new LearnEdge(start, start, Nodes[start].EggMoves());
            Edges.Add(startEdge, startEdge);
            if (startEdge.Needs == 0)
            {
                MakeChain(startEdge, out chain);
                return true;
            }

            Queue<LearnEdge> queue = [];
            queue.Enqueue(startEdge);
            while (queue.Count > 0)
            {
                var previousEdge = queue.Dequeue();
                var (_, child, childNeeds) = previousEdge;
                for (ushort father = 0; father < Nodes.Length; father++)
                {
                    if (father == child || !CanBeFather(child, father))
                        continue;

                    var edge = new LearnEdge(child, father, childNeeds);
                    var nf = Nodes[father];

                    var fatherHas = nf.LevelUpMoves();
                    if ((childNeeds & fatherHas) == childNeeds)
                    {
                        // The moves the child needs are a subset of what the father can learn through level-up, start of chain found
                        edge.Needs = 0;
                        Edges.Add(edge, previousEdge);
                        MakeChain(edge, out chain);
                        return true;
                    }

                    var fatherNeeds = nf.EggMoves();
                    if ((childNeeds & (fatherHas | fatherNeeds)) == childNeeds)
                    {
                        // The moves the child needs are a subset of what the father can learn, but it needs to inherit some Egg Moves still
                        edge.Needs = (byte)(childNeeds & fatherNeeds);
                        if (!Edges.ContainsKey(edge))
                            queue.Enqueue(edge);
                    }

                    // Mark this edge as visited
                    Edges.TryAdd(edge, previousEdge);
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
        /// Note that the same node or edge can be reached multiple times, since the needed moves for the child might be different.
        /// </remarks>
        private bool FindChainEncounter(ReadOnlySpan<ushort> moves, ushort start, GameVersion version, out ushort[]? chain, out IEncounterTemplate? enc)
        {
            HashSet<(ushort Father, byte Needs)> generated = [];
            PKM template = version.GetGeneration() switch
            {
                2 => new PK2 { Gender = 0 },
                3 => new PK3 { Gender = 0 },
                4 => new PK4 { Gender = 0 },
                5 => new PK5 { Gender = 0 },
                _ => throw new ArgumentOutOfRangeException(nameof(version)),
            };
            var needs = moves.ToArray();

            var startEdge = new LearnEdge(start, start, Nodes[start].EggMoves());
            HashSet<LearnEdge> visited = [];
            Queue<LearnEdge> queue = [];
            queue.Enqueue(startEdge);
            while (queue.Count > 0)
            {
                var previousEdge = queue.Dequeue();
                foreach (var (edge, _) in Edges.Where(pair => pair.Value == previousEdge))
                {
                    // Could be the father if there's another way to get the needed Egg Moves besides breeding.
                    // Look for any encounter that can learn these moves.
                    var (child, father, childNeeds) = edge;
                    if (generated.Add((father, childNeeds)) && (father != child || CanBeFather(child, father)))
                    {
                        for (var i = 0; i < moves.Length; i++)
                            needs[i] = ((childNeeds >> i) & 1) == 1 ? moves[i] : (ushort)0;

                        var nf = Nodes[father];
                        template.Species = nf.Species;

                        enc = EncounterMovesetGenerator.GenerateEncountersNonEgg(template, needs).FirstOrDefault();
                        if (enc is not null)
                        {
                            // The moves the child needs are a subset of what the father can learn through this encounter, start of chain found
                            MakeChain(edge, out chain);
                            return true;
                        }
                    }

                    // Still need to check the parents of this edge
                    // Mark this edge as visited
                    if (visited.Add(edge))
                        queue.Enqueue(edge);
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
        /// <param name="edge">Final edge</param>
        /// <param name="chain">Chain of fathers</param>
        private void MakeChain(LearnEdge edge, out ushort[]? chain)
        {
            int cnt = 1;
            for (var e = edge; e != Edges[e]; cnt++)
                e = Edges[e];

            chain = new ushort[cnt];
            int i = 0;
            for (var e = edge; i < cnt; i++)
            {
                chain[i] = Nodes[e.Father].Species;
                e = Edges[e];
            }
        }

        /// <summary>
        /// Checks whether the child node can have the father node as a father.
        /// </summary>
        /// <param name="child">Index of the child node</param>
        /// <param name="father">Index of the father node</param>
        private bool CanBeFather(ushort child, ushort father)
        {
            var nf = Nodes[father];
            var nc = Nodes[child];
            if ((nf.EggGroups & nc.EggGroups) == 0)
                return false; // No shared Egg Groups
            if (nf.EggGroups == UNDISCOVERED)
                return false; // Cannot breed
            if (nf.Gender is PersonalInfo.RatioMagicFemale or PersonalInfo.RatioMagicGenderless)
                return false; // Cannot be male
            if (nc.Gender is PersonalInfo.RatioMagicGenderless)
                return false; // Cannot have a father (must breed from Ditto)
            if (nc.Gender is PersonalInfo.RatioMagicMale)
            {
                var childSpecies = nc.Species;
                if (IsDualGenderParent(childSpecies))
                    return true;

                // Can only have itself or its evolutions as a father (must breed from Ditto)
                var fatherSpecies = nf.Species;
                return (fatherSpecies == childSpecies) || (Species)childSpecies switch
                {
                    Species.Tyrogue => (Species)fatherSpecies is Species.Hitmonlee or Species.Hitmonchan or Species.Hitmontop,
                    Species.Rufflet => (Species)fatherSpecies is Species.Braviary,
                    _ => false,
                };
            }
            return true;
        }
    }

    /// <summary>
    /// Node in the graph, representing a particular species.
    /// </summary>
    private record struct LearnNode
    {
        public required ushort Species { get; init; }

        /// <summary>
        /// Gender ratio of the species
        /// </summary>
        public required byte Gender { get; init; }

        /// <summary>
        /// Bitfield of flags for each Egg Group
        /// </summary>
        public required ushort EggGroups { get; set; }

        /// <summary>
        /// Bitfield containing whether each needed move can be learned by level up or as an Egg Move.
        /// </summary>
        /// <remarks>Lower half is level-up moves, upper half is Egg Moves.</remarks>
        public required byte Moves { get; set; }
    }

    /// <summary>
    /// Edge in the graph, representing a father-child relationship.
    /// </summary>
    private record struct LearnEdge(ushort Child, ushort Father, byte Needs);

    private static byte EggMoves(this LearnNode node) => (byte)(node.Moves >> 4);
    private static byte LevelUpMoves(this LearnNode node) => (byte)(node.Moves & 0x0F);
}
