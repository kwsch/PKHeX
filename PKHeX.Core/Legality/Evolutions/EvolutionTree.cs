using System;
using System.Collections.Generic;
using System.Linq;

namespace PKHeX.Core
{
    /// <summary>
    /// Generation specific Evolution Tree data.
    /// </summary>
    /// <remarks>
    /// Used to determine if a <see cref="PKM.Species"/> can evolve from prior steps in its evolution branch.
    /// </remarks>
    public class EvolutionTree
    {
        private static readonly EvolutionTree Evolves1;
        private static readonly EvolutionTree Evolves2;
        private static readonly EvolutionTree Evolves3;
        private static readonly EvolutionTree Evolves4;
        private static readonly EvolutionTree Evolves5;
        private static readonly EvolutionTree Evolves6;
        private static readonly EvolutionTree Evolves7;

        static EvolutionTree()
        {
            // Evolution tables need Personal Tables initialized beforehand, hence why the EvolutionTree data is initialized here.
            byte[] get(string resource) => Util.GetBinaryResource($"evos_{resource}.pkl");
            byte[][] unpack(string resource) => Data.UnpackMini(get(resource), resource);

            Evolves1 = new EvolutionTree(new[] { get("rby") }, GameVersion.RBY, PersonalTable.Y, Legal.MaxSpeciesID_1);
            Evolves2 = new EvolutionTree(new[] { get("gsc") }, GameVersion.GSC, PersonalTable.C, Legal.MaxSpeciesID_2);
            Evolves3 = new EvolutionTree(new[] { get("g3") }, GameVersion.RS, PersonalTable.RS, Legal.MaxSpeciesID_3);
            Evolves4 = new EvolutionTree(new[] { get("g4") }, GameVersion.DP, PersonalTable.DP, Legal.MaxSpeciesID_4);
            Evolves5 = new EvolutionTree(new[] { get("g5") }, GameVersion.BW, PersonalTable.BW, Legal.MaxSpeciesID_5);
            Evolves6 = new EvolutionTree(unpack("ao"), GameVersion.ORAS, PersonalTable.AO, Legal.MaxSpeciesID_6);
            Evolves7 = new EvolutionTree(unpack("uu"), GameVersion.USUM, PersonalTable.USUM, Legal.MaxSpeciesID_7_USUM);
        }
        internal static EvolutionTree GetEvolutionTree(int generation)
        {
            switch (generation)
            {
                case 1:
                    return Evolves1;
                case 2:
                    return Evolves2;
                case 3:
                    return Evolves3;
                case 4:
                    return Evolves4;
                case 5:
                    return Evolves5;
                case 6:
                    return Evolves6;
                default:
                    return Evolves7;
            }
        }

        private List<EvolutionSet> Entries { get; } = new List<EvolutionSet>();
        private readonly EvolutionLineage[] Lineage;
        private readonly GameVersion Game;
        private readonly PersonalTable Personal;
        private readonly int MaxSpeciesTree;

        public EvolutionTree(byte[][] data, GameVersion game, PersonalTable personal, int maxSpeciesTree)
        {
            Game = game;
            Personal = personal;
            MaxSpeciesTree = maxSpeciesTree;
            switch (game)
            {
                case GameVersion.RBY:
                    Entries = EvolutionSet1.GetArray(data[0], maxSpeciesTree);
                    break;
                case GameVersion.GSC:
                    Entries = EvolutionSet2.GetArray(data[0], maxSpeciesTree);
                    break;
                case GameVersion.RS:
                    Entries = EvolutionSet3.GetArray(data[0]);
                    break;
                case GameVersion.DP:
                    Entries = EvolutionSet4.GetArray(data[0]);
                    break;
                case GameVersion.BW:
                    Entries = EvolutionSet5.GetArray(data[0]);
                    break;
                case GameVersion.ORAS:
                    Entries.AddRange(data.Select(d => new EvolutionSet6(d)));
                    break;
                case GameVersion.USUM:
                    Entries.AddRange(data.Select(d => new EvolutionSet7(d)));
                    break;
            }

            // Create Lineages
            Lineage = new EvolutionLineage[Entries.Count];
            for (int i = 0; i < Entries.Count; i++)
                Lineage[i] = new EvolutionLineage();
            if (Game == GameVersion.ORAS)
                Array.Resize(ref Lineage, MaxSpeciesTree + 1);

            // Populate Lineages
            for (int i = 1; i < Lineage.Length; i++)
            {
                // Iterate over all possible evolutions
                var s = Entries[i];
                foreach (EvolutionMethod evo in s.PossibleEvolutions)
                {
                    int index = GetIndex(evo);
                    if (index < 0)
                        continue;

                    var sourceEvo = evo.Copy(i);

                    Lineage[index].Insert(sourceEvo);
                    // If current entries has a pre-evolution, propagate to evolution as well
                    if (Lineage[i].Chain.Count > 0)
                        Lineage[index].Insert(Lineage[i].Chain[0]);

                    if (index >= i) continue;
                    // If destination species evolves into something (ie a 'baby' Pokemon like Cleffa)
                    // Add it to the corresponding parent chains
                    foreach (EvolutionMethod mid in Entries[index].PossibleEvolutions)
                    {
                        int newIndex = GetIndex(mid);
                        if (newIndex < 0)
                            continue;

                        Lineage[newIndex].Insert(sourceEvo);
                    }
                }
            }
            FixEvoTreeManually();
        }

        // There's always oddballs.
        private void FixEvoTreeManually()
        {
            if (Game == GameVersion.USUM)
                FixEvoTreeSM();
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
                .Chain[1].StageEntryMethods[0]
                .Banlist = EvolutionMethod.BanSM;
            // Ban Exeggutor Evolution on SM
            Lineage[Personal.GetFormeIndex(103, 0)]
                .Chain[0].StageEntryMethods[0]
                .Banlist = EvolutionMethod.BanSM;
            // Ban Marowak Evolution on SM
            Lineage[Personal.GetFormeIndex(105, 0)]
                .Chain[0].StageEntryMethods[0]
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
        public List<DexLevel> GetValidPreEvolutions(PKM pkm, int maxLevel, int maxSpeciesOrigin = -1, bool skipChecks = false, int minLevel = 1)
        {
            int index = GetIndex(pkm);
            if (maxSpeciesOrigin <= 0)
                maxSpeciesOrigin = Legal.GetMaxSpeciesOrigin(pkm);
            return Lineage[index].GetExplicitLineage(pkm, maxLevel, skipChecks, MaxSpeciesTree, maxSpeciesOrigin, minLevel);
        }
    }
}
