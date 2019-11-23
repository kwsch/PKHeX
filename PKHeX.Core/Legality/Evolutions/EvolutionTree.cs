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
        private static readonly EvolutionTree Evolves8;

        static EvolutionTree()
        {
            // Evolution tables need Personal Tables initialized beforehand, hence why the EvolutionTree data is initialized here.
            static byte[] get(string resource) => Util.GetBinaryResource($"evos_{resource}.pkl");
            static byte[][] unpack(string resource) => Data.UnpackMini(get(resource), resource);

            Evolves1 = new EvolutionTree(new[] { get("rby") }, GameVersion.Gen1, PersonalTable.Y, Legal.MaxSpeciesID_1);
            Evolves2 = new EvolutionTree(new[] { get("gsc") }, GameVersion.Gen2, PersonalTable.C, Legal.MaxSpeciesID_2);
            Evolves3 = new EvolutionTree(new[] { get("g3") }, GameVersion.Gen3, PersonalTable.RS, Legal.MaxSpeciesID_3);
            Evolves4 = new EvolutionTree(new[] { get("g4") }, GameVersion.Gen4, PersonalTable.DP, Legal.MaxSpeciesID_4);
            Evolves5 = new EvolutionTree(new[] { get("g5") }, GameVersion.Gen5, PersonalTable.BW, Legal.MaxSpeciesID_5);
            Evolves6 = new EvolutionTree(unpack("ao"), GameVersion.Gen6, PersonalTable.AO, Legal.MaxSpeciesID_6);
            Evolves7 = new EvolutionTree(unpack("uu"), GameVersion.Gen7, PersonalTable.USUM, Legal.MaxSpeciesID_7_USUM);
            Evolves7b = new EvolutionTree(unpack("gg"), GameVersion.Gen7, PersonalTable.GG, Legal.MaxSpeciesID_7b);
            Evolves8 = new EvolutionTree(unpack("ss"), GameVersion.Gen8, PersonalTable.SWSH, Legal.MaxSpeciesID_8);

            // There's always oddballs.
            Evolves7.FixEvoTreeSM();
            Evolves8.FixEvoTreeSS();
        }

        internal static EvolutionTree GetEvolutionTree(int generation)
        {
            return generation switch
            {
                1 => Evolves1,
                2 => Evolves2,
                3 => Evolves3,
                4 => Evolves4,
                5 => Evolves5,
                6 => Evolves6,
                7 => Evolves7,
                _ => Evolves8
            };
        }

        internal static EvolutionTree GetEvolutionTree(PKM pkm, int generation)
        {
            return generation switch
            {
                1 => Evolves1,
                2 => Evolves2,
                3 => Evolves3,
                4 => Evolves4,
                5 => Evolves5,
                6 => Evolves6,
                7 => (pkm.GG ? Evolves7b : Evolves7),
                _ => Evolves8
            };
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
            return Game switch
            {
                GameVersion.Gen1 => EvolutionSet1.GetArray(data[0], MaxSpeciesTree),
                GameVersion.Gen2 => EvolutionSet1.GetArray(data[0], MaxSpeciesTree),
                GameVersion.Gen3 => EvolutionSet3.GetArray(data[0]),
                GameVersion.Gen4 => EvolutionSet4.GetArray(data[0]),
                GameVersion.Gen5 => EvolutionSet5.GetArray(data[0]),
                GameVersion.Gen6 => EvolutionSet6.GetArray(data),
                GameVersion.Gen7 => EvolutionSet7.GetArray(data),
                GameVersion.Gen8 => EvolutionSet7.GetArray(data),
                _ => throw new Exception()
            };
        }

        private EvolutionLineage[] CreateTree()
        {
            var lineage = new EvolutionLineage[Entries.Count];
            for (int i = 0; i < Entries.Count; i++)
                lineage[i] = new EvolutionLineage();
            if (Game == GameVersion.Gen6)
                Array.Resize(ref lineage, MaxSpeciesTree + 1);

            if (Game.GetGeneration() <= 6)
                GenerateEntriesSpeciesOnly(lineage);
            else
                GenerateEntriesSpeciesForm(lineage);

            return lineage;
        }

        private void GenerateEntriesSpeciesOnly(EvolutionLineage[] lineage)
        {
            for (int species = 1; species < lineage.Length; species++)
                CreateBranch(lineage, species, 0, species);
        }

        private void GenerateEntriesSpeciesForm(EvolutionLineage[] lineage)
        {
            for (int species = 1; species <= MaxSpeciesTree; species++)
            {
                var pi = Personal[species];
                var fc = pi.FormeCount;
                for (int form = 0; form < fc; form++)
                {
                    var index = Personal.GetFormeIndex(species, form);
                    CreateBranch(lineage, species, form, index);
                }
            }
        }

        private void CreateBranch(IReadOnlyList<EvolutionLineage> lineage, int species, int form, int index)
        {
            var evos = Entries[index];
            // Iterate over all possible evolutions
            foreach (var evo in evos)
                CreateLeaf(lineage, evo, species, form, index);
        }

        private void CreateLeaf(IReadOnlyList<EvolutionLineage> lineage, EvolutionMethod evo, int species, int form, int index)
        {
            int evolveTo = GetIndex(evo);
            if (evolveTo < 0)
                return;

            var chainTo = lineage[evolveTo];
            var current = lineage[index];
            var sourceEvo = evo.Copy(species, form);

            chainTo.Insert(sourceEvo);
            // If current entries has a pre-evolution, propagate to evolution as well
            if (current.Chain.Count != 0)
                chainTo.Chain.Insert(0, current.Chain[0]);

            if (evolveTo >= index)
                return;

            // If destination species evolves into something (ie a 'baby' Pokemon like Cleffa)
            // Add it to the corresponding parent chains
            foreach (var method in Entries[evolveTo])
            {
                int newIndex = GetIndex(method);
                if (newIndex < 0)
                    continue;

                lineage[newIndex].Insert(sourceEvo);
            }
        }

        private void FixEvoTreeSM()
        {
            UnpackForms((int)Species.Wormadam, 2);
            UnpackForms((int)Species.Gastrodon, 1);
            UnpackForms((int)Species.Meowstic, 1);
            UnpackForms((int)Species.Floette, 4);
            Lineage[(int)Species.Florges].Chain.RemoveAt(0); // ???
            UnpackForms((int)Species.Florges, 4);
            UnpackForms((int)Species.Gourgeist, 3);

            BanEvo((int)Species.Raichu, 1, EvolutionMethod.BanSM);
            BanEvo((int)Species.Marowak, 0, EvolutionMethod.BanSM);
            BanEvo((int)Species.Raichu, 0, EvolutionMethod.BanSM);
        }

        private void FixEvoTreeSS()
        {
            SpreadForms((int)Species.Silvally, 17);
        }

        private void BanEvo(int species, int type, IReadOnlyCollection<GameVersion> versionsBanned)
        {
            var entry = Personal.GetFormeIndex(species, 0);
            var lin = Lineage[entry];
            lin.Chain[type][0].Banlist = versionsBanned;
        }

        private void UnpackForms(int species, int formCount)
        {
            var baseChain = Lineage[species];
            for (int i = 1; i <= formCount; i++)
            {
                var entry = Personal.GetFormeIndex(species, i);
                var lin = Lineage[entry];
                lin.Chain.Add(new List<EvolutionMethod> { baseChain.Chain[0][i] });
            }
            baseChain.Chain[0].RemoveRange(1, formCount);
        }

        private void SpreadForms(int species, int formCount)
        {
            var baseChain = Lineage[species];
            for (int i = 1; i <= formCount; i++)
            {
                var entry = Personal.GetFormeIndex(species, i);
                var lin = Lineage[entry];
                lin.Chain.Add(baseChain.Chain[0]);
            }
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
