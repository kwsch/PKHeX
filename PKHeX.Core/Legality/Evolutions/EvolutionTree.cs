using System;
using System.Collections.Generic;
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
    public static readonly EvolutionTree Evolves8a = new(GetReader("la"), PLA, PersonalTable.LA, MaxSpeciesID_8a);
    public static readonly EvolutionTree Evolves8b = new(GetReader("bs"), BDSP, PersonalTable.BDSP, MaxSpeciesID_8b);

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
    private readonly IPersonalTable Personal;
    private readonly int MaxSpeciesTree;
    private readonly EvolutionReverseLookup Lineage;
    private bool FormSeparateEvoData => MaxSpeciesTree > MaxSpeciesID_6;

    internal static int GetLookupKey(ushort species, byte form) => species | (form << 11);

    #region Constructor

    private EvolutionTree(ReadOnlySpan<byte> data, GameVersion game, IPersonalTable personal, int maxSpeciesTree)
    {
        Personal = personal;
        MaxSpeciesTree = maxSpeciesTree;
        Entries = GetEntries(data, game);
        var connections = CreateTreeOld();
        Lineage = new(connections, maxSpeciesTree);
    }

    private EvolutionTree(BinLinkerAccessor data, GameVersion game, IPersonalTable personal, int maxSpeciesTree)
    {
        Personal = personal;
        MaxSpeciesTree = maxSpeciesTree;
        Entries = GetEntries(data, game);

        // Starting in Generation 7, forms have separate evolution data.
        var oldStyle = !FormSeparateEvoData;
        var connections = oldStyle ? CreateTreeOld() : CreateTree();
        Lineage = new(connections, maxSpeciesTree);
    }

    private IEnumerable<(int Key, EvolutionLink Value)> CreateTreeOld()
    {
        for (ushort sSpecies = 1; sSpecies <= MaxSpeciesTree; sSpecies++)
        {
            var fc = Personal[sSpecies].FormCount;
            for (byte sForm = 0; sForm < fc; sForm++)
            {
                var index = sSpecies;
                var evos = Entries[index];
                foreach (var evo in evos)
                {
                    var dSpecies = evo.Species;
                    if (dSpecies == 0)
                        continue;

                    var dForm = sSpecies == (int)Species.Espurr && evo.Method == EvolutionType.LevelUpFormFemale1 ? (byte)1 : sForm;
                    var key = GetLookupKey(dSpecies, dForm);

                    var link = new EvolutionLink(sSpecies, sForm, evo);
                    yield return (key, link);
                }
            }
        }
    }

    private IEnumerable<(int Key, EvolutionLink Value)> CreateTree()
    {
        for (ushort sSpecies = 1; sSpecies <= MaxSpeciesTree; sSpecies++)
        {
            var fc = Personal[sSpecies].FormCount;
            for (byte sForm = 0; sForm < fc; sForm++)
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
                    yield return (key, link);
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
        Gen7 or Gen8 or BDSP => EvolutionSet7.GetArray(data, false),
        PLA => EvolutionSet7.GetArray(data, true),
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
            BanEvo(s.Species, s.Form, pk => pk is IGigantamax {CanGigantamax: true});
    }

    private void FixEvoTreeBS()
    {
        BanEvo((int)Species.Glaceon, 0, pk => pk.CurrentLevel == pk.Met_Level); // Ice Stone is unreleased, requires Route 217 Ice Rock Level Up instead
        BanEvo((int)Species.Milotic, 0, pk => pk is IContestStats { CNT_Beauty: < 170 } || pk.CurrentLevel == pk.Met_Level); // Prism Scale is unreleased, requires 170 Beauty Level Up instead
    }

    private void BanEvo(ushort species, byte form, Func<PKM, bool> func)
    {
        var key = GetLookupKey(species, form);
        ref var node = ref Lineage[key];
        node.Ban(func);
    }

    #endregion

    /// <summary>
    /// Gets a list of evolutions for the input <see cref="PKM"/> by checking each evolution in the chain.
    /// </summary>
    /// <param name="pk">Pokémon data to check with.</param>
    /// <param name="levelMax">Maximum level to permit before the chain breaks.</param>
    /// <param name="maxSpeciesOrigin">Maximum species ID to permit within the chain.</param>
    /// <param name="skipChecks">Ignores an evolution's criteria, causing the returned list to have all possible evolutions.</param>
    /// <param name="levelMin">Minimum level to permit before the chain breaks.</param>
    /// <param name="stopSpecies">Final species to stop at, if known</param>
    public EvoCriteria[] GetValidPreEvolutions(PKM pk, byte levelMax, int maxSpeciesOrigin = -1, bool skipChecks = false, byte levelMin = 1, int stopSpecies = 0)
    {
        if (maxSpeciesOrigin <= 0)
            maxSpeciesOrigin = GetMaxSpeciesOrigin(pk);

        ushort species = pk.Species;
        byte form = pk.Form;

        return GetExplicitLineage(species, form, pk, levelMin, levelMax, maxSpeciesOrigin, skipChecks, stopSpecies);
    }

    public bool IsSpeciesDerivedFrom(ushort species, byte form, int otherSpecies, int otherForm, bool ignoreForm = true)
    {
        var evos = GetEvolutionsAndPreEvolutions(species, form);
        foreach (var (s, f) in evos)
        {
            if (s != otherSpecies)
                continue;
            if (ignoreForm)
                return true;
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
    public IEnumerable<(ushort Species, byte Form)> GetEvolutionsAndPreEvolutions(ushort species, byte form)
    {
        foreach (var s in GetPreEvolutions(species, form))
            yield return s;
        yield return (species, form);
        foreach (var s in GetEvolutions(species, form))
            yield return s;
    }

    public (ushort Species, byte Form) GetBaseSpeciesForm(ushort species, byte form, int skip = 0)
    {
        var chain = GetEvolutionsAndPreEvolutions(species, form);
        foreach (var c in chain)
        {
            if (skip == 0)
                return c;
            skip--;
        }
        return (species, form);
    }

    /// <summary>
    /// Gets all species the <see cref="species"/>-<see cref="form"/> can evolve from, yielded in order of increasing evolution stage.
    /// </summary>
    /// <param name="species">Species ID</param>
    /// <param name="form">Form ID</param>
    /// <returns>Enumerable of species IDs (with the Form IDs included, left shifted by 11).</returns>
    public IEnumerable<(ushort Species, byte Form)> GetPreEvolutions(ushort species, byte form)
    {
        int index = GetLookupKey(species, form);
        var node = Lineage[index];
        {
            // No convergent evolutions; first method is enough.
            var s = node.First.Tuple;
            if (s.Species == 0)
                yield break;

            var preEvolutions = GetPreEvolutions(s.Species, s.Form);
            foreach (var preEvo in preEvolutions)
                yield return preEvo;
            yield return s;
        }
    }

    /// <summary>
    /// Gets all species the <see cref="species"/>-<see cref="form"/> can evolve to, yielded in order of increasing evolution stage.
    /// </summary>
    /// <param name="species">Species ID</param>
    /// <param name="form">Form ID</param>
    /// <returns>Enumerable of species IDs (with the Form IDs included, left shifted by 11).</returns>
    public IEnumerable<(ushort Species, byte Form)> GetEvolutions(ushort species, byte form)
    {
        int index = !FormSeparateEvoData ? species : Personal.GetFormIndex(species, form);
        var evos = Entries[index];
        foreach (var method in evos)
        {
            var s = method.Species;
            if (s == 0)
                continue;
            var f = method.GetDestinationForm(form);
            yield return (s, f);
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
    /// <param name="stopSpecies">Final species to stop at, if known</param>
    public EvoCriteria[] GetExplicitLineage(ushort species, byte form, PKM pk, byte levelMin, byte levelMax, int maxSpeciesID, bool skipChecks, int stopSpecies)
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
        return Lineage.Reverse(species, form, pk, levelMin, levelMax, maxSpeciesID, skipChecks, stopSpecies);
    }
}
