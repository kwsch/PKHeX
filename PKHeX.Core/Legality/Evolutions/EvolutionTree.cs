using System;
using System.Collections.Generic;
using System.Linq;
using static PKHeX.Core.GameVersion;
using static PKHeX.Core.Legal;

namespace PKHeX.Core;

/// <summary>
/// Generation specific Evolution Tree data.
/// </summary>
/// <remarks>
/// Used to determine if a <see cref="PKM.Species"/> can evolve from prior steps in its evolution branch.
/// </remarks>
public sealed class EvolutionTree
{
    public static readonly EvolutionTree Evolves1 = new(GetResource("rby"), Gen1, PersonalTable.Y, MaxSpeciesID_1);
    public static readonly EvolutionTree Evolves2 = new(GetResource("gsc"), Gen2, PersonalTable.C, MaxSpeciesID_2);
    public static readonly EvolutionTree Evolves3 = new(GetResource("g3"), Gen3, PersonalTable.RS, MaxSpeciesID_3);
    public static readonly EvolutionTree Evolves4 = new(GetResource("g4"), Gen4, PersonalTable.DP, MaxSpeciesID_4);
    public static readonly EvolutionTree Evolves5 = new(GetResource("g5"), Gen5, PersonalTable.BW, MaxSpeciesID_5);
    public static readonly EvolutionTree Evolves6 = new(GetReader("ao"), Gen6, PersonalTable.AO, MaxSpeciesID_6);
    public static readonly EvolutionTree Evolves7 = new(GetReader("uu"), Gen7, PersonalTable.USUM, MaxSpeciesID_7_USUM);
    public static readonly EvolutionTree Evolves7b = new(GetReader("gg"), Gen7, PersonalTable.GG, MaxSpeciesID_7b);
    public static readonly EvolutionTree Evolves8 = new(GetReader("ss"), Gen8, PersonalTable.SWSH, MaxSpeciesID_8);
    public static readonly EvolutionTree Evolves8a = new(GetReader("la"), Gen8, PersonalTable.LA, MaxSpeciesID_8a);
    public static readonly EvolutionTree Evolves8b = new(GetReader("bs"), Gen8, PersonalTable.BDSP, MaxSpeciesID_8b);

    private static ReadOnlySpan<byte> GetResource(string resource) => Util.GetBinaryResource($"evos_{resource}.pkl");
    private static BinLinkerAccessor GetReader(string resource) => BinLinkerAccessor.Get(GetResource(resource), resource);

    static EvolutionTree()
    {
        // Add in banned evolution data!
        Evolves7.FixEvoTreeSM();
        Evolves8.FixEvoTreeSS();
        Evolves8b.FixEvoTreeBS();
    }

    public static EvolutionTree GetEvolutionTree(EntityContext context) => context switch
    {
        EntityContext.Gen1 => Evolves1,
        EntityContext.Gen2 => Evolves2,
        EntityContext.Gen3 => Evolves3,
        EntityContext.Gen4 => Evolves4,
        EntityContext.Gen5 => Evolves5,
        EntityContext.Gen6 => Evolves6,
        EntityContext.Gen7 => Evolves7,
        EntityContext.Gen8 => Evolves8,
        EntityContext.Gen7b => Evolves7b,
        EntityContext.Gen8a => Evolves8a,
        EntityContext.Gen8b => Evolves8b,
        _ => throw new ArgumentOutOfRangeException(nameof(context), context, null),
    };

    private readonly IReadOnlyList<EvolutionMethod[]> Entries;
    private readonly GameVersion Game;
    private readonly IPersonalTable Personal;
    private readonly int MaxSpeciesTree;
    private readonly ILookup<int, EvolutionLink> Lineage;
    private static int GetLookupKey(int species, int form) => species | (form << 11);

    #region Constructor

    private EvolutionTree(ReadOnlySpan<byte> data, GameVersion game, IPersonalTable personal, int maxSpeciesTree)
    {
        Game = game;
        Personal = personal;
        MaxSpeciesTree = maxSpeciesTree;
        Entries = GetEntries(data, game);

        // Starting in Generation 7, forms have separate evolution data.
        int format = Game - Gen1 + 1;
        var oldStyle = format < 7;
        var connections = oldStyle ? CreateTreeOld() : CreateTree();

        Lineage = connections.ToLookup(obj => obj.Key, obj => obj.Value);
    }

