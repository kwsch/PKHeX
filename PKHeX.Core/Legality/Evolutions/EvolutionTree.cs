using System;
using static PKHeX.Core.GameVersion;

namespace PKHeX.Core;

/// <summary>
/// Generation specific Evolution Tree data.
/// </summary>
/// <remarks>
/// Used to determine if a <see cref="PKM.Species"/> can evolve from prior steps in its evolution branch.
/// </remarks>
public sealed class EvolutionTree : EvolutionNetwork
{
    public const int MaxEvolutions = 3;
    public static readonly EvolutionTree Evolves1  = GetViaSpecies (PersonalTable.Y,    EvolutionSet1.GetArray(GetResource("rby"), 151));
    public static readonly EvolutionTree Evolves2  = GetViaSpecies (PersonalTable.C,    EvolutionSet1.GetArray(GetResource("gsc"), 251));
    public static readonly EvolutionTree Evolves3  = GetViaSpecies (PersonalTable.RS,   EvolutionSet3.GetArray(GetResource("g3")));
    public static readonly EvolutionTree Evolves4  = GetViaSpecies (PersonalTable.DP,   EvolutionSet4.GetArray(GetResource("g4")));
    public static readonly EvolutionTree Evolves5  = GetViaSpecies (PersonalTable.BW,   EvolutionSet5.GetArray(GetResource("g5")));
    public static readonly EvolutionTree Evolves6  = GetViaSpecies (PersonalTable.AO,   EvolutionSet6.GetArray(GetReader("ao")));
    public static readonly EvolutionTree Evolves7  = GetViaPersonal(PersonalTable.USUM, EvolutionSet7.GetArray(GetReader("uu"), true));
    public static readonly EvolutionTree Evolves7b = GetViaPersonal(PersonalTable.GG,   EvolutionSet7.GetArray(GetReader("gg"), false));
    public static readonly EvolutionTree Evolves8  = GetViaPersonal(PersonalTable.SWSH, EvolutionSet7.GetArray(GetReader("ss"), false));
    public static readonly EvolutionTree Evolves8a = GetViaPersonal(PersonalTable.LA,   EvolutionSet7.GetArray(GetReader("la"), true));
    public static readonly EvolutionTree Evolves8b = GetViaPersonal(PersonalTable.BDSP, EvolutionSet7.GetArray(GetReader("bs"), false));
    public static readonly EvolutionTree Evolves9  = GetViaPersonal(PersonalTable.SV,   EvolutionSet7.GetArray(GetReader("sv"), false));

    private static EvolutionTree GetViaSpecies(IPersonalTable t, EvolutionMethod[][] entries)
    {
        var forward = new EvolutionForwardSpecies(entries);
        var reverse = new EvolutionReverseSpecies(t, entries, t.MaxSpeciesID);
        return new EvolutionTree(forward, reverse);
    }

    private static EvolutionTree GetViaPersonal(IPersonalTable t, EvolutionMethod[][] entries)
    {
        var forward = new EvolutionForwardPersonal(entries, t);
        var reverse = new EvolutionReversePersonal(t, entries, t.MaxSpeciesID);
        return new EvolutionTree(forward, reverse);
    }

