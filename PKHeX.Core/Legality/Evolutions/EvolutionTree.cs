using System;
using System.Collections.Generic;

namespace PKHeX.Core
{
    /// <summary>
    /// Generation specific Evolution Tree data.
    /// </summary>
    /// <remarks>
    /// Used to determine if a <see cref="PKM.Species"/> can evolve from prior steps in its evolution branch.
    /// </remarks>
    public sealed class EvolutionTree
    {
        private static readonly EvolutionTree Evolves1;
        private static readonly EvolutionTree Evolves2;
        private static readonly EvolutionTree Evolves3;
        private static readonly EvolutionTree Evolves4;
        private static readonly EvolutionTree Evolves5;
        private static readonly EvolutionTree Evolves6;
        private static readonly EvolutionTree Evolves7;
        private static readonly EvolutionTree Evolves7b;

        static EvolutionTree()
        {
            // Evolution tables need Personal Tables initialized beforehand, hence why the EvolutionTree data is initialized here.
            byte[] get(string resource) => Util.GetBinaryResource($"evos_{resource}.pkl");
            byte[][] unpack(string resource) => Data.UnpackMini(get(resource), resource);

            Evolves1 = new EvolutionTree(new[] { get("rby") }, GameVersion.Gen1, PersonalTable.Y, Legal.MaxSpeciesID_1);
            Evolves2 = new EvolutionTree(new[] { get("gsc") }, GameVersion.Gen2, PersonalTable.C, Legal.MaxSpeciesID_2);
            Evolves3 = new EvolutionTree(new[] { get("g3") }, GameVersion.Gen3, PersonalTable.RS, Legal.MaxSpeciesID_3);
            Evolves4 = new EvolutionTree(new[] { get("g4") }, GameVersion.Gen4, PersonalTable.DP, Legal.MaxSpeciesID_4);
            Evolves5 = new EvolutionTree(new[] { get("g5") }, GameVersion.Gen5, PersonalTable.BW, Legal.MaxSpeciesID_5);
            Evolves6 = new EvolutionTree(unpack("ao"), GameVersion.Gen6, PersonalTable.AO, Legal.MaxSpeciesID_6);
            Evolves7 = new EvolutionTree(unpack("uu"), GameVersion.Gen7, PersonalTable.USUM, Legal.MaxSpeciesID_7_USUM);
            Evolves7b = new EvolutionTree(unpack("gg"), GameVersion.Gen7, PersonalTable.GG, Legal.MaxSpeciesID_7b);

            // There's always oddballs.
            Evolves7.FixEvoTreeSM();
        }

        internal static EvolutionTree GetEvolutionTree(int generation)
        {
            switch (generation)
            {
                case 1: return Evolves1;
                case 2: return Evolves2;
                case 3: return Evolves3;
                case 4: return Evolves4;
                case 5: return Evolves5;
                case 6: return Evolves6;
                default:
                    return Evolves7;
            }
        }

        internal static EvolutionTree GetEvolutionTree(PKM pkm, int generation)
        {
            switch (generation)
            {
                case 1: return Evolves1;
                case 2: return Evolves2;
                case 3: return Evolves3;
                case 4: return Evolves4;
                case 5: return Evolves5;
                case 6: return Evolves6;
                case 7 when pkm.GG: return Evolves7b;
                default:
                    return Evolves7;
            }
        }

        private readonly IReadOnlyList<EvolutionMethod[]> Entries;
        private readonly EvolutionLineage[] Lineage;
        private readonly GameVersion Game;
        private readonly PersonalTable Personal;
        private readonly int MaxSpeciesTree;

        private EvolutionTree(IReadOnlyList<byte[]> data, GameVersion game, PersonalTable personal, int maxSpeciesTree)
        {
            Game = game;
            Personal = personal;
            MaxSpeciesTree = maxSpeciesTree;
            Entries = GetEntries(data);
            Lineage = CreateTree();
        }

        private IReadOnlyList<EvolutionMethod[]> GetEntries(IReadOnlyList<byte[]> data)
        {
            switch (Game)
            {
                case GameVersion.Gen1:
                case GameVersion.Gen2: return EvolutionSet1.GetArray(data[0], MaxSpeciesTree);
                case GameVersion.Gen3: return EvolutionSet3.GetArray(data[0]);
                case GameVersion.Gen4: return EvolutionSet4.GetArray(data[0]);
                case GameVersion.Gen5: return EvolutionSet5.GetArray(data[0]);
                case GameVersion.Gen6: return EvolutionSet6.GetArray(data);
                case GameVersion.Gen7: return EvolutionSet7.GetArray(data);
                default: throw new Exception();
            }
        }

        private EvolutionLineage[] CreateTree()
        {
            var lineage = new EvolutionLineage[Entries.Count];
            for (int i = 0; i < Entries.Count; i++)
                lineage[i] = new EvolutionLineage();
            if (Game == GameVersion.Gen6)
                Array.Resize(ref lineage, MaxSpeciesTree + 1);

            // Populate Lineages
            for (int i = 1; i < lineage.Length; i++)
                CreateBranch(lineage, i);
            return lineage;
        }

        private void CreateBranch(IReadOnlyList<EvolutionLineage> lineage, int i)
        {
            // Iterate over all possible evolutions
            foreach (var evo in Entries[i])
                CreateLeaf(lineage, i, evo);
        }

