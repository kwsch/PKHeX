using System;
using System.Collections.Generic;
using System.Linq;
using static PKHeX.Core.GameVersion;

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
            static byte[][] unpack(string resource) => BinLinker.Unpack(get(resource), resource);

            Evolves1 = new EvolutionTree(new[] { get("rby") }, Gen1, PersonalTable.Y, Legal.MaxSpeciesID_1);
            Evolves2 = new EvolutionTree(new[] { get("gsc") }, Gen2, PersonalTable.C, Legal.MaxSpeciesID_2);
            Evolves3 = new EvolutionTree(new[] { get("g3") }, Gen3, PersonalTable.RS, Legal.MaxSpeciesID_3);
            Evolves4 = new EvolutionTree(new[] { get("g4") }, Gen4, PersonalTable.DP, Legal.MaxSpeciesID_4);
            Evolves5 = new EvolutionTree(new[] { get("g5") }, Gen5, PersonalTable.BW, Legal.MaxSpeciesID_5);
            Evolves6 = new EvolutionTree(unpack("ao"), Gen6, PersonalTable.AO, Legal.MaxSpeciesID_6);
            Evolves7 = new EvolutionTree(unpack("uu"), Gen7, PersonalTable.USUM, Legal.MaxSpeciesID_7_USUM);
            Evolves7b = new EvolutionTree(unpack("gg"), Gen7, PersonalTable.GG, Legal.MaxSpeciesID_7b);
            Evolves8 = new EvolutionTree(unpack("ss"), Gen8, PersonalTable.SWSH, Legal.MaxSpeciesID_8);

            // Throw in banned evolution data!
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
        private readonly GameVersion Game;
        private readonly PersonalTable Personal;
        private readonly int MaxSpeciesTree;
        private readonly ILookup<int, EvolutionLink> Lineage;
        private static int GetLookupKey(int species, int form) => species | (form << 11);

        #region Constructor

        private EvolutionTree(IReadOnlyList<byte[]> data, GameVersion game, PersonalTable personal, int maxSpeciesTree)
        {
            Game = game;
            Personal = personal;
            MaxSpeciesTree = maxSpeciesTree;
            Entries = GetEntries(data);

            // Starting in Generation 7, forms have separate evolution data.
            int format = Game - Gen1 + 1;
            var oldStyle = format < 7;
            var connections = oldStyle ? CreateTreeOld() : CreateTree();

            Lineage = connections.ToLookup(obj => obj.Key, obj => obj.Value);
        }

        private IEnumerable<KeyValuePair<int, EvolutionLink>> CreateTreeOld()
        {
            for (int sSpecies = 1; sSpecies <= MaxSpeciesTree; sSpecies++)
            {
                var fc = Personal[sSpecies].FormeCount;
                for (int sForm = 0; sForm < fc; sForm++)
                {
                    var index = sSpecies;
                    var evos = Entries[index];
                    foreach (var evo in evos)
                    {
                        var dSpecies = evo.Species;
                        if (dSpecies == 0)
                            continue;

                        var dForm = sSpecies == (int)Species.Espurr && evo.Method == (int)EvolutionType.LevelUpFormFemale1 ? 1 : sForm;
                        var key = GetLookupKey(dSpecies, dForm);

                        var link = new EvolutionLink(sSpecies, sForm, evo);
                        yield return new KeyValuePair<int, EvolutionLink>(key, link);
                    }
                }
            }
        }

        private IEnumerable<KeyValuePair<int, EvolutionLink>> CreateTree()
        {
            for (int sSpecies = 1; sSpecies <= MaxSpeciesTree; sSpecies++)
            {
                var fc = Personal[sSpecies].FormeCount;
                for (int sForm = 0; sForm < fc; sForm++)
                {
                    var index = Personal.GetFormeIndex(sSpecies, sForm);
                    var evos = Entries[index];
                    foreach (var evo in evos)
                    {
                        var dSpecies = evo.Species;
                        if (dSpecies == 0)
                            break;

                        var dForm = evo.GetDestinationForm(sForm);
                        var key = GetLookupKey(dSpecies, dForm);

                        var link = new EvolutionLink(sSpecies, sForm, evo);
                        yield return new KeyValuePair<int, EvolutionLink>(key, link);
                    }
                }
            }
        }

        private IReadOnlyList<EvolutionMethod[]> GetEntries(IReadOnlyList<byte[]> data)
        {
            return Game switch
            {
                Gen1 => EvolutionSet1.GetArray(data[0], MaxSpeciesTree),
                Gen2 => EvolutionSet1.GetArray(data[0], MaxSpeciesTree),
                Gen3 => EvolutionSet3.GetArray(data[0]),
                Gen4 => EvolutionSet4.GetArray(data[0]),
                Gen5 => EvolutionSet5.GetArray(data[0]),
                Gen6 => EvolutionSet6.GetArray(data),
                Gen7 => EvolutionSet7.GetArray(data),
                Gen8 => EvolutionSet7.GetArray(data),
                _ => throw new Exception()
            };
        }

        private void FixEvoTreeSM()
        {
            // Sun/Moon lack Ultra's Kantonian evolution methods.
            BanEvo((int)Species.Raichu, 1, pkm => pkm.IsUntraded && pkm.SM);
            BanEvo((int)Species.Marowak, 0, pkm => pkm.IsUntraded && pkm.SM);
            BanEvo((int)Species.Raichu, 0, pkm => pkm.IsUntraded && pkm.SM);
        }

        private void FixEvoTreeSS()
        {
            // Gigantamax Pikachu, Meowth-0, and Eevee are prevented from evolving.
            BanEvo((int)Species.Raichu, 0, pkm => pkm is IGigantamax g && g.CanGigantamax);
            BanEvo((int)Species.Raichu, 1, pkm => pkm is IGigantamax g && g.CanGigantamax || pkm.Gen8 || pkm.GG);
            BanEvo((int)Species.Persian, 0, pkm => pkm is IGigantamax g && g.CanGigantamax);

            BanEvo((int)Species.Weezing, 0, pkm => pkm.Gen8);

            foreach (var s in GetEvolutions((int)Species.Eevee, 0)) // Eeveelutions
                BanEvo(s, 0, pkm => pkm is IGigantamax g && g.CanGigantamax);
        }

        private void BanEvo(int species, int form, Func<PKM, bool> func)
        {
            var key = GetLookupKey(species, form);
            var node = Lineage[key];
            foreach (var link in node)
                link.IsBanned = func;
        }

        #endregion

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
            if (maxSpeciesOrigin <= 0)
                maxSpeciesOrigin = Legal.GetMaxSpeciesOrigin(pkm);
            if (pkm.IsEgg && !skipChecks)
            {
                return new List<EvoCriteria>(1)
                {
                    new EvoCriteria(pkm.Species, pkm.AltForm) { Level = maxLevel, MinLevel = maxLevel },
                };
            }

            // Shedinja's evolution case can be a little tricky; hard-code handling.
            if (pkm.Species == (int)Species.Shedinja && maxLevel >= 20 && (!pkm.HasOriginalMetLocation || pkm.Met_Level + 1 <= maxLevel))
            {
                return new List<EvoCriteria>(2)
                {
                    new EvoCriteria((int)Species.Shedinja, 0) { Level = maxLevel, MinLevel = 20 },
                    new EvoCriteria((int)Species.Nincada, 0) { Level = maxLevel, MinLevel = 1 },
                };
            }

            return GetExplicitLineage(pkm, maxLevel, skipChecks, maxSpeciesOrigin, minLevel);
        }

        /// <summary>
        /// Gets all species the <see cref="species"/>-<see cref="form"/> can evolve to &amp; from, yielded in order of increasing evolution stage.
        /// </summary>
        /// <param name="species">Species ID</param>
        /// <param name="form">Form ID</param>
        /// <returns>Enumerable of species IDs.</returns>
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
            int index = GetLookupKey(species, form);
            var node = Lineage[index];
            foreach (var method in node)
            {
                var s = method.Species;
                if (s == 0)
                    continue;
                var f = method.Form;
                yield return s;
                var preEvolutions = GetPreEvolutions(s, f);
                foreach (var preEvo in preEvolutions)
                    yield return preEvo;
            }
        }

        private IEnumerable<int> GetEvolutions(int species, int form)
        {
            int format = Game - Gen1 + 1;
            int index = format < 7 ? species : Personal.GetFormeIndex(species, form);
            var evos = Entries[index];
            foreach (var method in evos)
            {
                var s = method.Species;
                if (s == 0)
                    continue;
                var f = method.GetDestinationForm(form);
                yield return s;
                var nextEvolutions = GetEvolutions(s, f);
                foreach (var next in nextEvolutions)
                    yield return next;
            }
        }

        /// <summary>
        /// Generates the reverse evolution path for the input <see cref="pkm"/>.
        /// </summary>
        /// <param name="pkm">Entity data</param>
        /// <param name="maxLevel">Maximum level</param>
        /// <param name="skipChecks">Skip the secondary checks that validate the evolution</param>
        /// <param name="maxSpeciesOrigin">Clamp for maximum species ID</param>
        /// <param name="minLevel">Minimum level</param>
        /// <returns></returns>
        private List<EvoCriteria> GetExplicitLineage(PKM pkm, int maxLevel, bool skipChecks, int maxSpeciesOrigin, int minLevel)
        {
            int species = pkm.Species;
            int form = pkm.AltForm;
            int lvl = maxLevel;
            var first = new EvoCriteria(species, form) { Level = lvl };

            const int maxEvolutions = 3;
            var dl = new List<EvoCriteria>(maxEvolutions) { first };

            switch (species)
            {
                case (int)Species.Silvally: form = 0;
                    break;
            }

            // There aren't any circular evolution paths, and all lineages have at most 3 evolutions total.
            // There aren't any convergent evolution paths, so only yield the first connection.
            while (true)
            {
                var key = GetLookupKey(species, form);
                var node = Lineage[key];

                bool oneValid = false;
                foreach (var link in node)
                {
                    if (link.IsEvolutionBanned(pkm) && !skipChecks)
                        continue;

                    var evo = link.Method;
                    if (!evo.Valid(pkm, lvl, skipChecks))
                        continue;

                    if (evo.RequiresLevelUp && minLevel >= lvl)
                        break; // impossible evolution

                    oneValid = true;
                    UpdateMinValues(dl, evo);
                    if (evo.RequiresLevelUp)
                        lvl--;

                    species = link.Species;
                    form = link.Form;
                    var detail = evo.GetEvoCriteria(species, form, lvl);
                    dl.Add(detail);
                    break;
                }
                if (!oneValid)
                    break;
            }

            // Remove future gen pre-evolutions; no Munchlax from a Gen3 Snorlax, no Pichu from a Gen1-only Raichu, etc
            var last = dl[dl.Count - 1];
            if (last.Species > maxSpeciesOrigin && dl.Any(d => d.Species <= maxSpeciesOrigin))
                dl.RemoveAt(dl.Count - 1);

            // Last species is the wild/hatched species, the minimum level is 1 because it has not evolved from previous species
            last = dl[dl.Count - 1];
            last.MinLevel = 1;
            last.RequiresLvlUp = false;
            return dl;
        }

        private static void UpdateMinValues(IReadOnlyList<EvoCriteria> dl, EvolutionMethod evo)
        {
            var last = dl[dl.Count - 1];
            if (!evo.RequiresLevelUp)
            {
                // Evolutions like elemental stones, trade, etc
                last.MinLevel = 1;
                return;
            }
            if (evo.Level == 0)
            {
                // Friendship based Evolutions, Pichu -> Pikachu, Eevee -> Umbreon, etc
                last.MinLevel = 2;

                var first = dl[0];
                if (dl.Count > 1 && !first.RequiresLvlUp)
                    first.MinLevel = 2; // Raichu from Pikachu would have a minimum level of 1; accounting for Pichu (level up required) results in a minimum level of 2
            }
            else // level up evolutions
            {
                last.MinLevel = evo.Level;

                var first = dl[0];
                if (dl.Count > 1)
                {
                    if (first.RequiresLvlUp)
                    {
                        if (first.MinLevel <= evo.Level)
                            first.MinLevel = evo.Level + 1; // Pokemon like Crobat, its minimum level is Golbat minimum level + 1
                    }
                    else
                    {
                        if (first.MinLevel < evo.Level)
                            first.MinLevel = evo.Level; // Pokemon like Nidoqueen who evolve with an evolution stone, minimum level is prior evolution minimum level
                    }
                }
            }
            last.RequiresLvlUp = evo.RequiresLevelUp;
        }

        /// <summary>
        /// Links a <see cref="EvolutionMethod"/> to the source <see cref="Species"/> and <see cref="Form"/> that the method can be triggered from.
        /// </summary>
        private sealed class EvolutionLink
        {
            public readonly int Species;
            public readonly int Form;
            public readonly EvolutionMethod Method;
            public Func<PKM, bool>? IsBanned { private get; set; }

            public EvolutionLink(int species, int form, EvolutionMethod method)
            {
                Species = species;
                Form = form;
                Method = method;
            }

            /// <summary>
            /// Checks if the <see cref="Method"/> is allowed.
            /// </summary>
            /// <param name="pkm">Entity to check</param>
            /// <returns>True if banned, false if allowed.</returns>
            public bool IsEvolutionBanned(PKM pkm) => IsBanned != null && IsBanned(pkm);
        }
    }
}