    private EvolutionTree(IEvolutionForward forward, IEvolutionReverse reverse) : base(forward, reverse)
    {
    }

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
        EntityContext.Gen9 => Evolves9,
        EntityContext.Gen7b => Evolves7b,
        EntityContext.Gen8a => Evolves8a,
        EntityContext.Gen8b => Evolves8b,
        _ => throw new ArgumentOutOfRangeException(nameof(context), context, null),
    };

    internal static int GetLookupKey(ushort species, byte form) => species | (form << 11);

    private void FixEvoTreeSM()
    {
        // Sun/Moon lack Ultra's Kantonian evolution methods.
        static bool BanOnlySM(PKM pk) => pk is { IsUntraded: true, SM: true };
        BanEvo((int)Species.Raichu, 0, BanOnlySM);
        BanEvo((int)Species.Marowak, 0, BanOnlySM);
        BanEvo((int)Species.Exeggutor, 0, BanOnlySM);
    }

    private void FixEvoTreeSS()
    {
        // Gigantamax Pikachu, Meowth-0, and Eevee are prevented from evolving.
        // Raichu cannot be evolved to the Alolan variant at this time.
        static bool BanGmax(PKM pk) => pk is IGigantamax { CanGigantamax: true };
        BanEvo((int)Species.Raichu, 0, BanGmax);
        BanEvo((int)Species.Raichu, 1, pk => (pk is IGigantamax {CanGigantamax: true}) || pk.Version is (int)GO or >= (int)GP);
        BanEvo((int)Species.Persian, 0, BanGmax);
        BanEvo((int)Species.Persian, 1, BanGmax);
        BanEvo((int)Species.Perrserker, 0, BanGmax);

        BanEvo((int)Species.Exeggutor, 1, pk => pk.Version is (int)GO or >= (int)GP);
        BanEvo((int)Species.Marowak, 1, pk => pk.Version is (int)GO or >= (int)GP);
        BanEvo((int)Species.Weezing, 0, pk => pk.Version >= (int)SW);
        BanEvo((int)Species.MrMime, 0, pk => pk.Version >= (int)SW);

        foreach (var s in GetEvolutions((int)Species.Eevee, 0)) // Eeveelutions
            BanEvo(s.Species, s.Form, BanGmax);
    }

    private void FixEvoTreeBS()
    {
        BanEvo((int)Species.Glaceon, 0, pk => pk.CurrentLevel == pk.Met_Level); // Ice Stone is unreleased, requires Route 217 Ice Rock Level Up instead
        BanEvo((int)Species.Milotic, 0, pk => pk is IContestStatsReadOnly { CNT_Beauty: < 170 } || pk.CurrentLevel == pk.Met_Level); // Prism Scale is unreleased, requires 170 Beauty Level Up instead
    }

    /// <summary>
    /// Gets a list of evolutions for the input <see cref="PKM"/> by checking each evolution in the chain.
    /// </summary>
    /// <param name="pk">Pokémon data to check with.</param>
    /// <param name="levelMax">Maximum level to permit before the chain breaks.</param>
    /// <param name="skipChecks">Ignores an evolution's criteria, causing the returned list to have all possible evolutions.</param>
    /// <param name="levelMin">Minimum level to permit before the chain breaks.</param>
    /// <param name="stopSpecies">Final species to stop at, if known</param>
    public EvoCriteria[] GetValidPreEvolutions(PKM pk, byte levelMax, bool skipChecks = false, byte levelMin = 1, ushort stopSpecies = 0)
    {
        ushort species = pk.Species;
        byte form = pk.Form;

        Span<EvoCriteria> result = stackalloc EvoCriteria[MaxEvolutions];
        var count = GetExplicitLineage(result, species, form, pk, levelMin, levelMax, stopSpecies, skipChecks);
        return result[..count].ToArray();
    }

    /// <summary>
    /// Generates the reverse evolution path for the input <see cref="pk"/>.
    /// </summary>
    /// <param name="result">Result storage</param>
    /// <param name="species">Entity Species to begin the chain</param>
    /// <param name="form">Entity Form to begin the chain</param>
    /// <param name="pk">Entity data</param>
    /// <param name="levelMin">Minimum level</param>
    /// <param name="levelMax">Maximum level</param>
    /// <param name="skipChecks">Skip the secondary checks that validate the evolution</param>
    /// <param name="stopSpecies">Final species to stop at, if known</param>
    /// <returns>Count of entries filled.</returns>
    public int GetExplicitLineage(Span<EvoCriteria> result, ushort species, byte form, PKM pk, byte levelMin, byte levelMax, ushort stopSpecies, bool skipChecks)
    {
        if (pk.IsEgg && !skipChecks)
        {
            result[0] = new EvoCriteria { Species = species, Form = form, LevelMax = levelMax, LevelMin = levelMax };
            return 1;
        }

        // Shedinja's evolution case can be a little tricky; hard-code handling.
        if (species == (int)Species.Shedinja && levelMax >= 20 && (!pk.HasOriginalMetLocation || levelMin < levelMax))
        {
            var min = Math.Max(levelMin, (byte)20);
            result[0] = new EvoCriteria { Species = (ushort)Species.Shedinja, LevelMax = levelMax, LevelMin = min, Method = EvolutionType.LevelUp };
            result[1] = new EvoCriteria { Species = (ushort)Species.Nincada, LevelMax = levelMax, LevelMin = levelMin };
            return 2;
        }
        return Reverse.Lineage.Reverse(result, species, form, pk, levelMin, levelMax, stopSpecies, skipChecks);
    }
}