        private void CreateLeaf(IReadOnlyList<EvolutionLineage> lineage, int i, EvolutionMethod evo)
        {
            int index = GetIndex(evo);
            if (index < 0)
                return;

            var sourceEvo = evo.Copy(i);

            lineage[index].Insert(sourceEvo);
            // If current entries has a pre-evolution, propagate to evolution as well
            var current = lineage[i].Chain;
            if (current.Count > 0)
                lineage[index].Chain.Insert(0, current[0]);

            if (index >= i)
                return;

            // If destination species evolves into something (ie a 'baby' Pokemon like Cleffa)
            // Add it to the corresponding parent chains
            foreach (var method in Entries[index])
            {
                int newIndex = GetIndex(method);
                if (newIndex < 0)
                    continue;

                lineage[newIndex].Insert(sourceEvo);
            }
        }

        private void FixEvoTreeSM()
        {
            // Wormadam -- Copy Burmy 0 to Wormadam-1/2
            Lineage[Personal.GetFormeIndex(413, 1)].Chain.Insert(0, Lineage[413].Chain[0]);
            Lineage[Personal.GetFormeIndex(413, 2)].Chain.Insert(0, Lineage[413].Chain[0]);

            // Shellos -- Move Shellos-1 evo from Gastrodon-0 to Gastrodon-1
            Lineage[Personal.GetFormeIndex(422 + 1, 1)].Chain.Insert(0, Lineage[422 + 1].Chain[0]);
            Lineage[422+1].Chain.RemoveAt(0);

            // Meowstic -- Meowstic-1 (F) should point back to Espurr, copy Meowstic-0 (M)
            Lineage[Personal.GetFormeIndex(678, 1)].Chain.Insert(0, Lineage[678].Chain[0]);

            // Floette doesn't contain evo info for forms 1-4, copy. Florges points to form 0, no fix needed.
            var fbb = Lineage[669+1].Chain[0];
            for (int i = 1; i <= 4; i++) // NOT AZ
                Lineage[Personal.GetFormeIndex(669+1, i)].Chain.Insert(0, fbb);
            // Clear forme chains from Florges
            Lineage[671].Chain.RemoveRange(0, Lineage[671].Chain.Count - 2);

            // Gourgeist -- Sizes are still relevant. Formes are in reverse order.
            for (int i = 1; i <= 3; i++)
            {
                Lineage[Personal.GetFormeIndex(711, i)].Chain.Clear();
                Lineage[Personal.GetFormeIndex(711, i)].Chain.Add(Lineage[711].Chain[3-i]);
            }
            Lineage[711].Chain.RemoveRange(0, 3);

            // Ban Raichu Evolution on SM
            Lineage[Personal.GetFormeIndex(26, 0)]
                .Chain[1][0]
                .Banlist = EvolutionMethod.BanSM;
            // Ban Exeggutor Evolution on SM
            Lineage[Personal.GetFormeIndex(103, 0)]
                .Chain[0][0]
                .Banlist = EvolutionMethod.BanSM;
            // Ban Marowak Evolution on SM
            Lineage[Personal.GetFormeIndex(105, 0)]
                .Chain[0][0]
                .Banlist = EvolutionMethod.BanSM;
        }

        private int GetIndex(PKM pkm)
        {
            if (pkm.Format < 7)
                return pkm.Species;
            return Personal.GetFormeIndex(pkm.Species, pkm.AltForm);
        }

        private int GetIndex(EvolutionMethod evo)
        {
            int evolvesToSpecies = evo.Species;
            if (evolvesToSpecies == 0)
                return -1;

            if (Personal == null)
                return evolvesToSpecies;

            int evolvesToForm = evo.Form;
            if (evolvesToForm < 0)
                evolvesToForm = 0;

            return Personal.GetFormeIndex(evolvesToSpecies, evolvesToForm);
        }

        /// <summary>
        /// Gets a list of evolutions for the input <see cref="PKM"/> by checking each evolution in the chain.
        /// </summary>
        /// <param name="pkm">Pokémon data to check with.</param>
        /// <param name="maxLevel">Maximum level to permit before the chain breaks.</param>
        /// <param name="maxSpeciesOrigin">Maximum species ID to permit within the chain.</param>
        /// <param name="skipChecks">Ignores an evolution's criteria, causing the returned list to have all possible evolutions.</param>
        /// <param name="minLevel">Minimum level to permit before the chain breaks.</param>
        /// <returns></returns>
        public List<EvoCriteria> GetValidPreEvolutions(PKM pkm, int maxLevel, int maxSpeciesOrigin = -1, bool skipChecks = false, int minLevel = 1)
        {
            int index = GetIndex(pkm);
            if (maxSpeciesOrigin <= 0)
                maxSpeciesOrigin = Legal.GetMaxSpeciesOrigin(pkm);
            return Lineage[index].GetExplicitLineage(pkm, maxLevel, skipChecks, MaxSpeciesTree, maxSpeciesOrigin, minLevel);
        }

        public IEnumerable<int> GetEvolutionsAndPreEvolutions(int species, int form)
        {
            foreach (var s in GetPreEvolutions(species, form))
                yield return s;
            yield return species;
            foreach (var s in GetEvolutions(species, form))
                yield return s;
        }

        private IEnumerable<int> GetPreEvolutions(int species, int form)
        {
            int index = Personal.GetFormeIndex(species, form);
            var node = Lineage[index];
            foreach (var methods in node.Chain)
            {
                foreach (var prevo in methods)
                    yield return prevo.Species;
            }
        }

        private IEnumerable<int> GetEvolutions(int species, int form)
        {
            int index = Personal.GetFormeIndex(species, form);
            var node = Entries[index];
            foreach (var z in node)
            {
                var s = z.Species;
                if (s == 0)
                    continue;
                yield return s;
                foreach (var next in GetEvolutions(s, form))
                    yield return next;
            }
        }
    }
}