    private EvolutionTree(BinLinkerAccessor data, GameVersion game, IPersonalTable personal, int maxSpeciesTree)
    {
        Game = game;
        Personal = personal;
        MaxSpeciesTree = maxSpeciesTree;
        Entries = GetEntries(data, game);

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
            var fc = Personal[sSpecies].FormCount;
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
            var fc = Personal[sSpecies].FormCount;
            for (int sForm = 0; sForm < fc; sForm++)
            {
                var index = Personal.GetFormIndex(sSpecies, sForm);
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

    private static IReadOnlyList<EvolutionMethod[]> GetEntries(ReadOnlySpan<byte> data, GameVersion game) => game switch
    {
        Gen1 => EvolutionSet1.GetArray(data, 151),
        Gen2 => EvolutionSet1.GetArray(data, 251),
        Gen3 => EvolutionSet3.GetArray(data),
        Gen4 => EvolutionSet4.GetArray(data),
        Gen5 => EvolutionSet5.GetArray(data),
        _ => throw new ArgumentOutOfRangeException(nameof(game)),
    };

    private static IReadOnlyList<EvolutionMethod[]> GetEntries(BinLinkerAccessor data, GameVersion game) => game switch
    {
        Gen6 => EvolutionSet6.GetArray(data),
        Gen7 => EvolutionSet7.GetArray(data),
        Gen8 => EvolutionSet7.GetArray(data),
        _ => throw new ArgumentOutOfRangeException(nameof(game)),
    };

    private void FixEvoTreeSM()
    {
        // Sun/Moon lack Ultra's Kantonian evolution methods.
        BanEvo((int)Species.Raichu, 0, pk => pk.IsUntraded && pk.SM);
        BanEvo((int)Species.Marowak, 0, pk => pk.IsUntraded && pk.SM);
        BanEvo((int)Species.Exeggutor, 0, pk => pk.IsUntraded && pk.SM);
    }

    private void FixEvoTreeSS()
    {
        // Gigantamax Pikachu, Meowth-0, and Eevee are prevented from evolving.
        // Raichu cannot be evolved to the Alolan variant at this time.
        BanEvo((int)Species.Raichu, 0, pk => pk is IGigantamax {CanGigantamax: true});
        BanEvo((int)Species.Raichu, 1, pk => (pk is IGigantamax {CanGigantamax: true}) || pk.Version is (int)GO or >= (int)GP);
        BanEvo((int)Species.Persian, 0, pk => pk is IGigantamax {CanGigantamax: true});
        BanEvo((int)Species.Persian, 1, pk => pk is IGigantamax {CanGigantamax: true});
        BanEvo((int)Species.Perrserker, 0, pk => pk is IGigantamax {CanGigantamax: true});

        BanEvo((int)Species.Exeggutor, 1, pk => pk.Version is (int)GO or >= (int)GP);
        BanEvo((int)Species.Marowak, 1, pk => pk.Version is (int)GO or >= (int)GP);
        BanEvo((int)Species.Weezing, 0, pk => pk.Version >= (int)SW);
        BanEvo((int)Species.MrMime, 0, pk => pk.Version >= (int)SW);

        foreach (var s in GetEvolutions((int)Species.Eevee, 0)) // Eeveelutions
            BanEvo(s, 0, pk => pk is IGigantamax {CanGigantamax: true});
    }

    private void FixEvoTreeBS()
    {
        BanEvo((int)Species.Glaceon, 0, pk => pk.CurrentLevel == pk.Met_Level); // Ice Stone is unreleased, requires Route 217 Ice Rock Level Up instead
        BanEvo((int)Species.Milotic, 0, pk => pk is IContestStats { CNT_Beauty: < 170 } || pk.CurrentLevel == pk.Met_Level); // Prism Scale is unreleased, requires 170 Beauty Level Up instead
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
    /// <param name="pk">Pokémon data to check with.</param>
    /// <param name="maxLevel">Maximum level to permit before the chain breaks.</param>
    /// <param name="maxSpeciesOrigin">Maximum species ID to permit within the chain.</param>
    /// <param name="skipChecks">Ignores an evolution's criteria, causing the returned list to have all possible evolutions.</param>
    /// <param name="minLevel">Minimum level to permit before the chain breaks.</param>
    public EvoCriteria[] GetValidPreEvolutions(PKM pk, byte maxLevel, int maxSpeciesOrigin = -1, bool skipChecks = false, byte minLevel = 1)
    {
        if (maxSpeciesOrigin <= 0)
            maxSpeciesOrigin = GetMaxSpeciesOrigin(pk);

        ushort species = (ushort)pk.Species;
        byte form = (byte)pk.Form;

        return GetExplicitLineage(species, form, pk, minLevel, maxLevel, maxSpeciesOrigin, skipChecks);
    }

    public bool IsSpeciesDerivedFrom(int species, int form, int otherSpecies, int otherForm, bool ignoreForm = true)
    {
        var evos = GetEvolutionsAndPreEvolutions(species, form);
        foreach (var evo in evos)
        {
            var s = evo & 0x3FF;
            if (s != otherSpecies)
                continue;
            if (ignoreForm)
                return true;
            var f = evo >> 11;
            return f == otherForm;
        }
        return false;
    }

    /// <summary>
    /// Gets all species the <see cref="species"/>-<see cref="form"/> can evolve to &amp; from, yielded in order of increasing evolution stage.
    /// </summary>
    /// <param name="species">Species ID</param>
    /// <param name="form">Form ID</param>
    /// <returns>Enumerable of species IDs (with the Form IDs included, left shifted by 11).</returns>
    public IEnumerable<int> GetEvolutionsAndPreEvolutions(int species, int form)
    {
        foreach (var s in GetPreEvolutions(species, form))
            yield return s;
        yield return species;
        foreach (var s in GetEvolutions(species, form))
            yield return s;
    }

    public int GetBaseSpeciesForm(int species, int form, int skip = 0)
    {
        var chain = GetEvolutionsAndPreEvolutions(species, form);
        foreach (var c in chain)
        {
            if (skip == 0)
                return c;
            skip--;
        }
        return species | (form << 11);
    }

    /// <summary>
    /// Gets all species the <see cref="species"/>-<see cref="form"/> can evolve from, yielded in order of increasing evolution stage.
    /// </summary>
    /// <param name="species">Species ID</param>
    /// <param name="form">Form ID</param>
    /// <returns>Enumerable of species IDs (with the Form IDs included, left shifted by 11).</returns>
    public IEnumerable<int> GetPreEvolutions(int species, int form)
    {
        int index = GetLookupKey(species, form);
        var node = Lineage[index];
        foreach (var method in node)
        {
            var s = method.Species;
            if (s == 0)
                continue;
            var f = method.Form;
            var preEvolutions = GetPreEvolutions(s, f);
            foreach (var preEvo in preEvolutions)
                yield return preEvo;
            yield return s | (f << 11);
        }
    }

    /// <summary>
    /// Gets all species the <see cref="species"/>-<see cref="form"/> can evolve to, yielded in order of increasing evolution stage.
    /// </summary>
    /// <param name="species">Species ID</param>
    /// <param name="form">Form ID</param>
    /// <returns>Enumerable of species IDs (with the Form IDs included, left shifted by 11).</returns>
    public IEnumerable<int> GetEvolutions(int species, int form)
    {
        int format = Game - Gen1 + 1;
        int index = format < 7 ? species : Personal.GetFormIndex(species, form);
        var evos = Entries[index];
        foreach (var method in evos)
        {
            var s = method.Species;
            if (s == 0)
                continue;
            var f = method.GetDestinationForm(form);
            yield return s | (f << 11);
            var nextEvolutions = GetEvolutions(s, f);
            foreach (var nextEvo in nextEvolutions)
                yield return nextEvo;
        }
    }

    /// <summary>
    /// Generates the reverse evolution path for the input <see cref="pk"/>.
    /// </summary>
    /// <param name="species">Entity Species to begin the chain</param>
    /// <param name="form">Entity Form to begin the chain</param>
    /// <param name="pk">Entity data</param>
    /// <param name="levelMin">Minimum level</param>
    /// <param name="levelMax">Maximum level</param>
    /// <param name="maxSpeciesID">Clamp for maximum species ID</param>
    /// <param name="skipChecks">Skip the secondary checks that validate the evolution</param>
    public EvoCriteria[] GetExplicitLineage(ushort species, byte form, PKM pk, byte levelMin, byte levelMax, int maxSpeciesID, bool skipChecks)
    {
        if (pk.IsEgg && !skipChecks)
        {
            return new[]
            {
                new EvoCriteria{ Species = species, Form = form, LevelMax = levelMax, LevelMin = levelMax },
            };
        }

        // Shedinja's evolution case can be a little tricky; hard-code handling.
        if (species == (int)Species.Shedinja && levelMax >= 20 && (!pk.HasOriginalMetLocation || levelMin < levelMax))
        {
            var min = Math.Max(levelMin, (byte)20);
            return new[]
            {
                new EvoCriteria { Species = (ushort)Species.Shedinja, LevelMax = levelMax, LevelMin = min, Method = EvolutionType.LevelUp },
                new EvoCriteria { Species = (ushort)Species.Nincada, LevelMax = levelMax, LevelMin = levelMin },
            };
        }
        return GetLineage(species, form, pk, levelMin, levelMax, maxSpeciesID, skipChecks);
    }

    private EvoCriteria[] GetLineage(int species, int form, PKM pk, byte levelMin, byte levelMax, int maxSpeciesID, bool skipChecks)
    {
        var lvl = levelMax;
        var first = new EvoCriteria { Species = (ushort)species, Form = (byte)form, LevelMax = lvl };

        const int maxEvolutions = 3;
        Span<EvoCriteria> evos = stackalloc EvoCriteria[maxEvolutions];
        evos[0] = first;

        switch (species)
        {
            case (int)Species.Silvally:
                form = 0;
                break;
        }

        // There aren't any circular evolution paths, and all lineages have at most 3 evolutions total.
        // There aren't any convergent evolution paths, so only yield the first connection.
        int ctr = 1;
        while (true)
        {
            var key = GetLookupKey(species, form);
            bool oneValid = false;
            var node = Lineage[key];

            foreach (var link in node)
            {
                if (link.IsEvolutionBanned(pk) && !skipChecks)
                    continue;

                var evo = link.Method;
                if (!evo.Valid(pk, lvl, skipChecks))
                    continue;

                if (evo.RequiresLevelUp && levelMin >= lvl)
                    break; // impossible evolution

                UpdateMinValues(evos[..ctr], evo, levelMin);

                species = link.Species;
                form = link.Form;
                evos[ctr++] = evo.GetEvoCriteria((ushort)species, (byte)form, lvl);
                if (evo.RequiresLevelUp)
                    lvl--;

                oneValid = true;
                break;
            }
            if (!oneValid)
                break;
        }

        // Remove future gen pre-evolutions; no Munchlax from a Gen3 Snorlax, no Pichu from a Gen1-only Raichu, etc
        ref var last = ref evos[ctr - 1];
        if (last.Species > maxSpeciesID)
        {
            for (int i = 0; i < ctr; i++)
            {
                if (evos[i].Species > maxSpeciesID)
                    continue;
                ctr--;
                break;
            }
        }

        // Last species is the wild/hatched species, the minimum level is because it has not evolved from previous species
        var result = evos[..ctr];
        last = ref result[^1];
        last = last with { LevelMin = levelMin, LevelUpRequired = 0 };

        // Rectify minimum levels
        RectifyMinimumLevels(result);

        return result.ToArray();
    }

    private static void RectifyMinimumLevels(Span<EvoCriteria> result)
    {
        for (int i = result.Length - 2; i >= 0; i--)
        {
            ref var evo = ref result[i];
            var prev = result[i + 1];
            var min = (byte)Math.Max(prev.LevelMin + evo.LevelUpRequired, evo.LevelMin);
            evo = evo with { LevelMin = min };
        }
    }

    private static void UpdateMinValues(Span<EvoCriteria> evos, EvolutionMethod evo, byte minLevel)
    {
        ref var last = ref evos[^1];
        if (!evo.RequiresLevelUp)
        {
            // Evolutions like elemental stones, trade, etc
            last = last with { LevelMin = minLevel };
            return;
        }
        if (evo.Level == 0)
        {
            // Friendship based Level Up Evolutions, Pichu -> Pikachu, Eevee -> Umbreon, etc
            last = last with { LevelMin = (byte)(minLevel + 1) };

            // Raichu from Pikachu would have a minimum level of 1; accounting for Pichu (level up required) results in a minimum level of 2
            if (evos.Length > 1)
            {
                ref var first = ref evos[0];
                if (!first.RequiresLvlUp)
                    first = first with { LevelMin = (byte)(minLevel + 1) };
            }
        }
        else // level up evolutions
        {
            last = last with { LevelMin = evo.Level };

            if (evos.Length > 1)
            {
                ref var first = ref evos[0];
                if (first.RequiresLvlUp)
                {
                    // Pokemon like Crobat, its minimum level is Golbat minimum level + 1
                    if (first.LevelMin <= evo.Level)
                        first = first with {LevelMin = (byte)(evo.Level + 1) };
                }
                else
                {
                    // Pokemon like Nidoqueen who evolve with an evolution stone, minimum level is prior evolution minimum level
                    if (first.LevelMin < evo.Level)
                        first = first with { LevelMin = evo.Level };
                }
            }
        }
        last = last with { LevelUpRequired = evo.RequiresLevelUp ? (byte)1 : (byte)0 };
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
        /// <param name="pk">Entity to check</param>
        /// <returns>True if banned, false if allowed.</returns>
        public bool IsEvolutionBanned(PKM pk) => IsBanned != null && IsBanned(pk);
    }
}
